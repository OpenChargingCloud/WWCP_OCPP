///*
// * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Affero GPL license, Version 3.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.gnu.org/licenses/agpl.html
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using Newtonsoft.Json.Linq;

//using org.GraphDefined.Vanaheimr.Illias;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv1_6
//{

//    /// <summary>
//    /// An OCPP CSE cryptographic signature signing rule.
//    /// </summary>
//    public class SigningRule : IEquatable<SigningRule>
//    {

//        #region Data

//        /// <summary>
//        /// The JSON-LD context of this data structure.
//        /// </summary>
//        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/signingRule");

//        #endregion

//        #region Properties

//        /// <summary>
//        /// The context of the cryptographic signature signing rule.
//        /// </summary>
//        public JSONLDContext                        Context                 { get; }

//        /// <summary>
//        /// The priority of the cryptographic signature signing rule.
//        /// </summary>
//        public UInt32                               Priority                { get; }

//        /// <summary>
//        /// The context of the cryptographic signature action.
//        /// </summary>
//        public SigningRuleActions                    Action                  { get; }

//        /// <summary>
//        /// An optional cryptographic key pair for signing.
//        /// </summary>
//        public KeyPair?                             KeyPair                 { get; }

//        /// <summary>
//        /// An optional user identification generator for signing.
//        /// </summary>
//        public Func<ISignableMessage, String>?      UserIdGenerator         { get; }

//        /// <summary>
//        /// An optional multi-language description generator for signing.
//        /// </summary>
//        public Func<ISignableMessage, I18NString>?  DescriptionGenerator    { get; }

//        /// <summary>
//        /// An optional timestamp generator for signing.
//        /// </summary>
//        public Func<ISignableMessage, DateTime>?    TimestampGenerator      { get; }

//        #endregion

//        #region Constructor(s)

//        /// <summary>
//        /// Create a new OCPP CSE cryptographic signature signing rule.
//        /// </summary>
//        /// <param name="Context">The context of the cryptographic signature signing rule.</param>
//        /// <param name="Priority">The priority of the cryptographic signature signing rule.</param>
//        /// <param name="Action">The context of the cryptographic signature action.</param>
//        /// <param name="KeyPair">The optional cryptographic key pair for signing.</param>
//        /// <param name="UserIdGenerator">An optional user identification generator for signing.</param>
//        /// <param name="DescriptionGenerator">An optional multi-language description generator for signing.</param>
//        /// <param name="TimestampGenerator">An optional timestamp generator for signing.</param>
//        public SigningRule(JSONLDContext                        Context,
//                           UInt32                               Priority,
//                           SigningRuleActions                   Action,
//                           KeyPair?                             KeyPair                = null,
//                           Func<ISignableMessage, String>?      UserIdGenerator        = null,
//                           Func<ISignableMessage, I18NString>?  DescriptionGenerator   = null,
//                           Func<ISignableMessage, DateTime>?    TimestampGenerator     = null)
//        {

//            this.Context               = Context;
//            this.Priority              = Priority;
//            this.Action                = Action;
//            this.KeyPair               = KeyPair;
//            this.UserIdGenerator       = UserIdGenerator;
//            this.DescriptionGenerator  = DescriptionGenerator;
//            this.TimestampGenerator    = TimestampGenerator;

//            unchecked
//            {

//                hashCode = Context.              GetHashCode()       * 19 ^
//                           Priority.             GetHashCode()       * 17 ^
//                           Action.               GetHashCode()       * 13 ^
//                          (KeyPair?.             GetHashCode() ?? 0) * 11 ^
//                          (UserIdGenerator?.     GetHashCode() ?? 0) *  7 ^
//                          (DescriptionGenerator?.GetHashCode() ?? 0) *  5 ^
//                          (TimestampGenerator?.  GetHashCode() ?? 0) *  3 ^
//                           base.                 GetHashCode();

//            }

//        }

//        #endregion


//        #region Documentation

//        // tba.

//        #endregion

//        #region (static) Parse   (JSON, CustomSigningRuleParser = null)

