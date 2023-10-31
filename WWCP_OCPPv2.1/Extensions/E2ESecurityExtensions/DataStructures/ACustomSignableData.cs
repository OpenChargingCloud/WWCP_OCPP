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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using org.GraphDefined.Vanaheimr.Illias;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System.Security.Cryptography;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An abstract custom signable data container.
    /// </summary>
    public abstract class ACustomSignableData : ACustomData,
                                                IEquatable<ACustomSignableData>
    {

        #region Data

        private readonly HashSet<Signature> signatures;

        #endregion

        #region Properties

        /// <summary>
        /// The optional enumeration of keys to be used for signing this message.
        /// </summary>
        public IEnumerable<KeyPair>    SignKeys     { get; }

        /// <summary>
        /// The optional enumeration of information to be used for signing this message.
        /// </summary>
        public IEnumerable<SignInfo>   SignInfos    { get; }

        /// <summary>
        /// The optional enumeration of cryptographic signatures for this message.
        /// </summary>
        [Optional]
        public IEnumerable<Signature>  Signatures
            => signatures;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new custom signable data.
        /// </summary>
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ACustomSignableData(IEnumerable<KeyPair>?    SignKeys     = null,
                                   IEnumerable<SignInfo>?   SignInfos    = null,
                                   IEnumerable<Signature>?  Signatures   = null,
                                   CustomData?              CustomData   = null)

            : base(CustomData)

        {

            this.SignKeys    = SignKeys  ?? Array.Empty<KeyPair>();
            this.SignInfos   = SignInfos ?? Array.Empty<SignInfo>();
            this.signatures  = Signatures is not null && Signatures.Any()
                                   ? new HashSet<Signature>(Signatures)
                                   : new HashSet<Signature>();

            unchecked
            {

                hashCode = this.Signatures.CalcHashCode() * 3 ^
                           base.           GetHashCode();

            }

        }

        #endregion


        #region AddSignature   (Signature)

        /// <summary>
        /// Add a cryptographic signature to this message.
        /// </summary>
        /// <param name="Signature">A cryptographic signature.</param>
        public void AddSignature(Signature Signature)
        {
            lock (signatures)
            {

                if (signatures.Add(Signature))
                    hashCode = Signatures.CalcHashCode() * 3 ^
                               base.      GetHashCode();

            }
        }

        #endregion

        #region RemoveSignature(Signature)

        /// <summary>
        /// Remove a cryptographic signature from this message.
        /// </summary>
        /// <param name="Signature">A cryptographic signature.</param>
        public void RemoveSignature(Signature Signature)
        {
            lock (signatures)
            {

                if (signatures.Remove(Signature))
                    hashCode = Signatures.CalcHashCode() * 3 ^
                               base.GetHashCode();

            }
        }

        #endregion


        #region Sign  (JSONMessage, Context, KeyPair, out ErrorResponse, SignerName = null, Description = null, Timestamp = null)

        /// <summary>
        /// Sign the given data structure.
        /// </summary>
        /// <param name="JSONMessage"></param>
        /// <param name="Context"></param>
        /// <param name="KeyPair"></param>
        /// <param name="ErrorResponse"></param>
        /// <param name="SignerName"></param>
        /// <param name="Description"></param>
        /// <param name="Timestamp"></param>
        public Boolean Sign(JObject        JSONMessage,
                            JSONLDContext  Context,
                            KeyPair        KeyPair,
                            out String?    ErrorResponse,
                            String?        SignerName    = null,
                            I18NString?    Description   = null,
                            DateTime?      Timestamp     = null)
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
                    JSONMessage.AddFirst(new JProperty("@context", Context.ToString()));

                #region Initial checks

                if (KeyPair.Private is null || KeyPair.Private.IsNullOrEmpty())
                {
                    ErrorResponse = "The given key pair must contain a serialized private key!";
                    return false;
                }

                if (KeyPair.Public  is null || KeyPair.Public. IsNullOrEmpty())
                {
                    ErrorResponse = "The given key pair must contain a serialized public key!";
                    return false;
                }


                if (KeyPair.PrivateKey is null)
                {
                    ErrorResponse = "The given key pair must contain a private key!";
                    return false;
                }

                if (KeyPair.PublicKey is null)
                {
                    ErrorResponse = "The given key pair must contain a public key!";
                    return false;
                }

                #endregion

                var plainText   = JSONMessage.ToString(Formatting.None, SignableMessage.DefaultJSONConverters);

                var cryptoHash  = KeyPair.Algorithm switch {
                                      "secp521r1"  => SHA512.HashData(plainText.ToUTF8Bytes()),
                                      "secp384r1"  => SHA512.HashData(plainText.ToUTF8Bytes()),
                                      _            => SHA256.HashData(plainText.ToUTF8Bytes()),
                                  };

                var signer       = SignerUtilities.GetSigner("NONEwithECDSA");
                signer.Init(true, KeyPair.PrivateKey);
                signer.BlockUpdate(cryptoHash);
                var signature    = signer.GenerateSignature();

                AddSignature(new Signature(
                                 KeyId:            SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(KeyPair.PublicKey).PublicKeyData.GetBytes().ToBase64(),
                                 Value:            signature.ToBase64(),
                                 Algorithm:        KeyPair.Algorithm,
                                 SigningMethod:    null,
                                 EncodingMethod:   KeyPair.Encoding,
                                 Name:             SignerName,
                                 Description:      Description,
                                 Timestamp:        Timestamp
                             ));

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

        #region Verify(JSONMessage, out ErrorResponse)

        /// <summary>
        /// Verify the given data structure.
        /// </summary>
        /// <param name="JSONMessage">The JSON representation of the signable/verifiable message.</param>
        /// <param name="ErrorResponse">An optional error response in case of validation errors.</param>
        public Boolean Verify(JObject                   JSONMessage,
                              JSONLDContext             Context,
                              out String?               ErrorResponse,
                              VerificationRuleActions?  VerificationRuleAction = VerificationRuleActions.VerifyAll)
        {

            ErrorResponse = null;

            if (!Signatures.Any())
            {

                if (VerificationRuleAction == VerificationRuleActions.AcceptUnverified)
                    return true;

                ErrorResponse = "The given message does not contain any signatures!";
                return false;

            }

            try
            {

                if (JSONMessage["@context"] is null)
                    JSONMessage.AddFirst(new JProperty("@context", Context.ToString()));

                var jsonMessageCopy  = JObject.Parse(JSONMessage.ToString(Formatting.None, SignableMessage.DefaultJSONConverters));
                jsonMessageCopy.Remove("signatures");

                var plainText = jsonMessageCopy.ToString(Formatting.None, SignableMessage.DefaultJSONConverters);

                foreach (var signature in Signatures)
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

                    if (VerificationRuleAction == VerificationRuleActions.VerifyAny &&
                        signature.Status       == VerificationStatus.ValidSignature)
                    {
                        return true;
                    }

                }

                // Default, and when there is no signature policy (entry) defined!
                return Signatures.All(signature => signature.Status == VerificationStatus.ValidSignature);

            }
            catch (Exception e)
            {
                ErrorResponse = e.Message;
                return false;
            }

        }

        #endregion


        #region IEquatable<ACustomSignableData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two abstract custom signable data containers for equality.
        /// </summary>
        /// <param name="Object">An abstract custom signable data container to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ACustomSignableData aCustomSignableData &&
                   Equals(aCustomSignableData);

        #endregion

        #region Equals(ACustomSignableData)

        /// <summary>
        /// Compares two abstract custom signable data containers for equality.
        /// </summary>
        /// <param name="ACustomSignableData">An abstract custom signable data container to compare with.</param>
        public Boolean Equals(ACustomSignableData? ACustomSignableData)

            => ACustomSignableData is not null &&

             ((Signatures is     null && ACustomSignableData.Signatures is     null) ||
              (Signatures is not null && ACustomSignableData.Signatures is not null && Signatures.Equals(ACustomSignableData.Signatures)))&&

             ((CustomData is     null && ACustomSignableData.CustomData is     null) ||
              (CustomData is not null && ACustomSignableData.CustomData is not null && CustomData.Equals(ACustomSignableData.CustomData)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private Int32 hashCode;

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

            => CustomData?.ToString() ?? "";

        #endregion

    }

}
