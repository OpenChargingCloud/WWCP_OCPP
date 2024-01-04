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
using cloud.charging.open.protocols.OCPP.NN;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.NN;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPP.CS;
using Microsoft.AspNetCore.Http.HttpResults;

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

        #region NN_SendBootNotifications_toCSMS_Test()

        /// <summary>
        /// A test for sending boot notifications from a networking node to the CSMS.
        /// </summary>
        [Test]
        public async Task NN_SendBootNotifications_toCSMS_Test()
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


                    // Networking Node JSON Response IN
                    Assert.That(nnJSONResponseMessagesReceived.     Count,                    Is.EqualTo(1), "The BootNotification JSON request did not leave the networking node!");


                    // Networking Node Response IN
                    Assert.That(nnBootNotificationResponsesReceived.Count,                    Is.EqualTo(1), "The BootNotification response did not reach the networking node!");
                    var nnBootNotificationResponse = nnBootNotificationResponsesReceived.First();
                    Assert.That(nnBootNotificationResponse.Request.RequestId,                 Is.EqualTo(nnBootNotificationRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,                                   Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                              Is.EqualTo(RegistrationStatus.Accepted));

                });

            }

        }

        #endregion


        // CSMS -> Networking Node

        #region CSMS_SendReset_toNetworkingNode_Test()

        /// <summary>
        /// A test for resetting a networking node via the CSMS.
        /// </summary>
        [Test]
        public async Task CSMS_SendReset_toNetworkingNode_Test()
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

        #region CSMS_GetFile_toNetworkingNode_Test()

        /// <summary>
        /// A test for getting a file from a networking node via the CSMS.
        /// </summary>
        [Test]
        public async Task CSMS_GetFile_toNetworkingNode_Test()
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
                                           Filename:           FilePath.Parse("/test.txt")
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

        #region CSMS_SendFile_toNetworkingNode_Test()

        /// <summary>
        /// A test for sending a file from the CSMS to a networking node.
        /// </summary>
        [Test]
        public async Task CSMS_SendFile_toNetworkingNode_Test()
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

        #region NN_SendReset_toChargingStation_Test()

        /// <summary>
        /// A test for resetting a charging station.
        /// </summary>
        [Test]
        public async Task NN_SendReset_toChargingStation_Test()
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



        // Charging Station --Networking Node-> CSMS

        #region CS_SendBootNotifications_toCSMS_Test()

        /// <summary>
        /// A test for sending boot notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task CS_SendBootNotifications_toCSMS_Test()
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
                var nnBootNotificationRequestsReceived   = new ConcurrentList<BootNotificationRequest>();

                var nnBootNotificationRequestsSent       = new ConcurrentList<BootNotificationRequest>();
                var nnJSONMessageRequestsSent            = new ConcurrentList<OCPP_JSONRequestMessage>();
                var csmsBootNotificationRequests         = new ConcurrentList<BootNotificationRequest>();
                var nnJSONResponseMessagesReceived       = new ConcurrentList<OCPP_JSONResponseMessage>();
                var nnBootNotificationResponsesReceived  = new ConcurrentList<BootNotificationResponse>();
                var csBootNotificationResponsesReceived  = new ConcurrentList<BootNotificationResponse>();

                chargingStation.        OnBootNotificationRequest          += (timestamp, sender,             bootNotificationRequest) => {
                    csBootNotificationRequestsSent.TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                networkingNode.OCPP.IN. OnBootNotificationRequestReceived  += (timestamp, sender, connection, bootNotificationRequest) => {
                    nnBootNotificationRequestsReceived.TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };






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

                chargingStation.        OnBootNotificationResponse         += (timestamp, sender,             bootNotificationRequest, bootNotificationResponse, runtime) => {
                    csBootNotificationResponsesReceived.   TryAdd(bootNotificationResponse);
                    return Task.CompletedTask;
                };

                networkingNode.AnycastIds.Add(NetworkingNode_Id.CSMS);


                var reason    = BootReason.PowerUp;
                var response  = await chargingStation.SendBootNotification(
                                          BootReason:  reason
                                      );


                Assert.Multiple(() => {

                    // Charging Station JSON Request OUT
                    Assert.That(csBootNotificationRequestsSent.     Count,                    Is.EqualTo(1), "The BootNotification JSON request did not leave the charging station!");

                    // Networking Node Request OUT
                    Assert.That(nnBootNotificationRequestsSent.     Count,                    Is.EqualTo(1), "The BootNotification request did not leave the networking node!");
                    var nnBootNotificationRequest = nnBootNotificationRequestsSent.First();
                    Assert.That(nnBootNotificationRequest.DestinationNodeId,                  Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnBootNotificationRequest.NetworkPath.Length,                 Is.EqualTo(1));
                    Assert.That(nnBootNotificationRequest.NetworkPath.Source,                 Is.EqualTo(networkingNode.Id));
                    Assert.That(nnBootNotificationRequest.NetworkPath.Last,                   Is.EqualTo(networkingNode.Id));
                    Assert.That(nnBootNotificationRequest.Reason,                             Is.EqualTo(reason));

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


                    // Networking Node JSON Response IN
                    Assert.That(nnJSONResponseMessagesReceived.     Count,                    Is.EqualTo(1), "The BootNotification JSON request did not leave the networking node!");


                    // Networking Node Response IN
                    Assert.That(nnBootNotificationResponsesReceived.Count,                    Is.EqualTo(1), "The BootNotification response did not reach the networking node!");
                    var nnBootNotificationResponse = nnBootNotificationResponsesReceived.First();
                    Assert.That(nnBootNotificationResponse.Request.RequestId,                 Is.EqualTo(nnBootNotificationRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,                                   Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                              Is.EqualTo(RegistrationStatus.Accepted));

                });

            }

        }

        #endregion



        // CSMS --Networking Node-> Charging Station

        #region CSMS_SendReset_toChargingStation_Test()

        /// <summary>
        /// A test for resetting a charging station.
        /// </summary>
        [Test]
        public async Task CSMS_SendReset_toChargingStation_Test()
        {

            Assert.Multiple(() => {
                Assert.That(chargingStation,         Is.Not.Null);
                Assert.That(networkingNode,          Is.Not.Null);
                Assert.That(nnOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(chargingStation,         Is.Not.Null);
            });

            if (chargingStation        is not null &&
                networkingNode         is not null &&
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
