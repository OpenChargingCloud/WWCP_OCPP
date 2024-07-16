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

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// The common interface of all outgoing OCPP messages
    /// from a charging station management system.
    /// </summary>
    public interface ICSMSOutgoingMessages
    {

        //// Binary Data Streams Extensions

        //#region BinaryDataTransfer          (Request)

        ///// <summary>
        ///// Send the given vendor-specific binary data.
        ///// </summary>
        ///// <param name="Request">A BinaryDataTransfer request.</param>
        //Task<BinaryDataTransferResponse> BinaryDataTransfer(BinaryDataTransferRequest Request);

        //#endregion

        //#region GetFile                     (Request)

        ///// <summary>
        ///// Request to download the specified file.
        ///// </summary>
        ///// <param name="Request">A GetFile request.</param>
        //Task<CS.GetFileResponse> GetFile(CSMS.GetFileRequest Request);

        //#endregion

        //#region SendFile                    (Request)

        ///// <summary>
        ///// Send the given file to the charging station.
        ///// </summary>
        ///// <param name="Request">A SendFile request.</param>
        //Task<CS.SendFileResponse> SendFile(CSMS.SendFileRequest Request);

        //#endregion

        //#region DeleteFile                  (Request)

        ///// <summary>
        ///// Delete the given file from the charging station.
        ///// </summary>
        ///// <param name="Request">A DeleteFile request.</param>
        //Task<CS.DeleteFileResponse> DeleteFile(CSMS.DeleteFileRequest Request);

        //#endregion

        //#region ListDirectory               (Request)

        ///// <summary>
        ///// List the given directory of the charging station or networking node.
        ///// </summary>
        ///// <param name="Request">A ListDirectory request.</param>
        //Task<CS.ListDirectoryResponse> ListDirectory(CSMS.ListDirectoryRequest Request);

        //#endregion


        //// E2E Security Extensions

        //#region AddSignaturePolicy          (Request)

        ///// <summary>
        ///// Add a signature policy.
        ///// </summary>
        ///// <param name="Request">An AddSignaturePolicy request.</param>
        //Task<CS.AddSignaturePolicyResponse> AddSignaturePolicy(CSMS.AddSignaturePolicyRequest Request);

        //#endregion

        //#region UpdateSignaturePolicy       (Request)

        ///// <summary>
        ///// Update a signature policy.
        ///// </summary>
        ///// <param name="Request">An UpdateSignaturePolicy request.</param>
        //Task<CS.UpdateSignaturePolicyResponse> UpdateSignaturePolicy(CSMS.UpdateSignaturePolicyRequest Request);

        //#endregion

        //#region DeleteSignaturePolicy       (Request)

        ///// <summary>
        ///// Delete a signature policy.
        ///// </summary>
        ///// <param name="Request">A DeleteSignaturePolicy request.</param>
        //Task<CS.DeleteSignaturePolicyResponse> DeleteSignaturePolicy(CSMS.DeleteSignaturePolicyRequest Request);

        //#endregion

        //#region AddUserRole                 (Request)

        ///// <summary>
        ///// Add a user role.
        ///// </summary>
        ///// <param name="Request">An AddUserRole request.</param>
        //Task<CS.AddUserRoleResponse> AddUserRole(CSMS.AddUserRoleRequest Request);

        //#endregion

        //#region UpdateUserRole              (Request)

        ///// <summary>
        ///// Update a user role.
        ///// </summary>
        ///// <param name="Request">An UpdateUserRole request.</param>
        //Task<CS.UpdateUserRoleResponse> UpdateUserRole(CSMS.UpdateUserRoleRequest Request);

        //#endregion

        //#region DeleteUserRole              (Request)

        ///// <summary>
        ///// Delete a user role.
        ///// </summary>
        ///// <param name="Request">An DeleteUserRole request.</param>
        //Task<CS.DeleteUserRoleResponse> DeleteUserRole(CSMS.DeleteUserRoleRequest Request);

        //#endregion


        //#region SecureDataTransfer          (Request)

        ///// <summary>
        ///// Encrypt and send the given encapsulated security payload.
        ///// </summary>
        ///// <param name="Request">A SecureDataTransfer request.</param>
        //Task<SecureDataTransferResponse> SecureDataTransfer(SecureDataTransferRequest Request);

        //#endregion


    }

}
