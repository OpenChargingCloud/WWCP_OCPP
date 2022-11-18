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
    /// Extentions methods for a certificate action.
    /// </summary>
    public static class CertificateActionExtentions
    {

        #region Parse(Text)

        public static CertificateAction Parse(String Text)

            => Text.Trim() switch {
                   "Install"  => CertificateAction.Install,
                   "Update"   => CertificateAction.Update,
                   _          => CertificateAction.Unknown
               };

        public static Boolean TryParse(String Text, out CertificateAction Action)
        {
            switch (Text.Trim())
            {

                case "Install":
                    Action = CertificateAction.Install;
                    return true;

                case "Update":
                    Action = CertificateAction.Update;
                    return true;

                default:
                    Action = CertificateAction.Unknown;
                    return false;

            }
        }

        #endregion

        #region AsText(this CertificateAction)

        public static String AsText(this CertificateAction CertificateAction)

            => CertificateAction switch {
                   CertificateAction.Install  => "Install",
                   CertificateAction.Update   => "Update",
                   _                          => "Unknown"
               };

        #endregion

    }

    /// <summary>
    /// A certificate action.
    /// </summary>
    public enum CertificateAction
    {

        /// <summary>
        /// Unknown certificate action.
        /// </summary>
        Unknown,

        /// <summary>
        /// Install the provided certificate.
        /// </summary>
        Install,

        /// <summary>
        /// Update the provided certificate.
        /// </summary>
        Update

    }

}