//        /// <summary>
//        /// Parse the given JSON representation of a cryptographic signature.
//        /// </summary>
//        /// <param name="JSON">The JSON to be parsed.</param>
//        /// <param name="CustomSigningRuleParser">An optional delegate to parse custom cryptographic signatures.</param>
//        public static SigningRule Parse(JObject                                    JSON,
//                                        CustomJObjectParserDelegate<SigningRule>?  CustomSigningRuleParser   = null)
//        {

//            if (TryParse(JSON,
//                         out var signature,
//                         out var errorResponse,
//                         CustomSigningRuleParser) &&
//                signature is not null)
//            {
//                return signature;
//            }

//            throw new ArgumentException("The given JSON representation of a signature is invalid: " + errorResponse,
//                                        nameof(JSON));

//        }

//        #endregion

//        #region (static) TryParse(JSON, out SigningRule, out ErrorResponse, CustomSigningRuleParser = null)

//        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

//        /// <summary>
//        /// Try to parse the given JSON representation of a signature.
//        /// </summary>
//        /// <param name="JSON">The JSON to be parsed.</param>
//        /// <param name="SigningRule">The parsed connector type.</param>
//        /// <param name="ErrorResponse">An optional error response.</param>
//        public static Boolean TryParse(JObject           JSON,
//                                       out SigningRule?  SigningRule,
//                                       out String?       ErrorResponse)

//            => TryParse(JSON,
//                        out SigningRule,
//                        out ErrorResponse,
//                        null);


//        /// <summary>
//        /// Try to parse the given JSON representation of a signature.
//        /// </summary>
//        /// <param name="JSON">The JSON to be parsed.</param>
//        /// <param name="SigningRule">The parsed connector type.</param>
//        /// <param name="ErrorResponse">An optional error response.</param>
//        /// <param name="CustomSigningRuleParser">An optional delegate to parse custom signatures.</param>
//        public static Boolean TryParse(JObject                                    JSON,
//                                       out SigningRule?                           SigningRule,
//                                       out String?                                ErrorResponse,
//                                       CustomJObjectParserDelegate<SigningRule>?  CustomSigningRuleParser   = null)
//        {

//            try
//            {

//                SigningRule = default;

//                #region Priority      [mandatory]

//                if (!JSON.ParseMandatory("priority",
//                                         "signature policy priority",
//                                         out UInt32 Priority,
//                                         out ErrorResponse))
//                {
//                    return false;
//                }

//                #endregion

//                #region Action        [mandatory]

//                if (!JSON.ParseMandatory("action",
//                                         "signing rule action",
//                                         SigningRuleActionsExtensions.TryParse,
//                                         out SigningRuleActions Action,
//                                         out ErrorResponse))
//                {
//                    return false;
//                }

//                #endregion

//                #region KeyPair       [optional]

//                if (JSON.ParseOptionalJSON("keyPair",
//                                           "cryptographic key pair",
//                                           OCPP.KeyPair.TryParse,
//                                           out KeyPair? KeyPair,
//                                           out ErrorResponse))
//                {
//                    if (ErrorResponse is not null)
//                        return false;
//                }

//                #endregion

//                #region CustomData    [optional]

//                if (JSON.ParseOptionalJSON("customData",
//                                           "custom data",
//                                           WWCP.CustomData.TryParse,
//                                           out CustomData? CustomData,
//                                           out ErrorResponse))
//                {
//                    if (ErrorResponse is not null)
//                        return false;
//                }

//                #endregion


//                SigningRule = new SigningRule(
//                                  DefaultJSONLDContext,
//                                  Priority,
//                                  Action,
//                                  KeyPair,
//                                  null,  // UserIdGenerator
//                                  null,  // DescriptionGenerator
//                                  null,  // TimestampGenerator
//                                  CustomData
//                              );

//                if (CustomSigningRuleParser is not null)
//                    SigningRule = CustomSigningRuleParser(JSON,
//                                                          SigningRule);

//                return true;

//            }
//            catch (Exception e)
//            {
//                SigningRule    = default;
//                ErrorResponse  = "The given JSON representation of a signature is invalid: " + e.Message;
//                return false;
//            }

