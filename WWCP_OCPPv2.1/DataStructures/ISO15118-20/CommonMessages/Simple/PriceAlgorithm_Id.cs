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

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// Extension methods for price algorithm identifications.
    /// </summary>
    public static class PriceAlgorithmIdExtensions
    {

        /// <summary>
        /// Indicates whether this price algorithm identification is null or empty.
        /// </summary>
        /// <param name="PriceAlgorithmId">A price algorithm identification.</param>
        public static Boolean IsNullOrEmpty(this PriceAlgorithm_Id? PriceAlgorithmId)
            => !PriceAlgorithmId.HasValue || PriceAlgorithmId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this price algorithm identification is null or empty.
        /// </summary>
        /// <param name="PriceAlgorithmId">A price algorithm identification.</param>
        public static Boolean IsNotNullOrEmpty(this PriceAlgorithm_Id? PriceAlgorithmId)
            => PriceAlgorithmId.HasValue && PriceAlgorithmId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A price algorithm identification.
    /// 
    /// xs:pattern value="[\i-[:]][\c-[:]]*"/
    /// </summary>
    public readonly struct PriceAlgorithm_Id : IId,
                                               IEquatable<PriceAlgorithm_Id>,
                                               IComparable<PriceAlgorithm_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the price algorithm identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new price algorithm identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a price algorithm identification.</param>
        private PriceAlgorithm_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a price algorithm identification.
        /// </summary>
        /// <param name="Text">A text representation of a price algorithm identification.</param>
        public static PriceAlgorithm_Id Parse(String Text)
        {

            if (TryParse(Text, out var priceAlgorithmId))
                return priceAlgorithmId;

            throw new ArgumentException($"Invalid text representation of a price algorithm identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a price algorithm identification.
        /// </summary>
        /// <param name="Text">A text representation of a price algorithm identification.</param>
        public static PriceAlgorithm_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var priceAlgorithmId))
                return priceAlgorithmId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out PriceAlgorithmId)

        /// <summary>
        /// Try to parse the given text as a price algorithm identification.
        /// </summary>
        /// <param name="Text">A text representation of a price algorithm identification.</param>
        /// <param name="PriceAlgorithmId">The parsed price algorithm identification.</param>
        public static Boolean TryParse(String Text, out PriceAlgorithm_Id PriceAlgorithmId)
        {

            Text = Text.Trim();

            //ToDo: xs:hexBinary, length: 8

            if (Text.IsNotNullOrEmpty())
            {
                PriceAlgorithmId = new PriceAlgorithm_Id(Text);
                return true;
            }

            PriceAlgorithmId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this price algorithm identification.
        /// </summary>
        public PriceAlgorithm_Id Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (PriceAlgorithmId1, PriceAlgorithmId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceAlgorithmId1">A price algorithm identification.</param>
        /// <param name="PriceAlgorithmId2">Another price algorithm identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PriceAlgorithm_Id PriceAlgorithmId1,
                                           PriceAlgorithm_Id PriceAlgorithmId2)

            => PriceAlgorithmId1.Equals(PriceAlgorithmId2);

        #endregion

        #region Operator != (PriceAlgorithmId1, PriceAlgorithmId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceAlgorithmId1">A price algorithm identification.</param>
        /// <param name="PriceAlgorithmId2">Another price algorithm identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PriceAlgorithm_Id PriceAlgorithmId1,
                                           PriceAlgorithm_Id PriceAlgorithmId2)

            => !PriceAlgorithmId1.Equals(PriceAlgorithmId2);

        #endregion

        #region Operator <  (PriceAlgorithmId1, PriceAlgorithmId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceAlgorithmId1">A price algorithm identification.</param>
        /// <param name="PriceAlgorithmId2">Another price algorithm identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PriceAlgorithm_Id PriceAlgorithmId1,
                                          PriceAlgorithm_Id PriceAlgorithmId2)

            => PriceAlgorithmId1.CompareTo(PriceAlgorithmId2) < 0;

        #endregion

        #region Operator <= (PriceAlgorithmId1, PriceAlgorithmId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceAlgorithmId1">A price algorithm identification.</param>
        /// <param name="PriceAlgorithmId2">Another price algorithm identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PriceAlgorithm_Id PriceAlgorithmId1,
                                           PriceAlgorithm_Id PriceAlgorithmId2)

            => PriceAlgorithmId1.CompareTo(PriceAlgorithmId2) <= 0;

        #endregion

        #region Operator >  (PriceAlgorithmId1, PriceAlgorithmId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceAlgorithmId1">A price algorithm identification.</param>
        /// <param name="PriceAlgorithmId2">Another price algorithm identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PriceAlgorithm_Id PriceAlgorithmId1,
                                          PriceAlgorithm_Id PriceAlgorithmId2)

            => PriceAlgorithmId1.CompareTo(PriceAlgorithmId2) > 0;

        #endregion

        #region Operator >= (PriceAlgorithmId1, PriceAlgorithmId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceAlgorithmId1">A price algorithm identification.</param>
        /// <param name="PriceAlgorithmId2">Another price algorithm identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PriceAlgorithm_Id PriceAlgorithmId1,
                                           PriceAlgorithm_Id PriceAlgorithmId2)

            => PriceAlgorithmId1.CompareTo(PriceAlgorithmId2) >= 0;

        #endregion

        #endregion

        #region IComparable<PriceAlgorithmId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two price algorithm identifications.
        /// </summary>
        /// <param name="Object">A price algorithm identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PriceAlgorithm_Id priceAlgorithmId
                   ? CompareTo(priceAlgorithmId)
                   : throw new ArgumentException("The given object is not a price algorithm identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PriceAlgorithmId)

        /// <summary>
        /// Compares two price algorithm identifications.
        /// </summary>
        /// <param name="PriceAlgorithmId">A price algorithm identification to compare with.</param>
        public Int32 CompareTo(PriceAlgorithm_Id PriceAlgorithmId)

            => String.Compare(InternalId,
                              PriceAlgorithmId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<PriceAlgorithmId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two price algorithm identifications for equality.
        /// </summary>
        /// <param name="Object">A price algorithm identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PriceAlgorithm_Id priceAlgorithmId &&
                   Equals(priceAlgorithmId);

        #endregion

        #region Equals(PriceAlgorithmId)

        /// <summary>
        /// Compares two price algorithm identifications for equality.
        /// </summary>
        /// <param name="PriceAlgorithmId">A price algorithm identification to compare with.</param>
        public Boolean Equals(PriceAlgorithm_Id PriceAlgorithmId)

            => String.Equals(InternalId,
                             PriceAlgorithmId.InternalId,
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
