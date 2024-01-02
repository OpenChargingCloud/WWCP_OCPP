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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for update firmware status.
    /// </summary>
    public static class UpdateFirmwareStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an update firmware status.
        /// </summary>
        /// <param name="Text">A text representation of an update firmware status.</param>
        public static UpdateFirmwareStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return UpdateFirmwareStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an update firmware status.
        /// </summary>
        /// <param name="Text">A text representation of an update firmware status.</param>
        public static UpdateFirmwareStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out UpdateFirmwareStatus)

        /// <summary>
        /// Try to parse the given text as an update firmware status.
        /// </summary>
        /// <param name="Text">A text representation of an update firmware status.</param>
        /// <param name="UpdateFirmwareStatus">The parsed update firmware status.</param>
        public static Boolean TryParse(String Text, out UpdateFirmwareStatus UpdateFirmwareStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    UpdateFirmwareStatus = UpdateFirmwareStatus.Accepted;
                    return true;

                case "Rejected":
                    UpdateFirmwareStatus = UpdateFirmwareStatus.Rejected;
                    return true;

                case "AcceptedCanceled":
                    UpdateFirmwareStatus = UpdateFirmwareStatus.AcceptedCanceled;
                    return true;

                case "InvalidCertificate":
                    UpdateFirmwareStatus = UpdateFirmwareStatus.InvalidCertificate;
                    return true;

                case "RevokedCertificate":
                    UpdateFirmwareStatus = UpdateFirmwareStatus.RevokedCertificate;
                    return true;

                default:
                    UpdateFirmwareStatus = UpdateFirmwareStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this UpdateFirmwareStatus)

        /// <summary>
        /// Return a string representation of the given update firmware status.
        /// </summary>
        /// <param name="UpdateFirmwareStatus">A update firmware status.</param>
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
    /// Update firmware status.
    /// </summary>
    public enum UpdateFirmwareStatus
    {

        /// <summary>
        /// Unknown update firmware status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The firmware update request was accepted.
        /// This does not mean the firmware update is successful, the charging station will now start the firmware update process.
        /// </summary>
        Accepted,

        /// <summary>
        /// The firmware update request rejected.
        /// </summary>
        Rejected,

        /// <summary>
        /// The firmware update request was accepted, but in doing this has canceled an ongoing firmware update.
        /// </summary>
        AcceptedCanceled,

        /// <summary>
        /// The firmware signing certificate is invalid.
        /// </summary>
        InvalidCertificate,

        /// <summary>
        /// The firmware signing certificate has been revoked.
        /// </summary>
        RevokedCertificate

    }

}
