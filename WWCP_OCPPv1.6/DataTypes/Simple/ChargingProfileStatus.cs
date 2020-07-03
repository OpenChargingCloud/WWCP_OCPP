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
    /// Extentions methods for the charging profile status.
    /// </summary>
    public static class ChargingProfileStatusExtentions
    {

        #region AsChargingProfileStatus(Text)

        public static ChargingProfileStatus Parse(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return ChargingProfileStatus.Accepted;

                case "Rejected":
                    return ChargingProfileStatus.Rejected;

                case "NotSupported":
                    return ChargingProfileStatus.NotSupported;


                default:
                    return ChargingProfileStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this ChargingProfileStatus)

        public static String AsText(this ChargingProfileStatus ChargingProfileStatus)
        {

            switch (ChargingProfileStatus)
            {

                case ChargingProfileStatus.Accepted:
                    return "Accepted";

                case ChargingProfileStatus.Rejected:
                    return "Rejected";

                case ChargingProfileStatus.NotSupported:
                    return "NotSupported";


                default:
                    return "unknown";

            }

        }

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
