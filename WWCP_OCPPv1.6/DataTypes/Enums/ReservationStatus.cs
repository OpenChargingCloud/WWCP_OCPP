/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

using System;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extentions methods for the reservation status.
    /// </summary>
    public static class ReservationStatusExtentions
    {

        #region Parse(Text)

        public static ReservationStatus Parse(String Text)
        {

            switch (Text?.Trim())
            {

                case "Accepted":
                    return ReservationStatus.Accepted;

                case "Faulted":
                    return ReservationStatus.Faulted;

                case "Occupied":
                    return ReservationStatus.Occupied;

                case "Rejected":
                    return ReservationStatus.Rejected;

                case "Unavailable":
                    return ReservationStatus.Unavailable;


                default:
                    return ReservationStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this ReservationStatus)

        public static String AsText(this ReservationStatus ReservationStatus)
        {

            switch (ReservationStatus)
            {

                case ReservationStatus.Accepted:
                    return "Accepted";

                case ReservationStatus.Faulted:
                    return "Faulted";

                case ReservationStatus.Occupied:
                    return "Occupied";

                case ReservationStatus.Rejected:
                    return "Rejected";

                case ReservationStatus.Unavailable:
                    return "Unavailable";


                default:
                    return "unknown";

            }

        }

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
        /// Reservation has not been made. Charge Point is
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
