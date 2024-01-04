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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.NN;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NN
{

    public interface INetworkingNodeIN : NetworkingNode.     INetworkingNodeIncomingMessages,
                                         NetworkingNode.     INetworkingNodeIncomingMessageEvents,
                                         NetworkingNode.CS.  INetworkingNodeIncomingMessages,
                                         NetworkingNode.CS.  INetworkingNodeIncomingMessageEvents,
                                         NetworkingNode.CSMS.INetworkingNodeIncomingMessages,
                                         NetworkingNode.CSMS.INetworkingNodeIncomingMessageEvents
    {

        void WireEvents(NetworkingNode.CS.  INetworkingNodeIncomingMessages IncomingMessages);
        void WireEvents(NetworkingNode.CSMS.INetworkingNodeIncomingMessages IncomingMessages);


        #region Incoming Messages: Networking Node <- CSMS

        #region Reset

        Task RaiseOnResetRequest (DateTime               Timestamp,
                                  IEventSender           Sender,
                                  IWebSocketConnection   Connection,
                                  ResetRequest           Request);

        Task RaiseOnResetResponse(DateTime               Timestamp,
                                  IEventSender           Sender,
                                  IWebSocketConnection   Connection,
                                  ResetRequest           Request,
                                  ResetResponse          Response,
                                  TimeSpan               Runtime);

        #endregion

        #region UpdateFirmware

        Task RaiseOnUpdateFirmwareRequest (DateTime                 Timestamp,
                                           IEventSender             Sender,
                                           IWebSocketConnection     Connection,
                                           UpdateFirmwareRequest    Request);

        Task RaiseOnUpdateFirmwareResponse(DateTime                 Timestamp,
                                           IEventSender             Sender,
                                           IWebSocketConnection     Connection,
                                           UpdateFirmwareRequest    Request,
                                           UpdateFirmwareResponse   Response,
                                           TimeSpan                 Runtime);

        #endregion

        #region PublishFirmware

        Task RaiseOnPublishFirmwareRequest (DateTime                  Timestamp,
                                            IEventSender              Sender,
                                            IWebSocketConnection      Connection,
                                            PublishFirmwareRequest    Request);

        Task RaiseOnPublishFirmwareResponse(DateTime                  Timestamp,
                                            IEventSender              Sender,
                                            IWebSocketConnection      Connection,
                                            PublishFirmwareRequest    Request,
                                            PublishFirmwareResponse   Response,
                                            TimeSpan                  Runtime);

        #endregion

        #region UnpublishFirmware

        Task RaiseOnUnpublishFirmwareRequest (DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              IWebSocketConnection        Connection,
                                              UnpublishFirmwareRequest    Request);

        Task RaiseOnUnpublishFirmwareResponse(DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              IWebSocketConnection        Connection,
                                              UnpublishFirmwareRequest    Request,
                                              UnpublishFirmwareResponse   Response,
                                              TimeSpan                    Runtime);

        #endregion

        #region GetBaseReport

        Task RaiseOnGetBaseReportRequest (DateTime                Timestamp,
                                          IEventSender            Sender,
                                          IWebSocketConnection    Connection,
                                          GetBaseReportRequest    Request);

        Task RaiseOnGetBaseReportResponse(DateTime                Timestamp,
                                          IEventSender            Sender,
                                          IWebSocketConnection    Connection,
                                          GetBaseReportRequest    Request,
                                          GetBaseReportResponse   Response,
                                          TimeSpan                Runtime);

        #endregion

        #region GetReport

        Task RaiseOnGetReportRequest (DateTime               Timestamp,
                                      IEventSender           Sender,
                                      IWebSocketConnection   Connection,
                                      GetReportRequest       Request);

        Task RaiseOnGetReportResponse(DateTime               Timestamp,
                                      IEventSender           Sender,
                                      IWebSocketConnection   Connection,
                                      GetReportRequest       Request,
                                      GetReportResponse      Response,
                                      TimeSpan               Runtime);

        #endregion

        #region GetLog

        Task RaiseOnGetLogRequest (DateTime               Timestamp,
                                   IEventSender           Sender,
                                   IWebSocketConnection   Connection,
                                   GetLogRequest          Request);

        Task RaiseOnGetLogResponse(DateTime               Timestamp,
                                   IEventSender           Sender,
                                   IWebSocketConnection   Connection,
                                   GetLogRequest          Request,
                                   GetLogResponse         Response,
                                   TimeSpan               Runtime);

        #endregion

        #region SetVariables

        Task RaiseOnSetVariablesRequest (DateTime               Timestamp,
                                         IEventSender           Sender,
                                         IWebSocketConnection   Connection,
                                         SetVariablesRequest    Request);

        Task RaiseOnSetVariablesResponse(DateTime               Timestamp,
                                         IEventSender           Sender,
                                         IWebSocketConnection   Connection,
                                         SetVariablesRequest    Request,
                                         SetVariablesResponse   Response,
                                         TimeSpan               Runtime);

        #endregion

        #region GetVariables

        Task RaiseOnGetVariablesRequest (DateTime               Timestamp,
                                         IEventSender           Sender,
                                         IWebSocketConnection   Connection,
                                         GetVariablesRequest    Request);

        Task RaiseOnGetVariablesResponse(DateTime               Timestamp,
                                         IEventSender           Sender,
                                         IWebSocketConnection   Connection,
                                         GetVariablesRequest    Request,
                                         GetVariablesResponse   Response,
                                         TimeSpan               Runtime);

        #endregion

        #region SetMonitoringBase

        Task RaiseOnSetMonitoringBaseRequest (DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              IWebSocketConnection        Connection,
                                              SetMonitoringBaseRequest    Request);

        Task RaiseOnSetMonitoringBaseResponse(DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              IWebSocketConnection        Connection,
                                              SetMonitoringBaseRequest    Request,
                                              SetMonitoringBaseResponse   Response,
                                              TimeSpan                    Runtime);

        #endregion

        #region GetMonitoringReport

        Task RaiseOnGetMonitoringReportRequest (DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                IWebSocketConnection          Connection,
                                                GetMonitoringReportRequest    Request);

        Task RaiseOnGetMonitoringReportResponse(DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                IWebSocketConnection          Connection,
                                                GetMonitoringReportRequest    Request,
                                                GetMonitoringReportResponse   Response,
                                                TimeSpan                      Runtime);

        #endregion

        #region SetMonitoringLevel

        Task RaiseOnSetMonitoringLevelRequest (DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               IWebSocketConnection         Connection,
                                               SetMonitoringLevelRequest    Request);

        Task RaiseOnSetMonitoringLevelResponse(DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               IWebSocketConnection         Connection,
                                               SetMonitoringLevelRequest    Request,
                                               SetMonitoringLevelResponse   Response,
                                               TimeSpan                     Runtime);

        #endregion

        #region SetVariableMonitoring

        Task RaiseOnSetVariableMonitoringRequest (DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  IWebSocketConnection            Connection,
                                                  SetVariableMonitoringRequest    Request);

        Task RaiseOnSetVariableMonitoringResponse(DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  IWebSocketConnection            Connection,
                                                  SetVariableMonitoringRequest    Request,
                                                  SetVariableMonitoringResponse   Response,
                                                  TimeSpan                        Runtime);

        #endregion

        #region ClearVariableMonitoring

        Task RaiseOnClearVariableMonitoringRequest (DateTime                          Timestamp,
                                                    IEventSender                      Sender,
                                                    IWebSocketConnection              Connection,
                                                    ClearVariableMonitoringRequest    Request);

        Task RaiseOnClearVariableMonitoringResponse(DateTime                          Timestamp,
                                                    IEventSender                      Sender,
                                                    IWebSocketConnection              Connection,
                                                    ClearVariableMonitoringRequest    Request,
                                                    ClearVariableMonitoringResponse   Response,
                                                    TimeSpan                          Runtime);

        #endregion

        #region SetNetworkProfile

        Task RaiseOnSetNetworkProfileRequest (DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              IWebSocketConnection        Connection,
                                              SetNetworkProfileRequest    Request);

        Task RaiseOnSetNetworkProfileResponse(DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              IWebSocketConnection        Connection,
                                              SetNetworkProfileRequest    Request,
                                              SetNetworkProfileResponse   Response,
                                              TimeSpan                    Runtime);

        #endregion

        #region ChangeAvailability

        Task RaiseOnChangeAvailabilityRequest (DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               IWebSocketConnection         Connection,
                                               ChangeAvailabilityRequest    Request);

        Task RaiseOnChangeAvailabilityResponse(DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               IWebSocketConnection         Connection,
                                               ChangeAvailabilityRequest    Request,
                                               ChangeAvailabilityResponse   Response,
                                               TimeSpan                     Runtime);

        #endregion

        #region TriggerMessage

        Task RaiseOnTriggerMessageRequest (DateTime                 Timestamp,
                                           IEventSender             Sender,
                                           IWebSocketConnection     Connection,
                                           TriggerMessageRequest    Request);

        Task RaiseOnTriggerMessageResponse(DateTime                 Timestamp,
                                           IEventSender             Sender,
                                           IWebSocketConnection     Connection,
                                           TriggerMessageRequest    Request,
                                           TriggerMessageResponse   Response,
                                           TimeSpan                 Runtime);

        #endregion

        #region IncomingDataTransfer

        Task RaiseOnIncomingDataTransferRequest (DateTime               Timestamp,
                                                 IEventSender           Sender,
                                                 IWebSocketConnection   Connection,
                                                 DataTransferRequest    Request);

        Task RaiseOnIncomingDataTransferResponse(DateTime               Timestamp,
                                                 IEventSender           Sender,
                                                 IWebSocketConnection   Connection,
                                                 DataTransferRequest    Request,
                                                 DataTransferResponse   Response,
                                                 TimeSpan               Runtime);

        #endregion


        #region CertificateSigned

        Task RaiseOnCertificateSignedRequest (DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              IWebSocketConnection        Connection,
                                              CertificateSignedRequest    Request);

        Task RaiseOnCertificateSignedResponse(DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              IWebSocketConnection        Connection,
                                              CertificateSignedRequest    Request,
                                              CertificateSignedResponse   Response,
                                              TimeSpan                    Runtime);

        #endregion

        #region InstallCertificate

        Task RaiseOnInstallCertificateRequest (DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               IWebSocketConnection         Connection,
                                               InstallCertificateRequest    Request);

        Task RaiseOnInstallCertificateResponse(DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               IWebSocketConnection         Connection,
                                               InstallCertificateRequest    Request,
                                               InstallCertificateResponse   Response,
                                               TimeSpan                     Runtime);

        #endregion

        #region GetInstalledCertificateIds

        Task RaiseOnGetInstalledCertificateIdsRequest (DateTime                             Timestamp,
                                                       IEventSender                         Sender,
                                                       IWebSocketConnection                 Connection,
                                                       GetInstalledCertificateIdsRequest    Request);

        Task RaiseOnGetInstalledCertificateIdsResponse(DateTime                             Timestamp,
                                                       IEventSender                         Sender,
                                                       IWebSocketConnection                 Connection,
                                                       GetInstalledCertificateIdsRequest    Request,
                                                       GetInstalledCertificateIdsResponse   Response,
                                                       TimeSpan                             Runtime);

        #endregion

        #region DeleteCertificate

        Task RaiseOnDeleteCertificateRequest (DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              IWebSocketConnection        Connection,
                                              DeleteCertificateRequest    Request);

        Task RaiseOnDeleteCertificateResponse(DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              IWebSocketConnection        Connection,
                                              DeleteCertificateRequest    Request,
                                              DeleteCertificateResponse   Response,
                                              TimeSpan                    Runtime);

        #endregion

        #region NotifyCRL

        Task RaiseOnNotifyCRLRequest (DateTime               Timestamp,
                                      IEventSender           Sender,
                                      IWebSocketConnection   Connection,
                                      NotifyCRLRequest       Request);

        Task RaiseOnNotifyCRLResponse(DateTime               Timestamp,
                                      IEventSender           Sender,
                                      IWebSocketConnection   Connection,
                                      NotifyCRLRequest       Request,
                                      NotifyCRLResponse      Response,
                                      TimeSpan               Runtime);

        #endregion


        #region GetLocalListVersion

        Task RaiseOnGetLocalListVersionRequest (DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                IWebSocketConnection          Connection,
                                                GetLocalListVersionRequest    Request);

        Task RaiseOnGetLocalListVersionResponse(DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                IWebSocketConnection          Connection,
                                                GetLocalListVersionRequest    Request,
                                                GetLocalListVersionResponse   Response,
                                                TimeSpan                      Runtime);

        #endregion

        #region SendLocalList

        Task RaiseOnSendLocalListRequest (DateTime                Timestamp,
                                          IEventSender            Sender,
                                          IWebSocketConnection    Connection,
                                          SendLocalListRequest    Request);

        Task RaiseOnSendLocalListResponse(DateTime                Timestamp,
                                          IEventSender            Sender,
                                          IWebSocketConnection    Connection,
                                          SendLocalListRequest    Request,
                                          SendLocalListResponse   Response,
                                          TimeSpan                Runtime);

        #endregion

        #region ClearCache

        Task RaiseOnClearCacheRequest (DateTime               Timestamp,
                                       IEventSender           Sender,
                                       IWebSocketConnection   Connection,
                                       ClearCacheRequest      Request);

        Task RaiseOnClearCacheResponse(DateTime               Timestamp,
                                       IEventSender           Sender,
                                       IWebSocketConnection   Connection,
                                       ClearCacheRequest      Request,
                                       ClearCacheResponse     Response,
                                       TimeSpan               Runtime);

        #endregion


        #region ReserveNow

        Task RaiseOnReserveNowRequest (DateTime               Timestamp,
                                       IEventSender           Sender,
                                       IWebSocketConnection   Connection,
                                       ReserveNowRequest      Request);

        Task RaiseOnReserveNowResponse(DateTime               Timestamp,
                                       IEventSender           Sender,
                                       IWebSocketConnection   Connection,
                                       ReserveNowRequest      Request,
                                       ReserveNowResponse     Response,
                                       TimeSpan               Runtime);

        #endregion

        #region CancelReservation

        Task RaiseOnCancelReservationRequest (DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              IWebSocketConnection        Connection,
                                              CancelReservationRequest    Request);

        Task RaiseOnCancelReservationResponse(DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              IWebSocketConnection        Connection,
                                              CancelReservationRequest    Request,
                                              CancelReservationResponse   Response,
                                              TimeSpan                    Runtime);

        #endregion

        #region RequestStartTransaction

        Task RaiseOnRequestStartTransactionRequest (DateTime                          Timestamp,
                                                    IEventSender                      Sender,
                                                    IWebSocketConnection              Connection,
                                                    RequestStartTransactionRequest    Request);

        Task RaiseOnRequestStartTransactionResponse(DateTime                          Timestamp,
                                                    IEventSender                      Sender,
                                                    IWebSocketConnection              Connection,
                                                    RequestStartTransactionRequest    Request,
                                                    RequestStartTransactionResponse   Response,
                                                    TimeSpan                          Runtime);

        #endregion

        #region RequestStopTransaction

        Task RaiseOnRequestStopTransactionRequest (DateTime                         Timestamp,
                                                   IEventSender                     Sender,
                                                   IWebSocketConnection             Connection,
                                                   RequestStopTransactionRequest    Request);

        Task RaiseOnRequestStopTransactionResponse(DateTime                         Timestamp,
                                                   IEventSender                     Sender,
                                                   IWebSocketConnection             Connection,
                                                   RequestStopTransactionRequest    Request,
                                                   RequestStopTransactionResponse   Response,
                                                   TimeSpan                         Runtime);

        #endregion

        #region GetTransactionStatus

        Task RaiseOnGetTransactionStatusRequest (DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 IWebSocketConnection           Connection,
                                                 GetTransactionStatusRequest    Request);

        Task RaiseOnGetTransactionStatusResponse(DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 IWebSocketConnection           Connection,
                                                 GetTransactionStatusRequest    Request,
                                                 GetTransactionStatusResponse   Response,
                                                 TimeSpan                       Runtime);

        #endregion

        #region SetChargingProfile

        Task RaiseOnSetChargingProfileRequest (DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               IWebSocketConnection         Connection,
                                               SetChargingProfileRequest    Request);

        Task RaiseOnSetChargingProfileResponse(DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               IWebSocketConnection         Connection,
                                               SetChargingProfileRequest    Request,
                                               SetChargingProfileResponse   Response,
                                               TimeSpan                     Runtime);

        #endregion

        #region GetChargingProfiles

        Task RaiseOnGetChargingProfilesRequest (DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                IWebSocketConnection          Connection,
                                                GetChargingProfilesRequest    Request);

        Task RaiseOnGetChargingProfilesResponse(DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                IWebSocketConnection          Connection,
                                                GetChargingProfilesRequest    Request,
                                                GetChargingProfilesResponse   Response,
                                                TimeSpan                      Runtime);

        #endregion

        #region ClearChargingProfile

        Task RaiseOnClearChargingProfileRequest (DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 IWebSocketConnection           Connection,
                                                 ClearChargingProfileRequest    Request);

        Task RaiseOnClearChargingProfileResponse(DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 IWebSocketConnection           Connection,
                                                 ClearChargingProfileRequest    Request,
                                                 ClearChargingProfileResponse   Response,
                                                 TimeSpan                       Runtime);

        #endregion

        #region GetCompositeSchedule

        Task RaiseOnGetCompositeScheduleRequest (DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 IWebSocketConnection           Connection,
                                                 GetCompositeScheduleRequest    Request);

        Task RaiseOnGetCompositeScheduleResponse(DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 IWebSocketConnection           Connection,
                                                 GetCompositeScheduleRequest    Request,
                                                 GetCompositeScheduleResponse   Response,
                                                 TimeSpan                       Runtime);

        #endregion

        #region UpdateDynamicSchedule

        Task RaiseOnUpdateDynamicScheduleRequest (DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  IWebSocketConnection            Connection,
                                                  UpdateDynamicScheduleRequest    Request);

        Task RaiseOnUpdateDynamicScheduleResponse(DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  IWebSocketConnection            Connection,
                                                  UpdateDynamicScheduleRequest    Request,
                                                  UpdateDynamicScheduleResponse   Response,
                                                  TimeSpan                        Runtime);

        #endregion

        #region NotifyAllowedEnergyTransfer

        Task RaiseOnNotifyAllowedEnergyTransferRequest (DateTime                              Timestamp,
                                                        IEventSender                          Sender,
                                                        IWebSocketConnection                  Connection,
                                                        NotifyAllowedEnergyTransferRequest    Request);

        Task RaiseOnNotifyAllowedEnergyTransferResponse(DateTime                              Timestamp,
                                                        IEventSender                          Sender,
                                                        IWebSocketConnection                  Connection,
                                                        NotifyAllowedEnergyTransferRequest    Request,
                                                        NotifyAllowedEnergyTransferResponse   Response,
                                                        TimeSpan                              Runtime);

        #endregion

        #region UsePriorityCharging

        Task RaiseOnUsePriorityChargingRequest (DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                IWebSocketConnection          Connection,
                                                UsePriorityChargingRequest    Request);

        Task RaiseOnUsePriorityChargingResponse(DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                IWebSocketConnection          Connection,
                                                UsePriorityChargingRequest    Request,
                                                UsePriorityChargingResponse   Response,
                                                TimeSpan                      Runtime);

        #endregion

        #region UnlockConnector

        Task RaiseOnUnlockConnectorRequest (DateTime                  Timestamp,
                                            IEventSender              Sender,
                                            IWebSocketConnection      Connection,
                                            UnlockConnectorRequest    Request);

        Task RaiseOnUnlockConnectorResponse(DateTime                  Timestamp,
                                            IEventSender              Sender,
                                            IWebSocketConnection      Connection,
                                            UnlockConnectorRequest    Request,
                                            UnlockConnectorResponse   Response,
                                            TimeSpan                  Runtime);

        #endregion


        #region AFRRSignal

        Task RaiseOnAFRRSignalRequest (DateTime               Timestamp,
                                       IEventSender           Sender,
                                       IWebSocketConnection   Connection,
                                       AFRRSignalRequest      Request);

        Task RaiseOnAFRRSignalResponse(DateTime               Timestamp,
                                       IEventSender           Sender,
                                       IWebSocketConnection   Connection,
                                       AFRRSignalRequest      Request,
                                       AFRRSignalResponse     Response,
                                       TimeSpan               Runtime);

        #endregion


        #region SetDisplayMessage

        Task RaiseOnSetDisplayMessageRequest (DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              IWebSocketConnection        Connection,
                                              SetDisplayMessageRequest    Request);

        Task RaiseOnSetDisplayMessageResponse(DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              IWebSocketConnection        Connection,
                                              SetDisplayMessageRequest    Request,
                                              SetDisplayMessageResponse   Response,
                                              TimeSpan                    Runtime);

        #endregion

        #region GetDisplayMessages

        Task RaiseOnGetDisplayMessagesRequest (DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               IWebSocketConnection         Connection,
                                               GetDisplayMessagesRequest    Request);

        Task RaiseOnGetDisplayMessagesResponse(DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               IWebSocketConnection         Connection,
                                               GetDisplayMessagesRequest    Request,
                                               GetDisplayMessagesResponse   Response,
                                               TimeSpan                     Runtime);

        #endregion

        #region ClearDisplayMessage

        Task RaiseOnClearDisplayMessageRequest (DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                IWebSocketConnection          Connection,
                                                ClearDisplayMessageRequest    Request);

        Task RaiseOnClearDisplayMessageResponse(DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                IWebSocketConnection          Connection,
                                                ClearDisplayMessageRequest    Request,
                                                ClearDisplayMessageResponse   Response,
                                                TimeSpan                      Runtime);

        #endregion

        #region CostUpdated

        Task RaiseOnCostUpdatedRequest (DateTime               Timestamp,
                                        IEventSender           Sender,
                                        IWebSocketConnection   Connection,
                                        CostUpdatedRequest     Request);

        Task RaiseOnCostUpdatedResponse(DateTime               Timestamp,
                                        IEventSender           Sender,
                                        IWebSocketConnection   Connection,
                                        CostUpdatedRequest     Request,
                                        CostUpdatedResponse    Response,
                                        TimeSpan               Runtime);

        #endregion

        #region CustomerInformation

        Task RaiseOnCustomerInformationRequest (DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                IWebSocketConnection          Connection,
                                                CustomerInformationRequest    Request);

        Task RaiseOnCustomerInformationResponse(DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                IWebSocketConnection          Connection,
                                                CustomerInformationRequest    Request,
                                                CustomerInformationResponse   Response,
                                                TimeSpan                      Runtime);

        #endregion


        // Binary Data Streams Extensions

        #region IncomingBinaryDataTransfer

        Task RaiseOnIncomingBinaryDataTransferRequest (DateTime                     Timestamp,
                                                       IEventSender                 Sender,
                                                       IWebSocketConnection         Connection,
                                                       BinaryDataTransferRequest    Request);

        Task RaiseOnIncomingBinaryDataTransferResponse(DateTime                     Timestamp,
                                                       IEventSender                 Sender,
                                                       IWebSocketConnection         Connection,
                                                       BinaryDataTransferRequest    Request,
                                                       BinaryDataTransferResponse   Response,
                                                       TimeSpan                     Runtime);

        #endregion

        #region GetFile

        Task RaiseOnGetFileRequest(DateTime                Timestamp,
                                   IEventSender            Sender,
                                   IWebSocketConnection    Connection,
                                   GetFileRequest          Request);

        Task RaiseOnGetFileResponse(DateTime               Timestamp,
                                    IEventSender           Sender,
                                    IWebSocketConnection   Connection,
                                    GetFileRequest         Request,
                                    GetFileResponse        Response,
                                    TimeSpan               Runtime);

        #endregion

        #region SendFile

        Task RaiseOnSendFileRequest (DateTime               Timestamp,
                                     IEventSender           Sender,
                                     IWebSocketConnection   Connection,
                                     SendFileRequest        Request);

        Task RaiseOnSendFileResponse(DateTime               Timestamp,
                                     IEventSender           Sender,
                                     IWebSocketConnection   Connection,
                                     SendFileRequest        Request,
                                     SendFileResponse       Response,
                                     TimeSpan               Runtime);

        #endregion

        #region DeleteFile

        Task RaiseOnDeleteFileRequest (DateTime               Timestamp,
                                       IEventSender           Sender,
                                       IWebSocketConnection   Connection,
                                       DeleteFileRequest      Request);

        Task RaiseOnDeleteFileResponse(DateTime               Timestamp,
                                       IEventSender           Sender,
                                       IWebSocketConnection   Connection,
                                       DeleteFileRequest      Request,
                                       DeleteFileResponse     Response,
                                       TimeSpan               Runtime);

        #endregion


        // E2E Security Extensions

        #region AddSignaturePolicy

        Task RaiseOnAddSignaturePolicyRequest (DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               IWebSocketConnection         Connection,
                                               AddSignaturePolicyRequest    Request);

        Task RaiseOnAddSignaturePolicyResponse(DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               IWebSocketConnection         Connection,
                                               AddSignaturePolicyRequest    Request,
                                               AddSignaturePolicyResponse   Response,
                                               TimeSpan                     Runtime);

        #endregion

        #region UpdateSignaturePolicy

        Task RaiseOnUpdateSignaturePolicyRequest (DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  IWebSocketConnection            Connection,
                                                  UpdateSignaturePolicyRequest    Request);

        Task RaiseOnUpdateSignaturePolicyResponse(DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  IWebSocketConnection            Connection,
                                                  UpdateSignaturePolicyRequest    Request,
                                                  UpdateSignaturePolicyResponse   Response,
                                                  TimeSpan                        Runtime);

        #endregion

        #region DeleteSignaturePolicy

        Task RaiseOnDeleteSignaturePolicyRequest (DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  IWebSocketConnection            Connection,
                                                  DeleteSignaturePolicyRequest    Request);

        Task RaiseOnDeleteSignaturePolicyResponse(DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  IWebSocketConnection            Connection,
                                                  DeleteSignaturePolicyRequest    Request,
                                                  DeleteSignaturePolicyResponse   Response,
                                                  TimeSpan                        Runtime);

        #endregion

        #region AddUserRole

        Task RaiseOnAddUserRoleRequest (DateTime               Timestamp,
                                        IEventSender           Sender,
                                        IWebSocketConnection   Connection,
                                        AddUserRoleRequest     Request);

        Task RaiseOnAddUserRoleResponse(DateTime               Timestamp,
                                        IEventSender           Sender,
                                        IWebSocketConnection   Connection,
                                        AddUserRoleRequest     Request,
                                        AddUserRoleResponse    Response,
                                        TimeSpan               Runtime);

        #endregion

        #region UpdateUserRole

        Task RaiseOnUpdateUserRoleRequest (DateTime                 Timestamp,
                                           IEventSender             Sender,
                                           IWebSocketConnection     Connection,
                                           UpdateUserRoleRequest    Request);

        Task RaiseOnUpdateUserRoleResponse(DateTime                 Timestamp,
                                           IEventSender             Sender,
                                           IWebSocketConnection     Connection,
                                           UpdateUserRoleRequest    Request,
                                           UpdateUserRoleResponse   Response,
                                           TimeSpan                 Runtime);

        #endregion

        #region DeleteUserRole

        Task RaiseOnDeleteUserRoleRequest (DateTime                 Timestamp,
                                           IEventSender             Sender,
                                           IWebSocketConnection     Connection,
                                           DeleteUserRoleRequest    Request);

        Task RaiseOnDeleteUserRoleResponse(DateTime                 Timestamp,
                                           IEventSender             Sender,
                                           IWebSocketConnection     Connection,
                                           DeleteUserRoleRequest    Request,
                                           DeleteUserRoleResponse   Response,
                                           TimeSpan                 Runtime);

        #endregion


        // E2E Charging Tariffs Extensions

        #region SetDefaultChargingTariff

        Task RaiseOnSetDefaultChargingTariffRequest (DateTime                           Timestamp,
                                                     IEventSender                       Sender,
                                                     IWebSocketConnection               Connection,
                                                     SetDefaultChargingTariffRequest    Request);

        Task RaiseOnSetDefaultChargingTariffResponse(DateTime                           Timestamp,
                                                     IEventSender                       Sender,
                                                     IWebSocketConnection               Connection,
                                                     SetDefaultChargingTariffRequest    Request,
                                                     SetDefaultChargingTariffResponse   Response,
                                                     TimeSpan                           Runtime);

        #endregion

        #region GetDefaultChargingTariff

        Task RaiseOnGetDefaultChargingTariffRequest (DateTime                           Timestamp,
                                                     IEventSender                       Sender,
                                                     IWebSocketConnection               Connection,
                                                     GetDefaultChargingTariffRequest    Request);

        Task RaiseOnGetDefaultChargingTariffResponse(DateTime                           Timestamp,
                                                     IEventSender                       Sender,
                                                     IWebSocketConnection               Connection,
                                                     GetDefaultChargingTariffRequest    Request,
                                                     GetDefaultChargingTariffResponse   Response,
                                                     TimeSpan                           Runtime);

        #endregion

        #region RemoveDefaultChargingTariff

        Task RaiseOnRemoveDefaultChargingTariffRequest (DateTime                              Timestamp,
                                                        IEventSender                          Sender,
                                                        IWebSocketConnection                  Connection,
                                                        RemoveDefaultChargingTariffRequest    Request);

        Task RaiseOnRemoveDefaultChargingTariffResponse(DateTime                              Timestamp,
                                                        IEventSender                          Sender,
                                                        IWebSocketConnection                  Connection,
                                                        RemoveDefaultChargingTariffRequest    Request,
                                                        RemoveDefaultChargingTariffResponse   Response,
                                                        TimeSpan                              Runtime);

        #endregion

        #endregion

    }


    public interface INetworkingNodeOUT : NetworkingNode.     INetworkingNodeOutgoingMessages,
                                          NetworkingNode.     INetworkingNodeOutgoingMessageEvents,
                                          NetworkingNode.CS.  INetworkingNodeOutgoingMessages,
                                          NetworkingNode.CS.  INetworkingNodeOutgoingMessageEvents,
                                          NetworkingNode.CSMS.INetworkingNodeOutgoingMessages,
                                          NetworkingNode.CSMS.INetworkingNodeOutgoingMessageEvents

    {

        #region Outgoing Message Events

        #region DataTransfer

        Task RaiseOnDataTransferRequest (DateTime               Timestamp,
                                         IEventSender           Sender,
                                         DataTransferRequest    Request);

        Task RaiseOnDataTransferResponse(DateTime               Timestamp,
                                         IEventSender           Sender,
                                         DataTransferRequest    Request,
                                         DataTransferResponse   Response,
                                         TimeSpan               Runtime);

        #endregion

        #region BinaryDataTransfer

        Task RaiseOnBinaryDataTransferRequest (DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               BinaryDataTransferRequest    Request);

        Task RaiseOnBinaryDataTransferResponse(DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               BinaryDataTransferRequest    Request,
                                               BinaryDataTransferResponse   Response,
                                               TimeSpan                     Runtime);

        #endregion

        #region NotifyNetworkTopology

        Task RaiseOnNotifyNetworkTopologyRequest (DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  NotifyNetworkTopologyRequest    Request);

        Task RaiseOnNotifyNetworkTopologyResponse(DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  NotifyNetworkTopologyRequest    Request,
                                                  NotifyNetworkTopologyResponse   Response,
                                                  TimeSpan                        Runtime);

        #endregion

        #endregion

        #region Outgoing Message Events: Networking Node -> CSMS

        #region BootNotification

        Task RaiseOnBootNotificationRequest (DateTime                   Timestamp,
                                             IEventSender               Sender,
                                             BootNotificationRequest    Request);

        Task RaiseOnBootNotificationResponse(DateTime                   Timestamp,
                                             IEventSender               Sender,
                                             BootNotificationRequest    Request,
                                             BootNotificationResponse   Response,
                                             TimeSpan                   Runtime);

        #endregion

        #region FirmwareStatusNotification

        Task RaiseOnFirmwareStatusNotificationRequest (DateTime                             Timestamp,
                                                       IEventSender                         Sender,
                                                       FirmwareStatusNotificationRequest    Request);

        Task RaiseOnFirmwareStatusNotificationResponse(DateTime                             Timestamp,
                                                       IEventSender                         Sender,
                                                       FirmwareStatusNotificationRequest    Request,
                                                       FirmwareStatusNotificationResponse   Response,
                                                       TimeSpan                             Runtime);

        #endregion

        #region PublishFirmwareStatusNotification

        Task RaiseOnPublishFirmwareStatusNotificationRequest (DateTime                                    Timestamp,
                                                              IEventSender                                Sender,
                                                              PublishFirmwareStatusNotificationRequest    Request);

        Task RaiseOnPublishFirmwareStatusNotificationResponse(DateTime                                    Timestamp,
                                                              IEventSender                                Sender,
                                                              PublishFirmwareStatusNotificationRequest    Request,
                                                              PublishFirmwareStatusNotificationResponse   Response,
                                                              TimeSpan                                    Runtime);

        #endregion

        #region Heartbeat

        Task RaiseOnHeartbeatRequest (DateTime            Timestamp,
                                      IEventSender        Sender,
                                      HeartbeatRequest    Request);

        Task RaiseOnHeartbeatResponse(DateTime            Timestamp,
                                      IEventSender        Sender,
                                      HeartbeatRequest    Request,
                                      HeartbeatResponse   Response,
                                      TimeSpan            Runtime);

        #endregion

        #region NotifyEvent

        Task RaiseOnNotifyEventRequest (DateTime              Timestamp,
                                        IEventSender          Sender,
                                        NotifyEventRequest    Request);

        Task RaiseOnNotifyEventResponse(DateTime              Timestamp,
                                        IEventSender          Sender,
                                        NotifyEventRequest    Request,
                                        NotifyEventResponse   Response,
                                        TimeSpan              Runtime);

        #endregion

        #region SecurityEventNotification

        Task RaiseOnSecurityEventNotificationRequest (DateTime                            Timestamp,
                                                      IEventSender                        Sender,
                                                      SecurityEventNotificationRequest    Request);

        Task RaiseOnSecurityEventNotificationResponse(DateTime                            Timestamp,
                                                      IEventSender                        Sender,
                                                      SecurityEventNotificationRequest    Request,
                                                      SecurityEventNotificationResponse   Response,
                                                      TimeSpan                            Runtime);

        #endregion

        #region NotifyReport

        Task RaiseOnNotifyReportRequest (DateTime               Timestamp,
                                         IEventSender           Sender,
                                         NotifyReportRequest    Request);

        Task RaiseOnNotifyReportResponse(DateTime               Timestamp,
                                         IEventSender           Sender,
                                         NotifyReportRequest    Request,
                                         NotifyReportResponse   Response,
                                         TimeSpan               Runtime);

        #endregion

        #region NotifyMonitoringReport

        Task RaiseOnNotifyMonitoringReportRequest (DateTime                         Timestamp,
                                                   IEventSender                     Sender,
                                                   NotifyMonitoringReportRequest    Request);

        Task RaiseOnNotifyMonitoringReportResponse(DateTime                         Timestamp,
                                                   IEventSender                     Sender,
                                                   NotifyMonitoringReportRequest    Request,
                                                   NotifyMonitoringReportResponse   Response,
                                                   TimeSpan                         Runtime);

        #endregion

        #region LogStatusNotification

        Task RaiseOnLogStatusNotificationRequest (DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  LogStatusNotificationRequest    Request);

        Task RaiseOnLogStatusNotificationResponse(DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  LogStatusNotificationRequest    Request,
                                                  LogStatusNotificationResponse   Response,
                                                  TimeSpan                        Runtime);

        #endregion


        #region SignCertificate

        Task RaiseOnSignCertificateRequest (DateTime                  Timestamp,
                                            IEventSender              Sender,
                                            SignCertificateRequest    Request);

        Task RaiseOnSignCertificateResponse(DateTime                  Timestamp,
                                            IEventSender              Sender,
                                            SignCertificateRequest    Request,
                                            SignCertificateResponse   Response,
                                            TimeSpan                  Runtime);

        #endregion

        #region Get15118EVCertificate

        Task RaiseOnGet15118EVCertificateRequest (DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  Get15118EVCertificateRequest    Request);

        Task RaiseOnGet15118EVCertificateResponse(DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  Get15118EVCertificateRequest    Request,
                                                  Get15118EVCertificateResponse   Response,
                                                  TimeSpan                        Runtime);

        #endregion

        #region GetCertificateStatus

        Task RaiseOnGetCertificateStatusRequest (DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 GetCertificateStatusRequest    Request);

        Task RaiseOnGetCertificateStatusResponse(DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 GetCertificateStatusRequest    Request,
                                                 GetCertificateStatusResponse   Response,
                                                 TimeSpan                       Runtime);

        #endregion

        #region GetCRL

        Task RaiseOnGetCRLRequest (DateTime         Timestamp,
                                   IEventSender     Sender,
                                   GetCRLRequest    Request);

        Task RaiseOnGetCRLResponse(DateTime         Timestamp,
                                   IEventSender     Sender,
                                   GetCRLRequest    Request,
                                   GetCRLResponse   Response,
                                   TimeSpan         Runtime);

        #endregion


        #region ReservationStatusUpdate

        Task RaiseOnReservationStatusUpdateRequest (DateTime                          Timestamp,
                                                    IEventSender                      Sender,
                                                    ReservationStatusUpdateRequest    Request);

        Task RaiseOnReservationStatusUpdateResponse(DateTime                          Timestamp,
                                                    IEventSender                      Sender,
                                                    ReservationStatusUpdateRequest    Request,
                                                    ReservationStatusUpdateResponse   Response,
                                                    TimeSpan                          Runtime);

        #endregion

        #region Authorize

        Task RaiseOnAuthorizeRequest (DateTime            Timestamp,
                                      IEventSender        Sender,
                                      AuthorizeRequest    Request);

        Task RaiseOnAuthorizeResponse(DateTime            Timestamp,
                                      IEventSender        Sender,
                                      AuthorizeRequest    Request,
                                      AuthorizeResponse   Response,
                                      TimeSpan            Runtime);

        #endregion

        #region NotifyEVChargingNeeds

        Task RaiseOnNotifyEVChargingNeedsRequest (DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  NotifyEVChargingNeedsRequest    Request);

        Task RaiseOnNotifyEVChargingNeedsResponse(DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  NotifyEVChargingNeedsRequest    Request,
                                                  NotifyEVChargingNeedsResponse   Response,
                                                  TimeSpan                        Runtime);

        #endregion

        #region TransactionEvent

        Task RaiseOnTransactionEventRequest (DateTime                   Timestamp,
                                             IEventSender               Sender,
                                             TransactionEventRequest    Request);

        Task RaiseOnTransactionEventResponse(DateTime                   Timestamp,
                                             IEventSender               Sender,
                                             TransactionEventRequest    Request,
                                             TransactionEventResponse   Response,
                                             TimeSpan                   Runtime);

        #endregion

        #region StatusNotification

        Task RaiseOnStatusNotificationRequest (DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               StatusNotificationRequest    Request);

        Task RaiseOnStatusNotificationResponse(DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               StatusNotificationRequest    Request,
                                               StatusNotificationResponse   Response,
                                               TimeSpan                     Runtime);

        #endregion

        #region MeterValues

        Task RaiseOnMeterValuesRequest (DateTime              Timestamp,
                                        IEventSender          Sender,
                                        MeterValuesRequest    Request);

        Task RaiseOnMeterValuesResponse(DateTime              Timestamp,
                                        IEventSender          Sender,
                                        MeterValuesRequest    Request,
                                        MeterValuesResponse   Response,
                                        TimeSpan              Runtime);

        #endregion

        #region NotifyChargingLimit

        Task RaiseOnNotifyChargingLimitRequest (DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                NotifyChargingLimitRequest    Request);

        Task RaiseOnNotifyChargingLimitResponse(DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                NotifyChargingLimitRequest    Request,
                                                NotifyChargingLimitResponse   Response,
                                                TimeSpan                      Runtime);

        #endregion

        #region ClearedChargingLimit

        Task RaiseOnClearedChargingLimitRequest (DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 ClearedChargingLimitRequest    Request);

        Task RaiseOnClearedChargingLimitResponse(DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 ClearedChargingLimitRequest    Request,
                                                 ClearedChargingLimitResponse   Response,
                                                 TimeSpan                       Runtime);

        #endregion

        #region ReportChargingProfiles

        Task RaiseOnReportChargingProfilesRequest (DateTime                         Timestamp,
                                                   IEventSender                     Sender,
                                                   ReportChargingProfilesRequest    Request);

        Task RaiseOnReportChargingProfilesResponse(DateTime                         Timestamp,
                                                   IEventSender                     Sender,
                                                   ReportChargingProfilesRequest    Request,
                                                   ReportChargingProfilesResponse   Response,
                                                   TimeSpan                         Runtime);

        #endregion

        #region NotifyEVChargingSchedule

        Task RaiseOnNotifyEVChargingScheduleRequest (DateTime                           Timestamp,
                                                     IEventSender                       Sender,
                                                     NotifyEVChargingScheduleRequest    Request);

        Task RaiseOnNotifyEVChargingScheduleResponse(DateTime                           Timestamp,
                                                     IEventSender                       Sender,
                                                     NotifyEVChargingScheduleRequest    Request,
                                                     NotifyEVChargingScheduleResponse   Response,
                                                     TimeSpan                           Runtime);

        #endregion

        #region NotifyPriorityCharging

        Task RaiseOnNotifyPriorityChargingRequest (DateTime                         Timestamp,
                                                   IEventSender                     Sender,
                                                   NotifyPriorityChargingRequest    Request);

        Task RaiseOnNotifyPriorityChargingResponse(DateTime                         Timestamp,
                                                   IEventSender                     Sender,
                                                   NotifyPriorityChargingRequest    Request,
                                                   NotifyPriorityChargingResponse   Response,
                                                   TimeSpan                         Runtime);

        #endregion

        #region PullDynamicScheduleUpdate

        Task RaiseOnPullDynamicScheduleUpdateRequest (DateTime                            Timestamp,
                                                      IEventSender                        Sender,
                                                      PullDynamicScheduleUpdateRequest    Request);

        Task RaiseOnPullDynamicScheduleUpdateResponse(DateTime                            Timestamp,
                                                      IEventSender                        Sender,
                                                      PullDynamicScheduleUpdateRequest    Request,
                                                      PullDynamicScheduleUpdateResponse   Response,
                                                      TimeSpan                            Runtime);

        #endregion


        #region NotifyDisplayMessages

        Task RaiseOnNotifyDisplayMessagesRequest (DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  NotifyDisplayMessagesRequest    Request);

        Task RaiseOnNotifyDisplayMessagesResponse(DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  NotifyDisplayMessagesRequest    Request,
                                                  NotifyDisplayMessagesResponse   Response,
                                                  TimeSpan                        Runtime);

        #endregion

        #region NotifyCustomerInformation

        Task RaiseOnNotifyCustomerInformationRequest (DateTime                            Timestamp,
                                                      IEventSender                        Sender,
                                                      NotifyCustomerInformationRequest    Request);

        Task RaiseOnNotifyCustomerInformationResponse(DateTime                            Timestamp,
                                                      IEventSender                        Sender,
                                                      NotifyCustomerInformationRequest    Request,
                                                      NotifyCustomerInformationResponse   Response,
                                                      TimeSpan                            Runtime);

        #endregion

        #endregion

        #region Outgoing Message Events: Networking Node -> Charging Station

        #region Reset

        Task RaiseOnResetRequest (DateTime        Timestamp,
                                  IEventSender    Sender,
                                  ResetRequest    Request);

        Task RaiseOnResetResponse(DateTime        Timestamp,
                                  IEventSender    Sender,
                                  ResetRequest    Request,
                                  ResetResponse   Response,
                                  TimeSpan        Runtime);

        #endregion

        #region UpdateFirmware

        Task RaiseOnUpdateFirmwareRequest (DateTime                 Timestamp,
                                           IEventSender             Sender,
                                           UpdateFirmwareRequest    Request);

        Task RaiseOnUpdateFirmwareResponse(DateTime                 Timestamp,
                                           IEventSender             Sender,
                                           UpdateFirmwareRequest    Request,
                                           UpdateFirmwareResponse   Response,
                                           TimeSpan                 Runtime);

        #endregion

        #region PublishFirmware

        Task RaiseOnPublishFirmwareRequest (DateTime                  Timestamp,
                                            IEventSender              Sender,
                                            PublishFirmwareRequest    Request);

        Task RaiseOnPublishFirmwareResponse(DateTime                  Timestamp,
                                            IEventSender              Sender,
                                            PublishFirmwareRequest    Request,
                                            PublishFirmwareResponse   Response,
                                            TimeSpan                  Runtime);

        #endregion

        #region UnpublishFirmware

        Task RaiseOnUnpublishFirmwareRequest (DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              UnpublishFirmwareRequest    Request);

        Task RaiseOnUnpublishFirmwareResponse(DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              UnpublishFirmwareRequest    Request,
                                              UnpublishFirmwareResponse   Response,
                                              TimeSpan                    Runtime);

        #endregion

        #region GetBaseReport

        Task RaiseOnGetBaseReportRequest (DateTime                Timestamp,
                                          IEventSender            Sender,
                                          GetBaseReportRequest    Request);

        Task RaiseOnGetBaseReportResponse(DateTime                Timestamp,
                                          IEventSender            Sender,
                                          GetBaseReportRequest    Request,
                                          GetBaseReportResponse   Response,
                                          TimeSpan                Runtime);

        #endregion

        #region GetReport

        Task RaiseOnGetReportRequest (DateTime            Timestamp,
                                      IEventSender        Sender,
                                      GetReportRequest    Request);

        Task RaiseOnGetReportResponse(DateTime            Timestamp,
                                      IEventSender        Sender,
                                      GetReportRequest    Request,
                                      GetReportResponse   Response,
                                      TimeSpan            Runtime);

        #endregion

        #region GetLog

        Task RaiseOnGetLogRequest (DateTime         Timestamp,
                                   IEventSender     Sender,
                                   GetLogRequest    Request);

        Task RaiseOnGetLogResponse(DateTime         Timestamp,
                                   IEventSender     Sender,
                                   GetLogRequest    Request,
                                   GetLogResponse   Response,
                                   TimeSpan         Runtime);

        #endregion

        #region SetVariables

        Task RaiseOnSetVariablesRequest (DateTime               Timestamp,
                                         IEventSender           Sender,
                                         SetVariablesRequest    Request);

        Task RaiseOnSetVariablesResponse(DateTime               Timestamp,
                                         IEventSender           Sender,
                                         SetVariablesRequest    Request,
                                         SetVariablesResponse   Response,
                                         TimeSpan               Runtime);

        #endregion

        #region GetVariables

        Task RaiseOnGetVariablesRequest (DateTime               Timestamp,
                                         IEventSender           Sender,
                                         GetVariablesRequest    Request);

        Task RaiseOnGetVariablesResponse(DateTime               Timestamp,
                                         IEventSender           Sender,
                                         GetVariablesRequest    Request,
                                         GetVariablesResponse   Response,
                                         TimeSpan               Runtime);

        #endregion

        #region SetMonitoringBase

        Task RaiseOnSetMonitoringBaseRequest (DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              SetMonitoringBaseRequest    Request);

        Task RaiseOnSetMonitoringBaseResponse(DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              SetMonitoringBaseRequest    Request,
                                              SetMonitoringBaseResponse   Response,
                                              TimeSpan                    Runtime);

        #endregion

        #region GetMonitoringReport

        Task RaiseOnGetMonitoringReportRequest (DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                GetMonitoringReportRequest    Request);

        Task RaiseOnGetMonitoringReportResponse(DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                GetMonitoringReportRequest    Request,
                                                GetMonitoringReportResponse   Response,
                                                TimeSpan                      Runtime);

        #endregion

        #region SetMonitoringLevel

        Task RaiseOnSetMonitoringLevelRequest (DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               SetMonitoringLevelRequest    Request);

        Task RaiseOnSetMonitoringLevelResponse(DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               SetMonitoringLevelRequest    Request,
                                               SetMonitoringLevelResponse   Response,
                                               TimeSpan                     Runtime);

        #endregion

        #region SetVariableMonitoring

        Task RaiseOnSetVariableMonitoringRequest (DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  SetVariableMonitoringRequest    Request);

        Task RaiseOnSetVariableMonitoringResponse(DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  SetVariableMonitoringRequest    Request,
                                                  SetVariableMonitoringResponse   Response,
                                                  TimeSpan                        Runtime);

        #endregion

        #region ClearVariableMonitoring

        Task RaiseOnClearVariableMonitoringRequest (DateTime                          Timestamp,
                                                    IEventSender                      Sender,
                                                    ClearVariableMonitoringRequest    Request);

        Task RaiseOnClearVariableMonitoringResponse(DateTime                          Timestamp,
                                                    IEventSender                      Sender,
                                                    ClearVariableMonitoringRequest    Request,
                                                    ClearVariableMonitoringResponse   Response,
                                                    TimeSpan                          Runtime);

        #endregion

        #region SetNetworkProfile

        Task RaiseOnSetNetworkProfileRequest (DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              SetNetworkProfileRequest    Request);

        Task RaiseOnSetNetworkProfileResponse(DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              SetNetworkProfileRequest    Request,
                                              SetNetworkProfileResponse   Response,
                                              TimeSpan                    Runtime);

        #endregion

        #region ChangeAvailability

        Task RaiseOnChangeAvailabilityRequest (DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               ChangeAvailabilityRequest    Request);

        Task RaiseOnChangeAvailabilityResponse(DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               ChangeAvailabilityRequest    Request,
                                               ChangeAvailabilityResponse   Response,
                                               TimeSpan                     Runtime);

        #endregion

        #region TriggerMessage

        Task RaiseOnTriggerMessageRequest (DateTime                 Timestamp,
                                           IEventSender             Sender,
                                           TriggerMessageRequest    Request);

        Task RaiseOnTriggerMessageResponse(DateTime                 Timestamp,
                                           IEventSender             Sender,
                                           TriggerMessageRequest    Request,
                                           TriggerMessageResponse   Response,
                                           TimeSpan                 Runtime);

        #endregion


        #region CertificateSigned

        Task RaiseOnCertificateSignedRequest (DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              CertificateSignedRequest    Request);

        Task RaiseOnCertificateSignedResponse(DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              CertificateSignedRequest    Request,
                                              CertificateSignedResponse   Response,
                                              TimeSpan                    Runtime);

        #endregion

        #region InstallCertificate

        Task RaiseOnInstallCertificateRequest (DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               InstallCertificateRequest    Request);

        Task RaiseOnInstallCertificateResponse(DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               InstallCertificateRequest    Request,
                                               InstallCertificateResponse   Response,
                                               TimeSpan                     Runtime);

        #endregion

        #region GetInstalledCertificateIds

        Task RaiseOnGetInstalledCertificateIdsRequest (DateTime                             Timestamp,
                                                       IEventSender                         Sender,
                                                       GetInstalledCertificateIdsRequest    Request);

        Task RaiseOnGetInstalledCertificateIdsResponse(DateTime                             Timestamp,
                                                       IEventSender                         Sender,
                                                       GetInstalledCertificateIdsRequest    Request,
                                                       GetInstalledCertificateIdsResponse   Response,
                                                       TimeSpan                             Runtime);

        #endregion

        #region DeleteCertificate

        Task RaiseOnDeleteCertificateRequest (DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              DeleteCertificateRequest    Request);

        Task RaiseOnDeleteCertificateResponse(DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              DeleteCertificateRequest    Request,
                                              DeleteCertificateResponse   Response,
                                              TimeSpan                    Runtime);

        #endregion

        #region NotifyCRL

        Task RaiseOnNotifyCRLRequest (DateTime            Timestamp,
                                      IEventSender        Sender,
                                      NotifyCRLRequest    Request);

        Task RaiseOnNotifyCRLResponse(DateTime            Timestamp,
                                      IEventSender        Sender,
                                      NotifyCRLRequest    Request,
                                      NotifyCRLResponse   Response,
                                      TimeSpan            Runtime);

        #endregion


        #region GetLocalListVersion

        Task RaiseOnGetLocalListVersionRequest (DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                GetLocalListVersionRequest    Request);

        Task RaiseOnGetLocalListVersionResponse(DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                GetLocalListVersionRequest    Request,
                                                GetLocalListVersionResponse   Response,
                                                TimeSpan                      Runtime);

        #endregion

        #region SendLocalList

        Task RaiseOnSendLocalListRequest (DateTime                Timestamp,
                                          IEventSender            Sender,
                                          SendLocalListRequest    Request);

        Task RaiseOnSendLocalListResponse(DateTime                Timestamp,
                                          IEventSender            Sender,
                                          SendLocalListRequest    Request,
                                          SendLocalListResponse   Response,
                                          TimeSpan                Runtime);

        #endregion

        #region ClearCache

        Task RaiseOnClearCacheRequest (DateTime             Timestamp,
                                       IEventSender         Sender,
                                       ClearCacheRequest    Request);

        Task RaiseOnClearCacheResponse(DateTime             Timestamp,
                                       IEventSender         Sender,
                                       ClearCacheRequest    Request,
                                       ClearCacheResponse   Response,
                                       TimeSpan             Runtime);

        #endregion


        #region ReserveNow

        Task RaiseOnReserveNowRequest (DateTime             Timestamp,
                                       IEventSender         Sender,
                                       ReserveNowRequest    Request);

        Task RaiseOnReserveNowResponse(DateTime             Timestamp,
                                       IEventSender         Sender,
                                       ReserveNowRequest    Request,
                                       ReserveNowResponse   Response,
                                       TimeSpan             Runtime);

        #endregion

        #region CancelReservation

        Task RaiseOnCancelReservationRequest (DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              CancelReservationRequest    Request);

        Task RaiseOnCancelReservationResponse(DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              CancelReservationRequest    Request,
                                              CancelReservationResponse   Response,
                                              TimeSpan                    Runtime);

        #endregion

        #region RequestStartTransaction

        Task RaiseOnRequestStartTransactionRequest (DateTime                          Timestamp,
                                                    IEventSender                      Sender,
                                                    RequestStartTransactionRequest    Request);

        Task RaiseOnRequestStartTransactionResponse(DateTime                          Timestamp,
                                                    IEventSender                      Sender,
                                                    RequestStartTransactionRequest    Request,
                                                    RequestStartTransactionResponse   Response,
                                                    TimeSpan                          Runtime);

        #endregion

        #region RequestStopTransaction

        Task RaiseOnRequestStopTransactionRequest (DateTime                         Timestamp,
                                                   IEventSender                     Sender,
                                                   RequestStopTransactionRequest    Request);

        Task RaiseOnRequestStopTransactionResponse(DateTime                         Timestamp,
                                                   IEventSender                     Sender,
                                                   RequestStopTransactionRequest    Request,
                                                   RequestStopTransactionResponse   Response,
                                                   TimeSpan                         Runtime);

        #endregion

        #region GetTransactionStatus

        Task RaiseOnGetTransactionStatusRequest (DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 GetTransactionStatusRequest    Request);

        Task RaiseOnGetTransactionStatusResponse(DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 GetTransactionStatusRequest    Request,
                                                 GetTransactionStatusResponse   Response,
                                                 TimeSpan                       Runtime);

        #endregion

        #region SetChargingProfile

        Task RaiseOnSetChargingProfileRequest (DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               SetChargingProfileRequest    Request);

        Task RaiseOnSetChargingProfileResponse(DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               SetChargingProfileRequest    Request,
                                               SetChargingProfileResponse   Response,
                                               TimeSpan                     Runtime);

        #endregion

        #region GetChargingProfiles

        Task RaiseOnGetChargingProfilesRequest (DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                GetChargingProfilesRequest    Request);

        Task RaiseOnGetChargingProfilesResponse(DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                GetChargingProfilesRequest    Request,
                                                GetChargingProfilesResponse   Response,
                                                TimeSpan                      Runtime);

        #endregion

        #region ClearChargingProfile

        Task RaiseOnClearChargingProfileRequest (DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 ClearChargingProfileRequest    Request);

        Task RaiseOnClearChargingProfileResponse(DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 ClearChargingProfileRequest    Request,
                                                 ClearChargingProfileResponse   Response,
                                                 TimeSpan                       Runtime);

        #endregion

        #region GetCompositeSchedule

        Task RaiseOnGetCompositeScheduleRequest (DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 GetCompositeScheduleRequest    Request);

        Task RaiseOnGetCompositeScheduleResponse(DateTime                       Timestamp,
                                                 IEventSender                   Sender,
                                                 GetCompositeScheduleRequest    Request,
                                                 GetCompositeScheduleResponse   Response,
                                                 TimeSpan                       Runtime);

        #endregion

        #region UpdateDynamicSchedule

        Task RaiseOnUpdateDynamicScheduleRequest (DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  UpdateDynamicScheduleRequest    Request);

        Task RaiseOnUpdateDynamicScheduleResponse(DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  UpdateDynamicScheduleRequest    Request,
                                                  UpdateDynamicScheduleResponse   Response,
                                                  TimeSpan                        Runtime);

        #endregion

        #region NotifyAllowedEnergyTransfer

        Task RaiseOnNotifyAllowedEnergyTransferRequest (DateTime                              Timestamp,
                                                        IEventSender                          Sender,
                                                        NotifyAllowedEnergyTransferRequest    Request);

        Task RaiseOnNotifyAllowedEnergyTransferResponse(DateTime                              Timestamp,
                                                        IEventSender                          Sender,
                                                        NotifyAllowedEnergyTransferRequest    Request,
                                                        NotifyAllowedEnergyTransferResponse   Response,
                                                        TimeSpan                              Runtime);

        #endregion

        #region UsePriorityCharging

        Task RaiseOnUsePriorityChargingRequest (DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                UsePriorityChargingRequest    Request);

        Task RaiseOnUsePriorityChargingResponse(DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                UsePriorityChargingRequest    Request,
                                                UsePriorityChargingResponse   Response,
                                                TimeSpan                      Runtime);

        #endregion

        #region UnlockConnector

        Task RaiseOnUnlockConnectorRequest (DateTime                  Timestamp,
                                            IEventSender              Sender,
                                            UnlockConnectorRequest    Request);

        Task RaiseOnUnlockConnectorResponse(DateTime                  Timestamp,
                                            IEventSender              Sender,
                                            UnlockConnectorRequest    Request,
                                            UnlockConnectorResponse   Response,
                                            TimeSpan                  Runtime);

        #endregion


        #region AFRRSignal

        Task RaiseOnAFRRSignalRequest (DateTime             Timestamp,
                                       IEventSender         Sender,
                                       AFRRSignalRequest    Request);

        Task RaiseOnAFRRSignalResponse(DateTime             Timestamp,
                                       IEventSender         Sender,
                                       AFRRSignalRequest    Request,
                                       AFRRSignalResponse   Response,
                                       TimeSpan             Runtime);

        #endregion


        #region SetDisplayMessage

        Task RaiseOnSetDisplayMessageRequest (DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              SetDisplayMessageRequest    Request);

        Task RaiseOnSetDisplayMessageResponse(DateTime                    Timestamp,
                                              IEventSender                Sender,
                                              SetDisplayMessageRequest    Request,
                                              SetDisplayMessageResponse   Response,
                                              TimeSpan                    Runtime);

        #endregion

        #region GetDisplayMessages

        Task RaiseOnGetDisplayMessagesRequest (DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               GetDisplayMessagesRequest    Request);

        Task RaiseOnGetDisplayMessagesResponse(DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               GetDisplayMessagesRequest    Request,
                                               GetDisplayMessagesResponse   Response,
                                               TimeSpan                     Runtime);

        #endregion

        #region ClearDisplayMessage

        Task RaiseOnClearDisplayMessageRequest (DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                ClearDisplayMessageRequest    Request);

        Task RaiseOnClearDisplayMessageResponse(DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                ClearDisplayMessageRequest    Request,
                                                ClearDisplayMessageResponse   Response,
                                                TimeSpan                      Runtime);

        #endregion

        #region CostUpdated

        Task RaiseOnCostUpdatedRequest (DateTime              Timestamp,
                                        IEventSender          Sender,
                                        CostUpdatedRequest    Request);

        Task RaiseOnCostUpdatedResponse(DateTime              Timestamp,
                                        IEventSender          Sender,
                                        CostUpdatedRequest    Request,
                                        CostUpdatedResponse   Response,
                                        TimeSpan              Runtime);

        #endregion

        #region CustomerInformation

        Task RaiseOnCustomerInformationRequest (DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                CustomerInformationRequest    Request);

        Task RaiseOnCustomerInformationResponse(DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                CustomerInformationRequest    Request,
                                                CustomerInformationResponse   Response,
                                                TimeSpan                      Runtime);

        #endregion


        // Binary Data Streams Extensions

        #region GetFile

        Task RaiseOnGetFileRequest (DateTime          Timestamp,
                                    IEventSender      Sender,
                                    GetFileRequest    Request);

        Task RaiseOnGetFileResponse(DateTime          Timestamp,
                                    IEventSender      Sender,
                                    GetFileRequest    Request,
                                    GetFileResponse   Response,
                                    TimeSpan          Runtime);

        #endregion

        #region SendFile

        Task RaiseOnSendFileRequest (DateTime           Timestamp,
                                     IEventSender       Sender,
                                     SendFileRequest    Request);

        Task RaiseOnSendFileResponse(DateTime           Timestamp,
                                     IEventSender       Sender,
                                     SendFileRequest    Request,
                                     SendFileResponse   Response,
                                     TimeSpan           Runtime);

        #endregion

        #region DeleteFile

        Task RaiseOnDeleteFileRequest (DateTime             Timestamp,
                                       IEventSender         Sender,
                                       DeleteFileRequest    Request);

        Task RaiseOnDeleteFileResponse(DateTime             Timestamp,
                                       IEventSender         Sender,
                                       DeleteFileRequest    Request,
                                       DeleteFileResponse   Response,
                                       TimeSpan             Runtime);

        #endregion

        #region ListDirectory

        Task RaiseOnListDirectoryRequest (DateTime                Timestamp,
                                          IEventSender            Sender,
                                          ListDirectoryRequest    Request);

        Task RaiseOnListDirectoryResponse(DateTime                Timestamp,
                                          IEventSender            Sender,
                                          ListDirectoryRequest    Request,
                                          ListDirectoryResponse   Response,
                                          TimeSpan                Runtime);

        #endregion


        // E2E Security Extensions

        #region AddSignaturePolicy

        Task RaiseOnAddSignaturePolicyRequest (DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               AddSignaturePolicyRequest    Request);

        Task RaiseOnAddSignaturePolicyResponse(DateTime                     Timestamp,
                                               IEventSender                 Sender,
                                               AddSignaturePolicyRequest    Request,
                                               AddSignaturePolicyResponse   Response,
                                               TimeSpan                     Runtime);

        #endregion

        #region UpdateSignaturePolicy

        Task RaiseOnUpdateSignaturePolicyRequest (DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  UpdateSignaturePolicyRequest    Request);

        Task RaiseOnUpdateSignaturePolicyResponse(DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  UpdateSignaturePolicyRequest    Request,
                                                  UpdateSignaturePolicyResponse   Response,
                                                  TimeSpan                        Runtime);

        #endregion

        #region DeleteSignaturePolicy

        Task RaiseOnDeleteSignaturePolicyRequest (DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  DeleteSignaturePolicyRequest    Request);

        Task RaiseOnDeleteSignaturePolicyResponse(DateTime                        Timestamp,
                                                  IEventSender                    Sender,
                                                  DeleteSignaturePolicyRequest    Request,
                                                  DeleteSignaturePolicyResponse   Response,
                                                  TimeSpan                        Runtime);

        #endregion

        #region AddUserRole

        Task RaiseOnAddUserRoleRequest (DateTime              Timestamp,
                                        IEventSender          Sender,
                                        AddUserRoleRequest    Request);

        Task RaiseOnAddUserRoleResponse(DateTime              Timestamp,
                                        IEventSender          Sender,
                                        AddUserRoleRequest    Request,
                                        AddUserRoleResponse   Response,
                                        TimeSpan              Runtime);

        #endregion

        #region UpdateUserRole

        Task RaiseOnUpdateUserRoleRequest (DateTime                 Timestamp,
                                           IEventSender             Sender,
                                           UpdateUserRoleRequest    Request);

        Task RaiseOnUpdateUserRoleResponse(DateTime                 Timestamp,
                                           IEventSender             Sender,
                                           UpdateUserRoleRequest    Request,
                                           UpdateUserRoleResponse   Response,
                                           TimeSpan                 Runtime);

        #endregion

        #region DeleteUserRole

        Task RaiseOnDeleteUserRoleRequest (DateTime                 Timestamp,
                                           IEventSender             Sender,
                                           DeleteUserRoleRequest    Request);

        Task RaiseOnDeleteUserRoleResponse(DateTime                 Timestamp,
                                           IEventSender             Sender,
                                           DeleteUserRoleRequest    Request,
                                           DeleteUserRoleResponse   Response,
                                           TimeSpan                 Runtime);

        #endregion


        // E2E Charging Tariff Extensions

        #region SetDefaultChargingTariff

        Task RaiseOnSetDefaultChargingTariffRequest (DateTime                           Timestamp,
                                                     IEventSender                       Sender,
                                                     SetDefaultChargingTariffRequest    Request);

        Task RaiseOnSetDefaultChargingTariffResponse(DateTime                           Timestamp,
                                                     IEventSender                       Sender,
                                                     SetDefaultChargingTariffRequest    Request,
                                                     SetDefaultChargingTariffResponse   Response,
                                                     TimeSpan                           Runtime);

        #endregion

        #region GetDefaultChargingTariff

        Task RaiseOnGetDefaultChargingTariffRequest (DateTime                           Timestamp,
                                                     IEventSender                       Sender,
                                                     GetDefaultChargingTariffRequest    Request);

        Task RaiseOnGetDefaultChargingTariffResponse(DateTime                           Timestamp,
                                                     IEventSender                       Sender,
                                                     GetDefaultChargingTariffRequest    Request,
                                                     GetDefaultChargingTariffResponse   Response,
                                                     TimeSpan                           Runtime);

        #endregion

        #region RemoveDefaultChargingTariff

        Task RaiseOnRemoveDefaultChargingTariffRequest (DateTime                              Timestamp,
                                                        IEventSender                          Sender,
                                                        RemoveDefaultChargingTariffRequest    Request);

        Task RaiseOnRemoveDefaultChargingTariffResponse(DateTime                              Timestamp,
                                                        IEventSender                          Sender,
                                                        RemoveDefaultChargingTariffRequest    Request,
                                                        RemoveDefaultChargingTariffResponse   Response,
                                                        TimeSpan                              Runtime);

        #endregion

        #endregion

    }


    /// <summary>
    /// The common interface of all charging station.
    /// </summary>
    public interface INetworkingNode : IEventSender
    {

        NetworkingNode_Id           Id                       { get; }

        HashSet<NetworkingNode_Id>  AnycastIds               { get; }


        IOCPPAdapter                OCPP                     { get; }

        String                      Model                    { get; }
        String                      VendorName               { get; }
        String?                     SerialNumber             { get; }
        Modem?                      Modem                    { get; }
        String?                     FirmwareVersion          { get; }

        CustomData                  CustomData               { get; }



        String? ClientCloseMessage { get; }



        Task HandleErrors(String     Module,
                          String     Caller,
                          Exception  ExceptionOccured);


    }

}
