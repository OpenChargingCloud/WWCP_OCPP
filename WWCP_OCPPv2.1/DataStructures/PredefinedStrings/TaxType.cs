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
    /// Extension methods for tax types.
    /// </summary>
    public static class TaxTypeExtensions
    {

        /// <summary>
        /// Indicates whether this tax type is null or empty.
        /// </summary>
        /// <param name="TaxType">A tax type.</param>
        public static Boolean IsNullOrEmpty(this TaxType? TaxType)
            => !TaxType.HasValue || TaxType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this tax type is null or empty.
        /// </summary>
        /// <param name="TaxType">A tax type.</param>
        public static Boolean IsNotNullOrEmpty(this TaxType? TaxType)
            => TaxType.HasValue && TaxType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A tax type.
    /// </summary>
    public readonly struct TaxType : IId,
                                     IEquatable<TaxType>,
                                     IComparable<TaxType>
    {

        #region Data

        private readonly static Dictionary<String, TaxType>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                       InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this tax type is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this tax type is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the tax type.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new tax type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a tax type.</param>
        private TaxType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static TaxType Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new TaxType(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a tax type.
        /// </summary>
        /// <param name="Text">A text representation of a tax type.</param>
        public static TaxType Parse(String Text)
        {

            if (TryParse(Text, out var taxType))
                return taxType;

            throw new ArgumentException($"Invalid text representation of a tax type: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a tax type.
        /// </summary>
        /// <param name="Text">A text representation of a tax type.</param>
        public static TaxType? TryParse(String Text)
        {

            if (TryParse(Text, out var taxType))
                return taxType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TaxType)

        /// <summary>
        /// Try to parse the given text as a tax type.
        /// </summary>
        /// <param name="Text">A text representation of a tax type.</param>
        /// <param name="TaxType">The parsed tax type.</param>
        public static Boolean TryParse(String                           Text,
                                       [NotNullWhen(true)] out TaxType  TaxType)
        {

            Text = Text.Trim().SubstringMax(20);

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out TaxType))
                    TaxType = Register(Text);

                return true;

            }

            TaxType = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this tax type.
        /// </summary>
        public TaxType Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Valued Added Tax.
        /// </summary>
        public static TaxType VAT        { get; }
            = Register("VAT");

        /// <summary>
        /// A state tax.
        /// </summary>
        public static TaxType State      { get; }
            = Register("state");

        /// <summary>
        /// A federal tax.
        /// </summary>
        public static TaxType Federal    { get; }
            = Register("federal");

        #endregion


        #region Operator overloading

        #region Operator == (TaxType1, TaxType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxType1">A tax type.</param>
        /// <param name="TaxType2">Another tax type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TaxType TaxType1,
                                           TaxType TaxType2)

            => TaxType1.Equals(TaxType2);

        #endregion

        #region Operator != (TaxType1, TaxType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxType1">A tax type.</param>
        /// <param name="TaxType2">Another tax type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TaxType TaxType1,
                                           TaxType TaxType2)

            => !TaxType1.Equals(TaxType2);

        #endregion

        #region Operator <  (TaxType1, TaxType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxType1">A tax type.</param>
        /// <param name="TaxType2">Another tax type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TaxType TaxType1,
                                          TaxType TaxType2)

            => TaxType1.CompareTo(TaxType2) < 0;

        #endregion

        #region Operator <= (TaxType1, TaxType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxType1">A tax type.</param>
        /// <param name="TaxType2">Another tax type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TaxType TaxType1,
                                           TaxType TaxType2)

            => TaxType1.CompareTo(TaxType2) <= 0;

        #endregion

        #region Operator >  (TaxType1, TaxType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxType1">A tax type.</param>
        /// <param name="TaxType2">Another tax type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TaxType TaxType1,
                                          TaxType TaxType2)

            => TaxType1.CompareTo(TaxType2) > 0;

        #endregion

        #region Operator >= (TaxType1, TaxType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TaxType1">A tax type.</param>
        /// <param name="TaxType2">Another tax type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TaxType TaxType1,
                                           TaxType TaxType2)

            => TaxType1.CompareTo(TaxType2) >= 0;

        #endregion

        #endregion

        #region IComparable<TaxType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two tax types.
        /// </summary>
        /// <param name="Object">A tax type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TaxType taxType
                   ? CompareTo(taxType)
                   : throw new ArgumentException("The given object is not a tax type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TaxType)

        /// <summary>
        /// Compares two tax types.
        /// </summary>
        /// <param name="TaxType">A tax type to compare with.</param>
        public Int32 CompareTo(TaxType TaxType)

            => String.Compare(InternalId,
                              TaxType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TaxType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tax types for equality.
        /// </summary>
        /// <param name="Object">A tax type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TaxType taxType &&
                   Equals(taxType);

        #endregion

        #region Equals(TaxType)

        /// <summary>
        /// Compares two tax types for equality.
        /// </summary>
        /// <param name="TaxType">A tax type to compare with.</param>
        public Boolean Equals(TaxType TaxType)

            => String.Equals(InternalId,
                             TaxType.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
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
