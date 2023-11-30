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
    /// OCPP Charging Station Management System WebAPI extensions.
    /// </summary>
    public static class CSMSWebAPIExtensions
    {

        #region ParseChargeBoxId(this HTTPRequest, OCPPWebAPI, out ChargeBoxId,                out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the charging station identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPPWebAPI">The OCPP WebAPI.</param>
        /// <param name="ChargeBoxId">The parsed unique charging station identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when charging station identification was found; false else.</returns>
        public static Boolean ParseChargeBoxId(this HTTPRequest           HTTPRequest,
                                               CSMSWebAPI                 OCPPWebAPI,
                                               out ChargingStation_Id?          ChargeBoxId,
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

            ChargeBoxId = ChargingStation_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!ChargeBoxId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPWebAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid charging station identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseChargeBox  (this HTTPRequest, OCPPWebAPI, out ChargeBoxId, out ChargeBox, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the charging station identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPPWebAPI">The OCPP WebAPI.</param>
        /// <param name="ChargeBoxId">The parsed unique charging station identification.</param>
        /// <param name="ChargeBox">The resolved charging station.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when charging station identification was found; false else.</returns>
        public static Boolean ParseChargeBox(this HTTPRequest           HTTPRequest,
                                             CSMSWebAPI                 OCPPWebAPI,
                                             out ChargingStation_Id?          ChargeBoxId,
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

            ChargeBoxId = ChargingStation_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!ChargeBoxId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPPWebAPI.HTTPServiceName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid charging station identification!"" }".ToUTF8Bytes(),
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
                    Content         = @"{ ""description"": ""Unknown charging station identification!"" }".ToUTF8Bytes(),
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
        public readonly HTTPPath                   DefaultURLPathPrefix      = HTTPPath.Parse("webapi");

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public const String                        DefaultHTTPRealm          = "Open Charging Cloud OCPP WebAPI";

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public const String                        HTTPRoot                  = "cloud.charging.open.protocols.OCPPv2_1.WebAPI.CSMS.HTTPRoot.";


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

        #endregion

        #region Properties

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


        private ConcurrentDictionary<CSMS_Id, ICSMSService> csmss = new();

        public IEnumerable<ICSMSService> CSMSs
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
        public CustomJObjectSerializerDelegate<Signature>?                                           CustomSignatureSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                                   { get; set; }
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
                   HTTPServerName ?? $"OCPP {Version.String} CSMS Web API",
                   URLPathPrefix,
                   BasePath,
                   HTMLTemplate)

        {

            this.HTTPRealm           = HTTPRealm;
            this.HTTPLogins          = HTTPLogins ?? Array.Empty<KeyValuePair<String, String>>();

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


        #region AttachCSMS       (CSMS)

        public void AttachCSMS(ICSMSService CSMS)
        {

            this.csmss.TryAdd(CSMS.Id, CSMS);

            #region Generic Text Messages

            #region OnTextMessageRequestReceived

            CSMS.OnJSONMessageRequestReceived += async (timestamp,
                                                        webSocketServer,
                                                        webSocketConnection,
                                                        eventTrackingId,
                                                        requestTimestamp,
                                                        requestMessage) =>

                await this.EventLog.SubmitEvent("OnTextMessageRequestReceived",
                                                JSONObject.Create(
                                                    new JProperty("timestamp",    timestamp.          ToIso8601()),
                                                    new JProperty("connection",   webSocketConnection.ToJSON()),
                                                    new JProperty("message",      requestMessage)
                                                ));

            #endregion

            #region OnTextMessageResponseSent

            CSMS.OnJSONMessageResponseSent += async (timestamp,
                                                     webSocketServer,
                                                     webSocketConnection,
                                                     eventTrackingId,
                                                     requestTimestamp,
                                                     jsonRequestMessage,
                                                     binaryRequestMessage,
                                                     responseTimestamp,
                                                     responseMessage) =>

                await this.EventLog.SubmitEvent("OnTextMessageResponseSent",
                                                JSONObject.Create(
                                                    new JProperty("timestamp",    timestamp.          ToIso8601()),
                                                    new JProperty("connection",   webSocketConnection.ToJSON()),
                                                    new JProperty("message",      responseMessage)
                                                ));

            #endregion

            #region OnTextErrorResponseSent

            CSMS.OnJSONErrorResponseSent += async (timestamp,
                                                   webSocketServer,
                                                   webSocketConnection,
                                                   eventTrackingId,
                                                   requestTimestamp,
                                                   jsonRequestMessage,
                                                   binaryRequestMessage,
                                                   responseTimestamp,
                                                   responseMessage) =>

                await this.EventLog.SubmitEvent("OnTextErrorResponseSent",
                                                JSONObject.Create(
                                                    new JProperty("timestamp",    timestamp.          ToIso8601()),
                                                    new JProperty("connection",   webSocketConnection.ToJSON()),
                                                    new JProperty("message",      responseMessage)
                                                ));

            #endregion


            #region OnTextMessageRequestSent

            CSMS.OnJSONMessageRequestSent += async (timestamp,
                                                    webSocketServer,
                                                    webSocketConnection,
                                                    eventTrackingId,
                                                    requestTimestamp,
                                                    requestMessage) =>

                await this.EventLog.SubmitEvent("OnTextMessageRequestSent",
                                                JSONObject.Create(
                                                    new JProperty("timestamp",    timestamp.          ToIso8601()),
                                                    new JProperty("connection",   webSocketConnection.ToJSON()),
                                                    new JProperty("message",      requestMessage)
                                                ));

            #endregion

            #region OnTextMessageResponseReceived

            CSMS.OnJSONMessageResponseReceived += async (timestamp,
                                                         webSocketServer,
                                                         webSocketConnection,
                                                         eventTrackingId,
                                                         requestTimestamp,
                                                         jsonRequestMessage,
                                                         binaryRequestMessage,
                                                         responseTimestamp,
                                                         responseMessage) =>

                await this.EventLog.SubmitEvent("OnTextMessageResponseReceived",
                                                JSONObject.Create(
                                                    new JProperty("timestamp",    timestamp.          ToIso8601()),
                                                    new JProperty("connection",   webSocketConnection.ToJSON()),
                                                    new JProperty("message",      responseMessage)
                                                ));

            #endregion

            #region OnTextErrorResponseReceived

            CSMS.OnJSONErrorResponseReceived += async (timestamp,
                                                       webSocketServer,
                                                       webSocketConnection,
                                                       eventTrackingId,
                                                       requestTimestamp,
                                                       jsonRequestMessage,
                                                       binaryRequestMessage,
                                                       responseTimestamp,
                                                       responseMessage) =>

                await this.EventLog.SubmitEvent("OnTextErrorResponseReceived",
                                                JSONObject.Create(
                                                    new JProperty("timestamp",    timestamp.          ToIso8601()),
                                                    new JProperty("connection",   webSocketConnection.ToJSON()),
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
                                                    new JProperty("timestamp",    timestamp.          ToIso8601()),
                                                    new JProperty("connection",   webSocketConnection.ToJSON()),
                                                    new JProperty("message",      requestMessage)  // BASE64 encoded string!
                                                ));

            #endregion

            #region OnBinaryMessageResponseSent

            CSMS.OnBinaryMessageResponseSent += async (timestamp,
                                                       webSocketServer,
                                                       webSocketConnection,
                                                       eventTrackingId,
                                                       requestTimestamp,
                                                       jsonRequestMessage,
                                                       binaryRequestMessage,
                                                       responseTimestamp,
                                                       responseMessage) =>

                await this.EventLog.SubmitEvent("OnBinaryMessageResponseSent",
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

            //    await this.EventLog.SubmitEvent("OnBinaryErrorResponseSent",
            //                                    JSONObject.Create(
            //                                        new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                                        new JProperty("connection",   webSocketConnection.ToJSON()),
            //                                        new JProperty("message",      responseMessage)  // BASE64 encoded string!
            //                                    ));

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
                                                    new JProperty("timestamp",    timestamp.          ToIso8601()),
                                                    new JProperty("connection",   webSocketConnection.ToJSON()),
                                                    new JProperty("message",      requestMessage)  // BASE64 encoded string!
                                                ));

            #endregion

            #region OnBinaryMessageResponseReceived

            CSMS.OnBinaryMessageResponseReceived += async (timestamp,
                                                           webSocketServer,
                                                           webSocketConnection,
                                                           eventTrackingId,
                                                           requestTimestamp,
                                                           jsonRequestMessage,
                                                           binaryRequestMessage,
                                                           responseTimestamp,
                                                           responseMessage) =>

                await this.EventLog.SubmitEvent("OnBinaryMessageResponseReceived",
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

            //    await this.EventLog.SubmitEvent("OnBinaryErrorResponseReceived",
            //                                    JSONObject.Create(
            //                                        new JProperty("timestamp",    timestamp.          ToIso8601()),
            //                                        new JProperty("connection",   webSocketConnection.ToJSON()),
            //                                        new JProperty("message",      responseMessage)  // BASE64 encoded string!
            //                                    ));

            #endregion

            #endregion


            #region CSMS <- Charging Station Messages

            #region OnBootNotification                      (-Request/-Response)

            CSMS.OnBootNotificationRequest += async (logTimestamp, sender, connection, request) =>
                await this.EventLog.SubmitEvent("OnBootNotificationRequest",
                                                request.ToAbstractJSON(connection,
                                                                       request.ToJSON(CustomBootNotificationRequestSerializer,
                                                                                      CustomChargingStationSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnBootNotificationResponse += async (logTimestamp, sender, connection, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnBootNotificationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomBootNotificationRequestSerializer,
                                                                                        CustomChargingStationSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomBootNotificationResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnFirmwareStatusNotification            (-Request/-Response)

            CSMS.OnFirmwareStatusNotificationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnFirmwareStatusNotificationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomFirmwareStatusNotificationRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnFirmwareStatusNotificationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnFirmwareStatusNotificationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomFirmwareStatusNotificationRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomFirmwareStatusNotificationResponseSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnPublishFirmwareStatusNotification     (-Request/-Response)

            CSMS.OnPublishFirmwareStatusNotificationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnPublishFirmwareStatusNotificationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomPublishFirmwareStatusNotificationRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnPublishFirmwareStatusNotificationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnPublishFirmwareStatusNotificationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomPublishFirmwareStatusNotificationRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomPublishFirmwareStatusNotificationResponseSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnHeartbeat                             (-Request/-Response)

            CSMS.OnHeartbeatRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnHeartbeatRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomHeartbeatRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnHeartbeatResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnHeartbeatResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomHeartbeatRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomHeartbeatResponseSerializer,
                                                                                        CustomSignatureSerializer,
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
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyEventResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyEventResponse",
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

            #region OnSecurityEventNotification             (-Request/-Response)

            CSMS.OnSecurityEventNotificationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnSecurityEventNotificationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomSecurityEventNotificationRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSecurityEventNotificationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSecurityEventNotificationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomSecurityEventNotificationRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomSecurityEventNotificationResponseSerializer,
                                                                                        CustomSignatureSerializer,
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
                                                                                      CustomSignatureSerializer,
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
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyReportResponseSerializer,
                                                                                        CustomSignatureSerializer,
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
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyMonitoringReportResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyMonitoringReportResponse",
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

            #region OnLogStatusNotification                 (-Request/-Response)

            CSMS.OnLogStatusNotificationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnLogStatusNotificationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomLogStatusNotificationRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnLogStatusNotificationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnLogStatusNotificationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomLogStatusNotificationRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomLogStatusNotificationResponseSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnIncomingDataTransfer                  (-Request/-Response)

            CSMS.OnIncomingDataTransferRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnIncomingDataTransferRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomDataTransferRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnIncomingDataTransferResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnIncomingDataTransferResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomDataTransferRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomDataTransferResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion


            #region OnSignCertificate                       (-Request/-Response)

            CSMS.OnSignCertificateRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnSignCertificateRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomSignCertificateRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSignCertificateResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSignCertificateResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomSignCertificateRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomSignCertificateResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGet15118EVCertificate                 (-Request/-Response)

            CSMS.OnGet15118EVCertificateRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGet15118EVCertificateRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGet15118EVCertificateRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGet15118EVCertificateResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGet15118EVCertificateResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGet15118EVCertificateRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGet15118EVCertificateResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetCertificateStatus                  (-Request/-Response)

            CSMS.OnGetCertificateStatusRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetCertificateStatusRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetCertificateStatusRequestSerializer,
                                                                                      CustomOCSPRequestDataSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetCertificateStatusResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetCertificateStatusResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetCertificateStatusRequestSerializer,
                                                                                        CustomOCSPRequestDataSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetCertificateStatusResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetCRL                                (-Request/-Response)

            CSMS.OnGetCRLRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetCRLRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetCRLRequestSerializer,
                                                                                      CustomCertificateHashDataSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetCRLResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetCRLResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetCRLRequestSerializer,
                                                                                        CustomCertificateHashDataSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetCRLResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion


            #region OnReservationStatusUpdate               (-Request/-Response)

            CSMS.OnReservationStatusUpdateRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnReservationStatusUpdateRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomReservationStatusUpdateRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnReservationStatusUpdateResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnReservationStatusUpdateResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomReservationStatusUpdateRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomReservationStatusUpdateResponseSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnAuthorize                             (-Request/-Response)

            CSMS.OnAuthorizeRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnAuthorizeRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomAuthorizeRequestSerializer,
                                                                                      CustomIdTokenSerializer,
                                                                                      CustomAdditionalInfoSerializer,
                                                                                      CustomOCSPRequestDataSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnAuthorizeResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnAuthorizeResponse",
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
                                                                                      CustomSignatureSerializer,
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
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyEVChargingNeedsResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
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
                                                                                      CustomSignatureSerializer,
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

            #region OnStatusNotification                    (-Request/-Response)

            CSMS.OnStatusNotificationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnStatusNotificationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomStatusNotificationRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnStatusNotificationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnStatusNotificationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomStatusNotificationRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomStatusNotificationResponseSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnMeterValues                           (-Request/-Response)

            CSMS.OnMeterValuesRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnMeterValuesRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomMeterValuesRequestSerializer,
                                                                                      CustomMeterValueSerializer,
                                                                                      CustomSampledValueSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnMeterValuesResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnMeterValuesResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomMeterValuesRequestSerializer,
                                                                                        CustomMeterValueSerializer,
                                                                                        CustomSampledValueSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomMeterValuesResponseSerializer,
                                                                                        CustomSignatureSerializer,
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

                                                                                      CustomSignatureSerializer,
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

                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyChargingLimitResponseSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnClearedChargingLimit                  (-Request/-Response)

            CSMS.OnClearedChargingLimitRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnClearedChargingLimitRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomClearedChargingLimitRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnClearedChargingLimitResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnClearedChargingLimitResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomClearedChargingLimitRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomClearedChargingLimitResponseSerializer,
                                                                                        CustomSignatureSerializer,
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

                                                                                      CustomSignatureSerializer,
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

                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomReportChargingProfilesResponseSerializer,
                                                                                        CustomSignatureSerializer,
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

                                                                                      CustomSignatureSerializer,
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

                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyEVChargingScheduleResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyPriorityCharging                (-Request/-Response)

            CSMS.OnNotifyPriorityChargingRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnNotifyPriorityChargingRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomNotifyPriorityChargingRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyPriorityChargingResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyPriorityChargingResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomNotifyPriorityChargingRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyPriorityChargingResponseSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnPullDynamicScheduleUpdate             (-Request/-Response)

            CSMS.OnPullDynamicScheduleUpdateRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnPullDynamicScheduleUpdateRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomPullDynamicScheduleUpdateRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnPullDynamicScheduleUpdateResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnPullDynamicScheduleUpdateResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomPullDynamicScheduleUpdateRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomPullDynamicScheduleUpdateResponseSerializer,
                                                                                        CustomSignatureSerializer,
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
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyDisplayMessagesResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyDisplayMessagesResponse",
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

            #region OnNotifyCustomerInformation             (-Request/-Response)

            CSMS.OnNotifyCustomerInformationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnNotifyCustomerInformationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomNotifyCustomerInformationRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyCustomerInformationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyCustomerInformationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomNotifyCustomerInformationRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyCustomerInformationResponseSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #endregion

            #region CSMS -> Charging Station Messages

            #region OnReset                                 (-Request/-Response)

            CSMS.OnResetRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnResetRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomResetRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnResetResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnResetResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomResetRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomResetResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnUpdateFirmware                        (-Request/-Response)

            CSMS.OnUpdateFirmwareRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnUpdateFirmwareRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomUpdateFirmwareRequestSerializer,
                                                                                      CustomFirmwareSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnUpdateFirmwareResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnUpdateFirmwareResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomUpdateFirmwareRequestSerializer,
                                                                                        CustomFirmwareSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomUpdateFirmwareResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnPublishFirmware                       (-Request/-Response)

            CSMS.OnPublishFirmwareRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnPublishFirmwareRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomPublishFirmwareRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnPublishFirmwareResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnPublishFirmwareResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomPublishFirmwareRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomPublishFirmwareResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnUnpublishFirmware                     (-Request/-Response)

            CSMS.OnUnpublishFirmwareRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnUnpublishFirmwareRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomUnpublishFirmwareRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnUnpublishFirmwareResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnUnpublishFirmwareResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomUnpublishFirmwareRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomUnpublishFirmwareResponseSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetBaseReport                         (-Request/-Response)

            CSMS.OnGetBaseReportRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetBaseReportRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetBaseReportRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetBaseReportResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetBaseReportResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetBaseReportRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetBaseReportResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
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
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetReportResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetReportResponse",
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

            #region OnGetLog                                (-Request/-Response)

            CSMS.OnGetLogRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetLogRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetLogRequestSerializer,
                                                                                      CustomLogParametersSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetLogResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetLogResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetLogRequestSerializer,
                                                                                        CustomLogParametersSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetLogResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
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
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSetVariablesResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSetVariablesResponse",
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

            #region OnGetVariables                          (-Request/-Response)

            CSMS.OnGetVariablesRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetVariablesRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetVariablesRequestSerializer,
                                                                                      CustomGetVariableDataSerializer,
                                                                                      CustomComponentSerializer,
                                                                                      CustomEVSESerializer,
                                                                                      CustomVariableSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetVariablesResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetVariablesResponse",
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

            #region OnSetMonitoringBase                     (-Request/-Response)

            CSMS.OnSetMonitoringBaseRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnSetMonitoringBaseRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomSetMonitoringBaseRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSetMonitoringBaseResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSetMonitoringBaseResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomSetMonitoringBaseRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomSetMonitoringBaseResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
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
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetMonitoringReportResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetMonitoringReportResponse",
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

            #region OnSetMonitoringLevel                    (-Request/-Response)

            CSMS.OnSetMonitoringLevelRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnSetMonitoringLevelRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomSetMonitoringLevelRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSetMonitoringLevelResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSetMonitoringLevelResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomSetMonitoringLevelRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomSetMonitoringLevelResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
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
                                                                                      CustomPeriodicEventStreamParametersSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSetVariableMonitoringResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSetVariableMonitoringResponse",
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

            #region OnClearVariableMonitoring               (-Request/-Response)

            CSMS.OnClearVariableMonitoringRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnClearVariableMonitoringRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomClearVariableMonitoringRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnClearVariableMonitoringResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnClearVariableMonitoringResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomClearVariableMonitoringRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomClearVariableMonitoringResponseSerializer,
                                                                                        CustomClearMonitoringResultSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnSetNetworkProfile                     (-Request/-Response)

            CSMS.OnSetNetworkProfileRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnSetNetworkProfileRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomSetNetworkProfileRequestSerializer,
                                                                                      CustomNetworkConnectionProfileSerializer,
                                                                                      CustomVPNConfigurationSerializer,
                                                                                      CustomAPNConfigurationSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSetNetworkProfileResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSetNetworkProfileResponse",
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

            #region OnChangeAvailability                    (-Request/-Response)

            CSMS.OnChangeAvailabilityRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnChangeAvailabilityRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomChangeAvailabilityRequestSerializer,
                                                                                      CustomEVSESerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnChangeAvailabilityResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnChangeAvailabilityResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomChangeAvailabilityRequestSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomChangeAvailabilityResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnTriggerMessage                        (-Request/-Response)

            CSMS.OnTriggerMessageRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnTriggerMessageRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomTriggerMessageRequestSerializer,
                                                                                      CustomEVSESerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnTriggerMessageResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnTriggerMessageResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomTriggerMessageRequestSerializer,
                                                                                        CustomEVSESerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomTriggerMessageResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnDataTransfer                          (-Request/-Response)

            CSMS.OnDataTransferRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnDataTransferRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomData2TransferRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnDataTransferResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnDataTransferResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomData2TransferRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomData2TransferResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion


            #region OnCertificateSigned                     (-Request/-Response)

            CSMS.OnCertificateSignedRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnCertificateSignedRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomCertificateSignedRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnCertificateSignedResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnCertificateSignedResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomCertificateSignedRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomCertificateSignedResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnInstallCertificate                    (-Request/-Response)

            CSMS.OnInstallCertificateRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnInstallCertificateRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomInstallCertificateRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnInstallCertificateResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnInstallCertificateResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomInstallCertificateRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomInstallCertificateResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetInstalledCertificateIds            (-Request/-Response)

            CSMS.OnGetInstalledCertificateIdsRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetInstalledCertificateIdsRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetInstalledCertificateIdsRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetInstalledCertificateIdsResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetInstalledCertificateIdsResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetInstalledCertificateIdsRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetInstalledCertificateIdsResponseSerializer,
                                                                                        CustomCertificateHashDataSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnDeleteCertificate                     (-Request/-Response)

            CSMS.OnDeleteCertificateRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnDeleteCertificateRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomDeleteCertificateRequestSerializer,
                                                                                      CustomCertificateHashDataSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnDeleteCertificateResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnDeleteCertificateResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomDeleteCertificateRequestSerializer,
                                                                                        CustomCertificateHashDataSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomDeleteCertificateResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyCRL                             (-Request/-Response)

            CSMS.OnNotifyCRLRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnNotifyCRLRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomNotifyCRLRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyCRLResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyCRLResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomNotifyCRLRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyCRLResponseSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion


            #region OnGetLocalListVersion                   (-Request/-Response)

            CSMS.OnGetLocalListVersionRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetLocalListVersionRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetLocalListVersionRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetLocalListVersionResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetLocalListVersionResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetLocalListVersionRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetLocalListVersionResponseSerializer,
                                                                                        CustomSignatureSerializer,
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
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSendLocalListResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSendLocalListResponse",
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

            #region OnClearCache                            (-Request/-Response)

            CSMS.OnClearCacheRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnClearCacheRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomClearCacheRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnClearCacheResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnClearCacheResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomClearCacheRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomClearCacheResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion


            #region OnReserveNow                            (-Request/-Response)

            CSMS.OnReserveNowRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnReserveNowRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomReserveNowRequestSerializer,
                                                                                      CustomIdTokenSerializer,
                                                                                      CustomAdditionalInfoSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnReserveNowResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnReserveNowResponse",
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

            #region OnCancelReservation                     (-Request/-Response)

            CSMS.OnCancelReservationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnCancelReservationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomCancelReservationRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnCancelReservationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnCancelReservationResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomCancelReservationRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomCancelReservationResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
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

                                                                                      CustomTransactionLimitsSerializer,

                                                                                      CustomSignatureSerializer,
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

                                                                                        CustomTransactionLimitsSerializer,

                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomRequestStartTransactionResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnRequestStopTransaction                (-Request/-Response)

            CSMS.OnRequestStopTransactionRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnRequestStopTransactionRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomRequestStopTransactionRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnRequestStopTransactionResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnRequestStopTransactionResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomRequestStopTransactionRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomRequestStopTransactionResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetTransactionStatus                  (-Request/-Response)

            CSMS.OnGetTransactionStatusRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetTransactionStatusRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetTransactionStatusRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetTransactionStatusResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetTransactionStatusResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetTransactionStatusRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetTransactionStatusResponseSerializer,
                                                                                        CustomSignatureSerializer,
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

                                                                                      CustomSignatureSerializer,
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

                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomSetChargingProfileResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetChargingProfiles                   (-Request/-Response)

            CSMS.OnGetChargingProfilesRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetChargingProfilesRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetChargingProfilesRequestSerializer,
                                                                                      CustomChargingProfileCriterionSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetChargingProfilesResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetChargingProfilesResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetChargingProfilesRequestSerializer,
                                                                                        CustomChargingProfileCriterionSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetChargingProfilesResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnClearChargingProfile                  (-Request/-Response)

            CSMS.OnClearChargingProfileRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnClearChargingProfileRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomClearChargingProfileRequestSerializer,
                                                                                      CustomClearChargingProfileSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnClearChargingProfileResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnClearChargingProfileResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomClearChargingProfileRequestSerializer,
                                                                                        CustomClearChargingProfileSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomClearChargingProfileResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnGetCompositeSchedule                  (-Request/-Response)

            CSMS.OnGetCompositeScheduleRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetCompositeScheduleRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetCompositeScheduleRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetCompositeScheduleResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetCompositeScheduleResponse",
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

            #region OnUpdateDynamicSchedule                 (-Request/-Response)

            CSMS.OnUpdateDynamicScheduleRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnUpdateDynamicScheduleRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomUpdateDynamicScheduleRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnUpdateDynamicScheduleResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnUpdateDynamicScheduleResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomUpdateDynamicScheduleRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomUpdateDynamicScheduleResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnNotifyAllowedEnergyTransfer           (-Request/-Response)

            CSMS.OnNotifyAllowedEnergyTransferRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnNotifyAllowedEnergyTransferRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomNotifyAllowedEnergyTransferRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnNotifyAllowedEnergyTransferResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnNotifyAllowedEnergyTransferResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomNotifyAllowedEnergyTransferRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomNotifyAllowedEnergyTransferResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnUsePriorityCharging                   (-Request/-Response)

            CSMS.OnUsePriorityChargingRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnUsePriorityChargingRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomUsePriorityChargingRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnUsePriorityChargingResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnUsePriorityChargingResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomUsePriorityChargingRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomUsePriorityChargingResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnUnlockConnector                       (-Request/-Response)

            CSMS.OnUnlockConnectorRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnUnlockConnectorRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomUnlockConnectorRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnUnlockConnectorResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnUnlockConnectorResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomUnlockConnectorRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomUnlockConnectorResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion


            #region OnAFRRSignal                            (-Request/-Response)

            CSMS.OnAFRRSignalRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnAFRRSignalRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomAFRRSignalRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnAFRRSignalResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnAFRRSignalResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomAFRRSignalRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomAFRRSignalResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
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
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnSetDisplayMessageResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnSetDisplayMessageResponse",
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

            #region OnGetDisplayMessages                    (-Request/-Response)

            CSMS.OnGetDisplayMessagesRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnGetDisplayMessagesRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomGetDisplayMessagesRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnGetDisplayMessagesResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnGetDisplayMessagesResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomGetDisplayMessagesRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomGetDisplayMessagesResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnClearDisplayMessage                   (-Request/-Response)

            CSMS.OnClearDisplayMessageRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnClearDisplayMessageRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomClearDisplayMessageRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnClearDisplayMessageResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnClearDisplayMessageResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomClearDisplayMessageRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomClearDisplayMessageResponseSerializer,
                                                                                        CustomStatusInfoSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnCostUpdated                           (-Request/-Response)

            CSMS.OnCostUpdatedRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnCostUpdatedRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomCostUpdatedRequestSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnCostUpdatedResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnCostUpdatedResponse",
                                                response.ToAbstractJSON(request. ToJSON(CustomCostUpdatedRequestSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer),
                                                                        response.ToJSON(CustomCostUpdatedResponseSerializer,
                                                                                        CustomSignatureSerializer,
                                                                                        CustomCustomDataSerializer)));

            #endregion

            #region OnCustomerInformation                   (-Request/-Response)

            CSMS.OnCustomerInformationRequest += async (logTimestamp, sender, request) =>
                await this.EventLog.SubmitEvent("OnCustomerInformationRequest",
                                                request.ToAbstractJSON(request.ToJSON(CustomCustomerInformationRequestSerializer,
                                                                                      CustomIdTokenSerializer,
                                                                                      CustomAdditionalInfoSerializer,
                                                                                      CustomCertificateHashDataSerializer,
                                                                                      CustomSignatureSerializer,
                                                                                      CustomCustomDataSerializer)));

            CSMS.OnCustomerInformationResponse += async (logTimestamp, sender, request, response, runtime) =>
                await this.EventLog.SubmitEvent("OnCustomerInformationResponse",
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

            #endregion


            // Firmware API download messages
            // Logdata API upload messages
            // Diagnostics API upload messages

        }

        #endregion

        #region AttachCSMSChannel(CSMSChannel)

        public void AttachCSMSChannel(ICSMSChannel CSMSChannel)
        {

            #region WebSocket connections

            #region OnNewTCPConnection

            CSMSChannel.OnNewTCPConnection += async (logTimestamp,
                                                     sender,
                                                     connection,
                                                     eventTrackingId,
                                                     cancellationToken) =>

                await this.EventLog.SubmitEvent("OnNewTCPConnection",
                                                JSONObject.Create(
                                                    new JProperty("timestamp",    logTimestamp.ToIso8601()),
                                                    new JProperty("connection",   connection.  ToJSON())
                                                ));

            #endregion

            #region OnNewWebSocketConnection

            CSMSChannel.OnNewWebSocketConnection += async (logTimestamp,
                                                           sender,
                                                           connection,
                                                           eventTrackingId,
                                                           cancellationToken) =>

                await this.EventLog.SubmitEvent("OnNewWebSocketConnection",
                                                JSONObject.Create(
                                                    new JProperty("timestamp",    logTimestamp.ToIso8601()),
                                                    new JProperty("connection",   connection.  ToJSON())
                                                ));

            #endregion

            #region OnCloseMessageReceived

            CSMSChannel.OnCloseMessageReceived += async (logTimestamp,
                                                         sender,
                                                         connection,
                                                         eventTrackingId,
                                                         statusCode,
                                                         reason) =>

                await this.EventLog.SubmitEvent("OnCloseMessageReceived",
                                                JSONObject.Create(
                                                    new JProperty("timestamp",    logTimestamp.ToIso8601()),
                                                    new JProperty("connection",   connection.  ToJSON()),
                                                    new JProperty("statusCode",   statusCode.  ToString()),
                                                    new JProperty("reason",       reason)
                                                ));

            #endregion

            #region OnTCPConnectionClosed

            CSMSChannel.OnTCPConnectionClosed += async (logTimestamp,
                                                        sender,
                                                        connection,
                                                        reason,
                                                        eventTrackingId) =>

                await this.EventLog.SubmitEvent("OnTCPConnectionClosed",
                                                JSONObject.Create(
                                                    new JProperty("timestamp",    logTimestamp.ToIso8601()),
                                                    new JProperty("connection",   connection.  ToJSON()),
                                                    new JProperty("reason",       reason)
                                                ));

            #endregion

            // Failed (Charging Station) Authentication

            // (Generic) Error Handling

            #endregion


            // HTTP-SSEs: CSMS -> ChargePoint

            #region OnReset                       (-Request/-Response)

            CSMSChannel.OnResetRequest += async (logTimestamp,
                                                 sender,
                                                 request) =>

                await this.EventLog.SubmitEvent("OnResetRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
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
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnUpdateFirmware              (-Request/-Response)

            CSMSChannel.OnUpdateFirmwareRequest += async (logTimestamp,
                                                          sender,
                                                          request) =>

                await this.EventLog.SubmitEvent("OnUpdateFirmwareRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
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
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnPublishFirmware             (-Request/-Response)

            CSMSChannel.OnPublishFirmwareRequest += async (logTimestamp,
                                                           sender,
                                                           request) =>

                await this.EventLog.SubmitEvent("OnPublishFirmwareRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnPublishFirmwareResponse += async (logTimestamp,
                                                            sender,
                                                            request,
                                                            response,
                                                            runtime) =>

                await this.EventLog.SubmitEvent("OnPublishFirmwareResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnUnpublishFirmware           (-Request/-Response)

            CSMSChannel.OnUnpublishFirmwareRequest += async (logTimestamp,
                                                             sender,
                                                             request) =>

                await this.EventLog.SubmitEvent("OnUnpublishFirmwareRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnUnpublishFirmwareResponse += async (logTimestamp,
                                                              sender,
                                                              request,
                                                              response,
                                                              runtime) =>

                await this.EventLog.SubmitEvent("OnUnpublishFirmwareResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnGetBaseReport               (-Request/-Response)

            CSMSChannel.OnGetBaseReportRequest += async (logTimestamp,
                                                         sender,
                                                         request) =>

                await this.EventLog.SubmitEvent("OnGetBaseReportRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnGetBaseReportResponse += async (logTimestamp,
                                                          sender,
                                                          request,
                                                          response,
                                                          runtime) =>

                await this.EventLog.SubmitEvent("OnGetBaseReportResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnGetReport                   (-Request/-Response)

            CSMSChannel.OnGetReportRequest += async (logTimestamp,
                                                     sender,
                                                     request) =>

                await this.EventLog.SubmitEvent("OnGetReportRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnGetReportResponse += async (logTimestamp,
                                                      sender,
                                                      request,
                                                      response,
                                                      runtime) =>

                await this.EventLog.SubmitEvent("OnGetReportResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnGetLog                      (-Request/-Response)

            CSMSChannel.OnGetLogRequest += async (logTimestamp,
                                                  sender,
                                                  request) =>

                await this.EventLog.SubmitEvent("OnGetLogRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnGetLogResponse += async (logTimestamp,
                                                   sender,
                                                   request,
                                                   response,
                                                   runtime) =>

                await this.EventLog.SubmitEvent("OnGetLogResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnSetVariables                (-Request/-Response)

            CSMSChannel.OnSetVariablesRequest += async (logTimestamp,
                                                        sender,
                                                        request) =>

                await this.EventLog.SubmitEvent("OnSetVariablesRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnSetVariablesResponse += async (logTimestamp,
                                                         sender,
                                                         request,
                                                         response,
                                                         runtime) =>

                await this.EventLog.SubmitEvent("OnSetVariablesResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnGetVariables                (-Request/-Response)

            CSMSChannel.OnGetVariablesRequest += async (logTimestamp,
                                                        sender,
                                                        request) =>

                await this.EventLog.SubmitEvent("OnGetVariablesRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnGetVariablesResponse += async (logTimestamp,
                                                         sender,
                                                         request,
                                                         response,
                                                         runtime) =>

                await this.EventLog.SubmitEvent("OnGetVariablesResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnSetMonitoringBase           (-Request/-Response)

            CSMSChannel.OnSetMonitoringBaseRequest += async (logTimestamp,
                                                             sender,
                                                             request) =>

                await this.EventLog.SubmitEvent("OnSetMonitoringBaseRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnSetMonitoringBaseResponse += async (logTimestamp,
                                                              sender,
                                                              request,
                                                              response,
                                                              runtime) =>

                await this.EventLog.SubmitEvent("OnSetMonitoringBaseResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnGetMonitoringReport         (-Request/-Response)

            CSMSChannel.OnGetMonitoringReportRequest += async (logTimestamp,
                                                               sender,
                                                               request) =>

                await this.EventLog.SubmitEvent("OnGetMonitoringReportRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnGetMonitoringReportResponse += async (logTimestamp,
                                                                sender,
                                                                request,
                                                                response,
                                                                runtime) =>

                await this.EventLog.SubmitEvent("OnGetMonitoringReportResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnSetMonitoringLevel          (-Request/-Response)

            CSMSChannel.OnSetMonitoringLevelRequest += async (logTimestamp,
                                                              sender,
                                                              request) =>

                await this.EventLog.SubmitEvent("OnSetMonitoringLevelRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnSetMonitoringLevelResponse += async (logTimestamp,
                                                               sender,
                                                               request,
                                                               response,
                                                               runtime) =>

                await this.EventLog.SubmitEvent("OnSetMonitoringLevelResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnSetVariableMonitoring       (-Request/-Response)

            CSMSChannel.OnSetVariableMonitoringRequest += async (logTimestamp,
                                                                 sender,
                                                                 request) =>

                await this.EventLog.SubmitEvent("OnSetVariableMonitoringRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnSetVariableMonitoringResponse += async (logTimestamp,
                                                                  sender,
                                                                  request,
                                                                  response,
                                                                  runtime) =>

                await this.EventLog.SubmitEvent("OnSetVariableMonitoringResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnClearVariableMonitoring     (-Request/-Response)

            CSMSChannel.OnClearVariableMonitoringRequest += async (logTimestamp,
                                                                   sender,
                                                                   request) =>

                await this.EventLog.SubmitEvent("OnClearVariableMonitoringRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnClearVariableMonitoringResponse += async (logTimestamp,
                                                                    sender,
                                                                    request,
                                                                    response,
                                                                    runtime) =>

                await this.EventLog.SubmitEvent("OnClearVariableMonitoringResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnSetNetworkProfile           (-Request/-Response)

            CSMSChannel.OnSetNetworkProfileRequest += async (logTimestamp,
                                                             sender,
                                                             request) =>

                await this.EventLog.SubmitEvent("OnSetNetworkProfileRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnSetNetworkProfileResponse += async (logTimestamp,
                                                              sender,
                                                              request,
                                                              response,
                                                              runtime) =>

                await this.EventLog.SubmitEvent("OnSetNetworkProfileResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnChangeAvailability          (-Request/-Response)

            CSMSChannel.OnChangeAvailabilityRequest += async (logTimestamp,
                                                              sender,
                                                              request) =>

                await this.EventLog.SubmitEvent("OnChangeAvailabilityRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
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
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnTriggerMessage              (-Request/-Response)

            CSMSChannel.OnTriggerMessageRequest += async (logTimestamp,
                                                          sender,
                                                          request) =>

                await this.EventLog.SubmitEvent("OnTriggerMessageRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
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
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnDataTransfer                (-Request/-Response)

            CSMSChannel.OnDataTransferRequest += async (logTimestamp,
                                                        sender,
                                                        request) =>

                await this.EventLog.SubmitEvent("OnDataTransferRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
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
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion


            #region OnCertificateSignedRequest    (-Request/-Response)

            CSMSChannel.OnCertificateSignedRequest += async (logTimestamp,
                                                             sender,
                                                             request) =>

                await this.EventLog.SubmitEvent("OnCertificateSignedRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnCertificateSignedResponse += async (logTimestamp,
                                                              sender,
                                                              request,
                                                              response,
                                                              runtime) =>

                await this.EventLog.SubmitEvent("OnCertificateSignedResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnInstallCertificate          (-Request/-Response)

            CSMSChannel.OnInstallCertificateRequest += async (logTimestamp,
                                                              sender,
                                                              request) =>

                await this.EventLog.SubmitEvent("OnInstallCertificateRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnInstallCertificateResponse += async (logTimestamp,
                                                               sender,
                                                               request,
                                                               response,
                                                               runtime) =>

                await this.EventLog.SubmitEvent("OnInstallCertificateResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnGetInstalledCertificateIds  (-Request/-Response)

            CSMSChannel.OnGetInstalledCertificateIdsRequest += async (logTimestamp,
                                                                      sender,
                                                                      request) =>

                await this.EventLog.SubmitEvent("OnGetInstalledCertificateIdsRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnGetInstalledCertificateIdsResponse += async (logTimestamp,
                                                                       sender,
                                                                       request,
                                                                       response,
                                                                       runtime) =>

                await this.EventLog.SubmitEvent("OnGetInstalledCertificateIdsResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnDeleteCertificate           (-Request/-Response)

            CSMSChannel.OnDeleteCertificateRequest += async (logTimestamp,
                                                             sender,
                                                             request) =>

                await this.EventLog.SubmitEvent("OnDeleteCertificateRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnDeleteCertificateResponse += async (logTimestamp,
                                                              sender,
                                                              request,
                                                              response,
                                                              runtime) =>

                await this.EventLog.SubmitEvent("OnDeleteCertificateResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnNotifyCRLRequest            (-Request/-Response)

            CSMSChannel.OnNotifyCRLRequest += async (logTimestamp,
                                                     sender,
                                                     request) =>

                await this.EventLog.SubmitEvent("OnNotifyCRLRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnNotifyCRLResponse += async (logTimestamp,
                                                      sender,
                                                      request,
                                                      response,
                                                      runtime) =>

                await this.EventLog.SubmitEvent("OnNotifyCRLResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion


            #region OnGetLocalListVersion         (-Request/-Response)

            CSMSChannel.OnGetLocalListVersionRequest += async (logTimestamp,
                                                               sender,
                                                               request) =>

                await this.EventLog.SubmitEvent("OnGetLocalListVersionRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
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
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnSendLocalList               (-Request/-Response)

            CSMSChannel.OnSendLocalListRequest += async (logTimestamp,
                                                         sender,
                                                         request) =>

                await this.EventLog.SubmitEvent("OnSendLocalListRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
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
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnClearCache                  (-Request/-Response)

            CSMSChannel.OnClearCacheRequest += async (logTimestamp,
                                                      sender,
                                                      request) =>

                await this.EventLog.SubmitEvent("OnClearCacheRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
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
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion


            #region OnReserveNow                  (-Request/-Response)

            CSMSChannel.OnReserveNowRequest += async (logTimestamp,
                                                      sender,
                                                      request) =>

                await this.EventLog.SubmitEvent("OnReserveNowRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
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
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnCancelReservation           (-Request/-Response)

            CSMSChannel.OnCancelReservationRequest += async (logTimestamp,
                                                             sender,
                                                             request) =>

                await this.EventLog.SubmitEvent("OnCancelReservationRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
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
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnRequestStartTransaction     (-Request/-Response)

            CSMSChannel.OnRequestStartTransactionRequest += async (logTimestamp,
                                                                   sender,
                                                                   request) =>

                await this.EventLog.SubmitEvent("OnRequestStartTransactionRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnRequestStartTransactionResponse += async (logTimestamp,
                                                                    sender,
                                                                    request,
                                                                    response,
                                                                    runtime) =>

                await this.EventLog.SubmitEvent("OnRequestStartTransactionResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnRequestStopTransaction      (-Request/-Response)

            CSMSChannel.OnRequestStopTransactionRequest += async (logTimestamp,
                                                                  sender,
                                                                  request) =>

                await this.EventLog.SubmitEvent("OnRequestStopTransactionRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnRequestStopTransactionResponse += async (logTimestamp,
                                                                   sender,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                await this.EventLog.SubmitEvent("OnRequestStopTransactionResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnGetTransactionStatus        (-Request/-Response)

            CSMSChannel.OnGetTransactionStatusRequest += async (logTimestamp,
                                                                sender,
                                                                request) =>

                await this.EventLog.SubmitEvent("OnGetTransactionStatusRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnGetTransactionStatusResponse += async (logTimestamp,
                                                                 sender,
                                                                 request,
                                                                 response,
                                                                 runtime) =>

                await this.EventLog.SubmitEvent("OnGetTransactionStatusResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnSetChargingProfile          (-Request/-Response)

            CSMSChannel.OnSetChargingProfileRequest += async (logTimestamp,
                                                              sender,
                                                              request) =>

                await this.EventLog.SubmitEvent("OnSetChargingProfileRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
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
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnGetChargingProfiles         (-Request/-Response)

            CSMSChannel.OnGetChargingProfilesRequest += async (logTimestamp,
                                                               sender,
                                                               request) =>

                await this.EventLog.SubmitEvent("OnGetChargingProfilesRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnGetChargingProfilesResponse += async (logTimestamp,
                                                                sender,
                                                                request,
                                                                response,
                                                                runtime) =>

                await this.EventLog.SubmitEvent("OnGetChargingProfilesResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnClearChargingProfile        (-Request/-Response)

            CSMSChannel.OnClearChargingProfileRequest += async (logTimestamp,
                                                                sender,
                                                                request) =>

                await this.EventLog.SubmitEvent("OnClearChargingProfileRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
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
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnGetCompositeSchedule        (-Request/-Response)

            CSMSChannel.OnGetCompositeScheduleRequest += async (logTimestamp,
                                                                sender,
                                                                request) =>

                await this.EventLog.SubmitEvent("OnGetCompositeScheduleRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
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
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnUpdateDynamicSchedule       (-Request/-Response)

            CSMSChannel.OnUpdateDynamicScheduleRequest += async (logTimestamp,
                                                                 sender,
                                                                 request) =>

                await this.EventLog.SubmitEvent("OnUpdateDynamicScheduleRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnUpdateDynamicScheduleResponse += async (logTimestamp,
                                                                  sender,
                                                                  request,
                                                                  response,
                                                                  runtime) =>

                await this.EventLog.SubmitEvent("OnUpdateDynamicScheduleResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnNotifyAllowedEnergyTransfer (-Request/-Response)

            CSMSChannel.OnNotifyAllowedEnergyTransferRequest += async (logTimestamp,
                                                                       sender,
                                                                       request) =>

                await this.EventLog.SubmitEvent("OnNotifyAllowedEnergyTransferRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnNotifyAllowedEnergyTransferResponse += async (logTimestamp,
                                                                        sender,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                await this.EventLog.SubmitEvent("OnNotifyAllowedEnergyTransferResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnUsePriorityCharging         (-Request/-Response)

            CSMSChannel.OnUsePriorityChargingRequest += async (logTimestamp,
                                                               sender,
                                                               request) =>

                await this.EventLog.SubmitEvent("OnUsePriorityChargingRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnUsePriorityChargingResponse += async (logTimestamp,
                                                                sender,
                                                                request,
                                                                response,
                                                                runtime) =>

                await this.EventLog.SubmitEvent("OnUsePriorityChargingResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnUnlockConnector             (-Request/-Response)

            CSMSChannel.OnUnlockConnectorRequest += async (logTimestamp,
                                                           sender,
                                                           request) =>

                await this.EventLog.SubmitEvent("OnUnlockConnectorRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
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
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion


            #region OnSendAFRRSignal              (-Request/-Response)

            CSMSChannel.OnAFRRSignalRequest += async (logTimestamp,
                                                      sender,
                                                      request) =>

                await this.EventLog.SubmitEvent("OnAFRRSignalRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnAFRRSignalResponse += async (logTimestamp,
                                                       sender,
                                                       request,
                                                       response,
                                                       runtime) =>

                await this.EventLog.SubmitEvent("OnAFRRSignalResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion


            #region OnSetDisplayMessage           (-Request/-Response)

            CSMSChannel.OnSetDisplayMessageRequest += async (logTimestamp,
                                                             sender,
                                                             request) =>

                await this.EventLog.SubmitEvent("OnSetDisplayMessageRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnSetDisplayMessageResponse += async (logTimestamp,
                                                              sender,
                                                              request,
                                                              response,
                                                              runtime) =>

                await this.EventLog.SubmitEvent("OnSetDisplayMessageResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnGetDisplayMessages          (-Request/-Response)

            CSMSChannel.OnGetDisplayMessagesRequest += async (logTimestamp,
                                                              sender,
                                                              request) =>

                await this.EventLog.SubmitEvent("OnGetDisplayMessagesRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnGetDisplayMessagesResponse += async (logTimestamp,
                                                               sender,
                                                               request,
                                                               response,
                                                               runtime) =>

                await this.EventLog.SubmitEvent("OnGetDisplayMessagesResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnClearDisplayMessage         (-Request/-Response)

            CSMSChannel.OnClearDisplayMessageRequest += async (logTimestamp,
                                                               sender,
                                                               request) =>

                await this.EventLog.SubmitEvent("OnClearDisplayMessageRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnClearDisplayMessageResponse += async (logTimestamp,
                                                                sender,
                                                                request,
                                                                response,
                                                                runtime) =>

                await this.EventLog.SubmitEvent("OnClearDisplayMessageResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnCostUpdated                 (-Request/-Response)

            CSMSChannel.OnCostUpdatedRequest += async (logTimestamp,
                                                       sender,
                                                       request) =>

                await this.EventLog.SubmitEvent("OnCostUpdatedRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnCostUpdatedResponse += async (logTimestamp,
                                                        sender,
                                                        request,
                                                        response,
                                                        runtime) =>

                await this.EventLog.SubmitEvent("OnCostUpdatedResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnCustomerInformation         (-Request/-Response)

            CSMSChannel.OnCustomerInformationRequest += async (logTimestamp,
                                                               sender,
                                                               request) =>

                await this.EventLog.SubmitEvent("OnCustomerInformationRequest",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            CSMSChannel.OnCustomerInformationResponse += async (logTimestamp,
                                                                sender,
                                                                request,
                                                                response,
                                                                runtime) =>

                await this.EventLog.SubmitEvent("OnCustomerInformationResponse",
                                                new JObject(
                                                    new JProperty("timestamp",        logTimestamp.           ToIso8601()),
                                                    new JProperty("chargeBoxId",      request.ChargingStationId.    ToString()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString()),
                                                    new JProperty("request",          request.                ToJSON()),
                                                    new JProperty("response",         response.               ToJSON()),
                                                    new JProperty("runtime",          runtime.TotalMilliseconds)
                                                ));

            #endregion


        }

        #endregion


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

                if (request.Path.StartsWith(URLPathPrefix + "/js")        ||
                    request.Path.StartsWith(URLPathPrefix + "/events")    ||
                    request.Path.StartsWith(URLPathPrefix + "/chargeBox") ||
                    request.Path.StartsWith(URLPathPrefix + "/chargeBoxes"))
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

            HTTPBaseAPI.AddMethodCallback(
                HTTPHostname.Any,
                HTTPMethod.GET,
                new HTTPPath[] {
                    HTTPPath.Parse("/index.html"),
                    HTTPPath.Parse("/"),
                    HTTPPath.Parse("/{FileName}")
                },
                //HTTPContentType.HTML_UTF8,
                HTTPDelegate: Request => {

                    var filePath = (Request.ParsedURLParameters is not null && Request.ParsedURLParameters.Length > 0)
                                       ? Request.ParsedURLParameters.Last().Replace("/", ".")
                                       : "index.html";

                    if (filePath.EndsWith(".",      StringComparison.Ordinal))
                        filePath += "index.shtml";


                    if (filePath.EndsWith(".shtml", StringComparison.Ordinal))
                    {

                        var file = MixWithHTMLTemplate(filePath);

                        if (file.IsNullOrEmpty())
                            return Task.FromResult(
                                new HTTPResponse.Builder(Request) {
                                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                                    Server          = HTTPServiceName,
                                    Date            = Timestamp.Now,
                                    CacheControl    = "public, max-age=300",
                                    Connection      = "close"
                                }.AsImmutable);

                        else
                            return Task.FromResult(
                                new HTTPResponse.Builder(Request) {
                                    HTTPStatusCode  = HTTPStatusCode.OK,
                                    ContentType     = HTTPContentType.HTML_UTF8,
                                    Content         = file.ToUTF8Bytes(),
                                    Connection      = "close"
                                }.AsImmutable);

                    }

                    else
                    {

                        var fileStream = GetResourceMemoryStream(filePath);//this.GetType().Assembly.GetManifestResourceStream(HTTPRoot + "" + FilePath);

                        #region File not found!

                        if (fileStream is null)
                            return Task.FromResult(
                                new HTTPResponse.Builder(Request) {
                                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                                    Server          = HTTPServiceName,
                                    Date            = Timestamp.Now,
                                    CacheControl    = "public, max-age=300",
                                    Connection      = "close"
                                }.AsImmutable);

                        #endregion

                        #region Choose HTTP Content Type based on the file name extension...

                        var fileName             = filePath[(filePath.LastIndexOf("/") + 1)..];
                        var responseContentType  = fileName.Remove(0, fileName.LastIndexOf(".") + 1) switch {
                                                       "htm"   => HTTPContentType.HTML_UTF8,
                                                       "html"  => HTTPContentType.HTML_UTF8,
                                                       "css"   => HTTPContentType.CSS_UTF8,
                                                       "gif"   => HTTPContentType.GIF,
                                                       "jpg"   => HTTPContentType.JPEG,
                                                       "jpeg"  => HTTPContentType.JPEG,
                                                       "svg"   => HTTPContentType.SVG,
                                                       "png"   => HTTPContentType.PNG,
                                                       "ico"   => HTTPContentType.ICO,
                                                       "swf"   => HTTPContentType.SWF,
                                                       "js"    => HTTPContentType.JAVASCRIPT_UTF8,
                                                       "txt"   => HTTPContentType.TEXT_UTF8,
                                                       "xml"   => HTTPContentType.XMLTEXT_UTF8,
                                                       _       => HTTPContentType.OCTETSTREAM,
                                                   };

                        #endregion

                        #region Create HTTP Response

                        return Task.FromResult(
                            new HTTPResponse.Builder(Request) {
                                HTTPStatusCode  = HTTPStatusCode.OK,
                                Server          = HTTPServiceName,
                                Date            = Timestamp.Now,
                                ContentType     = responseContentType,
                                Content         = fileStream.ToArray(),
//                                CacheControl    = "public, max-age=300",
                                //Expires          = "Mon, 25 Jun 2015 21:31:12 GMT",
//                                KeepAlive       = new KeepAliveType(TimeSpan.FromMinutes(5), 500),
//                                Connection      = "Keep-Alive",
                                Connection      = "close"
                            }.AsImmutable);

                        #endregion

                    }

                });

            #endregion

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
                                                                      out ChargingStation_Id?          ChargeBoxId,
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
                                                         Content                    = MixWithHTMLTemplate("events.index.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                      });

            #endregion

            #endregion


        }

        #endregion


        #region TryGetChargeBox(ChargeBoxId, out ChargeBox)

        public Boolean TryGetChargeBox(ChargingStation_Id ChargeBoxId, out ChargeBox? ChargeBox)
        {

            foreach (var csms in csmss.Values)
            {
                if (csms.TryGetChargeBox(ChargeBoxId, out ChargeBox))
                    return true;
            }

            ChargeBox = null;
            return false;

        }

        #endregion


    }

}
