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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    public interface E2ESecurityExtensions_OutgoingMessages
    {

        #region AddSignerRole (Request)

        /// <summary>
        /// Give keys to a signer role that has none yet.
        /// </summary>
        /// <param name="Request">An AddSignerRole request.</param>
        Task<AddSignerRoleResponse> AddSignerRole(AddSignerRoleRequest Request);

        #endregion

        #region UpdateSignerRole (Request)

        /// <summary>
        /// Replace the keys of a signer role.
        /// </summary>
        /// <param name="Request">An UpdateSignerRole request.</param>
        Task<UpdateSignerRoleResponse> UpdateSignerRole(UpdateSignerRoleRequest Request);

        #endregion

        #region DeleteSignerRole (Request)

        /// <summary>
        /// Take the keys of a signer role away.
        /// </summary>
        /// <param name="Request">A DeleteSignerRole request.</param>
        Task<DeleteSignerRoleResponse> DeleteSignerRole(DeleteSignerRoleRequest Request);

        #endregion

        #region GetSignerRoles (Request)

        /// <summary>
        /// Ask for the role assignments a node has stored.
        /// </summary>
        /// <param name="Request">A GetSignerRoles request.</param>
        Task<GetSignerRolesResponse> GetSignerRoles(GetSignerRolesRequest Request);

        #endregion

        #region SetSignaturePolicy (Request)

        /// <summary>
        /// Set the policy table of a node.
        /// </summary>
        /// <param name="Request">A SetSignaturePolicy request.</param>
        Task<SetSignaturePolicyResponse> SetSignaturePolicy(SetSignaturePolicyRequest Request);

        #endregion

        #region GetSignaturePolicy (Request)

        /// <summary>
        /// Ask for the policy table a node has stored.
        /// </summary>
        /// <param name="Request">A GetSignaturePolicy request.</param>
        Task<GetSignaturePolicyResponse> GetSignaturePolicy(GetSignaturePolicyRequest Request);

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
