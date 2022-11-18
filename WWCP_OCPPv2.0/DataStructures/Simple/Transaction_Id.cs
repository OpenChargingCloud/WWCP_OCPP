/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extention methods for transaction identifications.
    /// </summary>
    public static class TransactionIdExtentions
    {

        /// <summary>
        /// Indicates whether this transaction identification is null or empty.
        /// </summary>
        /// <param name="TransactionId">A transaction identification.</param>
        public static Boolean IsNullOrEmpty(this Transaction_Id? TransactionId)
            => !TransactionId.HasValue || TransactionId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this transaction identification is null or empty.
        /// </summary>
        /// <param name="TransactionId">A transaction identification.</param>
        public static Boolean IsNotNullOrEmpty(this Transaction_Id? TransactionId)
            => TransactionId.HasValue && TransactionId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The transaction identification.
    /// </summary>
    public readonly struct Transaction_Id : IId,
                                            IEquatable<Transaction_Id>,
                                            IComparable<Transaction_Id>
    {

        #region Data

        /// <summary>
        /// The nummeric value of the transaction identification.
        /// </summary>
        public readonly UInt64 Value;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => Value == 0;

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => Value != 0;

        /// <summary>
        /// The length of the tag identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) Value.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new transaction identification.
        /// </summary>
        /// <param name="Number">A number.</param>
        private Transaction_Id(UInt64 Number)
        {
            this.Value = Number;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random transaction identification.
        /// </summary>
        public static Transaction_Id NewRandom

#pragma warning disable SCS0005 // Weak random number generator.
            => new ((UInt64) Random.Shared.Next(Int32.MaxValue));
#pragma warning restore SCS0005 // Weak random number generator.

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a transaction identification.
        /// </summary>
        /// <param name="Text">A text representation of a transaction identification.</param>
        public static Transaction_Id Parse(String Text)
        {

            if (TryParse(Text, out var transactionId))
                return transactionId;

            throw new ArgumentException("Invalid text representation of a transaction identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Integer)

        /// <summary>
        /// Parse the given number as a transaction identification.
        /// </summary>
        public static Transaction_Id Parse(UInt64 Integer)

            => new (Integer);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a transaction identification.
        /// </summary>
        /// <param name="Text">A text representation of a transaction identification.</param>
        public static Transaction_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var transactionId))
                return transactionId;

            return null;

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a transaction identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a transaction identification.</param>
        public static Transaction_Id? TryParse(UInt64 Number)
        {

            if (TryParse(Number, out var transactionId))
                return transactionId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out TransactionId)

        /// <summary>
        /// Try to parse the given string as a transaction identification.
        /// </summary>
        /// <param name="Text">A text representation of a transaction identification.</param>
        /// <param name="TransactionId">The parsed transaction identification.</param>
        public static Boolean TryParse(String Text, out Transaction_Id TransactionId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                UInt64.TryParse(Text, out var number))
            {
                TransactionId = new Transaction_Id(number);
                return true;
            }

            TransactionId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out TransactionId)

        /// <summary>
        /// Try to parse the given number as a transaction identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a transaction identification.</param>
        /// <param name="TransactionId">The parsed transaction identification.</param>
        public static Boolean TryParse(UInt64 Number, out Transaction_Id TransactionId)
        {

            TransactionId = new Transaction_Id(Number);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this transaction identification.
        /// </summary>
        public Transaction_Id Clone

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (TransactionId1, TransactionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransactionId1">A transaction identification.</param>
        /// <param name="TransactionId2">Another transaction identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Transaction_Id TransactionId1,
                                           Transaction_Id TransactionId2)

            => TransactionId1.Equals(TransactionId2);

        #endregion

        #region Operator != (TransactionId1, TransactionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransactionId1">A transaction identification.</param>
        /// <param name="TransactionId2">Another transaction identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Transaction_Id TransactionId1,
                                           Transaction_Id TransactionId2)

            => !TransactionId1.Equals(TransactionId2);

        #endregion

        #region Operator <  (TransactionId1, TransactionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransactionId1">A transaction identification.</param>
        /// <param name="TransactionId2">Another transaction identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Transaction_Id TransactionId1,
                                          Transaction_Id TransactionId2)

            => TransactionId1.CompareTo(TransactionId2) < 0;

        #endregion

        #region Operator <= (TransactionId1, TransactionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransactionId1">A transaction identification.</param>
        /// <param name="TransactionId2">Another transaction identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Transaction_Id TransactionId1,
                                           Transaction_Id TransactionId2)

            => TransactionId1.CompareTo(TransactionId2) <= 0;

        #endregion

        #region Operator >  (TransactionId1, TransactionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransactionId1">A transaction identification.</param>
        /// <param name="TransactionId2">Another transaction identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Transaction_Id TransactionId1,
                                          Transaction_Id TransactionId2)

            => TransactionId1.CompareTo(TransactionId2) > 0;

        #endregion

        #region Operator >= (TransactionId1, TransactionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransactionId1">A transaction identification.</param>
        /// <param name="TransactionId2">Another transaction identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Transaction_Id TransactionId1,
                                           Transaction_Id TransactionId2)

            => TransactionId1.CompareTo(TransactionId2) >= 0;

        #endregion

        #endregion

        #region IComparable<TransactionId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two transaction identifications.
        /// </summary>
        /// <param name="Object">A transaction identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Transaction_Id transactionId
                   ? CompareTo(transactionId)
                   : throw new ArgumentException("The given object is not a transaction identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TransactionId)

        /// <summary>
        /// Compares two transaction identifications.
        /// </summary>
        /// <param name="TransactionId">A transaction identification to compare with.</param>
        public Int32 CompareTo(Transaction_Id TransactionId)

            => Value.CompareTo(TransactionId.Value);

        #endregion

        #endregion

        #region IEquatable<TransactionId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two transaction identifications for equality.
        /// </summary>
        /// <param name="Object">A transaction identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Transaction_Id transactionId &&
                   Equals(transactionId);

        #endregion

        #region Equals(TransactionId)

        /// <summary>
        /// Compares two transaction identifications for equality.
        /// </summary>
        /// <param name="TransactionId">A transaction identification to compare with.</param>
        public Boolean Equals(Transaction_Id TransactionId)

            => Value.Equals(TransactionId.Value);

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
