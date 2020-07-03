/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

using System;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// Extentions methods for the firmware status.
    /// </summary>
    public static class FirmwareStatusExtentions
    {

        #region AsFirmwareStatus(Text)

        /// <summary>
        /// Parse the given string as a firmware status.
        /// </summary>
        /// <param name="Text">A string representation of a firmware status.</param>
        public static FirmwareStatus Parse(this String Text)
        {

            switch (Text?.ToLower())
            {

                case "downloaded":
                    return FirmwareStatus.Downloaded;

                case "downloadfailed":
                    return FirmwareStatus.DownloadFailed;

                case "downloading":
                    return FirmwareStatus.Downloading;

                case "idle":
                    return FirmwareStatus.Idle;

                case "installationfailed":
                    return FirmwareStatus.InstallationFailed;

                case "installed":
                    return FirmwareStatus.Installed;

                case "installing":
                    return FirmwareStatus.Installing;


                default:
                    return FirmwareStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this FirmwareStatus)

        /// <summary>
        /// Return a string representation of the given firmware status.
        /// </summary>
        /// <param name="FirmwareStatus">A firmware status.</param>
        public static String AsText(this FirmwareStatus FirmwareStatus)
        {

            switch (FirmwareStatus)
            {

                case FirmwareStatus.Downloaded:
                    return "Downloaded";

                case FirmwareStatus.DownloadFailed:
                    return "DownloadFailed";

                case FirmwareStatus.Downloading:
                    return "Downloading";

                case FirmwareStatus.Idle:
                    return "Idle";

                case FirmwareStatus.InstallationFailed:
                    return "InstallationFailed";

                case FirmwareStatus.Installed:
                    return "Installed";

                case FirmwareStatus.Installing:
                    return "Installing";


                default:
                    return "unknown";

            }

        }

        #endregion

    }


    /// <summary>
    /// Status of a firmware download as reported in a
    /// FirmwareStatusNotification request.
    /// </summary>
    public enum FirmwareStatus
    {

        /// <summary>
        /// Unknown firmware status.
        /// </summary>
        Unknown,


        /// <summary>
        /// New firmware has been downloaded by the charge point.
        /// </summary>
        Downloaded,

        /// <summary>
        /// Charge point failed to download the firmware.
        /// </summary>
        DownloadFailed,

        /// <summary>
        /// Firmware is being downloaded.
        /// </summary>
        Downloading,

        /// <summary>
        /// Charge point is not performing firmware update related tasks.
        /// Status Idle SHALL only be used as in a FirmwareStatusNotification
        /// request that was triggered by a TriggerMessage request.
        /// </summary>
        Idle,

        /// <summary>
        /// Installation of new firmware has failed.
        /// </summary>
        InstallationFailed,

        /// <summary>
        /// New firmware has successfully been installed in charge point.
        /// </summary>
        Installed,

        /// <summary>
        /// Firmware is being installed.
        /// </summary>
        Installing


    }

}
