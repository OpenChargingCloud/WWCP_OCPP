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

using System.Security.Cryptography;

using NUnit.Framework;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Web;
using cloud.charging.open.protocols.WWCP.Node.SecureShell;
using cloud.charging.open.protocols.WWCP.Node.Configuration;
using cloud.charging.open.protocols.WWCP.Node.TestKit;
using cloud.charging.open.protocols.OCPP.Node.TestKit;

using OCPPv1_6 = cloud.charging.open.protocols.OCPPv1_6;
using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests
{

    /// <summary>
    /// GetLog and UpdateFirmware of OCPP 2.1 with an "sftp://" location, as
    /// the bridge answers them with the node's file transfers: what the
    /// response says, what is told of the transfer after it, which failures
    /// are tried again, and that a new request cancels the one before it.
    /// </summary>
    [TestFixture]
    public class FileTransferBridgeTests
    {

        #region Data

        private const String  Login = WWCPNode.DefaultAdminUser;

        private String         directory  = "";
        private String         uploads    = "";
        private String         firmware   = "";
        private WWCPNode?      server;
        private IBridgedNode?  station;

        private readonly List<OCPPv2_1.UploadLogStatus>  logStatus       = [];
        private readonly List<OCPPv2_1.FirmwareStatus>   firmwareStatus  = [];
        private readonly List<TimeSpan>                  waited          = [];

        #endregion

        #region Setup / TearDown

        [SetUp]
        public void Setup()
        {
            directory = Path.Combine(Path.GetTempPath(), "ocpp-filetransfer-" + Guid.NewGuid().ToString("N")[..12]);
            uploads   = Path.Combine(directory, "server", "upload");
            firmware  = Path.Combine(directory, "server", "firmware");
            Directory.CreateDirectory(uploads);
            Directory.CreateDirectory(firmware);
            logStatus.     Clear();
            firmwareStatus.Clear();
            waited.        Clear();
            logStatus16.        Clear();
            diagnosticsStatus16.Clear();
            firmwareStatus16.   Clear();
        }

        [TearDown]
        public async Task TearDown()
        {

            if (station is not null) await station.DisposeAsync();
            if (server  is not null) await server. DisposeAsync();

            station  = null;
            server   = null;

            try
            {
                if (Directory.Exists(directory))
                    Directory.Delete(directory, recursive: true);
            }
            catch (Exception)
            { }

        }

        #endregion

        #region (private) Helpers

        private String Pin
            => server!.SSHHostKey!.Split(' ')[1];

        private UInt16 Port
            => server!.SSHPort!.Value.ToUInt16();

        private FileTransfers Transfers
            => station!.Bridge.FileTransfers;

        /// <summary>
        /// A file server, and a station that holds it, enrolled and confirmed.
        /// </summary>
        private async Task Enrolled(String? Pin = null, UInt16? Port = null, Boolean ServerRuns = true)
        {

            var home = Path.Combine(directory, "server");

            server = await TestPorts.StartedOnFreshPorts(() => {

                File.WriteAllText(Path.Combine(home, WWCPConfigFile.DefaultFileName),
                                  $$"""
                                  {
                                    "nts":   { "groups": { "legal": { "enabled": false } } },
                                    "ssh":   { "enabled": true, "port": {{TestPorts.Free()}}, "sftp": true }
                                  }
                                  """);

                var made = new WWCPNode(HTTPPort:          IPPort.Parse(TestPorts.Free()),
                                        AccountsPath:      Path.Combine(home, "accounts"),
                                        ConfigFile:        new WWCPConfigFile(Path.Combine(home, WWCPConfigFile.DefaultFileName)),
                                        CertificatesPath:  Path.Combine(home, "certificates"),
                                        LogToConsole:      false,
                                        BridgeDebugLog:    false);

                made.AddSftpArea(new SftpArea("upload",   uploads,  name => true, Read: Permission.Read("upload"),   Write: Permission.Edit("upload")));
                made.AddSftpArea(new SftpArea("firmware", firmware, name => true, Read: Permission.Read("firmware"), Write: Permission.Edit("firmware")));

                return made;

            });

            station = new BareNodes().New(Path.Combine(directory, "station"),
                                          JObject.Parse("""{ "nts": { "groups": { "legal": { "enabled": false } } }, "ssh": { "enabled": false } }"""));

            Assert.That(station.Node.SetFileServers([
                            new SSHFileServer("files",
                                              "127.0.0.1",
                                              Login,
                                              new NTSServerSSH(Port ?? this.Port, HostKeyPins: [ Pin ?? this.Pin ]),
                                              new SSHFileTransfers(FileTransferDirections.Upload | FileTransferDirections.Download,
                                                                   UploadPaths:    [ "/upload/" ],
                                                                   DownloadPaths:  [ "/firmware/" ]))
                        ]),
                        Is.Null);

            var line  = station.Node.FileTransferPublicKey("files", Login);
            var added = await server.AuthorizeSSHKey(Login, line!);
            Assert.That(added.IsAdded, Is.True, added.Reason);
            Assert.That(station.Node.ConfirmFileTransferKey("files", Login, line), Is.Null);

            // Between two attempts no time passes here, and what would have passed is kept.
            Transfers.Wait = (time, cancellationToken) => { lock (waited) waited.Add(time); return Task.CompletedTask; };

        }

        private String URL(String Path, UInt16? Port = null, String Host = "127.0.0.1")
            => $"sftp://{Login}@{Host}:{Port ?? this.Port}{Path}";

        private Task<OCPPv2_1.CS.GetLogResponse> GetLog(String  Location,
                                                        Int32   RequestId   = 42,
                                                        Byte?   Retries     = null)

            => station!.Bridge.GetLog(
                   new OCPPv2_1.CSMS.GetLogRequest(
                       SourceRouting.To(station.OCPPv2_1Node.Id),
                       RequestId,
                       OCPPv2_1.LogType.DiagnosticsLog,
                       new OCPPv2_1.LogParameters(org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.Parse(Location)),
                       Retries,
                       TimeSpan.FromSeconds(30)
                   ),
                   status => { lock (logStatus) logStatus.Add(status); return Task.CompletedTask; }
               );

        private Task<OCPPv2_1.CS.UpdateFirmwareResponse> UpdateFirmware(String           Location,
                                                                        Int32            RequestId    = 7,
                                                                        Byte?            Retries      = null,
                                                                        DateTimeOffset?  RetrieveAt   = null)

            => station!.Bridge.UpdateFirmware(
                   new OCPPv2_1.CSMS.UpdateFirmwareRequest(
                       SourceRouting.To(station.OCPPv2_1Node.Id),
                       new OCPPv2_1.Firmware(org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.Parse(Location),
                                             RetrieveAt ?? DateTimeOffset.UtcNow.AddMinutes(-1)),
                       RequestId,
                       Retries,
                       TimeSpan.FromSeconds(30)
                   ),
                   status => { lock (firmwareStatus) firmwareStatus.Add(status); return Task.CompletedTask; }
               );

        /// <summary>
        /// A stream whose reads wait until they are let go.
        /// </summary>
        private sealed class HeldStream(Byte[] Content) : MemoryStream(Content)
        {
            public TaskCompletionSource  Asked    { get; } = new (TaskCreationOptions.RunContinuationsAsynchronously);
            public TaskCompletionSource  Release  { get; } = new (TaskCreationOptions.RunContinuationsAsynchronously);
            public Boolean               Disposed { get; private set; }

            public override async ValueTask<Int32> ReadAsync(Memory<Byte> Buffer, CancellationToken CancellationToken = default)
            {
                Asked.TrySetResult();
                await Release.Task.WaitAsync(CancellationToken);
                return await base.ReadAsync(Buffer, CancellationToken);
            }

            protected override void Dispose(Boolean Disposing)
            {
                Disposed = true;
                base.Dispose(Disposing);
            }
        }

        #endregion


        #region TheOCPPNodeLeavesItsFilesToTheBridge

        [Test]
        public async Task TheOCPPNodeLeavesItsFilesToTheBridge()
        {

            await Enrolled();

            Assert.That(station!.OCPPv2_1Node,                                                       Is.InstanceOf<OCPPv2_1.IFileTransferNode>());
            Assert.That(((OCPPv2_1.IFileTransferNode) station.OCPPv2_1Node).GetLogProcessor,         Is.Not.Null);
            Assert.That(((OCPPv2_1.IFileTransferNode) station.OCPPv2_1Node).UpdateFirmwareProcessor, Is.Not.Null);

            // And the OCPP nodes a station builds anew are handed the same.
            station.NewOCPPNodes();

            Assert.That(((OCPPv2_1.IFileTransferNode) station.OCPPv2_1Node).GetLogProcessor,         Is.Not.Null);
            Assert.That(((OCPPv2_1.IFileTransferNode) station.OCPPv2_1Node).UpdateFirmwareProcessor, Is.Not.Null);

        }

        #endregion

        #region ALogIsAcceptedNamedAndUploaded

        [Test]
        public async Task ALogIsAcceptedNamedAndUploaded()
        {

            await Enrolled();

            var content = RandomNumberGenerator.GetBytes(80_000);
            FileRequest? asked = null;

            Transfers.FileProvider = (request, cancellationToken) => {
                asked = request;
                return Task.FromResult<FileToUpload?>(new FileToUpload(new MemoryStream(content), "cs001_DiagnosticsLog_42.tar.gz"));
            };

            var response = await GetLog(URL("/upload/"));

            Assert.That(response.Status,      Is.EqualTo(OCPPv2_1.LogStatus.Accepted));
            Assert.That(response.Filename,    Is.EqualTo("cs001_DiagnosticsLog_42.tar.gz"), "the response names the file the logic above provided");
            Assert.That(response.StatusInfo,  Is.Null);

            Assert.That(asked!.Kind,          Is.EqualTo("DiagnosticsLog"), "what the file is, is the provider's: it is told what was asked for");
            Assert.That(asked.RequestId,      Is.EqualTo(42));

            await Transfers.Upload;

            Assert.That(logStatus, Is.EqualTo(new[] { OCPPv2_1.UploadLogStatus.Uploading, OCPPv2_1.UploadLogStatus.Uploaded }));
            Assert.That(File.ReadAllBytes(Path.Combine(uploads, "cs001_DiagnosticsLog_42.tar.gz")), Is.EqualTo(content));
            Assert.That(waited, Is.Empty);

        }

        #endregion

        #region ARequestThatIsRefusedSaysLittleAndTellsNothing

        [Test]
        public async Task ARequestThatIsRefusedSaysLittleAndTellsNothing()
        {

            await Enrolled();

            var provided = 0;
            Transfers.FileProvider = (request, cancellationToken) => {
                provided++;
                return Task.FromResult<FileToUpload?>(new FileToUpload(new MemoryStream([ 1, 2, 3 ]), "a.log"));
            };

            // A server the station does not hold, a path it is not allowed, a port that is not the peer's.
            foreach (var location in new[] { URL("/upload/", Host: "127.0.0.2"), URL("/firmware/"), URL("/upload/", Port: (UInt16) (Port + 1)) })
            {

                var refused = await GetLog(location);

                Assert.That(refused.Status,                     Is.EqualTo(OCPPv2_1.LogStatus.Rejected), location);
                Assert.That(refused.Filename,                   Is.Null);
                Assert.That(refused.StatusInfo!.ReasonCode,     Is.EqualTo(FileTransfers.FileServerNotPermitted), "one reason code for all of them");
                Assert.That(refused.StatusInfo.AdditionalInfo,  Does.Not.Contain("127.0.0").And.Not.Contain("files").And.Not.Contain("upload"),
                            "the response does not say which file servers, logins and paths the station has");

            }

            var malformed = await GetLog($"sftp://{Login}@127.0.0.1:{Port}/upload/../etc/");
            Assert.That(malformed.Status,                  Is.EqualTo(OCPPv2_1.LogStatus.Rejected));
            Assert.That(malformed.StatusInfo!.ReasonCode,  Is.EqualTo(FileTransfers.InvalidURL));

            // No fallback to something weaker.
            var https = await GetLog("https://127.0.0.1/upload/");
            Assert.That(https.Status,                      Is.EqualTo(OCPPv2_1.LogStatus.Rejected));
            Assert.That(https.StatusInfo!.ReasonCode,      Is.EqualTo(FileTransfers.NotSupported));

            Assert.That(provided,   Is.Zero,  "nobody is asked for a file that will not be sent");
            Assert.That(logStatus,  Is.Empty, "and nothing is told of a transfer that never started");

            // Nothing to send, or nobody to provide it.
            Transfers.FileProvider = (request, cancellationToken) => Task.FromResult<FileToUpload?>(null);
            Assert.That((await GetLog(URL("/upload/"))).StatusInfo!.ReasonCode, Is.EqualTo(FileTransfers.NothingToSend));

            Transfers.FileProvider = null;
            Assert.That((await GetLog(URL("/upload/"))).StatusInfo!.ReasonCode, Is.EqualTo(FileTransfers.NotSupported));

            Assert.That(Directory.GetFiles(uploads), Is.Empty);

        }

        #endregion

        #region WhatMayBeBetterInAMinuteIsTriedAgainAndNothingElse

        [Test]
        public async Task WhatMayBeBetterInAMinuteIsTriedAgainAndNothingElse()
        {

            // Nobody listens where the file server is said to be.
            var closed = TestPorts.Free();
            await Enrolled(Port: closed);

            var provided = new MemoryStream(RandomNumberGenerator.GetBytes(1000));
            Transfers.FileProvider = (request, cancellationToken) => Task.FromResult<FileToUpload?>(new FileToUpload(provided, "a.log"));

            var response = await GetLog(URL("/upload/", Port: closed), Retries: 2);
            Assert.That(response.Status, Is.EqualTo(OCPPv2_1.LogStatus.Accepted), "whether the server is there is not known before the answer");

            await Transfers.Upload;

            Assert.That(logStatus, Is.EqualTo(new[] { OCPPv2_1.UploadLogStatus.Uploading, OCPPv2_1.UploadLogStatus.UploadFailure }));
            Assert.That(waited,    Is.EqualTo(new[] { TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30) }), "two retries, each after the interval of the request");
            Assert.That(provided.CanRead, Is.False, "the file is let go when the upload ended");

        }

        [Test]
        public async Task AForeignHostKeyIsNotAskedAgain()
        {

            // The station holds the file server to another key than the one it has.
            await Enrolled(Pin: "SHA256:hOEj1ea61PSArpO3/qj2Drlp024D2eiJ7DJQNNLl+hU");

            Transfers.FileProvider = (request, cancellationToken) => Task.FromResult<FileToUpload?>(new FileToUpload(new MemoryStream([ 1, 2, 3 ]), "a.log"));

            Assert.That((await GetLog(URL("/upload/"), Retries: 3)).Status, Is.EqualTo(OCPPv2_1.LogStatus.Accepted));
            await Transfers.Upload;

            Assert.That(logStatus, Is.EqualTo(new[] { OCPPv2_1.UploadLogStatus.Uploading, OCPPv2_1.UploadLogStatus.UploadFailure }));
            Assert.That(waited,    Is.Empty, "three retries were allowed, and none was made: a retry would only ask the same server again");
            Assert.That(Directory.GetFiles(uploads), Is.Empty);

        }

        #endregion

        #region ANewRequestCancelsTheOneBeforeIt

        [Test]
        public async Task ANewRequestCancelsTheOneBeforeIt()
        {

            await Enrolled();

            var first   = new HeldStream(RandomNumberGenerator.GetBytes(50_000));
            var second  = RandomNumberGenerator.GetBytes(2000);
            var files   = new Queue<FileToUpload>([ new FileToUpload(first, "first.log"), new FileToUpload(new MemoryStream(second), "second.log") ]);

            Transfers.FileProvider = (request, cancellationToken) => Task.FromResult<FileToUpload?>(files.Dequeue());

            Assert.That((await GetLog(URL("/upload/"), RequestId: 1)).Status, Is.EqualTo(OCPPv2_1.LogStatus.Accepted));

            // The first upload is on its way, and waits for its file.
            await first.Asked.Task.WaitAsync(TimeSpan.FromSeconds(20));
            var firstUpload = Transfers.Upload;

            var again = await GetLog(URL("/upload/"), RequestId: 2);

            Assert.That(again.Status,    Is.EqualTo(OCPPv2_1.LogStatus.AcceptedCanceled));
            Assert.That(again.Filename,  Is.EqualTo("second.log"));

            await firstUpload.WaitAsync(TimeSpan.FromSeconds(20));
            await Transfers.Upload.WaitAsync(TimeSpan.FromSeconds(20));

            Assert.That(logStatus, Is.EqualTo(new[] {
                                                  OCPPv2_1.UploadLogStatus.Uploading,
                                                  OCPPv2_1.UploadLogStatus.AcceptedCanceled,
                                                  OCPPv2_1.UploadLogStatus.Uploading,
                                                  OCPPv2_1.UploadLogStatus.Uploaded
                                              }),
                        "what the first one tells, it tells first");

            Assert.That(first.Disposed,                                        Is.True);
            Assert.That(Directory.GetFiles(uploads).Select(Path.GetFileName),  Is.EqualTo(new[] { "second.log" }), "nothing of the upload that was cancelled is left");
            Assert.That(File.ReadAllBytes(Path.Combine(uploads, "second.log")), Is.EqualTo(second));

        }

        #endregion

        #region AFirmwareImageIsDownloadedAndHandedOverComplete

        [Test]
        public async Task AFirmwareImageIsDownloadedAndHandedOverComplete()
        {

            await Enrolled();

            var image = RandomNumberGenerator.GetBytes(100_000);
            File.WriteAllBytes(Path.Combine(firmware, "station-4.2.1.bin"), image);

            DownloadedFile? taken        = null;
            Byte[]?         whenTaken    = null;
            var             toldBefore   = new List<OCPPv2_1.FirmwareStatus>();

            Transfers.FileConsumer = (file, cancellationToken) => {
                taken      = file;
                whenTaken  = File.ReadAllBytes(file.Path);
                lock (firmwareStatus) toldBefore.AddRange(firmwareStatus);
                return Task.CompletedTask;
            };

            var response = await UpdateFirmware(URL("/firmware/station-4.2.1.bin"));

            Assert.That(response.Status,      Is.EqualTo(OCPPv2_1.UpdateFirmwareStatus.Accepted));
            Assert.That(response.StatusInfo,  Is.Null);

            await Transfers.Download;

            Assert.That(firmwareStatus, Is.EqualTo(new[] { OCPPv2_1.FirmwareStatus.Downloading, OCPPv2_1.FirmwareStatus.Downloaded }),
                        "Downloaded is the last thing told here: what follows is for whoever takes the file");

            Assert.That(whenTaken,           Is.EqualTo(image), "handed over complete");
            Assert.That(toldBefore.Last(),   Is.EqualTo(OCPPv2_1.FirmwareStatus.Downloaded));
            Assert.That(taken!.RequestId,    Is.EqualTo(7));
            Assert.That(taken.Request,       Is.InstanceOf<OCPPv2_1.CSMS.UpdateFirmwareRequest>(), "with the request, for its signature and its dates");
            Assert.That(Path.GetDirectoryName(taken.Path), Is.EqualTo(Transfers.StagingDirectory));
            Assert.That(Directory.GetFiles(Transfers.StagingDirectory), Has.Length.EqualTo(1), "nothing unfinished is left beside it");

            // Larger than the station may hold: not fetched.
            firmwareStatus.Clear();
            Transfers.MaxDownloadSize = image.Length - 1;

            Assert.That((await UpdateFirmware(URL("/firmware/station-4.2.1.bin"), RequestId: 8)).Status, Is.EqualTo(OCPPv2_1.UpdateFirmwareStatus.Accepted));
            await Transfers.Download;

            Assert.That(firmwareStatus, Is.EqualTo(new[] { OCPPv2_1.FirmwareStatus.Downloading, OCPPv2_1.FirmwareStatus.DownloadFailed }));
            Assert.That(Directory.GetFiles(Transfers.StagingDirectory).Select(Path.GetFileName), Is.EqualTo(new[] { "7-station-4.2.1.bin" }));

            // From a place the station is not configured for: rejected, and nothing is told.
            firmwareStatus.Clear();

            var refused = await UpdateFirmware(URL("/upload/station-4.2.1.bin"));

            Assert.That(refused.Status,                  Is.EqualTo(OCPPv2_1.UpdateFirmwareStatus.Rejected));
            Assert.That(refused.StatusInfo!.ReasonCode,  Is.EqualTo(FileTransfers.FileServerNotPermitted));
            Assert.That(firmwareStatus,                  Is.Empty);

        }

        #endregion

        #region ADownloadWaitsUntilItIsDueAndForItsNextAttempt

        [Test]
        public async Task ADownloadWaitsUntilItIsDueAndForItsNextAttempt()
        {

            var closed = TestPorts.Free();
            await Enrolled(Port: closed);

            var due      = DateTimeOffset.UtcNow.AddHours(2);
            var response = await UpdateFirmware(URL("/firmware/station-4.2.1.bin", Port: closed), Retries: 1, RetrieveAt: due);

            Assert.That(response.Status, Is.EqualTo(OCPPv2_1.UpdateFirmwareStatus.Accepted));
            await Transfers.Download;

            Assert.That(firmwareStatus, Is.EqualTo(new[] {
                                                       OCPPv2_1.FirmwareStatus.DownloadScheduled,
                                                       OCPPv2_1.FirmwareStatus.Downloading,
                                                       OCPPv2_1.FirmwareStatus.DownloadPaused,
                                                       OCPPv2_1.FirmwareStatus.Downloading,
                                                       OCPPv2_1.FirmwareStatus.DownloadFailed
                                                   }));

            Assert.That(waited,                 Has.Count.EqualTo(2));
            Assert.That(waited[0].TotalMinutes, Is.EqualTo(120).Within(1), "until it is due");
            Assert.That(waited[1],              Is.EqualTo(TimeSpan.FromSeconds(30)), "and the interval of the request before its second attempt");
            Assert.That(Directory.Exists(Transfers.StagingDirectory) ? Directory.GetFiles(Transfers.StagingDirectory) : [], Is.Empty);

        }

        #endregion


        #region OCPP 1.6

        private readonly List<OCPPv1_6.UploadLogStatus>     logStatus16          = [];
        private readonly List<OCPPv1_6.DiagnosticsStatus>   diagnosticsStatus16  = [];
        private readonly List<OCPPv1_6.FirmwareStatus>      firmwareStatus16     = [];

        private static Func<TStatus, Task> Into<TStatus>(List<TStatus> Told)
            => status => { lock (Told) Told.Add(status); return Task.CompletedTask; };

        private static async Task Until(Func<Boolean> IsSo)
        {
            for (var i = 0; i < 100 && !IsSo(); i++)
                await Task.Delay(50);
        }

        private Task<OCPPv1_6.CP.GetLogResponse> GetLog16(String Location)

            => station!.Bridge.GetLog(
                   new OCPPv1_6.CS.GetLogRequest(
                       SourceRouting.To(((OCPPv1_6.CS.IChargePointNode) station.OCPPv1_6Node).Id),
                       OCPPv1_6.LogTypes.SecurityLog,
                       43,
                       new OCPPv1_6.LogParameters(org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.Parse(Location))
                   ),
                   Into(logStatus16)
               );

        private Task<OCPPv1_6.CP.GetDiagnosticsResponse> GetDiagnostics16(String Location)

            => station!.Bridge.GetDiagnostics(
                   new OCPPv1_6.CS.GetDiagnosticsRequest(
                       SourceRouting.To(((OCPPv1_6.CS.IChargePointNode) station.OCPPv1_6Node).Id),
                       Location
                   ),
                   Into(diagnosticsStatus16)
               );

        private Task<OCPPv1_6.CP.UpdateFirmwareResponse> UpdateFirmware16(String Location, Byte? Retries = null, DateTimeOffset? RetrieveAt = null)

            => station!.Bridge.UpdateFirmware(
                   new OCPPv1_6.CS.UpdateFirmwareRequest(
                       SourceRouting.To(((OCPPv1_6.CS.IChargePointNode) station.OCPPv1_6Node).Id),
                       org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.Parse(Location),
                       RetrieveAt ?? DateTimeOffset.UtcNow.AddMinutes(-1),
                       Retries,
                       TimeSpan.FromSeconds(30)
                   ),
                   Into(firmwareStatus16)
               );

        private Task<OCPPv1_6.CP.SignedUpdateFirmwareResponse> SignedUpdateFirmware16(String Location)

            => station!.Bridge.SignedUpdateFirmware(
                   new OCPPv1_6.CS.SignedUpdateFirmwareRequest(
                       SourceRouting.To(((OCPPv1_6.CS.IChargePointNode) station.OCPPv1_6Node).Id),
                       new OCPPv1_6.FirmwareImage(org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.Parse(Location),
                                                  DateTime.UtcNow.AddMinutes(-1),
                                                  "-----BEGIN CERTIFICATE-----",
                                                  "c2lnbmF0dXJl"),
                       9
                   ),
                   Into(firmwareStatus16)
               );


        [Test]
        public async Task TheOCPP1_6NodeLeavesItsFilesToTheBridge()
        {

            await Enrolled();

            Assert.That(station!.OCPPv1_6Node, Is.InstanceOf<OCPPv1_6.IFileTransferNode>());

            var node16 = (OCPPv1_6.IFileTransferNode) station.OCPPv1_6Node;

            Assert.That(node16.GetLogProcessor,                Is.Not.Null);
            Assert.That(node16.GetDiagnosticsProcessor,        Is.Not.Null);
            Assert.That(node16.UpdateFirmwareProcessor,        Is.Not.Null);
            Assert.That(node16.SignedUpdateFirmwareProcessor,  Is.Not.Null);

        }

        [Test]
        public async Task OCPP1_6_ALogAndTheDiagnosticsAreUploaded()
        {

            await Enrolled();

            var asked = new List<FileRequest>();
            var files = 0;

            Transfers.FileProvider = (request, cancellationToken) => {
                asked.Add(request);
                return Task.FromResult<FileToUpload?>(new FileToUpload(new MemoryStream(RandomNumberGenerator.GetBytes(5000)), $"file{++files}.tar.gz"));
            };

            // GetLog of the security extension.
            var log = await GetLog16(URL("/upload/"));

            Assert.That(log.Status,    Is.EqualTo(OCPPv1_6.LogStatus.Accepted));
            Assert.That(log.Filename,  Is.EqualTo("file1.tar.gz"));

            await Transfers.Upload;

            Assert.That(logStatus16,   Is.EqualTo(new[] { OCPPv1_6.UploadLogStatus.Uploading, OCPPv1_6.UploadLogStatus.Uploaded }));
            Assert.That(asked[0].Kind, Is.EqualTo("SecurityLog"));

            // GetDiagnostics, whose response is the file name alone.
            var diagnostics = await GetDiagnostics16(URL("/upload/"));

            Assert.That(diagnostics.FileName, Is.EqualTo("file2.tar.gz"));

            await Transfers.Upload;

            Assert.That(diagnosticsStatus16,  Is.EqualTo(new[] { OCPPv1_6.DiagnosticsStatus.Uploading, OCPPv1_6.DiagnosticsStatus.Uploaded }));
            Assert.That(asked[1].Kind,        Is.EqualTo("Diagnostics"));
            Assert.That(asked[1].RequestId,   Is.Null);

            Assert.That(Directory.GetFiles(uploads).Select(Path.GetFileName).Order(), Is.EqualTo(new[] { "file1.tar.gz", "file2.tar.gz" }));

        }

        [Test]
        public async Task OCPP1_6_ARefusalIsSaidInTheFewWordsThereAre()
        {

            await Enrolled();

            var provided = 0;
            Transfers.FileProvider = (request, cancellationToken) => {
                provided++;
                return Task.FromResult<FileToUpload?>(new FileToUpload(new MemoryStream([ 1, 2, 3 ]), "a.log"));
            };

            var elsewhere = URL("/upload/", Host: "127.0.0.2");

            // GetLog has a status, and no status info.
            var log = await GetLog16(elsewhere);

            Assert.That(log.Status,    Is.EqualTo(OCPPv1_6.LogStatus.Rejected));
            Assert.That(log.Filename,  Is.Null);
            Assert.That(logStatus16,   Is.Empty);

            // GetDiagnostics has neither: no file name, and the upload is told as failed.
            var diagnostics = await GetDiagnostics16(elsewhere);

            Assert.That(diagnostics.FileName, Is.Null.Or.Empty);
            await Until(() => diagnosticsStatus16.Count > 0);
            Assert.That(diagnosticsStatus16,  Is.EqualTo(new[] { OCPPv1_6.DiagnosticsStatus.UploadFailed }));

            // UpdateFirmware answers with nothing: the download is told as failed.
            await UpdateFirmware16(URL("/firmware/a.bin", Host: "127.0.0.2"));

            await Until(() => firmwareStatus16.Count > 0);
            Assert.That(firmwareStatus16,     Is.EqualTo(new[] { OCPPv1_6.FirmwareStatus.DownloadFailed }));

            // SignedUpdateFirmware has a status again.
            firmwareStatus16.Clear();

            var signed = await SignedUpdateFirmware16(URL("/firmware/a.bin", Host: "127.0.0.2"));

            Assert.That(signed.Status,        Is.EqualTo(OCPPv1_6.UpdateFirmwareStatus.Rejected));
            await Task.Delay(500);
            Assert.That(firmwareStatus16,     Is.Empty);

            Assert.That(provided,                    Is.Zero);
            Assert.That(Directory.GetFiles(uploads), Is.Empty);

        }

        [Test]
        public async Task OCPP1_6_AFirmwareImageIsDownloadedSignedOrNot()
        {

            await Enrolled();

            var image = RandomNumberGenerator.GetBytes(60_000);
            File.WriteAllBytes(Path.Combine(firmware, "station-4.2.1.bin"), image);

            var taken = new List<DownloadedFile>();
            Transfers.FileConsumer = (file, cancellationToken) => { taken.Add(file); return Task.CompletedTask; };

            // The plain one: no request id, no signature.
            await UpdateFirmware16(URL("/firmware/station-4.2.1.bin"));
            await Transfers.Download;

            Assert.That(firmwareStatus16,   Is.EqualTo(new[] { OCPPv1_6.FirmwareStatus.Downloading, OCPPv1_6.FirmwareStatus.Downloaded }));
            Assert.That(taken[0].RequestId, Is.Null);
            Assert.That(taken[0].Request,   Is.InstanceOf<OCPPv1_6.CS.UpdateFirmwareRequest>());
            Assert.That(File.ReadAllBytes(taken[0].Path), Is.EqualTo(image));

            // The signed one: the request goes with the file, for whoever checks its signature.
            firmwareStatus16.Clear();

            var signed = await SignedUpdateFirmware16(URL("/firmware/station-4.2.1.bin"));
            Assert.That(signed.Status,      Is.EqualTo(OCPPv1_6.UpdateFirmwareStatus.Accepted));

            await Transfers.Download;

            Assert.That(firmwareStatus16,   Is.EqualTo(new[] { OCPPv1_6.FirmwareStatus.Downloading, OCPPv1_6.FirmwareStatus.Downloaded }));
            Assert.That(taken[1].RequestId, Is.EqualTo(9));
            Assert.That(((OCPPv1_6.CS.SignedUpdateFirmwareRequest) taken[1].Request!).Firmware.Signature, Is.EqualTo("c2lnbmF0dXJl"));
            Assert.That(File.ReadAllBytes(taken[1].Path), Is.EqualTo(image));

        }

        [Test]
        public async Task OCPP1_6_APlainDownloadThatWaitsTellsNothingOfIt()
        {

            var closed = TestPorts.Free();
            await Enrolled(Port: closed);

            await UpdateFirmware16(URL("/firmware/station-4.2.1.bin", Port: closed), Retries: 1, RetrieveAt: DateTimeOffset.UtcNow.AddHours(1));
            await Transfers.Download;

            // A FirmwareStatusNotification has no word for scheduled and none for paused.
            Assert.That(firmwareStatus16, Is.EqualTo(new[] {
                                                         OCPPv1_6.FirmwareStatus.Downloading,
                                                         OCPPv1_6.FirmwareStatus.Downloading,
                                                         OCPPv1_6.FirmwareStatus.DownloadFailed
                                                     }));
            Assert.That(waited, Has.Count.EqualTo(2), "it waited until it was due, and for its second attempt, all the same");

        }

        #endregion

    }

}
