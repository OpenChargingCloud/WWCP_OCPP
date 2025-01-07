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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Certificate hash data.
    /// </summary>
    public class CertificateHashDataChain : ACustomData,
                                            IEquatable<CertificateHashDataChain>
    {

        #region Properties

        /// <summary>
        /// The cryptographic hashing algorithm.
        /// </summary>
        [Mandatory]
        public CertificateHashData  CertificateHashData    { get; }

        /// <summary>
        /// The hashed value of the the issuers name [max 128]
        /// </summary>
        [Mandatory]
        public GetCertificateIdUse  CertificateType        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new certificate hash data.
        /// </summary>
        /// <param name="CertificateHashData">Certificate hash data.</param>
        /// <param name="CertificateType">The certificate type.</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        public CertificateHashDataChain(CertificateHashData  CertificateHashData,
                                        GetCertificateIdUse  CertificateType,
                                        CustomData?          CustomData   = null)

            : base(CustomData)

        {

            this.CertificateHashData  = CertificateHashData;
            this.CertificateType      = CertificateType;

            unchecked
            {

                hashCode = this.CertificateHashData.GetHashCode() * 5 ^
                           this.CertificateType.    GetHashCode() * 3 ^
                           base.                    GetHashCode();

            }


        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetInstalledCertificateIdsResponse",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
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
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomCertificateHashDataChainParser = null)

        /// <summary>
        /// Parse the given JSON representation of certificate hash data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomCertificateHashDataChainParser">A delegate to parse custom certificate hash datas.</param>
        public static CertificateHashDataChain Parse(JObject                                            JSON,
                                                CustomJObjectParserDelegate<CertificateHashDataChain>?  CustomCertificateHashDataChainParser   = null)
        {

            if (TryParse(JSON,
                         out var certificateHashDataChain,
                         out var errorResponse,
                         CustomCertificateHashDataChainParser) &&
                certificateHashDataChain is not null)
            {
                return certificateHashDataChain;
            }

            throw new ArgumentException("The given JSON representation of certificate hash data is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out CertificateHashDataChain, out ErrorResponse, CustomCertificateHashDataChainParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of certificate hash data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CertificateHashDataChain">The parsed certificate hash data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       [NotNullWhen(true)]  out CertificateHashDataChain?  CertificateHashDataChain,
                                       [NotNullWhen(false)] out String?                    ErrorResponse)

            => TryParse(JSON,
                        out CertificateHashDataChain,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of certificate hash data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CertificateHashDataChain">The parsed certificate hash data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCertificateHashDataChainParser">A delegate to parse custom CertificateHashDataChains.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       [NotNullWhen(true)]  out CertificateHashDataChain?      CertificateHashDataChain,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       CustomJObjectParserDelegate<CertificateHashDataChain>?  CustomCertificateHashDataChainParser)
        {

            try
            {

                CertificateHashDataChain = null;

// {
//   "certificateHashData": {
//     "hashAlgorithm": "SHA256",
//     "issuerKeyHash": "89783f8f6ee47dc5e8f318fd4e8ef8d07fda19f99945e9b4da2884169662be05",
//     "issuerNameHash": "d5780595f5221adba8bbf9ba76ceade403518cc7cb3d642180654d823b8fc8a2",
//     "serialNumber": "6f973cf2da5ba83c"
//   },
//   "certificateType": "CSMSRootCertificate"
// }

                #region CertificateHashData    [mandatory]

                if (!JSON.ParseMandatoryJSON("certificateHashData",
                                             "certificate hash data",
                                             OCPPv2_1. CertificateHashData.TryParse,
                                             out CertificateHashData? CertificateHashData,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CertificateType        [mandatory]

                if (!JSON.ParseMandatory("certificateType",
                                         "certificate type",
                                         GetCertificateIdUse.TryParse,
                                         out GetCertificateIdUse CertificateType,
                                         out ErrorResponse))
                {
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


                CertificateHashDataChain = new CertificateHashDataChain(
                                               CertificateHashData,
                                               CertificateType,
                                               CustomData
                                           );

                if (CustomCertificateHashDataChainParser is not null)
                    CertificateHashDataChain = CustomCertificateHashDataChainParser(JSON,
                                                                                    CertificateHashDataChain);

                return true;

            }
            catch (Exception e)
            {
                CertificateHashDataChain  = default;
                ErrorResponse             = "The given JSON representation of certificate hash data chain is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCertificateHashDataChainSerializer = null, CustomCertificateHashDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCertificateHashDataChainSerializer">A delegate to serialize custom certificate hash data chains.</param>
        /// <param name="CustomCertificateHashDataSerializer">A delegate to serialize custom certificate hash datas.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CertificateHashDataChain>?  CustomCertificateHashDataChainSerializer   = null,
                              CustomJObjectSerializerDelegate<CertificateHashData>?       CustomCertificateHashDataSerializer        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("certificateHashData",   CertificateHashData.ToJSON  (CustomCertificateHashDataSerializer,
                                                                                                     CustomCustomDataSerializer)),

                                 new JProperty("certificateType",       CertificateType.    ToString()),

                           CustomData is not null
                               ? new JProperty("customData",            CustomData.         ToJSON  (CustomCustomDataSerializer))
                               : null

                       );

            return CustomCertificateHashDataChainSerializer is not null
                       ? CustomCertificateHashDataChainSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CertificateHashDataChain1, CertificateHashDataChain2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateHashDataChain1">Certificate hash data.</param>
        /// <param name="CertificateHashDataChain2">Other certificate hash data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CertificateHashDataChain? CertificateHashDataChain1,
                                           CertificateHashDataChain? CertificateHashDataChain2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CertificateHashDataChain1, CertificateHashDataChain2))
                return true;

            // If one is null, but not both, return false.
            if (CertificateHashDataChain1 is null || CertificateHashDataChain2 is null)
                return false;

            return CertificateHashDataChain1.Equals(CertificateHashDataChain2);

        }

        #endregion

        #region Operator != (CertificateHashDataChain1, CertificateHashDataChain2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateHashDataChain1">Certificate hash data.</param>
        /// <param name="CertificateHashDataChain2">Other certificate hash data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CertificateHashDataChain? CertificateHashDataChain1,
                                           CertificateHashDataChain? CertificateHashDataChain2)

            => !(CertificateHashDataChain1 == CertificateHashDataChain2);

        #endregion

        #endregion

        #region IEquatable<CertificateHashDataChain> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two certificate hash datas for equality.
        /// </summary>
        /// <param name="Object">Certificate hash data to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CertificateHashDataChain certificateHashDataChain &&
                   Equals(certificateHashDataChain);

        #endregion

        #region Equals(CertificateHashDataChain)

        /// <summary>
        /// Compares two certificate hash datas for equality.
        /// </summary>
        /// <param name="CertificateHashDataChain">Certificate hash data to compare with.</param>
        public Boolean Equals(CertificateHashDataChain? CertificateHashDataChain)

            => CertificateHashDataChain is not null &&

               CertificateHashData.Equals(CertificateHashDataChain.CertificateHashData) &&
               CertificateType.    Equals(CertificateHashDataChain.CertificateType)     &&
               base.               Equals(CertificateHashDataChain);

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

            => $"{CertificateHashData}, {CertificateType}";

        #endregion

    }

}
