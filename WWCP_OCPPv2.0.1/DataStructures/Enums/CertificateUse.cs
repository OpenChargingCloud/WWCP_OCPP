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

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Extensions methods for certificate uses.
    /// </summary>
    public static class CertificateUseExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a certificate use.
        /// </summary>
        /// <param name="Text">A text representation of a certificate use.</param>
        public static CertificateUse Parse(String Text)
        {

            if (TryParse(Text, out var certificateUse))
                return certificateUse;

            return CertificateUse.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a certificate use.
        /// </summary>
        /// <param name="Text">A text representation of a certificate use.</param>
        public static CertificateUse? TryParse(String Text)
        {

            if (TryParse(Text, out var certificateUse))
                return certificateUse;

            return null;

        }

        #endregion

        #region TryParse(Text, out CertificateUse)

        /// <summary>
        /// Try to parse the given text as a certificate use.
        /// </summary>
        /// <param name="Text">A text representation of a certificate use.</param>
        /// <param name="CertificateUse">The parsed certificate use.</param>
        public static Boolean TryParse(String Text, out CertificateUse CertificateUse)
        {
            switch (Text.Trim())
            {

                case "V2GRootCertificate":
                    CertificateUse = CertificateUse.V2GRootCertificate;
                    return true;

                case "MORootCertificate":
                    CertificateUse = CertificateUse.MORootCertificate;
                    return true;

                case "CSMSRootCertificate":
                    CertificateUse = CertificateUse.CSMSRootCertificate;
                    return true;

                case "V2GCertificateChain":
                    CertificateUse = CertificateUse.V2GCertificateChain;
                    return true;

                default:
                    CertificateUse = CertificateUse.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this CertificateUse)

        public static String AsText(this CertificateUse CertificateUse)

            => CertificateUse switch {
                   CertificateUse.V2GRootCertificate   => "V2GRootCertificate",
                   CertificateUse.MORootCertificate    => "MORootCertificate",
                   CertificateUse.CSMSRootCertificate  => "CSMSRootCertificate",
                   CertificateUse.V2GCertificateChain  => "V2GCertificateChain",
                   _                                   => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Certificate uses.
    /// </summary>
    public enum CertificateUse
    {

        /// <summary>
        /// Unknown certificate use status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Use for certificate of the V2G Root.
        /// </summary>
        V2GRootCertificate,

        /// <summary>
        /// Use for certificate from an e-mobility service provider.
        /// To support PnC charging with contracts from service providers that not derived their certificates from the V2G root.
        /// </summary>
        MORootCertificate,

        /// <summary>
        /// Root certificate for verification of the CSMS certificate.
        /// </summary>
        CSMSRootCertificate,

        /// <summary>
        /// ISO 15118 V2G certificate chain (excluding the V2GRootCertificate).
        /// </summary>
        V2GCertificateChain

    }

}
