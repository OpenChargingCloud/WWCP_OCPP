/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License: Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing: software
 * distributed under the License is distributed on an "AS IS" BASIS:
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND: either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extention methods for reservation update status types.
    /// </summary>
    public static class ReservationUpdateStatusTypesExtentions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a reservation update status type.
        /// </summary>
        /// <param name="Text">A text representation of a reservation update status type.</param>
        public static ReservationUpdateStatusTypes Parse(String Text)
        {

            if (TryParse(Text, out var reason))
                return reason;

            return ReservationUpdateStatusTypes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a reservation update status type.
        /// </summary>
        /// <param name="Text">A text representation of a reservation update status type.</param>
        public static ReservationUpdateStatusTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var reason))
                return reason;

            return null;

        }

        #endregion

        #region TryParse(Text, out ReservationUpdateStatusType)

        /// <summary>
        /// Try to parse the given text as a reservation update status type.
        /// </summary>
        /// <param name="Text">A text representation of a reservation update status type.</param>
        /// <param name="ReservationUpdateStatusType">The parsed reservation update status type.</param>
        public static Boolean TryParse(String Text, out ReservationUpdateStatusTypes ReservationUpdateStatusType)
        {
            switch (Text.Trim())
            {

                case "Expired":
                    ReservationUpdateStatusType = ReservationUpdateStatusTypes.Expired;
                    return true;

                case "Removed":
                    ReservationUpdateStatusType = ReservationUpdateStatusTypes.Removed;
                    return true;

                default:
                    ReservationUpdateStatusType = ReservationUpdateStatusTypes.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this Phase)

        public static String AsText(this ReservationUpdateStatusTypes ReservationUpdateStatusType)

            => ReservationUpdateStatusType switch {
                   ReservationUpdateStatusTypes.Expired  => "Expired",
                   ReservationUpdateStatusTypes.Removed  => "Removed",
                   _                                     => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Reservation update status types.
    /// </summary>
    public enum ReservationUpdateStatusTypes
    {

        /// <summary>
        /// Unknown reservation update status type.
        /// </summary>
        Unknown,

        /// <summary>
        /// The reservation was expired.
        /// </summary>
        Expired,

        /// <summary>
        /// The reservation was removed.
        /// </summary>
        Removed

    }

}
