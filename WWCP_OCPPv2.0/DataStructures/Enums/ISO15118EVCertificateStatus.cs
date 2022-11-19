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
    /// Extentions methods for ISO 15118 EV certificate status.
    /// </summary>
    public static class ISO15118EVCertificateStatusExtentions
    {

        #region Parse(Text)

        public static ISO15118EVCertificateStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"  => ISO15118EVCertificateStatus.Accepted,
                   "Failed"    => ISO15118EVCertificateStatus.Failed,
                   _           => ISO15118EVCertificateStatus.Unknown
               };

        public static Boolean TryParse(String Text, out ISO15118EVCertificateStatus Status)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    Status = ISO15118EVCertificateStatus.Accepted;
                    return true;

                case "Failed":
                    Status = ISO15118EVCertificateStatus.Failed;
                    return true;

                default:
                    Status = ISO15118EVCertificateStatus.Unknown;
                    return false;

            }
        }

        #endregion

        #region AsText(this ISO15118EVCertificateStatus)

        public static String AsText(this ISO15118EVCertificateStatus ISO15118EVCertificateStatus)

            => ISO15118EVCertificateStatus switch {
                   ISO15118EVCertificateStatus.Accepted  => "Accepted",
                   ISO15118EVCertificateStatus.Failed    => "Failed",
                   _                                     => "Unknown"
               };

        #endregion

    }

    /// <summary>
    /// ISO 15118 EV certificate status.
    /// </summary>
    public enum ISO15118EVCertificateStatus
    {

        /// <summary>
        /// Unknown ISO 15118 EV certificate status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The EXI message was processed properly and a EXI response is included.
        /// This is no indication whether the update was successful.
        /// </summary>
        Accepted,

        /// <summary>
        /// The processing of the EXI message was not successful.
        /// No EXI response is included.
        /// </summary>
        Failed

    }

}
