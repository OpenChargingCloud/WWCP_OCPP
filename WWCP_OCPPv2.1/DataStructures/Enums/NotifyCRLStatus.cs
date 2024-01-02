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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for notify certificate revocation list status.
    /// </summary>
    public static class NotifyCRLStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a notify certificate revocation list status.
        /// </summary>
        /// <param name="Text">A text representation of a notify certificate revocation list status.</param>
        public static NotifyCRLStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return NotifyCRLStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a notify certificate revocation list status.
        /// </summary>
        /// <param name="Text">A text representation of a notify certificate revocation list status.</param>
        public static NotifyCRLStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out NotifyCRLStatus)

        /// <summary>
        /// Try to parse the given text as a notify certificate revocation list status.
        /// </summary>
        /// <param name="Text">A text representation of a notify certificate revocation list status.</param>
        /// <param name="NotifyCRLStatus">The parsed notify certificate revocation list status.</param>
        public static Boolean TryParse(String Text, out NotifyCRLStatus NotifyCRLStatus)
        {
            switch (Text.Trim())
            {

                case "Available":
                    NotifyCRLStatus = NotifyCRLStatus.Available;
                    return true;

                case "Unavailable":
                    NotifyCRLStatus = NotifyCRLStatus.Unavailable;
                    return true;

                default:
                    NotifyCRLStatus = NotifyCRLStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this NotifyCRLStatus)

        /// <summary>
        /// Return a string representation of the given notify certificate revocation list status.
        /// </summary>
        /// <param name="NotifyCRLStatus">A notify certificate revocation list status.</param>
        public static String AsText(this NotifyCRLStatus NotifyCRLStatus)

            => NotifyCRLStatus switch {
                   NotifyCRLStatus.Available    => "Available",
                   NotifyCRLStatus.Unavailable  => "Unavailable",
                   _                            => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Notify certificate revocation list status.
    /// </summary>
    public enum NotifyCRLStatus
    {

        /// <summary>
        /// Unknown notify certificate revocation list status.
        /// </summary>
        Unknown,

        /// <summary>
        /// A certificate revocation list is available in given location.
        /// </summary>
        Available,

        /// <summary>
        /// No certificate revocation list is available.
        /// </summary>
        Unavailable

    }

}
