/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for install certificate uses.
    /// </summary>
    public static class InstallCertificateUseExtensions
    {

        /// <summary>
        /// Indicates whether this install certificate use is null or empty.
        /// </summary>
        /// <param name="InstallCertificateUse">An install certificate use.</param>
        public static Boolean IsNullOrEmpty(this InstallCertificateUse? InstallCertificateUse)
            => !InstallCertificateUse.HasValue || InstallCertificateUse.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this install certificate use is null or empty.
        /// </summary>
        /// <param name="InstallCertificateUse">An install certificate use.</param>
        public static Boolean IsNotNullOrEmpty(this InstallCertificateUse? InstallCertificateUse)
            => InstallCertificateUse.HasValue && InstallCertificateUse.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An install certificate use.
    /// </summary>
    public readonly struct InstallCertificateUse : IId,
                                                   IEquatable<InstallCertificateUse>,
                                                   IComparable<InstallCertificateUse>
    {

        #region Data

        private readonly static Dictionary<String, InstallCertificateUse>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                     InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this install certificate use is null or empty.
        /// </summary>
        public readonly  Boolean                          IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this install certificate use is NOT null or empty.
        /// </summary>
        public readonly  Boolean                          IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the install certificate use.
        /// </summary>
        public readonly  UInt64                           Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered install certificate uses.
        /// </summary>
        public static IEnumerable<InstallCertificateUse>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new install certificate use based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an install certificate use.</param>
        private InstallCertificateUse(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static InstallCertificateUse Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new InstallCertificateUse(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an install certificate use.
        /// </summary>
        /// <param name="Text">A text representation of an install certificate use.</param>
        public static InstallCertificateUse Parse(String Text)
        {

            if (TryParse(Text, out var installCertificateUse))
                return installCertificateUse;

            throw new ArgumentException($"Invalid text representation of an install certificate use: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an install certificate use.
        /// </summary>
        /// <param name="Text">A text representation of an install certificate use.</param>
        public static InstallCertificateUse? TryParse(String Text)
        {

            if (TryParse(Text, out var installCertificateUse))
                return installCertificateUse;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out InstallCertificateUse)

        /// <summary>
        /// Try to parse the given text as an install certificate use.
        /// </summary>
        /// <param name="Text">A text representation of an install certificate use.</param>
        /// <param name="InstallCertificateUse">The parsed install certificate use.</param>
        public static Boolean TryParse(String Text, out InstallCertificateUse InstallCertificateUse)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out InstallCertificateUse))
                    InstallCertificateUse = Register(Text);

                return true;

            }

            InstallCertificateUse = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this install certificate use.
        /// </summary>
        public InstallCertificateUse Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Use for certificate of the V2G Root.
        /// </summary>
        public static InstallCertificateUse V2GRootCertificate             { get; }
            = Register("V2GRootCertificate");

        /// <summary>
        /// Use for certificate from an e-mobility service provider.
        /// To support PnC charging with contracts from service providers that not derived their certificates from the V2G root.
        /// </summary>
        public static InstallCertificateUse MORootCertificate              { get; }
            = Register("MORootCertificate");

        /// <summary>
        /// Root certificate for verification of the CSMS certificate.
        /// </summary>
        public static InstallCertificateUse CSMSRootCertificate            { get; }
            = Register("CSMSRootCertificate");

        /// <summary>
        /// Root certificate for verification of the Manufacturer certificate.
        /// </summary>
        public static InstallCertificateUse ManufacturerRootCertificate    { get; }
            = Register("ManufacturerRootCertificate");

        /// <summary>
        /// OEM root certificate for 2-way TLS with the electric vehicle.
        /// </summary>
        public static InstallCertificateUse OEMRootCertificate             { get; }
            = Register("OEMRootCertificate");

        #endregion


        #region Operator overloading

        #region Operator == (InstallCertificateUse1, InstallCertificateUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InstallCertificateUse1">An install certificate use.</param>
        /// <param name="InstallCertificateUse2">Another install certificate use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (InstallCertificateUse InstallCertificateUse1,
                                           InstallCertificateUse InstallCertificateUse2)

            => InstallCertificateUse1.Equals(InstallCertificateUse2);

        #endregion

        #region Operator != (InstallCertificateUse1, InstallCertificateUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InstallCertificateUse1">An install certificate use.</param>
        /// <param name="InstallCertificateUse2">Another install certificate use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (InstallCertificateUse InstallCertificateUse1,
                                           InstallCertificateUse InstallCertificateUse2)

            => !InstallCertificateUse1.Equals(InstallCertificateUse2);

        #endregion

        #region Operator <  (InstallCertificateUse1, InstallCertificateUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InstallCertificateUse1">An install certificate use.</param>
        /// <param name="InstallCertificateUse2">Another install certificate use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (InstallCertificateUse InstallCertificateUse1,
                                          InstallCertificateUse InstallCertificateUse2)

            => InstallCertificateUse1.CompareTo(InstallCertificateUse2) < 0;

        #endregion

        #region Operator <= (InstallCertificateUse1, InstallCertificateUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InstallCertificateUse1">An install certificate use.</param>
        /// <param name="InstallCertificateUse2">Another install certificate use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (InstallCertificateUse InstallCertificateUse1,
                                           InstallCertificateUse InstallCertificateUse2)

            => InstallCertificateUse1.CompareTo(InstallCertificateUse2) <= 0;

        #endregion

        #region Operator >  (InstallCertificateUse1, InstallCertificateUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InstallCertificateUse1">An install certificate use.</param>
        /// <param name="InstallCertificateUse2">Another install certificate use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (InstallCertificateUse InstallCertificateUse1,
                                          InstallCertificateUse InstallCertificateUse2)

            => InstallCertificateUse1.CompareTo(InstallCertificateUse2) > 0;

        #endregion

        #region Operator >= (InstallCertificateUse1, InstallCertificateUse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InstallCertificateUse1">An install certificate use.</param>
        /// <param name="InstallCertificateUse2">Another install certificate use.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (InstallCertificateUse InstallCertificateUse1,
                                           InstallCertificateUse InstallCertificateUse2)

            => InstallCertificateUse1.CompareTo(InstallCertificateUse2) >= 0;

        #endregion

        #endregion

        #region IComparable<InstallCertificateUse> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two install certificate uses.
        /// </summary>
        /// <param name="Object">An install certificate use to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is InstallCertificateUse installCertificateUse
                   ? CompareTo(installCertificateUse)
                   : throw new ArgumentException("The given object is not an install certificate use!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(InstallCertificateUse)

        /// <summary>
        /// Compares two install certificate uses.
        /// </summary>
        /// <param name="InstallCertificateUse">An install certificate use to compare with.</param>
        public Int32 CompareTo(InstallCertificateUse InstallCertificateUse)

            => String.Compare(InternalId,
                              InstallCertificateUse.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<InstallCertificateUse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two install certificate uses for equality.
        /// </summary>
        /// <param name="Object">An install certificate use to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is InstallCertificateUse installCertificateUse &&
                   Equals(installCertificateUse);

        #endregion

        #region Equals(InstallCertificateUse)

        /// <summary>
        /// Compares two install certificate uses for equality.
        /// </summary>
        /// <param name="InstallCertificateUse">An install certificate use to compare with.</param>
        public Boolean Equals(InstallCertificateUse InstallCertificateUse)

            => String.Equals(InternalId,
                             InstallCertificateUse.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
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
