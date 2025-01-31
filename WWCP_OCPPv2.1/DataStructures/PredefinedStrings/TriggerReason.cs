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

        private readonly static Dictionary<String, TriggerReason>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                             InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this trigger reason is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this trigger reason is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the trigger reason.
        /// </summary>
        public readonly UInt64 Length
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


        #region (private static) Register(Text)

        private static TriggerReason Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new TriggerReason(Text)
               );

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

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out TriggerReason))
                    TriggerReason = Register(Text);

                return true;

            }

            TriggerReason = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this trigger reason.
        /// </summary>
        public TriggerReason Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// An abnormal error or fault condition has occurred.
        /// </summary>
        public static TriggerReason AbnormalCondition       { get; }
            = Register("AbnormalCondition");

        /// <summary>
        /// Charging is authorized, by any means. Might be an RFID, or other authorization means
        /// </summary>
        public static TriggerReason Authorized              { get; }
            = Register("Authorized");

        /// <summary>
        /// Cable is plugged in and an EV is detected.
        /// </summary>
        public static TriggerReason CablePluggedIn          { get; }
            = Register("CablePluggedIn");

        /// <summary>
        /// Rate of charging changed by more than LimitChangeSignificance.
        /// </summary>
        public static TriggerReason ChargingRateChanged     { get; }
            = Register("ChargingRateChanged");

        /// <summary>
        /// Charging State changed.
        /// </summary>
        public static TriggerReason ChargingStateChanged    { get; }
            = Register("ChargingStateChanged");

        /// <summary>
        /// The transaction was stopped because of the authorization status in the response to a transactionEventRequest.
        /// </summary>
        public static TriggerReason Deauthorized            { get; }
            = Register("Deauthorized");

        /// <summary>
        /// Maximum energy of charging reached. For example: in a pre-paid charging solution.
        /// </summary>
        public static TriggerReason EnergyLimitReached      { get; }
            = Register("EnergyLimitReached");

        /// <summary>
        /// Communication with EV lost, for example: cable disconnected.
        /// </summary>
        public static TriggerReason EVCommunicationLost     { get; }
            = Register("EVCommunicationLost");

        /// <summary>
        /// EV not connected before the connection is timed out.
        /// </summary>
        public static TriggerReason EVConnectTimeout        { get; }
            = Register("EVConnectTimeout");

        /// <summary>
        /// EV departed. For example: When a departing EV triggers a parking bay detector.
        /// </summary>
        public static TriggerReason EVDeparted              { get; }
            = Register("EVDeparted");

        /// <summary>
        /// EV detected. For example: When an arriving EV triggers a parking bay detector.
        /// </summary>
        public static TriggerReason EVDetected              { get; }
            = Register("EVDetected");

        /// <summary>
        /// Needed to send a clock aligned meter value.
        /// </summary>
        public static TriggerReason MeterValueClock         { get; }
            = Register("MeterValueClock");

        /// <summary>
        /// Needed to send a periodic meter value.
        /// </summary>
        public static TriggerReason MeterValuePeriodic      { get; }
            = Register("MeterValuePeriodic");

        /// <summary>
        /// A RequestStopTransactionRequest has been sent.
        /// </summary>
        public static TriggerReason RemoteStop              { get; }
            = Register("RemoteStop");

        /// <summary>
        /// A RequestStartTransactionRequest has been sent.
        /// </summary>
        public static TriggerReason RemoteStart             { get; }
            = Register("RemoteStart");

        /// <summary>
        /// CSMS sent a Reset Charging Station command.
        /// </summary>
        public static TriggerReason ResetCommand            { get; }
            = Register("ResetCommand");

        /// <summary>
        /// Signed data is received from the energy meter.
        /// </summary>
        public static TriggerReason SignedDataReceived      { get; }
            = Register("SignedDataReceived");

        /// <summary>
        /// An EV Driver has been authorized to stop charging. For example: By swiping an RFID card.
        /// </summary>
        public static TriggerReason StopAuthorized          { get; }
            = Register("StopAuthorized");

        /// <summary>
        /// Maximum time of charging reached. For example: in a pre-paid charging solution.
        /// </summary>
        public static TriggerReason TimeLimitReached        { get; }
            = Register("TimeLimitReached");

        /// <summary>
        /// Requested by the CSMS via a TriggerMessageRequest.
        /// </summary>
        public static TriggerReason Trigger                 { get; }
            = Register("Trigger");

        /// <summary>
        /// CSMS sent an Unlock Connector command.
        /// </summary>
        public static TriggerReason UnlockCommand           { get; }
            = Register("UnlockCommand");

        /// <summary>
        /// The (V2X) operation mode in charging schedule period has changed.
        /// </summary>
        public static TriggerReason OperationModeChanged    { get; }
            = Register("OperationModeChanged");

        /// <summary>
        /// The charging tariff for the transaction changed.
        /// </summary>
        public static TriggerReason TariffChanged           { get; }
            = Register("TariffChanged");

        /// <summary>
        /// Trigger used when TranactionEvent is sent (only) to report a running cost update.
        /// </summary>
        public static TriggerReason RunningCost             { get; }
            = Register("RunningCost");

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
