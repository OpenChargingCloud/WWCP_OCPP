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
 * See the License for the specific Event governing permissions and
 * limitations under the License.
 */

#region Usings

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for event identifications.
    /// </summary>
    public static class EventIdExtensions
    {

        /// <summary>
        /// Indicates whether this event identification is null or empty.
        /// </summary>
        /// <param name="EventId">An event identification.</param>
        public static Boolean IsNullOrEmpty(this Event_Id? EventId)
            => !EventId.HasValue || EventId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this event identification is null or empty.
        /// </summary>
        /// <param name="EventId">An event identification.</param>
        public static Boolean IsNotNullOrEmpty(this Event_Id? EventId)
            => EventId.HasValue && EventId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An event identification (but in OCPP this is just an integer!).
    /// </summary>
    public readonly struct Event_Id : IId,
                                      IEquatable<Event_Id>,
                                      IComparable<Event_Id>
    {

        #region Data

        /// <summary>
        /// The nummeric value of the event identification.
        /// </summary>
        public readonly UInt64 Value;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => false;

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => true;

        /// <summary>
        /// The length of the event identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) Value.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new event identification based on the given number.
        /// </summary>
        /// <param name="Number">A numeric representation of a display message identification.</param>
        private Event_Id(UInt64 Number)
        {
            this.Value = Number;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random event identification.
        /// </summary>
        public static Event_Id NewRandom

#pragma warning disable SCS0005 // Weak random number generator.
            => new ((UInt64) Random.Shared.Next(Int32.MaxValue));
#pragma warning restore SCS0005 // Weak random number generator.

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an event identification.
        /// </summary>
        /// <param name="Text">A text representation of an event identification.</param>
        public static Event_Id Parse(String Text)
        {

            if (TryParse(Text, out var evseId))
                return evseId;

            throw new ArgumentException($"Invalid text representation of an event identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Number)

        /// <summary>
        /// Parse the given number as an event identification.
        /// </summary>
        /// <param name="Number">A numeric representation of an event identification.</param>
        public static Event_Id Parse(UInt64 Number)

            => new (Number);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an event identification.
        /// </summary>
        /// <param name="Text">A text representation of an event identification.</param>
        public static Event_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var evseId))
                return evseId;

            return null;

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as an event identification.
        /// </summary>
        /// <param name="Number">A numeric representation of an event identification.</param>
        public static Event_Id? TryParse(UInt64 Number)
        {

            if (TryParse(Number, out var evseId))
                return evseId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out EventId)

        /// <summary>
        /// Try to parse the given text as an event identification.
        /// </summary>
        /// <param name="Text">A text representation of an event identification.</param>
        /// <param name="EventId">The parsed event identification.</param>
        public static Boolean TryParse(String Text, out Event_Id EventId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                UInt64.TryParse(Text, out var number))
            {
                EventId = new Event_Id(number);
                return true;
            }

            EventId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out EventId)

        /// <summary>
        /// Try to parse the given number as an event identification.
        /// </summary>
        /// <param name="Number">A numeric representation of an event identification.</param>
        /// <param name="EventId">The parsed event identification.</param>
        public static Boolean TryParse(UInt64 Number, out Event_Id EventId)
        {

            EventId = new Event_Id(Number);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this event identification.
        /// </summary>
        public Event_Id Clone

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (EventId1, EventId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventId1">An event identification.</param>
        /// <param name="EventId2">Another event identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Event_Id EventId1,
                                           Event_Id EventId2)

            => EventId1.Equals(EventId2);

        #endregion

        #region Operator != (EventId1, EventId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventId1">An event identification.</param>
        /// <param name="EventId2">Another event identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Event_Id EventId1,
                                           Event_Id EventId2)

            => !EventId1.Equals(EventId2);

        #endregion

        #region Operator <  (EventId1, EventId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventId1">An event identification.</param>
        /// <param name="EventId2">Another event identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Event_Id EventId1,
                                          Event_Id EventId2)

            => EventId1.CompareTo(EventId2) < 0;

        #endregion

        #region Operator <= (EventId1, EventId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventId1">An event identification.</param>
        /// <param name="EventId2">Another event identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Event_Id EventId1,
                                           Event_Id EventId2)

            => EventId1.CompareTo(EventId2) <= 0;

        #endregion

        #region Operator >  (EventId1, EventId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventId1">An event identification.</param>
        /// <param name="EventId2">Another event identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Event_Id EventId1,
                                          Event_Id EventId2)

            => EventId1.CompareTo(EventId2) > 0;

        #endregion

        #region Operator >= (EventId1, EventId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventId1">An event identification.</param>
        /// <param name="EventId2">Another event identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Event_Id EventId1,
                                           Event_Id EventId2)

            => EventId1.CompareTo(EventId2) >= 0;

        #endregion

        #endregion

        #region IComparable<EventId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two event identifications.
        /// </summary>
        /// <param name="Object">An event identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Event_Id evseId
                   ? CompareTo(evseId)
                   : throw new ArgumentException("The given object is not an event identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EventId)

        /// <summary>
        /// Compares two event identifications.
        /// </summary>
        /// <param name="EventId">An event identification to compare with.</param>
        public Int32 CompareTo(Event_Id EventId)

            => Value.CompareTo(EventId.Value);

        #endregion

        #endregion

        #region IEquatable<EventId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two event identifications for equality.
        /// </summary>
        /// <param name="Object">An event identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Event_Id evseId &&
                   Equals(evseId);

        #endregion

        #region Equals(EventId)

        /// <summary>
        /// Compares two event identifications for equality.
        /// </summary>
        /// <param name="EventId">An event identification to compare with.</param>
        public Boolean Equals(Event_Id EventId)

            => Value.Equals(EventId.Value);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Value.ToString();

        #endregion

    }

}
