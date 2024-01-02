/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extensions methods for clear message status.
    /// </summary>
    public static class ClearMessageStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a clear message status.
        /// </summary>
        /// <param name="Text">A text representation of a clear message status.</param>
        public static ClearMessageStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return ClearMessageStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a clear message status.
        /// </summary>
        /// <param name="Text">A text representation of a clear message status.</param>
        public static ClearMessageStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out ClearMessageStatus)

        /// <summary>
        /// Try to parse the given text as a clear message status.
        /// </summary>
        /// <param name="Text">A text representation of a clear message status.</param>
        /// <param name="ClearMessageStatus">The parsed clear message status.</param>
        public static Boolean TryParse(String Text, out ClearMessageStatus ClearMessageStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    ClearMessageStatus = ClearMessageStatus.Accepted;
                    return true;

                default:
                    ClearMessageStatus = ClearMessageStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this ClearMessageStatus)

        public static String AsText(this ClearMessageStatus ClearMessageStatus)

            => ClearMessageStatus switch {
                   ClearMessageStatus.Accepted  => "Accepted",
                   _                            => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Clear message status.
    /// </summary>
    public enum ClearMessageStatus
    {

        /// <summary>
        /// No message status(s) were found matching the request.
        /// </summary>
        Unknown,

        /// <summary>
        /// Request has been accepted and will be executed.
        /// </summary>
        Accepted

    }

}
