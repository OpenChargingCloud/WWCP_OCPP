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
    /// Extensions methods for set network profile status.
    /// </summary>
    public static class SetNetworkProfileStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a set network profile status.
        /// </summary>
        /// <param name="Text">A text representation of a set network profile status.</param>
        public static SetNetworkProfileStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return SetNetworkProfileStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a set network profile status.
        /// </summary>
        /// <param name="Text">A text representation of a set network profile status.</param>
        public static SetNetworkProfileStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out SetNetworkProfileStatus)

        /// <summary>
        /// Try to parse the given text as a set network profile status.
        /// </summary>
        /// <param name="Text">A text representation of a set network profile status.</param>
        /// <param name="SetNetworkProfileStatus">The parsed set network profile status.</param>
        public static Boolean TryParse(String Text, out SetNetworkProfileStatus SetNetworkProfileStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    SetNetworkProfileStatus = SetNetworkProfileStatus.Accepted;
                    return true;

                case "Rejected":
                    SetNetworkProfileStatus = SetNetworkProfileStatus.Rejected;
                    return true;

                case "Failed":
                    SetNetworkProfileStatus = SetNetworkProfileStatus.Failed;
                    return true;

                default:
                    SetNetworkProfileStatus = SetNetworkProfileStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this SetNetworkProfileStatus)

        public static String AsText(this SetNetworkProfileStatus SetNetworkProfileStatus)

            => SetNetworkProfileStatus switch {
                   SetNetworkProfileStatus.Accepted  => "Accepted",
                   SetNetworkProfileStatus.Rejected  => "Rejected",
                   SetNetworkProfileStatus.Failed    => "Failed",
                   _                                 => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Set network profile status.
    /// </summary>
    public enum SetNetworkProfileStatus
    {

        /// <summary>
        /// Unknown set network profile status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Setting new data was successful.
        /// </summary>
        Accepted,

        /// <summary>
        /// Setting new data rejected.
        /// </summary>
        Rejected,

        /// <summary>
        /// Setting new data failed.
        /// </summary>
        Failed

    }

}
