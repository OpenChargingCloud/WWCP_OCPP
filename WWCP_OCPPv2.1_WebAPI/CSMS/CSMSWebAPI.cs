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
using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// OCPP Charging Station Management System WebAPI extentions.
    /// </summary>
    public static class CSMSWebAPIExtentions
    {

        #region ParseChargeBoxId(this HTTPRequest, OCPPWebAPI, out ChargeBoxId,                out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the charge box identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPPWebAPI">The OCPP WebAPI.</param>
        /// <param name="ChargeBoxId">The parsed unique charge box identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when charge box identification was found; false else.</returns>
        public static Boolean ParseChargeBoxId(this HTTPRequest           HTTPRequest,
                                               CSMSWebAPI                 OCPPWebAPI,
                                               out ChargeBox_Id?          ChargeBoxId,
                                               out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (OCPPWebAPI  is null)
                throw new ArgumentNullException(nameof(OCPPWebAPI),   "The given OCPP WebAPI must not be null!");

            #endregion

            ChargeBoxId   = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPWebAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            ChargeBoxId = ChargeBox_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!ChargeBoxId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPWebAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid charge box identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseChargeBox  (this HTTPRequest, OCPPWebAPI, out ChargeBoxId, out ChargeBox, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the charge box identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPPWebAPI">The OCPP WebAPI.</param>
        /// <param name="ChargeBoxId">The parsed unique charge box identification.</param>
        /// <param name="ChargeBox">The resolved charge box.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when charge box identification was found; false else.</returns>
        public static Boolean ParseChargeBox(this HTTPRequest           HTTPRequest,
                                             CSMSWebAPI                 OCPPWebAPI,
                                             out ChargeBox_Id?          ChargeBoxId,
                                             out ChargeBox?             ChargeBox,
                                             out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (OCPPWebAPI  is null)
                throw new ArgumentNullException(nameof(OCPPWebAPI),   "The given OCPP WebAPI must not be null!");

            #endregion

            ChargeBoxId   = null;
            ChargeBox     = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPWebAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            ChargeBoxId = ChargeBox_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!ChargeBoxId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPWebAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid charge box identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!OCPPWebAPI.TryGetChargeBox(ChargeBoxId.Value, out ChargeBox)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = OCPPWebAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown charge box identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

    }


    /// <summary>
    /// The OCPP Charging Station Management System WebAPI.
    /// </summary>
    public class CSMSWebAPI : AHTTPAPIExtension<HTTPExtAPI>,
                              IHTTPAPIExtension<HTTPExtAPI>
    {

        #region Data

        /// <summary>
        /// The default HTTP URL prefix.
        /// </summary>
        public readonly HTTPPath                   DefaultURLPathPrefix        = HTTPPath.Parse("webapi");

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public const String                        DefaultHTTPRealm            = "Open Charging Cloud OCPP WebAPI";

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public const String                        HTTPRoot                    = "cloud.charging.open.protocols.OCPPv2_1.WebAPI.HTTPRoot.";


        //ToDo: http://www.iana.org/form/media-types

        /// <summary>
        /// The HTTP content type for serving OCPP+ XML data.
        /// </summary>
        public static readonly HTTPContentType     OCPPPlusJSONContentType     = new HTTPContentType("application", "vnd.OCPPPlus+json", "utf-8", null, null);

        /// <summary>
        /// The HTTP content type for serving OCPP+ HTML data.
        /// </summary>
        public static readonly HTTPContentType     OCPPPlusHTMLContentType     = new HTTPContentType("application", "vnd.OCPPPlus+html", "utf-8", null, null);

        /// <summary>
        /// The unique identification of the OCPP HTTP SSE event log.
        /// </summary>
        public static readonly HTTPEventSource_Id  EventLogId                  = HTTPEventSource_Id.Parse("OCPPEvents");

        #endregion

        #region Properties

        //public ICSMS3                                     CSMS                { get; }

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


        private ConcurrentDictionary<CSMS_Id, ICSMS3> csmss = new();

        public IEnumerable<ICSMS3> CSMSs
            => csmss.Values;


        public IEnumerable<ChargeBox> ChargeBoxes

            => csmss.Values.SelectMany(c => c.ChargeBoxes);

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
        public CustomJObjectSerializerDelegate<CS.DataTransferRequest>?                              CustomDataTransferRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<CSMS.DataTransferResponse>?                           CustomDataTransferResponseSerializer                         { get; set; }

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
        public CustomJObjectSerializerDelegate<NotifyPriorityChargingResponse>?                      CustomNotifyPriorityChargingResponseSerializer               { get; set; }
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
        public CustomJObjectSerializerDelegate<CSMS.DataTransferRequest>?                            CustomData2TransferRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<CS.DataTransferResponse>?                             CustomData2TransferResponseSerializer                         { get; set; }

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

        public CustomJObjectSerializerDelegate<ChargingStation>?                                     CustomChargingStationSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<StatusInfo>?                                          CustomStatusInfoSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<EventData>?                                           CustomEventDataSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<Component>?                                           CustomComponentSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                                                CustomEVSESerializer                                         { get; set; }
        public CustomJObjectSerializerDelegate<Variable>?                                            CustomVariableSerializer                                     { get; set; }
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

        /// <summary>
        /// Attach the given OCPP charging station management system WebAPI to the given HTTP API.
        /// </summary>
        /// <param name="TestCSMS">An OCPP charging station management system.</param>
        /// <param name="HTTPAPI">A HTTP API.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public CSMSWebAPI(HTTPExtAPI                                  HTTPAPI,
                          String?                                     HTTPServerName   = null,
                          HTTPPath?                                   URLPathPrefix    = null,
                          HTTPPath?                                   BasePath         = null,
                          String                                      HTTPRealm        = DefaultHTTPRealm,
                          IEnumerable<KeyValuePair<String, String>>?  HTTPLogins       = null,
                          String?                                     HTMLTemplate     = null)

            : base(HTTPAPI,
                   HTTPServerName,
                   URLPathPrefix,
                   BasePath,
                   HTMLTemplate)

        {

            this.HTTPRealm           = HTTPRealm;
            this.HTTPLogins          = HTTPLogins;

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

            RegisterURITemplates();

        }

        #endregion


        public void AttachCSMS(ICSMS3 CSMS)
        {

            this.csmss.TryAdd(CSMS.Id, CSMS);

            #region WebSocket connections

            #region OnNewTCPConnection

            CSMS.OnNewTCPConnection += async (logTimestamp,
                                              sender,
                                              connection,
                                              eventTrackingId,
                                              cancellationToken) =>

                await this.EventLog.SubmitEvent("OnNewTCPConnection",
                                                JSONObject.Create(
                                                    new JProperty("connection", connection.ToString())
                                                ));

            #endregion

            #region OnNewWebSocketConnection

            CSMS.OnNewWebSocketConnection += async (logTimestamp,
                                                    sender,
                                                    connection,
                                                    eventTrackingId,
                                                    cancellationToken) =>

                await this.EventLog.SubmitEvent("OnNewWebSocketConnection",
                                                JSONObject.Create(
                                                    new JProperty("connection", connection.ToString())
                                                ));

            #endregion

            #region OnCloseMessageReceived

            CSMS.OnCloseMessageReceived += async (logTimestamp,
                                                  sender,
                                                  connection,
                                                  eventTrackingId,
                                                  statusCode,
                                                  reason) =>

                await this.EventLog.SubmitEvent("OnCloseMessageReceived",
                                                JSONObject.Create(
                                                    new JProperty("connection",   connection.ToString()),
                                                    new JProperty("statusCode",   statusCode.ToString()),
                                                    new JProperty("reason",       reason)
                                                ));

            #endregion

            #region OnTCPConnectionClosed

            CSMS.OnTCPConnectionClosed += async (logTimestamp,
                                                 sender,
                                                 connection,
                                                 reason,
                                                 eventTrackingId) =>

                await this.EventLog.SubmitEvent("OnTCPConnectionClosed",
                                                JSONObject.Create(
                                                    new JProperty("connection",  connection.ToString()),
                                                    new JProperty("reason",      reason)
                                                ));

            #endregion

            // Failed (Charging Station) Authentication

            // (Generic) Error Handling

            #endregion


            #region Generic Text Messages

            #region OnTextMessageRequestReceived

            CSMS.OnTextMessageRequestReceived += async (timestamp,
                                                        webSocketServer,
                                                        webSocketConnection,
                                                        eventTrackingId,
                                                        requestTimestamp,
                                                        requestMessage) =>

                await this.EventLog.SubmitEvent("OnTextMessageRequestReceived",
                                                JSONObject.Create(
                                                    new JProperty("connection",   webSocketConnection.ToString()),
                                                    new JProperty("message",      requestMessage)
                                                ));

            #endregion

            #region OnTextMessageResponseSent

            CSMS.OnTextMessageResponseSent += async (timestamp,
                                                     webSocketServer,
                                                     webSocketConnection,
                                                     eventTrackingId,
                                                     requestTimestamp,
                                                     requestMessage,
                                                     responseTimestamp,
                                                     responseMessage) =>

                await this.EventLog.SubmitEvent("OnTextMessageResponseSent",
                                                JSONObject.Create(
                                                    new JProperty("connection",   webSocketConnection.ToString()),
                                                    new JProperty("message",      responseMessage)
                                                ));

            #endregion

            #region OnTextErrorResponseSent

            CSMS.OnTextErrorResponseSent += async (timestamp,
                                                   webSocketServer,
                                                   webSocketConnection,
                                                   eventTrackingId,
                                                   requestTimestamp,
                                                   requestMessage,
                                                   responseTimestamp,
                                                   responseMessage) =>

                await this.EventLog.SubmitEvent("OnTextErrorResponseSent",
                                                JSONObject.Create(
                                                    new JProperty("connection",   webSocketConnection.ToString()),
                                                    new JProperty("message",      responseMessage)
                                                ));

            #endregion


            #region OnTextMessageRequestSent

            CSMS.OnTextMessageRequestSent += async (timestamp,
                                                    webSocketServer,
                                                    webSocketConnection,
                                                    eventTrackingId,
                                                    requestTimestamp,
                                                    requestMessage) =>

                await this.EventLog.SubmitEvent("OnTextMessageRequestSent",
                                                JSONObject.Create(
                                                    new JProperty("connection",   webSocketConnection.ToString()),
                                                    new JProperty("message",      requestMessage)
                                                ));

            #endregion

            #region OnTextMessageResponseReceived

            CSMS.OnTextMessageResponseReceived += async (timestamp,
                                                         webSocketServer,
                                                         webSocketConnection,
                                                         eventTrackingId,
                                                         requestTimestamp,
                                                         requestMessage,
                                                         responseTimestamp,
                                                         responseMessage) =>

                await this.EventLog.SubmitEvent("OnTextMessageResponseReceived",
                                                JSONObject.Create(
                                                    new JProperty("connection",   webSocketConnection.ToString()),
                                                    new JProperty("message",      responseMessage)
                                                ));

            #endregion

            #region OnTextErrorResponseReceived

            CSMS.OnTextErrorResponseReceived += async (timestamp,
                                                       webSocketServer,
                                                       webSocketConnection,
                                                       eventTrackingId,
                                                       requestTimestamp,
                                                       requestMessage,
                                                       responseTimestamp,
                                                       responseMessage) =>

                await this.EventLog.SubmitEvent("OnTextErrorResponseReceived",
                                                JSONObject.Create(
                                                    new JProperty("connection",   webSocketConnection.ToString()),
                                                    new JProperty("message",      responseMessage)
                                                ));

            #endregion

            #endregion

            #region Generic Binary Messages

            #region OnBinaryMessageRequestReceived

            CSMS.OnBinaryMessageRequestReceived += async (timestamp,
                                                          webSocketServer,
                                                          webSocketConnection,
                                                          eventTrackingId,
                                                          requestTimestamp,
                                                          requestMessage) =>

                await this.EventLog.SubmitEvent("OnBinaryMessageRequestReceived",
                                                JSONObject.Create(
                                                    new JProperty("connection",   webSocketConnection.ToString()),
                                                    new JProperty("message",      requestMessage)  // BASE64 encoded string!
                                                ));

            #endregion

            #region OnBinaryMessageResponseSent

            CSMS.OnBinaryMessageResponseSent += async (timestamp,
                                                       webSocketServer,
                                                       webSocketConnection,
                                                       eventTrackingId,
                                                       requestTimestamp,
                                                       requestMessage,
                                                       responseTimestamp,
                                                       responseMessage) =>

                await this.EventLog.SubmitEvent("OnBinaryMessageResponseSent",
                                                JSONObject.Create(
                                                    new JProperty("connection",   webSocketConnection.ToString()),
                                                    new JProperty("message",      responseMessage)  // BASE64 encoded string!
                                                ));

            #endregion

            #region OnBinaryErrorResponseSent

            CSMS.OnBinaryErrorResponseSent += async (timestamp,
                                                     webSocketServer,
                                                     webSocketConnection,
                                                     eventTrackingId,
                                                     requestTimestamp,
                                                     requestMessage,
                                                     responseTimestamp,
                                                     responseMessage) =>

                await this.EventLog.SubmitEvent("OnBinaryErrorResponseSent",
                                                JSONObject.Create(
                                                    new JProperty("connection",   webSocketConnection.ToString()),
                                                    new JProperty("message",      responseMessage)  // BASE64 encoded string!
                                                ));

            #endregion


            #region OnBinaryMessageRequestSent

            CSMS.OnBinaryMessageRequestSent += async (timestamp,
                                                      webSocketServer,
                                                      webSocketConnection,
                                                      eventTrackingId,
                                                      requestTimestamp,
                                                      requestMessage) =>

                await this.EventLog.SubmitEvent("OnBinaryMessageRequestSent",
                                                JSONObject.Create(
                                                    new JProperty("connection",   webSocketConnection.ToString()),
                                                    new JProperty("message",      requestMessage)  // BASE64 encoded string!
                                                ));

            #endregion

            #region OnBinaryMessageResponseReceived

            CSMS.OnBinaryMessageResponseReceived += async (timestamp,
                                                           webSocketServer,
                                                           webSocketConnection,
                                                           eventTrackingId,
                                                           requestTimestamp,
                                                           requestMessage,
                                                           responseTimestamp,
                                                           responseMessage) =>

                await this.EventLog.SubmitEvent("OnBinaryMessageResponseReceived",
                                                JSONObject.Create(
                                                    new JProperty("connection",   webSocketConnection.ToString()),
                                                    new JProperty("message",      responseMessage)  // BASE64 encoded string!
                                                ));

            #endregion

            #region OnBinaryErrorResponseReceived

            CSMS.OnBinaryErrorResponseReceived += async (timestamp,
                                                         webSocketServer,
                                                         webSocketConnection,
                                                         eventTrackingId,
                                                         requestTimestamp,
                                                         requestMessage,
                                                         responseTimestamp,
                                                         responseMessage) =>

                await this.EventLog.SubmitEvent("OnBinaryErrorResponseReceived",
                                                JSONObject.Create(
                                                    new JProperty("connection",   webSocketConnection.ToString()),
                                                    new JProperty("message",      responseMessage)  // BASE64 encoded string!
                                                ));

            #endregion

            #endregion


            #region CSMS <- Charging Station Messages

            #region OnBootNotification                      (-Request/-Response)

            CSMS.OnBootNotificationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnBootNotificationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomBootNotificationRequestSerializer,
                                                                                      CustomChargingStationSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnBootNotificationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnBootNotificationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomBootNotificationRequestSerializer,
                                                                                        CustomChargingStationSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomBootNotificationResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnFirmwareStatusNotification            (-Request/-Response)

            CSMS.OnFirmwareStatusNotificationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnFirmwareStatusNotificationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomFirmwareStatusNotificationRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnFirmwareStatusNotificationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnFirmwareStatusNotificationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomFirmwareStatusNotificationRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomFirmwareStatusNotificationResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnPublishFirmwareStatusNotification     (-Request/-Response)

            CSMS.OnPublishFirmwareStatusNotificationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnPublishFirmwareStatusNotificationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomPublishFirmwareStatusNotificationRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnPublishFirmwareStatusNotificationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnPublishFirmwareStatusNotificationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomPublishFirmwareStatusNotificationRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomPublishFirmwareStatusNotificationResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnHeartbeat                             (-Request/-Response)

            CSMS.OnHeartbeatRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnHeartbeatRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomHeartbeatRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnHeartbeatResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnHeartbeatResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomHeartbeatRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomHeartbeatResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyEvent                           (-Request/-Response)

            CSMS.OnNotifyEventRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnNotifyEventRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomNotifyEventRequestSerializer,
                                                                                      CustomEventDataSerializer,
                                                                                      CustomComponentSerializer,
                                                                                      CustomEVSESerializer,
                                                                                      CustomVariableSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyEventResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyEventResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomNotifyEventRequestSerializer,
                                                                                        CustomEventDataSerializer,
                                                                                        CustomComponentSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomVariableSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyEventResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnSecurityEventNotification             (-Request/-Response)

            CSMS.OnSecurityEventNotificationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnSecurityEventNotificationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomSecurityEventNotificationRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSecurityEventNotificationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSecurityEventNotificationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomSecurityEventNotificationRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomSecurityEventNotificationResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyReport                          (-Request/-Response)

            CSMS.OnNotifyReportRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnNotifyReportRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomNotifyReportRequestSerializer,
                                                                                      CustomReportDataSerializer,
                                                                                      CustomComponentSerializer,
                                                                                      CustomEVSESerializer,
                                                                                      CustomVariableSerializer,
                                                                                      CustomVariableAttributeSerializer,
                                                                                      CustomVariableCharacteristicsSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyReportResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyReportResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomNotifyReportRequestSerializer,
                                                                                        CustomReportDataSerializer,
                                                                                        CustomComponentSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomVariableSerializer,
                                                                                        CustomVariableAttributeSerializer,
                                                                                        CustomVariableCharacteristicsSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyReportResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyMonitoringReport                (-Request/-Response)

            CSMS.OnNotifyMonitoringReportRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnNotifyMonitoringReportRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomNotifyMonitoringReportRequestSerializer,
                                                                                      CustomMonitoringDataSerializer,
                                                                                      CustomComponentSerializer,
                                                                                      CustomEVSESerializer,
                                                                                      CustomVariableSerializer,
                                                                                      CustomVariableMonitoringSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyMonitoringReportResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyMonitoringReportResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomNotifyMonitoringReportRequestSerializer,
                                                                                        CustomMonitoringDataSerializer,
                                                                                        CustomComponentSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomVariableSerializer,
                                                                                        CustomVariableMonitoringSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyMonitoringReportResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnLogStatusNotification                 (-Request/-Response)

            CSMS.OnLogStatusNotificationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnLogStatusNotificationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomLogStatusNotificationRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnLogStatusNotificationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnLogStatusNotificationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomLogStatusNotificationRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomLogStatusNotificationResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnIncomingDataTransfer                  (-Request/-Response)

            CSMS.OnIncomingDataTransferRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnIncomingDataTransferRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomDataTransferRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnIncomingDataTransferResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnIncomingDataTransferResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomDataTransferRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomDataTransferResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion


            #region OnSignCertificate                       (-Request/-Response)

            CSMS.OnSignCertificateRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnSignCertificateRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomSignCertificateRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSignCertificateResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSignCertificateResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomSignCertificateRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomSignCertificateResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGet15118EVCertificate                 (-Request/-Response)

            CSMS.OnGet15118EVCertificateRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGet15118EVCertificateRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGet15118EVCertificateRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGet15118EVCertificateResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGet15118EVCertificateResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGet15118EVCertificateRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGet15118EVCertificateResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetCertificateStatus                  (-Request/-Response)

            CSMS.OnGetCertificateStatusRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetCertificateStatusRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetCertificateStatusRequestSerializer,
                                                                                      CustomOCSPRequestDataSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetCertificateStatusResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetCertificateStatusResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetCertificateStatusRequestSerializer,
                                                                                        CustomOCSPRequestDataSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetCertificateStatusResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetCRL                                (-Request/-Response)

            CSMS.OnGetCRLRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetCRLRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetCRLRequestSerializer,
                                                                                      CustomCertificateHashDataSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetCRLResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetCRLResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetCRLRequestSerializer,
                                                                                        CustomCertificateHashDataSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetCRLResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion


            #region OnReservationStatusUpdate               (-Request/-Response)

            CSMS.OnReservationStatusUpdateRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnReservationStatusUpdateRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomReservationStatusUpdateRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnReservationStatusUpdateResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnReservationStatusUpdateResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomReservationStatusUpdateRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomReservationStatusUpdateResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnAuthorize                             (-Request/-Response)

            CSMS.OnAuthorizeRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnAuthorizeRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomAuthorizeRequestSerializer,
                                                                                      CustomIdTokenSerializer,
                                                                                      CustomAdditionalInfoSerializer,
                                                                                      CustomOCSPRequestDataSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnAuthorizeResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnAuthorizeResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomAuthorizeRequestSerializer,
                                                                                        CustomIdTokenSerializer,
                                                                                        CustomAdditionalInfoSerializer,
                                                                                        CustomOCSPRequestDataSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomAuthorizeResponseSerializer,
                                                                                        CustomIdTokenInfoSerializer,
                                                                                        CustomIdTokenSerializer,
                                                                                        CustomAdditionalInfoSerializer,
                                                                                        CustomMessageContentSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyEVChargingNeeds                 (-Request/-Response)

            CSMS.OnNotifyEVChargingNeedsRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnNotifyEVChargingNeedsRequest",
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
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyEVChargingNeedsResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyEVChargingNeedsResponse",
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
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyEVChargingNeedsResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnTransactionEvent                      (-Request/-Response)

            CSMS.OnTransactionEventRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnTransactionEventRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomTransactionEventRequestSerializer,
                                                                                      CustomTransactionSerializer,
                                                                                      CustomIdTokenSerializer,
                                                                                      CustomAdditionalInfoSerializer,
                                                                                      CustomEVSESerializer,
                                                                                      CustomMeterValueSerializer,
                                                                                      CustomSampledValueSerializer,
                                                                                      CustomSignedMeterValueSerializer,
                                                                                      CustomUnitsOfMeasureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnTransactionEventResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnTransactionEventResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomTransactionEventRequestSerializer,
                                                                                        CustomTransactionSerializer,
                                                                                        CustomIdTokenSerializer,
                                                                                        CustomAdditionalInfoSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomMeterValueSerializer,
                                                                                        CustomSampledValueSerializer,
                                                                                        CustomSignedMeterValueSerializer,
                                                                                        CustomUnitsOfMeasureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomTransactionEventResponseSerializer,
                                                                                        CustomIdTokenInfoSerializer,
                                                                                        CustomIdTokenSerializer,
                                                                                        CustomAdditionalInfoSerializer,
                                                                                        CustomMessageContentSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnStatusNotification                    (-Request/-Response)

            CSMS.OnStatusNotificationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnStatusNotificationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomStatusNotificationRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnStatusNotificationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnStatusNotificationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomStatusNotificationRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomStatusNotificationResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnMeterValues                           (-Request/-Response)

            CSMS.OnMeterValuesRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnMeterValuesRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomMeterValuesRequestSerializer,
                                                                                      CustomMeterValueSerializer,
                                                                                      CustomSampledValueSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnMeterValuesResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnMeterValuesResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomMeterValuesRequestSerializer,
                                                                                        CustomMeterValueSerializer,
                                                                                        CustomSampledValueSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomMeterValuesResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyChargingLimit                   (-Request/-Response)

            CSMS.OnNotifyChargingLimitRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnNotifyChargingLimitRequest",
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

                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyChargingLimitResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyChargingLimitResponse",
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

                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyChargingLimitResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnClearedChargingLimit                  (-Request/-Response)

            CSMS.OnClearedChargingLimitRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnClearedChargingLimitRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomClearedChargingLimitRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnClearedChargingLimitResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnClearedChargingLimitResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomClearedChargingLimitRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomClearedChargingLimitResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnReportChargingProfiles                (-Request/-Response)

            CSMS.OnReportChargingProfilesRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnReportChargingProfilesRequest",
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

                                                                                      CustomCustomDataSerializer)));

            CSMS.OnReportChargingProfilesResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnReportChargingProfilesResponse",
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

                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomReportChargingProfilesResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyEVChargingSchedule              (-Request/-Response)

            CSMS.OnNotifyEVChargingScheduleRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnNotifyEVChargingScheduleRequest",
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

                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyEVChargingScheduleResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyEVChargingScheduleResponse",
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

                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyEVChargingScheduleResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyPriorityCharging                (-Request/-Response)

            CSMS.OnNotifyPriorityChargingRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnNotifyPriorityChargingRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomNotifyPriorityChargingRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyPriorityChargingResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyPriorityChargingResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomNotifyPriorityChargingRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyPriorityChargingResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnPullDynamicScheduleUpdate             (-Request/-Response)

            CSMS.OnPullDynamicScheduleUpdateRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnPullDynamicScheduleUpdateRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomPullDynamicScheduleUpdateRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnPullDynamicScheduleUpdateResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnPullDynamicScheduleUpdateResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomPullDynamicScheduleUpdateRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomPullDynamicScheduleUpdateResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion


            #region OnNotifyDisplayMessages                 (-Request/-Response)

            CSMS.OnNotifyDisplayMessagesRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnNotifyDisplayMessagesRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomNotifyDisplayMessagesRequestSerializer,
                                                                                      CustomMessageInfoSerializer,
                                                                                      CustomMessageContentSerializer,
                                                                                      CustomComponentSerializer,
                                                                                      CustomEVSESerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyDisplayMessagesResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyDisplayMessagesResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomNotifyDisplayMessagesRequestSerializer,
                                                                                        CustomMessageInfoSerializer,
                                                                                        CustomMessageContentSerializer,
                                                                                        CustomComponentSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyDisplayMessagesResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyCustomerInformation             (-Request/-Response)

            CSMS.OnNotifyCustomerInformationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnNotifyCustomerInformationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomNotifyCustomerInformationRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyCustomerInformationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyCustomerInformationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomNotifyCustomerInformationRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyCustomerInformationResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #endregion

            #region CSMS -> Charging Station Messages

            #region OnReset                                 (-Request/-Response)

            CSMS.OnResetRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnResetRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomResetRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnResetResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnResetResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomResetRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomResetResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnUpdateFirmware                        (-Request/-Response)

            CSMS.OnUpdateFirmwareRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnUpdateFirmwareRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomUpdateFirmwareRequestSerializer,
                                                                                      CustomFirmwareSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnUpdateFirmwareResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnUpdateFirmwareResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomUpdateFirmwareRequestSerializer,
                                                                                        CustomFirmwareSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomUpdateFirmwareResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnPublishFirmware                       (-Request/-Response)

            CSMS.OnPublishFirmwareRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnPublishFirmwareRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomPublishFirmwareRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnPublishFirmwareResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnPublishFirmwareResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomPublishFirmwareRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomPublishFirmwareResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnUnpublishFirmware                     (-Request/-Response)

            CSMS.OnUnpublishFirmwareRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnUnpublishFirmwareRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomUnpublishFirmwareRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnUnpublishFirmwareResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnUnpublishFirmwareResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomUnpublishFirmwareRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomUnpublishFirmwareResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetBaseReport                         (-Request/-Response)

            CSMS.OnGetBaseReportRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetBaseReportRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetBaseReportRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetBaseReportResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetBaseReportResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetBaseReportRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetBaseReportResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetReport                             (-Request/-Response)

            CSMS.OnGetReportRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetReportRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetReportRequestSerializer,
                                                                                      CustomComponentVariableSerializer,
                                                                                      CustomComponentSerializer,
                                                                                      CustomEVSESerializer,
                                                                                      CustomVariableSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetReportResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetReportResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetReportRequestSerializer,
                                                                                        CustomComponentVariableSerializer,
                                                                                        CustomComponentSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomVariableSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetReportResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetLog                                (-Request/-Response)

            CSMS.OnGetLogRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetLogRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetLogRequestSerializer,
                                                                                      CustomLogParametersSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetLogResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetLogResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetLogRequestSerializer,
                                                                                        CustomLogParametersSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetLogResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnSetVariables                          (-Request/-Response)

            CSMS.OnSetVariablesRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnSetVariablesRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomSetVariablesRequestSerializer,
                                                                                      CustomSetVariableDataSerializer,
                                                                                      CustomComponentSerializer,
                                                                                      CustomEVSESerializer,
                                                                                      CustomVariableSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSetVariablesResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSetVariablesResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomSetVariablesRequestSerializer,
                                                                                        CustomSetVariableDataSerializer,
                                                                                        CustomComponentSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomVariableSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomSetVariablesResponseSerializer,
                                                                                        CustomSetVariableResultSerializer,
                                                                                        CustomComponentSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomVariableSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetVariables                          (-Request/-Response)

            CSMS.OnGetVariablesRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetVariablesRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetVariablesRequestSerializer,
                                                                                      CustomGetVariableDataSerializer,
                                                                                      CustomComponentSerializer,
                                                                                      CustomEVSESerializer,
                                                                                      CustomVariableSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetVariablesResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetVariablesResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetVariablesRequestSerializer,
                                                                                        CustomGetVariableDataSerializer,
                                                                                        CustomComponentSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomVariableSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetVariablesResponseSerializer,
                                                                                        CustomGetVariableResultSerializer,
                                                                                        CustomComponentSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomVariableSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnSetMonitoringBase                     (-Request/-Response)

            CSMS.OnSetMonitoringBaseRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnSetMonitoringBaseRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomSetMonitoringBaseRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSetMonitoringBaseResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSetMonitoringBaseResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomSetMonitoringBaseRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomSetMonitoringBaseResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetMonitoringReport                   (-Request/-Response)

            CSMS.OnGetMonitoringReportRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetMonitoringReportRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetMonitoringReportRequestSerializer,
                                                                                      CustomComponentVariableSerializer,
                                                                                      CustomComponentSerializer,
                                                                                      CustomEVSESerializer,
                                                                                      CustomVariableSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetMonitoringReportResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetMonitoringReportResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetMonitoringReportRequestSerializer,
                                                                                        CustomComponentVariableSerializer,
                                                                                        CustomComponentSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomVariableSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetMonitoringReportResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnSetMonitoringLevel                    (-Request/-Response)

            CSMS.OnSetMonitoringLevelRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnSetMonitoringLevelRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomSetMonitoringLevelRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSetMonitoringLevelResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSetMonitoringLevelResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomSetMonitoringLevelRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomSetMonitoringLevelResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnSetVariableMonitoring                 (-Request/-Response)

            CSMS.OnSetVariableMonitoringRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnSetVariableMonitoringRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomSetVariableMonitoringRequestSerializer,
                                                                                      CustomSetMonitoringDataSerializer,
                                                                                      CustomComponentSerializer,
                                                                                      CustomEVSESerializer,
                                                                                      CustomVariableSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSetVariableMonitoringResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSetVariableMonitoringResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomSetVariableMonitoringRequestSerializer,
                                                                                        CustomSetMonitoringDataSerializer,
                                                                                        CustomComponentSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomVariableSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomSetVariableMonitoringResponseSerializer,
                                                                                        CustomSetMonitoringResultSerializer,
                                                                                        CustomComponentSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomVariableSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnClearVariableMonitoring               (-Request/-Response)

            CSMS.OnClearVariableMonitoringRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnClearVariableMonitoringRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomClearVariableMonitoringRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnClearVariableMonitoringResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnClearVariableMonitoringResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomClearVariableMonitoringRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomClearVariableMonitoringResponseSerializer,
                                                                                        CustomClearMonitoringResultSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnSetNetworkProfile                     (-Request/-Response)

            CSMS.OnSetNetworkProfileRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnSetNetworkProfileRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomSetNetworkProfileRequestSerializer,
                                                                                      CustomNetworkConnectionProfileSerializer,
                                                                                      CustomVPNConfigurationSerializer,
                                                                                      CustomAPNConfigurationSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSetNetworkProfileResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSetNetworkProfileResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomSetNetworkProfileRequestSerializer,
                                                                                        CustomNetworkConnectionProfileSerializer,
                                                                                        CustomVPNConfigurationSerializer,
                                                                                        CustomAPNConfigurationSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomSetNetworkProfileResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnChangeAvailability                    (-Request/-Response)

            CSMS.OnChangeAvailabilityRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnChangeAvailabilityRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomChangeAvailabilityRequestSerializer,
                                                                                      CustomEVSESerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnChangeAvailabilityResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnChangeAvailabilityResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomChangeAvailabilityRequestSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomChangeAvailabilityResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnTriggerMessage                        (-Request/-Response)

            CSMS.OnTriggerMessageRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnTriggerMessageRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomTriggerMessageRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnTriggerMessageResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnTriggerMessageResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomTriggerMessageRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomTriggerMessageResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnDataTransfer                          (-Request/-Response)

            CSMS.OnDataTransferRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnDataTransferRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomData2TransferRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnDataTransferResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnDataTransferResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomData2TransferRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomData2TransferResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion


            #region OnCertificateSigned                     (-Request/-Response)

            CSMS.OnCertificateSignedRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnCertificateSignedRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomCertificateSignedRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnCertificateSignedResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnCertificateSignedResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomCertificateSignedRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomCertificateSignedResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnInstallCertificate                    (-Request/-Response)

            CSMS.OnInstallCertificateRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnInstallCertificateRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomInstallCertificateRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnInstallCertificateResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnInstallCertificateResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomInstallCertificateRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomInstallCertificateResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetInstalledCertificateIds            (-Request/-Response)

            CSMS.OnGetInstalledCertificateIdsRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetInstalledCertificateIdsRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetInstalledCertificateIdsRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetInstalledCertificateIdsResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetInstalledCertificateIdsResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetInstalledCertificateIdsRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetInstalledCertificateIdsResponseSerializer,
                                                                                        CustomCertificateHashDataSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnDeleteCertificate                     (-Request/-Response)

            CSMS.OnDeleteCertificateRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnDeleteCertificateRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomDeleteCertificateRequestSerializer,
                                                                                      CustomCertificateHashDataSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnDeleteCertificateResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnDeleteCertificateResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomDeleteCertificateRequestSerializer,
                                                                                        CustomCertificateHashDataSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomDeleteCertificateResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyCRL                             (-Request/-Response)

            CSMS.OnNotifyCRLRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnNotifyCRLRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomNotifyCRLRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyCRLResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyCRLResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomNotifyCRLRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyCRLResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion


            #region OnGetLocalListVersion                   (-Request/-Response)

            CSMS.OnGetLocalListVersionRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetLocalListVersionRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetLocalListVersionRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetLocalListVersionResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetLocalListVersionResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetLocalListVersionRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetLocalListVersionResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnSendLocalList                         (-Request/-Response)

            CSMS.OnSendLocalListRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnSendLocalListRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomSendLocalListRequestSerializer,
                                                                                      CustomAuthorizationDataSerializer,
                                                                                      CustomIdTokenSerializer,
                                                                                      CustomAdditionalInfoSerializer,
                                                                                      CustomIdTokenInfoSerializer,
                                                                                      CustomMessageContentSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSendLocalListResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSendLocalListResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomSendLocalListRequestSerializer,
                                                                                        CustomAuthorizationDataSerializer,
                                                                                        CustomIdTokenSerializer,
                                                                                        CustomAdditionalInfoSerializer,
                                                                                        CustomIdTokenInfoSerializer,
                                                                                        CustomMessageContentSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomSendLocalListResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnClearCache                            (-Request/-Response)

            CSMS.OnClearCacheRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnClearCacheRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomClearCacheRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnClearCacheResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnClearCacheResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomClearCacheRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomClearCacheResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion


            #region OnReserveNow                            (-Request/-Response)

            CSMS.OnReserveNowRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnReserveNowRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomReserveNowRequestSerializer,
                                                                                      CustomIdTokenSerializer,
                                                                                      CustomAdditionalInfoSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnReserveNowResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnReserveNowResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomReserveNowRequestSerializer,
                                                                                        CustomIdTokenSerializer,
                                                                                        CustomAdditionalInfoSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomReserveNowResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnCancelReservation                     (-Request/-Response)

            CSMS.OnCancelReservationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnCancelReservationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomCancelReservationRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnCancelReservationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnCancelReservationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomCancelReservationRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomCancelReservationResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnRequestStartTransaction               (-Request/-Response)

            CSMS.OnRequestStartTransactionRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnRequestStartTransactionRequest",
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

                                                                                      CustomCustomDataSerializer)));

            CSMS.OnRequestStartTransactionResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnRequestStartTransactionResponse",
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

                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomRequestStartTransactionResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnRequestStopTransaction                (-Request/-Response)

            CSMS.OnRequestStopTransactionRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnRequestStopTransactionRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomRequestStopTransactionRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnRequestStopTransactionResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnRequestStopTransactionResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomRequestStopTransactionRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomRequestStopTransactionResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetTransactionStatus                  (-Request/-Response)

            CSMS.OnGetTransactionStatusRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetTransactionStatusRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetTransactionStatusRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetTransactionStatusResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetTransactionStatusResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetTransactionStatusRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetTransactionStatusResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnSetChargingProfile                    (-Request/-Response)

            CSMS.OnSetChargingProfileRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnSetChargingProfileRequest",
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

                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSetChargingProfileResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSetChargingProfileResponse",
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

                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomSetChargingProfileResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetChargingProfiles                   (-Request/-Response)

            CSMS.OnGetChargingProfilesRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetChargingProfilesRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetChargingProfilesRequestSerializer,
                                                                                      CustomChargingProfileCriterionSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetChargingProfilesResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetChargingProfilesResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetChargingProfilesRequestSerializer,
                                                                                        CustomChargingProfileCriterionSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetChargingProfilesResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnClearChargingProfile                  (-Request/-Response)

            CSMS.OnClearChargingProfileRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnClearChargingProfileRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomClearChargingProfileRequestSerializer,
                                                                                      CustomClearChargingProfileSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnClearChargingProfileResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnClearChargingProfileResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomClearChargingProfileRequestSerializer,
                                                                                        CustomClearChargingProfileSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomClearChargingProfileResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetCompositeSchedule                  (-Request/-Response)

            CSMS.OnGetCompositeScheduleRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetCompositeScheduleRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetCompositeScheduleRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetCompositeScheduleResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetCompositeScheduleResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetCompositeScheduleRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetCompositeScheduleResponseSerializer,
                                                                                        CustomCompositeScheduleSerializer,
                                                                                        CustomChargingSchedulePeriodSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnUpdateDynamicSchedule                 (-Request/-Response)

            CSMS.OnUpdateDynamicScheduleRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnUpdateDynamicScheduleRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomUpdateDynamicScheduleRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnUpdateDynamicScheduleResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnUpdateDynamicScheduleResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomUpdateDynamicScheduleRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomUpdateDynamicScheduleResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyAllowedEnergyTransfer           (-Request/-Response)

            CSMS.OnNotifyAllowedEnergyTransferRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnNotifyAllowedEnergyTransferRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomNotifyAllowedEnergyTransferRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyAllowedEnergyTransferResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyAllowedEnergyTransferResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomNotifyAllowedEnergyTransferRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyAllowedEnergyTransferResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnUsePriorityCharging                   (-Request/-Response)

            CSMS.OnUsePriorityChargingRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnUsePriorityChargingRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomUsePriorityChargingRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnUsePriorityChargingResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnUsePriorityChargingResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomUsePriorityChargingRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomUsePriorityChargingResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnUnlockConnector                       (-Request/-Response)

            CSMS.OnUnlockConnectorRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnUnlockConnectorRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomUnlockConnectorRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnUnlockConnectorResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnUnlockConnectorResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomUnlockConnectorRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomUnlockConnectorResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion


            #region OnAFRRSignal                            (-Request/-Response)

            CSMS.OnAFRRSignalRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnAFRRSignalRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomAFRRSignalRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnAFRRSignalResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnAFRRSignalResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomAFRRSignalRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomAFRRSignalResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion


            #region OnSetDisplayMessage                     (-Request/-Response)

            CSMS.OnSetDisplayMessageRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnSetDisplayMessageRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomSetDisplayMessageRequestSerializer,
                                                                                      CustomMessageInfoSerializer,
                                                                                      CustomMessageContentSerializer,
                                                                                      CustomComponentSerializer,
                                                                                      CustomEVSESerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSetDisplayMessageResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSetDisplayMessageResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomSetDisplayMessageRequestSerializer,
                                                                                        CustomMessageInfoSerializer,
                                                                                        CustomMessageContentSerializer,
                                                                                        CustomComponentSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomSetDisplayMessageResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetDisplayMessages                    (-Request/-Response)

            CSMS.OnGetDisplayMessagesRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetDisplayMessagesRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetDisplayMessagesRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetDisplayMessagesResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetDisplayMessagesResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetDisplayMessagesRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetDisplayMessagesResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnClearDisplayMessage                   (-Request/-Response)

            CSMS.OnClearDisplayMessageRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnClearDisplayMessageRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomClearDisplayMessageRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnClearDisplayMessageResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnClearDisplayMessageResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomClearDisplayMessageRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomClearDisplayMessageResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnCostUpdated                           (-Request/-Response)

            CSMS.OnCostUpdatedRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnCostUpdatedRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomCostUpdatedRequestSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnCostUpdatedResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnCostUpdatedResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomCostUpdatedRequestSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomCostUpdatedResponseSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnCustomerInformation                   (-Request/-Response)

            CSMS.OnCustomerInformationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnCustomerInformationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomCustomerInformationRequestSerializer,
                                                                                      CustomIdTokenSerializer,
                                                                                      CustomAdditionalInfoSerializer,
                                                                                      CustomCertificateHashDataSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnCustomerInformationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnCustomerInformationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomCustomerInformationRequestSerializer,
                                                                                        CustomIdTokenSerializer,
                                                                                        CustomAdditionalInfoSerializer,
                                                                                        CustomCertificateHashDataSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomCustomerInformationResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #endregion


            // Firmware API download messages
            // Logdata API upload messages
            // Diagnostics API upload messages

        }

        public void Attach(ICSMSChannel CSMSChannel)
        {

            #region HTTP-SSEs: CSMS -> ChargePoint

            #region OnReset (-Request/-Response)

            CSMSChannel.OnResetRequest += async (logTimestamp,
                                                        sender,
                                                        request) =>

                await this.EventLog.SubmitEvent("OnResetRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnResetResponse += async (logTimestamp,
                                                         sender,
                                                         request,
                                                         response,
                                                         runtime) =>

                await this.EventLog.SubmitEvent("OnResetResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnChangeAvailability (-Request/-Response)

            CSMSChannel.OnChangeAvailabilityRequest += async (logTimestamp,
                                                                     sender,
                                                                     request) =>

                await this.EventLog.SubmitEvent("OnChangeAvailabilityRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnChangeAvailabilityResponse += async (logTimestamp,
                                                                      sender,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                await this.EventLog.SubmitEvent("OnChangeAvailabilityResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnDataTransfer (-Request/-Response)

            CSMSChannel.OnDataTransferRequest += async (logTimestamp,
                                                               sender,
                                                               request) =>

                await this.EventLog.SubmitEvent("OnDataTransferRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnDataTransferResponse += async (logTimestamp,
                                                                sender,
                                                                request,
                                                                response,
                                                                runtime) =>

                await this.EventLog.SubmitEvent("OnDataTransferResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnTriggerMessage (-Request/-Response)

            CSMSChannel.OnTriggerMessageRequest += async (logTimestamp,
                                                                 sender,
                                                                 request) =>

                await this.EventLog.SubmitEvent("OnTriggerMessageRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnTriggerMessageResponse += async (logTimestamp,
                                                                  sender,
                                                                  request,
                                                                  response,
                                                                  runtime) =>

                await this.EventLog.SubmitEvent("OnTriggerMessageResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnUpdateFirmware (-Request/-Response)

            CSMSChannel.OnUpdateFirmwareRequest += async (logTimestamp,
                                                                 sender,
                                                                 request) =>

                await this.EventLog.SubmitEvent("OnUpdateFirmwareRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnUpdateFirmwareResponse += async (logTimestamp,
                                                                  sender,
                                                                  request,
                                                                  response,
                                                                  runtime) =>

                await this.EventLog.SubmitEvent("OnUpdateFirmwareResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion


            #region OnReserveNow (-Request/-Response)

            CSMSChannel.OnReserveNowRequest += async (logTimestamp,
                                                             sender,
                                                             request) =>

                await this.EventLog.SubmitEvent("OnReserveNowRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnReserveNowResponse += async (logTimestamp,
                                                              sender,
                                                              request,
                                                              response,
                                                              runtime) =>

                await this.EventLog.SubmitEvent("OnReserveNowResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnCancelReservation (-Request/-Response)

            CSMSChannel.OnCancelReservationRequest += async (logTimestamp,
                                                                    sender,
                                                                    request) =>

                await this.EventLog.SubmitEvent("OnCancelReservationRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnCancelReservationResponse += async (logTimestamp,
                                                                     sender,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                await this.EventLog.SubmitEvent("OnCancelReservationResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnSetChargingProfile (-Request/-Response)

            CSMSChannel.OnSetChargingProfileRequest += async (logTimestamp,
                                                                     sender,
                                                                     request) =>

                await this.EventLog.SubmitEvent("OnSetChargingProfileRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnSetChargingProfileResponse += async (logTimestamp,
                                                                      sender,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                await this.EventLog.SubmitEvent("OnSetChargingProfileResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnClearChargingProfile (-Request/-Response)

            CSMSChannel.OnClearChargingProfileRequest += async (logTimestamp,
                                                                       sender,
                                                                       request) =>

                await this.EventLog.SubmitEvent("OnClearChargingProfileRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnClearChargingProfileResponse += async (logTimestamp,
                                                                        sender,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                await this.EventLog.SubmitEvent("OnClearChargingProfileResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnGetCompositeSchedule (-Request/-Response)

            CSMSChannel.OnGetCompositeScheduleRequest += async (logTimestamp,
                                                                       sender,
                                                                       request) =>

                await this.EventLog.SubmitEvent("OnGetCompositeScheduleRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnGetCompositeScheduleResponse += async (logTimestamp,
                                                                        sender,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                await this.EventLog.SubmitEvent("OnGetCompositeScheduleResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnUnlockConnector (-Request/-Response)

            CSMSChannel.OnUnlockConnectorRequest += async (logTimestamp,
                                                                  sender,
                                                                  request) =>

                await this.EventLog.SubmitEvent("OnUnlockConnectorRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnUnlockConnectorResponse += async (logTimestamp,
                                                                   sender,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                await this.EventLog.SubmitEvent("OnUnlockConnectorResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion


            #region OnGetLocalListVersion (-Request/-Response)

            CSMSChannel.OnGetLocalListVersionRequest += async (logTimestamp,
                                                                      sender,
                                                                      request) =>

                await this.EventLog.SubmitEvent("OnGetLocalListVersionRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnGetLocalListVersionResponse += async (logTimestamp,
                                                                       sender,
                                                                       request,
                                                                       response,
                                                                       runtime) =>

                await this.EventLog.SubmitEvent("OnGetLocalListVersionResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnSendLocalList (-Request/-Response)

            CSMSChannel.OnSendLocalListRequest += async (logTimestamp,
                                                                sender,
                                                                request) =>

                await this.EventLog.SubmitEvent("OnSendLocalListRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnSendLocalListResponse += async (logTimestamp,
                                                                 sender,
                                                                 request,
                                                                 response,
                                                                 runtime) =>

                await this.EventLog.SubmitEvent("OnSendLocalListResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnClearCache (-Request/-Response)

            CSMSChannel.OnClearCacheRequest += async (logTimestamp,
                                                             sender,
                                                             request) =>

                await this.EventLog.SubmitEvent("OnClearCacheRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnClearCacheResponse += async (logTimestamp,
                                                              sender,
                                                              request,
                                                              response,
                                                              runtime) =>

                await this.EventLog.SubmitEvent("OnClearCacheResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargeBoxId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #endregion

        }


        public Boolean TryGetChargeBox(ChargeBox_Id ChargeBoxId, out ChargeBox? ChargeBox)
        {

            foreach (var csms in csmss.Values)
            {
                if (csms.TryGetChargeBox(ChargeBoxId, out ChargeBox))
                    return true;
            }

            ChargeBox = null;
            return false;

        }


        #region (private) RegisterURLTemplates()

        #region Manage HTTP Resources

        #region (protected override) GetResourceStream      (ResourceName)

        protected override Stream? GetResourceStream(String ResourceName)

            => GetResourceStream(ResourceName,
                                 new Tuple<String, Assembly>(CSMSWebAPI.HTTPRoot, typeof(CSMSWebAPI).Assembly),
                                 new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                 new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceMemoryStream(ResourceName)

        protected override MemoryStream? GetResourceMemoryStream(String ResourceName)

            => GetResourceMemoryStream(ResourceName,
                                       new Tuple<String, Assembly>(CSMSWebAPI.HTTPRoot, typeof(CSMSWebAPI).Assembly),
                                       new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                       new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceString      (ResourceName)

        protected override String GetResourceString(String ResourceName)

            => GetResourceString(ResourceName,
                                 new Tuple<String, Assembly>(CSMSWebAPI.HTTPRoot, typeof(CSMSWebAPI).Assembly),
                                 new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                 new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceBytes       (ResourceName)

        protected override Byte[] GetResourceBytes(String ResourceName)

            => GetResourceBytes(ResourceName,
                                new Tuple<String, Assembly>(CSMSWebAPI.HTTPRoot, typeof(CSMSWebAPI).Assembly),
                                new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName)

        protected override String MixWithHTMLTemplate(String ResourceName)

            => MixWithHTMLTemplate(ResourceName,
                                   new Tuple<String, Assembly>(CSMSWebAPI.HTTPRoot, typeof(CSMSWebAPI).Assembly),
                                   new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                   new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName, HTMLConverter, ResourceAssemblies)

        protected override String MixWithHTMLTemplate(String                ResourceName,
                                                      Func<String, String>  HTMLConverter)

            => MixWithHTMLTemplate(ResourceName,
                                   HTMLConverter,
                                   new Tuple<String, Assembly>(CSMSWebAPI.HTTPRoot, typeof(CSMSWebAPI).Assembly),
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
                                       new Tuple<String, Assembly>(CSMSWebAPI.HTTPRoot, typeof(CSMSWebAPI).Assembly),
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

                if (request.Path.StartsWith(URLPathPrefix + "/chargeBoxes"))
                {
                    return HTTPExtAPI.Anonymous;
                }

                #endregion

                return null;

            });


            #region / (HTTPRoot)

            //HTTPBaseAPI.RegisterResourcesFolder(this,
            //                                HTTPHostname.Any,
            //                                URLPathPrefix,
            //                                "cloud.charging.open.protocols.OCPPv2_1.WebAPI.HTTPRoot",
            //                                DefaultFilename: "index.html");

            //HTTPServer.AddMethodCallback(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             URLPathPrefix,
            //                             HTTPDelegate: Request => {

            //                                 return Task.FromResult(
            //                                     new HTTPResponse.Builder(Request) {
            //                                         HTTPStatusCode             = HTTPStatusCode.OK,
            //                                         Server                     = DefaultHTTPServerName,
            //                                         Date                       = Timestamp.Now,
            //                                         AccessControlAllowOrigin   = "*",
            //                                         AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
            //                                         AccessControlAllowHeaders  = new[] { "Authorization" },
            //                                         ContentType                = HTTPContentType.HTML_UTF8,
            //                                         Content                    = ("<html><body>" +
            //                                                                          "This is an Open Charge Point Protocol v1.6 HTTP service!<br /><br />" +
            //                                                                          "<ul>" +
            //                                                                              "<li><a href=\"" + URLPathPrefix.ToString() + "/chargeBoxes\">Charge Boxes</a></li>" +
            //                                                                          "</ul>" +
            //                                                                       "<body></html>").ToUTF8Bytes(),
            //                                         Connection                 = "close"
            //                                     }.AsImmutable);

            //                             });

            #endregion


            #region ~/chargeBoxIds

            HTTPBaseAPI.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          URLPathPrefix + "chargeBoxIds",
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: Request => {

                                              return Task.FromResult(
                                                  new HTTPResponse.Builder(Request) {
                                                      HTTPStatusCode             = HTTPStatusCode.OK,
                                                      Server                     = HTTPServiceName,
                                                      Date                       = Timestamp.Now,
                                                      AccessControlAllowOrigin   = "*",
                                                      AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                      AccessControlAllowHeaders  = new[] { "Authorization" },
                                                      ContentType                = HTTPContentType.JSON_UTF8,
                                                      Content                    = JSONArray.Create(
                                                                                       ChargeBoxes.Select(chargeBox => new JObject(new JProperty("@id", chargeBox.Id.ToString())))
                                                                                   ).ToUTF8Bytes(Newtonsoft.Json.Formatting.None),
                                                      Connection                 = "close"
                                                  }.AsImmutable);

                                          });

            #endregion

            #region ~/chargeBoxes

            HTTPBaseAPI.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          URLPathPrefix + "chargeBoxes",
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: Request => {

                                              return Task.FromResult(
                                                  new HTTPResponse.Builder(Request) {
                                                      HTTPStatusCode             = HTTPStatusCode.OK,
                                                      Server                     = HTTPServiceName,
                                                      Date                       = Timestamp.Now,
                                                      AccessControlAllowOrigin   = "*",
                                                      AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                      AccessControlAllowHeaders  = new[] { "Authorization" },
                                                      ContentType                = HTTPContentType.JSON_UTF8,
                                                      Content                    = JSONArray.Create(
                                                                                       ChargeBoxes.Select(chargeBox => chargeBox.ToJSON())
                                                                                   ).ToUTF8Bytes(Newtonsoft.Json.Formatting.None),
                                                      Connection                 = "close"
                                                  }.AsImmutable);

                                          });

            #endregion


            #region ~/chargeBoxes/{chargeBoxId}

            HTTPBaseAPI.AddMethodCallback(HTTPHostname.Any,
                                      HTTPMethod.GET,
                                      URLPathPrefix + "chargeBoxes/{chargeBoxId}",
                                      HTTPContentType.JSON_UTF8,
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

                                          #region Check ChargeBoxId URL parameter

                                          if (!Request.ParseChargeBox(this,
                                                                      out ChargeBox_Id?          ChargeBoxId,
                                                                      out ChargeBox?             ChargeBox,
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
                                                  AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                  AccessControlAllowHeaders  = new[] { "Authorization" },
                                                  ContentType                = HTTPContentType.JSON_UTF8,
                                                  Content                    = ChargeBox.ToJSON().ToUTF8Bytes(Newtonsoft.Json.Formatting.None),
                                                  Connection                 = "close"
                                              }.AsImmutable);

                                      });

            #endregion


            #region ~/events

            #region HTML

            // --------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/events
            // --------------------------------------------------------------------
            HTTPBaseAPI.AddMethodCallback(HTTPHostname.Any,
                                      HTTPMethod.GET,
                                      URLPathPrefix + "events",
                                      HTTPContentType.HTML_UTF8,
                                      HTTPDelegate: Request => {

                                          #region Get HTTP user and its organizations

                                          //// Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                          //if (!TryGetHTTPUser(Request,
                                          //                    out User                   HTTPUser,
                                          //                    out HashSet<Organization>  HTTPOrganizations,
                                          //                    out HTTPResponse.Builder   Response,
                                          //                    Recursive:                 true))
                                          //{
                                          //    return Task.FromResult(Response.AsImmutable);
                                          //}

                                          #endregion

                                          return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         Server                     = HTTPServiceName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = new[] { "GET" },
                                                         AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("events.events.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                      });

            #endregion

            #endregion


        }

        #endregion


    }

}
