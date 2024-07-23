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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for registration status.
    /// </summary>
    public static class RegistrationStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a registration status.
        /// </summary>
        /// <param name="Text">A text representation of a registration status.</param>
        public static RegistrationStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return RegistrationStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a registration status.
        /// </summary>
        /// <param name="Text">A text representation of a registration status.</param>
        public static RegistrationStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out RegistrationStatus)

        /// <summary>
        /// Try to parse the given text as a registration status.
        /// </summary>
        /// <param name="Text">A text representation of a registration status.</param>
        /// <param name="RegistrationStatus">The parsed registration status.</param>
        public static Boolean TryParse(String Text, out RegistrationStatus RegistrationStatus)
        {
            switch (Text.Trim().ToLower())
            {

                case "accepted":
                    RegistrationStatus = RegistrationStatus.Accepted;
                    return true;

                case "pending":
                    RegistrationStatus = RegistrationStatus.Pending;
                    return true;

                case "rejected":
                    RegistrationStatus = RegistrationStatus.Rejected;
                    return true;

                default:
                    RegistrationStatus = RegistrationStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this RegistrationStatus)

        /// <summary>
        /// Return a string representation of the given registration status.
        /// </summary>
        /// <param name="RegistrationStatus">A registration status.</param>
        public static String AsText(this RegistrationStatus RegistrationStatus)

            => RegistrationStatus switch {
                   RegistrationStatus.Accepted  => "Accepted",
                   RegistrationStatus.Pending   => "Pending",
                   RegistrationStatus.Rejected  => "Rejected",
                   _                            => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Registration status.
    /// </summary>
    public enum RegistrationStatus
    {

        /// <summary>
        /// Unknown registration status.
        /// </summary>
        Unknown,


        /// <summary>
        /// Charge point is accepted by the central system.
        /// </summary>
        Accepted,

        /// <summary>
        /// The central system is not yet ready to accept the
        /// charging station. The central system may send messages
        /// to retrieve information or prepare the charging station.
        /// </summary>
        Pending,

        /// <summary>
        /// Charge point is not accepted by the central system.
        /// This may happen when the charging station identification
        /// is not (yet) known by the central system.
        /// </summary>
        Rejected,


        Error,
        SignatureError

    }

}
