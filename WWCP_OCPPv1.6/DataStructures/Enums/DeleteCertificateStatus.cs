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
    /// Extensions methods for the delete certificate status.
    /// </summary>
    public static class DeleteCertificateStatusExtensions
    {

        #region Parse(Text)

        public static DeleteCertificateStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"  => DeleteCertificateStatus.Accepted,
                   "Failed"    => DeleteCertificateStatus.Failed,
                   "NotFound"  => DeleteCertificateStatus.NotFound,
                   _           => DeleteCertificateStatus.Unknown
               };

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
    /// The status in a response to a delete certificate request.
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
