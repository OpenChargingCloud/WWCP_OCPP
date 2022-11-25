/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extention methods for severities.
    /// </summary>
    public static class SeveritiesExtentions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a severity.
        /// </summary>
        /// <param name="Text">A text representation of a severity.</param>
        public static Severities Parse(String Text)
        {

            if (TryParse(Text, out var severity))
                return severity;

            return Severities.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a severity.
        /// </summary>
        /// <param name="Text">A text representation of a severity.</param>
        public static Severities? TryParse(String Text)
        {

            if (TryParse(Text, out var severity))
                return severity;

            return null;

        }

        #endregion

        #region TryParse(Text, out BootReason)

        /// <summary>
        /// Try to parse the given text as a severity.
        /// </summary>
        /// <param name="Text">A text representation of a severity.</param>
        /// <param name="BootReason">The parsed severity.</param>
        public static Boolean TryParse(String Text, out Severities BootReason)
        {
            switch (Text.Trim())
            {

                case "Danger":
                    BootReason = Severities.Danger;
                    return true;

                case "Hardware Failure":
                    BootReason = Severities.HardwareFailure;
                    return true;

                case "System Failure":
                    BootReason = Severities.SystemFailure;
                    return true;

                case "Critical":
                    BootReason = Severities.Critical;
                    return true;

                case "Error":
                    BootReason = Severities.Error;
                    return true;

                case "Alert":
                    BootReason = Severities.Alert;
                    return true;

                case "Warning":
                    BootReason = Severities.Warning;
                    return true;

                case "Notice":
                    BootReason = Severities.Notice;
                    return true;

                case "Informational":
                    BootReason = Severities.Informational;
                    return true;

                case "Debug":
                    BootReason = Severities.Debug;
                    return true;

                default:
                    BootReason = Severities.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this Phase)

        public static String AsText(this Severities BootReason)

            => BootReason switch {
                   Severities.Danger           => "Danger",
                   Severities.HardwareFailure  => "Hardware Failure",
                   Severities.SystemFailure    => "System Failure",
                   Severities.Critical         => "Critical",
                   Severities.Error            => "Error",
                   Severities.Alert            => "Alert",
                   Severities.Warning          => "Warning",
                   Severities.Notice           => "Notice",
                   Severities.Informational    => "Informational",
                   Severities.Debug            => "Debug",
                   _                           => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Severities.
    /// </summary>
    public enum Severities
    {

        /// <summary>
        /// Indicates lives are potentially in danger.
        /// Urgent attention is needed and action should be taken immediately.
        /// </summary>
        Danger,

        /// <summary>
        /// Indicates that the Charging Station is unable to continue regular operations due to Hardware issues.
        /// Action is required.
        /// </summary>
        HardwareFailure,

        /// <summary>
        /// Indicates that the Charging Station is unable to continue regular operations due to software or minor hardware issues.
        /// Action is required.
        /// </summary>
        SystemFailure,

        /// <summary>
        /// Indicates a critical error.
        /// Action is required.
        /// </summary>
        Critical,

        /// <summary>
        /// Indicates a non-urgent error.
        /// Action is required.
        /// </summary>
        Error,

        /// <summary>
        /// Indicates an alert event.
        /// Default severity for any type of monitoring event.
        /// </summary>
        Alert,

        /// <summary>
        /// Indicates a warning event.
        /// Action may be required.
        /// </summary>
        Warning,

        /// <summary>
        /// Indicates an unusual event.
        /// No immediate action is required.
        /// </summary>
        Notice,

        /// <summary>
        /// Indicates a regular operational event.
        /// May be used for reporting, measuring throughput, etc.
        /// No action is required.
        /// </summary>
        Informational,

        /// <summary>
        /// Indicates information useful to developers for debugging, not useful during operations.
        /// </summary>
        Debug,

        /// <summary>
        /// Unknown severity.
        /// </summary>
        Unknown,

    }

}
