/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.tests.CentralSystem;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint
{

    /// <summary>
    /// A networking node between charge points and their central system, which
    /// dials the central system in standard networking mode - no network path
    /// on the wire - and forwards what the charge points below it send up.
    /// </summary>
    /// <remarks>
    /// <para>
    /// OCPP 1.6 has no local controller node of its own to test with, so a
    /// charge point node told to forward everything stands in for one: what is
    /// tested is the forwarding of the OCPP adapter, which every node has.
    /// </para>
    /// <para>
    /// Every node numbers its requests from the same start, so the requests of
    /// the charge points below and the node's own carry the same ids as often as
    /// not. The central system here answers nothing until the node has sent it
    /// both requests with the same id, so that both are on their way through the
    /// node at once, every time.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class ForwardingUpInStandardModeTests : ACentralSystemTests
    {

        #region Data

        private const String            Password = "1234abcd";

        private TestChargePointNode?    forwarder;
        private OCPPWebSocketServer?    forwarderServer;

        private readonly List<TestChargePointNode> chargePoints = [];

        #endregion

        #region SetupEachTest() / ShutdownEachTest()

        [SetUp]
        public override void SetupEachTest()
        {

            base.SetupEachTest();

            forwarder        = NewChargePoint("FWD-STD-01");

            forwarder.OCPP.FORWARD.DefaultForwardingDecision = OCPP.NetworkingNode.ForwardingDecisions.FORWARD;

            forwarderServer  = forwarder.AttachWebSocketServer(
                                   TCPPort:                null,   // Random port!
                                   RequireAuthentication:  true,
                                   DisableWebSocketPings:  true,
                                   AutoStart:              true
                               );

            testCentralSystem01!.AddOrUpdateHTTPBasicAuth(forwarder.Id, Password);

            var response = forwarder.ConnectOCPPWebSocketClient(
                               NextHopNetworkingNodeId:  NetworkingNode_Id.CentralSystem,
                               RemoteURL:                URL.Parse($"http://127.0.0.1:{testBackendWebSockets01!.IPPort}/{forwarder.Id}"),
                               HTTPAuthentication:       HTTPBasicAuthentication.Create(forwarder.Id.ToString(), Password),
                               DisableWebSocketPings:    true,
                               NetworkingMode:           NetworkingMode.Standard
                           ).GetAwaiter().GetResult();

            Assert.That(response.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols));

        }

        [TearDown]
        public override async Task ShutdownEachTest()
        {

            foreach (var chargePoint in chargePoints)
                await chargePoint.Stop();

            chargePoints.Clear();

            if (forwarder is not null)
                await forwarder.Stop();

            if (forwarderServer is not null)
                await forwarderServer.DisposeAsync();

            forwarder        = null;
            forwarderServer  = null;

            await base.ShutdownEachTest();

        }

        #endregion

        #region (private) helpers

        private TestChargePointNode NewChargePoint(String Id)
        {

            var chargePoint = new TestChargePointNode(
                                  ChargeBoxId:            NetworkingNode_Id.Parse(Id),
                                  Connectors:             [ new ConnectorSpec(Availabilities.Operative, MaxPower: Watt.FromKW(22)) ],
                                  ChargePointVendor:      "GraphDefined",
                                  ChargePointModel:       "VCP.Forwarding",
                                  DisableSendHeartbeats:  true,
                                  DNSClient:              testCentralSystem01!.DNSClient
                              );

            chargePoints.Add(chargePoint);

            return chargePoint;

        }

        /// <summary>
        /// A charge point connected to the forwarding node, in standard
        /// networking mode, with the central system as its next hop.
        /// </summary>
        private async Task<TestChargePointNode> AChargePointBelow(String Id)
        {

            var chargePoint = NewChargePoint(Id);

            forwarderServer!.AddOrUpdateHTTPBasicAuth(chargePoint.Id, Password);

            var response = await chargePoint.ConnectOCPPWebSocketClient(
                                     NextHopNetworkingNodeId:  NetworkingNode_Id.CentralSystem,
                                     RemoteURL:                URL.Parse($"http://127.0.0.1:{forwarderServer.IPPort}/{chargePoint.Id}"),
                                     HTTPAuthentication:       HTTPBasicAuthentication.Create(chargePoint.Id.ToString(), Password),
                                     DisableWebSocketPings:    true
                                 );

            Assert.That(response.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols));

            return chargePoint;

        }

        /// <summary>
        /// Let the central system answer nothing until the forwarding node has
        /// sent it the given number of requests, or five seconds have passed.
        /// </summary>
        /// <returns>What lets the central system go again, for the test's finally.</returns>
        private Action HoldTheCentralSystemUntilTheNodeSent(Int32 Requests)
        {

            var sent  = 0;
            var all   = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            OCPPv1_6.NetworkingNode.OnJSONRequestMessageSentDelegate     jsonSent  = (timestamp, sender, connection, request, result, cancellationToken) => {
                if (Interlocked.Increment(ref sent) >= Requests)
                    all.TrySetResult();
                return Task.CompletedTask;
            };

            OCPPv1_6.NetworkingNode.OnJSONRequestMessageReceivedDelegate jsonHold  = (timestamp, sender, connection, request, cancellationToken) =>
                Task.WhenAny(all.Task, Task.Delay(TimeSpan.FromSeconds(5)));

            forwarder!.          OCPP.OUT.OnJSONRequestMessageSent     += jsonSent;
            testCentralSystem01!.OCPP.IN. OnJSONRequestMessageReceived += jsonHold;

            return () => {
                forwarder!.          OCPP.OUT.OnJSONRequestMessageSent     -= jsonSent;
                testCentralSystem01!.OCPP.IN. OnJSONRequestMessageReceived -= jsonHold;
            };

        }

        #endregion


        #region TwoChargePoints_AskingWithTheSameRequestId_EachGetTheirOwnAnswer()

        /// <summary>
        /// Two charge points below the node send their BootNotifications with
        /// the same request id, both on their way at once: each is answered, and
        /// each with the answer to its own request.
        /// </summary>
        [Test]
        public async Task TwoChargePoints_AskingWithTheSameRequestId_EachGetTheirOwnAnswer()
        {

            var cp1      = await AChargePointBelow("CP-STD-01");
            var cp2      = await AChargePointBelow("CP-STD-02");

            var sameId   = Request_Id.Parse("4711");
            var release  = HoldTheCentralSystemUntilTheNodeSent(2);

            try
            {

                var boots = await Task.WhenAll(
                                cp1.SendBootNotification(ChargePointSerialNumber: "SN-01", RequestId: sameId, RequestTimeout: TimeSpan.FromSeconds(10)),
                                cp2.SendBootNotification(ChargePointSerialNumber: "SN-02", RequestId: sameId, RequestTimeout: TimeSpan.FromSeconds(10))
                            );

                Assert.Multiple(() => {

                    Assert.That(boots[0].Result.ResultCode,  Is.EqualTo(ResultCode.OK), $"CP-STD-01: {boots[0].Result}");
                    Assert.That(boots[0].Status,             Is.EqualTo(RegistrationStatus.Accepted));
                    Assert.That(boots[0].Request.RequestId,  Is.EqualTo(sameId));

                    Assert.That(boots[1].Result.ResultCode,  Is.EqualTo(ResultCode.OK), $"CP-STD-02: {boots[1].Result}");
                    Assert.That(boots[1].Status,             Is.EqualTo(RegistrationStatus.Accepted));
                    Assert.That(boots[1].Request.RequestId,  Is.EqualTo(sameId));

                });

            }
            finally
            {
                release();
            }

        }

        #endregion

        #region TheNodesOwnRequest_AndAForwardedOne_WithTheSameId_EachGetTheirOwnAnswer()

        /// <summary>
        /// The node asks the central system for itself while a charge point's
        /// request with the same id is on its way through it: the node gets the
        /// answer to its heartbeat, the charge point the one to its
        /// BootNotification.
        /// </summary>
        [Test]
        public async Task TheNodesOwnRequest_AndAForwardedOne_WithTheSameId_EachGetTheirOwnAnswer()
        {

            var cp1      = await AChargePointBelow("CP-STD-03");

            var sameId   = Request_Id.Parse("4712");
            var release  = HoldTheCentralSystemUntilTheNodeSent(2);

            try
            {

                var boot       = cp1.       SendBootNotification(ChargePointSerialNumber: "SN-03", RequestId: sameId, RequestTimeout: TimeSpan.FromSeconds(10));
                var heartbeat  = forwarder!.SendHeartbeat       (                                  RequestId: sameId, RequestTimeout: TimeSpan.FromSeconds(10));

                await Task.WhenAll(boot, heartbeat);

                Assert.Multiple(() => {

                    Assert.That(boot.Result.Result.ResultCode,       Is.EqualTo(ResultCode.OK), $"The charge point: {boot.Result.Result}");
                    Assert.That(boot.Result.Status,                  Is.EqualTo(RegistrationStatus.Accepted));

                    Assert.That(heartbeat.Result.Result.ResultCode,  Is.EqualTo(ResultCode.OK), $"The node: {heartbeat.Result.Result}");
                    Assert.That((heartbeat.Result.CurrentTime - Timestamp.Now).Duration(), Is.LessThan(TimeSpan.FromMinutes(1)));

                });

            }
            finally
            {
                release();
            }

        }

        #endregion

    }

}
