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
    /// Extension methods for time sources.
    /// </summary>
    public static class TimeSourceExtensions
    {

        /// <summary>
        /// Indicates whether this time source is null or empty.
        /// </summary>
        /// <param name="TimeSource">A time source.</param>
        public static Boolean IsNullOrEmpty(this TimeSource? TimeSource)
            => !TimeSource.HasValue || TimeSource.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this time source is null or empty.
        /// </summary>
        /// <param name="TimeSource">A time source.</param>
        public static Boolean IsNotNullOrEmpty(this TimeSource? TimeSource)
            => TimeSource.HasValue && TimeSource.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A time source.
    /// </summary>
    public readonly struct TimeSource : IId,
                                        IEquatable<TimeSource>,
                                        IComparable<TimeSource>
    {

        #region Data

        private readonly static Dictionary<String, TimeSource>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                             InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this time source is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this time source is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the time source.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new time source based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a time source.</param>
        private TimeSource(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static TimeSource Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new TimeSource(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a time source.
        /// </summary>
        /// <param name="Text">A text representation of a time source.</param>
        public static TimeSource Parse(String Text)
        {

            if (TryParse(Text, out var timeSource))
                return timeSource;

            throw new ArgumentException("The given text representation of a time source is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as time source.
        /// </summary>
        /// <param name="Text">A text representation of a time source.</param>
        public static TimeSource? TryParse(String Text)
        {

            if (TryParse(Text, out var timeSource))
                return timeSource;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TimeSource)

        /// <summary>
        /// Try to parse the given text as time source.
        /// </summary>
        /// <param name="Text">A text representation of a time source.</param>
        /// <param name="TimeSource">The parsed time source.</param>
        public static Boolean TryParse(String Text, out TimeSource TimeSource)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out TimeSource))
                    TimeSource = Register(Text);

                return true;

            }

            TimeSource = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this time source.
        /// </summary>
        public TimeSource Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Heartbeat
        /// </summary>
        public static TimeSource  Heartbeat               { get; }
            = Register("Heartbeat");

        /// <summary>
        /// NTP
        /// </summary>
        public static TimeSource  NTP                     { get; }
            = Register("NTP");

        /// <summary>
        /// GPS
        /// </summary>
        public static TimeSource  GPS                     { get; }
            = Register("GPS");

        /// <summary>
        /// RealTimeClock
        /// </summary>
        public static TimeSource  RealTimeClock           { get; }
            = Register("RealTimeClock");

        /// <summary>
        /// MobileNetwork
        /// </summary>
        public static TimeSource  MobileNetwork           { get; }
            = Register("MobileNetwork");

        /// <summary>
        /// RadioTimeTransmitter
        /// </summary>
        public static TimeSource  RadioTimeTransmitter    { get; }
            = Register("RadioTimeTransmitter");

        #endregion


        #region Operator overloading

        #region Operator == (TimeSource1, TimeSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeSource1">A time source.</param>
        /// <param name="TimeSource2">Another time source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TimeSource TimeSource1,
                                           TimeSource TimeSource2)

            => TimeSource1.Equals(TimeSource2);

        #endregion

        #region Operator != (TimeSource1, TimeSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeSource1">A time source.</param>
        /// <param name="TimeSource2">Another time source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TimeSource TimeSource1,
                                           TimeSource TimeSource2)

            => !TimeSource1.Equals(TimeSource2);

        #endregion

        #region Operator <  (TimeSource1, TimeSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeSource1">A time source.</param>
        /// <param name="TimeSource2">Another time source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TimeSource TimeSource1,
                                          TimeSource TimeSource2)

            => TimeSource1.CompareTo(TimeSource2) < 0;

        #endregion

        #region Operator <= (TimeSource1, TimeSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeSource1">A time source.</param>
        /// <param name="TimeSource2">Another time source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TimeSource TimeSource1,
                                           TimeSource TimeSource2)

            => TimeSource1.CompareTo(TimeSource2) <= 0;

        #endregion

        #region Operator >  (TimeSource1, TimeSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeSource1">A time source.</param>
        /// <param name="TimeSource2">Another time source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TimeSource TimeSource1,
                                          TimeSource TimeSource2)

            => TimeSource1.CompareTo(TimeSource2) > 0;

        #endregion

        #region Operator >= (TimeSource1, TimeSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TimeSource1">A time source.</param>
        /// <param name="TimeSource2">Another time source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TimeSource TimeSource1,
                                           TimeSource TimeSource2)

            => TimeSource1.CompareTo(TimeSource2) >= 0;

        #endregion

        #endregion

        #region IComparable<TimeSource> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two time sources.
        /// </summary>
        /// <param name="Object">A time source to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TimeSource timeSource
                   ? CompareTo(timeSource)
                   : throw new ArgumentException("The given object is not time source!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TimeSource)

        /// <summary>
        /// Compares two time sources.
        /// </summary>
        /// <param name="TimeSource">A time source to compare with.</param>
        public Int32 CompareTo(TimeSource TimeSource)

            => String.Compare(InternalId,
                              TimeSource.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TimeSource> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two time sources for equality.
        /// </summary>
        /// <param name="Object">A time source to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TimeSource timeSource &&
                   Equals(timeSource);

        #endregion

        #region Equals(TimeSource)

        /// <summary>
        /// Compares two time sources for equality.
        /// </summary>
        /// <param name="TimeSource">A time source to compare with.</param>
        public Boolean Equals(TimeSource TimeSource)

            => String.Equals(InternalId,
                             TimeSource.InternalId,
                             StringComparison.OrdinalIgnoreCase);

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
