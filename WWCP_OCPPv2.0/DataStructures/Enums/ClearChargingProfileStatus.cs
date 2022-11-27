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
    /// Extentions methods for clear charging profile status.
    /// </summary>
    public static class ClearChargingProfileStatusExtentions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a clear charging profile status.
        /// </summary>
        /// <param name="Text">A text representation of a clear charging profile status.</param>
        public static ClearChargingProfileStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return ClearChargingProfileStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a clear charging profile status.
        /// </summary>
        /// <param name="Text">A text representation of a clear charging profile status.</param>
        public static ClearChargingProfileStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out ClearChargingProfileStatus)

        /// <summary>
        /// Try to parse the given text as a clear charging profile status.
        /// </summary>
        /// <param name="Text">A text representation of a clear charging profile status.</param>
        /// <param name="ClearChargingProfileStatus">The parsed clear charging profile status.</param>
        public static Boolean TryParse(String Text, out ClearChargingProfileStatus ClearChargingProfileStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    ClearChargingProfileStatus = ClearChargingProfileStatus.Accepted;
                    return true;

                default:
                    ClearChargingProfileStatus = ClearChargingProfileStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this ClearChargingProfileStatus)

        public static String AsText(this ClearChargingProfileStatus ClearChargingProfileStatus)

            => ClearChargingProfileStatus switch {
                   ClearChargingProfileStatus.Accepted  => "Accepted",
                   _                                    => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Clear charging profile status.
    /// </summary>
    public enum ClearChargingProfileStatus
    {

        /// <summary>
        /// No charging profile(s) were found matching the request.
        /// </summary>
        Unknown,

        /// <summary>
        /// Request has been accepted and will be executed.
        /// </summary>
        Accepted

    }

}
