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
    /// An OCPP CSE cryptographic signature policy.
    /// </summary>
    public class SignaturePolicy : ACustomData,
                                   IEquatable<SignaturePolicy>
    {

        #region Data

        private readonly HashSet<SignaturePolicyEntry>  entries    = new();
        private readonly HashSet<KeyPair>               keyPairs   = new();

        #endregion

        #region Properties

        /// <summary>
        /// The enumeration of signature policy policy entries.
        /// </summary>
        [Mandatory]
        public IEnumerable<SignaturePolicyEntry>  Entries
            => entries;

        /// <summary>
        /// The enumeration of signature policy policy entries.
        /// </summary>
        [Mandatory]
        public IEnumerable<KeyPair>               KeyPairs
            => keyPairs;

        /// <summary>
        /// The default action.
        /// </summary>
        [Mandatory]
        public SignaturePolicyAction              DefaultAction            { get; }

        /// <summary>
        /// The optional default cryptographic signing key pair.
        /// </summary>
        [Optional]
        public KeyPair?                           DefaultSigningKeyPair    { get; }

        #endregion

        #region Constructor(s)

        #region SignaturePolicy(                DefaultAction,        DefaultSigningKeyPair,        CustomData = null)

        /// <summary>
        /// Create a new OCPP CSE cryptographic signature policy.
        /// </summary>
        /// <param name="DefaultAction">The optional default action of this policy.</param>
        /// <param name="DefaultSigningKeyPair">The optional default cryptographic signing key pair.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SignaturePolicy(SignaturePolicyAction?              DefaultAction,
                               KeyPair?                            DefaultSigningKeyPair,
                               CustomData?                         CustomData   = null)

            : this(null,
                   DefaultAction,
                   DefaultSigningKeyPair,
                   CustomData)

        { }

        #endregion

        #region SignaturePolicy(Entries = null, DefaultAction = null, DefaultSigningKeyPair = null, CustomData = null)

        /// <summary>
        /// Create a new OCPP CSE cryptographic signature policy.
        /// </summary>
        /// <param name="Entries">An optional enumeration of cryptographic signature policy entries.</param>
        /// <param name="DefaultAction">The optional default action of this policy.</param>
        /// <param name="DefaultSigningKeyPair">The optional default cryptographic signing key pair.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SignaturePolicy(IEnumerable<SignaturePolicyEntry>?  Entries                 = null,
                               SignaturePolicyAction?              DefaultAction           = null,
                               KeyPair?                            DefaultSigningKeyPair   = null,
                               CustomData?                         CustomData              = null)

            : base(CustomData)

        {

            if (Entries is not null)
                foreach (var entry in Entries)
                    entries.Add(entry);

            this.DefaultAction          = DefaultAction ?? SignaturePolicyAction.Reject;
            this.DefaultSigningKeyPair  = DefaultSigningKeyPair;

            if (this.DefaultAction == SignaturePolicyAction.sign &&
                this.DefaultSigningKeyPair is null)
            {
                throw new ArgumentException("If the default action is 'sign', a default signing key pair must be provided!");
            }

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

        #endregion


        #region Documentation

        // tba.

        #endregion


        public SignaturePolicy AddSigningRule(JSONLDContext  Context,
                                              KeyPair        KeyPair)
        {

            entries.Add(new SignaturePolicyEntry(
                            entries.Any() ? entries.Max(entry => entry.Priority) + 1 : 1,
                            Context,
                            SignaturePolicyAction.sign,
                            KeyPair
                        ));

            return this;

        }

        public SignaturePolicy AddSigningRule(UInt32         Priority,
                                              JSONLDContext  Context,
                                              KeyPair        KeyPair)
        {

            entries.Add(new SignaturePolicyEntry(
                            Priority,
                            Context,
                            SignaturePolicyAction.sign,
                            KeyPair
                        ));

            return this;

        }

        public SignaturePolicy AddVerificationRule(JSONLDContext  Context)
        {

            entries.Add(new SignaturePolicyEntry(
                            entries.Any() ? entries.Max(entry => entry.Priority) + 1 : 1,
                            Context,
                            SignaturePolicyAction.verify
                        ));

            return this;

        }

        public SignaturePolicy AddVerificationRule(UInt32         Priority,
                                                   JSONLDContext  Context)
        {

            entries.Add(new SignaturePolicyEntry(
                            Priority,
                            Context,
                            SignaturePolicyAction.verify
                        ));

            return this;

        }


        public Boolean Has(JSONLDContext                          Context,
                           out IEnumerable<SignaturePolicyEntry>  SignaturePolicyEntries)
        {

            SignaturePolicyEntries = entries.Where(entry => entry.Context == Context);

            return SignaturePolicyEntries.Any();

        }


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

            => $"-";

        #endregion

    }

}
