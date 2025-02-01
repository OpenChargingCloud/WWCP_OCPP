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
    /// Extensions methods for install certificate status.
    /// </summary>
    public static class InstallCertificateStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a certificate status.</param>
        public static InstallCertificateStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return InstallCertificateStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a certificate status.</param>
        public static InstallCertificateStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out CertificateStatus)

        /// <summary>
        /// Try to parse the given text as a certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a certificate status.</param>
        /// <param name="CertificateStatus">The parsed certificate status.</param>
        public static Boolean TryParse(String Text, out InstallCertificateStatus CertificateStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    CertificateStatus = InstallCertificateStatus.Accepted;
                    return true;

                case "Rejected":
                    CertificateStatus = InstallCertificateStatus.Rejected;
                    return true;

                case "Failed":
                    CertificateStatus = InstallCertificateStatus.Failed;
                    return true;

                default:
                    CertificateStatus = InstallCertificateStatus.Unknown;
                    return false;

            }
        }

        #endregion

        #region AsText  (this CertificateStatus)

        public static String AsText(this InstallCertificateStatus CertificateStatus)

            => CertificateStatus switch {
                   InstallCertificateStatus.Accepted  => "Accepted",
                   InstallCertificateStatus.Rejected  => "Rejected",
                   InstallCertificateStatus.Failed    => "Failed",
                   _                           => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Install certificate status.
    /// </summary>
    public enum InstallCertificateStatus
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
        /// The certificate is invalid and/or incorrect OR the CSO tries to install more certificates than allowed.
        /// </summary>
        Rejected,

        /// <summary>
        /// The certificate is valid and correct, but there is another reason the installation did not succeed.
        /// </summary>
        Failed

    }

}
