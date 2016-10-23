/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
 * This file is part of WWCP OCHP <https://github.com/OpenChargingCloud/WWCP_OCHP>
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

namespace org.GraphDefined.WWCP.OCHPv1_4
{

    /// <summary>
    /// Reflects the current status of the CDR. This is reflecting the status
    /// of internal processing in the clearing house. The value cannot be
    /// changed by the partner's systems directly. Implicit changes are made
    /// while uploading (including revised, rejected CDRs), approving or
    /// declining CDRs.
    /// </summary>
    public enum CDRStatus
    {

        /// <summary>
        /// Unknown status.
        /// </summary>
        Unknown,

        /// <summary>
        /// A new CDR before upload to the CHS.
        /// </summary>
        New,

        /// <summary>
        /// An uploaded CDR was accepted by the CHS as plausible.
        /// </summary>
        Accepted,

        /// <summary>
        /// The checked CDR again rejected by the CHS and is to be archived.
        /// </summary>
        Rejected,

        /// <summary>
        /// The CDR was declined by the owner (EVSP).
        /// </summary>
        Declined,

        /// <summary>
        /// The CDR was approved by the owner (EVSP).
        /// </summary>
        Approved,

        /// <summary>
        /// The CDR was revised by the CPO after a rejection by the owner.
        /// </summary>
        Revised

    }

}
