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
    /// Extentions methods for the certificate signed status.
    /// </summary>
    public static class CertificateSignedStatusExtentions
    {

        #region Parse(Text)

        public static CertificateSignedStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"  => CertificateSignedStatus.Accepted,
                   "Rejected"  => CertificateSignedStatus.Rejected,
                   _           => CertificateSignedStatus.Unknown
               };

        #endregion

        #region AsText(this CertificateSignedStatus)

        public static String AsText(this CertificateSignedStatus CertificateSignedStatus)

            => CertificateSignedStatus switch {
                   CertificateSignedStatus.Accepted  => "Accepted",
                   CertificateSignedStatus.Rejected  => "Rejected",
                   _                                 => "Unknown"
               };

        #endregion

    }

    /// <summary>
    /// The status in a response to a certificate signed request.
    /// </summary>
    public enum CertificateSignedStatus
    {

        /// <summary>
        /// Unknown certificate signed status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The signed certificate is valid.
        /// </summary>
        Accepted,

        /// <summary>
        /// The signed certificate is invalid.
        /// </summary>
        Rejected

    }

}
