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
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using cloud.charging.open.protocols.OCPP.Node.KeyPins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests
{

    /// <summary>
    /// The key-pin core of a node as a TLS client: what stands at the gate while the node says what
    /// happened - in the log, and in a security event. Whoever is told may take its time; a request of
    /// the server meanwhile meets the gate as it is after the change, not before.
    /// </summary>
    /// <remarks>
    /// No network: a connection that trusts on first use, kept in memory; a handshake made by asking the
    /// trust's validator about a certificate; the gate asked from within the delegates, which is where a
    /// request would meet it while a delegate is held.
    /// </remarks>
    [TestFixture]
    public class KeyPinCoreTests
    {

        #region (class) AConnection / Endpoints

        private sealed record AConnection(String                 Id,
                                          String                 Description,
                                          URL                    URL,
                                          IReadOnlyList<KeyPin>  ServerKeyPins,
                                          UInt64                 ServerKeyPinsVersion,
                                          Boolean                ServerKeyPinsConfirmed,
                                          KeyPin?                ServerKeyPinLearned,
                                          DateTimeOffset?        ServerKeyPinLearnedAt) : IPinnedTLSEndpoint
        {

            public Boolean             IsSecure                   => true;
            public Boolean             IsKeyPinned                => ServerKeyPins.Count > 0 || TrustOnFirstUse == KeyTrustOnFirstUse.Key;
            public RawPublicKeyMode    RawPublicKeys              => RawPublicKeyMode.Off;
            public KeyTrustOnFirstUse  TrustOnFirstUse            { get; init; } = KeyTrustOnFirstUse.Key;
            public TimeSpan            ServerKeyPinProbationTime  { get; init; } = TimeSpan.FromMinutes(10);

            public KeyPinState         ServerKeyPinState

                => ServerKeyPins.Count == 0 && ServerKeyPinLearned is not null
                       ? KeyPinState.Learned
                       : ServerKeyPins.Count > 0 && ServerKeyPinsConfirmed
                             ? KeyPinState.Confirmed
                             : KeyPinState.Configured;

        }

        private sealed class Endpoints(AConnection First) : IPinnedTLSEndpoints
        {

            private AConnection connection = First;

            /// <summary>
            /// Called after a write, before the store answers: where a request of the server would meet the
            /// gate while the store is still busy.
            /// </summary>
            public Action? AfterWrite { get; set; }

            public IEnumerable<IPinnedTLSEndpoint>  Secure          => [ connection ];
            public IReadOnlyList<PinSetSigner>      PinSetSigners   => [];

            public IPinnedTLSEndpoint? CopyOf(String Id)
                => Id == connection.Id ? connection : null;

            public Boolean TryLearnServerKeyPin(String Id, URL URL, KeyPin Pin, out String? NotWritten)
            {
                NotWritten  = null;
                connection  = connection with { ServerKeyPinLearned = Pin, ServerKeyPinLearnedAt = DateTimeOffset.UtcNow };
                return true;
            }

            public Boolean TryForgetLearnedServerKeyPin(String Id, KeyPin Pin)
            {
                connection = connection with { ServerKeyPinLearned = null, ServerKeyPinLearnedAt = null };
                return true;
            }

            public Boolean TryTakeSignedServerKeyPins(String Id, String? Pins, UInt64 Version, out KeyPin? Learned, out Boolean Confirmed,
                                                      [NotNullWhen(false)] out String? Error)
            {

                Learned    = connection.ServerKeyPinLearned;
                Confirmed  = false;
                Error      = null;

                if (Version <= connection.ServerKeyPinsVersion)
                {
                    Error = $"Version {Version} is not higher than {connection.ServerKeyPinsVersion}.";
                    return false;
                }

                var pins    = (Pins ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(KeyPin.Parse).ToArray();
                Confirmed   = Learned is not null && pins.Contains(Learned);

                connection  = connection with { ServerKeyPins = pins, ServerKeyPinsVersion = Version, ServerKeyPinsConfirmed = Confirmed,
                                                ServerKeyPinLearned = null, ServerKeyPinLearnedAt = null };

                AfterWrite?.Invoke();

                return true;

            }

            public Boolean TryChangeKeyPinTrust(String Id, ServerKeyPinSettings Settings, out KeyPin? Forgotten, out Boolean KeyPinnedChanged,
                                                [NotNullWhen(false)] out String? Error)
            {

                Forgotten         = null;
                KeyPinnedChanged  = false;
                Error             = null;

                // Trust on first use switched off: the key pin learned goes with it.
                if (Settings.TrustOnFirstUse != "None")
                {
                    Error = "Only trust on first use switched off, here.";
                    return false;
                }

                Forgotten         = connection.ServerKeyPinLearned;
                KeyPinnedChanged  = connection.ServerKeyPins.Count == 0;
                connection        = connection with { TrustOnFirstUse = KeyTrustOnFirstUse.None, ServerKeyPinLearned = null, ServerKeyPinLearnedAt = null };

                AfterWrite?.Invoke();

                return true;

            }

            public void DialAgain(String Id) { }

        }

        #endregion

        #region (class) AnyConnection

        /// <summary>
        /// A connection the server's requests come over: nothing but an identity for the gate.
        /// </summary>
        public class AnyConnection : DispatchProxy
        {
            protected override Object? Invoke(MethodInfo? TargetMethod, Object?[]? Arguments)
                => throw new NotSupportedException($"{TargetMethod?.Name} is not for a test of the gate.");
        }

        private static IMessageConnection NewConnection()
            => DispatchProxy.Create<IMessageConnection, AnyConnection>();

        #endregion

        #region Data

        private static readonly URL url = URL.Parse("wss://lc.test:2351/ocpp");

        private Endpoints?           endpoints;
        private ServerKeyPinning?    pinning;
        private IMessageConnection?  current;

        /// <summary>
        /// What the gate said of a request of the server, each time somebody was told something.
        /// </summary>
        private readonly List<(String Told, String? Gate)> gateWhileTelling = [];

        #endregion

        #region (private) Gate()

        private String? Gate()

            => pinning!.Probations.Gate(current!, "Reset", null, (probation, action, payload) => KeyPinGate.Rules(probation, action, payload, "station"));

        #endregion

        #region SetUp

        [SetUp]
        public void AConnectionThatTrustsOnFirstUse()
            => AConnectionThatTrustsOnFirstUse(TimeSpan.FromMinutes(10));

        private void AConnectionThatTrustsOnFirstUse(TimeSpan Probation)
        {

            gateWhileTelling.Clear();

            current    = NewConnection();
            endpoints  = new Endpoints(new AConnection("csms", "the CSMS", url, [], 0, false, null, null) { ServerKeyPinProbationTime = Probation });
            pinning    = new ServerKeyPinning(
                             endpoints,
                             new Probations(),
                             Notice:         text                   => gateWhileTelling.Add(($"notice: {text}",             Gate())),
                             Warning:        text                   => gateWhileTelling.Add(($"warning: {text}",            Gate())),
                             SecurityEvent:  (endpoint, type, info) => gateWhileTelling.Add(($"event: {type}",              Gate())),
                             Failed:         (endpoint, how, text)  => gateWhileTelling.Add(($"failed: {text}",             Gate())),
                             NodeName:       "station"
                         );

        }

        #endregion

        #region (private) FirstHandshake()

        /// <summary>
        /// The first handshake: the server shows a key, the trust learns its key pin, and the connection
        /// made with it is followed - what Follow does once the dial got through.
        /// </summary>
        private (ServerKeyPinTrust Trust, KeyPin Learned) FirstHandshake()
        {

            var trust = pinning!.TrustFor(endpoints!.CopyOf("csms")!, out var notDialled);

            Assert.That(trust, Is.Not.Null, notDialled);

            using var key          = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            using var certificate  = new CertificateRequest("CN=lc.test", key, HashAlgorithmName.SHA256).
                                         CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddDays(1));

            var result = trust!.Validator(this, certificate, null, null!, SslPolicyErrors.RemoteCertificateChainErrors);

            Assert.That(trust.Learned, Is.EqualTo(KeyPin.FromCertificate(certificate)), result.ToString());

            pinning.LearnIfFirst(endpoints.CopyOf("csms")!, url, trust, new Object(), current);

            return (trust, trust.Learned!);

        }

        #endregion


        #region AConnectionThatLearnedItsKey_IsGatedBeforeAnythingIsSaid()

        /// <summary>
        /// The connection that learned its server's key on first use is on probation before the key pin
        /// learned is said in a security event or in the log: a request of its server while either is
        /// being said is refused.
        /// </summary>
        [Test]
        public void AConnectionThatLearnedItsKey_IsGatedBeforeAnythingIsSaid()
        {

            FirstHandshake();

            Assert.Multiple(() => {

                Assert.That(gateWhileTelling.Select(told => told.Told), Has.Some.EqualTo("event: KeyPinLearned"));

                foreach (var (told, gate) in gateWhileTelling)
                    Assert.That(gate, Does.Contain("on probation"), $"While '{told}' was said, a request of the server on probation got through.");

                Assert.That(Gate(), Does.Contain("on probation"));

            });

        }

        #endregion

        #region APinSetWithoutTheLearnedKey_LeavesNoGapAtTheGate()

        /// <summary>
        /// A signed pin set without the key pin learned: its server is not believed any more before
        /// anything is said of it - a request of the refused server while the rejection is being said, in
        /// the log or in a security event, is refused - and stays refused until the connection is closed.
        /// </summary>
        [Test]
        public void APinSetWithoutTheLearnedKey_LeavesNoGapAtTheGate()
        {

            FirstHandshake();

            gateWhileTelling.Clear();

            using var other  = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var taken        = pinning!.TryTakeSignedServerKeyPins("csms", KeyPin.FromKey(other).ToHex(), 1, "the CSMS", out var error, new TaskCompletionSource().Task);

            Assert.Multiple(() => {

                Assert.That(taken, Is.True, error);
                Assert.That(gateWhileTelling.Select(told => told.Told), Has.Some.EqualTo("event: KeyPinRejected"));

                foreach (var (told, gate) in gateWhileTelling)
                    Assert.That(gate, Does.Contain("no longer believes"), $"While '{told}' was said, a request of the refused server got through.");

                Assert.That(Gate(), Does.Contain("no longer believes"));

            });

        }

        #endregion

        #region WhileTheStoreWritesAPinSet_TheServerMeetsTheGate()

        /// <summary>
        /// While the store writes a pin set without the key pin learned - which ends what the trust says it
        /// learned - and before it answers, the connection is still held at the gate: the server the pin
        /// set refuses gets nothing through in between.
        /// </summary>
        [Test]
        public void WhileTheStoreWritesAPinSet_TheServerMeetsTheGate()
        {

            FirstHandshake();

            gateWhileTelling.Clear();

            endpoints!.AfterWrite = () => gateWhileTelling.Add(("store wrote", Gate()));

            using var other  = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var taken        = pinning!.TryTakeSignedServerKeyPins("csms", KeyPin.FromKey(other).ToHex(), 1, "the CSMS", out var error, new TaskCompletionSource().Task);

            Assert.Multiple(() => {
                Assert.That(taken,                                                    Is.True, error);
                Assert.That(gateWhileTelling.Single(told => told.Told == "store wrote").Gate, Is.Not.Null,
                            "While the store wrote the pin set, a request of the refused server got through.");
                Assert.That(Gate(),                                                   Does.Contain("no longer believes"));
            });

        }

        #endregion

        #region WhileTheStoreSwitchesTrustOnFirstUseOff_TheServerMeetsTheGate()

        /// <summary>
        /// A signed SetVariables that switches trust on first use off forgets the key pin learned on
        /// probation: while the store writes that, and before it answers, the connection is still held at
        /// the gate; afterwards it is not believed.
        /// </summary>
        [Test]
        public void WhileTheStoreSwitchesTrustOnFirstUseOff_TheServerMeetsTheGate()
        {

            FirstHandshake();

            gateWhileTelling.Clear();

            endpoints!.AfterWrite = () => gateWhileTelling.Add(("store wrote", Gate()));

            var changed = pinning!.TryChangeKeyPinTrust("csms", new ServerKeyPinSettings(null, "None", null), "the CSMS", new TaskCompletionSource().Task, out var error);

            Assert.Multiple(() => {
                Assert.That(changed,                                                  Is.True, error);
                Assert.That(gateWhileTelling.Single(told => told.Told == "store wrote").Gate, Is.Not.Null,
                            "While the store switched trust on first use off, a request of the server no longer believed got through.");
                Assert.That(Gate(),                                                   Does.Contain("no longer believes"));
            });

        }

        #endregion

        #region AProbationThatRunsOut_LeavesNoGapAtTheGate()

        /// <summary>
        /// A probation that runs out without a signed pin set: the server nobody confirmed is not believed
        /// before anything is said of it - while the security event goes out and the log says so, a request
        /// of that server is refused.
        /// </summary>
        [Test]
        public async Task AProbationThatRunsOut_LeavesNoGapAtTheGate()
        {

            AConnectionThatTrustsOnFirstUse(TimeSpan.FromMilliseconds(200));

            FirstHandshake();

            var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(10);

            while (!gateWhileTelling.Any(told => told.Told == "event: KeyPinProbationExpired") && DateTime.UtcNow < deadline)
                await Task.Delay(20);

            var whileExpired = gateWhileTelling.SkipWhile(told => told.Told != "event: KeyPinProbationExpired").ToArray();

            Assert.Multiple(() => {

                Assert.That(whileExpired, Is.Not.Empty, "The probation did not run out.");

                foreach (var (told, gate) in whileExpired)
                    Assert.That(gate, Does.Contain("no longer believes"), $"While '{told}' was said, a request of the server nobody confirmed got through.");

            });

        }

        #endregion

        #region APinSetWithTheLearnedKey_EndsTheProbation()

        /// <summary>
        /// A signed pin set with the key pin learned confirms it: the probation ends, and the server is
        /// believed.
        /// </summary>
        [Test]
        public void APinSetWithTheLearnedKey_EndsTheProbation()
        {

            var (_, learned)  = FirstHandshake();

            gateWhileTelling.Clear();

            var taken         = pinning!.TryTakeSignedServerKeyPins("csms", learned.ToHex(), 1, "the CSMS", out var error);

            Assert.Multiple(() => {
                Assert.That(taken,                                        Is.True, error);
                Assert.That(gateWhileTelling.Select(told => told.Told),  Has.Some.EqualTo("event: KeyPinConfirmed"));
                Assert.That(Gate(),                                       Is.Null, "the confirmed server is refused");
            });

        }

        #endregion

        #region APinSetSigner_IsReadWhateverTheDayAndTheCulture(Written)

        /// <summary>
        /// When a pin set signer was written down is read as it was written, on a German machine as on
        /// any other, from the 13th of a month on as before it - and not with day and month swapped.
        /// </summary>
        [TestCase("2026-11-13T00:29:11.000Z")]
        [TestCase("2026-11-05T07:08:09.000Z")]
        [SetCulture("de-DE")]
        public void APinSetSigner_IsReadWhateverTheDayAndTheCulture(String Written)
        {

            using var key  = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var point      = key.ExportParameters(false).Q;
            var publicKey  = Convert.ToBase64String([ 4, .. point.X!, .. point.Y! ]);

            // Read from its text, as a file is: that is where the JSON reader
            // recognizes the date.
            var signer     = PinSetSigner.TryParse(Newtonsoft.Json.Linq.JObject.Parse(
                                                       new Newtonsoft.Json.Linq.JObject(
                                                           new Newtonsoft.Json.Linq.JProperty("publicKey",    publicKey),
                                                           new Newtonsoft.Json.Linq.JProperty("description",  "The CSMS"),
                                                           new Newtonsoft.Json.Linq.JProperty("createdAt",    Written)
                                                       ).ToString()
                                                   ));

            var written    = DateTimeOffset.Parse(Written, System.Globalization.CultureInfo.InvariantCulture);

            Assert.Multiple(() => {
                Assert.That(signer,                                   Is.Not.Null);
                Assert.That(signer!.CreatedAt,                        Is.EqualTo(written));
                Assert.That(PinSetSigner.TryParse(Newtonsoft.Json.Linq.JObject.Parse(signer.ToJSON().ToString()))!.CreatedAt,
                                                                      Is.EqualTo(written), "written down and read again");
            });

        }

        #endregion

        #region APinSetTheStoreRefuses_ChangesNothing()

        /// <summary>
        /// A pin set whose version is not higher than the one held is refused by the store: nothing
        /// changes, nothing is said, and the connection stays on probation.
        /// </summary>
        [Test]
        public void APinSetTheStoreRefuses_ChangesNothing()
        {

            var (trust, learned)  = FirstHandshake();

            gateWhileTelling.Clear();

            var taken             = pinning!.TryTakeSignedServerKeyPins("csms", learned.ToHex(), 0, "the CSMS", out var error);

            Assert.Multiple(() => {
                Assert.That(taken,             Is.False);
                Assert.That(error,             Does.Contain("not higher"));
                Assert.That(gateWhileTelling,  Is.Empty, "something was said of a pin set that was not taken");
                Assert.That(trust.Learned,     Is.EqualTo(learned));
                Assert.That(Gate(),            Does.Contain("on probation"));
            });

        }

        #endregion

    }

}
