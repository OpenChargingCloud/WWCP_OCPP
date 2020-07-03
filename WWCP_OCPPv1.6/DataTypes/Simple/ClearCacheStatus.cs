/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

#region Usings

using System;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// Extentions methods for the clear cache status.
    /// </summary>
    public static class ClearCacheStatusExtentions
    {

        #region AsClearCacheStatus(Text)

        public static ClearCacheStatus AsClearCacheStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return ClearCacheStatus.Accepted;

                case "Rejected":
                    return ClearCacheStatus.Rejected;


                default:
                    return ClearCacheStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this ClearCacheStatus)

        public static String AsText(this ClearCacheStatus ClearCacheStatus)
        {

            switch (ClearCacheStatus)
            {

                case ClearCacheStatus.Accepted:
                    return "Accepted";

                case ClearCacheStatus.Rejected:
                    return "Rejected";


                default:
                    return "unknown";

            }

        }

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
