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
    /// Extensions methods for the upload log status.
    /// </summary>
    public static class UploadLogStatusExtensions
    {

        #region Parse(Text)

        public static UploadLogStatus Parse(String Text)

            => Text.Trim() switch {
                   "BadMessage"             => UploadLogStatus.BadMessage,
                   "Idle"                   => UploadLogStatus.Idle,
                   "NotSupportedOperation"  => UploadLogStatus.NotSupportedOperation,
                   "PermissionDenied"       => UploadLogStatus.PermissionDenied,
                   "Uploaded"               => UploadLogStatus.Uploaded,
                   "UploadFailure"          => UploadLogStatus.UploadFailure,
                   "Uploading"              => UploadLogStatus.Uploading,
                   _                        => UploadLogStatus.Unknown
               };

        #endregion

        #region AsText(this UploadLogStatus)

        public static String AsText(this UploadLogStatus UploadLogStatus)

            => UploadLogStatus switch {
                   UploadLogStatus.BadMessage             => "BadMessage",
                   UploadLogStatus.Idle                   => "Idle",
                   UploadLogStatus.NotSupportedOperation  => "NotSupportedOperation",
                   UploadLogStatus.PermissionDenied       => "PermissionDenied",
                   UploadLogStatus.Uploaded               => "Uploaded",
                   UploadLogStatus.UploadFailure          => "UploadFailure",
                   UploadLogStatus.Uploading              => "Uploading",
                   _                                      => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// The status in a response to a log status notification request.
    /// </summary>
    public enum UploadLogStatus
    {

        /// <summary>
        /// Unknown update status.
        /// </summary>
        Unknown,

        /// <summary>
        /// A badly formatted packet or other protocol incompatibility was detected.
        /// </summary>
        BadMessage,

        /// <summary>
        /// The Charge Point is not uploading a log file. Idle SHALL only be used when the message was triggered by a ExtendedTriggerMessage.req.
        /// </summary>
        Idle,

        /// <summary>
        /// The server does not support the operation.
        /// </summary>
        NotSupportedOperation,

        /// <summary>
        /// Insufficient permissions to perform the operation.
        /// </summary>
        PermissionDenied,

        /// <summary>
        /// File has been uploaded successfully.
        /// </summary>
        Uploaded,

        /// <summary>
        /// Failed to upload the requested file.
        /// </summary>
        UploadFailure,

        /// <summary>
        /// File is being uploaded.
        /// </summary>
        Uploading

    }

}
