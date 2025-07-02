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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for fault types.
    /// </summary>
    public static class FaultTypeExtensions
    {

        /// <summary>
        /// Indicates whether this fault type is null or empty.
        /// </summary>
        /// <param name="FaultType">A fault type.</param>
        public static Boolean IsNullOrEmpty(this FaultType? FaultType)
            => !FaultType.HasValue || FaultType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this fault type is null or empty.
        /// </summary>
        /// <param name="FaultType">A fault type.</param>
        public static Boolean IsNotNullOrEmpty(this FaultType? FaultType)
            => FaultType.HasValue && FaultType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A fault type.
    /// </summary>
    public readonly struct FaultType : IId,
                                       IEquatable<FaultType>,
                                       IComparable<FaultType>
    {

        #region Data

        private readonly static Dictionary<String, FaultType>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                         InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this fault type is null or empty.
        /// </summary>
        public readonly Boolean               IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this fault type is NOT null or empty.
        /// </summary>
        public readonly Boolean               IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the fault type.
        /// </summary>
        public readonly UInt64                Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered fault types.
        /// </summary>
        public static IEnumerable<FaultType>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new fault type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a fault type.</param>
        private FaultType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static FaultType Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new FaultType(Text)
               );

        #endregion


        #region (static) Parse     (Text)

        /// <summary>
        /// Parse the given string as a fault type.
        /// </summary>
        /// <param name="Text">A text representation of a fault type.</param>
        public static FaultType Parse(String Text)
        {

            if (TryParse(Text, out var faultType))
                return faultType;

            throw new ArgumentException($"Invalid text representation of a fault type: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse  (Text)

        /// <summary>
        /// Try to parse the given text as a fault type.
        /// </summary>
        /// <param name="Text">A text representation of a fault type.</param>
        public static FaultType? TryParse(String Text)
        {

            if (TryParse(Text, out var faultType))
                return faultType;

            return null;

        }

        #endregion

        #region (static) TryParse  (Text, out FaultType)

        /// <summary>
        /// Try to parse the given text as a fault type.
        /// </summary>
        /// <param name="Text">A text representation of a fault type.</param>
        /// <param name="FaultType">The parsed fault type.</param>
        public static Boolean TryParse(String                             Text,
                                       [NotNullWhen(true)] out FaultType  FaultType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out FaultType))
                    FaultType = Register(Text);

                return true;

            }

            FaultType = default;
            return false;

        }

        #endregion

        #region (static) IsDefined (Text, out FaultType)

        /// <summary>
        /// Check whether the given text is a defined fault type.
        /// </summary>
        /// <param name="Text">A text representation of a fault type.</param>
        /// <param name="FaultType">The validated fault type.</param>
        public static Boolean IsDefined(String                            Text,
                                       [NotNullWhen(true)] out FaultType  FaultType)

            => lookup.TryGetValue(Text.Trim(), out FaultType);

        #endregion

        #region Clone

        /// <summary>
        /// Clone this fault type.
        /// </summary>
        public FaultType Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Voltage High
        /// </summary>
        public static FaultType  VoltageHigh        { get; }
            = Register("VoltageHigh");

        /// <summary>
        /// Voltage Low
        /// </summary>
        public static FaultType  VoltageLow         { get; }
            = Register("VoltageLow");

        /// <summary>
        /// Phase Loss
        /// </summary>
        public static FaultType  PhaseLoss          { get; }
            = Register("PhaseLoss");

        /// <summary>
        /// Residual Current
        /// </summary>
        public static FaultType  ResidualCurrent    { get; }
            = Register("ResidualCurrent");

        #endregion


        #region Operator overloading

        #region Operator == (FaultType1, FaultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FaultType1">A fault type.</param>
        /// <param name="FaultType2">Another fault type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (FaultType FaultType1,
                                           FaultType FaultType2)

            => FaultType1.Equals(FaultType2);

        #endregion

        #region Operator != (FaultType1, FaultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FaultType1">A fault type.</param>
        /// <param name="FaultType2">Another fault type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (FaultType FaultType1,
                                           FaultType FaultType2)

            => !FaultType1.Equals(FaultType2);

        #endregion

        #region Operator <  (FaultType1, FaultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FaultType1">A fault type.</param>
        /// <param name="FaultType2">Another fault type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (FaultType FaultType1,
                                          FaultType FaultType2)

            => FaultType1.CompareTo(FaultType2) < 0;

        #endregion

        #region Operator <= (FaultType1, FaultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FaultType1">A fault type.</param>
        /// <param name="FaultType2">Another fault type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (FaultType FaultType1,
                                           FaultType FaultType2)

            => FaultType1.CompareTo(FaultType2) <= 0;

        #endregion

        #region Operator >  (FaultType1, FaultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FaultType1">A fault type.</param>
        /// <param name="FaultType2">Another fault type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (FaultType FaultType1,
                                          FaultType FaultType2)

            => FaultType1.CompareTo(FaultType2) > 0;

        #endregion

        #region Operator >= (FaultType1, FaultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FaultType1">A fault type.</param>
        /// <param name="FaultType2">Another fault type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (FaultType FaultType1,
                                           FaultType FaultType2)

            => FaultType1.CompareTo(FaultType2) >= 0;

        #endregion

        #endregion

        #region IComparable<FaultType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two fault types.
        /// </summary>
        /// <param name="Object">A fault type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is FaultType faultType
                   ? CompareTo(faultType)
                   : throw new ArgumentException("The given object is not a fault type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(FaultType)

        /// <summary>
        /// Compares two fault types.
        /// </summary>
        /// <param name="FaultType">A fault type to compare with.</param>
        public Int32 CompareTo(FaultType FaultType)

            => String.Compare(InternalId,
                              FaultType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<FaultType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two fault types for equality.
        /// </summary>
        /// <param name="Object">A fault type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is FaultType faultType &&
                   Equals(faultType);

        #endregion

        #region Equals(FaultType)

        /// <summary>
        /// Compares two fault types for equality.
        /// </summary>
        /// <param name="FaultType">A fault type to compare with.</param>
        public Boolean Equals(FaultType FaultType)

            => String.Equals(InternalId,
                             FaultType.InternalId,
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
