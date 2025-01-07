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
    /// Extensions methods for certificate signed status.
    /// </summary>
    public static class CertificateSignedStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a certificate signed status.
        /// </summary>
        /// <param name="Text">A text representation of a certificate signed status.</param>
        public static CertificateSignedStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return CertificateSignedStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a certificate signed status.
        /// </summary>
        /// <param name="Text">A text representation of a certificate signed status.</param>
        public static CertificateSignedStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out CertificateSignedStatus)

        /// <summary>
        /// Try to parse the given text as a certificate signed status.
        /// </summary>
        /// <param name="Text">A text representation of a certificate signed status.</param>
        /// <param name="CertificateSignedStatus">The parsed certificate signed status.</param>
        public static Boolean TryParse(String Text, out CertificateSignedStatus CertificateSignedStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    CertificateSignedStatus = CertificateSignedStatus.Accepted;
                    return true;

                case "Rejected":
                    CertificateSignedStatus = CertificateSignedStatus.Rejected;
                    return true;

                default:
                    CertificateSignedStatus = CertificateSignedStatus.Unknown;
                    return false;

            }
        }

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
    /// Certificate signed status.
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
