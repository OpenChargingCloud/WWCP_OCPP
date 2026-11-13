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

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    #region The seams to the logic above

    /// <summary>
    /// A file to upload: a stream and the name it gets at the file server.
    /// </summary>
    /// <remarks>
    /// To the transport a diagnostics log and a security log are files, and
    /// nothing more. What is in one, of which time, as one file or an archive
    /// of several, is decided by whoever provides it.
    /// </remarks>
    /// <param name="Content">The file. A stream that can seek is read again for another attempt; one that cannot is sent once. It is disposed when the upload ended.</param>
    /// <param name="FileName">The name it gets: no path. It should say what makes it unique, as nothing is ever written over.</param>
    public sealed record FileToUpload(Stream  Content,
                                      String  FileName);


    /// <summary>
    /// What a request asks to be uploaded, as far as OCPP says it.
    /// </summary>
    /// <param name="Kind">What OCPP calls it: "DiagnosticsLog", "SecurityLog", "DataCollectorLog" - and "Diagnostics" for GetDiagnostics of OCPP 1.6.</param>
    /// <param name="RequestId">The id of the request, where it has one.</param>
    /// <param name="Oldest">The oldest entry asked for.</param>
    /// <param name="Latest">The latest entry asked for.</param>
    public sealed record FileRequest(String           Kind,
                                     Int32?           RequestId,
                                     DateTimeOffset?  Oldest  = null,
                                     DateTimeOffset?  Latest  = null);


    /// <summary>
    /// A file that was downloaded, handed over once its last byte is there.
    /// </summary>
    /// <remarks>
    /// The host key said who the server is, not what the file is. Whether a
    /// firmware image is genuine and may be installed is for whoever takes it:
    /// its signature, its version, its installation and everything that is
    /// reported of them.
    /// </remarks>
    /// <param name="Path">Where it lies, in the staging directory. It is the consumer's from now on.</param>
    /// <param name="RequestId">The id of the request, where it has one.</param>
    /// <param name="Request">The request that asked for it, of whatever OCPP version.</param>
    public sealed record DownloadedFile(String   Path,
                                        Int32?   RequestId,
                                        Object?  Request);

    #endregion

    #region What is told of a transfer

    /// <summary>
    /// What is told of an upload, in the words of OCPP 2.x's UploadLogStatus.
    /// </summary>
    public enum UploadState
    {
        Uploading,
        Uploaded,
        UploadFailure,
        PermissionDenied,
        NotSupportedOperation,
        AcceptedCanceled
    }

    /// <summary>
    /// What is told of a download, in the words of OCPP 2.x's FirmwareStatus -
    /// as far as the transport goes: what follows a file that is there is the
    /// consumer's to tell.
    /// </summary>
    public enum DownloadState
    {
        DownloadScheduled,
        Downloading,
        DownloadPaused,
        Downloaded,
        DownloadFailed
    }

    /// <summary>
    /// The answer to a request for a file transfer.
    /// </summary>
    /// <param name="Accepted">Whether the transfer was started.</param>
    /// <param name="ReplacedAnother">Whether a transfer that was running was cancelled for it: AcceptedCanceled.</param>
    /// <param name="ReasonCode">Why not: "InvalidURL", "FileServerNotPermitted", "NothingToSend", "NotSupported".</param>
    /// <param name="Info">A sentence for the response. It does not say which file servers the node has.</param>
    /// <param name="FileName">The name the file gets at the file server.</param>
    public sealed record FileTransferAnswer(Boolean  Accepted,
                                            Boolean  ReplacedAnother  = false,
                                            String?  ReasonCode       = null,
                                            String?  Info             = null,
                                            String?  FileName         = null);

    #endregion


    /// <summary>
    /// The file transfers a node makes on a request of OCPP - GetLog,
    /// GetDiagnostics, UpdateFirmware - whatever the version: the answer to
    /// the request, the transfer after it, its attempts, and what is told of
    /// it. By the white paper "SFTP for Logfile Uploads and Firmware
    /// Downloads".
    /// </summary>
    /// <remarks>
    /// <para>
    /// Everything that can be decided without the network is decided before
    /// the answer (<see cref="WWCPNode.JudgeFileTransfer"/>), so that a refusal
    /// is an answer and not a failure some minutes later. The answer says
    /// little: one reason code for everything the configuration lacks.
    /// </para>
    /// <para>
    /// One upload and one download run at a time; a new request cancels the
    /// one before it, as OCPP says. The retries of a request are for what may
    /// be better a minute later, and for nothing else: a refused host key, a
    /// refused login and a missing permission are not asked for again.
    /// </para>
    /// <para>
    /// What a file is, is not known here. The logic above provides the file
    /// to upload and takes the file that was downloaded.
    /// </para>
    /// </remarks>
    public sealed class FileTransfers
    {

        #region Data

        /// <summary>
        /// The reason code of a URL that is none: saying so discloses nothing.
        /// </summary>
        public const String  InvalidURL              = "InvalidURL";

        /// <summary>
        /// The one reason code for every file server, login, path or key the
        /// configuration does not have.
        /// </summary>
        public const String  FileServerNotPermitted  = "FileServerNotPermitted";

        /// <summary>
        /// The provider has no file for the request.
        /// </summary>
        public const String  NothingToSend           = "NothingToSend";

        /// <summary>
        /// Nothing provides files to upload, or the URL is of a protocol this
        /// node does not speak.
        /// </summary>
        public const String  NotSupported            = "NotSupported";

        private readonly WWCPNode                   node;
        private readonly Lock                       running   = new();
        private          CancellationTokenSource?   upload;
        private          CancellationTokenSource?   download;

        #endregion

        #region Properties

        /// <summary>
        /// Who provides the file a request asks to be uploaded; null where
        /// nobody does, and every such request is rejected.
        /// </summary>
        public Func<FileRequest, CancellationToken, Task<FileToUpload?>>?  FileProvider       { get; set; }

        /// <summary>
        /// Who takes a file that was downloaded; null where nobody does, and
        /// the file stays in the staging directory.
        /// </summary>
        public Func<DownloadedFile, CancellationToken, Task>?              FileConsumer       { get; set; }

        /// <summary>
        /// Where a download lies until its last byte is there, and until
        /// somebody takes it.
        /// </summary>
        public String                                                      StagingDirectory   { get; set; }

        /// <summary>
        /// The most a file to download may have; null: as much as the server
        /// says. FileTransferMaxDownloadSize.
        /// </summary>
        public Int64?                                                      MaxDownloadSize    { get; set; }

        /// <summary>
        /// How the time between two attempts, and until a download is due,
        /// passes.
        /// </summary>
        public Func<TimeSpan, CancellationToken, Task>                     Wait               { get; set; }  = Task.Delay;

        /// <summary>
        /// The upload that was started last, until everything of it is told.
        /// </summary>
        public Task                                                        Upload             { get; private set; }  = Task.CompletedTask;

        /// <summary>
        /// The download that was started last, until everything of it is told.
        /// </summary>
        public Task                                                        Download           { get; private set; }  = Task.CompletedTask;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The file transfers of the given node on requests of OCPP.
        /// </summary>
        public FileTransfers(WWCPNode Node)
        {

            this.node              = Node;
            this.StagingDirectory  = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(Node.ConfigFile.Path)) ?? ".", "downloads");

        }

        #endregion


        #region (private) Refused(Judgement)

        /// <summary>
        /// The answer to a request the node's configuration does not allow:
        /// the reason code, and a sentence that does not say what the node
        /// holds. What exactly was missing is in the node's log and in its
        /// security event.
        /// </summary>
        private static FileTransferAnswer Refused(FileTransferJudgement Judgement)

            => Judgement.Refusal == FileTransferRefusal.InvalidURL
                   ? new (false, ReasonCode: InvalidURL,             Info: $"The location is no sftp:// URL this node takes: {Judgement.Why}.")
                   : new (false, ReasonCode: FileServerNotPermitted, Info: "The location is no file server, login or path this node is configured for.");

        private static Boolean IsSftp(String? URL)
            => URL?.TrimStart().StartsWith("sftp://", StringComparison.OrdinalIgnoreCase) == true;

        #endregion

        #region StartUpload(URL, Request, Retries, RetryInterval, Tell, CancellationToken)

        /// <summary>
        /// Answer a request for an upload, and where it is accepted, upload
        /// the file after the answer.
        /// </summary>
        /// <param name="URL">The directory the request names.</param>
        /// <param name="Request">What it asks for.</param>
        /// <param name="Retries">How often the upload is tried again after the first attempt.</param>
        /// <param name="RetryInterval">How long to wait between two attempts.</param>
        /// <param name="Tell">Told every state of the upload, in order, after the answer.</param>
        public async Task<FileTransferAnswer> StartUpload(String?                   URL,
                                                          FileRequest               Request,
                                                          Byte?                     Retries,
                                                          TimeSpan?                 RetryInterval,
                                                          Func<UploadState, Task>   Tell,
                                                          CancellationToken         CancellationToken   = default)
        {

            if (!IsSftp(URL))
                return new (false, ReasonCode: NotSupported, Info: "Files are uploaded to sftp:// locations only.");

            // Everything that needs no network, before the answer.
            var judgement = node.JudgeFileTransfer(URL, FileTransferDirections.Upload);

            if (!judgement.IsAllowed)
                return Refused(judgement);

            if (FileProvider is not { } provider)
                return new (false, ReasonCode: NotSupported, Info: "Nothing on this node provides files to upload.");

            FileToUpload? file;

            try
            {
                file = await provider(Request, CancellationToken).ConfigureAwait(false);
            }
            catch (Exception problem) when (problem is not OperationCanceledException)
            {
                node.Log.Warning($"The file for a {Request.Kind} request could not be provided: {problem.Message}", "ocpp", "sftp", "filetransfer");
                file = null;
            }

            if (file is null)
                return new (false, ReasonCode: NothingToSend, Info: "There is no file for this request.");

            if (!SftpLocation.IsFileName(file.FileName))
            {
                await file.Content.DisposeAsync().ConfigureAwait(false);
                node.Log.Warning($"'{file.FileName}' is no name of a file to upload: the {Request.Kind} request is rejected.", "ocpp", "sftp", "filetransfer");
                return new (false, ReasonCode: NothingToSend, Info: "There is no file for this request.");
            }

            // A new request cancels the upload before it.
            var mine = new CancellationTokenSource();
            CancellationTokenSource? before;
            Task                     earlier;

            lock (running)
            {
                before   = upload;
                earlier  = Upload;
                upload   = mine;
                Upload   = Task.Run(() => Uploading(URL!, file, earlier, Retries ?? 0, RetryInterval ?? TimeSpan.Zero, Tell, mine));
            }

            var replaced = before is not null;
            before?.Cancel();

            return new (true, ReplacedAnother: replaced, FileName: file.FileName);

        }

        private async Task Uploading(String                    URL,
                                     FileToUpload              File,
                                     Task                      Earlier,
                                     Int32                     Retries,
                                     TimeSpan                  RetryInterval,
                                     Func<UploadState, Task>   Tell,
                                     CancellationTokenSource   Mine)
        {

            var state = UploadState.UploadFailure;

            try
            {

                // What the upload before it tells, it tells first.
                try { await Earlier.ConfigureAwait(false); } catch (Exception) { }

                await Told(Tell, UploadState.Uploading).ConfigureAwait(false);

                var start = File.Content.CanSeek ? File.Content.Position : 0;

                for (var attempt = 0; ; attempt++)
                {

                    var result = await node.UploadFileAsync(URL, File.Content, File.FileName, CancellationToken: Mine.Token).ConfigureAwait(false);

                    state = result.Outcome switch {
                                FileTransferOutcome.Done              => UploadState.Uploaded,
                                FileTransferOutcome.Cancelled         => UploadState.AcceptedCanceled,
                                FileTransferOutcome.LoginRefused      => UploadState.PermissionDenied,
                                FileTransferOutcome.PermissionDenied  => UploadState.PermissionDenied,
                                FileTransferOutcome.NoSuchFile        => UploadState.PermissionDenied,
                                FileTransferOutcome.NoSftp            => UploadState.NotSupportedOperation,
                                _                                     => UploadState.UploadFailure
                            };

                    // Only what may be better in a minute is tried again, and only a file that can be read again.
                    if (!result.MayBeTriedAgain || attempt >= Retries || !File.Content.CanSeek)
                        break;

                    await Wait(RetryInterval, Mine.Token).ConfigureAwait(false);
                    File.Content.Position = start;

                }

            }
            catch (OperationCanceledException)
            {
                state = UploadState.AcceptedCanceled;
            }
            catch (Exception problem)
            {
                node.Log.Warning($"The upload of '{File.FileName}' ended by {problem.GetType().Name}: {problem.Message}", "ocpp", "sftp", "filetransfer");
                state = UploadState.UploadFailure;
            }
            finally
            {

                try { await File.Content.DisposeAsync().ConfigureAwait(false); } catch (Exception) { }

                lock (running)
                    if (ReferenceEquals(upload, Mine))
                        upload = null;

                Mine.Dispose();

            }

            await Told(Tell, state).ConfigureAwait(false);

        }

        #endregion

        #region StartDownload(URL, RequestId, Request, RetrieveAt, Retries, RetryInterval, Tell)

        /// <summary>
        /// Answer a request for a download, and where it is accepted, download
        /// the file after the answer: when it is due, into the staging
        /// directory, taken up where an attempt before stopped.
        /// </summary>
        /// <param name="URL">The file the request names.</param>
        /// <param name="RequestId">The id of the request, where it has one.</param>
        /// <param name="Request">The request, handed to whoever takes the file.</param>
        /// <param name="RetrieveAt">When the file is to be fetched; null: now.</param>
        /// <param name="Retries">How often the download is tried again after the first attempt.</param>
        /// <param name="RetryInterval">How long to wait between two attempts.</param>
        /// <param name="Tell">Told every state of the download, in order, after the answer.</param>
        public FileTransferAnswer StartDownload(String?                     URL,
                                                Int32?                      RequestId,
                                                Object?                     Request,
                                                DateTimeOffset?             RetrieveAt,
                                                Byte?                       Retries,
                                                TimeSpan?                   RetryInterval,
                                                Func<DownloadState, Task>   Tell)
        {

            if (!IsSftp(URL))
                return new (false, ReasonCode: NotSupported, Info: "Files are downloaded from sftp:// locations only.");

            var judgement = node.JudgeFileTransfer(URL, FileTransferDirections.Download);

            if (!judgement.IsAllowed)
                return Refused(judgement);

            var name = Path.GetFileName(judgement.Location!.Path);

            if (!SftpLocation.IsFileName(name))
                return new (false, ReasonCode: InvalidURL, Info: "The location names no file.");

            var mine = new CancellationTokenSource();
            CancellationTokenSource? before;
            Task                     earlier;

            lock (running)
            {
                before    = download;
                earlier   = Download;
                download  = mine;
                Download  = Task.Run(() => Downloading(URL!, name, RequestId, Request, earlier, RetrieveAt, Retries ?? 0, RetryInterval ?? TimeSpan.Zero, Tell, mine));
            }

            var replaced = before is not null;
            before?.Cancel();

            return new (true, ReplacedAnother: replaced, FileName: name);

        }

        private async Task Downloading(String                      URL,
                                       String                      Name,
                                       Int32?                      RequestId,
                                       Object?                     Request,
                                       Task                        Earlier,
                                       DateTimeOffset?             RetrieveAt,
                                       Int32                       Retries,
                                       TimeSpan                    RetryInterval,
                                       Func<DownloadState, Task>   Tell,
                                       CancellationTokenSource     Mine)
        {

            var staged  = Path.Combine(StagingDirectory, $"{RequestId?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "x"}-{Name}");
            var partial = staged + ".download";
            var done    = false;

            try
            {

                try { await Earlier.ConfigureAwait(false); } catch (Exception) { }

                // Until it is due.
                if (RetrieveAt is DateTimeOffset due && due - node.TimeProvider.GetUtcNow() is { } until && until > TimeSpan.Zero)
                {

                    await Told(Tell, DownloadState.DownloadScheduled).ConfigureAwait(false);
                    await Wait(until, Mine.Token).ConfigureAwait(false);

                    // Judged again when the connection is made: a peer that is no file
                    // server any more by then is not dialled.
                    if (!node.JudgeFileTransfer(URL, FileTransferDirections.Download).IsAllowed)
                        return;

                }

                await Told(Tell, DownloadState.Downloading).ConfigureAwait(false);

                Directory.CreateDirectory(StagingDirectory);

                // A request starts anew: what another request left under that name is not its file.
                File.Delete(partial);
                File.Delete(staged);

                for (var attempt = 0; ; attempt++)
                {

                    FileTransferResult result;

                    await using (var file = new FileStream(partial, FileMode.Append, FileAccess.Write, FileShare.None))
                    {
                        // What an attempt before brought is not fetched again.
                        result = await node.DownloadFileAsync(URL, file, Offset: file.Length, MaxSize: MaxDownloadSize, CancellationToken: Mine.Token).ConfigureAwait(false);
                    }

                    if (result.IsDone)
                    {
                        File.Move(partial, staged);
                        done = true;
                        break;
                    }

                    if (result.Outcome == FileTransferOutcome.Cancelled)
                        throw new OperationCanceledException();

                    if (!result.MayBeTriedAgain || attempt >= Retries)
                        break;

                    // It waits for its next attempt.
                    await Told(Tell, DownloadState.DownloadPaused).ConfigureAwait(false);
                    await Wait(RetryInterval, Mine.Token).ConfigureAwait(false);
                    await Told(Tell, DownloadState.Downloading).ConfigureAwait(false);

                }

            }
            catch (OperationCanceledException)
            {
                // Another request took its place, and tells of itself.
                try { File.Delete(partial); } catch (Exception) { }
                return;
            }
            catch (Exception problem)
            {
                node.Log.Warning($"The download of '{Name}' ended by {problem.GetType().Name}: {problem.Message}", "ocpp", "sftp", "filetransfer");
            }
            finally
            {

                lock (running)
                    if (ReferenceEquals(download, Mine))
                        download = null;

                Mine.Dispose();

            }

            if (!done)
            {
                try { File.Delete(partial); } catch (Exception) { }
                await Told(Tell, DownloadState.DownloadFailed).ConfigureAwait(false);
                return;
            }

            await Told(Tell, DownloadState.Downloaded).ConfigureAwait(false);

            // Complete, or not at all - and from here on it is the consumer's.
            if (FileConsumer is { } consumer)
            {
                try
                {
                    await consumer(new DownloadedFile(staged, RequestId, Request), CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception problem)
                {
                    node.Log.Warning($"Whoever takes downloaded files did not take '{staged}': {problem.Message}", "ocpp", "sftp", "filetransfer");
                }
            }

        }

        #endregion

        #region (private) Told(Tell, State)

        /// <summary>
        /// Tell a state. A CSMS that cannot be told does not end a transfer.
        /// </summary>
        private async Task Told<TState>(Func<TState, Task> Tell, TState State)
        {
            try
            {
                await Tell(State).ConfigureAwait(false);
            }
            catch (Exception problem)
            {
                node.Log.Warning($"The CSMS could not be told '{State}' of a file transfer: {problem.Message}", "ocpp", "sftp", "filetransfer");
            }
        }

        #endregion

    }

}
