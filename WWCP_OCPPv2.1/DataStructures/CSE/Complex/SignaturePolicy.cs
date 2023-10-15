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

using System.Security.Cryptography;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An OCPP CSE cryptographic signature policy.
    /// </summary>
    public class SignaturePolicy : ACustomData,
                                   IEquatable<SignaturePolicy>
    {

        #region Data

        private        readonly HashSet<SigningRule>       signingRules           = new();
        private        readonly HashSet<VerificationRule>  verificationRules      = new();
        private        readonly HashSet<KeyPair>           keyPairs               = new();

        private static readonly JsonConverter[]            defaultJSONConverters  = new[] {
                                                                                        new Newtonsoft.Json.Converters.IsoDateTimeConverter {
                                                                                            DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffZ"
                                                                                        }
                                                                                    };

        #endregion

        #region Properties

        /// <summary>
        /// The enumeration of signature policy entries.
        /// </summary>
        [Mandatory]
        public IEnumerable<SigningRule>       SignaturePolicyEntries
            => signingRules;

        /// <summary>
        /// The enumeration of signature verification policy entries.
        /// </summary>
        [Mandatory]
        public IEnumerable<VerificationRule>  VerificationPolicyEntries
            => verificationRules;

        /// <summary>
        /// The enumeration of cryptographic key pairs.
        /// </summary>
        [Mandatory]
        public IEnumerable<KeyPair>           KeyPairs
            => keyPairs;


        /// <summary>
        /// The default signature action.
        /// </summary>
        [Mandatory]
        public SigningRuleAction              DefaultSigningAction          { get; }

        /// <summary>
        /// The optional default cryptographic signing key pair.
        /// </summary>
        [Optional]
        public KeyPair?                       DefaultSigningKeyPair         { get; }


        /// <summary>
        /// The default verification action.
        /// </summary>
        [Mandatory]
        public VerificationRuleAction         DefaultVerificationAction     { get; }

        /// <summary>
        /// The optional default cryptographic verification key pair.
        /// </summary>
        [Optional]
        public KeyPair?                       DefaultVerificationKeyPair    { get; }


        /// <summary>
        /// The optional timestamp before which the signature policy should not be used.
        /// </summary>
        [Mandatory]
        public DateTime                       NotBefore                     { get; }

        /// <summary>
        /// The optional timestamp after which the signature policy should not be used.
        /// </summary>
        [Optional]
        public DateTime?                      NotAfter                      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP CSE cryptographic signature policy.
        /// </summary>
        /// <param name="SigningRules">An optional enumeration of cryptographic signing rules.</param>
        /// <param name="DefaultSigningAction">An optional default signing action for this policy.</param>
        /// <param name="DefaultSigningKeyPair">An optional default cryptographic signing key pair.</param>
        /// 
        /// <param name="VerificationRules">An optional enumeration of cryptographic verification rules.</param>
        /// <param name="DefaultVerificationAction">An optional default verification action for this policy.</param>
        /// <param name="DefaultSigningKeyPair">An optional default cryptographic signing key pair.</param>
        /// 
        /// <param name="NotBefore">An optional timestamp before which the signature policy should not be used.</param>
        /// <param name="NotAfter">An optional timestamp after which the signature policy should not be used.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SignaturePolicy(IEnumerable<SigningRule>?       SigningRules                 = null,
                               SigningRuleAction?              DefaultSigningAction         = null,
                               KeyPair?                        DefaultSigningKeyPair        = null,

                               IEnumerable<VerificationRule>?  VerificationRules            = null,
                               VerificationRuleAction?         DefaultVerificationAction    = null,
                               KeyPair?                        DefaultVerificationKeyPair   = null,

                               DateTime?                       NotBefore                    = null,
                               DateTime?                       NotAfter                     = null,

                               CustomData?                     CustomData                   = null)

            : base(CustomData)

        {

            if (SigningRules is not null)
                foreach (var signingRule      in SigningRules)
                    signingRules.   Add(signingRule);

            this.DefaultSigningAction        = DefaultSigningAction      ?? SigningRuleAction.     ForwardUnsigned;
            this.DefaultSigningKeyPair       = DefaultSigningKeyPair;

            if (this.DefaultSigningAction == SigningRuleAction.Sign &&
                this.DefaultSigningKeyPair is null)
            {
                throw new ArgumentException("If the default action is 'sign', a default signing key pair must be provided!");
            }


            if (VerificationRules is not null)
                foreach (var verificationRule in VerificationRules)
                    verificationRules.Add(verificationRule);

            this.DefaultVerificationAction   = DefaultVerificationAction ?? VerificationRuleAction.AcceptUnverified;
            this.DefaultVerificationKeyPair  = DefaultVerificationKeyPair;

            if ((this.DefaultVerificationAction == VerificationRuleAction.VerifyAny ||
                 this.DefaultVerificationAction == VerificationRuleAction.VerifyAll) &&
                 this.DefaultVerificationKeyPair is null)
            {
                throw new ArgumentException("If the default action is 'VerifyAny' or 'VerifyAll', a default verification key pair must be provided!");
            }


            this.NotBefore                   = NotBefore ?? Timestamp.Now;
            this.NotAfter                    = NotAfter;

            unchecked
            {

                hashCode = //KeyId.          GetHashCode()       * 11 ^
                //           Value.          GetHashCode()       *  7 ^
                //          (SigningMethod?. GetHashCode() ?? 0) *  5 ^
                //          (EncodingMethod?.GetHashCode() ?? 0) *  3 ^
                           base.           GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion


        #region Signing...

        #region AddSigningRule (...)

        public SignaturePolicy AddSigningRule(JSONLDContext                        Context,
                                              KeyPair                              KeyPair,
                                              Func<ISignableMessage, String>?      UserIdGenerator        = null,
                                              Func<ISignableMessage, I18NString>?  DescriptionGenerator   = null,
                                              Func<ISignableMessage, DateTime>?    TimestampGenerator     = null)
        {

            lock (signingRules)
            {

                signingRules.Add(new SigningRule(
                                     signingRules.Any() ? signingRules.Max(entry => entry.Priority) + 1 : 1,
                                     Context,
                                     SigningRuleAction.Sign,
                                     KeyPair,
                                     UserIdGenerator,
                                     DescriptionGenerator,
                                     TimestampGenerator
                                 ));

                return this;

            }

        }

        #endregion

        #region AddSigningRule (Priority, ...)

        public SignaturePolicy AddSigningRule(UInt32                               Priority,
                                              JSONLDContext                        Context,
                                              KeyPair                              KeyPair,
                                              Func<ISignableMessage, String>?      UserIdGenerator        = null,
                                              Func<ISignableMessage, I18NString>?  DescriptionGenerator   = null,
                                              Func<ISignableMessage, DateTime>?    TimestampGenerator     = null)
        {

            lock (signingRules)
            {

                signingRules.Add(new SigningRule(
                                     Priority,
                                     Context,
                                     SigningRuleAction.Sign,
                                     KeyPair,
                                     UserIdGenerator,
                                     DescriptionGenerator,
                                     TimestampGenerator
                                 ));

                return this;

            }

        }

        #endregion

        #region AddSigningRules(...)

        public SignaturePolicy AddSigningRules(IEnumerable<SigningRule> SigningRules)
        {

            lock (signingRules)
            {

                foreach (var signingRule in SigningRules)
                    signingRules.Add(signingRule);

                return this;

            }

        }

        #endregion


        #region GetSigningRules(...)

        public IEnumerable<SigningRule> GetSigningRules(JSONLDContext Context)
        {

            var rules = signingRules.Where(signingRule => signingRule.Context == Context);

            if (rules.Any())
                return rules;

            return new[] {
                       new SigningRule(
                           0,
                           JSONLDContext.Parse("default"),
                           DefaultSigningAction
                       )
                   };

        }

        #endregion

        #region HasSigningRules(Context, out SigningRules)

        public Boolean HasSigningRules(JSONLDContext                 Context,
                                       out IEnumerable<SigningRule>  SigningRules)
        {

            SigningRules = signingRules.Where(signingRule => signingRule.Context == Context);

            return SigningRules.Any();

        }

        #endregion


        #region SignMessage        (SignableMessage, JSONMessage, out ErrorResponse, params SignInfos)

        /// <summary>
        /// Sign the given message.
        /// </summary>
        /// <param name="SignableMessage">A signable message.</param>
        /// <param name="JSONMessage">The JSON representation of the signable message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="SignInfos">An enumeration of cryptographic signature information or key pairs to sign the given message.</param>
        public Boolean SignMessage(ISignableMessage   SignableMessage,
                                   JObject            JSONMessage,
                                   out String?        ErrorResponse,
                                   params SignInfo[]  SignInfos)
        {

            #region Initial checks

            if (JSONMessage is null)
            {
                ErrorResponse = "The given JSON message must not be null!";
                return false;
            }

            //if (SignInfos is null || !SignInfos.Any())
            //{
            //    ErrorResponse = "The given key pairs must not be null or empty!";
            //    return false;
            //}

            #endregion

            try
            {

                if (JSONMessage["@context"] is null)
                    JSONMessage.AddFirst(new JProperty("@context", SignableMessage.Context.ToString()));

                IEnumerable<SigningRule>? signaturePolicyEntries = null;

                if ((SignInfos                 is not null && SignInfos.                Any()) ||
                    (SignableMessage.SignKeys  is not null && SignableMessage.SignKeys. Any()) ||
                    (SignableMessage.SignInfos is not null && SignableMessage.SignInfos.Any()) ||
                     HasSigningRules(SignableMessage.Context, out signaturePolicyEntries))
                {

                    var signInfos = new List<SignInfo>();

                    if (SignInfos                 is not null && SignInfos.Any())
                        signInfos.AddRange(SignInfos);

                    if (SignableMessage.SignKeys  is not null && SignableMessage.SignKeys. Any())
                        signInfos.AddRange(SignableMessage.SignKeys.Select(keyPair => keyPair.ToSignInfo1()));

                    if (SignableMessage.SignInfos is not null && SignableMessage.SignInfos.Any())
                        signInfos.AddRange(SignableMessage.SignInfos);

                    if (signaturePolicyEntries    is not null && signaturePolicyEntries.   Any())
                    {
                        foreach (var signaturePolicyEntry in signaturePolicyEntries)
                        {

                            var signInfo = signaturePolicyEntry.ToSignInfo();

                            if (signInfo is not null)
                                signInfos.Add(signInfo);

                        }
                    }


                    foreach (var signInfo in signInfos)
                    {

                        #region Initial checks

                        if (signInfo is null)
                        {
                            ErrorResponse = "The given key pair must not be null!";
                            return false;
                        }


                        if (signInfo.Private is null || signInfo.Private.IsNullOrEmpty())
                        {
                            ErrorResponse = "The given key pair must contain a serialized private key!";
                            return false;
                        }

                        if (signInfo.Public  is null || signInfo.Public. IsNullOrEmpty())
                        {
                            ErrorResponse = "The given key pair must contain a serialized public key!";
                            return false;
                        }


                        if (signInfo.PrivateKey is null)
                        {
                            ErrorResponse = "The given key pair must contain a private key!";
                            return false;
                        }

                        if (signInfo.PublicKey is null)
                        {
                            ErrorResponse = "The given key pair must contain a public key!";
                            return false;
                        }

                        #endregion

                        var plainText   = JSONMessage.ToString(Formatting.None, defaultJSONConverters);

                        var cryptoHash  = signInfo.Algorithm switch {
                                              "secp521r1"  => SHA512.HashData(plainText.ToUTF8Bytes()),
                                              "secp384r1"  => SHA512.HashData(plainText.ToUTF8Bytes()),
                                              _            => SHA256.HashData(plainText.ToUTF8Bytes()),
                                          };

                        var signer       = SignerUtilities.GetSigner("NONEwithECDSA");
                        signer.Init(true, signInfo.PrivateKey);
                        signer.BlockUpdate(cryptoHash);
                        var signature    = signer.GenerateSignature();

                        SignableMessage.AddSignature(new Signature(
                                                         KeyId:            SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(signInfo.PublicKey).PublicKeyData.GetBytes().ToBase64(),
                                                         Value:            signature.ToBase64(),
                                                         Algorithm:        signInfo.Algorithm,
                                                         SigningMethod:    null,
                                                         EncodingMethod:   signInfo.Encoding,
                                                         Name:             signInfo.SignerName?.       Invoke(SignableMessage),
                                                         Description:      signInfo.Description?.Invoke(SignableMessage),
                                                         Timestamp:        signInfo.Timestamp?.  Invoke(SignableMessage)
                                                     ));

                    }

                }

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = e.Message;
                return false;
            }

        }

        #endregion

        #region SignRequestMessage (RequestMessage,  JSONMessage, out ErrorResponse, params SignInfos)

        /// <summary>
        /// Sign the given request message.
        /// </summary>
        /// <param name="RequestMessage">The request message.</param>
        /// <param name="JSONMessage">The JSON representation of the request message.</param>
        /// <param name="ErrorResponse">An optional error response in case of signing errors.</param>
        /// <param name="SignInfos">One or multiple cryptographic signature information or key pairs to sign the request message.</param>
        public Boolean SignRequestMessage(IRequest           RequestMessage,
                                          JObject            JSONMessage,
                                          out String?        ErrorResponse,
                                          params SignInfo[]  SignInfos)
        {

            return SignMessage(RequestMessage,
                               JSONMessage,
                               out ErrorResponse,
                               SignInfos);

        }

        #endregion

        #region SignResponseMessage(ResponseMessage, JSONMessage, out ErrorResponse, params SignInfos)

        /// <summary>
        /// Sign the given response message.
        /// </summary>
        /// <param name="ResponseMessage">A response message.</param>
        /// <param name="JSONMessage">The JSON representation of the response message.</param>
        /// <param name="ErrorResponse">An optional error response in case of signing errors.</param>
        /// <param name="SignInfos">One or multiple cryptographic signature information or key pairs to sign the response message.</param>
        public Boolean SignResponseMessage(IResponse          ResponseMessage,
                                           JObject            JSONMessage,
                                           out String?        ErrorResponse,
                                           params SignInfo[]  SignInfos)
        {

            return SignMessage(ResponseMessage,
                               JSONMessage,
                               out ErrorResponse,
                               SignInfos);

        }

        #endregion

        #endregion

        #region Verification...

        #region AddVerificationRule (...)

        public SignaturePolicy AddVerificationRule(JSONLDContext           Context,
                                                   VerificationRuleAction  Action   = VerificationRuleAction.VerifyAll)
        {

            lock (verificationRules)
            {

                verificationRules.Add(new VerificationRule(
                                          verificationRules.Any() ? verificationRules.Max(entry => entry.Priority) + 1 : 1,
                                          Context,
                                          Action
                                      ));

                return this;

            }

        }

        #endregion

        #region AddVerificationRule (...)

        public SignaturePolicy AddVerificationRule(UInt32                  Priority,
                                                   JSONLDContext           Context,
                                                   VerificationRuleAction  Action   = VerificationRuleAction.VerifyAll)
        {

            lock (verificationRules)
            {

                verificationRules.Add(new VerificationRule(
                                          Priority,
                                          Context,
                                          Action
                                      ));

                return this;

            }

        }

        #endregion

        #region AddVerificationRules(...)

        public SignaturePolicy AddVerificationRules(IEnumerable<VerificationRule> VerificationRules)
        {

            lock (verificationRules)
            {

                foreach (var verificationRule in VerificationRules)
                    verificationRules.Add(verificationRule);

                return this;

            }

        }

        #endregion


        #region GetVerificationRules      (...)

        public IEnumerable<VerificationRule> GetVerificationRules(JSONLDContext Context)
        {

            var rules = verificationRules.Where(verificationRule => verificationRule.Context == Context);

            if (rules.Any())
                return rules;

            return new[] {
                       new VerificationRule(
                           0,
                           JSONLDContext.Parse("default"),
                           DefaultVerificationAction
                       )
                   };

        }

        #endregion

        #region GetHighestVerificationRule(...)

        public VerificationRule GetHighestVerificationRule(JSONLDContext Context)
        {

            var verificationRule = verificationRules.
                                       Where(verificationRule => verificationRule.Context == Context).
                                       MaxBy(verificationRule => verificationRule.Priority);

            if (verificationRule is not null)
                return verificationRule;

            return new VerificationRule(
                       0,
                       JSONLDContext.Parse("default"),
                       DefaultVerificationAction
                   );

        }

        #endregion


        #region VerifyMessage      (SignableMessage, JSONMessage, out ErrorResponse)

        /// <summary>
        /// Verify the given message.
        /// </summary>
        /// <param name="SignableMessage">A signable/verifiable message.</param>
        /// <param name="JSONMessage">The JSON representation of the signable/verifiable message.</param>
        /// <param name="ErrorResponse">An optional error response in case of validation errors.</param>
        public Boolean VerifyMessage(ISignableMessage  SignableMessage,
                                     JObject           JSONMessage,
                                     out String?       ErrorResponse)
        {

            ErrorResponse = null;

            var verificationPolicyEntry = GetHighestVerificationRule(SignableMessage.Context);

            if (!SignableMessage.Signatures.Any())
            {

                if (DefaultVerificationAction == VerificationRuleAction.AcceptUnverified)
                    return true;

                ErrorResponse = "The given message does not contain any signatures!";
                return false;

            }

            try
            {

                if (JSONMessage["@context"] is null)
                    JSONMessage.AddFirst(new JProperty("@context", SignableMessage.Context.ToString()));

                var jsonMessageCopy  = JObject.Parse(JSONMessage.ToString(Formatting.None, defaultJSONConverters));
                jsonMessageCopy.Remove("signatures");

                var plainText = jsonMessageCopy.ToString(Formatting.None, defaultJSONConverters);

                switch (verificationPolicyEntry.Action)
                {

                    case VerificationRuleAction.AcceptUnverified:
                        foreach (var signature in SignableMessage.Signatures)
                            signature.Status = VerificationStatus.Unverified;
                        return true;

                    case VerificationRuleAction.Drop:
                        foreach (var signature in SignableMessage.Signatures)
                            signature.Status = VerificationStatus.DropMessage;
                        return true;

                    case VerificationRuleAction.Reject:
                        foreach (var signature in SignableMessage.Signatures)
                            signature.Status = VerificationStatus.RejectMessage;
                        return true;

                }

                foreach (var signature in SignableMessage.Signatures)
                {

                    var ecp           = signature.Algorithm switch {
                                            "secp521r1"  => SecNamedCurves.GetByName("secp521r1"),
                                            "secp384r1"  => SecNamedCurves.GetByName("secp384r1"),
                                            _            => SecNamedCurves.GetByName("secp256r1"),
                                        };
                    var ecParams      = new ECDomainParameters(ecp.Curve, ecp.G, ecp.N, ecp.H, ecp.GetSeed());
                    var pubKeyParams  = new ECPublicKeyParameters("ECDSA", ecParams.Curve.DecodePoint(signature.KeyId.FromBase64()), ecParams);

                    var cryptoHash    = signature.Algorithm switch {
                                            "secp521r1"  => SHA512.HashData(plainText.ToUTF8Bytes()),
                                            "secp384r1"  => SHA512.HashData(plainText.ToUTF8Bytes()),
                                            _            => SHA256.HashData(plainText.ToUTF8Bytes()),
                                        };

                    var verifier      = SignerUtilities.GetSigner("NONEwithECDSA");
                    verifier.Init(false, pubKeyParams);
                    verifier.BlockUpdate(cryptoHash);
                    signature.Status  = verifier.VerifySignature(signature.Value.FromBase64())
                                            ? VerificationStatus.ValidSignature
                                            : VerificationStatus.InvalidSignature;

                    if (verificationPolicyEntry?.Action == VerificationRuleAction.VerifyAny &&
                        signature.Status == VerificationStatus.ValidSignature)
                    {
                        return true;
                    }

                }

                // Default, and when there is no signature policy (entry) defined!
                return SignableMessage.Signatures.All(signature => signature.Status == VerificationStatus.ValidSignature);

            }
            catch (Exception e)
            {
                ErrorResponse = e.Message;
                return false;
            }

        }

        #endregion

        #region VerifyMessage      (RequestMessage,  JSONMessage, out ErrorResponse)

        /// <summary>
        /// Verify the given request message.
        /// </summary>
        /// <param name="RequestMessage">The request message.</param>
        /// <param name="JSONMessage">The JSON representation of the request message.</param>
        /// <param name="ErrorResponse">An optional error response in case of validation errors.</param>
        public Boolean VerifyRequestMessage(IRequest     RequestMessage,
                                            JObject      JSONMessage,
                                            out String?  ErrorResponse)
        {

            return VerifyMessage(RequestMessage,
                                 JSONMessage,
                                 out ErrorResponse);

        }

        #endregion

        #region VerifyMessage      (RequestMessage,  JSONMessage, out ErrorResponse)

        /// <summary>
        /// Verify the given request message.
        /// </summary>
        /// <param name="ResponseMessage">A response message.</param>
        /// <param name="JSONMessage">The JSON representation of the request message.</param>
        /// <param name="ErrorResponse">An optional error response in case of validation errors.</param>
        public Boolean VerifyResponseMessage(IResponse    ResponseMessage,
                                             JObject      JSONMessage,
                                             out String?  ErrorResponse)
        {

            return VerifyMessage(ResponseMessage,
                                 JSONMessage,
                                 out ErrorResponse);

        }

        #endregion

        #endregion


        #region Operator overloading

        #region Operator == (SignaturePolicy1, SignaturePolicy2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignaturePolicy1">A signature policy.</param>
        /// <param name="SignaturePolicy2">Another signature policy.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SignaturePolicy? SignaturePolicy1,
                                           SignaturePolicy? SignaturePolicy2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SignaturePolicy1, SignaturePolicy2))
                return true;

            // If one is null, but not both, return false.
            if (SignaturePolicy1 is null || SignaturePolicy2 is null)
                return false;

            return SignaturePolicy1.Equals(SignaturePolicy2);

        }

        #endregion

        #region Operator != (SignaturePolicy1, SignaturePolicy2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignaturePolicy1">A signature policy.</param>
        /// <param name="SignaturePolicy2">Another signature policy.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SignaturePolicy? SignaturePolicy1,
                                           SignaturePolicy? SignaturePolicy2)

            => !(SignaturePolicy1 == SignaturePolicy2);

        #endregion

        #endregion

        #region IEquatable<SignaturePolicy> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signature policies for equality.
        /// </summary>
        /// <param name="Object">A signature policy to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignaturePolicy signaturePolicy &&
                   Equals(signaturePolicy);

        #endregion

        #region Equals(SignaturePolicy)

        /// <summary>
        /// Compares two signature policies for equality.
        /// </summary>
        /// <param name="SignaturePolicy">A signature policy to compare with.</param>
        public Boolean Equals(SignaturePolicy? SignaturePolicy)

            => SignaturePolicy is not null &&

               //String.Equals(KeyId,           SignaturePolicy.KeyId,           StringComparison.Ordinal) &&
               //String.Equals(Value,           SignaturePolicy.EncodingMethod,  StringComparison.Ordinal) &&
               //String.Equals(SigningMethod,   SignaturePolicy.SigningMethod,   StringComparison.Ordinal) &&
               //String.Equals(EncodingMethod,  SignaturePolicy.EncodingMethod,  StringComparison.Ordinal) &&

               base.  Equals(SignaturePolicy);

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

            => $"{SignaturePolicyEntries.Count()} / {KeyPairs.Count()} => Default: '{DefaultSigningAction}'{(DefaultSigningKeyPair is not null ? " / defaultKey" : "")}";

        #endregion

    }

}
