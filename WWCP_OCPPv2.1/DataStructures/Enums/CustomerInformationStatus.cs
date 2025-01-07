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
    /// Extensions methods for customer information status.
    /// </summary>
    public static class CustomerInformationStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a customer information status.
        /// </summary>
        /// <param name="Text">A text representation of a customer information status.</param>
        public static CustomerInformationStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return CustomerInformationStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a customer information status.
        /// </summary>
        /// <param name="Text">A text representation of a customer information status.</param>
        public static CustomerInformationStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out CustomerInformationStatus)

        /// <summary>
        /// Try to parse the given text as a customer information status.
        /// </summary>
        /// <param name="Text">A text representation of a customer information status.</param>
        /// <param name="CustomerInformationStatus">The parsed customer information status.</param>
        public static Boolean TryParse(String Text, out CustomerInformationStatus CustomerInformationStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    CustomerInformationStatus = CustomerInformationStatus.Accepted;
                    return true;

                case "Rejected":
                    CustomerInformationStatus = CustomerInformationStatus.Rejected;
                    return true;

                case "Invalid":
                    CustomerInformationStatus = CustomerInformationStatus.Rejected;
                    return true;

                default:
                    CustomerInformationStatus = CustomerInformationStatus.Unknown;
                    return false;

            }
        }

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
    /// Customer information status.
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
