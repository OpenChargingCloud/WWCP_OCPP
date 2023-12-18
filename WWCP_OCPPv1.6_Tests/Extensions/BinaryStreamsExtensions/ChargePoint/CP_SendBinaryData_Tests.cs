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
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.extensions.BinaryStreamsExtensions
{

    /// <summary>
    /// Unit tests for charge points sending binary messages to the central system.
    /// </summary>
    [TestFixture]
    public class CP_SendBinaryData_Tests : AChargePointTests
    {

        #region TransferBinaryData_Test()

        /// <summary>
        /// A test for transfering binary data to the CSMS.
        /// </summary>
        [Test]
        public async Task TransferBinaryData_Test()
        {

            Assert.Multiple(() => {
                Assert.That(testCentralSystem01,      Is.Not.Null);
                Assert.That(testBackendWebSockets01,  Is.Not.Null);
                Assert.That(chargePoint1,             Is.Not.Null);
                Assert.That(chargePoint2,             Is.Not.Null);
                Assert.That(chargePoint3,             Is.Not.Null);
            });

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1            is not null &&
                chargePoint2            is not null &&
                chargePoint3            is not null)
            {

                var binaryDataTransferRequests= new ConcurrentList<OCPP.CS.BinaryDataTransferRequest>();

                testCentralSystem01.OnIncomingBinaryDataTransferRequest += (timestamp, sender, connection, binaryDataTransferRequest) => {
                    binaryDataTransferRequests.TryAdd(binaryDataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.GraphDefined_TestMessage;
                var data       = "Hello world!".ToUTF8Bytes();

                var response   = await chargePoint1.TransferBinaryData(
                                           VendorId:    vendorId,
                                           MessageId:   messageId,
                                           Data:        data,
                                           Format:      BinaryFormats.TextIds
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                            Is.EqualTo(ResultCode. OK));
                    Assert.That(response.Status,                                       Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data?.ToUTF8String(),                         Is.EqualTo(data.Reverse().ToUTF8String()));

                    Assert.That(binaryDataTransferRequests.Count,                      Is.EqualTo(1));
                    Assert.That(binaryDataTransferRequests.First().DestinationNodeId,   Is.EqualTo(chargePoint1.Id));
                    Assert.That(binaryDataTransferRequests.First().VendorId,           Is.EqualTo(vendorId));
                    Assert.That(binaryDataTransferRequests.First().MessageId,          Is.EqualTo(messageId));
                    Assert.That(binaryDataTransferRequests.First().Data,               Is.EqualTo(data));

                });

             }

        }

        #endregion

    }

}
