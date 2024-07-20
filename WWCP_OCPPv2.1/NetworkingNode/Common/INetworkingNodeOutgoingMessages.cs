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

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The common interface of all networking node outgoing messages.
    /// </summary>
    public interface INetworkingNodeOutgoingMessages
    {

        #region DataTransfer                (Request)

        /// <summary>
        /// Send the given vendor-specific data.
        /// </summary>
        /// <param name="Request">A data transfer request.</param>
        public Task<DataTransferResponse> DataTransfer(DataTransferRequest Request);

        #endregion


        // Binary Data Streams Extensions

        #region BinaryDataTransfer          (Request)

        /// <summary>
        /// Send the given vendor-specific binary data.
        /// </summary>
        /// <param name="Request">A BinaryDataTransfer request.</param>
        Task<BinaryDataTransferResponse> BinaryDataTransfer(BinaryDataTransferRequest Request);

        #endregion

        #region GetFile                     (Request)

        /// <summary>
        /// Request to download the specified file.
        /// </summary>
        /// <param name="Request">A GetFile request.</param>
        Task<GetFileResponse> GetFile(GetFileRequest Request);

        #endregion

        #region SendFile                    (Request)

        /// <summary>
        /// Send the given file to the charging station.
        /// </summary>
        /// <param name="Request">A SendFile request.</param>
        Task<SendFileResponse> SendFile(SendFileRequest Request);

        #endregion

        #region DeleteFile                  (Request)

        /// <summary>
        /// Delete the given file from the charging station.
        /// </summary>
        /// <param name="Request">A DeleteFile request.</param>
        Task<DeleteFileResponse> DeleteFile(DeleteFileRequest Request);

        #endregion

        #region ListDirectory               (Request)

        /// <summary>
        /// List the given directory of the charging station or networking node.
        /// </summary>
        /// <param name="Request">A ListDirectory request.</param>
        Task<ListDirectoryResponse> ListDirectory(ListDirectoryRequest Request);

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


        #region SecureDataTransfer          (Request)

        /// <summary>
        /// Send the given vendor-specific binary data.
        /// </summary>
        /// <param name="Request">A SecureDataTransfer request.</param>
        Task<SecureDataTransferResponse> SecureDataTransfer(SecureDataTransferRequest Request);

        #endregion


        // Networking Overlay Extensions

        #region NotifyNetworkTopology       (Request)

        /// <summary>
        /// Notify about the current network topology or a current change within the topology.
        /// </summary>
        /// <param name="Request">A reset request.</param>
        Task<NotifyNetworkTopologyResponse> NotifyNetworkTopology(NotifyNetworkTopologyRequest Request);

        #endregion

    }

}
