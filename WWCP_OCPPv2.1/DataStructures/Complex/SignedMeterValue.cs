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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A signed meter value.
    /// </summary>
    public class SignedMeterValue : ACustomData,
                                    IEquatable<SignedMeterValue>
    {

        #region Properties

        /// <summary>
        /// The signed meter value (base64 encoded).
        /// [max 2500]
        /// </summary>
        [Mandatory]
        public String   SignedMeterData    { get; }

        /// <summary>
        /// Method used to encode the meter values before applying the digital signature algorithm.
        /// [max 50]
        /// </summary>
        [Mandatory]
        public String   EncodingMethod     { get; }

        /// <summary>
        /// Optional method used to create the digital signature.
        /// This information might already be included within the signedMeterData.
        /// [max 50]
        /// </summary>
        [Optional]
        public String?  SigningMethod      { get; }

        /// <summary>
        /// The optional, but when it is given base64 encoded public key.
        /// [max 2500]
        /// </summary>
        [Optional]
        public String?  PublicKey          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new signed meter value.
        /// </summary>
        /// <param name="SignedMeterData">The signed meter value (base64 encoded).</param>
        /// <param name="EncodingMethod">Method used to encode the meter values before applying the digital signature algorithm.</param>
        /// <param name="SigningMethod">An optional method used to create the digital signature. This information might already be included within the signedMeterData.</param>
        /// <param name="PublicKey">An optional, but when it is given base64 encoded public key..</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public SignedMeterValue(String       SignedMeterData,
                                String       EncodingMethod,
                                String?      SigningMethod   = null,
                                String?      PublicKey       = null,
                                CustomData?  CustomData      = null)

            : base(CustomData)

        {

            this.SignedMeterData  = SignedMeterData;
            this.EncodingMethod   = EncodingMethod;
            this.SigningMethod    = SigningMethod;
            this.PublicKey        = PublicKey;

            unchecked
            {

                hashCode = SignedMeterData.GetHashCode()       * 11 ^
                           EncodingMethod. GetHashCode()       *  5 ^
                          (SigningMethod?. GetHashCode() ?? 0) *  7 ^
                          (PublicKey?.     GetHashCode() ?? 0) *  3 ^
                           base.           GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Represent a signed version of the meter value.",
        //     "javaType": "SignedMeterValue",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "signedMeterData": {
        //             "description": "Base64 encoded, contains the signed data from the meter in the format specified in _encodingMethod_,
        //                             which might contain more then just the meter value. It can contain information like timestamps,
        //                             reference to a customer etc.",
        //             "type": "string",
        //             "maxLength": 32768
        //         },
        //         "signingMethod": {
        //             "description": "Method used to create the digital signature. Optional, if already included in _signedMeterData_.
        //                             Standard values for this are defined in Appendix as SigningMethodEnumStringType.",
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "encodingMethod": {
        //             "description": "Format used by the energy meter to encode the meter data. For example: OCMF or EDL.",
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "publicKey": {
        //             "description": "Base64 encoded, sending depends on configuration variable _PublicKeyWithSignedMeterValue_.",
        //             "type": "string",
        //             "maxLength": 2500
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "signedMeterData",
        //         "encodingMethod"
        //     ]
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
                return signedMeterValue;
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
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out SignedMeterValue?  SignedMeterValue,
                                       [NotNullWhen(false)] out String?            ErrorResponse)

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
                                       [NotNullWhen(true)]  out SignedMeterValue?      SignedMeterValue,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       CustomJObjectParserDelegate<SignedMeterValue>?  CustomSignedMeterValueParser   = null)
        {

            try
            {

                SignedMeterValue = default;

                #region SignedMeterData    [mandatory]

                if (!JSON.ParseMandatoryText("signedMeterData",
                                             "signed meter data",
                                             out String SignedMeterData,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EncodingMethod     [mandatory]

                if (!JSON.ParseMandatoryText("encodingMethod",
                                             "encoding method",
                                             out String EncodingMethod,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SigningMethod      [optional]

                var SigningMethod = JSON.GetString("signingMethod");

                #endregion

                #region PublicKey          [optional]

                var PublicKey = JSON.GetString("publicKey");

                #endregion

                #region CustomData         [optional]

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


                SignedMeterValue = new SignedMeterValue(
                                       SignedMeterData,
                                       EncodingMethod,
                                       SigningMethod,
                                       PublicKey,
                                       CustomData
                                   );

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

        #region ToJSON(CustomSignedMeterValueSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignedMeterValueSerializer">A delegate to serialize custom signed meter values.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignedMeterValue>?  CustomSignedMeterValueSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?        CustomCustomDataSerializer         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("signedMeterData",   SignedMeterData),
                                 new JProperty("encodingMethod",    EncodingMethod),

                           SigningMethod.IsNotNullOrEmpty()
                               ? new JProperty("signingMethod",     SigningMethod)
                               : null,

                           PublicKey.    IsNotNullOrEmpty()
                               ? new JProperty("publicKey",         PublicKey)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSignedMeterValueSerializer is not null
                       ? CustomSignedMeterValueSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this signed meter value.
        /// </summary>
        public SignedMeterValue Clone()

            => new (
                   new String(SignedMeterData.ToCharArray()),
                   new String(EncodingMethod. ToCharArray()),
                   SigningMethod.IsNotNullOrEmpty() ? new String(SigningMethod.  ToCharArray()) : null,
                   PublicKey.    IsNotNullOrEmpty() ? new String(PublicKey.      ToCharArray()) : null,
                   CustomData
               );

        #endregion


        #region Operator overloading

        #region Operator == (SignedMeterValue1, SignedMeterValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedMeterValue1">A signed meter value.</param>
        /// <param name="SignedMeterValue2">Another signed meter value.</param>
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
        /// <param name="SignedMeterValue1">A signed meter value.</param>
        /// <param name="SignedMeterValue2">Another signed meter value.</param>
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
        public Boolean Equals(SignedMeterValue? SignedMeterValue)

            => SignedMeterValue is not null &&

               String.Equals(SignedMeterData, SignedMeterValue.SignedMeterData, StringComparison.Ordinal) &&
               String.Equals(SigningMethod,   SignedMeterValue.SigningMethod,   StringComparison.Ordinal) &&
               String.Equals(EncodingMethod,  SignedMeterValue.EncodingMethod,  StringComparison.Ordinal) &&
               String.Equals(PublicKey,       SignedMeterValue.EncodingMethod,  StringComparison.Ordinal) &&

               base.  Equals(SignedMeterValue);

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

            => String.Concat(

                   $"{EncodingMethod}: '{SignedMeterData}'",

                   SigningMethod.IsNotNullOrEmpty()
                       ? $", signing method: {SigningMethod}"
                       : "",

                   PublicKey.    IsNotNullOrEmpty()
                       ? $", public key: {PublicKey}"
                       : ""

               );

        #endregion

    }

}
