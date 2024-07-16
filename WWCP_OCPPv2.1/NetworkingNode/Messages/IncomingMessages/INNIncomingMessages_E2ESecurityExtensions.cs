/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    public interface INNIncomingMessages_E2ESecurityExtensions
    {

        /// <summary>
        /// An event sent whenever an AddSignaturePolicy request was received.
        /// </summary>
        event OnAddSignaturePolicyDelegate       OnAddSignaturePolicy;

        /// <summary>
        /// An event sent whenever an UpdateSignaturePolicy request was received.
        /// </summary>
        event OnUpdateSignaturePolicyDelegate    OnUpdateSignaturePolicy;

        /// <summary>
        /// An event sent whenever a DeleteSignaturePolicy request was received.
        /// </summary>
        event OnDeleteSignaturePolicyDelegate    OnDeleteSignaturePolicy;

        /// <summary>
        /// An event sent whenever an AddUserRole request was received.
        /// </summary>
        event OnAddUserRoleDelegate              OnAddUserRole;

        /// <summary>
        /// An event sent whenever an UpdateUserRole request was received.
        /// </summary>
        event OnUpdateUserRoleDelegate           OnUpdateUserRole;

        /// <summary>
        /// An event sent whenever a DeleteUserRole request was received.
        /// </summary>
        event OnDeleteUserRoleDelegate           OnDeleteUserRole;


        /// <summary>
        /// An event sent whenever a SecureDataTransfer request was received.
        /// </summary>
        event OnSecureDataTransferDelegate       OnSecureDataTransfer;

    }

}
