/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
 * This file is part of WWCP <https://github.com/OpenChargingCloud/WWCP_OCPP>
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
    /// A binary data transfer response.
    /// </summary>
    public class BinaryDataTransferResponse : AResponse<CS.BinaryDataTransferRequest,
                                                        BinaryDataTransferResponse>,
                                              IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/binaryDataTransferResponse");

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
        public BinaryDataTransferStatus  Status        { get; }

        /// <summary>
        /// Optional response binary data.
        /// </summary>
        [Optional]
        public Byte[]?                   Data          { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?               StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region BinaryDataTransferResponse(Request, Status, BinaryData = null, StatusInfo = null, ...)

        /// <summary>
        /// Create a new binary data transfer response.
        /// </summary>
        /// <param name="Request">The binary data transfer request leading to this response.</param>
        /// <param name="Status">The success or failure status of the binary data transfer.</param>
        /// <param name="Data">A vendor-specific JSON token.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom binary data object to allow to store any kind of customer specific binary data.</param>
        public BinaryDataTransferResponse(CS.BinaryDataTransferRequest  Request,
                                          BinaryDataTransferStatus      Status,
                                          Byte[]?                       Data                = null,
                                          StatusInfo?                   StatusInfo          = null,
                                          DateTime?                     ResponseTimestamp   = null,

                                          IEnumerable<KeyPair>?         SignKeys            = null,
                                          IEnumerable<SignInfo>?        SignInfos           = null,
                                          IEnumerable<Signature>?       Signatures          = null,

                                          CustomData?                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status      = Status;
            this.Data        = Data;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region BinaryDataTransferResponse(Request, Result)

        /// <summary>
        /// Create a new binary data transfer response.
        /// </summary>
        /// <param name="Request">The binary data transfer request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public BinaryDataTransferResponse(CS.BinaryDataTransferRequest  Request,
                                    Result                  Result)

            : base(Request,
                   Result)

        {

            this.Status = BinaryDataTransferStatus.Unknown;

        }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:BinaryDataTransferResponse",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom binary data.",
        //       "javaType": "CustomData",
        //       "type": "object",
        //       "properties": {
        //         "vendorId": {
        //           "type": "string",
        //           "maxLength": 255
        //         }
        //       },
        //       "required": [
        //         "vendorId"
        //       ]
        //     },
        //     "BinaryDataTransferStatusEnumType": {
        //       "description": "This indicates the success or failure of the binary data transfer.\r\n",
        //       "javaType": "BinaryDataTransferStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "UnknownMessageId",
        //         "UnknownVendorId"
        //       ]
        //     },
        //     "StatusInfoType": {
        //       "description": "Element providing more information about the status.\r\n",
        //       "javaType": "StatusInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customBinaryData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "reasonCode": {
        //           "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "additionalInfo": {
        //           "description": "Additional text to provide detailed information.\r\n",
        //           "type": "string",
        //           "maxLength": 512
        //         }
        //       },
        //       "required": [
        //         "reasonCode"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customBinaryData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "status": {
        //       "$ref": "#/definitions/BinaryDataTransferStatusEnumType"
        //     },
        //     "statusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     },
        //     "binary data": {
        //       "description": "BinaryData without specified length or format, in response to request.\r\n"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, Binary, CustomBinaryDataTransferResponseSerializer = null)

        /// <summary>
        /// Parse the given binary representation of a binary data transfer response.
        /// </summary>
        /// <param name="Request">The binary data transfer request leading to this response.</param>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="CustomBinaryDataTransferResponseParser">A delegate to parse custom binary data transfer responses.</param>
        public static BinaryDataTransferResponse Parse(CS.BinaryDataTransferRequest                             Request,
                                                       Byte[]                                                   Binary,
                                                       CustomBinaryParserDelegate<BinaryDataTransferResponse>?  CustomBinaryDataTransferResponseParser  = null)
        {

            if (TryParse(Request,
                         Binary,
                         out var binaryDataTransferResponse,
                         out var errorResponse,
                         CustomBinaryDataTransferResponseParser) &&
                binaryDataTransferResponse is not null)
            {
                return binaryDataTransferResponse;
            }

            throw new ArgumentException("The given binary representation of a binary data transfer response is invalid: " + errorResponse,
                                        nameof(Binary));

        }

        #endregion

        #region (static) TryParse(Request, Binary, out BinaryDataTransferResponse, out ErrorResponse, CustomDataTransferResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a binary data transfer response.
        /// </summary>
        /// <param name="Request">The binary data transfer request leading to this response.</param>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="BinaryDataTransferResponse">The parsed binary data transfer response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDataTransferResponseParser">A delegate to parse custom binary data transfer responses.</param>
        public static Boolean TryParse(CS.BinaryDataTransferRequest                             Request,
                                       Byte[]                                                   Binary,
                                       out BinaryDataTransferResponse?                          BinaryDataTransferResponse,
                                       out String?                                              ErrorResponse,
                                       CustomBinaryParserDelegate<BinaryDataTransferResponse>?  CustomBinaryDataTransferResponseParser   = null)
        {

            try
            {

                BinaryDataTransferResponse = null;

                //#region BinaryDataTransferStatus    [mandatory]

                //if (!JSON.ParseMandatory("status",
                //                         "binary data transfer status",
                //                         BinaryDataTransferStatusExtensions.TryParse,
                //                         out BinaryDataTransferStatus BinaryDataTransferStatus,
                //                         out ErrorResponse))
                //{
                //    return false;
                //}

                //#endregion

                //#region BinaryData                  [optional]

                //var BinaryData = JSON["binary data"];

                //#endregion

                //#region StatusInfo            [optional]

                //if (JSON.ParseOptionalJSON("statusInfo",
                //                           "detailed status info",
                //                           OCPPv2_1.StatusInfo.TryParse,
                //                           out StatusInfo? StatusInfo,
                //                           out ErrorResponse))
                //{
                //    if (ErrorResponse is not null)
                //        return false;
                //}

                //#endregion

                //#region Signatures            [optional, OCPP_CSE]

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

                //#region CustomData            [optional]

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


                //BinaryDataTransferResponse = new BinaryDataTransferResponse(
                //                                 Request,
                //                                 BinaryDataTransferStatus,
                //                                 null,
                //                                 StatusInfo,
                //                                 null,
                //                                 null,
                //                                 null,
                //                                 Signatures,
                //                                 CustomData
                //                             );

                ErrorResponse = null;

                if (CustomBinaryDataTransferResponseParser is not null)
                    BinaryDataTransferResponse = CustomBinaryDataTransferResponseParser(Binary,
                                                                                        BinaryDataTransferResponse);

                return true;

            }
            catch (Exception e)
            {
                BinaryDataTransferResponse  = null;
                ErrorResponse               = "The given JSON representation of a binary data transfer response is invalid: " + e.Message;
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
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public Byte[] ToJSON(CustomBinarySerializerDelegate<BinaryDataTransferResponse>?  CustomBinaryDataTransferResponseSerializer   = null,
                             CustomJObjectSerializerDelegate<StatusInfo>?                 CustomStatusInfoSerializer                   = null,
                             CustomJObjectSerializerDelegate<Signature>?                  CustomSignatureSerializer                    = null,
                             CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                   = null)
        {

            //var json = JSONObject.Create(

            //                     new JProperty("status",       Status.    AsText()),

            //               BinaryData is not null
            //                   ? new JProperty("binary data",         BinaryData)
            //                   : null,

            //               StatusInfo is not null
            //                   ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
            //                                                                     CustomCustomDataSerializer))
            //                   : null,

            //               Signatures.Any()
            //                   ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
            //                                                                                                              CustomCustomDataSerializer))))
            //                   : null,

            //               CustomData is not null
            //                   ? new JProperty("customBinaryData",   CustomData.ToJSON(CustomCustomDataSerializer))
            //                   : null

            //           );

            var binary = new Byte[0];

            return CustomBinaryDataTransferResponseSerializer is not null
                       ? CustomBinaryDataTransferResponseSerializer(this, binary)
                       : binary;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The binary data transfer failed.
        /// </summary>
        /// <param name="Request">The binary data transfer request leading to this response.</param>
        public static BinaryDataTransferResponse Failed(CS.BinaryDataTransferRequest  Request)

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

             ((Data       is     null && BinaryDataTransferResponse.Data       is     null) ||
              (Data       is not null && BinaryDataTransferResponse.Data       is not null && Data.      Equals(BinaryDataTransferResponse.Data)))      &&

             ((StatusInfo is     null && BinaryDataTransferResponse.StatusInfo is     null) ||
               StatusInfo is not null && BinaryDataTransferResponse.StatusInfo is not null && StatusInfo.Equals(BinaryDataTransferResponse.StatusInfo)) &&

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

                return Status.     GetHashCode()       * 7 ^
                      (Data?.      GetHashCode() ?? 0) * 5 ^
                      (StatusInfo?.GetHashCode() ?? 0) * 3 ^

                       base.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(
                   Status
               );

        #endregion


    }

}
