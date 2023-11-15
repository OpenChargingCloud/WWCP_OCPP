/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
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
    /// Extension methods for event notification types.
    /// </summary>
    public static class EventNotificationTypeExtensions
    {

        /// <summary>
        /// Indicates whether this event notification type is null or empty.
        /// </summary>
        /// <param name="EventNotificationType">An event notification type.</param>
        public static Boolean IsNullOrEmpty(this EventNotificationType? EventNotificationType)
            => !EventNotificationType.HasValue || EventNotificationType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this event notification type is null or empty.
        /// </summary>
        /// <param name="EventNotificationType">An event notification type.</param>
        public static Boolean IsNotNullOrEmpty(this EventNotificationType? EventNotificationType)
            => EventNotificationType.HasValue && EventNotificationType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An event notification type.
    /// </summary>
    public readonly struct EventNotificationType : IId,
                                                   IEquatable<EventNotificationType>,
                                                   IComparable<EventNotificationType>
    {

        #region Data

        private readonly static Dictionary<String, EventNotificationType>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                     InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this event notification type is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this event notification type is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the event notification type.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new event notification type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an event notification type.</param>
        private EventNotificationType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static EventNotificationType Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new EventNotificationType(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an event notification type.
        /// </summary>
        /// <param name="Text">A text representation of an event notification type.</param>
        public static EventNotificationType Parse(String Text)
        {

            if (TryParse(Text, out var eventNotificationType))
                return eventNotificationType;

            throw new ArgumentException($"Invalid text representation of an event notification type: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an event notification type.
        /// </summary>
        /// <param name="Text">A text representation of an event notification type.</param>
        public static EventNotificationType? TryParse(String Text)
        {

            if (TryParse(Text, out var eventNotificationType))
                return eventNotificationType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EventNotificationType)

        /// <summary>
        /// Try to parse the given text as an event notification type.
        /// </summary>
        /// <param name="Text">A text representation of an event notification type.</param>
        /// <param name="EventNotificationType">The parsed event notification type.</param>
        public static Boolean TryParse(String Text, out EventNotificationType EventNotificationType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out EventNotificationType))
                    EventNotificationType = Register(Text);

                return true;

            }

            EventNotificationType = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this event notification type.
        /// </summary>
        public EventNotificationType Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The software implemented by the manufacturer triggered a hardwired notification.
        /// </summary>
        public static EventNotificationType HardWiredNotification    { get; }
            = Register("HardWiredNotification");

        /// <summary>
        /// Triggered by a monitor, which is hardwired by the manufacturer.
        /// </summary>
        public static EventNotificationType HardWiredMonitor         { get; }
            = Register("HardWiredMonitor");

        /// <summary>
        /// Triggered by a monitor, which is preconfigured by the manufacturer.
        /// </summary>
        public static EventNotificationType PreconfiguredMonitor     { get; }
            = Register("PreconfiguredMonitor");

        /// <summary>
        /// Triggered by a monitor, which is set with the set variable monitoring request message by the charging station operator.
        /// </summary>
        public static EventNotificationType CustomMonitor            { get; }
            = Register("CustomMonitor");

        #endregion


        #region Operator overloading

        #region Operator == (EventNotificationType1, EventNotificationType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventNotificationType1">An event notification type.</param>
        /// <param name="EventNotificationType2">Another event notification type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EventNotificationType EventNotificationType1,
                                           EventNotificationType EventNotificationType2)

            => EventNotificationType1.Equals(EventNotificationType2);

        #endregion

        #region Operator != (EventNotificationType1, EventNotificationType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventNotificationType1">An event notification type.</param>
        /// <param name="EventNotificationType2">Another event notification type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EventNotificationType EventNotificationType1,
                                           EventNotificationType EventNotificationType2)

            => !EventNotificationType1.Equals(EventNotificationType2);

        #endregion

        #region Operator <  (EventNotificationType1, EventNotificationType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventNotificationType1">An event notification type.</param>
        /// <param name="EventNotificationType2">Another event notification type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EventNotificationType EventNotificationType1,
                                          EventNotificationType EventNotificationType2)

            => EventNotificationType1.CompareTo(EventNotificationType2) < 0;

        #endregion

        #region Operator <= (EventNotificationType1, EventNotificationType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventNotificationType1">An event notification type.</param>
        /// <param name="EventNotificationType2">Another event notification type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EventNotificationType EventNotificationType1,
                                           EventNotificationType EventNotificationType2)

            => EventNotificationType1.CompareTo(EventNotificationType2) <= 0;

        #endregion

        #region Operator >  (EventNotificationType1, EventNotificationType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventNotificationType1">An event notification type.</param>
        /// <param name="EventNotificationType2">Another event notification type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EventNotificationType EventNotificationType1,
                                          EventNotificationType EventNotificationType2)

            => EventNotificationType1.CompareTo(EventNotificationType2) > 0;

        #endregion

        #region Operator >= (EventNotificationType1, EventNotificationType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EventNotificationType1">An event notification type.</param>
        /// <param name="EventNotificationType2">Another event notification type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EventNotificationType EventNotificationType1,
                                           EventNotificationType EventNotificationType2)

            => EventNotificationType1.CompareTo(EventNotificationType2) >= 0;

        #endregion

        #endregion

        #region IComparable<EventNotificationType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two event notification types.
        /// </summary>
        /// <param name="Object">An event notification type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EventNotificationType eventNotificationType
                   ? CompareTo(eventNotificationType)
                   : throw new ArgumentException("The given object is not an event notification type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EventNotificationType)

        /// <summary>
        /// Compares two event notification types.
        /// </summary>
        /// <param name="EventNotificationType">An event notification type to compare with.</param>
        public Int32 CompareTo(EventNotificationType EventNotificationType)

            => String.Compare(InternalId,
                              EventNotificationType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<EventNotificationType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two event notification types for equality.
        /// </summary>
        /// <param name="Object">An event notification type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EventNotificationType eventNotificationType &&
                   Equals(eventNotificationType);

        #endregion

        #region Equals(EventNotificationType)

        /// <summary>
        /// Compares two event notification types for equality.
        /// </summary>
        /// <param name="EventNotificationType">An event notification type to compare with.</param>
        public Boolean Equals(EventNotificationType EventNotificationType)

            => String.Equals(InternalId,
                             EventNotificationType.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
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
