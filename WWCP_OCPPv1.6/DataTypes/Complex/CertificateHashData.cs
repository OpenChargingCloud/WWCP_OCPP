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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Certificate hash data.
    /// </summary>
    public class CertificateHashData : IEquatable<CertificateHashData>
    {

        #region Properties

        /// <summary>
        /// The cryptographic hashing algorithm.
        /// </summary>
        public HashAlgorithms  HashAlgorithm          { get; }

        /// <summary>
        /// The hashed value of the IssuerName [max 128]
        /// </summary>
        public String          IssuerNameHash         { get; }

        /// <summary>
        /// The hashed value of the issuers public key [max 128]
        /// </summary>
        public String          IssuerPublicKeyHash    { get; }

        /// <summary>
        /// The serial number of the certificate [max 40].
        /// </summary>
        public String          SerialNumber           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new certificate hash data.
        /// </summary>
        /// <param name="HashAlgorithm">The cryptographic hashing algorithm.</param>
        /// <param name="IssuerNameHash">The hashed value of the IssuerName [max 128]</param>
        /// <param name="IssuerPublicKeyHash">The hashed value of the issuers public key [max 128]</param>
        /// <param name="SerialNumber">The serial number of the certificate [max 40].</param>
        public CertificateHashData(HashAlgorithms  HashAlgorithm,
                                   String          IssuerNameHash,
                                   String          IssuerPublicKeyHash,
                                   String          SerialNumber)
        {

            this.HashAlgorithm        = HashAlgorithm;
            this.IssuerNameHash       = IssuerNameHash;
            this.IssuerPublicKeyHash  = IssuerPublicKeyHash;
            this.SerialNumber         = SerialNumber;

        }

        #endregion


        #region Documentation

        // ???

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
                         CustomCertificateHashDataParser))
            {
                return certificateHashData!;
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
        public static Boolean TryParse(JObject                   JSON,
                                       out CertificateHashData?  CertificateHashData,
                                       out String?               ErrorResponse)

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
                                       out CertificateHashData?                           CertificateHashData,
                                       out String?                                        ErrorResponse,
                                       CustomJObjectParserDelegate<CertificateHashData>?  CustomCertificateHashDataParser)
        {

            try
            {

                CertificateHashData = null;

                #region HashAlgorithm          [mandatory]

                if (!JSON.MapMandatory("hashAlgorithm",
                                       "hash algorithm",
                                       HashAlgorithmsExtentions.Parse,
                                       out HashAlgorithms HashAlgorithm,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IssuerNameHash         [mandatory]

                if (!JSON.ParseMandatoryText("issuerNameHash",
                                             "issuer name hash",
                                             out String IssuerNameHash,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IssuerPublicKeyHash    [mandatory]

                if (!JSON.ParseMandatoryText("issuerKeyHash",
                                             "issuer public key hash",
                                             out String IssuerPublicKeyHash,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SerialNumber           [mandatory]

                if (!JSON.ParseMandatoryText("serialNumber",
                                             "serial number",
                                             out String SerialNumber,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                CertificateHashData = new CertificateHashData(HashAlgorithm,
                                                              IssuerNameHash,
                                                              IssuerPublicKeyHash,
                                                              SerialNumber);

                if (CustomCertificateHashDataParser is not null)
                    CertificateHashData = CustomCertificateHashDataParser(JSON,
                                                              CertificateHashData);

                return true;

            }
            catch (Exception e)
            {
                CertificateHashData  = default;
                ErrorResponse  = "The given JSON representation of certificate hash data is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCertificateHashDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCertificateHashDataSerializer">A delegate to serialize custom certificate hash datas.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CertificateHashData>? CustomCertificateHashDataSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("hashAlgorithm",   HashAlgorithm),
                           new JProperty("issuerNameHash",  IssuerNameHash),
                           new JProperty("issuerKeyHash",   IssuerPublicKeyHash),
                           new JProperty("serialNumber",    SerialNumber)
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
        public static Boolean operator == (CertificateHashData CertificateHashData1, CertificateHashData CertificateHashData2)
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
        public static Boolean operator != (CertificateHashData CertificateHashData1, CertificateHashData CertificateHashData2)

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
               SerialNumber.       Equals(CertificateHashData.SerialNumber);

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

                return HashAlgorithm.      GetHashCode() * 7 ^
                       IssuerNameHash.     GetHashCode() * 5 ^
                       IssuerPublicKeyHash.GetHashCode() * 3 ^
                       SerialNumber.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(HashAlgorithm,
                             ", ",
                             SerialNumber);

        #endregion


    }

}
