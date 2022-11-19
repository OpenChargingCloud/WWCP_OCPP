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

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an authorization status.
        /// </summary>
        /// <param name="Text">A text representation of an authorization status.</param>
        public static AuthorizationStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return AuthorizationStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an authorization status.
        /// </summary>
        /// <param name="Text">A text representation of an authorization status.</param>
        public static AuthorizationStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out Status)

        /// <summary>
        /// Try to parse the given text as an authorization status.
        /// </summary>
        /// <param name="Text">A text representation of an authorization status.</param>
        /// <param name="Status">The parsed authorization status.</param>
        public static Boolean TryParse(String Text, out AuthorizationStatus Status)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    Status = AuthorizationStatus.Accepted;
                    return true;

                case "Blocked":
                    Status = AuthorizationStatus.Blocked;
                    return true;

                case "ConcurrentTx":
                    Status = AuthorizationStatus.ConcurrentTx;
                    return true;

                case "Expired":
                    Status = AuthorizationStatus.Expired;
                    return true;

                case "Invalid":
                    Status = AuthorizationStatus.Invalid;
                    return true;

                case "NoCredit":
                    Status = AuthorizationStatus.NoCredit;
                    return true;

                case "NotAllowedTypeEVSE":
                    Status = AuthorizationStatus.NotAllowedTypeEVSE;
                    return true;

                case "NotAtThisLocation":
                    Status = AuthorizationStatus.NotAtThisLocation;
                    return true;

                case "NotAtThisTime":
                    Status = AuthorizationStatus.NotAtThisTime;
                    return true;

                default:
                    Status = AuthorizationStatus.Unknown;
                    return false;

            }
        }

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
