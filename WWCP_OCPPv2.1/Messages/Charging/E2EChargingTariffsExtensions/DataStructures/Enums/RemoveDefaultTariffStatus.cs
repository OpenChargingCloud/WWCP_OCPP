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
    /// Extensions methods for remove default charging tariff status.
    /// </summary>
    public static class RemoveDefaultChargingTariffStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a remove default charging tariff status.
        /// </summary>
        /// <param name="Text">A text representation of a remove default charging tariff status.</param>
        public static RemoveDefaultChargingTariffStatus Parse(String Text)
        {

            if (TryParse(Text, out var removeDefaultChargingTariffStatus))
                return removeDefaultChargingTariffStatus;

            return RemoveDefaultChargingTariffStatus.Rejected;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a remove default charging tariff status.
        /// </summary>
        /// <param name="Text">A text representation of a remove default charging tariff status.</param>
        public static RemoveDefaultChargingTariffStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var removeDefaultChargingTariffStatus))
                return removeDefaultChargingTariffStatus;

            return null;

        }

        #endregion

        #region TryParse(Text, out RemoveDefaultChargingTariffStatus)

        /// <summary>
        /// Try to parse the given text as a remove default charging tariff status.
        /// </summary>
        /// <param name="Text">A text representation of a remove default charging tariff status.</param>
        /// <param name="RemoveDefaultChargingTariffStatus">The parsed remove default charging tariff status.</param>
        public static Boolean TryParse(String Text, out RemoveDefaultChargingTariffStatus RemoveDefaultChargingTariffStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    RemoveDefaultChargingTariffStatus = RemoveDefaultChargingTariffStatus.Accepted;
                    return true;

                case "NotFound":
                    RemoveDefaultChargingTariffStatus = RemoveDefaultChargingTariffStatus.NotFound;
                    return true;

                default:
                    RemoveDefaultChargingTariffStatus = RemoveDefaultChargingTariffStatus.Rejected;
                    return false;

            }
        }

        #endregion

        #region AsText(this RemoveDefaultChargingTariffStatus)

        public static String AsText(this RemoveDefaultChargingTariffStatus RemoveDefaultChargingTariffStatus)

            => RemoveDefaultChargingTariffStatus switch {
                   RemoveDefaultChargingTariffStatus.Accepted  => "Accepted",
                   RemoveDefaultChargingTariffStatus.NotFound  => "NotFound",
                   _                                           => "Rejected"
            };

        #endregion

    }


    /// <summary>
    /// RemoveDefaultChargingTariffStatus types.
    /// </summary>
    public enum RemoveDefaultChargingTariffStatus
    {

        /// <summary>
        /// The default charging tariff was removed.
        /// </summary>
        Accepted,

        /// <summary>
        /// The default charging tariff could not be removed.
        /// </summary>
        Rejected,

        /// <summary>
        /// No default charging tariff found.
        /// </summary>
        NotFound,

        /// <summary>
        /// The found charging tariff identification does not match its expected value.
        /// </summary>
        ChargingTariffIdMismatch

    }

}
