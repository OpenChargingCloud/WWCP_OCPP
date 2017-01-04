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
    /// Defines the update-type-values.
    /// </summary>
    public enum UpdateTypes
    {

        /// <summary>
        /// Unknown update type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Indicates that the current local authorization List
        /// must be updated with the values in this message.
        /// </summary>
        Differential,

        /// <summary>
        /// Indicates that the current local authorization list
        /// must be replaced by the values in this message.
        /// </summary>
        Full

    }

}
