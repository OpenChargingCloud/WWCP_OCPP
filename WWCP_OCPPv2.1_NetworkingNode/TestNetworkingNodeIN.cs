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
                                            //  INetworkingNode2,
                                              IEventSender
    {

        public class INN : INetworkingNodeIN
        {

            #region Data

            private readonly TestNetworkingNode parentNetworkingNode;

            #endregion

            #region Events

            #region Incoming Messages: Networking Node <- CSMS

            #region Reset

            /// <summary>
            /// An event fired whenever a Reset request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnResetRequestDelegate?   OnResetRequest;

            public async Task RaiseOnResetRequest(DateTime                    Timestamp,
                                                  IEventSender                Sender,
                                                  IWebSocketConnection        Connection,
                                                  OCPPv2_1.CSMS.ResetRequest  Request)
            {

                var requestLogger = OnResetRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OCPPv2_1.CS.OnResetRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                             Sender,
                                                                                                             Connection,
                                                                                                             Request)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestNetworkingNode),
                                  nameof(OnResetRequest),
                                  e
                              );
                    }

                }

            }


            /// <summary>
            /// An event fired whenever a response to a Reset request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnResetResponseDelegate?  OnResetResponse;

            public async Task RaiseOnResetResponse(DateTime                    Timestamp,
                                                   IEventSender                Sender,
                                                   IWebSocketConnection        Connection,
                                                   OCPPv2_1.CSMS.ResetRequest  Request,
                                                   OCPPv2_1.CS.ResetResponse   Response,
                                                   TimeSpan                    Runtime)
            {

                var requestLogger = OnResetResponse;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OCPPv2_1.CS.OnResetResponseDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                             Sender,
                                                                                                             Connection,
                                                                                                             Request,
                                                                                                             Response,
                                                                                                             Runtime)).
                                                           ToArray();

                    try
                    {
                        await Task.WhenAll(requestLoggerTasks);
                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(TestNetworkingNode),
                                  nameof(OnResetRequest),
                                  e
                              );
                    }

                }

            }

            #endregion

            #region UpdateFirmware

            /// <summary>
            /// An event fired whenever an UpdateFirmware request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

            /// <summary>
            /// An event fired whenever a response to an UpdateFirmware request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

            #endregion

            #region PublishFirmware

            /// <summary>
            /// An event fired whenever a PublishFirmware request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnPublishFirmwareRequestDelegate?   OnPublishFirmwareRequest;

            /// <summary>
            /// An event fired whenever a response to a PublishFirmware request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnPublishFirmwareResponseDelegate?  OnPublishFirmwareResponse;

            #endregion

            #region UnpublishFirmware

            /// <summary>
            /// An event fired whenever an UnpublishFirmware request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnUnpublishFirmwareRequestDelegate?   OnUnpublishFirmwareRequest;

            /// <summary>
            /// An event fired whenever a response to an UnpublishFirmware request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnUnpublishFirmwareResponseDelegate?  OnUnpublishFirmwareResponse;

            #endregion

            #region GetBaseReport

            /// <summary>
            /// An event fired whenever a GetBaseReport request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetBaseReportRequestDelegate?   OnGetBaseReportRequest;

            /// <summary>
            /// An event fired whenever a response to a GetBaseReport request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetBaseReportResponseDelegate?  OnGetBaseReportResponse;

            #endregion

            #region GetReport

            /// <summary>
            /// An event fired whenever a GetReport request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetReportRequestDelegate?   OnGetReportRequest;

            /// <summary>
            /// An event fired whenever a response to a GetReport request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetReportResponseDelegate?  OnGetReportResponse;

            #endregion

            #region GetLog

            /// <summary>
            /// An event fired whenever a GetLog request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetLogRequestDelegate?   OnGetLogRequest;

            /// <summary>
            /// An event fired whenever a response to a GetLog request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetLogResponseDelegate?  OnGetLogResponse;

            #endregion

            #region SetVariables

            /// <summary>
            /// An event fired whenever a SetVariables request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSetVariablesRequestDelegate?   OnSetVariablesRequest;

            /// <summary>
            /// An event fired whenever a response to a SetVariables request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSetVariablesResponseDelegate?  OnSetVariablesResponse;

            #endregion

            #region GetVariables

            /// <summary>
            /// An event fired whenever a GetVariables request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetVariablesRequestDelegate?   OnGetVariablesRequest;

            /// <summary>
            /// An event fired whenever a response to a GetVariables request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetVariablesResponseDelegate?  OnGetVariablesResponse;

            #endregion

            #region SetMonitoringBase

            /// <summary>
            /// An event fired whenever a SetMonitoringBase request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSetMonitoringBaseRequestDelegate?   OnSetMonitoringBaseRequest;

            /// <summary>
            /// An event fired whenever a response to a SetMonitoringBase request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSetMonitoringBaseResponseDelegate?  OnSetMonitoringBaseResponse;

            #endregion

            #region GetMonitoringReport

            /// <summary>
            /// An event fired whenever a GetMonitoringReport request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetMonitoringReportRequestDelegate?   OnGetMonitoringReportRequest;

            /// <summary>
            /// An event fired whenever a response to a GetMonitoringReport request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetMonitoringReportResponseDelegate?  OnGetMonitoringReportResponse;

            #endregion

            #region SetMonitoringLevel

            /// <summary>
            /// An event fired whenever a SetMonitoringLevel request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSetMonitoringLevelRequestDelegate?   OnSetMonitoringLevelRequest;

            /// <summary>
            /// An event fired whenever a response to a SetMonitoringLevel request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSetMonitoringLevelResponseDelegate?  OnSetMonitoringLevelResponse;

            #endregion

            #region SetVariableMonitoring

            /// <summary>
            /// An event fired whenever a SetVariableMonitoring request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSetVariableMonitoringRequestDelegate?   OnSetVariableMonitoringRequest;

            /// <summary>
            /// An event fired whenever a response to a SetVariableMonitoring request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSetVariableMonitoringResponseDelegate?  OnSetVariableMonitoringResponse;

            #endregion

            #region ClearVariableMonitoring

            /// <summary>
            /// An event fired whenever a ClearVariableMonitoring request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnClearVariableMonitoringRequestDelegate?   OnClearVariableMonitoringRequest;

            /// <summary>
            /// An event fired whenever a response to a ClearVariableMonitoring request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnClearVariableMonitoringResponseDelegate?  OnClearVariableMonitoringResponse;

            #endregion

            #region SetNetworkProfile

            /// <summary>
            /// An event fired whenever a SetNetworkProfile request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSetNetworkProfileRequestDelegate?   OnSetNetworkProfileRequest;

            /// <summary>
            /// An event fired whenever a response to a SetNetworkProfile request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSetNetworkProfileResponseDelegate?  OnSetNetworkProfileResponse;

            #endregion

            #region ChangeAvailability

            /// <summary>
            /// An event fired whenever a ChangeAvailability request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

            /// <summary>
            /// An event fired whenever a response to a ChangeAvailability request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

            #endregion

            #region TriggerMessage

            /// <summary>
            /// An event fired whenever a TriggerMessage request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

            /// <summary>
            /// An event fired whenever a response to a TriggerMessage request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

            #endregion

            #region OnIncomingDataTransferRequest/-Response

            /// <summary>
            /// An event sent whenever a data transfer request was sent.
            /// </summary>
            public event OnIncomingDataTransferRequestDelegate?   OnIncomingDataTransferRequest;

            /// <summary>
            /// An event sent whenever a response to a data transfer request was sent.
            /// </summary>
            public event OnIncomingDataTransferResponseDelegate?  OnIncomingDataTransferResponse;

            #endregion


            #region SendSignedCertificate

            /// <summary>
            /// An event fired whenever a SignedCertificate request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

            /// <summary>
            /// An event fired whenever a response to a SignedCertificate request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

            #endregion

            #region InstallCertificate

            /// <summary>
            /// An event fired whenever an InstallCertificate request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

            /// <summary>
            /// An event fired whenever a response to an InstallCertificate request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

            #endregion

            #region GetInstalledCertificateIds

            /// <summary>
            /// An event fired whenever a GetInstalledCertificateIds request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

            /// <summary>
            /// An event fired whenever a response to a GetInstalledCertificateIds request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

            #endregion

            #region DeleteCertificate

            /// <summary>
            /// An event fired whenever a DeleteCertificate request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

            /// <summary>
            /// An event fired whenever a response to a DeleteCertificate request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

            #endregion

            #region NotifyCRL

            /// <summary>
            /// An event fired whenever a NotifyCRL request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyCRLRequestDelegate?   OnNotifyCRLRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyCRL request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyCRLResponseDelegate?  OnNotifyCRLResponse;

            #endregion


            #region GetLocalListVersion

            /// <summary>
            /// An event fired whenever a GetLocalListVersion request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

            /// <summary>
            /// An event fired whenever a response to a GetLocalListVersion request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

            #endregion

            #region SendLocalList

            /// <summary>
            /// An event fired whenever a SendLocalList request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

            /// <summary>
            /// An event fired whenever a response to a SendLocalList request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

            #endregion

            #region ClearCache

            /// <summary>
            /// An event fired whenever a ClearCache request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnClearCacheRequestDelegate?   OnClearCacheRequest;

            /// <summary>
            /// An event fired whenever a response to a ClearCache request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnClearCacheResponseDelegate?  OnClearCacheResponse;

            #endregion


            #region ReserveNow

            /// <summary>
            /// An event fired whenever a ReserveNow request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnReserveNowRequestDelegate?   OnReserveNowRequest;

            /// <summary>
            /// An event fired whenever a response to a ReserveNow request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnReserveNowResponseDelegate?  OnReserveNowResponse;

            #endregion

            #region CancelReservation

            /// <summary>
            /// An event fired whenever a CancelReservation request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

            /// <summary>
            /// An event fired whenever a response to a CancelReservation request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

            #endregion

            #region StartCharging

            /// <summary>
            /// An event fired whenever a RequestStartTransaction request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnRequestStartTransactionRequestDelegate?   OnRequestStartTransactionRequest;

            /// <summary>
            /// An event fired whenever a response to a RequestStartTransaction request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnRequestStartTransactionResponseDelegate?  OnRequestStartTransactionResponse;

            #endregion

            #region StopCharging

            /// <summary>
            /// An event fired whenever a RequestStopTransaction request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnRequestStopTransactionRequestDelegate?   OnRequestStopTransactionRequest;

            /// <summary>
            /// An event fired whenever a response to a RequestStopTransaction request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnRequestStopTransactionResponseDelegate?  OnRequestStopTransactionResponse;

            #endregion

            #region GetTransactionStatus

            /// <summary>
            /// An event fired whenever a GetTransactionStatus request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetTransactionStatusRequestDelegate?   OnGetTransactionStatusRequest;

            /// <summary>
            /// An event fired whenever a response to a GetTransactionStatus request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetTransactionStatusResponseDelegate?  OnGetTransactionStatusResponse;

            #endregion

            #region SetChargingProfile

            /// <summary>
            /// An event fired whenever a SetChargingProfile request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

            /// <summary>
            /// An event fired whenever a response to a SetChargingProfile request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

            #endregion

            #region GetChargingProfiles

            /// <summary>
            /// An event fired whenever a GetChargingProfiles request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetChargingProfilesRequestDelegate?   OnGetChargingProfilesRequest;

            /// <summary>
            /// An event fired whenever a response to a GetChargingProfiles request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetChargingProfilesResponseDelegate?  OnGetChargingProfilesResponse;

            #endregion

            #region ClearChargingProfile

            /// <summary>
            /// An event fired whenever a ClearChargingProfile request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

            /// <summary>
            /// An event fired whenever a response to a ClearChargingProfile request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

            #endregion

            #region GetCompositeSchedule

            /// <summary>
            /// An event fired whenever a GetCompositeSchedule request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

            /// <summary>
            /// An event fired whenever a response to a GetCompositeSchedule request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

            #endregion

            #region UpdateDynamicSchedule

            /// <summary>
            /// An event fired whenever an UpdateDynamicSchedule request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnUpdateDynamicScheduleRequestDelegate?   OnUpdateDynamicScheduleRequest;

            /// <summary>
            /// An event fired whenever a response to an UpdateDynamicSchedule request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnUpdateDynamicScheduleResponseDelegate?  OnUpdateDynamicScheduleResponse;

            #endregion

            #region NotifyAllowedEnergyTransfer

            /// <summary>
            /// An event fired whenever a NotifyAllowedEnergyTransfer request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyAllowedEnergyTransferRequestDelegate?   OnNotifyAllowedEnergyTransferRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyAllowedEnergyTransferResponseDelegate?  OnNotifyAllowedEnergyTransferResponse;

            #endregion

            #region UsePriorityCharging

            /// <summary>
            /// An event fired whenever a UsePriorityCharging request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnUsePriorityChargingRequestDelegate?   OnUsePriorityChargingRequest;

            /// <summary>
            /// An event fired whenever a response to a UsePriorityCharging request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnUsePriorityChargingResponseDelegate?  OnUsePriorityChargingResponse;

            #endregion

            #region UnlockConnector

            /// <summary>
            /// An event fired whenever an UnlockConnector request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

            /// <summary>
            /// An event fired whenever a response to an UnlockConnector request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

            #endregion


            #region AFRRSignal

            /// <summary>
            /// An event fired whenever an AFRR signal request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnAFRRSignalRequestDelegate?   OnAFRRSignalRequest;

            /// <summary>
            /// An event fired whenever a response to an AFRR signal request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnAFRRSignalResponseDelegate?  OnAFRRSignalResponse;

            #endregion


            #region SetDisplayMessage

            /// <summary>
            /// An event fired whenever a SetDisplayMessage request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSetDisplayMessageRequestDelegate?   OnSetDisplayMessageRequest;

            /// <summary>
            /// An event fired whenever a response to a SetDisplayMessage request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSetDisplayMessageResponseDelegate?  OnSetDisplayMessageResponse;

            #endregion

            #region GetDisplayMessages

            /// <summary>
            /// An event fired whenever a GetDisplayMessages request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetDisplayMessagesRequestDelegate?   OnGetDisplayMessagesRequest;

            /// <summary>
            /// An event fired whenever a response to a GetDisplayMessages request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetDisplayMessagesResponseDelegate?  OnGetDisplayMessagesResponse;

            #endregion

            #region ClearDisplayMessage

            /// <summary>
            /// An event fired whenever a ClearDisplayMessage request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnClearDisplayMessageRequestDelegate?   OnClearDisplayMessageRequest;

            /// <summary>
            /// An event fired whenever a response to a ClearDisplayMessage request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnClearDisplayMessageResponseDelegate?  OnClearDisplayMessageResponse;

            #endregion

            #region SendCostUpdated

            /// <summary>
            /// An event fired whenever a CostUpdated request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnCostUpdatedRequestDelegate?   OnCostUpdatedRequest;

            /// <summary>
            /// An event fired whenever a response to a CostUpdated request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnCostUpdatedResponseDelegate?  OnCostUpdatedResponse;

            #endregion

            #region RequestCustomerInformation

            /// <summary>
            /// An event fired whenever a CustomerInformation request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnCustomerInformationRequestDelegate?   OnCustomerInformationRequest;

            /// <summary>
            /// An event fired whenever a response to a CustomerInformation request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnCustomerInformationResponseDelegate?  OnCustomerInformationResponse;

            #endregion


            // Binary Data Streams Extensions

            #region OnIncomingBinaryDataTransferRequest/-Response

            /// <summary>
            /// An event sent whenever a BinaryDataTransfer request was sent.
            /// </summary>
            public event OnIncomingBinaryDataTransferRequestDelegate?   OnIncomingBinaryDataTransferRequest;

            /// <summary>
            /// An event sent whenever a response to a BinaryDataTransfer request was sent.
            /// </summary>
            public event OnIncomingBinaryDataTransferResponseDelegate?  OnIncomingBinaryDataTransferResponse;

            #endregion

            #region OnGetFileRequest/-Response

            /// <summary>
            /// An event sent whenever a GetFile request was sent.
            /// </summary>
            public event OCPP.CS.OnGetFileRequestDelegate?   OnGetFileRequest;

            /// <summary>
            /// An event sent whenever a response to a GetFile request was sent.
            /// </summary>
            public event OCPP.CS.OnGetFileResponseDelegate?  OnGetFileResponse;

            #endregion

            #region OnSendFileRequest/-Response

            /// <summary>
            /// An event sent whenever a SendFile request was sent.
            /// </summary>
            public event OCPP.CS.OnSendFileRequestDelegate?   OnSendFileRequest;

            /// <summary>
            /// An event sent whenever a response to a SendFile request was sent.
            /// </summary>
            public event OCPP.CS.OnSendFileResponseDelegate?  OnSendFileResponse;

            #endregion

            #region OnDeleteFileRequest/-Response

            /// <summary>
            /// An event sent whenever a DeleteFile request was sent.
            /// </summary>
            public event OCPP.CS.OnDeleteFileRequestDelegate?   OnDeleteFileRequest;

            /// <summary>
            /// An event sent whenever a response to a DeleteFile request was sent.
            /// </summary>
            public event OCPP.CS.OnDeleteFileResponseDelegate?  OnDeleteFileResponse;

            #endregion


            // E2E Security Extensions

            #region AddSignaturePolicy

            /// <summary>
            /// An event fired whenever a AddSignaturePolicy request was received from the CSMS.
            /// </summary>
            public event OCPP.CS.OnAddSignaturePolicyRequestDelegate?   OnAddSignaturePolicyRequest;

            /// <summary>
            /// An event fired whenever a response to a AddSignaturePolicy request was sent.
            /// </summary>
            public event OCPP.CS.OnAddSignaturePolicyResponseDelegate?  OnAddSignaturePolicyResponse;

            #endregion

            #region UpdateSignaturePolicy

            /// <summary>
            /// An event fired whenever a UpdateSignaturePolicy request was received from the CSMS.
            /// </summary>
            public event OCPP.CS.OnUpdateSignaturePolicyRequestDelegate?   OnUpdateSignaturePolicyRequest;

            /// <summary>
            /// An event fired whenever a response to a UpdateSignaturePolicy request was sent.
            /// </summary>
            public event OCPP.CS.OnUpdateSignaturePolicyResponseDelegate?  OnUpdateSignaturePolicyResponse;

            #endregion

            #region DeleteSignaturePolicy

            /// <summary>
            /// An event fired whenever a DeleteSignaturePolicy request was received from the CSMS.
            /// </summary>
            public event OCPP.CS.OnDeleteSignaturePolicyRequestDelegate?   OnDeleteSignaturePolicyRequest;

            /// <summary>
            /// An event fired whenever a response to a DeleteSignaturePolicy request was sent.
            /// </summary>
            public event OCPP.CS.OnDeleteSignaturePolicyResponseDelegate?  OnDeleteSignaturePolicyResponse;

            #endregion

            #region AddUserRole

            /// <summary>
            /// An event fired whenever a AddUserRole request was received from the CSMS.
            /// </summary>
            public event OCPP.CS.OnAddUserRoleRequestDelegate?   OnAddUserRoleRequest;

            /// <summary>
            /// An event fired whenever a response to a AddUserRole request was sent.
            /// </summary>
            public event OCPP.CS.OnAddUserRoleResponseDelegate?  OnAddUserRoleResponse;

            #endregion

            #region UpdateUserRole

            /// <summary>
            /// An event fired whenever a UpdateUserRole request was received from the CSMS.
            /// </summary>
            public event OCPP.CS.OnUpdateUserRoleRequestDelegate?   OnUpdateUserRoleRequest;

            /// <summary>
            /// An event fired whenever a response to a UpdateUserRole request was sent.
            /// </summary>
            public event OCPP.CS.OnUpdateUserRoleResponseDelegate?  OnUpdateUserRoleResponse;

            #endregion

            #region DeleteUserRole

            /// <summary>
            /// An event fired whenever a DeleteUserRole request was received from the CSMS.
            /// </summary>
            public event OCPP.CS.OnDeleteUserRoleRequestDelegate?   OnDeleteUserRoleRequest;

            /// <summary>
            /// An event fired whenever a response to a DeleteUserRole request was sent.
            /// </summary>
            public event OCPP.CS.OnDeleteUserRoleResponseDelegate?  OnDeleteUserRoleResponse;

            #endregion


            // E2E Charging Tariffs Extensions

            #region SetDefaultChargingTariff

            /// <summary>
            /// An event fired whenever a SetDefaultChargingTariff request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSetDefaultChargingTariffRequestDelegate?   OnSetDefaultChargingTariffRequest;

            /// <summary>
            /// An event fired whenever a response to a SetDefaultChargingTariff request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnSetDefaultChargingTariffResponseDelegate?  OnSetDefaultChargingTariffResponse;

            #endregion

            #region GetDefaultChargingTariff

            /// <summary>
            /// An event fired whenever a GetDefaultChargingTariff request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetDefaultChargingTariffRequestDelegate?   OnGetDefaultChargingTariffRequest;

            /// <summary>
            /// An event fired whenever a response to a GetDefaultChargingTariff request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnGetDefaultChargingTariffResponseDelegate?  OnGetDefaultChargingTariffResponse;

            #endregion

            #region RemoveDefaultChargingTariff

            /// <summary>
            /// An event fired whenever a RemoveDefaultChargingTariff request was received from the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnRemoveDefaultChargingTariffRequestDelegate?   OnRemoveDefaultChargingTariffRequest;

            /// <summary>
            /// An event fired whenever a response to a RemoveDefaultChargingTariff request was sent.
            /// </summary>
            public event OCPPv2_1.CS.OnRemoveDefaultChargingTariffResponseDelegate?  OnRemoveDefaultChargingTariffResponse;

            #endregion


            public event OCPPv2_1.CS.OnResetDelegate? OnReset;
            public event OCPPv2_1.CS.OnUpdateFirmwareDelegate? OnUpdateFirmware;
            public event OCPPv2_1.CS.OnPublishFirmwareDelegate? OnPublishFirmware;
            public event OCPPv2_1.CS.OnUnpublishFirmwareDelegate? OnUnpublishFirmware;
            public event OCPPv2_1.CS.OnGetBaseReportDelegate? OnGetBaseReport;
            public event OCPPv2_1.CS.OnGetReportDelegate? OnGetReport;
            public event OCPPv2_1.CS.OnGetLogDelegate? OnGetLog;
            public event OCPPv2_1.CS.OnSetVariablesDelegate? OnSetVariables;
            public event OCPPv2_1.CS.OnGetVariablesDelegate? OnGetVariables;
            public event OCPPv2_1.CS.OnSetMonitoringBaseDelegate? OnSetMonitoringBase;
            public event OCPPv2_1.CS.OnGetMonitoringReportDelegate? OnGetMonitoringReport;
            public event OCPPv2_1.CS.OnSetMonitoringLevelDelegate? OnSetMonitoringLevel;
            public event OCPPv2_1.CS.OnSetVariableMonitoringDelegate? OnSetVariableMonitoring;
            public event OCPPv2_1.CS.OnClearVariableMonitoringDelegate? OnClearVariableMonitoring;
            public event OCPPv2_1.CS.OnSetNetworkProfileDelegate? OnSetNetworkProfile;
            public event OCPPv2_1.CS.OnChangeAvailabilityDelegate? OnChangeAvailability;
            public event OCPPv2_1.CS.OnTriggerMessageDelegate? OnTriggerMessage;
            public event             OnIncomingDataTransferDelegate? OnIncomingDataTransfer;
            public event OCPPv2_1.CS.OnCertificateSignedDelegate? OnCertificateSigned;
            public event OCPPv2_1.CS.OnInstallCertificateDelegate? OnInstallCertificate;
            public event OCPPv2_1.CS.OnGetInstalledCertificateIdsDelegate? OnGetInstalledCertificateIds;
            public event OCPPv2_1.CS.OnDeleteCertificateDelegate? OnDeleteCertificate;
            public event OCPPv2_1.CS.OnNotifyCRLDelegate? OnNotifyCRL;
            public event OCPPv2_1.CS.OnGetLocalListVersionDelegate? OnGetLocalListVersion;
            public event OCPPv2_1.CS.OnSendLocalListDelegate? OnSendLocalList;
            public event OCPPv2_1.CS.OnClearCacheDelegate? OnClearCache;
            public event OCPPv2_1.CS.OnReserveNowDelegate? OnReserveNow;
            public event OCPPv2_1.CS.OnCancelReservationDelegate? OnCancelReservation;
            public event OCPPv2_1.CS.OnRequestStartTransactionDelegate? OnRequestStartTransaction;
            public event OCPPv2_1.CS.OnRequestStopTransactionDelegate? OnRequestStopTransaction;
            public event OCPPv2_1.CS.OnGetTransactionStatusDelegate? OnGetTransactionStatus;
            public event OCPPv2_1.CS.OnSetChargingProfileDelegate? OnSetChargingProfile;
            public event OCPPv2_1.CS.OnGetChargingProfilesDelegate? OnGetChargingProfiles;
            public event OCPPv2_1.CS.OnClearChargingProfileDelegate? OnClearChargingProfile;
            public event OCPPv2_1.CS.OnGetCompositeScheduleDelegate? OnGetCompositeSchedule;
            public event OCPPv2_1.CS.OnUpdateDynamicScheduleDelegate? OnUpdateDynamicSchedule;
            public event OCPPv2_1.CS.OnNotifyAllowedEnergyTransferDelegate? OnNotifyAllowedEnergyTransfer;
            public event OCPPv2_1.CS.OnUsePriorityChargingDelegate? OnUsePriorityCharging;
            public event OCPPv2_1.CS.OnUnlockConnectorDelegate? OnUnlockConnector;
            public event OCPPv2_1.CS.OnAFRRSignalDelegate? OnAFRRSignal;
            public event OCPPv2_1.CS.OnSetDisplayMessageDelegate? OnSetDisplayMessage;
            public event OCPPv2_1.CS.OnGetDisplayMessagesDelegate? OnGetDisplayMessages;
            public event OCPPv2_1.CS.OnClearDisplayMessageDelegate? OnClearDisplayMessage;
            public event OCPPv2_1.CS.OnCostUpdatedDelegate? OnCostUpdated;
            public event OCPPv2_1.CS.OnCustomerInformationDelegate? OnCustomerInformation;
            public event             OnIncomingBinaryDataTransferDelegate? OnIncomingBinaryDataTransfer;
            public event OnGetFileDelegate? OnGetFile;
            public event OnSendFileDelegate? OnSendFile;
            public event OnDeleteFileDelegate? OnDeleteFile;
            public event OnAddSignaturePolicyDelegate? OnAddSignaturePolicy;
            public event OnUpdateSignaturePolicyDelegate? OnUpdateSignaturePolicy;
            public event OnDeleteSignaturePolicyDelegate? OnDeleteSignaturePolicy;
            public event OnAddUserRoleDelegate? OnAddUserRole;
            public event OnUpdateUserRoleDelegate? OnUpdateUserRole;
            public event OnDeleteUserRoleDelegate? OnDeleteUserRole;
            public event OCPPv2_1.CS.OnSetDefaultChargingTariffDelegate? OnSetDefaultChargingTariff;
            public event OCPPv2_1.CS.OnGetDefaultChargingTariffDelegate? OnGetDefaultChargingTariff;
            public event OCPPv2_1.CS.OnRemoveDefaultChargingTariffDelegate? OnRemoveDefaultChargingTariff;

            #endregion

            #endregion

            #region Constructor(s)

            public INN(TestNetworkingNode NetworkingNode)
            {

                this.parentNetworkingNode = NetworkingNode;

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

}
