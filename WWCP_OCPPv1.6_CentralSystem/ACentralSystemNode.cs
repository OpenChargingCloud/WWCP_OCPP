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

using BCx509 = Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.NetworkingNode;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CentralSystem
{

    /// <summary>
    /// An abstract Charging Station Management System node.
    /// </summary>
    public abstract class ACentralSystemNode : AOCPPNetworkingNode,
                                               ICentralSystemNode
    {

        #region Data

        protected        readonly  ConcurrentDictionary<IdToken, IdTagInfo>                                      idTags                       = [];

        private          readonly  HashSet<SignaturePolicy>                                                      signaturePolicies            = [];

        //private          readonly  ConcurrentDictionary<NetworkingNode_Id, Tuple<CSMS.ICSMSChannel, DateTime>>   connectedNetworkingNodes     = [];

        protected static readonly  SemaphoreSlim                                                                 ChargingStationSemaphore     = new (1, 1);

        private          readonly  TimeSpan                                                                      defaultRequestTimeout        = TimeSpan.FromSeconds(30);

        #endregion

        #region Properties

        /// <summary>
        /// The Charging Station Management System vendor identification.
        /// </summary>
        [Mandatory]
        public String                      VendorName                        { get; }      = "";

        /// <summary>
        ///  The Charging Station Management System model identification.
        /// </summary>
        [Mandatory]
        public String                      Model                             { get; }      = "";

        /// <summary>
        /// The optional serial number of the Charging Station Management System.
        /// </summary>
        [Optional]
        public String?                     SerialNumber                      { get; }

        /// <summary>
        /// The optional firmware version of the Charging Station Management System.
        /// </summary>
        [Optional]
        public String?                     SoftwareVersion                   { get; }


        public AsymmetricCipherKeyPair?    ClientCAKeyPair                   { get; }
        public BCx509.X509Certificate?     ClientCACertificate               { get; }



        /// <summary>
        /// The time at the CSMS.
        /// </summary>
        public DateTime?                   CSMSTime                          { get; set; } = Timestamp.Now;


        //public HTTPAPI?                    HTTPAPI                           { get; }

        //public DownloadAPI?                HTTPDownloadAPI                   { get; }
        //public HTTPPath?                   HTTPDownloadAPI_Path              { get; }
        //public String?                     HTTPDownloadAPI_FileSystemPath    { get; }

        //public UploadAPI?                  HTTPUploadAPI                     { get; }
        //public HTTPPath?                   HTTPUploadAPI_Path                { get; }
        //public String?                     HTTPUploadAPI_FileSystemPath      { get; }

        //public QRCodeAPI?                  QRCodeAPI                         { get; }
        //public HTTPPath?                   QRCodeAPI_Path                    { get; }

        //public WebAPI?                     WebAPI                            { get; }
        //public HTTPPath?                   WebAPI_Path                       { get; }

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

        ///// <summary>
        ///// An event sent whenever a JSON message request was received.
        ///// </summary>
        //public event OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestReceived;

        ///// <summary>
        ///// An event sent whenever the response to a JSON message was sent.
        ///// </summary>
        //public event OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseSent;

        ///// <summary>
        ///// An event sent whenever the error response to a JSON message was sent.
        ///// </summary>
        //public event OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseSent;


        ///// <summary>
        ///// An event sent whenever a JSON message request was sent.
        ///// </summary>
        //public event OnWebSocketJSONMessageRequestDelegate?     OnJSONMessageRequestSent;

        ///// <summary>
        ///// An event sent whenever the response to a JSON message request was received.
        ///// </summary>
        //public event OnWebSocketJSONMessageResponseDelegate?    OnJSONMessageResponseReceived;

        ///// <summary>
        ///// An event sent whenever an error response to a JSON message request was received.
        ///// </summary>
        //public event OnWebSocketTextErrorResponseDelegate?      OnJSONErrorResponseReceived;

        #endregion

        #region Generic Binary Messages

        ///// <summary>
        ///// An event sent whenever a binary message request was received.
        ///// </summary>
        //public event OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestReceived;

        ///// <summary>
        ///// An event sent whenever the response to a binary message was sent.
        ///// </summary>
        //public event OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseSent;

        ///// <summary>
        ///// An event sent whenever the error response to a binary message was sent.
        ///// </summary>
        ////public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseSent;


        ///// <summary>
        ///// An event sent whenever a binary message request was sent.
        ///// </summary>
        //public event OnWebSocketBinaryMessageRequestDelegate?     OnBinaryMessageRequestSent;

        ///// <summary>
        ///// An event sent whenever the response to a binary message request was received.
        ///// </summary>
        //public event OnWebSocketBinaryMessageResponseDelegate?    OnBinaryMessageResponseReceived;

        ///// <summary>
        ///// An event sent whenever the error response to a binary message request was sent.
        ///// </summary>
        ////public event OnWebSocketBinaryErrorResponseDelegate?      OnBinaryErrorResponseReceived;

        #endregion


        #region CSMS <- Charging Station Messages

        //// Certificates

        //#region OnSignCertificate

        ///// <summary>
        ///// An event sent whenever a SignCertificate request was received.
        ///// </summary>
        //public event OnSignCertificateRequestDelegate?   OnSignCertificateRequest;

        ///// <summary>
        ///// An event sent whenever a response to a SignCertificate request was sent.
        ///// </summary>
        //public event OnSignCertificateResponseDelegate?  OnSignCertificateResponse;

        //#endregion


        //// Charging

        //#region OnAuthorize

        ///// <summary>
        ///// An event sent whenever an Authorize request was received.
        ///// </summary>
        //public event OnAuthorizeRequestReceivedDelegate?   OnAuthorizeRequestReceived;

        ///// <summary>
        ///// An event sent whenever a response to an Authorize request was sent.
        ///// </summary>
        //public event OnAuthorizeResponseSentDelegate?      OnAuthorizeResponseSent;

        //#endregion

        //#region OnMeterValues

        ///// <summary>
        ///// An event sent whenever a MeterValues request was received.
        ///// </summary>
        //public event OnMeterValuesRequestDelegate?   OnMeterValuesRequest;

        ///// <summary>
        ///// An event sent whenever a response to a MeterValues request was sent.
        ///// </summary>
        //public event OnMeterValuesResponseDelegate?  OnMeterValuesResponse;

        //#endregion

        //#region OnStartTransaction

        ///// <summary>
        ///// An event sent whenever a StartTransaction request was received.
        ///// </summary>
        //public event OnStartTransactionRequestDelegate?   OnStartTransactionRequest;

        ///// <summary>
        ///// An event sent whenever a response to a StartTransaction request was sent.
        ///// </summary>
        //public event OnStartTransactionResponseDelegate?  OnStartTransactionResponse;

        //#endregion

        //#region OnStatusNotification

        ///// <summary>
        ///// An event sent whenever a StatusNotification request was received.
        ///// </summary>
        //public event OnStatusNotificationRequestDelegate?   OnStatusNotificationRequest;

        ///// <summary>
        ///// An event sent whenever a response to a StatusNotification request was sent.
        ///// </summary>
        //public event OnStatusNotificationResponseDelegate?  OnStatusNotificationResponse;

        //#endregion

        //#region OnStopTransaction

        ///// <summary>
        ///// An event sent whenever a StopTransaction request was received.
        ///// </summary>
        //public event OnStopTransactionRequestDelegate?   OnStopTransactionRequest;

        ///// <summary>
        ///// An event sent whenever a response to a StopTransaction request was sent.
        ///// </summary>
        //public event OnStopTransactionResponseDelegate?  OnStopTransactionResponse;

        //#endregion


        //// Firmware

        //#region OnBootNotification

        ///// <summary>
        ///// An event sent whenever a BootNotification request was received.
        ///// </summary>
        //public event OnBootNotificationRequestDelegate?   OnBootNotificationRequest;

        ///// <summary>
        ///// An event sent whenever a response to a BootNotification request was sent.
        ///// </summary>
        //public event OnBootNotificationResponseDelegate?  OnBootNotificationResponse;

        //#endregion

        //#region OnFirmwareStatusNotification

        ///// <summary>
        ///// An event sent whenever a FirmwareStatusNotification request was received.
        ///// </summary>
        //public event OnFirmwareStatusNotificationRequestDelegate?   OnFirmwareStatusNotificationRequest;

        ///// <summary>
        ///// An event sent whenever a response to a FirmwareStatusNotification request was sent.
        ///// </summary>
        //public event OnFirmwareStatusNotificationResponseDelegate?  OnFirmwareStatusNotificationResponse;

        //#endregion

        //#region OnHeartbeat

        ///// <summary>
        ///// An event sent whenever a Heartbeat request was received.
        ///// </summary>
        //public event OnHeartbeatRequestDelegate?   OnHeartbeatRequest;

        ///// <summary>
        ///// An event sent whenever a response to a Heartbeat request was sent.
        ///// </summary>
        //public event OnHeartbeatResponseDelegate?  OnHeartbeatResponse;

        //#endregion

        //#region OnSignedFirmwareStatusNotification

        ///// <summary>
        ///// An event sent whenever a SignedFirmwareStatusNotification request was received.
        ///// </summary>
        //public event OnSignedFirmwareStatusNotificationRequestDelegate?   OnSignedFirmwareStatusNotificationRequest;

        ///// <summary>
        ///// An event sent whenever a response to a SignedFirmwareStatusNotification request was sent.
        ///// </summary>
        //public event OnSignedFirmwareStatusNotificationResponseDelegate?  OnSignedFirmwareStatusNotificationResponse;

        //#endregion


        //// Monitoring

        //#region OnDiagnosticsStatusNotification

        ///// <summary>
        ///// An event sent whenever a DiagnosticsStatusNotification request was received.
        ///// </summary>
        //public event OnDiagnosticsStatusNotificationRequestDelegate?   OnDiagnosticsStatusNotificationRequest;

        ///// <summary>
        ///// An event sent whenever a response to a DiagnosticsStatusNotification request was sent.
        ///// </summary>
        //public event OnDiagnosticsStatusNotificationResponseDelegate?  OnDiagnosticsStatusNotificationResponse;

        //#endregion

        //#region OnLogStatusNotification

        ///// <summary>
        ///// An event sent whenever a LogStatusNotification request was received.
        ///// </summary>
        //public event OnLogStatusNotificationRequestDelegate?   OnLogStatusNotificationRequest;

        ///// <summary>
        ///// An event sent whenever a response to a LogStatusNotification request was sent.
        ///// </summary>
        //public event OnLogStatusNotificationResponseDelegate?  OnLogStatusNotificationResponse;

        //#endregion

        //#region OnSecurityEventNotification

        ///// <summary>
        ///// An event sent whenever a SecurityEventNotification request was received.
        ///// </summary>
        //public event OnSecurityEventNotificationRequestDelegate?   OnSecurityEventNotificationRequest;

        ///// <summary>
        ///// An event sent whenever a response to a SecurityEventNotification request was sent.
        ///// </summary>
        //public event OnSecurityEventNotificationResponseDelegate?  OnSecurityEventNotificationResponse;

        //#endregion


        //#region OnIncomingDataTransfer

        ///// <summary>
        ///// An event sent whenever an IncomingDataTransfer request was received.
        ///// </summary>
        //public event OnIncomingDataTransferRequestDelegate?   OnIncomingDataTransferRequest;

        ///// <summary>
        ///// An event sent whenever a response to an IncomingDataTransfer request was sent.
        ///// </summary>
        //public event OnIncomingDataTransferResponseDelegate?  OnIncomingDataTransferResponse;

        //#endregion



        //// Binary Data Streams Extensions

        //#region OnIncomingBinaryDataTransfer (Request/-Response)

        /////// <summary>
        /////// An event sent whenever an IncomingBinaryDataTransfer request was received.
        /////// </summary>
        ////public event OnBinaryDataTransferRequestReceivedDelegate?   OnBinaryDataTransferRequestReceived;

        /////// <summary>
        /////// An event sent whenever a response to an IncomingBinaryDataTransfer request was sent.
        /////// </summary>
        ////public event OnBinaryDataTransferResponseSentDelegate?  OnBinaryDataTransferResponseSent;

        //#endregion

        #endregion

        #region CSMS -> Charging Station Messages

        //// Certificates

        //#region OnCertificateSigned

        ///// <summary>
        ///// An event sent whenever a CertificateSigned request was sent.
        ///// </summary>
        //public event CS.OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

        ///// <summary>
        ///// An event sent whenever a response to a CertificateSigned request was sent.
        ///// </summary>
        //public event CS.OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

        //#endregion

        //#region OnDeleteCertificate

        ///// <summary>
        ///// An event sent whenever a DeleteCertificate request was sent.
        ///// </summary>
        //public event CS.OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

        ///// <summary>
        ///// An event sent whenever a response to a DeleteCertificate request was sent.
        ///// </summary>
        //public event CS.OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

        //#endregion

        //#region OnGetInstalledCertificateIds

        ///// <summary>
        ///// An event sent whenever a GetInstalledCertificateIds request was sent.
        ///// </summary>
        //public event CS.OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

        ///// <summary>
        ///// An event sent whenever a response to a GetInstalledCertificateIds request was sent.
        ///// </summary>
        //public event CS.OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

        //#endregion

        //#region OnInstallCertificate

        ///// <summary>
        ///// An event sent whenever an InstallCertificate request was sent.
        ///// </summary>
        //public event CS.OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

        ///// <summary>
        ///// An event sent whenever a response to an InstallCertificate request was sent.
        ///// </summary>
        //public event CS.OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

        //#endregion


        //// Charging

        //#region OnCancelReservation

        ///// <summary>
        ///// An event sent whenever a CancelReservation request was sent.
        ///// </summary>
        //public event CS.OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        ///// <summary>
        ///// An event sent whenever a response to a CancelReservation request was sent.
        ///// </summary>
        //public event CS.OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        //#endregion

        //#region OnClearChargingProfile

        ///// <summary>
        ///// An event sent whenever a ClearChargingProfile request was sent.
        ///// </summary>
        //public event CS.OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        ///// <summary>
        ///// An event sent whenever a response to a ClearChargingProfile request was sent.
        ///// </summary>
        //public event CS.OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        //#endregion

        //#region OnGetCompositeSchedule

        ///// <summary>
        ///// An event sent whenever a GetCompositeSchedule request was sent.
        ///// </summary>
        //public event CS.OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        ///// <summary>
        ///// An event sent whenever a response to a GetCompositeSchedule request was sent.
        ///// </summary>
        //public event CS.OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        //#endregion

        //#region OnRemoteStartTransaction

        ///// <summary>
        ///// An event sent whenever a RemoteStartTransaction request was sent.
        ///// </summary>
        //public event CS.OnRemoteStartTransactionRequestDelegate?   OnRemoteStartTransactionRequest;

        ///// <summary>
        ///// An event sent whenever a response to a RemoteStartTransaction request was sent.
        ///// </summary>
        //public event CS.OnRemoteStartTransactionResponseDelegate?  OnRemoteStartTransactionResponse;

        //#endregion

        //#region OnRemoteStopTransaction

        ///// <summary>
        ///// An event sent whenever a RemoteStopTransaction request was sent.
        ///// </summary>
        //public event CS.OnRemoteStopTransactionRequestDelegate?   OnRemoteStopTransactionRequest;

        ///// <summary>
        ///// An event sent whenever a response to a RemoteStopTransaction request was sent.
        ///// </summary>
        //public event CS.OnRemoteStopTransactionResponseDelegate?  OnRemoteStopTransactionResponse;

        //#endregion

        //#region OnReserveNow

        ///// <summary>
        ///// An event sent whenever a ReserveNow request was sent.
        ///// </summary>
        //public event CS.OnReserveNowRequestDelegate?   OnReserveNowRequest;

        ///// <summary>
        ///// An event sent whenever a response to a ReserveNow request was sent.
        ///// </summary>
        //public event CS.OnReserveNowResponseDelegate?  OnReserveNowResponse;

        //#endregion

        //#region OnSetChargingProfile

        ///// <summary>
        ///// An event sent whenever a SetChargingProfile request was sent.
        ///// </summary>
        //public event CS.OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        ///// <summary>
        ///// An event sent whenever a response to a SetChargingProfile request was sent.
        ///// </summary>
        //public event CS.OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        //#endregion

        //#region OnUnlockConnector

        ///// <summary>
        ///// An event sent whenever an UnlockConnector request was sent.
        ///// </summary>
        //public event CS.OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        ///// <summary>
        ///// An event sent whenever a response to an UnlockConnector request was sent.
        ///// </summary>
        //public event CS.OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        //#endregion


        //// Firmware

        //#region OnReset

        ///// <summary>
        ///// An event sent whenever a Reset request was sent.
        ///// </summary>
        //public event CS.OnResetRequestDelegate?   OnResetRequest;

        ///// <summary>
        ///// An event sent whenever a response to a Reset request was sent.
        ///// </summary>
        //public event CS.OnResetResponseDelegate?  OnResetResponse;

        //#endregion

        //#region OnSignedUpdateFirmware

        ///// <summary>
        ///// An event sent whenever a SignedUpdateFirmware request was sent.
        ///// </summary>
        //public event CS.OnSignedUpdateFirmwareRequestDelegate?   OnSignedUpdateFirmwareRequest;

        ///// <summary>
        ///// An event sent whenever a response to a SignedUpdateFirmware request was sent.
        ///// </summary>
        //public event CS.OnSignedUpdateFirmwareResponseDelegate?  OnSignedUpdateFirmwareResponse;

        //#endregion

        //#region OnUpdateFirmware

        ///// <summary>
        ///// An event sent whenever an UpdateFirmware request was sent.
        ///// </summary>
        //public event CS.OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        ///// <summary>
        ///// An event sent whenever a response to an UpdateFirmware request was sent.
        ///// </summary>
        //public event CS.OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        //#endregion


        //// LocalList

        //#region OnClearCache

        ///// <summary>
        ///// An event sent whenever a ClearCache request was sent.
        ///// </summary>
        //public event CS.OnClearCacheRequestDelegate?   OnClearCacheRequest;

        ///// <summary>
        ///// An event sent whenever a response to a ClearCache request was sent.
        ///// </summary>
        //public event CS.OnClearCacheResponseDelegate?  OnClearCacheResponse;

        //#endregion

        //#region OnGetLocalListVersion

        ///// <summary>
        ///// An event sent whenever a GetLocalListVersion request was sent.
        ///// </summary>
        //public event CS.OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        ///// <summary>
        ///// An event sent whenever a response to a GetLocalListVersion request was sent.
        ///// </summary>
        //public event CS.OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        //#endregion

        //#region OnSendLocalList

        ///// <summary>
        ///// An event sent whenever a SendLocalList request was sent.
        ///// </summary>
        //public event CS.OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        ///// <summary>
        ///// An event sent whenever a response to a SendLocalList request was sent.
        ///// </summary>
        //public event CS.OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        //#endregion


        //// Monitoring

        //#region OnChangeAvailability

        ///// <summary>
        ///// An event sent whenever a ChangeAvailability request was sent.
        ///// </summary>
        //public event CS.OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        ///// <summary>
        ///// An event sent whenever a response to a ChangeAvailability request was sent.
        ///// </summary>
        //public event CS.OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        //#endregion

        //#region OnChangeConfiguration

        ///// <summary>
        ///// An event sent whenever a ChangeConfiguration request was sent.
        ///// </summary>
        //public event CS.OnChangeConfigurationRequestDelegate?   OnChangeConfigurationRequest;

        ///// <summary>
        ///// An event sent whenever a response to a ChangeConfiguration request was sent.
        ///// </summary>
        //public event CS.OnChangeConfigurationResponseDelegate?  OnChangeConfigurationResponse;

        //#endregion

        //#region OnExtendedTriggerMessage

        ///// <summary>
        ///// An event sent whenever an ExtendedTriggerMessage request was sent.
        ///// </summary>
        //public event CS.OnExtendedTriggerMessageRequestDelegate?   OnExtendedTriggerMessageRequest;

        ///// <summary>
        ///// An event sent whenever a response to an ExtendedTriggerMessage request was sent.
        ///// </summary>
        //public event CS.OnExtendedTriggerMessageResponseDelegate?  OnExtendedTriggerMessageResponse;

        //#endregion

        //#region OnGetConfiguration

        ///// <summary>
        ///// An event sent whenever a GetConfiguration request was sent.
        ///// </summary>
        //public event CS.OnGetConfigurationRequestDelegate?   OnGetConfigurationRequest;

        ///// <summary>
        ///// An event sent whenever a response to a GetConfiguration request was sent.
        ///// </summary>
        //public event CS.OnGetConfigurationResponseDelegate?  OnGetConfigurationResponse;

        //#endregion

        //#region OnGetDiagnostics

        ///// <summary>
        ///// An event sent whenever a GetDiagnostics request was sent.
        ///// </summary>
        //public event CS.OnGetDiagnosticsRequestDelegate?   OnGetDiagnosticsRequest;

        ///// <summary>
        ///// An event sent whenever a response to a GetDiagnostics request was sent.
        ///// </summary>
        //public event CS.OnGetDiagnosticsResponseDelegate?  OnGetDiagnosticsResponse;

        //#endregion

        //#region OnGetLog

        ///// <summary>
        ///// An event sent whenever a GetLog request was sent.
        ///// </summary>
        //public event CS.OnGetLogRequestDelegate?   OnGetLogRequest;

        ///// <summary>
        ///// An event sent whenever a response to a GetLog request was sent.
        ///// </summary>
        //public event CS.OnGetLogResponseDelegate?  OnGetLogResponse;

        //#endregion

        //#region OnTriggerMessage

        ///// <summary>
        ///// An event sent whenever a TriggerMessage request was sent.
        ///// </summary>
        //public event CS.OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        ///// <summary>
        ///// An event sent whenever a response to a TriggerMessage request was sent.
        ///// </summary>
        //public event CS.OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        //#endregion


        //#region OnDataTransfer

        ///// <summary>
        ///// An event sent whenever a reset request was sent.
        ///// </summary>
        //public event OnDataTransferRequestDelegate?   OnDataTransferRequest;

        ///// <summary>
        ///// An event sent whenever a response to a reset request was sent.
        ///// </summary>
        //public event OnDataTransferResponseDelegate?  OnDataTransferResponse;

        //#endregion


        // Binary Data Streams Extensions

        //#region OnBinaryDataTransfer          (Request/-Response)

        /////// <summary>
        /////// An event sent whenever a BinaryDataTransfer request will be sent to the charging station.
        /////// </summary>
        ////public event OnBinaryDataTransferRequestSentDelegate?       OnBinaryDataTransferRequest;

        /////// <summary>
        /////// An event sent whenever a response to a BinaryDataTransfer request was received.
        /////// </summary>
        ////public event OnBinaryDataTransferResponseReceivedDelegate?  OnBinaryDataTransferResponse;

        //#endregion

        //#region OnGetFile                     (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a GetFile request will be sent to the charging station.
        ///// </summary>
        //public event OnGetFileRequestDelegate?   OnGetFileRequest;

        ///// <summary>
        ///// An event sent whenever a response to a GetFile request was received.
        ///// </summary>
        //public event OnGetFileResponseDelegate?  OnGetFileResponse;

        //#endregion

        //#region OnSendFile                    (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a SendFile request will be sent to the charging station.
        ///// </summary>
        //public event OnSendFileRequestDelegate?   OnSendFileRequest;

        ///// <summary>
        ///// An event sent whenever a response to a SendFile request was received.
        ///// </summary>
        //public event OnSendFileResponseDelegate?  OnSendFileResponse;

        //#endregion

        //#region OnDeleteFile                  (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a DeleteFile request will be sent to the charging station.
        ///// </summary>
        //public event OnDeleteFileRequestDelegate?   OnDeleteFileRequest;

        ///// <summary>
        ///// An event sent whenever a response to a DeleteFile request was received.
        ///// </summary>
        //public event OnDeleteFileResponseDelegate?  OnDeleteFileResponse;

        //#endregion

        //#region OnListDirectory               (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a ListDirectory request will be sent to the charging station.
        ///// </summary>
        //public event OnListDirectoryRequestDelegate?   OnListDirectoryRequest;

        ///// <summary>
        ///// An event sent whenever a response to a ListDirectory request was received.
        ///// </summary>
        //public event OnListDirectoryResponseDelegate?  OnListDirectoryResponse;

        //#endregion


        //// E2E Security Extensions

        //#region AddSignaturePolicy            (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a AddSignaturePolicy request will be sent to the charging station.
        ///// </summary>
        //public event OnAddSignaturePolicyRequestDelegate?   OnAddSignaturePolicyRequest;

        ///// <summary>
        ///// An event fired whenever a response to a AddSignaturePolicy request was received.
        ///// </summary>
        //public event OnAddSignaturePolicyResponseDelegate?  OnAddSignaturePolicyResponse;

        //#endregion

        //#region UpdateSignaturePolicy         (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a UpdateSignaturePolicy request will be sent to the charging station.
        ///// </summary>
        //public event OnUpdateSignaturePolicyRequestDelegate?   OnUpdateSignaturePolicyRequest;

        ///// <summary>
        ///// An event fired whenever a response to a UpdateSignaturePolicy request was received.
        ///// </summary>
        //public event OnUpdateSignaturePolicyResponseDelegate?  OnUpdateSignaturePolicyResponse;

        //#endregion

        //#region DeleteSignaturePolicy         (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a DeleteSignaturePolicy request will be sent to the charging station.
        ///// </summary>
        //public event OnDeleteSignaturePolicyRequestDelegate?   OnDeleteSignaturePolicyRequest;

        ///// <summary>
        ///// An event fired whenever a response to a DeleteSignaturePolicy request was received.
        ///// </summary>
        //public event OnDeleteSignaturePolicyResponseDelegate?  OnDeleteSignaturePolicyResponse;

        //#endregion

        //#region AddUserRole                   (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a AddUserRole request will be sent to the charging station.
        ///// </summary>
        //public event OnAddUserRoleRequestDelegate?   OnAddUserRoleRequest;

        ///// <summary>
        ///// An event fired whenever a response to a AddUserRole request was received.
        ///// </summary>
        //public event OnAddUserRoleResponseDelegate?  OnAddUserRoleResponse;

        //#endregion

        //#region UpdateUserRole                (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a UpdateUserRole request will be sent to the charging station.
        ///// </summary>
        //public event OnUpdateUserRoleRequestDelegate?   OnUpdateUserRoleRequest;

        ///// <summary>
        ///// An event fired whenever a response to a UpdateUserRole request was received.
        ///// </summary>
        //public event OnUpdateUserRoleResponseDelegate?  OnUpdateUserRoleResponse;

        //#endregion

        //#region DeleteUserRole                (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a DeleteUserRole request will be sent to the charging station.
        ///// </summary>
        //public event OnDeleteUserRoleRequestDelegate?   OnDeleteUserRoleRequest;

        ///// <summary>
        ///// An event fired whenever a response to a DeleteUserRole request was received.
        ///// </summary>
        //public event OnDeleteUserRoleResponseDelegate?  OnDeleteUserRoleResponse;

        //#endregion


        //#region OnSecureDataTransfer          (Request/-Response)

        /////// <summary>
        /////// An event sent whenever a SecureDataTransfer request will be sent to the charging station.
        /////// </summary>
        ////public event OnSecureDataTransferRequestSentDelegate?       OnSecureDataTransferRequest;

        /////// <summary>
        /////// An event sent whenever a response to a SecureDataTransfer request was received.
        /////// </summary>
        ////public event OnSecureDataTransferResponseReceivedDelegate?  OnSecureDataTransferResponse;

        //#endregion

        #endregion

        #endregion

        #region Custom JSON serializer delegates

     //   CustomJObjectSerializerDelegate<DataTransferResponse>? CustomIncomingDataTransferResponseSerializer { get; set; }

        public CustomJObjectSerializerDelegate<DataTransferRequest>?                 CustomDataTransferRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<DataTransferResponse>?                CustomDataTransferResponseSerializer                    { get; set; }


        #region Messages CentralSystem <- Charge Box

        public CustomJObjectSerializerDelegate<ResetRequest>?                           CustomResetRequestSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<CP.ResetResponse>?                       CustomResetResponseSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<ChangeAvailabilityRequest>?              CustomChangeAvailabilityRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CP.ChangeAvailabilityResponse>?          CustomChangeAvailabilityResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<GetConfigurationRequest>?                CustomGetConfigurationRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CP.GetConfigurationResponse>?            CustomGetConfigurationResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<ChangeConfigurationRequest>?             CustomChangeConfigurationRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<CP.ChangeConfigurationResponse>?         CustomChangeConfigurationResponseSerializer             { get; set; }
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


        //// Binary Data Streams Extensions
        //public CustomBinarySerializerDelegate <BinaryDataTransferResponse>?             CustomIncomingBinaryDataTransferResponseSerializer      { get; set; }


        //// Binary Data Streams Extensions
        //public CustomBinarySerializerDelegate <BinaryDataTransferRequest>?              CustomBinaryDataTransferRequestSerializer               { get; set; }
        //public CustomJObjectSerializerDelegate<GetFileRequest>?                         CustomGetFileRequestSerializer                          { get; set; }
        //public CustomBinarySerializerDelegate <SendFileRequest>?                        CustomSendFileRequestSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<DeleteFileRequest>?                      CustomDeleteFileRequestSerializer                       { get; set; }
        //public CustomJObjectSerializerDelegate<ListDirectoryRequest>?                   CustomListDirectoryRequestSerializer                    { get; set; }


        //// E2E Security Extensions
        //public CustomJObjectSerializerDelegate<AddSignaturePolicyRequest>?              CustomAddSignaturePolicyRequestSerializer               { get; set; }
        //public CustomJObjectSerializerDelegate<UpdateSignaturePolicyRequest>?           CustomUpdateSignaturePolicyRequestSerializer            { get; set; }
        //public CustomJObjectSerializerDelegate<DeleteSignaturePolicyRequest>?           CustomDeleteSignaturePolicyRequestSerializer            { get; set; }
        //public CustomJObjectSerializerDelegate<AddUserRoleRequest>?                     CustomAddUserRoleRequestSerializer                      { get; set; }
        //public CustomJObjectSerializerDelegate<UpdateUserRoleRequest>?                  CustomUpdateUserRoleRequestSerializer                   { get; set; }
        //public CustomJObjectSerializerDelegate<DeleteUserRoleRequest>?                  CustomDeleteUserRoleRequestSerializer                   { get; set; }

        //public CustomBinarySerializerDelegate <SecureDataTransferRequest>?              CustomSecureDataTransferRequestSerializer               { get; set; }

        #endregion


        //// Binary Data Streams Extensions
        //public CustomBinarySerializerDelegate <BinaryDataTransferResponse>?             CustomBinaryDataTransferResponseSerializer              { get; set; }
        //public CustomBinarySerializerDelegate <OCPP.CS.GetFileResponse>?                CustomGetFileResponseSerializer                         { get; set; }
        //public CustomJObjectSerializerDelegate<OCPP.CS.SendFileResponse>?               CustomSendFileResponseSerializer                        { get; set; }
        //public CustomJObjectSerializerDelegate<OCPP.CS.DeleteFileResponse>?             CustomDeleteFileResponseSerializer                      { get; set; }
        //public CustomJObjectSerializerDelegate<OCPP.CS.ListDirectoryResponse>?          CustomListDirectoryResponseSerializer                   { get; set; }


        //// E2E Security Extensions
        //public CustomBinarySerializerDelegate <SecureDataTransferResponse>?             CustomSecureDataTransferResponseSerializer              { get; set; }



        #region Charging Station Request  Messages

        public CustomJObjectSerializerDelegate<CP.BootNotificationRequest>?                          CustomBootNotificationRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<CP.HeartbeatRequest>?                                 CustomHeartbeatRequestSerializer                             { get; set; }

        public CustomJObjectSerializerDelegate<CP.AuthorizeRequest>?                                 CustomAuthorizeRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<CP.StartTransactionRequest>?                          CustomStartTransactionRequestRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CP.StatusNotificationRequest>?                        CustomStatusNotificationRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<CP.MeterValuesRequest>?                               CustomMeterValuesRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CP.StopTransactionRequest>?                           CustomStopTransactionRequestRequestSerializer                { get; set; }

      //  public CustomJObjectSerializerDelegate<CP.DataTransferRequest>?                              CustomIncomingDataTransferRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CP.DiagnosticsStatusNotificationRequest>?             CustomDiagnosticsStatusNotificationRequestSerializer         { get; set; }
        public CustomJObjectSerializerDelegate<CP.FirmwareStatusNotificationRequest>?                CustomFirmwareStatusNotificationRequestSerializer            { get; set; }


        // Security extensions
        public CustomJObjectSerializerDelegate<CP.SecurityEventNotificationRequest>?                 CustomSecurityEventNotificationRequestSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<CP.LogStatusNotificationRequest>?                     CustomLogStatusNotificationRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<CP.SignCertificateRequest>?                           CustomSignCertificateRequestSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CP.SignedFirmwareStatusNotificationRequest>?          CustomSignedFirmwareStatusNotificationRequestSerializer      { get; set; }


        // Binary Data Streams Extensions
        //public CustomBinarySerializerDelegate <BinaryDataTransferRequest>?                           CustomIncomingBinaryDataTransferRequestSerializer            { get; set; }

        #endregion


        public CustomJObjectSerializerDelegate<ConfigurationKey>?                       CustomConfigurationKeySerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ChargingProfile>?                        CustomChargingProfileSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?                       CustomChargingScheduleSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                 CustomChargingSchedulePeriodSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizationData>?                      CustomAuthorizationDataSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<IdTagInfo>?                              CustomIdTagInfoSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<CertificateHashData>?                    CustomCertificateHashDataSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<LogParameters>?                          CustomLogParametersSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<FirmwareImage>?                          CustomFirmwareImageSerializer                           { get; set; }


        public CustomJObjectSerializerDelegate<WWCP.Signature>?                         CustomSignatureSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<WWCP.CustomData>?                        CustomCustomDataSerializer                              { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate<WWCP.Signature>?                          CustomBinarySignatureSerializer                         { get; set; }


        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract Charging Station Management System (CSMS).
        /// </summary>
        /// <param name="Id">The unique identification of this Charging Station Management System (CSMS).</param>
        /// <param name="Description">An optional multi-language description of the Charging Station Management System (CSMS).</param>
        public ACentralSystemNode(NetworkingNode_Id              Id,
                                  String                         VendorName,
                                  String                         Model,
                                  String?                        SerialNumber                     = null,
                                  String?                        SoftwareVersion                  = null,
                                  I18NString?                    Description                      = null,
                                  CustomData?                    CustomData                       = null,

                                  AsymmetricCipherKeyPair?       ClientCAKeyPair                  = null,
                                  BCx509.X509Certificate?        ClientCACertificate              = null,

                                  SignaturePolicy?               SignaturePolicy                  = null,
                                  SignaturePolicy?               ForwardingSignaturePolicy        = null,

                                  //Func<ACentralSystemNode, HTTPAPI>?      HTTPAPI                          = null,
                                  //Boolean                        HTTPAPI_Disabled                 = false,
                                  //IPPort?                        HTTPAPI_Port                     = null,
                                  //String?                        HTTPAPI_ServerName               = null,
                                  //String?                        HTTPAPI_ServiceName              = null,
                                  //EMailAddress?                  HTTPAPI_RobotEMailAddress        = null,
                                  //String?                        HTTPAPI_RobotGPGPassphrase       = null,
                                  //Boolean                        HTTPAPI_EventLoggingDisabled     = false,

                                  //Func<ACentralSystemNode, DownloadAPI>?  HTTPDownloadAPI                  = null,
                                  //Boolean                        HTTPDownloadAPI_Disabled         = false,
                                  //HTTPPath?                      HTTPDownloadAPI_Path             = null,
                                  //String?                        HTTPDownloadAPI_FileSystemPath   = null,

                                  Func<ACentralSystemNode, UploadAPI>?    HTTPUploadAPI                    = null,
                                  Boolean                        HTTPUploadAPI_Disabled           = false,
                                  HTTPPath?                      HTTPUploadAPI_Path               = null,
                                  String?                        HTTPUploadAPI_FileSystemPath     = null,

                                  //HTTPPath?                      FirmwareDownloadAPIPath          = null,
                                  //HTTPPath?                      LogfilesUploadAPIPath            = null,
                                  //HTTPPath?                      DiagnosticsUploadAPIPath         = null,

                                  //Func<ACentralSystemNode, QRCodeAPI>?    QRCodeAPI                        = null,
                                  //Boolean                        QRCodeAPI_Disabled               = false,
                                  //HTTPPath?                      QRCodeAPI_Path                   = null,

                                  //Func<ACentralSystemNode, WebAPI>?       WebAPI                           = null,
                                  //Boolean                        WebAPI_Disabled                  = false,
                                  //HTTPPath?                      WebAPI_Path                      = null,

                                  TimeSpan?                      DefaultRequestTimeout            = null,

                                  Boolean                        DisableSendHeartbeats            = false,
                                  TimeSpan?                      SendHeartbeatsEvery              = null,

                                  Boolean                        DisableMaintenanceTasks          = false,
                                  TimeSpan?                      MaintenanceEvery                 = null,

                                  ISMTPClient?                   SMTPClient                       = null,
                                  DNSClient?                     DNSClient                        = null)

            : base(Id,
                   Description,
                   CustomData,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   null,
                   //!HTTPAPI_Disabled
                   //    ? new HTTPExtAPI(
                   //          HTTPServerPort:         HTTPAPI_Port               ?? IPPort.Auto,
                   //          HTTPServerName:         HTTPAPI_ServerName         ?? "GraphDefined OCPP Test Central System",
                   //          HTTPServiceName:        HTTPAPI_ServiceName        ?? "GraphDefined OCPP Test Central System Service",
                   //          APIRobotEMailAddress:   HTTPAPI_RobotEMailAddress  ?? EMailAddress.Parse("GraphDefined OCPP Test Central System Robot <robot@charging.cloud>"),
                   //          APIRobotGPGPassphrase:  HTTPAPI_RobotGPGPassphrase ?? "test123",
                   //          SMTPClient:             SMTPClient                 ?? new NullMailer(),
                   //          DNSClient:              DNSClient,
                   //          AutoStart:              true
                   //      )
                   //    : null,

                   DisableSendHeartbeats,
                   SendHeartbeatsEvery,

                   DefaultRequestTimeout ?? TimeSpan.FromMinutes(1),

                   DisableMaintenanceTasks,
                   MaintenanceEvery,

                   DNSClient)

        {

            if (VendorName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(VendorName),  "The given vendor name must not be null or empty!");

            if (Model.     IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Model),       "The given model must not be null or empty!");

            this.VendorName                      = VendorName;
            this.Model                           = Model;
            this.SerialNumber                    = SerialNumber;
            this.SoftwareVersion                 = SoftwareVersion;

            this.ClientCAKeyPair                 = ClientCAKeyPair;
            this.ClientCACertificate             = ClientCACertificate;

            OCPP.IN.AnycastIds.Add(NetworkingNode_Id.CentralSystem);

            #region Setup Web-/Upload-/DownloadAPIs

            //this.HTTPUploadAPI_Path              = HTTPUploadAPI_Path             ?? HTTPPath.Parse("uploads");
            //this.HTTPDownloadAPI_Path            = HTTPDownloadAPI_Path           ?? HTTPPath.Parse("downloads");
            //this.QRCodeAPI_Path                  = QRCodeAPI_Path                 ?? HTTPPath.Parse("qr");
            //this.WebAPI_Path                     = WebAPI_Path                    ?? HTTPPath.Parse("webapi");

            //this.HTTPUploadAPI_FileSystemPath    = HTTPUploadAPI_FileSystemPath   ?? Path.Combine(AppContext.BaseDirectory, "UploadAPI");
            //this.HTTPDownloadAPI_FileSystemPath  = HTTPDownloadAPI_FileSystemPath ?? Path.Combine(AppContext.BaseDirectory, "DownloadAPI");

            //if (this.HTTPExtAPI is not null)
            //    this.HTTPAPI                     = !HTTPAPI_Disabled
            //                                           ? HTTPAPI?.Invoke(this)      ?? new HTTPAPI(
            //                                                                               this,
            //                                                                               HTTPExtAPI,
            //                                                                               EventLoggingDisabled: HTTPAPI_EventLoggingDisabled
            //                                                                           )
            //                                           : null;

            //if (this.HTTPAPI is not null)
            //{

            //    #region HTTP API Security Settings

            //    this.HTTPAPI.HTTPBaseAPI.HTTPServer.AddAuth(request => {

            //        // Allow some URLs for anonymous access...
            //        if (request.Path.StartsWith(this.HTTPAPI.URLPathPrefix + this.HTTPUploadAPI_Path)   ||
            //            request.Path.StartsWith(this.HTTPAPI.URLPathPrefix + this.HTTPDownloadAPI_Path) ||
            //            request.Path.StartsWith(this.HTTPAPI.URLPathPrefix + this.WebAPI_Path))
            //        {
            //            return HTTPExtAPI.Anonymous;
            //        }

            //        return null;

            //    });

            //    #endregion


            //    if (!HTTPUploadAPI_Disabled)
            //    {

            //        Directory.CreateDirectory(this.HTTPUploadAPI_FileSystemPath);
            //        this.HTTPUploadAPI              = HTTPUploadAPI?.Invoke(this)   ?? new UploadAPI(
            //                                                                               this,
            //                                                                               this.HTTPAPI.HTTPBaseAPI.HTTPServer,
            //                                                                               URLPathPrefix:   this.HTTPUploadAPI_Path,
            //                                                                               FileSystemPath:  this.HTTPUploadAPI_FileSystemPath
            //                                                                           );

            //    }

            //    if (!HTTPDownloadAPI_Disabled)
            //    {

            //        Directory.CreateDirectory(this.HTTPDownloadAPI_FileSystemPath);
            //        this.HTTPDownloadAPI            = HTTPDownloadAPI?.Invoke(this) ?? new DownloadAPI(
            //                                                                               this,
            //                                                                               this.HTTPAPI.HTTPBaseAPI.HTTPServer,
            //                                                                               URLPathPrefix:   this.HTTPDownloadAPI_Path,
            //                                                                               FileSystemPath:  this.HTTPDownloadAPI_FileSystemPath
            //                                                                           );

            //    }

            //    if (!QRCodeAPI_Disabled)
            //    {

            //        this.QRCodeAPI                  = QRCodeAPI?.Invoke(this)       ?? new QRCodeAPI(
            //                                                                               this,
            //                                                                               this.HTTPAPI.HTTPBaseAPI.HTTPServer,
            //                                                                               URLPathPrefix:   this.QRCodeAPI_Path
            //                                                                           );

            //    }

            //    if (!WebAPI_Disabled)
            //    {

            //        this.WebAPI                     = WebAPI?.Invoke(this)          ?? new WebAPI(
            //                                                                               this,
            //                                                                               this.HTTPAPI.HTTPBaseAPI.HTTPServer,
            //                                                                               URLPathPrefix:   this.WebAPI_Path
            //                                                                           );

            //    }

            //}

            #endregion

        }

        #endregion



        #region RegisterToken (IdTag, IdTagInfo)

        /// <summary>
        /// Register the given identification tag.
        /// </summary>
        /// <param name="IdTag">The identification tag.</param>
        /// <param name="IdTagInfo">The identification tag information.</param>
        public virtual Task<Boolean> RegisterToken(IdToken    IdTag,
                                                   IdTagInfo  IdTagInfo)

            => Task.FromResult(
                   idTags.TryAdd(
                       IdTag,
                       IdTagInfo
                   )
               );

        #endregion

        #region RemoveToken   (IdTag)

        /// <summary>
        /// Remove the given identification tag.
        /// </summary>
        /// <param name="IdTag">The identification tag.</param>
        public virtual Task<Boolean> RemoveToken(IdToken IdTag)

            => Task.FromResult(
                   idTags.TryRemove(IdTag, out _)
               );

        #endregion


        //#region (override) HandleErrors(Module, Caller, ErrorResponse)

        //public override Task HandleErrors(String  Module,
        //                                  String  Caller,
        //                                  String  ErrorResponse)
        //{

        //    DebugX.Log($"{Module}.{Caller}: {ErrorResponse}");

        //    return base.HandleErrors(
        //               Module,
        //               Caller,
        //               ErrorResponse
        //           );

        //}

        //#endregion

        //#region (override) HandleErrors(Module, Caller, ExceptionOccured)

        //public override Task HandleErrors(String     Module,
        //                                  String     Caller,
        //                                  Exception  ExceptionOccured)
        //{

        //    DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

        //    return base.HandleErrors(
        //               Module,
        //               Caller,
        //               ExceptionOccured
        //           );

        //}

        //#endregion


    }

}
