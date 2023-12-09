/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.BinaryStreamsExtensions
{

    /// <summary>
    /// Unit tests for a CSMS sending binary data messages to charging stations.
    /// </summary>
    [TestFixture]
    public class CSMS_SendBinaryData_Tests : AChargingStationTests
    {

        #region TransferBinaryData_Test()

        /// <summary>
        /// A test for transfering binary data to charging stations.
        /// </summary>
        [Test]
        public async Task TransferBinaryData_Test()
        {

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var binaryDataTransferRequests = new ConcurrentList<OCPP.CSMS.BinaryDataTransferRequest>();

                chargingStation1.OnIncomingBinaryDataTransferRequest += (timestamp, sender, binaryDataTransferRequest) => {
                    binaryDataTransferRequests.TryAdd(binaryDataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.GraphDefined_TestMessage;
                var data       = "Hello world!".ToUTF8Bytes();

                var response   = await testCSMS01.TransferBinaryData(
                                     NetworkingNodeId:    chargingStation1.Id,
                                     VendorId:            vendorId,
                                     MessageId:           messageId,
                                     Data:                data
                                 );


                Assert.AreEqual(ResultCode.OK,                   response.Result.ResultCode);
                Assert.AreEqual(data.Reverse().ToUTF8String(),   response.Data?.ToUTF8String());

                Assert.AreEqual(1,                               binaryDataTransferRequests.Count);
                Assert.AreEqual(chargingStation1.Id,             binaryDataTransferRequests.First().NetworkingNodeId);
                Assert.AreEqual(vendorId,                        binaryDataTransferRequests.First().VendorId);
                Assert.AreEqual(messageId,                       binaryDataTransferRequests.First().MessageId);
                Assert.AreEqual(data,                            binaryDataTransferRequests.First().Data);

            }

        }

        #endregion


    }

}
