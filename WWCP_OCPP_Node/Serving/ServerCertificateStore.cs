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
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

using BCx509 = Org.BouncyCastle.X509;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.PKI;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Logging;
using cloud.charging.open.protocols.WWCP.Node.Connections;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving
{

    /// <summary>
    /// The keys and certificates the charging station server authenticates
    /// with, in a directory of their own.
    /// </summary>
    /// <remarks>
    /// <b>Why more than one.</b> A certificate has to be replaced before it
    /// expires, and the replacement is frequently issued days before it becomes
    /// valid. So both live here at once, and which of them is presented is
    /// decided per connection rather than at a restart: the new one is uploaded
    /// the day it arrives, and takes over by itself the moment it is allowed to.
    ///
    /// <b>The private key is generated here and never leaves.</b> There is no
    /// import: a key that arrived over the network, through a browser and into
    /// a file has been in three places that a key should not have been in, and
    /// no amount of care afterwards takes it back out of them. What leaves is
    /// the signing request, which is a public document.
    ///
    /// <b>When nothing is valid, the last certificate keeps being served.</b>
    /// A charging station that checks certificates will refuse it and say so;
    /// one that does not will go on charging cars. Refusing to serve anything
    /// would take the site down in both cases, which is the worse of the two
    /// failures for a box in a car park - so this says so loudly in the log
    /// instead, and keeps the door open.
    ///
    /// <b>A server key needs no certificate authority.</b> A key can also be
    /// made as a server key, without a signing request: it is presented in a
    /// self-signed carrier certificate made from it, and a charging station
    /// that pinned its key pin - SHA-256 over its SubjectPublicKeyInfo - checks
    /// that and nothing else (EVQI Transport, key pins, 5.3). Where an
    /// authority issues a certificate for the same key later, that one is
    /// presented while it is valid, for the stations that check chains, and
    /// the carrier again when it runs out; the pin is the same in both. Made
    /// only when somebody asks, never by itself: a first certificate of any
    /// kind turns the station port to TLS at the next start.
    ///
    /// <b>The current key and the next one.</b> One key with a carrier is the
    /// current one and is presented. A next key can be announced ahead of a
    /// rollover: it is kept and its pin shown, so that the charging stations
    /// can be given it, and it is presented only once somebody switches to it
    /// (EVQI Transport, key pins, 5.5). The keys made for signing requests
    /// alone go on as before, until a server key is current.
    /// </remarks>
    public sealed class ServerCertificateStore : IDisposable
    {

        #region Data

        /// <summary>
        /// The directory beside the process, when nobody says otherwise.
        /// </summary>
        public const String  DefaultDirectoryName  = "ocpp-server-keys";

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

        /// <summary>
        /// What a carrier certificate is valid from: far in the past, so that a
        /// charging station whose clock believes it is 1970 does not refuse it
        /// for its dates (EVQI Transport, key pins, section 4, B0).
        /// </summary>
        public static readonly DateTimeOffset CarrierNotBefore  = new (1970, 1, 1,    0,  0,  0, TimeSpan.Zero);

        /// <summary>
        /// What a carrier certificate is valid until: 99991231235959Z, which
        /// RFC 5280 4.1.2.5 says for a certificate without a well-defined end.
        /// Nobody who pinned the key looks at it, and nothing about it may run
        /// out under a station that does.
        /// </summary>
        public static readonly DateTimeOffset CarrierNotAfter   = new (9999, 12, 31, 23, 59, 59, TimeSpan.Zero);

        /// <summary>
        /// What a carrier certificate is presented as, and how its wearer is
        /// presented in what this store says: a certificate an authority issued,
        /// or the carrier.
        /// </summary>
        public const String  PresentedAsCertificate  = "certificate";

        /// <summary>
        /// See <see cref="PresentedAsCertificate"/>.
        /// </summary>
        public const String  PresentedAsCarrier      = "carrier";

        private readonly Object                                   updateLock  = new ();
        private readonly Dictionary<String, ServerCertificateEntry>  entries  = [];

        /// <summary>
        /// The public key of every entry, which is what an uploaded certificate
        /// is matched against to find out whose it is.
        /// </summary>
        private readonly Dictionary<String, Byte[]>               publicKeys  = [];

        /// <summary>
        /// The chain that is being presented, the entry it came from, and which
        /// of the entry's certificates it is.
        /// </summary>
        private ServerCertificateChain?                           served;
        private String?                                           servedId;
        private String?                                           servedAs;

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
        /// usual: this is the one place in the controller where a wrong clock
        /// silently does the wrong thing. A box that boots believing it is 1970
        /// would find no certificate valid and a box a year fast would switch
        /// to one that nobody else accepts yet.
        /// </remarks>
        public TimeProvider  TimeProvider  { get; }

        /// <summary>
        /// Everything in the directory, newest key first.
        /// </summary>
        public IReadOnlyList<ServerCertificateEntry> Entries
        {
            get
            {
                lock (updateLock)
                    return [.. entries.Values.OrderByDescending(entry => entry.CreatedAt)];
            }
        }

        /// <summary>
        /// Whether there is any certificate at all - which is what decides
        /// whether this server can speak TLS.
        /// </summary>
        /// <remarks>
        /// A carrier certificate counts, the carrier of a next key does not:
        /// that one is not presented until somebody switches to it, and a key
        /// announced for later must not turn the port to TLS today.
        /// </remarks>
        public Boolean HasCertificate
        {
            get
            {
                lock (updateLock)
                    return entries.Values.Any(entry => !entry.IsNext &&
                                                        entry.CanBePresented &&
                                                       (entry.Certificate is not null || entry.Carrier is not null));
            }
        }

        /// <summary>
        /// The key that is presented to the charging stations now, or would be
        /// at the next handshake - whatever it is wrapped in - or null where
        /// there is none.
        /// </summary>
        /// <remarks>
        /// Its key pin is the one a charging station has to pin, which is why
        /// it is asked here without presenting anything: a page or a command
        /// that shows the pin must not be what switches the server over.
        /// </remarks>
        public ServerCertificateEntry? CurrentKey
            => GetCurrentKey(out _);

        /// <summary>
        /// The key that is presented to the charging stations now, or would be
        /// at the next handshake, and what it is presented in: a certificate an
        /// authority issued, <see cref="PresentedAsCertificate"/>, or its
        /// carrier, <see cref="PresentedAsCarrier"/>.
        /// </summary>
        /// <param name="PresentedAs">What the key is presented in, or null without a key.</param>
        public ServerCertificateEntry? GetCurrentKey(out String? PresentedAs)
        {
            lock (updateLock)
                return Choose(TimeProvider.GetUtcNow(), out PresentedAs);
        }

        /// <summary>
        /// The key presented now, or at the next handshake, and the announced
        /// next key, as one pair: read together, so that a switch in between
        /// cannot show the key before it beside no next key, or the new current
        /// key as the next one as well.
        /// </summary>
        /// <param name="Next">The announced next key, or null where none was made.</param>
        public ServerCertificateEntry? GetCurrentAndNextKey(out ServerCertificateEntry? Next)
        {
            lock (updateLock)
            {
                Next = entries.Values.FirstOrDefault(entry => entry.IsNext);
                return Choose(TimeProvider.GetUtcNow(), out _);
            }
        }

        /// <summary>
        /// The announced next key, or null where none was made.
        /// </summary>
        public ServerCertificateEntry? NextKey
        {
            get
            {
                lock (updateLock)
                    return entries.Values.FirstOrDefault(entry => entry.IsNext);
            }
        }

        /// <summary>
        /// The identification of the certificate currently being presented, or
        /// null while none has been chosen.
        /// </summary>
        public String? ServedId
        {
            get
            {
                lock (updateLock)
                    return servedId;
            }
        }

        /// <summary>
        /// Called with the full path of every file this store writes, just
        /// before it is written.
        /// </summary>
        /// <remarks>
        /// For the tests, and for nothing else: they make a write fail there,
        /// or fail partway through as on a full disk. Nothing on the outside of
        /// this store can: a key is named after its public key, so no test can
        /// put something in the way of its files beforehand, and no test can
        /// make a write stop halfway.
        /// </remarks>
        internal Action<String>? BeforeWriting { get; set; }

        /// <summary>
        /// Called with the full path of every file this store moves aside,
        /// moves back or deletes when it takes a key away, just before it does.
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
        /// Something happened here that belongs in the log of the controller:
        /// a certificate taken into use, a certificate that expired with no
        /// replacement, a file that could not be read.
        /// </summary>
        /// <remarks>
        /// An event rather than an event log handed in, so that this store can
        /// be built and tested without one - and so that what level a sentence
        /// is logged at stays the decision of the thing that owns the log.
        /// </remarks>
        public event Action<LogLevel, String>? OnNotice;

        /// <summary>
        /// The charging station server switched to its announced next key -
        /// whoever asked for it, the command line, the web page or the CSMS:
        /// the key pin presented before, where there was one, and the one
        /// presented from now on.
        /// </summary>
        /// <remarks>
        /// Raised once the switch is written, outside the lock: what it sets
        /// off may read this store again.
        /// </remarks>
        public event Action<KeyPin?, KeyPin>? OnSwitched;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The keys and certificates in the given directory; it need not exist
        /// yet.
        /// </summary>
        public ServerCertificateStore(String?        Path           = null,
                                      TimeProvider?  TimeProvider   = null)
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
        /// <remarks>
        /// A file that cannot be read is skipped with a line in the log rather
        /// than thrown: one unreadable key must not stop a controller that has
        /// three others, and the one certificate that is currently keeping a
        /// car park running is frequently one of the three.
        /// </remarks>
        public void Reload()
        {

            lock (updateLock)
            {

                foreach (var entry in entries.Values)
                    entry.Dispose();

                entries.Clear();
                publicKeys.Clear();

                served    = null;
                servedId  = null;
                servedAs  = null;

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

            }

        }

        #endregion

        #region TryCreateKey(Subject, ReachableAs, Algorithm, out Id, out CSR, out Error)

        /// <summary>
        /// Generate a key pair and the signing request that goes with it.
        /// </summary>
        /// <remarks>
        /// The request is written beside the key so that it can be downloaded
        /// again: it is frequently wanted a second time, by somebody who is not
        /// the person who pressed the button, and asking for a new key because
        /// the old request was lost would throw away a key for a filing
        /// mistake.
        /// </remarks>
        /// <param name="Subject">The common name to ask for, e.g. "lc001.example.org".</param>
        /// <param name="ReachableAs">The names and addresses the charging stations reach this controller under - a certificate without them in it is a certificate they will refuse.</param>
        /// <param name="Algorithm">One of <see cref="Algorithms"/>.</param>
        public Boolean TryCreateKey(String                            Subject,
                                    IEnumerable<String>               ReachableAs,
                                    String?                           Algorithm,
                                    [NotNullWhen(true)]  out String?  Id,
                                    [NotNullWhen(true)]  out String?  CSR,
                                    [NotNullWhen(false)] out String?  Error)

            => TryCreateKey(Subject, ReachableAs, Algorithm, out Id, out CSR, out Error, out _);

        /// <summary>
        /// Generate a key pair and the signing request that goes with it - and
        /// say whether a refusal was the files' rather than the request's.
        /// </summary>
        /// <param name="NotSaved">True where the key, its request or what is said of it could not be written: nothing about the request was wrong, and no key was made.</param>
        public Boolean TryCreateKey(String                            Subject,
                                    IEnumerable<String>               ReachableAs,
                                    String?                           Algorithm,
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
                Error = $"'{Algorithm}' is not a key this {NodeWording.Noun} generates ({String.Join(", ", KeyAlgorithm.All.Select(one => one.Id))}).";
                return false;
            }

            var subject = Subject?.Trim() ?? "";

            if (subject.Length == 0)
            {
                Error = $"A certificate needs a subject; the name the charging stations reach this {NodeWording.Noun} under is the one to use.";
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
                // may type, then handed on in the form Bouncy Castle signs
                // with - so the message about a bad subject is the readable one
                // and the request is still built by the half that knows every
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

            var names = ReachableAs.Select(name => name.Trim()).
                                    Where (name => name.Length > 0).
                                    Distinct(StringComparer.OrdinalIgnoreCase).
                                    ToArray();

            if (names.Length == 0)
            {
                Error = $"A certificate has to name what this {NodeWording.Noun} is reachable as, or no charging station will accept it. " +
                        "Fill in 'reachable as' before making a request.";
                return false;
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

                #region The signing request

                String csr;

                try
                {
                    // Built by Hermod, which knows every algorithm in the list
                    // above and what each one of them may honestly claim for
                    // itself - an Ed448 or an ML-DSA key asking to be allowed
                    // to encipher is a request a strict certificate authority
                    // is entitled to refuse.
                    csr = PKIFactory.GenerateCertificateSigningRequest(
                              pair,
                              subjectName,
                              algorithm,
                              names
                          ).ToPEM();
                }
                catch (Exception e)
                {
                    Error = $"The signing request could not be made: {e.Message}";
                    return false;
                }

                #endregion

                #region Written down, the key first and with its own permissions

                var createdAt = TimeProvider.GetUtcNow();

                try
                {

                    CreateDirectory();

                    WriteFile(
                        FilePath(id, "key.pem"),
                        PemEncoding.WriteString(
                            "PRIVATE KEY",
                            PrivateKeyInfoFactory.CreatePrivateKeyInfo(pair.Private).GetDerEncoded()
                        ) + Environment.NewLine,
                        OwnerOnly: true
                    );

                    WriteFile(FilePath(id, "csr.pem"), csr);

                    WriteFile(
                        FilePath(id, "json"),
                        new JObject(
                            new JProperty("id",         id),
                            new JProperty("algorithm",  algorithm.Id),
                            new JProperty("createdAt",  createdAt.ToString("o")),
                            new JProperty("subject",    subjectText),
                            new JProperty("reachableAs", new JArray(names))
                        ).ToString(Formatting.Indented) + Environment.NewLine
                    );

                }
                catch (Exception e)
                {

                    Error     = $"The key could not be written to '{Path}': {e.Message}";
                    NotSaved  = true;

                    // Not left behind half made: a key whose request or whose
                    // description was not written in full was read at the next
                    // start as a key nobody had asked for - made "now", of the
                    // default algorithm and for no subject - or could not be
                    // read, and said so at every start.
                    Forget(id, "key.pem", "csr.pem", "json");

                    return false;

                }

                #endregion

                var entry = new ServerCertificateEntry(id, algorithm.Name, createdAt, subjectText, KeyPin.FromSubjectPublicKeyInfo(publicKey));

                entries   [id] = entry;
                publicKeys[id] = publicKey;

                Id   = id;
                CSR  = csr;

            }

            OnNotice?.Invoke(LogLevel.Notice, $"A new {algorithm.Name} key '{id}' was generated and a signing request for {String.Join(", ", names)} is waiting to be collected.");

            return true;

        }

        #endregion

        #region TryCreateServerKey(Algorithm, AsNext, out Id, out KeyPin, out Error [, out NotSaved])

        /// <summary>
        /// Generate a server key that charging stations may pin, presented in a
        /// self-signed carrier certificate made from it - as the current key
        /// where nothing is presented yet, or as the announced next key.
        /// </summary>
        /// <param name="Algorithm">One of <see cref="Algorithms"/>; P-256 when nothing is said.</param>
        /// <param name="AsNext">Whether this is the next key, announced ahead of a rollover and presented only once somebody switches to it.</param>
        public Boolean TryCreateServerKey(String?                           Algorithm,
                                          Boolean                           AsNext,
                                          [NotNullWhen(true)]  out String?  Id,
                                          [NotNullWhen(true)]  out KeyPin?  KeyPin,
                                          [NotNullWhen(false)] out String?  Error)

            => TryCreateServerKey(Algorithm, AsNext, out Id, out KeyPin, out Error, out _);

        /// <summary>
        /// Generate a server key that charging stations may pin - and say
        /// whether a refusal was the files' rather than the request's.
        /// </summary>
        /// <remarks>
        /// <b>No signing request and no names.</b> A key pin vouches for the key
        /// alone, so nothing has to be known about what this controller is
        /// reachable as, and no certificate authority has to be asked. The key
        /// is presented in a carrier certificate that says what it is and what
        /// it is not: not an authority (basicConstraints CA:FALSE, no
        /// keyCertSign), for TLS servers only, valid from 1970 to 9999. A
        /// station that installed it as a root could otherwise be made to trust
        /// whatever its key signs (EVQI Transport, key pins, section 4, B0).
        ///
        /// <b>Never in place of what is presented.</b> Where something is
        /// presented already, the first key has to be announced as the next one
        /// and switched to: a key made here never takes over by itself, because
        /// every charging station that checks the old certificate or pinned the
        /// old key would be locked out at once. And where nothing is presented
        /// yet, there is nothing to announce a next key after.
        /// </remarks>
        /// <param name="NotSaved">True where the key, its carrier or what is said of it could not be written: nothing about the request was wrong, and no key was made.</param>
        public Boolean TryCreateServerKey(String?                           Algorithm,
                                          Boolean                           AsNext,
                                          [NotNullWhen(true)]  out String?  Id,
                                          [NotNullWhen(true)]  out KeyPin?  KeyPin,
                                          [NotNullWhen(false)] out String?  Error,
                                          out Boolean                       NotSaved)
        {

            Id        = null;
            KeyPin    = null;
            Error     = null;
            NotSaved  = false;

            var algorithm = KeyAlgorithm.Find(Algorithm ?? DefaultAlgorithm);

            if (algorithm is null)
            {
                Error = $"'{Algorithm}' is not a key this {NodeWording.Noun} generates ({String.Join(", ", KeyAlgorithm.All.Select(one => one.Id))}).";
                return false;
            }

            #region What may be made now

            lock (updateLock)
            {

                if (AsNext && entries.Values.FirstOrDefault(entry => entry.IsNext) is ServerCertificateEntry next)
                {
                    Error = $"'{next.Id}' is announced as the next key already. Switch to it, or remove it, before announcing another.";
                    return false;
                }

                if (AsNext && !HasCertificate)
                {
                    Error = "Nothing is presented to the charging stations yet, so there is nothing to announce a next key after. " +
                            "Make the server key itself first.";
                    return false;
                }

                if (!AsNext && HasCertificate)
                {
                    Error = $"'{Choose(TimeProvider.GetUtcNow(), out _)?.Id}' is presented to the charging stations already, and a new key does not " +
                             "take its place by itself: every station that checks it would be locked out at once. Announce the new " +
                             "key as the next one, hand its key pin to the stations, and switch to it.";
                    return false;
                }

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
            var pin        = org.GraphDefined.Vanaheimr.Hermod.KeyPin.FromSubjectPublicKeyInfo(publicKey);
            var createdAt  = TimeProvider.GetUtcNow();

            #region The carrier certificate

            String carrierPEM;
            String carrierSubject;

            try
            {
                (carrierPEM, carrierSubject) = CreateCarrier(pair, algorithm, id);
            }
            catch (Exception e)
            {
                Error = $"No carrier certificate could be made for the new key ({algorithm.Name}): {e.Message}";
                return false;
            }

            #endregion

            lock (updateLock)
            {

                if (entries.ContainsKey(id))
                {
                    Error = $"A key with the identification '{id}' is already here.";
                    return false;
                }

                #region Written down, the key first and with its own permissions

                try
                {

                    CreateDirectory();

                    WriteFile(
                        FilePath(id, "key.pem"),
                        PemEncoding.WriteString(
                            "PRIVATE KEY",
                            PrivateKeyInfoFactory.CreatePrivateKeyInfo(pair.Private).GetDerEncoded()
                        ) + Environment.NewLine,
                        OwnerOnly: true
                    );

                    WriteFile(FilePath(id, "carrier.pem"), carrierPEM);

                    WriteFile(
                        FilePath(id, "json"),
                        MetaOf(id, algorithm.Id, createdAt, carrierSubject, AsNext, AsNext ? null : createdAt)
                    );

                }
                catch (Exception e)
                {

                    Error     = $"The key could not be written to '{Path}': {e.Message}";
                    NotSaved  = true;

                    // Not left behind half made, as a key with a request is not.
                    Forget(id, "key.pem", "carrier.pem", "json");

                    return false;

                }

                #endregion

                // Read back the way it will be read at the next start: a
                // carrier that cannot be loaded from its own file is a server
                // that would present nothing after a restart.
                if (!TryLoadEntry(id, out var entry, out var loadedKey, out var problem))
                {
                    Forget(id, "key.pem", "carrier.pem", "json");
                    Error = $"The key was written but cannot be read back: {problem}";
                    return false;
                }

                if (!entry.CanBePresented)
                {

                    Forget(id, "key.pem", "carrier.pem", "json");
                    entry.Dispose();

                    // A key nobody can be shown is no server key: unlike a
                    // certificate that came from an authority, there is nothing
                    // here worth keeping for the day the platform learns it.
                    Error = entry.PresentationProblem ?? $"This machine cannot present {algorithm.Name} keys over TLS.";
                    return false;

                }

                entries   [id] = entry;
                publicKeys[id] = loadedKey;

                Id      = id;
                KeyPin  = pin;

            }

            OnNotice?.Invoke(
                LogLevel.Notice,
                AsNext
                    ? $"A new {algorithm.Name} key '{id}' was generated as the next server key, with the key pin {pin}. " +
                       "It is presented once somebody switches to it; until then, hand its key pin to the charging stations."
                    : $"A new {algorithm.Name} key '{id}' was generated as the server key, with the key pin {pin}, " +
                       "and is presented in a carrier certificate made from it."
            );

            return true;

        }

        #endregion

        #region TrySwitchToNextKey([IfNextKeyPinIs,] out Id, out Error [, out NotSaved])

        /// <summary>
        /// Present the announced next key from now on: it becomes the current
        /// one, and the key before it is kept, no longer presented, until
        /// somebody removes it.
        /// </summary>
        public Boolean TrySwitchToNextKey([NotNullWhen(true)]  out String?  Id,
                                          [NotNullWhen(false)] out String?  Error)

            => TrySwitchToNextKey(null, out Id, out Error, out _);

        /// <summary>
        /// Present the announced next key from now on - and say whether a
        /// refusal was the file's rather than the request's.
        /// </summary>
        /// <param name="NotSaved">True where the next key's file could not be written: nothing was switched.</param>
        public Boolean TrySwitchToNextKey([NotNullWhen(true)]  out String?  Id,
                                          [NotNullWhen(false)] out String?  Error,
                                          out Boolean                       NotSaved)

            => TrySwitchToNextKey(null, out Id, out Error, out NotSaved);

        /// <summary>
        /// Present the announced next key from now on, where it is still the
        /// key with the given key pin - and say whether a refusal was the
        /// file's rather than the request's.
        /// </summary>
        /// <remarks>
        /// Asked for, never done by itself: whether every charging station has
        /// the next key pin by now is known to whoever handed it out - the CSMS,
        /// or the technician - and not to this store. A station that has not is
        /// refused at its next handshake.
        ///
        /// <b>Compare and switch.</b> Whoever names the pin - the CSMS, which
        /// handed exactly that pin to its stations - gets that key or nothing:
        /// the next key is compared under the same lock that switches, so a
        /// switch and a new next key made meanwhile on the command line cannot
        /// make it a switch to a key nobody handed out.
        ///
        /// One file is written, the next key's: it stops being next and becomes
        /// the current one from now on. The key it takes over from is current
        /// no longer because the newer of the two is, so there is no moment at
        /// which a half-written switch leaves two current keys or none.
        /// </remarks>
        /// <param name="IfNextKeyPinIs">The key pin the announced next key has to have; whichever it has where null.</param>
        /// <param name="NotSaved">True where the next key's file could not be written: nothing was switched.</param>
        public Boolean TrySwitchToNextKey(KeyPin?                           IfNextKeyPinIs,
                                          [NotNullWhen(true)]  out String?  Id,
                                          [NotNullWhen(false)] out String?  Error,
                                          out Boolean                       NotSaved)
        {

            Id        = null;
            Error     = null;
            NotSaved  = false;

            String? before;
            KeyPin? beforePin;
            KeyPin  afterPin;

            lock (updateLock)
            {

                var next = entries.Values.FirstOrDefault(entry => entry.IsNext);

                if (next is null)
                {
                    Error = "No next key has been announced, so there is nothing to switch to.";
                    return false;
                }

                if (IfNextKeyPinIs is not null && next.KeyPin != IfNextKeyPinIs)
                {
                    Error = $"The announced next key is '{next.Id}' with the key pin {next.KeyPin}, not the key with the key pin {IfNextKeyPinIs}.";
                    return false;
                }

                if (next.Carrier is null || !next.CanBePresented)
                {
                    Error = $"The next key '{next.Id}' cannot be presented by this machine: {next.PresentationProblem ?? "it has no carrier certificate"}";
                    return false;
                }

                var presented  = Choose(TimeProvider.GetUtcNow(), out _);

                before         = presented?.Id;
                beforePin      = presented?.KeyPin;
                afterPin       = next.KeyPin;

                var now = TimeProvider.GetUtcNow();

                // Never earlier than the current key's own moment: a clock that
                // was set back would otherwise leave the old key the newer one.
                var current = CurrentServerKey();

                if (current?.CurrentSince is DateTimeOffset since && since >= now)
                    now = since.AddTicks(1);

                var switchedAt = now;

                try
                {
                    RewriteMeta(next.Id, meta => {
                        meta["next"]          = false;
                        meta["currentSince"]  = switchedAt.ToString("o");
                    });
                }
                catch (Exception e)
                {
                    Error     = $"The switch to '{next.Id}' could not be written to '{Path}': {e.Message}";
                    NotSaved  = true;
                    return false;
                }

                next.IsNext        = false;
                next.CurrentSince  = now;

                Id = next.Id;

            }

            OnNotice?.Invoke(
                LogLevel.Notice,
                $"The charging station server switched to the server key '{Id}'" +
                (before is not null ? $", which takes over from '{before}'" : "") +
                ". Charging stations that pinned only the key before are refused from their next handshake on."
            );

            OnSwitched?.Invoke(beforePin, afterPin);

            return true;

        }

        #endregion

        #region TryReadCSR(Id, out CSR, out Error)

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
                Error = File.Exists(FilePath(Id, "carrier.pem"))
                            ? $"'{Id}' is a server key that charging stations pin; it was made without a signing request."
                            : $"The signing request of '{Id}' is no longer in '{Path}'.";
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

        #region TryAddCertificate(PEM, ReachableAs, out Id, out Warnings, out Error)

        /// <summary>
        /// Take a certificate that came back from a certificate authority,
        /// together with whatever intermediates were sent with it.
        /// </summary>
        /// <remarks>
        /// <b>Everything that can be checked is checked here and not at the
        /// moment of the switch.</b> A certificate uploaded today to take over
        /// in two days switches over at whatever hour it becomes valid, with
        /// nobody watching; a mistake found then is found by the charging
        /// stations. So a certificate whose key is not here, or that is already
        /// expired, is refused now, while somebody is looking at the screen.
        ///
        /// What is reported but not refused is everything that may legitimately
        /// look wrong from in here: a chain this box cannot verify may be from
        /// a private authority every charging station on the site knows, and
        /// names that do not match may be a 'reachable as' list that is out of
        /// date rather than a wrong certificate.
        /// </remarks>
        /// <param name="PEM">The certificate, and any intermediates, as PEM.</param>
        /// <param name="ReachableAs">What the certificate ought to be valid for.</param>
        public Boolean TryAddCertificate(String                              PEM,
                                         IEnumerable<String>                 ReachableAs,
                                         [NotNullWhen(true)]  out String?    Id,
                                         out IReadOnlyList<String>           Warnings,
                                         [NotNullWhen(false)] out String?    Error)

            => TryAddCertificate(PEM, ReachableAs, out Id, out Warnings, out Error, out _);

        /// <summary>
        /// Take in the certificate that answers one of this controller's
        /// signing requests - and say whether a refusal was the file's rather
        /// than the certificate's.
        /// </summary>
        /// <param name="NotSaved">True where the certificate could not be written: nothing about it was wrong, it was not taken in, and one it was to replace is still the key's, now and at the next start.</param>
        public Boolean TryAddCertificate(String                              PEM,
                                         IEnumerable<String>                 ReachableAs,
                                         [NotNullWhen(true)]  out String?    Id,
                                         out IReadOnlyList<String>           Warnings,
                                         [NotNullWhen(false)] out String?    Error,
                                         out Boolean                         NotSaved)
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

                    var spki = certificate.PublicKey.ExportSubjectPublicKeyInfo();

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
                    Error = $"None of the certificates in this file belongs to a key of this {NodeWording.Noun}. " +
                            "A certificate is only usable here if it answers a signing request made here.";
                    return false;
                }

                #endregion

                var now = TimeProvider.GetUtcNow();

                #region Already expired

                if (new DateTimeOffset(leaf.NotAfter.ToUniversalTime()) <= now)
                {
                    Error = $"This certificate expired on {leaf.NotAfter.ToUniversalTime():yyyy-MM-dd} and can never be used. " +
                            $"By the clock of this {NodeWording.Noun} it is now {now:yyyy-MM-dd}.";
                    return false;
                }

                notAfter = new DateTimeOffset(leaf.NotAfter.ToUniversalTime());

                #endregion

                #region The intermediates, in the order they are sent in

                var pool           = uploaded.Cast<X509Certificate2>().
                                              Where(certificate => !certificate.RawData.AsSpan().SequenceEqual(leaf.RawData)).
                                              ToList();

                var intermediates  = OrderTowardsTheRoot(leaf, pool);

                #endregion

                #region Written down

                try
                {

                    CreateDirectory();

                    var pem = String.Join(
                                  Environment.NewLine,
                                  new[] { leaf }.Concat(intermediates).
                                      Select(certificate => PemEncoding.WriteString("CERTIFICATE", certificate.RawData))
                              ) + Environment.NewLine;

                    // Beside the certificate it replaces, and moved over it in
                    // one step: written in its place, a renewal the disk ran
                    // out in cut the one before it off, and at the next start
                    // the key was not read at all.
                    WriteFile(FilePath(leafId, "cert.pem.tmp"), pem);

                    File.Move(FilePath(leafId, "cert.pem.tmp"), FilePath(leafId, "cert.pem"), overwrite: true);

                }
                catch (Exception e)
                {

                    Error     = $"The certificate could not be written to '{Path}': {e.Message}";
                    NotSaved  = true;

                    // What was written of it, beside the one it was to replace.
                    Forget(leafId, "cert.pem.tmp");

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

                CheckEntry(entry, ReachableAs, now);

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

        #region KeyOf(Certificate)

        /// <summary>
        /// The key of this controller the given certificate belongs to - by its
        /// public key, which is the only thing that says so - or nothing.
        /// </summary>
        /// <remarks>
        /// What the upload asks before anything is taken in, so that a page can
        /// say of each certificate in a text which key it is for, and of one
        /// that is for none that it is not this controller's.
        /// </remarks>
        /// <param name="Certificate">A certificate.</param>
        public String? KeyOf(X509Certificate2 Certificate)
        {

            var spki = Certificate.PublicKey.ExportSubjectPublicKeyInfo();

            lock (updateLock)
            {
                return publicKeys.FirstOrDefault(pair => pair.Value.AsSpan().SequenceEqual(spki)).Key;
            }

        }

        #endregion

        #region TryRemove(Id, out Error)

        /// <summary>
        /// Throw a key, its request and its certificate away.
        /// </summary>
        /// <remarks>
        /// The one that is currently being presented is refused: a charging
        /// station server without a certificate stops speaking TLS, and doing
        /// that by deleting a file is not a decision anybody means to make.
        /// Upload the replacement first.
        /// </remarks>
        public Boolean TryRemove(String                            Id,
                                 [NotNullWhen(false)] out String?  Error)

            => TryRemove(Id, out Error, out _);

        /// <summary>
        /// Throw a key, its request and its certificate away - and say whether
        /// a refusal was the files' rather than the request's.
        /// </summary>
        /// <param name="NotSaved">True where a file could not be taken away: the key stays here, whole.</param>
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
                    Error = $"There is no key '{Id}' here.";
                    return false;
                }

                if (servedId == Id)
                {
                    Error = $"'{Id}' is the certificate this server is presenting right now. " +
                             "Upload the one that is to replace it first, then remove this one.";
                    return false;
                }

                // Asked whether or not anything was presented yet: the current
                // server key is what the next handshake presents, and taking it
                // away would hand the stations that pinned it another key.
                if (CurrentServerKey()?.Id == Id)
                {
                    Error = $"'{Id}' is the current server key, which the charging stations pinned. " +
                             "Announce a next key, switch to it, then remove this one.";
                    return false;
                }

                if (!TrySetAside(Id, [ "key.pem", "csr.pem", "cert.pem", "carrier.pem", "json" ], leftOver, out var setAside, out var problem))
                {
                    Error     = $"'{Id}' could not be removed from '{Path}': {problem}";
                    NotSaved  = true;
                }

                else
                {

                    entry.Dispose();
                    entries.Remove(Id);
                    publicKeys.Remove(Id);

                    Delete(setAside, leftOver);

                }

            }

            foreach (var file in leftOver)
                OnNotice?.Invoke(LogLevel.Warning, $"'{file}' could not be {(Error is null ? "deleted" : "put back")}, and is left over.");

            if (Error is not null)
                return false;

            OnNotice?.Invoke(LogLevel.Notice, $"The key '{Id}' and everything belonging to it was removed from '{Path}'.");

            return true;

        }

        #endregion

        #region Select()

        /// <summary>
        /// The certificate chain to present, decided afresh - which is what
        /// makes a certificate uploaded ahead of time take over by itself.
        /// </summary>
        /// <remarks>
        /// <b>The rule.</b> Of the certificates that are in their validity
        /// window right now, the one that became valid last; where two became
        /// valid at the same moment, the one that lasts longer; and where those
        /// are equal too, the one whose thumbprint sorts first - so that two
        /// starts of the same controller never disagree.
        ///
        /// When nothing is in its window, whatever was last presented goes on
        /// being presented, and on a fresh start that is the one that expired
        /// most recently. See the remarks on this class for why that is better
        /// than presenting nothing.
        ///
        /// <b>Once a server key is current</b>, the rule is that key's: the
        /// certificate an authority issued for it while that is valid, and its
        /// carrier otherwise. Another key's certificate is not presented beside
        /// it, however valid - a station that pinned the current key would be
        /// shown a key it never heard of.
        /// </remarks>
        public ServerCertificateChain? Select()
        {

            var now = TimeProvider.GetUtcNow();

            lock (updateLock)
            {

                var chosen = Choose(now, out var presentedAs);

                if (chosen is null)
                    return null;

                // Nothing valid, and something is going out already: it keeps
                // going out, see above.
                if (presentedAs == PresentedAsCertificate &&
                    !chosen.IsValidAt(now) &&
                    served is not null &&
                    servedId == chosen.Id &&
                    servedAs == presentedAs)
                {
                    return served;
                }

                if (servedId != chosen.Id || servedAs != presentedAs || served is null)
                {

                    if (presentedAs == PresentedAsCarrier)
                    {

                        served    = new ServerCertificateChain(chosen.Carrier!, []);
                        servedId  = chosen.Id;
                        servedAs  = presentedAs;

                        OnNotice?.Invoke(
                            LogLevel.Notice,
                            $"The charging station server is now presenting its server key '{chosen.Id}' in its carrier certificate, " +
                            $"with the key pin {chosen.KeyPin}. Charging stations that pinned it check the key alone; " +
                             "those that check chains need a certificate an authority issued for this key."
                        );

                    }

                    else
                    {

                        if (!chosen.IsValidAt(now))
                            OnNotice?.Invoke(
                                LogLevel.Critical,
                                $"No server certificate is valid right now - by this {NodeWording.Noun}'s clock it is {now:yyyy-MM-dd HH:mm}'Z'. " +
                                $"'{chosen.Certificate!.Subject}' (expired {chosen.NotAfter:yyyy-MM-dd}) is being presented anyway so that the " +
                                 "charging stations are not locked out. Upload a current certificate."
                            );

                        served    = new ServerCertificateChain(chosen.Certificate!, chosen.Intermediates.Cast<X509Certificate2>());
                        servedId  = chosen.Id;
                        servedAs  = presentedAs;

                        OnNotice?.Invoke(
                            LogLevel.Notice,
                            $"The charging station server is now presenting '{chosen.Certificate!.Subject}' " +
                            $"({chosen.Id}, valid until {chosen.NotAfter:yyyy-MM-dd}, {chosen.Intermediates.Count} intermediate(s))."
                        );

                    }

                }

                return served;

            }

        }

        #endregion

        #region (private) Choose(Now, out PresentedAs) / CurrentServerKey()

        /// <summary>
        /// The entry whose certificate <see cref="Select"/> presents at the
        /// given moment, and as which of its certificates - without presenting
        /// it, and without saying anything. Called with the lock held.
        /// </summary>
        private ServerCertificateEntry? Choose(DateTimeOffset  Now,
                                               out String?     PresentedAs)
        {

            PresentedAs = null;

            #region A server key is current: its certificate while valid, its carrier otherwise

            if (CurrentServerKey() is ServerCertificateEntry current)
            {

                PresentedAs = current.IsValidAt(Now)
                                  ? PresentedAsCertificate
                                  : PresentedAsCarrier;

                return current;

            }

            #endregion

            #region Keys with certificates alone, as before there were server keys

            // Not merely "has a certificate": one this machine cannot present
            // would turn every handshake into a failure, which is worse than
            // the expired certificate below. And never a next key's, which is
            // not presented before somebody switches to it.
            var usable = entries.Values.Where(entry => !entry.IsNext &&
                                                        entry.Certificate is not null &&
                                                        entry.CanBePresented).ToArray();

            if (usable.Length == 0)
                return null;

            PresentedAs = PresentedAsCertificate;

            var chosen = usable.Where(entry => entry.IsValidAt(Now)).
                                OrderByDescending(entry => entry.NotBefore!.Value).
                                ThenByDescending (entry => entry.NotAfter!. Value).
                                ThenBy           (entry => entry.Certificate!.Thumbprint, StringComparer.Ordinal).
                                FirstOrDefault();

            // Nothing is valid. Whatever is already going out keeps going out;
            // a restart in this state takes the one that lasted longest.
            return chosen
                       ?? (servedId is not null && servedAs == PresentedAsCertificate && usable.FirstOrDefault(entry => entry.Id == servedId) is ServerCertificateEntry going
                               ? going
                               : usable.OrderByDescending(entry => entry.NotAfter!.Value).First());

            #endregion

        }

        /// <summary>
        /// The current server key: of the keys with a carrier that are not the
        /// next one, the one that became current last - or null where there is
        /// none. Called with the lock held.
        /// </summary>
        private ServerCertificateEntry? CurrentServerKey()

            => entries.Values.Where(entry => !entry.IsNext &&
                                              entry.Carrier is not null &&
                                              entry.CanBePresented &&
                                              entry.CurrentSince.HasValue).
                              OrderByDescending(entry => entry.CurrentSince!.Value).
                              ThenBy           (entry => entry.Id, StringComparer.Ordinal).
                              FirstOrDefault();

        #endregion

        #region CheckExpiry(ReachableAs)

        /// <summary>
        /// Say what is about to run out, and how the certificates look against
        /// the names this controller is currently reachable under.
        /// </summary>
        /// <remarks>
        /// This is the half of planning a replacement that the store can do:
        /// the other half is somebody reading it. So the sentences are written
        /// to be read in a log by a person who was not thinking about
        /// certificates a moment ago.
        /// </remarks>
        public void CheckExpiry(IEnumerable<String> ReachableAs)
        {

            var now   = TimeProvider.GetUtcNow();
            var names = ReachableAs.ToArray();

            lock (updateLock)
            {

                foreach (var entry in entries.Values)
                    CheckEntry(entry, names, now);

                var valid = entries.Values.Where(entry => entry.IsValidAt(now)).ToArray();

                // Nothing valid is the loudest case there is, and it used to be
                // the quietest: an early return here meant the one state worth
                // shouting about was the one that said nothing.
                if (valid.Length == 0)
                {

                    if (entries.Values.Any(entry => entry.Certificate is not null))
                        OnNotice?.Invoke(
                            LogLevel.Critical,
                            $"No server certificate of the charging station server is valid - by this {NodeWording.Noun}'s clock it is " +
                            $"{now:yyyy-MM-dd}. The charging stations that check certificates are being turned away. " +
                             "Upload a current certificate."
                        );

                    return;

                }

                var last      = valid.Max(entry => entry.NotAfter!.Value);
                var remaining = last - now;

                // Only against the certificate that lasts longest, and only
                // when nothing takes over from it: a certificate expiring next
                // week with its replacement already uploaded is exactly what a
                // planned replacement looks like, and warning about it is how a
                // warning gets ignored.
                var replacement = entries.Values.Any(entry => entry.Certificate is not null &&
                                                              entry.NotAfter > last);

                if (replacement)
                    return;

                var days = (Int32) Math.Floor(remaining.TotalDays);

                if (days <= WarnDaysBefore.Max())
                    OnNotice?.Invoke(
                        days <= 3 ? LogLevel.Critical : days <= 14 ? LogLevel.Error : LogLevel.Warning,
                        $"The server certificate of the charging station server runs out in {days} day(s), on {last:yyyy-MM-dd}, " +
                         "and nothing is here to take over from it. A replacement can be uploaded now and will be used by itself " +
                         "from the day it becomes valid."
                    );

            }

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

                // What the next handshake presents, rather than what the last
                // one did: before the first station connects nothing was
                // presented yet, and the key pin to hand out is known all the
                // same.
                var current  = Choose(now, out var presentedAs);
                var next     = entries.Values.FirstOrDefault(entry => entry.IsNext);

                return new JObject(

                    new JProperty("directory",   Path),
                    new JProperty("now",         now.ToString("o")),
                    new JProperty("servedId",    servedId),

                    // The key pins a charging station is to pin, current key
                    // first (EVQI Transport, key pins, 7.4).
                    new JProperty("keyPins",     new JObject(
                        new JProperty("current", current is null ? null : new JObject(
                            new JProperty("id",         current.Id),
                            new JProperty("keyPin",     current.KeyPin.ToHex()),
                            new JProperty("createdAt",  current.CreatedAt.ToString("o")),
                            new JProperty("presented",  presentedAs)
                        )),
                        new JProperty("next",    next is null ? null : new JObject(
                            new JProperty("id",         next.Id),
                            new JProperty("keyPin",     next.KeyPin.ToHex()),
                            new JProperty("createdAt",  next.CreatedAt.ToString("o"))
                        ))
                    )),

                    new JProperty("entries",     new JArray(
                        entries.Values.OrderByDescending(entry => entry.CreatedAt).
                                       Select(entry => entry.ToJSON(now,
                                                                    entry.Id == servedId,
                                                                    entry.Id == current?.Id ? presentedAs : null))
                    )),

                    new JProperty("algorithms",  new JArray(
                        Algorithms.Select(algorithm => algorithm.ToJSON())
                    )),

                    // No import, and the page should say so rather than leave
                    // somebody looking for the button.
                    new JProperty("canImportPrivateKeys", false)

                );

            }

        }

        #endregion


        #region (private) TryLoadEntry(Id, out Entry, out PublicKey, out Error)

        /// <summary>
        /// One key, what is known about it, and its certificate when it has one.
        /// </summary>
        private Boolean TryLoadEntry(String                                       Id,
                                     [NotNullWhen(true)] out ServerCertificateEntry?  Entry,
                                     [NotNullWhen(true)] out Byte[]?                  PublicKey,
                                     [NotNullWhen(false)] out String?                 Error)
        {

            Entry      = null;
            PublicKey  = null;
            Error      = null;

            #region What is known about the key

            var algorithm     = DefaultAlgorithm;
            var createdAt     = TimeProvider.GetUtcNow();
            var subject       = "";
            var isNext        = false;
            var currentSince  = default(DateTimeOffset?);

            var metaPath      = FilePath(Id, "json");

            if (File.Exists(metaPath))
            {
                try
                {

                    var meta = JObject.Parse(File.ReadAllText(metaPath));

                    algorithm  = meta.Value<String>("algorithm") ?? DefaultAlgorithm;
                    subject    = meta.Value<String>("subject")   ?? "";
                    isNext     = meta.Value<Boolean?>("next")    ?? false;

                    if (DateTimeOffset.TryParse(meta.Value<String>("createdAt"),
                                                System.Globalization.CultureInfo.InvariantCulture,
                                                System.Globalization.DateTimeStyles.RoundtripKind,
                                                out var parsed))
                        createdAt = parsed;

                    if (DateTimeOffset.TryParse(meta.Value<String>("currentSince"),
                                                System.Globalization.CultureInfo.InvariantCulture,
                                                System.Globalization.DateTimeStyles.RoundtripKind,
                                                out var since))
                        currentSince = since;

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
                Error = $"'{algorithm}' is not a key algorithm this {NodeWording.Noun} knows.";
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
                                  PublicKeyOf(privateKey)
                              ).GetDerEncoded();

            }
            catch (Exception e)
            {
                Error = $"the private key is not a usable {kind.Name} key: {e.Message}";
                return false;
            }

            PublicKey = publicKey;

            var entry = new ServerCertificateEntry(Id, kind.Name, createdAt, subject, KeyPin.FromSubjectPublicKeyInfo(publicKey)) {
                            IsNext        = isNext,
                            CurrentSince  = currentSince
                        };

            #endregion

            #region The carrier certificate, when it is a server key

            var carrierPath = FilePath(Id, "carrier.pem");

            if (File.Exists(carrierPath))
            {

                X509Certificate2 carrier;

                try
                {
                    carrier = X509Certificate2.CreateFromPem(File.ReadAllText(carrierPath));
                }
                catch (Exception e)
                {
                    Error = $"'{carrierPath}' is not a certificate: {e.Message}";
                    return false;
                }

                using (carrier)
                {

                    if (!carrier.PublicKey.ExportSubjectPublicKeyInfo().AsSpan().SequenceEqual(publicKey))
                    {
                        Error = $"'{carrierPath}' is the carrier of another key.";
                        return false;
                    }

                    try
                    {
                        entry.Carrier = WithPrivateKey(carrier, privateKey);
                    }
                    catch (Exception e)
                    {
                        entry.CanBePresented      = false;
                        entry.PresentationProblem = $"This machine cannot make a usable TLS certificate out of its {kind.Name} key: {e.Message}";
                    }

                }

                if (entry.Carrier is not null &&
                    !KeyAlgorithm.CanBePresented(kind.Id, entry.Carrier))
                {
                    entry.CanBePresented      = false;
                    entry.PresentationProblem = $"{kind.Name} keys cannot be presented over TLS by this machine - the handshake fails.";
                }

            }

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
                    Error = $"'{certificatePath}' is not a certificate: {e.Message}";
                    return false;
                }

                if (collection.Count == 0)
                {
                    Error = $"'{certificatePath}' holds no certificate.";
                    return false;
                }

                // The leaf is the one the key belongs to, wherever in the file
                // it happens to sit.
                var leaf = collection.Cast<X509Certificate2>().
                                      FirstOrDefault(certificate => certificate.PublicKey.ExportSubjectPublicKeyInfo().AsSpan().SequenceEqual(publicKey));

                if (leaf is null)
                {
                    Error = $"'{certificatePath}' holds no certificate belonging to this key.";
                    return false;
                }

                try
                {
                    entry.Certificate = WithPrivateKey(leaf, privateKey);
                }
                catch (Exception e)
                {

                    // Not a reason to refuse the certificate: it is a perfectly
                    // good one, and the platform underneath is what cannot hold
                    // it. Kept, said out loud, and passed over when the server
                    // chooses what to present - because the day this runs
                    // somewhere else, or on a newer runtime, it may work.
                    entry.CanBePresented      = false;
                    entry.PresentationProblem = $"This machine cannot make a usable TLS certificate out of its {kind.Name} key: {e.Message} " +
                                                 "The certificate is kept, but it cannot be presented to a charging station from here.";

                }

                // Loading it is only half the question. A P-521 or an ML-DSA
                // certificate loads perfectly well and then finds no TLS stack
                // willing to negotiate with it, which is something only a
                // handshake finds out - so one is done, once per kind of key.
                if (entry.Certificate is not null &&
                    !KeyAlgorithm.CanBePresented(kind.Id, entry.Certificate))
                {

                    entry.CanBePresented      = false;
                    // "It is kept" followed "... certificates cannot be
                    // presented", with nothing for "it" to be (found by the CSMS).
                    entry.PresentationProblem = $"{kind.Name} certificates cannot be presented over TLS by this machine - the handshake fails. " +
                                                 "This one is kept, and will be used the day the platform underneath can serve it.";

                }

                foreach (var intermediate in OrderTowardsTheRoot(
                                                 leaf,
                                                 collection.Cast<X509Certificate2>().
                                                            Where(certificate => !certificate.RawData.AsSpan().SequenceEqual(leaf.RawData)).
                                                            ToList()))
                {
                    entry.Intermediates.Add(intermediate);
                }

            }

            #endregion

            Entry = entry;
            return true;

        }

        #endregion

        #region (private) CheckEntry(Entry, ReachableAs, Now)

        /// <summary>
        /// What is worth saying about one certificate, recomputed from scratch.
        /// </summary>
        private static void CheckEntry(ServerCertificateEntry  Entry,
                                       IEnumerable<String>     ReachableAs,
                                       DateTimeOffset          Now)
        {

            Entry.Warnings.Clear();

            // First, and before the check below that there is a certificate at
            // all: where the platform cannot hold this kind of key, there is no
            // .NET certificate to look at - so a check that began by returning
            // on a missing one would swallow the only sentence that explains
            // why it is missing.
            if (Entry.PresentationProblem is not null)
                Entry.Warnings.Add(Entry.PresentationProblem);

            if (Entry.Certificate is null)
                return;

            #region Does it cover what the charging stations dial?

            var covered  = Entry.SubjectAlternativeNames().ToArray();

            var missing  = ReachableAs.Select(name => name.Trim()).
                                       Where (name => name.Length > 0).
                                       Where (name => !covered.Any(alternative => Covers(alternative, name))).
                                       ToArray();

            if (missing.Length > 0)
                Entry.Warnings.Add(
                    covered.Length == 0
                        ? "This certificate names nothing it is valid for, so charging stations that check it will refuse it."
                        : $"This certificate is not valid for {String.Join(", ", missing)} - it covers {String.Join(", ", covered)}."
                );

            #endregion

            #region Does the chain hold up from here?

            var selfSigned = Entry.Certificate.SubjectName.RawData.AsSpan().SequenceEqual(Entry.Certificate.IssuerName.RawData);

            if (!selfSigned && Entry.Intermediates.Count == 0)
                Entry.Warnings.Add(
                    "No intermediate certificates were uploaded with this one. Charging stations that do not already " +
                    "know the issuer will not be able to build a chain to it."
                );

            if (selfSigned)
                Entry.Warnings.Add("This certificate signed itself, so only a charging station that was told to trust it specifically will accept it.");

            #endregion

            #region Is it in its window?

            if (Entry.NotBefore > Now)
                Entry.Warnings.Add($"Not valid until {Entry.NotBefore:yyyy-MM-dd HH:mm}'Z'; it will be taken into use by itself then.");

            else if (Entry.NotAfter < Now)
                Entry.Warnings.Add($"Expired on {Entry.NotAfter:yyyy-MM-dd}.");

            #endregion

        }

        #endregion

        #region (private static) Covers(Name, Wanted)

        /// <summary>
        /// Whether a name in a certificate covers a name a charging station
        /// dials, wildcards included.
        /// </summary>
        /// <remarks>
        /// A wildcard stands for exactly one label, which is what TLS says and
        /// not what most people expect: "*.example.org" is "a.example.org" and
        /// is not "a.b.example.org", and is not "example.org" either.
        /// </remarks>
        private static Boolean Covers(String Name, String Wanted)
        {

            if (String.Equals(Name, Wanted, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!Name.StartsWith("*.", StringComparison.Ordinal))
                return false;

            var suffix = Name[1..];

            if (!Wanted.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                return false;

            // Exactly one label in front of the suffix, and a real one.
            var head = Wanted[..^suffix.Length];

            return head.Length > 0 && !head.Contains('.');

        }

        #endregion

        #region (private static) OrderTowardsTheRoot(Leaf, Pool)

        /// <summary>
        /// The intermediates between a certificate and a root, in the order TLS
        /// wants them: each one signing the one before it.
        /// </summary>
        /// <remarks>
        /// The order matters to stricter clients, and what arrives in an upload
        /// is whatever order the certificate authority happened to write. A
        /// root that came along is dropped: it is either already trusted, in
        /// which case it is bytes on the wire that change nothing, or it is not,
        /// in which case sending it changes nothing either.
        /// </remarks>
        private static List<X509Certificate2> OrderTowardsTheRoot(X509Certificate2        Leaf,
                                                                  List<X509Certificate2>  Pool)
        {

            var ordered  = new List<X509Certificate2>();
            var current  = Leaf;

            while (true)
            {

                var issuer = Pool.FirstOrDefault(candidate => candidate.SubjectName.RawData.AsSpan().
                                                                  SequenceEqual(current.IssuerName.RawData));

                if (issuer is null)
                    break;

                Pool.Remove(issuer);

                // A root signs itself, and is not sent.
                if (issuer.SubjectName.RawData.AsSpan().SequenceEqual(issuer.IssuerName.RawData))
                    break;

                ordered.Add(issuer);
                current = issuer;

            }

            return ordered;

        }

        #endregion

        #region (private static) WithPrivateKey(Certificate, Key)

        /// <summary>
        /// A certificate with its private key attached, in a form the platform
        /// can actually present.
        /// </summary>
        /// <remarks>
        /// Through PKCS#12, and built by Bouncy Castle rather than by .NET: a
        /// certificate whose key was attached in memory is handed to the
        /// platform's TLS stack without one, and .NET has no key object at all
        /// for an Ed448 or an ML-DSA key to attach. Bouncy Castle can write
        /// every one of them into a PKCS#12 blob, and loading that back is the
        /// one door every kind of key goes through.
        ///
        /// It throws where the platform will not take the result - which is
        /// information and not a fault: see the caller.
        /// </remarks>
        private static X509Certificate2 WithPrivateKey(X509Certificate2        Certificate,
                                                       AsymmetricKeyParameter  PrivateKey)
        {

            var store       = new Pkcs12StoreBuilder().Build();
            var bouncy      = new BCx509.X509CertificateParser().ReadCertificate(Certificate.RawData);
            var entry       = new X509CertificateEntry(bouncy);

            store.SetCertificateEntry(bouncy.SubjectDN.ToString(), entry);
            store.SetKeyEntry        (bouncy.SubjectDN.ToString(), new AsymmetricKeyEntry(PrivateKey), [ entry ]);

            using var blob  = new MemoryStream();

            var password    = Guid.NewGuid().ToString("N");

            store.Save(blob, password.ToCharArray(), new SecureRandom());

            return X509CertificateLoader.LoadPkcs12(
                       blob.ToArray(),
                       password,
                       X509KeyStorageFlags.Exportable
                   );

        }

        #endregion

        #region (private static) PublicKeyOf(PrivateKey) / KeyId

        /// <summary>
        /// The public half of a private key, whatever kind it is.
        /// </summary>
        /// <remarks>
        /// Bouncy Castle has no one method for this: an RSA private key carries
        /// the modulus and exponent that make up the public one, an elliptic
        /// curve key is a scalar that has to be multiplied by the generator,
        /// and the newer kinds simply hand theirs over. So one branch each,
        /// and a sentence rather than a silent null for anything else.
        /// </remarks>
        private static AsymmetricKeyParameter PublicKeyOf(AsymmetricKeyParameter PrivateKey)

            => PrivateKey switch {

                   RsaPrivateCrtKeyParameters rsa
                       => new RsaKeyParameters(false, rsa.Modulus, rsa.PublicExponent),

                   ECPrivateKeyParameters ec
                       => new ECPublicKeyParameters(
                              ec.AlgorithmName,
                              new FixedPointCombMultiplier().Multiply(ec.Parameters.G, ec.D),
                              ec.Parameters
                          ),

                   Ed25519PrivateKeyParameters ed25519  => ed25519.GeneratePublicKey(),
                   Ed448PrivateKeyParameters   ed448    => ed448.  GeneratePublicKey(),
                   MLDsaPrivateKeyParameters   mldsa    => mldsa.  GetPublicKey(),
                   SlhDsaPrivateKeyParameters  slhdsa   => slhdsa. GetPublicKey(),

                   _ => throw new NotSupportedException($"{PrivateKey.GetType().Name} is not a private key this {NodeWording.Noun} knows how to read.")

               };

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

        #region (private static) CreateCarrier(KeyPair, Algorithm, Id)

        /// <summary>
        /// The self-signed carrier certificate of a server key, as PEM, and the
        /// subject it was made out to.
        /// </summary>
        /// <remarks>
        /// <b>Built to be harmless when it is misused.</b> Nothing in it but the
        /// key is meant to be believed, so it claims as little as it can: no
        /// authority (basicConstraints CA:FALSE, critical), no keyCertSign, a
        /// digital signature for a TLS server and nothing else. A station that
        /// installed it as a root rather than pinning its key cannot be made by
        /// its key to trust a certificate it "issued" - provided the station's
        /// TLS stack honours what is written here (EVQI Transport, key pins,
        /// section 4, B0).
        ///
        /// A name that says what it is, and no subject alternative name: there
        /// is no name it could be valid for that a station would check, and one
        /// that looked like the controller's would invite checking it. Dated
        /// from 1970 to 9999, for the same reason.
        ///
        /// Built by Bouncy Castle, as the signing requests are, so that every
        /// kind of key in <see cref="Algorithms"/> gets one.
        /// </remarks>
        private static (String PEM, String Subject) CreateCarrier(AsymmetricCipherKeyPair  KeyPair,
                                                                  KeyAlgorithm             Algorithm,
                                                                  String                   Id)
        {

            var name       = new X509Name($"CN=Key pin carrier {Id},OU=Checked by its key pin only");
            var generator  = new X509V3CertificateGenerator();

            generator.SetSerialNumber(PKIFactory.RandomSerial());
            generator.SetIssuerDN    (name);
            generator.SetSubjectDN   (name);
            generator.SetNotBefore   (CarrierNotBefore.UtcDateTime);
            generator.SetNotAfter    (CarrierNotAfter. UtcDateTime);
            generator.SetPublicKey   (KeyPair.Public);

            generator.AddExtension(X509Extensions.BasicConstraints,      true,   new BasicConstraints(false));
            generator.AddExtension(X509Extensions.KeyUsage,              true,   new KeyUsage(KeyUsage.DigitalSignature));
            generator.AddExtension(X509Extensions.ExtendedKeyUsage,      false,  new ExtendedKeyUsage(KeyPurposeID.id_kp_serverAuth));

            var carrier    = generator.Generate(new Asn1SignatureFactory(Algorithm.SignatureAlgorithm, KeyPair.Private));

            using var loaded = X509CertificateLoader.LoadCertificate(carrier.GetEncoded());

            return (PemEncoding.WriteString("CERTIFICATE", carrier.GetEncoded()) + Environment.NewLine,
                    loaded.Subject);

        }

        #endregion

        #region (private) MetaOf(...) / RewriteMeta(Id, Change)

        /// <summary>
        /// What is said of a server key beside it, as its file holds it.
        /// </summary>
        private static String MetaOf(String           Id,
                                     String           Algorithm,
                                     DateTimeOffset   CreatedAt,
                                     String           Subject,
                                     Boolean          IsNext,
                                     DateTimeOffset?  CurrentSince)
        {

            var meta = new JObject(
                           new JProperty("id",         Id),
                           new JProperty("algorithm",  Algorithm),
                           new JProperty("createdAt",  CreatedAt.ToString("o")),
                           new JProperty("subject",    Subject),
                           new JProperty("carrier",    true),
                           new JProperty("next",       IsNext)
                       );

            if (CurrentSince.HasValue)
                meta.Add("currentSince", CurrentSince.Value.ToString("o"));

            return meta.ToString(Formatting.Indented) + Environment.NewLine;

        }

        /// <summary>
        /// Change what is said of a key, as it stands in its file: read, changed
        /// and written beside it, then moved over it in one step - so that a
        /// write the disk ran out in leaves the file as it was rather than a key
        /// nothing can be read about at the next start.
        /// </summary>
        private void RewriteMeta(String          Id,
                                 Action<JObject> Change)
        {

            var path  = FilePath(Id, "json");
            var meta  = File.Exists(path)
                            ? JObject.Parse(File.ReadAllText(path))
                            : new JObject(new JProperty("id", Id));

            Change(meta);

            try
            {
                WriteFile(FilePath(Id, "json.tmp"), meta.ToString(Formatting.Indented) + Environment.NewLine);
                File.Move(FilePath(Id, "json.tmp"), path, overwrite: true);
            }
            catch
            {
                Forget(Id, "json.tmp");
                throw;
            }

        }

        #endregion

        #region (private) FilePath / Known / CreateDirectory / Complain / WriteFile

        private String FilePath(String Id, String Extension)
            => System.IO.Path.Combine(Path, $"{Id}.{Extension}");

        private Boolean Known(String Id)
        {
            lock (updateLock)
                return entries.ContainsKey(Id);
        }

        /// <summary>
        /// The directory, readable by its owner alone - see <see cref="OwnerOnly"/>.
        /// </summary>
        private void CreateDirectory()
        {

            if (Directory.Exists(Path))
                return;

            OwnerOnly.Directory(Path, Complain);

        }

        /// <summary>
        /// Say what could not be restricted: keys others may read are worth a
        /// warning, not worth failing over.
        /// </summary>
        private void Complain(String Problem)

            => OnNotice?.Invoke(LogLevel.Warning, Problem);

        /// <summary>
        /// One file of this store written, readable by its owner alone where
        /// that is asked for - past <see cref="BeforeWriting"/> first.
        /// </summary>
        private void WriteFile(String   Target,
                               String   Content,
                               Boolean  OwnerOnly = false)
        {

            BeforeWriting?.Invoke(Target);

            if (OwnerOnly)
                OwnerOnlyFile.Write(Target, Content);

            else
                File.WriteAllText(Target, Content);

        }

        #endregion

        #region (private) Forget(Id, params Extensions)

        /// <summary>
        /// Take the files of one key that were written for something that did
        /// not go in away again - as far as that goes: one that cannot be
        /// taken away either is left where it is.
        /// </summary>
        private void Forget(String Id, params String[] Extensions)
        {
            foreach (var extension in Extensions)
            {
                try
                {
                    var file = FilePath(Id, extension);
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
        /// The files of one key moved aside, as "*.removed", all of them or
        /// none: where one cannot be, those moved already are moved back.
        /// </summary>
        /// <remarks>
        /// So that a key is taken away whole or not at all. Deleted one after
        /// the other, a file somebody held open left the ones before it gone:
        /// the key was listed until the next start, and gone or half there
        /// after it (found by the charging station). What cannot be moved back
        /// is left over under its "*.removed" name, which the next start does
        /// not read, and said in the log.
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

                var file = FilePath(Id, extension);

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
                publicKeys.Clear();

                served    = null;
                servedId  = null;
                servedAs  = null;

            }
        }

        #endregion

        #region (override) ToString()

        public override String ToString()
        {
            lock (updateLock)
                return $"{entries.Count} key(s) in '{Path}', {entries.Values.Count(entry => entry.Certificate is not null)} with a certificate";
        }

        #endregion

    }

}
