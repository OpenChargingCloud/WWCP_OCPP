/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extentions methods for charging states.
    /// </summary>
    public static class ChargingStatesExtentions
    {

        #region Parse(Text)

        public static ChargingStates Parse(String Text)
        {

            switch (Text?.Trim())
            {

                case "Charging":
                    return ChargingStates.Charging;

                case "EVConnected":
                    return ChargingStates.EVConnected;

                case "SuspendedEV":
                    return ChargingStates.SuspendedEV;

                case "SuspendedEVSE":
                    return ChargingStates.SuspendedEVSE;

                case "Idle":
                    return ChargingStates.Idle;


                default:
                    return ChargingStates.Unknown;

            }

        }

        #endregion

        #region AsText(this ChargingStates)

        public static String AsText(this ChargingStates ChargingStates)
        {

            switch (ChargingStates)
            {

                case ChargingStates.Charging:
                    return "Charging";

                case ChargingStates.EVConnected:
                    return "EVConnected";

                case ChargingStates.SuspendedEV:
                    return "SuspendedEV";

                case ChargingStates.SuspendedEVSE:
                    return "SuspendedEVSE";

                case ChargingStates.Idle:
                    return "Idle";


                default:
                    return "Unknown";

            }

        }

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
