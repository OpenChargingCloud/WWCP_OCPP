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
    /// The CancelReservation response.
    /// </summary>
    public class CancelReservationResponse : AResponse<CancelReservationRequest,
                                                       CancelReservationResponse>,
                                             IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/cancelReservationResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext            Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the reservation cancellation.
        /// </summary>
        [Mandatory]
        public CancelReservationStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?              StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CancelReservation response.
        /// </summary>
        /// <param name="Request">The CancelReservation request leading to this response.</param>
        /// <param name="Status">The success or failure of the reservation.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="SourceRouting">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public CancelReservationResponse(CancelReservationRequest  Request,
                                         CancelReservationStatus   Status,
                                         StatusInfo?               StatusInfo          = null,

                                         Result?                   Result              = null,
                                         DateTime?                 ResponseTimestamp   = null,

                                         SourceRouting?        SourceRouting       = null,
                                         NetworkPath?              NetworkPath         = null,

                                         IEnumerable<KeyPair>?     SignKeys            = null,
                                         IEnumerable<SignInfo>?    SignInfos           = null,
                                         IEnumerable<Signature>?   Signatures          = null,

                                         CustomData?               CustomData          = null)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                       SourceRouting,
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
        //   "$id": "urn:OCPP:Cp:2:2020:3:CancelReservationResponse",
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
        //     "CancelReservationStatusEnumType": {
        //       "description": "This indicates the success or failure of the canceling of a reservation by CSMS.",
        //       "javaType": "CancelReservationStatusEnum",
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
        //       "$ref": "#/definitions/CancelReservationStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomCancelReservationResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a CancelReservation response.
        /// </summary>
        /// <param name="Request">The CancelReservation request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomCancelReservationResponseParser">A delegate to parse custom CancelReservation responses.</param>
        public static CancelReservationResponse Parse(CancelReservationRequest                                 Request,
                                                      JObject                                                  JSON,
                                                      SourceRouting                                            SourceRouting,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTime?                                                ResponseTimestamp                       = null,
                                                      CustomJObjectParserDelegate<CancelReservationResponse>?  CustomCancelReservationResponseParser   = null,
                                                      CustomJObjectParserDelegate<StatusInfo>?                 CustomStatusInfoParser                  = null,
                                                      CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                                      CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            if (TryParse(Request,
                         JSON,
                             SourceRouting,
                         NetworkPath,
                         out var cancelReservationResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomCancelReservationResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return cancelReservationResponse;
            }

            throw new ArgumentException("The given JSON representation of a CancelReservation response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out CancelReservationResponse, out ErrorResponse, CustomCancelReservationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a CancelReservation response.
        /// </summary>
        /// <param name="Request">The CancelReservation request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CancelReservationResponse">The parsed CancelReservation response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCancelReservationResponseParser">A delegate to parse custom CancelReservation responses.</param>
        public static Boolean TryParse(CancelReservationRequest                                 Request,
                                       JObject                                                  JSON,
                                       SourceRouting                                            SourceRouting,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out CancelReservationResponse?      CancelReservationResponse,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                ResponseTimestamp                       = null,
                                       CustomJObjectParserDelegate<CancelReservationResponse>?  CustomCancelReservationResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                 CustomStatusInfoParser                  = null,
                                       CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                       CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            try
            {

                CancelReservationResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "CancelReservation status",
                                         CancelReservationStatusExtensions.TryParse,
                                         out CancelReservationStatus Status,
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


                CancelReservationResponse = new CancelReservationResponse(

                                                Request,
                                                Status,
                                                StatusInfo,

                                                null,
                                                ResponseTimestamp,

                                                    SourceRouting,
                                                NetworkPath,

                                                null,
                                                null,
                                                Signatures,

                                                CustomData

                                            );

                if (CustomCancelReservationResponseParser is not null)
                    CancelReservationResponse = CustomCancelReservationResponseParser(JSON,
                                                                                      CancelReservationResponse);

                return true;

            }
            catch (Exception e)
            {
                CancelReservationResponse  = null;
                ErrorResponse              = "The given JSON representation of a CancelReservation response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCancelReservationResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCancelReservationResponseSerializer">A delegate to serialize custom CancelReservation responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CancelReservationResponse>?  CustomCancelReservationResponseSerializer   = null,
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

            return CustomCancelReservationResponseSerializer is not null
                       ? CustomCancelReservationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The CancelReservation failed because of a request error.
        /// </summary>
        /// <param name="Request">The CancelReservation request.</param>
        public static CancelReservationResponse RequestError(CancelReservationRequest  Request,
                                                             EventTracking_Id          EventTrackingId,
                                                             ResultCode                ErrorCode,
                                                             String?                   ErrorDescription    = null,
                                                             JObject?                  ErrorDetails        = null,
                                                             DateTime?                 ResponseTimestamp   = null,

                                                             SourceRouting?        SourceRouting       = null,
                                                             NetworkPath?              NetworkPath         = null,

                                                             IEnumerable<KeyPair>?     SignKeys            = null,
                                                             IEnumerable<SignInfo>?    SignInfos           = null,
                                                             IEnumerable<Signature>?   Signatures          = null,

                                                             CustomData?               CustomData          = null)

            => new (

                   Request,
                   CancelReservationStatus.Rejected,
                   null,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                       SourceRouting,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The CancelReservation failed.
        /// </summary>
        /// <param name="Request">The CancelReservation request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static CancelReservationResponse FormationViolation(CancelReservationRequest  Request,
                                                                   String                    ErrorDescription)

            => new (Request,
                    CancelReservationStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The CancelReservation failed.
        /// </summary>
        /// <param name="Request">The CancelReservation request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static CancelReservationResponse SignatureError(CancelReservationRequest  Request,
                                                               String                    ErrorDescription)

            => new (Request,
                    CancelReservationStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The CancelReservation failed.
        /// </summary>
        /// <param name="Request">The CancelReservation request.</param>
        /// <param name="Description">An optional error description.</param>
        public static CancelReservationResponse Failed(CancelReservationRequest  Request,
                                                       String?                   Description   = null)

            => new (Request,
                    CancelReservationStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The CancelReservation failed because of an exception.
        /// </summary>
        /// <param name="Request">The CancelReservation request.</param>
        /// <param name="Exception">The exception.</param>
        public static CancelReservationResponse ExceptionOccured(CancelReservationRequest  Request,
                                                                 Exception                 Exception)

            => new (Request,
                    CancelReservationStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (CancelReservationResponse1, CancelReservationResponse2)

        /// <summary>
        /// Compares two CancelReservation responses for equality.
        /// </summary>
        /// <param name="CancelReservationResponse1">A CancelReservation response.</param>
        /// <param name="CancelReservationResponse2">Another CancelReservation response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (CancelReservationResponse? CancelReservationResponse1,
                                           CancelReservationResponse? CancelReservationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CancelReservationResponse1, CancelReservationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (CancelReservationResponse1 is null || CancelReservationResponse2 is null)
                return false;

            return CancelReservationResponse1.Equals(CancelReservationResponse2);

        }

        #endregion

        #region Operator != (CancelReservationResponse1, CancelReservationResponse2)

        /// <summary>
        /// Compares two CancelReservation responses for inequality.
        /// </summary>
        /// <param name="CancelReservationResponse1">A CancelReservation response.</param>
        /// <param name="CancelReservationResponse2">Another CancelReservation response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CancelReservationResponse? CancelReservationResponse1,
                                           CancelReservationResponse? CancelReservationResponse2)

            => !(CancelReservationResponse1 == CancelReservationResponse2);

        #endregion

        #endregion

        #region IEquatable<CancelReservationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two CancelReservation responses for equality.
        /// </summary>
        /// <param name="Object">A CancelReservation response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CancelReservationResponse cancelReservationResponse &&
                   Equals(cancelReservationResponse);

        #endregion

        #region Equals(CancelReservationResponse)

        /// <summary>
        /// Compares two CancelReservation responses for equality.
        /// </summary>
        /// <param name="CancelReservationResponse">A CancelReservation response to compare with.</param>
        public override Boolean Equals(CancelReservationResponse? CancelReservationResponse)

            => CancelReservationResponse is not null &&

               Status.     Equals(CancelReservationResponse.Status) &&

             ((StatusInfo is     null && CancelReservationResponse.StatusInfo is     null) ||
               StatusInfo is not null && CancelReservationResponse.StatusInfo is not null && StatusInfo.Equals(CancelReservationResponse.StatusInfo)) &&

               base.GenericEquals(CancelReservationResponse);

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
