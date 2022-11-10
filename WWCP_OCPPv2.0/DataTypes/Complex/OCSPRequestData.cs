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

using System;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;
using System.Security.Policy;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Information to verify the electric vehicle/user contract certificate via OCSP.
    /// </summary>
    public class OCSPRequestData
    {

        #region Properties

        /// <summary>
        /// The used hash algorithm.
        /// </summary>
        public HashAlgorithms  HashAlgorithm     { get; }

        /// <summary>
        /// The hashed value of the issuer distinguished name (DN). 128
        /// </summary>
        public String          IssuerNameHash    { get; }

        /// <summary>
        /// The hashed value of the issuers public key. 128
        /// </summary>
        public String          IssuerKeyHash     { get; }

        /// <summary>
        /// The serial number of the certificate to verify. 40
        /// </summary>
        public String          SerialNumber      { get; }

        /// <summary>
        /// The case-insensitive responder URL. 512
        /// </summary>
        public String          ResponderURL      { get; }

        /// <summary>
        /// An optional custom data object to allow to store any kind of customer specific data.
        /// </summary>
        public CustomData      CustomData        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new wireless communication module.
        /// </summary>
        /// <param name="HashAlgorithm">The used hash algorithm.</param>
        /// <param name="IssuerNameHash">The hashed value of the issuer distinguished name (DN).</param>
        /// <param name="IssuerKeyHash">The hashed value of the issuers public key.</param>
        /// <param name="SerialNumber">The serial number of the certificate to verify.</param>
        /// <param name="ResponderURL">The case-insensitive responder URL.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public OCSPRequestData(HashAlgorithms  HashAlgorithm,
                               String          IssuerNameHash,
                               String          IssuerKeyHash,
                               String          SerialNumber,
                               String          ResponderURL,
                               CustomData      CustomData   = null)
        {

            this.HashAlgorithm   = HashAlgorithm;
            this.IssuerNameHash  = IssuerNameHash?.Trim();
            this.IssuerKeyHash   = IssuerKeyHash?. Trim();
            this.SerialNumber    = SerialNumber?.  Trim();
            this.ResponderURL    = ResponderURL?.  Trim();
            this.CustomData      = CustomData;

            if (this.IssuerNameHash.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(IssuerNameHash),  "The given issuer name hash must not be null or empty!");

            if (this.IssuerKeyHash.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(IssuerKeyHash),   "The given issuer key hash must not be null or empty!");

            if (this.SerialNumber.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(SerialNumber),    "The given serial number must not be null or empty!");

            if (this.ResponderURL.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ResponderURL),    "The given responder URL must not be null or empty!");

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:OCSPRequestDataType",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "description": "Information needed to verify the EV Contract Certificate via OCSP",
        //   "javaType": "OCSPRequestData",
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
        //       "description": "Hashed value of the Issuer DN (Distinguished Name).\r\n\r\n",
        //       "type": "string",
        //       "maxLength": 128
        //     },
        //     "issuerKeyHash": {
        //       "description": "Hashed value of the issuers public key\r\n",
        //       "type": "string",
        //       "maxLength": 128
        //     },
        //     "serialNumber": {
        //       "description": "The serial number of the certificate.\r\n",
        //       "type": "string",
        //       "maxLength": 40
        //     },
        //     "responderURL": {
        //       "description": "This contains the responder URL (Case insensitive). \r\n\r\n",
        //       "type": "string",
        //       "maxLength": 512
        //     }
        //   },
        //   "required": [
        //     "hashAlgorithm",
        //     "issuerNameHash",
        //     "issuerKeyHash",
        //     "serialNumber",
        //     "responderURL"
        //   ]
        // }

        #endregion

        #region (static) Parse   (OCSPRequestDataJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="OCSPRequestDataJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static OCSPRequestData Parse(JObject              OCSPRequestDataJSON,
                                            OnExceptionDelegate  OnException   = null)
        {

            if (TryParse(OCSPRequestDataJSON,
                         out OCSPRequestData modem,
                         OnException))
            {
                return modem;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (OCSPRequestDataText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a communication module.
        /// </summary>
        /// <param name="OCSPRequestDataText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static OCSPRequestData Parse(String               OCSPRequestDataText,
                                            OnExceptionDelegate  OnException   = null)
        {


            if (TryParse(OCSPRequestDataText,
                         out OCSPRequestData modem,
                         OnException))
            {
                return modem;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(OCSPRequestDataJSON, out OCSPRequestData, OnException = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="OCSPRequestDataJSON">The JSON to be parsed.</param>
        /// <param name="OCSPRequestData">The parsed connector type.</param>
        public static Boolean TryParse(JObject              OCSPRequestDataJSON,
                                       out OCSPRequestData  OCSPRequestData)

            => TryParse(OCSPRequestDataJSON,
                        out OCSPRequestData,
                        null);

        /// <summary>
        /// Try to parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="OCSPRequestDataJSON">The JSON to be parsed.</param>
        /// <param name="OCSPRequestData">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject              OCSPRequestDataJSON,
                                       out OCSPRequestData  OCSPRequestData,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                OCSPRequestData = default;

                #region HashAlgorithm

                if (!OCSPRequestDataJSON.MapMandatory("hashAlgorithm",
                                                      "hash algorithm",
                                                      HashAlgorithmsExtentions.Parse,
                                                      out HashAlgorithms  HashAlgorithm,
                                                      out String          ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IssuerNameHash

                if (!OCSPRequestDataJSON.ParseMandatoryText("issuerNameHash",
                                                            "issuer name hash",
                                                            out String  IssuerNameHash,
                                                            out         ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IssuerKeyHash

                if (!OCSPRequestDataJSON.ParseMandatoryText("issuerKeyHash",
                                                            "issuer key hash",
                                                            out String  IssuerKeyHash,
                                                            out         ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SerialNumber

                if (!OCSPRequestDataJSON.ParseMandatoryText("serialNumber",
                                                            "certificate serial number",
                                                            out String  SerialNumber,
                                                            out         ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ResponderURL

                if (!OCSPRequestDataJSON.ParseMandatoryText("responderURL",
                                                            "responder URL",
                                                            out String  ResponderURL,
                                                            out         ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData

                if (OCSPRequestDataJSON.ParseOptionalJSON("customData",
                                                          "custom data",
                                                          OCPPv2_0.CustomData.TryParse,
                                                          out CustomData  CustomData,
                                                          out             ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion


                OCSPRequestData = new OCSPRequestData(HashAlgorithm,
                                                      IssuerNameHash?.Trim(),
                                                      IssuerKeyHash?. Trim(),
                                                      SerialNumber?.  Trim(),
                                                      ResponderURL?.  Trim(),
                                                      CustomData);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, OCSPRequestDataJSON, e);

                OCSPRequestData = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(OCSPRequestDataText, out OCSPRequestData, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a communication module.
        /// </summary>
        /// <param name="OCSPRequestDataText">The text to be parsed.</param>
        /// <param name="OCSPRequestData">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               OCSPRequestDataText,
                                       out OCSPRequestData  OCSPRequestData,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                OCSPRequestDataText = OCSPRequestDataText?.Trim();

                if (OCSPRequestDataText.IsNotNullOrEmpty() &&
                    TryParse(JObject.Parse(OCSPRequestDataText),
                             out OCSPRequestData,
                             OnException))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, OCSPRequestDataText, e);
            }

            OCSPRequestData = default;
            return false;

        }

        #endregion

        #region ToJSON(CustomOCSPRequestDataResponseSerializer = null, CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomOCSPRequestDataResponseSerializer">A delegate to serialize custom OCSPRequestDatas.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<OCSPRequestData> CustomOCSPRequestDataResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>      CustomCustomDataResponseSerializer        = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("hashAlgorithm",     HashAlgorithm),
                           new JProperty("issuerNameHash",    IssuerNameHash),
                           new JProperty("issuerKeyHash",     IssuerKeyHash),
                           new JProperty("serialNumber",      SerialNumber),
                           new JProperty("responderURL",      ResponderURL),

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomOCSPRequestDataResponseSerializer is not null
                       ? CustomOCSPRequestDataResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (OCSPRequestData1, OCSPRequestData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCSPRequestData1">An id tag info.</param>
        /// <param name="OCSPRequestData2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (OCSPRequestData OCSPRequestData1, OCSPRequestData OCSPRequestData2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(OCSPRequestData1, OCSPRequestData2))
                return true;

            // If one is null, but not both, return false.
            if (OCSPRequestData1 is null || OCSPRequestData2 is null)
                return false;

            if (OCSPRequestData1 is null)
                throw new ArgumentNullException(nameof(OCSPRequestData1),  "The given id tag info must not be null!");

            return OCSPRequestData1.Equals(OCSPRequestData2);

        }

        #endregion

        #region Operator != (OCSPRequestData1, OCSPRequestData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCSPRequestData1">An id tag info.</param>
        /// <param name="OCSPRequestData2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (OCSPRequestData OCSPRequestData1, OCSPRequestData OCSPRequestData2)
            => !(OCSPRequestData1 == OCSPRequestData2);

        #endregion

        #endregion

        #region IEquatable<OCSPRequestData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is OCSPRequestData OCSPRequestData))
                return false;

            return Equals(OCSPRequestData);

        }

        #endregion

        #region Equals(OCSPRequestData)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="OCSPRequestData">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(OCSPRequestData OCSPRequestData)
        {

            if (OCSPRequestData is null)
                return false;

            return HashAlgorithm. Equals(OCSPRequestData.HashAlgorithm)  &&
                   IssuerNameHash.Equals(OCSPRequestData.IssuerNameHash) &&
                   IssuerKeyHash. Equals(OCSPRequestData.IssuerKeyHash)  &&
                   SerialNumber.  Equals(OCSPRequestData.SerialNumber)   &&
                   ResponderURL.  Equals(OCSPRequestData.ResponderURL)   &&

                   ((CustomData == null && OCSPRequestData.CustomData == null) ||
                    (CustomData is not null && OCSPRequestData.CustomData is not null && CustomData.Equals(OCSPRequestData.CustomData)));

        }

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

                return HashAlgorithm. GetHashCode() * 13 ^
                       IssuerNameHash.GetHashCode() * 11 ^
                       IssuerKeyHash. GetHashCode() *  7 ^
                       SerialNumber.  GetHashCode() *  5 ^
                       ResponderURL.  GetHashCode() *  3 ^

                       (CustomData is not null
                            ? CustomData.GetHashCode()
                            : 0);

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
