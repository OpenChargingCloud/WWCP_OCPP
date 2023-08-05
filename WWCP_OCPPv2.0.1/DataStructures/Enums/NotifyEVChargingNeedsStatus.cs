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

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Extensions methods for notify EV charging needs status.
    /// </summary>
    public static class NotifyEVChargingNeedsStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a notify EV charging needs status.
        /// </summary>
        /// <param name="Text">A text representation of a notify EV charging needs status.</param>
        public static NotifyEVChargingNeedsStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return NotifyEVChargingNeedsStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a notify EV charging needs status.
        /// </summary>
        /// <param name="Text">A text representation of a notify EV charging needs status.</param>
        public static NotifyEVChargingNeedsStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out NotifyEVChargingNeedsStatus)

        /// <summary>
        /// Try to parse the given text as a notify EV charging needs status.
        /// </summary>
        /// <param name="Text">A text representation of a notify EV charging needs status.</param>
        /// <param name="NotifyEVChargingNeedsStatus">The parsed notify EV charging needs status.</param>
        public static Boolean TryParse(String Text, out NotifyEVChargingNeedsStatus NotifyEVChargingNeedsStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    NotifyEVChargingNeedsStatus = NotifyEVChargingNeedsStatus.Accepted;
                    return true;

                case "Rejected":
                    NotifyEVChargingNeedsStatus = NotifyEVChargingNeedsStatus.Rejected;
                    return true;

                case "Processing":
                    NotifyEVChargingNeedsStatus = NotifyEVChargingNeedsStatus.Processing;
                    return true;

                default:
                    NotifyEVChargingNeedsStatus = NotifyEVChargingNeedsStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this NotifyEVChargingNeedsStatus)

        /// <summary>
        /// Return a string representation of the given notify EV charging needs status.
        /// </summary>
        /// <param name="NotifyEVChargingNeedsStatus">A notify EV charging needs status.</param>
        public static String AsText(this NotifyEVChargingNeedsStatus NotifyEVChargingNeedsStatus)

            => NotifyEVChargingNeedsStatus switch {
                   NotifyEVChargingNeedsStatus.Accepted    => "Accepted",
                   NotifyEVChargingNeedsStatus.Rejected    => "Rejected",
                   NotifyEVChargingNeedsStatus.Processing  => "Processing",
                   _                                       => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Notify EV charging needs status.
    /// </summary>
    public enum NotifyEVChargingNeedsStatus
    {

        /// <summary>
        /// Unknown notify EV charging needs status.
        /// </summary>
        Unknown,

        /// <summary>
        /// A schedule will be provided momentarily.
        /// </summary>
        Accepted,

        /// <summary>
        /// Service not available.
        /// </summary>
        Rejected,

        /// <summary>
        /// The CSMS is gathering information to provide a schedule.
        /// </summary>
        Processing

    }

}
