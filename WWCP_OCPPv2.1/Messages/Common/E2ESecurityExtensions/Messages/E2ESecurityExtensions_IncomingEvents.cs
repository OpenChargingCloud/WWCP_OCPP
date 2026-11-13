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

    public interface E2ESecurityExtensions_IncomingEvents
    {

        #region OnAddSignerRole (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever an AddSignerRole request was received.
        /// </summary>
        event OnAddSignerRoleRequestReceivedDelegate       OnAddSignerRoleRequestReceived;

        /// <summary>
        /// An event sent whenever a response to an AddSignerRole request was sent.
        /// </summary>
        event OnAddSignerRoleResponseReceivedDelegate      OnAddSignerRoleResponseReceived;

        #endregion

        #region OnUpdateSignerRole (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever an UpdateSignerRole request was received.
        /// </summary>
        event OnUpdateSignerRoleRequestReceivedDelegate       OnUpdateSignerRoleRequestReceived;

        /// <summary>
        /// An event sent whenever a response to an UpdateSignerRole request was sent.
        /// </summary>
        event OnUpdateSignerRoleResponseReceivedDelegate      OnUpdateSignerRoleResponseReceived;

        #endregion

        #region OnDeleteSignerRole (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever a DeleteSignerRole request was received.
        /// </summary>
        event OnDeleteSignerRoleRequestReceivedDelegate       OnDeleteSignerRoleRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a DeleteSignerRole request was sent.
        /// </summary>
        event OnDeleteSignerRoleResponseReceivedDelegate      OnDeleteSignerRoleResponseReceived;

        #endregion

        #region OnGetSignerRoles (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever a GetSignerRoles request was received.
        /// </summary>
        event OnGetSignerRolesRequestReceivedDelegate       OnGetSignerRolesRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a GetSignerRoles request was sent.
        /// </summary>
        event OnGetSignerRolesResponseReceivedDelegate      OnGetSignerRolesResponseReceived;

        #endregion

        #region OnSetSignaturePolicy (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever a SetSignaturePolicy request was received.
        /// </summary>
        event OnSetSignaturePolicyRequestReceivedDelegate       OnSetSignaturePolicyRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a SetSignaturePolicy request was sent.
        /// </summary>
        event OnSetSignaturePolicyResponseReceivedDelegate      OnSetSignaturePolicyResponseReceived;

        #endregion

        #region OnGetSignaturePolicy (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever a GetSignaturePolicy request was received.
        /// </summary>
        event OnGetSignaturePolicyRequestReceivedDelegate       OnGetSignaturePolicyRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a GetSignaturePolicy request was sent.
        /// </summary>
        event OnGetSignaturePolicyResponseReceivedDelegate      OnGetSignaturePolicyResponseReceived;

        #endregion


        #region OnSecureDataTransfer    (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever a SecureDataTransfer request was received.
        /// </summary>
        event OnSecureDataTransferRequestReceivedDelegate?      OnSecureDataTransferRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a SecureDataTransfer request was received.
        /// </summary>
        event OnSecureDataTransferResponseReceivedDelegate?     OnSecureDataTransferResponseReceived;

        #endregion

    }

}
