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
    /// Extension methods for message formats.
    /// </summary>
    public static class MessageFormatExtensions
    {

        /// <summary>
        /// Indicates whether this message format is null or empty.
        /// </summary>
        /// <param name="MessageFormat">A message format.</param>
        public static Boolean IsNullOrEmpty(this MessageFormat? MessageFormat)
            => !MessageFormat.HasValue || MessageFormat.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this message format is null or empty.
        /// </summary>
        /// <param name="MessageFormat">A message format.</param>
        public static Boolean IsNotNullOrEmpty(this MessageFormat? MessageFormat)
            => MessageFormat.HasValue && MessageFormat.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A message format.
    /// </summary>
    public readonly struct MessageFormat : IId,
                                           IEquatable<MessageFormat>,
                                           IComparable<MessageFormat>
    {

        #region Data

        private readonly static Dictionary<String, MessageFormat>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                             InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this message format is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this message format is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the message format.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new message format based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a message format.</param>
        private MessageFormat(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static MessageFormat Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new MessageFormat(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a message format.
        /// </summary>
        /// <param name="Text">A text representation of a message format.</param>
        public static MessageFormat Parse(String Text)
        {

            if (TryParse(Text, out var bootReason))
                return bootReason;

            throw new ArgumentException($"Invalid text representation of a message format: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a message format.
        /// </summary>
        /// <param name="Text">A text representation of a message format.</param>
        public static MessageFormat? TryParse(String Text)
        {

            if (TryParse(Text, out var bootReason))
                return bootReason;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out MessageFormat)

        /// <summary>
        /// Try to parse the given text as a message format.
        /// </summary>
        /// <param name="Text">A text representation of a message format.</param>
        /// <param name="MessageFormat">The parsed message format.</param>
        public static Boolean TryParse(String Text, out MessageFormat MessageFormat)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out MessageFormat))
                    MessageFormat = Register(Text);

                return true;

            }

            MessageFormat = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this message format.
        /// </summary>
        public MessageFormat Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// ASCII
        /// </summary>
        public static MessageFormat ASCII    { get; }
            = Register("ASCII");

        /// <summary>
        /// HTML
        /// </summary>
        public static MessageFormat HTML     { get; }
            = Register("HTML");

        /// <summary>
        /// URL/URI
        /// </summary>
        public static MessageFormat URI      { get; }
            = Register("URI");

        /// <summary>
        /// UTF8
        /// </summary>
        public static MessageFormat UTF8     { get; }
            = Register("UTF8");

        #endregion


        #region Operator overloading

        #region Operator == (MessageFormat1, MessageFormat2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageFormat1">A message format.</param>
        /// <param name="MessageFormat2">Another message format.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MessageFormat MessageFormat1,
                                           MessageFormat MessageFormat2)

            => MessageFormat1.Equals(MessageFormat2);

        #endregion

        #region Operator != (MessageFormat1, MessageFormat2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageFormat1">A message format.</param>
        /// <param name="MessageFormat2">Another message format.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MessageFormat MessageFormat1,
                                           MessageFormat MessageFormat2)

            => !MessageFormat1.Equals(MessageFormat2);

        #endregion

        #region Operator <  (MessageFormat1, MessageFormat2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageFormat1">A message format.</param>
        /// <param name="MessageFormat2">Another message format.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (MessageFormat MessageFormat1,
                                          MessageFormat MessageFormat2)

            => MessageFormat1.CompareTo(MessageFormat2) < 0;

        #endregion

        #region Operator <= (MessageFormat1, MessageFormat2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageFormat1">A message format.</param>
        /// <param name="MessageFormat2">Another message format.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (MessageFormat MessageFormat1,
                                           MessageFormat MessageFormat2)

            => MessageFormat1.CompareTo(MessageFormat2) <= 0;

        #endregion

        #region Operator >  (MessageFormat1, MessageFormat2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageFormat1">A message format.</param>
        /// <param name="MessageFormat2">Another message format.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (MessageFormat MessageFormat1,
                                          MessageFormat MessageFormat2)

            => MessageFormat1.CompareTo(MessageFormat2) > 0;

        #endregion

        #region Operator >= (MessageFormat1, MessageFormat2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageFormat1">A message format.</param>
        /// <param name="MessageFormat2">Another message format.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (MessageFormat MessageFormat1,
                                           MessageFormat MessageFormat2)

            => MessageFormat1.CompareTo(MessageFormat2) >= 0;

        #endregion

        #endregion

        #region IComparable<MessageFormat> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two message formats.
        /// </summary>
        /// <param name="Object">A message format to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is MessageFormat bootReason
                   ? CompareTo(bootReason)
                   : throw new ArgumentException("The given object is not a message format!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(MessageFormat)

        /// <summary>
        /// Compares two message formats.
        /// </summary>
        /// <param name="MessageFormat">A message format to compare with.</param>
        public Int32 CompareTo(MessageFormat MessageFormat)

            => String.Compare(InternalId,
                              MessageFormat.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<MessageFormat> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two message formats for equality.
        /// </summary>
        /// <param name="Object">A message format to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MessageFormat bootReason &&
                   Equals(bootReason);

        #endregion

        #region Equals(MessageFormat)

        /// <summary>
        /// Compares two message formats for equality.
        /// </summary>
        /// <param name="MessageFormat">A message format to compare with.</param>
        public Boolean Equals(MessageFormat MessageFormat)

            => String.Equals(InternalId,
                             MessageFormat.InternalId,
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
