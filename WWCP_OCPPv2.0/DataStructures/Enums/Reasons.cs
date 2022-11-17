/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extention methods for stop transaction reasons.
    /// </summary>
    public static class ReasonsExtentions
    {

        #region Parse(Text)

        public static Reasons AsReasons(String Text)

            => Text.Trim() switch {
                   "DeAuthorized"        => Reasons.DeAuthorized,
                   "EmergencyStop"       => Reasons.EmergencyStop,
                   "EnergyLimitReached"  => Reasons.EnergyLimitReached,
                   "EVDisconnected"      => Reasons.EVDisconnected,
                   "GroundFault"         => Reasons.GroundFault,
                   "ImmediateReset"      => Reasons.ImmediateReset,
                   "Local"               => Reasons.Local,
                   "LocalOutOfCredit"    => Reasons.LocalOutOfCredit,
                   "MasterPass"          => Reasons.MasterPass,
                   "Other"               => Reasons.Other,
                   "OvercurrentFault"    => Reasons.OvercurrentFault,
                   "PowerLoss"           => Reasons.PowerLoss,
                   "PowerQuality"        => Reasons.PowerQuality,
                   "Reboot"              => Reasons.Reboot,
                   "Remote"              => Reasons.Remote,
                   "SOCLimitReached"     => Reasons.SOCLimitReached,
                   "StoppedByEV"         => Reasons.StoppedByEV,
                   "TimeLimitReached"    => Reasons.TimeLimitReached,
                   "Timeout"             => Reasons.Timeout,
                   _                     => Reasons.Unknown
               };

        #endregion

        #region AsText(this Reasons)

        public static String AsText(this Reasons Reasons)

            => Reasons switch {
                   Reasons.DeAuthorized        => "DeAuthorized",
                   Reasons.EmergencyStop       => "EmergencyStop",
                   Reasons.EnergyLimitReached  => "EnergyLimitReached",
                   Reasons.EVDisconnected      => "EVDisconnected",
                   Reasons.GroundFault         => "GroundFault",
                   Reasons.ImmediateReset      => "ImmediateReset",
                   Reasons.Local               => "Local",
                   Reasons.LocalOutOfCredit    => "LocalOutOfCredit",
                   Reasons.MasterPass          => "MasterPass",
                   Reasons.Other               => "Other",
                   Reasons.OvercurrentFault    => "OvercurrentFault",
                   Reasons.PowerLoss           => "PowerLoss",
                   Reasons.PowerQuality        => "PowerQuality",
                   Reasons.Reboot              => "Reboot",
                   Reasons.Remote              => "Remote",
                   Reasons.SOCLimitReached     => "SOCLimitReached",
                   Reasons.StoppedByEV         => "StoppedByEV",
                   Reasons.TimeLimitReached    => "TimeLimitReached",
                   Reasons.Timeout             => "Timeout",
                   _                           => "unknown"
               };

        #endregion

    }


    /// <summary>
    /// The reasons for stopping a transaction.
    /// </summary>
    public enum Reasons
    {

        /// <summary>
        /// Unknown reason.
        /// </summary>
        Unknown,


        /// <summary>
        /// The transaction was stopped because of the authorization
        /// status in a StartTransaction response.
        /// </summary>
        DeAuthorized,

        /// <summary>
        /// The emergency stop button was used.
        /// </summary>
        EmergencyStop,


        EnergyLimitReached,

        /// <summary>
        /// Disconnection of the cable or vehicle moved away
        /// from inductive charge unit.
        /// </summary>
        EVDisconnected,


        GroundFault,


        ImmediateReset,


        /// <summary>
        /// Stopped locally on request of the user at the Charge Point.
        /// This is a regular termination of a transaction.
        /// </summary>
        Local,


        LocalOutOfCredit,


        MasterPass,


        /// <summary>
        /// Any other reason.
        /// </summary>
        Other,


        OvercurrentFault,


        /// <summary>
        /// Complete loss of power.
        /// </summary>
        PowerLoss,

        PowerQuality,


        /// <summary>
        /// A locally initiated reset/reboot occurred,
        /// e.g. the watchdog kicked in.
        /// </summary>
        Reboot,

        /// <summary>
        /// Stopped remotely on request of the user, e.g. via using
        /// a smartphone app or exceeding a (non local) prepaid credit.
        /// This is a regular termination of a transaction.
        /// </summary>
        Remote,


        SOCLimitReached,


        StoppedByEV,


        TimeLimitReached,


        Timeout

    }

}
