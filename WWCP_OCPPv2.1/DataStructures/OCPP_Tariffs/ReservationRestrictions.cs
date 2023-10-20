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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for reservation restrictions.
    /// </summary>
    public static class ReservationRestrictionsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a reservation restriction.
        /// </summary>
        /// <param name="Text">A text representation of a reservation restriction.</param>
        public static ReservationRestrictions Parse(String Text)
        {

            if (TryParse(Text, out var restriction))
                return restriction;

            return ReservationRestrictions.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a reservation restriction.
        /// </summary>
        /// <param name="Text">A text representation of a reservation restriction.</param>
        public static ReservationRestrictions? TryParse(String Text)
        {

            if (TryParse(Text, out var restriction))
                return restriction;

            return null;

        }

        #endregion

        #region TryParse(Text, out ReservationRestriction)

        /// <summary>
        /// Try to parse the given text as a reservation restriction.
        /// </summary>
        /// <param name="Text">A text representation of a reservation restriction.</param>
        /// <param name="ReservationRestriction">The parsed reservation restriction.</param>
        public static Boolean TryParse(String Text, out ReservationRestrictions ReservationRestriction)
        {
            switch (Text.Trim().ToUpper())
            {

                case "RESERVATION":
                    ReservationRestriction = ReservationRestrictions.RESERVATION;
                    return true;

                case "RESERVATION_EXPIRES":
                    ReservationRestriction = ReservationRestrictions.RESERVATION_EXPIRES;
                    return true;

                default:
                    ReservationRestriction = ReservationRestrictions.Unknown;
                    return false;

            }
        }

        #endregion

        #region AsText(this ReservationRestriction)

        public static String AsText(this ReservationRestrictions ReservationRestriction)

            => ReservationRestriction switch {
                   ReservationRestrictions.RESERVATION          => "RESERVATION",
                   ReservationRestrictions.RESERVATION_EXPIRES  => "RESERVATION_EXPIRES",
                   _                                            => "unknown"
               };

        #endregion

    }


    /// <summary>
    /// Reservation restriction types.
    /// </summary>
    public enum ReservationRestrictions
    {

        /// <summary>
        /// Unknown reservation restriction.
        /// </summary>
        Unknown,

        /// <summary>
        /// Used in tariff elements to describe costs for a reservation.
        /// </summary>
        RESERVATION,

        /// <summary>
        /// Used in tariff elements to describe costs for a reservation that expires
        /// (i.e. driver does not start a charging session before expiry_date of the reservation).
        /// </summary>
        RESERVATION_EXPIRES

    }

}
