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
    /// Extentions methods for the reset status.
    /// </summary>
    public static class CancelResetStatusExtentions
    {

        #region AsResetStatus(Text)

        public static ResetStatus AsResetStatus(this String Text)
        {

            switch (Text)
            {

                case "Accepted":
                    return ResetStatus.Accepted;

                case "Rejected":
                    return ResetStatus.Rejected;


                default:
                    return ResetStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this ResetStatus)

        public static String AsText(this ResetStatus ResetStatus)
        {

            switch (ResetStatus)
            {

                case ResetStatus.Accepted:
                    return "Accepted";

                case ResetStatus.Rejected:
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
    public enum ResetStatus
    {

        /// <summary>
        /// Unknown reset status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Command will be executed.
        /// </summary>
        Accepted,

        /// <summary>
        /// Command will not be executed.
        /// </summary>
        Rejected

    }

}
