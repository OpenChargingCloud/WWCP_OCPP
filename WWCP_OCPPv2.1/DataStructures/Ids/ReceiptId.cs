/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for receipt identifications.
    /// </summary>
    public static class ReceiptIdExtensions
    {

        /// <summary>
        /// Indicates whether this receipt identification is null or empty.
        /// </summary>
        /// <param name="ReceiptId">A receipt identification.</param>
        public static Boolean IsNullOrEmpty(this ReceiptId? ReceiptId)
            => !ReceiptId.HasValue || ReceiptId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this receipt identification is null or empty.
        /// </summary>
        /// <param name="ReceiptId">A receipt identification.</param>
        public static Boolean IsNotNullOrEmpty(this ReceiptId? ReceiptId)
            => ReceiptId.HasValue && ReceiptId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The receipt identification.
    /// </summary>
    public readonly struct ReceiptId : IId,
                                       IEquatable<ReceiptId>,
                                       IComparable<ReceiptId>
    {

        #region Data

        /// <summary>
        /// The nummeric value of the receipt identification.
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
        /// The length of the receipt identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) Value.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new receipt identification based on the given text.
        /// </summary>
        /// <param name="Text">A text representation of a receipt identification.</param>
        private ReceiptId(String Text)
        {
            this.Value = Text;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random receipt identification.
        /// </summary>
        public static ReceiptId NewRandom

            => new (RandomExtensions.RandomString(36));

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a receipt identification.
        /// </summary>
        /// <param name="Text">A text representation of a receipt identification.</param>
        public static ReceiptId Parse(String Text)
        {

            if (TryParse(Text, out var receiptId))
                return receiptId;

            throw new ArgumentException($"Invalid text representation of a receipt identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a receipt identification.
        /// </summary>
        /// <param name="Text">A text representation of a receipt identification.</param>
        public static ReceiptId? TryParse(String Text)
        {

            if (TryParse(Text, out var receiptId))
                return receiptId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out ReceiptId)

        /// <summary>
        /// Try to parse the given text as a receipt identification.
        /// </summary>
        /// <param name="Text">A text representation of a receipt identification.</param>
        /// <param name="ReceiptId">The parsed receipt identification.</param>
        public static Boolean TryParse(String Text, out ReceiptId ReceiptId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                ReceiptId = new ReceiptId(Text);
                return true;
            }

            ReceiptId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this receipt identification.
        /// </summary>
        public ReceiptId Clone

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (ReceiptId1, ReceiptId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReceiptId1">A receipt identification.</param>
        /// <param name="ReceiptId2">Another receipt identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ReceiptId ReceiptId1,
                                           ReceiptId ReceiptId2)

            => ReceiptId1.Equals(ReceiptId2);

        #endregion

        #region Operator != (ReceiptId1, ReceiptId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReceiptId1">A receipt identification.</param>
        /// <param name="ReceiptId2">Another receipt identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ReceiptId ReceiptId1,
                                           ReceiptId ReceiptId2)

            => !ReceiptId1.Equals(ReceiptId2);

        #endregion

        #region Operator <  (ReceiptId1, ReceiptId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReceiptId1">A receipt identification.</param>
        /// <param name="ReceiptId2">Another receipt identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ReceiptId ReceiptId1,
                                          ReceiptId ReceiptId2)

            => ReceiptId1.CompareTo(ReceiptId2) < 0;

        #endregion

        #region Operator <= (ReceiptId1, ReceiptId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReceiptId1">A receipt identification.</param>
        /// <param name="ReceiptId2">Another receipt identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ReceiptId ReceiptId1,
                                           ReceiptId ReceiptId2)

            => ReceiptId1.CompareTo(ReceiptId2) <= 0;

        #endregion

        #region Operator >  (ReceiptId1, ReceiptId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReceiptId1">A receipt identification.</param>
        /// <param name="ReceiptId2">Another receipt identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ReceiptId ReceiptId1,
                                          ReceiptId ReceiptId2)

            => ReceiptId1.CompareTo(ReceiptId2) > 0;

        #endregion

        #region Operator >= (ReceiptId1, ReceiptId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReceiptId1">A receipt identification.</param>
        /// <param name="ReceiptId2">Another receipt identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ReceiptId ReceiptId1,
                                           ReceiptId ReceiptId2)

            => ReceiptId1.CompareTo(ReceiptId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ReceiptId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two receipt identifications.
        /// </summary>
        /// <param name="Object">A receipt identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ReceiptId receiptId
                   ? CompareTo(receiptId)
                   : throw new ArgumentException("The given object is not a receipt identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ReceiptId)

        /// <summary>
        /// Compares two receipt identifications.
        /// </summary>
        /// <param name="ReceiptId">A receipt identification to compare with.</param>
        public Int32 CompareTo(ReceiptId ReceiptId)

            => String.Compare(Value,
                              ReceiptId.Value,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ReceiptId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two receipt identifications for equality.
        /// </summary>
        /// <param name="Object">A receipt identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ReceiptId receiptId &&
                   Equals(receiptId);

        #endregion

        #region Equals(ReceiptId)

        /// <summary>
        /// Compares two receipt identifications for equality.
        /// </summary>
        /// <param name="ReceiptId">A receipt identification to compare with.</param>
        public Boolean Equals(ReceiptId ReceiptId)

            => String.Equals(Value,
                             ReceiptId.Value,
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
