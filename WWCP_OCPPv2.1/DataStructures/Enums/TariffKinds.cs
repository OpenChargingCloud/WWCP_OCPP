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

#region Usings

using System.Diagnostics.CodeAnalysis;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for tariff kinds.
    /// </summary>
    public static class TariffKindsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a tariff kind.
        /// </summary>
        /// <param name="Text">A text representation of a tariff kind.</param>
        public static TariffKinds Parse(String Text)
        {

            if (TryParse(Text, out var tariffKind))
                return tariffKind;

            throw new ArgumentException("The given text representation of a tariff kind is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a tariff kind.
        /// </summary>
        /// <param name="Text">A text representation of a tariff kind.</param>
        public static TariffKinds? TryParse(String Text)
        {

            if (TryParse(Text, out var tariffKind))
                return tariffKind;

            return null;

        }

        #endregion

        #region TryParse(Text, out TariffKind)

        /// <summary>
        /// Try to parse the given text as a tariff kind.
        /// </summary>
        /// <param name="Text">A text representation of a tariff kind.</param>
        /// <param name="TariffKind">The parsed Tariff kind.</param>
        public static Boolean TryParse(String                               Text,
                                       [NotNullWhen(true)] out TariffKinds  TariffKind)
        {
            switch (Text.Trim())
            {

                case "UserTariff":
                    TariffKind = TariffKinds.UserTariff;
                    return true;

                case "DefaultTariff":
                    TariffKind = TariffKinds.DefaultTariff;
                    return true;

                default:
                    TariffKind = TariffKinds.DefaultTariff;
                    return false;

            }
        }

        #endregion

        #region AsText(this TariffKind)

        public static String AsText(this TariffKinds TariffKind)

            => TariffKind switch {
                   TariffKinds.UserTariff     => "UserTariff",
                   TariffKinds.DefaultTariff  => "DefaultTariff",
                   _                          => throw new ArgumentException("Invalid tariff kind!", nameof(TariffKind))
               };

        #endregion

    }


    /// <summary>
    /// Tariff kinds.
    /// </summary>
    public enum TariffKinds
    {

        /// <summary>
        /// The default tariff
        /// </summary>
        DefaultTariff,

        /// <summary>
        /// An user-specific tariff
        /// </summary>
        UserTariff

    }

}
