/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extentions methods for measurement locations.
    /// </summary>
    public static class MeasurementLocationsExtentions
    {

        #region Parse(Text)

        public static MeasurementLocations Parse(String Text)

            => Text.Trim() switch {
                   "Body"    => MeasurementLocations.Body,
                   "Cable"   => MeasurementLocations.Cable,
                   "EV"      => MeasurementLocations.EV,
                   "Inlet"   => MeasurementLocations.Inlet,
                   "Outlet"  => MeasurementLocations.Outlet,
                   _         => MeasurementLocations.Unknown
               };

        #endregion

        #region AsText(this MeasurementLocations)

        public static String AsText(this MeasurementLocations MeasurementLocations)

            => MeasurementLocations switch {
                   MeasurementLocations.Body    => "Body",
                   MeasurementLocations.Cable   => "Cable",
                   MeasurementLocations.EV      => "EV",
                   MeasurementLocations.Inlet   => "Inlet",
                   MeasurementLocations.Outlet  => "Outlet",
                   _                            => "Unknown"
               };

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
