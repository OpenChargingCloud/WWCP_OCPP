/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
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

namespace org.GraphDefined.WWCP.OCPPv1_6
{

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
