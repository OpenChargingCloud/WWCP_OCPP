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

        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this message trigger is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this message trigger is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the message trigger.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

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


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a message trigger.
        /// </summary>
        /// <param name="Text">A text representation of a message trigger.</param>
        public static MessageTrigger Parse(String Text)
        {

            if (TryParse(Text, out var messageTrigger))
                return messageTrigger;

            throw new ArgumentException("The given text representation of a message trigger is invalid!",
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

            #region Initial checks

            Text = Text.Trim();

            if (Text.IsNullOrEmpty())
            {
                MessageTrigger = default;
                return false;
            }

            #endregion

            try
            {
                MessageTrigger = new MessageTrigger(Text);
                return true;
            }
            catch (Exception)
            { }

            MessageTrigger = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this message trigger.
        /// </summary>
        public MessageTrigger Clone

            => new (
                   new String(InternalId.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// To trigger a BootNotification request
        /// </summary>
        public static MessageTrigger BootNotification
            => new ("BootNotification");

        /// <summary>
        /// To trigger LogStatusNotification request
        /// </summary>
        public static MessageTrigger LogStatusNotification
            => new ("LogStatusNotification");

        /// <summary>
        /// To trigger a DiagnosticsStatusNotification request
        /// </summary>
        public static MessageTrigger DiagnosticsStatusNotification
            => new ("DiagnosticsStatusNotification");

        /// <summary>
        /// To trigger a FirmwareStatusNotification request
        /// </summary>
        public static MessageTrigger FirmwareStatusNotification
            => new ("FirmwareStatusNotification");

        /// <summary>
        /// To trigger a Heartbeat request
        /// </summary>
        public static MessageTrigger Heartbeat
            => new ("Heartbeat");

        /// <summary>
        /// To trigger a MeterValues request
        /// </summary>
        public static MessageTrigger MeterValues
            => new ("MeterValues");

        /// <summary>
        /// To trigger a SignCertificate.req with certificateType: ChargingStationCertificate
        /// </summary>
        public static MessageTrigger SignChargingStationCertificate
            => new ("SignChargingStationCertificate");

        /// <summary>
        /// To trigger a SignCertificate with typeOfCertificate: V2GCertificate
        /// </summary>
        public static MessageTrigger SignV2GCertificate
            => new ("SignV2GCertificate");

        /// <summary>
        /// To trigger a StatusNotification request
        /// </summary>
        public static MessageTrigger StatusNotification
            => new ("StatusNotification");

        /// <summary>
        /// To trigger TransactionEvents
        /// </summary>
        public static MessageTrigger TransactionEvent
            => new ("TransactionEvent");

        /// <summary>
        /// To trigger a SignCertificate with typeOfCertificate: ChargingStationCertificate AND V2GCertificate
        /// </summary>
        public static MessageTrigger SignCombinedCertificate
            => new ("SignCombinedCertificate");

        /// <summary>
        /// To trigger PublishFirmwareStatusNotifications
        /// </summary>
        public static MessageTrigger PublishFirmwareStatusNotification
            => new ("PublishFirmwareStatusNotification");

        /// <summary>
        /// Custom trigger
        /// </summary>
        public static MessageTrigger CustomTrigger
            => new ("CustomTrigger");

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
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower()?.GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId;

        #endregion

    }

}
