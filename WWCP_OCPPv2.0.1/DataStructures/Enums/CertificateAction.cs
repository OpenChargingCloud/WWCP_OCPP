/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extensions methods for certificate actions.
    /// </summary>
    public static class CertificateActionExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a certificate action.
        /// </summary>
        /// <param name="Text">A text representation of a certificate action.</param>
        public static CertificateAction Parse(String Text)
        {

            if (TryParse(Text, out var action))
                return action;

            return CertificateAction.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a certificate action.
        /// </summary>
        /// <param name="Text">A text representation of a certificate action.</param>
        public static CertificateAction? TryParse(String Text)
        {

            if (TryParse(Text, out var action))
                return action;

            return null;

        }

        #endregion

        #region TryParse(Text, out CertificateAction)

        /// <summary>
        /// Try to parse the given text as a certificate action.
        /// </summary>
        /// <param name="Text">A text representation of a certificate action.</param>
        /// <param name="CertificateAction">The parsed certificate action.</param>
        public static Boolean TryParse(String Text, out CertificateAction CertificateAction)
        {
            switch (Text.Trim())
            {

                case "Install":
                    CertificateAction = CertificateAction.Install;
                    return true;

                case "Update":
                    CertificateAction = CertificateAction.Update;
                    return true;

                default:
                    CertificateAction = CertificateAction.Unknown;
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
    /// Certificate actions.
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
