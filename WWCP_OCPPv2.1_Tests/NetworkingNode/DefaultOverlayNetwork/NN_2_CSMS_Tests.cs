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
using cloud.charging.open.protocols.OCPPv2_1.LC;
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
    public class NN_2_CSMS_Tests : ADefaultOverlayNetwork
    {

        // Networking Node -> CSMS

        #region NN_2_CSMS_SendBootNotifications_Test()

        /// <summary>
        /// A test for sending signed boot notifications from a networking node to the CSMS.
        /// </summary>
        [Test]
        public async Task NN_2_CSMS_SendBootNotifications_Test()
        {

            Assert.Multiple(() => {
                Assert.That(localController,          Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (localController        is not null &&
                lcOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                var nnBootNotificationRequestsSent       = new ConcurrentList<BootNotificationRequest>();
                var nnJSONMessageRequestsSent            = new ConcurrentList<OCPP_JSONRequestMessage>();
                var csmsBootNotificationRequests         = new ConcurrentList<BootNotificationRequest>();
                var nnJSONResponseMessagesReceived       = new ConcurrentList<OCPP_JSONResponseMessage>();
                var nnBootNotificationResponsesReceived  = new ConcurrentList<BootNotificationResponse>();

                localController.OCPP.OUT.OnBootNotificationRequestSent      += (timestamp, sender,             bootNotificationRequest, sendMessageResult) => {
                    nnBootNotificationRequestsSent.TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.OUT.OnJSONRequestMessageSent           += (timestamp, sender, jsonRequestMessage, sendMessageResult) => {
                    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                CSMS.           OCPP.IN. OnBootNotificationRequestReceived  += (timestamp, sender, connection, bootNotificationRequest) => {
                    csmsBootNotificationRequests.  TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnJSONResponseMessageReceived      += (timestamp, sender, jsonResponseMessage) => {
                    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnBootNotificationResponseReceived += (timestamp, sender,             bootNotificationRequest, bootNotificationResponse, runtime) => {
                    nnBootNotificationResponsesReceived.   TryAdd(bootNotificationResponse);
                    return Task.CompletedTask;
                };


                var reason    = BootReason.PowerUp;
                var response  = await localController.SendBootNotification(
                                          BootReason:  reason
                                      );


                Assert.Multiple(() => {

                    // Networking Node Request OUT
                    Assert.That(nnBootNotificationRequestsSent.     Count,                    Is.EqualTo(1), "The BootNotification request did not leave the networking node!");
                    var nnBootNotificationRequest = nnBootNotificationRequestsSent.First();
                    Assert.That(nnBootNotificationRequest.DestinationId,                      Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnBootNotificationRequest.NetworkPath.Length,                 Is.EqualTo(1));
                    Assert.That(nnBootNotificationRequest.NetworkPath.Source,                 Is.EqualTo(localController.Id));
                    Assert.That(nnBootNotificationRequest.NetworkPath.Last,                   Is.EqualTo(localController.Id));
                    Assert.That(nnBootNotificationRequest.Reason,                             Is.EqualTo(reason));

                    Assert.That(nnBootNotificationRequest.Signatures.Any(),                   Is.True, "The outgoing BootNotification request is not signed!");


                    // Networking Node JSON Request OUT
                    Assert.That(nnJSONMessageRequestsSent.          Count,                    Is.EqualTo(1), "The BootNotification JSON request did not leave the networking node!");


                    // CSMS Request IN
                    Assert.That(csmsBootNotificationRequests.       Count,                    Is.EqualTo(1), "The BootNotification request did not reach the CSMS!");
                    var csmsBootNotificationRequest = csmsBootNotificationRequests.First();
                    Assert.That(csmsBootNotificationRequest.DestinationId,                    Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csmsBootNotificationRequest.NetworkPath.Length,               Is.EqualTo(1));
                    Assert.That(csmsBootNotificationRequest.NetworkPath.Source,               Is.EqualTo(localController.Id));
                    Assert.That(csmsBootNotificationRequest.NetworkPath.Last,                 Is.EqualTo(localController.Id));
                    Assert.That(csmsBootNotificationRequest.Reason,                           Is.EqualTo(reason));

                    Assert.That(csmsBootNotificationRequest.Signatures.Any(),                 Is.True, "The incoming BootNotification request is not signed!");
                    var csmsBootNotificationRequestSignature = csmsBootNotificationRequest.Signatures.First();
                    Assert.That(csmsBootNotificationRequestSignature.Status,                  Is.EqualTo(VerificationStatus.ValidSignature));

                    Assert.That(csmsBootNotificationRequest.ChargingStation,                  Is.Not.Null);
                    var chargingStation = csmsBootNotificationRequest.ChargingStation;
                    if (chargingStation is not null)
                    {

                        Assert.That(chargingStation.Model,             Is.EqualTo(localController.Model));
                        Assert.That(chargingStation.VendorName,        Is.EqualTo(localController.VendorName));
                        Assert.That(chargingStation.SerialNumber,      Is.EqualTo(localController.SerialNumber));
                        Assert.That(chargingStation.FirmwareVersion,   Is.EqualTo(localController.SoftwareVersion));
                        Assert.That(chargingStation.Modem,             Is.Not.Null);

                        if (chargingStation.Modem is not null &&
                            localController.Modem is not null)
                        {
                            Assert.That(chargingStation.Modem.ICCID,   Is.EqualTo(localController.Modem.ICCID));
                            Assert.That(chargingStation.Modem.IMSI,    Is.EqualTo(localController.Modem.IMSI));
                        }

                    }


                    // CSMS Response OUT


                    // Networking Node JSON Response IN
                    Assert.That(nnJSONResponseMessagesReceived.     Count,                    Is.EqualTo(1), "The BootNotification JSON request did not leave the networking node!");


                    // Networking Node Response IN
                    Assert.That(nnBootNotificationResponsesReceived.Count,                    Is.EqualTo(1), "The BootNotification response did not reach the networking node!");
                    var nnBootNotificationResponse = nnBootNotificationResponsesReceived.First();
                    Assert.That(nnBootNotificationResponse.Request.RequestId,                 Is.EqualTo(nnBootNotificationRequest.RequestId));

                    Assert.That(nnBootNotificationResponse.Signatures.Any(),                  Is.True, "The incoming BootNotification response is not signed!");
                    var nnBootNotificationResponseSignature = nnBootNotificationResponse.Signatures.First();
                    Assert.That(nnBootNotificationResponseSignature.Status,                   Is.EqualTo(VerificationStatus.ValidSignature));


                    // Result
                    Assert.That(response.Result.ResultCode,                                   Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                              Is.EqualTo(RegistrationStatus.Accepted));

                });

            }

        }

        #endregion

        #region NN_2_CSMS_Get15118EVCertificate_Test()

        /// <summary>
        /// A test for getting an ISO 15118 ev certificate from the CSMS.
        /// </summary>
        [Test]
        public async Task NN_2_CSMS_Get15118EVCertificate_Test()
        {

            Assert.Multiple(() => {
                Assert.That(localController,          Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (localController         is not null &&
                lcOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                var nnGet15118EVCertificateRequestsSent       = new ConcurrentList<Get15118EVCertificateRequest>();
                var nnJSONMessageRequestsSent                 = new ConcurrentList<OCPP_JSONRequestMessage>();
                var csmsGet15118EVCertificateRequests         = new ConcurrentList<Get15118EVCertificateRequest>();
                var nnJSONResponseMessagesReceived            = new ConcurrentList<OCPP_JSONResponseMessage>();
                var nnGet15118EVCertificateResponsesReceived  = new ConcurrentList<Get15118EVCertificateResponse>();

                localController.OCPP.OUT.OnGet15118EVCertificateRequestSent      += (timestamp, sender, get15118EVCertificateRequest) => {
                    nnGet15118EVCertificateRequestsSent.TryAdd(get15118EVCertificateRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.OUT.OnJSONRequestMessageSent                += (timestamp, sender, jsonRequestMessage, sendMessageResult) => {
                    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                CSMS.           OCPP.IN. OnGet15118EVCertificateRequestReceived  += (timestamp, sender, connection, get15118EVCertificateRequest) => {
                    csmsGet15118EVCertificateRequests.  TryAdd(get15118EVCertificateRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnJSONResponseMessageReceived           += (timestamp, sender, jsonResponseMessage) => {
                    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnGet15118EVCertificateResponseReceived += (timestamp, sender, get15118EVCertificateRequest, get15118EVCertificateResponse, runtime) => {
                    nnGet15118EVCertificateResponsesReceived.   TryAdd(get15118EVCertificateResponse);
                    return Task.CompletedTask;
                };


                var reason    = BootReason.PowerUp;
                var response  = await localController.Get15118EVCertificate(
                                          ISO15118SchemaVersion:  ISO15118SchemaVersion.Parse("xxx"),
                                          CertificateAction:      CertificateAction.Install,
                                          EXIRequest:             EXIData.Parse("xxx")
                                      );


                Assert.Multiple(() => {

                    // Networking Node Request OUT
                    Assert.That(nnGet15118EVCertificateRequestsSent.     Count,                    Is.EqualTo(1), "The Get15118EVCertificate request did not leave the networking node!");
                    var nnGet15118EVCertificateRequest = nnGet15118EVCertificateRequestsSent.First();
                    Assert.That(nnGet15118EVCertificateRequest.DestinationId,                  Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnGet15118EVCertificateRequest.NetworkPath.Length,                 Is.EqualTo(1));
                    Assert.That(nnGet15118EVCertificateRequest.NetworkPath.Source,                 Is.EqualTo(localController.Id));
                    Assert.That(nnGet15118EVCertificateRequest.NetworkPath.Last,                   Is.EqualTo(localController.Id));


                    // Networking Node JSON Request OUT
                    Assert.That(nnJSONMessageRequestsSent.               Count,                    Is.EqualTo(1), "The Get15118EVCertificate JSON request did not leave the networking node!");


                    // CSMS Request IN
                    Assert.That(csmsGet15118EVCertificateRequests.       Count,                    Is.EqualTo(1), "The Get15118EVCertificate request did not reach the CSMS!");
                    var csmsGet15118EVCertificateRequest = csmsGet15118EVCertificateRequests.First();
                    Assert.That(csmsGet15118EVCertificateRequest.DestinationId,                Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csmsGet15118EVCertificateRequest.NetworkPath.Length,               Is.EqualTo(1));
                    Assert.That(csmsGet15118EVCertificateRequest.NetworkPath.Source,               Is.EqualTo(localController.Id));
                    Assert.That(csmsGet15118EVCertificateRequest.NetworkPath.Last,                 Is.EqualTo(localController.Id));


                    // Networking Node JSON Response IN
                    Assert.That(nnJSONResponseMessagesReceived.          Count,                    Is.EqualTo(1), "The Get15118EVCertificate JSON request did not leave the networking node!");


                    // Networking Node Response IN
                    Assert.That(nnGet15118EVCertificateResponsesReceived.Count,                    Is.EqualTo(1), "The Get15118EVCertificate response did not reach the networking node!");
                    var nnGet15118EVCertificateResponse = nnGet15118EVCertificateResponsesReceived.First();
                    Assert.That(nnGet15118EVCertificateResponse.Request.RequestId,                 Is.EqualTo(nnGet15118EVCertificateRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,                                        Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                                   Is.EqualTo(ISO15118EVCertificateStatus.Accepted));

                });

            }

        }

        #endregion

        #region NN_2_CSMS_TransferData_Test()

        /// <summary>
        /// A test for sending sending signed vendor-specific data from a networking node to the CSMS.
        /// </summary>
        [Test]
        public async Task NN_2_CSMS_TransferData_Test()
        {

            Assert.Multiple(() => {
                Assert.That(localController,         Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (localController        is not null &&
                lcOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                var nnDataTransferRequestsSent       = new ConcurrentList<DataTransferRequest>();
                var nnJSONMessageRequestsSent        = new ConcurrentList<OCPP_JSONRequestMessage>();
                var csmsDataTransferRequests         = new ConcurrentList<DataTransferRequest>();
                var nnJSONResponseMessagesReceived   = new ConcurrentList<OCPP_JSONResponseMessage>();
                var nnDataTransferResponsesReceived  = new ConcurrentList<DataTransferResponse>();

                localController.OCPP.OUT.OnDataTransferRequestSent      += (timestamp, sender, dataTransferRequest, sendMessageResult) => {
                    nnDataTransferRequestsSent.TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.OUT.OnJSONRequestMessageSent       += (timestamp, sender, jsonRequestMessage, sendMessageResult) => {
                    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                CSMS.           OCPP.IN. OnDataTransferRequestReceived  += (timestamp, sender, connection, dataTransferRequest) => {
                    csmsDataTransferRequests.  TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnJSONResponseMessageReceived  += (timestamp, sender, jsonResponseMessage) => {
                    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnDataTransferResponseReceived += (timestamp, sender, dataTransferRequest, bootNotificationResponse, runtime) => {
                    nnDataTransferResponsesReceived.   TryAdd(bootNotificationResponse);
                    return Task.CompletedTask;
                };


                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.GraphDefined_TestMessage;
                var data       = "Hello world!";
                var response   = await localController.TransferData(
                                           DestinationId:   NetworkingNode_Id.CSMS,
                                           VendorId:        vendorId,
                                           MessageId:       messageId,
                                           Data:            data,
                                           CustomData:      null
                                       );


                Assert.Multiple(() => {

                    // Networking Node Request OUT
                    Assert.That(nnDataTransferRequestsSent.     Count,                    Is.EqualTo(1), "The DataTransfer request did not leave the networking node!");
                    var nnDataTransferRequest = nnDataTransferRequestsSent.First();
                    Assert.That(nnDataTransferRequest.DestinationId,                      Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnDataTransferRequest.NetworkPath.Length,                 Is.EqualTo(1));
                    Assert.That(nnDataTransferRequest.NetworkPath.Source,                 Is.EqualTo(localController.Id));
                    Assert.That(nnDataTransferRequest.NetworkPath.Last,                   Is.EqualTo(localController.Id));
                    Assert.That(nnDataTransferRequest.VendorId,                           Is.EqualTo(vendorId));
                    Assert.That(nnDataTransferRequest.MessageId,                          Is.EqualTo(messageId));
                    Assert.That(nnDataTransferRequest.Data?.ToString(),                   Is.EqualTo(data));

                    Assert.That(nnDataTransferRequest.Signatures.Any(),                   Is.True, "The outgoing DataTransfer request is not signed!");


                    // Networking Node JSON Request OUT
                    Assert.That(nnJSONMessageRequestsSent.      Count,                    Is.EqualTo(1), "The DataTransfer JSON request did not leave the networking node!");
                    var nnJSONMessageRequestSent = nnJSONMessageRequestsSent.First();
                    Assert.That(nnJSONMessageRequestSent.DestinationId,                   Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnJSONMessageRequestSent.NetworkPath.Length,              Is.EqualTo(1));
                    Assert.That(nnJSONMessageRequestSent.NetworkPath.Source,              Is.EqualTo(localController.Id));  // Because of "standard" networking mode!
                    Assert.That(nnJSONMessageRequestSent.NetworkPath.Last,                Is.EqualTo(localController.Id));  // Because of "standard" networking mode!


                    // CSMS Request IN
                    Assert.That(csmsDataTransferRequests.       Count,                    Is.EqualTo(1), "The DataTransfer request did not reach the CSMS!");
                    var csmsDataTransferRequest = csmsDataTransferRequests.First();
                    Assert.That(csmsDataTransferRequest.DestinationId,                    Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csmsDataTransferRequest.NetworkPath.Length,               Is.EqualTo(1));
                    Assert.That(csmsDataTransferRequest.NetworkPath.Source,               Is.EqualTo(localController.Id));
                    Assert.That(csmsDataTransferRequest.NetworkPath.Last,                 Is.EqualTo(localController.Id));
                    Assert.That(csmsDataTransferRequest.VendorId,                         Is.EqualTo(vendorId));
                    Assert.That(csmsDataTransferRequest.MessageId,                        Is.EqualTo(messageId));

                    Assert.That(csmsDataTransferRequest.Signatures.Any(),                 Is.True, "The incoming DataTransfer request is not signed!");
                    var csmsDataTransferRequestSignature = csmsDataTransferRequest.Signatures.First();
                    Assert.That(csmsDataTransferRequestSignature.Status,                  Is.EqualTo(VerificationStatus.ValidSignature));


                    // CSMS Response OUT


                    // Networking Node JSON Response IN
                    Assert.That(nnJSONResponseMessagesReceived. Count,                    Is.EqualTo(1), "The DataTransfer JSON request did not leave the networking node!");


                    // Networking Node Response IN
                    Assert.That(nnDataTransferResponsesReceived.Count,                    Is.EqualTo(1), "The DataTransfer response did not reach the networking node!");
                    var nnDataTransferResponse = nnDataTransferResponsesReceived.First();
                    Assert.That(nnDataTransferResponse.Request.RequestId,                 Is.EqualTo(nnDataTransferRequest.RequestId));

                    Assert.That(nnDataTransferResponse.Signatures.Any(),                  Is.True, "The incoming DataTransfer response is not signed!");
                    var nnDataTransferResponseSignature = nnDataTransferResponse.Signatures.First();
                    Assert.That(nnDataTransferResponseSignature.Status,                   Is.EqualTo(VerificationStatus.ValidSignature));


                    // Result
                    Assert.That(response.Result.ResultCode,                               Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                          Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data?.ToString(),                                Is.EqualTo(data.Reverse()));

                });

            }

        }

        #endregion


    }

}
