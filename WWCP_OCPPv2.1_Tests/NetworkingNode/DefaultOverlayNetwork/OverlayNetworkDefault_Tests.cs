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

namespace cloud.charging.open.protocols.OCPPv2_1.tests.NetworkingNode.NN
{

    /// <summary>
    /// Unit tests for networking nodes sending messages to the CSMS.
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
                    var chargingStation = csmsBootNotificationRequests.First().ChargingStation;
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
                Assert.That(networkingNode,          Is.Not.Null);
                Assert.That(nnOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(chargingStation,         Is.Not.Null);
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
                var nnJSONRequestMessagesSent            = new ConcurrentList<OCPP_JSONRequestMessage>();
                var csmsBootNotificationRequests         = new ConcurrentList<BootNotificationRequest>();
                var nnJSONResponseMessagesSent           = new ConcurrentList<OCPP_JSONResponseMessage>();
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
                    nnJSONRequestMessagesSent.          TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                CSMS.                       OnBootNotificationRequestReceived += (timestamp, sender, connection, bootNotificationRequest) => {
                    csmsBootNotificationRequests.       TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.FORWARD.OnJSONResponseMessageSent         += (timestamp, sender, jsonResponseMessage, sendOCPPMessageResult) => {
                    nnJSONResponseMessagesSent.         TryAdd(jsonResponseMessage);
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
                    Assert.That(nnJSONRequestMessage.DestinationNodeId,                       Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnJSONRequestMessage.NetworkPath.Length,                      Is.EqualTo(2));
                    Assert.That(nnJSONRequestMessage.NetworkPath.Source,                      Is.EqualTo(chargingStation.Id));
                    Assert.That(nnJSONRequestMessage.NetworkPath.Last,                        Is.EqualTo(networkingNode.Id));
                    Assert.That(nnJSONRequestMessage.NetworkingMode,                          Is.EqualTo(NetworkingMode.OverlayNetwork));


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


                    // Networking Node JSON Response OUT
                    Assert.That(nnJSONResponseMessagesSent.Count,                             Is.EqualTo(1), "The BootNotification JSON response did not leave the networking node!");
                    var nnJSONResponseMessage = nnJSONResponseMessagesSent.First();
                    Assert.That(nnJSONResponseMessage.DestinationNodeId,                      Is.EqualTo(chargingStation.Id));
                    //Assert.That(nnJSONResponseMessage.NetworkPath.Length,                     Is.EqualTo(2));
                    Assert.That(nnJSONResponseMessage.NetworkPath.Source,                     Is.EqualTo(chargingStation.Id));
                    Assert.That(nnJSONResponseMessage.NetworkPath.Last,                       Is.EqualTo(networkingNode.Id));
                    Assert.That(nnJSONResponseMessage.NetworkingMode,                         Is.EqualTo(NetworkingMode.OverlayNetwork));

                    //// Networking Node Response IN
                    //Assert.That(nnBootNotificationResponsesReceived.Count,                    Is.EqualTo(1), "The BootNotification response did not reach the networking node!");
                    //var nnBootNotificationResponse = nnBootNotificationResponsesReceived.First();
                    //Assert.That(nnBootNotificationResponse.Request.RequestId,                 Is.EqualTo(nnBootNotificationRequest.RequestId));


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
                var response   = await CSMS.Reset(
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


                    // Result
                    Assert.That(response.Result.ResultCode,             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                        Is.EqualTo(ResetStatus.Accepted));

                });

            }

        }

        #endregion


    }

}
