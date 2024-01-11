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

namespace cloud.charging.open.protocols.OCPP.CS
{

    /// <summary>
    /// The common interface of all events for incoming OCPP messages at a charging station.
    /// </summary>
    public interface ICSIncomingMessagesEvents
    {

        // Binary Data Streams Extensions

        #region OnIncomingBinaryDataTransfer

        /// <summary>
        /// An event sent whenever an IncomingBinaryDataTransfer request was received.
        /// </summary>
        event OnBinaryDataTransferRequestReceivedDelegate     OnIncomingBinaryDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to an IncomingBinaryDataTransfer request was sent.
        /// </summary>
        event OnBinaryDataTransferResponseSentDelegate    OnIncomingBinaryDataTransferResponse;

        #endregion

        #region OnGetFile

        /// <summary>
        /// An event sent whenever a GetFile request was received.
        /// </summary>
        event OnGetFileRequestDelegate                        OnGetFileRequest;

        /// <summary>
        /// An event sent whenever a response to a GetFile request was sent.
        /// </summary>
        event OnGetFileResponseDelegate                       OnGetFileResponse;

        #endregion

        #region OnSendFile

        /// <summary>
        /// An event sent whenever a SendFile request was received.
        /// </summary>
        event OnSendFileRequestDelegate                       OnSendFileRequest;

        /// <summary>
        /// An event sent whenever a response to a SendFile request was sent.
        /// </summary>
        event OnSendFileResponseDelegate                      OnSendFileResponse;

        #endregion

        #region OnDeleteFile

        /// <summary>
        /// An event sent whenever a DeleteFile request was received.
        /// </summary>
        event OnDeleteFileRequestDelegate                     OnDeleteFileRequest;

        /// <summary>
        /// An event sent whenever a response to a DeleteFile request was sent.
        /// </summary>
        event OnDeleteFileResponseDelegate                    OnDeleteFileResponse;

        #endregion


        // E2E Security Extensions

        #region OnAddSignaturePolicy

        /// <summary>
        /// An event sent whenever an AddSignaturePolicy request was received.
        /// </summary>
        event OnAddSignaturePolicyRequestDelegate             OnAddSignaturePolicyRequest;

        /// <summary>
        /// An event sent whenever a response to an AddSignaturePolicy request was sent.
        /// </summary>
        event OnAddSignaturePolicyResponseDelegate            OnAddSignaturePolicyResponse;

        #endregion

        #region OnUpdateSignaturePolicy

        /// <summary>
        /// An event sent whenever an UpdateSignaturePolicy request was received.
        /// </summary>
        event OnUpdateSignaturePolicyRequestDelegate          OnUpdateSignaturePolicyRequest;

        /// <summary>
        /// An event sent whenever a response to an UpdateSignaturePolicy request was sent.
        /// </summary>
        event OnUpdateSignaturePolicyResponseDelegate         OnUpdateSignaturePolicyResponse;

        #endregion

        #region OnDeleteSignaturePolicy

        /// <summary>
        /// An event sent whenever a DeleteSignaturePolicy request was received.
        /// </summary>
        event OnDeleteSignaturePolicyRequestDelegate          OnDeleteSignaturePolicyRequest;

        /// <summary>
        /// An event sent whenever a response to a DeleteSignaturePolicy request was sent.
        /// </summary>
        event OnDeleteSignaturePolicyResponseDelegate         OnDeleteSignaturePolicyResponse;

        #endregion

        #region OnAddUserRole

        /// <summary>
        /// An event sent whenever an AddUserRole request was received.
        /// </summary>
        event OnAddUserRoleRequestDelegate                    OnAddUserRoleRequest;

        /// <summary>
        /// An event sent whenever a response to an AddUserRole request was sent.
        /// </summary>
        event OnAddUserRoleResponseDelegate                   OnAddUserRoleResponse;

        #endregion

        #region OnUpdateUserRole

        /// <summary>
        /// An event sent whenever an UpdateUserRole request was received.
        /// </summary>
        event OnUpdateUserRoleRequestDelegate                 OnUpdateUserRoleRequest;

        /// <summary>
        /// An event sent whenever a response to an UpdateUserRole request was sent.
        /// </summary>
        event OnUpdateUserRoleResponseDelegate                OnUpdateUserRoleResponse;

        #endregion

        #region OnDeleteUserRole

        /// <summary>
        /// An event sent whenever a DeleteUserRole request was received.
        /// </summary>
        event OnDeleteUserRoleRequestDelegate                 OnDeleteUserRoleRequest;

        /// <summary>
        /// An event sent whenever a response to a DeleteUserRole request was sent.
        /// </summary>
        event OnDeleteUserRoleResponseDelegate                OnDeleteUserRoleResponse;

        #endregion


    }

}
