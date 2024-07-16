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

    public interface INetworkingNode_IncomingMessages_E2ESecurityExtensions
    {

        #region OnAddSignaturePolicy

        /// <summary>
        /// An event sent whenever an AddSignaturePolicy request was received.
        /// </summary>
        event OnAddSignaturePolicyDelegate            OnAddSignaturePolicy;

        #endregion

        #region OnUpdateSignaturePolicy

        /// <summary>
        /// An event sent whenever an UpdateSignaturePolicy request was received.
        /// </summary>
        event OnUpdateSignaturePolicyDelegate         OnUpdateSignaturePolicy;

        #endregion

        #region OnDeleteSignaturePolicy

        /// <summary>
        /// An event sent whenever a DeleteSignaturePolicy request was received.
        /// </summary>
        event OnDeleteSignaturePolicyDelegate         OnDeleteSignaturePolicy;

        #endregion

        #region OnAddUserRole

        /// <summary>
        /// An event sent whenever an AddUserRole request was received.
        /// </summary>
        event OnAddUserRoleDelegate                   OnAddUserRole;

        #endregion

        #region OnUpdateUserRole

        /// <summary>
        /// An event sent whenever an UpdateUserRole request was received.
        /// </summary>
        event OnUpdateUserRoleDelegate                OnUpdateUserRole;

        #endregion

        #region OnDeleteUserRole

        /// <summary>
        /// An event sent whenever a DeleteUserRole request was received.
        /// </summary>
        event OnDeleteUserRoleDelegate                OnDeleteUserRole;

        #endregion


        #region OnSecureDataTransfer

        /// <summary>
        /// An event sent whenever a SecureDataTransfer request was received.
        /// </summary>
        event OnSecureDataTransferDelegate            OnSecureDataTransfer;

        #endregion

    }


    public interface INetworkingNode_IncomingMessagesEvents_E2ESecurityExtensions
    {

        #region OnAddSignaturePolicy    (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever an AddSignaturePolicy request was received.
        /// </summary>
        event OnAddSignaturePolicyRequestReceivedDelegate       OnAddSignaturePolicyRequestReceived;

        /// <summary>
        /// An event sent whenever a response to an AddSignaturePolicy request was sent.
        /// </summary>
        event OnAddSignaturePolicyResponseReceivedDelegate      OnAddSignaturePolicyResponseReceived;

        #endregion

        #region OnUpdateSignaturePolicy (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever an UpdateSignaturePolicy request was received.
        /// </summary>
        event OnUpdateSignaturePolicyRequestReceivedDelegate    OnUpdateSignaturePolicyRequestReceived;

        /// <summary>
        /// An event sent whenever a response to an UpdateSignaturePolicy request was sent.
        /// </summary>
        event OnUpdateSignaturePolicyResponseReceivedDelegate   OnUpdateSignaturePolicyResponseReceived;

        #endregion

        #region OnDeleteSignaturePolicy (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever a DeleteSignaturePolicy request was received.
        /// </summary>
        event OnDeleteSignaturePolicyRequestReceivedDelegate    OnDeleteSignaturePolicyRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a DeleteSignaturePolicy request was sent.
        /// </summary>
        event OnDeleteSignaturePolicyResponseReceivedDelegate   OnDeleteSignaturePolicyResponseReceived;

        #endregion

        #region OnAddUserRole           (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever an AddUserRole request was received.
        /// </summary>
        event OnAddUserRoleRequestReceivedDelegate              OnAddUserRoleRequestReceived;

        /// <summary>
        /// An event sent whenever a response to an AddUserRole request was sent.
        /// </summary>
        event OnAddUserRoleResponseReceivedDelegate             OnAddUserRoleResponseReceived;

        #endregion

        #region OnUpdateUserRole        (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever an UpdateUserRole request was received.
        /// </summary>
        event OnUpdateUserRoleRequestReceivedDelegate           OnUpdateUserRoleRequestReceived;

        /// <summary>
        /// An event sent whenever a response to an UpdateUserRole request was sent.
        /// </summary>
        event OnUpdateUserRoleResponseReceivedDelegate          OnUpdateUserRoleResponseReceived;

        #endregion

        #region OnDeleteUserRole        (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever a DeleteUserRole request was received.
        /// </summary>
        event OnDeleteUserRoleRequestReceivedDelegate           OnDeleteUserRoleRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a DeleteUserRole request was sent.
        /// </summary>
        event OnDeleteUserRoleResponseReceivedDelegate          OnDeleteUserRoleResponseReceived;

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
