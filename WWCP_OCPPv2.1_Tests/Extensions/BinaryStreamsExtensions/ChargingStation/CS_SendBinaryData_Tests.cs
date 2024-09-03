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
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.BinaryStreamsExtensions
{

    /// <summary>
    /// Unit tests for charging stations sending binary messages to the CSMS.
    /// </summary>
    [TestFixture]
    public class CS_SendBinaryData_Tests : AChargingStationTests
    {

        #region TransferBinaryData_Test()

        /// <summary>
        /// A test for transfering binary data to the CSMS.
        /// </summary>
        [Test]
        public async Task TransferBinaryData_Test()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS01,               Is.Not.Null);
                Assert.That(testBackendWebSockets01,  Is.Not.Null);
                Assert.That(chargingStation1,         Is.Not.Null);
                Assert.That(chargingStation2,         Is.Not.Null);
                Assert.That(chargingStation3,         Is.Not.Null);
            });

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var binaryDataTransferRequests= new ConcurrentList<BinaryDataTransferRequest>();

                testCSMS01.OCPP.IN.OnBinaryDataTransferRequestReceived += (timestamp, sender, connection, binaryDataTransferRequest, ct) => {
                    binaryDataTransferRequests.TryAdd(binaryDataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.GraphDefined_TestMessage;
                var data       = "Hello world!".ToUTF8Bytes();


                var response   = await chargingStation1.TransferBinaryData(
                                           VendorId:             vendorId,
                                           MessageId:            messageId,
                                           Data:                 data,
                                           SerializationFormat:  SerializationFormats.BinaryTextIds
                                       );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                              Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                         Is.EqualTo(BinaryDataTransferStatus.Accepted));
                    Assert.That(response.Data?.ToUTF8String(),                           Is.EqualTo(data.Reverse().ToUTF8String()));

                    Assert.That(binaryDataTransferRequests.Count,                        Is.EqualTo(1), "The BinaryDataTransfer did not reach the CSMS!");
                    Assert.That(binaryDataTransferRequests.First().DestinationId,        Is.EqualTo(NetworkingNode_Id.Zero)); // Because of standard networking mode!
                    Assert.That(binaryDataTransferRequests.First().NetworkPath.Length,   Is.EqualTo(1));
                    Assert.That(binaryDataTransferRequests.First().NetworkPath.Source,   Is.EqualTo(chargingStation1.Id));
                    Assert.That(binaryDataTransferRequests.First().NetworkPath.Last,     Is.EqualTo(chargingStation1.Id));
                    Assert.That(binaryDataTransferRequests.First().VendorId,             Is.EqualTo(vendorId));
                    Assert.That(binaryDataTransferRequests.First().MessageId,            Is.EqualTo(messageId));
                    Assert.That(binaryDataTransferRequests.First().Data,                 Is.EqualTo(data));

                });

            }

        }

        #endregion

    }

}
