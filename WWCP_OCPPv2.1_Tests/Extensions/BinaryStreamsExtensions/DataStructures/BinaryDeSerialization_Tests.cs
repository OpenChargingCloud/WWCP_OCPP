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

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.BinaryStreamsExtensions.DataStructures
{

    /// <summary>
    /// Unit tests for charging stations sending messages to the CSMS.
    /// </summary>
    [TestFixture]
    public class BinaryDeSerialization_Tests
    {

        #region SerializeDeserialize_BinaryDataTransferRequests_Test()

        /// <summary>
        /// A test for transfering binary data to the CSMS.
        /// </summary>
        [Test]
        public void SerializeDeserialize_BinaryDataTransferRequests_Test()
        {

            var keyPair                    = KeyPair.GenerateKeys()!;
            var chargingStationId          = NetworkingNode_Id.Parse("DE*GEF*S12345678");
            var vendorId                   = Vendor_Id. GraphDefined;
            var messageId                  = Message_Id.GraphDefined_TestMessage;
            var data                       = "Hello world!".ToUTF8Bytes();

            var binaryDataTransferRequest  = new BinaryDataTransferRequest(
                                                 chargingStationId,
                                                 vendorId,
                                                 messageId,
                                                 data,
                                                 BinaryFormats.Compact
                                             );

            var signSuccess                = binaryDataTransferRequest.Sign(
                                                 binaryDataTransferRequest.ToBinary(
                                                     CustomBinaryDataTransferRequestSerializer:  null,
                                                     IncludeSignatures:                          false
                                                 ),
                                                 //BinaryDataTransferRequest.DefaultJSONLDContext,
                                                 keyPair,
                                                 out var errorResponse1,
                                                 SignerName:   "TestSigner1",
                                                 Description:  "TestDescription1".ToI18NString(),
                                                 Timestamp:     Timestamp.Now
                                             );

            Assert.IsTrue(signSuccess);
            Assert.IsNull(errorResponse1);


            var serializedRequest          = binaryDataTransferRequest.ToBinary();

            Assert.IsNotNull(serializedRequest);


            var success                    = BinaryDataTransferRequest.TryParse(
                                                 serializedRequest,
                                                 Request_Id.Parse("2"),
                                                 chargingStationId,
                                                 NetworkPath.Empty,
                                                 out var parsedBinaryDataTransferRequest,
                                                 out var errorResponse2
                                             );

            Assert.IsTrue   (success);
            Assert.IsNull   (errorResponse2);
            Assert.IsNotNull(parsedBinaryDataTransferRequest);

            if (parsedBinaryDataTransferRequest is not null)
            {

                Assert.AreEqual (vendorId,              parsedBinaryDataTransferRequest.VendorId);
                Assert.AreEqual (messageId,             parsedBinaryDataTransferRequest.MessageId);
                Assert.AreEqual (data.ToUTF8String(),   parsedBinaryDataTransferRequest.Data?.ToUTF8String());
                Assert.AreEqual (1,                     parsedBinaryDataTransferRequest.Signatures.Count());

                var verifed = parsedBinaryDataTransferRequest.Verify(parsedBinaryDataTransferRequest.ToBinary(
                                                                         CustomBinaryDataTransferRequestSerializer:   null,
                                                                         IncludeSignatures:                           false
                                                                     ),
                                                                     out var errorResponse3,
                                                                     VerificationRuleActions.VerifyAll);

                Assert.IsTrue(verifed);
                Assert.IsNull(errorResponse3);

            }

        }

        #endregion

    }

}
