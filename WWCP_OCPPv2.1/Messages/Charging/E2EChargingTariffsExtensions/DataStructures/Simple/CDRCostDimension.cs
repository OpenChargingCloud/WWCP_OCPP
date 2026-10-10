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
    /// Extension methods for cost dimensions.
    /// </summary>
    public static class CDRCostDimensionExtensions
    {

        /// <summary>
        /// Indicates whether this cost dimension is null or empty.
        /// </summary>
        /// <param name="CDRCostDimension">A cost dimension.</param>
        public static Boolean IsNullOrEmpty(this CDRCostDimension? CDRCostDimension)
            => !CDRCostDimension.HasValue || CDRCostDimension.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this cost dimension is NOT null or empty.
        /// </summary>
        /// <param name="CDRCostDimension">A cost dimension.</param>
        public static Boolean IsNotNullOrEmpty(this CDRCostDimension? CDRCostDimension)
            => CDRCostDimension.HasValue && CDRCostDimension.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a cost dimension.
    /// </summary>
    public readonly struct CDRCostDimension : IId<CDRCostDimension>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this cost dimension is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this cost dimension is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the cost dimension.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64)InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new cost dimension based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a cost dimension.</param>
        private CDRCostDimension(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a cost dimension.
        /// </summary>
        /// <param name="Text">A text representation of a cost dimension.</param>
        public static CDRCostDimension Parse(String Text)
        {

            if (TryParse(Text, out var costDimension))
                return costDimension;

            throw new ArgumentException($"Invalid text representation of a cost dimension: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a cost dimension.
        /// </summary>
        /// <param name="Text">A text representation of a cost dimension.</param>
        public static CDRCostDimension? TryParse(String Text)
        {

            if (TryParse(Text, out var costDimension))
                return costDimension;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CDRCostDimension)

        /// <summary>
        /// Try to parse the given text as a cost dimension.
        /// </summary>
        /// <param name="Text">A text representation of a cost dimension.</param>
        /// <param name="CDRCostDimension">The parsed cost dimension.</param>
        public static Boolean TryParse(String Text, out CDRCostDimension CDRCostDimension)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    CDRCostDimension = new CDRCostDimension(Text);
                    return true;
                }
                catch
                { }
            }

            CDRCostDimension = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this cost dimension.
        /// </summary>
        public CDRCostDimension Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Total amount of energy (dis-)charged during this charging period, defined in kWh.
        /// When negative, more energy was feed into the grid then charged into the EV.
        /// </summary>
        public static CDRCostDimension kWh
            => new ("kWh");

        /// <summary>
        /// Sum of the maximum current over all phases, reached during this charging period.
        /// When negative, more energy was feed into the grid then charged into the EV.
        /// </summary>
        public static CDRCostDimension MaxA
            => new ("MaxA");

        /// <summary>
        /// Sum of the minimum current over all phases, reached during this charging period.
        /// When negative, more energy was feed into the grid then charged into the EV.
        /// </summary>
        public static CDRCostDimension MinA
            => new ("MinA");

        /// <summary>
        /// Maximum power reached during this charging period.
        /// When negative, more energy was feed into the grid then charged into the EV.
        /// </summary>
        public static CDRCostDimension MaxKW
            => new ("MaxKW");

        /// <summary>
        /// Minumum power reached during this charging period.
        /// When negative, more energy was feed into the grid then charged into the EV.
        /// </summary>
        public static CDRCostDimension MinKW
            => new ("MinKW");

        /// <summary>
        /// Time reserved for future charging during this charging period.
        /// </summary>
        public static CDRCostDimension ReservationHours
            => new ("ReservationHours");

        /// <summary>
        /// Time charging during this charging period.
        /// </summary>
        public static CDRCostDimension ChargeHours
            => new ("ChargeHours");

        /// <summary>
        /// Time not charging during this charging period.
        /// </summary>
        public static CDRCostDimension IdleHours
            => new ("IdleHours");

        #endregion


        #region Operator overloading

        #region Operator == (CostDimension1, CostDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimension1">A cost dimension.</param>
        /// <param name="CostDimension2">Another cost dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator ==(CDRCostDimension CostDimension1,
                                           CDRCostDimension CostDimension2)

            => CostDimension1.Equals(CostDimension2);

        #endregion

        #region Operator != (CostDimension1, CostDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimension1">A cost dimension.</param>
        /// <param name="CostDimension2">Another cost dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator !=(CDRCostDimension CostDimension1,
                                           CDRCostDimension CostDimension2)

            => !CostDimension1.Equals(CostDimension2);

        #endregion

        #region Operator <  (CostDimension1, CostDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimension1">A cost dimension.</param>
        /// <param name="CostDimension2">Another cost dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <(CDRCostDimension CostDimension1,
                                          CDRCostDimension CostDimension2)

            => CostDimension1.CompareTo(CostDimension2) < 0;

        #endregion

        #region Operator <= (CostDimension1, CostDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimension1">A cost dimension.</param>
        /// <param name="CostDimension2">Another cost dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <=(CDRCostDimension CostDimension1,
                                           CDRCostDimension CostDimension2)

            => CostDimension1.CompareTo(CostDimension2) <= 0;

        #endregion

        #region Operator >  (CostDimension1, CostDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimension1">A cost dimension.</param>
        /// <param name="CostDimension2">Another cost dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >(CDRCostDimension CostDimension1,
                                          CDRCostDimension CostDimension2)

            => CostDimension1.CompareTo(CostDimension2) > 0;

        #endregion

        #region Operator >= (CostDimension1, CostDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimension1">A cost dimension.</param>
        /// <param name="CostDimension2">Another cost dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >=(CDRCostDimension CostDimension1,
                                           CDRCostDimension CostDimension2)

            => CostDimension1.CompareTo(CostDimension2) >= 0;

        #endregion

        #endregion

        #region IComparable<CDRCostDimension> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two cost dimensions.
        /// </summary>
        /// <param name="Object">A cost dimension to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CDRCostDimension costDimension
                   ? CompareTo(costDimension)
                   : throw new ArgumentException("The given object is not a cost dimension!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CDRCostDimension)

        /// <summary>
        /// Compares two cost dimensions.
        /// </summary>
        /// <param name="CDRCostDimension">A cost dimension to compare with.</param>
        public Int32 CompareTo(CDRCostDimension CDRCostDimension)

            => String.Compare(InternalId,
                              CDRCostDimension.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CDRCostDimension> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cost dimensions for equality.
        /// </summary>
        /// <param name="Object">A cost dimension to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CDRCostDimension costDimension &&
                   Equals(costDimension);

        #endregion

        #region Equals(CDRCostDimension)

        /// <summary>
        /// Compares two cost dimensions for equality.
        /// </summary>
        /// <param name="CDRCostDimension">A cost dimension to compare with.</param>
        public Boolean Equals(CDRCostDimension CDRCostDimension)

            => String.Equals(InternalId,
                             CDRCostDimension.InternalId,
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
