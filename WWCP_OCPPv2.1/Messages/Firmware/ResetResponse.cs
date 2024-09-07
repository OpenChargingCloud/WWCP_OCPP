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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A Reset response.
    /// </summary>
    public class ResetResponse : AResponse<ResetRequest,
                                           ResetResponse>,
                                 IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/ResetResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the Reset command.
        /// </summary>
        [Mandatory]
        public ResetStatus    Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Reset response.
        /// </summary>
        /// <param name="Request">The Reset request leading to this response.</param>
        /// <param name="Status">The success or failure of the Reset command.</param>
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
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ResetResponse(ResetRequest             Request,
                             ResetStatus              Status,
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
                           base.            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ResetResponse",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
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
        //     "ResetStatusEnumType": {
        //       "description": "This indicates whether the Charging Station is able to perform the Reset.",
        //       "javaType": "ResetStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "Scheduled"
        //       ]
        //     },
        //     "StatusInfoType": {
        //       "description": "Element providing more information about the status.",
        //       "javaType": "StatusInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "reasonCode": {
        //           "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "additionalInfo": {
        //           "description": "Additional text to provide detailed information.",
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
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "status": {
        //       "$ref": "#/definitions/ResetStatusEnumType"
        //     },
        //     "statusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomResetResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a Reset response.
        /// </summary>
        /// <param name="Request">The Reset request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomResetResponseParser">An optional delegate to parse custom Reset responses.</param>
        public static ResetResponse Parse(ResetRequest                                 Request,
                                          JObject                                      JSON,
                                          SourceRouting                                Destination,
                                          NetworkPath                                  NetworkPath,
                                          DateTime?                                    ResponseTimestamp           = null,
                                          CustomJObjectParserDelegate<ResetResponse>?  CustomResetResponseParser   = null,
                                          CustomJObjectParserDelegate<StatusInfo>?     CustomStatusInfoParser      = null,
                                          CustomJObjectParserDelegate<Signature>?      CustomSignatureParser       = null,
                                          CustomJObjectParserDelegate<CustomData>?     CustomCustomDataParser      = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var resetResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomResetResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return resetResponse;
            }

            throw new ArgumentException("The given JSON representation of a Reset response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ResetResponse, out ErrorResponse, CustomResetResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a Reset response.
        /// </summary>
        /// <param name="Request">The Reset request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ResetResponse">The parsed Reset response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomResetResponseParser">An optional delegate to parse custom Reset responses.</param>
        public static Boolean TryParse(ResetRequest                                 Request,
                                       JObject                                      JSON,
                                       SourceRouting                                Destination,
                                       NetworkPath                                  NetworkPath,
                                       [NotNullWhen(true)]  out ResetResponse?      ResetResponse,
                                       [NotNullWhen(false)] out String?             ErrorResponse,
                                       DateTime?                                    ResponseTimestamp           = null,
                                       CustomJObjectParserDelegate<ResetResponse>?  CustomResetResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?     CustomStatusInfoParser      = null,
                                       CustomJObjectParserDelegate<Signature>?      CustomSignatureParser       = null,
                                       CustomJObjectParserDelegate<CustomData>?     CustomCustomDataParser      = null)
        {

            try
            {

                ResetResponse = null;

                #region ResetStatus    [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "Reset status",
                                         ResetStatusExtensions.TryParse,
                                         out ResetStatus ResetStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo     [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           (JObject json, [NotNullWhen(true)] out StatusInfo? statusInfo, [NotNullWhen(false)] out String? errorResponse)
                                                 => OCPPv2_1.StatusInfo.TryParse(json, out statusInfo, out errorResponse, CustomStatusInfoParser),
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures     [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              (JObject json, [NotNullWhen(true)] out Signature? signature, [NotNullWhen(false)] out String? errorResponse)
                                                  => Signature.TryParse(json, out signature, out errorResponse, CustomSignatureParser, CustomCustomDataParser),
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData     [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           (JObject json, [NotNullWhen(true)] out CustomData? customData, [NotNullWhen(false)] out String? errorResponse)
                                                => WWCP.CustomData.TryParse(json, out customData, out errorResponse, CustomCustomDataParser),
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ResetResponse = new ResetResponse(

                                    Request,
                                    ResetStatus,
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

                if (CustomResetResponseParser is not null)
                    ResetResponse = CustomResetResponseParser(JSON,
                                                              ResetResponse);

                return true;

            }
            catch (Exception e)
            {
                ResetResponse  = null;
                ErrorResponse  = "The given JSON representation of a Reset response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomResetResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomResetResponseSerializer">A delegate to serialize custom Reset responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ResetResponse>?   CustomResetResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?      CustomStatusInfoSerializer      = null,
                              CustomJObjectSerializerDelegate<Signature>?       CustomSignatureSerializer       = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomResetResponseSerializer is not null
                       ? CustomResetResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The Reset failed because of a request error.
        /// </summary>
        /// <param name="Request">The Reset request.</param>
        public static ResetResponse RequestError(ResetRequest             Request,
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
                   ResetStatus.Rejected,
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
        /// The Reset failed.
        /// </summary>
        /// <param name="Request">The Reset request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ResetResponse FormationViolation(ResetRequest  Request,
                                                       String        ErrorDescription)

            => new (Request,
                    ResetStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The Reset failed.
        /// </summary>
        /// <param name="Request">The Reset request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ResetResponse SignatureError(ResetRequest  Request,
                                                   String        ErrorDescription)

            => new (Request,
                    ResetStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The Reset failed.
        /// </summary>
        /// <param name="Request">The Reset request.</param>
        /// <param name="Description">An optional error description.</param>
        public static ResetResponse Failed(ResetRequest  Request,
                                           String?       Description   = null)

            => new (Request,
                    ResetStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The Reset failed because of an exception.
        /// </summary>
        /// <param name="Request">The Reset request.</param>
        /// <param name="Exception">The exception.</param>
        public static ResetResponse ExceptionOccured(ResetRequest  Request,
                                                     Exception     Exception)

            => new (Request,
                    ResetStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (ResetResponse1, ResetResponse2)

        /// <summary>
        /// Compares two Reset responses for equality.
        /// </summary>
        /// <param name="ResetResponse1">A Reset response.</param>
        /// <param name="ResetResponse2">Another Reset response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ResetResponse? ResetResponse1,
                                           ResetResponse? ResetResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ResetResponse1, ResetResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ResetResponse1 is null || ResetResponse2 is null)
                return false;

            return ResetResponse1.Equals(ResetResponse2);

        }

        #endregion

        #region Operator != (ResetResponse1, ResetResponse2)

        /// <summary>
        /// Compares two Reset responses for inequality.
        /// </summary>
        /// <param name="ResetResponse1">A Reset response.</param>
        /// <param name="ResetResponse2">Another Reset response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ResetResponse? ResetResponse1,
                                           ResetResponse? ResetResponse2)

            => !(ResetResponse1 == ResetResponse2);

        #endregion

        #endregion

        #region IEquatable<ResetResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Reset responses for equality.
        /// </summary>
        /// <param name="Object">A Reset response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ResetResponse ResetResponse &&
                   Equals(ResetResponse);

        #endregion

        #region Equals(ResetResponse)

        /// <summary>
        /// Compares two Reset responses for equality.
        /// </summary>
        /// <param name="ResetResponse">A Reset response to compare with.</param>
        public override Boolean Equals(ResetResponse? ResetResponse)

            => ResetResponse is not null &&

               Status.     Equals(ResetResponse.Status) &&

             ((StatusInfo is     null && ResetResponse.StatusInfo is     null) ||
               StatusInfo is not null && ResetResponse.StatusInfo is not null && StatusInfo.Equals(ResetResponse.StatusInfo)) &&

               base.GenericEquals(ResetResponse);

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
