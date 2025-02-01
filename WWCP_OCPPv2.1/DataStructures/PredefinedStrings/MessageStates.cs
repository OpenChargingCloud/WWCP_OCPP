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
    /// Extension methods for message states.
    /// </summary>
    public static class MessageStateExtensions
    {

        /// <summary>
        /// Indicates whether this message state is null or empty.
        /// </summary>
        /// <param name="MessageState">A message state.</param>
        public static Boolean IsNullOrEmpty(this MessageState? MessageState)
            => !MessageState.HasValue || MessageState.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this message state is null or empty.
        /// </summary>
        /// <param name="MessageState">A message state.</param>
        public static Boolean IsNotNullOrEmpty(this MessageState? MessageState)
            => MessageState.HasValue && MessageState.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A message state.
    /// </summary>
    public readonly struct MessageState : IId,
                                          IEquatable<MessageState>,
                                          IComparable<MessageState>
    {

        #region Data

        private readonly static Dictionary<String, MessageState>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                            InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this message state is null or empty.
        /// </summary>
        public readonly  Boolean                    IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this message state is NOT null or empty.
        /// </summary>
        public readonly  Boolean                    IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the message state.
        /// </summary>
        public readonly  UInt64                     Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered message states.
        /// </summary>
        public static    IEnumerable<MessageState>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new message state based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a message state.</param>
        private MessageState(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static MessageState Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new MessageState(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a message state.
        /// </summary>
        /// <param name="Text">A text representation of a message state.</param>
        public static MessageState Parse(String Text)
        {

            if (TryParse(Text, out var messageState))
                return messageState;

            throw new ArgumentException($"Invalid text representation of a message state: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a message state.
        /// </summary>
        /// <param name="Text">A text representation of a message state.</param>
        public static MessageState? TryParse(String Text)
        {

            if (TryParse(Text, out var messageState))
                return messageState;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out MessageState)

        /// <summary>
        /// Try to parse the given text as a message state.
        /// </summary>
        /// <param name="Text">A text representation of a message state.</param>
        /// <param name="MessageState">The parsed message state.</param>
        public static Boolean TryParse(String Text, out MessageState MessageState)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out MessageState))
                    MessageState = Register(Text);

                return true;

            }

            MessageState = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this message state.
        /// </summary>
        public MessageState Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Message only to be shown while the charging station is charging.
        /// </summary>
        public static MessageState  Charging       { get; }
            = Register("Charging");

        /// <summary>
        /// Message only to be shown while the charging station is in faulted state.
        /// </summary>
        public static MessageState  Faulted        { get; }
            = Register("Faulted");

        /// <summary>
        /// Message only to be shown while the charging station is idle (not charging).
        /// </summary>
        public static MessageState  Idle           { get; }
            = Register("Idle");

        /// <summary>
        /// Message only to be shown while the charging station is in unavailable state.
        /// </summary>
        public static MessageState  Unavailable    { get; }
            = Register("Unavailable");

        /// <summary>
        /// Message only to be shown while the charging station is in suspended state.
        /// </summary>
        public static MessageState  Suspended      { get; }
            = Register("Suspended");

        /// <summary>
        /// Message only to be shown while the charging station is in discharging state.
        /// </summary>
        public static MessageState  Discharging    { get; }
            = Register("Discharging");

        #endregion


        #region Operator overloading

        #region Operator == (MessageState1, MessageState2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageState1">A message state.</param>
        /// <param name="MessageState2">Another message state.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MessageState MessageState1,
                                           MessageState MessageState2)

            => MessageState1.Equals(MessageState2);

        #endregion

        #region Operator != (MessageState1, MessageState2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageState1">A message state.</param>
        /// <param name="MessageState2">Another message state.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MessageState MessageState1,
                                           MessageState MessageState2)

            => !MessageState1.Equals(MessageState2);

        #endregion

        #region Operator <  (MessageState1, MessageState2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageState1">A message state.</param>
        /// <param name="MessageState2">Another message state.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (MessageState MessageState1,
                                          MessageState MessageState2)

            => MessageState1.CompareTo(MessageState2) < 0;

        #endregion

        #region Operator <= (MessageState1, MessageState2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageState1">A message state.</param>
        /// <param name="MessageState2">Another message state.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (MessageState MessageState1,
                                           MessageState MessageState2)

            => MessageState1.CompareTo(MessageState2) <= 0;

        #endregion

        #region Operator >  (MessageState1, MessageState2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageState1">A message state.</param>
        /// <param name="MessageState2">Another message state.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (MessageState MessageState1,
                                          MessageState MessageState2)

            => MessageState1.CompareTo(MessageState2) > 0;

        #endregion

        #region Operator >= (MessageState1, MessageState2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageState1">A message state.</param>
        /// <param name="MessageState2">Another message state.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (MessageState MessageState1,
                                           MessageState MessageState2)

            => MessageState1.CompareTo(MessageState2) >= 0;

        #endregion

        #endregion

        #region IComparable<MessageState> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two message states.
        /// </summary>
        /// <param name="Object">A message state to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is MessageState messageState
                   ? CompareTo(messageState)
                   : throw new ArgumentException("The given object is not a message state!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(MessageState)

        /// <summary>
        /// Compares two message states.
        /// </summary>
        /// <param name="MessageState">A message state to compare with.</param>
        public Int32 CompareTo(MessageState MessageState)

            => String.Compare(InternalId,
                              MessageState.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<MessageState> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two message states for equality.
        /// </summary>
        /// <param name="Object">A message state to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MessageState messageState &&
                   Equals(messageState);

        #endregion

        #region Equals(MessageState)

        /// <summary>
        /// Compares two message states for equality.
        /// </summary>
        /// <param name="MessageState">A message state to compare with.</param>
        public Boolean Equals(MessageState MessageState)

            => String.Equals(InternalId,
                             MessageState.InternalId,
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
