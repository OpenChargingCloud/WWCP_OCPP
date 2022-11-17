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
    /// Extentions methods for the certificate signing use.
    /// </summary>
    public static class CertificateSigningUseExtentions
    {

        #region Parse(Text)

        public static CertificateSigningUse Parse(String Text)

            => Text.Trim() switch {
                   "ChargingStationCertificate"  => CertificateSigningUse.ChargingStationCertificate,
                   "V2GCertificate"              => CertificateSigningUse.V2GCertificate,
                   _                             => CertificateSigningUse.Unknown
               };

        #endregion

        #region AsText(this CertificateSigningUse)

        public static String AsText(this CertificateSigningUse CertificateSigningUse)

            => CertificateSigningUse switch {
                   CertificateSigningUse.ChargingStationCertificate  => "ChargingStationCertificate",
                   CertificateSigningUse.V2GCertificate              => "V2GCertificate",
                   _                                                 => "unknown"
               };

        #endregion

    }

    /// <summary>
    /// The signing use of a certificate.
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
