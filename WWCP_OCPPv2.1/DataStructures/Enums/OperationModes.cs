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
    /// Extensions methods for operation modes.
    /// </summary>
    public static class OperationModesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a operation modes.
        /// </summary>
        /// <param name="Text">A text representation of a operation modes.</param>
        public static OperationModes Parse(String Text)
        {

            if (TryParse(Text, out var mode))
                return mode;

            return OperationModes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a operation modes.
        /// </summary>
        /// <param name="Text">A text representation of a operation modes.</param>
        public static OperationModes? TryParse(String Text)
        {

            if (TryParse(Text, out var mode))
                return mode;

            return null;

        }

        #endregion

        #region TryParse(Text, out OperationModes)

        /// <summary>
        /// Try to parse the given text as a operation modes.
        /// </summary>
        /// <param name="Text">A text representation of a operation modes.</param>
        /// <param name="OperationModes">The parsed operation modes.</param>
        public static Boolean TryParse(String Text, out OperationModes OperationModes)
        {
            switch (Text.Trim())
            {

                case "Idle":
                    OperationModes = OperationModes.Idle;
                    return true;

                case "ChargingOnly":
                    OperationModes = OperationModes.ChargingOnly;
                    return true;

                case "CentralSetpoint":
                    OperationModes = OperationModes.CentralSetpoint;
                    return true;

                case "ExternalSetpoint":
                    OperationModes = OperationModes.ExternalSetpoint;
                    return true;

                case "ExternalLimits":
                    OperationModes = OperationModes.ExternalLimits;
                    return true;

                case "CentralFrequency":
                    OperationModes = OperationModes.CentralFrequency;
                    return true;

                case "LocalFrequency":
                    OperationModes = OperationModes.LocalFrequency;
                    return true;

                case "LocalLoadBalancing":
                    OperationModes = OperationModes.LocalLoadBalancing;
                    return true;

                default:
                    OperationModes = OperationModes.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this OperationModes)

        /// <summary>
        /// Return a string representation of the given operation modes.
        /// </summary>
        /// <param name="OperationModes">A operation modes.</param>
        public static String AsText(this OperationModes OperationModes)

            => OperationModes switch {
                   OperationModes.Idle                => "Idle",
                   OperationModes.ChargingOnly        => "ChargingOnly",
                   OperationModes.CentralSetpoint     => "CentralSetpoint",
                   OperationModes.ExternalSetpoint    => "ExternalSetpoint",
                   OperationModes.ExternalLimits      => "ExternalLimits",
                   OperationModes.CentralFrequency    => "CentralFrequency",
                   OperationModes.LocalFrequency      => "LocalFrequency",
                   OperationModes.LocalLoadBalancing  => "LocalLoadBalancing",
                   _                                  => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Pricing types.
    /// </summary>
    public enum OperationModes
    {

        /// <summary>
        /// Unknown operation modes.
        /// </summary>
        Unknown,

        /// <summary>
        /// Minimize energy consumption by having the EV either on standby or in sleep.
        /// </summary>
        Idle,

        /// <summary>
        /// Classic charging or smart charging mode.
        /// </summary>
        ChargingOnly,

        /// <summary>
        /// Control of setpoint by the CSMS or some secondary actor that relais through the CSMS.
        /// </summary>
        CentralSetpoint,

        /// <summary>
        /// Control of setpoint by an external actor on the charging station.
        /// </summary>
        ExternalSetpoint,

        /// <summary>
        /// Control of (dis)charging limits by an external actor on the charging station.
        /// </summary>
        ExternalLimits,

        /// <summary>
        /// Frequency support with control by the CSMS or some secondary actor that relais through the CSMS.
        /// </summary>
        CentralFrequency,

        /// <summary>
        /// Frequency support with control in the charging station.
        /// </summary>
        LocalFrequency,

        /// <summary>
        /// Load balancing by the charging station.
        /// </summary>
        LocalLoadBalancing

    }

}
