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
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.NetworkingNode;
using cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint;
using cloud.charging.open.protocols.OCPPv1_6.tests.CentralSystem;
using cloud.charging.open.protocols.OCPPv1_6.tests.CPwithCS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.OverCBOR
{

    // The tests of OCPP 1.6, once more - with charge points that offer "ocpp1.6cbor"
    // alone, so that every message of theirs crosses the wire as CBOR in a binary
    // frame, and is JSON again before a test sees it.

    #region The fixtures over CBOR

    /// <summary>
    /// The messages of a central system to its charge points, over CBOR.
    /// </summary>
    [TestFixture]
    public class CS_Messages_OverCBOR_Tests : CS_Messages_Tests
    {

        protected override MessageEncodings Encoding => MessageEncodings.CBOR;

        [Test]
        public void TheChargePointSpeaksCBOR()
            => OverCBOR.AgreedOnCBOR(chargePoint1);

    }

    /// <summary>
    /// Charging, over CBOR.
    /// </summary>
    [TestFixture]
    public class Charging_OverCBOR_Tests : ChargingTests
    {

        protected override MessageEncodings Encoding => MessageEncodings.CBOR;

        [Test]
        public void TheChargePointSpeaksCBOR()
            => OverCBOR.AgreedOnCBOR(chargePoint1);

    }

    /// <summary>
    /// The messages of a charge point to its central system, over CBOR.
    /// </summary>
    [TestFixture]
    public class ChargePoint_Messages_OverCBOR_Tests : ChargePoint_Messages_Tests
    {
        protected override MessageEncodings Encoding => MessageEncodings.CBOR;
    }

    /// <summary>
    /// Binary data of a charge point, over CBOR: a message of the binary format
    /// crosses as a byte string.
    /// </summary>
    [TestFixture]
    public class CP_SendBinaryData_OverCBOR_Tests : cloud.charging.open.protocols.OCPPv1_6.tests.extensions.BinaryStreamsExtensions.CP_SendBinaryData_Tests
    {

        protected override MessageEncodings Encoding => MessageEncodings.CBOR;

        [Test]
        public void TheChargePointSpeaksCBOR()
            => OverCBOR.AgreedOnCBOR(chargePoint1);

    }

    /// <summary>
    /// Files asked of and stored at a charge point, over CBOR.
    /// </summary>
    [TestFixture]
    public class CSMS_GetStoreFiles_OverCBOR_Tests : cloud.charging.open.protocols.OCPPv1_6.tests.extensions.BinaryStreamsExtensions.CSMS_GetStoreFiles_Tests
    {

        protected override MessageEncodings Encoding => MessageEncodings.CBOR;

        [Test]
        public void TheChargePointSpeaksCBOR()
            => OverCBOR.AgreedOnCBOR(chargePoint1);

    }

    #endregion

    #region The extensions over CBOR

    /// <summary>
    /// The vendor extensions - a web payment that started, and one that failed: DataTransfer requests of a class of their own, over CBOR.
    /// </summary>
    [TestFixture]
    public class CS_Messages_VendorExtensions_OverCBOR_Tests : CS_Messages_VendorExtensions_Tests
    {

        protected override MessageEncodings Encoding => MessageEncodings.CBOR;

        [Test]
        public void TheChargePointSpeaksCBOR()
            => OverCBOR.AgreedOnCBOR(chargePoint1);

    }

    /// <summary>
    /// The network time extensions, over CBOR.
    /// </summary>
    [TestFixture]
    public class CS_Messages_NTSExtensions_OverCBOR_Tests : CS_Messages_NTSExtensions_Tests
    {

        protected override MessageEncodings Encoding => MessageEncodings.CBOR;

        [Test]
        public void TheChargePointSpeaksCBOR()
            => OverCBOR.AgreedOnCBOR(chargePoint1);

    }

    /// <summary>
    /// The diagnostic control extensions, over CBOR.
    /// </summary>
    [TestFixture]
    public class CS_Messages_DiagnosticControlExtensions_OverCBOR_Tests : CS_Messages_DiagnosticControlExtensions_Tests
    {

        protected override MessageEncodings Encoding => MessageEncodings.CBOR;

        [Test]
        public void TheChargePointSpeaksCBOR()
            => OverCBOR.AgreedOnCBOR(chargePoint1);

    }

    #endregion

    #region Over SSH, and through a node that forwards

    /// <summary>
    /// OCPP 1.6 over SSH, with charge points that offer "ocpp1.6cbor" alone when they
    /// sign in: the same subprotocol as over a WebSocket, and the same messages.
    /// </summary>
    [TestFixture]
    public class OCPPOverSSH_OverCBOR_Tests : SSH.OCPPOverSSHTests
    {

        protected override MessageEncodings Encoding => MessageEncodings.CBOR;

        [Test]
        public void TheFixtureSpeaksCBOR()
            => Assert.That(AgreedOn, Is.EqualTo(Version.WebSocketSubProtocolIdCBOR));

    }

    /// <summary>
    /// Forwarding up in standard mode, with CBOR below the node that forwards.
    /// </summary>
    [TestFixture]
    public class ForwardingUp_CBORDown_Tests : ForwardingUpInStandardModeTests
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
    }

    /// <summary>
    /// Forwarding up in standard mode, with CBOR above the node that forwards.
    /// </summary>
    [TestFixture]
    public class ForwardingUp_CBORUp_Tests : ForwardingUpInStandardModeTests
    {
        protected override MessageEncodings Uplink => MessageEncodings.CBOR;
    }

    /// <summary>
    /// Forwarding up in standard mode, with CBOR on both sides.
    /// </summary>
    [TestFixture]
    public class ForwardingUp_CBORBoth_Tests : ForwardingUpInStandardModeTests
    {
        protected override MessageEncodings Downlink => MessageEncodings.CBOR;
        protected override MessageEncodings Uplink   => MessageEncodings.CBOR;
    }

    #endregion

    #region The translation of OCPP 1.6

    /// <summary>
    /// What of OCPP 1.6 is translated through the class of a message.
    /// </summary>
    [TestFixture]
    public class TheMessagesOfOCPP16AsCBORTests
    {

        [Test]
        public void TheCoreProfile_IsTranslatedThroughItsClasses()
        {

            var requests   = OCPPMessagesAsCBOR.Requests. ToHashSet();
            var responses  = OCPPMessagesAsCBOR.Responses.ToHashSet();

            Assert.Multiple(() => {

                foreach (var action in new[] {
                                           "Authorize", "BootNotification", "ChangeAvailability", "ChangeConfiguration", "ClearCache",
                                           "DataTransfer", "GetConfiguration", "Heartbeat", "MeterValues", "RemoteStartTransaction",
                                           "RemoteStopTransaction", "Reset", "StartTransaction", "StatusNotification", "StopTransaction",
                                           "UnlockConnector"
                                       })
                {
                    Assert.That(requests,   Does.Contain(action), $"The request '{action}' is carried blind.");
                    Assert.That(responses,  Does.Contain(action), $"The response to '{action}' is carried blind.");
                }

                Assert.That(OCPPMessagesAsCBOR.KnowsRequest("NoSuchAction"),  Is.False);

            });

        }

        [Test]
        public void ARequest_ComesBackFromItsCBORAsItWent()
        {

            var json = new Newtonsoft.Json.Linq.JObject(
                           new Newtonsoft.Json.Linq.JProperty("chargePointVendor",  "GraphDefined"),
                           new Newtonsoft.Json.Linq.JProperty("chargePointModel",   "VCP.1")
                       );

            Assert.That(OCPPMessagesAsCBOR.TryRequestToCBOR("BootNotification", json, Request_Id.Parse("1"),
                                                            SourceRouting.To(NetworkingNode_Id.CentralSystem), NetworkPath.Empty,
                                                            out var cbor, out var request, out var e1),
                        Is.True, e1);

            Assert.That(OCPPMessagesAsCBOR.TryRequestToJSON("BootNotification", cbor, Request_Id.Parse("1"),
                                                            SourceRouting.To(NetworkingNode_Id.CentralSystem), NetworkPath.Empty,
                                                            out var back, out _, out var e2),
                        Is.True, e2);

            Assert.Multiple(() => {
                Assert.That(request,                                                Is.InstanceOf<BootNotificationRequest>());
                Assert.That(Newtonsoft.Json.Linq.JToken.DeepEquals(back, json),     Is.True, back?.ToString(Newtonsoft.Json.Formatting.None));
            });

        }

    }

    #endregion

    #region Signed with COSE

    /// <summary>
    /// A charge point of OCPP 1.6 that signs with COSE: the COSE_Sign1 over the CBOR
    /// of its message, verified by its central system - over JSON, where the
    /// signature travels as its bytes, and over CBOR.
    /// </summary>
    [TestFixture]
    public class SignedWithCOSE_Tests : AChargePointTests
    {

        [Test]
        public async Task ABootNotification_IsSignedWithCOSE_AndVerified()
        {

            var received = new ConcurrentList<BootNotificationRequest>();

            testCentralSystem01!.OCPP.IN.OnBootNotificationRequestReceived += (timestamp, sender, connection, request, ct) => {
                received.TryAdd(request);
                return Task.CompletedTask;
            };

            testCentralSystem01.OCPP.SignaturePolicy.AddVerificationRule(BootNotificationRequest.DefaultJSONLDContext,
                                                                         VerificationRuleActions.VerifyAll);

            chargePoint1!.OCPP.SignaturePolicy.Formats = SignatureFormats.COSE;

            var response = await chargePoint1.SendBootNotification(
                                     SignInfos: [ ECCKeyPair.GenerateKeys()!.ToSignInfo1("ahzf", I18NString.Create("Just a test!"), Timestamp.Now) ]
                                 );

            Assert.Multiple(() => {

                Assert.That(response.Result.ResultCode,                        Is.EqualTo(ResultCode.OK), response.Result.Description);
                Assert.That(received,                                          Has.Count.EqualTo(1));

                if (received.Count == 1)
                {
                    Assert.That(received.First().Signatures.Count(),           Is.EqualTo(1));
                    Assert.That(received.First().Signatures.First().IsCOSE,    Is.True);
                    Assert.That(received.First().Signatures.First().Status,    Is.EqualTo(cloud.charging.open.protocols.WWCP.VerificationStatus.ValidSignature));
                }

            });

        }

    }

    /// <summary>
    /// The same over a connection that speaks CBOR.
    /// </summary>
    [TestFixture]
    public class SignedWithCOSE_OverCBOR_Tests : SignedWithCOSE_Tests
    {

        protected override MessageEncodings Encoding => MessageEncodings.CBOR;

        [Test]
        public void TheChargePointSpeaksCBOR()
            => OverCBOR.AgreedOnCBOR(chargePoint1);

    }

    #endregion


    internal static class OverCBOR
    {

        /// <summary>
        /// The connection of the given charge point agreed on "ocpp1.6cbor" - or the
        /// fixture ran its tests over JSON, and proved nothing about CBOR.
        /// </summary>
        internal static void AgreedOnCBOR(TestChargePointNode? ChargePoint)
        {

            Assert.That(ChargePoint, Is.Not.Null);

            Assert.That(ChargePoint!.WWCPWebSocketClients.OfType<WWCPWebSocketClient>().Single().WebSocketClientConnection?.Subprotocol,
                        Is.EqualTo(Version.WebSocketSubProtocolIdCBOR));

        }

    }

}
