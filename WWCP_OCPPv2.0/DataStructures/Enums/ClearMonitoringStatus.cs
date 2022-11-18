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
    /// Extention methods for clear monitoring results.
    /// </summary>
    public static class ClearMonitoringStatusExtentions
    {

        #region Parse(Text)

        public static ClearMonitoringStatus Parse(this String Text)

            => Text.Trim() switch {
                   "Accepted"  => ClearMonitoringStatus.Accepted,
                   "Rejected"  => ClearMonitoringStatus.Rejected,
                   _           => ClearMonitoringStatus.NotFound
            };

        #endregion

        #region AsText(this Phase)

        public static String AsText(this ClearMonitoringStatus ClearMonitoringStatus)

            => ClearMonitoringStatus switch {
                   ClearMonitoringStatus.Accepted  => "Accepted",
                   ClearMonitoringStatus.Rejected  => "Rejected",
                   _                               => "NotFound"
            };

        #endregion

    }


    /// <summary>
    /// Clear monitoring status.
    /// </summary>
    public enum ClearMonitoringStatus
    {

        /// <summary>
        /// Monitor Id is not found.
        /// </summary>
        NotFound,

        /// <summary>
        /// Monitor successfully cleared.
        /// </summary>
        Accepted,

        /// <summary>
        /// Clearing of monitor rejected.
        /// </summary>
        Rejected

    }

}
