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
    /// Extension methods for transaction identifications.
    /// </summary>
    public static class TransactionIdExtensions
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
        /// The numeric value of the transaction identification.
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
        /// The length of the transaction identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) Value.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new transaction identification based on the given text.
        /// </summary>
        /// <param name="Text">A text representation of a transaction identification.</param>
        private Transaction_Id(String Text)
        {
            this.Value = Text;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random transaction identification.
        /// </summary>
        public static Transaction_Id NewRandom

            => new (RandomExtensions.RandomString(36));

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

            throw new ArgumentException($"Invalid text representation of a transaction identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a transaction identification.
        /// </summary>
        /// <param name="Text">A text representation of a transaction identification.</param>
        public static Transaction_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var transactionId))
                return transactionId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out TransactionId)

        /// <summary>
        /// Try to parse the given text as a transaction identification.
        /// </summary>
        /// <param name="Text">A text representation of a transaction identification.</param>
        /// <param name="TransactionId">The parsed transaction identification.</param>
        public static Boolean TryParse(String Text, out Transaction_Id TransactionId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                TransactionId = new Transaction_Id(Text);
                return true;
            }

            TransactionId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this transaction identification.
        /// </summary>
        public Transaction_Id Clone()

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

            => String.Compare(Value,
                              TransactionId.Value,
                              StringComparison.OrdinalIgnoreCase);

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

            => String.Equals(Value,
                             TransactionId.Value,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
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
