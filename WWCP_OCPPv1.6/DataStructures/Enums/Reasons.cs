/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extension methods for stop transaction reasons.
    /// </summary>
    public static class ReasonsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a stop transaction reason.
        /// </summary>
        /// <param name="Text">A text representation of a stop transaction reason.</param>
        public static Reasons Parse(String Text)
        {

            if (TryParse(Text, out var reason))
                return reason;

            return Reasons.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a stop transaction reason.
        /// </summary>
        /// <param name="Text">A text representation of a stop transaction reason.</param>
        public static Reasons? TryParse(String Text)
        {

            if (TryParse(Text, out var reason))
                return reason;

            return null;

        }

        #endregion

        #region TryParse(Text, out Reason)

        /// <summary>
        /// Try to parse the given text as a stop transaction reason.
        /// </summary>
        /// <param name="Text">A text representation of a stop transaction reason.</param>
        /// <param name="Reason">The parsed stop transaction reason.</param>
        public static Boolean TryParse(String Text, out Reasons Reason)
        {
            switch (Text.Trim())
            {

                case "EmergencyStop":
                    Reason = Reasons.EmergencyStop;
                    return true;

                case "EVDisconnected":
                    Reason = Reasons.EVDisconnected;
                    return true;

                case "HardReset":
                    Reason = Reasons.HardReset;
                    return true;

                case "Local":
                    Reason = Reasons.Local;
                    return true;

                case "Other":
                    Reason = Reasons.Other;
                    return true;

                case "PowerLoss":
                    Reason = Reasons.PowerLoss;
                    return true;

                case "Reboot":
                    Reason = Reasons.Reboot;
                    return true;

                case "Remote":
                    Reason = Reasons.Remote;
                    return true;

                case "SoftReset":
                    Reason = Reasons.SoftReset;
                    return true;

                case "UnlockCommand":
                    Reason = Reasons.UnlockCommand;
                    return true;

                case "DeAuthorized":
                    Reason = Reasons.DeAuthorized;
                    return true;

                default:
                    Reason = Reasons.Unknown;
                    return false;

            }
        }

        #endregion

        #region AsText  (this Reasons)

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
