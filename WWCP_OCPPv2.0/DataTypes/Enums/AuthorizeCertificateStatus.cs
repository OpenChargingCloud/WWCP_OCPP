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

#region Usings

using System;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extentions methods for the authorize certificate status.
    /// </summary>
    public static class AuthorizeCertificateStatusExtentions
    {

        #region Parse(Text)

        public static AuthorizeCertificateStatus Parse(String Text)
        {

            switch (Text?.Trim())
            {

                case "Accepted":
                    return AuthorizeCertificateStatus.Accepted;

                case "SignatureError":
                    return AuthorizeCertificateStatus.SignatureError;

                case "CertificateExpired":
                    return AuthorizeCertificateStatus.CertificateExpired;

                case "CertificateRevoked":
                    return AuthorizeCertificateStatus.CertificateRevoked;

                case "NoCertificateAvailable":
                    return AuthorizeCertificateStatus.NoCertificateAvailable;

                case "CertChainError":
                    return AuthorizeCertificateStatus.CertChainError;

                case "ContractCancelled":
                    return AuthorizeCertificateStatus.ContractCancelled;


                default:
                    return AuthorizeCertificateStatus.Unknown;

            }

        }

        #endregion

        #region AsText(this AuthorizeCertificateStatus)

        public static String AsText(this AuthorizeCertificateStatus AuthorizeCertificateStatus)
        {

            switch (AuthorizeCertificateStatus)
            {

                case AuthorizeCertificateStatus.Accepted:
                    return "Accepted";

                case AuthorizeCertificateStatus.SignatureError:
                    return "SignatureError";

                case AuthorizeCertificateStatus.CertificateExpired:
                    return "CertificateExpired";

                case AuthorizeCertificateStatus.CertificateRevoked:
                    return "CertificateRevoked";

                case AuthorizeCertificateStatus.NoCertificateAvailable:
                    return "NoCertificateAvailable";

                case AuthorizeCertificateStatus.CertChainError:
                    return "CertChainError";

                case AuthorizeCertificateStatus.ContractCancelled:
                    return "ContractCancelled";


                default:
                    return "Unknown";

            }

        }

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
