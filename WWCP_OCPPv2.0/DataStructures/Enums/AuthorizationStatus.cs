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
    /// Extentions methods for the authorization status.
    /// </summary>
    public static class AuthorizationStatusExtentions
    {

        #region Parse(Text)

        public static AuthorizationStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"            => AuthorizationStatus.Accepted,
                   "Blocked"             => AuthorizationStatus.Blocked,
                   "ConcurrentTx"        => AuthorizationStatus.ConcurrentTx,
                   "Expired"             => AuthorizationStatus.Expired,
                   "Invalid"             => AuthorizationStatus.Invalid,
                   "NoCredit"            => AuthorizationStatus.NoCredit,
                   "NotAllowedTypeEVSE"  => AuthorizationStatus.NotAllowedTypeEVSE,
                   "NotAtThisLocation"   => AuthorizationStatus.NotAtThisLocation,
                   "NotAtThisTime"       => AuthorizationStatus.NotAtThisTime,
                   _                     => AuthorizationStatus.Unknown
               };

        #endregion

        #region AsText(this AuthorizationStatus)

        public static String AsText(this AuthorizationStatus AuthorizationStatus)

            => AuthorizationStatus switch {
                   AuthorizationStatus.Accepted            => "Accepted",
                   AuthorizationStatus.Blocked             => "Blocked",
                   AuthorizationStatus.ConcurrentTx        => "ConcurrentTx",
                   AuthorizationStatus.Expired             => "Expired",
                   AuthorizationStatus.Invalid             => "Invalid",
                   AuthorizationStatus.NoCredit            => "NoCredit",
                   AuthorizationStatus.NotAllowedTypeEVSE  => "NotAllowedTypeEVSE",
                   AuthorizationStatus.NotAtThisLocation   => "NotAtThisLocation",
                   AuthorizationStatus.NotAtThisTime       => "NotAtThisTime",
                   _                                       => "Unknown"
               };

        #endregion

    }

    /// <summary>
    /// The status in a response to an authorize request.
    /// </summary>
    public enum AuthorizationStatus
    {

        /// <summary>
        /// Unknown authorization status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Identifier is allowed for charging.
        /// </summary>
        Accepted,

        /// <summary>
        /// Identifier has been blocked. Not allowed for charging.
        /// </summary>
        Blocked,

        /// <summary>
        /// Identifier is already involved in another transaction
        /// and multiple transactions are not allowed.
        /// </summary>
        ConcurrentTx,

        /// <summary>
        /// Identifier has expired. Not allowed for charging.
        /// </summary>
        Expired,

        /// <summary>
        /// Identifier is unknown. Not allowed for charging.
        /// </summary>
        Invalid,

        NoCredit,
        NotAllowedTypeEVSE,
        NotAtThisLocation,
        NotAtThisTime

    }

}
