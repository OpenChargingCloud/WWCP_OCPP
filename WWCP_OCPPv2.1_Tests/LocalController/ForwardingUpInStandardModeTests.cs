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

using System.Collections.Concurrent;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.LC;
using cloud.charging.open.protocols.OCPPv2_1.LocalController;
using cloud.charging.open.protocols.OCPPv2_1.tests.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.LocalController
{

    /// <summary>
    /// A local controller that dials its CSMS in standard networking mode - no
    /// network path on the wire, as a CSMS that knows nothing of overlay
    /// networks has it - and forwards its stations' requests up.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The CSMS answers what came over the controller's connection to the
    /// controller, and the answer comes back over the controller's client
    /// connection. Which of the stations behind it asked - or whether the
    /// controller asked for itself - only the controller can know.
    /// </para>
    /// <para>
    /// Every node numbers its requests from the same start, so the stations'
    /// requests and the controller's own carry the same ids as often as not.
    /// The CSMS here answers nothing until the controller has sent it both
    /// requests with the same id, so that both are on their way through the
    /// controller at once, every time.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class ForwardingUpInStandardModeTests : ACSMSTests
    {

        #region Data

        private const String              Password = "1234abcd";

        private TestLocalControllerNode?  localController;
        private OCPPWebSocketServer?      localControllerServer;

        private readonly List<TestChargingStationNode> stations = [];

        #endregion

        #region SetupOnce() / StopTheStations() / StopTheController()

        [OneTimeSetUp]
        public override async Task SetupOnce()
        {

            await base.SetupOnce();

            localController       = new TestLocalControllerNode(
                                        Id:                     NetworkingNode_Id.Parse("LC-STD-01"),
                                        VendorName:             "GraphDefined",
                                        Model:                  "LC.1",
                                        HTTPAPI_Disabled:       true,
                                        WebAPI_Disabled:        true,
                                        DisableSendHeartbeats:  true,
                                        DNSClient:              dnsClient
                                    );

            localControllerServer = localController.AttachWebSocketServer(
                                        HTTPServiceName:        "Forwarding up in standard mode",
                                        TCPPort:                null,   // Random port!
                                        RequireAuthentication:  true,
                                        DisableWebSocketPings:  true,
                                        AutoStart:              true
                                    );

            testCSMS1!.AddOrUpdateHTTPBasicAuth(localController.Id, Password);

            var response = await localController.ConnectOCPPWebSocketClient(
                                     NextHopNetworkingNodeId:  NetworkingNode_Id.CSMS,
                                     RemoteURL:                URL.Parse($"http://127.0.0.1:{testBackendWebSockets1!.IPPort}/{localController.Id}"),
                                     HTTPAuthentication:       HTTPBasicAuthentication.Create(localController.Id.ToString(), Password),
                                     DisableWebSocketPings:    true,
                                     NetworkingMode:           NetworkingMode.Standard
                                 );

            Assert.That(response.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols));

        }

        [TearDown]
        public async Task StopTheStations()
        {

            foreach (var station in stations)
                await station.Stop();

            stations.Clear();

        }

        [OneTimeTearDown]
        public async Task StopTheController()
        {

            if (localController is not null)
                await localController.Stop();

            if (localControllerServer is not null)
                await localControllerServer.DisposeAsync();

        }

        #endregion

        #region (private) helpers

        /// <summary>
        /// A charging station connected to the local controller, in standard
        /// networking mode, with the CSMS as its next hop.
        /// </summary>
        private async Task<TestChargingStationNode> AStation(String Id)
        {

            var station = new TestChargingStationNode(
                              Id:                     NetworkingNode_Id.Parse(Id),
                              VendorName:             "GraphDefined",
                              Model:                  "CS.1",
                              DisableSendHeartbeats:  true,
                              HTTPAPI_Disabled:       true,
                              WebAPI_Disabled:        true,
                              DNSClient:              dnsClient
                          );

            stations.Add(station);

            localControllerServer!.AddOrUpdateHTTPBasicAuth(station.Id, Password);

            var response = await station.ConnectOCPPWebSocketClient(
                                     NextHopNetworkingNodeId:  NetworkingNode_Id.CSMS,
                                     RemoteURL:                URL.Parse($"http://127.0.0.1:{localControllerServer.IPPort}/{station.Id}"),
                                     HTTPAuthentication:       HTTPBasicAuthentication.Create(station.Id.ToString(), Password),
                                     DisableWebSocketPings:    true
                                 );

            Assert.That(response.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols));

            return station;

        }

        /// <summary>
        /// Let the CSMS answer nothing until the controller has sent it the given
        /// number of requests, or five seconds have passed: so that they are all
        /// on their way through the controller at the same time.
        /// </summary>
        /// <remarks>
        /// Counted where the controller sends them rather than where the CSMS
        /// takes them: the CSMS reads one connection one message at a time, so
        /// while it holds the first, the second waits unread in the socket.
        /// </remarks>
        /// <returns>What lets the CSMS go again, for the test's finally.</returns>
        private Action HoldTheCSMSUntilTheControllerSent(Int32 Requests)
        {

            var sent  = 0;
            var all   = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            void OneMore()
            {
                if (Interlocked.Increment(ref sent) >= Requests)
                    all.TrySetResult();
            }

            OCPPv2_1.NetworkingNode.OnJSONRequestMessageSentDelegate      jsonSent    = (timestamp, sender, connection, request, result, cancellationToken) => { OneMore(); return Task.CompletedTask; };
            OCPPv2_1.NetworkingNode.OnBinaryRequestMessageSentDelegate    binarySent  = (timestamp, sender, connection, request, result, cancellationToken) => { OneMore(); return Task.CompletedTask; };

            OCPPv2_1.NetworkingNode.OnJSONRequestMessageReceivedDelegate   jsonHold    = (timestamp, sender, connection, request, cancellationToken) => Task.WhenAny(all.Task, Task.Delay(TimeSpan.FromSeconds(5)));
            OCPPv2_1.NetworkingNode.OnBinaryRequestMessageReceivedDelegate binaryHold  = (timestamp, sender, connection, request, cancellationToken) => Task.WhenAny(all.Task, Task.Delay(TimeSpan.FromSeconds(5)));

            localController!.OCPP.OUT.OnJSONRequestMessageSent   += jsonSent;
            localController!.OCPP.OUT.OnBinaryRequestMessageSent += binarySent;
            testCSMS1!.      OCPP.IN. OnJSONRequestMessageReceived   += jsonHold;
            testCSMS1!.      OCPP.IN. OnBinaryRequestMessageReceived += binaryHold;

            return () => {
                localController!.OCPP.OUT.OnJSONRequestMessageSent   -= jsonSent;
                localController!.OCPP.OUT.OnBinaryRequestMessageSent -= binarySent;
                testCSMS1!.      OCPP.IN. OnJSONRequestMessageReceived   -= jsonHold;
                testCSMS1!.      OCPP.IN. OnBinaryRequestMessageReceived -= binaryHold;
            };

        }

        #endregion


        #region TwoStations_AskingWithTheSameRequestId_EachGetTheirOwnAnswer()

        /// <summary>
        /// Two stations behind the controller send their BootNotifications with
        /// the same request id, both on their way at once: each is answered, and
        /// each with the answer to its own request.
        /// </summary>
        [Test]
        public async Task TwoStations_AskingWithTheSameRequestId_EachGetTheirOwnAnswer()
        {

            var cs1      = await AStation("CS-STD-01");
            var cs2      = await AStation("CS-STD-02");

            var sameId   = Request_Id.Parse("4711");
            var release  = HoldTheCSMSUntilTheControllerSent(2);

            try
            {

                var boots = await Task.WhenAll(
                                cs1.SendBootNotification(BootReason.PowerUp,       RequestId: sameId, RequestTimeout: TimeSpan.FromSeconds(10)),
                                cs2.SendBootNotification(BootReason.ApplicationReset, RequestId: sameId, RequestTimeout: TimeSpan.FromSeconds(10))
                            );

                Assert.Multiple(() => {

                    Assert.That(boots[0].Result.ResultCode,  Is.EqualTo(ResultCode.OK), $"CS-STD-01: {boots[0].Result}");
                    Assert.That(boots[0].Status,             Is.EqualTo(RegistrationStatus.Accepted));
                    Assert.That(boots[0].Request.RequestId,  Is.EqualTo(sameId));

                    Assert.That(boots[1].Result.ResultCode,  Is.EqualTo(ResultCode.OK), $"CS-STD-02: {boots[1].Result}");
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

        #region TheControllersOwnRequest_AndAForwardedOne_WithTheSameId_EachGetTheirOwnAnswer()

        /// <summary>
        /// The controller asks the CSMS for itself while a station's request
        /// with the same id is on its way through it: the controller gets the
        /// answer to its heartbeat, the station the one to its BootNotification.
        /// </summary>
        [Test]
        public async Task TheControllersOwnRequest_AndAForwardedOne_WithTheSameId_EachGetTheirOwnAnswer()
        {

            var cs1      = await AStation("CS-STD-03");

            var sameId   = Request_Id.Parse("4712");
            var release  = HoldTheCSMSUntilTheControllerSent(2);

            try
            {

                var boot       = cs1.            SendBootNotification(BootReason.PowerUp, RequestId: sameId, RequestTimeout: TimeSpan.FromSeconds(10));
                var heartbeat  = localController!.SendHeartbeat       (                   RequestId: sameId, RequestTimeout: TimeSpan.FromSeconds(10));

                await Task.WhenAll(boot, heartbeat);

                Assert.Multiple(() => {

                    Assert.That(boot.Result.Result.ResultCode,       Is.EqualTo(ResultCode.OK), $"The station: {boot.Result.Result}");
                    Assert.That(boot.Result.Status,                  Is.EqualTo(RegistrationStatus.Accepted));

                    Assert.That(heartbeat.Result.Result.ResultCode,  Is.EqualTo(ResultCode.OK), $"The controller: {heartbeat.Result.Result}");
                    Assert.That((heartbeat.Result.CurrentTime - Timestamp.Now).Duration(), Is.LessThan(TimeSpan.FromMinutes(1)));

                });

            }
            finally
            {
                release();
            }

        }

        #endregion

        #region TwoStations_TransferringBinaryDataWithTheSameRequestId_EachGetTheirOwnAnswer()

        /// <summary>
        /// The same for binary messages: two stations' binary data transfers
        /// with the same request id, both on their way at once, each answered
        /// with its own data reversed.
        /// </summary>
        [Test]
        public async Task TwoStations_TransferringBinaryDataWithTheSameRequestId_EachGetTheirOwnAnswer()
        {

            var cs1      = await AStation("CS-STD-04");
            var cs2      = await AStation("CS-STD-05");

            var sameId   = Request_Id.Parse("4713");
            var data1    = "Hello from 04".ToUTF8Bytes();
            var data2    = "Hello from 05".ToUTF8Bytes();
            var release  = HoldTheCSMSUntilTheControllerSent(2);

            try
            {

                var transfers = await Task.WhenAll(
                                    cs1.TransferBinaryData(VendorId: Vendor_Id.GraphDefined, MessageId: Message_Id.GraphDefined_TestMessage, Data: data1,
                                                           RequestId: sameId, RequestTimeout: TimeSpan.FromSeconds(10), SerializationFormat: SerializationFormats.BinaryTextIds),
                                    cs2.TransferBinaryData(VendorId: Vendor_Id.GraphDefined, MessageId: Message_Id.GraphDefined_TestMessage, Data: data2,
                                                           RequestId: sameId, RequestTimeout: TimeSpan.FromSeconds(10), SerializationFormat: SerializationFormats.BinaryTextIds)
                                );

                Assert.Multiple(() => {

                    Assert.That(transfers[0].Result.ResultCode,     Is.EqualTo(ResultCode.OK), $"CS-STD-04: {transfers[0].Result}");
                    Assert.That(transfers[0].Data?.ToUTF8String(),  Is.EqualTo(data1.Reverse().ToUTF8String()));

                    Assert.That(transfers[1].Result.ResultCode,     Is.EqualTo(ResultCode.OK), $"CS-STD-05: {transfers[1].Result}");
                    Assert.That(transfers[1].Data?.ToUTF8String(),  Is.EqualTo(data2.Reverse().ToUTF8String()));

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
