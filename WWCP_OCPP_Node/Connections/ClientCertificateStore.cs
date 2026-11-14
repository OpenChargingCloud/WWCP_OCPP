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

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

using org.GraphDefined.Vanaheimr.Hermod.PKI;

using cloud.charging.open.protocols.WWCP.Node.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    /// <summary>
    /// The keys and certificates this charging station holds up when it dials
    /// a back end.
    /// </summary>
    /// <remarks>
    /// A key is generated here, a signing request is made out of it and handed
    /// to whoever issues certificates for this fleet, and the answer is brought
    /// back and put beside the key. Nothing else ever moves: the private key is
    /// written once, into a file only its owner may read, and never leaves this
    /// directory - there is no import, because a key that arrived from
    /// somewhere else is a key somebody else has a copy of.
    ///
    /// Thirteen kinds of key can be asked for, from the curve every back end
    /// understands to the post-quantum ones - see <see cref="KeyAlgorithm"/> in
    /// Hermod, which is also what the local controller uses, so that the two
    /// ends of the same conversation offer the same list.
    ///
    /// <b>Making a key and holding it up are different questions.</b> All
    /// thirteen can be generated and made into a request that verifies. Whether
    /// this platform can then load the certificate that comes back together
    /// with its key is something else, found out by trying: .NET has no key
    /// object at all for an Ed448 or an ML-DSA key, and such a certificate is
    /// kept and passed over rather than refused - the runtime that cannot take
    /// it today may take it after an update.
    /// </remarks>
    public sealed class ClientCertificateStore : IDisposable
    {

        #region Data

        /// <summary>
        /// The directory beside the process, when nobody says otherwise.
        /// </summary>
        public const String  DefaultDirectoryName  = "ocpp-client-keys";

        /// <summary>
        /// The key algorithms that may be asked for - see
        /// <see cref="KeyAlgorithm"/>, which is where they and their several
        /// awkwardnesses live.
        /// </summary>
        public static IReadOnlyList<KeyAlgorithm> Algorithms
            => KeyAlgorithm.All;

        /// <summary>
        /// The algorithm a key is generated with when nothing says otherwise.
        /// </summary>
        public const String  DefaultAlgorithm      = KeyAlgorithm.DefaultId;

        /// <summary>
        /// How long before a certificate expires this store starts saying so.
        /// </summary>
        /// <remarks>
        /// Thirty days is roughly how long it takes to get a certificate out of
        /// an organisation that has to ask somebody. The later warnings are
        /// there for when the first one was read by nobody.
        /// </remarks>
        public static readonly Int32[] WarnDaysBefore = [ 30, 14, 7, 3, 1 ];

        /// <summary>
        /// The longest a subject may be written.
        /// </summary>
        public const Int32   MaxSubjectLength      = 200;

        private readonly Object                                     updateLock  = new ();
        private readonly Dictionary<String, ClientCertificateEntry>  entries    = [];

        /// <summary>
        /// The public key of every entry, which is what a certificate that
        /// comes back is matched against to find out whose it is.
        /// </summary>
        private readonly Dictionary<String, Byte[]>                 publicKeys  = [];

        #endregion

        #region Properties

        /// <summary>
        /// The directory the keys and certificates live in.
        /// </summary>
        public String        Path          { get; }

        /// <summary>
        /// The clock that decides which certificate is in its window.
        /// </summary>
        /// <remarks>
        /// Handed in rather than reached for, and for a sharper reason than
        /// usual: this is one of the few places in the station where a wrong
        /// clock silently does the wrong thing. A station that boots believing
        /// it is 1970 would find no certificate valid and could not dial home
        /// to be told otherwise.
        /// </remarks>
        public TimeProvider  TimeProvider  { get; }

        /// <summary>
        /// Everything in the directory, newest key first.
        /// </summary>
        public IReadOnlyList<ClientCertificateEntry> Entries
        {
            get
            {
                lock (updateLock)
                    return [.. entries.Values.OrderByDescending(entry => entry.CreatedAt)];
            }
        }

        /// <summary>
        /// The one this station would hold up at this moment, or null while
        /// there is none it could.
        /// </summary>
        public ClientCertificateEntry? InUse
        {
            get
            {
                lock (updateLock)
                    return Pick(TimeProvider.GetUtcNow());
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Something happened here that belongs in the log of the station: a
        /// key generated, a certificate taken in, one that expired with no
        /// replacement, a file that could not be read.
        /// </summary>
        /// <remarks>
        /// An event rather than an event log handed in, so that this store can
        /// be built and tested without one - and so that what level a sentence
        /// is logged at stays the decision of the thing that owns the log.
        /// </remarks>
        public event Action<LogLevel, String>? OnNotice;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The keys and certificates in the given directory; it need not exist
        /// yet.
        /// </summary>
        public ClientCertificateStore(String?        Path           = null,
                                      TimeProvider?  TimeProvider   = null)
        {

            this.Path          = System.IO.Path.GetFullPath(Path ?? DefaultDirectoryName);
            this.TimeProvider  = TimeProvider ?? System.TimeProvider.System;

            Reload();

        }

        #endregion


        #region Reload()

        /// <summary>
        /// Read the directory again, from nothing.
        /// </summary>
        /// <remarks>
        /// A file that cannot be read is skipped with a line in the log rather
        /// than thrown: one unreadable key must not stop a station that has
        /// another which works, and the one that works is frequently the one
        /// keeping it connected.
        /// </remarks>
        public void Reload()
        {

            lock (updateLock)
            {

                foreach (var entry in entries.Values)
                    entry.Dispose();

                entries.Clear();
                publicKeys.Clear();

                if (!Directory.Exists(Path))
                    return;

                foreach (var keyFile in Directory.GetFiles(Path, "*.key.pem").OrderBy(file => file))
                {

                    var id = System.IO.Path.GetFileName(keyFile);
                    id = id[..id.IndexOf(".key.pem", StringComparison.Ordinal)];

                    try
                    {

                        if (TryLoadEntry(id, out var entry, out var publicKey, out var problem))
                        {
                            entries   [id] = entry;
                            publicKeys[id] = publicKey;
                        }

                        else
                            OnNotice?.Invoke(LogLevel.Error, $"The key '{id}' in '{Path}' could not be read and is being ignored: {problem}");

                    }
                    catch (Exception e)
                    {
                        OnNotice?.Invoke(LogLevel.Error, $"The key '{id}' in '{Path}' could not be read and is being ignored: {e.Message}");
                    }

                }

                CheckEverything(TimeProvider.GetUtcNow());

            }

        }

        #endregion

        #region TryCreateKey  (Subject, Algorithm, [Asks,] out Id, out CSR, out Error [, out NotSaved])

        /// <summary>
        /// A new key, and the signing request to be handed to whoever issues
        /// certificates for this station.
        /// </summary>
        /// <remarks>
        /// The request asks for a client certificate and says so, rather than
        /// hoping: a certificate authority handed a request without an extended
        /// key usage frequently issues something that is not one, and a station
        /// finds that out at the next handshake.
        ///
        /// No subject alternative names are asked for. A station is not
        /// something anybody dials, so there is nothing it is reachable as -
        /// what a back end recognises it by is the subject, which is why that
        /// is the one thing somebody has to fill in.
        /// </remarks>
        /// <param name="Subject">Who this station says it is - plain text is read as a common name, a full distinguished name is taken as written.</param>
        /// <param name="Algorithm">Which kind of key, or null for the usual one.</param>
        public Boolean TryCreateKey(String                            Subject,
                                    String?                           Algorithm,
                                    [NotNullWhen(true)]  out String?  Id,
                                    [NotNullWhen(true)]  out String?  CSR,
                                    [NotNullWhen(false)] out String?  Error)

            => TryCreateKey(Subject, Algorithm, out Id, out CSR, out Error, out _);

        /// <summary>
        /// A new key and its signing request - and whether a refusal was the
        /// files' rather than the request's.
        /// </summary>
        /// <param name="Subject">Who this station says it is - plain text is read as a common name, a full distinguished name is taken as written.</param>
        /// <param name="Algorithm">Which kind of key, or null for the usual one.</param>
        /// <param name="Id">What the new key is called here.</param>
        /// <param name="CSR">Its signing request, as PEM.</param>
        /// <param name="Error">Why there is none.</param>
        /// <param name="NotSaved">True where its files could not be written: nothing about the request was wrong, and nothing of the key is kept.</param>
        public Boolean TryCreateKey(String                            Subject,
                                    String?                           Algorithm,
                                    [NotNullWhen(true)]  out String?  Id,
                                    [NotNullWhen(true)]  out String?  CSR,
                                    [NotNullWhen(false)] out String?  Error,
                                    out Boolean                       NotSaved)

            => TryCreateKey(Subject, Algorithm, null, out Id, out CSR, out Error, out NotSaved);

        /// <summary>
        /// A new key and its signing request, made to a profile and asking for
        /// more than a client certificate where somebody said so.
        /// </summary>
        /// <remarks>
        /// Signing in as a client is asked for whatever else is: that is what
        /// a key here is for, and not a choice. A signature is asked of the
        /// key for the same reason, and enciphering a key where its kind can.
        /// Everything in <paramref name="Asks"/> is added to that, and what a
        /// kind of key cannot do is refused here, with the reason, rather than
        /// by a certificate authority without one.
        /// </remarks>
        /// <param name="Subject">Who this station says it is - plain text is read as a common name, a full distinguished name is taken as written.</param>
        /// <param name="Algorithm">Which kind of key, or null for the usual one.</param>
        /// <param name="Asks">The template, its role, and the usages asked for beside the usual ones; null for a client certificate and nothing else.</param>
        /// <param name="Id">What the new key is called here.</param>
        /// <param name="CSR">Its signing request, as PEM.</param>
        /// <param name="Error">Why there is none.</param>
        /// <param name="NotSaved">True where its files could not be written: nothing about the request was wrong, and nothing of the key is kept.</param>
        public Boolean TryCreateKey(String                            Subject,
                                    String?                           Algorithm,
                                    ClientKeyAsks?                    Asks,
                                    [NotNullWhen(true)]  out String?  Id,
                                    [NotNullWhen(true)]  out String?  CSR,
                                    [NotNullWhen(false)] out String?  Error,
                                    out Boolean                       NotSaved)
        {

            Id        = null;
            CSR       = null;
            Error     = null;
            NotSaved  = false;

            #region What was asked for

            var algorithm = KeyAlgorithm.Find(Algorithm ?? DefaultAlgorithm);

            if (algorithm is null)
            {
                Error = $"'{Algorithm}' is not a key this {NodeWording.LongNoun} generates ({String.Join(", ", KeyAlgorithm.All.Select(one => one.Id))}).";
                return false;
            }

            var subject = Subject.Trim();

            if (subject.Length == 0)
            {
                Error = $"A certificate has to say who this {NodeWording.Noun} is, or no back end will know whose it is. " +
                        $"Fill in a subject - the identity this {NodeWording.Noun} dials with is the usual answer.";
                return false;
            }

            if (subject.Length > MaxSubjectLength)
            {
                Error = $"The subject may be at most {MaxSubjectLength} characters long.";
                return false;
            }

            // Plain text is what somebody types, so it is read as a common
            // name; a full distinguished name is taken as written.
            var distinguishedName = subject.Contains('=')
                                        ? subject
                                        : $"CN={subject.Replace("\\", "\\\\").Replace(",", "\\,").Replace("=", "\\=")}";

            X509Name subjectName;
            String   subjectText;

            try
            {
                // Read by .NET first because it is stricter about what somebody
                // may type, then handed on in the form Bouncy Castle signs with
                // - so the message about a bad subject is the readable one and
                // the request is still built by the half that knows every
                // algorithm.
                var x500     = new X500DistinguishedName(distinguishedName);
                subjectText  = x500.Name;
                subjectName  = new X509Name(x500.Name);
            }
            catch (Exception e)
            {
                Error = $"'{subject}' is not a usable certificate subject: {e.Message}";
                return false;
            }

            #endregion

            #region What it is asked for

            var template = CSRTemplate.Find(Asks?.Template is { Length: > 0 } named ? named : CSRTemplate.DefaultId);

            if (template is null)
            {
                Error = $"'{Asks?.Template}' is not a template this {NodeWording.LongNoun} makes a signing request to ({String.Join(", ", CSRTemplate.All.Select(one => one.Id))}).";
                return false;
            }

            var role = Asks?.Role?.Trim() is { Length: > 0 } said ? said : null;

            if (template.AsksForARole)
            {

                if (role is null)
                {
                    Error = $"A certificate for {template.Name} has to say which role its holder may play ({String.Join(", ", template.Roles)}, or the one its server knows).";
                    return false;
                }

                if (role.Length > CSRTemplate.MaxRoleLength || role.Any(Char.IsControl))
                {
                    Error = $"A role may be at most {CSRTemplate.MaxRoleLength} characters long, and all of them printable.";
                    return false;
                }

            }

            else if (role is not null)
            {
                Error = $"A certificate for {template.Name} has no role to ask for. Leave it out, or pick the template that has one.";
                return false;
            }

            // What the kind of key brings by itself, then what the template
            // and somebody ticking boxes add - each once, in that order.
            var keyUsageBits   = algorithm.CanEncipher
                                     ? KeyUsage.DigitalSignature | KeyUsage.KeyEncipherment
                                     : KeyUsage.DigitalSignature;

            var keyUsageNames  = new List<String> { "digitalSignature" };

            if (algorithm.CanEncipher)
                keyUsageNames.Add("keyEncipherment");

            foreach (var (name, fromTemplate) in template.KeyUsages.Select(name => (name, true)).
                                                 Concat((Asks?.KeyUsages ?? []).Select(name => (name, false))))
            {

                // Named again, as the page of an older version would: asked
                // for anyway.
                if (keyUsageNames.Contains(name.Trim(), StringComparer.OrdinalIgnoreCase))
                    continue;

                var usage = CSRKeyUsage.Find(name);

                if (usage is null)
                {
                    Error = $"'{name}' is not a key usage this {NodeWording.LongNoun} asks for ({String.Join(", ", CSRKeyUsage.All.Select(one => one.Id))}).";
                    return false;
                }

                if (!usage.Fits(algorithm))
                {

                    // What a template would like and the kind of key cannot
                    // do is left out; what somebody ticked is theirs to hear.
                    if (fromTemplate)
                        continue;

                    Error = $"A key of the kind {algorithm.Name} cannot be used for {usage.Id}: {usage.Remark}";
                    return false;

                }

                keyUsageBits |= usage.Bit;
                keyUsageNames.Add(usage.Id);

            }

            var purposes      = new List<DerObjectIdentifier>();
            var purposeNames  = new List<String> { CSRPurpose.ClientAuth };

            foreach (var name in template.ExtendedKeyUsages.Concat(Asks?.ExtendedKeyUsages ?? []))
            {

                if (name.Trim().Length == 0)
                    continue;

                if (!CSRPurpose.TryParse(name, out var purpose, out var written))
                {
                    Error = $"'{name}' is neither an extended key usage this {NodeWording.LongNoun} knows ({String.Join(", ", CSRPurpose.All.Select(one => one.Id))}) nor an object identifier such as 1.3.6.1.5.5.7.3.9.";
                    return false;
                }

                if (purposeNames.Contains(written))
                    continue;

                purposes.    Add(purpose);
                purposeNames.Add(written);

            }

            #endregion

            AsymmetricCipherKeyPair pair;

            try
            {
                pair = algorithm.Generate();
            }
            catch (Exception e)
            {
                Error = $"No {algorithm.Name} key could be generated: {e.Message}";
                return false;
            }

            var publicKey  = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pair.Public).GetDerEncoded();
            var id         = KeyId(publicKey);

            lock (updateLock)
            {

                if (entries.ContainsKey(id))
                {
                    // Two identical keys is not something that happens by
                    // chance; it happens when a directory was copied.
                    Error = $"A key with the identification '{id}' is already here.";
                    return false;
                }

                String csr;

                try
                {
                    csr = PKIFactory.GenerateCertificateSigningRequest(
                              pair,
                              subjectName,
                              algorithm,
                              null,
                              KeyPurposeID.id_kp_clientAuth,
                              keyUsageBits,
                              purposes,
                              template.ExtensionsFor(role)
                          ).ToPEM();
                }
                catch (Exception e)
                {
                    Error = $"The signing request could not be made: {e.Message}";
                    return false;
                }

                var createdAt = TimeProvider.GetUtcNow();

                #region Written down, the key first and with its own permissions

                try
                {

                    CreateDirectory();

                    OwnerOnlyFile.Write(
                        FilePath(id, "key.pem"),
                        PemEncoding.WriteString(
                            "PRIVATE KEY",
                            PrivateKeyInfoFactory.CreatePrivateKeyInfo(pair.Private).GetDerEncoded()
                        ) + Environment.NewLine
                    );

                    File.WriteAllText(FilePath(id, "csr.pem"), csr);

                    File.WriteAllText(
                        FilePath(id, "json"),
                        new JObject(
                            new JProperty("id",         id),
                            new JProperty("algorithm",  algorithm.Id),
                            new JProperty("createdAt",  createdAt.ToString("o")),
                            new JProperty("subject",    subjectText),
                            new JProperty("template",   template.Id),
                            new JProperty("role",       role),
                            new JProperty("keyUsages",          new JArray(keyUsageNames)),
                            new JProperty("extendedKeyUsages",  new JArray(purposeNames))
                        ).ToString(Formatting.Indented) + Environment.NewLine
                    );

                }
                catch (Exception e)
                {

                    // What was written of it goes again: a key without its
                    // description is one every start would complain about, and
                    // keep.
                    foreach (var extension in new[] { "key.pem", "csr.pem", "json" })
                    {
                        try
                        {
                            File.Delete(FilePath(id, extension));
                        }
                        catch
                        { }
                    }

                    Error     = $"The key could not be written to '{Path}': {e.Message}";
                    NotSaved  = true;
                    return false;

                }

                #endregion

                var entry = new ClientCertificateEntry(id, algorithm.Name, createdAt, subjectText) {
                                Template           = template.Id,
                                Role               = role,
                                KeyUsages          = keyUsageNames,
                                ExtendedKeyUsages  = purposeNames
                            };

                entries   [id] = entry;
                publicKeys[id] = publicKey;

                Id   = id;
                CSR  = csr;

            }

            OnNotice?.Invoke(LogLevel.Notice, $"A new {algorithm.Name} key '{id}' was generated and a signing request for {subjectText}" +
                                              (template.Id != CSRTemplate.DefaultId ? $", made to the template {template.Name}{(role is not null ? $" with the role {role}" : "")}," : "") +
                                              " is waiting to be collected.");

            return true;

        }

        #endregion

        #region TryReadCSR    (Id, out CSR, out Error)

        /// <summary>
        /// The signing request of a key, to be handed to a certificate
        /// authority.
        /// </summary>
        public Boolean TryReadCSR(String                            Id,
                                  [NotNullWhen(true)]  out String?  CSR,
                                  [NotNullWhen(false)] out String?  Error)
        {

            CSR    = null;
            Error  = null;

            if (!Known(Id))
            {
                Error = $"There is no key '{Id}' here.";
                return false;
            }

            var path = FilePath(Id, "csr.pem");

            if (!File.Exists(path))
            {
                Error = $"The signing request of '{Id}' is no longer in '{Path}'.";
                return false;
            }

            try
            {
                CSR = File.ReadAllText(path);
                return true;
            }
            catch (Exception e)
            {
                Error = $"The signing request of '{Id}' could not be read: {e.Message}";
                return false;
            }

        }

        #endregion

        #region TryAddCertificate(PEM, out Id, out Warnings, out Error [, out NotSaved])

        /// <summary>
        /// Take a certificate that came back from a certificate authority,
        /// together with whatever intermediates were sent with it.
        /// </summary>
        /// <remarks>
        /// <b>Everything that can be checked is checked here and not at the
        /// moment the station next dials.</b> A certificate uploaded today to
        /// take over in two days takes over at whatever hour it becomes valid,
        /// with nobody watching; a mistake found then is found by a station
        /// that can no longer connect. So a certificate whose key is not here,
        /// or that has already expired, is refused now, while somebody is
        /// looking at the screen.
        ///
        /// What is reported but not refused is everything that may legitimately
        /// look wrong from in here: a chain this station cannot verify may be
        /// from a private authority the back end knows perfectly well.
        /// </remarks>
        /// <param name="PEM">The certificate, and any intermediates, as PEM.</param>
        public Boolean TryAddCertificate(String                            PEM,
                                         [NotNullWhen(true)]  out String?  Id,
                                         out IReadOnlyList<String>         Warnings,
                                         [NotNullWhen(false)] out String?  Error)

            => TryAddCertificate(PEM, out Id, out Warnings, out Error, out _);

        /// <summary>
        /// Take a certificate that came back from a certificate authority - and
        /// say whether a refusal was the file's rather than the certificate's.
        /// </summary>
        /// <param name="PEM">The certificate, and any intermediates, as PEM.</param>
        /// <param name="Id">The key it belongs to.</param>
        /// <param name="Warnings">What is said about it without refusing it.</param>
        /// <param name="Error">Why it was not taken.</param>
        /// <param name="NotSaved">True where its file could not be written: nothing about the certificate was wrong, and the key keeps the certificate it had, if any.</param>
        public Boolean TryAddCertificate(String                            PEM,
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
                        $"with the certificate of this {NodeWording.Noun} first and any intermediates after it.";
                return false;
            }

            #endregion

            var subject   = "";
            var notAfter  = default(DateTimeOffset);

            lock (updateLock)
            {

                #region Whose is it?

                X509Certificate2? leaf    = null;
                String?           leafId  = null;

                foreach (var certificate in uploaded)
                {

                    var spki  = certificate.PublicKey.ExportSubjectPublicKeyInfo();
                    var match = publicKeys.FirstOrDefault(pair => pair.Value.AsSpan().SequenceEqual(spki));

                    if (match.Key is not null)
                    {
                        leaf    = certificate;
                        leafId  = match.Key;
                        break;
                    }

                }

                if (leaf is null || leafId is null)
                {
                    Error = $"None of the certificates in this file belongs to a key of this {NodeWording.LongNoun}. " +
                            "A certificate is only usable here if it answers a signing request made here.";
                    return false;
                }

                #endregion

                var now = TimeProvider.GetUtcNow();

                #region Already expired

                if (new DateTimeOffset(leaf.NotAfter.ToUniversalTime()) <= now)
                {
                    Error = $"This certificate expired on {leaf.NotAfter.ToUniversalTime():yyyy-MM-dd} and can never be used. " +
                            $"By the clock of this {NodeWording.LongNoun} it is now {now:yyyy-MM-dd}.";
                    return false;
                }

                notAfter = new DateTimeOffset(leaf.NotAfter.ToUniversalTime());

                #endregion

                var pool           = uploaded.Cast<X509Certificate2>().
                                              Where(certificate => !certificate.RawData.AsSpan().SequenceEqual(leaf.RawData)).
                                              ToList();

                var intermediates  = OrderTowardsTheRoot(leaf, pool);

                #region Written down

                // To a file of its own first, and moved over the one the key may
                // have already: written in place, a certificate that did not fit
                // on the disk took the one before it with it.
                var certificateFile  = FilePath(leafId, "cert.pem");
                var temporary        = certificateFile + ".tmp";

                try
                {

                    CreateDirectory();

                    File.WriteAllText(
                        temporary,
                        String.Join(
                            Environment.NewLine,
                            new[] { leaf }.Concat(intermediates).
                                Select(certificate => PemEncoding.WriteString("CERTIFICATE", certificate.RawData))
                        ) + Environment.NewLine
                    );

                    File.Move(temporary, certificateFile, overwrite: true);

                }
                catch (Exception e)
                {

                    try
                    {
                        if (File.Exists(temporary))
                            File.Delete(temporary);
                    }
                    catch
                    { }

                    Error     = $"The certificate could not be written to '{Path}': {e.Message}";
                    NotSaved  = true;
                    return false;

                }

                #endregion

                // Read back the way it will be read at the next start, rather
                // than assembled from what is still in hand: a certificate that
                // cannot be loaded from its own file is one that would be gone
                // after a restart, and that is worth finding out now.
                if (!TryLoadEntry(leafId, out var entry, out var publicKey, out var problem))
                {
                    Error = $"The certificate was written but cannot be read back: {problem}";
                    return false;
                }

                if (entries.TryGetValue(leafId, out var replaced))
                    replaced.Dispose();

                entries   [leafId] = entry;
                publicKeys[leafId] = publicKey;

                CheckEntry(entry, now);

                Id        = leafId;
                Warnings  = [.. entry.Warnings];
                subject   = entry.Certificate?.Subject ?? leafId;

            }

            OnNotice?.Invoke(
                LogLevel.Notice,
                $"A certificate for '{subject}' was taken in, valid until {notAfter:yyyy-MM-dd}." +
                (Warnings.Count > 0 ? $" {String.Join(" ", Warnings)}" : "")
            );

            return true;

        }

        #endregion

        #region TryRemove     (Id, out Error [, out NotSaved])

        /// <summary>
        /// Take a key and whatever belongs to it away, for good.
        /// </summary>
        /// <remarks>
        /// The one this station is holding up is not removed while it is: a
        /// station that deleted the certificate it dials with would come back
        /// from its next restart unable to connect, and the way to notice would
        /// be a car park that stopped reporting.
        /// </remarks>
        public Boolean TryRemove(String                            Id,
                                 [NotNullWhen(false)] out String?  Error)

            => TryRemove(Id, out Error, out _);

        /// <summary>
        /// Take a key and whatever belongs to it away - and say whether a
        /// refusal was its files' rather than the request's.
        /// </summary>
        /// <remarks>
        /// Every file of it is taken aside first, and only then deleted, so
        /// that one which cannot be taken aside - held open, or on a disk gone
        /// read-only - leaves every one of them where it was. Deleted one after
        /// the other, the key went while its description stayed: still listed,
        /// and gone at the next start.
        /// </remarks>
        /// <param name="Id">The key.</param>
        /// <param name="Error">Why it is still here.</param>
        /// <param name="NotSaved">True where its files could not be taken away: nothing about the request was wrong, and the key stays, whole.</param>
        public Boolean TryRemove(String                            Id,
                                 [NotNullWhen(false)] out String?  Error,
                                 out Boolean                       NotSaved)
        {

            Error     = null;
            NotSaved  = false;

            var leftOver  = new List<String>();
            var stuck     = (String?) null;

            lock (updateLock)
            {

                if (!entries.TryGetValue(Id, out var entry))
                {
                    Error = $"There is no key '{Id}' here.";
                    return false;
                }

                if (Pick(TimeProvider.GetUtcNow())?.Id == Id)
                {
                    Error = $"This is the certificate the {NodeWording.Noun} is holding up. Take another one into use first, " +
                            $"or this {NodeWording.Noun} will come back from its next restart unable to connect.";
                    return false;
                }

                var aside = new List<(String File, String Aside)>();

                foreach (var extension in new[] { "key.pem", "csr.pem", "cert.pem", "json" })
                {

                    var path = FilePath(Id, extension);

                    try
                    {
                        if (File.Exists(path))
                        {
                            File.Move(path, path + ".removed", overwrite: true);
                            aside.Add((path, path + ".removed"));
                        }
                    }
                    catch (Exception e)
                    {

                        foreach (var (file, removed) in aside)
                        {
                            try
                            {
                                File.Move(removed, file);
                            }
                            catch
                            {
                                leftOver.Add(removed);
                            }
                        }

                        stuck = $"'{path}' could not be removed: {e.Message}";

                        break;

                    }

                }

                if (stuck is null)
                {

                    foreach (var (_, removed) in aside)
                    {
                        try
                        {
                            File.Delete(removed);
                        }
                        catch
                        {
                            leftOver.Add(removed);
                        }
                    }

                    entry.Dispose();

                    entries.Remove(Id);
                    publicKeys.Remove(Id);

                }

            }

            foreach (var removed in leftOver)
                OnNotice?.Invoke(LogLevel.Warning, $"'{removed}' could not be {(stuck is not null ? "put back" : "deleted")}, and is left over.");

            if (stuck is not null)
            {
                Error     = stuck;
                NotSaved  = true;
                return false;
            }

            OnNotice?.Invoke(LogLevel.Notice, $"The key '{Id}' and everything belonging to it were removed.");

            return true;

        }

        #endregion

        #region CheckExpiry()

        /// <summary>
        /// Look over everything again and say what is worth saying.
        /// </summary>
        /// <remarks>
        /// Meant to be called now and again by whoever owns this store: a
        /// certificate does not expire when somebody opens a page, and a
        /// station whose certificate ran out in the night should have said so
        /// in the night.
        /// </remarks>
        public void CheckExpiry()
        {

            var now = TimeProvider.GetUtcNow();

            ClientCertificateEntry? inUse;

            lock (updateLock)
            {
                CheckEverything(now);
                inUse = Pick(now);
            }

            if (inUse is null)
            {
                OnNotice?.Invoke(
                    LogLevel.Warning,
                    entries.Count == 0
                        ? $"This {NodeWording.Noun} has no certificate of its own. It can only dial a back end that does not ask for one."
                        : $"None of this {NodeWording.Noun}'s certificates can be held up at the moment - every one of them has expired, is not valid yet, or cannot be loaded here."
                );
                return;
            }

            var daysLeft = (Int32) Math.Floor((inUse.NotAfter!.Value - now).TotalDays);

            if (WarnDaysBefore.Contains(daysLeft))
                OnNotice?.Invoke(
                    daysLeft <= 3 ? LogLevel.Error : LogLevel.Warning,
                    $"The certificate of this {NodeWording.Noun} runs out in {daysLeft} day(s), on {inUse.NotAfter:yyyy-MM-dd}. " +
                    "A new key and a new signing request take a minute; getting the answer back may take rather longer."
                );

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// The keys and certificates, as the web interface reads them.
        /// </summary>
        public JObject ToJSON()
        {

            var now = TimeProvider.GetUtcNow();

            lock (updateLock)
            {

                var inUse = Pick(now);

                return new JObject(

                           new JProperty("directory",   Path),
                           new JProperty("now",         now.ToString("o")),
                           new JProperty("inUseId",     inUse?.Id),

                           new JProperty("entries",     new JArray(
                               entries.Values.OrderByDescending(entry => entry.CreatedAt).
                                              Select(entry => {

                                                  var json = entry.ToJSON(now, entry.Id == inUse?.Id);

                                                  // A request still waiting to be collected is the
                                                  // one thing somebody opens this page for, and it
                                                  // is not a secret - it is meant to be handed to
                                                  // somebody else. So it travels with the entry
                                                  // rather than behind a second round trip.
                                                  if (entry.Certificate is null &&
                                                      TryReadCSR(entry.Id, out var csr, out _))
                                                  {
                                                      json.Add("csr", csr);
                                                  }

                                                  return json;

                                              })
                           )),

                           new JProperty("algorithms",  new JArray(
                               Algorithms.Select(algorithm => algorithm.ToJSON())
                           )),

                           new JProperty("defaultAlgorithm",  DefaultAlgorithm),
                           new JProperty("maxSubjectLength",  MaxSubjectLength),

                           // What a request may be made to and may ask for
                           // beside signing in as a client, which is not a
                           // choice and therefore not in the list.
                           new JProperty("templates",          new JArray(CSRTemplate.All.Select(template => template.ToJSON()))),
                           new JProperty("defaultTemplate",    CSRTemplate.DefaultId),
                           new JProperty("keyUsages",          new JArray(CSRKeyUsage.All.Select(usage   => usage.  ToJSON()))),
                           new JProperty("extendedKeyUsages",  new JArray(CSRPurpose. All.Select(purpose => purpose.ToJSON()))),

                           // No import, and the page should say so rather than
                           // leave somebody looking for the button: a key that
                           // arrived from somewhere else is a key somebody else
                           // has a copy of.
                           new JProperty("canImportPrivateKeys", false)

                       );

            }

        }

        #endregion


        #region (private) Pick(Now)

        /// <summary>
        /// The certificate this station would hold up at this moment: the
        /// newest one that is in its window and that this platform can load.
        /// </summary>
        /// <remarks>
        /// The newest rather than the longest-lived, because a new one is
        /// normally a renewal and a renewal is meant to take over.
        ///
        /// Called under the lock.
        /// </remarks>
        private ClientCertificateEntry? Pick(DateTimeOffset Now)

            => entries.Values.
                   Where(entry => entry.Certificate is not null &&
                                  entry.CanBeHeldUp             &&
                                  entry.NotBefore <= Now        &&
                                  entry.NotAfter  >  Now).
                   OrderByDescending(entry => entry.CreatedAt).
                   FirstOrDefault();

        #endregion

        #region (private) CheckEverything(Now) / CheckEntry(Entry, Now)

        /// <summary>
        /// Called under the lock.
        /// </summary>
        private void CheckEverything(DateTimeOffset Now)
        {
            foreach (var entry in entries.Values)
                CheckEntry(entry, Now);
        }

        /// <summary>
        /// What is worth saying about one entry, worked out again.
        /// </summary>
        /// <remarks>
        /// Rebuilt rather than remembered, because most of it depends on
        /// something other than the certificate - not least the day.
        /// </remarks>
        private void CheckEntry(ClientCertificateEntry  Entry,
                                DateTimeOffset          Now)
        {

            Entry.Warnings.Clear();

            if (Entry.Certificate is null)
            {
                Entry.Warnings.Add("No certificate yet: the signing request is still out.");
                return;
            }

            if (!Entry.CanBeHeldUp)
                Entry.Warnings.Add(
                    $"This certificate cannot be held up by this {NodeWording.Noun}: {Entry.CannotBeHeldUp}. " +
                    "The certificate itself may be perfectly good - a runtime that cannot take it today may take it after an update."
                );

            if (Entry.NotBefore > Now)
                Entry.Warnings.Add($"Not valid until {Entry.NotBefore:yyyy-MM-dd}.");

            if (Entry.NotAfter <= Now)
                Entry.Warnings.Add($"Expired on {Entry.NotAfter:yyyy-MM-dd}.");

            else
            {

                var daysLeft = (Int32) Math.Floor((Entry.NotAfter!.Value - Now).TotalDays);

                if (daysLeft <= WarnDaysBefore.Max())
                    Entry.Warnings.Add($"Runs out in {daysLeft} day(s), on {Entry.NotAfter:yyyy-MM-dd}.");

            }

            #region Does the chain hold up from here?

            using var chain = new X509Chain();

            chain.ChainPolicy.RevocationMode     = X509RevocationMode.NoCheck;
            chain.ChainPolicy.VerificationTime   = Now.UtcDateTime;
            chain.ChainPolicy.VerificationFlags  = X509VerificationFlags.NoFlag;

            foreach (var intermediate in Entry.Intermediates)
                chain.ChainPolicy.ExtraStore.Add(intermediate);

            if (!chain.Build(Entry.Certificate))
                Entry.Warnings.Add(
                    $"The chain of this certificate does not verify on this {NodeWording.Noun}: " +
                    String.Join(", ", chain.ChainStatus.Select(status => status.StatusInformation.Trim())) +
                    $". That may be quite correct - a private authority the back end knows is not one this {NodeWording.Noun} has to know."
                );

            #endregion

        }

        #endregion

        #region (private) TryLoadEntry(Id, out Entry, out PublicKey, out Error)

        /// <summary>
        /// One key, what is known about it, and its certificate when it has one.
        /// </summary>
        private Boolean TryLoadEntry(String                                          Id,
                                     [NotNullWhen(true)]  out ClientCertificateEntry? Entry,
                                     [NotNullWhen(true)]  out Byte[]?                 PublicKey,
                                     [NotNullWhen(false)] out String?                 Error)
        {

            Entry      = null;
            PublicKey  = null;
            Error      = null;

            #region What is known about the key

            var algorithm  = DefaultAlgorithm;
            var createdAt  = TimeProvider.GetUtcNow();
            var subject    = "";

            // What its request asked for: a key made before that was written
            // down asked for a client certificate, and for nothing else.
            var template           = CSRTemplate.DefaultId;
            String?  role          = null;
            String[] keyUsages     = [];
            String[] purposeNames  = [ CSRPurpose.ClientAuth ];

            var metaPath   = FilePath(Id, "json");

            if (File.Exists(metaPath))
            {
                try
                {

                    var meta = JObject.Parse(File.ReadAllText(metaPath));

                    algorithm  = meta.Value<String>("algorithm") ?? DefaultAlgorithm;
                    subject    = meta.Value<String>("subject")   ?? "";
                    template   = meta.Value<String>("template")  ?? CSRTemplate.DefaultId;
                    role       = meta.Value<String>("role");

                    if (meta["keyUsages"] is JArray usages)
                        keyUsages = [.. usages.Values<String>().OfType<String>()];

                    if (meta["extendedKeyUsages"] is JArray extended)
                        purposeNames = [.. extended.Values<String>().OfType<String>()];

                    if (DateTimeOffset.TryParse(meta.Value<String>("createdAt"),
                                                System.Globalization.CultureInfo.InvariantCulture,
                                                System.Globalization.DateTimeStyles.RoundtripKind,
                                                out var parsed))
                        createdAt = parsed;

                }
                catch (Exception e)
                {
                    Error = $"'{metaPath}' could not be read: {e.Message}";
                    return false;
                }
            }

            var kind = KeyAlgorithm.Find(algorithm);

            if (kind is null)
            {
                Error = $"'{algorithm}' is not a key algorithm this {NodeWording.LongNoun} knows.";
                return false;
            }

            #endregion

            #region The key itself

            String keyPEM;

            try
            {
                keyPEM = File.ReadAllText(FilePath(Id, "key.pem"));
            }
            catch (Exception e)
            {
                Error = $"the private key could not be read: {e.Message}";
                return false;
            }

            AsymmetricKeyParameter  privateKey;
            Byte[]                  publicKey;

            try
            {

                // Whatever kind it is - the encoding says so, so nothing here
                // has to be told which of a dozen algorithms to expect.
                privateKey  = PrivateKeyFactory.CreateKey(PemEncoding.Find(keyPEM) is PemFields fields
                                                              ? Convert.FromBase64String(keyPEM[fields.Base64Data])
                                                              : throw new FormatException("this is not a PEM private key"));

                publicKey   = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(
                                  PKIFactory.PublicKeyOf(privateKey)
                              ).GetDerEncoded();

            }
            catch (Exception e)
            {
                Error = $"the private key is not a usable {kind.Name} key: {e.Message}";
                return false;
            }

            PublicKey = publicKey;

            var entry = new ClientCertificateEntry(Id, kind.Name, createdAt, subject) {
                            Template           = template,
                            Role               = role,
                            KeyUsages          = keyUsages,
                            ExtendedKeyUsages  = purposeNames
                        };

            #endregion

            #region The certificate, when there is one

            var certificatePath = FilePath(Id, "cert.pem");

            if (File.Exists(certificatePath))
            {

                var collection = new X509Certificate2Collection();

                try
                {
                    collection.ImportFromPem(File.ReadAllText(certificatePath));
                }
                catch (Exception e)
                {
                    Error = $"'{certificatePath}' could not be read: {e.Message}";
                    return false;
                }

                if (collection.Count > 0)
                {

                    // The leaf is the one whose public key is this key's; it is
                    // written first, but a file somebody edited by hand may not
                    // have it first any more.
                    var leaf = collection.Cast<X509Certificate2>().
                                          FirstOrDefault(certificate => certificate.PublicKey.
                                                                            ExportSubjectPublicKeyInfo().
                                                                            AsSpan().SequenceEqual(publicKey))
                                   ?? collection[0];

                    foreach (var intermediate in collection.Cast<X509Certificate2>().
                                                            Where(certificate => !certificate.RawData.AsSpan().SequenceEqual(leaf.RawData)))
                        entry.Intermediates.Add(intermediate);

                    try
                    {
                        // Loading it is only half the question. An Ed448 or an
                        // ML-DSA certificate is perfectly valid and this
                        // platform may still have no key object to attach - so
                        // it is found out here, once, and the entry is kept
                        // either way.
                        entry.Certificate = PKIFactory.WithPrivateKey(leaf, privateKey);
                    }
                    catch (Exception e)
                    {
                        entry.Certificate     = leaf;
                        entry.CanBeHeldUp     = false;
                        entry.CannotBeHeldUp  = $"{e.GetType().Name}: {e.Message}";
                    }

                }

            }

            #endregion

            Entry = entry;
            return true;

        }

        #endregion

        #region (private static) OrderTowardsTheRoot(Leaf, Pool)

        /// <summary>
        /// The intermediates between a certificate and a root, in the order
        /// they belong in.
        /// </summary>
        /// <remarks>
        /// By issuer and subject, because that is what the order means, and a
        /// file that arrived in the wrong order is a file a person assembled.
        /// Anything left over at the end is appended rather than dropped: it
        /// was sent for a reason, and a back end that does not need it will
        /// ignore it.
        /// </remarks>
        private static List<X509Certificate2> OrderTowardsTheRoot(X509Certificate2        Leaf,
                                                                  List<X509Certificate2>  Pool)
        {

            var ordered = new List<X509Certificate2>();
            var wanted  = Leaf.Issuer;

            while (Pool.Count > 0)
            {

                var next = Pool.FirstOrDefault(certificate => certificate.Subject == wanted);

                if (next is null)
                    break;

                ordered.Add(next);
                Pool.Remove(next);

                // A self-signed certificate is the root, and a root is not sent
                // along: whoever is to trust it has it already, or does not.
                if (next.Subject == next.Issuer)
                {
                    ordered.Remove(next);
                    break;
                }

                wanted = next.Issuer;

            }

            ordered.AddRange(Pool.Where(certificate => certificate.Subject != certificate.Issuer));

            return ordered;

        }

        #endregion

        #region (private static) KeyId(SubjectPublicKeyInfo)

        /// <summary>
        /// What a key is called: where its public key hashes to.
        /// </summary>
        /// <remarks>
        /// Derived from the key rather than made up, so that a signing request
        /// and the certificate that answers it cannot end up filed under
        /// different names - and so that the same key uploaded twice is
        /// recognised as the same key.
        /// </remarks>
        private static String KeyId(Byte[] SubjectPublicKeyInfo)
            => Convert.ToHexStringLower(SHA256.HashData(SubjectPublicKeyInfo).AsSpan(0, 8));

        #endregion

        #region (private) FilePath / Known / CreateDirectory

        private String FilePath(String Id, String Extension)
            => System.IO.Path.Combine(Path, $"{Id}.{Extension}");

        private Boolean Known(String Id)
        {
            lock (updateLock)
                return entries.ContainsKey(Id);
        }

        private void CreateDirectory()
            => Directory.CreateDirectory(Path);

        #endregion

        #region Dispose()

        public void Dispose()
        {

            lock (updateLock)
            {

                foreach (var entry in entries.Values)
                    entry.Dispose();

                entries.Clear();
                publicKeys.Clear();

            }

        }

        #endregion

        #region (override) ToString()

        public override String ToString()

            => $"{entries.Count} key(s) in '{Path}'" +
               (InUse is not null ? $", holding up '{InUse.Id}'" : ", nothing to hold up");

        #endregion

    }

}
