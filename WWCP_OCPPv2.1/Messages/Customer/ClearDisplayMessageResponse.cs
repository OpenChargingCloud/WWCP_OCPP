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
    /// The ClearDisplayMessage response.
    /// </summary>
    public class ClearDisplayMessageResponse : AResponse<ClearDisplayMessageRequest,
                                                         ClearDisplayMessageResponse>,
                                               IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/clearDisplayMessageResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the ClearDisplayMessage command.
        /// </summary>
        [Mandatory]
        public ClearMessageStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?         StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ClearDisplayMessage response.
        /// </summary>
        /// <param name="Request">The ClearDisplayMessage request leading to this response.</param>
        /// <param name="Status">The success or failure of the reset command.</param>
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
        public ClearDisplayMessageResponse(ClearDisplayMessageRequest  Request,
                                           ClearMessageStatus          Status,
                                           StatusInfo?                 StatusInfo            = null,

                                           Result?                     Result                = null,
                                           DateTime?                   ResponseTimestamp     = null,

                                           SourceRouting?              Destination           = null,
                                           NetworkPath?                NetworkPath           = null,

                                           IEnumerable<KeyPair>?       SignKeys              = null,
                                           IEnumerable<SignInfo>?      SignInfos             = null,
                                           IEnumerable<Signature>?     Signatures            = null,

                                           CustomData?                 CustomData            = null,

                                           SerializationFormats?       SerializationFormat   = null,
                                           CancellationToken           CancellationToken     = default)

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
        //     "$id": "urn:OCPP:Cp:2:2025:1:ClearDisplayMessageResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "ClearMessageStatusEnumType": {
        //             "description": "Returns whether the Charging Station has been able to remove the message.",
        //             "javaType": "ClearMessageStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Unknown",
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
        //             "$ref": "#/definitions/ClearMessageStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomClearDisplayMessageResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ClearDisplayMessage response.
        /// </summary>
        /// <param name="Request">The ClearDisplayMessage request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomClearDisplayMessageResponseParser">A delegate to parse custom ClearDisplayMessage responses.</param>
        public static ClearDisplayMessageResponse Parse(ClearDisplayMessageRequest                                 Request,
                                                        JObject                                                    JSON,
                                                        SourceRouting                                          Destination,
                                                        NetworkPath                                                NetworkPath,
                                                        DateTime?                                                  ResponseTimestamp                         = null,
                                                        CustomJObjectParserDelegate<ClearDisplayMessageResponse>?  CustomClearDisplayMessageResponseParser   = null,
                                                        CustomJObjectParserDelegate<StatusInfo>?                   CustomStatusInfoParser                    = null,
                                                        CustomJObjectParserDelegate<Signature>?                    CustomSignatureParser                     = null,
                                                        CustomJObjectParserDelegate<CustomData>?                   CustomCustomDataParser                    = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var clearDisplayMessageResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomClearDisplayMessageResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return clearDisplayMessageResponse;
            }

            throw new ArgumentException("The given JSON representation of a ClearDisplayMessage response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ClearDisplayMessageResponse, out ErrorResponse, CustomClearDisplayMessageResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a ClearDisplayMessage response.
        /// </summary>
        /// <param name="Request">The ClearDisplayMessage request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearDisplayMessageResponse">The parsed ClearDisplayMessage response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearDisplayMessageResponseParser">A delegate to parse custom ClearDisplayMessage responses.</param>
        public static Boolean TryParse(ClearDisplayMessageRequest                                 Request,
                                       JObject                                                    JSON,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                                NetworkPath,
                                       [NotNullWhen(true)]  out ClearDisplayMessageResponse?      ClearDisplayMessageResponse,
                                       [NotNullWhen(false)] out String?                           ErrorResponse,
                                       DateTime?                                                  ResponseTimestamp                         = null,
                                       CustomJObjectParserDelegate<ClearDisplayMessageResponse>?  CustomClearDisplayMessageResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                   CustomStatusInfoParser                    = null,
                                       CustomJObjectParserDelegate<Signature>?                    CustomSignatureParser                     = null,
                                       CustomJObjectParserDelegate<CustomData>?                   CustomCustomDataParser                    = null)
        {

            try
            {

                ClearDisplayMessageResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "ClearDisplayMessage status",
                                         ClearMessageStatusExtensions.TryParse,
                                         out ClearMessageStatus Status,
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


                ClearDisplayMessageResponse = new ClearDisplayMessageResponse(

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

                if (CustomClearDisplayMessageResponseParser is not null)
                    ClearDisplayMessageResponse = CustomClearDisplayMessageResponseParser(JSON,
                                                                                          ClearDisplayMessageResponse);

                return true;

            }
            catch (Exception e)
            {
                ClearDisplayMessageResponse  = null;
                ErrorResponse                = "The given JSON representation of a ClearDisplayMessage response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearDisplayMessageResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearDisplayMessageResponseSerializer">A delegate to serialize custom ClearDisplayMessage responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                        IncludeJSONLDContext                          = false,
                              CustomJObjectSerializerDelegate<ClearDisplayMessageResponse>?  CustomClearDisplayMessageResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                   CustomStatusInfoSerializer                    = null,
                              CustomJObjectSerializerDelegate<Signature>?                    CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
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

            return CustomClearDisplayMessageResponseSerializer is not null
                       ? CustomClearDisplayMessageResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The ClearDisplayMessage failed because of a request error.
        /// </summary>
        /// <param name="Request">The ClearDisplayMessage request.</param>
        public static ClearDisplayMessageResponse RequestError(ClearDisplayMessageRequest  Request,
                                                               EventTracking_Id            EventTrackingId,
                                                               ResultCode                  ErrorCode,
                                                               String?                     ErrorDescription    = null,
                                                               JObject?                    ErrorDetails        = null,
                                                               DateTime?                   ResponseTimestamp   = null,

                                                               SourceRouting?              Destination         = null,
                                                               NetworkPath?                NetworkPath         = null,

                                                               IEnumerable<KeyPair>?       SignKeys            = null,
                                                               IEnumerable<SignInfo>?      SignInfos           = null,
                                                               IEnumerable<Signature>?     Signatures          = null,

                                                               CustomData?                 CustomData          = null)

            => new (

                   Request,
                   ClearMessageStatus.Unknown,
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
        /// The ClearDisplayMessage failed.
        /// </summary>
        /// <param name="Request">The ClearDisplayMessage request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ClearDisplayMessageResponse FormationViolation(ClearDisplayMessageRequest  Request,
                                                                     String                           ErrorDescription)

            => new (Request,
                    ClearMessageStatus.Unknown,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The ClearDisplayMessage failed.
        /// </summary>
        /// <param name="Request">The ClearDisplayMessage request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ClearDisplayMessageResponse SignatureError(ClearDisplayMessageRequest  Request,
                                                                 String                           ErrorDescription)

            => new (Request,
                    ClearMessageStatus.Unknown,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The ClearDisplayMessage failed.
        /// </summary>
        /// <param name="Request">The ClearDisplayMessage request.</param>
        /// <param name="Description">An optional error description.</param>
        public static ClearDisplayMessageResponse Failed(ClearDisplayMessageRequest  Request,
                                                         String?                          Description   = null)

            => new (Request,
                    ClearMessageStatus.Unknown,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The ClearDisplayMessage failed because of an exception.
        /// </summary>
        /// <param name="Request">The ClearDisplayMessage request.</param>
        /// <param name="Exception">The exception.</param>
        public static ClearDisplayMessageResponse ExceptionOccured(ClearDisplayMessageRequest  Request,
                                                                   Exception                        Exception)

            => new (Request,
                    ClearMessageStatus.Unknown,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (ClearDisplayMessageResponse1, ClearDisplayMessageResponse2)

        /// <summary>
        /// Compares two ClearDisplayMessage responses for equality.
        /// </summary>
        /// <param name="ClearDisplayMessageResponse1">A ClearDisplayMessage response.</param>
        /// <param name="ClearDisplayMessageResponse2">Another ClearDisplayMessage response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearDisplayMessageResponse? ClearDisplayMessageResponse1,
                                           ClearDisplayMessageResponse? ClearDisplayMessageResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearDisplayMessageResponse1, ClearDisplayMessageResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ClearDisplayMessageResponse1 is null || ClearDisplayMessageResponse2 is null)
                return false;

            return ClearDisplayMessageResponse1.Equals(ClearDisplayMessageResponse2);

        }

        #endregion

        #region Operator != (ClearDisplayMessageResponse1, ClearDisplayMessageResponse2)

        /// <summary>
        /// Compares two ClearDisplayMessage responses for inequality.
        /// </summary>
        /// <param name="ClearDisplayMessageResponse1">A ClearDisplayMessage response.</param>
        /// <param name="ClearDisplayMessageResponse2">Another ClearDisplayMessage response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearDisplayMessageResponse? ClearDisplayMessageResponse1,
                                           ClearDisplayMessageResponse? ClearDisplayMessageResponse2)

            => !(ClearDisplayMessageResponse1 == ClearDisplayMessageResponse2);

        #endregion

        #endregion

        #region IEquatable<ClearDisplayMessageResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClearDisplayMessage responses for equality.
        /// </summary>
        /// <param name="Object">A ClearDisplayMessage response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearDisplayMessageResponse clearDisplayMessageResponse &&
                   Equals(clearDisplayMessageResponse);

        #endregion

        #region Equals(ClearDisplayMessageResponse)

        /// <summary>
        /// Compares two ClearDisplayMessage responses for equality.
        /// </summary>
        /// <param name="ClearDisplayMessageResponse">A ClearDisplayMessage response to compare with.</param>
        public override Boolean Equals(ClearDisplayMessageResponse? ClearDisplayMessageResponse)

            => ClearDisplayMessageResponse is not null &&

               Status.     Equals(ClearDisplayMessageResponse.Status) &&

             ((StatusInfo is     null && ClearDisplayMessageResponse.StatusInfo is     null) ||
               StatusInfo is not null && ClearDisplayMessageResponse.StatusInfo is not null && StatusInfo.Equals(ClearDisplayMessageResponse.StatusInfo)) &&

               base.GenericEquals(ClearDisplayMessageResponse);

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
