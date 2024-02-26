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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using Newtonsoft.Json.Linq;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Illias;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// OCPP Charging Station Management System HTTP API extensions.
    /// </summary>
    public static class HTTPAPIExtensions
    {

        #region ParseChargingStationId(this HTTPRequest, OCPPHTTPAPI, out ChargingStationId,                out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the charging station identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPPHTTPAPI">The OCPP HTTP API.</param>
        /// <param name="ChargingStationId">The parsed unique charging station identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        public static Boolean ParseChargingStationId(this HTTPRequest                                HTTPRequest,
                                                     HTTPAPI                                         OCPPHTTPAPI,
                                                     [NotNullWhen(true)]  out ChargingStation_Id?    ChargingStationId,
                                                     [NotNullWhen(false)] out HTTPResponse.Builder?  HTTPResponse)
        {

            ChargingStationId  = null;
            HTTPResponse       = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPHTTPAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            ChargingStationId = ChargingStation_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!ChargingStationId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPHTTPAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.Application.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid charging station identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseChargingStation  (this HTTPRequest, OCPPHTTPAPI, out ChargingStationId, out ChargingStation, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the charging station identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPPHTTPAPI">The OCPP HTTP API.</param>
        /// <param name="ChargingStationId">The parsed unique charging station identification.</param>
        /// <param name="ChargingStation">The resolved charging station.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        public static Boolean ParseChargingStation(this HTTPRequest                                HTTPRequest,
                                                   HTTPAPI                                         OCPPHTTPAPI,
                                                   [NotNullWhen(true)]  out ChargingStation_Id?    ChargingStationId,
                                                   [NotNullWhen(true)]  out ChargingStation?       ChargingStation,
                                                   [NotNullWhen(false)] out HTTPResponse.Builder?  HTTPResponse)
        {

            ChargingStationId  = null;
            ChargingStation    = null;
            HTTPResponse       = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPHTTPAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            ChargingStationId = ChargingStation_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!ChargingStationId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPHTTPAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.Application.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid charging station identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            foreach (var csms in OCPPHTTPAPI.CSMSs)
            {
                if (csms.TryGetChargingStation(ChargingStationId.Value, out ChargingStation))
                    return true;
            }

            HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                HTTPStatusCode  = HTTPStatusCode.NotFound,
                Server          = OCPPHTTPAPI.HTTPServiceName,
                Date            = Timestamp.Now,
                ContentType     = HTTPContentType.Application.JSON_UTF8,
                Content         = @"{ ""description"": ""Unknown charging station identification!"" }".ToUTF8Bytes(),
                Connection      = "close"
            };

            return false;

        }

        #endregion

    }


    /// <summary>
    /// The OCPP Charging Station Management System HTTP API.
    /// </summary>
    public class HTTPAPI : AHTTPAPIExtension<HTTPExtAPI>,
                           IHTTPAPIExtension<HTTPExtAPI>
    {

        #region Data

        /// <summary>
        /// The default HTTP URL prefix.
        /// </summary>
        public readonly HTTPPath                   DefaultURLPathPrefix      = HTTPPath.Parse("webapi");

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public const    String                     DefaultHTTPServerName     = $"Open Charging Cloud OCPP {Version.String} CSMS HTTP API";

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public const String                        DefaultHTTPRealm          = "Open Charging Cloud OCPP CSMS HTTP API";

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public const String                        HTTPRoot                  = "cloud.charging.open.protocols.OCPPv2_1.CSMS.HTTPAPI.HTTPRoot.";


        //ToDo: http://www.iana.org/form/media-types

        /// <summary>
        /// The HTTP content type for serving OCPP+ XML data.
        /// </summary>
        public static readonly HTTPContentType     OCPPPlusJSONContentType   = new ("application", "vnd.OCPPPlus+json", "utf-8", null, null);

        /// <summary>
        /// The HTTP content type for serving OCPP+ HTML data.
        /// </summary>
        public static readonly HTTPContentType     OCPPPlusHTMLContentType   = new ("application", "vnd.OCPPPlus+html", "utf-8", null, null);

        /// <summary>
        /// The unique identification of the OCPP HTTP SSE event log.
        /// </summary>
        public static readonly HTTPEventSource_Id  EventLogId                = HTTPEventSource_Id.Parse("OCPPEvents");


        protected readonly  List<ACSMS>            csmss                     = [];

        #endregion

        #region Properties

        /// <summary>
        /// An enumeration of registered Charging Station Management Systems.
        /// </summary>
        public IEnumerable<ACSMS>                         CSMSs
            => csmss;

        /// <summary>
        /// The HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public String?                                    HTTPRealm           { get; }

        /// <summary>
        /// An enumeration of logins for an optional HTTP Basic Authentication.
        /// </summary>
        public IEnumerable<KeyValuePair<String, String>>  HTTPLogins          { get; }

        /// <summary>
        /// Send debug information via HTTP Server Sent Events.
        /// </summary>
        public HTTPEventSource<JObject>                   EventLog            { get; }

        #endregion

        #region Custom JSON serializer delegates

        #region Charging Station Messages

        public CustomJObjectSerializerDelegate<BootNotificationRequest>?                             CustomBootNotificationRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<BootNotificationResponse>?                            CustomBootNotificationResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<FirmwareStatusNotificationRequest>?                   CustomFirmwareStatusNotificationRequestSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<FirmwareStatusNotificationResponse>?                  CustomFirmwareStatusNotificationResponseSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<PublishFirmwareStatusNotificationRequest>?            CustomPublishFirmwareStatusNotificationRequestSerializer     { get; set; }
        public CustomJObjectSerializerDelegate<PublishFirmwareStatusNotificationResponse>?           CustomPublishFirmwareStatusNotificationResponseSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<HeartbeatRequest>?                                    CustomHeartbeatRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<HeartbeatResponse>?                                   CustomHeartbeatResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<NotifyEventRequest>?                                  CustomNotifyEventRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<NotifyEventResponse>?                                 CustomNotifyEventResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<SecurityEventNotificationRequest>?                    CustomSecurityEventNotificationRequestSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<SecurityEventNotificationResponse>?                   CustomSecurityEventNotificationResponseSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<NotifyReportRequest>?                                 CustomNotifyReportRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<NotifyReportResponse>?                                CustomNotifyReportResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<NotifyMonitoringReportRequest>?                       CustomNotifyMonitoringReportRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<NotifyMonitoringReportResponse>?                      CustomNotifyMonitoringReportResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<LogStatusNotificationRequest>?                        CustomLogStatusNotificationRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<LogStatusNotificationResponse>?                       CustomLogStatusNotificationResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<DataTransferRequest>?                                 CustomDataTransferRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<DataTransferResponse>?                                CustomDataTransferResponseSerializer                         { get; set; }

        public CustomJObjectSerializerDelegate<SignCertificateRequest>?                              CustomSignCertificateRequestSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<SignCertificateResponse>?                             CustomSignCertificateResponseSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<Get15118EVCertificateRequest>?                        CustomGet15118EVCertificateRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<Get15118EVCertificateResponse>?                       CustomGet15118EVCertificateResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<GetCertificateStatusRequest>?                         CustomGetCertificateStatusRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<GetCertificateStatusResponse>?                        CustomGetCertificateStatusResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<GetCRLRequest>?                                       CustomGetCRLRequestSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<GetCRLResponse>?                                      CustomGetCRLResponseSerializer                               { get; set; }

        public CustomJObjectSerializerDelegate<ReservationStatusUpdateRequest>?                      CustomReservationStatusUpdateRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<ReservationStatusUpdateResponse>?                     CustomReservationStatusUpdateResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizeRequest>?                                    CustomAuthorizeRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizeResponse>?                                   CustomAuthorizeResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<NotifyEVChargingNeedsRequest>?                        CustomNotifyEVChargingNeedsRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<NotifyEVChargingNeedsResponse>?                       CustomNotifyEVChargingNeedsResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<TransactionEventRequest>?                             CustomTransactionEventRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<TransactionEventResponse>?                            CustomTransactionEventResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<StatusNotificationRequest>?                           CustomStatusNotificationRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<StatusNotificationResponse>?                          CustomStatusNotificationResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<MeterValuesRequest>?                                  CustomMeterValuesRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<MeterValuesResponse>?                                 CustomMeterValuesResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<NotifyChargingLimitRequest>?                          CustomNotifyChargingLimitRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<NotifyChargingLimitResponse>?                         CustomNotifyChargingLimitResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<ClearedChargingLimitRequest>?                         CustomClearedChargingLimitRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<ClearedChargingLimitResponse>?                        CustomClearedChargingLimitResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<ReportChargingProfilesRequest>?                       CustomReportChargingProfilesRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<ReportChargingProfilesResponse>?                      CustomReportChargingProfilesResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<NotifyEVChargingScheduleRequest>?                     CustomNotifyEVChargingScheduleRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<NotifyEVChargingScheduleResponse>?                    CustomNotifyEVChargingScheduleResponseSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<NotifyPriorityChargingRequest>?                       CustomNotifyPriorityChargingRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<NotifySettlementRequest>?                             CustomNotifySettlementRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<NotifyPriorityChargingResponse>?                      CustomNotifyPriorityChargingResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<NotifySettlementResponse>?                            CustomNotifySettlementResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<PullDynamicScheduleUpdateRequest>?                    CustomPullDynamicScheduleUpdateRequestSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<PullDynamicScheduleUpdateResponse>?                   CustomPullDynamicScheduleUpdateResponseSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<NotifyDisplayMessagesRequest>?                        CustomNotifyDisplayMessagesRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<NotifyDisplayMessagesResponse>?                       CustomNotifyDisplayMessagesResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<NotifyCustomerInformationRequest>?                    CustomNotifyCustomerInformationRequestSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<NotifyCustomerInformationResponse>?                   CustomNotifyCustomerInformationResponseSerializer            { get; set; }

        #endregion

        #region CSMS Messages

        public CustomJObjectSerializerDelegate<ResetRequest>?                                        CustomResetRequestSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<ResetResponse>?                                       CustomResetResponseSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<UpdateFirmwareRequest>?                               CustomUpdateFirmwareRequestSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<UpdateFirmwareResponse>?                              CustomUpdateFirmwareResponseSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<PublishFirmwareRequest>?                              CustomPublishFirmwareRequestSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<PublishFirmwareResponse>?                             CustomPublishFirmwareResponseSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<UnpublishFirmwareRequest>?                            CustomUnpublishFirmwareRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<UnpublishFirmwareResponse>?                           CustomUnpublishFirmwareResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<GetBaseReportRequest>?                                CustomGetBaseReportRequestSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<GetBaseReportResponse>?                               CustomGetBaseReportResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<GetReportRequest>?                                    CustomGetReportRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<GetReportResponse>?                                   CustomGetReportResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<GetLogRequest>?                                       CustomGetLogRequestSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<GetLogResponse>?                                      CustomGetLogResponseSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<SetVariablesRequest>?                                 CustomSetVariablesRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<SetVariablesResponse>?                                CustomSetVariablesResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<GetVariablesRequest>?                                 CustomGetVariablesRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<GetVariablesResponse>?                                CustomGetVariablesResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringBaseRequest>?                            CustomSetMonitoringBaseRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringBaseResponse>?                           CustomSetMonitoringBaseResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<GetMonitoringReportRequest>?                          CustomGetMonitoringReportRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<GetMonitoringReportResponse>?                         CustomGetMonitoringReportResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringLevelRequest>?                           CustomSetMonitoringLevelRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringLevelResponse>?                          CustomSetMonitoringLevelResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<SetVariableMonitoringRequest>?                        CustomSetVariableMonitoringRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<SetVariableMonitoringResponse>?                       CustomSetVariableMonitoringResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<ClearVariableMonitoringRequest>?                      CustomClearVariableMonitoringRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<ClearVariableMonitoringResponse>?                     CustomClearVariableMonitoringResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<SetNetworkProfileRequest>?                            CustomSetNetworkProfileRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<SetNetworkProfileResponse>?                           CustomSetNetworkProfileResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<ChangeAvailabilityRequest>?                           CustomChangeAvailabilityRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<ChangeAvailabilityResponse>?                          CustomChangeAvailabilityResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<TriggerMessageRequest>?                               CustomTriggerMessageRequestSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<TriggerMessageResponse>?                              CustomTriggerMessageResponseSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<DataTransferRequest>?                                 CustomData2TransferRequestSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<DataTransferResponse>?                                CustomData2TransferResponseSerializer                        { get; set; }

        public CustomJObjectSerializerDelegate<CertificateSignedRequest>?                            CustomCertificateSignedRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CertificateSignedResponse>?                           CustomCertificateSignedResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<InstallCertificateRequest>?                           CustomInstallCertificateRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<InstallCertificateResponse>?                          CustomInstallCertificateResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<GetInstalledCertificateIdsRequest>?                   CustomGetInstalledCertificateIdsRequestSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<GetInstalledCertificateIdsResponse>?                  CustomGetInstalledCertificateIdsResponseSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<DeleteCertificateRequest>?                            CustomDeleteCertificateRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<DeleteCertificateResponse>?                           CustomDeleteCertificateResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<NotifyCRLRequest>?                                    CustomNotifyCRLRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<NotifyCRLResponse>?                                   CustomNotifyCRLResponseSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<GetLocalListVersionRequest>?                          CustomGetLocalListVersionRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<GetLocalListVersionResponse>?                         CustomGetLocalListVersionResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<SendLocalListRequest>?                                CustomSendLocalListRequestSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<SendLocalListResponse>?                               CustomSendLocalListResponseSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ClearCacheRequest>?                                   CustomClearCacheRequestSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<ClearCacheResponse>?                                  CustomClearCacheResponseSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<ReserveNowRequest>?                                   CustomReserveNowRequestSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<ReserveNowResponse>?                                  CustomReserveNowResponseSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CancelReservationRequest>?                            CustomCancelReservationRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<CancelReservationResponse>?                           CustomCancelReservationResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<RequestStartTransactionRequest>?                      CustomRequestStartTransactionRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<RequestStartTransactionResponse>?                     CustomRequestStartTransactionResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<RequestStopTransactionRequest>?                       CustomRequestStopTransactionRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<RequestStopTransactionResponse>?                      CustomRequestStopTransactionResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<GetTransactionStatusRequest>?                         CustomGetTransactionStatusRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<GetTransactionStatusResponse>?                        CustomGetTransactionStatusResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<SetChargingProfileRequest>?                           CustomSetChargingProfileRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<SetChargingProfileResponse>?                          CustomSetChargingProfileResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<GetChargingProfilesRequest>?                          CustomGetChargingProfilesRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<GetChargingProfilesResponse>?                         CustomGetChargingProfilesResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<ClearChargingProfileRequest>?                         CustomClearChargingProfileRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<ClearChargingProfileResponse>?                        CustomClearChargingProfileResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<GetCompositeScheduleRequest>?                         CustomGetCompositeScheduleRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<GetCompositeScheduleResponse>?                        CustomGetCompositeScheduleResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<UpdateDynamicScheduleRequest>?                        CustomUpdateDynamicScheduleRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<UpdateDynamicScheduleResponse>?                       CustomUpdateDynamicScheduleResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<NotifyAllowedEnergyTransferRequest>?                  CustomNotifyAllowedEnergyTransferRequestSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<NotifyAllowedEnergyTransferResponse>?                 CustomNotifyAllowedEnergyTransferResponseSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<UsePriorityChargingRequest>?                          CustomUsePriorityChargingRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<UsePriorityChargingResponse>?                         CustomUsePriorityChargingResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<UnlockConnectorRequest>?                              CustomUnlockConnectorRequestSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<UnlockConnectorResponse>?                             CustomUnlockConnectorResponseSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<AFRRSignalRequest>?                                   CustomAFRRSignalRequestSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<AFRRSignalResponse>?                                  CustomAFRRSignalResponseSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<SetDisplayMessageRequest>?                            CustomSetDisplayMessageRequestSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<SetDisplayMessageResponse>?                           CustomSetDisplayMessageResponseSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<GetDisplayMessagesRequest>?                           CustomGetDisplayMessagesRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<GetDisplayMessagesResponse>?                          CustomGetDisplayMessagesResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<ClearDisplayMessageRequest>?                          CustomClearDisplayMessageRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<ClearDisplayMessageResponse>?                         CustomClearDisplayMessageResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CostUpdatedRequest>?                                  CustomCostUpdatedRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CostUpdatedResponse>?                                 CustomCostUpdatedResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<CustomerInformationRequest>?                          CustomCustomerInformationRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<CustomerInformationResponse>?                         CustomCustomerInformationResponseSerializer                  { get; set; }

        #endregion

        #region Data Structures

        public CustomJObjectSerializerDelegate<OCPPv2_1.ChargingStation>?                            CustomChargingStationSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<OCPP.StatusInfo>?                                     CustomStatusInfoSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<OCPP.Signature>?                                      CustomSignatureSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<OCPP.CustomData>?                                     CustomCustomDataSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<EventData>?                                           CustomEventDataSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<Component>?                                           CustomComponentSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                                                CustomEVSESerializer                                         { get; set; }
        public CustomJObjectSerializerDelegate<Variable>?                                            CustomVariableSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<PeriodicEventStreamParameters>?                       CustomPeriodicEventStreamParametersSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<ReportData>?                                          CustomReportDataSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<VariableAttribute>?                                   CustomVariableAttributeSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<VariableCharacteristics>?                             CustomVariableCharacteristicsSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<MonitoringData>?                                      CustomMonitoringDataSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<VariableMonitoring>?                                  CustomVariableMonitoringSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<OCSPRequestData>?                                     CustomOCSPRequestDataSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<CertificateHashData>?                                 CustomCertificateHashDataSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<IdToken>?                                             CustomIdTokenSerializer                                      { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalInfo>?                                      CustomAdditionalInfoSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<IdTokenInfo>?                                         CustomIdTokenInfoSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<MessageContent>?                                      CustomMessageContentSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ChargingNeeds>?                                       CustomChargingNeedsSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<ACChargingParameters>?                                CustomACChargingParametersSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<DCChargingParameters>?                                CustomDCChargingParametersSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<V2XChargingParameters>?                               CustomV2XChargingParametersSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<EVEnergyOffer>?                                       CustomEVEnergyOfferSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<EVPowerSchedule>?                                     CustomEVPowerScheduleSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<EVPowerScheduleEntry>?                                CustomEVPowerScheduleEntrySerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EVAbsolutePriceSchedule>?                             CustomEVAbsolutePriceScheduleSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?                        CustomEVAbsolutePriceScheduleEntrySerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<EVPriceRule>?                                         CustomEVPriceRuleSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<Transaction>?                                         CustomTransactionSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<MeterValue>?                                          CustomMeterValueSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<SampledValue>?                                        CustomSampledValueSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<SignedMeterValue>?                                    CustomSignedMeterValueSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<UnitsOfMeasure>?                                      CustomUnitsOfMeasureSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?                                    CustomChargingScheduleSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<LimitBeyondSoC>?                                      CustomLimitBeyondSoCSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                              CustomChargingSchedulePeriodSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                                    CustomV2XFreqWattEntrySerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                                  CustomV2XSignalWattEntrySerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariff>?                                         CustomSalesTariffSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariffEntry>?                                    CustomSalesTariffEntrySerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<RelativeTimeInterval>?                                CustomRelativeTimeIntervalSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<ConsumptionCost>?                                     CustomConsumptionCostSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<Cost>?                                                CustomCostSerializer                                         { get; set; }

        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AbsolutePriceSchedule>?    CustomAbsolutePriceScheduleSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRuleStack>?           CustomPriceRuleStackSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRule>?                CustomPriceRuleSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.TaxRule>?                  CustomTaxRuleSerializer                                      { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRuleList>?         CustomOverstayRuleListSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRule>?             CustomOverstayRuleSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AdditionalService>?        CustomAdditionalServiceSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelSchedule>?       CustomPriceLevelScheduleSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntrySerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<TransactionLimits>?                                   CustomTransactionLimitsSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<ChargingProfile>?                                     CustomChargingProfileSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<MessageInfo>?                                         CustomMessageInfoSerializer                                  { get; set; }

        public CustomJObjectSerializerDelegate<Firmware>?                                            CustomFirmwareSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<ComponentVariable>?                                   CustomComponentVariableSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<LogParameters>?                                       CustomLogParametersSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<SetVariableData>?                                     CustomSetVariableDataSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<SetVariableResult>?                                   CustomSetVariableResultSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<GetVariableData>?                                     CustomGetVariableDataSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<GetVariableResult>?                                   CustomGetVariableResultSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringData>?                                   CustomSetMonitoringDataSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringResult>?                                 CustomSetMonitoringResultSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<ClearMonitoringResult>?                               CustomClearMonitoringResultSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<NetworkConnectionProfile>?                            CustomNetworkConnectionProfileSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<VPNConfiguration>?                                    CustomVPNConfigurationSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<APNConfiguration>?                                    CustomAPNConfigurationSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizationData>?                                   CustomAuthorizationDataSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<ChargingProfileCriterion>?                            CustomChargingProfileCriterionSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<ClearChargingProfile>?                                CustomClearChargingProfileSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<CompositeSchedule>?                                   CustomCompositeScheduleSerializer                            { get; set; }

        #endregion

        #endregion

        #region Constructor(s)

        #region HTTPAPI(...)

        /// <summary>
        /// Attach the given OCPP charging station management system HTTP API to the given HTTP API.
        /// </summary>
        /// <param name="TestCSMS">An OCPP charging station management system.</param>
        /// <param name="HTTPAPI">A HTTP API.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public HTTPAPI(HTTPExtAPI                                  HTTPAPI,
                       String?                                     HTTPServerName   = null,
                       HTTPPath?                                   URLPathPrefix    = null,
                       HTTPPath?                                   BasePath         = null,
                       String                                      HTTPRealm        = DefaultHTTPRealm,
                       IEnumerable<KeyValuePair<String, String>>?  HTTPLogins       = null,
                       String?                                     HTMLTemplate     = null)

            : base(HTTPAPI,
                   HTTPServerName ?? DefaultHTTPServerName,
                   URLPathPrefix,
                   BasePath,
                   HTMLTemplate)

        {

            this.HTTPRealm           = HTTPRealm;
            this.HTTPLogins          = HTTPLogins ?? [];

            // Link HTTP events...
            //HTTPServer.RequestLog   += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            //HTTPServer.ResponseLog  += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            //HTTPServer.ErrorLog     += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            var LogfilePrefix        = "HTTPSSEs" + Path.DirectorySeparatorChar;

            this.EventLog            = this.HTTPBaseAPI.AddJSONEventSource(
                                           EventIdentification:      EventLogId,
                                           URLTemplate:              this.URLPathPrefix + "/events",
                                           MaxNumberOfCachedEvents:  10000,
                                           RetryIntervall:           TimeSpan.FromSeconds(5),
                                           EnableLogging:            true,
                                           LogfilePrefix:            LogfilePrefix
                                       );

            this.HTMLTemplate = HTMLTemplate ?? GetResourceString("template.html");

            RegisterURITemplates();

        }

        #endregion

        #region HTTPAPI(CSMS, ...)

        /// <summary>
        /// Attach the given OCPP charging station management system HTTP API to the given HTTP API.
        /// </summary>
        /// <param name="TestCSMS">An OCPP charging station management system.</param>
        /// <param name="HTTPAPI">A HTTP API.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public HTTPAPI(ACSMS                                       CSMS,
                       HTTPExtAPI                                  HTTPAPI,
                       String?                                     HTTPServerName   = null,
                       HTTPPath?                                   URLPathPrefix    = null,
                       HTTPPath?                                   BasePath         = null,
                       String                                      HTTPRealm        = DefaultHTTPRealm,
                       IEnumerable<KeyValuePair<String, String>>?  HTTPLogins       = null,
                       String?                                     HTMLTemplate     = null)

            : this(HTTPAPI,
                   HTTPServerName,
                   URLPathPrefix,
                   BasePath,
                   HTTPRealm,
                   HTTPLogins,
                   HTMLTemplate)

        {

            AttachCSMS(CSMS);

        }

        #endregion

        #endregion


        #region AttachCSMS(CSMS)

        public void AttachCSMS(ACSMS CSMS)
        {

            csmss.Add(CSMS);


            // Wire HTTP Server Sent Events

            #region WebSocket connections

            #region OnServerStarted

            CSMS.OnServerStarted += (timestamp,
                                     server,
                                     eventTrackingId,
                                     cancellationToken) =>

                EventLog.SubmitEvent("OnServerStarted",
                                     new JObject(
                                         new JProperty("timestamp",          timestamp.      ToIso8601()),
                                         new JProperty("server",             server.IPSocket.ToString()),
                                         new JProperty("eventTrackingId",    eventTrackingId.ToString())
                                     ));

            #endregion

            #region OnNewTCPConnection

            CSMS.OnNewTCPConnection += (logTimestamp,
                                        sender,
                                        connection,
                                        eventTrackingId,
                                        cancellationToken) =>

                EventLog.SubmitEvent(
                    "OnNewTCPConnection",
                    JSONObject.Create(
                        new JProperty("timestamp",        logTimestamp.   ToIso8601()),
                        new JProperty("connection",       connection.     ToJSON()),
                        new JProperty("eventTrackingId",  eventTrackingId.ToString())
                    )
                );

            #endregion

            #region OnTCPConnectionClosed

            CSMS.OnTCPConnectionClosed += (timestamp,
                                           csmsChannel,
                                           connection,
                                           networkingNodeId,
                                           eventTrackingId,
                                           reason,
                                           cancellationToken) =>

                EventLog.SubmitEvent(
                    "OnTCPConnectionClosed",
                    JSONObject.Create(
                        new JProperty("timestamp",        timestamp.      ToIso8601()),
                        new JProperty("connection",       connection.     ToJSON()),
                        new JProperty("eventTrackingId",  eventTrackingId.ToString()),
                        new JProperty("reason",           reason)
                    )
                );

            #endregion

            #region OnHTTPRequest

            CSMS.OnHTTPRequest += (timestamp,
                                   server,
                                   httpRequest,
                                   cancellationToken) =>

                EventLog.SubmitEvent("OnHTTPRequest",
                                     new JObject(
                                         new JProperty("timestamp",          timestamp.      ToIso8601()),
                                         new JProperty("server",             server.IPSocket.ToString()),
                                         new JProperty("httpRequest",        httpRequest.    ToJSON())
                                     ));

            #endregion

            #region OnNewWebSocketConnection

            CSMS.OnNewWebSocketConnection += (timestamp,
                                              csmsChannel,
                                              connection,
                                              networkingNodeId,
                                              sharedSubprotocols,
                                              eventTrackingId,
                                              cancellationToken) =>

                EventLog.SubmitEvent(
                        "OnNewWebSocketConnection",
                        JSONObject.Create(
                            new JProperty("timestamp",           timestamp.      ToIso8601()),
                            new JProperty("connection",          connection.     ToJSON()),
                            new JProperty("eventTrackingId",     eventTrackingId.ToString()),
                            new JProperty("sharedSubprotocols",  new JArray(sharedSubprotocols))
                        )
                    );

            #endregion

            #region OnHTTPResponse

            CSMS.OnHTTPResponse += (timestamp,
                                    server,
                                    httpRequest,
                                    httpResponse,
                                    cancellationToken) =>

                EventLog.SubmitEvent("OnHTTPResponse",
                                     new JObject(
                                         new JProperty("timestamp",          timestamp.      ToIso8601()),
                                         new JProperty("server",             server.IPSocket.ToString()),
                                         new JProperty("httpRequest",        httpRequest.    ToJSON()),
                                         new JProperty("httpResponse",       httpResponse.   ToJSON())
                                     ));

            #endregion


            // OnWebSocketFrameSent
            // OnWebSocketFrameReceived

            // OnTextMessageSent
            // OnTextMessageReceived

            // OnBinaryMessageSent
            // OnBinaryMessageReceived


            #region OnPingMessageSent

            CSMS.OnPingMessageSent += (timestamp,
                                       server,
                                       connection,
                                       eventTrackingId,
                                       frame,
                                       cancellationToken) =>

                EventLog.SubmitEvent("OnPingMessageSent",
                                     new JObject(
                                         new JProperty("timestamp",          timestamp.      ToIso8601()),
                                         new JProperty("server",             server.IPSocket.ToString()),
                                         new JProperty("connection",         connection.     ToJSON()),
                                         new JProperty("frame",              frame.          ToJSON())
                                     ));

            #endregion

            #region OnPingMessageReceived

            CSMS.OnPingMessageReceived += (timestamp,
                                           server,
                                           connection,
                                           eventTrackingId,
                                           frame,
                                           cancellationToken) =>

                EventLog.SubmitEvent("OnPingMessageReceived",
                                     new JObject(
                                         new JProperty("timestamp",          timestamp.      ToIso8601()),
                                         new JProperty("server",             server.IPSocket.ToString()),
                                         new JProperty("connection",         connection.     ToJSON()),
                                         new JProperty("frame",              frame.          ToJSON())
                                     ));

            #endregion


            // OnPongMessageSent

            #region OnPongMessageReceived

            CSMS.OnPongMessageReceived += (timestamp,
                                           server,
                                           connection,
                                           eventTrackingId,
                                           frame,
                                           cancellationToken) =>

                EventLog.SubmitEvent("OnPongMessageReceived",
                                     new JObject(
                                         new JProperty("timestamp",          timestamp.      ToIso8601()),
                                         new JProperty("server",             server.IPSocket.ToString()),
                                         new JProperty("connection",         connection.     ToJSON()),
                                         new JProperty("frame",              frame.          ToJSON())
                                     ));

            #endregion


            #region OnCloseMessageReceived

            CSMS.OnCloseMessageReceived += (timestamp,
                                            sender,
                                            connection,
                                            networkingNodeId,
                                            eventTrackingId,
                                            statusCode,
                                            reason,
                                            cancellationToken) =>

                EventLog.SubmitEvent(
                    "OnCloseMessageReceived",
                    JSONObject.Create(
                        new JProperty("timestamp",        timestamp.   ToIso8601()),
                        new JProperty("connection",       connection.     ToJSON()),
                        new JProperty("eventTrackingId",  eventTrackingId.ToString()),
                        new JProperty("statusCode",       statusCode.     ToString()),
                        new JProperty("reason",           reason)
                    )
                );

            #endregion

            // OnServerStopped


            // Failed (Charging Station) Authentication

            // (Generic) Error Handling

            #endregion


            #region Generic JSON Messages

            #region OnJSONMessageRequestReceived

            CSMS.OnJSONMessageRequestReceived += (timestamp,
                                                  webSocketServer,
                                                  webSocketConnection,
                                                  networkingNodeId,
                                                  networkPath,
                                                  eventTrackingId,
                                                  requestTimestamp,
                                                  requestMessage,
                                                  cancellationToken) =>

                EventLog.SubmitEvent(nameof(CSMS.OnJSONMessageRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      requestMessage)
                                     ));

            #endregion

            #region OnJSONMessageResponseSent

            CSMS.OnJSONMessageResponseSent += (timestamp,
                                               webSocketServer,
                                               webSocketConnection,
                                               networkingNodeId,
                                               networkPath,
                                               eventTrackingId,
                                               requestTimestamp,
                                               jsonRequestMessage,
                                               binaryRequestMessage,
                                               responseTimestamp,
                                               responseMessage,
                                               cancellationToken) =>

                EventLog.SubmitEvent(nameof(CSMS.OnJSONMessageResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      responseMessage)
                                     ));

            #endregion

            #region OnJSONErrorResponseSent

            CSMS.OnJSONErrorResponseSent += (timestamp,
                                             webSocketServer,
                                             webSocketConnection,
                                             eventTrackingId,
                                             requestTimestamp,
                                             jsonRequestMessage,
                                             binaryRequestMessage,
                                             responseTimestamp,
                                             responseMessage,
                                             cancellationToken) =>

                EventLog.SubmitEvent(nameof(CSMS.OnJSONErrorResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      responseMessage)
                                     ));

            #endregion


            #region OnJSONMessageRequestSent

            CSMS.OnJSONMessageRequestSent += (timestamp,
                                              webSocketServer,
                                              webSocketConnection,
                                              networkingNodeId,
                                              networkPath,
                                              eventTrackingId,
                                              requestTimestamp,
                                              requestMessage,
                                              cancellationToken) =>

                EventLog.SubmitEvent(nameof(CSMS.OnJSONMessageRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      requestMessage)
                                     ));

            #endregion

            #region OnJSONMessageResponseReceived

            CSMS.OnJSONMessageResponseReceived += (timestamp,
                                                   webSocketServer,
                                                   webSocketConnection,
                                                   networkingNodeId,
                                                   networkPath,
                                                   eventTrackingId,
                                                   requestTimestamp,
                                                   jsonRequestMessage,
                                                   binaryRequestMessage,
                                                   responseTimestamp,
                                                   responseMessage,
                                                   cancellationToken) =>

                EventLog.SubmitEvent(nameof(CSMS.OnJSONMessageResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      responseMessage)
                                     ));

            #endregion

            #region OnJSONErrorResponseReceived

            CSMS.OnJSONErrorResponseReceived += (timestamp,
                                                 webSocketServer,
                                                 webSocketConnection,
                                                 eventTrackingId,
                                                 requestTimestamp,
                                                 jsonRequestMessage,
                                                 binaryRequestMessage,
                                                 responseTimestamp,
                                                 responseMessage,
                                                 cancellationToken) =>

                EventLog.SubmitEvent(nameof(CSMS.OnJSONErrorResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      responseMessage)
                                     ));

            #endregion

            #endregion

            #region Generic Binary Messages

            #region OnBinaryMessageRequestReceived

            CSMS.OnBinaryMessageRequestReceived += (timestamp,
                                                    webSocketServer,
                                                    webSocketConnection,
                                                    networkingNodeId,
                                                    networkPath,
                                                    eventTrackingId,
                                                    requestTimestamp,
                                                    requestMessage,
                                                    cancellationToken) =>

                EventLog.SubmitEvent(nameof(CSMS.OnBinaryMessageRequestReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      requestMessage)  // BASE64 encoded string!
                                     ));

            #endregion

            #region OnBinaryMessageResponseSent

            CSMS.OnBinaryMessageResponseSent += (timestamp,
                                                 webSocketServer,
                                                 webSocketConnection,
                                                 networkingNodeId,
                                                 networkPath,
                                                 eventTrackingId,
                                                 requestTimestamp,
                                                 jsonRequestMessage,
                                                 binaryRequestMessage,
                                                 responseTimestamp,
                                                 responseMessage,
                                                 cancellationToken) =>

                EventLog.SubmitEvent(nameof(CSMS.OnBinaryMessageResponseSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      responseMessage)  // BASE64 encoded string!
                                     ));

            #endregion

            #region OnBinaryErrorResponseSent

            //CSMS.OnBinaryErrorResponseSent += async (timestamp,
            //                                         webSocketServer,
            //                                         webSocketConnection,
            //                                         eventTrackingId,
            //                                         requestTimestamp,
            //                                         jsonRequestMessage,
            //                                         binaryRequestMessage,
            //                                         responseTimestamp,
            //                                         responseMessage) =>

            //    await this.EventLog.SubmitEvent(nameof(CSMS.OnBinaryErrorResponseSent),
            //                                    JSONObject.Create(
            //                                        new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                                        new JProperty("connection",   webSocketConnection.ToJSON()),
            //                                        new JProperty("message",      responseMessage)  // BASE64 encoded string!
            //                                    ));

            #endregion


            #region OnBinaryMessageRequestSent

            CSMS.OnBinaryMessageRequestSent += (timestamp,
                                                webSocketServer,
                                                webSocketConnection,
                                                networkingNodeId,
                                                networkPath,
                                                eventTrackingId,
                                                requestTimestamp,
                                                requestMessage,
                                                cancellationToken) =>

                EventLog.SubmitEvent(nameof(CSMS.OnBinaryMessageRequestSent),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      requestMessage)  // BASE64 encoded string!
                                     ));

            #endregion

            #region OnBinaryMessageResponseReceived

            CSMS.OnBinaryMessageResponseReceived += (timestamp,
                                                     webSocketServer,
                                                     webSocketConnection,
                                                     networkingNodeId,
                                                     networkPath,
                                                     eventTrackingId,
                                                     requestTimestamp,
                                                     jsonRequestMessage,
                                                     binaryRequestMessage,
                                                     responseTimestamp,
                                                     responseMessage,
                                                     cancellationToken) =>

                EventLog.SubmitEvent(nameof(CSMS.OnBinaryMessageResponseReceived),
                                     JSONObject.Create(
                                         new JProperty("timestamp",    timestamp.          ToIso8601()),
                                         new JProperty("connection",   webSocketConnection.ToJSON()),
                                         new JProperty("message",      responseMessage)  // BASE64 encoded string!
                                     ));

            #endregion

            #region OnBinaryErrorResponseReceived

            //CSMS.OnBinaryErrorResponseReceived += async (timestamp,
            //                                             webSocketServer,
            //                                             webSocketConnection,
            //                                             eventTrackingId,
            //                                             requestTimestamp,
            //                                             jsonRequestMessage,
            //                                             binaryRequestMessage,
            //                                             responseTimestamp,
            //                                             responseMessage) =>

            //    await this.EventLog.SubmitEvent(nameof(CSMS.OnBinaryErrorResponseReceived),
            //                                    JSONObject.Create(
            //                                        new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                                        new JProperty("connection",   webSocketConnection.ToJSON()),
            //                                        new JProperty("message",      responseMessage)  // BASE64 encoded string!
            //                                    ));

            #endregion

            #endregion


            #region ChargingStation  -> CSMS            Messages received

            #region Certificates

            #region OnGet15118EVCertificate                 (RequestReceived/-ResponseSent)

            CSMS.OnGet15118EVCertificateRequestReceived += (timestamp,
                                                            sender,
                                                            connection,
                                                            request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGet15118EVCertificateRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomGet15118EVCertificateRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnGet15118EVCertificateResponseSent += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         response,
                                                         runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGet15118EVCertificateResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomGet15118EVCertificateRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomGet15118EVCertificateResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnGetCertificateStatus                  (RequestReceived/-ResponseSent)

            CSMS.OnGetCertificateStatusRequestReceived += (timestamp,
                                                           sender,
                                                           connection,
                                                           request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetCertificateStatusRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomGetCertificateStatusRequestSerializer,
                                                                           CustomOCSPRequestDataSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnGetCertificateStatusResponseSent += (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        response,
                                                        runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetCertificateStatusResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomGetCertificateStatusRequestSerializer,
                                                                             CustomOCSPRequestDataSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomGetCertificateStatusResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnGetCRL                                (RequestReceived/-ResponseSent)

            CSMS.OnGetCRLRequestReceived += (timestamp,
                                             sender,
                                             connection,
                                             request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetCRLRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomGetCRLRequestSerializer,
                                                                           CustomCertificateHashDataSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnGetCRLResponseSent += (timestamp,
                                          sender,
                                          connection,
                                          request,
                                          response,
                                          runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetCRLResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomGetCRLRequestSerializer,
                                                                             CustomCertificateHashDataSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomGetCRLResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnSignCertificate                       (RequestReceived/-ResponseSent)

            CSMS.OnSignCertificateRequestReceived += (timestamp,
                                                      sender,
                                                      connection,
                                                      request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSignCertificateRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomSignCertificateRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnSignCertificateResponseSent += (timestamp,
                                                   sender,
                                                   connection,
                                                   request,
                                                   response,
                                                   runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSignCertificateResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomSignCertificateRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomSignCertificateResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #endregion

            #region Charging

            #region OnAuthorize                             (RequestReceived/-ResponseSent)

            CSMS.OnAuthorizeRequestReceived += (timestamp,
                                                sender,
                                                connection,
                                                request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnAuthorizeRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomAuthorizeRequestSerializer,
                                                                           CustomIdTokenSerializer,
                                                                           CustomAdditionalInfoSerializer,
                                                                           CustomOCSPRequestDataSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnAuthorizeResponseSent += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             response,
                                             runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnAuthorizeResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomAuthorizeRequestSerializer,
                                                                             CustomIdTokenSerializer,
                                                                             CustomAdditionalInfoSerializer,
                                                                             CustomOCSPRequestDataSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomAuthorizeResponseSerializer,
                                                                             CustomIdTokenInfoSerializer,
                                                                             CustomIdTokenSerializer,
                                                                             CustomAdditionalInfoSerializer,
                                                                             CustomMessageContentSerializer,
                                                                             CustomTransactionLimitsSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnClearedChargingLimit                  (RequestReceived/-ResponseSent)

            CSMS.OnClearedChargingLimitRequestReceived += (timestamp,
                                                           sender,
                                                           connection,
                                                           request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnClearedChargingLimitRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomClearedChargingLimitRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnClearedChargingLimitResponseSent += (timestamp,
                                                        sender,
                                                        connection,
                                                        request,
                                                        response,
                                                        runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnClearedChargingLimitResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomClearedChargingLimitRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomClearedChargingLimitResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnMeterValues                           (RequestReceived/-ResponseSent)

            CSMS.OnMeterValuesRequestReceived += (timestamp,
                                                  sender,
                                                  connection,
                                                  request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnMeterValuesRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomMeterValuesRequestSerializer,
                                                                           CustomMeterValueSerializer,
                                                                           CustomSampledValueSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnMeterValuesResponseSent += (timestamp,
                                               sender,
                                               connection,
                                               request,
                                               response,
                                               runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnMeterValuesResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomMeterValuesRequestSerializer,
                                                                             CustomMeterValueSerializer,
                                                                             CustomSampledValueSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomMeterValuesResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyChargingLimit                   (RequestReceived/-ResponseSent)

            CSMS.OnNotifyChargingLimitRequestReceived += (timestamp,
                                                          sender,
                                                          connection,
                                                          request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyChargingLimitRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomNotifyChargingLimitRequestSerializer,

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

                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnNotifyChargingLimitResponseSent += (timestamp,
                                                       sender,
                                                       connection,
                                                       request,
                                                       response,
                                                       runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyChargingLimitResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomNotifyChargingLimitRequestSerializer,

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

                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomNotifyChargingLimitResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyEVChargingNeeds                 (RequestReceived/-ResponseSent)

            CSMS.OnNotifyEVChargingNeedsRequestReceived += (timestamp,
                                                            sender,
                                                            connection,
                                                            request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyEVChargingNeedsRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomNotifyEVChargingNeedsRequestSerializer,
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
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnNotifyEVChargingNeedsResponseSent += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         response,
                                                         runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyEVChargingNeedsResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomNotifyEVChargingNeedsRequestSerializer,
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
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomNotifyEVChargingNeedsResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyEVChargingSchedule              (RequestReceived/-ResponseSent)

            CSMS.OnNotifyEVChargingScheduleRequestReceived += (timestamp,
                                                               sender,
                                                               connection,
                                                               request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyEVChargingScheduleRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomNotifyEVChargingScheduleRequestSerializer,

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

                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnNotifyEVChargingScheduleResponseSent += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            response,
                                                            runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyEVChargingScheduleResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomNotifyEVChargingScheduleRequestSerializer,

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

                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomNotifyEVChargingScheduleResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyPriorityCharging                (RequestReceived/-ResponseSent)

            CSMS.OnNotifyPriorityChargingRequestReceived += (timestamp,
                                                             sender,
                                                             connection,
                                                             request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyPriorityChargingRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomNotifyPriorityChargingRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnNotifyPriorityChargingResponseSent += (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          response,
                                                          runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyPriorityChargingResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomNotifyPriorityChargingRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomNotifyPriorityChargingResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnNotifySettlement                      (RequestReceived/-ResponseSent)

            CSMS.OnNotifySettlementRequestReceived += (timestamp,
                                                       sender,
                                                       connection,
                                                       request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifySettlementRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomNotifySettlementRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnNotifySettlementResponseSent += (timestamp,
                                                    sender,
                                                    connection,
                                                    request,
                                                    response,
                                                    runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifySettlementResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomNotifySettlementRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomNotifySettlementResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnPullDynamicScheduleUpdate             (RequestReceived/-ResponseSent)

            CSMS.OnPullDynamicScheduleUpdateRequestReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnPullDynamicScheduleUpdateRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomPullDynamicScheduleUpdateRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnPullDynamicScheduleUpdateResponseSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             response,
                                                             runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnPullDynamicScheduleUpdateResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomPullDynamicScheduleUpdateRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomPullDynamicScheduleUpdateResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnReportChargingProfiles                (RequestReceived/-ResponseSent)

            CSMS.OnReportChargingProfilesRequestReceived += (timestamp,
                                                             sender,
                                                             connection,
                                                             request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnReportChargingProfilesRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomReportChargingProfilesRequestSerializer,

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

                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnReportChargingProfilesResponseSent += (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          response,
                                                          runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnReportChargingProfilesResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomReportChargingProfilesRequestSerializer,

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

                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomReportChargingProfilesResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnReservationStatusUpdate               (RequestReceived/-ResponseSent)

            CSMS.OnReservationStatusUpdateRequestReceived += (timestamp,
                                                              sender,
                                                              connection,
                                                              request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnReservationStatusUpdateRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomReservationStatusUpdateRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnReservationStatusUpdateResponseSent += (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           response,
                                                           runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnReservationStatusUpdateResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomReservationStatusUpdateRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomReservationStatusUpdateResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnStatusNotification                    (RequestReceived/-ResponseSent)

            CSMS.OnStatusNotificationRequestReceived += (timestamp,
                                                         sender,
                                                         connection,
                                                         request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnStatusNotificationRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomStatusNotificationRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnStatusNotificationResponseSent += (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      response,
                                                      runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnStatusNotificationResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomStatusNotificationRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomStatusNotificationResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnTransactionEvent                      (RequestReceived/-ResponseSent)

            CSMS.OnTransactionEventRequestReceived += (timestamp,
                                                       sender,
                                                       connection,
                                                       request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnTransactionEventRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomTransactionEventRequestSerializer,
                                                                           CustomTransactionSerializer,
                                                                           CustomIdTokenSerializer,
                                                                           CustomAdditionalInfoSerializer,
                                                                           CustomEVSESerializer,
                                                                           CustomMeterValueSerializer,
                                                                           CustomSampledValueSerializer,
                                                                           CustomSignedMeterValueSerializer,
                                                                           CustomUnitsOfMeasureSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnTransactionEventResponseSent += (timestamp,
                                                    sender,
                                                    connection,
                                                    request,
                                                    response,
                                                    runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnTransactionEventResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomTransactionEventRequestSerializer,
                                                                             CustomTransactionSerializer,
                                                                             CustomIdTokenSerializer,
                                                                             CustomAdditionalInfoSerializer,
                                                                             CustomEVSESerializer,
                                                                             CustomMeterValueSerializer,
                                                                             CustomSampledValueSerializer,
                                                                             CustomSignedMeterValueSerializer,
                                                                             CustomUnitsOfMeasureSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomTransactionEventResponseSerializer,
                                                                             CustomIdTokenInfoSerializer,
                                                                             CustomIdTokenSerializer,
                                                                             CustomAdditionalInfoSerializer,
                                                                             CustomMessageContentSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #endregion

            #region Customer

            #region OnNotifyCustomerInformation             (RequestReceived/-ResponseSent)

            CSMS.OnNotifyCustomerInformationRequestReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyCustomerInformationRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomNotifyCustomerInformationRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnNotifyCustomerInformationResponseSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             response,
                                                             runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyCustomerInformationResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomNotifyCustomerInformationRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomNotifyCustomerInformationResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyDisplayMessages                 (RequestReceived/-ResponseSent)

            CSMS.OnNotifyDisplayMessagesRequestReceived += (timestamp,
                                                            sender,
                                                            connection,
                                                            request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyDisplayMessagesRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomNotifyDisplayMessagesRequestSerializer,
                                                                           CustomMessageInfoSerializer,
                                                                           CustomMessageContentSerializer,
                                                                           CustomComponentSerializer,
                                                                           CustomEVSESerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnNotifyDisplayMessagesResponseSent += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         response,
                                                         runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyDisplayMessagesResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomNotifyDisplayMessagesRequestSerializer,
                                                                             CustomMessageInfoSerializer,
                                                                             CustomMessageContentSerializer,
                                                                             CustomComponentSerializer,
                                                                             CustomEVSESerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomNotifyDisplayMessagesResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #endregion

            #region DeviceModel

            #region OnLogStatusNotification                 (RequestReceived/-ResponseSent)

            CSMS.OnLogStatusNotificationRequestReceived += (timestamp,
                                                            sender,
                                                            connection,
                                                            request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnLogStatusNotificationRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomLogStatusNotificationRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnLogStatusNotificationResponseSent += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         response,
                                                         runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnLogStatusNotificationResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomLogStatusNotificationRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomLogStatusNotificationResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyEvent                           (RequestReceived/-ResponseSent)

            CSMS.OnNotifyEventRequestReceived += (timestamp,
                                                  sender,
                                                  connection,
                                                  request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyEventRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomNotifyEventRequestSerializer,
                                                                           CustomEventDataSerializer,
                                                                           CustomComponentSerializer,
                                                                           CustomEVSESerializer,
                                                                           CustomVariableSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnNotifyEventResponseSent += (timestamp,
                                               sender,
                                               connection,
                                               request,
                                               response,
                                               runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyEventResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomNotifyEventRequestSerializer,
                                                                             CustomEventDataSerializer,
                                                                             CustomComponentSerializer,
                                                                             CustomEVSESerializer,
                                                                             CustomVariableSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomNotifyEventResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyMonitoringReport                (RequestReceived/-ResponseSent)

            CSMS.OnNotifyMonitoringReportRequestReceived += (timestamp,
                                                             sender,
                                                             connection,
                                                             request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyMonitoringReportRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomNotifyMonitoringReportRequestSerializer,
                                                                           CustomMonitoringDataSerializer,
                                                                           CustomComponentSerializer,
                                                                           CustomEVSESerializer,
                                                                           CustomVariableSerializer,
                                                                           CustomVariableMonitoringSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnNotifyMonitoringReportResponseSent += (timestamp,
                                                          sender,
                                                          connection,
                                                          request,
                                                          response,
                                                          runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyMonitoringReportResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomNotifyMonitoringReportRequestSerializer,
                                                                             CustomMonitoringDataSerializer,
                                                                             CustomComponentSerializer,
                                                                             CustomEVSESerializer,
                                                                             CustomVariableSerializer,
                                                                             CustomVariableMonitoringSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomNotifyMonitoringReportResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyReport                          (RequestReceived/-ResponseSent)

            CSMS.OnNotifyReportRequestReceived += (timestamp,
                                                   sender,
                                                   connection,
                                                   request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyReportRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomNotifyReportRequestSerializer,
                                                                           CustomReportDataSerializer,
                                                                           CustomComponentSerializer,
                                                                           CustomEVSESerializer,
                                                                           CustomVariableSerializer,
                                                                           CustomVariableAttributeSerializer,
                                                                           CustomVariableCharacteristicsSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnNotifyReportResponseSent += (timestamp,
                                                sender,
                                                connection,
                                                request,
                                                response,
                                                runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyReportResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomNotifyReportRequestSerializer,
                                                                             CustomReportDataSerializer,
                                                                             CustomComponentSerializer,
                                                                             CustomEVSESerializer,
                                                                             CustomVariableSerializer,
                                                                             CustomVariableAttributeSerializer,
                                                                             CustomVariableCharacteristicsSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomNotifyReportResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnSecurityEventNotification             (RequestReceived/-ResponseSent)

            CSMS.OnSecurityEventNotificationRequestReceived += (timestamp,
                                                                sender,
                                                                connection,
                                                                request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSecurityEventNotificationRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomSecurityEventNotificationRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnSecurityEventNotificationResponseSent += (timestamp,
                                                             sender,
                                                             connection,
                                                             request,
                                                             response,
                                                             runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSecurityEventNotificationResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomSecurityEventNotificationRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomSecurityEventNotificationResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #endregion

            #region Firmware

            #region OnBootNotification                      (RequestReceived/-ResponseSent)

            CSMS.OnBootNotificationRequestReceived += (timestamp,
                                                       sender,
                                                       connection,
                                                       request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnBootNotificationRequestReceived),
                                     request.ToAbstractJSON(connection,
                                                            request.ToJSON(CustomBootNotificationRequestSerializer,
                                                                           CustomChargingStationSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnBootNotificationResponseSent += (timestamp,
                                                    sender,
                                                    connection,
                                                    request,
                                                    response,
                                                    runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnBootNotificationResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomBootNotificationRequestSerializer,
                                                                             CustomChargingStationSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomBootNotificationResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnFirmwareStatusNotification            (RequestReceived/-ResponseSent)

            CSMS.OnFirmwareStatusNotificationRequestReceived += (timestamp,
                                                                 sender,
                                                                 connection,
                                                                 request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnFirmwareStatusNotificationRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomFirmwareStatusNotificationRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnFirmwareStatusNotificationResponseSent += (timestamp,
                                                              sender,
                                                              connection,
                                                              request,
                                                              response,
                                                              runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnFirmwareStatusNotificationResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomFirmwareStatusNotificationRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomFirmwareStatusNotificationResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnHeartbeat                             (RequestReceived/-ResponseSent)

            CSMS.OnHeartbeatRequestReceived += (timestamp,
                                                sender,
                                                connection,
                                                request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnHeartbeatRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomHeartbeatRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnHeartbeatResponseSent += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             response,
                                             runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnHeartbeatResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomHeartbeatRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomHeartbeatResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnPublishFirmwareStatusNotification     (RequestReceived/-ResponseSent)

            CSMS.OnPublishFirmwareStatusNotificationRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnPublishFirmwareStatusNotificationRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomPublishFirmwareStatusNotificationRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnPublishFirmwareStatusNotificationResponseSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnPublishFirmwareStatusNotificationResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomPublishFirmwareStatusNotificationRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomPublishFirmwareStatusNotificationResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #endregion

            #endregion

            #region CSMS             -> ChargingStation Messages sent

            #region Certificates

            #region OnCertificateSigned                     (RequestSent/-ResponseReceived)

            CSMS.OnCertificateSignedRequestSent += (timestamp,
                                                    sender,
                                                    request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnCertificateSignedRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomCertificateSignedRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnCertificateSignedResponseReceived += (timestamp,
                                                         sender,
                                                         request,
                                                         response,
                                                         runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnCertificateSignedResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomCertificateSignedRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomCertificateSignedResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnDeleteCertificate                     (RequestSent/-ResponseReceived)

            CSMS.OnDeleteCertificateRequestSent += (timestamp,
                                                    sender,
                                                    request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnDeleteCertificateRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomDeleteCertificateRequestSerializer,
                                                                           CustomCertificateHashDataSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnDeleteCertificateResponseReceived += (timestamp,
                                                         sender,
                                                         request,
                                                         response,
                                                         runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnDeleteCertificateResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomDeleteCertificateRequestSerializer,
                                                                             CustomCertificateHashDataSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomDeleteCertificateResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnGetInstalledCertificateIds            (RequestSent/-ResponseReceived)

            CSMS.OnGetInstalledCertificateIdsRequestSent += (timestamp,
                                                             sender,
                                                             request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetInstalledCertificateIdsRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomGetInstalledCertificateIdsRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnGetInstalledCertificateIdsResponseReceived += (timestamp,
                                                                  sender,
                                                                  request,
                                                                  response,
                                                                  runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetInstalledCertificateIdsResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomGetInstalledCertificateIdsRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomGetInstalledCertificateIdsResponseSerializer,
                                                                             CustomCertificateHashDataSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnInstallCertificate                    (RequestSent/-ResponseReceived)

            CSMS.OnInstallCertificateRequestSent += (timestamp,
                                                     sender,
                                                     request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnInstallCertificateRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomInstallCertificateRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnInstallCertificateResponseReceived += (timestamp,
                                                          sender,
                                                          request,
                                                          response,
                                                          runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnInstallCertificateResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomInstallCertificateRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomInstallCertificateResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyCRL                             (RequestSent/-ResponseReceived)

            CSMS.OnNotifyCRLRequestSent += (timestamp,
                                            sender,
                                            request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyCRLRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomNotifyCRLRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnNotifyCRLResponseReceived += (timestamp,
                                                 sender,
                                                 request,
                                                 response,
                                                 runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyCRLResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomNotifyCRLRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomNotifyCRLResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #endregion

            #region Charging

            #region OnCancelReservation                     (RequestSent/-ResponseReceived)

            CSMS.OnCancelReservationRequestSent += (timestamp,
                                                    sender,
                                                    request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnCancelReservationRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomCancelReservationRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnCancelReservationResponseReceived += (timestamp,
                                                         sender,
                                                         request,
                                                         response,
                                                         runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnCancelReservationResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomCancelReservationRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomCancelReservationResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnClearChargingProfile                  (RequestSent/-ResponseReceived)

            CSMS.OnClearChargingProfileRequestSent += (timestamp,
                                                       sender,
                                                       request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnClearChargingProfileRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomClearChargingProfileRequestSerializer,
                                                                           CustomClearChargingProfileSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnClearChargingProfileResponseReceived += (timestamp,
                                                            sender,
                                                            request,
                                                            response,
                                                            runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnClearChargingProfileResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomClearChargingProfileRequestSerializer,
                                                                             CustomClearChargingProfileSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomClearChargingProfileResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnGetChargingProfiles                   (RequestSent/-ResponseReceived)

            CSMS.OnGetChargingProfilesRequestSent += (timestamp,
                                                      sender,
                                                      request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetChargingProfilesRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomGetChargingProfilesRequestSerializer,
                                                                           CustomChargingProfileCriterionSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnGetChargingProfilesResponseReceived += (timestamp,
                                                           sender,
                                                           request,
                                                           response,
                                                           runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetChargingProfilesResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomGetChargingProfilesRequestSerializer,
                                                                             CustomChargingProfileCriterionSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomGetChargingProfilesResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnGetCompositeSchedule                  (RequestSent/-ResponseReceived)

            CSMS.OnGetCompositeScheduleRequestSent += (timestamp,
                                                       sender,
                                                       request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetCompositeScheduleRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomGetCompositeScheduleRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnGetCompositeScheduleResponseReceived += (timestamp,
                                                            sender,
                                                            request,
                                                            response,
                                                            runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetCompositeScheduleResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomGetCompositeScheduleRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomGetCompositeScheduleResponseSerializer,
                                                                             CustomCompositeScheduleSerializer,
                                                                             CustomChargingSchedulePeriodSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnGetTransactionStatus                  (RequestSent/-ResponseReceived)

            CSMS.OnGetTransactionStatusRequestSent += (timestamp,
                                                       sender,
                                                       request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetTransactionStatusRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomGetTransactionStatusRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnGetTransactionStatusResponseReceived += (timestamp,
                                                            sender,
                                                            request,
                                                            response,
                                                            runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetTransactionStatusResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomGetTransactionStatusRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomGetTransactionStatusResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyAllowedEnergyTransfer           (RequestSent/-ResponseReceived)

            CSMS.OnNotifyAllowedEnergyTransferRequestSent += (timestamp,
                                                              sender,
                                                              request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyAllowedEnergyTransferRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomNotifyAllowedEnergyTransferRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnNotifyAllowedEnergyTransferResponseReceived += (timestamp,
                                                                   sender,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnNotifyAllowedEnergyTransferResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomNotifyAllowedEnergyTransferRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomNotifyAllowedEnergyTransferResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnRequestStartTransaction               (RequestSent/-ResponseReceived)

            CSMS.OnRequestStartTransactionRequestSent += (timestamp,
                                                          sender,
                                                          request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnRequestStartTransactionRequestSent),
                                                request.ToAbstractJSON(request.ToJSON(CustomRequestStartTransactionRequestSerializer,

                                                                                      CustomIdTokenSerializer,
                                                                                      CustomAdditionalInfoSerializer,
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

                                                                                      CustomTransactionLimitsSerializer,

                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));


            CSMS.OnRequestStartTransactionResponseReceived += (timestamp,
                                                               sender,
                                                               request,
                                                               response,
                                                               runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnRequestStartTransactionResponseReceived),
                                                response.ToAbstractJSON(request. ToJSON(CustomRequestStartTransactionRequestSerializer,

                                                                                        CustomIdTokenSerializer,
                                                                                        CustomAdditionalInfoSerializer,
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

                                                                                        CustomTransactionLimitsSerializer,

                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomRequestStartTransactionResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnRequestStopTransaction                (RequestSent/-ResponseReceived)

            CSMS.OnRequestStopTransactionRequestSent += (timestamp,
                                                         sender,
                                                         request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnRequestStopTransactionRequestSent),
                                                request.ToAbstractJSON(request.ToJSON(CustomRequestStopTransactionRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));


            CSMS.OnRequestStopTransactionResponseReceived += (timestamp,
                                                              sender,
                                                              request,
                                                              response,
                                                              runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnRequestStopTransactionResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomRequestStopTransactionRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomRequestStopTransactionResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnReserveNow                            (RequestSent/-ResponseReceived)

            CSMS.OnReserveNowRequestSent += (timestamp,
                                             sender,
                                             request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnReserveNowRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomReserveNowRequestSerializer,
                                                                           CustomIdTokenSerializer,
                                                                           CustomAdditionalInfoSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnReserveNowResponseReceived += (timestamp,
                                                  sender,
                                                  request,
                                                  response,
                                                  runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnReserveNowResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomReserveNowRequestSerializer,
                                                                             CustomIdTokenSerializer,
                                                                             CustomAdditionalInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomReserveNowResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnSetChargingProfile                    (RequestSent/-ResponseReceived)

            CSMS.OnSetChargingProfileRequestSent += (timestamp,
                                                     sender,
                                                     request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSetChargingProfileRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomSetChargingProfileRequestSerializer,

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

                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnSetChargingProfileResponseReceived += (timestamp,
                                                          sender,
                                                          request,
                                                          response,
                                                          runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSetChargingProfileResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomSetChargingProfileRequestSerializer,

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

                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomSetChargingProfileResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnUnlockConnector                       (RequestSent/-ResponseReceived)

            CSMS.OnUnlockConnectorRequestSent += (timestamp,
                                                  sender,
                                                  request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnUnlockConnectorRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomUnlockConnectorRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnUnlockConnectorResponseReceived += (timestamp,
                                                       sender,
                                                       request,
                                                       response,
                                                       runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnUnlockConnectorResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomUnlockConnectorRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomUnlockConnectorResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnUpdateDynamicSchedule                 (RequestSent/-ResponseReceived)

            CSMS.OnUpdateDynamicScheduleRequestSent += (timestamp,
                                                        sender,
                                                        request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnUpdateDynamicScheduleRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomUpdateDynamicScheduleRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnUpdateDynamicScheduleResponseReceived += (timestamp,
                                                             sender,
                                                             request,
                                                             response,
                                                             runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnUpdateDynamicScheduleResponseReceived),
                                     response.ToAbstractJSON(request.ToJSON(CustomUpdateDynamicScheduleRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomUpdateDynamicScheduleResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnUsePriorityCharging                   (RequestSent/-ResponseReceived)

            CSMS.OnUsePriorityChargingRequestSent += (timestamp,
                                                      sender,
                                                      request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnUsePriorityChargingRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomUsePriorityChargingRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnUsePriorityChargingResponseReceived += (timestamp,
                                                           sender,
                                                           request,
                                                           response,
                                                           runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnUsePriorityChargingResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomUsePriorityChargingRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomUsePriorityChargingResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #endregion

            #region Customer

            #region OnClearDisplayMessage                   (RequestSent/-ResponseReceived)

            CSMS.OnClearDisplayMessageRequestSent += (timestamp,
                                                      sender,
                                                      request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnClearDisplayMessageRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomClearDisplayMessageRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnClearDisplayMessageResponseReceived += (timestamp,
                                                           sender,
                                                           request,
                                                           response,
                                                           runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnClearDisplayMessageResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomClearDisplayMessageRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomClearDisplayMessageResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnCostUpdated                           (RequestSent/-ResponseReceived)

            CSMS.OnCostUpdatedRequestSent += (timestamp,
                                              sender,
                                              request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnCostUpdatedRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomCostUpdatedRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnCostUpdatedResponseReceived += (timestamp,
                                                   sender,
                                                   request,
                                                   response,
                                                   runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnCostUpdatedResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomCostUpdatedRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomCostUpdatedResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnCustomerInformation                   (RequestSent/-ResponseReceived)

            CSMS.OnCustomerInformationRequestSent += (timestamp,
                                                      sender,
                                                      request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnCustomerInformationRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomCustomerInformationRequestSerializer,
                                                                           CustomIdTokenSerializer,
                                                                           CustomAdditionalInfoSerializer,
                                                                           CustomCertificateHashDataSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnCustomerInformationResponseReceived += (timestamp,
                                                           sender,
                                                           request,
                                                           response,
                                                           runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnCustomerInformationResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomCustomerInformationRequestSerializer,
                                                                             CustomIdTokenSerializer,
                                                                             CustomAdditionalInfoSerializer,
                                                                             CustomCertificateHashDataSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomCustomerInformationResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnGetDisplayMessages                    (RequestSent/-ResponseReceived)

            CSMS.OnGetDisplayMessagesRequestSent += (timestamp,
                                                     sender,
                                                     request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetDisplayMessagesRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomGetDisplayMessagesRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnGetDisplayMessagesResponseReceived += (timestamp,
                                                          sender,
                                                          request,
                                                          response,
                                                          runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetDisplayMessagesResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomGetDisplayMessagesRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomGetDisplayMessagesResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnSetDisplayMessage                     (RequestSent/-ResponseReceived)

            CSMS.OnSetDisplayMessageRequestSent += (timestamp,
                                                    sender,
                                                    request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSetDisplayMessageRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomSetDisplayMessageRequestSerializer,
                                                                           CustomMessageInfoSerializer,
                                                                           CustomMessageContentSerializer,
                                                                           CustomComponentSerializer,
                                                                           CustomEVSESerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnSetDisplayMessageResponseReceived += (timestamp,
                                                         sender,
                                                         request,
                                                         response,
                                                         runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSetDisplayMessageResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomSetDisplayMessageRequestSerializer,
                                                                             CustomMessageInfoSerializer,
                                                                             CustomMessageContentSerializer,
                                                                             CustomComponentSerializer,
                                                                             CustomEVSESerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomSetDisplayMessageResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #endregion

            #region DeviceModel

            #region OnChangeAvailability                    (RequestSent/-ResponseReceived)

            CSMS.OnChangeAvailabilityRequestSent += (timestamp,
                                                     sender,
                                                     request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnChangeAvailabilityRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomChangeAvailabilityRequestSerializer,
                                                                           CustomEVSESerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnChangeAvailabilityResponseReceived += (timestamp,
                                                          sender,
                                                          request,
                                                          response,
                                                          runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnChangeAvailabilityResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomChangeAvailabilityRequestSerializer,
                                                                             CustomEVSESerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomChangeAvailabilityResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnClearVariableMonitoring               (RequestSent/-ResponseReceived)

            CSMS.OnClearVariableMonitoringRequestSent += (timestamp,
                                                          sender,
                                                          request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnClearVariableMonitoringRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomClearVariableMonitoringRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnClearVariableMonitoringResponseReceived += (timestamp,
                                                               sender,
                                                               request,
                                                               response,
                                                               runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnClearVariableMonitoringResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomClearVariableMonitoringRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomClearVariableMonitoringResponseSerializer,
                                                                             CustomClearMonitoringResultSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnGetBaseReport                         (RequestSent/-ResponseReceived)

            CSMS.OnGetBaseReportRequestSent += (timestamp,
                                                sender,
                                                request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetBaseReportRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomGetBaseReportRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnGetBaseReportResponseReceived += (timestamp,
                                                     sender,
                                                     request,
                                                     response,
                                                     runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetBaseReportResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomGetBaseReportRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomGetBaseReportResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnGetLog                                (RequestSent/-ResponseReceived)

            CSMS.OnGetLogRequestSent += (timestamp,
                                         sender,
                                         request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetLogRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomGetLogRequestSerializer,
                                                                           CustomLogParametersSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnGetLogResponseReceived += (timestamp,
                                              sender,
                                              request,
                                              response,
                                              runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetLogResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomGetLogRequestSerializer,
                                                                             CustomLogParametersSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomGetLogResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnGetMonitoringReport                   (RequestSent/-ResponseReceived)

            CSMS.OnGetMonitoringReportRequestSent += (timestamp,
                                                      sender,
                                                      request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetMonitoringReportRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomGetMonitoringReportRequestSerializer,
                                                                           CustomComponentVariableSerializer,
                                                                           CustomComponentSerializer,
                                                                           CustomEVSESerializer,
                                                                           CustomVariableSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnGetMonitoringReportResponseReceived += (timestamp,
                                                           sender,
                                                           request,
                                                           response,
                                                           runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetMonitoringReportResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomGetMonitoringReportRequestSerializer,
                                                                             CustomComponentVariableSerializer,
                                                                             CustomComponentSerializer,
                                                                             CustomEVSESerializer,
                                                                             CustomVariableSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomGetMonitoringReportResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnGetReport                             (RequestSent/-ResponseReceived)

            CSMS.OnGetReportRequestSent += (timestamp,
                                            sender,
                                            request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetReportRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomGetReportRequestSerializer,
                                                                           CustomComponentVariableSerializer,
                                                                           CustomComponentSerializer,
                                                                           CustomEVSESerializer,
                                                                           CustomVariableSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnGetReportResponseReceived += (timestamp,
                                                 sender,
                                                 request,
                                                 response,
                                                 runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetReportResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomGetReportRequestSerializer,
                                                                             CustomComponentVariableSerializer,
                                                                             CustomComponentSerializer,
                                                                             CustomEVSESerializer,
                                                                             CustomVariableSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomGetReportResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnGetVariables                          (RequestSent/-ResponseReceived)

            CSMS.OnGetVariablesRequestSent += (timestamp,
                                               sender,
                                               request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetVariablesRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomGetVariablesRequestSerializer,
                                                                           CustomGetVariableDataSerializer,
                                                                           CustomComponentSerializer,
                                                                           CustomEVSESerializer,
                                                                           CustomVariableSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnGetVariablesResponseReceived += (timestamp,
                                                    sender,
                                                    request,
                                                    response,
                                                    runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetVariablesResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomGetVariablesRequestSerializer,
                                                                             CustomGetVariableDataSerializer,
                                                                             CustomComponentSerializer,
                                                                             CustomEVSESerializer,
                                                                             CustomVariableSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomGetVariablesResponseSerializer,
                                                                             CustomGetVariableResultSerializer,
                                                                             CustomComponentSerializer,
                                                                             CustomEVSESerializer,
                                                                             CustomVariableSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnSetMonitoringBase                     (RequestSent/-ResponseReceived)

            CSMS.OnSetMonitoringBaseRequestSent += (timestamp,
                                                    sender,
                                                    request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSetMonitoringBaseRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomSetMonitoringBaseRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnSetMonitoringBaseResponseReceived += (timestamp,
                                                         sender,
                                                         request,
                                                         response,
                                                         runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSetMonitoringBaseResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomSetMonitoringBaseRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomSetMonitoringBaseResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnSetMonitoringLevel                    (RequestSent/-ResponseReceived)

            CSMS.OnSetMonitoringLevelRequestSent += (timestamp,
                                                     sender,
                                                     request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSetMonitoringLevelRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomSetMonitoringLevelRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnSetMonitoringLevelResponseReceived += (timestamp,
                                                          sender,
                                                          request,
                                                          response,
                                                          runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSetMonitoringLevelResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomSetMonitoringLevelRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomSetMonitoringLevelResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnSetNetworkProfile                     (RequestSent/-ResponseReceived)

            CSMS.OnSetNetworkProfileRequestSent += (timestamp,
                                                    sender,
                                                    request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSetNetworkProfileRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomSetNetworkProfileRequestSerializer,
                                                                           CustomNetworkConnectionProfileSerializer,
                                                                           CustomVPNConfigurationSerializer,
                                                                           CustomAPNConfigurationSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnSetNetworkProfileResponseReceived += (timestamp,
                                                         sender,
                                                         request,
                                                         response,
                                                         runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSetNetworkProfileResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomSetNetworkProfileRequestSerializer,
                                                                             CustomNetworkConnectionProfileSerializer,
                                                                             CustomVPNConfigurationSerializer,
                                                                             CustomAPNConfigurationSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomSetNetworkProfileResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnSetVariableMonitoring                 (RequestSent/-ResponseReceived)

            CSMS.OnSetVariableMonitoringRequestSent += (timestamp,
                                                        sender,
                                                        request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSetVariableMonitoringRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomSetVariableMonitoringRequestSerializer,
                                                                           CustomSetMonitoringDataSerializer,
                                                                           CustomComponentSerializer,
                                                                           CustomEVSESerializer,
                                                                           CustomVariableSerializer,
                                                                           CustomPeriodicEventStreamParametersSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnSetVariableMonitoringResponseReceived += (timestamp,
                                                             sender,
                                                             request,
                                                             response,
                                                             runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSetVariableMonitoringResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomSetVariableMonitoringRequestSerializer,
                                                                             CustomSetMonitoringDataSerializer,
                                                                             CustomComponentSerializer,
                                                                             CustomEVSESerializer,
                                                                             CustomVariableSerializer,
                                                                             CustomPeriodicEventStreamParametersSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomSetVariableMonitoringResponseSerializer,
                                                                             CustomSetMonitoringResultSerializer,
                                                                             CustomComponentSerializer,
                                                                             CustomEVSESerializer,
                                                                             CustomVariableSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnSetVariables                          (RequestSent/-ResponseReceived)

            CSMS.OnSetVariablesRequestSent += (timestamp,
                                               sender,
                                               request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSetVariablesRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomSetVariablesRequestSerializer,
                                                                           CustomSetVariableDataSerializer,
                                                                           CustomComponentSerializer,
                                                                           CustomEVSESerializer,
                                                                           CustomVariableSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnSetVariablesResponseReceived += (timestamp,
                                                    sender,
                                                    request,
                                                    response,
                                                    runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSetVariablesResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomSetVariablesRequestSerializer,
                                                                             CustomSetVariableDataSerializer,
                                                                             CustomComponentSerializer,
                                                                             CustomEVSESerializer,
                                                                             CustomVariableSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomSetVariablesResponseSerializer,
                                                                             CustomSetVariableResultSerializer,
                                                                             CustomComponentSerializer,
                                                                             CustomEVSESerializer,
                                                                             CustomVariableSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnTriggerMessage                        (RequestSent/-ResponseReceived)

            CSMS.OnTriggerMessageRequestSent += (timestamp,
                                                 sender,
                                                 request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnTriggerMessageRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomTriggerMessageRequestSerializer,
                                                                           CustomEVSESerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnTriggerMessageResponseReceived += (timestamp,
                                                      sender,
                                                      request,
                                                      response,
                                                      runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnTriggerMessageResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomTriggerMessageRequestSerializer,
                                                                             CustomEVSESerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomTriggerMessageResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #endregion

            #region Firmware

            #region OnPublishFirmware                       (RequestSent/-ResponseReceived)

            CSMS.OnPublishFirmwareRequestSent += (timestamp,
                                                  sender,
                                                  request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnPublishFirmwareRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomPublishFirmwareRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnPublishFirmwareResponseReceived += (timestamp,
                                                       sender,
                                                       request,
                                                       response,
                                                       runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnPublishFirmwareResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomPublishFirmwareRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomPublishFirmwareResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnReset                                 (RequestSent/-ResponseReceived)

            CSMS.OnResetRequestSent += (timestamp,
                                        sender,
                                        request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnResetRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomResetRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnResetResponseReceived += (timestamp,
                                             sender,
                                             request,
                                             response,
                                             runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnResetResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomResetRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomResetResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnUnpublishFirmware                     (RequestSent/-ResponseReceived)

            CSMS.OnUnpublishFirmwareRequestSent += (timestamp,
                                                    sender,
                                                    request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnUnpublishFirmwareRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomUnpublishFirmwareRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnUnpublishFirmwareResponseReceived += (timestamp,
                                                         sender,
                                                         request,
                                                         response,
                                                         runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnUnpublishFirmwareResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomUnpublishFirmwareRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomUnpublishFirmwareResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnUpdateFirmware                        (RequestSent/-ResponseReceived)

            CSMS.OnUpdateFirmwareRequestSent += (timestamp,
                                                  sender,
                                                  request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnUpdateFirmwareRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomUpdateFirmwareRequestSerializer,
                                                                           CustomFirmwareSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnUpdateFirmwareResponseReceived += (timestamp,
                                                      sender,
                                                      request,
                                                      response,
                                                      runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnUpdateFirmwareResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomUpdateFirmwareRequestSerializer,
                                                                             CustomFirmwareSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomUpdateFirmwareResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #endregion

            #region Grid

            #region OnAFRRSignal                            (RequestSent/-ResponseReceived)

            CSMS.OnAFRRSignalRequestSent += (timestamp,
                                             sender,
                                             request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnAFRRSignalRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomAFRRSignalRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnAFRRSignalResponseReceived += (timestamp,
                                                  sender,
                                                  request,
                                                  response,
                                                  runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnAFRRSignalResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomAFRRSignalRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomAFRRSignalResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #endregion

            #region LocalList

            #region OnClearCache                            (RequestSent/-ResponseReceived)

            CSMS.OnClearCacheRequestSent += (timestamp,
                                             sender,
                                             request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnClearCacheRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomClearCacheRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnClearCacheResponseReceived += (timestamp,
                                                  sender,
                                                  request,
                                                  response,
                                                  runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnClearCacheResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomClearCacheRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomClearCacheResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnGetLocalListVersion                   (RequestSent/-ResponseReceived)

            CSMS.OnGetLocalListVersionRequestSent += (timestamp,
                                                      sender,
                                                      request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetLocalListVersionRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomGetLocalListVersionRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnGetLocalListVersionResponseReceived += (timestamp,
                                                           sender,
                                                           request,
                                                           response,
                                                           runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnGetLocalListVersionResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomGetLocalListVersionRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomGetLocalListVersionResponseSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnSendLocalList                         (RequestSent/-ResponseReceived)

            CSMS.OnSendLocalListRequestSent += (timestamp,
                                                sender,
                                                request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSendLocalListRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomSendLocalListRequestSerializer,
                                                                           CustomAuthorizationDataSerializer,
                                                                           CustomIdTokenSerializer,
                                                                           CustomAdditionalInfoSerializer,
                                                                           CustomIdTokenInfoSerializer,
                                                                           CustomMessageContentSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnSendLocalListResponseReceived += (timestamp,
                                                     sender,
                                                     request,
                                                     response,
                                                     runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnSendLocalListResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomSendLocalListRequestSerializer,
                                                                             CustomAuthorizationDataSerializer,
                                                                             CustomIdTokenSerializer,
                                                                             CustomAdditionalInfoSerializer,
                                                                             CustomIdTokenInfoSerializer,
                                                                             CustomMessageContentSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomSendLocalListResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #endregion

            #endregion

            #region CSMS            <-> ChargingStation Messages exchanged

            #region OnDataTransfer                          (RequestX/-ResponseX)

            CSMS.OnDataTransferRequestSent += (timestamp,
                                               sender,
                                               request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnDataTransferRequestSent),
                                     request.ToAbstractJSON(request.ToJSON(CustomData2TransferRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnDataTransferResponseReceived += (timestamp,
                                                    sender,
                                                    request,
                                                    response,
                                                    runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnDataTransferResponseReceived),
                                     response.ToAbstractJSON(request. ToJSON(CustomData2TransferRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomData2TransferResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));



            CSMS.OnDataTransferRequestReceived += (timestamp,
                                                   sender,
                                                   connection,
                                                   request) =>

                EventLog.SubmitEvent(nameof(CSMS.OnDataTransferRequestReceived),
                                     request.ToAbstractJSON(request.ToJSON(CustomDataTransferRequestSerializer,
                                                                           CustomSignatureSerializer,
                                                                           CustomCustomDataSerializer)));


            CSMS.OnDataTransferResponseSent += (timestamp,
                                                sender,
                                                connection,
                                                request,
                                                response,
                                                runtime) =>

                EventLog.SubmitEvent(nameof(CSMS.OnDataTransferResponseSent),
                                     response.ToAbstractJSON(request. ToJSON(CustomDataTransferRequestSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer),
                                                             response.ToJSON(CustomDataTransferResponseSerializer,
                                                                             CustomStatusInfoSerializer,
                                                                             CustomSignatureSerializer,
                                                                             CustomCustomDataSerializer)));

            #endregion

            #region OnBinaryDataTransfer                    (RequestX/-ResponseX)

            //CSMS.OnBinaryDataTransferRequestSent += (timestamp,
            //                                         sender,
            //                                         request) =>

            //    EventLog.SubmitEvent(nameof(CSMS.OnBinaryDataTransferRequestSent),
            //                         request.ToAbstractJSON(request.ToJSON(CustomData2TransferRequestSerializer,
            //                                                               CustomSignatureSerializer,
            //                                                               CustomCustomDataSerializer)));


            //CSMS.OnBinaryDataTransferResponseReceived += (timestamp,
            //                                              sender,
            //                                              request,
            //                                              response,
            //                                              runtime) =>

            //    EventLog.SubmitEvent(nameof(CSMS.OnDataTransferResponseReceived),
            //                         response.ToAbstractJSON(request. ToJSON(CustomData2TransferRequestSerializer,
            //                                                                 CustomSignatureSerializer,
            //                                                                 CustomCustomDataSerializer),
            //                                                 response.ToJSON(CustomData2TransferResponseSerializer,
            //                                                                 CustomStatusInfoSerializer,
            //                                                                 CustomSignatureSerializer,
            //                                                                 CustomCustomDataSerializer)));



            //CSMS.OnBinaryDataTransferRequestReceived += (timestamp,
            //                                             sender,
            //                                             connection,
            //                                             request) =>

            //    EventLog.SubmitEvent(nameof(CSMS.OnDataTransferRequestReceived),
            //                         request.ToAbstractJSON(request.ToJSON(CustomDataTransferRequestSerializer,
            //                                                               CustomSignatureSerializer,
            //                                                               CustomCustomDataSerializer)));


            //CSMS.OnBinaryDataTransferResponseSent += (timestamp,
            //                                          sender,
            //                                          connection,
            //                                          request,
            //                                          response,
            //                                          runtime) =>

            //    EventLog.SubmitEvent(nameof(CSMS.OnDataTransferResponseSent),
            //                         response.ToAbstractJSON(request. ToJSON(CustomDataTransferRequestSerializer,
            //                                                                 CustomSignatureSerializer,
            //                                                                 CustomCustomDataSerializer),
            //                                                 response.ToJSON(CustomDataTransferResponseSerializer,
            //                                                                 CustomStatusInfoSerializer,
            //                                                                 CustomSignatureSerializer,
            //                                                                 CustomCustomDataSerializer)));

            #endregion

            #endregion


            // Firmware API download messages
            // Logdata API upload messages
            // Diagnostics API upload messages

        }

        #endregion


        #region (private) RegisterURLTemplates()

        #region Manage HTTP Resources

        #region (protected override) GetResourceStream      (ResourceName)

        protected override Stream? GetResourceStream(String ResourceName)

            => GetResourceStream(ResourceName,
                                 new Tuple<String, Assembly>(HTTPAPI.HTTPRoot, typeof(HTTPAPI).Assembly),
                                 new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                 new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceMemoryStream(ResourceName)

        protected override MemoryStream? GetResourceMemoryStream(String ResourceName)

            => GetResourceMemoryStream(ResourceName,
                                       new Tuple<String, Assembly>(HTTPAPI.HTTPRoot, typeof(HTTPAPI).Assembly),
                                       new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                       new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceString      (ResourceName)

        protected override String GetResourceString(String ResourceName)

            => GetResourceString(ResourceName,
                                 new Tuple<String, Assembly>(HTTPAPI.HTTPRoot, typeof(HTTPAPI).Assembly),
                                 new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                 new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceBytes       (ResourceName)

        protected override Byte[] GetResourceBytes(String ResourceName)

            => GetResourceBytes(ResourceName,
                                new Tuple<String, Assembly>(HTTPAPI.HTTPRoot, typeof(HTTPAPI).Assembly),
                                new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName)

        protected override String MixWithHTMLTemplate(String ResourceName)

            => MixWithHTMLTemplate(ResourceName,
                                   new Tuple<String, Assembly>(HTTPAPI.HTTPRoot, typeof(HTTPAPI).Assembly),
                                   new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                   new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName, HTMLConverter, ResourceAssemblies)

        protected override String MixWithHTMLTemplate(String                ResourceName,
                                                      Func<String, String>  HTMLConverter)

            => MixWithHTMLTemplate(ResourceName,
                                   HTMLConverter,
                                   new Tuple<String, Assembly>(HTTPAPI.HTTPRoot, typeof(HTTPAPI).Assembly),
                                   new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                   new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (Template, ResourceName, ResourceAssemblies)

        protected override String MixWithHTMLTemplate(String   Template,
                                                      String   ResourceName,
                                                      String?  Content   = null)

            => MixWithHTMLTemplate(Template,
                                   ResourceName,
                                   new[] {
                                       new Tuple<String, Assembly>(HTTPAPI.HTTPRoot, typeof(HTTPAPI).Assembly),
                                       new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                       new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly)
                                   },
                                   Content);

        #endregion

        #endregion

        private void RegisterURITemplates()
        {

            HTTPBaseAPI.HTTPServer.AddAuth  (request => {

                #region Allow some URLs for anonymous access...

                if (request.Path.StartsWith(URLPathPrefix + "/js")        ||
                    request.Path.StartsWith(URLPathPrefix + "/events")    ||
                    request.Path.StartsWith(URLPathPrefix + "/chargingStation") ||
                    request.Path.StartsWith(URLPathPrefix + "/chargingStations"))
                {
                    return HTTPExtAPI.Anonymous;
                }

                #endregion

                return null;

            });


            #region /shared/UsersAPI

            //HTTPBaseAPI.RegisterResourcesFolder(this,
            //                                    HTTPHostname.Any,
            //                                    URLPathPrefix + "shared/UsersAPI",
            //                                    HTTPRoot.Substring(0, HTTPRoot.Length - 1),
            //                                    typeof(UsersAPI).Assembly);

            #endregion


            #region / (HTTPRoot)

            //HTTPBaseAPI.MapResourceAssemblyFolder(
            //    HTTPHostname.Any,
            //    URLPathPrefix,
            //    "default",
            //    DefaultFilename: "index.html"
            //);

            #endregion


            #region ~/chargingStationIds

            HTTPBaseAPI.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          URLPathPrefix + "chargingStationIds",
                                          HTTPContentType.Application.JSON_UTF8,
                                          HTTPDelegate: Request => {

                                              return Task.FromResult(
                                                  new HTTPResponse.Builder(Request) {
                                                      HTTPStatusCode             = HTTPStatusCode.OK,
                                                      Server                     = HTTPServiceName,
                                                      Date                       = Timestamp.Now,
                                                      AccessControlAllowOrigin   = "*",
                                                      AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                      AccessControlAllowHeaders  = [ "Authorization" ],
                                                      ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                      Content                    = JSONArray.Create(
                                                                                       csmss.SelectMany(csms => csms.ChargingStations).Select(chargingStation => new JObject(new JProperty("@id", chargingStation.Id.ToString())))
                                                                                   ).ToUTF8Bytes(Newtonsoft.Json.Formatting.None),
                                                      Connection                 = "close"
                                                  }.AsImmutable);

                                          });

            #endregion

            #region ~/chargingStations

            HTTPBaseAPI.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          URLPathPrefix + "chargingStations",
                                          HTTPContentType.Application.JSON_UTF8,
                                          HTTPDelegate: Request => {

                                              return Task.FromResult(
                                                  new HTTPResponse.Builder(Request) {
                                                      HTTPStatusCode             = HTTPStatusCode.OK,
                                                      Server                     = HTTPServiceName,
                                                      Date                       = Timestamp.Now,
                                                      AccessControlAllowOrigin   = "*",
                                                      AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                      AccessControlAllowHeaders  = [ "Authorization" ],
                                                      ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                      Content                    = JSONArray.Create(
                                                                                       csmss.SelectMany(csms => csms.ChargingStations).Select(chargingStation => chargingStation.ToJSON())
                                                                                   ).ToUTF8Bytes(Newtonsoft.Json.Formatting.None),
                                                      Connection                 = "close"
                                                  }.AsImmutable);

                                          });

            #endregion


            #region ~/chargingStations/{chargingStationId}

            HTTPBaseAPI.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          URLPathPrefix + "chargingStations/{chargingStationId}",
                                          HTTPContentType.Application.JSON_UTF8,
                                          HTTPDelegate: Request => {

                                              #region Get HTTP user and its organizations

                                              //// Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                              //if (!TryGetHTTPUser(Request,
                                              //                    out User                   HTTPUser,
                                              //                    out HashSet<Organization>  HTTPOrganizations,
                                              //                    out HTTPResponse.Builder   Response,
                                              //                    AccessLevel:               Access_Levels.ReadOnly,
                                              //                    Recursive:                 true))
                                              //{
                                              //    return Task.FromResult(Response.AsImmutable);
                                              //}

                                              #endregion

                                              #region Check ChargingStationId URL parameter

                                              if (!Request.ParseChargingStation(this,
                                                                                out ChargingStation_Id?    ChargingStationId,
                                                                                out CSMS.ChargingStation?  ChargingStation,
                                                                                out HTTPResponse.Builder?  Response))
                                              {
                                                  return Task.FromResult(Response.AsImmutable);
                                              }

                                              #endregion


                                              return Task.FromResult(
                                                  new HTTPResponse.Builder(Request) {
                                                      HTTPStatusCode             = HTTPStatusCode.OK,
                                                      Server                     = HTTPServiceName,
                                                      Date                       = Timestamp.Now,
                                                      AccessControlAllowOrigin   = "*",
                                                      AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                      AccessControlAllowHeaders  = [ "Authorization" ],
                                                      ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                      Content                    = ChargingStation.ToJSON().ToUTF8Bytes(Newtonsoft.Json.Formatting.None),
                                                      Connection                 = "close"
                                                  }.AsImmutable);

                                          });

            #endregion


        }

        #endregion


    }

}
