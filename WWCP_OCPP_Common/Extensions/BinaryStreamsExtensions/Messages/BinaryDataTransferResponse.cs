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
    public class BinaryDataTransferResponse : AResponse<BinaryDataTransferRequest,
                                                        BinaryDataTransferResponse>,
                                              IResponse
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

        /// <summary>
        /// The binary format of the given message.
        /// </summary>
        [Mandatory]
        public BinaryFormats             Format                  { get; }

        #endregion

        #region Constructor(s)

        #region BinaryDataTransferResponse(Request, Status, AdditionalStatusInfo = null, BinaryData = null, ...)

        /// <summary>
        /// Create a new binary data transfer response.
        /// </summary>
        /// <param name="Request">The BinaryDataTransfer request leading to this response.</param>
        /// <param name="Status">The success or failure status of the binary data transfer.</param>
        /// <param name="AdditionalStatusInfo">Optional detailed status information.</param>
        /// <param name="Data">A vendor-specific JSON token.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        public BinaryDataTransferResponse(BinaryDataTransferRequest  Request,
                                          BinaryDataTransferStatus   Status,
                                          String?                    AdditionalStatusInfo   = null,
                                          Byte[]?                    Data                   = null,
                                          BinaryFormats?             Format                 = null,
                                          DateTime?                  ResponseTimestamp      = null,

                                          IEnumerable<KeyPair>?      SignKeys               = null,
                                          IEnumerable<SignInfo>?     SignInfos              = null,
                                          IEnumerable<Signature>?    Signatures             = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures)

        {

            this.Status                = Status;
            this.AdditionalStatusInfo  = AdditionalStatusInfo;
            this.Data                  = Data;
            this.Format                = Format ?? BinaryFormats.TextIds;

        }

        #endregion

        #region BinaryDataTransferResponse(Request, Result)

        /// <summary>
        /// Create a new binary data transfer response.
        /// </summary>
        /// <param name="Request">The BinaryDataTransfer request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public BinaryDataTransferResponse(BinaryDataTransferRequest  Request,
                                          Result                     Result)

            : base(Request,
                   Result)

        {

            this.Status = BinaryDataTransferStatus.Rejected;

        }

        #endregion

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
        /// <param name="CustomBinaryDataTransferResponseParser">A delegate to parse custom binary data transfer responses.</param>
        public static BinaryDataTransferResponse Parse(BinaryDataTransferRequest                                Request,
                                                       Byte[]                                                   Binary,
                                                       CustomBinaryParserDelegate<BinaryDataTransferResponse>?  CustomBinaryDataTransferResponseParser  = null)
        {

            if (TryParse(Request,
                         Binary,
                         out var binaryDataTransferResponse,
                         out var errorResponse,
                         CustomBinaryDataTransferResponseParser))
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
        /// <param name="BinaryDataTransferResponse">The parsed binary data transfer response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomBinaryDataTransferResponseParser">A delegate to parse custom binary data transfer responses.</param>
        public static Boolean TryParse(BinaryDataTransferRequest                                Request,
                                       Byte[]                                                   Binary,
                                       [NotNullWhen(true)]  out BinaryDataTransferResponse?     BinaryDataTransferResponse,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       CustomBinaryParserDelegate<BinaryDataTransferResponse>?  CustomBinaryDataTransferResponseParser   = null)
        {

            try
            {

                BinaryDataTransferResponse = null;

                var stream  = new MemoryStream(Binary);
                var format  = BinaryFormatsExtensions.Parse(stream.ReadUInt16());

                switch (format)
                {

                    case BinaryFormats.Compact:
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
                                                             format,

                                                             null,
                                                             null,
                                                             null //Signatures

                                                         );

                        }
                        break;

                    case BinaryFormats.TextIds:
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
                                                             format,

                                                             null,
                                                             null,
                                                             null //Signatures

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

            binaryStream.Write(Format.AsBytes(), 0, 2);

            switch (Format)
            {

                case BinaryFormats.Compact: {

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

                case BinaryFormats.TextIds: {

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

                case BinaryFormats.TagLengthValue: {

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
        /// The binary data transfer failed.
        /// </summary>
        /// <param name="Request">The BinaryDataTransfer request leading to this response.</param>
        public static BinaryDataTransferResponse Failed(BinaryDataTransferRequest  Request)

            => new (Request,
                    Result.Server());

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
