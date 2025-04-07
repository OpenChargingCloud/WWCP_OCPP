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
    /// Extension methods for Distributed Energy Resource (DER) units.
    /// </summary>
    public static class DERUnitExtensions
    {

        /// <summary>
        /// Indicates whether this DER unit is null or empty.
        /// </summary>
        /// <param name="DERUnit">A DER unit.</param>
        public static Boolean IsNullOrEmpty(this DERUnit? DERUnit)
            => !DERUnit.HasValue || DERUnit.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this DER unit is null or empty.
        /// </summary>
        /// <param name="DERUnit">A DER unit.</param>
        public static Boolean IsNotNullOrEmpty(this DERUnit? DERUnit)
            => DERUnit.HasValue && DERUnit.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A Distributed Energy Resource (DER) unit.
    /// </summary>
    public readonly struct DERUnit : IId,
                                     IEquatable<DERUnit>,
                                     IComparable<DERUnit>
    {

        #region Data

        private readonly static Dictionary<String, DERUnit>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                      InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this DER unit is null or empty.
        /// </summary>
        public readonly  Boolean                      IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this DER unit is NOT null or empty.
        /// </summary>
        public readonly  Boolean                      IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the DER unit.
        /// </summary>
        public readonly  UInt64                       Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered DER units.
        /// </summary>
        public static    IEnumerable<DERUnit>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DER unit based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a DER unit.</param>
        private DERUnit(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static DERUnit Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new DERUnit(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a DER unit.
        /// </summary>
        /// <param name="Text">A text representation of a DER unit.</param>
        public static DERUnit Parse(String Text)
        {

            if (TryParse(Text, out var derUnit))
                return derUnit;

            throw new ArgumentException($"Invalid text representation of a DER unit: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a DER unit.
        /// </summary>
        /// <param name="Text">A text representation of a DER unit.</param>
        public static DERUnit? TryParse(String Text)
        {

            if (TryParse(Text, out var derUnit))
                return derUnit;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out DERUnit)

        /// <summary>
        /// Try to parse the given text as a DER unit.
        /// </summary>
        /// <param name="Text">A text representation of a DER unit.</param>
        /// <param name="DERUnit">The parsed DER unit.</param>
        public static Boolean TryParse(String Text, out DERUnit DERUnit)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out DERUnit))
                    DERUnit = Register(Text);

                return true;

            }

            DERUnit = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this DER unit.
        /// </summary>
        public DERUnit Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Not Applicable
        /// </summary>
        public static DERUnit  Not_Applicable    { get; }
            = Register("Not_Applicable");

        /// <summary>
        /// Percentage of Maximum Active Power (PctMaxW)
        /// </summary>
        public static DERUnit  PctMaxW           { get; }
            = Register("PctMaxW");

        /// <summary>
        /// Percentage of Maximum Reactive Power (PctMaxVar)
        /// </summary>
        public static DERUnit  PctMaxVar         { get; }
            = Register("PctMaxVar");

        /// <summary>
        /// Percentage of Available Active Power (PctWAvail)
        /// </summary>
        public static DERUnit  PctWAvail         { get; }
            = Register("PctWAvail");

        /// <summary>
        /// Percentage of Available Reactive Power (PctVarAvail)
        /// </summary>
        public static DERUnit  PctVarAvail       { get; }
            = Register("PctVarAvail");

        /// <summary>
        /// Percentage of Effective Voltage (PctEffectiveV)
        /// </summary>
        public static DERUnit  PctEffectiveV     { get; }
            = Register("PctEffectiveV");

        #endregion


        #region Operator overloading

        #region Operator == (DERUnit1, DERUnit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERUnit1">A DER unit.</param>
        /// <param name="DERUnit2">Another DER unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DERUnit DERUnit1,
                                           DERUnit DERUnit2)

            => DERUnit1.Equals(DERUnit2);

        #endregion

        #region Operator != (DERUnit1, DERUnit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERUnit1">A DER unit.</param>
        /// <param name="DERUnit2">Another DER unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DERUnit DERUnit1,
                                           DERUnit DERUnit2)

            => !DERUnit1.Equals(DERUnit2);

        #endregion

        #region Operator <  (DERUnit1, DERUnit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERUnit1">A DER unit.</param>
        /// <param name="DERUnit2">Another DER unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (DERUnit DERUnit1,
                                          DERUnit DERUnit2)

            => DERUnit1.CompareTo(DERUnit2) < 0;

        #endregion

        #region Operator <= (DERUnit1, DERUnit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERUnit1">A DER unit.</param>
        /// <param name="DERUnit2">Another DER unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (DERUnit DERUnit1,
                                           DERUnit DERUnit2)

            => DERUnit1.CompareTo(DERUnit2) <= 0;

        #endregion

        #region Operator >  (DERUnit1, DERUnit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERUnit1">A DER unit.</param>
        /// <param name="DERUnit2">Another DER unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (DERUnit DERUnit1,
                                          DERUnit DERUnit2)

            => DERUnit1.CompareTo(DERUnit2) > 0;

        #endregion

        #region Operator >= (DERUnit1, DERUnit2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERUnit1">A DER unit.</param>
        /// <param name="DERUnit2">Another DER unit.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (DERUnit DERUnit1,
                                           DERUnit DERUnit2)

            => DERUnit1.CompareTo(DERUnit2) >= 0;

        #endregion

        #endregion

        #region IComparable<DERUnit> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two DER units.
        /// </summary>
        /// <param name="Object">A DER unit to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is DERUnit derUnit
                   ? CompareTo(derUnit)
                   : throw new ArgumentException("The given object is not a DER unit!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(DERUnit)

        /// <summary>
        /// Compares two DER units.
        /// </summary>
        /// <param name="DERUnit">A DER unit to compare with.</param>
        public Int32 CompareTo(DERUnit DERUnit)

            => String.Compare(InternalId,
                              DERUnit.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<DERUnit> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DER units for equality.
        /// </summary>
        /// <param name="Object">A DER unit to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DERUnit derUnit &&
                   Equals(derUnit);

        #endregion

        #region Equals(DERUnit)

        /// <summary>
        /// Compares two DER units for equality.
        /// </summary>
        /// <param name="DERUnit">A DER unit to compare with.</param>
        public Boolean Equals(DERUnit DERUnit)

            => String.Equals(InternalId,
                             DERUnit.InternalId,
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
