/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An OCPP CSE cryptographic signature entry.
    /// </summary>
    public class SignaturePolicyEntry : ACustomData,
                                        IEquatable<SignaturePolicyEntry>
    {

        #region Properties

        /// <summary>
        /// The priority of the cryptographic signature entry.
        /// </summary>
        public UInt32                               Priority                { get; }

        /// <summary>
        /// The context of the cryptographic signature entry.
        /// </summary>
        public JSONLDContext                        Context                 { get; }

        /// <summary>
        /// The context of the cryptographic signature action.
        /// </summary>
        public SignaturePolicyAction                Action                  { get; }

        /// <summary>
        /// An optional user identification generator for signing.
        /// </summary>
        public KeyPair?                             KeyPair                 { get; }

        /// <summary>
        /// An optional user identification generator for signing.
        /// </summary>
        public Func<ISignableMessage, String>?      UserIdGenerator         { get; }

        /// <summary>
        /// An optional multi-language description generator for signing.
        /// </summary>
        public Func<ISignableMessage, I18NString>?  DescriptionGenerator    { get; }

        /// <summary>
        /// An optional timestamp generator for signing.
        /// </summary>
        public Func<ISignableMessage, DateTime>?    TimestampGenerator      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP CSE cryptographic signature entry.
        /// </summary>
        /// <param name="Priority">The priority of the cryptographic signature entry.</param>
        /// <param name="Context">The context of the cryptographic signature entry.</param>
        /// <param name="Action">The context of the cryptographic signature action.</param>
        /// <param name="KeyPair">The optional cryptographic key pair for signing or verification.</param>
        /// <param name="UserIdGenerator">An optional user identification generator for signing.</param>
        /// <param name="DescriptionGenerator">An optional multi-language description generator for signing.</param>
        /// <param name="TimestampGenerator">An optional timestamp generator for signing.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SignaturePolicyEntry(UInt32                               Priority,
                                    JSONLDContext                        Context,
                                    SignaturePolicyAction                Action,
                                    KeyPair?                             KeyPair                = null,
                                    Func<ISignableMessage, String>?      UserIdGenerator        = null,
                                    Func<ISignableMessage, I18NString>?  DescriptionGenerator   = null,
                                    Func<ISignableMessage, DateTime>?    TimestampGenerator     = null,
                                    CustomData?                          CustomData             = null)

            : base(CustomData)

        {

            this.Priority              = Priority;
            this.Context               = Context;
            this.Action                = Action;
            this.KeyPair               = KeyPair;
            this.UserIdGenerator       = UserIdGenerator;
            this.DescriptionGenerator  = DescriptionGenerator;
            this.TimestampGenerator    = TimestampGenerator;

            unchecked
            {

                hashCode = Priority.             GetHashCode()       * 19 ^
                           Context.              GetHashCode()       * 17 ^
                           Action.               GetHashCode()       * 13 ^
                          (KeyPair?.             GetHashCode() ?? 0) * 11 ^
                          (UserIdGenerator?.     GetHashCode() ?? 0) *  7 ^
                          (DescriptionGenerator?.GetHashCode() ?? 0) *  5 ^
                          (TimestampGenerator?.  GetHashCode() ?? 0) *  3 ^
                           base.                 GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, CustomSignaturePolicyEntryParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cryptographic signature.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSignaturePolicyEntryParser">A delegate to parse custom cryptographic signatures.</param>
        public static SignaturePolicyEntry Parse(JObject                                  JSON,
                                      CustomJObjectParserDelegate<SignaturePolicyEntry>?  CustomSignaturePolicyEntryParser   = null)
        {

            if (TryParse(JSON,
                         out var signature,
                         out var errorResponse,
                         CustomSignaturePolicyEntryParser))
            {
                return signature!;
            }

            throw new ArgumentException("The given JSON representation of a signature is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SignaturePolicyEntry, out ErrorResponse, CustomSignaturePolicyEntryParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a signature.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SignaturePolicyEntry">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject         JSON,
                                       out SignaturePolicyEntry?  SignaturePolicyEntry,
                                       out String?     ErrorResponse)

            => TryParse(JSON,
                        out SignaturePolicyEntry,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a signature.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SignaturePolicyEntry">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignaturePolicyEntryParser">A delegate to parse custom signatures.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       out SignaturePolicyEntry?                           SignaturePolicyEntry,
                                       out String?                              ErrorResponse,
                                       CustomJObjectParserDelegate<SignaturePolicyEntry>?  CustomSignaturePolicyEntryParser   = null)
        {

            try
            {

                SignaturePolicyEntry = default;

                //#region KeyId             [mandatory]

                //if (!JSON.ParseMandatoryText("keyId",
                //                             "key identification",
                //                             out String KeyId,
                //                             out ErrorResponse))
                //{
                //    return false;
                //}

                //#endregion

                //#region Value             [mandatory]

                //if (!JSON.ParseMandatoryText("value",
                //                             "signature value",
                //                             out String Value,
                //                             out ErrorResponse))
                //{
                //    return false;
                //}

                //#endregion

                //#region Algorithm         [optional]

                //var Algorithm = JSON.GetString("algorithm");

                //#endregion

                //#region SigningMethod     [optional]

                //var SigningMethod   = JSON.GetString("signingMethod");

                //#endregion

                //#region EncodingMethod    [optional]

                //var EncodingMethod  = JSON.GetString("encodingMethod");

                //#endregion

                //#region CustomData        [optional]

                //if (JSON.ParseOptionalJSON("customData",
                //                           "custom data",
                //                           OCPPv2_1.CustomData.TryParse,
                //                           out CustomData CustomData,
                //                           out ErrorResponse))
                //{
                //    if (ErrorResponse is not null)
                //        return false;
                //}

                //#endregion

                ErrorResponse = null;


                //SignaturePolicyEntry = new SignaturePolicyEntry(
                //                KeyId,
                //                Value,
                //                Algorithm,
                //                SigningMethod,
                //                EncodingMethod,
                //                CustomData
                //            );

                if (CustomSignaturePolicyEntryParser is not null)
                    SignaturePolicyEntry = CustomSignaturePolicyEntryParser(JSON,
                                                      SignaturePolicyEntry);

                return true;

            }
            catch (Exception e)
            {
                SignaturePolicyEntry      = default;
                ErrorResponse  = "The given JSON representation of a signature is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignaturePolicyEntrySerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignaturePolicyEntrySerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignaturePolicyEntry>?   CustomSignaturePolicyEntrySerializer    = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                           //      new JProperty("keyId",            KeyId),
                           //      new JProperty("value",            Value),
                           //      new JProperty("signingMethod",    SigningMethod),
                           //      new JProperty("encodingMethod",   EncodingMethod),

                           //String.Equals(Algorithm, "secp256r1",
                           //              StringComparison.OrdinalIgnoreCase)
                           //    ? null
                           //    : new JProperty("algorithm",        Algorithm),

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSignaturePolicyEntrySerializer is not null
                       ? CustomSignaturePolicyEntrySerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SignaturePolicyEntry1, SignaturePolicyEntry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignaturePolicyEntry1">A signature.</param>
        /// <param name="SignaturePolicyEntry2">Another signature.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SignaturePolicyEntry? SignaturePolicyEntry1,
                                           SignaturePolicyEntry? SignaturePolicyEntry2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SignaturePolicyEntry1, SignaturePolicyEntry2))
                return true;

            // If one is null, but not both, return false.
            if (SignaturePolicyEntry1 is null || SignaturePolicyEntry2 is null)
                return false;

            return SignaturePolicyEntry1.Equals(SignaturePolicyEntry2);

        }

        #endregion

        #region Operator != (SignaturePolicyEntry1, SignaturePolicyEntry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignaturePolicyEntry1">A signature.</param>
        /// <param name="SignaturePolicyEntry2">Another signature.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SignaturePolicyEntry? SignaturePolicyEntry1,
                                           SignaturePolicyEntry? SignaturePolicyEntry2)

            => !(SignaturePolicyEntry1 == SignaturePolicyEntry2);

        #endregion

        #endregion

        #region IEquatable<SignaturePolicyEntry> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signatures for equality.
        /// </summary>
        /// <param name="Object">A signature to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignaturePolicyEntry signature &&
                   Equals(signature);

        #endregion

        #region Equals(SignaturePolicyEntry)

        /// <summary>
        /// Compares two signatures for equality.
        /// </summary>
        /// <param name="SignaturePolicyEntry">A signature to compare with.</param>
        public Boolean Equals(SignaturePolicyEntry? SignaturePolicyEntry)

            => SignaturePolicyEntry is not null &&

               Priority.Equals(SignaturePolicyEntry.Priority) &&
               Context. Equals(SignaturePolicyEntry.Context)  &&
               Action.  Equals(SignaturePolicyEntry.Action)   &&

             ((KeyPair is null     && SignaturePolicyEntry.KeyPair is null) ||
              (KeyPair is not null && SignaturePolicyEntry.KeyPair is not null && KeyPair.Equals(SignaturePolicyEntry.KeyPair))) &&

               base.    Equals(SignaturePolicyEntry);

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

            => $"{Context} => {Action} ({Priority})";

        #endregion

    }

}
