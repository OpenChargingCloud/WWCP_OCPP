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
    /// The CertificateHashData chain.
    /// </summary>
    public class CertificateHashDataChain : ACustomData,
                                            IEquatable<CertificateHashDataChain>
    {

        #region Properties

        /// <summary>
        /// The cryptographic hashing algorithm.
        /// </summary>
        [Mandatory]
        public CertificateHashData               CertificateHashData         { get; }

        /// <summary>
        /// The hashed value of the the issuers name [max 128]
        /// </summary>
        [Mandatory]
        public GetCertificateIdUse               CertificateType             { get; }

        /// <summary>
        /// The optional enumeration of child CertificateHashDataChain.
        /// </summary>
        [Optional]
        public IEnumerable<CertificateHashData>  ChildCertificateHashData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new CertificateHashData chain.
        /// </summary>
        /// <param name="CertificateHashData">Certificate hash data.</param>
        /// <param name="CertificateType">The certificate type.</param>
        /// <param name="ChildCertificateHashData">An optional enumeration of child CertificateHashDataChain.</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        public CertificateHashDataChain(CertificateHashData                CertificateHashData,
                                        GetCertificateIdUse                CertificateType,
                                        IEnumerable<CertificateHashData>?  ChildCertificateHashData   = null,
                                        CustomData?                        CustomData                 = null)

            : base(CustomData)

        {

            this.CertificateHashData       = CertificateHashData;
            this.CertificateType           = CertificateType;
            this.ChildCertificateHashData  = ChildCertificateHashData?.Distinct() ?? [];

            unchecked
            {

                hashCode = this.CertificateHashData.     GetHashCode() * 7 ^
                           this.CertificateType.         GetHashCode() * 5 ^
                           this.ChildCertificateHashData.GetHashCode() * 3 ^
                           base.                         GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "CertificateHashDataChain",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "certificateHashData": {
        //             "$ref": "#/definitions/CertificateHashDataType"
        //         },
        //         "certificateType": {
        //             "$ref": "#/definitions/GetCertificateIdUseEnumType"
        //         },
        //         "childCertificateHashData": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/CertificateHashDataType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 4
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "certificateType",
        //         "certificateHashData"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomCertificateHashDataChainParser = null)

        /// <summary>
        /// Parse the given JSON representation of a CertificateHashDataChain.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomCertificateHashDataChainParser">A delegate to parse custom CertificateHashDataChains.</param>
        public static CertificateHashDataChain Parse(JObject                                                 JSON,
                                                     CustomJObjectParserDelegate<CertificateHashDataChain>?  CustomCertificateHashDataChainParser   = null)
        {

            if (TryParse(JSON,
                         out var certificateHashDataChain,
                         out var errorResponse,
                         CustomCertificateHashDataChainParser))
            {
                return certificateHashDataChain;
            }

            throw new ArgumentException("The given JSON representation of a CertificateHashDataChain is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out CertificateHashDataChain, out ErrorResponse, CustomCertificateHashDataChainParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a CertificateHashDataChain.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CertificateHashDataChain">The parsed CertificateHashDataChain.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       [NotNullWhen(true)]  out CertificateHashDataChain?  CertificateHashDataChain,
                                       [NotNullWhen(false)] out String?                    ErrorResponse)

            => TryParse(JSON,
                        out CertificateHashDataChain,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a CertificateHashDataChain.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CertificateHashDataChain">The parsed CertificateHashDataChain.</param>
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

                #region CertificateHashData         [mandatory]

                if (!JSON.ParseMandatoryJSON("certificateHashData",
                                             "CertificateHashDataChain",
                                             OCPPv2_1. CertificateHashData.TryParse,
                                             out CertificateHashData? CertificateHashData,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CertificateType             [mandatory]

                if (!JSON.ParseMandatory("certificateType",
                                         "certificate type",
                                         GetCertificateIdUse.TryParse,
                                         out GetCertificateIdUse CertificateType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChildCertificateHashData    [optional]

                if (JSON.ParseOptionalHashSet("childCertificateHashData",
                                              "child certificate hash data",
                                              CertificateHashData.TryParse,
                                              out HashSet<CertificateHashData> ChildCertificateHashData,
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


                CertificateHashDataChain = new CertificateHashDataChain(
                                               CertificateHashData,
                                               CertificateType,
                                               ChildCertificateHashData,
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
                ErrorResponse             = "The given JSON representation of a CertificateHashDataChain chain is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCertificateHashDataChainSerializer = null, CustomCertificateHashDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCertificateHashDataChainSerializer">A delegate to serialize custom CertificateHashDataChain chains.</param>
        /// <param name="CustomCertificateHashDataSerializer">A delegate to serialize custom CertificateHashDataChains.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CertificateHashDataChain>?  CustomCertificateHashDataChainSerializer   = null,
                              CustomJObjectSerializerDelegate<CertificateHashData>?       CustomCertificateHashDataSerializer        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("certificateHashData",        CertificateHashData.ToJSON  (CustomCertificateHashDataSerializer,
                                                                                                          CustomCustomDataSerializer)),

                                 new JProperty("certificateType",            CertificateType.    ToString()),

                           ChildCertificateHashData.Any()
                               ? new JProperty("childCertificateHashData",   new JArray(ChildCertificateHashData.Select(childCertificateHashData => childCertificateHashData.ToJSON(CustomCertificateHashDataSerializer,
                                                                                                                                                                                    CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                 CustomData.         ToJSON  (CustomCustomDataSerializer))
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
        /// <param name="CertificateHashDataChain2">Other CertificateHashDataChain.</param>
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
        /// <param name="CertificateHashDataChain2">Other CertificateHashDataChain.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CertificateHashDataChain? CertificateHashDataChain1,
                                           CertificateHashDataChain? CertificateHashDataChain2)

            => !(CertificateHashDataChain1 == CertificateHashDataChain2);

        #endregion

        #endregion

        #region IEquatable<CertificateHashDataChain> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two CertificateHashDataChains for equality.
        /// </summary>
        /// <param name="Object">Certificate hash data to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CertificateHashDataChain certificateHashDataChain &&
                   Equals(certificateHashDataChain);

        #endregion

        #region Equals(CertificateHashDataChain)

        /// <summary>
        /// Compares two CertificateHashDataChains for equality.
        /// </summary>
        /// <param name="CertificateHashDataChain">Certificate hash data to compare with.</param>
        public Boolean Equals(CertificateHashDataChain? CertificateHashDataChain)

            => CertificateHashDataChain is not null &&

               CertificateHashData.                    Equals(CertificateHashDataChain.CertificateHashData)      &&
               CertificateType.                        Equals(CertificateHashDataChain.CertificateType)          &&
               ChildCertificateHashData.ToHashSet().SetEquals(CertificateHashDataChain.ChildCertificateHashData) &&
               base.                                   Equals(CertificateHashDataChain);

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
