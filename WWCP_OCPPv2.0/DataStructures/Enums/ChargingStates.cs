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

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a charging state.
        /// </summary>
        /// <param name="Text">A text representation of a charging state.</param>
        public static ChargingStates Parse(String Text)
        {

            if (TryParse(Text, out var state))
                return state;

            return ChargingStates.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charging state.
        /// </summary>
        /// <param name="Text">A text representation of a charging state.</param>
        public static ChargingStates? TryParse(String Text)
        {

            if (TryParse(Text, out var state))
                return state;

            return null;

        }

        #endregion

        #region TryParse(Text, out ChargingState)

        /// <summary>
        /// Try to parse the given text as a charging state.
        /// </summary>
        /// <param name="Text">A text representation of a charging state.</param>
        /// <param name="ChargingState">The parsed charging state.</param>
        public static Boolean TryParse(String Text, out ChargingStates ChargingState)
        {
            switch (Text.Trim())
            {

                case "Charging":
                    ChargingState = ChargingStates.Charging;
                    return true;

                case "EVConnected":
                    ChargingState = ChargingStates.EVConnected;
                    return true;

                case "SuspendedEV":
                    ChargingState = ChargingStates.SuspendedEV;
                    return true;

                case "SuspendedEVSE":
                    ChargingState = ChargingStates.SuspendedEVSE;
                    return true;

                case "Idle":
                    ChargingState = ChargingStates.Idle;
                    return true;

                default:
                    ChargingState = ChargingStates.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this ChargingState)

        public static String AsText(this ChargingStates ChargingState)

            => ChargingState switch {
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

        /// <summary>
        /// Charging
        /// </summary>
        Charging,

        /// <summary>
        /// EV connected
        /// </summary>
        EVConnected,

        /// <summary>
        /// Suspended EV
        /// </summary>
        SuspendedEV,

        /// <summary>
        /// SuspendedEVSE
        /// </summary>
        SuspendedEVSE,

        /// <summary>
        /// Idle
        /// </summary>
        Idle

    }

}
