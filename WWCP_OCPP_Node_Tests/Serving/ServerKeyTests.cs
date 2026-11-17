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

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Hermod;


using cloud.charging.open.protocols.OCPP.Node.Serving;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Serving
{

    /// <summary>
    /// The server key charging stations pin: made without a certificate
    /// authority and presented in a self-signed carrier certificate, the same
    /// key pin in every wrapper, a next key announced ahead of a rollover and
    /// presented only once somebody switches to it - and all of it read back
    /// the way the next start reads it.
    /// </summary>
    /// <remarks>
    /// No certificate store is touched, except by the one test that has an
    /// authority issue a certificate for the key: a key pin needs none.
    /// </remarks>
    public class ServerKeyTests
    {

        #region Data

        private String                  directory  = default!;
        private TestClock               clock      = default!;
        private ServerCertificateStore  store      = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeAStore()
        {
            directory = ServingTestDirectories.TemporaryDirectory("serverkeys");
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

        #region (private) ServerKey(AsNext = false) / KeyPinOfTheKeyFile(Id) / CSRFor(Id)

        /// <summary>
        /// A server key, made as asked.
        /// </summary>
        private (String Id, KeyPin Pin) ServerKey(Boolean AsNext = false)
        {

            Assert.That(store.TryCreateServerKey(null, AsNext, out var id, out var pin, out var error),
                        Is.True, error);

            return (id!, pin!);

        }

        /// <summary>
        /// The key pin of the private key on disk, worked out by .NET from the
        /// key file alone - not by the code that is being tested.
        /// </summary>
        private KeyPin KeyPinOfTheKeyFile(String Id)
        {

            using var key = ECDsa.Create();

            key.ImportFromPem(File.ReadAllText(Path.Combine(directory, $"{Id}.key.pem")));

            return KeyPin.FromSHA256(SHA256.HashData(key.ExportSubjectPublicKeyInfo()));

        }

        /// <summary>
        /// A signing request for the key on disk, as an operator would make one
        /// with openssl - the store makes none for a server key.
        /// </summary>
        private String CSRFor(String Id)
        {

            using var key = ECDsa.Create();

            key.ImportFromPem(File.ReadAllText(Path.Combine(directory, $"{Id}.key.pem")));

            var request = new CertificateRequest("CN=lc001.example.org", key, HashAlgorithmName.SHA256);

            request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension([ new Oid("1.3.6.1.5.5.7.3.1") ], false));

            return request.CreateSigningRequestPem();

        }

        #endregion


        #region AServerKeyIsPresentedInItsCarrier()

        /// <summary>
        /// A server key made without a signing request is presented at once, in
        /// a carrier certificate made from it - one that claims nothing but the
        /// key: no authority, no keyCertSign, no name anybody dials, and dates
        /// nobody's clock falls outside.
        /// </summary>
        [Test]
        public void AServerKeyIsPresentedInItsCarrier()
        {

            var (id, pin) = ServerKey();

            var chain     = store.Select();
            var carrier   = chain?.Certificate;

            Assert.That(carrier, Is.Not.Null, "nothing is presented");

            var basic     = carrier!.Extensions.OfType<X509BasicConstraintsExtension>().SingleOrDefault();
            var usage     = carrier.Extensions.OfType<X509KeyUsageExtension>().SingleOrDefault();
            var purposes  = carrier.Extensions.OfType<X509EnhancedKeyUsageExtension>().SingleOrDefault();

            Assert.Multiple(() => {

                Assert.That(store.HasCertificate,                       Is.True);
                Assert.That(store.CurrentKey?.Id,                       Is.EqualTo(id));
                Assert.That(chain!.Intermediates.Count,                 Is.Zero);
                Assert.That(carrier.HasPrivateKey,                      Is.True, "the carrier came without its key, so it cannot be presented");

                Assert.That(KeyPin.FromCertificate(carrier),            Is.EqualTo(pin),                 "the carrier's key pin is not the key's");
                Assert.That(KeyPinOfTheKeyFile(id),                     Is.EqualTo(pin),                 "the key pin is not the key file's");
                Assert.That(store.CurrentKey!.KeyPin,                   Is.EqualTo(pin));

                Assert.That(carrier.Subject,                            Does.Contain("Key pin carrier").And.Contain(id));
                Assert.That(carrier.Issuer,                             Is.EqualTo(carrier.Subject),     "a carrier signs itself");

                Assert.That(basic,                                      Is.Not.Null,                     "no basicConstraints");
                Assert.That(basic!.CertificateAuthority,                Is.False,                        "a carrier must not be an authority");
                Assert.That(basic.Critical,                             Is.True);

                Assert.That(usage,                                      Is.Not.Null,                     "no keyUsage");
                Assert.That(usage!.KeyUsages,                           Is.EqualTo(X509KeyUsageFlags.DigitalSignature));
                Assert.That(usage.KeyUsages & X509KeyUsageFlags.KeyCertSign, Is.EqualTo(X509KeyUsageFlags.None), "a carrier must not sign certificates");
                Assert.That(usage.Critical,                             Is.True);

                Assert.That(purposes?.EnhancedKeyUsages.Cast<Oid>().Select(oid => oid.Value),
                                                                        Is.EqualTo(new[] { "1.3.6.1.5.5.7.3.1" }), "a carrier is for a TLS server and nothing else");

                Assert.That(carrier.Extensions.OfType<X509SubjectAlternativeNameExtension>(), Is.Empty, "a carrier names nothing to dial");

                // Read as encoded: .NET hands NotAfter back in local time, and
                // the last second of 9999 is past what that can hold east of
                // Greenwich.
                var encoded = new Org.BouncyCastle.X509.X509CertificateParser().ReadCertificate(carrier.RawData);

                Assert.That(new DateTimeOffset(encoded.NotBefore, TimeSpan.Zero), Is.EqualTo(ServerCertificateStore.CarrierNotBefore));
                Assert.That(new DateTimeOffset(encoded.NotAfter,  TimeSpan.Zero), Is.EqualTo(ServerCertificateStore.CarrierNotAfter));

                Assert.That(File.Exists(Path.Combine(directory, $"{id}.carrier.pem")), Is.True);
                Assert.That(File.Exists(Path.Combine(directory, $"{id}.csr.pem")),     Is.False, "a server key has no signing request");

            });

        }

        #endregion

        #region AServerKeyIsTheSameAfterARestart()

        /// <summary>
        /// Read back as the next start reads it: the same key, the same carrier,
        /// the same pin, still the current one.
        /// </summary>
        [Test]
        public void AServerKeyIsTheSameAfterARestart()
        {

            var (id, pin)  = ServerKey();
            var before     = store.Select()!.Certificate.Thumbprint;

            using var restarted = new ServerCertificateStore(directory, clock);

            var entry      = restarted.Entries.SingleOrDefault(one => one.Id == id);
            var presented  = restarted.Select()?.Certificate;

            Assert.Multiple(() => {
                Assert.That(entry,                              Is.Not.Null);
                Assert.That(entry!.KeyPin,                      Is.EqualTo(pin));
                Assert.That(entry.HasCarrier,                   Is.True);
                Assert.That(entry.IsNext,                       Is.False);
                Assert.That(entry.CreatedAt,                    Is.EqualTo(clock.Now));
                Assert.That(entry.CurrentSince,                 Is.EqualTo(clock.Now));
                Assert.That(restarted.CurrentKey?.Id,           Is.EqualTo(id));
                Assert.That(presented?.Thumbprint,              Is.EqualTo(before), "another carrier came back");
                Assert.That(presented!.HasPrivateKey,           Is.True);
                Assert.That(KeyPin.FromCertificate(presented),  Is.EqualTo(pin));
            });

        }

        #endregion

        #region ACertificateIssuedForTheKeyLaterKeepsItsPin()

        /// <summary>
        /// The same key in two wrappers: a certificate an authority issued for
        /// the server key later is presented while it is valid - for the
        /// stations that check chains - and the carrier again once it runs out.
        /// The carrier is not lost to it, now or after a restart, and the key pin
        /// is the same throughout.
        /// </summary>
        [Test]
        public void ACertificateIssuedForTheKeyLaterKeepsItsPin()
        {

            var (id, pin) = ServerKey();

            using var ca          = TestCA.Create($"Test CA {Guid.NewGuid()}");
            using var certificate = ca.Sign(CSRFor(id), clock.Now.AddDays(-1), clock.Now.AddDays(30));

            Assert.That(store.TryAddCertificate(ca.ChainPEM(certificate), [], out var taken, out _, out var error),
                        Is.True, error);

            var whileValid = store.Select()?.Certificate;

            clock.Advance(TimeSpan.FromDays(60));

            var afterwards = store.Select()?.Certificate;

            using var restarted   = new ServerCertificateStore(directory, clock);
            var       restartedIs = restarted.Select()?.Certificate;

            Assert.Multiple(() => {

                Assert.That(taken,                                 Is.EqualTo(id), "the certificate was not taken in for the server key");

                Assert.That(whileValid?.Thumbprint,                Is.EqualTo(certificate.Thumbprint), "the authority's certificate is not presented while it is valid");
                Assert.That(KeyPin.FromCertificate(whileValid!),   Is.EqualTo(pin));

                Assert.That(afterwards?.Subject,                   Does.Contain("Key pin carrier"),    "the carrier did not take over when the certificate ran out");
                Assert.That(KeyPin.FromCertificate(afterwards!),   Is.EqualTo(pin));

                Assert.That(restartedIs?.Subject,                  Does.Contain("Key pin carrier"));
                Assert.That(restarted.Entries.Single().Certificate?.Thumbprint, Is.EqualTo(certificate.Thumbprint), "the certificate was lost at the restart");
                Assert.That(restarted.Entries.Single().HasCarrier, Is.True, "the carrier was lost to the certificate");
                Assert.That(restarted.CurrentKey?.KeyPin,          Is.EqualTo(pin));

            });

        }

        #endregion

        #region ANextKeyIsPresentedOnlyAfterTheSwitch()

        /// <summary>
        /// The rollover: a next key is announced and its pin shown, while the
        /// current key goes on being presented; the switch makes it the current
        /// one, keeps when each was made, and leaves the key before it in place,
        /// presented no more, until somebody removes it.
        /// </summary>
        [Test]
        public void ANextKeyIsPresentedOnlyAfterTheSwitch()
        {

            var (currentId, currentPin)  = ServerKey();
            var madeFirst                = clock.Now;

            clock.Advance(TimeSpan.FromDays(300));

            var (nextId, nextPin)        = ServerKey(AsNext: true);
            var madeNext                 = clock.Now;

            var beforeTheSwitch          = store.Select()?.Certificate;
            var announced                = store.NextKey;
            var json                     = store.ToJSON();

            clock.Advance(TimeSpan.FromDays(14));

            Assert.That(store.TrySwitchToNextKey(out var switchedTo, out var error), Is.True, error);

            var afterTheSwitch           = store.Select()?.Certificate;

            Assert.Multiple(() => {

                Assert.That(KeyPin.FromCertificate(beforeTheSwitch!),  Is.EqualTo(currentPin), "the next key was presented before the switch");
                Assert.That(announced?.Id,                             Is.EqualTo(nextId));
                Assert.That(json["keyPins"]?["current"]?.Value<String>("keyPin"), Is.EqualTo(currentPin.ToHex()));
                Assert.That(json["keyPins"]?["next"]?.   Value<String>("keyPin"), Is.EqualTo(nextPin.ToHex()));

                Assert.That(switchedTo,                                Is.EqualTo(nextId));
                Assert.That(KeyPin.FromCertificate(afterTheSwitch!),   Is.EqualTo(nextPin),    "the switch did not change what is presented");
                Assert.That(store.CurrentKey?.Id,                      Is.EqualTo(nextId));
                Assert.That(store.NextKey,                             Is.Null);

                Assert.That(store.Entries.Single(entry => entry.Id == currentId).CreatedAt,  Is.EqualTo(madeFirst));
                Assert.That(store.Entries.Single(entry => entry.Id == nextId).   CreatedAt,  Is.EqualTo(madeNext));
                Assert.That(store.Entries.Single(entry => entry.Id == nextId).   CurrentSince, Is.EqualTo(clock.Now));

            });

            // The key before: kept, presented no more, and removable now; the
            // current one is not.
            Assert.Multiple(() => {
                Assert.That(store.TryRemove(nextId,    out var refused), Is.False, "the current server key was thrown away");
                Assert.That(refused,                                     Does.Contain(nextId));
                Assert.That(store.TryRemove(currentId, out var removed), Is.True,  removed);
            });

        }

        #endregion

        #region TheNextKeyAndTheSwitchAreTheSameAfterARestart()

        [Test]
        public void TheNextKeyAndTheSwitchAreTheSameAfterARestart()
        {

            var (currentId, _)   = ServerKey();
            clock.Advance(TimeSpan.FromDays(1));
            var (nextId, nextPin) = ServerKey(AsNext: true);

            using (var restarted = new ServerCertificateStore(directory, clock))
            {
                Assert.Multiple(() => {
                    Assert.That(restarted.NextKey?.Id,     Is.EqualTo(nextId),    "the next key was not announced any more after a restart");
                    Assert.That(restarted.CurrentKey?.Id,  Is.EqualTo(currentId), "the next key took over at a restart");
                });
            }

            clock.Advance(TimeSpan.FromDays(1));

            Assert.That(store.TrySwitchToNextKey(out _, out var error), Is.True, error);

            using var afterTheSwitch = new ServerCertificateStore(directory, clock);

            Assert.Multiple(() => {
                Assert.That(afterTheSwitch.CurrentKey?.Id,                                   Is.EqualTo(nextId), "the switch was undone by a restart");
                Assert.That(afterTheSwitch.NextKey,                                          Is.Null);
                Assert.That(KeyPin.FromCertificate(afterTheSwitch.Select()!.Certificate),    Is.EqualTo(nextPin));
            });

        }

        #endregion

        #region AServerKeyNeverTakesThePlaceOfWhatIsPresented()

        /// <summary>
        /// Where something is presented already - a server key, or a certificate
        /// an authority issued for a key of a signing request - a second server
        /// key does not take its place: it has to be announced as the next one.
        /// And where nothing is presented, there is nothing to announce a next
        /// key after.
        /// </summary>
        [Test]
        public void AServerKeyNeverTakesThePlaceOfWhatIsPresented()
        {

            var nextWithoutAKey = store.TryCreateServerKey(null, AsNext: true, out _, out _, out var nothingPresented);

            var (id, _)         = ServerKey();

            var secondCurrent   = store.TryCreateServerKey(null, AsNext: false, out _, out _, out var presentedAlready);

            ServerKey(AsNext: true);

            var secondNext      = store.TryCreateServerKey(null, AsNext: true, out _, out _, out var announcedAlready);

            Assert.Multiple(() => {
                Assert.That(nextWithoutAKey,                   Is.False);
                Assert.That(nothingPresented,                  Does.Contain("Nothing is presented"));
                Assert.That(secondCurrent,                     Is.False);
                Assert.That(presentedAlready,                  Does.Contain("Announce the new key as the next one"));
                Assert.That(secondNext,                        Is.False);
                Assert.That(announcedAlready,                  Does.Contain("announced as the next key already"));
                Assert.That(store.CurrentKey?.Id,              Is.EqualTo(id));
                Assert.That(store.Entries.Count,               Is.EqualTo(2));
            });

        }

        [Test]
        public void ACertificateOfASigningRequestIsNotReplacedByAServerKey()
        {

            using var ca          = TestCA.Create($"Test CA {Guid.NewGuid()}");

            Assert.That(store.TryCreateKey("lc001.example.org", [ "lc001.example.org" ], null, out var id, out var csr, out var keyError),
                        Is.True, keyError);

            using var certificate = ca.Sign(csr!, clock.Now.AddDays(-1), clock.Now.AddYears(1));

            Assert.That(store.TryAddCertificate(ca.ChainPEM(certificate), [ "lc001.example.org" ], out _, out _, out var addError),
                        Is.True, addError);

            var created = store.TryCreateServerKey(null, AsNext: false, out _, out _, out var refused);

            Assert.Multiple(() => {
                Assert.That(created,                                   Is.False);
                Assert.That(refused,                                   Does.Contain(id));
                Assert.That(store.Select()?.Certificate.Thumbprint,    Is.EqualTo(certificate.Thumbprint));
                Assert.That(store.CurrentKey?.KeyPin,                  Is.EqualTo(KeyPin.FromCertificate(certificate)),
                            "the key pin to hand out is not the one of the key presented");
            });

        }

        #endregion

        #region ASwitchWithoutANextKeyIsRefused()

        [Test]
        public void ASwitchWithoutANextKeyIsRefused()
        {

            var (id, _) = ServerKey();

            Assert.Multiple(() => {
                Assert.That(store.TrySwitchToNextKey(out _, out var error), Is.False);
                Assert.That(error,                                          Does.Contain("No next key"));
                Assert.That(store.CurrentKey?.Id,                           Is.EqualTo(id));
            });

        }

        #endregion

        #region TheCurrentServerKeyCannotBeThrownAway()

        /// <summary>
        /// Before anything was presented as well: the current server key is
        /// what the next handshake presents.
        /// </summary>
        [Test]
        public void TheCurrentServerKeyCannotBeThrownAway()
        {

            var (id, _) = ServerKey();

            Assert.Multiple(() => {
                Assert.That(store.ServedId,                     Is.Null, "something was presented already");
                Assert.That(store.TryRemove(id, out var error), Is.False);
                Assert.That(error,                              Does.Contain("current server key"));
                Assert.That(store.Entries.Count,                Is.EqualTo(1));
            });

        }

        #endregion

        #region TheKeyPinsAreInWhatThePageReads()

        /// <summary>
        /// Every key with its key pin, as 64 lower-case hexadecimal digits, when
        /// it was made, whether it is the next one, and which is presented in
        /// what.
        /// </summary>
        [Test]
        public void TheKeyPinsAreInWhatThePageReads()
        {

            var (id, pin)          = ServerKey();
            var (nextId, nextPin)  = ServerKey(AsNext: true);

            var json     = store.ToJSON();
            var entries  = json["entries"]!.Children<JObject>().ToDictionary(entry => entry.Value<String>("id")!);

            Assert.Multiple(() => {

                Assert.That(json["keyPins"]?["current"]?.Value<String>("id"),         Is.EqualTo(id));
                Assert.That(json["keyPins"]?["current"]?.Value<String>("keyPin"),     Does.Match("^[0-9a-f]{64}$"));
                Assert.That(json["keyPins"]?["current"]?.Value<String>("keyPin"),     Is.EqualTo(pin.ToHex()));
                Assert.That(json["keyPins"]?["current"]?.Value<String>("presented"),  Is.EqualTo(ServerCertificateStore.PresentedAsCarrier));
                Assert.That(json["keyPins"]?["current"]?.Value<String>("createdAt"),  Is.Not.Null);
                Assert.That(json["keyPins"]?["next"]?.   Value<String>("keyPin"),     Is.EqualTo(nextPin.ToHex()));

                Assert.That(entries[id].    Value<String>("keyPin"),                  Is.EqualTo(pin.ToHex()));
                Assert.That(entries[id].    Value<Boolean>("hasCarrier"),             Is.True);
                Assert.That(entries[id].    Value<Boolean>("next"),                   Is.False);
                Assert.That(entries[id].    Value<String>("presented"),               Is.EqualTo(ServerCertificateStore.PresentedAsCarrier));
                Assert.That(entries[nextId].Value<Boolean>("next"),                   Is.True);
                Assert.That(entries[nextId].Value<String>("presented"),               Is.Null);

            });

        }

        #endregion

    }

}
