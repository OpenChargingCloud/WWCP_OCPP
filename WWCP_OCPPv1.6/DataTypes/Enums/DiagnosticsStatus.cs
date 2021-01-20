/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extentions methods for the diagnostics status.
    /// </summary>
    public static class DiagnosticsStatusExtentions
    {

        #region Parse(Text)

        /// <summary>
        /// Parse the given string as a diagnostics status.
        /// </summary>
        /// <param name="Text">A string representation of a diagnostics status.</param>
        public static DiagnosticsStatus Parse(String Text)
        {

            switch (Text?.ToLower())
            {

                case "idle":
                    return DiagnosticsStatus.Idle;

                case "uploaded":
                    return DiagnosticsStatus.Uploaded;

                case "uploadfailed":
                    return DiagnosticsStatus.UploadFailed;

                case "uploading":
                    return DiagnosticsStatus.Uploading;


                default:
                    return DiagnosticsStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this DiagnosticsStatus)

        /// <summary>
        /// Return a string representation of the given diagnostics status.
        /// </summary>
        /// <param name="DiagnosticsStatus">A diagnostics status.</param>
        public static String AsText(this DiagnosticsStatus DiagnosticsStatus)
        {

            switch (DiagnosticsStatus)
            {

                case DiagnosticsStatus.Idle:
                    return "Idle";

                case DiagnosticsStatus.Uploaded:
                    return "Uploaded";

                case DiagnosticsStatus.UploadFailed:
                    return "UploadFailed";

                case DiagnosticsStatus.Uploading:
                    return "Uploading";


                default:
                    return "unknown";

            }

        }

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
