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

using cloud.charging.open.protocols.WWCP.Node.Logging;


using cloud.charging.open.protocols.OCPP.Node.Serving;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Serving
{

    /// <summary>
    /// The keys and certificates the charging station server authenticates
    /// with: making them, taking them in, and which of them is presented on any
    /// given day.
    /// </summary>
    /// <remarks>
    /// Everything here runs against a clock that can be moved, because
    /// everything here is about dates. A certificate that takes over in two
    /// days cannot be tested by waiting two days.
    /// </remarks>
    public class ServerCertificateTests
    {

        #region Data

        private String                  directory  = default!;
        private TestClock               clock      = default!;
        private ServerCertificateStore  store      = default!;

        private static readonly String[] ReachableAs = [ "lc001.example.org", "192.168.1.10" ];

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeAStore()
        {
            directory = ServingTestDirectories.TemporaryDirectory("certificates");
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

        #region (private) NewKey(...) / Issue(...) / CutOff(...)

        /// <summary>
        /// A key and its signing request.
        /// </summary>
        private (String Id, String CSR) NewKey(String? Algorithm = null)
        {

            Assert.That(store.TryCreateKey("lc001.example.org", ReachableAs, Algorithm, out var id, out var csr, out var error),
                        Is.True, error);

            return (id!, csr!);

        }

        /// <summary>
        /// A key, a certificate for it over the given period, and that
        /// certificate taken in.
        /// </summary>
        private String Issue(TestCA          CA,
                             DateTimeOffset  NotBefore,
                             DateTimeOffset  NotAfter)
        {

            var (id, csr)         = NewKey();
            using var certificate = CA.Sign(csr, NotBefore, NotAfter);

            Assert.That(store.TryAddCertificate(CA.ChainPEM(certificate), ReachableAs, out var taken, out _, out var error),
                        Is.True, error);

            Assert.That(taken, Is.EqualTo(id));

            return id;

        }

        /// <summary>
        /// The store's writes of a file with this in its name fail partway
        /// through, as on a full disk: the file is there, with the beginning
        /// of what was to go into it, and the write throws.
        /// </summary>
        private void CutOff(String Name, String Beginning)
        {
            store.BeforeWriting = path => {

                if (!Path.GetFileName(path).Contains(Name, StringComparison.Ordinal))
                    return;

                File.WriteAllText(path, Beginning);

                throw new IOException("There is not enough space on the disk.");

            };
        }

        #endregion


        #region ARequestNamesEverythingThisControllerIsReachedAs()

        /// <summary>
        /// A certificate that does not carry the name a charging station dialled
        /// is a certificate the station refuses, whatever else is right about
        /// it. So the names go into the request, and both kinds of them.
        /// </summary>
        [Test]
        public void ARequestNamesEverythingThisControllerIsReachedAs()
        {

            var (_, csr) = NewKey();

            var request  = CertificateRequest.LoadSigningRequestPem(
                               csr,
                               HashAlgorithmName.SHA256,
                               CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions
                           );

            var san      = request.CertificateExtensions.OfType<X509SubjectAlternativeNameExtension>().FirstOrDefault();

            Assert.Multiple(() => {

                Assert.That(csr, Does.StartWith("-----BEGIN CERTIFICATE REQUEST-----"));
                Assert.That(san, Is.Not.Null, "The signing request named nothing this controller is reachable as.");

                Assert.That(san!.EnumerateDnsNames(),                                   Is.EquivalentTo(new[] { "lc001.example.org" }));
                Assert.That(san. EnumerateIPAddresses().Select(address => address.ToString()), Is.EquivalentTo(new[] { "192.168.1.10" }));

                Assert.That(request.CertificateExtensions.OfType<X509EnhancedKeyUsageExtension>().
                                Single().EnhancedKeyUsages.Cast<Oid>().Select(oid => oid.Value),
                            Does.Contain("1.3.6.1.5.5.7.3.1"),
                            "The request did not ask for a server certificate.");

            });

        }

        #endregion

        #region ARequestWithoutNamesIsRefused()

        /// <summary>
        /// Rather than handing somebody a request that produces a certificate
        /// nothing will accept.
        /// </summary>
        [Test]
        public void ARequestWithoutNamesIsRefused()
        {

            Assert.That(store.TryCreateKey("lc001", [], null, out _, out _, out var error), Is.False);
            Assert.That(error, Does.Contain("reachable as"));

        }

        #endregion

        #region AKeyThisControllerDoesNotGenerateIsRefused()

        [Test]
        public void AKeyThisControllerDoesNotGenerateIsRefused()
        {

            Assert.That(store.TryCreateKey("lc001", ReachableAs, "dsa-512", out _, out _, out var error), Is.False);
            Assert.That(error, Does.Contain("dsa-512"));

        }

        #endregion

        #region ThePrivateKeyIsWrittenDownAndNeverHandedOut()

        /// <summary>
        /// It is generated here and stays here: what leaves is the signing
        /// request, which is a public document.
        /// </summary>
        [Test]
        public void ThePrivateKeyIsWrittenDownAndNeverHandedOut()
        {

            var (id, _) = NewKey();

            var onDisk  = File.ReadAllText(Path.Combine(directory, $"{id}.key.pem"));
            var shown   = store.ToJSON().ToString();

            Assert.Multiple(() => {

                Assert.That(onDisk, Does.Contain("PRIVATE KEY"));

                Assert.That(shown,  Does.Not.Contain("PRIVATE KEY"),
                            "The private key turned up in what the web interface is shown.");

                Assert.That(shown,  Does.Not.Contain(onDisk.Split('\n')[1].Trim()),
                            "A line of the private key turned up in what the web interface is shown.");

                Assert.That(store.ToJSON().Value<Boolean>("canImportPrivateKeys"), Is.False,
                            "The web interface is being offered a way to import a private key.");

            });

        }

        #endregion

        #region ARequestCanBeCollectedAgainLater()

        /// <summary>
        /// Frequently by somebody who is not the person who pressed the button.
        /// </summary>
        [Test]
        public void ARequestCanBeCollectedAgainLater()
        {

            var (id, csr) = NewKey();

            Assert.That(store.TryReadCSR(id, out var again, out var error), Is.True, error);
            Assert.That(again, Is.EqualTo(csr));

        }

        #endregion


        #region ACertificateForAKeyThatIsNotHereIsRefused()

        /// <summary>
        /// A certificate is only usable here if it answers a request made here.
        /// </summary>
        [Test]
        public void ACertificateForAKeyThatIsNotHereIsRefused()
        {

            using var ca      = TestCA.Create("Somebody Else");
            using var foreign = ca.SignFor("lc001.example.org", clock.Now.AddDays(-1), clock.Now.AddYears(1), ClientAuthentication: false);

            NewKey();

            Assert.Multiple(() => {
                Assert.That(store.TryAddCertificate(TestCA.ToPEM(foreign), ReachableAs, out _, out _, out var error), Is.False);
                Assert.That(error, Does.Contain("belongs to a key"));
            });

        }

        #endregion

        #region SomethingThatIsNotACertificateIsRefused()

        [Test]
        public void SomethingThatIsNotACertificateIsRefused()
        {

            NewKey();

            Assert.That(store.TryAddCertificate("Dear sir, please find the certificate attached.", ReachableAs, out _, out _, out var error),
                        Is.False);

            Assert.That(error, Does.Contain("certificate"));

        }

        #endregion

        #region AnAlreadyExpiredCertificateIsRefusedAtTheDoor()

        /// <summary>
        /// Everything that can be checked is checked while somebody is looking
        /// at the screen, rather than at whatever hour the switch would have
        /// happened. A certificate that has already run out can never be used,
        /// so it is refused now.
        /// </summary>
        [Test]
        public void AnAlreadyExpiredCertificateIsRefusedAtTheDoor()
        {

            using var ca  = TestCA.Create("Test CA");
            var (_, csr)  = NewKey();

            using var old = ca.Sign(csr, clock.Now.AddYears(-2), clock.Now.AddDays(-1));

            Assert.Multiple(() => {
                Assert.That(store.TryAddCertificate(ca.ChainPEM(old), ReachableAs, out _, out _, out var error), Is.False);
                Assert.That(error, Does.Contain("expired"));
            });

        }

        #endregion

        #region ACertificateForOtherNamesIsTakenInAndSaysSo()

        /// <summary>
        /// Reported rather than refused: a 'reachable as' list that is out of
        /// date looks exactly like a wrong certificate from in here, and only
        /// one of the two is the certificate's fault.
        /// </summary>
        [Test]
        public void ACertificateForOtherNamesIsTakenInAndSaysSo()
        {

            using var ca = TestCA.Create("Test CA");

            Assert.That(store.TryCreateKey("lc001.example.org", [ "somewhere.else.example" ], null, out var id, out var csr, out var keyError),
                        Is.True, keyError);

            using var certificate = ca.Sign(csr!, clock.Now.AddDays(-1), clock.Now.AddYears(1));

            Assert.That(store.TryAddCertificate(ca.ChainPEM(certificate), ReachableAs, out _, out var warnings, out var error),
                        Is.True, error);

            Assert.That(warnings.Any(warning => warning.Contains("lc001.example.org")), Is.True,
                        $"Nothing was said about the names it is not valid for: {String.Join(" | ", warnings)}");

        }

        #endregion

        #region IntermediatesAreKeptAndTheRootIsNot()

        /// <summary>
        /// A client is expected to know roots and not intermediates, so TLS has
        /// the server send everything in between and nothing above it.
        /// </summary>
        [Test]
        public void IntermediatesAreKeptAndTheRootIsNot()
        {

            using var ca = TestCA.Create("Test CA", WithIntermediate: true);

            var (id, csr)         = NewKey();
            using var certificate = ca.Sign(csr, clock.Now.AddDays(-1), clock.Now.AddYears(1));

            // The way an authority actually sends it back: leaf, issuing CA and
            // root, all in one file.
            Assert.That(store.TryAddCertificate(
                            TestCA.ToPEM(certificate, ca.Intermediate!, ca.Certificate),
                            ReachableAs,
                            out _, out _, out var error
                        ), Is.True, error);

            var entry = store.Entries.Single(entry => entry.Id == id);

            Assert.Multiple(() => {
                Assert.That(entry.Intermediates.Count, Is.EqualTo(1),
                            "The root was sent along, or the issuing certificate was dropped.");
                Assert.That(entry.Intermediates[0].Subject, Does.Contain("Issuing CA"));
            });

        }

        #endregion

        #region ACertificateWithoutItsIntermediatesSaysSo()

        [Test]
        public void ACertificateWithoutItsIntermediatesSaysSo()
        {

            using var ca = TestCA.Create("Test CA", WithIntermediate: true);

            var (_, csr)          = NewKey();
            using var certificate = ca.Sign(csr, clock.Now.AddDays(-1), clock.Now.AddYears(1));

            Assert.That(store.TryAddCertificate(TestCA.ToPEM(certificate), ReachableAs, out _, out var warnings, out var error),
                        Is.True, error);

            Assert.That(warnings.Any(warning => warning.Contains("intermediate")), Is.True,
                        $"Nothing was said about the missing intermediates: {String.Join(" | ", warnings)}");

        }

        #endregion


        #region ACertificateThatIsNotValidYetWaitsItsTurn()

        /// <summary>
        /// The whole reason there is more than one: the replacement is uploaded
        /// the day it arrives and takes over by itself the moment it may.
        /// </summary>
        [Test]
        public void ACertificateThatIsNotValidYetWaitsItsTurn()
        {

            using var ca = TestCA.Create("Test CA");

            var current  = Issue(ca, clock.Now.AddYears(-1), clock.Now.AddDays(5));
            var next     = Issue(ca, clock.Now.AddDays(2),   clock.Now.AddYears(1));

            Assert.Multiple(() => {

                Assert.That(store.Select()?.Certificate.Thumbprint,
                            Is.EqualTo(store.Entries.Single(entry => entry.Id == current).Certificate!.Thumbprint),
                            "A certificate that is not valid yet was put into use.");

                Assert.That(store.ServedId, Is.EqualTo(current));

                var waiting = store.Entries.Single(entry => entry.Id == next);

                Assert.That(waiting.ToJSON(clock.Now, false)["certificate"]?.Value<String>("state"),
                            Is.EqualTo("pending"));

            });

        }

        #endregion

        #region TheReplacementTakesOverOnTheDayItBecomesValid()

        [Test]
        public void TheReplacementTakesOverOnTheDayItBecomesValid()
        {

            using var ca = TestCA.Create("Test CA");

            var current  = Issue(ca, clock.Now.AddYears(-1), clock.Now.AddDays(5));
            var next     = Issue(ca, clock.Now.AddDays(2),   clock.Now.AddYears(1));

            store.Select();
            Assert.That(store.ServedId, Is.EqualTo(current), "The wrong one was in use to begin with.");

            clock.Advance(TimeSpan.FromDays(3));

            store.Select();

            Assert.That(store.ServedId, Is.EqualTo(next),
                        "The replacement did not take over on the day it became valid.");

        }

        #endregion

        #region TheNewerOfTwoValidCertificatesIsTheOnePresented()

        /// <summary>
        /// Of the certificates that are in their window, the one that became
        /// valid last - so that uploading a replacement is what changes which
        /// one is used, and not the order the files happen to be read in.
        /// </summary>
        [Test]
        public void TheNewerOfTwoValidCertificatesIsTheOnePresented()
        {

            using var ca = TestCA.Create("Test CA");

            Issue(ca, clock.Now.AddDays(-30), clock.Now.AddDays(30));
            var newer = Issue(ca, clock.Now.AddDays(-1), clock.Now.AddDays(400));

            store.Select();

            Assert.That(store.ServedId, Is.EqualTo(newer));

        }

        #endregion

        #region WhenNothingIsValidTheLastOneKeepsGoingOut()

        /// <summary>
        /// A station that checks certificates will refuse it and say so; one
        /// that does not will go on charging cars. Refusing to serve anything
        /// would take the site down in both cases.
        /// </summary>
        [Test]
        public void WhenNothingIsValidTheLastOneKeepsGoingOut()
        {

            using var ca = TestCA.Create("Test CA");

            var only     = Issue(ca, clock.Now.AddYears(-1), clock.Now.AddDays(2));

            var before   = store.Select();
            Assert.That(before, Is.Not.Null);

            clock.Advance(TimeSpan.FromDays(10));

            var after    = store.Select();

            Assert.Multiple(() => {
                Assert.That(after,             Is.Not.Null, "Nothing at all was presented once the certificate had run out.");
                Assert.That(after!.CacheKey,   Is.EqualTo(before!.CacheKey));
                Assert.That(store.ServedId,    Is.EqualTo(only));
            });

        }

        #endregion

        #region AFreshStartWithNothingValidPresentsTheLastToExpire()

        /// <summary>
        /// The same decision, made by a process that was not running when the
        /// certificate ran out and so has nothing it was already presenting.
        /// </summary>
        [Test]
        public void AFreshStartWithNothingValidPresentsTheLastToExpire()
        {

            using var ca = TestCA.Create("Test CA");

            Issue(ca, clock.Now.AddYears(-2), clock.Now.AddDays(1));
            var later = Issue(ca, clock.Now.AddYears(-2), clock.Now.AddDays(3));

            clock.Advance(TimeSpan.FromDays(10));

            using var restarted = new ServerCertificateStore(directory, clock);

            var chosen = restarted.Select();

            Assert.Multiple(() => {
                Assert.That(chosen,               Is.Not.Null, "A controller that restarted with only expired certificates presented none.");
                Assert.That(restarted.ServedId,   Is.EqualTo(later));
            });

        }

        #endregion

        #region NothingAtAllIsPresentedWhenThereIsNoCertificate()

        [Test]
        public void NothingAtAllIsPresentedWhenThereIsNoCertificate()
        {

            NewKey();

            Assert.Multiple(() => {
                Assert.That(store.Select(),         Is.Null);
                Assert.That(store.HasCertificate,   Is.False);
            });

        }

        #endregion

        #region TheSameAnswerComesBackUntilSomethingChanges()

        /// <summary>
        /// The selector is asked once per accepted connection, so it must not
        /// build a new chain every time: whoever caches a TLS context keys it by
        /// what the chain holds, and a new object for the same certificates
        /// would throw that cache away on every connection.
        /// </summary>
        [Test]
        public void TheSameAnswerComesBackUntilSomethingChanges()
        {

            using var ca = TestCA.Create("Test CA");

            Issue(ca, clock.Now.AddYears(-1), clock.Now.AddYears(1));

            var first   = store.Select();
            var second  = store.Select();

            Assert.Multiple(() => {
                Assert.That(first,   Is.Not.Null);
                Assert.That(second,  Is.SameAs(first));
            });

        }

        #endregion


        #region TheCertificateInUseCannotBeThrownAway()

        /// <summary>
        /// A server without a certificate stops speaking TLS, and doing that by
        /// deleting a file is not a decision anybody means to make.
        /// </summary>
        [Test]
        public void TheCertificateInUseCannotBeThrownAway()
        {

            using var ca = TestCA.Create("Test CA");

            var id = Issue(ca, clock.Now.AddYears(-1), clock.Now.AddYears(1));

            store.Select();

            Assert.Multiple(() => {
                Assert.That(store.TryRemove(id, out var error), Is.False);
                Assert.That(error,                              Does.Contain("presenting right now"));
                Assert.That(File.Exists(Path.Combine(directory, $"{id}.key.pem")), Is.True);
            });

        }

        #endregion

        #region AKeyThatIsNotInUseGoesAwayEntirely()

        [Test]
        public void AKeyThatIsNotInUseGoesAwayEntirely()
        {

            var (id, _) = NewKey();

            Assert.That(store.TryRemove(id, out var error), Is.True, error);

            Assert.Multiple(() => {

                Assert.That(store.Entries.Any(entry => entry.Id == id), Is.False);

                foreach (var extension in new[] { "key.pem", "csr.pem", "json" })
                    Assert.That(File.Exists(Path.Combine(directory, $"{id}.{extension}")), Is.False,
                                $"'{id}.{extension}' was left behind.");

                Assert.That(Directory.GetFiles(directory, $"{id}.*"), Is.Empty, "what was left behind, set aside or not");

            });

        }

        #endregion

        #region AKeyOneOfWhoseFilesCannotBeTakenAwayStaysWhole(Extension)

        /// <summary>
        /// A key one of whose files cannot be taken away - held open by
        /// somebody, as Windows keeps a file then - stays, whole, now and when
        /// the store is read again: its files are set aside before any of them
        /// is deleted, and put back where one cannot be. Deleted one after the
        /// other, a file held open left the ones before it gone, and the key
        /// listed until the next start and gone after it (found by the charging
        /// station).
        /// </summary>
        [TestCase("key.pem")]
        [TestCase("csr.pem")]
        [TestCase("json")]
        public void AKeyOneOfWhoseFilesCannotBeTakenAwayStaysWhole(String Extension)
        {

            var (id, _)   = NewKey();
            var before    = FilesOf(id);

            store.BeforeRemoving = path => {
                if (path.EndsWith($".{Extension}", StringComparison.Ordinal))
                    throw new IOException("The process cannot access the file because it is being used by another process.");
            };

            var removed   = store.TryRemove(id, out var error, out var notSaved);

            store.BeforeRemoving = null;

            var after     = FilesOf(id);

            store.Reload();

            Assert.Multiple(() => {

                Assert.That(removed,   Is.False);
                Assert.That(notSaved,  Is.True);
                Assert.That(error,     Does.StartWith($"'{id}' could not be removed from '{store.Path}': "));

                Assert.That(after,     Is.EqualTo(before),  "its files");
                Assert.That(store.Entries.Select(entry => entry.Id), Does.Contain(id), "the key, read again");

            });

        }

        /// <summary>
        /// The files of a key, by name, with what is in them.
        /// </summary>
        private (String Name, String Content)[] FilesOf(String Id)

            => Directory.GetFiles(directory, $"{Id}.*").
                         Order(StringComparer.Ordinal).
                         Select(file => (Path.GetFileName(file), File.ReadAllText(file))).
                         ToArray();

        #endregion

        #region AFileOfAKeyThatCannotBePutBackIsSaidAndLeftOver()

        /// <summary>
        /// A file of a key that cannot be moved back, once another could not be
        /// set aside, is said in the log and left over under its "*.removed"
        /// name, which the next start does not read; the key is refused all the
        /// same, and the files that could be moved back are.
        /// </summary>
        [Test]
        public void AFileOfAKeyThatCannotBePutBackIsSaidAndLeftOver()
        {

            var (id, _)   = NewKey();
            var said      = new List<String>();

            store.OnNotice += (level, message) => said.Add($"{level}: {message}");

            store.BeforeRemoving = path => {
                if (path.EndsWith(".json", StringComparison.Ordinal) || path.EndsWith(".key.pem.removed", StringComparison.Ordinal))
                    throw new IOException("The process cannot access the file because it is being used by another process.");
            };

            var removed   = store.TryRemove(id, out var error, out var notSaved);

            store.BeforeRemoving = null;

            var leftOver  = Path.Combine(directory, $"{id}.key.pem.removed");

            Assert.Multiple(() => {
                Assert.That(removed,   Is.False);
                Assert.That(notSaved,  Is.True);
                Assert.That(said,      Has.One.EqualTo($"Warning: '{leftOver}' could not be put back, and is left over."));
                Assert.That(FilesOf(id).Select(file => file.Name),
                            Is.EqualTo(new[] { $"{id}.csr.pem", $"{id}.json", $"{id}.key.pem.removed" }),
                            "the request put back, the description never moved, the key left over");
            });

        }

        #endregion

        #region AFileOfAKeyThatCannotBeDeletedOnceAsideIsSaidAndLeftOver()

        /// <summary>
        /// A file of a key that cannot be deleted once all of them are set aside
        /// is said in the log and left over under its "*.removed" name, which
        /// the next start does not read: the key is gone, now and then.
        /// </summary>
        [Test]
        public void AFileOfAKeyThatCannotBeDeletedOnceAsideIsSaidAndLeftOver()
        {

            var (id, _)   = NewKey();
            var said      = new List<String>();

            store.OnNotice += (level, message) => said.Add($"{level}: {message}");

            store.BeforeRemoving = path => {
                if (path.EndsWith(".csr.pem.removed", StringComparison.Ordinal))
                    throw new IOException("The process cannot access the file because it is being used by another process.");
            };

            var removed   = store.TryRemove(id, out var error, out _);

            store.BeforeRemoving = null;

            var leftOver  = Path.Combine(directory, $"{id}.csr.pem.removed");

            store.Reload();

            Assert.Multiple(() => {
                Assert.That(removed,   Is.True, error);
                Assert.That(said,      Has.One.EqualTo($"Warning: '{leftOver}' could not be deleted, and is left over."));
                Assert.That(FilesOf(id).Select(file => file.Name), Is.EqualTo(new[] { $"{id}.csr.pem.removed" }), "what is left over");
                Assert.That(store.Entries.Select(entry => entry.Id), Does.Not.Contain(id), "the key, read again");
            });

        }

        #endregion

        #region AKeyWhoseFilesCannotBeWrittenLeavesNothingBehind(Extension, Beginning)

        /// <summary>
        /// A key is three files - itself, its signing request and what is said
        /// of it - and one that could not be written in full was left behind.
        /// At the next start it was a key nobody had asked for, made "now", of
        /// the default algorithm and for no subject, or one that could not be
        /// read and said so at every start. Whichever of the three the disk
        /// runs out in, nothing of the key stays, now or when the store is read
        /// again.
        /// </summary>
        [TestCase("key.pem",  "-----BEGIN PRIVATE KEY-----")]
        [TestCase("csr.pem",  "-----BEGIN CERTIFICATE REQUEST-----")]
        [TestCase("json",     "{")]
        public void AKeyWhoseFilesCannotBeWrittenLeavesNothingBehind(String Extension, String Beginning)
        {

            CutOff($".{Extension}", Beginning);

            var made        = store.TryCreateKey("lc001.example.org", ReachableAs, null, out _, out _, out var error, out var notSaved);

            store.BeforeWriting = null;

            var keys        = store.Entries.Select(entry => entry.Id).ToArray();
            var left        = Directory.GetFiles(directory).Select(Path.GetFileName).ToArray();

            var said        = new List<String>();
            store.OnNotice += (level, message) => said.Add($"{level}: {message}");

            store.Reload();

            Assert.Multiple(() => {

                Assert.That(made,      Is.False);
                Assert.That(notSaved,  Is.True);
                Assert.That(error,     Does.StartWith($"The key could not be written to '{store.Path}': "));

                Assert.That(keys,      Is.Empty,  "the keys");
                Assert.That(left,      Is.Empty,  "what was left behind");

                Assert.That(store.Entries.Select(entry => entry.Id), Is.Empty, "the keys, read again");
                Assert.That(said,      Is.Empty,  "what reading them again said");

            });

        }

        #endregion

        #region EverythingIsStillThereAfterARestart()

        /// <summary>
        /// Read back the way it will be read at the next start, and not from
        /// what was still in hand when it was written.
        /// </summary>
        [Test]
        public void EverythingIsStillThereAfterARestart()
        {

            using var ca = TestCA.Create("Test CA", WithIntermediate: true);

            var id = Issue(ca, clock.Now.AddYears(-1), clock.Now.AddYears(1));

            using var restarted = new ServerCertificateStore(directory, clock);

            var entry = restarted.Entries.SingleOrDefault(entry => entry.Id == id);

            Assert.Multiple(() => {
                Assert.That(entry,                        Is.Not.Null);
                Assert.That(entry!.Certificate,           Is.Not.Null);
                Assert.That(entry.Certificate!.HasPrivateKey, Is.True,
                            "The certificate came back without the private key, so it cannot be used for TLS.");
                Assert.That(entry.Intermediates.Count,    Is.EqualTo(1));
                Assert.That(entry.CreatedAt,              Is.EqualTo(clock.Now));
            });

        }

        #endregion

        #region ARenewalTakesThePlaceOfTheCertificateBeforeIt()

        /// <summary>
        /// A certificate for a key that has one already - its renewal - takes
        /// the place of the one before it, now and when the store is read
        /// again, and nothing of how it was written is left beside it.
        /// </summary>
        [Test]
        public void ARenewalTakesThePlaceOfTheCertificateBeforeIt()
        {

            using var ca       = TestCA.Create("Test CA");

            var id             = Issue(ca, clock.Now.AddYears(-1), clock.Now.AddYears(1));

            Assert.That(store.TryReadCSR(id, out var csr, out var csrError), Is.True, csrError);

            using var renewal  = ca.Sign(csr!, clock.Now, clock.Now.AddYears(2));

            Assert.That(store.TryAddCertificate(ca.ChainPEM(renewal), ReachableAs, out var taken, out _, out var error),
                        Is.True, error);

            var inEffect       = store.Entries.Single().Certificate?.Thumbprint;
            var files          = Directory.GetFiles(directory).Select(Path.GetFileName).Order(StringComparer.Ordinal).ToArray();

            store.Reload();

            Assert.Multiple(() => {

                Assert.That(taken,     Is.EqualTo(id));
                Assert.That(inEffect,  Is.EqualTo(renewal.Thumbprint),  "the certificate in effect");
                Assert.That(files,     Is.EqualTo(new[] { $"{id}.cert.pem", $"{id}.csr.pem", $"{id}.json", $"{id}.key.pem" }),
                                       "the files of the key");

                Assert.That(store.Entries.SingleOrDefault(entry => entry.Id == id)?.Certificate?.Thumbprint,
                            Is.EqualTo(renewal.Thumbprint),
                            "the certificate, read again");

            });

        }

        #endregion

        #region ARenewalThatCannotBeWrittenKeepsTheCertificateBeforeIt()

        /// <summary>
        /// A renewal the disk runs out in halfway through is not taken in, and
        /// the certificate before it is still the key's, now and when the store
        /// is read again. It was cut off: the key went on presenting it until
        /// the next start, and was not read at all then.
        /// </summary>
        [Test]
        public void ARenewalThatCannotBeWrittenKeepsTheCertificateBeforeIt()
        {

            using var ca       = TestCA.Create("Test CA");

            var id             = Issue(ca, clock.Now.AddYears(-1), clock.Now.AddYears(1));
            var before         = store.Entries.Single().Certificate!.Thumbprint;

            Assert.That(store.TryReadCSR(id, out var csr, out var csrError), Is.True, csrError);

            using var renewal  = ca.Sign(csr!, clock.Now, clock.Now.AddYears(2));
            var pem            = ca.ChainPEM(renewal);

            // Its file, under whatever name it is written first.
            CutOff($"{id}.cert.pem", pem[..(pem.Length / 2)]);

            var taken          = store.TryAddCertificate(pem, ReachableAs, out _, out _, out var error, out var notSaved);

            store.BeforeWriting = null;

            var inEffect       = store.Entries.Single().Certificate?.Thumbprint;
            var files          = Directory.GetFiles(directory).Select(Path.GetFileName).Order(StringComparer.Ordinal).ToArray();

            var said           = new List<String>();
            store.OnNotice    += (level, message) => said.Add($"{level}: {message}");

            store.Reload();

            Assert.Multiple(() => {

                Assert.That(taken,     Is.False);
                Assert.That(notSaved,  Is.True);
                Assert.That(error,     Does.StartWith($"The certificate could not be written to '{store.Path}': "));

                Assert.That(inEffect,  Is.EqualTo(before),  "the certificate in effect");
                Assert.That(files,     Is.EqualTo(new[] { $"{id}.cert.pem", $"{id}.csr.pem", $"{id}.json", $"{id}.key.pem" }),
                                       "the files of the key");

                Assert.That(store.Entries.SingleOrDefault(entry => entry.Id == id)?.Certificate?.Thumbprint,
                            Is.EqualTo(before),
                            $"the certificate, read again: {String.Join(" | ", said)}");

            });

        }

        #endregion

        #region AnUnreadableKeyIsSkippedAndTheOthersStillWork()

        /// <summary>
        /// One broken file must not stop a controller that has three others -
        /// and the one keeping a car park running is frequently one of the
        /// three.
        /// </summary>
        [Test]
        public void AnUnreadableKeyIsSkippedAndTheOthersStillWork()
        {

            using var ca = TestCA.Create("Test CA");

            var good = Issue(ca, clock.Now.AddYears(-1), clock.Now.AddYears(1));

            File.WriteAllText(Path.Combine(directory, "broken.key.pem"), "-----BEGIN PRIVATE KEY-----\nnope\n-----END PRIVATE KEY-----\n");

            var said = new List<String>();

            using var restarted = new ServerCertificateStore(directory, clock);
            restarted.OnNotice += (level, message) => said.Add(message);
            restarted.Reload();

            Assert.Multiple(() => {
                Assert.That(restarted.Select()?.Certificate.Thumbprint, Is.Not.Null);
                Assert.That(restarted.ServedId,                         Is.EqualTo(good));
                Assert.That(said.Any(message => message.Contains("broken")), Is.True,
                            "The unreadable key was skipped without a word.");
            });

        }

        #endregion

        #region AnExpiryIsAnnouncedWhileThereIsStillTimeToActOnIt()

        [Test]
        public void AnExpiryIsAnnouncedWhileThereIsStillTimeToActOnIt()
        {

            using var ca = TestCA.Create("Test CA");

            Issue(ca, clock.Now.AddYears(-1), clock.Now.AddDays(10));

            var said = new List<String>();
            store.OnNotice += (level, message) => said.Add(message);

            store.CheckExpiry(ReachableAs);

            Assert.That(said.Any(message => message.Contains("runs out in 9 day(s)") ||
                                            message.Contains("runs out in 10 day(s)")),
                        Is.True,
                        $"Nothing was said about a certificate ten days from running out: {String.Join(" | ", said)}");

        }

        #endregion

        #region NothingValidAtAllIsTheLoudestThingThereIs()

        /// <summary>
        /// The state where every charging station that checks certificates is
        /// being turned away. A check that walked past it in silence would be
        /// quiet in exactly the one case that needs a voice.
        /// </summary>
        [Test]
        public void NothingValidAtAllIsTheLoudestThingThereIs()
        {

            using var ca = TestCA.Create("Test CA");

            Issue(ca, clock.Now.AddYears(-1), clock.Now.AddDays(1));

            clock.Advance(TimeSpan.FromDays(10));

            var said = new List<(LogLevel Level, String Message)>();
            store.OnNotice += (level, message) => said.Add((level, message));

            store.CheckExpiry(ReachableAs);

            Assert.That(said.Any(entry => entry.Level == LogLevel.Critical &&
                                          entry.Message.Contains("No server certificate")),
                        Is.True,
                        $"A controller with nothing valid said: {String.Join(" | ", said.Select(entry => $"{entry.Level}: {entry.Message}"))}");

        }

        #endregion

        #region NothingIsAnnouncedWhenTheReplacementIsAlreadyHere()

        /// <summary>
        /// A certificate expiring next week with its replacement already
        /// uploaded is what a planned replacement looks like. Warning about it
        /// is how a warning gets ignored.
        /// </summary>
        [Test]
        public void NothingIsAnnouncedWhenTheReplacementIsAlreadyHere()
        {

            using var ca = TestCA.Create("Test CA");

            Issue(ca, clock.Now.AddYears(-1), clock.Now.AddDays(10));
            Issue(ca, clock.Now.AddDays(5),   clock.Now.AddYears(1));

            var said = new List<String>();
            store.OnNotice += (level, message) => said.Add(message);

            store.CheckExpiry(ReachableAs);

            Assert.That(said.Any(message => message.Contains("runs out")), Is.False,
                        $"A planned replacement was warned about anyway: {String.Join(" | ", said)}");

        }

        #endregion

    }

}
