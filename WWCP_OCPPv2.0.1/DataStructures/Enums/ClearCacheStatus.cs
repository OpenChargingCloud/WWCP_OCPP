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
    /// Extensions methods for clear cache status.
    /// </summary>
    public static class ClearCacheStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a clear cache status.
        /// </summary>
        /// <param name="Text">A text representation of a clear cache status.</param>
        public static ClearCacheStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return ClearCacheStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a clear cache status.
        /// </summary>
        /// <param name="Text">A text representation of a clear cache status.</param>
        public static ClearCacheStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out ClearCacheStatus)

        /// <summary>
        /// Try to parse the given text as a clear cache status.
        /// </summary>
        /// <param name="Text">A text representation of a clear cache status.</param>
        /// <param name="ClearCacheStatus">The parsed clear cache status.</param>
        public static Boolean TryParse(String Text, out ClearCacheStatus ClearCacheStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    ClearCacheStatus = ClearCacheStatus.Accepted;
                    return true;

                case "Rejected":
                    ClearCacheStatus = ClearCacheStatus.Rejected;
                    return true;

                default:
                    ClearCacheStatus = ClearCacheStatus.Unknown;
                    return false;

            }
        }

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
    /// Clear cache status.
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
