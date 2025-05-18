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
    /// Extension methods for install certificate groups.
    /// </summary>
    public static class CertificateGroupExtensions
    {

        /// <summary>
        /// Indicates whether this install certificate group is null or empty.
        /// </summary>
        /// <param name="CertificateGroup">A certificate group.</param>
        public static Boolean IsNullOrEmpty(this CertificateGroup? CertificateGroup)
            => !CertificateGroup.HasValue || CertificateGroup.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this install certificate group is null or empty.
        /// </summary>
        /// <param name="CertificateGroup">A certificate group.</param>
        public static Boolean IsNotNullOrEmpty(this CertificateGroup? CertificateGroup)
            => CertificateGroup.HasValue && CertificateGroup.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A certificate group.
    /// </summary>
    public readonly struct CertificateGroup : IId,
                                              IEquatable<CertificateGroup>,
                                              IComparable<CertificateGroup>
    {

        #region Data

        private readonly static Dictionary<String, CertificateGroup>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                     InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this install certificate group is null or empty.
        /// </summary>
        public readonly  Boolean                          IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this install certificate group is NOT null or empty.
        /// </summary>
        public readonly  Boolean                          IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the install certificate group.
        /// </summary>
        public readonly  UInt64                           Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered install certificate groups.
        /// </summary>
        public static IEnumerable<CertificateGroup>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new install certificate group based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a certificate group.</param>
        private CertificateGroup(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static CertificateGroup Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new CertificateGroup(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a certificate group.
        /// </summary>
        /// <param name="Text">A text representation of a certificate group.</param>
        public static CertificateGroup Parse(String Text)
        {

            if (TryParse(Text, out var certificateGroup))
                return certificateGroup;

            throw new ArgumentException($"Invalid text representation of a certificate group: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a certificate group.
        /// </summary>
        /// <param name="Text">A text representation of a certificate group.</param>
        public static CertificateGroup? TryParse(String Text)
        {

            if (TryParse(Text, out var certificateGroup))
                return certificateGroup;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CertificateGroup)

        /// <summary>
        /// Try to parse the given text as a certificate group.
        /// </summary>
        /// <param name="Text">A text representation of a certificate group.</param>
        /// <param name="CertificateGroup">The parsed install certificate group.</param>
        public static Boolean TryParse(String Text, out CertificateGroup CertificateGroup)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out CertificateGroup))
                    CertificateGroup = Register(Text);

                return true;

            }

            CertificateGroup = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this install certificate group.
        /// </summary>
        public CertificateGroup Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (CertificateGroup1, CertificateGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateGroup1">A certificate group.</param>
        /// <param name="CertificateGroup2">Another install certificate group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CertificateGroup CertificateGroup1,
                                           CertificateGroup CertificateGroup2)

            => CertificateGroup1.Equals(CertificateGroup2);

        #endregion

        #region Operator != (CertificateGroup1, CertificateGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateGroup1">A certificate group.</param>
        /// <param name="CertificateGroup2">Another install certificate group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CertificateGroup CertificateGroup1,
                                           CertificateGroup CertificateGroup2)

            => !CertificateGroup1.Equals(CertificateGroup2);

        #endregion

        #region Operator <  (CertificateGroup1, CertificateGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateGroup1">A certificate group.</param>
        /// <param name="CertificateGroup2">Another install certificate group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CertificateGroup CertificateGroup1,
                                          CertificateGroup CertificateGroup2)

            => CertificateGroup1.CompareTo(CertificateGroup2) < 0;

        #endregion

        #region Operator <= (CertificateGroup1, CertificateGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateGroup1">A certificate group.</param>
        /// <param name="CertificateGroup2">Another install certificate group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CertificateGroup CertificateGroup1,
                                           CertificateGroup CertificateGroup2)

            => CertificateGroup1.CompareTo(CertificateGroup2) <= 0;

        #endregion

        #region Operator >  (CertificateGroup1, CertificateGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateGroup1">A certificate group.</param>
        /// <param name="CertificateGroup2">Another install certificate group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CertificateGroup CertificateGroup1,
                                          CertificateGroup CertificateGroup2)

            => CertificateGroup1.CompareTo(CertificateGroup2) > 0;

        #endregion

        #region Operator >= (CertificateGroup1, CertificateGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateGroup1">A certificate group.</param>
        /// <param name="CertificateGroup2">Another install certificate group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CertificateGroup CertificateGroup1,
                                           CertificateGroup CertificateGroup2)

            => CertificateGroup1.CompareTo(CertificateGroup2) >= 0;

        #endregion

        #endregion

        #region IComparable<CertificateGroup> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two install certificate groups.
        /// </summary>
        /// <param name="Object">A certificate group to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CertificateGroup certificateGroup
                   ? CompareTo(certificateGroup)
                   : throw new ArgumentException("The given object is not a certificate group!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CertificateGroup)

        /// <summary>
        /// Compares two install certificate groups.
        /// </summary>
        /// <param name="CertificateGroup">A certificate group to compare with.</param>
        public Int32 CompareTo(CertificateGroup CertificateGroup)

            => String.Compare(InternalId,
                              CertificateGroup.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CertificateGroup> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two install certificate groups for equality.
        /// </summary>
        /// <param name="Object">A certificate group to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CertificateGroup certificateGroup &&
                   Equals(certificateGroup);

        #endregion

        #region Equals(CertificateGroup)

        /// <summary>
        /// Compares two install certificate groups for equality.
        /// </summary>
        /// <param name="CertificateGroup">A certificate group to compare with.</param>
        public Boolean Equals(CertificateGroup CertificateGroup)

            => String.Equals(InternalId,
                             CertificateGroup.InternalId,
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
