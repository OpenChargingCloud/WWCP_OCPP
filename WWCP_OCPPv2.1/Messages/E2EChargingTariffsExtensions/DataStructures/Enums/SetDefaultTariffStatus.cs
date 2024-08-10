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
    /// Extensions methods for set default charging tariff status.
    /// </summary>
    public static class SetDefaultChargingTariffStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a set default charging tariff status.
        /// </summary>
        /// <param name="Text">A text representation of a set default charging tariff status.</param>
        public static SetDefaultChargingTariffStatus Parse(String Text)
        {

            if (TryParse(Text, out var setDefaultChargingTariffStatus))
                return setDefaultChargingTariffStatus;

            return SetDefaultChargingTariffStatus.Rejected;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a set default charging tariff status.
        /// </summary>
        /// <param name="Text">A text representation of a set default charging tariff status.</param>
        public static SetDefaultChargingTariffStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var setDefaultChargingTariffStatus))
                return setDefaultChargingTariffStatus;

            return null;

        }

        #endregion

        #region TryParse(Text, out SetDefaultChargingTariffStatus)

        /// <summary>
        /// Try to parse the given text as a set default charging tariff status.
        /// </summary>
        /// <param name="Text">A text representation of a set default charging tariff status.</param>
        /// <param name="SetDefaultChargingTariffStatus">The parsed set default charging tariff status.</param>
        public static Boolean TryParse(String Text, out SetDefaultChargingTariffStatus SetDefaultChargingTariffStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    SetDefaultChargingTariffStatus = SetDefaultChargingTariffStatus.Accepted;
                    return true;

                case "TooLarge":
                    SetDefaultChargingTariffStatus = SetDefaultChargingTariffStatus.TooLarge;
                    return true;

                default:
                    SetDefaultChargingTariffStatus = SetDefaultChargingTariffStatus.Rejected;
                    return false;

            }
        }

        #endregion

        #region AsText(this SetDefaultChargingTariffStatus)

        public static String AsText(this SetDefaultChargingTariffStatus SetDefaultChargingTariffStatus)

            => SetDefaultChargingTariffStatus switch {
                   SetDefaultChargingTariffStatus.Accepted  => "Accepted",
                   SetDefaultChargingTariffStatus.TooLarge  => "TooLarge",
                   _                                        => "Rejected"
            };

        #endregion

    }


    /// <summary>
    /// SetDefaultChargingTariffStatus types.
    /// </summary>
    public enum SetDefaultChargingTariffStatus
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
