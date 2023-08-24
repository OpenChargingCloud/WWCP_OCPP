/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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
    /// Extensions methods for send local list status.
    /// </summary>
    public static class SendLocalListStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an update status.
        /// </summary>
        /// <param name="Text">A text representation of an update status.</param>
        public static SendLocalListStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return SendLocalListStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an update status.
        /// </summary>
        /// <param name="Text">A text representation of an update status.</param>
        public static SendLocalListStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out SendLocalListStatus)

        /// <summary>
        /// Try to parse the given text as an update status.
        /// </summary>
        /// <param name="Text">A text representation of an update status.</param>
        /// <param name="SendLocalListStatus">The parsed update status.</param>
        public static Boolean TryParse(String Text, out SendLocalListStatus SendLocalListStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    SendLocalListStatus = SendLocalListStatus.Accepted;
                    return true;

                case "Failed":
                    SendLocalListStatus = SendLocalListStatus.Failed;
                    return true;

                case "NotSupported":
                    SendLocalListStatus = SendLocalListStatus.NotSupported;
                    return true;

                case "VersionMismatch":
                    SendLocalListStatus = SendLocalListStatus.VersionMismatch;
                    return true;

                default:
                    SendLocalListStatus = SendLocalListStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this SendLocalListStatus)

        public static String AsText(this SendLocalListStatus SendLocalListStatus)

            => SendLocalListStatus switch {
                   SendLocalListStatus.Accepted         => "Accepted",
                   SendLocalListStatus.Failed           => "Failed",
                   SendLocalListStatus.NotSupported     => "NotSupported",
                   SendLocalListStatus.VersionMismatch  => "VersionMismatch",
                   _                             => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Send local list status.
    /// </summary>
    public enum SendLocalListStatus
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
