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
    /// Extentions methods for the reservation status.
    /// </summary>
    public static class ReservationStatusExtentions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a reservation status.
        /// </summary>
        /// <param name="Text">A text representation of a reservation status.</param>
        public static ReservationStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return ReservationStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a reservation status.
        /// </summary>
        /// <param name="Text">A text representation of a reservation status.</param>
        public static ReservationStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out ReservationStatus)

        /// <summary>
        /// Try to parse the given text as a reservation status.
        /// </summary>
        /// <param name="Text">A text representation of a reservation status.</param>
        /// <param name="ReservationStatus">The parsed reservation status.</param>
        public static Boolean TryParse(String Text, out ReservationStatus ReservationStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    ReservationStatus = ReservationStatus.Accepted;
                    return true;

                case "Faulted":
                    ReservationStatus = ReservationStatus.Faulted;
                    return true;

                case "Occupied":
                    ReservationStatus = ReservationStatus.Occupied;
                    return true;

                case "Rejected":
                    ReservationStatus = ReservationStatus.Rejected;
                    return true;

                case "Unavailable":
                    ReservationStatus = ReservationStatus.Unavailable;
                    return true;

                default:
                    ReservationStatus = ReservationStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this ReservationStatus)

        public static String AsText(this ReservationStatus ReservationStatus)

            => ReservationStatus switch {
                   ReservationStatus.Accepted     => "Accepted",
                   ReservationStatus.Faulted      => "Faulted",
                   ReservationStatus.Occupied     => "Occupied",
                   ReservationStatus.Rejected     => "Rejected",
                   ReservationStatus.Unavailable  => "Unavailable",
                   _                              => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Defines the reservation-status-values.
    /// </summary>
    public enum ReservationStatus
    {

        /// <summary>
        /// Unknown reservation status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Reservation has been made.
        /// </summary>
        Accepted,

        /// <summary>
        /// Reservation has not been made, because evse, connectors
        /// or specified connector are in a faulted state.
        /// </summary>
        Faulted,

        /// <summary>
        /// Reservation has not been made. The EVSE or
        /// the specified connector are occupied.
        /// </summary>
        Occupied,

        /// <summary>
        /// Reservation has not been made. The charging station is
        /// not configured to accept reservations.
        /// </summary>
        Rejected,

        /// <summary>
        /// Reservation has not been made, because evse, connectors
        /// or specified connector are in an unavailable state.
        /// </summary>
        Unavailable

    }

}
