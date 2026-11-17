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
using System.Security.Cryptography.X509Certificates;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;

using org.GraphDefined.Vanaheimr.Hermod.PKI;


using cloud.charging.open.protocols.OCPP.Node.Serving;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Serving
{

    /// <summary>
    /// Every kind of key this local controller will make for itself, from the
    /// curve every charging station understands to the ones being prepared for.
    /// </summary>
    /// <remarks>
    /// Two questions are kept apart throughout, because they have different
    /// answers: whether a key and its signing request can be <em>made</em>, and
    /// whether the platform underneath can then <em>present</em> the
    /// certificate that comes back. The first is the same everywhere. The
    /// second depends on the operating system, the runtime and the year - so
    /// nothing here asserts which algorithms are presentable, only that
    /// whatever the answer is, the store behaves consistently with it.
    /// </remarks>
    public class KeyAlgorithmTests
    {

        #region Data

        private String                  directory  = default!;
        private TestClock               clock      = default!;
        private ServerCertificateStore  store      = default!;

        private static readonly String[] ReachableAs = [ "lc001.example.org", "192.168.1.10" ];

        /// <summary>
        /// Every algorithm, by identification - so a new one in the table is a
        /// new test case without a line being added here.
        /// </summary>
        public static IEnumerable<String> EveryAlgorithm
            => KeyAlgorithm.All.Select(algorithm => algorithm.Id);

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeAStore()
        {
            directory = ServingTestDirectories.TemporaryDirectory("algorithms");
            clock     = TestClock.At(2026, 6, 1);
            store     = new ServerCertificateStore(directory, clock);
        }

        [TearDown]
        public void RemoveTheDirectory()
        {
            store?.Dispose();
            ServingTestDirectories.Remove(directory);
        }

        #endregion


        #region EveryAlgorithmMakesAKeyAndARequestThatVerifies()

        /// <summary>
        /// The signing request is signed by the key it names, and says so. A
        /// certificate authority checks exactly this before it issues anything,
        /// so a request that does not verify is a wasted trip.
        /// </summary>
        [Test]
        [TestCaseSource(nameof(EveryAlgorithm))]
        public void EveryAlgorithmMakesAKeyAndARequestThatVerifies(String Algorithm)
        {

            Assert.That(store.TryCreateKey("lc001.example.org", ReachableAs, Algorithm, out var id, out var csr, out var error),
                        Is.True, error);

            var request = new PemReader(new StringReader(csr!)).ReadObject() as Pkcs10CertificationRequest;

            Assert.That(request, Is.Not.Null, "What came back is not a PKCS#10 signing request.");

            var info = request!.GetCertificationRequestInfo();

            Assert.Multiple(() => {

                Assert.That(request.Verify(), Is.True,
                            "The signing request is not signed by the key it names.");

                Assert.That(info.Subject.ToString(), Does.Contain("lc001.example.org"));

                Assert.That(store.Entries.Single(entry => entry.Id == id).Algorithm,
                            Is.EqualTo(KeyAlgorithm.Find(Algorithm)!.Name));

            });

        }

        #endregion

        #region EveryRequestCarriesTheNamesAndWhatItIsFor()

        /// <summary>
        /// The names go in through Bouncy Castle now, for every algorithm
        /// alike - and a request without them produces a certificate no
        /// charging station accepts, whichever key it was made with.
        /// </summary>
        [Test]
        [TestCaseSource(nameof(EveryAlgorithm))]
        public void EveryRequestCarriesTheNamesAndWhatItIsFor(String Algorithm)
        {

            Assert.That(store.TryCreateKey("lc001.example.org", ReachableAs, Algorithm, out _, out var csr, out var error),
                        Is.True, error);

            var request    = (Pkcs10CertificationRequest) new PemReader(new StringReader(csr!)).ReadObject();
            var extensions = request.GetRequestedExtensions();

            Assert.That(extensions, Is.Not.Null, "The request asked for no extensions at all.");

            var names = GeneralNames.GetInstance(
                            extensions!.GetExtensionParsedValue(X509Extensions.SubjectAlternativeName)
                        ).GetNames();

            var extendedKeyUsage = ExtendedKeyUsage.GetInstance(
                                       extensions.GetExtensionParsedValue(X509Extensions.ExtendedKeyUsage)
                                   );

            Assert.Multiple(() => {

                Assert.That(names.Any(name => name.TagNo == GeneralName.DnsName   && name.Name.ToString() == "lc001.example.org"),
                            Is.True, "The domain name is not in the request.");

                Assert.That(names.Any(name => name.TagNo == GeneralName.IPAddress),
                            Is.True, "The IP address is not in the request.");

                Assert.That(extendedKeyUsage.HasKeyPurposeId(KeyPurposeID.id_kp_serverAuth),
                            Is.True, "The request did not ask for a server certificate.");

            });

        }

        #endregion

        #region AKeyOfEveryKindSurvivesARestart()

        /// <summary>
        /// Written as PKCS#8 and read back by what the encoding says it is - so
        /// nothing has to be told in advance which of a dozen kinds to expect.
        /// </summary>
        [Test]
        [TestCaseSource(nameof(EveryAlgorithm))]
        public void AKeyOfEveryKindSurvivesARestart(String Algorithm)
        {

            Assert.That(store.TryCreateKey("lc001.example.org", ReachableAs, Algorithm, out var id, out _, out var error),
                        Is.True, error);

            using var restarted = new ServerCertificateStore(directory, clock);

            var entry = restarted.Entries.SingleOrDefault(entry => entry.Id == id);

            Assert.Multiple(() => {
                Assert.That(entry,            Is.Not.Null, "The key was not read back.");
                Assert.That(entry!.Algorithm, Is.EqualTo(KeyAlgorithm.Find(Algorithm)!.Name));
                Assert.That(entry.Id,         Is.EqualTo(id),
                            "The key came back under a different identification, so its request and its certificate would be filed apart.");
            });

        }

        #endregion

        #region ACertificateOfEveryKindIsEitherPresentedOrExplained()

        /// <summary>
        /// The invariant that holds on every platform: a certificate is either
        /// one this machine can present - and then it may be chosen - or one it
        /// cannot, and then it is kept, passed over, and says why.
        /// </summary>
        /// <remarks>
        /// Deliberately not "Ed448 does not work here". That would be a fact
        /// about this machine written into a test, and it would fail on the
        /// next one - which is the opposite of what a test is for. What is
        /// asserted is that the store and the page never disagree with the
        /// platform.
        /// </remarks>
        [Test]
        [TestCaseSource(nameof(EveryAlgorithm))]
        public void ACertificateOfEveryKindIsEitherPresentedOrExplained(String Algorithm)
        {

            using var ca = TestCA.Create("Test CA");

            Assert.That(store.TryCreateKey("lc001.example.org", ReachableAs, Algorithm, out var id, out var csr, out var keyError),
                        Is.True, keyError);

            using var certificate = ca.Sign(csr!, clock.Now.AddDays(-1), clock.Now.AddYears(1));

            Assert.That(store.TryAddCertificate(ca.ChainPEM(certificate), ReachableAs, out var taken, out _, out var addError),
                        Is.True, addError);

            Assert.That(taken, Is.EqualTo(id),
                        "A certificate was filed under a key other than the one it belongs to.");

            var entry  = store.Entries.Single(entry => entry.Id == id);
            var chosen = store.Select();

            if (entry.CanBePresented)
                Assert.Multiple(() => {
                    Assert.That(entry.Certificate,               Is.Not.Null);
                    Assert.That(entry.Certificate!.HasPrivateKey, Is.True,
                                "A certificate said to be presentable came back without a usable private key.");
                    Assert.That(chosen,                          Is.Not.Null);
                    Assert.That(store.ServedId,                  Is.EqualTo(id));
                });

            else
                Assert.Multiple(() => {

                    Assert.That(chosen,      Is.Null,
                                "A certificate this machine cannot present was chosen anyway; every handshake would fail.");

                    Assert.That(store.HasCertificate, Is.False,
                                "The port would be switched to TLS for a certificate that cannot be presented.");

                    Assert.That(entry.Warnings.Any(warning => warning.Contains("cannot")), Is.True,
                                $"Nothing was said about why it cannot be used: {String.Join(" | ", entry.Warnings)}");

                    // Of this one certificate: "It is kept" followed "...
                    // certificates cannot be presented", with nothing for "it"
                    // to be (found by the CSMS).
                    Assert.That(entry.Warnings.Where(warning => warning.Contains("the handshake fails")),
                                Has.All.Contains("This one is kept"),
                                "what is kept is this certificate, said as one");

                });

        }

        #endregion

        #region TheAnswerAboutPresentingIsTheSameEveryTime()

        /// <summary>
        /// It is asked by doing a handshake, so it had better be asked once.
        /// </summary>
        [Test]
        public void TheAnswerAboutPresentingIsTheSameEveryTime()
        {

            using var ca = TestCA.Create("Test CA");

            store.TryCreateKey("lc001.example.org", ReachableAs, "ecdsa-p256", out var id, out var csr, out _);

            using var certificate = ca.Sign(csr!, clock.Now.AddDays(-1), clock.Now.AddYears(1));

            store.TryAddCertificate(ca.ChainPEM(certificate), ReachableAs, out _, out _, out _);

            var entry = store.Entries.Single(entry => entry.Id == id);

            var first  = KeyAlgorithm.CanBePresented("ecdsa-p256", entry.Certificate!);
            var second = KeyAlgorithm.CanBePresented("ecdsa-p256", entry.Certificate!);

            Assert.Multiple(() => {
                Assert.That(first,  Is.True, "A P-256 certificate could not be presented, which would be a broken machine.");
                Assert.That(second, Is.EqualTo(first));
                Assert.That(KeyAlgorithm.Find("ecdsa-p256")!.KnownToBePresentable, Is.True);
            });

        }

        #endregion

        #region WhatTheWebInterfaceIsOfferedIsWhatCanBeAskedFor()

        [Test]
        public void WhatTheWebInterfaceIsOfferedIsWhatCanBeAskedFor()
        {

            var offered = store.ToJSON()["algorithms"]!.
                              Select(algorithm => algorithm.Value<String>("id")).
                              ToArray();

            Assert.Multiple(() => {

                Assert.That(offered, Is.EqualTo(KeyAlgorithm.All.Select(algorithm => algorithm.Id)));

                // The ones this was asked for by name.
                Assert.That(offered, Does.Contain("ed448"));
                Assert.That(offered, Does.Contain("ed25519"));
                Assert.That(offered, Does.Contain("ecdsa-p521"));
                Assert.That(offered, Does.Contain("ml-dsa-65"));

                // And every one says something about itself, because "ML-DSA-87"
                // on its own tells nobody whether to pick it.
                foreach (var algorithm in store.ToJSON()["algorithms"]!)
                    Assert.That(algorithm.Value<String>("remark"), Is.Not.Null.And.Not.Empty,
                                $"'{algorithm.Value<String>("id")}' is offered without a word about what it is.");

            });

        }

        #endregion

        #region AKeyThisControllerDoesNotKnowIsStillRefused()

        [Test]
        public void AKeyThisControllerDoesNotKnowIsStillRefused()
        {

            Assert.Multiple(() => {

                Assert.That(store.TryCreateKey("lc001", ReachableAs, "secp521r2", out _, out _, out var mistyped), Is.False,
                            "A curve that does not exist was accepted.");
                Assert.That(mistyped, Does.Contain("secp521r2"));

                Assert.That(store.TryCreateKey("lc001", ReachableAs, "dsa-512", out _, out _, out var ancient), Is.False);
                Assert.That(ancient,  Does.Contain("dsa-512"));

            });

        }

        #endregion

    }

}
