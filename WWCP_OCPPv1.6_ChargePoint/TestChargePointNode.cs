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

using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.Logging;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A Charge Point for testing.
    /// </summary>
    public class TestChargePointNode : AChargePointNode,
                                       IChargePointNode
    {

        #region Events

        #region Outgoing messages: Charge Point --(NN)-> Central System

        //#region OnBootNotification                 (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a BootNotification request will be sent to the central system.
        ///// </summary>
        //public event OnBootNotificationRequestDelegate?                     OnBootNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a BootNotification request was received.
        ///// </summary>
        //public event OnBootNotificationResponseDelegate?                    OnBootNotificationResponse;

        //#endregion

        //#region OnHeartbeat                        (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a Heartbeat request will be sent to the central system.
        ///// </summary>
        //public event OnHeartbeatRequestDelegate?                            OnHeartbeatRequest;

        ///// <summary>
        ///// An event fired whenever a response to a Heartbeat request was received.
        ///// </summary>
        //public event OnHeartbeatResponseDelegate?                           OnHeartbeatResponse;

        //#endregion

        //#region OnDiagnosticsStatusNotification    (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a DiagnosticsStatusNotification request will be sent to the central system.
        ///// </summary>
        //public event OnDiagnosticsStatusNotificationRequestDelegate?        OnDiagnosticsStatusNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a DiagnosticsStatusNotification request was received.
        ///// </summary>
        //public event OnDiagnosticsStatusNotificationResponseDelegate?       OnDiagnosticsStatusNotificationResponse;

        //#endregion

        //#region OnFirmwareStatusNotification       (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a FirmwareStatusNotification request will be sent to the central system.
        ///// </summary>
        //public event OnFirmwareStatusNotificationRequestDelegate?           OnFirmwareStatusNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a FirmwareStatusNotification request was received.
        ///// </summary>
        //public event OnFirmwareStatusNotificationResponseDelegate?          OnFirmwareStatusNotificationResponse;

        //#endregion


        //#region OnAuthorize                        (Request/-Response)

        ///// <summary>
        ///// An event fired whenever an Authorize request will be sent to the central system.
        ///// </summary>
        //public event OnAuthorizeRequestDelegate?                            OnAuthorizeRequest;

        ///// <summary>
        ///// An event fired whenever a response to an Authorize request was received.
        ///// </summary>
        //public event OnAuthorizeResponseDelegate?                           OnAuthorizeResponse;

        //#endregion

        //#region OnStartTransaction                 (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a StartTransaction request will be sent to the central system.
        ///// </summary>
        //public event OnStartTransactionRequestDelegate?                     OnStartTransactionRequest;

        ///// <summary>
        ///// An event fired whenever a response to a StartTransaction request was received.
        ///// </summary>
        //public event OnStartTransactionResponseDelegate?                    OnStartTransactionResponse;

        //#endregion

        //#region OnStatusNotification               (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a StatusNotification request will be sent to the central system.
        ///// </summary>
        //public event OnStatusNotificationRequestDelegate?                   OnStatusNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a StatusNotification request was received.
        ///// </summary>
        //public event OnStatusNotificationResponseDelegate?                  OnStatusNotificationResponse;

        //#endregion

        //#region OnMeterValues                      (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a MeterValues request will be sent to the central system.
        ///// </summary>
        //public event OnMeterValuesRequestDelegate?                          OnMeterValuesRequest;

        ///// <summary>
        ///// An event fired whenever a response to a MeterValues request was received.
        ///// </summary>
        //public event OnMeterValuesResponseDelegate?                         OnMeterValuesResponse;

        //#endregion

        //#region OnStopTransaction                  (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a StopTransaction request will be sent to the central system.
        ///// </summary>
        //public event OnStopTransactionRequestDelegate?                      OnStopTransactionRequest;

        ///// <summary>
        ///// An event fired whenever a response to a StopTransaction request was received.
        ///// </summary>
        //public event OnStopTransactionResponseDelegate?                     OnStopTransactionResponse;

        //#endregion


        //#region OnDataTransfer                     (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a DataTransfer request will be sent to the central system.
        ///// </summary>
        //public event OnDataTransferRequestDelegate?                         OnDataTransferRequest;

        ///// <summary>
        ///// An event fired whenever a response to a DataTransfer request was received.
        ///// </summary>
        //public event OnDataTransferResponseDelegate?                        OnDataTransferResponse;

        //#endregion


        //// Security Extensions

        //#region OnLogStatusNotification            (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a LogStatusNotification request will be sent to the central system.
        ///// </summary>
        //public event OnLogStatusNotificationRequestDelegate?                OnLogStatusNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a LogStatusNotification request was received.
        ///// </summary>
        //public event OnLogStatusNotificationResponseDelegate?               OnLogStatusNotificationResponse;

        //#endregion

        //#region OnSecurityEventNotification        (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a SecurityEventNotification request will be sent to the central system.
        ///// </summary>
        //public event OnSecurityEventNotificationRequestDelegate?            OnSecurityEventNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SecurityEventNotification request was received.
        ///// </summary>
        //public event OnSecurityEventNotificationResponseDelegate?           OnSecurityEventNotificationResponse;

        //#endregion

        //#region OnSignCertificate                  (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a SignCertificate request will be sent to the central system.
        ///// </summary>
        //public event OnSignCertificateRequestDelegate?                      OnSignCertificateRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SignCertificate request was received.
        ///// </summary>
        //public event OnSignCertificateResponseDelegate?                     OnSignCertificateResponse;

        //#endregion

        //#region OnSignedFirmwareStatusNotification (Request/-Response)

        ///// <summary>
        ///// An event fired whenever a SignedFirmwareStatusNotification request will be sent to the central system.
        ///// </summary>
        //public event OnSignedFirmwareStatusNotificationRequestDelegate?     OnSignedFirmwareStatusNotificationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SignedFirmwareStatusNotification request was received.
        ///// </summary>
        //public event OnSignedFirmwareStatusNotificationResponseDelegate?    OnSignedFirmwareStatusNotificationResponse;

        //#endregion


        //// Binary Data Streams Extensions

        //#region OnBinaryDataTransfer               (Request/-Response)

        /////// <summary>
        /////// An event fired whenever a BinaryDataTransfer request will be sent to the central system.
        /////// </summary>
        ////public event OnBinaryDataTransferRequestSentDelegate?                   OnBinaryDataTransferRequestSent;

        /////// <summary>
        /////// An event fired whenever a response to a BinaryDataTransfer request was received.
        /////// </summary>
        ////public event OnBinaryDataTransferResponseReceivedDelegate?                  OnBinaryDataTransferResponseReceived;

        //#endregion

        #endregion

        #region Incoming messages: Charge Point <-(NN)-- Central System

        //#region OnReset                  (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a Reset request was received.
        ///// </summary>
        //public event OnResetRequestDelegate?                      OnResetRequest;

        ///// <summary>
        ///// An event sent whenever a response to a Reset request was sent.
        ///// </summary>
        //public event OnResetResponseDelegate?                     OnResetResponse;

        //#endregion

        //#region OnChangeAvailability     (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a ChangeAvailability request was received.
        ///// </summary>
        //public event OnChangeAvailabilityRequestDelegate?         OnChangeAvailabilityRequest;

        ///// <summary>
        ///// An event sent whenever a response to a ChangeAvailability request was sent.
        ///// </summary>
        //public event OnChangeAvailabilityResponseDelegate?        OnChangeAvailabilityResponse;

        //#endregion

        //#region OnGetConfiguration       (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a GetConfiguration request was received.
        ///// </summary>
        //public event OnGetConfigurationRequestDelegate?           OnGetConfigurationRequest;

        ///// <summary>
        ///// An event sent whenever a response to a GetConfiguration request was sent.
        ///// </summary>
        //public event OnGetConfigurationResponseDelegate?          OnGetConfigurationResponse;

        //#endregion

        //#region OnChangeConfiguration    (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a reset request was received.
        ///// </summary>
        //public event OnChangeConfigurationRequestDelegate?        OnChangeConfigurationRequest;

        ///// <summary>
        ///// An event sent whenever a response to a reset request was sent.
        ///// </summary>
        //public event OnChangeConfigurationResponseDelegate?       OnChangeConfigurationResponse;

        //#endregion

        //#region OnIncomingDataTransfer   (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a data transfer request was received.
        ///// </summary>
        //public event OnIncomingDataTransferRequestDelegate?   OnIncomingDataTransferRequest;

        ///// <summary>
        ///// An event sent whenever a response to a data transfer request was sent.
        ///// </summary>
        //public event OnIncomingDataTransferResponseDelegate?  OnIncomingDataTransferResponse;

        //#endregion

        //#region GetDiagnostics (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a reset request was received.
        ///// </summary>
        //public event OnGetDiagnosticsRequestDelegate?   OnGetDiagnosticsRequest;

        ///// <summary>
        ///// An event sent whenever a response to a reset request was sent.
        ///// </summary>
        //public event OnGetDiagnosticsResponseDelegate?  OnGetDiagnosticsResponse;

        //#endregion

        //#region TriggerMessage (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a reset request was received.
        ///// </summary>
        //public event OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        ///// <summary>
        ///// An event sent whenever a response to a reset request was sent.
        ///// </summary>
        //public event OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        //#endregion

        //#region UpdateFirmware (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a reset request was received.
        ///// </summary>
        //public event OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        ///// <summary>
        ///// An event sent whenever a response to a reset request was sent.
        ///// </summary>
        //public event OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        //#endregion


        //#region OnReserveNow (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a reserve now request was received.
        ///// </summary>
        //public event OnReserveNowRequestDelegate?   OnReserveNowRequest;

        ///// <summary>
        ///// An event sent whenever a response to a reserve now request was sent.
        ///// </summary>
        //public event OnReserveNowResponseDelegate?  OnReserveNowResponse;

        //#endregion

        //#region OnCancelReservation      (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a cancel reservation request was received.
        ///// </summary>
        //public event OnCancelReservationRequestDelegate?          OnCancelReservationRequest;

        ///// <summary>
        ///// An event sent whenever a response to a cancel reservation request was sent.
        ///// </summary>
        //public event OnCancelReservationResponseDelegate?         OnCancelReservationResponse;

        //#endregion

        //#region OnRemoteStartTransaction (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a remote start transaction request was received.
        ///// </summary>
        //public event OnRemoteStartTransactionRequestDelegate?     OnRemoteStartTransactionRequest;

        ///// <summary>
        ///// An event sent whenever a response to a remote start transaction request was sent.
        ///// </summary>
        //public event OnRemoteStartTransactionResponseDelegate?    OnRemoteStartTransactionResponse;

        //#endregion

        //#region OnRemoteStopTransaction  (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a remote stop transaction request was received.
        ///// </summary>
        //public event OnRemoteStopTransactionRequestDelegate?   OnRemoteStopTransactionRequest;

        ///// <summary>
        ///// An event sent whenever a response to a remote stop transaction request was sent.
        ///// </summary>
        //public event OnRemoteStopTransactionResponseDelegate?  OnRemoteStopTransactionResponse;

        //#endregion

        //#region SetChargingProfile (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a reset request was received.
        ///// </summary>
        //public event OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        ///// <summary>
        ///// An event sent whenever a response to a reset request was sent.
        ///// </summary>
        //public event OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        //#endregion

        //#region ClearChargingProfile (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a reset request was received.
        ///// </summary>
        //public event OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        ///// <summary>
        ///// An event sent whenever a response to a reset request was sent.
        ///// </summary>
        //public event OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        //#endregion

        //#region GetCompositeSchedule (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a reset request was received.
        ///// </summary>
        //public event OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        ///// <summary>
        ///// An event sent whenever a response to a reset request was sent.
        ///// </summary>
        //public event OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        //#endregion

        //#region UnlockConnector (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a reset request was received.
        ///// </summary>
        //public event OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        ///// <summary>
        ///// An event sent whenever a response to a reset request was sent.
        ///// </summary>
        //public event OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        //#endregion


        //#region GetLocalListVersion (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a reset request was received.
        ///// </summary>
        //public event OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        ///// <summary>
        ///// An event sent whenever a response to a reset request was sent.
        ///// </summary>
        //public event OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        //#endregion

        //#region SendLocalList (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a reset request was received.
        ///// </summary>
        //public event OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        ///// <summary>
        ///// An event sent whenever a response to a reset request was sent.
        ///// </summary>
        //public event OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        //#endregion

        //#region ClearCache (Request/-Response)

        ///// <summary>
        ///// An event sent whenever a reset request was received.
        ///// </summary>
        //public event OnClearCacheRequestDelegate?   OnClearCacheRequest;

        ///// <summary>
        ///// An event sent whenever a response to a reset request was sent.
        ///// </summary>
        //public event OnClearCacheResponseDelegate?  OnClearCacheResponse;

        //#endregion


        ////public event OnBinaryDataTransferRequestReceivedDelegate?             OnIncomingBinaryDataTransferRequest;
        ////public event OnBinaryDataTransferResponseSentDelegate?            OnIncomingBinaryDataTransferResponse;

        //public event OCPP.CS.OnGetFileRequestDelegate?                        OnGetFileRequest;
        //public event OCPP.CS.OnGetFileResponseDelegate?                       OnGetFileResponse;

        //public event OCPP.CS.OnSendFileRequestDelegate?                       OnSendFileRequest;
        //public event OCPP.CS.OnSendFileResponseDelegate?                      OnSendFileResponse;

        //public event OCPP.CS.OnDeleteFileRequestDelegate?                     OnDeleteFileRequest;
        //public event OCPP.CS.OnDeleteFileResponseDelegate?                    OnDeleteFileResponse;

        //public event OCPP.CS.OnAddSignaturePolicyRequestDelegate?             OnAddSignaturePolicyRequest;
        //public event OCPP.CS.OnAddSignaturePolicyResponseDelegate?            OnAddSignaturePolicyResponse;

        //public event OCPP.CS.OnUpdateSignaturePolicyRequestDelegate?          OnUpdateSignaturePolicyRequest;
        //public event OCPP.CS.OnUpdateSignaturePolicyResponseDelegate?         OnUpdateSignaturePolicyResponse;

        //public event OCPP.CS.OnDeleteSignaturePolicyRequestDelegate?          OnDeleteSignaturePolicyRequest;
        //public event OCPP.CS.OnDeleteSignaturePolicyResponseDelegate?         OnDeleteSignaturePolicyResponse;

        //public event OCPP.CS.OnAddUserRoleRequestDelegate?                    OnAddUserRoleRequest;
        //public event OCPP.CS.OnAddUserRoleResponseDelegate?                   OnAddUserRoleResponse;

        //public event OCPP.CS.OnUpdateUserRoleRequestDelegate?                 OnUpdateUserRoleRequest;
        //public event OCPP.CS.OnUpdateUserRoleResponseDelegate?                OnUpdateUserRoleResponse;

        //public event OCPP.CS.OnDeleteUserRoleRequestDelegate?                 OnDeleteUserRoleRequest;
        //public event OCPP.CS.OnDeleteUserRoleResponseDelegate?                OnDeleteUserRoleResponse;

        #endregion

        #endregion

        #region Custom JSON serializer delegates

        #region Request/Response Messages
        //public CustomJObjectSerializerDelegate<CS.DataTransferRequest>?                              CustomIncomingDataTransferRequestSerializer                  { get; set; }
        //public CustomJObjectSerializerDelegate<CP.DataTransferResponse>?                             CustomIncomingDataTransferResponseSerializer                 { get; set; }


        // Binary Data Streams Extensions
        //public CustomBinarySerializerDelegate <BinaryDataTransferRequest>?                           CustomBinaryDataTransferRequestSerializer                    { get; set; }
        //public CustomBinarySerializerDelegate <BinaryDataTransferResponse>?                          CustomBinaryDataTransferResponseSerializer                   { get; set; }

        #endregion

        #region Data Structures
     //   public CustomJObjectSerializerDelegate<OCPP.Signature>?                                      CustomSignatureSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                                   { get; set; }


        // Binary Data Streams Extensions
        //  public CustomBinarySerializerDelegate <OCPP.Signature>?                                      CustomBinarySignatureSerializer                              { get; set; }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge point for testing.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="NumberOfConnectors">Number of available connectors.</param>
        /// 
        /// <param name="Description">An optional multi-language charge box description.</param>
        /// <param name="ChargePointVendor">An optional charge point vendor identification.</param>
        /// <param name="ChargePointModel">An optional charge point model identification.</param>
        /// <param name="ChargePointSerialNumber">An optional serial number of the charge point.</param>
        /// <param name="ChargeBoxSerialNumber">An optional serial number of the charge point.</param>
        /// <param name="FirmwareVersion">An optional firmware version of the charge point.</param>
        /// <param name="Iccid">An optional ICCID of the charge point's SIM card.</param>
        /// <param name="IMSI">An optional IMSI of the charge point’s SIM card.</param>
        /// <param name="MeterType">An optional meter type of the main power meter of the charge point.</param>
        /// <param name="MeterSerialNumber">An optional serial number of the main power meter of the charge point.</param>
        /// <param name="MeterPublicKey">An optional public key of the main power meter of the charge point.</param>
        /// 
        /// <param name="DefaultRequestTimeout">The default request timeout for all requests.</param>
        /// 
        /// 
        /// <param name="DisableSendHeartbeats">Whether to disable sending heartbeat requests.</param>
        /// <param name="SendHeartbeatsEvery">The time span between heartbeat requests.</param>
        /// 
        /// <param name="DisableMaintenanceTasks">Whether to disable maintenance tasks.</param>
        /// <param name="MaintenanceEvery">The time span between maintenance task runs.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public TestChargePointNode(NetworkingNode_Id           ChargeBoxId,
                                   IEnumerable<ConnectorSpec>  Connectors,

                                   I18NString?                 Description                    = null,
                                   String?                     ChargePointVendor              = null,
                                   String?                     ChargePointModel               = null,
                                   String?                     ChargePointSerialNumber        = null,
                                   String?                     ChargeBoxSerialNumber          = null,
                                   String?                     FirmwareVersion                = null,
                                   String?                     Iccid                          = null,
                                   String?                     IMSI                           = null,
                                   IEnergyMeter?               UplinkEnergyMeter              = null,

                                   TimeSpan?                   DefaultRequestTimeout          = null,

                                   SignaturePolicy?            SignaturePolicy                = null,
                                   SignaturePolicy?            ForwardingSignaturePolicy      = null,

                                   Boolean                     HTTPAPI_Disabled               = false,
                                   IPPort?                     HTTPAPI_Port                   = null,
                                   String?                     HTTPAPI_ServerName             = null,
                                   String?                     HTTPAPI_ServiceName            = null,
                                   EMailAddress?               HTTPAPI_RobotEMailAddress      = null,
                                   String?                     HTTPAPI_RobotGPGPassphrase     = null,
                                   Boolean                     HTTPAPI_EventLoggingDisabled   = false,

                                   //WebAPI?                     WebAPI                         = null,
                                   Boolean                     WebAPI_Disabled                = false,
                                   HTTPPath?                   WebAPI_Path                    = null,

                                   WebSocketServer?            ControlWebSocketServer         = null,

                                   Boolean                     DisableSendHeartbeats          = false,
                                   TimeSpan?                   SendHeartbeatsEvery            = null,

                                   Boolean                     DisableMaintenanceTasks        = false,
                                   TimeSpan?                   MaintenanceEvery               = null,

                                   CustomData?                 CustomData                     = null,
                                   DNSClient?                  DNSClient                      = null)

            : base(ChargeBoxId,
                   Connectors,

                   Description,
                   ChargePointVendor        ?? "GraphDefined",
                   ChargePointModel         ?? "CP1.6",
                   ChargePointSerialNumber  ?? "CP-S/N-00000005",
                   ChargeBoxSerialNumber    ?? "CB-S/N-00000023",
                   FirmwareVersion          ?? "FW-1.01a",
                   Iccid                    ?? "891004234814455936F",
                   IMSI                     ?? "262012345678901",
                   UplinkEnergyMeter,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   HTTPAPI_Disabled,
                   HTTPAPI_Port,
                   HTTPAPI_ServerName,
                   HTTPAPI_ServiceName,
                   HTTPAPI_RobotEMailAddress,
                   HTTPAPI_RobotGPGPassphrase,
                   HTTPAPI_EventLoggingDisabled,

                   //null,//WebAPI,
                   WebAPI_Disabled,
                   WebAPI_Path,

                   ControlWebSocketServer,

                   DefaultRequestTimeout,

                   DisableSendHeartbeats,
                   SendHeartbeatsEvery,

                   DisableMaintenanceTasks,
                   MaintenanceEvery,

                   CustomData,
                   DNSClient)

        {

            this.Configuration.TryAdd("hello",          new ConfigurationData("world",    AccessRights.ReadOnly,  false));
            this.Configuration.TryAdd("changeMe",       new ConfigurationData("now",      AccessRights.ReadWrite, false));
            this.Configuration.TryAdd("doNotChangeMe",  new ConfigurationData("never",    AccessRights.ReadOnly,  false));
            this.Configuration.TryAdd("password",       new ConfigurationData("12345678", AccessRights.WriteOnly, false));

        }

        #endregion


        private Task HandleErrors(String     Module,
                                  String     Caller,
                                  Exception  ExceptionOccured)
        {

            DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

            return Task.CompletedTask;

        }


        #region InitSOAP(...)

        public async Task InitSOAP(String                                                     From,
                                   String                                                     To,

                                   URL                                                        RemoteURL,
                                   HTTPHostname?                                              VirtualHostname              = null,
                                   String?                                                    Description                  = null,
                                   RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                   LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                   X509Certificate?                                           ClientCert                   = null,
                                   SslProtocols?                                              TLSProtocol                  = null,
                                   Boolean?                                                   PreferIPv4                   = null,
                                   String?                                                    HTTPUserAgent                = null,
                                   HTTPPath?                                                  URLPathPrefix                = null,
                                   Tuple<String, String>?                                     WSSLoginPassword             = null,
                                   HTTPContentType?                                           HTTPContentType              = null,
                                   TimeSpan?                                                  RequestTimeout               = null,
                                   TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                   UInt16?                                                    MaxNumberOfRetries           = null,
                                   UInt32?                                                    InternalBufferSize           = null,
                                   Boolean                                                    UseHTTPPipelining            = false,
                                   String?                                                    LoggingPath                  = null,
                                   String?                                                    LoggingContext               = null,
                                   LogfileCreatorDelegate?                                    LogfileCreator               = null,
                                   Boolean?                                                   DisableLogging               = false,
                                   HTTPClientLogger?                                          HTTPLogger                   = null,

                                   String?                                                    HTTPServerName               = null,
                                   IPPort?                                                    TCPPort                      = null,
                                   String?                                                    ServiceName                  = null,
                                   HTTPPath?                                                  URLPrefix                    = null,
                                   HTTPContentType?                                           ContentType                  = null,
                                   Boolean                                                    RegisterHTTPRootService      = true,
                                   DNSClient?                                                 DNSClient                    = null,
                                   Boolean                                                    AutoStart                    = false)

        {

            //this.CPClient = new ChargePointSOAPClient(
            //                    Id,
            //                    From,
            //                    To,

            //                    RemoteURL,
            //                    VirtualHostname,
            //                    Description,
            //                    PreferIPv4,
            //                    RemoteCertificateValidator,
            //                    LocalCertificateSelector,
            //                    ClientCert,
            //                    TLSProtocol,
            //                    HTTPUserAgent,
            //                    URLPathPrefix,
            //                    WSSLoginPassword,
            //                    HTTPContentType,
            //                    RequestTimeout,
            //                    TransmissionRetryDelay,
            //                    MaxNumberOfRetries,
            //                    InternalBufferSize,
            //                    UseHTTPPipelining,
            //                    LoggingPath,
            //                    LoggingContext,
            //                    LogfileCreator,
            //                    DisableLogging,
            //                    HTTPLogger,
            //                    DNSClient ?? this.DNSClient
            //                );

            //this.CPServer = new ChargePointSOAPServer(
            //                    HTTPServerName,
            //                    TCPPort,
            //                    ServiceName,
            //                    URLPrefix,
            //                    ContentType,
            //                    RegisterHTTPRootService,
            //                    DNSClient ?? this.DNSClient,
            //                    AutoStart
            //                );

            //WireEvents(CPServer);

        }

        #endregion


        #region WireEvents(CPServer)

        //public void WireEvents(ICPIncomingMessages CPServer)
        //{

        //    #region OnReset

        //    CPServer.OnReset += async (LogTimestamp,
        //                               Sender,
        //                               connection,
        //                               Request,
        //                               CancellationToken) => {

        //        #region Send OnResetRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnResetRequest?.Invoke(startTime,
        //                                   this,
        //                                   connection,
        //                                   Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnResetRequest));
        //        }

        //        #endregion


        //        await Task.Delay(10);


        //        ResetResponse? response = null;


        //        DebugX.Log(String.Concat("ChargeBox[", Id, "] Incoming '", Request.ResetType, "' reset request accepted."));
        //        response = new ResetResponse(Request,
        //                                     ResetStatus.Accepted);


        //        #region Send OnResetResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnResetResponse?.Invoke(responseTimestamp,
        //                                    this,
        //                                    connection,
        //                                    Request,
        //                                    response,
        //                                    responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnResetResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnChangeAvailability

        //    CPServer.OnChangeAvailability += async (LogTimestamp,
        //                                            Sender,
        //                                            connection,
        //                                            Request,
        //                                            CancellationToken) => {

        //        #region Send OnChangeAvailabilityRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnChangeAvailabilityRequest?.Invoke(startTime,
        //                                                this,
        //                                                connection,
        //                                                Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnChangeAvailabilityRequest));
        //        }

        //        #endregion


        //        await Task.Delay(10);


        //        ChangeAvailabilityResponse? response = null;


        //            DebugX.Log(String.Concat("ChargeBox[", Id, "] Incoming ChangeAvailability '", Request.Availability, "' request for connector '", Request.ConnectorId, "'."));

        //            if (connectors.ContainsKey(Request.ConnectorId))
        //            {

        //                connectors[Request.ConnectorId].Availability = Request.Availability;

        //                response = new ChangeAvailabilityResponse(Request,
        //                                                          AvailabilityStatus.Accepted);

        //            }
        //            else
        //                response = new ChangeAvailabilityResponse(Request,
        //                                                          AvailabilityStatus.Rejected);


        //        #region Send OnChangeAvailabilityResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnChangeAvailabilityResponse?.Invoke(responseTimestamp,
        //                                                 this,
        //                                                 connection,
        //                                                 Request,
        //                                                 response,
        //                                                 responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnChangeAvailabilityResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnGetConfiguration

        //    CPServer.OnGetConfiguration += async (LogTimestamp,
        //                                          Sender,
        //                                          connection,
        //                                          Request,
        //                                          CancellationToken) => {

        //        #region Send OnGetConfigurationRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnGetConfigurationRequest?.Invoke(startTime,
        //                                              this,
        //                                              connection,
        //                                              Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetConfigurationRequest));
        //        }

        //        #endregion


        //        await Task.Delay(10);


        //        GetConfigurationResponse? response = null;

        //        DebugX.Log(String.Concat("ChargeBox[", Id, "] Incoming get configuration request."));

        //        var configurationKeys  = new List<ConfigurationKey>();
        //        var unkownKeys         = new List<String>();

        //        if (Request.Keys.Any())
        //        {
        //            foreach (var key in Request.Keys)
        //            {

        //                if (Configuration.TryGetValue(key, out var configurationData))
        //                    configurationKeys.Add(new ConfigurationKey(key,
        //                                                               configurationData.AccessRights,
        //                                                               configurationData.Value));

        //                else
        //                    unkownKeys.Add(key);

        //            }
        //        }
        //        else
        //        {
        //            foreach (var configuration in Configuration)
        //            {
        //                configurationKeys.Add(new ConfigurationKey(configuration.Key,
        //                                                           configuration.Value.AccessRights,
        //                                                           configuration.Value.Value));
        //            }
        //        }


        //        response = new GetConfigurationResponse(Request,
        //                                                configurationKeys,
        //                                                unkownKeys);


        //        #region Send OnGetConfigurationResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnGetConfigurationResponse?.Invoke(responseTimestamp,
        //                                               this,
        //                                               connection,
        //                                               Request,
        //                                               response,
        //                                               responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetConfigurationResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnChangeConfiguration

        //    CPServer.OnChangeConfiguration += async (LogTimestamp,
        //                                             Sender,
        //                                             connection,
        //                                             Request,
        //                                             CancellationToken) => {

        //        #region Send OnChangeConfigurationRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnChangeConfigurationRequest?.Invoke(startTime,
        //                                                 this,
        //                                                 connection,
        //                                                 Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnChangeConfigurationRequest));
        //        }

        //        #endregion


        //        await Task.Delay(10);


        //        ChangeConfigurationResponse? response = null;



        //        DebugX.Log(String.Concat("ChargeBox[", Id, "] Incoming change configuration for '", Request.Key, "' with value '", Request.Value, "'."));

        //        if (Configuration.TryGetValue(Request.Key, out var configurationData))
        //        {
        //            if (configurationData.AccessRights == AccessRights.ReadOnly)
        //            {

        //                response                 = new ChangeConfigurationResponse(Request,
        //                                                                           ConfigurationStatus.Rejected);

        //            }
        //            else
        //            {

        //                configurationData.Value  = Request.Value;

        //                response                 = new ChangeConfigurationResponse(Request,
        //                                                                           configurationData.RebootRequired
        //                                                                               ? ConfigurationStatus.RebootRequired
        //                                                                               : ConfigurationStatus.Accepted);

        //            }
        //        }
        //        else
        //        {

        //            Configuration.Add(Request.Key,
        //                              new ConfigurationData(Request.Value,
        //                                                    AccessRights.ReadWrite,
        //                                                    false));

        //            response  = new ChangeConfigurationResponse(Request,
        //                                                        ConfigurationStatus.Accepted);

        //        }



        //        #region Send OnChangeConfigurationResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnChangeConfigurationResponse?.Invoke(responseTimestamp,
        //                                                  this,
        //                                                  connection,
        //                                                  Request,
        //                                                  response,
        //                                                  responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnChangeConfigurationResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnIncomingDataTransfer

        //    CPServer.OnIncomingDataTransfer += async (LogTimestamp,
        //                                              Sender,
        //                                              connection,
        //                                              request,
        //                                              cancellationToken) => {

        //        #region Send OnDataTransferRequest event

        //        var startTime = Timestamp.Now;

        //        var onIncomingDataTransferRequest = OnIncomingDataTransferRequest;
        //        if (onIncomingDataTransferRequest is not null)
        //        {
        //            try
        //            {

        //                await Task.WhenAll(onIncomingDataTransferRequest.GetInvocationList().
        //                                       OfType <OnIncomingDataTransferRequestDelegate>().
        //                                       Select (loggingDelegate => loggingDelegate.Invoke(startTime,
        //                                                                                         this,
        //                                                                                         connection,
        //                                                                                         request)).
        //                                       ToArray());

        //            }
        //            catch (Exception e)
        //            {
        //                await HandleErrors(
        //                          nameof(TestChargePoint),
        //                          nameof(OnIncomingDataTransferRequest),
        //                          e
        //                      );
        //            }

        //        }

        //        #endregion


        //        #region Check charging station identification

        //        DataTransferResponse? response = null;

        //        if (request.DestinationId != Id)
        //        {
        //            response = new DataTransferResponse(
        //                           Request:  request,
        //                           Result:   Result.GenericError(
        //                                         $"Charging station '{Id}': Invalid DataTransfer request for charging station '{request.DestinationId}'!"
        //                                     )
        //                       );
        //        }

        //        #endregion

        //        #region Check request signature(s)

        //        else
        //        {

        //            if (!SignaturePolicy.VerifyRequestMessage(
        //                     request,
        //                     request.ToJSON(
        //                         CustomIncomingDataTransferRequestSerializer,
        //                         CustomSignatureSerializer,
        //                         CustomCustomDataSerializer
        //                     ),
        //                     out var errorResponse
        //                 ))
        //            {

        //                response = new DataTransferResponse(
        //                               Request:  request,
        //                               Result:   Result.SignatureError(
        //                                             $"Invalid signature: {errorResponse}"
        //                                         )
        //                           );

        //            }

        //        #endregion

        //            else
        //            {

        //                DebugX.Log($"Charging Station '{Id}': Incoming data transfer request: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data ?? "-"}!");

        //                // VendorId
        //                // MessageId
        //                // Data

        //                var responseData = request.Data;

        //                if (request.Data is not null)
        //                {

        //                    if      (request.Data.Type == JTokenType.String)
        //                        responseData = request.Data.ToString().Reverse();

        //                    else if (request.Data.Type == JTokenType.Object) {

        //                        var responseObject = new JObject();

        //                        foreach (var property in (request.Data as JObject)!)
        //                        {
        //                            if (property.Value?.Type == JTokenType.String)
        //                                responseObject.Add(property.Key,
        //                                                   property.Value.ToString().Reverse());
        //                        }

        //                        responseData = responseObject;

        //                    }

        //                    else if (request.Data.Type == JTokenType.Array) {

        //                        var responseArray = new JArray();

        //                        foreach (var element in (request.Data as JArray)!)
        //                        {
        //                            if (element?.Type == JTokenType.String)
        //                                responseArray.Add(element.ToString().Reverse());
        //                        }

        //                        responseData = responseArray;

        //                    }

        //                }

        //                if (request.VendorId == Vendor_Id.GraphDefined)
        //                {
        //                    response = new DataTransferResponse(
        //                                   request,
        //                                   DataTransferStatus.Accepted,
        //                                   responseData
        //                               );
        //                }
        //                else
        //                    response = new DataTransferResponse(
        //                                   request,
        //                                   DataTransferStatus.Rejected
        //                               );

        //            }

        //        }

        //        #region Sign response message

        //        SignaturePolicy.SignResponseMessage(
        //            response,
        //            response.ToJSON(
        //                CustomIncomingDataTransferResponseSerializer,
        //                null, //CustomStatusInfoSerializer,
        //                CustomSignatureSerializer,
        //                CustomCustomDataSerializer
        //            ),
        //            out var errorResponse2);

        //        #endregion


        //        #region Send OnDataTransferResponse event

        //        var responseLogger = OnIncomingDataTransferResponse;
        //        if (responseLogger is not null)
        //        {

        //            var responseTime         = Timestamp.Now;

        //            var responseLoggerTasks  = responseLogger.GetInvocationList().
        //                                                      OfType <OnIncomingDataTransferResponseDelegate>().
        //                                                      Select (loggingDelegate => loggingDelegate.Invoke(responseTime,
        //                                                                                                        this,
        //                                                                                                        connection,
        //                                                                                                        request,
        //                                                                                                        response,
        //                                                                                                        responseTime - startTime)).
        //                                                      ToArray();

        //            try
        //            {
        //                await Task.WhenAll(responseLoggerTasks);
        //            }
        //            catch (Exception e)
        //            {
        //                await HandleErrors(
        //                          nameof(TestChargePoint),
        //                          nameof(OnIncomingDataTransferResponse),
        //                          e
        //                      );
        //            }

        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnGetDiagnostics

        //    CPServer.OnGetDiagnostics += async (LogTimestamp,
        //                                        Sender,
        //                                        connection,
        //                                        Request,
        //                                        CancellationToken) => {

        //        #region Send OnGetDiagnosticsRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnGetDiagnosticsRequest?.Invoke(startTime,
        //                                            this,
        //                                            connection,
        //                                            Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetDiagnosticsRequest));
        //        }

        //        #endregion


        //        await Task.Delay(10);


        //        GetDiagnosticsResponse? response = null;


        //            DebugX.Log(String.Concat("ChargeBox[", Id, "] Incoming get diagnostics request"));

        //            response = new GetDiagnosticsResponse(Request);


        //        #region Send OnGetDiagnosticsResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnGetDiagnosticsResponse?.Invoke(responseTimestamp,
        //                                             this,
        //                                             connection,
        //                                             Request,
        //                                             response,
        //                                             responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetDiagnosticsResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnTriggerMessage

        //    CPServer.OnTriggerMessage += async (LogTimestamp,
        //                                        Sender,
        //                                        connection,
        //                                        Request,
        //                                        CancellationToken) => {

        //        #region Send OnTriggerMessageRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnTriggerMessageRequest?.Invoke(startTime,
        //                                            this,
        //                                            connection,
        //                                            Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnTriggerMessageRequest));
        //        }

        //        #endregion


        //        await Task.Delay(10);


        //        TriggerMessageResponse? response = null;

        //        DebugX.Log($"ChargeBox[{Id}] Incoming TriggerMessage request for '" + Request.RequestedMessage + "' at connector '" + Request.ConnectorId + "'.");

        //        response = new TriggerMessageResponse(Request,
        //                                              TriggerMessageStatus.Rejected);


        //        #region Send OnTriggerMessageResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnTriggerMessageResponse?.Invoke(responseTimestamp,
        //                                             this,
        //                                             connection,
        //                                             Request,
        //                                             response,
        //                                             responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnTriggerMessageResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnUpdateFirmware

        //    CPServer.OnUpdateFirmware += async (LogTimestamp,
        //                                        Sender,
        //                                        connection,
        //                                        Request,
        //                                        CancellationToken) => {

        //        #region Send OnUpdateFirmwareRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnUpdateFirmwareRequest?.Invoke(startTime,
        //                                            this,
        //                                            connection,
        //                                            Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnUpdateFirmwareRequest));
        //        }

        //        #endregion


        //        await Task.Delay(10);


        //        UpdateFirmwareResponse? response = null;


        //        DebugX.Log($"ChargeBox[{Id}] Incoming UpdateFirmware request for '" + Request.FirmwareURL + "'.");

        //        response = new UpdateFirmwareResponse(Request);


        //        #region Send OnUpdateFirmwareResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnUpdateFirmwareResponse?.Invoke(responseTimestamp,
        //                                             this,
        //                                             connection,
        //                                             Request,
        //                                             response,
        //                                             responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnUpdateFirmwareResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion


        //    #region OnReserveNow

        //    CPServer.OnReserveNow += async (LogTimestamp,
        //                                    Sender,
        //                                    connection,
        //                                    Request,
        //                                    CancellationToken) => {

        //        #region Send OnReserveNowRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnReserveNowRequest?.Invoke(startTime,
        //                                        this,
        //                                        connection,
        //                                        Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnReserveNowRequest));
        //        }

        //        #endregion

        //        //transactionId1 = Request.ChargingProfile?.TransactionId;

        //        var response = new ReserveNowResponse(Request,
        //                                              ReservationStatus.Accepted);

        //        #region Send OnReserveNowResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnReserveNowResponse?.Invoke(responseTimestamp,
        //                                         this,
        //                                         connection,
        //                                         Request,
        //                                         response,
        //                                         responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnReserveNowResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnCancelReservation

        //    CPServer.OnCancelReservation += async (LogTimestamp,
        //                                           Sender,
        //                                           connection,
        //                                           Request,
        //                                           CancellationToken) => {

        //        #region Send OnCancelReservationRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnCancelReservationRequest?.Invoke(startTime,
        //                                               this,
        //                                               connection,
        //                                               Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnCancelReservationRequest));
        //        }

        //        #endregion

        //        //transactionId1 = Request.ChargingProfile?.TransactionId;

        //        var response = new CancelReservationResponse(Request,
        //                                                     CancelReservationStatus.Accepted);

        //        #region Send OnCancelReservationResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnCancelReservationResponse?.Invoke(responseTimestamp,
        //                                                this,
        //                                                connection,
        //                                                Request,
        //                                                response,
        //                                                responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnCancelReservationResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnRemoteStartTransaction

        //    CPServer.OnRemoteStartTransaction += async (LogTimestamp,
        //                                                Sender,
        //                                                connection,
        //                                                Request,
        //                                                CancellationToken) => {

        //        #region Send OnRemoteStartTransactionRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnRemoteStartTransactionRequest?.Invoke(startTime,
        //                                                    this,
        //                                                    connection,
        //                                                    Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnRemoteStartTransactionRequest));
        //        }

        //        #endregion


        //        await Task.Delay(10);


        //        RemoteStartTransactionResponse? response = null;


        //        {

        //            DebugX.Log($"ChargeBox[{Id}] Incoming RemoteStartTransaction for '" + Request.ConnectorId + "' with IdTag '" + Request.IdTag + "'.");

        //            // ToDo: lock(connectors)

        //            ChargePointConnector connector = null;

        //            if (!Request.ConnectorId.HasValue && connectors.Count == 1)
        //                connector = connectors.First().Value;

        //            else
        //                connectors.TryGetValue(Request.ConnectorId.Value, out connector);

        //            if (connector != null && connector.IsCharging == false)
        //            {

        //                connector.IsCharging         = true;
        //                connector.StartTimestamp     = Timestamp.Now;

        //                var startTransactionRequest  = new StartTransactionRequest(Id,
        //                                                                           Request.ConnectorId ?? Connector_Id.Parse(0),
        //                                                                           Request.IdTag,
        //                                                                           Timestamp.Now,
        //                                                                           connector.MeterStartValue,
        //                                                                           null); // ReservationId

        //                EnqueuedRequests.Add(new EnqueuedRequest("StartTransaction",
        //                                                     startTransactionRequest,
        //                                                     startTransactionRequest.ToJSON(),
        //                                                     Timestamp.Now,
        //                                                     EnqueuedRequest.EnqueuedStatus.New,
        //                                                     response => {
        //                                                         if (response is OCPP.WebSockets.OCPP_JSONResponseMessage wsResponseMessage &&
        //                                                             CS.StartTransactionResponse.TryParse(startTransactionRequest,
        //                                                                                                  wsResponseMessage.Payload,
        //                                                                                                  out var startTransactionResponse,
        //                                                                                                  out var ErrorResponse))
        //                                                         {


        //                                                             connector.IdToken          = Request.IdTag;
        //                                                             connector.ChargingProfile  = Request.ChargingProfile;
        //                                                             connector.IdTagInfo        = startTransactionResponse.IdTagInfo;
        //                                                             connector.TransactionId    = startTransactionResponse.TransactionId;

        //                                                             DebugX.Log(nameof(TestChargePoint), "Connector " + startTransactionRequest.ConnectorId + " started charging... " + startTransactionResponse.TransactionId);

        //                                                         }
        //                                                     }));

        //                // ToDo: StartTransaction request might fail!
        //                var statusNotificationRequest  = new StatusNotificationRequest(Id,
        //                                                                               Request.ConnectorId ?? Connector_Id.Parse(0),
        //                                                                               ChargePointStatus.Charging,
        //                                                                               ChargePointErrorCodes.NoError);

        //                EnqueuedRequests.Add(new EnqueuedRequest("StatusNotification",
        //                                                     statusNotificationRequest,
        //                                                     statusNotificationRequest.ToJSON(),
        //                                                     Timestamp.Now,
        //                                                     EnqueuedRequest.EnqueuedStatus.New,
        //                                                     response => {
        //                                                         //if (response is WebSockets.WSResponseMessage wsResponseMessage &&
        //                                                         //    CS.StartTransactionResponse.TryParse(startTransactionRequest,
        //                                                         //                                         wsResponseMessage.Message,
        //                                                         //                                         out CS.StartTransactionResponse  startTransactionResponse,
        //                                                         //                                         out String                       ErrorResponse))
        //                                                         //{
        //                                                         //    connector.IdToken          = Request.IdTag;
        //                                                         //    connector.ChargingProfile  = Request.ChargingProfile;
        //                                                         //    connector.IdTagInfo        = startTransactionResponse.IdTagInfo;
        //                                                         //    connector.TransactionId    = startTransactionResponse.TransactionId;
        //                                                         //    DebugX.Log(nameof(TestChargePoint), "Connector " + startTransactionRequest.ConnectorId + " started charging...");
        //                                                         //}
        //                                                     }));

        //                response = new RemoteStartTransactionResponse(Request,
        //                                                              RemoteStartStopStatus.Accepted);

        //            }
        //            else
        //                response = new RemoteStartTransactionResponse(Request,
        //                                                              RemoteStartStopStatus.Rejected);

        //        }


        //        #region Send OnRemoteStartTransactionResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnRemoteStartTransactionResponse?.Invoke(responseTimestamp,
        //                                                     this,
        //                                                     connection,
        //                                                     Request,
        //                                                     response,
        //                                                     responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnRemoteStartTransactionResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnRemoteStopTransaction

        //    CPServer.OnRemoteStopTransaction += async (LogTimestamp,
        //                                               Sender,
        //                                               connection,
        //                                               Request,
        //                                               CancellationToken) => {

        //        #region Send OnRemoteStopTransactionRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnRemoteStopTransactionRequest?.Invoke(startTime,
        //                                                   this,
        //                                                   connection,
        //                                                   Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnRemoteStopTransactionRequest));
        //        }

        //        #endregion


        //        await Task.Delay(10);


        //        RemoteStopTransactionResponse? response = null;


        //        DebugX.Log($"ChargeBox[{Id}] Incoming RemoteStopTransaction for '" + Request.TransactionId + "'.");

        //        // ToDo: lock(connectors)

        //        var connector = connectors.Values.Where(conn => conn.IsCharging && conn.TransactionId == Request.TransactionId).FirstOrDefault();

        //        if (connector != null)
        //        {

        //            connector.StopTimestamp  = Timestamp.Now;
        //            connector.IsCharging     = false;

        //            var stopTransactionRequest = new StopTransactionRequest(Id,
        //                                                                    Request.TransactionId,
        //                                                                    Timestamp.Now,
        //                                                                    connector.MeterStopValue,
        //                                                                    null,  // IdTag
        //                                                                    Reasons.Remote,
        //                                                                    null);

        //            EnqueuedRequests.Add(new EnqueuedRequest("StopTransaction",
        //                                                 stopTransactionRequest, // TransactionData
        //                                                 stopTransactionRequest.ToJSON(),
        //                                                 Timestamp.Now,
        //                                                 EnqueuedRequest.EnqueuedStatus.New,
        //                                                 response => {
        //                                                     if (response is OCPP.WebSockets.OCPP_JSONResponseMessage wsResponseMessage &&
        //                                                         CS.StopTransactionResponse.TryParse(stopTransactionRequest,
        //                                                                                             wsResponseMessage.Payload,
        //                                                                                             out var stopTransactionResponse,
        //                                                                                             out var ErrorResponse))
        //                                                     {
        //                                                         DebugX.Log(nameof(TestChargePoint), "Connector " + connector.Id + " stopped charging...");
        //                                                     }
        //                                                 }));


        //            // ToDo: StopTransaction request might fail!
        //            var statusNotificationRequest  = new StatusNotificationRequest(Id,
        //                                                                           connector.Id,
        //                                                                           ChargePointStatus.Available,
        //                                                                           ChargePointErrorCodes.NoError);

        //            EnqueuedRequests.Add(new EnqueuedRequest("StatusNotification",
        //                                                 statusNotificationRequest,
        //                                                 statusNotificationRequest.ToJSON(),
        //                                                 Timestamp.Now,
        //                                                 EnqueuedRequest.EnqueuedStatus.New,
        //                                                 response => {
        //                                                     //if (response is WebSockets.WSResponseMessage wsResponseMessage &&
        //                                                     //    CS.StartTransactionResponse.TryParse(startTransactionRequest,
        //                                                     //                                         wsResponseMessage.Message,
        //                                                     //                                         out CS.StartTransactionResponse  startTransactionResponse,
        //                                                     //                                         out String                       ErrorResponse))
        //                                                     //{
        //                                                     //    connector.IdToken          = Request.IdTag;
        //                                                     //    connector.ChargingProfile  = Request.ChargingProfile;
        //                                                     //    connector.IdTagInfo        = startTransactionResponse.IdTagInfo;
        //                                                     //    connector.TransactionId    = startTransactionResponse.TransactionId;
        //                                                     //    DebugX.Log(nameof(TestChargePoint), "Connector " + startTransactionRequest.ConnectorId + " started charging...");
        //                                                     //}
        //                                                 }));


        //            response = new RemoteStopTransactionResponse(Request,
        //                                                         RemoteStartStopStatus.Accepted);

        //        }
        //        else
        //            response = new RemoteStopTransactionResponse(Request,
        //                                                         RemoteStartStopStatus.Rejected);


        //        #region Send OnRemoteStopTransactionResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnRemoteStopTransactionResponse?.Invoke(responseTimestamp,
        //                                                    this,
        //                                                    connection,
        //                                                    Request,
        //                                                    response,
        //                                                    responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnRemoteStopTransactionResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnSetChargingProfile

        //    CPServer.OnSetChargingProfile += async (LogTimestamp,
        //                                            Sender,
        //                                            connection,
        //                                            Request,
        //                                            CancellationToken) => {

        //        #region Send OnSetChargingProfileRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnSetChargingProfileRequest?.Invoke(startTime,
        //                                                this,
        //                                                connection,
        //                                                Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSetChargingProfileRequest));
        //        }

        //        #endregion


        //        await Task.Delay(10);


        //        SetChargingProfileResponse? response = null;

        //        if (Request.ChargingProfile is null)
        //        {

        //            response = new SetChargingProfileResponse(Request,
        //                                                      ChargingProfileStatus.Rejected);

        //        }
        //        else
        //        {

        //            DebugX.Log($"ChargeBox[{Id}] Incoming SetChargingProfile for '" + Request.ConnectorId + "'.");

        //            // ToDo: lock(connectors)

        //            if (Request.ConnectorId.ToString() == "0")
        //            {
        //                foreach (var conn in connectors.Values)
        //                {

        //                    if (!Request.ChargingProfile.TransactionId.HasValue)
        //                        conn.ChargingProfile = Request.ChargingProfile;

        //                    else if (conn.TransactionId == Request.ChargingProfile.TransactionId.Value)
        //                        conn.ChargingProfile = Request.ChargingProfile;

        //                }
        //            }
        //            else if (connectors.ContainsKey(Request.ConnectorId))
        //            {

        //                connectors[Request.ConnectorId].ChargingProfile = Request.ChargingProfile;

        //                response = new SetChargingProfileResponse(Request,
        //                                                          ChargingProfileStatus.Accepted);

        //            }
        //            else
        //                response = new SetChargingProfileResponse(Request,
        //                                                          ChargingProfileStatus.Rejected);

        //        }


        //        #region Send OnSetChargingProfileResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnSetChargingProfileResponse?.Invoke(responseTimestamp,
        //                                                 this,
        //                                                 connection,
        //                                                 Request,
        //                                                 response,
        //                                                 responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSetChargingProfileResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnClearChargingProfile

        //    CPServer.OnClearChargingProfile += async (LogTimestamp,
        //                                              Sender,
        //                                              connection,
        //                                              Request,
        //                                              CancellationToken) => {

        //        #region Send OnClearChargingProfileRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnClearChargingProfileRequest?.Invoke(startTime,
        //                                                  this,
        //                                                  connection,
        //                                                  Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnClearChargingProfileRequest));
        //        }

        //        #endregion


        //        ClearChargingProfileResponse? response = null;



        //        #region Send OnClearChargingProfileResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnClearChargingProfileResponse?.Invoke(responseTimestamp,
        //                                                   this,
        //                                                   connection,
        //                                                   Request,
        //                                                   response,
        //                                                   responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnClearChargingProfileResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnGetCompositeSchedule

        //    CPServer.OnGetCompositeSchedule += async (LogTimestamp,
        //                                              Sender,
        //                                              connection,
        //                                              Request,
        //                                              CancellationToken) => {

        //        #region Send OnGetCompositeScheduleRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnGetCompositeScheduleRequest?.Invoke(startTime,
        //                                                  this,
        //                                                  connection,
        //                                                  Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetCompositeScheduleRequest));
        //        }

        //        #endregion


        //        GetCompositeScheduleResponse? response = null;



        //        #region Send OnGetCompositeScheduleResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnGetCompositeScheduleResponse?.Invoke(responseTimestamp,
        //                                                   this,
        //                                                   connection,
        //                                                   Request,
        //                                                   response,
        //                                                   responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetCompositeScheduleResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnUnlockConnector

        //    CPServer.OnUnlockConnector += async (LogTimestamp,
        //                                         Sender,
        //                                         connection,
        //                                         Request,
        //                                         CancellationToken) => {

        //        #region Send OnUnlockConnectorRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnUnlockConnectorRequest?.Invoke(startTime,
        //                                             this,
        //                                             connection,
        //                                             Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnUnlockConnectorRequest));
        //        }

        //        #endregion


        //        await Task.Delay(10);


        //        UnlockConnectorResponse? response = null;


        //        DebugX.Log($"ChargeBox[{Id}] Incoming UnlockConnector for '" + Request.ConnectorId + "'.");

        //        // ToDo: lock(connectors)

        //        if (connectors.ContainsKey(Request.ConnectorId))
        //        {

        //            // What to do here?!

        //            response = new UnlockConnectorResponse(Request,
        //                                                   UnlockStatus.Unlocked);

        //        }
        //        else
        //            response = new UnlockConnectorResponse(Request,
        //                                                   UnlockStatus.UnlockFailed);


        //        #region Send OnUnlockConnectorResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnUnlockConnectorResponse?.Invoke(responseTimestamp,
        //                                              this,
        //                                              connection,
        //                                              Request,
        //                                              response,
        //                                              responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnUnlockConnectorResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion


        //    #region OnGetLocalListVersion

        //    CPServer.OnGetLocalListVersion += async (LogTimestamp,
        //                                             Sender,
        //                                             connection,
        //                                             Request,
        //                                             CancellationToken) => {

        //        #region Send OnGetLocalListVersionRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnGetLocalListVersionRequest?.Invoke(startTime,
        //                                                 this,
        //                                                 connection,
        //                                                 Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetLocalListVersionRequest));
        //        }

        //        #endregion


        //        await Task.Delay(10);


        //        GetLocalListVersionResponse? response = null;



        //        DebugX.Log($"ChargeBox[{Id}] Incoming GetLocalListVersion request.");

        //        response = new GetLocalListVersionResponse(Request,
        //                                                   0);


        //        #region Send OnGetLocalListVersionResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnGetLocalListVersionResponse?.Invoke(responseTimestamp,
        //                                                  this,
        //                                                  connection,
        //                                                  Request,
        //                                                  response,
        //                                                  responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnGetLocalListVersionResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnSendLocalList

        //    CPServer.OnSendLocalList += async (LogTimestamp,
        //                                       Sender,
        //                                       connection,
        //                                       Request,
        //                                       CancellationToken) => {

        //        #region Send OnSendLocalListRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnSendLocalListRequest?.Invoke(startTime,
        //                                           this,
        //                                           connection,
        //                                           Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSendLocalListRequest));
        //        }

        //        #endregion


        //        await Task.Delay(10);


        //        SendLocalListResponse? response = null;


        //        DebugX.Log($"ChargeBox[{Id}] Incoming SendLocalList request: '" + Request.UpdateType + "' version '" + Request.ListVersion + "'.");

        //        response = new SendLocalListResponse(Request,
        //                                             UpdateStatus.NotSupported);


        //        #region Send OnSendLocalListResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnSendLocalListResponse?.Invoke(responseTimestamp,
        //                                            this,
        //                                            connection,
        //                                            Request,
        //                                            response,
        //                                            responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSendLocalListResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion

        //    #region OnClearCache

        //    CPServer.OnClearCache += async (LogTimestamp,
        //                                    Sender,
        //                                    connection,
        //                                    Request,
        //                                    CancellationToken) => {

        //        #region Send OnClearCacheRequest event

        //        var startTime = Timestamp.Now;

        //        try
        //        {

        //            OnClearCacheRequest?.Invoke(startTime,
        //                                        this,
        //                                        connection,
        //                                        Request);
        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnClearCacheRequest));
        //        }

        //        #endregion


        //        await Task.Delay(10);


        //        ClearCacheResponse? response = null;


        //        DebugX.Log($"ChargeBox[{Id}] Incoming ClearCache request.");

        //        response = new ClearCacheResponse(Request,
        //                                          ClearCacheStatus.Rejected);


        //        #region Send OnClearCacheResponse event

        //        try
        //        {

        //            var responseTimestamp = Timestamp.Now;

        //            OnClearCacheResponse?.Invoke(responseTimestamp,
        //                                         this,
        //                                         connection,
        //                                         Request,
        //                                         response,
        //                                         responseTimestamp - startTime);

        //        }
        //        catch (Exception e)
        //        {
        //            DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnClearCacheResponse));
        //        }

        //        #endregion

        //        return response;

        //    };

        //    #endregion


        //    //ToDo: Add security extensions

        //    //ToDo: Add Binary Data Streams Extensions

        //}

        #endregion




        public String? ClientCloseMessage => throw new NotImplementedException();

        public URL RemoteURL => throw new NotImplementedException();

        public HTTPHostname? VirtualHostname => throw new NotImplementedException();

     //   string? IHTTPClient.Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public RemoteTLSServerCertificateValidationHandler<IWebSocketClient>? RemoteCertificateValidator => throw new NotImplementedException();

        public X509Certificate? ClientCert => throw new NotImplementedException();

        public SslProtocols TLSProtocol => throw new NotImplementedException();

        public Boolean PreferIPv4 => throw new NotImplementedException();

        public String HTTPUserAgent => throw new NotImplementedException();

        public TimeSpan RequestTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TransmissionRetryDelayDelegate TransmissionRetryDelay => throw new NotImplementedException();

        public UInt16 MaxNumberOfRetries => throw new NotImplementedException();

        public Boolean UseHTTPPipelining => throw new NotImplementedException();

        public HTTPClientLogger? HTTPLogger => throw new NotImplementedException();

     //   RemoteTLSServerCertificateValidationHandler<IHTTPClient>? IHTTPClient.RemoteCertificateValidator => throw new NotImplementedException();



        #region Charge Point -> Central System Messages

        //#region BootNotification                 (Request)

        ///// <summary>
        ///// Send a boot notification to the central system.
        ///// </summary>
        ///// <param name="Request">A BootNotification request.</param>
        //public async Task<CS.BootNotificationResponse> BootNotification(BootNotificationRequest Request)
        //{

        //    #region Send OnBootNotificationRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnBootNotificationRequest?.Invoke(startTime,
        //                                          this,
        //                                          Request);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnBootNotificationRequest));
        //    }

        //    #endregion


        //    CS.BootNotificationResponse? response = null;

        //    if (CPClient is not null)
        //        response = await CPClient.BootNotification   (Request);

        //    if (response is not null)
        //    {
        //        switch (response.Status)
        //        {

        //            case RegistrationStatus.Accepted:
        //                this.CentralSystemTime      = response.CurrentTime;
        //                this.SendHeartbeatEvery     = response.HeartbeatInterval >= TimeSpan.FromSeconds(5) ? response.HeartbeatInterval : TimeSpan.FromSeconds(5);
        //                this.SendHeartbeatTimer.Change(this.SendHeartbeatEvery, this.SendHeartbeatEvery);
        //                this.DisableSendHeartbeats  = false;
        //                break;

        //            case RegistrationStatus.Pending:
        //                // Do not reconnect before: response.HeartbeatInterval
        //                break;

        //            case RegistrationStatus.Rejected:
        //                // Do not reconnect before: response.HeartbeatInterval
        //                break;

        //        }
        //    }

        //    response ??= new CS.BootNotificationResponse(Request,
        //                                                 Result.Server("Response is null!"));


        //    #region Send OnBootNotificationResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnBootNotificationResponse?.Invoke(endTime,
        //                                           this,
        //                                           Request,
        //                                           response,
        //                                           endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnBootNotificationResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region Heartbeat                        (Request)

        ///// <summary>
        ///// Send a heartbeat to the central system.
        ///// </summary>
        ///// <param name="Request">A Heartbeat request.</param>
        //public async Task<CS.HeartbeatResponse> Heartbeat(HeartbeatRequest Request)
        //{

        //    #region Send OnHeartbeatRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnHeartbeatRequest?.Invoke(startTime,
        //                                   this,
        //                                   Request);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnHeartbeatRequest));
        //    }

        //    #endregion


        //    CS.HeartbeatResponse? response = null;

        //    if (CPClient is not null)
        //        response = await CPClient.Heartbeat   (Request);

        //    if (response is not null)
        //    {
        //        this.CentralSystemTime = response.CurrentTime;
        //    }

        //    response ??= new CS.HeartbeatResponse(Request,
        //                                          Result.Server("Response is null!"));


        //    #region Send OnHeartbeatResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnHeartbeatResponse?.Invoke(endTime,
        //                                    this,
        //                                    Request,
        //                                    response,
        //                                    endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnHeartbeatResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region DiagnosticsStatusNotification    (Request)

        ///// <summary>
        ///// Send a diagnostics status notification to the central system.
        ///// </summary>
        ///// <param name="Request">A DiagnosticsStatusNotification request.</param>
        //public async Task<CS.DiagnosticsStatusNotificationResponse> DiagnosticsStatusNotification(DiagnosticsStatusNotificationRequest Request)
        //{

        //    #region Send OnDiagnosticsStatusNotificationRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnDiagnosticsStatusNotificationRequest?.Invoke(startTime,
        //                                                       this,
        //                                                       Request);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
        //    }

        //    #endregion


        //    CS.DiagnosticsStatusNotificationResponse? response = null;

        //    if (CPClient is not null)
        //        response = await CPClient.DiagnosticsStatusNotification   (Request);

        //    response ??= new CS.DiagnosticsStatusNotificationResponse(Request,
        //                                                              Result.Server("Response is null!"));


        //    #region Send OnDiagnosticsStatusNotificationResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnDiagnosticsStatusNotificationResponse?.Invoke(endTime,
        //                                                        this,
        //                                                        Request,
        //                                                        response,
        //                                                        endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region FirmwareStatusNotification       (Request)

        ///// <summary>
        ///// Send a firmware status notification to the central system.
        ///// </summary>
        ///// <param name="Request">A FirmwareStatusNotification request.</param>
        //public async Task<CS.FirmwareStatusNotificationResponse> FirmwareStatusNotification(FirmwareStatusNotificationRequest Request)
        //{

        //    #region Send OnFirmwareStatusNotificationRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnFirmwareStatusNotificationRequest?.Invoke(startTime,
        //                                                    this,
        //                                                    Request);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnFirmwareStatusNotificationRequest));
        //    }

        //    #endregion


        //    CS.FirmwareStatusNotificationResponse? response = null;

        //    if (CPClient is not null)
        //        response = await CPClient.FirmwareStatusNotification   (Request);

        //    response ??= new CS.FirmwareStatusNotificationResponse(Request,
        //                                                           Result.Server("Response is null!"));


        //    #region Send OnFirmwareStatusNotificationResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnFirmwareStatusNotificationResponse?.Invoke(endTime,
        //                                                     this,
        //                                                     Request,
        //                                                     response,
        //                                                     endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnFirmwareStatusNotificationResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion


        //#region Authorize                        (Request)

        ///// <summary>
        ///// Authorize the given (RFID) token.
        ///// </summary>
        ///// <param name="Request">An Authorize request.</param>
        //public async Task<CS.AuthorizeResponse> Authorize(AuthorizeRequest Request)
        //{

        //    #region Send OnAuthorizeRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnAuthorizeRequest?.Invoke(startTime,
        //                                   this,
        //                                   Request);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnAuthorizeRequest));
        //    }

        //    #endregion


        //    CS.AuthorizeResponse? response = null;

        //    if (CPClient is not null)
        //        response = await CPClient.Authorize   (Request);

        //    response ??= new CS.AuthorizeResponse(Request,
        //                                          Result.Server("Response is null!"));


        //    #region Send OnAuthorizeResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnAuthorizeResponse?.Invoke(endTime,
        //                                    this,
        //                                    Request,
        //                                    response,
        //                                    endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnAuthorizeResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region StartTransaction                 (Request)

        ///// <summary>
        ///// Send a notification about a started charging process at the given connector.
        ///// </summary>
        ///// <param name="Request">A StartTransaction request.</param>
        //public async Task<CS.StartTransactionResponse> StartTransaction(StartTransactionRequest Request)
        //{

        //    #region Send OnStartTransactionRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnStartTransactionRequest?.Invoke(startTime,
        //                                          this,
        //                                          Request);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStartTransactionRequest));
        //    }

        //    #endregion


        //    CS.StartTransactionResponse? response = null;

        //    if (CPClient is not null)
        //        response = await CPClient.StartTransaction   (Request);

        //    if (response is not null)
        //    {

        //        if (connectors.TryGetValue(Request.ConnectorId, out var connector))
        //        {

        //            connector.IsCharging     = true;
        //            connector.IdToken        = Request.IdTag;
        //            connector.IdTagInfo      = response.IdTagInfo;
        //            connector.TransactionId  = response.TransactionId;

        //            DebugX.Log(nameof(TestChargePoint), "Connector " + Request.ConnectorId + " started (local) charging with transaction identification " + response.TransactionId + "...");

        //        }
        //        else
        //            DebugX.Log(nameof(TestChargePoint), "Unkown connector " + Request.ConnectorId + "!");

        //    }

        //    response ??= new CS.StartTransactionResponse(Request,
        //                                                 OCPP.Result.Server("Response is null!"));


        //    #region Send OnStartTransactionResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnStartTransactionResponse?.Invoke(endTime,
        //                                           this,
        //                                           Request,
        //                                           response,
        //                                           endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStartTransactionResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region StatusNotification               (Request)

        ///// <summary>
        ///// Send a status notification for the given connector.
        ///// </summary>
        ///// <param name="Request">A StatusNotification request.</param>
        //public async Task<CS.StatusNotificationResponse> StatusNotification(StatusNotificationRequest Request)
        //{

        //    #region Send OnStatusNotificationRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnStatusNotificationRequest?.Invoke(startTime,
        //                                            this,
        //                                            Request);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStatusNotificationRequest));
        //    }

        //    #endregion


        //    CS.StatusNotificationResponse? response = null;

        //    if (CPClient is not null)
        //        response = await CPClient.StatusNotification   (Request);

        //    response ??= new CS.StatusNotificationResponse(Request,
        //                                                   Result.Server("Response is null!"));


        //    #region Send OnStatusNotificationResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnStatusNotificationResponse?.Invoke(endTime,
        //                                             this,
        //                                             Request,
        //                                             response,
        //                                             endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStatusNotificationResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region MeterValues                      (Request)

        ///// <summary>
        ///// Send meter values for the given connector.
        ///// </summary>
        ///// <param name="Request">A MeterValues request.</param>
        //public async Task<CS.MeterValuesResponse> MeterValues(MeterValuesRequest Request)
        //{

        //    #region Send OnMeterValuesRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnMeterValuesRequest?.Invoke(startTime,
        //                                     this,
        //                                     Request);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnMeterValuesRequest));
        //    }

        //    #endregion


        //    CS.MeterValuesResponse? response = null;

        //    if (CPClient is not null)
        //        response = await CPClient.MeterValues   (Request);

        //    response ??= new CS.MeterValuesResponse(Request,
        //                                            Result.Server("Response is null!"));


        //    #region Send OnMeterValuesResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnMeterValuesResponse?.Invoke(endTime,
        //                                      this,
        //                                      Request,
        //                                      response,
        //                                      endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnMeterValuesResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region StopTransaction                  (Request)

        ///// <summary>
        ///// Stop a charging process at the given connector.
        ///// </summary>
        ///// <param name="Request">A StopTransaction request.</param>
        //public async Task<CS.StopTransactionResponse> StopTransaction(StopTransactionRequest Request)
        //{

        //    #region Send OnStopTransactionRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnStopTransactionRequest?.Invoke(startTime,
        //                                         this,
        //                                         Request);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStopTransactionRequest));
        //    }

        //    #endregion


        //    CS.StopTransactionResponse? response = null;

        //    if (CPClient is not null)
        //        response = await CPClient.StopTransaction   (Request);

        //    response ??= new CS.StopTransactionResponse(Request,
        //                                                Result.Server("Response is null!"));


        //    #region Send OnStopTransactionResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnStopTransactionResponse?.Invoke(endTime,
        //                                          this,
        //                                          Request,
        //                                          response,
        //                                          endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnStopTransactionResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion


        //#region DataTransfer                     (Request)

        ///// <summary>
        ///// Send the given vendor-specific data to the central system.
        ///// </summary>
        ///// <param name="Request">A DataTransfer request.</param>
        //public async Task<CS.DataTransferResponse> DataTransfer(CP.DataTransferRequest Request)
        //{

        //    #region Send OnDataTransferRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnDataTransferRequest?.Invoke(startTime,
        //                                      this,
        //                                      Request);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDataTransferRequest));
        //    }

        //    #endregion


        //    CS.DataTransferResponse? response = null;

        //    if (CPClient is not null)
        //        response = await CPClient.DataTransfer(Request);

        //    response ??= new CS.DataTransferResponse(Request,
        //                                             Result.Server("Response is null!"));


        //    #region Send OnDataTransferResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnDataTransferResponse?.Invoke(endTime,
        //                                       this,
        //                                       Request,
        //                                       response,
        //                                       endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnDataTransferResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion


        //// Security Extensions

        //#region LogStatusNotification            (Request)

        ///// <summary>
        ///// Stop a charging process at the given connector.
        ///// </summary>
        ///// <param name="Request">A LogStatusNotification request.</param>
        //public async Task<CS.LogStatusNotificationResponse> LogStatusNotification(LogStatusNotificationRequest Request)
        //{

        //    #region Send OnLogStatusNotificationRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnLogStatusNotificationRequest?.Invoke(startTime,
        //                                               this,
        //                                               Request);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnLogStatusNotificationRequest));
        //    }

        //    #endregion


        //    CS.LogStatusNotificationResponse? response = null;

        //    if (CPClient is not null)
        //        response = await CPClient.LogStatusNotification(Request);

        //    response ??= new CS.LogStatusNotificationResponse(Request,
        //                                                Result.Server("Response is null!"));


        //    #region Send OnLogStatusNotificationResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnLogStatusNotificationResponse?.Invoke(endTime,
        //                                                this,
        //                                                Request,
        //                                                response,
        //                                                endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnLogStatusNotificationResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region SecurityEventNotification        (Request)

        ///// <summary>
        ///// Stop a charging process at the given connector.
        ///// </summary>
        ///// <param name="Request">A SendSecurityEventNotification request.</param>
        //public async Task<CS.SecurityEventNotificationResponse> SecurityEventNotification(SecurityEventNotificationRequest Request)
        //{

        //    #region Send OnSecurityEventNotificationRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnSecurityEventNotificationRequest?.Invoke(startTime,
        //                                                   this,
        //                                                   Request);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSecurityEventNotificationRequest));
        //    }

        //    #endregion


        //    CS.SecurityEventNotificationResponse? response = null;

        //    if (CPClient is not null)
        //        response = await CPClient.SecurityEventNotification(Request);

        //    response ??= new CS.SecurityEventNotificationResponse(Request,
        //                                                          Result.Server("Response is null!"));


        //    #region Send OnSecurityEventNotificationResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnSecurityEventNotificationResponse?.Invoke(endTime,
        //                                                    this,
        //                                                    Request,
        //                                                    response,
        //                                                    endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSecurityEventNotificationResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region SignCertificate                  (Request)

        ///// <summary>
        ///// Stop a charging process at the given connector.
        ///// </summary>
        ///// <param name="Request">A SignCertificate request.</param>
        //public async Task<CS.SignCertificateResponse> SignCertificate(SignCertificateRequest Request)
        //{

        //    #region Send OnSignCertificateRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnSignCertificateRequest?.Invoke(startTime,
        //                                         this,
        //                                         Request);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSignCertificateRequest));
        //    }

        //    #endregion


        //    CS.SignCertificateResponse? response = null;

        //    if (CPClient is not null)
        //        response = await CPClient.SignCertificate(Request);

        //    response ??= new CS.SignCertificateResponse(Request,
        //                                                Result.Server("Response is null!"));


        //    #region Send OnSignCertificateResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnSignCertificateResponse?.Invoke(endTime,
        //                                          this,
        //                                          Request,
        //                                          response,
        //                                          endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSignCertificateResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion

        //#region SignedFirmwareStatusNotification (Request)

        ///// <summary>
        ///// Stop a charging process at the given connector.
        ///// </summary>
        ///// <param name="Request">A SignedFirmwareStatusNotification request.</param>
        //public async Task<CS.SignedFirmwareStatusNotificationResponse> SignedFirmwareStatusNotification(SignedFirmwareStatusNotificationRequest Request)
        //{

        //    #region Send OnSignedFirmwareStatusNotificationRequest event

        //    var startTime = Timestamp.Now;

        //    try
        //    {

        //        OnSignedFirmwareStatusNotificationRequest?.Invoke(startTime,
        //                                                          this,
        //                                                          Request);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSignedFirmwareStatusNotificationRequest));
        //    }

        //    #endregion


        //    CS.SignedFirmwareStatusNotificationResponse? response = null;

        //    if (CPClient is not null)
        //        response = await CPClient.SignedFirmwareStatusNotification(Request);

        //    response ??= new CS.SignedFirmwareStatusNotificationResponse(Request,
        //                                                                 Result.Server("Response is null!"));


        //    #region Send OnSignedFirmwareStatusNotificationResponse event

        //    var endTime = Timestamp.Now;

        //    try
        //    {

        //        OnSignedFirmwareStatusNotificationResponse?.Invoke(endTime,
        //                                                           this,
        //                                                           Request,
        //                                                           response,
        //                                                           endTime - startTime);

        //    }
        //    catch (Exception e)
        //    {
        //        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnSignedFirmwareStatusNotificationResponse));
        //    }

        //    #endregion

        //    return response;

        //}

        //#endregion


        //// Binary Data Streams Extensions

        //#region BinaryDataTransfer               (Request)

        /////// <summary>
        /////// Send the given vendor-specific binary data to the central system.
        /////// </summary>
        /////// <param name="Request">A BinaryDataTransfer request.</param>
        ////public async Task<BinaryDataTransferResponse>
        ////    BinaryDataTransfer(BinaryDataTransferRequest Request)

        ////{

        ////    #region Send OnBinaryDataTransferRequest event

        ////    var startTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnBinaryDataTransferRequestSent?.Invoke(startTime,
        ////                                            this,
        ////                                            Request);

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnBinaryDataTransferRequestSent));
        ////    }

        ////    #endregion


        ////    var response = CPClient is not null

        ////                       ? SignaturePolicy.SignRequestMessage(
        ////                             Request,
        ////                             Request.ToBinary(
        ////                                 CustomBinaryDataTransferRequestSerializer,
        ////                                 CustomBinarySignatureSerializer,
        ////                                 IncludeSignatures: false
        ////                             ),
        ////                             out var errorResponse
        ////                         )

        ////                             ? await CPClient.BinaryDataTransfer(Request)

        ////                             : new BinaryDataTransferResponse(
        ////                                   Request,
        ////                                   Result.SignatureError(errorResponse)
        ////                               )

        ////                       : new BinaryDataTransferResponse(
        ////                             Request,
        ////                             Result.UnknownOrUnreachable(Request.DestinationId)
        ////                         );

        ////    SignaturePolicy.VerifyResponseMessage(
        ////        response,
        ////        response.ToBinary(
        ////            CustomBinaryDataTransferResponseSerializer,
        ////            null, //CustomStatusInfoSerializer,
        ////            CustomBinarySignatureSerializer,
        ////            IncludeSignatures: false
        ////        ),
        ////        out errorResponse
        ////    );


        ////    #region Send OnBinaryDataTransferResponse event

        ////    var endTime = Timestamp.Now;

        ////    try
        ////    {

        ////        OnBinaryDataTransferResponseReceived?.Invoke(endTime,
        ////                                             this,
        ////                                             Request,
        ////                                             response,
        ////                                             endTime - startTime);

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnBinaryDataTransferResponseReceived));
        ////    }

        ////    #endregion

        ////    return response;

        ////}

        //#endregion

        #endregion


        #region Dispose()

        public void Dispose()
        {
            
        }

        #endregion


    }

}
