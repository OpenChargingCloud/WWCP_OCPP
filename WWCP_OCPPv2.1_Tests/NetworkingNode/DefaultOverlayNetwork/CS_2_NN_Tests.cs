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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.NetworkingNode.OverlayNetwork.Default
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
    public class CS_2_NN_Tests : ADefaultOverlayNetwork
    {

        // Charging Station -> Networking Node

        #region CS_2_NN_SendBootNotifications_Test()

        /// <summary>
        /// A test for sending signed Boot Notifications from a charging station to a networking node.
        /// </summary>
        [Test]
        public async Task CS_2_NN_SendBootNotifications_Test()
        {

            Assert.Multiple(() => {
                Assert.That(localController,         Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,  Is.Not.Null);
                Assert.That(chargingStation,        Is.Not.Null);
            });

            if (localController        is not null &&
                lcOCPPWebSocketServer  is not null &&
                chargingStation        is not null)
            {

                var csBootNotificationRequestsSent       = new ConcurrentList<BootNotificationRequest>();
                var nnJSONMessageRequestsReceived        = new ConcurrentList<OCPP_JSONRequestMessage>();
                var nnBootNotificationRequestsReceived   = new ConcurrentList<BootNotificationRequest>();
                var nnBootNotificationResponsesSent      = new ConcurrentList<BootNotificationResponse>();
                var nnJSONResponseMessagesSent           = new ConcurrentList<OCPP_JSONResponseMessage>();
                var csBootNotificationResponsesReceived  = new ConcurrentList<BootNotificationResponse>();

                chargingStation.OCPP.OUT.OnBootNotificationRequestSent      += (timestamp, sender, connection, bootNotificationRequest, sentMessageResult, ct) => {
                    csBootNotificationRequestsSent.     TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnJSONRequestMessageReceived       += (timestamp, sender, connection, jsonRequestMessage, ct) => {
                    nnJSONMessageRequestsReceived.      TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnBootNotificationRequestReceived  += (timestamp, sender, connection, bootNotificationRequest, ct) => {
                    nnBootNotificationRequestsReceived. TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                //networkingNode.OCPP.OUT.nnBootNotificationResponsesSent     += (timestamp, sender, connection, jsonResponseMessage, ct) => {
                //    nnBootNotificationResponsesSent.    TryAdd(jsonResponseMessage);
                //    return Task.CompletedTask;
                //};

                localController.OCPP.OUT.OnJSONResponseMessageSent          += (timestamp, sender, connection, jsonResponseMessage, sentMessageResult, ct) => {
                    nnJSONResponseMessagesSent.         TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                chargingStation.OCPP.IN. OnBootNotificationResponseReceived += (timestamp, sender, connection, bootNotificationRequest, bootNotificationResponse, runtime, ct) => {
                    csBootNotificationResponsesReceived.TryAdd(bootNotificationResponse);
                    return Task.CompletedTask;
                };

                // "Standard" networking mode and the networking node acts as CSMS!
                localController.OCPP.IN.     AnycastIds.      Add(NetworkingNode_Id.CSMS);
                localController.OCPP.FORWARD.AnycastIdsDenied.Add(NetworkingNode_Id.CSMS);


                var reason    = BootReason.PowerUp;
                var response  = await chargingStation.SendBootNotification(
                                          BootReason:  reason
                                      );


                Assert.Multiple(() => {

                    // Charging Station Request OUT
                    Assert.That(csBootNotificationRequestsSent.     Count,                    Is.EqualTo(1), "The BootNotification request did not leave the charging station!");
                    var csBootNotificationRequest = csBootNotificationRequestsSent.First();
                    Assert.That(csBootNotificationRequest.DestinationId,                  Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csBootNotificationRequest.NetworkPath.Length,                 Is.EqualTo(0)); // Because of "standard" networking mode
                    Assert.That(csBootNotificationRequest.Reason,                             Is.EqualTo(reason));

                    Assert.That(csBootNotificationRequest.Signatures.Any(),                   Is.True, "The outgoing BootNotification request is not signed!");


                    // Networking Node JSON Request IN
                    Assert.That(nnJSONMessageRequestsReceived.      Count,                    Is.EqualTo(1), "The BootNotification JSON request did not reach the networking node!");
                    var nnJSONMessageRequest = nnJSONMessageRequestsReceived.First();
                    Assert.That(nnJSONMessageRequest.DestinationId,                       Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnJSONMessageRequest.NetworkPath.Length,                      Is.EqualTo(1));
                    Assert.That(nnJSONMessageRequest.NetworkPath.Source,                      Is.EqualTo(chargingStation.Id));
                    Assert.That(nnJSONMessageRequest.NetworkPath.Last,                        Is.EqualTo(chargingStation.Id));


                    // Networking Node Request IN
                    Assert.That(nnBootNotificationRequestsReceived. Count,                    Is.EqualTo(1), "The BootNotification request did not reach the networking node!");
                    var nnBootNotificationRequest = nnBootNotificationRequestsReceived.First();
                    Assert.That(nnBootNotificationRequest.DestinationId,                  Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnBootNotificationRequest.NetworkPath.Length,                 Is.EqualTo(1));
                    Assert.That(nnBootNotificationRequest.NetworkPath.Source,                 Is.EqualTo(chargingStation.Id));
                    Assert.That(nnBootNotificationRequest.NetworkPath.Last,                   Is.EqualTo(chargingStation.Id));
                    Assert.That(nnBootNotificationRequest.Reason,                             Is.EqualTo(reason));

                    Assert.That(nnBootNotificationRequest.Signatures.Any(),                   Is.True, "The incoming BootNotification request is not signed!");
                    var csmsBootNotificationRequestSignature = nnBootNotificationRequest.Signatures.First();
                    Assert.That(csmsBootNotificationRequestSignature.Status,                  Is.EqualTo(VerificationStatus.ValidSignature));

                    Assert.That(nnBootNotificationRequest.ChargingStation,                    Is.Not.Null);
                    var chargingStation2 = nnBootNotificationRequest.ChargingStation;
                    if (chargingStation2 is not null)
                    {

                        Assert.That(chargingStation2.Model,             Is.EqualTo(chargingStation.Model));
                        Assert.That(chargingStation2.VendorName,        Is.EqualTo(chargingStation.VendorName));
                        Assert.That(chargingStation2.SerialNumber,      Is.EqualTo(chargingStation.SerialNumber));
                        Assert.That(chargingStation2.FirmwareVersion,   Is.EqualTo(chargingStation.FirmwareVersion));
                        Assert.That(chargingStation2.Modem,             Is.Not.Null);

                        if (chargingStation2.Modem is not null &&
                            localController.Modem is not null)
                        {
                            Assert.That(chargingStation2.Modem.ICCID,   Is.EqualTo(chargingStation.Modem.ICCID));
                            Assert.That(chargingStation2.Modem.IMSI,    Is.EqualTo(chargingStation.Modem.IMSI));
                        }

                    }


                    // Networking Node Response OUT


                    // Networking Node JSON Response OUT
                    Assert.That(nnJSONResponseMessagesSent.         Count,                    Is.EqualTo(1), "The BootNotification JSON response did not leave the networking node!");


                    // Charging Station Response IN
                    Assert.That(csBootNotificationResponsesReceived.Count,                    Is.EqualTo(1), "The BootNotification response did not reach the charging station!");
                    var csBootNotificationResponse = csBootNotificationResponsesReceived.First();
                    Assert.That(csBootNotificationResponse.Request.RequestId,                 Is.EqualTo(nnBootNotificationRequest.RequestId));

                    Assert.That(csBootNotificationResponse.Signatures.Any(),                  Is.True, "The incoming BootNotification response is not signed!");
                    var csBootNotificationResponseSignature = csBootNotificationResponse.Signatures.First();
                    Assert.That(csBootNotificationResponseSignature.Status,                   Is.EqualTo(VerificationStatus.ValidSignature));


                    // Result
                    Assert.That(response.Result.ResultCode,                                   Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                              Is.EqualTo(RegistrationStatus.Accepted));

                });

            }

        }

        #endregion

        #region CS_2_NN_TransferData_Test()

        /// <summary>
        /// A test for sending signed vendor-specific data from a charging station to a networking node.
        /// </summary>
        [Test]
        public async Task CS_2_NN_TransferData_Test()
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

                var csDataTransferRequestsSent       = new ConcurrentList<DataTransferRequest>();
                var nnJSONMessageRequestsReceived    = new ConcurrentList<OCPP_JSONRequestMessage>();
                var nnDataTransferRequestsReceived   = new ConcurrentList<DataTransferRequest>();
                var nnDataTransferResponsesSent      = new ConcurrentList<DataTransferResponse>();
                var nnJSONResponseMessagesSent       = new ConcurrentList<OCPP_JSONResponseMessage>();
                var csDataTransferResponsesReceived  = new ConcurrentList<DataTransferResponse>();

                chargingStation.OCPP.OUT.OnDataTransferRequestSent      += (timestamp, sender, connection, dataTransferRequest, sentMessageResult, ct) => {
                    csDataTransferRequestsSent.     TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnJSONRequestMessageReceived   += (timestamp, sender, connection, jsonRequestMessage, ct) => {
                    nnJSONMessageRequestsReceived.  TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnDataTransferRequestReceived  += (timestamp, sender, connection, dataTransferRequest, ct) => {
                    nnDataTransferRequestsReceived. TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.OUT.OnDataTransferResponseSent     += (timestamp, sender, connection, dataTransferRequest, dataTransferResponse, runtime, sentMessageResult, ct) => {
                    nnDataTransferResponsesSent.    TryAdd(dataTransferResponse);
                    return Task.CompletedTask;
                };

                localController.OCPP.OUT.OnJSONResponseMessageSent      += (timestamp, sender, connection, jsonResponseMessage, sentMessageResult, ct) => {
                    nnJSONResponseMessagesSent.     TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                chargingStation.OCPP.IN. OnDataTransferResponseReceived += (timestamp, sender, connection, dataTransferRequest, dataTransferResponse, runtime, ct) => {
                    csDataTransferResponsesReceived.TryAdd(dataTransferResponse);
                    return Task.CompletedTask;
                };

                // "Standard" networking mode and the networking node acts as CSMS!
                localController.OCPP.IN.     AnycastIds.      Add(NetworkingNode_Id.CSMS);
                localController.OCPP.FORWARD.AnycastIdsDenied.Add(NetworkingNode_Id.CSMS);


                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.GraphDefined_TestMessage;
                var data       = "Hello world!";
                var response   = await chargingStation.TransferData(
                                           VendorId:     vendorId,
                                           MessageId:    messageId,
                                           Data:         data,
                                           CustomData:   null
                                       );


                Assert.Multiple(() => {

                    // Charging Station Request OUT
                    Assert.That(csDataTransferRequestsSent.     Count,                    Is.EqualTo(1), "The DataTransfer request did not leave the charging station!");
                    var csDataTransferRequest = csDataTransferRequestsSent.First();
                    Assert.That(csDataTransferRequest.DestinationId,                  Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csDataTransferRequest.NetworkPath.Length,                 Is.EqualTo(0)); // Because of "standard" networking mode
                    Assert.That(csDataTransferRequest.VendorId,                           Is.EqualTo(vendorId));
                    Assert.That(csDataTransferRequest.MessageId,                          Is.EqualTo(messageId));
                    Assert.That(csDataTransferRequest.Data?.ToString(),                   Is.EqualTo(data));

                    Assert.That(csDataTransferRequest.Signatures.Any(),                   Is.True, "The outgoing DataTransfer request is not signed!");


                    // Networking Node JSON Request IN
                    Assert.That(nnJSONMessageRequestsReceived.  Count,                    Is.EqualTo(1), "The DataTransfer JSON request did not reach the networking node!");
                    var nnJSONMessageRequest = nnJSONMessageRequestsReceived.First();
                    Assert.That(nnJSONMessageRequest.DestinationId,                   Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnJSONMessageRequest.NetworkPath.Length,                  Is.EqualTo(1));
                    Assert.That(nnJSONMessageRequest.NetworkPath.Source,                  Is.EqualTo(chargingStation.Id));
                    Assert.That(nnJSONMessageRequest.NetworkPath.Last,                    Is.EqualTo(chargingStation.Id));


                    // Networking Node Request IN
                    Assert.That(nnDataTransferRequestsReceived. Count,                    Is.EqualTo(1), "The DataTransfer request did not reach the networking node!");
                    var nnDataTransferRequestReceived = nnDataTransferRequestsReceived.First();
                    Assert.That(nnDataTransferRequestReceived.DestinationId,          Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnDataTransferRequestReceived.NetworkPath.Length,         Is.EqualTo(1));
                    Assert.That(nnDataTransferRequestReceived.NetworkPath.Source,         Is.EqualTo(chargingStation.Id));
                    Assert.That(nnDataTransferRequestReceived.NetworkPath.Last,           Is.EqualTo(chargingStation.Id));
                    Assert.That(nnDataTransferRequestReceived.VendorId,                   Is.EqualTo(vendorId));
                    Assert.That(nnDataTransferRequestReceived.MessageId,                  Is.EqualTo(messageId));
                    Assert.That(nnDataTransferRequestReceived.Data?.ToString(),           Is.EqualTo(data));

                    Assert.That(nnDataTransferRequestReceived.Signatures.Any(),           Is.True, "The incoming DataTransfer request is not signed!");
                    var nnDataTransferRequestSignature = nnDataTransferRequestReceived.Signatures.First();
                    Assert.That(nnDataTransferRequestSignature.Status,                    Is.EqualTo(VerificationStatus.ValidSignature));


                    // Networking Node Response OUT
                    Assert.That(nnDataTransferResponsesSent.    Count,                    Is.EqualTo(1), "The DataTransfer response did not leave the networking node!");
                    var nnDataTransferResponseSent = nnDataTransferResponsesSent.First();
                    Assert.That(nnDataTransferResponseSent.DestinationId,             Is.EqualTo(chargingStation.Id));
                    Assert.That(nnDataTransferResponseSent.NetworkPath.Length,            Is.EqualTo(1));
                    Assert.That(nnDataTransferResponseSent.NetworkPath.Source,            Is.EqualTo(localController.Id));
                    Assert.That(nnDataTransferResponseSent.NetworkPath.Last,              Is.EqualTo(localController.Id));
                    Assert.That(nnDataTransferResponseSent.Data?.ToString(),              Is.EqualTo(data.Reverse()));

                    Assert.That(nnDataTransferResponseSent.Signatures.Any(),              Is.True, "The DataTransfer response is not signed!");


                    // Networking Node JSON Response OUT
                    Assert.That(nnJSONResponseMessagesSent.     Count,                    Is.EqualTo(1), "The DataTransfer JSON response did not leave the networking node!");


                    // Charging Station Response IN
                    Assert.That(csDataTransferResponsesReceived.Count,                    Is.EqualTo(1), "The DataTransfer response did not reach the charging station!");
                    var csDataTransferResponse = csDataTransferResponsesReceived.First();
                    Assert.That(csDataTransferResponse.Request.RequestId,                 Is.EqualTo(nnDataTransferRequestReceived.RequestId));

                    Assert.That(csDataTransferResponse.Signatures.Any(),                  Is.True, "The incoming DataTransfer response is not signed!");
                    var csDataTransferResponseSignature = csDataTransferResponse.Signatures.First();
                    Assert.That(csDataTransferResponseSignature.Status,                   Is.EqualTo(VerificationStatus.ValidSignature));


                    // Result
                    Assert.That(response.Result.ResultCode,                               Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                          Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data?.ToString(),                                Is.EqualTo(data.Reverse()));


                });

            }

        }

        #endregion

        #region CS_2_NN_TransferBinaryData_Test()

        /// <summary>
        /// A test for sending signed custom binary data from a charging station to a networking node.
        /// </summary>
        [Test]
        public async Task CS_2_NN_TransferBinaryData_Test()
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

                var csBinaryDataTransferRequestsSent       = new ConcurrentList<BinaryDataTransferRequest>();
                var nnBinaryMessageRequestsReceived        = new ConcurrentList<OCPP_BinaryRequestMessage>();
                var nnBinaryDataTransferRequestsReceived   = new ConcurrentList<BinaryDataTransferRequest>();
                var nnBinaryDataTransferResponsesSent      = new ConcurrentList<BinaryDataTransferResponse>();
                var nnBinaryResponseMessagesSent           = new ConcurrentList<OCPP_BinaryResponseMessage>();
                var csBinaryDataTransferResponsesReceived  = new ConcurrentList<BinaryDataTransferResponse>();

                chargingStation.OCPP.OUT.OnBinaryDataTransferRequestSent      += (timestamp, sender, connection, bootNotificationRequest, sentMessageResult, ct) => {
                    csBinaryDataTransferRequestsSent.     TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN.OnBinaryRequestMessageReceived        += (timestamp, sender, connection, jsonRequestMessage, ct) => {
                    nnBinaryMessageRequestsReceived.      TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN.OnBinaryDataTransferRequestReceived   += (timestamp, sender, connection, bootNotificationRequest, ct) => {
                    nnBinaryDataTransferRequestsReceived. TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                //networkingNode.OCPP.OUT.nnBinaryDataTransferResponsesSent     += (timestamp, sender, connection, jsonResponseMessage, ct) => {
                //    nnBinaryDataTransferResponsesSent.    TryAdd(jsonResponseMessage);
                //    return Task.CompletedTask;
                //};

                localController.OCPP.OUT.OnBinaryResponseMessageSent          += (timestamp, sender, connection, jsonResponseMessage, sentMessageResult, ct) => {
                    nnBinaryResponseMessagesSent.         TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                chargingStation.OCPP.IN. OnBinaryDataTransferResponseReceived += (timestamp, sender, connection, bootNotificationRequest, bootNotificationResponse, runtime, ct) => {
                    csBinaryDataTransferResponsesReceived.TryAdd(bootNotificationResponse);
                    return Task.CompletedTask;
                };

                // "Standard" networking mode and the networking node acts as CSMS!
                localController.OCPP.IN.     AnycastIds.      Add(NetworkingNode_Id.CSMS);
                localController.OCPP.FORWARD.AnycastIdsDenied.Add(NetworkingNode_Id.CSMS);


                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.GraphDefined_TestMessage;
                var response   = await chargingStation.TransferBinaryData(
                                           VendorId:   vendorId,
                                           MessageId:  messageId
                                       );


                Assert.Multiple(() => {

                    // Charging Station Request OUT
                    Assert.That(csBinaryDataTransferRequestsSent.     Count,                    Is.EqualTo(1), "The BinaryDataTransfer request did not leave the charging station!");
                    var csBinaryDataTransferRequest = csBinaryDataTransferRequestsSent.First();
                    Assert.That(csBinaryDataTransferRequest.DestinationId,                      Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csBinaryDataTransferRequest.NetworkPath.Length,                 Is.EqualTo(0)); // Because of "standard" networking mode
                    Assert.That(csBinaryDataTransferRequest.VendorId,                           Is.EqualTo(vendorId));
                    Assert.That(csBinaryDataTransferRequest.MessageId,                          Is.EqualTo(messageId));

                    //Assert.That(csBinaryDataTransferRequest.Signatures.Any(),                   Is.True, "The outgoing BinaryDataTransfer request is not signed!");


                    // Networking Node Binary Request IN
                    Assert.That(nnBinaryMessageRequestsReceived.      Count,                Is.EqualTo(1), "The BinaryDataTransfer Binary request did not reach the networking node!");
                    var nnBinaryMessageRequest = nnBinaryMessageRequestsReceived.First();
                    Assert.That(nnBinaryMessageRequest.DestinationId,                       Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnBinaryMessageRequest.NetworkPath.Length,                  Is.EqualTo(1));
                    Assert.That(nnBinaryMessageRequest.NetworkPath.Source,                  Is.EqualTo(chargingStation.Id));
                    Assert.That(nnBinaryMessageRequest.NetworkPath.Last,                    Is.EqualTo(chargingStation.Id));


                    // Networking Node Request IN
                    Assert.That(nnBinaryDataTransferRequestsReceived. Count,                    Is.EqualTo(1), "The BinaryDataTransfer request did not reach the networking node!");
                    var nnBinaryDataTransferRequest = nnBinaryDataTransferRequestsReceived.First();
                    Assert.That(nnBinaryDataTransferRequest.DestinationId,                      Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnBinaryDataTransferRequest.NetworkPath.Length,                 Is.EqualTo(1));
                    Assert.That(nnBinaryDataTransferRequest.NetworkPath.Source,                 Is.EqualTo(chargingStation.Id));
                    Assert.That(nnBinaryDataTransferRequest.NetworkPath.Last,                   Is.EqualTo(chargingStation.Id));
                    Assert.That(nnBinaryDataTransferRequest.VendorId,                           Is.EqualTo(vendorId));
                    Assert.That(nnBinaryDataTransferRequest.MessageId,                          Is.EqualTo(messageId));

                    //Assert.That(nnBinaryDataTransferRequest.Signatures.Any(),                   Is.True, "The incoming BinaryDataTransfer request is not signed!");
                    //var csmsBinaryDataTransferRequestSignature = nnBinaryDataTransferRequest.Signatures.First();
                    //Assert.That(csmsBinaryDataTransferRequestSignature.Status,                  Is.EqualTo(VerificationStatus.ValidSignature));


                    // Networking Node Response OUT


                    // Networking Node Binary Response OUT
                    Assert.That(nnBinaryResponseMessagesSent.     Count,                    Is.EqualTo(1), "The BinaryDataTransfer Binary response did not leave the networking node!");


                    // Charging Station Response IN
                    Assert.That(csBinaryDataTransferResponsesReceived.Count,                    Is.EqualTo(1), "The BinaryDataTransfer response did not reach the charging station!");
                    var csBinaryDataTransferResponse = csBinaryDataTransferResponsesReceived.First();
                    Assert.That(csBinaryDataTransferResponse.Request.RequestId,                 Is.EqualTo(nnBinaryDataTransferRequest.RequestId));

                    //Assert.That(csBinaryDataTransferResponse.Signatures.Any(),                  Is.True, "The incoming BinaryDataTransfer response is not signed!");
                    //var csBinaryDataTransferResponseSignature = csBinaryDataTransferResponse.Signatures.First();
                    //Assert.That(csBinaryDataTransferResponseSignature.Status,                   Is.EqualTo(VerificationStatus.ValidSignature));


                    // Result
                    Assert.That(response.Result.ResultCode,                               Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                          Is.EqualTo(BinaryDataTransferStatus.Accepted));

                });

            }

        }

        #endregion


    }

}
