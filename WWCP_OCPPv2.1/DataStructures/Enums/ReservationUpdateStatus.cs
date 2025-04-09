/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License: Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing: software
 * distributed under the License is distributed on an "AS IS" BASIS:
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND: either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for reservation update status.
    /// </summary>
    public static class ReservationUpdateStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a reservation update status.
        /// </summary>
        /// <param name="Text">A text representation of a reservation update status.</param>
        public static ReservationUpdateStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return ReservationUpdateStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a reservation update status.
        /// </summary>
        /// <param name="Text">A text representation of a reservation update status.</param>
        public static ReservationUpdateStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out ReservationUpdateStatus)

        /// <summary>
        /// Try to parse the given text as a reservation update status.
        /// </summary>
        /// <param name="Text">A text representation of a reservation update status.</param>
        /// <param name="ReservationUpdateStatus">The parsed reservation update status.</param>
        public static Boolean TryParse(String Text, out ReservationUpdateStatus ReservationUpdateStatus)
        {
            switch (Text.Trim())
            {

                case "Expired":
                    ReservationUpdateStatus = ReservationUpdateStatus.Expired;
                    return true;

                case "Removed":
                    ReservationUpdateStatus = ReservationUpdateStatus.Removed;
                    return true;

                default:
                    ReservationUpdateStatus = ReservationUpdateStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this ReservationUpdateStatus)

        public static String AsText(this ReservationUpdateStatus ReservationUpdateStatus)

            => ReservationUpdateStatus switch {
                   ReservationUpdateStatus.Expired  => "Expired",
                   ReservationUpdateStatus.Removed  => "Removed",
                   _                                => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Reservation update status.
    /// </summary>
    public enum ReservationUpdateStatus
    {

        /// <summary>
        /// Unknown reservation update status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The reservation was expired.
        /// </summary>
        Expired,

        /// <summary>
        /// The reservation was removed.
        /// </summary>
        Removed,

        /// <summary>
        /// The reservation was used, but no transaction was started.
        /// </summary>
        NoTransaction

    }

}
