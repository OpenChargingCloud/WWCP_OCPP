/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics;

using Newtonsoft.Json;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace org.GraphDefined.WWCP.OCPP.LocalController
{

    /// <summary>
    /// An OCPP Local Controller Application.
    /// </summary>
    public class Program
    {

        #region (class) CommandException

        public class CommandException(String Message) : Exception(Message)
        {

            #region (static) NotWithinOCPPv1_6

            public static CommandException NotWithinOCPPv1_6
                => new ("This command ist not available within OCPP v1.6!");

            #endregion

        }

        #endregion

        #region Data

        private const           String         debugLogFile      = "debug.log";
        private const           String         ocppVersion1_6    = "v1.6";
        private const           String         ocppVersion2_1    = "v2.1";

        private static readonly SemaphoreSlim  cliLock           = new (1, 1);
        private static readonly SemaphoreSlim  logfileLock1_6    = new (1, 1);
        private static readonly SemaphoreSlim  logfileLock2_6    = new (1, 1);

        private readonly static String         logfileNameV1_6   = Path.Combine(AppContext.BaseDirectory, "OCPPv1.6_Messages.log");
        private readonly static String         logfileNameV2_1   = Path.Combine(AppContext.BaseDirectory, "OCPPv2.1_Messages.log");

        #endregion


        private static async Task DebugLog(String             Message,
                                           CancellationToken  CancellationToken)
        {

            try
            {
                await cliLock.WaitAsync(CancellationToken);
                DebugX.Log(Message);
            }
            catch (Exception e)
            {
                //DebugX.LogException(e, $"{nameof(testCSMSv2_1)}.{nameof(testCSMSv2_1.OnNewTCPConnection)}");
                DebugX.LogException(e, $"{nameof(DebugLog)}");
            }
            finally
            {
                cliLock.Release();
            }

        }


        private static async Task Log(String             LogFileName,
                                      String             Message,
                                      SemaphoreSlim      LogFileLock,
                                      CancellationToken  CancellationToken)
        {

            var retry = 0;

            do
            {
                try
                {

                    retry++;

                    await LogFileLock.WaitAsync(CancellationToken);

                    await File.AppendAllTextAsync(
                             LogFileName,
                             Message + Environment.NewLine,
                             CancellationToken
                         );

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, $"{nameof(WriteToLogfileV2_1)}");
                }
                finally
                {
                    LogFileLock.Release();
                }


            }
            while (retry > 3);

        }

        private static Task WriteToLogfileV2_1(String             Message,
                                               CancellationToken  CancellationToken)

            => Log(logfileNameV2_1,
                   Message,
                   logfileLock2_6,
                   CancellationToken);


        /// <summary>
        /// Start the OCPP Local Controller Application.
        /// </summary>
        /// <param name="Arguments">Command line arguments</param>
        public static async Task Main(String[] Arguments)
        {

            #region Data

            var dnsClient = new DNSClient(SearchForIPv6DNSServers: false);

            #endregion

            #region Debug to Console/file

            var debugFile    = new TextWriterTraceListener(debugLogFile);

            var debugTargets = new[] {
                debugFile,
                new TextWriterTraceListener(Console.Out)
            };

            Trace.Listeners.AddRange(debugTargets);

            #endregion


            #region Setup OCPP v2.1 Local Controller

            var testLCv2_1 = new TestNetworkingNode(

                                 Id:                      NetworkingNode_Id.Parse("OCPPv2.1-LocalController-01"),

                                 VendorName:              "GraphDefined OEM #1",
                                 Model:                   "VLC.1",
                                 Description:             I18NString.Create(Languages.en, "Our first virtual local controller!"),
                                 SerialNumber:            "SN-LC0001",
                                 FirmwareVersion:         "v0.1",
                                 Modem:                   new Modem(
                                                              ICCID:   "0001",
                                                              IMSI:    "1112"
                                                          ),
                                 DisableSendHeartbeats:   true,

                                 //HTTPBasicAuth:           new Tuple<String, String>("GDNN001", "1234"),
                                 DNSClient:               dnsClient

                             );


            //HTTPUploadPort:          IPPort.Parse(9921),

            var ocppWebSocketServer01 = testLCv2_1.AttachWebSocketServer(
                                            TCPPort:                         IPPort.Parse(9103),
                                            RequireAuthentication:           true,
                                            DisableWebSocketPings:           true,
                                            WebSocketPingEvery:              TimeSpan.FromMinutes(1),
                                            //SlowNetworkSimulationDelay:  TimeSpan.FromMilliseconds(10),
                                            AutoStart:                       true
                                        );

            //var ocppWebSocketServer02 = testLCv2_1.AttachWebSocketServer(
            //                                TCPPort:                         IPPort.Parse(9104),
            //                                ServerCertificateSelector:       null,
            //                                ClientCertificateSelector:       null,
            //                                ClientCertificateValidator:      null,
            //                                RequireAuthentication:           true,
            //                                DisableWebSocketPings:           true,
            //                                WebSocketPingEvery:              TimeSpan.FromMinutes(1),
            //                                //SlowNetworkSimulationDelay:  TimeSpan.FromMilliseconds(10),
            //                                AutoStart:                       true
            //                            );

            var ocppWebSocketServer23 = testLCv2_1.AttachWebAPI(
                                            HTTPServerPort:                  IPPort.Parse(9203),
                                            RequireAuthentication:           true,
                                            AutoStart:                       true
                                        );


            //      var webSocketServerV2_1        = testCSMSv2_1.CSMSServers.First() as WebSocketServer;

            //testCSMSv2_1.AddOrUpdateHTTPBasicAuth(NetworkingNode_Id.Parse("CP001"), "dummy-dev-password");


            #region HTTP Web Socket connections

            testLCv2_1.OnNewTCPConnection               += async (timestamp, server, connection,                   eventTrackingId,                     cancellationToken) => {

                await DebugLog(
                    $"New TCP connection from {connection.RemoteSocket}",
                    cancellationToken
                );

                await WriteToLogfileV2_1(
                    $"{timestamp.ToIso8601()}\tNEW TCP\t-\t{connection.RemoteSocket}",
                    cancellationToken
                );

            };

            testLCv2_1.OnNewWebSocketConnection         += async (timestamp, server, connection, networkingNodeId, sharedSubprotocols, eventTrackingId, cancellationToken) => {

                await DebugLog(
                    $"New HTTP web socket connection from '{networkingNodeId}' ({connection.RemoteSocket}) using '{sharedSubprotocols.AggregateWith(", ")}'",
                    cancellationToken
                );

                await WriteToLogfileV2_1(
                    $"{timestamp.ToIso8601()}\tNEW WS\t{networkingNodeId}\t{connection.RemoteSocket}",
                    cancellationToken
                );

            };

            testLCv2_1.OnCloseMessageReceived           += async (timestamp, server, connection, networkingNodeId, eventTrackingId, statusCode, reason, cancellationToken) => {

                await DebugLog(
                    $"'{networkingNodeId}' wants to close its HTTP web socket connection ({connection.RemoteSocket}): {statusCode}{(reason is not null ? $", '{reason}'" : "")}",
                    cancellationToken
                );

                await WriteToLogfileV2_1(
                    $"{timestamp.ToIso8601()}\tCLOSE\t{networkingNodeId}\t{connection.RemoteSocket}",
                    cancellationToken
                );

            };

            testLCv2_1.OnTCPConnectionClosed            += async (timestamp, server, connection, networkingNodeId, eventTrackingId, reason,             cancellationToken) => {

                await DebugLog(
                    $"'{networkingNodeId}' closed its HTTP web socket connection ({connection.RemoteSocket}){(reason is not null ? $": '{reason}'" : "")}",
                    cancellationToken
                );

                await WriteToLogfileV2_1(
                    $"{timestamp.ToIso8601()}\tCLOSED\t{networkingNodeId}\t{connection.RemoteSocket}",
                    cancellationToken
                );

            };

            #endregion

            #region HTTP Web Socket Pings/Pongs

            //(testCSMSv2_1.CSMSServers.First() as WebSocketServer).OnPingMessageReceived += async (timestamp, server, connection, eventTrackingId, frame) => {
            //    DebugX.Log(nameof(WebSocketServer) + ": Ping received: '" + frame.Payload.ToUTF8String() + "' (" + connection.TryGetCustomData("chargingStationId") + ", " + connection.RemoteSocket + ")");
            //    lock (testCSMSv2_1)
            //    {
            //        File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "TextMessages.log"),
            //                           String.Concat(timestamp.ToIso8601(), "\tPING IN\t", connection.TryGetCustomData("chargingStationId"), "\t", connection.RemoteSocket, Environment.NewLine));
            //    }
            //};

            //(testCSMSv2_1.CSMSServers.First() as WebSocketServer).OnPingMessageSent += async (timestamp, server, connection, eventTrackingId, frame) => {
            //    DebugX.Log(nameof(WebSocketServer) + ": Ping sent:     '" + frame.Payload.ToUTF8String() + "' (" + connection.TryGetCustomData("chargingStationId") + ", " + connection.RemoteSocket + ")");
            //    lock (testCSMSv2_1)
            //    {
            //        File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "TextMessages.log"),
            //                           String.Concat(timestamp.ToIso8601(), "\tPING OUT\t", connection.TryGetCustomData("chargingStationId"), "\t", connection.RemoteSocket, Environment.NewLine));
            //    }
            //};

            //(testCSMSv2_1.CSMSServers.First() as WebSocketServer).OnPongMessageReceived += async (timestamp, server, connection, eventTrackingId, frame) => {
            //    DebugX.Log(nameof(WebSocketServer) + ": Pong received: '" + frame.Payload.ToUTF8String() + "' (" + connection.TryGetCustomData("chargingStationId") + ", " + connection.RemoteSocket + ")");
            //    lock (testCSMSv2_1)
            //    {
            //        File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "TextMessages.log"),
            //                           String.Concat(timestamp.ToIso8601(), "\tPONG IN\t", connection.TryGetCustomData("chargingStationId"), "\t", connection.RemoteSocket, Environment.NewLine));
            //    }
            //};

            #endregion

            #region JSON Messages

            testLCv2_1.OCPP.IN.OnJSONMessageRequestReceived     += async (timestamp,
                                                                          server,
                                                                          jsonRequestMessage) => {

                await DebugLog(
                    $"Received a JSON web socket request from '{jsonRequestMessage.NetworkPath.Source}': '{jsonRequestMessage.Payload.ToString(Formatting.None)}'!",
                    jsonRequestMessage.CancellationToken
                );

                await WriteToLogfileV2_1(
                    // RemoteSocket???
                    $"{jsonRequestMessage.RequestTimestamp.ToIso8601()}\tREQ IN\t{jsonRequestMessage.NetworkPath.Source}\t{jsonRequestMessage.NetworkPath.Source}\t{jsonRequestMessage.Payload.ToString(Formatting.None)}",
                    jsonRequestMessage.CancellationToken
                );

            };

            testLCv2_1.OCPP.OUT.OnJSONMessageResponseSent       += async (timestamp,
                                                                          server,
                                                                          jsonResponseMessage) => {

                await DebugLog(
                    $"Sent a JSON web socket response to '{jsonResponseMessage.DestinationId}': '{jsonResponseMessage.Payload.ToString(Formatting.None)}'!",
                    jsonResponseMessage.CancellationToken
                );

                await WriteToLogfileV2_1(
                    // RemoteSocket???
                    $"{jsonResponseMessage.ResponseTimestamp.ToIso8601()}\tRES OUT\t{jsonResponseMessage.DestinationId}\t{jsonResponseMessage.DestinationId}\t{jsonResponseMessage.Payload.ToString(Formatting.None)}",
                    jsonResponseMessage.CancellationToken
                );

            };

            testLCv2_1.OCPP.OUT.OnJSONMessageRequestSent        += async (timestamp,
                                                                          server,
                                                                          jsonRequestMessage) => {

                await DebugLog(
                    $"Sent a JSON web socket request to '{jsonRequestMessage.DestinationId}': '{jsonRequestMessage.Payload.ToString(Formatting.None)}'!",
                    jsonRequestMessage.CancellationToken
                );

                await WriteToLogfileV2_1(
                    $"{jsonRequestMessage.RequestTimestamp.ToIso8601()}\tREQ OUT\t{jsonRequestMessage.DestinationId}\t{jsonRequestMessage.DestinationId}\t{jsonRequestMessage.Payload.ToString(Formatting.None)}",
                    jsonRequestMessage.CancellationToken
                );

            };

            testLCv2_1.OCPP.IN.OnJSONMessageResponseReceived    += async (timestamp,
                                                                          server,
                                                                          jsonResponseMessage) => {

                await DebugLog(
                    $"Received a JSON web socket response from '{jsonResponseMessage.NetworkPath.Source}': '{jsonResponseMessage.Payload.ToString(Formatting.None)}'!",
                    jsonResponseMessage.CancellationToken
                );

                await WriteToLogfileV2_1(
                    $"{jsonResponseMessage.ResponseTimestamp.ToIso8601()}\tRES IN\t{jsonResponseMessage.NetworkPath.Source}\t{jsonResponseMessage.NetworkPath.Source}\t{jsonResponseMessage.Payload.ToString(Formatting.None)}",
                    jsonResponseMessage.CancellationToken
                );

            };

            #endregion

            #region Binary Messages

            testLCv2_1.OCPP.IN.OnBinaryMessageRequestReceived   += async (timestamp,
                                                                          server,
                                                                          binaryRequestMessage) => {

                                                                              await DebugLog(
                    $"Received a binary web socket request from '{binaryRequestMessage.NetworkPath.Source}': '{binaryRequestMessage.Payload.ToBase64()}'!",
                    binaryRequestMessage.CancellationToken
                );

                await WriteToLogfileV2_1(
                    $"{binaryRequestMessage.RequestTimestamp.ToIso8601()}\tREQ IN\t{binaryRequestMessage.DestinationId}\t{binaryRequestMessage.DestinationId}\t{binaryRequestMessage.Payload.ToBase64()}",
                    binaryRequestMessage.CancellationToken
                );

            };

            testLCv2_1.OCPP.OUT.OnBinaryMessageResponseSent     += async (timestamp,
                                                                          server,
                                                                          binaryResponseMessage) => {

                await DebugLog(
                    $"Sent a binary web socket response to '{binaryResponseMessage.DestinationId}': '{binaryResponseMessage.Payload.ToBase64()}'!",
                    binaryResponseMessage.CancellationToken
                );

                await WriteToLogfileV2_1(
                    $"{binaryResponseMessage.ResponseTimestamp.ToIso8601()}\tRES OUT\t{binaryResponseMessage.DestinationId}\t{binaryResponseMessage.DestinationId}\t{binaryResponseMessage.Payload.ToBase64()}",
                    binaryResponseMessage.CancellationToken
                );

            };

            testLCv2_1.OCPP.OUT.OnBinaryMessageRequestSent      += async (timestamp,
                                                                          server,
                                                                          binaryRequestMessage) => {

                                                                              await DebugLog(
                    $"Sent a binary web socket request to '{binaryRequestMessage.DestinationId}': '{binaryRequestMessage.Payload.ToBase64()}'!",
                    binaryRequestMessage.CancellationToken
                );

                await WriteToLogfileV2_1(
                    $"{binaryRequestMessage.RequestTimestamp.ToIso8601()}\tREQ OUT\t{binaryRequestMessage.DestinationId}\t{binaryRequestMessage.DestinationId}\t{binaryRequestMessage.Payload.ToBase64()}",
                    binaryRequestMessage.CancellationToken
                );

            };

            testLCv2_1.OCPP.IN.OnBinaryMessageResponseReceived  += async (timestamp,
                                                                          server,
                                                                          binaryResponseMessage) => {

                                                                              await DebugLog(
                    $"Received a binary web socket response from '{binaryResponseMessage.NetworkPath.Source}': '{binaryResponseMessage.Payload.ToBase64()}'!",
                    binaryResponseMessage.CancellationToken
                );

                await WriteToLogfileV2_1(
                    $"{binaryResponseMessage.ResponseTimestamp.ToIso8601()}\tRES IN\t{binaryResponseMessage.NetworkPath.Source}\t{binaryResponseMessage.NetworkPath.Source}\t{binaryResponseMessage.Payload.ToBase64()}",
                    binaryResponseMessage.CancellationToken
                );

            };

            #endregion

            // ERRORS!!!

            #endregion


            DebugX.Log();
            DebugX.Log("Startup finished...");

            #region Wait for key 'Q' pressed... and quit.

            #region Data

            String[]?  commandArray        = null;
            var        ocppVersion         = "v2.1";
            var        chargingStationId   = "";
            var        quit                = false;

            #endregion

            do
            {

                try
                {

                    commandArray = Console.ReadLine()?.Trim()?.Split(' ');

                    if (commandArray is not null &&
                        commandArray.Length != 0)
                    {

                        var command = commandArray[0]?.ToLower();

                        if (command is not null &&
                            command.Length > 0)
                        {

                            if (command == "q")
                                quit = true;

                            #region Use {chargingStationId}

                            if (command.Equals("use", StringComparison.OrdinalIgnoreCase) && commandArray.Length == 2)
                            {

                                chargingStationId = commandArray[1];

                                DebugX.Log($"Now using charging station '{chargingStationId}'!");

                            }

                            #endregion

                            #region AddHTTPBasicAuth

                            //   AddHTTPBasicAuth abcd1234
                            if (command.Equals("AddHTTPBasicAuth", StringComparison.OrdinalIgnoreCase) && commandArray.Length == 2)
                            {
                           //     testLCv2_1.         AddOrUpdateHTTPBasicAuth(NetworkingNode_Id.Parse(chargingStationId), commandArray[2]);
                            }

                            #endregion


                            if (chargingStationId == "")
                                DebugX.Log("No charging station selected!");

                            else
                            {

                                #region Reset

                                //   Reset ...
                                if (command.Equals("reset", StringComparison.OrdinalIgnoreCase) && commandArray.Length == 2)
                                {

                                    var resetType = commandArray[1].ToLower();

                                    var response = await testLCv2_1.OCPP.OUT.Reset(
                                                       new OCPPv2_1.CSMS.ResetRequest(
                                                           NetworkingNodeId:    NetworkingNode_Id.Parse(chargingStationId),
                                                           ResetType:           resetType switch {
                                                                                    "immediate"  => OCPPv2_1.ResetType.Immediate,
                                                                                    "onidle"     => OCPPv2_1.ResetType.OnIdle,
                                                                                    _            => throw new CommandException("reset Immediate|OnIdle")
                                                                                }
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }


                                //   Reset {EVSEId} ...
                                if (command.Equals("reset", StringComparison.OrdinalIgnoreCase) && commandArray.Length == 3)
                                {

                                    var resetType = commandArray[2].ToLower();

                                    var response = await testLCv2_1.OCPP.OUT.Reset(
                                                       new OCPPv2_1.CSMS.ResetRequest(
                                                           NetworkingNodeId:    NetworkingNode_Id.Parse(chargingStationId),
                                                           ResetType:           resetType switch {
                                                                                    "immediate"  => OCPPv2_1.ResetType.Immediate,
                                                                                    "onidle"     => OCPPv2_1.ResetType.OnIdle,
                                                                                    _            => throw new CommandException("reset {EVSEId} Immediate|OnIdle")
                                                                                },
                                                           EVSEId:              OCPPv2_1.EVSE_Id.Parse(commandArray[1])
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region UpdateFirmware

                                //   UpdateFirmware https://api2.ocpp.charging.cloud:9901/firmware.bin
                                if (command == "UpdateFirmware".ToLower() && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.UpdateFirmware(
                                                       new OCPPv2_1.CSMS.UpdateFirmwareRequest(
                                                            NetworkingNodeId:          NetworkingNode_Id.Parse(chargingStationId),
                                                            Firmware:                  new OCPPv2_1.Firmware(
                                                                                           FirmwareURL:          URL.Parse(commandArray[1]),
                                                                                           RetrieveTimestamp:    Timestamp.Now,
                                                                                           InstallTimestamp:     Timestamp.Now,
                                                                                           SigningCertificate:   "xxx",
                                                                                           Signature:            "yyy"
                                                                                       ),
                                                            UpdateFirmwareRequestId:   RandomExtensions.RandomInt32(),
                                                            Retries:                   3,
                                                            RetryInterval:             null
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                // PublishFirmware

                                // UnpublishFirmware

                                #region GetBaseReport

                                //   GetBaseReport conf
                                //   GetBaseReport full
                                if (command == "GetBaseReport".ToLower() && (commandArray.Length == 1 || commandArray.Length == 2))
                                {

                                    var response = await testLCv2_1.OCPP.OUT.GetBaseReport(
                                                       new OCPPv2_1.CSMS.GetBaseReportRequest(
                                                           NetworkingNodeId:         NetworkingNode_Id.Parse(chargingStationId),
                                                           GetBaseReportRequestId:   RandomExtensions.RandomInt32(),
                                                           ReportBase:               commandArray[1] switch {
                                                                                         "conf"  => OCPPv2_1.ReportBase.ConfigurationInventory,
                                                                                         "full"  => OCPPv2_1.ReportBase.FullInventory,
                                                                                         _       => OCPPv2_1.ReportBase.SummaryInventory
                                                                                     }
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region GetReport

                                //   GetReport OCPPCommCtrlr
                                if (command == "GetReport".ToLower() && (commandArray.Length == 2 || commandArray.Length == 3))
                                {

                                    var response = await testLCv2_1.OCPP.OUT.GetReport(
                                                       new OCPPv2_1.CSMS.GetReportRequest(
                                                           NetworkingNodeId:     NetworkingNode_Id.Parse(chargingStationId),
                                                           GetReportRequestId:   RandomExtensions.RandomInt32(),
                                                           //ComponentCriteria:    new[] {
                                                           //                          OCPPv2_1.ComponentCriteria.Active
                                                           //                      },
                                                           ComponentVariables:   new[] {
                                                                                     new OCPPv2_1.ComponentVariable(
                                                                                         Component:   new OCPPv2_1.Component(
                                                                                                          Name:       commandArray[1],
                                                                                                          Instance:   null,
                                                                                                          EVSE:       null
                                                                                                      )
                                                                                         //Variable:    new OCPPv2_1.Variable(
                                                                                         //                 Name:       "",
                                                                                         //                 Instance:   null
                                                                                         //             )
                                                                                     )
                                                                                 }

                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region GetLog

                                if (command == "GetLog".ToLower() && commandArray.Length == 3)
                                {

                                    //   getlog http://172.20.101.28:9921 security
                                    //   getlog https://api2.ocpp.charging.cloud:9901 diagnostics
                                    //   getlog https://api2.ocpp.charging.cloud:9901 security
                                    //   getlog https://api2.ocpp.charging.cloud:9901 datacollector
                                    var response = await testLCv2_1.OCPP.OUT.GetLog(
                                                       new OCPPv2_1.CSMS.GetLogRequest(
                                                           NetworkingNodeId:      NetworkingNode_Id.Parse(chargingStationId),
                                                           LogType:         commandArray[2].ToLower() switch {
                                                                                 "security"       => OCPPv2_1.LogType.SecurityLog,
                                                                                 "datacollector"  => OCPPv2_1.LogType.DataCollectorLog,
                                                                                 _                => OCPPv2_1.LogType.DiagnosticsLog
                                                                            },
                                                           LogRequestId:    1,
                                                           Log:             new OCPPv2_1.LogParameters(
                                                                                RemoteLocation:    URL.Parse(commandArray[1]),
                                                                                OldestTimestamp:   null,
                                                                                LatestTimestamp:   null
                                                                            ),
                                                           Retries:         null,
                                                           RetryInterval:   null
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region SetVariables

                                //   SetVariables component variable value
                                if (command == "SetVariables".ToLower() && commandArray.Length == 4)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.SetVariables(
                                                       new OCPPv2_1.CSMS.SetVariablesRequest(
                                                           NetworkingNodeId:   NetworkingNode_Id.Parse(chargingStationId),
                                                           VariableData:       new[] {
                                                                                   new OCPPv2_1.SetVariableData(
                                                                                       new OCPPv2_1.Component(
                                                                                           Name:       commandArray[1],
                                                                                           Instance:   null,
                                                                                           EVSE:       null
                                                                                       ),
                                                                                       new OCPPv2_1.Variable(
                                                                                           Name:       commandArray[2],
                                                                                           Instance:   null
                                                                                       ),
                                                                                       commandArray[3]
                                                                                       //OCPPv2_1.AttributeTypes.Actual
                                                                                   )
                                                                               }
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region GetVariables

                                //   GetVariables component variable
                                if (command == "GetVariables".ToLower() && commandArray.Length == 3)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.GetVariables(
                                                       new OCPPv2_1.CSMS.GetVariablesRequest(
                                                           NetworkingNodeId:     NetworkingNode_Id.Parse(chargingStationId),
                                                           VariableData:   new[] {
                                                                               new OCPPv2_1.GetVariableData(
                                                                                   new OCPPv2_1.Component(
                                                                                       Name:       commandArray[1],
                                                                                       Instance:   null,
                                                                                       EVSE:       null
                                                                                   ),
                                                                                   new OCPPv2_1.Variable(
                                                                                       Name:       commandArray[2],
                                                                                       Instance:   null
                                                                                   )
                                                                               )
                                                                           }
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region SetMonitoringBase

                                //   SetMonitoringBase
                                //   SetMonitoringBase factory
                                //   SetMonitoringBase hard
                                if (command == "SetMonitoringBase".ToLower() && (commandArray.Length == 1 || commandArray.Length == 2))
                                {

                                    var response = await testLCv2_1.OCPP.OUT.SetMonitoringBase(
                                                       new OCPPv2_1.CSMS.SetMonitoringBaseRequest(
                                                           NetworkingNodeId: NetworkingNode_Id.Parse(chargingStationId),
                                                           MonitoringBase:   commandArray[1] switch {
                                                                                 "factory"  => OCPPv2_1.MonitoringBase.FactoryDefault,
                                                                                 "hard"     => OCPPv2_1.MonitoringBase.HardWiredOnly,
                                                                                 _          => OCPPv2_1.MonitoringBase.All
                                                                             }
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region GetMonitoringReport

                                //   GetMonitoringReport component [variable]
                                if (command == "GetMonitoringReport".ToLower() && (commandArray.Length == 2 || commandArray.Length == 3))
                                {

                                    var response = await testLCv2_1.OCPP.OUT.GetMonitoringReport(
                                                       new OCPPv2_1.CSMS.GetMonitoringReportRequest(
                                                           NetworkingNodeId:               NetworkingNode_Id.Parse(chargingStationId),
                                                           GetMonitoringReportRequestId:   RandomExtensions.RandomInt32(),
                                                           MonitoringCriteria:             new[] {
                                                                                               OCPPv2_1.MonitoringCriterion.PeriodicMonitoring
                                                                                           },
                                                           ComponentVariables:             new[] {
                                                                                               new OCPPv2_1.ComponentVariable(
                                                                                                   new OCPPv2_1.Component(
                                                                                                       Name:       commandArray[1],
                                                                                                       Instance:   null,
                                                                                                       EVSE:       null
                                                                                                   ),
                                                                                                   commandArray.Length == 3
                                                                                                       ? new OCPPv2_1.Variable(
                                                                                                             Name:       commandArray[2],
                                                                                                             Instance:   null
                                                                                                         )
                                                                                                       : null
                                                                                               )
                                                                                           }
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region SetMonitoringLevel

                                //   SetMonitoringLevel debug
                                //   SetMonitoringLevel informational
                                //   SetMonitoringLevel notice
                                //   SetMonitoringLevel warning
                                //   SetMonitoringLevel alert
                                //   SetMonitoringLevel critical
                                //   SetMonitoringLevel systemfailure
                                //   SetMonitoringLevel hardwarefailure
                                //   SetMonitoringLevel danger
                                if (command == "SetMonitoringLevel".ToLower() && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.SetMonitoringLevel(
                                                       new OCPPv2_1.CSMS.SetMonitoringLevelRequest(
                                                           NetworkingNodeId:    NetworkingNode_Id.Parse(chargingStationId),
                                                           Severity:      commandArray[1].ToLower() switch {
                                                                              "danger"           => OCPPv2_1.Severities.Danger,
                                                                              "hardwarefailure"  => OCPPv2_1.Severities.HardwareFailure,
                                                                              "systemfailure"    => OCPPv2_1.Severities.SystemFailure,
                                                                              "critical"         => OCPPv2_1.Severities.Critical,
                                                                              "alert"            => OCPPv2_1.Severities.Alert,
                                                                              "warning"          => OCPPv2_1.Severities.Warning,
                                                                              "notice"           => OCPPv2_1.Severities.Notice,
                                                                              "informational"    => OCPPv2_1.Severities.Informational,
                                                                              "debug"            => OCPPv2_1.Severities.Debug,
                                                                              _                  => OCPPv2_1.Severities.Error
                                                                          }
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region ClearVariableMonitoring

                                //   ClearVariableMonitoring 1
                                if (command == "ClearVariableMonitoring".ToLower() && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.ClearVariableMonitoring(
                                                       new OCPPv2_1.CSMS.ClearVariableMonitoringRequest(
                                                           NetworkingNodeId:        NetworkingNode_Id.Parse(chargingStationId),
                                                           VariableMonitoringIds:   new[] {
                                                                                        OCPPv2_1.VariableMonitoring_Id.Parse(commandArray[1])
                                                                                    }
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                // SetNetworkProfile

                                #region Change Availability

                                //   ChangeAvailability operative
                                //   ChangeAvailability inoperative
                                if (command == "ChangeAvailability".ToLower() && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.ChangeAvailability(
                                                       new OCPPv2_1.CSMS.ChangeAvailabilityRequest(
                                                           NetworkingNodeId:    NetworkingNode_Id.Parse(chargingStationId),
                                                           OperationalStatus:   commandArray[1].ToLower() switch {
                                                                                    "operative"  => OCPPv2_1.OperationalStatus.Operative,
                                                                                    _            => OCPPv2_1.OperationalStatus.Inoperative
                                                                                }
                                                       )
                                                   );


                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }



                                //   ChangeAvailability 1 operative
                                //   ChangeAvailability 1 inoperative
                                if (command == "ChangeAvailability".ToLower() && commandArray.Length == 3)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.ChangeAvailability(
                                                       new OCPPv2_1.CSMS.ChangeAvailabilityRequest(
                                                           NetworkingNodeId:    NetworkingNode_Id.Parse(chargingStationId),
                                                           OperationalStatus:   commandArray[2].ToLower() switch {
                                                                                    "operative"  => OCPPv2_1.OperationalStatus.Operative,
                                                                                    _            => OCPPv2_1.OperationalStatus.Inoperative
                                                                                },
                                                           EVSE:                new OCPPv2_1.EVSE(
                                                                                    Id:  OCPPv2_1.EVSE_Id.Parse(commandArray[1])
                                                                                )
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }



                                //   ChangeAvailability 1 1 operative
                                //   ChangeAvailability 1 1 inoperative
                                if (command == "ChangeAvailability".ToLower() && commandArray.Length == 4)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.ChangeAvailability(
                                                       new OCPPv2_1.CSMS.ChangeAvailabilityRequest(
                                                           NetworkingNodeId:    NetworkingNode_Id.Parse(chargingStationId),
                                                           OperationalStatus:   commandArray[3].ToLower() switch {
                                                                                    "operative"  => OCPPv2_1.OperationalStatus.Operative,
                                                                                    _            => OCPPv2_1.OperationalStatus.Inoperative
                                                                                },
                                                           EVSE:                new OCPPv2_1.EVSE(
                                                                                    Id:            OCPPv2_1.EVSE_Id.     Parse(commandArray[1]),
                                                                                    ConnectorId:   OCPPv2_1.Connector_Id.Parse(commandArray[2])
                                                                                )
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region Trigger Message

                                //   TriggerMessage BootNotification
                                //   TriggerMessage LogStatusNotification
                                //   TriggerMessage DiagnosticsStatusNotification
                                //   TriggerMessage FirmwareStatusNotification
                                //   TriggerMessage Heartbeat
                                //   TriggerMessage MeterValues
                                //   TriggerMessage SignChargePointCertificate
                                if (command == "TriggerMessage".ToLower() && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.TriggerMessage(
                                                       new OCPPv2_1.CSMS.TriggerMessageRequest(
                                                           NetworkingNodeId:   NetworkingNode_Id.Parse(chargingStationId),
                                                           RequestedMessage:   commandArray[1].ToLower() switch {
                                                                                   "bootnotification"                => cloud.charging.open.protocols.OCPP.MessageTrigger.BootNotification,
                                                                                   "logstatusnotification"           => cloud.charging.open.protocols.OCPP.MessageTrigger.LogStatusNotification,
                                                                                   "diagnosticsstatusnotification"   => cloud.charging.open.protocols.OCPP.MessageTrigger.DiagnosticsStatusNotification,
                                                                                   "firmwarestatusnotification"      => cloud.charging.open.protocols.OCPP.MessageTrigger.FirmwareStatusNotification,
                                                                                   "metervalues"                     => cloud.charging.open.protocols.OCPP.MessageTrigger.MeterValues,
                                                                                   "SignChargingStationCertificate"  => cloud.charging.open.protocols.OCPP.MessageTrigger.SignChargingStationCertificate,
                                                                                   "statusnotification"              => cloud.charging.open.protocols.OCPP.MessageTrigger.StatusNotification,
                                                                                   _                                 => cloud.charging.open.protocols.OCPP.MessageTrigger.Heartbeat
                                                                               }
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }



                                //   TriggerMessage 1 BootNotification
                                //   TriggerMessage 1 LogStatusNotification
                                //   TriggerMessage 1 DiagnosticsStatusNotification
                                //   TriggerMessage 1 FirmwareStatusNotification
                                //   TriggerMessage 1 Heartbeat
                                //   TriggerMessage 1 MeterValues
                                //   TriggerMessage 1 SignChargePointCertificate
                                if (command == "TriggerMessage".ToLower() && commandArray.Length == 3)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.TriggerMessage(
                                                       new OCPPv2_1.CSMS.TriggerMessageRequest(
                                                           NetworkingNodeId:   NetworkingNode_Id.Parse(chargingStationId),
                                                           RequestedMessage:   commandArray[2].ToLower() switch {
                                                                                   "bootnotification"                => cloud.charging.open.protocols.OCPP.MessageTrigger.BootNotification,
                                                                                   "logstatusnotification"           => cloud.charging.open.protocols.OCPP.MessageTrigger.LogStatusNotification,
                                                                                   "diagnosticsstatusnotification"   => cloud.charging.open.protocols.OCPP.MessageTrigger.DiagnosticsStatusNotification,
                                                                                   "firmwarestatusnotification"      => cloud.charging.open.protocols.OCPP.MessageTrigger.FirmwareStatusNotification,
                                                                                   "metervalues"                     => cloud.charging.open.protocols.OCPP.MessageTrigger.MeterValues,
                                                                                   "SignChargingStationCertificate"  => cloud.charging.open.protocols.OCPP.MessageTrigger.SignChargingStationCertificate,
                                                                                   "statusnotification"              => cloud.charging.open.protocols.OCPP.MessageTrigger.StatusNotification,
                                                                                   _                                 => cloud.charging.open.protocols.OCPP.MessageTrigger.Heartbeat
                                                                               },
                                                           EVSE:               new OCPPv2_1.EVSE(
                                                                                   OCPPv2_1.EVSE_Id.Parse(commandArray[1])
                                                                               )
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }


                                //   TriggerMessage 1 1 StatusNotification
                                if (command == "TriggerMessage".ToLower() && commandArray.Length == 4)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.TriggerMessage(
                                                       new OCPPv2_1.CSMS.TriggerMessageRequest(
                                                           NetworkingNodeId:   NetworkingNode_Id.Parse(chargingStationId),
                                                           RequestedMessage:   commandArray[3].ToLower() switch {
                                                                                   "bootnotification"                => cloud.charging.open.protocols.OCPP.MessageTrigger.BootNotification,
                                                                                   "logstatusnotification"           => cloud.charging.open.protocols.OCPP.MessageTrigger.LogStatusNotification,
                                                                                   "diagnosticsstatusnotification"   => cloud.charging.open.protocols.OCPP.MessageTrigger.DiagnosticsStatusNotification,
                                                                                   "firmwarestatusnotification"      => cloud.charging.open.protocols.OCPP.MessageTrigger.FirmwareStatusNotification,
                                                                                   "metervalues"                     => cloud.charging.open.protocols.OCPP.MessageTrigger.MeterValues,
                                                                                   "SignChargingStationCertificate"  => cloud.charging.open.protocols.OCPP.MessageTrigger.SignChargingStationCertificate,
                                                                                   "statusnotification"              => cloud.charging.open.protocols.OCPP.MessageTrigger.StatusNotification,
                                                                                   _                                 => cloud.charging.open.protocols.OCPP.MessageTrigger.Heartbeat
                                                                               },
                                                           EVSE:               new OCPPv2_1.EVSE(
                                                                                   OCPPv2_1.EVSE_Id.     Parse(commandArray[1]),
                                                                                   OCPPv2_1.Connector_Id.Parse(commandArray[2])
                                                                               )
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region Update Firmware

                                //   UpdateFirmware http://95.89.178.27:9901/firmware.bin
                                if (command == "UpdateFirmware".ToLower() && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.UpdateFirmware(
                                                       new OCPPv2_1.CSMS.UpdateFirmwareRequest(
                                                           NetworkingNodeId:          NetworkingNode_Id.Parse(chargingStationId),
                                                           Firmware:                  new OCPPv2_1.Firmware(
                                                                                          FirmwareURL:          URL.Parse(commandArray[1]),
                                                                                          RetrieveTimestamp:    Timestamp.Now,
                                                                                          InstallTimestamp:     Timestamp.Now + TimeSpan.FromMinutes(1),
                                                                                          SigningCertificate:   "-----BEGIN CERTIFICATE-----\n" +
                                                                                                                "MIICFzCCAZwCFCqVyLDfPQJywMwU7pwXbiUREPH/MAoGCCqGSM49BAMDMGoxCzAJ\n" +
                                                                                                                "BgNVBAYTAk5MMRIwEAYDVQQIDAlGbGV2b2xhbmQxDzANBgNVBAcMBkFsbWVyZTET\n" +
                                                                                                                "MBEGA1UECgwKQWxmZW4gTi5WLjEMMAoGA1UECwwDQUNFMRMwEQYDVQQDDApBSFdQ\n" +
                                                                                                                "MDEtREVWMCAXDTIyMDIwODEzMTEzNFoYDzIwNjQwMjA4MTMxMTM0WjByMQswCQYD\n" +
                                                                                                                "VQQGEwJOTDESMBAGA1UECAwJRmxldm9sYW5kMQ8wDQYDVQQHDAZBbG1lcmUxEzAR\n" +
                                                                                                                "BgNVBAoMCkFsZmVuIE4uVi4xDDAKBgNVBAsMA0FDRTEbMBkGA1UEAwwSQUhXUC1E\n" +
                                                                                                                "RVYtRGV2ZWxvcGVyMHYwEAYHKoZIzj0CAQYFK4EEACIDYgAEdjJ42sfXY7af4BaT\n" +
                                                                                                                "2SU69RUtl5Wudb/wj/X8t19HYkQdqMg7R93AN6+K8x1ZGb+YWRLsPWt/EtYhmvAc\n" +
                                                                                                                "77Hjbu/ufori4IBs5qgQGa9na/alvexSG0qShRs79FUZIKcFMAoGCCqGSM49BAMD\n" +
                                                                                                                "A2kAMGYCMQDWOw4qFA1NFfVspD3NkL7D8fSppDmQAWAn+KFdqhs/1rhP1ldt822C\n" +
                                                                                                                "eEzEBzdUfp0CMQCzFmVQbAuxwn9sMoiB7GSpaMa2ayT0WJcgoLSaFFet2sf2ZlJy\n" +
                                                                                                                "9nHH2QCphACm184=\n" +
                                                                                                                "-----END CERTIFICATE-----\n",
                                                                                          Signature:            "3066023100f3e4beaa47d963d90051233603f59ade779aacd7d8939bcf41b5dc7a9cf139b433d859dfb6d4fbb885d32b225da6fa42023100cc10f35ba4440a69b1789bed7a3031eb30f5cdf2ce8ea2e9070968eaa862ad9e6fb334652184e46925a0df79355b74e8"
                                                                                      ),
                                                           UpdateFirmwareRequestId:   RandomExtensions.RandomInt32(),
                                                           Retries:                   3,
                                                           RetryInterval:             TimeSpan.FromSeconds(10)
                                                       )
                                                   );;

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region Transfer Data

                                //   TransferData graphdefined
                                if (command == "transferdata".ToLower() && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.DataTransfer(
                                                       new OCPPv2_1.DataTransferRequest(
                                                           NetworkingNodeId:   NetworkingNode_Id.Parse(chargingStationId),
                                                           VendorId:           Vendor_Id.Parse(commandArray[1])
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }




                                //   TransferData graphdefined message
                                if (command == "transferdata".ToLower() && (commandArray.Length == 2 || commandArray.Length == 3))
                                {

                                    var response = await testLCv2_1.OCPP.OUT.DataTransfer(
                                                       new OCPPv2_1.DataTransferRequest(
                                                           NetworkingNodeId:    NetworkingNode_Id.Parse(chargingStationId),
                                                           VendorId:            Vendor_Id.   Parse(commandArray[1]),
                                                           MessageId:           commandArray.Length == 3 ? Message_Id.Parse(commandArray[2]) : null
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }




                                //   TransferData graphdefined message data
                                if (command == "transferdata".ToLower() && commandArray.Length == 4)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.DataTransfer(
                                                       new OCPPv2_1.DataTransferRequest(
                                                           NetworkingNodeId:   NetworkingNode_Id.Parse(chargingStationId),
                                                           VendorId:           Vendor_Id. Parse(commandArray[1]),
                                                           MessageId:          Message_Id.Parse(commandArray[2]),
                                                           Data:               commandArray[3]
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion


                                #region SendSignedCertificate

                                //   SendSignedCertificate $Filename v2g|csc
                                if (command == "SendSignedCertificate".ToLower() && commandArray.Length == 3)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.CertificateSigned(
                                                       new OCPPv2_1.CSMS.CertificateSignedRequest(
                                                           NetworkingNodeId:   NetworkingNode_Id.Parse(chargingStationId),
                                                           CertificateChain:   OCPPv2_1.CertificateChain.Parse(
                                                                                   File.ReadAllText(commandArray[1])
                                                                               ),
                                                           CertificateType:    commandArray[2].ToLower() switch {
                                                                                   "v2g"  => OCPPv2_1.CertificateSigningUse.V2GCertificate,
                                                                                   _      => OCPPv2_1.CertificateSigningUse.ChargingStationCertificate
                                                                               }
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region InstallCertificate

                                if (command == "InstallCertificate".ToLower() && commandArray.Length == 3)
                                {

                                    //   InstallCertificate $FileName oem|mo|csms|manu|v2g
                                    var response = await testLCv2_1.OCPP.OUT.InstallCertificate(
                                                       new OCPPv2_1.CSMS.InstallCertificateRequest(
                                                           NetworkingNodeId:  NetworkingNode_Id.Parse(chargingStationId),
                                                           CertificateType:   commandArray[2].ToLower() switch {
                                                                                  "oem"   => OCPPv2_1.InstallCertificateUse.OEMRootCertificate,
                                                                                  "mo"    => OCPPv2_1.InstallCertificateUse.MORootCertificate,
                                                                                  "csms"  => OCPPv2_1.InstallCertificateUse.CSMSRootCertificate,
                                                                                  "manu"  => OCPPv2_1.InstallCertificateUse.ManufacturerRootCertificate,
                                                                                  _       => OCPPv2_1.InstallCertificateUse.V2GRootCertificate
                                                           },
                                                           Certificate:       OCPPv2_1.Certificate.Parse(
                                                                                  File.ReadAllText(commandArray[1])
                                                                              )
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region GetInstalledCertificateIds

                                if (command == "GetInstalledCertificateIds".ToLower() && commandArray.Length == 2)
                                {

                                    //   GetInstalledCertificateIds v2grc
                                    //   GetInstalledCertificateIds morc
                                    //   GetInstalledCertificateIds csrc
                                    //   GetInstalledCertificateIds v2gcc
                                    var response = await testLCv2_1.OCPP.OUT.GetInstalledCertificateIds(
                                                       new OCPPv2_1.CSMS.GetInstalledCertificateIdsRequest(
                                                           NetworkingNodeId:   NetworkingNode_Id.Parse(chargingStationId),
                                                           CertificateTypes:   new[] {
                                                                                   commandArray[1].ToLower() switch {
                                                                                       "v2g"   => OCPPv2_1.GetCertificateIdUse.V2GRootCertificate,
                                                                                       "mo"    => OCPPv2_1.GetCertificateIdUse.MORootCertificate,
                                                                                       "csms"  => OCPPv2_1.GetCertificateIdUse.CSMSRootCertificate,
                                                                                       "manu"  => OCPPv2_1.GetCertificateIdUse.ManufacturerRootCertificate,
                                                                                       "oem"   => OCPPv2_1.GetCertificateIdUse.OEMRootCertificate,
                                                                                       _       => OCPPv2_1.GetCertificateIdUse.V2GCertificateChain
                                                                                   }
                                                                               }
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region DeleteCertificate

                                //   DeleteCertificate $HashAlgorithm $IssuerNameHash $IssuerPublicKeyHash $SerialNumber
                                if (command == "DeleteCertificate".ToLower() && commandArray.Length == 5)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.DeleteCertificate(
                                                       new OCPPv2_1.CSMS.DeleteCertificateRequest(
                                                           NetworkingNodeId:      NetworkingNode_Id.Parse(chargingStationId),
                                                           CertificateHashData:   new OCPPv2_1.CertificateHashData(
                                                                                      commandArray[1].ToLower() switch {
                                                                                          "sha512"  => OCPPv2_1.HashAlgorithms.SHA512,
                                                                                          "sha384"  => OCPPv2_1.HashAlgorithms.SHA384,
                                                                                          _         => OCPPv2_1.HashAlgorithms.SHA256
                                                                                      },
                                                                                      commandArray[2],
                                                                                      commandArray[3],
                                                                                      commandArray[4]
                                                                                  )
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                // NotifyCRLAvailability


                                #region GetLocalListVersion

                                //   GetLocalListVersion
                                if (command == "GetLocalListVersion".ToLower() && commandArray.Length == 1)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.GetLocalListVersion(
                                                       new OCPPv2_1.CSMS.GetLocalListVersionRequest(
                                                           NetworkingNode_Id.Parse(chargingStationId)
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region SendLocalList

                                //   SendLocalList
                                if (command == "SendLocalList".ToLower() && commandArray.Length == 1)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.SendLocalList(
                                                       new OCPPv2_1.CSMS.SendLocalListRequest(
                                                       NetworkingNode_Id.Parse(chargingStationId),
                                                             1, // 0 is not allowed!
                                                             OCPPv2_1.UpdateTypes.Full,
                                                             new[] {
                                                                 //new OCPPv2_1.AuthorizationData(OCPPv2_1.IdToken.Parse("046938f2fc6880"), new OCPPv2_1.IdTagInfo(OCPPv2_1.AuthorizationStatus.Blocked)),
                                                                 //new OCPPv2_1.AuthorizationData(OCPPv2_1.IdToken.Parse("aabbcc11"),       new OCPPv2_1.IdTagInfo(OCPPv2_1.AuthorizationStatus.Accepted)),
                                                                 //new OCPPv2_1.AuthorizationData(OCPPv2_1.IdToken.Parse("aabbcc22"),       new OCPPv2_1.IdTagInfo(OCPPv2_1.AuthorizationStatus.Accepted)),
                                                                 //new OCPPv2_1.AuthorizationData(OCPPv2_1.IdToken.Parse("aabbcc33"),       new OCPPv2_1.IdTagInfo(OCPPv2_1.AuthorizationStatus.Accepted)),
                                                                 new OCPPv2_1.AuthorizationData(
                                                                     new OCPPv2_1.IdToken(
                                                                         Value:   "cabot",
                                                                         Type:    OCPPv2_1.IdTokenType.ISO14443
                                                                     ),
                                                                     new OCPPv2_1.IdTokenInfo(
                                                                         OCPPv2_1.AuthorizationStatus.Accepted
                                                                     )
                                                                 )
                                                             }
                                                         )
                                                     );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region ClearCache

                                //   clearcache GD002
                                if (command == "clearcache"             && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.ClearCache(
                                                       new OCPPv2_1.CSMS.ClearCacheRequest(
                                                           NetworkingNode_Id.Parse(chargingStationId)
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion


                                #region ReserveNow

                                //   ReserveNow 1 $ReservationId aabbccdd
                                if (command == "ReserveNow".ToLower() && commandArray.Length == 4)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.ReserveNow(
                                                       new OCPPv2_1.CSMS.ReserveNowRequest(
                                                           NetworkingNodeId:   NetworkingNode_Id.  Parse(chargingStationId),
                                                           Id:                 OCPPv2_1.Reservation_Id.Parse(commandArray[2]),
                                                           ExpiryDate:         Timestamp.Now + TimeSpan.FromMinutes(15),
                                                           IdToken:            new OCPPv2_1.IdToken(
                                                                                   Value:             commandArray[3],
                                                                                   Type:              OCPPv2_1.IdTokenType.eMAID,
                                                                                   AdditionalInfos:   null
                                                                               ),
                                                           ConnectorType:      null, //OCPPv2_1.ConnectorTypes.sType2,
                                                           EVSEId:             OCPPv2_1.EVSE_Id.       Parse(commandArray[1]),
                                                           GroupIdToken:       null
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region Cancel Reservation

                                //   CancelReservation $ReservationId
                                if (command == "CancelReservation".ToLower() && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.CancelReservation(
                                                       new OCPPv2_1.CSMS.CancelReservationRequest(
                                                           NetworkingNodeId:  NetworkingNode_Id.Parse(chargingStationId),
                                                           ReservationId:     OCPPv2_1.Reservation_Id.   Parse(commandArray[1])
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region Remote Start Transaction

                                //   RemoteStart 1 $IdToken
                                if (command == "remotestart".ToLower() && commandArray.Length == 3)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.RequestStartTransaction(
                                                       new OCPPv2_1.CSMS.RequestStartTransactionRequest(
                                                           NetworkingNodeId:                   NetworkingNode_Id.Parse(chargingStationId),
                                                           RequestStartTransactionRequestId:   OCPPv2_1.RemoteStart_Id.NewRandom,
                                                           IdToken:                            new OCPPv2_1.IdToken(
                                                                                                   Value:             commandArray[2],
                                                                                                   Type:              OCPPv2_1.IdTokenType.ISO14443,
                                                                                                   AdditionalInfos:   null
                                                                                               ),
                                                           EVSEId:                             OCPPv2_1.EVSE_Id.Parse(commandArray[1]),
                                                           ChargingProfile:                    null,
                                                           GroupIdToken:                       null
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region Remote Stop Transaction

                                //   RemoteStop $TransactionId
                                if (command == "RemoteStop".ToLower() && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.RequestStopTransaction(
                                                       new OCPPv2_1.CSMS.RequestStopTransactionRequest(
                                                           NetworkingNodeId:   NetworkingNode_Id.Parse(chargingStationId),
                                                           TransactionId:      OCPPv2_1.Transaction_Id.    Parse(commandArray[1])
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region GetTransactionStatus

                                //   GetTransactionStatus
                                if (command == "GetTransactionStatus".ToLower() && commandArray.Length == 1)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.GetTransactionStatus(
                                                       new OCPPv2_1.CSMS.GetTransactionStatusRequest(
                                                           NetworkingNodeId:    NetworkingNode_Id.Parse(chargingStationId)
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }



                                //   GetTransactionStatus $TransactionId
                                if (command == "GetTransactionStatus".ToLower() && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.GetTransactionStatus(
                                                       new OCPPv2_1.CSMS.GetTransactionStatusRequest(
                                                           NetworkingNodeId:    NetworkingNode_Id.Parse(chargingStationId),
                                                           TransactionId:       OCPPv2_1.Transaction_Id.    Parse(commandArray[1])
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region SetChargingProfile

                                //   setprofile1 1
                                if (command == "setprofile1"            && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.SetChargingProfile(
                                                       new OCPPv2_1.CSMS.SetChargingProfileRequest(
                                                           NetworkingNodeId:   NetworkingNode_Id.Parse(chargingStationId),
                                                           EVSEId:             OCPPv2_1.EVSE_Id.     Parse(commandArray[1]),
                                                           ChargingProfile:    new OCPPv2_1.ChargingProfile(
                                                                                   OCPPv2_1.ChargingProfile_Id.Parse("100"),
                                                                                   0,
                                                                                   OCPPv2_1.ChargingProfilePurpose.TxDefaultProfile,
                                                                                   OCPPv2_1.ChargingProfileKinds.Recurring,
                                                                                   [
                                                                                       new OCPPv2_1.ChargingSchedule(
                                                                                           Id:                       OCPPv2_1.ChargingSchedule_Id.Parse("1"),
                                                                                           ChargingRateUnit:         OCPPv2_1.ChargingRateUnits.Amperes,
                                                                                           ChargingSchedulePeriods:  [
                                                                                                                         new OCPPv2_1.ChargingSchedulePeriod(
                                                                                                                             StartPeriod:     TimeSpan.FromHours(0),  // == 00:00 Uhr
                                                                                                                             Limit:           OCPPv2_1.ChargingRateValue.Parse(16, OCPPv2_1.ChargingRateUnits.Amperes),
                                                                                                                             NumberOfPhases:  3
                                                                                                                         ),
                                                                                                                         new OCPPv2_1.ChargingSchedulePeriod(
                                                                                                                             StartPeriod:     TimeSpan.FromHours(8),  // == 08:00 Uhr
                                                                                                                             Limit:           OCPPv2_1.ChargingRateValue.Parse(6,  OCPPv2_1.ChargingRateUnits.Amperes),
                                                                                                                             NumberOfPhases:  3
                                                                                                                         ),
                                                                                                                         new OCPPv2_1.ChargingSchedulePeriod(
                                                                                                                             StartPeriod:     TimeSpan.FromHours(20), // == 20:00 Uhr
                                                                                                                             Limit:           OCPPv2_1.ChargingRateValue.Parse(12, OCPPv2_1.ChargingRateUnits.Amperes),
                                                                                                                             NumberOfPhases:  3
                                                                                                                         )
                                                                                                                     ],
                                                                                           Duration:                 TimeSpan.FromDays(7),
                                                                                           StartSchedule:            DateTime.Parse("2023-03-29T00:00:00Z").ToUniversalTime()
                                                                                       )
                                                                                   ],
                                                                                   null, //Transaction_Id.TryParse(5678),
                                                                                   OCPPv2_1.RecurrencyKinds.Daily,
                                                                                   DateTime.Parse("2022-11-01T00:00:00Z").ToUniversalTime(),
                                                                                   DateTime.Parse("2023-12-01T00:00:00Z").ToUniversalTime()
                                                                               )
                                                           )
                                                       );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                //   setprofile2 GD002 1
                                if (command == "setprofile2"            && commandArray.Length == 3)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.SetChargingProfile(
                                                       new OCPPv2_1.CSMS.SetChargingProfileRequest(
                                                           NetworkingNodeId:   NetworkingNode_Id.Parse(chargingStationId),
                                                           EVSEId:             OCPPv2_1.EVSE_Id.     Parse(commandArray[2]),
                                                           ChargingProfile:    new OCPPv2_1.ChargingProfile(
                                                                                   OCPPv2_1.ChargingProfile_Id.Parse("100"),
                                                                                   2,
                                                                                   OCPPv2_1.ChargingProfilePurpose.TxDefaultProfile,
                                                                                   OCPPv2_1.ChargingProfileKinds.Recurring,
                                                                                   [
                                                                                       new OCPPv2_1.ChargingSchedule(
                                                                                           Id:                       OCPPv2_1.ChargingSchedule_Id.Parse("1"),
                                                                                           ChargingRateUnit:         OCPPv2_1.ChargingRateUnits.Amperes,
                                                                                           ChargingSchedulePeriods:  [
                                                                                                                         new OCPPv2_1.ChargingSchedulePeriod(
                                                                                                                             StartPeriod:     TimeSpan.FromHours(0),  // == 00:00 Uhr
                                                                                                                             Limit:           OCPPv2_1.ChargingRateValue.Parse(11, OCPPv2_1.ChargingRateUnits.Amperes),
                                                                                                                             NumberOfPhases:  3
                                                                                                                         ),
                                                                                                                         new OCPPv2_1.ChargingSchedulePeriod(
                                                                                                                             StartPeriod:     TimeSpan.FromHours(6),  // == 06:00 Uhr
                                                                                                                             Limit:           OCPPv2_1.ChargingRateValue.Parse(6,  OCPPv2_1.ChargingRateUnits.Amperes),
                                                                                                                             NumberOfPhases:  3
                                                                                                                         ),
                                                                                                                         new OCPPv2_1.ChargingSchedulePeriod(
                                                                                                                             StartPeriod:     TimeSpan.FromHours(21), // == 21:00 Uhr
                                                                                                                             Limit:           OCPPv2_1.ChargingRateValue.Parse(11, OCPPv2_1.ChargingRateUnits.Amperes),
                                                                                                                             NumberOfPhases:  3
                                                                                                                         )
                                                                                                                     ],
                                                                                           Duration:                 TimeSpan.FromDays(7),
                                                                                           StartSchedule:            DateTime.Parse("2023-03-29T00:00:00Z").ToUniversalTime()
                                                                                       )
                                                                                   ],
                                                                                   null, //Transaction_Id.TryParse(6789),
                                                                                   OCPPv2_1.RecurrencyKinds.Daily,
                                                                                   DateTime.Parse("2022-11-01T00:00:00Z").ToUniversalTime(),
                                                                                   DateTime.Parse("2023-12-01T00:00:00Z").ToUniversalTime()
                                                                               )
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region GetChargingProfiles

                                //   GetChargingProfiles
                                if (command == "GetChargingProfiles".ToLower() && commandArray.Length == 1)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.GetChargingProfiles(
                                                       new OCPPv2_1.CSMS.GetChargingProfilesRequest(
                                                           NetworkingNodeId:               NetworkingNode_Id.Parse(chargingStationId),
                                                           GetChargingProfilesRequestId:   RandomExtensions.RandomInt32(),
                                                           ChargingProfile:                new OCPPv2_1.ChargingProfileCriterion(
                                                                                               ChargingProfilePurpose:   OCPPv2_1.ChargingProfilePurpose.TxDefaultProfile,
                                                                                               StackLevel:               null,
                                                                                               ChargingProfileIds:       null,
                                                                                               ChargingLimitSources:     null
                                                                                           ),
                                                           EVSEId:                         null
                                                       )
                                                   );;

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }



                                //   GetChargingProfiles $ChargingProfileId
                                if (command == "GetChargingProfiles".ToLower() && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.GetChargingProfiles(
                                                       new OCPPv2_1.CSMS.GetChargingProfilesRequest(
                                                           NetworkingNodeId:               NetworkingNode_Id.Parse(chargingStationId),
                                                           GetChargingProfilesRequestId:   RandomExtensions.RandomInt32(),
                                                           ChargingProfile:                new OCPPv2_1.ChargingProfileCriterion(
                                                                                               ChargingProfilePurpose:   null,
                                                                                               StackLevel:               null,
                                                                                               ChargingProfileIds:       new[] {
                                                                                                                             OCPPv2_1.ChargingProfile_Id.Parse(commandArray[1])
                                                                                                                         },
                                                                                               ChargingLimitSources:     null
                                                                                           ),
                                                           EVSEId:                         null
                                                       )
                                                   );;

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region ClearChargingProfile

                                //   ClearChargingProfile
                                if (command == "ClearChargingProfile".ToLower() && commandArray.Length == 1)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.ClearChargingProfile(
                                                       new OCPPv2_1.CSMS.ClearChargingProfileRequest(
                                                           NetworkingNodeId:   NetworkingNode_Id.Parse(chargingStationId)
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }



                                //   ClearChargingProfile $ChargingProfileId
                                if (command == "ClearChargingProfile".ToLower() && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.ClearChargingProfile(
                                                       new OCPPv2_1.CSMS.ClearChargingProfileRequest(
                                                           NetworkingNodeId:    NetworkingNode_Id.Parse(chargingStationId),
                                                           ChargingProfileId:   OCPPv2_1.ChargingProfile_Id.Parse(commandArray[1])
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }



                                //   ClearChargingProfile $ConnectorId/EVSEId $ChargingProfileId
                                if (command == "ClearChargingProfile".ToLower() && commandArray.Length == 3)
                                {

                                    //   ClearChargingProfile $EVSEId $ChargingProfileId
                                    var response = await testLCv2_1.OCPP.OUT.ClearChargingProfile(
                                                       new OCPPv2_1.CSMS.ClearChargingProfileRequest(
                                                           NetworkingNodeId:          NetworkingNode_Id.Parse(chargingStationId),
                                                           ChargingProfileId:         OCPPv2_1.ChargingProfile_Id.Parse(commandArray[2]),
                                                           ChargingProfileCriteria:   new OCPPv2_1.ClearChargingProfile(
                                                                                          EVSEId:                   OCPPv2_1.EVSE_Id.Parse(commandArray[1]),
                                                                                          ChargingProfilePurpose:   null,
                                                                                          StackLevel:               null
                                                                                      )
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region GetCompositeSchedule

                                //   GetCompositeSchedule 1 3600
                                if (command == "GetCompositeSchedule".ToLower() && commandArray.Length == 3)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.GetCompositeSchedule(
                                                       new OCPPv2_1.CSMS.GetCompositeScheduleRequest(
                                                           NetworkingNodeId:   NetworkingNode_Id.Parse(chargingStationId),
                                                           Duration:           TimeSpan.FromSeconds(UInt32.Parse(commandArray[2])),
                                                           EVSEId:             OCPPv2_1.EVSE_Id.Parse(commandArray[1]),
                                                           ChargingRateUnit:   null
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                // UpdateDynamicSchedule

                                // NotifyAllowedenergyTransfer

                                // UsePriorityCharging

                                #region Unlock Connector

                                //   UnlockConnector 1 1
                                if (command == "UnlockConnector".ToLower() && commandArray.Length == 3)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.UnlockConnector(
                                                       new OCPPv2_1.CSMS.UnlockConnectorRequest(
                                                           NetworkingNodeId:    NetworkingNode_Id.Parse(chargingStationId),
                                                           EVSEId:        OCPPv2_1.EVSE_Id.     Parse(commandArray[1]),
                                                           ConnectorId:   OCPPv2_1.Connector_Id.Parse(commandArray[2])
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion


                                // SendAFDRSignal


                                #region SetDisplayMessage

                                //   SetDisplayMessage test123
                                if (command == "SetDisplayMessage".ToLower() && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.SetDisplayMessage(
                                                       new OCPPv2_1.CSMS.SetDisplayMessageRequest(
                                                           NetworkingNodeId:    NetworkingNode_Id.Parse(chargingStationId),
                                                           Message:             new OCPPv2_1.MessageInfo(
                                                                                    Id:               OCPPv2_1.DisplayMessage_Id.NewRandom,
                                                                                    Priority:         OCPPv2_1.MessagePriority.NormalCycle,
                                                                                    Message:          new OCPPv2_1.MessageContent(
                                                                                                          Content:      commandArray[1],
                                                                                                          Language:     OCPPv2_1.Language_Id.EN,
                                                                                                          Format:       OCPPv2_1.MessageFormat.UTF8,
                                                                                                          CustomData:   null
                                                                                                      ),
                                                                                    State:            OCPPv2_1.MessageState.Idle,
                                                                                    StartTimestamp:   Timestamp.Now,
                                                                                    EndTimestamp:     Timestamp.Now + TimeSpan.FromHours(1),
                                                                                    TransactionId:    null,
                                                                                    Display:          null
                                                                                )
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region GetDisplayMessages

                                //   GetDisplayMessages
                                if (command == "GetDisplayMessages".ToLower() && commandArray.Length == 1)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.GetDisplayMessages(
                                                       new OCPPv2_1.CSMS.GetDisplayMessagesRequest(
                                                           NetworkingNodeId:              NetworkingNode_Id.Parse(chargingStationId),
                                                           GetDisplayMessagesRequestId:   RandomExtensions.RandomInt32(),
                                                           Ids:                           null,
                                                           Priority:                      null,
                                                           State:                         null
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }



                                //   GetDisplayMessages 1
                                if (command == "GetDisplayMessages".ToLower() && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.GetDisplayMessages(
                                                       new OCPPv2_1.CSMS.GetDisplayMessagesRequest(
                                                           NetworkingNodeId:              NetworkingNode_Id.Parse(chargingStationId),
                                                           GetDisplayMessagesRequestId:   RandomExtensions.RandomInt32(),
                                                           Ids:                           new[] {
                                                                                              OCPPv2_1.DisplayMessage_Id.Parse(commandArray[1])
                                                                                          },
                                                           Priority:                      null,
                                                           State:                         null
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }



                                //   GetDisplayMessagesByState Idle
                                if (command == "GetDisplayMessagesByState".ToLower() && commandArray.Length == 2)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.GetDisplayMessages(
                                                       new OCPPv2_1.CSMS.GetDisplayMessagesRequest(
                                                           NetworkingNodeId:              NetworkingNode_Id.Parse(chargingStationId),
                                                           GetDisplayMessagesRequestId:   RandomExtensions.RandomInt32(),
                                                           Ids:                           null,
                                                           Priority:                      null,
                                                           State:                         OCPPv2_1.MessageState.Parse(commandArray[1].ToLower())
                                                                                             // charging
                                                                                             // faulted
                                                                                             // idle
                                                                                             // unavailable
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }
                                #endregion

                                #region SendCostUpdated

                                //   SendCostUpdate 123.45 ABCDEFG
                                if (command == "SendCostUpdate".ToLower() && commandArray.Length == 3)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.CostUpdated(
                                                       new OCPPv2_1.CSMS.CostUpdatedRequest(
                                                           NetworkingNodeId:    NetworkingNode_Id.Parse(chargingStationId),
                                                           TotalCost:           Decimal.                    Parse(commandArray[1]),
                                                           TransactionId:       OCPPv2_1.Transaction_Id.    Parse(commandArray[2])
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                                #region RequestCustomerInformation

                                //   RequestCustomerInformation $RFIDId
                                if (command == "RequestCustomerInformation".ToLower() && commandArray.Length == 1)
                                {

                                    var response = await testLCv2_1.OCPP.OUT.CustomerInformation(
                                                       new OCPPv2_1.CSMS.CustomerInformationRequest(
                                                           NetworkingNodeId:               NetworkingNode_Id.Parse(chargingStationId),
                                                           CustomerInformationRequestId:   RandomExtensions.RandomInt32(),
                                                           Report:                         true,
                                                           Clear:                          false,
                                                           CustomerIdentifier:             null,
                                                           IdToken:                        new OCPPv2_1.IdToken(
                                                                                               Value:             commandArray[1],
                                                                                               Type:              OCPPv2_1.IdTokenType.ISO14443,
                                                                                               AdditionalInfos:   null
                                                                                           ),
                                                           CustomerCertificate:            null
                                                       )
                                                   );

                                    DebugX.Log(commandArray.AggregateWith(" ") + " => " + response.Runtime.TotalMilliseconds + " ms");
                                    DebugX.Log(response.ToJSON().ToString());

                                }

                                #endregion

                            }

                        }

                    }

                }

                #region Handle exceptions

                catch (CommandException e)
                {
                    DebugX.Log($"Invalid command: {e.Message}");
                }
                catch (Exception e)
                {
                    DebugX.LogException(e);
                }

                #endregion

            } while (!quit);

            await testLCv2_1.Shutdown();

            foreach (var DebugListener in Trace.Listeners)
                (DebugListener as TextWriterTraceListener)?.Flush();

            #endregion


        }

    }

}
