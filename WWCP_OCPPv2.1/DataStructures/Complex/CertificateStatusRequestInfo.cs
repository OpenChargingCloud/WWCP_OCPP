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
    /// CertificateStatusRequestInfo.
    /// </summary>
    public class CertificateStatusRequestInfo : ACustomData,
                                                IEquatable<CertificateStatusRequestInfo>
    {

        #region Properties

        /// <summary>
        /// The CertificateStatusRequestInfo to be verified.
        /// </summary>
        [Mandatory]
        public CertificateHashData      CertificateHashData    { get; }

        /// <summary>
        /// The source of the certificate status.
        /// </summary>
        [Mandatory]
        public CertificateStatusSource  Source                 { get; }

        /// <summary>
        /// The enumeration of URLs to check the certificate status.
        /// </summary>
        [Mandatory]
        public IEnumerable<URL>         URLs                   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new CertificateStatusRequestInfo.
        /// </summary>
        /// <param name="CertificateHashData">The CertificateStatusRequestInfo to be verified.</param>
        /// <param name="Source">A source of the certificate status.</param>
        /// <param name="URLs">An enumeration of URLs to check the certificate status.</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        public CertificateStatusRequestInfo(CertificateHashData      CertificateHashData,
                                            CertificateStatusSource  Source,
                                            IEnumerable<URL>         URLs,
                                            CustomData?              CustomData   = null)

            : base(CustomData)

        {

            this.CertificateHashData  = CertificateHashData;
            this.Source               = Source;
            this.URLs                 = URLs.Distinct();

            unchecked
            {

                hashCode = this.CertificateHashData.GetHashCode() *  7 ^
                           this.Source.             GetHashCode() *  5 ^
                           this.URLs.               GetHashCode() *  3 ^
                           base.                    GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "CertificateStatusRequestInfo",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "hashAlgorithm": {
        //             "$ref": "#/definitions/HashAlgorithmEnumType"
        //         },
        //         "issuerNameHash": {
        //             "description": "The hash of the issuer\u2019s distinguished
        //                             name (DN), that must be calculated over the DER
        //                             encoding of the issuer\u2019s name field in the certificate being checked.",
        //             "type": "string",
        //             "maxLength": 128
        //         },
        //         "issuerKeyHash": {
        //             "description": "The hash of the DER encoded public key:
        //                             the value (excluding tag and length) of the subject public key field in the issuer\u2019s certificate.",
        //             "type": "string",
        //             "maxLength": 128
        //         },
        //         "serialNumber": {
        //             "description": "The string representation of the hexadecimal value of the serial number without the prefix \"0x\" and without leading zeroes.",
        //             "type": "string",
        //             "maxLength": 40
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "hashAlgorithm",
        //         "issuerNameHash",
        //         "issuerKeyHash",
        //         "serialNumber"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomCertificateStatusRequestInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of CertificateStatusRequestInfo.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomCertificateStatusRequestInfoParser">A delegate to parse custom CertificateStatusRequestInfos.</param>
        public static CertificateStatusRequestInfo Parse(JObject                                                     JSON,
                                                         CustomJObjectParserDelegate<CertificateStatusRequestInfo>?  CustomCertificateStatusRequestInfoParser   = null)
        {

            if (TryParse(JSON,
                         out var certificateStatusRequestInfo,
                         out var errorResponse,
                         CustomCertificateStatusRequestInfoParser))
            {
                return certificateStatusRequestInfo;
            }

            throw new ArgumentException("The given JSON representation of CertificateStatusRequestInfo is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out CertificateStatusRequestInfo, out ErrorResponse, CustomCertificateStatusRequestInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of CertificateStatusRequestInfo.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CertificateStatusRequestInfo">The parsed CertificateStatusRequestInfo.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       [NotNullWhen(true)]  out CertificateStatusRequestInfo?  CertificateStatusRequestInfo,
                                       [NotNullWhen(false)] out String?                        ErrorResponse)

            => TryParse(JSON,
                        out CertificateStatusRequestInfo,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of CertificateStatusRequestInfo.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CertificateStatusRequestInfo">The parsed CertificateStatusRequestInfo.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCertificateStatusRequestInfoParser">A delegate to parse custom CertificateStatusRequestInfos.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       [NotNullWhen(true)]  out CertificateStatusRequestInfo?      CertificateStatusRequestInfo,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       CustomJObjectParserDelegate<CertificateStatusRequestInfo>?  CustomCertificateStatusRequestInfoParser)
        {

            try
            {

                CertificateStatusRequestInfo = null;

                #region CertificateHashData    [mandatory]

                if (!JSON.ParseMandatoryJSON("certificateStatusRequestInfo",
                                             "certificate hash data",
                                             OCPPv2_1.CertificateHashData.TryParse,
                                             out CertificateHashData? CertificateHashData,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Source                 [mandatory]

                if (!JSON.ParseMandatory("source",
                                         "certificate status source",
                                         CertificateStatusSource.TryParse,
                                         out CertificateStatusSource Source,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region URLs                   [mandatory]

                if (!JSON.ParseMandatoryHashSet("serialNumber",
                                                "serial number",
                                                URL.TryParse,
                                                out HashSet<URL> URLs,
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


                CertificateStatusRequestInfo = new CertificateStatusRequestInfo(
                                                   CertificateHashData,
                                                   Source,
                                                   URLs,
                                                   CustomData
                                               );

                if (CustomCertificateStatusRequestInfoParser is not null)
                    CertificateStatusRequestInfo = CustomCertificateStatusRequestInfoParser(JSON,
                                                                                            CertificateStatusRequestInfo);

                return true;

            }
            catch (Exception e)
            {
                CertificateStatusRequestInfo  = default;
                ErrorResponse                 = "The given JSON representation of CertificateStatusRequestInfo is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCertificateStatusRequestInfoSerializer = null, CustomCustomDataSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCertificateStatusRequestInfoSerializer">A delegate to serialize custom CertificateStatusRequestInfos.</param>
        /// <param name="CustomCertificateHashDataSerializer">A delegate to serialize custom CertificateHashDatas.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CertificateStatusRequestInfo>?  CustomCertificateStatusRequestInfoSerializer   = null,
                              CustomJObjectSerializerDelegate<CertificateHashData>?           CustomCertificateHashDataSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("certificateHashData",   CertificateHashData.ToJSON(CustomCertificateHashDataSerializer,
                                                                                                   CustomCustomDataSerializer)),

                                 new JProperty("source",                Source.             ToString()),

                                 new JProperty("urls",                  new JArray(URLs.Select(url => url.ToString()))),

                           CustomData is not null
                               ? new JProperty("customData",            CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomCertificateStatusRequestInfoSerializer is not null
                       ? CustomCertificateStatusRequestInfoSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CertificateStatusRequestInfo1, CertificateStatusRequestInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateStatusRequestInfo1">Certificate hash data.</param>
        /// <param name="CertificateStatusRequestInfo2">Other CertificateStatusRequestInfo.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CertificateStatusRequestInfo? CertificateStatusRequestInfo1,
                                           CertificateStatusRequestInfo? CertificateStatusRequestInfo2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CertificateStatusRequestInfo1, CertificateStatusRequestInfo2))
                return true;

            // If one is null, but not both, return false.
            if (CertificateStatusRequestInfo1 is null || CertificateStatusRequestInfo2 is null)
                return false;

            return CertificateStatusRequestInfo1.Equals(CertificateStatusRequestInfo2);

        }

        #endregion

        #region Operator != (CertificateStatusRequestInfo1, CertificateStatusRequestInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateStatusRequestInfo1">Certificate hash data.</param>
        /// <param name="CertificateStatusRequestInfo2">Other CertificateStatusRequestInfo.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CertificateStatusRequestInfo? CertificateStatusRequestInfo1,
                                           CertificateStatusRequestInfo? CertificateStatusRequestInfo2)

            => !(CertificateStatusRequestInfo1 == CertificateStatusRequestInfo2);

        #endregion

        #endregion

        #region IEquatable<CertificateStatusRequestInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two CertificateStatusRequestInfos for equality.
        /// </summary>
        /// <param name="Object">Certificate hash data to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CertificateStatusRequestInfo certificateStatusRequestInfo &&
                   Equals(certificateStatusRequestInfo);

        #endregion

        #region Equals(CertificateStatusRequestInfo)

        /// <summary>
        /// Compares two CertificateStatusRequestInfos for equality.
        /// </summary>
        /// <param name="CertificateStatusRequestInfo">Certificate hash data to compare with.</param>
        public Boolean Equals(CertificateStatusRequestInfo? CertificateStatusRequestInfo)

            => CertificateStatusRequestInfo is not null &&

               CertificateHashData.Equals(CertificateStatusRequestInfo.CertificateHashData) &&
               Source.             Equals(CertificateStatusRequestInfo.Source)              &&

               URLs.ToHashSet().SetEquals(CertificateStatusRequestInfo.URLs.ToHashSet()) &&

               base.               Equals(CertificateStatusRequestInfo);

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

            => $"Serial '{CertificateHashData.SerialNumber}' via '{Source}' from: {URLs.AggregateWith(", ")}";

        #endregion

    }

}
