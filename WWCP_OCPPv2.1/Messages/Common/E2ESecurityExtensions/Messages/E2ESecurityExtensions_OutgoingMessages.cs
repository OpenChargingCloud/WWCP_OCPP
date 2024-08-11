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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    public interface E2ESecurityExtensions_OutgoingMessages
    {

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
        /// Encrypt and send the given encapsulated security payload.
        /// </summary>
        /// <param name="Request">A SecureDataTransfer request.</param>
        Task<SecureDataTransferResponse> SecureDataTransfer(SecureDataTransferRequest Request);

        #endregion


    }

}
