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
    /// Extension methods for Distributed Energy Resource (DER) control status.
    /// </summary>
    public static class NTPModeExtensions
    {

        /// <summary>
        /// Indicates whether this NTP mode is null or empty.
        /// </summary>
        /// <param name="NTPMode">A NTP mode.</param>
        public static Boolean IsNullOrEmpty(this NTPMode? NTPMode)
            => !NTPMode.HasValue || NTPMode.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this NTP mode is null or empty.
        /// </summary>
        /// <param name="NTPMode">A NTP mode.</param>
        public static Boolean IsNotNullOrEmpty(this NTPMode? NTPMode)
            => NTPMode.HasValue && NTPMode.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A Network Time Protocol (NTP) Mode.
    /// </summary>
    public readonly struct NTPMode : IId,
                                     IEquatable<NTPMode>,
                                     IComparable<NTPMode>
    {

        #region Data

        private readonly static Dictionary<String, NTPMode>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                  InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this NTP mode is null or empty.
        /// </summary>
        public readonly  Boolean                          IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this NTP mode is NOT null or empty.
        /// </summary>
        public readonly  Boolean                          IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the NTP mode.
        /// </summary>
        public readonly  UInt64                           Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered reset types.
        /// </summary>
        public static    IEnumerable<NTPMode>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new NTP mode based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a NTP mode.</param>
        private NTPMode(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static NTPMode Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new NTPMode(Text)
               );

        #endregion


        #region (static) Parse     (Text)

        /// <summary>
        /// Parse the given string as a NTP mode.
        /// </summary>
        /// <param name="Text">A text representation of a NTP mode.</param>
        public static NTPMode Parse(String Text)
        {

            if (TryParse(Text, out var ntpMode))
                return ntpMode;

            throw new ArgumentException($"Invalid text representation of a NTP mode: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse  (Text)

        /// <summary>
        /// Try to parse the given text as a NTP mode.
        /// </summary>
        /// <param name="Text">A text representation of a NTP mode.</param>
        public static NTPMode? TryParse(String Text)
        {

            if (TryParse(Text, out var ntpMode))
                return ntpMode;

            return null;

        }

        #endregion

        #region (static) TryParse  (Text, out NTPMode)

        /// <summary>
        /// Try to parse the given text as a NTP mode.
        /// </summary>
        /// <param name="Text">A text representation of a NTP mode.</param>
        /// <param name="NTPMode">The parsed NTP mode.</param>
        public static Boolean TryParse (String                                      Text,
                                        [NotNullWhen(true)] out NTPMode  NTPMode)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out NTPMode))
                    NTPMode = Register(Text);

                return true;

            }

            NTPMode = default;
            return false;

        }

        #endregion

        #region (static) IsDefined (Text, out NTPMode)

        /// <summary>
        /// Check whether the given text is a defined NTP mode.
        /// </summary>
        /// <param name="Text">A text representation of a NTP mode.</param>
        /// <param name="NTPMode">The validated NTP mode.</param>
        public static Boolean IsDefined(String                                     Text,
                                       [NotNullWhen(true)] out NTPMode  NTPMode)

            => lookup.TryGetValue(Text.Trim(), out NTPMode);

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this NTP mode.
        /// </summary>
        public NTPMode Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// NTP v4
        /// </summary>
        public static NTPMode  NTPv4       { get; }
            = Register("NTPv4");

        /// <summary>
        /// NTS v4
        /// </summary>
        public static NTPMode  NTSv4       { get; }
            = Register("NTSv4");

        /// <summary>
        /// NTP v4 via TLS
        /// </summary>
        public static NTPMode  NTPv4TLS    { get; }
            = Register("NTPv4TLS");

        #endregion


        #region Operator overloading

        #region Operator == (NTPMode1, NTPMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NTPMode1">A NTP mode.</param>
        /// <param name="NTPMode2">Another NTP mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NTPMode NTPMode1,
                                           NTPMode NTPMode2)

            => NTPMode1.Equals(NTPMode2);

        #endregion

        #region Operator != (NTPMode1, NTPMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NTPMode1">A NTP mode.</param>
        /// <param name="NTPMode2">Another NTP mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NTPMode NTPMode1,
                                           NTPMode NTPMode2)

            => !NTPMode1.Equals(NTPMode2);

        #endregion

        #region Operator <  (NTPMode1, NTPMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NTPMode1">A NTP mode.</param>
        /// <param name="NTPMode2">Another NTP mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (NTPMode NTPMode1,
                                          NTPMode NTPMode2)

            => NTPMode1.CompareTo(NTPMode2) < 0;

        #endregion

        #region Operator <= (NTPMode1, NTPMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NTPMode1">A NTP mode.</param>
        /// <param name="NTPMode2">Another NTP mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (NTPMode NTPMode1,
                                           NTPMode NTPMode2)

            => NTPMode1.CompareTo(NTPMode2) <= 0;

        #endregion

        #region Operator >  (NTPMode1, NTPMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NTPMode1">A NTP mode.</param>
        /// <param name="NTPMode2">Another NTP mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (NTPMode NTPMode1,
                                          NTPMode NTPMode2)

            => NTPMode1.CompareTo(NTPMode2) > 0;

        #endregion

        #region Operator >= (NTPMode1, NTPMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NTPMode1">A NTP mode.</param>
        /// <param name="NTPMode2">Another NTP mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (NTPMode NTPMode1,
                                           NTPMode NTPMode2)

            => NTPMode1.CompareTo(NTPMode2) >= 0;

        #endregion

        #endregion

        #region IComparable<NTPMode> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two NTP mode.
        /// </summary>
        /// <param name="Object">A NTP mode to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is NTPMode ntpMode
                   ? CompareTo(ntpMode)
                   : throw new ArgumentException("The given object is not a NTP mode!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(NTPMode)

        /// <summary>
        /// Compares two NTP mode.
        /// </summary>
        /// <param name="NTPMode">A NTP mode to compare with.</param>
        public Int32 CompareTo(NTPMode NTPMode)

            => String.Compare(InternalId,
                              NTPMode.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<NTPMode> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NTP mode for equality.
        /// </summary>
        /// <param name="Object">A NTP mode to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NTPMode ntpMode &&
                   Equals(ntpMode);

        #endregion

        #region Equals(NTPMode)

        /// <summary>
        /// Compares two NTP mode for equality.
        /// </summary>
        /// <param name="NTPMode">A NTP mode to compare with.</param>
        public Boolean Equals(NTPMode NTPMode)

            => String.Equals(InternalId,
                             NTPMode.InternalId,
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
