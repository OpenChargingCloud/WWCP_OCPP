/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Extensions methods for get charging profile status.
    /// </summary>
    public static class GetChargingProfileStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a get charging profile status.
        /// </summary>
        /// <param name="Text">A text representation of a get charging profile status.</param>
        public static GetChargingProfileStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return GetChargingProfileStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a get charging profile status.
        /// </summary>
        /// <param name="Text">A text representation of a get charging profile status.</param>
        public static GetChargingProfileStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out GetChargingProfileStatus)

        /// <summary>
        /// Try to parse the given text as a get charging profile status.
        /// </summary>
        /// <param name="Text">A text representation of a get charging profile status.</param>
        /// <param name="GetChargingProfileStatus">The parsed get charging profile status.</param>
        public static Boolean TryParse(String Text, out GetChargingProfileStatus GetChargingProfileStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    GetChargingProfileStatus = GetChargingProfileStatus.Accepted;
                    return true;

                case "NoProfiles":
                    GetChargingProfileStatus = GetChargingProfileStatus.NoProfiles;
                    return true;

                default:
                    GetChargingProfileStatus = GetChargingProfileStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this GetChargingProfileStatus)

        public static String AsText(this GetChargingProfileStatus GetChargingProfileStatus)

            => GetChargingProfileStatus switch {
                   GetChargingProfileStatus.Accepted    => "Accepted",
                   GetChargingProfileStatus.NoProfiles  => "NoProfiles",
                   _                                    => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Get charging profile status.
    /// </summary>
    public enum GetChargingProfileStatus
    {

        /// <summary>
        /// Unknown get charging profile status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Normal successful completion (no errors).
        /// </summary>
        Accepted,

        /// <summary>
        /// No charging profiles found that match the given information.
        /// </summary>
        NoProfiles

    }

}
