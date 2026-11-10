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

using System.Collections.Concurrent;

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.CentralSystem
{

    /// <summary>
    /// A charge point connected to two central systems at once answers each of
    /// them over the connection its request came over.
    /// </summary>
    /// <remarks>
    /// Both connections lead to "CentralSystem", and a node has one route by that
    /// name: to whichever connection came up last. An answer used to be looked up
    /// by that name, so the answer to the first central system's request went to
    /// the second one, which knew nothing of it, and the first waited out its
    /// timeout. And an error to a request the charge point had no processor for
    /// was addressed to nobody at all, and dropped before it was sent.
    /// </remarks>
    [TestFixture]
    public class AnswersFollowTheirConnectionTests : ACentralSystemTests
    {

        #region Data

        private TestCentralSystemNode?             testCentralSystem02;
        private OCPPWebSocketServer?               testBackendWebSockets02;
        private TestChargePointNode?               chargePoint;

        private readonly ConcurrentQueue<Byte>     receivedBy01  = [];
        private readonly ConcurrentQueue<Byte>     receivedBy02  = [];

        #endregion

        #region Setup / TearDown

        [SetUp]
        public override void SetupEachTest()
        {

            base.SetupEachTest();

            testCentralSystem02      = new TestCentralSystemNode(
                                           Id:                       NetworkingNode_Id.Parse("OCPPTest02"),
                                           VendorName:               "GraphDefined",
                                           Model:                    "CSSimulator2",
                                           HTTPAPI_Disabled:         true,
                                           HTTPUploadAPI_Disabled:   true,
                                           DNSClient:                new DNSClient(
                                                                         SearchForIPv6DNSServers: false,
                                                                         SearchForIPv4DNSServers: false
                                                                     )
                                       );

            testBackendWebSockets02  = testCentralSystem02.AttachWebSocketServer(
                                           AutoStart:  true
                                       );

            receivedBy01.Clear();
            receivedBy02.Clear();

            Record(testBackendWebSockets01!, receivedBy01);
            Record(testBackendWebSockets02,  receivedBy02);

        }

        [TearDown]
        public override async Task ShutdownEachTest()
        {

            if (chargePoint is not null)
                await chargePoint.Stop();

            if (testCentralSystem02 is not null)
                await testCentralSystem02.Shutdown();

            if (testBackendWebSockets02 is not null)
                await testBackendWebSockets02.DisposeAsync();

            chargePoint              = null;
            testCentralSystem02      = null;
            testBackendWebSockets02  = null;

            await base.ShutdownEachTest();

        }

        #endregion


        #region (private static) Record(Server, Received)

        /// <summary>
        /// Write down every answer the given server receives: a response (3), a
        /// request error (4) or a response error (5) - and not what a charge point
        /// says by itself.
        /// </summary>
        private static void Record(OCPPWebSocketServer    Server,
                                   ConcurrentQueue<Byte>  Received)
        {

            Server.OnJSONMessageReceived += (timestamp, webSocketServer, webSocketConnection, messageTimestamp, eventTrackingId, sourceNodeId, message, cancellationToken) => {

                var type = message[0].Value<Byte>();

                if (type is 3 or 4 or 5)
                    Received.Enqueue(type);

                return Task.CompletedTask;

            };

        }

        #endregion

        #region (private) AChargePointConnectedToBoth(Id)

        /// <summary>
        /// A charge point connected to the first central system and then to the
        /// second one, both as its central system.
        /// </summary>
        private async Task<TestChargePointNode> AChargePointConnectedToBoth(String Id)
        {

            chargePoint = new TestChargePointNode(
                              ChargeBoxId:        NetworkingNode_Id.Parse(Id),
                              Connectors:         [ new ConnectorSpec(Availabilities.Operative, MaxPower: Watt.FromKW(22)) ],
                              ChargePointVendor:  "GraphDefined OEM #1",
                              ChargePointModel:   "VCP.1",
                              DNSClient:          testCentralSystem01!.DNSClient
                          );

            foreach (var (centralSystem, server) in new[] { (testCentralSystem01!, testBackendWebSockets01!),
                                                            (testCentralSystem02!, testBackendWebSockets02!) })
            {

                centralSystem.AddOrUpdateHTTPBasicAuth(chargePoint.Id, "1234abcd");

                var upgrade = await chargePoint.ConnectOCPPWebSocketClient(
                                        RemoteURL:                URL.Parse($"http://127.0.0.1:{server.IPPort}/{chargePoint.Id}"),
                                        NextHopNetworkingNodeId:  NetworkingNode_Id.CentralSystem,
                                        HTTPAuthentication:       HTTPBasicAuthentication.Create(chargePoint.Id.ToString(), "1234abcd"),
                                        DisableWebSocketPings:    true
                                    );

                Assert.That(upgrade.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols),
                            $"The charge point could not connect to '{centralSystem.Id}'.");

            }

            return chargePoint;

        }

        #endregion

        #region (private static) Ask(CentralSystem, ChargePoint, Action)

        private static Task<SendRequestState> Ask(TestCentralSystemNode  CentralSystem,
                                                  NetworkingNode_Id      ChargePoint,
                                                  String                 Action)

            => CentralSystem.OCPP.OUT.SendJSONRequestAndWait(
                   new OCPP_JSONRequestMessage(
                       RequestTimestamp:   Timestamp.Now,
                       EventTrackingId:    EventTracking_Id.New,
                       NetworkingMode:     NetworkingMode.Standard,
                       Destination:        SourceRouting.To(ChargePoint),
                       NetworkPath:        NetworkPath.From(CentralSystem.Id),
                       RequestId:          Request_Id.NewRandom(),
                       Action:             Action,
                       Payload:            new JObject(),
                       RequestTimeout:     Timestamp.Now + TimeSpan.FromSeconds(5)
                   )
               );

        #endregion


        #region TheCentralSystemThatConnectedFirstIsAnswered()

        /// <summary>
        /// The first central system asks, the second one connected last: the
        /// answer goes to the first, and the second receives nothing.
        /// </summary>
        [Test]
        public async Task TheCentralSystemThatConnectedFirstIsAnswered()
        {

            var cp     = await AChargePointConnectedToBoth("GD-MH-101");
            var state  = await Ask(testCentralSystem01!, cp.Id, "GetLocalListVersion");

            Assert.Multiple(() => {

                Assert.That(state.JSONResponse,  Is.Not.Null,
                            $"The central system that asked was not answered: '{state.JSONRequestErrorMessage?.ErrorCode}'.");

                Assert.That(receivedBy02,        Is.Empty,
                            "The central system that connected last was handed the answer to another one's request.");

            });

        }

        #endregion

        #region TheCentralSystemThatConnectedLastIsAnswered()

        [Test]
        public async Task TheCentralSystemThatConnectedLastIsAnswered()
        {

            var cp     = await AChargePointConnectedToBoth("GD-MH-102");
            var state  = await Ask(testCentralSystem02!, cp.Id, "GetLocalListVersion");

            Assert.Multiple(() => {

                Assert.That(state.JSONResponse,  Is.Not.Null,
                            $"The central system that asked was not answered: '{state.JSONRequestErrorMessage?.ErrorCode}'.");

                Assert.That(receivedBy01,        Is.Empty,
                            "The central system that connected first was handed the answer to another one's request.");

            });

        }

        #endregion

        #region AnUnknownActionIsRefusedToTheCentralSystemThatSentIt()

        /// <summary>
        /// A request the charge point has no processor for is refused, and the
        /// refusal reaches the central system that sent it, rather than its timeout.
        /// </summary>
        [Test]
        public async Task AnUnknownActionIsRefusedToTheCentralSystemThatSentIt()
        {

            var cp     = await AChargePointConnectedToBoth("GD-MH-103");
            var state  = await Ask(testCentralSystem01!, cp.Id, "NoSuchAction");

            Assert.Multiple(() => {

                Assert.That(state.JSONResponse,                        Is.Null);

                Assert.That(state.JSONRequestErrorMessage?.ErrorCode,  Is.EqualTo(ResultCode.ProtocolError),
                            "The refusal of an unknown action did not reach the central system that sent it.");

                Assert.That(receivedBy02,                              Is.Empty,
                            "The refusal went to the central system that connected last.");

            });

        }

        #endregion

    }

}
