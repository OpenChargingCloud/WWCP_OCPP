﻿/*
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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The GetCertificateStatus response.
    /// </summary>
    public class GetCertificateStatusResponse : AResponse<CS.GetCertificateStatusRequest,
                                                          GetCertificateStatusResponse>,
                                                IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getCertificateStatusResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext         Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Whether the central system was able to retrieve the OCSP certificate status.
        /// </summary>
        [Mandatory]
        public GetCertificateStatus  Status         { get; }

        /// <summary>
        /// The optional DER encoded and then base64 OCSP response as defined in IETF RFC 6960.
        /// MAY only be omitted when status was not "Accepted".
        /// </summary>
        [Optional]
        public OCSPResult?           OCSPResult    { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?           StatusInfo     { get; }

        #endregion

        #region Constructor(s)

        #region GetCertificateStatusResponse(Request, Status, EXIResponse, StatusInfo = null, ...)

        /// <summary>
        /// Create a new GetCertificateStatus response.
        /// </summary>
        /// <param name="Request">The GetCertificateStatus request leading to this response.</param>
        /// <param name="Status">Whether the central system was able to retrieve the OCSP certificate status.</param>
        /// <param name="OCSPResult">The optional DER encoded and then base64 OCSP response as defined in IETF RFC 6960. MAY only be omitted when status was not "Accepted".</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetCertificateStatusResponse(CS.GetCertificateStatusRequest  Request,
                                            GetCertificateStatus            Status,
                                            OCSPResult                      OCSPResult,
                                            StatusInfo?                     StatusInfo          = null,
                                            DateTime?                       ResponseTimestamp   = null,

                                            NetworkingNode_Id?              DestinationId       = null,
                                            NetworkPath?                    NetworkPath         = null,

                                            IEnumerable<KeyPair>?           SignKeys            = null,
                                            IEnumerable<SignInfo>?          SignInfos           = null,
                                            IEnumerable<Signature>?         Signatures          = null,

                                            CustomData?                     CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status      = Status;
            this.OCSPResult  = OCSPResult;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region GetCertificateStatusResponse(Request, Result)

        /// <summary>
        /// Create a new GetCertificateStatus response.
        /// </summary>
        /// <param name="Request">The GetCertificateStatus request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetCertificateStatusResponse(CS.GetCertificateStatusRequest  Request,
                                            Result                          Result,
                                            DateTime?                       ResponseTimestamp   = null,

                                            NetworkingNode_Id?              DestinationId       = null,
                                            NetworkPath?                    NetworkPath         = null,

                                            IEnumerable<KeyPair>?           SignKeys            = null,
                                            IEnumerable<SignInfo>?          SignInfos           = null,
                                            IEnumerable<Signature>?         Signatures          = null,

                                            CustomData?                     CustomData          = null)

            : base(Request,
                   Result,
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetCertificateStatusResponse",
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
        //     "GetCertificateStatusEnumType": {
        //       "description": "This indicates whether the charging station was able to retrieve the OCSP certificate status.",
        //       "javaType": "GetCertificateStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
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
        //       "$ref": "#/definitions/GetCertificateStatusEnumType"
        //     },
        //     "statusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     },
        //     "ocspResult": {
        //       "description": "OCSPResponse class as defined in &lt;&lt;ref-ocpp_security_24, IETF RFC 6960&gt;&gt;. DER encoded (as defined in &lt;&lt;ref-ocpp_security_24, IETF RFC 6960&gt;&gt;), and then base64 encoded. MAY only be omitted when status is not Accepted.",
        //       "type": "string",
        //       "maxLength": 5500
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetCertificateStatusResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetCertificateStatus response.
        /// </summary>
        /// <param name="Request">The GetCertificateStatus request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetCertificateStatusResponseParser">A delegate to parse custom GetCertificateStatus responses.</param>
        public static GetCertificateStatusResponse Parse(CS.GetCertificateStatusRequest                              Request,
                                                         JObject                                                     JSON,
                                                         NetworkingNode_Id                                           DestinationId,
                                                         NetworkPath                                                 NetworkPath,
                                                         DateTime?                                                   ResponseTimestamp                          = null,
                                                         CustomJObjectParserDelegate<GetCertificateStatusResponse>?  CustomGetCertificateStatusResponseParser   = null,
                                                         CustomJObjectParserDelegate<StatusInfo>?                    CustomStatusInfoParser                     = null,
                                                         CustomJObjectParserDelegate<Signature>?                     CustomSignatureParser                      = null,
                                                         CustomJObjectParserDelegate<CustomData>?                    CustomCustomDataParser                     = null)
        {

            if (TryParse(Request,
                         JSON,
                         DestinationId,
                         NetworkPath,
                         out var getCertificateStatusResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetCertificateStatusResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getCertificateStatusResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetCertificateStatus response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetCertificateStatusResponse, out ErrorResponse, CustomGetCertificateStatusResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetCertificateStatus response.
        /// </summary>
        /// <param name="Request">The GetCertificateStatus request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetCertificateStatusResponse">The parsed GetCertificateStatus response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetCertificateStatusResponseParser">A delegate to parse custom GetCertificateStatus responses.</param>
        public static Boolean TryParse(CS.GetCertificateStatusRequest                              Request,
                                       JObject                                                     JSON,
                                       NetworkingNode_Id                                           DestinationId,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out GetCertificateStatusResponse?      GetCertificateStatusResponse,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       DateTime?                                                   ResponseTimestamp                          = null,
                                       CustomJObjectParserDelegate<GetCertificateStatusResponse>?  CustomGetCertificateStatusResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                    CustomStatusInfoParser                     = null,
                                       CustomJObjectParserDelegate<Signature>?                     CustomSignatureParser                      = null,
                                       CustomJObjectParserDelegate<CustomData>?                    CustomCustomDataParser                     = null)
        {

            try
            {

                GetCertificateStatusResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "GetCertificateStatus",
                                         GetCertificateStatusExtensions.TryParse,
                                         out GetCertificateStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region OCSPResult    [optional/mandatory]

                if (!JSON.ParseMandatory("ocspResult",
                                         "OCSP result",
                                         OCPPv2_1.OCSPResult.TryParse,
                                         out OCSPResult OCSPResult,
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


                GetCertificateStatusResponse = new GetCertificateStatusResponse(

                                                   Request,
                                                   Status,
                                                   OCSPResult,
                                                   StatusInfo,
                                                   ResponseTimestamp,

                                                   DestinationId,
                                                   NetworkPath,

                                                   null,
                                                   null,
                                                   Signatures,

                                                   CustomData

                                               );

                if (CustomGetCertificateStatusResponseParser is not null)
                    GetCertificateStatusResponse = CustomGetCertificateStatusResponseParser(JSON,
                                                                                            GetCertificateStatusResponse);

                return true;

            }
            catch (Exception e)
            {
                GetCertificateStatusResponse  = null;
                ErrorResponse                 = "The given JSON representation of a GetCertificateStatus response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetCertificateStatusResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetCertificateStatusResponseSerializer">A delegate to serialize custom GetCertificateStatus responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetCertificateStatusResponse>?  CustomGetCertificateStatusResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                    CustomStatusInfoSerializer                     = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.     AsText()),
                                 new JProperty("ocspResult",   OCSPResult. ToString()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo. ToJSON(CustomStatusInfoSerializer,
                                                                                  CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetCertificateStatusResponseSerializer is not null
                       ? CustomGetCertificateStatusResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetCertificateStatus failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetCertificateStatus request.</param>
        public static GetCertificateStatusResponse RequestError(CS.GetCertificateStatusRequest  Request,
                                                                EventTracking_Id                EventTrackingId,
                                                                ResultCode                      ErrorCode,
                                                                String?                         ErrorDescription    = null,
                                                                JObject?                        ErrorDetails        = null,
                                                                DateTime?                       ResponseTimestamp   = null,

                                                                NetworkingNode_Id?              DestinationId       = null,
                                                                NetworkPath?                    NetworkPath         = null,

                                                                IEnumerable<KeyPair>?           SignKeys            = null,
                                                                IEnumerable<SignInfo>?          SignInfos           = null,
                                                                IEnumerable<Signature>?         Signatures          = null,

                                                                CustomData?                     CustomData          = null)

            => new (

                   Request,
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
        /// The GetCertificateStatus failed.
        /// </summary>
        /// <param name="Request">The GetCertificateStatus request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetCertificateStatusResponse FormationViolation(CS.GetCertificateStatusRequest  Request,
                                                                      String                          ErrorDescription)

            => new (Request,
                    Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The GetCertificateStatus failed.
        /// </summary>
        /// <param name="Request">The GetCertificateStatus request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetCertificateStatusResponse SignatureError(CS.GetCertificateStatusRequest  Request,
                                                                  String                          ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The GetCertificateStatus failed.
        /// </summary>
        /// <param name="Request">The GetCertificateStatus request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetCertificateStatusResponse Failed(CS.GetCertificateStatusRequest  Request,
                                                          String?                         Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The GetCertificateStatus failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetCertificateStatus request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetCertificateStatusResponse ExceptionOccured(CS.GetCertificateStatusRequest  Request,
                                                                    Exception                       Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetCertificateStatusResponse1, GetCertificateStatusResponse2)

        /// <summary>
        /// Compares two GetCertificateStatus responses for equality.
        /// </summary>
        /// <param name="GetCertificateStatusResponse1">A GetCertificateStatus response.</param>
        /// <param name="GetCertificateStatusResponse2">Another GetCertificateStatus response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetCertificateStatusResponse? GetCertificateStatusResponse1,
                                           GetCertificateStatusResponse? GetCertificateStatusResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetCertificateStatusResponse1, GetCertificateStatusResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetCertificateStatusResponse1 is null || GetCertificateStatusResponse2 is null)
                return false;

            return GetCertificateStatusResponse1.Equals(GetCertificateStatusResponse2);

        }

        #endregion

        #region Operator != (GetCertificateStatusResponse1, GetCertificateStatusResponse2)

        /// <summary>
        /// Compares two GetCertificateStatus responses for inequality.
        /// </summary>
        /// <param name="GetCertificateStatusResponse1">A GetCertificateStatus response.</param>
        /// <param name="GetCertificateStatusResponse2">Another GetCertificateStatus response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetCertificateStatusResponse? GetCertificateStatusResponse1,
                                           GetCertificateStatusResponse? GetCertificateStatusResponse2)

            => !(GetCertificateStatusResponse1 == GetCertificateStatusResponse2);

        #endregion

        #endregion

        #region IEquatable<GetCertificateStatusResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetCertificateStatus responses for equality.
        /// </summary>
        /// <param name="Object">A GetCertificateStatus response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetCertificateStatusResponse getCertificateStatusResponse &&
                   Equals(getCertificateStatusResponse);

        #endregion

        #region Equals(GetCertificateStatusResponse)

        /// <summary>
        /// Compares two GetCertificateStatus responses for equality.
        /// </summary>
        /// <param name="GetCertificateStatusResponse">A GetCertificateStatus response to compare with.</param>
        public override Boolean Equals(GetCertificateStatusResponse? GetCertificateStatusResponse)

            => GetCertificateStatusResponse is not null &&

               Status.     Equals(GetCertificateStatusResponse.Status)     &&
               OCSPResult. Equals(GetCertificateStatusResponse.OCSPResult) &&

             ((StatusInfo is     null && GetCertificateStatusResponse.StatusInfo is     null) ||
               StatusInfo is not null && GetCertificateStatusResponse.StatusInfo is not null && StatusInfo.Equals(GetCertificateStatusResponse.StatusInfo)) &&

               base.GenericEquals(GetCertificateStatusResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Status.     GetHashCode()       * 7 ^
                       OCSPResult. GetHashCode()       * 5 ^
                      (StatusInfo?.GetHashCode() ?? 0) * 3 ^

                       base.       GetHashCode();

            }
        }

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
