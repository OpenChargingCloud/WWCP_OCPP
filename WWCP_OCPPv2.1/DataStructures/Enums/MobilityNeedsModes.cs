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
    /// Extensions methods for mobility needs modes.
    /// </summary>
    public static class MobilityNeedsModesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a mobility needs modes.
        /// </summary>
        /// <param name="Text">A text representation of a mobility needs modes.</param>
        public static MobilityNeedsModes Parse(String Text)
        {

            if (TryParse(Text, out var mode))
                return mode;

            return MobilityNeedsModes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a mobility needs modes.
        /// </summary>
        /// <param name="Text">A text representation of a mobility needs modes.</param>
        public static MobilityNeedsModes? TryParse(String Text)
        {

            if (TryParse(Text, out var mode))
                return mode;

            return null;

        }

        #endregion

        #region TryParse(Text, out MobilityNeedsModes)

        /// <summary>
        /// Try to parse the given text as a mobility needs modes.
        /// </summary>
        /// <param name="Text">A text representation of a mobility needs modes.</param>
        /// <param name="MobilityNeedsModes">The parsed mobility needs modes.</param>
        public static Boolean TryParse(String Text, out MobilityNeedsModes MobilityNeedsModes)
        {
            switch (Text.Trim())
            {

                case "EVCC":
                    MobilityNeedsModes = MobilityNeedsModes.EVCC;
                    return true;

                case "EVCC_SECC":
                    MobilityNeedsModes = MobilityNeedsModes.EVCC_SECC;
                    return true;

                default:
                    MobilityNeedsModes = MobilityNeedsModes.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this MobilityNeedsModes)

        /// <summary>
        /// Return a string representation of the given mobility needs modes.
        /// </summary>
        /// <param name="MobilityNeedsModes">A mobility needs modes.</param>
        public static String AsText(this MobilityNeedsModes MobilityNeedsModes)

            => MobilityNeedsModes switch {
                   MobilityNeedsModes.EVCC       => "EVCC",
                   MobilityNeedsModes.EVCC_SECC  => "EVCC_SECC",
                   _                             => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Mobility needs modes.
    /// </summary>
    public enum MobilityNeedsModes
    {

        /// <summary>
        /// Unknown mobility needs modes.
        /// </summary>
        Unknown,

        /// <summary>
        /// Only EV determines min/target state-of-charge and departure time.
        /// </summary>
        EVCC,

        /// <summary>
        /// The Charging station or the CSMS may also update min/target
        /// state-of-charge and departure time.
        /// </summary>
        EVCC_SECC

    }

}
