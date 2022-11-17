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
    /// Extentions methods for transaction events.
    /// </summary>
    public static class TransactionEventsExtentions
    {

        #region Parse(Text)

        public static TransactionEvents Parse(String Text)

            => Text.Trim() switch {
                   "Started"  => TransactionEvents.Started,
                   "Updated"  => TransactionEvents.Updated,
                   "Ended"    => TransactionEvents.Ended,
                   _          => TransactionEvents.Unknown
               };

        #endregion

        #region AsText(this TransactionEvents)

        public static String AsText(this TransactionEvents TransactionEvents)

            => TransactionEvents switch {
                   TransactionEvents.Started  => "Started",
                   TransactionEvents.Updated  => "Updated",
                   TransactionEvents.Ended    => "Ended",
                   _                          => "Unknown"
               };

        #endregion

    }

    /// <summary>
    /// Transaction events.
    /// </summary>
    public enum TransactionEvents
    {

        /// <summary>
        /// Unknown transaction event.
        /// </summary>
        Unknown,

        Started,
        Updated,
        Ended

    }

}
