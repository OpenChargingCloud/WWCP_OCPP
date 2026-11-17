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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP.Node.Connections;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving
{

    /// <summary>
    /// One charging station that may sign in to this local controller, and
    /// what it may sign in with.
    /// </summary>
    /// <remarks>
    /// <b>A login may carry both credentials, or neither.</b> Both, because a
    /// station being moved from a password to a TOTP has to be able to use
    /// either while it is being moved. Neither, because a station that comes in
    /// on a client certificate proves who it is without one - and may still
    /// want to be listed here, so that it belongs to a group that decides
    /// something about it.
    ///
    /// <b>What is stored differs between the two, and has to.</b> The password
    /// is a PBKDF2 hash and cannot be read back. The TOTP shared secret is the
    /// secret itself, because verifying a token means computing it. See
    /// <see cref="TOTPSettings"/>. The SSH keys are public keys, and nothing
    /// about them is secret: they are written and shown alike. So are the key
    /// pins, which are hashes of public keys.
    /// </remarks>
    /// <param name="Id">What the charging station calls itself in OCPP.</param>
    /// <param name="GroupId">The group that decides what it may do.</param>
    /// <param name="Password">Its password, hashed, or null when it has none.</param>
    /// <param name="TOTP">What it needs to be let in with a one-time token, or null when it has none.</param>
    /// <param name="Enabled">Whether it may sign in at the moment.</param>
    /// <param name="AddedAt">When it was added.</param>
    /// <param name="Note">What it is, for whoever reads the list later.</param>
    /// <param name="SSHKeys">The SSH public keys it signs in with for OCPP over SSH, as authorized_keys lines; null or none when it has none.</param>
    /// <param name="KeyPins">The key pins of the TLS client keys it signs in with; null or none when it has none.</param>
    public sealed record ChargingStationLogin(String                         Id,
                                              String                         GroupId,
                                              SecurePassword?                Password,
                                              TOTPSettings?                  TOTP,
                                              Boolean                        Enabled,
                                              DateTimeOffset                 AddedAt,
                                              String?                        Note,
                                              IReadOnlyList<UserSSHKey>?     SSHKeys  = null,
                                              IReadOnlyList<StationKeyPin>?  KeyPins  = null)
    {

        #region Data

        /// <summary>
        /// How many SSH keys one charging station may have: one, and the one
        /// replacing it while it is being replaced - and some room.
        /// </summary>
        public const Int32 MaxSSHKeys = 8;

        /// <summary>
        /// How many key pins one charging station may have: its key, the one
        /// replacing it, and room for a transition - the four a key-pinned
        /// endpoint holds (EVQI Transport, key pins, 5.5).
        /// </summary>
        public const Int32 MaxKeyPins = KeyPinPolicy.MaxPins;

        #endregion

        #region Properties

        /// <summary>
        /// Its SSH keys, none rather than null.
        /// </summary>
        public IReadOnlyList<UserSSHKey> Keys
            => SSHKeys ?? [];

        /// <summary>
        /// Whether it has an SSH key at all, switched on or not.
        /// </summary>
        public Boolean HasSSHKey
            => SSHKeys is { Count: > 0 };

        /// <summary>
        /// Its key pins, none rather than null.
        /// </summary>
        public IReadOnlyList<StationKeyPin> Pins
            => KeyPins ?? [];

        /// <summary>
        /// Whether it is let in by the key pin of its TLS client key - and so
        /// not by a certificate authority's word for it: a station listed with
        /// key pins is one or the other, never both (EVQI Transport, key pins,
        /// 5.4).
        /// </summary>
        public Boolean HasKeyPin
            => KeyPins is { Count: > 0 };

        /// <summary>
        /// Whether a password has been set for this login.
        /// </summary>
        public Boolean HasPassword
            => Password.HasValue;

        /// <summary>
        /// Whether this login can be let in with a one-time token.
        /// </summary>
        public Boolean HasTOTP
            => TOTP is not null;

        /// <summary>
        /// The ways this login could prove who it is, before the group it
        /// belongs to has had its say.
        /// </summary>
        /// <remarks>
        /// A client certificate is not in here. It is not something the login
        /// carries - it is something the station brings, checked against the
        /// accepted chains - so whether it counts is a question for the group
        /// alone.
        /// </remarks>
        public IEnumerable<AuthMethod> Credentials
        {
            get
            {

                if (HasPassword)
                    yield return AuthMethod.Basic;

                if (HasTOTP)
                    yield return AuthMethod.TOTP;

                if (HasSSHKey)
                    yield return AuthMethod.SSHKey;

                if (HasKeyPin)
                    yield return AuthMethod.KeyPin;

            }
        }

        #endregion


        #region FindSSHKey(PublicKeyBlob, At)

        /// <summary>
        /// The key of this station a client offers - by its public key blob -
        /// where it may sign in with it at the given time: switched on, not
        /// one that vouches for certificates, and within its not-before and
        /// expiry-time. Null where there is none.
        /// </summary>
        /// <remarks>
        /// Whether the station itself is switched on, and what its group says,
        /// is not asked here: see <see cref="ChargingStationLogins.FindSSHKey"/>.
        /// </remarks>
        /// <param name="PublicKeyBlob">The public key blob the client offers.</param>
        /// <param name="At">When the client signs in.</param>
        public UserSSHKey? FindSSHKey(ReadOnlySpan<Byte>  PublicKeyBlob,
                                      DateTimeOffset      At)
        {

            foreach (var key in Keys)
                if (!key.Key.IsCertAuthority &&
                    !key.IsDisabled &&
                     key.Key.Matches(PublicKeyBlob) &&
                     key.Key.IsValidAt(At))
                {
                    return key;
                }

            return null;

        }

        #endregion

        #region FindKeyPin(KeyPin)

        /// <summary>
        /// This station's entry of the given key pin, or null where it has none.
        /// </summary>
        /// <remarks>
        /// Whether the station is switched on, and what its group says, is not
        /// asked here: see <see cref="ChargingStationLogins.FindKeyPin"/>.
        /// </remarks>
        /// <param name="KeyPin">The key pin of the key a client presented.</param>
        public StationKeyPin? FindKeyPin(KeyPin KeyPin)

            => Pins.FirstOrDefault(known => known.Pin == KeyPin);

        #endregion


        #region (static) TryParse(JSON, out Login, out Error)

        /// <summary>
        /// One login, as it stands in the file.
        /// </summary>
        /// <remarks>
        /// A file written before this controller had heard of groups holds no
        /// 'group' on any login, and that has to keep working: those logins
        /// join the group that behaves the way they did.
        /// </remarks>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out ChargingStationLogin?  Login,
                                       [NotNullWhen(false)] out String?                Error)
        {

            Login  = null;
            Error  = null;

            var id = JSON.Value<String>("id")?.Trim() ?? "";

            if (id.Length == 0)
            {
                Error = "every charging station needs an 'id'.";
                return false;
            }

            #region Its password, when it has one

            SecurePassword? password = null;

            var passwordText = JSON.Value<String>("password");

            if (passwordText is not null && passwordText.Length > 0)
            {

                if (SecurePassword.TryParse(passwordText) is not SecurePassword parsed)
                {
                    Error = $"the password of '{id}' is not a hash this {NodeWording.Noun} can read.";
                    return false;
                }

                password = parsed;

            }

            #endregion

            #region Its TOTP settings, when it has any

            TOTPSettings? totp = null;

            if (JSON["totp"] is JObject totpJSON)
            {

                if (!TOTPSettings.TryParse(totpJSON, out totp, out var problem))
                {
                    Error = $"the TOTP configuration of '{id}': {problem}";
                    return false;
                }

            }

            #endregion

            #region Its SSH keys, when it has any

            List<UserSSHKey>? sshKeys = null;

            if (JSON["sshKeys"] is JToken keysToken && keysToken.Type != JTokenType.Null)
            {

                if (keysToken is not JArray keyArray)
                {
                    Error = $"the 'sshKeys' of '{id}' must be a JSON array.";
                    return false;
                }

                sshKeys = [];

                foreach (var keyToken in keyArray)
                {

                    if (keyToken is not JObject keyJSON)
                    {
                        Error = $"an SSH key of '{id}' is not a JSON object.";
                        return false;
                    }

                    if (!UserSSHKey.TryParse(keyJSON, out var sshKey, out var problem))
                    {
                        Error = $"an SSH key of '{id}' cannot be read: {problem}";
                        return false;
                    }

                    // A key twice is one key: what lets the station in is the
                    // same either way, and taking one away has to take it away.
                    if (sshKeys.Any(known => known.Fingerprint == sshKey.Fingerprint))
                    {
                        Error = $"the SSH key {sshKey.Fingerprint} is listed twice for '{id}'.";
                        return false;
                    }

                    sshKeys.Add(sshKey);

                }

                if (sshKeys.Count == 0)
                    sshKeys = null;

            }

            #endregion

            #region Its key pins, when it has any

            List<StationKeyPin>? keyPins = null;

            if (JSON["keyPins"] is JToken pinsToken && pinsToken.Type != JTokenType.Null)
            {

                if (pinsToken is not JArray pinArray)
                {
                    Error = $"the 'keyPins' of '{id}' must be a JSON array.";
                    return false;
                }

                keyPins = [];

                foreach (var pinToken in pinArray)
                {

                    if (pinToken is not JObject pinJSON)
                    {
                        Error = $"a key pin of '{id}' is not a JSON object.";
                        return false;
                    }

                    if (!StationKeyPin.TryParse(pinJSON, out var keyPin, out var problem))
                    {
                        Error = $"a key pin of '{id}' cannot be read: {problem}";
                        return false;
                    }

                    if (keyPins.Any(known => known.Pin == keyPin.Pin))
                    {
                        Error = $"the key pin {keyPin.Pin} is listed twice for '{id}'.";
                        return false;
                    }

                    keyPins.Add(keyPin);

                }

                if (keyPins.Count > MaxKeyPins)
                {
                    Error = $"'{id}' has {keyPins.Count} key pins, and a charging station may have at most {MaxKeyPins}.";
                    return false;
                }

                if (keyPins.Count == 0)
                    keyPins = null;

            }

            #endregion

            var addedAt = DateTimeOffset.MinValue;

            DateTimeOffset.TryParse(JSON.Value<String>("addedAt"),
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    System.Globalization.DateTimeStyles.RoundtripKind,
                                    out addedAt);

            var group = JSON.Value<String>("group")?.Trim().ToLowerInvariant();

            Login = new ChargingStationLogin(
                        id,
                        group is { Length: > 0 } ? group : LoginGroup.DefaultId,
                        password,
                        totp,
                        JSON.Value<Boolean?>("enabled") ?? true,
                        addedAt,
                        JSON.Value<String>("note")?.Trim() is { Length: > 0 } note ? note : null,
                        sshKeys,
                        keyPins
                    );

            return true;

        }

        #endregion

        #region ToJSON(WithSecrets = false)

        /// <summary>
        /// This charging station, with the hash and the shared secret only
        /// where the file is being written.
        /// </summary>
        public JObject ToJSON(Boolean WithSecrets = false)
        {

            var json = new JObject(
                           new JProperty("id",           Id),
                           new JProperty("group",        GroupId),
                           new JProperty("enabled",      Enabled),
                           new JProperty("addedAt",      AddedAt.ToString("o")),
                           new JProperty("hasPassword",  HasPassword),
                           new JProperty("hasTOTP",      HasTOTP),
                           new JProperty("hasSSHKey",    HasSSHKey),
                           new JProperty("hasKeyPin",    HasKeyPin)
                       );

            if (Note is not null)
                json.Add("note", Note);

            // Public keys, so the same with and without the secrets.
            if (HasSSHKey)
                json.Add("sshKeys", new JArray(Keys.Select(key => key.ToJSON())));

            // Hashes of public keys, the same.
            if (HasKeyPin)
                json.Add("keyPins", new JArray(Pins.Select(pin => pin.ToJSON())));

            if (TOTP is not null)
                json.Add("totp", TOTP.ToJSON(WithSecret: WithSecrets));

            if (WithSecrets && Password.HasValue)
                json.Add("password", Password.Value.ToString());

            return json;

        }

        #endregion

        #region (override) ToString()

        public override String ToString()

            => $"{Id} in '{GroupId}' ({(Enabled ? "on" : "off")}): " +
               $"{(HasPassword || HasTOTP || HasSSHKey || HasKeyPin ? String.Join(" and ", Credentials.Select(method => method.AsText())) : "no credentials")}";

        #endregion

    }

}
