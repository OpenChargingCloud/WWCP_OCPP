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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extensions methods for the remote start stop status.
    /// </summary>
    public static class RemoteStartStopStatusExtensions
    {

        #region Parse(Text)

        public static RemoteStartStopStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"  => RemoteStartStopStatus.Accepted,
                   "Rejected"  => RemoteStartStopStatus.Rejected,
                   _           => RemoteStartStopStatus.Unknown
               };

        #endregion

        #region AsText(this RemoteStartStopStatus)

        public static String AsText(this RemoteStartStopStatus RemoteStartStopStatus)

            => RemoteStartStopStatus switch {
                   RemoteStartStopStatus.Accepted  => "Accepted",
                   RemoteStartStopStatus.Rejected  => "Rejected",
                   _                               => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Defines the remote-start-stop-status-values.
    /// </summary>
    public enum RemoteStartStopStatus
    {

        /// <summary>
        /// Unknown remote-start-stop status.
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
