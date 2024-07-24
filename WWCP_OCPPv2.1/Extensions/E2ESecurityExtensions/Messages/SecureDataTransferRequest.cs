/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Diagnostics.CodeAnalysis;

using org.GraphDefined.Vanaheimr.Illias;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The SecureDataTransfer request.
    /// </summary>
    public class SecureDataTransferRequest : ARequest<SecureDataTransferRequest>,
                                             IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/secureDataTransferRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Encryption parameters.
        /// </summary>
        [Mandatory]
        public UInt16         Parameter     { get; }

        /// <summary>
        /// The unique identification of the encryption key.
        /// </summary>
        [Mandatory]
        public UInt16         KeyId         { get; }

        /// <summary>
        /// The first half of the cryptographic nonce.
        /// </summary>
        [Mandatory]
        public UInt64         Nonce         { get; }

        /// <summary>
        /// The counter part of the cryptographic nonce.
        /// </summary>
        [Mandatory]
        public UInt64         Counter       { get; }

        /// <summary>
        /// The encrypted encapsulated security payload.
        /// </summary>
        [Mandatory]
        public Byte[]         Ciphertext    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SecureDataTransfer request.
        /// </summary>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="Parameter">Encryption parameters.</param>
        /// <param name="KeyId">The unique identification of the encryption key.</param>
        /// <param name="Nonce">The first half of the cryptographic nonce.</param>
        /// <param name="Counter">The counter part of the cryptographic nonce.</param>
        /// <param name="Ciphertext">The encrypted encapsulated security payload.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public SecureDataTransferRequest(NetworkingNode_Id        DestinationId,
                                         UInt16                   Parameter,
                                         UInt16                   KeyId,
                                         UInt64                   Nonce,
                                         UInt64                   Counter,
                                         Byte[]                   Ciphertext,

                                         IEnumerable<KeyPair>?    SignKeys            = null,
                                         IEnumerable<SignInfo>?   SignInfos           = null,
                                         IEnumerable<Signature>?  Signatures          = null,

                                         Request_Id?              RequestId           = null,
                                         DateTime?                RequestTimestamp    = null,
                                         TimeSpan?                RequestTimeout      = null,
                                         EventTracking_Id?        EventTrackingId     = null,
                                         NetworkPath?             NetworkPath         = null,
                                         CancellationToken        CancellationToken   = default)

            : base(DestinationId,
                   nameof(SecureDataTransferRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   null, //CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   CancellationToken)

        {

            this.Parameter   = Parameter;
            this.KeyId       = KeyId;
            this.Nonce       = Nonce;
            this.Counter     = Counter;
            this.Ciphertext  = Ciphertext;


            unchecked
            {

                hashCode = this.Parameter. GetHashCode() * 13 ^
                           this.KeyId.     GetHashCode() * 11 ^
                           this.Nonce.     GetHashCode() *  7 ^
                           this.Counter.   GetHashCode() *  5 ^
                           this.Ciphertext.GetHashCode() *  3 ^
                           base.           GetHashCode();

            }

        }

        #endregion


        #region Encrypt(...)

        /// <summary>
        /// Create a new SecureDataTransferRequest and encrypt the given plaintext
        /// using the given cryptographic key.
        /// </summary>
        /// <param name="DestinationId"></param>
        /// <param name="Parameter"></param>
        /// <param name="KeyId"></param>
        /// <param name="Key"></param>
        /// <param name="Nonce"></param>
        /// <param name="Counter"></param>
        /// <param name="Payload"></param>
        /// 
        /// <param name="SignKeys"></param>
        /// <param name="SignInfos"></param>
        /// <param name="Signatures"></param>
        /// 
        /// <param name="RequestId"></param>
        /// <param name="RequestTimestamp"></param>
        /// <param name="RequestTimeout"></param>
        /// <param name="EventTrackingId"></param>
        /// <param name="NetworkPath"></param>
        /// <param name="CancellationToken"></param>
        public static SecureDataTransferRequest Encrypt(NetworkingNode_Id        DestinationId,
                                                        UInt16                   Parameter,
                                                        UInt16                   KeyId,
                                                        Byte[]                   Key,
                                                        UInt64                   Nonce,
                                                        UInt64                   Counter,
                                                        Byte[]                   Payload,

                                                        IEnumerable<KeyPair>?    SignKeys            = null,
                                                        IEnumerable<SignInfo>?   SignInfos           = null,
                                                        IEnumerable<Signature>?  Signatures          = null,

                                                        Request_Id?              RequestId           = null,
                                                        DateTime?                RequestTimestamp    = null,
                                                        TimeSpan?                RequestTimeout      = null,
                                                        EventTracking_Id?        EventTrackingId     = null,
                                                        NetworkPath?             NetworkPath         = null,
                                                        CancellationToken        CancellationToken   = default)
        {

            try
            {

                var nonce = new Byte[16]; // 128-bit Nonce
                Array.Copy(BitConverter.GetBytes(Nonce),   0, nonce, 0, 8);
                Array.Copy(BitConverter.GetBytes(Counter), 0, nonce, 8, 8);

                var cipher = new BufferedBlockCipher(new SicBlockCipher(new AesEngine()));
                cipher.Init(true, new ParametersWithIV(new KeyParameter(Key), nonce));

                var ciphertext = new Byte[cipher.GetOutputSize(Payload.Length)];
                int length = cipher.ProcessBytes(Payload, 0, Payload.Length, ciphertext, 0);
                cipher.DoFinal(ciphertext, length);

                return new SecureDataTransferRequest(

                           DestinationId,
                           Parameter,
                           KeyId,
                           Nonce,
                           Counter,
                           ciphertext,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           RequestId,
                           RequestTimestamp,
                           RequestTimeout,
                           EventTrackingId,
                           NetworkPath,
                           CancellationToken

                       );

            }
            catch (Exception e)
            {
                throw new Exception("Could not encrypt payload!", e);
            }

        }

        #endregion

        #region Decrypt(Key)

        /// <summary>
        /// Decrypt the ciphertext using the given cryptographic key.
        /// </summary>
        /// <param name="Key">A cryptographic key</param>
        public Byte[] Decrypt(Byte[] Key)
        {

            var nonce = new Byte[16]; // 128-bit Nonce
            Array.Copy(BitConverter.GetBytes(Nonce),   0, nonce, 0, 8);
            Array.Copy(BitConverter.GetBytes(Counter), 0, nonce, 8, 8);

            var cipher = new BufferedBlockCipher(new SicBlockCipher(new AesEngine()));
            cipher.Init(false, new ParametersWithIV(new KeyParameter(Key), nonce));

            var plaintext = new Byte[cipher.GetOutputSize(Ciphertext.Length)];
            var length    = cipher.ProcessBytes(Ciphertext, 0, Ciphertext.Length, plaintext, 0);
            cipher.DoFinal(plaintext, length);

            return plaintext;

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Secure, RequestId, DestinationId, NetworkPath, CustomDataTransferRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SecureDataTransfer request.
        /// </summary>
        /// <param name="Secure">The binary to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomDataTransferRequestParser">An optional delegate to parse custom SecureDataTransfer requests.</param>
        public static SecureDataTransferRequest Parse(Byte[]                                                  Secure,
                                                      Request_Id                                              RequestId,
                                                      NetworkingNode_Id                                       DestinationId,
                                                      NetworkPath                                             NetworkPath,
                                                      DateTime?                                               RequestTimestamp                  = null,
                                                      TimeSpan?                                               RequestTimeout                    = null,
                                                      EventTracking_Id?                                       EventTrackingId                   = null,
                                                      CustomBinaryParserDelegate<SecureDataTransferRequest>?  CustomDataTransferRequestParser   = null)
        {

            if (TryParse(Secure,
                         RequestId,
                         DestinationId,
                         NetworkPath,
                         out var secureDataTransferRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomDataTransferRequestParser))
            {
                return secureDataTransferRequest;
            }

            throw new ArgumentException("The given binary representation of a SecureDataTransfer request is invalid: " + errorResponse,
                                        nameof(Secure));

        }

        #endregion

        #region (static) TryParse(Secure, RequestId, DestinationId, NetworkPath, out SecureDataTransferRequest, out ErrorResponse, CustomSecureDataTransferRequestParser = null)

        /// <summary>
        /// Try to parse the given binary representation of a SecureDataTransfer request.
        /// </summary>
        /// <param name="Secure">The binary to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SecureDataTransferRequest">The parsed SecureDataTransfer request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSecureDataTransferRequestParser">An optional delegate to parse custom SecureDataTransfer requests.</param>
        public static Boolean TryParse(Byte[]                                                  Secure,
                                       Request_Id                                              RequestId,
                                       NetworkingNode_Id                                       DestinationId,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out SecureDataTransferRequest?     SecureDataTransferRequest,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       DateTime?                                               RequestTimestamp                        = null,
                                       TimeSpan?                                               RequestTimeout                          = null,
                                       EventTracking_Id?                                       EventTrackingId                         = null,
                                       CustomBinaryParserDelegate<SecureDataTransferRequest>?  CustomSecureDataTransferRequestParser   = null)
        {

            try
            {

                SecureDataTransferRequest  = null;

                var stream                 = new MemoryStream(Secure);

                var parameter              = stream.ReadUInt16();
                var keyId                  = stream.ReadUInt16();
                var nonce                  = stream.ReadUInt64();
                var counter                = stream.ReadUInt64();
                var ciphertextLength       = stream.ReadUInt64();
                var ciphertext             = stream.ReadBytes(ciphertextLength);

                #region Signatures

                var signatures             = new HashSet<Signature>();
                var signaturesCount        = stream.ReadByte();
                for (var i = 0; i < signaturesCount; i++)
                {

                    var signatureLength    = stream.ReadUInt16();
                    var signatureBytes     = stream.ReadBytes((UInt64) signatureLength);

                    if (!Signature.TryParse(signatureBytes, out var signature, out ErrorResponse))
                        return false;

                    signatures.Add(signature);

                }

                #endregion


                SecureDataTransferRequest  = new SecureDataTransferRequest(

                                                 DestinationId,
                                                 parameter,
                                                 keyId,
                                                 nonce,
                                                 counter,
                                                 ciphertext,

                                                 null,
                                                 null,
                                                 signatures,

                                                 RequestId,
                                                 RequestTimestamp,
                                                 RequestTimeout,
                                                 EventTrackingId,
                                                 NetworkPath

                                             );

                ErrorResponse = null;

                if (CustomSecureDataTransferRequestParser is not null)
                    SecureDataTransferRequest = CustomSecureDataTransferRequestParser(Secure,
                                                                                      SecureDataTransferRequest);

                return true;

            }
            catch (Exception e)
            {
                SecureDataTransferRequest  = null;
                ErrorResponse              = "The given binary representation of a SecureDataTransfer request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToBinary(CustomSecureDataTransferRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a binary representation of this object.
        /// </summary>
        /// <param name="CustomSecureDataTransferRequestSerializer">A delegate to serialize custom SecureDataTransfer requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="IncludeSignatures">Whether to include the digital signatures (default), or not.</param>
        public Byte[] ToBinary(CustomBinarySerializerDelegate<SecureDataTransferRequest>?  CustomSecureDataTransferRequestSerializer   = null,
                               CustomBinarySerializerDelegate<Signature>?                  CustomSignatureSerializer                   = null,
                               Boolean                                                     IncludeSignatures                           = true)
        {

            var binaryStream = new MemoryStream();

            binaryStream.WriteUInt16(Parameter);
            binaryStream.WriteUInt16(KeyId);
            binaryStream.WriteUInt64(Nonce);
            binaryStream.WriteUInt64(Counter);
            binaryStream.WriteUInt64((UInt64) Ciphertext.LongLength);
            binaryStream.Write      (Ciphertext);

            #region Signatures

            var signaturesCount = (UInt16) (IncludeSignatures ? Signatures.Count() : 0);
            binaryStream.WriteByte((Byte) signaturesCount);

            if (IncludeSignatures) {
                foreach (var signature in Signatures)
                {
                    var binarySignature = signature.ToBinary(CustomSignatureSerializer);
                    binaryStream.WriteUInt16((UInt16) binarySignature.Length);
                    binaryStream.Write(binarySignature);
                }
            }

            #endregion

            var binary = binaryStream.ToArray();

            return CustomSecureDataTransferRequestSerializer is not null
                       ? CustomSecureDataTransferRequestSerializer(this, binary)
                       : binary;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SecureDataTransferRequest1, SecureDataTransferRequest2)

        /// <summary>
        /// Compares two SecureDataTransfer requests for equality.
        /// </summary>
        /// <param name="SecureDataTransferRequest1">A SecureDataTransfer request.</param>
        /// <param name="SecureDataTransferRequest2">Another SecureDataTransfer request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SecureDataTransferRequest? SecureDataTransferRequest1,
                                           SecureDataTransferRequest? SecureDataTransferRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SecureDataTransferRequest1, SecureDataTransferRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SecureDataTransferRequest1 is null || SecureDataTransferRequest2 is null)
                return false;

            return SecureDataTransferRequest1.Equals(SecureDataTransferRequest2);

        }

        #endregion

        #region Operator != (SecureDataTransferRequest1, SecureDataTransferRequest2)

        /// <summary>
        /// Compares two SecureDataTransfer requests for inequality.
        /// </summary>
        /// <param name="SecureDataTransferRequest1">A SecureDataTransfer request.</param>
        /// <param name="SecureDataTransferRequest2">Another SecureDataTransfer request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SecureDataTransferRequest? SecureDataTransferRequest1,
                                           SecureDataTransferRequest? SecureDataTransferRequest2)

            => !(SecureDataTransferRequest1 == SecureDataTransferRequest2);

        #endregion

        #endregion

        #region IEquatable<SecureDataTransferRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SecureDataTransfer requests for equality.
        /// </summary>
        /// <param name="Object">A SecureDataTransfer request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SecureDataTransferRequest secureDataTransferRequest &&
                   Equals(secureDataTransferRequest);

        #endregion

        #region Equals(SecureDataTransferRequest)

        /// <summary>
        /// Compares two SecureDataTransfer requests for equality.
        /// </summary>
        /// <param name="SecureDataTransferRequest">A SecureDataTransfer request to compare with.</param>
        public override Boolean Equals(SecureDataTransferRequest? SecureDataTransferRequest)

            => SecureDataTransferRequest is not null               &&

             //  VendorId.Equals(SecureDataTransferRequest.VendorId) &&

             //((MessageId is     null && SecureDataTransferRequest.MessageId is     null) ||
             // (MessageId is not null && SecureDataTransferRequest.MessageId is not null && MessageId.  Equals(SecureDataTransferRequest.MessageId))) &&

             //((Payload      is     null && SecureDataTransferRequest.Payload      is     null) ||
             // (Payload      is not null && SecureDataTransferRequest.Payload      is not null && Payload.SequenceEqual(SecureDataTransferRequest.Payload)))      &&

               base.GenericEquals(SecureDataTransferRequest);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{KeyId} / {Counter}";

        #endregion


    }

}
