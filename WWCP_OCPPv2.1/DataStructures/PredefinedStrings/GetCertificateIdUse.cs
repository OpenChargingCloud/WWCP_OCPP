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
    /// Extension methods for get certificate id uses.
    /// </summary>
    public static class GetCertificateIdUseExtensions
    {

        /// <summary>
        /// Indicates whether this get certificate id use is null or empty.
        /// </summary>
        /// <param name="GetCertificateIdUse">A get certificate id use.</param>
        public static Boolean IsNullOrEmpty(this GetCertificateIdUse? GetCertificateIdUse)
            => !GetCertificateIdUse.HasValue || GetCertificateIdUse.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this get certificate id use is null or empty.
        /// </summary>
        /// <param name="GetCertificateIdUse">A get certificate id use.</param>
        public static Boolean IsNotNullOrEmpty(this GetCertificateIdUse? GetCertificateIdUse)
            => GetCertificateIdUse.HasValue && GetCertificateIdUse.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A get certificate id use.
    /// </summary>
    public readonly struct GetCertificateIdUse : IId,
                                                   IEquatable<GetCertificateIdUse>,
                                                   IComparable<GetCertificateIdUse>
    {

        #region Data

        private readonly static Dictionary<String, GetCertificateIdUse>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                     InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this get certificate id use is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this get certificate id use is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the get certificate id use.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get certificate id use based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a get certificate id use.</param>
        private GetCertificateIdUse(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static GetCertificateIdUse Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new GetCertificateIdUse(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a get certificate id use.
        /// </summary>
        /// <param name="Text">A text representation of a get certificate id use.</param>
        public static GetCertificateIdUse Parse(String Text)
        {

            if (TryParse(Text, out var installCertificateUse))
                return installCertificateUse;

            throw new ArgumentException($"Invalid text representation of a get certificate id use: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a get certificate id use.
        /// </summary>
        /// <param name="Text">A text representation of a get certificate id use.</param>
        public static GetCertificateIdUse? TryParse(String Text)
        {

            if (TryParse(Text, out var installCertificateUse))
                return installCertificateUse;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out GetCertificateIdUse)

        /// <summary>
        /// Try to parse the given text as a get certificate id use.
        /// </summary>
        /// <param name="Text">A text representation of a get certificate id use.</param>
        /// <param name="GetCertificateIdUse">The parsed get certificate id use.</param>
        public static Boolean TryParse(String Text, out GetCertificateIdUse GetCertificateIdUse)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out GetCertificateIdUse))
                    GetCertificateIdUse = Register(Text);

                return true;

            }

            GetCertificateIdUse = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this get certificate id use.
        /// </summary>
        public GetCertificateIdUse Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Use for certificate of the V2G Root.
        /// </summary>
        public static GetCertificateIdUse V2GRootCertificate             { get; }
            = Register("V2GRootCertificate");

        /// <summary>
        /// Use for certificate from an e-mobility service provider.
        /// To support PnC charging with contracts from service providers that not derived their certificates from the V2G root.
        /// </summary>
        public static GetCertificateIdUse MORootCertificate              { get; }
            = Register("MORootCertificate");

        /// <summary>
        /// Root certificate for verification of the CSMS certificate.
        /// </summary>
        public static GetCertificateIdUse CSMSRootCertificate            { get; }
            = Register("CSMSRootCertificate");

        /// <summary>
        /// ISO 15118 V2G certificate chain (excluding the V2GRootCertificate).
        /// </summary>
        public static GetCertificateIdUse V2GCertificateChain            { get; }
            = Register("V2GCertificateChain");

        /// <summary>
        /// Root certificate for verification of the Manufacturer certificate.
        /// </summary>
        public static GetCertificateIdUse ManufacturerRootCertificate    { get; }
            = Register("ManufacturerRootCertificate");

        /// <summary>
        /// OEM root certificate for 2-way TLS with the electric vehicle.
        /// </summary>
        public static GetCertificateIdUse OEMRootCertificate             { get; }
            = Register("OEMRootCertificate");

        #endregion


        #region Operator overloading

        #region Operator == (GetCertificateIdUse1, GetCertificateIdUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetCertificateIdUse1">A get certificate id use.</param>
        /// <param name="GetCertificateIdUse2">Another get certificate id use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (GetCertificateIdUse GetCertificateIdUse1,
                                           GetCertificateIdUse GetCertificateIdUse2)

            => GetCertificateIdUse1.Equals(GetCertificateIdUse2);

        #endregion

        #region Operator != (GetCertificateIdUse1, GetCertificateIdUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetCertificateIdUse1">A get certificate id use.</param>
        /// <param name="GetCertificateIdUse2">Another get certificate id use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (GetCertificateIdUse GetCertificateIdUse1,
                                           GetCertificateIdUse GetCertificateIdUse2)

            => !GetCertificateIdUse1.Equals(GetCertificateIdUse2);

        #endregion

        #region Operator <  (GetCertificateIdUse1, GetCertificateIdUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetCertificateIdUse1">A get certificate id use.</param>
        /// <param name="GetCertificateIdUse2">Another get certificate id use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (GetCertificateIdUse GetCertificateIdUse1,
                                          GetCertificateIdUse GetCertificateIdUse2)

            => GetCertificateIdUse1.CompareTo(GetCertificateIdUse2) < 0;

        #endregion

        #region Operator <= (GetCertificateIdUse1, GetCertificateIdUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetCertificateIdUse1">A get certificate id use.</param>
        /// <param name="GetCertificateIdUse2">Another get certificate id use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (GetCertificateIdUse GetCertificateIdUse1,
                                           GetCertificateIdUse GetCertificateIdUse2)

            => GetCertificateIdUse1.CompareTo(GetCertificateIdUse2) <= 0;

        #endregion

        #region Operator >  (GetCertificateIdUse1, GetCertificateIdUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetCertificateIdUse1">A get certificate id use.</param>
        /// <param name="GetCertificateIdUse2">Another get certificate id use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (GetCertificateIdUse GetCertificateIdUse1,
                                          GetCertificateIdUse GetCertificateIdUse2)

            => GetCertificateIdUse1.CompareTo(GetCertificateIdUse2) > 0;

        #endregion

        #region Operator >= (GetCertificateIdUse1, GetCertificateIdUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GetCertificateIdUse1">A get certificate id use.</param>
        /// <param name="GetCertificateIdUse2">Another get certificate id use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (GetCertificateIdUse GetCertificateIdUse1,
                                           GetCertificateIdUse GetCertificateIdUse2)

            => GetCertificateIdUse1.CompareTo(GetCertificateIdUse2) >= 0;

        #endregion

        #endregion

        #region IComparable<GetCertificateIdUse> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two get certificate id uses.
        /// </summary>
        /// <param name="Object">A get certificate id use to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is GetCertificateIdUse installCertificateUse
                   ? CompareTo(installCertificateUse)
                   : throw new ArgumentException("The given object is not a get certificate id use!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(GetCertificateIdUse)

        /// <summary>
        /// Compares two get certificate id uses.
        /// </summary>
        /// <param name="GetCertificateIdUse">A get certificate id use to compare with.</param>
        public Int32 CompareTo(GetCertificateIdUse GetCertificateIdUse)

            => String.Compare(InternalId,
                              GetCertificateIdUse.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<GetCertificateIdUse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get certificate id uses for equality.
        /// </summary>
        /// <param name="Object">A get certificate id use to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetCertificateIdUse installCertificateUse &&
                   Equals(installCertificateUse);

        #endregion

        #region Equals(GetCertificateIdUse)

        /// <summary>
        /// Compares two get certificate id uses for equality.
        /// </summary>
        /// <param name="GetCertificateIdUse">A get certificate id use to compare with.</param>
        public Boolean Equals(GetCertificateIdUse GetCertificateIdUse)

            => String.Equals(InternalId,
                             GetCertificateIdUse.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
