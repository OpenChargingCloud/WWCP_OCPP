﻿/*
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
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.NetworkingNode.CSMS
{

    /// <summary>
    /// Unit tests for charging stations sending messages to the CSMS.
    /// </summary>
    [TestFixture]
    public class CSMS_Messages_Tests : AChargingStationWithLocalControllersTests
    {

        #region Reset_ChargingStation_Test()

        /// <summary>
        /// A test for sending a reset message to a charging station.
        /// </summary>
        [Test]
        public async Task Reset_ChargingStation_Test()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,                       Is.Not.Null);
                Assert.That(testBackendWebSockets1,          Is.Not.Null);
                Assert.That(localController1,                  Is.Not.Null);
//                Assert.That(testNetworkingNodeWebSockets01,   Is.Not.Null);
                Assert.That(chargingStation1,                 Is.Not.Null);
                Assert.That(chargingStation2,                 Is.Not.Null);
                Assert.That(chargingStation3,                 Is.Not.Null);
            });

            if (testCSMS1              is not null &&
                testBackendWebSockets1 is not null &&
                localController1         is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var csmsResetRequests   = new ConcurrentList<ResetRequest>();
                var nnResetRequestsIN   = new ConcurrentList<ResetRequest>();
                var nnResetRequestsFWD  = new ConcurrentList<Tuple<ResetRequest, RequestForwardingDecision<ResetRequest, ResetResponse>>>();
                var nnResetRequestsOUT  = new ConcurrentList<ResetRequest>();
                var csResetRequests     = new ConcurrentList<ResetRequest>();

                testCSMS1.      OCPP.OUT.    OnResetRequestSent     += (timestamp, sender, connection, resetRequest, sentMessageResult, ct) => {
                    csmsResetRequests.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                //networkingNode1.IN.     OnResetRequest += (timestamp, sender, connection, resetRequest) => {
                //    nnResetRequestsIN.TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                localController1.OCPP.FORWARD.OnResetRequestFiltered  += (timestamp, sender, connection, resetRequest, forwardingDecision, ct) => {
                    nnResetRequestsFWD.TryAdd(new Tuple<ResetRequest, RequestForwardingDecision<ResetRequest, ResetResponse>>(resetRequest, forwardingDecision));
                    return Task.CompletedTask;
                };

                //networkingNode1.OUT.    OnResetRequest += (timestamp, sender,             resetRequest) => {
                //    nnResetRequestsOUT.TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                chargingStation1.OCPP.IN.     OnResetRequestReceived += (timestamp, sender, connection, resetRequest, ct) => {
                    csResetRequests.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                // Charging Station 1 is reachable via the networking node 1!
                // Good old "static routing" ;)
                testCSMS1.Routing.AddOrUpdateStaticRouting(
                    chargingStation1.Id,
                    localController1.Id
                );


                var resetType  = ResetType.Immediate;
                var response   = await testCSMS1.Reset(
                                     Destination:    SourceRouting.To( chargingStation1.Id),
                                     ResetType:           resetType,
                                     CustomData:          null
                                 );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                      Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                 Is.EqualTo(ResetStatus.Accepted));

                    Assert.That(csmsResetRequests. Count,                        Is.EqualTo(1), "The ResetRequest did not leave the CSMS!");

                    Assert.That(nnResetRequestsIN. Count,                        Is.EqualTo(1), "The ResetRequest did not reach the INPUT of the networking node!");
                    Assert.That(nnResetRequestsIN. First().DestinationId,        Is.EqualTo(chargingStation1.Id));
                    Assert.That(nnResetRequestsIN. First().NetworkPath.Length,   Is.EqualTo(1));
                    Assert.That(nnResetRequestsIN. First().NetworkPath.Source,   Is.EqualTo(testCSMS1.      Id));
                    Assert.That(nnResetRequestsIN. First().NetworkPath.Last,     Is.EqualTo(testCSMS1.      Id));

                    Assert.That(nnResetRequestsFWD.Count,                        Is.EqualTo(1), "The ResetRequest did not reach the FORWARD of the networking node!");

                    Assert.That(nnResetRequestsOUT.Count,                        Is.EqualTo(1), "The ResetRequest did not reach the OUTPUT of the networking node!");
                    Assert.That(nnResetRequestsOUT.First().DestinationId,        Is.EqualTo(chargingStation1.Id));
                    Assert.That(nnResetRequestsOUT.First().NetworkPath.Length,   Is.EqualTo(1));
                    Assert.That(nnResetRequestsOUT.First().NetworkPath.Source,   Is.EqualTo(testCSMS1.      Id));
                    Assert.That(nnResetRequestsOUT.First().NetworkPath.Last,     Is.EqualTo(testCSMS1.      Id));

                    Assert.That(csResetRequests.   Count,                        Is.EqualTo(1), "The ResetRequest did not reach the charging station!");
                    // Because of 'standard' networking mode towards the charging station!
                    Assert.That(csResetRequests.   First().DestinationId,        Is.EqualTo(NetworkingNode_Id.Zero));
                    Assert.That(csResetRequests.   First().NetworkPath.Length,   Is.EqualTo(0));

                });

            }

        }

        #endregion


        #region TransferData_Test()

        /// <summary>
        /// A test for transfering vendor-specific data to a charging station.
        /// </summary>
        [Test]
        public async Task TransferData_Test()
        {

            InitLocalController1 = true;

            Assert.Multiple(() => {
                Assert.That(testCSMS1,               Is.Not.Null);
                Assert.That(testBackendWebSockets1,  Is.Not.Null);
                Assert.That(localController1,         Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer1,  Is.Not.Null);
                Assert.That(chargingStation1,         Is.Not.Null);
                Assert.That(chargingStation2,         Is.Not.Null);
                Assert.That(chargingStation3,         Is.Not.Null);
            });

            if (testCSMS1              is not null &&
                testBackendWebSockets1 is not null &&
                localController1        is not null &&
                lcOCPPWebSocketServer1 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var csmsDataTransferRequestsOUT     = new ConcurrentList<DataTransferRequest>();
                var nnDataTransferRequestsIN        = new ConcurrentList<DataTransferRequest>();
                var nnDataTransferRequestsFWD       = new ConcurrentList<Tuple<DataTransferRequest, RequestForwardingDecision<DataTransferRequest, DataTransferResponse>>>();
                var nnDataTransferRequestsOUT       = new ConcurrentList<DataTransferRequest>();
                var csIncomingDataTransferRequests  = new ConcurrentList<DataTransferRequest>();

                testCSMS1.OCPP.OUT.  OnDataTransferRequestSent             += (timestamp, sender, connection, binaryDataTransferRequest, sentMessageResult, ct) => {
                    csmsDataTransferRequestsOUT.   TryAdd(binaryDataTransferRequest);
                    return Task.CompletedTask;
                };

                //networkingNode1.IN.     OnIncomingDataTransferRequest += (timestamp, sender, connection, incomingDataTransferRequest) => {
                //    nnDataTransferRequestsIN.      TryAdd(incomingDataTransferRequest);
                //    return Task.CompletedTask;
                //};

                localController1.OCPP.FORWARD.OnDataTransferRequestFiltered += (timestamp, sender, connection, binaryDataTransferRequest, forwardingDecision, ct) => {
                    nnDataTransferRequestsFWD.TryAdd(new Tuple<DataTransferRequest, RequestForwardingDecision<DataTransferRequest, DataTransferResponse>>(binaryDataTransferRequest, forwardingDecision));
                    return Task.CompletedTask;
                };

                //networkingNode1.OUT.    OnDataTransferRequest         += (timestamp, sender,             binaryDataTransferRequest) => {
                //    nnDataTransferRequestsOUT.     TryAdd(binaryDataTransferRequest);
                //    return Task.CompletedTask;
                //};

                chargingStation1.OCPP.IN.     OnDataTransferRequestReceived += (timestamp, sender, connection, incomingDataTransferRequest, ct) => {
                    csIncomingDataTransferRequests.TryAdd(incomingDataTransferRequest);
                    return Task.CompletedTask;
                };

                // Charging Station 1 is reachable via the networking node 1!
                // Good old "static routing" ;)
                testCSMS1.Routing.AddOrUpdateStaticRouting(
                    chargingStation1.Id,
                    localController1.Id
                );

                //chargingStation1.NetworkingMode = OCPP.WebSockets.NetworkingMode.NetworkingExtensions;


                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.GraphDefined_TestMessage;
                var data       = "Hello world!";


                var response   = await testCSMS1.TransferData(
                                           Destination:    SourceRouting.To( chargingStation1.Id),
                                           VendorId:            vendorId,
                                           MessageId:           messageId,
                                           Data:                data
                                       );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                        Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                                   Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data?.ToString(),                                         Is.EqualTo(data.Reverse().ToString()));

                    Assert.That(csmsDataTransferRequestsOUT.   Count,                        Is.EqualTo(1), "The DataTransfer did not leave the CSMS!");

                    Assert.That(nnDataTransferRequestsIN.      Count,                        Is.EqualTo(1), "The DataTransfer did not reach the networking node!");
                    Assert.That(nnDataTransferRequestsIN.      First().DestinationId,        Is.EqualTo(chargingStation1.Id));
                    Assert.That(nnDataTransferRequestsIN.      First().NetworkPath.Length,   Is.EqualTo(1));
                    Assert.That(nnDataTransferRequestsIN.      First().NetworkPath.Source,   Is.EqualTo(testCSMS1.Id));
                    Assert.That(nnDataTransferRequestsIN.      First().NetworkPath.Last,     Is.EqualTo(testCSMS1.Id));
                    Assert.That(nnDataTransferRequestsIN.      First().VendorId,             Is.EqualTo(vendorId));
                    Assert.That(nnDataTransferRequestsIN.      First().MessageId,            Is.EqualTo(messageId));
                    Assert.That(nnDataTransferRequestsIN.      First().Data,                 Is.EqualTo(data));

                    Assert.That(nnDataTransferRequestsFWD.     Count,                        Is.EqualTo(1), "The DataTransfer did not reach the FORWARD of the networking node!");

                    Assert.That(nnDataTransferRequestsOUT.     Count,                        Is.EqualTo(1), "The DataTransfer did not reach the OUTPUT of the networking node!");

                    Assert.That(csIncomingDataTransferRequests.Count,                        Is.EqualTo(1), "The DataTransfer did not reach the charging station!");
                    //Assert.That(csIncomingDataTransferRequests.First().DestinationId,    Is.EqualTo(NetworkingNode_Id.CSMS));
                    //Assert.That(csIncomingDataTransferRequests.First().NetworkPath.Length,   Is.EqualTo(2));
                    //Assert.That(csIncomingDataTransferRequests.First().NetworkPath.Source,   Is.EqualTo(chargingStation1.Id));
                    //Assert.That(csIncomingDataTransferRequests.First().NetworkPath.Last,     Is.EqualTo(networkingNode1. Id));
                    Assert.That(csIncomingDataTransferRequests.First().VendorId,             Is.EqualTo(vendorId));
                    Assert.That(csIncomingDataTransferRequests.First().MessageId,            Is.EqualTo(messageId));
                    Assert.That(csIncomingDataTransferRequests.First().Data,                 Is.EqualTo(data));

                });

            }

        }

        #endregion

        #region TransferBinaryData_Test()

        /// <summary>
        /// A test for transfering vendor-specific binary data to a charging station.
        /// </summary>
        [Test]
        public async Task TransferBinaryData_Test()
        {

            InitLocalController1 = true;

            Assert.Multiple(() => {
                Assert.That(testCSMS1,               Is.Not.Null);
                Assert.That(testBackendWebSockets1,  Is.Not.Null);
                Assert.That(localController1,         Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer1,  Is.Not.Null);
                Assert.That(chargingStation1,         Is.Not.Null);
                Assert.That(chargingStation2,         Is.Not.Null);
                Assert.That(chargingStation3,         Is.Not.Null);
            });

            if (testCSMS1              is not null &&
                testBackendWebSockets1 is not null &&
                localController1        is not null &&
                lcOCPPWebSocketServer1 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var csmsBinaryDataTransferRequestsOUT     = new ConcurrentList<BinaryDataTransferRequest>();
                var nnBinaryDataTransferRequestsIN        = new ConcurrentList<BinaryDataTransferRequest>();
                var nnBinaryDataTransferRequestsFWD       = new ConcurrentList<Tuple<BinaryDataTransferRequest, RequestForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>>>();
                var nnBinaryDataTransferRequestsOUT       = new ConcurrentList<BinaryDataTransferRequest>();
                var csIncomingBinaryDataTransferRequests  = new ConcurrentList<BinaryDataTransferRequest>();

                testCSMS1.      OCPP.OUT.    OnBinaryDataTransferRequestSent     += (timestamp, sender, connection, binaryDataTransferRequest, sentMessageResult, ct) => {
                    csmsBinaryDataTransferRequestsOUT.   TryAdd(binaryDataTransferRequest);
                    return Task.CompletedTask;
                };

                //networkingNode1.IN.     OnIncomingBinaryDataTransferRequest += (timestamp, sender, connection, incomingBinaryDataTransferRequest) => {
                //    nnBinaryDataTransferRequestsIN.      TryAdd(incomingBinaryDataTransferRequest);
                //    return Task.CompletedTask;
                //};

                localController1.OCPP.FORWARD.OnBinaryDataTransferRequestFiltered += (timestamp, sender, connection, binaryDataTransferRequest, forwardingDecision, ct) => {
                    nnBinaryDataTransferRequestsFWD.TryAdd(new Tuple<BinaryDataTransferRequest, RequestForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>>(binaryDataTransferRequest, forwardingDecision));
                    return Task.CompletedTask;
                };

                //networkingNode1.OUT.    OnBinaryDataTransferRequest         += (timestamp, sender,             binaryDataTransferRequest) => {
                //    nnBinaryDataTransferRequestsOUT.     TryAdd(binaryDataTransferRequest);
                //    return Task.CompletedTask;
                //};

                chargingStation1.OCPP.IN.     OnBinaryDataTransferRequestReceived += (timestamp, sender, connection, incomingBinaryDataTransferRequest, ct) => {
                    csIncomingBinaryDataTransferRequests.TryAdd(incomingBinaryDataTransferRequest);
                    return Task.CompletedTask;
                };

                // Charging Station 1 is reachable via the networking node 1!
                // Good old "static routing" ;)
                testCSMS1.Routing.AddOrUpdateStaticRouting(
                    chargingStation1.Id,
                    localController1.Id
                );

                //chargingStation1.NetworkingMode = OCPP.WebSockets.NetworkingMode.NetworkingExtensions;


                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.GraphDefined_TestMessage;
                var data       = "Hello world!".ToUTF8Bytes();


                var response   = await testCSMS1.TransferBinaryData(
                                           Destination:          SourceRouting.To(chargingStation1.Id),
                                           VendorId:             vendorId,
                                           MessageId:            messageId,
                                           Data:                 data,
                                           SerializationFormat:  SerializationFormats.BinaryTextIds
                                       );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                        Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                                   Is.EqualTo(BinaryDataTransferStatus.Accepted));
                    Assert.That(response.Data?.ToUTF8String(),                                     Is.EqualTo(data.Reverse().ToUTF8String()));

                    Assert.That(csmsBinaryDataTransferRequestsOUT.   Count,                        Is.EqualTo(1), "The BinaryDataTransfer did not leave the CSMS!");

                    Assert.That(nnBinaryDataTransferRequestsIN.      Count,                        Is.EqualTo(1), "The BinaryDataTransfer did not reach the networking node!");
                    Assert.That(nnBinaryDataTransferRequestsIN.      First().DestinationId,        Is.EqualTo(chargingStation1.Id));
                    Assert.That(nnBinaryDataTransferRequestsIN.      First().NetworkPath.Length,   Is.EqualTo(1));
                    Assert.That(nnBinaryDataTransferRequestsIN.      First().NetworkPath.Source,   Is.EqualTo(testCSMS1.Id));
                    Assert.That(nnBinaryDataTransferRequestsIN.      First().NetworkPath.Last,     Is.EqualTo(testCSMS1.Id));
                    Assert.That(nnBinaryDataTransferRequestsIN.      First().VendorId,             Is.EqualTo(vendorId));
                    Assert.That(nnBinaryDataTransferRequestsIN.      First().MessageId,            Is.EqualTo(messageId));
                    Assert.That(nnBinaryDataTransferRequestsIN.      First().Data,                 Is.EqualTo(data));

                    Assert.That(nnBinaryDataTransferRequestsFWD.     Count,                        Is.EqualTo(1), "The BinaryDataTransfer did not reach the FORWARD of the networking node!");

                    Assert.That(nnBinaryDataTransferRequestsOUT.     Count,                        Is.EqualTo(1), "The BinaryDataTransfer did not reach the OUTPUT of the networking node!");

                    Assert.That(csIncomingBinaryDataTransferRequests.Count,                        Is.EqualTo(1), "The BinaryDataTransfer did not reach the charging station!");
                    //Assert.That(csIncomingBinaryDataTransferRequests.First().DestinationId,    Is.EqualTo(NetworkingNode_Id.CSMS));
                    //Assert.That(csIncomingBinaryDataTransferRequests.First().NetworkPath.Length,   Is.EqualTo(2));
                    //Assert.That(csIncomingBinaryDataTransferRequests.First().NetworkPath.Source,   Is.EqualTo(chargingStation1.Id));
                    //Assert.That(csIncomingBinaryDataTransferRequests.First().NetworkPath.Last,     Is.EqualTo(networkingNode1. Id));
                    Assert.That(csIncomingBinaryDataTransferRequests.First().VendorId,             Is.EqualTo(vendorId));
                    Assert.That(csIncomingBinaryDataTransferRequests.First().MessageId,            Is.EqualTo(messageId));
                    Assert.That(csIncomingBinaryDataTransferRequests.First().Data,                 Is.EqualTo(data));

                });

            }

        }

        #endregion


    }

}
