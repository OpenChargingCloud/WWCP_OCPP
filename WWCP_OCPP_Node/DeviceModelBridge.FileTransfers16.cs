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

using OCPPv1_6 = cloud.charging.open.protocols.OCPPv1_6;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// GetLog, GetDiagnostics, UpdateFirmware and SignedUpdateFirmware of OCPP
    /// 1.6 with an "sftp://" location, answered by the node's file transfers
    /// as those of OCPP 2.1 are.
    /// </summary>
    /// <remarks>
    /// OCPP 1.6 has fewer words. A response has no status info, so a refusal
    /// is Rejected and nothing more. GetDiagnostics and UpdateFirmware have no
    /// status in their responses at all: a refusal is the response without a
    /// file name, or the empty one, and then the notification that the upload
    /// or the download failed (section 5.1 of the white paper "SFTP for
    /// Logfile Uploads and Firmware Downloads"). And where 1.6 has no word for
    /// a state - an upload that another request cancelled, a plain download
    /// that waits - it is told as a failure, or not told.
    /// </remarks>
    public sealed partial class DeviceModelBridge
    {

        #region (private) ServeFileTransfers(ChargePoint)

        /// <summary>
        /// Leave the four requests of the given node of OCPP 1.6 to the node's
        /// file transfers, where it is one that leaves them to somebody.
        /// </summary>
        private void ServeFileTransfers(OCPPv1_6.CP.IConfigurationKeysNode ChargePoint)
        {

            _ = FileTransfers;

            if (ChargePoint is not OCPPv1_6.IFileTransferNode fileTransferNode)
                return;

            fileTransferNode.GetLogProcessor                = (timestamp, sender, connection, request, cancellationToken)
                                                                  => GetLog              (request, status => fileTransferNode.SendLogStatus           (status, request.LogRequestId),    cancellationToken);

            fileTransferNode.GetDiagnosticsProcessor        = (timestamp, sender, connection, request, cancellationToken)
                                                                  => GetDiagnostics      (request, status => fileTransferNode.SendDiagnosticsStatus   (status),                          cancellationToken);

            fileTransferNode.UpdateFirmwareProcessor        = (timestamp, sender, connection, request, cancellationToken)
                                                                  => UpdateFirmware      (request, status => fileTransferNode.SendFirmwareStatus      (status),                          cancellationToken);

            fileTransferNode.SignedUpdateFirmwareProcessor  = (timestamp, sender, connection, request, cancellationToken)
                                                                  => SignedUpdateFirmware(request, status => fileTransferNode.SendSignedFirmwareStatus(status, request.UpdateRequestId), cancellationToken);

        }

        #endregion

        #region (private) AfterTheResponse(Tell)

        /// <summary>
        /// Tell something of a request whose response has no place for it,
        /// once the response is on its way.
        /// </summary>
        private void AfterTheResponse(Func<Task> Tell)
        {
            _ = Task.Run(async () => {
                try
                {
                    // The response is written by whoever called the processor, right after it returns.
                    await Task.Delay(TimeSpan.FromMilliseconds(250)).ConfigureAwait(false);
                    await Tell().ConfigureAwait(false);
                }
                catch (Exception problem)
                {
                    node.Log.Warning($"The central system could not be told of a file transfer that was refused: {problem.Message}", "ocpp", "sftp", FileTransferService);
                }
            });
        }

        #endregion


        #region GetLog(Request, Tell, CancellationToken = default)

        /// <summary>
        /// Answer a GetLog request of OCPP 1.6, and upload the log after the
        /// answer.
        /// </summary>
        /// <param name="Request">The GetLog request.</param>
        /// <param name="Tell">Told how the upload goes: the LogStatusNotifications.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        public async Task<OCPPv1_6.CP.GetLogResponse> GetLog(OCPPv1_6.CS.GetLogRequest                 Request,
                                                             Func<OCPPv1_6.UploadLogStatus, Task>      Tell,
                                                             CancellationToken                         CancellationToken   = default)
        {

            node.Log.Info($"'{Request.NetworkPath.Source}' asked this {node.Kind.Name} over OCPP 1.6 for its {Request.LogType} (request {Request.LogRequestId}).",
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
                                                     UploadState.Uploading              => OCPPv1_6.UploadLogStatus.Uploading,
                                                     UploadState.Uploaded               => OCPPv1_6.UploadLogStatus.Uploaded,
                                                     UploadState.PermissionDenied       => OCPPv1_6.UploadLogStatus.PermissionDenied,
                                                     UploadState.NotSupportedOperation  => OCPPv1_6.UploadLogStatus.NotSupportedOperation,
                                                     // 1.6 has no word for an upload another request cancelled.
                                                     _                                  => OCPPv1_6.UploadLogStatus.UploadFailure
                                                 }),
                                   CancellationToken
                               ).ConfigureAwait(false);

            return answer.Accepted

                       ? new OCPPv1_6.CP.GetLogResponse(
                             Request,
                             answer.ReplacedAnother
                                 ? OCPPv1_6.LogStatus.AcceptedCanceled
                                 : OCPPv1_6.LogStatus.Accepted,
                             answer.FileName
                         )

                       : new OCPPv1_6.CP.GetLogResponse(
                             Request,
                             OCPPv1_6.LogStatus.Rejected
                         );

        }

        #endregion

        #region GetDiagnostics(Request, Tell, CancellationToken = default)

        /// <summary>
        /// Answer a GetDiagnostics request of OCPP 1.6, and upload the
        /// diagnostics after the answer.
        /// </summary>
        /// <remarks>
        /// The response has no status: where the request is refused, it names
        /// no file, and the upload is told as failed.
        /// </remarks>
        /// <param name="Request">The GetDiagnostics request.</param>
        /// <param name="Tell">Told how the upload goes: the DiagnosticsStatusNotifications.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        public async Task<OCPPv1_6.CP.GetDiagnosticsResponse> GetDiagnostics(OCPPv1_6.CS.GetDiagnosticsRequest             Request,
                                                                             Func<OCPPv1_6.DiagnosticsStatus, Task>        Tell,
                                                                             CancellationToken                             CancellationToken   = default)
        {

            node.Log.Info($"'{Request.NetworkPath.Source}' asked this {node.Kind.Name} over OCPP 1.6 for its diagnostics.",
                          "ocpp", "sftp", FileTransferService);

            var answer = await FileTransfers.StartUpload(
                                   Request.Location,
                                   new FileRequest("Diagnostics",
                                                   null,
                                                   Request.StartTime,
                                                   Request.StopTime),
                                   Request.Retries,
                                   Request.RetryInterval,
                                   state => Tell(state switch {
                                                     UploadState.Uploading  => OCPPv1_6.DiagnosticsStatus.Uploading,
                                                     UploadState.Uploaded   => OCPPv1_6.DiagnosticsStatus.Uploaded,
                                                     _                      => OCPPv1_6.DiagnosticsStatus.UploadFailed
                                                 }),
                                   CancellationToken
                               ).ConfigureAwait(false);

            if (!answer.Accepted)
                AfterTheResponse(() => Tell(OCPPv1_6.DiagnosticsStatus.UploadFailed));

            return new OCPPv1_6.CP.GetDiagnosticsResponse(
                       Request,
                       answer.Accepted
                           ? answer.FileName
                           : null
                   );

        }

        #endregion

        #region UpdateFirmware(Request, Tell, CancellationToken = default)

        /// <summary>
        /// Answer an UpdateFirmware request of OCPP 1.6, and download the image
        /// after the answer.
        /// </summary>
        /// <remarks>
        /// The response is empty: where the request is refused, the download
        /// is told as failed. The image has no signature; whether it is taken
        /// at all is for whoever takes the file.
        /// </remarks>
        /// <param name="Request">The UpdateFirmware request.</param>
        /// <param name="Tell">Told how the download goes: the FirmwareStatusNotifications.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        public Task<OCPPv1_6.CP.UpdateFirmwareResponse> UpdateFirmware(OCPPv1_6.CS.UpdateFirmwareRequest       Request,
                                                                       Func<OCPPv1_6.FirmwareStatus, Task>     Tell,
                                                                       CancellationToken                       CancellationToken   = default)
        {

            node.Log.Info($"'{Request.NetworkPath.Source}' asked this {node.Kind.Name} over OCPP 1.6 to fetch a firmware image.",
                          "ocpp", "sftp", FileTransferService);

            var answer = FileTransfers.StartDownload(
                             Request.FirmwareURL.ToString(),
                             null,
                             Request,
                             Request.RetrieveTimestamp,
                             Request.Retries,
                             Request.RetryInterval,
                             state => state switch {
                                          DownloadState.Downloading     => Tell(OCPPv1_6.FirmwareStatus.Downloading),
                                          DownloadState.Downloaded      => Tell(OCPPv1_6.FirmwareStatus.Downloaded),
                                          DownloadState.DownloadFailed  => Tell(OCPPv1_6.FirmwareStatus.DownloadFailed),
                                          // A FirmwareStatusNotification has no word for a download that waits.
                                          _                             => Task.CompletedTask
                                      }
                         );

            if (!answer.Accepted)
                AfterTheResponse(() => Tell(OCPPv1_6.FirmwareStatus.DownloadFailed));

            return Task.FromResult(new OCPPv1_6.CP.UpdateFirmwareResponse(Request));

        }

        #endregion

        #region SignedUpdateFirmware(Request, Tell, CancellationToken = default)

        /// <summary>
        /// Answer a SignedUpdateFirmware request of OCPP 1.6, and download the
        /// image after the answer.
        /// </summary>
        /// <remarks>
        /// Downloaded is the last thing told here. The signature of the image
        /// is checked by whoever takes the file - and every status after it is
        /// theirs to tell.
        /// </remarks>
        /// <param name="Request">The SignedUpdateFirmware request.</param>
        /// <param name="Tell">Told how the download goes: the SignedFirmwareStatusNotifications.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        public Task<OCPPv1_6.CP.SignedUpdateFirmwareResponse> SignedUpdateFirmware(OCPPv1_6.CS.SignedUpdateFirmwareRequest     Request,
                                                                                   Func<OCPPv1_6.FirmwareStatus, Task>         Tell,
                                                                                   CancellationToken                           CancellationToken   = default)
        {

            node.Log.Info($"'{Request.NetworkPath.Source}' asked this {node.Kind.Name} over OCPP 1.6 to fetch a signed firmware image (request {Request.UpdateRequestId}).",
                          "ocpp", "sftp", FileTransferService);

            var answer = FileTransfers.StartDownload(
                             Request.Firmware.RemoteLocation.ToString(),
                             Request.UpdateRequestId,
                             Request,
                             Request.Firmware.RetrieveTimestamp,
                             Request.Retries,
                             Request.RetryInterval,
                             state => Tell(state switch {
                                               DownloadState.DownloadScheduled  => OCPPv1_6.FirmwareStatus.DownloadScheduled,
                                               DownloadState.Downloading        => OCPPv1_6.FirmwareStatus.Downloading,
                                               DownloadState.DownloadPaused     => OCPPv1_6.FirmwareStatus.DownloadPaused,
                                               DownloadState.Downloaded         => OCPPv1_6.FirmwareStatus.Downloaded,
                                               _                                => OCPPv1_6.FirmwareStatus.DownloadFailed
                                           })
                         );

            return Task.FromResult(
                       new OCPPv1_6.CP.SignedUpdateFirmwareResponse(
                           Request,
                           !answer.Accepted
                               ? OCPPv1_6.UpdateFirmwareStatus.Rejected
                               : answer.ReplacedAnother
                                     ? OCPPv1_6.UpdateFirmwareStatus.AcceptedCanceled
                                     : OCPPv1_6.UpdateFirmwareStatus.Accepted
                       )
                   );

        }

        #endregion

    }

}
