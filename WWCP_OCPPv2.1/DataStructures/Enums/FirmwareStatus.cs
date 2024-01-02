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
    /// Extensions methods for firmware status.
    /// </summary>
    public static class FirmwareStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a firmware status.
        /// </summary>
        /// <param name="Text">A text representation of a firmware status.</param>
        public static FirmwareStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return FirmwareStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a firmware status.
        /// </summary>
        /// <param name="Text">A text representation of a firmware status.</param>
        public static FirmwareStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out FirmwareStatus)

        /// <summary>
        /// Try to parse the given text as a firmware status.
        /// </summary>
        /// <param name="Text">A text representation of a firmware status.</param>
        /// <param name="FirmwareStatus">The parsed firmware status.</param>
        public static Boolean TryParse(String Text, out FirmwareStatus FirmwareStatus)
        {
            switch (Text.Trim())
            {

                case "Downloaded":
                    FirmwareStatus = FirmwareStatus.Downloaded;
                    return true;

                case "DownloadFailed":
                    FirmwareStatus = FirmwareStatus.DownloadFailed;
                    return true;

                case "Downloading":
                    FirmwareStatus = FirmwareStatus.Downloading;
                    return true;

                case "DownloadScheduled":
                    FirmwareStatus = FirmwareStatus.DownloadScheduled;
                    return true;

                case "DownloadPaused":
                    FirmwareStatus = FirmwareStatus.DownloadPaused;
                    return true;

                case "Idle":
                    FirmwareStatus = FirmwareStatus.Idle;
                    return true;

                case "InstallationFailed":
                    FirmwareStatus = FirmwareStatus.InstallationFailed;
                    return true;

                case "Installing":
                    FirmwareStatus = FirmwareStatus.Installing;
                    return true;

                case "Installed":
                    FirmwareStatus = FirmwareStatus.Installed;
                    return true;

                case "InstallRebooting":
                    FirmwareStatus = FirmwareStatus.InstallRebooting;
                    return true;

                case "InstallScheduled":
                    FirmwareStatus = FirmwareStatus.InstallScheduled;
                    return true;

                case "InstallVerificationFailed":
                    FirmwareStatus = FirmwareStatus.InstallVerificationFailed;
                    return true;

                case "InvalidSignature":
                    FirmwareStatus = FirmwareStatus.InvalidSignature;
                    return true;

                case "SignatureVerified":
                    FirmwareStatus = FirmwareStatus.SignatureVerified;
                    return true;

                default:
                    FirmwareStatus = FirmwareStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this FirmwareStatus)

        /// <summary>
        /// Return a string representation of the given firmware status.
        /// </summary>
        /// <param name="FirmwareStatus">A firmware status.</param>
        public static String AsText(this FirmwareStatus FirmwareStatus)

            => FirmwareStatus switch {
                   FirmwareStatus.Downloaded                 => "Downloaded",
                   FirmwareStatus.DownloadFailed             => "DownloadFailed",
                   FirmwareStatus.Downloading                => "Downloading",
                   FirmwareStatus.DownloadScheduled          => "DownloadScheduled",
                   FirmwareStatus.DownloadPaused             => "DownloadPaused",
                   FirmwareStatus.Idle                       => "Idle",
                   FirmwareStatus.InstallationFailed         => "InstallationFailed",
                   FirmwareStatus.Installing                 => "Installing",
                   FirmwareStatus.Installed                  => "Installed",
                   FirmwareStatus.InstallRebooting           => "InstallRebooting",
                   FirmwareStatus.InstallScheduled           => "InstallScheduled",
                   FirmwareStatus.InstallVerificationFailed  => "InstallVerificationFailed",
                   FirmwareStatus.InvalidSignature           => "InvalidSignature",
                   FirmwareStatus.SignatureVerified          => "SignatureVerified",
                   _                                         => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Firmware status.
    /// </summary>
    public enum FirmwareStatus
    {

        /// <summary>
        /// Unknown firmware status.
        /// </summary>
        Unknown,

        /// <summary>
        /// New firmware has been downloaded by the charging station (Intermediate state).
        /// </summary>
        Downloaded,

        /// <summary>
        /// The charging station failed to download the firmware (Failure end state).
        /// </summary>
        DownloadFailed,

        /// <summary>
        /// The firmware is being downloaded (Intermediate state).
        /// </summary>
        Downloading,

        /// <summary>
        /// Downloading of new firmware has been scheduled (Intermediate state).
        /// </summary>
        DownloadScheduled,

        /// <summary>
        /// Downloading of the firmware has been paused (Intermediate state).
        /// </summary>
        DownloadPaused,

        /// <summary>
        /// The charging station is not performing firmware update related tasks.
        /// Status Idle SHALL only be used as in a FirmwareStatusNotification
        /// request that was triggered by a TriggerMessage request.
        /// </summary>
        Idle,

        /// <summary>
        /// Installation of new firmware has failed (Failure end state).
        /// </summary>
        InstallationFailed,

        /// <summary>
        /// Firmware is being installed (Intermediate state).
        /// </summary>
        Installing,

        /// <summary>
        /// New firmware has successfully been installed in charging station (Successful end state).
        /// </summary>
        Installed,

        /// <summary>
        /// The charging station is about to reboot to activate new firmware. This status MAY be omitted
        /// if a reboot is an integral part of the installation and cannot be reported separately
        /// (Intermediate state).
        /// </summary>
        InstallRebooting,

        /// <summary>
        /// Installation of the downloaded firmware is scheduled to take place on installDateTime given in SignedUpdateFirmware.req (Intermediate state).
        /// </summary>
        InstallScheduled,

        /// <summary>
        /// Verification of the new firmware (e.g. using a checksum or some other means) has failed and installation will not proceed (Failure end state).
        /// </summary>
        InstallVerificationFailed,

        /// <summary>
        ///  The firmware signature is not valid (Failure end state).
        /// </summary>
        InvalidSignature,

        /// <summary>
        ///  Provide signature successfully verified (Intermediate state).
        /// </summary>
        SignatureVerified

    }

}
