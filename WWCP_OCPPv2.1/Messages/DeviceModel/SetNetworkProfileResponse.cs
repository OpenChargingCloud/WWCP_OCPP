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
    /// The SetNetworkProfile response.
    /// </summary>
    public class SetNetworkProfileResponse : AResponse<SetNetworkProfileRequest,
                                                       SetNetworkProfileResponse>,
                                             IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/setNetworkProfileResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext            Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Whether the charging station was able to accept the request.
        /// </summary>
        [Mandatory]
        public SetNetworkProfileStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?              StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetNetworkProfile response.
        /// </summary>
        /// <param name="Request">The SetNetworkProfile request leading to this response.</param>
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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public SetNetworkProfileResponse(SetNetworkProfileRequest  Request,
                                         SetNetworkProfileStatus   Status,
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
        //   "$id": "urn:OCPP:Cp:2:2020:3:SetNetworkProfileResponse",
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
        //     "SetNetworkProfileStatusEnumType": {
        //       "description": "Result of operation.",
        //       "javaType": "SetNetworkProfileStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "Failed"
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
        //       "$ref": "#/definitions/SetNetworkProfileStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomSetNetworkProfileResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SetNetworkProfile response.
        /// </summary>
        /// <param name="Request">The SetNetworkProfile request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetNetworkProfileResponseParser">A delegate to parse custom SetNetworkProfile responses.</param>
        public static SetNetworkProfileResponse Parse(SetNetworkProfileRequest                                 Request,
                                                      JObject                                                  JSON,
                                                      SourceRouting                                        Destination,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTime?                                                ResponseTimestamp                       = null,
                                                      CustomJObjectParserDelegate<SetNetworkProfileResponse>?  CustomSetNetworkProfileResponseParser   = null,
                                                      CustomJObjectParserDelegate<StatusInfo>?                 CustomStatusInfoParser                  = null,
                                                      CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                                      CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var setNetworkProfileResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSetNetworkProfileResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return setNetworkProfileResponse;
            }

            throw new ArgumentException("The given JSON representation of a SetNetworkProfile response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SetNetworkProfileResponse, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SetNetworkProfile response.
        /// </summary>
        /// <param name="Request">The SetNetworkProfile request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetNetworkProfileResponse">The parsed SetNetworkProfile response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetNetworkProfileResponseParser">A delegate to parse custom SetNetworkProfile responses.</param>
        public static Boolean TryParse(SetNetworkProfileRequest                                 Request,
                                       JObject                                                  JSON,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out SetNetworkProfileResponse?      SetNetworkProfileResponse,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                ResponseTimestamp                       = null,
                                       CustomJObjectParserDelegate<SetNetworkProfileResponse>?  CustomSetNetworkProfileResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                 CustomStatusInfoParser                  = null,
                                       CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                       CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            try
            {

                SetNetworkProfileResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "SetNetworkProfile status",
                                         SetNetworkProfileStatusExtensions.TryParse,
                                         out SetNetworkProfileStatus Status,
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


                SetNetworkProfileResponse = new SetNetworkProfileResponse(

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

                if (CustomSetNetworkProfileResponseParser is not null)
                    SetNetworkProfileResponse = CustomSetNetworkProfileResponseParser(JSON,
                                                                                      SetNetworkProfileResponse);

                return true;

            }
            catch (Exception e)
            {
                SetNetworkProfileResponse  = null;
                ErrorResponse              = "The given JSON representation of a SetNetworkProfile response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetNetworkProfileResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetNetworkProfileResponseSerializer">A delegate to serialize custom charging profile responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetNetworkProfileResponse>?  CustomSetNetworkProfileResponseSerializer   = null,
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

            return CustomSetNetworkProfileResponseSerializer is not null
                       ? CustomSetNetworkProfileResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SetNetworkProfile failed because of a request error.
        /// </summary>
        /// <param name="Request">The SetNetworkProfile request.</param>
        public static SetNetworkProfileResponse RequestError(SetNetworkProfileRequest  Request,
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
                   SetNetworkProfileStatus.Rejected,
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
        /// The SetNetworkProfile failed.
        /// </summary>
        /// <param name="Request">The SetNetworkProfile request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetNetworkProfileResponse FormationViolation(SetNetworkProfileRequest  Request,
                                                                   String                    ErrorDescription)

            => new (Request,
                    SetNetworkProfileStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The SetNetworkProfile failed.
        /// </summary>
        /// <param name="Request">The SetNetworkProfile request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetNetworkProfileResponse SignatureError(SetNetworkProfileRequest  Request,
                                                               String                    ErrorDescription)

            => new (Request,
                    SetNetworkProfileStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The SetNetworkProfile failed.
        /// </summary>
        /// <param name="Request">The SetNetworkProfile request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SetNetworkProfileResponse Failed(SetNetworkProfileRequest  Request,
                                                       String?                   Description   = null)

            => new (Request,
                    SetNetworkProfileStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The SetNetworkProfile failed because of an exception.
        /// </summary>
        /// <param name="Request">The SetNetworkProfile request.</param>
        /// <param name="Exception">The exception.</param>
        public static SetNetworkProfileResponse ExceptionOccured(SetNetworkProfileRequest  Request,
                                                                 Exception                 Exception)

            => new (Request,
                    SetNetworkProfileStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SetNetworkProfileResponse1, SetNetworkProfileResponse2)

        /// <summary>
        /// Compares two SetNetworkProfile responses for equality.
        /// </summary>
        /// <param name="SetNetworkProfileResponse1">A SetNetworkProfile response.</param>
        /// <param name="SetNetworkProfileResponse2">Another SetNetworkProfile response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetNetworkProfileResponse? SetNetworkProfileResponse1,
                                           SetNetworkProfileResponse? SetNetworkProfileResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetNetworkProfileResponse1, SetNetworkProfileResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SetNetworkProfileResponse1 is null || SetNetworkProfileResponse2 is null)
                return false;

            return SetNetworkProfileResponse1.Equals(SetNetworkProfileResponse2);

        }

        #endregion

        #region Operator != (SetNetworkProfileResponse1, SetNetworkProfileResponse2)

        /// <summary>
        /// Compares two SetNetworkProfile responses for inequality.
        /// </summary>
        /// <param name="SetNetworkProfileResponse1">A SetNetworkProfile response.</param>
        /// <param name="SetNetworkProfileResponse2">Another SetNetworkProfile response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetNetworkProfileResponse? SetNetworkProfileResponse1,
                                           SetNetworkProfileResponse? SetNetworkProfileResponse2)

            => !(SetNetworkProfileResponse1 == SetNetworkProfileResponse2);

        #endregion

        #endregion

        #region IEquatable<SetNetworkProfileResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetNetworkProfile responses for equality.
        /// </summary>
        /// <param name="Object">A SetNetworkProfile response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetNetworkProfileResponse setNetworkProfileResponse &&
                   Equals(setNetworkProfileResponse);

        #endregion

        #region Equals(SetNetworkProfileResponse)

        /// <summary>
        /// Compares two SetNetworkProfile responses for equality.
        /// </summary>
        /// <param name="SetNetworkProfileResponse">A SetNetworkProfile response to compare with.</param>
        public override Boolean Equals(SetNetworkProfileResponse? SetNetworkProfileResponse)

            => SetNetworkProfileResponse is not null &&

               Status.     Equals(SetNetworkProfileResponse.Status) &&

             ((StatusInfo is     null && SetNetworkProfileResponse.StatusInfo is     null) ||
               StatusInfo is not null && SetNetworkProfileResponse.StatusInfo is not null && StatusInfo.Equals(SetNetworkProfileResponse.StatusInfo)) &&

               base.GenericEquals(SetNetworkProfileResponse);

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
