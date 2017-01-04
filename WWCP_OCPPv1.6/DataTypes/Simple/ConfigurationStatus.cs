/*
 * Copyright (c) 2014-2017 GraphDefined GmbH
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
    /// Defines the configuration-status-values.
    /// </summary>
    public enum ConfigurationStatus
    {

        /// <summary>
        /// Unknown configuration status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Configuration key supported and setting has been changed.
        /// </summary>
        Accepted,

        /// <summary>
        /// Configuration key supported, but setting could not be changed.
        /// </summary>
        Rejected,

        /// <summary>
        /// Configuration key supported and setting has been changed, but
        /// change will be available after reboot (The charge Point will
        /// not reboot itself).
        /// </summary>
        RebootRequired,

        /// <summary>
        /// Configuration key is not supported.
        /// </summary>
        NotSupported

    }

}
