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
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The GetCertificateStatus response.
    /// </summary>
    public class GetCertificateStatusResponse : AResponse<GetCertificateStatusRequest,
                                                          GetCertificateStatusResponse>,
                                                IResponse<Result>
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

        /// <summary>
        /// Create a new GetCertificateStatus response.
        /// </summary>
        /// <param name="Request">The GetCertificateStatus request leading to this response.</param>
        /// <param name="Status">Whether the central system was able to retrieve the OCSP certificate status.</param>
        /// <param name="OCSPResult">The optional DER encoded and then base64 OCSP response as defined in IETF RFC 6960. MAY only be omitted when status was not "Accepted".</param>
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
        public GetCertificateStatusResponse(GetCertificateStatusRequest  Request,
                                            GetCertificateStatus         Status,
                                            OCSPResult                   OCSPResult,
                                            StatusInfo?                  StatusInfo            = null,

                                            Result?                      Result                = null,
                                            DateTimeOffset?              ResponseTimestamp     = null,

                                            SourceRouting?               Destination           = null,
                                            NetworkPath?                 NetworkPath           = null,

                                            IEnumerable<KeyPair>?        SignKeys              = null,
                                            IEnumerable<SignInfo>?       SignInfos             = null,
                                            IEnumerable<Signature>?      Signatures            = null,

                                            CustomData?                  CustomData            = null,

                                            SerializationFormats?        SerializationFormat   = null,
                                            CancellationToken            CancellationToken     = default)

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
            this.OCSPResult  = OCSPResult;
            this.StatusInfo  = StatusInfo;

            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 7 ^
                           this.OCSPResult. GetHashCode()       * 5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^

                           base.            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:GetCertificateStatusResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "GetCertificateStatusEnumType": {
        //             "description": "This indicates whether the charging station was able to retrieve the OCSP certificate status.",
        //             "javaType": "GetCertificateStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Failed"
        //             ]
        //         },
        //         "StatusInfoType": {
        //             "description": "Element providing more information about the status.",
        //             "javaType": "StatusInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "reasonCode": {
        //                     "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "additionalInfo": {
        //                     "description": "Additional text to provide detailed information.",
        //                     "type": "string",
        //                     "maxLength": 1024
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "reasonCode"
        //             ]
        //         },
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "status": {
        //             "$ref": "#/definitions/GetCertificateStatusEnumType"
        //         },
        //         "statusInfo": {
        //             "$ref": "#/definitions/StatusInfoType"
        //         },
        //         "ocspResult": {
        //             "description": "OCSPResponse class as defined in &lt;&lt;ref-ocpp_security_24, IETF RFC 6960&gt;&gt;. DER encoded (as defined in &lt;&lt;ref-ocpp_security_24, IETF RFC 6960&gt;&gt;), and then base64 encoded. MAY only be omitted when status is not Accepted. +\r\nThe minimum supported length is 18000. If a longer _ocspResult_ is supported, then the supported length must be communicated in variable OCPPCommCtrlr.FieldLength[ \"GetCertificateStatusResponse.ocspResult\" ].",
        //             "type": "string",
        //             "maxLength": 18000
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetCertificateStatusResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetCertificateStatus response.
        /// </summary>
        /// <param name="Request">The GetCertificateStatus request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetCertificateStatusResponseParser">A delegate to parse custom GetCertificateStatus responses.</param>
        public static GetCertificateStatusResponse Parse(GetCertificateStatusRequest                                 Request,
                                                         JObject                                                     JSON,
                                                         SourceRouting                                               Destination,
                                                         NetworkPath                                                 NetworkPath,
                                                         DateTimeOffset?                                             ResponseTimestamp                          = null,
                                                         CustomJObjectParserDelegate<GetCertificateStatusResponse>?  CustomGetCertificateStatusResponseParser   = null,
                                                         CustomJObjectParserDelegate<StatusInfo>?                    CustomStatusInfoParser                     = null,
                                                         CustomJObjectParserDelegate<Signature>?                     CustomSignatureParser                      = null,
                                                         CustomJObjectParserDelegate<CustomData>?                    CustomCustomDataParser                     = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
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
        public static Boolean TryParse(GetCertificateStatusRequest                                 Request,
                                       JObject                                                     JSON,
                                       SourceRouting                                               Destination,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out GetCertificateStatusResponse?      GetCertificateStatusResponse,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       DateTimeOffset?                                             ResponseTimestamp                          = null,
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
                                           WWCP.CustomData.TryParse,
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

                                                   null,
                                                   ResponseTimestamp,

                                                   Destination,
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
        public JObject ToJSON(Boolean                                                         IncludeJSONLDContext                           = false,
                              CustomJObjectSerializerDelegate<GetCertificateStatusResponse>?  CustomGetCertificateStatusResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                    CustomStatusInfoSerializer                     = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("status",       Status.              AsText()),
                                 new JProperty("ocspResult",   OCSPResult.          ToString()),

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
        public static GetCertificateStatusResponse RequestError(GetCertificateStatusRequest  Request,
                                                                EventTracking_Id             EventTrackingId,
                                                                ResultCode                   ErrorCode,
                                                                String?                      ErrorDescription    = null,
                                                                JObject?                     ErrorDetails        = null,
                                                                DateTimeOffset?              ResponseTimestamp   = null,

                                                                SourceRouting?               Destination         = null,
                                                                NetworkPath?                 NetworkPath         = null,

                                                                IEnumerable<KeyPair>?        SignKeys            = null,
                                                                IEnumerable<SignInfo>?       SignInfos           = null,
                                                                IEnumerable<Signature>?      Signatures          = null,

                                                                CustomData?                  CustomData          = null)

            => new (

                   Request,
                   GetCertificateStatus.Failed,
                   OCPPv2_1.OCSPResult.Empty,
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
        /// The GetCertificateStatus failed.
        /// </summary>
        /// <param name="Request">The GetCertificateStatus request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetCertificateStatusResponse FormationViolation(GetCertificateStatusRequest  Request,
                                                                      String                       ErrorDescription)

            => new (Request,
                    GetCertificateStatus.Failed,
                    OCPPv2_1.OCSPResult.Empty,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetCertificateStatus failed.
        /// </summary>
        /// <param name="Request">The GetCertificateStatus request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetCertificateStatusResponse SignatureError(GetCertificateStatusRequest  Request,
                                                                  String                       ErrorDescription)

            => new (Request,
                    GetCertificateStatus.Failed,
                    OCPPv2_1.OCSPResult.Empty,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetCertificateStatus failed.
        /// </summary>
        /// <param name="Request">The GetCertificateStatus request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetCertificateStatusResponse Failed(GetCertificateStatusRequest  Request,
                                                          String?                      Description   = null)

            => new (Request,
                    GetCertificateStatus.Failed,
                    OCPPv2_1.OCSPResult.Empty,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The GetCertificateStatus failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetCertificateStatus request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetCertificateStatusResponse ExceptionOccurred(GetCertificateStatusRequest  Request,
                                                                    Exception                    Exception)

            => new (Request,
                    GetCertificateStatus.Failed,
                    OCPPv2_1.OCSPResult.Empty,
                    Result:  Result.FromException(Exception));

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
