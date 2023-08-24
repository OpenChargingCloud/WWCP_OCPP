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
    /// Extensions methods for reset status.
    /// </summary>
    public static class ResetStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a reset status.
        /// </summary>
        /// <param name="Text">A text representation of a reset status.</param>
        public static ResetStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return ResetStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a reset status.
        /// </summary>
        /// <param name="Text">A text representation of a reset status.</param>
        public static ResetStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out ResetStatus)

        /// <summary>
        /// Try to parse the given text as a reset status.
        /// </summary>
        /// <param name="Text">A text representation of a reset status.</param>
        /// <param name="ResetStatus">The parsed reset status.</param>
        public static Boolean TryParse(String Text, out ResetStatus ResetStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    ResetStatus = ResetStatus.Accepted;
                    return true;

                case "Rejected":
                    ResetStatus = ResetStatus.Rejected;
                    return true;

                case "Scheduled":
                    ResetStatus = ResetStatus.Rejected;
                    return true;

                default:
                    ResetStatus = ResetStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this ResetStatus)

        public static String AsText(this ResetStatus ResetStatus)

            => ResetStatus switch {
                   ResetStatus.Accepted   => "Accepted",
                   ResetStatus.Rejected   => "Rejected",
                   ResetStatus.Scheduled  => "Scheduled",
                   _                      => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Reset status.
    /// </summary>
    public enum ResetStatus
    {

        /// <summary>
        /// Unknown reset status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Command will be executed.
        /// </summary>
        Accepted,

        /// <summary>
        /// Command will not be executed.
        /// </summary>
        Rejected,

        /// <summary>
        /// Reset command is scheduled as the charging station is still busy with a process that cannot be interrupted.
        /// Reset will be executed when this process is finished.
        /// </summary>
        Scheduled

    }

}
