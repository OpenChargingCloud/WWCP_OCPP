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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.OverlayNetworking.CS
{

    /// <summary>
    /// Unit tests for charging stations sending messages to the CSMS.
    /// </summary>
    [TestFixture]
    public class CS_Messages_Tests : AChargingStationWithNetworkingNodeTests
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


        #region SendBootNotifications_Test()

        /// <summary>
        /// A test for sending boot notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task SendBootNotifications_Test()
        {

            InitNetworkingNode1 = true;

            Assert.Multiple(() => {
                Assert.That(testCSMS01,               Is.Not.Null);
                Assert.That(testBackendWebSockets01,  Is.Not.Null);
                Assert.That(localController1,         Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer01,  Is.Not.Null);
                Assert.That(chargingStation1,         Is.Not.Null);
                Assert.That(chargingStation2,         Is.Not.Null);
                Assert.That(chargingStation3,         Is.Not.Null);
            });

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                localController1        is not null &&
                lcOCPPWebSocketServer01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var csBootNotificationRequests     = new ConcurrentList<BootNotificationRequest>();
                var nnBootNotificationRequestsIN   = new ConcurrentList<BootNotificationRequest>();
                var nnBootNotificationRequestsFWD  = new ConcurrentList<Tuple<BootNotificationRequest, ForwardingDecision<BootNotificationRequest, BootNotificationResponse>>>();
                var nnBootNotificationRequestsOUT  = new ConcurrentList<BootNotificationRequest>();
                var csmsBootNotificationRequests   = new ConcurrentList<BootNotificationRequest>();

                chargingStation1.OCPP.OUT.    OnBootNotificationRequestSent     += (timestamp, sender,             bootNotificationRequest) => {
                    csBootNotificationRequests.   TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                //networkingNode1.IN.     OnBootNotificationRequest += (timestamp, sender, connection, bootNotificationRequest) => {
                //    nnBootNotificationRequestsIN. TryAdd(bootNotificationRequest);
                //    return Task.CompletedTask;
                //};

                localController1.OCPP.FORWARD.OnBootNotificationRequestFiltered += (timestamp, sender, connection, bootNotificationRequest, forwardingDecision) => {
                    nnBootNotificationRequestsFWD.TryAdd(new Tuple<BootNotificationRequest, ForwardingDecision<BootNotificationRequest, BootNotificationResponse>>(bootNotificationRequest, forwardingDecision));
                    return Task.CompletedTask;
                };

                //networkingNode1.OUT.    OnBootNotificationRequest += (timestamp, sender,             bootNotificationRequest) => {
                //    nnBootNotificationRequestsOUT.TryAdd(bootNotificationRequest);
                //    return Task.CompletedTask;
                //};

                testCSMS01.      OCPP.IN.     OnBootNotificationRequestReceived += (timestamp, sender, connection, bootNotificationRequest) => {
                    csmsBootNotificationRequests. TryAdd(bootNotificationRequest);
                    return Task.CompletedTask;
                };

                //chargingStation1.NetworkingMode = NetworkingMode.OverlayNetwork;


                var reason    = BootReason.PowerUp;
                var response  = await chargingStation1.SendBootNotification(
                                    BootReason:   reason,
                                    CustomData:   null
                                );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                  Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                             Is.EqualTo(RegistrationStatus.Accepted));

                    Assert.That(csBootNotificationRequests.    Count,                        Is.EqualTo(1), "The BootNotification did not leave the charging station!");

                    Assert.That(nnBootNotificationRequestsIN.  Count,                        Is.EqualTo(1), "The BootNotification did not reach the INPUT of the networking node!");
                    Assert.That(nnBootNotificationRequestsIN.  First().DestinationId,        Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnBootNotificationRequestsIN.  First().NetworkPath.Length,   Is.EqualTo(1));
                    Assert.That(nnBootNotificationRequestsIN.  First().NetworkPath.Source,   Is.EqualTo(chargingStation1.Id));
                    Assert.That(nnBootNotificationRequestsIN.  First().NetworkPath.Last,     Is.EqualTo(chargingStation1.Id));
                    Assert.That(nnBootNotificationRequestsIN.  First().Reason,               Is.EqualTo(reason));

                    Assert.That(nnBootNotificationRequestsFWD. Count,                        Is.EqualTo(1), "The BootNotification did not reach the FORWARD of the networking node!");

                    Assert.That(nnBootNotificationRequestsOUT. Count,                        Is.EqualTo(1), "The BootNotification did not reach the OUTPUT of the networking node!");

                    Assert.That(csmsBootNotificationRequests.  Count,                        Is.EqualTo(1), "The BootNotification did not reach the CSMS!");
                    Assert.That(csmsBootNotificationRequests.  First().DestinationId,        Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csmsBootNotificationRequests.  First().NetworkPath.Length,   Is.EqualTo(2));
                    Assert.That(csmsBootNotificationRequests.  First().NetworkPath.Source,   Is.EqualTo(chargingStation1.Id));
                    Assert.That(csmsBootNotificationRequests.  First().NetworkPath.Last,     Is.EqualTo(localController1. Id));
                    Assert.That(csmsBootNotificationRequests.  First().Reason,               Is.EqualTo(reason));

                    Assert.That(nnBootNotificationRequestsIN.  First().ChargingStation,      Is.Not.Null);

                });

                var chargingStation = csmsBootNotificationRequests.First().ChargingStation;
                if (chargingStation is not null)
                {

                    Assert.That(chargingStation.Model,             Is.EqualTo(chargingStation1.Model));
                    Assert.That(chargingStation.VendorName,        Is.EqualTo(chargingStation1.VendorName));
                    Assert.That(chargingStation.SerialNumber,      Is.EqualTo(chargingStation1.SerialNumber));
                    Assert.That(chargingStation.FirmwareVersion,   Is.EqualTo(chargingStation1.FirmwareVersion));
                    Assert.That(chargingStation.Modem,             Is.Not.Null);

                    if (chargingStation. Modem is not null &&
                        chargingStation1.Modem is not null)
                    {
                        Assert.That(chargingStation.Modem.ICCID,   Is.EqualTo(chargingStation1.Modem.ICCID));
                        Assert.That(chargingStation.Modem.IMSI,    Is.EqualTo(chargingStation1.Modem.IMSI));
                    }

                }

            }

        }

        #endregion




        #region TransferBinaryData_Test()

        /// <summary>
        /// A test for transfering binary data to the CSMS.
        /// </summary>
        [Test]
        public async Task TransferBinaryData_Test()
        {

            InitNetworkingNode1 = true;

            Assert.Multiple(() => {
                Assert.That(testCSMS01,               Is.Not.Null);
                Assert.That(testBackendWebSockets01,  Is.Not.Null);
                Assert.That(localController1,         Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer01,  Is.Not.Null);
                Assert.That(chargingStation1,         Is.Not.Null);
                Assert.That(chargingStation2,         Is.Not.Null);
                Assert.That(chargingStation3,         Is.Not.Null);
            });

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                localController1        is not null &&
                lcOCPPWebSocketServer01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var csBinaryDataTransferRequestsOUT         = new ConcurrentList<BinaryDataTransferRequest>();
                var nnBinaryDataTransferRequestsIN          = new ConcurrentList<BinaryDataTransferRequest>();
                var nnBinaryDataTransferRequestsFWD         = new ConcurrentList<Tuple<BinaryDataTransferRequest, ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>>>();
                var nnBinaryDataTransferRequestsOUT         = new ConcurrentList<BinaryDataTransferRequest>();
                var csmsIncomingBinaryDataTransferRequests  = new ConcurrentList<BinaryDataTransferRequest>();

                chargingStation1.OCPP.OUT.    OnBinaryDataTransferRequestSent     += (timestamp, sender,             binaryDataTransferRequest) => {
                    csBinaryDataTransferRequestsOUT.TryAdd(binaryDataTransferRequest);
                    return Task.CompletedTask;
                };

                //networkingNode1.IN.     OnIncomingBinaryDataTransferRequest += (timestamp, sender, connection, incomingBinaryDataTransferRequest) => {
                //    nnBinaryDataTransferRequestsIN. TryAdd(incomingBinaryDataTransferRequest);
                //    return Task.CompletedTask;
                //};

                localController1.OCPP.FORWARD.OnBinaryDataTransferRequestFiltered += (timestamp, sender, connection, binaryDataTransferRequest, forwardingDecision) => {
                    nnBinaryDataTransferRequestsFWD.TryAdd(new Tuple<BinaryDataTransferRequest, ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>>(binaryDataTransferRequest, forwardingDecision));
                    return Task.CompletedTask;
                };

                //networkingNode1.OUT.    OnBinaryDataTransferRequest         += (timestamp, sender,             binaryDataTransferRequest) => {
                //    nnBinaryDataTransferRequestsOUT.TryAdd(binaryDataTransferRequest);
                //    return Task.CompletedTask;
                //};

                testCSMS01.      OCPP.IN.     OnBinaryDataTransferRequestReceived += (timestamp, sender, connection, incomingBinaryDataTransferRequest) => {
                    csmsIncomingBinaryDataTransferRequests.TryAdd(incomingBinaryDataTransferRequest);
                    return Task.CompletedTask;
                };

                //chargingStation1.NetworkingMode = NetworkingMode.OverlayNetwork;


                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.GraphDefined_TestMessage;
                var data       = "Hello world!".ToUTF8Bytes();

                var response   = await chargingStation1.TransferBinaryData(
                                           VendorId:    vendorId,
                                           MessageId:   messageId,
                                           Data:        data,
                                           Format:      BinaryFormats.TextIds
                                       );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                          Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                                     Is.EqualTo(BinaryDataTransferStatus.Accepted));
                    Assert.That(response.Data?.ToUTF8String(),                                       Is.EqualTo(data.Reverse().ToUTF8String()));

                    Assert.That(csBinaryDataTransferRequestsOUT.       Count,                        Is.EqualTo(1), "The BinaryDataTransfer did not leave the charging station!");

                    Assert.That(nnBinaryDataTransferRequestsIN.        Count,                        Is.EqualTo(1), "The BinaryDataTransfer did not reach the INPUT of the networking node!");
                    Assert.That(nnBinaryDataTransferRequestsIN.        First().DestinationId,    Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(nnBinaryDataTransferRequestsIN.        First().NetworkPath.Length,   Is.EqualTo(1));
                    Assert.That(nnBinaryDataTransferRequestsIN.        First().NetworkPath.Source,   Is.EqualTo(chargingStation1.Id));
                    Assert.That(nnBinaryDataTransferRequestsIN.        First().NetworkPath.Last,     Is.EqualTo(chargingStation1.Id));
                    Assert.That(nnBinaryDataTransferRequestsIN.        First().VendorId,             Is.EqualTo(vendorId));
                    Assert.That(nnBinaryDataTransferRequestsIN.        First().MessageId,            Is.EqualTo(messageId));
                    Assert.That(nnBinaryDataTransferRequestsIN.        First().Data,                 Is.EqualTo(data));

                    Assert.That(nnBinaryDataTransferRequestsFWD.       Count,                        Is.EqualTo(1), "The BinaryDataTransfer did not reach the FORWARD of the networking node!");

                    Assert.That(nnBinaryDataTransferRequestsOUT.       Count,                        Is.EqualTo(1), "The BinaryDataTransfer did not reach the OUTPUT of the networking node!");

                    Assert.That(csmsIncomingBinaryDataTransferRequests.Count,                        Is.EqualTo(1), "The BinaryDataTransfer did not reach the CSMS!");
                    Assert.That(csmsIncomingBinaryDataTransferRequests.First().DestinationId,    Is.EqualTo(NetworkingNode_Id.CSMS));
                    Assert.That(csmsIncomingBinaryDataTransferRequests.First().NetworkPath.Length,   Is.EqualTo(2));
                    Assert.That(csmsIncomingBinaryDataTransferRequests.First().NetworkPath.Source,   Is.EqualTo(chargingStation1.Id));
                    Assert.That(csmsIncomingBinaryDataTransferRequests.First().NetworkPath.Last,     Is.EqualTo(localController1. Id));
                    Assert.That(csmsIncomingBinaryDataTransferRequests.First().VendorId,             Is.EqualTo(vendorId));
                    Assert.That(csmsIncomingBinaryDataTransferRequests.First().MessageId,            Is.EqualTo(messageId));
                    Assert.That(csmsIncomingBinaryDataTransferRequests.First().Data,                 Is.EqualTo(data));

                });

            }

        }

        #endregion



    }

}
