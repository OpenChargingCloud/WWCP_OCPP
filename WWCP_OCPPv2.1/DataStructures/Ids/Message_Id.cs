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

        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the message identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new message identification based on the given text.
        /// </summary>
        /// <param name="Text">A text representation of a message identification.</param>
        private Message_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a message identification.
        /// </summary>
        /// <param name="Text">A text representation of a message identification.</param>
        public static Message_Id Parse(String Text)
        {

            if (TryParse(Text, out var messageId))
                return messageId;

            throw new ArgumentException("The given text representation of a message identification is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

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

        #endregion

        #region (static) TryParse(Text, out MessageId)

        /// <summary>
        /// Try to parse the given text as a message identification.
        /// </summary>
        /// <param name="Text">A text representation of a message identification.</param>
        /// <param name="MessageId">The parsed message identification.</param>
        public static Boolean TryParse(String Text, out Message_Id MessageId)
        {

            #region Initial checks

            Text = Text.Trim();

            if (Text.IsNullOrEmpty())
            {
                MessageId = default;
                return false;
            }

            #endregion

            try
            {
                MessageId = new Message_Id(Text);
                return true;
            }
            catch (Exception)
            { }

            MessageId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this message identification.
        /// </summary>
        public Message_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

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

            => String.Compare(InternalId,
                              MessageId.InternalId,
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

            => String.Equals(InternalId,
                             MessageId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

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
