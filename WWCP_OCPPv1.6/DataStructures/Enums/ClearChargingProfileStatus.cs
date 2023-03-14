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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extensions methods for the clear charging profile status.
    /// </summary>
    public static class ClearChargingProfileStatusExtensions
    {

        #region Parse(Text)

        public static ClearChargingProfileStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"  => ClearChargingProfileStatus.Accepted,
                   _           => ClearChargingProfileStatus.Unknown
               };

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
    /// Defines the clear-charging-profile-status-values.
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
