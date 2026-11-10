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

using Newtonsoft.Json.Linq;

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

        #region AResponseError_IsNoAnswer_TheStationGetsTheCSMSsAnswer()

        /// <summary>
        /// The CSMS sends a response error under the id of a station's request that the
        /// controller forwarded, before it answers that request: the station gets the
        /// CSMS's answer, not the error.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A response error - a CALLRESULTERROR in OCPP 2.1 - answers no request. It is
        /// "sent back on receipt of a CALLRESULT that contains errors", and its id is the
        /// one of that CALLRESULT (OCPP 2.1 Edition 2, Part 4, 4.2.3 and 4.1.4).
        /// </para>
        /// <para>
        /// The controller used to take it for the answer to the request it had forwarded
        /// under that id: the station heard the error, and the CSMS's answer after it
        /// found nobody waiting.
        /// </para>
        /// </remarks>
        [Test]
        public async Task AResponseError_IsNoAnswer_TheStationGetsTheCSMSsAnswer()
        {

            var cs1 = await AStation("CS-STD-08");

            OCPPv2_1.NetworkingNode.OnJSONRequestMessageReceivedDelegate anErrorFirst = async (timestamp, sender, connection, request, cancellationToken) => {

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
                                  "This is no answer!"
                              )
                          );

            };

            testCSMS1!.OCPP.IN.OnJSONRequestMessageReceived += anErrorFirst;

            try
            {

                var transfer = await cs1.TransferData(
                                         VendorId:        Vendor_Id. GraphDefined,
                                         MessageId:       Message_Id.GraphDefined_TestMessage,
                                         Data:            "Hello world!",
                                         RequestId:       Request_Id.Parse("4715"),
                                         RequestTimeout:  TimeSpan.FromSeconds(10)
                                     );

                Assert.Multiple(() => {
                    Assert.That(transfer.Result.ResultCode,  Is.EqualTo(ResultCode.OK),  $"{transfer.Result}");
                    Assert.That(transfer.Data?.ToString(),   Is.EqualTo("!dlrow olleH"));
                });

            }
            finally
            {
                testCSMS1!.OCPP.IN.OnJSONRequestMessageReceived -= anErrorFirst;
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


        #region ABinaryRequestError_ToAForwardedBinaryRequest_GoesBackToTheStationThatAsked()

        /// <summary>
        /// The CSMS answers a station's forwarded binary request with a binary request
        /// error: the controller reads it as one, sends it on down, and the station hears
        /// the CSMS's code and description at once.
        /// </summary>
        /// <remarks>
        /// A binary error used to be written as JSON text into a binary frame. The
        /// controller's binary parsers took it for nothing they knew, and the station
        /// waited out its whole timeout.
        /// </remarks>
        [Test]
        public async Task ABinaryRequestError_ToAForwardedBinaryRequest_GoesBackToTheStationThatAsked()
        {

            var cs1 = await AStation("CS-STD-10");

            OCPPv2_1.NetworkingNode.OnBinaryRequestMessageReceivedDelegate answerWithABinaryError = async (timestamp, sender, connection, request, cancellationToken) => {

                if (request.Action == "BinaryDataTransfer")
                    await testCSMS1!.OCPP.OUT.SendBinaryRequestError(
                              new OCPP_BinaryRequestErrorMessage(
                                  Timestamp.Now,
                                  EventTracking_Id.New,
                                  NetworkingMode.Standard,
                                  SourceRouting.To(request.NetworkPath.Source),
                                  NetworkPath.From(testCSMS1.Id),
                                  request.RequestId,
                                  ResultCode.SecurityError,
                                  "The CSMS does not take this from you!"
                              )
                          );

            };

            testCSMS1!.OCPP.IN.OnBinaryRequestMessageReceived += answerWithABinaryError;

            try
            {

                var started   = Timestamp.Now;

                var transfer  = await cs1.TransferBinaryData(
                                          VendorId:             Vendor_Id. GraphDefined,
                                          MessageId:            Message_Id.GraphDefined_TestMessage,
                                          Data:                 "Hello world!".ToUTF8Bytes(),
                                          RequestId:            Request_Id.Parse("4720"),
                                          RequestTimeout:       TimeSpan.FromSeconds(10),
                                          SerializationFormat:  SerializationFormats.BinaryTextIds
                                      );

                Assert.Multiple(() => {
                    Assert.That(transfer.Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),  $"{transfer.Result}");
                    Assert.That(transfer.Result.Description,  Is.EqualTo("The CSMS does not take this from you!"));
                    Assert.That(transfer.Request.RequestId,   Is.EqualTo(Request_Id.Parse("4720")));
                    Assert.That(Waited(started),              Is.LessThan(TimeSpan.FromSeconds(5)));
                });

            }
            finally
            {
                testCSMS1!.OCPP.IN.OnBinaryRequestMessageReceived -= answerWithABinaryError;
            }

        }

        #endregion

        #region ABinaryResponseError_IsNoAnswer_TheStationGetsTheCSMSsAnswer()

        /// <summary>
        /// The same for a binary response error: the station gets the CSMS's answer to
        /// its binary data transfer, not the error.
        /// </summary>
        [Test]
        public async Task ABinaryResponseError_IsNoAnswer_TheStationGetsTheCSMSsAnswer()
        {

            var cs1 = await AStation("CS-STD-11");

            OCPPv2_1.NetworkingNode.OnBinaryRequestMessageReceivedDelegate aBinaryErrorFirst = async (timestamp, sender, connection, request, cancellationToken) => {

                if (request.Action == "BinaryDataTransfer")
                    await testCSMS1!.OCPP.OUT.SendBinaryResponseError(
                              new OCPP_BinaryResponseErrorMessage(
                                  Timestamp.Now,
                                  EventTracking_Id.New,
                                  NetworkingMode.Standard,
                                  SourceRouting.To(request.NetworkPath.Source),
                                  NetworkPath.From(testCSMS1.Id),
                                  request.RequestId,
                                  ResultCode.InternalError,
                                  "This is no answer!"
                              )
                          );

            };

            testCSMS1!.OCPP.IN.OnBinaryRequestMessageReceived += aBinaryErrorFirst;

            try
            {

                var data      = "Hello world!".ToUTF8Bytes();

                var transfer  = await cs1.TransferBinaryData(
                                          VendorId:             Vendor_Id. GraphDefined,
                                          MessageId:            Message_Id.GraphDefined_TestMessage,
                                          Data:                 data,
                                          RequestId:            Request_Id.Parse("4721"),
                                          RequestTimeout:       TimeSpan.FromSeconds(10),
                                          SerializationFormat:  SerializationFormats.BinaryTextIds
                                      );

                Assert.Multiple(() => {
                    Assert.That(transfer.Result.ResultCode,     Is.EqualTo(ResultCode.OK),  $"{transfer.Result}");
                    Assert.That(transfer.Data?.ToUTF8String(),  Is.EqualTo(data.Reverse().ToUTF8String()));
                });

            }
            finally
            {
                testCSMS1!.OCPP.IN.OnBinaryRequestMessageReceived -= aBinaryErrorFirst;
            }

        }

        #endregion

        #region AStationsResponseError_ToAnAnswerOfTheCSMS_ReachesTheCSMS(Format)

        /// <summary>
        /// A station cannot read the CSMS's answer to a request the controller forwarded,
        /// and says so with a response error, in JSON and in binary: the controller sends
        /// it on to the CSMS, under the id the CSMS answered.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A response error goes from whoever asked to whoever answered (OCPP 2.1 Edition 2,
        /// Part 4, 4.2.3), under the id of the answer it is about (4.1.4). The station's id
        /// is not the CSMS's: the controller forwarded the request under one of its own.
        /// </para>
        /// <para>
        /// The controller looked the station's id up among the requests it still waited
        /// for an answer to, found none, and kept the error.
        /// </para>
        /// </remarks>
        [TestCase("JSON")]
        [TestCase("Binary")]
        public async Task AStationsResponseError_ToAnAnswerOfTheCSMS_ReachesTheCSMS(String Format)
        {

            var cs1         = await AStation(Format == "JSON" ? "CS-STD-17" : "CS-STD-18");
            var stationsId  = Request_Id.Parse(Format == "JSON" ? "4730" : "4731");

            var answered    = new TaskCompletionSource<Request_Id>(TaskCreationOptions.RunContinuationsAsynchronously);
            var heard       = new TaskCompletionSource<(Request_Id RequestId, ResultCode ErrorCode, String? ErrorDescription)>(TaskCreationOptions.RunContinuationsAsynchronously);

            OCPPv2_1.NetworkingNode.OnJSONRequestMessageReceivedDelegate         jsonAsked     = (timestamp, sender, connection, request, cancellationToken) => {
                if (request.Action == "DataTransfer")
                    answered.TrySetResult(request.RequestId);
                return Task.CompletedTask;
            };

            OCPPv2_1.NetworkingNode.OnBinaryRequestMessageReceivedDelegate       binaryAsked   = (timestamp, sender, connection, request, cancellationToken) => {
                if (request.Action == "BinaryDataTransfer")
                    answered.TrySetResult(request.RequestId);
                return Task.CompletedTask;
            };

            OCPPv2_1.NetworkingNode.OnJSONResponseErrorMessageReceivedDelegate   jsonHeard     = (timestamp, sender, connection, error, cancellationToken) => {
                heard.TrySetResult((error.RequestId, error.ErrorCode, error.ErrorDescription));
                return Task.CompletedTask;
            };

            OCPPv2_1.NetworkingNode.OnBinaryResponseErrorMessageReceivedDelegate binaryHeard   = (timestamp, sender, connection, error, cancellationToken) => {
                heard.TrySetResult((error.RequestId, error.ErrorCode, error.ErrorDescription));
                return Task.CompletedTask;
            };

            testCSMS1!.OCPP.IN.OnJSONRequestMessageReceived           += jsonAsked;
            testCSMS1!.OCPP.IN.OnBinaryRequestMessageReceived         += binaryAsked;
            testCSMS1!.OCPP.IN.OnJSONResponseErrorMessageReceived     += jsonHeard;
            testCSMS1!.OCPP.IN.OnBinaryResponseErrorMessageReceived   += binaryHeard;

            try
            {

                if (Format == "JSON")
                {

                    var transfer = await cs1.TransferData(
                                             VendorId:        Vendor_Id. GraphDefined,
                                             MessageId:       Message_Id.GraphDefined_TestMessage,
                                             Data:            "Hello world!",
                                             RequestId:       stationsId,
                                             RequestTimeout:  TimeSpan.FromSeconds(10)
                                         );

                    Assert.That(transfer.Result.ResultCode, Is.EqualTo(ResultCode.OK), $"{transfer.Result}");

                    await cs1.OCPP.OUT.SendJSONResponseError(
                              new OCPP_JSONResponseErrorMessage(
                                  Timestamp.Now,
                                  EventTracking_Id.New,
                                  NetworkingMode.Standard,
                                  SourceRouting.CSMS,
                                  NetworkPath.From(cs1.Id),
                                  stationsId,
                                  ResultCode.FormationViolation,
                                  "The station cannot read this answer!"
                              )
                          );

                }

                else
                {

                    var transfer = await cs1.TransferBinaryData(
                                             VendorId:             Vendor_Id. GraphDefined,
                                             MessageId:            Message_Id.GraphDefined_TestMessage,
                                             Data:                 "Hello world!".ToUTF8Bytes(),
                                             RequestId:            stationsId,
                                             RequestTimeout:       TimeSpan.FromSeconds(10),
                                             SerializationFormat:  SerializationFormats.BinaryTextIds
                                         );

                    Assert.That(transfer.Result.ResultCode, Is.EqualTo(ResultCode.OK), $"{transfer.Result}");

                    await cs1.OCPP.OUT.SendBinaryResponseError(
                              new OCPP_BinaryResponseErrorMessage(
                                  Timestamp.Now,
                                  EventTracking_Id.New,
                                  NetworkingMode.Standard,
                                  SourceRouting.CSMS,
                                  NetworkPath.From(cs1.Id),
                                  stationsId,
                                  ResultCode.FormationViolation,
                                  "The station cannot read this answer!"
                              )
                          );

                }

                var arrived = await Task.WhenAny(heard.Task, Task.Delay(TimeSpan.FromSeconds(5))) == heard.Task;

                Assert.That(arrived, Is.True, "The station's response error did not reach the CSMS.");

                Assert.Multiple(() => {
                    Assert.That(heard.Task.Result.RequestId,         Is.EqualTo(answered.Task.Result), "Not under the id the CSMS answered.");
                    Assert.That(heard.Task.Result.ErrorCode,         Is.EqualTo(ResultCode.FormationViolation));
                    Assert.That(heard.Task.Result.ErrorDescription,  Is.EqualTo("The station cannot read this answer!"));
                });

            }
            finally
            {
                testCSMS1!.OCPP.IN.OnJSONRequestMessageReceived           -= jsonAsked;
                testCSMS1!.OCPP.IN.OnBinaryRequestMessageReceived         -= binaryAsked;
                testCSMS1!.OCPP.IN.OnJSONResponseErrorMessageReceived     -= jsonHeard;
                testCSMS1!.OCPP.IN.OnBinaryResponseErrorMessageReceived   -= binaryHeard;
            }

        }

        #endregion

        #region AResponseError_DoesNotAnswerTheControllersOwnRequest(Format)

        /// <summary>
        /// The CSMS sends the controller a response error under the id of a request the
        /// controller is still waiting for an answer to, in JSON and in binary: the
        /// controller keeps waiting, and gets what the CSMS answers - here the refusal of
        /// an action it does not know.
        /// </summary>
        /// <remarks>
        /// A response error is about an answer the controller sent, under the id of the
        /// request that answer was to. Every node numbers its requests from the same start,
        /// so that id is one of the controller's own as often as not. The error used to
        /// end the controller's own request with that id.
        /// </remarks>
        [TestCase("JSON")]
        [TestCase("Binary")]
        public async Task AResponseError_DoesNotAnswerTheControllersOwnRequest(String Format)
        {

            var requestId  = Request_Id.NewRandom();

            OCPPv2_1.NetworkingNode.OnJSONRequestMessageReceivedDelegate   jsonErrorFirst    = async (timestamp, sender, connection, request, cancellationToken) => {

                if (request.RequestId == requestId)
                {

                    await testCSMS1!.OCPP.OUT.SendJSONResponseError(
                              new OCPP_JSONResponseErrorMessage(
                                  Timestamp.Now,
                                  EventTracking_Id.New,
                                  NetworkingMode.Standard,
                                  SourceRouting.To(request.NetworkPath.Source),
                                  NetworkPath.From(testCSMS1.Id),
                                  request.RequestId,
                                  ResultCode.InternalError,
                                  "This is no answer!"
                              )
                          );

                    // Its answer only later: a node that still took the error for one
                    // would have it, alone, long before.
                    await Task.Delay(TimeSpan.FromSeconds(1));

                }

            };

            OCPPv2_1.NetworkingNode.OnBinaryRequestMessageReceivedDelegate binaryErrorFirst  = async (timestamp, sender, connection, request, cancellationToken) => {

                if (request.RequestId == requestId)
                {

                    await testCSMS1!.OCPP.OUT.SendBinaryResponseError(
                              new OCPP_BinaryResponseErrorMessage(
                                  Timestamp.Now,
                                  EventTracking_Id.New,
                                  NetworkingMode.Standard,
                                  SourceRouting.To(request.NetworkPath.Source),
                                  NetworkPath.From(testCSMS1.Id),
                                  request.RequestId,
                                  ResultCode.InternalError,
                                  "This is no answer!"
                              )
                          );

                    // Its answer only later: a node that still took the error for one
                    // would have it, alone, long before.
                    await Task.Delay(TimeSpan.FromSeconds(1));

                }

            };

            testCSMS1!.OCPP.IN.OnJSONRequestMessageReceived   += jsonErrorFirst;
            testCSMS1!.OCPP.IN.OnBinaryRequestMessageReceived += binaryErrorFirst;

            try
            {

                var state = Format == "JSON"

                                ? await localController!.OCPP.OUT.SendJSONRequestAndWait(
                                            new OCPP_JSONRequestMessage(
                                                RequestTimestamp:  Timestamp.Now,
                                                EventTrackingId:   EventTracking_Id.New,
                                                NetworkingMode:    NetworkingMode.Standard,
                                                Destination:       SourceRouting.CSMS,
                                                NetworkPath:       NetworkPath.From(localController.Id),
                                                RequestId:         requestId,
                                                Action:            "NoSuchAction",
                                                Payload:           new JObject(),
                                                RequestTimeout:    Timestamp.Now + TimeSpan.FromSeconds(10)
                                            )
                                        )

                                : await localController!.OCPP.OUT.SendBinaryRequestAndWait(
                                            new OCPP_BinaryRequestMessage(
                                                RequestTimestamp:  Timestamp.Now,
                                                EventTrackingId:   EventTracking_Id.New,
                                                NetworkingMode:    NetworkingMode.Standard,
                                                Destination:       SourceRouting.CSMS,
                                                NetworkPath:       NetworkPath.From(localController.Id),
                                                RequestId:         requestId,
                                                Action:            "NoSuchBinaryAction",
                                                Payload:           "Hello world!".ToUTF8Bytes(),
                                                RequestTimeout:    Timestamp.Now + TimeSpan.FromSeconds(10)
                                            )
                                        );

                Assert.That(state.JSONRequestErrorMessage?.ErrorCode,  Is.EqualTo(ResultCode.ProtocolError),
                            $"The CSMS's refusal was not what the controller got: {state.JSONRequestErrorMessage?.ErrorCode}");

            }
            finally
            {
                testCSMS1!.OCPP.IN.OnJSONRequestMessageReceived   -= jsonErrorFirst;
                testCSMS1!.OCPP.IN.OnBinaryRequestMessageReceived -= binaryErrorFirst;
            }

        }

        #endregion

        #region AForwardedRequest_IsCountedAsForwarded_AndNotAsDropped()

        /// <summary>
        /// A request the controller forwards counts as forwarded, and as nothing else.
        /// </summary>
        /// <remarks>
        /// REPLACE used to begin a chain of its own after the FORWARD branch, whose last
        /// else counted every forwarded request as dropped, too.
        /// </remarks>
        [Test]
        public async Task AForwardedRequest_IsCountedAsForwarded_AndNotAsDropped()
        {

            var cs1        = await AStation("CS-STD-12");

            var forwarded  = localController!.OCPP.FORWARD.ForwardedMessages;
            var replaced   = localController!.OCPP.FORWARD.ReplacedMessages;
            var rejected   = localController!.OCPP.FORWARD.RejectedMessages;
            var dropped    = localController!.OCPP.FORWARD.DroppedMessages;

            var boot       = await cs1.SendBootNotification(BootReason.PowerUp, RequestTimeout: TimeSpan.FromSeconds(10));

            Assert.Multiple(() => {
                Assert.That(boot.Result.ResultCode,                              Is.EqualTo(ResultCode.OK), $"{boot.Result}");
                Assert.That(localController.OCPP.FORWARD.ForwardedMessages,     Is.EqualTo(forwarded + 1), "forwarded");
                Assert.That(localController.OCPP.FORWARD.ReplacedMessages,      Is.EqualTo(replaced),      "replaced");
                Assert.That(localController.OCPP.FORWARD.RejectedMessages,      Is.EqualTo(rejected),      "rejected");
                Assert.That(localController.OCPP.FORWARD.DroppedMessages,       Is.EqualTo(dropped),       "dropped");
            });

        }

        #endregion

        #region ARequestTheControllerRejects_WithoutARejectResponse_IsRefusedToTheStation()

        /// <summary>
        /// A filter of the controller REJECTs a station's request without saying what to
        /// answer: the station is told so at once, and the request counts as rejected.
        /// </summary>
        /// <remarks>
        /// A REJECT without a reject response used to fall through to DROP: no answer,
        /// and the station waited out its whole timeout.
        /// </remarks>
        [Test]
        public async Task ARequestTheControllerRejects_WithoutARejectResponse_IsRefusedToTheStation()
        {

            var cs1 = await AStation("CS-STD-13");

            OCPPv2_1.NetworkingNode.OnAnyJSONRequestFilterDelegate reject = (timestamp, sender, connection, request, cancellationToken) =>
                Task.FromResult(
                    request.Action == "DataTransfer"
                        ? OCPPv2_1.NetworkingNode.RequestForwardingDecision.REJECT("Not through this controller!")
                        : OCPPv2_1.NetworkingNode.RequestForwardingDecision.NEXT(request)
                );

            var rejected  = localController!.OCPP.FORWARD.RejectedMessages;
            var dropped   = localController!.OCPP.FORWARD.DroppedMessages;

            localController!.OCPP.FORWARD.OnAnyJSONRequestFilter += reject;

            try
            {

                var started   = Timestamp.Now;

                var transfer  = await cs1.TransferData(
                                          VendorId:        Vendor_Id. GraphDefined,
                                          MessageId:       Message_Id.GraphDefined_TestMessage,
                                          Data:            "Hello world!",
                                          RequestId:       Request_Id.Parse("4722"),
                                          RequestTimeout:  TimeSpan.FromSeconds(10)
                                      );

                Assert.Multiple(() => {
                    Assert.That(transfer.Result.ResultCode,                       Is.EqualTo(ResultCode.Filtered),  $"{transfer.Result}");
                    Assert.That(transfer.Result.Description,                      Is.EqualTo("Not through this controller!"));
                    Assert.That(Waited(started),                                  Is.LessThan(TimeSpan.FromSeconds(5)));
                    Assert.That(localController.OCPP.FORWARD.RejectedMessages,   Is.EqualTo(rejected + 1), "rejected");
                    Assert.That(localController.OCPP.FORWARD.DroppedMessages,    Is.EqualTo(dropped),      "dropped");
                });

            }
            finally
            {
                localController!.OCPP.FORWARD.OnAnyJSONRequestFilter -= reject;
            }

        }

        #endregion

        #region ARequestTheControllerCannotRead_IsRefusedToTheStation()

        /// <summary>
        /// The controller cannot read a station's request it was to forward: the station
        /// is told so at once, with the reason.
        /// </summary>
        /// <remarks>
        /// A forwarding processor that cannot read a request REJECTs it without a reject
        /// response, which used to mean no answer at all.
        /// </remarks>
        [Test]
        public async Task ARequestTheControllerCannotRead_IsRefusedToTheStation()
        {

            var cs1 = await AStation("CS-STD-14");

            localController!.OCPP.CustomDataTransferRequestParser = (json, request) => throw new ArgumentException("This controller cannot read it!");

            try
            {

                var started   = Timestamp.Now;

                var transfer  = await cs1.TransferData(
                                          VendorId:        Vendor_Id. GraphDefined,
                                          MessageId:       Message_Id.GraphDefined_TestMessage,
                                          Data:            "Hello world!",
                                          RequestId:       Request_Id.Parse("4723"),
                                          RequestTimeout:  TimeSpan.FromSeconds(10)
                                      );

                Assert.Multiple(() => {
                    Assert.That(transfer.Result.ResultCode,   Is.EqualTo(ResultCode.Filtered),  $"{transfer.Result}");
                    Assert.That(transfer.Result.Description,  Does.Contain("This controller cannot read it!"));
                    Assert.That(Waited(started),              Is.LessThan(TimeSpan.FromSeconds(5)));
                });

            }
            finally
            {
                localController!.OCPP.CustomDataTransferRequestParser = null;
            }

        }

        #endregion

        #region AForwardedBinaryRequest_IsCountedAsForwarded_AndNotAsDropped()

        /// <summary>
        /// A binary request the controller forwards counts as forwarded, and as nothing else.
        /// </summary>
        /// <remarks>
        /// Binary requests were not counted at all, and their REPLACE began a chain of its
        /// own after the FORWARD branch, as for JSON requests.
        /// </remarks>
        [Test]
        public async Task AForwardedBinaryRequest_IsCountedAsForwarded_AndNotAsDropped()
        {

            var cs1        = await AStation("CS-STD-15");

            var forwarded  = localController!.OCPP.FORWARD.ForwardedMessages;
            var dropped    = localController!.OCPP.FORWARD.DroppedMessages;

            var transfer   = await cs1.TransferBinaryData(
                                       VendorId:             Vendor_Id. GraphDefined,
                                       MessageId:            Message_Id.GraphDefined_TestMessage,
                                       Data:                 "Hello world!".ToUTF8Bytes(),
                                       RequestTimeout:       TimeSpan.FromSeconds(10),
                                       SerializationFormat:  SerializationFormats.BinaryTextIds
                                   );

            Assert.Multiple(() => {
                Assert.That(transfer.Result.ResultCode,                          Is.EqualTo(ResultCode.OK), $"{transfer.Result}");
                Assert.That(localController.OCPP.FORWARD.ForwardedMessages,     Is.EqualTo(forwarded + 1), "forwarded");
                Assert.That(localController.OCPP.FORWARD.DroppedMessages,       Is.EqualTo(dropped),       "dropped");
            });

        }

        #endregion

        #region ABinaryRequestTheControllerRejects_WithoutARejectResponse_IsRefusedToTheStation()

        /// <summary>
        /// A filter of the controller REJECTs a station's binary request without saying
        /// what to answer: the station is told so at once, with a binary request error.
        /// </summary>
        [Test]
        public async Task ABinaryRequestTheControllerRejects_WithoutARejectResponse_IsRefusedToTheStation()
        {

            var cs1 = await AStation("CS-STD-16");

            OCPPv2_1.NetworkingNode.OnAnyBinaryRequestFilterDelegate reject = (timestamp, sender, connection, request, cancellationToken) =>
                Task.FromResult(
                    request.Action == "BinaryDataTransfer"
                        ? OCPPv2_1.NetworkingNode.RequestForwardingDecision.REJECT("Not through this controller!")
                        : new OCPPv2_1.NetworkingNode.RequestForwardingDecision(OCPP.NetworkingNode.ForwardingDecisions.NEXT)
                );

            var rejected  = localController!.OCPP.FORWARD.RejectedMessages;

            localController!.OCPP.FORWARD.OnAnyBinaryRequestFilter += reject;

            try
            {

                var started   = Timestamp.Now;

                var transfer  = await cs1.TransferBinaryData(
                                          VendorId:             Vendor_Id. GraphDefined,
                                          MessageId:            Message_Id.GraphDefined_TestMessage,
                                          Data:                 "Hello world!".ToUTF8Bytes(),
                                          RequestId:            Request_Id.Parse("4724"),
                                          RequestTimeout:       TimeSpan.FromSeconds(10),
                                          SerializationFormat:  SerializationFormats.BinaryTextIds
                                      );

                Assert.Multiple(() => {
                    Assert.That(transfer.Result.ResultCode,                       Is.EqualTo(ResultCode.Filtered),  $"{transfer.Result}");
                    Assert.That(transfer.Result.Description,                      Is.EqualTo("Not through this controller!"));
                    Assert.That(Waited(started),                                  Is.LessThan(TimeSpan.FromSeconds(5)));
                    Assert.That(localController.OCPP.FORWARD.RejectedMessages,   Is.EqualTo(rejected + 1), "rejected");
                });

            }
            finally
            {
                localController!.OCPP.FORWARD.OnAnyBinaryRequestFilter -= reject;
            }

        }

        #endregion

        #region AnUnknownAction_IsRefusedToTheControllerThatSentIt(Format)

        /// <summary>
        /// The controller asks its CSMS something the CSMS has no processor for, in JSON
        /// and in binary: the refusal comes back over the connection the CSMS accepted, at
        /// once.
        /// </summary>
        /// <remarks>
        /// The refusal was addressed to nobody. Over a client connection that did not
        /// matter (84b12cf2), but a server looks its connection up by the destination, and
        /// found none for nobody.
        /// </remarks>
        [TestCase("JSON")]
        [TestCase("Binary")]
        public async Task AnUnknownAction_IsRefusedToTheControllerThatSentIt(String Format)
        {

            var started  = Timestamp.Now;

            var state    = Format == "JSON"

                               ? await localController!.OCPP.OUT.SendJSONRequestAndWait(
                                           new OCPP_JSONRequestMessage(
                                               RequestTimestamp:  Timestamp.Now,
                                               EventTrackingId:   EventTracking_Id.New,
                                               NetworkingMode:    NetworkingMode.Standard,
                                               Destination:       SourceRouting.CSMS,
                                               NetworkPath:       NetworkPath.From(localController.Id),
                                               RequestId:         Request_Id.NewRandom(),
                                               Action:            "NoSuchAction",
                                               Payload:           new JObject(),
                                               RequestTimeout:    Timestamp.Now + TimeSpan.FromSeconds(10)
                                           )
                                       )

                               : await localController!.OCPP.OUT.SendBinaryRequestAndWait(
                                           new OCPP_BinaryRequestMessage(
                                               RequestTimestamp:  Timestamp.Now,
                                               EventTrackingId:   EventTracking_Id.New,
                                               NetworkingMode:    NetworkingMode.Standard,
                                               Destination:       SourceRouting.CSMS,
                                               NetworkPath:       NetworkPath.From(localController.Id),
                                               RequestId:         Request_Id.NewRandom(),
                                               Action:            "NoSuchBinaryAction",
                                               Payload:           "Hello world!".ToUTF8Bytes(),
                                               RequestTimeout:    Timestamp.Now + TimeSpan.FromSeconds(10)
                                           )
                                       );

            Assert.Multiple(() => {
                Assert.That(state.JSONRequestErrorMessage?.ErrorCode,  Is.EqualTo(ResultCode.ProtocolError),
                            $"The CSMS's refusal did not reach the controller: {state.JSONRequestErrorMessage?.ErrorCode}");
                Assert.That(Waited(started),                           Is.LessThan(TimeSpan.FromSeconds(5)));
            });

        }

        #endregion

    }

}
