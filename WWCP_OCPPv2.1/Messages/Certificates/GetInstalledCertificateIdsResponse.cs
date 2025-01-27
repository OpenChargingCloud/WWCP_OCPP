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
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The GetInstalledCertificateIds response.
    /// </summary>
    public class GetInstalledCertificateIdsResponse : AResponse<GetInstalledCertificateIdsRequest,
                                                                GetInstalledCertificateIdsResponse>,
                                                      IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getInstalledCertificateIdsResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                          Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The Charge Point indicates if it can process the request.
        /// </summary>
        [Mandatory]
        public GetInstalledCertificateStatus          Status                      { get; }

        /// <summary>
        /// The optional enumeration of information about available certificates.
        /// </summary>
        [Mandatory]
        public IEnumerable<CertificateHashDataChain>  CertificateHashDataChain    { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?                            StatusInfo                  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetInstalledCertificateIds response.
        /// </summary>
        /// <param name="Request">The GetInstalledCertificateIds request leading to this response.</param>
        /// <param name="Status">The Charge Point indicates if it can process the request.</param>
        /// <param name="CertificateHashDataChain">An optional enumeration of information about available certificates.</param>
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
        public GetInstalledCertificateIdsResponse(CSMS.GetInstalledCertificateIdsRequest  Request,
                                                  GetInstalledCertificateStatus           Status,
                                                  IEnumerable<CertificateHashDataChain>?  CertificateHashDataChain   = null,
                                                  StatusInfo?                             StatusInfo                 = null,

                                                  Result?                                 Result                     = null,
                                                  DateTime?                               ResponseTimestamp          = null,

                                                  SourceRouting?                          Destination                = null,
                                                  NetworkPath?                            NetworkPath                = null,

                                                  IEnumerable<KeyPair>?                   SignKeys                   = null,
                                                  IEnumerable<SignInfo>?                  SignInfos                  = null,
                                                  IEnumerable<Signature>?                 Signatures                 = null,

                                                  CustomData?                             CustomData                 = null,

                                                  SerializationFormats?                   SerializationFormat        = null,
                                                  CancellationToken                       CancellationToken          = default)

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

            this.Status                    = Status;
            this.CertificateHashDataChain  = CertificateHashDataChain?.Distinct() ?? [];
            this.StatusInfo                = StatusInfo;

            unchecked
            {

                hashCode = this.Status.                  GetHashCode()       * 7 ^
                           this.CertificateHashDataChain.GetHashCode()       * 5 ^
                          (this.StatusInfo?.             GetHashCode() ?? 0) * 3 ^

                           base.                         GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetInstalledCertificateIdsResponse",
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
        //     "GetCertificateIdUseEnumType": {
        //       "description": "Indicates the type of the requested certificate(s).",
        //       "javaType": "GetCertificateIdUseEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "V2GRootCertificate",
        //         "MORootCertificate",
        //         "CSMSRootCertificate",
        //         "V2GCertificateChain",
        //         "ManufacturerRootCertificate"
        //       ]
        //     },
        //     "GetInstalledCertificateStatusEnumType": {
        //       "description": "Charging Station indicates if it can process the request.",
        //       "javaType": "GetInstalledCertificateStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "NotFound"
        //       ]
        //     },
        //     "HashAlgorithmEnumType": {
        //       "description": "Used algorithms for the hashes provided.",
        //       "javaType": "HashAlgorithmEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "SHA256",
        //         "SHA384",
        //         "SHA512"
        //       ]
        //     },
        //     "CertificateHashDataChainType": {
        //       "javaType": "CertificateHashDataChain",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "certificateHashData": {
        //           "$ref": "#/definitions/CertificateHashDataType"
        //         },
        //         "certificateType": {
        //           "$ref": "#/definitions/GetCertificateIdUseEnumType"
        //         },
        //         "childCertificateHashData": {
        //           "type": "array",
        //           "additionalItems": false,
        //           "items": {
        //             "$ref": "#/definitions/CertificateHashDataType"
        //           },
        //           "minItems": 1,
        //           "maxItems": 4
        //         }
        //       },
        //       "required": [
        //         "certificateType",
        //         "certificateHashData"
        //       ]
        //     },
        //     "CertificateHashDataType": {
        //       "javaType": "CertificateHashData",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "hashAlgorithm": {
        //           "$ref": "#/definitions/HashAlgorithmEnumType"
        //         },
        //         "issuerNameHash": {
        //           "description": "Hashed value of the Issuer DN (Distinguished Name).\r\n\r\n",
        //           "type": "string",
        //           "maxLength": 128
        //         },
        //         "issuerKeyHash": {
        //           "description": "Hashed value of the issuers public key\r\n",
        //           "type": "string",
        //           "maxLength": 128
        //         },
        //         "serialNumber": {
        //           "description": "The serial number of the certificate.",
        //           "type": "string",
        //           "maxLength": 40
        //         }
        //       },
        //       "required": [
        //         "hashAlgorithm",
        //         "issuerNameHash",
        //         "issuerKeyHash",
        //         "serialNumber"
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
        //       "$ref": "#/definitions/GetInstalledCertificateStatusEnumType"
        //     },
        //     "statusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     },
        //     "certificateHashDataChain": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/CertificateHashDataChainType"
        //       },
        //       "minItems": 1
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetInstalledCertificateIdsResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetInstalledCertificateIds response.
        /// </summary>
        /// <param name="Request">The GetInstalledCertificateIds request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetInstalledCertificateIdsResponseParser">A delegate to parse custom GetInstalledCertificateIds responses.</param>
        public static GetInstalledCertificateIdsResponse Parse(CSMS.GetInstalledCertificateIdsRequest                            Request,
                                                               JObject                                                           JSON,
                                                               SourceRouting                                                 Destination,
                                                               NetworkPath                                                       NetworkPath,
                                                               DateTime?                                                         ResponseTimestamp                                = null,
                                                               CustomJObjectParserDelegate<GetInstalledCertificateIdsResponse>?  CustomGetInstalledCertificateIdsResponseParser   = null,
                                                               CustomJObjectParserDelegate<CertificateHashData>?                 CustomCertificateHashDataParser                  = null,
                                                               CustomJObjectParserDelegate<StatusInfo>?                          CustomStatusInfoParser                           = null,
                                                               CustomJObjectParserDelegate<Signature>?                           CustomSignatureParser                            = null,
                                                               CustomJObjectParserDelegate<CustomData>?                          CustomCustomDataParser                           = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var getInstalledCertificateIdsResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetInstalledCertificateIdsResponseParser,
                         CustomCertificateHashDataParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getInstalledCertificateIdsResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetInstalledCertificateIds response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetInstalledCertificateIdsResponse, out ErrorResponse, CustomGetInstalledCertificateIdsResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetInstalledCertificateIds response.
        /// </summary>
        /// <param name="Request">The GetInstalledCertificateIds request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetInstalledCertificateIdsResponse">The parsed GetInstalledCertificateIds response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetInstalledCertificateIdsResponseParser">A delegate to parse custom GetInstalledCertificateIds responses.</param>
        public static Boolean TryParse(CSMS.GetInstalledCertificateIdsRequest                            Request,
                                       JObject                                                           JSON,
                                       SourceRouting                                                 Destination,
                                       NetworkPath                                                       NetworkPath,
                                       [NotNullWhen(true)]  out GetInstalledCertificateIdsResponse?      GetInstalledCertificateIdsResponse,
                                       [NotNullWhen(false)] out String?                                  ErrorResponse,
                                       DateTime?                                                         ResponseTimestamp                                = null,
                                       CustomJObjectParserDelegate<GetInstalledCertificateIdsResponse>?  CustomGetInstalledCertificateIdsResponseParser   = null,
                                       CustomJObjectParserDelegate<CertificateHashData>?                 CustomCertificateHashDataParser                  = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                          CustomStatusInfoParser                           = null,
                                       CustomJObjectParserDelegate<Signature>?                           CustomSignatureParser                            = null,
                                       CustomJObjectParserDelegate<CustomData>?                          CustomCustomDataParser                           = null)
        {

            try
            {

                GetInstalledCertificateIdsResponse = null;

                #region Status                      [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "status",
                                         GetInstalledCertificateStatusExtensions.TryParse,
                                         out GetInstalledCertificateStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CertificateHashDataChain    [mandatory]

                if (!JSON.ParseMandatoryJSON("certificateHashDataChain",
                                             "certificate hash data chain",
                                             OCPPv2_1.CertificateHashDataChain.TryParse,
                                             out IEnumerable<CertificateHashDataChain> CertificateHashDataChain,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo                  [optional]

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

                #region Signatures                  [optional, OCPP_CSE]

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

                #region CustomData                  [optional]

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


                GetInstalledCertificateIdsResponse = new GetInstalledCertificateIdsResponse(

                                                         Request,
                                                         Status,
                                                         CertificateHashDataChain,
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

                if (CustomGetInstalledCertificateIdsResponseParser is not null)
                    GetInstalledCertificateIdsResponse = CustomGetInstalledCertificateIdsResponseParser(JSON,
                                                                                                        GetInstalledCertificateIdsResponse);

                return true;

            }
            catch (Exception e)
            {
                GetInstalledCertificateIdsResponse  = null;
                ErrorResponse                       = "The given JSON representation of a GetInstalledCertificateIds response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetInstalledCertificateIdsResponseSerializer = null, CustomCertificateHashDataChainSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetInstalledCertificateIdsResponseSerializer">A delegate to serialize custom GetInstalledCertificateIds responses.</param>
        /// <param name="CustomCertificateHashDataChainSerializer">A delegate to serialize custom CertificateHashDataChains.</param>
        /// <param name="CustomCertificateHashDataSerializer">A delegate to serialize custom CertificateHashData.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                               IncludeJSONLDContext                                 = false,
                              CustomJObjectSerializerDelegate<GetInstalledCertificateIdsResponse>?  CustomGetInstalledCertificateIdsResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CertificateHashDataChain>?            CustomCertificateHashDataChainSerializer             = null,
                              CustomJObjectSerializerDelegate<CertificateHashData>?                 CustomCertificateHashDataSerializer                  = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                          CustomStatusInfoSerializer                           = null,
                              CustomJObjectSerializerDelegate<Signature>?                           CustomSignatureSerializer                            = null,
                              CustomJObjectSerializerDelegate<CustomData>?                          CustomCustomDataSerializer                           = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",                   DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("status",                     Status.              AsText()),
                                 new JProperty("certificateHashDataChain",   new JArray(CertificateHashDataChain.Select(certificateHashDataChain => certificateHashDataChain.ToJSON(CustomCertificateHashDataChainSerializer,
                                                                                                                                                                                    CustomCertificateHashDataSerializer)))),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",                 StatusInfo.          ToJSON(CustomStatusInfoSerializer,
                                                                                                         CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",                 new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                        CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                 CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetInstalledCertificateIdsResponseSerializer is not null
                       ? CustomGetInstalledCertificateIdsResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetInstalledCertificateIds failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetInstalledCertificateIds request.</param>
        public static GetInstalledCertificateIdsResponse RequestError(CSMS.GetInstalledCertificateIdsRequest  Request,
                                                                      EventTracking_Id                        EventTrackingId,
                                                                      ResultCode                              ErrorCode,
                                                                      String?                                 ErrorDescription    = null,
                                                                      JObject?                                ErrorDetails        = null,
                                                                      DateTime?                               ResponseTimestamp   = null,

                                                                      SourceRouting?                          Destination         = null,
                                                                      NetworkPath?                            NetworkPath         = null,

                                                                      IEnumerable<KeyPair>?                   SignKeys            = null,
                                                                      IEnumerable<SignInfo>?                  SignInfos           = null,
                                                                      IEnumerable<Signature>?                 Signatures          = null,

                                                                      CustomData?                             CustomData          = null)

            => new (

                   Request,
                   GetInstalledCertificateStatus.NotFound,
                   null,
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
        /// The GetInstalledCertificateIds failed.
        /// </summary>
        /// <param name="Request">The GetInstalledCertificateIds request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetInstalledCertificateIdsResponse FormationViolation(CSMS.GetInstalledCertificateIdsRequest  Request,
                                                                            String                                  ErrorDescription)

            => new (Request,
                    GetInstalledCertificateStatus.NotFound,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetInstalledCertificateIds failed.
        /// </summary>
        /// <param name="Request">The GetInstalledCertificateIds request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetInstalledCertificateIdsResponse SignatureError(CSMS.GetInstalledCertificateIdsRequest  Request,
                                                                        String                                  ErrorDescription)

            => new (Request,
                    GetInstalledCertificateStatus.NotFound,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetInstalledCertificateIds failed.
        /// </summary>
        /// <param name="Request">The GetInstalledCertificateIds request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetInstalledCertificateIdsResponse Failed(CSMS.GetInstalledCertificateIdsRequest  Request,
                                                                String?                                 Description   = null)

            => new (Request,
                    GetInstalledCertificateStatus.NotFound,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The GetInstalledCertificateIds failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetInstalledCertificateIds request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetInstalledCertificateIdsResponse ExceptionOccured(CSMS.GetInstalledCertificateIdsRequest  Request,
                                                                          Exception                               Exception)

            => new (Request,
                    GetInstalledCertificateStatus.NotFound,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetInstalledCertificateIdsResponse1, GetInstalledCertificateIdsResponse2)

        /// <summary>
        /// Compares two GetInstalledCertificateIds responses for equality.
        /// </summary>
        /// <param name="GetInstalledCertificateIdsResponse1">A GetInstalledCertificateIds response.</param>
        /// <param name="GetInstalledCertificateIdsResponse2">Another GetInstalledCertificateIds response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetInstalledCertificateIdsResponse? GetInstalledCertificateIdsResponse1,
                                           GetInstalledCertificateIdsResponse? GetInstalledCertificateIdsResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetInstalledCertificateIdsResponse1, GetInstalledCertificateIdsResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetInstalledCertificateIdsResponse1 is null || GetInstalledCertificateIdsResponse2 is null)
                return false;

            return GetInstalledCertificateIdsResponse1.Equals(GetInstalledCertificateIdsResponse2);

        }

        #endregion

        #region Operator != (GetInstalledCertificateIdsResponse1, GetInstalledCertificateIdsResponse2)

        /// <summary>
        /// Compares two GetInstalledCertificateIds responses for inequality.
        /// </summary>
        /// <param name="GetInstalledCertificateIdsResponse1">A GetInstalledCertificateIds response.</param>
        /// <param name="GetInstalledCertificateIdsResponse2">Another GetInstalledCertificateIds response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetInstalledCertificateIdsResponse? GetInstalledCertificateIdsResponse1,
                                           GetInstalledCertificateIdsResponse? GetInstalledCertificateIdsResponse2)

            => !(GetInstalledCertificateIdsResponse1 == GetInstalledCertificateIdsResponse2);

        #endregion

        #endregion

        #region IEquatable<GetInstalledCertificateIdsResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetInstalledCertificateIds responses for equality.
        /// </summary>
        /// <param name="Object">A GetInstalledCertificateIds response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetInstalledCertificateIdsResponse getInstalledCertificateIdsResponse &&
                   Equals(getInstalledCertificateIdsResponse);

        #endregion

        #region Equals(GetInstalledCertificateIdsResponse)

        /// <summary>
        /// Compares two GetInstalledCertificateIds responses for equality.
        /// </summary>
        /// <param name="GetInstalledCertificateIdsResponse">A GetInstalledCertificateIds response to compare with.</param>
        public override Boolean Equals(GetInstalledCertificateIdsResponse? GetInstalledCertificateIdsResponse)

            => GetInstalledCertificateIdsResponse is not null &&

               Status.     Equals(GetInstalledCertificateIdsResponse.Status) &&

               CertificateHashDataChain.Count().Equals(GetInstalledCertificateIdsResponse.CertificateHashDataChain.Count()) &&
               CertificateHashDataChain.All(certificateHashData => GetInstalledCertificateIdsResponse.CertificateHashDataChain.Contains(certificateHashData)) &&

             ((StatusInfo is     null && GetInstalledCertificateIdsResponse.StatusInfo is     null) ||
               StatusInfo is not null && GetInstalledCertificateIdsResponse.StatusInfo is not null && StatusInfo.Equals(GetInstalledCertificateIdsResponse.StatusInfo)) &&

               base.GenericEquals(GetInstalledCertificateIdsResponse);

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
