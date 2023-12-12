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

using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.CSMS;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    public interface ICSMSClient
    {


        // Binary Data Streams Extensions

        #region TransferBinaryData          (Request)

        /// <summary>
        /// Send the given vendor-specific binary data.
        /// </summary>
        /// <param name="Request">A binary data transfer request.</param>
        Task<OCPP.CS.BinaryDataTransferResponse> BinaryDataTransfer(OCPP.CSMS.BinaryDataTransferRequest Request);

        #endregion

        #region GetFile                     (Request)

        /// <summary>
        /// Request to download the specified file.
        /// </summary>
        /// <param name="Request">A GetFile request.</param>
        Task<OCPP.CS.GetFileResponse> GetFile(GetFileRequest Request);

        #endregion

        #region SendFile                    (Request)

        /// <summary>
        /// Send the given file to the charging station.
        /// </summary>
        /// <param name="Request">A SendFile request.</param>
        Task<OCPP.CS.SendFileResponse> SendFile(SendFileRequest Request);

        #endregion

        #region DeleteFile                  (Request)

        /// <summary>
        /// Delete the given file from the charging station.
        /// </summary>
        /// <param name="Request">A DeleteFile request.</param>
        Task<OCPP.CS.DeleteFileResponse> DeleteFile(DeleteFileRequest Request);

        #endregion


        // E2E Security Extensions

        #region AddSignaturePolicy          (Request)

        /// <summary>
        /// Add a signature policy.
        /// </summary>
        /// <param name="Request">An AddSignaturePolicy request.</param>
        Task<AddSignaturePolicyResponse> AddSignaturePolicy(AddSignaturePolicyRequest Request);

        #endregion

        #region UpdateSignaturePolicy       (Request)

        /// <summary>
        /// Update a signature policy.
        /// </summary>
        /// <param name="Request">An UpdateSignaturePolicy request.</param>
        Task<UpdateSignaturePolicyResponse> UpdateSignaturePolicy(UpdateSignaturePolicyRequest Request);

        #endregion

        #region DeleteSignaturePolicy       (Request)

        /// <summary>
        /// Delete a signature policy.
        /// </summary>
        /// <param name="Request">A DeleteSignaturePolicy request.</param>
        Task<DeleteSignaturePolicyResponse> DeleteSignaturePolicy(DeleteSignaturePolicyRequest Request);

        #endregion

        #region AddUserRole                 (Request)

        /// <summary>
        /// Add a user role.
        /// </summary>
        /// <param name="Request">An AddUserRole request.</param>
        Task<AddUserRoleResponse> AddUserRole(AddUserRoleRequest Request);

        #endregion

        #region UpdateUserRole              (Request)

        /// <summary>
        /// Update a user role.
        /// </summary>
        /// <param name="Request">An UpdateUserRole request.</param>
        Task<UpdateUserRoleResponse> UpdateUserRole(UpdateUserRoleRequest Request);

        #endregion

        #region DeleteUserRole              (Request)

        /// <summary>
        /// Delete a user role.
        /// </summary>
        /// <param name="Request">An DeleteUserRole request.</param>
        Task<DeleteUserRoleResponse> DeleteUserRole(DeleteUserRoleRequest Request);

        #endregion

    }

}
