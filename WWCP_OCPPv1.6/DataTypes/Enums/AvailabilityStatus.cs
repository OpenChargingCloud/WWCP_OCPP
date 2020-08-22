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

namespace cloud.charging.adapters.OCPPv1_6
{

    /// <summary>
    /// Extentions methods for the availability status.
    /// </summary>
    public static class AvailabilityStatusExtentions
    {

        #region Parse(Text)

        public static AvailabilityStatus Parse(String Text)
        {

            switch (Text?.Trim())
            {

                case "Accepted":
                    return AvailabilityStatus.Accepted;

                case "Rejected":
                    return AvailabilityStatus.Rejected;

                case "Scheduled":
                    return AvailabilityStatus.Scheduled;


                default:
                    return AvailabilityStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this AvailabilityStatus)

        public static String AsText(this AvailabilityStatus AvailabilityStatus)
        {

            switch (AvailabilityStatus)
            {

                case AvailabilityStatus.Accepted:
                    return "Accepted";

                case AvailabilityStatus.Rejected:
                    return "Rejected";

                case AvailabilityStatus.Scheduled:
                    return "Scheduled";


                default:
                    return "unknown";

            }

        }

        #endregion

    }


    /// <summary>
    /// Defines the availability-status-values.
    /// </summary>
    public enum AvailabilityStatus
    {

        /// <summary>
        /// Unknown availability status.
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
        /// Request has been accepted and will be executed when
        /// transaction(s) in progress have finished.
        /// </summary>
        Scheduled

    }

}
