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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for measurement locations.
    /// </summary>
    public static class MeasurementLocationsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a measurement location.
        /// </summary>
        /// <param name="Text">A text representation of a measurement location.</param>
        public static MeasurementLocations Parse(String Text)
        {

            if (TryParse(Text, out var location))
                return location;

            return MeasurementLocations.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a measurement location.
        /// </summary>
        /// <param name="Text">A text representation of a measurement location.</param>
        public static MeasurementLocations? TryParse(String Text)
        {

            if (TryParse(Text, out var location))
                return location;

            return null;

        }

        #endregion

        #region TryParse(Text, out MeasurementLocation)

        /// <summary>
        /// Try to parse the given text as a measurement location.
        /// </summary>
        /// <param name="Text">A text representation of a measurement location.</param>
        /// <param name="MeasurementLocation">The parsed measurement location.</param>
        public static Boolean TryParse(String Text, out MeasurementLocations MeasurementLocation)
        {
            switch (Text.Trim())
            {

                case "Body":
                    MeasurementLocation = MeasurementLocations.Body;
                    return true;

                case "Cable":
                    MeasurementLocation = MeasurementLocations.Cable;
                    return true;

                case "EV":
                    MeasurementLocation = MeasurementLocations.EV;
                    return true;

                case "Inlet":
                    MeasurementLocation = MeasurementLocations.Inlet;
                    return true;

                case "Outlet":
                    MeasurementLocation = MeasurementLocations.Outlet;
                    return true;

                case "LocalGrid":
                    MeasurementLocation = MeasurementLocations.LocalGrid;
                    return true;

                default:
                    MeasurementLocation = MeasurementLocations.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this MeasurementLocation)

        public static String AsText(this MeasurementLocations MeasurementLocation)

            => MeasurementLocation switch {
                   MeasurementLocations.Body       => "Body",
                   MeasurementLocations.Cable      => "Cable",
                   MeasurementLocations.EV         => "EV",
                   MeasurementLocations.Inlet      => "Inlet",
                   MeasurementLocations.Outlet     => "Outlet",
                   MeasurementLocations.LocalGrid  => "LocalGrid",
                   _                               => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Measurement locations.
    /// </summary>
    public enum MeasurementLocations
    {

        /// <summary>
        /// Unknown charging state.
        /// </summary>
        Unknown,

        /// <summary>
        /// Body
        /// </summary>
        Body,

        /// <summary>
        /// Cable
        /// </summary>
        Cable,

        /// <summary>
        /// EV
        /// </summary>
        EV,

        /// <summary>
        /// Inlet
        /// </summary>
        Inlet,

        /// <summary>
        /// Outlet
        /// </summary>
        Outlet,

        /// <summary>
        /// Local grid meter.
        /// </summary>
        LocalGrid

    }

}
