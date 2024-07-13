/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.NN;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.OverlayNetworking.OverlayNetwork.Default
{

    /// <summary>
    /// Unit tests for a charging station, a networking nodes and a CSMS connected via a (default) overlay network.
    /// This means the charging station communicates with the networking node via classical HTTP Web Socket JSON messages,
    /// and the networking node communicates with the CSMS via extended HTTP Web Socket JSON messages.
    /// 
    /// CS  --[classic JSON]->  NN  --[Overlay Network JSON]->  CSMS
    /// 
    /// All messages are digitally signed via a signature policy.
    /// </summary>
    [TestFixture]
    [NonParallelizable]
    public class CSMS_2_CS_Tests : ADefaultOverlayNetwork
    {

        // CSMS --Networking Node-> Charging Station

        #region CSMS_2_CS_SendReset_Test()

        /// <summary>
        /// A test for resetting a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_2_CS_SendReset_Test()
        {

            Assert.Multiple(() => {
                Assert.That(chargingStation,         Is.Not.Null);
                Assert.That(localController,          Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
            });

            if (chargingStation        is not null &&
                localController         is not null &&
                lcOCPPWebSocketServer  is not null &&
                CSMS                   is not null)
            {

                var csmsResetRequestsSent           = new ConcurrentList<ResetRequest>();
                var nnResetRequestsForwarded        = new ConcurrentList<ForwardingDecision<ResetRequest, ResetResponse>>();
                var nnJSONRequestMessagesSent       = new ConcurrentList<Tuple<OCPP_JSONRequestMessage,  SendMessageResult>>();
                var csResetRequests                 = new ConcurrentList<ResetRequest>();
                var nnJSONResponseMessagesSent      = new ConcurrentList<Tuple<OCPP_JSONResponseMessage, SendMessageResult>>();
                //var nnJSONResponseMessagesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
                var csmsResetResponsesReceived      = new ConcurrentList<ResetResponse>();

                CSMS.OCPP.OUT.               OnResetRequestSent                 += (timestamp, sender, resetRequest) => {
                    csmsResetRequestsSent.     TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.FORWARD.OnResetRequestLogging      += (timestamp, sender, connection, resetRequest, forwardingDecision) => {
                    nnResetRequestsForwarded.  TryAdd(forwardingDecision);
                    return Task.CompletedTask;
                };

                localController.OCPP.OUT.OnJSONRequestMessageSent   += (timestamp, sender, jsonRequestMessage, sendOCPPMessageResult) => {
                    nnJSONRequestMessagesSent. TryAdd(new Tuple<OCPP_JSONRequestMessage, SendMessageResult>(jsonRequestMessage, sendOCPPMessageResult));
                    return Task.CompletedTask;
                };

                chargingStation.            OnResetRequest             += (timestamp, sender, connection, resetRequest) => {
                    csResetRequests.           TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.OUT.OnJSONResponseMessageSent  += (timestamp, sender, jsonResponseMessage, sendOCPPMessageResult) => {
                    nnJSONResponseMessagesSent.TryAdd(new Tuple<OCPP_JSONResponseMessage, SendMessageResult>(jsonResponseMessage, sendOCPPMessageResult));
                    return Task.CompletedTask;
                };

                CSMS.OCPP.IN.                OnResetResponseReceived            += (timestamp, sender, resetRequest, resetResponse, runtime) => {
                    csmsResetResponsesReceived.TryAdd(resetResponse);
                    return Task.CompletedTask;
                };

                CSMS.           OCPP.AddStaticRouting(chargingStation.Id, localController.Id);
                localController.OCPP.AddStaticRouting(CSMS.Id,            NetworkingNode_Id.CSMS); //Fix me!


                var resetType  = ResetType.Immediate;
                var response   = await CSMS.Reset(
                                           DestinationNodeId:  chargingStation.Id,
                                           ResetType:          resetType
                                       );


                Assert.Multiple(() => {

                    // CSMS Request OUT
                    Assert.That(csmsResetRequestsSent.         Count,             Is.EqualTo(1), "The Reset request did not leave the CSMS!");
                    var csmsResetRequest = csmsResetRequestsSent.First();
                    Assert.That(csmsResetRequest.Signatures.Any(),                Is.True, "The outgoing Reset request is not signed!");

                    // Networking Node Request FORWARD
                    Assert.That(nnResetRequestsForwarded.      Count,             Is.EqualTo(1), "The Reset request did not reach the networking node!");
                    var nnResetRequest = nnResetRequestsForwarded.First();
                    Assert.That(nnResetRequest.Request.DestinationNodeId,         Is.EqualTo(chargingStation.Id));
                    //Assert.That(nnResetRequest.Request.NetworkPath.Length,        Is.EqualTo(1));
                    //Assert.That(nnResetRequest.Request.NetworkPath.Source,        Is.EqualTo(CSMS.Id));
                    //Assert.That(nnResetRequest.Request.NetworkPath.Last,          Is.EqualTo(CSMS.Id));
                    Assert.That(nnResetRequest.Request.ResetType,                 Is.EqualTo(resetType));
                    Assert.That(nnResetRequest.Result,                            Is.EqualTo(ForwardingResult.FORWARD));


                    // Charging Station Request IN
                    Assert.That(csResetRequests.               Count,             Is.EqualTo(1), "The Reset request did not reach the charging station!");
                    var csResetRequest = csResetRequests.First();
                    Assert.That(csResetRequest.DestinationNodeId,                 Is.EqualTo(NetworkingNode_Id.Zero));   // Because of "standard" networking mode!
                    Assert.That(csResetRequest.NetworkPath.Length,                Is.EqualTo(0));                        // Because of "standard" networking mode!
                    Assert.That(csResetRequest.ResetType,                         Is.EqualTo(resetType));


                    // Networking Node JSON Response FORWARD
                    Assert.That(nnJSONResponseMessagesSent.Count,                 Is.EqualTo(1), "The Reset JSON response did not leave the networking node!");
                    var nnJSONResponseMessage = nnJSONResponseMessagesSent.First();
                    Assert.That(nnJSONResponseMessage.Item1.DestinationId,    Is.EqualTo(CSMS.Id));
                    Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Length,   Is.EqualTo(2));
                    Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Source,   Is.EqualTo(chargingStation.Id));
                    Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Last,     Is.EqualTo(localController.Id));
                    Assert.That(nnJSONResponseMessage.Item1.NetworkingMode,       Is.EqualTo(NetworkingMode.OverlayNetwork));
                    Assert.That(nnJSONResponseMessage.Item2,                      Is.EqualTo(SendMessageResult.Success));


                    // CSMS Response IN
                    Assert.That(csmsResetResponsesReceived.    Count,             Is.EqualTo(1), "The Reset response did not reach the networking node!");
                    var csmsResetResponse = csmsResetResponsesReceived.First();
                    Assert.That(csmsResetResponse.Request.RequestId,              Is.EqualTo(csmsResetRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,                       Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                  Is.EqualTo(ResetStatus.Accepted));

                });

            }

        }

        #endregion

        #region CSMS_2_CS_TransferData_Test()

        /// <summary>
        /// A test for sending signed vendor-specific data from the CSMS to a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_2_CS_TransferData_Test()
        {

            Assert.Multiple(() => {
                Assert.That(chargingStation,        Is.Not.Null);
                Assert.That(localController,        Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,  Is.Not.Null);
                Assert.That(CSMS,                   Is.Not.Null);
            });

            if (chargingStation        is not null &&
                localController        is not null &&
                lcOCPPWebSocketServer  is not null &&
                CSMS                   is not null)
            {

                var csmsDataTransferRequestsSent           = new ConcurrentList<DataTransferRequest>();
                var nnDataTransferRequestsForwarded        = new ConcurrentList<ForwardingDecision<DataTransferRequest, DataTransferResponse>>();
                var nnJSONRequestMessagesSent              = new ConcurrentList<Tuple<OCPP_JSONRequestMessage,  SendMessageResult>>();
                var csDataTransferRequests                 = new ConcurrentList<DataTransferRequest>();
                var nnJSONResponseMessagesSent             = new ConcurrentList<Tuple<OCPP_JSONResponseMessage, SendMessageResult>>();
                //var nnJSONResponseMessagesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
                var csmsDataTransferResponsesReceived      = new ConcurrentList<DataTransferResponse>();

                CSMS.OCPP.OUT.               OnDataTransferRequestSent         += (timestamp, sender, DataTransferRequest) => {
                    csmsDataTransferRequestsSent.     TryAdd(DataTransferRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.FORWARD.OnDataTransferRequestFiltered += (timestamp, sender, connection, DataTransferRequest, forwardingDecision) => {
                    nnDataTransferRequestsForwarded.  TryAdd(forwardingDecision);
                    return Task.CompletedTask;
                };

                localController.OCPP.OUT.OnJSONRequestMessageSent      += (timestamp, sender, jsonRequestMessage, sendOCPPMessageResult) => {
                    nnJSONRequestMessagesSent. TryAdd(new Tuple<OCPP_JSONRequestMessage, SendMessageResult>(jsonRequestMessage, sendOCPPMessageResult));
                    return Task.CompletedTask;
                };

                chargingStation.            OnDataTransferRequestReceived += (timestamp, sender, connection, DataTransferRequest) => {
                    csDataTransferRequests.           TryAdd(DataTransferRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.OUT.OnJSONResponseMessageSent     += (timestamp, sender, jsonResponseMessage, sendOCPPMessageResult) => {
                    nnJSONResponseMessagesSent.TryAdd(new Tuple<OCPP_JSONResponseMessage, SendMessageResult>(jsonResponseMessage, sendOCPPMessageResult));
                    return Task.CompletedTask;
                };

                CSMS.OCPP.IN.                OnDataTransferResponseReceived        += (timestamp, sender, DataTransferRequest, DataTransferResponse, runtime) => {
                    csmsDataTransferResponsesReceived.TryAdd(DataTransferResponse);
                    return Task.CompletedTask;
                };

                CSMS.           OCPP.AddStaticRouting(chargingStation.Id, localController.Id);
                localController.OCPP.AddStaticRouting(CSMS.Id,            NetworkingNode_Id.CSMS); //Fix me!


                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.GraphDefined_TestMessage;
                var data       = "Hello world!";
                var response   = await CSMS.TransferData(
                                           DestinationNodeId:  chargingStation.Id,
                                           VendorId:           vendorId,
                                           MessageId:          messageId,
                                           Data:               data,
                                           CustomData:         null
                                       );


                Assert.Multiple(() => {

                    // CSMS Request OUT
                    Assert.That(csmsDataTransferRequestsSent.         Count,             Is.EqualTo(1), "The DataTransfer request did not leave the CSMS!");
                    var csmsDataTransferRequest = csmsDataTransferRequestsSent.First();
                    Assert.That(csmsDataTransferRequest.DestinationNodeId,               Is.EqualTo(chargingStation.Id));
                    Assert.That(csmsDataTransferRequest.NetworkPath.Length,              Is.EqualTo(1));
                    Assert.That(csmsDataTransferRequest.NetworkPath.Source,              Is.EqualTo(CSMS.Id));
                    Assert.That(csmsDataTransferRequest.NetworkPath.Last,                Is.EqualTo(CSMS.Id));
                    Assert.That(csmsDataTransferRequest.VendorId,                        Is.EqualTo(vendorId));
                    Assert.That(csmsDataTransferRequest.MessageId,                       Is.EqualTo(messageId));
                    Assert.That(csmsDataTransferRequest.Data?.ToString(),                Is.EqualTo(data));

                    Assert.That(csmsDataTransferRequest.Signatures.Any(),                Is.True, "The outgoing DataTransfer request is not signed!");


                    // Networking Node Request FORWARD
                    Assert.That(nnDataTransferRequestsForwarded.      Count,             Is.EqualTo(1), "The DataTransfer request did not reach the networking node!");
                    var nnDataTransfer = nnDataTransferRequestsForwarded.First();
                    Assert.That(nnDataTransfer.Request.DestinationNodeId,                Is.EqualTo(chargingStation.Id));
                    Assert.That(nnDataTransfer.Request.NetworkPath.Length,               Is.EqualTo(1));
                    Assert.That(nnDataTransfer.Request.NetworkPath.Source,               Is.EqualTo(CSMS.Id));
                    Assert.That(nnDataTransfer.Request.NetworkPath.Last,                 Is.EqualTo(CSMS.Id));
                    Assert.That(nnDataTransfer.Request.VendorId,                         Is.EqualTo(vendorId));
                    Assert.That(nnDataTransfer.Request.MessageId,                        Is.EqualTo(messageId));
                    Assert.That(nnDataTransfer.Request.Data?.ToString(),                 Is.EqualTo(data));
                    Assert.That(nnDataTransfer.Result,                                   Is.EqualTo(ForwardingResult.FORWARD));


                    // Charging Station Request IN
                    Assert.That(csDataTransferRequests.               Count,             Is.EqualTo(1), "The DataTransfer request did not reach the charging station!");
                    var csDataTransferRequest = csDataTransferRequests.First();
                    Assert.That(csDataTransferRequest.DestinationNodeId,                 Is.EqualTo(NetworkingNode_Id.Zero));   // Because of "standard" networking mode!
                    Assert.That(csDataTransferRequest.NetworkPath.Length,                Is.EqualTo(0));                        // Because of "standard" networking mode!
                    Assert.That(csDataTransferRequest.VendorId,                          Is.EqualTo(vendorId));
                    Assert.That(csDataTransferRequest.MessageId,                         Is.EqualTo(messageId));
                    Assert.That(csDataTransferRequest.Data?.ToString(),                  Is.EqualTo(data));


                    // Networking Node JSON Response FORWARD
                    Assert.That(nnJSONResponseMessagesSent.Count,                 Is.EqualTo(1), "The DataTransfer JSON response did not leave the networking node!");
                    var nnJSONResponseMessage = nnJSONResponseMessagesSent.First();
                    Assert.That(nnJSONResponseMessage.Item1.DestinationId,    Is.EqualTo(CSMS.Id));
                    Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Length,   Is.EqualTo(2));
                    Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Source,   Is.EqualTo(chargingStation.Id));
                    Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Last,     Is.EqualTo(localController.Id));
                    Assert.That(nnJSONResponseMessage.Item1.NetworkingMode,       Is.EqualTo(NetworkingMode.OverlayNetwork));
                    Assert.That(nnJSONResponseMessage.Item2,                      Is.EqualTo(SendMessageResult.Success));


                    // CSMS Response IN
                    Assert.That(csmsDataTransferResponsesReceived.    Count,             Is.EqualTo(1), "The DataTransfer response did not reach the networking node!");
                    var csmsDataTransferResponse = csmsDataTransferResponsesReceived.First();
                    Assert.That(csmsDataTransferResponse.Request.RequestId,              Is.EqualTo(csmsDataTransferRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,                       Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                  Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data?.ToString(),                        Is.EqualTo(data.Reverse()));

                });

            }

        }

        #endregion


    }

}
