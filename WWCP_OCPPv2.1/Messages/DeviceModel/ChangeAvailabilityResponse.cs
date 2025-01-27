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
    /// The ChangeAvailability response.
    /// </summary>
    public class ChangeAvailabilityResponse : AResponse<ChangeAvailabilityRequest,
                                                        ChangeAvailabilityResponse>,
                                              IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/changeAvailabilityResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext             Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the ChangeAvailability command.
        /// </summary>
        [Mandatory]
        public ChangeAvailabilityStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?               StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ChangeAvailability response.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request leading to this response.</param>
        /// <param name="Status">The success or failure of the ChangeAvailability command.</param>
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
        public ChangeAvailabilityResponse(ChangeAvailabilityRequest  Request,
                                          ChangeAvailabilityStatus   Status,
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
        //   "$id": "urn:OCPP:Cp:2:2020:3:ChangeAvailabilityResponse",
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
        //     "ChangeAvailabilityStatusEnumType": {
        //       "description": "This indicates whether the Charging Station is able to perform the availability change.",
        //       "javaType": "ChangeAvailabilityStatusEnum",
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
        //       "$ref": "#/definitions/ChangeAvailabilityStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomChangeAvailabilityResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ChangeAvailability response.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomChangeAvailabilityResponseParser">A delegate to parse custom ChangeAvailability responses.</param>
        public static ChangeAvailabilityResponse Parse(ChangeAvailabilityRequest                                 Request,
                                                       JObject                                                   JSON,
                                                       SourceRouting                                         Destination,
                                                       NetworkPath                                               NetworkPath,
                                                       DateTime?                                                 ResponseTimestamp                        = null,
                                                       CustomJObjectParserDelegate<ChangeAvailabilityResponse>?  CustomChangeAvailabilityResponseParser   = null,
                                                       CustomJObjectParserDelegate<StatusInfo>?                  CustomStatusInfoParser                   = null,
                                                       CustomJObjectParserDelegate<Signature>?                   CustomSignatureParser                    = null,
                                                       CustomJObjectParserDelegate<CustomData>?                  CustomCustomDataParser                   = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var changeAvailabilityResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomChangeAvailabilityResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return changeAvailabilityResponse;
            }

            throw new ArgumentException("The given JSON representation of a ChangeAvailability response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ChangeAvailabilityResponse, out ErrorResponse, CustomChangeAvailabilityResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a ChangeAvailability response.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChangeAvailabilityResponse">The parsed ChangeAvailability response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChangeAvailabilityResponseParser">A delegate to parse custom ChangeAvailability responses.</param>
        public static Boolean TryParse(ChangeAvailabilityRequest                                 Request,
                                       JObject                                                   JSON,
                                       SourceRouting                                         Destination,
                                       NetworkPath                                               NetworkPath,
                                       [NotNullWhen(true)]  out ChangeAvailabilityResponse?      ChangeAvailabilityResponse,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       DateTime?                                                 ResponseTimestamp                        = null,
                                       CustomJObjectParserDelegate<ChangeAvailabilityResponse>?  CustomChangeAvailabilityResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                  CustomStatusInfoParser                   = null,
                                       CustomJObjectParserDelegate<Signature>?                   CustomSignatureParser                    = null,
                                       CustomJObjectParserDelegate<CustomData>?                  CustomCustomDataParser                   = null)
        {

            try
            {

                ChangeAvailabilityResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "availability status",
                                         ChangeAvailabilityStatusExtensions.TryParse,
                                         out ChangeAvailabilityStatus Status,
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


                ChangeAvailabilityResponse = new ChangeAvailabilityResponse(

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

                if (CustomChangeAvailabilityResponseParser is not null)
                    ChangeAvailabilityResponse = CustomChangeAvailabilityResponseParser(JSON,
                                                                                        ChangeAvailabilityResponse);

                return true;

            }
            catch (Exception e)
            {
                ChangeAvailabilityResponse  = null;
                ErrorResponse               = "The given JSON representation of a ChangeAvailability response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChangeAvailabilityResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChangeAvailabilityResponseSerializer">A delegate to serialize custom ChangeAvailability responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                       IncludeJSONLDContext                         = false,
                              CustomJObjectSerializerDelegate<ChangeAvailabilityResponse>?  CustomChangeAvailabilityResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                  CustomStatusInfoSerializer                   = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
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

            return CustomChangeAvailabilityResponseSerializer is not null
                       ? CustomChangeAvailabilityResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The ChangeAvailability failed because of a request error.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request.</param>
        public static ChangeAvailabilityResponse RequestError(ChangeAvailabilityRequest  Request,
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
                   ChangeAvailabilityStatus.Rejected,
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
        /// The ChangeAvailability failed.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ChangeAvailabilityResponse FormationViolation(ChangeAvailabilityRequest  Request,
                                                                    String                     ErrorDescription)

            => new (Request,
                    ChangeAvailabilityStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The ChangeAvailability failed.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ChangeAvailabilityResponse SignatureError(ChangeAvailabilityRequest  Request,
                                                                String                     ErrorDescription)

            => new (Request,
                    ChangeAvailabilityStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The ChangeAvailability failed.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request.</param>
        /// <param name="Description">An optional error description.</param>
        public static ChangeAvailabilityResponse Failed(ChangeAvailabilityRequest  Request,
                                                        String?                    Description   = null)

            => new (Request,
                    ChangeAvailabilityStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The ChangeAvailability failed because of an exception.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request.</param>
        /// <param name="Exception">The exception.</param>
        public static ChangeAvailabilityResponse ExceptionOccured(ChangeAvailabilityRequest  Request,
                                                                  Exception                  Exception)

            => new (Request,
                    ChangeAvailabilityStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (ChangeAvailabilityResponse1, ChangeAvailabilityResponse2)

        /// <summary>
        /// Compares two ChangeAvailability responses for equality.
        /// </summary>
        /// <param name="ChangeAvailabilityResponse1">A ChangeAvailability response.</param>
        /// <param name="ChangeAvailabilityResponse2">Another ChangeAvailability response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ChangeAvailabilityResponse? ChangeAvailabilityResponse1,
                                           ChangeAvailabilityResponse? ChangeAvailabilityResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChangeAvailabilityResponse1, ChangeAvailabilityResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ChangeAvailabilityResponse1 is null || ChangeAvailabilityResponse2 is null)
                return false;

            return ChangeAvailabilityResponse1.Equals(ChangeAvailabilityResponse2);

        }

        #endregion

        #region Operator != (ChangeAvailabilityResponse1, ChangeAvailabilityResponse2)

        /// <summary>
        /// Compares two ChangeAvailability responses for inequality.
        /// </summary>
        /// <param name="ChangeAvailabilityResponse1">A ChangeAvailability response.</param>
        /// <param name="ChangeAvailabilityResponse2">Another ChangeAvailability response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ChangeAvailabilityResponse? ChangeAvailabilityResponse1,
                                           ChangeAvailabilityResponse? ChangeAvailabilityResponse2)

            => !(ChangeAvailabilityResponse1 == ChangeAvailabilityResponse2);

        #endregion

        #endregion

        #region IEquatable<ChangeAvailabilityResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ChangeAvailability responses for equality.
        /// </summary>
        /// <param name="Object">A ChangeAvailability response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChangeAvailabilityResponse changeAvailabilityResponse &&
                   Equals(changeAvailabilityResponse);

        #endregion

        #region Equals(ChangeAvailabilityResponse)

        /// <summary>
        /// Compares two ChangeAvailability responses for equality.
        /// </summary>
        /// <param name="ChangeAvailabilityResponse">A ChangeAvailability response to compare with.</param>
        public override Boolean Equals(ChangeAvailabilityResponse? ChangeAvailabilityResponse)

            => ChangeAvailabilityResponse is not null &&

               Status.     Equals(ChangeAvailabilityResponse.Status) &&

             ((StatusInfo is     null && ChangeAvailabilityResponse.StatusInfo is     null) ||
               StatusInfo is not null && ChangeAvailabilityResponse.StatusInfo is not null && StatusInfo.Equals(ChangeAvailabilityResponse.StatusInfo)) &&

               base.GenericEquals(ChangeAvailabilityResponse);

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
