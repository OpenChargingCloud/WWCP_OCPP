/*
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
    /// Extension methods for the reading context.
    /// </summary>
    public static class ReadingContextExtensions
    {

        #region Parse(Text)

        public static ReadingContexts Parse(String Text)

            => Text.Trim() switch {
                   "Interruption.Begin"  => ReadingContexts.InterruptionBegin,
                   "Interruption.End"    => ReadingContexts.InterruptionEnd,
                   "Other"               => ReadingContexts.Other,
                   "Sample.Clock"        => ReadingContexts.SampleClock,
                   "Transaction.Begin"   => ReadingContexts.TransactionBegin,
                   "Transaction.End"     => ReadingContexts.TransactionEnd,
                   "Trigger"             => ReadingContexts.Trigger,
                   _                     => ReadingContexts.SamplePeriodic
               };

        #endregion

        #region AsText(this ReadingContexts)

        public static String AsText(this ReadingContexts ReadingContexts)

            => ReadingContexts switch {
                   ReadingContexts.InterruptionBegin  => "Interruption.Begin",
                   ReadingContexts.InterruptionEnd    => "Interruption.End",
                   ReadingContexts.Other              => "Other",
                   ReadingContexts.SampleClock        => "Sample.Clock",
                   ReadingContexts.TransactionBegin   => "Transaction.Begin",
                   ReadingContexts.TransactionEnd     => "Transaction.End",
                   ReadingContexts.Trigger            => "Trigger",
                   _                                  => "Sample.Periodic"
               };

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
