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
    /// Extensions methods for transaction events.
    /// </summary>
    public static class TransactionEventsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a transaction event.
        /// </summary>
        /// <param name="Text">A text representation of a transaction event.</param>
        public static TransactionEvents Parse(String Text)
        {

            if (TryParse(Text, out var reason))
                return reason;

            return TransactionEvents.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a transaction event.
        /// </summary>
        /// <param name="Text">A text representation of a transaction event.</param>
        public static TransactionEvents? TryParse(String Text)
        {

            if (TryParse(Text, out var reason))
                return reason;

            return null;

        }

        #endregion

        #region TryParse(Text, out TransactionEvent)

        /// <summary>
        /// Try to parse the given text as a transaction event.
        /// </summary>
        /// <param name="Text">A text representation of a transaction event.</param>
        /// <param name="TransactionEvent">The parsed transaction event.</param>
        public static Boolean TryParse(String Text, out TransactionEvents TransactionEvent)
        {
            switch (Text.Trim())
            {

                case "Started":
                    TransactionEvent = TransactionEvents.Started;
                    return true;

                case "Updated":
                    TransactionEvent = TransactionEvents.Updated;
                    return true;

                case "Ended":
                    TransactionEvent = TransactionEvents.Ended;
                    return true;

                default:
                    TransactionEvent = TransactionEvents.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this TransactionEvent)

        public static String AsText(this TransactionEvents TransactionEvent)

            => TransactionEvent switch {
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

        /// <summary>
        /// The transaction just started.
        /// </summary>
        Started,

        /// <summary>
        /// The transaction was updated.
        /// </summary>
        Updated,

        /// <summary>
        /// The transaction ended.
        /// </summary>
        Ended

    }

}
