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

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// A binary data transfer response.
    /// </summary>
    public class SecureDataTransferResponse : AResponse<SecureDataTransferRequest,
                                                        SecureDataTransferResponse>,
                                              IResponse
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
        /// The success or failure status of the binary data transfer.
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

        #region SecureDataTransferResponse(Request, Status, AdditionalStatusInfo = null, SecureData = null, ...)

        /// <summary>
        /// Create a new binary data transfer response.
        /// </summary>
        /// <param name="Request">The SecureDataTransfer request leading to this response.</param>
        /// <param name="Status">The success or failure status of the binary data transfer.</param>
        /// <param name="AdditionalStatusInfo">Optional detailed status information.</param>
        /// <param name="Parameter">Optional encryption parameters.</param>
        /// <param name="KeyId">The optional unique identification of the encryption key.</param>
        /// <param name="Nonce">The optional first half of the cryptographic nonce.</param>
        /// <param name="Counter">The optional counter part of the cryptographic nonce.</param>
        /// <param name="Ciphertext">The encrypted encapsulated optional security payload.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        public SecureDataTransferResponse(SecureDataTransferRequest  Request,
                                          SecureDataTransferStatus   Status,
                                          String?                    AdditionalStatusInfo   = null,
                                          UInt16?                    Parameter              = null,
                                          UInt16?                    KeyId                  = null,
                                          UInt64?                    Nonce                  = null,
                                          UInt64?                    Counter                = null,
                                          Byte[]?                    Ciphertext             = null,
                                          DateTime?                  ResponseTimestamp      = null,

                                          NetworkingNode_Id?         DestinationNodeId      = null,
                                          NetworkPath?               NetworkPath            = null,

                                          IEnumerable<KeyPair>?      SignKeys               = null,
                                          IEnumerable<SignInfo>?     SignInfos              = null,
                                          IEnumerable<Signature>?    Signatures             = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   DestinationNodeId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures)

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

        #region SecureDataTransferResponse(Request, Result)

        /// <summary>
        /// Create a new binary data transfer response.
        /// </summary>
        /// <param name="Request">The SecureDataTransfer request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public SecureDataTransferResponse(SecureDataTransferRequest  Request,
                                          Result                     Result)

            : base(Request,
                   Result)

        {

            this.Status = SecureDataTransferStatus.Rejected;

        }

        #endregion

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, Secure, CustomSecureDataTransferResponseSerializer = null)

        /// <summary>
        /// Parse the given JSON representation of a binary data transfer response.
        /// </summary>
        /// <param name="Request">The SecureDataTransfer request leading to this response.</param>
        /// <param name="Secure">The binary to be parsed.</param>
        /// <param name="CustomSecureDataTransferResponseParser">A delegate to parse custom binary data transfer responses.</param>
        public static SecureDataTransferResponse Parse(SecureDataTransferRequest                                Request,
                                                       Byte[]                                                   Secure,
                                                       CustomBinaryParserDelegate<SecureDataTransferResponse>?  CustomSecureDataTransferResponseParser  = null)
        {

            if (TryParse(Request,
                         Secure,
                         out var secureDataTransferResponse,
                         out var errorResponse,
                         CustomSecureDataTransferResponseParser))
            {
                return secureDataTransferResponse;
            }

            throw new ArgumentException("The given binary representation of a binary data transfer response is invalid: " + errorResponse,
                                        nameof(Secure));

        }

        #endregion

        #region (static) TryParse(Request, Secure, out SecureDataTransferResponse, out ErrorResponse, CustomSecureDataTransferResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a binary data transfer response.
        /// </summary>
        /// <param name="Request">The SecureDataTransfer request leading to this response.</param>
        /// <param name="Secure">The binary to be parsed.</param>
        /// <param name="SecureDataTransferResponse">The parsed binary data transfer response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSecureDataTransferResponseParser">A delegate to parse custom binary data transfer responses.</param>
        public static Boolean TryParse(SecureDataTransferRequest                                Request,
                                       Byte[]                                                   Secure,
                                       [NotNullWhen(true)]  out SecureDataTransferResponse?     SecureDataTransferResponse,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       CustomBinaryParserDelegate<SecureDataTransferResponse>?  CustomSecureDataTransferResponseParser   = null)
        {

            try
            {

                SecureDataTransferResponse = null;

                var stream  = new MemoryStream(Secure);


                ErrorResponse = null;

                if (CustomSecureDataTransferResponseParser is not null)
                    SecureDataTransferResponse = CustomSecureDataTransferResponseParser(Secure,
                                                                                        SecureDataTransferResponse);

                return true;

            }
            catch (Exception e)
            {
                SecureDataTransferResponse  = null;
                ErrorResponse               = "The given binary representation of a binary data transfer response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToBinary(CustomSecureDataTransferResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a binary representation of this object.
        /// </summary>
        /// <param name="CustomSecureDataTransferResponseSerializer">A delegate to serialize custom binary data transfer responses.</param>
        /// <param name="CustomSecureSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="IncludeSignatures">Whether to include the digital signatures (default), or not.</param>
        public Byte[] ToBinary(CustomBinarySerializerDelegate<SecureDataTransferResponse>?  CustomSecureDataTransferResponseSerializer   = null,
                               CustomBinarySerializerDelegate<Signature>?                   CustomSecureSignatureSerializer              = null,
                               Boolean                                                      IncludeSignatures                            = true)
        {

            var binaryStream = new MemoryStream();

            var binary = binaryStream.ToArray();

            return CustomSecureDataTransferResponseSerializer is not null
                       ? CustomSecureDataTransferResponseSerializer(this, binary)
                       : binary;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SecureDataTransfer failed.
        /// </summary>
        /// <param name="Request">The SecureDataTransfer request.</param>
        /// <param name="Description">An optional error decription.</param>
        public static SecureDataTransferResponse Failed(SecureDataTransferRequest  Request,
                                                        String?                    Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The SecureDataTransfer failed because of an exception.
        /// </summary>
        /// <param name="Request">The SecureDataTransfer request.</param>
        /// <param name="Exception">The exception.</param>
        public static SecureDataTransferResponse ExceptionOccured(Exception  Exception)

            => new (null,
                    Result.FromException(Exception));


        /// <summary>
        /// The SecureDataTransfer failed because of an exception.
        /// </summary>
        /// <param name="Request">The SecureDataTransfer request.</param>
        /// <param name="Exception">The exception.</param>
        public static SecureDataTransferResponse ExceptionOccured(SecureDataTransferRequest  Request,
                                                                  Exception                  Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SecureDataTransferResponse1, SecureDataTransferResponse2)

        /// <summary>
        /// Compares two binary data transfer responses for equality.
        /// </summary>
        /// <param name="SecureDataTransferResponse1">A binary data transfer response.</param>
        /// <param name="SecureDataTransferResponse2">Another binary data transfer response.</param>
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
        /// Compares two binary data transfer responses for inequality.
        /// </summary>
        /// <param name="SecureDataTransferResponse1">A binary data transfer response.</param>
        /// <param name="SecureDataTransferResponse2">Another binary data transfer response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SecureDataTransferResponse? SecureDataTransferResponse1,
                                           SecureDataTransferResponse? SecureDataTransferResponse2)

            => !(SecureDataTransferResponse1 == SecureDataTransferResponse2);

        #endregion

        #endregion

        #region IEquatable<SecureDataTransferResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two binary data transfer responses for equality.
        /// </summary>
        /// <param name="Object">A binary data transfer response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SecureDataTransferResponse secureDataTransferResponse &&
                   Equals(secureDataTransferResponse);

        #endregion

        #region Equals(SecureDataTransferResponse)

        /// <summary>
        /// Compares two binary data transfer responses for equality.
        /// </summary>
        /// <param name="SecureDataTransferResponse">A binary data transfer response to compare with.</param>
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
