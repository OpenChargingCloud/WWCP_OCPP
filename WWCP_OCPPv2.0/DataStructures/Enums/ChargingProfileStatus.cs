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
    /// Extensions methods for charging profile status.
    /// </summary>
    public static class ChargingProfileStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a charging profile status.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile status.</param>
        public static ChargingProfileStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return ChargingProfileStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charging profile status.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile status.</param>
        public static ChargingProfileStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out ChargingProfileStatus)

        /// <summary>
        /// Try to parse the given text as a charging profile status.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile status.</param>
        /// <param name="ChargingProfileStatus">The parsed charging profile status.</param>
        public static Boolean TryParse(String Text, out ChargingProfileStatus ChargingProfileStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    ChargingProfileStatus = ChargingProfileStatus.Accepted;
                    return true;

                case "Rejected":
                    ChargingProfileStatus = ChargingProfileStatus.Rejected;
                    return true;

                case "NotSupported":
                    ChargingProfileStatus = ChargingProfileStatus.NotSupported;
                    return true;

                default:
                    ChargingProfileStatus = ChargingProfileStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this ChargingProfileStatus)

        public static String AsText(this ChargingProfileStatus ChargingProfileStatus)

            => ChargingProfileStatus switch {
                   ChargingProfileStatus.Accepted      => "Accepted",
                   ChargingProfileStatus.Rejected      => "Rejected",
                   ChargingProfileStatus.NotSupported  => "NotSupported",
                   _                                   => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Charging profile status.
    /// </summary>
    public enum ChargingProfileStatus
    {

        /// <summary>
        /// Unknown charging profile status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Request has been accepted and will be executed.
        /// </summary>
        Accepted,

        /// <summary>
        /// Request has not been accepted and will not be executed.
        /// </summary>
        Rejected,

        /// <summary>
        /// Charge Point indicates that the request is not supported.
        /// </summary>
        NotSupported

    }

}
