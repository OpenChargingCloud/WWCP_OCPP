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

using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    public delegate Task  OnWebSocketClientTextMessageResponseDelegate  (DateTime                  Timestamp,
                                                                         ChargingStationWSClient   Client,
                                                                         WebSocketFrame            Frame,
                                                                         EventTracking_Id          EventTrackingId,
                                                                         DateTime                  RequestTimestamp,
                                                                         String                    RequestMessage,
                                                                         DateTime                  ResponseTimestamp,
                                                                         String                    ResponseMessage);


    public delegate Task  OnWebSocketClientBinaryMessageResponseDelegate(DateTime                  Timestamp,
                                                                         ChargingStationWSClient   Client,
                                                                         WebSocketFrame            Frame,
                                                                         EventTracking_Id          EventTrackingId,
                                                                         DateTime                  RequestTimestamp,
                                                                         Byte[]                    RequestMessage,
                                                                         DateTime                  ResponseTimestamp,
                                                                         Byte[]                    ResponseMessage);


    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region (class) SendRequestState2

        public class SendRequestState2
        {

            public DateTime                       Timestamp            { get; }
            public OCPP_WebSocket_RequestMessage  WSRequestMessage     { get; }
            public DateTime                       Timeout              { get; }

            public DateTime?                      ResponseTimestamp    { get; set; }
            public JObject?                       Response             { get; set; }

            public ResultCodes?                   ErrorCode            { get; set; }
            public String?                        ErrorDescription     { get; set; }
            public JObject?                       ErrorDetails         { get; set; }


            public Boolean                        NoErrors
                 => !ErrorCode.HasValue;

            public Boolean                        HasErrors
                 =>  ErrorCode.HasValue;


            public SendRequestState2(DateTime                       Timestamp,
                                     OCPP_WebSocket_RequestMessage  WSRequestMessage,
                                     DateTime                       Timeout,

                                     DateTime?                      ResponseTimestamp   = null,
                                     JObject?                       Response            = null,

                                     ResultCodes?                   ErrorCode           = null,
                                     String?                        ErrorDescription    = null,
                                     JObject?                       ErrorDetails        = null)
            {

                this.Timestamp          = Timestamp;
                this.WSRequestMessage   = WSRequestMessage;
                this.Timeout            = Timeout;

                this.ResponseTimestamp  = ResponseTimestamp;
                this.Response           = Response;

                this.ErrorCode          = ErrorCode;
                this.ErrorDescription   = ErrorDescription;
                this.ErrorDetails       = ErrorDetails;

            }

        }

        #endregion


        #region Data

        /// <summary>
        /// The default HTTP user agent string.
        /// </summary>
        public new const       String    DefaultHTTPUserAgent    = $"GraphDefined OCPP {Version.String} CP WebSocket Client";

        private const          String    LogfileName             = "ChargePointWSClient.log";

        public static readonly TimeSpan  DefaultRequestTimeout   = TimeSpan.FromSeconds(30);

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this charge box.
        /// </summary>
        public ChargeBox_Id                         ChargeBoxIdentity               { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => ChargeBoxIdentity.ToString();

        /// <summary>
        /// The source URI of the websocket message.
        /// </summary>
        public String                                From                            { get; }

        /// <summary>
        /// The destination URI of the websocket message.
        /// </summary>
        public String                                To                              { get; }

        /// <summary>
        /// The JSON formatting to use.
        /// </summary>
        public Formatting                            JSONFormatting                  { get; set; } = Formatting.None;

        /// <summary>
        /// The attached OCPP CP client (HTTP/websocket client) logger.
        /// </summary>
        //public ChargePointWSClient.CPClientLogger    Logger                          { get; }

        #endregion

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<ResetRequest>?                        CustomResetRequestParser                         { get; set; }
        public CustomJObjectParserDelegate<UpdateFirmwareRequest>?               CustomUpdateFirmwareRequestParser                { get; set; }
        public CustomJObjectParserDelegate<PublishFirmwareRequest>?              CustomPublishFirmwareRequestParser               { get; set; }
        public CustomJObjectParserDelegate<UnpublishFirmwareRequest>?            CustomUnpublishFirmwareRequestParser             { get; set; }
        public CustomJObjectParserDelegate<GetBaseReportRequest>?                CustomGetBaseReportRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<GetReportRequest>?                    CustomGetReportRequestParser                     { get; set; }
        public CustomJObjectParserDelegate<GetLogRequest>?                       CustomGetLogRequestParser                        { get; set; }
        public CustomJObjectParserDelegate<SetVariablesRequest>?                 CustomSetVariablesRequestParser                  { get; set; }
        public CustomJObjectParserDelegate<GetVariablesRequest>?                 CustomGetVariablesRequestParser                  { get; set; }
        public CustomJObjectParserDelegate<SetMonitoringBaseRequest>?            CustomSetMonitoringBaseRequestParser             { get; set; }
        public CustomJObjectParserDelegate<GetMonitoringReportRequest>?          CustomGetMonitoringReportRequestParser           { get; set; }
        public CustomJObjectParserDelegate<SetMonitoringLevelRequest>?           CustomSetMonitoringLevelRequestParser            { get; set; }
        public CustomJObjectParserDelegate<SetVariableMonitoringRequest>?        CustomSetVariableMonitoringRequestParser         { get; set; }
        public CustomJObjectParserDelegate<ClearVariableMonitoringRequest>?      CustomClearVariableMonitoringRequestParser       { get; set; }
        public CustomJObjectParserDelegate<SetNetworkProfileRequest>?            CustomSetNetworkProfileRequestParser             { get; set; }
        public CustomJObjectParserDelegate<ChangeAvailabilityRequest>?           CustomChangeAvailabilityRequestParser            { get; set; }
        public CustomJObjectParserDelegate<TriggerMessageRequest>?               CustomTriggerMessageRequestParser                { get; set; }
        public CustomJObjectParserDelegate<CSMS.DataTransferRequest>?            CustomIncomingDataTransferRequestParser          { get; set; }

        public CustomJObjectParserDelegate<CertificateSignedRequest>?            CustomCertificateSignedRequestParser             { get; set; }
        public CustomJObjectParserDelegate<InstallCertificateRequest>?           CustomInstallCertificateRequestParser            { get; set; }
        public CustomJObjectParserDelegate<GetInstalledCertificateIdsRequest>?   CustomGetInstalledCertificateIdsRequestParser    { get; set; }
        public CustomJObjectParserDelegate<DeleteCertificateRequest>?            CustomDeleteCertificateRequestParser             { get; set; }
        public CustomJObjectParserDelegate<NotifyCRLRequest>?                    CustomNotifyCRLRequestParser                     { get; set; }

        public CustomJObjectParserDelegate<GetLocalListVersionRequest>?          CustomGetLocalListVersionRequestParser           { get; set; }
        public CustomJObjectParserDelegate<SendLocalListRequest>?                CustomSendLocalListRequestParser                 { get; set; }
        public CustomJObjectParserDelegate<ClearCacheRequest>?                   CustomClearCacheRequestParser                    { get; set; }

        public CustomJObjectParserDelegate<ReserveNowRequest>?                   CustomReserveNowRequestParser                    { get; set; }
        public CustomJObjectParserDelegate<CancelReservationRequest>?            CustomCancelReservationRequestParser             { get; set; }

        public CustomJObjectParserDelegate<RequestStartTransactionRequest>?      CustomRequestStartTransactionRequestParser       { get; set; }
        public CustomJObjectParserDelegate<RequestStopTransactionRequest>?       CustomRequestStopTransactionRequestParser        { get; set; }
        public CustomJObjectParserDelegate<GetTransactionStatusRequest>?         CustomGetTransactionStatusRequestParser          { get; set; }
        public CustomJObjectParserDelegate<SetChargingProfileRequest>?           CustomSetChargingProfileRequestParser            { get; set; }
        public CustomJObjectParserDelegate<GetChargingProfilesRequest>?          CustomGetChargingProfilesRequestParser           { get; set; }
        public CustomJObjectParserDelegate<ClearChargingProfileRequest>?         CustomClearChargingProfileRequestParser          { get; set; }
        public CustomJObjectParserDelegate<GetCompositeScheduleRequest>?         CustomGetCompositeScheduleRequestParser          { get; set; }
        public CustomJObjectParserDelegate<UpdateDynamicScheduleRequest>?        CustomUpdateDynamicScheduleRequestParser         { get; set; }
        public CustomJObjectParserDelegate<NotifyAllowedEnergyTransferRequest>?  CustomNotifyAllowedEnergyTransferRequestParser   { get; set; }
        public CustomJObjectParserDelegate<UsePriorityChargingRequest>?          CustomUsePriorityChargingRequestParser           { get; set; }
        public CustomJObjectParserDelegate<UnlockConnectorRequest>?              CustomUnlockConnectorRequestParser               { get; set; }

        public CustomJObjectParserDelegate<AFRRSignalRequest>?                   CustomAFRRSignalRequestParser                    { get; set; }

        public CustomJObjectParserDelegate<SetDisplayMessageRequest>?            CustomSetDisplayMessageRequestParser             { get; set; }
        public CustomJObjectParserDelegate<GetDisplayMessagesRequest>?           CustomGetDisplayMessagesRequestParser            { get; set; }
        public CustomJObjectParserDelegate<ClearDisplayMessageRequest>?          CustomClearDisplayMessageRequestParser           { get; set; }
        public CustomJObjectParserDelegate<CostUpdatedRequest>?                  CustomCostUpdatedRequestParser                   { get; set; }
        public CustomJObjectParserDelegate<CustomerInformationRequest>?          CustomCustomerInformationRequestParser           { get; set; }

        #endregion

        #region Custom JSON serializer delegates

        // Messages
        public CustomJObjectSerializerDelegate<BootNotificationRequest>?                   CustomBootNotificationRequestSerializer                     { get; set; }

        public CustomJObjectSerializerDelegate<FirmwareStatusNotificationRequest>?         CustomFirmwareStatusNotificationRequestSerializer           { get; set; }

        public CustomJObjectSerializerDelegate<PublishFirmwareStatusNotificationRequest>?  CustomPublishFirmwareStatusNotificationRequestSerializer    { get; set; }

        public CustomJObjectSerializerDelegate<HeartbeatRequest>?                          CustomHeartbeatRequestSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<NotifyEventRequest>?                        CustomNotifyEventRequestSerializer                          { get; set; }

        public CustomJObjectSerializerDelegate<SecurityEventNotificationRequest>?          CustomSecurityEventNotificationSerializer                   { get; set; }

        public CustomJObjectSerializerDelegate<NotifyReportRequest>?                       CustomNotifyReportRequestSerializer                         { get; set; }

        public CustomJObjectSerializerDelegate<NotifyMonitoringReportRequest>?             CustomNotifyMonitoringReportRequestSerializer               { get; set; }

        public CustomJObjectSerializerDelegate<LogStatusNotificationRequest>?              CustomLogStatusNotificationSerializer                       { get; set; }

        public CustomJObjectSerializerDelegate<DataTransferRequest>?                       CustomDataTransferRequestSerializer                         { get; set; }

        public CustomJObjectSerializerDelegate<SignCertificateRequest>?                    CustomSignCertificateRequestSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<Get15118EVCertificateRequest>?              CustomGet15118EVCertificateSerializer                       { get; set; }

        public CustomJObjectSerializerDelegate<GetCertificateStatusRequest>?               CustomGetCertificateStatusSerializer                        { get; set; }

        public CustomJObjectSerializerDelegate<GetCRLRequest>?                             CustomGetCRLSerializer                                      { get; set; }

        public CustomJObjectSerializerDelegate<ReservationStatusUpdateRequest>?            CustomReservationStatusUpdateRequestSerializer              { get; set; }

        public CustomJObjectSerializerDelegate<AuthorizeRequest>?                          CustomAuthorizeRequestSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<NotifyEVChargingNeedsRequest>?              CustomNotifyEVChargingNeedsRequestSerializer                { get; set; }

        public CustomJObjectSerializerDelegate<TransactionEventRequest>?                   CustomTransactionEventRequestSerializer                     { get; set; }

        public CustomJObjectSerializerDelegate<StatusNotificationRequest>?                 CustomStatusNotificationRequestSerializer                   { get; set; }

        public CustomJObjectSerializerDelegate<MeterValuesRequest>?                        CustomMeterValuesRequestSerializer                          { get; set; }

        public CustomJObjectSerializerDelegate<NotifyChargingLimitRequest>?                CustomNotifyChargingLimitRequestSerializer                  { get; set; }

        public CustomJObjectSerializerDelegate<ClearedChargingLimitRequest>?               CustomClearedChargingLimitRequestSerializer                 { get; set; }

        public CustomJObjectSerializerDelegate<ReportChargingProfilesRequest>?             CustomReportChargingProfilesRequestSerializer               { get; set; }

        public CustomJObjectSerializerDelegate<NotifyEVChargingScheduleRequest>?           CustomNotifyEVChargingScheduleRequestSerializer             { get; set; }

        public CustomJObjectSerializerDelegate<NotifyPriorityChargingRequest>?             CustomNotifyPriorityChargingRequestSerializer               { get; set; }

        public CustomJObjectSerializerDelegate<PullDynamicScheduleUpdateRequest>?          CustomPullDynamicScheduleUpdateRequestSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<NotifyDisplayMessagesRequest>?              CustomNotifyDisplayMessagesRequestSerializer                { get; set; }

        public CustomJObjectSerializerDelegate<NotifyCustomerInformationRequest>?          CustomNotifyCustomerInformationRequestSerializer            { get; set; }


        // Data Structures
        public CustomJObjectSerializerDelegate<ChargingStation>?                           CustomChargingStationSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<CustomData>?                                CustomCustomDataSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<EventData>?                                 CustomEventDataSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<Component>?                                 CustomComponentSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                                      CustomEVSESerializer                                        { get; set; }
        public CustomJObjectSerializerDelegate<Variable>?                                  CustomVariableSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<VariableMonitoring>?                        CustomVariableMonitoringSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<ReportData>?                                CustomReportDataSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<VariableAttribute>?                         CustomVariableAttributeSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<VariableCharacteristics>?                   CustomVariableCharacteristicsSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<MonitoringData>?                            CustomMonitoringDataSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<OCSPRequestData>?                           CustomOCSPRequestDataSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<IdToken>?                                   CustomIdTokenSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalInfo>?                            CustomAdditionalInfoSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<ChargingNeeds>?                             CustomChargingNeedsSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ACChargingParameters>?                      CustomACChargingParametersSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<DCChargingParameters>?                      CustomDCChargingParametersSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<V2XChargingParameters>?                     CustomV2XChargingParametersSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<EVEnergyOffer>?                             CustomEVEnergyOfferSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<EVPowerSchedule>?                           CustomEVPowerScheduleSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<EVPowerScheduleEntry>?                      CustomEVPowerScheduleEntrySerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<EVAbsolutePriceSchedule>?                   CustomEVAbsolutePriceScheduleSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?              CustomEVAbsolutePriceScheduleEntrySerializer                { get; set; }
        public CustomJObjectSerializerDelegate<EVPriceRule>?                               CustomEVPriceRuleSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<Transaction>?                               CustomTransactionSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<MeterValue>?                                CustomMeterValueSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<SampledValue>?                              CustomSampledValueSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<SignedMeterValue>?                          CustomSignedMeterValueSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<UnitsOfMeasure>?                            CustomUnitsOfMeasureSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?                          CustomChargingScheduleSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<LimitBeyondSoC>?                            CustomLimitBeyondSoCSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                    CustomChargingSchedulePeriodSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                          CustomV2XFreqWattEntrySerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                        CustomV2XSignalWattEntrySerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariff>?                               CustomSalesTariffSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariffEntry>?                          CustomSalesTariffEntrySerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<RelativeTimeInterval>?                      CustomRelativeTimeIntervalSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ConsumptionCost>?                           CustomConsumptionCostSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<Cost>?                                      CustomCostSerializer                                        { get; set; }

        public CustomJObjectSerializerDelegate<AbsolutePriceSchedule>?                     CustomAbsolutePriceScheduleSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<PriceRuleStack>?                            CustomPriceRuleStackSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<PriceRule>?                                 CustomPriceRuleSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<TaxRule>?                                   CustomTaxRuleSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<OverstayRuleList>?                          CustomOverstayRuleListSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<OverstayRule>?                              CustomOverstayRuleSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalService>?                         CustomAdditionalServiceSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<PriceLevelSchedule>?                        CustomPriceLevelScheduleSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<PriceLevelScheduleEntry>?                   CustomPriceLevelScheduleEntrySerializer                     { get; set; }

        public CustomJObjectSerializerDelegate<ChargingProfile>?                           CustomChargingProfileSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<MessageInfo>?                               CustomMessageInfoSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<MessageContent>?                            CustomMessageContentSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<CertificateHashData>?                       CustomCertificateHashDataSerializer                         { get; set; }

        #endregion

        #region Events

        public event OnWebSocketClientTextMessageResponseDelegate?    OnTextMessageResponseReceived;
        public event OnWebSocketClientTextMessageResponseDelegate?    OnTextMessageResponseSent;

        public event OnWebSocketClientBinaryMessageResponseDelegate?  OnBinaryMessageResponseReceived;
        public event OnWebSocketClientBinaryMessageResponseDelegate?  OnBinaryMessageResponseSent;


        #region Charging Station -> CSMS

        #region OnBootNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a boot notification request will be sent to the CSMS.
        /// </summary>
        public event OnBootNotificationRequestDelegate?     OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a boot notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?               OnBootNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?              OnBootNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        public event OnBootNotificationResponseDelegate?    OnBootNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a firmware status notification request will be sent to the CSMS.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?     OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a firmware status notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                         OnFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                        OnFirmwareStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?    OnFirmwareStatusNotificationResponse;

        #endregion

        #region OnPublishFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a publish firmware status notification request will be sent to the CSMS.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationRequestDelegate?     OnPublishFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a publish firmware status notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                                OnPublishFirmwareStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a publish firmware status notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                               OnPublishFirmwareStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a publish firmware status notification request was received.
        /// </summary>
        public event OnPublishFirmwareStatusNotificationResponseDelegate?    OnPublishFirmwareStatusNotificationResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat request will be sent to the CSMS.
        /// </summary>
        public event OnHeartbeatRequestDelegate?     OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a heartbeat request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?        OnHeartbeatWSRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        public event ClientResponseLogHandler?       OnHeartbeatWSResponse;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        public event OnHeartbeatResponseDelegate?    OnHeartbeatResponse;

        #endregion

        #region OnNotifyEventRequest/-Response

        /// <summary>
        /// An event fired whenever a notify event request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyEventRequestDelegate?     OnNotifyEventRequest;

        /// <summary>
        /// An event fired whenever a notify event request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?          OnNotifyEventWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify event request was received.
        /// </summary>
        public event ClientResponseLogHandler?         OnNotifyEventWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify event request was received.
        /// </summary>
        public event OnNotifyEventResponseDelegate?    OnNotifyEventResponse;

        #endregion

        #region OnSecurityEventNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a security event notification request will be sent to the CSMS.
        /// </summary>
        public event OnSecurityEventNotificationRequestDelegate?     OnSecurityEventNotificationRequest;

        /// <summary>
        /// An event fired whenever a security event notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                        OnSecurityEventNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a security event notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                       OnSecurityEventNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a security event notification request was received.
        /// </summary>
        public event OnSecurityEventNotificationResponseDelegate?    OnSecurityEventNotificationResponse;

        #endregion

        #region OnNotifyReportRequest/-Response

        /// <summary>
        /// An event fired whenever a notify report request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyReportRequestDelegate?     OnNotifyReportRequest;

        /// <summary>
        /// An event fired whenever a notify report request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?           OnNotifyReportWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify report request was received.
        /// </summary>
        public event ClientResponseLogHandler?          OnNotifyReportWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify report request was received.
        /// </summary>
        public event OnNotifyReportResponseDelegate?    OnNotifyReportResponse;

        #endregion

        #region OnNotifyMonitoringReportRequest/-Response

        /// <summary>
        /// An event fired whenever a notify monitoring report request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyMonitoringReportRequestDelegate?     OnNotifyMonitoringReportRequest;

        /// <summary>
        /// An event fired whenever a notify monitoring report request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                     OnNotifyMonitoringReportWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify monitoring report request was received.
        /// </summary>
        public event ClientResponseLogHandler?                    OnNotifyMonitoringReportWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify monitoring report request was received.
        /// </summary>
        public event OnNotifyMonitoringReportResponseDelegate?    OnNotifyMonitoringReportResponse;

        #endregion

        #region OnLogStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a log status notification request will be sent to the CSMS.
        /// </summary>
        public event OnLogStatusNotificationRequestDelegate?     OnLogStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a log status notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                    OnLogStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a log status notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                   OnLogStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a log status notification request was received.
        /// </summary>
        public event OnLogStatusNotificationResponseDelegate?    OnLogStatusNotificationResponse;

        #endregion

        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer request will be sent to the CSMS.
        /// </summary>
        public event OnDataTransferRequestDelegate?     OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a data transfer request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?           OnDataTransferWSRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        public event ClientResponseLogHandler?          OnDataTransferWSResponse;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        public event OnDataTransferResponseDelegate?    OnDataTransferResponse;

        #endregion


        #region OnSignCertificateRequest/-Response

        /// <summary>
        /// An event fired whenever a sign certificate request will be sent to the CSMS.
        /// </summary>
        public event OnSignCertificateRequestDelegate?     OnSignCertificateRequest;

        /// <summary>
        /// An event fired whenever a sign certificate request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?              OnSignCertificateWSRequest;

        /// <summary>
        /// An event fired whenever a response to a sign certificate request was received.
        /// </summary>
        public event ClientResponseLogHandler?             OnSignCertificateWSResponse;

        /// <summary>
        /// An event fired whenever a response to a sign certificate request was received.
        /// </summary>
        public event OnSignCertificateResponseDelegate?    OnSignCertificateResponse;

        #endregion

        #region OnGet15118EVCertificateRequest/-Response

        /// <summary>
        /// An event fired whenever a get 15118 EV certificate request will be sent to the CSMS.
        /// </summary>
        public event OnGet15118EVCertificateRequestDelegate?     OnGet15118EVCertificateRequest;

        /// <summary>
        /// An event fired whenever a get 15118 EV certificate request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                    OnGet15118EVCertificateWSRequest;

        /// <summary>
        /// An event fired whenever a response to a get 15118 EV certificate request was received.
        /// </summary>
        public event ClientResponseLogHandler?                   OnGet15118EVCertificateWSResponse;

        /// <summary>
        /// An event fired whenever a response to a get 15118 EV certificate request was received.
        /// </summary>
        public event OnGet15118EVCertificateResponseDelegate?    OnGet15118EVCertificateResponse;

        #endregion

        #region OnGetCertificateStatusRequest/-Response

        /// <summary>
        /// An event fired whenever a get certificate status request will be sent to the CSMS.
        /// </summary>
        public event OnGetCertificateStatusRequestDelegate?     OnGetCertificateStatusRequest;

        /// <summary>
        /// An event fired whenever a get certificate status request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                   OnGetCertificateStatusWSRequest;

        /// <summary>
        /// An event fired whenever a response to a get certificate status request was received.
        /// </summary>
        public event ClientResponseLogHandler?                  OnGetCertificateStatusWSResponse;

        /// <summary>
        /// An event fired whenever a response to a get certificate status request was received.
        /// </summary>
        public event OnGetCertificateStatusResponseDelegate?    OnGetCertificateStatusResponse;

        #endregion

        #region OnGetCRLRequest/-Response

        /// <summary>
        /// An event fired whenever a get certificate revocation list request will be sent to the CSMS.
        /// </summary>
        public event OnGetCRLRequestDelegate?     OnGetCRLRequest;

        /// <summary>
        /// An event fired whenever a get certificate revocation list request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?     OnGetCRLWSRequest;

        /// <summary>
        /// An event fired whenever a response to a get certificate revocation list request was received.
        /// </summary>
        public event ClientResponseLogHandler?    OnGetCRLWSResponse;

        /// <summary>
        /// An event fired whenever a response to a get certificate revocation list request was received.
        /// </summary>
        public event OnGetCRLResponseDelegate?    OnGetCRLResponse;

        #endregion


        #region OnReservationStatusUpdateRequest/-Response

        /// <summary>
        /// An event fired whenever a reservation status update request will be sent to the CSMS.
        /// </summary>
        public event OnReservationStatusUpdateRequestDelegate?     OnReservationStatusUpdateRequest;

        /// <summary>
        /// An event fired whenever a reservation status update request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                      OnReservationStatusUpdateWSRequest;

        /// <summary>
        /// An event fired whenever a response to a reservation status update request was received.
        /// </summary>
        public event ClientResponseLogHandler?                     OnReservationStatusUpdateWSResponse;

        /// <summary>
        /// An event fired whenever a response to a reservation status update request was received.
        /// </summary>
        public event OnReservationStatusUpdateResponseDelegate?    OnReservationStatusUpdateResponse;

        #endregion

        #region OnAuthorizeRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize request will be sent to the CSMS.
        /// </summary>
        public event OnAuthorizeRequestDelegate?     OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever an authorize request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?        OnAuthorizeWSRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event ClientResponseLogHandler?       OnAuthorizeWSResponse;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event OnAuthorizeResponseDelegate?    OnAuthorizeResponse;

        #endregion

        #region OnNotifyEVChargingNeedsRequest/-Response

        /// <summary>
        /// An event fired whenever a notify EV charging needs request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyEVChargingNeedsRequestDelegate?     OnNotifyEVChargingNeedsRequest;

        /// <summary>
        /// An event fired whenever a notify EV charging needs request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                    OnNotifyEVChargingNeedsWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify EV charging needs request was received.
        /// </summary>
        public event ClientResponseLogHandler?                   OnNotifyEVChargingNeedsWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify EV charging needs request was received.
        /// </summary>
        public event OnNotifyEVChargingNeedsResponseDelegate?    OnNotifyEVChargingNeedsResponse;

        #endregion

        #region OnTransactionEventRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize request will be sent to the CSMS.
        /// </summary>
        public event OnTransactionEventRequestDelegate?     OnTransactionEventRequest;

        /// <summary>
        /// An event fired whenever an authorize request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?               OnTransactionEventWSRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event ClientResponseLogHandler?              OnTransactionEventWSResponse;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event OnTransactionEventResponseDelegate?    OnTransactionEventResponse;

        #endregion

        #region OnStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a status notification request will be sent to the CSMS.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?     OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a status notification request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                 OnStatusNotificationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification request was received.
        /// </summary>
        public event ClientResponseLogHandler?                OnStatusNotificationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a status notification request was received.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?    OnStatusNotificationResponse;

        #endregion

        #region OnMeterValuesRequest/-Response

        /// <summary>
        /// An event fired whenever a meter values request will be sent to the CSMS.
        /// </summary>
        public event OnMeterValuesRequestDelegate?     OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a meter values request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?          OnMeterValuesWSRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values request was received.
        /// </summary>
        public event ClientResponseLogHandler?         OnMeterValuesWSResponse;

        /// <summary>
        /// An event fired whenever a response to a meter values request was received.
        /// </summary>
        public event OnMeterValuesResponseDelegate?    OnMeterValuesResponse;

        #endregion

        #region OnNotifyChargingLimitRequest/-Response

        /// <summary>
        /// An event fired whenever a notify charging limit request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyChargingLimitRequestDelegate?     OnNotifyChargingLimitRequest;

        /// <summary>
        /// An event fired whenever a notify charging limit request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                  OnNotifyChargingLimitWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify charging limit request was received.
        /// </summary>
        public event ClientResponseLogHandler?                 OnNotifyChargingLimitWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify charging limit request was received.
        /// </summary>
        public event OnNotifyChargingLimitResponseDelegate?    OnNotifyChargingLimitResponse;

        #endregion

        #region OnClearedChargingLimitRequest/-Response

        /// <summary>
        /// An event fired whenever a cleared charging limit request will be sent to the CSMS.
        /// </summary>
        public event OnClearedChargingLimitRequestDelegate?     OnClearedChargingLimitRequest;

        /// <summary>
        /// An event fired whenever a cleared charging limit request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                   OnClearedChargingLimitWSRequest;

        /// <summary>
        /// An event fired whenever a response to a cleared charging limit request was received.
        /// </summary>
        public event ClientResponseLogHandler?                  OnClearedChargingLimitWSResponse;

        /// <summary>
        /// An event fired whenever a response to a cleared charging limit request was received.
        /// </summary>
        public event OnClearedChargingLimitResponseDelegate?    OnClearedChargingLimitResponse;

        #endregion

        #region OnReportChargingProfilesRequest/-Response

        /// <summary>
        /// An event fired whenever a ReportChargingProfiles request will be sent to the CSMS.
        /// </summary>
        public event OnReportChargingProfilesRequestDelegate?     OnReportChargingProfilesRequest;

        /// <summary>
        /// An event fired whenever a ReportChargingProfiles request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                     OnReportChargingProfilesWSRequest;

        /// <summary>
        /// An event fired whenever a response to a ReportChargingProfiles request was received.
        /// </summary>
        public event ClientResponseLogHandler?                    OnReportChargingProfilesWSResponse;

        /// <summary>
        /// An event fired whenever a response to a ReportChargingProfiles request was received.
        /// </summary>
        public event OnReportChargingProfilesResponseDelegate?    OnReportChargingProfilesResponse;

        #endregion

        #region OnNotifyEVChargingScheduleRequest/-Response

        /// <summary>
        /// An event fired whenever a NotifyEVChargingSchedule request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyEVChargingScheduleRequestDelegate?     OnNotifyEVChargingScheduleRequest;

        /// <summary>
        /// An event fired whenever a NotifyEVChargingSchedule request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                       OnNotifyEVChargingScheduleWSRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingSchedule request was received.
        /// </summary>
        public event ClientResponseLogHandler?                      OnNotifyEVChargingScheduleWSResponse;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingSchedule request was received.
        /// </summary>
        public event OnNotifyEVChargingScheduleResponseDelegate?    OnNotifyEVChargingScheduleResponse;

        #endregion

        #region OnNotifyPriorityChargingRequest/-Response

        /// <summary>
        /// An event fired whenever a NotifyPriorityCharging request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyPriorityChargingRequestDelegate?     OnNotifyPriorityChargingRequest;

        /// <summary>
        /// An event fired whenever a NotifyPriorityCharging request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                     OnNotifyPriorityChargingWSRequest;

        /// <summary>
        /// An event fired whenever a response to a NotifyPriorityCharging request was received.
        /// </summary>
        public event ClientResponseLogHandler?                    OnNotifyPriorityChargingWSResponse;

        /// <summary>
        /// An event fired whenever a response to a NotifyPriorityCharging request was received.
        /// </summary>
        public event OnNotifyPriorityChargingResponseDelegate?    OnNotifyPriorityChargingResponse;

        #endregion

        #region OnPullDynamicScheduleUpdateRequest/-Response

        /// <summary>
        /// An event fired whenever a PullDynamicScheduleUpdate request will be sent to the CSMS.
        /// </summary>
        public event OnPullDynamicScheduleUpdateRequestDelegate?     OnPullDynamicScheduleUpdateRequest;

        /// <summary>
        /// An event fired whenever a PullDynamicScheduleUpdate request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                        OnPullDynamicScheduleUpdateWSRequest;

        /// <summary>
        /// An event fired whenever a response to a PullDynamicScheduleUpdate request was received.
        /// </summary>
        public event ClientResponseLogHandler?                       OnPullDynamicScheduleUpdateWSResponse;

        /// <summary>
        /// An event fired whenever a response to a PullDynamicScheduleUpdate request was received.
        /// </summary>
        public event OnPullDynamicScheduleUpdateResponseDelegate?    OnPullDynamicScheduleUpdateResponse;

        #endregion


        #region OnNotifyDisplayMessagesRequest/-Response

        /// <summary>
        /// An event fired whenever a notify display messages request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyDisplayMessagesRequestDelegate?     OnNotifyDisplayMessagesRequest;

        /// <summary>
        /// An event fired whenever a notify display messages request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                    OnNotifyDisplayMessagesWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify display messages request was received.
        /// </summary>
        public event ClientResponseLogHandler?                   OnNotifyDisplayMessagesWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify display messages request was received.
        /// </summary>
        public event OnNotifyDisplayMessagesResponseDelegate?    OnNotifyDisplayMessagesResponse;

        #endregion

        #region OnNotifyCustomerInformationRequest/-Response

        /// <summary>
        /// An event fired whenever a notify customer information request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyCustomerInformationRequestDelegate?     OnNotifyCustomerInformationRequest;

        /// <summary>
        /// An event fired whenever a notify customer information request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                        OnNotifyCustomerInformationWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify customer information request was received.
        /// </summary>
        public event ClientResponseLogHandler?                       OnNotifyCustomerInformationWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify customer information request was received.
        /// </summary>
        public event OnNotifyCustomerInformationResponseDelegate?    OnNotifyCustomerInformationResponse;

        #endregion

        #endregion

        #region Charging Station <- CSMS

        #region OnReset

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?     OnResetWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnResetRequestDelegate?        OnResetRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnResetDelegate?               OnReset;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnResetResponseDelegate?       OnResetResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?    OnResetWSResponse;

        #endregion

        #region OnUpdateFirmware

        /// <summary>
        /// An event sent whenever an update firmware websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?           OnUpdateFirmwareWSRequest;

        /// <summary>
        /// An event sent whenever an update firmware request was received.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate?     OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever an update firmware request was received.
        /// </summary>
        public event OnUpdateFirmwareDelegate?            OnUpdateFirmware;

        /// <summary>
        /// An event sent whenever a response to an update firmware request was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate?    OnUpdateFirmwareResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an update firmware request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?          OnUpdateFirmwareWSResponse;

        #endregion

        #region OnPublishFirmware

        /// <summary>
        /// An event sent whenever a publish firmware websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?            OnPublishFirmwareWSRequest;

        /// <summary>
        /// An event sent whenever a publish firmware request was received.
        /// </summary>
        public event OnPublishFirmwareRequestDelegate?     OnPublishFirmwareRequest;

        /// <summary>
        /// An event sent whenever a publish firmware request was received.
        /// </summary>
        public event OnPublishFirmwareDelegate?            OnPublishFirmware;

        /// <summary>
        /// An event sent whenever a response to a publish firmware request was sent.
        /// </summary>
        public event OnPublishFirmwareResponseDelegate?    OnPublishFirmwareResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a publish firmware request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?           OnPublishFirmwareWSResponse;

        #endregion

        #region OnUnpublishFirmware

        /// <summary>
        /// An event sent whenever an unpublish firmware websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnUnpublishFirmwareWSRequest;

        /// <summary>
        /// An event sent whenever an unpublish firmware request was received.
        /// </summary>
        public event OnUnpublishFirmwareRequestDelegate?     OnUnpublishFirmwareRequest;

        /// <summary>
        /// An event sent whenever an unpublish firmware request was received.
        /// </summary>
        public event OnUnpublishFirmwareDelegate?            OnUnpublishFirmware;

        /// <summary>
        /// An event sent whenever a response to an unpublish firmware request was sent.
        /// </summary>
        public event OnUnpublishFirmwareResponseDelegate?    OnUnpublishFirmwareResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an unpublish firmware request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnUnpublishFirmwareWSResponse;

        #endregion

        #region OnGetBaseReport

        /// <summary>
        /// An event sent whenever a get base report websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?          OnGetBaseReportWSRequest;

        /// <summary>
        /// An event sent whenever a get base report request was received.
        /// </summary>
        public event OnGetBaseReportRequestDelegate?     OnGetBaseReportRequest;

        /// <summary>
        /// An event sent whenever a get base report request was received.
        /// </summary>
        public event OnGetBaseReportDelegate?            OnGetBaseReport;

        /// <summary>
        /// An event sent whenever a response to a get base report request was sent.
        /// </summary>
        public event OnGetBaseReportResponseDelegate?    OnGetBaseReportResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get base report request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?         OnGetBaseReportWSResponse;

        #endregion

        #region OnGetReport

        /// <summary>
        /// An event sent whenever a get report websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?      OnGetReportWSRequest;

        /// <summary>
        /// An event sent whenever a get report request was received.
        /// </summary>
        public event OnGetReportRequestDelegate?     OnGetReportRequest;

        /// <summary>
        /// An event sent whenever a get report request was received.
        /// </summary>
        public event OnGetReportDelegate?            OnGetReport;

        /// <summary>
        /// An event sent whenever a response to a get report request was sent.
        /// </summary>
        public event OnGetReportResponseDelegate?    OnGetReportResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get report request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?     OnGetReportWSResponse;

        #endregion

        #region OnGetLog

        /// <summary>
        /// An event sent whenever a get log websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?     OnGetLogWSRequest;

        /// <summary>
        /// An event sent whenever a get log request was received.
        /// </summary>
        public event OnGetLogRequestDelegate?       OnGetLogRequest;

        /// <summary>
        /// An event sent whenever a get log request was received.
        /// </summary>
        public event OnGetLogDelegate?              OnGetLog;

        /// <summary>
        /// An event sent whenever a response to a get log request was sent.
        /// </summary>
        public event OnGetLogResponseDelegate?      OnGetLogResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get log request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?    OnGetLogWSResponse;

        #endregion

        #region OnSetVariables

        /// <summary>
        /// An event sent whenever a set variables websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?         OnSetVariablesWSRequest;

        /// <summary>
        /// An event sent whenever a set variables request was received.
        /// </summary>
        public event OnSetVariablesRequestDelegate?     OnSetVariablesRequest;

        /// <summary>
        /// An event sent whenever a set variables request was received.
        /// </summary>
        public event OnSetVariablesDelegate?            OnSetVariables;

        /// <summary>
        /// An event sent whenever a response to a set variables request was sent.
        /// </summary>
        public event OnSetVariablesResponseDelegate?    OnSetVariablesResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set variables request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?        OnSetVariablesWSResponse;

        #endregion

        #region OnGetVariables

        /// <summary>
        /// An event sent whenever a get variables websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?         OnGetVariablesWSRequest;

        /// <summary>
        /// An event sent whenever a get variables request was received.
        /// </summary>
        public event OnGetVariablesRequestDelegate?     OnGetVariablesRequest;

        /// <summary>
        /// An event sent whenever a get variables request was received.
        /// </summary>
        public event OnGetVariablesDelegate?            OnGetVariables;

        /// <summary>
        /// An event sent whenever a response to a get variables request was sent.
        /// </summary>
        public event OnGetVariablesResponseDelegate?    OnGetVariablesResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get variables request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?        OnGetVariablesWSResponse;

        #endregion

        #region OnSetMonitoringBase

        /// <summary>
        /// An event sent whenever a set monitoring base websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnSetMonitoringBaseWSRequest;

        /// <summary>
        /// An event sent whenever a set monitoring base request was received.
        /// </summary>
        public event OnSetMonitoringBaseRequestDelegate?     OnSetMonitoringBaseRequest;

        /// <summary>
        /// An event sent whenever a set monitoring base request was received.
        /// </summary>
        public event OnSetMonitoringBaseDelegate?            OnSetMonitoringBase;

        /// <summary>
        /// An event sent whenever a response to a set monitoring base request was sent.
        /// </summary>
        public event OnSetMonitoringBaseResponseDelegate?    OnSetMonitoringBaseResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set monitoring base request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnSetMonitoringBaseWSResponse;

        #endregion

        #region OnGetMonitoringReport

        /// <summary>
        /// An event sent whenever a get monitoring report websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                OnGetMonitoringReportWSRequest;

        /// <summary>
        /// An event sent whenever a get monitoring report request was received.
        /// </summary>
        public event OnGetMonitoringReportRequestDelegate?     OnGetMonitoringReportRequest;

        /// <summary>
        /// An event sent whenever a get monitoring report request was received.
        /// </summary>
        public event OnGetMonitoringReportDelegate?            OnGetMonitoringReport;

        /// <summary>
        /// An event sent whenever a response to a get monitoring report request was sent.
        /// </summary>
        public event OnGetMonitoringReportResponseDelegate?    OnGetMonitoringReportResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get monitoring report request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?               OnGetMonitoringReportWSResponse;

        #endregion

        #region OnSetMonitoringLevel

        /// <summary>
        /// An event sent whenever a set monitoring level websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?               OnSetMonitoringLevelWSRequest;

        /// <summary>
        /// An event sent whenever a set monitoring level request was received.
        /// </summary>
        public event OnSetMonitoringLevelRequestDelegate?     OnSetMonitoringLevelRequest;

        /// <summary>
        /// An event sent whenever a set monitoring level request was received.
        /// </summary>
        public event OnSetMonitoringLevelDelegate?            OnSetMonitoringLevel;

        /// <summary>
        /// An event sent whenever a response to a set monitoring level request was sent.
        /// </summary>
        public event OnSetMonitoringLevelResponseDelegate?    OnSetMonitoringLevelResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set monitoring level request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?              OnSetMonitoringLevelWSResponse;

        #endregion

        #region OnSetVariableMonitoring

        /// <summary>
        /// An event sent whenever a set variable monitoring websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                  OnSetVariableMonitoringWSRequest;

        /// <summary>
        /// An event sent whenever a set variable monitoring request was received.
        /// </summary>
        public event OnSetVariableMonitoringRequestDelegate?     OnSetVariableMonitoringRequest;

        /// <summary>
        /// An event sent whenever a set variable monitoring request was received.
        /// </summary>
        public event OnSetVariableMonitoringDelegate?            OnSetVariableMonitoring;

        /// <summary>
        /// An event sent whenever a response to a set variable monitoring request was sent.
        /// </summary>
        public event OnSetVariableMonitoringResponseDelegate?    OnSetVariableMonitoringResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set variable monitoring request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                 OnSetVariableMonitoringWSResponse;

        #endregion

        #region OnClearVariableMonitoring

        /// <summary>
        /// An event sent whenever a clear variable monitoring websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                    OnClearVariableMonitoringWSRequest;

        /// <summary>
        /// An event sent whenever a clear variable monitoring request was received.
        /// </summary>
        public event OnClearVariableMonitoringRequestDelegate?     OnClearVariableMonitoringRequest;

        /// <summary>
        /// An event sent whenever a clear variable monitoring request was received.
        /// </summary>
        public event OnClearVariableMonitoringDelegate?            OnClearVariableMonitoring;

        /// <summary>
        /// An event sent whenever a response to a clear variable monitoring request was sent.
        /// </summary>
        public event OnClearVariableMonitoringResponseDelegate?    OnClearVariableMonitoringResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a clear variable monitoring request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                   OnClearVariableMonitoringWSResponse;

        #endregion

        #region OnSetNetworkProfile

        /// <summary>
        /// An event sent whenever a set network profile websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnSetNetworkProfileWSRequest;

        /// <summary>
        /// An event sent whenever a set network profile request was received.
        /// </summary>
        public event OnSetNetworkProfileRequestDelegate?     OnSetNetworkProfileRequest;

        /// <summary>
        /// An event sent whenever a set network profile request was received.
        /// </summary>
        public event OnSetNetworkProfileDelegate?            OnSetNetworkProfile;

        /// <summary>
        /// An event sent whenever a response to a set network profile request was sent.
        /// </summary>
        public event OnSetNetworkProfileResponseDelegate?    OnSetNetworkProfileResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set network profile request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnSetNetworkProfileWSResponse;

        #endregion

        #region OnChangeAvailability

        /// <summary>
        /// An event sent whenever a change availability websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?               OnChangeAvailabilityWSRequest;

        /// <summary>
        /// An event sent whenever a change availability request was received.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate?     OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a change availability request was received.
        /// </summary>
        public event OnChangeAvailabilityDelegate?            OnChangeAvailability;

        /// <summary>
        /// An event sent whenever a response to a change availability request was sent.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate?    OnChangeAvailabilityResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a change availability request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?              OnChangeAvailabilityWSResponse;

        #endregion

        #region OnTriggerMessage

        /// <summary>
        /// An event sent whenever a trigger message websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?           OnTriggerMessageWSRequest;

        /// <summary>
        /// An event sent whenever a trigger message request was received.
        /// </summary>
        public event OnTriggerMessageRequestDelegate?     OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a trigger message request was received.
        /// </summary>
        public event OnTriggerMessageDelegate?            OnTriggerMessage;

        /// <summary>
        /// An event sent whenever a response to a trigger message request was sent.
        /// </summary>
        public event OnTriggerMessageResponseDelegate?    OnTriggerMessageResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a trigger message request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?          OnTriggerMessageWSResponse;

        #endregion

        #region OnIncomingDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                 OnIncomingDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate?     OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferDelegate?            OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate?    OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a data transfer request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                OnIncomingDataTransferWSResponse;

        #endregion


        #region OnCertificateSigned

        /// <summary>
        /// An event sent whenever a certificate signed websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnCertificateSignedWSRequest;

        /// <summary>
        /// An event sent whenever a certificate signed request was received.
        /// </summary>
        public event OnCertificateSignedRequestDelegate?     OnCertificateSignedRequest;

        /// <summary>
        /// An event sent whenever a certificate signed request was received.
        /// </summary>
        public event OnCertificateSignedDelegate?            OnCertificateSigned;

        /// <summary>
        /// An event sent whenever a response to a certificate signed request was sent.
        /// </summary>
        public event OnCertificateSignedResponseDelegate?    OnCertificateSignedResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a certificate signed request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnCertificateSignedWSResponse;

        #endregion

        #region OnInstallCertificate

        /// <summary>
        /// An event sent whenever an install certificate websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                  OnInstallCertificateWSRequest;

        /// <summary>
        /// An event sent whenever an install certificate request was received.
        /// </summary>
        public event OnInstallCertificateRequestDelegate?        OnInstallCertificateRequest;

        /// <summary>
        /// An event sent whenever an install certificate request was received.
        /// </summary>
        public event OnInstallCertificateDelegate?               OnInstallCertificate;

        /// <summary>
        /// An event sent whenever a response to an install certificate request was sent.
        /// </summary>
        public event OnInstallCertificateResponseDelegate?       OnInstallCertificateResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an install certificate request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                 OnInstallCertificateWSResponse;

        #endregion

        #region OnGetInstalledCertificateIds

        /// <summary>
        /// An event sent whenever a get installed certificate ids websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                       OnGetInstalledCertificateIdsWSRequest;

        /// <summary>
        /// An event sent whenever a get installed certificate ids request was received.
        /// </summary>
        public event OnGetInstalledCertificateIdsRequestDelegate?     OnGetInstalledCertificateIdsRequest;

        /// <summary>
        /// An event sent whenever a get installed certificate ids request was received.
        /// </summary>
        public event OnGetInstalledCertificateIdsDelegate?            OnGetInstalledCertificateIds;

        /// <summary>
        /// An event sent whenever a response to a get installed certificate ids request was sent.
        /// </summary>
        public event OnGetInstalledCertificateIdsResponseDelegate?    OnGetInstalledCertificateIdsResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get installed certificate ids request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                      OnGetInstalledCertificateIdsWSResponse;

        #endregion

        #region OnDeleteCertificate

        /// <summary>
        /// An event sent whenever a delete certificate websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnDeleteCertificateWSRequest;

        /// <summary>
        /// An event sent whenever a delete certificate request was received.
        /// </summary>
        public event OnDeleteCertificateRequestDelegate?     OnDeleteCertificateRequest;

        /// <summary>
        /// An event sent whenever a delete certificate request was received.
        /// </summary>
        public event OnDeleteCertificateDelegate?            OnDeleteCertificate;

        /// <summary>
        /// An event sent whenever a response to a delete certificate request was sent.
        /// </summary>
        public event OnDeleteCertificateResponseDelegate?    OnDeleteCertificateResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a delete certificate request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnDeleteCertificateWSResponse;

        #endregion

        #region OnNotifyCRL

        /// <summary>
        /// An event sent whenever a NotifyCRL websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?      OnNotifyCRLWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyCRL request was received.
        /// </summary>
        public event OnNotifyCRLRequestDelegate?     OnNotifyCRLRequest;

        /// <summary>
        /// An event sent whenever a NotifyCRL request was received.
        /// </summary>
        public event OnNotifyCRLDelegate?            OnNotifyCRL;

        /// <summary>
        /// An event sent whenever a response to a NotifyCRL request was sent.
        /// </summary>
        public event OnNotifyCRLResponseDelegate?    OnNotifyCRLResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a NotifyCRL request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?     OnNotifyCRLWSResponse;

        #endregion


        #region OnGetLocalListVersion

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                OnGetLocalListVersionWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate?     OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetLocalListVersionDelegate?            OnGetLocalListVersion;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate?    OnGetLocalListVersionResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?               OnGetLocalListVersionWSResponse;

        #endregion

        #region OnSendLocalList

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?          OnSendLocalListWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSendLocalListRequestDelegate?     OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSendLocalListDelegate?            OnSendLocalList;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnSendLocalListResponseDelegate?    OnSendLocalListResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?         OnSendLocalListWSResponse;

        #endregion

        #region OnClearCache

        /// <summary>
        /// An event sent whenever a reset websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?       OnClearCacheWSRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearCacheRequestDelegate?     OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearCacheDelegate?            OnClearCache;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnClearCacheResponseDelegate?    OnClearCacheResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reset request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?      OnClearCacheWSResponse;

        #endregion


        #region OnReserveNow

        /// <summary>
        /// An event sent whenever a reserve now websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?       OnReserveNowWSRequest;

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        public event OnReserveNowRequestDelegate?     OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        public event OnReserveNowDelegate?            OnReserveNow;

        /// <summary>
        /// An event sent whenever a response to a reserve now request was sent.
        /// </summary>
        public event OnReserveNowResponseDelegate?    OnReserveNowResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a reserve now request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?      OnReserveNowWSResponse;

        #endregion

        #region OnCancelReservation

        /// <summary>
        /// An event sent whenever a cancel reservation websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnCancelReservationWSRequest;

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationRequestDelegate?     OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationDelegate?            OnCancelReservation;

        /// <summary>
        /// An event sent whenever a response to a cancel reservation request was sent.
        /// </summary>
        public event OnCancelReservationResponseDelegate?    OnCancelReservationResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a cancel reservation request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnCancelReservationWSResponse;

        #endregion

        #region OnRequestStartTransaction

        /// <summary>
        /// An event sent whenever a request start transaction websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                    OnRequestStartTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a request start transaction request was received.
        /// </summary>
        public event OnRequestStartTransactionRequestDelegate?     OnRequestStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a request start transaction request was received.
        /// </summary>
        public event OnRequestStartTransactionDelegate?            OnRequestStartTransaction;

        /// <summary>
        /// An event sent whenever a response to a request start transaction request was sent.
        /// </summary>
        public event OnRequestStartTransactionResponseDelegate?    OnRequestStartTransactionResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a request start transaction request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                   OnRequestStartTransactionWSResponse;

        #endregion

        #region OnRequestStopTransaction

        /// <summary>
        /// An event sent whenever a request stop transaction websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                   OnRequestStopTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a request stop transaction request was received.
        /// </summary>
        public event OnRequestStopTransactionRequestDelegate?     OnRequestStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a request stop transaction request was received.
        /// </summary>
        public event OnRequestStopTransactionDelegate?            OnRequestStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a request stop transaction request was sent.
        /// </summary>
        public event OnRequestStopTransactionResponseDelegate?    OnRequestStopTransactionResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a request stop transaction request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                  OnRequestStopTransactionWSResponse;

        #endregion

        #region OnGetTransactionStatus

        /// <summary>
        /// An event sent whenever a get transaction status websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                 OnGetTransactionStatusWSRequest;

        /// <summary>
        /// An event sent whenever a get transaction status request was received.
        /// </summary>
        public event OnGetTransactionStatusRequestDelegate?     OnGetTransactionStatusRequest;

        /// <summary>
        /// An event sent whenever a get transaction status request was received.
        /// </summary>
        public event OnGetTransactionStatusDelegate?            OnGetTransactionStatus;

        /// <summary>
        /// An event sent whenever a response to a get transaction status request was sent.
        /// </summary>
        public event OnGetTransactionStatusResponseDelegate?    OnGetTransactionStatusResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get transaction status request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                OnGetTransactionStatusWSResponse;

        #endregion

        #region OnSetChargingProfile

        /// <summary>
        /// An event sent whenever a set charging profile websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?               OnSetChargingProfileWSRequest;

        /// <summary>
        /// An event sent whenever a set charging profile request was received.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate?     OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a set charging profile request was received.
        /// </summary>
        public event OnSetChargingProfileDelegate?            OnSetChargingProfile;

        /// <summary>
        /// An event sent whenever a response to a set charging profile request was sent.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate?    OnSetChargingProfileResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set charging profile request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?              OnSetChargingProfileWSResponse;

        #endregion

        #region OnGetChargingProfiles

        /// <summary>
        /// An event sent whenever a get charging profiles websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                OnGetChargingProfilesWSRequest;

        /// <summary>
        /// An event sent whenever a get charging profiles request was received.
        /// </summary>
        public event OnGetChargingProfilesRequestDelegate?     OnGetChargingProfilesRequest;

        /// <summary>
        /// An event sent whenever a get charging profiles request was received.
        /// </summary>
        public event OnGetChargingProfilesDelegate?            OnGetChargingProfiles;

        /// <summary>
        /// An event sent whenever a response to a get charging profiles request was sent.
        /// </summary>
        public event OnGetChargingProfilesResponseDelegate?    OnGetChargingProfilesResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get charging profiles request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?               OnGetChargingProfilesWSResponse;

        #endregion

        #region OnClearChargingProfile

        /// <summary>
        /// An event sent whenever a clear charging profile websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                 OnClearChargingProfileWSRequest;

        /// <summary>
        /// An event sent whenever a clear charging profile request was received.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate?     OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a clear charging profile request was received.
        /// </summary>
        public event OnClearChargingProfileDelegate?            OnClearChargingProfile;

        /// <summary>
        /// An event sent whenever a response to a clear charging profile request was sent.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate?    OnClearChargingProfileResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a clear charging profile request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                OnClearChargingProfileWSResponse;

        #endregion

        #region OnGetCompositeSchedule

        /// <summary>
        /// An event sent whenever a GetCompositeSchedule websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                 OnGetCompositeScheduleWSRequest;

        /// <summary>
        /// An event sent whenever a GetCompositeSchedule request was received.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate?     OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a GetCompositeSchedule request was received.
        /// </summary>
        public event OnGetCompositeScheduleDelegate?            OnGetCompositeSchedule;

        /// <summary>
        /// An event sent whenever a response to a GetCompositeSchedule request was sent.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate?    OnGetCompositeScheduleResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a GetCompositeSchedule request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                OnGetCompositeScheduleWSResponse;

        #endregion

        #region OnUpdateDynamicSchedule

        /// <summary>
        /// An event sent whenever an UpdateDynamicSchedule websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                  OnUpdateDynamicScheduleWSRequest;

        /// <summary>
        /// An event sent whenever an UpdateDynamicSchedule request was received.
        /// </summary>
        public event OnUpdateDynamicScheduleRequestDelegate?     OnUpdateDynamicScheduleRequest;

        /// <summary>
        /// An event sent whenever an UpdateDynamicSchedule request was received.
        /// </summary>
        public event OnUpdateDynamicScheduleDelegate?            OnUpdateDynamicSchedule;

        /// <summary>
        /// An event sent whenever a response to an UpdateDynamicSchedule request was sent.
        /// </summary>
        public event OnUpdateDynamicScheduleResponseDelegate?    OnUpdateDynamicScheduleResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an UpdateDynamicSchedule request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                 OnUpdateDynamicScheduleWSResponse;

        #endregion

        #region OnNotifyAllowedEnergyTransfer

        /// <summary>
        /// An event sent whenever a NotifyAllowedEnergyTransfer websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                        OnNotifyAllowedEnergyTransferWSRequest;

        /// <summary>
        /// An event sent whenever a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferRequestDelegate?     OnNotifyAllowedEnergyTransferRequest;

        /// <summary>
        /// An event sent whenever a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferDelegate?            OnNotifyAllowedEnergyTransfer;

        /// <summary>
        /// An event sent whenever a response to a NotifyAllowedEnergyTransfer request was sent.
        /// </summary>
        public event OnNotifyAllowedEnergyTransferResponseDelegate?    OnNotifyAllowedEnergyTransferResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a NotifyAllowedEnergyTransfer request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?                       OnNotifyAllowedEnergyTransferWSResponse;

        #endregion

        #region OnUsePriorityCharging

        /// <summary>
        /// An event sent whenever an UsePriorityCharging websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                OnUsePriorityChargingWSRequest;

        /// <summary>
        /// An event sent whenever an UsePriorityCharging request was received.
        /// </summary>
        public event OnUsePriorityChargingRequestDelegate?     OnUsePriorityChargingRequest;

        /// <summary>
        /// An event sent whenever an UsePriorityCharging request was received.
        /// </summary>
        public event OnUsePriorityChargingDelegate?            OnUsePriorityCharging;

        /// <summary>
        /// An event sent whenever a response to an UsePriorityCharging request was sent.
        /// </summary>
        public event OnUsePriorityChargingResponseDelegate?    OnUsePriorityChargingResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an UsePriorityCharging request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?               OnUsePriorityChargingWSResponse;

        #endregion

        #region OnUnlockConnector

        /// <summary>
        /// An event sent whenever an UnlockConnector websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?            OnUnlockConnectorWSRequest;

        /// <summary>
        /// An event sent whenever an UnlockConnector request was received.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate?     OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever an UnlockConnector request was received.
        /// </summary>
        public event OnUnlockConnectorDelegate?            OnUnlockConnector;

        /// <summary>
        /// An event sent whenever a response to an UnlockConnector request was sent.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate?    OnUnlockConnectorResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an UnlockConnector request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?           OnUnlockConnectorWSResponse;

        #endregion


        #region OnAFRRSignal

        /// <summary>
        /// An event sent whenever an AFRRSignal websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?       OnAFRRSignalWSRequest;

        /// <summary>
        /// An event sent whenever an AFRRSignal request was received.
        /// </summary>
        public event OnAFRRSignalRequestDelegate?     OnAFRRSignalRequest;

        /// <summary>
        /// An event sent whenever an AFRRSignal request was received.
        /// </summary>
        public event OnAFRRSignalDelegate?            OnAFRRSignal;

        /// <summary>
        /// An event sent whenever a response to an AFRRSignal request was sent.
        /// </summary>
        public event OnAFRRSignalResponseDelegate?    OnAFRRSignalResponse;

        /// <summary>
        /// An event sent whenever a websocket response to an AFRRSignal request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?      OnAFRRSignalWSResponse;

        #endregion


        #region OnSetDisplayMessage

        /// <summary>
        /// An event sent whenever a set display message websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?              OnSetDisplayMessageWSRequest;

        /// <summary>
        /// An event sent whenever a set display message request was received.
        /// </summary>
        public event OnSetDisplayMessageRequestDelegate?     OnSetDisplayMessageRequest;

        /// <summary>
        /// An event sent whenever a set display message request was received.
        /// </summary>
        public event OnSetDisplayMessageDelegate?            OnSetDisplayMessage;

        /// <summary>
        /// An event sent whenever a response to a set display message request was sent.
        /// </summary>
        public event OnSetDisplayMessageResponseDelegate?    OnSetDisplayMessageResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a set display message request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?             OnSetDisplayMessageWSResponse;

        #endregion

        #region OnGetDisplayMessages

        /// <summary>
        /// An event sent whenever a get display messages websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?               OnGetDisplayMessagesWSRequest;

        /// <summary>
        /// An event sent whenever a get display messages request was received.
        /// </summary>
        public event OnGetDisplayMessagesRequestDelegate?     OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event sent whenever a get display messages request was received.
        /// </summary>
        public event OnGetDisplayMessagesDelegate?            OnGetDisplayMessages;

        /// <summary>
        /// An event sent whenever a response to a get display messages request was sent.
        /// </summary>
        public event OnGetDisplayMessagesResponseDelegate?    OnGetDisplayMessagesResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a get display messages request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?              OnGetDisplayMessagesWSResponse;

        #endregion

        #region OnClearDisplayMessage

        /// <summary>
        /// An event sent whenever a clear display message websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                OnClearDisplayMessageWSRequest;

        /// <summary>
        /// An event sent whenever a clear display message request was received.
        /// </summary>
        public event OnClearDisplayMessageRequestDelegate?     OnClearDisplayMessageRequest;

        /// <summary>
        /// An event sent whenever a clear display message request was received.
        /// </summary>
        public event OnClearDisplayMessageDelegate?            OnClearDisplayMessage;

        /// <summary>
        /// An event sent whenever a response to a clear display message request was sent.
        /// </summary>
        public event OnClearDisplayMessageResponseDelegate?    OnClearDisplayMessageResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a clear display message request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?               OnClearDisplayMessageWSResponse;

        #endregion

        #region OnCostUpdated

        /// <summary>
        /// An event sent whenever a cost updated websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?        OnCostUpdatedWSRequest;

        /// <summary>
        /// An event sent whenever a cost updated request was received.
        /// </summary>
        public event OnCostUpdatedRequestDelegate?     OnCostUpdatedRequest;

        /// <summary>
        /// An event sent whenever a cost updated request was received.
        /// </summary>
        public event OnCostUpdatedDelegate?            OnCostUpdated;

        /// <summary>
        /// An event sent whenever a response to a cost updated request was sent.
        /// </summary>
        public event OnCostUpdatedResponseDelegate?    OnCostUpdatedResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a cost updated request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?       OnCostUpdatedWSResponse;

        #endregion

        #region OnCustomerInformation

        /// <summary>
        /// An event sent whenever a customer information websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?                OnCustomerInformationWSRequest;

        /// <summary>
        /// An event sent whenever a customer information request was received.
        /// </summary>
        public event OnCustomerInformationRequestDelegate?     OnCustomerInformationRequest;

        /// <summary>
        /// An event sent whenever a customer information request was received.
        /// </summary>
        public event OnCustomerInformationDelegate?            OnCustomerInformation;

        /// <summary>
        /// An event sent whenever a response to a customer information request was sent.
        /// </summary>
        public event OnCustomerInformationResponseDelegate?    OnCustomerInformationResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a customer information request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?               OnCustomerInformationWSResponse;

        #endregion

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging station websocket client running on a charging station
        /// and connecting to a CSMS to invoke methods.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of this charge box.</param>
        /// <param name="From">The source URI of the websocket message.</param>
        /// <param name="To">The destination URI of the websocket message.</param>
        /// 
        /// <param name="RemoteURL">The remote URL of the HTTP endpoint to connect to.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this HTTP/websocket client.</param>
        /// <param name="RemoteCertificateValidator">The remote SSL/TLS certificate validator.</param>
        /// <param name="ClientCertificateSelector">A delegate to select a TLS client certificate.</param>
        /// <param name="ClientCert">The SSL/TLS client certificate to use of HTTP authentication.</param>
        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
        /// <param name="URLPathPrefix">An optional default URL path prefix.</param>
        /// <param name="HTTPAuthentication">The WebService-Security username/password.</param>
        /// <param name="RequestTimeout">An optional Request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
        /// <param name="UseHTTPPipelining">Whether to pipeline multiple HTTP Request through a single HTTP/TCP connection.</param>
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="LoggingContext">An optional context for logging client methods.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public ChargingStationWSClient(ChargeBox_Id                         ChargeBoxIdentity,
                                       String                               From,
                                       String                               To,

                                       URL                                  RemoteURL,
                                       HTTPHostname?                        VirtualHostname              = null,
                                       String?                              Description                  = null,
                                       Boolean?                             PreferIPv4                   = null,
                                       RemoteCertificateValidationHandler?  RemoteCertificateValidator   = null,
                                       LocalCertificateSelectionHandler?    ClientCertificateSelector    = null,
                                       X509Certificate?                     ClientCert                   = null,
                                       SslProtocols?                        TLSProtocol                  = null,
                                       String                               HTTPUserAgent                = DefaultHTTPUserAgent,
                                       IHTTPAuthentication?                 HTTPAuthentication           = null,
                                       TimeSpan?                            RequestTimeout               = null,
                                       TransmissionRetryDelayDelegate?      TransmissionRetryDelay       = null,
                                       UInt16?                              MaxNumberOfRetries           = 3,
                                       UInt32?                              InternalBufferSize           = null,

                                       IEnumerable<String>?                 SecWebSocketProtocols        = null,

                                       Boolean                              DisableWebSocketPings        = false,
                                       TimeSpan?                            WebSocketPingEvery           = null,
                                       TimeSpan?                            SlowNetworkSimulationDelay   = null,

                                       Boolean                              DisableMaintenanceTasks      = false,
                                       TimeSpan?                            MaintenanceEvery             = null,

                                       String?                              LoggingPath                  = null,
                                       String                               LoggingContext               = null, //CPClientLogger.DefaultContext,
                                       LogfileCreatorDelegate?              LogfileCreator               = null,
                                       HTTPClientLogger?                    HTTPLogger                   = null,
                                       DNSClient?                           DNSClient                    = null)

            : base(RemoteURL,
                   VirtualHostname,
                   Description,
                   PreferIPv4,
                   RemoteCertificateValidator,
                   ClientCertificateSelector,
                   ClientCert,
                   TLSProtocol,
                   HTTPUserAgent,
                   HTTPAuthentication,
                   RequestTimeout ?? DefaultRequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,

                   SecWebSocketProtocols,

                   DisableWebSocketPings,
                   WebSocketPingEvery,
                   SlowNetworkSimulationDelay,

                   DisableMaintenanceTasks,
                   MaintenanceEvery,

                   LoggingPath,
                   LoggingContext,
                   LogfileCreator,
                   HTTPLogger,
                   DNSClient)

        {

            #region Initial checks

            if (ChargeBoxIdentity.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null or empty!");

            if (From.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(From),               "The given websocket message source must not be null or empty!");

            if (To.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(To),                 "The given websocket message destination must not be null or empty!");

            #endregion

            this.ChargeBoxIdentity                  = ChargeBoxIdentity;
            this.From                               = From;
            this.To                                 = To;

            //this.Logger                             = new ChargePointwebsocketClient.CPClientLogger(this,
            //                                                                                   LoggingPath,
            //                                                                                   LoggingContext,
            //                                                                                   LogfileCreator);

        }

        #endregion


        #region ProcessWebSocketTextFrame(frame)

        public override async Task ProcessWebSocketTextFrame(WebSocketFrame frame)
        {

            var textPayload = frame.Payload.ToUTF8String();

            if (textPayload == "[]")
                DebugX.Log(nameof(ChargingStationWSClient), " [] received!");

            else if (OCPP_WebSocket_RequestMessage. TryParse(textPayload, out var requestMessage)  && requestMessage  is not null)
            {

                var requestTimestamp         = Timestamp.Now;
                var requestJSON              = JArray.Parse(textPayload);
                var cancellationTokenSource  = new CancellationTokenSource();

                JObject?                     OCPPResponseJSON   = null;
                OCPP_WebSocket_ErrorMessage? ErrorMessage       = null;

                switch (requestMessage.Action)
                {

                    case "Reset":
                        {

                            #region Send OnResetWSRequest event

                            try
                            {

                                OnResetWSRequest?.Invoke(Timestamp.Now,
                                                         this,
                                                         requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnResetWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (ResetRequest.TryParse(requestMessage.Message,
                                                          requestMessage.RequestId,
                                                          ChargeBoxIdentity,
                                                          out var request,
                                                          out var errorResponse,
                                                          CustomResetRequestParser) && request is not null) {

                                    #region Send OnResetRequest event

                                    try
                                    {

                                        OnResetRequest?.Invoke(Timestamp.Now,
                                                               this,
                                                               request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnResetRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    ResetResponse? response = null;

                                    var results = OnReset?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnResetDelegate)?.Invoke(Timestamp.Now,
                                                                                                                       this,
                                                                                                                       request,
                                                                                                                       cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= ResetResponse.Failed(request);

                                    #endregion

                                    #region Send OnResetResponse event

                                    try
                                    {

                                        OnResetResponse?.Invoke(Timestamp.Now,
                                                                this,
                                                                request,
                                                                response,
                                                                response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnResetResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnResetWSResponse event

                            try
                            {

                                OnResetWSResponse?.Invoke(Timestamp.Now,
                                                          this,
                                                          requestJSON,
                                                          new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                             OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnResetWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "UpdateFirmware":
                        {

                            #region Send OnUpdateFirmwareWSRequest event

                            try
                            {

                                OnUpdateFirmwareWSRequest?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateFirmwareWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (UpdateFirmwareRequest.TryParse(requestMessage.Message,
                                                                   requestMessage.RequestId,
                                                                   ChargeBoxIdentity,
                                                                   out var request,
                                                                   out var errorResponse,
                                                                   CustomUpdateFirmwareRequestParser) && request is not null) {

                                    #region Send OnUpdateFirmwareRequest event

                                    try
                                    {

                                        OnUpdateFirmwareRequest?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateFirmwareRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    UpdateFirmwareResponse? response = null;

                                    var results = OnUpdateFirmware?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnUpdateFirmwareDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                this,
                                                                                                                                request,
                                                                                                                                cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= UpdateFirmwareResponse.Failed(request);

                                    #endregion

                                    #region Send OnUpdateFirmwareResponse event

                                    try
                                    {

                                        OnUpdateFirmwareResponse?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         response,
                                                                         response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateFirmwareResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnUpdateFirmwareWSResponse event

                            try
                            {

                                OnUpdateFirmwareWSResponse?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   requestJSON,
                                                                   new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                      OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateFirmwareWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "PublishFirmware":
                        {

                            #region Send OnPublishFirmwareWSRequest event

                            try
                            {

                                OnPublishFirmwareWSRequest?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPublishFirmwareWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (PublishFirmwareRequest.TryParse(requestMessage.Message,
                                                                   requestMessage.RequestId,
                                                                   ChargeBoxIdentity,
                                                                   out var request,
                                                                   out var errorResponse,
                                                                   CustomPublishFirmwareRequestParser) && request is not null) {

                                    #region Send OnPublishFirmwareRequest event

                                    try
                                    {

                                        OnPublishFirmwareRequest?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPublishFirmwareRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    PublishFirmwareResponse? response = null;

                                    var results = OnPublishFirmware?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnPublishFirmwareDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                 this,
                                                                                                                                 request,
                                                                                                                                 cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= PublishFirmwareResponse.Failed(request);

                                    #endregion

                                    #region Send OnPublishFirmwareResponse event

                                    try
                                    {

                                        OnPublishFirmwareResponse?.Invoke(Timestamp.Now,
                                                                          this,
                                                                          request,
                                                                          response,
                                                                          response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPublishFirmwareResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnPublishFirmwareWSResponse event

                            try
                            {

                                OnPublishFirmwareWSResponse?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    requestJSON,
                                                                    new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                       OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPublishFirmwareWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "UnpublishFirmware":
                        {

                            #region Send OnUnpublishFirmwareWSRequest event

                            try
                            {

                                OnUnpublishFirmwareWSRequest?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUnpublishFirmwareWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (UnpublishFirmwareRequest.TryParse(requestMessage.Message,
                                                                      requestMessage.RequestId,
                                                                      ChargeBoxIdentity,
                                                                      out var request,
                                                                      out var errorResponse,
                                                                      CustomUnpublishFirmwareRequestParser) && request is not null) {

                                    #region Send OnUnpublishFirmwareRequest event

                                    try
                                    {

                                        OnUnpublishFirmwareRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUnpublishFirmwareRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    UnpublishFirmwareResponse? response = null;

                                    var results = OnUnpublishFirmware?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnUnpublishFirmwareDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                   this,
                                                                                                                                   request,
                                                                                                                                   cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= UnpublishFirmwareResponse.Failed(request);

                                    #endregion

                                    #region Send OnUnpublishFirmwareResponse event

                                    try
                                    {

                                        OnUnpublishFirmwareResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request,
                                                                            response,
                                                                            response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUnpublishFirmwareResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnUnpublishFirmwareWSResponse event

                            try
                            {

                                OnUnpublishFirmwareWSResponse?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    requestJSON,
                                                                    new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                       OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUnpublishFirmwareWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "GetBaseReport":
                        {

                            #region Send OnGetBaseReportWSRequest event

                            try
                            {

                                OnGetBaseReportWSRequest?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetBaseReportWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (GetBaseReportRequest.TryParse(requestMessage.Message,
                                                                  requestMessage.RequestId,
                                                                  ChargeBoxIdentity,
                                                                  out var request,
                                                                  out var errorResponse,
                                                                  CustomGetBaseReportRequestParser) && request is not null) {

                                    #region Send OnGetBaseReportRequest event

                                    try
                                    {

                                        OnGetBaseReportRequest?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetBaseReportRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    GetBaseReportResponse? response = null;

                                    var results = OnGetBaseReport?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnGetBaseReportDelegate)?.Invoke(Timestamp.Now,
                                                                                                                               this,
                                                                                                                               request,
                                                                                                                               cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= GetBaseReportResponse.Failed(request);

                                    #endregion

                                    #region Send OnGetBaseReportResponse event

                                    try
                                    {

                                        OnGetBaseReportResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        request,
                                                                        response,
                                                                        response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetBaseReportResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnGetBaseReportWSResponse event

                            try
                            {

                                OnGetBaseReportWSResponse?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  requestJSON,
                                                                  new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                     OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetBaseReportWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "GetReport":
                        {

                            #region Send OnGetReportWSRequest event

                            try
                            {

                                OnGetReportWSRequest?.Invoke(Timestamp.Now,
                                                             this,
                                                             requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetReportWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (GetReportRequest.TryParse(requestMessage.Message,
                                                              requestMessage.RequestId,
                                                              ChargeBoxIdentity,
                                                              out var request,
                                                              out var errorResponse,
                                                              CustomGetReportRequestParser) && request is not null) {

                                    #region Send OnGetReportRequest event

                                    try
                                    {

                                        OnGetReportRequest?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetReportRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    GetReportResponse? response = null;

                                    var results = OnGetReport?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnGetReportDelegate)?.Invoke(Timestamp.Now,
                                                                                                                           this,
                                                                                                                           request,
                                                                                                                           cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= GetReportResponse.Failed(request);

                                    #endregion

                                    #region Send OnGetReportResponse event

                                    try
                                    {

                                        OnGetReportResponse?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request,
                                                                    response,
                                                                    response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetReportResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnGetReportWSResponse event

                            try
                            {

                                OnGetReportWSResponse?.Invoke(Timestamp.Now,
                                                              this,
                                                              requestJSON,
                                                              new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                 OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetReportWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "GetLog":
                        {

                            #region Send OnGetLogWSRequest event

                            try
                            {

                                OnGetLogWSRequest?.Invoke(Timestamp.Now,
                                                          this,
                                                          requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetLogWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (GetLogRequest.TryParse(requestMessage.Message,
                                                           requestMessage.RequestId,
                                                           ChargeBoxIdentity,
                                                           out var request,
                                                           out var errorResponse,
                                                           CustomGetLogRequestParser) && request is not null) {

                                    #region Send OnGetLogRequest event

                                    try
                                    {

                                        OnGetLogRequest?.Invoke(Timestamp.Now,
                                                                this,
                                                                request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetLogRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    GetLogResponse? response = null;

                                    var results = OnGetLog?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnGetLogDelegate)?.Invoke(Timestamp.Now,
                                                                                                                        this,
                                                                                                                        request,
                                                                                                                        cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= GetLogResponse.Failed(request);

                                    #endregion

                                    #region Send OnGetLogResponse event

                                    try
                                    {

                                        OnGetLogResponse?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 request,
                                                                 response,
                                                                 response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetLogResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnGetLogWSResponse event

                            try
                            {

                                OnGetLogWSResponse?.Invoke(Timestamp.Now,
                                                           this,
                                                           requestJSON,
                                                           new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                              OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetLogWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "SetVariables":
                        {

                            #region Send OnSetVariablesWSRequest event

                            try
                            {

                                OnSetVariablesWSRequest?.Invoke(Timestamp.Now,
                                                                this,
                                                                requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetVariablesWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (SetVariablesRequest.TryParse(requestMessage.Message,
                                                                 requestMessage.RequestId,
                                                                 ChargeBoxIdentity,
                                                                 out var request,
                                                                 out var errorResponse,
                                                                 CustomSetVariablesRequestParser) && request is not null) {

                                    #region Send OnSetVariablesRequest event

                                    try
                                    {

                                        OnSetVariablesRequest?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetVariablesRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    SetVariablesResponse? response = null;

                                    var results = OnSetVariables?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnSetVariablesDelegate)?.Invoke(Timestamp.Now,
                                                                                                                              this,
                                                                                                                              request,
                                                                                                                              cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= SetVariablesResponse.Failed(request);

                                    #endregion

                                    #region Send OnSetVariablesResponse event

                                    try
                                    {

                                        OnSetVariablesResponse?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       request,
                                                                       response,
                                                                       response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetVariablesResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnSetVariablesWSResponse event

                            try
                            {

                                OnSetVariablesWSResponse?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 requestJSON,
                                                                 new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                    OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetVariablesWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "GetVariables":
                        {

                            #region Send OnGetVariablesWSRequest event

                            try
                            {

                                OnGetVariablesWSRequest?.Invoke(Timestamp.Now,
                                                                this,
                                                                requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetVariablesWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (GetVariablesRequest.TryParse(requestMessage.Message,
                                                                 requestMessage.RequestId,
                                                                 ChargeBoxIdentity,
                                                                 out var request,
                                                                 out var errorResponse,
                                                                 CustomGetVariablesRequestParser) && request is not null) {

                                    #region Send OnGetVariablesRequest event

                                    try
                                    {

                                        OnGetVariablesRequest?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetVariablesRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    GetVariablesResponse? response = null;

                                    var results = OnGetVariables?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnGetVariablesDelegate)?.Invoke(Timestamp.Now,
                                                                                                                              this,
                                                                                                                              request,
                                                                                                                              cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= GetVariablesResponse.Failed(request);

                                    #endregion

                                    #region Send OnGetVariablesResponse event

                                    try
                                    {

                                        OnGetVariablesResponse?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       request,
                                                                       response,
                                                                       response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetVariablesResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnGetVariablesWSResponse event

                            try
                            {

                                OnGetVariablesWSResponse?.Invoke(Timestamp.Now,
                                                          this,
                                                          requestJSON,
                                                          new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                             OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetVariablesWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "SetMonitoringBase":
                        {

                            #region Send OnSetMonitoringBaseWSRequest event

                            try
                            {

                                OnSetMonitoringBaseWSRequest?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetMonitoringBaseWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (SetMonitoringBaseRequest.TryParse(requestMessage.Message,
                                                                      requestMessage.RequestId,
                                                                      ChargeBoxIdentity,
                                                                      out var request,
                                                                      out var errorResponse,
                                                                      CustomSetMonitoringBaseRequestParser) && request is not null) {

                                    #region Send OnSetMonitoringBaseRequest event

                                    try
                                    {

                                        OnSetMonitoringBaseRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetMonitoringBaseRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    SetMonitoringBaseResponse? response = null;

                                    var results = OnSetMonitoringBase?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnSetMonitoringBaseDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                   this,
                                                                                                                                   request,
                                                                                                                                   cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= SetMonitoringBaseResponse.Failed(request);

                                    #endregion

                                    #region Send OnSetMonitoringBaseResponse event

                                    try
                                    {

                                        OnSetMonitoringBaseResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request,
                                                                            response,
                                                                            response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetMonitoringBaseResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnSetMonitoringBaseWSResponse event

                            try
                            {

                                OnSetMonitoringBaseWSResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      requestJSON,
                                                                      new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                         OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetMonitoringBaseWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "GetMonitoringReport":
                        {

                            #region Send OnGetMonitoringReportWSRequest event

                            try
                            {

                                OnGetMonitoringReportWSRequest?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetMonitoringReportWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (GetMonitoringReportRequest.TryParse(requestMessage.Message,
                                                                        requestMessage.RequestId,
                                                                        ChargeBoxIdentity,
                                                                        out var request,
                                                                        out var errorResponse,
                                                                        CustomGetMonitoringReportRequestParser) && request is not null) {

                                    #region Send OnGetMonitoringReportRequest event

                                    try
                                    {

                                        OnGetMonitoringReportRequest?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetMonitoringReportRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    GetMonitoringReportResponse? response = null;

                                    var results = OnGetMonitoringReport?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnGetMonitoringReportDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                     this,
                                                                                                                                     request,
                                                                                                                                     cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= GetMonitoringReportResponse.Failed(request);

                                    #endregion

                                    #region Send OnGetMonitoringReportResponse event

                                    try
                                    {

                                        OnGetMonitoringReportResponse?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request,
                                                                              response,
                                                                              response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetMonitoringReportResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnGetMonitoringReportWSResponse event

                            try
                            {

                                OnGetMonitoringReportWSResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        requestJSON,
                                                                        new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                           OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetMonitoringReportWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "SetMonitoringLevel":
                        {

                            #region Send OnSetMonitoringLevelWSRequest event

                            try
                            {

                                OnSetMonitoringLevelWSRequest?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetMonitoringLevelWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (SetMonitoringLevelRequest.TryParse(requestMessage.Message,
                                                                       requestMessage.RequestId,
                                                                       ChargeBoxIdentity,
                                                                       out var request,
                                                                       out var errorResponse,
                                                                       CustomSetMonitoringLevelRequestParser) && request is not null) {

                                    #region Send OnSetMonitoringLevelRequest event

                                    try
                                    {

                                        OnSetMonitoringLevelRequest?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetMonitoringLevelRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    SetMonitoringLevelResponse? response = null;

                                    var results = OnSetMonitoringLevel?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnSetMonitoringLevelDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                    this,
                                                                                                                                    request,
                                                                                                                                    cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= SetMonitoringLevelResponse.Failed(request);

                                    #endregion

                                    #region Send OnSetMonitoringLevelResponse event

                                    try
                                    {

                                        OnSetMonitoringLevelResponse?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request,
                                                                             response,
                                                                             response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetMonitoringLevelResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnSetMonitoringLevelWSResponse event

                            try
                            {

                                OnSetMonitoringLevelWSResponse?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       requestJSON,
                                                                       new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                          OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetMonitoringLevelWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "SetVariableMonitoring":
                        {

                            #region Send OnSetVariableMonitoringWSRequest event

                            try
                            {

                                OnSetVariableMonitoringWSRequest?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetVariableMonitoringWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (SetVariableMonitoringRequest.TryParse(requestMessage.Message,
                                                                          requestMessage.RequestId,
                                                                          ChargeBoxIdentity,
                                                                          out var request,
                                                                          out var errorResponse,
                                                                          CustomSetVariableMonitoringRequestParser) && request is not null) {

                                    #region Send OnSetVariableMonitoringRequest event

                                    try
                                    {

                                        OnSetVariableMonitoringRequest?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetVariableMonitoringRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    SetVariableMonitoringResponse? response = null;

                                    var results = OnSetVariableMonitoring?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnSetVariableMonitoringDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                       this,
                                                                                                                                       request,
                                                                                                                                       cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= SetVariableMonitoringResponse.Failed(request);

                                    #endregion

                                    #region Send OnSetVariableMonitoringResponse event

                                    try
                                    {

                                        OnSetVariableMonitoringResponse?.Invoke(Timestamp.Now,
                                                                                this,
                                                                                request,
                                                                                response,
                                                                                response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetVariableMonitoringResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnSetVariableMonitoringWSResponse event

                            try
                            {

                                OnSetVariableMonitoringWSResponse?.Invoke(Timestamp.Now,
                                                                          this,
                                                                          requestJSON,
                                                                          new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                             OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetVariableMonitoringWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "ClearVariableMonitoring":
                        {

                            #region Send OnClearVariableMonitoringWSRequest event

                            try
                            {

                                OnClearVariableMonitoringWSRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearVariableMonitoringWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (ClearVariableMonitoringRequest.TryParse(requestMessage.Message,
                                                                            requestMessage.RequestId,
                                                                            ChargeBoxIdentity,
                                                                            out var request,
                                                                            out var errorResponse,
                                                                            CustomClearVariableMonitoringRequestParser) && request is not null) {

                                    #region Send OnClearVariableMonitoringRequest event

                                    try
                                    {

                                        OnClearVariableMonitoringRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearVariableMonitoringRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    ClearVariableMonitoringResponse? response = null;

                                    var results = OnClearVariableMonitoring?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnClearVariableMonitoringDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                         this,
                                                                                                                                         request,
                                                                                                                                         cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= ClearVariableMonitoringResponse.Failed(request);

                                    #endregion

                                    #region Send OnClearVariableMonitoringResponse event

                                    try
                                    {

                                        OnClearVariableMonitoringResponse?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  request,
                                                                                  response,
                                                                                  response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearVariableMonitoringResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnClearVariableMonitoringWSResponse event

                            try
                            {

                                OnClearVariableMonitoringWSResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            requestJSON,
                                                                            new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                               OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearVariableMonitoringWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "SetNetworkProfile":
                        {

                            #region Send OnSetNetworkProfileWSRequest event

                            try
                            {

                                OnSetNetworkProfileWSRequest?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetNetworkProfileWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (SetNetworkProfileRequest.TryParse(requestMessage.Message,
                                                                      requestMessage.RequestId,
                                                                      ChargeBoxIdentity,
                                                                      out var request,
                                                                      out var errorResponse,
                                                                      CustomSetNetworkProfileRequestParser) && request is not null) {

                                    #region Send OnSetNetworkProfileRequest event

                                    try
                                    {

                                        OnSetNetworkProfileRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetNetworkProfileRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    SetNetworkProfileResponse? response = null;

                                    var results = OnSetNetworkProfile?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnSetNetworkProfileDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                   this,
                                                                                                                                   request,
                                                                                                                                   cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= SetNetworkProfileResponse.Failed(request);

                                    #endregion

                                    #region Send OnSetNetworkProfileResponse event

                                    try
                                    {

                                        OnSetNetworkProfileResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request,
                                                                            response,
                                                                            response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetNetworkProfileResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnSetNetworkProfileWSResponse event

                            try
                            {

                                OnSetNetworkProfileWSResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      requestJSON,
                                                                      new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                         OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetNetworkProfileWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "ChangeAvailability":
                        {

                            #region Send OnChangeAvailabilityWSRequest event

                            try
                            {

                                OnChangeAvailabilityWSRequest?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnChangeAvailabilityWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (ChangeAvailabilityRequest.TryParse(requestMessage.Message,
                                                                       requestMessage.RequestId,
                                                                       ChargeBoxIdentity,
                                                                       out var request,
                                                                       out var errorResponse,
                                                                       CustomChangeAvailabilityRequestParser) && request is not null) {

                                    #region Send OnChangeAvailabilityRequest event

                                    try
                                    {

                                        OnChangeAvailabilityRequest?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnChangeAvailabilityRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    ChangeAvailabilityResponse? response = null;

                                    var results = OnChangeAvailability?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnChangeAvailabilityDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                    this,
                                                                                                                                    request,
                                                                                                                                    cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= ChangeAvailabilityResponse.Failed(request);

                                    #endregion

                                    #region Send OnChangeAvailabilityResponse event

                                    try
                                    {

                                        OnChangeAvailabilityResponse?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request,
                                                                             response,
                                                                             response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnChangeAvailabilityResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnChangeAvailabilityWSResponse event

                            try
                            {

                                OnChangeAvailabilityWSResponse?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       requestJSON,
                                                                       new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                          OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnChangeAvailabilityWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "TriggerMessage":
                        {

                            #region Send OnTriggerMessageWSRequest event

                            try
                            {

                                OnTriggerMessageWSRequest?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTriggerMessageWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (TriggerMessageRequest.TryParse(requestMessage.Message,
                                                                   requestMessage.RequestId,
                                                                   ChargeBoxIdentity,
                                                                   out var request,
                                                                   out var errorResponse,
                                                                   CustomTriggerMessageRequestParser) && request is not null) {

                                    #region Send OnTriggerMessageRequest event

                                    try
                                    {

                                        OnTriggerMessageRequest?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTriggerMessageRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    TriggerMessageResponse? response = null;

                                    var results = OnTriggerMessage?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnTriggerMessageDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                this,
                                                                                                                                request,
                                                                                                                                cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= TriggerMessageResponse.Failed(request);

                                    #endregion

                                    #region Send OnTriggerMessageResponse event

                                    try
                                    {

                                        OnTriggerMessageResponse?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         request,
                                                                         response,
                                                                         response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTriggerMessageResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnTriggerMessageWSResponse event

                            try
                            {

                                OnTriggerMessageWSResponse?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   requestJSON,
                                                                   new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                      OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTriggerMessageWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "DataTransfer":
                        {

                            #region Send OnIncomingDataTransferWSRequest event

                            try
                            {

                                OnIncomingDataTransferWSRequest?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnIncomingDataTransferWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (CSMS.DataTransferRequest.TryParse(requestMessage.Message,
                                                                    requestMessage.RequestId,
                                                                    ChargeBoxIdentity,
                                                                    out var request,
                                                                    out var errorResponse,
                                                                    CustomIncomingDataTransferRequestParser) && request is not null) {

                                    #region Send OnIncomingDataTransferRequest event

                                    try
                                    {

                                        OnIncomingDataTransferRequest?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDataTransferRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    DataTransferResponse? response = null;

                                    var results = OnIncomingDataTransfer?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnIncomingDataTransferDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                      this,
                                                                                                                                      request,
                                                                                                                                      cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= DataTransferResponse.Failed(request);

                                    #endregion

                                    #region Send OnIncomingDataTransferResponse event

                                    try
                                    {

                                        OnIncomingDataTransferResponse?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               request,
                                                                               response,
                                                                               response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnIncomingDataTransferResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnIncomingDataTransferWSResponse event

                            try
                            {

                                OnIncomingDataTransferWSResponse?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         requestJSON,
                                                                         new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                            OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnIncomingDataTransferWSResponse));
                            }

                            #endregion

                        }
                        break;


                    case "CertificateSigned":
                        {

                            #region Send OnCertificateSignedWSRequest event

                            try
                            {

                                OnCertificateSignedWSRequest?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCertificateSignedWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (CertificateSignedRequest.TryParse(requestMessage.Message,
                                                                      requestMessage.RequestId,
                                                                      ChargeBoxIdentity,
                                                                      out var request,
                                                                      out var errorResponse,
                                                                      CustomCertificateSignedRequestParser) && request is not null) {

                                    #region Send OnCertificateSignedRequest event

                                    try
                                    {

                                        OnCertificateSignedRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCertificateSignedRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    CertificateSignedResponse? response = null;

                                    var results = OnCertificateSigned?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnCertificateSignedDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                   this,
                                                                                                                                   request,
                                                                                                                                   cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= CertificateSignedResponse.Failed(request);

                                    #endregion

                                    #region Send OnCertificateSignedResponse event

                                    try
                                    {

                                        OnCertificateSignedResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request,
                                                                            response,
                                                                            response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCertificateSignedResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnCertificateSignedWSResponse event

                            try
                            {

                                OnCertificateSignedWSResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      requestJSON,
                                                                      new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                         OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCertificateSignedWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "InstallCertificate":
                        {

                            #region Send OnInstallCertificateWSRequest event

                            try
                            {

                                OnInstallCertificateWSRequest?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnInstallCertificateWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (InstallCertificateRequest.TryParse(requestMessage.Message,
                                                                       requestMessage.RequestId,
                                                                       ChargeBoxIdentity,
                                                                       out var request,
                                                                       out var errorResponse,
                                                                       CustomInstallCertificateRequestParser) && request is not null) {

                                    #region Send OnInstallCertificateRequest event

                                    try
                                    {

                                        OnInstallCertificateRequest?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnInstallCertificateRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    InstallCertificateResponse? response = null;

                                    var results = OnInstallCertificate?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnInstallCertificateDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                    this,
                                                                                                                                    request,
                                                                                                                                    cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= InstallCertificateResponse.Failed(request);

                                    #endregion

                                    #region Send OnInstallCertificateResponse event

                                    try
                                    {

                                        OnInstallCertificateResponse?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request,
                                                                             response,
                                                                             response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnInstallCertificateResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnInstallCertificateWSResponse event

                            try
                            {

                                OnInstallCertificateWSResponse?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       requestJSON,
                                                                       new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                          OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnInstallCertificateWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "GetInstalledCertificateIds":
                        {

                            #region Send OnGetInstalledCertificateIdsWSRequest event

                            try
                            {

                                OnGetInstalledCertificateIdsWSRequest?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetInstalledCertificateIdsWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (GetInstalledCertificateIdsRequest.TryParse(requestMessage.Message,
                                                                               requestMessage.RequestId,
                                                                               ChargeBoxIdentity,
                                                                               out var request,
                                                                               out var errorResponse,
                                                                               CustomGetInstalledCertificateIdsRequestParser) && request is not null) {

                                    #region Send OnGetInstalledCertificateIdsRequest event

                                    try
                                    {

                                        OnGetInstalledCertificateIdsRequest?.Invoke(Timestamp.Now,
                                                                                    this,
                                                                                    request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetInstalledCertificateIdsRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    GetInstalledCertificateIdsResponse? response = null;

                                    var results = OnGetInstalledCertificateIds?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnGetInstalledCertificateIdsDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                            this,
                                                                                                                                            request,
                                                                                                                                            cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= GetInstalledCertificateIdsResponse.Failed(request);

                                    #endregion

                                    #region Send OnGetInstalledCertificateIdsResponse event

                                    try
                                    {

                                        OnGetInstalledCertificateIdsResponse?.Invoke(Timestamp.Now,
                                                                                     this,
                                                                                     request,
                                                                                     response,
                                                                                     response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetInstalledCertificateIdsResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnGetInstalledCertificateIdsWSResponse event

                            try
                            {

                                OnGetInstalledCertificateIdsWSResponse?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               requestJSON,
                                                                               new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                                  OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetInstalledCertificateIdsWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "DeleteCertificate":
                        {

                            #region Send OnDeleteCertificateWSRequest event

                            try
                            {

                                OnDeleteCertificateWSRequest?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDeleteCertificateWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (DeleteCertificateRequest.TryParse(requestMessage.Message,
                                                                      requestMessage.RequestId,
                                                                      ChargeBoxIdentity,
                                                                      out var request,
                                                                      out var errorResponse,
                                                                      CustomDeleteCertificateRequestParser) && request is not null) {

                                    #region Send OnDeleteCertificateRequest event

                                    try
                                    {

                                        OnDeleteCertificateRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDeleteCertificateRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    DeleteCertificateResponse? response = null;

                                    var results = OnDeleteCertificate?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnDeleteCertificateDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                   this,
                                                                                                                                   request,
                                                                                                                                   cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= DeleteCertificateResponse.Failed(request);

                                    #endregion

                                    #region Send OnDeleteCertificateResponse event

                                    try
                                    {

                                        OnDeleteCertificateResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request,
                                                                            response,
                                                                            response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDeleteCertificateResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnDeleteCertificateWSResponse event

                            try
                            {

                                OnDeleteCertificateWSResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      requestJSON,
                                                                      new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                         OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDeleteCertificateWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "NotifyCRL":
                        {

                            #region Send OnNotifyCRLWSRequest event

                            try
                            {

                                OnNotifyCRLWSRequest?.Invoke(Timestamp.Now,
                                                             this,
                                                             requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyCRLWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (NotifyCRLRequest.TryParse(requestMessage.Message,
                                                              requestMessage.RequestId,
                                                              ChargeBoxIdentity,
                                                              out var request,
                                                              out var errorResponse,
                                                              CustomNotifyCRLRequestParser) && request is not null) {

                                    #region Send OnNotifyCRLRequest event

                                    try
                                    {

                                        OnNotifyCRLRequest?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyCRLRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    NotifyCRLResponse? response = null;

                                    var results = OnNotifyCRL?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnNotifyCRLDelegate)?.Invoke(Timestamp.Now,
                                                                                                                           this,
                                                                                                                           request,
                                                                                                                           cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= NotifyCRLResponse.Failed(request);

                                    #endregion

                                    #region Send OnNotifyCRLResponse event

                                    try
                                    {

                                        OnNotifyCRLResponse?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request,
                                                                    response,
                                                                    response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyCRLResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnNotifyCRLWSResponse event

                            try
                            {

                                OnNotifyCRLWSResponse?.Invoke(Timestamp.Now,
                                                              this,
                                                              requestJSON,
                                                              new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                 OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyCRLWSResponse));
                            }

                            #endregion

                        }
                        break;


                    case "GetLocalListVersion":
                        {

                            #region Send OnGetLocalListVersionWSRequest event

                            try
                            {

                                OnGetLocalListVersionWSRequest?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetLocalListVersionWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (GetLocalListVersionRequest.TryParse(requestMessage.Message,
                                                                        requestMessage.RequestId,
                                                                        ChargeBoxIdentity,
                                                                        out var request,
                                                                        out var errorResponse,
                                                                        CustomGetLocalListVersionRequestParser) && request is not null) {

                                    #region Send OnGetLocalListVersionRequest event

                                    try
                                    {

                                        OnGetLocalListVersionRequest?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetLocalListVersionRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    GetLocalListVersionResponse? response = null;

                                    var results = OnGetLocalListVersion?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnGetLocalListVersionDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                     this,
                                                                                                                                     request,
                                                                                                                                     cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= GetLocalListVersionResponse.Failed(request);

                                    #endregion

                                    #region Send OnGetLocalListVersionResponse event

                                    try
                                    {

                                        OnGetLocalListVersionResponse?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request,
                                                                              response,
                                                                              response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetLocalListVersionResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnGetLocalListVersionWSResponse event

                            try
                            {

                                OnGetLocalListVersionWSResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        requestJSON,
                                                                        new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                           OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetLocalListVersionWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "SendLocalList":
                        {

                            #region Send OnSendLocalListWSRequest event

                            try
                            {

                                OnSendLocalListWSRequest?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSendLocalListWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (SendLocalListRequest.TryParse(requestMessage.Message,
                                                                  requestMessage.RequestId,
                                                                  ChargeBoxIdentity,
                                                                  out var request,
                                                                  out var errorResponse,
                                                                  CustomSendLocalListRequestParser) && request is not null) {

                                    #region Send OnSendLocalListRequest event

                                    try
                                    {

                                        OnSendLocalListRequest?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSendLocalListRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    SendLocalListResponse? response = null;

                                    var results = OnSendLocalList?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnSendLocalListDelegate)?.Invoke(Timestamp.Now,
                                                                                                                               this,
                                                                                                                               request,
                                                                                                                               cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= SendLocalListResponse.Failed(request);

                                    #endregion

                                    #region Send OnSendLocalListResponse event

                                    try
                                    {

                                        OnSendLocalListResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        request,
                                                                        response,
                                                                        response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSendLocalListResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnSendLocalListWSResponse event

                            try
                            {

                                OnSendLocalListWSResponse?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  requestJSON,
                                                                  new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                     OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSendLocalListWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "ClearCache":
                        {

                            #region Send OnClearCacheWSRequest event

                            try
                            {

                                OnClearCacheWSRequest?.Invoke(Timestamp.Now,
                                                              this,
                                                              requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearCacheWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (ClearCacheRequest.TryParse(requestMessage.Message,
                                                               requestMessage.RequestId,
                                                               ChargeBoxIdentity,
                                                               out var request,
                                                               out var errorResponse,
                                                               CustomClearCacheRequestParser) && request is not null) {

                                    #region Send OnClearCacheRequest event

                                    try
                                    {

                                        OnClearCacheRequest?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearCacheRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    ClearCacheResponse? response = null;

                                    var results = OnClearCache?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnClearCacheDelegate)?.Invoke(Timestamp.Now,
                                                                                                                            this,
                                                                                                                            request,
                                                                                                                            cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= ClearCacheResponse.Failed(request);

                                    #endregion

                                    #region Send OnClearCacheResponse event

                                    try
                                    {

                                        OnClearCacheResponse?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     request,
                                                                     response,
                                                                     response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearCacheResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnClearCacheWSResponse event

                            try
                            {

                                OnClearCacheWSResponse?.Invoke(Timestamp.Now,
                                                               this,
                                                               requestJSON,
                                                               new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                  OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearCacheWSResponse));
                            }

                            #endregion

                        }
                        break;


                    case "ReserveNow":
                        {

                            #region Send OnReserveNowWSRequest event

                            try
                            {

                                OnReserveNowWSRequest?.Invoke(Timestamp.Now,
                                                              this,
                                                              requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReserveNowWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (ReserveNowRequest.TryParse(requestMessage.Message,
                                                               requestMessage.RequestId,
                                                               ChargeBoxIdentity,
                                                               out var request,
                                                               out var errorResponse,
                                                               CustomReserveNowRequestParser) && request is not null) {

                                    #region Send OnReserveNowRequest event

                                    try
                                    {

                                        OnReserveNowRequest?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReserveNowRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    ReserveNowResponse? response = null;

                                    var results = OnReserveNow?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnReserveNowDelegate)?.Invoke(Timestamp.Now,
                                                                                                                            this,
                                                                                                                            request,
                                                                                                                            cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= ReserveNowResponse.Failed(request);

                                    #endregion

                                    #region Send OnReserveNowResponse event

                                    try
                                    {

                                        OnReserveNowResponse?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     request,
                                                                     response,
                                                                     response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReserveNowResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnReserveNowWSResponse event

                            try
                            {

                                OnReserveNowWSResponse?.Invoke(Timestamp.Now,
                                                               this,
                                                               requestJSON,
                                                               new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                  OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReserveNowWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "CancelReservation":
                        {

                            #region Send OnCancelReservationWSRequest event

                            try
                            {

                                OnCancelReservationWSRequest?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCancelReservationWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (CancelReservationRequest.TryParse(requestMessage.Message,
                                                                      requestMessage.RequestId,
                                                                      ChargeBoxIdentity,
                                                                      out var request,
                                                                      out var errorResponse,
                                                                      CustomCancelReservationRequestParser) && request is not null) {

                                    #region Send OnCancelReservationRequest event

                                    try
                                    {

                                        OnCancelReservationRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCancelReservationRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    CancelReservationResponse? response = null;

                                    var results = OnCancelReservation?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnCancelReservationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                   this,
                                                                                                                                   request,
                                                                                                                                   cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= CancelReservationResponse.Failed(request);

                                    #endregion

                                    #region Send OnCancelReservationResponse event

                                    try
                                    {

                                        OnCancelReservationResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request,
                                                                            response,
                                                                            response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCancelReservationResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnCancelReservationWSResponse event

                            try
                            {

                                OnCancelReservationWSResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      requestJSON,
                                                                      new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                         OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCancelReservationWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "RequestStartTransaction":
                        {

                            #region Send OnRequestStartTransactionWSRequest event

                            try
                            {

                                OnRequestStartTransactionWSRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnRequestStartTransactionWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (RequestStartTransactionRequest.TryParse(requestMessage.Message,
                                                                            requestMessage.RequestId,
                                                                            ChargeBoxIdentity,
                                                                            out var request,
                                                                            out var errorResponse,
                                                                            CustomRequestStartTransactionRequestParser) && request is not null) {

                                    #region Send OnRequestStartTransactionRequest event

                                    try
                                    {

                                        OnRequestStartTransactionRequest?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnRequestStartTransactionRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    RequestStartTransactionResponse? response = null;

                                    var results = OnRequestStartTransaction?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnRequestStartTransactionDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                         this,
                                                                                                                                         request,
                                                                                                                                         cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= RequestStartTransactionResponse.Failed(request);

                                    #endregion

                                    #region Send OnRequestStartTransactionResponse event

                                    try
                                    {

                                        OnRequestStartTransactionResponse?.Invoke(Timestamp.Now,
                                                                                  this,
                                                                                  request,
                                                                                  response,
                                                                                  response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnRequestStartTransactionResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnRequestStartTransactionWSResponse event

                            try
                            {

                                OnRequestStartTransactionWSResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            requestJSON,
                                                                            new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                               OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnRequestStartTransactionWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "RequestStopTransaction":
                        {

                            #region Send OnRequestStopTransactionWSRequest event

                            try
                            {

                                OnRequestStopTransactionWSRequest?.Invoke(Timestamp.Now,
                                                                          this,
                                                                          requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnRequestStopTransactionWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (RequestStopTransactionRequest.TryParse(requestMessage.Message,
                                                                           requestMessage.RequestId,
                                                                           ChargeBoxIdentity,
                                                                           out var request,
                                                                           out var errorResponse,
                                                                           CustomRequestStopTransactionRequestParser) && request is not null) {

                                    #region Send OnRequestStopTransactionRequest event

                                    try
                                    {

                                        OnRequestStopTransactionRequest?.Invoke(Timestamp.Now,
                                                                                this,
                                                                                request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnRequestStopTransactionRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    RequestStopTransactionResponse? response = null;

                                    var results = OnRequestStopTransaction?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnRequestStopTransactionDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                        this,
                                                                                                                                        request,
                                                                                                                                        cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= RequestStopTransactionResponse.Failed(request);

                                    #endregion

                                    #region Send OnRequestStopTransactionResponse event

                                    try
                                    {

                                        OnRequestStopTransactionResponse?.Invoke(Timestamp.Now,
                                                                                 this,
                                                                                 request,
                                                                                 response,
                                                                                 response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnRequestStopTransactionResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnRequestStopTransactionWSResponse event

                            try
                            {

                                OnRequestStopTransactionWSResponse?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           requestJSON,
                                                                           new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                              OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnRequestStopTransactionWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "GetTransactionStatus":
                        {

                            #region Send OnGetTransactionStatusWSRequest event

                            try
                            {

                                OnGetTransactionStatusWSRequest?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetTransactionStatusWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (GetTransactionStatusRequest.TryParse(requestMessage.Message,
                                                                         requestMessage.RequestId,
                                                                         ChargeBoxIdentity,
                                                                         out var request,
                                                                         out var errorResponse,
                                                                         CustomGetTransactionStatusRequestParser) && request is not null) {

                                    #region Send OnGetTransactionStatusRequest event

                                    try
                                    {

                                        OnGetTransactionStatusRequest?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetTransactionStatusRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    GetTransactionStatusResponse? response = null;

                                    var results = OnGetTransactionStatus?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnGetTransactionStatusDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                      this,
                                                                                                                                      request,
                                                                                                                                      cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= GetTransactionStatusResponse.Failed(request);

                                    #endregion

                                    #region Send OnGetTransactionStatusResponse event

                                    try
                                    {

                                        OnGetTransactionStatusResponse?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               request,
                                                                               response,
                                                                               response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetTransactionStatusResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnGetTransactionStatusWSResponse event

                            try
                            {

                                OnGetTransactionStatusWSResponse?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         requestJSON,
                                                                         new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                            OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetTransactionStatusWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "SetChargingProfile":
                        {

                            #region Send OnSetChargingProfileWSRequest event

                            try
                            {

                                OnSetChargingProfileWSRequest?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetChargingProfileWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (SetChargingProfileRequest.TryParse(requestMessage.Message,
                                                                       requestMessage.RequestId,
                                                                       ChargeBoxIdentity,
                                                                       out var request,
                                                                       out var errorResponse,
                                                                       CustomSetChargingProfileRequestParser) && request is not null) {

                                    #region Send OnSetChargingProfileRequest event

                                    try
                                    {

                                        OnSetChargingProfileRequest?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetChargingProfileRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    SetChargingProfileResponse? response = null;

                                    var results = OnSetChargingProfile?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnSetChargingProfileDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                    this,
                                                                                                                                    request,
                                                                                                                                    cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= SetChargingProfileResponse.Failed(request);

                                    #endregion

                                    #region Send OnSetChargingProfileResponse event

                                    try
                                    {

                                        OnSetChargingProfileResponse?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request,
                                                                             response,
                                                                             response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetChargingProfileResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnSetChargingProfileWSResponse event

                            try
                            {

                                OnSetChargingProfileWSResponse?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       requestJSON,
                                                                       new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                          OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetChargingProfileWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "GetChargingProfiles":
                        {

                            #region Send OnGetChargingProfilesWSRequest event

                            try
                            {

                                OnGetChargingProfilesWSRequest?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetChargingProfilesWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (GetChargingProfilesRequest.TryParse(requestMessage.Message,
                                                                        requestMessage.RequestId,
                                                                        ChargeBoxIdentity,
                                                                        out var request,
                                                                        out var errorResponse,
                                                                        CustomGetChargingProfilesRequestParser) && request is not null) {

                                    #region Send OnGetChargingProfilesRequest event

                                    try
                                    {

                                        OnGetChargingProfilesRequest?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetChargingProfilesRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    GetChargingProfilesResponse? response = null;

                                    var results = OnGetChargingProfiles?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnGetChargingProfilesDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                     this,
                                                                                                                                     request,
                                                                                                                                     cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= GetChargingProfilesResponse.Failed(request);

                                    #endregion

                                    #region Send OnGetChargingProfilesResponse event

                                    try
                                    {

                                        OnGetChargingProfilesResponse?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request,
                                                                              response,
                                                                              response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetChargingProfilesResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnGetChargingProfilesWSResponse event

                            try
                            {

                                OnGetChargingProfilesWSResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        requestJSON,
                                                                        new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                           OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetChargingProfilesWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "ClearChargingProfile":
                        {

                            #region Send OnClearChargingProfileWSRequest event

                            try
                            {

                                OnClearChargingProfileWSRequest?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearChargingProfileWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (ClearChargingProfileRequest.TryParse(requestMessage.Message,
                                                                         requestMessage.RequestId,
                                                                         ChargeBoxIdentity,
                                                                         out var request,
                                                                         out var errorResponse,
                                                                         CustomClearChargingProfileRequestParser) && request is not null) {

                                    #region Send OnClearChargingProfileRequest event

                                    try
                                    {

                                        OnClearChargingProfileRequest?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearChargingProfileRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    ClearChargingProfileResponse? response = null;

                                    var results = OnClearChargingProfile?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnClearChargingProfileDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                      this,
                                                                                                                                      request,
                                                                                                                                      cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= ClearChargingProfileResponse.Failed(request);

                                    #endregion

                                    #region Send OnClearChargingProfileResponse event

                                    try
                                    {

                                        OnClearChargingProfileResponse?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               request,
                                                                               response,
                                                                               response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearChargingProfileResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnClearChargingProfileWSResponse event

                            try
                            {

                                OnClearChargingProfileWSResponse?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         requestJSON,
                                                                         new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                            OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearChargingProfileWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "GetCompositeSchedule":
                        {

                            #region Send OnGetCompositeScheduleWSRequest event

                            try
                            {

                                OnGetCompositeScheduleWSRequest?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetCompositeScheduleWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (GetCompositeScheduleRequest.TryParse(requestMessage.Message,
                                                                         requestMessage.RequestId,
                                                                         ChargeBoxIdentity,
                                                                         out var request,
                                                                         out var errorResponse,
                                                                         CustomGetCompositeScheduleRequestParser) && request is not null) {

                                    #region Send OnGetCompositeScheduleRequest event

                                    try
                                    {

                                        OnGetCompositeScheduleRequest?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetCompositeScheduleRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    GetCompositeScheduleResponse? response = null;

                                    var results = OnGetCompositeSchedule?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnGetCompositeScheduleDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                      this,
                                                                                                                                      request,
                                                                                                                                      cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= GetCompositeScheduleResponse.Failed(request);

                                    #endregion

                                    #region Send OnGetCompositeScheduleResponse event

                                    try
                                    {

                                        OnGetCompositeScheduleResponse?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               request,
                                                                               response,
                                                                               response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetCompositeScheduleResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnGetCompositeScheduleWSResponse event

                            try
                            {

                                OnGetCompositeScheduleWSResponse?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         requestJSON,
                                                                         new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                            OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetCompositeScheduleWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "UpdateDynamicSchedule":
                        {

                            #region Send OnUpdateDynamicScheduleWSRequest event

                            try
                            {

                                OnUpdateDynamicScheduleWSRequest?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateDynamicScheduleWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (UpdateDynamicScheduleRequest.TryParse(requestMessage.Message,
                                                                          requestMessage.RequestId,
                                                                          ChargeBoxIdentity,
                                                                          out var request,
                                                                          out var errorResponse,
                                                                          CustomUpdateDynamicScheduleRequestParser) && request is not null) {

                                    #region Send OnUpdateDynamicScheduleRequest event

                                    try
                                    {

                                        OnUpdateDynamicScheduleRequest?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateDynamicScheduleRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    UpdateDynamicScheduleResponse? response = null;

                                    var results = OnUpdateDynamicSchedule?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnUpdateDynamicScheduleDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                       this,
                                                                                                                                       request,
                                                                                                                                       cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= UpdateDynamicScheduleResponse.Failed(request);

                                    #endregion

                                    #region Send OnUpdateDynamicScheduleResponse event

                                    try
                                    {

                                        OnUpdateDynamicScheduleResponse?.Invoke(Timestamp.Now,
                                                                                this,
                                                                                request,
                                                                                response,
                                                                                response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateDynamicScheduleResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnUpdateDynamicScheduleWSResponse event

                            try
                            {

                                OnUpdateDynamicScheduleWSResponse?.Invoke(Timestamp.Now,
                                                                          this,
                                                                          requestJSON,
                                                                          new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                             OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUpdateDynamicScheduleWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "NotifyAllowedEnergyTransfer":
                        {

                            #region Send OnNotifyAllowedEnergyTransferWSRequest event

                            try
                            {

                                OnNotifyAllowedEnergyTransferWSRequest?.Invoke(Timestamp.Now,
                                                                               this,
                                                                               requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyAllowedEnergyTransferWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (NotifyAllowedEnergyTransferRequest.TryParse(requestMessage.Message,
                                                                                requestMessage.RequestId,
                                                                                ChargeBoxIdentity,
                                                                                out var request,
                                                                                out var errorResponse,
                                                                                CustomNotifyAllowedEnergyTransferRequestParser) && request is not null) {

                                    #region Send OnNotifyAllowedEnergyTransferRequest event

                                    try
                                    {

                                        OnNotifyAllowedEnergyTransferRequest?.Invoke(Timestamp.Now,
                                                                                     this,
                                                                                     request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyAllowedEnergyTransferRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    NotifyAllowedEnergyTransferResponse? response = null;

                                    var results = OnNotifyAllowedEnergyTransfer?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnNotifyAllowedEnergyTransferDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                             this,
                                                                                                                                             request,
                                                                                                                                             cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= NotifyAllowedEnergyTransferResponse.Failed(request);

                                    #endregion

                                    #region Send OnNotifyAllowedEnergyTransferResponse event

                                    try
                                    {

                                        OnNotifyAllowedEnergyTransferResponse?.Invoke(Timestamp.Now,
                                                                                      this,
                                                                                      request,
                                                                                      response,
                                                                                      response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyAllowedEnergyTransferResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnNotifyAllowedEnergyTransferWSResponse event

                            try
                            {

                                OnNotifyAllowedEnergyTransferWSResponse?.Invoke(Timestamp.Now,
                                                                                this,
                                                                                requestJSON,
                                                                                new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                                   OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyAllowedEnergyTransferWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "UsePriorityCharging":
                        {

                            #region Send OnUsePriorityChargingWSRequest event

                            try
                            {

                                OnUsePriorityChargingWSRequest?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUsePriorityChargingWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (UsePriorityChargingRequest.TryParse(requestMessage.Message,
                                                                        requestMessage.RequestId,
                                                                        ChargeBoxIdentity,
                                                                        out var request,
                                                                        out var errorResponse,
                                                                        CustomUsePriorityChargingRequestParser) && request is not null) {

                                    #region Send OnUsePriorityChargingRequest event

                                    try
                                    {

                                        OnUsePriorityChargingRequest?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUsePriorityChargingRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    UsePriorityChargingResponse? response = null;

                                    var results = OnUsePriorityCharging?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnUsePriorityChargingDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                     this,
                                                                                                                                     request,
                                                                                                                                     cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= UsePriorityChargingResponse.Failed(request);

                                    #endregion

                                    #region Send OnUsePriorityChargingResponse event

                                    try
                                    {

                                        OnUsePriorityChargingResponse?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request,
                                                                              response,
                                                                              response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUsePriorityChargingResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnUsePriorityChargingWSResponse event

                            try
                            {

                                OnUsePriorityChargingWSResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        requestJSON,
                                                                        new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                           OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUsePriorityChargingWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "UnlockConnector":
                        {

                            #region Send OnUnlockConnectorWSRequest event

                            try
                            {

                                OnUnlockConnectorWSRequest?.Invoke(Timestamp.Now,
                                                                   this,
                                                                   requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUnlockConnectorWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (UnlockConnectorRequest.TryParse(requestMessage.Message,
                                                                    requestMessage.RequestId,
                                                                    ChargeBoxIdentity,
                                                                    out var request,
                                                                    out var errorResponse,
                                                                    CustomUnlockConnectorRequestParser) && request is not null) {

                                    #region Send OnUnlockConnectorRequest event

                                    try
                                    {

                                        OnUnlockConnectorRequest?.Invoke(Timestamp.Now,
                                                                         this,
                                                                         request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUnlockConnectorRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    UnlockConnectorResponse? response = null;

                                    var results = OnUnlockConnector?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnUnlockConnectorDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                 this,
                                                                                                                                 request,
                                                                                                                                 cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= UnlockConnectorResponse.Failed(request);

                                    #endregion

                                    #region Send OnUnlockConnectorResponse event

                                    try
                                    {

                                        OnUnlockConnectorResponse?.Invoke(Timestamp.Now,
                                                                          this,
                                                                          request,
                                                                          response,
                                                                          response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUnlockConnectorResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnUnlockConnectorWSResponse event

                            try
                            {

                                OnUnlockConnectorWSResponse?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    requestJSON,
                                                                    new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                       OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnUnlockConnectorWSResponse));
                            }

                            #endregion

                        }
                        break;


                    case "AFRRSignal":
                        {

                            #region Send OnAFRRSignalWSRequest event

                            try
                            {

                                OnAFRRSignalWSRequest?.Invoke(Timestamp.Now,
                                                              this,
                                                              requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnAFRRSignalWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (AFRRSignalRequest.TryParse(requestMessage.Message,
                                                               requestMessage.RequestId,
                                                               ChargeBoxIdentity,
                                                               out var request,
                                                               out var errorResponse,
                                                               CustomAFRRSignalRequestParser) && request is not null) {

                                    #region Send OnAFRRSignalRequest event

                                    try
                                    {

                                        OnAFRRSignalRequest?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnAFRRSignalRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    AFRRSignalResponse? response = null;

                                    var results = OnAFRRSignal?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnAFRRSignalDelegate)?.Invoke(Timestamp.Now,
                                                                                                                            this,
                                                                                                                            request,
                                                                                                                            cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= AFRRSignalResponse.Failed(request);

                                    #endregion

                                    #region Send OnAFRRSignalResponse event

                                    try
                                    {

                                        OnAFRRSignalResponse?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     request,
                                                                     response,
                                                                     response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnAFRRSignalResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnAFRRSignalWSResponse event

                            try
                            {

                                OnAFRRSignalWSResponse?.Invoke(Timestamp.Now,
                                                               this,
                                                               requestJSON,
                                                               new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                  OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnAFRRSignalWSResponse));
                            }

                            #endregion

                        }
                        break;


                    case "SetDisplayMessage":
                        {

                            #region Send OnSetDisplayMessageWSRequest event

                            try
                            {

                                OnSetDisplayMessageWSRequest?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetDisplayMessageWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (SetDisplayMessageRequest.TryParse(requestMessage.Message,
                                                                      requestMessage.RequestId,
                                                                      ChargeBoxIdentity,
                                                                      out var request,
                                                                      out var errorResponse,
                                                                      CustomSetDisplayMessageRequestParser) && request is not null) {

                                    #region Send OnSetDisplayMessageRequest event

                                    try
                                    {

                                        OnSetDisplayMessageRequest?.Invoke(Timestamp.Now,
                                                                           this,
                                                                           request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetDisplayMessageRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    SetDisplayMessageResponse? response = null;

                                    var results = OnSetDisplayMessage?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnSetDisplayMessageDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                   this,
                                                                                                                                   request,
                                                                                                                                   cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= SetDisplayMessageResponse.Failed(request);

                                    #endregion

                                    #region Send OnSetDisplayMessageResponse event

                                    try
                                    {

                                        OnSetDisplayMessageResponse?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request,
                                                                            response,
                                                                            response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetDisplayMessageResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnSetDisplayMessageWSResponse event

                            try
                            {

                                OnSetDisplayMessageWSResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      requestJSON,
                                                                      new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                         OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSetDisplayMessageWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "GetDisplayMessages":
                        {

                            #region Send OnGetDisplayMessagesWSRequest event

                            try
                            {

                                OnGetDisplayMessagesWSRequest?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetDisplayMessagesWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (GetDisplayMessagesRequest.TryParse(requestMessage.Message,
                                                                       requestMessage.RequestId,
                                                                       ChargeBoxIdentity,
                                                                       out var request,
                                                                       out var errorResponse,
                                                                       CustomGetDisplayMessagesRequestParser) && request is not null) {

                                    #region Send OnGetDisplayMessagesRequest event

                                    try
                                    {

                                        OnGetDisplayMessagesRequest?.Invoke(Timestamp.Now,
                                                                            this,
                                                                            request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetDisplayMessagesRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    GetDisplayMessagesResponse? response = null;

                                    var results = OnGetDisplayMessages?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnGetDisplayMessagesDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                    this,
                                                                                                                                    request,
                                                                                                                                    cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= GetDisplayMessagesResponse.Failed(request);

                                    #endregion

                                    #region Send OnGetDisplayMessagesResponse event

                                    try
                                    {

                                        OnGetDisplayMessagesResponse?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request,
                                                                             response,
                                                                             response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetDisplayMessagesResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnGetDisplayMessagesWSResponse event

                            try
                            {

                                OnGetDisplayMessagesWSResponse?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       requestJSON,
                                                                       new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                          OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetDisplayMessagesWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "ClearDisplayMessage":
                        {

                            #region Send OnClearDisplayMessageWSRequest event

                            try
                            {

                                OnClearDisplayMessageWSRequest?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearDisplayMessageWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (ClearDisplayMessageRequest.TryParse(requestMessage.Message,
                                                                        requestMessage.RequestId,
                                                                        ChargeBoxIdentity,
                                                                        out var request,
                                                                        out var errorResponse,
                                                                        CustomClearDisplayMessageRequestParser) && request is not null) {

                                    #region Send OnClearDisplayMessageRequest event

                                    try
                                    {

                                        OnClearDisplayMessageRequest?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearDisplayMessageRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    ClearDisplayMessageResponse? response = null;

                                    var results = OnClearDisplayMessage?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnClearDisplayMessageDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                     this,
                                                                                                                                     request,
                                                                                                                                     cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= ClearDisplayMessageResponse.Failed(request);

                                    #endregion

                                    #region Send OnClearDisplayMessageResponse event

                                    try
                                    {

                                        OnClearDisplayMessageResponse?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request,
                                                                              response,
                                                                              response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearDisplayMessageResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnClearDisplayMessageWSResponse event

                            try
                            {

                                OnClearDisplayMessageWSResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        requestJSON,
                                                                        new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                           OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearDisplayMessageWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "CostUpdated":
                        {

                            #region Send OnCostUpdatedWSRequest event

                            try
                            {

                                OnCostUpdatedWSRequest?.Invoke(Timestamp.Now,
                                                               this,
                                                               requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCostUpdatedWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (CostUpdatedRequest.TryParse(requestMessage.Message,
                                                                requestMessage.RequestId,
                                                                ChargeBoxIdentity,
                                                                out var request,
                                                                out var errorResponse,
                                                                CustomCostUpdatedRequestParser) && request is not null) {

                                    #region Send OnCostUpdatedRequest event

                                    try
                                    {

                                        OnCostUpdatedRequest?.Invoke(Timestamp.Now,
                                                                     this,
                                                                     request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCostUpdatedRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    CostUpdatedResponse? response = null;

                                    var results = OnCostUpdated?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnCostUpdatedDelegate)?.Invoke(Timestamp.Now,
                                                                                                                             this,
                                                                                                                             request,
                                                                                                                             cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= CostUpdatedResponse.Failed(request);

                                    #endregion

                                    #region Send OnCostUpdatedResponse event

                                    try
                                    {

                                        OnCostUpdatedResponse?.Invoke(Timestamp.Now,
                                                                      this,
                                                                      request,
                                                                      response,
                                                                      response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCostUpdatedResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnCostUpdatedWSResponse event

                            try
                            {

                                OnCostUpdatedWSResponse?.Invoke(Timestamp.Now,
                                                                this,
                                                                requestJSON,
                                                                new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                   OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCostUpdatedWSResponse));
                            }

                            #endregion

                        }
                        break;

                    case "CustomerInformation":
                        {

                            #region Send OnCustomerInformationWSRequest event

                            try
                            {

                                OnCustomerInformationWSRequest?.Invoke(Timestamp.Now,
                                                                       this,
                                                                       requestJSON);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCustomerInformationWSRequest));
                            }

                            #endregion

                            try
                            {

                                if (CustomerInformationRequest.TryParse(requestMessage.Message,
                                                                        requestMessage.RequestId,
                                                                        ChargeBoxIdentity,
                                                                        out var request,
                                                                        out var errorResponse,
                                                                        CustomCustomerInformationRequestParser) && request is not null) {

                                    #region Send OnCustomerInformationRequest event

                                    try
                                    {

                                        OnCustomerInformationRequest?.Invoke(Timestamp.Now,
                                                                             this,
                                                                             request);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCustomerInformationRequest));
                                    }

                                    #endregion

                                    #region Call async subscribers

                                    CustomerInformationResponse? response = null;

                                    var results = OnCustomerInformation?.
                                                      GetInvocationList()?.
                                                      SafeSelect(subscriber => (subscriber as OnCustomerInformationDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                     this,
                                                                                                                                     request,
                                                                                                                                     cancellationTokenSource.Token)).
                                                      ToArray();

                                    if (results?.Length > 0)
                                    {

                                        await Task.WhenAll(results!);

                                        response = results.FirstOrDefault()?.Result;

                                    }

                                    response ??= CustomerInformationResponse.Failed(request);

                                    #endregion

                                    #region Send OnCustomerInformationResponse event

                                    try
                                    {

                                        OnCustomerInformationResponse?.Invoke(Timestamp.Now,
                                                                              this,
                                                                              request,
                                                                              response,
                                                                              response.Runtime);

                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCustomerInformationResponse));
                                    }

                                    #endregion

                                    OCPPResponseJSON = response.ToJSON();

                                }

                                else
                                    ErrorMessage = OCPP_WebSocket_ErrorMessage.CouldNotParse(requestMessage.RequestId,
                                                                                             requestMessage.Action,
                                                                                             requestMessage.Message,
                                                                                             errorResponse);

                            }
                            catch (Exception e)
                            {
                                ErrorMessage = OCPP_WebSocket_ErrorMessage.FormationViolation(requestMessage.RequestId,
                                                                                              requestMessage.Action,
                                                                                              requestJSON,
                                                                                              e);
                            }

                            #region Send OnCustomerInformationWSResponse event

                            try
                            {

                                OnCustomerInformationWSResponse?.Invoke(Timestamp.Now,
                                                                        this,
                                                                        requestJSON,
                                                                        new OCPP_WebSocket_ResponseMessage(requestMessage.RequestId,
                                                                                                           OCPPResponseJSON ?? new JObject()).ToJSON());

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnCustomerInformationWSResponse));
                            }

                            #endregion

                        }
                        break;

                }

                if (OCPPResponseJSON is not null)
                {

                    await SendText(new OCPP_WebSocket_ResponseMessage(
                                       requestMessage.RequestId,
                                       OCPPResponseJSON).
                                       ToJSON().
                                       ToString(JSONFormatting));

                    #region OnTextMessageResponseSent

                    try
                    {

                        OnTextMessageResponseSent?.Invoke(Timestamp.Now,
                                                          this,
                                                          frame,
                                                          EventTracking_Id.New,
                                                          requestTimestamp,
                                                          requestJSON.     ToString(JSONFormatting),
                                                          Timestamp.Now,
                                                          OCPPResponseJSON.ToString(JSONFormatting));

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTextMessageResponseSent));
                    }

                    #endregion

                }

            }

            else if (OCPP_WebSocket_ResponseMessage.TryParse(textPayload, out var responseMessage) && responseMessage is not null)
            {
                lock (requests)
                {

                    if (requests.TryGetValue(responseMessage.RequestId, out var sendRequestState))
                    {

                        sendRequestState.ResponseTimestamp  = Timestamp.Now;
                        sendRequestState.Response           = responseMessage.Message;

                        #region OnTextMessageResponseReceived

                        try
                        {

                            OnTextMessageResponseReceived?.Invoke(Timestamp.Now,
                                                                  this,
                                                                  frame,
                                                                  EventTracking_Id.New,
                                                                  sendRequestState.Timestamp,
                                                                  sendRequestState.WSRequestMessage.ToJSON().ToString(JSONFormatting),
                                                                  sendRequestState.ResponseTimestamp.Value,
                                                                  sendRequestState.Response.ToString(JSONFormatting));

                        }
                        catch (Exception e)
                        {
                            DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTextMessageResponseReceived));
                        }

                        #endregion

                    }

                    else
                        DebugX.Log(nameof(ChargingStationWSClient), " Received unknown OCPP response message: " + textPayload);

                }
            }

            else if (OCPP_WebSocket_ErrorMessage.TryParse(textPayload, out var wsErrorMessage))
            {
                DebugX.Log(nameof(ChargingStationWSClient), " Received unknown OCPP error message: " + textPayload);
            }

            else
                DebugX.Log(nameof(ChargingStationWSClient), " Received unknown OCPP request/response message: " + textPayload);

        }

        #endregion


        public readonly Dictionary<Request_Id, SendRequestState2> requests = new ();


        #region SendRequest(Action, RequestId, Message)

        public async Task<OCPP_WebSocket_RequestMessage> SendRequest(String      Action,
                                                                     Request_Id  RequestId,
                                                                     JObject     Message)
        {

            OCPP_WebSocket_RequestMessage? wsRequestMessage = null;

            if (await MaintenanceSemaphore.WaitAsync(SemaphoreSlimTimeout).
                                           ConfigureAwait(false))
            {
                try
                {

                    if (HTTPStream is not null)
                    {

                        wsRequestMessage = new OCPP_WebSocket_RequestMessage(
                                               RequestId,
                                               Action,
                                               Message
                                           );

                        await SendText(wsRequestMessage.
                                       ToJSON().
                                       ToString(JSONFormatting));

                        requests.Add(RequestId,
                                     new SendRequestState2(
                                         Timestamp.Now,
                                         wsRequestMessage,
                                         Timestamp.Now + RequestTimeout
                                     ));

                    }
                    else
                    {

                        wsRequestMessage = new OCPP_WebSocket_RequestMessage(
                                               RequestId,
                                               Action,
                                               Message,
                                               ErrorMessage: "Invalid WebSocket connection!"
                                           );

                    }

                }
                catch (Exception e)
                {

                    while (e.InnerException is not null)
                        e = e.InnerException;

                    wsRequestMessage = new OCPP_WebSocket_RequestMessage(
                                           RequestId,
                                           Action,
                                           Message,
                                           ErrorMessage: e.Message
                                       );

                    DebugX.LogException(e);

                }
                finally
                {
                    MaintenanceSemaphore.Release();
                }
            }

            else
                wsRequestMessage = new OCPP_WebSocket_RequestMessage(
                                       RequestId,
                                       Action,
                                       Message,
                                       ErrorMessage: "Could not aquire the maintenance tasks lock!"
                                   );

            return wsRequestMessage;

        }

        #endregion

        #region (private) WaitForResponse(RequestMessage)

        private async Task<SendRequestState2> WaitForResponse(OCPP_WebSocket_RequestMessage RequestMessage)
        {

            var endTime = Timestamp.Now + RequestTimeout;

            #region Wait for a response... till timeout

            do
            {

                try
                {

                    await Task.Delay(25);

                    if (requests.TryGetValue(RequestMessage.RequestId, out var sendRequestState2) &&
                       (sendRequestState2?.Response is not null ||
                        sendRequestState2?.ErrorCode.HasValue == true))
                    {

                        lock (requests)
                        {
                            requests.Remove(RequestMessage.RequestId);
                        }

                        return sendRequestState2;

                    }

                }
                catch (Exception e)
                {
                    DebugX.Log(String.Concat(nameof(ChargingStationWSClient), ".", nameof(WaitForResponse), " exception occured: ", e.Message));
                }

            }
            while (Timestamp.Now < endTime);

            #endregion

            return new SendRequestState2(
                       Timestamp:          Timestamp.Now,
                       WSRequestMessage:   RequestMessage,
                       Timeout:            endTime,

                       Response:           null,
                       ErrorCode:          ResultCodes.Timeout,
                       ErrorDescription:   null,
                       ErrorDetails:       null
                   );

        }

        #endregion


        #region SendBootNotification                 (Request)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="Request">A BootNotification request.</param>
        public async Task<BootNotificationResponse>

            SendBootNotification(BootNotificationRequest Request)

        {

            #region Send OnBootNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnBootNotificationRequest?.Invoke(startTime,
                                                  this,
                                                  Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnBootNotificationRequest));
            }

            #endregion


            BootNotificationResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomBootNotificationRequestSerializer,
                                                                  CustomChargingStationSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (BootNotificationResponse.TryParse(Request,
                                                          sendRequestState.Response,
                                                          out var bootNotificationResponse,
                                                          out var errorResponse) &&
                        bootNotificationResponse is not null)
                    {
                        response = bootNotificationResponse;
                    }

                    response ??= new BootNotificationResponse(Request,
                                                              Result.Format(errorResponse));

                }

                response ??= new BootNotificationResponse(Request,
                                                          Result.FromSendRequestState(sendRequestState));

            }

            response ??= new BootNotificationResponse(Request,
                                                      Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnBootNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendFirmwareStatusNotification       (Request)

        /// <summary>
        /// Send a firmware status notification.
        /// </summary>
        /// <param name="Request">A FirmwareStatusNotification request.</param>
        public async Task<FirmwareStatusNotificationResponse>

            SendFirmwareStatusNotification(FirmwareStatusNotificationRequest Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnFirmwareStatusNotificationRequest));
            }

            #endregion


            FirmwareStatusNotificationResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomFirmwareStatusNotificationRequestSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (FirmwareStatusNotificationResponse.TryParse(Request,
                                                                    sendRequestState.Response,
                                                                    out var firmwareStatusNotificationResponse,
                                                                    out var errorResponse) &&
                        firmwareStatusNotificationResponse is not null)
                    {
                        response = firmwareStatusNotificationResponse;
                    }

                    response ??= new FirmwareStatusNotificationResponse(Request,
                                                                        Result.Format(errorResponse));

                }

                response ??= new FirmwareStatusNotificationResponse(Request,
                                                                    Result.FromSendRequestState(sendRequestState));

            }

            response ??= new FirmwareStatusNotificationResponse(Request,
                                                                Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendPublishFirmwareStatusNotification(Request)

        /// <summary>
        /// Send a publish firmware status notification.
        /// </summary>
        /// <param name="Request">A PublishFirmwareStatusNotification request.</param>
        public async Task<PublishFirmwareStatusNotificationResponse>

            SendPublishFirmwareStatusNotification(PublishFirmwareStatusNotificationRequest Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPublishFirmwareStatusNotificationRequest));
            }

            #endregion


            PublishFirmwareStatusNotificationResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomPublishFirmwareStatusNotificationRequestSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (PublishFirmwareStatusNotificationResponse.TryParse(Request,
                                                                           sendRequestState.Response,
                                                                           out var publishFirmwareStatusNotificationResponse,
                                                                           out var errorResponse) &&
                        publishFirmwareStatusNotificationResponse is not null)
                    {
                        response = publishFirmwareStatusNotificationResponse;
                    }

                    response ??= new PublishFirmwareStatusNotificationResponse(Request,
                                                                               Result.Format(errorResponse));

                }

                response ??= new PublishFirmwareStatusNotificationResponse(Request,
                                                                           Result.FromSendRequestState(sendRequestState));

            }

            response ??= new PublishFirmwareStatusNotificationResponse(Request,
                                                                       Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPublishFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendHeartbeat                        (Request)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="Request">A Heartbeat request.</param>
        public async Task<HeartbeatResponse>

            SendHeartbeat(HeartbeatRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnHeartbeatRequest));
            }

            #endregion


            HeartbeatResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomHeartbeatRequestSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (HeartbeatResponse.TryParse(Request,
                                                          sendRequestState.Response,
                                                          out var heartbeatResponse,
                                                          out var errorResponse) &&
                        heartbeatResponse is not null)
                    {
                        response = heartbeatResponse;
                    }

                    response ??= new HeartbeatResponse(Request,
                                                       Result.Format(errorResponse));

                }

                response ??= new HeartbeatResponse(Request,
                                                   Result.FromSendRequestState(sendRequestState));

            }

            response ??= new HeartbeatResponse(Request,
                                               Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnHeartbeatResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyEvent                          (Request)

        /// <summary>
        /// Notify about an event.
        /// </summary>
        /// <param name="Request">A NotifyEvent request.</param>
        public async Task<NotifyEventResponse>

            NotifyEvent(NotifyEventRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyEventRequest));
            }

            #endregion


            NotifyEventResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomNotifyEventRequestSerializer,
                                                                  CustomEventDataSerializer,
                                                                  CustomComponentSerializer,
                                                                  CustomEVSESerializer,
                                                                  CustomVariableSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (NotifyEventResponse.TryParse(Request,
                                                     sendRequestState.Response,
                                                     out var notifyEventResponse,
                                                     out var errorResponse) &&
                        notifyEventResponse is not null)
                    {
                        response = notifyEventResponse;
                    }

                    response ??= new NotifyEventResponse(Request,
                                                         Result.Format(errorResponse));

                }

                response ??= new NotifyEventResponse(Request,
                                                     Result.FromSendRequestState(sendRequestState));

            }

            response ??= new NotifyEventResponse(Request,
                                                 Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyEventResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendSecurityEventNotification        (Request)

        /// <summary>
        /// Send a security event notification.
        /// </summary>
        /// <param name="Request">A SecurityEventNotification request.</param>
        public async Task<SecurityEventNotificationResponse>

            SendSecurityEventNotification(SecurityEventNotificationRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSecurityEventNotificationRequest));
            }

            #endregion


            SecurityEventNotificationResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomSecurityEventNotificationSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (SecurityEventNotificationResponse.TryParse(Request,
                                                                   sendRequestState.Response,
                                                                   out var securityEventNotificationResponse,
                                                                   out var errorResponse) &&
                        securityEventNotificationResponse is not null)
                    {
                        response = securityEventNotificationResponse;
                    }

                    response ??= new SecurityEventNotificationResponse(Request,
                                                                       Result.Format(errorResponse));

                }

                response ??= new SecurityEventNotificationResponse(Request,
                                                                   Result.FromSendRequestState(sendRequestState));

            }

            response ??= new SecurityEventNotificationResponse(Request,
                                                               Result.GenericError(requestMessage.ErrorMessage));


            #region Send OnSecurityEventNotificationResponse event

            var endTime = Timestamp.Now;

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSecurityEventNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyReport                         (Request)

        /// <summary>
        /// Notify about a report.
        /// </summary>
        /// <param name="Request">A NotifyReport request.</param>
        public async Task<NotifyReportResponse>

            NotifyReport(NotifyReportRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyReportRequest));
            }

            #endregion


            NotifyReportResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomNotifyReportRequestSerializer,
                                                                  CustomReportDataSerializer,
                                                                  CustomComponentSerializer,
                                                                  CustomEVSESerializer,
                                                                  CustomVariableSerializer,
                                                                  CustomVariableAttributeSerializer,
                                                                  CustomVariableCharacteristicsSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (NotifyReportResponse.TryParse(Request,
                                                      sendRequestState.Response,
                                                      out var notifyReportResponse,
                                                      out var errorResponse) &&
                        notifyReportResponse is not null)
                    {
                        response = notifyReportResponse;
                    }

                    response ??= new NotifyReportResponse(Request,
                                                          Result.Format(errorResponse));

                }

                response ??= new NotifyReportResponse(Request,
                                                      Result.FromSendRequestState(sendRequestState));

            }

            response ??= new NotifyReportResponse(Request,
                                                  Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyReportResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyMonitoringReport               (Request)

        /// <summary>
        /// Notify about a monitoring report.
        /// </summary>
        /// <param name="Request">A NotifyMonitoringReport request.</param>
        public async Task<NotifyMonitoringReportResponse>

            NotifyMonitoringReport(NotifyMonitoringReportRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyMonitoringReportRequest));
            }

            #endregion


            NotifyMonitoringReportResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomNotifyMonitoringReportRequestSerializer,
                                                                  CustomMonitoringDataSerializer,
                                                                  CustomComponentSerializer,
                                                                  CustomEVSESerializer,
                                                                  CustomVariableSerializer,
                                                                  CustomVariableMonitoringSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (NotifyMonitoringReportResponse.TryParse(Request,
                                                                sendRequestState.Response,
                                                                out var notifyMonitoringReportResponse,
                                                                out var errorResponse) &&
                        notifyMonitoringReportResponse is not null)
                    {
                        response = notifyMonitoringReportResponse;
                    }

                    response ??= new NotifyMonitoringReportResponse(Request,
                                                                    Result.Format(errorResponse));

                }

                response ??= new NotifyMonitoringReportResponse(Request,
                                                                Result.FromSendRequestState(sendRequestState));

            }

            response ??= new NotifyMonitoringReportResponse(Request,
                                                            Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyMonitoringReportResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendLogStatusNotification            (Request)

        /// <summary>
        /// Send a log status notification.
        /// </summary>
        /// <param name="Request">A LogStatusNotification request.</param>
        public async Task<LogStatusNotificationResponse>

            SendLogStatusNotification(LogStatusNotificationRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnLogStatusNotificationRequest));
            }

            #endregion


            LogStatusNotificationResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomLogStatusNotificationSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (LogStatusNotificationResponse.TryParse(Request,
                                                               sendRequestState.Response,
                                                               out var logStatusNotificationResponse,
                                                               out var errorResponse) &&
                        logStatusNotificationResponse is not null)
                    {
                        response = logStatusNotificationResponse;
                    }

                    response ??= new LogStatusNotificationResponse(Request,
                                                                   Result.Format(errorResponse));

                }

                response ??= new LogStatusNotificationResponse(Request,
                                                               Result.FromSendRequestState(sendRequestState));

            }

            response ??= new LogStatusNotificationResponse(Request,
                                                           Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnLogStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region TransferData                         (Request)

        /// <summary>
        /// Send vendor-specific data.
        /// </summary>
        /// <param name="Request">A DataTransfer request.</param>
        public async Task<CSMS.DataTransferResponse>

            TransferData(DataTransferRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            CSMS.DataTransferResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomDataTransferRequestSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (CSMS.DataTransferResponse.TryParse(Request,
                                                           sendRequestState.Response,
                                                           out var dataTransferResponse,
                                                           out var errorResponse) &&
                        dataTransferResponse is not null)
                    {
                        response = dataTransferResponse;
                    }

                    response ??= new CSMS.DataTransferResponse(Request,
                                                               Result.Format(errorResponse));

                }

                response ??= new CSMS.DataTransferResponse(Request,
                                                           Result.FromSendRequestState(sendRequestState));

            }

            response ??= new CSMS.DataTransferResponse(Request,
                                                       Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region SendCertificateSigningRequest        (Request)

        /// <summary>
        /// Send a sign certificate request.
        /// </summary>
        /// <param name="Request">A SignCertificate request.</param>
        public async Task<SignCertificateResponse>

            SendCertificateSigningRequest(SignCertificateRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSignCertificateRequest));
            }

            #endregion


            SignCertificateResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomSignCertificateRequestSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (SignCertificateResponse.TryParse(Request,
                                                         sendRequestState.Response,
                                                         out var signCertificateResponse,
                                                         out var errorResponse) &&
                        signCertificateResponse is not null)
                    {
                        response = signCertificateResponse;
                    }

                    response ??= new SignCertificateResponse(Request,
                                                             Result.Format(errorResponse));

                }

                response ??= new SignCertificateResponse(Request,
                                                         Result.FromSendRequestState(sendRequestState));

            }

            response ??= new SignCertificateResponse(Request,
                                                     Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSignCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region Get15118EVCertificate                (Request)

        /// <summary>
        /// Request a 15118 EV certificate.
        /// </summary>
        /// <param name="Request">A Get15118EVCertificate request.</param>
        public async Task<Get15118EVCertificateResponse>

            Get15118EVCertificate(Get15118EVCertificateRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGet15118EVCertificateRequest));
            }

            #endregion


            Get15118EVCertificateResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomGet15118EVCertificateSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (Get15118EVCertificateResponse.TryParse(Request,
                                                               sendRequestState.Response,
                                                               out var get15118EVCertificateResponse,
                                                               out var errorResponse) &&
                        get15118EVCertificateResponse is not null)
                    {
                        response = get15118EVCertificateResponse;
                    }

                    response ??= new Get15118EVCertificateResponse(Request,
                                                                   Result.Format(errorResponse));

                }

                response ??= new Get15118EVCertificateResponse(Request,
                                                               Result.FromSendRequestState(sendRequestState));

            }

            response ??= new Get15118EVCertificateResponse(Request,
                                                           Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGet15118EVCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCertificateStatus                 (Request)

        /// <summary>
        /// Send a get certificate status request.
        /// </summary>
        /// <param name="Request">A GetCertificateStatus request.</param>
        public async Task<GetCertificateStatusResponse>

            GetCertificateStatus(GetCertificateStatusRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetCertificateStatusRequest));
            }

            #endregion


            GetCertificateStatusResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomGetCertificateStatusSerializer,
                                                                  CustomOCSPRequestDataSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (GetCertificateStatusResponse.TryParse(Request,
                                                              sendRequestState.Response,
                                                              out var getCertificateStatusResponse,
                                                              out var errorResponse) &&
                        getCertificateStatusResponse is not null)
                    {
                        response = getCertificateStatusResponse;
                    }

                    response ??= new GetCertificateStatusResponse(Request,
                                                                  Result.Format(errorResponse));

                }

                response ??= new GetCertificateStatusResponse(Request,
                                                              Result.FromSendRequestState(sendRequestState));

            }

            response ??= new GetCertificateStatusResponse(Request,
                                                          Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetCertificateStatusResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCRL                               (Request)

        /// <summary>
        /// Send a get certificate revocation list request.
        /// </summary>
        /// <param name="Request">A GetCRL request.</param>
        public async Task<GetCRLResponse>

            GetCRL(GetCRLRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetCRLRequest));
            }

            #endregion


            GetCRLResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomGetCRLSerializer,
                                                                  CustomCertificateHashDataSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (GetCRLResponse.TryParse(Request,
                                                sendRequestState.Response,
                                                out var getCertificateStatusResponse,
                                                out var errorResponse) &&
                        getCertificateStatusResponse is not null)
                    {
                        response = getCertificateStatusResponse;
                    }

                    response ??= new GetCRLResponse(Request,
                                                    Result.Format(errorResponse));

                }

                response ??= new GetCRLResponse(Request,
                                                Result.FromSendRequestState(sendRequestState));

            }

            response ??= new GetCRLResponse(Request,
                                            Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetCRLResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region SendReservationStatusUpdate          (Request)

        /// <summary>
        /// Send a reservation status update.
        /// </summary>
        /// <param name="Request">A ReservationStatusUpdate request.</param>
        public async Task<ReservationStatusUpdateResponse>

            SendReservationStatusUpdate(ReservationStatusUpdateRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReservationStatusUpdateRequest));
            }

            #endregion


            ReservationStatusUpdateResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomReservationStatusUpdateRequestSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (ReservationStatusUpdateResponse.TryParse(Request,
                                                                 sendRequestState.Response,
                                                                 out var reservationStatusUpdateResponse,
                                                                 out var errorResponse) &&
                        reservationStatusUpdateResponse is not null)
                    {
                        response = reservationStatusUpdateResponse;
                    }

                    response ??= new ReservationStatusUpdateResponse(Request,
                                                                     Result.Format(errorResponse));

                }

                response ??= new ReservationStatusUpdateResponse(Request,
                                                                 Result.FromSendRequestState(sendRequestState));

            }

            response ??= new ReservationStatusUpdateResponse(Request,
                                                             Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReservationStatusUpdateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region Authorize                            (Request)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="Request">An Authorize request.</param>
        public async Task<AuthorizeResponse>

            Authorize(AuthorizeRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnAuthorizeRequest));
            }

            #endregion


            AuthorizeResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomAuthorizeRequestSerializer,
                                                                  CustomIdTokenSerializer,
                                                                  CustomAdditionalInfoSerializer,
                                                                  CustomOCSPRequestDataSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (AuthorizeResponse.TryParse(Request,
                                                   sendRequestState.Response,
                                                   out var authorizeResponse,
                                                   out var errorResponse) &&
                        authorizeResponse is not null)
                    {
                        response = authorizeResponse;
                    }

                    response ??= new AuthorizeResponse(Request,
                                                       Result.Format(errorResponse));

                }

                response ??= new AuthorizeResponse(Request,
                                                   Result.FromSendRequestState(sendRequestState));

            }

            response ??= new AuthorizeResponse(Request,
                                               Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnAuthorizeResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyEVChargingNeeds                (Request)

        /// <summary>
        /// Notify about EV charging needs.
        /// </summary>
        /// <param name="Request">A NotifyEVChargingNeeds request.</param>
        public async Task<NotifyEVChargingNeedsResponse>

            NotifyEVChargingNeeds(NotifyEVChargingNeedsRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyEVChargingNeedsRequest));
            }

            #endregion


            NotifyEVChargingNeedsResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomNotifyEVChargingNeedsRequestSerializer,
                                                                  CustomChargingNeedsSerializer,
                                                                  CustomACChargingParametersSerializer,
                                                                  CustomDCChargingParametersSerializer,
                                                                  CustomV2XChargingParametersSerializer,
                                                                  CustomEVEnergyOfferSerializer,
                                                                  CustomEVPowerScheduleSerializer,
                                                                  CustomEVPowerScheduleEntrySerializer,
                                                                  CustomEVAbsolutePriceScheduleSerializer,
                                                                  CustomEVAbsolutePriceScheduleEntrySerializer,
                                                                  CustomEVPriceRuleSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (NotifyEVChargingNeedsResponse.TryParse(Request,
                                                               sendRequestState.Response,
                                                               out var notifyEVChargingNeedsResponse,
                                                               out var errorResponse) &&
                        notifyEVChargingNeedsResponse is not null)
                    {
                        response = notifyEVChargingNeedsResponse;
                    }

                    response ??= new NotifyEVChargingNeedsResponse(Request,
                                                                   Result.Format(errorResponse));

                }

                response ??= new NotifyEVChargingNeedsResponse(Request,
                                                               Result.FromSendRequestState(sendRequestState));

            }

            response ??= new NotifyEVChargingNeedsResponse(Request,
                                                           Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyEVChargingNeedsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendTransactionEvent                 (Request)

        /// <summary>
        /// Send a transaction event.
        /// </summary>
        /// <param name="Request">A TransactionEvent request.</param>
        public async Task<TransactionEventResponse>

            SendTransactionEvent(TransactionEventRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTransactionEventRequest));
            }

            #endregion


            TransactionEventResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomTransactionEventRequestSerializer,
                                                                  CustomTransactionSerializer,
                                                                  CustomIdTokenSerializer,
                                                                  CustomAdditionalInfoSerializer,
                                                                  CustomEVSESerializer,
                                                                  CustomMeterValueSerializer,
                                                                  CustomSampledValueSerializer,
                                                                  CustomSignedMeterValueSerializer,
                                                                  CustomUnitsOfMeasureSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (TransactionEventResponse.TryParse(Request,
                                                          sendRequestState.Response,
                                                          out var transactionEventResponse,
                                                          out var errorResponse) &&
                        transactionEventResponse is not null)
                    {
                        response = transactionEventResponse;
                    }

                    response ??= new TransactionEventResponse(Request,
                                                              Result.Format(errorResponse));

                }

                response ??= new TransactionEventResponse(Request,
                                                          Result.FromSendRequestState(sendRequestState));

            }

            response ??= new TransactionEventResponse(Request,
                                                      Result.GenericError(requestMessage.ErrorMessage));


            #region Send OnTransactionEventResponse event

            var endTime = Timestamp.Now;

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnTransactionEventResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendStatusNotification               (Request)

        /// <summary>
        /// Send a status notification for the given connector.
        /// </summary>
        /// <param name="Request">A StatusNotification request.</param>
        public async Task<StatusNotificationResponse>

            SendStatusNotification(StatusNotificationRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnStatusNotificationRequest));
            }

            #endregion


            StatusNotificationResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomStatusNotificationRequestSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (StatusNotificationResponse.TryParse(Request,
                                                            sendRequestState.Response,
                                                            out var statusNotificationResponse,
                                                            out var errorResponse) &&
                        statusNotificationResponse is not null)
                    {
                        response = statusNotificationResponse;
                    }

                    response ??= new StatusNotificationResponse(Request,
                                                                Result.Format(errorResponse));

                }

                response ??= new StatusNotificationResponse(Request,
                                                            Result.FromSendRequestState(sendRequestState));

            }

            response ??= new StatusNotificationResponse(Request,
                                                        Result.GenericError(requestMessage.ErrorMessage));


            #region Send OnStatusNotificationResponse event

            var endTime = Timestamp.Now;

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendMeterValues                      (Request)

        /// <summary>
        /// Send meter values.
        /// </summary>
        /// <param name="Request">A MeterValues request.</param>
        public async Task<MeterValuesResponse>

            SendMeterValues(MeterValuesRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnMeterValuesRequest));
            }

            #endregion


            MeterValuesResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomMeterValuesRequestSerializer,
                                                                  CustomMeterValueSerializer,
                                                                  CustomSampledValueSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (MeterValuesResponse.TryParse(Request,
                                                     sendRequestState.Response,
                                                     out var meterValuesResponse,
                                                     out var errorResponse) &&
                        meterValuesResponse is not null)
                    {
                        response = meterValuesResponse;
                    }

                    response ??= new MeterValuesResponse(Request,
                                                         Result.Format(errorResponse));

                }

                response ??= new MeterValuesResponse(Request,
                                                     Result.FromSendRequestState(sendRequestState));

            }

            response ??= new MeterValuesResponse(Request,
                                                 Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnMeterValuesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyChargingLimit                  (Request)

        /// <summary>
        /// Notify about charging limits.
        /// </summary>
        /// <param name="Request">A NotifyChargingLimit request.</param>
        public async Task<NotifyChargingLimitResponse>

            NotifyChargingLimit(NotifyChargingLimitRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyChargingLimitRequest));
            }

            #endregion


            NotifyChargingLimitResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomNotifyChargingLimitRequestSerializer,
                                                                  CustomChargingScheduleSerializer,
                                                                  CustomLimitBeyondSoCSerializer,
                                                                  CustomChargingSchedulePeriodSerializer,
                                                                  CustomV2XFreqWattEntrySerializer,
                                                                  CustomV2XSignalWattEntrySerializer,
                                                                  CustomSalesTariffSerializer,
                                                                  CustomSalesTariffEntrySerializer,
                                                                  CustomRelativeTimeIntervalSerializer,
                                                                  CustomConsumptionCostSerializer,
                                                                  CustomCostSerializer,

                                                                  CustomAbsolutePriceScheduleSerializer,
                                                                  CustomPriceRuleStackSerializer,
                                                                  CustomPriceRuleSerializer,
                                                                  CustomTaxRuleSerializer,
                                                                  CustomOverstayRuleListSerializer,
                                                                  CustomOverstayRuleSerializer,
                                                                  CustomAdditionalServiceSerializer,

                                                                  CustomPriceLevelScheduleSerializer,
                                                                  CustomPriceLevelScheduleEntrySerializer,

                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (NotifyChargingLimitResponse.TryParse(Request,
                                                             sendRequestState.Response,
                                                             out var notifyChargingLimitResponse,
                                                             out var errorResponse) &&
                        notifyChargingLimitResponse is not null)
                    {
                        response = notifyChargingLimitResponse;
                    }

                    response ??= new NotifyChargingLimitResponse(Request,
                                                                 Result.Format(errorResponse));

                }

                response ??= new NotifyChargingLimitResponse(Request,
                                                             Result.FromSendRequestState(sendRequestState));

            }

            response ??= new NotifyChargingLimitResponse(Request,
                                                         Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyChargingLimitResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SendClearedChargingLimit             (Request)

        /// <summary>
        /// Inform about a cleared charging limit.
        /// </summary>
        /// <param name="Request">A ClearedChargingLimit request.</param>
        public async Task<ClearedChargingLimitResponse>

            SendClearedChargingLimit(ClearedChargingLimitRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearedChargingLimitRequest));
            }

            #endregion


            ClearedChargingLimitResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomClearedChargingLimitRequestSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (ClearedChargingLimitResponse.TryParse(Request,
                                                              sendRequestState.Response,
                                                              out var clearedChargingLimitResponse,
                                                              out var errorResponse) &&
                        clearedChargingLimitResponse is not null)
                    {
                        response = clearedChargingLimitResponse;
                    }

                    response ??= new ClearedChargingLimitResponse(Request,
                                                                  Result.Format(errorResponse));

                }

                response ??= new ClearedChargingLimitResponse(Request,
                                                              Result.FromSendRequestState(sendRequestState));

            }

            response ??= new ClearedChargingLimitResponse(Request,
                                                          Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnClearedChargingLimitResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ReportChargingProfiles               (Request)

        /// <summary>
        /// Report about charging profiles.
        /// </summary>
        /// <param name="Request">A ReportChargingProfiles request.</param>
        public async Task<ReportChargingProfilesResponse>

            ReportChargingProfiles(ReportChargingProfilesRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReportChargingProfilesRequest));
            }

            #endregion


            ReportChargingProfilesResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomReportChargingProfilesRequestSerializer,
                                                                  CustomChargingProfileSerializer,
                                                                  CustomLimitBeyondSoCSerializer,
                                                                  CustomChargingScheduleSerializer,
                                                                  CustomChargingSchedulePeriodSerializer,
                                                                  CustomV2XFreqWattEntrySerializer,
                                                                  CustomV2XSignalWattEntrySerializer,
                                                                  CustomSalesTariffSerializer,
                                                                  CustomSalesTariffEntrySerializer,
                                                                  CustomRelativeTimeIntervalSerializer,
                                                                  CustomConsumptionCostSerializer,
                                                                  CustomCostSerializer,

                                                                  CustomAbsolutePriceScheduleSerializer,
                                                                  CustomPriceRuleStackSerializer,
                                                                  CustomPriceRuleSerializer,
                                                                  CustomTaxRuleSerializer,
                                                                  CustomOverstayRuleListSerializer,
                                                                  CustomOverstayRuleSerializer,
                                                                  CustomAdditionalServiceSerializer,

                                                                  CustomPriceLevelScheduleSerializer,
                                                                  CustomPriceLevelScheduleEntrySerializer,

                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (ReportChargingProfilesResponse.TryParse(Request,
                                                                sendRequestState.Response,
                                                                out var reportChargingProfilesResponse,
                                                                out var errorResponse) &&
                        reportChargingProfilesResponse is not null)
                    {
                        response = reportChargingProfilesResponse;
                    }

                    response ??= new ReportChargingProfilesResponse(Request,
                                                                    Result.Format(errorResponse));

                }

                response ??= new ReportChargingProfilesResponse(Request,
                                                                Result.FromSendRequestState(sendRequestState));

            }

            response ??= new ReportChargingProfilesResponse(Request,
                                                            Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnReportChargingProfilesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyEVChargingSchedule             (Request)

        /// <summary>
        /// Notify about an EV charging schedule.
        /// </summary>
        /// <param name="Request">A NotifyEVChargingSchedule request.</param>
        public async Task<NotifyEVChargingScheduleResponse>

            NotifyEVChargingSchedule(NotifyEVChargingScheduleRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyEVChargingScheduleRequest));
            }

            #endregion


            NotifyEVChargingScheduleResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomNotifyEVChargingScheduleRequestSerializer,
                                                                  CustomChargingScheduleSerializer,
                                                                  CustomLimitBeyondSoCSerializer,
                                                                  CustomChargingSchedulePeriodSerializer,
                                                                  CustomV2XFreqWattEntrySerializer,
                                                                  CustomV2XSignalWattEntrySerializer,
                                                                  CustomSalesTariffSerializer,
                                                                  CustomSalesTariffEntrySerializer,
                                                                  CustomRelativeTimeIntervalSerializer,
                                                                  CustomConsumptionCostSerializer,
                                                                  CustomCostSerializer,

                                                                  CustomAbsolutePriceScheduleSerializer,
                                                                  CustomPriceRuleStackSerializer,
                                                                  CustomPriceRuleSerializer,
                                                                  CustomTaxRuleSerializer,
                                                                  CustomOverstayRuleListSerializer,
                                                                  CustomOverstayRuleSerializer,
                                                                  CustomAdditionalServiceSerializer,

                                                                  CustomPriceLevelScheduleSerializer,
                                                                  CustomPriceLevelScheduleEntrySerializer,

                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (NotifyEVChargingScheduleResponse.TryParse(Request,
                                                                  sendRequestState.Response,
                                                                  out var reportChargingProfilesResponse,
                                                                  out var errorResponse) &&
                        reportChargingProfilesResponse is not null)
                    {
                        response = reportChargingProfilesResponse;
                    }

                    response ??= new NotifyEVChargingScheduleResponse(Request,
                                                                      Result.Format(errorResponse));

                }

                response ??= new NotifyEVChargingScheduleResponse(Request,
                                                                  Result.FromSendRequestState(sendRequestState));

            }

            response ??= new NotifyEVChargingScheduleResponse(Request,
                                                              Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyEVChargingScheduleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyPriorityCharging               (Request)

        /// <summary>
        /// Notify about priority charging.
        /// </summary>
        /// <param name="Request">A NotifyPriorityCharging request.</param>
        public async Task<NotifyPriorityChargingResponse>

            NotifyPriorityCharging(NotifyPriorityChargingRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyPriorityChargingRequest));
            }

            #endregion


            NotifyPriorityChargingResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomNotifyPriorityChargingRequestSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (NotifyPriorityChargingResponse.TryParse(Request,
                                                                sendRequestState.Response,
                                                                out var reportChargingProfilesResponse,
                                                                out var errorResponse) &&
                        reportChargingProfilesResponse is not null)
                    {
                        response = reportChargingProfilesResponse;
                    }

                    response ??= new NotifyPriorityChargingResponse(Request,
                                                                    Result.Format(errorResponse));

                }

                response ??= new NotifyPriorityChargingResponse(Request,
                                                                Result.FromSendRequestState(sendRequestState));

            }

            response ??= new NotifyPriorityChargingResponse(Request,
                                                            Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyPriorityChargingResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PullDynamicScheduleUpdate            (Request)

        /// <summary>
        /// Pull a dynamic schedule update.
        /// </summary>
        /// <param name="Request">A PullDynamicScheduleUpdate request.</param>
        public async Task<PullDynamicScheduleUpdateResponse>

            PullDynamicScheduleUpdate(PullDynamicScheduleUpdateRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPullDynamicScheduleUpdateRequest));
            }

            #endregion


            PullDynamicScheduleUpdateResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomPullDynamicScheduleUpdateRequestSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (PullDynamicScheduleUpdateResponse.TryParse(Request,
                                                                   sendRequestState.Response,
                                                                   out var reportChargingProfilesResponse,
                                                                   out var errorResponse) &&
                        reportChargingProfilesResponse is not null)
                    {
                        response = reportChargingProfilesResponse;
                    }

                    response ??= new PullDynamicScheduleUpdateResponse(Request,
                                                                       Result.Format(errorResponse));

                }

                response ??= new PullDynamicScheduleUpdateResponse(Request,
                                                                   Result.FromSendRequestState(sendRequestState));

            }

            response ??= new PullDynamicScheduleUpdateResponse(Request,
                                                               Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnPullDynamicScheduleUpdateResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region NotifyDisplayMessages                (Request)

        /// <summary>
        /// Notify about display messages.
        /// </summary>
        /// <param name="Request">A NotifyDisplayMessages request.</param>
        public async Task<NotifyDisplayMessagesResponse>

            NotifyDisplayMessages(NotifyDisplayMessagesRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyDisplayMessagesRequest));
            }

            #endregion


            NotifyDisplayMessagesResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomNotifyDisplayMessagesRequestSerializer,
                                                                  CustomMessageInfoSerializer,
                                                                  CustomMessageContentSerializer,
                                                                  CustomComponentSerializer,
                                                                  CustomEVSESerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (NotifyDisplayMessagesResponse.TryParse(Request,
                                                               sendRequestState.Response,
                                                               out var notifyDisplayMessagesResponse,
                                                               out var errorResponse) &&
                        notifyDisplayMessagesResponse is not null)
                    {
                        response = notifyDisplayMessagesResponse;
                    }

                    response ??= new NotifyDisplayMessagesResponse(Request,
                                                                   Result.Format(errorResponse));

                }

                response ??= new NotifyDisplayMessagesResponse(Request,
                                                               Result.FromSendRequestState(sendRequestState));

            }

            response ??= new NotifyDisplayMessagesResponse(Request,
                                                           Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyDisplayMessagesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyCustomerInformation            (Request)

        /// <summary>
        /// Notify about customer information.
        /// </summary>
        /// <param name="Request">A NotifyCustomerInformation request.</param>
        public async Task<NotifyCustomerInformationResponse>

            NotifyCustomerInformation(NotifyCustomerInformationRequest  Request)

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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyCustomerInformationRequest));
            }

            #endregion


            NotifyCustomerInformationResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(CustomNotifyCustomerInformationRequestSerializer,
                                                                  CustomCustomDataSerializer));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (NotifyCustomerInformationResponse.TryParse(Request,
                                                                   sendRequestState.Response,
                                                                   out var notifyCustomerInformationResponse,
                                                                   out var errorResponse) &&
                        notifyCustomerInformationResponse is not null)
                    {
                        response = notifyCustomerInformationResponse;
                    }

                    response ??= new NotifyCustomerInformationResponse(Request,
                                                                       Result.Format(errorResponse));

                }

                response ??= new NotifyCustomerInformationResponse(Request,
                                                                   Result.FromSendRequestState(sendRequestState));

            }

            response ??= new NotifyCustomerInformationResponse(Request,
                                                               Result.GenericError(requestMessage.ErrorMessage));


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
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyCustomerInformationResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
