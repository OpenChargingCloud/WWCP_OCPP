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
    /// The common interface of all incoming OCPP messages at a charging station.
    /// </summary>
    public interface ICSIncomingMessages
    {

        // Binary Data Streams Extensions

        #region OnIncomingBinaryDataTransfer

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was received.
        /// </summary>
        event OnIncomingBinaryDataTransferDelegate   OnIncomingBinaryDataTransfer;

        #endregion

        #region OnGetFile

        /// <summary>
        /// An event sent whenever a GetFile request was received.
        /// </summary>
        event OnGetFileDelegate                      OnGetFile;

        #endregion

        #region OnSendFile

        /// <summary>
        /// An event sent whenever a SendFile request was received.
        /// </summary>
        event OnSendFileDelegate                     OnSendFile;

        #endregion

        #region OnDeleteFile

        /// <summary>
        /// An event sent whenever a DeleteFile request was received.
        /// </summary>
        event OnDeleteFileDelegate                     OnDeleteFile;

        #endregion

        #region OnListDirectory

        /// <summary>
        /// An event sent whenever a ListDirectory request was received.
        /// </summary>
        event OnListDirectoryDelegate                  OnListDirectory;

        #endregion


        // E2E Security Extensions

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


    }

}
