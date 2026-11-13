/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A node of OCPP 2.1 that leaves GetLog and UpdateFirmware to whoever
    /// moves its files.
    /// </summary>
    /// <remarks>
    /// The node itself has no file servers, no keys and no files: those are
    /// the WWCP node's it runs in, which WWCP_OCPP does not know. So it leaves
    /// the answer to the processor it is given - see DeviceModelBridge in
    /// WWCP_OCPP_Node - and sends what that tells of a transfer. Without a
    /// processor it answers as it did before.
    ///
    /// A processor and not one more handler of OnGetLog: the adapter asks the
    /// first handler of an event and no other, and the node's own comes first.
    /// </remarks>
    public interface IFileTransferNode
    {

        /// <summary>
        /// What answers a GetLog request; null where nothing does.
        /// </summary>
        OnGetLogDelegate?          GetLogProcessor            { get; set; }

        /// <summary>
        /// What answers an UpdateFirmware request; null where nothing does.
        /// </summary>
        OnUpdateFirmwareDelegate?  UpdateFirmwareProcessor    { get; set; }

        /// <summary>
        /// Tell the CSMS how the upload of a log goes: a LogStatusNotification.
        /// </summary>
        /// <param name="Status">How it goes.</param>
        /// <param name="LogRequestId">The id of the GetLog request it belongs to.</param>
        Task SendLogStatus(UploadLogStatus    Status,
                           Int32?             LogRequestId,
                           CancellationToken  CancellationToken   = default);

        /// <summary>
        /// Tell the CSMS how the download of a firmware image goes: a
        /// FirmwareStatusNotification.
        /// </summary>
        /// <param name="Status">How it goes.</param>
        /// <param name="UpdateFirmwareRequestId">The id of the UpdateFirmware request it belongs to.</param>
        Task SendFirmwareStatus(FirmwareStatus     Status,
                                Int32?             UpdateFirmwareRequestId,
                                CancellationToken  CancellationToken   = default);

    }

}
