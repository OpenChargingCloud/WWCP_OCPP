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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extensions methods for the reservation status.
    /// </summary>
    public static class ReservationStatusExtensions
    {

        #region Parse(Text)

        public static ReservationStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"     => ReservationStatus.Accepted,
                   "Faulted"      => ReservationStatus.Faulted,
                   "Occupied"     => ReservationStatus.Occupied,
                   "Rejected"     => ReservationStatus.Rejected,
                   "Unavailable"  => ReservationStatus.Unavailable,
                   _              => ReservationStatus.Unknown
               };

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
        /// Reservation has not been made, because connectors
        /// or specified connector are in a faulted state.
        /// </summary>
        Faulted,

        /// <summary>
        /// Reservation has not been made. All connectors or
        /// the specified connector are occupied.
        /// </summary>
        Occupied,

        /// <summary>
        /// Reservation has not been made. The charge point is
        /// not configured to accept reservations.
        /// </summary>
        Rejected,

        /// <summary>
        /// Reservation has not been made, because connectors
        /// or specified connector are in an unavailable state.
        /// </summary>
        Unavailable

    }

}
