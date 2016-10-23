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
    /// Result of a registration in response to a BootNotification request.
    /// </summary>
    public enum RegistrationStatus
    {

        /// <summary>
        /// Unknown registration status.
        /// </summary>
        Unknown,


        /// <summary>
        /// Charge point is accepted by Central System.
        /// </summary>
        Accepted,

        /// <summary>
        /// Central System is not yet ready to accept the
        /// Charge Point. Central System may send messages
        /// to retrieve information or prepare the Charge Point.
        /// </summary>
        Pending,

        /// <summary>
        /// Charge point is not accepted by Central System.
        /// This may happen when the Charge Point id is not
        /// known by Central System.
        /// </summary>
        Rejected

    }

}
