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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP.Node.Configuration;
using cloud.charging.open.protocols.WWCP.Node.Logging;

using cloud.charging.open.protocols.OCPP.Node.KeyPins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    /// <summary>
    /// What the server of a wss:// connection is to be held to, as a page or
    /// the API sends it - see <see cref="ConnectionEntry.Pins"/>.
    /// </summary>
    /// <param name="Sent">What it is to be held to; null for nothing.</param>
    /// <param name="Shown">What the page showed it held to when it was drawn; null for nothing.</param>
    /// <param name="SaysWhatWasShown">Whether the page said what it showed. Where it did, what it changed is taken and what the server is held to since - learned on first use, or taken from a signed pin set - stays; where it did not, what was sent is what the server is held to.</param>
    public sealed record ServerPinsChange(ServerPins?  Sent,
                                          ServerPins?  Shown             = null,
                                          Boolean      SaysWhatWasShown  = false);


    /// <summary>
    /// The places this charging station dials, and the credentials it proves
    /// itself with when it gets there.
    /// </summary>
    /// <remarks>
    /// Two lists in one place, because the thing that has to hold is the
    /// reference between them: a connection names its credentials rather than
    /// repeating them, so that a password written down once is a password
    /// changed once. Splitting them into two stores would put that reference
    /// across a seam where nothing checks it, and the first thing anybody
    /// would do is delete a login that three connections were using.
    ///
    /// Two files rather than one, and for a reason that is not tidiness: the
    /// credentials hold secrets and are written so that only their owner may
    /// read them, and the connections hold none and are ordinary. A single
    /// file would have to be treated as secret throughout, which makes it
    /// harder to hand somebody the half that is safe to hand over.
    ///
    /// <b>The secrets never come back out.</b> A password or a shared secret
    /// goes in and is written to disk; what the web interface is told is
    /// whether one is set. Changing a record with the secret field left empty
    /// keeps the secret that is already there, which is what lets somebody fix
    /// a typo in a description without being shown - or having to retype - the
    /// password.
    /// </remarks>
    public sealed class ConnectionStore : IPinnedTLSEndpoints
    {

        #region IPinnedTLSEndpoints

        // What the key-pin core of WWCP_OCPP_Node asks of the connections: the
        // wss:// ones, a copy of one, and dialling one again - beside the
        // writes, which are this store's own methods of the same names.

        IEnumerable<IPinnedTLSEndpoint> IPinnedTLSEndpoints.Secure
            => Connections.Where(entry => entry.IsSecure);

        IPinnedTLSEndpoint? IPinnedTLSEndpoints.CopyOf(String Id)
            => CopyOf(Id);

        void IPinnedTLSEndpoints.DialAgain(String Id)
            => DialAgain(Id);

        #endregion


        #region Data

        /// <summary>
        /// The directory beside the configuration file, when nobody says
        /// otherwise.
        /// </summary>
        public const String  DefaultDirectoryName    = "ocpp-connections";

        /// <summary>
        /// The file the credentials live in - owner-readable only.
        /// </summary>
        public const String  AuthenticationsFileName = "authentications.json";

        /// <summary>
        /// The file the connections lived in until they were made peers: read
        /// once, to make the peers of it, and left as it was.
        /// </summary>
        public const String  ConnectionsFileName     = "connections.json";

        /// <summary>
        /// The file the peers live in, with their endpoints in them (section
        /// 11.7.1 of EVQI Transport).
        /// </summary>
        public const String  PeersFileName           = "peers.json";

        /// <summary>
        /// The file the keys a pin set is believed of are kept in.
        /// </summary>
        public const String  PinSetSignersFileName   = "pinSetSigners.json";

        private readonly Object                                   updateLock       = new ();
        private readonly Dictionary<String, AuthenticationEntry>  authentications  = [];
        private readonly Dictionary<String, ConnectionEntry>      connections      = [];
        private readonly Dictionary<String, PeerEntry>            peers            = [];
        private readonly Dictionary<String, Int64>                dialRevisions    = [];

        #endregion

        #region Properties

        /// <summary>
        /// The directory the two files live in.
        /// </summary>
        public String        Path                  { get; }

        /// <summary>
        /// The clock that stamps a record when it is written down.
        /// </summary>
        public TimeProvider  TimeProvider          { get; }

        /// <summary>
        /// Which client certificates exist, for the connections that name one.
        /// </summary>
        /// <remarks>
        /// A function rather than a list, because the answer changes while
        /// this store is alive - somebody may remove a certificate on the page
        /// next door - and because this store has no business holding on to
        /// the certificate store itself. Left unset, a connection naming a
        /// certificate is taken at its word; the station sets it.
        /// </remarks>
        public Func<IEnumerable<String>>? KnownCertificateIds { get; set; }

        /// <summary>
        /// Every set of credentials, newest first.
        /// </summary>
        public IReadOnlyList<AuthenticationEntry> Authentications
        {
            get
            {
                lock (updateLock)
                    return [.. authentications.Values.OrderByDescending(entry => entry.CreatedAt)];
            }
        }

        /// <summary>
        /// Every connection, newest first.
        /// </summary>
        public IReadOnlyList<ConnectionEntry> Connections
        {
            get
            {
                lock (updateLock)
                {
                    CheckEverything();
                    return [.. connections.Values.OrderByDescending(entry => entry.CreatedAt)];
                }
            }
        }

        /// <summary>
        /// Every peer, oldest first, with the ids of its endpoints - which are
        /// the connections above.
        /// </summary>
        public IReadOnlyList<PeerEntry> Peers
        {
            get
            {
                lock (updateLock)
                    return [.. OrderedPeers()];
            }
        }

        /// <summary>
        /// The peer the given connection is an endpoint of.
        /// </summary>
        public PeerEntry? PeerOf(String ConnectionId)
        {
            lock (updateLock)
                return PeerOfUnlocked(ConnectionId);
        }

        /// <summary>
        /// What became of connections.json when the peers were made of it, if
        /// they were.
        /// </summary>
        public PeerMigration? Migration { get; private set; }

        /// <summary>
        /// How often the way the given connection is dialled was changed since
        /// this store was made: its address, its version, whether it dials by
        /// itself, the credentials it names or what they hold, its certificate,
        /// the host key somebody pinned and how it is trusted.
        /// </summary>
        /// <remarks>
        /// What the station compares with the revision it dialled, to dial a
        /// changed connection again at once. A description written anew, a host
        /// key learned on first use or a signed pin set taken are no change of
        /// it: the connection that is up stays up.
        /// </remarks>
        public Int64 DialRevisionOf(String ConnectionId)
        {
            lock (updateLock)
                return dialRevisions.GetValueOrDefault(ConnectionId);
        }

        #endregion

        #region Events

        /// <summary>
        /// Something happened here that belongs in the log of the station: a
        /// connection written down, a login removed, a file that could not be
        /// read, the connections made peers.
        /// </summary>
        /// <remarks>
        /// What is said while the store is being made, before anybody can have
        /// listened, is kept and told to the first one who does: the store reads
        /// its files when it is made, and a file that cannot be read, or the
        /// peers made of connections.json, would otherwise be told to nobody.
        /// </remarks>
        public event Action<LogLevel, String>? OnNotice
        {

            add
            {

                List<(LogLevel Level, String Message)>? untoldSoFar;

                lock (updateLock)
                {
                    onNotice    += value;
                    untoldSoFar  = untold;
                    untold       = null;
                }

                foreach (var (level, message) in untoldSoFar ?? [])
                    value?.Invoke(level, message);

            }

            remove
            {
                lock (updateLock)
                    onNotice -= value;
            }

        }

        private Action<LogLevel, String>?                 onNotice;
        private List<(LogLevel Level, String Message)>?   untold = [];

        /// <summary>
        /// Tell whoever listens, or keep it for the first one who will.
        /// </summary>
        private void Notice(LogLevel Level, String Message)
        {

            if (onNotice is Action<LogLevel, String> listeners)
                listeners(Level, Message);

            else if (untold is not null && untold.Count < 1000)
                untold.Add((Level, Message));

        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The connections and credentials in the given directory; it need not
        /// exist yet.
        /// </summary>
        public ConnectionStore(String?        Path           = null,
                               TimeProvider?  TimeProvider   = null)
        {

            this.Path          = System.IO.Path.GetFullPath(Path ?? DefaultDirectoryName);
            this.TimeProvider  = TimeProvider ?? System.TimeProvider.System;

            Reload();

        }

        #endregion


        #region Reload()

        /// <summary>
        /// Read both files again, from nothing.
        /// </summary>
        /// <remarks>
        /// A record that cannot be read is skipped with a line in the log
        /// rather than thrown: one connection written down wrongly must not
        /// stop a station that has another which works, and the one that works
        /// is frequently the one keeping it reachable.
        /// </remarks>
        public void Reload()
        {

            lock (updateLock)
            {

                authentications.Clear();
                connections.    Clear();

                foreach (var json in Read(AuthenticationsFileName))
                {
                    if (AuthenticationEntry.TryParse(json, out var entry, out var problem))
                        authentications[entry.Id] = entry;
                    else
                        Notice(LogLevel.Warning, $"A set of credentials could not be read: {problem}");
                }

                peers.Clear();
                Migration = null;

                if (File.Exists(System.IO.Path.Combine(Path, PeersFileName)))
                    ReadPeers();

                else
                {

                    foreach (var json in Read(ConnectionsFileName))
                    {
                        if (ConnectionEntry.TryParse(json, out var entry, out var problem))
                            connections[entry.Id] = entry;
                        else
                            Notice(LogLevel.Warning, $"A connection could not be read: {problem}");
                    }

                    if (connections.Count > 0)
                        Migrate();

                }

                pinSetSigners.Clear();

                foreach (var json in Read(PinSetSignersFileName))
                {
                    if (PinSetSigner.TryParse(json) is PinSetSigner signer)
                        pinSetSigners.Add(signer);
                    else
                        Notice(LogLevel.Warning, $"A signer of pin sets could not be read: {json.ToString(Newtonsoft.Json.Formatting.None)}");
                }

            }

        }

        #endregion

        #region (private) Read(FileName) / TrySaveAuthentications(out Error) / TrySaveConnections(out Error)

        /// <summary>
        /// Called under the lock.
        /// </summary>
        private IEnumerable<JObject> Read(String FileName)
        {

            var file = System.IO.Path.Combine(Path, FileName);

            if (!File.Exists(file))
                return [];

            try
            {
                return JArray.Parse(File.ReadAllText(file)).OfType<JObject>().ToArray();
            }
            catch (Exception e)
            {
                Notice(LogLevel.Error, $"'{FileName}' could not be read and is being ignored: {e.Message}");
                return [];
            }

        }

        /// <summary>
        /// The credentials, written out. Called under the lock, once a change
        /// of them is made, and before it is said to be.
        /// </summary>
        /// <remarks>
        /// They hold the secrets, so they are written the way the private keys
        /// next door are - and the mode goes on at creation, not afterwards.
        /// </remarks>
        private Boolean TrySaveAuthentications([NotNullWhen(false)] out String? Error)

            => TrySave(AuthenticationsFileName,
                       new JArray(authentications.Values.
                                      OrderBy(entry => entry.CreatedAt).
                                      Select (entry => entry.ToJSON(IncludeSecrets: true))),
                       true,
                       out Error);

        /// <summary>
        /// The peers and their endpoints - the connections - written out to
        /// peers.json. Called under the lock, once a change of them is made,
        /// and before it is said to be.
        /// </summary>
        private Boolean TrySaveConnections([NotNullWhen(false)] out String? Error)
        {

            var json = new JObject(
                           new JProperty("peers", new JArray(
                               OrderedPeers().Select(peer => peer.ToJSON(peer.EndpointIds.
                                                                             Where (connections.ContainsKey).
                                                                             Select(id => connections[id])))))
                       );

            if (Migration is not null)
                json.Add(new JProperty("migration", Migration.ToJSON()));

            if (!TrySave(PeersFileName, json, false, out Error))
                return false;

            // Told outside the lock, so that whoever follows may ask again.
            if (OnConnectionsChanged is Action changed)
                Task.Run(changed);

            return true;

        }

        private Boolean TrySavePinSetSigners([NotNullWhen(false)] out String? Error)

            => TrySave(PinSetSignersFileName,
                       new JArray(pinSetSigners.Select(signer => signer.ToJSON())),
                       false,
                       out Error);

        /// <summary>
        /// The connections were changed and saved: written down, changed, taken
        /// away, a host key learned or forgotten, a pin set taken.
        /// </summary>
        public event Action? OnConnectionsChanged;

        private readonly List<PinSetSigner> pinSetSigners = [];

        /// <summary>
        /// The keys a pin set is believed of, in the order they were written down.
        /// </summary>
        public IReadOnlyList<PinSetSigner> PinSetSigners
        {
            get
            {
                lock (updateLock)
                    return [.. pinSetSigners];
            }
        }

        /// <summary>
        /// Write down a key a pin set is believed of.
        /// </summary>
        public Boolean TryAddPinSetSigner(String?                           PublicKey,
                                          String?                           Description,
                                          [NotNullWhen(false)] out String?  Error,
                                          out Boolean                       NotSaved)
        {

            NotSaved = false;

            if (!PinSetSigner.TryNormalize(PublicKey, out var publicKey, out Error))
                return false;

            var description = (Description ?? "").Trim();

            if (description.Length == 0 || description.Length > ConnectionEntry.MaxDescriptionLength)
            {
                Error = $"Say whose key it is, in at most {ConnectionEntry.MaxDescriptionLength} characters.";
                return false;
            }

            lock (updateLock)
            {

                if (pinSetSigners.Any(signer => signer.PublicKey == publicKey))
                {
                    Error = "That key is written down already.";
                    return false;
                }

                var signer = new PinSetSigner(publicKey, description, TimeProvider.GetUtcNow());

                pinSetSigners.Add(signer);

                if (!TrySavePinSetSigners(out Error))
                {
                    pinSetSigners.Remove(signer);
                    NotSaved = true;
                    return false;
                }

                Notice(LogLevel.Notice, $"A pin set signed by '{description}' ({publicKey}) is believed from now on.");
                return true;

            }

        }

        /// <summary>
        /// Take away a key a pin set is believed of.
        /// </summary>
        public Boolean TryRemovePinSetSigner(String?                           PublicKey,
                                             [NotNullWhen(false)] out String?  Error,
                                             out Boolean                       NotSaved)
        {

            NotSaved = false;

            if (!PinSetSigner.TryNormalize(PublicKey, out var publicKey, out Error))
                return false;

            lock (updateLock)
            {

                var signer = pinSetSigners.FirstOrDefault(signer => signer.PublicKey == publicKey);

                if (signer is null)
                {
                    Error = "That key is not written down here.";
                    return false;
                }

                pinSetSigners.Remove(signer);

                if (!TrySavePinSetSigners(out Error))
                {
                    pinSetSigners.Add(signer);
                    NotSaved = true;
                    return false;
                }

                Notice(LogLevel.Notice, $"A pin set signed by '{signer.Description}' is no longer believed.");
                return true;

            }

        }

        /// <summary>
        /// One of the two files, written whole: to a file of its own first, and
        /// moved over the old one, so that it is the old one or the new one and
        /// never half of each.
        /// </summary>
        /// <remarks>
        /// Whole files rather than one record at a time: there are a handful
        /// of these, and a rewrite that either happened or did not is easier
        /// to reason about than a directory half-way through being changed.
        /// Only the file that was changed is written. Where it cannot be, the
        /// caller puts the change back: both files were written in place, and
        /// one that could not be threw out of a change already made - the page
        /// was answered 500 by the web server, and the station went on with
        /// what it had not kept until it was started again.
        /// </remarks>
        /// <param name="FileName">Which of the two.</param>
        /// <param name="Entries">What it is to hold.</param>
        /// <param name="OwnerOnly">Whether only its owner may read it.</param>
        /// <param name="Error">Why it was not written.</param>
        private Boolean TrySave(String                            FileName,
                                JToken                            Entries,
                                Boolean                           OwnerOnly,
                                [NotNullWhen(false)] out String?  Error)
        {

            var file       = System.IO.Path.Combine(Path, FileName);
            var temporary  = file + ".tmp";

            try
            {

                Directory.CreateDirectory(Path);

                // Made anew, so that the mode goes on at its creation: one left
                // over from before would keep the mode it was made with.
                File.Delete(temporary);

                if (OwnerOnly)
                    OwnerOnlyFile.Write(temporary, Entries.ToString(Formatting.Indented));

                else
                    File.WriteAllText(temporary, Entries.ToString(Formatting.Indented));

                File.Move(temporary, file, overwrite: true);

                Error = null;
                return true;

            }
            catch (Exception e)
            {

                // What holds the secrets is not left lying about beside them.
                try
                {
                    if (File.Exists(temporary))
                        File.Delete(temporary);
                }
                catch
                { }

                Error = $"'{file}' could not be written: {e.Message}";
                return false;

            }

        }

        #endregion

        #region (private static) NewId()

        /// <summary>
        /// A fresh identification, which is only ever compared and never read
        /// out loud.
        /// </summary>
        private static String NewId()
            => Convert.ToHexString(RandomNumberGenerator.GetBytes(8)).ToLowerInvariant();

        #endregion


        #region TryAddAuthentication   (Description, Kind, Login, Secret, ..., out Id, out Error [, out NotSaved])

        /// <summary>
        /// Write down one set of credentials.
        /// </summary>
        public Boolean TryAddAuthentication(String?                          Description,
                                            String?                          Kind,
                                            String?                          Login,
                                            String?                          Secret,
                                            [NotNullWhen(true)]  out String? Id,
                                            [NotNullWhen(false)] out String? Error,
                                            Double?                          ValiditySeconds     = null,
                                            UInt32?                          Length              = null,
                                            String?                          Alphabet            = null,
                                            String?                          HashAlgorithm       = null,
                                            Boolean?                         TLSChannelBinding   = null)

            => TryAddAuthentication(Description, Kind, Login, Secret, out Id, out Error, out _,
                                    ValiditySeconds, Length, Alphabet, HashAlgorithm, TLSChannelBinding);

        /// <summary>
        /// Write down one set of credentials - and say whether a refusal was
        /// the file's rather than theirs.
        /// </summary>
        /// <param name="NotSaved">True where the file of the credentials could not be written: nothing about them was wrong, and they were not taken.</param>
        public Boolean TryAddAuthentication(String?                          Description,
                                            String?                          Kind,
                                            String?                          Login,
                                            String?                          Secret,
                                            [NotNullWhen(true)]  out String? Id,
                                            [NotNullWhen(false)] out String? Error,
                                            out Boolean                      NotSaved,
                                            Double?                          ValiditySeconds     = null,
                                            UInt32?                          Length              = null,
                                            String?                          Alphabet            = null,
                                            String?                          HashAlgorithm       = null,
                                            Boolean?                         TLSChannelBinding   = null)
        {

            Id        = null;
            NotSaved  = false;

            if (!TryParseKind(Kind, out var kind, out Error))
                return false;

            if (!AuthenticationEntry.Validate(Description, kind, Login, Secret, Alphabet, Length, out Error))
                return false;

            if (String.IsNullOrWhiteSpace(Secret))
            {
                Error = kind == AuthenticationKind.Basic
                            ? "A password is needed. It can be changed later without being shown again, but it has to be set once."
                            : "A shared secret is needed. It can be changed later without being shown again, but it has to be set once.";
                return false;
            }

            lock (updateLock)
            {

                var entry = new AuthenticationEntry(
                                NewId(),
                                Description!.Trim(),
                                kind,
                                Login!.Trim(),
                                TimeProvider.GetUtcNow()
                            );

                Apply(entry, Secret, ValiditySeconds, Length, Alphabet, HashAlgorithm, TLSChannelBinding);

                authentications[entry.Id] = entry;

                if (!TrySaveAuthentications(out Error))
                {
                    authentications.Remove(entry.Id);
                    NotSaved = true;
                    return false;
                }

                Notice(LogLevel.Info, $"Credentials written down: {entry}.");

                Id = entry.Id;
                return true;

            }

        }

        #endregion

        #region TryUpdateAuthentication(Id, Description, Kind, Login, Secret, ..., out Error [, out NotSaved])

        /// <summary>
        /// Change one set of credentials.
        /// </summary>
        /// <remarks>
        /// An empty secret keeps whatever is already there. That is the whole
        /// reason this is not a delete and an add: somebody fixing a
        /// description should not have to know the password, and asking them
        /// for it again is how a password ends up written on a sticky note.
        /// </remarks>
        public Boolean TryUpdateAuthentication(String?                          Id,
                                               String?                          Description,
                                               String?                          Kind,
                                               String?                          Login,
                                               String?                          Secret,
                                               [NotNullWhen(false)] out String? Error,
                                               Double?                          ValiditySeconds     = null,
                                               UInt32?                          Length              = null,
                                               String?                          Alphabet            = null,
                                               String?                          HashAlgorithm       = null,
                                               Boolean?                         TLSChannelBinding   = null)

            => TryUpdateAuthentication(Id, Description, Kind, Login, Secret, out Error, out _,
                                       ValiditySeconds, Length, Alphabet, HashAlgorithm, TLSChannelBinding);

        /// <summary>
        /// Change one set of credentials - and say whether a refusal was the
        /// file's rather than the change's.
        /// </summary>
        /// <param name="NotSaved">True where the file of the credentials could not be written: nothing about the change was wrong, and they stay as they were.</param>
        public Boolean TryUpdateAuthentication(String?                          Id,
                                               String?                          Description,
                                               String?                          Kind,
                                               String?                          Login,
                                               String?                          Secret,
                                               [NotNullWhen(false)] out String? Error,
                                               out Boolean                      NotSaved,
                                               Double?                          ValiditySeconds     = null,
                                               UInt32?                          Length              = null,
                                               String?                          Alphabet            = null,
                                               String?                          HashAlgorithm       = null,
                                               Boolean?                         TLSChannelBinding   = null)
        {

            NotSaved = false;

            if (!TryParseKind(Kind, out var kind, out Error))
                return false;

            if (!AuthenticationEntry.Validate(Description, kind, Login, Secret, Alphabet, Length, out Error))
                return false;

            lock (updateLock)
            {

                if (Id is null || !authentications.TryGetValue(Id, out var entry))
                {
                    Error = "There are no credentials here under that identification.";
                    return false;
                }

                // Changing what kind it is means the secret that is there is
                // the wrong kind of secret, so one has to be given.
                if (entry.Kind != kind && String.IsNullOrWhiteSpace(Secret))
                {
                    Error = kind == AuthenticationKind.Basic
                                ? "Changing these to HTTP Basic needs a password: a shared secret is not one."
                                : "Changing these to HTTP TOTP needs a shared secret: a password is not one.";
                    return false;
                }

                var was = entry.Copy();

                entry.Description  = Description!.Trim();
                entry.Login        = Login!.Trim();
                entry.Kind         = kind;

                Apply(entry, Secret, ValiditySeconds, Length, Alphabet, HashAlgorithm, TLSChannelBinding);

                if (!TrySaveAuthentications(out Error))
                {
                    authentications[Id] = was;
                    NotSaved = true;
                    return false;
                }

                Notice(LogLevel.Info, $"Credentials changed: {entry}.");

                // What a connection proves itself with changed for every one
                // that names them: they are dialled again with it.
                var naming = connections.Values.Where(connection => connection.AuthenticationId == Id).ToArray();

                foreach (var connection in naming)
                    BumpDialRevision(connection.Id);

                if (naming.Length > 0 && OnConnectionsChanged is Action changed)
                    Task.Run(changed);

                Error = null;
                return true;

            }

        }

        #endregion

        #region (private) Apply(Entry, Secret, ...) / TryParseKind(Text, out Kind, out Error)

        /// <summary>
        /// The half of a change that is the same whether it is new or not.
        /// Called under the lock.
        /// </summary>
        private static void Apply(AuthenticationEntry  Entry,
                                  String?              Secret,
                                  Double?              ValiditySeconds,
                                  UInt32?              Length,
                                  String?              Alphabet,
                                  String?              HashAlgorithm,
                                  Boolean?             TLSChannelBinding)
        {

            var secret = Secret?.Trim();

            if (Entry.Kind == AuthenticationKind.Basic)
            {

                if (!String.IsNullOrEmpty(secret))
                    Entry.Password = secret;

                // Whatever was here as TOTP is not a password, and keeping it
                // would leave a secret on disk that nothing can use.
                Entry.SharedSecret       = null;
                Entry.Validity           = null;
                Entry.Length             = null;
                Entry.Alphabet           = null;
                Entry.HashAlgorithm      = null;

            }

            else
            {

                if (!String.IsNullOrEmpty(secret))
                    Entry.SharedSecret = secret;

                Entry.Password           = null;

                Entry.Validity           = ValiditySeconds is Double seconds && seconds > 0
                                               ? TimeSpan.FromSeconds(seconds)
                                               : null;

                Entry.Length             = Length;

                Entry.Alphabet           = String.IsNullOrWhiteSpace(Alphabet) ? null : Alphabet.Trim();

                Entry.HashAlgorithm      = HashAlgorithm?.Trim().ToUpperInvariant() switch {
                                               "SHA384"  => TOTPHashAlgorithm.SHA384,
                                               "SHA512"  => TOTPHashAlgorithm.SHA512,
                                               "SHA256"  => TOTPHashAlgorithm.SHA256,
                                               _         => null
                                           };

                // Left out, it is what it was - unbound for credentials written
                // down anew, see AuthenticationEntry.TLSChannelBinding.
                Entry.TLSChannelBinding  = TLSChannelBinding ?? Entry.TLSChannelBinding;

            }

        }

        private static Boolean TryParseKind(String?                          Text,
                                            out AuthenticationKind           Kind,
                                            [NotNullWhen(false)] out String? Error)
        {

            switch (Text?.Trim().ToLowerInvariant())
            {

                case "basic":
                    Kind   = AuthenticationKind.Basic;
                    Error  = null;
                    return true;

                case "totp":
                    Kind   = AuthenticationKind.TOTP;
                    Error  = null;
                    return true;

                default:
                    Kind   = AuthenticationKind.Basic;
                    Error  = $"'{Text}' is not a kind of authentication this {NodeWording.Noun} knows.";
                    return false;

            }

        }

        #endregion

        #region TryRemoveAuthentication(Id, out Error [, out NotSaved])

        /// <summary>
        /// Take one set of credentials away, for good.
        /// </summary>
        /// <remarks>
        /// Refused while a connection is using it, and the refusal names the
        /// connection. The alternative - removing it and leaving the
        /// connection pointing at nothing - turns one deliberate act into a
        /// station that cannot dial home and gives nobody a reason why.
        /// </remarks>
        public Boolean TryRemoveAuthentication(String?                          Id,
                                               [NotNullWhen(false)] out String? Error)

            => TryRemoveAuthentication(Id, out Error, out _);

        /// <summary>
        /// Take one set of credentials away - and say whether a refusal was
        /// the file's rather than the request's.
        /// </summary>
        /// <param name="NotSaved">True where the file of the credentials could not be written: nothing about the request was wrong, and they stay.</param>
        public Boolean TryRemoveAuthentication(String?                          Id,
                                               [NotNullWhen(false)] out String? Error,
                                               out Boolean                      NotSaved)
        {

            NotSaved = false;

            lock (updateLock)
            {

                if (Id is null || !authentications.TryGetValue(Id, out var entry))
                {
                    Error = "There are no credentials here under that identification.";
                    return false;
                }

                var used = connections.Values.
                               Where(connection => connection.AuthenticationId == Id).
                               ToArray();

                if (used.Length > 0)
                {
                    Error = $"These credentials are in use by {String.Join(", ", used.Select(one => $"'{one.Description}'"))}. " +
                             "Point those somewhere else first.";
                    return false;
                }

                authentications.Remove(Id);

                if (!TrySaveAuthentications(out Error))
                {
                    authentications[Id] = entry;
                    NotSaved = true;
                    return false;
                }

                Notice(LogLevel.Info, $"Credentials removed: {entry}.");

                Error = null;
                return true;

            }

        }

        #endregion


        #region TryAddConnection   (Description, URL, ConnectionType, ..., out Id, out Error [, out NotSaved])

        /// <summary>
        /// Write down one connection.
        /// </summary>
        public Boolean TryAddConnection(String?                          Description,
                                        String?                          URL,
                                        String?                          ConnectionType,
                                        Boolean?                         AutoConnect,
                                        String?                          AuthenticationId,
                                        String?                          CertificateId,
                                        [NotNullWhen(true)]  out String? Id,
                                        [NotNullWhen(false)] out String? Error,
                                        String?                          OCPPVersion     = null,
                                        String?                          SshHostKey      = null,
                                        SshTrustSettings?                SshTrust        = null,
                                        String?                          ServerKeyPins   = null,
                                        ServerKeyPinSettings?            KeyPinTrust     = null,
                                        ServerPinsChange?                Pins            = null,
                                        String?                          HTTPVersion     = null,
                                        String?                          Encoding        = null,
                                        String?                          SignatureFormats = null)

            => TryAddConnection(Description, URL, ConnectionType, AutoConnect, AuthenticationId, CertificateId,
                                out Id, out Error, out _, OCPPVersion, SshHostKey, SshTrust, ServerKeyPins, KeyPinTrust, Pins, HTTPVersion, Encoding, SignatureFormats);

        /// <summary>
        /// Write down one connection - and say whether a refusal was the
        /// file's rather than the connection's.
        /// </summary>
        /// <param name="NotSaved">True where the file of the connections could not be written: nothing about this one was wrong, and it was not taken.</param>
        /// <param name="SshHostKey">The host keys the server of an ssh:// connection may show, separated by commas; none where it is not dialled or learns one on first use.</param>
        /// <param name="SshTrust">How an ssh:// connection believes its server beyond its pins; none for no trust on first use, mismatches refused.</param>
        /// <param name="ServerKeyPins">The key pins of the keys the server of a wss:// connection may show, separated by commas; none where it is checked by its CA or learns one on first use.</param>
        /// <param name="KeyPinTrust">Whether a wss:// connection offers raw public keys and learns a key pin on first use, and how long that waits; none for neither.</param>
        /// <param name="Pins">What the server of a wss:// connection checked by its CA is held to beside that; none for nothing.</param>
        /// <param name="HTTPVersion">The version of HTTP the WebSocket is opened over - "auto", "1.1" or "2"; none for "auto".</param>
        /// <param name="Encoding">How its messages are written - "json", "cbor" or "auto"; none for "json".</param>
        /// <param name="SignatureFormats">Which signatures it writes - "jws", "cose" or "both"; none, or "policy", for what the signature policy says.</param>
        public Boolean TryAddConnection(String?                          Description,
                                        String?                          URL,
                                        String?                          ConnectionType,
                                        Boolean?                         AutoConnect,
                                        String?                          AuthenticationId,
                                        String?                          CertificateId,
                                        [NotNullWhen(true)]  out String? Id,
                                        [NotNullWhen(false)] out String? Error,
                                        out Boolean                      NotSaved,
                                        String?                          OCPPVersion     = null,
                                        String?                          SshHostKey      = null,
                                        SshTrustSettings?                SshTrust        = null,
                                        String?                          ServerKeyPins   = null,
                                        ServerKeyPinSettings?            KeyPinTrust     = null,
                                        ServerPinsChange?                Pins            = null,
                                        String?                          HTTPVersion     = null,
                                        String?                          Encoding        = null,
                                        String?                          SignatureFormats = null)
        {

            Id        = null;
            NotSaved  = false;

            if (!ConnectionEntry.Validate(Description, URL, ConnectionType, OCPPVersion,
                                          out var url, out var type, out var version, out Error))
                return false;

            if (!ConnectionEntry.TryParseHTTPVersion(url, HTTPVersion, out var httpVersion, out Error))
                return false;

            if (!ConnectionEntry.TryParseEncoding(version, Encoding, out var encoding, out Error))
                return false;

            if (!ConnectionEntry.TryParseSignatureFormats(version, SignatureFormats, out var saysSignatureFormats, out var signatureFormats, out Error))
                return false;

            if (!ConnectionEntry.ValidateHostKey(url, SshHostKey, out var hostKey, out Error))
                return false;

            if (!ConnectionEntry.ValidateSshTrust(SshTrust?.TrustOnFirstUse, SshTrust?.OnPinMismatch, SshTrust?.ProbationSeconds,
                                                  out var trustOnFirstUse, out var onPinMismatch, out var probationTime, out Error))
                return false;

            if (!ConnectionEntry.ValidateServerKeyPins(url, ServerKeyPins, out var serverKeyPins, out Error))
                return false;

            if (!ConnectionEntry.ValidateKeyPinTrust(url, KeyPinTrust?.RawPublicKeys, KeyPinTrust?.TrustOnFirstUse, KeyPinTrust?.ProbationSeconds,
                                                     out var rawPublicKeys, out var keyTrustOnFirstUse, out var keyPinProbationTime, out Error))
                return false;

            if (!ConnectionEntry.CheckKeyPinned(serverKeyPins, rawPublicKeys, keyTrustOnFirstUse, out Error))
                return false;

            var pins = Pins?.Sent is { SaysAnything: true } sent ? sent : null;

            if (pins is not null && url.Scheme != URIScheme.wss && url.Scheme != URIScheme.https)
            {
                Error = $"'{url}' makes no TLS handshake, so its server shows no certificate to hold it to.";
                return false;
            }

            if (!ConnectionEntry.CheckOneWay(serverKeyPins, keyTrustOnFirstUse, pins, out Error))
                return false;

            lock (updateLock)
            {

                if (!CheckReferences(AuthenticationId, CertificateId, url.Scheme == URIScheme.ssh,
                                     out var authenticationId, out var certificateId, out Error))
                    return false;

                var entry = new ConnectionEntry(
                                NewId(),
                                Description!.Trim(),
                                url,
                                type,
                                TimeProvider.GetUtcNow()
                            ) {
                                OCPPVersion         = version,
                                AutoConnect  = AutoConnect ?? false,
                                AuthenticationId    = authenticationId,
                                CertificateId       = certificateId,
                                SshHostKey          = hostKey,
                                SshTrustOnFirstUse  = url.Scheme == URIScheme.ssh ? trustOnFirstUse : SshTrustOnFirstUse.None,
                                SshOnPinMismatch    = url.Scheme == URIScheme.ssh ? onPinMismatch   : SshOnPinMismatch.Refuse,
                                SshProbationTime    = probationTime,
                                ServerKeyPins              = serverKeyPins,
                                RawPublicKeys              = rawPublicKeys,
                                TrustOnFirstUse            = keyTrustOnFirstUse,
                                ServerKeyPinProbationTime  = keyPinProbationTime,
                                Pins                       = pins,
                                HTTPVersion                = httpVersion ?? WebSocketHTTPVersion.Auto,
                                Encoding                   = encoding    ?? cloud.charging.open.protocols.OCPP.WebSockets.MessageEncodings.JSON,
                                SignatureFormats           = signatureFormats
                            };

                var peersBefore = SnapshotPeers();

                connections[entry.Id] = entry;

                // A peer of its own, with this one endpoint: the primary if it
                // dials by itself and nothing else is in charge, an observer if
                // it dials by itself, a service if not.
                var peer = NewPeerOf(entry);
                peer.Role = !entry.AutoConnect
                                ? PeerRole.Service
                                : peers.Values.Any(other => other.Role == PeerRole.Primary)
                                      ? PeerRole.Observer
                                      : PeerRole.Primary;
                peers[peer.Id] = peer;

                var notes = Normalize();

                if (!TrySaveConnections(out Error))
                {
                    connections.Remove(entry.Id);
                    RestorePeers(peersBefore);
                    NotSaved = true;
                    return false;
                }

                Notice(LogLevel.Info, $"Connection written down: {entry}, as peer {peer}.");
                Tell(notes, LogLevel.Notice);

                Id = entry.Id;
                return true;

            }

        }

        #endregion

        #region TryUpdateConnection(Id, Description, URL, ConnectionType, ..., out Error [, out NotSaved])

        /// <summary>
        /// Change one connection.
        /// </summary>
        public Boolean TryUpdateConnection(String?                          Id,
                                           String?                          Description,
                                           String?                          URL,
                                           String?                          ConnectionType,
                                           Boolean?                         AutoConnect,
                                           String?                          AuthenticationId,
                                           String?                          CertificateId,
                                           [NotNullWhen(false)] out String? Error,
                                           String?                          OCPPVersion            = null,
                                           String?                          SshHostKey             = null,
                                           String?                          SshHostKeyAsShown      = null,
                                           SshTrustSettings?                SshTrust               = null,
                                           String?                          ServerKeyPins          = null,
                                           String?                          ServerKeyPinsAsShown   = null,
                                           ServerKeyPinSettings?            KeyPinTrust            = null,
                                           ServerPinsChange?                Pins                   = null,
                                           String?                          HTTPVersion            = null,
                                           String?                          Encoding               = null,
                                           String?                          SignatureFormats       = null)

            => TryUpdateConnection(Id, Description, URL, ConnectionType, AutoConnect, AuthenticationId, CertificateId,
                                   out Error, out _, OCPPVersion, SshHostKey, SshHostKeyAsShown, SshTrust,
                                   ServerKeyPins, ServerKeyPinsAsShown, KeyPinTrust, Pins, HTTPVersion, Encoding, SignatureFormats);

        /// <summary>
        /// Change one connection - and say whether a refusal was the file's
        /// rather than the change's.
        /// </summary>
        /// <param name="NotSaved">True where the file of the connections could not be written: nothing about the change was wrong, and the connection stays as it was.</param>
        /// <param name="SshHostKey">The host keys the server of an ssh:// connection may show, separated by commas: "" for none, and nothing to keep those there are.</param>
        /// <param name="SshHostKeyAsShown">The host keys the page showed when it was drawn. Where <paramref name="SshHostKey"/> is still that, those there are now stay: a signed pin set taken meanwhile is not undone by a change of something else.</param>
        /// <param name="SshTrust">How an ssh:// connection believes its server beyond its pins; nothing to keep what it says.</param>
        /// <param name="ServerKeyPins">The key pins of the keys the server of a wss:// connection may show, separated by commas: "" for none, and nothing to keep those there are.</param>
        /// <param name="ServerKeyPinsAsShown">The key pins the page showed when it was drawn. Where <paramref name="ServerKeyPins"/> are still those, those there are now stay: a signed pin set taken meanwhile is not undone by a change of something else.</param>
        /// <param name="KeyPinTrust">Whether a wss:// connection offers raw public keys and learns a key pin on first use, and how long that waits; nothing to keep what it says.</param>
        /// <param name="Pins">What the server of a wss:// connection checked by its CA is held to beside that; nothing to keep what it is held to.</param>
        /// <param name="HTTPVersion">The version of HTTP the WebSocket is opened over - "auto", "1.1" or "2"; nothing to keep the one it has.</param>
        /// <param name="Encoding">How its messages are written - "json", "cbor" or "auto"; nothing to keep what it says.</param>
        /// <param name="SignatureFormats">Which signatures it writes - "jws", "cose", "both", or "policy" for what the signature policy says; nothing to keep what it says.</param>
        public Boolean TryUpdateConnection(String?                          Id,
                                           String?                          Description,
                                           String?                          URL,
                                           String?                          ConnectionType,
                                           Boolean?                         AutoConnect,
                                           String?                          AuthenticationId,
                                           String?                          CertificateId,
                                           [NotNullWhen(false)] out String? Error,
                                           out Boolean                      NotSaved,
                                           String?                          OCPPVersion            = null,
                                           String?                          SshHostKey             = null,
                                           String?                          SshHostKeyAsShown      = null,
                                           SshTrustSettings?                SshTrust               = null,
                                           String?                          ServerKeyPins          = null,
                                           String?                          ServerKeyPinsAsShown   = null,
                                           ServerKeyPinSettings?            KeyPinTrust            = null,
                                           ServerPinsChange?                Pins                   = null,
                                           String?                          HTTPVersion            = null,
                                           String?                          Encoding               = null,
                                           String?                          SignatureFormats       = null)
        {

            NotSaved = false;

            if (!ConnectionEntry.Validate(Description, URL, ConnectionType, OCPPVersion,
                                          out var url, out var type, out var version, out Error))
                return false;

            if (!ConnectionEntry.TryParseHTTPVersion(url, HTTPVersion, out var httpVersion, out Error))
                return false;

            if (!ConnectionEntry.TryParseEncoding(version, Encoding, out var encoding, out Error))
                return false;

            if (!ConnectionEntry.TryParseSignatureFormats(version, SignatureFormats, out var saysSignatureFormats, out var signatureFormats, out Error))
                return false;

            // Whether the host key is to stay as it is: left out, or sent back
            // as the page showed it.
            var keepsHostKey = SshHostKey is null ||
                               (SshHostKeyAsShown is not null &&
                                ConnectionEntry.Named(SshHostKey) == ConnectionEntry.Named(SshHostKeyAsShown));

            String? hostKey = null;

            if (!keepsHostKey && !ConnectionEntry.ValidateHostKey(url, SshHostKey, out hostKey, out Error))
                return false;

            var trustOnFirstUse  = SshTrustOnFirstUse.None;
            var onPinMismatch    = SshOnPinMismatch.Refuse;
            var probationTime    = ConnectionEntry.DefaultSshProbationTime;

            if (SshTrust is not null &&
                !ConnectionEntry.ValidateSshTrust(SshTrust.TrustOnFirstUse, SshTrust.OnPinMismatch, SshTrust.ProbationSeconds,
                                                  out trustOnFirstUse, out onPinMismatch, out probationTime, out Error))
                return false;

            // The same for the key pins of a wss:// connection: left out, or sent
            // back as the page showed them.
            var keepsKeyPins = ServerKeyPins is null ||
                               (ServerKeyPinsAsShown is not null && SamePins(ServerKeyPins, ServerKeyPinsAsShown));

            IReadOnlyList<KeyPin> keyPins = [];

            if (!keepsKeyPins && !ConnectionEntry.ValidateServerKeyPins(url, ServerKeyPins, out keyPins, out Error))
                return false;

            var rawPublicKeys        = RawPublicKeyMode.Off;
            var keyTrustOnFirstUse   = KeyTrustOnFirstUse.None;
            var keyPinProbationTime  = ConnectionEntry.DefaultServerKeyPinProbationTime;

            if (KeyPinTrust is not null &&
                !ConnectionEntry.ValidateKeyPinTrust(url, KeyPinTrust.RawPublicKeys, KeyPinTrust.TrustOnFirstUse, KeyPinTrust.ProbationSeconds,
                                                     out rawPublicKeys, out keyTrustOnFirstUse, out keyPinProbationTime, out Error))
                return false;

            var overTLS = url.Scheme == URIScheme.wss || url.Scheme == URIScheme.https;

            lock (updateLock)
            {

                if (Id is null || !connections.TryGetValue(Id, out var entry))
                {
                    Error = "There is no connection here under that identification.";
                    return false;
                }

                if (!CheckReferences(AuthenticationId, CertificateId, url.Scheme == URIScheme.ssh,
                                     out var authenticationId, out var certificateId, out Error))
                    return false;

                // A host key kept goes with a connection that is no longer
                // ssh://: there is nothing left that would check it.
                if (keepsHostKey)
                    hostKey = url.Scheme == URIScheme.ssh ? entry.SshHostKey : null;

                // So do key pins with a connection that is no longer wss://, and
                // what it said of raw public keys and of trust on first use.
                if (keepsKeyPins)
                    keyPins = overTLS ? entry.ServerKeyPins : [];

                if (KeyPinTrust is null)
                {
                    rawPublicKeys        = overTLS ? entry.RawPublicKeys   : RawPublicKeyMode.Off;
                    keyTrustOnFirstUse   = overTLS ? entry.TrustOnFirstUse : KeyTrustOnFirstUse.None;
                    keyPinProbationTime  = entry.ServerKeyPinProbationTime;
                }

                if (!ConnectionEntry.CheckKeyPinned(keyPins, rawPublicKeys, keyTrustOnFirstUse, out Error))
                    return false;

                // What it is held to: as it is, where nothing was sent; what
                // the page changed of what it showed, where it said what it
                // showed, so that a root learned or a signed pin set taken
                // meanwhile stays; and what was sent otherwise. All of it goes
                // with a connection that is no longer wss://.
                var pins = entry.Pins;

                if (!overTLS)
                {

                    if (Pins?.Sent is { SaysAnything: true })
                    {
                        Error = $"'{url}' makes no TLS handshake, so its server shows no certificate to hold it to.";
                        return false;
                    }

                    pins = null;

                }

                else if (Pins is not null)
                {

                    if (!Pins.SaysWhatWasShown)
                        pins = Pins.Sent is { SaysAnything: true } sent ? sent : null;

                    else if (!ServerPins.TryAfterAPageSaved(entry.Pins, Pins.Shown, Pins.Sent, $"connection '{entry.Id}'.{ConnectionEntry.PinsKey}", out pins, out Error))
                        return false;

                }

                if (!ConnectionEntry.CheckOneWay(keyPins, keyTrustOnFirstUse, pins, out Error))
                    return false;

                var peer = PeerOfUnlocked(entry.Id);

                if (peer is not null && peer.EndpointIds.Count > 1 && version != peer.OCPPVersion)
                {
                    Error = $"Every endpoint of a peer speaks one OCPP version, and '{peer.Description}' speaks {ConnectionEntry.AsText(peer.OCPPVersion)}. " +
                            "Make this connection a peer of its own first.";
                    return false;
                }

                var peersBefore = SnapshotPeers();
                var was = entry.Copy();

                entry.Description         = Description!.Trim();
                entry.URL                 = url;
                entry.ConnectionType      = type;
                entry.OCPPVersion         = version;
                entry.AutoConnect  = AutoConnect ?? false;
                entry.AuthenticationId    = authenticationId;
                entry.CertificateId       = certificateId;

                // Kept where the change does not say, and nothing of an
                // ssh:// connection, which a ws:// one may have become.
                entry.HTTPVersion         = url.Scheme == URIScheme.ssh
                                                ? WebSocketHTTPVersion.Auto
                                                : httpVersion ?? entry.HTTPVersion;

                // Kept where the change does not say.
                entry.Encoding            = encoding ?? entry.Encoding;

                // Kept where the change does not say; "policy" says that the
                // connection says nothing any more.
                entry.SignatureFormats    = saysSignatureFormats
                                                ? signatureFormats
                                                : entry.SignatureFormats;

                // A key learned on probation goes with the pins somebody wrote,
                // and with a server elsewhere; so does the version of the
                // signed pin set the pins came from.
                if (!keepsHostKey || was.URL != url)
                {
                    entry.SshLearnedHostKey   = null;
                    entry.SshLearnedAt        = null;
                }

                if (!keepsHostKey)
                    entry.SshHostKeysVersion  = 0;

                entry.SshHostKey          = hostKey;

                if (url.Scheme != URIScheme.ssh)
                {
                    entry.SshTrustOnFirstUse  = SshTrustOnFirstUse.None;
                    entry.SshOnPinMismatch    = SshOnPinMismatch.Refuse;
                    entry.SshLearnedHostKey   = null;
                    entry.SshLearnedAt        = null;
                }
                else if (SshTrust is not null)
                {
                    entry.SshTrustOnFirstUse  = trustOnFirstUse;
                    entry.SshOnPinMismatch    = onPinMismatch;
                    entry.SshProbationTime    = probationTime;
                }

                // A key pin learned on probation goes with the pins somebody
                // wrote, with a server elsewhere, and with trust on first use
                // switched off; the version of the signed pin set, and that it
                // confirmed what was learned, go with the pins.
                if (!keepsKeyPins || was.URL != url || keyTrustOnFirstUse == KeyTrustOnFirstUse.None || keyPins.Count > 0)
                {
                    entry.ServerKeyPinLearned    = null;
                    entry.ServerKeyPinLearnedAt  = null;
                }

                if (!keepsKeyPins || keyPins.Count == 0)
                {
                    entry.ServerKeyPinsVersion    = 0;
                    entry.ServerKeyPinsConfirmed  = false;
                }

                entry.ServerKeyPins              = keyPins;
                entry.RawPublicKeys              = rawPublicKeys;
                entry.TrustOnFirstUse            = keyTrustOnFirstUse;
                entry.ServerKeyPinProbationTime  = keyPinProbationTime;

                // The version of the signed pin set goes with roots somebody
                // wrote: what a hand changed is no longer what was signed.
                if (pins is not { Roots.Count: > 0 } ||
                    !pins.Roots.SequenceEqual(was.Pins?.Roots ?? [], StringComparer.OrdinalIgnoreCase))
                {
                    entry.PinsVersion            = 0;
                }

                entry.Pins                       = pins;

                // A peer of this one endpoint follows it: its version, its kind
                // where that came from the connection's type, its description
                // where that was the connection's.
                if (peer is not null && peer.EndpointIds.Count == 1)
                {

                    peer.OCPPVersion = version;

                    if (peer.Kind is PeerKind.CSMS or PeerKind.LocalController)
                        peer.Kind = PeerEntry.KindOf(type);

                    if (peer.Description == was.Description)
                        peer.Description = entry.Description;

                }

                var notes = Normalize();

                Int64? revisionWas = ChangesHowItIsDialled(was, entry) ? BumpDialRevision(Id) : null;

                if (!TrySaveConnections(out Error))
                {
                    connections[Id] = was;
                    RestorePeers(peersBefore);
                    if (revisionWas is Int64 revision)
                        dialRevisions[Id] = revision;
                    NotSaved = true;
                    return false;
                }

                Notice(LogLevel.Info, $"Connection changed: {entry}.");
                Tell(notes, LogLevel.Notice);

                Error = null;
                return true;

            }

        }

        #endregion

        #region TrySetCertificate(Id, CertificateId, out Error)

        /// <summary>
        /// Have one connection show the certificate of another key, and nothing
        /// else about it changed: what CertificateSigned does once the
        /// certificate a station asked its CSMS for has come.
        /// </summary>
        /// <remarks>
        /// The connection is dialled again, as after a change of its
        /// certificate by somebody's hand.
        /// </remarks>
        /// <param name="Id">The connection.</param>
        /// <param name="CertificateId">The key whose certificate it shows from now on.</param>
        /// <param name="Error">Why it was not changed.</param>
        public Boolean TrySetCertificate(String                           Id,
                                         String                           CertificateId,
                                         [NotNullWhen(false)] out String? Error)
        {

            lock (updateLock)
            {

                if (!connections.TryGetValue(Id, out var entry))
                {
                    Error = $"There is no connection '{Id}' here.";
                    return false;
                }

                if (!entry.IsSecure || entry.IsSSH)
                {
                    Error = $"'{entry.Description}' makes no TLS handshake a certificate could be shown in.";
                    return false;
                }

                if (KnownCertificateIds is not null && !KnownCertificateIds().Contains(CertificateId))
                {
                    Error = $"This {NodeWording.Noun} has no client key '{CertificateId}'.";
                    return false;
                }

                if (entry.CertificateId == CertificateId)
                {
                    Error = null;
                    return true;
                }

                var was          = entry.Copy();
                var revisionWas  = BumpDialRevision(Id);

                entry.CertificateId = CertificateId;

                if (!TrySaveConnections(out Error))
                {
                    connections[Id]    = was;
                    dialRevisions[Id]  = revisionWas;
                    return false;
                }

                Notice(LogLevel.Notice,
                       $"'{entry.Description}' shows the certificate of the key {CertificateId} from now on" +
                       (was.CertificateId is not null ? $", and no longer that of {was.CertificateId}" : "") +
                        ". It is dialled again.");

                Error = null;
                return true;

            }

        }

        #endregion

        #region TryLearnHostKey(Id, URL, HostKey, out Error)

        /// <summary>
        /// Write down the host key the first connection of an ssh:// connection
        /// that trusts on first use was shown: on probation, until a signed pin
        /// set confirms it (white paper "OCPP over SSH", section 10.4).
        /// </summary>
        /// <remarks>
        /// Only where nothing changed meanwhile: the connection is still there,
        /// still goes where it went when it was dialled, still trusts on first
        /// use, and pins and learned no host key yet. Written down rather than
        /// only remembered, so that a restart does not learn again from whoever
        /// answers first - and so that the probation runs on.
        /// </remarks>
        /// <param name="Id">The connection.</param>
        /// <param name="URL">Where it went when it was dialled.</param>
        /// <param name="HostKey">The host key it was shown, as a fingerprint.</param>
        /// <param name="Error">Why it was not written down - nothing where it was not to be.</param>
        /// <returns>Whether it was written down.</returns>
        public Boolean TryLearnHostKey(String       Id,
                                       URL          URL,
                                       String       HostKey,
                                       out String?  Error)
        {

            Error = null;

            lock (updateLock)
            {

                if (!connections.TryGetValue(Id, out var entry)                  ||
                    entry.URL != URL                                             ||
                    entry.URL.Scheme != URIScheme.ssh                            ||
                    entry.SshTrustOnFirstUse != SshTrustOnFirstUse.HostKey       ||
                    entry.SshHostKey is not null                                 ||
                    entry.SshLearnedHostKey is not null)
                {
                    return false;
                }

                entry.SshLearnedHostKey  = HostKey;
                entry.SshLearnedAt       = TimeProvider.GetUtcNow();

                if (!TrySaveConnections(out Error))
                {
                    entry.SshLearnedHostKey  = null;
                    entry.SshLearnedAt       = null;
                    return false;
                }

                Notice(LogLevel.Notice,
                                 $"'{entry.Description}' pins no host key and trusts on first use, so the one its server showed on the first " +
                                 $"connection was learned: {HostKey}. It is on probation for {entry.SshProbationTime.TotalMinutes:0.#} minute(s), " +
                                  "until a signed pin set confirms it; until then nothing but such a pin set is executed.");

                return true;

            }

        }

        #endregion

        #region TryChangeSshTrust(Id, Settings, out Error)

        /// <summary>
        /// Change how a connection over SSH believes its server beyond its pins - what a signed pin set of
        /// SSHClientController says: what is not said stays.
        /// </summary>
        public Boolean TryChangeSshTrust(String                            Id,
                                         SshTrustSettings                  Settings,
                                         [NotNullWhen(false)] out String?  Error)
        {

            lock (updateLock)
            {

                if (!connections.TryGetValue(Id, out var entry) || !entry.IsSSH)
                {
                    Error = $"There is no connection over SSH '{Id}' here.";
                    return false;
                }

                if (!ConnectionEntry.ValidateSshTrust(Settings.TrustOnFirstUse  ?? entry.SshTrustOnFirstUse.ToString(),
                                                      Settings.OnPinMismatch    ?? entry.SshOnPinMismatch.  ToString(),
                                                      Settings.ProbationSeconds ?? (Int64) entry.SshProbationTime.TotalSeconds,
                                                      out var trustOnFirstUse,
                                                      out var onPinMismatch,
                                                      out var probationTime,
                                                      out Error))
                    return false;

                var was = entry.Copy();

                entry.SshTrustOnFirstUse  = trustOnFirstUse;
                entry.SshOnPinMismatch    = onPinMismatch;
                entry.SshProbationTime    = probationTime;

                if (!TrySaveConnections(out Error))
                {
                    connections[Id] = was;
                    return false;
                }

                Notice(LogLevel.Notice,
                                 $"'{entry.Description}' believes its server as a signed pin set said: trust on first use {trustOnFirstUse}, " +
                                 $"a mismatch {onPinMismatch}, a probation of {probationTime.TotalSeconds:0} s.");

                Error = null;
                return true;

            }

        }

        #endregion

        #region TryForgetLearnedHostKey(Id, HostKey)

        /// <summary>
        /// Forget the host key learned on first use - after a probation that
        /// ran out without a signed pin set - so that the next connection
        /// learns again.
        /// </summary>
        /// <param name="Id">The connection.</param>
        /// <param name="HostKey">The host key learned, so that one learned again meanwhile is not forgotten.</param>
        /// <returns>Whether it was forgotten.</returns>
        public Boolean TryForgetLearnedHostKey(String  Id,
                                               String  HostKey)
        {

            lock (updateLock)
            {

                if (!connections.TryGetValue(Id, out var entry) || entry.SshLearnedHostKey != HostKey)
                    return false;

                var learnedAt = entry.SshLearnedAt;

                entry.SshLearnedHostKey  = null;
                entry.SshLearnedAt       = null;

                if (!TrySaveConnections(out var error))
                {
                    entry.SshLearnedHostKey  = HostKey;
                    entry.SshLearnedAt       = learnedAt;
                    Notice(LogLevel.Warning, $"'{entry.Description}' could not forget the host key it learned: {error}.");
                    return false;
                }

                Notice(LogLevel.Warning,
                                 $"'{entry.Description}' forgot the host key {HostKey} it learned on its first connection: no signed pin set " +
                                  "confirmed it in time. The next connection learns again.");

                return true;

            }

        }

        #endregion

        #region TryTakeSignedHostKeys(Id, HostKeys, Version, out Confirmed, out Error)

        /// <summary>
        /// Take the host keys of a signed pin set as the connection's pins: only
        /// with a higher version than the one it holds, so that an older pin
        /// set cannot be brought back (white paper "OCPP over SSH", section
        /// 10.4; "Key Pins and Raw Public Keys", section 5.7).
        /// </summary>
        /// <param name="Id">The connection.</param>
        /// <param name="HostKeys">The host keys of the pin set, separated by commas.</param>
        /// <param name="Version">The version of the pin set.</param>
        /// <param name="Learned">The host key that was learned on probation and is among them, or the one that is not; null where none was learned.</param>
        /// <param name="Confirmed">Whether the host key learned on probation is among them.</param>
        /// <param name="Error">Why they were not taken.</param>
        public Boolean TryTakeSignedHostKeys(String                           Id,
                                             String                           HostKeys,
                                             UInt64                           Version,
                                             out String?                      Learned,
                                             out Boolean                      Confirmed,
                                             [NotNullWhen(false)] out String? Error)
        {

            Learned    = null;
            Confirmed  = false;

            lock (updateLock)
            {

                if (!connections.TryGetValue(Id, out var entry))
                {
                    Error = $"There is no connection '{Id}' here.";
                    return false;
                }

                if (!ConnectionEntry.ValidateHostKey(entry.URL, HostKeys, out var hostKey, out Error))
                    return false;

                if (hostKey is null)
                {
                    Error = "A signed pin set without a host key would leave the connection with nothing to believe its server by.";
                    return false;
                }

                if (Version <= entry.SshHostKeysVersion)
                {
                    Error = $"The pin set has version {Version}, and the connection holds version {entry.SshHostKeysVersion}: " +
                             "only a higher one is taken, so that an older pin set cannot be brought back.";
                    return false;
                }

                var was     = entry.Copy();

                var learned = entry.SshLearnedHostKey;

                Learned     = learned;
                Confirmed   = learned is not null &&
                              ConnectionEntry.SplitHostKeys(hostKey).Any(pin => String.Equals(pin, learned, StringComparison.Ordinal));

                entry.SshHostKey          = hostKey;
                entry.SshHostKeysVersion  = Version;
                entry.SshLearnedHostKey   = null;
                entry.SshLearnedAt        = null;

                if (!TrySaveConnections(out Error))
                {
                    connections[Id] = was;
                    return false;
                }

                Notice(LogLevel.Notice,
                                 $"'{entry.Description}' took the host keys of the signed pin set of version {Version}: {hostKey}" +
                                 (Learned is null
                                      ? "."
                                      : Confirmed
                                            ? $"; they confirm the one learned on its first connection ({Learned}), and its probation ends."
                                            : $"; the one learned on its first connection ({Learned}) is not among them, so that server is not believed any more."));

                Error = null;
                return true;

            }

        }

        #endregion


        #region TryLearnServerKeyPin(Id, URL, Pin, out Error)

        /// <summary>
        /// Write down the key pin of the key the first handshake of a wss://
        /// connection that trusts on first use was shown: on probation, until a
        /// signed pin set confirms it ("Key Pins and Raw Public Keys", section
        /// 5.6).
        /// </summary>
        /// <remarks>
        /// Only where nothing changed meanwhile: the connection is still there,
        /// still goes where it went when it was dialled, still trusts on first
        /// use, and pins and learned no key yet. Written down rather than only
        /// remembered, so that a restart does not learn again from whoever
        /// answers first - and so that the probation runs on.
        /// </remarks>
        /// <param name="Id">The connection.</param>
        /// <param name="URL">Where it went when it was dialled.</param>
        /// <param name="Pin">The key pin of the key it was shown.</param>
        /// <param name="Error">Why it was not written down - nothing where it was not to be.</param>
        /// <returns>Whether it was written down.</returns>
        public Boolean TryLearnServerKeyPin(String       Id,
                                            URL          URL,
                                            KeyPin       Pin,
                                            out String?  Error)
        {

            Error = null;

            lock (updateLock)
            {

                if (!connections.TryGetValue(Id, out var entry)                     ||
                    entry.URL != URL                                                ||
                    !entry.IsSecure                                                 ||
                    entry.TrustOnFirstUse != KeyTrustOnFirstUse.Key                 ||
                    entry.ServerKeyPins.Count > 0                                   ||
                    entry.ServerKeyPinLearned is not null)
                {
                    return false;
                }

                entry.ServerKeyPinLearned    = Pin;
                entry.ServerKeyPinLearnedAt  = TimeProvider.GetUtcNow();

                if (!TrySaveConnections(out Error))
                {
                    entry.ServerKeyPinLearned    = null;
                    entry.ServerKeyPinLearnedAt  = null;
                    return false;
                }

                Notice(LogLevel.Notice,
                       $"'{entry.Description}' pins no key and trusts on first use, so the key pin of the one its server showed on the first " +
                       $"connection was learned: {Pin}. It is on probation for {entry.ServerKeyPinProbationTime.TotalMinutes:0.#} minute(s), " +
                        "until a signed pin set confirms it; until then nothing but such a pin set is executed.");

                return true;

            }

        }

        #endregion

        #region TryForgetLearnedServerKeyPin(Id, Pin)

        /// <summary>
        /// Forget the key pin learned on first use - after a probation that ran
        /// out without a signed pin set - so that the next connection learns
        /// again.
        /// </summary>
        /// <param name="Id">The connection.</param>
        /// <param name="Pin">The key pin learned, so that one learned again meanwhile is not forgotten.</param>
        /// <returns>Whether it was forgotten.</returns>
        public Boolean TryForgetLearnedServerKeyPin(String  Id,
                                                    KeyPin  Pin)
        {

            lock (updateLock)
            {

                if (!connections.TryGetValue(Id, out var entry) || entry.ServerKeyPinLearned != Pin)
                    return false;

                var learnedAt = entry.ServerKeyPinLearnedAt;

                entry.ServerKeyPinLearned    = null;
                entry.ServerKeyPinLearnedAt  = null;

                if (!TrySaveConnections(out var error))
                {
                    entry.ServerKeyPinLearned    = Pin;
                    entry.ServerKeyPinLearnedAt  = learnedAt;
                    Notice(LogLevel.Warning, $"'{entry.Description}' could not forget the key pin it learned: {error}.");
                    return false;
                }

                Notice(LogLevel.Warning,
                       $"'{entry.Description}' forgot the key pin {Pin} it learned on its first connection: no signed pin set " +
                        "confirmed it in time. The next connection learns again.");

                return true;

            }

        }

        #endregion

        #region TryTakeSignedServerKeyPins(Id, Pins, Version, out Learned, out Confirmed, out Error)

        /// <summary>
        /// Take the key pins of a signed pin set as the pins of a wss://
        /// connection: only with a higher version than the one it holds, so that
        /// an older pin set cannot be brought back ("Key Pins and Raw Public
        /// Keys", sections 5.6 and 5.7).
        /// </summary>
        /// <remarks>
        /// Who signed it is not asked here: that is asked where the pin set
        /// arrives, of the signers this station believes. A pin set that has the
        /// key pin learned on probation confirms it, and the probation ends; one
        /// that has not replaces it - the server that answered first is not the
        /// one the CSMS knows, and the signed pins are what the next connection
        /// is held to.
        /// </remarks>
        /// <param name="Id">The connection.</param>
        /// <param name="Pins">The key pins of the pin set, separated by commas.</param>
        /// <param name="Version">The version of the pin set.</param>
        /// <param name="Learned">The key pin that was learned on probation, among them or not; null where none was learned.</param>
        /// <param name="Confirmed">Whether the key pin learned on probation is among them.</param>
        /// <param name="Error">Why they were not taken.</param>
        public Boolean TryTakeSignedServerKeyPins(String                           Id,
                                                  String?                          Pins,
                                                  UInt64                           Version,
                                                  out KeyPin?                      Learned,
                                                  out Boolean                      Confirmed,
                                                  [NotNullWhen(false)] out String? Error)
        {

            Learned    = null;
            Confirmed  = false;

            lock (updateLock)
            {

                if (!connections.TryGetValue(Id, out var entry))
                {
                    Error = $"There is no connection '{Id}' here.";
                    return false;
                }

                if (!entry.IsSecure)
                {
                    Error = $"'{entry.Description}' makes no TLS handshake, so there is no key of its server to pin.";
                    return false;
                }

                if (!ConnectionEntry.ValidateServerKeyPins(entry.URL, Pins, out var pins, out Error))
                    return false;

                if (pins.Count == 0)
                {
                    Error = "A signed pin set without a key pin would leave the connection with nothing to believe its server by.";
                    return false;
                }

                if (Version <= entry.ServerKeyPinsVersion)
                {
                    Error = $"The pin set has version {Version}, and the connection holds version {entry.ServerKeyPinsVersion}: " +
                             "only a higher one is taken, so that an older pin set cannot be brought back.";
                    return false;
                }

                if (!ConnectionEntry.CheckKeyPinned(pins, entry.RawPublicKeys, entry.TrustOnFirstUse, out Error))
                    return false;

                var was      = entry.Copy();

                var learned  = entry.ServerKeyPinLearned;

                Learned      = learned;
                Confirmed    = learned is not null && pins.Contains(learned);

                entry.ServerKeyPins           = pins;
                entry.ServerKeyPinsVersion    = Version;
                entry.ServerKeyPinsConfirmed  = Confirmed;
                entry.ServerKeyPinLearned     = null;
                entry.ServerKeyPinLearnedAt   = null;

                if (!TrySaveConnections(out Error))
                {
                    connections[Id] = was;
                    return false;
                }

                Notice(LogLevel.Notice,
                       $"'{entry.Description}' took the key pins of the signed pin set of version {Version}: {ConnectionEntry.AsText(pins)}" +
                       (Learned is null
                            ? "."
                            : Confirmed
                                  ? $"; they confirm the one learned on its first connection ({Learned}), and its probation ends."
                                  : $"; the one learned on its first connection ({Learned}) is not among them, so that server is not believed any more."));

                Error = null;
                return true;

            }

        }

        #endregion

        #region TryLearnPin(Id, URL, What, Fingerprint)

        /// <summary>
        /// Hold the server of a wss:// connection to the root or to the
        /// certificate it was first believed with: what trust on first use
        /// makes of a connection checked by its CA, written into its entry like
        /// a pin somebody typed.
        /// </summary>
        /// <remarks>
        /// Only where nothing changed meanwhile: the connection is still there,
        /// still goes where it went when it was dialled, still learns that, and
        /// is held to nothing of that kind yet. No probation, unlike a key
        /// learned on first use: the CA vouched for what is learned here.
        /// </remarks>
        /// <param name="Id">The connection.</param>
        /// <param name="URL">Where it went when it was dialled.</param>
        /// <param name="What">What is learned: the root or the certificate.</param>
        /// <param name="Fingerprint">Its fingerprint.</param>
        /// <returns>Whether the server is held to it now.</returns>
        public Boolean TryLearnPin(String           Id,
                                   URL              URL,
                                   protocols.WWCP.Node.Configuration.TrustOnFirstUse  What,
                                   String           Fingerprint)
        {

            lock (updateLock)
            {

                if (!connections.TryGetValue(Id, out var entry) ||
                    entry.URL != URL                            ||
                    !entry.IsSecure                             ||
                    entry.IsKeyPinned                           ||
                    entry.Pins is not ServerPins pins           ||
                    pins.TrustOnFirstUse != What)
                {
                    return false;
                }

                // Learned already - by another handshake, or by somebody's hand.
                if (What switch {
                        protocols.WWCP.Node.Configuration.TrustOnFirstUse.Root         => pins.Roots.       Count > 0,
                        protocols.WWCP.Node.Configuration.TrustOnFirstUse.Certificate  => pins.Certificates.Count > 0,
                        _                                                              => true
                    })
                {
                    return pins.Roots.Contains(Fingerprint, StringComparer.OrdinalIgnoreCase) ||
                           pins.Certificates.Contains(Fingerprint, StringComparer.OrdinalIgnoreCase);
                }

                entry.Pins = pins.Learning(What, Fingerprint);

                if (!TrySaveConnections(out _))
                {
                    entry.Pins = pins;
                    return false;
                }

                Notice(LogLevel.Notice,
                       $"'{entry.Description}' is held to the {What.AsText()} {Fingerprint} from now on: the first one its server was believed with, trusted on first use.");

                return true;

            }

        }

        #endregion

        #region TryTakeSignedServerCAKeyPins(Id, Pins, Version, out Error)

        /// <summary>
        /// Take the key pins of a signed pin set as the roots the chain of the
        /// server of a wss:// connection may end at: only with a higher version
        /// than the one it holds, so that an older pin set cannot be brought
        /// back.
        /// </summary>
        /// <remarks>
        /// Who signed it is not asked here: that is asked where the pin set
        /// arrives, of the signers this station believes. The pins are those of
        /// keys - the SHA-256 over the SubjectPublicKeyInfo of a CA, as the key
        /// pins of a server are written - and replace every root the connection
        /// was held to; what else it is held to stays. Not for a connection
        /// whose server is known by its key: that one asks no CA.
        /// </remarks>
        /// <param name="Id">The connection.</param>
        /// <param name="Pins">The key pins of the roots, separated by commas.</param>
        /// <param name="Version">The version of the pin set.</param>
        /// <param name="Error">Why they were not taken.</param>
        public Boolean TryTakeSignedServerCAKeyPins(String                           Id,
                                                    String?                          Pins,
                                                    UInt64                           Version,
                                                    [NotNullWhen(false)] out String? Error)
        {

            lock (updateLock)
            {

                if (!connections.TryGetValue(Id, out var entry))
                {
                    Error = $"There is no connection '{Id}' here.";
                    return false;
                }

                if (!entry.IsSecure)
                {
                    Error = $"'{entry.Description}' makes no TLS handshake, so there is no chain of its server to hold to a root.";
                    return false;
                }

                if (entry.IsKeyPinned)
                {
                    Error = $"'{entry.Description}' knows its server by the key of that server and asks no CA: a root it were held to would never be asked.";
                    return false;
                }

                var roots = new List<String>();

                foreach (var text in (Pins ?? "").Split([ ',', ';', '\n' ], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {

                    if (!KeyPin.TryParse(text, out var keyPin))
                    {
                        Error = $"'{text}' is not a key pin: 64 hexadecimal digits, with or without colons between the bytes, or 'sha256//' and the base64 of 32 bytes.";
                        return false;
                    }

                    var root = ServerPins.KeyPinPrefix + keyPin.ToHex().ToLowerInvariant();

                    if (!roots.Contains(root))
                        roots.Add(root);

                }

                if (roots.Count == 0)
                {
                    Error = "A signed pin set without a key pin would leave the connection held to no root.";
                    return false;
                }

                if (roots.Count > ServerPins.MaxPins)
                {
                    Error = $"A connection is held to at most {ServerPins.MaxPins} roots, and the pin set has {roots.Count}.";
                    return false;
                }

                if (Version <= entry.PinsVersion)
                {
                    Error = $"The pin set has version {Version}, and the connection holds version {entry.PinsVersion}: " +
                             "only a higher one is taken, so that an older pin set cannot be brought back.";
                    return false;
                }

                var was         = entry.Copy();
                var revisionWas = BumpDialRevision(Id);

                entry.Pins         = (entry.Pins ?? ServerPins.None) with { Roots = roots };
                entry.PinsVersion  = Version;

                if (!TrySaveConnections(out Error))
                {
                    connections[Id]    = was;
                    dialRevisions[Id]  = revisionWas;
                    return false;
                }

                Notice(LogLevel.Notice,
                       $"'{entry.Description}' took the roots of the signed pin set of version {Version}: its chain may end at {String.Join(" or ", roots)}. " +
                        "It is dialled again, held to them.");

                Error = null;
                return true;

            }

        }

        #endregion

        #region TryChangeKeyPinTrust(Id, Settings, out Forgotten, out KeyPinnedChanged, out Error)

        /// <summary>
        /// Change how a wss:// connection believes its server beyond its key
        /// pins - what a signed SetVariables of TLSClientController says of raw
        /// public keys, trust on first use and the probation: what is not said
        /// stays ("Key Pins and Raw Public Keys", section 7.2).
        /// </summary>
        /// <remarks>
        /// <para>
        /// What <see cref="TryChangeSshTrust"/> is for a connection over SSH,
        /// checked as the same change on the page is: raw public keys only where
        /// the connection is key-pinned. A key pin learned on probation goes
        /// where trust on first use is switched off, as on the page - nothing
        /// would learn it any more - and is handed back, so that the station
        /// ends the probation that went with it.
        /// </para>
        /// <para>
        /// Not dialled again here, even where the change makes a connection
        /// key-pinned that was checked by its CA, or the other way round - whose
        /// client was made with the validator of the one or the other, and would
        /// go on with it until the next start. That is said in
        /// <paramref name="KeyPinnedChanged"/>, and the caller dials it again
        /// with <see cref="DialAgain"/> once it answered what asked for the
        /// change, over the very connection that is hung up then. Every other
        /// change is read at the next handshake, as the key pins are, and drops
        /// nothing.
        /// </para>
        /// </remarks>
        /// <param name="Id">The connection.</param>
        /// <param name="Settings">What changes: "Off", "Prefer" or "Require"; "None" or "Key"; seconds of probation.</param>
        /// <param name="Forgotten">The key pin learned on probation that went with trust on first use; null where none did.</param>
        /// <param name="KeyPinnedChanged">Whether the connection is key-pinned now where it was not, or the other way round: it has to be dialled again.</param>
        /// <param name="Error">Why nothing was changed.</param>
        public Boolean TryChangeKeyPinTrust(String                            Id,
                                            ServerKeyPinSettings              Settings,
                                            out KeyPin?                       Forgotten,
                                            out Boolean                       KeyPinnedChanged,
                                            [NotNullWhen(false)] out String?  Error)
        {

            Forgotten         = null;
            KeyPinnedChanged  = false;

            lock (updateLock)
            {

                if (!connections.TryGetValue(Id, out var entry) || !entry.IsSecure)
                {
                    Error = $"There is no wss:// connection '{Id}' here.";
                    return false;
                }

                if (!ConnectionEntry.ValidateKeyPinTrust(entry.URL,
                                                         Settings.RawPublicKeys    ?? entry.RawPublicKeys.  ToString(),
                                                         Settings.TrustOnFirstUse  ?? entry.TrustOnFirstUse.ToString(),
                                                         Settings.ProbationSeconds ?? (Int64) entry.ServerKeyPinProbationTime.TotalSeconds,
                                                         out var rawPublicKeys,
                                                         out var trustOnFirstUse,
                                                         out var probationTime,
                                                         out Error))
                    return false;

                if (!ConnectionEntry.CheckKeyPinned(entry.ServerKeyPins, rawPublicKeys, trustOnFirstUse, out Error))
                    return false;

                var was = entry.Copy();

                entry.RawPublicKeys              = rawPublicKeys;
                entry.TrustOnFirstUse            = trustOnFirstUse;
                entry.ServerKeyPinProbationTime  = probationTime;

                if (trustOnFirstUse == KeyTrustOnFirstUse.None && entry.ServerKeyPinLearned is not null)
                {
                    Forgotten                    = entry.ServerKeyPinLearned;
                    entry.ServerKeyPinLearned    = null;
                    entry.ServerKeyPinLearnedAt  = null;
                }

                if (!TrySaveConnections(out Error))
                {
                    connections[Id] = was;
                    Forgotten = null;
                    return false;
                }

                KeyPinnedChanged = was.IsKeyPinned != entry.IsKeyPinned;

                Notice(LogLevel.Notice,
                       $"'{entry.Description}' believes its server as a signed pin set said: raw public keys {rawPublicKeys}, trust on first use " +
                       $"{trustOnFirstUse}, a probation of {probationTime.TotalSeconds:0} s" +
                       (Forgotten is not null
                            ? $"; the key pin {Forgotten} it learned on probation is forgotten with trust on first use."
                            : "."));

                Error = null;
                return true;

            }

        }

        #endregion

        #region (internal) DialAgain(Id)

        /// <summary>
        /// Have the given connection dialled again, as after a change of how it
        /// is dialled: the one made before is hung up.
        /// </summary>
        /// <remarks>
        /// For a change written before, whose dialling had to wait - see
        /// <see cref="TryChangeKeyPinTrust"/>. Its revision is raised, and whoever
        /// follows the connections is told, as after a change written down.
        /// </remarks>
        /// <param name="Id">The connection.</param>
        public void DialAgain(String Id)
        {

            lock (updateLock)
            {

                if (!connections.ContainsKey(Id))
                    return;

                BumpDialRevision(Id);

            }

            if (OnConnectionsChanged is Action changed)
                Task.Run(changed);

        }

        #endregion

        #region (internal) CopyOf(Id) / (private static) SamePins(Text1, Text2)

        /// <summary>
        /// The connection as it stands now, in an entry of its own that nothing
        /// changes - or null where there is none under that identification.
        /// </summary>
        /// <remarks>
        /// For what asks at every TLS handshake which key pins a connection has:
        /// a copy taken under the lock, so that a change written meanwhile is
        /// read whole or not at all, and without working out the warnings of
        /// every connection, as <see cref="Connections"/> does.
        /// </remarks>
        public ConnectionEntry? CopyOf(String Id)
        {
            lock (updateLock)
                return connections.TryGetValue(Id, out var entry)
                           ? entry.Copy()
                           : null;
        }

        /// <summary>
        /// Whether two texts of key pins say the same pins in the same order,
        /// however they are written; the texts themselves where one of them
        /// cannot be read.
        /// </summary>
        private static Boolean SamePins(String Text1, String Text2)
        {

            static String[]? Normalized(String Text)
            {
                var pins = new List<String>();
                foreach (var one in ConnectionEntry.SplitServerKeyPins(Text))
                {
                    if (!KeyPin.TryParse(one, out var pin))
                        return null;
                    pins.Add(pin.ToHex());
                }
                return [.. pins.Distinct()];
            }

            return Normalized(Text1) is String[] pins1 && Normalized(Text2) is String[] pins2
                       ? pins1.SequenceEqual(pins2)
                       : ConnectionEntry.Named(Text1) == ConnectionEntry.Named(Text2);

        }

        #endregion

        #region TryRemoveConnection(Id, out Error [, out NotSaved])

        /// <summary>
        /// Take one connection away, for good.
        /// </summary>
        /// <remarks>
        /// Nothing refers to a connection, so nothing stands in the way. The
        /// credentials it used stay: they are frequently the same ones the
        /// replacement will use.
        /// </remarks>
        public Boolean TryRemoveConnection(String?                          Id,
                                           [NotNullWhen(false)] out String? Error)

            => TryRemoveConnection(Id, out Error, out _);

        /// <summary>
        /// Take one connection away - and say whether a refusal was the file's
        /// rather than the request's.
        /// </summary>
        /// <param name="NotSaved">True where the file of the connections could not be written: nothing about the request was wrong, and the connection stays.</param>
        public Boolean TryRemoveConnection(String?                          Id,
                                           [NotNullWhen(false)] out String? Error,
                                           out Boolean                      NotSaved)
        {

            NotSaved = false;

            lock (updateLock)
            {

                if (Id is null || !connections.TryGetValue(Id, out var entry))
                {
                    Error = "There is no connection here under that identification.";
                    return false;
                }

                var peersBefore = SnapshotPeers();

                connections.Remove(Id);
                PeerOfUnlocked(Id)?.RemoveEndpoint(Id);

                var notes = Normalize();

                if (!TrySaveConnections(out Error))
                {
                    connections[Id] = entry;
                    RestorePeers(peersBefore);
                    NotSaved = true;
                    return false;
                }

                Notice(LogLevel.Info, $"Connection removed: {entry}.");
                Tell(notes, LogLevel.Notice);

                Error = null;
                return true;

            }

        }

        #endregion

        #region (private) BumpDialRevision(Id) / ChangesHowItIsDialled(Was, Now)

        /// <summary>
        /// The given connection is dialled differently now. Called under the
        /// lock; says what the revision was, to put it back where the change
        /// could not be written.
        /// </summary>
        private Int64 BumpDialRevision(String Id)
        {
            var was = dialRevisions.GetValueOrDefault(Id);
            dialRevisions[Id] = was + 1;
            return was;
        }

        /// <summary>
        /// Whether a connection changed in a way that has to be dialled again.
        /// </summary>
        private static Boolean ChangesHowItIsDialled(ConnectionEntry Was, ConnectionEntry Now)

            => Was.URL                 != Now.URL                 ||
               Was.OCPPVersion         != Now.OCPPVersion         ||
               Was.AutoConnect         != Now.AutoConnect         ||
               Was.AuthenticationId    != Now.AuthenticationId    ||
               Was.CertificateId       != Now.CertificateId       ||
               Was.HTTPVersion         != Now.HTTPVersion         ||
               Was.Encoding            != Now.Encoding            ||
               Was.SignatureFormats    != Now.SignatureFormats    ||
               Was.SshHostKey          != Now.SshHostKey          ||
               Was.SshTrustOnFirstUse  != Now.SshTrustOnFirstUse  ||
               Was.SshOnPinMismatch    != Now.SshOnPinMismatch    ||
               Was.SshProbationTime    != Now.SshProbationTime    ||

               // The key pins someone edits. Learning one and taking a signed
               // pin set are no edit and dial nothing again: a rejected pin set
               // drops its connection by itself.
               !Was.ServerKeyPins.SequenceEqual(Now.ServerKeyPins)        ||
               Was.RawPublicKeys              != Now.RawPublicKeys              ||
               Was.TrustOnFirstUse            != Now.TrustOnFirstUse            ||
               Was.ServerKeyPinProbationTime  != Now.ServerKeyPinProbationTime  ||

               // What a connection checked by its CA is held to, where somebody
               // edits it: the server it has now may be one it would not get
               // past any more. Learning on first use is no edit.
               !(Was.Pins ?? ServerPins.None).SameAs(Now.Pins);

        #endregion

        #region Peers: (private) ReadPeers() / Migrate() / Normalize() and their helpers

        /// <summary>
        /// peers.json, read into the peers and their endpoints. Called under the
        /// lock.
        /// </summary>
        /// <remarks>
        /// Once peers.json is there, it is what this station dials, and
        /// connections.json beside it is not read again. A peers.json that
        /// cannot be read at all is ignored, as connections.json was: the
        /// station dials nothing rather than something nobody wrote down any
        /// more.
        /// </remarks>
        private void ReadPeers()
        {

            JObject file;

            try
            {
                file = JObject.Parse(File.ReadAllText(System.IO.Path.Combine(Path, PeersFileName)));
            }
            catch (Exception e)
            {
                Notice(LogLevel.Error, $"'{PeersFileName}' could not be read and is being ignored: {e.Message}");
                return;
            }

            Migration = PeerMigration.TryParse(file["migration"]);

            foreach (var json in (file["peers"] as JArray)?.OfType<JObject>() ?? [])
            {

                if (!PeerEntry.TryParse(json, out var peer, out var endpoints, out var problem))
                {
                    Notice(LogLevel.Warning, $"A peer could not be read: {problem}");
                    continue;
                }

                if (peers.ContainsKey(peer.Id))
                {
                    Notice(LogLevel.Warning, $"Peer '{peer.Id}' is written down twice; the second one is ignored.");
                    continue;
                }

                foreach (var endpointJSON in endpoints)
                {

                    if (!ConnectionEntry.TryParse(endpointJSON, out var entry, out problem))
                    {
                        Notice(LogLevel.Warning, $"An endpoint of peer '{peer.Id}' could not be read: {problem}");
                        continue;
                    }

                    if (connections.ContainsKey(entry.Id))
                    {
                        Notice(LogLevel.Warning, $"Endpoint '{entry.Id}' is written down twice; the one of peer '{peer.Id}' is ignored.");
                        continue;
                    }

                    connections[entry.Id] = entry;
                    peer.AddEndpoint(entry.Id);

                }

                if (peer.EndpointIds.Count == 0)
                {
                    Notice(LogLevel.Warning, $"Peer '{peer.Id}' has no endpoint that could be read and is ignored.");
                    continue;
                }

                peers[peer.Id] = peer;

            }

            // What a hand made the file say against the rules is put right here
            // and told, not written back: whoever edited it should see why.
            Tell(Normalize(), LogLevel.Warning);

        }


        /// <summary>
        /// The peers made of connections.json, once, when there is no
        /// peers.json yet (section 11.7.1 of EVQI Transport). Called under the
        /// lock.
        /// </summary>
        /// <remarks>
        /// Nothing is dialled differently. Every connection stays an endpoint
        /// under its own id, and keeps whether it dials by itself; the roles are
        /// written down, and what a role may do applies from work package P9
        /// on. Every connection becomes a peer of its own, but a CSMSBackup,
        /// which becomes the next endpoint of the one CSMS of its OCPP version,
        /// where there is exactly one. The oldest connection that dials by
        /// itself makes its peer the primary; the oldest of the other OCPP
        /// version makes its peer a successor, since the model has one back end
        /// in charge where there were two, one per OCPP node; every other peer
        /// dialled by itself is an observer, and one that is not a service.
        ///
        /// connections.json is left as it was, for a station of an older
        /// version; it is not read again. The migration is told, line by line,
        /// and kept in peers.json.
        /// </remarks>
        private void Migrate()
        {

            var notes    = new List<String>();
            var ordered  = connections.Values.
                               OrderBy(entry => entry.CreatedAt).
                               ThenBy (entry => entry.Id, StringComparer.Ordinal).
                               ToArray();

            foreach (var entry in ordered.Where(entry => entry.ConnectionType != ConnectionType.CSMSBackup))
                peers[entry.Id] = NewPeerOf(entry);

            var csms = peers.Values.Where(peer => peer.Kind == PeerKind.CSMS).ToArray();

            foreach (var backup in ordered.Where(entry => entry.ConnectionType == ConnectionType.CSMSBackup))
            {

                var backedUp = csms.Where(peer => peer.OCPPVersion == backup.OCPPVersion).ToArray();

                if (backedUp.Length == 1)
                {
                    backedUp[0].AddEndpoint(backup.Id);
                    notes.Add($"'{backup.Description}' ({backup.Id}), a CSMSBackup, is the next endpoint of peer {backedUp[0].Id} " +
                              $"'{backedUp[0].Description}', the one CSMS of {ConnectionEntry.AsText(backup.OCPPVersion)}.");
                }

                else
                {
                    peers[backup.Id] = NewPeerOf(backup);
                    notes.Add($"'{backup.Description}' ({backup.Id}), a CSMSBackup, is a peer of its own: " +
                              (backedUp.Length == 0
                                   ? $"there is no CSMS of {ConnectionEntry.AsText(backup.OCPPVersion)} it would back up."
                                   : $"{backedUp.Length} CSMS of {ConnectionEntry.AsText(backup.OCPPVersion)} could be the one it backs up."));
                }

            }

            foreach (var peer in peers.Values)
                peer.Role = IsDialled(peer) ? PeerRole.Observer : PeerRole.Service;

            if (OldestDialled() is PeerEntry primary)
            {

                primary.Role = PeerRole.Primary;

                var otherVersion = primary.OCPPVersion == OCPPVersion.OCPP1_6 ? OCPPVersion.OCPP2_1 : OCPPVersion.OCPP1_6;

                if (OldestDialled(otherVersion) is PeerEntry successor)
                {
                    successor.Role = PeerRole.Successor;
                    notes.Add($"Peer {successor.Id} '{successor.Description}' was in charge of the {ConnectionEntry.AsText(otherVersion)} node, " +
                              $"as '{primary.Description}' was of the {ConnectionEntry.AsText(primary.OCPPVersion)} one. The {NodeWording.Noun} has one primary now, " +
                              $"so it is a successor: dialled and registered with as before, and held to a successor's rights once they apply.");
                }

            }

            notes.InsertRange(0, OrderedPeers().Select(peer => $"Peer {peer.Id} '{peer.Description}': {peer.Kind}, {peer.Role}, " +
                                                                $"{ConnectionEntry.AsText(peer.OCPPVersion)}, endpoint(s) {String.Join(", ", peer.EndpointIds)}."));

            notes.Insert(0, $"The {connections.Count} connection(s) of {ConnectionsFileName} are {peers.Count} peer(s) in {PeersFileName} now; " +
                            $"nothing is dialled differently.");

            notes.AddRange(Normalize());

            Migration = new PeerMigration(TimeProvider.GetUtcNow(), notes);

            Tell(notes, LogLevel.Notice);

            if (!TrySaveConnections(out var error))
                Notice(LogLevel.Error, $"The peers made of {ConnectionsFileName} could not be written, and are made again at the next start: {error}");

        }


        /// <summary>
        /// The rules a peer list keeps, made to hold. Called under the lock;
        /// says what it changed.
        /// </summary>
        /// <remarks>
        /// - Every connection is the endpoint of exactly one peer.
        /// - A peer none of whose endpoints dials by itself is a service, and a
        ///   service one of whose endpoints does is not.
        /// - At most one primary, and one as soon as a peer is dialled by
        ///   itself: the oldest such peer, where none is.
        /// - Rights are a secondary's; a primary has every right.
        /// </remarks>
        private List<String> Normalize()
        {

            var notes = new List<String>();

            foreach (var peer in peers.Values)
                foreach (var id in peer.EndpointIds.Where(id => !connections.ContainsKey(id)).ToArray())
                    peer.RemoveEndpoint(id);

            foreach (var peer in peers.Values.Where(peer => peer.EndpointIds.Count == 0).ToArray())
            {
                peers.Remove(peer.Id);
                notes.Add($"Peer {peer.Id} '{peer.Description}' has no endpoint left and is gone.");
            }

            var owned = peers.Values.SelectMany(peer => peer.EndpointIds).ToHashSet();

            foreach (var entry in connections.Values.Where(entry => !owned.Contains(entry.Id)).OrderBy(entry => entry.CreatedAt).ToArray())
            {
                var peer = NewPeerOf(entry);
                peer.Role = IsDialled(peer) ? PeerRole.Observer : PeerRole.Service;
                peers[peer.Id] = peer;
                notes.Add($"Connection '{entry.Description}' ({entry.Id}) belonged to no peer and is peer {peer.Id} of its own now, a {peer.Role}.");
            }

            foreach (var peer in peers.Values)
            {

                var dialled = IsDialled(peer);

                if (!dialled && peer.Role != PeerRole.Service)
                {
                    notes.Add($"None of the endpoints of peer {peer.Id} '{peer.Description}' dials by itself, so it is no {peer.Role} but a Service now.");
                    peer.Role = PeerRole.Service;
                }

                else if (dialled && peer.Role == PeerRole.Service)
                {
                    peer.Role = PeerRole.Observer;
                    notes.Add($"An endpoint of peer {peer.Id} '{peer.Description}' dials by itself, so it is no Service but an Observer now.");
                }

            }

            var primaries = peers.Values.Where(peer => peer.Role == PeerRole.Primary).OrderBy(FirstDialledAt).ToArray();

            foreach (var extra in primaries.Skip(1))
            {
                extra.Role = PeerRole.Successor;
                notes.Add($"Peer {extra.Id} '{extra.Description}' was a second primary and is a Successor now: one back end is in charge.");
            }

            if (primaries.Length == 0 && OldestDialled() is PeerEntry next)
            {
                notes.Add($"Switchover: peer {next.Id} '{next.Description}' is the primary now, as the oldest peer dialled by itself; it was a {next.Role}.");
                next.Role = PeerRole.Primary;
            }

            foreach (var peer in peers.Values.Where(peer => peer.Role == PeerRole.Primary && peer.Rights is not null))
            {
                peer.Rights = null;
                notes.Add($"The rights of peer {peer.Id} '{peer.Description}' are gone: a primary has every right.");
            }

            return notes;

        }


        private PeerEntry NewPeerOf(ConnectionEntry Entry)
        {

            var peer = new PeerEntry(peers.ContainsKey(Entry.Id) ? NewId() : Entry.Id,
                                     Entry.Description,
                                     PeerEntry.KindOf(Entry.ConnectionType),
                                     PeerRole.Service,
                                     Entry.OCPPVersion,
                                     Entry.CreatedAt);

            peer.AddEndpoint(Entry.Id);
            return peer;

        }

        /// <summary>
        /// Whether one of the endpoints of the given peer dials by itself.
        /// </summary>
        private Boolean IsDialled(PeerEntry Peer)

            => Peer.EndpointIds.Any(id => connections.TryGetValue(id, out var entry) && entry.AutoConnect);

        /// <summary>
        /// When the oldest of the endpoints of the given peer that dial by
        /// themselves was written down; the end of time for a peer with none.
        /// </summary>
        private DateTimeOffset FirstDialledAt(PeerEntry Peer)

            => Peer.EndpointIds.
                   Select (id => connections.TryGetValue(id, out var entry) && entry.AutoConnect ? entry.CreatedAt : DateTimeOffset.MaxValue).
                   DefaultIfEmpty(DateTimeOffset.MaxValue).
                   Min();

        /// <summary>
        /// The peer this station has dialled first, of the given OCPP version
        /// or of both.
        /// </summary>
        private PeerEntry? OldestDialled(OCPPVersion? Version = null)

            => peers.Values.
                   Where  (peer => IsDialled(peer) && (Version is null || peer.OCPPVersion == Version)).
                   OrderBy(FirstDialledAt).
                   ThenBy (peer => peer.Id, StringComparer.Ordinal).
                   FirstOrDefault();

        private IEnumerable<PeerEntry> OrderedPeers()

            => peers.Values.
                   OrderBy(peer => peer.CreatedAt).
                   ThenBy (peer => peer.Id, StringComparer.Ordinal);

        private PeerEntry? PeerOfUnlocked(String ConnectionId)

            => peers.Values.FirstOrDefault(peer => peer.EndpointIds.Contains(ConnectionId));

        private PeerEntry[] SnapshotPeers()

            => [.. peers.Values.Select(peer => peer.Copy())];

        private void RestorePeers(PeerEntry[] Snapshot)
        {
            peers.Clear();
            foreach (var peer in Snapshot)
                peers[peer.Id] = peer;
        }

        private void Tell(IEnumerable<String> Notes, LogLevel Level)
        {
            foreach (var note in Notes)
                Notice(Level, note);
        }

        #endregion

        #region TryUpdatePeer(Id, Description, Kind, Role, Identity, Rights, out Error [, out NotSaved])

        /// <summary>
        /// Change what a peer is: its description, kind, role, identity or
        /// rights. Whatever is null stays as it is; an empty identity and a JSON
        /// null for the rights take them away.
        /// </summary>
        /// <remarks>
        /// A peer made the primary takes over from the one that was: that one
        /// is a successor then, and the change is told as a switchover. The
        /// primary cannot be made anything else directly - the oldest peer
        /// dialled by itself would be in charge again at once, often the same -
        /// so another one is made the primary instead. Whether a peer is a
        /// service follows from its endpoints: one that dials by itself makes
        /// its peer one that is dialled always.
        /// </remarks>
        public Boolean TryUpdatePeer(String?                          Id,
                                     String?                          Description,
                                     String?                          Kind,
                                     String?                          Role,
                                     String?                          Identity,
                                     JToken?                          Rights,
                                     [NotNullWhen(false)] out String? Error)

            => TryUpdatePeer(Id, Description, Kind, Role, Identity, Rights, out Error, out _);

        public Boolean TryUpdatePeer(String?                          Id,
                                     String?                          Description,
                                     String?                          Kind,
                                     String?                          Role,
                                     String?                          Identity,
                                     JToken?                          Rights,
                                     [NotNullWhen(false)] out String? Error,
                                     out Boolean                      NotSaved)
        {

            NotSaved = false;

            var description = Description?.Trim();

            if (description is not null && (description.Length == 0 || description.Length > PeerEntry.MaxDescriptionLength))
            {
                Error = $"A peer is described in 1 to {PeerEntry.MaxDescriptionLength} characters.";
                return false;
            }

            var kind = PeerKind.CSMS;

            if (Kind is not null && !PeerEntry.TryParseKind(Kind, out kind))
            {
                Error = $"'{Kind}' is no kind of peer: {String.Join(", ", Enum.GetNames<PeerKind>())}.";
                return false;
            }

            var role = PeerRole.Service;

            if (Role is not null && !PeerEntry.TryParseRole(Role, out role))
            {
                Error = $"'{Role}' is no role of a peer: {String.Join(", ", Enum.GetNames<PeerRole>())}.";
                return false;
            }

            var identity = Identity?.Trim();

            if (identity is not null && (identity.Length > PeerEntry.MaxIdentityLength || identity.Any(Char.IsWhiteSpace)))
            {
                Error = $"The identity towards a peer is at most {PeerEntry.MaxIdentityLength} characters, without spaces.";
                return false;
            }

            PeerRights? rights = null;

            if (Rights is not null && !PeerRights.TryParse(Rights, out rights, out Error))
                return false;

            lock (updateLock)
            {

                if (Id is null || !peers.TryGetValue(Id, out var peer))
                {
                    Error = "There is no peer here under that identification.";
                    return false;
                }

                var newRole    = Role   is not null ? role   : peer.Role;
                var newRights  = Rights is not null ? rights : peer.Rights;

                if (peer.Role == PeerRole.Primary && newRole != PeerRole.Primary)
                {
                    Error = $"'{peer.Description}' is the primary. To hand the {NodeWording.Noun} to another back end, make that one the primary.";
                    return false;
                }

                if (newRole == PeerRole.Service && IsDialled(peer))
                {
                    Error = $"An endpoint of '{peer.Description}' dials by itself, and a service is dialled on demand only. Switch that off first.";
                    return false;
                }

                if (newRole != PeerRole.Service && !IsDialled(peer))
                {
                    Error = $"None of the endpoints of '{peer.Description}' dials by itself, and a {newRole} is dialled always. Switch that on for one of them first.";
                    return false;
                }

                if (newRole == PeerRole.Primary && newRights is not null)
                {
                    Error = "A primary has every right; rights are for a secondary. Take them away first.";
                    return false;
                }

                var before  = SnapshotPeers();
                var notes   = new List<String>();

                if (newRole == PeerRole.Primary && peer.Role != PeerRole.Primary)
                {

                    foreach (var previous in peers.Values.Where(other => other.Role == PeerRole.Primary))
                    {
                        previous.Role = PeerRole.Successor;
                        notes.Add($"Switchover: peer {peer.Id} '{peer.Description}' is the primary now, in place of peer {previous.Id} " +
                                  $"'{previous.Description}', which is a successor.");
                    }

                    if (notes.Count == 0)
                        notes.Add($"Switchover: peer {peer.Id} '{peer.Description}' is the primary now.");

                }

                else if (newRole != peer.Role)
                    notes.Add($"Peer {peer.Id} '{peer.Description}' is a {newRole} now; it was a {peer.Role}.");

                if (description is not null)  peer.Description  = description;
                if (Kind        is not null)  peer.Kind         = kind;
                if (identity    is not null)  peer.Identity     = identity.Length > 0 ? identity : null;

                peer.Role    = newRole;
                peer.Rights  = newRights;

                notes.AddRange(Normalize());

                if (!TrySaveConnections(out Error))
                {
                    RestorePeers(before);
                    NotSaved = true;
                    return false;
                }

                Notice(LogLevel.Info, $"Peer changed: {peer}.");
                Tell(notes, LogLevel.Notice);

                Error = null;
                return true;

            }

        }

        #endregion

        #region TryMoveEndpoint(ConnectionId, PeerId, Position, out Error [, out NotSaved])

        /// <summary>
        /// Make a connection an endpoint of the given peer, at the given place
        /// among its endpoints, or the last: a backup, say, put after the CSMS it
        /// backs up. A peer left without endpoints is gone.
        /// </summary>
        public Boolean TryMoveEndpoint(String?                          ConnectionId,
                                       String?                          PeerId,
                                       Int32?                           Position,
                                       [NotNullWhen(false)] out String? Error)

            => TryMoveEndpoint(ConnectionId, PeerId, Position, out Error, out _);

        public Boolean TryMoveEndpoint(String?                          ConnectionId,
                                       String?                          PeerId,
                                       Int32?                           Position,
                                       [NotNullWhen(false)] out String? Error,
                                       out Boolean                      NotSaved)
        {

            NotSaved = false;

            lock (updateLock)
            {

                if (ConnectionId is null || !connections.TryGetValue(ConnectionId, out var entry))
                {
                    Error = "There is no connection here under that identification.";
                    return false;
                }

                if (PeerId is null || !peers.TryGetValue(PeerId, out var peer))
                {
                    Error = "There is no peer here under that identification.";
                    return false;
                }

                if (entry.OCPPVersion != peer.OCPPVersion)
                {
                    Error = $"Every endpoint of a peer speaks one OCPP version: '{peer.Description}' speaks {ConnectionEntry.AsText(peer.OCPPVersion)}, " +
                            $"'{entry.Description}' {ConnectionEntry.AsText(entry.OCPPVersion)}.";
                    return false;
                }

                var before = SnapshotPeers();

                PeerOfUnlocked(entry.Id)?.RemoveEndpoint(entry.Id);
                peer.AddEndpoint(entry.Id, Position);

                var notes = Normalize();

                if (!TrySaveConnections(out Error))
                {
                    RestorePeers(before);
                    NotSaved = true;
                    return false;
                }

                Notice(LogLevel.Info, $"'{entry.Description}' ({entry.Id}) is endpoint {peer.EndpointIds.ToList().IndexOf(entry.Id) + 1} of peer {peer}.");
                Tell(notes, LogLevel.Notice);

                Error = null;
                return true;

            }

        }

        #endregion


        #region (private) CheckReferences(...) / CheckEverything() / CheckEntry(Entry)

        /// <summary>
        /// What a connection says it proves itself with, checked against what
        /// is actually here. Called under the lock.
        /// </summary>
        private Boolean CheckReferences(String?                          AuthenticationId,
                                        String?                          CertificateId,
                                        Boolean                          OverSSH,
                                        out String?                      Authentication,
                                        out String?                      Certificate,
                                        [NotNullWhen(false)] out String? Error)
        {

            Authentication  = ConnectionEntry.Named(AuthenticationId);
            Certificate     = ConnectionEntry.Named(CertificateId);
            Error           = null;

            if (Authentication is not null && Certificate is not null)
            {
                Error = "A connection proves itself one way, not two. Choose credentials or a client certificate.";
                return false;
            }

            if (Authentication is not null && !authentications.ContainsKey(Authentication))
            {
                Error = "Those credentials are not configured here.";
                return false;
            }

            // Over SSH this station signs in with its own key, and a password
            // on top where the server asks for one. A TLS client certificate
            // has no handshake to be shown in, and a TOTP password is answered
            // in an HTTP header there is none of.
            if (OverSSH && Certificate is not null)
            {
                Error = "A client certificate is shown in a TLS handshake, and an ssh:// connection makes none: " +
                        $"it signs in with this {NodeWording.Noun}'s SSH key.";
                return false;
            }

            if (OverSSH && Authentication is not null && authentications[Authentication].Kind == AuthenticationKind.TOTP)
            {
                Error = $"An ssh:// connection signs in with this {NodeWording.Noun}'s SSH key, and a password on top where wanted. " +
                        "A TOTP password is answered over HTTP, which it does not speak.";
                return false;
            }

            if (Certificate is not null &&
                KnownCertificateIds is not null &&
                !KnownCertificateIds().Contains(Certificate))
            {
                Error = $"That client certificate is not on this {NodeWording.Noun}.";
                return false;
            }

            return true;

        }

        /// <summary>
        /// Called under the lock.
        /// </summary>
        private void CheckEverything()
        {
            foreach (var entry in connections.Values)
                CheckEntry(entry);
        }

        /// <summary>
        /// What is worth saying about one connection, worked out again.
        /// </summary>
        /// <remarks>
        /// Rebuilt rather than remembered, because all of it depends on
        /// something other than the connection: which credentials still exist,
        /// whether they have a secret yet, which certificates are still here.
        /// A warning cached at the moment of writing would go on saying
        /// something that stopped being true on the page next door.
        /// </remarks>
        private void CheckEntry(ConnectionEntry Entry)
        {

            Entry.Warnings.Clear();

            if (Entry.AuthenticationId is not null)
            {

                if (!authentications.TryGetValue(Entry.AuthenticationId, out var credentials))
                    Entry.Warnings.Add("The credentials this connection names are not configured here any more.");

                else
                {

                    if (!credentials.HasSecret)
                        Entry.Warnings.Add($"'{credentials.Description}' has no secret set, so this connection cannot prove itself.");

                    if (credentials.Kind == AuthenticationKind.Basic && !Entry.IsSecure && !Entry.IsSSH)
                        Entry.Warnings.Add("HTTP Basic over a connection without TLS sends the password in the clear to anybody on the way.");

                    if (credentials.Kind == AuthenticationKind.TOTP && Entry.IsSSH)
                        Entry.Warnings.Add($"'{credentials.Description}' is a TOTP password, which is answered over HTTP. " +
                                            "An ssh:// connection does not speak it, so it is not dialled.");

                    if (credentials.Kind == AuthenticationKind.Basic && Entry.IsSSH &&
                        Entry.URL.Login is String login && login != credentials.Login)
                        Entry.Warnings.Add($"The URL signs in as '{login}' and '{credentials.Description}' as '{credentials.Login}'. " +
                                            "It is one login, so it is not dialled until the two agree.");

                    if (credentials.Kind == AuthenticationKind.TOTP && credentials.TLSChannelBinding && !Entry.IsSSH &&
                        !Entry.BindsItsPassword)
                        Entry.Warnings.Add($"'{credentials.Description}' binds its password to the TLS session, and one is bound only on a " +
                                            $"connection that names HTTP/3 and nothing else: over HTTP/1.1 and HTTP/2 this {NodeWording.Noun}'s TLS exports " +
                                            "no keying material to bind one with, so it is not dialled. Name HTTP/3 for it, or switch the " +
                                            "binding off to have the password sent unbound - and know that an unbound password can be replayed " +
                                            "within its validity.");

                }

            }

            if (Entry.CertificateId is not null)
            {

                if (KnownCertificateIds is not null && !KnownCertificateIds().Contains(Entry.CertificateId))
                    Entry.Warnings.Add($"The client certificate this connection names is not on this {NodeWording.Noun} any more.");

                if (!Entry.IsSecure)
                    Entry.Warnings.Add("A client certificate is shown during a TLS handshake, and this connection makes none.");

            }

            // Over SSH this station always shows its own key.
            if (Entry.AuthenticationId is null && Entry.CertificateId is null && !Entry.IsSSH)
                Entry.Warnings.Add("This connection proves nothing about who is dialling. A back end that asks will refuse it.");

            if (Entry.IsSSH)
            {

                if (Entry.PinsMD5)
                    Entry.Warnings.Add("An MD5 fingerprint is pinned, and MD5 fingerprints are no longer taken: this connection is not dialled " +
                                       "until it is replaced by the SHA-256 one - the connection test shows it.");

                if (Entry.SshHostKey is null && Entry.SshLearnedHostKey is not null)
                    Entry.Warnings.Add($"The host key {Entry.SshLearnedHostKey} was learned on the first connection and is on probation: " +
                                        "until a signed pin set confirms it, nothing but such a pin set is executed.");

                else if (Entry.SshHostKey is null && Entry.SshTrustOnFirstUse == SshTrustOnFirstUse.HostKey)
                    Entry.Warnings.Add("No host key is pinned, so the first server that answers is believed, on probation for " +
                                       $"{Entry.SshProbationTime.TotalMinutes:0.#} minute(s) until a signed pin set confirms its key. " +
                                        "Pin it now to decide yourself - the connection test shows the key.");

                else if (Entry.SshHostKey is null)
                    Entry.Warnings.Add("No host key is pinned and trust on first use is off, so this connection is not dialled: " +
                                       "pin its server's host key - the connection test shows it - or let it learn the first one, on probation.");

                if (Entry.SshOnPinMismatch != SshOnPinMismatch.Refuse)
                    Entry.Warnings.Add($"A host key that matches no pin is believed all the same ({Entry.SshOnPinMismatch}): whoever answers on the way " +
                                        "is taken for the server. That is for test benches, and for the move to a server whose key is not pinned yet.");

            }

            if (Entry.IsKeyPinned)
            {

                if (Entry.ServerKeyPinLearned is not null)
                    Entry.Warnings.Add($"The key pin {Entry.ServerKeyPinLearned} was learned on the first connection and is on probation: " +
                                        "until a signed pin set confirms it, nothing but such a pin set is executed.");

                else if (Entry.ServerKeyPins.Count == 0)
                    Entry.Warnings.Add("No key is pinned, so the first server that answers is believed, on probation for " +
                                       $"{Entry.ServerKeyPinProbationTime.TotalMinutes:0.#} minute(s) until a signed pin set confirms its key. " +
                                        "Pin it now to decide yourself - the connection test shows its key pin.");

                // Raw public keys (RFC 7250) are offered where a connection
                // prefers or requires them. Preferred, there is nothing to warn
                // of: a server that knows none shows its certificate, and its
                // key is checked by the same pins.
                if (Entry.RawPublicKeys == RawPublicKeyMode.Require)
                    Entry.Warnings.Add("This connection requires raw public keys: a server that shows a certificate is turned away, whatever " +
                                       "key is in it. Prefer takes the same key in a certificate as well, by the same key pins.");

            }

        }

        #endregion


        #region ToJSON()

        /// <summary>
        /// Everything this store holds, as the web interface sees it - which
        /// is everything except the secrets.
        /// </summary>
        public JObject ToJSON()
        {

            lock (updateLock)
            {

                CheckEverything();

                return new JObject(

                           new JProperty("directory",              Path),

                           new JProperty("authentications",        new JArray(
                               authentications.Values.
                                   OrderByDescending(entry => entry.CreatedAt).
                                   Select(entry => entry.ToJSON()))),

                           new JProperty("connections",            new JArray(
                               connections.Values.
                                   OrderByDescending(entry => entry.CreatedAt).
                                   Select(entry => entry.ToJSON()))),

                           new JProperty("peers",                  new JArray(
                               OrderedPeers().Select(peer => peer.ToAPIJSON()))),

                           new JProperty("peerKinds",              new JArray(Enum.GetNames<PeerKind>())),
                           new JProperty("peerRoles",              new JArray(Enum.GetNames<PeerRole>())),

                           new JProperty("migration",              Migration?.ToJSON()),

                           new JProperty("pinSetSigners",          new JArray(
                               pinSetSigners.Select(signer => signer.ToJSON()))),

                           new JProperty("connectionTypes",        new JArray("CSMS", "CSMSBackup", "LocalController")),
                           new JProperty("ocppVersions",           new JArray("OCPP2.1", "OCPP1.6")),

                           new JProperty("maxDescriptionLength",   AuthenticationEntry.MaxDescriptionLength),
                           new JProperty("minSharedSecretLength",  AuthenticationEntry.MinSharedSecretLength),

                           // Said out loud rather than left to be discovered:
                           // somebody looking for where the password went
                           // should find the reason instead.
                           new JProperty("secretsAreReadable",     false),

                           new JProperty("totpDefaults",           new JObject(
                               new JProperty("validitySeconds",    AuthenticationEntry.DefaultValidity.TotalSeconds),
                               new JProperty("length",             AuthenticationEntry.DefaultLength),
                               new JProperty("alphabet",           AuthenticationEntry.DefaultAlphabet),
                               new JProperty("hashAlgorithm",      "SHA256")))

                       );

            }

        }

        #endregion

        #region (override) ToString()

        public override String ToString()

            => $"{peers.Count} peer(s) with {connections.Count} connection(s), {authentications.Count} set(s) of credentials in '{Path}'";

        #endregion

    }

}
