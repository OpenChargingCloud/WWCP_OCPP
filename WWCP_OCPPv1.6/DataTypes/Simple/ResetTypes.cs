/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
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
    /// Defines the reset-type-values.
    /// </summary>
    public enum ResetTypes
    {

        /// <summary>
        /// Unknown reset type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Full reboot of Charge Point software.
        /// </summary>
        Hard,

        /// <summary>
        /// Return to initial status, gracefully terminating
        /// any transactions in progress.
        /// </summary>
        Soft

    }

}
