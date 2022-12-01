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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extensions methods for the update status.
    /// </summary>
    public static class UpdateStatusExtensions
    {

        #region Parse(Text)

        public static UpdateStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"         => UpdateStatus.Accepted,
                   "Failed"           => UpdateStatus.Failed,
                   "NotSupported"     => UpdateStatus.NotSupported,
                   "VersionMismatch"  => UpdateStatus.VersionMismatch,
                   _                  => UpdateStatus.Unknown
               };

        #endregion

        #region AsText(this UpdateStatus)

        public static String AsText(this UpdateStatus UpdateStatus)

            => UpdateStatus switch {
                   UpdateStatus.Accepted         => "Accepted",
                   UpdateStatus.Failed           => "Failed",
                   UpdateStatus.NotSupported     => "NotSupported",
                   UpdateStatus.VersionMismatch  => "VersionMismatch",
                   _                             => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Defines the update-status-values.
    /// </summary>
    public enum UpdateStatus
    {

        /// <summary>
        /// Unknown update status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Local authorization list successfully updated.
        /// </summary>
        Accepted,

        /// <summary>
        /// Failed to update the local authorization list.
        /// </summary>
        Failed,

        /// <summary>
        /// Update of Local Authorization List is not
        /// supported by Charge Point.
        /// </summary>
        NotSupported,

        /// <summary>
        /// Version number in the request for a differential
        /// update is less or equal then version number of
        /// current list.
        /// </summary>
        VersionMismatch

    }

}
