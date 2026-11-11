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

using System.Buffers.Binary;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The encryption methods of SecureDataTransfer, selected by its "Parameter".
    /// </summary>
    /// <remarks>
    /// <para>
    /// White paper "Signed OCPP Messages and Signer Roles", section 2.6 and Q20:
    /// AES-CTR (0) stays as it was, and AES-GCM (1) is a second method, which also
    /// authenticates the ciphertext and the header fields before it.
    /// </para>
    /// <para>
    /// AES-GCM takes its 96-bit IV and its additional authenticated data from the
    /// header as it is on the wire (big-endian): the IV is the 8 bytes of the nonce
    /// and the last 4 bytes of the counter, the additional authenticated data the 20
    /// bytes of parameter, key id, nonce and counter. The 16-byte tag follows the
    /// ciphertext. AES-CTR builds its IV in the machine's byte order, as it always did.
    /// </para>
    /// </remarks>
    public static class SecureDataTransferMethods
    {

        #region Methods

        /// <summary>
        /// AES in counter mode, without authentication.
        /// </summary>
        public const UInt16 AES_CTR  = 0;

        /// <summary>
        /// AES in Galois/counter mode: encrypted and authenticated.
        /// </summary>
        public const UInt16 AES_GCM  = 1;

        /// <summary>
        /// The length of an AES-GCM tag in bytes.
        /// </summary>
        public const Int32  TagSize  = 16;

        /// <summary>
        /// Whether the given parameter names a method this library knows.
        /// </summary>
        public static Boolean IsKnown(UInt16 Parameter)

            => Parameter is AES_CTR or AES_GCM;

        #endregion


        #region Header(Parameter, KeyId, Nonce, Counter)

        /// <summary>
        /// The first 20 bytes of a SecureDataTransfer message on the wire.
        /// </summary>
        public static Byte[] Header(UInt16  Parameter,
                                    UInt16  KeyId,
                                    UInt64  Nonce,
                                    UInt64  Counter)
        {

            var header = new Byte[20];

            BinaryPrimitives.WriteUInt16BigEndian(header.AsSpan( 0, 2), Parameter);
            BinaryPrimitives.WriteUInt16BigEndian(header.AsSpan( 2, 2), KeyId);
            BinaryPrimitives.WriteUInt64BigEndian(header.AsSpan( 4, 8), Nonce);
            BinaryPrimitives.WriteUInt64BigEndian(header.AsSpan(12, 8), Counter);

            return header;

        }

        #endregion

        #region Encrypt(Parameter, KeyId, Key, Nonce, Counter, Plaintext)

        /// <summary>
        /// Encrypt the given plaintext by the method the parameter names.
        /// </summary>
        /// <exception cref="ArgumentException">The parameter names no method this library knows.</exception>
        public static Byte[] Encrypt(UInt16  Parameter,
                                     UInt16  KeyId,
                                     Byte[]  Key,
                                     UInt64  Nonce,
                                     UInt64  Counter,
                                     Byte[]  Plaintext)

            => Parameter switch {
                   AES_CTR => CTR(true, Key, Nonce, Counter, Plaintext),
                   AES_GCM => GCM(true, Key, Header(Parameter, KeyId, Nonce, Counter), Plaintext),
                   _       => throw new ArgumentException($"Unknown SecureDataTransfer method '{Parameter}'!", nameof(Parameter))
               };

        #endregion

        #region Decrypt(Parameter, KeyId, Key, Nonce, Counter, Ciphertext)

        /// <summary>
        /// Decrypt the given ciphertext by the method the parameter names.
        /// </summary>
        /// <exception cref="ArgumentException">The parameter names no method this library knows.</exception>
        /// <exception cref="InvalidCipherTextException">AES-GCM: the tag does not verify; the ciphertext or the header was changed, or the key is another.</exception>
        public static Byte[] Decrypt(UInt16  Parameter,
                                     UInt16  KeyId,
                                     Byte[]  Key,
                                     UInt64  Nonce,
                                     UInt64  Counter,
                                     Byte[]  Ciphertext)

            => Parameter switch {
                   AES_CTR => CTR(false, Key, Nonce, Counter, Ciphertext),
                   AES_GCM => GCM(false, Key, Header(Parameter, KeyId, Nonce, Counter), Ciphertext),
                   _       => throw new ArgumentException($"Unknown SecureDataTransfer method '{Parameter}'!", nameof(Parameter))
               };

        #endregion


        #region (private) CTR(Encrypt, Key, Nonce, Counter, Input)

        private static Byte[] CTR(Boolean  Encrypt,
                                  Byte[]   Key,
                                  UInt64   Nonce,
                                  UInt64   Counter,
                                  Byte[]   Input)
        {

            var iv = new Byte[16];
            Array.Copy(BitConverter.GetBytes(Nonce),   0, iv, 0, 8);
            Array.Copy(BitConverter.GetBytes(Counter), 0, iv, 8, 8);

            var cipher = new BufferedBlockCipher(new SicBlockCipher(new AesEngine()));
            cipher.Init(Encrypt, new ParametersWithIV(new KeyParameter(Key), iv));

            var output = new Byte[cipher.GetOutputSize(Input.Length)];
            var length = cipher.ProcessBytes(Input, 0, Input.Length, output, 0);
            cipher.DoFinal(output, length);

            return output;

        }

        #endregion

        #region (private) GCM(Encrypt, Key, Header, Input)

        private static Byte[] GCM(Boolean  Encrypt,
                                  Byte[]   Key,
                                  Byte[]   Header,
                                  Byte[]   Input)
        {

            // 96-bit IV: the nonce and the last 4 bytes of the counter, as on the wire.
            var iv = new Byte[12];
            Array.Copy(Header,  4, iv, 0, 8);
            Array.Copy(Header, 16, iv, 8, 4);

            var cipher = new GcmBlockCipher(new AesEngine());
            cipher.Init(Encrypt, new AeadParameters(new KeyParameter(Key), TagSize * 8, iv, Header));

            var output = new Byte[cipher.GetOutputSize(Input.Length)];
            var length = cipher.ProcessBytes(Input, 0, Input.Length, output, 0);
            length    += cipher.DoFinal(output, length);

            return length == output.Length
                       ? output
                       : output[..length];

        }

        #endregion

    }

}
