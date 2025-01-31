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

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonTypes
{

    /// <summary>
    /// Extension methods for tax rule identifications.
    /// </summary>
    public static class TaxRuleIdExtensions
    {

        /// <summary>
        /// Indicates whether this tax rule identification is null or empty.
        /// </summary>
        /// <param name="TaxRuleId">A tax rule identification.</param>
        public static Boolean IsNullOrEmpty(this TaxRule_Id? TaxRuleId)
            => !TaxRuleId.HasValue || TaxRuleId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this tax rule identification is null or empty.
        /// </summary>
        /// <param name="TaxRuleId">A tax rule identification.</param>
        public static Boolean IsNotNullOrEmpty(this TaxRule_Id? TaxRuleId)
            => TaxRuleId.HasValue && TaxRuleId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A tax rule identification.
    /// </summary>
    public readonly struct TaxRule_Id : IId,
                                        IEquatable<TaxRule_Id>,
                                        IComparable<TaxRule_Id>
    {

        #region Data

        /// <summary>
        /// The nummeric value of the tax rule identification.
        /// </summary>
        public readonly UInt32 Value;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => false;

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => true;

        /// <summary>
        /// The length of the tax rule identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) Value.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new tax rule identification based on the given number.
        /// </summary>
        /// <param name="Number">A numeric representation of a display message identification.</param>
        private TaxRule_Id(UInt32 Number)
        {
            this.Value = Number;
        }

        #endregion


        #region Documentation

        // <xs:simpleType name="tax ruleIDType">
        //     <xs:restriction base="xs:unsignedShort"/>
        // </xs:simpleType>

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a tax rule identification.
        /// </summary>
        /// <param name="Text">A text representation of a tax rule identification.</param>
        public static TaxRule_Id Parse(String Text)
        {

            if (TryParse(Text, out var taxRuleId))
                return taxRuleId;

            throw new ArgumentException($"Invalid text representation of a tax rule identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Number)

        /// <summary>
        /// Parse the given number as a tax rule identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a tax rule identification.</param>
        public static TaxRule_Id Parse(UInt32 Number)
        {

            if (TryParse(Number, out var taxRuleId))
                return taxRuleId;

            throw new ArgumentException($"Invalid numeric representation of a tax rule identification: '{Number}'!",
                                        nameof(Number));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a tax rule identification.
        /// </summary>
        /// <param name="Text">A text representation of a tax rule identification.</param>
        public static TaxRule_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var taxRuleId))
                return taxRuleId;

            return null;

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a tax rule identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a tax rule identification.</param>
        public static TaxRule_Id? TryParse(UInt32 Number)
        {

            if (TryParse(Number, out var taxRuleId))
                return taxRuleId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out TaxRuleId)

        /// <summary>
        /// Try to parse the given text as a tax rule identification.
        /// </summary>
        /// <param name="Text">A text representation of a tax rule identification.</param>
        /// <param name="TaxRuleId">The parsed tax rule identification.</param>
        public static Boolean TryParse(String Text, out TaxRule_Id TaxRuleId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                UInt32.TryParse(Text, out var number))
            {
                TaxRuleId = new TaxRule_Id(number);
                return true;
            }

            TaxRuleId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out TaxRuleId)

        /// <summary>
        /// Try to parse the given number as a tax rule identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a tax rule identification.</param>
        /// <param name="TaxRuleId">The parsed tax rule identification.</param>
        public static Boolean TryParse(UInt32 Number, out TaxRule_Id TaxRuleId)
        {

            TaxRuleId = new TaxRule_Id(Number);

            return true;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this tax rule identification.
        /// </summary>
        public TaxRule_Id Clone()

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (TaxRuleId1, TaxRuleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRuleId1">A tax rule identification.</param>
        /// <param name="TaxRuleId2">Another tax rule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TaxRule_Id TaxRuleId1,
                                           TaxRule_Id TaxRuleId2)

            => TaxRuleId1.Equals(TaxRuleId2);

        #endregion

        #region Operator != (TaxRuleId1, TaxRuleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRuleId1">A tax rule identification.</param>
        /// <param name="TaxRuleId2">Another tax rule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TaxRule_Id TaxRuleId1,
                                           TaxRule_Id TaxRuleId2)

            => !TaxRuleId1.Equals(TaxRuleId2);

        #endregion

        #region Operator <  (TaxRuleId1, TaxRuleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRuleId1">A tax rule identification.</param>
        /// <param name="TaxRuleId2">Another tax rule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TaxRule_Id TaxRuleId1,
                                          TaxRule_Id TaxRuleId2)

            => TaxRuleId1.CompareTo(TaxRuleId2) < 0;

        #endregion

        #region Operator <= (TaxRuleId1, TaxRuleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRuleId1">A tax rule identification.</param>
        /// <param name="TaxRuleId2">Another tax rule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TaxRule_Id TaxRuleId1,
                                           TaxRule_Id TaxRuleId2)

            => TaxRuleId1.CompareTo(TaxRuleId2) <= 0;

        #endregion

        #region Operator >  (TaxRuleId1, TaxRuleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRuleId1">A tax rule identification.</param>
        /// <param name="TaxRuleId2">Another tax rule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TaxRule_Id TaxRuleId1,
                                          TaxRule_Id TaxRuleId2)

            => TaxRuleId1.CompareTo(TaxRuleId2) > 0;

        #endregion

        #region Operator >= (TaxRuleId1, TaxRuleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxRuleId1">A tax rule identification.</param>
        /// <param name="TaxRuleId2">Another tax rule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TaxRule_Id TaxRuleId1,
                                           TaxRule_Id TaxRuleId2)

            => TaxRuleId1.CompareTo(TaxRuleId2) >= 0;

        #endregion

        #endregion

        #region IComparable<TaxRuleId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two tax rule identifications.
        /// </summary>
        /// <param name="Object">A tax rule identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TaxRule_Id taxRuleId
                   ? CompareTo(taxRuleId)
                   : throw new ArgumentException("The given object is not a tax rule identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TaxRuleId)

        /// <summary>
        /// Compares two tax rule identifications.
        /// </summary>
        /// <param name="TaxRuleId">A tax rule identification to compare with.</param>
        public Int32 CompareTo(TaxRule_Id TaxRuleId)

            => Value.CompareTo(TaxRuleId.Value);

        #endregion

        #endregion

        #region IEquatable<TaxRuleId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tax rule identifications for equality.
        /// </summary>
        /// <param name="Object">A tax rule identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TaxRule_Id taxRuleId &&
                   Equals(taxRuleId);

        #endregion

        #region Equals(TaxRuleId)

        /// <summary>
        /// Compares two tax rule identifications for equality.
        /// </summary>
        /// <param name="TaxRuleId">A tax rule identification to compare with.</param>
        public Boolean Equals(TaxRule_Id TaxRuleId)

            => Value.Equals(TaxRuleId.Value);

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
