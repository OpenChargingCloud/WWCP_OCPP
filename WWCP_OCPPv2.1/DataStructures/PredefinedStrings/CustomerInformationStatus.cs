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
    /// Extension methods for customer information status.
    /// </summary>
    public static class CustomerInformationStatusExtensions
    {

        /// <summary>
        /// Indicates whether this customer information status is null or empty.
        /// </summary>
        /// <param name="CustomerInformationStatus">A customer information status.</param>
        public static Boolean IsNullOrEmpty(this CustomerInformationStatus? CustomerInformationStatus)
            => !CustomerInformationStatus.HasValue || CustomerInformationStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this customer information status is null or empty.
        /// </summary>
        /// <param name="CustomerInformationStatus">A customer information status.</param>
        public static Boolean IsNotNullOrEmpty(this CustomerInformationStatus? CustomerInformationStatus)
            => CustomerInformationStatus.HasValue && CustomerInformationStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A customer information status.
    /// </summary>
    public readonly struct CustomerInformationStatus : IId,
                                                       IEquatable<CustomerInformationStatus>,
                                                       IComparable<CustomerInformationStatus>
    {

        #region Data

        private readonly static Dictionary<String, CustomerInformationStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                  InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this customer information status is null or empty.
        /// </summary>
        public readonly  Boolean                          IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this customer information status is NOT null or empty.
        /// </summary>
        public readonly  Boolean                          IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the customer information status.
        /// </summary>
        public readonly  UInt64                           Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered reset types.
        /// </summary>
        public static    IEnumerable<CustomerInformationStatus>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new customer information status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a customer information status.</param>
        private CustomerInformationStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static CustomerInformationStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new CustomerInformationStatus(Text)
               );

        #endregion


        #region (static) Parse     (Text)

        /// <summary>
        /// Parse the given string as a customer information status.
        /// </summary>
        /// <param name="Text">A text representation of a customer information status.</param>
        public static CustomerInformationStatus Parse(String Text)
        {

            if (TryParse(Text, out var customerInformationStatus))
                return customerInformationStatus;

            throw new ArgumentException($"Invalid text representation of a customer information status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse  (Text)

        /// <summary>
        /// Try to parse the given text as a customer information status.
        /// </summary>
        /// <param name="Text">A text representation of a customer information status.</param>
        public static CustomerInformationStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var customerInformationStatus))
                return customerInformationStatus;

            return null;

        }

        #endregion

        #region (static) TryParse  (Text, out CustomerInformationStatus)

        /// <summary>
        /// Try to parse the given text as a customer information status.
        /// </summary>
        /// <param name="Text">A text representation of a customer information status.</param>
        /// <param name="CustomerInformationStatus">The parsed customer information status.</param>
        public static Boolean TryParse (String                                      Text,
                                        [NotNullWhen(true)] out CustomerInformationStatus  CustomerInformationStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out CustomerInformationStatus))
                    CustomerInformationStatus = Register(Text);

                return true;

            }

            CustomerInformationStatus = default;
            return false;

        }

        #endregion

        #region (static) IsDefined (Text, out CustomerInformationStatus)

        /// <summary>
        /// Check whether the given text is a defined customer information status.
        /// </summary>
        /// <param name="Text">A text representation of a customer information status.</param>
        /// <param name="CustomerInformationStatus">The validated customer information status.</param>
        public static Boolean IsDefined(String                                     Text,
                                       [NotNullWhen(true)] out CustomerInformationStatus  CustomerInformationStatus)

            => lookup.TryGetValue(Text.Trim(), out CustomerInformationStatus);

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this customer information status.
        /// </summary>
        public CustomerInformationStatus Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Accepted
        /// </summary>
        public static CustomerInformationStatus  Accepted          { get; }
            = Register("Accepted");

        /// <summary>
        /// Rejected
        /// </summary>
        public static CustomerInformationStatus  Rejected          { get; }
            = Register("Rejected");

        /// <summary>
        /// Invalid
        /// </summary>
        public static CustomerInformationStatus  Invalid           { get; }
            = Register("Invalid");


        public static CustomerInformationStatus  Error             { get; }
            = Register("Error");

        public static CustomerInformationStatus  SignatureError    { get; }
            = Register("SignatureError");

        #endregion


        #region Operator overloading

        #region Operator == (CustomerInformationStatus1, CustomerInformationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerInformationStatus1">A customer information status.</param>
        /// <param name="CustomerInformationStatus2">Another customer information status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CustomerInformationStatus CustomerInformationStatus1,
                                           CustomerInformationStatus CustomerInformationStatus2)

            => CustomerInformationStatus1.Equals(CustomerInformationStatus2);

        #endregion

        #region Operator != (CustomerInformationStatus1, CustomerInformationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerInformationStatus1">A customer information status.</param>
        /// <param name="CustomerInformationStatus2">Another customer information status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CustomerInformationStatus CustomerInformationStatus1,
                                           CustomerInformationStatus CustomerInformationStatus2)

            => !CustomerInformationStatus1.Equals(CustomerInformationStatus2);

        #endregion

        #region Operator <  (CustomerInformationStatus1, CustomerInformationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerInformationStatus1">A customer information status.</param>
        /// <param name="CustomerInformationStatus2">Another customer information status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CustomerInformationStatus CustomerInformationStatus1,
                                          CustomerInformationStatus CustomerInformationStatus2)

            => CustomerInformationStatus1.CompareTo(CustomerInformationStatus2) < 0;

        #endregion

        #region Operator <= (CustomerInformationStatus1, CustomerInformationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerInformationStatus1">A customer information status.</param>
        /// <param name="CustomerInformationStatus2">Another customer information status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CustomerInformationStatus CustomerInformationStatus1,
                                           CustomerInformationStatus CustomerInformationStatus2)

            => CustomerInformationStatus1.CompareTo(CustomerInformationStatus2) <= 0;

        #endregion

        #region Operator >  (CustomerInformationStatus1, CustomerInformationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerInformationStatus1">A customer information status.</param>
        /// <param name="CustomerInformationStatus2">Another customer information status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CustomerInformationStatus CustomerInformationStatus1,
                                          CustomerInformationStatus CustomerInformationStatus2)

            => CustomerInformationStatus1.CompareTo(CustomerInformationStatus2) > 0;

        #endregion

        #region Operator >= (CustomerInformationStatus1, CustomerInformationStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomerInformationStatus1">A customer information status.</param>
        /// <param name="CustomerInformationStatus2">Another customer information status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CustomerInformationStatus CustomerInformationStatus1,
                                           CustomerInformationStatus CustomerInformationStatus2)

            => CustomerInformationStatus1.CompareTo(CustomerInformationStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<CustomerInformationStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two customer information status.
        /// </summary>
        /// <param name="Object">A customer information status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CustomerInformationStatus customerInformationStatus
                   ? CompareTo(customerInformationStatus)
                   : throw new ArgumentException("The given object is not a customer information status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CustomerInformationStatus)

        /// <summary>
        /// Compares two customer information status.
        /// </summary>
        /// <param name="CustomerInformationStatus">A customer information status to compare with.</param>
        public Int32 CompareTo(CustomerInformationStatus CustomerInformationStatus)

            => String.Compare(InternalId,
                              CustomerInformationStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CustomerInformationStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two customer information status for equality.
        /// </summary>
        /// <param name="Object">A customer information status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CustomerInformationStatus customerInformationStatus &&
                   Equals(customerInformationStatus);

        #endregion

        #region Equals(CustomerInformationStatus)

        /// <summary>
        /// Compares two customer information status for equality.
        /// </summary>
        /// <param name="CustomerInformationStatus">A customer information status to compare with.</param>
        public Boolean Equals(CustomerInformationStatus CustomerInformationStatus)

            => String.Equals(InternalId,
                             CustomerInformationStatus.InternalId,
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
