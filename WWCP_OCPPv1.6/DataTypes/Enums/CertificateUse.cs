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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extentions methods for the certificate use.
    /// </summary>
    public static class CertificateUseExtentions
    {

        #region Parse(Text)

        public static CertificateUse Parse(String Text)

            => Text.Trim() switch {
                   "CentralSystemRootCertificate"  => CertificateUse.CentralSystemRootCertificate,
                   "ManufacturerRootCertificate"   => CertificateUse.ManufacturerRootCertificate,
                   _                               => CertificateUse.Unknown
               };

        #endregion

        #region AsText(this CertificateUse)

        public static String AsText(this CertificateUse CertificateUse)

            => CertificateUse switch {
                   CertificateUse.CentralSystemRootCertificate  => "CentralSystemRootCertificate",
                   CertificateUse.ManufacturerRootCertificate   => "ManufacturerRootCertificate",
                   _                                            => "unknown"
               };

        #endregion

    }

    /// <summary>
    /// The use of a certificate.
    /// </summary>
    public enum CertificateUse
    {

        /// <summary>
        /// Unknown certificate use status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Root certificate, used by the CA to sign the Central System and Charge Point certificate.
        /// </summary>
        CentralSystemRootCertificate,

        /// <summary>
        /// Root certificate for verification of the Manufacturer certificate.
        /// </summary>
        ManufacturerRootCertificate

    }

}
