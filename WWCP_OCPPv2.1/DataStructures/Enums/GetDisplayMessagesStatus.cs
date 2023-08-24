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
    /// Extention methods for get display messages status.
    /// </summary>
    public static class GetDisplayMessagesStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a get display messages status.
        /// </summary>
        /// <param name="Text">A text representation of a get display messages status.</param>
        public static GetDisplayMessagesStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return GetDisplayMessagesStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a get display messages status.
        /// </summary>
        /// <param name="Text">A text representation of a get display messages status.</param>
        public static GetDisplayMessagesStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out GetDisplayMessagesStatus)

        /// <summary>
        /// Try to parse the given text as a get display messages status.
        /// </summary>
        /// <param name="Text">A text representation of a get display messages status.</param>
        /// <param name="GetDisplayMessagesStatus">The parsed get display messages status.</param>
        public static Boolean TryParse(String Text, out GetDisplayMessagesStatus GetDisplayMessagesStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    GetDisplayMessagesStatus = GetDisplayMessagesStatus.Accepted;
                    return true;

                default:
                    GetDisplayMessagesStatus = GetDisplayMessagesStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this GetDisplayMessagesStatus)

        public static String AsText(this GetDisplayMessagesStatus GetDisplayMessagesStatus)

            => GetDisplayMessagesStatus switch {
                   GetDisplayMessagesStatus.Accepted  => "Accepted",
                   _                                   => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Get display messages status.
    /// </summary>
    public enum GetDisplayMessagesStatus
    {

        /// <summary>
        /// No messages found that match the given criteria.
        /// </summary>
        Unknown,

        /// <summary>
        /// Request accepted, there are display messages found that match all the requested criteria.
        /// The charging station will send NotifyDisplayMessagesRequest messages to report the requested display messages.
        /// </summary>
        Accepted

    }

}
