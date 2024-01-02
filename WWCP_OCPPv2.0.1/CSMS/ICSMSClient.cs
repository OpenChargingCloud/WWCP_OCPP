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

using cloud.charging.open.protocols.OCPPv2_0_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1.CSMS
{

    /// <summary>
    /// The common interface of all CSMS clients.
    /// </summary>
    public interface ICSMSClient : ICSMSClientEvents
    {

        ChargeBox_Id                            ChargeBoxIdentity    { get; }
        String                                  From                 { get; }
        String                                  To                   { get; }

        //     CSMSSOAPClient.CSClientLogger  Logger               { get; }


        #region Reset

        /// <summary>
        /// Reset the given charge box.
        /// </summary>
        /// <param name="Request">A reset request.</param>
        Task<ResetResponse> Reset(ResetRequest Request);

        #endregion

        #region UpdateFirmware

        /// <summary>
        /// Initiate a firmware download from the given location at the given charge box.
        /// </summary>
        /// <param name="Request">An update firmware request.</param>
        Task<UpdateFirmwareResponse> UpdateFirmware(UpdateFirmwareRequest Request);

        #endregion

        #region PublishFirmware

        /// <summary>
        /// Publish a firmware.
        /// </summary>
        /// <param name="Request">A publish firmware request.</param>
        Task<PublishFirmwareResponse> PublishFirmware(PublishFirmwareRequest Request);

        #endregion

        #region UnpublishFirmware

        /// <summary>
        /// Unpublish a firmware.
        /// </summary>
        /// <param name="Request">An unpublish firmware request.</param>
        Task<UnpublishFirmwareResponse> UnpublishFirmware(UnpublishFirmwareRequest Request);

        #endregion

        #region GetBaseReport

        /// <summary>
        /// Get a base report.
        /// </summary>
        /// <param name="Request">A get base report request.</param>
        Task<GetBaseReportResponse> GetBaseReport(GetBaseReportRequest Request);

        #endregion

        #region GetReport

        /// <summary>
        /// Get a report.
        /// </summary>
        /// <param name="Request">A get report request.</param>
        Task<GetReportResponse> GetReport(GetReportRequest Request);

        #endregion

        #region GetLog

        /// <summary>
        /// Retrieve log files from the charging station.
        /// </summary>
        /// <param name="Request">A get log request.</param>
        Task<GetLogResponse> GetLog(GetLogRequest Request);

        #endregion

        #region SetVariables

        /// <summary>
        /// Set variables.
        /// </summary>
        /// <param name="Request">A set variables request.</param>
        Task<SetVariablesResponse> SetVariables(SetVariablesRequest Request);

        #endregion

        #region GetVariables

        /// <summary>
        /// Get all variables.
        /// </summary>
        /// <param name="Request">A get variables request.</param>
        Task<GetVariablesResponse> GetVariables(GetVariablesRequest Request);

        #endregion

        #region SetMonitoringBase

        /// <summary>
        /// Set the monitoring base.
        /// </summary>
        /// <param name="Request">A set monitoring base request.</param>
        Task<SetMonitoringBaseResponse> SetMonitoringBase(SetMonitoringBaseRequest Request);

        #endregion

        #region GetMonitoringReport

        /// <summary>
        /// Get a monitoring report.
        /// </summary>
        /// <param name="Request">A get monitoring report request.</param>
        Task<GetMonitoringReportResponse> GetMonitoringReport(GetMonitoringReportRequest Request);

        #endregion

        #region SetMonitoringLevel

        /// <summary>
        /// Set the monitoring level.
        /// </summary>
        /// <param name="Request">A set monitoring level request.</param>
        Task<SetMonitoringLevelResponse> SetMonitoringLevel(SetMonitoringLevelRequest Request);

        #endregion

        #region SetVariableMonitoring

        /// <summary>
        /// Set a variable monitoring.
        /// </summary>
        /// <param name="Request">A set variable monitoring request.</param>
        Task<SetVariableMonitoringResponse> SetVariableMonitoring(SetVariableMonitoringRequest Request);

        #endregion

        #region ClearVariableMonitoring

        /// <summary>
        /// Remove the given variable monitoring.
        /// </summary>
        /// <param name="Request">A clear variable monitoring request.</param>
        Task<ClearVariableMonitoringResponse> ClearVariableMonitoring(ClearVariableMonitoringRequest Request);

        #endregion

        #region SetNetworkProfile

        /// <summary>
        /// Set the network profile.
        /// </summary>
        /// <param name="Request">A set network profile request.</param>
        Task<SetNetworkProfileResponse> SetNetworkProfile(SetNetworkProfileRequest Request);

        #endregion

        #region ChangeAvailability

        /// <summary>
        /// Change the availability of the given charging station or EVSE.
        /// </summary>
        /// <param name="Request">A change availability request.</param>
        Task<ChangeAvailabilityResponse> ChangeAvailability(ChangeAvailabilityRequest Request);

        #endregion

        #region TriggerMessage

        /// <summary>
        /// Create a trigger for the given message at the given charging station or EVSE.
        /// </summary>
        /// <param name="Request">A trigger message request.</param>
        Task<TriggerMessageResponse> TriggerMessage(TriggerMessageRequest Request);

        #endregion

        #region TransferData

        /// <summary>
        /// Send the given vendor-specific data.
        /// </summary>
        /// <param name="Request">A data transfer request.</param>
        Task<CS.DataTransferResponse> TransferData(DataTransferRequest Request);

        #endregion


        #region SendSignedCertificate

        /// <summary>
        /// Send the signed certificate to the charging station.
        /// </summary>
        /// <param name="Request">A certificate signed request.</param>
        Task<CertificateSignedResponse> SendSignedCertificate(CertificateSignedRequest Request);

        #endregion

        #region InstallCertificate

        /// <summary>
        /// Install the given certificate within the charging station.
        /// </summary>
        /// <param name="Request">An install certificate request.</param>
        Task<InstallCertificateResponse> InstallCertificate(InstallCertificateRequest Request);

        #endregion

        #region GetInstalledCertificateIds

        /// <summary>
        /// Retrieve a list of all installed certificates within the charging station.
        /// </summary>
        /// <param name="Request">A get installed certificate ids request.</param>
        Task<GetInstalledCertificateIdsResponse> GetInstalledCertificateIds(GetInstalledCertificateIdsRequest Request);

        #endregion

        #region DeleteCertificate

        /// <summary>
        /// Remove the given certificate from the charging station.
        /// </summary>
        /// <param name="Request">A delete certificate request.</param>
        Task<DeleteCertificateResponse> DeleteCertificate(DeleteCertificateRequest Request);

        #endregion


        #region GetLocalListVersion

        /// <summary>
        /// Return the local white list of the given charging station.
        /// </summary>
        /// <param name="Request">A get local list version request.</param>
        Task<GetLocalListVersionResponse> GetLocalListVersion(GetLocalListVersionRequest Request);

        #endregion

        #region SendLocalList

        /// <summary>
        /// Set the local white liste at the given charging station.
        /// </summary>
        /// <param name="Request">A send local list request.</param>
        Task<SendLocalListResponse> SendLocalList(SendLocalListRequest Request);

        #endregion

        #region ClearCache

        /// <summary>
        /// Clear the local white liste cache of the given charging station.
        /// </summary>
        /// <param name="Request">A clear cache request.</param>
        Task<ClearCacheResponse> ClearCache(ClearCacheRequest Request);

        #endregion


        #region ReserveNow

        /// <summary>
        /// Create a charging reservation at the given charging station.
        /// </summary>
        /// <param name="Request">A reserve now request.</param>
        Task<ReserveNowResponse> ReserveNow(ReserveNowRequest  Request);

        #endregion

        #region CancelReservation

        /// <summary>
        /// Cancel the given charging reservation.
        /// </summary>
        /// <param name="Request">A cancel reservation request.</param>
        Task<CancelReservationResponse> CancelReservation(CancelReservationRequest Request);

        #endregion

        #region StartCharging

        /// <summary>
        /// Start a charging process (transaction).
        /// </summary>
        /// <param name="Request">A request start transaction request.</param>
        Task<RequestStartTransactionResponse> StartCharging(RequestStartTransactionRequest Request);

        #endregion

        #region StopCharging

        /// <summary>
        /// Stop a charging process (transaction).
        /// </summary>
        /// <param name="Request">A request stop transaction request.</param>
        Task<RequestStopTransactionResponse> StopCharging(RequestStopTransactionRequest Request);

        #endregion

        #region GetTransactionStatus

        /// <summary>
        /// Get the status of a charging process (transaction).
        /// </summary>
        /// <param name="Request">A get transaction status request.</param>
        Task<GetTransactionStatusResponse> GetTransactionStatus(GetTransactionStatusRequest Request);

        #endregion

        #region SetChargingProfile

        /// <summary>
        /// Set the charging profile of the given EVSE at the given charging station.
        /// </summary>
        /// <param name="Request">A set charging profile request.</param>
        Task<SetChargingProfileResponse> SetChargingProfile(SetChargingProfileRequest Request);

        #endregion

        #region GetChargingProfiles

        /// <summary>
        /// Get all charging profiles from the given charging station.
        /// </summary>
        /// <param name="Request">A get charging profiles request.</param>
        Task<GetChargingProfilesResponse> GetChargingProfiles(GetChargingProfilesRequest Request);

        #endregion

        #region ClearChargingProfile

        /// <summary>
        /// Remove matching charging profiles from the given charging station.
        /// </summary>
        /// <param name="Request">A clear charging profile request.</param>
        Task<ClearChargingProfileResponse> ClearChargingProfile(ClearChargingProfileRequest Request);

        #endregion

        #region GetCompositeSchedule

        /// <summary>
        /// Return the charging schedule at the given charging station and EVSE.
        /// </summary>
        /// <param name="Request">A get composite schedule request.</param>
        Task<GetCompositeScheduleResponse> GetCompositeSchedule(GetCompositeScheduleRequest Request);

        #endregion

        #region UnlockConnector

        /// <summary>
        /// Unlock the given EVSE/connector at the given charging station.
        /// </summary>
        /// <param name="Request">An unlock connector request.</param>
        Task<UnlockConnectorResponse> UnlockConnector(UnlockConnectorRequest Request);

        #endregion


        #region SetDisplayMessage

        /// <summary>
        /// Set a display message.
        /// </summary>
        /// <param name="Request">A set display message request.</param>
        Task<SetDisplayMessageResponse> SetDisplayMessage(SetDisplayMessageRequest Request);

        #endregion

        #region GetDisplayMessages

        /// <summary>
        /// Get all display messages.
        /// </summary>
        /// <param name="Request">A get display messages request.</param>
        Task<GetDisplayMessagesResponse> GetDisplayMessages(GetDisplayMessagesRequest Request);

        #endregion

        #region ClearDisplayMessage

        /// <summary>
        /// Remove the given display message.
        /// </summary>
        /// <param name="Request">A clear display message request.</param>
        Task<ClearDisplayMessageResponse> ClearDisplayMessage(ClearDisplayMessageRequest Request);

        #endregion

        #region SendCostUpdated

        /// <summary>
        /// Send updated cost(s).
        /// </summary>
        /// <param name="Request">A cost updated request.</param>
        Task<CostUpdatedResponse> SendCostUpdated(CostUpdatedRequest Request);

        #endregion

        #region RequestCustomerInformation

        /// <summary>
        /// Request customer information.
        /// </summary>
        /// <param name="Request">A customer information request.</param>
        Task<CustomerInformationResponse> RequestCustomerInformation(CustomerInformationRequest Request);

        #endregion


    }

}
