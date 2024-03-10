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
    public class CS_2_CSMS_Tests : ADefaultOverlayNetwork
    {

        // Charging Station --Networking Node-> CSMS

        #region CS_2_CSMS_SendBootNotifications_Test()

        /// <summary>
        /// A test for sending boot notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task CS_2_CSMS_SendBootNotifications_Test()
        {

            Assert.Multiple(() => {
                Assert.That(chargingStation,         Is.Not.Null);
                Assert.That(localController,          Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (chargingStation        is not null &&
                localController         is not null &&
                lcOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                var csBootNotificationRequestsSent       = new ConcurrentList<BootNotificationRequest>();
                var nnBootNotificationRequestsForwarded  = new ConcurrentList<ForwardingDecision<BootNotificationRequest, BootNotificationResponse>>();
                var nnJSONRequestMessagesSent            = new ConcurrentList<Tuple<OCPP_JSONRequestMessage,  SendOCPPMessageResult>>();
                var csmsBootNotificationRequests         = new ConcurrentList<BootNotificationRequest>();
                var nnJSONResponseMessagesSent           = new ConcurrentList<Tuple<OCPP_JSONResponseMessage, SendOCPPMessageResult>>();
                var csBootNotificationResponsesReceived  = new ConcurrentList<BootNotificationResponse>();

                chargingStation.            OnBootNotificationRequest         += (timestamp, sender, bootNotificationRequest) => {
                    csBootNotificationRequestsSent.     TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.FORWARD.OnBootNotificationRequestLogging  += (timestamp, sender, connection, bootNotificationRequest, forwardingDecision) => {
                    nnBootNotificationRequestsForwarded.TryAdd(forwardingDecision);
                    return Task.CompletedTask;
                };

                localController.OCPP.FORWARD.OnJSONRequestMessageSent          += (timestamp, sender, jsonRequestMessage, sendOCPPMessageResult) => {
                    nnJSONRequestMessagesSent.          TryAdd(new Tuple<OCPP_JSONRequestMessage, SendOCPPMessageResult>(jsonRequestMessage, sendOCPPMessageResult));
                    return Task.CompletedTask;
                };

                CSMS.                       OnBootNotificationRequestReceived += (timestamp, sender, connection, bootNotificationRequest) => {
                    csmsBootNotificationRequests.       TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.FORWARD.OnJSONResponseMessageSent         += (timestamp, sender, jsonResponseMessage, sendOCPPMessageResult) => {
                    nnJSONResponseMessagesSent.         TryAdd(new Tuple<OCPP_JSONResponseMessage, SendOCPPMessageResult>(jsonResponseMessage, sendOCPPMessageResult));
                    return Task.CompletedTask;
                };

                chargingStation.            OnBootNotificationResponse        += (timestamp, sender, bootNotificationRequest, bootNotificationResponse, runtime) => {
                    csBootNotificationResponsesReceived.   TryAdd(bootNotificationResponse);
                    return Task.CompletedTask;
                };

                //networkingNode.AnycastIds.Add(NetworkingNode_Id.CSMS);


                var reason    = BootReason.PowerUp;
                var response  = await chargingStation.SendBootNotification(
                                          BootReason:  reason
                                      );


                Assert.Multiple(() => {

                    // Charging Station JSON Request OUT
                    Assert.That(csBootNotificationRequestsSent.     Count,                    Is.EqualTo(1), "The BootNotification JSON request did not leave the charging station!");
                    var csBootNotificationRequest = csBootNotificationRequestsSent.First();
                    Assert.That(csBootNotificationRequest.Signatures.Any(),                   Is.True, "The outgoing BootNotification request is not signed!");


                    // Networking Node Request FORWARD
                    Assert.That(nnBootNotificationRequestsForwarded.Count,                    Is.EqualTo(1), "The BootNotification request did not reach the networking node!");
                    var nnBootNotification = nnBootNotificationRequestsForwarded.First();
                    Assert.That(nnBootNotification.Request.DestinationNodeId,                 Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnBootNotification.Request.NetworkPath.Length,                Is.EqualTo(1));
                    Assert.That(nnBootNotification.Request.NetworkPath.Source,                Is.EqualTo(chargingStation.Id));
                    Assert.That(nnBootNotification.Request.NetworkPath.Last,                  Is.EqualTo(chargingStation.Id));
                    Assert.That(nnBootNotification.Request.Reason,                            Is.EqualTo(reason));
                    Assert.That(nnBootNotification.Result,                                    Is.EqualTo(ForwardingResult.FORWARD));


                    // Networking Node JSON Request OUT
                    Assert.That(nnJSONRequestMessagesSent.           Count,                   Is.EqualTo(1), "The BootNotification JSON request did not leave the networking node!");
                    var nnJSONRequestMessage = nnJSONRequestMessagesSent.First();
                    Assert.That(nnJSONRequestMessage.Item1.DestinationId,                 Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnJSONRequestMessage.Item1.NetworkPath.Length,                Is.EqualTo(2));
                    Assert.That(nnJSONRequestMessage.Item1.NetworkPath.Source,                Is.EqualTo(chargingStation.Id));
                    Assert.That(nnJSONRequestMessage.Item1.NetworkPath.Last,                  Is.EqualTo(localController.Id));
                    Assert.That(nnJSONRequestMessage.Item1.NetworkingMode,                    Is.EqualTo(NetworkingMode.OverlayNetwork));
                    Assert.That(nnJSONRequestMessage.Item2,                                   Is.EqualTo(SendOCPPMessageResult.Success));


                    // CSMS Request IN
                    Assert.That(csmsBootNotificationRequests.       Count,                    Is.EqualTo(1), "The BootNotification request did not reach the CSMS!");
                    var csmsBootNotificationRequest = csmsBootNotificationRequests.First();
                    Assert.That(csmsBootNotificationRequest.DestinationNodeId,                Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csmsBootNotificationRequest.NetworkPath.Length,               Is.EqualTo(2));
                    Assert.That(csmsBootNotificationRequest.NetworkPath.Source,               Is.EqualTo(chargingStation.Id));
                    Assert.That(csmsBootNotificationRequest.NetworkPath.Last,                 Is.EqualTo(localController. Id));
                    Assert.That(csmsBootNotificationRequest.Reason,                           Is.EqualTo(reason));

                    Assert.That(csmsBootNotificationRequest.ChargingStation,                  Is.Not.Null);
                    var bootNotificationChargingStation = csmsBootNotificationRequests.First().ChargingStation;
                    if (bootNotificationChargingStation is not null)
                    {

                        Assert.That(bootNotificationChargingStation.Model,                    Is.EqualTo(chargingStation.Model));
                        Assert.That(bootNotificationChargingStation.VendorName,               Is.EqualTo(chargingStation.VendorName));
                        Assert.That(bootNotificationChargingStation.SerialNumber,             Is.EqualTo(chargingStation.SerialNumber));
                        Assert.That(bootNotificationChargingStation.FirmwareVersion,          Is.EqualTo(chargingStation.FirmwareVersion));
                        Assert.That(bootNotificationChargingStation.Modem,                    Is.Not.Null);

                        if (bootNotificationChargingStation.Modem is not null &&
                            chargingStation.                Modem is not null)
                        {
                            Assert.That(bootNotificationChargingStation.Modem.ICCID,          Is.EqualTo(chargingStation.Modem.ICCID));
                            Assert.That(bootNotificationChargingStation.Modem.IMSI,           Is.EqualTo(chargingStation.Modem.IMSI));
                        }

                    }


                    // Networking Node JSON Response FORWARD
                    Assert.That(nnJSONResponseMessagesSent.Count,                             Is.EqualTo(1), "The BootNotification JSON response did not leave the networking node!");
                    var nnJSONResponseMessage = nnJSONResponseMessagesSent.First();
                    Assert.That(nnJSONResponseMessage.Item1.DestinationId,                Is.EqualTo(chargingStation.Id));
                    //ToDo: network path length is 3 instead of 2 as "CSMS" is added to the list of "csms01" as the anycast is not recognized!
                    //Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Length,               Is.EqualTo(2));
                    Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Source,               Is.EqualTo(CSMS.Id));
                    Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Last,                 Is.EqualTo(localController.Id));
                    Assert.That(nnJSONResponseMessage.Item1.NetworkingMode,                   Is.EqualTo(NetworkingMode.Standard));
                    Assert.That(nnJSONResponseMessage.Item2,                                  Is.EqualTo(SendOCPPMessageResult.Success));


                    // Charging Station Response IN
                    Assert.That(csBootNotificationResponsesReceived.Count,                    Is.EqualTo(1), "The BootNotification response did not reach the networking node!");
                    var csBootNotificationResponse = csBootNotificationResponsesReceived.First();
                    Assert.That(csBootNotificationResponse.Request.RequestId,                 Is.EqualTo(csBootNotificationRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,                                   Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                              Is.EqualTo(RegistrationStatus.Accepted));

                });

            }

        }

        #endregion


        #region CS_2_CSMS_TransferData_ACCEPTED_Test()

        /// <summary>
        /// A test for sending signed vendor-specific data from a charging station to the CSMS.
        /// </summary>
        [Test]
        public async Task CS_2_CSMS_TransferData_ACCEPTED_Test()
        {

            Assert.Multiple(() => {
                Assert.That(chargingStation,         Is.Not.Null);
                Assert.That(localController,          Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (chargingStation        is not null &&
                localController         is not null &&
                lcOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                var csDataTransferRequestsSent       = new ConcurrentList<DataTransferRequest>();
                var nnDataTransferRequestsForwarded  = new ConcurrentList<ForwardingDecision<DataTransferRequest, DataTransferResponse>>();
                var nnJSONRequestMessagesSent        = new ConcurrentList<Tuple<OCPP_JSONRequestMessage,  SendOCPPMessageResult>>();
                var csmsDataTransferRequests         = new ConcurrentList<DataTransferRequest>();
                var nnJSONResponseMessagesSent       = new ConcurrentList<Tuple<OCPP_JSONResponseMessage, SendOCPPMessageResult>>();
                var csDataTransferResponsesReceived  = new ConcurrentList<DataTransferResponse>();

                chargingStation.            OnDataTransferRequestSent      += (timestamp, sender, dataTransferRequest) => {
                    csDataTransferRequestsSent.     TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.FORWARD.OnDataTransferRequestLogging   += (timestamp, sender, connection, dataTransferRequest, forwardingDecision) => {
                    nnDataTransferRequestsForwarded.TryAdd(forwardingDecision);
                    return Task.CompletedTask;
                };

                localController.OCPP.FORWARD.OnJSONRequestMessageSent       += (timestamp, sender, jsonRequestMessage, sendOCPPMessageResult) => {
                    nnJSONRequestMessagesSent.          TryAdd(new Tuple<OCPP_JSONRequestMessage, SendOCPPMessageResult>(jsonRequestMessage, sendOCPPMessageResult));
                    return Task.CompletedTask;
                };

                CSMS.                       OnDataTransferRequestReceived  += (timestamp, sender, connection, dataTransferRequest) => {
                    csmsDataTransferRequests.       TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.FORWARD.OnJSONResponseMessageSent      += (timestamp, sender, jsonResponseMessage, sendOCPPMessageResult) => {
                    nnJSONResponseMessagesSent.         TryAdd(new Tuple<OCPP_JSONResponseMessage, SendOCPPMessageResult>(jsonResponseMessage, sendOCPPMessageResult));
                    return Task.CompletedTask;
                };

                chargingStation.            OnDataTransferResponseReceived += (timestamp, sender, dataTransferRequest, dataTransferResponse, runtime) => {
                    csDataTransferResponsesReceived.   TryAdd(dataTransferResponse);
                    return Task.CompletedTask;
                };

                //networkingNode.AnycastIds.Add(NetworkingNode_Id.CSMS);


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

                    // Charging Station JSON Request OUT
                    Assert.That(csDataTransferRequestsSent.     Count,                    Is.EqualTo(1), "The DataTransfer JSON request did not leave the charging station!");
                    var csDataTransferRequest = csDataTransferRequestsSent.First();
                    Assert.That(csDataTransferRequest.DestinationNodeId,                  Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csDataTransferRequest.NetworkPath.Length,                 Is.EqualTo(0)); // Because of "standard" networking mode
                    Assert.That(csDataTransferRequest.VendorId,                           Is.EqualTo(vendorId));
                    Assert.That(csDataTransferRequest.MessageId,                          Is.EqualTo(messageId));
                    Assert.That(csDataTransferRequest.Data?.ToString(),                   Is.EqualTo(data));

                    Assert.That(csDataTransferRequest.Signatures.Any(),                   Is.True, "The outgoing DataTransfer request is not signed!");


                    // Networking Node Request FORWARD
                    Assert.That(nnDataTransferRequestsForwarded.Count,                    Is.EqualTo(1), "The DataTransfer request did not reach the networking node!");
                    var nnDataTransfer = nnDataTransferRequestsForwarded.First();
                    Assert.That(nnDataTransfer.Request.DestinationNodeId,                 Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnDataTransfer.Request.NetworkPath.Length,                Is.EqualTo(1));
                    Assert.That(nnDataTransfer.Request.NetworkPath.Source,                Is.EqualTo(chargingStation.Id));
                    Assert.That(nnDataTransfer.Request.NetworkPath.Last,                  Is.EqualTo(chargingStation.Id));
                    Assert.That(nnDataTransfer.Request.VendorId,                          Is.EqualTo(vendorId));
                    Assert.That(nnDataTransfer.Request.MessageId,                         Is.EqualTo(messageId));
                    Assert.That(nnDataTransfer.Request.Data?.ToString(),                  Is.EqualTo(data));
                    Assert.That(nnDataTransfer.Result,                                    Is.EqualTo(ForwardingResult.FORWARD));


                    // Networking Node JSON Request OUT
                    Assert.That(nnJSONRequestMessagesSent.           Count,               Is.EqualTo(1), "The DataTransfer JSON request did not leave the networking node!");
                    var nnJSONRequestMessage = nnJSONRequestMessagesSent.First();
                    Assert.That(nnJSONRequestMessage.Item1.DestinationId,             Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnJSONRequestMessage.Item1.NetworkPath.Length,            Is.EqualTo(2));
                    Assert.That(nnJSONRequestMessage.Item1.NetworkPath.Source,            Is.EqualTo(chargingStation.Id));
                    Assert.That(nnJSONRequestMessage.Item1.NetworkPath.Last,              Is.EqualTo(localController.Id));
                    Assert.That(nnJSONRequestMessage.Item1.NetworkingMode,                Is.EqualTo(NetworkingMode.OverlayNetwork));
                    Assert.That(nnJSONRequestMessage.Item2,                               Is.EqualTo(SendOCPPMessageResult.Success));


                    // CSMS Request IN
                    Assert.That(csmsDataTransferRequests.       Count,                    Is.EqualTo(1), "The DataTransfer request did not reach the CSMS!");
                    var csmsDataTransferRequest = csmsDataTransferRequests.First();
                    Assert.That(csmsDataTransferRequest.DestinationNodeId,                Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csmsDataTransferRequest.NetworkPath.Length,               Is.EqualTo(2));
                    Assert.That(csmsDataTransferRequest.NetworkPath.Source,               Is.EqualTo(chargingStation.Id));
                    Assert.That(csmsDataTransferRequest.NetworkPath.Last,                 Is.EqualTo(localController. Id));
                    Assert.That(csmsDataTransferRequest.VendorId,                         Is.EqualTo(vendorId));
                    Assert.That(csmsDataTransferRequest.MessageId,                        Is.EqualTo(messageId));
                    Assert.That(csmsDataTransferRequest.Data?.ToString(),                 Is.EqualTo(data));

                    Assert.That(csmsDataTransferRequest.Signatures.Any(),                 Is.True, "The incoming DataTransfer request at the CSMS is not signed!");


                    // Networking Node JSON Response FORWARD
                    Assert.That(nnJSONResponseMessagesSent.Count,                             Is.EqualTo(1), "The DataTransfer JSON response did not leave the networking node!");
                    var nnJSONResponseMessage = nnJSONResponseMessagesSent.First();
                    Assert.That(nnJSONResponseMessage.Item1.DestinationId,                Is.EqualTo(chargingStation.Id));
                    //ToDo: network path length is 3 instead of 2 as "CSMS" is added to the list of "csms01" as the anycast is not recognized!
                    //Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Length,               Is.EqualTo(2));
                    Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Source,               Is.EqualTo(CSMS.Id));
                    Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Last,                 Is.EqualTo(localController.Id));
                    Assert.That(nnJSONResponseMessage.Item1.NetworkingMode,                   Is.EqualTo(NetworkingMode.Standard));
                    Assert.That(nnJSONResponseMessage.Item2,                                  Is.EqualTo(SendOCPPMessageResult.Success));


                    // Charging Station Response IN
                    Assert.That(csDataTransferResponsesReceived.Count,                    Is.EqualTo(1), "The DataTransfer response did not reach the networking node!");
                    var csDataTransferResponse = csDataTransferResponsesReceived.First();
                    Assert.That(csDataTransferResponse.Request.RequestId,                 Is.EqualTo(csDataTransferRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,                               Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                          Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data?.ToString(),                                Is.EqualTo(data.Reverse()));

                });

            }

        }

        #endregion

        #region CS_2_CSMS_TransferData_REJECTED_Test()

        /// <summary>
        /// A test for sending signed vendor-specific data from a charging station to the CSMS,
        /// but the request is REJECTED by the networking node!
        /// </summary>
        [Test]
        public async Task CS_2_CSMS_TransferData_REJECTED_Test()
        {

            Assert.Multiple(() => {
                Assert.That(chargingStation,         Is.Not.Null);
                Assert.That(localController,          Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (chargingStation        is not null &&
                localController         is not null &&
                lcOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                var csDataTransferRequestsSent       = new ConcurrentList<DataTransferRequest>();
                var nnDataTransferRequestsForwarded  = new ConcurrentList<ForwardingDecision<DataTransferRequest, DataTransferResponse>>();
                var nnJSONRequestMessagesSent        = new ConcurrentList<Tuple<OCPP_JSONRequestMessage,  SendOCPPMessageResult>>();
                var csmsDataTransferRequests         = new ConcurrentList<DataTransferRequest>();
                var nnJSONResponseMessagesSent       = new ConcurrentList<Tuple<OCPP_JSONResponseMessage, SendOCPPMessageResult>>();
                var csDataTransferResponsesReceived  = new ConcurrentList<DataTransferResponse>();

                chargingStation.            OnDataTransferRequestSent      += (timestamp, sender, dataTransferRequest) => {
                    csDataTransferRequestsSent.     TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.FORWARD.OnDataTransferRequestLogging   += (timestamp, sender, connection, dataTransferRequest, forwardingDecision) => {
                    nnDataTransferRequestsForwarded.TryAdd(forwardingDecision);
                    return Task.CompletedTask;
                };

                localController.OCPP.FORWARD.OnJSONRequestMessageSent       += (timestamp, sender, jsonRequestMessage, sendOCPPMessageResult) => {
                    nnJSONRequestMessagesSent.          TryAdd(new Tuple<OCPP_JSONRequestMessage, SendOCPPMessageResult>(jsonRequestMessage, sendOCPPMessageResult));
                    return Task.CompletedTask;
                };

                CSMS.                       OnDataTransferRequestReceived  += (timestamp, sender, connection, dataTransferRequest) => {
                    csmsDataTransferRequests.       TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.FORWARD.OnJSONResponseMessageSent      += (timestamp, sender, jsonResponseMessage, sendOCPPMessageResult) => {
                    nnJSONResponseMessagesSent.         TryAdd(new Tuple<OCPP_JSONResponseMessage, SendOCPPMessageResult>(jsonResponseMessage, sendOCPPMessageResult));
                    return Task.CompletedTask;
                };

                chargingStation.            OnDataTransferResponseReceived += (timestamp, sender, dataTransferRequest, dataTransferResponse, runtime) => {
                    csDataTransferResponsesReceived.   TryAdd(dataTransferResponse);
                    return Task.CompletedTask;
                };

                //networkingNode.AnycastIds.Add(NetworkingNode_Id.CSMS);


                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.GraphDefined_TestMessage;
                var data       = "Please REJECT!";
                var response   = await chargingStation.TransferData(
                                           VendorId:     vendorId,
                                           MessageId:    messageId,
                                           Data:         data,
                                           CustomData:   null
                                       );


                Assert.Multiple(() => {

                    // Charging Station JSON Request OUT
                    Assert.That(csDataTransferRequestsSent.     Count,                    Is.EqualTo(1), "The DataTransfer JSON request did not leave the charging station!");
                    var csDataTransferRequest = csDataTransferRequestsSent.First();
                    Assert.That(csDataTransferRequest.DestinationNodeId,                  Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csDataTransferRequest.NetworkPath.Length,                 Is.EqualTo(0)); // Because of "standard" networking mode
                    Assert.That(csDataTransferRequest.VendorId,                           Is.EqualTo(vendorId));
                    Assert.That(csDataTransferRequest.MessageId,                          Is.EqualTo(messageId));
                    Assert.That(csDataTransferRequest.Data?.ToString(),                   Is.EqualTo(data));

                    Assert.That(csDataTransferRequest.Signatures.Any(),                   Is.True, "The outgoing DataTransfer request is not signed!");


                    // Networking Node Request FORWARD
                    Assert.That(nnDataTransferRequestsForwarded.Count,                    Is.EqualTo(1), "The DataTransfer request did not reach the networking node!");
                    var nnDataTransfer = nnDataTransferRequestsForwarded.First();
                    Assert.That(nnDataTransfer.Request.DestinationNodeId,                 Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnDataTransfer.Request.NetworkPath.Length,                Is.EqualTo(1));
                    Assert.That(nnDataTransfer.Request.NetworkPath.Source,                Is.EqualTo(chargingStation.Id));
                    Assert.That(nnDataTransfer.Request.NetworkPath.Last,                  Is.EqualTo(chargingStation.Id));
                    Assert.That(nnDataTransfer.Request.VendorId,                          Is.EqualTo(vendorId));
                    Assert.That(nnDataTransfer.Request.MessageId,                         Is.EqualTo(messageId));
                    Assert.That(nnDataTransfer.Request.Data?.ToString(),                  Is.EqualTo(data));
                    Assert.That(nnDataTransfer.Result,                                    Is.EqualTo(ForwardingResult.REJECT));


                    //// Networking Node JSON Request OUT
                    //Assert.That(nnJSONRequestMessagesSent.           Count,               Is.EqualTo(1), "The DataTransfer JSON request did not leave the networking node!");
                    //var nnJSONRequestMessage = nnJSONRequestMessagesSent.First();
                    //Assert.That(nnJSONRequestMessage.Item1.DestinationNodeId,             Is.EqualTo(NetworkingNode_Id.CSMS));
                    //Assert.That(nnJSONRequestMessage.Item1.NetworkPath.Length,            Is.EqualTo(2));
                    //Assert.That(nnJSONRequestMessage.Item1.NetworkPath.Source,            Is.EqualTo(chargingStation.Id));
                    //Assert.That(nnJSONRequestMessage.Item1.NetworkPath.Last,              Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnJSONRequestMessage.Item1.NetworkingMode,                Is.EqualTo(NetworkingMode.OverlayNetwork));
                    //Assert.That(nnJSONRequestMessage.Item2,                               Is.EqualTo(SendOCPPMessageResult.Success));


                    //// CSMS Request IN
                    //Assert.That(csmsDataTransferRequests.       Count,                    Is.EqualTo(1), "The DataTransfer request did not reach the CSMS!");
                    //var csmsDataTransferRequest = csmsDataTransferRequests.First();
                    //Assert.That(csmsDataTransferRequest.DestinationNodeId,                Is.EqualTo(NetworkingNode_Id.CSMS));
                    //Assert.That(csmsDataTransferRequest.NetworkPath.Length,               Is.EqualTo(2));
                    //Assert.That(csmsDataTransferRequest.NetworkPath.Source,               Is.EqualTo(chargingStation.Id));
                    //Assert.That(csmsDataTransferRequest.NetworkPath.Last,                 Is.EqualTo(networkingNode. Id));
                    //Assert.That(csmsDataTransferRequest.VendorId,                         Is.EqualTo(vendorId));
                    //Assert.That(csmsDataTransferRequest.MessageId,                        Is.EqualTo(messageId));
                    //Assert.That(csmsDataTransferRequest.Data?.ToString(),                 Is.EqualTo(data));

                    //Assert.That(csmsDataTransferRequest.Signatures.Any(),                 Is.True, "The incoming DataTransfer request at the CSMS is not signed!");


                    //// Networking Node JSON Response FORWARD
                    //Assert.That(nnJSONResponseMessagesSent.Count,                             Is.EqualTo(1), "The DataTransfer JSON response did not leave the networking node!");
                    //var nnJSONResponseMessage = nnJSONResponseMessagesSent.First();
                    //Assert.That(nnJSONResponseMessage.Item1.DestinationNodeId,                Is.EqualTo(chargingStation.Id));
                    ////ToDo: network path length is 3 instead of 2 as "CSMS" is added to the list of "csms01" as the anycast is not recognized!
                    ////Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Length,               Is.EqualTo(2));
                    //Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Source,               Is.EqualTo(CSMS.Id));
                    //Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Last,                 Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnJSONResponseMessage.Item1.NetworkingMode,                   Is.EqualTo(NetworkingMode.Standard));
                    //Assert.That(nnJSONResponseMessage.Item2,                                  Is.EqualTo(SendOCPPMessageResult.Success));


                    // Charging Station Response IN
                    Assert.That(csDataTransferResponsesReceived.Count,                    Is.EqualTo(1), "The DataTransfer response did not reach the networking node!");
                    var csDataTransferResponse = csDataTransferResponsesReceived.First();
                    Assert.That(csDataTransferResponse.Request.RequestId,                 Is.EqualTo(csDataTransferRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,                               Is.EqualTo(ResultCode.Filtered));
                    Assert.That(response.Status,                                          Is.EqualTo(DataTransferStatus.Rejected));
                    //Assert.That(response.Data?.ToString(),                                Is.EqualTo(data.Reverse()));

                });

            }

        }

        #endregion


    }

}
