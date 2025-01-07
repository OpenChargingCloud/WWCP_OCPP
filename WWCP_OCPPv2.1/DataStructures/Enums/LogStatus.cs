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
    /// Extensions methods for get log status.
    /// </summary>
    public static class LogStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a log status.
        /// </summary>
        /// <param name="Text">A text representation of a log status.</param>
        public static LogStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return LogStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a log status.
        /// </summary>
        /// <param name="Text">A text representation of a log status.</param>
        public static LogStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out LogStatus)

        /// <summary>
        /// Try to parse the given text as a log status.
        /// </summary>
        /// <param name="Text">A text representation of a log status.</param>
        /// <param name="LogStatus">The parsed log status.</param>
        public static Boolean TryParse(String Text, out LogStatus LogStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    LogStatus = LogStatus.Accepted;
                    return true;

                case "Rejected":
                    LogStatus = LogStatus.Rejected;
                    return true;

                case "AcceptedCanceled":
                    LogStatus = LogStatus.AcceptedCanceled;
                    return true;

                default:
                    LogStatus = LogStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this LogStatus)

        public static String AsText(this LogStatus LogStatus)

            => LogStatus switch {
                   LogStatus.Accepted          => "Accepted",
                   LogStatus.Rejected          => "Rejected",
                   LogStatus.AcceptedCanceled  => "AcceptedCanceled",
                      _                        => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Get log status.
    /// </summary>
    public enum LogStatus
    {

        /// <summary>
        /// Unknown generic status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Accepted this log upload. This does not mean the log file is uploaded is successfully, the Charge Point will now start the log file upload.
        /// </summary>
        Accepted,

        /// <summary>
        /// Log update request rejected.
        /// </summary>
        Rejected,

        /// <summary>
        /// Accepted this log upload, but in doing this has canceled an ongoing log file upload.
        /// </summary>
        AcceptedCanceled

    }

}
