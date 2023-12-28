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

namespace cloud.charging.open.protocols.OCPP.NN.CS
{

    /// <summary>
    /// The common interface of all networking node incoming CS messages.
    /// </summary>
    public interface INetworkingNodeIncomingMessages : NN.INetworkingNodeIncomingMessages
    {

        #region OnGetFile               (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetFile request was received.
        /// </summary>
        //event OnGetFileDelegate  OnGetFile;

        #endregion

        #region OnSendFile              (Request/-Response)

        /// <summary>
        /// An event sent whenever a SendFile request was received.
        /// </summary>
        //event OnSendFileDelegate  OnSendFile;

        #endregion

        #region OnDeleteFile            (Request/-Response)

        /// <summary>
        /// An event sent whenever a DeleteFile request was received.
        /// </summary>
        //event OnDeleteFileDelegate  OnDeleteFile;

        #endregion

        #region OnListDirectory         (Request/-Response)

        /// <summary>
        /// An event sent whenever a ListDirectory request was received.
        /// </summary>
        //event OnListDirectoryDelegate  OnListDirectory;

        #endregion

    }

}
