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

#region Usings

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    public interface E2ESecurityExtensions_IncomingMessages
    {


        /// <summary>
        /// An event sent whenever an AddSignerRole request was received.
        /// </summary>
        event OnAddSignerRoleDelegate    OnAddSignerRole;

        /// <summary>
        /// An event sent whenever an UpdateSignerRole request was received.
        /// </summary>
        event OnUpdateSignerRoleDelegate    OnUpdateSignerRole;

        /// <summary>
        /// An event sent whenever a DeleteSignerRole request was received.
        /// </summary>
        event OnDeleteSignerRoleDelegate    OnDeleteSignerRole;

        /// <summary>
        /// An event sent whenever a GetSignerRoles request was received.
        /// </summary>
        event OnGetSignerRolesDelegate    OnGetSignerRoles;

        /// <summary>
        /// An event sent whenever a SetSignaturePolicy request was received.
        /// </summary>
        event OnSetSignaturePolicyDelegate    OnSetSignaturePolicy;

        /// <summary>
        /// An event sent whenever a GetSignaturePolicy request was received.
        /// </summary>
        event OnGetSignaturePolicyDelegate    OnGetSignaturePolicy;


        /// <summary>
        /// An event sent whenever a SecureDataTransfer request was received.
        /// </summary>
        event OnSecureDataTransferDelegate       OnSecureDataTransfer;

    }

}
