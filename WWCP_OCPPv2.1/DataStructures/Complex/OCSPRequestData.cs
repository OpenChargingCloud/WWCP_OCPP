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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Information to verify the electric vehicle/user contract certificate via OCSP.
    /// </summary>
    public class OCSPRequestData : ACustomData,
                                   ICBORSerializable<OCSPRequestData>,
                                   IEquatable<OCSPRequestData>
    {

        #region Properties

        /// <summary>
        /// The used hash algorithm.
        /// </summary>
        [Mandatory]
        public HashAlgorithm  HashAlgorithm     { get; }

        /// <summary>
        /// The hash of the issuer's distinguished name (DN), that must be calculated over
        /// the DER encoding of the issuer's name field in the certificate being checked.
        /// [max 128]
        /// </summary>
        [Mandatory]
        public String         IssuerNameHash    { get; }

        /// <summary>
        /// The hash of the DER encoded public key: the value (excluding tag and length) of the subject public key field in the issuer's certificate.
        /// [max 128]
        /// </summary>
        [Mandatory]
        public String         IssuerKeyHash     { get; }

        /// <summary>
        /// The string representation of the hexadecimal value of the serial number without the '0x'-prefix and without leading zeroes.
        /// [max 40]
        /// </summary>
        [Mandatory]
        public String         SerialNumber      { get; }

        /// <summary>
        /// The case-insensitive responder URL.
        /// [max 2000]
        /// </summary>
        [Mandatory]
        public URL            ResponderURL      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new OCSP request data.
        /// </summary>
        /// <param name="HashAlgorithm">The used hash algorithm.</param>
        /// <param name="IssuerNameHash">The hash of the issuer's distinguished name (DN), that must be calculated over the DER encoding of the issuer's name field in the certificate being checked.</param>
        /// <param name="IssuerKeyHash">The hash of the DER encoded public key: the value (excluding tag and length) of the subject public key field in the issuer's certificate.</param>
        /// <param name="SerialNumber">The string representation of the hexadecimal value of the serial number without the '0x'-prefix and without leading zeroes.</param>
        /// <param name="ResponderURL">The case-insensitive responder URL.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public OCSPRequestData(HashAlgorithm  HashAlgorithm,
                               String         IssuerNameHash,
                               String         IssuerKeyHash,
                               String         SerialNumber,
                               URL            ResponderURL,
                               CustomData?    CustomData   = null)

            : base(CustomData)

        {

            this.HashAlgorithm   = HashAlgorithm;
            this.IssuerNameHash  = IssuerNameHash.Trim();
            this.IssuerKeyHash   = IssuerKeyHash. Trim();
            this.SerialNumber    = SerialNumber.  Trim();
            this.ResponderURL    = ResponderURL;

            if (this.IssuerNameHash.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(IssuerNameHash),  "The given issuer name hash must not be null or empty!");

            if (this.IssuerKeyHash.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(IssuerKeyHash),   "The given issuer key hash must not be null or empty!");

            if (this.SerialNumber.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(SerialNumber),    "The given serial number must not be null or empty!");

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Information about a certificate for an OCSP check.",
        //     "javaType": "OCSPRequestData",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "hashAlgorithm": {
        //             "$ref": "#/definitions/HashAlgorithmEnumType"
        //         },
        //         "issuerNameHash": {
        //             "description": "The hash of the issuer\u2019s distinguished name (DN), that must be calculated over the DER
        //                             encoding of the issuer\u2019s name field in the certificate being checked.",
        //             "type": "string",
        //             "maxLength": 128
        //         },
        //         "issuerKeyHash": {
        //             "description": "The hash of the DER encoded public key:
        //                             the value (excluding tag and length) of the subject
        //                             public key field in the issuer\u2019s certificate.",
        //             "type": "string",
        //             "maxLength": 128
        //         },
        //         "serialNumber": {
        //             "description": "The string representation of the hexadecimal value of the serial number without the
        //                             prefix \"0x\" and without leading zeroes.",
        //             "type": "string",
        //             "maxLength": 40
        //         },
        //         "responderURL": {
        //             "description": "This contains the responder URL (Case insensitive).",
        //             "type": "string",
        //             "maxLength": 2000
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "hashAlgorithm",
        //         "issuerNameHash",
        //         "issuerKeyHash",
        //         "serialNumber",
        //         "responderURL"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomOCSPRequestDataParser = null)

        /// <summary>
        /// Parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomOCSPRequestDataParser">A delegate to parse custom OCSP request data.</param>
        public static OCSPRequestData Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<OCSPRequestData>?  CustomOCSPRequestDataParser   = null)
        {

            if (TryParse(JSON,
                         out var ocspRequestData,
                         out var errorResponse,
                         CustomOCSPRequestDataParser))
            {
                return ocspRequestData;
            }

            throw new ArgumentException("The given JSON representation of OCSP request data is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(OCSPRequestDataJSON, out OCSPRequestData, out ErrorResponse, CustomOCSPRequestDataParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of OCSP request data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="OCSPRequestData">The parsed OCSP request data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out OCSPRequestData?  OCSPRequestData,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(JSON,
                        out OCSPRequestData,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of OCSP request data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="OCSPRequestData">The parsed OCSP request data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomOCSPRequestDataParser">A delegate to parse custom OCSP request data.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out OCSPRequestData?      OCSPRequestData,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       CustomJObjectParserDelegate<OCSPRequestData>?  CustomOCSPRequestDataParser)
        {

            try
            {

                OCSPRequestData = default;

                #region HashAlgorithm     [mandatory]

                if (!JSON.ParseMandatory("hashAlgorithm",
                                         "hash algorithm",
                                         OCPPv2_1.HashAlgorithm.TryParse,
                                         out HashAlgorithm HashAlgorithm,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IssuerNameHash    [mandatory]

                if (!JSON.ParseMandatoryText("issuerNameHash",
                                             "issuer name hash",
                                             out String? IssuerNameHash,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IssuerKeyHash     [mandatory]

                if (!JSON.ParseMandatoryText("issuerKeyHash",
                                             "issuer key hash",
                                             out String? IssuerKeyHash,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SerialNumber      [mandatory]

                if (!JSON.ParseMandatoryText("serialNumber",
                                             "certificate serial number",
                                             out String? SerialNumber,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ResponderURL      [mandatory]

                if (!JSON.ParseMandatory("responderURL",
                                         "responder URL",
                                         URL.TryParse,
                                         out URL ResponderURL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData        [optional]

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


                OCSPRequestData = new OCSPRequestData(
                                      HashAlgorithm,
                                      IssuerNameHash.Trim(),
                                      IssuerKeyHash. Trim(),
                                      SerialNumber.  Trim(),
                                      ResponderURL,
                                      CustomData
                                  );

                if (CustomOCSPRequestDataParser is not null)
                    OCSPRequestData = CustomOCSPRequestDataParser(JSON,
                                                                  OCSPRequestData);

                return true;

            }
            catch (Exception e)
            {
                OCSPRequestData  = null;
                ErrorResponse    = "The given JSON representation of OCSP request data is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomOCSPRequestDataSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomOCSPRequestDataSerializer">A delegate to serialize custom OCSP request data.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<OCSPRequestData>?  CustomOCSPRequestDataSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?       CustomCustomDataSerializer        = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("hashAlgorithm",    HashAlgorithm.ToString()),
                                 new JProperty("issuerNameHash",   IssuerNameHash),
                                 new JProperty("issuerKeyHash",    IssuerKeyHash),
                                 new JProperty("serialNumber",     SerialNumber),
                                 new JProperty("responderURL",     ResponderURL. ToString()),

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.   ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomOCSPRequestDataSerializer is not null
                       ? CustomOCSPRequestDataSerializer(this, json)
                       : json;

        }

        #endregion


        #region (static) TryParseCBOR(CBOR, out OCSPRequestData, out ErrorResponse, CustomOCSPRequestDataParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of an OCSP request data.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="OCSPRequestData">The OCSP request data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out OCSPRequestData?  OCSPRequestData,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out OCSPRequestData,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of an OCSP request data.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="OCSPRequestData">The OCSP request data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomOCSPRequestDataParser">An optional delegate to read custom OCSP request data.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out OCSPRequestData?           OCSPRequestData,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<OCSPRequestData>?  CustomOCSPRequestDataParser)
        {

            try
            {

                OCSPRequestData = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of an OCSP request data is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("hashAlgorithm",
                                             "hash algorithm",
                                             out var HashAlgorithmText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!OCPPv2_1.HashAlgorithm.TryParse(HashAlgorithmText, out var HashAlgorithm))
                {
                    ErrorResponse = $"Invalid hash algorithm '{HashAlgorithmText}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("issuerNameHash",
                                             "issuer name hash",
                                             out var IssuerNameHash,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryText("issuerKeyHash",
                                             "issuer key hash",
                                             out var IssuerKeyHash,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryText("serialNumber",
                                             "serial number",
                                             out var SerialNumber,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryText("responderURL",
                                             "responder URL",
                                             out var ResponderURLText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!URL.TryParse(ResponderURLText, out var ResponderURL))
                {
                    ErrorResponse = $"Invalid responder URL '{ResponderURLText}'!";
                    return false;
                }

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                OCSPRequestData = new OCSPRequestData(
                                      HashAlgorithm,
                                      IssuerNameHash,
                                      IssuerKeyHash,
                                      SerialNumber,
                                      ResponderURL,
                                      CustomData
                                  );

                if (CustomOCSPRequestDataParser is not null)
                    OCSPRequestData = CustomOCSPRequestDataParser(CBOR,
                                                 OCSPRequestData);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                OCSPRequestData  = default;
                ErrorResponse  = "The given CBOR representation of an OCSP request data is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<OCSPRequestData>.TryParse(CBOR, out OCSPRequestData, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of an OCSP request data - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<OCSPRequestData>.TryParse(CBORValue                         CBOR,
                                                                 out OCSPRequestData                  Value,
                                                                 [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomOCSPRequestDataSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this OCSP request data: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomOCSPRequestDataSerializer">A delegate to serialize custom OCSP request data.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<OCSPRequestData>? CustomOCSPRequestDataSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("hashAlgorithm",                          CBORValue.FromText(HashAlgorithm.ToString())),
                           ("issuerNameHash",                         CBORValue.FromText(IssuerNameHash)),
                           ("issuerKeyHash",                          CBORValue.FromText(IssuerKeyHash)),
                           ("serialNumber",                           CBORValue.FromText(SerialNumber)),
                           ("responderURL",                           CBORValue.FromText(ResponderURL.ToString())),
                           ("customData",              CustomData?.ToCBOR())
                       );

            return CustomOCSPRequestDataSerializer is not null
                       ? CustomOCSPRequestDataSerializer(this, cbor)
                       : cbor;

        }

        #endregion

        #region Operator overloading

        #region Operator == (OCSPRequestData1, OCSPRequestData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCSPRequestData1">OCSP request data.</param>
        /// <param name="OCSPRequestData2">Other OCSP request data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (OCSPRequestData? OCSPRequestData1,
                                           OCSPRequestData? OCSPRequestData2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(OCSPRequestData1, OCSPRequestData2))
                return true;

            // If one is null, but not both, return false.
            if (OCSPRequestData1 is null || OCSPRequestData2 is null)
                return false;

            return OCSPRequestData1.Equals(OCSPRequestData2);

        }

        #endregion

        #region Operator != (OCSPRequestData1, OCSPRequestData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCSPRequestData1">OCSP request data.</param>
        /// <param name="OCSPRequestData2">Other OCSP request data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (OCSPRequestData? OCSPRequestData1,
                                           OCSPRequestData? OCSPRequestData2)

            => !(OCSPRequestData1 == OCSPRequestData2);

        #endregion

        #endregion

        #region IEquatable<OCSPRequestData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two OCSP request data for equality.
        /// </summary>
        /// <param name="Object">OCSP request data to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is OCSPRequestData ocspRequestData &&
                   Equals(ocspRequestData);

        #endregion

        #region Equals(OCSPRequestData)

        /// <summary>
        /// Compares two OCSP request data for equality.
        /// </summary>
        /// <param name="OCSPRequestData">OCSP request data to compare with.</param>
        public Boolean Equals(OCSPRequestData? OCSPRequestData)

            => OCSPRequestData is not null &&

               HashAlgorithm. Equals(OCSPRequestData.HashAlgorithm)  &&
               IssuerNameHash.Equals(OCSPRequestData.IssuerNameHash) &&
               IssuerKeyHash. Equals(OCSPRequestData.IssuerKeyHash)  &&
               SerialNumber.  Equals(OCSPRequestData.SerialNumber)   &&
               ResponderURL.  Equals(OCSPRequestData.ResponderURL)   &&

               base.          Equals(OCSPRequestData);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return HashAlgorithm. GetHashCode() * 13 ^
                       IssuerNameHash.GetHashCode() * 11 ^
                       IssuerKeyHash. GetHashCode() *  7 ^
                       SerialNumber.  GetHashCode() *  5 ^
                       ResponderURL.  GetHashCode() *  3 ^

                       base.          GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(SerialNumber, " (", HashAlgorithm.ToString(), ")");

        #endregion

    }

}
