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
    /// Extentions methods for the authorize certificate status.
    /// </summary>
    public static class AuthorizeCertificateStatusExtentions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an authorize certificate status.
        /// </summary>
        /// <param name="Text">A text representation of an authorize certificate status.</param>
        public static AuthorizeCertificateStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return AuthorizeCertificateStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an authorize certificate status.
        /// </summary>
        /// <param name="Text">A text representation of an authorize certificate status.</param>
        public static AuthorizeCertificateStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out Status)

        /// <summary>
        /// Try to parse the given text as an authorize certificate status.
        /// </summary>
        /// <param name="Text">A text representation of an authorize certificate status.</param>
        /// <param name="Status">The parsed authorize certificate status.</param>
        public static Boolean TryParse(String Text, out AuthorizeCertificateStatus Status)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    Status = AuthorizeCertificateStatus.Accepted;
                    return true;

                case "SignatureError":
                    Status = AuthorizeCertificateStatus.SignatureError;
                    return true;

                case "CertificateExpired":
                    Status = AuthorizeCertificateStatus.CertificateExpired;
                    return true;

                case "CertificateRevoked":
                    Status = AuthorizeCertificateStatus.CertificateRevoked;
                    return true;

                case "NoCertificateAvailable":
                    Status = AuthorizeCertificateStatus.NoCertificateAvailable;
                    return true;

                case "AcceCertChainErrorpted":
                    Status = AuthorizeCertificateStatus.CertChainError;
                    return true;

                case "ContractCancelled":
                    Status = AuthorizeCertificateStatus.ContractCancelled;
                    return true;

                default:
                    Status = AuthorizeCertificateStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this AuthorizeCertificateStatus)

        public static String AsText(this AuthorizeCertificateStatus AuthorizeCertificateStatus)

            => AuthorizeCertificateStatus switch {
                   AuthorizeCertificateStatus.Accepted                => "Accepted",
                   AuthorizeCertificateStatus.SignatureError          => "SignatureError",
                   AuthorizeCertificateStatus.CertificateExpired      => "CertificateExpired",
                   AuthorizeCertificateStatus.CertificateRevoked      => "CertificateRevoked",
                   AuthorizeCertificateStatus.NoCertificateAvailable  => "NoCertificateAvailable",
                   AuthorizeCertificateStatus.CertChainError          => "CertChainError",
                   AuthorizeCertificateStatus.ContractCancelled       => "ContractCancelled",
                   _                                                  => "Unknown"
               };

        #endregion

    }

    /// <summary>
    /// The status in a response to an authorize certificate request.
    /// </summary>
    public enum AuthorizeCertificateStatus
    {

        /// <summary>
        /// Unknown authorize certificate status.
        /// </summary>
        Unknown,

        Accepted,
        SignatureError,
        CertificateExpired,
        CertificateRevoked,
        NoCertificateAvailable,
        CertChainError,
        ContractCancelled

    }

}
