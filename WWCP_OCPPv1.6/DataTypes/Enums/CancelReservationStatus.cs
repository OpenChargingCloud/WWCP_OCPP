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

#region Usings

using System;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extentions methods for the cancel reservation status.
    /// </summary>
    public static class CancelReservationStatusExtentions
    {

        #region Parse(Text)

        /// <summary>
        /// Parse the given string as a cancel reservation status.
        /// </summary>
        /// <param name="Text">A string representation of a cancel reservation status.</param>
        public static CancelReservationStatus Parse(String Text)
        {

            switch (Text?.ToLower())
            {

                case "accepted":
                    return CancelReservationStatus.Accepted;

                case "rejected":
                    return CancelReservationStatus.Rejected;


                default:
                    return CancelReservationStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this CancelReservationStatus)

        /// <summary>
        /// Return a string representation of the given cancel reservation status.
        /// </summary>
        /// <param name="CancelReservationStatus">A cancel reservation status.</param>
        public static String AsText(this CancelReservationStatus CancelReservationStatus)
        {

            switch (CancelReservationStatus)
            {

                case CancelReservationStatus.Accepted:
                    return "Accepted";

                case CancelReservationStatus.Rejected:
                    return "Rejected";


                default:
                    return "unknown";

            }

        }

        #endregion

    }


    /// <summary>
    /// Defines the cancel-reservation-status-values.
    /// </summary>
    public enum CancelReservationStatus
    {

        /// <summary>
        /// Unknown cancel-reservation status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Reservation for the identifier has been canceled. 
        /// </summary>
        Accepted,

        /// <summary>
        /// Reservation could not be canceled, because there
        /// is no reservation active for the identifier.
        /// </summary>
        Rejected

    }

}
