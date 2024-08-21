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
    /// Extensions methods for tariff status.
    /// </summary>
    public static class TariffStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a tariff status.
        /// </summary>
        /// <param name="Text">A text representation of a tariff status.</param>
        public static TariffStatus Parse(String Text)
        {

            if (TryParse(Text, out var tariffStatus))
                return tariffStatus;

            return TariffStatus.Rejected;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a tariff status.
        /// </summary>
        /// <param name="Text">A text representation of a tariff status.</param>
        public static TariffStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var tariffStatus))
                return tariffStatus;

            return null;

        }

        #endregion

        #region TryParse(Text, out TariffStatus)

        /// <summary>
        /// Try to parse the given text as a tariff status.
        /// </summary>
        /// <param name="Text">A text representation of a tariff status.</param>
        /// <param name="TariffStatus">The parsed tariff status.</param>
        public static Boolean TryParse(String Text, out TariffStatus TariffStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    TariffStatus = TariffStatus.Accepted;
                    return true;

                case "TooManyElements":
                    TariffStatus = TariffStatus.TooManyElements;
                    return true;

                case "ConditionNotSupported":
                    TariffStatus = TariffStatus.ConditionNotSupported;
                    return true;

                case "NoTariff":
                    TariffStatus = TariffStatus.NoTariff;
                    return true;

                case "TariffInUse":
                    TariffStatus = TariffStatus.TariffInUse;
                    return true;

                case "TxNotFound":
                    TariffStatus = TariffStatus.TxNotFound;
                    return true;

                case "NoCurrencyChange":
                    TariffStatus = TariffStatus.NoCurrencyChange;
                    return true;

                default:
                    TariffStatus = TariffStatus.Rejected;
                    return false;

            }
        }

        #endregion

        #region AsText(this TariffStatus)

        public static String AsText(this TariffStatus TariffStatus)

            => TariffStatus switch {
                   TariffStatus.Accepted               => "Accepted",
                   TariffStatus.TooManyElements        => "TooManyElements",
                   TariffStatus.ConditionNotSupported  => "ConditionNotSupported",
                   TariffStatus.NoTariff               => "NoTariff",
                   TariffStatus.TariffInUse            => "TariffInUse",
                   TariffStatus.TxNotFound             => "TxNotFound",
                   TariffStatus.NoCurrencyChange       => "NoCurrencyChange",
                   _                                   => "Rejected"
               };

        #endregion

    }


    /// <summary>
    /// TariffStatus types.
    /// </summary>
    public enum TariffStatus
    {

        /// <summary>
        /// The tariff has been accepted.
        /// </summary>
        Accepted,

        /// <summary>
        /// The tariff has been rejected. More info in statusInfo.
        /// </summary>
        Rejected,

        /// <summary>
        /// The tariff has too many elements and cannot be processed.
        /// </summary>
        TooManyElements,

        /// <summary>
        /// A condition is not supported, or conditions are not supported at all.
        /// </summary>
        ConditionNotSupported,

        /// <summary>
        /// No tariff for EVSE of IdToken (ClearDefault/UserTariff).
        /// </summary>
        NoTariff,

        /// <summary>
        /// Tariff is currently in use (ClearDefault/UserTariff).
        /// </summary>
        TariffInUse,

        /// <summary>
        /// Transaction does not exist or has already ended (ChangeTransactionTariff).
        /// </summary>
        TxNotFound,

        /// <summary>
        /// Cannot change currency during a transaction (ChangeTransactionTariff).
        /// </summary>
        NoCurrencyChange

    }

}
