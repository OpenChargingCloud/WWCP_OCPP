/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

#region Usings

using System;

#endregion

namespace cloud.charging.adapters.OCPPv1_6
{

    /// <summary>
    /// Extentions methods for the recurrency kinds.
    /// </summary>
    public static class RecurrencyKindsExtentions
    {

        #region Parse(Text)

        public static RecurrencyKinds Parse(String Text)
        {

            switch (Text)
            {

                case "Daily":
                    return RecurrencyKinds.Daily;

                case "Weekly":
                    return RecurrencyKinds.Weekly;


                default:
                    return RecurrencyKinds.Unknown;

            }

        }

        #endregion

        #region AsText(this RecurrencyKindType)

        public static String AsText(this RecurrencyKinds RecurrencyKindType)
        {

            switch (RecurrencyKindType)
            {

                case RecurrencyKinds.Daily:
                    return "Daily";

                case RecurrencyKinds.Weekly:
                    return "Weekly";


                default:
                    return "unknown";

            }

        }

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
