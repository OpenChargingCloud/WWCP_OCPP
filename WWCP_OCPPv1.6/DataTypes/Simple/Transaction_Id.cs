﻿/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

using System;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// The transaction identification.
    /// </summary>
    public readonly struct Transaction_Id : IId,
                                            IEquatable<Transaction_Id>,
                                            IComparable<Transaction_Id>
    {

        #region Data

        private readonly UInt64 InternalId;

        private static readonly Random random = new Random(DateTime.Now.Millisecond);

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId != 0;

        /// <summary>
        /// The length of the tag identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new transaction identification.
        /// </summary>
        /// <param name="Integer">An integer.</param>
        private Transaction_Id(UInt64 Integer)
        {
            this.InternalId = Integer;
        }

        #endregion


        #region (static) Random

        /// <summary>
        /// Create a new random request identification.
        /// </summary>
        public static Transaction_Id Random

            => new Transaction_Id((UInt64) random.Next());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a transaction identification.
        /// </summary>
        /// <param name="Text">A text representation of a transaction identification.</param>
        public static Transaction_Id Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a transaction identification must not be null or empty!");

            #endregion

            if (TryParse(Text, out Transaction_Id transactionId))
                return transactionId;

            throw new ArgumentNullException(nameof(Text), "The given text representation of a transaction identification is invalid!");

        }

        #endregion

        #region (static) Parse   (Integer)

        /// <summary>
        /// Parse the given number as a transaction identification.
        /// </summary>
        public static Transaction_Id Parse(UInt64 Integer)
            => new Transaction_Id(Integer);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a transaction identification.
        /// </summary>
        /// <param name="Text">A text representation of a transaction identification.</param>
        public static Transaction_Id? TryParse(String Text)
        {

            if (TryParse(Text, out Transaction_Id transactionId))
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

            if (TryParse(Number, out Transaction_Id transactionId))
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

            #region Initial checks

            Text = Text?.Trim();

            if (Text.IsNullOrEmpty())
            {
                TransactionId = default;
                return false;
            }

            #endregion

            if (UInt64.TryParse(Text, out UInt64 number))
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
            => new Transaction_Id(InternalId);

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

            => !(TransactionId1 == TransactionId2);

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

            => !(TransactionId1 > TransactionId2);

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

            => !(TransactionId1 < TransactionId2);

        #endregion

        #endregion

        #region IComparable<TransactionId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Transaction_Id transactionId
                   ? CompareTo(transactionId)
                   : throw new ArgumentException("The given object is not a transaction identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TransactionId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransactionId">An object to compare with.</param>
        public Int32 CompareTo(Transaction_Id TransactionId)

            => InternalId.CompareTo(TransactionId.InternalId);

        #endregion

        #endregion

        #region IEquatable<TransactionId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Transaction_Id transactionId &&
                   Equals(transactionId);

        #endregion

        #region Equals(TransactionId)

        /// <summary>
        /// Compares two transaction identifications for equality.
        /// </summary>
        /// <param name="TransactionId">A transaction identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Transaction_Id TransactionId)

            => InternalId.Equals(TransactionId.InternalId);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId.ToString();

        #endregion

    }

}
