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

namespace cloud.charging.adapters.OCPPv2_0
{

    /// <summary>
    /// Extentions methods for measurement locations.
    /// </summary>
    public static class MeasurementLocationsExtentions
    {

        #region Parse(Text)

        public static MeasurementLocations Parse(String Text)
        {

            switch (Text?.Trim())
            {

                case "Body":
                    return MeasurementLocations.Body;

                case "Cable":
                    return MeasurementLocations.Cable;

                case "EV":
                    return MeasurementLocations.EV;

                case "Inlet":
                    return MeasurementLocations.Inlet;

                case "Outlet":
                    return MeasurementLocations.Outlet;


                default:
                    return MeasurementLocations.Unknown;

            }

        }

        #endregion

        #region AsText(this MeasurementLocations)

        public static String AsText(this MeasurementLocations MeasurementLocations)
        {

            switch (MeasurementLocations)
            {

                case MeasurementLocations.Body:
                    return "Body";

                case MeasurementLocations.Cable:
                    return "Cable";

                case MeasurementLocations.EV:
                    return "EV";

                case MeasurementLocations.Inlet:
                    return "Inlet";

                case MeasurementLocations.Outlet:
                    return "Outlet";


                default:
                    return "Unknown";

            }

        }

        #endregion

    }

    /// <summary>
    /// The measurement locations.
    /// </summary>
    public enum MeasurementLocations
    {

        /// <summary>
        /// Unknown charging state.
        /// </summary>
        Unknown,

        Body,
        Cable,
        EV,
        Inlet,
        Outlet

    }

}
