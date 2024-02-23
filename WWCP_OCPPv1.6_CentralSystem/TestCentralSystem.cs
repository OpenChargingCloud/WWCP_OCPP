/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Collections.Concurrent;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using Newtonsoft.Json.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A central system for testing.
    /// </summary>
    public class TestCentralSystem : ICentralSystemService,
                                     IEventSender
    {

        #region Data

        private          readonly  HashSet<SignaturePolicy>                                                                      signaturePolicies        = [];

        private          readonly  HashSet<OCPPv1_6.CS.ICentralSystemChannel>                                                    centralSystemServers     = [];

        private          readonly  ConcurrentDictionary<NetworkingNode_Id, Tuple<OCPPv1_6.CS.ICentralSystemChannel, DateTime>>   reachableChargeBoxes     = [];

        private          readonly  HTTPExtAPI                                                                                    TestAPI;

        private          readonly  OCPPWebAPI                                                                                    WebAPI;

        protected static readonly  SemaphoreSlim                                                                                 ChargeBoxesSemaphore     = new (1, 1);

        protected static readonly  TimeSpan                                                                                      SemaphoreSlimTimeout     = TimeSpan.FromSeconds(5);

        public    static readonly  IPPort                                                                                        DefaultHTTPUploadPort    = IPPort.Parse(9901);

        private                    Int64                                                                                         internalRequestId        = 900000;

        private                    TimeSpan                                                                                      defaultRequestTimeout    = TimeSpan.FromSeconds(30);

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this central system.
        /// </summary>
        public CentralSystem_Id  CentralSystemId    { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => CentralSystemId.ToString();


        public UploadAPI  HTTPUploadAPI             { get; }

        public IPPort     HTTPUploadPort            { get; }

        public DNSClient  DNSClient                 { get; }

        /// <summary>
        /// Require a HTTP Basic Authentication of all charging boxes.
        /// </summary>
        public Boolean    RequireAuthentication     { get; }

        /// <summary>
        /// The default request timeout for all requests.
        /// </summary>
        public TimeSpan   DefaultRequestTimeout     { get; }


        /// <summary>
        /// An enumeration of central system servers.
        /// </summary>
        public IEnumerable<OCPPv1_6.CS.ICentralSystemChannel> CentralSystemServers
            => centralSystemServers;

        /// <summary>
        /// The unique identifications of all connected or reachable charge boxes.
        /// </summary>
        public IEnumerable<NetworkingNode_Id> ConnectedNetworkingNodeIds
            => reachableChargeBoxes.Values.SelectMany(tuple => tuple.Item1.ConnectedNetworkingNodeIds);


        public Dictionary<String, Transaction_Id> TransactionIds = [];


        /// <summary>
        /// The enumeration of all signature policies.
        /// </summary>
        public IEnumerable<SignaturePolicy>  SignaturePolicies
            => signaturePolicies;

        /// <summary>
        /// The currently active signature policy.
        /// </summary>
        public SignaturePolicy               SignaturePolicy
            => SignaturePolicies.First();


        #endregion

        #region Events

        #region WebSocket connections

        /// <summary>
        /// An event sent whenever the HTTP web socket server started.
        /// </summary>
        public event OnServerStartedDelegate?                         OnServerStarted;

        /// <summary>
        /// An event sent whenever a new TCP connection was accepted.
        /// </summary>
        public event OnValidateTCPConnectionDelegate?                 OnValidateTCPConnection;

        /// <summary>
        /// An event sent whenever a new TCP connection was accepted.
        /// </summary>
        public event OnNewTCPConnectionDelegate?                      OnNewTCPConnection;

        /// <summary>
        /// An event sent whenever a HTTP request was received.
        /// </summary>
        public event HTTPRequestLogDelegate?                          OnHTTPRequest;

        /// <summary>
        /// An event sent whenever the HTTP headers of a new web socket connection
        /// need to be validated or filtered by an upper layer application logic.
        /// </summary>
        public event OnValidateWebSocketConnectionDelegate?           OnValidateWebSocketConnection;

        /// <summary>
        /// An event sent whenever the HTTP connection switched successfully to web socket.
        /// </summary>
        public event OnCSMSNewWebSocketConnectionDelegate?            OnNewWebSocketConnection;

        /// <summary>
        /// An event sent whenever a reponse to a HTTP request was sent.
        /// </summary>
        public event HTTPResponseLogDelegate?                         OnHTTPResponse;

        /// <summary>
        /// An event sent whenever a web socket close frame was received.
        /// </summary>
        public event OnCSMSCloseMessageReceivedDelegate?              OnCloseMessageReceived;

        /// <summary>
        /// An event sent whenever a TCP connection was closed.
        /// </summary>
        public event OnCSMSTCPConnectionClosedDelegate?               OnTCPConnectionClosed;

        /// <summary>
        /// An event sent whenever the HTTP web socket server stopped.
        /// </summary>
        public event OnServerStoppedDelegate?                         OnServerStopped;

        #endregion


        #region Generic JSON Messages

        /// <summary>
        /// An event sent whenever a JSON message request was received.
        /// </summary>
        public event OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a JSON message was sent.
        /// </summary>
        public event OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a JSON message was sent.
        /// </summary>
        public event OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseSent;


        /// <summary>
        /// An event sent whenever a JSON message request was sent.
        /// </summary>
        public event OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a JSON message request was received.
        /// </summary>
        public event OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseReceived;

        /// <summary>
        /// An event sent whenever an error response to a JSON message request was received.
        /// </summary>
        public event OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseReceived;

        #endregion

        #region Generic Binary Messages

        /// <summary>
        /// An event sent whenever a binary message request was received.
        /// </summary>
        public event OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestReceived;

        /// <summary>
        /// An event sent whenever the response to a binary message was sent.
        /// </summary>
        public event OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseSent;

        /// <summary>
        /// An event sent whenever the error response to a binary message was sent.
        /// </summary>
        //public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseSent;


        /// <summary>
        /// An event sent whenever a binary message request was sent.
        /// </summary>
        public event OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestSent;

        /// <summary>
        /// An event sent whenever the response to a binary message request was received.
        /// </summary>
        public event OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseReceived;

        /// <summary>
        /// An event sent whenever the error response to a binary message request was sent.
        /// </summary>
        //public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseReceived;

        #endregion


        #region CSMS <- Charging Station Messages

        // Certificates

        #region OnSignCertificate

        /// <summary>
        /// An event sent whenever a SignCertificate request was received.
        /// </summary>
        public event OnSignCertificateRequestDelegate?   OnSignCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to a SignCertificate request was sent.
        /// </summary>
        public event OnSignCertificateResponseDelegate?  OnSignCertificateResponse;

        #endregion


        // Charging

        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an Authorize request was received.
        /// </summary>
        public event OnAuthorizeRequestDelegate?   OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever a response to an Authorize request was sent.
        /// </summary>
        public event OnAuthorizeResponseDelegate?  OnAuthorizeResponse;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a MeterValues request was received.
        /// </summary>
        public event OnMeterValuesRequestDelegate?   OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a response to a MeterValues request was sent.
        /// </summary>
        public event OnMeterValuesResponseDelegate?  OnMeterValuesResponse;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a StartTransaction request was received.
        /// </summary>
        public event OnStartTransactionRequestDelegate?   OnStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a StartTransaction request was sent.
        /// </summary>
        public event OnStartTransactionResponseDelegate?  OnStartTransactionResponse;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a StatusNotification request was received.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?   OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a StatusNotification request was sent.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?  OnStatusNotificationResponse;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a StopTransaction request was received.
        /// </summary>
        public event OnStopTransactionRequestDelegate?   OnStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a StopTransaction request was sent.
        /// </summary>
        public event OnStopTransactionResponseDelegate?  OnStopTransactionResponse;

        #endregion


        // Firmware

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a BootNotification request was received.
        /// </summary>
        public event OnBootNotificationRequestDelegate?   OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a BootNotification request was sent.
        /// </summary>
        public event OnBootNotificationResponseDelegate?  OnBootNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a FirmwareStatusNotification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a FirmwareStatusNotification request was sent.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?  OnFirmwareStatusNotificationResponse;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a Heartbeat request was received.
        /// </summary>
        public event OnHeartbeatRequestDelegate?   OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a response to a Heartbeat request was sent.
        /// </summary>
        public event OnHeartbeatResponseDelegate?  OnHeartbeatResponse;

        #endregion

        #region OnSignedFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a SignedFirmwareStatusNotification request was received.
        /// </summary>
        public event OnSignedFirmwareStatusNotificationRequestDelegate?   OnSignedFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a SignedFirmwareStatusNotification request was sent.
        /// </summary>
        public event OnSignedFirmwareStatusNotificationResponseDelegate?  OnSignedFirmwareStatusNotificationResponse;

        #endregion


        // Monitoring

        #region OnDiagnosticsStatusNotification

        /// <summary>
        /// An event sent whenever a DiagnosticsStatusNotification request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationRequestDelegate?   OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a DiagnosticsStatusNotification request was sent.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseDelegate?  OnDiagnosticsStatusNotificationResponse;

        #endregion

        #region OnLogStatusNotification

        /// <summary>
        /// An event sent whenever a LogStatusNotification request was received.
        /// </summary>
        public event OnLogStatusNotificationRequestDelegate?   OnLogStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a LogStatusNotification request was sent.
        /// </summary>
        public event OnLogStatusNotificationResponseDelegate?  OnLogStatusNotificationResponse;

        #endregion

        #region OnSecurityEventNotification

        /// <summary>
        /// An event sent whenever a SecurityEventNotification request was received.
        /// </summary>
        public event OnSecurityEventNotificationRequestDelegate?   OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a SecurityEventNotification request was sent.
        /// </summary>
        public event OnSecurityEventNotificationResponseDelegate?  OnSecurityEventNotificationResponse;

        #endregion


        #region OnIncomingDataTransfer

        /// <summary>
        /// An event sent whenever an IncomingDataTransfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate?   OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to an IncomingDataTransfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate?  OnIncomingDataTransferResponse;

        #endregion



        // Binary Data Streams Extensions

        #region OnIncomingBinaryDataTransfer (Request/-Response)

        /// <summary>
        /// An event sent whenever an IncomingBinaryDataTransfer request was received.
        /// </summary>
        public event OnBinaryDataTransferRequestReceivedDelegate?   OnBinaryDataTransferRequestReceived;

        /// <summary>
        /// An event sent whenever a response to an IncomingBinaryDataTransfer request was sent.
        /// </summary>
        public event OnBinaryDataTransferResponseSentDelegate?  OnBinaryDataTransferResponseSent;

        #endregion

        #endregion

        #region CSMS -> Charging Station Messages

        // Certificates

        #region OnCertificateSigned

        /// <summary>
        /// An event sent whenever a CertificateSigned request was sent.
        /// </summary>
        public event CS.OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

        /// <summary>
        /// An event sent whenever a response to a CertificateSigned request was sent.
        /// </summary>
        public event CS.OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

        #endregion

        #region OnDeleteCertificate

        /// <summary>
        /// An event sent whenever a DeleteCertificate request was sent.
        /// </summary>
        public event CS.OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to a DeleteCertificate request was sent.
        /// </summary>
        public event CS.OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

        #endregion

        #region OnGetInstalledCertificateIds

        /// <summary>
        /// An event sent whenever a GetInstalledCertificateIds request was sent.
        /// </summary>
        public event CS.OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event sent whenever a response to a GetInstalledCertificateIds request was sent.
        /// </summary>
        public event CS.OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

        #endregion

        #region OnInstallCertificate

        /// <summary>
        /// An event sent whenever an InstallCertificate request was sent.
        /// </summary>
        public event CS.OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

        /// <summary>
        /// An event sent whenever a response to an InstallCertificate request was sent.
        /// </summary>
        public event CS.OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

        #endregion


        // Charging

        #region OnCancelReservation

        /// <summary>
        /// An event sent whenever a CancelReservation request was sent.
        /// </summary>
        public event CS.OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a response to a CancelReservation request was sent.
        /// </summary>
        public event CS.OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        #endregion

        #region OnClearChargingProfile

        /// <summary>
        /// An event sent whenever a ClearChargingProfile request was sent.
        /// </summary>
        public event CS.OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a ClearChargingProfile request was sent.
        /// </summary>
        public event CS.OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        #endregion

        #region OnGetCompositeSchedule

        /// <summary>
        /// An event sent whenever a GetCompositeSchedule request was sent.
        /// </summary>
        public event CS.OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a GetCompositeSchedule request was sent.
        /// </summary>
        public event CS.OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        #endregion

        #region OnRemoteStartTransaction

        /// <summary>
        /// An event sent whenever a RemoteStartTransaction request was sent.
        /// </summary>
        public event CS.OnRemoteStartTransactionRequestDelegate?   OnRemoteStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a RemoteStartTransaction request was sent.
        /// </summary>
        public event CS.OnRemoteStartTransactionResponseDelegate?  OnRemoteStartTransactionResponse;

        #endregion

        #region OnRemoteStopTransaction

        /// <summary>
        /// An event sent whenever a RemoteStopTransaction request was sent.
        /// </summary>
        public event CS.OnRemoteStopTransactionRequestDelegate?   OnRemoteStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a RemoteStopTransaction request was sent.
        /// </summary>
        public event CS.OnRemoteStopTransactionResponseDelegate?  OnRemoteStopTransactionResponse;

        #endregion

        #region OnReserveNow

        /// <summary>
        /// An event sent whenever a ReserveNow request was sent.
        /// </summary>
        public event CS.OnReserveNowRequestDelegate?   OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a response to a ReserveNow request was sent.
        /// </summary>
        public event CS.OnReserveNowResponseDelegate?  OnReserveNowResponse;

        #endregion

        #region OnSetChargingProfile

        /// <summary>
        /// An event sent whenever a SetChargingProfile request was sent.
        /// </summary>
        public event CS.OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a SetChargingProfile request was sent.
        /// </summary>
        public event CS.OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        #endregion

        #region OnUnlockConnector

        /// <summary>
        /// An event sent whenever an UnlockConnector request was sent.
        /// </summary>
        public event CS.OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever a response to an UnlockConnector request was sent.
        /// </summary>
        public event CS.OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        #endregion


        // Firmware

        #region OnReset

        /// <summary>
        /// An event sent whenever a Reset request was sent.
        /// </summary>
        public event CS.OnResetRequestDelegate?   OnResetRequest;

        /// <summary>
        /// An event sent whenever a response to a Reset request was sent.
        /// </summary>
        public event CS.OnResetResponseDelegate?  OnResetResponse;

        #endregion

        #region OnSignedUpdateFirmware

        /// <summary>
        /// An event sent whenever a SignedUpdateFirmware request was sent.
        /// </summary>
        public event CS.OnSignedUpdateFirmwareRequestDelegate?   OnSignedUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to a SignedUpdateFirmware request was sent.
        /// </summary>
        public event CS.OnSignedUpdateFirmwareResponseDelegate?  OnSignedUpdateFirmwareResponse;

        #endregion

        #region OnUpdateFirmware

        /// <summary>
        /// An event sent whenever an UpdateFirmware request was sent.
        /// </summary>
        public event CS.OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to an UpdateFirmware request was sent.
        /// </summary>
        public event CS.OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        #endregion


        // LocalList

        #region OnClearCache

        /// <summary>
        /// An event sent whenever a ClearCache request was sent.
        /// </summary>
        public event CS.OnClearCacheRequestDelegate?   OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a response to a ClearCache request was sent.
        /// </summary>
        public event CS.OnClearCacheResponseDelegate?  OnClearCacheResponse;

        #endregion

        #region OnGetLocalListVersion

        /// <summary>
        /// An event sent whenever a GetLocalListVersion request was sent.
        /// </summary>
        public event CS.OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a response to a GetLocalListVersion request was sent.
        /// </summary>
        public event CS.OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        #endregion

        #region OnSendLocalList

        /// <summary>
        /// An event sent whenever a SendLocalList request was sent.
        /// </summary>
        public event CS.OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a response to a SendLocalList request was sent.
        /// </summary>
        public event CS.OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        #endregion


        // Monitoring

        #region OnChangeAvailability

        /// <summary>
        /// An event sent whenever a ChangeAvailability request was sent.
        /// </summary>
        public event CS.OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a response to a ChangeAvailability request was sent.
        /// </summary>
        public event CS.OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        #endregion

        #region OnChangeConfiguration

        /// <summary>
        /// An event sent whenever a ChangeConfiguration request was sent.
        /// </summary>
        public event CS.OnChangeConfigurationRequestDelegate?   OnChangeConfigurationRequest;

        /// <summary>
        /// An event sent whenever a response to a ChangeConfiguration request was sent.
        /// </summary>
        public event CS.OnChangeConfigurationResponseDelegate?  OnChangeConfigurationResponse;

        #endregion

        #region OnExtendedTriggerMessage

        /// <summary>
        /// An event sent whenever an ExtendedTriggerMessage request was sent.
        /// </summary>
        public event CS.OnExtendedTriggerMessageRequestDelegate?   OnExtendedTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a response to an ExtendedTriggerMessage request was sent.
        /// </summary>
        public event CS.OnExtendedTriggerMessageResponseDelegate?  OnExtendedTriggerMessageResponse;

        #endregion

        #region OnGetConfiguration

        /// <summary>
        /// An event sent whenever a GetConfiguration request was sent.
        /// </summary>
        public event CS.OnGetConfigurationRequestDelegate?   OnGetConfigurationRequest;

        /// <summary>
        /// An event sent whenever a response to a GetConfiguration request was sent.
        /// </summary>
        public event CS.OnGetConfigurationResponseDelegate?  OnGetConfigurationResponse;

        #endregion

        #region OnGetDiagnostics

        /// <summary>
        /// An event sent whenever a GetDiagnostics request was sent.
        /// </summary>
        public event CS.OnGetDiagnosticsRequestDelegate?   OnGetDiagnosticsRequest;

        /// <summary>
        /// An event sent whenever a response to a GetDiagnostics request was sent.
        /// </summary>
        public event CS.OnGetDiagnosticsResponseDelegate?  OnGetDiagnosticsResponse;

        #endregion

        #region OnGetLog

        /// <summary>
        /// An event sent whenever a GetLog request was sent.
        /// </summary>
        public event CS.OnGetLogRequestDelegate?   OnGetLogRequest;

        /// <summary>
        /// An event sent whenever a response to a GetLog request was sent.
        /// </summary>
        public event CS.OnGetLogResponseDelegate?  OnGetLogResponse;

        #endregion

        #region OnTriggerMessage

        /// <summary>
        /// An event sent whenever a TriggerMessage request was sent.
        /// </summary>
        public event CS.OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a TriggerMessage request was sent.
        /// </summary>
        public event CS.OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        #endregion


        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a reset request was sent.
        /// </summary>
        public event OnDataTransferRequestDelegate?   OnDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnDataTransferResponseDelegate?  OnDataTransferResponse;

        #endregion


        // Binary Data Streams Extensions

        #region OnBinaryDataTransfer          (Request/-Response)

        /// <summary>
        /// An event sent whenever a BinaryDataTransfer request will be sent to the charging station.
        /// </summary>
        public event OnBinaryDataTransferRequestSentDelegate?       OnBinaryDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a BinaryDataTransfer request was received.
        /// </summary>
        public event OnBinaryDataTransferResponseReceivedDelegate?  OnBinaryDataTransferResponse;

        #endregion

        #region OnGetFile                     (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetFile request will be sent to the charging station.
        /// </summary>
        public event OnGetFileRequestDelegate?   OnGetFileRequest;

        /// <summary>
        /// An event sent whenever a response to a GetFile request was received.
        /// </summary>
        public event OnGetFileResponseDelegate?  OnGetFileResponse;

        #endregion

        #region OnSendFile                    (Request/-Response)

        /// <summary>
        /// An event sent whenever a SendFile request will be sent to the charging station.
        /// </summary>
        public event OnSendFileRequestDelegate?   OnSendFileRequest;

        /// <summary>
        /// An event sent whenever a response to a SendFile request was received.
        /// </summary>
        public event OnSendFileResponseDelegate?  OnSendFileResponse;

        #endregion

        #region OnDeleteFile                  (Request/-Response)

        /// <summary>
        /// An event sent whenever a DeleteFile request will be sent to the charging station.
        /// </summary>
        public event OnDeleteFileRequestDelegate?   OnDeleteFileRequest;

        /// <summary>
        /// An event sent whenever a response to a DeleteFile request was received.
        /// </summary>
        public event OnDeleteFileResponseDelegate?  OnDeleteFileResponse;

        #endregion

        #region OnListDirectory               (Request/-Response)

        /// <summary>
        /// An event sent whenever a ListDirectory request will be sent to the charging station.
        /// </summary>
        public event OnListDirectoryRequestDelegate?   OnListDirectoryRequest;

        /// <summary>
        /// An event sent whenever a response to a ListDirectory request was received.
        /// </summary>
        public event OnListDirectoryResponseDelegate?  OnListDirectoryResponse;

        #endregion


        // E2E Security Extensions

        #region AddSignaturePolicy            (Request/-Response)

        /// <summary>
        /// An event fired whenever a AddSignaturePolicy request will be sent to the charging station.
        /// </summary>
        public event OnAddSignaturePolicyRequestDelegate?   OnAddSignaturePolicyRequest;

        /// <summary>
        /// An event fired whenever a response to a AddSignaturePolicy request was received.
        /// </summary>
        public event OnAddSignaturePolicyResponseDelegate?  OnAddSignaturePolicyResponse;

        #endregion

        #region UpdateSignaturePolicy         (Request/-Response)

        /// <summary>
        /// An event fired whenever a UpdateSignaturePolicy request will be sent to the charging station.
        /// </summary>
        public event OnUpdateSignaturePolicyRequestDelegate?   OnUpdateSignaturePolicyRequest;

        /// <summary>
        /// An event fired whenever a response to a UpdateSignaturePolicy request was received.
        /// </summary>
        public event OnUpdateSignaturePolicyResponseDelegate?  OnUpdateSignaturePolicyResponse;

        #endregion

        #region DeleteSignaturePolicy         (Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteSignaturePolicy request will be sent to the charging station.
        /// </summary>
        public event OnDeleteSignaturePolicyRequestDelegate?   OnDeleteSignaturePolicyRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteSignaturePolicy request was received.
        /// </summary>
        public event OnDeleteSignaturePolicyResponseDelegate?  OnDeleteSignaturePolicyResponse;

        #endregion

        #region AddUserRole                   (Request/-Response)

        /// <summary>
        /// An event fired whenever a AddUserRole request will be sent to the charging station.
        /// </summary>
        public event OnAddUserRoleRequestDelegate?   OnAddUserRoleRequest;

        /// <summary>
        /// An event fired whenever a response to a AddUserRole request was received.
        /// </summary>
        public event OnAddUserRoleResponseDelegate?  OnAddUserRoleResponse;

        #endregion

        #region UpdateUserRole                (Request/-Response)

        /// <summary>
        /// An event fired whenever a UpdateUserRole request will be sent to the charging station.
        /// </summary>
        public event OnUpdateUserRoleRequestDelegate?   OnUpdateUserRoleRequest;

        /// <summary>
        /// An event fired whenever a response to a UpdateUserRole request was received.
        /// </summary>
        public event OnUpdateUserRoleResponseDelegate?  OnUpdateUserRoleResponse;

        #endregion

        #region DeleteUserRole                (Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteUserRole request will be sent to the charging station.
        /// </summary>
        public event OnDeleteUserRoleRequestDelegate?   OnDeleteUserRoleRequest;

        /// <summary>
        /// An event fired whenever a response to a DeleteUserRole request was received.
        /// </summary>
        public event OnDeleteUserRoleResponseDelegate?  OnDeleteUserRoleResponse;

        #endregion


        #region OnSecureDataTransfer          (Request/-Response)

        /// <summary>
        /// An event sent whenever a SecureDataTransfer request will be sent to the charging station.
        /// </summary>
        public event OnSecureDataTransferRequestSentDelegate?       OnSecureDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a SecureDataTransfer request was received.
        /// </summary>
        public event OnSecureDataTransferResponseReceivedDelegate?  OnSecureDataTransferResponse;

        #endregion

        #endregion

        #endregion

        #region Custom JSON serializer delegates

        CustomJObjectSerializerDelegate<DataTransferResponse>? CustomIncomingDataTransferResponseSerializer { get; set; }



        #region Messages CentralSystem <- Charge Box

        public CustomJObjectSerializerDelegate<ResetRequest>?                           CustomResetRequestSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<CP.ResetResponse>?                       CustomResetResponseSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<ChangeAvailabilityRequest>?              CustomChangeAvailabilityRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CP.ChangeAvailabilityResponse>?          CustomChangeAvailabilityResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<GetConfigurationRequest>?                CustomGetConfigurationRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CP.GetConfigurationResponse>?            CustomGetConfigurationResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<ChangeConfigurationRequest>?             CustomChangeConfigurationRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CP.ChangeConfigurationResponse>?         CustomChangeConfigurationResponseSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<CS.DataTransferRequest>?                 CustomDataTransferRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CP.DataTransferResponse>?                CustomDataTransferResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<GetDiagnosticsRequest>?                  CustomGetDiagnosticsRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CP.GetDiagnosticsResponse>?              CustomGetDiagnosticsResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<TriggerMessageRequest>?                  CustomTriggerMessageRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CP.TriggerMessageResponse>?              CustomTriggerMessageResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<UpdateFirmwareRequest>?                  CustomUpdateFirmwareRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CP.UpdateFirmwareResponse>?              CustomUpdateFirmwareResponseSerializer                  { get; set; }


        public CustomJObjectSerializerDelegate<ReserveNowRequest>?                      CustomReserveNowRequestSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CP.ReserveNowResponse>?                  CustomReserveNowResponseSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<CancelReservationRequest>?               CustomCancelReservationRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CP.CancelReservationResponse>?           CustomCancelReservationResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<RemoteStartTransactionRequest>?          CustomRemoteStartTransactionRequestSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<CP.RemoteStartTransactionResponse>?      CustomRemoteStartTransactionResponseSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<RemoteStopTransactionRequest>?           CustomRemoteStopTransactionRequestSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<CP.RemoteStopTransactionResponse>?       CustomRemoteStopTransactionResponseSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<SetChargingProfileRequest>?              CustomSetChargingProfileRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CP.SetChargingProfileResponse>?          CustomSetChargingProfileResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<ClearChargingProfileRequest>?            CustomClearChargingProfileRequestSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<CP.ClearChargingProfileResponse>?        CustomClearChargingProfileResponseSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<GetCompositeScheduleRequest>?            CustomGetCompositeScheduleRequestSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<CP.GetCompositeScheduleResponse>?        CustomGetCompositeScheduleResponseSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<UnlockConnectorRequest>?                 CustomUnlockConnectorRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CP.UnlockConnectorResponse>?             CustomUnlockConnectorResponseSerializer                 { get; set; }

        public CustomJObjectSerializerDelegate<GetLocalListVersionRequest>?             CustomGetLocalListVersionRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CP.GetLocalListVersionResponse>?         CustomGetLocalListVersionResponseSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<SendLocalListRequest>?                   CustomSendLocalListRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CP.SendLocalListResponse>?               CustomSendLocalListResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<ClearCacheRequest>?                      CustomClearCacheRequestSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CP.ClearCacheResponse>?                  CustomClearCacheResponseSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<CertificateSignedRequest>?               CustomCertificateSignedRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CP.CertificateSignedResponse>?           CustomCertificateSignedResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<DeleteCertificateRequest>?               CustomDeleteCertificateRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CP.DeleteCertificateResponse>?           CustomDeleteCertificateResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<ExtendedTriggerMessageRequest>?          CustomExtendedTriggerMessageRequestSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<CP.ExtendedTriggerMessageResponse>?      CustomExtendedTriggerMessageResponseSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<GetInstalledCertificateIdsRequest>?      CustomGetInstalledCertificateIdsRequestSerializer       { get; set; }
        public CustomJObjectSerializerDelegate<CP.GetInstalledCertificateIdsResponse>?  CustomGetInstalledCertificateIdsResponseSerializer      { get; set; }
        public CustomJObjectSerializerDelegate<GetLogRequest>?                          CustomGetLogRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CP.GetLogResponse>?                      CustomGetLogResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<InstallCertificateRequest>?              CustomInstallCertificateRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CP.InstallCertificateResponse>?          CustomInstallCertificateResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<SignedUpdateFirmwareRequest>?            CustomSignedUpdateFirmwareRequestSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<CP.SignedUpdateFirmwareResponse>?        CustomSignedUpdateFirmwareResponseSerializer            { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate <BinaryDataTransferResponse>?             CustomIncomingBinaryDataTransferResponseSerializer      { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate <BinaryDataTransferRequest>?              CustomBinaryDataTransferRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<GetFileRequest>?                         CustomGetFileRequestSerializer                          { get; set; }
        public CustomBinarySerializerDelegate <SendFileRequest>?                        CustomSendFileRequestSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<DeleteFileRequest>?                      CustomDeleteFileRequestSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<ListDirectoryRequest>?                   CustomListDirectoryRequestSerializer                    { get; set; }


        // E2E Security Extensions
        public CustomJObjectSerializerDelegate<AddSignaturePolicyRequest>?              CustomAddSignaturePolicyRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<UpdateSignaturePolicyRequest>?           CustomUpdateSignaturePolicyRequestSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<DeleteSignaturePolicyRequest>?           CustomDeleteSignaturePolicyRequestSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<AddUserRoleRequest>?                     CustomAddUserRoleRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<UpdateUserRoleRequest>?                  CustomUpdateUserRoleRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<DeleteUserRoleRequest>?                  CustomDeleteUserRoleRequestSerializer                   { get; set; }

        public CustomBinarySerializerDelegate <SecureDataTransferRequest>?              CustomSecureDataTransferRequestSerializer               { get; set; }

        #endregion


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate <BinaryDataTransferResponse>?             CustomBinaryDataTransferResponseSerializer              { get; set; }
        public CustomBinarySerializerDelegate <OCPP.CS.GetFileResponse>?                CustomGetFileResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<OCPP.CS.SendFileResponse>?               CustomSendFileResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<OCPP.CS.DeleteFileResponse>?             CustomDeleteFileResponseSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<OCPP.CS.ListDirectoryResponse>?          CustomListDirectoryResponseSerializer                   { get; set; }


        // E2E Security Extensions
        public CustomBinarySerializerDelegate <SecureDataTransferResponse>?             CustomSecureDataTransferResponseSerializer              { get; set; }



        #region Charging Station Request  Messages

        public CustomJObjectSerializerDelegate<CP.BootNotificationRequest>?                          CustomBootNotificationRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<CP.HeartbeatRequest>?                                 CustomHeartbeatRequestSerializer                             { get; set; }

        public CustomJObjectSerializerDelegate<CP.AuthorizeRequest>?                                 CustomAuthorizeRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<CP.StartTransactionRequest>?                          CustomStartTransactionRequestRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CP.StatusNotificationRequest>?                        CustomStatusNotificationRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CP.MeterValuesRequest>?                               CustomMeterValuesRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CP.StopTransactionRequest>?                           CustomStopTransactionRequestRequestSerializer                { get; set; }

        public CustomJObjectSerializerDelegate<CP.DataTransferRequest>?                              CustomIncomingDataTransferRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CP.DiagnosticsStatusNotificationRequest>?             CustomDiagnosticsStatusNotificationRequestSerializer         { get; set; }
        public CustomJObjectSerializerDelegate<CP.FirmwareStatusNotificationRequest>?                CustomFirmwareStatusNotificationRequestSerializer            { get; set; }


        // Security extensions
        public CustomJObjectSerializerDelegate<CP.SecurityEventNotificationRequest>?                 CustomSecurityEventNotificationRequestSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<CP.LogStatusNotificationRequest>?                     CustomLogStatusNotificationRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CP.SignCertificateRequest>?                           CustomSignCertificateRequestSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CP.SignedFirmwareStatusNotificationRequest>?          CustomSignedFirmwareStatusNotificationRequestSerializer      { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate <BinaryDataTransferRequest>?                           CustomIncomingBinaryDataTransferRequestSerializer            { get; set; }

        #endregion


        public CustomJObjectSerializerDelegate<StatusInfo>?                             CustomStatusInfoSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<ConfigurationKey>?                       CustomConfigurationKeySerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ChargingProfile>?                        CustomChargingProfileSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?                       CustomChargingScheduleSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                 CustomChargingSchedulePeriodSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizationData>?                      CustomAuthorizationDataSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<IdTagInfo>?                              CustomIdTagInfoSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<CertificateHashData>?                    CustomCertificateHashDataSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<LogParameters>?                          CustomLogParametersSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<FirmwareImage>?                          CustomFirmwareImageSerializer                           { get; set; }


        public CustomJObjectSerializerDelegate<OCPP.Signature>?                         CustomSignatureSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<CustomData>?                             CustomCustomDataSerializer                              { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate<OCPP.Signature>?                          CustomBinarySignatureSerializer                         { get; set; }


        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new central system for testing.
        /// </summary>
        /// <param name="CentralSystemId">The unique identification of this central system.</param>
        /// <param name="RequireAuthentication">Require a HTTP Basic Authentication of all charging boxes.</param>
        public TestCentralSystem(CentralSystem_Id  CentralSystemId,
                                 Boolean           RequireAuthentication   = true,
                                 TimeSpan?         DefaultRequestTimeout   = null,
                                 IPPort?           HTTPUploadPort          = null,
                                 DNSClient?        DNSClient               = null,

                                 SignaturePolicy?  SignaturePolicy         = null)
        {

            if (CentralSystemId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(CentralSystemId), "The given central system identification must not be null or empty!");

            this.CentralSystemId        = CentralSystemId;
            this.RequireAuthentication  = RequireAuthentication;
            this.DefaultRequestTimeout  = DefaultRequestTimeout ?? defaultRequestTimeout;
            this.HTTPUploadPort         = HTTPUploadPort        ?? DefaultHTTPUploadPort;

            Directory.CreateDirectory("HTTPSSEs");

            this.TestAPI                = new HTTPExtAPI(
                                              HTTPServerPort:        IPPort.Parse(3500),
                                              HTTPServerName:        "GraphDefined OCPP v1.6 Test Central System",
                                              HTTPServiceName:       "GraphDefined OCPP v1.6 Test Central System Service",
                                              APIRobotEMailAddress:  EMailAddress.Parse("GraphDefined OCPP Test Central System Robot <robot@charging.cloud>"),
                                              SMTPClient:            new NullMailer(),
                                              DNSClient:             DNSClient,
                                              AutoStart:             true
                                          );

            this.TestAPI.HTTPServer.AddAuth(request => {

                #region Allow some URLs for anonymous access...

                if (request.Path.StartsWith(TestAPI.URLPathPrefix + "/webapi"))
                {
                    return HTTPExtAPI.Anonymous;
                }

                #endregion

                return null;

            });


            this.HTTPUploadAPI           = new UploadAPI(
                                               this,
                                               new HTTPServer(
                                                   this.HTTPUploadPort,
                                                   "Open Charging Cloud OCPP Upload Server",
                                                   "Open Charging Cloud OCPP Upload Service"
                                               )
                                           );

            this.WebAPI                  = new OCPPWebAPI(
                                               this,
                                               TestAPI.HTTPServer
                                           );

            this.DNSClient               = DNSClient ?? new DNSClient(SearchForIPv6DNSServers: false);

            this.signaturePolicies.Add(SignaturePolicy ?? new SignaturePolicy());

        }

        #endregion


        #region AttachSOAPService(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/SOAP.
        /// </summary>
        /// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="ServiceName">The TCP service name shown e.g. on service startup.</param>
        /// <param name="URLPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="ContentType">An optional HTTP content type to use.</param>
        /// <param name="RegisterHTTPRootService">Register HTTP root services for sending a notice to clients connecting via HTML or plain text.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CentralSystemSOAPServer AttachSOAPService(String            HTTPServerName            = CentralSystemSOAPServer.DefaultHTTPServerName,
                                                         IPPort?           TCPPort                   = null,
                                                         String?           ServiceName               = null,
                                                         HTTPPath?         URLPrefix                 = null,
                                                         HTTPContentType?  ContentType               = null,
                                                         Boolean           RegisterHTTPRootService   = true,
                                                         DNSClient?        DNSClient                 = null,
                                                         Boolean           AutoStart                 = false)
        {

            var centralSystemServer = new CentralSystemSOAPServer(
                                          HTTPServerName,
                                          TCPPort,
                                          ServiceName,
                                          URLPrefix,
                                          ContentType,
                                          RegisterHTTPRootService,
                                          DNSClient ?? this.DNSClient,
                                          false
                                      );

            //Attach(centralSystemServer);

            if (AutoStart)
                centralSystemServer.Start();

            return centralSystemServer;

        }

        #endregion

        #region AttachWebSocketService(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/WebSocket.
        /// </summary>
        /// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CentralSystemWSServer AttachWebSocketService(String       HTTPServerName               = CentralSystemWSServer.DefaultHTTPServiceName,
                                                            IIPAddress?  IPAddress                    = null,
                                                            IPPort?      TCPPort                      = null,

                                                            Boolean      DisableWebSocketPings        = false,
                                                            TimeSpan?    WebSocketPingEvery           = null,
                                                            TimeSpan?    SlowNetworkSimulationDelay   = null,

                                                            DNSClient?   DNSClient                    = null,
                                                            Boolean      AutoStart                    = false)
        {

            var centralSystemServer = new CentralSystemWSServer(

                                          NetworkingNode_Id.Parse(CentralSystemId.ToString()),

                                          HTTPServerName,
                                          IPAddress,
                                          TCPPort,

                                          RequireAuthentication,
                                          DisableWebSocketPings,
                                          WebSocketPingEvery,
                                          SlowNetworkSimulationDelay,

                                          DNSClient: DNSClient ?? this.DNSClient,
                                          AutoStart: false

                                      );

            #region WebSocket related

            #region OnServerStarted

            centralSystemServer.OnServerStarted += async (timestamp,
                                                          webSocketServer,
                                                          eventTrackingId,
                                                          cancellationToken) => {

                var onServerStarted = OnServerStarted;
                if (onServerStarted is not null)
                {
                    try
                    {

                        await Task.WhenAll(onServerStarted.GetInvocationList().
                                               OfType <OnServerStartedDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              webSocketServer,
                                                                              eventTrackingId,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCentralSystem),
                                  nameof(OnServerStarted),
                                  e
                              );
                    }
                }

            };

            #endregion

            #region OnNewTCPConnection

            centralSystemServer.OnNewTCPConnection += async (timestamp,
                                                             webSocketServer,
                                                             newTCPConnection,
                                                             eventTrackingId,
                                                             cancellationToken) => {

                var onNewTCPConnection = OnNewTCPConnection;
                if (onNewTCPConnection is not null)
                {
                    try
                    {

                        await Task.WhenAll(onNewTCPConnection.GetInvocationList().
                                               OfType <OnNewTCPConnectionDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              webSocketServer,
                                                                              newTCPConnection,
                                                                              eventTrackingId,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCentralSystem),
                                  nameof(OnNewTCPConnection),
                                  e
                              );
                    }
                }

            };

            #endregion

            // Failed (Charging Station) Authentication

            #region OnNewWebSocketConnection

            centralSystemServer.OnCSMSNewWebSocketConnection += async (timestamp,
                                                                       csmsChannel,
                                                                       newConnection,
                                                                       networkingNodeId,
                                                                       eventTrackingId,
                                                                       sharedSubprotocols,
                                                                       cancellationToken) => {

                // A new connection from the same networking node/charge box will replace the older one!
                if (!reachableChargeBoxes.TryAdd(networkingNodeId, new Tuple<ICentralSystemChannel, DateTime>(csmsChannel as OCPPv1_6.CS.ICentralSystemChannel, timestamp)))
                     reachableChargeBoxes[networkingNodeId]      = new Tuple<ICentralSystemChannel, DateTime>(csmsChannel as OCPPv1_6.CS.ICentralSystemChannel, timestamp);


                var onNewWebSocketConnection = OnNewWebSocketConnection;
                if (onNewWebSocketConnection is not null)
                {
                    try
                    {

                        await Task.WhenAll(onNewWebSocketConnection.GetInvocationList().
                                               OfType <OnCSMSNewWebSocketConnectionDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              csmsChannel,
                                                                              newConnection,
                                                                              networkingNodeId,
                                                                              eventTrackingId,
                                                                              sharedSubprotocols,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCentralSystem),
                                  nameof(OnNewWebSocketConnection),
                                  e
                              );
                    }
                }

            };

            #endregion

            #region OnCloseMessageReceived

            centralSystemServer.OnCSMSCloseMessageReceived += async (timestamp,
                                                                     csmsChannel,
                                                                     connection,
                                                                     networkingNodeId,
                                                                     eventTrackingId,
                                                                     statusCode,
                                                                     reason,
                                                                     cancellationToken) => {

                var onCloseMessageReceived = OnCloseMessageReceived;
                if (onCloseMessageReceived is not null)
                {
                    try
                    {

                        await Task.WhenAll(onCloseMessageReceived.GetInvocationList().
                                               OfType <OnCSMSCloseMessageReceivedDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              csmsChannel,
                                                                              connection,
                                                                              networkingNodeId,
                                                                              eventTrackingId,
                                                                              statusCode,
                                                                              reason,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCentralSystem),
                                  nameof(OnCloseMessageReceived),
                                  e
                              );
                    }
                }

            };

            #endregion

            #region OnTCPConnectionClosed

            centralSystemServer.OnTCPConnectionClosed += async (timestamp,
                                                                server,
                                                                connection,
                                                                eventTrackingId,
                                                                reason,
                                                                cancellationToken) => {

                var onTCPConnectionClosed = OnTCPConnectionClosed;
                if (onTCPConnectionClosed is not null)
                {
                    try
                    {

                        await Task.WhenAll(onTCPConnectionClosed.GetInvocationList().
                                               OfType <OnTCPConnectionClosedDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              server,
                                                                              connection,
                                                                              eventTrackingId,
                                                                              reason,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCentralSystem),
                                  nameof(OnTCPConnectionClosed),
                                  e
                              );
                    }
                }

            };

            #endregion

            #region OnServerStopped

            centralSystemServer.OnServerStopped += async (timestamp,
                                                          server,
                                                          eventTrackingId,
                                                          reason,
                                                          cancellationToken) => {

                var onServerStopped = OnServerStopped;
                if (onServerStopped is not null)
                {
                    try
                    {

                        await Task.WhenAll(onServerStopped.GetInvocationList().
                                                 OfType <OnServerStoppedDelegate>().
                                                 Select (loggingDelegate => loggingDelegate.Invoke(
                                                                                timestamp,
                                                                                server,
                                                                                eventTrackingId,
                                                                                reason,
                                                                                cancellationToken
                                                                            )).
                                                 ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCentralSystem),
                                  nameof(OnServerStopped),
                                  e
                              );
                    }
                }

            };

            #endregion

            // (Generic) Error Handling

            #endregion


            #region OnJSONMessageRequestReceived

            centralSystemServer.OnJSONMessageRequestReceived += async (timestamp,
                                                                       webSocketServer,
                                                                       webSocketConnection,
                                                                       networkingNodeId,
                                                                       networkPath,
                                                                       eventTrackingId,
                                                                       requestTimestamp,
                                                                       requestMessage,
                                                                       cancellationToken) => {

                var onJSONMessageRequestReceived = OnJSONMessageRequestReceived;
                if (onJSONMessageRequestReceived is not null)
                {
                    try
                    {

                        await Task.WhenAll(onJSONMessageRequestReceived.GetInvocationList().
                                               OfType <OnWebSocketJSONMessageRequestDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              webSocketServer,
                                                                              webSocketConnection,
                                                                              networkingNodeId,
                                                                              networkPath,
                                                                              eventTrackingId,
                                                                              requestTimestamp,
                                                                              requestMessage,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCentralSystem),
                                  nameof(OnJSONMessageRequestReceived),
                                  e
                              );
                    }
                }

            };

            #endregion

            #region OnJSONMessageResponseSent

            centralSystemServer.OnJSONMessageResponseSent += async (timestamp,
                                                                    webSocketServer,
                                                                    webSocketConnection,
                                                                    networkingNodeId,
                                                                    networkPath,
                                                                    eventTrackingId,
                                                                    requestTimestamp,
                                                                    jsonRequestMessage,
                                                                    binaryRequestMessage,
                                                                    responseTimestamp,
                                                                    responseMessage,
                                                                    cancellationToken) => {

                var onJSONMessageResponseSent = OnJSONMessageResponseSent;
                if (onJSONMessageResponseSent is not null)
                {
                    try
                    {

                        await Task.WhenAll(onJSONMessageResponseSent.GetInvocationList().
                                               OfType <OnWebSocketJSONMessageResponseDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              webSocketServer,
                                                                              webSocketConnection,
                                                                              networkingNodeId,
                                                                              networkPath,
                                                                              eventTrackingId,
                                                                              requestTimestamp,
                                                                              jsonRequestMessage,
                                                                              binaryRequestMessage,
                                                                              responseTimestamp,
                                                                              responseMessage,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCentralSystem),
                                  nameof(OnJSONMessageResponseSent),
                                  e
                              );
                    }
                }

            };

            #endregion

            #region OnJSONErrorResponseSent

            centralSystemServer.OnJSONErrorResponseSent += async (timestamp,
                                                                  webSocketServer,
                                                                  webSocketConnection,
                                                                  eventTrackingId,
                                                                  requestTimestamp,
                                                                  jsonRequestMessage,
                                                                  binaryRequestMessage,
                                                                  responseTimestamp,
                                                                  responseMessage,
                                                                  cancellationToken) => {

                var onJSONErrorResponseSent = OnJSONErrorResponseSent;
                if (onJSONErrorResponseSent is not null)
                {
                    try
                    {

                        await Task.WhenAll(onJSONErrorResponseSent.GetInvocationList().
                                               OfType <OnWebSocketTextErrorResponseDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              webSocketServer,
                                                                              webSocketConnection,
                                                                              eventTrackingId,
                                                                              requestTimestamp,
                                                                              jsonRequestMessage,
                                                                              binaryRequestMessage,
                                                                              responseTimestamp,
                                                                              responseMessage,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCentralSystem),
                                  nameof(OnJSONErrorResponseSent),
                                  e
                              );
                    }
                }

            };

            #endregion


            #region OnJSONMessageRequestSent

            centralSystemServer.OnJSONMessageRequestSent += async (timestamp,
                                                                   webSocketServer,
                                                                   webSocketConnection,
                                                                   networkingNodeId,
                                                                   networkPath,
                                                                   eventTrackingId,
                                                                   requestTimestamp,
                                                                   requestMessage,
                                                                   cancellationToken) => {

                var onJSONMessageRequestSent = OnJSONMessageRequestSent;
                if (onJSONMessageRequestSent is not null)
                {
                    try
                    {

                        await Task.WhenAll(onJSONMessageRequestSent.GetInvocationList().
                                               OfType <OnWebSocketJSONMessageRequestDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              webSocketServer,
                                                                              webSocketConnection,
                                                                              networkingNodeId,
                                                                              networkPath,
                                                                              eventTrackingId,
                                                                              requestTimestamp,
                                                                              requestMessage,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCentralSystem),
                                  nameof(OnJSONMessageRequestSent),
                                  e
                              );
                    }
                }

            };

            #endregion

            #region OnJSONMessageResponseReceived

            centralSystemServer.OnJSONMessageResponseReceived += async (timestamp,
                                                                        webSocketServer,
                                                                        webSocketConnection,
                                                                        networkingNodeId,
                                                                        networkPath,
                                                                        eventTrackingId,
                                                                        requestTimestamp,
                                                                        jsonRequestMessage,
                                                                        binaryRequestMessage,
                                                                        responseTimestamp,
                                                                        responseMessage,
                                                                        cancellationToken) => {

                var onJSONMessageResponseReceived = OnJSONMessageResponseReceived;
                if (onJSONMessageResponseReceived is not null)
                {
                    try
                    {

                        await Task.WhenAll(onJSONMessageResponseReceived.GetInvocationList().
                                               OfType <OnWebSocketJSONMessageResponseDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              webSocketServer,
                                                                              webSocketConnection,
                                                                              networkingNodeId,
                                                                              networkPath,
                                                                              eventTrackingId,
                                                                              requestTimestamp,
                                                                              jsonRequestMessage,
                                                                              binaryRequestMessage,
                                                                              responseTimestamp,
                                                                              responseMessage,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCentralSystem),
                                  nameof(OnJSONMessageResponseReceived),
                                  e
                              );
                    }
                }

            };

            #endregion

            #region OnJSONErrorResponseReceived

            centralSystemServer.OnJSONErrorResponseReceived += async (timestamp,
                                                                      webSocketServer,
                                                                      webSocketConnection,
                                                                      eventTrackingId,
                                                                      requestTimestamp,
                                                                      jsonRequestMessage,
                                                                      binaryRequestMessage,
                                                                      responseTimestamp,
                                                                      responseMessage,
                                                                      cancellationToken) => {

                var onJSONErrorResponseReceived = OnJSONErrorResponseReceived;
                if (onJSONErrorResponseReceived is not null)
                {
                    try
                    {

                        await Task.WhenAll(onJSONErrorResponseReceived.GetInvocationList().
                                               OfType <OnWebSocketTextErrorResponseDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              timestamp,
                                                                              webSocketServer,
                                                                              webSocketConnection,
                                                                              eventTrackingId,
                                                                              requestTimestamp,
                                                                              jsonRequestMessage,
                                                                              binaryRequestMessage,
                                                                              responseTimestamp,
                                                                              responseMessage,
                                                                              cancellationToken
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCentralSystem),
                                  nameof(OnJSONErrorResponseReceived),
                                  e
                              );
                    }
                }

            };

            #endregion


            Attach(centralSystemServer);


            if (AutoStart)
                centralSystemServer.Start();

            return centralSystemServer;

        }

        #endregion

        #region Attach(CentralSystemServer)

        public void Attach(ICentralSystemChannel CentralSystemServer)
        {

            centralSystemServers.Add(CentralSystemServer);


            // Wire events...

            #region OnBootNotification

            CentralSystemServer.OnBootNotification += async (LogTimestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             cancellationToken) => {

                #region Send OnBootNotificationRequest event

                var startTime      = Timestamp.Now;

                var requestLogger  = OnBootNotificationRequest;
                if (requestLogger is not null)
                {
                    try
                    {

                        await Task.WhenAll(requestLogger.GetInvocationList().
                                               OfType <OnBootNotificationRequestDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              startTime,
                                                                              this,
                                                                              connection,
                                                                              request
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCentralSystem),
                                  nameof(OnBootNotificationRequest),
                                  e
                              );
                    }

                }

                #endregion


                Console.WriteLine("OnBootNotification: " + request.DestinationNodeId             + ", " +
                                                           request.ChargePointVendor       + ", " +
                                                           request.ChargePointModel        + ", " +
                                                           request.ChargePointSerialNumber + ", " +
                                                           request.ChargeBoxSerialNumber);


                //await AddChargeBoxIfNotExists(new ChargeBox(request.NetworkingNodeId,
                //                                            1,
                //                                            request.ChargePointVendor,
                //                                            request.ChargePointModel,
                //                                            null,
                //                                            request.ChargePointSerialNumber,
                //                                            request.ChargeBoxSerialNumber,
                //                                            request.FirmwareVersion,
                //                                            request.Iccid,
                //                                            request.IMSI,
                //                                            request.MeterType,
                //                                            request.MeterSerialNumber));


                await Task.Delay(100, cancellationToken);


                var response = new BootNotificationResponse(Request:            request,
                                                            Status:             RegistrationStatus.Accepted,
                                                            CurrentTime:        Timestamp.Now,
                                                            HeartbeatInterval:  TimeSpan.FromMinutes(5));


                #region Send OnBootNotificationResponse event

                var responseLogger = OnBootNotificationResponse;
                if (responseLogger is not null)
                {
                    try
                    {

                        var responseTime = Timestamp.Now;

                        await Task.WhenAll(responseLogger.GetInvocationList().
                                               OfType <OnBootNotificationResponseDelegate>().
                                               Select (loggingDelegate => loggingDelegate.Invoke(
                                                                              responseTime,
                                                                              this,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              responseTime - startTime
                                                                          )).
                                               ToArray());

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestCentralSystem),
                                  nameof(OnBootNotificationResponse),
                                  e
                              );
                    }

                }

                #endregion

                return response;

            };

            #endregion

            #region OnHeartbeat

            CentralSystemServer.OnHeartbeat += async (LogTimestamp,
                                                      Sender,
                                                      connection,
                                                      Request,
                                                      CancellationToken) => {

                #region Send OnHeartbeatRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnHeartbeatRequest?.Invoke(startTime,
                                               this,
                                               connection,
                                               Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnHeartbeatRequest));
                }

                #endregion


                Console.WriteLine("OnHeartbeat: " + Request.DestinationNodeId);


                await Task.Delay(100, CancellationToken);


                var response = new HeartbeatResponse(Request:      Request,
                                                     CurrentTime:  Timestamp.Now);


                #region Send OnHeartbeatResponse event

                try
                {

                    OnHeartbeatResponse?.Invoke(Timestamp.Now,
                                                this,
                                                connection,
                                                Request,
                                                response,
                                                Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnHeartbeatResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnDiagnosticsStatusNotification

            CentralSystemServer.OnDiagnosticsStatusNotification += async (LogTimestamp,
                                                                          Sender,
                                                                          connection,
                                                                          Request,
                                                                          CancellationToken) => {

                #region Send OnDiagnosticsStatusNotificationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnDiagnosticsStatusNotificationRequest?.Invoke(startTime,
                                                                   this,
                                                                   connection,
                                                                   Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
                }

                #endregion


                Console.WriteLine("OnDiagnosticsStatusNotification: " + Request.Status);


                await Task.Delay(100, CancellationToken);

                var response = new DiagnosticsStatusNotificationResponse(Request);


                #region Send OnDiagnosticsStatusResponse event

                try
                {

                    OnDiagnosticsStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    connection,
                                                                    Request,
                                                                    response,
                                                                    Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnFirmwareStatusNotification

            CentralSystemServer.OnFirmwareStatusNotification += async (LogTimestamp,
                                                                       Sender,
                                                                       connection,
                                                                       Request,
                                                                       CancellationToken) => {

                #region Send OnFirmwareStatusNotificationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnFirmwareStatusNotificationRequest?.Invoke(startTime,
                                                                this,
                                                                connection,
                                                                Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnFirmwareStatusNotificationRequest));
                }

                #endregion


                Console.WriteLine("OnFirmwareStatus: " + Request.Status);

                await Task.Delay(100, CancellationToken);

                var response = new FirmwareStatusNotificationResponse(Request);


                #region Send OnFirmwareStatusResponse event

                try
                {

                    OnFirmwareStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 connection,
                                                                 Request,
                                                                 response,
                                                                 Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnFirmwareStatusNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion


            #region OnAuthorize

            CentralSystemServer.OnAuthorize += async (LogTimestamp,
                                                      Sender,
                                                      connection,
                                                      Request,
                                                      CancellationToken) => {

                #region Send OnAuthorizeRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnAuthorizeRequest?.Invoke(startTime,
                                               this,
                                               connection,
                                               Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnAuthorizeRequest));
                }

                #endregion


                Console.WriteLine("OnAuthorize: " + Request.DestinationNodeId + ", " +
                                                    Request.IdTag);

                await Task.Delay(100, CancellationToken);

                var response = new AuthorizeResponse(
                                   Request:    Request,
                                   IdTagInfo:  new IdTagInfo(
                                                   Status:      AuthorizationStatus.Accepted,
                                                   ExpiryDate:  Timestamp.Now.AddDays(3)
                                               )
                               );


                #region Send OnAuthorizeResponse event

                try
                {

                    OnAuthorizeResponse?.Invoke(Timestamp.Now,
                                                this,
                                                connection,
                                                Request,
                                                response,
                                                Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnAuthorizeResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnStartTransaction

            CentralSystemServer.OnStartTransaction += async (LogTimestamp,
                                                             Sender,
                                                             connection,
                                                             Request,
                                                             CancellationToken) => {

                #region Send OnStartTransactionRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnStartTransactionRequest?.Invoke(startTime,
                                                      this,
                                                      connection,
                                                      Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStartTransactionRequest));
                }

                #endregion


                Console.WriteLine("OnStartTransaction: " + Request.DestinationNodeId + ", " +
                                                           Request.ConnectorId + ", " +
                                                           Request.IdTag + ", " +
                                                           Request.StartTimestamp + ", " +
                                                           Request.MeterStart + ", " +
                                                           Request.ReservationId ?? "-");

                await Task.Delay(100, CancellationToken);

                var response = new StartTransactionResponse(Request:        Request,
                                                            TransactionId:  Transaction_Id.NewRandom,
                                                            IdTagInfo:      new IdTagInfo(Status:      AuthorizationStatus.Accepted,
                                                                                          ExpiryDate:  Timestamp.Now.AddDays(3)));

                var key = Request.DestinationNodeId + "*" + Request.ConnectorId;

                if (TransactionIds.ContainsKey(key))
                    TransactionIds[key] = response.TransactionId;
                else
                    TransactionIds.Add(key, response.TransactionId);


                #region Send OnStartTransactionResponse event

                try
                {

                    OnStartTransactionResponse?.Invoke(Timestamp.Now,
                                                       this,
                                                       connection,
                                                       Request,
                                                       response,
                                                       Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStartTransactionResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnStatusNotification

            CentralSystemServer.OnStatusNotification += async (LogTimestamp,
                                                               Sender,
                                                               connection,
                                                               Request,
                                                               CancellationToken) => {

                #region Send OnStatusNotificationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnStatusNotificationRequest?.Invoke(startTime,
                                                        this,
                                                        connection,
                                                        Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStatusNotificationRequest));
                }

                #endregion


                Console.WriteLine("OnStatusNotification: " + Request.ConnectorId     + ", " +
                                                             Request.Status          + ", " +
                                                             Request.ErrorCode       + ", " +
                                                             Request.Info            + ", " +
                                                             Request.StatusTimestamp + ", " +
                                                             Request.VendorId        + ", " +
                                                             Request.VendorErrorCode);


                await Task.Delay(100, CancellationToken);

                var response = new StatusNotificationResponse(Request);


                #region Send OnStatusNotificationResponse event

                try
                {

                    OnStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                         this,
                                                         connection,
                                                         Request,
                                                         response,
                                                         Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStatusNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnMeterValues

            CentralSystemServer.OnMeterValues += async (LogTimestamp,
                                                        Sender,
                                                        connection,
                                                        Request,
                                                        CancellationToken) => {

                #region Send OnMeterValuesRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnMeterValuesRequest?.Invoke(startTime,
                                                 this,
                                                 connection,
                                                 Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnMeterValuesRequest));
                }

                                                            #endregion


                Console.WriteLine("OnMeterValues: " + Request.ConnectorId + ", " +
                                                      Request.TransactionId);

                Console.WriteLine(Request.MeterValues.SafeSelect(meterValue => meterValue.Timestamp.ToIso8601() +
                                          meterValue.SampledValues.SafeSelect(sampledValue => sampledValue.Context + ", " + sampledValue.Value + ", " + sampledValue.Value).AggregateWith("; ")).AggregateWith(Environment.NewLine));


                await Task.Delay(100, CancellationToken);

                var response = new MeterValuesResponse(Request);


                #region Send OnMeterValuesResponse event

                try
                {

                    OnMeterValuesResponse?.Invoke(Timestamp.Now,
                                                  this,
                                                  connection,
                                                  Request,
                                                  response,
                                                  Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnMeterValuesResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnStopTransaction

            CentralSystemServer.OnStopTransaction += async (LogTimestamp,
                                                            Sender,
                                                            connection,
                                                            Request,
                                                            CancellationToken) => {

                #region Send OnStopTransactionRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnStopTransactionRequest?.Invoke(startTime,
                                                     this,
                                                     connection,
                                                     Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStopTransactionRequest));
                }

                #endregion


                Console.WriteLine("OnStopTransaction: " + Request.TransactionId + ", " +
                                                          Request.IdTag + ", " +
                                                          Request.StopTimestamp.ToIso8601() + ", " +
                                                          Request.MeterStop + ", " +
                                                          Request.Reason);

                Console.WriteLine(Request.TransactionData.SafeSelect(transactionData => transactionData.Timestamp.ToIso8601() +
                                          transactionData.SampledValues.SafeSelect(sampledValue => sampledValue.Context + ", " + sampledValue.Value + ", " + sampledValue.Value).AggregateWith("; ")).AggregateWith(Environment.NewLine));

                await Task.Delay(100, CancellationToken);

                var response = new StopTransactionResponse(Request:    Request,
                                                           IdTagInfo:  new IdTagInfo(Status:      AuthorizationStatus.Accepted,
                                                                                     ExpiryDate:  Timestamp.Now.AddDays(3)));

                var kvp = TransactionIds.Where(trid => trid.Value == Request.TransactionId).ToArray();
                if (kvp.SafeAny())
                    TransactionIds.Remove(kvp.First().Key);


                #region Send OnStopTransactionResponse event

                try
                {

                    OnStopTransactionResponse?.Invoke(Timestamp.Now,
                                                      this,
                                                      connection,
                                                      Request,
                                                      response,
                                                      Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStopTransactionResponse));
                }

                #endregion

                return response;

            };

            #endregion


            #region OnIncomingDataTransfer

            CentralSystemServer.OnIncomingDataTransfer += async (LogTimestamp,
                                                                 Sender,
                                                                 connection,
                                                                 request,
                                                                 CancellationToken) => {

                #region Send OnIncomingDataRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnIncomingDataTransferRequest?.Invoke(startTime,
                                                          this,
                                                          connection,
                                                          request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnIncomingDataTransferRequest));
                }

                #endregion


                // VendorId
                // MessageId
                // Data

                DebugX.Log("OnIncomingDataTransfer: " + request.VendorId  + ", " +
                                                        request.MessageId + ", " +
                                                        request.Data);


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


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToJSON(
                                       CustomIncomingDataTransferRequestSerializer,
                                       CustomSignatureSerializer,
                                       CustomCustomDataSerializer
                                   ),
                                   out var errorResponse
                               )

                                   ? new DataTransferResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : request.VendorId == Vendor_Id.GraphDefined

                                         ? new (
                                               Request:      request,
                                               Status:       DataTransferStatus.Accepted,
                                               Data:         responseData,
                                               StatusInfo:   null,
                                               CustomData:   null
                                           )

                                         : new DataTransferResponse(
                                               Request:      request,
                                               Status:       DataTransferStatus.Rejected,
                                               Data:         null,
                                               StatusInfo:   null,
                                               CustomData:   null
                                         );

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToJSON(
                        CustomIncomingDataTransferResponseSerializer,
                        null,
                        CustomSignatureSerializer,
                        CustomCustomDataSerializer
                    ),
                    out var errorResponse2);


                #region Send OnIncomingDataResponse event

                try
                {

                    OnIncomingDataTransferResponse?.Invoke(Timestamp.Now,
                                                           this,
                                                           connection,
                                                           request,
                                                           response,
                                                           Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnIncomingDataTransferResponse));
                }

                #endregion

                return response;

            };

            #endregion


            // Security extensions

            #region OnSecurityEventNotification

            CentralSystemServer.OnSecurityEventNotification += async (LogTimestamp,
                                                                       Sender,
                                                                       connection,
                                                                       Request,
                                                                       CancellationToken) => {

                #region Send OnSecurityEventNotificationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnSecurityEventNotificationRequest?.Invoke(startTime,
                                                                this,
                                                                connection,
                                                                Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSecurityEventNotificationRequest));
                }

                #endregion


                Console.WriteLine("OnSecurityEventNotification: " );

                await Task.Delay(100, CancellationToken);

                var response = new SecurityEventNotificationResponse(Request);


                #region Send OnFirmwareStatusResponse event

                try
                {

                    OnSecurityEventNotificationResponse?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 connection,
                                                                 Request,
                                                                 response,
                                                                 Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSecurityEventNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnLogStatusNotification

            CentralSystemServer.OnLogStatusNotification += async (LogTimestamp,
                                                                       Sender,
                                                                       connection,
                                                                       Request,
                                                                       CancellationToken) => {

                #region Send OnLogStatusNotificationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnLogStatusNotificationRequest?.Invoke(startTime,
                                                                this,
                                                                connection,
                                                                Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnLogStatusNotificationRequest));
                }

                #endregion


                Console.WriteLine("OnLogStatusNotification: " + Request.Status);

                await Task.Delay(100, CancellationToken);

                var response = new LogStatusNotificationResponse(Request);


                #region Send OnFirmwareStatusResponse event

                try
                {

                    OnLogStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 connection,
                                                                 Request,
                                                                 response,
                                                                 Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnLogStatusNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnSignCertificate

            CentralSystemServer.OnSignCertificate += async (LogTimestamp,
                                                                       Sender,
                                                                       connection,
                                                                       Request,
                                                                       CancellationToken) => {

                #region Send OnSignCertificateRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnSignCertificateRequest?.Invoke(startTime,
                                                                this,
                                                                connection,
                                                                Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSignCertificateRequest));
                }

                #endregion


                Console.WriteLine("OnSignCertificate: " );

                await Task.Delay(100, CancellationToken);

                var response = new SignCertificateResponse(Request, GenericStatus.Accepted);


                #region Send OnFirmwareStatusResponse event

                try
                {

                    OnSignCertificateResponse?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 connection,
                                                                 Request,
                                                                 response,
                                                                 Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSignCertificateResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnSignedFirmwareStatusNotification

            CentralSystemServer.OnSignedFirmwareStatusNotification += async (LogTimestamp,
                                                                       Sender,
                                                                       connection,
                                                                       Request,
                                                                       CancellationToken) => {

                #region Send OnSignedFirmwareStatusNotificationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnSignedFirmwareStatusNotificationRequest?.Invoke(startTime,
                                                                this,
                                                                connection,
                                                                Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSignedFirmwareStatusNotificationRequest));
                }

                #endregion


                Console.WriteLine("OnSignedFirmwareStatusNotification: " + Request.Status);

                await Task.Delay(100, CancellationToken);

                var response = new SignedFirmwareStatusNotificationResponse(Request);


                #region Send OnFirmwareStatusResponse event

                try
                {

                    OnSignedFirmwareStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 connection,
                                                                 Request,
                                                                 response,
                                                                 Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSignedFirmwareStatusNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion


            // Binary Data Streams Extensions

            #region OnIncomingBinaryDataTransfer

            CentralSystemServer.OnIncomingBinaryDataTransfer += async (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       cancellationToken) => {

                #region Send OnIncomingBinaryDataTransfer event

                var startTime = Timestamp.Now;

                try
                {

                    OnBinaryDataTransferRequestReceived?.Invoke(startTime,
                                                                this,
                                                                connection,
                                                                request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnBinaryDataTransferRequestReceived));
                }

                #endregion


                // VendorId
                // MessageId
                // BinaryData

                DebugX.Log("OnIncomingBinaryDataTransfer: " + request.VendorId  + ", " +
                                                              request.MessageId + ", " +
                                                              request.Data?.ToUTF8String() ?? "-");


                var responseBinaryData = Array.Empty<byte>();

                if (request.Data is not null)
                {
                    responseBinaryData = ((Byte[]) request.Data.Clone()).Reverse();
                }


                var response = !SignaturePolicy.VerifyRequestMessage(
                                   request,
                                   request.ToBinary(
                                       CustomIncomingBinaryDataTransferRequestSerializer,
                                       CustomBinarySignatureSerializer,
                                       IncludeSignatures: false
                                   ),
                                   out var errorResponse
                               )

                                   ? new BinaryDataTransferResponse(
                                         Request:      request,
                                         Result:       Result.SignatureError(
                                                           $"Invalid signature(s): {errorResponse}"
                                                       )
                                     )

                                   : request.VendorId == Vendor_Id.GraphDefined

                                         ? new (
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

                SignaturePolicy.SignResponseMessage(
                    response,
                    response.ToBinary(
                        CustomIncomingBinaryDataTransferResponseSerializer,
                        null, //CustomStatusInfoSerializer,
                        CustomBinarySignatureSerializer,
                        IncludeSignatures: false
                    ),
                    out var errorResponse2);


                #region Send OnIncomingBinaryDataTransferResponse event

                try
                {

                    OnBinaryDataTransferResponseSent?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 connection,
                                                                 request,
                                                                 response,
                                                                 Timestamp.Now - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnBinaryDataTransferResponseSent));
                }

                #endregion

                return response;

            };

            #endregion

        }

        #endregion


        #region AddHTTPBasicAuth(NetworkingNodeId, Password)

        /// <summary>
        /// Add the given HTTP Basic Authentication password for the given charge box.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of the charge box.</param>
        /// <param name="Password">The password of the charge box.</param>
        public void AddHTTPBasicAuth(NetworkingNode_Id  NetworkingNodeId,
                                     String             Password)
        {

            foreach (var centralSystemServer in centralSystemServers)
            {
                if (centralSystemServer is CentralSystemWSServer centralSystemWSServer)
                {

                    //centralSystemWSServer.AddHTTPBasicAuth(NetworkingNodeId,
                    //                                       Password);

                }
            }

        }

        #endregion


        #region ChargeBoxes

        #region Data

        /// <summary>
        /// An enumeration of all charge boxes.
        /// </summary>
        protected internal readonly Dictionary<ChargeBox_Id, ChargeBox> chargeBoxes = [];

        /// <summary>
        /// An enumeration of all charge boxes.
        /// </summary>
        public IEnumerable<ChargeBox> ChargeBoxes
        {
            get
            {

                if (ChargeBoxesSemaphore.Wait(SemaphoreSlimTimeout))
                {
                    try
                    {

                        return chargeBoxes.Values.ToArray();

                    }
                    finally
                    {
                        try
                        {
                            ChargeBoxesSemaphore.Release();
                        }
                        catch
                        { }
                    }
                }

                return new ChargeBox[0];

            }
        }

        #endregion


        #region (protected internal) WriteToDatabaseFileAndNotify(ChargeBox,                      MessageType,    OldChargeBox = null, ...)

        ///// <summary>
        ///// Write the given chargeBox to the database and send out notifications.
        ///// </summary>
        ///// <param name="ChargeBox">The charge box.</param>
        ///// <param name="MessageType">The chargeBox notification.</param>
        ///// <param name="OldChargeBox">The old/updated charge box.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        //protected internal async Task WriteToDatabaseFileAndNotify(ChargeBox             ChargeBox,
        //                                                           NotificationMessageType  MessageType,
        //                                                           ChargeBox             OldChargeBox   = null,
        //                                                           EventTracking_Id         EventTrackingId   = null,
        //                                                           User_Id?                 CurrentUserId     = null)
        //{

        //    if (ChargeBox is null)
        //        throw new ArgumentNullException(nameof(ChargeBox),  "The given chargeBox must not be null or empty!");

        //    if (MessageType.IsNullOrEmpty)
        //        throw new ArgumentNullException(nameof(MessageType),   "The given message type must not be null or empty!");


        //    var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

        //    await WriteToDatabaseFile(MessageType,
        //                              ChargeBox.ToJSON(false, true),
        //                              eventTrackingId,
        //                              CurrentUserId);

        //    await SendNotifications(ChargeBox,
        //                            MessageType,
        //                            OldChargeBox,
        //                            eventTrackingId,
        //                            CurrentUserId);

        //}

        #endregion

        #region (protected internal) SendNotifications           (ChargeBox,                      MessageType(s), OldChargeBox = null, ...)

        //protected virtual String ChargeBoxHTMLInfo(ChargeBox ChargeBox)

        //    => String.Concat(ChargeBox.Name.IsNeitherNullNorEmpty()
        //                         ? String.Concat("<a href=\"https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id, "\">", ChargeBox.Name.FirstText(), "</a> ",
        //                                        "(<a href=\"https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id, "\">", ChargeBox.Id, "</a>)")
        //                         : String.Concat("<a href=\"https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id, "\">", ChargeBox.Id, "</a>"));

        //protected virtual String ChargeBoxTextInfo(ChargeBox ChargeBox)

        //    => String.Concat(ChargeBox.Name.IsNeitherNullNorEmpty()
        //                         ? String.Concat("'", ChargeBox.Name.FirstText(), "' (", ChargeBox.Id, ")")
        //                         : String.Concat("'", ChargeBox.Id.ToString(), "'"));


        ///// <summary>
        ///// Send chargeBox notifications.
        ///// </summary>
        ///// <param name="ChargeBox">The charge box.</param>
        ///// <param name="MessageType">The chargeBox notification.</param>
        ///// <param name="OldChargeBox">The old/updated charge box.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">The invoking chargeBox identification</param>
        //protected internal virtual Task SendNotifications(ChargeBox             ChargeBox,
        //                                                  NotificationMessageType  MessageType,
        //                                                  ChargeBox             OldChargeBox   = null,
        //                                                  EventTracking_Id         EventTrackingId   = null,
        //                                                  User_Id?                 CurrentUserId     = null)

        //    => SendNotifications(ChargeBox,
        //                         new NotificationMessageType[] { MessageType },
        //                         OldChargeBox,
        //                         EventTrackingId,
        //                         CurrentUserId);


        ///// <summary>
        ///// Send chargeBox notifications.
        ///// </summary>
        ///// <param name="ChargeBox">The charge box.</param>
        ///// <param name="MessageTypes">The chargeBox notifications.</param>
        ///// <param name="OldChargeBox">The old/updated charge box.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">The invoking chargeBox identification</param>
        //protected internal async virtual Task SendNotifications(ChargeBox                          ChargeBox,
        //                                                        IEnumerable<NotificationMessageType>  MessageTypes,
        //                                                        ChargeBox                          OldChargeBox   = null,
        //                                                        EventTracking_Id                      EventTrackingId   = null,
        //                                                        User_Id?                              CurrentUserId     = null)
        //{

        //    if (ChargeBox is null)
        //        throw new ArgumentNullException(nameof(ChargeBox),  "The given chargeBox must not be null or empty!");

        //    var messageTypesHash = new HashSet<NotificationMessageType>(MessageTypes.Where(messageType => !messageType.IsNullOrEmpty));

        //    if (messageTypesHash.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(MessageTypes),  "The given enumeration of message types must not be null or empty!");

        //    if (messageTypesHash.Contains(addChargeBoxIfNotExists_MessageType))
        //        messageTypesHash.Add(addChargeBox_MessageType);

        //    if (messageTypesHash.Contains(addOrUpdateChargeBox_MessageType))
        //        messageTypesHash.Add(OldChargeBox == null
        //                               ? addChargeBox_MessageType
        //                               : updateChargeBox_MessageType);

        //    var messageTypes = messageTypesHash.ToArray();


        //    ComparizionResult? comparizionResult = null;

        //    if (messageTypes.Contains(updateChargeBox_MessageType))
        //        comparizionResult = ChargeBox.CompareWith(OldChargeBox);


        //    if (!DisableNotifications)
        //    {

        //        #region Telegram Notifications

        //        if (TelegramClient != null)
        //        {
        //            try
        //            {

        //                var AllTelegramNotifications  = ChargeBox.GetNotificationsOf<TelegramNotification>(messageTypes).
        //                                                     ToSafeHashSet();

        //                if (AllTelegramNotifications.SafeAny())
        //                {

        //                    if (messageTypes.Contains(addChargeBox_MessageType))
        //                        await TelegramClient.SendTelegrams(ChargeBoxHTMLInfo(ChargeBox) + " was successfully created.",
        //                                                           AllTelegramNotifications.Select(TelegramNotification => TelegramNotification.Username),
        //                                                           Telegram.Bot.Types.Enums.ParseMode.Html);

        //                    if (messageTypes.Contains(updateChargeBox_MessageType))
        //                        await TelegramClient.SendTelegrams(ChargeBoxHTMLInfo(ChargeBox) + " information had been successfully updated.\n" + comparizionResult?.ToTelegram(),
        //                                                           AllTelegramNotifications.Select(TelegramNotification => TelegramNotification.Username),
        //                                                           Telegram.Bot.Types.Enums.ParseMode.Html);

        //                }

        //            }
        //            catch (Exception e)
        //            {
        //                DebugX.LogException(e);
        //            }
        //        }

        //        #endregion

        //        #region SMS Notifications

        //        try
        //        {

        //            var AllSMSNotifications  = ChargeBox.GetNotificationsOf<SMSNotification>(messageTypes).
        //                                                    ToSafeHashSet();

        //            if (AllSMSNotifications.SafeAny())
        //            {

        //                if (messageTypes.Contains(addChargeBox_MessageType))
        //                    SendSMS(String.Concat("ChargeBox '", ChargeBox.Name.FirstText(), "' was successfully created. ",
        //                                          "https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id),
        //                            AllSMSNotifications.Select(smsPhoneNumber => smsPhoneNumber.PhoneNumber.ToString()).ToArray(),
        //                            SMSSenderName);

        //                if (messageTypes.Contains(updateChargeBox_MessageType))
        //                    SendSMS(String.Concat("ChargeBox '", ChargeBox.Name.FirstText(), "' information had been successfully updated. ",
        //                                          "https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id),
        //                                          // + {Updated information}
        //                            AllSMSNotifications.Select(smsPhoneNumber => smsPhoneNumber.PhoneNumber.ToString()).ToArray(),
        //                            SMSSenderName);

        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.LogException(e);
        //        }

        //        #endregion

        //        #region HTTPS Notifications

        //        try
        //        {

        //            var AllHTTPSNotifications  = ChargeBox.GetNotificationsOf<HTTPSNotification>(messageTypes).
        //                                                      ToSafeHashSet();

        //            if (AllHTTPSNotifications.SafeAny())
        //            {

        //                if (messageTypes.Contains(addChargeBox_MessageType))
        //                    await SendHTTPSNotifications(AllHTTPSNotifications,
        //                                                 new JObject(
        //                                                     new JProperty("chargeBoxCreated",
        //                                                         ChargeBox.ToJSON()
        //                                                     ),
        //                                                     new JProperty("timestamp", Timestamp.Now.ToIso8601())
        //                                                 ));

        //                if (messageTypes.Contains(updateChargeBox_MessageType))
        //                    await SendHTTPSNotifications(AllHTTPSNotifications,
        //                                                 new JObject(
        //                                                     new JProperty("chargeBoxUpdated",
        //                                                         ChargeBox.ToJSON()
        //                                                     ),
        //                                                     new JProperty("timestamp", Timestamp.Now.ToIso8601())
        //                                                 ));

        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.LogException(e);
        //        }

        //        #endregion

        //        #region EMailNotifications

        //        if (SMTPClient != null)
        //        {
        //            try
        //            {

        //                var AllEMailNotifications  = ChargeBox.GetNotificationsOf<EMailNotification>(messageTypes).
        //                                                          ToSafeHashSet();

        //                if (AllEMailNotifications.SafeAny())
        //                {

        //                    if (messageTypes.Contains(addChargeBox_MessageType))
        //                        await SMTPClient.Send(
        //                                 new HTMLEMailBuilder() {

        //                                     From           = Robot.EMail,
        //                                     To             = EMailAddressListBuilder.Create(EMailAddressList.Create(AllEMailNotifications.Select(emailnotification => emailnotification.EMailAddress))),
        //                                     Passphrase     = APIPassphrase,
        //                                     Subject        = ChargeBoxTextInfo(ChargeBox) + " was successfully created",

        //                                     HTMLText       = String.Concat(HTMLEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                    ChargeBoxHTMLInfo(ChargeBox) + " was successfully created.",
        //                                                                    HTMLEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                     PlainText      = String.Concat(TextEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                    ChargeBoxTextInfo(ChargeBox) + " was successfully created.\r\n",
        //                                                                    "https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id, "\r\r\r\r",
        //                                                                    TextEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                     SecurityLevel  = EMailSecurity.autosign

        //                                 });

        //                    if (messageTypes.Contains(updateChargeBox_MessageType))
        //                        await SMTPClient.Send(
        //                                 new HTMLEMailBuilder() {

        //                                     From           = Robot.EMail,
        //                                     To             = EMailAddressListBuilder.Create(EMailAddressList.Create(AllEMailNotifications.Select(emailnotification => emailnotification.EMailAddress))),
        //                                     Passphrase     = APIPassphrase,
        //                                     Subject        = ChargeBoxTextInfo(ChargeBox) + " information had been successfully updated",

        //                                     HTMLText       = String.Concat(HTMLEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                    ChargeBoxHTMLInfo(ChargeBox) + " information had been successfully updated.<br /><br />",
        //                                                                    comparizionResult?.ToHTML() ?? "",
        //                                                                    HTMLEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                     PlainText      = String.Concat(TextEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                    ChargeBoxTextInfo(ChargeBox) + " information had been successfully updated.\r\r\r\r",
        //                                                                    comparizionResult?.ToText() ?? "",
        //                                                                    "\r\r\r\r",
        //                                                                    "https://", ExternalDNSName, BasePath, "/chargeBoxs/", ChargeBox.Id, "\r\r\r\r",
        //                                                                    TextEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                     SecurityLevel  = EMailSecurity.autosign

        //                                 });

        //                }

        //            }
        //            catch (Exception e)
        //            {
        //                DebugX.LogException(e);
        //            }
        //        }

        //        #endregion

        //    }

        //}

        #endregion

        #region (protected internal) SendNotifications           (ChargeBox, ParentChargeBoxes, MessageType(s), ...)

        ///// <summary>
        ///// Send chargeBox notifications.
        ///// </summary>
        ///// <param name="ChargeBox">The charge box.</param>
        ///// <param name="ParentChargeBoxes">The enumeration of parent charge boxes.</param>
        ///// <param name="MessageType">The chargeBox notification.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">The invoking chargeBox identification</param>
        //protected internal virtual Task SendNotifications(ChargeBox               ChargeBox,
        //                                                  IEnumerable<ChargeBox>  ParentChargeBoxes,
        //                                                  NotificationMessageType    MessageType,
        //                                                  EventTracking_Id           EventTrackingId   = null,
        //                                                  User_Id?                   CurrentUserId     = null)

        //    => SendNotifications(ChargeBox,
        //                         ParentChargeBoxes,
        //                         new NotificationMessageType[] { MessageType },
        //                         EventTrackingId,
        //                         CurrentUserId);


        ///// <summary>
        ///// Send chargeBox notifications.
        ///// </summary>
        ///// <param name="ChargeBox">The charge box.</param>
        ///// <param name="ParentChargeBoxes">The enumeration of parent charge boxes.</param>
        ///// <param name="MessageTypes">The user notifications.</param>
        ///// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        ///// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        //protected internal async virtual Task SendNotifications(ChargeBox                          ChargeBox,
        //                                                        IEnumerable<ChargeBox>             ParentChargeBoxes,
        //                                                        IEnumerable<NotificationMessageType>  MessageTypes,
        //                                                        EventTracking_Id                      EventTrackingId   = null,
        //                                                        User_Id?                              CurrentUserId     = null)
        //{

        //    if (ChargeBox is null)
        //        throw new ArgumentNullException(nameof(ChargeBox),         "The given chargeBox must not be null or empty!");

        //    if (ParentChargeBoxes is null)
        //        ParentChargeBoxes = new ChargeBox[0];

        //    var messageTypesHash = new HashSet<NotificationMessageType>(MessageTypes.Where(messageType => !messageType.IsNullOrEmpty));

        //    if (messageTypesHash.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(MessageTypes),         "The given enumeration of message types must not be null or empty!");

        //    //if (messageTypesHash.Contains(addUserIfNotExists_MessageType))
        //    //    messageTypesHash.Add(addUser_MessageType);

        //    //if (messageTypesHash.Contains(addOrUpdateUser_MessageType))
        //    //    messageTypesHash.Add(OldChargeBox == null
        //    //                           ? addUser_MessageType
        //    //                           : updateUser_MessageType);

        //    var messageTypes = messageTypesHash.ToArray();


        //    if (!DisableNotifications)
        //    {

        //        #region Telegram Notifications

        //        if (TelegramClient != null)
        //        {
        //            try
        //            {

        //                var AllTelegramNotifications  = ParentChargeBoxes.
        //                                                    SelectMany(parent => parent.User2ChargeBoxEdges).
        //                                                    SelectMany(edge   => edge.Source.GetNotificationsOf<TelegramNotification>(deleteChargeBox_MessageType)).
        //                                                    ToSafeHashSet();

        //                if (AllTelegramNotifications.SafeAny())
        //                {

        //                    if (messageTypes.Contains(deleteChargeBox_MessageType))
        //                        await TelegramClient.SendTelegrams(ChargeBoxHTMLInfo(ChargeBox) + " has been deleted.",
        //                                                           AllTelegramNotifications.Select(TelegramNotification => TelegramNotification.Username),
        //                                                           Telegram.Bot.Types.Enums.ParseMode.Html);

        //                }

        //            }
        //            catch (Exception e)
        //            {
        //                DebugX.LogException(e);
        //            }
        //        }

        //        #endregion

        //        #region SMS Notifications

        //        try
        //        {

        //            var AllSMSNotifications = ParentChargeBoxes.
        //                                          SelectMany(parent => parent.User2ChargeBoxEdges).
        //                                          SelectMany(edge   => edge.Source.GetNotificationsOf<SMSNotification>(deleteChargeBox_MessageType)).
        //                                          ToSafeHashSet();

        //            if (AllSMSNotifications.SafeAny())
        //            {

        //                if (messageTypes.Contains(deleteChargeBox_MessageType))
        //                    SendSMS(String.Concat("ChargeBox '", ChargeBox.Name.FirstText(), "' has been deleted."),
        //                            AllSMSNotifications.Select(smsPhoneNumber => smsPhoneNumber.PhoneNumber.ToString()).ToArray(),
        //                            SMSSenderName);

        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.LogException(e);
        //        }

        //        #endregion

        //        #region HTTPS Notifications

        //        try
        //        {

        //            var AllHTTPSNotifications = ParentChargeBoxes.
        //                                            SelectMany(parent => parent.User2ChargeBoxEdges).
        //                                            SelectMany(edge   => edge.Source.GetNotificationsOf<HTTPSNotification>(deleteChargeBox_MessageType)).
        //                                            ToSafeHashSet();

        //            if (AllHTTPSNotifications.SafeAny())
        //            {

        //                if (messageTypes.Contains(deleteChargeBox_MessageType))
        //                    await SendHTTPSNotifications(AllHTTPSNotifications,
        //                                                 new JObject(
        //                                                     new JProperty("chargeBoxDeleted",
        //                                                         ChargeBox.ToJSON()
        //                                                     ),
        //                                                     new JProperty("timestamp", Timestamp.Now.ToIso8601())
        //                                                 ));

        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.LogException(e);
        //        }

        //        #endregion

        //        #region EMailNotifications

        //        if (SMTPClient != null)
        //        {
        //            try
        //            {

        //                var AllEMailNotifications = ParentChargeBoxes.
        //                                                SelectMany(parent => parent.User2ChargeBoxEdges).
        //                                                SelectMany(edge   => edge.Source.GetNotificationsOf<EMailNotification>(deleteChargeBox_MessageType)).
        //                                                ToSafeHashSet();

        //                if (AllEMailNotifications.SafeAny())
        //                {

        //                    if (messageTypes.Contains(deleteChargeBox_MessageType))
        //                        await SMTPClient.Send(
        //                             new HTMLEMailBuilder() {

        //                                 From           = Robot.EMail,
        //                                 To             = EMailAddressListBuilder.Create(EMailAddressList.Create(AllEMailNotifications.Select(emailnotification => emailnotification.EMailAddress))),
        //                                 Passphrase     = APIPassphrase,
        //                                 Subject        = ChargeBoxTextInfo(ChargeBox) + " has been deleted",

        //                                 HTMLText       = String.Concat(HTMLEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                ChargeBoxHTMLInfo(ChargeBox) + " has been deleted.<br />",
        //                                                                HTMLEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                 PlainText      = String.Concat(TextEMailHeader(ExternalDNSName, BasePath, EMailType.Notification),
        //                                                                ChargeBoxTextInfo(ChargeBox) + " has been deleted.\r\n",
        //                                                                TextEMailFooter(ExternalDNSName, BasePath, EMailType.Notification)),

        //                                 SecurityLevel  = EMailSecurity.autosign

        //                             });

        //                }

        //            }
        //            catch (Exception e)
        //            {
        //                DebugX.LogException(e);
        //            }
        //        }

        //        #endregion

        //    }

        //}

        #endregion

        #region (protected internal) GetChargeBoxSerializator (Request, ChargeBox)

        protected internal ChargeBoxToJSONDelegate GetChargeBoxSerializator(HTTPRequest  Request,
                                                                            User         User)
        {

            switch (User?.Id.ToString())
            {

                default:
                    return (chargeBox,
                            embedded,
                            expandTags,
                            includeCryptoHash)

                            => chargeBox.ToJSON(embedded,
                                                expandTags,
                                                includeCryptoHash);

            }

        }

        #endregion


        #region AddChargeBox           (ChargeBox, OnAdded = null, ...)

        /// <summary>
        /// A delegate called whenever a charge box was added.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the chargeBox was added.</param>
        /// <param name="ChargeBox">The added charge box.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public delegate Task OnChargeBoxAddedDelegate(DateTime           Timestamp,
                                                      ChargeBox          ChargeBox,
                                                      EventTracking_Id?  EventTrackingId   = null,
                                                      User_Id?           CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a charge box was added.
        /// </summary>
        public event OnChargeBoxAddedDelegate? OnChargeBoxAdded;


        #region (protected internal) _AddChargeBox(ChargeBox, OnAdded = null, ...)

        /// <summary>
        /// Add the given chargeBox to the API.
        /// </summary>
        /// <param name="ChargeBox">A new chargeBox to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        protected internal async Task<AddChargeBoxResult>

            _AddChargeBox(ChargeBox                             ChargeBox,
                          Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                          EventTracking_Id?                     EventTrackingId   = null,
                          User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargeBox.API is not null && ChargeBox.API != this)
                return AddChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox is already attached to another API!".ToI18NString(),
                           eventTrackingId,
                           CentralSystemId,
                           this
                       );

            if (chargeBoxes.ContainsKey(ChargeBox.Id))
                return AddChargeBoxResult.ArgumentError(
                           ChargeBox,
                           $"ChargeBox identification '{ChargeBox.Id}' already exists!".ToI18NString(),
                           eventTrackingId,
                           CentralSystemId,
                           this
                       );

            //if (ChargeBox.Id.Length < MinNetworkingNodeIdLength)
            //    return AddChargeBoxResult.ArgumentError(ChargeBox,
            //                                               eventTrackingId,
            //                                               nameof(ChargeBox),
            //                                               "ChargeBox identification '" + ChargeBox.Id + "' is too short!");

            //if (ChargeBox.Name.IsNullOrEmpty() || ChargeBox.Name.IsNullOrEmpty())
            //    return AddChargeBoxResult.ArgumentError(ChargeBox,
            //                                               eventTrackingId,
            //                                               nameof(ChargeBox),
            //                                               "The given chargeBox name must not be null!");

            //if (ChargeBox.Name.Length < MinChargeBoxNameLength)
            //    return AddChargeBoxResult.ArgumentError(ChargeBox,
            //                                       nameof(ChargeBox),
            //                                       "ChargeBox name '" + ChargeBox.Name + "' is too short!");

            ChargeBox.API = this;


            //await WriteToDatabaseFile(addChargeBox_MessageType,
            //                          ChargeBox.ToJSON(false, true),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargeBoxes.Add(ChargeBox.Id, ChargeBox);

            OnAdded?.Invoke(ChargeBox,
                            eventTrackingId);

            var OnChargeBoxAddedLocal = OnChargeBoxAdded;
            if (OnChargeBoxAddedLocal is not null)
                await OnChargeBoxAddedLocal.Invoke(Timestamp.Now,
                                                   ChargeBox,
                                                   eventTrackingId,
                                                   CurrentUserId);

            //await SendNotifications(ChargeBox,
            //                        addChargeBox_MessageType,
            //                        null,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return AddChargeBoxResult.Success(
                       ChargeBox,
                       eventTrackingId,
                       CentralSystemId,
                       this
                   );

        }

        #endregion

        #region AddChargeBox                      (ChargeBox, OnAdded = null, ...)

        /// <summary>
        /// Add the given chargeBox and add him/her to the given charge box.
        /// </summary>
        /// <param name="ChargeBox">A new charge box.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public async Task<AddChargeBoxResult>

            AddChargeBox(ChargeBox                             ChargeBox,
                         Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                         EventTracking_Id?                     EventTrackingId   = null,
                         User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargeBoxesSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _AddChargeBox(ChargeBox,
                                               OnAdded,
                                               eventTrackingId,
                                               CurrentUserId);

                }
                catch (Exception e)
                {

                    return AddChargeBoxResult.Error(
                               ChargeBox,
                               e,
                               eventTrackingId,
                               CentralSystemId,
                               this
                           );

                }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }

            }

            return AddChargeBoxResult.LockTimeout(
                       ChargeBox,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       CentralSystemId,
                       this
                   );

        }

        #endregion

        #endregion

        #region AddChargeBoxIfNotExists(ChargeBox, OnAdded = null, ...)

        #region (protected internal) _AddChargeBoxIfNotExists(ChargeBox, OnAdded = null, ...)

        /// <summary>
        /// When it has not been created before, add the given chargeBox to the API.
        /// </summary>
        /// <param name="ChargeBox">A new chargeBox to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        protected internal async Task<AddChargeBoxResult>

            _AddChargeBoxIfNotExists(ChargeBox                             ChargeBox,
                                     Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                                     EventTracking_Id?                     EventTrackingId   = null,
                                     User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargeBox.API != null && ChargeBox.API != this)
                return AddChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox is already attached to another API!".ToI18NString(),
                           eventTrackingId,
                           CentralSystemId,
                           this
                       );

            if (chargeBoxes.ContainsKey(ChargeBox.Id))
                return AddChargeBoxResult.NoOperation(
                           chargeBoxes[ChargeBox.Id],
                           eventTrackingId,
                           CentralSystemId,
                           this
                       );

            //if (ChargeBox.Id.Length < MinNetworkingNodeIdLength)
            //    return AddChargeBoxResult.ArgumentError(ChargeBox,
            //                                                          eventTrackingId,
            //                                                          nameof(ChargeBox),
            //                                                          "ChargeBox identification '" + ChargeBox.Id + "' is too short!");

            //if (ChargeBox.Name.IsNullOrEmpty() || ChargeBox.Name.IsNullOrEmpty())
            //    return AddChargeBoxResult.ArgumentError(ChargeBox,
            //                                                          eventTrackingId,
            //                                                          nameof(ChargeBox),
            //                                                          "The given chargeBox name must not be null!");

            //if (ChargeBox.Name.Length < MinChargeBoxNameLength)
            //    return AddChargeBoxResult.ArgumentError(ChargeBox,
            //                                                  nameof(ChargeBox),
            //                                                  "ChargeBox name '" + ChargeBox.Name + "' is too short!");

            ChargeBox.API = this;


            //await WriteToDatabaseFile(addChargeBoxIfNotExists_MessageType,
            //                          ChargeBox.ToJSON(false, true),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargeBoxes.Add(ChargeBox.Id, ChargeBox);

            OnAdded?.Invoke(ChargeBox,
                            eventTrackingId);

            var OnChargeBoxAddedLocal = OnChargeBoxAdded;
            if (OnChargeBoxAddedLocal != null)
                await OnChargeBoxAddedLocal.Invoke(Timestamp.Now,
                                                   ChargeBox,
                                                   eventTrackingId,
                                                   CurrentUserId);

            //await SendNotifications(ChargeBox,
            //                        addChargeBoxIfNotExists_MessageType,
            //                        null,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return AddChargeBoxResult.Success(
                       ChargeBox,
                       eventTrackingId,
                       CentralSystemId,
                       this
                   );

        }

        #endregion

        #region AddChargeBoxIfNotExists                      (ChargeBox, OnAdded = null, ...)

        /// <summary>
        /// Add the given chargeBox and add him/her to the given charge box.
        /// </summary>
        /// <param name="ChargeBox">A new charge box.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public async Task<AddChargeBoxResult>

            AddChargeBoxIfNotExists(ChargeBox                             ChargeBox,
                                    Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                                    EventTracking_Id?                     EventTrackingId   = null,
                                    User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargeBoxesSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _AddChargeBoxIfNotExists(ChargeBox,
                                                          OnAdded,
                                                          eventTrackingId,
                                                          CurrentUserId);

                }
                catch (Exception e)
                {

                    return AddChargeBoxResult.Error(
                               ChargeBox,
                               e,
                               eventTrackingId,
                               CentralSystemId,
                               this
                           );

                }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }

            }

            return AddChargeBoxResult.LockTimeout(
                       ChargeBox,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       CentralSystemId,
                       this
                   );

        }

        #endregion

        #endregion

        #region AddOrUpdateChargeBox   (ChargeBox, OnAdded = null, OnUpdated = null, ...)

        #region (protected internal) _AddOrUpdateChargeBox(ChargeBox, OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given chargeBox to/within the API.
        /// </summary>
        /// <param name="ChargeBox">A charge box.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        protected internal async Task<AddOrUpdateChargeBoxResult>

            _AddOrUpdateChargeBox(ChargeBox                             ChargeBox,
                                  Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                                  Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                                  EventTracking_Id?                     EventTrackingId   = null,
                                  User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargeBox.API != null && ChargeBox.API != this)
                return AddOrUpdateChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox is already attached to another API!".ToI18NString(),
                           eventTrackingId,
                           CentralSystemId,
                           this
                       );

            //if (ChargeBox.Id.Length < MinNetworkingNodeIdLength)
            //    return AddOrUpdateChargeBoxResult.ArgumentError(ChargeBox,
            //                                                       eventTrackingId,
            //                                                       nameof(ChargeBox),
            //                                                       "The given chargeBox identification '" + ChargeBox.Id + "' is too short!");

            //if (ChargeBox.Name.IsNullOrEmpty() || ChargeBox.Name.IsNullOrEmpty())
            //    return AddOrUpdateChargeBoxResult.ArgumentError(ChargeBox,
            //                                                       eventTrackingId,
            //                                                       nameof(ChargeBox),
            //                                                       "The given chargeBox name must not be null!");

            //if (ChargeBox.Name.Length < MinChargeBoxNameLength)
            //    return AddOrUpdateChargeBoxResult.ArgumentError(ChargeBox,
            //                                               eventTrackingId,
            //                                               nameof(ChargeBox),
            //                                               "ChargeBox name '" + ChargeBox.Name + "' is too short!");

            ChargeBox.API = this;


            //await WriteToDatabaseFile(addOrUpdateChargeBox_MessageType,
            //                          ChargeBox.ToJSON(false, true),
            //                          eventTrackingId,
            //                          CurrentUserId);

            if (chargeBoxes.TryGetValue(ChargeBox.Id, out var OldChargeBox))
            {
                chargeBoxes.Remove(OldChargeBox.Id);
                ChargeBox.CopyAllLinkedDataFromBase(OldChargeBox);
            }

            chargeBoxes.Add(ChargeBox.Id, ChargeBox);

            if (OldChargeBox is null)
            {

                OnAdded?.Invoke(ChargeBox,
                                eventTrackingId);

                var OnChargeBoxAddedLocal = OnChargeBoxAdded;
                if (OnChargeBoxAddedLocal != null)
                    await OnChargeBoxAddedLocal.Invoke(Timestamp.Now,
                                                       ChargeBox,
                                                       eventTrackingId,
                                                       CurrentUserId);

                //await SendNotifications(ChargeBox,
                //                        addChargeBox_MessageType,
                //                        null,
                //                        eventTrackingId,
                //                        CurrentUserId);

                return AddOrUpdateChargeBoxResult.Added(
                           ChargeBox,
                           eventTrackingId,
                           CentralSystemId,
                           this
                       );

            }

            OnUpdated?.Invoke(ChargeBox,
                              eventTrackingId);

            var OnChargeBoxUpdatedLocal = OnChargeBoxUpdated;
            if (OnChargeBoxUpdatedLocal != null)
                await OnChargeBoxUpdatedLocal.Invoke(Timestamp.Now,
                                                        ChargeBox,
                                                        OldChargeBox,
                                                        eventTrackingId,
                                                        CurrentUserId);

            //await SendNotifications(ChargeBox,
            //                        updateChargeBox_MessageType,
            //                        OldChargeBox,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return AddOrUpdateChargeBoxResult.Updated(
                       ChargeBox,
                       eventTrackingId,
                       CentralSystemId,
                       this
                   );

        }

        #endregion

        #region AddOrUpdateChargeBox                      (ChargeBox, OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given chargeBox to/within the API.
        /// </summary>
        /// <param name="ChargeBox">A charge box.</param>
        /// <param name="OnAdded">A delegate run whenever the chargeBox has been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public async Task<AddOrUpdateChargeBoxResult>

            AddOrUpdateChargeBox(ChargeBox                             ChargeBox,
                                 Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                                 Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                                 EventTracking_Id?                     EventTrackingId   = null,
                                 User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargeBoxesSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _AddOrUpdateChargeBox(ChargeBox,
                                                       OnAdded,
                                                       OnUpdated,
                                                       eventTrackingId,
                                                       CurrentUserId);

                }
                catch (Exception e)
                {

                    return AddOrUpdateChargeBoxResult.Error(
                               ChargeBox,
                               e,
                               eventTrackingId,
                               CentralSystemId,
                               this
                           );

                }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return AddOrUpdateChargeBoxResult.LockTimeout(
                       ChargeBox,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       CentralSystemId,
                       this
                   );

        }

        #endregion

        #endregion

        #region UpdateChargeBox        (ChargeBox,                 OnUpdated = null, ...)

        /// <summary>
        /// A delegate called whenever a charge box was updated.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the chargeBox was updated.</param>
        /// <param name="ChargeBox">The updated charge box.</param>
        /// <param name="OldChargeBox">The old charge box.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public delegate Task OnChargeBoxUpdatedDelegate(DateTime           Timestamp,
                                                        ChargeBox          ChargeBox,
                                                        ChargeBox          OldChargeBox,
                                                        EventTracking_Id?  EventTrackingId   = null,
                                                        User_Id?           CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a charge box was updated.
        /// </summary>
        public event OnChargeBoxUpdatedDelegate? OnChargeBoxUpdated;


        #region (protected internal) _UpdateChargeBox(ChargeBox,                 OnUpdated = null, ...)

        /// <summary>
        /// Update the given chargeBox to/within the API.
        /// </summary>
        /// <param name="ChargeBox">A charge box.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        protected internal async Task<UpdateChargeBoxResult>

            _UpdateChargeBox(ChargeBox                             ChargeBox,
                             Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                             EventTracking_Id?                     EventTrackingId   = null,
                             User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (!_TryGetChargeBox(ChargeBox.Id, out var OldChargeBox))
                return UpdateChargeBoxResult.ArgumentError(
                           ChargeBox,
                           $"The given chargeBox '{ChargeBox.Id}' does not exists in this API!".ToI18NString(),
                           eventTrackingId,
                           CentralSystemId,
                           this
                       );

            if (ChargeBox.API != null && ChargeBox.API != this)
                return UpdateChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox is already attached to another API!".ToI18NString(),
                           eventTrackingId,
                           CentralSystemId,
                           this
                       );

            ChargeBox.API = this;


            //await WriteToDatabaseFile(updateChargeBox_MessageType,
            //                          ChargeBox.ToJSON(),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargeBoxes.Remove(OldChargeBox.Id);
            ChargeBox.CopyAllLinkedDataFromBase(OldChargeBox);
            chargeBoxes.Add(ChargeBox.Id, ChargeBox);

            OnUpdated?.Invoke(ChargeBox,
                              eventTrackingId);

            var OnChargeBoxUpdatedLocal = OnChargeBoxUpdated;
            if (OnChargeBoxUpdatedLocal is not null)
                await OnChargeBoxUpdatedLocal.Invoke(Timestamp.Now,
                                                     ChargeBox,
                                                     OldChargeBox,
                                                     eventTrackingId,
                                                     CurrentUserId);

            //await SendNotifications(ChargeBox,
            //                        updateChargeBox_MessageType,
            //                        OldChargeBox,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return UpdateChargeBoxResult.Success(
                       ChargeBox,
                       eventTrackingId,
                       CentralSystemId,
                       this
                   );

        }

        #endregion

        #region UpdateChargeBox                      (ChargeBox,                 OnUpdated = null, ...)

        /// <summary>
        /// Update the given chargeBox to/within the API.
        /// </summary>
        /// <param name="ChargeBox">A charge box.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public async Task<UpdateChargeBoxResult>

            UpdateChargeBox(ChargeBox                             ChargeBox,
                            Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                            EventTracking_Id?                     EventTrackingId   = null,
                            User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargeBoxesSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _UpdateChargeBox(ChargeBox,
                                                  OnUpdated,
                                                  eventTrackingId,
                                                  CurrentUserId);

                }
                catch (Exception e)
                {

                    return UpdateChargeBoxResult.Error(
                               ChargeBox,
                               e,
                               eventTrackingId,
                               CentralSystemId,
                               this
                           );

                }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return UpdateChargeBoxResult.LockTimeout(
                       ChargeBox,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       CentralSystemId,
                       this
                   );

        }

        #endregion


        #region (protected internal) _UpdateChargeBox(ChargeBox, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given charge box.
        /// </summary>
        /// <param name="ChargeBox">An charge box.</param>
        /// <param name="UpdateDelegate">A delegate to update the given charge box.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        protected internal async Task<UpdateChargeBoxResult>

            _UpdateChargeBox(ChargeBox                             ChargeBox,
                             Action<ChargeBox.Builder>             UpdateDelegate,
                             Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                             EventTracking_Id?                     EventTrackingId   = null,
                             User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (!_ChargeBoxExists(ChargeBox.Id))
                return UpdateChargeBoxResult.ArgumentError(
                           ChargeBox,
                           $"The given chargeBox '{ChargeBox.Id}' does not exists in this API!".ToI18NString(),
                           eventTrackingId,
                           CentralSystemId,
                           this
                       );

            if (ChargeBox.API != this)
                return UpdateChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox is not attached to this API!".ToI18NString(),
                           eventTrackingId,
                           CentralSystemId,
                           this
                       );

            if (UpdateDelegate is null)
                return UpdateChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given update delegate must not be null!".ToI18NString(),
                           eventTrackingId,
                           CentralSystemId,
                           this
                       );


            var builder = ChargeBox.ToBuilder();
            UpdateDelegate(builder);
            var updatedChargeBox = builder.ToImmutable;

            //await WriteToDatabaseFile(updateChargeBox_MessageType,
            //                          updatedChargeBox.ToJSON(),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargeBoxes.Remove(ChargeBox.Id);
            updatedChargeBox.CopyAllLinkedDataFromBase(ChargeBox);
            chargeBoxes.Add(updatedChargeBox.Id, updatedChargeBox);

            OnUpdated?.Invoke(updatedChargeBox,
                              eventTrackingId);

            var OnChargeBoxUpdatedLocal = OnChargeBoxUpdated;
            if (OnChargeBoxUpdatedLocal is not null)
                await OnChargeBoxUpdatedLocal.Invoke(Timestamp.Now,
                                                     updatedChargeBox,
                                                     ChargeBox,
                                                     eventTrackingId,
                                                     CurrentUserId);

            //await SendNotifications(updatedChargeBox,
            //                        updateChargeBox_MessageType,
            //                        ChargeBox,
            //                        eventTrackingId,
            //                        CurrentUserId);

            return UpdateChargeBoxResult.Success(
                       ChargeBox,
                       eventTrackingId,
                       CentralSystemId,
                       this
                   );

        }

        #endregion

        #region UpdateChargeBox                      (ChargeBox, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given charge box.
        /// </summary>
        /// <param name="ChargeBox">An charge box.</param>
        /// <param name="UpdateDelegate">A delegate to update the given charge box.</param>
        /// <param name="OnUpdated">A delegate run whenever the chargeBox has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public async Task<UpdateChargeBoxResult>

            UpdateChargeBox(ChargeBox                             ChargeBox,
                            Action<ChargeBox.Builder>             UpdateDelegate,
                            Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                            EventTracking_Id?                     EventTrackingId   = null,
                            User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargeBoxesSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _UpdateChargeBox(ChargeBox,
                                                  UpdateDelegate,
                                                  OnUpdated,
                                                  eventTrackingId,
                                                  CurrentUserId);

                }
                catch (Exception e)
                {

                    return UpdateChargeBoxResult.Error(
                               ChargeBox,
                               e,
                               eventTrackingId,
                               CentralSystemId,
                               this
                           );

                }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return UpdateChargeBoxResult.LockTimeout(
                       ChargeBox,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       CentralSystemId,
                       this
                   );

        }

        #endregion

        #endregion

        #region DeleteChargeBox        (ChargeBox, OnDeleted = null, ...)

        /// <summary>
        /// A delegate called whenever a charge box was deleted.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the chargeBox was deleted.</param>
        /// <param name="ChargeBox">The chargeBox to be deleted.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public delegate Task OnChargeBoxDeletedDelegate(DateTime           Timestamp,
                                                        ChargeBox          ChargeBox,
                                                        EventTracking_Id?  EventTrackingId   = null,
                                                        User_Id?           CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a charge box was deleted.
        /// </summary>
        public event OnChargeBoxDeletedDelegate? OnChargeBoxDeleted;


        #region (protected internal virtual) _CanDeleteChargeBox(ChargeBox)

        /// <summary>
        /// Determines whether the chargeBox can safely be deleted from the API.
        /// </summary>
        /// <param name="ChargeBox">The chargeBox to be deleted.</param>
        protected internal virtual I18NString? _CanDeleteChargeBox(ChargeBox ChargeBox)
        {

            //if (ChargeBox.Users.Any())
            //    return new I18NString(Languages.en, "The chargeBox still has members!");

            //if (ChargeBox.SubChargeBoxes.Any())
            //    return new I18NString(Languages.en, "The chargeBox still has sub chargeBoxs!");

            return null;

        }

        #endregion

        #region (protected internal) _DeleteChargeBox(ChargeBox, OnDeleted = null, ...)

        /// <summary>
        /// Delete the given charge box.
        /// </summary>
        /// <param name="ChargeBox">The chargeBox to be deleted.</param>
        /// <param name="OnDeleted">A delegate run whenever the chargeBox has been deleted successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task<DeleteChargeBoxResult>

            _DeleteChargeBox(ChargeBox                             ChargeBox,
                             Action<ChargeBox, EventTracking_Id>?  OnDeleted         = null,
                             EventTracking_Id?                     EventTrackingId   = null,
                             User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargeBox.API != this)
                return DeleteChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox is not attached to this API!".ToI18NString(),
                           eventTrackingId,
                           CentralSystemId,
                           this
                       );

            if (!chargeBoxes.TryGetValue(ChargeBox.Id, out var ChargeBoxToBeDeleted))
                return DeleteChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox does not exists in this API!".ToI18NString(),
                           eventTrackingId,
                           CentralSystemId,
                           this
                       );


            var veto = _CanDeleteChargeBox(ChargeBox);

            if (veto is not null)
                return DeleteChargeBoxResult.CanNotBeRemoved(
                           ChargeBox,
                           eventTrackingId,
                           CentralSystemId,
                           this,
                           veto
                       );


            //// Get all parent chargeBoxs now, because later
            //// the --isChildOf--> edge will no longer be available!
            //var parentChargeBoxes = ChargeBox.GetAllParents(parent => parent != NoOwner).
            //                                       ToArray();


            //// Remove all: this --edge--> other_chargeBox
            //foreach (var edge in ChargeBox.ChargeBox2ChargeBoxOutEdges.ToArray())
            //    await _UnlinkChargeBoxes(edge.Source,
            //                               edge.EdgeLabel,
            //                               edge.Target,
            //                               EventTrackingId,
            //                               SuppressNotifications:  false,
            //                               CurrentUserId:          CurrentUserId);

            //// Remove all: this <--edge-- other_chargeBox
            //foreach (var edge in ChargeBox.ChargeBox2ChargeBoxInEdges.ToArray())
            //    await _UnlinkChargeBoxes(edge.Target,
            //                               edge.EdgeLabel,
            //                               edge.Source,
            //                               EventTrackingId,
            //                               SuppressNotifications:  false,
            //                               CurrentUserId:          CurrentUserId);


            //await WriteToDatabaseFile(deleteChargeBox_MessageType,
            //                          ChargeBox.ToJSON(false, true),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargeBoxes.Remove(ChargeBox.Id);

            OnDeleted?.Invoke(ChargeBox,
                              eventTrackingId);

            var OnChargeBoxDeletedLocal = OnChargeBoxDeleted;
            if (OnChargeBoxDeletedLocal is not null)
                await OnChargeBoxDeletedLocal.Invoke(Timestamp.Now,
                                                     ChargeBox,
                                                     eventTrackingId,
                                                     CurrentUserId);

            //await SendNotifications(ChargeBox,
            //                        parentChargeBoxes,
            //                        deleteChargeBox_MessageType,
            //                        eventTrackingId,
            //                        CurrentUserId);


            return DeleteChargeBoxResult.Success(
                       ChargeBox,
                       eventTrackingId,
                       CentralSystemId,
                       this
                   );

        }

        #endregion

        #region DeleteChargeBox                      (ChargeBox, OnDeleted = null, ...)

        /// <summary>
        /// Delete the given charge box.
        /// </summary>
        /// <param name="ChargeBox">The chargeBox to be deleted.</param>
        /// <param name="OnDeleted">A delegate run whenever the chargeBox has been deleted successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<DeleteChargeBoxResult>

            DeleteChargeBox(ChargeBox                             ChargeBox,
                            Action<ChargeBox, EventTracking_Id>?  OnDeleted         = null,
                            EventTracking_Id?                     EventTrackingId   = null,
                            User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (await ChargeBoxesSemaphore.WaitAsync(SemaphoreSlimTimeout))
            {
                try
                {

                    return await _DeleteChargeBox(ChargeBox,
                                                  OnDeleted,
                                                  eventTrackingId,
                                                  CurrentUserId);

                }
                catch (Exception e)
                {

                    return DeleteChargeBoxResult.Error(
                               ChargeBox,
                               e,
                               eventTrackingId,
                               CentralSystemId,
                               this
                           );

                }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }

            }

            return DeleteChargeBoxResult.LockTimeout(
                       ChargeBox,
                       SemaphoreSlimTimeout,
                       eventTrackingId,
                       CentralSystemId,
                       this
                   );

        }

        #endregion

        #endregion


        #region ChargeBoxExists(NetworkingNodeId)

        /// <summary>
        /// Determines whether the given chargeBox identification exists within this API.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of an charge box.</param>
        protected internal Boolean _ChargeBoxExists(ChargeBox_Id NetworkingNodeId)

            => NetworkingNodeId.IsNotNullOrEmpty && chargeBoxes.ContainsKey(NetworkingNodeId);

        /// <summary>
        /// Determines whether the given chargeBox identification exists within this API.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of an charge box.</param>
        protected internal Boolean _ChargeBoxExists(ChargeBox_Id? NetworkingNodeId)

            => NetworkingNodeId.IsNotNullOrEmpty() && chargeBoxes.ContainsKey(NetworkingNodeId.Value);


        /// <summary>
        /// Determines whether the given chargeBox identification exists within this API.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of an charge box.</param>
        public Boolean ChargeBoxExists(ChargeBox_Id NetworkingNodeId)
        {

            if (ChargeBoxesSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _ChargeBoxExists(NetworkingNodeId);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return false;

        }

        /// <summary>
        /// Determines whether the given chargeBox identification exists within this API.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of an charge box.</param>
        public Boolean ChargeBoxExists(ChargeBox_Id? NetworkingNodeId)
        {

            if (ChargeBoxesSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _ChargeBoxExists(NetworkingNodeId);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return false;

        }

        #endregion

        #region GetChargeBox   (NetworkingNodeId)

        /// <summary>
        /// Get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of an charge box.</param>
        protected internal ChargeBox? _GetChargeBox(ChargeBox_Id NetworkingNodeId)
        {

            if (NetworkingNodeId.IsNotNullOrEmpty && chargeBoxes.TryGetValue(NetworkingNodeId, out var chargeBox))
                return chargeBox;

            return default;

        }

        /// <summary>
        /// Get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of an charge box.</param>
        protected internal ChargeBox? _GetChargeBox(ChargeBox_Id? NetworkingNodeId)
        {

            if (NetworkingNodeId is not null && chargeBoxes.TryGetValue(NetworkingNodeId.Value, out var chargeBox))
                return chargeBox;

            return default;

        }


        /// <summary>
        /// Get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of an charge box.</param>
        public ChargeBox? GetChargeBox(ChargeBox_Id NetworkingNodeId)
        {

            if (ChargeBoxesSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _GetChargeBox(NetworkingNodeId);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return null;

        }

        /// <summary>
        /// Get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of an charge box.</param>
        public ChargeBox? GetChargeBox(ChargeBox_Id? NetworkingNodeId)
        {

            if (ChargeBoxesSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _GetChargeBox(NetworkingNodeId);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return null;

        }

        #endregion

        #region TryGetChargeBox(NetworkingNodeId, out ChargeBox)

        /// <summary>
        /// Try to get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of an charge box.</param>
        /// <param name="ChargeBox">The charge box.</param>
        protected internal Boolean _TryGetChargeBox(ChargeBox_Id    NetworkingNodeId,
                                                    out ChargeBox?  ChargeBox)
        {

            if (NetworkingNodeId.IsNotNullOrEmpty && chargeBoxes.TryGetValue(NetworkingNodeId, out var chargeBox))
            {
                ChargeBox = chargeBox;
                return true;
            }

            ChargeBox = null;
            return false;

        }

        /// <summary>
        /// Try to get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of an charge box.</param>
        /// <param name="ChargeBox">The charge box.</param>
        protected internal Boolean _TryGetChargeBox(ChargeBox_Id?   NetworkingNodeId,
                                                    out ChargeBox?  ChargeBox)
        {

            if (NetworkingNodeId is not null && chargeBoxes.TryGetValue(NetworkingNodeId.Value, out var chargeBox))
            {
                ChargeBox = chargeBox;
                return true;
            }

            ChargeBox = null;
            return false;

        }


        /// <summary>
        /// Try to get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of an charge box.</param>
        /// <param name="ChargeBox">The charge box.</param>
        public Boolean TryGetChargeBox(ChargeBox_Id    NetworkingNodeId,
                                       out ChargeBox?  ChargeBox)
        {

            if (ChargeBoxesSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _TryGetChargeBox(NetworkingNodeId, out ChargeBox);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            ChargeBox = null;
            return false;

        }

        /// <summary>
        /// Try to get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of an charge box.</param>
        /// <param name="ChargeBox">The charge box.</param>
        public Boolean TryGetChargeBox(ChargeBox_Id?   NetworkingNodeId,
                                       out ChargeBox?  ChargeBox)
        {

            if (ChargeBoxesSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return _TryGetChargeBox(NetworkingNodeId, out ChargeBox);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        ChargeBoxesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            ChargeBox = null;
            return false;

        }

        #endregion

        #endregion


        #region CSMS -> Charging Station Messages

        #region (private) NextRequestId

        private Request_Id NextRequestId
        {
            get
            {

                Interlocked.Increment(ref internalRequestId);

                return Request_Id.Parse(internalRequestId.ToString());

            }
        }

        public CentralSystem_Id Id => throw new NotImplementedException();

        Request_Id ICentralSystem.NextRequestId => throw new NotImplementedException();

        public IEnumerable<ICSMSChannel> CSMSChannels => throw new NotImplementedException();

        public NetworkingNode_Id ChargeBoxIdentity => throw new NotImplementedException();

        public string From => throw new NotImplementedException();

        public string To => throw new NotImplementedException();

        #endregion


        #region Reset                       (Request)

        /// <summary>
        /// Reset the given charge box.
        /// </summary>
        /// <param name="Request">A Reset request.</param>
        public async Task<CP.ResetResponse> Reset(ResetRequest Request)
        {

            #region Send OnResetRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnResetRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnResetRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomResetRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.Reset(Request)

                                      : new CP.ResetResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.ResetResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomResetResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnResetResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnResetResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnResetResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ChangeAvailability          (Request)

        /// <summary>
        /// ChangeAvailability the given charge box.
        /// </summary>
        /// <param name="Request">A ChangeAvailability request.</param>
        public async Task<CP.ChangeAvailabilityResponse> ChangeAvailability(ChangeAvailabilityRequest Request)
        {

            #region Send OnChangeAvailabilityRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnChangeAvailabilityRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnChangeAvailabilityRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomChangeAvailabilityRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.ChangeAvailability(Request)

                                      : new CP.ChangeAvailabilityResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.ChangeAvailabilityResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomChangeAvailabilityResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnChangeAvailabilityResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnChangeAvailabilityResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnChangeAvailabilityResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetConfiguration            (Request)

        /// <summary>
        /// GetConfiguration the given charge box.
        /// </summary>
        /// <param name="Request">A GetConfiguration request.</param>
        public async Task<CP.GetConfigurationResponse> GetConfiguration(GetConfigurationRequest Request)
        {

            #region Send OnGetConfigurationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetConfigurationRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetConfigurationRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetConfigurationRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetConfiguration(Request)

                                      : new CP.GetConfigurationResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.GetConfigurationResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetConfigurationResponseSerializer,
                    CustomConfigurationKeySerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetConfigurationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetConfigurationResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetConfigurationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ChangeConfiguration         (Request)

        /// <summary>
        /// ChangeConfiguration the given charge box.
        /// </summary>
        /// <param name="Request">A ChangeConfiguration request.</param>
        public async Task<CP.ChangeConfigurationResponse> ChangeConfiguration(ChangeConfigurationRequest Request)
        {

            #region Send OnChangeConfigurationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnChangeConfigurationRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnChangeConfigurationRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomChangeConfigurationRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.ChangeConfiguration(Request)

                                      : new CP.ChangeConfigurationResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.ChangeConfigurationResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomChangeConfigurationResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnChangeConfigurationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnChangeConfigurationResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnChangeConfigurationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DataTransfer                (Request)

        /// <summary>
        /// DataTransfer the given charge box.
        /// </summary>
        /// <param name="Request">A DataTransfer request.</param>
        public async Task<CP.DataTransferResponse> DataTransfer(DataTransferRequest Request)
        {

            #region Send OnDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDataTransferRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomDataTransferRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.DataTransfer(Request)

                                      : new CP.DataTransferResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.DataTransferResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomDataTransferResponseSerializer,
                    null,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDataTransferResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetDiagnostics              (Request)

        /// <summary>
        /// GetDiagnostics the given charge box.
        /// </summary>
        /// <param name="Request">A GetDiagnostics request.</param>
        public async Task<CP.GetDiagnosticsResponse> GetDiagnostics(GetDiagnosticsRequest Request)
        {

            #region Send OnGetDiagnosticsRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetDiagnosticsRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetDiagnosticsRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetDiagnosticsRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetDiagnostics(Request)

                                      : new CP.GetDiagnosticsResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.GetDiagnosticsResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetDiagnosticsResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetDiagnosticsResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetDiagnosticsResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetDiagnosticsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region TriggerMessage              (Request)

        /// <summary>
        /// TriggerMessage the given charge box.
        /// </summary>
        /// <param name="Request">A TriggerMessage request.</param>
        public async Task<CP.TriggerMessageResponse> TriggerMessage(TriggerMessageRequest Request)
        {

            #region Send OnTriggerMessageRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnTriggerMessageRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnTriggerMessageRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomTriggerMessageRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.TriggerMessage(Request)

                                      : new CP.TriggerMessageResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.TriggerMessageResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomTriggerMessageResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnTriggerMessageResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnTriggerMessageResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnTriggerMessageResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UpdateFirmware              (Request)

        /// <summary>
        /// UpdateFirmware the given charge box.
        /// </summary>
        /// <param name="Request">A UpdateFirmware request.</param>
        public async Task<CP.UpdateFirmwareResponse> UpdateFirmware(UpdateFirmwareRequest Request)
        {

            #region Send OnUpdateFirmwareRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnUpdateFirmwareRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomUpdateFirmwareRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.UpdateFirmware(Request)

                                      : new CP.UpdateFirmwareResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.UpdateFirmwareResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomUpdateFirmwareResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnUpdateFirmwareResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnUpdateFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region ReserveNow                  (Request)

        /// <summary>
        /// ReserveNow the given charge box.
        /// </summary>
        /// <param name="Request">A ReserveNow request.</param>
        public async Task<CP.ReserveNowResponse> ReserveNow(ReserveNowRequest Request)
        {

            #region Send OnReserveNowRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnReserveNowRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnReserveNowRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomReserveNowRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.ReserveNow(Request)

                                      : new CP.ReserveNowResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.ReserveNowResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomReserveNowResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnReserveNowResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnReserveNowResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnReserveNowResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region CancelReservation           (Request)

        /// <summary>
        /// CancelReservation the given charge box.
        /// </summary>
        /// <param name="Request">A CancelReservation request.</param>
        public async Task<CP.CancelReservationResponse> CancelReservation(CancelReservationRequest Request)
        {

            #region Send OnCancelReservationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnCancelReservationRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnCancelReservationRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomCancelReservationRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.CancelReservation(Request)

                                      : new CP.CancelReservationResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.CancelReservationResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomCancelReservationResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnCancelReservationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnCancelReservationResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnCancelReservationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region RemoteStartTransaction      (Request)

        /// <summary>
        /// RemoteStartTransaction the given charge box.
        /// </summary>
        /// <param name="Request">A RemoteStartTransaction request.</param>
        public async Task<CP.RemoteStartTransactionResponse> RemoteStartTransaction(RemoteStartTransactionRequest Request)
        {

            #region Send OnRemoteStartTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRemoteStartTransactionRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnRemoteStartTransactionRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomRemoteStartTransactionRequestSerializer,
                                          CustomChargingProfileSerializer,
                                          CustomChargingScheduleSerializer,
                                          CustomChargingSchedulePeriodSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.RemoteStartTransaction(Request)

                                      : new CP.RemoteStartTransactionResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.RemoteStartTransactionResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomRemoteStartTransactionResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnRemoteStartTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnRemoteStartTransactionResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnRemoteStartTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region RemoteStopTransaction       (Request)

        /// <summary>
        /// RemoteStopTransaction the given charge box.
        /// </summary>
        /// <param name="Request">A RemoteStopTransaction request.</param>
        public async Task<CP.RemoteStopTransactionResponse> RemoteStopTransaction(RemoteStopTransactionRequest Request)
        {

            #region Send OnRemoteStopTransactionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnRemoteStopTransactionRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnRemoteStopTransactionRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomRemoteStopTransactionRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.RemoteStopTransaction(Request)

                                      : new CP.RemoteStopTransactionResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.RemoteStopTransactionResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomRemoteStopTransactionResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnRemoteStopTransactionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnRemoteStopTransactionResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnRemoteStopTransactionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SetChargingProfile          (Request)

        /// <summary>
        /// SetChargingProfile the given charge box.
        /// </summary>
        /// <param name="Request">A SetChargingProfile request.</param>
        public async Task<CP.SetChargingProfileResponse> SetChargingProfile(SetChargingProfileRequest Request)
        {

            #region Send OnSetChargingProfileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetChargingProfileRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSetChargingProfileRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomSetChargingProfileRequestSerializer,
                                          CustomChargingProfileSerializer,
                                          CustomChargingScheduleSerializer,
                                          CustomChargingSchedulePeriodSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SetChargingProfile(Request)

                                      : new CP.SetChargingProfileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.SetChargingProfileResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomSetChargingProfileResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnSetChargingProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetChargingProfileResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSetChargingProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearChargingProfile        (Request)

        /// <summary>
        /// ClearChargingProfile the given charge box.
        /// </summary>
        /// <param name="Request">A ClearChargingProfile request.</param>
        public async Task<CP.ClearChargingProfileResponse> ClearChargingProfile(ClearChargingProfileRequest Request)
        {

            #region Send OnClearChargingProfileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearChargingProfileRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnClearChargingProfileRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomClearChargingProfileRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.ClearChargingProfile(Request)

                                      : new CP.ClearChargingProfileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.ClearChargingProfileResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomClearChargingProfileResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnClearChargingProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearChargingProfileResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnClearChargingProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCompositeSchedule        (Request)

        /// <summary>
        /// GetCompositeSchedule the given charge box.
        /// </summary>
        /// <param name="Request">A GetCompositeSchedule request.</param>
        public async Task<CP.GetCompositeScheduleResponse> GetCompositeSchedule(GetCompositeScheduleRequest Request)
        {

            #region Send OnGetCompositeScheduleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetCompositeScheduleRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetCompositeScheduleRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetCompositeScheduleRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetCompositeSchedule(Request)

                                      : new CP.GetCompositeScheduleResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.GetCompositeScheduleResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetCompositeScheduleResponseSerializer,
                    CustomChargingScheduleSerializer,
                    CustomChargingSchedulePeriodSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetCompositeScheduleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetCompositeScheduleResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetCompositeScheduleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UnlockConnector             (Request)

        /// <summary>
        /// UnlockConnector the given charge box.
        /// </summary>
        /// <param name="Request">A UnlockConnector request.</param>
        public async Task<CP.UnlockConnectorResponse> UnlockConnector(UnlockConnectorRequest Request)
        {

            #region Send OnUnlockConnectorRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUnlockConnectorRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnUnlockConnectorRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomUnlockConnectorRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.UnlockConnector(Request)

                                      : new CP.UnlockConnectorResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.UnlockConnectorResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomUnlockConnectorResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnUnlockConnectorResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUnlockConnectorResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnUnlockConnectorResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetLocalListVersion         (Request)

        /// <summary>
        /// GetLocalListVersion the given charge box.
        /// </summary>
        /// <param name="Request">A GetLocalListVersion request.</param>
        public async Task<CP.GetLocalListVersionResponse> GetLocalListVersion(GetLocalListVersionRequest Request)
        {

            #region Send OnGetLocalListVersionRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetLocalListVersionRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetLocalListVersionRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetLocalListVersionRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetLocalListVersion(Request)

                                      : new CP.GetLocalListVersionResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.GetLocalListVersionResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetLocalListVersionResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetLocalListVersionResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetLocalListVersionResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetLocalListVersionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendLocalList               (Request)

        /// <summary>
        /// SendLocalList the given charge box.
        /// </summary>
        /// <param name="Request">A SendLocalList request.</param>
        public async Task<CP.SendLocalListResponse> SendLocalList(SendLocalListRequest Request)
        {

            #region Send OnSendLocalListRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSendLocalListRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSendLocalListRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomSendLocalListRequestSerializer,
                                          CustomAuthorizationDataSerializer,
                                          CustomIdTagInfoSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SendLocalList(Request)

                                      : new CP.SendLocalListResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.SendLocalListResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomSendLocalListResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnSendLocalListResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSendLocalListResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSendLocalListResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearCache                  (Request)

        /// <summary>
        /// ClearCache the given charge box.
        /// </summary>
        /// <param name="Request">A ClearCache request.</param>
        public async Task<CP.ClearCacheResponse> ClearCache(ClearCacheRequest Request)
        {

            #region Send OnClearCacheRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearCacheRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnClearCacheRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomClearCacheRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.ClearCache(Request)

                                      : new CP.ClearCacheResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.ClearCacheResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomClearCacheResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnClearCacheResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearCacheResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnClearCacheResponse));
            }

            #endregion

            return response;

        }

        #endregion


        // Security extensions

        #region CertificateSigned           (Request)

        /// <summary>
        /// CertificateSigned the given charge box.
        /// </summary>
        /// <param name="Request">A CertificateSigned request.</param>
        public async Task<CP.CertificateSignedResponse> CertificateSigned(CertificateSignedRequest Request)
        {

            #region Send OnCertificateSignedRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnCertificateSignedRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnCertificateSignedRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomCertificateSignedRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.CertificateSigned(Request)

                                      : new CP.CertificateSignedResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.CertificateSignedResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomCertificateSignedResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnCertificateSignedResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnCertificateSignedResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnCertificateSignedResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteCertificate           (Request)

        /// <summary>
        /// DeleteCertificate the given charge box.
        /// </summary>
        /// <param name="Request">A DeleteCertificate request.</param>
        public async Task<CP.DeleteCertificateResponse> DeleteCertificate(DeleteCertificateRequest Request)
        {

            #region Send OnDeleteCertificateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDeleteCertificateRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDeleteCertificateRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomDeleteCertificateRequestSerializer,
                                          CustomCertificateHashDataSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.DeleteCertificate(Request)

                                      : new CP.DeleteCertificateResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.DeleteCertificateResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomDeleteCertificateResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnDeleteCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDeleteCertificateResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDeleteCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ExtendedTriggerMessage      (Request)

        /// <summary>
        /// ExtendedTriggerMessage the given charge box.
        /// </summary>
        /// <param name="Request">A ExtendedTriggerMessage request.</param>
        public async Task<CP.ExtendedTriggerMessageResponse> ExtendedTriggerMessage(ExtendedTriggerMessageRequest Request)
        {

            #region Send OnExtendedTriggerMessageRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnExtendedTriggerMessageRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnExtendedTriggerMessageRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomExtendedTriggerMessageRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.ExtendedTriggerMessage(Request)

                                      : new CP.ExtendedTriggerMessageResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.ExtendedTriggerMessageResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomExtendedTriggerMessageResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnExtendedTriggerMessageResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnExtendedTriggerMessageResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnExtendedTriggerMessageResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetInstalledCertificateIds  (Request)

        /// <summary>
        /// GetInstalledCertificateIds the given charge box.
        /// </summary>
        /// <param name="Request">A GetInstalledCertificateIds request.</param>
        public async Task<CP.GetInstalledCertificateIdsResponse> GetInstalledCertificateIds(GetInstalledCertificateIdsRequest Request)
        {

            #region Send OnGetInstalledCertificateIdsRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetInstalledCertificateIdsRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetInstalledCertificateIdsRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetInstalledCertificateIdsRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetInstalledCertificateIds(Request)

                                      : new CP.GetInstalledCertificateIdsResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.GetInstalledCertificateIdsResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetInstalledCertificateIdsResponseSerializer,
                    CustomCertificateHashDataSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetInstalledCertificateIdsResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetInstalledCertificateIdsResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetInstalledCertificateIdsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetLog                      (Request)

        /// <summary>
        /// GetLog the given charge box.
        /// </summary>
        /// <param name="Request">A GetLog request.</param>
        public async Task<CP.GetLogResponse> GetLog(GetLogRequest Request)
        {

            #region Send OnGetLogRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetLogRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetLogRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetLogRequestSerializer,
                                          CustomLogParametersSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetLog(Request)

                                      : new CP.GetLogResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.GetLogResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomGetLogResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetLogResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetLogResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetLogResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region InstallCertificate          (Request)

        /// <summary>
        /// InstallCertificate the given charge box.
        /// </summary>
        /// <param name="Request">A InstallCertificate request.</param>
        public async Task<CP.InstallCertificateResponse> InstallCertificate(InstallCertificateRequest Request)
        {

            #region Send OnInstallCertificateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnInstallCertificateRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnInstallCertificateRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomInstallCertificateRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.InstallCertificate(Request)

                                      : new CP.InstallCertificateResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.InstallCertificateResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomInstallCertificateResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnInstallCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnInstallCertificateResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnInstallCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SignedUpdateFirmware        (Request)

        /// <summary>
        /// SignedUpdateFirmware the given charge box.
        /// </summary>
        /// <param name="Request">A SignedUpdateFirmware request.</param>
        public async Task<CP.SignedUpdateFirmwareResponse> SignedUpdateFirmware(SignedUpdateFirmwareRequest Request)
        {

            #region Send OnSignedUpdateFirmwareRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSignedUpdateFirmwareRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSignedUpdateFirmwareRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomSignedUpdateFirmwareRequestSerializer,
                                          CustomFirmwareImageSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SignedUpdateFirmware(Request)

                                      : new CP.SignedUpdateFirmwareResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new CP.SignedUpdateFirmwareResponse(
                                      Request,
                                      Result.Server("Unknown or unreachable charge box!")
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomSignedUpdateFirmwareResponseSerializer,
                    CustomSignatureSerializer,
                    CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnSignedUpdateFirmwareResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSignedUpdateFirmwareResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSignedUpdateFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion


        // Binary Data Streams Extensions

        #region BinaryDataTransfer          (Request)

        /// <summary>
        /// Transfer the given data to the given charging station.
        /// </summary>
        /// <param name="Request">A BinaryDataTransfer request.</param>
        public async Task<BinaryDataTransferResponse> BinaryDataTransfer(BinaryDataTransferRequest Request)
        {

            #region Send OnBinaryDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnBinaryDataTransferRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnBinaryDataTransferRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToBinary(
                                          CustomBinaryDataTransferRequestSerializer,
                                          CustomBinarySignatureSerializer,
                                          IncludeSignatures: false
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.BinaryDataTransfer(Request)

                                      : new BinaryDataTransferResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new BinaryDataTransferResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToBinary(
                    CustomBinaryDataTransferResponseSerializer,
                    null, // CustomStatusInfoSerializer
                    CustomBinarySignatureSerializer,
                    IncludeSignatures: false
                ),
                out errorResponse
            );


            #region Send OnBinaryDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnBinaryDataTransferResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnBinaryDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetFile                     (Request)

        /// <summary>
        /// Request the given file from the charging station.
        /// </summary>
        /// <param name="Request">A GetFile request.</param>
        public async Task<OCPP.CS.GetFileResponse> GetFile(GetFileRequest Request)
        {

            #region Send OnGetFileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetFileRequest?.Invoke(startTime,
                                         this,
                                         Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetFileRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomGetFileRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.GetFile(Request)

                                      : new OCPP.CS.GetFileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.GetFileResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToBinary(
                    CustomGetFileResponseSerializer,
                    null, // CustomStatusInfoSerializer
                    CustomBinarySignatureSerializer,
                    IncludeSignatures: false
                ),
                out errorResponse
            );


            #region Send OnGetFileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetFileResponse?.Invoke(endTime,
                                          this,
                                          Request,
                                          response,
                                          endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnGetFileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendFile                    (Request)

        /// <summary>
        /// Request the given file from the charging station.
        /// </summary>
        /// <param name="Request">A SendFile request.</param>
        public async Task<OCPP.CS.SendFileResponse> SendFile(SendFileRequest Request)
        {

            #region Send OnSendFileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSendFileRequest?.Invoke(startTime,
                                          this,
                                          Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSendFileRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToBinary(
                                          CustomSendFileRequestSerializer,
                                          CustomBinarySignatureSerializer,
                                          IncludeSignatures: false
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SendFile(Request)

                                      : new OCPP.CS.SendFileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.SendFileResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomSendFileResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer
                ),
                out errorResponse
            );


            #region Send OnSendFileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSendFileResponse?.Invoke(endTime,
                                           this,
                                           Request,
                                           response,
                                           endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSendFileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteFile                  (Request)

        /// <summary>
        /// Delete the given file from the charging station.
        /// </summary>
        /// <param name="Request">A DeleteFile request.</param>
        public async Task<OCPP.CS.DeleteFileResponse> DeleteFile(DeleteFileRequest Request)
        {

            #region Send OnDeleteFileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDeleteFileRequest?.Invoke(startTime,
                                            this,
                                            Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDeleteFileRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomDeleteFileRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.DeleteFile(Request)

                                      : new OCPP.CS.DeleteFileResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.DeleteFileResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomDeleteFileResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer
                ),
                out errorResponse
            );


            #region Send OnDeleteFileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDeleteFileResponse?.Invoke(endTime,
                                             this,
                                             Request,
                                             response,
                                             endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDeleteFileResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ListDirectory               (Request)

        /// <summary>
        /// Delete the given file from the charging station.
        /// </summary>
        /// <param name="Request">A ListDirectory request.</param>
        public async Task<OCPP.CS.ListDirectoryResponse> ListDirectory(ListDirectoryRequest Request)
        {

            #region Send OnListDirectoryRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnListDirectoryRequest?.Invoke(startTime,
                                               this,
                                               Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnListDirectoryRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          CustomListDirectoryRequestSerializer,
                                          CustomSignatureSerializer,
                                          CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.ListDirectory(Request)

                                      : new OCPP.CS.ListDirectoryResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.ListDirectoryResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    CustomListDirectoryResponseSerializer,
                    CustomStatusInfoSerializer,
                    CustomSignatureSerializer
                ),
                out errorResponse
            );


            #region Send OnListDirectoryResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnListDirectoryResponse?.Invoke(endTime,
                                                this,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnListDirectoryResponse));
            }

            #endregion

            return response;

        }

        #endregion



        // E2E Security Extensions

        #region AddSignaturePolicy          (Request)

        /// <summary>
        /// Add a signature policy.
        /// </summary>
        /// <param name="Request">An AddSignaturePolicy request.</param>
        public async Task<OCPP.CS.AddSignaturePolicyResponse> AddSignaturePolicy(AddSignaturePolicyRequest Request)
        {

            #region Send OnAddSignaturePolicyRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnAddSignaturePolicyRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnAddSignaturePolicyRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          //CustomAddSignaturePolicyRequestSerializer,
                                          //CustomMessageInfoSerializer,
                                          //CustomMessageContentSerializer,
                                          //CustomComponentSerializer,
                                          //CustomEVSESerializer,
                                          //CustomSignatureSerializer,
                                          //CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.AddSignaturePolicy(Request)

                                      : new OCPP.CS.AddSignaturePolicyResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.AddSignaturePolicyResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    //CustomAddSignaturePolicyResponseSerializer,
                    //CustomStatusInfoSerializer,
                    //CustomSignatureSerializer,
                    //CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnAddSignaturePolicyResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnAddSignaturePolicyResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnAddSignaturePolicyResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UpdateSignaturePolicy       (Request)

        /// <summary>
        /// Set a display message.
        /// </summary>
        /// <param name="Request">A UpdateSignaturePolicy request.</param>
        public async Task<OCPP.CS.UpdateSignaturePolicyResponse> UpdateSignaturePolicy(UpdateSignaturePolicyRequest Request)
        {

            #region Send OnUpdateSignaturePolicyRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateSignaturePolicyRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnUpdateSignaturePolicyRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          //CustomUpdateSignaturePolicyRequestSerializer,
                                          //CustomMessageInfoSerializer,
                                          //CustomMessageContentSerializer,
                                          //CustomComponentSerializer,
                                          //CustomEVSESerializer,
                                          //CustomSignatureSerializer,
                                          //CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.UpdateSignaturePolicy(Request)

                                      : new OCPP.CS.UpdateSignaturePolicyResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.UpdateSignaturePolicyResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    //CustomUpdateSignaturePolicyResponseSerializer,
                    //CustomStatusInfoSerializer,
                    //CustomSignatureSerializer,
                    //CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnUpdateSignaturePolicyResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUpdateSignaturePolicyResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnUpdateSignaturePolicyResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteSignaturePolicy       (Request)

        /// <summary>
        /// Set a display message.
        /// </summary>
        /// <param name="Request">A DeleteSignaturePolicy request.</param>
        public async Task<OCPP.CS.DeleteSignaturePolicyResponse> DeleteSignaturePolicy(DeleteSignaturePolicyRequest Request)
        {

            #region Send OnDeleteSignaturePolicyRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDeleteSignaturePolicyRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDeleteSignaturePolicyRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          //CustomDeleteSignaturePolicyRequestSerializer,
                                          //CustomMessageInfoSerializer,
                                          //CustomMessageContentSerializer,
                                          //CustomComponentSerializer,
                                          //CustomEVSESerializer,
                                          //CustomSignatureSerializer,
                                          //CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.DeleteSignaturePolicy(Request)

                                      : new OCPP.CS.DeleteSignaturePolicyResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.DeleteSignaturePolicyResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    //CustomDeleteSignaturePolicyResponseSerializer,
                    //CustomStatusInfoSerializer,
                    //CustomSignatureSerializer,
                    //CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnDeleteSignaturePolicyResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDeleteSignaturePolicyResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDeleteSignaturePolicyResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region AddUserRole                 (Request)

        /// <summary>
        /// Set a display message.
        /// </summary>
        /// <param name="Request">A AddUserRole request.</param>
        public async Task<OCPP.CS.AddUserRoleResponse> AddUserRole(AddUserRoleRequest Request)
        {

            #region Send OnAddUserRoleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnAddUserRoleRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnAddUserRoleRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          //CustomAddUserRoleRequestSerializer,
                                          //CustomMessageInfoSerializer,
                                          //CustomMessageContentSerializer,
                                          //CustomComponentSerializer,
                                          //CustomEVSESerializer,
                                          //CustomSignatureSerializer,
                                          //CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.AddUserRole(Request)

                                      : new OCPP.CS.AddUserRoleResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.AddUserRoleResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    //CustomAddUserRoleResponseSerializer,
                    //CustomStatusInfoSerializer,
                    //CustomSignatureSerializer,
                    //CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnAddUserRoleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnAddUserRoleResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnAddUserRoleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UpdateUserRole              (Request)

        /// <summary>
        /// Set a display message.
        /// </summary>
        /// <param name="Request">A UpdateUserRole request.</param>
        public async Task<OCPP.CS.UpdateUserRoleResponse> UpdateUserRole(UpdateUserRoleRequest Request)
        {

            #region Send OnUpdateUserRoleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateUserRoleRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnUpdateUserRoleRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          //CustomUpdateUserRoleRequestSerializer,
                                          //CustomMessageInfoSerializer,
                                          //CustomMessageContentSerializer,
                                          //CustomComponentSerializer,
                                          //CustomEVSESerializer,
                                          //CustomSignatureSerializer,
                                          //CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.UpdateUserRole(Request)

                                      : new OCPP.CS.UpdateUserRoleResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.UpdateUserRoleResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    //CustomUpdateUserRoleResponseSerializer,
                    //CustomStatusInfoSerializer,
                    //CustomSignatureSerializer,
                    //CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnUpdateUserRoleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUpdateUserRoleResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnUpdateUserRoleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteUserRole              (Request)

        /// <summary>
        /// Set a display message.
        /// </summary>
        /// <param name="Request">A DeleteUserRole request.</param>
        public async Task<OCPP.CS.DeleteUserRoleResponse> DeleteUserRole(DeleteUserRoleRequest Request)
        {

            #region Send OnDeleteUserRoleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDeleteUserRoleRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDeleteUserRoleRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToJSON(
                                          //CustomDeleteUserRoleRequestSerializer,
                                          //CustomMessageInfoSerializer,
                                          //CustomMessageContentSerializer,
                                          //CustomComponentSerializer,
                                          //CustomEVSESerializer,
                                          //CustomSignatureSerializer,
                                          //CustomCustomDataSerializer
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.DeleteUserRole(Request)

                                      : new OCPP.CS.DeleteUserRoleResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new OCPP.CS.DeleteUserRoleResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    //CustomDeleteUserRoleResponseSerializer,
                    //CustomStatusInfoSerializer,
                    //CustomSignatureSerializer,
                    //CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnDeleteUserRoleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDeleteUserRoleResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDeleteUserRoleResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region SecureDataTransfer          (Request)

        /// <summary>
        /// Transfer the given data to the given charging station.
        /// </summary>
        /// <param name="Request">A SecureDataTransfer request.</param>
        public async Task<SecureDataTransferResponse> SecureDataTransfer(SecureDataTransferRequest Request)
        {

            #region Send OnSecureDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSecureDataTransferRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSecureDataTransferRequest));
            }

            #endregion


            var response  = reachableChargeBoxes.TryGetValue(Request.DestinationNodeId, out var centralSystem) &&
                                centralSystem is not null

                                ? SignaturePolicy.SignRequestMessage(
                                      Request,
                                      Request.ToBinary(
                                          CustomSecureDataTransferRequestSerializer,
                                          CustomBinarySignatureSerializer,
                                          IncludeSignatures: false
                                      ),
                                      out var errorResponse
                                  )

                                      ? await centralSystem.Item1.SecureDataTransfer(Request)

                                      : new SecureDataTransferResponse(
                                            Request,
                                            Result.SignatureError(errorResponse)
                                        )

                                : new SecureDataTransferResponse(
                                      Request,
                                      Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                  );


            SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToBinary(
                    CustomSecureDataTransferResponseSerializer,
                    // CustomStatusInfoSerializer
                    CustomBinarySignatureSerializer,
                    IncludeSignatures: false
                ),
                out errorResponse
            );


            #region Send OnSecureDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSecureDataTransferResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnSecureDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #endregion



        #region Shutdown(Message, Wait = true)

        /// <summary>
        /// Shutdown the HTTP web socket listener thread.
        /// </summary>
        /// <param name="Message">An optional shutdown message.</param>
        /// <param name="Wait">Wait until the server finally shutted down.</param>
        public async Task Shutdown(String?  Message   = null,
                                   Boolean  Wait      = true)
        {

            var centralSystemServersCopy = centralSystemServers.ToArray();
            if (centralSystemServersCopy.Length > 0)
            {
                try
                {

                    await Task.WhenAll(centralSystemServers.
                                           Select(centralSystemServer => centralSystemServer.Shutdown(
                                                                             Message,
                                                                             Wait
                                                                         )).
                                           ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(TestCentralSystem),
                              nameof(Shutdown),
                              e
                          );
                }
            }

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


    }

}
