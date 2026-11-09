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

        #region AStationsHeartbeat_AndStatusNotification_ThroughTheController_AreAnsweredByTheCSMS()

        /// <summary>
        /// A heartbeat and a status notification - requests the controller has
        /// no filter of its own for - are forwarded up and answered by the
        /// CSMS, not turned away by the controller as "Filtered".
        /// </summary>
        [Test]
        public async Task AStationsHeartbeat_AndStatusNotification_ThroughTheController_AreAnsweredByTheCSMS()
        {

            var cs1        = await AStation("CS-STD-06");
            var atTheCSMS  = new ConcurrentQueue<String>();

            OCPPv2_1.NetworkingNode.OnJSONRequestMessageReceivedDelegate seen = (timestamp, sender, connection, request, cancellationToken) => {
                atTheCSMS.Enqueue(request.Action);
                return Task.CompletedTask;
            };

            testCSMS1!.OCPP.IN.OnJSONRequestMessageReceived += seen;

            try
            {

                var heartbeat  = await cs1.SendHeartbeat(
                                           RequestTimeout:  TimeSpan.FromSeconds(10)
                                       );

                var status     = await cs1.SendStatusNotification(
                                           EVSEId:          EVSE_Id.     Parse(1),
                                           ConnectorId:     Connector_Id.Parse(1),
                                           Timestamp:       Timestamp.Now,
                                           Status:          ConnectorStatus.Available,
                                           RequestTimeout:  TimeSpan.FromSeconds(10)
                                       );

                Assert.Multiple(() => {
                    Assert.That(heartbeat.Result.ResultCode,  Is.EqualTo(ResultCode.OK), $"The heartbeat: {heartbeat.Result}");
                    Assert.That(status.   Result.ResultCode,  Is.EqualTo(ResultCode.OK), $"The status notification: {status.Result}");
                    Assert.That(atTheCSMS,                    Does.Contain("Heartbeat"),          "The heartbeat did not reach the CSMS.");
                    Assert.That(atTheCSMS,                    Does.Contain("StatusNotification"), "The status notification did not reach the CSMS.");
                });

            }
            finally
            {
                testCSMS1!.OCPP.IN.OnJSONRequestMessageReceived -= seen;
            }

        }

        #endregion


        #region (private) Waited(Started)

        /// <summary>
        /// How long a station waited for what came back: an error that is lost
        /// on its way leaves it waiting for its whole request timeout.
        /// </summary>
        private static TimeSpan Waited(DateTimeOffset Started)
            => Timestamp.Now - Started;

        #endregion

        #region ARequestError_ToAForwardedRequest_GoesBackToTheStationThatAsked()

        /// <summary>
        /// The CSMS cannot read a station's request the controller forwarded,
        /// and answers it with a request error: the error goes back down to the
        /// station, under the station's own request id, at once - rather than
        /// being taken by the controller for an error to a request of its own
        /// and dropped, which left the station waiting out its timeout.
        /// </summary>
        [Test]
        public async Task ARequestError_ToAForwardedRequest_GoesBackToTheStationThatAsked()
        {

            var cs1 = await AStation("CS-STD-07");

            testCSMS1!.OCPP.CustomDataTransferRequestParser = (json, request) => throw new ArgumentException("This CSMS cannot read it!");

            try
            {

                var started   = Timestamp.Now;

                var transfer  = await cs1.TransferData(
                                          VendorId:        Vendor_Id. GraphDefined,
                                          MessageId:       Message_Id.GraphDefined_TestMessage,
                                          Data:            "Hello world!",
                                          RequestId:       Request_Id.Parse("4714"),
                                          RequestTimeout:  TimeSpan.FromSeconds(10)
                                      );

                Assert.Multiple(() => {
                    Assert.That(transfer.Result.ResultCode,  Is.EqualTo(ResultCode.FormationViolation),  $"{transfer.Result}");
                    Assert.That(transfer.Result.ResultCode,  Is.Not.EqualTo(ResultCode.Timeout), $"The CSMS's request error did not reach the station: {transfer.Result}");
                    Assert.That(transfer.Request.RequestId,  Is.EqualTo(Request_Id.Parse("4714")));
                    Assert.That(Waited(started),             Is.LessThan(TimeSpan.FromSeconds(5)));
                });

            }
            finally
            {
                testCSMS1!.OCPP.CustomDataTransferRequestParser = null;
            }

        }

        #endregion

        #region AResponseError_ToAForwardedRequest_GoesBackToTheStationThatAsked()

        /// <summary>
        /// The CSMS answers a station's forwarded request with a response error:
        /// it goes back down to the station, as an answer does, at once.
        /// </summary>
        [Test]
        public async Task AResponseError_ToAForwardedRequest_GoesBackToTheStationThatAsked()
        {

            var cs1 = await AStation("CS-STD-08");

            // Before the CSMS answers the request itself, which then comes too
            // late: the controller has nobody waiting for it any more.
            OCPPv2_1.NetworkingNode.OnJSONRequestMessageReceivedDelegate answerWithAnError = async (timestamp, sender, connection, request, cancellationToken) => {

                if (request.Action == "DataTransfer")
                    await testCSMS1!.OCPP.OUT.SendJSONResponseError(
                              new OCPP_JSONResponseErrorMessage(
                                  Timestamp.Now,
                                  EventTracking_Id.New,
                                  NetworkingMode.Standard,
                                  SourceRouting.To(request.NetworkPath.Source),
                                  NetworkPath.From(testCSMS1.Id),
                                  request.RequestId,
                                  ResultCode.InternalError,
                                  "The CSMS could not answer it!"
                              )
                          );

            };

            testCSMS1!.OCPP.IN.OnJSONRequestMessageReceived += answerWithAnError;

            try
            {

                var started   = Timestamp.Now;

                var transfer  = await cs1.TransferData(
                                          VendorId:        Vendor_Id. GraphDefined,
                                          MessageId:       Message_Id.GraphDefined_TestMessage,
                                          Data:            "Hello world!",
                                          RequestId:       Request_Id.Parse("4715"),
                                          RequestTimeout:  TimeSpan.FromSeconds(10)
                                      );

                // Not the CSMS's code: what the asker makes of a response error says
                // GenericError, whoever sent it (Result.FromSendRequestState reads
                // request errors only).
                Assert.Multiple(() => {
                    Assert.That(transfer.Result.ResultCode,  Is.Not.EqualTo(ResultCode.OK),      $"{transfer.Result}");
                    Assert.That(transfer.Result.ResultCode,  Is.Not.EqualTo(ResultCode.Timeout), $"The CSMS's response error did not reach the station: {transfer.Result}");
                    Assert.That(Waited(started),             Is.LessThan(TimeSpan.FromSeconds(5)));
                });

            }
            finally
            {
                testCSMS1!.OCPP.IN.OnJSONRequestMessageReceived -= answerWithAnError;
            }

        }

        #endregion

        #region ARequestError_ToAForwardedBinaryRequest_GoesBackToTheStationThatAsked()

        /// <summary>
        /// The same for a binary request the CSMS cannot read, which it answers
        /// - as it answers any request it cannot read - with a request error.
        /// </summary>
        [Test]
        public async Task ARequestError_ToAForwardedBinaryRequest_GoesBackToTheStationThatAsked()
        {

            var cs1 = await AStation("CS-STD-09");

            testCSMS1!.OCPP.CustomBinaryDataTransferRequestParser = (binary, request) => throw new ArgumentException("This CSMS cannot read it!");

            try
            {

                var started   = Timestamp.Now;

                var transfer  = await cs1.TransferBinaryData(
                                          VendorId:             Vendor_Id. GraphDefined,
                                          MessageId:            Message_Id.GraphDefined_TestMessage,
                                          Data:                 "Hello world!".ToUTF8Bytes(),
                                          RequestId:            Request_Id.Parse("4716"),
                                          RequestTimeout:       TimeSpan.FromSeconds(10),
                                          SerializationFormat:  SerializationFormats.BinaryTextIds
                                      );

                Assert.Multiple(() => {
                    Assert.That(transfer.Result.ResultCode,  Is.EqualTo(ResultCode.FormationViolation),  $"{transfer.Result}");
                    Assert.That(transfer.Result.ResultCode,  Is.Not.EqualTo(ResultCode.Timeout), $"The CSMS's request error did not reach the station: {transfer.Result}");
                    Assert.That(Waited(started),             Is.LessThan(TimeSpan.FromSeconds(5)));
                });

            }
            finally
            {
                testCSMS1!.OCPP.CustomBinaryDataTransferRequestParser = null;
            }

        }

        #endregion

    }

}
