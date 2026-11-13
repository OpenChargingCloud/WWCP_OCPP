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

using System.Security.Cryptography;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;

using org.GraphDefined.Vanaheimr.Hermod.PKI;

using cloud.charging.open.protocols.OCPP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.TestKit;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Connections
{

    /// <summary>
    /// The keys and certificates this charging station holds up when it dials
    /// a back end.
    /// </summary>
    /// <remarks>
    /// Two questions are kept apart throughout, because they have different
    /// answers: whether a key and its signing request can be <em>made</em>,
    /// which is the same everywhere and true of all thirteen kinds, and whether
    /// this platform can then <em>load</em> the certificate that comes back
    /// together with its key - which depends on the runtime and the year. .NET
    /// has no key object at all for an Ed448 or an ML-DSA key today.
    ///
    /// The certificate authority in here stays a P-256 one throughout, and
    /// issues to subjects of every kind. That is deliberate and it is what a
    /// real fleet looks like - whoever issues certificates does not change
    /// their root because one station asked for a newer kind of key. It used
    /// to be the only thing that worked: the signature was chosen from the
    /// subject's key rather than the issuer's, so an authority could only
    /// issue to its own family. That is fixed in Hermod, and
    /// EveryAlgorithmGoesTheWholeWayRound is what says so.
    /// </remarks>
    [TestFixture]
    public class ClientCertificateTests
    {

        #region Data

        private String                  directory  = "";
        private ClientCertificateStore  store      = default!;

        /// <summary>
        /// A clock the tests decide, because what is "expired" depends on it.
        /// </summary>
        private MovableClock clock      = default!;

        /// <summary>
        /// Every algorithm, by identification - so a new one in Hermod's list
        /// is a new test case without a line being added here.
        /// </summary>
        public static IEnumerable<String> EveryAlgorithm
            => KeyAlgorithm.All.Select(algorithm => algorithm.Id);

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeAStore()
        {

            directory  = TestNodes.TemporaryDirectory("certificates");
            clock      = new MovableClock(DateTimeOffset.UtcNow);
            store      = new ClientCertificateStore(Path.Combine(directory, "keys"), clock);

        }

        [TearDown]
        public void TakeItAwayAgain()
        {
            store?.Dispose();
            TestNodes.Remove(directory);
        }

        #endregion

        #region (private) ACertificateFor(Id, Days = 365)

        /// <summary>
        /// What a certificate authority would send back for a key that is
        /// already here: the signing request, signed.
        /// </summary>
        private String ACertificateFor(String  Id,
                                       Int32   Days   = 365)
        {

            Assert.That(store.TryReadCSR(Id, out var csr, out var error), Is.True, error);

            var request  = (new PemReader(new StringReader(csr!)).ReadObject() as Pkcs10CertificationRequest)!;

            var caKeys   = PKIFactory.GenerateECCKeyPair("secp256r1");
            var ca       = PKIFactory.CreateRootCACertificate(
                               RootKeyPair:  caKeys,
                               SubjectName:  "Test CSMS CA"
                           );

            var issued   = PKIFactory.SignClientCertificate(
                               request,
                               caKeys.Private,
                               ca,
                               lifeTime: TimeSpan.FromDays(Days)
                           );

            return String.Join(
                       Environment.NewLine,
                       PemEncoding.WriteString("CERTIFICATE", issued.GetEncoded()),
                       PemEncoding.WriteString("CERTIFICATE", ca.    GetEncoded())
                   ) + Environment.NewLine;

        }

        #endregion


        #region EveryAlgorithmMakesAKeyAndARequestThatVerifies(Algorithm)

        /// <summary>
        /// The signing request is signed by the key it names, and says so.
        /// </summary>
        /// <remarks>
        /// A certificate authority checks exactly this before it issues
        /// anything, so a request that does not verify is a wasted trip - and
        /// for Ed448 and ML-DSA it is the whole point: .NET cannot sign one of
        /// these at all.
        /// </remarks>
        [Test]
        [TestCaseSource(nameof(EveryAlgorithm))]
        public void EveryAlgorithmMakesAKeyAndARequestThatVerifies(String Algorithm)
        {

            Assert.That(store.TryCreateKey("cs001.example.org", Algorithm, out var id, out var csr, out var error),
                        Is.True, error);

            var request = new PemReader(new StringReader(csr!)).ReadObject() as Pkcs10CertificationRequest;

            Assert.That(request, Is.Not.Null, "What came back is not a PKCS#10 signing request.");

            Assert.Multiple(() => {

                Assert.That(request!.Verify(), Is.True,
                            "The signing request is not signed by the key it names.");

                Assert.That(request.GetCertificationRequestInfo().Subject.ToString(),
                            Does.Contain("cs001.example.org"));

                Assert.That(store.Entries.Single(entry => entry.Id == id).Algorithm,
                            Is.EqualTo(KeyAlgorithm.Find(Algorithm)!.Name));

            });

        }

        #endregion

        #region EveryRequestAsksForAClientCertificate(Algorithm)

        /// <summary>
        /// A station asks for a certificate to dial with, and says so.
        /// </summary>
        /// <remarks>
        /// Said in the request rather than hoped for in the answer: a
        /// certificate authority handed a request without an extended key usage
        /// frequently issues something that is not a client certificate, and a
        /// station finds that out at the next handshake.
        /// </remarks>
        [Test]
        [TestCaseSource(nameof(EveryAlgorithm))]
        public void EveryRequestAsksForAClientCertificate(String Algorithm)
        {

            Assert.That(store.TryCreateKey("cs001.example.org", Algorithm, out _, out var csr, out var error),
                        Is.True, error);

            var request     = (new PemReader(new StringReader(csr!)).ReadObject() as Pkcs10CertificationRequest)!;

            var extensions  = X509Extensions.GetInstance(
                                  AttributePkcs.GetInstance(
                                      request.GetCertificationRequestInfo().Attributes!.ToArray().
                                          First(entry => AttributePkcs.GetInstance(entry).AttrType.
                                                             Equals(PkcsObjectIdentifiers.Pkcs9AtExtensionRequest))
                                  ).AttrValues[0]
                              );

            var usage       = ExtendedKeyUsage.GetInstance(
                                  extensions.GetExtensionParsedValue(X509Extensions.ExtendedKeyUsage)
                              );

            Assert.Multiple(() => {

                Assert.That(usage.HasKeyPurposeId(KeyPurposeID.id_kp_clientAuth), Is.True,
                            "The request does not ask for a client certificate.");

                Assert.That(usage.HasKeyPurposeId(KeyPurposeID.id_kp_serverAuth), Is.False,
                            "The request also asks to be a server, which this station is not.");

                // A station is not something anybody dials, so there is nothing
                // it is reachable as.
                Assert.That(extensions.GetExtension(X509Extensions.SubjectAlternativeName), Is.Null,
                            "The request names addresses this station is supposedly reachable at.");

            });

        }

        #endregion

        #region AKeySurvivesARestart()

        /// <summary>
        /// What was written down is what comes back.
        /// </summary>
        /// <remarks>
        /// A store is read from its directory at every start, so a key that
        /// cannot be read back is a key that was lost the moment the station
        /// restarted - which is exactly when nobody is watching.
        /// </remarks>
        [Test]
        public void AKeySurvivesARestart()
        {

            Assert.That(store.TryCreateKey("cs001.example.org", "ecdsa-p384", out var id, out var csr, out var error),
                        Is.True, error);

            store.Dispose();

            using var again = new ClientCertificateStore(Path.Combine(directory, "keys"), clock);

            var entry = again.Entries.SingleOrDefault(one => one.Id == id);

            Assert.That(entry, Is.Not.Null, "The key is gone after a restart.");

            Assert.Multiple(() => {
                Assert.That(entry!.Algorithm, Is.EqualTo("ECDSA P-384 (secp384r1)"),
                            "What kind of key it is was not remembered.");
                Assert.That(entry.Subject,    Does.Contain("cs001.example.org"));
                Assert.That(again.TryReadCSR(id!, out var readBack, out _), Is.True);
                Assert.That(readBack,         Is.EqualTo(csr));
            });

        }

        #endregion

        #region ACertificateIsTakenInAndHeldUp()

        /// <summary>
        /// The ordinary way round: a key, a request, an answer, and a station
        /// that now has something to dial with.
        /// </summary>
        [Test]
        public void ACertificateIsTakenInAndHeldUp()
        {

            Assert.That(store.InUse, Is.Null, "A station with no certificate thinks it has one.");

            Assert.That(store.TryCreateKey("cs001.example.org", "ecdsa-p256", out var id, out _, out var error),
                        Is.True, error);

            Assert.That(store.InUse, Is.Null,
                        "A key whose signing request is still out is being treated as a certificate.");

            Assert.That(store.TryAddCertificate(ACertificateFor(id!), out var takenIn, out var warnings, out var problem),
                        Is.True, problem);

            Assert.Multiple(() => {

                Assert.That(takenIn,          Is.EqualTo(id),   "The certificate was filed under another key.");
                Assert.That(store.InUse?.Id,  Is.EqualTo(id),   "The station is not holding up the certificate it was just given.");

                // The root is not sent along: whoever is to trust it has it
                // already, or does not.
                Assert.That(store.InUse!.Intermediates.Count, Is.Zero,
                            "The root certificate was kept as an intermediate.");

                // A private authority is not one this station has to know, so
                // the chain not verifying here is worth saying and not worth
                // refusing.
                Assert.That(warnings.Any(warning => warning.Contains("chain")), Is.True,
                            "Nothing was said about a chain this station cannot verify.");

            });

        }

        #endregion

        #region EveryAlgorithmGoesTheWholeWayRound(Algorithm)

        /// <summary>
        /// A key of every kind, a request, a certificate back, and a station
        /// holding it up - not just the elliptic curve ones.
        /// </summary>
        /// <remarks>
        /// The whole round trip, because until now only half of it was
        /// measured. Every algorithm was shown to make a signing request that
        /// verifies, but the answer only ever came back for an elliptic curve
        /// key - not by choice, but because Hermod's signer chose the signature
        /// from the subject's key instead of the issuer's, and this test's
        /// certificate authority is a P-256 one. Asking it for an Ed448
        /// certificate ended in a cast failing deep inside Bouncy Castle.
        ///
        /// So a station could be given an Ed448 or an ML-DSA key and a request
        /// to hand out, and then had nowhere to take the answer. That is the
        /// half this measures, and it is the half somebody notices.
        ///
        /// The authority stays P-256 on purpose. It is the mismatch that is
        /// being tested, and it is also what a real fleet looks like: whoever
        /// issues certificates does not change their root because one station
        /// asked for a newer kind of key.
        ///
        /// Whether the station can then hold the certificate up is a second
        /// question with a different answer, and it is asked separately rather
        /// than pinned: it is about what the runtime has a key object for
        /// today. .NET gained ML-DSA in 10 and has nothing for Ed448 yet, and
        /// that is a sentence with a date in it - so what is asserted is that
        /// the station either holds the certificate up or says why it cannot,
        /// never which of the two it is.
        /// </remarks>
        [Test]
        [TestCaseSource(nameof(EveryAlgorithm))]
        public void EveryAlgorithmGoesTheWholeWayRound(String Algorithm)
        {

            Assert.That(store.TryCreateKey("cs001.example.org", Algorithm, out var id, out _, out var error),
                        Is.True, error);

            Assert.That(store.TryAddCertificate(ACertificateFor(id!), out var takenIn, out _, out var problem),
                        Is.True, $"A {Algorithm} certificate could not be taken in: {problem}");

            var entry = store.Entries.Single(entry => entry.Id == id);

            Assert.Multiple(() => {

                Assert.That(takenIn,            Is.EqualTo(id),
                            $"The {Algorithm} certificate was filed under another key.");

                Assert.That(entry.Certificate,  Is.Not.Null,
                            $"The {Algorithm} certificate was accepted and then not kept.");

                Assert.That(entry.Certificate!.Subject, Does.Contain("cs001.example.org"),
                            $"The {Algorithm} certificate is about somebody else.");

                if (entry.CanBeHeldUp)
                    Assert.That(store.InUse?.Id, Is.EqualTo(id),
                                $"The station can hold up the {Algorithm} certificate and is not doing it.");

                else
                    Assert.That(entry.CannotBeHeldUp, Is.Not.Null.And.Not.Empty,
                                $"The {Algorithm} certificate is not being held up and nobody is told why.");

            });

        }

        #endregion

        #region ACertificateForSomebodyElsesKeyIsRefused()

        /// <summary>
        /// A certificate is only usable here if it answers a request made here.
        /// </summary>
        [Test]
        public void ACertificateForSomebodyElsesKeyIsRefused()
        {

            Assert.That(store.TryCreateKey("cs001.example.org", "ecdsa-p256", out var id, out _, out var error),
                        Is.True, error);

            var mine = ACertificateFor(id!);

            // Another station's certificate, made the same way.
            using var elsewhere = new ClientCertificateStore(Path.Combine(directory, "elsewhere"), clock);

            Assert.That(elsewhere.TryCreateKey("cs002.example.org", "ecdsa-p256", out var otherId, out _, out error),
                        Is.True, error);

            Assert.That(elsewhere.TryReadCSR(otherId!, out var otherCSR, out _), Is.True);

            var theirs = ACertificateForRequest(otherCSR!);

            Assert.That(store.TryAddCertificate(theirs, out _, out _, out var refused), Is.False,
                        "A certificate belonging to another station's key was taken in.");

            Assert.That(refused, Does.Contain("belongs to a key of this charging station").IgnoreCase.Or.
                                 Contains("signing request made here"));

            // And the one that does belong here still works afterwards.
            Assert.That(store.TryAddCertificate(mine, out _, out _, out var problem), Is.True, problem);

        }

        /// <summary>The same as ACertificateFor, for a request from elsewhere.</summary>
        private static String ACertificateForRequest(String CSR)
        {

            var request  = (new PemReader(new StringReader(CSR)).ReadObject() as Pkcs10CertificationRequest)!;
            var caKeys   = PKIFactory.GenerateECCKeyPair("secp256r1");
            var ca       = PKIFactory.CreateRootCACertificate(RootKeyPair: caKeys, SubjectName: "Another CA");

            return PemEncoding.WriteString(
                       "CERTIFICATE",
                       PKIFactory.SignClientCertificate(request, caKeys.Private, ca).GetEncoded()
                   ) + Environment.NewLine;

        }

        #endregion

        #region AnExpiredCertificateIsRefusedWhileSomebodyIsLooking()

        /// <summary>
        /// A certificate that can never be used is refused now, not at the next
        /// handshake.
        /// </summary>
        /// <remarks>
        /// And by this station's clock, which is named in the sentence: the
        /// usual cause of a certificate that looks expired is a station whose
        /// clock is wrong, and somebody reading "it is now 1970" knows at once
        /// what to look at.
        /// </remarks>
        [Test]
        public void AnExpiredCertificateIsRefusedWhileSomebodyIsLooking()
        {

            Assert.That(store.TryCreateKey("cs001.example.org", "ecdsa-p256", out var id, out _, out var error),
                        Is.True, error);

            var certificate = ACertificateFor(id!, Days: 30);

            // Two months later.
            clock.Now = clock.Now.AddDays(60);

            Assert.That(store.TryAddCertificate(certificate, out _, out _, out var refused), Is.False,
                        "A certificate that had already expired was taken in.");

            Assert.Multiple(() => {
                Assert.That(refused, Does.Contain("expired"));
                Assert.That(refused, Does.Contain("clock of this charging station"),
                            "The station did not say which clock it is going by.");
            });

        }

        #endregion

        #region TheCertificateInUseCannotBeRemoved()

        /// <summary>
        /// A station does not delete the certificate it dials with.
        /// </summary>
        /// <remarks>
        /// It would keep working until the next restart, and the way to notice
        /// would be a car park that had stopped reporting.
        /// </remarks>
        [Test]
        public void TheCertificateInUseCannotBeRemoved()
        {

            Assert.That(store.TryCreateKey("cs001.example.org", "ecdsa-p256", out var id, out _, out var error),
                        Is.True, error);

            Assert.That(store.TryAddCertificate(ACertificateFor(id!), out _, out _, out var problem), Is.True, problem);

            Assert.That(store.TryRemove(id!, out var refused), Is.False,
                        "The certificate the station is holding up was removed.");

            Assert.That(refused, Does.Contain("holding up"));

            // A key whose request is still out is nobody's certificate, and
            // goes without argument.
            Assert.That(store.TryCreateKey("cs001.example.org", "ecdsa-p384", out var spare, out _, out error),
                        Is.True, error);

            Assert.That(store.TryRemove(spare!, out var wentWrong), Is.True, wentWrong);
            Assert.That(store.Entries.Any(entry => entry.Id == spare), Is.False);

        }

        #endregion

        #region WhatSomebodyMayNotAskFor()

        /// <summary>
        /// The two things a person types, and what happens when they are wrong.
        /// </summary>
        [Test]
        public void WhatSomebodyMayNotAskFor()
        {

            Assert.Multiple(() => {

                Assert.That(store.TryCreateKey("cs001.example.org", "quantum-banana", out _, out _, out var unknown),
                            Is.False, "A key algorithm nobody has heard of was accepted.");
                Assert.That(unknown, Does.Contain("ecdsa-p256"),
                            "The refusal does not say what may be asked for instead.");

                Assert.That(store.TryCreateKey("   ", "ecdsa-p256", out _, out _, out var empty),
                            Is.False, "A certificate with nobody's name on it was made.");
                Assert.That(empty, Does.Contain("who this station is"));

                Assert.That(store.TryCreateKey(new String('x', ClientCertificateStore.MaxSubjectLength + 1),
                                               "ecdsa-p256", out _, out _, out var tooLong),
                            Is.False);
                Assert.That(tooLong, Does.Contain("at most"));

                Assert.That(store.TryReadCSR("nothing-like-this", out _, out var missing), Is.False);
                Assert.That(missing, Does.Contain("no key"));

                Assert.That(store.TryAddCertificate("this is not a certificate", out _, out _, out var notPEM), Is.False);
                Assert.That(notPEM, Is.Not.Null);

            });

        }

        #endregion

        #region ThePrivateKeyNeverLeavesTheDirectory()

        /// <summary>
        /// There is no way to import one, and the page is told so.
        /// </summary>
        /// <remarks>
        /// A key that arrived from somewhere else is a key somebody else has a
        /// copy of. The answer carries it as a fact rather than leaving
        /// somebody hunting for a button that was never there.
        /// </remarks>
        [Test]
        public void ThePrivateKeyNeverLeavesTheDirectory()
        {

            Assert.That(store.TryCreateKey("cs001.example.org", "ecdsa-p256", out var id, out _, out var error),
                        Is.True, error);

            var json = store.ToJSON();

            Assert.Multiple(() => {

                Assert.That(json.Value<Boolean>("canImportPrivateKeys"), Is.False);

                // Everything a page needs to offer the choice.
                Assert.That(json["algorithms"]?.Count(), Is.EqualTo(KeyAlgorithm.All.Count));
                Assert.That(json.Value<String>("defaultAlgorithm"), Is.EqualTo("ecdsa-p256"));

                // And nothing that would let a key out of here.
                Assert.That(json.ToString(), Does.Not.Contain("PRIVATE KEY"),
                            "The private key is in what the web interface is handed.");

                Assert.That(File.Exists(Path.Combine(directory, "keys", $"{id}.key.pem")), Is.True,
                            "The private key was not written where it belongs.");

            });

        }

        #endregion

    }

}
