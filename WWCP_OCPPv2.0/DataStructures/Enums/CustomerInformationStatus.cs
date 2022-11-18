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
    /// Extentions methods for customer information status.
    /// </summary>
    public static class CustomerInformationStatusExtentions
    {

        #region Parse(Text)

        public static CustomerInformationStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"  => CustomerInformationStatus.Accepted,
                   "Rejected"  => CustomerInformationStatus.Rejected,
                   "Invalid"   => CustomerInformationStatus.Invalid,
                   _           => CustomerInformationStatus.Unknown
               };

        #endregion

        #region AsText(this CustomerInformationStatus)

        public static String AsText(this CustomerInformationStatus CustomerInformationStatus)

            => CustomerInformationStatus switch {
                   CustomerInformationStatus.Accepted  => "Accepted",
                   CustomerInformationStatus.Rejected  => "Rejected",
                   CustomerInformationStatus.Invalid   => "Invalid",
                   _                                   => "Unknown"
               };

        #endregion

    }

    /// <summary>
    /// The customer information status.
    /// </summary>
    public enum CustomerInformationStatus
    {

        /// <summary>
        /// Unknown customer information status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The charging station accepted the message.
        /// </summary>
        Accepted,

        /// <summary>
        /// When the charging station is in a state where it cannot process this request
        /// </summary>
        Rejected,

        /// <summary>
        /// In a request to the charging station no reference to a customer is included.
        /// </summary>
        Invalid

    }

}
