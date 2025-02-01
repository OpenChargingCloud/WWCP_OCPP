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
    /// Extension methods for energy transfer modes.
    /// </summary>
    public static class EnergyTransferModeExtensions
    {

        /// <summary>
        /// Indicates whether this energy transfer mode is null or empty.
        /// </summary>
        /// <param name="EnergyTransferMode">An energy transfer mode.</param>
        public static Boolean IsNullOrEmpty(this EnergyTransferMode? EnergyTransferMode)
            => !EnergyTransferMode.HasValue || EnergyTransferMode.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this energy transfer mode is null or empty.
        /// </summary>
        /// <param name="EnergyTransferMode">An energy transfer mode.</param>
        public static Boolean IsNotNullOrEmpty(this EnergyTransferMode? EnergyTransferMode)
            => EnergyTransferMode.HasValue && EnergyTransferMode.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An energy transfer mode.
    /// </summary>
    public readonly struct EnergyTransferMode : IId,
                                                IEquatable<EnergyTransferMode>,
                                                IComparable<EnergyTransferMode>
    {

        #region Data

        private readonly static Dictionary<String, EnergyTransferMode>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                  InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this energy transfer mode is null or empty.
        /// </summary>
        public readonly  Boolean                          IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this energy transfer mode is NOT null or empty.
        /// </summary>
        public readonly  Boolean                          IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the energy transfer mode.
        /// </summary>
        public readonly  UInt64                           Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered transfer modes.
        /// </summary>
        public static    IEnumerable<EnergyTransferMode>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new energy transfer mode based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an energy transfer mode.</param>
        private EnergyTransferMode(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static EnergyTransferMode Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new EnergyTransferMode(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an energy transfer mode.
        /// </summary>
        /// <param name="Text">A text representation of an energy transfer mode.</param>
        public static EnergyTransferMode Parse(String Text)
        {

            if (TryParse(Text, out var energyTransferMode))
                return energyTransferMode;

            throw new ArgumentException($"Invalid text representation of an energy transfer mode: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as energy transfer mode.
        /// </summary>
        /// <param name="Text">A text representation of an energy transfer mode.</param>
        public static EnergyTransferMode? TryParse(String Text)
        {

            if (TryParse(Text, out var energyTransferMode))
                return energyTransferMode;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EnergyTransferMode)

        /// <summary>
        /// Try to parse the given text as energy transfer mode.
        /// </summary>
        /// <param name="Text">A text representation of an energy transfer mode.</param>
        /// <param name="EnergyTransferMode">The parsed energy transfer mode.</param>
        public static Boolean TryParse(String Text, out EnergyTransferMode EnergyTransferMode)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out EnergyTransferMode))
                    EnergyTransferMode = Register(Text);

                return true;

            }

            EnergyTransferMode = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this energy transfer mode.
        /// </summary>
        public EnergyTransferMode Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// AC single phase charging according to IEC 62196.
        /// </summary>
        public static EnergyTransferMode  AC_SinglePhase    { get; }
            = Register("AC_single_phase");

        /// <summary>
        /// AC two phase charging according to IEC 62196.
        /// </summary>
        public static EnergyTransferMode  AC_TwoPhases      { get; }
            = Register("AC_two_phase");

        /// <summary>
        /// AC three phase charging according to IEC 62196.
        /// </summary>
        public static EnergyTransferMode  AC_ThreePhases    { get; }
            = Register("AC_three_phase");

        /// <summary>
        /// DC charging.
        /// </summary>
        public static EnergyTransferMode  DC                { get; }
            = Register("DC");

        /// <summary>
        /// AC bidirectional
        /// </summary>
        public static EnergyTransferMode  AC_BPT            { get; }
            = Register("AC_BPT");

        /// <summary>
        /// AC bidirectional DER
        /// </summary>
        public static EnergyTransferMode  AC_BPT_DER        { get; }
            = Register("AC_BPT_DER");

        /// <summary>
        /// AC DER
        /// </summary>
        public static EnergyTransferMode  AC_DER            { get; }
            = Register("AC_DER");

        /// <summary>
        /// DC bidirectional
        /// </summary>
        public static EnergyTransferMode  DC_BPT            { get; }
            = Register("DC_BPT");

        /// <summary>
        /// DC charging via ACDP (pantograph)
        /// </summary>
        public static EnergyTransferMode  DC_ACDP           { get; }
            = Register("DC_ACDP");

        /// <summary>
        /// DC bidirectional charging via ACDP (pantograph)
        /// </summary>
        public static EnergyTransferMode  DC_ACDP_BPT       { get; }
            = Register("DC_ACDP_BPT");

        /// <summary>
        /// Wireless power transfer
        /// </summary>
        public static EnergyTransferMode  WPT               { get; }
            = Register("WPT");

        #endregion


        #region Operator overloading

        #region Operator == (EnergyTransferMode1, EnergyTransferMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyTransferMode1">An energy transfer mode.</param>
        /// <param name="EnergyTransferMode2">Another energy transfer mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EnergyTransferMode EnergyTransferMode1,
                                           EnergyTransferMode EnergyTransferMode2)

            => EnergyTransferMode1.Equals(EnergyTransferMode2);

        #endregion

        #region Operator != (EnergyTransferMode1, EnergyTransferMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyTransferMode1">An energy transfer mode.</param>
        /// <param name="EnergyTransferMode2">Another energy transfer mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EnergyTransferMode EnergyTransferMode1,
                                           EnergyTransferMode EnergyTransferMode2)

            => !EnergyTransferMode1.Equals(EnergyTransferMode2);

        #endregion

        #region Operator <  (EnergyTransferMode1, EnergyTransferMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyTransferMode1">An energy transfer mode.</param>
        /// <param name="EnergyTransferMode2">Another energy transfer mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EnergyTransferMode EnergyTransferMode1,
                                          EnergyTransferMode EnergyTransferMode2)

            => EnergyTransferMode1.CompareTo(EnergyTransferMode2) < 0;

        #endregion

        #region Operator <= (EnergyTransferMode1, EnergyTransferMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyTransferMode1">An energy transfer mode.</param>
        /// <param name="EnergyTransferMode2">Another energy transfer mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EnergyTransferMode EnergyTransferMode1,
                                           EnergyTransferMode EnergyTransferMode2)

            => EnergyTransferMode1.CompareTo(EnergyTransferMode2) <= 0;

        #endregion

        #region Operator >  (EnergyTransferMode1, EnergyTransferMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyTransferMode1">An energy transfer mode.</param>
        /// <param name="EnergyTransferMode2">Another energy transfer mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EnergyTransferMode EnergyTransferMode1,
                                          EnergyTransferMode EnergyTransferMode2)

            => EnergyTransferMode1.CompareTo(EnergyTransferMode2) > 0;

        #endregion

        #region Operator >= (EnergyTransferMode1, EnergyTransferMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyTransferMode1">An energy transfer mode.</param>
        /// <param name="EnergyTransferMode2">Another energy transfer mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EnergyTransferMode EnergyTransferMode1,
                                           EnergyTransferMode EnergyTransferMode2)

            => EnergyTransferMode1.CompareTo(EnergyTransferMode2) >= 0;

        #endregion

        #endregion

        #region IComparable<EnergyTransferMode> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two energy transfer modes.
        /// </summary>
        /// <param name="Object">energy transfer mode to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EnergyTransferMode energyTransferMode
                   ? CompareTo(energyTransferMode)
                   : throw new ArgumentException("The given object is not energy transfer mode!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EnergyTransferMode)

        /// <summary>
        /// Compares two energy transfer modes.
        /// </summary>
        /// <param name="EnergyTransferMode">energy transfer mode to compare with.</param>
        public Int32 CompareTo(EnergyTransferMode EnergyTransferMode)

            => String.Compare(InternalId,
                              EnergyTransferMode.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<EnergyTransferMode> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two energy transfer modes for equality.
        /// </summary>
        /// <param name="Object">energy transfer mode to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EnergyTransferMode energyTransferMode &&
                   Equals(energyTransferMode);

        #endregion

        #region Equals(EnergyTransferMode)

        /// <summary>
        /// Compares two energy transfer modes for equality.
        /// </summary>
        /// <param name="EnergyTransferMode">energy transfer mode to compare with.</param>
        public Boolean Equals(EnergyTransferMode EnergyTransferMode)

            => String.Equals(InternalId,
                             EnergyTransferMode.InternalId,
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
