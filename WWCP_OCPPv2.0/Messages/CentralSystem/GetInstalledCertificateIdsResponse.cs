/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.CP
{

    /// <summary>
    /// A get installed certificate ids response.
    /// </summary>
    public class GetInstalledCertificateIdsResponse : AResponse<CS.GetInstalledCertificateIdsRequest,
                                                                   GetInstalledCertificateIdsResponse>
    {

        #region Properties

        /// <summary>
        /// The Charge Point indicates if it can process the request.
        /// </summary>
        [Mandatory]
        public GetInstalledCertificateStatus     Status                      { get; }

        /// <summary>
        /// The optional enumeration of information about available certificates.
        /// </summary>
        [Mandatory]
        public IEnumerable<CertificateHashData>  CertificateHashDataChain    { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?                       StatusInfo                  { get; }

        #endregion

        #region Constructor(s)

        #region GetInstalledCertificateIdsResponse(Request, Status, CertificateHashDataChain, StatusInfo = null, CustomData = null)

        /// <summary>
        /// Create a new get installed certificate ids response.
        /// </summary>
        /// <param name="Request">The get installed certificate ids request leading to this response.</param>
        /// <param name="Status">The Charge Point indicates if it can process the request.</param>
        /// <param name="CertificateHashDataChain">An optional enumeration of information about available certificates.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public GetInstalledCertificateIdsResponse(CS.GetInstalledCertificateIdsRequest  Request,
                                                  GetInstalledCertificateStatus         Status,
                                                  IEnumerable<CertificateHashData>      CertificateHashDataChain,
                                                  StatusInfo?                           StatusInfo                 = null,
                                                  CustomData?                           CustomData                 = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            if (!CertificateHashDataChain.Any())
                throw new ArgumentException("The given enumeration of information about available certificates must not be empty!",
                                            nameof(CertificateHashDataChain));

            this.Status                    = Status;
            this.CertificateHashDataChain  = CertificateHashDataChain.Distinct();
            this.StatusInfo                = StatusInfo;

        }

        #endregion

        #region GetInstalledCertificateIdsResponse(Request, Result)

        /// <summary>
        /// Create a new get installed certificate ids response.
        /// </summary>
        /// <param name="Request">The get installed certificate ids request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetInstalledCertificateIdsResponse(CS.GetInstalledCertificateIdsRequest  Request,
                                                  Result                                Result)

            : base(Request,
                   Result)

        {

            this.Status                    = GetInstalledCertificateStatus.Unknown;
            this.CertificateHashDataChain  = Array.Empty<CertificateHashData>();

        }

        #endregion

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
        //       "description": "Indicates the type of the requested certificate(s).\r\n",
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
        //       "description": "Charging Station indicates if it can process the request.\r\n",
        //       "javaType": "GetInstalledCertificateStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "NotFound"
        //       ]
        //     },
        //     "HashAlgorithmEnumType": {
        //       "description": "Used algorithms for the hashes provided.\r\n",
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
        //           "description": "The serial number of the certificate.\r\n",
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
        //       "description": "Element providing more information about the status.\r\n",
        //       "javaType": "StatusInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "reasonCode": {
        //           "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "additionalInfo": {
        //           "description": "Additional text to provide detailed information.\r\n",
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
        /// Parse the given JSON representation of a get installed certificate ids response.
        /// </summary>
        /// <param name="Request">The get installed certificate ids request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetInstalledCertificateIdsResponseParser">A delegate to parse custom get installed certificate ids responses.</param>
        public static GetInstalledCertificateIdsResponse Parse(CS.GetInstalledCertificateIdsRequest                              Request,
                                                               JObject                                                           JSON,
                                                               CustomJObjectParserDelegate<GetInstalledCertificateIdsResponse>?  CustomGetInstalledCertificateIdsResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var getInstalledCertificateIdsResponse,
                         out var errorResponse,
                         CustomGetInstalledCertificateIdsResponseParser))
            {
                return getInstalledCertificateIdsResponse!;
            }

            throw new ArgumentException("The given JSON representation of a get installed certificate ids response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetInstalledCertificateIdsResponse, out ErrorResponse, CustomGetInstalledCertificateIdsResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get installed certificate ids response.
        /// </summary>
        /// <param name="Request">The get installed certificate ids request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetInstalledCertificateIdsResponse">The parsed get installed certificate ids response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetInstalledCertificateIdsResponseParser">A delegate to parse custom get installed certificate ids responses.</param>
        public static Boolean TryParse(CS.GetInstalledCertificateIdsRequest                              Request,
                                       JObject                                                           JSON,
                                       out GetInstalledCertificateIdsResponse?                           GetInstalledCertificateIdsResponse,
                                       out String?                                                       ErrorResponse,
                                       CustomJObjectParserDelegate<GetInstalledCertificateIdsResponse>?  CustomGetInstalledCertificateIdsResponseParser   = null)
        {

            try
            {

                GetInstalledCertificateIdsResponse = null;

                #region Status                      [mandatory]

                if (JSON.ParseMandatory("status",
                                        "status",
                                        GetInstalledCertificateStatusExtensions.TryParse,
                                        out GetInstalledCertificateStatus Status,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CertificateHashDataChain    [mandatory]

                if (!JSON.ParseMandatoryJSON("certificateHashDataChain",
                                             "certificate hash data chain",
                                             CertificateHashData.TryParse,
                                             out IEnumerable<CertificateHashData> CertificateHashDataChain,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (CertificateHashDataChain is null)
                    return false;

                #endregion

                #region StatusInfo                  [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_0.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                  [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetInstalledCertificateIdsResponse = new GetInstalledCertificateIdsResponse(Request,
                                                                                            Status,
                                                                                            CertificateHashDataChain,
                                                                                            StatusInfo,
                                                                                            CustomData);

                if (CustomGetInstalledCertificateIdsResponseParser is not null)
                    GetInstalledCertificateIdsResponse = CustomGetInstalledCertificateIdsResponseParser(JSON,
                                                                                                        GetInstalledCertificateIdsResponse);

                return true;

            }
            catch (Exception e)
            {
                GetInstalledCertificateIdsResponse  = null;
                ErrorResponse                       = "The given JSON representation of a get installed certificate ids response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetInstalledCertificateIdsResponseSerializer = null, CustomCertificateHashDataSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetInstalledCertificateIdsResponseSerializer">A delegate to serialize custom get installed certificate ids responses.</param>
        /// <param name="CustomCertificateHashDataSerializer">A delegate to serialize custom certificate hash data.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetInstalledCertificateIdsResponse>?  CustomGetInstalledCertificateIdsResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CertificateHashData>?                 CustomCertificateHashDataSerializer                  = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                          CustomStatusInfoSerializer                           = null,
                              CustomJObjectSerializerDelegate<CustomData>?                          CustomCustomDataSerializer                           = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",                    Status.    AsText()),
                                 new JProperty("certificateHashDataChain",  new JArray(CertificateHashDataChain.Select(certificateHashData => certificateHashData.ToJSON(CustomCertificateHashDataSerializer)))),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",                StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                              CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetInstalledCertificateIdsResponseSerializer is not null
                       ? CustomGetInstalledCertificateIdsResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get installed certificate ids request failed.
        /// </summary>
        /// <param name="Request">The get installed certificate ids request leading to this response.</param>
        public static GetInstalledCertificateIdsResponse Failed(CS.GetInstalledCertificateIdsRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (GetInstalledCertificateIdsResponse1, GetInstalledCertificateIdsResponse2)

        /// <summary>
        /// Compares two get installed certificate ids responses for equality.
        /// </summary>
        /// <param name="GetInstalledCertificateIdsResponse1">A get installed certificate ids response.</param>
        /// <param name="GetInstalledCertificateIdsResponse2">Another get installed certificate ids response.</param>
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
        /// Compares two get installed certificate ids responses for inequality.
        /// </summary>
        /// <param name="GetInstalledCertificateIdsResponse1">A get installed certificate ids response.</param>
        /// <param name="GetInstalledCertificateIdsResponse2">Another get installed certificate ids response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetInstalledCertificateIdsResponse? GetInstalledCertificateIdsResponse1,
                                           GetInstalledCertificateIdsResponse? GetInstalledCertificateIdsResponse2)

            => !(GetInstalledCertificateIdsResponse1 == GetInstalledCertificateIdsResponse2);

        #endregion

        #endregion

        #region IEquatable<GetInstalledCertificateIdsResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get installed certificate ids responses for equality.
        /// </summary>
        /// <param name="Object">A get installed certificate ids response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetInstalledCertificateIdsResponse getInstalledCertificateIdsResponse &&
                   Equals(getInstalledCertificateIdsResponse);

        #endregion

        #region Equals(GetInstalledCertificateIdsResponse)

        /// <summary>
        /// Compares two get installed certificate ids responses for equality.
        /// </summary>
        /// <param name="GetInstalledCertificateIdsResponse">A get installed certificate ids response to compare with.</param>
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

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Status.                  GetHashCode()       * 7 ^
                       CertificateHashDataChain.GetHashCode()       * 5 ^
                      (StatusInfo?.             GetHashCode() ?? 0) * 3 ^

                       base.                    GetHashCode();

            }
        }

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
