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

#region Usings

using System.Reflection;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.NN;
using cloud.charging.open.protocols.OCPPv2_1.NN;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A networking node for testing.
    /// </summary>
    public partial class TestNetworkingNode : INetworkingNode,
                                              IEventSender
    {

        public class ActingAsCS : IEventSender
        {

            #region Data

            private readonly            TestNetworkingNode          parentNetworkingNode;

            /// <summary>
            /// The default time span between heartbeat requests.
            /// </summary>
            public readonly             TimeSpan                    DefaultSendHeartbeatEvery   = TimeSpan.FromSeconds(30);

            protected static readonly   TimeSpan                    SemaphoreSlimTimeout        = TimeSpan.FromSeconds(5);

            /// <summary>
            /// The default maintenance interval.
            /// </summary>
            public readonly             TimeSpan                    DefaultMaintenanceEvery     = TimeSpan.FromSeconds(1);
            private static readonly     SemaphoreSlim               MaintenanceSemaphore        = new (1, 1);
            private readonly            Timer                       MaintenanceTimer;

            private readonly            Timer                       SendHeartbeatTimer;


            private readonly            List<EnqueuedRequest>       EnqueuedRequests;

            public                      IHTTPAuthentication?        HTTPAuthentication          { get; }

            #endregion

            #region Properties

            /// <summary>
            /// The client connected to a CSMS.
            /// </summary>
            public INetworkingNodeOutgoingMessages?   CSClient                    { get; private set; }


            public String? ClientCloseMessage
                => "-";


            /// <summary>
            /// The sender identification.
            /// </summary>
            String IEventSender.Id
                => parentNetworkingNode.Id.ToString();


            /// <summary>
            /// The time span between heartbeat requests.
            /// </summary>
            public TimeSpan                 SendHeartbeatEvery          { get; set; }

            /// <summary>
            /// The time at the CSMS.
            /// </summary>
            public DateTime?                CSMSTime                    { get; private set; }

            /// <summary>
            /// The default request timeout for all requests.
            /// </summary>
            public TimeSpan                 DefaultRequestTimeout       { get; }

            /// <summary>
            /// The maintenance interval.
            /// </summary>
            public TimeSpan                 MaintenanceEvery            { get; }

            /// <summary>
            /// Disable all maintenance tasks.
            /// </summary>
            public Boolean                  DisableMaintenanceTasks     { get; set; }

            /// <summary>
            /// Disable all heartbeats.
            /// </summary>
            public Boolean                  DisableSendHeartbeats       { get; set; }


            #region ToDo's

            public URL RemoteURL => throw new NotImplementedException();

            public HTTPHostname? VirtualHostname => throw new NotImplementedException();

   //         string? IHTTPClient.Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public RemoteCertificateValidationHandler? RemoteCertificateValidator => throw new NotImplementedException();

            public X509Certificate? ClientCert => throw new NotImplementedException();

            public SslProtocols TLSProtocol => throw new NotImplementedException();

            public bool PreferIPv4 => throw new NotImplementedException();

            public string HTTPUserAgent => throw new NotImplementedException();

            public TimeSpan RequestTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public TransmissionRetryDelayDelegate TransmissionRetryDelay => throw new NotImplementedException();

            public ushort MaxNumberOfRetries => throw new NotImplementedException();

            public bool UseHTTPPipelining => throw new NotImplementedException();

            public HTTPClientLogger? HTTPLogger => throw new NotImplementedException();

            #endregion

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new charging station for testing.
            /// </summary>
            /// <param name="Id">The charging station identification.</param>
            /// 
            /// <param name="SendHeartbeatEvery">The time span between heartbeat requests.</param>
            /// 
            /// <param name="DefaultRequestTimeout">The default request timeout for all requests.</param>
            public ActingAsCS(TestNetworkingNode                 NetworkingNode,

                              Boolean                            DisableSendHeartbeats     = false,
                              TimeSpan?                          SendHeartbeatEvery        = null,

                              Boolean                            DisableMaintenanceTasks   = false,
                              TimeSpan?                          MaintenanceEvery          = null,

                              TimeSpan?                          DefaultRequestTimeout     = null,
                              IHTTPAuthentication?               HTTPAuthentication        = null,
                              DNSClient?                         DNSClient                 = null,

                              SignaturePolicy?                   SignaturePolicy           = null)

            {

                this.parentNetworkingNode     = NetworkingNode;

                //this.Configuration = new Dictionary<String, ConfigurationData> {
                //    { "hello",          new ConfigurationData("world",    AccessRights.ReadOnly,  false) },
                //    { "changeMe",       new ConfigurationData("now",      AccessRights.ReadWrite, false) },
                //    { "doNotChangeMe",  new ConfigurationData("never",    AccessRights.ReadOnly,  false) },
                //    { "password",       new ConfigurationData("12345678", AccessRights.WriteOnly, false) }
                //};
                this.EnqueuedRequests         = [];

                this.DefaultRequestTimeout    = DefaultRequestTimeout ?? TimeSpan.FromMinutes(1);

                this.DisableSendHeartbeats    = DisableSendHeartbeats;
                this.SendHeartbeatEvery       = SendHeartbeatEvery    ?? DefaultSendHeartbeatEvery;
                this.SendHeartbeatTimer       = new Timer(
                                                    DoSendHeartbeatSync,
                                                    null,
                                                    this.SendHeartbeatEvery,
                                                    this.SendHeartbeatEvery
                                                );

                this.DisableMaintenanceTasks  = DisableMaintenanceTasks;
                this.MaintenanceEvery         = MaintenanceEvery      ?? DefaultMaintenanceEvery;
                //this.MaintenanceTimer         = new Timer(
                //                                    DoMaintenanceSync,
                //                                    null,
                //                                    this.MaintenanceEvery,
                //                                    this.MaintenanceEvery
                //                                );

                this.HTTPAuthentication       = HTTPAuthentication;

            }

            #endregion


            #region HandleErrors(Module, Caller, ExceptionOccured)

            private Task HandleErrors(String     Module,
                                      String     Caller,
                                      Exception  ExceptionOccured)
            {

                DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

                return Task.CompletedTask;

            }

            #endregion


            #region ConnectWebSocket(...)

            public async Task<HTTPResponse?> ConnectWebSocket(URL                                  RemoteURL,
                                                              HTTPHostname?                        VirtualHostname              = null,
                                                              String?                              Description                  = null,
                                                              RemoteCertificateValidationHandler?  RemoteCertificateValidator   = null,
                                                              LocalCertificateSelectionHandler?    ClientCertificateSelector    = null,
                                                              X509Certificate?                     ClientCert                   = null,
                                                              SslProtocols?                        TLSProtocol                  = null,
                                                              Boolean?                             PreferIPv4                   = null,
                                                              String?                              HTTPUserAgent                = null,
                                                              IHTTPAuthentication?                 HTTPAuthentication           = null,
                                                              TimeSpan?                            RequestTimeout               = null,
                                                              TransmissionRetryDelayDelegate?      TransmissionRetryDelay       = null,
                                                              UInt16?                              MaxNumberOfRetries           = null,
                                                              UInt32?                              InternalBufferSize           = null,

                                                              IEnumerable<String>?                 SecWebSocketProtocols        = null,
                                                              NetworkingMode?                      NetworkingMode               = null,

                                                              Boolean                              DisableMaintenanceTasks      = false,
                                                              TimeSpan?                            MaintenanceEvery             = null,
                                                              Boolean                              DisableWebSocketPings        = false,
                                                              TimeSpan?                            WebSocketPingEvery           = null,
                                                              TimeSpan?                            SlowNetworkSimulationDelay   = null,

                                                              String?                              LoggingPath                  = null,
                                                              String?                              LoggingContext               = null,
                                                              LogfileCreatorDelegate?              LogfileCreator               = null,
                                                              HTTPClientLogger?                    HTTPLogger                   = null,
                                                              DNSClient?                           DNSClient                    = null)

            {

                var networkingNodeWSClient = new NetworkingNodeWSClient(

                                                 parentNetworkingNode.Id,

                                                 RemoteURL,
                                                 VirtualHostname,
                                                 Description,
                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 ClientCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 HTTPAuthentication ?? this.HTTPAuthentication,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,

                                                 SecWebSocketProtocols ?? new[] { Version.WebSocketSubProtocolId },
                                                 NetworkingMode        ?? OCPP.WebSockets.NetworkingMode.NetworkingExtensions,

                                                 DisableWebSocketPings,
                                                 WebSocketPingEvery,
                                                 SlowNetworkSimulationDelay,

                                                 DisableMaintenanceTasks,
                                                 MaintenanceEvery,

                                                 LoggingPath,
                                                 LoggingContext,
                                                 LogfileCreator,
                                                 HTTPLogger,
                                                 DNSClient ?? parentNetworkingNode.DNSClient
                                             );

                this.CSClient  = networkingNodeWSClient;

                parentNetworkingNode.IN.WireEvents(networkingNodeWSClient);
                WireEvents(networkingNodeWSClient);

                var response = await networkingNodeWSClient.Connect();

                return response;

            }

            #endregion

            #region WireEvents(IncomingMessages)


            private readonly ConcurrentDictionary<DisplayMessage_Id,     MessageInfo>     displayMessages   = new ();
            private readonly ConcurrentDictionary<Reservation_Id,        Reservation_Id>  reservations      = new ();
            private readonly ConcurrentDictionary<Transaction_Id,        Transaction>     transactions      = new ();
            private readonly ConcurrentDictionary<Transaction_Id,        Decimal>         totalCosts        = new ();
            private readonly ConcurrentDictionary<InstallCertificateUse, Certificate>     certificates      = new ();

            public void WireEvents(INetworkingNodeIncomingMessages IncomingMessages)
            {

                #region OnUpdateFirmware

                IncomingMessages.OnUpdateFirmware += async (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            cancellationToken) => {

                    #region Send OnUpdateFirmwareRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUpdateFirmwareRequest(startTime,
                                                                               this,
                                                                               connection,
                                                                               request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.UpdateFirmwareResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.UpdateFirmwareResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid UpdateFirmware request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomUpdateFirmwareRequestSerializer,
                                     parentNetworkingNode.CustomFirmwareSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.UpdateFirmwareResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming UpdateFirmware request ({request.UpdateFirmwareRequestId}) for '" + request.Firmware.FirmwareURL + "'.");

                            // Firmware,
                            // UpdateFirmwareRequestId
                            // Retries
                            // RetryIntervals

                            response = new OCPPv2_1.CS.UpdateFirmwareResponse(
                                           Request:      request,
                                           Status:       UpdateFirmwareStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomUpdateFirmwareResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnUpdateFirmwareResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUpdateFirmwareResponse(responseTime,
                                                                                this,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                responseTime - startTime);


                    #endregion

                    return response;

                };

                #endregion

                #region OnPublishFirmware

                IncomingMessages.OnPublishFirmware += async (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             cancellationToken) => {

                    #region Send OnPublishFirmwareRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnPublishFirmwareRequest(startTime,
                                                                                this,
                                                                                connection,
                                                                                request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.PublishFirmwareResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.PublishFirmwareResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid PublishFirmware request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomPublishFirmwareRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.PublishFirmwareResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming PublishFirmware request ({request.PublishFirmwareRequestId}) for '" + request.DownloadLocation + "'.");

                            // PublishFirmwareRequestId
                            // DownloadLocation
                            // MD5Checksum
                            // Retries
                            // RetryInterval

                            response = new OCPPv2_1.CS.PublishFirmwareResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomPublishFirmwareResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnPublishFirmwareResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnPublishFirmwareResponse(responseTime,
                                                                                 this,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnUnpublishFirmware

                IncomingMessages.OnUnpublishFirmware += async (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) => {

                    #region Send OnUnpublishFirmwareRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUnpublishFirmwareRequest(startTime,
                                                                                  this,
                                                                                  connection,
                                                                                  request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.UnpublishFirmwareResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.UnpublishFirmwareResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid UnpublishFirmware request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomUnpublishFirmwareRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.UnpublishFirmwareResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming UnpublishFirmware request for '" + request.MD5Checksum + "'.");

                            // MD5Checksum

                            response = new OCPPv2_1.CS.UnpublishFirmwareResponse(
                                           Request:      request,
                                           Status:       UnpublishFirmwareStatus.Unpublished,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomUnpublishFirmwareResponseSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnUnpublishFirmwareResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUnpublishFirmwareResponse(responseTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetBaseReport

                IncomingMessages.OnGetBaseReport += async (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           cancellationToken) => {

                    #region Send OnGetBaseReportRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetBaseReportRequest(startTime,
                                                                              this,
                                                                              connection,
                                                                              request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetBaseReportResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetBaseReportResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetBaseReport request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomGetBaseReportRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetBaseReportResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetBaseReport request ({request.GetBaseReportRequestId}) accepted.");

                            // GetBaseReportRequestId
                            // ReportBase

                            response = new OCPPv2_1.CS.GetBaseReportResponse(
                                           Request:      request,
                                           Status:       GenericDeviceModelStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomGetBaseReportResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetBaseReportResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetBaseReportResponse(responseTime,
                                                                               this,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetReport

                IncomingMessages.OnGetReport += async (timestamp,
                                                       sender,
                                                       connection,
                                                       request,
                                                       cancellationToken) => {

                    #region Send OnGetReportRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetReportRequest(startTime,
                                                                          this,
                                                                          connection,
                                                                          request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetReportResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetReportResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetReport request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomGetReportRequestSerializer,
                                     parentNetworkingNode.CustomComponentVariableSerializer,
                                     parentNetworkingNode.CustomComponentSerializer,
                                     parentNetworkingNode.CustomEVSESerializer,
                                     parentNetworkingNode.CustomVariableSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetReportResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetReport request ({request.GetReportRequestId}) accepted.");

                            // GetReportRequestId
                            // ComponentCriteria
                            // ComponentVariables

                            response = new OCPPv2_1.CS.GetReportResponse(
                                           Request:      request,
                                           Status:       GenericDeviceModelStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomGetReportResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetReportResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetReportResponse(responseTime,
                                                                           this,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetLog

                IncomingMessages.OnGetLog += async (timestamp,
                                                    sender,
                                                    connection,
                                                    request,
                                                    cancellationToken) => {

                    #region Send OnGetLogRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetLogRequest(startTime,
                                                                       this,
                                                                       connection,
                                                                       request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetLogResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetLogResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetLog request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomGetLogRequestSerializer,
                                     parentNetworkingNode.CustomLogParametersSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetLogResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetLog request ({request.LogRequestId}) accepted.");

                            // LogType
                            // LogRequestId
                            // Log
                            // Retries
                            // RetryInterval

                            response = new OCPPv2_1.CS.GetLogResponse(
                                           Request:      request,
                                           Status:       LogStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomGetLogResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetLogResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetLogResponse(responseTime,
                                                                        this,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnSetVariables

                IncomingMessages.OnSetVariables += async (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          cancellationToken) => {

                    #region Send OnSetVariablesRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetVariablesRequest(startTime,
                                                                             this,
                                                                             connection,
                                                                             request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SetVariablesResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SetVariablesResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SetVariables request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomSetVariablesRequestSerializer,
                                     parentNetworkingNode.CustomSetVariableDataSerializer,
                                     parentNetworkingNode.CustomComponentSerializer,
                                     parentNetworkingNode.CustomEVSESerializer,
                                     parentNetworkingNode.CustomVariableSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SetVariablesResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SetVariables request accepted.");

                            // VariableData

                            response = new OCPPv2_1.CS.SetVariablesResponse(
                                           Request:              request,
                                           SetVariableResults:   request.VariableData.Select(variableData => new SetVariableResult(
                                                                                                                 Status:                SetVariableStatus.Accepted,
                                                                                                                 Component:             variableData.Component,
                                                                                                                 Variable:              variableData.Variable,
                                                                                                                 AttributeType:         variableData.AttributeType,
                                                                                                                 AttributeStatusInfo:   null,
                                                                                                                 CustomData:            null
                                                                                                             )),
                                           CustomData:           null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomSetVariablesResponseSerializer,
                            parentNetworkingNode.CustomSetVariableResultSerializer,
                            parentNetworkingNode.CustomComponentSerializer,
                            parentNetworkingNode.CustomEVSESerializer,
                            parentNetworkingNode.CustomVariableSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetVariablesResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetVariablesResponse(responseTime,
                                                                              this,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetVariables

                IncomingMessages.OnGetVariables += async (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          cancellationToken) => {

                    #region Send OnGetVariablesRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetVariablesRequest(startTime,
                                                                             this,
                                                                             connection,
                                                                             request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetVariablesResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetVariablesResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetVariables request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomGetVariablesRequestSerializer,
                                     parentNetworkingNode.CustomGetVariableDataSerializer,
                                     parentNetworkingNode.CustomComponentSerializer,
                                     parentNetworkingNode.CustomEVSESerializer,
                                     parentNetworkingNode.CustomVariableSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetVariablesResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetVariables request accepted.");

                            // VariableData

                            response = new OCPPv2_1.CS.GetVariablesResponse(
                                           Request:      request,
                                           Results:      request.VariableData.Select(variableData => new GetVariableResult(
                                                                                                         AttributeStatus:       GetVariableStatus.Accepted,
                                                                                                         Component:             variableData.Component,
                                                                                                         Variable:              variableData.Variable,
                                                                                                         AttributeValue:        "",
                                                                                                         AttributeType:         variableData.AttributeType,
                                                                                                         AttributeStatusInfo:   null,
                                                                                                         CustomData:            null
                                                                                                     )),
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomGetVariablesResponseSerializer,
                            parentNetworkingNode.CustomGetVariableResultSerializer,
                            parentNetworkingNode.CustomComponentSerializer,
                            parentNetworkingNode.CustomEVSESerializer,
                            parentNetworkingNode.CustomVariableSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetVariablesResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetVariablesResponse(responseTime,
                                                                              this,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnSetMonitoringBase

                IncomingMessages.OnSetMonitoringBase += async (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) => {

                    #region Send OnSetMonitoringBaseRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetMonitoringBaseRequest(startTime,
                                                                                  this,
                                                                                  connection,
                                                                                  request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SetMonitoringBaseResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SetMonitoringBaseResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SetMonitoringBase request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomSetMonitoringBaseRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SetMonitoringBaseResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SetMonitoringBase request accepted.");

                            // MonitoringBase

                            response = new OCPPv2_1.CS.SetMonitoringBaseResponse(
                                           Request:      request,
                                           Status:       GenericDeviceModelStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomSetMonitoringBaseResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetMonitoringBaseResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetMonitoringBaseResponse(responseTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetMonitoringReport

                IncomingMessages.OnGetMonitoringReport += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                    #region Send OnGetMonitoringReportRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetMonitoringReportRequest(startTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetMonitoringReportResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetMonitoringReportResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetMonitoringReport request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomGetMonitoringReportRequestSerializer,
                                     parentNetworkingNode.CustomComponentVariableSerializer,
                                     parentNetworkingNode.CustomComponentSerializer,
                                     parentNetworkingNode.CustomEVSESerializer,
                                     parentNetworkingNode.CustomVariableSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetMonitoringReportResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetMonitoringReport request ({request.GetMonitoringReportRequestId}) accepted.");

                            // GetMonitoringReportRequestId
                            // MonitoringCriteria
                            // ComponentVariables

                            response = new OCPPv2_1.CS.GetMonitoringReportResponse(
                                           Request:      request,
                                           Status:       GenericDeviceModelStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomGetMonitoringReportResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetMonitoringReportResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetMonitoringReportResponse(responseTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnSetMonitoringLevel

                IncomingMessages.OnSetMonitoringLevel += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                    #region Send OnSetMonitoringLevelRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetMonitoringLevelRequest(startTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SetMonitoringLevelResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SetMonitoringLevelResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SetMonitoringLevel request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomSetMonitoringLevelRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SetMonitoringLevelResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SetMonitoringLevel request accepted.");

                            // Severity

                            response = new OCPPv2_1.CS.SetMonitoringLevelResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomSetMonitoringLevelResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetMonitoringLevelResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetMonitoringLevelResponse(responseTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request,
                                                                                    response,
                                                                                    responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnSetVariableMonitoring

                IncomingMessages.OnSetVariableMonitoring += async (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   cancellationToken) => {

                    #region Send OnSetVariableMonitoringRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetVariableMonitoringRequest(startTime,
                                                                                      this,
                                                                                      connection,
                                                                                      request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SetVariableMonitoringResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SetVariableMonitoringResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SetVariableMonitoring request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomSetVariableMonitoringRequestSerializer,
                                     parentNetworkingNode.CustomSetMonitoringDataSerializer,
                                     parentNetworkingNode.CustomComponentSerializer,
                                     parentNetworkingNode.CustomEVSESerializer,
                                     parentNetworkingNode.CustomVariableSerializer,
                                     parentNetworkingNode.CustomPeriodicEventStreamParametersSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SetVariableMonitoringResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SetMonitoringLevel request accepted.");

                            // MonitoringData

                            response = new OCPPv2_1.CS.SetVariableMonitoringResponse(
                                           Request:                request,
                                           SetMonitoringResults:   request.MonitoringData.Select(setMonitoringData => new SetMonitoringResult(
                                                                                                                          Status:                 SetMonitoringStatus.Accepted,
                                                                                                                          MonitorType:            setMonitoringData.MonitorType,
                                                                                                                          Severity:               setMonitoringData.Severity,
                                                                                                                          Component:              setMonitoringData.Component,
                                                                                                                          Variable:               setMonitoringData.Variable,
                                                                                                                          VariableMonitoringId:   setMonitoringData.VariableMonitoringId,
                                                                                                                          StatusInfo:             null,
                                                                                                                          CustomData:             null
                                                                                                                      )),
                                           CustomData:             null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomSetVariableMonitoringResponseSerializer,
                            parentNetworkingNode.CustomSetMonitoringResultSerializer,
                            parentNetworkingNode.CustomComponentSerializer,
                            parentNetworkingNode.CustomEVSESerializer,
                            parentNetworkingNode.CustomVariableSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetVariableMonitoringResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetVariableMonitoringResponse(responseTime,
                                                                                       this,
                                                                                       connection,
                                                                                       request,
                                                                                       response,
                                                                                       responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnClearVariableMonitoring

                IncomingMessages.OnClearVariableMonitoring += async (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     cancellationToken) => {

                    #region Send OnClearVariableMonitoringRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnClearVariableMonitoringRequest(startTime,
                                                                                        this,
                                                                                        connection,
                                                                                        request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.ClearVariableMonitoringResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.ClearVariableMonitoringResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid ClearVariableMonitoring request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomClearVariableMonitoringRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.ClearVariableMonitoringResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming ClearVariableMonitoring request (VariableMonitoringIds: {request.VariableMonitoringIds.AggregateWith(", ")})");

                            // VariableMonitoringIds

                            response = new OCPPv2_1.CS.ClearVariableMonitoringResponse(
                                           Request:                  request,
                                           ClearMonitoringResults:   request.VariableMonitoringIds.Select(variableMonitoringId => new ClearMonitoringResult(
                                                                                                                                      Status:       ClearMonitoringStatus.Accepted,
                                                                                                                                      Id:           variableMonitoringId,
                                                                                                                                      StatusInfo:   null,
                                                                                                                                      CustomData:   null
                                                                                                                                  )),
                                           CustomData:               null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomClearVariableMonitoringResponseSerializer,
                            parentNetworkingNode.CustomClearMonitoringResultSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnClearVariableMonitoringResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnClearVariableMonitoringResponse(responseTime,
                                                                                         this,
                                                                                         connection,
                                                                                         request,
                                                                                         response,
                                                                                         responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnSetNetworkProfile

                IncomingMessages.OnSetNetworkProfile += async (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) => {

                    #region Send OnSetNetworkProfileRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetNetworkProfileRequest(startTime,
                                                                                  this,
                                                                                  connection,
                                                                                  request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SetNetworkProfileResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SetNetworkProfileResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SetNetworkProfile request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomSetNetworkProfileRequestSerializer,
                                     parentNetworkingNode.CustomNetworkConnectionProfileSerializer,
                                     parentNetworkingNode.CustomVPNConfigurationSerializer,
                                     parentNetworkingNode.CustomAPNConfigurationSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SetNetworkProfileResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SetNetworkProfile request for configuration slot {request.ConfigurationSlot}!");

                            // ConfigurationSlot
                            // NetworkConnectionProfile

                            response = new OCPPv2_1.CS.SetNetworkProfileResponse(
                                           Request:      request,
                                           Status:       SetNetworkProfileStatus.Accepted,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomSetNetworkProfileResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetNetworkProfileResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetNetworkProfileResponse(responseTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnChangeAvailability

                IncomingMessages.OnChangeAvailability += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                    #region Send OnChangeAvailabilityRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnChangeAvailabilityRequest(startTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.ChangeAvailabilityResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.ChangeAvailabilityResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid ChangeAvailability request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomChangeAvailabilityRequestSerializer,
                                     parentNetworkingNode.CustomEVSESerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.ChangeAvailabilityResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming ChangeAvailability request {request.OperationalStatus.AsText()}{(request.EVSE is not null ? $" for EVSE '{request.EVSE.Id}'{(request.EVSE.ConnectorId.HasValue ? $"/{request.EVSE.ConnectorId}" : "")}" : "")}!");

                            // OperationalStatus
                            // EVSE
 
                            response = request.EVSE is null

                                           ? new OCPPv2_1.CS.ChangeAvailabilityResponse(
                                                 Request:      request,
                                                 Status:       ChangeAvailabilityStatus.Accepted,
                                                 CustomData:   null
                                             )

                                           : new OCPPv2_1.CS.ChangeAvailabilityResponse(
                                                 Request:      request,
                                                 Status:       ChangeAvailabilityStatus.Rejected,
                                                 CustomData:   null
                                             );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomChangeAvailabilityResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnChangeAvailabilityResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnChangeAvailabilityResponse(responseTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request,
                                                                                    response,
                                                                                    responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnTriggerMessage

                IncomingMessages.OnTriggerMessage += async (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            cancellationToken) => {

                    #region Send OnTriggerMessageRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnTriggerMessageRequest(startTime,
                                                                               this,
                                                                               connection,
                                                                               request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.TriggerMessageResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.TriggerMessageResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid TriggerMessage request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomTriggerMessageRequestSerializer,
                                     parentNetworkingNode.CustomEVSESerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.TriggerMessageResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming TriggerMessage request for '{request.RequestedMessage}'{(request.EVSE is not null ? $" at EVSE '{request.EVSE.Id}'" : "")}!");

                            // RequestedMessage
                            // EVSE

                            _ = Task.Run(async () => {

                                if (request.RequestedMessage == MessageTrigger.BootNotification)
                                {
                                    await parentNetworkingNode.SendBootNotification(
                                              BootReason:  BootReason.Triggered,
                                              CustomData:  null
                                          );
                                }


                                    // LogStatusNotification
                                    // DiagnosticsStatusNotification
                                    // FirmwareStatusNotification

                                    // Seems not to be allowed any more!
                                    //case MessageTriggers.Heartbeat:
                                    //    await this.SendHeartbeat(
                                    //              CustomData:   null
                                    //          );
                                    //    break;

                                    // MeterValues
                                    // SignChargingStationCertificate

                                else if (request.RequestedMessage == MessageTrigger.StatusNotification &&
                                         request.EVSE is not null)
                                {
                                    await parentNetworkingNode.SendStatusNotification(
                                              EVSEId:        request.EVSE.Id,
                                              ConnectorId:   Connector_Id.Parse(1),
                                              Timestamp:     Timestamp.Now,
                                              Status:        ConnectorStatus.Unavailable,
                                              CustomData:    null
                                          );
                                }

                            },
                            CancellationToken.None);


                            if (request.RequestedMessage == MessageTrigger.BootNotification ||
                                request.RequestedMessage == MessageTrigger.LogStatusNotification ||
                                request.RequestedMessage == MessageTrigger.DiagnosticsStatusNotification ||
                                request.RequestedMessage == MessageTrigger.FirmwareStatusNotification ||
                              //MessageTriggers.Heartbeat
                                request.RequestedMessage == MessageTrigger.SignChargingStationCertificate)
                            {

                                response = new OCPPv2_1.CS.TriggerMessageResponse(
                                               request,
                                               TriggerMessageStatus.Accepted
                                           );

                            }



                            if (response == null &&
                               (request.RequestedMessage == MessageTrigger.MeterValues ||
                                request.RequestedMessage == MessageTrigger.StatusNotification))
                            {

                                response = request.EVSE is not null

                                               ? new OCPPv2_1.CS.TriggerMessageResponse(
                                                     request,
                                                     TriggerMessageStatus.Accepted
                                                 )

                                               : new OCPPv2_1.CS.TriggerMessageResponse(
                                                     request,
                                                     TriggerMessageStatus.Rejected
                                                 );

                            }

                        }

                    }

                    response ??= new OCPPv2_1.CS.TriggerMessageResponse(
                                     request,
                                     TriggerMessageStatus.Rejected
                                 );

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomTriggerMessageResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnTriggerMessageResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnTriggerMessageResponse(responseTime,
                                                                                this,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnIncomingDataTransfer

                IncomingMessages.OnIncomingDataTransfer += async (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  cancellationToken) => {

                    #region Send OnIncomingDataTransferRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnIncomingDataTransferRequest(startTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request);

                    #endregion


                    #region Check charging station identification

                    DataTransferResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.DataTransferResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid DataTransfer request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomIncomingDataTransferRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new DataTransferResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming data transfer request: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data ?? "-"}!");

                            // VendorId
                            // MessageId
                            // Data

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

                            if (request.VendorId == Vendor_Id.GraphDefined)
                            {
                                response = new DataTransferResponse(
                                               request,
                                               DataTransferStatus.Accepted,
                                               responseData
                                           );
                            }
                            else
                                response = new DataTransferResponse(
                                               request,
                                               DataTransferStatus.Rejected
                                           );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomIncomingDataTransferResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnIncomingDataTransferResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnIncomingDataTransferResponse(responseTime,
                                                                                      this,
                                                                                      connection,
                                                                                      request,
                                                                                      response,
                                                                                      responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion


                #region OnCertificateSigned

                IncomingMessages.OnCertificateSigned += async (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) => {

                    #region Send OnCertificateSignedRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnCertificateSignedRequest(startTime,
                                                                                  this,
                                                                                  connection,
                                                                                  request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.CertificateSignedResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.CertificateSignedResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid CertificateSigned request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomCertificateSignedRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.CertificateSignedResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming CertificateSigned request{(request.CertificateType.HasValue ? $"(certificate type: {request.CertificateType.Value})" : "")}!");

                            // CertificateChain
                            // CertificateType

                            response = new OCPPv2_1.CS.CertificateSignedResponse(
                                           Request:      request,
                                           Status:       request.CertificateChain.FirstOrDefault()?.Parsed is not null
                                                             ? CertificateSignedStatus.Accepted
                                                             : CertificateSignedStatus.Rejected,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomCertificateSignedResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnCertificateSignedResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnCertificateSignedResponse(responseTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnInstallCertificate

                IncomingMessages.OnInstallCertificate += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                    #region Send OnInstallCertificateRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnInstallCertificateRequest(startTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.InstallCertificateResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.InstallCertificateResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid InstallCertificate request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomInstallCertificateRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.InstallCertificateResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming InstallCertificate request (certificate type: {request.CertificateType}!");

                            // CertificateType
                            // Certificate

                            var success = certificates.AddOrUpdate(request.CertificateType,
                                                                       a    => request.Certificate,
                                                                      (b,c) => request.Certificate);

                            response = new OCPPv2_1.CS.InstallCertificateResponse(
                                           Request:      request,
                                           Status:       request.Certificate?.Parsed is not null
                                                             ? CertificateStatus.Accepted
                                                             : CertificateStatus.Rejected,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomInstallCertificateResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnInstallCertificateResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnInstallCertificateResponse(responseTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request,
                                                                                    response,
                                                                                    responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetInstalledCertificateIds

                IncomingMessages.OnGetInstalledCertificateIds += async (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        cancellationToken) => {

                    #region Send OnGetInstalledCertificateIdsRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetInstalledCertificateIdsRequest(startTime,
                                                                                           this,
                                                                                           connection,
                                                                                           request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetInstalledCertificateIdsResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetInstalledCertificateIdsResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetInstalledCertificateIds request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomGetInstalledCertificateIdsRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetInstalledCertificateIdsResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetInstalledCertificateIds request for certificate types: {request.CertificateTypes.Select(certificateType => certificateType).AggregateWith(", ")}!");

                            // CertificateTypes

                            var certs = new List<CertificateHashData>();

                            foreach (var certificateType in request.CertificateTypes)
                            {

                                if (certificates.TryGetValue(InstallCertificateUse.Parse(certificateType.ToString()), out var cert))
                                    certs.Add(new CertificateHashData(
                                                  HashAlgorithm:         HashAlgorithms.SHA256,
                                                  IssuerNameHash:        cert.Parsed?.Issuer               ?? "-",
                                                  IssuerPublicKeyHash:   cert.Parsed?.GetPublicKeyString() ?? "-",
                                                  SerialNumber:          cert.Parsed?.SerialNumber         ?? "-",
                                                  CustomData:            null
                                              ));

                            }

                            response = new OCPPv2_1.CS.GetInstalledCertificateIdsResponse(
                                           Request:                    request,
                                           Status:                     GetInstalledCertificateStatus.Accepted,
                                           CertificateHashDataChain:   certs,
                                           StatusInfo:                 null,
                                           CustomData:                 null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomGetInstalledCertificateIdsResponseSerializer,
                            parentNetworkingNode.CustomCertificateHashDataSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetInstalledCertificateIdsResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetInstalledCertificateIdsResponse(responseTime,
                                                                                            this,
                                                                                            connection,
                                                                                            request,
                                                                                            response,
                                                                                            responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnDeleteCertificate

                IncomingMessages.OnDeleteCertificate += async (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) => {

                    #region Send OnDeleteCertificateRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnDeleteCertificateRequest(startTime,
                                                                                  this,
                                                                                  connection,
                                                                                  request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.DeleteCertificateResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.DeleteCertificateResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid DeleteCertificate request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomDeleteCertificateRequestSerializer,
                                     parentNetworkingNode.CustomCertificateHashDataSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.DeleteCertificateResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming DeleteCertificate request!");

                            // CertificateHashData

                            var certKV  = certificates.FirstOrDefault(certificateKV => request.CertificateHashData.SerialNumber == certificateKV.Value.Parsed?.SerialNumber);

                            var success = certificates.TryRemove(certKV);

                            response = new OCPPv2_1.CS.DeleteCertificateResponse(
                                           Request:      request,
                                           Status:       success
                                                             ? DeleteCertificateStatus.Accepted
                                                             : DeleteCertificateStatus.NotFound,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomDeleteCertificateResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnDeleteCertificateResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnDeleteCertificateResponse(responseTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnNotifyCRL

                IncomingMessages.OnNotifyCRL += async (timestamp,
                                                       sender,
                                                       connection,
                                                       request,
                                                       cancellationToken) => {

                    #region Send OnNotifyCRLRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnNotifyCRLRequest(startTime,
                                                                          this,
                                                                          connection,
                                                                          request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.NotifyCRLResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.NotifyCRLResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid NotifyCRL request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomNotifyCRLRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.NotifyCRLResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming NotifyCRL request!");

                            // NotifyCRLRequestId
                            // Availability
                            // Location

                            response = new OCPPv2_1.CS.NotifyCRLResponse(
                                           Request:      request,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomNotifyCRLResponseSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnNotifyCRLResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnNotifyCRLResponse(responseTime,
                                                                           this,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion


                #region OnGetLocalListVersion

                IncomingMessages.OnGetLocalListVersion += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                    #region Send OnGetLocalListVersionRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetLocalListVersionRequest(startTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetLocalListVersionResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetLocalListVersionResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetLocalListVersion request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomGetLocalListVersionRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetLocalListVersionResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetLocalListVersion request!");

                            // none

                            response = new OCPPv2_1.CS.GetLocalListVersionResponse(
                                           Request:         request,
                                           VersionNumber:   0,
                                           CustomData:      null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomGetLocalListVersionResponseSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetLocalListVersionResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetLocalListVersionResponse(responseTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnSendLocalList

                IncomingMessages.OnSendLocalList += async (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           cancellationToken) => {

                    #region Send OnSendLocalListRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSendLocalListRequest(startTime,
                                                                              this,
                                                                              connection,
                                                                              request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SendLocalListResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SendLocalListResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SendLocalList request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomSendLocalListRequestSerializer,
                                     parentNetworkingNode.CustomAuthorizationDataSerializer,
                                     parentNetworkingNode.CustomIdTokenSerializer,
                                     parentNetworkingNode.CustomAdditionalInfoSerializer,
                                     parentNetworkingNode.CustomIdTokenInfoSerializer,
                                     parentNetworkingNode.CustomMessageContentSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SendLocalListResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SendLocalList request: '{request.UpdateType.AsText()}' version '{request.VersionNumber}'!");

                            // VersionNumber
                            // UpdateType
                            // LocalAuthorizationList

                            response = new OCPPv2_1.CS.SendLocalListResponse(
                                           Request:      request,
                                           Status:       SendLocalListStatus.Accepted,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomSendLocalListResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSendLocalListResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSendLocalListResponse(responseTime,
                                                                               this,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnClearCache

                IncomingMessages.OnClearCache += async (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        cancellationToken) => {

                    #region Send OnClearCacheRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnClearCacheRequest(startTime,
                                                                           this,
                                                                           connection,
                                                                           request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.ClearCacheResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.ClearCacheResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid ClearCache request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomClearCacheRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.ClearCacheResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming ClearCache request!");

                            // none

                            response = new OCPPv2_1.CS.ClearCacheResponse(
                                           Request:      request,
                                           Status:       ClearCacheStatus.Accepted,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomClearCacheResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnClearCacheResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnClearCacheResponse(responseTime,
                                                                            this,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion


                #region OnReserveNow

                IncomingMessages.OnReserveNow += async (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        cancellationToken) => {

                    #region Send OnReserveNowRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnReserveNowRequest(startTime,
                                                                           this,
                                                                           connection,
                                                                           request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.ReserveNowResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.ReserveNowResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid ReserveNow request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomReserveNowRequestSerializer,
                                     parentNetworkingNode.CustomIdTokenSerializer,
                                     parentNetworkingNode.CustomAdditionalInfoSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.ReserveNowResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming ReserveNow request (reservation id: {request.Id}, idToken: '{request.IdToken.Value}'{(request.EVSEId.HasValue ? $", evseId: '{request.EVSEId.Value}'" : "")})!");

                            // ReservationId
                            // ExpiryDate
                            // IdToken
                            // ConnectorType
                            // EVSEId
                            // GroupIdToken

                            var success = reservations.TryAdd(request.Id,
                                                              request.Id);

                            response = new OCPPv2_1.CS.ReserveNowResponse(
                                           Request:      request,
                                           Status:       success
                                                             ? ReservationStatus.Accepted
                                                             : ReservationStatus.Rejected,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomReserveNowResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnReserveNowResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnReserveNowResponse(responseTime,
                                                                            this,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnCancelReservation

                IncomingMessages.OnCancelReservation += async (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) => {

                    #region Send OnCancelReservationRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnCancelReservationRequest(startTime,
                                                                                  this,
                                                                                  connection,
                                                                                  request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.CancelReservationResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.CancelReservationResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid CancelReservation request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomCancelReservationRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.CancelReservationResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            var success = reservations.ContainsKey(request.ReservationId)
                                              ? reservations.TryRemove(request.ReservationId, out _)
                                              : true;

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming CancelReservation request for reservation id '{request.ReservationId}': {(success ? "accepted" : "rejected")}!");

                            // ReservationId

                            response = new OCPPv2_1.CS.CancelReservationResponse(
                                           Request:      request,
                                           Status:       success
                                                             ? CancelReservationStatus.Accepted
                                                             : CancelReservationStatus.Rejected,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomCancelReservationResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnCancelReservationResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnCancelReservationResponse(responseTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnRequestStartTransaction

                IncomingMessages.OnRequestStartTransaction += async (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     cancellationToken) => {

                    #region Send OnRequestStartTransactionRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnRequestStartTransactionRequest(startTime,
                                                                                        this,
                                                                                        connection,
                                                                                        request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.RequestStartTransactionResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.RequestStartTransactionResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid RequestStartTransaction request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(

                                     parentNetworkingNode.CustomRequestStartTransactionRequestSerializer,
                                     parentNetworkingNode.CustomIdTokenSerializer,
                                     parentNetworkingNode.CustomAdditionalInfoSerializer,
                                     parentNetworkingNode.CustomChargingProfileSerializer,
                                     parentNetworkingNode.CustomLimitBeyondSoCSerializer,
                                     parentNetworkingNode.CustomChargingScheduleSerializer,
                                     parentNetworkingNode.CustomChargingSchedulePeriodSerializer,
                                     parentNetworkingNode.CustomV2XFreqWattEntrySerializer,
                                     parentNetworkingNode.CustomV2XSignalWattEntrySerializer,
                                     parentNetworkingNode.CustomSalesTariffSerializer,
                                     parentNetworkingNode.CustomSalesTariffEntrySerializer,
                                     parentNetworkingNode.CustomRelativeTimeIntervalSerializer,
                                     parentNetworkingNode.CustomConsumptionCostSerializer,
                                     parentNetworkingNode.CustomCostSerializer,

                                     parentNetworkingNode.CustomAbsolutePriceScheduleSerializer,
                                     parentNetworkingNode.CustomPriceRuleStackSerializer,
                                     parentNetworkingNode.CustomPriceRuleSerializer,
                                     parentNetworkingNode.CustomTaxRuleSerializer,
                                     parentNetworkingNode.CustomOverstayRuleListSerializer,
                                     parentNetworkingNode.CustomOverstayRuleSerializer,
                                     parentNetworkingNode.CustomAdditionalServiceSerializer,

                                     parentNetworkingNode.CustomPriceLevelScheduleSerializer,
                                     parentNetworkingNode.CustomPriceLevelScheduleEntrySerializer,

                                     parentNetworkingNode.CustomTransactionLimitsSerializer,

                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer

                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.RequestStartTransactionResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming RequestStartTransaction for '{(request.EVSEId?.ToString() ?? "-")}'!");

                            // ToDo: lock(evses)

                            response = new OCPPv2_1.CS.RequestStartTransactionResponse(
                                           request,
                                           RequestStartStopStatus.Rejected
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomRequestStartTransactionResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnRequestStartTransactionResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnRequestStartTransactionResponse(responseTime,
                                                                                         this,
                                                                                         connection,
                                                                                         request,
                                                                                         response,
                                                                                         responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnRequestStopTransaction

                IncomingMessages.OnRequestStopTransaction += async (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    cancellationToken) => {

                    #region Send OnRequestStopTransactionRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnRequestStopTransactionRequest(startTime,
                                                                                       this,
                                                                                       connection,
                                                                                       request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.RequestStopTransactionResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.RequestStopTransactionResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid RequestStopTransaction request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomRequestStopTransactionRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.RequestStopTransactionResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming RequestStopTransaction for '{request.TransactionId}'!");

                            // TransactionId

                            response = new OCPPv2_1.CS.RequestStopTransactionResponse(
                                           request,
                                           RequestStartStopStatus.Rejected
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomRequestStopTransactionResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnRequestStopTransactionResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnRequestStopTransactionResponse(responseTime,
                                                                                        this,
                                                                                        connection,
                                                                                        request,
                                                                                        response,
                                                                                        responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetTransactionStatus

                IncomingMessages.OnGetTransactionStatus += async (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  cancellationToken) => {

                    #region Send OnGetTransactionStatusRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetTransactionStatusRequest(startTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetTransactionStatusResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetTransactionStatusResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetTransactionStatus request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomGetTransactionStatusRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetTransactionStatusResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetTransactionStatus for '{request.TransactionId}'!");

                            // TransactionId

                            response = new OCPPv2_1.CS.GetTransactionStatusResponse(
                                           request,
                                           MessagesInQueue:    false,
                                           OngoingIndicator:   false
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomGetTransactionStatusResponseSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetTransactionStatusResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetTransactionStatusResponse(responseTime,
                                                                                      this,
                                                                                      connection,
                                                                                      request,
                                                                                      response,
                                                                                      responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnSetChargingProfile

                IncomingMessages.OnSetChargingProfile += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                    #region Send OnSetChargingProfileRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetChargingProfileRequest(startTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SetChargingProfileResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SetChargingProfileResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SetChargingProfile request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(

                                     parentNetworkingNode.CustomSetChargingProfileRequestSerializer,
                                     parentNetworkingNode.CustomChargingProfileSerializer,
                                     parentNetworkingNode.CustomLimitBeyondSoCSerializer,
                                     parentNetworkingNode.CustomChargingScheduleSerializer,
                                     parentNetworkingNode.CustomChargingSchedulePeriodSerializer,
                                     parentNetworkingNode.CustomV2XFreqWattEntrySerializer,
                                     parentNetworkingNode.CustomV2XSignalWattEntrySerializer,
                                     parentNetworkingNode.CustomSalesTariffSerializer,
                                     parentNetworkingNode.CustomSalesTariffEntrySerializer,
                                     parentNetworkingNode.CustomRelativeTimeIntervalSerializer,
                                     parentNetworkingNode.CustomConsumptionCostSerializer,
                                     parentNetworkingNode.CustomCostSerializer,

                                     parentNetworkingNode.CustomAbsolutePriceScheduleSerializer,
                                     parentNetworkingNode.CustomPriceRuleStackSerializer,
                                     parentNetworkingNode.CustomPriceRuleSerializer,
                                     parentNetworkingNode.CustomTaxRuleSerializer,
                                     parentNetworkingNode.CustomOverstayRuleListSerializer,
                                     parentNetworkingNode.CustomOverstayRuleSerializer,
                                     parentNetworkingNode.CustomAdditionalServiceSerializer,

                                     parentNetworkingNode.CustomPriceLevelScheduleSerializer,
                                     parentNetworkingNode.CustomPriceLevelScheduleEntrySerializer,

                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer

                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SetChargingProfileResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SetChargingProfile for '{request.EVSEId}'!");

                            // EVSEId
                            // ChargingProfile

                            response = new OCPPv2_1.CS.SetChargingProfileResponse(
                                           request,
                                           ChargingProfileStatus.Rejected
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomSetChargingProfileResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetChargingProfileResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetChargingProfileResponse(responseTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request,
                                                                                    response,
                                                                                    responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetChargingProfiles

                IncomingMessages.OnGetChargingProfiles += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                    #region Send OnGetChargingProfilesRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetChargingProfilesRequest(startTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetChargingProfilesResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetChargingProfilesResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetChargingProfiles request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomGetChargingProfilesRequestSerializer,
                                     parentNetworkingNode.CustomChargingProfileCriterionSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetChargingProfilesResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetChargingProfiles request ({request.GetChargingProfilesRequestId}) for '{request.EVSEId}'!");

                            // GetChargingProfilesRequestId
                            // ChargingProfile
                            // EVSEId

                            response = new OCPPv2_1.CS.GetChargingProfilesResponse(
                                           request,
                                           GetChargingProfileStatus.Unknown
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomGetChargingProfilesResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetChargingProfilesResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetChargingProfilesResponse(responseTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnClearChargingProfile

                IncomingMessages.OnClearChargingProfile += async (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  cancellationToken) => {

                    #region Send OnClearChargingProfileRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnClearChargingProfileRequest(startTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.ClearChargingProfileResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.ClearChargingProfileResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid ClearChargingProfile request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomClearChargingProfileRequestSerializer,
                                     parentNetworkingNode.CustomClearChargingProfileSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.ClearChargingProfileResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming ClearChargingProfile request for charging profile identification '{request.ChargingProfileId}'!");

                            // ChargingProfileId
                            // ChargingProfileCriteria

                            response = new OCPPv2_1.CS.ClearChargingProfileResponse(
                                           Request:      request,
                                           Status:       ClearChargingProfileStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomClearChargingProfileResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnClearChargingProfileResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnClearChargingProfileResponse(responseTime,
                                                                                      this,
                                                                                      connection,
                                                                                      request,
                                                                                      response,
                                                                                      responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetCompositeSchedule

                IncomingMessages.OnGetCompositeSchedule += async (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  cancellationToken) => {

                    #region Send OnGetCompositeScheduleRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetCompositeScheduleRequest(startTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetCompositeScheduleResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetCompositeScheduleResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetCompositeSchedule request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomGetCompositeScheduleRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetCompositeScheduleResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetCompositeSchedule request for the next {request.Duration.TotalMinutes} minutes of EVSE '{request.EVSEId}'!");

                            // Duration,
                            // EVSEId,
                            // ChargingRateUnit

                            response = new OCPPv2_1.CS.GetCompositeScheduleResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           Schedule:     null,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomGetCompositeScheduleResponseSerializer,
                            parentNetworkingNode.CustomCompositeScheduleSerializer,
                            parentNetworkingNode.CustomChargingSchedulePeriodSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetCompositeScheduleResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetCompositeScheduleResponse(responseTime,
                                                                                      this,
                                                                                      connection,
                                                                                      request,
                                                                                      response,
                                                                                      responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnUpdateDynamicSchedule

                IncomingMessages.OnUpdateDynamicSchedule += async (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   cancellationToken) => {

                    #region Send OnUpdateDynamicScheduleRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUpdateDynamicScheduleRequest(startTime,
                                                                                      this,
                                                                                      connection,
                                                                                      request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.UpdateDynamicScheduleResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.UpdateDynamicScheduleResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid UpdateDynamicSchedule request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomUpdateDynamicScheduleRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.UpdateDynamicScheduleResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming UpdateDynamicSchedule request for charging profile '{request.ChargingProfileId}'!");

                            // ChargingProfileId

                            // Limit
                            // Limit_L2
                            // Limit_L3

                            // DischargeLimit
                            // DischargeLimit_L2
                            // DischargeLimit_L3

                            // Setpoint
                            // Setpoint_L2
                            // Setpoint_L3

                            // SetpointReactive
                            // SetpointReactive_L2
                            // SetpointReactive_L3

                            response = new OCPPv2_1.CS.UpdateDynamicScheduleResponse(
                                           Request:      request,
                                           Status:       ChargingProfileStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomUpdateDynamicScheduleResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnUpdateDynamicScheduleResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUpdateDynamicScheduleResponse(responseTime,
                                                                                       this,
                                                                                       connection,
                                                                                       request,
                                                                                       response,
                                                                                       responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnNotifyAllowedEnergyTransfer

                IncomingMessages.OnNotifyAllowedEnergyTransfer += async (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         cancellationToken) => {

                    #region Send OnNotifyAllowedEnergyTransferRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnNotifyAllowedEnergyTransferRequest(startTime,
                                                                                            this,
                                                                                            connection,
                                                                                            request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.NotifyAllowedEnergyTransferResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.NotifyAllowedEnergyTransferResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid NotifyAllowedEnergyTransfer request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomNotifyAllowedEnergyTransferRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.NotifyAllowedEnergyTransferResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming NotifyAllowedEnergyTransfer request allowing energy transfer modes: '{request.AllowedEnergyTransferModes.Select(mode => mode.ToString()).AggregateWith(", ")}'!");

                            // AllowedEnergyTransferModes

                            response = new OCPPv2_1.CS.NotifyAllowedEnergyTransferResponse(
                                           Request:      request,
                                           Status:       NotifyAllowedEnergyTransferStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomNotifyAllowedEnergyTransferResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnNotifyAllowedEnergyTransferResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnNotifyAllowedEnergyTransferResponse(responseTime,
                                                                                             this,
                                                                                             connection,
                                                                                             request,
                                                                                             response,
                                                                                             responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnUsePriorityCharging

                IncomingMessages.OnUsePriorityCharging += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                    #region Send OnUsePriorityChargingRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUsePriorityChargingRequest(startTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.UsePriorityChargingResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.UsePriorityChargingResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid UsePriorityCharging request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomUsePriorityChargingRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.UsePriorityChargingResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming UsePriorityCharging request for transaction '{request.TransactionId}': {(request.Activate ? "active" : "disabled")}!");

                            // TransactionId
                            // Activate

                            response = new OCPPv2_1.CS.UsePriorityChargingResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomUsePriorityChargingResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnUsePriorityChargingResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUsePriorityChargingResponse(responseTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnUnlockConnector

                IncomingMessages.OnUnlockConnector += async (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             cancellationToken) => {

                    #region Send OnUnlockConnectorRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUnlockConnectorRequest(startTime,
                                                                                this,
                                                                                connection,
                                                                                request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.UnlockConnectorResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.UnlockConnectorResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid UnlockConnector request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomUnlockConnectorRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.UnlockConnectorResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming UnlockConnector request for EVSE '{request.EVSEId}' and connector '{request.ConnectorId}'!");

                            // EVSEId
                            // ConnectorId

                            response = new OCPPv2_1.CS.UnlockConnectorResponse(
                                           Request:      request,
                                           Status:       UnlockStatus.UnlockFailed,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomUnlockConnectorResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnUnlockConnectorResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUnlockConnectorResponse(responseTime,
                                                                                 this,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion


                #region OnAFRRSignal

                IncomingMessages.OnAFRRSignal += async (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        cancellationToken) => {

                    #region Send OnAFRRSignalRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnAFRRSignalRequest(startTime,
                                                                           this,
                                                                           connection,
                                                                           request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.AFRRSignalResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.AFRRSignalResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid AFRRSignal request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomAFRRSignalRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.AFRRSignalResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming AFRRSignal '{request.Signal}' for timestamp '{request.ActivationTimestamp}'!");

                            // ActivationTimestamp
                            // Signal

                            response = new OCPPv2_1.CS.AFRRSignalResponse(
                                           Request:      request,
                                           Status:       request.ActivationTimestamp < Timestamp.Now - TimeSpan.FromDays(1)
                                                             ? GenericStatus.Rejected
                                                             : GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomAFRRSignalResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnAFRRSignalResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnAFRRSignalResponse(responseTime,
                                                                            this,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion


                #region OnSetDisplayMessage

                IncomingMessages.OnSetDisplayMessage += async (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) => {

                    #region Send OnSetDisplayMessageRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetDisplayMessageRequest(startTime,
                                                                                  this,
                                                                                  connection,
                                                                                  request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SetDisplayMessageResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SetDisplayMessageResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SetDisplayMessage request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomSetDisplayMessageRequestSerializer,
                                     parentNetworkingNode.CustomMessageInfoSerializer,
                                     parentNetworkingNode.CustomMessageContentSerializer,
                                     parentNetworkingNode.CustomComponentSerializer,
                                     parentNetworkingNode.CustomEVSESerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SetDisplayMessageResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SetDisplayMessage '{request.Message.Message.Content}'!");

                            // Message

                            if (displayMessages.TryAdd(request.Message.Id,
                                                       request.Message)) {

                                response = new OCPPv2_1.CS.SetDisplayMessageResponse(
                                               Request:      request,
                                               Status:       DisplayMessageStatus.Accepted,
                                               StatusInfo:   null,
                                               CustomData:   null
                                           );

                            }

                            else
                                response = new OCPPv2_1.CS.SetDisplayMessageResponse(
                                               Request:      request,
                                               Status:       DisplayMessageStatus.Rejected,
                                               StatusInfo:   null,
                                               CustomData:   null
                                           );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomSetDisplayMessageResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetDisplayMessageResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetDisplayMessageResponse(responseTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetDisplayMessages

                IncomingMessages.OnGetDisplayMessages += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                    #region Send OnGetDisplayMessagesRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetDisplayMessagesRequest(startTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetDisplayMessagesResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetDisplayMessagesResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetDisplayMessages request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomGetDisplayMessagesRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetDisplayMessagesResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetDisplayMessages request ({request.GetDisplayMessagesRequestId})!");

                            // GetDisplayMessagesRequestId
                            // Ids
                            // Priority
                            // State

                            _ = Task.Run(async () => {

                                var filteredDisplayMessages = displayMessages.Values.
                                                                  Where(displayMessage =>  request.Ids is null || !request.Ids.Any() || request.Ids.Contains(displayMessage.Id)).
                                                                  Where(displayMessage => !request.State.   HasValue || (displayMessage.State.HasValue && displayMessage.State.Value == request.State.   Value)).
                                                                  Where(displayMessage => !request.Priority.HasValue ||  displayMessage.Priority                                     == request.Priority.Value).
                                                                  ToArray();

                                await parentNetworkingNode.NotifyDisplayMessages(
                                          NotifyDisplayMessagesRequestId:   1,
                                          MessageInfos:                     filteredDisplayMessages,
                                          ToBeContinued:                    false,
                                          CustomData:                       null
                                      );

                            },
                            CancellationToken.None);

                            response = new OCPPv2_1.CS.GetDisplayMessagesResponse(
                                           request,
                                           GetDisplayMessagesStatus.Accepted
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomGetDisplayMessagesResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetDisplayMessagesResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetDisplayMessagesResponse(responseTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request,
                                                                                    response,
                                                                                    responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnClearDisplayMessage

                IncomingMessages.OnClearDisplayMessage += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                    #region Send OnClearDisplayMessageRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnClearDisplayMessageRequest(startTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.ClearDisplayMessageResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.ClearDisplayMessageResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid ClearDisplayMessage request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomClearDisplayMessageRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.ClearDisplayMessageResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming ClearDisplayMessage request ({request.DisplayMessageId})!");

                            // DisplayMessageId

                            if (displayMessages.TryGetValue(request.DisplayMessageId, out var messageInfo) &&
                                displayMessages.TryRemove(new KeyValuePair<DisplayMessage_Id, MessageInfo>(request.DisplayMessageId, messageInfo))) {

                                response = new OCPPv2_1.CS.ClearDisplayMessageResponse(
                                               request,
                                               ClearMessageStatus.Accepted
                                           );

                            }

                            else
                                response = new OCPPv2_1.CS.ClearDisplayMessageResponse(
                                               request,
                                               ClearMessageStatus.Unknown
                                           );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomClearDisplayMessageResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnClearDisplayMessageResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnClearDisplayMessageResponse(responseTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnCostUpdated

                IncomingMessages.OnCostUpdated += async (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         cancellationToken) => {

                    #region Send OnCostUpdatedRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnCostUpdatedRequest(startTime,
                                                                            this,
                                                                            connection,
                                                                            request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.CostUpdatedResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.CostUpdatedResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid CostUpdated request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomCostUpdatedRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.CostUpdatedResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming CostUpdated request '{request.TotalCost}' for transaction '{request.TransactionId}'!");

                            // TotalCost
                            // TransactionId

                            if (transactions.ContainsKey(request.TransactionId)) {

                                totalCosts.AddOrUpdate(request.TransactionId,
                                                       request.TotalCost,
                                                       (transactionId, totalCost) => request.TotalCost);

                                response = new OCPPv2_1.CS.CostUpdatedResponse(
                                               request
                                           );

                            }

                            else
                                response = new OCPPv2_1.CS.CostUpdatedResponse(
                                               request,
                                               Result.GenericError($"Unknown transaction identification '{request.TransactionId}'!")
                                           );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomCostUpdatedResponseSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnCostUpdatedResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnCostUpdatedResponse(responseTime,
                                                                             this,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnCustomerInformation

                IncomingMessages.OnCustomerInformation += async (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request,
                                                                 cancellationToken) => {

                    #region Send OnCustomerInformationRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnCustomerInformationRequest(startTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.CustomerInformationResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.CustomerInformationResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid parentNetworkingNode.CustomerInformation request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomCustomerInformationRequestSerializer,
                                     parentNetworkingNode.CustomIdTokenSerializer,
                                     parentNetworkingNode.CustomAdditionalInfoSerializer,
                                     parentNetworkingNode.CustomCertificateHashDataSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.CustomerInformationResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            var command   = new String[] {

                                                request.Report
                                                    ? "report"
                                                    : "",

                                                request.Clear
                                                    ? "clear"
                                                    : "",

                                            }.Where(text => text.IsNotNullOrEmpty()).
                                              AggregateWith(" and ");

                            var customer  = request.IdToken is not null
                                               ? $"IdToken: {request.IdToken.Value}"
                                               : request.CustomerCertificate is not null
                                                     ? $"certificate s/n: {request.CustomerCertificate.SerialNumber}"
                                                     : request.CustomerIdentifier.HasValue
                                                           ? $"customer identifier: {request.CustomerIdentifier.Value}"
                                                           : "-";


                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming parentNetworkingNode.CustomerInformation request ({request.CustomerInformationRequestId}) to {command} for customer '{customer}'!");

                            // parentNetworkingNode.CustomerInformationRequestId
                            // Report
                            // Clear
                            // parentNetworkingNode.CustomerIdentifier
                            // IdToken
                            // parentNetworkingNode.CustomerCertificate



                            _ = Task.Run(async () => {

                                await parentNetworkingNode.NotifyCustomerInformation(
                                          NotifyCustomerInformationRequestId:   1,
                                          Data:                                 customer,
                                          SequenceNumber:                       1,
                                          GeneratedAt:                          Timestamp.Now,
                                          ToBeContinued:                        false,
                                          CustomData:                           null
                                      );

                            },
                            CancellationToken.None);



                            response = new OCPPv2_1.CS.CustomerInformationResponse(
                                           request,
                                           CustomerInformationStatus.Accepted
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomCustomerInformationResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnCustomerInformationResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnCustomerInformationResponse(responseTime,
                                                                                     this,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion


                // Binary Data Streams Extensions

                #region OnIncomingBinaryDataTransfer

                IncomingMessages.OnIncomingBinaryDataTransfer += async (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        cancellationToken) => {

                    #region Send OnBinaryDataTransferRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnIncomingBinaryDataTransferRequest(startTime,
                                                                                           this,
                                                                                           connection,
                                                                                           request);

                    #endregion


                    #region Check charging station identification

                    BinaryDataTransferResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.BinaryDataTransferResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid BinaryDataTransfer request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToBinary(
                                     parentNetworkingNode.CustomIncomingBinaryDataTransferRequestSerializer,
                                     parentNetworkingNode.CustomBinarySignatureSerializer,
                                     IncludeSignatures: false
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new BinaryDataTransferResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming BinaryDataTransfer request: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data?.ToHexString() ?? "-"}!");

                            // VendorId
                            // MessageId
                            // Data

                            var responseBinaryData = request.Data;

                            if (request.Data is not null)
                                responseBinaryData = request.Data.Reverse();

                            response = request.VendorId == Vendor_Id.GraphDefined

                                           ? new BinaryDataTransferResponse(
                                                 Request:                request,
                                                 Status:                 BinaryDataTransferStatus.Accepted,
                                                 AdditionalStatusInfo:   null,
                                                 Data:                   responseBinaryData
                                             )

                                           : new BinaryDataTransferResponse(
                                                 Request:                request,
                                                 Status:                 BinaryDataTransferStatus.Rejected,
                                                 AdditionalStatusInfo:   null,
                                                 Data:                   responseBinaryData
                                             );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToBinary(
                            parentNetworkingNode.CustomIncomingBinaryDataTransferResponseSerializer,
                            null, //CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomBinarySignatureSerializer,
                            IncludeSignatures: false
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnBinaryDataTransferResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnIncomingBinaryDataTransferResponse(responseTime,
                                                                                            this,
                                                                                            connection,
                                                                                            request,
                                                                                            response,
                                                                                            responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetFile

                IncomingMessages.OnGetFile += async (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     cancellationToken) => {

                    #region Send OnGetFileRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetFileRequest(startTime,
                                                                        this,
                                                                        connection,
                                                                        request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.GetFileResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.GetFileResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetFile request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomGetFileRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.GetFileResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetFile request: {request.FileName}!");

                            response = request.FileName.ToString() == "/hello/world.txt"

                                           ? new OCPP.CS.GetFileResponse(
                                                 Request:           request,
                                                 FileName:          request.FileName,
                                                 Status:            GetFileStatus.Success,
                                                 FileContent:       "Hello world!".ToUTF8Bytes(),
                                                 FileContentType:   ContentType.Text.Plain,
                                                 FileSHA256:        SHA256.HashData("Hello world!".ToUTF8Bytes()),
                                                 FileSHA512:        SHA512.HashData("Hello world!".ToUTF8Bytes())
                                             )

                                           : new OCPP.CS.GetFileResponse(
                                                 Request:           request,
                                                 FileName:          request.FileName,
                                                 Status:            GetFileStatus.NotFound
                                             );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToBinary(
                            parentNetworkingNode.CustomGetFileResponseSerializer,
                            null, //CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomBinarySignatureSerializer,
                            IncludeSignatures: false
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetFileResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetFileResponse(responseTime,
                                                                         this,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnSendFile

                IncomingMessages.OnSendFile += async (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      cancellationToken) => {

                    #region Send OnSendFileRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSendFileRequest(startTime,
                                                                         this,
                                                                         connection,
                                                                         request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.SendFileResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.SendFileResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SendFile request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToBinary(
                                     parentNetworkingNode.CustomSendFileRequestSerializer,
                                     parentNetworkingNode.CustomBinarySignatureSerializer,
                                     IncludeSignatures: false
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.SendFileResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SendFile request: {request.FileName}!");

                            response = request.FileName.ToString() == "/hello/world.txt"

                                           ? new OCPP.CS.SendFileResponse(
                                                 Request:   request,
                                                 FileName:  request.FileName,
                                                 Status:    SendFileStatus.Success
                                             )

                                           : new OCPP.CS.SendFileResponse(
                                                 Request:   request,
                                                 FileName:  request.FileName,
                                                 Status:    SendFileStatus.NotFound
                                             );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomSendFileResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSendFileResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSendFileResponse(responseTime,
                                                                          this,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnDeleteFile

                IncomingMessages.OnDeleteFile += async (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        cancellationToken) => {

                    #region Send OnDeleteFileRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnDeleteFileRequest(startTime,
                                                                           this,
                                                                           connection,
                                                                           request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.DeleteFileResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.DeleteFileResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid DeleteFile request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomDeleteFileRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.DeleteFileResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming DeleteFile request: {request.FileName}!");

                            response = request.FileName.ToString() == "/hello/world.txt"

                                           ? new OCPP.CS.DeleteFileResponse(
                                                 Request:   request,
                                                 FileName:  request.FileName,
                                                 Status:    DeleteFileStatus.Success
                                             )

                                           : new OCPP.CS.DeleteFileResponse(
                                                 Request:   request,
                                                 FileName:  request.FileName,
                                                 Status:    DeleteFileStatus.NotFound
                                             );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomDeleteFileResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnDeleteFileResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnDeleteFileResponse(responseTime,
                                                                            this,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion


                // E2E Security Extensions

                #region OnAddSignaturePolicy

                IncomingMessages.OnAddSignaturePolicy += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                cancellationToken) => {

                    #region Send OnAddSignaturePolicyRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnAddSignaturePolicyRequest(startTime,
                                                                                   this,
                                                                                   connection,
                                                                                   request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.AddSignaturePolicyResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.AddSignaturePolicyResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid AddSignaturePolicy request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     //CustomAddSignaturePolicyRequestSerializer,
                                     //CustomMessageInfoSerializer,
                                     //CustomMessageContentSerializer,
                                     //CustomComponentSerializer,
                                     //CustomEVSESerializer,
                                     //CustomSignatureSerializer,
                                     //CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.AddSignaturePolicyResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming AddSignaturePolicy!");

                            // Message


                                response = new OCPP.CS.AddSignaturePolicyResponse(
                                               Request:      request,
                                               Status:       GenericStatus.Accepted,
                                               StatusInfo:   null,
                                               CustomData:   null
                                           );


                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomAddSignaturePolicyResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnAddSignaturePolicyResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnAddSignaturePolicyResponse(responseTime,
                                                                                    this,
                                                                                    connection,
                                                                                    request,
                                                                                    response,
                                                                                    responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnUpdateSignaturePolicy

                IncomingMessages.OnUpdateSignaturePolicy += async (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   cancellationToken) => {

                    #region Send OnUpdateSignaturePolicyRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUpdateSignaturePolicyRequest(startTime,
                                                                                      this,
                                                                                      connection,
                                                                                      request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.UpdateSignaturePolicyResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.UpdateSignaturePolicyResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid UpdateSignaturePolicy request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     //CustomUpdateSignaturePolicyRequestSerializer,
                                     //CustomMessageInfoSerializer,
                                     //CustomMessageContentSerializer,
                                     //CustomComponentSerializer,
                                     //CustomEVSESerializer,
                                     //CustomSignatureSerializer,
                                     //CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.UpdateSignaturePolicyResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming UpdateSignaturePolicy!");

                            // Message

                            response = new OCPP.CS.UpdateSignaturePolicyResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomUpdateSignaturePolicyResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnUpdateSignaturePolicyResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUpdateSignaturePolicyResponse(responseTime,
                                                                                       this,
                                                                                       connection,
                                                                                       request,
                                                                                       response,
                                                                                       responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnDeleteSignaturePolicy

                IncomingMessages.OnDeleteSignaturePolicy += async (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   cancellationToken) => {

                    #region Send OnDeleteSignaturePolicyRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnDeleteSignaturePolicyRequest(startTime,
                                                                                      this,
                                                                                      connection,
                                                                                      request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.DeleteSignaturePolicyResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.DeleteSignaturePolicyResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid DeleteSignaturePolicy request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     //CustomDeleteSignaturePolicyRequestSerializer,
                                     //CustomMessageInfoSerializer,
                                     //CustomMessageContentSerializer,
                                     //CustomComponentSerializer,
                                     //CustomEVSESerializer,
                                     //CustomSignatureSerializer,
                                     //CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.DeleteSignaturePolicyResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming DeleteSignaturePolicy!");

                            // Message

                            response = new OCPP.CS.DeleteSignaturePolicyResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomDeleteSignaturePolicyResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnDeleteSignaturePolicyResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnDeleteSignaturePolicyResponse(responseTime,
                                                                                       this,
                                                                                       connection,
                                                                                       request,
                                                                                       response,
                                                                                       responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnAddUserRole

                IncomingMessages.OnAddUserRole += async (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         cancellationToken) => {

                    #region Send OnAddUserRoleRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnAddUserRoleRequest(startTime,
                                                                            this,
                                                                            connection,
                                                                            request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.AddUserRoleResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.AddUserRoleResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid AddUserRole request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     //CustomAddUserRoleRequestSerializer,
                                     //CustomMessageInfoSerializer,
                                     //CustomMessageContentSerializer,
                                     //CustomComponentSerializer,
                                     //CustomEVSESerializer,
                                     //CustomSignatureSerializer,
                                     //CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.AddUserRoleResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming AddUserRole!");

                            // Message

                            response = new OCPP.CS.AddUserRoleResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomAddUserRoleResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnAddUserRoleResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnAddUserRoleResponse(responseTime,
                                                                             this,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnUpdateUserRole

                IncomingMessages.OnUpdateUserRole += async (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            cancellationToken) => {

                    #region Send OnUpdateUserRoleRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUpdateUserRoleRequest(startTime,
                                                                               this,
                                                                               connection,
                                                                               request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.UpdateUserRoleResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.UpdateUserRoleResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid UpdateUserRole request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     //CustomUpdateUserRoleRequestSerializer,
                                     //CustomMessageInfoSerializer,
                                     //CustomMessageContentSerializer,
                                     //CustomComponentSerializer,
                                     //CustomEVSESerializer,
                                     //CustomSignatureSerializer,
                                     //CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.UpdateUserRoleResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming UpdateUserRole!");

                            // Message

                            response = new OCPP.CS.UpdateUserRoleResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomUpdateUserRoleResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnUpdateUserRoleResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnUpdateUserRoleResponse(responseTime,
                                                                                this,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnDeleteUserRole

                IncomingMessages.OnDeleteUserRole += async (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            cancellationToken) => {

                    #region Send OnDeleteUserRoleRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnDeleteUserRoleRequest(startTime,
                                                                               this,
                                                                               connection,
                                                                               request);

                    #endregion


                    #region Check charging station identification

                    OCPP.CS.DeleteUserRoleResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPP.CS.DeleteUserRoleResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid DeleteUserRole request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     //CustomDeleteUserRoleRequestSerializer,
                                     //CustomMessageInfoSerializer,
                                     //CustomMessageContentSerializer,
                                     //CustomComponentSerializer,
                                     //CustomEVSESerializer,
                                     //CustomSignatureSerializer,
                                     //CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPP.CS.DeleteUserRoleResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming DeleteUserRole!");

                            // Message

                            response = new OCPP.CS.DeleteUserRoleResponse(
                                           Request:      request,
                                           Status:       GenericStatus.Accepted,
                                           StatusInfo:   null,
                                           CustomData:   null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomDeleteUserRoleResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnDeleteUserRoleResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnDeleteUserRoleResponse(responseTime,
                                                                                this,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion


                // E2E Charging Tariffs Extensions

                #region OnSetDefaultChargingTariff

                IncomingMessages.OnSetDefaultChargingTariff += async (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      cancellationToken) => {

                    #region Send OnSetDefaultChargingTariffRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetDefaultChargingTariffRequest(startTime,
                                                                                         this,
                                                                                         connection,
                                                                                         request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.SetDefaultChargingTariffResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.SetDefaultChargingTariffResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid SetDefaultChargingTariff request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomSetDefaultChargingTariffRequestSerializer,
                                     parentNetworkingNode.CustomChargingTariffSerializer,
                                     parentNetworkingNode.CustomPriceSerializer,
                                     parentNetworkingNode.CustomTariffElementSerializer,
                                     parentNetworkingNode.CustomPriceComponentSerializer,
                                     parentNetworkingNode.CustomTaxRateSerializer,
                                     parentNetworkingNode.CustomTariffRestrictionsSerializer,
                                     parentNetworkingNode.CustomEnergyMixSerializer,
                                     parentNetworkingNode.CustomEnergySourceSerializer,
                                     parentNetworkingNode.CustomEnvironmentalImpactSerializer,
                                     parentNetworkingNode.CustomIdTokenSerializer,
                                     parentNetworkingNode.CustomAdditionalInfoSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.SetDefaultChargingTariffResponse(
                                           Request:   request,
                                           Result:    Result.SignatureError(
                                                          $"Invalid signature: {errorResponse}"
                                                      )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming SetDefaultChargingTariff!");

                            List<EVSEStatusInfo<SetDefaultChargingTariffStatus>>? evseStatusInfos = null;

                            if (!request.ChargingTariff.Verify(out var err))
                            {
                                response = new OCPPv2_1.CS.SetDefaultChargingTariffResponse(
                                               Request:      request,
                                               Status:       SetDefaultChargingTariffStatus.InvalidSignature,
                                               StatusInfo:   new StatusInfo(
                                                                 ReasonCode:       "Invalid charging tariff signature(s)!",
                                                                 AdditionalInfo:   err,
                                                                 CustomData:       null
                                                             ),
                                               CustomData:   null
                                           );
                            }

                            else if (!request.EVSEIds.Any())
                            {

                                response = new OCPPv2_1.CS.SetDefaultChargingTariffResponse(
                                               Request:           request,
                                               Status:            SetDefaultChargingTariffStatus.Accepted,
                                               StatusInfo:        null,
                                               EVSEStatusInfos:   null,
                                               CustomData:        null
                                           );

                            }

                            else
                            {

                                foreach (var evseId in request.EVSEIds)
                                {
                                        response = new OCPPv2_1.CS.SetDefaultChargingTariffResponse(
                                                       Request:   request,
                                                       Result:    Result.SignatureError(
                                                                      $"Invalid EVSE identification: {evseId}"
                                                                  )
                                                   );
                                }

                                if (response == null)
                                {

                                    evseStatusInfos = new List<EVSEStatusInfo<SetDefaultChargingTariffStatus>>();

                                    foreach (var evseId in request.EVSEIds)
                                    {

                                        evseStatusInfos.Add(new EVSEStatusInfo<SetDefaultChargingTariffStatus>(
                                                                EVSEId:           evseId,
                                                                Status:           SetDefaultChargingTariffStatus.Accepted,
                                                                ReasonCode:       null,
                                                                AdditionalInfo:   null,
                                                                CustomData:       null
                                                            ));

                                    }

                                    response = new OCPPv2_1.CS.SetDefaultChargingTariffResponse(
                                                   Request:           request,
                                                   Status:            SetDefaultChargingTariffStatus.Accepted,
                                                   StatusInfo:        null,
                                                   EVSEStatusInfos:   evseStatusInfos,
                                                   CustomData:        null
                                               );

                                }

                            }

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomSetDefaultChargingTariffResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomEVSEStatusInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSetDefaultChargingTariffResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnSetDefaultChargingTariffResponse(responseTime,
                                                                                          this,
                                                                                          connection,
                                                                                          request,
                                                                                          response,
                                                                                          responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnGetDefaultChargingTariff

                IncomingMessages.OnGetDefaultChargingTariff += async (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      cancellationToken) => {

                    #region Send OnGetDefaultChargingTariffRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetDefaultChargingTariffRequest(startTime,
                                                                                         this,
                                                                                         connection,
                                                                                         request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.GetDefaultChargingTariffResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.GetDefaultChargingTariffResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid GetDefaultChargingTariff request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomGetDefaultChargingTariffRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.GetDefaultChargingTariffResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming GetDefaultChargingTariff!");

                            response = new OCPPv2_1.CS.GetDefaultChargingTariffResponse(
                                           Request:             request,
                                           Status:              GenericStatus.Accepted,
                                           StatusInfo:          null,
                                           ChargingTariffs:     null,
                                           ChargingTariffMap:   null,
                                           CustomData:          null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomGetDefaultChargingTariffResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomChargingTariffSerializer,
                            parentNetworkingNode.CustomPriceSerializer,
                            parentNetworkingNode.CustomTariffElementSerializer,
                            parentNetworkingNode.CustomPriceComponentSerializer,
                            parentNetworkingNode.CustomTaxRateSerializer,
                            parentNetworkingNode.CustomTariffRestrictionsSerializer,
                            parentNetworkingNode.CustomEnergyMixSerializer,
                            parentNetworkingNode.CustomEnergySourceSerializer,
                            parentNetworkingNode.CustomEnvironmentalImpactSerializer,
                            parentNetworkingNode.CustomIdTokenSerializer,
                            parentNetworkingNode.CustomAdditionalInfoSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetDefaultChargingTariffResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnGetDefaultChargingTariffResponse(responseTime,
                                                                                          this,
                                                                                          connection,
                                                                                          request,
                                                                                          response,
                                                                                          responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

                #region OnRemoveDefaultChargingTariff

                IncomingMessages.OnRemoveDefaultChargingTariff += async (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         cancellationToken) => {

                    #region Send OnRemoveDefaultChargingTariffRequest event

                    var startTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnRemoveDefaultChargingTariffRequest(startTime,
                                                                                            this,
                                                                                            connection,
                                                                                            request);

                    #endregion


                    #region Check charging station identification

                    OCPPv2_1.CS.RemoveDefaultChargingTariffResponse? response = null;

                    //if (request.ChargingStationId != Id)
                    //{
                    //    response = new OCPPv2_1.CS.RemoveDefaultChargingTariffResponse(
                    //                   Request:  request,
                    //                   Result:   Result.GenericError(
                    //                                 $"Charging Station '{parentNetworkingNode.Id}': Invalid RemoveDefaultChargingTariff request for charging station '{request.ChargingStationId}'!"
                    //                             )
                    //               );
                    //}
                    if (1 == 2) { }

                    #endregion

                    #region Check request signature(s)

                    else
                    {

                        if (!parentNetworkingNode.SignaturePolicy.VerifyRequestMessage(
                                 request,
                                 request.ToJSON(
                                     parentNetworkingNode.CustomRemoveDefaultChargingTariffRequestSerializer,
                                     parentNetworkingNode.CustomSignatureSerializer,
                                     parentNetworkingNode.CustomCustomDataSerializer
                                 ),
                                 out var errorResponse
                             ))
                        {

                            response = new OCPPv2_1.CS.RemoveDefaultChargingTariffResponse(
                                           Request:  request,
                                           Result:   Result.SignatureError(
                                                         $"Invalid signature: {errorResponse}"
                                                     )
                                       );

                        }

                    #endregion

                        else
                        {

                            DebugX.Log($"Charging Station '{parentNetworkingNode.Id}': Incoming RemoveDefaultChargingTariff!");

                            response = new OCPPv2_1.CS.RemoveDefaultChargingTariffResponse(
                                           Request:           request,
                                           Status:            RemoveDefaultChargingTariffStatus.Accepted,
                                           StatusInfo:        null,
                                           EVSEStatusInfos:   null,
                                           CustomData:        null
                                       );

                        }

                    }

                    #region Sign response message

                    parentNetworkingNode.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.CustomRemoveDefaultChargingTariffResponseSerializer,
                            parentNetworkingNode.CustomStatusInfoSerializer,
                            parentNetworkingNode.CustomEVSEStatusInfoSerializer2,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnRemoveDefaultChargingTariffResponse event

                    var responseTime = Timestamp.Now;

                    await parentNetworkingNode.IN.RaiseOnRemoveDefaultChargingTariffResponse(responseTime,
                                                                                             this,
                                                                                             connection,
                                                                                             request,
                                                                                             response,
                                                                                             responseTime - startTime);

                    #endregion

                    return response;

                };

                #endregion

            }

            #endregion


            #region (Timer) DoMaintenance(State)

            private void DoMaintenanceSync(Object? State)
            {
                if (!DisableMaintenanceTasks)
                    DoMaintenance(State).Wait();
            }

            protected internal virtual async Task _DoMaintenance(Object State)
            {

                foreach (var enqueuedRequest in EnqueuedRequests.ToArray())
                {
                    if (CSClient is NetworkingNodeWSClient wsClient)
                    {

                        var response = await wsClient.SendRequest(
                                                 enqueuedRequest.NetworkingNodeId,
                                                 enqueuedRequest.NetworkPath,
                                                 enqueuedRequest.Command,
                                                 enqueuedRequest.Request.RequestId,
                                                 enqueuedRequest.RequestJSON
                                             );

                        enqueuedRequest.ResponseAction(response);

                        EnqueuedRequests.Remove(enqueuedRequest);

                    }
                }

            }

            private async Task DoMaintenance(Object State)
            {

                if (await MaintenanceSemaphore.WaitAsync(SemaphoreSlimTimeout).
                                               ConfigureAwait(false))
                {
                    try
                    {

                        await _DoMaintenance(State);

                    }
                    catch (Exception e)
                    {

                        while (e.InnerException is not null)
                            e = e.InnerException;

                        DebugX.LogException(e);

                    }
                    finally
                    {
                        MaintenanceSemaphore.Release();
                    }
                }
                else
                    DebugX.LogT("Could not aquire the maintenance tasks lock!");

            }

            #endregion

            #region (Timer) DoSendHeartbeatSync(State)

            private void DoSendHeartbeatSync(Object? State)
            {
                if (!DisableSendHeartbeats)
                {
                    try
                    {
                        this.parentNetworkingNode.SendHeartbeat().Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogException(e, nameof(DoSendHeartbeatSync));
                    }
                }
            }

            #endregion


            #region (private) NextRequestId

            public Request_Id NextRequestId
                => parentNetworkingNode.NextRequestId;

            #endregion


            public static void ShowAllRequests()
            {

                var interfaceType      = typeof(IRequest);
                var implementingTypes  = Assembly.GetAssembly(interfaceType)?.
                                                  GetTypes().
                                                  Where(t => interfaceType.IsAssignableFrom(t) &&
                                                             !t.IsInterface &&
                                                              t.FullName is not null &&
                                                              t.FullName.StartsWith("cloud.charging.open.protocols.OCPPv2_1.CS.")).
                                                  ToArray() ?? [];

                foreach (var type in implementingTypes)
                {

                    var jsonJDContextProp  = type.GetField("DefaultJSONLDContext", BindingFlags.Public | BindingFlags.Static);
                    var jsonJDContextValue = jsonJDContextProp?.GetValue(null)?.ToString();

                    Console.WriteLine($"{type.Name}: JSONJDContext = {jsonJDContextValue}");

                }

            }

            public static void ShowAllResponses()
            {

                var interfaceType      = typeof(IResponse);
                var implementingTypes  = Assembly.GetAssembly(interfaceType)?.
                                                  GetTypes().
                                                  Where(t => interfaceType.IsAssignableFrom(t) &&
                                                             !t.IsInterface &&
                                                              t.FullName is not null &&
                                                              t.FullName.StartsWith("cloud.charging.open.protocols.OCPPv2_1.CSMS.")).
                                                  ToArray() ?? [];

                foreach (var type in implementingTypes)
                {

                    var jsonJDContextProp  = type.GetField("DefaultJSONLDContext", BindingFlags.Public | BindingFlags.Static);
                    var jsonJDContextValue = jsonJDContextProp?.GetValue(null)?.ToString();

                    Console.WriteLine($"{type.Name}: JSONJDContext = {jsonJDContextValue}");

                }

            }


            #region Dispose()

            public void Dispose()
            { }

            #endregion

        }


    }

}
