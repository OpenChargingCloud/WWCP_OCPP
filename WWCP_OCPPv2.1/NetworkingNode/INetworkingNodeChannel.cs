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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.NN;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The common interface of all central systems channels.
    /// CSMS might have multiple channels, e.g. a SOAP and a WebSockets channel.
    /// </summary>
    public interface INetworkingNodeChannel  //INetworkingNodeOutgoingMessages
                                              // INetworkingNodeOutgoingMessageEvents,
                                              // INetworkingNodeIncomingMessages,
                                              // INetworkingNodeIncomingMessageEvents
                                             // OCPP.NN.INetworkingNodeOutgoingMessages,
                                             // OCPP.NN.CSMS.INetworkingNodeOutgoingMessages,

                                             // CS.  INetworkingNodeOutgoingMessages,
                                             // CS.  INetworkingNodeOutgoingMessageEvents,
                                             // CS.  INetworkingNodeIncomingMessages,
                                             // CS.  INetworkingNodeIncomingMessageEvents,

                                             // CSMS.INetworkingNodeOutgoingMessages
                                             // CSMS.INetworkingNodeOutgoingMessageEvents,
                                             // CSMS.INetworkingNodeIncomingMessages
                                             // CSMS.INetworkingNodeIncomingMessageEvents

    {

        IEnumerable<NetworkingNode_Id> NetworkingNodeIds { get; }


        #region HTTP Web Socket connection management

        /// <summary>
        /// An event sent whenever the HTTP connection switched successfully to web socket.
        /// </summary>
        event CSMS.OnNetworkingNodeNewWebSocketConnectionDelegate?    OnNetworkingNodeNewWebSocketConnection;

        /// <summary>
        /// An event sent whenever a web socket close frame was received.
        /// </summary>
        event CSMS.OnNetworkingNodeCloseMessageReceivedDelegate?      OnNetworkingNodeCloseMessageReceived;

        /// <summary>
        /// An event sent whenever a TCP connection was closed.
        /// </summary>
        event CSMS.OnNetworkingNodeTCPConnectionClosedDelegate?       OnNetworkingNodeTCPConnectionClosed;

        #endregion

        #region OnJSONMessage   (-Received/-ResponseSent/-ErrorResponseSent)

        event OnWebSocketJSONMessageRequestDelegate?      OnJSONMessageRequestReceived;

        event OnWebSocketJSONMessageResponseDelegate?     OnJSONMessageResponseSent;

        event OnWebSocketTextErrorResponseDelegate?       OnJSONErrorResponseSent;


        event OnWebSocketJSONMessageRequestDelegate?      OnJSONMessageRequestSent;

        event OnWebSocketJSONMessageResponseDelegate?     OnJSONMessageResponseReceived;

        event OnWebSocketTextErrorResponseDelegate?       OnJSONErrorResponseReceived;

        #endregion

        #region OnBinaryMessage (-Received/-ResponseSent/-ErrorResponseSent)

        event OnWebSocketBinaryMessageRequestDelegate?    OnBinaryMessageRequestReceived;

        event OnWebSocketBinaryMessageResponseDelegate?   OnBinaryMessageResponseSent;

        //event OnWebSocketBinaryErrorResponseDelegate?     OnBinaryErrorResponseSent;


        event OnWebSocketBinaryMessageRequestDelegate?    OnBinaryMessageRequestSent;

        event OnWebSocketBinaryMessageResponseDelegate?   OnBinaryMessageResponseReceived;

        //event OnWebSocketBinaryErrorResponseDelegate?     OnBinaryErrorResponseReceived;

        #endregion



        Task<OCPPv2_1.CS.ResetResponse>                       Reset                      (OCPPv2_1.CSMS.ResetRequest                       Request);
        Task<OCPPv2_1.CS.UpdateFirmwareResponse>              UpdateFirmware             (OCPPv2_1.CSMS.UpdateFirmwareRequest              Request);
        Task<OCPPv2_1.CS.PublishFirmwareResponse>             PublishFirmware            (OCPPv2_1.CSMS.PublishFirmwareRequest             Request);
        Task<OCPPv2_1.CS.UnpublishFirmwareResponse>           UnpublishFirmware          (OCPPv2_1.CSMS.UnpublishFirmwareRequest           Request);
        Task<OCPPv2_1.CS.GetBaseReportResponse>               GetBaseReport              (OCPPv2_1.CSMS.GetBaseReportRequest               Request);
        Task<OCPPv2_1.CS.GetReportResponse>                   GetReport                  (OCPPv2_1.CSMS.GetReportRequest                   Request);
        Task<OCPPv2_1.CS.GetLogResponse>                      GetLog                     (OCPPv2_1.CSMS.GetLogRequest                      Request);
        Task<OCPPv2_1.CS.SetVariablesResponse>                SetVariables               (OCPPv2_1.CSMS.SetVariablesRequest                Request);
        Task<OCPPv2_1.CS.GetVariablesResponse>                GetVariables               (OCPPv2_1.CSMS.GetVariablesRequest                Request);
        Task<OCPPv2_1.CS.SetMonitoringBaseResponse>           SetMonitoringBase          (OCPPv2_1.CSMS.SetMonitoringBaseRequest           Request);
        Task<OCPPv2_1.CS.GetMonitoringReportResponse>         GetMonitoringReport        (OCPPv2_1.CSMS.GetMonitoringReportRequest         Request);
        Task<OCPPv2_1.CS.SetMonitoringLevelResponse>          SetMonitoringLevel         (OCPPv2_1.CSMS.SetMonitoringLevelRequest          Request);
        Task<OCPPv2_1.CS.SetVariableMonitoringResponse>       SetVariableMonitoring      (OCPPv2_1.CSMS.SetVariableMonitoringRequest       Request);
        Task<OCPPv2_1.CS.ClearVariableMonitoringResponse>     ClearVariableMonitoring    (OCPPv2_1.CSMS.ClearVariableMonitoringRequest     Request);
        Task<OCPPv2_1.CS.SetNetworkProfileResponse>           SetNetworkProfile          (OCPPv2_1.CSMS.SetNetworkProfileRequest           Request);
        Task<OCPPv2_1.CS.ChangeAvailabilityResponse>          ChangeAvailability         (OCPPv2_1.CSMS.ChangeAvailabilityRequest          Request);
        Task<OCPPv2_1.CS.TriggerMessageResponse>              TriggerMessage             (OCPPv2_1.CSMS.TriggerMessageRequest              Request);

        Task<OCPPv2_1.CS.CertificateSignedResponse>           CertificateSigned          (OCPPv2_1.CSMS.CertificateSignedRequest           Request);
        Task<OCPPv2_1.CS.InstallCertificateResponse>          InstallCertificate         (OCPPv2_1.CSMS.InstallCertificateRequest          Request);
        Task<OCPPv2_1.CS.GetInstalledCertificateIdsResponse>  GetInstalledCertificateIds (OCPPv2_1.CSMS.GetInstalledCertificateIdsRequest  Request);
        Task<OCPPv2_1.CS.DeleteCertificateResponse>           DeleteCertificate          (OCPPv2_1.CSMS.DeleteCertificateRequest           Request);
        Task<OCPPv2_1.CS.NotifyCRLResponse>                   NotifyCRL                  (OCPPv2_1.CSMS.NotifyCRLRequest                   Request);

        Task<OCPPv2_1.CS.GetLocalListVersionResponse>         GetLocalListVersion        (OCPPv2_1.CSMS.GetLocalListVersionRequest         Request);
        Task<OCPPv2_1.CS.SendLocalListResponse>               SendLocalList              (OCPPv2_1.CSMS.SendLocalListRequest               Request);
        Task<OCPPv2_1.CS.ClearCacheResponse>                  ClearCache                 (OCPPv2_1.CSMS.ClearCacheRequest                  Request);

        Task<OCPPv2_1.CS.ReserveNowResponse>                  ReserveNow                 (OCPPv2_1.CSMS.ReserveNowRequest                  Request);
        Task<OCPPv2_1.CS.CancelReservationResponse>           CancelReservation          (OCPPv2_1.CSMS.CancelReservationRequest           Request);
        Task<OCPPv2_1.CS.RequestStartTransactionResponse>     RequestStartTransaction    (OCPPv2_1.CSMS.RequestStartTransactionRequest     Request);
        Task<OCPPv2_1.CS.RequestStopTransactionResponse>      RequestStopTransaction     (OCPPv2_1.CSMS.RequestStopTransactionRequest      Request);
        Task<OCPPv2_1.CS.GetTransactionStatusResponse>        GetTransactionStatus       (OCPPv2_1.CSMS.GetTransactionStatusRequest        Request);
        Task<OCPPv2_1.CS.SetChargingProfileResponse>          SetChargingProfile         (OCPPv2_1.CSMS.SetChargingProfileRequest          Request);
        Task<OCPPv2_1.CS.GetChargingProfilesResponse>         GetChargingProfiles        (OCPPv2_1.CSMS.GetChargingProfilesRequest         Request);
        Task<OCPPv2_1.CS.ClearChargingProfileResponse>        ClearChargingProfile       (OCPPv2_1.CSMS.ClearChargingProfileRequest        Request);
        Task<OCPPv2_1.CS.GetCompositeScheduleResponse>        GetCompositeSchedule       (OCPPv2_1.CSMS.GetCompositeScheduleRequest        Request);
        Task<OCPPv2_1.CS.UpdateDynamicScheduleResponse>       UpdateDynamicSchedule      (OCPPv2_1.CSMS.UpdateDynamicScheduleRequest       Request);
        Task<OCPPv2_1.CS.NotifyAllowedEnergyTransferResponse> NotifyAllowedEnergyTransfer(OCPPv2_1.CSMS.NotifyAllowedEnergyTransferRequest Request);
        Task<OCPPv2_1.CS.UsePriorityChargingResponse>         UsePriorityCharging        (OCPPv2_1.CSMS.UsePriorityChargingRequest         Request);
        Task<OCPPv2_1.CS.UnlockConnectorResponse>             UnlockConnector            (OCPPv2_1.CSMS.UnlockConnectorRequest             Request);

        Task<OCPPv2_1.CS.AFRRSignalResponse>                  AFRRSignal                 (OCPPv2_1.CSMS.AFRRSignalRequest                  Request);

        Task<OCPPv2_1.CS.SetDisplayMessageResponse>           SetDisplayMessage          (OCPPv2_1.CSMS.SetDisplayMessageRequest           Request);
        Task<OCPPv2_1.CS.GetDisplayMessagesResponse>          GetDisplayMessages         (OCPPv2_1.CSMS.GetDisplayMessagesRequest          Request);
        Task<OCPPv2_1.CS.ClearDisplayMessageResponse>         ClearDisplayMessage        (OCPPv2_1.CSMS.ClearDisplayMessageRequest         Request);
        Task<OCPPv2_1.CS.CostUpdatedResponse>                 CostUpdated                (OCPPv2_1.CSMS.CostUpdatedRequest                 Request);
        Task<OCPPv2_1.CS.CustomerInformationResponse>         CustomerInformation        (OCPPv2_1.CSMS.CustomerInformationRequest         Request);


        Task<DataTransferResponse>           DataTransfer         (          DataTransferRequest           Request);

        Task<AddSignaturePolicyResponse>     AddSignaturePolicy   (OCPP.CSMS.AddSignaturePolicyRequest     Request);
        Task<UpdateSignaturePolicyResponse>  UpdateSignaturePolicy(OCPP.CSMS.UpdateSignaturePolicyRequest  Request);
        Task<DeleteSignaturePolicyResponse>  DeleteSignaturePolicy(OCPP.CSMS.DeleteSignaturePolicyRequest  Request);

        Task<AddUserRoleResponse>            AddUserRole          (OCPP.CSMS.AddUserRoleRequest            Request);
        Task<UpdateUserRoleResponse>         UpdateUserRole       (OCPP.CSMS.UpdateUserRoleRequest         Request);
        Task<DeleteUserRoleResponse>         DeleteUserRole       (OCPP.CSMS.DeleteUserRoleRequest         Request);


        Task<BinaryDataTransferResponse>     BinaryDataTransfer   (          BinaryDataTransferRequest     Request);
        Task<GetFileResponse>                GetFile              (OCPP.CSMS.GetFileRequest                Request);
        Task<SendFileResponse>               SendFile             (OCPP.CSMS.SendFileRequest               Request);
        Task<DeleteFileResponse>             DeleteFile           (OCPP.CSMS.DeleteFileRequest             Request);


        Task<NotifyNetworkTopologyResponse>  NotifyNetworkTopology(          NotifyNetworkTopologyRequest  Request);

        Task<OCPPv2_1.CS.SetDefaultChargingTariffResponse>     SetDefaultChargingTariff   (OCPPv2_1.CSMS.SetDefaultChargingTariffRequest    Request);
        Task<OCPPv2_1.CS.GetDefaultChargingTariffResponse>     GetDefaultChargingTariff   (OCPPv2_1.CSMS.GetDefaultChargingTariffRequest    Request);
        Task<OCPPv2_1.CS.RemoveDefaultChargingTariffResponse>  RemoveDefaultChargingTariff(OCPPv2_1.CSMS.RemoveDefaultChargingTariffRequest Request);



        event OCPPv2_1.CSMS.OnFirmwareStatusNotificationDelegate?           OnFirmwareStatusNotification;
        event OCPPv2_1.CSMS.OnPublishFirmwareStatusNotificationDelegate?    OnPublishFirmwareStatusNotification;
        event OCPPv2_1.CSMS.OnHeartbeatDelegate?                            OnHeartbeat;
        event OCPPv2_1.CSMS.OnNotifyEventDelegate?                          OnNotifyEvent;
        event OCPPv2_1.CSMS.OnSecurityEventNotificationDelegate?            OnSecurityEventNotification;
        event OCPPv2_1.CSMS.OnNotifyReportDelegate?                         OnNotifyReport;
        event OCPPv2_1.CSMS.OnNotifyMonitoringReportDelegate?               OnNotifyMonitoringReport;
        event OCPPv2_1.CSMS.OnLogStatusNotificationDelegate?                OnLogStatusNotification;

        event OCPPv2_1.CSMS.OnSignCertificateDelegate?                      OnSignCertificate;
        event OCPPv2_1.CSMS.OnGet15118EVCertificateDelegate?                OnGet15118EVCertificate;
        event OCPPv2_1.CSMS.OnGetCertificateStatusDelegate?                 OnGetCertificateStatus;
        event OCPPv2_1.CSMS.OnGetCRLDelegate?                               OnGetCRL;

        event OCPPv2_1.CSMS.OnReservationStatusUpdateDelegate?              OnReservationStatusUpdate;
        event OCPPv2_1.CSMS.OnAuthorizeDelegate?                            OnAuthorize;
        event OCPPv2_1.CSMS.OnNotifyEVChargingNeedsDelegate?                OnNotifyEVChargingNeeds;
        event OCPPv2_1.CSMS.OnTransactionEventDelegate?                     OnTransactionEvent;
        event OCPPv2_1.CSMS.OnStatusNotificationDelegate?                   OnStatusNotification;
        event OCPPv2_1.CSMS.OnMeterValuesDelegate?                          OnMeterValues;
        event OCPPv2_1.CSMS.OnNotifyChargingLimitDelegate?                  OnNotifyChargingLimit;
        event OCPPv2_1.CSMS.OnClearedChargingLimitDelegate?                 OnClearedChargingLimit;
        event OCPPv2_1.CSMS.OnReportChargingProfilesDelegate?               OnReportChargingProfiles;
        event OCPPv2_1.CSMS.OnNotifyEVChargingScheduleDelegate?             OnNotifyEVChargingSchedule;
        event OCPPv2_1.CSMS.OnNotifyPriorityChargingDelegate?               OnNotifyPriorityCharging;
        event OCPPv2_1.CSMS.OnPullDynamicScheduleUpdateDelegate?            OnPullDynamicScheduleUpdate;

        event OCPPv2_1.CSMS.OnNotifyDisplayMessagesDelegate?                OnNotifyDisplayMessages;
        event OCPPv2_1.CSMS.OnNotifyCustomerInformationDelegate?            OnNotifyCustomerInformation;

        event OnIncomingBinaryDataTransferDelegate?                         OnIncomingBinaryDataTransfer;


        void AddStaticRouting   (NetworkingNode_Id DestinationNodeId,
                                 NetworkingNode_Id NetworkingHubId);

        void RemoveStaticRouting(NetworkingNode_Id DestinationNodeId,
                                 NetworkingNode_Id NetworkingHubId);


        /// <summary>
        /// Start the HTTP web socket listener thread.
        /// </summary>
        void Start();

        /// <summary>
        /// Shutdown the HTTP web socket listener thread.
        /// </summary>
        /// <param name="Message">An optional shutdown message.</param>
        /// <param name="Wait">Wait until the server finally shutted down.</param>
        Task Shutdown(String?  Message   = null,
                      Boolean  Wait      = true);

    }

}
