/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extension methods for locations.
    /// </summary>
    public static class LocationExtensions
    {

        #region Parse(Text)

        public static Locations Parse(String Text)

            => Text.Trim() switch {
                   "Body"   => Locations.Body,
                   "Cable"  => Locations.Cable,
                   "EV"     => Locations.EV,
                   "Inlet"  => Locations.Inlet,
                   _        => Locations.Outlet
               };

        #endregion

        #region AsText(this Location)

        public static String AsText(this Locations Location)

            => Location switch {
                   Locations.Body   => "Body",
                   Locations.Cable  => "Cable",
                   Locations.EV     => "EV",
                   Locations.Inlet  => "Inlet",
                   _                => "Outlet",
               };

        #endregion

    }


    /// <summary>
    /// Allowable values of the optional "location" field of
    /// a value element in SampledValue.
    /// </summary>
    public enum Locations
    {

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
        Outlet

    }

}
