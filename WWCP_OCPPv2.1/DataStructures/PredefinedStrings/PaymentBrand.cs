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

#region Usings

using System.Diagnostics.CodeAnalysis;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for payment brands.
    /// </summary>
    public static class PaymentBrandExtensions
    {

        /// <summary>
        /// Indicates whether this payment brand is null or empty.
        /// </summary>
        /// <param name="PaymentBrand">A payment brand.</param>
        public static Boolean IsNullOrEmpty(this PaymentBrand? PaymentBrand)
            => !PaymentBrand.HasValue || PaymentBrand.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this payment brand is null or empty.
        /// </summary>
        /// <param name="PaymentBrand">A payment brand.</param>
        public static Boolean IsNotNullOrEmpty(this PaymentBrand? PaymentBrand)
            => PaymentBrand.HasValue && PaymentBrand.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A payment brand.
    /// </summary>
    public readonly struct PaymentBrand : IId,
                                          IEquatable<PaymentBrand>,
                                          IComparable<PaymentBrand>
    {

        #region Data

        private readonly static Dictionary<String, PaymentBrand>  lookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly        String                             InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this payment brand is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this payment brand is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the payment brand.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new payment brand based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a payment brand.</param>
        private PaymentBrand(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static PaymentBrand Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new PaymentBrand(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a payment brand.
        /// </summary>
        /// <param name="Text">A text representation of a payment brand.</param>
        public static PaymentBrand Parse(String Text)
        {

            if (TryParse(Text, out var paymentBrand))
                return paymentBrand;

            throw new ArgumentException($"Invalid text representation of a payment brand: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as payment brand.
        /// </summary>
        /// <param name="Text">A text representation of a payment brand.</param>
        public static PaymentBrand? TryParse(String Text)
        {

            if (TryParse(Text, out var paymentBrand))
                return paymentBrand;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out PaymentBrand)

        /// <summary>
        /// Try to parse the given text as payment brand.
        /// </summary>
        /// <param name="Text">A text representation of a payment brand.</param>
        /// <param name="PaymentBrand">The parsed payment brand.</param>
        public static Boolean TryParse(String                                Text,
                                       [NotNullWhen(true)] out PaymentBrand  PaymentBrand)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out PaymentBrand))
                    PaymentBrand = Register(Text);

                return true;

            }

            PaymentBrand = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this payment brand.
        /// </summary>
        public PaymentBrand Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (PaymentBrand1, PaymentBrand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentBrand1">A payment brand.</param>
        /// <param name="PaymentBrand2">Another payment brand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PaymentBrand PaymentBrand1,
                                           PaymentBrand PaymentBrand2)

            => PaymentBrand1.Equals(PaymentBrand2);

        #endregion

        #region Operator != (PaymentBrand1, PaymentBrand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentBrand1">A payment brand.</param>
        /// <param name="PaymentBrand2">Another payment brand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PaymentBrand PaymentBrand1,
                                           PaymentBrand PaymentBrand2)

            => !PaymentBrand1.Equals(PaymentBrand2);

        #endregion

        #region Operator <  (PaymentBrand1, PaymentBrand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentBrand1">A payment brand.</param>
        /// <param name="PaymentBrand2">Another payment brand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PaymentBrand PaymentBrand1,
                                          PaymentBrand PaymentBrand2)

            => PaymentBrand1.CompareTo(PaymentBrand2) < 0;

        #endregion

        #region Operator <= (PaymentBrand1, PaymentBrand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentBrand1">A payment brand.</param>
        /// <param name="PaymentBrand2">Another payment brand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PaymentBrand PaymentBrand1,
                                           PaymentBrand PaymentBrand2)

            => PaymentBrand1.CompareTo(PaymentBrand2) <= 0;

        #endregion

        #region Operator >  (PaymentBrand1, PaymentBrand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentBrand1">A payment brand.</param>
        /// <param name="PaymentBrand2">Another payment brand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PaymentBrand PaymentBrand1,
                                          PaymentBrand PaymentBrand2)

            => PaymentBrand1.CompareTo(PaymentBrand2) > 0;

        #endregion

        #region Operator >= (PaymentBrand1, PaymentBrand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentBrand1">A payment brand.</param>
        /// <param name="PaymentBrand2">Another payment brand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PaymentBrand PaymentBrand1,
                                           PaymentBrand PaymentBrand2)

            => PaymentBrand1.CompareTo(PaymentBrand2) >= 0;

        #endregion

        #endregion

        #region IComparable<PaymentBrand> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two payment brands.
        /// </summary>
        /// <param name="Object">A payment brand to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PaymentBrand paymentBrand
                   ? CompareTo(paymentBrand)
                   : throw new ArgumentException("The given object is not payment brand!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PaymentBrand)

        /// <summary>
        /// Compares two payment brands.
        /// </summary>
        /// <param name="PaymentBrand">A payment brand to compare with.</param>
        public Int32 CompareTo(PaymentBrand PaymentBrand)

            => String.Compare(InternalId,
                              PaymentBrand.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<PaymentBrand> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two payment brands for equality.
        /// </summary>
        /// <param name="Object">A payment brand to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PaymentBrand paymentBrand &&
                   Equals(paymentBrand);

        #endregion

        #region Equals(PaymentBrand)

        /// <summary>
        /// Compares two payment brands for equality.
        /// </summary>
        /// <param name="PaymentBrand">A payment brand to compare with.</param>
        public Boolean Equals(PaymentBrand PaymentBrand)

            => String.Equals(InternalId,
                             PaymentBrand.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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
