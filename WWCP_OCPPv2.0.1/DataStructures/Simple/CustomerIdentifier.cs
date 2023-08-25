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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Extention methods for customer identifiers.
    /// </summary>
    public static class CustomerIdentifierExtensions
    {

        /// <summary>
        /// Indicates whether this customer identifier is null or empty.
        /// </summary>
        /// <param name="CustomerIdentifier">A customer identifier.</param>
        public static Boolean IsNullOrEmpty(this CustomerIdentifier? CustomerIdentifier)
            => !CustomerIdentifier.HasValue || CustomerIdentifier.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this customer identifier is null or empty.
        /// </summary>
        /// <param name="CustomerIdentifier">A customer identifier.</param>
        public static Boolean IsNotNullOrEmpty(this CustomerIdentifier? CustomerIdentifier)
            => CustomerIdentifier.HasValue && CustomerIdentifier.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A customer identifier.
    /// </summary>
    public readonly struct CustomerIdentifier : IId,
                                                IEquatable<CustomerIdentifier>,
                                                IComparable<CustomerIdentifier>
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
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identifier is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the customer identifier.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new customer identifier based on the given text.
        /// </summary>
        /// <param name="Text">A text representation of a customer identifier.</param>
        private CustomerIdentifier(String Text)
        {
            this.InternalId  = Text;
        }

        #endregion


        #region (static) NewRandom(Length = 64)

        /// <summary>
        /// Create a new random customer identifier.
        /// </summary>
        /// <param name="Length">The expected length of the customer identifier.</param>
        public static CustomerIdentifier NewRandom(UInt16 Length = 64)

            => new (RandomExtensions.RandomString(Length));

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a customer identifier.
        /// </summary>
        /// <param name="Text">A text representation of a customer identifier.</param>
        public static CustomerIdentifier Parse(String Text)
        {

            if (TryParse(Text, out var customerIdentifier))
                return customerIdentifier;

            throw new ArgumentException("Invalid text representation of a customer identifier: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a customer identifier.
        /// </summary>
        /// <param name="Text">A text representation of a customer identifier.</param>
        public static CustomerIdentifier? TryParse(String Text)
        {

            if (TryParse(Text, out var customerIdentifier))
                return customerIdentifier;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CustomerIdentifier)

        /// <summary>
        /// Try to parse the given text as a customer identifier.
        /// </summary>
        /// <param name="Text">A text representation of a customer identifier.</param>
        /// <param name="CustomerIdentifier">The parsed customer identifier.</param>
        public static Boolean TryParse(String Text, out CustomerIdentifier CustomerIdentifier)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                CustomerIdentifier = new CustomerIdentifier(Text);
                return true;
            }

            CustomerIdentifier = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this customer identifier.
        /// </summary>
        public CustomerIdentifier Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (CustomerIdentifier1, CustomerIdentifier2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerIdentifier1">A customer identifier.</param>
        /// <param name="CustomerIdentifier2">Another customer identifier.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CustomerIdentifier CustomerIdentifier1,
                                           CustomerIdentifier CustomerIdentifier2)

            => CustomerIdentifier1.Equals(CustomerIdentifier2);

        #endregion

        #region Operator != (CustomerIdentifier1, CustomerIdentifier2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerIdentifier1">A customer identifier.</param>
        /// <param name="CustomerIdentifier2">Another customer identifier.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CustomerIdentifier CustomerIdentifier1,
                                           CustomerIdentifier CustomerIdentifier2)

            => !CustomerIdentifier1.Equals(CustomerIdentifier2);

        #endregion

        #region Operator <  (CustomerIdentifier1, CustomerIdentifier2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerIdentifier1">A customer identifier.</param>
        /// <param name="CustomerIdentifier2">Another customer identifier.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CustomerIdentifier CustomerIdentifier1,
                                          CustomerIdentifier CustomerIdentifier2)

            => CustomerIdentifier1.CompareTo(CustomerIdentifier2) < 0;

        #endregion

        #region Operator <= (CustomerIdentifier1, CustomerIdentifier2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerIdentifier1">A customer identifier.</param>
        /// <param name="CustomerIdentifier2">Another customer identifier.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CustomerIdentifier CustomerIdentifier1,
                                           CustomerIdentifier CustomerIdentifier2)

            => CustomerIdentifier1.CompareTo(CustomerIdentifier2) <= 0;

        #endregion

        #region Operator >  (CustomerIdentifier1, CustomerIdentifier2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerIdentifier1">A customer identifier.</param>
        /// <param name="CustomerIdentifier2">Another customer identifier.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CustomerIdentifier CustomerIdentifier1,
                                          CustomerIdentifier CustomerIdentifier2)

            => CustomerIdentifier1.CompareTo(CustomerIdentifier2) > 0;

        #endregion

        #region Operator >= (CustomerIdentifier1, CustomerIdentifier2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerIdentifier1">A customer identifier.</param>
        /// <param name="CustomerIdentifier2">Another customer identifier.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CustomerIdentifier CustomerIdentifier1,
                                           CustomerIdentifier CustomerIdentifier2)

            => CustomerIdentifier1.CompareTo(CustomerIdentifier2) >= 0;

        #endregion

        #endregion

        #region IComparable<CustomerIdentifier> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two customer identifiers.
        /// </summary>
        /// <param name="Object">A customer identifier to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CustomerIdentifier customerIdentifier
                   ? CompareTo(customerIdentifier)
                   : throw new ArgumentException("The given object is not a customer identifier!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CustomerIdentifier)

        /// <summary>
        /// Compares two customer identifiers.
        /// </summary>
        /// <param name="CustomerIdentifier">A customer identifier to compare with.</param>
        public Int32 CompareTo(CustomerIdentifier CustomerIdentifier)

            => String.Compare(InternalId,
                              CustomerIdentifier.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CustomerIdentifier> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two customer identifiers for equality.
        /// </summary>
        /// <param name="Object">A customer identifier to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CustomerIdentifier customerIdentifier &&
                   Equals(customerIdentifier);

        #endregion

        #region Equals(CustomerIdentifier)

        /// <summary>
        /// Compares two customer identifiers for equality.
        /// </summary>
        /// <param name="CustomerIdentifier">A customer identifier to compare with.</param>
        public Boolean Equals(CustomerIdentifier CustomerIdentifier)

            => String.Equals(InternalId,
                             CustomerIdentifier.InternalId,
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
