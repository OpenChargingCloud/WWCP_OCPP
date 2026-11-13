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

using Newtonsoft.Json.Linq;

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

        #region Encodings

        /// <summary>
        /// How the charge point speaks to the node that forwards.
        /// </summary>
        protected virtual MessageEncodings Downlink
            => MessageEncodings.JSON;

        /// <summary>
        /// How the node that forwards speaks to the central system.
        /// </summary>
        protected virtual MessageEncodings Uplink
            => MessageEncodings.JSON;

        #endregion

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
                               SecWebSocketProtocols:    OCPPSubprotocols.Offer([ Version.WebSocketSubProtocolId ], Uplink),
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
                                     SecWebSocketProtocols:    OCPPSubprotocols.Offer([ Version.WebSocketSubProtocolId ], Downlink),
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

        #region (private) Waited(Started)

        /// <summary>
        /// How long a charge point waited for what came back: an error that is
        /// lost on its way leaves it waiting for its whole request timeout.
        /// </summary>
        private static TimeSpan Waited(DateTimeOffset Started)
            => Timestamp.Now - Started;

        #endregion

        #region ARequestError_ToAForwardedRequest_GoesBackToTheChargePointThatAsked()

        /// <summary>
        /// The central system answers a charge point's forwarded request with a
        /// request error: the error goes back down to the charge point, under
        /// its own request id, at once - rather than being taken by the node for
        /// an error to a request of its own and dropped.
        /// </summary>
        /// <remarks>
        /// The central system itself cannot read the request. Its request error
        /// used to be addressed to nobody - OCPPWebSocketAdapterIN of 1.6 had no
        /// AddressToSender, as 2.1 has - and over the connection the central
        /// system accepted it was never sent at all.
        /// </remarks>
        [Test]
        public async Task ARequestError_ToAForwardedRequest_GoesBackToTheChargePointThatAsked()
        {

            var cp1 = await AChargePointBelow("CP-STD-04");

            testCentralSystem01!.OCPP.CustomDataTransferRequestParser = (json, request) => throw new ArgumentException("The central system cannot read it!");

            var started   = Timestamp.Now;

            var transfer  = await cp1.TransferData(
                                      VendorId:        Vendor_Id. GraphDefined,
                                      MessageId:       Message_Id.GraphDefined_TestMessage,
                                      Data:            "Hello world!",
                                      RequestId:       Request_Id.Parse("4714"),
                                      RequestTimeout:  TimeSpan.FromSeconds(10)
                                  );

            Assert.Multiple(() => {
                Assert.That(transfer.Result.ResultCode,  Is.EqualTo(ResultCode.FormationViolation),  $"{transfer.Result}");
                Assert.That(transfer.Result.ResultCode,  Is.Not.EqualTo(ResultCode.Timeout), $"The central system's request error did not reach the charge point: {transfer.Result}");
                Assert.That(transfer.Request.RequestId,  Is.EqualTo(Request_Id.Parse("4714")));
                Assert.That(Waited(started),             Is.LessThan(TimeSpan.FromSeconds(5)));
            });

        }

        #endregion

        #region AResponseError_IsNoAnswer_TheChargePointGetsTheCentralSystemsAnswer()

        /// <summary>
        /// The central system sends a response error under the id of a charge point's
        /// request that the node forwarded, before it answers that request: the charge
        /// point gets the central system's answer, not the error.
        /// </summary>
        /// <remarks>
        /// OCPP 1.6 has no response errors; this library sends them in 1.6 as OCPP 2.1
        /// does its CALLRESULTERROR, which answers no request: it is "sent back on receipt
        /// of a CALLRESULT that contains errors", under the id of that CALLRESULT (OCPP 2.1
        /// Edition 2, Part 4, 4.2.3 and 4.1.4). The node used to take it for the answer to
        /// the request it had forwarded under that id.
        /// </remarks>
        [Test]
        public async Task AResponseError_IsNoAnswer_TheChargePointGetsTheCentralSystemsAnswer()
        {

            var cp1 = await AChargePointBelow("CP-STD-05");

            testCentralSystem01!.OCPP.IN.OnJSONRequestMessageReceived += async (timestamp, sender, connection, request, cancellationToken) => {

                if (request.Action == "DataTransfer")
                    await testCentralSystem01.OCPP.OUT.SendJSONResponseError(
                              new OCPP_JSONResponseErrorMessage(
                                  Timestamp.Now,
                                  EventTracking_Id.New,
                                  NetworkingMode.Standard,
                                  SourceRouting.To(request.NetworkPath.Source),
                                  NetworkPath.From(testCentralSystem01.Id),
                                  request.RequestId,
                                  ResultCode.InternalError,
                                  "This is no answer!"
                              )
                          );

            };

            var transfer = await cp1.TransferData(
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

        #endregion

        #region AChargePointsResponseError_ToAnAnswerOfTheCentralSystem_ReachesTheCentralSystem()

        /// <summary>
        /// A charge point cannot read the central system's answer to a request the node
        /// forwarded, and says so with a response error: the node sends it on to the
        /// central system, under the id the central system answered.
        /// </summary>
        /// <remarks>
        /// The node forwarded the request under an id of its own. It looked the charge
        /// point's id up among the requests it still waited for an answer to, found none,
        /// and kept the error.
        /// </remarks>
        [Test]
        public async Task AChargePointsResponseError_ToAnAnswerOfTheCentralSystem_ReachesTheCentralSystem()
        {

            var cp1             = await AChargePointBelow("CP-STD-09");
            var chargePointsId  = Request_Id.Parse("4730");

            var answered        = new TaskCompletionSource<Request_Id>(TaskCreationOptions.RunContinuationsAsynchronously);
            var heard           = new TaskCompletionSource<(Request_Id RequestId, ResultCode ErrorCode, String? ErrorDescription)>(TaskCreationOptions.RunContinuationsAsynchronously);

            testCentralSystem01!.OCPP.IN.OnJSONRequestMessageReceived       += (timestamp, sender, connection, request, cancellationToken) => {
                if (request.Action == "DataTransfer")
                    answered.TrySetResult(request.RequestId);
                return Task.CompletedTask;
            };

            testCentralSystem01!.OCPP.IN.OnJSONResponseErrorMessageReceived += (timestamp, sender, connection, error, cancellationToken) => {
                heard.TrySetResult((error.RequestId, error.ErrorCode, error.ErrorDescription));
                return Task.CompletedTask;
            };

            var transfer = await cp1.TransferData(
                                     VendorId:        Vendor_Id. GraphDefined,
                                     MessageId:       Message_Id.GraphDefined_TestMessage,
                                     Data:            "Hello world!",
                                     RequestId:       chargePointsId,
                                     RequestTimeout:  TimeSpan.FromSeconds(10)
                                 );

            Assert.That(transfer.Result.ResultCode, Is.EqualTo(ResultCode.OK), $"{transfer.Result}");

            await cp1.OCPP.OUT.SendJSONResponseError(
                      new OCPP_JSONResponseErrorMessage(
                          Timestamp.Now,
                          EventTracking_Id.New,
                          NetworkingMode.Standard,
                          SourceRouting.To(NetworkingNode_Id.CentralSystem),
                          NetworkPath.From(cp1.Id),
                          chargePointsId,
                          ResultCode.FormationViolation,
                          "The charge point cannot read this answer!"
                      )
                  );

            var arrived = await Task.WhenAny(heard.Task, Task.Delay(TimeSpan.FromSeconds(5))) == heard.Task;

            Assert.That(arrived, Is.True, "The charge point's response error did not reach the central system.");

            Assert.Multiple(() => {
                Assert.That(heard.Task.Result.RequestId,         Is.EqualTo(answered.Task.Result), "Not under the id the central system answered.");
                Assert.That(heard.Task.Result.ErrorCode,         Is.EqualTo(ResultCode.FormationViolation));
                Assert.That(heard.Task.Result.ErrorDescription,  Is.EqualTo("The charge point cannot read this answer!"));
            });

        }

        #endregion

        #region AResponseError_DoesNotAnswerTheNodesOwnRequest(Format)

        /// <summary>
        /// The central system sends the node a response error under the id of a request
        /// the node is still waiting for an answer to, in JSON and in binary: the node
        /// keeps waiting, and gets what the central system answers - here the refusal of
        /// an action it does not know.
        /// </summary>
        /// <remarks>
        /// A response error is about an answer the node sent, under the id of the request
        /// that answer was to. Every node numbers its requests from the same start, so that
        /// id is one of the node's own as often as not. The error used to end the node's
        /// own request with that id.
        /// </remarks>
        [TestCase("JSON")]
        [TestCase("Binary")]
        public async Task AResponseError_DoesNotAnswerTheNodesOwnRequest(String Format)
        {

            var requestId = Request_Id.NewRandom();

            testCentralSystem01!.OCPP.IN.OnJSONRequestMessageReceived   += async (timestamp, sender, connection, request, cancellationToken) => {

                if (request.RequestId == requestId)
                {

                    await testCentralSystem01.OCPP.OUT.SendJSONResponseError(
                              new OCPP_JSONResponseErrorMessage(
                                  Timestamp.Now,
                                  EventTracking_Id.New,
                                  NetworkingMode.Standard,
                                  SourceRouting.To(request.NetworkPath.Source),
                                  NetworkPath.From(testCentralSystem01.Id),
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

            testCentralSystem01!.OCPP.IN.OnBinaryRequestMessageReceived += async (timestamp, sender, connection, request, cancellationToken) => {

                if (request.RequestId == requestId)
                {

                    await testCentralSystem01.OCPP.OUT.SendBinaryResponseError(
                              new OCPP_BinaryResponseErrorMessage(
                                  Timestamp.Now,
                                  EventTracking_Id.New,
                                  NetworkingMode.Standard,
                                  SourceRouting.To(request.NetworkPath.Source),
                                  NetworkPath.From(testCentralSystem01.Id),
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

            var state = Format == "JSON"

                            ? await forwarder!.OCPP.OUT.SendJSONRequestAndWait(
                                        new OCPP_JSONRequestMessage(
                                            RequestTimestamp:  Timestamp.Now,
                                            EventTrackingId:   EventTracking_Id.New,
                                            NetworkingMode:    NetworkingMode.Standard,
                                            Destination:       SourceRouting.To(NetworkingNode_Id.CentralSystem),
                                            NetworkPath:       NetworkPath.From(forwarder.Id),
                                            RequestId:         requestId,
                                            Action:            "NoSuchAction",
                                            Payload:           new JObject(),
                                            RequestTimeout:    Timestamp.Now + TimeSpan.FromSeconds(10)
                                        )
                                    )

                            : await forwarder!.OCPP.OUT.SendBinaryRequestAndWait(
                                        new OCPP_BinaryRequestMessage(
                                            RequestTimestamp:  Timestamp.Now,
                                            EventTrackingId:   EventTracking_Id.New,
                                            NetworkingMode:    NetworkingMode.Standard,
                                            Destination:       SourceRouting.To(NetworkingNode_Id.CentralSystem),
                                            NetworkPath:       NetworkPath.From(forwarder.Id),
                                            RequestId:         requestId,
                                            Action:            "NoSuchBinaryAction",
                                            Payload:           "Hello world!".ToUTF8Bytes(),
                                            RequestTimeout:    Timestamp.Now + TimeSpan.FromSeconds(10)
                                        )
                                    );

            Assert.That(state.JSONRequestErrorMessage?.ErrorCode,  Is.EqualTo(ResultCode.ProtocolError),
                        $"The central system's refusal was not what the node got: {state.JSONRequestErrorMessage?.ErrorCode}");

        }

        #endregion


        #region ARequestTheNodeRejects_WithoutARejectResponse_IsRefusedToTheChargePoint()

        /// <summary>
        /// A filter of the node REJECTs a charge point's request without saying what to
        /// answer: the charge point is told so at once.
        /// </summary>
        /// <remarks>
        /// A REJECT without a reject response used to fall through to DROP: no answer,
        /// and the charge point waited out its whole timeout.
        /// </remarks>
        [Test]
        public async Task ARequestTheNodeRejects_WithoutARejectResponse_IsRefusedToTheChargePoint()
        {

            var cp1 = await AChargePointBelow("CP-STD-06");

            forwarder!.OCPP.FORWARD.OnAnyJSONRequestFilter += (timestamp, sender, connection, request, cancellationToken) =>
                Task.FromResult(
                    request.Action == "DataTransfer"
                        ? OCPPv1_6.NetworkingNode.RequestForwardingDecision.REJECT("Not through this node!")
                        : OCPPv1_6.NetworkingNode.RequestForwardingDecision.NEXT(request)
                );

            var started   = Timestamp.Now;

            var transfer  = await cp1.TransferData(
                                      VendorId:        Vendor_Id. GraphDefined,
                                      MessageId:       Message_Id.GraphDefined_TestMessage,
                                      Data:            "Hello world!",
                                      RequestId:       Request_Id.Parse("4722"),
                                      RequestTimeout:  TimeSpan.FromSeconds(10)
                                  );

            Assert.Multiple(() => {
                Assert.That(transfer.Result.ResultCode,   Is.EqualTo(ResultCode.Filtered),  $"{transfer.Result}");
                Assert.That(transfer.Result.Description,  Is.EqualTo("Not through this node!"));
                Assert.That(Waited(started),              Is.LessThan(TimeSpan.FromSeconds(5)));
            });

        }

        #endregion

        #region ARequestTheNodeRejects_WithARejectResponse_TheChargePointGetsThatResponse()

        /// <summary>
        /// A filter of the node REJECTs a charge point's request and says what to
        /// answer: the charge point gets that response, as the answer to its request.
        /// </summary>
        /// <remarks>
        /// The reject response of a filter was never sent: every REJECT went as a request
        /// error "Filtered", and what the filter had made was thrown away.
        /// </remarks>
        [Test]
        public async Task ARequestTheNodeRejects_WithARejectResponse_TheChargePointGetsThatResponse()
        {

            var cp1 = await AChargePointBelow("CP-STD-11");

            forwarder!.OCPP.FORWARD.OnAnyJSONRequestFilter += (timestamp, sender, connection, request, cancellationToken) =>
                Task.FromResult(
                    request.Action == "DataTransfer"
                        ? new OCPPv1_6.NetworkingNode.RequestForwardingDecision(
                              OCPP.NetworkingNode.ForwardingDecisions.REJECT,
                              new JObject(
                                  new JProperty("status",  "Rejected"),
                                  new JProperty("data",    "Not through this node!")
                              )
                          )
                        : OCPPv1_6.NetworkingNode.RequestForwardingDecision.NEXT(request)
                );

            var started   = Timestamp.Now;

            var transfer  = await cp1.TransferData(
                                      VendorId:        Vendor_Id. GraphDefined,
                                      MessageId:       Message_Id.GraphDefined_TestMessage,
                                      Data:            "Hello world!",
                                      RequestId:       Request_Id.Parse("4731"),
                                      RequestTimeout:  TimeSpan.FromSeconds(10)
                                  );

            Assert.Multiple(() => {
                Assert.That(transfer.Result.ResultCode,   Is.EqualTo(ResultCode.OK),  $"{transfer.Result}");
                Assert.That(transfer.Status,              Is.EqualTo(DataTransferStatus.Rejected));
                Assert.That(transfer.Data?.ToString(),    Is.EqualTo("Not through this node!"));
                Assert.That(Waited(started),              Is.LessThan(TimeSpan.FromSeconds(5)));
            });

        }

        #endregion

        #region ARequestTheNodeCannotRead_IsRefusedToTheChargePoint()

        /// <summary>
        /// The node cannot read a charge point's request it was to forward: the charge
        /// point is told so at once, with the reason.
        /// </summary>
        /// <remarks>
        /// A BootNotification, because it is the one action 1.6 has a forwarding
        /// processor for; that processor REJECTs what it cannot read without a reject
        /// response, which used to mean no answer at all.
        /// </remarks>
        [Test]
        public async Task ARequestTheNodeCannotRead_IsRefusedToTheChargePoint()
        {

            var cp1 = await AChargePointBelow("CP-STD-07");

            forwarder!.OCPP.CustomBootNotificationRequestParser = (json, request) => throw new ArgumentException("This node cannot read it!");

            var started  = Timestamp.Now;

            var boot     = await cp1.SendBootNotification(
                                     ChargePointSerialNumber:  "SN-07",
                                     RequestId:                Request_Id.Parse("4723"),
                                     RequestTimeout:           TimeSpan.FromSeconds(10)
                                 );

            Assert.Multiple(() => {
                Assert.That(boot.Result.ResultCode,   Is.EqualTo(ResultCode.Filtered),  $"{boot.Result}");
                Assert.That(boot.Result.Description,  Does.Contain("This node cannot read it!"));
                Assert.That(Waited(started),          Is.LessThan(TimeSpan.FromSeconds(5)));
            });

        }

        #endregion

        #region AnUnknownAction_IsRefusedToTheNodeThatSentIt()

        /// <summary>
        /// The node asks its central system something the central system has no
        /// processor for: the refusal comes back over the connection the central system
        /// accepted, at once.
        /// </summary>
        /// <remarks>
        /// The refusal was addressed to nobody. Over a client connection that did not
        /// matter (84b12cf2), but a server looks its connection up by the destination, and
        /// found none for nobody.
        /// </remarks>
        [Test]
        public async Task AnUnknownAction_IsRefusedToTheNodeThatSentIt()
        {

            var started  = Timestamp.Now;

            var state    = await forwarder!.OCPP.OUT.SendJSONRequestAndWait(
                                     new OCPP_JSONRequestMessage(
                                         RequestTimestamp:  Timestamp.Now,
                                         EventTrackingId:   EventTracking_Id.New,
                                         NetworkingMode:    NetworkingMode.Standard,
                                         Destination:       SourceRouting.To(NetworkingNode_Id.CentralSystem),
                                         NetworkPath:       NetworkPath.From(forwarder.Id),
                                         RequestId:         Request_Id.NewRandom(),
                                         Action:            "NoSuchAction",
                                         Payload:           new JObject(),
                                         RequestTimeout:    Timestamp.Now + TimeSpan.FromSeconds(10)
                                     )
                                 );

            Assert.Multiple(() => {
                Assert.That(state.JSONRequestErrorMessage?.ErrorCode,  Is.EqualTo(ResultCode.ProtocolError),
                            $"The central system's refusal did not reach the node: {state.JSONRequestErrorMessage?.ErrorCode}");
                Assert.That(Waited(started),                           Is.LessThan(TimeSpan.FromSeconds(5)));
            });

        }

        #endregion

    }

}
