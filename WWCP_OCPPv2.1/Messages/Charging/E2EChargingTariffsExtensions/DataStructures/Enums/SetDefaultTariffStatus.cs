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
    /// Extensions methods for set default charging tariff status.
    /// </summary>
    public static class SetDefaultE2EChargingTariffStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a set default charging tariff status.
        /// </summary>
        /// <param name="Text">A text representation of a set default charging tariff status.</param>
        public static SetDefaultE2EChargingTariffStatus Parse(String Text)
        {

            if (TryParse(Text, out var setDefaultChargingTariffStatus))
                return setDefaultChargingTariffStatus;

            return SetDefaultE2EChargingTariffStatus.Rejected;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a set default charging tariff status.
        /// </summary>
        /// <param name="Text">A text representation of a set default charging tariff status.</param>
        public static SetDefaultE2EChargingTariffStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var setDefaultChargingTariffStatus))
                return setDefaultChargingTariffStatus;

            return null;

        }

        #endregion

        #region TryParse(Text, out SetDefaultE2EChargingTariffStatus)

        /// <summary>
        /// Try to parse the given text as a set default charging tariff status.
        /// </summary>
        /// <param name="Text">A text representation of a set default charging tariff status.</param>
        /// <param name="SetDefaultE2EChargingTariffStatus">The parsed set default charging tariff status.</param>
        public static Boolean TryParse(String Text, out SetDefaultE2EChargingTariffStatus SetDefaultE2EChargingTariffStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    SetDefaultE2EChargingTariffStatus = SetDefaultE2EChargingTariffStatus.Accepted;
                    return true;

                case "TooLarge":
                    SetDefaultE2EChargingTariffStatus = SetDefaultE2EChargingTariffStatus.TooLarge;
                    return true;

                default:
                    SetDefaultE2EChargingTariffStatus = SetDefaultE2EChargingTariffStatus.Rejected;
                    return false;

            }
        }

        #endregion

        #region AsText(this SetDefaultE2EChargingTariffStatus)

        public static String AsText(this SetDefaultE2EChargingTariffStatus SetDefaultE2EChargingTariffStatus)

            => SetDefaultE2EChargingTariffStatus switch {
                   SetDefaultE2EChargingTariffStatus.Accepted  => "Accepted",
                   SetDefaultE2EChargingTariffStatus.TooLarge  => "TooLarge",
                   _                                        => "Rejected"
            };

        #endregion

    }


    /// <summary>
    /// SetDefaultE2EChargingTariffStatus types.
    /// </summary>
    public enum SetDefaultE2EChargingTariffStatus
    {

        /// <summary>
        /// The default charging tariff was accepted.
        /// </summary>
        Accepted,

        /// <summary>
        /// The default charging tariff was rejected.
        /// </summary>
        Rejected,

        /// <summary>
        /// The default charging tariff is too large to handle.
        /// </summary>
        TooLarge,

        /// <summary>
        /// The signature(s) of the default charging tariff is invalid.
        /// </summary>
        InvalidSignature,

        /// <summary>
        /// The signature chain(s) of the default charging tariff are invalid.
        /// </summary>
        InvalidSignatureChain

    }

}
