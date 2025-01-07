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
    /// Extensions methods for ISO 15118 EV certificate status.
    /// </summary>
    public static class ISO15118EVCertificateStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an ISO 15118 EV certificate status.
        /// </summary>
        /// <param name="Text">A text representation of an ISO 15118 EV certificate status.</param>
        public static ISO15118EVCertificateStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return ISO15118EVCertificateStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an ISO 15118 EV certificate status.
        /// </summary>
        /// <param name="Text">A text representation of an ISO 15118 EV certificate status.</param>
        public static ISO15118EVCertificateStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out ISO15118EVCertificateStatus)

        /// <summary>
        /// Try to parse the given text as an ISO 15118 EV certificate status.
        /// </summary>
        /// <param name="Text">A text representation of an ISO 15118 EV certificate status.</param>
        /// <param name="ISO15118EVCertificateStatus">The parsed ISO 15118 EV certificate status.</param>
        public static Boolean TryParse(String Text, out ISO15118EVCertificateStatus ISO15118EVCertificateStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    ISO15118EVCertificateStatus = ISO15118EVCertificateStatus.Accepted;
                    return true;

                case "Failed":
                    ISO15118EVCertificateStatus = ISO15118EVCertificateStatus.Failed;
                    return true;

                default:
                    ISO15118EVCertificateStatus = ISO15118EVCertificateStatus.Unknown;
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
