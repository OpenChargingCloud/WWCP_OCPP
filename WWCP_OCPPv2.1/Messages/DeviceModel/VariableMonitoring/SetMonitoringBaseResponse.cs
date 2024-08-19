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
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A SetMonitoringBase response.
    /// </summary>
    public class SetMonitoringBaseResponse : AResponse<SetMonitoringBaseRequest,
                                                       SetMonitoringBaseResponse>,
                                             IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/setMonitoringBaseResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext             Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Whether the charging station was able to accept the request.
        /// </summary>
        [Mandatory]
        public GenericDeviceModelStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?               StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetMonitoringBase response.
        /// </summary>
        /// <param name="Request">The SetMonitoringBase request leading to this response.</param>
        /// <param name="Status">Whether the charging station was able to accept the request.</param>
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
        public SetMonitoringBaseResponse(SetMonitoringBaseRequest  Request,
                                         GenericDeviceModelStatus  Status,
                                         StatusInfo?               StatusInfo            = null,

                                         Result?                   Result                = null,
                                         DateTime?                 ResponseTimestamp     = null,

                                         SourceRouting?            Destination           = null,
                                         NetworkPath?              NetworkPath           = null,

                                         IEnumerable<KeyPair>?     SignKeys              = null,
                                         IEnumerable<SignInfo>?    SignInfos             = null,
                                         IEnumerable<Signature>?   Signatures            = null,

                                         CustomData?               CustomData            = null,

                                         SerializationFormats?     SerializationFormat   = null,
                                         CancellationToken         CancellationToken     = default)

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
        //   "$id": "urn:OCPP:Cp:2:2020:3:SetMonitoringBaseResponse",
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
        //     "GenericDeviceModelStatusEnumType": {
        //       "description": "Indicates whether the Charging Station was able to accept the request.",
        //       "javaType": "GenericDeviceModelStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "NotSupported",
        //         "EmptyResultSet"
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
        //       "$ref": "#/definitions/GenericDeviceModelStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomSetMonitoringBaseResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SetMonitoringBase response.
        /// </summary>
        /// <param name="Request">The SetMonitoringBase request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetMonitoringBaseResponseParser">A delegate to parse custom SetMonitoringBase responses.</param>
        public static SetMonitoringBaseResponse Parse(SetMonitoringBaseRequest                                 Request,
                                                      JObject                                                  JSON,
                                                      SourceRouting                                        Destination,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTime?                                                ResponseTimestamp                       = null,
                                                      CustomJObjectParserDelegate<SetMonitoringBaseResponse>?  CustomSetMonitoringBaseResponseParser   = null,
                                                      CustomJObjectParserDelegate<StatusInfo>?                 CustomStatusInfoParser                  = null,
                                                      CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                                      CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var setDisplayMessageResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSetMonitoringBaseResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return setDisplayMessageResponse;
            }

            throw new ArgumentException("The given JSON representation of a SetMonitoringBase response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SetMonitoringBaseResponse, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SetMonitoringBase response.
        /// </summary>
        /// <param name="Request">The SetMonitoringBase request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetMonitoringBaseResponse">The parsed SetMonitoringBase response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetMonitoringBaseResponseParser">A delegate to parse custom SetMonitoringBase responses.</param>
        public static Boolean TryParse(SetMonitoringBaseRequest                                 Request,
                                       JObject                                                  JSON,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out SetMonitoringBaseResponse?      SetMonitoringBaseResponse,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                ResponseTimestamp                       = null,
                                       CustomJObjectParserDelegate<SetMonitoringBaseResponse>?  CustomSetMonitoringBaseResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                 CustomStatusInfoParser                  = null,
                                       CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                       CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            try
            {

                SetMonitoringBaseResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "display message status",
                                         GenericDeviceModelStatusExtensions.TryParse,
                                         out GenericDeviceModelStatus Status,
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetMonitoringBaseResponse = new SetMonitoringBaseResponse(

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

                if (CustomSetMonitoringBaseResponseParser is not null)
                    SetMonitoringBaseResponse = CustomSetMonitoringBaseResponseParser(JSON,
                                                                                      SetMonitoringBaseResponse);

                return true;

            }
            catch (Exception e)
            {
                SetMonitoringBaseResponse  = null;
                ErrorResponse              = "The given JSON representation of a SetMonitoringBase response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetMonitoringBaseResponseSerializer = null, CustomStatusInfoSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetMonitoringBaseResponseSerializer">A delegate to serialize custom charging profile responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetMonitoringBaseResponse>?  CustomSetMonitoringBaseResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                 CustomStatusInfoSerializer                  = null,
                              CustomJObjectSerializerDelegate<Signature>?                  CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
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

            return CustomSetMonitoringBaseResponseSerializer is not null
                       ? CustomSetMonitoringBaseResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SetMonitoringBase failed because of a request error.
        /// </summary>
        /// <param name="Request">The SetMonitoringBase request.</param>
        public static SetMonitoringBaseResponse RequestError(SetMonitoringBaseRequest  Request,
                                                             EventTracking_Id          EventTrackingId,
                                                             ResultCode                ErrorCode,
                                                             String?                   ErrorDescription    = null,
                                                             JObject?                  ErrorDetails        = null,
                                                             DateTime?                 ResponseTimestamp   = null,

                                                             SourceRouting?            Destination         = null,
                                                             NetworkPath?              NetworkPath         = null,

                                                             IEnumerable<KeyPair>?     SignKeys            = null,
                                                             IEnumerable<SignInfo>?    SignInfos           = null,
                                                             IEnumerable<Signature>?   Signatures          = null,

                                                             CustomData?               CustomData          = null)

            => new (

                   Request,
                   GenericDeviceModelStatus.Rejected,
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
        /// The SetMonitoringBase failed.
        /// </summary>
        /// <param name="Request">The SetMonitoringBase request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetMonitoringBaseResponse FormationViolation(SetMonitoringBaseRequest  Request,
                                                                   String                    ErrorDescription)

            => new (Request,
                    GenericDeviceModelStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The SetMonitoringBase failed.
        /// </summary>
        /// <param name="Request">The SetMonitoringBase request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetMonitoringBaseResponse SignatureError(SetMonitoringBaseRequest  Request,
                                                               String                    ErrorDescription)

            => new (Request,
                    GenericDeviceModelStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The SetMonitoringBase failed.
        /// </summary>
        /// <param name="Request">The SetMonitoringBase request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SetMonitoringBaseResponse Failed(SetMonitoringBaseRequest  Request,
                                                       String?                   Description   = null)

            => new (Request,
                    GenericDeviceModelStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The SetMonitoringBase failed because of an exception.
        /// </summary>
        /// <param name="Request">The SetMonitoringBase request.</param>
        /// <param name="Exception">The exception.</param>
        public static SetMonitoringBaseResponse ExceptionOccured(SetMonitoringBaseRequest  Request,
                                                                 Exception                 Exception)

            => new (Request,
                    GenericDeviceModelStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SetMonitoringBaseResponse1, SetMonitoringBaseResponse2)

        /// <summary>
        /// Compares two SetMonitoringBase responses for equality.
        /// </summary>
        /// <param name="SetMonitoringBaseResponse1">A SetMonitoringBase response.</param>
        /// <param name="SetMonitoringBaseResponse2">Another SetMonitoringBase response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetMonitoringBaseResponse? SetMonitoringBaseResponse1,
                                           SetMonitoringBaseResponse? SetMonitoringBaseResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetMonitoringBaseResponse1, SetMonitoringBaseResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SetMonitoringBaseResponse1 is null || SetMonitoringBaseResponse2 is null)
                return false;

            return SetMonitoringBaseResponse1.Equals(SetMonitoringBaseResponse2);

        }

        #endregion

        #region Operator != (SetMonitoringBaseResponse1, SetMonitoringBaseResponse2)

        /// <summary>
        /// Compares two SetMonitoringBase responses for inequality.
        /// </summary>
        /// <param name="SetMonitoringBaseResponse1">A SetMonitoringBase response.</param>
        /// <param name="SetMonitoringBaseResponse2">Another SetMonitoringBase response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetMonitoringBaseResponse? SetMonitoringBaseResponse1,
                                           SetMonitoringBaseResponse? SetMonitoringBaseResponse2)

            => !(SetMonitoringBaseResponse1 == SetMonitoringBaseResponse2);

        #endregion

        #endregion

        #region IEquatable<SetMonitoringBaseResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetMonitoringBase responses for equality.
        /// </summary>
        /// <param name="Object">A SetMonitoringBase response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetMonitoringBaseResponse setDisplayMessageResponse &&
                   Equals(setDisplayMessageResponse);

        #endregion

        #region Equals(SetMonitoringBaseResponse)

        /// <summary>
        /// Compares two SetMonitoringBase responses for equality.
        /// </summary>
        /// <param name="SetMonitoringBaseResponse">A SetMonitoringBase response to compare with.</param>
        public override Boolean Equals(SetMonitoringBaseResponse? SetMonitoringBaseResponse)

            => SetMonitoringBaseResponse is not null &&

               Status.     Equals(SetMonitoringBaseResponse.Status) &&

             ((StatusInfo is     null && SetMonitoringBaseResponse.StatusInfo is     null) ||
               StatusInfo is not null && SetMonitoringBaseResponse.StatusInfo is not null && StatusInfo.Equals(SetMonitoringBaseResponse.StatusInfo)) &&

               base.GenericEquals(SetMonitoringBaseResponse);

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
