﻿/*
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

using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.TimeServer
{

    /// <summary>
    /// A time server node for testing.
    /// </summary>
    public partial class TestTimeServerNode : ATimeServerNode
    {

        #region Properties


        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new time server node for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this time server node.</param>
        public TestTimeServerNode(NetworkingNode_Id  Id,
                               String             VendorName,
                               String             Model,
                               String?            SerialNumber                   = null,
                               String?            SoftwareVersion                = null,
                               I18NString?        Description                    = null,
                               CustomData?        CustomData                     = null,

                               SignaturePolicy?   SignaturePolicy                = null,
                               SignaturePolicy?   ForwardingSignaturePolicy      = null,

                               Boolean            HTTPAPI_Disabled               = false,
                               IPPort?            HTTPAPI_Port                   = null,
                               String?            HTTPAPI_ServerName             = null,
                               String?            HTTPAPI_ServiceName            = null,
                               EMailAddress?      HTTPAPI_RobotEMailAddress      = null,
                               String?            HTTPAPI_RobotGPGPassphrase     = null,
                               Boolean            HTTPAPI_EventLoggingDisabled   = false,

                               WebAPI?            WebAPI                         = null,
                               Boolean            WebAPI_Disabled                = false,
                               HTTPPath?          WebAPI_Path                    = null,

                               WebSocketServer?   ControlWebSocketServer         = null,

                               Boolean            DisableSendHeartbeats          = false,
                               TimeSpan?          SendHeartbeatsEvery            = null,
                               TimeSpan?          DefaultRequestTimeout          = null,

                               Boolean            DisableMaintenanceTasks        = false,
                               TimeSpan?          MaintenanceEvery               = null,
                               DNSClient?         DNSClient                      = null)

            : base(Id,
                   VendorName,
                   Model,
                   SerialNumber,
                   SoftwareVersion,
                   Description,
                   CustomData,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   HTTPAPI_Disabled,
                   HTTPAPI_Port,
                   HTTPAPI_ServerName,
                   HTTPAPI_ServiceName,
                   HTTPAPI_RobotEMailAddress,
                   HTTPAPI_RobotGPGPassphrase,
                   HTTPAPI_EventLoggingDisabled,

                   WebAPI,
                   WebAPI_Disabled,
                   WebAPI_Path,

                   ControlWebSocketServer,

                   DisableSendHeartbeats,
                   SendHeartbeatsEvery,
                   DefaultRequestTimeout,

                   DisableMaintenanceTasks,
                   MaintenanceEvery,
                   DNSClient)

        {

            //Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "HTTPSSEs"));



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
                                                       ct) =>

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
                                                    ct) =>

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
                                                          ct) =>

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
                                                     ct) =>

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

                DebugX.Log($"TimeServer '{Id}': Incoming BinaryDataTransfer request: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data?.ToHexString() ?? "-"}!");

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
                                                               ct) =>

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

                DebugX.Log($"TimeServer '{Id}': Incoming SecureDataTransfer request!");

                // VendorId
                // MessageId
                // Data

                var secureData          = request.Decrypt(GetDecryptionKey(request.NetworkPath.Source, request.KeyId)).ToUTF8String();
                var responseSecureData  = secureData?.Reverse();
                var keyId               = (UInt16) 1;

                return Task.FromResult(
                           request.Ciphertext is not null

                               ? SecureDataTransferResponse.Encrypt(
                                     Request:                request,
                                     Status:                 SecureDataTransferStatus.Accepted,
                                     Destination:            SourceRouting.To(request.NetworkPath.Source),
                                     Parameter:              0,
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
                                                               ct) =>

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

                DebugX.Log($"TimeServer '{Id}': Incoming DataTransfer: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data?.ToString() ?? "-"}!");

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

                DebugX.Log($"TimeServer '{Id}': Incoming MessageTransfer: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data?.ToString() ?? "-"}!");

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

                DebugX.Log($"TimeServer '{Id}': Incoming '{request.ResetType}' reset request!");

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
                                                  ct) =>

                Task.FromResult(
                    RequestForwardingDecision<ResetRequest, CS.ResetResponse>.FORWARD(
                        request
                    )
                );

            #endregion

            #region OnRequestStartTransaction

            OCPP.FORWARD.OnRequestStartTransactionRequestFilter += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    ct) =>

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
                                                                   ct) =>

                Task.FromResult(
                    RequestForwardingDecision<RequestStopTransactionRequest, CS.RequestStopTransactionResponse>.FORWARD(
                        request
                    )
                );

            #endregion


            #region DeviceModel

            #region Variables

            #region OnGetBaseReport

            OCPP.FORWARD.OnGetBaseReportRequestFilter +=
                (timestamp, sender, connection, request, ct) =>

                Task.FromResult(
                    RequestForwardingDecision<GetBaseReportRequest, CS.GetBaseReportResponse>.FORWARD(
                        request
                    )
                );

            #endregion

            #region OnGetReport

            OCPP.FORWARD.OnGetReportRequestFilter +=
                (timestamp, sender, connection, request, ct) =>

                Task.FromResult(
                    RequestForwardingDecision<GetReportRequest, CS.GetReportResponse>.FORWARD(
                        request
                    )
                );

            #endregion

            #region OnGetVariables

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

            #region OnSetDefaultTariff

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

            #region OnAuthorize

            OCPP.FORWARD.OnAuthorizeRequestFilter += (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      ct) =>

                Task.FromResult(
                    RequestForwardingDecision<CS.AuthorizeRequest, AuthorizeResponse>.FORWARD(request)
                );

            #endregion

            #region OnTransactionEvent

            OCPP.FORWARD.OnTransactionEventRequestFilter += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             ct) =>

                Task.FromResult(
                    RequestForwardingDecision<CS.TransactionEventRequest, TransactionEventResponse>.FORWARD(request)
                );

            #endregion

            #region OnMeterValues

            OCPP.FORWARD.OnMeterValuesRequestFilter += (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        ct) =>

                Task.FromResult(
                    RequestForwardingDecision<CS.MeterValuesRequest, MeterValuesResponse>.FORWARD(request)
                );

            #endregion


        }

        #endregion


    }

}
