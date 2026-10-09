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
    /// Extension methods for DER control identifications.
    /// </summary>
    public static class DERControlIdExtensions
    {

        /// <summary>
        /// Indicates whether this DER control identification is null or empty.
        /// </summary>
        /// <param name="DERControlId">A DER control identification.</param>
        public static Boolean IsNullOrEmpty(this DERControl_Id? DERControlId)
            => !DERControlId.HasValue || DERControlId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this DER control identification is null or empty.
        /// </summary>
        /// <param name="DERControlId">A DER control identification.</param>
        public static Boolean IsNotNullOrEmpty(this DERControl_Id? DERControlId)
            => DERControlId.HasValue && DERControlId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A DER control identification.
    /// </summary>
    public readonly struct DERControl_Id : IId,
                                           IEquatable<DERControl_Id>,
                                           IComparable<DERControl_Id>
    {

        #region Data

        /// <summary>
        /// The text of the DER control identification - the specification's
        /// identifierString of up to 36 characters, e.g. a UUID.
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
        /// The length of the DER control identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (Value?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DER control identification based on the given text.
        /// </summary>
        /// <param name="Text">The text of a DER control identification.</param>
        private DERControl_Id(String Text)
        {
            this.Value = Text;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random DER control identification.
        /// </summary>
        public static DERControl_Id NewRandom

            => new (Guid.NewGuid().ToString());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a DER control identification.
        /// </summary>
        /// <param name="Text">A text representation of a DER control identification.</param>
        public static DERControl_Id Parse(String Text)
        {

            if (TryParse(Text, out var derControlId))
                return derControlId;

            throw new ArgumentException($"Invalid text representation of a DER control identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Number)

        /// <summary>
        /// Parse the given number as a DER control identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a DER control identification.</param>
        public static DERControl_Id Parse(UInt64 Number)

            => new (Number.ToString(System.Globalization.CultureInfo.InvariantCulture));

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a DER control identification.
        /// </summary>
        /// <param name="Text">A text representation of a DER control identification.</param>
        public static DERControl_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var derControlId))
                return derControlId;

            return null;

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a DER control identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a DER control identification.</param>
        public static DERControl_Id? TryParse(UInt64 Number)
        {

            if (TryParse(Number, out var derControlId))
                return derControlId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out DERControlId)

        /// <summary>
        /// Try to parse the given text as a DER control identification.
        /// </summary>
        /// <param name="Text">A text representation of a DER control identification.</param>
        /// <param name="DERControlId">The parsed DER control identification.</param>
        public static Boolean TryParse(String Text, out DERControl_Id DERControlId)
        {

            // Any identifierString of up to 36 characters - not only a number.
            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                Text.Length <= 36)
            {
                DERControlId = new DERControl_Id(Text);
                return true;
            }

            DERControlId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out DERControlId)

        /// <summary>
        /// Try to parse the given number as a DER control identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a DER control identification.</param>
        /// <param name="DERControlId">The parsed DER control identification.</param>
        public static Boolean TryParse(UInt64 Number, out DERControl_Id DERControlId)
        {

            DERControlId = new DERControl_Id(Number.ToString(System.Globalization.CultureInfo.InvariantCulture));

            return true;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this DER control identification.
        /// </summary>
        public DERControl_Id Clone()

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (DERControlId1, DERControlId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlId1">A DER control identification.</param>
        /// <param name="DERControlId2">Another DER control identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DERControl_Id DERControlId1,
                                           DERControl_Id DERControlId2)

            => DERControlId1.Equals(DERControlId2);

        #endregion

        #region Operator != (DERControlId1, DERControlId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlId1">A DER control identification.</param>
        /// <param name="DERControlId2">Another DER control identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DERControl_Id DERControlId1,
                                           DERControl_Id DERControlId2)

            => !DERControlId1.Equals(DERControlId2);

        #endregion

        #region Operator <  (DERControlId1, DERControlId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlId1">A DER control identification.</param>
        /// <param name="DERControlId2">Another DER control identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (DERControl_Id DERControlId1,
                                          DERControl_Id DERControlId2)

            => DERControlId1.CompareTo(DERControlId2) < 0;

        #endregion

        #region Operator <= (DERControlId1, DERControlId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlId1">A DER control identification.</param>
        /// <param name="DERControlId2">Another DER control identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (DERControl_Id DERControlId1,
                                           DERControl_Id DERControlId2)

            => DERControlId1.CompareTo(DERControlId2) <= 0;

        #endregion

        #region Operator >  (DERControlId1, DERControlId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlId1">A DER control identification.</param>
        /// <param name="DERControlId2">Another DER control identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (DERControl_Id DERControlId1,
                                          DERControl_Id DERControlId2)

            => DERControlId1.CompareTo(DERControlId2) > 0;

        #endregion

        #region Operator >= (DERControlId1, DERControlId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERControlId1">A DER control identification.</param>
        /// <param name="DERControlId2">Another DER control identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (DERControl_Id DERControlId1,
                                           DERControl_Id DERControlId2)

            => DERControlId1.CompareTo(DERControlId2) >= 0;

        #endregion

        #endregion

        #region IComparable<DERControlId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two DER control identifications.
        /// </summary>
        /// <param name="Object">A DER control identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is DERControl_Id derControlId
                   ? CompareTo(derControlId)
                   : throw new ArgumentException("The given object is not a DER control identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(DERControlId)

        /// <summary>
        /// Compares two DER control identifications.
        /// </summary>
        /// <param name="DERControlId">A DER control identification to compare with.</param>
        public Int32 CompareTo(DERControl_Id DERControlId)

            => String.Compare(Value, DERControlId.Value, StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<DERControlId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DER control identifications for equality.
        /// </summary>
        /// <param name="Object">A DER control identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DERControl_Id derControlId &&
                   Equals(derControlId);

        #endregion

        #region Equals(DERControlId)

        /// <summary>
        /// Compares two DER control identifications for equality.
        /// </summary>
        /// <param name="DERControlId">A DER control identification to compare with.</param>
        public Boolean Equals(DERControl_Id DERControlId)

            => String.Equals(Value, DERControlId.Value, StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Value?.GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Value ?? "";

        #endregion

    }

}
