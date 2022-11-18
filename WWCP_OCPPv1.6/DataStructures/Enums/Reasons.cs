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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extention methods for stop transaction reasons.
    /// </summary>
    public static class ReasonsExtentions
    {

        #region Parse(Text)

        public static Reasons AsReasons(String Text)

            => Text.Trim() switch {
                   "EmergencyStop"   => Reasons.EmergencyStop,
                   "EVDisconnected"  => Reasons.EVDisconnected,
                   "HardReset"       => Reasons.HardReset,
                   "Local"           => Reasons.Local,
                   "Other"           => Reasons.Other,
                   "PowerLoss"       => Reasons.PowerLoss,
                   "Reboot"          => Reasons.Reboot,
                   "Remote"          => Reasons.Remote,
                   "SoftReset"       => Reasons.SoftReset,
                   "UnlockCommand"   => Reasons.UnlockCommand,
                   "DeAuthorized"    => Reasons.DeAuthorized,
                   _                 => Reasons.Unknown
               };

        #endregion

        #region AsText(this Reasons)

        public static String AsText(this Reasons Reasons)

            => Reasons switch {
                   Reasons.EmergencyStop   => "EmergencyStop",
                   Reasons.EVDisconnected  => "EVDisconnected",
                   Reasons.HardReset       => "HardReset",
                   Reasons.Local           => "Local",
                   Reasons.Other           => "Other",
                   Reasons.PowerLoss       => "PowerLoss",
                   Reasons.Reboot          => "Reboot",
                   Reasons.Remote          => "Remote",
                   Reasons.SoftReset       => "SoftReset",
                   Reasons.UnlockCommand   => "UnlockCommand",
                   Reasons.DeAuthorized    => "DeAuthorized",
                   _                       => "Unknown"
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
        /// The emergency stop button was used.
        /// </summary>
        EmergencyStop,

        /// <summary>
        /// Disconnection of the cable or vehicle moved away
        /// from inductive charge unit.
        /// </summary>
        EVDisconnected,

        /// <summary>
        /// A hard reset command was received.
        /// </summary>
        HardReset,

        /// <summary>
        /// Stopped locally on request of the user at the Charge Point.
        /// This is a regular termination of a transaction.
        /// </summary>
        Local,

        /// <summary>
        /// Any other reason.
        /// </summary>
        Other,

        /// <summary>
        /// Complete loss of power.
        /// </summary>
        PowerLoss,

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

        /// <summary>
        /// A soft reset command was received.
        /// </summary>
        SoftReset,

        /// <summary>
        /// Central System sent an Unlock Connector command.
        /// </summary>
        UnlockCommand,

        /// <summary>
        /// The transaction was stopped because of the authorization
        /// status in a StartTransaction response.
        /// </summary>
        DeAuthorized

    }

}
