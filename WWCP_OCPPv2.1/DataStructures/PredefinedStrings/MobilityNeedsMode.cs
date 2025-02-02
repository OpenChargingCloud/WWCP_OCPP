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
    /// Extension methods for mobility needs modes.
    /// </summary>
    public static class MobilityNeedsModeExtensions
    {

        /// <summary>
        /// Indicates whether this mobility needs mode is null or empty.
        /// </summary>
        /// <param name="MobilityNeedsMode">A mobility needs mode.</param>
        public static Boolean IsNullOrEmpty(this MobilityNeedsMode? MobilityNeedsMode)
            => !MobilityNeedsMode.HasValue || MobilityNeedsMode.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this mobility needs mode is null or empty.
        /// </summary>
        /// <param name="MobilityNeedsMode">A mobility needs mode.</param>
        public static Boolean IsNotNullOrEmpty(this MobilityNeedsMode? MobilityNeedsMode)
            => MobilityNeedsMode.HasValue && MobilityNeedsMode.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A mobility needs mode.
    /// </summary>
    public readonly struct MobilityNeedsMode : IId,
                                               IEquatable<MobilityNeedsMode>,
                                               IComparable<MobilityNeedsMode>
    {

        #region Data

        private readonly static Dictionary<String, MobilityNeedsMode>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                 InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this mobility needs mode is null or empty.
        /// </summary>
        public readonly  Boolean                         IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this mobility needs mode is NOT null or empty.
        /// </summary>
        public readonly  Boolean                         IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the mobility needs mode.
        /// </summary>
        public readonly  UInt64                          Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered needs modes.
        /// </summary>
        public static    IEnumerable<MobilityNeedsMode>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new mobility needs mode based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a mobility needs mode.</param>
        private MobilityNeedsMode(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static MobilityNeedsMode Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new MobilityNeedsMode(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a mobility needs mode.
        /// </summary>
        /// <param name="Text">A text representation of a mobility needs mode.</param>
        public static MobilityNeedsMode Parse(String Text)
        {

            if (TryParse(Text, out var mobilityNeedsMode))
                return mobilityNeedsMode;

            throw new ArgumentException($"Invalid text representation of a mobility needs mode: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as mobility needs mode.
        /// </summary>
        /// <param name="Text">A text representation of a mobility needs mode.</param>
        public static MobilityNeedsMode? TryParse(String Text)
        {

            if (TryParse(Text, out var mobilityNeedsMode))
                return mobilityNeedsMode;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out MobilityNeedsMode)

        /// <summary>
        /// Try to parse the given text as mobility needs mode.
        /// </summary>
        /// <param name="Text">A text representation of a mobility needs mode.</param>
        /// <param name="MobilityNeedsMode">The parsed mobility needs mode.</param>
        public static Boolean TryParse(String Text, out MobilityNeedsMode MobilityNeedsMode)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out MobilityNeedsMode))
                    MobilityNeedsMode = Register(Text);

                return true;

            }

            MobilityNeedsMode = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this mobility needs mode.
        /// </summary>
        public MobilityNeedsMode Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Only EV determines min/target state-of-charge and departure time.
        /// </summary>
        public static MobilityNeedsMode  EVCC         { get; }
            = Register("EVCC");

        /// <summary>
        /// The Charging station or the CSMS may also update min/target
        /// state-of-charge and departure time.
        /// </summary>
        public static MobilityNeedsMode  EVCC_SECC    { get; }
            = Register("EVCC_SECC");

        #endregion


        #region Operator overloading

        #region Operator == (MobilityNeedsMode1, MobilityNeedsMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MobilityNeedsMode1">A mobility needs mode.</param>
        /// <param name="MobilityNeedsMode2">Another mobility needs mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MobilityNeedsMode MobilityNeedsMode1,
                                           MobilityNeedsMode MobilityNeedsMode2)

            => MobilityNeedsMode1.Equals(MobilityNeedsMode2);

        #endregion

        #region Operator != (MobilityNeedsMode1, MobilityNeedsMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MobilityNeedsMode1">A mobility needs mode.</param>
        /// <param name="MobilityNeedsMode2">Another mobility needs mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MobilityNeedsMode MobilityNeedsMode1,
                                           MobilityNeedsMode MobilityNeedsMode2)

            => !MobilityNeedsMode1.Equals(MobilityNeedsMode2);

        #endregion

        #region Operator <  (MobilityNeedsMode1, MobilityNeedsMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MobilityNeedsMode1">A mobility needs mode.</param>
        /// <param name="MobilityNeedsMode2">Another mobility needs mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (MobilityNeedsMode MobilityNeedsMode1,
                                          MobilityNeedsMode MobilityNeedsMode2)

            => MobilityNeedsMode1.CompareTo(MobilityNeedsMode2) < 0;

        #endregion

        #region Operator <= (MobilityNeedsMode1, MobilityNeedsMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MobilityNeedsMode1">A mobility needs mode.</param>
        /// <param name="MobilityNeedsMode2">Another mobility needs mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (MobilityNeedsMode MobilityNeedsMode1,
                                           MobilityNeedsMode MobilityNeedsMode2)

            => MobilityNeedsMode1.CompareTo(MobilityNeedsMode2) <= 0;

        #endregion

        #region Operator >  (MobilityNeedsMode1, MobilityNeedsMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MobilityNeedsMode1">A mobility needs mode.</param>
        /// <param name="MobilityNeedsMode2">Another mobility needs mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (MobilityNeedsMode MobilityNeedsMode1,
                                          MobilityNeedsMode MobilityNeedsMode2)

            => MobilityNeedsMode1.CompareTo(MobilityNeedsMode2) > 0;

        #endregion

        #region Operator >= (MobilityNeedsMode1, MobilityNeedsMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MobilityNeedsMode1">A mobility needs mode.</param>
        /// <param name="MobilityNeedsMode2">Another mobility needs mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (MobilityNeedsMode MobilityNeedsMode1,
                                           MobilityNeedsMode MobilityNeedsMode2)

            => MobilityNeedsMode1.CompareTo(MobilityNeedsMode2) >= 0;

        #endregion

        #endregion

        #region IComparable<MobilityNeedsMode> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two mobility needs modes.
        /// </summary>
        /// <param name="Object">A mobility needs mode to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is MobilityNeedsMode mobilityNeedsMode
                   ? CompareTo(mobilityNeedsMode)
                   : throw new ArgumentException("The given object is not mobility needs mode!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(MobilityNeedsMode)

        /// <summary>
        /// Compares two mobility needs modes.
        /// </summary>
        /// <param name="MobilityNeedsMode">A mobility needs mode to compare with.</param>
        public Int32 CompareTo(MobilityNeedsMode MobilityNeedsMode)

            => String.Compare(InternalId,
                              MobilityNeedsMode.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<MobilityNeedsMode> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two mobility needs modes for equality.
        /// </summary>
        /// <param name="Object">A mobility needs mode to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MobilityNeedsMode mobilityNeedsMode &&
                   Equals(mobilityNeedsMode);

        #endregion

        #region Equals(MobilityNeedsMode)

        /// <summary>
        /// Compares two mobility needs modes for equality.
        /// </summary>
        /// <param name="MobilityNeedsMode">A mobility needs mode to compare with.</param>
        public Boolean Equals(MobilityNeedsMode MobilityNeedsMode)

            => String.Equals(InternalId,
                             MobilityNeedsMode.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.GetHashCode() ?? 0;

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
