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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extensions methods for unpublish firmware status.
    /// </summary>
    public static class UnpublishFirmwareStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an unpublish firmware status.
        /// </summary>
        /// <param name="Text">A text representation of an unpublish firmware status.</param>
        public static UnpublishFirmwareStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return UnpublishFirmwareStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an unpublish firmware status.
        /// </summary>
        /// <param name="Text">A text representation of an unpublish firmware status.</param>
        public static UnpublishFirmwareStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out UnpublishFirmwareStatus)

        /// <summary>
        /// Try to parse the given text as an unpublish firmware status.
        /// </summary>
        /// <param name="Text">A text representation of an unpublish firmware status.</param>
        /// <param name="UnpublishFirmwareStatus">The parsed unpublish firmware status.</param>
        public static Boolean TryParse(String Text, out UnpublishFirmwareStatus UnpublishFirmwareStatus)
        {
            switch (Text.Trim())
            {

                case "DownloadOngoing":
                    UnpublishFirmwareStatus = UnpublishFirmwareStatus.DownloadOngoing;
                    return true;

                case "NoFirmware":
                    UnpublishFirmwareStatus = UnpublishFirmwareStatus.NoFirmware;
                    return true;

                case "Unpublished":
                    UnpublishFirmwareStatus = UnpublishFirmwareStatus.Unpublished;
                    return true;

                default:
                    UnpublishFirmwareStatus = UnpublishFirmwareStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this UnpublishFirmwareStatus)

        public static String AsText(this UnpublishFirmwareStatus UnpublishFirmwareStatus)

            => UnpublishFirmwareStatus switch {
                   UnpublishFirmwareStatus.DownloadOngoing  => "DownloadOngoing",
                   UnpublishFirmwareStatus.NoFirmware       => "NoFirmware",
                   UnpublishFirmwareStatus.Unpublished      => "Unpublished",
                   _                                        => "Unknown"
               };

        #endregion

    }

    /// <summary>
    /// Unpublish firmware status.
    /// </summary>
    public enum UnpublishFirmwareStatus
    {

        /// <summary>
        /// Unknown unpublish firmware status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Firmware is being downloaded.
        /// </summary>
        DownloadOngoing,

        /// <summary>
        /// There is no published file.
        /// </summary>
        NoFirmware,

        /// <summary>
        /// Firmware file no longer being published.
        /// </summary>
        Unpublished

    }

}
