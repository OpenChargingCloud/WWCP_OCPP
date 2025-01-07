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

    public interface BinaryStreamsExtensions_OutgoingMessages
    {

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


    }

}
