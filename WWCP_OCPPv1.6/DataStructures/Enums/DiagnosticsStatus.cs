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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extensions methods for the diagnostics status.
    /// </summary>
    public static class DiagnosticsStatusExtensions
    {

        #region Parse(Text)

        /// <summary>
        /// Parse the given string as a diagnostics status.
        /// </summary>
        /// <param name="Text">A string representation of a diagnostics status.</param>
        public static DiagnosticsStatus Parse(String Text)

            => Text.Trim() switch {
                   "Idle"          => DiagnosticsStatus.Idle,
                   "Uploaded"      => DiagnosticsStatus.Uploaded,
                   "Uploadfailed"  => DiagnosticsStatus.UploadFailed,
                   "Uploading"     => DiagnosticsStatus.Uploading,
                   _               => DiagnosticsStatus.Unknown
               };

        #endregion

        #region AsText(this DiagnosticsStatus)

        /// <summary>
        /// Return a string representation of the given diagnostics status.
        /// </summary>
        /// <param name="DiagnosticsStatus">A diagnostics status.</param>
        public static String AsText(this DiagnosticsStatus DiagnosticsStatus)

            => DiagnosticsStatus switch {
                   DiagnosticsStatus.Idle          => "Idle",
                   DiagnosticsStatus.Uploaded      => "Uploaded",
                   DiagnosticsStatus.UploadFailed  => "UploadFailed",
                   DiagnosticsStatus.Uploading     => "Uploading",
                   _                               => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Status in DiagnosticsStatusNotification request.
    /// </summary>
    public enum DiagnosticsStatus
    {

        /// <summary>
        /// Unknown diagnostics status.
        /// </summary>
        Unknown,


        /// <summary>
        /// Charge point is not performing diagnostics related tasks.
        /// Status Idle SHALL only be used as in a DiagnosticsStatusNotification
        /// request that was triggered by a TriggerMessage request.
        /// </summary>
        Idle,

        /// <summary>
        /// Diagnostics information has been uploaded.
        /// </summary>
        Uploaded,

        /// <summary>
        /// Uploading of diagnostics failed.
        /// </summary>
        UploadFailed,

        /// <summary>
        /// File is being uploaded.
        /// </summary>
        Uploading

    }

}
