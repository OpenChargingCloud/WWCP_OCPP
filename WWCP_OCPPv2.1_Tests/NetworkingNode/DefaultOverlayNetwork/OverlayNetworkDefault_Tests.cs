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
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.NetworkingNode
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
    public class OverlayNetworkDefault_Tests : ADefaultOverlayNetwork
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
                Assert.That(networkingNode,          Is.Not.Null);
                Assert.That(nnOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (networkingNode         is not null &&
                nnOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                var nnBootNotificationRequestsSent       = new ConcurrentList<BootNotificationRequest>();
                var nnJSONMessageRequestsSent            = new ConcurrentList<OCPP_JSONRequestMessage>();
                var csmsBootNotificationRequests         = new ConcurrentList<BootNotificationRequest>();
                var nnJSONResponseMessagesReceived       = new ConcurrentList<OCPP_JSONResponseMessage>();
                var nnBootNotificationResponsesReceived  = new ConcurrentList<BootNotificationResponse>();

                networkingNode.OCPP.OUT.OnBootNotificationRequestSent      += (timestamp, sender,             bootNotificationRequest) => {
                    nnBootNotificationRequestsSent.TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.OUT.OnJSONMessageRequestSent           += (timestamp, sender, jsonRequestMessage) => {
                    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                CSMS.                   OnBootNotificationRequestReceived  += (timestamp, sender, connection, bootNotificationRequest) => {
                    csmsBootNotificationRequests.  TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.IN. OnJSONMessageResponseReceived      += (timestamp, sender, jsonResponseMessage) => {
                    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.IN. OnBootNotificationResponseReceived += (timestamp, sender,             bootNotificationRequest, bootNotificationResponse, runtime) => {
                    nnBootNotificationResponsesReceived.   TryAdd(bootNotificationResponse);
                    return Task.CompletedTask;
                };


                var reason    = BootReason.PowerUp;
                var response  = await networkingNode.SendBootNotification(
                                          BootReason:  reason
                                      );


                Assert.Multiple(() => {

                    // Networking Node Request OUT
                    Assert.That(nnBootNotificationRequestsSent.     Count,                    Is.EqualTo(1), "The BootNotification request did not leave the networking node!");
                    var nnBootNotificationRequest = nnBootNotificationRequestsSent.First();
                    Assert.That(nnBootNotificationRequest.DestinationNodeId,                  Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnBootNotificationRequest.NetworkPath.Length,                 Is.EqualTo(1));
                    Assert.That(nnBootNotificationRequest.NetworkPath.Source,                 Is.EqualTo(networkingNode.Id));
                    Assert.That(nnBootNotificationRequest.NetworkPath.Last,                   Is.EqualTo(networkingNode.Id));
                    Assert.That(nnBootNotificationRequest.Reason,                             Is.EqualTo(reason));

                    Assert.That(nnBootNotificationRequest.Signatures.Any(),                   Is.True, "The outgoing BootNotification request is not signed!");


                    // Networking Node JSON Request OUT
                    Assert.That(nnJSONMessageRequestsSent.          Count,                    Is.EqualTo(1), "The BootNotification JSON request did not leave the networking node!");


                    // CSMS Request IN
                    Assert.That(csmsBootNotificationRequests.       Count,                    Is.EqualTo(1), "The BootNotification request did not reach the CSMS!");
                    var csmsBootNotificationRequest = csmsBootNotificationRequests.First();
                    Assert.That(csmsBootNotificationRequest.DestinationNodeId,                Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csmsBootNotificationRequest.NetworkPath.Length,               Is.EqualTo(1));
                    Assert.That(csmsBootNotificationRequest.NetworkPath.Source,               Is.EqualTo(networkingNode.Id));
                    Assert.That(csmsBootNotificationRequest.NetworkPath.Last,                 Is.EqualTo(networkingNode.Id));
                    Assert.That(csmsBootNotificationRequest.Reason,                           Is.EqualTo(reason));

                    Assert.That(csmsBootNotificationRequest.Signatures.Any(),                 Is.True, "The incoming BootNotification request is not signed!");
                    var csmsBootNotificationRequestSignature = csmsBootNotificationRequest.Signatures.First();
                    Assert.That(csmsBootNotificationRequestSignature.Status,                  Is.EqualTo(VerificationStatus.ValidSignature));

                    Assert.That(csmsBootNotificationRequest.ChargingStation,                  Is.Not.Null);
                    var chargingStation = csmsBootNotificationRequest.ChargingStation;
                    if (chargingStation is not null)
                    {

                        Assert.That(chargingStation.Model,             Is.EqualTo(networkingNode.Model));
                        Assert.That(chargingStation.VendorName,        Is.EqualTo(networkingNode.VendorName));
                        Assert.That(chargingStation.SerialNumber,      Is.EqualTo(networkingNode.SerialNumber));
                        Assert.That(chargingStation.FirmwareVersion,   Is.EqualTo(networkingNode.FirmwareVersion));
                        Assert.That(chargingStation.Modem,             Is.Not.Null);

                        if (chargingStation.Modem is not null &&
                            networkingNode.Modem is not null)
                        {
                            Assert.That(chargingStation.Modem.ICCID,   Is.EqualTo(networkingNode.Modem.ICCID));
                            Assert.That(chargingStation.Modem.IMSI,    Is.EqualTo(networkingNode.Modem.IMSI));
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
                Assert.That(networkingNode,          Is.Not.Null);
                Assert.That(nnOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (networkingNode         is not null &&
                nnOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                var nnGet15118EVCertificateRequestsSent       = new ConcurrentList<Get15118EVCertificateRequest>();
                var nnJSONMessageRequestsSent                 = new ConcurrentList<OCPP_JSONRequestMessage>();
                var csmsGet15118EVCertificateRequests         = new ConcurrentList<Get15118EVCertificateRequest>();
                var nnJSONResponseMessagesReceived            = new ConcurrentList<OCPP_JSONResponseMessage>();
                var nnGet15118EVCertificateResponsesReceived  = new ConcurrentList<Get15118EVCertificateResponse>();

                networkingNode.OCPP.OUT.OnGet15118EVCertificateRequestSent      += (timestamp, sender, get15118EVCertificateRequest) => {
                    nnGet15118EVCertificateRequestsSent.TryAdd(get15118EVCertificateRequest);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.OUT.OnJSONMessageRequestSent                += (timestamp, sender, jsonRequestMessage) => {
                    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                CSMS.                   OnGet15118EVCertificateRequestReceived  += (timestamp, sender, connection, get15118EVCertificateRequest) => {
                    csmsGet15118EVCertificateRequests.  TryAdd(get15118EVCertificateRequest);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.IN. OnJSONMessageResponseReceived           += (timestamp, sender, jsonResponseMessage) => {
                    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.IN. OnGet15118EVCertificateResponseReceived += (timestamp, sender, get15118EVCertificateRequest, get15118EVCertificateResponse, runtime) => {
                    nnGet15118EVCertificateResponsesReceived.   TryAdd(get15118EVCertificateResponse);
                    return Task.CompletedTask;
                };


                var reason    = BootReason.PowerUp;
                var response  = await networkingNode.Get15118EVCertificate(
                                          ISO15118SchemaVersion:  ISO15118SchemaVersion.Parse("xxx"),
                                          CertificateAction:      CertificateAction.Install,
                                          EXIRequest:             EXIData.Parse("xxx")
                                      );


                Assert.Multiple(() => {

                    // Networking Node Request OUT
                    Assert.That(nnGet15118EVCertificateRequestsSent.     Count,                    Is.EqualTo(1), "The Get15118EVCertificate request did not leave the networking node!");
                    var nnGet15118EVCertificateRequest = nnGet15118EVCertificateRequestsSent.First();
                    Assert.That(nnGet15118EVCertificateRequest.DestinationNodeId,                  Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnGet15118EVCertificateRequest.NetworkPath.Length,                 Is.EqualTo(1));
                    Assert.That(nnGet15118EVCertificateRequest.NetworkPath.Source,                 Is.EqualTo(networkingNode.Id));
                    Assert.That(nnGet15118EVCertificateRequest.NetworkPath.Last,                   Is.EqualTo(networkingNode.Id));


                    // Networking Node JSON Request OUT
                    Assert.That(nnJSONMessageRequestsSent.               Count,                    Is.EqualTo(1), "The Get15118EVCertificate JSON request did not leave the networking node!");


                    // CSMS Request IN
                    Assert.That(csmsGet15118EVCertificateRequests.       Count,                    Is.EqualTo(1), "The Get15118EVCertificate request did not reach the CSMS!");
                    var csmsGet15118EVCertificateRequest = csmsGet15118EVCertificateRequests.First();
                    Assert.That(csmsGet15118EVCertificateRequest.DestinationNodeId,                Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csmsGet15118EVCertificateRequest.NetworkPath.Length,               Is.EqualTo(1));
                    Assert.That(csmsGet15118EVCertificateRequest.NetworkPath.Source,               Is.EqualTo(networkingNode.Id));
                    Assert.That(csmsGet15118EVCertificateRequest.NetworkPath.Last,                 Is.EqualTo(networkingNode.Id));


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


        // CSMS -> Networking Node

        #region CSMS_2_NN_SendReset_Test()

        /// <summary>
        /// A test for resetting a networking node via the CSMS.
        /// </summary>
        [Test]
        public async Task CSMS_2_NN_SendReset_Test()
        {

            Assert.Multiple(() => {
                Assert.That(networkingNode,          Is.Not.Null);
                Assert.That(nnOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (networkingNode         is not null &&
                nnOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                //var nnResetRequestsSent             = new ConcurrentList<ResetRequest>();
                //var nnJSONMessageRequestsSent       = new ConcurrentList<OCPP_JSONRequestMessage>();
                //var csResetRequests                 = new ConcurrentList<ResetRequest>();
                //var nnJSONResponseMessagesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
                //var nnResetResponsesReceived        = new ConcurrentList<ResetResponse>();

                //networkingNode.OCPP.OUT.OnResetRequestSent             += (timestamp, sender, resetRequest) => {
                //    nnResetRequestsSent.TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.OUT.OnJSONMessageRequestSent       += (timestamp, sender, jsonRequestMessage) => {
                //    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                //    return Task.CompletedTask;
                //};

                //chargingStation.        OnResetRequest                 += (timestamp, sender, connection, resetRequest) => {
                //    csResetRequests.               TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnJSONMessageResponseReceived  += (timestamp, sender, jsonResponseMessage) => {
                //    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnResetResponseReceived        += (timestamp, sender, resetRequest, resetResponse, runtime) => {
                //    nnResetResponsesReceived.      TryAdd(resetResponse);
                //    return Task.CompletedTask;
                //};


                var resetType  = ResetType.Immediate;
                var response   = await CSMS.Reset(
                                           DestinationNodeId:  networkingNode.Id,
                                           ResetType:          resetType
                                       );


                Assert.Multiple(() => {

                    //// Networking Node Request OUT
                    //Assert.That(nnResetRequestsSent.           Count,   Is.EqualTo(1), "The Reset request did not leave the networking node!");
                    //var nnResetRequest = nnResetRequestsSent.First();
                    //Assert.That(nnResetRequest.DestinationNodeId,       Is.EqualTo(chargingStation.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Length,      Is.EqualTo(1));
                    //Assert.That(nnResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Request OUT
                    //Assert.That(nnJSONMessageRequestsSent.     Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Charging Station Request IN
                    //Assert.That(csResetRequests.               Count,   Is.EqualTo(1), "The Reset request did not reach the charging station!");
                    //var csResetRequest = csResetRequests.First();
                    ////Assert.That(csResetRequest.DestinationNodeId,       Is.EqualTo(chargingStation.Id));   // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Length,      Is.EqualTo(1));                     // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Response IN
                    //Assert.That(nnJSONResponseMessagesReceived.Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Networking Node Response IN
                    //Assert.That(nnResetResponsesReceived.      Count,   Is.EqualTo(1), "The Reset response did not reach the networking node!");
                    //var nnResetResponse = nnResetResponsesReceived.First();
                    //Assert.That(nnResetResponse.Request.RequestId,      Is.EqualTo(nnResetRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                        Is.EqualTo(ResetStatus.Accepted));

                });

            }

        }

        #endregion


        #region CSMS_2_NN_DeleteFile_Test()

        /// <summary>
        /// A test for deleteing a file from a networking node via the CSMS.
        /// </summary>
        [Test]
        public async Task CSMS_2_NN_DeleteFile_Test()
        {

            Assert.Multiple(() => {
                Assert.That(networkingNode,          Is.Not.Null);
                Assert.That(nnOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (networkingNode         is not null &&
                nnOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                //var nnResetRequestsSent             = new ConcurrentList<ResetRequest>();
                //var nnJSONMessageRequestsSent       = new ConcurrentList<OCPP_JSONRequestMessage>();
                //var csResetRequests                 = new ConcurrentList<ResetRequest>();
                //var nnJSONResponseMessagesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
                //var nnResetResponsesReceived        = new ConcurrentList<ResetResponse>();

                //networkingNode.OCPP.OUT.OnResetRequestSent             += (timestamp, sender, resetRequest) => {
                //    nnResetRequestsSent.TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.OUT.OnJSONMessageRequestSent       += (timestamp, sender, jsonRequestMessage) => {
                //    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                //    return Task.CompletedTask;
                //};

                //chargingStation.        OnResetRequest                 += (timestamp, sender, connection, resetRequest) => {
                //    csResetRequests.               TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnJSONMessageResponseReceived  += (timestamp, sender, jsonResponseMessage) => {
                //    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnResetResponseReceived        += (timestamp, sender, resetRequest, resetResponse, runtime) => {
                //    nnResetResponsesReceived.      TryAdd(resetResponse);
                //    return Task.CompletedTask;
                //};


                var resetType  = ResetType.Immediate;
                var response   = await CSMS.DeleteFile(
                                           DestinationNodeId:  networkingNode.Id,
                                           FileName:           FilePath.Parse("/test.txt")
                                       );


                Assert.Multiple(() => {

                    //// Networking Node Request OUT
                    //Assert.That(nnResetRequestsSent.           Count,   Is.EqualTo(1), "The Reset request did not leave the networking node!");
                    //var nnResetRequest = nnResetRequestsSent.First();
                    //Assert.That(nnResetRequest.DestinationNodeId,       Is.EqualTo(chargingStation.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Length,      Is.EqualTo(1));
                    //Assert.That(nnResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Request OUT
                    //Assert.That(nnJSONMessageRequestsSent.     Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Charging Station Request IN
                    //Assert.That(csResetRequests.               Count,   Is.EqualTo(1), "The Reset request did not reach the charging station!");
                    //var csResetRequest = csResetRequests.First();
                    ////Assert.That(csResetRequest.DestinationNodeId,       Is.EqualTo(chargingStation.Id));   // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Length,      Is.EqualTo(1));                     // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Response IN
                    //Assert.That(nnJSONResponseMessagesReceived.Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Networking Node Response IN
                    //Assert.That(nnResetResponsesReceived.      Count,   Is.EqualTo(1), "The Reset response did not reach the networking node!");
                    //var nnResetResponse = nnResetResponsesReceived.First();
                    //Assert.That(nnResetResponse.Request.RequestId,      Is.EqualTo(nnResetRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                        Is.EqualTo(DeleteFileStatus.Success));

                });

            }

        }

        #endregion

        #region CSMS_2_NN_GetFile_Test()

        /// <summary>
        /// A test for getting a file from a networking node via the CSMS.
        /// </summary>
        [Test]
        public async Task CSMS_2_NN_GetFile_Test()
        {

            Assert.Multiple(() => {
                Assert.That(networkingNode,          Is.Not.Null);
                Assert.That(nnOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (networkingNode         is not null &&
                nnOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                //var nnResetRequestsSent             = new ConcurrentList<ResetRequest>();
                //var nnJSONMessageRequestsSent       = new ConcurrentList<OCPP_JSONRequestMessage>();
                //var csResetRequests                 = new ConcurrentList<ResetRequest>();
                //var nnJSONResponseMessagesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
                //var nnResetResponsesReceived        = new ConcurrentList<ResetResponse>();

                //networkingNode.OCPP.OUT.OnResetRequestSent             += (timestamp, sender, resetRequest) => {
                //    nnResetRequestsSent.TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.OUT.OnJSONMessageRequestSent       += (timestamp, sender, jsonRequestMessage) => {
                //    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                //    return Task.CompletedTask;
                //};

                //chargingStation.        OnResetRequest                 += (timestamp, sender, connection, resetRequest) => {
                //    csResetRequests.               TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnJSONMessageResponseReceived  += (timestamp, sender, jsonResponseMessage) => {
                //    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnResetResponseReceived        += (timestamp, sender, resetRequest, resetResponse, runtime) => {
                //    nnResetResponsesReceived.      TryAdd(resetResponse);
                //    return Task.CompletedTask;
                //};


                var resetType  = ResetType.Immediate;
                var response   = await CSMS.GetFile(
                                           DestinationNodeId:  networkingNode.Id,
                                           FileName:           FilePath.Parse("/test.txt")
                                       );


                Assert.Multiple(() => {

                    //// Networking Node Request OUT
                    //Assert.That(nnResetRequestsSent.           Count,   Is.EqualTo(1), "The Reset request did not leave the networking node!");
                    //var nnResetRequest = nnResetRequestsSent.First();
                    //Assert.That(nnResetRequest.DestinationNodeId,       Is.EqualTo(chargingStation.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Length,      Is.EqualTo(1));
                    //Assert.That(nnResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Request OUT
                    //Assert.That(nnJSONMessageRequestsSent.     Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Charging Station Request IN
                    //Assert.That(csResetRequests.               Count,   Is.EqualTo(1), "The Reset request did not reach the charging station!");
                    //var csResetRequest = csResetRequests.First();
                    ////Assert.That(csResetRequest.DestinationNodeId,       Is.EqualTo(chargingStation.Id));   // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Length,      Is.EqualTo(1));                     // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Response IN
                    //Assert.That(nnJSONResponseMessagesReceived.Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Networking Node Response IN
                    //Assert.That(nnResetResponsesReceived.      Count,   Is.EqualTo(1), "The Reset response did not reach the networking node!");
                    //var nnResetResponse = nnResetResponsesReceived.First();
                    //Assert.That(nnResetResponse.Request.RequestId,      Is.EqualTo(nnResetRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                        Is.EqualTo(GetFileStatus.Success));

                });

            }

        }

        #endregion

        #region CSMS_2_NN_SendFile_Test()

        /// <summary>
        /// A test for sending a file from the CSMS to a networking node.
        /// </summary>
        [Test]
        public async Task CSMS_2_NN_SendFile_Test()
        {

            Assert.Multiple(() => {
                Assert.That(networkingNode,          Is.Not.Null);
                Assert.That(nnOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (networkingNode         is not null &&
                nnOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                //var nnResetRequestsSent             = new ConcurrentList<ResetRequest>();
                //var nnJSONMessageRequestsSent       = new ConcurrentList<OCPP_JSONRequestMessage>();
                //var csResetRequests                 = new ConcurrentList<ResetRequest>();
                //var nnJSONResponseMessagesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
                //var nnResetResponsesReceived        = new ConcurrentList<ResetResponse>();

                //networkingNode.OCPP.OUT.OnResetRequestSent             += (timestamp, sender, resetRequest) => {
                //    nnResetRequestsSent.TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.OUT.OnJSONMessageRequestSent       += (timestamp, sender, jsonRequestMessage) => {
                //    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                //    return Task.CompletedTask;
                //};

                //chargingStation.        OnResetRequest                 += (timestamp, sender, connection, resetRequest) => {
                //    csResetRequests.               TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnJSONMessageResponseReceived  += (timestamp, sender, jsonResponseMessage) => {
                //    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnResetResponseReceived        += (timestamp, sender, resetRequest, resetResponse, runtime) => {
                //    nnResetResponsesReceived.      TryAdd(resetResponse);
                //    return Task.CompletedTask;
                //};


                var resetType  = ResetType.Immediate;
                var response   = await CSMS.SendFile(
                                           DestinationNodeId:  networkingNode.Id,
                                           FileName:           FilePath.Parse("/test.txt"),
                                           FileContent:        "Hello world!".ToUTF8Bytes(),
                                           FileContentType:    ContentType.Text.Plain
                                       );


                Assert.Multiple(() => {

                    //// Networking Node Request OUT
                    //Assert.That(nnResetRequestsSent.           Count,   Is.EqualTo(1), "The Reset request did not leave the networking node!");
                    //var nnResetRequest = nnResetRequestsSent.First();
                    //Assert.That(nnResetRequest.DestinationNodeId,       Is.EqualTo(chargingStation.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Length,      Is.EqualTo(1));
                    //Assert.That(nnResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Request OUT
                    //Assert.That(nnJSONMessageRequestsSent.     Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Charging Station Request IN
                    //Assert.That(csResetRequests.               Count,   Is.EqualTo(1), "The Reset request did not reach the charging station!");
                    //var csResetRequest = csResetRequests.First();
                    ////Assert.That(csResetRequest.DestinationNodeId,       Is.EqualTo(chargingStation.Id));   // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Length,      Is.EqualTo(1));                     // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Response IN
                    //Assert.That(nnJSONResponseMessagesReceived.Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Networking Node Response IN
                    //Assert.That(nnResetResponsesReceived.      Count,   Is.EqualTo(1), "The Reset response did not reach the networking node!");
                    //var nnResetResponse = nnResetResponsesReceived.First();
                    //Assert.That(nnResetResponse.Request.RequestId,      Is.EqualTo(nnResetRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                        Is.EqualTo(SendFileStatus.Success));

                });

            }

        }

        #endregion








        // Networking Node -> Charging Station

        #region NN_2_CS_SendReset_Test()

        /// <summary>
        /// A test for resetting a charging station.
        /// </summary>
        [Test]
        public async Task NN_2_CS_SendReset_Test()
        {

            Assert.Multiple(() => {
                Assert.That(networkingNode,         Is.Not.Null);
                Assert.That(nnOCPPWebSocketServer,  Is.Not.Null);
                Assert.That(chargingStation,        Is.Not.Null);
            });

            if (networkingNode         is not null &&
                nnOCPPWebSocketServer  is not null &&
                chargingStation        is not null)
            {

                var nnResetRequestsSent             = new ConcurrentList<ResetRequest>();
                var nnJSONMessageRequestsSent       = new ConcurrentList<OCPP_JSONRequestMessage>();
                var csResetRequests                 = new ConcurrentList<ResetRequest>();
                var nnJSONResponseMessagesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
                var nnResetResponsesReceived        = new ConcurrentList<ResetResponse>();

                networkingNode.OCPP.OUT.OnResetRequestSent             += (timestamp, sender, resetRequest) => {
                    nnResetRequestsSent.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.OUT.OnJSONMessageRequestSent       += (timestamp, sender, jsonRequestMessage) => {
                    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                chargingStation.        OnResetRequest                 += (timestamp, sender, connection, resetRequest) => {
                    csResetRequests.               TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.IN. OnJSONMessageResponseReceived  += (timestamp, sender, jsonResponseMessage) => {
                    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.IN. OnResetResponseReceived        += (timestamp, sender, resetRequest, resetResponse, runtime) => {
                    nnResetResponsesReceived.      TryAdd(resetResponse);
                    return Task.CompletedTask;
                };


                var resetType  = ResetType.Immediate;
                var response   = await networkingNode.Reset(
                                           DestinationNodeId:  chargingStation.Id,
                                           ResetType:          resetType
                                       );


                Assert.Multiple(() => {

                    // Networking Node Request OUT
                    Assert.That(nnResetRequestsSent.           Count,   Is.EqualTo(1), "The Reset request did not leave the networking node!");
                    var nnResetRequest = nnResetRequestsSent.First();
                    Assert.That(nnResetRequest.DestinationNodeId,       Is.EqualTo(chargingStation.Id));
                    Assert.That(nnResetRequest.NetworkPath.Length,      Is.EqualTo(1));
                    Assert.That(nnResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));
                    Assert.That(nnResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));
                    Assert.That(nnResetRequest.ResetType,               Is.EqualTo(resetType));

                    Assert.That(nnResetRequest.Signatures.Any(),        Is.True, "The outgoing Reset request is not signed!");


                    // Networking Node JSON Request OUT
                    Assert.That(nnJSONMessageRequestsSent.     Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");


                    // Charging Station Request IN
                    Assert.That(csResetRequests.               Count,   Is.EqualTo(1), "The Reset request did not reach the charging station!");
                    var csResetRequest = csResetRequests.First();
                    //Assert.That(csResetRequest.DestinationNodeId,       Is.EqualTo(chargingStation.Id));   // Because of "standard" networking mode!
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


        // Charging Station -> Networking Node

        #region CS_2_NN_SendBootNotifications_Test()

        /// <summary>
        /// A test for sending signed Boot Notifications from a charging station to a networking node.
        /// </summary>
        [Test]
        public async Task CS_2_NN_SendBootNotifications_Test()
        {

            Assert.Multiple(() => {
                Assert.That(networkingNode,         Is.Not.Null);
                Assert.That(nnOCPPWebSocketServer,  Is.Not.Null);
                Assert.That(chargingStation,        Is.Not.Null);
            });

            if (networkingNode         is not null &&
                nnOCPPWebSocketServer  is not null &&
                chargingStation        is not null)
            {

                var csBootNotificationRequestsSent       = new ConcurrentList<BootNotificationRequest>();
                var nnJSONMessageRequestsReceived        = new ConcurrentList<OCPP_JSONRequestMessage>();
                var nnBootNotificationRequestsReceived   = new ConcurrentList<BootNotificationRequest>();
                var nnBootNotificationResponsesSent      = new ConcurrentList<BootNotificationResponse>();
                var nnJSONResponseMessagesSent           = new ConcurrentList<OCPP_JSONResponseMessage>();
                var csBootNotificationResponsesReceived  = new ConcurrentList<BootNotificationResponse>();

                chargingStation.OnBootNotificationRequest                  += (timestamp, sender, bootNotificationRequest) => {
                    csBootNotificationRequestsSent.     TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.IN.OnJSONMessageRequestReceived        += (timestamp, sender, jsonRequestMessage) => {
                    nnJSONMessageRequestsReceived.      TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.IN.OnBootNotificationRequestReceived  += (timestamp, sender, connection, bootNotificationRequest) => {
                    nnBootNotificationRequestsReceived. TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                //networkingNode.OCPP.OUT.nnBootNotificationResponsesSent += (timestamp, sender, jsonResponseMessage) => {
                //    nnBootNotificationResponsesSent.    TryAdd(jsonResponseMessage);
                //    return Task.CompletedTask;
                //};

                networkingNode.OCPP.OUT.OnJSONMessageResponseSent         += (timestamp, sender, jsonResponseMessage) => {
                    nnJSONResponseMessagesSent.         TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                chargingStation.        OnBootNotificationResponse        += (timestamp, sender, bootNotificationRequest, bootNotificationResponse, runtime) => {
                    csBootNotificationResponsesReceived.TryAdd(bootNotificationResponse);
                    return Task.CompletedTask;
                };

                // "Standard" networking mode and the networking node acts as CSMS!
                networkingNode.OCPP.IN.     AnycastIds.      Add(NetworkingNode_Id.CSMS);
                networkingNode.OCPP.FORWARD.AnycastIdsDenied.Add(NetworkingNode_Id.CSMS);


                var reason    = BootReason.PowerUp;
                var response  = await chargingStation.SendBootNotification(
                                          BootReason:  reason
                                      );


                Assert.Multiple(() => {

                    // Charging Station Request OUT
                    Assert.That(csBootNotificationRequestsSent.     Count,                    Is.EqualTo(1), "The BootNotification request did not leave the charging station!");
                    var csBootNotificationRequest = csBootNotificationRequestsSent.First();
                    Assert.That(csBootNotificationRequest.DestinationNodeId,                  Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csBootNotificationRequest.NetworkPath.Length,                 Is.EqualTo(0)); // Because of "standard" networking mode
                    Assert.That(csBootNotificationRequest.Reason,                             Is.EqualTo(reason));

                    Assert.That(csBootNotificationRequest.Signatures.Any(),                   Is.True, "The outgoing BootNotification request is not signed!");


                    // Networking Node JSON Request IN
                    Assert.That(nnJSONMessageRequestsReceived.      Count,                    Is.EqualTo(1), "The BootNotification JSON request did not reach the networking node!");
                    var nnJSONMessageRequest = nnJSONMessageRequestsReceived.First();
                    Assert.That(nnJSONMessageRequest.DestinationNodeId,                       Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnJSONMessageRequest.NetworkPath.Length,                      Is.EqualTo(1));
                    Assert.That(nnJSONMessageRequest.NetworkPath.Source,                      Is.EqualTo(chargingStation.Id));
                    Assert.That(nnJSONMessageRequest.NetworkPath.Last,                        Is.EqualTo(chargingStation.Id));


                    // Networking Node Request IN
                    Assert.That(nnBootNotificationRequestsReceived. Count,                    Is.EqualTo(1), "The BootNotification request did not reach the networking node!");
                    var nnBootNotificationRequest = nnBootNotificationRequestsReceived.First();
                    Assert.That(nnBootNotificationRequest.DestinationNodeId,                  Is.EqualTo(NetworkingNode_Id.CSMS));
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
                            networkingNode.Modem is not null)
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

        #region CS_2_NN_SendDataTransfers_Test()

        /// <summary>
        /// A test for sending signed custom data from a charging station to a networking node.
        /// </summary>
        [Test]
        public async Task CS_2_NN_SendDataTransfers_Test()
        {

            Assert.Multiple(() => {
                Assert.That(networkingNode,         Is.Not.Null);
                Assert.That(nnOCPPWebSocketServer,  Is.Not.Null);
                Assert.That(chargingStation,        Is.Not.Null);
            });

            if (networkingNode         is not null &&
                nnOCPPWebSocketServer  is not null &&
                chargingStation        is not null)
            {

                var csDataTransferRequestsSent       = new ConcurrentList<DataTransferRequest>();
                var nnJSONMessageRequestsReceived    = new ConcurrentList<OCPP_JSONRequestMessage>();
                var nnDataTransferRequestsReceived   = new ConcurrentList<DataTransferRequest>();
                var nnDataTransferResponsesSent      = new ConcurrentList<DataTransferResponse>();
                var nnJSONResponseMessagesSent       = new ConcurrentList<OCPP_JSONResponseMessage>();
                var csDataTransferResponsesReceived  = new ConcurrentList<DataTransferResponse>();

                chargingStation.OnDataTransferRequestSent              += (timestamp, sender, dataTransferRequest) => {
                    csDataTransferRequestsSent.     TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.IN.OnJSONMessageRequestReceived    += (timestamp, sender, jsonRequestMessage) => {
                    nnJSONMessageRequestsReceived.  TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.IN.OnDataTransferRequestReceived   += (timestamp, sender, connection, dataTransferRequest) => {
                    nnDataTransferRequestsReceived. TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.OUT.OnDataTransferResponseSent     += (timestamp, sender, connection, dataTransferRequest, dataTransferResponse, runtime) => {
                    nnDataTransferResponsesSent.    TryAdd(dataTransferResponse);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.OUT.OnJSONMessageResponseSent      += (timestamp, sender, jsonResponseMessage) => {
                    nnJSONResponseMessagesSent.     TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                chargingStation.        OnDataTransferResponseReceived += (timestamp, sender, dataTransferRequest, dataTransferResponse, runtime) => {
                    csDataTransferResponsesReceived.TryAdd(dataTransferResponse);
                    return Task.CompletedTask;
                };

                // "Standard" networking mode and the networking node acts as CSMS!
                networkingNode.OCPP.IN.     AnycastIds.      Add(NetworkingNode_Id.CSMS);
                networkingNode.OCPP.FORWARD.AnycastIdsDenied.Add(NetworkingNode_Id.CSMS);


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
                    Assert.That(csDataTransferRequest.DestinationNodeId,                  Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csDataTransferRequest.NetworkPath.Length,                 Is.EqualTo(0)); // Because of "standard" networking mode
                    Assert.That(csDataTransferRequest.VendorId,                           Is.EqualTo(vendorId));
                    Assert.That(csDataTransferRequest.MessageId,                          Is.EqualTo(messageId));

                    Assert.That(csDataTransferRequest.Signatures.Any(),                   Is.True, "The outgoing DataTransfer request is not signed!");


                    // Networking Node JSON Request IN
                    Assert.That(nnJSONMessageRequestsReceived.  Count,                    Is.EqualTo(1), "The DataTransfer JSON request did not reach the networking node!");
                    var nnJSONMessageRequest = nnJSONMessageRequestsReceived.First();
                    Assert.That(nnJSONMessageRequest.DestinationNodeId,                   Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnJSONMessageRequest.NetworkPath.Length,                  Is.EqualTo(1));
                    Assert.That(nnJSONMessageRequest.NetworkPath.Source,                  Is.EqualTo(chargingStation.Id));
                    Assert.That(nnJSONMessageRequest.NetworkPath.Last,                    Is.EqualTo(chargingStation.Id));


                    // Networking Node Request IN
                    Assert.That(nnDataTransferRequestsReceived. Count,                    Is.EqualTo(1), "The DataTransfer request did not reach the networking node!");
                    var nnDataTransferRequestReceived = nnDataTransferRequestsReceived.First();
                    Assert.That(nnDataTransferRequestReceived.DestinationNodeId,          Is.EqualTo(NetworkingNode_Id.CSMS));
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
                    Assert.That(nnDataTransferResponseSent.DestinationNodeId,             Is.EqualTo(chargingStation.Id));
                    Assert.That(nnDataTransferResponseSent.NetworkPath.Length,            Is.EqualTo(1));
                    Assert.That(nnDataTransferResponseSent.NetworkPath.Source,            Is.EqualTo(networkingNode.Id));
                    Assert.That(nnDataTransferResponseSent.NetworkPath.Last,              Is.EqualTo(networkingNode.Id));
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

                });

            }

        }

        #endregion

        #region CS_2_NN_SendBinaryDataTransfers_Test()

        /// <summary>
        /// A test for sending signed custom binary data from a charging station to a networking node.
        /// </summary>
        [Test]
        public async Task CS_2_NN_SendBinaryDataTransfers_Test()
        {

            Assert.Multiple(() => {
                Assert.That(networkingNode,         Is.Not.Null);
                Assert.That(nnOCPPWebSocketServer,  Is.Not.Null);
                Assert.That(chargingStation,        Is.Not.Null);
            });

            if (networkingNode         is not null &&
                nnOCPPWebSocketServer  is not null &&
                chargingStation        is not null)
            {

                var csBinaryDataTransferRequestsSent       = new ConcurrentList<BinaryDataTransferRequest>();
                var nnBinaryMessageRequestsReceived        = new ConcurrentList<OCPP_BinaryRequestMessage>();
                var nnBinaryDataTransferRequestsReceived   = new ConcurrentList<BinaryDataTransferRequest>();
                var nnBinaryDataTransferResponsesSent      = new ConcurrentList<BinaryDataTransferResponse>();
                var nnBinaryResponseMessagesSent           = new ConcurrentList<OCPP_BinaryResponseMessage>();
                var csBinaryDataTransferResponsesReceived  = new ConcurrentList<BinaryDataTransferResponse>();

                chargingStation.OnBinaryDataTransferRequestSent              += (timestamp, sender, bootNotificationRequest) => {
                    csBinaryDataTransferRequestsSent.     TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.IN.OnBinaryMessageRequestReceived        += (timestamp, sender, jsonRequestMessage) => {
                    nnBinaryMessageRequestsReceived.      TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.IN.OnBinaryDataTransferRequestReceived   += (timestamp, sender, connection, bootNotificationRequest) => {
                    nnBinaryDataTransferRequestsReceived. TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                //networkingNode.OCPP.OUT.nnBinaryDataTransferResponsesSent  += (timestamp, sender, jsonResponseMessage) => {
                //    nnBinaryDataTransferResponsesSent.    TryAdd(jsonResponseMessage);
                //    return Task.CompletedTask;
                //};

                networkingNode.OCPP.OUT.OnBinaryMessageResponseSent          += (timestamp, sender, jsonResponseMessage) => {
                    nnBinaryResponseMessagesSent.         TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                chargingStation.        OnBinaryDataTransferResponseReceived += (timestamp, sender, bootNotificationRequest, bootNotificationResponse, runtime) => {
                    csBinaryDataTransferResponsesReceived.TryAdd(bootNotificationResponse);
                    return Task.CompletedTask;
                };

                // "Standard" networking mode and the networking node acts as CSMS!
                networkingNode.OCPP.IN.     AnycastIds.      Add(NetworkingNode_Id.CSMS);
                networkingNode.OCPP.FORWARD.AnycastIdsDenied.Add(NetworkingNode_Id.CSMS);


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
                    Assert.That(csBinaryDataTransferRequest.DestinationNodeId,                  Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csBinaryDataTransferRequest.NetworkPath.Length,                 Is.EqualTo(0)); // Because of "standard" networking mode
                    Assert.That(csBinaryDataTransferRequest.VendorId,                           Is.EqualTo(vendorId));
                    Assert.That(csBinaryDataTransferRequest.MessageId,                          Is.EqualTo(messageId));

                    //Assert.That(csBinaryDataTransferRequest.Signatures.Any(),                   Is.True, "The outgoing BinaryDataTransfer request is not signed!");


                    // Networking Node Binary Request IN
                    Assert.That(nnBinaryMessageRequestsReceived.      Count,                Is.EqualTo(1), "The BinaryDataTransfer Binary request did not reach the networking node!");
                    var nnBinaryMessageRequest = nnBinaryMessageRequestsReceived.First();
                    Assert.That(nnBinaryMessageRequest.DestinationNodeId,                   Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnBinaryMessageRequest.NetworkPath.Length,                  Is.EqualTo(1));
                    Assert.That(nnBinaryMessageRequest.NetworkPath.Source,                  Is.EqualTo(chargingStation.Id));
                    Assert.That(nnBinaryMessageRequest.NetworkPath.Last,                    Is.EqualTo(chargingStation.Id));


                    // Networking Node Request IN
                    Assert.That(nnBinaryDataTransferRequestsReceived. Count,                    Is.EqualTo(1), "The BinaryDataTransfer request did not reach the networking node!");
                    var nnBinaryDataTransferRequest = nnBinaryDataTransferRequestsReceived.First();
                    Assert.That(nnBinaryDataTransferRequest.DestinationNodeId,                  Is.EqualTo(NetworkingNode_Id.CSMS));
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
                Assert.That(networkingNode,          Is.Not.Null);
                Assert.That(nnOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (chargingStation        is not null &&
                networkingNode         is not null &&
                nnOCPPWebSocketServer  is not null &&
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

                networkingNode.OCPP.FORWARD.OnBootNotificationRequestLogging  += (timestamp, sender, connection, bootNotificationRequest, forwardingDecision) => {
                    nnBootNotificationRequestsForwarded.TryAdd(forwardingDecision);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.FORWARD.OnJSONRequestMessageSent          += (timestamp, sender, jsonRequestMessage, sendOCPPMessageResult) => {
                    nnJSONRequestMessagesSent.          TryAdd(new Tuple<OCPP_JSONRequestMessage, SendOCPPMessageResult>(jsonRequestMessage, sendOCPPMessageResult));
                    return Task.CompletedTask;
                };

                CSMS.                       OnBootNotificationRequestReceived += (timestamp, sender, connection, bootNotificationRequest) => {
                    csmsBootNotificationRequests.       TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.FORWARD.OnJSONResponseMessageSent         += (timestamp, sender, jsonResponseMessage, sendOCPPMessageResult) => {
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
                    var nnBootNotificationRequest = nnBootNotificationRequestsForwarded.First();
                    Assert.That(nnBootNotificationRequest.Request.DestinationNodeId,          Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnBootNotificationRequest.Request.NetworkPath.Length,         Is.EqualTo(1));
                    Assert.That(nnBootNotificationRequest.Request.NetworkPath.Source,         Is.EqualTo(chargingStation.Id));
                    Assert.That(nnBootNotificationRequest.Request.NetworkPath.Last,           Is.EqualTo(chargingStation.Id));
                    Assert.That(nnBootNotificationRequest.Request.Reason,                     Is.EqualTo(reason));
                    Assert.That(nnBootNotificationRequest.Result,                             Is.EqualTo(ForwardingResult.FORWARD));


                    // Networking Node JSON Request OUT
                    Assert.That(nnJSONRequestMessagesSent.           Count,                   Is.EqualTo(1), "The BootNotification JSON request did not leave the networking node!");
                    var nnJSONRequestMessage = nnJSONRequestMessagesSent.First();
                    Assert.That(nnJSONRequestMessage.Item1.DestinationNodeId,                 Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnJSONRequestMessage.Item1.NetworkPath.Length,                Is.EqualTo(2));
                    Assert.That(nnJSONRequestMessage.Item1.NetworkPath.Source,                Is.EqualTo(chargingStation.Id));
                    Assert.That(nnJSONRequestMessage.Item1.NetworkPath.Last,                  Is.EqualTo(networkingNode.Id));
                    Assert.That(nnJSONRequestMessage.Item1.NetworkingMode,                    Is.EqualTo(NetworkingMode.OverlayNetwork));
                    Assert.That(nnJSONRequestMessage.Item2,                                   Is.EqualTo(SendOCPPMessageResult.Success));


                    // CSMS Request IN
                    Assert.That(csmsBootNotificationRequests.       Count,                    Is.EqualTo(1), "The BootNotification request did not reach the CSMS!");
                    var csmsBootNotificationRequest = csmsBootNotificationRequests.First();
                    Assert.That(csmsBootNotificationRequest.DestinationNodeId,                Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csmsBootNotificationRequest.NetworkPath.Length,               Is.EqualTo(2));
                    Assert.That(csmsBootNotificationRequest.NetworkPath.Source,               Is.EqualTo(chargingStation.Id));
                    Assert.That(csmsBootNotificationRequest.NetworkPath.Last,                 Is.EqualTo(networkingNode. Id));
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
                    Assert.That(nnJSONResponseMessage.Item1.DestinationNodeId,                Is.EqualTo(chargingStation.Id));
                    //ToDo: network path length is 3 instead of 2 as "CSMS" is added to the list of "csms01" as the anycast is not recognized!
                    //Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Length,               Is.EqualTo(2));
                    Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Source,               Is.EqualTo(CSMS.Id));
                    Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Last,                 Is.EqualTo(networkingNode.Id));
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
                Assert.That(networkingNode,          Is.Not.Null);
                Assert.That(nnOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
            });

            if (chargingStation        is not null &&
                networkingNode         is not null &&
                nnOCPPWebSocketServer  is not null &&
                CSMS                   is not null)
            {

                var csmsResetRequestsSent           = new ConcurrentList<ResetRequest>();
                var nnResetRequestsForwarded        = new ConcurrentList<ForwardingDecision<ResetRequest, ResetResponse>>();
                var nnJSONRequestMessagesSent       = new ConcurrentList<Tuple<OCPP_JSONRequestMessage,  SendOCPPMessageResult>>();
                var csResetRequests                 = new ConcurrentList<ResetRequest>();
                var nnJSONResponseMessagesSent      = new ConcurrentList<Tuple<OCPP_JSONResponseMessage, SendOCPPMessageResult>>();
                //var nnJSONResponseMessagesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
                var csmsResetResponsesReceived      = new ConcurrentList<ResetResponse>();

                CSMS.                   OnResetRequest                 += (timestamp, sender, resetRequest) => {
                    csmsResetRequestsSent.     TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.FORWARD.OnResetRequestLogging      += (timestamp, sender, connection, resetRequest, forwardingDecision) => {
                    nnResetRequestsForwarded.  TryAdd(forwardingDecision);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.FORWARD.OnJSONRequestMessageSent   += (timestamp, sender, jsonRequestMessage, sendOCPPMessageResult) => {
                    nnJSONRequestMessagesSent. TryAdd(new Tuple<OCPP_JSONRequestMessage, SendOCPPMessageResult>(jsonRequestMessage, sendOCPPMessageResult));
                    return Task.CompletedTask;
                };

                chargingStation.            OnResetRequest             += (timestamp, sender, connection, resetRequest) => {
                    csResetRequests.           TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.FORWARD.OnJSONResponseMessageSent  += (timestamp, sender, jsonResponseMessage, sendOCPPMessageResult) => {
                    nnJSONResponseMessagesSent.TryAdd(new Tuple<OCPP_JSONResponseMessage, SendOCPPMessageResult>(jsonResponseMessage, sendOCPPMessageResult));
                    return Task.CompletedTask;
                };

                CSMS.                       OnResetResponse            += (timestamp, sender, resetRequest, resetResponse, runtime) => {
                    csmsResetResponsesReceived.TryAdd(resetResponse);
                    return Task.CompletedTask;
                };

                CSMS.               AddStaticRouting(chargingStation.Id, networkingNode.Id);
                networkingNode.OCPP.AddStaticRouting(CSMS.Id,            NetworkingNode_Id.CSMS); //Fix me!


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
                    //Assert.That(csResetRequest.DestinationNodeId,                 Is.EqualTo(chargingStation.Id));   // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.NetworkPath.Length,                Is.EqualTo(1));                     // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.NetworkPath.Source,                Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.NetworkPath.Last,                  Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    Assert.That(csResetRequest.ResetType,                         Is.EqualTo(resetType));


                    // Networking Node JSON Response FORWARD
                    Assert.That(nnJSONResponseMessagesSent.Count,                 Is.EqualTo(1), "The Reset JSON response did not leave the networking node!");
                    var nnJSONResponseMessage = nnJSONResponseMessagesSent.First();
                    //Assert.That(nnJSONResponseMessage.Item1.DestinationNodeId,    Is.EqualTo(chargingStation.Id));
                    //ToDo: network path length is 3 instead of 2 as "CSMS" is added to the list of "csms01" as the anycast is not recognized!
                    //Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Length,   Is.EqualTo(2));
                    //Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Source,   Is.EqualTo(CSMS.Id));
                    //Assert.That(nnJSONResponseMessage.Item1.NetworkPath.Last,     Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnJSONResponseMessage.Item1.NetworkingMode,       Is.EqualTo(NetworkingMode.Standard));
                    Assert.That(nnJSONResponseMessage.Item2,                      Is.EqualTo(SendOCPPMessageResult.Success));


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


    }

}
