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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extensions methods for cancel reservation status.
    /// </summary>
    public static class CancelReservationStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a cancel reservation status.
        /// </summary>
        /// <param name="Text">A text representation of a cancel reservation status.</param>
        public static CancelReservationStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return CancelReservationStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a cancel reservation status.
        /// </summary>
        /// <param name="Text">A text representation of a cancel reservation status.</param>
        public static CancelReservationStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out CancelReservationStatus)

        /// <summary>
        /// Try to parse the given text as a cancel reservation status.
        /// </summary>
        /// <param name="Text">A text representation of a cancel reservation status.</param>
        /// <param name="CancelReservationStatus">The parsed cancel reservation status.</param>
        public static Boolean TryParse(String Text, out CancelReservationStatus CancelReservationStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    CancelReservationStatus = CancelReservationStatus.Accepted;
                    return true;

                case "Rejected":
                    CancelReservationStatus = CancelReservationStatus.Rejected;
                    return true;

                default:
                    CancelReservationStatus = CancelReservationStatus.Unknown;
                    return false;

            }
        }

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
    /// The cancel reservation status.
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
