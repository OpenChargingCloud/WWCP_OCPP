﻿/*
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
    /// The Get15118EVCertificate response.
    /// </summary>
    public class Get15118EVCertificateResponse : AResponse<Get15118EVCertificateRequest,
                                                           Get15118EVCertificateResponse>,
                                                 IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/get15118EVCertificateResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the EXI message processing.
        /// </summary>
        [Mandatory]
        public ISO15118EVCertificateStatus  Status                { get; }

        /// <summary>
        /// Base64 encoded certificate installation response to the electric vehicle.
        /// [max 5600]
        /// </summary>
        [Mandatory]
        public EXIData                      EXIResponse           { get; }

        /// <summary>
        /// The number of contracts that can be retrieved with additional requests.
        /// </summary>
        [Optional]
        public UInt32?                      RemainingContracts    { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?                  StatusInfo            { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Get15118EVCertificate response.
        /// </summary>
        /// <param name="Request">The Get15118EVCertificate request leading to this response.</param>
        /// <param name="Status">The success or failure of the EXI message processing.</param>
        /// <param name="EXIResponse">Base64 encoded certificate installation response to the electric vehicle.</param>
        /// <param name="RemainingContracts">The number of contracts that can be retrieved with additional requests.</param>
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
        public Get15118EVCertificateResponse(Get15118EVCertificateRequest  Request,
                                             ISO15118EVCertificateStatus   Status,
                                             EXIData                       EXIResponse,
                                             UInt32?                       RemainingContracts    = null,
                                             StatusInfo?                   StatusInfo            = null,

                                             Result?                       Result                = null,
                                             DateTime?                     ResponseTimestamp     = null,

                                             SourceRouting?                Destination           = null,
                                             NetworkPath?                  NetworkPath           = null,

                                             IEnumerable<KeyPair>?         SignKeys              = null,
                                             IEnumerable<SignInfo>?        SignInfos             = null,
                                             IEnumerable<Signature>?       Signatures            = null,

                                             CustomData?                   CustomData            = null,

                                             SerializationFormats?         SerializationFormat   = null,
                                             CancellationToken             CancellationToken     = default)

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

            this.Status              = Status;
            this.EXIResponse         = EXIResponse;
            this.RemainingContracts  = RemainingContracts;
            this.StatusInfo          = StatusInfo;

            unchecked
            {

                hashCode = this.Status.             GetHashCode()       * 11 ^
                           this.EXIResponse.        GetHashCode()       *  7 ^
                          (this.RemainingContracts?.GetHashCode() ?? 0) *  5 ^
                          (this.StatusInfo?.        GetHashCode() ?? 0) *  3 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:Get15118EVCertificateResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "Iso15118EVCertificateStatusEnumType": {
        //             "description": "Indicates whether the message was processed properly.",
        //             "javaType": "Iso15118EVCertificateStatusEnum",
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
        //             "$ref": "#/definitions/Iso15118EVCertificateStatusEnumType"
        //         },
        //         "statusInfo": {
        //             "$ref": "#/definitions/StatusInfoType"
        //         },
        //         "exiResponse": {
        //             "description": "*(2/1)* Raw CertificateInstallationRes response for the EV, Base64 encoded. +\r\nExtended to support ISO 15118-20 certificates. The minimum supported length is 17000. If a longer _exiResponse_ is supported, then the supported length must be communicated in variable OCPPCommCtrlr.FieldLength[ \"Get15118EVCertificateResponse.exiResponse\" ].",
        //             "type": "string",
        //             "maxLength": 17000
        //         },
        //         "remainingContracts": {
        //             "description": "*(2.1)* Number of contracts that can be retrieved with additional requests.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "status",
        //         "exiResponse"
        //     ]
        // }
        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a Get15118EVCertificate response.
        /// </summary>
        /// <param name="Request">The Get15118EVCertificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGet15118EVCertificateResponseParser">A delegate to parse custom Get15118EVCertificate responses.</param>
        public static Get15118EVCertificateResponse Parse(CS.Get15118EVCertificateRequest                              Request,
                                                          JObject                                                      JSON,
                                                          SourceRouting                                                Destination,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTime?                                                    ResponseTimestamp                           = null,
                                                          CustomJObjectParserDelegate<Get15118EVCertificateResponse>?  CustomGet15118EVCertificateResponseParser   = null,
                                                          CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                                          CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                                          CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var get15118EVCertificateResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGet15118EVCertificateResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return get15118EVCertificateResponse;
            }

            throw new ArgumentException("The given JSON representation of a Get15118EVCertificate response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out Get15118EVCertificateResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a Get15118EVCertificate response.
        /// </summary>
        /// <param name="Request">The Get15118EVCertificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Get15118EVCertificateResponse">The parsed Get15118EVCertificate response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGet15118EVCertificateResponseParser">A delegate to parse custom Get15118EVCertificate responses.</param>
        public static Boolean TryParse(CS.Get15118EVCertificateRequest                              Request,
                                       JObject                                                      JSON,
                                       SourceRouting                                                Destination,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out Get15118EVCertificateResponse?      Get15118EVCertificateResponse,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTime?                                                    ResponseTimestamp                           = null,
                                       CustomJObjectParserDelegate<Get15118EVCertificateResponse>?  CustomGet15118EVCertificateResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                       CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                       CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            try
            {

                Get15118EVCertificateResponse = null;

                #region Status                [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "ISO 15118 EV certificate status",
                                         ISO15118EVCertificateStatusExtensions.TryParse,
                                         out ISO15118EVCertificateStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EXIRequest            [mandatory]

                if (!JSON.ParseMandatory("exiResponse",
                                         "EXI response",
                                         EXIData.TryParse,
                                         out EXIData EXIRequest,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region RemainingContracts    [optional]

                if (JSON.ParseOptional("remainingContracts",
                                       "remaining contracts",
                                       out UInt32? RemainingContracts,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StatusInfo            [optional]

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

                #region Signatures            [optional, OCPP_CSE]

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

                #region CustomData            [optional]

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


                Get15118EVCertificateResponse = new Get15118EVCertificateResponse(

                                                    Request,
                                                    Status,
                                                    EXIRequest,
                                                    RemainingContracts,
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

                if (CustomGet15118EVCertificateResponseParser is not null)
                    Get15118EVCertificateResponse = CustomGet15118EVCertificateResponseParser(JSON,
                                                                                              Get15118EVCertificateResponse);

                return true;

            }
            catch (Exception e)
            {
                Get15118EVCertificateResponse  = null;
                ErrorResponse                  = "The given JSON representation of a Get15118EVCertificate response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGet15118EVCertificateResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGet15118EVCertificateResponseSerializer">A delegate to serialize custom Get15118EVCertificate responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                          IncludeJSONLDContext                            = false,
                              CustomJObjectSerializerDelegate<Get15118EVCertificateResponse>?  CustomGet15118EVCertificateResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                     CustomStatusInfoSerializer                      = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",             DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("status",               Status.              AsText()),
                                 new JProperty("exiResponse",          EXIResponse.         ToString()),

                           RemainingContracts is not null
                               ? new JProperty("remainingContracts",   RemainingContracts.Value)
                               : null,

                           StatusInfo is not null
                               ? new JProperty("statusInfo",           StatusInfo.          ToJSON(CustomStatusInfoSerializer,
                                                                                                   CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",           new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                  CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",           CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGet15118EVCertificateResponseSerializer is not null
                       ? CustomGet15118EVCertificateResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The Get15118EVCertificate failed because of a request error.
        /// </summary>
        /// <param name="Request">The Get15118EVCertificate request.</param>
        public static Get15118EVCertificateResponse RequestError(Get15118EVCertificateRequest  Request,
                                                                 EventTracking_Id              EventTrackingId,
                                                                 ResultCode                    ErrorCode,
                                                                 String?                       ErrorDescription    = null,
                                                                 JObject?                      ErrorDetails        = null,
                                                                 DateTime?                     ResponseTimestamp   = null,

                                                                 SourceRouting?                Destination         = null,
                                                                 NetworkPath?                  NetworkPath         = null,

                                                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                                                 IEnumerable<Signature>?       Signatures          = null,

                                                                 CustomData?                   CustomData          = null)

            => new (

                   Request,
                   ISO15118EVCertificateStatus.Failed,
                   EXIData.Empty,
                   null,
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
        /// The Get15118EVCertificate failed.
        /// </summary>
        /// <param name="Request">The Get15118EVCertificate request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static Get15118EVCertificateResponse FormationViolation(Get15118EVCertificateRequest  Request,
                                                                       String                        ErrorDescription)

            => new (Request,
                    ISO15118EVCertificateStatus.Failed,
                    EXIData.Empty,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The Get15118EVCertificate failed.
        /// </summary>
        /// <param name="Request">The Get15118EVCertificate request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static Get15118EVCertificateResponse SignatureError(Get15118EVCertificateRequest  Request,
                                                                   String                        ErrorDescription)

            => new (Request,
                    ISO15118EVCertificateStatus.Failed,
                    EXIData.Empty,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The Get15118EVCertificate failed.
        /// </summary>
        /// <param name="Request">The Get15118EVCertificate request.</param>
        /// <param name="Description">An optional error description.</param>
        public static Get15118EVCertificateResponse Failed(Get15118EVCertificateRequest  Request,
                                                           String?                       Description   = null)

            => new (Request,
                    ISO15118EVCertificateStatus.Failed,
                    EXIData.Empty,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The Get15118EVCertificate failed because of an exception.
        /// </summary>
        /// <param name="Request">The Get15118EVCertificate request.</param>
        /// <param name="Exception">The exception.</param>
        public static Get15118EVCertificateResponse ExceptionOccured(Get15118EVCertificateRequest  Request,
                                                                     Exception                     Exception)

            => new (Request,
                    ISO15118EVCertificateStatus.Failed,
                    EXIData.Empty,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (Get15118EVCertificateResponse1, Get15118EVCertificateResponse2)

        /// <summary>
        /// Compares two Get15118EVCertificate responses for equality.
        /// </summary>
        /// <param name="Get15118EVCertificateResponse1">A Get15118EVCertificate response.</param>
        /// <param name="Get15118EVCertificateResponse2">Another Get15118EVCertificate response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (Get15118EVCertificateResponse? Get15118EVCertificateResponse1,
                                           Get15118EVCertificateResponse? Get15118EVCertificateResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(Get15118EVCertificateResponse1, Get15118EVCertificateResponse2))
                return true;

            // If one is null, but not both, return false.
            if (Get15118EVCertificateResponse1 is null || Get15118EVCertificateResponse2 is null)
                return false;

            return Get15118EVCertificateResponse1.Equals(Get15118EVCertificateResponse2);

        }

        #endregion

        #region Operator != (Get15118EVCertificateResponse1, Get15118EVCertificateResponse2)

        /// <summary>
        /// Compares two Get15118EVCertificate responses for inequality.
        /// </summary>
        /// <param name="Get15118EVCertificateResponse1">A Get15118EVCertificate response.</param>
        /// <param name="Get15118EVCertificateResponse2">Another Get15118EVCertificate response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Get15118EVCertificateResponse? Get15118EVCertificateResponse1,
                                           Get15118EVCertificateResponse? Get15118EVCertificateResponse2)

            => !(Get15118EVCertificateResponse1 == Get15118EVCertificateResponse2);

        #endregion

        #endregion

        #region IEquatable<Get15118EVCertificateResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Get15118EVCertificate responses for equality.
        /// </summary>
        /// <param name="Object">A Get15118EVCertificate response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Get15118EVCertificateResponse get15118EVCertificateResponse &&
                   Equals(get15118EVCertificateResponse);

        #endregion

        #region Equals(Get15118EVCertificateResponse)

        /// <summary>
        /// Compares two Get15118EVCertificate responses for equality.
        /// </summary>
        /// <param name="Get15118EVCertificateResponse">A Get15118EVCertificate response to compare with.</param>
        public override Boolean Equals(Get15118EVCertificateResponse? Get15118EVCertificateResponse)

            => Get15118EVCertificateResponse is not null &&

               Status.     Equals(Get15118EVCertificateResponse.Status)      &&
               EXIResponse.Equals(Get15118EVCertificateResponse.EXIResponse) &&

             ((RemainingContracts is     null && Get15118EVCertificateResponse.RemainingContracts is null) ||
               RemainingContracts is not null && Get15118EVCertificateResponse.RemainingContracts is not null && RemainingContracts.Value.Equals(Get15118EVCertificateResponse.RemainingContracts.Value)) &&

             ((StatusInfo         is     null && Get15118EVCertificateResponse.StatusInfo         is     null) ||
               StatusInfo         is not null && Get15118EVCertificateResponse.StatusInfo         is not null && StatusInfo.              Equals(Get15118EVCertificateResponse.StatusInfo)) &&

               base.GenericEquals(Get15118EVCertificateResponse);

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
