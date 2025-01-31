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
    public class CertificateHashData : ACustomData,
                                       IEquatable<CertificateHashData>
    {

        #region Properties

        /// <summary>
        /// The cryptographic hashing algorithm.
        /// </summary>
        [Mandatory]
        public HashAlgorithm  HashAlgorithm          { get; }

        /// <summary>
        /// The hashed value of the the issuers name [max 128]
        /// </summary>
        [Mandatory]
        public String         IssuerNameHash         { get; }

        /// <summary>
        /// The hashed value of the issuers public key [max 128]
        /// </summary>
        [Mandatory]
        public String         IssuerPublicKeyHash    { get; }

        /// <summary>
        /// The serial number of the certificate [max 40].
        /// </summary>
        [Mandatory]
        public String         SerialNumber           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new certificate hash data.
        /// </summary>
        /// <param name="HashAlgorithm">The cryptographic hashing algorithm.</param>
        /// <param name="IssuerNameHash">The hashed value of the the issuers name [max 128]</param>
        /// <param name="IssuerPublicKeyHash">The hashed value of the issuers public key [max 128]</param>
        /// <param name="SerialNumber">The serial number of the certificate [max 40].</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        public CertificateHashData(HashAlgorithm  HashAlgorithm,
                                   String         IssuerNameHash,
                                   String         IssuerPublicKeyHash,
                                   String         SerialNumber,
                                   CustomData?    CustomData   = null)

            : base(CustomData)

        {

            this.HashAlgorithm        = HashAlgorithm;
            this.IssuerNameHash       = IssuerNameHash;
            this.IssuerPublicKeyHash  = IssuerPublicKeyHash;
            this.SerialNumber         = SerialNumber;

            unchecked
            {

                hashCode = this.HashAlgorithm.      GetHashCode() * 11 ^
                           this.IssuerNameHash.     GetHashCode() *  7 ^
                           this.IssuerPublicKeyHash.GetHashCode() *  5 ^
                           this.SerialNumber.       GetHashCode() *  3 ^
                           base.                    GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // "CertificateHashDataType": {
        //   "javaType": "CertificateHashData",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "hashAlgorithm": {
        //       "$ref": "#/definitions/HashAlgorithmEnumType"
        //     },
        //     "issuerNameHash": {
        //       "description": "Hashed value of the Issuer DN (Distinguished Name).",
        //       "type": "string",
        //       "maxLength": 128
        //     },
        //     "issuerKeyHash": {
        //       "description": "Hashed value of the issuers public key",
        //       "type": "string",
        //       "maxLength": 128
        //     },
        //     "serialNumber": {
        //       "description": "The serial number of the certificate.",
        //       "type": "string",
        //       "maxLength": 40
        //     }
        //   },
        //   "required": [
        //     "hashAlgorithm",
        //     "issuerNameHash",
        //     "issuerKeyHash",
        //     "serialNumber"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomCertificateHashDataParser = null)

        /// <summary>
        /// Parse the given JSON representation of certificate hash data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomCertificateHashDataParser">A delegate to parse custom certificate hash datas.</param>
        public static CertificateHashData Parse(JObject                                            JSON,
                                                CustomJObjectParserDelegate<CertificateHashData>?  CustomCertificateHashDataParser   = null)
        {

            if (TryParse(JSON,
                         out var certificateHashData,
                         out var errorResponse,
                         CustomCertificateHashDataParser) &&
                certificateHashData is not null)
            {
                return certificateHashData;
            }

            throw new ArgumentException("The given JSON representation of certificate hash data is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out CertificateHashData, out ErrorResponse, CustomCertificateHashDataParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of certificate hash data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CertificateHashData">The parsed certificate hash data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out CertificateHashData?  CertificateHashData,
                                       [NotNullWhen(false)] out String?               ErrorResponse)

            => TryParse(JSON,
                        out CertificateHashData,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of certificate hash data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CertificateHashData">The parsed certificate hash data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCertificateHashDataParser">A delegate to parse custom CertificateHashDatas.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       [NotNullWhen(true)]  out CertificateHashData?      CertificateHashData,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       CustomJObjectParserDelegate<CertificateHashData>?  CustomCertificateHashDataParser)
        {

            try
            {

                CertificateHashData = null;

                #region HashAlgorithm          [mandatory]

                if (!JSON.ParseMandatory("hashAlgorithm",
                                         "hash algorithm",
                                         OCPPv2_1.HashAlgorithm.TryParse,
                                         out HashAlgorithm HashAlgorithm,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IssuerNameHash         [mandatory]

                if (!JSON.ParseMandatoryText("issuerNameHash",
                                             "issuer name hash",
                                             out String? IssuerNameHash,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IssuerPublicKeyHash    [mandatory]

                if (!JSON.ParseMandatoryText("issuerKeyHash",
                                             "issuer public key hash",
                                             out String? IssuerPublicKeyHash,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SerialNumber           [mandatory]

                if (!JSON.ParseMandatoryText("serialNumber",
                                             "serial number",
                                             out String? SerialNumber,
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


                CertificateHashData = new CertificateHashData(
                                          HashAlgorithm,
                                          IssuerNameHash,
                                          IssuerPublicKeyHash,
                                          SerialNumber,
                                          CustomData
                                      );

                if (CustomCertificateHashDataParser is not null)
                    CertificateHashData = CustomCertificateHashDataParser(JSON,
                                                                          CertificateHashData);

                return true;

            }
            catch (Exception e)
            {
                CertificateHashData  = default;
                ErrorResponse        = "The given JSON representation of certificate hash data is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCertificateHashDataSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCertificateHashDataSerializer">A delegate to serialize custom certificate hash datas.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CertificateHashData>?  CustomCertificateHashDataSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("hashAlgorithm",    HashAlgorithm.ToString()),
                                 new JProperty("issuerNameHash",   IssuerNameHash),
                                 new JProperty("issuerKeyHash",    IssuerPublicKeyHash),
                                 new JProperty("serialNumber",     SerialNumber),

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.   ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomCertificateHashDataSerializer is not null
                       ? CustomCertificateHashDataSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CertificateHashData1, CertificateHashData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateHashData1">Certificate hash data.</param>
        /// <param name="CertificateHashData2">Other certificate hash data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CertificateHashData? CertificateHashData1,
                                           CertificateHashData? CertificateHashData2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CertificateHashData1, CertificateHashData2))
                return true;

            // If one is null, but not both, return false.
            if (CertificateHashData1 is null || CertificateHashData2 is null)
                return false;

            return CertificateHashData1.Equals(CertificateHashData2);

        }

        #endregion

        #region Operator != (CertificateHashData1, CertificateHashData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateHashData1">Certificate hash data.</param>
        /// <param name="CertificateHashData2">Other certificate hash data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CertificateHashData? CertificateHashData1,
                                           CertificateHashData? CertificateHashData2)

            => !(CertificateHashData1 == CertificateHashData2);

        #endregion

        #endregion

        #region IEquatable<CertificateHashData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two certificate hash datas for equality.
        /// </summary>
        /// <param name="Object">Certificate hash data to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CertificateHashData certificateHashData &&
                   Equals(certificateHashData);

        #endregion

        #region Equals(CertificateHashData)

        /// <summary>
        /// Compares two certificate hash datas for equality.
        /// </summary>
        /// <param name="CertificateHashData">Certificate hash data to compare with.</param>
        public Boolean Equals(CertificateHashData? CertificateHashData)

            => CertificateHashData is not null &&

               HashAlgorithm.      Equals(CertificateHashData.HashAlgorithm)       &&
               IssuerNameHash.     Equals(CertificateHashData.IssuerNameHash)      &&
               IssuerPublicKeyHash.Equals(CertificateHashData.IssuerPublicKeyHash) &&
               SerialNumber.       Equals(CertificateHashData.SerialNumber)        &&

               base.               Equals(CertificateHashData);

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

            => $"{HashAlgorithm}, {SerialNumber}";

        #endregion

    }

}
