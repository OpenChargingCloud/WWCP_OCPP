/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.Extensions.E2ESecurityExtensions
{

    /// <summary>
    /// Which signatures a node writes is a matter of the connection a message leaves
    /// over: the JWS over its JSON, the COSE_Sign1 over its CBOR, or both. Where a
    /// connection says nothing, the signature policy of the node decides.
    /// </summary>
    /// <remarks>
    /// Every charging station signs with a key of its own: the stations number their
    /// requests alike, and one key under one message id is one signature to a CSMS
    /// that remembers what it has seen.
    /// </remarks>
    [TestFixture]
    public class SignatureFormatsPerConnectionTests : AChargingStationTests
    {

        #region (private) Helpers

        private static SignInfo[] Signed()
            => [ ECCKeyPair.GenerateKeys()!.ToSignInfo1("ahzf", I18NString.Create("Just a test!"), Timestamp.Now) ];

        private static String FormatsOf(IEnumerable<Signature> Signatures)
            => String.Join("+", Signatures.Select(signature => signature.IsCOSE ? "COSE" : "JWS").Order());

        private static String Why(BootNotificationResponse Response)
            => $"{Response.Result.ResultCode}: {Response.Result.Description}";

        private WWCPWebSocketClient ClientOf(TestChargingStationNode ChargingStation)
            => ChargingStation.WWCPWebSocketClients.OfType<WWCPWebSocketClient>().Single();

        #endregion

        #region SayNothingAgain()

        /// <summary>
        /// What a test said of a connection, and of a policy, is taken back.
        /// </summary>
        [TearDown]
        public void SayNothingAgain()
        {

            foreach (var chargingStation in new[] { chargingStation1, chargingStation2, chargingStation3 })
            {

                if (chargingStation is null)
                    continue;

                chargingStation.OCPP.SetSignatureFormats(NetworkingNode_Id.CSMS, null);
                chargingStation.OCPP.SignaturePolicy.Formats = SignatureFormats;

                foreach (var client in chargingStation.WWCPWebSocketClients.OfType<WWCPWebSocketClient>())
                    chargingStation.OCPP.SetSignatureFormats(client, null);

                testCSMS1?.OCPP.SetSignatureFormats(chargingStation.Id, null);

            }

        }

        #endregion


        #region ARequest_IsSignedTheWayItsConnectionSays()

        /// <summary>
        /// Three charging stations of one policy: the connection of the first says
        /// COSE, the node at the far end of the second's both, the third's nothing.
        /// </summary>
        [Test]
        public async Task ARequest_IsSignedTheWayItsConnectionSays()
        {

            var received = new ConcurrentList<CS.BootNotificationRequest>();

            testCSMS1!.OCPP.IN.OnBootNotificationRequestReceived += (timestamp, sender, connection, request, ct) => {
                received.TryAdd(request);
                return Task.CompletedTask;
            };

            testCSMS1.OCPP.SignaturePolicy.AddVerificationRule(BootNotificationRequest.DefaultJSONLDContext,
                                                               VerificationRuleActions.VerifyAll);

            // Said of the connection itself, and of the node at its far end.
            chargingStation1!.OCPP.SetSignatureFormats(ClientOf(chargingStation1),  SignatureFormats.COSE);
            chargingStation2!.OCPP.SetSignatureFormats(NetworkingNode_Id.CSMS,      SignatureFormats.Both);

            var response1 = await chargingStation1. SendBootNotification(BootReason: BootReason.PowerUp, SignInfos: Signed());
            var response2 = await chargingStation2. SendBootNotification(BootReason: BootReason.PowerUp, SignInfos: Signed());
            var response3 = await chargingStation3!.SendBootNotification(BootReason: BootReason.PowerUp, SignInfos: Signed());

            await EveryMessageHandled();

            String Of(String VendorName)
                => received.LastOrDefault(request => request.ChargingStation.VendorName == VendorName) is CS.BootNotificationRequest request
                       ? FormatsOf(request.Signatures)
                       : "not received";

            Assert.Multiple(() => {

                Assert.That(response1.Result.ResultCode,     Is.EqualTo(ResultCode.OK), Why(response1));
                Assert.That(response2.Result.ResultCode,     Is.EqualTo(ResultCode.OK), Why(response2));
                Assert.That(response3.Result.ResultCode,     Is.EqualTo(ResultCode.OK), Why(response3));

                Assert.That(Of("GraphDefined OEM #1"),       Is.EqualTo("COSE"));
                Assert.That(Of("GraphDefined OEM #2"),       Is.EqualTo("COSE+JWS"));
                Assert.That(Of("GraphDefined OEM #3"),       Is.EqualTo("JWS"), "what the policy says, where the connection says nothing");

                Assert.That(received.SelectMany(request => request.Signatures).Select(signature => signature.Status),
                            Is.All.EqualTo(VerificationStatus.ValidSignature));

                Assert.That(chargingStation1.OCPP.SignatureFormatsFor(NetworkingNode_Id.CSMS),  Is.EqualTo(SignatureFormats.COSE));
                Assert.That(chargingStation2.OCPP.SignatureFormatsFor(NetworkingNode_Id.CSMS),  Is.EqualTo(SignatureFormats.Both));
                Assert.That(chargingStation3.OCPP.SignatureFormatsFor(NetworkingNode_Id.CSMS),  Is.Null);

            });

        }

        #endregion

        #region AResponse_IsSignedTheWayTheConnectionOfItsRequestSays()

        /// <summary>
        /// A CSMS that signs its responses with COSE for one charging station, and
        /// the way its policy says for every other one.
        /// </summary>
        [Test]
        public async Task AResponse_IsSignedTheWayTheConnectionOfItsRequestSays()
        {

            testCSMS1!.OCPP.SignaturePolicy.AddSigningRule(BootNotificationResponse.DefaultJSONLDContext, ECCKeyPair.GenerateKeys()!);

            testCSMS1.OCPP.SetSignatureFormats(chargingStation1!.Id, SignatureFormats.COSE);

            // Signed, whether or not the CSMS of this fixture asks for it.
            var response1 = await chargingStation1. SendBootNotification(BootReason: BootReason.PowerUp, SignInfos: Signed());
            var response2 = await chargingStation2!.SendBootNotification(BootReason: BootReason.PowerUp, SignInfos: Signed());

            Assert.Multiple(() => {

                Assert.That(response1.Result.ResultCode,     Is.EqualTo(ResultCode.OK), Why(response1));
                Assert.That(response2.Result.ResultCode,     Is.EqualTo(ResultCode.OK), Why(response2));

                Assert.That(FormatsOf(response1.Signatures), Is.EqualTo("COSE"));
                Assert.That(FormatsOf(response2.Signatures), Is.EqualTo("JWS"));

            });

        }

        #endregion

        #region SayingNothingAgain_IsWhatThePolicySays()

        [Test]
        public async Task SayingNothingAgain_IsWhatThePolicySays()
        {

            var received = new ConcurrentList<CS.BootNotificationRequest>();

            testCSMS1!.OCPP.IN.OnBootNotificationRequestReceived += (timestamp, sender, connection, request, ct) => {
                received.TryAdd(request);
                return Task.CompletedTask;
            };

            var client = ClientOf(chargingStation1!);

            chargingStation1!.OCPP.SetSignatureFormats(client, SignatureFormats.COSE);
            var response1 = await chargingStation1.SendBootNotification(BootReason: BootReason.PowerUp, SignInfos: Signed());

            chargingStation1.OCPP.SetSignatureFormats(client, null);
            var response2 = await chargingStation1.SendBootNotification(BootReason: BootReason.PowerUp, SignInfos: Signed());

            // The policy of the node, where no connection says otherwise.
            chargingStation1.OCPP.SignaturePolicy.Formats = SignatureFormats.Both;
            var response3 = await chargingStation1.SendBootNotification(BootReason: BootReason.PowerUp, SignInfos: Signed());

            await EveryMessageHandled();

            Assert.Multiple(() => {

                Assert.That(response1.Result.ResultCode,     Is.EqualTo(ResultCode.OK), Why(response1));
                Assert.That(response2.Result.ResultCode,     Is.EqualTo(ResultCode.OK), Why(response2));
                Assert.That(response3.Result.ResultCode,     Is.EqualTo(ResultCode.OK), Why(response3));

                Assert.That(received.Select(request => FormatsOf(request.Signatures)),          Is.EqualTo(new[] { "COSE", "JWS", "COSE+JWS" }));
                Assert.That(chargingStation1.OCPP.SignatureFormatsFor(NetworkingNode_Id.CSMS),  Is.Null);

            });

        }

        #endregion

    }

}
