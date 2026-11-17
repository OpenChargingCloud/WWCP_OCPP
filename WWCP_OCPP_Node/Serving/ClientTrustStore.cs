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
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Logging;

using cloud.charging.open.protocols.OCPP.Node.Connections;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving
{

    /// <summary>
    /// What a charging station's certificate has to lead to before this local
    /// controller lets it in.
    /// </summary>
    /// <remarks>
    /// <b>This list and nothing else.</b> Not the trust store of the operating
    /// system: the certificate authorities in there are the several hundred
    /// that the world trusts to vouch for web sites, and every one of them
    /// could issue a certificate for a charging station that this controller
    /// would then believe. A charging station is vouched for by whoever runs
    /// the charging network, and that is a list with one or two entries on it.
    ///
    /// <b>An empty list is not "trust everybody".</b> It is "nobody has been
    /// named yet", and with security profile 3 that means no station connects
    /// with a certificate. The mistake this avoids is the one where an empty
    /// configuration reads as an open door.
    ///
    /// <b>Intermediates are kept, not only anchors.</b> A charging station is
    /// supposed to send the certificates between itself and its root, and a
    /// good many do not; one held here is the difference between a station that
    /// connects and one turned away over a certificate that was fine.
    /// </remarks>
    public sealed class ClientTrustStore : IDisposable
    {

        #region Data

        /// <summary>
        /// The directory beside the process, when nobody says otherwise.
        /// </summary>
        public const String  DefaultDirectoryName  = "ocpp-client-trust";

        /// <summary>
        /// The object identifier of "this certificate may be used by a client
        /// to authenticate itself".
        /// </summary>
        public const String  ClientAuthenticationOID  = "1.3.6.1.5.5.7.3.2";

        /// <summary>
        /// The longest a name given to a chain may be.
        /// </summary>
        public const Int32   MaxNameLength         = 100;

        /// <summary>
        /// How many chains may be trusted at once. A limit because this is a
        /// list of who may vouch for a charging station, and such a list being
        /// long is itself the problem.
        /// </summary>
        public const Int32   MaxEntries            = 50;

        private readonly Object                                updateLock  = new ();
        private readonly Dictionary<String, ClientTrustEntry>  entries     = [];

        #endregion

        #region Properties

        /// <summary>
        /// The directory the accepted chains live in.
        /// </summary>
        public String        Path          { get; }

        /// <summary>
        /// The clock that decides whether an anchor is inside its own validity.
        /// </summary>
        public TimeProvider  TimeProvider  { get; }

        /// <summary>
        /// Everything in the directory, newest first.
        /// </summary>
        public IReadOnlyList<ClientTrustEntry> Entries
        {
            get
            {
                lock (updateLock)
                    return [.. entries.Values.OrderByDescending(entry => entry.AddedAt)];
            }
        }

        /// <summary>
        /// How many chains are switched on - which is what decides whether a
        /// charging station can connect with a certificate at all.
        /// </summary>
        public Int32 EnabledCount
        {
            get
            {
                lock (updateLock)
                    return entries.Values.Count(entry => entry.Enabled);
            }
        }

        /// <summary>
        /// Called with the full path of every file this store writes, just
        /// before it is written.
        /// </summary>
        /// <remarks>
        /// For the tests, and for nothing else: they make a write fail there,
        /// or fail partway through as on a full disk. Something put in the way
        /// of a file refuses its write before it begins; nothing on the outside
        /// of this store can make one stop halfway.
        /// </remarks>
        internal Action<String>? BeforeWriting { get; set; }

        /// <summary>
        /// Called with the full path of every file this store moves aside,
        /// moves back or deletes when it takes a chain away, just before it
        /// does.
        /// </summary>
        /// <remarks>
        /// For the tests, and for nothing else: they make one of them fail
        /// there, as a file somebody holds open does on Windows - on every
        /// system, and at the step they choose.
        /// </remarks>
        internal Action<String>? BeforeRemoving { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Something happened here that belongs in the log of the controller.
        /// </summary>
        public event Action<LogLevel, String>? OnNotice;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The accepted chains in the given directory; it need not exist yet.
        /// </summary>
        public ClientTrustStore(String?        Path          = null,
                                TimeProvider?  TimeProvider  = null)
        {

            this.Path          = System.IO.Path.GetFullPath(Path ?? DefaultDirectoryName);
            this.TimeProvider  = TimeProvider ?? System.TimeProvider.System;

            Reload();

        }

        #endregion


        #region KeepToItsOwner()

        /// <summary>
        /// Keep everybody but the account the controller runs under out of the
        /// directory and of everything in it, where it is there already - what
        /// an older controller left open taken back. What cannot be done is
        /// said as a warning.
        /// </summary>
        /// <remarks>
        /// Not done by the constructor, which nobody is listening to yet: the
        /// controller calls it once it is. A directory made later is made so.
        /// </remarks>
        public void KeepToItsOwner()
        {
            if (Directory.Exists(Path))
                OwnerOnly.Directory(Path, Complain);
        }

        #endregion

        #region Reload()

        /// <summary>
        /// Read the directory again, from nothing.
        /// </summary>
        public void Reload()
        {

            lock (updateLock)
            {

                foreach (var entry in entries.Values)
                    entry.Dispose();

                entries.Clear();

                if (!Directory.Exists(Path))
                    return;

                foreach (var file in Directory.GetFiles(Path, "*.pem").OrderBy(file => file))
                {

                    var id = System.IO.Path.GetFileNameWithoutExtension(file);

                    try
                    {

                        if (TryLoadEntry(id, out var entry, out var problem))
                            entries[id] = entry;

                        else
                            OnNotice?.Invoke(LogLevel.Error, $"The accepted chain '{id}' in '{Path}' could not be read and is being ignored: {problem}");

                    }
                    catch (Exception e)
                    {
                        OnNotice?.Invoke(LogLevel.Error, $"The accepted chain '{id}' in '{Path}' could not be read and is being ignored: {e.Message}");
                    }

                }

            }

        }

        #endregion

        #region TryAdd(PEM, Name, out Id, out Warnings, out Error)

        /// <summary>
        /// Accept certificates that lead to the certificate in this file.
        /// </summary>
        /// <remarks>
        /// The anchor is the certificate in the file that nothing else in the
        /// file signed - the top of whatever was uploaded. Everything below it
        /// is kept as an intermediate, which is what makes it possible to
        /// upload a root and its issuing certificate authority in one go, the
        /// way they are usually handed out.
        /// </remarks>
        /// <param name="PEM">One or more certificates, as PEM.</param>
        /// <param name="Name">What to call this chain, or null to use the subject of its anchor.</param>
        public Boolean TryAdd(String                            PEM,
                              String?                           Name,
                              [NotNullWhen(true)]  out String?  Id,
                              out IReadOnlyList<String>         Warnings,
                              [NotNullWhen(false)] out String?  Error)

            => TryAdd(PEM, Name, out Id, out Warnings, out Error, out _);

        /// <summary>
        /// Accept certificates that lead to the certificate in this file - and
        /// say whether a refusal was the files' rather than the chain's.
        /// </summary>
        /// <param name="NotSaved">True where the chain or what is said of it could not be written: what was written of them is taken away again, and nothing is accepted.</param>
        public Boolean TryAdd(String                            PEM,
                              String?                           Name,
                              [NotNullWhen(true)]  out String?  Id,
                              out IReadOnlyList<String>         Warnings,
                              [NotNullWhen(false)] out String?  Error,
                              out Boolean                       NotSaved)
        {

            Id        = null;
            Warnings  = [];
            Error     = null;
            NotSaved  = false;

            #region What was uploaded

            var uploaded = new X509Certificate2Collection();

            try
            {
                uploaded.ImportFromPem(PEM);
            }
            catch (Exception e)
            {
                Error = $"This is not a certificate: {e.Message}";
                return false;
            }

            if (uploaded.Count == 0)
            {
                Error = "There is no certificate in this file. What is expected is PEM - the '-----BEGIN CERTIFICATE-----' kind - " +
                        "holding the certificate authority whose charging stations should be let in, and any certificates below it.";
                return false;
            }

            var name = Name?.Trim() ?? "";

            if (name.Length > MaxNameLength)
            {
                Error = $"The name may be at most {MaxNameLength} characters long.";
                return false;
            }

            #endregion

            #region Which of them is the anchor?

            // The one nothing else here was issued by - walk up from anywhere
            // and stop where the issuer is no longer in the file.
            var pool    = uploaded.Cast<X509Certificate2>().ToList();
            var anchor  = pool[0];

            while (true)
            {

                var issuer = pool.FirstOrDefault(candidate => !ReferenceEquals(candidate, anchor) &&
                                                              candidate.SubjectName.RawData.AsSpan().SequenceEqual(anchor.IssuerName.RawData));

                if (issuer is null)
                    break;

                anchor = issuer;

            }

            #endregion

            var id  = Convert.ToHexStringLower(SHA256.HashData(anchor.RawData).AsSpan(0, 8));
            var now = TimeProvider.GetUtcNow();

            lock (updateLock)
            {

                if (entries.ContainsKey(id))
                {
                    Error = $"'{anchor.Subject}' is already accepted here, as '{entries[id].Name}'.";
                    return false;
                }

                if (entries.Count >= MaxEntries)
                {
                    Error = $"At most {MaxEntries} chains may be accepted at once. Remove one that is no longer used.";
                    return false;
                }

                #region Written down

                try
                {

                    CreateDirectory();

                    var ordered = new[] { anchor }.Concat(pool.Where(certificate => !ReferenceEquals(certificate, anchor)));

                    WriteFile(
                        System.IO.Path.Combine(Path, $"{id}.pem"),
                        String.Join(
                            Environment.NewLine,
                            ordered.Select(certificate => PemEncoding.WriteString("CERTIFICATE", certificate.RawData))
                        ) + Environment.NewLine
                    );

                    WriteFile(
                        System.IO.Path.Combine(Path, $"{id}.json"),
                        new JObject(
                            new JProperty("id",       id),
                            new JProperty("name",     name.Length > 0 ? name : anchor.Subject),
                            new JProperty("addedAt",  now.ToString("o")),
                            new JProperty("enabled",  true)
                        ).ToString(Formatting.Indented) + Environment.NewLine
                    );

                }
                catch (Exception e)
                {

                    Error     = $"The chain could not be written to '{Path}': {e.Message}";
                    NotSaved  = true;

                    // Not left behind half made: a chain without what is said
                    // of it was accepted at the next start, switched on and
                    // named by its subject.
                    Forget(id, "pem", "json");

                    return false;

                }

                #endregion

                if (!TryLoadEntry(id, out var entry, out var problem))
                {
                    Error = $"The chain was written but cannot be read back: {problem}";
                    return false;
                }

                entries[id] = entry;

                Id        = id;
                Warnings  = [.. entry.Warnings];

            }

            OnNotice?.Invoke(
                LogLevel.Notice,
                $"Charging stations whose certificate leads to '{anchor.Subject}' are now accepted." +
                (Warnings.Count > 0 ? $" {String.Join(" ", Warnings)}" : "")
            );

            return true;

        }

        #endregion

        #region TrySetEnabled(Id, Enabled, out Error [, out NotSaved]) / TryRename(Id, Name, out Error [, out NotSaved])

        /// <summary>
        /// Switch a chain on or off without throwing it away.
        /// </summary>
        public Boolean TrySetEnabled(String                            Id,
                                     Boolean                           Enabled,
                                     [NotNullWhen(false)] out String?  Error)

            => TrySetEnabled(Id, Enabled, out Error, out _);

        /// <summary>
        /// Switch a chain on or off - and say whether a refusal was the file's
        /// rather than the change's.
        /// </summary>
        /// <param name="NotSaved">True where what is said of the chain could not be written: it is on or off as it was, now and at the next start.</param>
        public Boolean TrySetEnabled(String                            Id,
                                     Boolean                           Enabled,
                                     [NotNullWhen(false)] out String?  Error,
                                     out Boolean                       NotSaved)
        {

            Error     = null;
            NotSaved  = false;

            lock (updateLock)
            {

                if (!entries.TryGetValue(Id, out var entry))
                {
                    Error = $"There is no accepted chain '{Id}' here.";
                    return false;
                }

                if (entry.Enabled == Enabled)
                    return true;

                entry.Enabled = Enabled;

                if (!TrySaveMeta(entry, out Error))
                {
                    entry.Enabled  = !Enabled;
                    NotSaved       = true;
                    return false;
                }

                OnNotice?.Invoke(
                    LogLevel.Notice,
                    Enabled
                        ? $"Charging stations whose certificate leads to '{entry.Name}' are accepted again."
                        : $"Charging stations whose certificate leads to '{entry.Name}' are no longer accepted."
                );

            }

            return true;

        }

        /// <summary>
        /// Give a chain another name.
        /// </summary>
        public Boolean TryRename(String                            Id,
                                 String                            Name,
                                 [NotNullWhen(false)] out String?  Error)

            => TryRename(Id, Name, out Error, out _);

        /// <summary>
        /// Give a chain another name - and say whether a refusal was the file's
        /// rather than the name's.
        /// </summary>
        /// <param name="NotSaved">True where what is said of the chain could not be written: it keeps the name it had, now and at the next start.</param>
        public Boolean TryRename(String                            Id,
                                 String                            Name,
                                 [NotNullWhen(false)] out String?  Error,
                                 out Boolean                       NotSaved)
        {

            Error     = null;
            NotSaved  = false;

            var name = Name?.Trim() ?? "";

            if (name.Length == 0)
            {
                Error = "A chain needs a name to be found by.";
                return false;
            }

            if (name.Length > MaxNameLength)
            {
                Error = $"The name may be at most {MaxNameLength} characters long.";
                return false;
            }

            lock (updateLock)
            {

                if (!entries.TryGetValue(Id, out var entry))
                {
                    Error = $"There is no accepted chain '{Id}' here.";
                    return false;
                }

                var previous = entry.Name;
                entry.Name   = name;

                if (!TrySaveMeta(entry, out Error))
                {
                    entry.Name  = previous;
                    NotSaved    = true;
                    return false;
                }

            }

            return true;

        }

        #endregion

        #region TryRemove(Id, out Error [, out NotSaved])

        /// <summary>
        /// Stop accepting certificates that lead to this anchor, and throw it
        /// away.
        /// </summary>
        public Boolean TryRemove(String                            Id,
                                 [NotNullWhen(false)] out String?  Error)

            => TryRemove(Id, out Error, out _);

        /// <summary>
        /// Stop accepting certificates that lead to this anchor, and throw it
        /// away - and say whether a refusal was the files' rather than the
        /// request's.
        /// </summary>
        /// <param name="NotSaved">True where a file could not be taken away: the chain stays, whole, and is still accepted.</param>
        public Boolean TryRemove(String                            Id,
                                 [NotNullWhen(false)] out String?  Error,
                                 out Boolean                       NotSaved)
        {

            Error     = null;
            NotSaved  = false;

            var leftOver = new List<String>();

            lock (updateLock)
            {

                if (!entries.TryGetValue(Id, out var entry))
                {
                    Error = $"There is no accepted chain '{Id}' here.";
                    return false;
                }

                if (!TrySetAside(Id, [ "pem", "json" ], leftOver, out var setAside, out var problem))
                {
                    Error     = $"'{Id}' could not be removed from '{Path}': {problem}";
                    NotSaved  = true;
                }

                else
                {

                    OnNotice?.Invoke(LogLevel.Notice, $"'{entry.Name}' is no longer accepted; charging stations whose certificate leads to it will be turned away.");

                    entry.Dispose();
                    entries.Remove(Id);

                    Delete(setAside, leftOver);

                }

            }

            foreach (var file in leftOver)
                OnNotice?.Invoke(LogLevel.Warning, $"'{file}' could not be {(Error is null ? "deleted" : "put back")}, and is left over.");

            if (Error is not null)
                return false;

            return true;

        }

        #endregion

        #region Validate(Certificate, PresentedChain, CheckRevocation)

        /// <summary>
        /// Whether this charging station's certificate leads to something this
        /// local controller was told to accept.
        /// </summary>
        /// <remarks>
        /// Built against the chains in this store alone, with
        /// <see cref="X509ChainTrustMode.CustomRootTrust"/> - so a certificate
        /// signed by a public authority that the operating system happens to
        /// trust is turned away like any other stranger.
        ///
        /// Whether a station without a certificate may connect is not decided
        /// here: that depends on which OCPP security profiles are allowed, and
        /// this store has no opinion on them. It says "no certificate" and
        /// leaves it to the caller.
        /// </remarks>
        public ClientTrustResult Validate(X509Certificate2?  Certificate,
                                          X509Chain?         PresentedChain,
                                          Boolean            CheckRevocation)
        {

            if (Certificate is null)
                return new ClientTrustResult(false, null, null, "no certificate was presented");

            #region What may this certificate be used for?

            // Only when it says: a certificate with no extended key usage at
            // all is unrestricted, which is a different thing from one that
            // lists what it is for and does not list this.
            var extendedKeyUsage = Certificate.Extensions.OfType<X509EnhancedKeyUsageExtension>().FirstOrDefault();

            if (extendedKeyUsage is not null &&
                extendedKeyUsage.EnhancedKeyUsages.Count > 0 &&
                !extendedKeyUsage.EnhancedKeyUsages.Cast<Oid>().Any(oid => oid.Value == ClientAuthenticationOID))
            {
                return new ClientTrustResult(
                           false,
                           null,
                           Certificate.Subject,
                           "the certificate says what it may be used for, and authenticating a client is not among it"
                       );
            }

            #endregion

            using var chain = new X509Chain();

            lock (updateLock)
            {

                var enabled = entries.Values.Where(entry => entry.Enabled).ToArray();

                if (enabled.Length == 0)
                    return new ClientTrustResult(
                               false,
                               null,
                               Certificate.Subject,
                               $"this {NodeWording.Noun} has not been told which certificate authorities to accept charging stations from"
                           );

                foreach (var entry in enabled)
                {

                    chain.ChainPolicy.CustomTrustStore.Add(entry.Anchor);

                    foreach (var intermediate in entry.Intermediates)
                        chain.ChainPolicy.ExtraStore.Add(intermediate);

                }

            }

            // Whatever the station sent along on the way; it is only material
            // to build with, never something that decides trust.
            if (PresentedChain is not null)
                foreach (var element in PresentedChain.ChainElements)
                    chain.ChainPolicy.ExtraStore.Add(element.Certificate);

            chain.ChainPolicy.TrustMode       = X509ChainTrustMode.CustomRootTrust;
            chain.ChainPolicy.RevocationMode  = CheckRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck;
            chain.ChainPolicy.RevocationFlag  = X509RevocationFlag.ExcludeRoot;

            Boolean built;

            try
            {
                built = chain.Build(Certificate);
            }
            catch (Exception e)
            {
                return new ClientTrustResult(false, null, Certificate.Subject, $"the chain could not be built: {e.Message}");
            }

            if (!built)
            {

                var why = chain.ChainStatus.Length > 0
                              ? String.Join("; ", chain.ChainStatus.Select(status => status.StatusInformation.Trim()).Distinct())
                              : "it does not lead to an accepted certificate authority";

                return new ClientTrustResult(false, null, Certificate.Subject, why);

            }

            #region Which of the accepted chains did it lead to?

            var root = chain.ChainElements[^1].Certificate;

            lock (updateLock)
            {

                var anchor = entries.Values.FirstOrDefault(entry => entry.Enabled &&
                                                                    entry.Anchor.Thumbprint == root.Thumbprint);

                // CustomRootTrust should make this impossible; if it ever is
                // possible, the answer is no.
                if (anchor is null)
                    return new ClientTrustResult(false, null, Certificate.Subject, "it leads to a certificate authority that is not accepted here");

                return new ClientTrustResult(true, anchor.Id, Certificate.Subject, $"accepted through '{anchor.Name}'");

            }

            #endregion

        }

        #endregion

        #region CheckExpiry()

        /// <summary>
        /// Say what is about to run out. An anchor that expires stops being
        /// able to vouch for anything, and every charging station under it is
        /// turned away on the same day.
        /// </summary>
        public void CheckExpiry()
        {

            var now = TimeProvider.GetUtcNow();

            lock (updateLock)
            {
                foreach (var entry in entries.Values.Where(entry => entry.Enabled))
                {

                    var days = (Int32) Math.Floor((entry.NotAfter - now).TotalDays);

                    if (days < 0)
                        OnNotice?.Invoke(
                            LogLevel.Critical,
                            $"'{entry.Name}' expired on {entry.NotAfter:yyyy-MM-dd}. Every charging station whose certificate leads to it is being turned away."
                        );

                    else if (days <= 30)
                        OnNotice?.Invoke(
                            days <= 7 ? LogLevel.Error : LogLevel.Warning,
                            $"'{entry.Name}' runs out in {days} day(s), on {entry.NotAfter:yyyy-MM-dd}. " +
                             "Every charging station whose certificate leads to it will be turned away from that day."
                        );

                }
            }

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// The accepted chains, as the web interface reads them.
        /// </summary>
        public JObject ToJSON()
        {

            var now = TimeProvider.GetUtcNow();

            lock (updateLock)

                return new JObject(

                    new JProperty("directory",  Path),
                    new JProperty("now",        now.ToString("o")),
                    new JProperty("enabled",    entries.Values.Count(entry => entry.Enabled)),
                    new JProperty("maxEntries", MaxEntries),

                    new JProperty("entries",    new JArray(
                        entries.Values.OrderByDescending(entry => entry.AddedAt).
                                       Select(entry => entry.ToJSON(now))
                    ))

                );

        }

        #endregion


        #region (private) TryLoadEntry(Id, out Entry, out Error)

        private Boolean TryLoadEntry(String                                    Id,
                                     [NotNullWhen(true)]  out ClientTrustEntry? Entry,
                                     [NotNullWhen(false)] out String?           Error)
        {

            Entry  = null;
            Error  = null;

            var collection = new X509Certificate2Collection();

            try
            {
                collection.ImportFromPem(File.ReadAllText(System.IO.Path.Combine(Path, $"{Id}.pem")));
            }
            catch (Exception e)
            {
                Error = $"it is not a certificate: {e.Message}";
                return false;
            }

            if (collection.Count == 0)
            {
                Error = "it holds no certificate.";
                return false;
            }

            // The anchor was written first.
            var anchor = collection[0];

            if (Convert.ToHexStringLower(SHA256.HashData(anchor.RawData).AsSpan(0, 8)) != Id)
            {
                Error = "the file does not hold the certificate its name says it does.";
                return false;
            }

            var name     = anchor.Subject;
            var addedAt  = TimeProvider.GetUtcNow();
            var enabled  = true;

            var metaPath = System.IO.Path.Combine(Path, $"{Id}.json");

            if (File.Exists(metaPath))
            {
                try
                {

                    var meta = JObject.Parse(File.ReadAllText(metaPath));

                    name     = meta.Value<String>("name")    ?? name;
                    enabled  = meta.Value<Boolean?>("enabled") ?? true;

                    if (DateTimeOffset.TryParse(meta.Value<String>("addedAt"),
                                                System.Globalization.CultureInfo.InvariantCulture,
                                                System.Globalization.DateTimeStyles.RoundtripKind,
                                                out var parsed))
                        addedAt = parsed;

                }
                catch (Exception e)
                {
                    Error = $"'{metaPath}' could not be read: {e.Message}";
                    return false;
                }
            }

            var entry = new ClientTrustEntry(Id, name, addedAt, enabled, anchor);

            for (var i = 1; i < collection.Count; i++)
                entry.Intermediates.Add(collection[i]);

            #region What is worth saying about it

            var now = TimeProvider.GetUtcNow();

            if (!entry.IsCertificateAuthority)
                entry.Warnings.Add(
                    "This is not a certificate authority, so it vouches for nothing but itself: only a charging station " +
                    "presenting this very certificate will be let in."
                );

            if (entry.NotAfter < now)
                entry.Warnings.Add($"Expired on {entry.NotAfter:yyyy-MM-dd}; nothing leading to it can be accepted any more.");

            else if (entry.NotBefore > now)
                entry.Warnings.Add($"Not valid until {entry.NotBefore:yyyy-MM-dd}.");

            #endregion

            Entry = entry;
            return true;

        }

        #endregion

        #region (private) TrySaveMeta(Entry, out Error)

        private Boolean TrySaveMeta(ClientTrustEntry                  Entry,
                                    [NotNullWhen(false)] out String?  Error)
        {

            Error = null;

            var file = System.IO.Path.Combine(Path, $"{Entry.Id}.json");

            try
            {

                // Beside what it replaces, and moved over it in one step:
                // written in its place, a change the disk ran out in cut what
                // was said of the chain off, and at the next start the chain
                // was not read at all - its charging stations turned away.
                WriteFile(
                    file + ".tmp",
                    new JObject(
                        new JProperty("id",       Entry.Id),
                        new JProperty("name",     Entry.Name),
                        new JProperty("addedAt",  Entry.AddedAt.ToString("o")),
                        new JProperty("enabled",  Entry.Enabled)
                    ).ToString(Formatting.Indented) + Environment.NewLine
                );

                File.Move(file + ".tmp", file, overwrite: true);

                return true;

            }
            catch (Exception e)
            {

                Error = $"'{Entry.Id}' could not be written to '{Path}': {e.Message}";

                // What was written of it, beside what it was to replace.
                Forget(Entry.Id, "json.tmp");

                return false;

            }

        }

        #endregion

        #region (private) CreateDirectory() / Complain(Problem) / WriteFile(Target, Content)

        /// <summary>
        /// The directory, its owner's alone - see <see cref="OwnerOnly"/>.
        /// </summary>
        /// <remarks>
        /// Nothing in here is secret - these are certificates, and a
        /// certificate is a public document - but whoever may write here
        /// decides which charging stations are let in.
        /// </remarks>
        private void CreateDirectory()
        {

            if (Directory.Exists(Path))
                return;

            OwnerOnly.Directory(Path, Complain);

        }

        /// <summary>
        /// Say what could not be restricted: a store others may change is worth
        /// a warning, not worth failing over.
        /// </summary>
        private void Complain(String Problem)

            => OnNotice?.Invoke(LogLevel.Warning, Problem);

        /// <summary>
        /// One file of this store written - past <see cref="BeforeWriting"/>
        /// first.
        /// </summary>
        private void WriteFile(String Target, String Content)
        {

            BeforeWriting?.Invoke(Target);

            File.WriteAllText(Target, Content);

        }

        #endregion

        #region (private) Forget(Id, params Extensions)

        /// <summary>
        /// Take the files of one chain that were written for something that
        /// did not go in away again - as far as that goes: one that cannot be
        /// taken away either is left where it is.
        /// </summary>
        private void Forget(String Id, params String[] Extensions)
        {
            foreach (var extension in Extensions)
            {
                try
                {
                    var file = System.IO.Path.Combine(Path, $"{Id}.{extension}");
                    if (File.Exists(file))
                        File.Delete(file);
                }
                catch (Exception)
                {
                    // Left where it is, see above.
                }
            }
        }

        #endregion

        #region (private) TrySetAside(Id, Extensions, LeftOver, out SetAside, out Problem) / Delete(SetAside, LeftOver)

        /// <summary>
        /// The files of one chain moved aside, as "*.removed", all of them or
        /// none: where one cannot be, those moved already are moved back.
        /// </summary>
        /// <remarks>
        /// So that a chain is taken away whole or not at all. Deleted one after
        /// the other, a description somebody held open left its certificates
        /// gone: the chain was accepted until the next start and gone after it
        /// (found by the charging station). What cannot be moved back is left
        /// over under its "*.removed" name, which the next start does not read,
        /// and said in the log.
        /// </remarks>
        private Boolean TrySetAside(String                            Id,
                                    String[]                          Extensions,
                                    List<String>                      LeftOver,
                                    out List<String>                  SetAside,
                                    [NotNullWhen(false)] out String?  Problem)
        {

            SetAside  = [];
            Problem   = null;

            foreach (var extension in Extensions)
            {

                var file = System.IO.Path.Combine(Path, $"{Id}.{extension}");

                if (!File.Exists(file))
                    continue;

                try
                {
                    BeforeRemoving?.Invoke(file);
                    File.Move(file, file + ".removed", overwrite: true);
                    SetAside.Add(file);
                }
                catch (Exception e)
                {

                    Problem = e.Message;

                    foreach (var moved in SetAside)
                    {
                        try
                        {
                            BeforeRemoving?.Invoke(moved + ".removed");
                            File.Move(moved + ".removed", moved);
                        }
                        catch (Exception)
                        {
                            LeftOver.Add(moved + ".removed");
                        }
                    }

                    SetAside.Clear();

                    return false;

                }

            }

            return true;

        }

        /// <summary>
        /// The files moved aside deleted; what cannot be is left over under its
        /// "*.removed" name, which the next start does not read.
        /// </summary>
        private void Delete(List<String>  SetAside,
                            List<String>  LeftOver)
        {
            foreach (var file in SetAside)
            {
                try
                {
                    BeforeRemoving?.Invoke(file + ".removed");
                    File.Delete(file + ".removed");
                }
                catch (Exception)
                {
                    LeftOver.Add(file + ".removed");
                }
            }
        }

        #endregion

        #region Dispose()

        public void Dispose()
        {
            lock (updateLock)
            {

                foreach (var entry in entries.Values)
                    entry.Dispose();

                entries.Clear();

            }
        }

        #endregion

        #region (override) ToString()

        public override String ToString()
        {
            lock (updateLock)
                return $"{entries.Values.Count(entry => entry.Enabled)} of {entries.Count} chain(s) accepted, in '{Path}'";
        }

        #endregion

    }


    /// <summary>
    /// What came of holding a charging station's certificate against the
    /// chains this local controller accepts.
    /// </summary>
    /// <param name="Accepted">Whether the certificate leads to an accepted chain.</param>
    /// <param name="AnchorId">Which chain it led to, when it led to one.</param>
    /// <param name="Subject">Who the certificate says the station is.</param>
    /// <param name="Reason">Why, in a sentence that can go into a log.</param>
    public sealed record ClientTrustResult(Boolean  Accepted,
                                           String?  AnchorId,
                                           String?  Subject,
                                           String   Reason);

}
