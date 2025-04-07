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
    /// Extension methods for islanding detection methods.
    /// </summary>
    public static class IslandingDetectionMethodExtensions
    {

        /// <summary>
        /// Indicates whether this islanding detection method is null or empty.
        /// </summary>
        /// <param name="IslandingDetectionMethod">An islanding detection method.</param>
        public static Boolean IsNullOrEmpty(this IslandingDetectionMethod? IslandingDetectionMethod)
            => !IslandingDetectionMethod.HasValue || IslandingDetectionMethod.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this islanding detection method is null or empty.
        /// </summary>
        /// <param name="IslandingDetectionMethod">An islanding detection method.</param>
        public static Boolean IsNotNullOrEmpty(this IslandingDetectionMethod? IslandingDetectionMethod)
            => IslandingDetectionMethod.HasValue && IslandingDetectionMethod.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An islanding detection method.
    /// </summary>
    public readonly struct IslandingDetectionMethod : IId,
                                                      IEquatable<IslandingDetectionMethod>,
                                                      IComparable<IslandingDetectionMethod>
    {

        #region Data

        private readonly static Dictionary<String, IslandingDetectionMethod>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                  InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this islanding detection method is null or empty.
        /// </summary>
        public readonly  Boolean                          IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this islanding detection method is NOT null or empty.
        /// </summary>
        public readonly  Boolean                          IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the islanding detection method.
        /// </summary>
        public readonly  UInt64                           Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered transfer modes.
        /// </summary>
        public static    IEnumerable<IslandingDetectionMethod>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new islanding detection method based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an islanding detection method.</param>
        private IslandingDetectionMethod(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static IslandingDetectionMethod Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new IslandingDetectionMethod(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an islanding detection method.
        /// </summary>
        /// <param name="Text">A text representation of an islanding detection method.</param>
        public static IslandingDetectionMethod Parse(String Text)
        {

            if (TryParse(Text, out var islandingDetectionMethod))
                return islandingDetectionMethod;

            throw new ArgumentException($"Invalid text representation of an islanding detection method: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as islanding detection method.
        /// </summary>
        /// <param name="Text">A text representation of an islanding detection method.</param>
        public static IslandingDetectionMethod? TryParse(String Text)
        {

            if (TryParse(Text, out var islandingDetectionMethod))
                return islandingDetectionMethod;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out IslandingDetectionMethod)

        /// <summary>
        /// Try to parse the given text as islanding detection method.
        /// </summary>
        /// <param name="Text">A text representation of an islanding detection method.</param>
        /// <param name="IslandingDetectionMethod">The parsed islanding detection method.</param>
        public static Boolean TryParse(String Text, out IslandingDetectionMethod IslandingDetectionMethod)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out IslandingDetectionMethod))
                    IslandingDetectionMethod = Register(Text);

                return true;

            }

            IslandingDetectionMethod = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this islanding detection method.
        /// </summary>
        public IslandingDetectionMethod Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// No Anti Islanding Support
        /// </summary>
        public static IslandingDetectionMethod  NoAntiIslandingSupport    { get; }
            = Register("NoAntiIslandingSupport");

        /// <summary>
        /// RoCoF
        /// </summary>
        public static IslandingDetectionMethod  RoCoF                     { get; }
            = Register("RoCoF");

        /// <summary>
        /// UVP OVP
        /// </summary>
        public static IslandingDetectionMethod  UVP_OVP                   { get; }
            = Register("UVP_OVP");

        /// <summary>
        /// UFP OFP
        /// </summary>
        public static IslandingDetectionMethod  UFP_OFP                   { get; }
            = Register("UFP_OFP");

        /// <summary>
        /// Voltage Vector Shift
        /// </summary>
        public static IslandingDetectionMethod  VoltageVectorShift        { get; }
            = Register("VoltageVectorShift");

        /// <summary>
        /// Zero Crossing Detection
        /// </summary>
        public static IslandingDetectionMethod  ZeroCrossingDetection     { get; }
            = Register("ZeroCrossingDetection");

        /// <summary>
        /// Other Passive
        /// </summary>
        public static IslandingDetectionMethod  OtherPassive              { get; }
            = Register("OtherPassive");

        /// <summary>
        /// Impedance Measurement
        /// </summary>
        public static IslandingDetectionMethod  ImpedanceMeasurement      { get; }
            = Register("ImpedanceMeasurement");

        /// <summary>
        /// Impedance At Frequency
        /// </summary>
        public static IslandingDetectionMethod  ImpedanceAtFrequency      { get; }
            = Register("ImpedanceAtFrequency");

        /// <summary>
        /// Slip Mode Frequency Shift
        /// </summary>
        public static IslandingDetectionMethod  SlipModeFrequencyShift    { get; }
            = Register("SlipModeFrequencyShift");

        /// <summary>
        /// Sandia Frequency Shift
        /// </summary>
        public static IslandingDetectionMethod  SandiaFrequencyShift      { get; }
            = Register("SandiaFrequencyShift");

        /// <summary>
        /// Sandia Voltage Shift
        /// </summary>
        public static IslandingDetectionMethod  SandiaVoltageShift        { get; }
            = Register("SandiaVoltageShift");

        /// <summary>
        /// Frequency Jump
        /// </summary>
        public static IslandingDetectionMethod  FrequencyJump             { get; }
            = Register("FrequencyJump");

        /// <summary>
        /// RCLQ Factor
        /// </summary>
        public static IslandingDetectionMethod  RCLQFactor                { get; }
            = Register("RCLQFactor");

        /// <summary>
        /// Other Active
        /// </summary>
        public static IslandingDetectionMethod  OtherActive               { get; }
            = Register("OtherActive");

        #endregion


        #region Operator overloading

        #region Operator == (IslandingDetectionMethod1, IslandingDetectionMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IslandingDetectionMethod1">An islanding detection method.</param>
        /// <param name="IslandingDetectionMethod2">Another islanding detection method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (IslandingDetectionMethod IslandingDetectionMethod1,
                                           IslandingDetectionMethod IslandingDetectionMethod2)

            => IslandingDetectionMethod1.Equals(IslandingDetectionMethod2);

        #endregion

        #region Operator != (IslandingDetectionMethod1, IslandingDetectionMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IslandingDetectionMethod1">An islanding detection method.</param>
        /// <param name="IslandingDetectionMethod2">Another islanding detection method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (IslandingDetectionMethod IslandingDetectionMethod1,
                                           IslandingDetectionMethod IslandingDetectionMethod2)

            => !IslandingDetectionMethod1.Equals(IslandingDetectionMethod2);

        #endregion

        #region Operator <  (IslandingDetectionMethod1, IslandingDetectionMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IslandingDetectionMethod1">An islanding detection method.</param>
        /// <param name="IslandingDetectionMethod2">Another islanding detection method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (IslandingDetectionMethod IslandingDetectionMethod1,
                                          IslandingDetectionMethod IslandingDetectionMethod2)

            => IslandingDetectionMethod1.CompareTo(IslandingDetectionMethod2) < 0;

        #endregion

        #region Operator <= (IslandingDetectionMethod1, IslandingDetectionMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IslandingDetectionMethod1">An islanding detection method.</param>
        /// <param name="IslandingDetectionMethod2">Another islanding detection method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (IslandingDetectionMethod IslandingDetectionMethod1,
                                           IslandingDetectionMethod IslandingDetectionMethod2)

            => IslandingDetectionMethod1.CompareTo(IslandingDetectionMethod2) <= 0;

        #endregion

        #region Operator >  (IslandingDetectionMethod1, IslandingDetectionMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IslandingDetectionMethod1">An islanding detection method.</param>
        /// <param name="IslandingDetectionMethod2">Another islanding detection method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (IslandingDetectionMethod IslandingDetectionMethod1,
                                          IslandingDetectionMethod IslandingDetectionMethod2)

            => IslandingDetectionMethod1.CompareTo(IslandingDetectionMethod2) > 0;

        #endregion

        #region Operator >= (IslandingDetectionMethod1, IslandingDetectionMethod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IslandingDetectionMethod1">An islanding detection method.</param>
        /// <param name="IslandingDetectionMethod2">Another islanding detection method.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (IslandingDetectionMethod IslandingDetectionMethod1,
                                           IslandingDetectionMethod IslandingDetectionMethod2)

            => IslandingDetectionMethod1.CompareTo(IslandingDetectionMethod2) >= 0;

        #endregion

        #endregion

        #region IComparable<IslandingDetectionMethod> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two islanding detection methods.
        /// </summary>
        /// <param name="Object">islanding detection method to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is IslandingDetectionMethod islandingDetectionMethod
                   ? CompareTo(islandingDetectionMethod)
                   : throw new ArgumentException("The given object is not islanding detection method!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(IslandingDetectionMethod)

        /// <summary>
        /// Compares two islanding detection methods.
        /// </summary>
        /// <param name="IslandingDetectionMethod">islanding detection method to compare with.</param>
        public Int32 CompareTo(IslandingDetectionMethod IslandingDetectionMethod)

            => String.Compare(InternalId,
                              IslandingDetectionMethod.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<IslandingDetectionMethod> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two islanding detection methods for equality.
        /// </summary>
        /// <param name="Object">islanding detection method to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is IslandingDetectionMethod islandingDetectionMethod &&
                   Equals(islandingDetectionMethod);

        #endregion

        #region Equals(IslandingDetectionMethod)

        /// <summary>
        /// Compares two islanding detection methods for equality.
        /// </summary>
        /// <param name="IslandingDetectionMethod">islanding detection method to compare with.</param>
        public Boolean Equals(IslandingDetectionMethod IslandingDetectionMethod)

            => String.Equals(InternalId,
                             IslandingDetectionMethod.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
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
