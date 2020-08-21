/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

namespace cloud.charging.adapters.OCPPv1_6
{

    /// <summary>
    /// A transaction identification.
    /// </summary>
    public struct Transaction_Id : IId,
                                   IEquatable<Transaction_Id>,
                                   IComparable<Transaction_Id>
    {

        #region Data

        private readonly UInt64 _Value;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => _Value != 0;

        /// <summary>
        /// The length of the tag identification.
        /// </summary>
        public UInt64 Length
            => 0;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new OCPP transaction identification.
        /// </summary>
        /// <param name="Token">An integer.</param>
        private Transaction_Id(UInt64 Token)
        {
            this._Value = Token;
        }

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
            => new Transaction_Id(_Value);

        #endregion


        #region Operator overloading

        #region Operator == (TransactionId1, TransactionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransactionId1">An transaction identification.</param>
        /// <param name="TransactionId2">Another transaction identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Transaction_Id TransactionId1, Transaction_Id TransactionId2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TransactionId1, TransactionId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) TransactionId1 == null) || ((Object) TransactionId2 == null))
                return false;

            if ((Object) TransactionId1 == null)
                throw new ArgumentNullException(nameof(TransactionId1),  "The given transaction identification must not be null!");

            return TransactionId1.Equals(TransactionId2);

        }

        #endregion

        #region Operator != (TransactionId1, TransactionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransactionId1">An transaction identification.</param>
        /// <param name="TransactionId2">Another transaction identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Transaction_Id TransactionId1, Transaction_Id TransactionId2)
            => !(TransactionId1 == TransactionId2);

        #endregion

        #region Operator <  (TransactionId1, TransactionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransactionId1">An transaction identification.</param>
        /// <param name="TransactionId2">Another transaction identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Transaction_Id TransactionId1, Transaction_Id TransactionId2)
        {

            if ((Object) TransactionId1 == null)
                throw new ArgumentNullException(nameof(TransactionId1),  "The given transaction identification must not be null!");

            return TransactionId1.CompareTo(TransactionId2) < 0;

        }

        #endregion

        #region Operator <= (TransactionId1, TransactionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransactionId1">An transaction identification.</param>
        /// <param name="TransactionId2">Another transaction identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Transaction_Id TransactionId1, Transaction_Id TransactionId2)
            => !(TransactionId1 > TransactionId2);

        #endregion

        #region Operator >  (TransactionId1, TransactionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransactionId1">An transaction identification.</param>
        /// <param name="TransactionId2">Another transaction identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Transaction_Id TransactionId1, Transaction_Id TransactionId2)
        {

            if ((Object) TransactionId1 == null)
                throw new ArgumentNullException(nameof(TransactionId1),  "The given transaction identification must not be null!");

            return TransactionId1.CompareTo(TransactionId2) > 0;

        }

        #endregion

        #region Operator >= (TransactionId1, TransactionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransactionId1">An transaction identification.</param>
        /// <param name="TransactionId2">Another transaction identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Transaction_Id TransactionId1, Transaction_Id TransactionId2)
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
        {

            if (Object is null)
                throw new ArgumentNullException(nameof(Object),  "The given object must not be null!");

            // Check if the given object is a transaction identification.
            if (!(Object is Transaction_Id))
                throw new ArgumentException("The given object is not a transaction identification!", nameof(Object));

            return CompareTo((Transaction_Id) Object);

        }

        #endregion

        #region CompareTo(TransactionId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransactionId">An object to compare with.</param>
        public Int32 CompareTo(Transaction_Id TransactionId)
        {

            if ((Object) TransactionId == null)
                throw new ArgumentNullException(nameof(TransactionId),  "The given transaction identification must not be null!");

            return _Value.CompareTo(TransactionId._Value);

        }

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
        {

            if (Object is null)
                return false;

            // Check if the given object is a transaction identification.
            if (!(Object is Transaction_Id))
                return false;

            return this.Equals((Transaction_Id) Object);

        }

        #endregion

        #region Equals(TransactionId)

        /// <summary>
        /// Compares two transaction identifications for equality.
        /// </summary>
        /// <param name="TransactionId">An transaction identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Transaction_Id TransactionId)
        {

            if ((Object) TransactionId == null)
                return false;

            return _Value.Equals(TransactionId._Value);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => _Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => _Value.ToString();

        #endregion


    }

}
