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
    /// Extentions methods for update status.
    /// </summary>
    public static class UpdateStatusExtentions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an update status.
        /// </summary>
        /// <param name="Text">A text representation of an update status.</param>
        public static UpdateStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return UpdateStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an update status.
        /// </summary>
        /// <param name="Text">A text representation of an update status.</param>
        public static UpdateStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out UpdateStatus)

        /// <summary>
        /// Try to parse the given text as an update status.
        /// </summary>
        /// <param name="Text">A text representation of an update status.</param>
        /// <param name="UpdateStatus">The parsed update status.</param>
        public static Boolean TryParse(String Text, out UpdateStatus UpdateStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    UpdateStatus = UpdateStatus.Accepted;
                    return true;

                case "Failed":
                    UpdateStatus = UpdateStatus.Failed;
                    return true;

                case "NotSupported":
                    UpdateStatus = UpdateStatus.NotSupported;
                    return true;

                case "VersionMismatch":
                    UpdateStatus = UpdateStatus.VersionMismatch;
                    return true;

                default:
                    UpdateStatus = UpdateStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this UpdateStatus)

        public static String AsText(this UpdateStatus UpdateStatus)

            => UpdateStatus switch {
                   UpdateStatus.Accepted         => "Accepted",
                   UpdateStatus.Failed           => "Failed",
                   UpdateStatus.NotSupported     => "NotSupported",
                   UpdateStatus.VersionMismatch  => "VersionMismatch",
                   _                             => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Update status.
    /// </summary>
    public enum UpdateStatus
    {

        /// <summary>
        /// Unknown update status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Local authorization list successfully updated.
        /// </summary>
        Accepted,

        /// <summary>
        /// Failed to update the local authorization list.
        /// </summary>
        Failed,

        /// <summary>
        /// Update of Local Authorization List is not
        /// supported by Charge Point.
        /// </summary>
        NotSupported,

        /// <summary>
        /// Version number in the request for a differential
        /// update is less or equal then version number of
        /// current list.
        /// </summary>
        VersionMismatch

    }

}
