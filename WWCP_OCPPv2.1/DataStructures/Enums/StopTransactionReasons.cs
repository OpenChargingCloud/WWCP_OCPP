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
    /// Extension methods for stop transaction reasons.
    /// </summary>
    public static class StopTransactionReasonExtensions
    {

        /// <summary>
        /// Indicates whether this stop transaction reason is null or empty.
        /// </summary>
        /// <param name="StopTransactionReason">A stop transaction reason.</param>
        public static Boolean IsNullOrEmpty(this StopTransactionReason? StopTransactionReason)
            => !StopTransactionReason.HasValue || StopTransactionReason.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this stop transaction reason is null or empty.
        /// </summary>
        /// <param name="StopTransactionReason">A stop transaction reason.</param>
        public static Boolean IsNotNullOrEmpty(this StopTransactionReason? StopTransactionReason)
            => StopTransactionReason.HasValue && StopTransactionReason.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A stop transaction reason.
    /// </summary>
    public readonly struct StopTransactionReason : IId,
                                                   IEquatable<StopTransactionReason>,
                                                   IComparable<StopTransactionReason>
    {

        #region Data

        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this stop transaction reason is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this stop transaction reason is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the stop transaction reason.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new stop transaction reason based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a stop transaction reason.</param>
        private StopTransactionReason(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a stop transaction reason.
        /// </summary>
        /// <param name="Text">A text representation of a stop transaction reason.</param>
        public static StopTransactionReason Parse(String Text)
        {

            if (TryParse(Text, out var stopTransactionReason))
                return stopTransactionReason;

            throw new ArgumentException("The given text representation of a stop transaction reason is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as stop transaction reason.
        /// </summary>
        /// <param name="Text">A text representation of a stop transaction reason.</param>
        public static StopTransactionReason? TryParse(String Text)
        {

            if (TryParse(Text, out var stopTransactionReason))
                return stopTransactionReason;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out StopTransactionReason)

        /// <summary>
        /// Try to parse the given text as stop transaction reason.
        /// </summary>
        /// <param name="Text">A text representation of a stop transaction reason.</param>
        /// <param name="StopTransactionReason">The parsed stop transaction reason.</param>
        public static Boolean TryParse(String Text, out StopTransactionReason StopTransactionReason)
        {

            #region Initial checks

            Text = Text.Trim();

            if (Text.IsNullOrEmpty())
            {
                StopTransactionReason = default;
                return false;
            }

            #endregion

            try
            {
                StopTransactionReason = new StopTransactionReason(Text);
                return true;
            }
            catch (Exception)
            { }

            StopTransactionReason = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this stop transaction reason.
        /// </summary>
        public StopTransactionReason Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// CSMS cannot accept the requested energy transfer type or other part of the EV charging needs.
        /// </summary>
        public static StopTransactionReason ChargingNeedsNotAccepted    { get; }
            = new ("ChargingNeedsNotAccepted");

        /// <summary>
        /// The transaction was stopped because of the authorization
        /// status in a StartTransaction response.
        /// </summary>
        public static StopTransactionReason DeAuthorized                { get; }
            = new ("DeAuthorized");

        /// <summary>
        /// The emergency stop button was used.
        /// </summary>
        public static StopTransactionReason EmergencyStop               { get; }
            = new ("EmergencyStop");

        /// <summary>
        /// EV charging session reached a locally enforced maximum energy transfer limit.
        /// </summary>
        public static StopTransactionReason EnergyLimitReached          { get; }
            = new ("EnergyLimitReached");

        /// <summary>
        /// Disconnection of the cable or vehicle moved away from inductive charge unit.
        /// </summary>
        public static StopTransactionReason EVDisconnected              { get; }
            = new ("EVDisconnected");

        /// <summary>
        /// A GroundFault has occurred.
        /// </summary>
        public static StopTransactionReason GroundFault                 { get; }
            = new ("GroundFault");

        /// <summary>
        /// A Reset(Immediate) command was received.
        /// </summary>
        public static StopTransactionReason ImmediateReset              { get; }
            = new ("ImmediateReset");

        /// <summary>
        /// Stopped locally on request of the EV Driver at the charging station.
        /// This is a regular termination of a transaction.
        /// Examples: presenting an IdToken tag, pressing a button to stop.
        /// </summary>
        public static StopTransactionReason Local                       { get; }
            = new ("Local");

        /// <summary>
        /// A local credit limit enforced through the charging station has been exceeded.
        /// </summary>
        public static StopTransactionReason LocalOutOfCredit            { get; }
            = new ("LocalOutOfCredit");

        /// <summary>
        /// The transaction was stopped using a token with a MasterPassGroupId.
        /// </summary>
        public static StopTransactionReason MasterPass                  { get; }
            = new ("MasterPass");

        /// <summary>
        /// Any other reason.
        /// </summary>
        public static StopTransactionReason Other                       { get; }
            = new ("Other");

        /// <summary>
        /// A larger than intended electric current has occurred.
        /// </summary>
        public static StopTransactionReason OvercurrentFault            { get; }
            = new ("OvercurrentFault");

        /// <summary>
        /// Complete loss of power.
        /// </summary>
        public static StopTransactionReason PowerLoss                   { get; }
            = new ("PowerLoss");

        /// <summary>
        /// Quality of power too low, e.g. voltage too low/high, phase imbalance, etc
        /// </summary>
        public static StopTransactionReason PowerQuality                { get; }
            = new ("PowerQuality");

        /// <summary>
        /// A locally initiated reset/reboot occurred, e.g. the watchdog kicked in.
        /// </summary>
        public static StopTransactionReason Reboot                      { get; }
            = new ("Reboot");

        /// <summary>
        /// Stopped remotely on request of the CSMS. This is a regular termination of a transaction.
        /// Examples: termination using a smartphone app, exceeding a(non local) prepaid credit.
        /// </summary>
        public static StopTransactionReason Remote                      { get; }
            = new ("Remote");

        /// <summary>
        /// Electric vehicle has reported reaching a locally enforced maximum battery state-of-charge.
        /// </summary>
        public static StopTransactionReason SOCLimitReached             { get; }
            = new ("SOCLimitReached");

        /// <summary>
        /// The transaction was stopped by the EV.
        /// </summary>
        public static StopTransactionReason StoppedByEV                 { get; }
            = new ("StoppedByEV");

        /// <summary>
        /// EV charging session reached a locally enforced time limit.
        /// </summary>
        public static StopTransactionReason TimeLimitReached            { get; }
            = new ("TimeLimitReached");

        /// <summary>
        /// EV not connected within timeout.
        /// </summary>
        public static StopTransactionReason Timeout                     { get; }
            = new ("Timeout");

        #endregion


        #region Operator overloading

        #region Operator == (StopTransactionReason1, StopTransactionReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopTransactionReason1">A stop transaction reason.</param>
        /// <param name="StopTransactionReason2">Another stop transaction reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (StopTransactionReason StopTransactionReason1,
                                           StopTransactionReason StopTransactionReason2)

            => StopTransactionReason1.Equals(StopTransactionReason2);

        #endregion

        #region Operator != (StopTransactionReason1, StopTransactionReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopTransactionReason1">A stop transaction reason.</param>
        /// <param name="StopTransactionReason2">Another stop transaction reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (StopTransactionReason StopTransactionReason1,
                                           StopTransactionReason StopTransactionReason2)

            => !StopTransactionReason1.Equals(StopTransactionReason2);

        #endregion

        #region Operator <  (StopTransactionReason1, StopTransactionReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopTransactionReason1">A stop transaction reason.</param>
        /// <param name="StopTransactionReason2">Another stop transaction reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (StopTransactionReason StopTransactionReason1,
                                          StopTransactionReason StopTransactionReason2)

            => StopTransactionReason1.CompareTo(StopTransactionReason2) < 0;

        #endregion

        #region Operator <= (StopTransactionReason1, StopTransactionReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopTransactionReason1">A stop transaction reason.</param>
        /// <param name="StopTransactionReason2">Another stop transaction reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (StopTransactionReason StopTransactionReason1,
                                           StopTransactionReason StopTransactionReason2)

            => StopTransactionReason1.CompareTo(StopTransactionReason2) <= 0;

        #endregion

        #region Operator >  (StopTransactionReason1, StopTransactionReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopTransactionReason1">A stop transaction reason.</param>
        /// <param name="StopTransactionReason2">Another stop transaction reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (StopTransactionReason StopTransactionReason1,
                                          StopTransactionReason StopTransactionReason2)

            => StopTransactionReason1.CompareTo(StopTransactionReason2) > 0;

        #endregion

        #region Operator >= (StopTransactionReason1, StopTransactionReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopTransactionReason1">A stop transaction reason.</param>
        /// <param name="StopTransactionReason2">Another stop transaction reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (StopTransactionReason StopTransactionReason1,
                                           StopTransactionReason StopTransactionReason2)

            => StopTransactionReason1.CompareTo(StopTransactionReason2) >= 0;

        #endregion

        #endregion

        #region IComparable<StopTransactionReason> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two stop transaction reasons.
        /// </summary>
        /// <param name="Object">A stop transaction reason to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is StopTransactionReason stopTransactionReason
                   ? CompareTo(stopTransactionReason)
                   : throw new ArgumentException("The given object is not stop transaction reason!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(StopTransactionReason)

        /// <summary>
        /// Compares two stop transaction reasons.
        /// </summary>
        /// <param name="StopTransactionReason">A stop transaction reason to compare with.</param>
        public Int32 CompareTo(StopTransactionReason StopTransactionReason)

            => String.Compare(InternalId,
                              StopTransactionReason.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<StopTransactionReason> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two stop transaction reasons for equality.
        /// </summary>
        /// <param name="Object">A stop transaction reason to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StopTransactionReason stopTransactionReason &&
                   Equals(stopTransactionReason);

        #endregion

        #region Equals(StopTransactionReason)

        /// <summary>
        /// Compares two stop transaction reasons for equality.
        /// </summary>
        /// <param name="StopTransactionReason">A stop transaction reason to compare with.</param>
        public Boolean Equals(StopTransactionReason StopTransactionReason)

            => String.Equals(InternalId,
                             StopTransactionReason.InternalId,
                             StringComparison.Ordinal);

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
