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
    /// Extensions methods for log types.
    /// </summary>
    public static class LogTypesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a log type.
        /// </summary>
        /// <param name="Text">A text representation of a log type.</param>
        public static LogTypes Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return LogTypes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a log type.
        /// </summary>
        /// <param name="Text">A text representation of a log type.</param>
        public static LogTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out LogType)

        /// <summary>
        /// Try to parse the given text as a log type.
        /// </summary>
        /// <param name="Text">A text representation of a log type.</param>
        /// <param name="LogType">The parsed log type.</param>
        public static Boolean TryParse(String Text, out LogTypes LogType)
        {
            switch (Text.Trim())
            {

                case "DiagnosticsLog":
                    LogType = LogTypes.DiagnosticsLog;
                    return true;

                case "SecurityLog":
                    LogType = LogTypes.SecurityLog;
                    return true;

                case "DataCollectorLog":
                    LogType = LogTypes.DataCollectorLog;
                    return true;

                default:
                    LogType = LogTypes.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this LogType)

        public static String AsText(this LogTypes LogType)

            => LogType switch {
                   LogTypes.DiagnosticsLog    => "DiagnosticsLog",
                   LogTypes.SecurityLog       => "SecurityLog",
                   LogTypes.DataCollectorLog  => "DataCollectorLog",
                   _                          => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Log types.
    /// </summary>
    public enum LogTypes
    {

        /// <summary>
        /// Unknown generic status.
        /// </summary>
        Unknown,

        /// <summary>
        /// This contains the field definition of a diagnostics log file.
        /// </summary>
        DiagnosticsLog,

        /// <summary>
        /// Sent by the Central System to the Charge Point to request that the Charge Point uploads the security log.
        /// </summary>
        SecurityLog,

        /// <summary>
        /// The log of sampled measurements from the DataCollector component.
        /// </summary>
        DataCollectorLog

    }

}
