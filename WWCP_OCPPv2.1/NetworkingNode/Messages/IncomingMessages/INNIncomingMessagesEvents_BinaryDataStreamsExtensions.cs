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

    public interface INetworkingNode_IncomingMessagesEvents_BinaryDataStreamsExtensions
    {

        #region OnBinaryDataTransfer (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request was received.
        /// </summary>
        event OnBinaryDataTransferRequestReceivedDelegate?     OnBinaryDataTransferRequestReceived;

        /// <summary>
        /// An event fired whenever a response to a BinaryDataTransfer request was received.
        /// </summary>
        event OnBinaryDataTransferResponseReceivedDelegate?    OnBinaryDataTransferResponseReceived;

        #endregion

        #region OnDeleteFile         (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever a DeleteFile request was received.
        /// </summary>
        event OnDeleteFileRequestReceivedDelegate           OnDeleteFileRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a DeleteFile request was sent.
        /// </summary>
        event OnDeleteFileResponseReceivedDelegate          OnDeleteFileResponseReceived;

        #endregion

        #region OnGetFile            (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever a GetFile request was received.
        /// </summary>
        event OnGetFileRequestReceivedDelegate              OnGetFileRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a GetFile request was sent.
        /// </summary>
        event OnGetFileResponseReceivedDelegate             OnGetFileResponseReceived;

        #endregion

        #region OnListDirectory      (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever a ListDirectory request was received.
        /// </summary>
        event OnListDirectoryRequestReceivedDelegate        OnListDirectoryRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a ListDirectory request was sent.
        /// </summary>
        event OnListDirectoryResponseReceivedDelegate       OnListDirectoryResponseReceived;

        #endregion

        #region OnSendFile           (RequestReceived/-ResponseSent)

        /// <summary>
        /// An event sent whenever a SendFile request was received.
        /// </summary>
        event OnSendFileRequestReceivedDelegate             OnSendFileRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a SendFile request was sent.
        /// </summary>
        event OnSendFileResponseReceivedDelegate            OnSendFileResponseReceived;

        #endregion

    }

}
