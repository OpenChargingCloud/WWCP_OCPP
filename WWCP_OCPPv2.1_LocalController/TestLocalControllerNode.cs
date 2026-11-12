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

using System.Collections.Concurrent;
using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Norn.NTS;

using cloud.charging.open.protocols.OCPPv2_1.LC;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.OCPP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.LocalController
{

    /// <summary>
    /// An OCPP Local Controller node for testing.
    /// </summary>
    public partial class TestLocalControllerNode : ALocalControllerNode,
                                                   ILocalControllerNode
    {

        #region Data

        /// <summary>
        /// The reports of this local controller's own Device Model accepted by GetBaseReport or
        /// GetReport, until their response is sent: the NotifyReport requests follow it. Keyed by the
        /// request itself.
        /// </summary>
        private readonly ConcurrentDictionary<Object, IReadOnlyList<ReportData>> pendingReports = new (ReferenceEqualityComparer.Instance);

        #endregion

        #region Properties

        public HashSet<NetworkingNode_Id>  AllowedChargingStations    { get; } = [];

        /// <summary>
        /// What decides on a charging station's Authorize request in place of
        /// AllowedChargingStations: a controller that knows its stations by
        /// their logins, say, and forwards for whoever signed in. Null for the
        /// list.
        /// </summary>
        /// <remarks>
        /// A property rather than one more handler of OnAuthorizeRequestFilter:
        /// only the first filter of a request is asked, and that is the one
        /// this node adds for itself.
        /// </remarks>
        public OnAuthorizeRequestFilterDelegate?  AuthorizeRequestFilter    { get; set; }

        /// <summary>
        /// The largest number of report data in one NotifyReport request of this local controller's
        /// own report; a longer report is sent in several parts.
        /// </summary>
        public UInt16                      ReportDataPerNotifyReport  { get; set; } = 100;

        #endregion


        #region (private) SendPendingReport(Request, ReportRequestId, Destination, SentMessageResult)

        /// <summary>
        /// Send the report of this local controller's own Device Model accepted for the given request,
        /// now that its response is out.
        /// </summary>
        /// <remarks>
        /// The response of a report forwarded for a station is sent by this node too, and is not
        /// among the pending ones: the station sends its own report.
        /// </remarks>
        /// <param name="Request">The GetBaseReport or GetReport request.</param>
        /// <param name="ReportRequestId">The request id the NotifyReport requests refer to.</param>
        /// <param name="Destination">Whom to send the report to.</param>
        /// <param name="SentMessageResult">How sending the response went.</param>
        private void SendPendingReport(Object              Request,
                                       Int32               ReportRequestId,
                                       SourceRouting       Destination,
                                       SentMessageResults  SentMessageResult)
        {

            if (!pendingReports.TryRemove(Request, out var reportData) ||
                SentMessageResult != SentMessageResults.Success)
            {
                return;
            }

            // Not awaited here: this runs while the response is being sent, and the
            // answers to the NotifyReport requests come in over that same connection.
            _ = Task.Run(async () => {

                try
                {

                    var generatedAt = Now;

                    await DeviceModel.SendReportAsync(
                              reportData,
                              ReportDataPerNotifyReport,
                              (sequenceNumber, part, toBeContinued) => this.NotifyReport(
                                                                           NotifyReportRequestId:   ReportRequestId,
                                                                           SequenceNumber:          sequenceNumber,
                                                                           GeneratedAt:             generatedAt.UtcDateTime,
                                                                           ReportData:              part,
                                                                           ToBeContinued:           toBeContinued,
                                                                           Destination:             Destination
                                                                       )
                          );

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, $"Local controller '{Id}': NotifyReport {ReportRequestId}");
                }

            });

        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new local controller for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this local controller.</param>
        public TestLocalControllerNode(NetworkingNode_Id                         Id,
                                       String                                    VendorName,
                                       String                                    Model,
                                       String?                                   SerialNumber                     = null,
                                       String?                                   SoftwareVersion                  = null,
                                       Modem?                                    Modem                            = null,
                                       I18NString?                               Description                      = null,
                                       CustomData?                               CustomData                       = null,

                                       SignaturePolicy?                          SignaturePolicy                  = null,
                                       SignaturePolicy?                          ForwardingSignaturePolicy        = null,

                                       Func<ALocalControllerNode, HTTPAPI>?      HTTPAPI                          = null,
                                       Boolean                                   HTTPAPI_Disabled                 = false,
                                       IPPort?                                   HTTPAPI_Port                     = null,
                                       String?                                   HTTPAPI_ServerName               = null,
                                       String?                                   HTTPAPI_ServiceName              = null,
                                       EMailAddress?                             HTTPAPI_RobotEMailAddress        = null,
                                       String?                                   HTTPAPI_RobotGPGPassphrase       = null,
                                       Boolean                                   HTTPAPI_EventLoggingDisabled     = false,

                                       Func<ALocalControllerNode, DownloadAPI>?  HTTPDownloadAPI                  = null,
                                       Boolean                                   HTTPDownloadAPI_Disabled         = false,
                                       HTTPPath?                                 HTTPDownloadAPI_Path             = null,
                                       String?                                   HTTPDownloadAPI_FileSystemPath   = null,

                                       Func<ALocalControllerNode, UploadAPI>?    HTTPUploadAPI                    = null,
                                       Boolean                                   HTTPUploadAPI_Disabled           = false,
                                       HTTPPath?                                 HTTPUploadAPI_Path               = null,
                                       String?                                   HTTPUploadAPI_FileSystemPath     = null,

                                       Func<ALocalControllerNode, WebAPI>?       WebAPI                           = null,
                                       Boolean                                   WebAPI_Disabled                  = false,
                                       HTTPPath?                                 WebAPI_Path                      = null,

                                       Func<ALocalControllerNode, NTSServer>?    NTSServer                        = null,
                                       Boolean                                   NTSServer_Disabled               = true,

                                       WebSocketServer?                          ControlWebSocketServer           = null,

                                       TimeSpan?                                 DefaultRequestTimeout            = null,

                                       Boolean                                   DisableSendHeartbeats            = false,
                                       TimeSpan?                                 SendHeartbeatsEvery              = null,

                                       Boolean                                   DisableMaintenanceTasks          = false,
                                       TimeSpan?                                 MaintenanceEvery                 = null,
                                       DNSClient?                                DNSClient                        = null,

                                       TimeProvider?                             Clock                            = null)

            : base(Id,
                   VendorName,
                   Model,
                   SerialNumber,
                   SoftwareVersion,
                   Modem,
                   Description,
                   CustomData,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   HTTPAPI,
                   HTTPAPI_Disabled,
                   HTTPAPI_Port,
                   HTTPAPI_ServerName,
                   HTTPAPI_ServiceName,
                   HTTPAPI_RobotEMailAddress,
                   HTTPAPI_RobotGPGPassphrase,
                   HTTPAPI_EventLoggingDisabled,

                   HTTPDownloadAPI,
                   HTTPDownloadAPI_Disabled,
                   HTTPDownloadAPI_Path,
                   HTTPDownloadAPI_FileSystemPath,

                   HTTPUploadAPI,
                   HTTPUploadAPI_Disabled,
                   HTTPUploadAPI_Path,
                   HTTPUploadAPI_FileSystemPath,

                   WebAPI,
                   WebAPI_Disabled,
                   WebAPI_Path,

                   NTSServer,
                   NTSServer_Disabled,

                   ControlWebSocketServer,

                   DefaultRequestTimeout,

                   DisableSendHeartbeats,
                   SendHeartbeatsEvery,

                   DisableMaintenanceTasks,
                   MaintenanceEvery,
                   DNSClient,
                   Clock)

        {

            //Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "HTTPSSEs"));



            #region OnBootNotification

            // A local controller normally forwards a BootNotification, and OCPP.FORWARD below
            // does exactly that. But it can also be the CSMS for the stations behind it - a
            // node that answers to NetworkingNode_Id.CSMS itself rather than passing it on -
            // and then the request ends up here. Without a handler the adapter falls back to
            // BootNotificationResponse.Failed, so a station booting against its local
            // controller was told RegistrationStatus.Error and had no way to know why.
            OCPP.IN.OnBootNotification += (timestamp,
                                           sender,
                                           connection,
                                           request,
                                           cancellationToken) => {

                DebugX.Log($"'{Id}': Incoming BootNotification request: {request.ChargingStation.SerialNumber}, {request.Reason}");

                // The station just told us how to reach it: over the connection this arrived on.
                if (!Routing.LookupNetworkingNode(request.NetworkPath.Source, out _))
                {
                    Routing.AddOrUpdateStaticRouting(
                        request.NetworkPath.Source,
                        request.NetworkPath.Last
                    );
                }

                return Task.FromResult(
                           new BootNotificationResponse(
                               Request:               request,
                               NetworkPath:           NetworkPath.From(Id),
                               Status:                RegistrationStatus.Accepted,
                               CurrentTime:           Now,
                               Interval:              TimeSpan.FromMinutes(5),
                               SerializationFormat:   request.SerializationFormat
                           )
                       );

            };

            #endregion


            // Common

            #region BinaryDataStreamsExtensions

            #region Files

            #region OnDeleteFile

            OCPP.IN.OnDeleteFile += (timestamp,
                                     sender,
                                     connection,
                                     request,
                                     ct) => {

                return Task.FromResult(
                           new DeleteFileResponse(
                               Request:       request,
                               NetworkPath:   NetworkPath.From(Id),
                               FileName:      request.FileName,
                               Status:        DeleteFileStatus.Success
                           )
                       );

            };


            OCPP.FORWARD.OnDeleteFileRequestFilter += (timestamp,
                                                       sender,
                                                       connection,
                                                       request,
                                                       cancellationToken) =>

                Task.FromResult(
                    RequestForwardingDecision<DeleteFileRequest, DeleteFileResponse>.FORWARD(request)
                );

            #endregion

            #region OnGetFile

            OCPP.IN.OnGetFile += (timestamp,
                                  sender,
                                  connection,
                                  request,
                                  ct) => {

                var fileContent = "Hello world!".ToUTF8Bytes();

                return Task.FromResult(
                           new GetFileResponse(
                               Request:           request,
                               NetworkPath:       NetworkPath.From(Id),
                               FileName:          request.FileName,
                               Status:            GetFileStatus.Success,
                               FileContent:       fileContent,
                               FileContentType:   ContentType.Text.Plain,
                               FileSHA256:        SHA256.HashData(fileContent),
                               FileSHA512:        SHA512.HashData(fileContent)
                           )
                       );

            };


            OCPP.FORWARD.OnGetFileRequestFilter += (timestamp,
                                                    sender,
                                                    connection,
                                                    request,
                                                    cancellationToken) =>

                Task.FromResult(
                    RequestForwardingDecision<GetFileRequest, GetFileResponse>.FORWARD(request)
                );

            #endregion

            #region OnListDirectory

            OCPP.IN.OnListDirectory += (timestamp,
                                        sender,
                                        connection,
                                        request,
                                        ct) => {

                return Task.FromResult(
                           new ListDirectoryResponse(
                               Request:            request,
                               NetworkPath:        NetworkPath.From(Id),
                               DirectoryPath:      request.DirectoryPath,
                               Status:             ListDirectoryStatus.Success,
                               DirectoryListing:   new DirectoryListing()
                           )
                       );

            };


            OCPP.FORWARD.OnListDirectoryRequestFilter += (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          cancellationToken) =>

                Task.FromResult(
                    RequestForwardingDecision<ListDirectoryRequest, ListDirectoryResponse>.FORWARD(request)
                );

            #endregion

            #region OnSendFile

            OCPP.IN.OnSendFile += (timestamp,
                                   sender,
                                   connection,
                                   request,
                                   ct) => {

                return Task.FromResult(
                           new SendFileResponse(
                               Request:       request,
                               NetworkPath:   NetworkPath.From(Id),
                               FileName:      request.FileName,
                               Status:        SendFileStatus.Success
                           )
                       );

            };


            OCPP.FORWARD.OnSendFileRequestFilter += (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     cancellationToken) =>

                Task.FromResult(
                    RequestForwardingDecision<SendFileRequest, SendFileResponse>.FORWARD(request)
                );

            #endregion

            #endregion

            #region OnBinaryDataTransfer

            OCPP.IN.OnBinaryDataTransfer += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             ct) => {

                DebugX.Log($"Local Controller '{Id}': Incoming BinaryDataTransfer request: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data?.ToHexString() ?? "-"}!");

                // VendorId
                // MessageId
                // Data

                var responseBinaryData = request.Data;

                if (request.Data is not null)
                    responseBinaryData = request.Data.Reverse();

                return Task.FromResult(
                           request.VendorId == Vendor_Id.GraphDefined

                               ? new BinaryDataTransferResponse(
                                       Request:                request,
                                       NetworkPath:            NetworkPath.From(Id),
                                       Status:                 BinaryDataTransferStatus.Accepted,
                                       AdditionalStatusInfo:   null,
                                       Data:                   responseBinaryData
                                   )

                               : new BinaryDataTransferResponse(
                                       Request:                request,
                                       NetworkPath:            NetworkPath.From(Id),
                                       Status:                 BinaryDataTransferStatus.Rejected,
                                       AdditionalStatusInfo:   null,
                                       Data:                   responseBinaryData
                                   )
                       );

            };


            OCPP.FORWARD.OnBinaryDataTransferRequestFilter += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) =>

                Task.FromResult(
                    RequestForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>.FORWARD(request)
                );

            #endregion

            #endregion

            #region E2ESecurityExtensions

            #region OnSecureDataTransfer

            OCPP.IN.OnSecureDataTransfer += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             ct) => {

                DebugX.Log($"Local Controller '{Id}': Incoming SecureDataTransfer request!");

                // VendorId
                // MessageId
                // Data

                // Null for a method this node does not know, or an AES-GCM tag that does not verify.
                var plaintext           = request.TryDecrypt(GetDecryptionKey(request.NetworkPath.Source, request.KeyId));
                var secureData          = plaintext?.ToUTF8String();
                var responseSecureData  = secureData?.Reverse();
                var keyId               = (UInt16) 1;

                return Task.FromResult(
                           plaintext is not null

                               ? SecureDataTransferResponse.Encrypt(
                                     Request:                request,
                                     Status:                 SecureDataTransferStatus.Accepted,
                                     Destination:            SourceRouting.To(request.NetworkPath.Source),
                                     Parameter:              request.Parameter,
                                     KeyId:                  keyId,
                                     Key:                    GetEncryptionKey    (request.NetworkPath.Source, keyId),
                                     Nonce:                  GetEncryptionNonce  (request.NetworkPath.Source, keyId),
                                     Counter:                GetEncryptionCounter(request.NetworkPath.Source, keyId),
                                     Payload:                responseSecureData?.ToUTF8Bytes() ?? [],
                                     AdditionalStatusInfo:   null,

                                     SignKeys:               null,
                                     SignInfos:              null,
                                     Signatures:             null,

                                     EventTrackingId:        request.EventTrackingId,
                                     NetworkPath:            NetworkPath.From(Id)

                                 )

                               : new SecureDataTransferResponse(
                                     Request:                request,
                                     NetworkPath:            NetworkPath.From(Id),
                                     Status:                 SecureDataTransferStatus.Rejected,
                                     AdditionalStatusInfo:   null,
                                     Ciphertext:             responseSecureData?.ToUTF8Bytes()
                                 )
                       );

            };


            OCPP.FORWARD.OnSecureDataTransferRequestFilter += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) =>

                Task.FromResult(
                    RequestForwardingDecision<SecureDataTransferRequest, SecureDataTransferResponse>.FORWARD(request)
                );

            #endregion

            #endregion

            #region OnDataTransfer

            OCPP.IN.OnDataTransfer += (timestamp,
                                       sender,
                                       connection,
                                       request,
                                       ct) => {

                DebugX.Log($"Local Controller '{Id}': Incoming DataTransfer: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data?.ToString() ?? "-"}!");

                var responseData = request.Data;

                if (request.Data is not null)
                {

                    if      (request.Data.Type == JTokenType.String)
                        responseData = request.Data.ToString().Reverse();

                    else if (request.Data.Type == JTokenType.Object) {

                        var responseObject = new JObject();

                        foreach (var property in (request.Data as JObject)!)
                        {
                            if (property.Value?.Type == JTokenType.String)
                                responseObject.Add(property.Key,
                                                    property.Value.ToString().Reverse());
                        }

                        responseData = responseObject;

                    }

                    else if (request.Data.Type == JTokenType.Array) {

                        var responseArray = new JArray();

                        foreach (var element in (request.Data as JArray)!)
                        {
                            if (element?.Type == JTokenType.String)
                                responseArray.Add(element.ToString().Reverse());
                        }

                        responseData = responseArray;

                    }

                }


                var response =  request.VendorId == Vendor_Id.GraphDefined

                                    ? new DataTransferResponse(
                                          Request:       request,
                                          NetworkPath:   NetworkPath.From(Id),
                                          Status:        DataTransferStatus.Accepted,
                                          Data:          responseData,
                                          StatusInfo:    null,
                                          CustomData:    null
                                      )

                                    : new DataTransferResponse(
                                          Request:       request,
                                          NetworkPath:   NetworkPath.From(Id),
                                          Status:        DataTransferStatus.Rejected,
                                          Data:          null,
                                          StatusInfo:    null,
                                          CustomData:    null
                                      );


                return Task.FromResult(response);

            };


            OCPP.FORWARD.OnDataTransferRequestFilter += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         ct) => {

                if (request.Data?.ToString() == "Please REJECT!")
                {

                    var response = new DataTransferResponse(
                                       request,
                                       DataTransferStatus.Rejected,
                                       Result: Result.Filtered("This message is not allowed!")
                                   );

                    // The response is sent to whoever asked, as this node's answer: signed
                    // by this node, as its signature policy says, like any answer of its own.
                    OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            true,
                            OCPP.CustomDataTransferResponseSerializer,
                            OCPP.CustomStatusInfoSerializer,
                            OCPP.CustomSignatureSerializer,
                            OCPP.CustomCustomDataSerializer
                        ),
                        out _
                    );

                    return Task.FromResult(
                               new RequestForwardingDecision<DataTransferRequest, DataTransferResponse>(
                                   request,
                                   ForwardingDecisions.REJECT,
                                   response,
                                   response.ToJSON(
                                       false,
                                       OCPP.CustomDataTransferResponseSerializer,
                                       OCPP.CustomStatusInfoSerializer,
                                       OCPP.CustomSignatureSerializer,
                                       OCPP.CustomCustomDataSerializer
                                   ),
                                   "The message was REJECTED!"
                               )
                           );

                }

                else
                    return Task.FromResult(
                               RequestForwardingDecision<DataTransferRequest, DataTransferResponse>.FORWARD(request)
                           );

            };

            #endregion

            #region OnMessageTransfer

            OCPP.IN.OnMessageTransferMessageReceived += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         ct) => {

                DebugX.Log($"Local Controller '{Id}': Incoming MessageTransfer: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data?.ToString() ?? "-"}!");

                var responseData = request.Data;

                if (request.Data is not null)
                {

                    if      (request.Data.Type == JTokenType.String)
                        responseData = request.Data.ToString().Reverse();

                    else if (request.Data.Type == JTokenType.Object) {

                        var responseObject = new JObject();

                        foreach (var property in (request.Data as JObject)!)
                        {
                            if (property.Value?.Type == JTokenType.String)
                                responseObject.Add(property.Key,
                                                    property.Value.ToString().Reverse());
                        }

                        responseData = responseObject;

                    }

                    else if (request.Data.Type == JTokenType.Array) {

                        var responseArray = new JArray();

                        foreach (var element in (request.Data as JArray)!)
                        {
                            if (element?.Type == JTokenType.String)
                                responseArray.Add(element.ToString().Reverse());
                        }

                        responseData = responseArray;

                    }

                }

                return Task.CompletedTask;

            };


            OCPP.FORWARD.OnMessageTransferMessageFilter += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            ct) => {

                if (request.Data?.ToString() == "Please REJECT!")
                {

                    //var response = new DataTransferResponse(
                    //                   request,
                    //                   DataTransferStatus.Rejected,
                    //                   Result: Result.Filtered("This message is not allowed!")
                    //               );

                    return Task.FromResult(
                               new MessageForwardingDecision<MessageTransferMessage>(
                                   request,
                                   ForwardingDecisions.REJECT,
                                   //response,
                                   //response.ToJSON(
                                   //    OCPP.CustomDataTransferResponseSerializer,
                                   //    OCPP.CustomStatusInfoSerializer,
                                   //    OCPP.CustomSignatureSerializer,
                                   //    OCPP.CustomCustomDataSerializer
                                   //),
                                   RejectMessage: "The message was REJECTED!"
                               )
                           );

                }

                else
                    return Task.FromResult(
                               MessageForwardingDecision<MessageTransferMessage>.FORWARD(request)
                           );

            };

            #endregion



            // CSMS -> CS

            #region OnReset

            OCPP.IN.OnReset += async (timestamp,
                                      sender,
                                      connection,
                                      request,
                                      ct) => {

                CS.ResetResponse? response = null;

                DebugX.Log($"Local Controller '{Id}': Incoming '{request.ResetType}' reset request!");

                // ResetType

                // EVSE Id is ignored!
                response  = new CS.ResetResponse(
                                Request:      request,
                                Status:       ResetStatus.Accepted,
                                StatusInfo:   null,
                                CustomData:   null
                            );

                return response;

            };


            OCPP.FORWARD.OnResetRequestFilter += (timestamp,
                                                  sender,
                                                  connection,
                                                  request,
                                                  cancellationToken) =>

                Task.FromResult(
                    RequestForwardingDecision<ResetRequest, CS.ResetResponse>.FORWARD(request)
                );

            #endregion

            #region OnRequestStartTransaction

            OCPP.FORWARD.OnRequestStartTransactionRequestFilter += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    cancellationToken) =>

                Task.FromResult(
                    RequestForwardingDecision<RequestStartTransactionRequest, CS.RequestStartTransactionResponse>.FORWARD(
                        request
                    )
                );

            #endregion

            #region OnRequestStopTransaction

            OCPP.FORWARD.OnRequestStopTransactionRequestFilter += (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   cancellationToken) =>

                Task.FromResult(
                    RequestForwardingDecision<RequestStopTransactionRequest, CS.RequestStopTransactionResponse>.FORWARD(
                        request
                    )
                );

            #endregion


            #region DeviceModel

            // Two paths that never meet. OCPP.IN sees only the requests addressed to this
            // local controller itself - the adapter compares the destination with this node's
            // id (or an anycast id it accepts) before any handler runs - and answers them from
            // this node's own Device Model (ALocalControllerNode.DeviceModel.cs). OCPP.FORWARD
            // sees only the requests for others, and hands them on unchanged to the station
            // they name, which answers them from its own Device Model under its own rules.

            #region Variables

            #region OnGetBaseReport

            OCPP.IN.OnGetBaseReport += (timestamp,
                                        sender,
                                        connection,
                                        request,
                                        ct) => {

                var (status, reportData) = DeviceModel.BaseReport(
                                               AllComponentConfigs,
                                               request.ReportBase
                                           );

                // The report follows the response, see OnGetBaseReportResponseSent.
                if (status == GenericDeviceModelStatus.Accepted)
                    pendingReports[request] = reportData;

                return Task.FromResult(
                           new CS.GetBaseReportResponse(
                               Request:      request,
                               Status:       status,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            OCPP.OUT.OnGetBaseReportResponseSent += (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     response,
                                                     runtime,
                                                     sentMessageResult,
                                                     ct) => {

                SendPendingReport(
                    request,
                    (Int32) request.GetBaseReportRequestId,
                    SourceRouting.To(request.NetworkPath.Source),
                    sentMessageResult
                );

                return Task.CompletedTask;

            };


            OCPP.FORWARD.OnGetBaseReportRequestFilter +=
                (timestamp, sender, connection, request, ct) =>

                Task.FromResult(
                    RequestForwardingDecision<GetBaseReportRequest, CS.GetBaseReportResponse>.FORWARD(
                        request
                    )
                );

            #endregion

            #region OnGetReport

            OCPP.IN.OnGetReport += (timestamp,
                                    sender,
                                    connection,
                                    request,
                                    ct) => {

                var (status, reportData) = DeviceModel.Report(
                                               AllComponentConfigs,
                                               request.ComponentCriteria,
                                               request.ComponentVariables
                                           );

                // The report follows the response, see OnGetReportResponseSent.
                if (status == GenericDeviceModelStatus.Accepted)
                    pendingReports[request] = reportData;

                return Task.FromResult(
                           new CS.GetReportResponse(
                               Request:      request,
                               Status:       status,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            OCPP.OUT.OnGetReportResponseSent += (timestamp,
                                                 sender,
                                                 connection,
                                                 request,
                                                 response,
                                                 runtime,
                                                 sentMessageResult,
                                                 ct) => {

                SendPendingReport(
                    request,
                    request.GetReportRequestId,
                    SourceRouting.To(request.NetworkPath.Source),
                    sentMessageResult
                );

                return Task.CompletedTask;

            };


            OCPP.FORWARD.OnGetReportRequestFilter +=
                (timestamp, sender, connection, request, ct) =>

                Task.FromResult(
                    RequestForwardingDecision<GetReportRequest, CS.GetReportResponse>.FORWARD(
                        request
                    )
                );

            #endregion

            #region OnGetVariables

            OCPP.IN.OnGetVariables += (timestamp,
                                       sender,
                                       connection,
                                       request,
                                       ct) =>

                Task.FromResult(
                    new CS.GetVariablesResponse(
                        Request:     request,
                        Results:     DeviceModel.GetVariables(
                                         request.VariableData,
                                         (name, evse) => GetComponentConfigs(name, evse)
                                     ),
                        CustomData:  null
                    )
                );


            OCPP.FORWARD.OnGetVariablesRequestFilter +=
                (timestamp, sender, connection, request, ct) =>

                Task.FromResult(
                    RequestForwardingDecision<GetVariablesRequest, CS.GetVariablesResponse>.FORWARD(
                        request
                    )
                );

            #endregion

            #region OnNotifyReport

            OCPP.FORWARD.OnNotifyReportRequestFilter +=
                (timestamp, sender, connection, request, ct) =>

                Task.FromResult(
                    RequestForwardingDecision<CS.NotifyReportRequest, NotifyReportResponse>.FORWARD(
                        request
                    )
                );

            #endregion

            #region OnSetVariables

            // Each entry reaches the source of its component together with the whole request -
            // its sender by the network path, and its signatures - so that a source can check
            // the right a variable needs: EVQICtrlr[TLSServer].ActiveKeyPin is meant to need
            // "certificates:edit" ("Signed OCPP Messages and Signer Roles", Q12).
            OCPP.IN.OnSetVariables += async (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             ct) =>

                new CS.SetVariablesResponse(
                    Request:             request,
                    SetVariableResults:  await DeviceModel.SetVariablesAsync(
                                                   request,
                                                   (name, evse) => GetComponentConfigs(name, evse),
                                                   ct,
                                                   NewInstanceOf
                                               ),
                    CustomData:          null
                );


            OCPP.FORWARD.OnSetVariablesRequestFilter +=
                (timestamp, sender, connection, request, ct) =>

                Task.FromResult(
                    RequestForwardingDecision<SetVariablesRequest, CS.SetVariablesResponse>.FORWARD(
                        request
                    )
                );

            #endregion

            #endregion

            #endregion


            #region Charging

            #region Tariffs

            #region OnChangeTransactionTariff

            OCPP.FORWARD.OnChangeTransactionTariffRequestFilter +=
                (timestamp, sender, connection, request, ct) =>

                Task.FromResult(
                    RequestForwardingDecision<ChangeTransactionTariffRequest, CS.ChangeTransactionTariffResponse>.FORWARD(
                        request
                    )
                );

            #endregion

            #region OnClearTariffs

            OCPP.FORWARD.OnClearTariffsRequestFilter +=
                (timestamp, sender, connection, request, ct) =>

                Task.FromResult(
                    RequestForwardingDecision<ClearTariffsRequest, CS.ClearTariffsResponse>.FORWARD(
                        request
                    )
                );

            #endregion

            #region OnGetTariffs

            OCPP.FORWARD.OnGetTariffsRequestFilter +=
                (timestamp, sender, connection, request, ct) =>

                Task.FromResult(
                    RequestForwardingDecision<GetTariffsRequest, CS.GetTariffsResponse>.FORWARD(
                        request
                    )
                );

            #endregion

            #region OnSetDefaultTariff

            OCPP.FORWARD.OnSetDefaultTariffRequestFilter +=
                (timestamp, sender, connection, request, ct) =>

                Task.FromResult(
                    RequestForwardingDecision<SetDefaultTariffRequest, CS.SetDefaultTariffResponse>.FORWARD(
                        request
                    )
                );

            #endregion

            #endregion

            #endregion


            // CS -> CSMS

            #region OnBootNotification

            OCPP.FORWARD.OnBootNotificationRequestFilter += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             cancellationToken) =>

                Task.FromResult(
                    RequestForwardingDecision<CS.BootNotificationRequest, BootNotificationResponse>.FORWARD(request)
                );

            //Task.FromResult(
            //    ForwardingDecision<CS.BootNotificationRequest, BootNotificationResponse>.REPLACE(
            //        request,
            //        new CS.BootNotificationRequest(
            //            request.DestinationId,
            //            request.ChargingStation,
            //            request.Reason,
            //            request.SignKeys,
            //            request.SignInfos,
            //            request.Signatures,
            //            request.CustomData,
            //            request.RequestId,
            //            request.RequestTimestamp,
            //            request.RequestTimeout,
            //            request.EventTrackingId,
            //            request.NetworkPath,
            //            request.CancellationToken
            //        ),
            //        nameof(CS.BootNotificationRequest)[..^7],
            //        NetworkingNode_Id.Parse("/dev/null")
            //    )
            //);

            #endregion

            #region OnAuthorize

            OCPP.FORWARD.OnAuthorizeRequestFilter += (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      cancellationToken) =>

                AuthorizeRequestFilter is OnAuthorizeRequestFilterDelegate filter

                    ? filter(timestamp, sender, connection, request, cancellationToken)

                    : Task.FromResult(
                          AllowedChargingStations.Contains(request.NetworkPath.Source)
                              ? RequestForwardingDecision<CS.AuthorizeRequest, AuthorizeResponse>.FORWARD(request)
                              : RequestForwardingDecision<CS.AuthorizeRequest, AuthorizeResponse>.REJECT (request, "Unauthorized!")
                      );

            #endregion

            #region OnTransactionEvent

            OCPP.FORWARD.OnTransactionEventRequestFilter += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             cancellationToken) =>

                Task.FromResult(
                    RequestForwardingDecision<CS.TransactionEventRequest, TransactionEventResponse>.FORWARD(request)
                );

            #endregion

            #region OnMeterValues

            OCPP.FORWARD.OnMeterValuesRequestFilter += (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        cancellationToken) =>

                Task.FromResult(
                    RequestForwardingDecision<CS.MeterValuesRequest, MeterValuesResponse>.FORWARD(request)
                );

            #endregion


        }

        #endregion


    }

}
