/*
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
    /// The SendLocalList response.
    /// </summary>
    public class SendLocalListResponse : AResponse<SendLocalListRequest,
                                                   SendLocalListResponse>,
                                         IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/sendLocalListResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext        Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the SendLocalList command.
        /// </summary>
        [Mandatory]
        public SendLocalListStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?          StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SendLocalList response.
        /// </summary>
        /// <param name="Request">The SendLocalList request leading to this response.</param>
        /// <param name="Status">The success or failure of the SendLocalList command.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public SendLocalListResponse(SendLocalListRequest     Request,
                                     SendLocalListStatus      Status,
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
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SendLocalListResponse",
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
        //     "SendLocalListStatusEnumType": {
        //       "description": "This indicates whether the Charging Station has successfully received and applied the update of the Local Authorization List.",
        //       "javaType": "SendLocalListStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Failed",
        //         "VersionMismatch"
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
        //       "$ref": "#/definitions/SendLocalListStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomSendLocalListResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SendLocalList response.
        /// </summary>
        /// <param name="Request">The SendLocalList request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSendLocalListResponseParser">A delegate to parse custom SendLocalList responses.</param>
        public static SendLocalListResponse Parse(SendLocalListRequest                                 Request,
                                                  JObject                                              JSON,
                                                  SourceRouting                                    Destination,
                                                  NetworkPath                                          NetworkPath,
                                                  DateTime?                                            ResponseTimestamp                   = null,
                                                  CustomJObjectParserDelegate<SendLocalListResponse>?  CustomSendLocalListResponseParser   = null,
                                                  CustomJObjectParserDelegate<StatusInfo>?             CustomStatusInfoParser              = null,
                                                  CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                                  CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var sendLocalListResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSendLocalListResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return sendLocalListResponse;
            }

            throw new ArgumentException("The given JSON representation of a SendLocalList response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SendLocalListResponse, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SendLocalList response.
        /// </summary>
        /// <param name="Request">The SendLocalList request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SendLocalListResponse">The parsed SendLocalList response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSendLocalListResponseParser">A delegate to parse custom SendLocalList responses.</param>
        public static Boolean TryParse(SendLocalListRequest                                 Request,
                                       JObject                                              JSON,
                                       SourceRouting                                    Destination,
                                       NetworkPath                                          NetworkPath,
                                       [NotNullWhen(true)]  out SendLocalListResponse?      SendLocalListResponse,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       DateTime?                                            ResponseTimestamp                   = null,
                                       CustomJObjectParserDelegate<SendLocalListResponse>?  CustomSendLocalListResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?             CustomStatusInfoParser              = null,
                                       CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                       CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            try
            {

                SendLocalListResponse = null;

                #region SendLocalListStatus    [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "update status",
                                         SendLocalListStatusExtensions.TryParse,
                                         out SendLocalListStatus SendLocalListStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo             [optional]

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

                #region Signatures             [optional, OCPP_CSE]

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

                #region CustomData             [optional]

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


                SendLocalListResponse = new SendLocalListResponse(

                                            Request,
                                            SendLocalListStatus,
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

                if (CustomSendLocalListResponseParser is not null)
                    SendLocalListResponse = CustomSendLocalListResponseParser(JSON,
                                                                              SendLocalListResponse);

                return true;

            }
            catch (Exception e)
            {
                SendLocalListResponse  = null;
                ErrorResponse          = "The given JSON representation of a SendLocalList response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSendLocalListResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSendLocalListResponseSerializer">A delegate to serialize custom SendLocalList responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                  IncludeJSONLDContext                    = false,
                              CustomJObjectSerializerDelegate<SendLocalListResponse>?  CustomSendLocalListResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?             CustomStatusInfoSerializer              = null,
                              CustomJObjectSerializerDelegate<Signature>?              CustomSignatureSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
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

            return CustomSendLocalListResponseSerializer is not null
                       ? CustomSendLocalListResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SendLocalList failed because of a request error.
        /// </summary>
        /// <param name="Request">The SendLocalList request.</param>
        public static SendLocalListResponse RequestError(SendLocalListRequest     Request,
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
                   SendLocalListStatus.Failed,
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
        /// The SendLocalList failed.
        /// </summary>
        /// <param name="Request">The SendLocalList request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SendLocalListResponse FormationViolation(SendLocalListRequest  Request,
                                                               String                ErrorDescription)

            => new (Request,
                    SendLocalListStatus.Failed,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The SendLocalList failed.
        /// </summary>
        /// <param name="Request">The SendLocalList request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SendLocalListResponse SignatureError(SendLocalListRequest  Request,
                                                           String                ErrorDescription)

            => new (Request,
                    SendLocalListStatus.Failed,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The SendLocalList failed.
        /// </summary>
        /// <param name="Request">The SendLocalList request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SendLocalListResponse Failed(SendLocalListRequest  Request,
                                                   String?               Description   = null)

            => new (Request,
                    SendLocalListStatus.Failed,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The SendLocalList failed because of an exception.
        /// </summary>
        /// <param name="Request">The SendLocalList request.</param>
        /// <param name="Exception">The exception.</param>
        public static SendLocalListResponse ExceptionOccured(SendLocalListRequest  Request,
                                                             Exception             Exception)

            => new (Request,
                    SendLocalListStatus.Failed,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SendLocalListResponse1, SendLocalListResponse2)

        /// <summary>
        /// Compares two SendLocalList responses for equality.
        /// </summary>
        /// <param name="SendLocalListResponse1">A SendLocalList response.</param>
        /// <param name="SendLocalListResponse2">Another SendLocalList response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SendLocalListResponse? SendLocalListResponse1,
                                           SendLocalListResponse? SendLocalListResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SendLocalListResponse1, SendLocalListResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SendLocalListResponse1 is null || SendLocalListResponse2 is null)
                return false;

            return SendLocalListResponse1.Equals(SendLocalListResponse2);

        }

        #endregion

        #region Operator != (SendLocalListResponse1, SendLocalListResponse2)

        /// <summary>
        /// Compares two SendLocalList responses for inequality.
        /// </summary>
        /// <param name="SendLocalListResponse1">A SendLocalList response.</param>
        /// <param name="SendLocalListResponse2">Another SendLocalList response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SendLocalListResponse? SendLocalListResponse1,
                                           SendLocalListResponse? SendLocalListResponse2)

            => !(SendLocalListResponse1 == SendLocalListResponse2);

        #endregion

        #endregion

        #region IEquatable<SendLocalListResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SendLocalList responses for equality.
        /// </summary>
        /// <param name="Object">A SendLocalList response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SendLocalListResponse sendLocalListResponse &&
                   Equals(sendLocalListResponse);

        #endregion

        #region Equals(SendLocalListResponse)

        /// <summary>
        /// Compares two SendLocalList responses for equality.
        /// </summary>
        /// <param name="SendLocalListResponse">A SendLocalList response to compare with.</param>
        public override Boolean Equals(SendLocalListResponse? SendLocalListResponse)

            => SendLocalListResponse is not null &&

               Status.     Equals(SendLocalListResponse.Status) &&

             ((StatusInfo is     null && SendLocalListResponse.StatusInfo is     null) ||
               StatusInfo is not null && SendLocalListResponse.StatusInfo is not null && StatusInfo.Equals(SendLocalListResponse.StatusInfo)) &&

               base.GenericEquals(SendLocalListResponse);

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
