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
    /// Extension methods for payment recognitions.
    /// </summary>
    public static class PaymentRecognitionExtensions
    {

        /// <summary>
        /// Indicates whether this payment recognition is null or empty.
        /// </summary>
        /// <param name="PaymentRecognition">A payment recognition.</param>
        public static Boolean IsNullOrEmpty(this PaymentRecognition? PaymentRecognition)
            => !PaymentRecognition.HasValue || PaymentRecognition.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this payment recognition is null or empty.
        /// </summary>
        /// <param name="PaymentRecognition">A payment recognition.</param>
        public static Boolean IsNotNullOrEmpty(this PaymentRecognition? PaymentRecognition)
            => PaymentRecognition.HasValue && PaymentRecognition.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A payment recognition.
    /// </summary>
    public readonly struct PaymentRecognition : IId,
                                                IEquatable<PaymentRecognition>,
                                                IComparable<PaymentRecognition>
    {

        #region Data

        private readonly static Dictionary<String, PaymentRecognition>  lookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly        String                             InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this payment recognition is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this payment recognition is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the payment recognition.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new payment recognition based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a payment recognition.</param>
        private PaymentRecognition(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static PaymentRecognition Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new PaymentRecognition(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a payment recognition.
        /// </summary>
        /// <param name="Text">A text representation of a payment recognition.</param>
        public static PaymentRecognition Parse(String Text)
        {

            if (TryParse(Text, out var paymentRecognition))
                return paymentRecognition;

            throw new ArgumentException($"Invalid text representation of a payment recognition: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as payment recognition.
        /// </summary>
        /// <param name="Text">A text representation of a payment recognition.</param>
        public static PaymentRecognition? TryParse(String Text)
        {

            if (TryParse(Text, out var paymentRecognition))
                return paymentRecognition;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out PaymentRecognition)

        /// <summary>
        /// Try to parse the given text as payment recognition.
        /// </summary>
        /// <param name="Text">A text representation of a payment recognition.</param>
        /// <param name="PaymentRecognition">The parsed payment recognition.</param>
        public static Boolean TryParse(String                                      Text,
                                       [NotNullWhen(true)] out PaymentRecognition  PaymentRecognition)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out PaymentRecognition))
                    PaymentRecognition = Register(Text);

                return true;

            }

            PaymentRecognition = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this payment recognition.
        /// </summary>
        public PaymentRecognition Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

#pragma warning disable IDE1006 // Naming Styles

        /// <summary>
        /// Credit Card
        /// </summary>
        public static PaymentRecognition  CC       { get; }
            = Register("CC");

        /// <summary>
        /// Debit Card
        /// </summary>
        public static PaymentRecognition  Debit    { get; }
            = Register("Debit");

#pragma warning restore IDE1006 // Naming Styles

        #endregion


        #region Operator overloading

        #region Operator == (PaymentRecognition1, PaymentRecognition2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentRecognition1">A payment recognition.</param>
        /// <param name="PaymentRecognition2">Another payment recognition.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PaymentRecognition PaymentRecognition1,
                                           PaymentRecognition PaymentRecognition2)

            => PaymentRecognition1.Equals(PaymentRecognition2);

        #endregion

        #region Operator != (PaymentRecognition1, PaymentRecognition2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentRecognition1">A payment recognition.</param>
        /// <param name="PaymentRecognition2">Another payment recognition.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PaymentRecognition PaymentRecognition1,
                                           PaymentRecognition PaymentRecognition2)

            => !PaymentRecognition1.Equals(PaymentRecognition2);

        #endregion

        #region Operator <  (PaymentRecognition1, PaymentRecognition2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentRecognition1">A payment recognition.</param>
        /// <param name="PaymentRecognition2">Another payment recognition.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PaymentRecognition PaymentRecognition1,
                                          PaymentRecognition PaymentRecognition2)

            => PaymentRecognition1.CompareTo(PaymentRecognition2) < 0;

        #endregion

        #region Operator <= (PaymentRecognition1, PaymentRecognition2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentRecognition1">A payment recognition.</param>
        /// <param name="PaymentRecognition2">Another payment recognition.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PaymentRecognition PaymentRecognition1,
                                           PaymentRecognition PaymentRecognition2)

            => PaymentRecognition1.CompareTo(PaymentRecognition2) <= 0;

        #endregion

        #region Operator >  (PaymentRecognition1, PaymentRecognition2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentRecognition1">A payment recognition.</param>
        /// <param name="PaymentRecognition2">Another payment recognition.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PaymentRecognition PaymentRecognition1,
                                          PaymentRecognition PaymentRecognition2)

            => PaymentRecognition1.CompareTo(PaymentRecognition2) > 0;

        #endregion

        #region Operator >= (PaymentRecognition1, PaymentRecognition2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PaymentRecognition1">A payment recognition.</param>
        /// <param name="PaymentRecognition2">Another payment recognition.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PaymentRecognition PaymentRecognition1,
                                           PaymentRecognition PaymentRecognition2)

            => PaymentRecognition1.CompareTo(PaymentRecognition2) >= 0;

        #endregion

        #endregion

        #region IComparable<PaymentRecognition> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two payment recognitions.
        /// </summary>
        /// <param name="Object">A payment recognition to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PaymentRecognition paymentRecognition
                   ? CompareTo(paymentRecognition)
                   : throw new ArgumentException("The given object is not payment recognition!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PaymentRecognition)

        /// <summary>
        /// Compares two payment recognitions.
        /// </summary>
        /// <param name="PaymentRecognition">A payment recognition to compare with.</param>
        public Int32 CompareTo(PaymentRecognition PaymentRecognition)

            => String.Compare(InternalId,
                              PaymentRecognition.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<PaymentRecognition> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two payment recognitions for equality.
        /// </summary>
        /// <param name="Object">A payment recognition to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PaymentRecognition paymentRecognition &&
                   Equals(paymentRecognition);

        #endregion

        #region Equals(PaymentRecognition)

        /// <summary>
        /// Compares two payment recognitions for equality.
        /// </summary>
        /// <param name="PaymentRecognition">A payment recognition to compare with.</param>
        public Boolean Equals(PaymentRecognition PaymentRecognition)

            => String.Equals(InternalId,
                             PaymentRecognition.InternalId,
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
