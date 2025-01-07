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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for control modes.
    /// </summary>
    public static class ControlModesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a control modes.
        /// </summary>
        /// <param name="Text">A text representation of a control modes.</param>
        public static ControlModes Parse(String Text)
        {

            if (TryParse(Text, out var mode))
                return mode;

            return ControlModes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a control modes.
        /// </summary>
        /// <param name="Text">A text representation of a control modes.</param>
        public static ControlModes? TryParse(String Text)
        {

            if (TryParse(Text, out var mode))
                return mode;

            return null;

        }

        #endregion

        #region TryParse(Text, out ControlModes)

        /// <summary>
        /// Try to parse the given text as a control modes.
        /// </summary>
        /// <param name="Text">A text representation of a control modes.</param>
        /// <param name="ControlModes">The parsed control modes.</param>
        public static Boolean TryParse(String Text, out ControlModes ControlModes)
        {
            switch (Text.Trim())
            {

                case "Scheduled":
                    ControlModes = ControlModes.Scheduled;
                    return true;

                case "Dynamic":
                    ControlModes = ControlModes.Dynamic;
                    return true;

                default:
                    ControlModes = ControlModes.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this ControlModes)

        /// <summary>
        /// Return a string representation of the given control modes.
        /// </summary>
        /// <param name="ControlModes">A control modes.</param>
        public static String AsText(this ControlModes ControlModes)

            => ControlModes switch {
                   ControlModes.Scheduled  => "Scheduled",
                   ControlModes.Dynamic    => "Dynamic",
                   _                       => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Control modes.
    /// </summary>
    public enum ControlModes
    {

        /// <summary>
        /// Unknown control modes.
        /// </summary>
        Unknown,

        /// <summary>
        /// The EVSE provides up to three schedules for EV to choose from.
        /// The EV follows the selected schedule.
        /// </summary>
        Scheduled,

        /// <summary>
        /// EVSE executes a single schedule by sending setpoints to EV at every interval.
        /// </summary>
        Dynamic

    }

}
