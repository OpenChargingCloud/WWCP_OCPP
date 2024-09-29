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

using Newtonsoft.Json.Linq;

using System.Diagnostics.CodeAnalysis;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A binary data transfer response.
    /// </summary>
    public class BinaryDataTransferResponse : AResponse<BinaryDataTransferRequest,
                                                        BinaryDataTransferResponse>,
                                              IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/binaryDataTransferResponse");

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
        public BinaryDataTransferStatus  Status                  { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public String?                   AdditionalStatusInfo    { get; }

        /// <summary>
        /// Optional response binary data.
        /// </summary>
        [Optional]
        public Byte[]?                   Data                    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new binary data transfer response.
        /// </summary>
        /// <param name="Request">The BinaryDataTransfer request leading to this response.</param>
        /// <param name="Status">The success or failure status of the binary data transfer.</param>
        /// <param name="AdditionalStatusInfo">Optional detailed status information.</param>
        /// <param name="Data">A vendor-specific JSON token.</param>
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
        public BinaryDataTransferResponse(BinaryDataTransferRequest  Request,
                                          BinaryDataTransferStatus   Status,
                                          String?                    AdditionalStatusInfo   = null,
                                          Byte[]?                    Data                   = null,

                                          Result?                    Result                 = null,
                                          DateTime?                  ResponseTimestamp      = null,

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
            this.Data                  = Data;

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, Binary, CustomBinaryDataTransferResponseSerializer = null)

        /// <summary>
        /// Parse the given JSON representation of a binary data transfer response.
        /// </summary>
        /// <param name="Request">The BinaryDataTransfer request leading to this response.</param>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomBinaryDataTransferResponseParser">An optional delegate to parse custom binary data transfer responses.</param>
        public static BinaryDataTransferResponse Parse(BinaryDataTransferRequest                                Request,
                                                       Byte[]                                                   Binary,
                                                       SourceRouting                                            Destination,
                                                       NetworkPath                                              NetworkPath,
                                                       DateTime?                                                ResponseTimestamp                        = null,
                                                       CustomBinaryParserDelegate<BinaryDataTransferResponse>?  CustomBinaryDataTransferResponseParser   = null,
                                                       CustomJObjectParserDelegate<StatusInfo>?                 CustomStatusInfoParser                   = null,
                                                       CustomBinaryParserDelegate<Signature>?                   CustomBinarySignatureParser              = null)
        {

            if (TryParse(Request,
                         Binary,
                     Destination,
                         NetworkPath,
                         out var binaryDataTransferResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomBinaryDataTransferResponseParser,
                         CustomStatusInfoParser,
                         CustomBinarySignatureParser))
            {
                return binaryDataTransferResponse;
            }

            throw new ArgumentException("The given binary representation of a binary data transfer response is invalid: " + errorResponse,
                                        nameof(Binary));

        }

        #endregion

        #region (static) TryParse(Request, Binary, out BinaryDataTransferResponse, out ErrorResponse, CustomBinaryDataTransferResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a binary data transfer response.
        /// </summary>
        /// <param name="Request">The BinaryDataTransfer request leading to this response.</param>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="AuthorizeResponse">The parsed authorize response.</param>
        /// <param name="BinaryDataTransferResponse">The parsed binary data transfer response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomBinaryDataTransferResponseParser">An optional delegate to parse custom binary data transfer responses.</param>
        public static Boolean TryParse(BinaryDataTransferRequest                                Request,
                                       Byte[]                                                   Binary,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out BinaryDataTransferResponse?     BinaryDataTransferResponse,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                ResponseTimestamp                        = null,
                                       CustomBinaryParserDelegate<BinaryDataTransferResponse>?  CustomBinaryDataTransferResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                 CustomStatusInfoParser                   = null,
                                       CustomBinaryParserDelegate<Signature>?                   CustomBinarySignatureParser              = null)
        {

            try
            {

                BinaryDataTransferResponse = null;

                var stream  = new MemoryStream(Binary);
                var format  = SerializationFormatsExtensions.Parse(stream.ReadUInt16());

                switch (format)
                {

                    case SerializationFormats.BinaryCompact:
                        {

                            var binaryDataTransferStatusByte    = stream.ReadByte();

                            if (!BinaryDataTransferStatus.TryParse(binaryDataTransferStatusByte.ToString(), out var binaryDataTransferStatus))
                            {
                                ErrorResponse = $"The received binary data transfer status '{binaryDataTransferStatusByte}' is invalid!";
                                return false;
                            }

                            String? additionalStatusInfo = null;
                            var additionalStatusInfoLength      = stream.ReadUInt16();
                            if (additionalStatusInfoLength > 0)
                                additionalStatusInfo = stream.ReadUTF8String(additionalStatusInfoLength);

                            var dataLength                      = stream.ReadUInt64();
                            var data                            = stream.ReadBytes(dataLength);


                            BinaryDataTransferResponse = new BinaryDataTransferResponse(

                                                             Request,
                                                             binaryDataTransferStatus,
                                                             additionalStatusInfo,
                                                             data,

                                                             null,
                                                             ResponseTimestamp,

                                                         Destination,
                                                             NetworkPath,

                                                             null,
                                                             null,
                                                             null, //Signatures

                                                             format

                                                         );

                        }
                        break;

                    case SerializationFormats.BinaryTextIds:
                        {

                            var binaryDataTransferStatusLength  = stream.ReadUInt16();
                            var binaryDataTransferStatusText    = stream.ReadUTF8String(binaryDataTransferStatusLength);

                            if (!BinaryDataTransferStatus.TryParse(binaryDataTransferStatusText, out var binaryDataTransferStatus))
                            {
                                ErrorResponse = $"The received binary data transfer status '{binaryDataTransferStatusText}' is invalid!";
                                return false;
                            }

                            String? additionalStatusInfo = null;
                            var additionalStatusInfoLength      = stream.ReadUInt16();
                            if (additionalStatusInfoLength > 0)
                                additionalStatusInfo = stream.ReadUTF8String(additionalStatusInfoLength);

                            var dataLength                      = stream.ReadUInt64();
                            var data                            = stream.ReadBytes(dataLength);


                            BinaryDataTransferResponse = new BinaryDataTransferResponse(

                                                             Request,
                                                             binaryDataTransferStatus,
                                                             additionalStatusInfo,
                                                             data,

                                                             null,
                                                             ResponseTimestamp,

                                                         Destination,
                                                             NetworkPath,

                                                             null,
                                                             null,
                                                             null, //Signatures

                                                             format

                                                         );

                        }
                        break;

                }

                ErrorResponse = null;

                if (CustomBinaryDataTransferResponseParser is not null)
                    BinaryDataTransferResponse = CustomBinaryDataTransferResponseParser(Binary,
                                                                                        BinaryDataTransferResponse);

                return true;

            }
            catch (Exception e)
            {
                BinaryDataTransferResponse  = null;
                ErrorResponse               = "The given binary representation of a binary data transfer response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToBinary(CustomBinaryDataTransferResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a binary representation of this object.
        /// </summary>
        /// <param name="CustomBinaryDataTransferResponseSerializer">A delegate to serialize custom binary data transfer responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomBinarySignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="IncludeSignatures">Whether to include the digital signatures (default), or not.</param>
        public Byte[] ToBinary(CustomBinarySerializerDelegate<BinaryDataTransferResponse>?  CustomBinaryDataTransferResponseSerializer   = null,
                               CustomJObjectSerializerDelegate<StatusInfo>?                 CustomStatusInfoSerializer                   = null,
                               CustomBinarySerializerDelegate<Signature>?                   CustomBinarySignatureSerializer              = null,
                               Boolean                                                      IncludeSignatures                            = true)
        {

            var binaryStream = new MemoryStream();

            binaryStream.Write(SerializationFormat.AsBytes(), 0, 2);

            switch (SerializationFormat)
            {

                case SerializationFormats.BinaryCompact: {

                    binaryStream.WriteByte(Status.ToString().ToUTF8Bytes()[0]);

                    var AdditionalStatusInfoBytes  = AdditionalStatusInfo?.ToUTF8Bytes() ?? [];
                    binaryStream.WriteUInt16((UInt16) AdditionalStatusInfoBytes.Length);
                    if (AdditionalStatusInfoBytes.Length > 0)
                        binaryStream.Write(AdditionalStatusInfoBytes,  0, AdditionalStatusInfoBytes.Length);

                    var data = Data                                                      ?? [];
                    binaryStream.WriteUInt64((UInt64) data.LongLength);
                    binaryStream.Write(data,                           0, data.                     Length);

                    var signaturesCount = (UInt16) (IncludeSignatures ? Signatures.Count() : 0);
                    binaryStream.Write(BitConverter.GetBytes(signaturesCount),            0, 2);

                    if (IncludeSignatures) {
                        foreach (var signature in Signatures)
                        {
                            var binarySignature = signature.ToBinary();
                            binaryStream.Write(BitConverter.GetBytes((UInt16) binarySignature.Length));
                            binaryStream.Write(binarySignature);
                        }
                    }

                }
                break;

                case SerializationFormats.BinaryTextIds: {

                    var statusBytes                = Status.    ToString().ToUTF8Bytes();
                    binaryStream.WriteUInt16((UInt16) statusBytes.              Length);
                    binaryStream.Write(statusBytes,                    0, statusBytes.              Length);

                    var AdditionalStatusInfoBytes  = AdditionalStatusInfo?.ToUTF8Bytes() ?? [];
                    binaryStream.WriteUInt16((UInt16) AdditionalStatusInfoBytes.Length);
                    if (AdditionalStatusInfoBytes.Length > 0)
                        binaryStream.Write(AdditionalStatusInfoBytes,  0, AdditionalStatusInfoBytes.Length);

                    var data = Data                                                      ?? [];
                    binaryStream.WriteUInt64((UInt64) data.LongLength);
                    binaryStream.Write(data,                           0, data.                     Length);

                    var signaturesCount = (UInt16) (IncludeSignatures ? Signatures.Count() : 0);
                    binaryStream.Write(BitConverter.GetBytes(signaturesCount),            0, 2);

                    if (IncludeSignatures) {
                        foreach (var signature in Signatures)
                        {
                            var binarySignature = signature.ToBinary();
                            binaryStream.Write(BitConverter.GetBytes((UInt16) binarySignature.Length));
                            binaryStream.Write(binarySignature);
                        }
                    }

                }
                break;

                case SerializationFormats.BinaryTLV: {

                    var data = Data                                          ?? [];
                    binaryStream.Write(BitConverter.GetBytes((UInt16) BinaryTags.Data),       0, 2);
                    binaryStream.Write(BitConverter.GetBytes((UInt16) 0),                     0, 2);
                    binaryStream.Write(BitConverter.GetBytes((UInt32) data.Length),           0, 4);
                    binaryStream.Write(data,                                                  0, data.          Length);

                    var signaturesCount = (UInt16) (IncludeSignatures ? Signatures.Count() : 0);
                    binaryStream.Write(BitConverter.GetBytes(signaturesCount),            0, 2);

                    if (signaturesCount > 0)
                    {
                        
                    }

                }
                break;

            }

            var binary = binaryStream.ToArray();

            return CustomBinaryDataTransferResponseSerializer is not null
                       ? CustomBinaryDataTransferResponseSerializer(this, binary)
                       : binary;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The BinaryDataTransfer failed because of a request error.
        /// </summary>
        /// <param name="Request">The BinaryDataTransfer request.</param>
        public static BinaryDataTransferResponse RequestError(BinaryDataTransferRequest  Request,
                                                              EventTracking_Id           EventTrackingId,
                                                              ResultCode                 ErrorCode,
                                                              String?                    ErrorDescription    = null,
                                                              JObject?                   ErrorDetails        = null,
                                                              DateTime?                  ResponseTimestamp   = null,

                                                              SourceRouting?             Destination         = null,
                                                              NetworkPath?               NetworkPath         = null,

                                                              IEnumerable<KeyPair>?      SignKeys            = null,
                                                              IEnumerable<SignInfo>?     SignInfos           = null,
                                                              IEnumerable<Signature>?    Signatures          = null)

            => new (

                   Request,
                   BinaryDataTransferStatus.Rejected,
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
        /// The BinaryDataTransfer failed.
        /// </summary>
        /// <param name="Request">The BinaryDataTransfer request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static BinaryDataTransferResponse FormationViolation(BinaryDataTransferRequest  Request,
                                                                    String                     ErrorDescription)

            => new (Request,
                    BinaryDataTransferStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The BinaryDataTransfer failed.
        /// </summary>
        /// <param name="Request">The BinaryDataTransfer request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static BinaryDataTransferResponse SignatureError(BinaryDataTransferRequest  Request,
                                                                String                     ErrorDescription)

            => new (Request,
                    BinaryDataTransferStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The BinaryDataTransfer failed.
        /// </summary>
        /// <param name="Request">The BinaryDataTransfer request.</param>
        /// <param name="Description">An optional error description.</param>
        public static BinaryDataTransferResponse Failed(BinaryDataTransferRequest  Request,
                                                        String?                    Description   = null)

            => new (Request,
                    BinaryDataTransferStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The BinaryDataTransfer failed because of an exception.
        /// </summary>
        /// <param name="Request">The BinaryDataTransfer request.</param>
        /// <param name="Exception">The exception.</param>
        public static BinaryDataTransferResponse ExceptionOccured(BinaryDataTransferRequest  Request,
                                                                  Exception                  Exception)

            => new (Request,
                    BinaryDataTransferStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (BinaryDataTransferResponse1, BinaryDataTransferResponse2)

        /// <summary>
        /// Compares two binary data transfer responses for equality.
        /// </summary>
        /// <param name="BinaryDataTransferResponse1">A binary data transfer response.</param>
        /// <param name="BinaryDataTransferResponse2">Another binary data transfer response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (BinaryDataTransferResponse? BinaryDataTransferResponse1,
                                           BinaryDataTransferResponse? BinaryDataTransferResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(BinaryDataTransferResponse1, BinaryDataTransferResponse2))
                return true;

            // If one is null, but not both, return false.
            if (BinaryDataTransferResponse1 is null || BinaryDataTransferResponse2 is null)
                return false;

            return BinaryDataTransferResponse1.Equals(BinaryDataTransferResponse2);

        }

        #endregion

        #region Operator != (BinaryDataTransferResponse1, BinaryDataTransferResponse2)

        /// <summary>
        /// Compares two binary data transfer responses for inequality.
        /// </summary>
        /// <param name="BinaryDataTransferResponse1">A binary data transfer response.</param>
        /// <param name="BinaryDataTransferResponse2">Another binary data transfer response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (BinaryDataTransferResponse? BinaryDataTransferResponse1,
                                           BinaryDataTransferResponse? BinaryDataTransferResponse2)

            => !(BinaryDataTransferResponse1 == BinaryDataTransferResponse2);

        #endregion

        #endregion

        #region IEquatable<BinaryDataTransferResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two binary data transfer responses for equality.
        /// </summary>
        /// <param name="Object">A binary data transfer response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BinaryDataTransferResponse binaryDataTransferResponse &&
                   Equals(binaryDataTransferResponse);

        #endregion

        #region Equals(BinaryDataTransferResponse)

        /// <summary>
        /// Compares two binary data transfer responses for equality.
        /// </summary>
        /// <param name="BinaryDataTransferResponse">A binary data transfer response to compare with.</param>
        public override Boolean Equals(BinaryDataTransferResponse? BinaryDataTransferResponse)

            => BinaryDataTransferResponse is not null &&

               Status.     Equals(BinaryDataTransferResponse.Status) &&

             ((AdditionalStatusInfo is     null && BinaryDataTransferResponse.AdditionalStatusInfo is     null) ||
               AdditionalStatusInfo is not null && BinaryDataTransferResponse.AdditionalStatusInfo is not null && AdditionalStatusInfo.Equals(BinaryDataTransferResponse.AdditionalStatusInfo)) &&

             ((Data                 is     null && BinaryDataTransferResponse.Data                 is     null) ||
              (Data                 is not null && BinaryDataTransferResponse.Data                 is not null && Data.                Equals(BinaryDataTransferResponse.Data)))                &&

               base.GenericEquals(BinaryDataTransferResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Status.               GetHashCode()       * 7 ^
                      (AdditionalStatusInfo?.GetHashCode() ?? 0) * 5 ^
                      (Data?.                GetHashCode() ?? 0) * 3 ^
                       base.                 GetHashCode();

            }
        }

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

                   Data?.Length > 0
                       ? $": '{Data.ToBase64().SubstringMax(100)}' [{Data.Length} bytes]"
                       : ""

               );

        #endregion

    }

}
