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
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPPv1_6.NetworkingNode;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv1_6.CSMS;

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

        protected readonly ConcurrentDictionary<IdToken, IdTagInfo>  idTags                  = [];

        private   readonly TimeSpan                                  defaultRequestTimeout   = TimeSpan.FromSeconds(30);

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

        /// <summary>
        /// Whether to "auto create" unknown charge boxes and which type of access they should have.
        /// </summary>
        public ChargeBoxAccessTypes?  AutoCreatedChargeBoxesAccessType    { get; set; }

        /// <summary>
        /// Whether unknown charge boxes are automatically created.
        /// </summary>
        public Boolean                AutoCreateUnknownChargeBoxes
            => AutoCreatedChargeBoxesAccessType.HasValue;

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
                                  Boolean                                 HTTPUploadAPI_Disabled           = false,
                                  HTTPPath?                               HTTPUploadAPI_Path               = null,
                                  String?                                 HTTPUploadAPI_FileSystemPath     = null,

                                  //HTTPPath?                      FirmwareDownloadAPIPath          = null,
                                  //HTTPPath?                      LogfilesUploadAPIPath            = null,
                                  //HTTPPath?                      DiagnosticsUploadAPIPath         = null,

                                  //Func<ACentralSystemNode, QRCodeAPI>?    QRCodeAPI                        = null,
                                  //Boolean                        QRCodeAPI_Disabled               = false,
                                  //HTTPPath?                      QRCodeAPI_Path                   = null,

                                  //Func<ACentralSystemNode, WebAPI>?       WebAPI                           = null,
                                  //Boolean                        WebAPI_Disabled                  = false,
                                  //HTTPPath?                      WebAPI_Path                      = null,

                                  WebSocketServer?               ControlWebSocketServer             = null,

                                  ChargeBoxAccessTypes?          AutoCreatedChargeBoxesAccessType   = null,

                                  TimeSpan?                      DefaultRequestTimeout              = null,

                                  Boolean                        DisableSendHeartbeats              = false,
                                  TimeSpan?                      SendHeartbeatsEvery                = null,

                                  Boolean                        DisableMaintenanceTasks            = false,
                                  TimeSpan?                      MaintenanceEvery                   = null,

                                  ISMTPClient?                   SMTPClient                         = null,
                                  DNSClient?                     DNSClient                          = null)

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

                   ControlWebSocketServer,

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

            this.VendorName                        = VendorName;
            this.Model                             = Model;
            this.SerialNumber                      = SerialNumber;
            this.SoftwareVersion                   = SoftwareVersion;

            this.ClientCAKeyPair                   = ClientCAKeyPair;
            this.ClientCACertificate               = ClientCACertificate;

            this.AutoCreatedChargeBoxesAccessType  = AutoCreatedChargeBoxesAccessType;

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


        #region ChargeBoxes

        #region Data

        protected static readonly SemaphoreSlim ChargeBoxesSemaphore = new(1, 1);

        /// <summary>
        /// An enumeration of all charge boxes.
        /// </summary>
        protected internal readonly ConcurrentDictionary<ChargeBox_Id, ChargeBoxAccess> chargeBoxes = [];

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

                        return chargeBoxes.Values.
                                   Where (kvp => kvp.ChargeBox is not null).
                                   Select(kvp => kvp.ChargeBox).
                                   Cast<ChargeBox>().
                                   ToArray();

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

                return [];

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

        //        if (TelegramClient is not null)
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
        //                                                     new JProperty("timestamp", Timestamp.Now.ToISO8601())
        //                                                 ));

        //                if (messageTypes.Contains(updateChargeBox_MessageType))
        //                    await SendHTTPSNotifications(AllHTTPSNotifications,
        //                                                 new JObject(
        //                                                     new JProperty("chargeBoxUpdated",
        //                                                         ChargeBox.ToJSON()
        //                                                     ),
        //                                                     new JProperty("timestamp", Timestamp.Now.ToISO8601())
        //                                                 ));

        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.LogException(e);
        //        }

        //        #endregion

        //        #region EMailNotifications

        //        if (SMTPClient is not null)
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

        //        if (TelegramClient is not null)
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
        //                                                     new JProperty("timestamp", Timestamp.Now.ToISO8601())
        //                                                 ));

        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.LogException(e);
        //        }

        //        #endregion

        //        #region EMailNotifications

        //        if (SMTPClient is not null)
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
                          ChargeBoxAccessTypes                  ChargeBoxAccessType,
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
                           Id,
                           this
                       );

            if (chargeBoxes.ContainsKey(ChargeBox.Id))
                return AddChargeBoxResult.ArgumentError(
                           ChargeBox,
                           $"ChargeBox identification '{ChargeBox.Id}' already exists!".ToI18NString(),
                           eventTrackingId,
                           Id,
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

            chargeBoxes.TryAdd(
                ChargeBox.Id,
                new ChargeBoxAccess(
                    ChargeBox,
                    ChargeBoxAccessType
                )
            );

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
                       Id,
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
                         ChargeBoxAccessTypes                  ChargeBoxAccessType,
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
                                               ChargeBoxAccessType,
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
                               Id,
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
                       Id,
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
                                     ChargeBoxAccessTypes                  ChargeBoxAccessType,
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
                           Id,
                           this
                       );

            if (chargeBoxes.ContainsKey(ChargeBox.Id))
                return AddChargeBoxResult.NoOperation(
                           chargeBoxes[ChargeBox.Id].ChargeBox,
                           eventTrackingId,
                           Id,
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

            chargeBoxes.TryAdd(
                ChargeBox.Id,
                new ChargeBoxAccess(
                    ChargeBox,
                    ChargeBoxAccessType
                )
            );

            OnAdded?.Invoke(ChargeBox,
                            eventTrackingId);

            var OnChargeBoxAddedLocal = OnChargeBoxAdded;
            if (OnChargeBoxAddedLocal is not null)
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
                       Id,
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
                                    ChargeBoxAccessTypes                  ChargeBoxAccessType,
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
                                                          ChargeBoxAccessType,
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
                               Id,
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
                       Id,
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
                                  ChargeBoxAccessTypes                  ChargeBoxAccessType,
                                  Action<ChargeBox, EventTracking_Id>?  OnAdded           = null,
                                  Action<ChargeBox, EventTracking_Id>?  OnUpdated         = null,
                                  EventTracking_Id?                     EventTrackingId   = null,
                                  User_Id?                              CurrentUserId     = null)

        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (ChargeBox.API is not null && ChargeBox.API != this)
                return AddOrUpdateChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox is already attached to another API!".ToI18NString(),
                           eventTrackingId,
                           Id,
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

            if (chargeBoxes.TryGetValue(ChargeBox.Id, out var OldChargeBoxAccess))
            {
                chargeBoxes.Remove(ChargeBox.Id, out _);
                ChargeBox.CopyAllLinkedDataFromBase(OldChargeBoxAccess.ChargeBox);
            }

            chargeBoxes.TryAdd(
                ChargeBox.Id,
                new ChargeBoxAccess(
                    ChargeBox,
                    ChargeBoxAccessType
                )
            );

            if (OldChargeBoxAccess is null)
            {

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

                return AddOrUpdateChargeBoxResult.Added(
                           ChargeBox,
                           eventTrackingId,
                           Id,
                           this
                       );

            }

            OnUpdated?.Invoke(ChargeBox,
                              eventTrackingId);

            var OnChargeBoxUpdatedLocal = OnChargeBoxUpdated;
            if (OnChargeBoxUpdatedLocal is not null)
                await OnChargeBoxUpdatedLocal.Invoke(Timestamp.Now,
                                                     ChargeBox,
                                                     OldChargeBoxAccess.ChargeBox,
                                                     ChargeBoxAccessType,
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
                       Id,
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
                                 ChargeBoxAccessTypes                  ChargeBoxAccessType,
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
                                                       ChargeBoxAccessType,
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
                               Id,
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
                       Id,
                       this
                   );

        }

        #endregion

        #endregion


        public Task<AddChargeBoxAccessResult> AddChargeBoxAccess(ChargeBox_Id          ChargeBoxId,
                                                                 ChargeBoxAccessTypes  ChargeBoxAccessType,
                                                                 EventTracking_Id?     EventTrackingId   = null,
                                                                 User_Id?              CurrentUserId     = null)
        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (chargeBoxes.ContainsKey(ChargeBoxId))
                return Task.FromResult(
                           AddChargeBoxAccessResult.ArgumentError(
                               ChargeBoxId,
                               ChargeBoxAccessType,
                               $"The given chargeBox '{ChargeBoxId}' already exists in this API!".ToI18NString(),
                               eventTrackingId,
                               Id,
                               this
                           )
                       );

            if (chargeBoxes.TryAdd(ChargeBoxId, new ChargeBoxAccess(ChargeBoxAccessType)))
                return Task.FromResult(
                           AddChargeBoxAccessResult.Success(
                               ChargeBoxId,
                               ChargeBoxAccessType,
                               EventTrackingId,
                               Id,
                               this
                           )
                       );

            // TryAdd(...) failed!
            return Task.FromResult(
                       AddChargeBoxAccessResult.Error(
                           ChargeBoxId,
                           ChargeBoxAccessType,
                           I18NString.Create($"Could not add charge box '{ChargeBoxId}' with access {ChargeBoxAccessType}''!"),
                           EventTrackingId,
                           Id,
                           this
                       )
                   );

        }


        public async Task<UpdateChargeBoxAccessResult> UpdateChargeBoxAccess(ChargeBox             ChargeBox,
                                                                             ChargeBoxAccessTypes  ChargeBoxAccessType,
                                                                             EventTracking_Id?     EventTrackingId   = null,
                                                                             User_Id?              CurrentUserId     = null)
        {

            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            if (!chargeBoxes.TryGetValue(ChargeBox.Id, out var chargeBoxAccess))
                return UpdateChargeBoxAccessResult.ArgumentError(
                           ChargeBox,
                           ChargeBoxAccessType,
                           $"The given chargeBox '{ChargeBox.Id}' does not exists in this API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            //if (ChargeBox.API is not null && ChargeBox.API != this)
            //    return UpdateChargeBoxResult.ArgumentError(
            //               ChargeBox,
            //               "The given chargeBox is already attached to another API!".ToI18NString(),
            //               eventTrackingId,
            //               Id,
            //               this
            //           );

            if (chargeBoxAccess.ChargeBoxAccessType != ChargeBoxAccessType)
            {

                chargeBoxAccess.ChargeBoxAccessType = ChargeBoxAccessType;

                return UpdateChargeBoxAccessResult.Success(
                           ChargeBox,
                           ChargeBoxAccessType,
                           EventTrackingId,
                           Id,
                           this
                       );

            }

            return UpdateChargeBoxAccessResult.NoOperation(
                       ChargeBox,
                       ChargeBoxAccessType,
                       EventTrackingId,
                       Id,
                       this
                   );

        }


        #region UpdateChargeBox        (ChargeBox,                 OnUpdated = null, ...)

        /// <summary>
        /// A delegate called whenever a charge box was updated.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the chargeBox was updated.</param>
        /// <param name="ChargeBox">The updated charge box.</param>
        /// <param name="OldChargeBox">The old charge box.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional chargeBox identification initiating this command/request.</param>
        public delegate Task OnChargeBoxUpdatedDelegate(DateTime              Timestamp,
                                                        ChargeBox             ChargeBox,
                                                        ChargeBox             OldChargeBox,
                                                        ChargeBoxAccessTypes  ChargeBoxAccessType,
                                                        EventTracking_Id?     EventTrackingId   = null,
                                                        User_Id?              CurrentUserId     = null);

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
                             ChargeBoxAccessTypes                  ChargeBoxAccessType,
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
                           Id,
                           this
                       );

            if (ChargeBox.API is not null && ChargeBox.API != this)
                return UpdateChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox is already attached to another API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            ChargeBox.API = this;


            //await WriteToDatabaseFile(updateChargeBox_MessageType,
            //                          ChargeBox.ToJSON(),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargeBoxes.Remove(OldChargeBox.Id, out _);
            ChargeBox.CopyAllLinkedDataFromBase(OldChargeBox);

            chargeBoxes.TryAdd(
                ChargeBox.Id,
                new ChargeBoxAccess(
                    ChargeBox,
                    ChargeBoxAccessType
                )
            );


            OnUpdated?.Invoke(ChargeBox,
                              eventTrackingId);

            var OnChargeBoxUpdatedLocal = OnChargeBoxUpdated;
            if (OnChargeBoxUpdatedLocal is not null)
                await OnChargeBoxUpdatedLocal.Invoke(Timestamp.Now,
                                                     ChargeBox,
                                                     OldChargeBox,
                                                     ChargeBoxAccessType,
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
                       Id,
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
                            ChargeBoxAccessTypes                  ChargeBoxAccessType,
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
                                                  ChargeBoxAccessType,
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
                               Id,
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
                       Id,
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
                             ChargeBoxAccessTypes                  ChargeBoxAccessType,
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
                           Id,
                           this
                       );

            if (ChargeBox.API != this)
                return UpdateChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox is not attached to this API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );

            if (UpdateDelegate is null)
                return UpdateChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given update delegate must not be null!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );


            var builder = ChargeBox.ToBuilder();
            UpdateDelegate(builder);
            var updatedChargeBox = builder.ToImmutable;

            //await WriteToDatabaseFile(updateChargeBox_MessageType,
            //                          updatedChargeBox.ToJSON(),
            //                          eventTrackingId,
            //                          CurrentUserId);

            chargeBoxes.Remove(ChargeBox.Id, out _);
            updatedChargeBox.CopyAllLinkedDataFromBase(ChargeBox);

            chargeBoxes.TryAdd(
                updatedChargeBox.Id,
                new ChargeBoxAccess(
                    updatedChargeBox,
                    ChargeBoxAccessType
                )
            );


            OnUpdated?.Invoke(updatedChargeBox,
                              eventTrackingId);

            var OnChargeBoxUpdatedLocal = OnChargeBoxUpdated;
            if (OnChargeBoxUpdatedLocal is not null)
                await OnChargeBoxUpdatedLocal.Invoke(Timestamp.Now,
                                                     updatedChargeBox,
                                                     ChargeBox,
                                                     ChargeBoxAccessType,
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
                       Id,
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
                            ChargeBoxAccessTypes                  ChargeBoxAccessType,
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
                                                  ChargeBoxAccessType,
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
                               Id,
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
                       Id,
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
                           Id,
                           this
                       );

            if (!chargeBoxes.TryGetValue(ChargeBox.Id, out var ChargeBoxToBeDeleted))
                return DeleteChargeBoxResult.ArgumentError(
                           ChargeBox,
                           "The given chargeBox does not exists in this API!".ToI18NString(),
                           eventTrackingId,
                           Id,
                           this
                       );


            var veto = _CanDeleteChargeBox(ChargeBox);

            if (veto is not null)
                return DeleteChargeBoxResult.CanNotBeRemoved(
                           ChargeBox,
                           eventTrackingId,
                           Id,
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

            chargeBoxes.Remove(ChargeBox.Id, out _);

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
                       Id,
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
                               Id,
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
                       Id,
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

            if (NetworkingNodeId.IsNotNullOrEmpty && chargeBoxes.TryGetValue(NetworkingNodeId, out var chargeBoxAccess))
                return chargeBoxAccess.ChargeBox;

            return default;

        }

        /// <summary>
        /// Get the chargeBox having the given unique identification.
        /// </summary>
        /// <param name="NetworkingNodeId">The unique identification of an charge box.</param>
        protected internal ChargeBox? _GetChargeBox(ChargeBox_Id? NetworkingNodeId)
        {

            if (NetworkingNodeId is not null && chargeBoxes.TryGetValue(NetworkingNodeId.Value, out var chargeBoxAccess))
                return chargeBoxAccess.ChargeBox;

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

            if (NetworkingNodeId.IsNotNullOrEmpty && chargeBoxes.TryGetValue(NetworkingNodeId, out var chargeBoxAccess))
            {
                ChargeBox = chargeBoxAccess.ChargeBox;
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

            if (NetworkingNodeId is not null && chargeBoxes.TryGetValue(NetworkingNodeId.Value, out var chargeBoxAccess))
            {
                ChargeBox = chargeBoxAccess.ChargeBox;
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

        //#region (override) HandleErrors(Module, Caller, ExceptionOccurred)

        //public override Task HandleErrors(String     Module,
        //                                  String     Caller,
        //                                  Exception  ExceptionOccurred)
        //{

        //    DebugX.LogException(ExceptionOccurred, $"{Module}.{Caller}");

        //    return base.HandleErrors(
        //               Module,
        //               Caller,
        //               ExceptionOccurred
        //           );

        //}

        //#endregion


    }

}
