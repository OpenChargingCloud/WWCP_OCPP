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

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Extention methods for severities.
    /// </summary>
    public static class SeveritiesExtensions
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

        #region Parse   (Number)

        /// <summary>
        /// Parse the given number as a severity.
        /// </summary>
        /// <param name="Number">A numeric representation of a severity.</param>
        public static Severities Number(Byte Number)
        {

            if (TryParse(Number, out var severity))
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

        #region TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a severity.
        /// </summary>
        /// <param name="Number">A numeric representation of a severity.</param>
        public static Severities? TryParse(Byte Number)
        {

            if (TryParse(Number, out var severity))
                return severity;

            return null;

        }

        #endregion

        #region TryParse(Text,   out Severity)

        /// <summary>
        /// Try to parse the given text as a severity.
        /// </summary>
        /// <param name="Text">A text representation of a severity.</param>
        /// <param name="Severity">The parsed severity.</param>
        public static Boolean TryParse(String Text, out Severities Severity)
        {
            switch (Text.Trim())
            {

                case "Danger":
                    Severity = Severities.Danger;
                    return true;

                case "Hardware Failure":
                    Severity = Severities.HardwareFailure;
                    return true;

                case "System Failure":
                    Severity = Severities.SystemFailure;
                    return true;

                case "Critical":
                    Severity = Severities.Critical;
                    return true;

                case "Error":
                    Severity = Severities.Error;
                    return true;

                case "Alert":
                    Severity = Severities.Alert;
                    return true;

                case "Warning":
                    Severity = Severities.Warning;
                    return true;

                case "Notice":
                    Severity = Severities.Notice;
                    return true;

                case "Informational":
                    Severity = Severities.Informational;
                    return true;

                case "Debug":
                    Severity = Severities.Debug;
                    return true;

                default:
                    Severity = Severities.Unknown;
                    return false;

            }
        }

        #endregion

        #region TryParse(Number, out Severity)

        /// <summary>
        /// Try to parse the given number as a severity.
        /// </summary>
        /// <param name="Number">A numeric representation of a severity.</param>
        /// <param name="Severity">The parsed severity.</param>
        public static Boolean TryParse(Byte Number, out Severities Severity)
        {
            switch (Number)
            {

                case 0:
                    Severity = Severities.Danger;
                    return true;

                case 1:
                    Severity = Severities.HardwareFailure;
                    return true;

                case 2:
                    Severity = Severities.SystemFailure;
                    return true;

                case 3:
                    Severity = Severities.Critical;
                    return true;

                case 4:
                    Severity = Severities.Error;
                    return true;

                case 5:
                    Severity = Severities.Alert;
                    return true;

                case 6:
                    Severity = Severities.Warning;
                    return true;

                case 7:
                    Severity = Severities.Notice;
                    return true;

                case 8:
                    Severity = Severities.Informational;
                    return true;

                case 9:
                    Severity = Severities.Debug;
                    return true;

                default:
                    Severity = Severities.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText  (this Severity)

        public static String AsText(this Severities Severity)

            => Severity switch {
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

        #region AsNumber(this Severity)

        public static Byte AsNumber(this Severities Severity)

            => Severity switch {
                   Severities.Danger           => 0,
                   Severities.HardwareFailure  => 1,
                   Severities.SystemFailure    => 2,
                   Severities.Critical         => 3,
                   Severities.Error            => 4,
                   Severities.Alert            => 5,
                   Severities.Warning          => 6,
                   Severities.Notice           => 7,
                   Severities.Informational    => 8,
                   Severities.Debug            => 9,
                   _                           => 0
               };

        #endregion

    }


    #region Documentation

    // The severity that will be assigned to an event that is triggered by this monitor.
    // The severity range is 0-9, with 0 as the highest and 9 as the lowest severity level.
    // 
    // The severity levels have the following meaning:
    //   *0-Danger*             Indicates lives are potentially in danger. Urgent attention is needed and action should be taken immediately.
    //   *1-Hardware Failure*   Indicates that the Charging Station is unable to continue regular operations due to Hardware issues. Action is required.
    //   *2-System Failure*     Indicates that the Charging Station is unable to continue regular operations due to software or minor hardware issues. Action is required.
    //   *3-Critical*           Indicates a critical error. Action is required.
    //   *4-Error*              Indicates a non-urgent error. Action is required.
    //   *5-Alert*              Indicates an alert event. Default severity for any type of monitoring event.
    //   *6-Warning*            Indicates a warning event. Action may be required.
    //   *7-Notice*             Indicates an unusual event. No immediate action is required.
    //   *8-Informational*      Indicates a regular operational event. May be used for reporting, measuring throughput, etc. No action is required.
    //   *9-Debug*              Indicates information useful to developers for debugging, not useful during operations.

    #endregion


    /// <summary>
    /// Severities.
    /// </summary>
    public enum Severities
    {

        /// <summary>
        /// Unknown severity.
        /// </summary>
        Unknown,

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
        Debug

    }

}
