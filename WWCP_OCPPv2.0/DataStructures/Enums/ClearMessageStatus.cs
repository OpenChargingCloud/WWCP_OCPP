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
    /// Extentions methods for the clear message status.
    /// </summary>
    public static class ClearMessageStatusExtentions
    {

        #region Parse(Text)

        public static ClearMessageStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"  => ClearMessageStatus.Accepted,
                   _           => ClearMessageStatus.Unknown
               };

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
    /// Defines the clear message status values.
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
