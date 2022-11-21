﻿/*
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
    /// Extentions methods for the certificate status.
    /// </summary>
    public static class CertificateStatusExtentions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a certificate status.</param>
        public static CertificateStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return CertificateStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a certificate status.</param>
        public static CertificateStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out Status)

        /// <summary>
        /// Try to parse the given text as a certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a certificate status.</param>
        /// <param name="Status">The parsed certificate status.</param>
        public static Boolean TryParse(String Text, out CertificateStatus Status)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    Status = CertificateStatus.Accepted;
                    return true;

                case "Rejected":
                    Status = CertificateStatus.Rejected;
                    return true;

                case "Failed":
                    Status = CertificateStatus.Failed;
                    return true;

                default:
                    Status = CertificateStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this CertificateStatus)

        public static String AsText(this CertificateStatus CertificateStatus)

            => CertificateStatus switch {
                   CertificateStatus.Accepted  => "Accepted",
                   CertificateStatus.Rejected  => "Rejected",
                   CertificateStatus.Failed    => "Failed",
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
        /// The certificate is invalid and/or incorrect OR the CSO tries to install more certificates than allowed.
        /// </summary>
        Rejected,

        /// <summary>
        /// The certificate is valid and correct, but there is another reason the installation did not succeed.
        /// </summary>
        Failed

    }

}
