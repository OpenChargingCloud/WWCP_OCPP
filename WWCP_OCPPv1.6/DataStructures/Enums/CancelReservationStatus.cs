﻿/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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
    /// Extensions methods for the cancel reservation status.
    /// </summary>
    public static class CancelReservationStatusExtensions
    {

        #region Parse(Text)

        /// <summary>
        /// Parse the given string as a cancel reservation status.
        /// </summary>
        /// <param name="Text">A string representation of a cancel reservation status.</param>
        public static CancelReservationStatus Parse(String Text)

            => Text.ToLower() switch {
                   "Accepted"  => CancelReservationStatus.Accepted,
                   "Rejected"  => CancelReservationStatus.Rejected,
                   _           => CancelReservationStatus.Unknown
               };

        #endregion

        #region AsText(this CancelReservationStatus)

        /// <summary>
        /// Return a string representation of the given cancel reservation status.
        /// </summary>
        /// <param name="CancelReservationStatus">A cancel reservation status.</param>
        public static String AsText(this CancelReservationStatus CancelReservationStatus)

            => CancelReservationStatus switch {
                   CancelReservationStatus.Accepted  => "Accepted",
                   CancelReservationStatus.Rejected  => "Rejected",
                   _                                 => "Unknown"
               };

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
