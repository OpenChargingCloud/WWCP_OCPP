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
    /// Extension methods for islanding detection types.
    /// </summary>
    public static class IslandingDetectionTypeExtensions
    {

        /// <summary>
        /// Indicates whether this islanding detection type is null or empty.
        /// </summary>
        /// <param name="IslandingDetectionType">An islanding detection type.</param>
        public static Boolean IsNullOrEmpty(this IslandingDetectionType? IslandingDetectionType)
            => !IslandingDetectionType.HasValue || IslandingDetectionType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this islanding detection type is null or empty.
        /// </summary>
        /// <param name="IslandingDetectionType">An islanding detection type.</param>
        public static Boolean IsNotNullOrEmpty(this IslandingDetectionType? IslandingDetectionType)
            => IslandingDetectionType.HasValue && IslandingDetectionType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An islanding detection type.
    /// </summary>
    public readonly struct IslandingDetectionType : IId,
                                                    IEquatable<IslandingDetectionType>,
                                                    IComparable<IslandingDetectionType>
    {

        #region Data

        private readonly static Dictionary<String, IslandingDetectionType>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                  InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this islanding detection type is null or empty.
        /// </summary>
        public readonly  Boolean                          IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this islanding detection type is NOT null or empty.
        /// </summary>
        public readonly  Boolean                          IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the islanding detection type.
        /// </summary>
        public readonly  UInt64                           Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered transfer modes.
        /// </summary>
        public static    IEnumerable<IslandingDetectionType>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new islanding detection type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an islanding detection type.</param>
        private IslandingDetectionType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static IslandingDetectionType Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new IslandingDetectionType(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an islanding detection type.
        /// </summary>
        /// <param name="Text">A text representation of an islanding detection type.</param>
        public static IslandingDetectionType Parse(String Text)
        {

            if (TryParse(Text, out var islandingDetectionType))
                return islandingDetectionType;

            throw new ArgumentException($"Invalid text representation of an islanding detection type: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as islanding detection type.
        /// </summary>
        /// <param name="Text">A text representation of an islanding detection type.</param>
        public static IslandingDetectionType? TryParse(String Text)
        {

            if (TryParse(Text, out var islandingDetectionType))
                return islandingDetectionType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out IslandingDetectionType)

        /// <summary>
        /// Try to parse the given text as islanding detection type.
        /// </summary>
        /// <param name="Text">A text representation of an islanding detection type.</param>
        /// <param name="IslandingDetectionType">The parsed islanding detection type.</param>
        public static Boolean TryParse(String Text, out IslandingDetectionType IslandingDetectionType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out IslandingDetectionType))
                    IslandingDetectionType = Register(Text);

                return true;

            }

            IslandingDetectionType = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this islanding detection type.
        /// </summary>
        public IslandingDetectionType Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// NoAntiIslandingSupport
        /// </summary>
        public static IslandingDetectionType  NoAntiIslandingSupport    { get; }
            = Register("NoAntiIslandingSupport");

        /// <summary>
        /// RoCoF
        /// </summary>
        public static IslandingDetectionType  RoCoF                     { get; }
            = Register("RoCoF");

        /// <summary>
        /// UVP_OVP
        /// </summary>
        public static IslandingDetectionType  UVP_OVP                   { get; }
            = Register("UVP_OVP");

        /// <summary>
        /// UFP_OFP
        /// </summary>
        public static IslandingDetectionType  UFP_OFP                   { get; }
            = Register("UFP_OFP");

        /// <summary>
        /// VoltageVectorShift
        /// </summary>
        public static IslandingDetectionType  VoltageVectorShift        { get; }
            = Register("VoltageVectorShift");

        /// <summary>
        /// ZeroCrossingDetection
        /// </summary>
        public static IslandingDetectionType  ZeroCrossingDetection     { get; }
            = Register("ZeroCrossingDetection");

        /// <summary>
        /// OtherPassive
        /// </summary>
        public static IslandingDetectionType  OtherPassive              { get; }
            = Register("OtherPassive");

        /// <summary>
        /// ImpedanceMeasurement
        /// </summary>
        public static IslandingDetectionType  ImpedanceMeasurement      { get; }
            = Register("ImpedanceMeasurement");

        /// <summary>
        /// ImpedanceAtFrequency
        /// </summary>
        public static IslandingDetectionType  ImpedanceAtFrequency      { get; }
            = Register("ImpedanceAtFrequency");

        /// <summary>
        /// SlipModeFrequencyShift
        /// </summary>
        public static IslandingDetectionType  SlipModeFrequencyShift    { get; }
            = Register("SlipModeFrequencyShift");

        /// <summary>
        /// SandiaFrequencyShift
        /// </summary>
        public static IslandingDetectionType  SandiaFrequencyShift      { get; }
            = Register("SandiaFrequencyShift");

        /// <summary>
        /// SandiaVoltageShift
        /// </summary>
        public static IslandingDetectionType  SandiaVoltageShift        { get; }
            = Register("SandiaVoltageShift");

        /// <summary>
        /// FrequencyJump
        /// </summary>
        public static IslandingDetectionType  FrequencyJump             { get; }
            = Register("FrequencyJump");

        /// <summary>
        /// RCLQFactor
        /// </summary>
        public static IslandingDetectionType  RCLQFactor                { get; }
            = Register("RCLQFactor");

        /// <summary>
        /// OtherActive
        /// </summary>
        public static IslandingDetectionType  OtherActive               { get; }
            = Register("OtherActive");

        #endregion


        #region Operator overloading

        #region Operator == (IslandingDetectionType1, IslandingDetectionType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IslandingDetectionType1">An islanding detection type.</param>
        /// <param name="IslandingDetectionType2">Another islanding detection type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (IslandingDetectionType IslandingDetectionType1,
                                           IslandingDetectionType IslandingDetectionType2)

            => IslandingDetectionType1.Equals(IslandingDetectionType2);

        #endregion

        #region Operator != (IslandingDetectionType1, IslandingDetectionType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IslandingDetectionType1">An islanding detection type.</param>
        /// <param name="IslandingDetectionType2">Another islanding detection type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (IslandingDetectionType IslandingDetectionType1,
                                           IslandingDetectionType IslandingDetectionType2)

            => !IslandingDetectionType1.Equals(IslandingDetectionType2);

        #endregion

        #region Operator <  (IslandingDetectionType1, IslandingDetectionType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IslandingDetectionType1">An islanding detection type.</param>
        /// <param name="IslandingDetectionType2">Another islanding detection type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (IslandingDetectionType IslandingDetectionType1,
                                          IslandingDetectionType IslandingDetectionType2)

            => IslandingDetectionType1.CompareTo(IslandingDetectionType2) < 0;

        #endregion

        #region Operator <= (IslandingDetectionType1, IslandingDetectionType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IslandingDetectionType1">An islanding detection type.</param>
        /// <param name="IslandingDetectionType2">Another islanding detection type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (IslandingDetectionType IslandingDetectionType1,
                                           IslandingDetectionType IslandingDetectionType2)

            => IslandingDetectionType1.CompareTo(IslandingDetectionType2) <= 0;

        #endregion

        #region Operator >  (IslandingDetectionType1, IslandingDetectionType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IslandingDetectionType1">An islanding detection type.</param>
        /// <param name="IslandingDetectionType2">Another islanding detection type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (IslandingDetectionType IslandingDetectionType1,
                                          IslandingDetectionType IslandingDetectionType2)

            => IslandingDetectionType1.CompareTo(IslandingDetectionType2) > 0;

        #endregion

        #region Operator >= (IslandingDetectionType1, IslandingDetectionType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IslandingDetectionType1">An islanding detection type.</param>
        /// <param name="IslandingDetectionType2">Another islanding detection type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (IslandingDetectionType IslandingDetectionType1,
                                           IslandingDetectionType IslandingDetectionType2)

            => IslandingDetectionType1.CompareTo(IslandingDetectionType2) >= 0;

        #endregion

        #endregion

        #region IComparable<IslandingDetectionType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two islanding detection types.
        /// </summary>
        /// <param name="Object">islanding detection type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is IslandingDetectionType islandingDetectionType
                   ? CompareTo(islandingDetectionType)
                   : throw new ArgumentException("The given object is not islanding detection type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(IslandingDetectionType)

        /// <summary>
        /// Compares two islanding detection types.
        /// </summary>
        /// <param name="IslandingDetectionType">islanding detection type to compare with.</param>
        public Int32 CompareTo(IslandingDetectionType IslandingDetectionType)

            => String.Compare(InternalId,
                              IslandingDetectionType.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<IslandingDetectionType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two islanding detection types for equality.
        /// </summary>
        /// <param name="Object">islanding detection type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is IslandingDetectionType islandingDetectionType &&
                   Equals(islandingDetectionType);

        #endregion

        #region Equals(IslandingDetectionType)

        /// <summary>
        /// Compares two islanding detection types for equality.
        /// </summary>
        /// <param name="IslandingDetectionType">islanding detection type to compare with.</param>
        public Boolean Equals(IslandingDetectionType IslandingDetectionType)

            => String.Equals(InternalId,
                             IslandingDetectionType.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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
