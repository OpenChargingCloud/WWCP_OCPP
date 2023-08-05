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

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Extention methods for the reading context.
    /// </summary>
    public static class ReadingContextsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a reading context.
        /// </summary>
        /// <param name="Text">A text representation of a reading context.</param>
        public static ReadingContexts Parse(String Text)
        {

            if (TryParse(Text, out var context))
                return context;

            return ReadingContexts.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a reading context.
        /// </summary>
        /// <param name="Text">A text representation of a reading context.</param>
        public static ReadingContexts? TryParse(String Text)
        {

            if (TryParse(Text, out var context))
                return context;

            return null;

        }

        #endregion

        #region TryParse(Text, out ReadingContext)

        /// <summary>
        /// Try to parse the given text as a reading context.
        /// </summary>
        /// <param name="Text">A text representation of a reading context.</param>
        /// <param name="ReadingContext">The parsed reading context.</param>
        public static Boolean TryParse(String Text, out ReadingContexts ReadingContext)
        {
            switch (Text.Trim())
            {

                case "Interruption.Begin":
                    ReadingContext = ReadingContexts.InterruptionBegin;
                    return true;

                case "Interruption.End":
                    ReadingContext = ReadingContexts.InterruptionEnd;
                    return true;

                case "Other":
                    ReadingContext = ReadingContexts.Other;
                    return true;

                case "Sample.Clock":
                    ReadingContext = ReadingContexts.SampleClock;
                    return true;

                case "Transaction.Begin":
                    ReadingContext = ReadingContexts.TransactionBegin;
                    return true;

                case "Transaction.End":
                    ReadingContext = ReadingContexts.TransactionEnd;
                    return true;

                case "Trigger":
                    ReadingContext = ReadingContexts.Trigger;
                    return true;

                case "Sample.Periodic":
                    ReadingContext = ReadingContexts.SamplePeriodic;
                    return true;

                default:
                    ReadingContext = ReadingContexts.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this ReadingContext)

        public static String AsText(this ReadingContexts ReadingContext)

            => ReadingContext switch {
                   ReadingContexts.InterruptionBegin  => "Interruption.Begin",
                   ReadingContexts.InterruptionEnd    => "Interruption.End",
                   ReadingContexts.Other              => "Other",
                   ReadingContexts.SampleClock        => "Sample.Clock",
                   ReadingContexts.TransactionBegin   => "Transaction.Begin",
                   ReadingContexts.TransactionEnd     => "Transaction.End",
                   ReadingContexts.Trigger            => "Trigger",
                   ReadingContexts.SamplePeriodic     => "Sample.Periodic",
                   _                                  => "Unknown"
            };

        #endregion

    }


    /// <summary>
    /// Reading contexts.
    /// </summary>
    public enum ReadingContexts
    {

        /// <summary>
        /// Unknown reading context.
        /// </summary>
        Unknown,

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
