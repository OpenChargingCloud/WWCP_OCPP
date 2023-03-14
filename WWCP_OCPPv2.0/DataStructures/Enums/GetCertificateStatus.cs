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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extensions methods for get certificate status.
    /// </summary>
    public static class GetCertificateStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a get certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a get certificate status.</param>
        public static GetCertificateStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return GetCertificateStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a get certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a get certificate status.</param>
        public static GetCertificateStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out GetCertificateStatus)

        /// <summary>
        /// Try to parse the given text as a get certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a get certificate status.</param>
        /// <param name="GetCertificateStatus">The parsed get certificate status.</param>
        public static Boolean TryParse(String Text, out GetCertificateStatus GetCertificateStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    GetCertificateStatus = GetCertificateStatus.Accepted;
                    return true;

                case "Failed":
                    GetCertificateStatus = GetCertificateStatus.Failed;
                    return true;

                default:
                    GetCertificateStatus = GetCertificateStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this GetCertificateStatus)

        public static String AsText(this GetCertificateStatus GetCertificateStatus)

            => GetCertificateStatus switch {
                   GetCertificateStatus.Accepted  => "Accepted",
                   GetCertificateStatus.Failed    => "Failed",
                   _                              => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Get certificate status.
    /// </summary>
    public enum GetCertificateStatus
    {

        /// <summary>
        /// Unknown get certificate status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Successfully retrieved the OCSP certificate status.
        /// </summary>
        Accepted,

        /// <summary>
        /// Failed to retrieve the OCSP certificate status.
        /// </summary>
        Failed

    }

}
