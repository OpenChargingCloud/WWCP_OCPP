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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The SecureDataTransfer response.
    /// </summary>
    public class SecureDataTransferResponse : AResponse<SecureDataTransferRequest,
                                                        SecureDataTransferResponse>,
                                              IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/secureDataTransferResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext             Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure status of the SecureDataTransfer.
        /// </summary>
        [Mandatory]
        public SecureDataTransferStatus  Status                  { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public String?                   AdditionalStatusInfo    { get; }

        /// <summary>
        /// Optional encryption parameters.
        /// </summary>
        [Optional]
        public UInt16?                   Parameter               { get; }

        /// <summary>
        /// The optional unique identification of the encryption key.
        /// </summary>
        [Optional]
        public UInt16?                   KeyId                   { get; }

        /// <summary>
        /// The optional first half of the cryptographic nonce.
        /// </summary>
        [Optional]
        public UInt64?                   Nonce                   { get; }

        /// <summary>
        /// The optional counter part of the cryptographic nonce.
        /// </summary>
        [Optional]
        public UInt64?                   Counter                 { get; }

        /// <summary>
        /// The encrypted encapsulated optional security payload.
        /// </summary>
        [Optional]
        public Byte[]?                   Ciphertext              { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SecureDataTransfer response.
        /// </summary>
        /// <param name="Request">The SecureDataTransfer request leading to this response.</param>
        /// <param name="Status">The success or failure status of the SecureDataTransfer.</param>
        /// <param name="AdditionalStatusInfo">Optional detailed status information.</param>
        /// <param name="Parameter">Optional encryption parameters.</param>
        /// <param name="KeyId">The optional unique identification of the encryption key.</param>
        /// <param name="Nonce">The optional first half of the cryptographic nonce.</param>
        /// <param name="Counter">The optional counter part of the cryptographic nonce.</param>
        /// <param name="Ciphertext">The encrypted encapsulated optional security payload.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        public SecureDataTransferResponse(SecureDataTransferRequest  Request,
                                          SecureDataTransferStatus   Status,
                                          String?                    AdditionalStatusInfo   = null,
                                          UInt16?                    Parameter              = null,
                                          UInt16?                    KeyId                  = null,
                                          UInt64?                    Nonce                  = null,
                                          UInt64?                    Counter                = null,
                                          Byte[]?                    Ciphertext             = null,

                                          Result?                    Result                 = null,
                                          DateTimeOffset?            ResponseTimestamp      = null,

                                          SourceRouting?             Destination            = null,
                                          NetworkPath?               NetworkPath            = null,

                                          IEnumerable<KeyPair>?      SignKeys               = null,
                                          IEnumerable<SignInfo>?     SignInfos              = null,
                                          IEnumerable<Signature>?    Signatures             = null,

                                          SerializationFormats?      SerializationFormat    = null,
                                          CancellationToken          CancellationToken      = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   null,

                   SerializationFormat ?? SerializationFormats.BinaryTextIds,
                   CancellationToken)

        {

            this.Status                = Status;
            this.AdditionalStatusInfo  = AdditionalStatusInfo;

            this.Parameter             = Parameter;
            this.KeyId                 = KeyId;
            this.Nonce                 = Nonce;
            this.Counter               = Counter;
            this.Ciphertext            = Ciphertext;

            unchecked
            {

                hashCode = this.Parameter.  GetHashCode()       * 13 ^
                           this.KeyId.      GetHashCode()       * 11 ^
                           this.Nonce.      GetHashCode()       *  7 ^
                           this.Counter.    GetHashCode()       *  5 ^
                          (this.Ciphertext?.GetHashCode() ?? 0) *  3 ^
                           base.            GetHashCode();

            }

        }

        #endregion


        #region Encrypt(...)

        /// <summary>
        /// Create a new SecureDataTransferRequest and encrypt the given plaintext
        /// using the given cryptographic key.
        /// </summary>
        /// <param name="Destination"></param>
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
        public static SecureDataTransferResponse Encrypt(SecureDataTransferRequest  Request,
                                                         SecureDataTransferStatus   Status,
                                                         SourceRouting              Destination,
                                                         UInt16                     Parameter,
                                                         UInt16                     KeyId,
                                                         Byte[]                     Key,
                                                         UInt64                     Nonce,
                                                         UInt64                     Counter,
                                                         Byte[]                     Payload,

                                                         String?                    AdditionalStatusInfo   = null,

                                                         IEnumerable<KeyPair>?      SignKeys               = null,
                                                         IEnumerable<SignInfo>?     SignInfos              = null,
                                                         IEnumerable<Signature>?    Signatures             = null,

                                                         DateTimeOffset?            ResponseTimestamp      = null,
                                                         EventTracking_Id?          EventTrackingId        = null,
                                                         NetworkPath?               NetworkPath            = null)
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

                return new SecureDataTransferResponse(

                           Request,
                           Status,
                           AdditionalStatusInfo,

                           Parameter,
                           KeyId,
                           Nonce,
                           Counter,
                           ciphertext,

                           Result.OK(),
                           ResponseTimestamp,

                           Destination,
                           NetworkPath,

                           SignKeys,
                           SignInfos,
                           Signatures

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
            Array.Copy(BitConverter.GetBytes(Nonce   ?? 0), 0, nonce, 0, 8);
            Array.Copy(BitConverter.GetBytes(Counter ?? 0), 0, nonce, 8, 8);

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

        #region (static) Parse   (Request, SecureData, CustomSecureDataTransferResponseSerializer = null)

        /// <summary>
        /// Parse the given JSON representation of a SecureDataTransfer response.
        /// </summary>
        /// <param name="Request">The SecureDataTransfer request leading to this response.</param>
        /// <param name="SecureData">The binary to be parsed.</param>
        /// <param name="CustomSecureDataTransferResponseParser">An optional delegate to parse custom SecureDataTransfer responses.</param>
        /// <param name="CustomBinarySignatureParser">An optional delegate to parse custom binary signatures.</param>
        public static SecureDataTransferResponse Parse(SecureDataTransferRequest                                Request,
                                                       Byte[]                                                   SecureData,
                                                       SourceRouting                                        Destination,
                                                       NetworkPath                                              NetworkPath,
                                                       DateTimeOffset?                                          ResponseTimestamp                        = null,
                                                       CustomBinaryParserDelegate<SecureDataTransferResponse>?  CustomSecureDataTransferResponseParser   = null,
                                                       CustomBinaryParserDelegate<Signature>?                   CustomBinarySignatureParser              = null)
        {

            if (TryParse(Request,
                         SecureData,
                         Destination,
                         NetworkPath,
                         out var secureDataTransferResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSecureDataTransferResponseParser,
                         CustomBinarySignatureParser))
            {
                return secureDataTransferResponse;
            }

            throw new ArgumentException("The given binary representation of a SecureDataTransfer response is invalid: " + errorResponse,
                                        nameof(SecureData));

        }

        #endregion

        #region (static) TryParse(Request, SecureData, out SecureDataTransferResponse, out ErrorResponse, CustomSecureDataTransferResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SecureDataTransfer response.
        /// </summary>
        /// <param name="Request">The SecureDataTransfer request leading to this response.</param>
        /// <param name="SecureData">The binary to be parsed.</param>
        /// <param name="SecureDataTransferResponse">The parsed SecureDataTransfer response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSecureDataTransferResponseParser">An optional delegate to parse custom SecureDataTransfer responses.</param>
        /// <param name="CustomBinarySignatureParser">An optional delegate to parse custom binary signatures.</param>
        public static Boolean TryParse(SecureDataTransferRequest                                Request,
                                       Byte[]                                                   SecureData,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out SecureDataTransferResponse?     SecureDataTransferResponse,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTimeOffset?                                          ResponseTimestamp                        = null,
                                       CustomBinaryParserDelegate<SecureDataTransferResponse>?  CustomSecureDataTransferResponseParser   = null,
                                       CustomBinaryParserDelegate<Signature>?                   CustomBinarySignatureParser              = null)
        {

            try
            {

                SecureDataTransferResponse      = null;

                var stream                      = new MemoryStream(SecureData);

                var status                      = SecureDataTransferStatus.TryParse(stream.ReadUInt16());

                var additionalStatusInfoLength  = stream.ReadUInt32();
                var additionalStatusInfo        = additionalStatusInfoLength > 0
                                                      ? stream.ReadBytes(additionalStatusInfoLength)?.ToUTF8String()?.Trim()
                                                      : null;

                var parameter                   = stream.ReadUInt16();
                var keyId                       = stream.ReadUInt16();
                var nonce                       = stream.ReadUInt64();
                var counter                     = stream.ReadUInt64();

                var ciphertextLength            = stream.ReadUInt64();
                var ciphertext                  = ciphertextLength > 0
                                                      ? stream.ReadBytes(ciphertextLength)
                                                      : [];

                #region Signatures

                var signatures            = new HashSet<Signature>();
                var signaturesCount       = stream.ReadByte();
                for (var i = 0; i < signaturesCount; i++)
                {

                    var signatureLength   = stream.ReadUInt16();
                    var signatureBytes    = stream.ReadBytes((UInt64) signatureLength);

                    if (!Signature.TryParse(signatureBytes, out var signature, out ErrorResponse))
                        return false;

                    signatures.Add(signature);

                }

                #endregion


                SecureDataTransferResponse      = new SecureDataTransferResponse(

                                                      Request,
                                                      status,
                                                      additionalStatusInfo,
                                                      parameter,
                                                      keyId,
                                                      nonce,
                                                      counter,
                                                      ciphertext,

                                                      null,
                                                      ResponseTimestamp,

                                                      Destination,
                                                      NetworkPath,

                                                      null,
                                                      null,
                                                      signatures

                                                  );


                ErrorResponse = null;

                if (CustomSecureDataTransferResponseParser is not null)
                    SecureDataTransferResponse = CustomSecureDataTransferResponseParser(SecureData,
                                                                                        SecureDataTransferResponse);

                return true;

            }
            catch (Exception e)
            {
                SecureDataTransferResponse  = null;
                ErrorResponse               = "The given binary representation of a SecureDataTransfer response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToBinary(CustomSecureDataTransferResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a binary representation of this object.
        /// </summary>
        /// <param name="CustomSecureDataTransferResponseSerializer">A delegate to serialize custom SecureDataTransfer responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="IncludeSignatures">Whether to include the digital signatures (default), or not.</param>
        public Byte[] ToBinary(CustomBinarySerializerDelegate<SecureDataTransferResponse>?  CustomSecureDataTransferResponseSerializer   = null,
                               CustomBinarySerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                               Boolean                                                      IncludeSignatures                            = true)
        {

            var binaryStream = new MemoryStream();

            binaryStream.WriteUInt16(Status.NumericId);
            binaryStream.WriteUInt32((UInt16) (AdditionalStatusInfo?.Length ?? 0));

            if (AdditionalStatusInfo.IsNotNullOrEmpty())
                binaryStream.Write(AdditionalStatusInfo.ToUTF8Bytes());

            binaryStream.WriteUInt16(Parameter ?? 0);
            binaryStream.WriteUInt16(KeyId     ?? 0);
            binaryStream.WriteUInt64(Nonce     ?? 0);
            binaryStream.WriteUInt64(Counter   ?? 0);

            var ciphertextLength = (UInt64) (Ciphertext?.LongLength ?? 0);
            binaryStream.WriteUInt64(ciphertextLength);
            if (ciphertextLength > 0)
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

            return CustomSecureDataTransferResponseSerializer is not null
                       ? CustomSecureDataTransferResponseSerializer(this, binary)
                       : binary;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SecureDataTransfer failed because of a request error.
        /// </summary>
        /// <param name="Request">The SecureDataTransfer request.</param>
        public static SecureDataTransferResponse RequestError(SecureDataTransferRequest  Request,
                                                              EventTracking_Id           EventTrackingId,
                                                              ResultCode                 ErrorCode,
                                                              String?                    ErrorDescription    = null,
                                                              JObject?                   ErrorDetails        = null,
                                                              DateTimeOffset?            ResponseTimestamp   = null,

                                                              SourceRouting?             Destination         = null,
                                                              NetworkPath?               NetworkPath         = null,

                                                              IEnumerable<KeyPair>?      SignKeys            = null,
                                                              IEnumerable<SignInfo>?     SignInfos           = null,
                                                              IEnumerable<Signature>?    Signatures          = null)

            => new (

                   Request,
                   SecureDataTransferStatus.Rejected,
                   null,
                   null,
                   null,
                   null,
                   null,
                   null,
                  OCPPv2_1.Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures

               );


        /// <summary>
        /// The SecureDataTransfer failed.
        /// </summary>
        /// <param name="Request">The SecureDataTransfer request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SecureDataTransferResponse FormationViolation(SecureDataTransferRequest  Request,
                                                                    String                     ErrorDescription)

            => new (Request,
                    SecureDataTransferStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The SecureDataTransfer failed.
        /// </summary>
        /// <param name="Request">The SecureDataTransfer request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SecureDataTransferResponse SignatureError(SecureDataTransferRequest  Request,
                                                                String                     ErrorDescription)

            => new (Request,
                    SecureDataTransferStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The SecureDataTransfer failed.
        /// </summary>
        /// <param name="Request">The SecureDataTransfer request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SecureDataTransferResponse Failed(SecureDataTransferRequest  Request,
                                                        String?                    Description   = null)

            => new (Request,
                    SecureDataTransferStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The SecureDataTransfer failed because of an exception.
        /// </summary>
        /// <param name="Request">The SecureDataTransfer request.</param>
        /// <param name="Exception">The exception.</param>
        public static SecureDataTransferResponse ExceptionOccurred(SecureDataTransferRequest  Request,
                                                                  Exception                  Exception)

            => new (Request,
                    SecureDataTransferStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SecureDataTransferResponse1, SecureDataTransferResponse2)

        /// <summary>
        /// Compares two SecureDataTransfer responses for equality.
        /// </summary>
        /// <param name="SecureDataTransferResponse1">A SecureDataTransfer response.</param>
        /// <param name="SecureDataTransferResponse2">Another SecureDataTransfer response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SecureDataTransferResponse? SecureDataTransferResponse1,
                                           SecureDataTransferResponse? SecureDataTransferResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SecureDataTransferResponse1, SecureDataTransferResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SecureDataTransferResponse1 is null || SecureDataTransferResponse2 is null)
                return false;

            return SecureDataTransferResponse1.Equals(SecureDataTransferResponse2);

        }

        #endregion

        #region Operator != (SecureDataTransferResponse1, SecureDataTransferResponse2)

        /// <summary>
        /// Compares two SecureDataTransfer responses for inequality.
        /// </summary>
        /// <param name="SecureDataTransferResponse1">A SecureDataTransfer response.</param>
        /// <param name="SecureDataTransferResponse2">Another SecureDataTransfer response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SecureDataTransferResponse? SecureDataTransferResponse1,
                                           SecureDataTransferResponse? SecureDataTransferResponse2)

            => !(SecureDataTransferResponse1 == SecureDataTransferResponse2);

        #endregion

        #endregion

        #region IEquatable<SecureDataTransferResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SecureDataTransfer responses for equality.
        /// </summary>
        /// <param name="Object">A SecureDataTransfer response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SecureDataTransferResponse secureDataTransferResponse &&
                   Equals(secureDataTransferResponse);

        #endregion

        #region Equals(SecureDataTransferResponse)

        /// <summary>
        /// Compares two SecureDataTransfer responses for equality.
        /// </summary>
        /// <param name="SecureDataTransferResponse">A SecureDataTransfer response to compare with.</param>
        public override Boolean Equals(SecureDataTransferResponse? SecureDataTransferResponse)

            => SecureDataTransferResponse is not null &&

               Status.     Equals(SecureDataTransferResponse.Status) &&

             ((AdditionalStatusInfo is     null && SecureDataTransferResponse.AdditionalStatusInfo is     null) ||
               AdditionalStatusInfo is not null && SecureDataTransferResponse.AdditionalStatusInfo is not null && AdditionalStatusInfo.Equals(SecureDataTransferResponse.AdditionalStatusInfo)) &&

             //((Data                 is     null && SecureDataTransferResponse.Data                 is     null) ||
             // (Data                 is not null && SecureDataTransferResponse.Data                 is not null && Data.                Equals(SecureDataTransferResponse.Data)))                &&

               base.GenericEquals(SecureDataTransferResponse);

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

            => String.Concat(

                   Status,

                   AdditionalStatusInfo.IsNullOrEmpty()
                       ? ""
                       : $" ({AdditionalStatusInfo})",

                   $"{KeyId} / {Counter}"

               );

        #endregion

    }

}
