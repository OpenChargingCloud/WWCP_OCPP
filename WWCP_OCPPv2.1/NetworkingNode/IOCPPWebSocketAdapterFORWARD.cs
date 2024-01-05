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

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region OnBinaryDataTransferRequestFilter(ed)Delegate

    /// <summary>
    /// A BinaryDataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>>

        OnBinaryDataTransferRequestFilterDelegate(DateTime                    Timestamp,
                                                  IEventSender                Sender,
                                                  IWebSocketConnection        Connection,
                                                  BinaryDataTransferRequest   Request,
                                                  CancellationToken           CancellationToken);


    /// <summary>
    /// A BinaryDataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnBinaryDataTransferRequestFilteredDelegate(DateTime                                                                    Timestamp,
                                                    IEventSender                                                                Sender,
                                                    IWebSocketConnection                                                        Connection,
                                                    BinaryDataTransferRequest                                                   Request,
                                                    ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>   ForwardingDecision);

    #endregion

    #region OnDeleteFileRequestFilter(ed)Delegate

    /// <summary>
    /// A DeleteFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<DeleteFileRequest, DeleteFileResponse>>

        OnDeleteFileRequestFilterDelegate(DateTime               Timestamp,
                                          IEventSender           Sender,
                                          IWebSocketConnection   Connection,
                                          DeleteFileRequest      Request,
                                          CancellationToken      CancellationToken);


    /// <summary>
    /// A DeleteFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnDeleteFileRequestFilteredDelegate(DateTime                                                    Timestamp,
                                            IEventSender                                                Sender,
                                            IWebSocketConnection                                        Connection,
                                            DeleteFileRequest                                           Request,
                                            ForwardingDecision<DeleteFileRequest, DeleteFileResponse>   ForwardingDecision);

    #endregion

    #region OnGetFileRequestFilter(ed)Delegate

    /// <summary>
    /// A GetFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetFileRequest, GetFileResponse>>

        OnGetFileRequestFilterDelegate(DateTime               Timestamp,
                                          IEventSender           Sender,
                                          IWebSocketConnection   Connection,
                                          GetFileRequest      Request,
                                          CancellationToken      CancellationToken);


    /// <summary>
    /// A GetFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetFileRequestFilteredDelegate(DateTime                                                    Timestamp,
                                            IEventSender                                                Sender,
                                            IWebSocketConnection                                        Connection,
                                            GetFileRequest                                           Request,
                                            ForwardingDecision<GetFileRequest, GetFileResponse>   ForwardingDecision);

    #endregion

    #region OnListDirectoryRequestFilter(ed)Delegate

    /// <summary>
    /// A ListDirectory request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ListDirectoryRequest, ListDirectoryResponse>>

        OnListDirectoryRequestFilterDelegate(DateTime               Timestamp,
                                          IEventSender           Sender,
                                          IWebSocketConnection   Connection,
                                          ListDirectoryRequest      Request,
                                          CancellationToken      CancellationToken);


    /// <summary>
    /// A ListDirectory request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnListDirectoryRequestFilteredDelegate(DateTime                                                    Timestamp,
                                            IEventSender                                                Sender,
                                            IWebSocketConnection                                        Connection,
                                            ListDirectoryRequest                                           Request,
                                            ForwardingDecision<ListDirectoryRequest, ListDirectoryResponse>   ForwardingDecision);

    #endregion

    #region OnSendFileRequestFilter(ed)Delegate

    /// <summary>
    /// A SendFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SendFileRequest, SendFileResponse>>

        OnSendFileRequestFilterDelegate(DateTime               Timestamp,
                                          IEventSender           Sender,
                                          IWebSocketConnection   Connection,
                                          SendFileRequest      Request,
                                          CancellationToken      CancellationToken);


    /// <summary>
    /// A SendFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnSendFileRequestFilteredDelegate(DateTime                                                    Timestamp,
                                            IEventSender                                                Sender,
                                            IWebSocketConnection                                        Connection,
                                            SendFileRequest                                           Request,
                                            ForwardingDecision<SendFileRequest, SendFileResponse>   ForwardingDecision);

    #endregion



    /// <summary>
    /// A Reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ResetRequest, ResetResponse>>

        OnResetRequestFilterDelegate(DateTime               Timestamp,
                                     IEventSender           Sender,
                                     IWebSocketConnection   Connection,
                                     ResetRequest           Request,
                                     CancellationToken      CancellationToken);


    /// <summary>
    /// A Reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnResetRequestFilteredDelegate(DateTime                                          Timestamp,
                                       IEventSender                                      Sender,
                                       IWebSocketConnection                              Connection,
                                       ResetRequest                                      Request,
                                       ForwardingDecision<ResetRequest, ResetResponse>   ForwardingDecision);


    /// <summary>
    /// A BootNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<BootNotificationRequest, BootNotificationResponse>>

        OnBootNotificationRequestFilterDelegate(DateTime                  Timestamp,
                                                IEventSender              Sender,
                                                IWebSocketConnection      Connection,
                                                BootNotificationRequest   Request,
                                                CancellationToken         CancellationToken);


    /// <summary>
    /// A BootNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnBootNotificationRequestFilteredDelegate(DateTime                                                                Timestamp,
                                                  IEventSender                                                            Sender,
                                                  IWebSocketConnection                                                    Connection,
                                                  BootNotificationRequest                                                 Request,
                                                  ForwardingDecision<BootNotificationRequest, BootNotificationResponse>   ForwardingDecision);


    /// <summary>
    /// A DataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<DataTransferRequest, DataTransferResponse>>

        OnDataTransferRequestFilterDelegate(DateTime               Timestamp,
                                            IEventSender           Sender,
                                            IWebSocketConnection   Connection,
                                            DataTransferRequest    Request,
                                            CancellationToken      CancellationToken);


    /// <summary>
    /// A DataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket client connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnDataTransferRequestFilteredDelegate(DateTime                                                        Timestamp,
                                              IEventSender                                                    Sender,
                                              IWebSocketConnection                                            Connection,
                                              DataTransferRequest                                             Request,
                                              ForwardingDecision<DataTransferRequest, DataTransferResponse>   ForwardingDecision);




    public interface IOCPPWebSocketAdapterFORWARD
    {

        ForwardingResult                DefaultResult        { get; set; }

        IEnumerable<NetworkingNode_Id>  AnycastIdsAllowed    { get; }

        IEnumerable<NetworkingNode_Id>  AnycastIdsDenied     { get; }


        event OnDataTransferRequestFilterDelegate?          OnDataTransferRequest;
        event OnDataTransferRequestFilteredDelegate?        OnDataTransferRequestLogging;

        #region BinaryDataStreamsExtensions

        event OnBinaryDataTransferRequestFilterDelegate?    OnBinaryDataTransferRequest;
        event OnBinaryDataTransferRequestFilteredDelegate?  OnBinaryDataTransferRequestLogging;

        event OnDeleteFileRequestFilterDelegate?            OnDeleteFileRequest;
        event OnDeleteFileRequestFilteredDelegate?          OnDeleteFileRequestLogging;

        event OnGetFileRequestFilterDelegate?               OnGetFileRequest;
        event OnGetFileRequestFilteredDelegate?             OnGetFileRequestLogging;

        event OnListDirectoryRequestFilterDelegate?         OnListDirectoryRequest;
        event OnListDirectoryRequestFilteredDelegate?       OnListDirectoryRequestLogging;

        event OnSendFileRequestFilterDelegate?              OnSendFileRequest;
        event OnSendFileRequestFilteredDelegate?            OnSendFileRequestLogging;

        #endregion


        event OnBootNotificationRequestFilterDelegate?      OnBootNotificationRequest;
        event OnBootNotificationRequestFilteredDelegate?    OnBootNotificationRequestLogging;

        event OnResetRequestFilterDelegate?                 OnResetRequest;
        event OnResetRequestFilteredDelegate?               OnResetRequestLogging;


        event OnAddSignaturePolicyDelegate? OnAddSignaturePolicy;
        event OnAddUserRoleDelegate? OnAddUserRole;
        event OnAFRRSignalDelegate? OnAFRRSignal;
        event OnCancelReservationDelegate? OnCancelReservation;
        event OnCertificateSignedDelegate? OnCertificateSigned;
        event OnChangeAvailabilityDelegate? OnChangeAvailability;
        event OnClearCacheDelegate? OnClearCache;
        event OnClearChargingProfileDelegate? OnClearChargingProfile;
        event OnClearDisplayMessageDelegate? OnClearDisplayMessage;
        event OnClearVariableMonitoringDelegate? OnClearVariableMonitoring;
        event OnCostUpdatedDelegate? OnCostUpdated;
        event OnCustomerInformationDelegate? OnCustomerInformation;
        event OnDeleteCertificateDelegate? OnDeleteCertificate;
        
        event OnDeleteSignaturePolicyDelegate? OnDeleteSignaturePolicy;
        event OnDeleteUserRoleDelegate? OnDeleteUserRole;
        event OnGetBaseReportDelegate? OnGetBaseReport;
        event OnGetChargingProfilesDelegate? OnGetChargingProfiles;
        event OnGetCompositeScheduleDelegate? OnGetCompositeSchedule;
        event OnGetDefaultChargingTariffDelegate? OnGetDefaultChargingTariff;
        event OnGetDisplayMessagesDelegate? OnGetDisplayMessages;
        event OnGetInstalledCertificateIdsDelegate? OnGetInstalledCertificateIds;
        event OnGetLocalListVersionDelegate? OnGetLocalListVersion;
        event OnGetLogDelegate? OnGetLog;
        event OnGetMonitoringReportDelegate? OnGetMonitoringReport;
        event OnGetReportDelegate? OnGetReport;
        event OnGetTransactionStatusDelegate? OnGetTransactionStatus;
        event OnGetVariablesDelegate? OnGetVariables;
        event OnIncomingDataTransferDelegate? OnIncomingDataTransfer;
        event OnInstallCertificateDelegate? OnInstallCertificate;
        event OnNotifyAllowedEnergyTransferDelegate? OnNotifyAllowedEnergyTransfer;
        event OnNotifyCRLDelegate? OnNotifyCRL;
        event OnPublishFirmwareDelegate? OnPublishFirmware;
        event OnRemoveDefaultChargingTariffDelegate? OnRemoveDefaultChargingTariff;
        event OnRequestStartTransactionDelegate? OnRequestStartTransaction;
        event OnRequestStopTransactionDelegate? OnRequestStopTransaction;
        event OnReserveNowDelegate? OnReserveNow;
        event OnSendLocalListDelegate? OnSendLocalList;
        event OnSetChargingProfileDelegate? OnSetChargingProfile;
        event OnSetDefaultChargingTariffDelegate? OnSetDefaultChargingTariff;
        event OnSetDisplayMessageDelegate? OnSetDisplayMessage;
        event OnSetMonitoringBaseDelegate? OnSetMonitoringBase;
        event OnSetMonitoringLevelDelegate? OnSetMonitoringLevel;
        event OnSetNetworkProfileDelegate? OnSetNetworkProfile;
        event OnSetVariableMonitoringDelegate? OnSetVariableMonitoring;
        event OnSetVariablesDelegate? OnSetVariables;
        event OnTriggerMessageDelegate? OnTriggerMessage;
        event OnUnlockConnectorDelegate? OnUnlockConnector;
        event OnUnpublishFirmwareDelegate? OnUnpublishFirmware;
        event OnUpdateDynamicScheduleDelegate? OnUpdateDynamicSchedule;
        event OnUpdateFirmwareDelegate? OnUpdateFirmware;
        event OnUpdateSignaturePolicyDelegate? OnUpdateSignaturePolicy;
        event OnUpdateUserRoleDelegate? OnUpdateUserRole;
        event OnUsePriorityChargingDelegate? OnUsePriorityCharging;




        Task ProcessJSONRequestMessage   (OCPP_JSONRequestMessage     JSONRequestMessage);
        Task ProcessJSONResponseMessage  (OCPP_JSONResponseMessage    JSONResponseMessage);
        Task ProcessJSONErrorMessage     (OCPP_JSONErrorMessage       JSONErrorMessage);


        Task ProcessBinaryRequestMessage (OCPP_BinaryRequestMessage   BinaryRequestMessage);
        Task ProcessBinaryResponseMessage(OCPP_BinaryResponseMessage  BinaryResponseMessage);






        Task<ForwardingDecision<DataTransferRequest,       DataTransferResponse>>       ProcessDataTransfer      (DataTransferRequest       Request, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>> ProcessBinaryDataTransfer(BinaryDataTransferRequest Request, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        Task<ForwardingDecision<BootNotificationRequest,   BootNotificationResponse>>   ProcessBootNotification  (BootNotificationRequest   Request, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        Task<ForwardingDecision<ResetRequest,              ResetResponse>>              ProcessReset             (ResetRequest              Request, IWebSocketConnection Connection, CancellationToken CancellationToken = default);


    }

}
