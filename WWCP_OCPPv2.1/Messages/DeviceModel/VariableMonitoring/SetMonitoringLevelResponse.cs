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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A SetMonitoringLevel response.
    /// </summary>
    public class SetMonitoringLevelResponse : AResponse<SetMonitoringLevelRequest,
                                                        SetMonitoringLevelResponse>,
                                              IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/setMonitoringLevelResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Whether the charging station was able to accept the request.
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
        /// Create a new SetMonitoringLevel response.
        /// </summary>
        /// <param name="Request">The SetMonitoringLevel request leading to this response.</param>
        /// <param name="Status">Whether the charging station was able to accept the request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public SetMonitoringLevelResponse(SetMonitoringLevelRequest  Request,
                                          GenericStatus              Status,
                                          StatusInfo?                StatusInfo            = null,

                                          Result?                    Result                = null,
                                          DateTime?                  ResponseTimestamp     = null,

                                          SourceRouting?             Destination           = null,
                                          NetworkPath?               NetworkPath           = null,

                                          IEnumerable<KeyPair>?      SignKeys              = null,
                                          IEnumerable<SignInfo>?     SignInfos             = null,
                                          IEnumerable<Signature>?    Signatures            = null,

                                          CustomData?                CustomData            = null,

                                          SerializationFormats?      SerializationFormat   = null,
                                          CancellationToken          CancellationToken     = default)

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
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SetMonitoringLevelResponse",
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
        //     "GenericStatusEnumType": {
        //       "description": "Indicates whether the Charging Station was able to accept the request.",
        //       "javaType": "GenericStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected"
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
        //       "$ref": "#/definitions/GenericStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomSetMonitoringLevelResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SetMonitoringLevel response.
        /// </summary>
        /// <param name="Request">The SetMonitoringLevel request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetMonitoringLevelResponseParser">A delegate to parse custom SetMonitoringLevel responses.</param>
        public static SetMonitoringLevelResponse Parse(SetMonitoringLevelRequest                                 Request,
                                                       JObject                                                   JSON,
                                                       SourceRouting                                         Destination,
                                                       NetworkPath                                               NetworkPath,
                                                       DateTime?                                                 ResponseTimestamp                        = null,
                                                       CustomJObjectParserDelegate<SetMonitoringLevelResponse>?  CustomSetMonitoringLevelResponseParser   = null,
                                                       CustomJObjectParserDelegate<StatusInfo>?                  CustomStatusInfoParser                   = null,
                                                       CustomJObjectParserDelegate<Signature>?                   CustomSignatureParser                    = null,
                                                       CustomJObjectParserDelegate<CustomData>?                  CustomCustomDataParser                   = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var setMonitoringLevelResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSetMonitoringLevelResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return setMonitoringLevelResponse;
            }

            throw new ArgumentException("The given JSON representation of a SetMonitoringLevel response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SetMonitoringLevelResponse, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SetMonitoringLevel response.
        /// </summary>
        /// <param name="Request">The SetMonitoringLevel request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetMonitoringLevelResponse">The parsed SetMonitoringLevel response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetMonitoringLevelResponseParser">A delegate to parse custom SetMonitoringLevel responses.</param>
        public static Boolean TryParse(SetMonitoringLevelRequest                                 Request,
                                       JObject                                                   JSON,
                                       SourceRouting                                         Destination,
                                       NetworkPath                                               NetworkPath,
                                       [NotNullWhen(true)]  out SetMonitoringLevelResponse?      SetMonitoringLevelResponse,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       DateTime?                                                 ResponseTimestamp                        = null,
                                       CustomJObjectParserDelegate<SetMonitoringLevelResponse>?  CustomSetMonitoringLevelResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                  CustomStatusInfoParser                   = null,
                                       CustomJObjectParserDelegate<Signature>?                   CustomSignatureParser                    = null,
                                       CustomJObjectParserDelegate<CustomData>?                  CustomCustomDataParser                   = null)
        {

            try
            {

                SetMonitoringLevelResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "monitoring level status",
                                         GenericStatusExtensions.TryParse,
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


                SetMonitoringLevelResponse = new SetMonitoringLevelResponse(

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

                if (CustomSetMonitoringLevelResponseParser is not null)
                    SetMonitoringLevelResponse = CustomSetMonitoringLevelResponseParser(JSON,
                                                                                        SetMonitoringLevelResponse);

                return true;

            }
            catch (Exception e)
            {
                SetMonitoringLevelResponse  = null;
                ErrorResponse               = "The given JSON representation of a SetMonitoringLevel response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetMonitoringLevelResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetMonitoringLevelResponseSerializer">A delegate to serialize custom charging profile responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetMonitoringLevelResponse>?  CustomSetMonitoringLevelResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                  CustomStatusInfoSerializer                   = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
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

            return CustomSetMonitoringLevelResponseSerializer is not null
                       ? CustomSetMonitoringLevelResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SetMonitoringLevel failed because of a request error.
        /// </summary>
        /// <param name="Request">The SetMonitoringLevel request.</param>
        public static SetMonitoringLevelResponse RequestError(SetMonitoringLevelRequest  Request,
                                                              EventTracking_Id           EventTrackingId,
                                                              ResultCode                 ErrorCode,
                                                              String?                    ErrorDescription    = null,
                                                              JObject?                   ErrorDetails        = null,
                                                              DateTime?                  ResponseTimestamp   = null,

                                                              SourceRouting?             Destination         = null,
                                                              NetworkPath?               NetworkPath         = null,

                                                              IEnumerable<KeyPair>?      SignKeys            = null,
                                                              IEnumerable<SignInfo>?     SignInfos           = null,
                                                              IEnumerable<Signature>?    Signatures          = null,

                                                              CustomData?                CustomData          = null)

            => new (

                   Request,
                   GenericStatus.Rejected,
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
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The SetMonitoringLevel failed.
        /// </summary>
        /// <param name="Request">The SetMonitoringLevel request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetMonitoringLevelResponse FormationViolation(SetMonitoringLevelRequest  Request,
                                                                    String                     ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The SetMonitoringLevel failed.
        /// </summary>
        /// <param name="Request">The SetMonitoringLevel request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetMonitoringLevelResponse SignatureError(SetMonitoringLevelRequest  Request,
                                                                String                     ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The SetMonitoringLevel failed.
        /// </summary>
        /// <param name="Request">The SetMonitoringLevel request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SetMonitoringLevelResponse Failed(SetMonitoringLevelRequest  Request,
                                                        String?                    Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The SetMonitoringLevel failed because of an exception.
        /// </summary>
        /// <param name="Request">The SetMonitoringLevel request.</param>
        /// <param name="Exception">The exception.</param>
        public static SetMonitoringLevelResponse ExceptionOccured(SetMonitoringLevelRequest  Request,
                                                                  Exception                  Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SetMonitoringLevelResponse1, SetMonitoringLevelResponse2)

        /// <summary>
        /// Compares two SetMonitoringLevel responses for equality.
        /// </summary>
        /// <param name="SetMonitoringLevelResponse1">A SetMonitoringLevel response.</param>
        /// <param name="SetMonitoringLevelResponse2">Another SetMonitoringLevel response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetMonitoringLevelResponse? SetMonitoringLevelResponse1,
                                           SetMonitoringLevelResponse? SetMonitoringLevelResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetMonitoringLevelResponse1, SetMonitoringLevelResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SetMonitoringLevelResponse1 is null || SetMonitoringLevelResponse2 is null)
                return false;

            return SetMonitoringLevelResponse1.Equals(SetMonitoringLevelResponse2);

        }

        #endregion

        #region Operator != (SetMonitoringLevelResponse1, SetMonitoringLevelResponse2)

        /// <summary>
        /// Compares two SetMonitoringLevel responses for inequality.
        /// </summary>
        /// <param name="SetMonitoringLevelResponse1">A SetMonitoringLevel response.</param>
        /// <param name="SetMonitoringLevelResponse2">Another SetMonitoringLevel response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetMonitoringLevelResponse? SetMonitoringLevelResponse1,
                                           SetMonitoringLevelResponse? SetMonitoringLevelResponse2)

            => !(SetMonitoringLevelResponse1 == SetMonitoringLevelResponse2);

        #endregion

        #endregion

        #region IEquatable<SetMonitoringLevelResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetMonitoringLevel responses for equality.
        /// </summary>
        /// <param name="Object">A SetMonitoringLevel response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetMonitoringLevelResponse setMonitoringLevelResponse &&
                   Equals(setMonitoringLevelResponse);

        #endregion

        #region Equals(SetMonitoringLevelResponse)

        /// <summary>
        /// Compares two SetMonitoringLevel responses for equality.
        /// </summary>
        /// <param name="SetMonitoringLevelResponse">A SetMonitoringLevel response to compare with.</param>
        public override Boolean Equals(SetMonitoringLevelResponse? SetMonitoringLevelResponse)

            => SetMonitoringLevelResponse is not null &&

               Status.     Equals(SetMonitoringLevelResponse.Status) &&

             ((StatusInfo is     null && SetMonitoringLevelResponse.StatusInfo is     null) ||
               StatusInfo is not null && SetMonitoringLevelResponse.StatusInfo is not null && StatusInfo.Equals(SetMonitoringLevelResponse.StatusInfo)) &&

               base.GenericEquals(SetMonitoringLevelResponse);

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
