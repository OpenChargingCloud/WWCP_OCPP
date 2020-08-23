/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

#endregion

namespace cloud.charging.adapters.OCPPv2_0
{

    /// <summary>
    /// A signed (energy) meter value.
    /// </summary>
    public class SignedMeterValue
    {

        #region Properties

        /// <summary>
        /// The signed meter value (base64 encoded). 2500
        /// </summary>
        public String      SignedMeterData    { get; }

        /// <summary>
        /// Method used to create the digital signature. 50
        /// </summary>
        public String      SigningMethod      { get; }

        /// <summary>
        /// Method used to encode the meter values before applying the digital signature algorithm. 50
        /// </summary>
        public String      EncodingMethod     { get; }

        /// <summary>
        /// The optional public key (base64 encoded). 2500
        /// </summary>
        public String      PublicKey          { get; }

        /// <summary>
        /// An optional custom data object to allow to store any kind of customer specific data.
        /// </summary>
        public CustomData  CustomData         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new signed (energy) meter value.
        /// </summary>
        /// <param name="SignedMeterData">The signed meter value (base64 encoded).</param>
        /// <param name="SigningMethod">Method used to create the digital signature.</param>
        /// <param name="EncodingMethod">Method used to encode the meter values before applying the digital signature algorithm.</param>
        /// <param name="PublicKey">The optional public key (base64 encoded).</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SignedMeterValue(String      SignedMeterData,
                                String      SigningMethod,
                                String      EncodingMethod,
                                String      PublicKey    = null,
                                CustomData  CustomData   = null)
        {

            this.SignedMeterData  = SignedMeterData;
            this.SigningMethod    = SigningMethod;
            this.EncodingMethod   = EncodingMethod;
            this.PublicKey        = PublicKey;
            this.CustomData       = CustomData;

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

        #region (static) Parse   (SignedMeterValueJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="SignedMeterValueJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SignedMeterValue Parse(JObject              SignedMeterValueJSON,
                                             OnExceptionDelegate  OnException   = null)
        {

            if (TryParse(SignedMeterValueJSON,
                         out SignedMeterValue evse,
                         OnException))
            {
                return evse;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (SignedMeterValueText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a communication module.
        /// </summary>
        /// <param name="SignedMeterValueText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SignedMeterValue Parse(String               SignedMeterValueText,
                                             OnExceptionDelegate  OnException   = null)
        {


            if (TryParse(SignedMeterValueText,
                         out SignedMeterValue evse,
                         OnException))
            {
                return evse;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(SignedMeterValueJSON, out SignedMeterValue, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="SignedMeterValueJSON">The JSON to be parsed.</param>
        /// <param name="SignedMeterValue">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject               SignedMeterValueJSON,
                                       out SignedMeterValue  SignedMeterValue,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                SignedMeterValue = default;

                #region SignedMeterData

                if (!SignedMeterValueJSON.ParseMandatoryText("signedMeterData",
                                                             "signed meter data",
                                                             out String  SignedMeterData,
                                                             out String  ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SigningMethod

                if (!SignedMeterValueJSON.ParseMandatoryText("SigningMethod",
                                                             "signing method",
                                                             out String  SigningMethod,
                                                             out         ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EncodingMethod

                if (!SignedMeterValueJSON.ParseMandatoryText("encodingMethod",
                                                             "encoding method",
                                                             out String  EncodingMethod,
                                                             out         ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region PublicKey

                if (!SignedMeterValueJSON.ParseMandatoryText("publicKey",
                                                             "public key",
                                                             out String  PublicKey,
                                                             out         ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData

                if (SignedMeterValueJSON.ParseOptionalJSON("customData",
                                                           "custom data",
                                                           OCPPv2_0.CustomData.TryParse,
                                                           out CustomData  CustomData,
                                                           out             ErrorResponse,
                                                           OnException))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                SignedMeterValue = new SignedMeterValue(SignedMeterData,
                                                        SigningMethod,
                                                        EncodingMethod,
                                                        PublicKey,
                                                        CustomData);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, SignedMeterValueJSON, e);

                SignedMeterValue = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(SignedMeterValueText, out SignedMeterValue, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a communication module.
        /// </summary>
        /// <param name="SignedMeterValueText">The text to be parsed.</param>
        /// <param name="SignedMeterValue">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                SignedMeterValueText,
                                       out SignedMeterValue  SignedMeterValue,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                SignedMeterValueText = SignedMeterValueText?.Trim();

                if (SignedMeterValueText.IsNotNullOrEmpty() &&
                    TryParse(JObject.Parse(SignedMeterValueText),
                             out SignedMeterValue,
                             OnException))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, SignedMeterValueText, e);
            }

            SignedMeterValue = default;
            return false;

        }

        #endregion

        #region ToJSON(CustomSignedMeterValueResponseSerializer = null, CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignedMeterValueResponseSerializer">A delegate to serialize custom SignedMeterValue objects.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignedMeterValue> CustomSignedMeterValueResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>       CustomCustomDataResponseSerializer         = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("signedMeterData",   SignedMeterData),
                           new JProperty("signingMethod",     SigningMethod),
                           new JProperty("encodingMethod",    EncodingMethod),

                           PublicKey != null
                               ? new JProperty("publicKey",   PublicKey)
                               : null,

                           CustomData != null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomSignedMeterValueResponseSerializer != null
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
        public static Boolean operator == (SignedMeterValue SignedMeterValue1, SignedMeterValue SignedMeterValue2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SignedMeterValue1, SignedMeterValue2))
                return true;

            // If one is null, but not both, return false.
            if (SignedMeterValue1 is null || SignedMeterValue2 is null)
                return false;

            if (SignedMeterValue1 is null)
                throw new ArgumentNullException(nameof(SignedMeterValue1),  "The given id tag info must not be null!");

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
        public static Boolean operator != (SignedMeterValue SignedMeterValue1, SignedMeterValue SignedMeterValue2)
            => !(SignedMeterValue1 == SignedMeterValue2);

        #endregion

        #endregion

        #region IEquatable<SignedMeterValue> Members

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

            if (!(Object is SignedMeterValue SignedMeterValue))
                return false;

            return Equals(SignedMeterValue);

        }

        #endregion

        #region Equals(SignedMeterValue)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="SignedMeterValue">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(SignedMeterValue SignedMeterValue)
        {

            if (SignedMeterValue is null)
                return false;

            return String.Equals(SignedMeterData, SignedMeterValue.SignedMeterData, StringComparison.OrdinalIgnoreCase) &&
                   String.Equals(SigningMethod,   SignedMeterValue.SigningMethod,   StringComparison.OrdinalIgnoreCase) &&
                   String.Equals(EncodingMethod,  SignedMeterValue.EncodingMethod,  StringComparison.OrdinalIgnoreCase) &&

                   ((PublicKey == null  && SignedMeterValue.PublicKey  == null) ||
                    (PublicKey != null  && SignedMeterValue.PublicKey  != null && PublicKey. Equals(SignedMeterValue.PublicKey))) &&

                   ((CustomData == null && SignedMeterValue.CustomData == null) ||
                    (CustomData != null && SignedMeterValue.CustomData != null && CustomData.Equals(SignedMeterValue.CustomData)));

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

                return SignedMeterData.  GetHashCode() * 11 ^
                       SigningMethod.    GetHashCode() *  7 ^
                       EncodingMethod.   GetHashCode() *  5 ^

                       (PublicKey != null
                            ? PublicKey. GetHashCode() *  3
                            : 0) ^

                       (CustomData != null
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

            => String.Concat(SignedMeterData, ", ",
                             SigningMethod,   ", ",
                             EncodingMethod);

        #endregion

    }

}
