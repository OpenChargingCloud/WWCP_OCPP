﻿/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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
    /// Extensions methods for the trigger message status.
    /// </summary>
    public static class TriggerMessageStatusExtensions
    {

        #region Parse(Text)

        public static TriggerMessageStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"  => TriggerMessageStatus.Accepted,
                   "Rejected"  => TriggerMessageStatus.Rejected,
                   _           => TriggerMessageStatus.NotImplemented
               };

        #endregion

        #region AsText(this TriggerMessageStatus)

        public static String AsText(this TriggerMessageStatus TriggerMessageStatus)

            => TriggerMessageStatus switch {
                   TriggerMessageStatus.Accepted  => "Accepted",
                   TriggerMessageStatus.Rejected  => "Rejected",
                   _                              => "NotImplemented"
               };

        #endregion

    }


    /// <summary>
    /// Trigger message status.
    /// </summary>
    public enum TriggerMessageStatus
    {

        /// <summary>
        /// Requested notification will be sent.
        /// </summary>
        Accepted,

        /// <summary>
        /// Requested notification will not be sent.
        /// </summary>
        Rejected,

        /// <summary>
        /// Requested notification cannot be sent because
        /// it is either not implemented or unknown.
        /// </summary>
        NotImplemented

    }

}
