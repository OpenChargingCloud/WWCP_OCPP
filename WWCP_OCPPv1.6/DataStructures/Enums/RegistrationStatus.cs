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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extensions methods for the registration status.
    /// </summary>
    public static class RegistrationStatusExtensions
    {

        #region Parse(Text)

        /// <summary>
        /// Parse the given string as registration status.
        /// </summary>
        /// <param name="Text">A string representation of a registration status.</param>
        public static RegistrationStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"  => RegistrationStatus.Accepted,
                   "Pending"   => RegistrationStatus.Pending,
                   "Rejected"  => RegistrationStatus.Rejected,
                   _           => RegistrationStatus.Unknown
               };

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
    /// Result of a registration in response to a BootNotification request.
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
        /// charge point. The central system may send messages
        /// to retrieve information or prepare the charge point.
        /// </summary>
        Pending,

        /// <summary>
        /// Charge point is not accepted by the central system.
        /// This may happen when the charge point identification
        /// is not (yet) known by the central system.
        /// </summary>
        Rejected

    }

}
