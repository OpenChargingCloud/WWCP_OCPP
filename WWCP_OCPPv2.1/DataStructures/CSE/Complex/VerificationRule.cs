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
    /// An OCPP CSE cryptographic signature verification entry.
    /// </summary>
    public class VerificationRule : ACustomData,
                                           IEquatable<VerificationRule>
    {

        #region Properties

        /// <summary>
        /// The priority of the cryptographic signature verification entry.
        /// </summary>
        public UInt32                    Priority    { get; }

        /// <summary>
        /// The context of the cryptographic signature verification entry.
        /// </summary>
        public JSONLDContext             Context     { get; }

        /// <summary>
        /// An optional cryptographic key pair for verification.
        /// </summary>
        public KeyPair?                  KeyPair     { get; }

        /// <summary>
        /// The context of the cryptographic signature verification action.
        /// </summary>
        public VerificationRuleAction  Action      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP CSE cryptographic signature verification entry.
        /// </summary>
        /// <param name="Priority">The priority of the cryptographic signature entry.</param>
        /// <param name="Context">The context of the cryptographic signature entry.</param>
        /// <param name="Action">The context of the cryptographic signature action.</param>
        /// <param name="KeyPair">The optional cryptographic key pair for verification.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public VerificationRule(UInt32                    Priority,
                                       JSONLDContext             Context,
                                       VerificationRuleAction  Action,
                                       KeyPair?                  KeyPair      = null,
                                       CustomData?               CustomData   = null)

            : base(CustomData)

        {

            this.Priority  = Priority;
            this.Context   = Context;
            this.Action    = Action;
            this.KeyPair   = KeyPair;

            unchecked
            {

                hashCode = Priority.GetHashCode()       * 11 ^
                           Context. GetHashCode()       *  7 ^
                           Action.  GetHashCode()       *  5 ^
                          (KeyPair?.GetHashCode() ?? 0) *  3 ^
                           base.    GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, CustomVerificationPolicyEntryParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cryptographic signature.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomVerificationPolicyEntryParser">A delegate to parse custom cryptographic signatures.</param>
        public static VerificationRule Parse(JObject                                  JSON,
                                      CustomJObjectParserDelegate<VerificationRule>?  CustomVerificationPolicyEntryParser   = null)
        {

            if (TryParse(JSON,
                         out var signature,
                         out var errorResponse,
                         CustomVerificationPolicyEntryParser))
            {
                return signature!;
            }

            throw new ArgumentException("The given JSON representation of a signature is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out VerificationPolicyEntry, out ErrorResponse, CustomVerificationPolicyEntryParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a signature.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VerificationPolicyEntry">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject         JSON,
                                       out VerificationRule?  VerificationPolicyEntry,
                                       out String?     ErrorResponse)

            => TryParse(JSON,
                        out VerificationPolicyEntry,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a signature.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VerificationPolicyEntry">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVerificationPolicyEntryParser">A delegate to parse custom signatures.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       out VerificationRule?                           VerificationPolicyEntry,
                                       out String?                              ErrorResponse,
                                       CustomJObjectParserDelegate<VerificationRule>?  CustomVerificationPolicyEntryParser   = null)
        {

            try
            {

                VerificationPolicyEntry = default;

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


                //VerificationPolicyEntry = new VerificationPolicyEntry(
                //                KeyId,
                //                Value,
                //                Algorithm,
                //                SigningMethod,
                //                EncodingMethod,
                //                CustomData
                //            );

                if (CustomVerificationPolicyEntryParser is not null)
                    VerificationPolicyEntry = CustomVerificationPolicyEntryParser(JSON,
                                                      VerificationPolicyEntry);

                return true;

            }
            catch (Exception e)
            {
                VerificationPolicyEntry      = default;
                ErrorResponse  = "The given JSON representation of a signature is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVerificationPolicyEntrySerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVerificationPolicyEntrySerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<VerificationRule>?   CustomVerificationPolicyEntrySerializer    = null,
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

            return CustomVerificationPolicyEntrySerializer is not null
                       ? CustomVerificationPolicyEntrySerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (VerificationPolicyEntry1, VerificationPolicyEntry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VerificationPolicyEntry1">A signature.</param>
        /// <param name="VerificationPolicyEntry2">Another signature.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VerificationRule? VerificationPolicyEntry1,
                                           VerificationRule? VerificationPolicyEntry2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(VerificationPolicyEntry1, VerificationPolicyEntry2))
                return true;

            // If one is null, but not both, return false.
            if (VerificationPolicyEntry1 is null || VerificationPolicyEntry2 is null)
                return false;

            return VerificationPolicyEntry1.Equals(VerificationPolicyEntry2);

        }

        #endregion

        #region Operator != (VerificationPolicyEntry1, VerificationPolicyEntry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VerificationPolicyEntry1">A signature.</param>
        /// <param name="VerificationPolicyEntry2">Another signature.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VerificationRule? VerificationPolicyEntry1,
                                           VerificationRule? VerificationPolicyEntry2)

            => !(VerificationPolicyEntry1 == VerificationPolicyEntry2);

        #endregion

        #endregion

        #region IEquatable<VerificationPolicyEntry> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signatures for equality.
        /// </summary>
        /// <param name="Object">A signature to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VerificationRule signature &&
                   Equals(signature);

        #endregion

        #region Equals(VerificationPolicyEntry)

        /// <summary>
        /// Compares two signatures for equality.
        /// </summary>
        /// <param name="VerificationPolicyEntry">A signature to compare with.</param>
        public Boolean Equals(VerificationRule? VerificationPolicyEntry)

            => VerificationPolicyEntry is not null &&

               Priority.Equals(VerificationPolicyEntry.Priority) &&
               Context. Equals(VerificationPolicyEntry.Context)  &&
               Action.  Equals(VerificationPolicyEntry.Action)   &&

             ((KeyPair is null     && VerificationPolicyEntry.KeyPair is null) ||
              (KeyPair is not null && VerificationPolicyEntry.KeyPair is not null && KeyPair.Equals(VerificationPolicyEntry.KeyPair))) &&

               base.    Equals(VerificationPolicyEntry);

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
