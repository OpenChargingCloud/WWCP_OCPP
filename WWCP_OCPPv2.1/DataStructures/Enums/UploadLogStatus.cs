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
    /// Extensions methods for upload log status.
    /// </summary>
    public static class UploadLogStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an upload log status.
        /// </summary>
        /// <param name="Text">A text representation of an upload log status.</param>
        public static UploadLogStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return UploadLogStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an upload log status.
        /// </summary>
        /// <param name="Text">A text representation of an upload log status.</param>
        public static UploadLogStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out UploadLogStatus)

        /// <summary>
        /// Try to parse the given text as an upload log status.
        /// </summary>
        /// <param name="Text">A text representation of an upload log status.</param>
        /// <param name="UploadLogStatus">The parsed upload log status.</param>
        public static Boolean TryParse(String Text, out UploadLogStatus UploadLogStatus)
        {
            switch (Text.Trim())
            {

                case "BadMessage":
                    UploadLogStatus = UploadLogStatus.BadMessage;
                    return true;

                case "Idle":
                    UploadLogStatus = UploadLogStatus.Idle;
                    return true;

                case "NotSupportedOperation":
                    UploadLogStatus = UploadLogStatus.NotSupportedOperation;
                    return true;

                case "PermissionDenied":
                    UploadLogStatus = UploadLogStatus.PermissionDenied;
                    return true;

                case "Uploaded":
                    UploadLogStatus = UploadLogStatus.Uploaded;
                    return true;

                case "UploadFailure":
                    UploadLogStatus = UploadLogStatus.UploadFailure;
                    return true;

                case "Uploading":
                    UploadLogStatus = UploadLogStatus.Uploading;
                    return true;

                case "AcceptedCanceled":
                    UploadLogStatus = UploadLogStatus.AcceptedCanceled;
                    return true;

                default:
                    UploadLogStatus = UploadLogStatus.Unknown;
                    return false;

            }
        }

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
                   UploadLogStatus.AcceptedCanceled       => "AcceptedCanceled",
                   _                                      => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Upload log status.
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
        Uploading,

        /// <summary>
        /// On-going log upload is canceled and new request to upload log has been accepted.
        /// </summary>
        AcceptedCanceled

    }

}
