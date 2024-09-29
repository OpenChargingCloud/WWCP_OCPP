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
    /// The SetDisplayMessage response.
    /// </summary>
    public class SetDisplayMessageResponse : AResponse<SetDisplayMessageRequest,
                                                       SetDisplayMessageResponse>,
                                             IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/setDisplayMessageResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext         Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Whether the charging station is able to display the message.
        /// </summary>
        [Mandatory]
        public DisplayMessageStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?           StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetDisplayMessage response.
        /// </summary>
        /// <param name="Request">The SetDisplayMessage request leading to this response.</param>
        /// <param name="Status">Whether the charging station is able to display the message.</param>
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
        public SetDisplayMessageResponse(SetDisplayMessageRequest  Request,
                                         DisplayMessageStatus      Status,
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
        //   "$id": "urn:OCPP:Cp:2:2020:3:SetDisplayMessageResponse",
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
        //     "DisplayMessageStatusEnumType": {
        //       "description": "This indicates whether the Charging Station is able to display the message.",
        //       "javaType": "DisplayMessageStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "NotSupportedMessageFormat",
        //         "Rejected",
        //         "NotSupportedPriority",
        //         "NotSupportedState",
        //         "UnknownTransaction"
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
        //       "$ref": "#/definitions/DisplayMessageStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomSetDisplayMessageResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SetDisplayMessage response.
        /// </summary>
        /// <param name="Request">The SetDisplayMessage request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetDisplayMessageResponseParser">A delegate to parse custom SetDisplayMessage responses.</param>
        public static SetDisplayMessageResponse Parse(SetDisplayMessageRequest                                 Request,
                                                      JObject                                                  JSON,
                                                      SourceRouting                                        Destination,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTime?                                                ResponseTimestamp                       = null,
                                                      CustomJObjectParserDelegate<SetDisplayMessageResponse>?  CustomSetDisplayMessageResponseParser   = null,
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
                         CustomSetDisplayMessageResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return setDisplayMessageResponse;
            }

            throw new ArgumentException("The given JSON representation of a SetDisplayMessage response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SetDisplayMessageResponse, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SetDisplayMessage response.
        /// </summary>
        /// <param name="Request">The SetDisplayMessage request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetDisplayMessageResponse">The parsed SetDisplayMessage response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetDisplayMessageResponseParser">A delegate to parse custom SetDisplayMessage responses.</param>
        public static Boolean TryParse(SetDisplayMessageRequest                                 Request,
                                       JObject                                                  JSON,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out SetDisplayMessageResponse?      SetDisplayMessageResponse,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                ResponseTimestamp                       = null,
                                       CustomJObjectParserDelegate<SetDisplayMessageResponse>?  CustomSetDisplayMessageResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                 CustomStatusInfoParser                  = null,
                                       CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                       CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            try
            {

                SetDisplayMessageResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "display message status",
                                         DisplayMessageStatusExtensions.TryParse,
                                         out DisplayMessageStatus Status,
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


                SetDisplayMessageResponse = new SetDisplayMessageResponse(

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

                if (CustomSetDisplayMessageResponseParser is not null)
                    SetDisplayMessageResponse = CustomSetDisplayMessageResponseParser(JSON,
                                                                                      SetDisplayMessageResponse);

                return true;

            }
            catch (Exception e)
            {
                SetDisplayMessageResponse  = null;
                ErrorResponse              = "The given JSON representation of a SetDisplayMessage response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetDisplayMessageResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetDisplayMessageResponseSerializer">A delegate to serialize custom charging profile responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetDisplayMessageResponse>?  CustomSetDisplayMessageResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                 CustomStatusInfoSerializer                  = null,
                              CustomJObjectSerializerDelegate<Signature>?                  CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",      Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",  StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetDisplayMessageResponseSerializer is not null
                       ? CustomSetDisplayMessageResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SetDisplayMessage failed because of a request error.
        /// </summary>
        /// <param name="Request">The SetDisplayMessage request.</param>
        public static SetDisplayMessageResponse RequestError(SetDisplayMessageRequest  Request,
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
                   DisplayMessageStatus.Rejected,
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
        /// The SetDisplayMessage failed.
        /// </summary>
        /// <param name="Request">The SetDisplayMessage request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetDisplayMessageResponse FormationViolation(SetDisplayMessageRequest  Request,
                                                                   String                    ErrorDescription)

            => new (Request,
                    DisplayMessageStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The SetDisplayMessage failed.
        /// </summary>
        /// <param name="Request">The SetDisplayMessage request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetDisplayMessageResponse SignatureError(SetDisplayMessageRequest  Request,
                                                               String                    ErrorDescription)

            => new (Request,
                    DisplayMessageStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The SetDisplayMessage failed.
        /// </summary>
        /// <param name="Request">The SetDisplayMessage request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SetDisplayMessageResponse Failed(SetDisplayMessageRequest  Request,
                                                       String?                   Description   = null)

            => new (Request,
                    DisplayMessageStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The SetDisplayMessage failed because of an exception.
        /// </summary>
        /// <param name="Request">The SetDisplayMessage request.</param>
        /// <param name="Exception">The exception.</param>
        public static SetDisplayMessageResponse ExceptionOccured(SetDisplayMessageRequest  Request,
                                                                 Exception                 Exception)

            => new (Request,
                    DisplayMessageStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SetDisplayMessageResponse1, SetDisplayMessageResponse2)

        /// <summary>
        /// Compares two SetDisplayMessage responses for equality.
        /// </summary>
        /// <param name="SetDisplayMessageResponse1">A SetDisplayMessage response.</param>
        /// <param name="SetDisplayMessageResponse2">Another SetDisplayMessage response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetDisplayMessageResponse? SetDisplayMessageResponse1,
                                           SetDisplayMessageResponse? SetDisplayMessageResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetDisplayMessageResponse1, SetDisplayMessageResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SetDisplayMessageResponse1 is null || SetDisplayMessageResponse2 is null)
                return false;

            return SetDisplayMessageResponse1.Equals(SetDisplayMessageResponse2);

        }

        #endregion

        #region Operator != (SetDisplayMessageResponse1, SetDisplayMessageResponse2)

        /// <summary>
        /// Compares two SetDisplayMessage responses for inequality.
        /// </summary>
        /// <param name="SetDisplayMessageResponse1">A SetDisplayMessage response.</param>
        /// <param name="SetDisplayMessageResponse2">Another SetDisplayMessage response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetDisplayMessageResponse? SetDisplayMessageResponse1,
                                           SetDisplayMessageResponse? SetDisplayMessageResponse2)

            => !(SetDisplayMessageResponse1 == SetDisplayMessageResponse2);

        #endregion

        #endregion

        #region IEquatable<SetDisplayMessageResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetDisplayMessage responses for equality.
        /// </summary>
        /// <param name="Object">A SetDisplayMessage response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetDisplayMessageResponse setDisplayMessageResponse &&
                   Equals(setDisplayMessageResponse);

        #endregion

        #region Equals(SetDisplayMessageResponse)

        /// <summary>
        /// Compares two SetDisplayMessage responses for equality.
        /// </summary>
        /// <param name="SetDisplayMessageResponse">A SetDisplayMessage response to compare with.</param>
        public override Boolean Equals(SetDisplayMessageResponse? SetDisplayMessageResponse)

            => SetDisplayMessageResponse is not null &&

               Status.     Equals(SetDisplayMessageResponse.Status) &&

             ((StatusInfo is     null && SetDisplayMessageResponse.StatusInfo is     null) ||
               StatusInfo is not null && SetDisplayMessageResponse.StatusInfo is not null && StatusInfo.Equals(SetDisplayMessageResponse.StatusInfo)) &&

               base.GenericEquals(SetDisplayMessageResponse);

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
