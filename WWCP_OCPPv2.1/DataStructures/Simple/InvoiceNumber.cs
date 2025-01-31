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
    /// Extension methods for invoice numbers.
    /// </summary>
    public static class InvoiceNumberExtensions
    {

        /// <summary>
        /// Indicates whether this invoice number is null or empty.
        /// </summary>
        /// <param name="InvoiceNumber">An invoice number.</param>
        public static Boolean IsNullOrEmpty(this InvoiceNumber? InvoiceNumber)
            => !InvoiceNumber.HasValue || InvoiceNumber.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this invoice number is null or empty.
        /// </summary>
        /// <param name="InvoiceNumber">An invoice number.</param>
        public static Boolean IsNotNullOrEmpty(this InvoiceNumber? InvoiceNumber)
            => InvoiceNumber.HasValue && InvoiceNumber.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An invoice number.
    /// </summary>
    public readonly struct InvoiceNumber : IId,
                                           IEquatable<InvoiceNumber>,
                                           IComparable<InvoiceNumber>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identifier is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identifier is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the invoice number.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new invoice number based on the given text.
        /// </summary>
        /// <param name="Text">A text representation of an invoice number.</param>
        private InvoiceNumber(String Text)
        {
            this.InternalId  = Text;
        }

        #endregion


        #region (static) NewRandom(Length = 64)

        /// <summary>
        /// Create a new random invoice number.
        /// </summary>
        /// <param name="Length">The expected length of the invoice number.</param>
        public static InvoiceNumber NewRandom(UInt16 Length = 64)

            => new (RandomExtensions.RandomString(Length));

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an invoice number.
        /// </summary>
        /// <param name="Text">A text representation of an invoice number.</param>
        public static InvoiceNumber Parse(String Text)
        {

            if (TryParse(Text, out var invoiceNumber))
                return invoiceNumber;

            throw new ArgumentException($"Invalid text representation of an invoice number: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an invoice number.
        /// </summary>
        /// <param name="Text">A text representation of an invoice number.</param>
        public static InvoiceNumber? TryParse(String Text)
        {

            if (TryParse(Text, out var invoiceNumber))
                return invoiceNumber;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out InvoiceNumber)

        /// <summary>
        /// Try to parse the given text as an invoice number.
        /// </summary>
        /// <param name="Text">A text representation of an invoice number.</param>
        /// <param name="InvoiceNumber">The parsed invoice number.</param>
        public static Boolean TryParse(String Text, out InvoiceNumber InvoiceNumber)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                InvoiceNumber = new InvoiceNumber(Text);
                return true;
            }

            InvoiceNumber = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this invoice number.
        /// </summary>
        public InvoiceNumber Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (InvoiceNumber1, InvoiceNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceNumber1">An invoice number.</param>
        /// <param name="InvoiceNumber2">Another invoice number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (InvoiceNumber InvoiceNumber1,
                                           InvoiceNumber InvoiceNumber2)

            => InvoiceNumber1.Equals(InvoiceNumber2);

        #endregion

        #region Operator != (InvoiceNumber1, InvoiceNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceNumber1">An invoice number.</param>
        /// <param name="InvoiceNumber2">Another invoice number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (InvoiceNumber InvoiceNumber1,
                                           InvoiceNumber InvoiceNumber2)

            => !InvoiceNumber1.Equals(InvoiceNumber2);

        #endregion

        #region Operator <  (InvoiceNumber1, InvoiceNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceNumber1">An invoice number.</param>
        /// <param name="InvoiceNumber2">Another invoice number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (InvoiceNumber InvoiceNumber1,
                                          InvoiceNumber InvoiceNumber2)

            => InvoiceNumber1.CompareTo(InvoiceNumber2) < 0;

        #endregion

        #region Operator <= (InvoiceNumber1, InvoiceNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceNumber1">An invoice number.</param>
        /// <param name="InvoiceNumber2">Another invoice number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (InvoiceNumber InvoiceNumber1,
                                           InvoiceNumber InvoiceNumber2)

            => InvoiceNumber1.CompareTo(InvoiceNumber2) <= 0;

        #endregion

        #region Operator >  (InvoiceNumber1, InvoiceNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceNumber1">An invoice number.</param>
        /// <param name="InvoiceNumber2">Another invoice number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (InvoiceNumber InvoiceNumber1,
                                          InvoiceNumber InvoiceNumber2)

            => InvoiceNumber1.CompareTo(InvoiceNumber2) > 0;

        #endregion

        #region Operator >= (InvoiceNumber1, InvoiceNumber2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="InvoiceNumber1">An invoice number.</param>
        /// <param name="InvoiceNumber2">Another invoice number.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (InvoiceNumber InvoiceNumber1,
                                           InvoiceNumber InvoiceNumber2)

            => InvoiceNumber1.CompareTo(InvoiceNumber2) >= 0;

        #endregion

        #endregion

        #region IComparable<InvoiceNumber> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two invoice numbers.
        /// </summary>
        /// <param name="Object">An invoice number to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is InvoiceNumber invoiceNumber
                   ? CompareTo(invoiceNumber)
                   : throw new ArgumentException("The given object is not an invoice number!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(InvoiceNumber)

        /// <summary>
        /// Compares two invoice numbers.
        /// </summary>
        /// <param name="InvoiceNumber">An invoice number to compare with.</param>
        public Int32 CompareTo(InvoiceNumber InvoiceNumber)

            => String.Compare(InternalId,
                              InvoiceNumber.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<InvoiceNumber> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two invoice numbers for equality.
        /// </summary>
        /// <param name="Object">An invoice number to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is InvoiceNumber invoiceNumber &&
                   Equals(invoiceNumber);

        #endregion

        #region Equals(InvoiceNumber)

        /// <summary>
        /// Compares two invoice numbers for equality.
        /// </summary>
        /// <param name="InvoiceNumber">An invoice number to compare with.</param>
        public Boolean Equals(InvoiceNumber InvoiceNumber)

            => String.Equals(InternalId,
                             InvoiceNumber.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.GetHashCode() ?? 0;

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
