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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.LC;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.NetworkingNode.OverlayNetwork.Default
{

    /// <summary>
    /// Unit tests for a charging station, a networking nodes and a CSMS connected via a (default) overlay network.
    /// This means the charging station communicates with the networking node via classical HTTP WebSocket JSON messages,
    /// and the networking node communicates with the CSMS via extended HTTP WebSocket JSON messages.
    /// 
    /// CS  --[classic JSON]->  NN  --[Overlay Network JSON]->  CSMS
    /// 
    /// All messages are digitally signed via a signature policy.
    /// </summary>
    [TestFixture]
    [NonParallelizable]
    public class NN_2_CS_Tests : ADefaultOverlayNetwork
    {

        // Networking Node -> Charging Station

        #region NN_2_CS_SendReset_Test()

        /// <summary>
        /// A test for resetting a charging station.
        /// </summary>
        [Test]
        public async Task NN_2_CS_SendReset_Test()
        {

            Assert.Multiple(() => {
                Assert.That(localController,         Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,  Is.Not.Null);
                Assert.That(chargingStation,        Is.Not.Null);
            });

            if (localController         is not null &&
                lcOCPPWebSocketServer  is not null &&
                chargingStation        is not null)
            {

                var nnResetRequestsSent             = new ConcurrentList<ResetRequest>();
                var nnJSONMessageRequestsSent       = new ConcurrentList<OCPP_JSONRequestMessage>();
                var csResetRequests                 = new ConcurrentList<ResetRequest>();
                var nnJSONResponseMessagesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
                var nnResetResponsesReceived        = new ConcurrentList<ResetResponse>();

                localController.OCPP.OUT.OnResetRequestSent             += (timestamp, sender, connection, resetRequest, sentMessageResult, ct) => {
                    nnResetRequestsSent.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.OUT.OnJSONRequestMessageSent       += (timestamp, sender, connection, jsonRequestMessage, sentMessageResult, ct) => {
                    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                chargingStation.OCPP.IN. OnResetRequestReceived         += (timestamp, sender, connection, resetRequest, ct) => {
                    csResetRequests.               TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnJSONResponseMessageReceived  += (timestamp, sender, connection, jsonResponseMessage, ct) => {
                    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnResetResponseReceived        += (timestamp, sender, connection, resetRequest, resetResponse, runtime, ct) => {
                    nnResetResponsesReceived.      TryAdd(resetResponse);
                    return Task.CompletedTask;
                };


                var resetType  = ResetType.Immediate;
                var response   = await localController.Reset(
                                           Destination:    SourceRouting.To(chargingStation.Id),
                                           ResetType:      resetType
                                       );


                Assert.Multiple(() => {

                    // Networking Node Request OUT
                    Assert.That(nnResetRequestsSent.           Count,   Is.EqualTo(1), "The Reset request did not leave the networking node!");
                    var nnResetRequest = nnResetRequestsSent.First();
                    Assert.That(nnResetRequest.DestinationId,           Is.EqualTo(chargingStation.Id));
                    Assert.That(nnResetRequest.NetworkPath.Length,      Is.EqualTo(1));
                    Assert.That(nnResetRequest.NetworkPath.Source,      Is.EqualTo(localController.Id));
                    Assert.That(nnResetRequest.NetworkPath.Last,        Is.EqualTo(localController.Id));
                    Assert.That(nnResetRequest.ResetType,               Is.EqualTo(resetType));

                    Assert.That(nnResetRequest.Signatures.Any(),        Is.True, "The outgoing Reset request is not signed!");


                    // Networking Node JSON Request OUT
                    Assert.That(nnJSONMessageRequestsSent.     Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");


                    // Charging Station Request IN
                    Assert.That(csResetRequests.               Count,   Is.EqualTo(1), "The Reset request did not reach the charging station!");
                    var csResetRequest = csResetRequests.First();
                    //Assert.That(csResetRequest.DestinationId,       Is.EqualTo(chargingStation.Id));   // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.NetworkPath.Length,      Is.EqualTo(1));                     // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    Assert.That(csResetRequest.ResetType,               Is.EqualTo(resetType));


                    // Networking Node JSON Response IN
                    Assert.That(nnJSONResponseMessagesReceived.Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");


                    // Networking Node Response IN
                    Assert.That(nnResetResponsesReceived.      Count,   Is.EqualTo(1), "The Reset response did not reach the networking node!");
                    var nnResetResponse = nnResetResponsesReceived.First();
                    Assert.That(nnResetResponse.Request.RequestId,      Is.EqualTo(nnResetRequest.RequestId));

                    Assert.That(nnResetResponse.Signatures.Any(),       Is.True, "The incoming Reset response is not signed!");
                    var nnResetResponseSignature = nnResetResponse.Signatures.First();
                    Assert.That(nnResetResponseSignature.Status,        Is.EqualTo(VerificationStatus.ValidSignature));


                    // Result
                    Assert.That(response.Result.ResultCode,             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                        Is.EqualTo(ResetStatus.Accepted));

                });

            }

        }

        #endregion

        #region NN_2_CS_TransferData_Test()

        /// <summary>
        /// A test for sending signed vendor-specific data from a networking node to a charging station.
        /// </summary>
        [Test]
        public async Task NN_2_CS_TransferData_Test()
        {

            Assert.Multiple(() => {
                Assert.That(localController,         Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,  Is.Not.Null);
                Assert.That(chargingStation,        Is.Not.Null);
            });

            if (localController         is not null &&
                lcOCPPWebSocketServer  is not null &&
                chargingStation        is not null)
            {

                var nnDataTransferRequestsSent       = new ConcurrentList<DataTransferRequest>();
                var nnJSONMessageRequestsSent        = new ConcurrentList<OCPP_JSONRequestMessage>();
                var csDataTransferRequests           = new ConcurrentList<DataTransferRequest>();
                var nnJSONResponseMessagesReceived   = new ConcurrentList<OCPP_JSONResponseMessage>();
                var nnDataTransferResponsesReceived  = new ConcurrentList<DataTransferResponse>();

                localController.OCPP.OUT.OnDataTransferRequestSent      += (timestamp, sender, connection, dataTransferRequest, sentMessageResult, ct) => {
                    nnDataTransferRequestsSent.     TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.OUT.OnJSONRequestMessageSent       += (timestamp, sender, connection, jsonRequestMessage, sentMessageResult, ct) => {
                    nnJSONMessageRequestsSent.      TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                chargingStation.OCPP.IN. OnDataTransferRequestReceived  += (timestamp, sender, connection, dataTransferRequest, ct) => {
                    csDataTransferRequests.         TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnJSONResponseMessageReceived  += (timestamp, sender, connection, jsonResponseMessage, ct) => {
                    nnJSONResponseMessagesReceived. TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnDataTransferResponseReceived += (timestamp, sender, connection, dataTransferRequest, resetResponse, runtime, ct) => {
                    nnDataTransferResponsesReceived.TryAdd(resetResponse);
                    return Task.CompletedTask;
                };


                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.GraphDefined_TestMessage;
                var data       = "Hello world!";
                var response   = await localController.TransferData(
                                           Destination:   SourceRouting.To( chargingStation.Id),
                                           VendorId:      vendorId,
                                           MessageId:     messageId,
                                           Data:          data
                                       );


                Assert.Multiple(() => {

                    // Networking Node Request OUT
                    Assert.That(nnDataTransferRequestsSent.           Count,   Is.EqualTo(1), "The DataTransfer request did not leave the networking node!");
                    var nnDataTransferRequest = nnDataTransferRequestsSent.First();
                    Assert.That(nnDataTransferRequest.DestinationId,           Is.EqualTo(chargingStation.Id));
                    Assert.That(nnDataTransferRequest.NetworkPath.Length,      Is.EqualTo(1));
                    Assert.That(nnDataTransferRequest.NetworkPath.Source,      Is.EqualTo(localController.Id));
                    Assert.That(nnDataTransferRequest.NetworkPath.Last,        Is.EqualTo(localController.Id));
                    Assert.That(nnDataTransferRequest.VendorId,                Is.EqualTo(vendorId));
                    Assert.That(nnDataTransferRequest.MessageId,               Is.EqualTo(messageId));
                    Assert.That(nnDataTransferRequest.Data?.ToString(),        Is.EqualTo(data));

                    Assert.That(nnDataTransferRequest.Signatures.Any(),        Is.True, "The outgoing DataTransfer request is not signed!");


                    // Networking Node JSON Request OUT
                    Assert.That(nnJSONMessageRequestsSent.     Count,   Is.EqualTo(1), "The DataTransfer JSON request did not leave the networking node!");


                    // Charging Station Request IN
                    Assert.That(csDataTransferRequests.               Count,   Is.EqualTo(1), "The DataTransfer request did not reach the charging station!");
                    var csDataTransferRequest = csDataTransferRequests.First();
                    //Assert.That(csDataTransferRequest.DestinationId,       Is.EqualTo(chargingStation.Id));   // Because of "standard" networking mode!
                    //Assert.That(csDataTransferRequest.NetworkPath.Length,      Is.EqualTo(1));                     // Because of "standard" networking mode!
                    //Assert.That(csDataTransferRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    //Assert.That(csDataTransferRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    Assert.That(csDataTransferRequest.VendorId,                Is.EqualTo(vendorId));
                    Assert.That(csDataTransferRequest.MessageId,               Is.EqualTo(messageId));
                    Assert.That(csDataTransferRequest.Data?.ToString(),        Is.EqualTo(data));


                    // Networking Node JSON Response IN
                    Assert.That(nnJSONResponseMessagesReceived.Count,   Is.EqualTo(1), "The DataTransfer JSON request did not leave the networking node!");


                    // Networking Node Response IN
                    Assert.That(nnDataTransferResponsesReceived.      Count,   Is.EqualTo(1), "The DataTransfer response did not reach the networking node!");
                    var nnDataTransferResponse = nnDataTransferResponsesReceived.First();
                    Assert.That(nnDataTransferResponse.Request.RequestId,      Is.EqualTo(nnDataTransferRequest.RequestId));

                    Assert.That(nnDataTransferResponse.Signatures.Any(),       Is.True, "The incoming DataTransfer response is not signed!");
                    var nnDataTransferResponseSignature = nnDataTransferResponse.Signatures.First();
                    Assert.That(nnDataTransferResponseSignature.Status,        Is.EqualTo(VerificationStatus.ValidSignature));


                    // Result
                    Assert.That(response.Result.ResultCode,             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                        Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data?.ToString(),              Is.EqualTo(data.Reverse()));


                });

            }

        }

        #endregion


    }

}
