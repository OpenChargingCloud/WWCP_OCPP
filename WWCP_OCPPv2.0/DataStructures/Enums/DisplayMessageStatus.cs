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
    /// Extention methods for display message status.
    /// </summary>
    public static class DisplayMessageStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a display message status.
        /// </summary>
        /// <param name="Text">A text representation of a display message status.</param>
        public static DisplayMessageStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return DisplayMessageStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a display message status.
        /// </summary>
        /// <param name="Text">A text representation of a display message status.</param>
        public static DisplayMessageStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out DisplayMessageStatus)

        /// <summary>
        /// Try to parse the given text as a display message status.
        /// </summary>
        /// <param name="Text">A text representation of a display message status.</param>
        /// <param name="DisplayMessageStatus">The parsed display message status.</param>
        public static Boolean TryParse(String Text, out DisplayMessageStatus DisplayMessageStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    DisplayMessageStatus = DisplayMessageStatus.Accepted;
                    return true;

                case "NotSupportedMessageFormat":
                    DisplayMessageStatus = DisplayMessageStatus.NotSupportedMessageFormat;
                    return true;

                case "Rejected":
                    DisplayMessageStatus = DisplayMessageStatus.Rejected;
                    return true;

                case "NotSupportedPriority":
                    DisplayMessageStatus = DisplayMessageStatus.NotSupportedPriority;
                    return true;

                case "NotSupportedState":
                    DisplayMessageStatus = DisplayMessageStatus.NotSupportedState;
                    return true;

                case "UnknownTransaction":
                    DisplayMessageStatus = DisplayMessageStatus.UnknownTransaction;
                    return true;

                default:
                    DisplayMessageStatus = DisplayMessageStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this DisplayMessageStatus)

        public static String AsText(this DisplayMessageStatus DisplayMessageStatus)

            => DisplayMessageStatus switch {
                   DisplayMessageStatus.Accepted                   => "Accepted",
                   DisplayMessageStatus.NotSupportedMessageFormat  => "NotSupportedMessageFormat",
                   DisplayMessageStatus.Rejected                   => "Rejected",
                   DisplayMessageStatus.NotSupportedPriority       => "NotSupportedPriority",
                   DisplayMessageStatus.NotSupportedState          => "NotSupportedState",
                   DisplayMessageStatus.UnknownTransaction         => "UnknownTransaction",
                   _                                               => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Display message status.
    /// </summary>
    public enum DisplayMessageStatus
    {

        /// <summary>
        /// Unknown display message status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The request to display the message was accepted.
        /// </summary>
        Accepted,

        /// <summary>
        /// None of the formats in the given message are supported.
        /// </summary>
        NotSupportedMessageFormat,

        /// <summary>
        /// Request cannot be handled.
        /// </summary>
        Rejected,

        /// <summary>
        /// The given message priority is not supported for displaying messages at the charging station.
        /// </summary>
        NotSupportedPriority,

        /// <summary>
        /// The given message state is not supported for displaying messages at the charging station.
        /// </summary>
        NotSupportedState,

        /// <summary>
        /// The given transaction identification is not known or not ongoing.
        /// </summary>
        UnknownTransaction

    }

}
