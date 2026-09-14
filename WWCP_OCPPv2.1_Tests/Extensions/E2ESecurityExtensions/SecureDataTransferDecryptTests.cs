/*
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

using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.E2ESecurityExtensions
{

    /// <summary>
    /// A SecureDataTransfer response carries no ciphertext when it never carried a payload -
    /// a timeout, or a rejection. Decrypt() used to walk straight into that null.
    /// </summary>
    [TestFixture]
    public class SecureDataTransferDecryptTests
    {

        #region (private) Key / ARequest()

        private static readonly Byte[] key = "0123456789abcdef"u8.ToArray();

        private static SecureDataTransferRequest ARequest()

            => SecureDataTransferRequest.Encrypt(
                   Destination:  SourceRouting.To(NetworkingNode_Id.Parse("lc01")),
                   Parameter:    0,
                   KeyId:        1,
                   Key:          key,
                   Nonce:        1,
                   Counter:      1,
                   Payload:      "Hello world!".ToUTF8Bytes()
               );

        #endregion


        #region ARoundTrip_StillWorks()

        /// <summary>
        /// The guard must not get in the way of an actual decryption.
        /// </summary>
        [Test]
        public void ARoundTrip_StillWorks()
        {

            var request = ARequest();

            Assert.Multiple(() => {
                Assert.That(request.Ciphertext,                      Is.Not.Null);
                Assert.That(request.Decrypt(key).ToUTF8String(),     Is.EqualTo("Hello world!"));
            });

        }

        #endregion

        #region AResponseWithoutACiphertext_SaysSo()

        /// <summary>
        /// This is what a timed-out SecureDataTransfer looks like on the way back.
        /// It used to throw a bare NullReferenceException from inside Decrypt().
        /// </summary>
        [Test]
        public void AResponseWithoutACiphertext_SaysSo()
        {

            var response = new SecureDataTransferResponse(
                               Request:  ARequest(),
                               Status:   SecureDataTransferStatus.Rejected
                           );

            Assert.That(response.Ciphertext, Is.Null);

            var exception = Assert.Throws<InvalidOperationException>(() => response.Decrypt(key));

            Assert.Multiple(() => {
                Assert.That(exception,          Is.Not.InstanceOf<NullReferenceException>());
                Assert.That(exception!.Message, Does.Contain("no ciphertext"));
            });

        }

        #endregion

    }

}
