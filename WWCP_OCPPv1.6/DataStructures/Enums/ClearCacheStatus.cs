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
    /// Extensions methods for the clear cache status.
    /// </summary>
    public static class ClearCacheStatusExtensions
    {

        #region Parse(Text)

        public static ClearCacheStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"  => ClearCacheStatus.Accepted,
                   "Rejected"  => ClearCacheStatus.Rejected,
                   _           => ClearCacheStatus.Unknown
               };

        #endregion

        #region AsText(this ClearCacheStatus)

        public static String AsText(this ClearCacheStatus ClearCacheStatus)

            => ClearCacheStatus switch {
                   ClearCacheStatus.Accepted  => "Accepted",
                   ClearCacheStatus.Rejected  => "Rejected",
                   _                          => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Defines the clear-cache-status-values.
    /// </summary>
    public enum ClearCacheStatus
    {

        /// <summary>
        /// Unknown clear-cache status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Command has been executed.
        /// </summary>
        Accepted,

        /// <summary>
        /// Command has not been executed.
        /// </summary>
        Rejected

    }

}
