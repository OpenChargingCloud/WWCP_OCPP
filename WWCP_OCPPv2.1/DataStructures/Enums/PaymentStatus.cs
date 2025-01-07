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
    /// Extensions methods for payment status.
    /// </summary>
    public static class PaymentStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a payment status.
        /// </summary>
        /// <param name="Text">A text representation of a payment status.</param>
        public static PaymentStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return PaymentStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a payment status.
        /// </summary>
        /// <param name="Text">A text representation of a payment status.</param>
        public static PaymentStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out PaymentStatus)

        /// <summary>
        /// Try to parse the given text as a payment status.
        /// </summary>
        /// <param name="Text">A text representation of a payment status.</param>
        /// <param name="PaymentStatus">The parsed payment status.</param>
        public static Boolean TryParse(String Text, out PaymentStatus PaymentStatus)
        {
            switch (Text.Trim())
            {

                case "Settled":
                    PaymentStatus = PaymentStatus.Settled;
                    return true;

                case "Cancelled":
                    PaymentStatus = PaymentStatus.Cancelled;
                    return true;

                case "Rejected":
                    PaymentStatus = PaymentStatus.Rejected;
                    return true;

                case "Failed":
                    PaymentStatus = PaymentStatus.Failed;
                    return true;

                default:
                    PaymentStatus = PaymentStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this PaymentStatus)

        /// <summary>
        /// Return a string representation of the given payment status.
        /// </summary>
        /// <param name="PaymentStatus">A payment status.</param>
        public static String AsText(this PaymentStatus PaymentStatus)

            => PaymentStatus switch {
                   PaymentStatus.Settled    => "Settled",
                   PaymentStatus.Cancelled  => "Cancelled",
                   PaymentStatus.Rejected   => "Rejected",
                   PaymentStatus.Failed     => "Failed",
                   _                        => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Payment status.
    /// </summary>
    public enum PaymentStatus
    {

        /// <summary>
        /// Unknown payment status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Settled successfully by the PSP.
        /// </summary>
        Settled,

        /// <summary>
        /// No billable part of the OCPP transaction, cancellation sent to the PSP.
        /// </summary>
        Cancelled,

        /// <summary>
        /// Rejected by the PSP.
        /// </summary>
        Rejected,

        /// <summary>
        /// After the final attempt that fails due to communication.
        /// </summary>
        Failed

    }

}
