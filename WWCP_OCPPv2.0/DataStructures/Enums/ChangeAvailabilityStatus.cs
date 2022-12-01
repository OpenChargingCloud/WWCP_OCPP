﻿/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extensions methods for change availability status.
    /// </summary>
    public static class ChangeAvailabilityStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a change availability status.
        /// </summary>
        /// <param name="Text">A text representation of a change availability status.</param>
        public static ChangeAvailabilityStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return ChangeAvailabilityStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a change availability status.
        /// </summary>
        /// <param name="Text">A text representation of a change availability status.</param>
        public static ChangeAvailabilityStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out ChangeAvailabilityStatus)

        /// <summary>
        /// Try to parse the given text as a change availability status.
        /// </summary>
        /// <param name="Text">A text representation of a change availability status.</param>
        /// <param name="ChangeAvailabilityStatus">The parsed change availability status.</param>
        public static Boolean TryParse(String Text, out ChangeAvailabilityStatus ChangeAvailabilityStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    ChangeAvailabilityStatus = ChangeAvailabilityStatus.Accepted;
                    return true;

                case "Rejected":
                    ChangeAvailabilityStatus = ChangeAvailabilityStatus.Rejected;
                    return true;

                case "Scheduled":
                    ChangeAvailabilityStatus = ChangeAvailabilityStatus.Scheduled;
                    return true;

                default:
                    ChangeAvailabilityStatus = ChangeAvailabilityStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this ChangeAvailabilityStatus)

        public static String AsText(this ChangeAvailabilityStatus ChangeAvailabilityStatus)

            => ChangeAvailabilityStatus switch {
                   ChangeAvailabilityStatus.Accepted   => "Accepted",
                   ChangeAvailabilityStatus.Rejected   => "Rejected",
                   ChangeAvailabilityStatus.Scheduled  => "Scheduled",
                   _                                   => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Change availability status.
    /// </summary>
    public enum ChangeAvailabilityStatus
    {

        /// <summary>
        /// Unknown availability status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Request has been accepted and will be executed.
        /// </summary>
        Accepted,

        /// <summary>
        /// Request has not been accepted and will not be executed.
        /// </summary>
        Rejected,

        /// <summary>
        /// Request has been accepted and will be executed when
        /// transaction(s) in progress have finished.
        /// </summary>
        Scheduled

    }

}
