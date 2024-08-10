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
    /// The ClearChargingProfile response.
    /// </summary>
    public class ClearChargingProfileResponse : AResponse<ClearChargingProfileRequest,
                                                          ClearChargingProfileResponse>,
                                                IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/clearChargingProfileResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext               Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the ClearChargingProfile command.
        /// </summary>
        [Mandatory]
        public ClearChargingProfileStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?                 StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ClearChargingProfile response.
        /// </summary>
        /// <param name="Request">The ClearChargingProfile request leading to this response.</param>
        /// <param name="Status">The success or failure of the reset command.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
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
        public ClearChargingProfileResponse(ClearChargingProfileRequest  Request,
                                            ClearChargingProfileStatus   Status,
                                            StatusInfo?                  StatusInfo          = null,

                                            Result?                      Result              = null,
                                            DateTime?                    ResponseTimestamp   = null,

                                            NetworkingNode_Id?           DestinationId       = null,
                                            NetworkPath?                 NetworkPath         = null,

                                            IEnumerable<KeyPair>?        SignKeys            = null,
                                            IEnumerable<SignInfo>?       SignInfos           = null,
                                            IEnumerable<Signature>?      Signatures          = null,

                                            CustomData?                  CustomData          = null)

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
                           base.            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ClearChargingProfileResponse",
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
        //     "ClearChargingProfileStatusEnumType": {
        //       "description": "Indicates if the Charging Station was able to execute the request.",
        //       "javaType": "ClearChargingProfileStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Unknown"
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
        //       "$ref": "#/definitions/ClearChargingProfileStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomClearChargingProfileResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ClearChargingProfile response.
        /// </summary>
        /// <param name="Request">The ClearChargingProfile request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomClearChargingProfileResponseParser">A delegate to parse custom ClearChargingProfile responses.</param>
        public static ClearChargingProfileResponse Parse(ClearChargingProfileRequest                                 Request,
                                                         JObject                                                     JSON,
                                                         NetworkingNode_Id                                           DestinationId,
                                                         NetworkPath                                                 NetworkPath,
                                                         DateTime?                                                   ResponseTimestamp                          = null,
                                                         CustomJObjectParserDelegate<ClearChargingProfileResponse>?  CustomClearChargingProfileResponseParser   = null,
                                                         CustomJObjectParserDelegate<StatusInfo>?                    CustomStatusInfoParser                     = null,
                                                         CustomJObjectParserDelegate<Signature>?                     CustomSignatureParser                      = null,
                                                         CustomJObjectParserDelegate<CustomData>?                    CustomCustomDataParser                     = null)
        {

            if (TryParse(Request,
                         JSON,
                         DestinationId,
                         NetworkPath,
                         out var clearChargingProfileResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomClearChargingProfileResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return clearChargingProfileResponse;
            }

            throw new ArgumentException("The given JSON representation of a ClearChargingProfile response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ClearChargingProfileResponse, out ErrorResponse, CustomClearChargingProfileResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a ClearChargingProfile response.
        /// </summary>
        /// <param name="Request">The ClearChargingProfile request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearChargingProfileResponse">The parsed ClearChargingProfile response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearChargingProfileResponseParser">A delegate to parse custom ClearChargingProfile responses.</param>
        public static Boolean TryParse(ClearChargingProfileRequest                                 Request,
                                       JObject                                                     JSON,
                                       NetworkingNode_Id                                           DestinationId,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out ClearChargingProfileResponse?      ClearChargingProfileResponse,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       DateTime?                                                   ResponseTimestamp                          = null,
                                       CustomJObjectParserDelegate<ClearChargingProfileResponse>?  CustomClearChargingProfileResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                    CustomStatusInfoParser                     = null,
                                       CustomJObjectParserDelegate<Signature>?                     CustomSignatureParser                      = null,
                                       CustomJObjectParserDelegate<CustomData>?                    CustomCustomDataParser                     = null)
        {

            try
            {

                ClearChargingProfileResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "ClearChargingProfile status",
                                         ClearChargingProfileStatusExtensions.TryParse,
                                         out ClearChargingProfileStatus Status,
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


                ClearChargingProfileResponse = new ClearChargingProfileResponse(

                                                   Request,
                                                   Status,
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

                if (CustomClearChargingProfileResponseParser is not null)
                    ClearChargingProfileResponse = CustomClearChargingProfileResponseParser(JSON,
                                                                                            ClearChargingProfileResponse);

                return true;

            }
            catch (Exception e)
            {
                ClearChargingProfileResponse  = null;
                ErrorResponse                 = "The given JSON representation of a ClearChargingProfile response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearChargingProfileResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearChargingProfileResponseSerializer">A delegate to serialize custom ClearChargingProfile responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearChargingProfileResponse>?  CustomClearChargingProfileResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                    CustomStatusInfoSerializer                     = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
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

            return CustomClearChargingProfileResponseSerializer is not null
                       ? CustomClearChargingProfileResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The ClearChargingProfile failed because of a request error.
        /// </summary>
        /// <param name="Request">The ClearChargingProfile request.</param>
        public static ClearChargingProfileResponse RequestError(ClearChargingProfileRequest  Request,
                                                                EventTracking_Id             EventTrackingId,
                                                                ResultCode                   ErrorCode,
                                                                String?                      ErrorDescription    = null,
                                                                JObject?                     ErrorDetails        = null,
                                                                DateTime?                    ResponseTimestamp   = null,

                                                                NetworkingNode_Id?           DestinationId       = null,
                                                                NetworkPath?                 NetworkPath         = null,

                                                                IEnumerable<KeyPair>?        SignKeys            = null,
                                                                IEnumerable<SignInfo>?       SignInfos           = null,
                                                                IEnumerable<Signature>?      Signatures          = null,

                                                                CustomData?                  CustomData          = null)

            => new (

                   Request,
                   ClearChargingProfileStatus.Unknown,
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
        /// The ClearChargingProfile failed.
        /// </summary>
        /// <param name="Request">The ClearChargingProfile request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ClearChargingProfileResponse FormationViolation(ClearChargingProfileRequest  Request,
                                                                      String                       ErrorDescription)

            => new (Request,
                    ClearChargingProfileStatus.Unknown,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The ClearChargingProfile failed.
        /// </summary>
        /// <param name="Request">The ClearChargingProfile request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ClearChargingProfileResponse SignatureError(ClearChargingProfileRequest  Request,
                                                                  String                       ErrorDescription)

            => new (Request,
                    ClearChargingProfileStatus.Unknown,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The ClearChargingProfile failed.
        /// </summary>
        /// <param name="Request">The ClearChargingProfile request.</param>
        /// <param name="Description">An optional error description.</param>
        public static ClearChargingProfileResponse Failed(ClearChargingProfileRequest  Request,
                                                          String?                      Description   = null)

            => new (Request,
                    ClearChargingProfileStatus.Unknown,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The ClearChargingProfile failed because of an exception.
        /// </summary>
        /// <param name="Request">The ClearChargingProfile request.</param>
        /// <param name="Exception">The exception.</param>
        public static ClearChargingProfileResponse ExceptionOccured(ClearChargingProfileRequest  Request,
                                                                    Exception                    Exception)

            => new (Request,
                    ClearChargingProfileStatus.Unknown,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (ClearChargingProfileResponse1, ClearChargingProfileResponse2)

        /// <summary>
        /// Compares two ClearChargingProfile responses for equality.
        /// </summary>
        /// <param name="ClearChargingProfileResponse1">A ClearChargingProfile response.</param>
        /// <param name="ClearChargingProfileResponse2">Another ClearChargingProfile response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearChargingProfileResponse? ClearChargingProfileResponse1,
                                           ClearChargingProfileResponse? ClearChargingProfileResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearChargingProfileResponse1, ClearChargingProfileResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ClearChargingProfileResponse1 is null || ClearChargingProfileResponse2 is null)
                return false;

            return ClearChargingProfileResponse1.Equals(ClearChargingProfileResponse2);

        }

        #endregion

        #region Operator != (ClearChargingProfileResponse1, ClearChargingProfileResponse2)

        /// <summary>
        /// Compares two ClearChargingProfile responses for inequality.
        /// </summary>
        /// <param name="ClearChargingProfileResponse1">A ClearChargingProfile response.</param>
        /// <param name="ClearChargingProfileResponse2">Another ClearChargingProfile response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearChargingProfileResponse? ClearChargingProfileResponse1,
                                           ClearChargingProfileResponse? ClearChargingProfileResponse2)

            => !(ClearChargingProfileResponse1 == ClearChargingProfileResponse2);

        #endregion

        #endregion

        #region IEquatable<ClearChargingProfileResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClearChargingProfile responses for equality.
        /// </summary>
        /// <param name="Object">A ClearChargingProfile response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearChargingProfileResponse clearChargingProfileResponse &&
                   Equals(clearChargingProfileResponse);

        #endregion

        #region Equals(ClearChargingProfileResponse)

        /// <summary>
        /// Compares two ClearChargingProfile responses for equality.
        /// </summary>
        /// <param name="ClearChargingProfileResponse">A ClearChargingProfile response to compare with.</param>
        public override Boolean Equals(ClearChargingProfileResponse? ClearChargingProfileResponse)

            => ClearChargingProfileResponse is not null &&

               Status.     Equals(ClearChargingProfileResponse.Status) &&

             ((StatusInfo is     null && ClearChargingProfileResponse.StatusInfo is     null) ||
               StatusInfo is not null && ClearChargingProfileResponse.StatusInfo is not null && StatusInfo.Equals(ClearChargingProfileResponse.StatusInfo)) &&

               base.GenericEquals(ClearChargingProfileResponse);

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
