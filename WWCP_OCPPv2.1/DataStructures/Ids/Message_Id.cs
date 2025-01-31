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
    /// Extension methods for a message identification.
    /// </summary>
    public static class MessageIdExtensions
    {

        /// <summary>
        /// Indicates whether this message identification is null or empty.
        /// </summary>
        /// <param name="MessageId">A message identification.</param>
        public static Boolean IsNullOrEmpty(this Message_Id? MessageId)
            => !MessageId.HasValue || MessageId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this message identification is null or empty.
        /// </summary>
        /// <param name="MessageId">A message identification.</param>
        public static Boolean IsNotNullOrEmpty(this Message_Id? MessageId)
            => MessageId.HasValue && MessageId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A message identification.
    /// </summary>
    public readonly struct Message_Id : IId,
                                        IEquatable<Message_Id>,
                                        IComparable<Message_Id>
    {

        #region Data

        private readonly static Dictionary<String, Message_Id> lookup = new(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Properties

        public String  TextId       { get; }

        public UInt32  NumericId    { get; }


        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => TextId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => TextId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the message identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (TextId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new message identification based on the given text and optional number.
        /// </summary>
        /// <param name="Text">A text representation of a message identification.</param>
        /// <param name="NumericId">An optional numeric representation of a message identification.</param>
        private Message_Id(String  Text,
                           UInt32  NumericId   = 0)
        {

            this.TextId  = Text;
            this.NumericId   = NumericId;

        }

        #endregion


        #region (private static) Register(Text, NumericId = 0)

        private static Message_Id Register(String  Text,
                                           UInt32  NumericId   = 0)

            => lookup.AddAndReturnValue(
                   Text,
                   new Message_Id(Text, NumericId)
               );

        #endregion


        #region (static) Parse   (Text, NumericId = 0)

        /// <summary>
        /// Parse the given string as a message identification.
        /// </summary>
        /// <param name="Text">A text representation of a message identification.</param>
        public static Message_Id Parse(String  Text,
                                       UInt32  NumericId   = 0)
        {

            if (TryParse(Text, out var messageId, NumericId))
                return messageId;

            throw new ArgumentException("The given text representation of a message identification is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text, NumericId = 0)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given text as a message identification.
        /// </summary>
        /// <param name="Text">A text representation of a message identification.</param>
        public static Message_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var messageId))
                return messageId;

            return null;

        }


        /// <summary>
        /// Try to parse the given text as a message identification.
        /// </summary>
        /// <param name="Text">A text representation of a message identification.</param>
        public static Message_Id? TryParse(String  Text,
                                           UInt32  NumericId)
        {

            if (TryParse(Text, out var messageId, NumericId))
                return messageId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out MessageId, NumericId = 0)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given text as a message identification.
        /// </summary>
        /// <param name="Text">A text representation of a message identification.</param>
        /// <param name="MessageId">The parsed message identification.</param>
        public static Boolean TryParse(String          Text,
                                       out Message_Id  MessageId)

            => TryParse(Text,
                        out MessageId,
                        0);


        /// <summary>
        /// Try to parse the given text as a message identification.
        /// </summary>
        /// <param name="Text">A text representation of a message identification.</param>
        /// <param name="MessageId">The parsed message identification.</param>
        public static Boolean TryParse(String          Text,
                                       out Message_Id  MessageId,
                                       UInt32          NumericId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out MessageId))
                    MessageId = Register(Text, NumericId);

                return true;

            }

            MessageId = default;
            return false;

        }

        #endregion


        #region (static) Parse   (Number)

        /// <summary>
        /// Parse the given number as a message identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a message identification.</param>
        public static Message_Id Parse(UInt32 Number)
        {

            if (TryParse(Number, out var messageId))
                return messageId;

            throw new ArgumentException("The given numeric representation of a message identification is invalid!",
                                        nameof(Number));

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a message identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a message identification.</param>
        public static Message_Id? TryParse(UInt32 Number)
        {

            if (TryParse(Number, out var messageId))
                return messageId;

            return null;

        }

        #endregion

        #region (static) TryParse(Number, out MessageId)

        /// <summary>
        /// Try to parse the given number as a message identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a message identification.</param>
        /// <param name="MessageId">The parsed message identification.</param>
        public static Boolean TryParse(UInt32         Number,
                                       out Message_Id  MessageId)
        {

            var matches = lookup.Values.Where(messageId => messageId.NumericId == Number);

            if (matches.Any())
            {
                MessageId = matches.First();
                return true;
            }

            MessageId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this message identification.
        /// </summary>
        public Message_Id Clone()

            => new (
                   TextId.CloneString(),
                   NumericId
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Test Message Open Charge Alliance
        /// </summary>
        public static Message_Id OpenChargeAlliance_TestMessage    { get; }
            = Register("Open Charge Alliance - Test Message",  1);

        /// <summary>
        /// Test Message Open Charging Cloud
        /// </summary>
        public static Message_Id OpenChargingCloud_TestMessage     { get; }
            = Register("Open Charging Cloud - Test Message",   2);

        /// <summary>
        /// Test Message GraphDefined GmbH
        /// </summary>
        public static Message_Id GraphDefined_TestMessage          { get; }
            = Register("GraphDefined - Test Message",          3);

        #endregion


        #region Operator overloading

        #region Operator == (MessageId1, MessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageId1">A message identification.</param>
        /// <param name="MessageId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Message_Id MessageId1,
                                           Message_Id MessageId2)

            => MessageId1.Equals(MessageId2);

        #endregion

        #region Operator != (MessageId1, MessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageId1">A message identification.</param>
        /// <param name="MessageId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Message_Id MessageId1,
                                           Message_Id MessageId2)

            => !MessageId1.Equals(MessageId2);

        #endregion

        #region Operator <  (MessageId1, MessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageId1">A message identification.</param>
        /// <param name="MessageId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Message_Id MessageId1,
                                          Message_Id MessageId2)

            => MessageId1.CompareTo(MessageId2) < 0;

        #endregion

        #region Operator <= (MessageId1, MessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageId1">A message identification.</param>
        /// <param name="MessageId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Message_Id MessageId1,
                                           Message_Id MessageId2)

            => MessageId1.CompareTo(MessageId2) <= 0;

        #endregion

        #region Operator >  (MessageId1, MessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageId1">A message identification.</param>
        /// <param name="MessageId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Message_Id MessageId1,
                                          Message_Id MessageId2)

            => MessageId1.CompareTo(MessageId2) > 0;

        #endregion

        #region Operator >= (MessageId1, MessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageId1">A message identification.</param>
        /// <param name="MessageId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Message_Id MessageId1,
                                           Message_Id MessageId2)

            => MessageId1.CompareTo(MessageId2) >= 0;

        #endregion

        #endregion

        #region IComparable<MessageId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two message identifications.
        /// </summary>
        /// <param name="Object">A message identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Message_Id messageId
                   ? CompareTo(messageId)
                   : throw new ArgumentException("The given object is not a message identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(MessageId)

        /// <summary>
        /// Compares two message identifications.
        /// </summary>
        /// <param name="MessageId">A message identification to compare with.</param>
        public Int32 CompareTo(Message_Id MessageId)

            => String.Compare(TextId,
                              MessageId.TextId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<MessageId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two message identifications for equality.
        /// </summary>
        /// <param name="Object">A message identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Message_Id messageId &&
                   Equals(messageId);

        #endregion

        #region Equals(MessageId)

        /// <summary>
        /// Compares two message identifications for equality.
        /// </summary>
        /// <param name="MessageId">A message identification to compare with.</param>
        public Boolean Equals(Message_Id MessageId)

            => String.Equals(TextId,
                             MessageId.TextId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => TextId?.GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{TextId ?? ""} ({NumericId})";

        #endregion

    }

}
