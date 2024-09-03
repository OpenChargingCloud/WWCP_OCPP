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
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.OverlayNetworking.NN
{

    /// <summary>
    /// Unit tests for networking nodes sending messages to the CSMS.
    /// </summary>
    [TestFixture]
    public class NN_Messages_Tests : AChargingStationWithNetworkingNodeTests
    {

        #region Init_Test()

        ///// <summary>
        ///// A test for creating charging stations.
        ///// </summary>
        //[Test]
        //public void ChargingStation_Init_Test()
        //{

        //    ClassicAssert.IsNotNull(testCSMS01);
        //    ClassicAssert.IsNotNull(testBackendWebSockets01);
        //    ClassicAssert.IsNotNull(chargingStation1);
        //    ClassicAssert.IsNotNull(chargingStation2);
        //    ClassicAssert.IsNotNull(chargingStation3);

        //    if (testCSMS01              is not null &&
        //        testBackendWebSockets01 is not null &&
        //        chargingStation1        is not null &&
        //        chargingStation2        is not null &&
        //        chargingStation3        is not null)
        //    {

        //        ClassicAssert.AreEqual("GraphDefined OEM #1",  chargingStation1.VendorName);
        //        ClassicAssert.AreEqual("GraphDefined OEM #2",  chargingStation2.VendorName);
        //        ClassicAssert.AreEqual("GraphDefined OEM #3",  chargingStation3.VendorName);

        //    }

        //}

        #endregion


        // to CSMS

        #region SendBootNotifications_Test()

        /// <summary>
        /// A test for sending boot notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task SendBootNotifications_Test()
        {

            InitNetworkingNode1 = true;

            Assert.Multiple(() => {
                Assert.That(localController1,           Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer01,   Is.Not.Null);
                Assert.That(testCSMS01,                Is.Not.Null);
            });

            if (localController1          is not null &&
                lcOCPPWebSocketServer01  is not null &&
                testCSMS01               is not null)
            {

                var nnBootNotificationRequestsSent       = new ConcurrentList<BootNotificationRequest>();
                var nnJSONMessageRequestsSent            = new ConcurrentList<OCPP_JSONRequestMessage>();
                var csmsBootNotificationRequests         = new ConcurrentList<BootNotificationRequest>();
                var nnJSONResponseMessagesReceived       = new ConcurrentList<OCPP_JSONResponseMessage>();
                var nnBootNotificationResponsesReceived  = new ConcurrentList<BootNotificationResponse>();

                localController1.OCPP.OUT.OnBootNotificationRequestSent      += (timestamp, sender, connection, bootNotificationRequest, sentMessageResult, ct) => {
                    nnBootNotificationRequestsSent.TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                localController1.OCPP.OUT.OnJSONRequestMessageSent           += (timestamp, sender, connection, jsonRequestMessage, sentMessageResult, ct) => {
                    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                testCSMS01.OCPP.IN.       OnBootNotificationRequestReceived  += (timestamp, sender, connection, bootNotificationRequest, ct) => {
                    csmsBootNotificationRequests.  TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                localController1.OCPP.IN. OnJSONResponseMessageReceived      += (timestamp, sender, connection, jsonResponseMessage, ct) => {
                    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                //networkingNode1.ocppOUT.OnBootNotificationResponse    += (timestamp, sender,             bootNotificationRequest, bootNotificationResponse, runtime) => {
                //    nnBootNotificationResponses. TryAdd(bootNotificationResponse);
                //    return Task.CompletedTask;
                //};

                localController1.OCPP.IN. OnBootNotificationResponseReceived += (timestamp, sender, connection, bootNotificationRequest, bootNotificationResponse, runtime, ct) => {
                    nnBootNotificationResponsesReceived.   TryAdd(bootNotificationResponse);
                    return Task.CompletedTask;
                };


                var reason    = BootReason.PowerUp;
                var response  = await localController1.SendBootNotification(
                                          BootReason:  reason
                                      );


                Assert.Multiple(() => {

                    // Networking Node Request OUT
                    Assert.That(nnBootNotificationRequestsSent.     Count,                    Is.EqualTo(1), "The BootNotification request did not leave the networking node!");
                    var nnBootNotificationRequest = nnBootNotificationRequestsSent.First();
                    Assert.That(nnBootNotificationRequest.DestinationId,                  Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnBootNotificationRequest.NetworkPath.Length,                 Is.EqualTo(1));
                    Assert.That(nnBootNotificationRequest.NetworkPath.Source,                 Is.EqualTo(localController1.Id));
                    Assert.That(nnBootNotificationRequest.NetworkPath.Last,                   Is.EqualTo(localController1.Id));
                    Assert.That(nnBootNotificationRequest.Reason,                             Is.EqualTo(reason));

                    // Networking Node JSON Request OUT
                    Assert.That(nnJSONMessageRequestsSent.          Count,                    Is.EqualTo(1), "The BootNotification JSON request did not leave the networking node!");

                    // CSMS Request IN
                    Assert.That(csmsBootNotificationRequests.       Count,                    Is.EqualTo(1), "The BootNotification request did not reach the CSMS!");
                    var csmsBootNotificationRequest = csmsBootNotificationRequests.First();
                    Assert.That(csmsBootNotificationRequest.DestinationId,                Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csmsBootNotificationRequest.NetworkPath.Length,               Is.EqualTo(1));
                    Assert.That(csmsBootNotificationRequest.NetworkPath.Source,               Is.EqualTo(localController1.Id));
                    Assert.That(csmsBootNotificationRequest.NetworkPath.Last,                 Is.EqualTo(localController1.Id));
                    Assert.That(csmsBootNotificationRequest.Reason,                           Is.EqualTo(reason));

                    Assert.That(csmsBootNotificationRequest.ChargingStation,                  Is.Not.Null);
                    var chargingStation = csmsBootNotificationRequests.First().ChargingStation;
                    if (chargingStation is not null)
                    {

                        Assert.That(chargingStation.Model,             Is.EqualTo(localController1.Model));
                        Assert.That(chargingStation.VendorName,        Is.EqualTo(localController1.VendorName));
                        Assert.That(chargingStation.SerialNumber,      Is.EqualTo(localController1.SerialNumber));
                        Assert.That(chargingStation.FirmwareVersion,   Is.EqualTo(localController1.SoftwareVersion));
                        Assert.That(chargingStation.Modem,             Is.Not.Null);

                        if (chargingStation.Modem is not null &&
                            localController1.Modem is not null)
                        {
                            Assert.That(chargingStation.Modem.ICCID,   Is.EqualTo(localController1.Modem.ICCID));
                            Assert.That(chargingStation.Modem.IMSI,    Is.EqualTo(localController1.Modem.IMSI));
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



        // to Charging Station

        #region SendChargingStationReset_Test()

        /// <summary>
        /// A test for resetting a charging station.
        /// </summary>
        [Test]
        public async Task SendChargingStationReset_Test()
        {

            InitNetworkingNode1 = true;

            Assert.Multiple(() => {
                Assert.That(localController1,           Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer01,   Is.Not.Null);
                Assert.That(chargingStation1,          Is.Not.Null);
            });

            if (localController1          is not null &&
                lcOCPPWebSocketServer01  is not null &&
                chargingStation1         is not null)
            {

                var nnResetRequestsSent             = new ConcurrentList<ResetRequest>();
                var nnJSONMessageRequestsSent       = new ConcurrentList<OCPP_JSONRequestMessage>();
                var csResetRequests                 = new ConcurrentList<ResetRequest>();
                var nnJSONResponseMessagesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
                var nnResetResponsesReceived        = new ConcurrentList<ResetResponse>();

                localController1.OCPP.OUT.OnResetRequestSent             += (timestamp, sender, connection, resetRequest, sentMessageResult, ct) => {
                    nnResetRequestsSent.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                localController1.OCPP.OUT.OnJSONRequestMessageSent       += (timestamp, sender, connection, jsonRequestMessage, sentMessageResult, ct) => {
                    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                chargingStation1.OCPP.IN. OnResetRequestReceived         += (timestamp, sender, connection, resetRequest, ct) => {
                    csResetRequests.               TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                localController1.OCPP.IN. OnJSONResponseMessageReceived  += (timestamp, sender, connection, jsonResponseMessage, ct) => {
                    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                localController1.OCPP.IN. OnResetResponseReceived        += (timestamp, sender, connection, resetRequest, resetResponse, runtime, ct) => {
                    nnResetResponsesReceived.      TryAdd(resetResponse);
                    return Task.CompletedTask;
                };


                var resetType  = ResetType.Immediate;
                var response   = await localController1.Reset(
                                           Destination:    SourceRouting.To(chargingStation1.Id),
                                           ResetType:          resetType
                                       );


                Assert.Multiple(() => {

                    // Networking Node Request OUT
                    Assert.That(nnResetRequestsSent.           Count,   Is.EqualTo(1), "The Reset request did not leave the networking node!");
                    var nnResetRequest = nnResetRequestsSent.First();
                    Assert.That(nnResetRequest.DestinationId,       Is.EqualTo(chargingStation1.Id));
                    Assert.That(nnResetRequest.NetworkPath.Length,      Is.EqualTo(1));
                    Assert.That(nnResetRequest.NetworkPath.Source,      Is.EqualTo(localController1.Id));
                    Assert.That(nnResetRequest.NetworkPath.Last,        Is.EqualTo(localController1.Id));
                    Assert.That(nnResetRequest.ResetType,               Is.EqualTo(resetType));

                    // Networking Node JSON Request OUT
                    Assert.That(nnJSONMessageRequestsSent.     Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    // Charging Station Request IN
                    Assert.That(csResetRequests.               Count,   Is.EqualTo(1), "The Reset request did not reach the charging station!");
                    var csResetRequest = csResetRequests.First();
                    //Assert.That(csResetRequest.DestinationId,       Is.EqualTo(chargingStation1.Id));   // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.NetworkPath.Length,      Is.EqualTo(1));                     // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode1.Id));    // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode1.Id));    // Because of "standard" networking mode!
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



        #region NotifyNetworkTopology_Test1()

        /// <summary>
        /// A test for sending NotifyNetworkTopology requests to the CSMS.
        /// </summary>
        [Test]
        public async Task NotifyNetworkTopology_Test1()
        {

            InitNetworkingNode1 = true;

            Assert.Multiple(() => {
                Assert.That(testCSMS01,                       Is.Not.Null);
                Assert.That(testBackendWebSockets01,          Is.Not.Null);
                Assert.That(localController1,                  Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer01,   Is.Not.Null);
                Assert.That(chargingStation1,                 Is.Not.Null);
                Assert.That(chargingStation2,                 Is.Not.Null);
                Assert.That(chargingStation3,                 Is.Not.Null);
            });

            if (testCSMS01               is not null &&
                testBackendWebSockets01  is not null &&
                localController1         is not null &&
                lcOCPPWebSocketServer01  is not null &&
                chargingStation1         is not null &&
                chargingStation2         is not null &&
                chargingStation3         is not null)
            {

                var csmsNotifyNetworkTopologyMessages  = new ConcurrentList<NotifyNetworkTopologyMessage>();

                testCSMS01.OCPP.IN.OnNotifyNetworkTopologyMessageReceived += (timestamp, sender, connection, notifyNetworkTopologyRequest, ct) => {
                    csmsNotifyNetworkTopologyMessages.TryAdd(notifyNetworkTopologyRequest);
                    return Task.CompletedTask;
                };


                var reason    = BootReason.PowerUp;
                var response  = await localController1.OCPP.OUT.NotifyNetworkTopology(
                                    new NotifyNetworkTopologyMessage(
                                        SourceRouting.CSMS,
                                        new NetworkTopologyInformation(
                                            RoutingNode:   localController1.Id,
                                            Routes:        [
                                                               new NetworkRoutingInformation(
                                                                   DestinationId:   localController1.Id,
                                                                   Priority:        23,
                                                                   Uplink:          new VirtualNetworkLinkInformation(
                                                                                        Distance:     2,
                                                                                        Capacity:     BitsPerSecond.     ParseBPS        ( 5000M,  10M),
                                                                                        Latency:      TimeSpanExtensions.FromMilliseconds(    40,    3),
                                                                                        PacketLoss:   PercentageDouble.  Parse           (  0.05, 0.05)
                                                                                    ),
                                                                   Downlink:        new VirtualNetworkLinkInformation(
                                                                                        Distance:     3,
                                                                                        Capacity:     BitsPerSecond.     ParseBPS        (15000M,  30M),
                                                                                        Latency:      TimeSpanExtensions.FromMilliseconds(    20,   23),
                                                                                        PacketLoss:   PercentageDouble.  Parse           (  0.52, 0.12)
                                                                                    )
                                                               )
                                                           ],
                                            NotBefore:     Timestamp.Now - TimeSpan.FromMinutes(5),
                                            NotAfter:      Timestamp.Now + TimeSpan.FromHours  (6),
                                            Priority:      5,
                                            CustomData:    null
                                        )
                                    )
                                );


                Assert.Multiple(() => {

                    //Assert.That(response.Result.ResultCode,                                    Is.EqualTo(ResultCode.OK));
                    //Assert.That(response.Status,                                               Is.EqualTo(NetworkTopologyStatus.Accepted));

                    Assert.That(csmsNotifyNetworkTopologyMessages.Count,                       Is.EqualTo(1), "The Network Topology Notification did not reach the CSMS!");
                    var csmsNotifyNetworkTopologyMessage = csmsNotifyNetworkTopologyMessages.First();

                    Assert.That(csmsNotifyNetworkTopologyMessage.DestinationId,                Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csmsNotifyNetworkTopologyMessage.NetworkPath.Length,           Is.EqualTo(1));
                    Assert.That(csmsNotifyNetworkTopologyMessage.NetworkPath.Source,           Is.EqualTo(localController1.Id));
                    Assert.That(csmsNotifyNetworkTopologyMessage.NetworkPath.Last,             Is.EqualTo(localController1.Id));

                    Assert.That(csmsNotifyNetworkTopologyMessage.NetworkTopologyInformation,   Is.Not.Null);

                });

                var networkTopologyInformation = csmsNotifyNetworkTopologyMessages.First().NetworkTopologyInformation;
                if (networkTopologyInformation is not null)
                {

                    Assert.That(networkTopologyInformation.RoutingNode,                  Is.EqualTo(localController1.Id));
                    Assert.That(networkTopologyInformation.Priority,                     Is.EqualTo(5));
                    Assert.That(networkTopologyInformation.Routes,                       Is.Not.Null);

                    var routes = networkTopologyInformation.Routes;
                    if (routes is not null)
                    {

                        Assert.That(routes.Values.First().DestinationId,                 Is.EqualTo(localController1.Id));
                        Assert.That(routes.Values.First().Priority,                      Is.EqualTo(23));

                        Assert.That(routes.Values.First().Uplink,                        Is.Not.Null);
                        Assert.That(routes.Values.First().Uplink?.Capacity,              Is.EqualTo(5000));
                        Assert.That(routes.Values.First().Uplink?.Latency,               Is.EqualTo(TimeSpan.FromMilliseconds(10)));
                        Assert.That(routes.Values.First().Uplink?.PacketLoss?.Value,     Is.EqualTo(0.05));

                        Assert.That(routes.Values.First().Downlink,                      Is.Not.Null);
                        Assert.That(routes.Values.First().Downlink?.Capacity,            Is.EqualTo(15000));
                        Assert.That(routes.Values.First().Downlink?.Latency,             Is.EqualTo(TimeSpan.FromMilliseconds(23)));
                        Assert.That(routes.Values.First().Downlink?.PacketLoss?.Value,   Is.EqualTo(0.42));

                    }

                }

            }

        }

        #endregion

        #region NotifyNetworkTopology_Test2()

        /// <summary>
        /// A test for sending NotifyNetworkTopology requests to the CSMS.
        /// </summary>
        [Test]
        public async Task NotifyNetworkTopology_Test2()
        {

            InitNetworkingNode1 = true;

            Assert.Multiple(() => {
                Assert.That(testCSMS01,                       Is.Not.Null);
                Assert.That(testBackendWebSockets01,          Is.Not.Null);
                Assert.That(localController1,                  Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer01,   Is.Not.Null);
                Assert.That(chargingStation1,                 Is.Not.Null);
                Assert.That(chargingStation2,                 Is.Not.Null);
                Assert.That(chargingStation3,                 Is.Not.Null);
            });

            if (testCSMS01                     is not null &&
                testBackendWebSockets01        is not null &&
                localController1               is not null &&
                lcOCPPWebSocketServer01 is not null &&
                chargingStation1               is not null &&
                chargingStation2               is not null &&
                chargingStation3               is not null)
            {

                var csmsNotifyNetworkTopologyMessages  = new ConcurrentList<NotifyNetworkTopologyMessage>();

                testCSMS01.OCPP.IN.OnNotifyNetworkTopologyMessageReceived += (timestamp, sender, connection, notifyNetworkTopologyRequest, ct) => {
                    csmsNotifyNetworkTopologyMessages.TryAdd(notifyNetworkTopologyRequest);
                    return Task.CompletedTask;
                };


                var reason    = BootReason.PowerUp;
                var response  = await localController1.NotifyNetworkTopology(
                                    Destination:                  SourceRouting.CSMS,
                                    NetworkTopologyInformation:   new NetworkTopologyInformation(
                                                                      RoutingNode:   localController1.Id,
                                                                      Routes:        [
                                                                                         new NetworkRoutingInformation(
                                                                                             DestinationId:   localController1.Id,
                                                                                             Priority:        23,
                                                                                             Uplink:          new VirtualNetworkLinkInformation(
                                                                                                                  Distance:     2,
                                                                                                                  Capacity:     BitsPerSecond.     ParseBPS        ( 5000M,  10M),
                                                                                                                  Latency:      TimeSpanExtensions.FromMilliseconds(    40,    3),
                                                                                                                  PacketLoss:   PercentageDouble.  Parse           (  0.05, 0.05)
                                                                                                              ),
                                                                                             Downlink:        new VirtualNetworkLinkInformation(
                                                                                                                  Distance:     3,
                                                                                                                  Capacity:     BitsPerSecond.     ParseBPS        (15000M,  30M),
                                                                                                                  Latency:      TimeSpanExtensions.FromMilliseconds(    20,   23),
                                                                                                                  PacketLoss:   PercentageDouble.  Parse           (  0.52, 0.12)
                                                                                                              )
                                                                                         )
                                                                                     ],
                                                                      NotBefore:     Timestamp.Now - TimeSpan.FromMinutes(5),
                                                                      NotAfter:      Timestamp.Now + TimeSpan.FromHours  (6),
                                                                      Priority:      5,
                                                                      CustomData:    null
                                                                  )
                                );


                Assert.Multiple(() => {

                    //Assert.That(response.Result.ResultCode,                                    Is.EqualTo(ResultCode.OK));
                    //Assert.That(response.Status,                                               Is.EqualTo(NetworkTopologyStatus.Accepted));

                    Assert.That(csmsNotifyNetworkTopologyMessages.Count,                       Is.EqualTo(1), "The Network Topology Notification did not reach the CSMS!");
                    var csmsNotifyNetworkTopologyMessage = csmsNotifyNetworkTopologyMessages.First();

                    Assert.That(csmsNotifyNetworkTopologyMessage.DestinationId,                Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csmsNotifyNetworkTopologyMessage.NetworkPath.Length,           Is.EqualTo(1));
                    Assert.That(csmsNotifyNetworkTopologyMessage.NetworkPath.Source,           Is.EqualTo(localController1.Id));
                    Assert.That(csmsNotifyNetworkTopologyMessage.NetworkPath.Last,             Is.EqualTo(localController1.Id));

                    Assert.That(csmsNotifyNetworkTopologyMessage.NetworkTopologyInformation,   Is.Not.Null);

                });

                var networkTopologyInformation = csmsNotifyNetworkTopologyMessages.First().NetworkTopologyInformation;
                if (networkTopologyInformation is not null)
                {

                    Assert.That(networkTopologyInformation.RoutingNode,                        Is.EqualTo(localController1.Id));
                    Assert.That(networkTopologyInformation.Priority,                           Is.EqualTo(5));
                    Assert.That(networkTopologyInformation.Routes,                             Is.Not.Null);

                    var routes = networkTopologyInformation.Routes;
                    if (routes is not null)
                    {

                        Assert.That(routes.Values.First().DestinationId,                       Is.EqualTo(localController1.Id));
                        Assert.That(routes.Values.First().Priority,                            Is.EqualTo(23));

                        Assert.That(routes.Values.First().Uplink,                              Is.Not.Null);
                        Assert.That(routes.Values.First().Uplink?.Capacity,                    Is.EqualTo(5000));
                        Assert.That(routes.Values.First().Uplink?.Latency,                     Is.EqualTo(TimeSpan.FromMilliseconds(10)));
                        Assert.That(routes.Values.First().Uplink?.PacketLoss?.Value,           Is.EqualTo(0.05));

                        Assert.That(routes.Values.First().Downlink,                            Is.Not.Null);
                        Assert.That(routes.Values.First().Downlink?.Capacity,                  Is.EqualTo(15000));
                        Assert.That(routes.Values.First().Downlink?.Latency,                   Is.EqualTo(TimeSpan.FromMilliseconds(23)));
                        Assert.That(routes.Values.First().Downlink?.PacketLoss?.Value,         Is.EqualTo(0.42));

                    }

                }

            }

        }

        #endregion


    }

}
