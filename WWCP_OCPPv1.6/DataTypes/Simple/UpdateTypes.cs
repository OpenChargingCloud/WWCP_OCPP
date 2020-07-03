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
    /// Extentions methods for the update types.
    /// </summary>
    public static class UpdateTypesExtentions
    {

        #region AsUpdateType(Text)

        public static UpdateTypes AsUpdateType(this String Text)
        {

            switch (Text)
            {

                case "Differential":
                    return UpdateTypes.Differential;

                case "Full":
                    return UpdateTypes.Full;


                default:
                    return UpdateTypes.Unknown;

            }

        }

        #endregion

        #region AsText(this UpdateType)

        public static String AsText(this UpdateTypes UpdateType)
        {

            switch (UpdateType)
            {

                case UpdateTypes.Differential:
                    return "Differential";

                case UpdateTypes.Full:
                    return "Full";


                default:
                    return "unknown";

            }

        }

        #endregion

    }


    /// <summary>
    /// Defines the update-type-values.
    /// </summary>
    public enum UpdateTypes
    {

        /// <summary>
        /// Unknown update type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Indicates that the current local authorization List
        /// must be updated with the values in this message.
        /// </summary>
        Differential,

        /// <summary>
        /// Indicates that the current local authorization list
        /// must be replaced by the values in this message.
        /// </summary>
        Full

    }

}
