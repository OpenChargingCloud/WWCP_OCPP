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

using System.Diagnostics.CodeAnalysis;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extension methods for reset types.
    /// </summary>
    public static class ResetTypeExtensions
    {

        /// <summary>
        /// Indicates whether this reset type is null or empty.
        /// </summary>
        /// <param name="ResetType">A reset type.</param>
        public static Boolean IsNullOrEmpty(this ResetType? ResetType)
            => !ResetType.HasValue || ResetType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this reset type is null or empty.
        /// </summary>
        /// <param name="ResetType">A reset type.</param>
        public static Boolean IsNotNullOrEmpty(this ResetType? ResetType)
            => ResetType.HasValue && ResetType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A reset type.
    /// </summary>
    public readonly struct ResetType : IId,
                                       IEquatable<ResetType>,
                                       IComparable<ResetType>
    {

        #region Data

        private readonly static Dictionary<String, ResetType>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                         InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this reset type is null or empty.
        /// </summary>
        public readonly Boolean               IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this reset type is NOT null or empty.
        /// </summary>
        public readonly Boolean               IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the reset type.
        /// </summary>
        public readonly UInt64                Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered reset types.
        /// </summary>
        public static IEnumerable<ResetType>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new reset type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a reset type.</param>
        private ResetType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static ResetType Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new ResetType(Text)
               );

        #endregion


        #region (static) Parse     (Text)

        /// <summary>
        /// Parse the given string as a reset type.
        /// </summary>
        /// <param name="Text">A text representation of a reset type.</param>
        public static ResetType Parse(String Text)
        {

            if (TryParse(Text, out var resetType))
                return resetType;

            throw new ArgumentException($"Invalid text representation of a reset type: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse  (Text)

        /// <summary>
        /// Try to parse the given text as a reset type.
        /// </summary>
        /// <param name="Text">A text representation of a reset type.</param>
        public static ResetType? TryParse(String Text)
        {

            if (TryParse(Text, out var resetType))
                return resetType;

            return null;

        }

        #endregion

        #region (static) TryParse  (Text, out ResetType)

        /// <summary>
        /// Try to parse the given text as a reset type.
        /// </summary>
        /// <param name="Text">A text representation of a reset type.</param>
        /// <param name="ResetType">The parsed reset type.</param>
        public static Boolean TryParse(String                             Text,
                                       [NotNullWhen(true)] out ResetType  ResetType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out ResetType))
                    ResetType = Register(Text);

                return true;

            }

            ResetType = default;
            return false;

        }

        #endregion

        #region (static) IsDefined (Text, out ResetType)

        /// <summary>
        /// Check whether the given text is a defined reset type.
        /// </summary>
        /// <param name="Text">A text representation of a reset type.</param>
        /// <param name="ResetType">The validated reset type.</param>
        public static Boolean IsDefined(String                            Text,
                                       [NotNullWhen(true)] out ResetType  ResetType)

            => lookup.TryGetValue(Text.Trim(), out ResetType);

        #endregion

        #region Clone

        /// <summary>
        /// Clone this reset type.
        /// </summary>
        public ResetType Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Full reboot of Charge Point software.
        /// </summary>
        public static ResetType  Hard    { get; }
            = Register("Hard");

        /// <summary>
        /// Return to initial status, gracefully terminating
        /// any transactions in progress.
        /// </summary>
        public static ResetType  Soft    { get; }
            = Register("Soft");

        #endregion


        #region Operator overloading

        #region Operator == (ResetType1, ResetType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetType1">A reset type.</param>
        /// <param name="ResetType2">Another reset type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ResetType ResetType1,
                                           ResetType ResetType2)

            => ResetType1.Equals(ResetType2);

        #endregion

        #region Operator != (ResetType1, ResetType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetType1">A reset type.</param>
        /// <param name="ResetType2">Another reset type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ResetType ResetType1,
                                           ResetType ResetType2)

            => !ResetType1.Equals(ResetType2);

        #endregion

        #region Operator <  (ResetType1, ResetType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetType1">A reset type.</param>
        /// <param name="ResetType2">Another reset type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ResetType ResetType1,
                                          ResetType ResetType2)

            => ResetType1.CompareTo(ResetType2) < 0;

        #endregion

        #region Operator <= (ResetType1, ResetType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetType1">A reset type.</param>
        /// <param name="ResetType2">Another reset type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ResetType ResetType1,
                                           ResetType ResetType2)

            => ResetType1.CompareTo(ResetType2) <= 0;

        #endregion

        #region Operator >  (ResetType1, ResetType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetType1">A reset type.</param>
        /// <param name="ResetType2">Another reset type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ResetType ResetType1,
                                          ResetType ResetType2)

            => ResetType1.CompareTo(ResetType2) > 0;

        #endregion

        #region Operator >= (ResetType1, ResetType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ResetType1">A reset type.</param>
        /// <param name="ResetType2">Another reset type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ResetType ResetType1,
                                           ResetType ResetType2)

            => ResetType1.CompareTo(ResetType2) >= 0;

        #endregion

        #endregion

        #region IComparable<ResetType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two reset types.
        /// </summary>
        /// <param name="Object">A reset type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ResetType resetType
                   ? CompareTo(resetType)
                   : throw new ArgumentException("The given object is not a reset type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ResetType)

        /// <summary>
        /// Compares two reset types.
        /// </summary>
        /// <param name="ResetType">A reset type to compare with.</param>
        public Int32 CompareTo(ResetType ResetType)

            => String.Compare(InternalId,
                              ResetType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ResetType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two reset types for equality.
        /// </summary>
        /// <param name="Object">A reset type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ResetType resetType &&
                   Equals(resetType);

        #endregion

        #region Equals(ResetType)

        /// <summary>
        /// Compares two reset types for equality.
        /// </summary>
        /// <param name="ResetType">A reset type to compare with.</param>
        public Boolean Equals(ResetType ResetType)

            => String.Equals(InternalId,
                             ResetType.InternalId,
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
