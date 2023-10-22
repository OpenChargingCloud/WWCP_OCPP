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

using org.GraphDefined.Vanaheimr.Illias;

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
