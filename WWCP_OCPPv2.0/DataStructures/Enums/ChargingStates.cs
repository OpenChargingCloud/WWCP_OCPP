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
    /// Extentions methods for charging states.
    /// </summary>
    public static class ChargingStatesExtentions
    {

        #region Parse(Text)

        public static ChargingStates Parse(String Text)

            => Text.Trim() switch {
                   "Charging"       => ChargingStates.Charging,
                   "EVConnected"    => ChargingStates.EVConnected,
                   "SuspendedEV"    => ChargingStates.SuspendedEV,
                   "SuspendedEVSE"  => ChargingStates.SuspendedEVSE,
                   "Idle"           => ChargingStates.Idle,
                   _                => ChargingStates.Unknown
               };

        #endregion

        #region AsText(this ChargingStates)

        public static String AsText(this ChargingStates ChargingStates)

            => ChargingStates switch {
                   ChargingStates.Charging       => "Charging",
                   ChargingStates.EVConnected    => "EVConnected",
                   ChargingStates.SuspendedEV    => "SuspendedEV",
                   ChargingStates.SuspendedEVSE  => "SuspendedEVSE",
                   ChargingStates.Idle           => "Idle",
                   _                             => "Unknown"
               };

        #endregion

    }

    /// <summary>
    /// The charging states.
    /// </summary>
    public enum ChargingStates
    {

        /// <summary>
        /// Unknown charging state.
        /// </summary>
        Unknown,

        Charging,
        EVConnected,
        SuspendedEV,
        SuspendedEVSE,
        Idle

    }

}
