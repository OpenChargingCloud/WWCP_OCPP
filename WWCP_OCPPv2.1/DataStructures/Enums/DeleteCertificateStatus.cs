/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for delete certificate status.
    /// </summary>
    public static class DeleteCertificateStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a delete certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a delete certificate status.</param>
        public static DeleteCertificateStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return DeleteCertificateStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a delete certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a delete certificate status.</param>
        public static DeleteCertificateStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out DeleteCertificateStatus)

        /// <summary>
        /// Try to parse the given text as a delete certificate status.
        /// </summary>
        /// <param name="Text">A text representation of a delete certificate status.</param>
        /// <param name="DeleteCertificateStatus">The parsed delete certificate status.</param>
        public static Boolean TryParse(String Text, out DeleteCertificateStatus DeleteCertificateStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    DeleteCertificateStatus = DeleteCertificateStatus.Accepted;
                    return true;

                case "Failed":
                    DeleteCertificateStatus = DeleteCertificateStatus.Failed;
                    return true;

                case "NotFound":
                    DeleteCertificateStatus = DeleteCertificateStatus.NotFound;
                    return true;

                default:
                    DeleteCertificateStatus = DeleteCertificateStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this DeleteCertificateStatus)

        public static String AsText(this DeleteCertificateStatus DeleteCertificateStatus)

            => DeleteCertificateStatus switch {
                   DeleteCertificateStatus.Accepted  => "Accepted",
                   DeleteCertificateStatus.Failed    => "Failed",
                   DeleteCertificateStatus.NotFound  => "NotFound",
                   _                                 => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Delete certificate status.
    /// </summary>
    public enum DeleteCertificateStatus
    {

        /// <summary>
        /// Unknown delete certificate status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Normal successful completion (no errors).
        /// </summary>
        Accepted,

        /// <summary>
        /// Processing failure.
        /// </summary>
        Failed,

        /// <summary>
        /// Requested resource not found.
        /// </summary>
        NotFound

    }

}
