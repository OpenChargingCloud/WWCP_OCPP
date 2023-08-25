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
    /// Extensions methods for priority charging status.
    /// </summary>
    public static class PriorityChargingStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a priority charging status.
        /// </summary>
        /// <param name="Text">A text representation of a priority charging status.</param>
        public static PriorityChargingStatus Parse(String Text)
        {

            if (TryParse(Text, out var mode))
                return mode;

            return PriorityChargingStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a priority charging status.
        /// </summary>
        /// <param name="Text">A text representation of a priority charging status.</param>
        public static PriorityChargingStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var mode))
                return mode;

            return null;

        }

        #endregion

        #region TryParse(Text, out PriorityChargingStatus)

        /// <summary>
        /// Try to parse the given text as a priority charging status.
        /// </summary>
        /// <param name="Text">A text representation of a priority charging status.</param>
        /// <param name="PriorityChargingStatus">The parsed priority charging status.</param>
        public static Boolean TryParse(String Text, out PriorityChargingStatus PriorityChargingStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    PriorityChargingStatus = PriorityChargingStatus.Accepted;
                    return true;

                case "Rejected":
                    PriorityChargingStatus = PriorityChargingStatus.Rejected;
                    return true;

                case "NoProfile":
                    PriorityChargingStatus = PriorityChargingStatus.NoProfile;
                    return true;

                default:
                    PriorityChargingStatus = PriorityChargingStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this PriorityChargingStatus)

        /// <summary>
        /// Return a string representation of the given priority charging status.
        /// </summary>
        /// <param name="PriorityChargingStatus">A priority charging status.</param>
        public static String AsText(this PriorityChargingStatus PriorityChargingStatus)

            => PriorityChargingStatus switch {
                   PriorityChargingStatus.Accepted   => "Accepted",
                   PriorityChargingStatus.Rejected   => "Rejected",
                   PriorityChargingStatus.NoProfile  => "NoProfile",
                   _                                 => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Pricing types.
    /// </summary>
    public enum PriorityChargingStatus
    {

        /// <summary>
        /// Unknown priority charging status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The request has been accepted.
        /// </summary>
        Accepted,

        /// <summary>
        /// The request has been rejected.
        /// </summary>
        Rejected,

        /// <summary>
        /// No priority charging profile present
        /// </summary>
        NoProfile

    }

}
