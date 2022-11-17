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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A signed meter value.
    /// </summary>
    public class SignedMeterValue : ACustomData
    {

        #region Properties

        /// <summary>
        /// The signed meter value (base64 encoded). 2500
        /// </summary>
        public String   SignedMeterData    { get; }

        /// <summary>
        /// Method used to create the digital signature. 50
        /// </summary>
        public String   SigningMethod      { get; }

        /// <summary>
        /// Method used to encode the meter values before applying the digital signature algorithm. 50
        /// </summary>
        public String   EncodingMethod     { get; }

        /// <summary>
        /// The optional public key (base64 encoded). 2500
        /// </summary>
        public String?  PublicKey          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new signed meter value.
        /// </summary>
        /// <param name="SignedMeterData">The signed meter value (base64 encoded).</param>
        /// <param name="SigningMethod">Method used to create the digital signature.</param>
        /// <param name="EncodingMethod">Method used to encode the meter values before applying the digital signature algorithm.</param>
        /// <param name="PublicKey">The optional public key (base64 encoded).</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SignedMeterValue(String       SignedMeterData,
                                String       SigningMethod,
                                String       EncodingMethod,
                                String?      PublicKey    = null,
                                CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.SignedMeterData  = SignedMeterData;
            this.SigningMethod    = SigningMethod;
            this.EncodingMethod   = EncodingMethod;
            this.PublicKey        = PublicKey;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SignedMeterValueType",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "description": "Represent a signed version of the meter value.\r\n",
        //   "javaType": "SignedMeterValue",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "signedMeterData": {
        //       "description": "Base64 encoded, contains the signed data which might contain more then just the meter value. It can contain information like timestamps, reference to a customer etc.\r\n",
        //       "type": "string",
        //       "maxLength": 2500
        //     },
        //     "signingMethod": {
        //       "description": "Method used to create the digital signature.\r\n",
        //       "type": "string",
        //       "maxLength": 50
        //     },
        //     "encodingMethod": {
        //       "description": "Method used to encode the meter values before applying the digital signature algorithm.\r\n",
        //       "type": "string",
        //       "maxLength": 50
        //     },
        //     "publicKey": {
        //       "description": "Base64 encoded, sending depends on configuration variable _PublicKeyWithSignedMeterValue_.\r\n",
        //       "type": "string",
        //       "maxLength": 2500
        //     }
        //   },
        //   "required": [
        //     "signedMeterData",
        //     "signingMethod",
        //     "encodingMethod",
        //     "publicKey"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomSignedMeterValueParser = null)

        /// <summary>
        /// Parse the given JSON representation of a signed meter value.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSignedMeterValueParser">A delegate to parse custom signed meter values.</param>
        public static SignedMeterValue Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<SignedMeterValue>?  CustomSignedMeterValueParser   = null)
        {

            if (TryParse(JSON,
                         out var signedMeterValue,
                         out var errorResponse,
                         CustomSignedMeterValueParser))
            {
                return signedMeterValue!;
            }

            throw new ArgumentException("The given JSON representation of a signed meter value is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SignedMeterValue, out ErrorResponse, CustomSignedMeterValueParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a signed meter value.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SignedMeterValue">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                JSON,
                                       out SignedMeterValue?  SignedMeterValue,
                                       out String?            ErrorResponse)

            => TryParse(JSON,
                        out SignedMeterValue,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a signed meter value.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SignedMeterValue">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignedMeterValueParser">A delegate to parse custom signed meter values.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       out SignedMeterValue?                           SignedMeterValue,
                                       out String?                                     ErrorResponse,
                                       CustomJObjectParserDelegate<SignedMeterValue>?  CustomSignedMeterValueParser   = null)
        {

            try
            {

                SignedMeterValue = default;

                #region SignedMeterData

                if (!JSON.ParseMandatoryText("signedMeterData",
                                             "signed meter data",
                                             out String SignedMeterData,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SigningMethod

                if (!JSON.ParseMandatoryText("SigningMethod",
                                             "signing method",
                                             out String SigningMethod,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EncodingMethod

                if (!JSON.ParseMandatoryText("encodingMethod",
                                             "encoding method",
                                             out String EncodingMethod,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region PublicKey

                if (!JSON.ParseMandatoryText("publicKey",
                                             "public key",
                                             out String PublicKey,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData

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


                SignedMeterValue = new SignedMeterValue(SignedMeterData,
                                                        SigningMethod,
                                                        EncodingMethod,
                                                        PublicKey,
                                                        CustomData);

                if (CustomSignedMeterValueParser is not null)
                    SignedMeterValue = CustomSignedMeterValueParser(JSON,
                                                                    SignedMeterValue);

                return true;

            }
            catch (Exception e)
            {
                SignedMeterValue  = default;
                ErrorResponse     = "The given JSON representation of a signed meter value is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignedMeterValueResponseSerializer = null, CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignedMeterValueResponseSerializer">A delegate to serialize custom signed meter values.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignedMeterValue>?  CustomSignedMeterValueResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?        CustomCustomDataResponseSerializer         = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("signedMeterData",   SignedMeterData),
                           new JProperty("signingMethod",     SigningMethod),
                           new JProperty("encodingMethod",    EncodingMethod),

                           PublicKey  is not null
                               ? new JProperty("publicKey",   PublicKey)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomSignedMeterValueResponseSerializer is not null
                       ? CustomSignedMeterValueResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SignedMeterValue1, SignedMeterValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedMeterValue1">An id tag info.</param>
        /// <param name="SignedMeterValue2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SignedMeterValue? SignedMeterValue1,
                                           SignedMeterValue? SignedMeterValue2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SignedMeterValue1, SignedMeterValue2))
                return true;

            // If one is null, but not both, return false.
            if (SignedMeterValue1 is null || SignedMeterValue2 is null)
                return false;

            return SignedMeterValue1.Equals(SignedMeterValue2);

        }

        #endregion

        #region Operator != (SignedMeterValue1, SignedMeterValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedMeterValue1">An id tag info.</param>
        /// <param name="SignedMeterValue2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SignedMeterValue? SignedMeterValue1,
                                           SignedMeterValue? SignedMeterValue2)

            => !(SignedMeterValue1 == SignedMeterValue2);

        #endregion

        #endregion

        #region IEquatable<SignedMeterValue> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signed meter values for equality.
        /// </summary>
        /// <param name="Object">A signed meter value to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignedMeterValue signedMeterValue &&
                   Equals(signedMeterValue);

        #endregion

        #region Equals(SignedMeterValue)

        /// <summary>
        /// Compares two signed meter values for equality.
        /// </summary>
        /// <param name="SignedMeterValue">A signed meter value to compare with.</param>
        public Boolean Equals(SignedMeterValue SignedMeterValue)

            => SignedMeterValue is not null &&

               String.Equals(SignedMeterData, SignedMeterValue.SignedMeterData, StringComparison.OrdinalIgnoreCase) &&
               String.Equals(SigningMethod,   SignedMeterValue.SigningMethod,   StringComparison.OrdinalIgnoreCase) &&
               String.Equals(EncodingMethod,  SignedMeterValue.EncodingMethod,  StringComparison.OrdinalIgnoreCase) &&

             ((PublicKey is     null  && SignedMeterValue.PublicKey  is     null) ||
              (PublicKey is not null  && SignedMeterValue.PublicKey  is not null && PublicKey. Equals(SignedMeterValue.PublicKey))) &&

               base.  Equals(SignedMeterValue);

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

                return SignedMeterData.GetHashCode()       * 11 ^
                       SigningMethod.  GetHashCode()       *  7 ^
                       EncodingMethod. GetHashCode()       *  5 ^

                       (PublicKey?.    GetHashCode() ?? 0) *  3 ^
                       base.           GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(SignedMeterData, ", ",
                             SigningMethod,   ", ",
                             EncodingMethod);

        #endregion

    }

}
