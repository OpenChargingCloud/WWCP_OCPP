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

using System.Collections.Concurrent;

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// An answer that arrives before its sender has written the request down
    /// is still the answer to that request.
    /// </summary>
    /// <remarks>
    /// SendJSONRequestAndWait and SendBinaryRequestAndWait used to send a
    /// request first and note it as in flight second. An answer arriving
    /// between the two had nobody to go to: it was dropped as an unknown
    /// response, and the sender then waited out its whole request timeout for
    /// an answer it had already been given. A peer on the same machine answers
    /// inside that gap whenever the sender's continuation is scheduled late -
    /// which is what the CSMSCLI nightly kept meeting on a loaded debian:13
    /// runner: a test or two a night, a different one each time, among them
    /// GetDisplayMessages_Test, with the request received on the far side and
    /// the sender reporting a timeout a minute later.
    ///
    /// These tests do not wait for the scheduler to be unlucky. The adapter
    /// calls a request's SentAction - and with it the "request sent" events -
    /// between the two steps, so one that returns only once the answer is in
    /// holds the gap open for exactly as long as the answer needs to arrive
    /// in it.
    /// </remarks>
    [TestFixture]
    public class AnswerBeforeTheRequestIsRecordedTests : AChargingStationTests
    {

        #region Data

        /// <summary>
        /// The requests an answer has arrived for, as the "response received"
        /// events report them - which is before the adapter looks the answer up.
        /// </summary>
        private readonly ConcurrentDictionary<Request_Id, DateTimeOffset> answered = new();

        #endregion

        #region (private) HoldUntilAnswered(RequestId)

        /// <summary>
        /// Return once the answer to the given request has arrived, and a moment
        /// later: the adapter hands an answer on right after its "response
        /// received" event returns, and the moment is for that.
        /// </summary>
        private void HoldUntilAnswered(Request_Id RequestId)
        {
            SpinWait.SpinUntil(() => answered.ContainsKey(RequestId), TimeSpan.FromSeconds(10));
            Thread.Sleep(250);
        }

        #endregion


        #region AChargingStationKeepsAnAnswerThatArrivesBeforeItRecordedTheRequest()

        /// <summary>
        /// A heartbeat, answered by the CSMS while the charging station is still
        /// between sending it and noting it.
        /// </summary>
        [Test]
        public async Task AChargingStationKeepsAnAnswerThatArrivesBeforeItRecordedTheRequest()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,         Is.Not.Null);
                Assert.That(chargingStation1,  Is.Not.Null);
            });

            chargingStation1!.OCPP.IN.OnJSONResponseMessageReceived += (timestamp, sender, connection, response, ct) => {
                answered.TryAdd(response.RequestId, timestamp);
                return Task.CompletedTask;
            };

            var requestId         = Request_Id.NewRandom();

            var sendRequestState  = await chargingStation1.OCPP.OUT.SendJSONRequestAndWait(
                                              new OCPP_JSONRequestMessage(
                                                  RequestTimestamp:   Timestamp.Now,
                                                  EventTrackingId:    EventTracking_Id.New,
                                                  NetworkingMode:     NetworkingMode.Standard,
                                                  Destination:        SourceRouting.CSMS,
                                                  NetworkPath:        NetworkPath.From(chargingStation1.Id),
                                                  RequestId:          requestId,
                                                  Action:             "Heartbeat",
                                                  Payload:            new JObject(),
                                                  RequestTimeout:     Timestamp.Now + TimeSpan.FromSeconds(5)
                                              ),
                                              sentMessageResult => HoldUntilAnswered(requestId)
                                          );

            Assert.Multiple(() => {

                Assert.That(answered.ContainsKey(requestId),   Is.True,
                            "The CSMS never answered at all, which proves nothing either way.");

                Assert.That(sendRequestState.JSONResponse,     Is.Not.Null,
                            $"The answer arrived and was thrown away: the charging station reported '{sendRequestState.JSONRequestErrorMessage?.ErrorCode}' instead.");

            });

        }

        #endregion

        #region ACSMSKeepsAnAnswerThatArrivesBeforeItRecordedTheRequest()

        /// <summary>
        /// The other way round, which is the way a CSMS asks its charging stations
        /// for anything: over its WebSocket server rather than a client.
        /// </summary>
        [Test]
        public async Task ACSMSKeepsAnAnswerThatArrivesBeforeItRecordedTheRequest()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,         Is.Not.Null);
                Assert.That(chargingStation1,  Is.Not.Null);
            });

            testCSMS1!.OCPP.IN.OnJSONResponseMessageReceived += (timestamp, sender, connection, response, ct) => {
                answered.TryAdd(response.RequestId, timestamp);
                return Task.CompletedTask;
            };

            var requestId         = Request_Id.NewRandom();

            var sendRequestState  = await testCSMS1.OCPP.OUT.SendJSONRequestAndWait(
                                              new OCPP_JSONRequestMessage(
                                                  RequestTimestamp:   Timestamp.Now,
                                                  EventTrackingId:    EventTracking_Id.New,
                                                  NetworkingMode:     NetworkingMode.Standard,
                                                  Destination:        SourceRouting.To(chargingStation1!.Id),
                                                  NetworkPath:        NetworkPath.From(testCSMS1.Id),
                                                  RequestId:          requestId,
                                                  Action:             "GetLocalListVersion",
                                                  Payload:            new JObject(),
                                                  RequestTimeout:     Timestamp.Now + TimeSpan.FromSeconds(5)
                                              ),
                                              sentMessageResult => HoldUntilAnswered(requestId)
                                          );

            Assert.Multiple(() => {

                Assert.That(answered.ContainsKey(requestId),   Is.True,
                            "The charging station never answered at all, which proves nothing either way.");

                Assert.That(sendRequestState.JSONResponse,     Is.Not.Null,
                            $"The answer arrived and was thrown away: the CSMS reported '{sendRequestState.JSONRequestErrorMessage?.ErrorCode}' instead.");

            });

        }

        #endregion

        #region ACSMSKeepsABinaryAnswerThatArrivesBeforeItRecordedTheRequest()

        /// <summary>
        /// Binary requests went through the same two steps in the same order, so
        /// a BinaryDataTransfer, held open by its own "request sent" event.
        /// </summary>
        [Test]
        public async Task ACSMSKeepsABinaryAnswerThatArrivesBeforeItRecordedTheRequest()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,         Is.Not.Null);
                Assert.That(chargingStation1,  Is.Not.Null);
            });

            testCSMS1!.OCPP.IN.OnBinaryResponseMessageReceived += (timestamp, sender, connection, response, ct) => {
                answered.TryAdd(response.RequestId, timestamp);
                return Task.CompletedTask;
            };

            var requestId  = Request_Id.NewRandom();

            testCSMS1.OCPP.OUT.OnBinaryDataTransferRequestSent += (timestamp, sender, connection, request, sentMessageResult, ct) => {
                if (request.RequestId == requestId)
                    HoldUntilAnswered(requestId);
                return Task.CompletedTask;
            };

            var data       = "Hello world!".ToUTF8Bytes();

            var response   = await testCSMS1.TransferBinaryData(
                                       Destination:      SourceRouting.To(chargingStation1!.Id),
                                       VendorId:         Vendor_Id.GraphDefined,
                                       MessageId:        Message_Id.GraphDefined_TestMessage,
                                       Data:             data,
                                       RequestId:        requestId,
                                       RequestTimeout:   TimeSpan.FromSeconds(5)
                                   );

            Assert.Multiple(() => {

                Assert.That(answered.ContainsKey(requestId),   Is.True,
                            "The charging station never answered at all, which proves nothing either way.");

                Assert.That(response.Result.ResultCode,         Is.EqualTo(ResultCode.OK),
                            "The answer arrived and was thrown away.");

                Assert.That(response.Data?.ToUTF8String(),      Is.EqualTo(data.Reverse().ToUTF8String()));

            });

        }

        #endregion

    }

}
