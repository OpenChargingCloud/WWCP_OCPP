﻿/*
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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The PublishFirmware response.
    /// </summary>
    public class PublishFirmwareResponse : AResponse<PublishFirmwareRequest,
                                                     PublishFirmwareResponse>,
                                           IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/publishFirmwareResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the PublishFirmware request.
        /// </summary>
        [Mandatory]
        public GenericStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new PublishFirmware response.
        /// </summary>
        /// <param name="Request">The PublishFirmware request leading to this response.</param>
        /// <param name="Status">The success or failure of the PublishFirmware request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
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
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public PublishFirmwareResponse(PublishFirmwareRequest   Request,
                                       GenericStatus            Status,
                                       StatusInfo?              StatusInfo            = null,

                                       Result?                  Result                = null,
                                       DateTime?                ResponseTimestamp     = null,

                                       SourceRouting?           Destination           = null,
                                       NetworkPath?             NetworkPath           = null,

                                       IEnumerable<KeyPair>?    SignKeys              = null,
                                       IEnumerable<SignInfo>?   SignInfos             = null,
                                       IEnumerable<Signature>?  Signatures            = null,

                                       CustomData?              CustomData            = null,

                                       SerializationFormats?    SerializationFormat   = null,
                                       CancellationToken        CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:PublishFirmwareResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "GenericStatusEnumType": {
        //             "description": "Indicates whether the request was accepted.",
        //             "javaType": "GenericStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected"
        //             ]
        //         },
        //         "StatusInfoType": {
        //             "description": "Element providing more information about the status.",
        //             "javaType": "StatusInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "reasonCode": {
        //                     "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "additionalInfo": {
        //                     "description": "Additional text to provide detailed information.",
        //                     "type": "string",
        //                     "maxLength": 1024
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "reasonCode"
        //             ]
        //         },
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "status": {
        //             "$ref": "#/definitions/GenericStatusEnumType"
        //         },
        //         "statusInfo": {
        //             "$ref": "#/definitions/StatusInfoType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a PublishFirmware response.
        /// </summary>
        /// <param name="Request">The PublishFirmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomPublishFirmwareResponseParser">A delegate to parse custom PublishFirmware responses.</param>
        public static PublishFirmwareResponse Parse(PublishFirmwareRequest                                 Request,
                                                    JObject                                                JSON,
                                                    SourceRouting                                          Destination,
                                                    NetworkPath                                            NetworkPath,
                                                    DateTime?                                              ResponseTimestamp                     = null,
                                                    CustomJObjectParserDelegate<PublishFirmwareResponse>?  CustomPublishFirmwareResponseParser   = null,
                                                    CustomJObjectParserDelegate<StatusInfo>?               CustomStatusInfoParser                = null,
                                                    CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                                    CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var publishFirmwareResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomPublishFirmwareResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return publishFirmwareResponse;
            }

            throw new ArgumentException("The given JSON representation of a PublishFirmware response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out PublishFirmwareResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a PublishFirmware response.
        /// </summary>
        /// <param name="Request">The PublishFirmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PublishFirmwareResponse">The parsed PublishFirmware response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPublishFirmwareResponseParser">A delegate to parse custom PublishFirmware responses.</param>
        public static Boolean TryParse(PublishFirmwareRequest                                 Request,
                                       JObject                                                JSON,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out PublishFirmwareResponse?      PublishFirmwareResponse,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       DateTime?                                              ResponseTimestamp                     = null,
                                       CustomJObjectParserDelegate<PublishFirmwareResponse>?  CustomPublishFirmwareResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?               CustomStatusInfoParser                = null,
                                       CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                       CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            try
            {

                PublishFirmwareResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "PublishFirmware status",
                                         GenericStatus.TryParse,
                                         out GenericStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo    [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures    [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                PublishFirmwareResponse = new PublishFirmwareResponse(

                                              Request,
                                              Status,
                                              StatusInfo,

                                              null,
                                              ResponseTimestamp,

                                              Destination,
                                              NetworkPath,

                                              null,
                                              null,
                                              Signatures,

                                              CustomData

                                          );

                if (CustomPublishFirmwareResponseParser is not null)
                    PublishFirmwareResponse = CustomPublishFirmwareResponseParser(JSON,
                                                                                  PublishFirmwareResponse);

                return true;

            }
            catch (Exception e)
            {
                PublishFirmwareResponse  = null;
                ErrorResponse            = "The given JSON representation of a PublishFirmware response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPublishFirmwareResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPublishFirmwareResponseSerializer">A delegate to serialize custom PublishFirmware responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                    IncludeJSONLDContext                      = false,
                              CustomJObjectSerializerDelegate<PublishFirmwareResponse>?  CustomPublishFirmwareResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?               CustomStatusInfoSerializer                = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("status",       Status.              AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.          ToJSON(CustomStatusInfoSerializer,
                                                                                           CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomPublishFirmwareResponseSerializer is not null
                       ? CustomPublishFirmwareResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The PublishFirmware failed because of a request error.
        /// </summary>
        /// <param name="Request">The PublishFirmware request.</param>
        public static PublishFirmwareResponse RequestError(PublishFirmwareRequest   Request,
                                                           EventTracking_Id         EventTrackingId,
                                                           ResultCode               ErrorCode,
                                                           String?                  ErrorDescription    = null,
                                                           JObject?                 ErrorDetails        = null,
                                                           DateTime?                ResponseTimestamp   = null,

                                                           SourceRouting?           Destination         = null,
                                                           NetworkPath?             NetworkPath         = null,

                                                           IEnumerable<KeyPair>?    SignKeys            = null,
                                                           IEnumerable<SignInfo>?   SignInfos           = null,
                                                           IEnumerable<Signature>?  Signatures          = null,

                                                           CustomData?              CustomData          = null)

            => new (

                   Request,
                   GenericStatus.Rejected,
                   null,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The PublishFirmware failed.
        /// </summary>
        /// <param name="Request">The PublishFirmware request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static PublishFirmwareResponse FormationViolation(PublishFirmwareRequest  Request,
                                                                 String                  ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The PublishFirmware failed.
        /// </summary>
        /// <param name="Request">The PublishFirmware request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static PublishFirmwareResponse SignatureError(PublishFirmwareRequest  Request,
                                                             String                  ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The PublishFirmware failed.
        /// </summary>
        /// <param name="Request">The PublishFirmware request.</param>
        /// <param name="Description">An optional error description.</param>
        public static PublishFirmwareResponse Failed(PublishFirmwareRequest  Request,
                                                     String?                 Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The PublishFirmware failed because of an exception.
        /// </summary>
        /// <param name="Request">The PublishFirmware request.</param>
        /// <param name="Exception">The exception.</param>
        public static PublishFirmwareResponse ExceptionOccured(PublishFirmwareRequest  Request,
                                                               Exception               Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (PublishFirmwareResponse1, PublishFirmwareResponse2)

        /// <summary>
        /// Compares two PublishFirmware responses for equality.
        /// </summary>
        /// <param name="PublishFirmwareResponse1">A PublishFirmware response.</param>
        /// <param name="PublishFirmwareResponse2">Another PublishFirmware response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (PublishFirmwareResponse? PublishFirmwareResponse1,
                                           PublishFirmwareResponse? PublishFirmwareResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(PublishFirmwareResponse1, PublishFirmwareResponse2))
                return true;

            // If one is null, but not both, return false.
            if (PublishFirmwareResponse1 is null || PublishFirmwareResponse2 is null)
                return false;

            return PublishFirmwareResponse1.Equals(PublishFirmwareResponse2);

        }

        #endregion

        #region Operator != (PublishFirmwareResponse1, PublishFirmwareResponse2)

        /// <summary>
        /// Compares two PublishFirmware responses for inequality.
        /// </summary>
        /// <param name="PublishFirmwareResponse1">A PublishFirmware response.</param>
        /// <param name="PublishFirmwareResponse2">Another PublishFirmware response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (PublishFirmwareResponse? PublishFirmwareResponse1,
                                           PublishFirmwareResponse? PublishFirmwareResponse2)

            => !(PublishFirmwareResponse1 == PublishFirmwareResponse2);

        #endregion

        #endregion

        #region IEquatable<PublishFirmwareResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two PublishFirmware responses for equality.
        /// </summary>
        /// <param name="Object">A PublishFirmware response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PublishFirmwareResponse publishFirmwareResponse &&
                   Equals(publishFirmwareResponse);

        #endregion

        #region Equals(PublishFirmwareResponse)

        /// <summary>
        /// Compares two PublishFirmware responses for equality.
        /// </summary>
        /// <param name="PublishFirmwareResponse">A PublishFirmware response to compare with.</param>
        public override Boolean Equals(PublishFirmwareResponse? PublishFirmwareResponse)

            => PublishFirmwareResponse is not null &&

               Status.Equals(PublishFirmwareResponse.Status) &&

             ((StatusInfo is     null && PublishFirmwareResponse.StatusInfo is     null) ||
               StatusInfo is not null && PublishFirmwareResponse.StatusInfo is not null && StatusInfo.Equals(PublishFirmwareResponse.StatusInfo)) &&

               base.GenericEquals(PublishFirmwareResponse);

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

            => Status.AsText();

        #endregion

    }

}
