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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extensions methods for the recurrency kinds.
    /// </summary>
    public static class RecurrencyKindsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a recurrency kind.
        /// </summary>
        /// <param name="Text">A text representation of a recurrency kind.</param>
        public static RecurrencyKinds Parse(String Text)
        {

            if (TryParse(Text, out var kind))
                return kind;

            return RecurrencyKinds.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a recurrency kind.
        /// </summary>
        /// <param name="Text">A text representation of a recurrency kind.</param>
        public static RecurrencyKinds? TryParse(String Text)
        {

            if (TryParse(Text, out var kind))
                return kind;

            return null;

        }

        #endregion

        #region TryParse(Text, out RecurrencyKind)

        /// <summary>
        /// Try to parse the given text as a recurrency kind.
        /// </summary>
        /// <param name="Text">A text representation of a recurrency kind.</param>
        /// <param name="RecurrencyKind">The parsed recurrency kind.</param>
        public static Boolean TryParse(String Text, out RecurrencyKinds RecurrencyKind)
        {
            switch (Text.Trim())
            {

                case "Daily":
                    RecurrencyKind = RecurrencyKinds.Daily;
                    return true;

                case "Weekly":
                    RecurrencyKind = RecurrencyKinds.Weekly;
                    return true;

                default:
                    RecurrencyKind = RecurrencyKinds.Unknown;
                    return false;

            }
        }

        #endregion

        #region AsText  (this RecurrencyKindType)

        public static String AsText(this RecurrencyKinds RecurrencyKindType)

            => RecurrencyKindType switch {
                   RecurrencyKinds.Daily   => "Daily",
                   RecurrencyKinds.Weekly  => "Weekly",
                   _                       => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Defines the recurrency-kind-type-values.
    /// </summary>
    public enum RecurrencyKinds
    {

        /// <summary>
        /// Unknown recurrency-kind type.
        /// </summary>
        Unknown,

        /// <summary>
        /// The schedule restarts at the beginning of the next day.
        /// </summary>
        Daily,

        /// <summary>
        /// The schedule restarts at the beginning of the next week
        /// (defined as Monday morning).
        /// </summary>
        Weekly

    }

}
