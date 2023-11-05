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
    /// Extension methods for trigger reasons.
    /// </summary>
    public static class TriggerReasonExtensions
    {

        /// <summary>
        /// Indicates whether this trigger reason is null or empty.
        /// </summary>
        /// <param name="TriggerReason">A trigger reason.</param>
        public static Boolean IsNullOrEmpty(this TriggerReason? TriggerReason)
            => !TriggerReason.HasValue || TriggerReason.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this trigger reason is null or empty.
        /// </summary>
        /// <param name="TriggerReason">A trigger reason.</param>
        public static Boolean IsNotNullOrEmpty(this TriggerReason? TriggerReason)
            => TriggerReason.HasValue && TriggerReason.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A trigger reason.
    /// </summary>
    public readonly struct TriggerReason : IId,
                                           IEquatable<TriggerReason>,
                                           IComparable<TriggerReason>
    {

        #region Data

        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this trigger reason is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this trigger reason is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the trigger reason.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new trigger reason based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a trigger reason.</param>
        private TriggerReason(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a trigger reason.
        /// </summary>
        /// <param name="Text">A text representation of a trigger reason.</param>
        public static TriggerReason Parse(String Text)
        {

            if (TryParse(Text, out var triggerReason))
                return triggerReason;

            throw new ArgumentException("The given text representation of a trigger reason is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as trigger reason.
        /// </summary>
        /// <param name="Text">A text representation of a trigger reason.</param>
        public static TriggerReason? TryParse(String Text)
        {

            if (TryParse(Text, out var triggerReason))
                return triggerReason;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TriggerReason)

        /// <summary>
        /// Try to parse the given text as trigger reason.
        /// </summary>
        /// <param name="Text">A text representation of a trigger reason.</param>
        /// <param name="TriggerReason">The parsed trigger reason.</param>
        public static Boolean TryParse(String Text, out TriggerReason TriggerReason)
        {

            #region Initial checks

            Text = Text.Trim();

            if (Text.IsNullOrEmpty())
            {
                TriggerReason = default;
                return false;
            }

            #endregion

            try
            {
                TriggerReason = new TriggerReason(Text);
                return true;
            }
            catch (Exception)
            { }

            TriggerReason = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this trigger reason.
        /// </summary>
        public TriggerReason Clone

            => new (
                   new String(InternalId.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// An abnormal error or fault condition has occurred.
        /// </summary>
        public static TriggerReason AbnormalCondition
            => new ("AbnormalCondition");

        /// <summary>
        /// Charging is authorized, by any means. Might be an RFID, or other authorization means
        /// </summary>
        public static TriggerReason Authorized
            => new ("Authorized");

        /// <summary>
        /// Cable is plugged in and an EV is detected.
        /// </summary>
        public static TriggerReason CablePluggedIn
            => new ("CablePluggedIn");

        /// <summary>
        /// Rate of charging changed by more than LimitChangeSignificance.
        /// </summary>
        public static TriggerReason ChargingRateChanged
            => new ("ChargingRateChanged");

        /// <summary>
        /// Charging State changed.
        /// </summary>
        public static TriggerReason ChargingStateChanged
            => new ("ChargingStateChanged");

        /// <summary>
        /// The transaction was stopped because of the authorization status in the response to a transactionEventRequest.
        /// </summary>
        public static TriggerReason Deauthorized
            => new ("Deauthorized");

        /// <summary>
        /// Maximum energy of charging reached. For example: in a pre-paid charging solution.
        /// </summary>
        public static TriggerReason EnergyLimitReached
            => new ("EnergyLimitReached");

        /// <summary>
        /// Communication with EV lost, for example: cable disconnected.
        /// </summary>
        public static TriggerReason EVCommunicationLost
            => new ("EVCommunicationLost");

        /// <summary>
        /// EV not connected before the connection is timed out.
        /// </summary>
        public static TriggerReason EVConnectTimeout
            => new ("EVConnectTimeout");

        /// <summary>
        /// EV departed. For example: When a departing EV triggers a parking bay detector.
        /// </summary>
        public static TriggerReason EVDeparted
            => new ("EVDeparted");

        /// <summary>
        /// EV detected. For example: When an arriving EV triggers a parking bay detector.
        /// </summary>
        public static TriggerReason EVDetected
            => new ("EVDetected");

        /// <summary>
        /// Needed to send a clock aligned meter value.
        /// </summary>
        public static TriggerReason MeterValueClock
            => new ("MeterValueClock");

        /// <summary>
        /// Needed to send a periodic meter value.
        /// </summary>
        public static TriggerReason MeterValuePeriodic
            => new ("MeterValuePeriodic");

        /// <summary>
        /// A RequestStopTransactionRequest has been sent.
        /// </summary>
        public static TriggerReason RemoteStop
            => new ("RemoteStop");

        /// <summary>
        /// A RequestStartTransactionRequest has been sent.
        /// </summary>
        public static TriggerReason RemoteStart
            => new ("RemoteStart");

        /// <summary>
        /// CSMS sent a Reset Charging Station command.
        /// </summary>
        public static TriggerReason ResetCommand
            => new ("ResetCommand");

        /// <summary>
        /// Signed data is received from the energy meter.
        /// </summary>
        public static TriggerReason SignedDataReceived
            => new ("SignedDataReceived");

        /// <summary>
        /// An EV Driver has been authorized to stop charging. For example: By swiping an RFID card.
        /// </summary>
        public static TriggerReason StopAuthorized
            => new ("StopAuthorized");

        /// <summary>
        /// Maximum time of charging reached. For example: in a pre-paid charging solution.
        /// </summary>
        public static TriggerReason TimeLimitReached
            => new ("TimeLimitReached");

        /// <summary>
        /// Requested by the CSMS via a TriggerMessageRequest.
        /// </summary>
        public static TriggerReason Trigger
            => new ("Trigger");

        /// <summary>
        /// CSMS sent an Unlock Connector command.
        /// </summary>
        public static TriggerReason UnlockCommand
            => new ("UnlockCommand");

        /// <summary>
        /// The (V2X) operation mode in charging schedule period has changed.
        /// </summary>
        public static TriggerReason OperationModeChanged
            => new ("OperationModeChanged");

        /// <summary>
        /// The charging tariff for the transaction changed.
        /// </summary>
        public static TriggerReason TariffChanged
            => new("TariffChanged");

        /// <summary>
        /// Trigger used when TranactionEvent is sent (only) to report a running cost update.
        /// </summary>
        public static TriggerReason RunningCost
            => new ("RunningCost");

        #endregion


        #region Operator overloading

        #region Operator == (TriggerReason1, TriggerReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TriggerReason1">A trigger reason.</param>
        /// <param name="TriggerReason2">Another trigger reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TriggerReason TriggerReason1,
                                           TriggerReason TriggerReason2)

            => TriggerReason1.Equals(TriggerReason2);

        #endregion

        #region Operator != (TriggerReason1, TriggerReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TriggerReason1">A trigger reason.</param>
        /// <param name="TriggerReason2">Another trigger reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TriggerReason TriggerReason1,
                                           TriggerReason TriggerReason2)

            => !TriggerReason1.Equals(TriggerReason2);

        #endregion

        #region Operator <  (TriggerReason1, TriggerReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TriggerReason1">A trigger reason.</param>
        /// <param name="TriggerReason2">Another trigger reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TriggerReason TriggerReason1,
                                          TriggerReason TriggerReason2)

            => TriggerReason1.CompareTo(TriggerReason2) < 0;

        #endregion

        #region Operator <= (TriggerReason1, TriggerReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TriggerReason1">A trigger reason.</param>
        /// <param name="TriggerReason2">Another trigger reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TriggerReason TriggerReason1,
                                           TriggerReason TriggerReason2)

            => TriggerReason1.CompareTo(TriggerReason2) <= 0;

        #endregion

        #region Operator >  (TriggerReason1, TriggerReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TriggerReason1">A trigger reason.</param>
        /// <param name="TriggerReason2">Another trigger reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TriggerReason TriggerReason1,
                                          TriggerReason TriggerReason2)

            => TriggerReason1.CompareTo(TriggerReason2) > 0;

        #endregion

        #region Operator >= (TriggerReason1, TriggerReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TriggerReason1">A trigger reason.</param>
        /// <param name="TriggerReason2">Another trigger reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TriggerReason TriggerReason1,
                                           TriggerReason TriggerReason2)

            => TriggerReason1.CompareTo(TriggerReason2) >= 0;

        #endregion

        #endregion

        #region IComparable<TriggerReason> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two trigger reasons.
        /// </summary>
        /// <param name="Object">A trigger reason to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TriggerReason triggerReason
                   ? CompareTo(triggerReason)
                   : throw new ArgumentException("The given object is not trigger reason!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TriggerReason)

        /// <summary>
        /// Compares two trigger reasons.
        /// </summary>
        /// <param name="TriggerReason">A trigger reason to compare with.</param>
        public Int32 CompareTo(TriggerReason TriggerReason)

            => String.Compare(InternalId,
                              TriggerReason.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TriggerReason> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two trigger reasons for equality.
        /// </summary>
        /// <param name="Object">A trigger reason to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TriggerReason triggerReason &&
                   Equals(triggerReason);

        #endregion

        #region Equals(TriggerReason)

        /// <summary>
        /// Compares two trigger reasons for equality.
        /// </summary>
        /// <param name="TriggerReason">A trigger reason to compare with.</param>
        public Boolean Equals(TriggerReason TriggerReason)

            => String.Equals(InternalId,
                             TriggerReason.InternalId,
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
