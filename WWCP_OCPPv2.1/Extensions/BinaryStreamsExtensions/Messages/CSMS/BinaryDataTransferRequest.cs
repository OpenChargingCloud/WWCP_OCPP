/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The binary data transfer request.
    /// </summary>
    public class BinaryDataTransferRequest : ARequest<BinaryDataTransferRequest>,
                                             IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/binaryDataTransferRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The vendor identification or namespace of the given message.
        /// </summary>
        [Mandatory]
        public Vendor_Id      VendorId     { get; }

        /// <summary>
        /// An optional message identification field.
        /// </summary>
        [Optional]
        public Message_Id?    MessageId    { get; }

        /// <summary>
        /// Optional message binary data without specified length or format.
        /// </summary>
        [Optional]
        public Byte[]?        Data         { get; }

        /// <summary>
        /// The binary format of the given message.
        /// </summary>
        [Mandatory]
        public BinaryFormats  Format       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new binary data transfer request.
        /// </summary>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification.</param>
        /// <param name="Data">Optional vendor-specific binary data.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom binary data object to allow to store any kind of customer specific binary data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public BinaryDataTransferRequest(ChargingStation_Id       ChargingStationId,
                                         Vendor_Id                VendorId,
                                         Message_Id?              MessageId           = null,
                                         Byte[]?                  Data                = null,
                                         BinaryFormats?           Format              = null,

                                         IEnumerable<KeyPair>?    SignKeys            = null,
                                         IEnumerable<SignInfo>?   SignInfos           = null,
                                         IEnumerable<Signature>?  Signatures          = null,

                                         CustomData?              CustomData          = null,

                                         Request_Id?              RequestId           = null,
                                         DateTime?                RequestTimestamp    = null,
                                         TimeSpan?                RequestTimeout      = null,
                                         EventTracking_Id?        EventTrackingId     = null,
                                         CancellationToken        CancellationToken   = default)

            : base(ChargingStationId,
                   "BinaryDataTransfer",

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.VendorId   = VendorId;
            this.MessageId  = MessageId;
            this.Data       = Data;
            this.Format     = Format ?? BinaryFormats.TextIds;


            unchecked
            {

                hashCode = this.VendorId.  GetHashCode()       * 11 ^
                          (this.MessageId?.GetHashCode() ?? 0) *  7 ^
                          (this.Data?.     GetHashCode() ?? 0) *  5 ^
                           this.Format.    GetHashCode()       *  3 ^
                           base.           GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Binary, RequestId, ChargingStationId, CustomDataTransferRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a binary data transfer request.
        /// </summary>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="CustomDataTransferRequestParser">A delegate to parse custom binary data transfer requests.</param>
        public static BinaryDataTransferRequest Parse(Byte[]                                                  Binary,
                                                      Request_Id                                              RequestId,
                                                      ChargingStation_Id                                      ChargingStationId,
                                                      CustomBinaryParserDelegate<BinaryDataTransferRequest>?  CustomDataTransferRequestParser   = null)
        {

            if (TryParse(Binary,
                         RequestId,
                         ChargingStationId,
                         out var binaryDataTransferRequest,
                         out var errorResponse,
                         CustomDataTransferRequestParser) &&
                binaryDataTransferRequest is not null)
            {
                return binaryDataTransferRequest;
            }

            throw new ArgumentException("The given binary representation of a binary data transfer request is invalid: " + errorResponse,
                                        nameof(Binary));

        }

        #endregion

        #region (static) TryParse(Binary, RequestId, ChargingStationId, out BinaryDataTransferRequest, OnException = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given binary representation of a binary data transfer request.
        /// </summary>
        /// <param name="Binary">The binary data to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="BinaryDataTransferRequest">The parsed BinaryDataTransfer request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(Byte[]                          Binary,
                                       Request_Id                      RequestId,
                                       ChargingStation_Id              ChargingStationId,
                                       out BinaryDataTransferRequest?  BinaryDataTransferRequest,
                                       out String?                     ErrorResponse)

            => TryParse(Binary,
                        RequestId,
                        ChargingStationId,
                        out BinaryDataTransferRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given binary representation of a binary data transfer request.
        /// </summary>
        /// <param name="Binary">The binary data to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="BinaryDataTransferRequest">The parsed BinaryDataTransfer request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDataTransferRequestParser">A delegate to parse custom BinaryDataTransfer requests.</param>
        public static Boolean TryParse(Byte[]                                                  Binary,
                                       Request_Id                                              RequestId,
                                       ChargingStation_Id                                      ChargingStationId,
                                       out BinaryDataTransferRequest?                          BinaryDataTransferRequest,
                                       out String?                                             ErrorResponse,
                                       CustomBinaryParserDelegate<BinaryDataTransferRequest>?  CustomDataTransferRequestParser)
        {

            try
            {

                BinaryDataTransferRequest = null;

                //#region VendorId             [mandatory]

                //if (!JSON.ParseMandatory("vendorId",
                //                         "vendor identification",
                //                         Vendor_Id.TryParse,
                //                         out Vendor_Id VendorId,
                //                         out ErrorResponse))
                //{
                //    return false;
                //}

                //#endregion

                //#region MessageId            [optional]

                //var MessageId = JSON.GetString("messageId");

                //#endregion

                //#region BinaryData                 [optional]

                //var BinaryData = JSON["binary data"];

                //#endregion

                //#region Signatures           [optional, OCPP_CSE]

                //if (JSON.ParseOptionalHashSet("signatures",
                //                              "cryptographic signatures",
                //                              Signature.TryParse,
                //                              out HashSet<Signature> Signatures,
                //                              out ErrorResponse))
                //{
                //    if (ErrorResponse is not null)
                //        return false;
                //}

                //#endregion

                //#region CustomData           [optional]

                //if (JSON.ParseOptionalJSON("customBinaryData",
                //                           "custom binary data",
                //                           OCPPv2_1.CustomData.TryParse,
                //                           out CustomData CustomData,
                //                           out ErrorResponse))
                //{
                //    if (ErrorResponse is not null)
                //        return false;
                //}

                //#endregion

                //#region ChargingStationId    [optional, OCPP_CSE]

                //if (JSON.ParseOptional("chargingStationId",
                //                       "charging station identification",
                //                       ChargingStation_Id.TryParse,
                //                       out ChargingStation_Id? chargingStationId_PayLoad,
                //                       out ErrorResponse))
                //{

                //    if (ErrorResponse is not null)
                //        return false;

                //    if (chargingStationId_PayLoad.HasValue)
                //        ChargingStationId = chargingStationId_PayLoad.Value;

                //}

                //#endregion

                ErrorResponse = null;
                //BinaryDataTransferRequest = new BinaryDataTransferRequest(
                //                                ChargingStationId,
                //                                VendorId,
                //                                MessageId,
                //                                BinaryData,
                //                                null,
                //                                null,
                //                                Signatures,
                //                                CustomData,
                //                                RequestId
                //                            );

                if (CustomDataTransferRequestParser is not null)
                    BinaryDataTransferRequest = CustomDataTransferRequestParser(Binary,
                                                                                BinaryDataTransferRequest);

                return true;

            }
            catch (Exception e)
            {
                BinaryDataTransferRequest  = null;
                ErrorResponse              = "The given binary representation of a binary data transfer request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToBinary(CustomBinaryDataTransferRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a binary representation of this object.
        /// </summary>
        /// <param name="CustomBinaryDataTransferRequestSerializer">A delegate to serialize custom binary data transfer requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        /// <param name="IncludeSignatures">Whether to include the digital signatures (default), or not.</param>
        public Byte[] ToBinary(CustomBinarySerializerDelegate<BinaryDataTransferRequest>?  CustomBinaryDataTransferRequestSerializer   = null,
                               CustomJObjectSerializerDelegate<Signature>?                 CustomSignatureSerializer                   = null,
                               CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                  = null,
                               Boolean                                                     IncludeSignatures                           = true)
        {

            var binaryStream = new MemoryStream();

            binaryStream.Write(Format.AsBytes(), 0, 2);

            switch (Format)
            {

                case BinaryFormats.Compact: {

                    binaryStream.Write(BitConverter.GetBytes(VendorId.  NumericId));
                    binaryStream.Write(BitConverter.GetBytes(MessageId?.NumericId ?? 0));
                    binaryStream.Write(BitConverter.GetBytes((UInt32) (Data?.LongLength ?? 0)));

                    if (Data is not null)
                        binaryStream.Write(Data,                                          0, (Int32) (Data?.LongLength ?? 0));

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

                    var vendorIdBytes  = VendorId.  InternalId.ToUTF8Bytes();
                    binaryStream.WriteUInt16((UInt16) vendorIdBytes.Length);
                    binaryStream.Write(vendorIdBytes,       0, vendorIdBytes. Length);

                    var messageIdBytes = MessageId?.InternalId.ToUTF8Bytes() ?? [];
                    binaryStream.WriteUInt16((UInt16) messageIdBytes.Length);
                    if (messageIdBytes.Length > 0)
                        binaryStream.Write(messageIdBytes,  0, messageIdBytes.Length);

                    var data = Data                                          ?? [];
                    binaryStream.WriteUInt64((UInt64) data.          LongLength);
                    binaryStream.Write(data,                0, data.          Length); //ToDo: Fix me for >2 GB!

                    var signaturesCount = IncludeSignatures ? Signatures.Count() : 0;
                    binaryStream.WriteUInt16((UInt16) signaturesCount);

                    if (signaturesCount > 0)
                    {
                        
                    }

                }
                break;


                case BinaryFormats.TagLengthValue: {

                    var vendorIdBytes  = VendorId.  ToString().ToUTF8Bytes();
                    binaryStream.Write(BitConverter.GetBytes((UInt16) BinaryTags.VendorId),   0, 2);
                    binaryStream.Write(BitConverter.GetBytes((UInt16) vendorIdBytes.Length),  0, 2);
                    binaryStream.Write(vendorIdBytes,                                         0, vendorIdBytes. Length);

                    if (MessageId.HasValue) {
                        var messageIdBytes = MessageId?.ToString().ToUTF8Bytes() ?? [];
                        binaryStream.Write(BitConverter.GetBytes((UInt16) BinaryTags.MessageId),  0, 2);
                        binaryStream.Write(BitConverter.GetBytes((UInt16) messageIdBytes.Length), 0, 2);
                        binaryStream.Write(messageIdBytes,                                        0, messageIdBytes.Length);
                    }

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

            return CustomBinaryDataTransferRequestSerializer is not null
                       ? CustomBinaryDataTransferRequestSerializer(this, binary)
                       : binary;

        }

        #endregion


        #region Operator overloading

        #region Operator == (BinaryDataTransferRequest1, BinaryDataTransferRequest2)

        /// <summary>
        /// Compares two BinaryDataTransfer requests for equality.
        /// </summary>
        /// <param name="BinaryDataTransferRequest1">A BinaryDataTransfer request.</param>
        /// <param name="BinaryDataTransferRequest2">Another BinaryDataTransfer request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (BinaryDataTransferRequest? BinaryDataTransferRequest1,
                                           BinaryDataTransferRequest? BinaryDataTransferRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(BinaryDataTransferRequest1, BinaryDataTransferRequest2))
                return true;

            // If one is null, but not both, return false.
            if (BinaryDataTransferRequest1 is null || BinaryDataTransferRequest2 is null)
                return false;

            return BinaryDataTransferRequest1.Equals(BinaryDataTransferRequest2);

        }

        #endregion

        #region Operator != (BinaryDataTransferRequest1, BinaryDataTransferRequest2)

        /// <summary>
        /// Compares two BinaryDataTransfer requests for inequality.
        /// </summary>
        /// <param name="BinaryDataTransferRequest1">A BinaryDataTransfer request.</param>
        /// <param name="BinaryDataTransferRequest2">Another BinaryDataTransfer request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (BinaryDataTransferRequest? BinaryDataTransferRequest1,
                                           BinaryDataTransferRequest? BinaryDataTransferRequest2)

            => !(BinaryDataTransferRequest1 == BinaryDataTransferRequest2);

        #endregion

        #endregion

        #region IEquatable<BinaryDataTransferRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two binary data transfer requests for equality.
        /// </summary>
        /// <param name="Object">A binary data transfer request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BinaryDataTransferRequest binaryDataTransferRequest &&
                   Equals(binaryDataTransferRequest);

        #endregion

        #region Equals(BinaryDataTransferRequest)

        /// <summary>
        /// Compares two binary data transfer requests for equality.
        /// </summary>
        /// <param name="BinaryDataTransferRequest">A binary data transfer request to compare with.</param>
        public override Boolean Equals(BinaryDataTransferRequest? BinaryDataTransferRequest)

            => BinaryDataTransferRequest is not null               &&

               VendorId.Equals(BinaryDataTransferRequest.VendorId) &&

             ((MessageId is     null && BinaryDataTransferRequest.MessageId is     null) ||
              (MessageId is not null && BinaryDataTransferRequest.MessageId is not null && MessageId.Equals(BinaryDataTransferRequest.MessageId))) &&

             ((Data      is     null && BinaryDataTransferRequest.Data      is     null) ||
              (Data      is not null && BinaryDataTransferRequest.Data      is not null && Data.     Equals(BinaryDataTransferRequest.Data)))      &&

               base.GenericEquals(BinaryDataTransferRequest);

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

            => $"{VendorId}: {MessageId?.ToString() ?? "-"} => {Data?.ToBase64().SubstringMax(100) ?? "-"}";

        #endregion


    }

}
