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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extensions methods for the certificate status.
    /// </summary>
    public static class CertificateStatusExtensions
    {

        #region Parse(Text)

        public static CertificateStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"  => CertificateStatus.Accepted,
                   "Failed"    => CertificateStatus.Failed,
                   "Rejected"  => CertificateStatus.Rejected,
                   _           => CertificateStatus.Unknown
               };

        #endregion

        #region AsText(this CertificateStatus)

        public static String AsText(this CertificateStatus CertificateStatus)

            => CertificateStatus switch {
                   CertificateStatus.Accepted  => "Accepted",
                   CertificateStatus.Failed    => "Failed",
                   CertificateStatus.Rejected  => "Rejected",
                   _                           => "Unknown"
               };

        #endregion

    }

    /// <summary>
    /// The status in a response to an install certificate request.
    /// </summary>
    public enum CertificateStatus
    {

        /// <summary>
        /// Unknown certificate status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The installation of the certificate succeeded.
        /// </summary>
        Accepted,

        /// <summary>
        /// The certificate is valid and correct, but there is another reason the installation did not succeed.
        /// </summary>
        Failed,

        /// <summary>
        /// The certificate is invalid and/or incorrect OR the CPO tries to install more certificates than allowed.
        /// </summary>
        Rejected

    }

}