//        }

//        #endregion

//        #region ToJSON(CustomSigningRuleSerializer = null, CustomKeyPairSerializer = null, ...)

//        /// <summary>
//        /// Return a JSON representation of this object.
//        /// </summary>
//        /// <param name="CustomSigningRuleSerializer">A delegate to serialize cryptographic signature objects.</param>
//        /// <param name="CustomKeyPairSerializer">A delegate to serialize cryptographic key pairs.</param>
//        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
//        public JObject ToJSON(CustomJObjectSerializerDelegate<SigningRule>?  CustomSigningRuleSerializer   = null,
//                              CustomJObjectSerializerDelegate<WWCP.KeyPair>? CustomKeyPairSerializer       = null,
//                              CustomJObjectSerializerDelegate<CustomData>?   CustomCustomDataSerializer    = null)
//        {

//            var json = JSONObject.Create(

//                                 new JProperty("priority",     Priority),
//                                 new JProperty("action",       Action.    AsText()),

//                           KeyPair is not null
//                               ? new JProperty("keyPair",      KeyPair.   ToJSON(CustomKeyPairSerializer,
//                                                                                 CustomCustomDataSerializer))
//                               : null,

//                           CustomData is not null
//                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
//                               : null

//                       );

//            return CustomSigningRuleSerializer is not null
//                       ? CustomSigningRuleSerializer(this, json)
//                       : json;

//        }

//        #endregion


//        #region Operator overloading

//        #region Operator == (SigningRule1, SigningRule2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="SigningRule1">A signing rule.</param>
//        /// <param name="SigningRule2">Another signing rule.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator == (SigningRule? SigningRule1,
//                                           SigningRule? SigningRule2)
//        {

//            // If both are null, or both are same instance, return true.
//            if (ReferenceEquals(SigningRule1, SigningRule2))
//                return true;

//            // If one is null, but not both, return false.
//            if (SigningRule1 is null || SigningRule2 is null)
//                return false;

//            return SigningRule1.Equals(SigningRule2);

//        }

//        #endregion

//        #region Operator != (SigningRule1, SigningRule2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="SigningRule1">A signing rule.</param>
//        /// <param name="SigningRule2">Another signing rule.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator != (SigningRule? SigningRule1,
//                                           SigningRule? SigningRule2)

//            => !(SigningRule1 == SigningRule2);

//        #endregion

//        #endregion

//        #region IEquatable<SigningRule> Members

//        #region Equals(Object)

//        /// <summary>
//        /// Compares two signing rules for equality.
//        /// </summary>
//        /// <param name="Object">A signing rule to compare with.</param>
//        public override Boolean Equals(Object? Object)

//            => Object is SigningRule signingRule &&
//                   Equals(signingRule);

//        #endregion

//        #region Equals(SigningRule)

//        /// <summary>
//        /// Compares two signing rules for equality.
//        /// </summary>
//        /// <param name="SigningRule">A signing rule to compare with.</param>
//        public Boolean Equals(SigningRule? SigningRule)

//            => SigningRule is not null &&

//               Priority.Equals(SigningRule.Priority) &&
//               Context. Equals(SigningRule.Context)  &&
//               Action.  Equals(SigningRule.Action)   &&

//             ((KeyPair is null     && SigningRule.KeyPair is null) ||
//              (KeyPair is not null && SigningRule.KeyPair is not null && KeyPair.Equals(SigningRule.KeyPair))) &&

//               base.    Equals(SigningRule);

//        #endregion

//        #endregion

//        #region (override) GetHashCode()

//        private readonly Int32 hashCode;

//        /// <summary>
//        /// Return the hash code of this object.
//        /// </summary>
//        public override Int32 GetHashCode()
//            => hashCode;

//        #endregion

//        #region (override) ToString()

//        /// <summary>
//        /// Return a text representation of this object.
//        /// </summary>
//        public override String ToString()

//            => $"{Context} => {Action} ({Priority})";

//        #endregion

//    }

//}
