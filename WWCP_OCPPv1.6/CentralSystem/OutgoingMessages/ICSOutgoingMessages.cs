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

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The common interface of all central system clients.
    /// </summary>
    public interface ICSOutgoingMessages : OCPP.ICSMSOutgoingMessages
    {

        OCPP.NetworkingNode_Id                  ChargeBoxIdentity    { get; }
        String                                  From                 { get; }
        String                                  To                   { get; }

        //CentralSystemSOAPClient.CSClientLogger  Logger               { get; }

        #region Reset

        /// <summary>
        /// Reset the given charge box.
        /// </summary>
        /// <param name="Request">A reset request.</param>
        Task<CP.ResetResponse> Reset(ResetRequest Request);

        #endregion

        #region ChangeAvailability

        /// <summary>
        /// Change the availability of the given charge box connector.
        /// </summary>
        /// <param name="Request">A change availability request.</param>
        Task<CP.ChangeAvailabilityResponse> ChangeAvailability(ChangeAvailabilityRequest Request);

        #endregion

        #region GetConfiguration

        /// <summary>
        /// Get the configuration of the given charge box.
        /// </summary>
        /// <param name="Request">A get configuration request.</param>
        Task<CP.GetConfigurationResponse> GetConfiguration(GetConfigurationRequest Request);

        #endregion

        #region ChangeConfiguration

        /// <summary>
        /// Change the configuration of the given charge box.
        /// </summary>
        /// <param name="Request">A change configuration request.</param>
        Task<CP.ChangeConfigurationResponse> ChangeConfiguration(ChangeConfigurationRequest Request);

        #endregion

        #region DataTransfer

        /// <summary>
        /// Transfer the given data to the given charge box.
        /// </summary>
        /// <param name="Request">A data transfer request.</param>
        Task<CP.DataTransferResponse> DataTransfer(DataTransferRequest Request);

        #endregion

        #region GetDiagnostics

        /// <summary>
        /// Upload diagnostics data of the given charge box to the given file location.
        /// </summary>
        /// <param name="Request">A get diagnostics request.</param>
        Task<CP.GetDiagnosticsResponse> GetDiagnostics(GetDiagnosticsRequest Request);

        #endregion

        #region TriggerMessage

        /// <summary>
        /// Create a trigger for the given message at the given charge box connector.
        /// </summary>
        /// <param name="Request">A trigger message request.</param>
        Task<CP.TriggerMessageResponse> TriggerMessage(TriggerMessageRequest Request);

        #endregion

        #region UpdateFirmware

        /// <summary>
        /// Initiate a firmware download from the given location at the given charge box.
        /// </summary>
        /// <param name="Request">An update firmware request.</param>
        Task<CP.UpdateFirmwareResponse> UpdateFirmware(UpdateFirmwareRequest Request);

        #endregion


        #region ReserveNow

        /// <summary>
        /// Create a charging reservation at the given charge box connector.
        /// </summary>
        /// <param name="Request">A reserve now request.</param>
        Task<CP.ReserveNowResponse> ReserveNow(ReserveNowRequest  Request);

        #endregion

        #region CancelReservation

        /// <summary>
        /// Cancel the given charging reservation at the given charge box.
        /// </summary>
        /// <param name="Request">A cancel reservation request.</param>
        Task<CP.CancelReservationResponse> CancelReservation(CancelReservationRequest Request);

        #endregion

        #region RemoteStartTransaction

        /// <summary>
        /// Start a charging session at the given charge box connector using the given charging profile.
        /// </summary>
        /// <param name="Request">A remote start transaction request.</param>
        Task<CP.RemoteStartTransactionResponse> RemoteStartTransaction(RemoteStartTransactionRequest Request);

        #endregion

        #region RemoteStopTransaction

        /// <summary>
        /// Stop a charging session at the given charge box.
        /// </summary>
        /// <param name="Request">A remote stop transaction request.</param>
        Task<CP.RemoteStopTransactionResponse> RemoteStopTransaction(RemoteStopTransactionRequest Request);

        #endregion

        #region SetChargingProfile

        /// <summary>
        /// Set the charging profile of the given charge box connector.
        /// </summary>
        /// <param name="Request">A set charging profile request.</param>
        Task<CP.SetChargingProfileResponse> SetChargingProfile(SetChargingProfileRequest Request);

        #endregion

        #region ClearChargingProfile

        /// <summary>
        /// Remove the charging profile at the given charge box connector.
        /// </summary>
        /// <param name="Request">A clear charging profile request.</param>
        Task<CP.ClearChargingProfileResponse> ClearChargingProfile(ClearChargingProfileRequest Request);

        #endregion

        #region GetCompositeSchedule

        /// <summary>
        /// Return the charging schedule of the given charge box connector.
        /// </summary>
        /// <param name="Request">A get composite schedule request.</param>
        Task<CP.GetCompositeScheduleResponse> GetCompositeSchedule(GetCompositeScheduleRequest Request);

        #endregion

        #region UnlockConnector

        /// <summary>
        /// Unlock the given charge box connector.
        /// </summary>
        /// <param name="Request">An unlock connector request.</param>
        Task<CP.UnlockConnectorResponse> UnlockConnector(UnlockConnectorRequest Request);

        #endregion


        #region GetLocalListVersion

        /// <summary>
        /// Return the local white list of the given charge box.
        /// </summary>
        /// <param name="Request">A get local list version request.</param>
        Task<CP.GetLocalListVersionResponse> GetLocalListVersion(GetLocalListVersionRequest Request);

        #endregion

        #region SendLocalList

        /// <summary>
        /// Set the local white liste at the given charge box.
        /// </summary>
        /// <param name="Request">A send local list request.</param>
        Task<CP.SendLocalListResponse> SendLocalList(SendLocalListRequest Request);

        #endregion

        #region ClearCache

        /// <summary>
        /// Clear the local white liste cache of the given charge box.
        /// </summary>
        /// <param name="Request">A clear cache request.</param>
        Task<CP.ClearCacheResponse> ClearCache(ClearCacheRequest Request);

        #endregion



        // Security extensions

        #region CertificateSigned

        /// <summary>
        /// Send the signed certificate to the charge point.
        /// </summary>
        /// <param name="Request">A certificate signed request.</param>
        Task<CP.CertificateSignedResponse> CertificateSigned(CertificateSignedRequest Request);

        #endregion

        #region DeleteCertificate

        /// <summary>
        /// Delete the given certificate on the charge point.
        /// </summary>
        /// <param name="Request">A delete certificate request.</param>
        Task<CP.DeleteCertificateResponse> DeleteCertificate(DeleteCertificateRequest Request);

        #endregion

        #region ExtendedTriggerMessage

        /// <summary>
        /// Send an extended trigger message to the charge point.
        /// </summary>
        /// <param name="Request">A extended trigger message request.</param>
        Task<CP.ExtendedTriggerMessageResponse> ExtendedTriggerMessage(ExtendedTriggerMessageRequest Request);

        #endregion

        #region GetInstalledCertificateIds

        /// <summary>
        /// Retrieve a list of all installed certificates within the charge point.
        /// </summary>
        /// <param name="Request">A get installed certificate ids request.</param>
        Task<CP.GetInstalledCertificateIdsResponse> GetInstalledCertificateIds(GetInstalledCertificateIdsRequest Request);

        #endregion

        #region GetLog

        /// <summary>
        /// Retrieve log files from the charge point.
        /// </summary>
        /// <param name="Request">A get log request.</param>
        Task<CP.GetLogResponse> GetLog(GetLogRequest Request);

        #endregion

        #region InstallCertificate

        /// <summary>
        /// Install the given certificate within the charge point.
        /// </summary>
        /// <param name="Request">An install certificate request.</param>
        Task<CP.InstallCertificateResponse> InstallCertificate(InstallCertificateRequest Request);

        #endregion

        #region SignedUpdateFirmware

        /// <summary>
        /// Update the firmware of the charge point.
        /// </summary>
        /// <param name="Request">A signed update firmware request.</param>
        Task<CP.SignedUpdateFirmwareResponse> SignedUpdateFirmware(SignedUpdateFirmwareRequest Request);

        #endregion


    }

}
