/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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
    /// Extensions methods for publish firmware status.
    /// </summary>
    public static class PublishFirmwareStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a publish firmware status.
        /// </summary>
        /// <param name="Text">A text representation of a publish firmware status.</param>
        public static PublishFirmwareStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return PublishFirmwareStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a publish firmware status.
        /// </summary>
        /// <param name="Text">A text representation of a publish firmware status.</param>
        public static PublishFirmwareStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out PublishFirmwareStatus)

        /// <summary>
        /// Try to parse the given text as a publish firmware status.
        /// </summary>
        /// <param name="Text">A text representation of a publish firmware status.</param>
        /// <param name="PublishFirmwareStatus">The parsed publish firmware status.</param>
        public static Boolean TryParse(String Text, out PublishFirmwareStatus PublishFirmwareStatus)
        {
            switch (Text.Trim())
            {

                case "Idle":
                    PublishFirmwareStatus = PublishFirmwareStatus.Idle;
                    return true;

                case "DownloadScheduled":
                    PublishFirmwareStatus = PublishFirmwareStatus.DownloadScheduled;
                    return true;

                case "Downloading":
                    PublishFirmwareStatus = PublishFirmwareStatus.Downloading;
                    return true;

                case "Downloaded":
                    PublishFirmwareStatus = PublishFirmwareStatus.Downloaded;
                    return true;

                case "Published":
                    PublishFirmwareStatus = PublishFirmwareStatus.Published;
                    return true;

                case "DownloadFailed":
                    PublishFirmwareStatus = PublishFirmwareStatus.DownloadFailed;
                    return true;

                case "DownloadPaused":
                    PublishFirmwareStatus = PublishFirmwareStatus.DownloadPaused;
                    return true;

                case "InvalidChecksum":
                    PublishFirmwareStatus = PublishFirmwareStatus.InvalidChecksum;
                    return true;

                case "ChecksumVerified":
                    PublishFirmwareStatus = PublishFirmwareStatus.ChecksumVerified;
                    return true;

                case "PublishFailed":
                    PublishFirmwareStatus = PublishFirmwareStatus.PublishFailed;
                    return true;

                default:
                    PublishFirmwareStatus = PublishFirmwareStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this PublishFirmwareStatus)

        /// <summary>
        /// Return a string representation of the given publish firmware status.
        /// </summary>
        /// <param name="PublishFirmwareStatus">A publish firmware status.</param>
        public static String AsText(this PublishFirmwareStatus PublishFirmwareStatus)

            => PublishFirmwareStatus switch {
                   PublishFirmwareStatus.Idle               => "Idle",
                   PublishFirmwareStatus.DownloadScheduled  => "DownloadScheduled",
                   PublishFirmwareStatus.Downloading        => "Downloading",
                   PublishFirmwareStatus.Downloaded         => "Downloaded",
                   PublishFirmwareStatus.Published          => "Published",
                   PublishFirmwareStatus.DownloadFailed     => "DownloadFailed",
                   PublishFirmwareStatus.DownloadPaused     => "DownloadPaused",
                   PublishFirmwareStatus.InvalidChecksum    => "InvalidChecksum",
                   PublishFirmwareStatus.ChecksumVerified   => "ChecksumVerified",
                   PublishFirmwareStatus.PublishFailed      => "PublishFailed",
                   _                                        => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Status of a publish firmware download as reported in a
    /// PublishFirmwareStatusNotification request.
    /// </summary>
    public enum PublishFirmwareStatus
    {

        /// <summary>
        /// Unknown publish firmware status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The local controller is not performing publish firmware update related tasks.
        /// Status Idle SHALL only be used as in a PublishFirmwareStatusNotification
        /// request that was triggered by a TriggerMessage request.
        /// </summary>
        Idle,

        /// <summary>
        /// Downloading of new publish firmware has been scheduled (Intermediate state).
        /// </summary>
        DownloadScheduled,

        /// <summary>
        /// The firmware is being downloaded (Intermediate state).
        /// </summary>
        Downloading,

        /// <summary>
        /// The new firmware has been downloaded by the local controller (Intermediate state).
        /// </summary>
        Downloaded,

        /// <summary>
        /// The new firmware has successfully been published in the local controller (Successful end state).
        /// </summary>
        Published,

        /// <summary>
        /// The local controller failed to download the publish firmware (Failure end state).
        /// </summary>
        DownloadFailed,

        /// <summary>
        /// Downloading of the new firmware has been paused (Intermediate state).
        /// </summary>
        DownloadPaused,

        /// <summary>
        ///  The firmware checksum is not valid (Failure end state).
        /// </summary>
        InvalidChecksum,

        /// <summary>
        ///  Provide checksum successfully verified (Intermediate state).
        /// </summary>
        ChecksumVerified,

        /// <summary>
        /// Publishing the new firmware has failed (Failure end state).
        /// </summary>
        PublishFailed

    }

}
