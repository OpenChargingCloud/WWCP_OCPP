/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
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
        public GetInstalledCertificateStatus     Status                 { get; }

        /// <summary>
        /// The optional enumeration of information about available certificates.
        /// </summary>
        public IEnumerable<CertificateHashData>  CertificateHashData    { get; }

        #endregion

        #region Constructor(s)

        #region GetInstalledCertificateIdsResponse(Request, Status, CertificateHashData)

        /// <summary>
        /// Create a new get installed certificate ids response.
        /// </summary>
        /// <param name="Request">The get installed certificate ids request leading to this response.</param>
        /// <param name="Status">An optional enumeration of information about available certificates.</param>
        /// <param name="CertificateHashData">The Charge Point includes the Certificate information for each available certificate.</param>
        public GetInstalledCertificateIdsResponse(CS.GetInstalledCertificateIdsRequest  Request,
                                                  GetInstalledCertificateStatus         Status,
                                                  IEnumerable<CertificateHashData>      CertificateHashData)

            : base(Request,
                   Result.OK())

        {

            this.Status               = Status;
            this.CertificateHashData  = CertificateHashData ?? Array.Empty<CertificateHashData>();

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

            this.Status               = GetInstalledCertificateStatus.Unknown;
            this.CertificateHashData  = Array.Empty<CertificateHashData>();

        }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:GetInstalledCertificateIds.conf",
        //   "definitions": {
        //     "GetInstalledCertificateStatusEnumType": {
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "NotFound"
        //       ]
        //     },
        //     "HashAlgorithmEnumType": {
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "SHA256",
        //         "SHA384",
        //         "SHA512"
        //       ]
        // },
        //     "CertificateHashDataType": {
        //     "javaType": "CertificateHashData",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "hashAlgorithm": {
        //             "$ref": "#/definitions/HashAlgorithmEnumType"
        //         },
        //         "issuerNameHash": {
        //             "type": "string",
        //           "maxLength": 128
        //         },
        //         "issuerKeyHash": {
        //             "type": "string",
        //           "maxLength": 128
        //         },
        //         "serialNumber": {
        //             "type": "string",
        //           "maxLength": 40
        //         }
        //     },
        //       "required": [
        //         "hashAlgorithm",
        //         "issuerNameHash",
        //         "issuerKeyHash",
        //         "serialNumber"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "certificateHashData": {
        //         "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //             "$ref": "#/definitions/CertificateHashDataType"
        //       },
        //       "minItems": 1
        //     },
        //     "status": {
        //         "$ref": "#/definitions/GetInstalledCertificateStatusEnumType"
        //     }
        // },
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

                #region Status                 [mandatory]

                if (JSON.MapMandatory("status",
                                      "status",
                                      GetInstalledCertificateStatusExtensions.Parse,
                                      out GetInstalledCertificateStatus Status,
                                      out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CertificateHashData    [optional]

                if (JSON.ParseOptionalJSON("certificateHashData",
                                           "certificate hash data",
                                           OCPPv1_6.CertificateHashData.TryParse,
                                           out IEnumerable<CertificateHashData> CertificateHashData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetInstalledCertificateIdsResponse = new GetInstalledCertificateIdsResponse(Request,
                                                                                            Status,
                                                                                            CertificateHashData);

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

        #region ToJSON(CustomGetInstalledCertificateIdsResponseSerializer = null, CustomConfigurationKeySerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetInstalledCertificateIdsResponseSerializer">A delegate to serialize custom get installed certificate ids responses.</param>
        /// <param name="CustomCertificateHashDataSerializer">A delegate to serialize custom certificate hash data.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetInstalledCertificateIdsResponse>?  CustomGetInstalledCertificateIdsResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CertificateHashData>?                 CustomCertificateHashDataSerializer                  = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                      CustomSignatureSerializer                            = null,
                              CustomJObjectSerializerDelegate<CustomData>?                          CustomCustomDataSerializer                           = null)
        {

            var json = JSONObject.Create(

                           new JProperty("status",  Status.AsText()),

                           CertificateHashData.Any()
                               ? new JProperty("certificateHashData",  new JArray(CertificateHashData.Select(certificateHashData => certificateHashData.ToJSON(CustomCertificateHashDataSerializer))))
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

               Status.Equals(GetInstalledCertificateIdsResponse.Status) &&

               CertificateHashData.Count().Equals(GetInstalledCertificateIdsResponse.CertificateHashData.Count()) &&
               CertificateHashData.All(certificateHashData => GetInstalledCertificateIdsResponse.CertificateHashData.Contains(certificateHashData));

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

                return Status.             GetHashCode() * 3 ^
                       CertificateHashData.CalcHashCode();

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
