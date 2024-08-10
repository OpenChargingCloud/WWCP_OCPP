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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An OCPP CSE cryptographic verificationRule verification entry.
    /// </summary>
    public class VerificationRule : ACustomData,
                                    IEquatable<VerificationRule>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this data structure.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/verificationRule");

        #endregion

        #region Properties

        /// <summary>
        /// The context of the cryptographic verificationRule verification entry.
        /// </summary>
        public JSONLDContext            Context     { get; }

        /// <summary>
        /// The priority of the cryptographic verificationRule verification entry.
        /// </summary>
        public UInt32                   Priority    { get; }

        /// <summary>
        /// An optional cryptographic key pair for verification.
        /// </summary>
        public KeyPair?                 KeyPair     { get; }

        /// <summary>
        /// The context of the cryptographic verificationRule verification action.
        /// </summary>
        public VerificationRuleActions  Action      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP CSE cryptographic verificationRule verification entry.
        /// </summary>
        /// <param name="Context">The context of the cryptographic verificationRule entry.</param>
        /// <param name="Priority">The priority of the cryptographic verificationRule entry.</param>
        /// <param name="Action">The context of the cryptographic verificationRule action.</param>
        /// <param name="KeyPair">The optional cryptographic key pair for verification.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public VerificationRule(JSONLDContext            Context,
                                UInt32                   Priority,
                                VerificationRuleActions  Action,
                                KeyPair?                 KeyPair      = null,
                                CustomData?              CustomData   = null)

            : base(CustomData)

        {

            this.Context   = Context;
            this.Priority  = Priority;
            this.Action    = Action;
            this.KeyPair   = KeyPair;

            unchecked
            {

                hashCode = Context. GetHashCode()       * 11 ^
                           Priority.GetHashCode()       *  7 ^
                           Action.  GetHashCode()       *  5 ^
                          (KeyPair?.GetHashCode() ?? 0) *  3 ^
                           base.    GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, CustomVerificationRuleParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cryptographic verificationRule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomVerificationRuleParser">An optional delegate to parse custom cryptographic verification rules.</param>
        public static VerificationRule Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<VerificationRule>?  CustomVerificationRuleParser   = null)
        {

            if (TryParse(JSON,
                         out var verificationRule,
                         out var errorResponse,
                         CustomVerificationRuleParser) &&
                verificationRule is not null)
            {
                return verificationRule;
            }

            throw new ArgumentException("The given JSON representation of a verificationRule is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out VerificationRule, out ErrorResponse, CustomVerificationRuleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a verificationRule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VerificationRule">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                JSON,
                                       out VerificationRule?  VerificationRule,
                                       out String?            ErrorResponse)

            => TryParse(JSON,
                        out VerificationRule,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a verificationRule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VerificationRule">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVerificationRuleParser">An optional delegate to parse custom verification rules.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       out VerificationRule?                           VerificationRule,
                                       out String?                                     ErrorResponse,
                                       CustomJObjectParserDelegate<VerificationRule>?  CustomVerificationRuleParser   = null)
        {

            try
            {

                VerificationRule = default;

                #region Priority      [mandatory]

                if (!JSON.ParseMandatory("priority",
                                         "verificationRule policy priority",
                                         out UInt32 Priority,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Action        [mandatory]

                if (!JSON.ParseMandatory("action",
                                         "verification rule action",
                                         VerificationRuleActionsExtensions.TryParse,
                                         out VerificationRuleActions Action,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region KeyPair       [optional]

                if (JSON.ParseOptionalJSON("keyPair",
                                           "cryptographic key pair",
                                           OCPPv2_1.KeyPair.TryParse,
                                           out KeyPair? KeyPair,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                VerificationRule = new VerificationRule(
                                              DefaultJSONLDContext,
                                              Priority,
                                              Action,
                                              KeyPair,
                                              CustomData
                                          );

                if (CustomVerificationRuleParser is not null)
                    VerificationRule = CustomVerificationRuleParser(JSON,
                                                                                  VerificationRule);

                return true;

            }
            catch (Exception e)
            {
                VerificationRule      = default;
                ErrorResponse  = "The given JSON representation of a verificationRule is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVerificationRuleSerializer = null, CustomKeyPairSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVerificationRuleSerializer">A delegate to serialize cryptographic verificationRule objects.</param>
        /// <param name="CustomKeyPairSerializer">A delegate to serialize cryptographic key pairs.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<VerificationRule>?  CustomVerificationRuleSerializer   = null,
                              CustomJObjectSerializerDelegate<KeyPair>?           CustomKeyPairSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?        CustomCustomDataSerializer         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("priority",     Priority),
                                 new JProperty("action",       Action.    AsText()),

                           KeyPair is not null
                               ? new JProperty("keyPair",      KeyPair.   ToJSON(CustomKeyPairSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomVerificationRuleSerializer is not null
                       ? CustomVerificationRuleSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (VerificationRule1, VerificationRule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VerificationRule1">A verification rule.</param>
        /// <param name="VerificationRule2">Another verification rule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VerificationRule? VerificationRule1,
                                           VerificationRule? VerificationRule2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(VerificationRule1, VerificationRule2))
                return true;

            // If one is null, but not both, return false.
            if (VerificationRule1 is null || VerificationRule2 is null)
                return false;

            return VerificationRule1.Equals(VerificationRule2);

        }

        #endregion

        #region Operator != (VerificationRule1, VerificationRule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VerificationRule1">A verification rule.</param>
        /// <param name="VerificationRule2">Another verification rule.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VerificationRule? VerificationRule1,
                                           VerificationRule? VerificationRule2)

            => !(VerificationRule1 == VerificationRule2);

        #endregion

        #endregion

        #region IEquatable<VerificationRule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two verification rules for equality.
        /// </summary>
        /// <param name="Object">A verification rule to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VerificationRule verificationRule &&
                   Equals(verificationRule);

        #endregion

        #region Equals(VerificationRule)

        /// <summary>
        /// Compares two verification rules for equality.
        /// </summary>
        /// <param name="VerificationRule">A verification rule to compare with.</param>
        public Boolean Equals(VerificationRule? VerificationRule)

            => VerificationRule is not null &&

               Priority.Equals(VerificationRule.Priority) &&
               Context. Equals(VerificationRule.Context)  &&
               Action.  Equals(VerificationRule.Action)   &&

             ((KeyPair is null     && VerificationRule.KeyPair is null) ||
              (KeyPair is not null && VerificationRule.KeyPair is not null && KeyPair.Equals(VerificationRule.KeyPair))) &&

               base.    Equals(VerificationRule);

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
