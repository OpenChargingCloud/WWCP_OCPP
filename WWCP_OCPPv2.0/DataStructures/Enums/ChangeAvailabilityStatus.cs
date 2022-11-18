/*
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
    /// Extentions methods for the change availability status.
    /// </summary>
    public static class ChangeAvailabilityStatusExtentions
    {

        #region Parse(Text)

        public static ChangeAvailabilityStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"   => ChangeAvailabilityStatus.Accepted,
                   "Rejected"   => ChangeAvailabilityStatus.Rejected,
                   "Scheduled"  => ChangeAvailabilityStatus.Scheduled,
                   _            => ChangeAvailabilityStatus.Unknown
               };

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
    /// Defines the change availability status values.
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
