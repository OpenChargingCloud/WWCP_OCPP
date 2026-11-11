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

using Org.BouncyCastle.Crypto;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.E2ESecurityExtensions
{

    /// <summary>
    /// SecureDataTransfer with AES-GCM, the second method beside AES-CTR (white paper
    /// "Signed OCPP Messages and Signer Roles", section 2.6, Q20): the ciphertext and the
    /// header before it are authenticated, so that a change to either is refused.
    /// </summary>
    [TestFixture]
    public class SecureDataTransferAESGCMTests
    {

        #region Data

        private static readonly Byte[]         key          = Convert.FromHexString("5a733d6660df00c447ff184ae971e1d5bba5de5784768795ee6535867130aa12");
        private static readonly Byte[]         anotherKey   = Convert.FromHexString("00733d6660df00c447ff184ae971e1d5bba5de5784768795ee6535867130aa12");
        private static readonly SourceRouting  toTheLC      = SourceRouting.To(NetworkingNode_Id.Parse("lc01"));
        private const           UInt64         nonce        = 0x0102030405060708;
        private const           UInt64         counter      = 0x1112131415161718;
        private const           String         text         = "Hello world, authenticated!";

        #endregion

        #region (private) ARequest(Parameter = AES-GCM) / Changed(...)

        private static SecureDataTransferRequest ARequest(UInt16 Parameter = SecureDataTransferMethods.AES_GCM)

            => SecureDataTransferRequest.Encrypt(
                   Destination:  toTheLC,
                   Parameter:    Parameter,
                   KeyId:        1,
                   Key:          key,
                   Nonce:        nonce,
                   Counter:      counter,
                   Payload:      text.ToUTF8Bytes()
               );

        /// <summary>
        /// The same ciphertext under other header fields, as an attacker on the way could send it.
        /// </summary>
        private static SecureDataTransferRequest Changed(SecureDataTransferRequest  Request,
                                                         UInt16?                    Parameter    = null,
                                                         UInt16?                    KeyId        = null,
                                                         UInt64?                    Nonce        = null,
                                                         UInt64?                    Counter      = null,
                                                         Byte[]?                    Ciphertext   = null)

            => new (Request.Destination,
                    Parameter  ?? Request.Parameter,
                    KeyId      ?? Request.KeyId,
                    Nonce      ?? Request.Nonce,
                    Counter    ?? Request.Counter,
                    Ciphertext ?? Request.Ciphertext);

        #endregion


        #region TheCiphertextIsTheOneOpenSSLComputes()

        /// <summary>
        /// The same key, header and text give the bytes Node.js (OpenSSL) computes by section 2.6:
        /// IV 0102030405060708 15161718, the 20 header bytes as additional data, the tag at the end.
        /// </summary>
        [Test]
        public void TheCiphertextIsTheOneOpenSSLComputes()
        {

            var request = ARequest();

            Assert.Multiple(() => {
                Assert.That(Convert.ToHexString(SecureDataTransferMethods.Header(1, 1, nonce, counter)),
                            Is.EqualTo("0001000101020304050607081112131415161718"));
                Assert.That(Convert.ToHexString(request.Ciphertext),
                            Is.EqualTo("CE836DED840ADB2D2B648BBA84DEDCF964F4E240B5123BF27C64F52188F6F3499E9F94C06B50C18A0E4FF6"));
                Assert.That(request.Decrypt(key).ToUTF8String(), Is.EqualTo(text));
            });

        }

        #endregion

        #region ARequestAndItsResponseRoundTrip()

        [Test]
        public void ARequestAndItsResponseRoundTrip()
        {

            var request   = ARequest();
            var response  = SecureDataTransferResponse.Encrypt(
                                Request:      request,
                                Status:       SecureDataTransferStatus.Accepted,
                                Destination:  SourceRouting.CSMS,
                                Parameter:    request.Parameter,
                                KeyId:        2,
                                Key:          key,
                                Nonce:        nonce + 1,
                                Counter:      1,
                                Payload:      "!detacitnehtua ,dlrow olleH".ToUTF8Bytes()
                            );

            Assert.Multiple(() => {
                Assert.That(request.Ciphertext.Length,             Is.EqualTo(text.Length + SecureDataTransferMethods.TagSize));
                Assert.That(request.Decrypt(key).ToUTF8String(),   Is.EqualTo(text));
                Assert.That(response.Parameter,                    Is.EqualTo(SecureDataTransferMethods.AES_GCM));
                Assert.That(response.Decrypt(key).ToUTF8String(),  Is.EqualTo("!detacitnehtua ,dlrow olleH"));
            });

        }

        #endregion

        #region AChangedHeaderFieldIsRefused(Field)

        /// <summary>
        /// The header is the additional authenticated data: another key id, nonce or counter
        /// under the same ciphertext does not decrypt.
        /// </summary>
        [TestCase("KeyId")]
        [TestCase("Nonce")]
        [TestCase("Counter")]
        public void AChangedHeaderFieldIsRefused(String Field)
        {

            var request  = ARequest();
            var changed  = Field switch {
                               "KeyId"  => Changed(request, KeyId:   2),
                               "Nonce"  => Changed(request, Nonce:   nonce + 1),
                               _        => Changed(request, Counter: counter + 1)
                           };

            Assert.Multiple(() => {
                Assert.That(changed.TryDecrypt(key), Is.Null, $"A changed {Field} decrypted!");
                Assert.Throws<InvalidCipherTextException>(() => changed.Decrypt(key));
            });

        }

        #endregion

        #region AChangedCiphertextOrTagIsRefused(Position)

        [TestCase(0,  TestName = "AChangedCiphertextIsRefused")]
        [TestCase(-1, TestName = "AChangedTagIsRefused")]
        public void AChangedCiphertextOrTagIsRefused(Int32 Position)
        {

            var request    = ARequest();
            var ciphertext = request.Ciphertext.ToArray();
            ciphertext[Position < 0 ? ciphertext.Length + Position : Position] ^= 0x01;

            Assert.That(Changed(request, Ciphertext: ciphertext).TryDecrypt(key), Is.Null);

        }

        #endregion

        #region AnotherKeyIsRefused()

        [Test]
        public void AnotherKeyIsRefused()
        {
            Assert.That(ARequest().TryDecrypt(anotherKey), Is.Null);
        }

        #endregion

        #region AnUnknownMethodIsRefused()

        /// <summary>
        /// A parameter that names no method: the sender cannot encrypt by it, and a receiver
        /// gets no plaintext - the test nodes answer it "Rejected".
        /// </summary>
        [Test]
        public void AnUnknownMethodIsRefused()
        {

            Assert.Multiple(() => {
                Assert.That(SecureDataTransferMethods.IsKnown(7), Is.False);
                Assert.Throws<Exception>(() => ARequest(7));
                Assert.That(Changed(ARequest(), Parameter: 7).TryDecrypt(key), Is.Null);
            });

        }

        #endregion

        #region AESCTRStaysAsItWas()

        /// <summary>
        /// AES-CTR (parameter 0) is unchanged, and still authenticates nothing: a changed
        /// byte gives a changed plaintext, not a refusal. That is why AES-GCM exists.
        /// </summary>
        [Test]
        public void AESCTRStaysAsItWas()
        {

            var request    = ARequest(SecureDataTransferMethods.AES_CTR);
            var ciphertext = request.Ciphertext.ToArray();
            ciphertext[0] ^= 0x01;

            Assert.Multiple(() => {
                Assert.That(request.Ciphertext.Length,                 Is.EqualTo(text.Length));
                Assert.That(request.Decrypt(key).ToUTF8String(),       Is.EqualTo(text));
                Assert.That(Changed(request, Ciphertext: ciphertext).TryDecrypt(key)?.ToUTF8String(),
                            Is.Not.Null.And.Not.EqualTo(text));
            });

        }

        #endregion

    }

}
