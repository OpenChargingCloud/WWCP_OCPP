/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extentions methods for the update firmware status.
    /// </summary>
    public static class UpdateFirmwareStatusExtentions
    {

        #region Parse(Text)

        public static UpdateFirmwareStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"            => UpdateFirmwareStatus.Accepted,
                   "Rejected"            => UpdateFirmwareStatus.Rejected,
                   "AcceptedCanceled"    => UpdateFirmwareStatus.AcceptedCanceled,
                   "InvalidCertificate"  => UpdateFirmwareStatus.InvalidCertificate,
                   "RevokedCertificate"  => UpdateFirmwareStatus.RevokedCertificate,
                   _                     => UpdateFirmwareStatus.Unknown
               };

        #endregion

        #region AsText(this UpdateFirmwareStatus)

        public static String AsText(this UpdateFirmwareStatus UpdateFirmwareStatus)

            => UpdateFirmwareStatus switch {
                   UpdateFirmwareStatus.Accepted            => "Accepted",
                   UpdateFirmwareStatus.Rejected            => "Rejected",
                   UpdateFirmwareStatus.AcceptedCanceled    => "AcceptedCanceled",
                   UpdateFirmwareStatus.InvalidCertificate  => "InvalidCertificate",
                   UpdateFirmwareStatus.RevokedCertificate  => "RevokedCertificate",
                   _                                        => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// The status in a response to a signed update firmware request.
    /// </summary>
    public enum UpdateFirmwareStatus
    {

        /// <summary>
        /// Unknown update status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Accepted this firmware update request. This does not mean the firmware update is successful, the Charge Point will now start the firmware update process.
        /// </summary>
        Accepted,

        /// <summary>
        /// Firmware update request rejected.
        /// </summary>
        Rejected,

        /// <summary>
        /// Accepted this firmware update request, but in doing this has canceled an ongoing firmware update.
        /// </summary>
        AcceptedCanceled,

        /// <summary>
        /// The certificate is invalid.
        /// </summary>
        InvalidCertificate,

        /// <summary>
        /// Failure end state. The Firmware Signing certificate has been revoked.
        /// </summary>
        RevokedCertificate

    }

}
