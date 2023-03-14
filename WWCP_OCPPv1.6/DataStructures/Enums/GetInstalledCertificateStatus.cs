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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extensions methods for the the get installed certificate status.
    /// </summary>
    public static class GetInstalledCertificateStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a get installed certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a get installed certificate status.</param>
        public static GetInstalledCertificateStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return GetInstalledCertificateStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a get installed certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a get installed certificate status.</param>
        public static GetInstalledCertificateStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out Status)

        /// <summary>
        /// Try to parse the given text as a get installed certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a get installed certificate status.</param>
        /// <param name="Status">The parsed get installed certificate status.</param>
        public static Boolean TryParse(String Text, out GetInstalledCertificateStatus Status)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    Status = GetInstalledCertificateStatus.Accepted;
                    return true;

                case "NotFound":
                    Status = GetInstalledCertificateStatus.NotFound;
                    return true;

                default:
                    Status = GetInstalledCertificateStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this GetInstalledCertificateStatus)

        public static String AsText(this GetInstalledCertificateStatus GetInstalledCertificateStatus)

            => GetInstalledCertificateStatus switch {
                   GetInstalledCertificateStatus.Accepted  => "Accepted",
                   GetInstalledCertificateStatus.NotFound  => "NotFound",
                   _                                       => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// The get installed certificate status.
    /// </summary>
    public enum GetInstalledCertificateStatus
    {

        /// <summary>
        /// Unknown generic status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Normal successful completion (no errors).
        /// </summary>
        Accepted,

        /// <summary>
        /// Requested certificate not found.
        /// </summary>
        NotFound

    }

}
