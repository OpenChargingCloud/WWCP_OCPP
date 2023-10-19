/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for currencies.
    /// </summary>
    public static class CurrencyExtensions
    {

        /// <summary>
        /// Indicates whether this currency is null or empty.
        /// </summary>
        /// <param name="Currency">A currency.</param>
        public static Boolean IsNullOrEmpty(this Currency? Currency)
            => !Currency.HasValue || Currency.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this currency is NOT null or empty.
        /// </summary>
        /// <param name="Currency">A currency.</param>
        public static Boolean IsNotNullOrEmpty(this Currency? Currency)
            => Currency.HasValue && Currency.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The ISO 4217 code of a currency.
    /// </summary>
    public readonly struct Currency : IId<Currency>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this currency is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this currency is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the currency.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ISO 4217 code of a currency based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a currency.</param>
        private Currency(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a currency.
        /// </summary>
        /// <param name="Text">A text representation of a currency.</param>
        public static Currency Parse(String Text)
        {

            if (TryParse(Text, out var currency))
                return currency;

            throw new ArgumentException($"Invalid text representation of a currency: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a currency.
        /// </summary>
        /// <param name="Text">A text representation of a currency.</param>
        public static Currency? TryParse(String Text)
        {

            if (TryParse(Text, out var currency))
                return currency;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out Currency)

        /// <summary>
        /// Try to parse the given text as a currency.
        /// </summary>
        /// <param name="Text">A text representation of a currency.</param>
        /// <param name="Currency">The parsed currency.</param>
        public static Boolean TryParse(String Text, out Currency Currency)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    Currency = new Currency(Text);
                    return true;
                }
                catch
                { }
            }

            Currency = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this currency.
        /// </summary>
        public Currency Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// EUR, €.
        /// </summary>
        public static Currency EUR
            => new ("EUR");

        #endregion


        #region Operator overloading

        #region Operator == (Currency1, Currency2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Currency1">A currency.</param>
        /// <param name="Currency2">Another currency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Currency Currency1,
                                           Currency Currency2)

            => Currency1.Equals(Currency2);

        #endregion

        #region Operator != (Currency1, Currency2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Currency1">A currency.</param>
        /// <param name="Currency2">Another currency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Currency Currency1,
                                           Currency Currency2)

            => !Currency1.Equals(Currency2);

        #endregion

        #region Operator <  (Currency1, Currency2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Currency1">A currency.</param>
        /// <param name="Currency2">Another currency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Currency Currency1,
                                          Currency Currency2)

            => Currency1.CompareTo(Currency2) < 0;

        #endregion

        #region Operator <= (Currency1, Currency2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Currency1">A currency.</param>
        /// <param name="Currency2">Another currency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Currency Currency1,
                                           Currency Currency2)

            => Currency1.CompareTo(Currency2) <= 0;

        #endregion

        #region Operator >  (Currency1, Currency2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Currency1">A currency.</param>
        /// <param name="Currency2">Another currency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Currency Currency1,
                                          Currency Currency2)

            => Currency1.CompareTo(Currency2) > 0;

        #endregion

        #region Operator >= (Currency1, Currency2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Currency1">A currency.</param>
        /// <param name="Currency2">Another currency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Currency Currency1,
                                           Currency Currency2)

            => Currency1.CompareTo(Currency2) >= 0;

        #endregion

        #endregion

        #region IComparable<Currency> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two currencies.
        /// </summary>
        /// <param name="Object">A currency to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Currency currency
                   ? CompareTo(currency)
                   : throw new ArgumentException("The given object is not a currency!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Currency)

        /// <summary>
        /// Compares two currencies.
        /// </summary>
        /// <param name="Currency">A currency to compare with.</param>
        public Int32 CompareTo(Currency Currency)

            => String.Compare(InternalId,
                              Currency.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<Currency> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two currencies for equality.
        /// </summary>
        /// <param name="Object">A currency to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Currency currency &&
                   Equals(currency);

        #endregion

        #region Equals(Currency)

        /// <summary>
        /// Compares two currencies for equality.
        /// </summary>
        /// <param name="Currency">A currency to compare with.</param>
        public Boolean Equals(Currency Currency)

            => String.Equals(InternalId,
                             Currency.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
