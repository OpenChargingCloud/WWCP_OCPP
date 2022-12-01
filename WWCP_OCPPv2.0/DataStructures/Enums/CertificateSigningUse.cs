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
    /// Extensions methods for certificate signing uses.
    /// </summary>
    public static class CertificateSigningUseExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a certificate signing use.
        /// </summary>
        /// <param name="Text">A text representation of a certificate signing use.</param>
        public static CertificateSigningUse Parse(String Text)
        {

            if (TryParse(Text, out var certificateSigningUse))
                return certificateSigningUse;

            return CertificateSigningUse.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a certificate signing use.
        /// </summary>
        /// <param name="Text">A text representation of a certificate signing use.</param>
        public static CertificateSigningUse? TryParse(String Text)
        {

            if (TryParse(Text, out var certificateSigningUse))
                return certificateSigningUse;

            return null;

        }

        #endregion

        #region TryParse(Text, out CertificateSigningUse)

        /// <summary>
        /// Try to parse the given text as a certificate signing use.
        /// </summary>
        /// <param name="Text">A text representation of a certificate signing use.</param>
        /// <param name="CertificateSigningUse">The parsed certificate signing use.</param>
        public static Boolean TryParse(String Text, out CertificateSigningUse CertificateSigningUse)
        {
            switch (Text.Trim())
            {

                case "ChargingStationCertificate":
                    CertificateSigningUse = CertificateSigningUse.ChargingStationCertificate;
                    return true;

                case "V2GCertificate":
                    CertificateSigningUse = CertificateSigningUse.V2GCertificate;
                    return true;

                default:
                    CertificateSigningUse = CertificateSigningUse.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this CertificateSigningUse)

        public static String AsText(this CertificateSigningUse CertificateSigningUse)

            => CertificateSigningUse switch {
                   CertificateSigningUse.ChargingStationCertificate  => "ChargingStationCertificate",
                   CertificateSigningUse.V2GCertificate              => "V2GCertificate",
                   _                                                 => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Certificate signing uses.
    /// </summary>
    public enum CertificateSigningUse
    {

        /// <summary>
        /// Unknown certificate signing use.
        /// </summary>
        Unknown,

        /// <summary>
        /// Client side certificate used by the Charging Station to connect the the CSMS.
        /// </summary>
        ChargingStationCertificate,

        /// <summary>
        /// Use for certificate for 15118 connections.
        /// This means that the certificate should be derived from the V2G root.
        /// </summary>
        V2GCertificate

    }

}
