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
    /// Extentions methods for the get composite schedule status.
    /// </summary>
    public static class GetCompositeScheduleStatusExtentions
    {

        #region Parse(Text)

        public static GetCompositeScheduleStatus Parse(String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return GetCompositeScheduleStatus.Accepted;

                case "Rejected":
                    return GetCompositeScheduleStatus.Rejected;


                default:
                    return GetCompositeScheduleStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this GetCompositeScheduleStatus)

        public static String AsText(this GetCompositeScheduleStatus GetCompositeScheduleStatus)
        {

            switch (GetCompositeScheduleStatus)
            {

                case GetCompositeScheduleStatus.Accepted:
                    return "Accepted";

                case GetCompositeScheduleStatus.Rejected:
                    return "Rejected";


                default:
                    return "unknown";

            }

        }

        #endregion

    }


    /// <summary>
    /// Defines the reset-status-values.
    /// </summary>
    public enum GetCompositeScheduleStatus
    {

        /// <summary>
        /// Unknown get composite schedule status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Request has been accepted and will be executed.
        /// </summary>
        Accepted,

        /// <summary>
        /// Request has not been accepted and will not be executed.
        /// </summary>
        Rejected

    }

}
