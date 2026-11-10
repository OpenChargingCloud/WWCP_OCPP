/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.E2ESecurityExtensions
{

    /// <summary>
    /// SecureDataTransfer encrypts with AES in counter mode, and its IV is the nonce of the
    /// message followed by its counter. Both used to be the constant 1, so every message
    /// under one key had the same key stream, and the XOR of two ciphertexts was the XOR of
    /// their plaintexts.
    /// </summary>
    [TestFixture]
    public class SecureDataTransferNonceTests : AChargingStationTests
    {

        #region EveryMessageGetsANonceOfItsOwn()

        [Test]
        public void EveryMessageGetsANonceOfItsOwn()
        {

            Assert.That(chargingStation1, Is.Not.Null);

            var nonces = Enumerable.Range(0, 100).
                                    Select(_ => chargingStation1!.GetEncryptionNonce(NetworkingNode_Id.CSMS, 1)).
                                    ToList();

            Assert.That(nonces.Distinct().Count(), Is.EqualTo(nonces.Count),
                        "Two messages under the same key got the same nonce!");

        }

        #endregion

        #region TheSameTextSentTwiceIsTwoDifferentCiphertexts()

        [Test]
        public void TheSameTextSentTwiceIsTwoDifferentCiphertexts()
        {

            Assert.That(chargingStation1, Is.Not.Null);

            var key = "0123456789abcdef"u8.ToArray();

            SecureDataTransferRequest Encrypted()

                => SecureDataTransferRequest.Encrypt(
                       Destination:  SourceRouting.CSMS,
                       Parameter:    0,
                       KeyId:        1,
                       Key:          key,
                       Nonce:        chargingStation1!.GetEncryptionNonce  (NetworkingNode_Id.CSMS, 1),
                       Counter:      chargingStation1!.GetEncryptionCounter(NetworkingNode_Id.CSMS, 1),
                       Payload:      "The same text, twice".ToUTF8Bytes()
                   );

            var first   = Encrypted();
            var second  = Encrypted();

            Assert.Multiple(() => {
                Assert.That(second.Ciphertext, Is.Not.EqualTo(first.Ciphertext),
                            "The same text gave the same ciphertext twice: the key stream came round again!");
                Assert.That(first. Decrypt(key).ToUTF8String(), Is.EqualTo("The same text, twice"));
                Assert.That(second.Decrypt(key).ToUTF8String(), Is.EqualTo("The same text, twice"));
            });

        }

        #endregion

    }

}
