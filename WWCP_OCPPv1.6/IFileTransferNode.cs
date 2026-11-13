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

using cloud.charging.open.protocols.OCPPv1_6.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A node of OCPP 1.6 that leaves GetLog, GetDiagnostics, UpdateFirmware
    /// and SignedUpdateFirmware to whoever moves its files.
    /// </summary>
    /// <remarks>
    /// The node itself has no file servers, no keys and no files: those are
    /// the WWCP node's it runs in, which WWCP_OCPP does not know. So it leaves
    /// the answer to the processor it is given - see DeviceModelBridge in
    /// WWCP_OCPP_Node - and sends what that tells of a transfer. Without a
    /// processor a request is not answered here at all, as before.
    /// </remarks>
    public interface IFileTransferNode
    {

        /// <summary>
        /// What answers a GetLog request; null where nothing does.
        /// </summary>
        OnGetLogDelegate?                GetLogProcessor                  { get; set; }

        /// <summary>
        /// What answers a GetDiagnostics request; null where nothing does.
        /// </summary>
        OnGetDiagnosticsDelegate?        GetDiagnosticsProcessor          { get; set; }

        /// <summary>
        /// What answers an UpdateFirmware request; null where nothing does.
        /// </summary>
        OnUpdateFirmwareDelegate?        UpdateFirmwareProcessor          { get; set; }

        /// <summary>
        /// What answers a SignedUpdateFirmware request; null where nothing does.
        /// </summary>
        OnSignedUpdateFirmwareDelegate?  SignedUpdateFirmwareProcessor    { get; set; }

        /// <summary>
        /// Tell the central system how the upload of a log goes: a
        /// LogStatusNotification.
        /// </summary>
        Task SendLogStatus(UploadLogStatus    Status,
                           Int32?             LogRequestId,
                           CancellationToken  CancellationToken   = default);

        /// <summary>
        /// Tell the central system how the upload of the diagnostics goes: a
        /// DiagnosticsStatusNotification.
        /// </summary>
        Task SendDiagnosticsStatus(DiagnosticsStatus  Status,
                                   CancellationToken  CancellationToken   = default);

        /// <summary>
        /// Tell the central system how the download of a firmware image goes:
        /// a FirmwareStatusNotification.
        /// </summary>
        Task SendFirmwareStatus(FirmwareStatus     Status,
                                CancellationToken  CancellationToken   = default);

        /// <summary>
        /// Tell the central system how the download of a signed firmware image
        /// goes: a SignedFirmwareStatusNotification.
        /// </summary>
        Task SendSignedFirmwareStatus(FirmwareStatus     Status,
                                      Int32?             UpdateRequestId,
                                      CancellationToken  CancellationToken   = default);

    }

}
