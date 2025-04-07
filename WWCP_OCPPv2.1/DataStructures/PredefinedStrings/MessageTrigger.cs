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
    /// Extension methods for message triggers.
    /// </summary>
    public static class MessageTriggerExtensions
    {

        /// <summary>
        /// Indicates whether this message trigger is null or empty.
        /// </summary>
        /// <param name="MessageTrigger">A message trigger.</param>
        public static Boolean IsNullOrEmpty(this MessageTrigger? MessageTrigger)
            => !MessageTrigger.HasValue || MessageTrigger.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this message trigger is null or empty.
        /// </summary>
        /// <param name="MessageTrigger">A message trigger.</param>
        public static Boolean IsNotNullOrEmpty(this MessageTrigger? MessageTrigger)
            => MessageTrigger.HasValue && MessageTrigger.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A message trigger.
    /// </summary>
    public readonly struct MessageTrigger : IId,
                                            IEquatable<MessageTrigger>,
                                            IComparable<MessageTrigger>
    {

        #region Data

        private readonly static Dictionary<String, MessageTrigger>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                              InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this message trigger is null or empty.
        /// </summary>
        public readonly Boolean                    IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this message trigger is NOT null or empty.
        /// </summary>
        public readonly Boolean                    IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the message trigger.
        /// </summary>
        public readonly UInt64                     Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered message triggers.
        /// </summary>
        public static IEnumerable<MessageTrigger>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new message trigger based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a message trigger.</param>
        private MessageTrigger(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static MessageTrigger Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new MessageTrigger(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a message trigger.
        /// </summary>
        /// <param name="Text">A text representation of a message trigger.</param>
        public static MessageTrigger Parse(String Text)
        {

            if (TryParse(Text, out var messageTrigger))
                return messageTrigger;

            throw new ArgumentException($"Invalid text representation of a message trigger: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as message trigger.
        /// </summary>
        /// <param name="Text">A text representation of a message trigger.</param>
        public static MessageTrigger? TryParse(String Text)
        {

            if (TryParse(Text, out var messageTrigger))
                return messageTrigger;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out MessageTrigger)

        /// <summary>
        /// Try to parse the given text as message trigger.
        /// </summary>
        /// <param name="Text">A text representation of a message trigger.</param>
        /// <param name="MessageTrigger">The parsed message trigger.</param>
        public static Boolean TryParse(String Text, out MessageTrigger MessageTrigger)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out MessageTrigger))
                    MessageTrigger = Register(Text);

                return true;

            }

            MessageTrigger = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this message trigger.
        /// </summary>
        public MessageTrigger Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// To trigger a BootNotification request
        /// </summary>
        public static MessageTrigger  BootNotification                     { get; }
            = Register("BootNotification");

        /// <summary>
        /// To trigger LogStatusNotification request
        /// </summary>
        public static MessageTrigger  LogStatusNotification                { get; }
            = Register("LogStatusNotification");

        /// <summary>
        /// To trigger a FirmwareStatusNotification request
        /// </summary>
        public static MessageTrigger  FirmwareStatusNotification           { get; }
            = Register("FirmwareStatusNotification");

        /// <summary>
        /// To trigger a Heartbeat request
        /// </summary>
        public static MessageTrigger  Heartbeat                            { get; }
            = Register("Heartbeat");

        /// <summary>
        /// To trigger a MeterValues request
        /// </summary>
        public static MessageTrigger  MeterValues                          { get; }
            = Register("MeterValues");

        /// <summary>
        /// To trigger an OCPP v2.x SignCertificate request with certificateType: ChargingStationCertificate
        /// </summary>
        public static MessageTrigger  SignChargingStationCertificate       { get; }
            = Register("SignChargingStationCertificate");

        /// <summary>
        /// To trigger a SignCertificate with typeOfCertificate: V2GCertificate
        /// </summary>
        public static MessageTrigger  SignV2GCertificate                   { get; }
            = Register("SignV2GCertificate");

        /// <summary>
        /// To trigger a SignCertificate with typeOfCertificate: V2G20Certificate
        /// </summary>
        public static MessageTrigger  SignV2G20Certificate                 { get; }
            = Register("SignV2G20Certificate");

        /// <summary>
        /// To trigger a StatusNotification request
        /// </summary>
        public static MessageTrigger  StatusNotification                   { get; }
            = Register("StatusNotification");

        /// <summary>
        /// To trigger TransactionEvents
        /// </summary>
        public static MessageTrigger  TransactionEvent                     { get; }
            = Register("TransactionEvent");

        /// <summary>
        /// To trigger a SignCertificate with typeOfCertificate: ChargingStationCertificate AND V2GCertificate
        /// </summary>
        public static MessageTrigger  SignCombinedCertificate              { get; }
            = Register("SignCombinedCertificate");

        /// <summary>
        /// To trigger PublishFirmwareStatusNotifications
        /// </summary>
        public static MessageTrigger  PublishFirmwareStatusNotification    { get; }
            = Register("PublishFirmwareStatusNotification");

        /// <summary>
        /// Custom trigger
        /// </summary>
        public static MessageTrigger  CustomTrigger                        { get; }
            = Register("CustomTrigger");

        #endregion


        #region Operator overloading

        #region Operator == (MessageTrigger1, MessageTrigger2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageTrigger1">A message trigger.</param>
        /// <param name="MessageTrigger2">Another message trigger.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MessageTrigger MessageTrigger1,
                                           MessageTrigger MessageTrigger2)

            => MessageTrigger1.Equals(MessageTrigger2);

        #endregion

        #region Operator != (MessageTrigger1, MessageTrigger2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageTrigger1">A message trigger.</param>
        /// <param name="MessageTrigger2">Another message trigger.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MessageTrigger MessageTrigger1,
                                           MessageTrigger MessageTrigger2)

            => !MessageTrigger1.Equals(MessageTrigger2);

        #endregion

        #region Operator <  (MessageTrigger1, MessageTrigger2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageTrigger1">A message trigger.</param>
        /// <param name="MessageTrigger2">Another message trigger.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (MessageTrigger MessageTrigger1,
                                          MessageTrigger MessageTrigger2)

            => MessageTrigger1.CompareTo(MessageTrigger2) < 0;

        #endregion

        #region Operator <= (MessageTrigger1, MessageTrigger2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageTrigger1">A message trigger.</param>
        /// <param name="MessageTrigger2">Another message trigger.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (MessageTrigger MessageTrigger1,
                                           MessageTrigger MessageTrigger2)

            => MessageTrigger1.CompareTo(MessageTrigger2) <= 0;

        #endregion

        #region Operator >  (MessageTrigger1, MessageTrigger2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageTrigger1">A message trigger.</param>
        /// <param name="MessageTrigger2">Another message trigger.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (MessageTrigger MessageTrigger1,
                                          MessageTrigger MessageTrigger2)

            => MessageTrigger1.CompareTo(MessageTrigger2) > 0;

        #endregion

        #region Operator >= (MessageTrigger1, MessageTrigger2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageTrigger1">A message trigger.</param>
        /// <param name="MessageTrigger2">Another message trigger.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (MessageTrigger MessageTrigger1,
                                           MessageTrigger MessageTrigger2)

            => MessageTrigger1.CompareTo(MessageTrigger2) >= 0;

        #endregion

        #endregion

        #region IComparable<MessageTrigger> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two message triggers.
        /// </summary>
        /// <param name="Object">A message trigger to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is MessageTrigger messageTrigger
                   ? CompareTo(messageTrigger)
                   : throw new ArgumentException("The given object is not message trigger!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(MessageTrigger)

        /// <summary>
        /// Compares two message triggers.
        /// </summary>
        /// <param name="MessageTrigger">A message trigger to compare with.</param>
        public Int32 CompareTo(MessageTrigger MessageTrigger)

            => String.Compare(InternalId,
                              MessageTrigger.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<MessageTrigger> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two message triggers for equality.
        /// </summary>
        /// <param name="Object">A message trigger to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MessageTrigger messageTrigger &&
                   Equals(messageTrigger);

        #endregion

        #region Equals(MessageTrigger)

        /// <summary>
        /// Compares two message triggers for equality.
        /// </summary>
        /// <param name="MessageTrigger">A message trigger to compare with.</param>
        public Boolean Equals(MessageTrigger MessageTrigger)

            => String.Equals(InternalId,
                             MessageTrigger.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
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
