﻿/*
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
    /// An abstract custom data container.
    /// </summary>
    public abstract class ACustomData : IEquatable<ACustomData>
    {

        #region Properties

        /// <summary>
        /// An optional custom data object to allow to store any kind of customer specific data.
        /// </summary>
        public CustomData?  CustomData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract custom data container.
        /// </summary>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ACustomData(CustomData? CustomData = null)
        {

            this.CustomData = CustomData;

        }

        #endregion


        #region IEquatable<ACustomData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two abstract custom data containers for equality.
        /// </summary>
        /// <param name="Object">An abstract custom data container to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ACustomData aCustomData &&
                   Equals(aCustomData);

        #endregion

        #region Equals(ACustomData)

        /// <summary>
        /// Compares two abstract custom data containers for equality.
        /// </summary>
        /// <param name="ACustomData">An abstract custom data container to compare with.</param>
        public Boolean Equals(ACustomData? ACustomData)

            => ACustomData is not null &&

             ((CustomData is     null && ACustomData.CustomData is     null) ||
              (CustomData is not null && ACustomData.CustomData is not null && CustomData.Equals(ACustomData.CustomData)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => CustomData?.GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => CustomData?.ToString() ?? "";

        #endregion

    }


    public abstract class ACustomSignableData : ACustomData,
                                                IEquatable<ACustomSignableData>
    {

        #region Data

        private readonly HashSet<Signature> signatures;

        #endregion

        #region Properties

        /// <summary>
        /// The optional enumeration of cryptographic signatures for this message.
        /// </summary>
        [Optional]
        public IEnumerable<Signature>         Signatures
            => signatures;

        public IEnumerable<KeyPair>           SignKeys                      { get; }
        public IEnumerable<SignInfo>          SignInfos                     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new custom signable data.
        /// </summary>
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

        }

        #endregion


        public void AddSignature(Signature Signature)
        {
            lock (signatures)
            {
                signatures.Add(Signature);
            }
        }


        #region IEquatable<ACustomData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two abstract custom data containers for equality.
        /// </summary>
        /// <param name="Object">An abstract custom data container to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ACustomSignableData aCustomData &&
                   Equals(aCustomData);

        #endregion

        #region Equals(ACustomData)

        /// <summary>
        /// Compares two abstract custom data containers for equality.
        /// </summary>
        /// <param name="ACustomData">An abstract custom data container to compare with.</param>
        public Boolean Equals(ACustomSignableData? ACustomData)

            => ACustomData is not null &&

             ((CustomData is     null && ACustomData.CustomData is     null) ||
              (CustomData is not null && ACustomData.CustomData is not null && CustomData.Equals(ACustomData.CustomData)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => CustomData?.GetHashCode() ?? 0;

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
