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

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.SecureShell;

using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// GetLog and UpdateFirmware of OCPP 2.1 with an "sftp://" location,
    /// answered by the node's file transfers, and the security events of the
    /// white paper "SFTP for Logfile Uploads and Firmware Downloads".
    /// </summary>
    /// <remarks>
    /// What OCPP adds to <see cref="FileTransfers"/> is its words: the status
    /// of a response and its reason code, and the notifications a transfer is
    /// told by. What a log or a firmware image is, is neither's: the logic
    /// above provides the file to upload and takes the one that was downloaded
    /// (<see cref="FileTransfers.FileProvider"/>, <see cref="FileTransfers.FileConsumer"/>).
    /// </remarks>
    public sealed partial class DeviceModelBridge
    {

        #region Data

        /// <summary>
        /// A request named a file server, a login or a path the configuration
        /// does not allow.
        /// </summary>
        public const String  FileTransferRefused        = "FileTransferRefused";

        /// <summary>
        /// A file server showed a host key that neither a pin nor a CA vouches
        /// for.
        /// </summary>
        public const String  InvalidFileServerHostKey   = "InvalidFileServerHostKey";

        /// <summary>
        /// What the log and a monitor file these events under.
        /// </summary>
        public const String  FileTransferService        = "filetransfer";

        private FileTransfers?  fileTransfers;

        #endregion

        #region FileTransfers

        /// <summary>
        /// The file transfers the node makes on requests of OCPP.
        /// </summary>
        public FileTransfers FileTransfers
        {
            get
            {
                lock (componentsLock)
                {

                    if (fileTransfers is not null)
                        return fileTransfers;

                    fileTransfers = new FileTransfers(node);

                    // Somebody with the right to send requests asked for a place the
                    // configuration does not know; and a file server that is not the one
                    // the node holds. Both are for the CSMS to hear of.
                    node.OnFileTransferRefused += (judgement, direction) =>
                        TellCSMS(FileTransferRefused,
                                 $"{(direction == FileTransferDirections.Upload ? "An upload to" : "A download from")} " +
                                 $"{judgement.Location?.WithoutUserInfo ?? "a location"} was refused: {judgement.Why}.",
                                 FileTransferService);

                    node.OnFileServerHostKeyRefused += (server, shown) =>
                        TellCSMS(InvalidFileServerHostKey,
                                 $"The file server '{server.Id}' ({server.Host}:{server.Port}) showed the SSH host key {shown}, which neither a pin nor a CA of it vouches for: refused.",
                                 FileTransferService);

                    return fileTransfers;

                }
            }
        }

        #endregion

        #region (private) ServeFileTransfers(Node)

        /// <summary>
        /// Leave GetLog and UpdateFirmware of the given node of OCPP 2.1 to the
        /// node's file transfers, where it is one that leaves them to somebody.
        /// </summary>
        private void ServeFileTransfers(OCPPv2_1.IDeviceModelNode Node)
        {

            _ = FileTransfers;

            if (Node is not OCPPv2_1.IFileTransferNode fileTransferNode)
                return;

            fileTransferNode.GetLogProcessor          = (timestamp, sender, connection, request, cancellationToken)
                                                            => GetLog        (request, status => fileTransferNode.SendLogStatus     (status, request.LogRequestId),            cancellationToken);

            fileTransferNode.UpdateFirmwareProcessor  = (timestamp, sender, connection, request, cancellationToken)
                                                            => UpdateFirmware(request, status => fileTransferNode.SendFirmwareStatus(status, request.UpdateFirmwareRequestId), cancellationToken);

        }

        #endregion


        #region GetLog(Request, Tell, CancellationToken = default)

        /// <summary>
        /// Answer a GetLog request of OCPP 2.1, and upload the log after the
        /// answer.
        /// </summary>
        /// <param name="Request">The GetLog request.</param>
        /// <param name="Tell">Told how the upload goes: the LogStatusNotifications.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        public async Task<OCPPv2_1.CS.GetLogResponse> GetLog(OCPPv2_1.CSMS.GetLogRequest               Request,
                                                             Func<OCPPv2_1.UploadLogStatus, Task>      Tell,
                                                             CancellationToken                         CancellationToken   = default)
        {

            node.Log.Info($"'{Request.NetworkPath.Source}' asked this {node.Kind.Name} over OCPP for its {Request.LogType} (request {Request.LogRequestId}).",
                          "ocpp", "sftp", FileTransferService);

            var answer = await FileTransfers.StartUpload(
                                   Request.Log.RemoteLocation.ToString(),
                                   new FileRequest(Request.LogType.ToString(),
                                                   Request.LogRequestId,
                                                   Request.Log.OldestTimestamp,
                                                   Request.Log.LatestTimestamp),
                                   Request.Retries,
                                   Request.RetryInterval,
                                   state => Tell(state switch {
                                                     UploadState.Uploading              => OCPPv2_1.UploadLogStatus.Uploading,
                                                     UploadState.Uploaded               => OCPPv2_1.UploadLogStatus.Uploaded,
                                                     UploadState.PermissionDenied       => OCPPv2_1.UploadLogStatus.PermissionDenied,
                                                     UploadState.NotSupportedOperation  => OCPPv2_1.UploadLogStatus.NotSupportedOperation,
                                                     UploadState.AcceptedCanceled       => OCPPv2_1.UploadLogStatus.AcceptedCanceled,
                                                     _                                  => OCPPv2_1.UploadLogStatus.UploadFailure
                                                 }),
                                   CancellationToken
                               ).ConfigureAwait(false);

            return answer.Accepted

                       ? new OCPPv2_1.CS.GetLogResponse(
                             Request,
                             answer.ReplacedAnother
                                 ? OCPPv2_1.LogStatus.AcceptedCanceled
                                 : OCPPv2_1.LogStatus.Accepted,
                             Filename:    answer.FileName
                         )

                       : new OCPPv2_1.CS.GetLogResponse(
                             Request,
                             OCPPv2_1.LogStatus.Rejected,
                             StatusInfo:  new OCPPv2_1.StatusInfo(answer.ReasonCode ?? FileTransfers.NotSupported, answer.Info)
                         );

        }

        #endregion

        #region UpdateFirmware(Request, Tell, CancellationToken = default)

        /// <summary>
        /// Answer an UpdateFirmware request of OCPP 2.1, and download the
        /// image after the answer.
        /// </summary>
        /// <remarks>
        /// Downloaded is the last thing told here. Whether the image is
        /// genuine, and installing it, is for whoever takes the file - and so
        /// is every status after it.
        /// </remarks>
        /// <param name="Request">The UpdateFirmware request.</param>
        /// <param name="Tell">Told how the download goes: the FirmwareStatusNotifications.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        public Task<OCPPv2_1.CS.UpdateFirmwareResponse> UpdateFirmware(OCPPv2_1.CSMS.UpdateFirmwareRequest     Request,
                                                                       Func<OCPPv2_1.FirmwareStatus, Task>     Tell,
                                                                       CancellationToken                       CancellationToken   = default)
        {

            node.Log.Info($"'{Request.NetworkPath.Source}' asked this {node.Kind.Name} over OCPP to fetch a firmware image (request {Request.UpdateFirmwareRequestId}).",
                          "ocpp", "sftp", FileTransferService);

            var answer = FileTransfers.StartDownload(
                             Request.Firmware.FirmwareURL.ToString(),
                             Request.UpdateFirmwareRequestId,
                             Request,
                             Request.Firmware.RetrieveTimestamp,
                             Request.Retries,
                             Request.RetryInterval,
                             state => Tell(state switch {
                                               DownloadState.DownloadScheduled  => OCPPv2_1.FirmwareStatus.DownloadScheduled,
                                               DownloadState.Downloading        => OCPPv2_1.FirmwareStatus.Downloading,
                                               DownloadState.DownloadPaused     => OCPPv2_1.FirmwareStatus.DownloadPaused,
                                               DownloadState.Downloaded         => OCPPv2_1.FirmwareStatus.Downloaded,
                                               _                                => OCPPv2_1.FirmwareStatus.DownloadFailed
                                           })
                         );

            return Task.FromResult(

                       answer.Accepted

                           ? new OCPPv2_1.CS.UpdateFirmwareResponse(
                                 Request,
                                 answer.ReplacedAnother
                                     ? OCPPv2_1.UpdateFirmwareStatus.AcceptedCanceled
                                     : OCPPv2_1.UpdateFirmwareStatus.Accepted
                             )

                           : new OCPPv2_1.CS.UpdateFirmwareResponse(
                                 Request,
                                 OCPPv2_1.UpdateFirmwareStatus.Rejected,
                                 new OCPPv2_1.StatusInfo(answer.ReasonCode ?? FileTransfers.NotSupported, answer.Info)
                             )

                   );

        }

        #endregion

    }

}
