/*
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
    /// Extensions methods for the charging profile status.
    /// </summary>
    public static class ChargingProfileStatusExtensions
    {

        #region Parse(Text)

        public static ChargingProfileStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"      => ChargingProfileStatus.Accepted,
                   "Rejected"      => ChargingProfileStatus.Rejected,
                   "NotSupported"  => ChargingProfileStatus.NotSupported,
                   _               => ChargingProfileStatus.Unknown
               };

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
    /// Defines the charging-profile-status-values.
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
