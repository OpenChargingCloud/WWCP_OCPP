/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The CBOR representations of the data structures of WWCP Core, which
    /// have a JSON representation only: signature policies and user roles.
    /// </summary>
    public static class CBORExtensions
    {

        #region Signature policies: ToCBOR(SignaturePolicy), TryParseSignaturePolicy(CBOR, out SignaturePolicy, out ErrorResponse)

        /// <summary>
        /// The given signature policy as CBOR: the keys of its JSON object, and
        /// its values as what they are.
        /// </summary>
        /// <remarks>
        /// Its signing and verification rules say the public keys of their key
        /// pairs alone, as its JSON object does.
        /// </remarks>
        /// <param name="SignaturePolicy">A signature policy.</param>
        public static CBORValue ToCBOR(this SignaturePolicy SignaturePolicy)

            => OCPPCBORExtensions.Map(
                   ("id",          CBORValue.FromText  (SignaturePolicy.Id.ToString())),
                   ("priority",    CBORValue.FromUInt64(SignaturePolicy.Priority)),
                   ("notBefore",   SignaturePolicy.NotBefore.ToCBOR()),
                   ("notAfter",    SignaturePolicy.NotAfter?.ToCBOR()),
                   ("signingRules",       OCPPCBORExtensions.Array(SignaturePolicy.SignaturePolicyEntries.
                                                                       ToArray().
                                                                       OrderBy(rule => rule.Priority).
                                                                       ThenBy (rule => rule.Context.ToString(), StringComparer.Ordinal),
                                                                   rule => rule.ToCBOR())),
                   ("defaultSigningAction",        SignaturePolicy.DefaultSigningAction != SigningRuleActions.ForwardUnsigned
                                                       ? (CBORValue?) CBORValue.FromText(SignaturePolicy.DefaultSigningAction.AsText())
                                                       : null),
                   ("defaultSigningKeyPair",       SignaturePolicy.DefaultSigningKeyPair?.ToPublicCBOR()),
                   ("verificationRules",  OCPPCBORExtensions.Array(SignaturePolicy.VerificationPolicyEntries.
                                                                       ToArray().
                                                                       OrderBy(rule => rule.Priority).
                                                                       ThenBy (rule => rule.Context.ToString(), StringComparer.Ordinal),
                                                                   rule => rule.ToCBOR())),
                   ("defaultVerificationAction",   SignaturePolicy.DefaultVerificationAction != VerificationRuleActions.AcceptUnverified
                                                       ? (CBORValue?) CBORValue.FromText(SignaturePolicy.DefaultVerificationAction.AsText())
                                                       : null),
                   ("defaultVerificationKeyPair",  SignaturePolicy.DefaultVerificationKeyPair?.ToPublicCBOR()),
                   ("signatures",  OCPPCBORExtensions.Array(SignaturePolicy.Signatures, s => s.ToCBOR())),
                   ("customData",  SignaturePolicy.CustomData?.ToCBOR())
               );


        /// <summary>
        /// Try to read the given CBOR value as a signature policy.
        /// </summary>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="SignaturePolicy">The signature policy.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseSignaturePolicy(CBORValue                                  CBOR,
                                                      [NotNullWhen(true)]  out SignaturePolicy?  SignaturePolicy,
                                                      [NotNullWhen(false)] out String?           ErrorResponse)
        {

            try
            {

                SignaturePolicy = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a signature policy is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("id",
                                             "signature policy identification",
                                             out var IdText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!SignaturePolicy_Id.TryParse(IdText, out var Id))
                {
                    ErrorResponse = $"Invalid signature policy identification '{IdText}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryUInt64("priority",
                                               "signature policy priority",
                                               out var PriorityNumber,
                                               out ErrorResponse))
                {
                    return false;
                }

                if (PriorityNumber > Byte.MaxValue)
                {
                    ErrorResponse = $"Invalid signature policy priority '{PriorityNumber}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryValue<DateTimeOffset>("notBefore",
                                                              "start schedule",
                                                              OCPPCBORExtensions.TryParseTimestamp,
                                                              out var NotBefore,
                                                              out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptionalValue("notAfter",
                                        "end schedule",
                                        OCPPCBORExtensions.TryParseTimestamp,
                                        out DateTimeOffset? NotAfter,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<SigningRule>("signingRules",
                                                    "signing rules",
                                                    TryParseSigningRule,
                                                    out var SigningRules,
                                                    out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<VerificationRule>("verificationRules",
                                                         "verification rules",
                                                         TryParseVerificationRule,
                                                         out var VerificationRules,
                                                         out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                if (!TryOptional<SigningRuleActions>     (CBOR, "defaultSigningAction",      "default signing action",      SigningRuleActionsExtensions.     TryParse, out var DefaultSigningAction,      out ErrorResponse) ||
                    !TryOptional<VerificationRuleActions>(CBOR, "defaultVerificationAction", "default verification action", VerificationRuleActionsExtensions.TryParse, out var DefaultVerificationAction, out ErrorResponse))
                {
                    ErrorResponse ??= "The default action of the signature policy is invalid!";
                    return false;
                }

                CBOR.ParseOptional("defaultSigningKeyPair",
                                   "default signing key pair",
                                   TryParseKeyPair,
                                   out ECCKeyPair? DefaultSigningKeyPair,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("defaultVerificationKeyPair",
                                   "default verification key pair",
                                   TryParseKeyPair,
                                   out ECCKeyPair? DefaultVerificationKeyPair,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<Signature>("signatures",
                                               "cryptographic signatures",
                                               OCPPCBORExtensions.TryParseSignature,
                                               out var Signatures,
                                               out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;


                SignaturePolicy = new SignaturePolicy(
                                      Id,
                                      (Byte) PriorityNumber,
                                      NotBefore,
                                      NotAfter,
                                      SigningRules,
                                      DefaultSigningAction,
                                      DefaultSigningKeyPair,
                                      VerificationRules,
                                      DefaultVerificationAction,
                                      DefaultVerificationKeyPair,
                                      null,
                                      null,
                                      Signatures,
                                      CustomData
                                  );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SignaturePolicy  = null;
                ErrorResponse    = "The given CBOR representation of a signature policy is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region Key pairs: ToPublicCBOR(KeyPair), TryParseKeyPair(CBOR, out KeyPair, out ErrorResponse)

        private delegate Boolean TryParseText<T>(String Text, out T Value);

        /// <summary>
        /// An optional text of a map, read as what it names.
        /// </summary>
        private static Boolean TryOptional<T>(CBORValue                         CBOR,
                                              String                            Key,
                                              String                            Description,
                                              TryParseText<T>                   Parser,
                                              out T?                            Value,
                                              out String?                       ErrorResponse)

            where T : struct

        {

            Value = null;

            if (CBOR.ParseOptionalText(Key, Description, out var text, out ErrorResponse))
            {

                if (ErrorResponse is not null)
                    return false;

                if (!Parser(text!, out var parsed))
                {
                    ErrorResponse = $"Invalid {Description} '{text}'!";
                    return false;
                }

                Value = parsed;

            }

            return ErrorResponse is null;

        }


        /// <summary>
        /// The given key pair as CBOR without its private key - what a key pair
        /// says of itself to anybody else: the keys of its public JSON object,
        /// and its public key as the bytes it is.
        /// </summary>
        /// <param name="KeyPair">A key pair.</param>
        public static CBORValue ToPublicCBOR(this KeyPair KeyPair)

            => OCPPCBORExtensions.Map(
                   ("public",         CBORValue.FromBytes(KeyPair.PublicKeyBytes)),
                   ("algorithm",      KeyPair.Algorithm     != CryptoAlgorithm.    Secp256r1 ? (CBORValue?) CBORValue.FromText(KeyPair.Algorithm.    ToString()) : null),
                   ("serialization",  KeyPair.Serialization != CryptoSerialization.RAW       ? (CBORValue?) CBORValue.FromText(KeyPair.Serialization.ToString()) : null),
                   ("encoding",       KeyPair.Encoding      != CryptoEncoding.     BASE64    ? (CBORValue?) CBORValue.FromText(KeyPair.Encoding.     ToString()) : null),
                   ("customData",     KeyPair.CustomData?.ToCBOR())
               );


        /// <summary>
        /// Try to read the given CBOR value as a key pair: its public key, and
        /// its private key should somebody have sent it.
        /// </summary>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="KeyPair">The key pair.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseKeyPair(CBORValue                             CBOR,
                                              [NotNullWhen(true)]  out ECCKeyPair?  KeyPair,
                                              [NotNullWhen(false)] out String?      ErrorResponse)
        {

            KeyPair = null;

            try
            {

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a key pair is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryBytes("public", "public key", out var Public, out ErrorResponse))
                    return false;

                CBOR.ParseOptionalBytes("private", "private key", out var Private, out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                if (!TryOptional<CryptoAlgorithm>    (CBOR, "algorithm",     "crypto algorithm",     CryptoAlgorithm.    TryParse, out var Algorithm,     out ErrorResponse) ||
                    !TryOptional<CryptoSerialization>(CBOR, "serialization", "crypto serialization", CryptoSerialization.TryParse, out var Serialization, out ErrorResponse) ||
                    !TryOptional<CryptoEncoding>     (CBOR, "encoding",      "crypto encoding",      CryptoEncoding.     TryParse, out var Encoding,      out ErrorResponse))
                {
                    ErrorResponse ??= "The given CBOR representation of a key pair is invalid!";
                    return false;
                }

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                KeyPair = new ECCKeyPair(
                              Public,
                              Private is { Length: > 0 } ? Private : null,
                              Algorithm,
                              Serialization,
                              Encoding,
                              CustomData
                          );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                KeyPair        = null;
                ErrorResponse  = "The given CBOR representation of a key pair is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region Rules: ToCBOR(SigningRule), TryParseSigningRule(...), ToCBOR(VerificationRule), TryParseVerificationRule(...)

        /// <summary>
        /// The given signing rule as CBOR: the keys of its JSON object - the
        /// message it is for, and the public key of its key pair.
        /// </summary>
        /// <param name="SigningRule">A signing rule.</param>
        public static CBORValue ToCBOR(this SigningRule SigningRule)

            => OCPPCBORExtensions.Map(
                   ("context",     CBORValue.FromText  (SigningRule.Context.ToString())),
                   ("priority",    CBORValue.FromUInt64(SigningRule.Priority)),
                   ("action",      CBORValue.FromText  (SigningRule.Action.AsText())),
                   ("keyPair",     SigningRule.KeyPair?.   ToPublicCBOR()),
                   ("customData",  SigningRule.CustomData?.ToCBOR())
               );


        /// <summary>
        /// The given verification rule as CBOR: the keys of its JSON object - the
        /// message it is for, and the public key of its key pair.
        /// </summary>
        /// <param name="VerificationRule">A verification rule.</param>
        public static CBORValue ToCBOR(this VerificationRule VerificationRule)

            => OCPPCBORExtensions.Map(
                   ("context",            CBORValue.FromText  (VerificationRule.Context.ToString())),
                   ("priority",           CBORValue.FromUInt64(VerificationRule.Priority)),
                   ("action",             CBORValue.FromText  (VerificationRule.Action.AsText())),
                   ("keyPair",            VerificationRule.KeyPair?.ToPublicCBOR()),
                   ("withoutTimeWindow",  VerificationRule.WithoutTimeWindow ? (CBORValue?) CBORValue.True : null),
                   ("customData",         VerificationRule.CustomData?.ToCBOR())
               );


        /// <summary>
        /// What both kinds of rules have: the message they are for, their
        /// priority, the text of their action, their key pair and custom data.
        /// </summary>
        private static Boolean TryParseRule(CBORValue        CBOR,
                                            String           Kind,
                                            JSONLDContext    DefaultContext,
                                            out JSONLDContext  Context,
                                            out UInt32       Priority,
                                            out String       ActionText,
                                            out ECCKeyPair?  KeyPair,
                                            out CustomData?  CustomData,
                                            out String?      ErrorResponse)
        {

            Context     = DefaultContext;
            Priority    = 0;
            ActionText  = "";
            KeyPair     = null;
            CustomData  = null;

            if (CBOR.Kind != CBORValueKind.Map)
            {
                ErrorResponse = $"The given CBOR representation of a {Kind} rule is not a map!";
                return false;
            }

            if (!TryOptional<JSONLDContext>(CBOR, "context", $"context of the {Kind} rule", JSONLDContext.TryParse, out var context, out ErrorResponse))
            {
                ErrorResponse ??= $"The context of the {Kind} rule is invalid!";
                return false;
            }

            if (context.HasValue)
                Context = context.Value;

            if (!CBOR.ParseMandatoryUInt64("priority", $"{Kind} rule priority", out var priority, out ErrorResponse))
                return false;

            if (priority > UInt32.MaxValue)
            {
                ErrorResponse = $"Invalid {Kind} rule priority '{priority}'!";
                return false;
            }

            Priority = (UInt32) priority;

            if (!CBOR.ParseMandatoryText("action", $"{Kind} rule action", out var actionText, out ErrorResponse))
                return false;

            ActionText = actionText ?? "";

            CBOR.ParseOptional("keyPair",
                               "cryptographic key pair",
                               TryParseKeyPair,
                               out KeyPair,
                               out ErrorResponse);

            if (ErrorResponse is not null)
                return false;

            CBOR.ParseOptional("customData",
                               "custom data",
                               OCPPCBORExtensions.TryParseCustomData,
                               out CustomData,
                               out ErrorResponse);

            return ErrorResponse is null;

        }


        /// <summary>
        /// Try to read the given CBOR value as a signing rule.
        /// </summary>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="SigningRule">The signing rule.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseSigningRule(CBORValue                              CBOR,
                                                  [NotNullWhen(true)]  out SigningRule?  SigningRule,
                                                  [NotNullWhen(false)] out String?       ErrorResponse)
        {

            SigningRule = null;

            try
            {

                if (!TryParseRule(CBOR, "signing", WWCP.SigningRule.DefaultJSONLDContext,
                                  out var context, out var priority, out var actionText, out var keyPair, out var customData, out ErrorResponse))
                {
                    ErrorResponse ??= "The given CBOR representation of a signing rule is invalid!";
                    return false;
                }

                if (!SigningRuleActionsExtensions.TryParse(actionText, out var action))
                {
                    ErrorResponse = $"Invalid signing rule action '{actionText}'!";
                    return false;
                }

                SigningRule = new SigningRule(
                                  context,
                                  priority,
                                  action,
                                  keyPair,
                                  CustomData: customData
                              );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SigningRule    = null;
                ErrorResponse  = "The given CBOR representation of a signing rule is invalid: " + e.Message;
                return false;
            }

        }


        /// <summary>
        /// Try to read the given CBOR value as a verification rule.
        /// </summary>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="VerificationRule">The verification rule.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseVerificationRule(CBORValue                                   CBOR,
                                                       [NotNullWhen(true)]  out VerificationRule?  VerificationRule,
                                                       [NotNullWhen(false)] out String?            ErrorResponse)
        {

            VerificationRule = null;

            try
            {

                if (!TryParseRule(CBOR, "verification", WWCP.VerificationRule.DefaultJSONLDContext,
                                  out var context, out var priority, out var actionText, out var keyPair, out var customData, out ErrorResponse))
                {
                    ErrorResponse ??= "The given CBOR representation of a verification rule is invalid!";
                    return false;
                }

                if (!VerificationRuleActionsExtensions.TryParse(actionText, out var action))
                {
                    ErrorResponse = $"Invalid verification rule action '{actionText}'!";
                    return false;
                }

                CBOR.ParseOptionalBoolean("withoutTimeWindow", "without time window", out var withoutTimeWindow, out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                VerificationRule = new VerificationRule(
                                       context,
                                       priority,
                                       action,
                                       keyPair,
                                       withoutTimeWindow ?? false,
                                       customData
                                   );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                VerificationRule  = null;
                ErrorResponse     = "The given CBOR representation of a verification rule is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region User roles: ToCBOR(UserRole), TryParseUserRole(CBOR, out UserRole, out ErrorResponse)

        /// <summary>
        /// The given user role as CBOR: the keys of its JSON object, and its
        /// values as what they are.
        /// </summary>
        /// <remarks>
        /// Its key pairs say their public keys alone, as its JSON object does.
        /// </remarks>
        /// <param name="UserRole">A user role.</param>
        public static CBORValue ToCBOR(this UserRole UserRole)

            => OCPPCBORExtensions.Map(
                   ("id",           CBORValue.FromText(UserRole.Id.ToString())),
                   ("description",  UserRole.Description.IsNotNullOrEmpty()
                                        ? (CBORValue?) UserRole.Description.ToCBOR()
                                        : null),
                   ("keyPairs",     OCPPCBORExtensions.Array(UserRole.KeyPairs, keyPair => keyPair.ToPublicCBOR())),
                   ("customData",   UserRole.CustomData?.ToCBOR())
               );


        /// <summary>
        /// Try to read the given CBOR value as a user role.
        /// </summary>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="UserRole">The user role.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseUserRole(CBORValue                           CBOR,
                                               [NotNullWhen(true)]  out UserRole?  UserRole,
                                               [NotNullWhen(false)] out String?    ErrorResponse)
        {

            try
            {

                UserRole = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a user role is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("id",
                                             "user role identification",
                                             out var IdText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!UserRole_Id.TryParse(IdText, out var Id))
                {
                    ErrorResponse = $"Invalid user role identification '{IdText}'!";
                    return false;
                }

                CBOR.ParseOptional("description",
                                   "user role description",
                                   I18NString.TryParse,
                                   out I18NString? Description,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<ECCKeyPair>("keyPairs",
                                                   "cryptographic key pairs",
                                                   TryParseKeyPair,
                                                   out var KeyPairs,
                                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;


                UserRole = new UserRole(
                               Id,
                               Description,
                               KeyPairs,
                               CustomData
                           );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                UserRole       = null;
                ErrorResponse  = "The given CBOR representation of a user role is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

    }

}
