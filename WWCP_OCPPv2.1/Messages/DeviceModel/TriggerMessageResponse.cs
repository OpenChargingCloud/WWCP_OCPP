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
    /// The TriggerMessage response.
    /// </summary>
    public class TriggerMessageResponse : AResponse<TriggerMessageRequest,
                                                    TriggerMessageResponse>,
                                          IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/triggerMessageResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext         Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the TriggerMessage command.
        /// </summary>
        [Mandatory]
        public TriggerMessageStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?           StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new TriggerMessage response.
        /// </summary>
        /// <param name="Request">The TriggerMessage request leading to this response.</param>
        /// <param name="Status">The success or failure of the TriggerMessage command.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="DestinationId">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public TriggerMessageResponse(TriggerMessageRequest    Request,
                                      TriggerMessageStatus     Status,
                                      StatusInfo?              StatusInfo          = null,

                                      Result?                  Result              = null,
                                      DateTime?                ResponseTimestamp   = null,

                                      NetworkingNode_Id?       DestinationId       = null,
                                      NetworkPath?             NetworkPath         = null,

                                      IEnumerable<KeyPair>?    SignKeys            = null,
                                      IEnumerable<SignInfo>?   SignInfos           = null,
                                      IEnumerable<Signature>?  Signatures          = null,

                                      CustomData?              CustomData          = null)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

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
        //   "$id": "urn:OCPP:Cp:2:2020:3:TriggerMessageResponse",
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
        //     "TriggerMessageStatusEnumType": {
        //       "description": "Indicates whether the Charging Station will send the requested notification or not.",
        //       "javaType": "TriggerMessageStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "NotImplemented"
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
        //       "$ref": "#/definitions/TriggerMessageStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomTriggerMessageResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a TriggerMessage response.
        /// </summary>
        /// <param name="Request">The TriggerMessage request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomTriggerMessageResponseParser">A delegate to parse custom TriggerMessage responses.</param>
        public static TriggerMessageResponse Parse(TriggerMessageRequest                                 Request,
                                                   JObject                                               JSON,
                                                   NetworkingNode_Id                                     DestinationId,
                                                   NetworkPath                                           NetworkPath,
                                                   DateTime?                                             ResponseTimestamp                    = null,
                                                   CustomJObjectParserDelegate<TriggerMessageResponse>?  CustomTriggerMessageResponseParser   = null,
                                                   CustomJObjectParserDelegate<StatusInfo>?              CustomStatusInfoParser               = null,
                                                   CustomJObjectParserDelegate<Signature>?               CustomSignatureParser                = null,
                                                   CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {

            if (TryParse(Request,
                         JSON,
                         DestinationId,
                         NetworkPath,
                         out var triggerMessageResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomTriggerMessageResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return triggerMessageResponse;
            }

            throw new ArgumentException("The given JSON representation of a TriggerMessage response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out TriggerMessageResponse, out ErrorResponse, CustomTriggerMessageResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a TriggerMessage response.
        /// </summary>
        /// <param name="Request">The TriggerMessage request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="TriggerMessageResponse">The parsed TriggerMessage response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTriggerMessageResponseParser">A delegate to parse custom TriggerMessage responses.</param>
        public static Boolean TryParse(TriggerMessageRequest                                 Request,
                                       JObject                                               JSON,
                                       NetworkingNode_Id                                     DestinationId,
                                       NetworkPath                                           NetworkPath,
                                       [NotNullWhen(true)]  out TriggerMessageResponse?      TriggerMessageResponse,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       DateTime?                                             ResponseTimestamp                    = null,
                                       CustomJObjectParserDelegate<TriggerMessageResponse>?  CustomTriggerMessageResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?              CustomStatusInfoParser               = null,
                                       CustomJObjectParserDelegate<Signature>?               CustomSignatureParser                = null,
                                       CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {

            try
            {

                TriggerMessageResponse = null;

                #region TriggerMessageStatus    [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "TriggerMessage status",
                                         TriggerMessageStatusExtensions.TryParse,
                                         out TriggerMessageStatus TriggerMessageStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo              [optional]

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

                #region Signatures              [optional, OCPP_CSE]

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

                #region CustomData              [optional]

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


                TriggerMessageResponse = new TriggerMessageResponse(

                                             Request,
                                             TriggerMessageStatus,
                                             StatusInfo,

                                             null,
                                             ResponseTimestamp,

                                             DestinationId,
                                             NetworkPath,

                                             null,
                                             null,
                                             Signatures,

                                             CustomData

                                         );

                if (CustomTriggerMessageResponseParser is not null)
                    TriggerMessageResponse = CustomTriggerMessageResponseParser(JSON,
                                                                                TriggerMessageResponse);

                return true;

            }
            catch (Exception e)
            {
                TriggerMessageResponse  = null;
                ErrorResponse           = "The given JSON representation of a TriggerMessage response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTriggerMessageResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTriggerMessageResponseSerializer">A delegate to serialize custom TriggerMessage responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TriggerMessageResponse>?  CustomTriggerMessageResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?              CustomStatusInfoSerializer               = null,
                              CustomJObjectSerializerDelegate<Signature>?               CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
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

            return CustomTriggerMessageResponseSerializer is not null
                       ? CustomTriggerMessageResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The TriggerMessage failed because of a request error.
        /// </summary>
        /// <param name="Request">The TriggerMessage request.</param>
        public static TriggerMessageResponse RequestError(TriggerMessageRequest    Request,
                                                          EventTracking_Id         EventTrackingId,
                                                          ResultCode               ErrorCode,
                                                          String?                  ErrorDescription    = null,
                                                          JObject?                 ErrorDetails        = null,
                                                          DateTime?                ResponseTimestamp   = null,

                                                          NetworkingNode_Id?       DestinationId       = null,
                                                          NetworkPath?             NetworkPath         = null,

                                                          IEnumerable<KeyPair>?    SignKeys            = null,
                                                          IEnumerable<SignInfo>?   SignInfos           = null,
                                                          IEnumerable<Signature>?  Signatures          = null,

                                                          CustomData?              CustomData          = null)

            => new (

                   Request,
                   TriggerMessageStatus.Rejected,
                   null,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The TriggerMessage failed.
        /// </summary>
        /// <param name="Request">The TriggerMessage request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static TriggerMessageResponse FormationViolation(TriggerMessageRequest  Request,
                                                                String                 ErrorDescription)

            => new (Request,
                    TriggerMessageStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The TriggerMessage failed.
        /// </summary>
        /// <param name="Request">The TriggerMessage request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static TriggerMessageResponse SignatureError(TriggerMessageRequest  Request,
                                                            String                 ErrorDescription)

            => new (Request,
                    TriggerMessageStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The TriggerMessage failed.
        /// </summary>
        /// <param name="Request">The TriggerMessage request.</param>
        /// <param name="Description">An optional error description.</param>
        public static TriggerMessageResponse Failed(TriggerMessageRequest  Request,
                                                    String?                Description   = null)

            => new (Request,
                    TriggerMessageStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The TriggerMessage failed because of an exception.
        /// </summary>
        /// <param name="Request">The TriggerMessage request.</param>
        /// <param name="Exception">The exception.</param>
        public static TriggerMessageResponse ExceptionOccured(TriggerMessageRequest  Request,
                                                              Exception              Exception)

            => new (Request,
                    TriggerMessageStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (TriggerMessageResponse1, TriggerMessageResponse2)

        /// <summary>
        /// Compares two TriggerMessage responses for equality.
        /// </summary>
        /// <param name="TriggerMessageResponse1">A TriggerMessage response.</param>
        /// <param name="TriggerMessageResponse2">Another TriggerMessage response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (TriggerMessageResponse? TriggerMessageResponse1,
                                           TriggerMessageResponse? TriggerMessageResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TriggerMessageResponse1, TriggerMessageResponse2))
                return true;

            // If one is null, but not both, return false.
            if (TriggerMessageResponse1 is null || TriggerMessageResponse2 is null)
                return false;

            return TriggerMessageResponse1.Equals(TriggerMessageResponse2);

        }

        #endregion

        #region Operator != (TriggerMessageResponse1, TriggerMessageResponse2)

        /// <summary>
        /// Compares two TriggerMessage responses for inequality.
        /// </summary>
        /// <param name="TriggerMessageResponse1">A TriggerMessage response.</param>
        /// <param name="TriggerMessageResponse2">Another TriggerMessage response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (TriggerMessageResponse? TriggerMessageResponse1,
                                           TriggerMessageResponse? TriggerMessageResponse2)

            => !(TriggerMessageResponse1 == TriggerMessageResponse2);

        #endregion

        #endregion

        #region IEquatable<TriggerMessageResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two TriggerMessage responses for equality.
        /// </summary>
        /// <param name="Object">A TriggerMessage response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TriggerMessageResponse triggerMessageResponse &&
                   Equals(triggerMessageResponse);

        #endregion

        #region Equals(TriggerMessageResponse)

        /// <summary>
        /// Compares two TriggerMessage responses for equality.
        /// </summary>
        /// <param name="TriggerMessageResponse">A TriggerMessage response to compare with.</param>
        public override Boolean Equals(TriggerMessageResponse? TriggerMessageResponse)

            => TriggerMessageResponse is not null &&

               Status.     Equals(TriggerMessageResponse.Status) &&

             ((StatusInfo is     null && TriggerMessageResponse.StatusInfo is     null) ||
               StatusInfo is not null && TriggerMessageResponse.StatusInfo is not null && StatusInfo.Equals(TriggerMessageResponse.StatusInfo)) &&

               base.GenericEquals(TriggerMessageResponse);

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

            => Status.ToString();

        #endregion

    }

}
