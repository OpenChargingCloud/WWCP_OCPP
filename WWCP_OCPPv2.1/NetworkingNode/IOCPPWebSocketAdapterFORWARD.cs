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
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region CS

    #region BinaryDataStreamsExtensions

    #endregion

    #region Certificates

    #region OnGet15118EVCertificateRequestFilter(ed)Delegate

    /// <summary>
    /// A Get15118EVCertificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<Get15118EVCertificateRequest, Get15118EVCertificateResponse>>

        OnGet15118EVCertificateRequestFilterDelegate(DateTime                       Timestamp,
                                                     IEventSender                   Sender,
                                                     IWebSocketConnection           Connection,
                                                     Get15118EVCertificateRequest   Request,
                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A filtered Get15118EVCertificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGet15118EVCertificateRequestFilteredDelegate(DateTime                                                                          Timestamp,
                                                       IEventSender                                                                      Sender,
                                                       IWebSocketConnection                                                              Connection,
                                                       Get15118EVCertificateRequest                                                      Request,
                                                       ForwardingDecision<Get15118EVCertificateRequest, Get15118EVCertificateResponse>   ForwardingDecision);

    #endregion

    #region OnGetCertificateStatusRequestFilter(ed)Delegate

    /// <summary>
    /// A GetCertificateStatus request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetCertificateStatusRequest, GetCertificateStatusResponse>>

        OnGetCertificateStatusRequestFilterDelegate(DateTime                      Timestamp,
                                                    IEventSender                  Sender,
                                                    IWebSocketConnection          Connection,
                                                    GetCertificateStatusRequest   Request,
                                                    CancellationToken             CancellationToken);


    /// <summary>
    /// A filtered GetCertificateStatus request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetCertificateStatusRequestFilteredDelegate(DateTime                                                                        Timestamp,
                                                      IEventSender                                                                    Sender,
                                                      IWebSocketConnection                                                            Connection,
                                                      GetCertificateStatusRequest                                                     Request,
                                                      ForwardingDecision<GetCertificateStatusRequest, GetCertificateStatusResponse>   ForwardingDecision);

    #endregion

    #region OnGetCRLRequestFilter(ed)Delegate

    /// <summary>
    /// A GetCRL request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetCRLRequest, GetCRLResponse>>

        OnGetCRLRequestFilterDelegate(DateTime               Timestamp,
                                      IEventSender           Sender,
                                      IWebSocketConnection   Connection,
                                      GetCRLRequest          Request,
                                      CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered GetCRL request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetCRLRequestFilteredDelegate(DateTime                                            Timestamp,
                                        IEventSender                                        Sender,
                                        IWebSocketConnection                                Connection,
                                        GetCRLRequest                                       Request,
                                        ForwardingDecision<GetCRLRequest, GetCRLResponse>   ForwardingDecision);

    #endregion

    #region OnSignCertificateRequestFilter(ed)Delegate

    /// <summary>
    /// A SignCertificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SignCertificateRequest, SignCertificateResponse>>

        OnSignCertificateRequestFilterDelegate(DateTime                 Timestamp,
                                               IEventSender             Sender,
                                               IWebSocketConnection     Connection,
                                               SignCertificateRequest   Request,
                                               CancellationToken        CancellationToken);


    /// <summary>
    /// A filtered SignCertificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnSignCertificateRequestFilteredDelegate(DateTime                                                              Timestamp,
                                                 IEventSender                                                          Sender,
                                                 IWebSocketConnection                                                  Connection,
                                                 SignCertificateRequest                                                Request,
                                                 ForwardingDecision<SignCertificateRequest, SignCertificateResponse>   ForwardingDecision);

    #endregion

    #endregion

    #region Charging

    #region OnAuthorizeRequestFilter(ed)Delegate

    /// <summary>
    /// A Authorize request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<AuthorizeRequest, AuthorizeResponse>>

        OnAuthorizeRequestFilterDelegate(DateTime               Timestamp,
                                         IEventSender           Sender,
                                         IWebSocketConnection   Connection,
                                         AuthorizeRequest       Request,
                                         CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered Authorize request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnAuthorizeRequestFilteredDelegate(DateTime                                                  Timestamp,
                                           IEventSender                                              Sender,
                                           IWebSocketConnection                                      Connection,
                                           AuthorizeRequest                                          Request,
                                           ForwardingDecision<AuthorizeRequest, AuthorizeResponse>   ForwardingDecision);

    #endregion

    #region OnClearedChargingLimitRequestFilter(ed)Delegate

    /// <summary>
    /// A ClearedChargingLimit request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ClearedChargingLimitRequest, ClearedChargingLimitResponse>>

        OnClearedChargingLimitRequestFilterDelegate(DateTime                      Timestamp,
                                                    IEventSender                  Sender,
                                                    IWebSocketConnection          Connection,
                                                    ClearedChargingLimitRequest   Request,
                                                    CancellationToken             CancellationToken);


    /// <summary>
    /// A filtered ClearedChargingLimit request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnClearedChargingLimitRequestFilteredDelegate(DateTime                                                                        Timestamp,
                                                      IEventSender                                                                    Sender,
                                                      IWebSocketConnection                                                            Connection,
                                                      ClearedChargingLimitRequest                                                     Request,
                                                      ForwardingDecision<ClearedChargingLimitRequest, ClearedChargingLimitResponse>   ForwardingDecision);

    #endregion

    #region OnMeterValuesRequestFilter(ed)Delegate

    /// <summary>
    /// A MeterValues request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<MeterValuesRequest, MeterValuesResponse>>

        OnMeterValuesRequestFilterDelegate(DateTime               Timestamp,
                                           IEventSender           Sender,
                                           IWebSocketConnection   Connection,
                                           MeterValuesRequest     Request,
                                           CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered MeterValues request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnMeterValuesRequestFilteredDelegate(DateTime                                                      Timestamp,
                                             IEventSender                                                  Sender,
                                             IWebSocketConnection                                          Connection,
                                             MeterValuesRequest                                            Request,
                                             ForwardingDecision<MeterValuesRequest, MeterValuesResponse>   ForwardingDecision);

    #endregion

    #region OnNotifyChargingLimitRequestFilter(ed)Delegate

    /// <summary>
    /// A NotifyChargingLimit request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<NotifyChargingLimitRequest, NotifyChargingLimitResponse>>

        OnNotifyChargingLimitRequestFilterDelegate(DateTime                     Timestamp,
                                                   IEventSender                 Sender,
                                                   IWebSocketConnection         Connection,
                                                   NotifyChargingLimitRequest   Request,
                                                   CancellationToken            CancellationToken);


    /// <summary>
    /// A filtered NotifyChargingLimit request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifyChargingLimitRequestFilteredDelegate(DateTime                                                                      Timestamp,
                                                     IEventSender                                                                  Sender,
                                                     IWebSocketConnection                                                          Connection,
                                                     NotifyChargingLimitRequest                                                    Request,
                                                     ForwardingDecision<NotifyChargingLimitRequest, NotifyChargingLimitResponse>   ForwardingDecision);

    #endregion

    #region OnNotifyEVChargingNeedsRequestFilter(ed)Delegate

    /// <summary>
    /// A NotifyEVChargingNeeds request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<NotifyEVChargingNeedsRequest, NotifyEVChargingNeedsResponse>>

        OnNotifyEVChargingNeedsRequestFilterDelegate(DateTime                       Timestamp,
                                                     IEventSender                   Sender,
                                                     IWebSocketConnection           Connection,
                                                     NotifyEVChargingNeedsRequest   Request,
                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A filtered NotifyEVChargingNeeds request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifyEVChargingNeedsRequestFilteredDelegate(DateTime                                                                          Timestamp,
                                                       IEventSender                                                                      Sender,
                                                       IWebSocketConnection                                                              Connection,
                                                       NotifyEVChargingNeedsRequest                                                      Request,
                                                       ForwardingDecision<NotifyEVChargingNeedsRequest, NotifyEVChargingNeedsResponse>   ForwardingDecision);

    #endregion

    #region OnNotifyEVChargingScheduleRequestFilter(ed)Delegate

    /// <summary>
    /// A NotifyEVChargingSchedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<NotifyEVChargingScheduleRequest, NotifyEVChargingScheduleResponse>>

        OnNotifyEVChargingScheduleRequestFilterDelegate(DateTime                          Timestamp,
                                                        IEventSender                      Sender,
                                                        IWebSocketConnection              Connection,
                                                        NotifyEVChargingScheduleRequest   Request,
                                                        CancellationToken                 CancellationToken);


    /// <summary>
    /// A filtered NotifyEVChargingSchedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifyEVChargingScheduleRequestFilteredDelegate(DateTime                                                                                Timestamp,
                                                          IEventSender                                                                            Sender,
                                                          IWebSocketConnection                                                                    Connection,
                                                          NotifyEVChargingScheduleRequest                                                         Request,
                                                          ForwardingDecision<NotifyEVChargingScheduleRequest, NotifyEVChargingScheduleResponse>   ForwardingDecision);

    #endregion

    #region OnNotifyPriorityChargingRequestFilter(ed)Delegate

    /// <summary>
    /// A NotifyPriorityCharging request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<NotifyPriorityChargingRequest, NotifyPriorityChargingResponse>>

        OnNotifyPriorityChargingRequestFilterDelegate(DateTime                        Timestamp,
                                                      IEventSender                    Sender,
                                                      IWebSocketConnection            Connection,
                                                      NotifyPriorityChargingRequest   Request,
                                                      CancellationToken               CancellationToken);


    /// <summary>
    /// A filtered NotifyPriorityCharging request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifyPriorityChargingRequestFilteredDelegate(DateTime                                                                            Timestamp,
                                                        IEventSender                                                                        Sender,
                                                        IWebSocketConnection                                                                Connection,
                                                        NotifyPriorityChargingRequest                                                       Request,
                                                        ForwardingDecision<NotifyPriorityChargingRequest, NotifyPriorityChargingResponse>   ForwardingDecision);

    #endregion

    #region OnPullDynamicScheduleUpdateRequestFilter(ed)Delegate

    /// <summary>
    /// A PullDynamicScheduleUpdate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<PullDynamicScheduleUpdateRequest, PullDynamicScheduleUpdateResponse>>

        OnPullDynamicScheduleUpdateRequestFilterDelegate(DateTime                           Timestamp,
                                                         IEventSender                       Sender,
                                                         IWebSocketConnection               Connection,
                                                         PullDynamicScheduleUpdateRequest   Request,
                                                         CancellationToken                  CancellationToken);


    /// <summary>
    /// A filtered PullDynamicScheduleUpdate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnPullDynamicScheduleUpdateRequestFilteredDelegate(DateTime                                                                                  Timestamp,
                                                           IEventSender                                                                              Sender,
                                                           IWebSocketConnection                                                                      Connection,
                                                           PullDynamicScheduleUpdateRequest                                                          Request,
                                                           ForwardingDecision<PullDynamicScheduleUpdateRequest, PullDynamicScheduleUpdateResponse>   ForwardingDecision);

    #endregion

    #region OnReportChargingProfilesRequestFilter(ed)Delegate

    /// <summary>
    /// A ReportChargingProfiles request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ReportChargingProfilesRequest, ReportChargingProfilesResponse>>

        OnReportChargingProfilesRequestFilterDelegate(DateTime                        Timestamp,
                                                      IEventSender                    Sender,
                                                      IWebSocketConnection            Connection,
                                                      ReportChargingProfilesRequest   Request,
                                                      CancellationToken               CancellationToken);


    /// <summary>
    /// A filtered ReportChargingProfiles request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnReportChargingProfilesRequestFilteredDelegate(DateTime                                                                            Timestamp,
                                                        IEventSender                                                                        Sender,
                                                        IWebSocketConnection                                                                Connection,
                                                        ReportChargingProfilesRequest                                                       Request,
                                                        ForwardingDecision<ReportChargingProfilesRequest, ReportChargingProfilesResponse>   ForwardingDecision);

    #endregion

    #region OnReservationStatusUpdateRequestFilter(ed)Delegate

    /// <summary>
    /// A ReservationStatusUpdate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ReservationStatusUpdateRequest, ReservationStatusUpdateResponse>>

        OnReservationStatusUpdateRequestFilterDelegate(DateTime                         Timestamp,
                                                       IEventSender                     Sender,
                                                       IWebSocketConnection             Connection,
                                                       ReservationStatusUpdateRequest   Request,
                                                       CancellationToken                CancellationToken);


    /// <summary>
    /// A filtered ReservationStatusUpdate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnReservationStatusUpdateRequestFilteredDelegate(DateTime                                                                              Timestamp,
                                                         IEventSender                                                                          Sender,
                                                         IWebSocketConnection                                                                  Connection,
                                                         ReservationStatusUpdateRequest                                                        Request,
                                                         ForwardingDecision<ReservationStatusUpdateRequest, ReservationStatusUpdateResponse>   ForwardingDecision);

    #endregion

    #region OnStatusNotificationRequestFilter(ed)Delegate

    /// <summary>
    /// A StatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<StatusNotificationRequest, StatusNotificationResponse>>

        OnStatusNotificationRequestFilterDelegate(DateTime                    Timestamp,
                                                  IEventSender                Sender,
                                                  IWebSocketConnection        Connection,
                                                  StatusNotificationRequest   Request,
                                                  CancellationToken           CancellationToken);


    /// <summary>
    /// A filtered StatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnStatusNotificationRequestFilteredDelegate(DateTime                                                                    Timestamp,
                                                    IEventSender                                                                Sender,
                                                    IWebSocketConnection                                                        Connection,
                                                    StatusNotificationRequest                                                   Request,
                                                    ForwardingDecision<StatusNotificationRequest, StatusNotificationResponse>   ForwardingDecision);

    #endregion

    #region OnTransactionEventRequestFilter(ed)Delegate

    /// <summary>
    /// A TransactionEvent request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<TransactionEventRequest, TransactionEventResponse>>

        OnTransactionEventRequestFilterDelegate(DateTime                  Timestamp,
                                                IEventSender              Sender,
                                                IWebSocketConnection      Connection,
                                                TransactionEventRequest   Request,
                                                CancellationToken         CancellationToken);


    /// <summary>
    /// A filtered TransactionEvent request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnTransactionEventRequestFilteredDelegate(DateTime                                                                Timestamp,
                                                  IEventSender                                                            Sender,
                                                  IWebSocketConnection                                                    Connection,
                                                  TransactionEventRequest                                                 Request,
                                                  ForwardingDecision<TransactionEventRequest, TransactionEventResponse>   ForwardingDecision);

    #endregion

    #endregion

    #region Customer

    #region OnNotifyCustomerInformationRequestFilter(ed)Delegate

    /// <summary>
    /// A NotifyCustomerInformation request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<NotifyCustomerInformationRequest, NotifyCustomerInformationResponse>>

        OnNotifyCustomerInformationRequestFilterDelegate(DateTime                           Timestamp,
                                                         IEventSender                       Sender,
                                                         IWebSocketConnection               Connection,
                                                         NotifyCustomerInformationRequest   Request,
                                                         CancellationToken                  CancellationToken);


    /// <summary>
    /// A filtered NotifyCustomerInformation request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifyCustomerInformationRequestFilteredDelegate(DateTime                                                                                  Timestamp,
                                                           IEventSender                                                                              Sender,
                                                           IWebSocketConnection                                                                      Connection,
                                                           NotifyCustomerInformationRequest                                                          Request,
                                                           ForwardingDecision<NotifyCustomerInformationRequest, NotifyCustomerInformationResponse>   ForwardingDecision);

    #endregion

    #region OnNotifyDisplayMessagesRequestFilter(ed)Delegate

    /// <summary>
    /// A NotifyDisplayMessages request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<NotifyDisplayMessagesRequest, NotifyDisplayMessagesResponse>>

        OnNotifyDisplayMessagesRequestFilterDelegate(DateTime                       Timestamp,
                                                     IEventSender                   Sender,
                                                     IWebSocketConnection           Connection,
                                                     NotifyDisplayMessagesRequest   Request,
                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A filtered NotifyDisplayMessages request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifyDisplayMessagesRequestFilteredDelegate(DateTime                                                                          Timestamp,
                                                       IEventSender                                                                      Sender,
                                                       IWebSocketConnection                                                              Connection,
                                                       NotifyDisplayMessagesRequest                                                      Request,
                                                       ForwardingDecision<NotifyDisplayMessagesRequest, NotifyDisplayMessagesResponse>   ForwardingDecision);

    #endregion

    #endregion

    #region DeviceModel

    #region OnLogStatusNotificationRequestFilter(ed)Delegate

    /// <summary>
    /// A LogStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<LogStatusNotificationRequest, LogStatusNotificationResponse>>

        OnLogStatusNotificationRequestFilterDelegate(DateTime                       Timestamp,
                                                     IEventSender                   Sender,
                                                     IWebSocketConnection           Connection,
                                                     LogStatusNotificationRequest   Request,
                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A filtered LogStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnLogStatusNotificationRequestFilteredDelegate(DateTime                                                                          Timestamp,
                                                       IEventSender                                                                      Sender,
                                                       IWebSocketConnection                                                              Connection,
                                                       LogStatusNotificationRequest                                                      Request,
                                                       ForwardingDecision<LogStatusNotificationRequest, LogStatusNotificationResponse>   ForwardingDecision);

    #endregion

    #region OnNotifyEventRequestFilter(ed)Delegate

    /// <summary>
    /// A NotifyEvent request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<NotifyEventRequest, NotifyEventResponse>>

        OnNotifyEventRequestFilterDelegate(DateTime               Timestamp,
                                           IEventSender           Sender,
                                           IWebSocketConnection   Connection,
                                           NotifyEventRequest     Request,
                                           CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered NotifyEvent request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifyEventRequestFilteredDelegate(DateTime                                                      Timestamp,
                                             IEventSender                                                  Sender,
                                             IWebSocketConnection                                          Connection,
                                             NotifyEventRequest                                            Request,
                                             ForwardingDecision<NotifyEventRequest, NotifyEventResponse>   ForwardingDecision);

    #endregion

    #region OnNotifyMonitoringReportRequestFilter(ed)Delegate

    /// <summary>
    /// A NotifyMonitoringReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<NotifyMonitoringReportRequest, NotifyMonitoringReportResponse>>

        OnNotifyMonitoringReportRequestFilterDelegate(DateTime                        Timestamp,
                                                      IEventSender                    Sender,
                                                      IWebSocketConnection            Connection,
                                                      NotifyMonitoringReportRequest   Request,
                                                      CancellationToken               CancellationToken);


    /// <summary>
    /// A filtered NotifyMonitoringReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifyMonitoringReportRequestFilteredDelegate(DateTime                                                                            Timestamp,
                                                        IEventSender                                                                        Sender,
                                                        IWebSocketConnection                                                                Connection,
                                                        NotifyMonitoringReportRequest                                                       Request,
                                                        ForwardingDecision<NotifyMonitoringReportRequest, NotifyMonitoringReportResponse>   ForwardingDecision);

    #endregion

    #region OnNotifyReportRequestFilter(ed)Delegate

    /// <summary>
    /// A NotifyReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<NotifyReportRequest, NotifyReportResponse>>

        OnNotifyReportRequestFilterDelegate(DateTime               Timestamp,
                                            IEventSender           Sender,
                                            IWebSocketConnection   Connection,
                                            NotifyReportRequest    Request,
                                            CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered NotifyReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifyReportRequestFilteredDelegate(DateTime                                                        Timestamp,
                                              IEventSender                                                    Sender,
                                              IWebSocketConnection                                            Connection,
                                              NotifyReportRequest                                             Request,
                                              ForwardingDecision<NotifyReportRequest, NotifyReportResponse>   ForwardingDecision);

    #endregion

    #region OnSecurityEventNotificationRequestFilter(ed)Delegate

    /// <summary>
    /// A SecurityEventNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SecurityEventNotificationRequest, SecurityEventNotificationResponse>>

        OnSecurityEventNotificationRequestFilterDelegate(DateTime                           Timestamp,
                                                         IEventSender                       Sender,
                                                         IWebSocketConnection               Connection,
                                                         SecurityEventNotificationRequest   Request,
                                                         CancellationToken                  CancellationToken);


    /// <summary>
    /// A filtered SecurityEventNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnSecurityEventNotificationRequestFilteredDelegate(DateTime                                                                                  Timestamp,
                                                           IEventSender                                                                              Sender,
                                                           IWebSocketConnection                                                                      Connection,
                                                           SecurityEventNotificationRequest                                                          Request,
                                                           ForwardingDecision<SecurityEventNotificationRequest, SecurityEventNotificationResponse>   ForwardingDecision);

    #endregion

    #endregion

    #region Firmware

    #region OnBootNotificationRequestFilter(ed)Delegate

    /// <summary>
    /// A BootNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<BootNotificationRequest, BootNotificationResponse>>

        OnBootNotificationRequestFilterDelegate(DateTime                  Timestamp,
                                                IEventSender              Sender,
                                                IWebSocketConnection      Connection,
                                                BootNotificationRequest   Request,
                                                CancellationToken         CancellationToken);


    /// <summary>
    /// A filtered BootNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnBootNotificationRequestFilteredDelegate(DateTime                                                                Timestamp,
                                                  IEventSender                                                            Sender,
                                                  IWebSocketConnection                                                    Connection,
                                                  BootNotificationRequest                                                 Request,
                                                  ForwardingDecision<BootNotificationRequest, BootNotificationResponse>   ForwardingDecision);

    #endregion

    #region OnFirmwareStatusNotificationRequestFilter(ed)Delegate

    /// <summary>
    /// A FirmwareStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<FirmwareStatusNotificationRequest, FirmwareStatusNotificationResponse>>

        OnFirmwareStatusNotificationRequestFilterDelegate(DateTime                            Timestamp,
                                                          IEventSender                        Sender,
                                                          IWebSocketConnection                Connection,
                                                          FirmwareStatusNotificationRequest   Request,
                                                          CancellationToken                   CancellationToken);


    /// <summary>
    /// A filtered FirmwareStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnFirmwareStatusNotificationRequestFilteredDelegate(DateTime                                                                                    Timestamp,
                                                            IEventSender                                                                                Sender,
                                                            IWebSocketConnection                                                                        Connection,
                                                            FirmwareStatusNotificationRequest                                                           Request,
                                                            ForwardingDecision<FirmwareStatusNotificationRequest, FirmwareStatusNotificationResponse>   ForwardingDecision);

    #endregion

    #region OnHeartbeatRequestFilter(ed)Delegate

    /// <summary>
    /// A Heartbeat request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<HeartbeatRequest, HeartbeatResponse>>

        OnHeartbeatRequestFilterDelegate(DateTime               Timestamp,
                                         IEventSender           Sender,
                                         IWebSocketConnection   Connection,
                                         HeartbeatRequest       Request,
                                         CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered Heartbeat request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnHeartbeatRequestFilteredDelegate(DateTime                                                  Timestamp,
                                           IEventSender                                              Sender,
                                           IWebSocketConnection                                      Connection,
                                           HeartbeatRequest                                          Request,
                                           ForwardingDecision<HeartbeatRequest, HeartbeatResponse>   ForwardingDecision);

    #endregion

    #region OnPublishFirmwareStatusNotificationRequestFilter(ed)Delegate

    /// <summary>
    /// A PublishFirmwareStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<PublishFirmwareStatusNotificationRequest, PublishFirmwareStatusNotificationResponse>>

        OnPublishFirmwareStatusNotificationRequestFilterDelegate(DateTime                                   Timestamp,
                                                                 IEventSender                               Sender,
                                                                 IWebSocketConnection                       Connection,
                                                                 PublishFirmwareStatusNotificationRequest   Request,
                                                                 CancellationToken                          CancellationToken);


    /// <summary>
    /// A filtered PublishFirmwareStatusNotification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnPublishFirmwareStatusNotificationRequestFilteredDelegate(DateTime                                                                                                  Timestamp,
                                                                   IEventSender                                                                                              Sender,
                                                                   IWebSocketConnection                                                                                      Connection,
                                                                   PublishFirmwareStatusNotificationRequest                                                                  Request,
                                                                   ForwardingDecision<PublishFirmwareStatusNotificationRequest, PublishFirmwareStatusNotificationResponse>   ForwardingDecision);

    #endregion

    #endregion

    #endregion

    #region CSMS

    #region BinaryDataStreamsExtensions

    #region OnDeleteFileRequestFilter(ed)Delegate

    /// <summary>
    /// A DeleteFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<DeleteFileRequest, DeleteFileResponse>>

        OnDeleteFileRequestFilterDelegate(DateTime               Timestamp,
                                          IEventSender           Sender,
                                          IWebSocketConnection   Connection,
                                          DeleteFileRequest      Request,
                                          CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered DeleteFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
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
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetFileRequest, GetFileResponse>>

        OnGetFileRequestFilterDelegate(DateTime               Timestamp,
                                          IEventSender           Sender,
                                          IWebSocketConnection   Connection,
                                          GetFileRequest      Request,
                                          CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered GetFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
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
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ListDirectoryRequest, ListDirectoryResponse>>

        OnListDirectoryRequestFilterDelegate(DateTime               Timestamp,
                                          IEventSender           Sender,
                                          IWebSocketConnection   Connection,
                                          ListDirectoryRequest      Request,
                                          CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered ListDirectory request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
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
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SendFileRequest, SendFileResponse>>

        OnSendFileRequestFilterDelegate(DateTime               Timestamp,
                                          IEventSender           Sender,
                                          IWebSocketConnection   Connection,
                                          SendFileRequest      Request,
                                          CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered SendFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnSendFileRequestFilteredDelegate(DateTime                                                    Timestamp,
                                            IEventSender                                                Sender,
                                            IWebSocketConnection                                        Connection,
                                            SendFileRequest                                           Request,
                                            ForwardingDecision<SendFileRequest, SendFileResponse>   ForwardingDecision);

    #endregion

    #endregion

    #region Certificates

    #region OnCertificateSignedRequestFilter(ed)Delegate

    /// <summary>
    /// A CertificateSigned request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<CertificateSignedRequest, CertificateSignedResponse>>

        OnCertificateSignedRequestFilterDelegate(DateTime                   Timestamp,
                                                 IEventSender               Sender,
                                                 IWebSocketConnection       Connection,
                                                 CertificateSignedRequest   Request,
                                                 CancellationToken          CancellationToken);


    /// <summary>
    /// A filtered CertificateSigned request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnCertificateSignedRequestFilteredDelegate(DateTime                                                      Timestamp,
                                       IEventSender                                                              Sender,
                                       IWebSocketConnection                                                      Connection,
                                       CertificateSignedRequest                                                  Request,
                                       ForwardingDecision<CertificateSignedRequest, CertificateSignedResponse>   ForwardingDecision);

    #endregion

    #region OnDeleteCertificateRequestFilter(ed)Delegate

    /// <summary>
    /// A DeleteCertificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<DeleteCertificateRequest, DeleteCertificateResponse>>

        OnDeleteCertificateRequestFilterDelegate(DateTime                   Timestamp,
                                                 IEventSender               Sender,
                                                 IWebSocketConnection       Connection,
                                                 DeleteCertificateRequest   Request,
                                                 CancellationToken          CancellationToken);


    /// <summary>
    /// A filtered DeleteCertificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnDeleteCertificateRequestFilteredDelegate(DateTime                                                                  Timestamp,
                                                   IEventSender                                                              Sender,
                                                   IWebSocketConnection                                                      Connection,
                                                   DeleteCertificateRequest                                                  Request,
                                                   ForwardingDecision<DeleteCertificateRequest, DeleteCertificateResponse>   ForwardingDecision);

    #endregion

    #region OnGetInstalledCertificateIdsRequestFilter(ed)Delegate

    /// <summary>
    /// A GetInstalledCertificateIds request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetInstalledCertificateIdsRequest, GetInstalledCertificateIdsResponse>>

        OnGetInstalledCertificateIdsRequestFilterDelegate(DateTime                            Timestamp,
                                                          IEventSender                        Sender,
                                                          IWebSocketConnection                Connection,
                                                          GetInstalledCertificateIdsRequest   Request,
                                                          CancellationToken                   CancellationToken);


    /// <summary>
    /// A filtered GetInstalledCertificateIds request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetInstalledCertificateIdsRequestFilteredDelegate(DateTime                                                                                    Timestamp,
                                                            IEventSender                                                                                Sender,
                                                            IWebSocketConnection                                                                        Connection,
                                                            GetInstalledCertificateIdsRequest                                                           Request,
                                                            ForwardingDecision<GetInstalledCertificateIdsRequest, GetInstalledCertificateIdsResponse>   ForwardingDecision);

    #endregion

    #region OnInstallCertificateRequestFilter(ed)Delegate

    /// <summary>
    /// A InstallCertificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<InstallCertificateRequest, InstallCertificateResponse>>

        OnInstallCertificateRequestFilterDelegate(DateTime                    Timestamp,
                                                  IEventSender                Sender,
                                                  IWebSocketConnection        Connection,
                                                  InstallCertificateRequest   Request,
                                                  CancellationToken           CancellationToken);


    /// <summary>
    /// A filtered InstallCertificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnInstallCertificateRequestFilteredDelegate(DateTime                                                                    Timestamp,
                                                    IEventSender                                                                Sender,
                                                    IWebSocketConnection                                                        Connection,
                                                    InstallCertificateRequest                                                   Request,
                                                    ForwardingDecision<InstallCertificateRequest, InstallCertificateResponse>   ForwardingDecision);

    #endregion

    #region OnNotifyCRLRequestFilter(ed)Delegate

    /// <summary>
    /// A NotifyCRL request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<NotifyCRLRequest, NotifyCRLResponse>>

        OnNotifyCRLRequestFilterDelegate(DateTime               Timestamp,
                                         IEventSender           Sender,
                                         IWebSocketConnection   Connection,
                                         NotifyCRLRequest       Request,
                                         CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered NotifyCRL request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifyCRLRequestFilteredDelegate(DateTime                                                  Timestamp,
                                           IEventSender                                              Sender,
                                           IWebSocketConnection                                      Connection,
                                           NotifyCRLRequest                                          Request,
                                           ForwardingDecision<NotifyCRLRequest, NotifyCRLResponse>   ForwardingDecision);

    #endregion

    #endregion

    #region Charging

    #region OnCancelReservationRequestFilter(ed)Delegate

    /// <summary>
    /// A CancelReservation request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<CancelReservationRequest, CancelReservationResponse>>

        OnCancelReservationRequestFilterDelegate(DateTime                   Timestamp,
                                                 IEventSender               Sender,
                                                 IWebSocketConnection       Connection,
                                                 CancelReservationRequest   Request,
                                                 CancellationToken          CancellationToken);


    /// <summary>
    /// A filtered CancelReservation request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnCancelReservationRequestFilteredDelegate(DateTime                                                                  Timestamp,
                                                   IEventSender                                                              Sender,
                                                   IWebSocketConnection                                                      Connection,
                                                   CancelReservationRequest                                                  Request,
                                                   ForwardingDecision<CancelReservationRequest, CancelReservationResponse>   ForwardingDecision);

    #endregion

    #region OnClearChargingProfileRequestFilter(ed)Delegate

    /// <summary>
    /// A ClearChargingProfile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ClearChargingProfileRequest, ClearChargingProfileResponse>>

        OnClearChargingProfileRequestFilterDelegate(DateTime                      Timestamp,
                                                    IEventSender                  Sender,
                                                    IWebSocketConnection          Connection,
                                                    ClearChargingProfileRequest   Request,
                                                    CancellationToken             CancellationToken);


    /// <summary>
    /// A filtered ClearChargingProfile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnClearChargingProfileRequestFilteredDelegate(DateTime                                                                        Timestamp,
                                                      IEventSender                                                                    Sender,
                                                      IWebSocketConnection                                                            Connection,
                                                      ClearChargingProfileRequest                                                     Request,
                                                      ForwardingDecision<ClearChargingProfileRequest, ClearChargingProfileResponse>   ForwardingDecision);

    #endregion

    #region OnGetChargingProfilesRequestFilter(ed)Delegate

    /// <summary>
    /// A GetChargingProfiles request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetChargingProfilesRequest, GetChargingProfilesResponse>>

        OnGetChargingProfilesRequestFilterDelegate(DateTime                     Timestamp,
                                                   IEventSender                 Sender,
                                                   IWebSocketConnection         Connection,
                                                   GetChargingProfilesRequest   Request,
                                                   CancellationToken            CancellationToken);


    /// <summary>
    /// A filtered GetChargingProfiles request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetChargingProfilesRequestFilteredDelegate(DateTime                                                                      Timestamp,
                                                     IEventSender                                                                  Sender,
                                                     IWebSocketConnection                                                          Connection,
                                                     GetChargingProfilesRequest                                                    Request,
                                                     ForwardingDecision<GetChargingProfilesRequest, GetChargingProfilesResponse>   ForwardingDecision);

    #endregion

    #region OnGetCompositeScheduleRequestFilter(ed)Delegate

    /// <summary>
    /// A GetCompositeSchedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetCompositeScheduleRequest, GetCompositeScheduleResponse>>

        OnGetCompositeScheduleRequestFilterDelegate(DateTime                      Timestamp,
                                                    IEventSender                  Sender,
                                                    IWebSocketConnection          Connection,
                                                    GetCompositeScheduleRequest   Request,
                                                    CancellationToken             CancellationToken);


    /// <summary>
    /// A filtered GetCompositeSchedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetCompositeScheduleRequestFilteredDelegate(DateTime                                                                        Timestamp,
                                                      IEventSender                                                                    Sender,
                                                      IWebSocketConnection                                                            Connection,
                                                      GetCompositeScheduleRequest                                                     Request,
                                                      ForwardingDecision<GetCompositeScheduleRequest, GetCompositeScheduleResponse>   ForwardingDecision);

    #endregion  

    #region OnGetTransactionStatusRequestFilter(ed)Delegate

    /// <summary>
    /// A GetTransactionStatus request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetTransactionStatusRequest, GetTransactionStatusResponse>>

        OnGetTransactionStatusRequestFilterDelegate(DateTime                      Timestamp,
                                                    IEventSender                  Sender,
                                                    IWebSocketConnection          Connection,
                                                    GetTransactionStatusRequest   Request,
                                                    CancellationToken             CancellationToken);


    /// <summary>
    /// A filtered GetTransactionStatus request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetTransactionStatusRequestFilteredDelegate(DateTime                                                                        Timestamp,
                                                      IEventSender                                                                    Sender,
                                                      IWebSocketConnection                                                            Connection,
                                                      GetTransactionStatusRequest                                                     Request,
                                                      ForwardingDecision<GetTransactionStatusRequest, GetTransactionStatusResponse>   ForwardingDecision);

    #endregion

    #region OnNotifyAllowedEnergyTransferRequestFilter(ed)Delegate

    /// <summary>
    /// A NotifyAllowedEnergyTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<NotifyAllowedEnergyTransferRequest, NotifyAllowedEnergyTransferResponse>>

        OnNotifyAllowedEnergyTransferRequestFilterDelegate(DateTime                             Timestamp,
                                                           IEventSender                         Sender,
                                                           IWebSocketConnection                 Connection,
                                                           NotifyAllowedEnergyTransferRequest   Request,
                                                           CancellationToken                    CancellationToken);


    /// <summary>
    /// A filtered NotifyAllowedEnergyTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnNotifyAllowedEnergyTransferRequestFilteredDelegate(DateTime                                                                                      Timestamp,
                                                             IEventSender                                                                                  Sender,
                                                             IWebSocketConnection                                                                          Connection,
                                                             NotifyAllowedEnergyTransferRequest                                                            Request,
                                                             ForwardingDecision<NotifyAllowedEnergyTransferRequest, NotifyAllowedEnergyTransferResponse>   ForwardingDecision);

    #endregion

    #region OnRequestStartTransactionRequestFilter(ed)Delegate

    /// <summary>
    /// A RequestStartTransaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<RequestStartTransactionRequest, RequestStartTransactionResponse>>

        OnRequestStartTransactionRequestFilterDelegate(DateTime                         Timestamp,
                                                       IEventSender                     Sender,
                                                       IWebSocketConnection             Connection,
                                                       RequestStartTransactionRequest   Request,
                                                       CancellationToken                CancellationToken);


    /// <summary>
    /// A filtered RequestStartTransaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnRequestStartTransactionRequestFilteredDelegate(DateTime                                                                              Timestamp,
                                                         IEventSender                                                                          Sender,
                                                         IWebSocketConnection                                                                  Connection,
                                                         RequestStartTransactionRequest                                                        Request,
                                                         ForwardingDecision<RequestStartTransactionRequest, RequestStartTransactionResponse>   ForwardingDecision);

    #endregion

    #region OnRequestStopTransactionRequestFilter(ed)Delegate

    /// <summary>
    /// A RequestStopTransaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<RequestStopTransactionRequest, RequestStopTransactionResponse>>

        OnRequestStopTransactionRequestFilterDelegate(DateTime                        Timestamp,
                                                      IEventSender                    Sender,
                                                      IWebSocketConnection            Connection,
                                                      RequestStopTransactionRequest   Request,
                                                      CancellationToken               CancellationToken);


    /// <summary>
    /// A filtered RequestStopTransaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnRequestStopTransactionRequestFilteredDelegate(DateTime                                                                            Timestamp,
                                                        IEventSender                                                                        Sender,
                                                        IWebSocketConnection                                                                Connection,
                                                        RequestStopTransactionRequest                                                       Request,
                                                        ForwardingDecision<RequestStopTransactionRequest, RequestStopTransactionResponse>   ForwardingDecision);

    #endregion

    #region OnReserveNowRequestFilter(ed)Delegate

    /// <summary>
    /// A ReserveNow request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ReserveNowRequest, ReserveNowResponse>>

        OnReserveNowRequestFilterDelegate(DateTime               Timestamp,
                                          IEventSender           Sender,
                                          IWebSocketConnection   Connection,
                                          ReserveNowRequest      Request,
                                          CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered ReserveNow request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnReserveNowRequestFilteredDelegate(DateTime                                                    Timestamp,
                                            IEventSender                                                Sender,
                                            IWebSocketConnection                                        Connection,
                                            ReserveNowRequest                                           Request,
                                            ForwardingDecision<ReserveNowRequest, ReserveNowResponse>   ForwardingDecision);

    #endregion

    #region OnSetChargingProfileRequestFilter(ed)Delegate

    /// <summary>
    /// A SetChargingProfile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SetChargingProfileRequest, SetChargingProfileResponse>>

        OnSetChargingProfileRequestFilterDelegate(DateTime                    Timestamp,
                                                  IEventSender                Sender,
                                                  IWebSocketConnection        Connection,
                                                  SetChargingProfileRequest   Request,
                                                  CancellationToken           CancellationToken);


    /// <summary>
    /// A filtered SetChargingProfile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnSetChargingProfileRequestFilteredDelegate(DateTime                                                                    Timestamp,
                                                    IEventSender                                                                Sender,
                                                    IWebSocketConnection                                                        Connection,
                                                    SetChargingProfileRequest                                                   Request,
                                                    ForwardingDecision<SetChargingProfileRequest, SetChargingProfileResponse>   ForwardingDecision);

    #endregion

    #region OnUnlockConnectorRequestFilter(ed)Delegate

    /// <summary>
    /// A UnlockConnector request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<UnlockConnectorRequest, UnlockConnectorResponse>>

        OnUnlockConnectorRequestFilterDelegate(DateTime                 Timestamp,
                                               IEventSender             Sender,
                                               IWebSocketConnection     Connection,
                                               UnlockConnectorRequest   Request,
                                               CancellationToken        CancellationToken);


    /// <summary>
    /// A filtered UnlockConnector request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnUnlockConnectorRequestFilteredDelegate(DateTime                                                              Timestamp,
                                                 IEventSender                                                          Sender,
                                                 IWebSocketConnection                                                  Connection,
                                                 UnlockConnectorRequest                                                Request,
                                                 ForwardingDecision<UnlockConnectorRequest, UnlockConnectorResponse>   ForwardingDecision);

    #endregion

    #region OnUpdateDynamicScheduleRequestFilter(ed)Delegate

    /// <summary>
    /// A UpdateDynamicSchedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<UpdateDynamicScheduleRequest, UpdateDynamicScheduleResponse>>

        OnUpdateDynamicScheduleRequestFilterDelegate(DateTime                       Timestamp,
                                                     IEventSender                   Sender,
                                                     IWebSocketConnection           Connection,
                                                     UpdateDynamicScheduleRequest   Request,
                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A filtered UpdateDynamicSchedule request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnUpdateDynamicScheduleRequestFilteredDelegate(DateTime                                                                          Timestamp,
                                                       IEventSender                                                                      Sender,
                                                       IWebSocketConnection                                                              Connection,
                                                       UpdateDynamicScheduleRequest                                                      Request,
                                                       ForwardingDecision<UpdateDynamicScheduleRequest, UpdateDynamicScheduleResponse>   ForwardingDecision);

    #endregion

    #region OnUsePriorityChargingRequestFilter(ed)Delegate

    /// <summary>
    /// A UsePriorityCharging request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<UsePriorityChargingRequest, UsePriorityChargingResponse>>

        OnUsePriorityChargingRequestFilterDelegate(DateTime                     Timestamp,
                                                   IEventSender                 Sender,
                                                   IWebSocketConnection         Connection,
                                                   UsePriorityChargingRequest   Request,
                                                   CancellationToken            CancellationToken);


    /// <summary>
    /// A filtered UsePriorityCharging request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnUsePriorityChargingRequestFilteredDelegate(DateTime                                                                      Timestamp,
                                                     IEventSender                                                                  Sender,
                                                     IWebSocketConnection                                                          Connection,
                                                     UsePriorityChargingRequest                                                    Request,
                                                     ForwardingDecision<UsePriorityChargingRequest, UsePriorityChargingResponse>   ForwardingDecision);

    #endregion

    #endregion

    #region Customer

    #region OnClearDisplayMessageRequestFilter(ed)Delegate

    /// <summary>
    /// A ClearDisplayMessage request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ClearDisplayMessageRequest, ClearDisplayMessageResponse>>

        OnClearDisplayMessageRequestFilterDelegate(DateTime                     Timestamp,
                                                   IEventSender                 Sender,
                                                   IWebSocketConnection         Connection,
                                                   ClearDisplayMessageRequest   Request,
                                                   CancellationToken            CancellationToken);


    /// <summary>
    /// A filtered ClearDisplayMessage request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnClearDisplayMessageRequestFilteredDelegate(DateTime                                                                      Timestamp,
                                                     IEventSender                                                                  Sender,
                                                     IWebSocketConnection                                                          Connection,
                                                     ClearDisplayMessageRequest                                                    Request,
                                                     ForwardingDecision<ClearDisplayMessageRequest, ClearDisplayMessageResponse>   ForwardingDecision);

    #endregion

    #region OnCostUpdatedRequestFilter(ed)Delegate

    /// <summary>
    /// A CostUpdated request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<CostUpdatedRequest, CostUpdatedResponse>>

        OnCostUpdatedRequestFilterDelegate(DateTime               Timestamp,
                                           IEventSender           Sender,
                                           IWebSocketConnection   Connection,
                                           CostUpdatedRequest     Request,
                                           CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered CostUpdated request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnCostUpdatedRequestFilteredDelegate(DateTime                                                      Timestamp,
                                             IEventSender                                                  Sender,
                                             IWebSocketConnection                                          Connection,
                                             CostUpdatedRequest                                            Request,
                                             ForwardingDecision<CostUpdatedRequest, CostUpdatedResponse>   ForwardingDecision);

    #endregion

    #region OnCustomerInformationRequestFilter(ed)Delegate

    /// <summary>
    /// A CustomerInformation request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<CustomerInformationRequest, CustomerInformationResponse>>

        OnCustomerInformationRequestFilterDelegate(DateTime                     Timestamp,
                                                   IEventSender                 Sender,
                                                   IWebSocketConnection         Connection,
                                                   CustomerInformationRequest   Request,
                                                   CancellationToken            CancellationToken);


    /// <summary>
    /// A filtered CustomerInformation request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnCustomerInformationRequestFilteredDelegate(DateTime                                                                      Timestamp,
                                                     IEventSender                                                                  Sender,
                                                     IWebSocketConnection                                                          Connection,
                                                     CustomerInformationRequest                                                    Request,
                                                     ForwardingDecision<CustomerInformationRequest, CustomerInformationResponse>   ForwardingDecision);

    #endregion

    #region OnGetDisplayMessagesRequestFilter(ed)Delegate

    /// <summary>
    /// A GetDisplayMessages request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetDisplayMessagesRequest, GetDisplayMessagesResponse>>

        OnGetDisplayMessagesRequestFilterDelegate(DateTime                    Timestamp,
                                                  IEventSender                Sender,
                                                  IWebSocketConnection        Connection,
                                                  GetDisplayMessagesRequest   Request,
                                                  CancellationToken           CancellationToken);


    /// <summary>
    /// A filtered GetDisplayMessages request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetDisplayMessagesRequestFilteredDelegate(DateTime                                                                    Timestamp,
                                                    IEventSender                                                                Sender,
                                                    IWebSocketConnection                                                        Connection,
                                                    GetDisplayMessagesRequest                                                   Request,
                                                    ForwardingDecision<GetDisplayMessagesRequest, GetDisplayMessagesResponse>   ForwardingDecision);

    #endregion

    #region OnSetDisplayMessageRequestFilter(ed)Delegate

    /// <summary>
    /// A SetDisplayMessage request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SetDisplayMessageRequest, SetDisplayMessageResponse>>

        OnSetDisplayMessageRequestFilterDelegate(DateTime                   Timestamp,
                                                 IEventSender               Sender,
                                                 IWebSocketConnection       Connection,
                                                 SetDisplayMessageRequest   Request,
                                                 CancellationToken          CancellationToken);


    /// <summary>
    /// A filtered SetDisplayMessage request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnSetDisplayMessageRequestFilteredDelegate(DateTime                                                                  Timestamp,
                                                   IEventSender                                                              Sender,
                                                   IWebSocketConnection                                                      Connection,
                                                   SetDisplayMessageRequest                                                  Request,
                                                   ForwardingDecision<SetDisplayMessageRequest, SetDisplayMessageResponse>   ForwardingDecision);

    #endregion

    #endregion

    #region DeviceModel

    #region OnChangeAvailabilityRequestFilter(ed)Delegate

    /// <summary>
    /// A ChangeAvailability request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ChangeAvailabilityRequest, ChangeAvailabilityResponse>>

        OnChangeAvailabilityRequestFilterDelegate(DateTime                    Timestamp,
                                                  IEventSender                Sender,
                                                  IWebSocketConnection        Connection,
                                                  ChangeAvailabilityRequest   Request,
                                                  CancellationToken           CancellationToken);


    /// <summary>
    /// A filtered ChangeAvailability request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnChangeAvailabilityRequestFilteredDelegate(DateTime                                                                    Timestamp,
                                                    IEventSender                                                                Sender,
                                                    IWebSocketConnection                                                        Connection,
                                                    ChangeAvailabilityRequest                                                   Request,
                                                    ForwardingDecision<ChangeAvailabilityRequest, ChangeAvailabilityResponse>   ForwardingDecision);

    #endregion

    #region OnClearVariableMonitoringRequestFilter(ed)Delegate

    /// <summary>
    /// A ClearVariableMonitoring request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ClearVariableMonitoringRequest, ClearVariableMonitoringResponse>>

        OnClearVariableMonitoringRequestFilterDelegate(DateTime                         Timestamp,
                                                       IEventSender                     Sender,
                                                       IWebSocketConnection             Connection,
                                                       ClearVariableMonitoringRequest   Request,
                                                       CancellationToken                CancellationToken);


    /// <summary>
    /// A filtered ClearVariableMonitoring request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnClearVariableMonitoringRequestFilteredDelegate(DateTime                                                                              Timestamp,
                                                         IEventSender                                                                          Sender,
                                                         IWebSocketConnection                                                                  Connection,
                                                         ClearVariableMonitoringRequest                                                        Request,
                                                         ForwardingDecision<ClearVariableMonitoringRequest, ClearVariableMonitoringResponse>   ForwardingDecision);

    #endregion

    #region OnGetBaseReportRequestFilter(ed)Delegate

    /// <summary>
    /// A GetBaseReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetBaseReportRequest, GetBaseReportResponse>>

        OnGetBaseReportRequestFilterDelegate(DateTime               Timestamp,
                                             IEventSender           Sender,
                                             IWebSocketConnection   Connection,
                                             GetBaseReportRequest   Request,
                                             CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered GetBaseReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetBaseReportRequestFilteredDelegate(DateTime                                                          Timestamp,
                                               IEventSender                                                      Sender,
                                               IWebSocketConnection                                              Connection,
                                               GetBaseReportRequest                                              Request,
                                               ForwardingDecision<GetBaseReportRequest, GetBaseReportResponse>   ForwardingDecision);

    #endregion

    #region OnGetLogRequestFilter(ed)Delegate

    /// <summary>
    /// A GetLog request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetLogRequest, GetLogResponse>>

        OnGetLogRequestFilterDelegate(DateTime               Timestamp,
                                      IEventSender           Sender,
                                      IWebSocketConnection   Connection,
                                      GetLogRequest          Request,
                                      CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered GetLog request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetLogRequestFilteredDelegate(DateTime                                            Timestamp,
                                        IEventSender                                        Sender,
                                        IWebSocketConnection                                Connection,
                                        GetLogRequest                                       Request,
                                        ForwardingDecision<GetLogRequest, GetLogResponse>   ForwardingDecision);

    #endregion

    #region OnGetMonitoringReportRequestFilter(ed)Delegate

    /// <summary>
    /// A GetMonitoringReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetMonitoringReportRequest, GetMonitoringReportResponse>>

        OnGetMonitoringReportRequestFilterDelegate(DateTime                     Timestamp,
                                                   IEventSender                 Sender,
                                                   IWebSocketConnection         Connection,
                                                   GetMonitoringReportRequest   Request,
                                                   CancellationToken            CancellationToken);


    /// <summary>
    /// A filtered GetMonitoringReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetMonitoringReportRequestFilteredDelegate(DateTime                                                                      Timestamp,
                                                     IEventSender                                                                  Sender,
                                                     IWebSocketConnection                                                          Connection,
                                                     GetMonitoringReportRequest                                                    Request,
                                                     ForwardingDecision<GetMonitoringReportRequest, GetMonitoringReportResponse>   ForwardingDecision);

    #endregion

    #region OnGetReportRequestFilter(ed)Delegate

    /// <summary>
    /// A GetReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetReportRequest, GetReportResponse>>

        OnGetReportRequestFilterDelegate(DateTime               Timestamp,
                                         IEventSender           Sender,
                                         IWebSocketConnection   Connection,
                                         GetReportRequest       Request,
                                         CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered GetReport request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetReportRequestFilteredDelegate(DateTime                                                  Timestamp,
                                           IEventSender                                              Sender,
                                           IWebSocketConnection                                      Connection,
                                           GetReportRequest                                          Request,
                                           ForwardingDecision<GetReportRequest, GetReportResponse>   ForwardingDecision);

    #endregion

    #region OnGetVariablesRequestFilter(ed)Delegate

    /// <summary>
    /// A GetVariables request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetVariablesRequest, GetVariablesResponse>>

        OnGetVariablesRequestFilterDelegate(DateTime               Timestamp,
                                            IEventSender           Sender,
                                            IWebSocketConnection   Connection,
                                            GetVariablesRequest    Request,
                                            CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered GetVariables request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetVariablesRequestFilteredDelegate(DateTime                                                        Timestamp,
                                              IEventSender                                                    Sender,
                                              IWebSocketConnection                                            Connection,
                                              GetVariablesRequest                                             Request,
                                              ForwardingDecision<GetVariablesRequest, GetVariablesResponse>   ForwardingDecision);

    #endregion

    #region OnSetMonitoringBaseRequestFilter(ed)Delegate

    /// <summary>
    /// A SetMonitoringBase request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SetMonitoringBaseRequest, SetMonitoringBaseResponse>>

        OnSetMonitoringBaseRequestFilterDelegate(DateTime                   Timestamp,
                                                 IEventSender               Sender,
                                                 IWebSocketConnection       Connection,
                                                 SetMonitoringBaseRequest   Request,
                                                 CancellationToken          CancellationToken);


    /// <summary>
    /// A filtered SetMonitoringBase request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnSetMonitoringBaseRequestFilteredDelegate(DateTime                                                                  Timestamp,
                                                   IEventSender                                                              Sender,
                                                   IWebSocketConnection                                                      Connection,
                                                   SetMonitoringBaseRequest                                                  Request,
                                                   ForwardingDecision<SetMonitoringBaseRequest, SetMonitoringBaseResponse>   ForwardingDecision);

    #endregion

    #region OnSetMonitoringLevelRequestFilter(ed)Delegate

    /// <summary>
    /// A SetMonitoringLevel request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SetMonitoringLevelRequest, SetMonitoringLevelResponse>>

        OnSetMonitoringLevelRequestFilterDelegate(DateTime                    Timestamp,
                                                  IEventSender                Sender,
                                                  IWebSocketConnection        Connection,
                                                  SetMonitoringLevelRequest   Request,
                                                  CancellationToken           CancellationToken);


    /// <summary>
    /// A filtered SetMonitoringLevel request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnSetMonitoringLevelRequestFilteredDelegate(DateTime                                                                    Timestamp,
                                                    IEventSender                                                                Sender,
                                                    IWebSocketConnection                                                        Connection,
                                                    SetMonitoringLevelRequest                                                   Request,
                                                    ForwardingDecision<SetMonitoringLevelRequest, SetMonitoringLevelResponse>   ForwardingDecision);

    #endregion

    #region OnSetNetworkProfileRequestFilter(ed)Delegate

    /// <summary>
    /// A SetNetworkProfile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SetNetworkProfileRequest, SetNetworkProfileResponse>>

        OnSetNetworkProfileRequestFilterDelegate(DateTime                   Timestamp,
                                                 IEventSender               Sender,
                                                 IWebSocketConnection       Connection,
                                                 SetNetworkProfileRequest   Request,
                                                 CancellationToken          CancellationToken);


    /// <summary>
    /// A filtered SetNetworkProfile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnSetNetworkProfileRequestFilteredDelegate(DateTime                                                                  Timestamp,
                                                   IEventSender                                                              Sender,
                                                   IWebSocketConnection                                                      Connection,
                                                   SetNetworkProfileRequest                                                  Request,
                                                   ForwardingDecision<SetNetworkProfileRequest, SetNetworkProfileResponse>   ForwardingDecision);

    #endregion

    #region OnSetVariableMonitoringRequestFilter(ed)Delegate

    /// <summary>
    /// A SetVariableMonitoring request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SetVariableMonitoringRequest, SetVariableMonitoringResponse>>

        OnSetVariableMonitoringRequestFilterDelegate(DateTime                       Timestamp,
                                                     IEventSender                   Sender,
                                                     IWebSocketConnection           Connection,
                                                     SetVariableMonitoringRequest   Request,
                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A filtered SetVariableMonitoring request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnSetVariableMonitoringRequestFilteredDelegate(DateTime                                                                          Timestamp,
                                                       IEventSender                                                                      Sender,
                                                       IWebSocketConnection                                                              Connection,
                                                       SetVariableMonitoringRequest                                                      Request,
                                                       ForwardingDecision<SetVariableMonitoringRequest, SetVariableMonitoringResponse>   ForwardingDecision);

    #endregion

    #region OnSetVariablesRequestFilter(ed)Delegate

    /// <summary>
    /// A SetVariables request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SetVariablesRequest, SetVariablesResponse>>

        OnSetVariablesRequestFilterDelegate(DateTime               Timestamp,
                                            IEventSender           Sender,
                                            IWebSocketConnection   Connection,
                                            SetVariablesRequest    Request,
                                            CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered SetVariables request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnSetVariablesRequestFilteredDelegate(DateTime                                                        Timestamp,
                                              IEventSender                                                    Sender,
                                              IWebSocketConnection                                            Connection,
                                              SetVariablesRequest                                             Request,
                                              ForwardingDecision<SetVariablesRequest, SetVariablesResponse>   ForwardingDecision);

    #endregion

    #region OnTriggerMessageRequestFilter(ed)Delegate

    /// <summary>
    /// A TriggerMessage request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<TriggerMessageRequest, TriggerMessageResponse>>

        OnTriggerMessageRequestFilterDelegate(DateTime                Timestamp,
                                              IEventSender            Sender,
                                              IWebSocketConnection    Connection,
                                              TriggerMessageRequest   Request,
                                              CancellationToken       CancellationToken);


    /// <summary>
    /// A filtered TriggerMessage request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnTriggerMessageRequestFilteredDelegate(DateTime                                                            Timestamp,
                                                IEventSender                                                        Sender,
                                                IWebSocketConnection                                                Connection,
                                                TriggerMessageRequest                                               Request,
                                                ForwardingDecision<TriggerMessageRequest, TriggerMessageResponse>   ForwardingDecision);

    #endregion

    #endregion

    #region E2EChargingTariffsExtensions

    #region OnGetDefaultChargingTariffRequestFilter(ed)Delegate

    /// <summary>
    /// A GetDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetDefaultChargingTariffRequest, GetDefaultChargingTariffResponse>>

        OnGetDefaultChargingTariffRequestFilterDelegate(DateTime                          Timestamp,
                                                        IEventSender                      Sender,
                                                        IWebSocketConnection              Connection,
                                                        GetDefaultChargingTariffRequest   Request,
                                                        CancellationToken                 CancellationToken);


    /// <summary>
    /// A filtered GetDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetDefaultChargingTariffRequestFilteredDelegate(DateTime                                                                                Timestamp,
                                                          IEventSender                                                                            Sender,
                                                          IWebSocketConnection                                                                    Connection,
                                                          GetDefaultChargingTariffRequest                                                         Request,
                                                          ForwardingDecision<GetDefaultChargingTariffRequest, GetDefaultChargingTariffResponse>   ForwardingDecision);

    #endregion

    #region OnRemoveDefaultChargingTariffRequestFilter(ed)Delegate

    /// <summary>
    /// A RemoveDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<RemoveDefaultChargingTariffRequest, RemoveDefaultChargingTariffResponse>>

        OnRemoveDefaultChargingTariffRequestFilterDelegate(DateTime                             Timestamp,
                                                           IEventSender                         Sender,
                                                           IWebSocketConnection                 Connection,
                                                           RemoveDefaultChargingTariffRequest   Request,
                                                           CancellationToken                    CancellationToken);


    /// <summary>
    /// A filtered RemoveDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnRemoveDefaultChargingTariffRequestFilteredDelegate(DateTime                                                                                      Timestamp,
                                                             IEventSender                                                                                  Sender,
                                                             IWebSocketConnection                                                                          Connection,
                                                             RemoveDefaultChargingTariffRequest                                                            Request,
                                                             ForwardingDecision<RemoveDefaultChargingTariffRequest, RemoveDefaultChargingTariffResponse>   ForwardingDecision);

    #endregion

    #region OnSetDefaultChargingTariffRequestFilter(ed)Delegate

    /// <summary>
    /// A SetDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SetDefaultChargingTariffRequest, SetDefaultChargingTariffResponse>>

        OnSetDefaultChargingTariffRequestFilterDelegate(DateTime                          Timestamp,
                                                        IEventSender                      Sender,
                                                        IWebSocketConnection              Connection,
                                                        SetDefaultChargingTariffRequest   Request,
                                                        CancellationToken                 CancellationToken);


    /// <summary>
    /// A filtered SetDefaultChargingTariff request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnSetDefaultChargingTariffRequestFilteredDelegate(DateTime                                                                                Timestamp,
                                                          IEventSender                                                                            Sender,
                                                          IWebSocketConnection                                                                    Connection,
                                                          SetDefaultChargingTariffRequest                                                         Request,
                                                          ForwardingDecision<SetDefaultChargingTariffRequest, SetDefaultChargingTariffResponse>   ForwardingDecision);

    #endregion

    #endregion

    #region E2ESecurityExtensions

    #region OnAddSignaturePolicyRequestFilter(ed)Delegate

    /// <summary>
    /// A AddSignaturePolicy request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<AddSignaturePolicyRequest, AddSignaturePolicyResponse>>

        OnAddSignaturePolicyRequestFilterDelegate(DateTime                    Timestamp,
                                                  IEventSender                Sender,
                                                  IWebSocketConnection        Connection,
                                                  AddSignaturePolicyRequest   Request,
                                                  CancellationToken           CancellationToken);


    /// <summary>
    /// A filtered AddSignaturePolicy request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnAddSignaturePolicyRequestFilteredDelegate(DateTime                                                                    Timestamp,
                                                    IEventSender                                                                Sender,
                                                    IWebSocketConnection                                                        Connection,
                                                    AddSignaturePolicyRequest                                                   Request,
                                                    ForwardingDecision<AddSignaturePolicyRequest, AddSignaturePolicyResponse>   ForwardingDecision);

    #endregion

    #region OnAddUserRoleRequestFilter(ed)Delegate

    /// <summary>
    /// A AddUserRole request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<AddUserRoleRequest, AddUserRoleResponse>>

        OnAddUserRoleRequestFilterDelegate(DateTime               Timestamp,
                                           IEventSender           Sender,
                                           IWebSocketConnection   Connection,
                                           AddUserRoleRequest     Request,
                                           CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered AddUserRole request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnAddUserRoleRequestFilteredDelegate(DateTime                                                      Timestamp,
                                             IEventSender                                                  Sender,
                                             IWebSocketConnection                                          Connection,
                                             AddUserRoleRequest                                            Request,
                                             ForwardingDecision<AddUserRoleRequest, AddUserRoleResponse>   ForwardingDecision);

    #endregion

    #region OnDeleteSignaturePolicyRequestFilter(ed)Delegate

    /// <summary>
    /// A DeleteSignaturePolicy request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<DeleteSignaturePolicyRequest, DeleteSignaturePolicyResponse>>

        OnDeleteSignaturePolicyRequestFilterDelegate(DateTime                       Timestamp,
                                                     IEventSender                   Sender,
                                                     IWebSocketConnection           Connection,
                                                     DeleteSignaturePolicyRequest   Request,
                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A filtered DeleteSignaturePolicy request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnDeleteSignaturePolicyRequestFilteredDelegate(DateTime                                                                          Timestamp,
                                                       IEventSender                                                                      Sender,
                                                       IWebSocketConnection                                                              Connection,
                                                       DeleteSignaturePolicyRequest                                                      Request,
                                                       ForwardingDecision<DeleteSignaturePolicyRequest, DeleteSignaturePolicyResponse>   ForwardingDecision);

    #endregion

    #region OnDeleteUserRoleRequestFilter(ed)Delegate

    /// <summary>
    /// A DeleteUserRole request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<DeleteUserRoleRequest, DeleteUserRoleResponse>>

        OnDeleteUserRoleRequestFilterDelegate(DateTime                Timestamp,
                                              IEventSender            Sender,
                                              IWebSocketConnection    Connection,
                                              DeleteUserRoleRequest   Request,
                                              CancellationToken       CancellationToken);


    /// <summary>
    /// A filtered DeleteUserRole request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnDeleteUserRoleRequestFilteredDelegate(DateTime                                                            Timestamp,
                                                IEventSender                                                        Sender,
                                                IWebSocketConnection                                                Connection,
                                                DeleteUserRoleRequest                                               Request,
                                                ForwardingDecision<DeleteUserRoleRequest, DeleteUserRoleResponse>   ForwardingDecision);

    #endregion

    #region OnUpdateSignaturePolicyRequestFilter(ed)Delegate

    /// <summary>
    /// A UpdateSignaturePolicy request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<UpdateSignaturePolicyRequest, UpdateSignaturePolicyResponse>>

        OnUpdateSignaturePolicyRequestFilterDelegate(DateTime                       Timestamp,
                                                     IEventSender                   Sender,
                                                     IWebSocketConnection           Connection,
                                                     UpdateSignaturePolicyRequest   Request,
                                                     CancellationToken              CancellationToken);


    /// <summary>
    /// A filtered UpdateSignaturePolicy request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnUpdateSignaturePolicyRequestFilteredDelegate(DateTime                                                                          Timestamp,
                                                       IEventSender                                                                      Sender,
                                                       IWebSocketConnection                                                              Connection,
                                                       UpdateSignaturePolicyRequest                                                      Request,
                                                       ForwardingDecision<UpdateSignaturePolicyRequest, UpdateSignaturePolicyResponse>   ForwardingDecision);

    #endregion

    #region OnUpdateUserRoleRequestFilter(ed)Delegate

    /// <summary>
    /// A UpdateUserRole request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<UpdateUserRoleRequest, UpdateUserRoleResponse>>

        OnUpdateUserRoleRequestFilterDelegate(DateTime                Timestamp,
                                              IEventSender            Sender,
                                              IWebSocketConnection    Connection,
                                              UpdateUserRoleRequest   Request,
                                              CancellationToken       CancellationToken);


    /// <summary>
    /// A filtered UpdateUserRole request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnUpdateUserRoleRequestFilteredDelegate(DateTime                                                            Timestamp,
                                                IEventSender                                                        Sender,
                                                IWebSocketConnection                                                Connection,
                                                UpdateUserRoleRequest                                               Request,
                                                ForwardingDecision<UpdateUserRoleRequest, UpdateUserRoleResponse>   ForwardingDecision);

    #endregion

    #endregion

    #region Firmware

    #region OnPublishFirmwareRequestFilter(ed)Delegate

    /// <summary>
    /// A PublishFirmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<PublishFirmwareRequest, PublishFirmwareResponse>>

        OnPublishFirmwareRequestFilterDelegate(DateTime                 Timestamp,
                                               IEventSender             Sender,
                                               IWebSocketConnection     Connection,
                                               PublishFirmwareRequest   Request,
                                               CancellationToken        CancellationToken);


    /// <summary>
    /// A filtered PublishFirmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnPublishFirmwareRequestFilteredDelegate(DateTime                                                              Timestamp,
                                                 IEventSender                                                          Sender,
                                                 IWebSocketConnection                                                  Connection,
                                                 PublishFirmwareRequest                                                Request,
                                                 ForwardingDecision<PublishFirmwareRequest, PublishFirmwareResponse>   ForwardingDecision);

    #endregion

    #region OnResetRequestFilter(ed)Delegate

    /// <summary>
    /// A Reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ResetRequest, ResetResponse>>

        OnResetRequestFilterDelegate(DateTime               Timestamp,
                                     IEventSender           Sender,
                                     IWebSocketConnection   Connection,
                                     ResetRequest           Request,
                                     CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered Reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnResetRequestFilteredDelegate(DateTime                                          Timestamp,
                                       IEventSender                                      Sender,
                                       IWebSocketConnection                              Connection,
                                       ResetRequest                                      Request,
                                       ForwardingDecision<ResetRequest, ResetResponse>   ForwardingDecision);

    #endregion

    #region OnUnpublishFirmwareRequestFilter(ed)Delegate

    /// <summary>
    /// A filtered UnpublishFirmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<UnpublishFirmwareRequest, UnpublishFirmwareResponse>>

        OnUnpublishFirmwareRequestFilterDelegate(DateTime                   Timestamp,
                                                 IEventSender               Sender,
                                                 IWebSocketConnection       Connection,
                                                 UnpublishFirmwareRequest   Request,
                                                 CancellationToken          CancellationToken);


    /// <summary>
    /// A UnpublishFirmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnUnpublishFirmwareRequestFilteredDelegate(DateTime                                                                  Timestamp,
                                                   IEventSender                                                              Sender,
                                                   IWebSocketConnection                                                      Connection,
                                                   UnpublishFirmwareRequest                                                  Request,
                                                   ForwardingDecision<UnpublishFirmwareRequest, UnpublishFirmwareResponse>   ForwardingDecision);

    #endregion

    #region OnUpdateFirmwareRequestFilter(ed)Delegate

    /// <summary>
    /// A filtered UpdateFirmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<UpdateFirmwareRequest, UpdateFirmwareResponse>>

        OnUpdateFirmwareRequestFilterDelegate(DateTime                Timestamp,
                                              IEventSender            Sender,
                                              IWebSocketConnection    Connection,
                                              UpdateFirmwareRequest   Request,
                                              CancellationToken       CancellationToken);


    /// <summary>
    /// A UpdateFirmware request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnUpdateFirmwareRequestFilteredDelegate(DateTime                                                            Timestamp,
                                                IEventSender                                                        Sender,
                                                IWebSocketConnection                                                Connection,
                                                UpdateFirmwareRequest                                               Request,
                                                ForwardingDecision<UpdateFirmwareRequest, UpdateFirmwareResponse>   ForwardingDecision);

    #endregion

    #endregion

    #region Grid

    #region OnAFRRSignalRequestFilter(ed)Delegate

    /// <summary>
    /// An AFRRSignal request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<AFRRSignalRequest, AFRRSignalResponse>>

        OnAFRRSignalRequestFilterDelegate(DateTime               Timestamp,
                                          IEventSender           Sender,
                                          IWebSocketConnection   Connection,
                                          AFRRSignalRequest      Request,
                                          CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered AFRRSignal request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnAFRRSignalRequestFilteredDelegate(DateTime                                                    Timestamp,
                                            IEventSender                                                Sender,
                                            IWebSocketConnection                                        Connection,
                                            AFRRSignalRequest                                           Request,
                                            ForwardingDecision<AFRRSignalRequest, AFRRSignalResponse>   ForwardingDecision);

    #endregion

    #endregion

    #region LocalList

    #region OnClearCacheRequestFilter(ed)Delegate

    /// <summary>
    /// A ClearCache request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<ClearCacheRequest, ClearCacheResponse>>

        OnClearCacheRequestFilterDelegate(DateTime               Timestamp,
                                          IEventSender           Sender,
                                          IWebSocketConnection   Connection,
                                          ClearCacheRequest      Request,
                                          CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered ClearCache request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnClearCacheRequestFilteredDelegate(DateTime                                                    Timestamp,
                                            IEventSender                                                Sender,
                                            IWebSocketConnection                                        Connection,
                                            ClearCacheRequest                                           Request,
                                            ForwardingDecision<ClearCacheRequest, ClearCacheResponse>   ForwardingDecision);

    #endregion

    #region OnGetLocalListVersionRequestFilter(ed)Delegate

    /// <summary>
    /// A GetLocalListVersion request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<GetLocalListVersionRequest, GetLocalListVersionResponse>>

        OnGetLocalListVersionRequestFilterDelegate(DateTime                     Timestamp,
                                                   IEventSender                 Sender,
                                                   IWebSocketConnection         Connection,
                                                   GetLocalListVersionRequest   Request,
                                                   CancellationToken            CancellationToken);


    /// <summary>
    /// A filtered GetLocalListVersion request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnGetLocalListVersionRequestFilteredDelegate(DateTime                                                                      Timestamp,
                                                     IEventSender                                                                  Sender,
                                                     IWebSocketConnection                                                          Connection,
                                                     GetLocalListVersionRequest                                                    Request,
                                                     ForwardingDecision<GetLocalListVersionRequest, GetLocalListVersionResponse>   ForwardingDecision);

    #endregion

    #region OnSendLocalListRequestFilter(ed)Delegate

    /// <summary>
    /// A SendLocalList request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<SendLocalListRequest, SendLocalListResponse>>

        OnSendLocalListRequestFilterDelegate(DateTime               Timestamp,
                                             IEventSender           Sender,
                                             IWebSocketConnection   Connection,
                                             SendLocalListRequest   Request,
                                             CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered SendLocalList request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnSendLocalListRequestFilteredDelegate(DateTime                                                          Timestamp,
                                               IEventSender                                                      Sender,
                                               IWebSocketConnection                                              Connection,
                                               SendLocalListRequest                                              Request,
                                               ForwardingDecision<SendLocalListRequest, SendLocalListResponse>   ForwardingDecision);

    #endregion

    #endregion

    #endregion

    #region OnBinaryDataTransferRequestFilter(ed)Delegate

    /// <summary>
    /// A BinaryDataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>>

        OnBinaryDataTransferRequestFilterDelegate(DateTime                    Timestamp,
                                                  IEventSender                Sender,
                                                  IWebSocketConnection        Connection,
                                                  BinaryDataTransferRequest   Request,
                                                  CancellationToken           CancellationToken);


    /// <summary>
    /// A filtered BinaryDataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnBinaryDataTransferRequestFilteredDelegate(DateTime                                                                    Timestamp,
                                                    IEventSender                                                                Sender,
                                                    IWebSocketConnection                                                        Connection,
                                                    BinaryDataTransferRequest                                                   Request,
                                                    ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>   ForwardingDecision);

    #endregion

    #region OnDataTransferRequestFilter(ed)Delegate

    /// <summary>
    /// A DataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ForwardingDecision<DataTransferRequest, DataTransferResponse>>

        OnDataTransferRequestFilterDelegate(DateTime               Timestamp,
                                            IEventSender           Sender,
                                            IWebSocketConnection   Connection,
                                            DataTransferRequest    Request,
                                            CancellationToken      CancellationToken);


    /// <summary>
    /// A filtered DataTransfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Connection">The HTTP Web Socket connection.</param>
    /// <param name="Request">The request.</param>
    /// <param name="ForwardingDecision">The forwarding decision.</param>
    public delegate Task

        OnDataTransferRequestFilteredDelegate(DateTime                                                        Timestamp,
                                              IEventSender                                                    Sender,
                                              IWebSocketConnection                                            Connection,
                                              DataTransferRequest                                             Request,
                                              ForwardingDecision<DataTransferRequest, DataTransferResponse>   ForwardingDecision);

    #endregion




    /// <summary>
    /// A filtered JSON request message.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request message.</param>
    /// <param name="Sender">The sender of the request message.</param>
    /// <param name="JSONRequestMessage">The JSON request message.</param>
    /// <param name="SendOCPPMessageResult">The result of the sending attempt.</param>
    public delegate Task

        OnJSONRequestMessageSentDelegate(DateTime                 Timestamp,
                                         IEventSender             Sender,
                                         OCPP_JSONRequestMessage  JSONRequestMessage,
                                         SendOCPPMessageResult    SendOCPPMessageResult);

    /// <summary>
    /// A filtered JSON response message.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the response message.</param>
    /// <param name="Sender">The sender of the response message.</param>
    /// <param name="JSONResponseMessage">The JSON response message.</param>
    /// <param name="SendOCPPMessageResult">The result of the sending attempt.</param>
    public delegate Task

        OnJSONResponseMessageSentDelegate(DateTime                  Timestamp,
                                          IEventSender              Sender,
                                          OCPP_JSONResponseMessage  JSONResponseMessage,
                                          SendOCPPMessageResult     SendOCPPMessageResult);



    public interface IOCPPWebSocketAdapterFORWARD
    {

        ForwardingResult            DefaultResult        { get; set; }

        HashSet<NetworkingNode_Id>  AnycastIdsAllowed    { get; }

        HashSet<NetworkingNode_Id>  AnycastIdsDenied     { get; }

        #region Events

        #region CS

        #region BinaryDataStreamsExtensions

        #endregion

        #region Certificates

        event OnGet15118EVCertificateRequestFilterDelegate?                 OnGet15118EVCertificateRequest;
        event OnGet15118EVCertificateRequestFilteredDelegate?               OnGet15118EVCertificateRequestLogging;

        event OnGetCertificateStatusRequestFilterDelegate?                  OnGetCertificateStatusRequest;
        event OnGetCertificateStatusRequestFilteredDelegate?                OnGetCertificateStatusRequestLogging;

        event OnGetCRLRequestFilterDelegate?                                OnGetCRLRequest;
        event OnGetCRLRequestFilteredDelegate?                              OnGetCRLRequestLogging;

        event OnSignCertificateRequestFilterDelegate?                       OnSignCertificateRequest;
        event OnSignCertificateRequestFilteredDelegate?                     OnSignCertificateRequestLogging;

        #endregion

        #region Charging

        event OnAuthorizeRequestFilterDelegate?                             OnAuthorizeRequest;
        event OnAuthorizeRequestFilteredDelegate?                           OnAuthorizeRequestLogging;

        event OnClearedChargingLimitRequestFilterDelegate?                  OnClearedChargingLimitRequest;
        event OnClearedChargingLimitRequestFilteredDelegate?                OnClearedChargingLimitRequestLogging;

        event OnMeterValuesRequestFilterDelegate?                           OnMeterValuesRequest;
        event OnMeterValuesRequestFilteredDelegate?                         OnMeterValuesRequestLogging;

        event OnNotifyChargingLimitRequestFilterDelegate?                   OnNotifyChargingLimitRequest;
        event OnNotifyChargingLimitRequestFilteredDelegate?                 OnNotifyChargingLimitRequestLogging;

        event OnNotifyEVChargingNeedsRequestFilterDelegate?                 OnNotifyEVChargingNeedsRequest;
        event OnNotifyEVChargingNeedsRequestFilteredDelegate?               OnNotifyEVChargingNeedsRequestLogging;

        event OnNotifyEVChargingScheduleRequestFilterDelegate?              OnNotifyEVChargingScheduleRequest;
        event OnNotifyEVChargingScheduleRequestFilteredDelegate?            OnNotifyEVChargingScheduleRequestLogging;

        event OnNotifyPriorityChargingRequestFilterDelegate?                OnNotifyPriorityChargingRequest;
        event OnNotifyPriorityChargingRequestFilteredDelegate?              OnNotifyPriorityChargingRequestLogging;

        event OnPullDynamicScheduleUpdateRequestFilterDelegate?             OnPullDynamicScheduleUpdateRequest;
        event OnPullDynamicScheduleUpdateRequestFilteredDelegate?           OnPullDynamicScheduleUpdateRequestLogging;

        event OnReportChargingProfilesRequestFilterDelegate?                OnReportChargingProfilesRequest;
        event OnReportChargingProfilesRequestFilteredDelegate?              OnReportChargingProfilesRequestLogging;

        event OnReservationStatusUpdateRequestFilterDelegate?               OnReservationStatusUpdateRequest;
        event OnReservationStatusUpdateRequestFilteredDelegate?             OnReservationStatusUpdateRequestLogging;

        event OnStatusNotificationRequestFilterDelegate?                    OnStatusNotificationRequest;
        event OnStatusNotificationRequestFilteredDelegate?                  OnStatusNotificationRequestLogging;

        event OnTransactionEventRequestFilterDelegate?                      OnTransactionEventRequest;
        event OnTransactionEventRequestFilteredDelegate?                    OnTransactionEventRequestLogging;

        #endregion

        #region Customer

        event OnNotifyCustomerInformationRequestFilterDelegate?             OnNotifyCustomerInformationRequest;
        event OnNotifyCustomerInformationRequestFilteredDelegate?           OnNotifyCustomerInformationRequestLogging;

        event OnNotifyDisplayMessagesRequestFilterDelegate?                 OnNotifyDisplayMessagesRequest;
        event OnNotifyDisplayMessagesRequestFilteredDelegate?               OnNotifyDisplayMessagesRequestLogging;

        #endregion

        #region DeviceModel

        event OnLogStatusNotificationRequestFilterDelegate?                 OnLogStatusNotificationRequest;
        event OnLogStatusNotificationRequestFilteredDelegate?               OnLogStatusNotificationRequestLogging;

        event OnNotifyEventRequestFilterDelegate?                           OnNotifyEventRequest;
        event OnNotifyEventRequestFilteredDelegate?                         OnNotifyEventRequestLogging;

        event OnNotifyMonitoringReportRequestFilterDelegate?                OnNotifyMonitoringReportRequest;
        event OnNotifyMonitoringReportRequestFilteredDelegate?              OnNotifyMonitoringReportRequestLogging;

        event OnNotifyReportRequestFilterDelegate?                          OnNotifyReportRequest;
        event OnNotifyReportRequestFilteredDelegate?                        OnNotifyReportRequestLogging;

        event OnSecurityEventNotificationRequestFilterDelegate?             OnSecurityEventNotificationRequest;
        event OnSecurityEventNotificationRequestFilteredDelegate?           OnSecurityEventNotificationRequestLogging;

        #endregion

        #region Firmware

        event OnBootNotificationRequestFilterDelegate?                      OnBootNotificationRequest;
        event OnBootNotificationRequestFilteredDelegate?                    OnBootNotificationRequestLogging;

        event OnFirmwareStatusNotificationRequestFilterDelegate?            OnFirmwareStatusNotificationRequest;
        event OnFirmwareStatusNotificationRequestFilteredDelegate?          OnFirmwareStatusNotificationRequestLogging;

        event OnHeartbeatRequestFilterDelegate?                             OnHeartbeatRequest;
        event OnHeartbeatRequestFilteredDelegate?                           OnHeartbeatRequestLogging;

        event OnPublishFirmwareStatusNotificationRequestFilterDelegate?     OnPublishFirmwareStatusNotificationRequest;
        event OnPublishFirmwareStatusNotificationRequestFilteredDelegate?   OnPublishFirmwareStatusNotificationRequestLogging;

        #endregion

        #endregion

        #region CSMS

        #region BinaryDataStreamsExtensions

        event OnDeleteFileRequestFilterDelegate?                            OnDeleteFileRequest;
        event OnDeleteFileRequestFilteredDelegate?                          OnDeleteFileRequestLogging;

        event OnGetFileRequestFilterDelegate?                               OnGetFileRequest;
        event OnGetFileRequestFilteredDelegate?                             OnGetFileRequestLogging;

        event OnListDirectoryRequestFilterDelegate?                         OnListDirectoryRequest;
        event OnListDirectoryRequestFilteredDelegate?                       OnListDirectoryRequestLogging;

        event OnSendFileRequestFilterDelegate?                              OnSendFileRequest;
        event OnSendFileRequestFilteredDelegate?                            OnSendFileRequestLogging;

        #endregion

        #region Certificates

        event OnCertificateSignedRequestFilterDelegate?                     OnCertificateSignedRequest;
        event OnCertificateSignedRequestFilteredDelegate?                   OnCertificateSignedRequestLogging;

        event OnDeleteCertificateRequestFilterDelegate?                     OnDeleteCertificateRequest;
        event OnDeleteCertificateRequestFilteredDelegate?                   OnDeleteCertificateRequestLogging;

        event OnGetInstalledCertificateIdsRequestFilterDelegate?            OnGetInstalledCertificateIdsRequest;
        event OnGetInstalledCertificateIdsRequestFilteredDelegate?          OnGetInstalledCertificateIdsRequestLogging;

        event OnInstallCertificateRequestFilterDelegate?                    OnInstallCertificateRequest;
        event OnInstallCertificateRequestFilteredDelegate?                  OnInstallCertificateRequestLogging;

        event OnNotifyCRLRequestFilterDelegate?                             OnNotifyCRLRequest;
        event OnNotifyCRLRequestFilteredDelegate?                           OnNotifyCRLRequestLogging;

        #endregion

        #region Charging

        event OnCancelReservationRequestFilterDelegate?                     OnCancelReservationRequest;
        event OnCancelReservationRequestFilteredDelegate?                   OnCancelReservationRequestLogging;

        event OnClearChargingProfileRequestFilterDelegate?                  OnClearChargingProfileRequest;
        event OnClearChargingProfileRequestFilteredDelegate?                OnClearChargingProfileRequestLogging;

        event OnGetChargingProfilesRequestFilterDelegate?                   OnGetChargingProfilesRequest;
        event OnGetChargingProfilesRequestFilteredDelegate?                 OnGetChargingProfilesRequestLogging;

        event OnGetCompositeScheduleRequestFilterDelegate?                  OnGetCompositeScheduleRequest;
        event OnGetCompositeScheduleRequestFilteredDelegate?                OnGetCompositeScheduleRequestLogging;

        event OnGetTransactionStatusRequestFilterDelegate?                  OnGetTransactionStatusRequest;
        event OnGetTransactionStatusRequestFilteredDelegate?                OnGetTransactionStatusRequestLogging;

        event OnNotifyAllowedEnergyTransferRequestFilterDelegate?           OnNotifyAllowedEnergyTransferRequest;
        event OnNotifyAllowedEnergyTransferRequestFilteredDelegate?         OnNotifyAllowedEnergyTransferRequestLogging;

        event OnRequestStartTransactionRequestFilterDelegate?               OnRequestStartTransactionRequest;
        event OnRequestStartTransactionRequestFilteredDelegate?             OnRequestStartTransactionRequestLogging;

        event OnRequestStopTransactionRequestFilterDelegate?                OnRequestStopTransactionRequest;
        event OnRequestStopTransactionRequestFilteredDelegate?              OnRequestStopTransactionRequestLogging;

        event OnReserveNowRequestFilterDelegate?                            OnReserveNowRequest;
        event OnReserveNowRequestFilteredDelegate?                          OnReserveNowRequestLogging;

        event OnSetChargingProfileRequestFilterDelegate?                    OnSetChargingProfileRequest;
        event OnSetChargingProfileRequestFilteredDelegate?                  OnSetChargingProfileRequestLogging;

        event OnUnlockConnectorRequestFilterDelegate?                       OnUnlockConnectorRequest;
        event OnUnlockConnectorRequestFilteredDelegate?                     OnUnlockConnectorRequestLogging;

        event OnUpdateDynamicScheduleRequestFilterDelegate?                 OnUpdateDynamicScheduleRequest;
        event OnUpdateDynamicScheduleRequestFilteredDelegate?               OnUpdateDynamicScheduleRequestLogging;

        event OnUsePriorityChargingRequestFilterDelegate?                   OnUsePriorityChargingRequest;
        event OnUsePriorityChargingRequestFilteredDelegate?                 OnUsePriorityChargingRequestLogging;

        #endregion

        #region Customer

        event OnClearDisplayMessageRequestFilterDelegate?                   OnClearDisplayMessageRequest;
        event OnClearDisplayMessageRequestFilteredDelegate?                 OnClearDisplayMessageRequestLogging;

        event OnCostUpdatedRequestFilterDelegate?                           OnCostUpdatedRequest;
        event OnCostUpdatedRequestFilteredDelegate?                         OnCostUpdatedRequestLogging;

        event OnCustomerInformationRequestFilterDelegate?                   OnCustomerInformationRequest;
        event OnCustomerInformationRequestFilteredDelegate?                 OnCustomerInformationRequestLogging;

        event OnGetDisplayMessagesRequestFilterDelegate?                    OnGetDisplayMessagesRequest;
        event OnGetDisplayMessagesRequestFilteredDelegate?                  OnGetDisplayMessagesRequestLogging;

        event OnSetDisplayMessageRequestFilterDelegate?                     OnSetDisplayMessageRequest;
        event OnSetDisplayMessageRequestFilteredDelegate?                   OnSetDisplayMessageRequestLogging;

        #endregion

        #region DeviceModel

        event OnChangeAvailabilityRequestFilterDelegate?                    OnChangeAvailabilityRequest;
        event OnChangeAvailabilityRequestFilteredDelegate?                  OnChangeAvailabilityRequestLogging;

        event OnClearVariableMonitoringRequestFilterDelegate?               OnClearVariableMonitoringRequest;
        event OnClearVariableMonitoringRequestFilteredDelegate?             OnClearVariableMonitoringRequestLogging;

        event OnGetBaseReportRequestFilterDelegate?                         OnGetBaseReportRequest;
        event OnGetBaseReportRequestFilteredDelegate?                       OnGetBaseReportRequestLogging;

        event OnGetLogRequestFilterDelegate?                                OnGetLogRequest;
        event OnGetLogRequestFilteredDelegate?                              OnGetLogRequestLogging;

        event OnGetMonitoringReportRequestFilterDelegate?                   OnGetMonitoringReportRequest;
        event OnGetMonitoringReportRequestFilteredDelegate?                 OnGetMonitoringReportRequestLogging;

        event OnGetReportRequestFilterDelegate?                             OnGetReportRequest;
        event OnGetReportRequestFilteredDelegate?                           OnGetReportRequestLogging;

        event OnGetVariablesRequestFilterDelegate?                          OnGetVariablesRequest;
        event OnGetVariablesRequestFilteredDelegate?                        OnGetVariablesRequestLogging;

        event OnSetMonitoringBaseRequestFilterDelegate?                     OnSetMonitoringBaseRequest;
        event OnSetMonitoringBaseRequestFilteredDelegate?                   OnSetMonitoringBaseRequestLogging;

        event OnSetMonitoringLevelRequestFilterDelegate?                    OnSetMonitoringLevelRequest;
        event OnSetMonitoringLevelRequestFilteredDelegate?                  OnSetMonitoringLevelRequestLogging;

        event OnSetNetworkProfileRequestFilterDelegate?                     OnSetNetworkProfileRequest;
        event OnSetNetworkProfileRequestFilteredDelegate?                   OnSetNetworkProfileRequestLogging;

        event OnSetVariableMonitoringRequestFilterDelegate?                 OnSetVariableMonitoringRequest;
        event OnSetVariableMonitoringRequestFilteredDelegate?               OnSetVariableMonitoringRequestLogging;

        event OnSetVariablesRequestFilterDelegate?                          OnSetVariablesRequest;
        event OnSetVariablesRequestFilteredDelegate?                        OnSetVariablesRequestLogging;

        event OnTriggerMessageRequestFilterDelegate?                        OnTriggerMessageRequest;
        event OnTriggerMessageRequestFilteredDelegate?                      OnTriggerMessageRequestLogging;

        #endregion

        #region E2EChargingTariffsExtensions

        event OnGetDefaultChargingTariffRequestFilterDelegate?              OnGetDefaultChargingTariffRequest;
        event OnGetDefaultChargingTariffRequestFilteredDelegate?            OnGetDefaultChargingTariffRequestLogging;

        event OnRemoveDefaultChargingTariffRequestFilterDelegate?           OnRemoveDefaultChargingTariffRequest;
        event OnRemoveDefaultChargingTariffRequestFilteredDelegate?         OnRemoveDefaultChargingTariffRequestLogging;

        event OnSetDefaultChargingTariffRequestFilterDelegate?              OnSetDefaultChargingTariffRequest;
        event OnSetDefaultChargingTariffRequestFilteredDelegate?            OnSetDefaultChargingTariffRequestLogging;

        #endregion

        #region E2ESecurityExtensions

        event OnAddSignaturePolicyRequestFilterDelegate?                    OnAddSignaturePolicyRequest;
        event OnAddSignaturePolicyRequestFilteredDelegate?                  OnAddSignaturePolicyRequestLogging;

        event OnAddUserRoleRequestFilterDelegate?                           OnAddUserRoleRequest;
        event OnAddUserRoleRequestFilteredDelegate?                         OnAddUserRoleRequestLogging;

        event OnDeleteSignaturePolicyRequestFilterDelegate?                 OnDeleteSignaturePolicyRequest;
        event OnDeleteSignaturePolicyRequestFilteredDelegate?               OnDeleteSignaturePolicyRequestLogging;

        event OnDeleteUserRoleRequestFilterDelegate?                        OnDeleteUserRoleRequest;
        event OnDeleteUserRoleRequestFilteredDelegate?                      OnDeleteUserRoleRequestLogging;

        event OnUpdateSignaturePolicyRequestFilterDelegate?                 OnUpdateSignaturePolicyRequest;
        event OnUpdateSignaturePolicyRequestFilteredDelegate?               OnUpdateSignaturePolicyRequestLogging;

        event OnUpdateUserRoleRequestFilterDelegate?                        OnUpdateUserRoleRequest;
        event OnUpdateUserRoleRequestFilteredDelegate?                      OnUpdateUserRoleRequestLogging;

        #endregion

        #region Firmware

        event OnPublishFirmwareRequestFilterDelegate?                       OnPublishFirmwareRequest;
        event OnPublishFirmwareRequestFilteredDelegate?                     OnPublishFirmwareRequestLogging;

        event OnResetRequestFilterDelegate?                                 OnResetRequest;
        event OnResetRequestFilteredDelegate?                               OnResetRequestLogging;

        event OnUnpublishFirmwareRequestFilterDelegate?                     OnUnpublishFirmwareRequest;
        event OnUnpublishFirmwareRequestFilteredDelegate?                   OnUnpublishFirmwareRequestLogging;

        event OnUpdateFirmwareRequestFilterDelegate?                        OnUpdateFirmwareRequest;
        event OnUpdateFirmwareRequestFilteredDelegate?                      OnUpdateFirmwareRequestLogging;

        #endregion

        #region Grid

        event OnAFRRSignalRequestFilterDelegate?                            OnAFRRSignalRequest;
        event OnAFRRSignalRequestFilteredDelegate?                          OnAFRRSignalRequestLogging;

        #endregion

        #region LocalList

        event OnClearCacheRequestFilterDelegate?                            OnClearCacheRequest;
        event OnClearCacheRequestFilteredDelegate?                          OnClearCacheRequestLogging;

        event OnGetLocalListVersionRequestFilterDelegate?                   OnGetLocalListVersionRequest;
        event OnGetLocalListVersionRequestFilteredDelegate?                 OnGetLocalListVersionRequestLogging;

        event OnSendLocalListRequestFilterDelegate?                         OnSendLocalListRequest;
        event OnSendLocalListRequestFilteredDelegate?                       OnSendLocalListRequestLogging;

        #endregion

        #endregion

        // Bidirectional

        event OnBinaryDataTransferRequestFilterDelegate?                    OnBinaryDataTransferRequest;
        event OnBinaryDataTransferRequestFilteredDelegate?                  OnBinaryDataTransferRequestLogging;

        event OnDataTransferRequestFilterDelegate?                          OnDataTransferRequest;
        event OnDataTransferRequestFilteredDelegate?                        OnDataTransferRequestLogging;


        event OnJSONRequestMessageSentDelegate?   OnJSONRequestMessageSent;
        event OnJSONResponseMessageSentDelegate?  OnJSONResponseMessageSent;

        #endregion


        NetworkingNode_Id? GetForwardedNodeId(Request_Id RequestId);


        Task ProcessJSONRequestMessage   (OCPP_JSONRequestMessage     JSONRequestMessage);
        Task ProcessJSONResponseMessage  (OCPP_JSONResponseMessage    JSONResponseMessage);
        Task ProcessJSONErrorMessage     (OCPP_JSONErrorMessage       JSONErrorMessage);


        Task ProcessBinaryRequestMessage (OCPP_BinaryRequestMessage   BinaryRequestMessage);
        Task ProcessBinaryResponseMessage(OCPP_BinaryResponseMessage  BinaryResponseMessage);


        #region CS

        #region BinaryDataStreamsExtensions

        #endregion

        #region Certificates
        Task<ForwardingDecision> Forward_Get15118EVCertificate             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetCertificateStatus              (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetCRL                            (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SignCertificate                   (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region Charging
        Task<ForwardingDecision> Forward_Authorize                         (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_ClearedChargingLimit              (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_MeterValues                       (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyChargingLimit               (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyEVChargingNeeds             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyEVChargingSchedule          (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyPriorityCharging            (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_PullDynamicScheduleUpdate         (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_ReportChargingProfiles            (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_ReservationStatusUpdate           (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_StatusNotification                (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_TransactionEvent                  (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region Customer
        Task<ForwardingDecision> Forward_NotifyCustomerInformation         (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyDisplayMessages             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region DeviceModel
        Task<ForwardingDecision> Forward_LogStatusNotification             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyEvent                       (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyMonitoringReport            (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyReport                      (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SecurityEventNotification         (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region Firmware

        Task<ForwardingDecision> Forward_BootNotification                  (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_FirmwareStatusNotification        (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_Heartbeat                         (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_PublishFirmwareStatusNotification (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #endregion

        #region CSMS

        #region BinaryDataStreamsExtensions

        Task<ForwardingDecision> Forward_DeleteFile                        (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetFile                           (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_ListDirectory                     (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SendFile                          (OCPP_BinaryRequestMessage BinaryRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region Certificates
        Task<ForwardingDecision> Forward_CertificateSigned                 (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_DeleteCertificate                 (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetInstalledCertificateIds        (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_InstallCertificate                (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyCRL                         (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region Charging

        Task<ForwardingDecision> Forward_CancelReservation                 (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_ClearChargingProfile              (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetChargingProfiles               (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetCompositeSchedule              (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetTransactionStatus              (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_NotifyAllowedEnergyTransfer       (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_RequestStartTransaction           (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_RequestStopTransaction            (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_ReserveNow                        (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SetChargingProfile                (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_UnlockConnector                   (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_UpdateDynamicSchedule             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_UsePriorityCharging               (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region Customer

        Task<ForwardingDecision> Forward_ClearDisplayMessage               (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_CostUpdated                       (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_CustomerInformation               (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetDisplayMessages                (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SetDisplayMessage                 (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region DeviceModel
        Task<ForwardingDecision> Forward_ChangeAvailability                (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_ClearVariableMonitoring           (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetBaseReport                     (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetLog                            (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetMonitoringReport               (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetReport                         (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetVariables                      (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SetMonitoringBase                 (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SetMonitoringLevel                (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SetNetworkProfile                 (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SetVariableMonitoring             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SetVariables                      (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_TriggerMessage                    (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region E2EChargingTariffsExtensions

        Task<ForwardingDecision> Forward_GetDefaultChargingTariff          (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_RemoveDefaultChargingTariff       (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SetDefaultChargingTariff          (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region E2ESecurityExtensions

        Task<ForwardingDecision> Forward_AddSignaturePolicy                (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_AddUserRole                       (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_DeleteSignaturePolicy             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_DeleteUserRole                    (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_UpdateSignaturePolicy             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_UpdateUserRole                    (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region Firmware

        Task<ForwardingDecision> Forward_PublishFirmware                   (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_Reset                             (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_UnpublishFirmware                 (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_UpdateFirmware                    (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region Grid
        Task<ForwardingDecision> Forward_AFRRSignal                        (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #region LocalList

        Task<ForwardingDecision> Forward_ClearCache                        (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_GetLocalListVersion               (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision> Forward_SendLocalList                     (OCPP_JSONRequestMessage   JSONRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);

        #endregion

        #endregion

        Task<ForwardingDecision>                 Forward_BinaryDataTransfer                (OCPP_BinaryRequestMessage BinaryRequestMessage, IWebSocketConnection Connection, CancellationToken CancellationToken = default);
        Task<ForwardingDecision>                 Forward_DataTransfer                      (OCPP_JSONRequestMessage   JSONRequestMessage,   IWebSocketConnection Connection, CancellationToken CancellationToken = default);


    }

}
