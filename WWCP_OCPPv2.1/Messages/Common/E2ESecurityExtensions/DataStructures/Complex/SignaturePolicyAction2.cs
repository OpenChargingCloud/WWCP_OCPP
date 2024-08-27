/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.WWCP;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An OCPP CSE cryptographic signature.
    /// </summary>
    public class SignaturePolicyAction2 : ACustomData,
                                         IEquatable<SignaturePolicyAction2>
    {

        #region Properties

        /// <summary>
        /// The unique key identification, e.g. the prefix of the public key.
        /// </summary>
        [Mandatory]
        public String   KeyId              { get; }

        /// <summary>
        /// The signature value.
        /// </summary>
        [Mandatory]
        public String   Value              { get; }

        [Optional]
        public String   Algorithm            { get; }

        /// <summary>
        /// The optional method used to create the digital signature.
        /// </summary>
        [Optional]
        public String?  SigningMethod      { get; }

        /// <summary>
        /// The optional encoding method.
        /// </summary>
        [Optional]
        public String?  EncodingMethod     { get; }



        public Boolean? Status { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP CSE cryptographic signature.
        /// </summary>
        /// <param name="KeyId">An unique key identification, e.g. a prefix of a public key.</param>
        /// <param name="Value">A signature value.</param>
        /// <param name="SigningMethod">An optional method used to create the digital signature.</param>
        /// <param name="EncodingMethod">An optional encoding method.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SignaturePolicyAction2(String       KeyId,
                         String       Value,
                         String?      Algorithm        = "secp256r1",
                         String?      SigningMethod    = null,
                         String?      EncodingMethod   = null,
                         CustomData?  CustomData       = null)

            : base(CustomData)

        {

            this.KeyId            = KeyId;
            this.Value            = Value;
            this.Algorithm        = Algorithm ?? "secp256r1";
            this.SigningMethod    = SigningMethod;
            this.EncodingMethod   = EncodingMethod;

            unchecked
            {

                hashCode = KeyId.          GetHashCode()       * 11 ^
                           Value.          GetHashCode()       *  7 ^
                          (SigningMethod?. GetHashCode() ?? 0) *  5 ^
                          (EncodingMethod?.GetHashCode() ?? 0) *  3 ^

                           base.           GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, CustomSignaturePolicyAction2Parser = null)

        /// <summary>
        /// Parse the given JSON representation of a cryptographic signature.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSignaturePolicyAction2Parser">An optional delegate to parse custom cryptographic signatures.</param>
        public static SignaturePolicyAction2 Parse(JObject                                               JSON,
                                                   CustomJObjectParserDelegate<SignaturePolicyAction2>?  CustomSignaturePolicyAction2Parser   = null)
        {

            if (TryParse(JSON,
                         out var signaturePolicyAction2,
                         out var errorResponse,
                         CustomSignaturePolicyAction2Parser) &&
                signaturePolicyAction2 is not null)
            {
                return signaturePolicyAction2;
            }

            throw new ArgumentException("The given JSON representation of a signature is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SignaturePolicyAction2, out ErrorResponse, CustomSignaturePolicyAction2Parser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a signature.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SignaturePolicyAction2">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject         JSON,
                                       out SignaturePolicyAction2?  SignaturePolicyAction2,
                                       out String?     ErrorResponse)

            => TryParse(JSON,
                        out SignaturePolicyAction2,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a signature.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SignaturePolicyAction2">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignaturePolicyAction2Parser">An optional delegate to parse custom signatures.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       out SignaturePolicyAction2?                           SignaturePolicyAction2,
                                       out String?                              ErrorResponse,
                                       CustomJObjectParserDelegate<SignaturePolicyAction2>?  CustomSignaturePolicyAction2Parser   = null)
        {

            try
            {

                SignaturePolicyAction2 = default;

                #region KeyId             [mandatory]

                if (!JSON.ParseMandatoryText("keyId",
                                             "key identification",
                                             out String KeyId,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Value             [mandatory]

                if (!JSON.ParseMandatoryText("value",
                                             "signature value",
                                             out String Value,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Algorithm         [optional]

                var Algorithm = JSON.GetString("algorithm");

                #endregion

                #region SigningMethod     [optional]

                var SigningMethod   = JSON.GetString("signingMethod");

                #endregion

                #region EncodingMethod    [optional]

                var EncodingMethod  = JSON.GetString("encodingMethod");

                #endregion

                #region CustomData        [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SignaturePolicyAction2 = new SignaturePolicyAction2(
                                KeyId,
                                Value,
                                Algorithm,
                                SigningMethod,
                                EncodingMethod,
                                CustomData
                            );

                if (CustomSignaturePolicyAction2Parser is not null)
                    SignaturePolicyAction2 = CustomSignaturePolicyAction2Parser(JSON,
                                                      SignaturePolicyAction2);

                return true;

            }
            catch (Exception e)
            {
                SignaturePolicyAction2      = default;
                ErrorResponse  = "The given JSON representation of a signature is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignaturePolicyAction2Serializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignaturePolicyAction2Serializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignaturePolicyAction2>?   CustomSignaturePolicyAction2Serializer    = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("keyId",            KeyId),
                                 new JProperty("value",            Value),
                                 new JProperty("signingMethod",    SigningMethod),
                                 new JProperty("encodingMethod",   EncodingMethod),

                           String.Equals(Algorithm, "secp256r1",
                                         StringComparison.OrdinalIgnoreCase)
                               ? null
                               : new JProperty("algorithm",        Algorithm),

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSignaturePolicyAction2Serializer is not null
                       ? CustomSignaturePolicyAction2Serializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SignaturePolicyAction21, SignaturePolicyAction22)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignaturePolicyAction21">A signature.</param>
        /// <param name="SignaturePolicyAction22">Another signature.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SignaturePolicyAction2? SignaturePolicyAction21,
                                           SignaturePolicyAction2? SignaturePolicyAction22)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SignaturePolicyAction21, SignaturePolicyAction22))
                return true;

            // If one is null, but not both, return false.
            if (SignaturePolicyAction21 is null || SignaturePolicyAction22 is null)
                return false;

            return SignaturePolicyAction21.Equals(SignaturePolicyAction22);

        }

        #endregion

        #region Operator != (SignaturePolicyAction21, SignaturePolicyAction22)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignaturePolicyAction21">A signature.</param>
        /// <param name="SignaturePolicyAction22">Another signature.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SignaturePolicyAction2? SignaturePolicyAction21,
                                           SignaturePolicyAction2? SignaturePolicyAction22)

            => !(SignaturePolicyAction21 == SignaturePolicyAction22);

        #endregion

        #endregion

        #region IEquatable<SignaturePolicyAction2> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signatures for equality.
        /// </summary>
        /// <param name="Object">A signature to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignaturePolicyAction2 signature &&
                   Equals(signature);

        #endregion

        #region Equals(SignaturePolicyAction2)

        /// <summary>
        /// Compares two signatures for equality.
        /// </summary>
        /// <param name="SignaturePolicyAction2">A signature to compare with.</param>
        public Boolean Equals(SignaturePolicyAction2? SignaturePolicyAction2)

            => SignaturePolicyAction2 is not null &&

               String.Equals(KeyId,           SignaturePolicyAction2.KeyId,           StringComparison.Ordinal) &&
               String.Equals(Value,           SignaturePolicyAction2.EncodingMethod,  StringComparison.Ordinal) &&
               String.Equals(SigningMethod,   SignaturePolicyAction2.SigningMethod,   StringComparison.Ordinal) &&
               String.Equals(EncodingMethod,  SignaturePolicyAction2.EncodingMethod,  StringComparison.Ordinal) &&

               base.  Equals(SignaturePolicyAction2);

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

            => $"{KeyId}: {Value}";

        #endregion

    }

}
