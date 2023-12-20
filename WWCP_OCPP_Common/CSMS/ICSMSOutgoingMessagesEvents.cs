/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using cloud.charging.open.protocols.OCPP.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// The common interface of all events for outgoing OCPP messages
    /// from a charging station management system.
    /// </summary>
    public interface ICSMSOutgoingMessagesEvents
    {

        // Binary Data Streams Extensions

        #region BinaryDataTransfer             (Request/-Response)

        /// <summary>
        /// An event fired whenever a Reset request will be sent to a charging station.
        /// </summary>
        event OnBinaryDataTransferRequestDelegate?              OnBinaryDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a Reset request was received.
        /// </summary>
        event OnBinaryDataTransferResponseDelegate?             OnBinaryDataTransferResponse;

        #endregion

        #region GetFile                        (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetFile request will be sent to a charging station.
        /// </summary>
        event OnGetFileRequestDelegate?                         OnGetFileRequest;

        /// <summary>
        /// An event fired whenever a response to a GetFile request was received.
        /// </summary>
        event OnGetFileResponseDelegate?                        OnGetFileResponse;

        #endregion

        #region SendFile                       (Request/-Response)

        /// <summary>
        /// An event fired whenever a SendFile request will be sent to a charging station.
        /// </summary>
        event OnSendFileRequestDelegate?                        OnSendFileRequest;

        /// <summary>
        /// An event fired whenever a response to a SendFile request was received.
        /// </summary>
        event OnSendFileResponseDelegate?                       OnSendFileResponse;

        #endregion

        #region DeleteFile                     (Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteFile request will be sent to a charging station.
        /// </summary>
        event OnDeleteFileRequestDelegate?                      OnDeleteFileRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteFile request was received.
        /// </summary>
        event OnDeleteFileResponseDelegate?                     OnDeleteFileResponse;

        #endregion

        #region ListDirectory                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a ListDirectory request will be sent to a charging station.
        /// </summary>
        event OnListDirectoryRequestDelegate?                   OnListDirectoryRequest;

        /// <summary>
        /// An event fired whenever a response to a ListDirectory request was received.
        /// </summary>
        event OnListDirectoryResponseDelegate?                  OnListDirectoryResponse;

        #endregion


        // E2E Security Extensions

        #region AddSignaturePolicy             (Request/-Response)

        /// <summary>
        /// An event fired whenever an AddSignaturePolicy request will be sent to a charging station.
        /// </summary>
        event OnAddSignaturePolicyRequestDelegate?              OnAddSignaturePolicyRequest;

        /// <summary>
        /// An event fired whenever a response to an AddSignaturePolicy request was received.
        /// </summary>
        event OnAddSignaturePolicyResponseDelegate?             OnAddSignaturePolicyResponse;

        #endregion

        #region UpdateSignaturePolicy          (Request/-Response)

        /// <summary>
        /// An event fired whenever an UpdateSignaturePolicy request will be sent to a charging station.
        /// </summary>
        event OnUpdateSignaturePolicyRequestDelegate?           OnUpdateSignaturePolicyRequest;

        /// <summary>
        /// An event fired whenever a response to an UpdateSignaturePolicy request was received.
        /// </summary>
        event OnUpdateSignaturePolicyResponseDelegate?          OnUpdateSignaturePolicyResponse;

        #endregion

        #region DeleteSignaturePolicy          (Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteSignaturePolicy request will be sent to a charging station.
        /// </summary>
        event OnDeleteSignaturePolicyRequestDelegate?           OnDeleteSignaturePolicyRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteSignaturePolicy request was received.
        /// </summary>
        event OnDeleteSignaturePolicyResponseDelegate?          OnDeleteSignaturePolicyResponse;

        #endregion

        #region AddUserRole                    (Request/-Response)

        /// <summary>
        /// An event fired whenever an AddUserRole request will be sent to a charging station.
        /// </summary>
        event OnAddUserRoleRequestDelegate?                     OnAddUserRoleRequest;

        /// <summary>
        /// An event fired whenever a response to an AddUserRole request was received.
        /// </summary>
        event OnAddUserRoleResponseDelegate?                    OnAddUserRoleResponse;

        #endregion

        #region UpdateUserRole                 (Request/-Response)

        /// <summary>
        /// An event fired whenever an UpdateUserRole request will be sent to a charging station.
        /// </summary>
        event OnUpdateUserRoleRequestDelegate?                  OnUpdateUserRoleRequest;

        /// <summary>
        /// An event fired whenever a response to an UpdateUserRole request was received.
        /// </summary>
        event OnUpdateUserRoleResponseDelegate?                 OnUpdateUserRoleResponse;

        #endregion

        #region DeleteUserRole                 (Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteUserRole request will be sent to a charging station.
        /// </summary>
        event OnDeleteUserRoleRequestDelegate?                  OnDeleteUserRoleRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteUserRole request was received.
        /// </summary>
        event OnDeleteUserRoleResponseDelegate?                 OnDeleteUserRoleResponse;

        #endregion


    }

}
