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
    /// Extension methods for message priorities.
    /// </summary>
    public static class MessagePriorityExtensions
    {

        /// <summary>
        /// Indicates whether this message priority is null or empty.
        /// </summary>
        /// <param name="MessagePriority">A message priority.</param>
        public static Boolean IsNullOrEmpty(this MessagePriority? MessagePriority)
            => !MessagePriority.HasValue || MessagePriority.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this message priority is null or empty.
        /// </summary>
        /// <param name="MessagePriority">A message priority.</param>
        public static Boolean IsNotNullOrEmpty(this MessagePriority? MessagePriority)
            => MessagePriority.HasValue && MessagePriority.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A message priority.
    /// </summary>
    public readonly struct MessagePriority : IId,
                                             IEquatable<MessagePriority>,
                                             IComparable<MessagePriority>
    {

        #region Data

        private readonly static Dictionary<String, MessagePriority>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                               InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this message priority is null or empty.
        /// </summary>
        public readonly  Boolean                       IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this message priority is NOT null or empty.
        /// </summary>
        public readonly  Boolean                       IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the message priority.
        /// </summary>
        public readonly  UInt64                        Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered message priorities.
        /// </summary>
        public static    IEnumerable<MessagePriority>  Values
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new message priority based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a message priority.</param>
        private MessagePriority(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static MessagePriority Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new MessagePriority(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a message priority.
        /// </summary>
        /// <param name="Text">A text representation of a message priority.</param>
        public static MessagePriority Parse(String Text)
        {

            if (TryParse(Text, out var messagePriority))
                return messagePriority;

            throw new ArgumentException($"Invalid text representation of a message priority: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a message priority.
        /// </summary>
        /// <param name="Text">A text representation of a message priority.</param>
        public static MessagePriority? TryParse(String Text)
        {

            if (TryParse(Text, out var messagePriority))
                return messagePriority;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out MessagePriority)

        /// <summary>
        /// Try to parse the given text as a message priority.
        /// </summary>
        /// <param name="Text">A text representation of a message priority.</param>
        /// <param name="MessagePriority">The parsed message priority.</param>
        public static Boolean TryParse(String Text, out MessagePriority MessagePriority)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out MessagePriority))
                    MessagePriority = Register(Text);

                return true;

            }

            MessagePriority = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this message priority.
        /// </summary>
        public MessagePriority Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Show this message always in front. Highest priority, don’t cycle with other messages.
        /// When a newer message with this message priority is received, this message is replaced.
        /// No message of the charging station itself may override this message.
        /// </summary>
        public static MessagePriority  AlwaysFront    { get; }
            = Register("AlwaysFront");

        /// <summary>
        /// Show this message in front of the normal cycle of messages.
        /// When multiple messages with this priority have to be shown, they SHALL be cycled.
        /// </summary>
        public static MessagePriority  InFront        { get; }
            = Register("InFront");

        /// <summary>
        /// Show this message in the normal cycle of display messages.
        /// </summary>
        public static MessagePriority  NormalCycle    { get; }
            = Register("NormalCycle");

        #endregion


        #region Operator overloading

        #region Operator == (MessagePriority1, MessagePriority2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessagePriority1">A message priority.</param>
        /// <param name="MessagePriority2">Another message priority.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MessagePriority MessagePriority1,
                                           MessagePriority MessagePriority2)

            => MessagePriority1.Equals(MessagePriority2);

        #endregion

        #region Operator != (MessagePriority1, MessagePriority2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessagePriority1">A message priority.</param>
        /// <param name="MessagePriority2">Another message priority.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MessagePriority MessagePriority1,
                                           MessagePriority MessagePriority2)

            => !MessagePriority1.Equals(MessagePriority2);

        #endregion

        #region Operator <  (MessagePriority1, MessagePriority2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessagePriority1">A message priority.</param>
        /// <param name="MessagePriority2">Another message priority.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (MessagePriority MessagePriority1,
                                          MessagePriority MessagePriority2)

            => MessagePriority1.CompareTo(MessagePriority2) < 0;

        #endregion

        #region Operator <= (MessagePriority1, MessagePriority2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessagePriority1">A message priority.</param>
        /// <param name="MessagePriority2">Another message priority.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (MessagePriority MessagePriority1,
                                           MessagePriority MessagePriority2)

            => MessagePriority1.CompareTo(MessagePriority2) <= 0;

        #endregion

        #region Operator >  (MessagePriority1, MessagePriority2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessagePriority1">A message priority.</param>
        /// <param name="MessagePriority2">Another message priority.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (MessagePriority MessagePriority1,
                                          MessagePriority MessagePriority2)

            => MessagePriority1.CompareTo(MessagePriority2) > 0;

        #endregion

        #region Operator >= (MessagePriority1, MessagePriority2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessagePriority1">A message priority.</param>
        /// <param name="MessagePriority2">Another message priority.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (MessagePriority MessagePriority1,
                                           MessagePriority MessagePriority2)

            => MessagePriority1.CompareTo(MessagePriority2) >= 0;

        #endregion

        #endregion

        #region IComparable<MessagePriority> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two message priorities.
        /// </summary>
        /// <param name="Object">A message priority to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is MessagePriority messagePriority
                   ? CompareTo(messagePriority)
                   : throw new ArgumentException("The given object is not a message priority!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(MessagePriority)

        /// <summary>
        /// Compares two message priorities.
        /// </summary>
        /// <param name="MessagePriority">A message priority to compare with.</param>
        public Int32 CompareTo(MessagePriority MessagePriority)

            => String.Compare(InternalId,
                              MessagePriority.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<MessagePriority> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two message priorities for equality.
        /// </summary>
        /// <param name="Object">A message priority to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MessagePriority messagePriority &&
                   Equals(messagePriority);

        #endregion

        #region Equals(MessagePriority)

        /// <summary>
        /// Compares two message priorities for equality.
        /// </summary>
        /// <param name="MessagePriority">A message priority to compare with.</param>
        public Boolean Equals(MessagePriority MessagePriority)

            => String.Equals(InternalId,
                             MessagePriority.InternalId,
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
