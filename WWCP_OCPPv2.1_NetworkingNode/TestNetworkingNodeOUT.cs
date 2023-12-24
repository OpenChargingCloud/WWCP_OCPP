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

        public class OUTT : INetworkingNodeOUT
        {

            /// <summary>
            /// The sender identification.
            /// </summary>
            String IEventSender.Id
                => parentNetworkingNode.Id.ToString();


            #region Data

            private readonly            TestNetworkingNode          parentNetworkingNode;

            #endregion

            #region Events

            #region Outgoing Message Events

            #region DataTransfer

            /// <summary>
            /// An event fired whenever a DataTransfer request will be sent to the CSMS.
            /// </summary>
            public event OnDataTransferRequestDelegate?   OnDataTransferRequest;

            /// <summary>
            /// An event fired whenever a response to a DataTransfer request was received.
            /// </summary>
            public event OnDataTransferResponseDelegate?  OnDataTransferResponse;

            #endregion


            // Binary Data Streams Extensions

            #region BinaryDataTransfer

            /// <summary>
            /// An event fired whenever a BinaryDataTransfer request will be sent to the CSMS.
            /// </summary>
            public event OnBinaryDataTransferRequestDelegate?   OnBinaryDataTransferRequest;

            /// <summary>
            /// An event fired whenever a response to a BinaryDataTransfer request was received.
            /// </summary>
            public event OnBinaryDataTransferResponseDelegate?  OnBinaryDataTransferResponse;

            #endregion


            // Overlay Networking Extensions

            #region OnNotifyNetworkTopology (Request/-Response)

            /// <summary>
            /// An event fired whenever a NotifyNetworkTopology request will be sent to another node.
            /// </summary>
            public event OCPP.CS.OnNotifyNetworkTopologyRequestDelegate?   OnNotifyNetworkTopologyRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyNetworkTopology request was received.
            /// </summary>
            public event OCPP.CS.OnNotifyNetworkTopologyResponseDelegate?  OnNotifyNetworkTopologyResponse;

            #endregion

            #endregion

            #region Outgoing Message Events: Networking Node -> CSMS

            #region SendBootNotification

            /// <summary>
            /// An event fired whenever a BootNotification request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnBootNotificationRequestDelegate?   OnBootNotificationRequest;

            /// <summary>
            /// An event fired whenever a response to a BootNotification request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnBootNotificationResponseDelegate?  OnBootNotificationResponse;

            #endregion

            #region SendFirmwareStatusNotification

            /// <summary>
            /// An event fired whenever a FirmwareStatusNotification request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnFirmwareStatusNotificationRequestDelegate?   OnFirmwareStatusNotificationRequest;

            /// <summary>
            /// An event fired whenever a response to a FirmwareStatusNotification request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnFirmwareStatusNotificationResponseDelegate?  OnFirmwareStatusNotificationResponse;

            #endregion

            #region SendPublishFirmwareStatusNotification

            /// <summary>
            /// An event fired whenever a PublishFirmwareStatusNotification request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnPublishFirmwareStatusNotificationRequestDelegate?   OnPublishFirmwareStatusNotificationRequest;

            /// <summary>
            /// An event fired whenever a response to a PublishFirmwareStatusNotification request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnPublishFirmwareStatusNotificationResponseDelegate?  OnPublishFirmwareStatusNotificationResponse;

            #endregion

            #region SendHeartbeat

            /// <summary>
            /// An event fired whenever a Heartbeat request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnHeartbeatRequestDelegate?   OnHeartbeatRequest;

            /// <summary>
            /// An event fired whenever a response to a Heartbeat request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnHeartbeatResponseDelegate?  OnHeartbeatResponse;

            #endregion

            #region NotifyEvent

            /// <summary>
            /// An event fired whenever a NotifyEvent request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyEventRequestDelegate?   OnNotifyEventRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyEvent request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyEventResponseDelegate?  OnNotifyEventResponse;

            #endregion

            #region SendSecurityEventNotification

            /// <summary>
            /// An event fired whenever a SecurityEventNotification request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSecurityEventNotificationRequestDelegate?   OnSecurityEventNotificationRequest;

            /// <summary>
            /// An event fired whenever a response to a SecurityEventNotification request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnSecurityEventNotificationResponseDelegate?  OnSecurityEventNotificationResponse;

            #endregion

            #region NotifyReport

            /// <summary>
            /// An event fired whenever a NotifyReport request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyReportRequestDelegate?   OnNotifyReportRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyReport request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyReportResponseDelegate?  OnNotifyReportResponse;

            #endregion

            #region NotifyMonitoringReport

            /// <summary>
            /// An event fired whenever a NotifyMonitoringReport request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyMonitoringReportRequestDelegate?   OnNotifyMonitoringReportRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyMonitoringReport request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyMonitoringReportResponseDelegate?  OnNotifyMonitoringReportResponse;

            #endregion

            #region SendLogStatusNotification

            /// <summary>
            /// An event fired whenever a LogStatusNotification request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnLogStatusNotificationRequestDelegate?   OnLogStatusNotificationRequest;

            /// <summary>
            /// An event fired whenever a response to a LogStatusNotification request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnLogStatusNotificationResponseDelegate?  OnLogStatusNotificationResponse;

            #endregion


            #region SignCertificate

            /// <summary>
            /// An event fired whenever a SignCertificate request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnSignCertificateRequestDelegate?   OnSignCertificateRequest;

            /// <summary>
            /// An event fired whenever a response to a SignCertificate request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnSignCertificateResponseDelegate?  OnSignCertificateResponse;

            #endregion

            #region Get15118EVCertificate

            /// <summary>
            /// An event fired whenever a Get15118EVCertificate request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGet15118EVCertificateRequestDelegate?   OnGet15118EVCertificateRequest;

            /// <summary>
            /// An event fired whenever a response to a Get15118EVCertificate request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnGet15118EVCertificateResponseDelegate?  OnGet15118EVCertificateResponse;

            #endregion

            #region GetCertificateStatus

            /// <summary>
            /// An event fired whenever a GetCertificateStatus request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetCertificateStatusRequestDelegate?   OnGetCertificateStatusRequest;

            /// <summary>
            /// An event fired whenever a response to a GetCertificateStatus request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnGetCertificateStatusResponseDelegate?  OnGetCertificateStatusResponse;

            #endregion

            #region GetCRL

            /// <summary>
            /// An event fired whenever a GetCRL request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnGetCRLRequestDelegate?   OnGetCRLRequest;

            /// <summary>
            /// An event fired whenever a response to a GetCRL request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnGetCRLResponseDelegate?  OnGetCRLResponse;

            #endregion


            #region SendReservationStatusUpdate

            /// <summary>
            /// An event fired whenever a ReservationStatusUpdate request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnReservationStatusUpdateRequestDelegate?   OnReservationStatusUpdateRequest;

            /// <summary>
            /// An event fired whenever a response to a ReservationStatusUpdate request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnReservationStatusUpdateResponseDelegate?  OnReservationStatusUpdateResponse;

            #endregion

            #region Authorize

            /// <summary>
            /// An event fired whenever an Authorize request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnAuthorizeRequestDelegate?   OnAuthorizeRequest;

            /// <summary>
            /// An event fired whenever a response to an Authorize request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnAuthorizeResponseDelegate?  OnAuthorizeResponse;

            #endregion

            #region NotifyEVChargingNeeds

            /// <summary>
            /// An event fired whenever a NotifyEVChargingNeeds request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyEVChargingNeedsRequestDelegate?   OnNotifyEVChargingNeedsRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyEVChargingNeeds request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyEVChargingNeedsResponseDelegate?  OnNotifyEVChargingNeedsResponse;

            #endregion

            #region SendTransactionEvent

            /// <summary>
            /// An event fired whenever a TransactionEvent will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnTransactionEventRequestDelegate?   OnTransactionEventRequest;

            /// <summary>
            /// An event fired whenever a response to a TransactionEvent request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnTransactionEventResponseDelegate?  OnTransactionEventResponse;

            #endregion

            #region SendStatusNotification

            /// <summary>
            /// An event fired whenever a StatusNotification request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnStatusNotificationRequestDelegate?   OnStatusNotificationRequest;

            /// <summary>
            /// An event fired whenever a response to a StatusNotification request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnStatusNotificationResponseDelegate?  OnStatusNotificationResponse;

            #endregion

            #region SendMeterValues

            /// <summary>
            /// An event fired whenever a MeterValues request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnMeterValuesRequestDelegate?   OnMeterValuesRequest;

            /// <summary>
            /// An event fired whenever a response to a MeterValues request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnMeterValuesResponseDelegate?  OnMeterValuesResponse;

            #endregion

            #region NotifyChargingLimit

            /// <summary>
            /// An event fired whenever a NotifyChargingLimit request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyChargingLimitRequestDelegate?   OnNotifyChargingLimitRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyChargingLimit request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyChargingLimitResponseDelegate?  OnNotifyChargingLimitResponse;

            #endregion

            #region SendClearedChargingLimit

            /// <summary>
            /// An event fired whenever a ClearedChargingLimit request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnClearedChargingLimitRequestDelegate?   OnClearedChargingLimitRequest;

            /// <summary>
            /// An event fired whenever a response to a ClearedChargingLimit request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnClearedChargingLimitResponseDelegate?  OnClearedChargingLimitResponse;

            #endregion

            #region ReportChargingProfiles

            /// <summary>
            /// An event fired whenever a ReportChargingProfiles request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnReportChargingProfilesRequestDelegate?   OnReportChargingProfilesRequest;

            /// <summary>
            /// An event fired whenever a response to a ReportChargingProfiles request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnReportChargingProfilesResponseDelegate?  OnReportChargingProfilesResponse;

            #endregion

            #region NotifyEVChargingSchedule

            /// <summary>
            /// An event fired whenever a NotifyEVChargingSchedule request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyEVChargingScheduleRequestDelegate?   OnNotifyEVChargingScheduleRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyEVChargingSchedule request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyEVChargingScheduleResponseDelegate?  OnNotifyEVChargingScheduleResponse;

            #endregion

            #region NotifyPriorityCharging

            /// <summary>
            /// An event fired whenever a NotifyPriorityCharging request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyPriorityChargingRequestDelegate?   OnNotifyPriorityChargingRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyPriorityCharging request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyPriorityChargingResponseDelegate?  OnNotifyPriorityChargingResponse;

            #endregion

            #region PullDynamicScheduleUpdate

            /// <summary>
            /// An event fired whenever a PullDynamicScheduleUpdate request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnPullDynamicScheduleUpdateRequestDelegate?   OnPullDynamicScheduleUpdateRequest;

            /// <summary>
            /// An event fired whenever a response to a PullDynamicScheduleUpdate request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnPullDynamicScheduleUpdateResponseDelegate?  OnPullDynamicScheduleUpdateResponse;

            #endregion


            #region NotifyDisplayMessages

            /// <summary>
            /// An event fired whenever a NotifyDisplayMessages request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyDisplayMessagesRequestDelegate?   OnNotifyDisplayMessagesRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyDisplayMessages request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyDisplayMessagesResponseDelegate?  OnNotifyDisplayMessagesResponse;

            #endregion

            #region NotifyCustomerInformation

            /// <summary>
            /// An event fired whenever a NotifyCustomerInformation request will be sent to the CSMS.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyCustomerInformationRequestDelegate?   OnNotifyCustomerInformationRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyCustomerInformation request was received.
            /// </summary>
            public event OCPPv2_1.CS.OnNotifyCustomerInformationResponseDelegate?  OnNotifyCustomerInformationResponse;

            #endregion

            #endregion

            #region Outgoing Message Events: Networking Node -> Charging Station

            #region OnReset                       (Request/-Response)

            /// <summary>
            /// An event fired whenever a Reset request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnResetRequestDelegate?   OnResetRequest;

            public async Task RaiseOnResetRequest(DateTime                    Timestamp,
                                                  IEventSender                Sender,
                                                  OCPPv2_1.CSMS.ResetRequest  Request)
            {

                var requestLogger = OnResetRequest;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OCPPv2_1.CSMS.OnResetRequestDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                             Sender,
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
            /// An event fired whenever a response to a Reset request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnResetResponseDelegate?  OnResetResponse;

            public async Task RaiseOnResetResponse(DateTime                    Timestamp,
                                                   IEventSender                Sender,
                                                   OCPPv2_1.CSMS.ResetRequest  Request,
                                                   OCPPv2_1.CS.ResetResponse   Response,
                                                   TimeSpan                    Runtime)
            {

                var requestLogger = OnResetResponse;
                if (requestLogger is not null)
                {

                    var requestLoggerTasks = requestLogger.GetInvocationList().
                                                           OfType <OCPPv2_1.CSMS.OnResetResponseDelegate>().
                                                           Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                             Sender,
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

            #region OnUpdateFirmware              (Request/-Response)

            /// <summary>
            /// An event fired whenever an UpdateFirmware request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

            /// <summary>
            /// An event fired whenever a response to an UpdateFirmware request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

            #endregion

            #region OnPublishFirmware             (Request/-Response)

            /// <summary>
            /// An event fired whenever a PublishFirmware request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnPublishFirmwareRequestDelegate?   OnPublishFirmwareRequest;

            /// <summary>
            /// An event fired whenever a response to a PublishFirmware request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnPublishFirmwareResponseDelegate?  OnPublishFirmwareResponse;

            #endregion

            #region OnUnpublishFirmware           (Request/-Response)

            /// <summary>
            /// An event fired whenever an UnpublishFirmware request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnUnpublishFirmwareRequestDelegate?   OnUnpublishFirmwareRequest;

            /// <summary>
            /// An event fired whenever a response to an UnpublishFirmware request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnUnpublishFirmwareResponseDelegate?  OnUnpublishFirmwareResponse;

            #endregion

            #region OnGetBaseReport               (Request/-Response)

            /// <summary>
            /// An event fired whenever a GetBaseReport request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetBaseReportRequestDelegate?   OnGetBaseReportRequest;

            /// <summary>
            /// An event fired whenever a response to a GetBaseReport request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetBaseReportResponseDelegate?  OnGetBaseReportResponse;

            #endregion

            #region OnGetReport                   (Request/-Response)

            /// <summary>
            /// An event fired whenever a GetReport request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetReportRequestDelegate?   OnGetReportRequest;

            /// <summary>
            /// An event fired whenever a response to a GetReport request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetReportResponseDelegate?  OnGetReportResponse;

            #endregion

            #region OnGetLog                      (Request/-Response)

            /// <summary>
            /// An event fired whenever a GetLog request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetLogRequestDelegate?   OnGetLogRequest;

            /// <summary>
            /// An event fired whenever a response to a GetLog request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetLogResponseDelegate?  OnGetLogResponse;

            #endregion

            #region OnSetVariables                (Request/-Response)

            /// <summary>
            /// An event fired whenever a SetVariables request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSetVariablesRequestDelegate?   OnSetVariablesRequest;

            /// <summary>
            /// An event fired whenever a response to a SetVariables request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSetVariablesResponseDelegate?  OnSetVariablesResponse;

            #endregion

            #region OnGetVariables                (Request/-Response)

            /// <summary>
            /// An event fired whenever a GetVariables request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetVariablesRequestDelegate?   OnGetVariablesRequest;

            /// <summary>
            /// An event fired whenever a response to a GetVariables request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetVariablesResponseDelegate?  OnGetVariablesResponse;

            #endregion

            #region OnSetMonitoringBase           (Request/-Response)

            /// <summary>
            /// An event fired whenever a SetMonitoringBase request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSetMonitoringBaseRequestDelegate?   OnSetMonitoringBaseRequest;

            /// <summary>
            /// An event fired whenever a response to a SetMonitoringBase request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSetMonitoringBaseResponseDelegate?  OnSetMonitoringBaseResponse;

            #endregion

            #region OnGetMonitoringReport         (Request/-Response)

            /// <summary>
            /// An event fired whenever a GetMonitoringReport request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetMonitoringReportRequestDelegate?   OnGetMonitoringReportRequest;

            /// <summary>
            /// An event fired whenever a response to a GetMonitoringReport request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetMonitoringReportResponseDelegate?  OnGetMonitoringReportResponse;

            #endregion

            #region OnSetMonitoringLevel          (Request/-Response)

            /// <summary>
            /// An event fired whenever a SetMonitoringLevel request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSetMonitoringLevelRequestDelegate?   OnSetMonitoringLevelRequest;

            /// <summary>
            /// An event fired whenever a response to a SetMonitoringLevel request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSetMonitoringLevelResponseDelegate?  OnSetMonitoringLevelResponse;

            #endregion

            #region SetVariableMonitoring         (Request/-Response)

            /// <summary>
            /// An event fired whenever a SetVariableMonitoring request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSetVariableMonitoringRequestDelegate?   OnSetVariableMonitoringRequest;

            /// <summary>
            /// An event fired whenever a response to a SetVariableMonitoring request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSetVariableMonitoringResponseDelegate?  OnSetVariableMonitoringResponse;

            #endregion

            #region OnClearVariableMonitoring     (Request/-Response)

            /// <summary>
            /// An event fired whenever a ClearVariableMonitoring request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnClearVariableMonitoringRequestDelegate?   OnClearVariableMonitoringRequest;

            /// <summary>
            /// An event fired whenever a response to a ClearVariableMonitoring request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnClearVariableMonitoringResponseDelegate?  OnClearVariableMonitoringResponse;

            #endregion

            #region OnSetNetworkProfile           (Request/-Response)

            /// <summary>
            /// An event fired whenever a SetNetworkProfile request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSetNetworkProfileRequestDelegate?   OnSetNetworkProfileRequest;

            /// <summary>
            /// An event fired whenever a response to a SetNetworkProfile request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSetNetworkProfileResponseDelegate?  OnSetNetworkProfileResponse;

            #endregion

            #region OnChangeAvailability          (Request/-Response)

            /// <summary>
            /// An event fired whenever a ChangeAvailability request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

            /// <summary>
            /// An event fired whenever a response to a ChangeAvailability request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

            #endregion

            #region OnTriggerMessage              (Request/-Response)

            /// <summary>
            /// An event fired whenever a TriggerMessage request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

            /// <summary>
            /// An event fired whenever a response to a TriggerMessage request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

            #endregion


            #region OnCertificateSigned           (Request/-Response)

            /// <summary>
            /// An event fired whenever a SignedCertificate request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

            /// <summary>
            /// An event fired whenever a response to a SignedCertificate request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

            #endregion

            #region OnInstallCertificate          (Request/-Response)

            /// <summary>
            /// An event fired whenever an InstallCertificate request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

            /// <summary>
            /// An event fired whenever a response to an InstallCertificate request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

            #endregion

            #region OnGetInstalledCertificateIds  (Request/-Response)

            /// <summary>
            /// An event fired whenever a GetInstalledCertificateIds request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

            /// <summary>
            /// An event fired whenever a response to a GetInstalledCertificateIds request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

            #endregion

            #region OnDeleteCertificate           (Request/-Response)

            /// <summary>
            /// An event fired whenever a DeleteCertificate request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

            /// <summary>
            /// An event fired whenever a response to a DeleteCertificate request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

            #endregion

            #region OnNotifyCRL                   (Request/-Response)

            /// <summary>
            /// An event fired whenever a NotifyCRL request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnNotifyCRLRequestDelegate?   OnNotifyCRLRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyCRL request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnNotifyCRLResponseDelegate?  OnNotifyCRLResponse;

            #endregion


            #region OnGetLocalListVersion         (Request/-Response)

            /// <summary>
            /// An event fired whenever a GetLocalListVersion request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

            /// <summary>
            /// An event fired whenever a response to a GetLocalListVersion request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

            #endregion

            #region OnSendLocalList               (Request/-Response)

            /// <summary>
            /// An event fired whenever a SendLocalList request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

            /// <summary>
            /// An event fired whenever a response to a SendLocalList request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

            #endregion

            #region OnClearCache                  (Request/-Response)

            /// <summary>
            /// An event fired whenever a ClearCache request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnClearCacheRequestDelegate?   OnClearCacheRequest;

            /// <summary>
            /// An event fired whenever a response to a ClearCache request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnClearCacheResponseDelegate?  OnClearCacheResponse;

            #endregion


            #region OnReserveNow                  (Request/-Response)

            /// <summary>
            /// An event fired whenever a ReserveNow request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnReserveNowRequestDelegate?   OnReserveNowRequest;

            /// <summary>
            /// An event fired whenever a response to a ReserveNow request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnReserveNowResponseDelegate?  OnReserveNowResponse;

            #endregion

            #region OnCancelReservation           (Request/-Response)

            /// <summary>
            /// An event fired whenever a CancelReservation request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

            /// <summary>
            /// An event fired whenever a response to a CancelReservation request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

            #endregion

            #region OnRequestStartTransaction     (Request/-Response)

            /// <summary>
            /// An event fired whenever a RequestStartTransaction request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnRequestStartTransactionRequestDelegate?   OnRequestStartTransactionRequest;

            /// <summary>
            /// An event fired whenever a response to a RequestStartTransaction request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnRequestStartTransactionResponseDelegate?  OnRequestStartTransactionResponse;

            #endregion

            #region OnRequestStopTransaction      (Request/-Response)

            /// <summary>
            /// An event fired whenever a RequestStopTransaction request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnRequestStopTransactionRequestDelegate?   OnRequestStopTransactionRequest;

            /// <summary>
            /// An event fired whenever a response to a RequestStopTransaction request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnRequestStopTransactionResponseDelegate?  OnRequestStopTransactionResponse;

            #endregion

            #region OnGetTransactionStatus        (Request/-Response)

            /// <summary>
            /// An event fired whenever a GetTransactionStatus request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetTransactionStatusRequestDelegate?   OnGetTransactionStatusRequest;

            /// <summary>
            /// An event fired whenever a response to a GetTransactionStatus request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetTransactionStatusResponseDelegate?  OnGetTransactionStatusResponse;

            #endregion

            #region OnSetChargingProfile          (Request/-Response)

            /// <summary>
            /// An event fired whenever a SetChargingProfile request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

            /// <summary>
            /// An event fired whenever a response to a SetChargingProfile request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

            #endregion

            #region OnGetChargingProfiles         (Request/-Response)

            /// <summary>
            /// An event fired whenever a GetChargingProfiles request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetChargingProfilesRequestDelegate?   OnGetChargingProfilesRequest;

            /// <summary>
            /// An event fired whenever a response to a GetChargingProfiles request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetChargingProfilesResponseDelegate?  OnGetChargingProfilesResponse;

            #endregion

            #region OnClearChargingProfile        (Request/-Response)

            /// <summary>
            /// An event fired whenever a ClearChargingProfile request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

            /// <summary>
            /// An event fired whenever a response to a ClearChargingProfile request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

            #endregion

            #region OnGetCompositeSchedule        (Request/-Response)

            /// <summary>
            /// An event fired whenever a GetCompositeSchedule request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

            /// <summary>
            /// An event fired whenever a response to a GetCompositeSchedule request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

            #endregion

            #region OnUpdateDynamicSchedule       (Request/-Response)

            /// <summary>
            /// An event fired whenever a UpdateDynamicSchedule request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnUpdateDynamicScheduleRequestDelegate?   OnUpdateDynamicScheduleRequest;

            /// <summary>
            /// An event fired whenever a response to a UpdateDynamicSchedule request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnUpdateDynamicScheduleResponseDelegate?  OnUpdateDynamicScheduleResponse;

            #endregion

            #region OnNotifyAllowedEnergyTransfer (Request/-Response)

            /// <summary>
            /// An event fired whenever a NotifyAllowedEnergyTransfer request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnNotifyAllowedEnergyTransferRequestDelegate?   OnNotifyAllowedEnergyTransferRequest;

            /// <summary>
            /// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnNotifyAllowedEnergyTransferResponseDelegate?  OnNotifyAllowedEnergyTransferResponse;

            #endregion

            #region OnUsePriorityCharging         (Request/-Response)

            /// <summary>
            /// An event fired whenever a UsePriorityCharging request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnUsePriorityChargingRequestDelegate?   OnUsePriorityChargingRequest;

            /// <summary>
            /// An event fired whenever a response to a UsePriorityCharging request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnUsePriorityChargingResponseDelegate?  OnUsePriorityChargingResponse;

            #endregion

            #region OnUnlockConnector             (Request/-Response)

            /// <summary>
            /// An event fired whenever an UnlockConnector request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

            /// <summary>
            /// An event fired whenever a response to an UnlockConnector request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

            #endregion


            #region OnAFRRSignal                  (Request/-Response)

            /// <summary>
            /// An event fired whenever an AFRRSignal request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnAFRRSignalRequestDelegate?   OnAFRRSignalRequest;

            /// <summary>
            /// An event fired whenever a response to an AFRRSignal request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnAFRRSignalResponseDelegate?  OnAFRRSignalResponse;

            #endregion


            #region SetDisplayMessage/-Response   (Request/-Response)

            /// <summary>
            /// An event fired whenever a SetDisplayMessage request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSetDisplayMessageRequestDelegate?   OnSetDisplayMessageRequest;

            /// <summary>
            /// An event fired whenever a response to a SetDisplayMessage request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSetDisplayMessageResponseDelegate?  OnSetDisplayMessageResponse;

            #endregion

            #region OnGetDisplayMessages          (Request/-Response)

            /// <summary>
            /// An event fired whenever a GetDisplayMessages request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetDisplayMessagesRequestDelegate?   OnGetDisplayMessagesRequest;

            /// <summary>
            /// An event fired whenever a response to a GetDisplayMessages request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetDisplayMessagesResponseDelegate?  OnGetDisplayMessagesResponse;

            #endregion

            #region OnClearDisplayMessage         (Request/-Response)

            /// <summary>
            /// An event fired whenever a ClearDisplayMessage request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnClearDisplayMessageRequestDelegate?   OnClearDisplayMessageRequest;

            /// <summary>
            /// An event fired whenever a response to a ClearDisplayMessage request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnClearDisplayMessageResponseDelegate?  OnClearDisplayMessageResponse;

            #endregion

            #region OnCostUpdated                 (Request/-Response)

            /// <summary>
            /// An event fired whenever a CostUpdated request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnCostUpdatedRequestDelegate?   OnCostUpdatedRequest;

            /// <summary>
            /// An event fired whenever a response to a CostUpdated request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnCostUpdatedResponseDelegate?  OnCostUpdatedResponse;

            #endregion

            #region OnCustomerInformation         (Request/-Response)

            /// <summary>
            /// An event fired whenever a CustomerInformation request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnCustomerInformationRequestDelegate?   OnCustomerInformationRequest;

            /// <summary>
            /// An event fired whenever a response to a CustomerInformation request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnCustomerInformationResponseDelegate?  OnCustomerInformationResponse;

            #endregion


            // Binary Data Streams Extensions

            #region OnGetFile                     (Request/-Response)

            /// <summary>
            /// An event sent whenever a GetFile request will be sent to the charging station.
            /// </summary>
            public event OCPP.CSMS.OnGetFileRequestDelegate?   OnGetFileRequest;

            /// <summary>
            /// An event sent whenever a response to a GetFile request was received.
            /// </summary>
            public event OCPP.CSMS.OnGetFileResponseDelegate?  OnGetFileResponse;

            #endregion

            #region OnSendFile                    (Request/-Response)

            /// <summary>
            /// An event sent whenever a SendFile request will be sent to the charging station.
            /// </summary>
            public event OCPP.CSMS.OnSendFileRequestDelegate?   OnSendFileRequest;

            /// <summary>
            /// An event sent whenever a response to a SendFile request was received.
            /// </summary>
            public event OCPP.CSMS.OnSendFileResponseDelegate?  OnSendFileResponse;

            #endregion

            #region OnDeleteFile                  (Request/-Response)

            /// <summary>
            /// An event sent whenever a DeleteFile request will be sent to the charging station.
            /// </summary>
            public event OCPP.CSMS.OnDeleteFileRequestDelegate?   OnDeleteFileRequest;

            /// <summary>
            /// An event sent whenever a response to a DeleteFile request was received.
            /// </summary>
            public event OCPP.CSMS.OnDeleteFileResponseDelegate?  OnDeleteFileResponse;

            #endregion

            #region OnListDirectory               (Request/-Response)

            /// <summary>
            /// An event sent whenever a ListDirectory request will be sent to the charging station.
            /// </summary>
            public event OCPP.CSMS.OnListDirectoryRequestDelegate?   OnListDirectoryRequest;

            /// <summary>
            /// An event sent whenever a response to a ListDirectory request was received.
            /// </summary>
            public event OCPP.CSMS.OnListDirectoryResponseDelegate?  OnListDirectoryResponse;

            #endregion


            // E2E Security Extensions

            #region AddSignaturePolicy            (Request/-Response)

            /// <summary>
            /// An event fired whenever a AddSignaturePolicy request will be sent to the charging station.
            /// </summary>
            public event OCPP.CSMS.OnAddSignaturePolicyRequestDelegate?   OnAddSignaturePolicyRequest;

            /// <summary>
            /// An event fired whenever a response to a AddSignaturePolicy request was received.
            /// </summary>
            public event OCPP.CSMS.OnAddSignaturePolicyResponseDelegate?  OnAddSignaturePolicyResponse;

            #endregion

            #region UpdateSignaturePolicy         (Request/-Response)

            /// <summary>
            /// An event fired whenever a UpdateSignaturePolicy request will be sent to the charging station.
            /// </summary>
            public event OCPP.CSMS.OnUpdateSignaturePolicyRequestDelegate?   OnUpdateSignaturePolicyRequest;

            /// <summary>
            /// An event fired whenever a response to a UpdateSignaturePolicy request was received.
            /// </summary>
            public event OCPP.CSMS.OnUpdateSignaturePolicyResponseDelegate?  OnUpdateSignaturePolicyResponse;

            #endregion

            #region DeleteSignaturePolicy         (Request/-Response)

            /// <summary>
            /// An event fired whenever a DeleteSignaturePolicy request will be sent to the charging station.
            /// </summary>
            public event OCPP.CSMS.OnDeleteSignaturePolicyRequestDelegate?   OnDeleteSignaturePolicyRequest;

            /// <summary>
            /// An event fired whenever a response to a DeleteSignaturePolicy request was received.
            /// </summary>
            public event OCPP.CSMS.OnDeleteSignaturePolicyResponseDelegate?  OnDeleteSignaturePolicyResponse;

            #endregion

            #region AddUserRole                   (Request/-Response)

            /// <summary>
            /// An event fired whenever a AddUserRole request will be sent to the charging station.
            /// </summary>
            public event OCPP.CSMS.OnAddUserRoleRequestDelegate?   OnAddUserRoleRequest;

            /// <summary>
            /// An event fired whenever a response to a AddUserRole request was received.
            /// </summary>
            public event OCPP.CSMS.OnAddUserRoleResponseDelegate?  OnAddUserRoleResponse;

            #endregion

            #region UpdateUserRole                (Request/-Response)

            /// <summary>
            /// An event fired whenever a UpdateUserRole request will be sent to the charging station.
            /// </summary>
            public event OCPP.CSMS.OnUpdateUserRoleRequestDelegate?   OnUpdateUserRoleRequest;

            /// <summary>
            /// An event fired whenever a response to a UpdateUserRole request was received.
            /// </summary>
            public event OCPP.CSMS.OnUpdateUserRoleResponseDelegate?  OnUpdateUserRoleResponse;

            #endregion

            #region DeleteUserRole                (Request/-Response)

            /// <summary>
            /// An event fired whenever a DeleteUserRole request will be sent to the charging station.
            /// </summary>
            public event OCPP.CSMS.OnDeleteUserRoleRequestDelegate?   OnDeleteUserRoleRequest;

            /// <summary>
            /// An event fired whenever a response to a DeleteUserRole request was received.
            /// </summary>
            public event OCPP.CSMS.OnDeleteUserRoleResponseDelegate?  OnDeleteUserRoleResponse;

            #endregion


            // E2E Charging Tariff Extensions

            #region SetDefaultChargingTariff      (Request/-Response)

            /// <summary>
            /// An event fired whenever a SetDefaultChargingTariff request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSetDefaultChargingTariffRequestDelegate?   OnSetDefaultChargingTariffRequest;

            /// <summary>
            /// An event fired whenever a response to a SetDefaultChargingTariff request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnSetDefaultChargingTariffResponseDelegate?  OnSetDefaultChargingTariffResponse;

            #endregion

            #region GetDefaultChargingTariff      (Request/-Response)

            /// <summary>
            /// An event fired whenever a GetDefaultChargingTariff request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetDefaultChargingTariffRequestDelegate?   OnGetDefaultChargingTariffRequest;

            /// <summary>
            /// An event fired whenever a response to a GetDefaultChargingTariff request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnGetDefaultChargingTariffResponseDelegate?  OnGetDefaultChargingTariffResponse;

            #endregion

            #region RemoveDefaultChargingTariff   (Request/-Response)

            /// <summary>
            /// An event fired whenever a RemoveDefaultChargingTariff request will be sent to the charging station.
            /// </summary>
            public event OCPPv2_1.CSMS.OnRemoveDefaultChargingTariffRequestDelegate?   OnRemoveDefaultChargingTariffRequest;

            /// <summary>
            /// An event fired whenever a response to a RemoveDefaultChargingTariff request was received.
            /// </summary>
            public event OCPPv2_1.CSMS.OnRemoveDefaultChargingTariffResponseDelegate?  OnRemoveDefaultChargingTariffResponse;

            #endregion

            #endregion

            #endregion

            #region Constructor(s)

            public OUTT(TestNetworkingNode NetworkingNode)
            {

                this.parentNetworkingNode = NetworkingNode;

            }

            #endregion


            #region Outgoing Messages (Common)

            #region DataTransfer                      (Request)

            /// <summary>
            /// Send the given vendor-specific data to the CSMS.
            /// </summary>
            /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
            /// <param name="MessageId">An optional message identification.</param>
            /// <param name="Data">A vendor-specific JSON token.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<DataTransferResponse>
                DataTransfer(DataTransferRequest Request)

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
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnDataTransferRequest));
                }

                #endregion


                DataTransferResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomDataTransferRequestSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new DataTransferResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.DataTransfer(Request)

                                    : new DataTransferResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomDataTransferResponseSerializer,
                        parentNetworkingNode.CustomStatusInfoSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
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
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnDataTransferResponse));
                }

                #endregion

                return response;

            }

            #endregion

            // Binary Data Streams Extensions

            #region BinaryDataTransfer                    (Request)

            /// <summary>
            /// Send the given vendor-specific binary data to the CSMS.
            /// </summary>
            /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
            /// <param name="MessageId">An optional message identification.</param>
            /// <param name="BinaryData">A vendor-specific JSON token.</param>
            /// <param name="CustomBinaryData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<BinaryDataTransferResponse>
                BinaryDataTransfer(BinaryDataTransferRequest Request)

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
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnBinaryDataTransferRequest));
                }

                #endregion


                BinaryDataTransferResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToBinary(
                            parentNetworkingNode.CustomBinaryDataTransferRequestSerializer,
                            parentNetworkingNode.CustomBinarySignatureSerializer,
                            IncludeSignatures: false
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new BinaryDataTransferResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.BinaryDataTransfer(Request)

                                    : new BinaryDataTransferResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToBinary(
                        parentNetworkingNode.CustomBinaryDataTransferResponseSerializer,
                        null, //parentNetworkingNode.CustomStatusInfoSerializer,
                        parentNetworkingNode.CustomBinarySignatureSerializer,
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
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnBinaryDataTransferResponse));
                }

                #endregion

                return response;

            }

            #endregion


            // Overlay Networking Extensions

            #region NotifyNetworkTopology                 (Request)

            /// <summary>
            /// Notify about the current network topology or a current change within the topology.
            /// </summary>
            /// <param name="Request">A reset request.</param>
            public async Task<NotifyNetworkTopologyResponse> NotifyNetworkTopology(NotifyNetworkTopologyRequest Request)
            {

                #region Send OnNotifyNetworkTopologyRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnNotifyNetworkTopologyRequest?.Invoke(startTime,
                                                           this,
                                                           Request);
                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyNetworkTopologyRequest));
                }

                #endregion


                NotifyNetworkTopologyResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomNotifyNetworkTopologyRequestSerializer,
                            parentNetworkingNode.CustomNetworkTopologyInformationSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new NotifyNetworkTopologyResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.NotifyNetworkTopology(Request)

                                    : new NotifyNetworkTopologyResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {

                    response = parentNetworkingNode.LookupNetworkingNode(Request.DestinationNodeId, out var communicationChannel) &&
                                    communicationChannel is not null

                                    // FUTURE!!!

                                    ? await communicationChannel.NotifyNetworkTopology(Request)

                                    : new NotifyNetworkTopologyResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomNotifyNetworkTopologyResponseSerializer,
                        //parentNetworkingNode.CustomStatusInfoSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnNotifyNetworkTopologyResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnNotifyNetworkTopologyResponse?.Invoke(endTime,
                                                            this,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyNetworkTopologyResponse));
                }

                #endregion

                return response;

            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            #endregion

            #endregion

            #region Outgoing Messages: Networking Node -> CSMS

            #region BootNotification                  (Request)

            /// <summary>
            /// Send a boot notification.
            /// </summary>
            /// <param name="Request">A boot notification request.</param>
            public async Task<OCPPv2_1.CSMS.BootNotificationResponse>
                BootNotification(OCPPv2_1.CS.BootNotificationRequest Request)

            {

                #region Send OnBootNotificationRequest event

                var startTime  = Timestamp.Now;

                try
                {

                    OnBootNotificationRequest?.Invoke(startTime,
                                                      this,
                                                      Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnBootNotificationRequest));
                }

                #endregion


                OCPPv2_1.CSMS.BootNotificationResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomBootNotificationRequestSerializer,
                            parentNetworkingNode.CustomChargingStationSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.BootNotificationResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.BootNotification(Request)

                                    : new OCPPv2_1.CSMS.BootNotificationResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomBootNotificationResponseSerializer,
                        parentNetworkingNode.CustomStatusInfoSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                switch (response.Status)
                {

                    case RegistrationStatus.Accepted:
                        this.parentNetworkingNode.CSMSTime               = response.CurrentTime;
                        this.parentNetworkingNode.SendHeartbeatEvery     = response.Interval >= TimeSpan.FromSeconds(5) ? response.Interval : TimeSpan.FromSeconds(5);
                 //       this.SendHeartbeatTimer.Change(this.SendHeartbeatEvery, this.SendHeartbeatEvery);
                        this.parentNetworkingNode.DisableSendHeartbeats  = false;
                        break;

                    case RegistrationStatus.Pending:
                        // Do not reconnect before: response.HeartbeatInterval
                        break;

                    case RegistrationStatus.Rejected:
                        // Do not reconnect before: response.HeartbeatInterval
                        break;

                }


                #region Send OnBootNotificationResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnBootNotificationResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnBootNotificationResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region FirmwareStatusNotification        (Request)

            /// <summary>
            /// Send a firmware status notification to the CSMS.
            /// </summary>
            /// <param name="Status">The status of the firmware installation.</param>
            /// <param name="UpdateFirmwareRequestId">The (optional) request id that was provided in the UpdateFirmwareRequest that started this firmware update.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.FirmwareStatusNotificationResponse>
                FirmwareStatusNotification(OCPPv2_1.CS.FirmwareStatusNotificationRequest Request)

            {

                #region Send OnFirmwareStatusNotificationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnFirmwareStatusNotificationRequest?.Invoke(startTime,
                                                                this,
                                                                Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnFirmwareStatusNotificationRequest));
                }

                #endregion


                OCPPv2_1.CSMS.FirmwareStatusNotificationResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomFirmwareStatusNotificationRequestSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.FirmwareStatusNotificationResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.FirmwareStatusNotification(Request)

                                    : new OCPPv2_1.CSMS.FirmwareStatusNotificationResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomFirmwareStatusNotificationResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnFirmwareStatusNotificationResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnFirmwareStatusNotificationResponse?.Invoke(endTime,
                                                                 this,
                                                                 Request,
                                                                 response,
                                                                 endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnFirmwareStatusNotificationResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region PublishFirmwareStatusNotification (Request)

            /// <summary>
            /// Send a publish firmware status notification to the CSMS.
            /// </summary>
            /// <param name="Status">The progress status of the publish firmware request.</param>
            /// <param name="PublishFirmwareStatusNotificationRequestId">The optional unique identification of the publish firmware status notification request.</param>
            /// <param name="DownloadLocations">The optional enumeration of downstream firmware download locations for all attached charging stations.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.PublishFirmwareStatusNotificationResponse>
                PublishFirmwareStatusNotification(OCPPv2_1.CS.PublishFirmwareStatusNotificationRequest Request)

            {

                #region Send OnPublishFirmwareStatusNotificationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnPublishFirmwareStatusNotificationRequest?.Invoke(startTime,
                                                                       this,
                                                                       Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnPublishFirmwareStatusNotificationRequest));
                }

                #endregion


                OCPPv2_1.CSMS.PublishFirmwareStatusNotificationResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomPublishFirmwareStatusNotificationRequestSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.PublishFirmwareStatusNotificationResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.PublishFirmwareStatusNotification(Request)

                                    : new OCPPv2_1.CSMS.PublishFirmwareStatusNotificationResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomPublishFirmwareStatusNotificationResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnPublishFirmwareStatusNotificationResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnPublishFirmwareStatusNotificationResponse?.Invoke(endTime,
                                                                        this,
                                                                        Request,
                                                                        response,
                                                                        endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnPublishFirmwareStatusNotificationResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region Heartbeat                         (Request)

            /// <summary>
            /// Send a heartbeat.
            /// </summary>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.HeartbeatResponse>
                Heartbeat(OCPPv2_1.CS.HeartbeatRequest Request)

            {

                #region Send OnHeartbeatRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnHeartbeatRequest?.Invoke(startTime,
                                               this,
                                               Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnHeartbeatRequest));
                }

                #endregion


                OCPPv2_1.CSMS.HeartbeatResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomHeartbeatRequestSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.HeartbeatResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.Heartbeat(Request)

                                    : new OCPPv2_1.CSMS.HeartbeatResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomHeartbeatResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnHeartbeatResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnHeartbeatResponse?.Invoke(endTime,
                                                this,
                                                Request,
                                                response,
                                                endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnHeartbeatResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region NotifyEvent                       (Request)

            /// <summary>
            /// Notify about an event.
            /// </summary>
            /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
            /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
            /// <param name="EventData">The enumeration of event data.</param>
            /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.NotifyEventResponse>
                NotifyEvent(OCPPv2_1.CS.NotifyEventRequest Request)

            {

                #region Send OnNotifyEventRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnNotifyEventRequest?.Invoke(startTime,
                                                 this,
                                                 Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyEventRequest));
                }

                #endregion


                OCPPv2_1.CSMS.NotifyEventResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomNotifyEventRequestSerializer,
                            parentNetworkingNode.CustomEventDataSerializer,
                            parentNetworkingNode.CustomComponentSerializer,
                            parentNetworkingNode.CustomEVSESerializer,
                            parentNetworkingNode.CustomVariableSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.NotifyEventResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.NotifyEvent(Request)

                                    : new OCPPv2_1.CSMS.NotifyEventResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomNotifyEventResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnNotifyEventResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnNotifyEventResponse?.Invoke(endTime,
                                                  this,
                                                  Request,
                                                  response,
                                                  endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyEventResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region SecurityEventNotification         (Request)

            /// <summary>
            /// Send a security event notification.
            /// </summary>
            /// <param name="Type">Type of the security event.</param>
            /// <param name="Timestamp">The timestamp of the security event.</param>
            /// <param name="TechInfo">Optional additional information about the occurred security event.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.SecurityEventNotificationResponse>
                SecurityEventNotification(OCPPv2_1.CS.SecurityEventNotificationRequest Request)

            {

                #region Send OnSecurityEventNotificationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnSecurityEventNotificationRequest?.Invoke(startTime,
                                                               this,
                                                               Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnSecurityEventNotificationRequest));
                }

                #endregion


                OCPPv2_1.CSMS.SecurityEventNotificationResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomSecurityEventNotificationRequestSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.SecurityEventNotificationResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.SecurityEventNotification(Request)

                                    : new OCPPv2_1.CSMS.SecurityEventNotificationResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomSecurityEventNotificationResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnSecurityEventNotificationResponse event

                var endTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

                try
                {

                    OnSecurityEventNotificationResponse?.Invoke(endTime,
                                                                this,
                                                                Request,
                                                                response,
                                                                endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnSecurityEventNotificationResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region NotifyReport                      (Request)

            /// <summary>
            /// Notify about a report.
            /// </summary>
            /// <param name="NotifyReportRequestId">The unique identification of the notify report request.</param>
            /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
            /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
            /// <param name="ReportData">The enumeration of report data. A single report data element contains only the component, variable and variable report data that caused the event.</param>
            /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the report follows in an upcoming NotifyReportRequest message. Default value when omitted is false.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.NotifyReportResponse>
                NotifyReport(OCPPv2_1.CS.NotifyReportRequest Request)

            {

                #region Send OnNotifyReportRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnNotifyReportRequest?.Invoke(startTime,
                                                  this,
                                                  Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyReportRequest));
                }

                #endregion


                OCPPv2_1.CSMS.NotifyReportResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomNotifyReportRequestSerializer,
                            parentNetworkingNode.CustomReportDataSerializer,
                            parentNetworkingNode.CustomComponentSerializer,
                            parentNetworkingNode.CustomEVSESerializer,
                            parentNetworkingNode.CustomVariableSerializer,
                            parentNetworkingNode.CustomVariableAttributeSerializer,
                            parentNetworkingNode.CustomVariableCharacteristicsSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.NotifyReportResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.NotifyReport(Request)

                                    : new OCPPv2_1.CSMS.NotifyReportResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomNotifyReportResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnNotifyReportResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnNotifyReportResponse?.Invoke(endTime,
                                                   this,
                                                   Request,
                                                   response,
                                                   endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyReportResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region NotifyMonitoringReport            (Request)

            /// <summary>
            /// Notify about a monitoring report.
            /// </summary>
            /// <param name="NotifyMonitoringReportRequestId">The unique identification of the notify monitoring report request.</param>
            /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
            /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
            /// <param name="MonitoringData">The enumeration of event data. A single event data element contains only the component, variable and variable monitoring data that caused the event.</param>
            /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.NotifyMonitoringReportResponse>
                NotifyMonitoringReport(OCPPv2_1.CS.NotifyMonitoringReportRequest Request)

            {

                #region Send OnNotifyMonitoringReportRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnNotifyMonitoringReportRequest?.Invoke(startTime,
                                                            this,
                                                            Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyMonitoringReportRequest));
                }

                #endregion


                OCPPv2_1.CSMS.NotifyMonitoringReportResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomNotifyMonitoringReportRequestSerializer,
                            parentNetworkingNode.CustomMonitoringDataSerializer,
                            parentNetworkingNode.CustomComponentSerializer,
                            parentNetworkingNode.CustomEVSESerializer,
                            parentNetworkingNode.CustomVariableSerializer,
                            parentNetworkingNode.CustomVariableMonitoringSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.NotifyMonitoringReportResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.NotifyMonitoringReport(Request)

                                    : new OCPPv2_1.CSMS.NotifyMonitoringReportResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomNotifyMonitoringReportResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnNotifyMonitoringReportResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnNotifyMonitoringReportResponse?.Invoke(endTime,
                                                             this,
                                                             Request,
                                                             response,
                                                             endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyMonitoringReportResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region LogStatusNotification             (Request)

            /// <summary>
            /// Send a log status notification.
            /// </summary>
            /// <param name="Status">The status of the log upload.</param>
            /// <param name="LogRequestId">The optional request id that was provided in the GetLog request that started this log upload.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.LogStatusNotificationResponse>
                LogStatusNotification(OCPPv2_1.CS.LogStatusNotificationRequest Request)

            {

                #region Send OnLogStatusNotificationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnLogStatusNotificationRequest?.Invoke(startTime,
                                                           this,
                                                           Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnLogStatusNotificationRequest));
                }

                #endregion


                OCPPv2_1.CSMS.LogStatusNotificationResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomLogStatusNotificationRequestSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.LogStatusNotificationResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.LogStatusNotification(Request)

                                    : new OCPPv2_1.CSMS.LogStatusNotificationResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomLogStatusNotificationResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnLogStatusNotificationResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnLogStatusNotificationResponse?.Invoke(endTime,
                                                            this,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnLogStatusNotificationResponse));
                }

                #endregion

                return response;

            }

            #endregion


            #region SignCertificate                   (Request)

            /// <summary>
            /// Send a heartbeat.
            /// </summary>
            /// <param name="CSR">The PEM encoded RFC 2986 certificate signing request (CSR) [max 5500].</param>
            /// <param name="CertificateType">Whether the certificate is to be used for both the 15118 connection (if implemented) and the charging station to central system (CSMS) connection.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.SignCertificateResponse>
                SignCertificate(OCPPv2_1.CS.SignCertificateRequest Request)

            {

                #region Send OnSignCertificateRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnSignCertificateRequest?.Invoke(startTime,
                                                     this,
                                                     Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnSignCertificateRequest));
                }

                #endregion


                OCPPv2_1.CSMS.SignCertificateResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomSignCertificateRequestSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.SignCertificateResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.SignCertificate(Request)

                                    : new OCPPv2_1.CSMS.SignCertificateResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomSignCertificateResponseSerializer,
                        parentNetworkingNode.CustomStatusInfoSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnSignCertificateResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnSignCertificateResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnSignCertificateResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region Get15118EVCertificate             (Request)

            /// <summary>
            /// Get an ISO 15118 contract certificate.
            /// </summary>
            /// <param name="ISO15118SchemaVersion">ISO/IEC 15118 schema version used for the session between charging station and electric vehicle. Required for parsing the EXI data stream within the central system.</param>
            /// <param name="CertificateAction">Whether certificate needs to be installed or updated.</param>
            /// <param name="EXIRequest">Base64 encoded certificate installation request from the electric vehicle. [max 5600]</param>
            /// <param name="MaximumContractCertificateChains">Optional number of contracts that EV wants to install at most.</param>
            /// <param name="PrioritizedEMAIds">An optional enumeration of eMA Ids that have priority in case more contracts than maximumContractCertificateChains are available.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.Get15118EVCertificateResponse>
                Get15118EVCertificate(OCPPv2_1.CS.Get15118EVCertificateRequest Request)

            {

                #region Send OnGet15118EVCertificateRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnGet15118EVCertificateRequest?.Invoke(startTime,
                                                           this,
                                                           Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnGet15118EVCertificateRequest));
                }

                #endregion


                OCPPv2_1.CSMS.Get15118EVCertificateResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomGet15118EVCertificateRequestSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.Get15118EVCertificateResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.Get15118EVCertificate(Request)

                                    : new OCPPv2_1.CSMS.Get15118EVCertificateResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomGet15118EVCertificateResponseSerializer,
                        parentNetworkingNode.CustomStatusInfoSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnGet15118EVCertificateResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnGet15118EVCertificateResponse?.Invoke(endTime,
                                                            this,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnGet15118EVCertificateResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region GetCertificateStatus              (Request)

            /// <summary>
            /// Get the status of a certificate.
            /// </summary>
            /// <param name="OCSPRequestData">The certificate of which the status is requested.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.GetCertificateStatusResponse>
                GetCertificateStatus(OCPPv2_1.CS.GetCertificateStatusRequest Request)

            {

                #region Send OnGetCertificateStatusRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnGetCertificateStatusRequest?.Invoke(startTime,
                                                          this,
                                                          Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnGetCertificateStatusRequest));
                }

                #endregion


                OCPPv2_1.CSMS.GetCertificateStatusResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomGetCertificateStatusRequestSerializer,
                            parentNetworkingNode.CustomOCSPRequestDataSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.GetCertificateStatusResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.GetCertificateStatus(Request)

                                    : new OCPPv2_1.CSMS.GetCertificateStatusResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomGetCertificateStatusResponseSerializer,
                        parentNetworkingNode.CustomStatusInfoSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnGetCertificateStatusResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnGetCertificateStatusResponse?.Invoke(endTime,
                                                           this,
                                                           Request,
                                                           response,
                                                           endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnGetCertificateStatusResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region GetCRL                            (Request)

            /// <summary>
            /// Get a certificate revocation list from CSMS for the specified certificate.
            /// </summary>
            /// 
            /// <param name="GetCRLRequestId">The identification of this request.</param>
            /// <param name="CertificateHashData">Certificate hash data.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.GetCRLResponse>
                GetCRL(OCPPv2_1.CS.GetCRLRequest Request)

            {

                #region Send OnGetCRLRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnGetCRLRequest?.Invoke(startTime,
                                            this,
                                            Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnGetCRLRequest));
                }

                #endregion


                OCPPv2_1.CSMS.GetCRLResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomGetCRLRequestSerializer,
                            parentNetworkingNode.CustomCertificateHashDataSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.GetCRLResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.GetCRL(Request)

                                    : new OCPPv2_1.CSMS.GetCRLResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomGetCRLResponseSerializer,
                        parentNetworkingNode.CustomStatusInfoSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnGetCRLResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnGetCRLResponse?.Invoke(endTime,
                                             this,
                                             Request,
                                             response,
                                             endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnGetCRLResponse));
                }

                #endregion

                return response;

            }

            #endregion


            #region ReservationStatusUpdate           (Request)

            /// <summary>
            /// Send a reservation status update.
            /// </summary>
            /// <param name="ReservationId">The unique identification of the transaction to update.</param>
            /// <param name="ReservationUpdateStatus">The updated reservation status.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.ReservationStatusUpdateResponse>
                ReservationStatusUpdate(OCPPv2_1.CS.ReservationStatusUpdateRequest Request)

            {

                #region Send OnReservationStatusUpdateRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnReservationStatusUpdateRequest?.Invoke(startTime,
                                                             this,
                                                             Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnReservationStatusUpdateRequest));
                }

                #endregion


                OCPPv2_1.CSMS.ReservationStatusUpdateResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomReservationStatusUpdateRequestSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.ReservationStatusUpdateResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.ReservationStatusUpdate(Request)

                                    : new OCPPv2_1.CSMS.ReservationStatusUpdateResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomReservationStatusUpdateResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnReservationStatusUpdateResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnReservationStatusUpdateResponse?.Invoke(endTime,
                                                              this,
                                                              Request,
                                                              response,
                                                              endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnReservationStatusUpdateResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region Authorize                         (Request)

            /// <summary>
            /// Authorize the given token.
            /// </summary>
            /// <param name="IdToken">The identifier that needs to be authorized.</param>
            /// <param name="Certificate">An optional X.509 certificated presented by the electric vehicle/user (PEM format).</param>
            /// <param name="ISO15118CertificateHashData">Optional information to verify the electric vehicle/user contract certificate via OCSP.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.AuthorizeResponse>
                Authorize(OCPPv2_1.CS.AuthorizeRequest Request)

            {

                #region Send OnAuthorizeRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnAuthorizeRequest?.Invoke(startTime,
                                               this,
                                               Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnAuthorizeRequest));
                }

                #endregion


                OCPPv2_1.CSMS.AuthorizeResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomAuthorizeRequestSerializer,
                            parentNetworkingNode.CustomIdTokenSerializer,
                            parentNetworkingNode.CustomAdditionalInfoSerializer,
                            parentNetworkingNode.CustomOCSPRequestDataSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.AuthorizeResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.Authorize(Request)

                                    : new OCPPv2_1.CSMS.AuthorizeResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomAuthorizeResponseSerializer,
                        parentNetworkingNode.CustomIdTokenInfoSerializer,
                        parentNetworkingNode.CustomIdTokenSerializer,
                        parentNetworkingNode.CustomAdditionalInfoSerializer,
                        parentNetworkingNode.CustomMessageContentSerializer,
                        parentNetworkingNode.CustomTransactionLimitsSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnAuthorizeResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnAuthorizeResponse?.Invoke(endTime,
                                                this,
                                                Request,
                                                response,
                                                endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnAuthorizeResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region NotifyEVChargingNeeds             (Request)

            /// <summary>
            /// Notify about EV charging needs.
            /// </summary>
            /// <param name="EVSEId">The EVSE and connector to which the EV is connected to.</param>
            /// <param name="ChargingNeeds">The characteristics of the energy delivery required.</param>
            /// <param name="ReceivedTimestamp">An optional timestamp when the EV charging needs had been received, e.g. when the charging station was offline.</param>
            /// <param name="MaxScheduleTuples">The optional maximum number of schedule tuples per schedule the car supports.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.NotifyEVChargingNeedsResponse>
                NotifyEVChargingNeeds(OCPPv2_1.CS.NotifyEVChargingNeedsRequest Request)

            {

                #region Send OnNotifyEVChargingNeedsRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnNotifyEVChargingNeedsRequest?.Invoke(startTime,
                                                           this,
                                                           Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyEVChargingNeedsRequest));
                }

                #endregion


                OCPPv2_1.CSMS.NotifyEVChargingNeedsResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomNotifyEVChargingNeedsRequestSerializer,
                            parentNetworkingNode.CustomChargingNeedsSerializer,
                            parentNetworkingNode.CustomACChargingParametersSerializer,
                            parentNetworkingNode.CustomDCChargingParametersSerializer,
                            parentNetworkingNode.CustomV2XChargingParametersSerializer,
                            parentNetworkingNode.CustomEVEnergyOfferSerializer,
                            parentNetworkingNode.CustomEVPowerScheduleSerializer,
                            parentNetworkingNode.CustomEVPowerScheduleEntrySerializer,
                            parentNetworkingNode.CustomEVAbsolutePriceScheduleSerializer,
                            parentNetworkingNode.CustomEVAbsolutePriceScheduleEntrySerializer,
                            parentNetworkingNode.CustomEVPriceRuleSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.NotifyEVChargingNeedsResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.NotifyEVChargingNeeds(Request)

                                    : new OCPPv2_1.CSMS.NotifyEVChargingNeedsResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomNotifyEVChargingNeedsResponseSerializer,
                        parentNetworkingNode.CustomStatusInfoSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnNotifyEVChargingNeedsResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnNotifyEVChargingNeedsResponse?.Invoke(endTime,
                                                            this,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyEVChargingNeedsResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region TransactionEvent                  (Request)

            /// <summary>
            /// Send a transaction event.
            /// </summary>
            /// <param name="EventType">The type of this transaction event. The first event of a transaction SHALL be of type "started", the last of type "ended". All others should be of type "updated".</param>
            /// <param name="Timestamp">The timestamp at which this transaction event occurred.</param>
            /// <param name="TriggerReason">The reason the charging station sends this message.</param>
            /// <param name="SequenceNumber">This incremental sequence number, helps to determine whether all messages of a transaction have been received.</param>
            /// <param name="TransactionInfo">Transaction related information.</param>
            /// 
            /// <param name="Offline">An optional indication whether this transaction event happened when the charging station was offline.</param>
            /// <param name="NumberOfPhasesUsed">An optional numer of electrical phases used, when the charging station is able to report it.</param>
            /// <param name="CableMaxCurrent">An optional maximum current of the connected cable in amperes.</param>
            /// <param name="ReservationId">An optional unqiue reservation identification of the reservation that terminated as a result of this transaction.</param>
            /// <param name="IdToken">An optional identification token for which a transaction has to be/was started.</param>
            /// <param name="EVSE">An optional indication of the EVSE (and connector) used.</param>
            /// <param name="MeterValues">An optional enumeration of meter values.</param>
            /// <param name="PreconditioningStatus">The optional current status of the battery management system within the EV.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.TransactionEventResponse>
                TransactionEvent(OCPPv2_1.CS.TransactionEventRequest Request)

            {

                #region Send OnTransactionEventRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnTransactionEventRequest?.Invoke(startTime,
                                                      this,
                                                      Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnTransactionEventRequest));
                }

                #endregion


                OCPPv2_1.CSMS.TransactionEventResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomTransactionEventRequestSerializer,
                            parentNetworkingNode.CustomTransactionSerializer,
                            parentNetworkingNode.CustomIdTokenSerializer,
                            parentNetworkingNode.CustomAdditionalInfoSerializer,
                            parentNetworkingNode.CustomEVSESerializer,
                            parentNetworkingNode.CustomMeterValueSerializer,
                            parentNetworkingNode.CustomSampledValueSerializer,
                            parentNetworkingNode.CustomSignedMeterValueSerializer,
                            parentNetworkingNode.CustomUnitsOfMeasureSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.TransactionEventResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.TransactionEvent(Request)

                                    : new OCPPv2_1.CSMS.TransactionEventResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomTransactionEventResponseSerializer,
                        parentNetworkingNode.CustomIdTokenInfoSerializer,
                        parentNetworkingNode.CustomIdTokenSerializer,
                        parentNetworkingNode.CustomAdditionalInfoSerializer,
                        parentNetworkingNode.CustomMessageContentSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnTransactionEventResponse event

                var endTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

                try
                {

                    OnTransactionEventResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnTransactionEventResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region StatusNotification                (Request)

            /// <summary>
            /// Send a status notification for the given connector.
            /// </summary>
            /// <param name="EVSEId">The identification of the EVSE to which the connector belongs for which the the status is reported.</param>
            /// <param name="ConnectorId">The identification of the connector within the EVSE for which the status is reported.</param>
            /// <param name="Timestamp">The time for which the status is reported.</param>
            /// <param name="Status">The current status of the connector.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.StatusNotificationResponse>
                StatusNotification(OCPPv2_1.CS.StatusNotificationRequest Request)

            {

                #region Send OnStatusNotificationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnStatusNotificationRequest?.Invoke(startTime,
                                                        this,
                                                        Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnStatusNotificationRequest));
                }

                #endregion


                OCPPv2_1.CSMS.StatusNotificationResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomStatusNotificationRequestSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.StatusNotificationResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.StatusNotification(Request)

                                    : new OCPPv2_1.CSMS.StatusNotificationResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomStatusNotificationResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnStatusNotificationResponse event

                var endTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

                try
                {

                    OnStatusNotificationResponse?.Invoke(endTime,
                                                         this,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnStatusNotificationResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region MeterValues                       (Request)

            /// <summary>
            /// Send a meter values for the given connector.
            /// </summary>
            /// <param name="EVSEId">The EVSE identification at the charging station.</param>
            /// <param name="MeterValues">The sampled meter values with timestamps.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.MeterValuesResponse>
                MeterValues(OCPPv2_1.CS.MeterValuesRequest Request)

            {

                #region Send OnMeterValuesRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnMeterValuesRequest?.Invoke(startTime,
                                                 this,
                                                 Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnMeterValuesRequest));
                }

                #endregion


                OCPPv2_1.CSMS.MeterValuesResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomMeterValuesRequestSerializer,
                            parentNetworkingNode.CustomMeterValueSerializer,
                            parentNetworkingNode.CustomSampledValueSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.MeterValuesResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.MeterValues(Request)

                                    : new OCPPv2_1.CSMS.MeterValuesResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomMeterValuesResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnMeterValuesResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnMeterValuesResponse?.Invoke(endTime,
                                                  this,
                                                  Request,
                                                  response,
                                                  endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnMeterValuesResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region NotifyChargingLimit               (Request)

            /// <summary>
            /// Notify about a charging limit.
            /// </summary>
            /// <param name="ChargingLimit">The charging limit, its source and whether it is grid critical.</param>
            /// <param name="ChargingSchedules">Limits for the available power or current over time, as set by the external source.</param>
            /// <param name="EVSEId">An optional EVSE identification, when the charging schedule contained in this notification applies to an EVSE.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.NotifyChargingLimitResponse>
                NotifyChargingLimit(OCPPv2_1.CS.NotifyChargingLimitRequest Request)

            {

                #region Send OnNotifyChargingLimitRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnNotifyChargingLimitRequest?.Invoke(startTime,
                                                         this,
                                                         Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyChargingLimitRequest));
                }

                #endregion


                OCPPv2_1.CSMS.NotifyChargingLimitResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(

                            parentNetworkingNode.CustomNotifyChargingLimitRequestSerializer,
                            parentNetworkingNode.CustomChargingScheduleSerializer,
                            parentNetworkingNode.CustomLimitBeyondSoCSerializer,
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

                    response  = new OCPPv2_1.CSMS.NotifyChargingLimitResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.NotifyChargingLimit(Request)

                                    : new OCPPv2_1.CSMS.NotifyChargingLimitResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomNotifyChargingLimitResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnNotifyChargingLimitResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnNotifyChargingLimitResponse?.Invoke(endTime,
                                                          this,
                                                          Request,
                                                          response,
                                                          endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyChargingLimitResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region ClearedChargingLimit              (Request)

            /// <summary>
            /// Send a heartbeat.
            /// </summary>
            /// <param name="ChargingLimitSource">A source of the charging limit.</param>
            /// <param name="EVSEId">An optional EVSE identification.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.ClearedChargingLimitResponse>
                ClearedChargingLimit(OCPPv2_1.CS.ClearedChargingLimitRequest Request)

            {

                #region Send OnClearedChargingLimitRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnClearedChargingLimitRequest?.Invoke(startTime,
                                                          this,
                                                          Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnClearedChargingLimitRequest));
                }

                #endregion


                OCPPv2_1.CSMS.ClearedChargingLimitResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomClearedChargingLimitRequestSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.ClearedChargingLimitResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.ClearedChargingLimit(Request)

                                    : new OCPPv2_1.CSMS.ClearedChargingLimitResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomClearedChargingLimitResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnClearedChargingLimitResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnClearedChargingLimitResponse?.Invoke(endTime,
                                                           this,
                                                           Request,
                                                           response,
                                                           endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnClearedChargingLimitResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region ReportChargingProfiles            (Request)

            /// <summary>
            /// Report about all charging profiles.
            /// </summary>
            /// <param name="ReportChargingProfilesRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting ReportChargingProfilesRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
            /// <param name="ChargingLimitSource">The source that has installed this charging profile.</param>
            /// <param name="EVSEId">The evse to which the charging profile applies. If evseId = 0, the message contains an overall limit for the charging station.</param>
            /// <param name="ChargingProfiles">The enumeration of charging profiles.</param>
            /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the charging profiles follows. Default value when omitted is false.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.ReportChargingProfilesResponse>
                ReportChargingProfiles(OCPPv2_1.CS.ReportChargingProfilesRequest Request)

            {

                #region Send OnReportChargingProfilesRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnReportChargingProfilesRequest?.Invoke(startTime,
                                                            this,
                                                            Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnReportChargingProfilesRequest));
                }

                #endregion


                OCPPv2_1.CSMS.ReportChargingProfilesResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(

                            parentNetworkingNode.CustomReportChargingProfilesRequestSerializer,
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

                    response  = new OCPPv2_1.CSMS.ReportChargingProfilesResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.ReportChargingProfiles(Request)

                                    : new OCPPv2_1.CSMS.ReportChargingProfilesResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomReportChargingProfilesResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnReportChargingProfilesResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnReportChargingProfilesResponse?.Invoke(endTime,
                                                             this,
                                                             Request,
                                                             response,
                                                             endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnReportChargingProfilesResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region NotifyEVChargingSchedule          (Request)

            /// <summary>
            /// Notify about an EV charging schedule.
            /// </summary>
            /// <param name="NotifyEVChargingScheduleRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting NotifyEVChargingScheduleRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
            /// <param name="TimeBase">The charging periods contained within the charging schedule are relative to this time base.</param>
            /// <param name="EVSEId">The charging schedule applies to this EVSE.</param>
            /// <param name="ChargingSchedule">Planned energy consumption of the EV over time. Always relative to the time base.</param>
            /// <param name="SelectedScheduleTupleId">The optional identification of the selected charging schedule from the provided charging profile.</param>
            /// <param name="PowerToleranceAcceptance">True when power tolerance is accepted.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.NotifyEVChargingScheduleResponse>
                NotifyEVChargingSchedule(OCPPv2_1.CS.NotifyEVChargingScheduleRequest Request)

            {

                #region Send OnNotifyEVChargingScheduleRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnNotifyEVChargingScheduleRequest?.Invoke(startTime,
                                                              this,
                                                              Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyEVChargingScheduleRequest));
                }

                #endregion


                OCPPv2_1.CSMS.NotifyEVChargingScheduleResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(

                            parentNetworkingNode.CustomNotifyEVChargingScheduleRequestSerializer,
                            parentNetworkingNode.CustomChargingScheduleSerializer,
                            parentNetworkingNode.CustomLimitBeyondSoCSerializer,
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

                    response  = new OCPPv2_1.CSMS.NotifyEVChargingScheduleResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.NotifyEVChargingSchedule(Request)

                                    : new OCPPv2_1.CSMS.NotifyEVChargingScheduleResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomNotifyEVChargingScheduleResponseSerializer,
                        parentNetworkingNode.CustomStatusInfoSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnNotifyEVChargingScheduleResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnNotifyEVChargingScheduleResponse?.Invoke(endTime,
                                                               this,
                                                               Request,
                                                               response,
                                                               endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyEVChargingScheduleResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region NotifyPriorityCharging            (Request)

            /// <summary>
            /// Notify about priority charging.
            /// </summary>
            /// <param name="NotifyPriorityChargingRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting NotifyPriorityChargingRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
            /// <param name="TransactionId">The transaction for which priority charging is requested.</param>
            /// <param name="Activated">True, when priority charging was activated, or false, when it has stopped using the priority charging profile.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.NotifyPriorityChargingResponse>
                NotifyPriorityCharging(OCPPv2_1.CS.NotifyPriorityChargingRequest Request)

            {

                #region Send OnNotifyPriorityChargingRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnNotifyPriorityChargingRequest?.Invoke(startTime,
                                                            this,
                                                            Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyPriorityChargingRequest));
                }

                #endregion


                OCPPv2_1.CSMS.NotifyPriorityChargingResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomNotifyPriorityChargingRequestSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.NotifyPriorityChargingResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.NotifyPriorityCharging(Request)

                                    : new OCPPv2_1.CSMS.NotifyPriorityChargingResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomNotifyPriorityChargingResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnNotifyPriorityChargingResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnNotifyPriorityChargingResponse?.Invoke(endTime,
                                                             this,
                                                             Request,
                                                             response,
                                                             endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyPriorityChargingResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region PullDynamicScheduleUpdate         (Request)

            /// <summary>
            /// Report about all charging profiles.
            /// </summary>
            /// <param name="PullDynamicScheduleUpdateRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting PullDynamicScheduleUpdateRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
            /// <param name="ChargingProfileId">The identification of the charging profile for which an update is requested.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse>
                PullDynamicScheduleUpdate(OCPPv2_1.CS.PullDynamicScheduleUpdateRequest Request)

            {

                #region Send OnPullDynamicScheduleUpdateRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnPullDynamicScheduleUpdateRequest?.Invoke(startTime,
                                                               this,
                                                               Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnPullDynamicScheduleUpdateRequest));
                }

                #endregion


                OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomPullDynamicScheduleUpdateRequestSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.PullDynamicScheduleUpdate(Request)

                                    : new OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomPullDynamicScheduleUpdateResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnPullDynamicScheduleUpdateResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnPullDynamicScheduleUpdateResponse?.Invoke(endTime,
                                                                this,
                                                                Request,
                                                                response,
                                                                endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnPullDynamicScheduleUpdateResponse));
                }

                #endregion

                return response;

            }

            #endregion


            #region NotifyDisplayMessages             (Request)

            /// <summary>
            /// NotifyDisplayMessages the given token.
            /// </summary>
            /// <param name="NotifyDisplayMessagesRequestId">The unique identification of the notify display messages request.</param>
            /// <param name="MessageInfos">The requested display messages as configured in the charging station.</param>
            /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyDisplayMessagesRequest message. Default value when omitted is false.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.NotifyDisplayMessagesResponse>
                NotifyDisplayMessages(OCPPv2_1.CS.NotifyDisplayMessagesRequest Request)

            {

                #region Send OnNotifyDisplayMessagesRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnNotifyDisplayMessagesRequest?.Invoke(startTime,
                                                           this,
                                                           Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyDisplayMessagesRequest));
                }

                #endregion


                OCPPv2_1.CSMS.NotifyDisplayMessagesResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomNotifyDisplayMessagesRequestSerializer,
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

                    response  = new OCPPv2_1.CSMS.NotifyDisplayMessagesResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.NotifyDisplayMessages(Request)

                                    : new OCPPv2_1.CSMS.NotifyDisplayMessagesResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomNotifyDisplayMessagesResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnNotifyDisplayMessagesResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnNotifyDisplayMessagesResponse?.Invoke(endTime,
                                                            this,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyDisplayMessagesResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #region NotifyCustomerInformation         (Request)

            /// <summary>
            /// NotifyCustomerInformation the given token.
            /// </summary>
            /// <param name="NotifyCustomerInformationRequestId">The unique identification of the notify customer information request.</param>
            /// <param name="Data">The requested data or a part of the requested data. No format specified in which the data is returned.</param>
            /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
            /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
            /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
            /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
            /// 
            /// <param name="RequestId">An optional request identification.</param>
            /// <param name="RequestTimestamp">An optional request timestamp.</param>
            /// <param name="RequestTimeout">An optional timeout for this request.</param>
            /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
            /// <param name="CancellationToken">An optional token to cancel this request.</param>
            public async Task<OCPPv2_1.CSMS.NotifyCustomerInformationResponse>
                NotifyCustomerInformation(OCPPv2_1.CS.NotifyCustomerInformationRequest Request)

            {

                #region Send OnNotifyCustomerInformationRequest event

                var startTime = Timestamp.Now;

                try
                {

                    OnNotifyCustomerInformationRequest?.Invoke(startTime,
                                                               this,
                                                               Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyCustomerInformationRequest));
                }

                #endregion


                OCPPv2_1.CSMS.NotifyCustomerInformationResponse? response = null;

                if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                        Request,
                        Request.ToJSON(
                            parentNetworkingNode.CustomNotifyCustomerInformationRequestSerializer,
                            parentNetworkingNode.CustomSignatureSerializer,
                            parentNetworkingNode.CustomCustomDataSerializer
                        ),
                        out var errorResponse
                    ))
                {

                    response  = new OCPPv2_1.CSMS.NotifyCustomerInformationResponse(
                                    Request,
                                    Result.SignatureError(errorResponse)
                                );

                }

                // ToDo: Currently hardcoded CSMS lookup!
                else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
                {

                    response  = parentNetworkingNode.AsCS.CSClient is not null

                                    ? await parentNetworkingNode.AsCS.CSClient.NotifyCustomerInformation(Request)

                                    : new OCPPv2_1.CSMS.NotifyCustomerInformationResponse(
                                          Request,
                                          Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                      );

                }

                else
                {
                    // ...
                }


                parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                    response,
                    response.ToJSON(
                        parentNetworkingNode.CustomNotifyCustomerInformationResponseSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out errorResponse
                );


                #region Send OnNotifyCustomerInformationResponse event

                var endTime = Timestamp.Now;

                try
                {

                    OnNotifyCustomerInformationResponse?.Invoke(endTime,
                                                                this,
                                                                Request,
                                                                response,
                                                                endTime - startTime);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyCustomerInformationResponse));
                }

                #endregion

                return response;

            }

            #endregion

            #endregion

            #region Outgoing Messages: Networking Node -> Charging Station

            

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
