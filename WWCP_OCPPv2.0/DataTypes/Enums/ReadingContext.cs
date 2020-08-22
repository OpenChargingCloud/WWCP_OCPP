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

namespace cloud.charging.adapters.OCPPv2_0
{

    /// <summary>
    /// Extention methods for the reading context.
    /// </summary>
    public static class ReadingContextExtentions
    {

        #region Parse(Text)

        public static ReadingContexts Parse(String Text)
        {

            switch (Text?.Trim())
            {

                case "Interruption.Begin":
                    return ReadingContexts.InterruptionBegin;

                case "Interruption.End":
                    return ReadingContexts.InterruptionEnd;

                case "Other":
                    return ReadingContexts.Other;

                case "Sample.Clock":
                    return ReadingContexts.SampleClock;

                case "Transaction.Begin":
                    return ReadingContexts.TransactionBegin;

                case "Transaction.End":
                    return ReadingContexts.TransactionEnd;

                case "Trigger":
                    return ReadingContexts.Trigger;


                default:
                    return ReadingContexts.SamplePeriodic;

            }

        }

        #endregion

        #region AsText(this ReadingContexts)

        public static String AsText(this ReadingContexts ReadingContexts)
        {

            switch (ReadingContexts)
            {

                case ReadingContexts.InterruptionBegin:
                    return "Interruption.Begin";

                case ReadingContexts.InterruptionEnd:
                    return "Interruption.End";

                case ReadingContexts.Other:
                    return "Other";

                case ReadingContexts.SampleClock:
                    return "Sample.Clock";

                case ReadingContexts.TransactionBegin:
                    return "Transaction.Begin";

                case ReadingContexts.TransactionEnd:
                    return "Transaction.End";

                case ReadingContexts.Trigger:
                    return "Trigger";


                default:
                    return "Sample.Periodic";

            }

        }

        #endregion

    }


    /// <summary>
    /// The values of the context field of a value in SampledValue.
    /// </summary>
    public enum ReadingContexts
    {

        /// <summary>
        /// Value taken at start of interruption.
        /// </summary>
        InterruptionBegin,

        /// <summary>
        /// Value taken when resuming after interruption.
        /// </summary>
        InterruptionEnd,

        /// <summary>
        /// Value for any other situations.
        /// </summary>
        Other,

        /// <summary>
        /// Value taken at clock aligned interval.
        /// </summary>
        SampleClock,

        /// <summary>
        /// Value taken as periodic sample relative to start time of transaction.
        /// </summary>
        SamplePeriodic,

        /// <summary>
        /// Value taken at the beginning of a transaction.
        /// </summary>
        TransactionBegin,

        /// <summary>
        /// Value taken at the end of a transaction.
        /// </summary>
        TransactionEnd,

        /// <summary>
        /// Value taken in response to a TriggerMessage request.
        /// </summary>
        Trigger


    }

}
