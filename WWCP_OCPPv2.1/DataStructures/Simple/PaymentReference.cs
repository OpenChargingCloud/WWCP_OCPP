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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for payment references.
    /// </summary>
    public static class PaymentReferenceExtensions
    {

        /// <summary>
        /// Indicates whether this payment reference is null or empty.
        /// </summary>
        /// <param name="PaymentReference">A payment reference.</param>
        public static Boolean IsNullOrEmpty(this PaymentReference? PaymentReference)
            => !PaymentReference.HasValue || PaymentReference.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this payment reference is null or empty.
        /// </summary>
        /// <param name="PaymentReference">A payment reference.</param>
        public static Boolean IsNotNullOrEmpty(this PaymentReference? PaymentReference)
            => PaymentReference.HasValue && PaymentReference.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The payment reference.
    /// </summary>
    public readonly struct PaymentReference : IId,
                                              IEquatable<PaymentReference>,
                                              IComparable<PaymentReference>
    {

        #region Data

        /// <summary>
        /// The nummeric value of the payment reference.
        /// </summary>
        public readonly String Value;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => Value.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => Value.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the payment reference.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) Value.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new payment reference based on the given text.
        /// </summary>
        /// <param name="Text">A text representation of a payment reference.</param>
        private PaymentReference(String Text)
        {
            this.Value = Text;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random payment reference.
        /// </summary>
        public static PaymentReference NewRandom

            => new (RandomExtensions.RandomString(36));

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a payment reference.
        /// </summary>
        /// <param name="Text">A text representation of a payment reference.</param>
        public static PaymentReference Parse(String Text)
        {

            if (TryParse(Text, out var paymentReference))
                return paymentReference;

            throw new ArgumentException($"Invalid text representation of a payment reference: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a payment reference.
        /// </summary>
        /// <param name="Text">A text representation of a payment reference.</param>
        public static PaymentReference? TryParse(String Text)
        {

            if (TryParse(Text, out var paymentReference))
                return paymentReference;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out PaymentReference)

        /// <summary>
        /// Try to parse the given text as a payment reference.
        /// </summary>
        /// <param name="Text">A text representation of a payment reference.</param>
        /// <param name="PaymentReference">The parsed payment reference.</param>
        public static Boolean TryParse(String Text, out PaymentReference PaymentReference)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                PaymentReference = new PaymentReference(Text);
                return true;
            }

            PaymentReference = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this payment reference.
        /// </summary>
        public PaymentReference Clone()

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (PaymentReference1, PaymentReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentReference1">A payment reference.</param>
        /// <param name="PaymentReference2">Another payment reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PaymentReference PaymentReference1,
                                           PaymentReference PaymentReference2)

            => PaymentReference1.Equals(PaymentReference2);

        #endregion

        #region Operator != (PaymentReference1, PaymentReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentReference1">A payment reference.</param>
        /// <param name="PaymentReference2">Another payment reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PaymentReference PaymentReference1,
                                           PaymentReference PaymentReference2)

            => !PaymentReference1.Equals(PaymentReference2);

        #endregion

        #region Operator <  (PaymentReference1, PaymentReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentReference1">A payment reference.</param>
        /// <param name="PaymentReference2">Another payment reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PaymentReference PaymentReference1,
                                          PaymentReference PaymentReference2)

            => PaymentReference1.CompareTo(PaymentReference2) < 0;

        #endregion

        #region Operator <= (PaymentReference1, PaymentReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentReference1">A payment reference.</param>
        /// <param name="PaymentReference2">Another payment reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PaymentReference PaymentReference1,
                                           PaymentReference PaymentReference2)

            => PaymentReference1.CompareTo(PaymentReference2) <= 0;

        #endregion

        #region Operator >  (PaymentReference1, PaymentReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentReference1">A payment reference.</param>
        /// <param name="PaymentReference2">Another payment reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PaymentReference PaymentReference1,
                                          PaymentReference PaymentReference2)

            => PaymentReference1.CompareTo(PaymentReference2) > 0;

        #endregion

        #region Operator >= (PaymentReference1, PaymentReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentReference1">A payment reference.</param>
        /// <param name="PaymentReference2">Another payment reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PaymentReference PaymentReference1,
                                           PaymentReference PaymentReference2)

            => PaymentReference1.CompareTo(PaymentReference2) >= 0;

        #endregion

        #endregion

        #region IComparable<PaymentReference> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two payment references.
        /// </summary>
        /// <param name="Object">A payment reference to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PaymentReference paymentReference
                   ? CompareTo(paymentReference)
                   : throw new ArgumentException("The given object is not a payment reference!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PaymentReference)

        /// <summary>
        /// Compares two payment references.
        /// </summary>
        /// <param name="PaymentReference">A payment reference to compare with.</param>
        public Int32 CompareTo(PaymentReference PaymentReference)

            => String.Compare(Value,
                              PaymentReference.Value,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<PaymentReference> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two payment references for equality.
        /// </summary>
        /// <param name="Object">A payment reference to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PaymentReference paymentReference &&
                   Equals(paymentReference);

        #endregion

        #region Equals(PaymentReference)

        /// <summary>
        /// Compares two payment references for equality.
        /// </summary>
        /// <param name="PaymentReference">A payment reference to compare with.</param>
        public Boolean Equals(PaymentReference PaymentReference)

            => String.Equals(Value,
                             PaymentReference.Value,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Value.ToString();

        #endregion

    }

}
