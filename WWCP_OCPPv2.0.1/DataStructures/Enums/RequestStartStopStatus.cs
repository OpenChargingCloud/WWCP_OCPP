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

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Extensions methods for request start stop status.
    /// </summary>
    public static class RequestStartStopStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a request start stop status.
        /// </summary>
        /// <param name="Text">A text representation of a request start stop status.</param>
        public static RequestStartStopStatus Parse(String Text)
        {

            if (TryParse(Text, out var reason))
                return reason;

            return RequestStartStopStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a request start stop status.
        /// </summary>
        /// <param name="Text">A text representation of a request start stop status.</param>
        public static RequestStartStopStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var reason))
                return reason;

            return null;

        }

        #endregion

        #region TryParse(Text, out RequestStartStopStatus)

        /// <summary>
        /// Try to parse the given text as a request start stop status.
        /// </summary>
        /// <param name="Text">A text representation of a request start stop status.</param>
        /// <param name="RequestStartStopStatus">The parsed request start stop status.</param>
        public static Boolean TryParse(String Text, out RequestStartStopStatus RequestStartStopStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    RequestStartStopStatus = RequestStartStopStatus.Accepted;
                    return true;

                case "Rejected":
                    RequestStartStopStatus = RequestStartStopStatus.Rejected;
                    return true;

                default:
                    RequestStartStopStatus = RequestStartStopStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this RequestStartStopStatus)

        public static String AsText(this RequestStartStopStatus RequestStartStopStatus)

            => RequestStartStopStatus switch {
                   RequestStartStopStatus.Accepted  => "Accepted",
                   RequestStartStopStatus.Rejected  => "Rejected",
                   _                                => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Request start stop status.
    /// </summary>
    public enum RequestStartStopStatus
    {

        /// <summary>
        /// Unknown request-start-stop status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Command will be executed.
        /// </summary>
        Accepted,

        /// <summary>
        /// Command will not be executed.
        /// </summary>
        Rejected

    }

}
