/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP.Node.Logging;
using cloud.charging.open.protocols.OCPP.Node.Connections;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving
{

    /// <summary>
    /// Which charging stations may sign in to this local controller, with
    /// what, and under which group's rules.
    /// </summary>
    /// <remarks>
    /// <b>Its own file, with its own permissions.</b> These are credentials,
    /// and the configuration file beside this one is an ordinary file that
    /// anybody who can read the directory may read. So they live here, and the
    /// file is written for its owner alone. Passwords are written the way the
    /// web login is written - PBKDF2 over a random salt - and a password that
    /// has been set is not readable again, not by this controller and not by
    /// whoever set it. A TOTP shared secret is the one thing here that cannot
    /// be hashed, because verifying a token means computing it.
    ///
    /// <b>A list, not a door.</b> A charging station that is not in this file
    /// cannot sign in with a password or a token, whatever it calls itself.
    /// Adding one is a deliberate act, which is the only way the list means
    /// anything.
    ///
    /// <b>Groups decide, logins belong.</b> What a station may do is said by
    /// the group it is in, so that it is said once for a site rather than
    /// forty times. See <see cref="LoginGroup"/>.
    /// </remarks>
    public sealed class ChargingStationLogins
    {

        #region Data

        /// <summary>
        /// The file beside the process, when nobody says otherwise.
        /// </summary>
        public const String  DefaultFileName    = "ocpp-stations.json";

        /// <summary>
        /// The longest a charging station identification may be; what OCPP 2.1
        /// allows for one.
        /// </summary>
        public const Int32   MaxIdLength        = 48;

        /// <summary>
        /// The longest a note beside a station may be.
        /// </summary>
        public const Int32   MaxNoteLength      = 200;

        /// <summary>
        /// The shortest password that may be set by hand. OCPP asks for at
        /// least 16 characters of entropy in the password of a charging
        /// station, and a controller that accepted "1234" would be the weakest
        /// part of a system built around that rule.
        /// </summary>
        public const Int32   MinPasswordLength  = 16;

        /// <summary>
        /// The longest one may be; what OCPP allows for the field.
        /// </summary>
        public const Int32   MaxPasswordLength  = 64;

        /// <summary>
        /// How many charging stations may be listed.
        /// </summary>
        public const Int32   MaxStations        = 1000;

        /// <summary>
        /// Something to hash when there is nothing to compare against; see
        /// <see cref="Verify"/>.
        /// </summary>
        private const String TimingEqualiser       = "no such charging station";

        /// <summary>
        /// Something to derive a token from when there is nothing to compare
        /// against; see <see cref="VerifyTOTP"/>.
        /// </summary>
        private const String TOTPTimingEqualiser   = "no-such-charging-station-at-all";

        private readonly Object                                    updateLock  = new ();
        private readonly Dictionary<String, ChargingStationLogin>  logins      = [];
        private readonly Dictionary<String, LoginGroup>            groups      = [];

        /// <summary>
        /// Every key pin that was listed for a station at any time since this
        /// store was made - see <see cref="WasEverListed"/>.
        /// </summary>
        private readonly HashSet<KeyPin>                           pinsEverListed  = [];

        #endregion

        #region Properties

        /// <summary>
        /// The file the logins live in.
        /// </summary>
        public String   Path     { get; }

        /// <summary>
        /// Whether the file exists.
        /// </summary>
        public Boolean  Exists
            => File.Exists(Path);

        /// <summary>
        /// The clock this controller reads.
        /// </summary>
        public TimeProvider  TimeProvider  { get; }

        /// <summary>
        /// Every charging station in the file, by identification.
        /// </summary>
        public IReadOnlyList<ChargingStationLogin> Logins
        {
            get
            {
                lock (updateLock)
                    return [.. logins.Values.OrderBy(login => login.Id, StringComparer.OrdinalIgnoreCase)];
            }
        }

        /// <summary>
        /// Every group, the built-in one first and the rest by name.
        /// </summary>
        public IReadOnlyList<LoginGroup> Groups
        {
            get
            {
                lock (updateLock)
                    return [.. groups.Values.OrderByDescending(group => group.IsBuiltIn).
                                             ThenBy          (group => group.Id, StringComparer.OrdinalIgnoreCase)];
            }
        }

        /// <summary>
        /// How many of them may sign in at the moment.
        /// </summary>
        /// <remarks>
        /// A station in a group that is switched off may not, however switched
        /// on the station itself is - so the group has its say here too, and
        /// the number on the page is the number that could actually come in.
        /// </remarks>
        public Int32 EnabledCount
        {
            get
            {
                lock (updateLock)
                    return logins.Values.Count(login => login.Enabled &&
                                                        groups.GetValueOrDefault(login.GroupId)?.Enabled == true);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Something happened here that belongs in the log of the controller.
        /// </summary>
        public event Action<LogLevel, String>? OnNotice;

        /// <summary>
        /// The list changed, and whatever is serving charging stations has to
        /// be told - a station whose password was taken away must stop being
        /// able to use it now and not at the next start.
        /// </summary>
        public event Action? OnChanged;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The logins in the given file; it need not exist yet.
        /// </summary>
        public ChargingStationLogins(String?        Path          = null,
                                     TimeProvider?  TimeProvider  = null)
        {

            this.Path          = System.IO.Path.GetFullPath(Path ?? DefaultFileName);
            this.TimeProvider  = TimeProvider ?? System.TimeProvider.System;

            // Before anything is read, so that a controller with no file at all
            // still has the group every login falls back to.
            this.groups[LoginGroup.DefaultId] = LoginGroup.Default(this.TimeProvider.GetUtcNow());

        }

        #endregion


        #region TryLoad(out Error)

        /// <summary>
        /// Read the file. No file is not a failure - it is a local controller
        /// that has not been told about any charging station yet.
        /// </summary>
        /// <remarks>
        /// A file that is there but unreadable <em>is</em> a failure, and a loud
        /// one: carrying on with an empty list would turn every charging
        /// station on the site away while looking like nothing was wrong.
        ///
        /// A login pointing at a group that is not in the file is a failure of
        /// the same kind, and deliberately not repaired by moving it to the
        /// default group: that group is the permissive one, so the repair would
        /// quietly hand a station more than whoever wrote the file meant it to
        /// have.
        /// </remarks>
        public Boolean TryLoad([NotNullWhen(false)] out String? Error)
        {

            Error = null;

            lock (updateLock)
            {

                logins.Clear();
                groups.Clear();
                groups[LoginGroup.DefaultId] = LoginGroup.Default(TimeProvider.GetUtcNow());

                if (!File.Exists(Path))
                    return true;

                JObject document;

                try
                {

                    var text = File.ReadAllText(Path);

                    if (String.IsNullOrWhiteSpace(text))
                        return true;

                    document = JObject.Parse(text);

                }
                catch (Exception e)
                {
                    Error = $"'{Path}' could not be read: {e.Message}";
                    return false;
                }

                #region The groups, when the file knows about any

                if (document["groups"] is JToken groupsToken && groupsToken.Type != JTokenType.Null)
                {

                    if (groupsToken is not JArray groupArray)
                    {
                        Error = $"'{Path}': 'groups' must be a JSON array.";
                        return false;
                    }

                    foreach (var token in groupArray)
                    {

                        if (token is not JObject json)
                        {
                            Error = $"'{Path}': every entry of 'groups' must be a JSON object.";
                            return false;
                        }

                        if (!LoginGroup.TryParse(json, out var group, out var problem))
                        {
                            Error = $"'{Path}': {problem}";
                            return false;
                        }

                        groups[group.Id] = group;

                    }

                }

                #endregion

                #region The logins

                if (document["stations"] is not JArray stations)
                {
                    Error = $"'{Path}' must hold a 'stations' array.";
                    return false;
                }

                foreach (var token in stations)
                {

                    if (token is not JObject json)
                    {
                        Error = $"'{Path}': every entry of 'stations' must be a JSON object.";
                        return false;
                    }

                    if (!ChargingStationLogin.TryParse(json, out var login, out var problem))
                    {
                        Error = $"'{Path}': {problem}";
                        return false;
                    }

                    if (!groups.ContainsKey(login.GroupId))
                    {
                        Error = $"'{Path}': the charging station '{login.Id}' belongs to the group '{login.GroupId}', which is not in this file.";
                        return false;
                    }

                    // One key is one charging station, written by hand or not:
                    // a key two stations share lets either sign in as the other.
                    foreach (var key in login.Keys)
                        if (logins.Values.FirstOrDefault(other => other.Keys.Any(known => known.Fingerprint == key.Fingerprint)) is ChargingStationLogin holder)
                        {
                            Error = $"'{Path}': the SSH key {key.Fingerprint} is listed for the charging stations '{holder.Id}' and '{login.Id}'; one key is one charging station.";
                            return false;
                        }

                    // The same for a key pin, and for a sharper reason: the pin
                    // is the whole of who the station is, so a pin two stations
                    // share would let the one connect as the other on its path.
                    foreach (var keyPin in login.Pins)
                        if (logins.Values.FirstOrDefault(other => other.FindKeyPin(keyPin.Pin) is not null) is ChargingStationLogin holder)
                        {
                            Error = $"'{Path}': the key pin {keyPin.Pin} is listed for the charging stations '{holder.Id}' and '{login.Id}'; one key is one charging station.";
                            return false;
                        }

                    logins[login.Id] = login;

                }

                #endregion

                foreach (var login in logins.Values)
                    foreach (var keyPin in login.Pins)
                        pinsEverListed.Add(keyPin.Pin);

            }

            return true;

        }

        #endregion


        #region TryAddOrUpdateGroup(Id, Name, Enabled, AuthMethods, SecurityProfiles, Note, out Error [, out NotSaved])

        /// <summary>
        /// Make a group, or change what one allows.
        /// </summary>
        public Boolean TryAddOrUpdateGroup(String?                           Id,
                                           String?                           Name,
                                           Boolean                           Enabled,
                                           IEnumerable<AuthMethod>?          AuthMethods,
                                           IEnumerable<Byte>?                SecurityProfiles,
                                           String?                           Note,
                                           [NotNullWhen(false)] out String?  Error)

            => TryAddOrUpdateGroup(Id, Name, Enabled, AuthMethods, SecurityProfiles, Note, out Error, out _);

        /// <summary>
        /// Make a group, or change what one allows - and say whether a refusal
        /// was the file's rather than the change's.
        /// </summary>
        /// <param name="NotSaved">True where the file could not be written: see <see cref="TryRemove(String, out String?, out Boolean)"/>.</param>
        public Boolean TryAddOrUpdateGroup(String?                           Id,
                                           String?                           Name,
                                           Boolean                           Enabled,
                                           IEnumerable<AuthMethod>?          AuthMethods,
                                           IEnumerable<Byte>?                SecurityProfiles,
                                           String?                           Note,
                                           [NotNullWhen(false)] out String?  Error,
                                           out Boolean                       NotSaved)

            => TryAddOrUpdateGroup(Id, Name, Enabled, AuthMethods, SecurityProfiles, Note, null, out Error, out NotSaved);

        /// <summary>
        /// Make a group, or change what one allows - and what its members
        /// have over their SSH connection besides OCPP.
        /// </summary>
        /// <param name="SSHChannels">Of <see cref="LoginGroup.KnownSSHChannels"/>; null to leave what the group gives as it is, which for a new group is nothing.</param>
        /// <param name="NotSaved">True where the file could not be written: see <see cref="TryRemove(String, out String?, out Boolean)"/>.</param>
        public Boolean TryAddOrUpdateGroup(String?                           Id,
                                           String?                           Name,
                                           Boolean                           Enabled,
                                           IEnumerable<AuthMethod>?          AuthMethods,
                                           IEnumerable<Byte>?                SecurityProfiles,
                                           String?                           Note,
                                           IEnumerable<String>?              SSHChannels,
                                           [NotNullWhen(false)] out String?  Error,
                                           out Boolean                       NotSaved)
        {

            NotSaved = false;

            lock (updateLock)
            {

                // Normalised the same way TryCreate will normalise it, so that
                // "Field-Test" and "field-test" are the same group rather than
                // a second one that shadows the first.
                var existing = groups.GetValueOrDefault(Id?.Trim().ToLowerInvariant() ?? "");

                if (!LoginGroup.TryCreate(Id,
                                          Name,
                                          Enabled,
                                          AuthMethods,
                                          SecurityProfiles,
                                          Note,
                                          existing?.AddedAt ?? TimeProvider.GetUtcNow(),
                                          out var group,
                                          out Error))
                {
                    return false;
                }

                if (SSHChannels is null)
                    group = group with { SSHChannels = existing?.SSHChannels };

                else
                {

                    if (!LoginGroup.TryChannels(SSHChannels, out var channels, out Error))
                        return false;

                    group = group with { SSHChannels = channels };

                }

                if (existing is null && groups.Count >= LoginGroup.MaxGroups)
                {
                    Error = $"At most {LoginGroup.MaxGroups} login groups may be listed here.";
                    return false;
                }

                groups[group.Id] = group;

                if (!TrySave(out Error))
                {

                    NotSaved = true;

                    if (existing is not null)
                        groups[group.Id] = existing;
                    else
                        groups.Remove(group.Id);

                    return false;

                }

                OnNotice?.Invoke(
                    LogLevel.Notice,
                    existing is null
                        ? $"The login group '{group.Id}' was added: {group}."
                        : $"The login group '{group.Id}' was changed: {group}."
                );

            }

            OnChanged?.Invoke();

            return true;

        }

        #endregion

        #region TryRemoveGroup(Id, out Error [, out NotSaved])

        /// <summary>
        /// Forget a group nobody is in.
        /// </summary>
        /// <remarks>
        /// A group with members is not removed and its members are not moved
        /// to the default group behind somebody's back: that group allows more
        /// than most, so the tidying would be a quiet widening. Whoever wants
        /// the group gone moves its members first, and sees where they land.
        /// </remarks>
        public Boolean TryRemoveGroup(String                            Id,
                                      [NotNullWhen(false)] out String?  Error)

            => TryRemoveGroup(Id, out Error, out _);

        /// <summary>
        /// Forget a group nobody is in - and say whether a refusal was the
        /// file's rather than the change's.
        /// </summary>
        /// <param name="NotSaved">True where the file could not be written: see <see cref="TryRemove(String, out String?, out Boolean)"/>.</param>
        public Boolean TryRemoveGroup(String                            Id,
                                      [NotNullWhen(false)] out String?  Error,
                                      out Boolean                       NotSaved)
        {

            Error     = null;
            NotSaved  = false;

            var id = Id?.Trim().ToLowerInvariant() ?? "";

            lock (updateLock)
            {

                if (!groups.TryGetValue(id, out var group))
                {
                    Error = $"This {NodeWording.Noun} has never heard of a login group called '{Id}'.";
                    return false;
                }

                if (group.IsBuiltIn)
                {
                    Error = "The default login group cannot be removed; it is what a login without a group of its own belongs to.";
                    return false;
                }

                var members = logins.Values.Count(login => login.GroupId == id);

                if (members > 0)
                {
                    Error = $"The login group '{id}' still holds {members} charging station(s). Move them to another group first.";
                    return false;
                }

                groups.Remove(id);

                if (!TrySave(out Error))
                {
                    NotSaved    = true;
                    groups[id]  = group;
                    return false;
                }

                OnNotice?.Invoke(LogLevel.Notice, $"The login group '{id}' was removed.");

            }

            OnChanged?.Invoke();

            return true;

        }

        #endregion

        #region TrySetGroup(Id, GroupId, out Error [, out NotSaved])

        /// <summary>
        /// Move a charging station into another group.
        /// </summary>
        public Boolean TrySetGroup(String                            Id,
                                   String?                           GroupId,
                                   [NotNullWhen(false)] out String?  Error)

            => TrySetGroup(Id, GroupId, out Error, out _);

        /// <summary>
        /// Move a charging station into another group - and say whether a
        /// refusal was the file's rather than the change's.
        /// </summary>
        /// <param name="NotSaved">True where the file could not be written: see <see cref="TryRemove(String, out String?, out Boolean)"/>.</param>
        public Boolean TrySetGroup(String                            Id,
                                   String?                           GroupId,
                                   [NotNullWhen(false)] out String?  Error,
                                   out Boolean                       NotSaved)
        {

            Error     = null;
            NotSaved  = false;

            var groupId = GroupId?.Trim().ToLowerInvariant() ?? "";

            if (groupId.Length == 0)
                groupId = LoginGroup.DefaultId;

            lock (updateLock)
            {

                if (!logins.TryGetValue(Id, out var login))
                {
                    Error = $"This {NodeWording.Noun} has never heard of a charging station called '{Id}'.";
                    return false;
                }

                if (!groups.ContainsKey(groupId))
                {
                    Error = $"This {NodeWording.Noun} has never heard of a login group called '{GroupId}'.";
                    return false;
                }

                if (login.GroupId == groupId)
                    return true;

                logins[Id] = login with { GroupId = groupId };

                if (!TrySave(out Error))
                {
                    NotSaved    = true;
                    logins[Id]  = login;
                    return false;
                }

                OnNotice?.Invoke(LogLevel.Notice, $"The charging station '{Id}' is now in the login group '{groupId}'.");

            }

            OnChanged?.Invoke();

            return true;

        }

        #endregion


        #region TrySetPassword(Id, Password, GroupId, Note, out Generated, out Error [, out NotSaved])

        /// <summary>
        /// Add a charging station, or give one that is already here a new
        /// password.
        /// </summary>
        /// <remarks>
        /// A password of null means "make one up", and the made-up one comes
        /// back here once - to be shown to whoever is standing in front of the
        /// charging station about to type it in. It is kept nowhere but in its
        /// hash, so a second look is a second password.
        /// </remarks>
        /// <param name="Id">What the charging station calls itself in OCPP.</param>
        /// <param name="Password">The password, or null to have one made up.</param>
        /// <param name="GroupId">The group it should belong to, or null for the one it is in already.</param>
        /// <param name="Note">What this station is, for whoever reads the list later.</param>
        /// <param name="Generated">The password, when it was made up here.</param>
        public Boolean TrySetPassword(String                            Id,
                                      String?                           Password,
                                      String?                           GroupId,
                                      String?                           Note,
                                      out String?                       Generated,
                                      [NotNullWhen(false)] out String?  Error)

            => TrySetPassword(Id, Password, GroupId, Note, out Generated, out Error, out _);

        /// <summary>
        /// Add a charging station, or give one that is already here a new
        /// password - and say whether a refusal was the file's rather than the
        /// change's.
        /// </summary>
        /// <param name="NotSaved">True where the file could not be written: see <see cref="TryRemove(String, out String?, out Boolean)"/>.</param>
        public Boolean TrySetPassword(String                            Id,
                                      String?                           Password,
                                      String?                           GroupId,
                                      String?                           Note,
                                      out String?                       Generated,
                                      [NotNullWhen(false)] out String?  Error,
                                      out Boolean                       NotSaved)
        {

            Generated  = null;
            Error      = null;
            NotSaved   = false;

            if (!TryCheckIdAndNote(Id, Note, out var id, out var note, out Error))
                return false;

            var password = Password?.Trim();

            if (password is null || password.Length == 0)
            {
                // 32 characters of Base64Url: what OCPP asks for, and typable
                // off a screen by somebody standing at a charging station.
                password   = Convert.ToBase64String(RandomNumberGenerator.GetBytes(24)).
                                     Replace('+', '-').
                                     Replace('/', '_').
                                     TrimEnd('=');
                Generated  = password;
            }

            else if (password.Length < MinPasswordLength)
            {
                Error = $"A charging station password must be at least {MinPasswordLength} characters long; OCPP asks for that much. " +
                         "Leave it empty to have one made up.";
                return false;
            }

            else if (password.Length > MaxPasswordLength)
            {
                Error = $"A charging station password may be at most {MaxPasswordLength} characters long.";
                return false;
            }

            if (!TryChange(id,
                           note,
                           GroupId,
                           login => login with { Password = SecurePassword.Create(password) },
                           existed => existed
                                          ? $"The password of the charging station '{id}' was changed."
                                          : $"The charging station '{id}' may now sign in to this {NodeWording.Noun}.",
                           out Error,
                           out NotSaved))
            {
                Generated = null;
                return false;
            }

            return true;

        }

        #endregion

        #region TrySetTOTP(Id, SharedSecret, ValidityTime, Length, Alphabet, HashAlgorithm, GroupId, Note, out Generated, out Error [, out NotSaved])

        /// <summary>
        /// Add a charging station, or give one that is already here what it
        /// needs to be let in with a one-time token.
        /// </summary>
        /// <remarks>
        /// A shared secret of null means "make one up", and unlike a password
        /// the made-up one is not the only time it can be read: the secret has
        /// to stay readable for the controller to compute tokens from, so it
        /// can be looked at again by whoever may manage certificates. It still
        /// comes back here once, because that is the moment somebody is
        /// standing in front of the charging station.
        /// </remarks>
        /// <param name="Generated">The shared secret, when it was made up here.</param>
        public Boolean TrySetTOTP(String                            Id,
                                  String?                           SharedSecret,
                                  TimeSpan?                         ValidityTime,
                                  UInt32?                           Length,
                                  String?                           Alphabet,
                                  TOTPHashAlgorithm?                HashAlgorithm,
                                  String?                           GroupId,
                                  String?                           Note,
                                  out String?                       Generated,
                                  [NotNullWhen(false)] out String?  Error)

            => TrySetTOTP(Id, SharedSecret, ValidityTime, Length, Alphabet, HashAlgorithm, GroupId, Note, out Generated, out Error, out _);

        /// <summary>
        /// Add a charging station, or give one that is already here what it
        /// needs to be let in with a one-time token - and say whether a refusal
        /// was the file's rather than the change's.
        /// </summary>
        /// <param name="NotSaved">True where the file could not be written: see <see cref="TryRemove(String, out String?, out Boolean)"/>.</param>
        public Boolean TrySetTOTP(String                            Id,
                                  String?                           SharedSecret,
                                  TimeSpan?                         ValidityTime,
                                  UInt32?                           Length,
                                  String?                           Alphabet,
                                  TOTPHashAlgorithm?                HashAlgorithm,
                                  String?                           GroupId,
                                  String?                           Note,
                                  out String?                       Generated,
                                  [NotNullWhen(false)] out String?  Error,
                                  out Boolean                       NotSaved)
        {

            Generated  = null;
            Error      = null;
            NotSaved   = false;

            if (!TryCheckIdAndNote(Id, Note, out var id, out var note, out Error))
                return false;

            if (!TOTPSettings.TryCreate(SharedSecret,
                                        ValidityTime,
                                        Length,
                                        Alphabet,
                                        HashAlgorithm,
                                        out var settings,
                                        out var generated,
                                        out Error))
            {
                return false;
            }

            if (!TryChange(id,
                           note,
                           GroupId,
                           // A new secret does not undo the binding that was
                           // asked for: that would be less than configured,
                           // quietly.
                           login => login with { TOTP = settings with { TLSChannelBinding = login.TOTP?.TLSChannelBinding ?? false } },
                           existed => existed
                                          ? $"The TOTP configuration of the charging station '{id}' was changed: {settings}."
                                          : $"The charging station '{id}' may now sign in to this {NodeWording.Noun} with a one-time token.",
                           out Error,
                           out NotSaved))
            {
                return false;
            }

            Generated = generated;

            return true;

        }

        #endregion

        #region TrySetTOTPBinding(Id, Bound, out Error [, out NotSaved])

        /// <summary>
        /// Say whether the one-time token of a charging station has to be
        /// bound to the TLS session of its connection, leaving its secret and
        /// everything else about it alone.
        /// </summary>
        public Boolean TrySetTOTPBinding(String                            Id,
                                         Boolean                           Bound,
                                         [NotNullWhen(false)] out String?  Error)

            => TrySetTOTPBinding(Id, Bound, out Error, out _);

        /// <summary>
        /// Say whether the one-time token of a charging station has to be
        /// bound to the TLS session of its connection - and say whether a
        /// refusal was the file's rather than the change's.
        /// </summary>
        /// <param name="NotSaved">True where the file could not be written: see <see cref="TryRemove(String, out String?, out Boolean)"/>.</param>
        public Boolean TrySetTOTPBinding(String                            Id,
                                         Boolean                           Bound,
                                         [NotNullWhen(false)] out String?  Error,
                                         out Boolean                       NotSaved)

            => TryClear(Id,
                        login => login.HasTOTP,
                        login => login with { TOTP = login.TOTP! with { TLSChannelBinding = Bound } },
                        Bound
                            ? $"The one-time token of the charging station '{Id}' has to be bound to the TLS session of its connection from now on: it signs in over HTTP/3 only."
                            : $"The one-time token of the charging station '{Id}' need no longer be bound to the TLS session of its connection.",
                        $"The charging station '{Id}' has no TOTP configuration to bind.",
                        out Error,
                        out NotSaved);

        #endregion

        #region TryClearPassword(Id, out Error [, out NotSaved]) / TryClearTOTP(Id, out Error [, out NotSaved])

        /// <summary>
        /// Take the password away from a charging station, leaving everything
        /// else about it alone.
        /// </summary>
        public Boolean TryClearPassword(String                            Id,
                                        [NotNullWhen(false)] out String?  Error)

            => TryClearPassword(Id, out Error, out _);

        /// <summary>
        /// Take the password away from a charging station - and say whether a
        /// refusal was the file's rather than the change's.
        /// </summary>
        /// <param name="NotSaved">True where the file could not be written: see <see cref="TryRemove(String, out String?, out Boolean)"/>.</param>
        public Boolean TryClearPassword(String                            Id,
                                        [NotNullWhen(false)] out String?  Error,
                                        out Boolean                       NotSaved)

            => TryClear(Id,
                        login => login.HasPassword,
                        login => login with { Password = null },
                        $"The charging station '{Id}' can no longer sign in with a password.",
                        $"The charging station '{Id}' has no password to take away.",
                        out Error,
                        out NotSaved);

        /// <summary>
        /// Take the one-time token away from a charging station, leaving
        /// everything else about it alone.
        /// </summary>
        public Boolean TryClearTOTP(String                            Id,
                                    [NotNullWhen(false)] out String?  Error)

            => TryClearTOTP(Id, out Error, out _);

        /// <summary>
        /// Take the one-time token away from a charging station - and say
        /// whether a refusal was the file's rather than the change's.
        /// </summary>
        /// <param name="NotSaved">True where the file could not be written: see <see cref="TryRemove(String, out String?, out Boolean)"/>.</param>
        public Boolean TryClearTOTP(String                            Id,
                                    [NotNullWhen(false)] out String?  Error,
                                    out Boolean                       NotSaved)

            => TryClear(Id,
                        login => login.HasTOTP,
                        login => login with { TOTP = null },
                        $"The charging station '{Id}' can no longer sign in with a one-time token.",
                        $"The charging station '{Id}' has no TOTP configuration to take away.",
                        out Error,
                        out NotSaved);

        #endregion

        #region TryAddSSHKey(Id, Line, Label, GroupId, Note, CreatedBy, out Key, out Error [, out NotSaved])

        /// <summary>
        /// Add a charging station, or give one that is already here an SSH key
        /// to sign in with for OCPP over SSH.
        /// </summary>
        /// <param name="Id">What the charging station calls itself in OCPP - and signs in to SSH as.</param>
        /// <param name="Line">Its public key, as one line of an authorized_keys file: "ssh-ed25519 AAAA... comment", options such as from= in front where wanted.</param>
        /// <param name="Label">What the key is called, if anything.</param>
        /// <param name="GroupId">The group it should belong to, or null for the one it is in already.</param>
        /// <param name="Note">What this station is, for whoever reads the list later.</param>
        /// <param name="CreatedBy">Who added the key, if anybody said.</param>
        /// <param name="Key">The key, as it was added.</param>
        public Boolean TryAddSSHKey(String                                Id,
                                    String?                               Line,
                                    String?                               Label,
                                    String?                               GroupId,
                                    String?                               Note,
                                    String?                               CreatedBy,
                                    [NotNullWhen(true)]  out UserSSHKey?  Key,
                                    [NotNullWhen(false)] out String?      Error)

            => TryAddSSHKey(Id, Line, Label, GroupId, Note, CreatedBy, out Key, out Error, out _);

        /// <summary>
        /// Add a charging station, or give one that is already here an SSH key
        /// - and say whether a refusal was the file's rather than the change's.
        /// </summary>
        /// <remarks>
        /// <b>One key is one charging station.</b> A key that already lets
        /// another station in is refused: the station signs in under its own
        /// name, and a key two stations share is a key either can use to be
        /// the other.
        ///
        /// <b>Its own key, not an authority.</b> A cert-authority line vouches
        /// for certificates rather than signing in itself; that is how a whole
        /// fleet of stations would be let in, and it is not what this is.
        /// </remarks>
        /// <param name="NotSaved">True where the file could not be written: see <see cref="TryRemove(String, out String?, out Boolean)"/>.</param>
        public Boolean TryAddSSHKey(String                                Id,
                                    String?                               Line,
                                    String?                               Label,
                                    String?                               GroupId,
                                    String?                               Note,
                                    String?                               CreatedBy,
                                    [NotNullWhen(true)]  out UserSSHKey?  Key,
                                    [NotNullWhen(false)] out String?      Error,
                                    out Boolean                           NotSaved)
        {

            Key       = null;
            NotSaved  = false;

            if (!TryCheckIdAndNote(Id, Note, out var id, out var note, out Error))
                return false;

            if (Line is null || Line.Trim().Length == 0)
            {
                Error = "An SSH public key is required, as one line of an authorized_keys file: \"ssh-ed25519 AAAA... comment\".";
                return false;
            }

            if (!UserSSHKey.TryCreate(Line, Label, TimeProvider.GetUtcNow(), CreatedBy, out var key, out var refused))
            {
                Error = refused;
                return false;
            }

            if (key.Key.IsCertAuthority)
            {
                Error = "That is a cert-authority line, which vouches for SSH certificates rather than signing in itself. " +
                        "A charging station signs in with a key of its own: give its public key.";
                return false;
            }

            lock (updateLock)
            {

                var existing = logins.GetValueOrDefault(id);

                if (existing is null && logins.Count >= MaxStations)
                {
                    Error = $"At most {MaxStations} charging stations may be listed here.";
                    return false;
                }

                var groupId = GroupId?.Trim().ToLowerInvariant() is { Length: > 0 } wanted
                                  ? wanted
                                  : existing?.GroupId ?? LoginGroup.DefaultId;

                if (!groups.ContainsKey(groupId))
                {
                    Error = $"This {NodeWording.Noun} has never heard of a login group called '{groupId}'.";
                    return false;
                }

                if (logins.Values.FirstOrDefault(login => login.Keys.Any(known => known.Fingerprint == key.Fingerprint)) is ChargingStationLogin holder)
                {
                    Error = holder.Id == id
                                ? $"The charging station '{id}' has the SSH key {key.Fingerprint} already."
                                : $"The SSH key {key.Fingerprint} already lets the charging station '{holder.Id}' in. " +
                                   "One key is one charging station: give this one a key of its own.";
                    return false;
                }

                if (existing is not null && existing.Keys.Count >= ChargingStationLogin.MaxSSHKeys)
                {
                    Error = $"A charging station may have at most {ChargingStationLogin.MaxSSHKeys} SSH keys; take an old one away first.";
                    return false;
                }

                logins[id] = (existing ?? new ChargingStationLogin(
                                              id,
                                              groupId,
                                              null,
                                              null,
                                              true,
                                              TimeProvider.GetUtcNow(),
                                              null
                                          )) with {
                                 GroupId  = groupId,
                                 Note     = note.Length > 0 ? note : existing?.Note,
                                 SSHKeys  = [ .. existing?.Keys ?? [], key ]
                             };

                if (!TrySave(out Error))
                {

                    NotSaved = true;

                    if (existing is not null)
                        logins[id] = existing;
                    else
                        logins.Remove(id);

                    return false;

                }

                OnNotice?.Invoke(
                    LogLevel.Notice,
                    existing is not null
                        ? $"The charging station '{id}' may now sign in with the SSH key {key}."
                        : $"The charging station '{id}' may now sign in to this {NodeWording.Noun}, with the SSH key {key}."
                );

            }

            OnChanged?.Invoke();

            Key = key;
            return true;

        }

        #endregion

        #region TryRemoveSSHKey(Id, Fingerprint, out Error [, out NotSaved])

        /// <summary>
        /// Take an SSH key away from a charging station, leaving everything
        /// else about it alone.
        /// </summary>
        /// <param name="Id">The charging station.</param>
        /// <param name="Fingerprint">The key's SHA-256 fingerprint, "SHA256:..." as OpenSSH writes it.</param>
        public Boolean TryRemoveSSHKey(String                            Id,
                                       String                            Fingerprint,
                                       [NotNullWhen(false)] out String?  Error)

            => TryRemoveSSHKey(Id, Fingerprint, out Error, out _);

        /// <summary>
        /// Take an SSH key away from a charging station - and say whether a
        /// refusal was the file's rather than the change's.
        /// </summary>
        /// <remarks>
        /// A session the key opened ends: whoever serves OCPP over SSH is told
        /// through <see cref="OnChanged"/>, and asks every open session again.
        /// </remarks>
        /// <param name="NotSaved">True where the file could not be written: see <see cref="TryRemove(String, out String?, out Boolean)"/>.</param>
        public Boolean TryRemoveSSHKey(String                            Id,
                                       String                            Fingerprint,
                                       [NotNullWhen(false)] out String?  Error,
                                       out Boolean                       NotSaved)
        {

            Error     = null;
            NotSaved  = false;

            lock (updateLock)
            {

                if (!logins.TryGetValue(Id, out var login))
                {
                    Error = $"This {NodeWording.Noun} has never heard of a charging station called '{Id}'.";
                    return false;
                }

                var key = login.Keys.FirstOrDefault(known => known.Fingerprint == Fingerprint);

                if (key is null)
                {
                    Error = $"The charging station '{Id}' has no SSH key {Fingerprint}.";
                    return false;
                }

                var rest = login.Keys.Where(known => !ReferenceEquals(known, key)).ToArray();

                logins[Id] = login with { SSHKeys = rest.Length > 0 ? rest : null };

                if (!TrySave(out Error))
                {
                    NotSaved    = true;
                    logins[Id]  = login;
                    return false;
                }

                OnNotice?.Invoke(LogLevel.Notice, $"The charging station '{Id}' can no longer sign in with the SSH key {key}.");

            }

            OnChanged?.Invoke();

            return true;

        }

        #endregion

        #region TryAddKeyPin(Id, Pin, Label, GroupId, Note, AddedBy, out KeyPin, out Error [, out NotSaved])

        /// <summary>
        /// Add a charging station, or give one that is already here the key pin
        /// of a TLS client key to sign in with.
        /// </summary>
        /// <param name="Id">What the charging station calls itself in OCPP - the last segment of the URL it connects to.</param>
        /// <param name="Pin">The key pin: 64 hexadecimal digits, with or without colons, or "sha256//" and base64.</param>
        /// <param name="Label">What the key is called, if anything.</param>
        /// <param name="GroupId">The group it should belong to, or null for the one it is in already.</param>
        /// <param name="Note">What this station is, for whoever reads the list later.</param>
        /// <param name="AddedBy">Who listed the pin, if anybody said.</param>
        /// <param name="KeyPin">The key pin, as it was listed.</param>
        public Boolean TryAddKeyPin(String                                   Id,
                                    String?                                  Pin,
                                    String?                                  Label,
                                    String?                                  GroupId,
                                    String?                                  Note,
                                    String?                                  AddedBy,
                                    [NotNullWhen(true)]  out StationKeyPin?  KeyPin,
                                    [NotNullWhen(false)] out String?         Error)

            => TryAddKeyPin(Id, Pin, Label, GroupId, Note, AddedBy, out KeyPin, out Error, out _);

        /// <summary>
        /// Add a charging station, or give one that is already here a key pin -
        /// and say whether a refusal was the file's rather than the change's.
        /// </summary>
        /// <remarks>
        /// <b>One key is one charging station.</b> A key pin that already lets
        /// another station in is refused. With a key pin that is not a matter of
        /// tidiness: a certificate accepted by its key pin names nobody, and the
        /// station it is let in as is the one its pin is listed for - so a pin
        /// two stations shared would let either be the other.
        /// </remarks>
        /// <param name="NotSaved">True where the file could not be written: see <see cref="TryRemove(String, out String?, out Boolean)"/>.</param>
        public Boolean TryAddKeyPin(String                                   Id,
                                    String?                                  Pin,
                                    String?                                  Label,
                                    String?                                  GroupId,
                                    String?                                  Note,
                                    String?                                  AddedBy,
                                    [NotNullWhen(true)]  out StationKeyPin?  KeyPin,
                                    [NotNullWhen(false)] out String?         Error,
                                    out Boolean                              NotSaved)
        {

            KeyPin    = null;
            NotSaved  = false;

            if (!TryCheckIdAndNote(Id, Note, out var id, out var note, out Error))
                return false;

            if (!StationKeyPin.TryCreate(Pin, Label, TimeProvider.GetUtcNow(), AddedBy, out var keyPin, out Error))
                return false;

            lock (updateLock)
            {

                var existing = logins.GetValueOrDefault(id);

                if (existing is null && logins.Count >= MaxStations)
                {
                    Error = $"At most {MaxStations} charging stations may be listed here.";
                    return false;
                }

                var groupId = GroupId?.Trim().ToLowerInvariant() is { Length: > 0 } wanted
                                  ? wanted
                                  : existing?.GroupId ?? LoginGroup.DefaultId;

                if (!groups.ContainsKey(groupId))
                {
                    Error = $"This {NodeWording.Noun} has never heard of a login group called '{groupId}'.";
                    return false;
                }

                if (logins.Values.FirstOrDefault(login => login.FindKeyPin(keyPin.Pin) is not null) is ChargingStationLogin holder)
                {
                    Error = holder.Id == id
                                ? $"The charging station '{id}' has the key pin {keyPin.Pin} already."
                                : $"The key pin {keyPin.Pin} already lets the charging station '{holder.Id}' in. " +
                                   "One key is one charging station: give this one a key of its own.";
                    return false;
                }

                if (existing is not null && existing.Pins.Count >= ChargingStationLogin.MaxKeyPins)
                {
                    Error = $"A charging station may have at most {ChargingStationLogin.MaxKeyPins} key pins; take an old one away first.";
                    return false;
                }

                logins[id] = (existing ?? new ChargingStationLogin(
                                              id,
                                              groupId,
                                              null,
                                              null,
                                              true,
                                              TimeProvider.GetUtcNow(),
                                              null
                                          )) with {
                                 GroupId  = groupId,
                                 Note     = note.Length > 0 ? note : existing?.Note,
                                 KeyPins  = [ .. existing?.Pins ?? [], keyPin ]
                             };

                if (!TrySave(out Error))
                {

                    NotSaved = true;

                    if (existing is not null)
                        logins[id] = existing;
                    else
                        logins.Remove(id);

                    return false;

                }

                pinsEverListed.Add(keyPin.Pin);

                OnNotice?.Invoke(
                    LogLevel.Notice,
                    existing is not null
                        ? $"The charging station '{id}' may now sign in with the key pin {keyPin}."
                        : $"The charging station '{id}' may now sign in to this {NodeWording.Noun}, with the key pin {keyPin}."
                );

            }

            OnChanged?.Invoke();

            KeyPin = keyPin;
            return true;

        }

        #endregion

        #region TryRemoveKeyPin(Id, Pin, out Error [, out NotSaved])

        /// <summary>
        /// Take a key pin away from a charging station, leaving everything else
        /// about it alone.
        /// </summary>
        /// <param name="Id">The charging station.</param>
        /// <param name="Pin">The key pin, in any of the notations <see cref="StationKeyPin.TryCreate"/> reads.</param>
        public Boolean TryRemoveKeyPin(String                            Id,
                                       String                            Pin,
                                       [NotNullWhen(false)] out String?  Error)

            => TryRemoveKeyPin(Id, Pin, out Error, out _);

        /// <summary>
        /// Take a key pin away from a charging station - and say whether a
        /// refusal was the file's rather than the change's.
        /// </summary>
        /// <remarks>
        /// A connection the pin let in stays open, as one a password let in
        /// does when the password is taken away: what is decided at the door is
        /// decided at the door. The next one is refused.
        /// </remarks>
        /// <param name="NotSaved">True where the file could not be written: see <see cref="TryRemove(String, out String?, out Boolean)"/>.</param>
        public Boolean TryRemoveKeyPin(String                            Id,
                                       String                            Pin,
                                       [NotNullWhen(false)] out String?  Error,
                                       out Boolean                       NotSaved)
        {

            Error     = null;
            NotSaved  = false;

            if (!org.GraphDefined.Vanaheimr.Hermod.KeyPin.TryParse(Pin, out var pin))
            {
                Error = $"'{Pin}' is not a key pin.";
                return false;
            }

            lock (updateLock)
            {

                if (!logins.TryGetValue(Id, out var login))
                {
                    Error = $"This {NodeWording.Noun} has never heard of a charging station called '{Id}'.";
                    return false;
                }

                var keyPin = login.FindKeyPin(pin);

                if (keyPin is null)
                {
                    Error = $"The charging station '{Id}' has no key pin {pin}.";
                    return false;
                }

                var rest = login.Pins.Where(known => !ReferenceEquals(known, keyPin)).ToArray();

                logins[Id] = login with { KeyPins = rest.Length > 0 ? rest : null };

                if (!TrySave(out Error))
                {
                    NotSaved    = true;
                    logins[Id]  = login;
                    return false;
                }

                OnNotice?.Invoke(LogLevel.Notice, $"The charging station '{Id}' can no longer sign in with the key pin {keyPin}.");

            }

            OnChanged?.Invoke();

            return true;

        }

        #endregion

        #region TrySetEnabled(Id, Enabled, out Error [, out NotSaved])

        /// <summary>
        /// Let a charging station in, or stop letting it in, without throwing
        /// its credentials away.
        /// </summary>
        public Boolean TrySetEnabled(String                            Id,
                                     Boolean                           Enabled,
                                     [NotNullWhen(false)] out String?  Error)

            => TrySetEnabled(Id, Enabled, out Error, out _);

        /// <summary>
        /// Let a charging station in, or stop letting it in - and say whether
        /// a refusal was the file's rather than the change's.
        /// </summary>
        /// <param name="NotSaved">True where the file could not be written: see <see cref="TryRemove(String, out String?, out Boolean)"/>.</param>
        public Boolean TrySetEnabled(String                            Id,
                                     Boolean                           Enabled,
                                     [NotNullWhen(false)] out String?  Error,
                                     out Boolean                       NotSaved)
        {

            Error     = null;
            NotSaved  = false;

            lock (updateLock)
            {

                if (!logins.TryGetValue(Id, out var login))
                {
                    Error = $"This {NodeWording.Noun} has never heard of a charging station called '{Id}'.";
                    return false;
                }

                if (login.Enabled == Enabled)
                    return true;

                logins[Id] = login with { Enabled = Enabled };

                if (!TrySave(out Error))
                {
                    NotSaved    = true;
                    logins[Id]  = login;
                    return false;
                }

                OnNotice?.Invoke(
                    LogLevel.Notice,
                    Enabled
                        ? $"The charging station '{Id}' may sign in again."
                        : $"The charging station '{Id}' may no longer sign in."
                );

            }

            OnChanged?.Invoke();

            return true;

        }

        #endregion

        #region TryRemove(Id, out Error [, out NotSaved])

        /// <summary>
        /// Forget a charging station entirely.
        /// </summary>
        public Boolean TryRemove(String                            Id,
                                 [NotNullWhen(false)] out String?  Error)

            => TryRemove(Id, out Error, out _);

        /// <summary>
        /// Forget a charging station entirely - and say whether a refusal was
        /// the file's rather than the change's.
        /// </summary>
        /// <remarks>
        /// Every change here says so, where it may be refused for two reasons
        /// that are answered differently: something about it was wrong - no
        /// such station, a password too short - or it was fine and the file
        /// could not be written, the change put back as it was. The web
        /// interface answered both with the 404 or 400 of the first, and a
        /// disk that was full was a station "not found".
        /// </remarks>
        /// <param name="NotSaved">True where the file could not be written, and the change was undone.</param>
        public Boolean TryRemove(String                            Id,
                                 [NotNullWhen(false)] out String?  Error,
                                 out Boolean                       NotSaved)
        {

            Error     = null;
            NotSaved  = false;

            lock (updateLock)
            {

                if (!logins.TryGetValue(Id, out var login))
                {
                    Error = $"This {NodeWording.Noun} has never heard of a charging station called '{Id}'.";
                    return false;
                }

                logins.Remove(Id);

                if (!TrySave(out Error))
                {
                    NotSaved    = true;
                    logins[Id]  = login;
                    return false;
                }

                OnNotice?.Invoke(LogLevel.Notice, $"The charging station '{Id}' was removed; it can no longer sign in.");

            }

            OnChanged?.Invoke();

            return true;

        }

        #endregion


        #region Verify(Id, Password)

        /// <summary>
        /// Whether this pair may sign in.
        /// </summary>
        /// <remarks>
        /// A station that is switched off is refused as surely as one that was
        /// never here; the two are not told apart on the way out, because
        /// whoever is asking has no business learning which identifications
        /// exist. What the group allows is decided by the caller, which knows
        /// what the transport looked like.
        /// </remarks>
        public Boolean Verify(String Id, String Password)
        {

            ChargingStationLogin? login;

            lock (updateLock)
                login = logins.GetValueOrDefault(Id);

            // Still hashed when there is nothing to compare against, so that an
            // unknown identification does not answer faster than a known one.
            // A fixed word rather than the one that was sent: the work is the
            // same either way, and hashing what arrived would mean an empty
            // password throws here instead of being refused.
            if (login is null || !login.Enabled || login.Password is not SecurePassword password)
            {
                SecurePassword.Create(TimingEqualiser).Verify(TimingEqualiser);
                return false;
            }

            return password.Verify(Password);

        }

        #endregion

        #region VerifyTOTP(Id, Token)

        /// <summary>
        /// Whether this is the one-time token this charging station should be
        /// showing right now.
        /// </summary>
        /// <remarks>
        /// The same silence as <see cref="Verify"/>, and the same equalising:
        /// a token is derived for an identification that is not here either,
        /// so that the answer takes the same shape of time whether or not the
        /// station exists.
        /// </remarks>
        /// <param name="Bound">Whether the token was sent as one bound to the TLS session of its connection.</param>
        /// <param name="TLSExporterMaterial">The keying material of the connection it arrived on, or null where its TLS exports none.</param>
        public Boolean VerifyTOTP(String   Id,
                                  String   Token,
                                  Boolean  Bound                = false,
                                  Byte[]?  TLSExporterMaterial  = null)
        {

            ChargingStationLogin? login;

            lock (updateLock)
                login = logins.GetValueOrDefault(Id);

            if (login is null || !login.Enabled || login.TOTP is not TOTPSettings settings)
            {

                TOTPGenerator.GenerateTOTPs(
                    TOTPTimingEqualiser,
                    TOTPSettings.DefaultValidityTime,
                    TOTPSettings.DefaultLength,
                    TOTPSettings.DefaultAlphabet,
                    TimeProvider.GetUtcNow(),
                    null,
                    TOTPHashAlgorithm.SHA256
                );

                return false;

            }

            return settings.Matches(Token, TimeProvider.GetUtcNow(), Bound, TLSExporterMaterial);

        }

        #endregion

        #region FindSSHKey(Id, PublicKeyBlob, At)

        /// <summary>
        /// The charging station of this identification and its key a client
        /// offers, where the two may sign in at the given time: the station
        /// switched on, and the key one of its own, switched on and valid.
        /// Null where they may not.
        /// </summary>
        /// <remarks>
        /// What the group allows is decided by the caller, as for
        /// <see cref="Verify"/>. No hashing to equalise here: a public key
        /// proves itself by a signature the SSH server checks, and nothing
        /// about this lookup takes longer for a station that exists.
        /// </remarks>
        /// <param name="Id">What the station signs in as.</param>
        /// <param name="PublicKeyBlob">The public key blob the client offers.</param>
        /// <param name="At">When the client signs in.</param>
        public (ChargingStationLogin Login, UserSSHKey Key)? FindSSHKey(String              Id,
                                                                         ReadOnlySpan<Byte>  PublicKeyBlob,
                                                                         DateTimeOffset      At)
        {

            ChargingStationLogin? login;

            lock (updateLock)
                login = logins.GetValueOrDefault(Id);

            if (login is null || !login.Enabled)
                return null;

            return login.FindSSHKey(PublicKeyBlob, At) is UserSSHKey key
                       ? (login, key)
                       : null;

        }

        #endregion

        #region FindKeyPin(KeyPin) / WasEverListed(KeyPin)

        /// <summary>
        /// The charging station the given key pin is listed for, switched on or
        /// not - or null where it is nobody's.
        /// </summary>
        /// <remarks>
        /// Not narrowed to the stations that may sign in, unlike
        /// <see cref="FindSSHKey"/>: a pin of a station that is switched off is
        /// still that station's pin, and a certificate with its key is refused
        /// as that station - not taken for a stranger's whose common name then
        /// decides who it is. What the station and its group allow is the
        /// caller's to ask.
        /// </remarks>
        /// <param name="KeyPin">The key pin of the key a client presented.</param>
        public ChargingStationLogin? FindKeyPin(KeyPin KeyPin)
        {
            lock (updateLock)
                return logins.Values.FirstOrDefault(login => login.FindKeyPin(KeyPin) is not null);
        }

        /// <summary>
        /// Whether the given key pin was listed for a charging station at any
        /// time since this store was made, whether or not it still is.
        /// </summary>
        /// <remarks>
        /// For the moment between a TLS handshake and the WebSocket upgrade that
        /// follows it, which the client decides the length of. A certificate let
        /// through the handshake by its key pin skipped the accepted chains; if
        /// the pin is taken away before the upgrade arrives, the certificate
        /// must not be read as one that went through them. Whoever asks this
        /// holds such a certificate against the chains once more.
        /// </remarks>
        /// <param name="KeyPin">A key pin.</param>
        public Boolean WasEverListed(KeyPin KeyPin)
        {
            lock (updateLock)
                return pinsEverListed.Contains(KeyPin);
        }

        #endregion

        #region TryGet(Id, out Login) / GroupOf(Login)

        /// <summary>
        /// One charging station, when it is listed here.
        /// </summary>
        public Boolean TryGet(String                                        Id,
                              [NotNullWhen(true)] out ChargingStationLogin?  Login)
        {
            lock (updateLock)
            {
                Login = logins.GetValueOrDefault(Id);
                return Login is not null;
            }
        }

        /// <summary>
        /// The group a login belongs to.
        /// </summary>
        /// <remarks>
        /// Never null for a login that came from this store: a login whose
        /// group is missing is refused while the file is read. A login from
        /// anywhere else falls back to nothing, and the caller must treat that
        /// as "not allowed" rather than as "no restrictions".
        /// </remarks>
        public LoginGroup? GroupOf(ChargingStationLogin Login)
        {
            lock (updateLock)
                return groups.GetValueOrDefault(Login.GroupId);
        }

        /// <summary>
        /// One group by its identification.
        /// </summary>
        public LoginGroup? GetGroup(String Id)
        {
            lock (updateLock)
                return groups.GetValueOrDefault(Id?.Trim().ToLowerInvariant() ?? "");
        }

        #endregion

        #region SecurePasswords()

        /// <summary>
        /// The stations that may sign in with a password, in the shape the
        /// WebSocket server keeps them in.
        /// </summary>
        /// <remarks>
        /// Only those whose group still allows a password at all - the server
        /// keeps this list to answer with, and a station whose group has had
        /// Basic Authentication taken away should not be in it.
        /// </remarks>
        public IReadOnlyDictionary<String, SecurePassword> SecurePasswords()
        {
            lock (updateLock)
                return logins.Values.Where(login => login.Enabled &&
                                                    login.Password.HasValue &&
                                                    groups.GetValueOrDefault(login.GroupId)?.Allows(AuthMethod.Basic) == true).
                                     ToDictionary(login => login.Id, login => login.Password!.Value);
        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// The charging stations and their groups, as the web interface reads
        /// them.
        /// </summary>
        /// <remarks>
        /// Without the hashes and without the shared secrets. A page showing
        /// who may sign in is a page that whoever may read the configuration
        /// may open; a PBKDF2 hash on a screen is a hash somebody can take away
        /// and attack offline, and a shared secret on a screen is a charging
        /// station somebody can impersonate outright.
        /// </remarks>
        public JObject ToJSON()
        {

            lock (updateLock)

                return new JObject(

                    new JProperty("file",               Path),
                    new JProperty("enabled",            logins.Values.Count(login => login.Enabled &&
                                                                                     groups.GetValueOrDefault(login.GroupId)?.Enabled == true)),
                    new JProperty("maxStations",        MaxStations),
                    new JProperty("maxGroups",          LoginGroup.MaxGroups),
                    new JProperty("minPasswordLength",  MinPasswordLength),
                    new JProperty("minSecretLength",    TOTPSettings.MinSecretLength),
                    new JProperty("defaultGroup",       LoginGroup.DefaultId),

                    new JProperty("groups",             new JArray(
                        groups.Values.OrderByDescending(group => group.IsBuiltIn).
                                      ThenBy          (group => group.Id, StringComparer.OrdinalIgnoreCase).
                                      Select          (group => group.ToJSON(logins.Values.Count(login => login.GroupId == group.Id)))
                    )),

                    new JProperty("stations",           new JArray(
                        logins.Values.OrderBy(login => login.Id, StringComparer.OrdinalIgnoreCase).
                                      Select (login => login.ToJSON())
                    ))

                );

        }

        #endregion


        #region (private) TryCheckIdAndNote(...) / TryChange(...) / TryClear(...)

        /// <summary>
        /// What every way of adding a charging station has to check first.
        /// </summary>
        private static Boolean TryCheckIdAndNote(String?                           Id,
                                                 String?                           Note,
                                                 out String                        CheckedId,
                                                 out String                        CheckedNote,
                                                 [NotNullWhen(false)] out String?  Error)
        {

            CheckedId    = Id?.Trim()   ?? "";
            CheckedNote  = Note?.Trim() ?? "";
            Error        = null;

            if (CheckedId.Length == 0)
            {
                Error = "A charging station needs the identification it signs in with.";
                return false;
            }

            if (CheckedId.Length > MaxIdLength)
            {
                Error = $"A charging station identification may be at most {MaxIdLength} characters long.";
                return false;
            }

            if (CheckedNote.Length > MaxNoteLength)
            {
                Error = $"The note may be at most {MaxNoteLength} characters long.";
                return false;
            }

            return true;

        }

        /// <summary>
        /// Add a charging station or change one, write the file, and put
        /// everything back where it was when the file could not be written.
        /// </summary>
        private Boolean TryChange(String                                              Id,
                                  String                                              Note,
                                  String?                                             GroupId,
                                  Func<ChargingStationLogin, ChargingStationLogin>    Change,
                                  Func<Boolean, String>                               Notice,
                                  [NotNullWhen(false)] out String?                    Error,
                                  out Boolean                                         NotSaved)
        {

            Error     = null;
            NotSaved  = false;

            lock (updateLock)
            {

                if (!logins.ContainsKey(Id) && logins.Count >= MaxStations)
                {
                    Error = $"At most {MaxStations} charging stations may be listed here.";
                    return false;
                }

                var existing  = logins.GetValueOrDefault(Id);

                var groupId   = GroupId?.Trim().ToLowerInvariant() is { Length: > 0 } wanted
                                    ? wanted
                                    : existing?.GroupId ?? LoginGroup.DefaultId;

                if (!groups.ContainsKey(groupId))
                {
                    Error = $"This {NodeWording.Noun} has never heard of a login group called '{groupId}'.";
                    return false;
                }

                logins[Id] = Change(
                                 existing
                                     ?? new ChargingStationLogin(
                                            Id,
                                            groupId,
                                            null,
                                            null,
                                            true,
                                            TimeProvider.GetUtcNow(),
                                            null
                                        )
                             ) with {
                                 GroupId  = groupId,
                                 Note     = Note.Length > 0 ? Note : existing?.Note
                             };

                if (!TrySave(out Error))
                {

                    NotSaved = true;

                    if (existing is not null)
                        logins[Id] = existing;
                    else
                        logins.Remove(Id);

                    return false;

                }

                OnNotice?.Invoke(LogLevel.Notice, Notice(existing is not null));

            }

            OnChanged?.Invoke();

            return true;

        }

        /// <summary>
        /// Take one credential away from a charging station.
        /// </summary>
        private Boolean TryClear(String                                            Id,
                                 Func<ChargingStationLogin, Boolean>               Has,
                                 Func<ChargingStationLogin, ChargingStationLogin>  Change,
                                 String                                            Notice,
                                 String                                            NothingThere,
                                 [NotNullWhen(false)] out String?                  Error,
                                 out Boolean                                       NotSaved)
        {

            Error     = null;
            NotSaved  = false;

            lock (updateLock)
            {

                if (!logins.TryGetValue(Id, out var login))
                {
                    Error = $"This {NodeWording.Noun} has never heard of a charging station called '{Id}'.";
                    return false;
                }

                if (!Has(login))
                {
                    Error = NothingThere;
                    return false;
                }

                logins[Id] = Change(login);

                if (!TrySave(out Error))
                {
                    NotSaved    = true;
                    logins[Id]  = login;
                    return false;
                }

                OnNotice?.Invoke(LogLevel.Notice, Notice);

            }

            OnChanged?.Invoke();

            return true;

        }

        #endregion

        #region (private) TrySave(out Error)

        /// <summary>
        /// Write the file, readable by its owner alone.
        /// </summary>
        private Boolean TrySave([NotNullWhen(false)] out String? Error)
        {

            Error = null;

            try
            {

                var directory = System.IO.Path.GetDirectoryName(Path);

                if (!String.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                var temporary = Path + ".tmp";

                OwnerOnlyFile.Write(
                    temporary,
                    new JObject(

                        new JProperty("groups",   new JArray(
                            groups.Values.OrderByDescending(group => group.IsBuiltIn).
                                          ThenBy          (group => group.Id, StringComparer.OrdinalIgnoreCase).
                                          Select          (group => group.ToJSON())
                        )),

                        new JProperty("stations", new JArray(
                            logins.Values.OrderBy(login => login.Id, StringComparer.OrdinalIgnoreCase).
                                          Select (login => login.ToJSON(WithSecrets: true))
                        ))

                    ).ToString(Formatting.Indented) + Environment.NewLine
                );

                File.Move(temporary, Path, overwrite: true);

                return true;

            }
            catch (Exception e)
            {
                Error = $"'{Path}' could not be written: {e.Message}";
                return false;
            }

        }

        #endregion

        #region (override) ToString()

        public override String ToString()
        {
            lock (updateLock)
                return $"{logins.Values.Count(login => login.Enabled)} of {logins.Count} charging station(s) " +
                       $"in {groups.Count} group(s) may sign in";
        }

        #endregion

    }

}
