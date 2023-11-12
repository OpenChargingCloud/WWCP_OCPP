/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License: Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing: software
 * distributed under the License is distributed on an "AS IS" BASIS:
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND: either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for monitor types.
    /// </summary>
    public static class MonitorTypesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a monitor type.
        /// </summary>
        /// <param name="Text">A text representation of a monitor type.</param>
        public static MonitorTypes Parse(String Text)
        {

            if (TryParse(Text, out var monitorType))
                return monitorType;

            return MonitorTypes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a monitor type.
        /// </summary>
        /// <param name="Text">A text representation of a monitor type.</param>
        public static MonitorTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var monitorType))
                return monitorType;

            return null;

        }

        #endregion

        #region TryParse(Text, out MonitorType)

        /// <summary>
        /// Try to parse the given text as a monitor type.
        /// </summary>
        /// <param name="Text">A text representation of a monitor type.</param>
        /// <param name="MonitorType">The parsed monitor type.</param>
        public static Boolean TryParse(String Text, out MonitorTypes MonitorType)
        {
            switch (Text.Trim())
            {

                case "UpperThreshold":
                    MonitorType = MonitorTypes.UpperThreshold;
                    return true;

                case "LowerThreshold":
                    MonitorType = MonitorTypes.LowerThreshold;
                    return true;

                case "Delta":
                    MonitorType = MonitorTypes.Delta;
                    return true;

                case "Periodic":
                    MonitorType = MonitorTypes.Periodic;
                    return true;

                case "PeriodicClockAligned":
                    MonitorType = MonitorTypes.PeriodicClockAligned;
                    return true;

                case "TargetDelta":
                    MonitorType = MonitorTypes.TargetDelta;
                    return true;

                case "TargetDeltaRelative":
                    MonitorType = MonitorTypes.TargetDeltaRelative;
                    return true;

                default:
                    MonitorType = MonitorTypes.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this MonitorType)

        public static String AsText(this MonitorTypes MonitorType)

            => MonitorType switch {
                   MonitorTypes.UpperThreshold        => "UpperThreshold",
                   MonitorTypes.LowerThreshold        => "LowerThreshold",
                   MonitorTypes.Delta                 => "Delta",
                   MonitorTypes.Periodic              => "Periodic",
                   MonitorTypes.PeriodicClockAligned  => "PeriodicClockAligned",
                   MonitorTypes.TargetDelta           => "TargetDelta",
                   MonitorTypes.TargetDeltaRelative   => "TargetDeltaRelative",
                   _                                  => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Monitor types.
    /// </summary>
    public enum MonitorTypes
    {

        /// <summary>
        /// Unknown monitor type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Triggers an event notice when the actual value of the variable rises above the "monitorValue".
        /// </summary>
        UpperThreshold,

        /// <summary>
        /// Triggers an event notice when the actual value of the variable drops below "monitorValue".
        /// </summary>
        LowerThreshold,

        /// <summary>
        /// Triggers an event notice when the actual value has changed more than plus or minus "monitorValue" since the
        /// time that this monitor was set or since the last time this event notice was sent, whichever was last.
        /// For variables that are not numeric, like boolean, string or enumerations, a monitor of type Delta will
        /// trigger an event notice whenever the variable changes, regardless of the value of monitorValue.
        /// </summary>
        Delta,

        /// <summary>
        /// Triggers an event notice every "monitorValue" seconds interval, starting from the time that this monitor was set.
        /// </summary>
        Periodic,

        /// <summary>
        /// Triggers an event notice every "monitorValue" seconds interval, starting from the nearest clock-aligned interval
        /// after this monitor was set.
        /// </summary>
        /// <example>A monitorValue of 900 will trigger event notices at 0, 15, 30 and 45 minutes after the hour, every hour.</example>
        PeriodicClockAligned,

        /// <summary>
        /// Triggers an event notice when the actual value differs from the target value more than plus
        /// or minus monitorValue since the time that this monitor was set or since the last time this
        /// event notice was sent, whichever was last. Behavior of this type of monitor for a variable that
        /// is not numeric, is not defined.
        /// Example: when target = 100, monitorValue = 10, then an event is triggered when actual < 90 or actual > 110.
        /// </summary>
        TargetDelta,

        /// <summary>
        /// Triggers an event notice when the actual value differs from the target value more than plus
        /// or minus (monitorValue * target value) since the time that this monitor was set or since the
        /// last time this event notice was sent, whichever was last. Behavior of this type of monitor for a
        /// variable that is not numeric, is not defined.
        /// Example: when target = 100, monitorValue = 0.1, then an event is triggered when actual < 90 or actual > 110.
        /// </summary>
        TargetDeltaRelative

    }

}
