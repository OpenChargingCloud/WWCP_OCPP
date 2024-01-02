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
    /// Extension methods for cost dimensions.
    /// </summary>
    public static class CostDimensionExtensions
    {

        /// <summary>
        /// Indicates whether this cost dimension is null or empty.
        /// </summary>
        /// <param name="CostDimension">A cost dimension.</param>
        public static Boolean IsNullOrEmpty(this CostDimension? CostDimension)
            => !CostDimension.HasValue || CostDimension.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this cost dimension is NOT null or empty.
        /// </summary>
        /// <param name="CostDimension">A cost dimension.</param>
        public static Boolean IsNotNullOrEmpty(this CostDimension? CostDimension)
            => CostDimension.HasValue && CostDimension.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a cost dimension.
    /// </summary>
    public readonly struct CostDimension : IId<CostDimension>
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
        private CostDimension(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a cost dimension.
        /// </summary>
        /// <param name="Text">A text representation of a cost dimension.</param>
        public static CostDimension Parse(String Text)
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
        public static CostDimension? TryParse(String Text)
        {

            if (TryParse(Text, out var costDimension))
                return costDimension;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CostDimension)

        /// <summary>
        /// Try to parse the given text as a cost dimension.
        /// </summary>
        /// <param name="Text">A text representation of a cost dimension.</param>
        /// <param name="CostDimension">The parsed cost dimension.</param>
        public static Boolean TryParse(String Text, out CostDimension CostDimension)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    CostDimension = new CostDimension(Text);
                    return true;
                }
                catch
                { }
            }

            CostDimension = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this cost dimension.
        /// </summary>
        public CostDimension Clone

            => new(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Total amount of energy (dis-)charged during this charging period, defined in kWh.
        /// When negative, more energy was feed into the grid then charged into the EV.
        /// </summary>
        public static CostDimension kWh
            => new ("kWh");

        /// <summary>
        /// Sum of the maximum current over all phases, reached during this charging period.
        /// When negative, more energy was feed into the grid then charged into the EV.
        /// </summary>
        public static CostDimension MaxA
            => new ("MaxA");

        /// <summary>
        /// Sum of the minimum current over all phases, reached during this charging period.
        /// When negative, more energy was feed into the grid then charged into the EV.
        /// </summary>
        public static CostDimension MinA
            => new("MinA");

        /// <summary>
        /// Maximum power reached during this charging period.
        /// When negative, more energy was feed into the grid then charged into the EV.
        /// </summary>
        public static CostDimension MaxKW
            => new("MaxKW");

        /// <summary>
        /// Minumum power reached during this charging period.
        /// When negative, more energy was feed into the grid then charged into the EV.
        /// </summary>
        public static CostDimension MinKW
            => new("MinKW");

        /// <summary>
        /// Time reserved for future charging during this charging period.
        /// </summary>
        public static CostDimension ReservationHours
            => new ("ReservationHours");

        /// <summary>
        /// Time charging during this charging period.
        /// </summary>
        public static CostDimension ChargeHours
            => new ("ChargeHours");

        /// <summary>
        /// Time not charging during this charging period.
        /// </summary>
        public static CostDimension IdleHours
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
        public static Boolean operator ==(CostDimension CostDimension1,
                                           CostDimension CostDimension2)

            => CostDimension1.Equals(CostDimension2);

        #endregion

        #region Operator != (CostDimension1, CostDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimension1">A cost dimension.</param>
        /// <param name="CostDimension2">Another cost dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator !=(CostDimension CostDimension1,
                                           CostDimension CostDimension2)

            => !CostDimension1.Equals(CostDimension2);

        #endregion

        #region Operator <  (CostDimension1, CostDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimension1">A cost dimension.</param>
        /// <param name="CostDimension2">Another cost dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <(CostDimension CostDimension1,
                                          CostDimension CostDimension2)

            => CostDimension1.CompareTo(CostDimension2) < 0;

        #endregion

        #region Operator <= (CostDimension1, CostDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimension1">A cost dimension.</param>
        /// <param name="CostDimension2">Another cost dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <=(CostDimension CostDimension1,
                                           CostDimension CostDimension2)

            => CostDimension1.CompareTo(CostDimension2) <= 0;

        #endregion

        #region Operator >  (CostDimension1, CostDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimension1">A cost dimension.</param>
        /// <param name="CostDimension2">Another cost dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >(CostDimension CostDimension1,
                                          CostDimension CostDimension2)

            => CostDimension1.CompareTo(CostDimension2) > 0;

        #endregion

        #region Operator >= (CostDimension1, CostDimension2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDimension1">A cost dimension.</param>
        /// <param name="CostDimension2">Another cost dimension.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >=(CostDimension CostDimension1,
                                           CostDimension CostDimension2)

            => CostDimension1.CompareTo(CostDimension2) >= 0;

        #endregion

        #endregion

        #region IComparable<CostDimension> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two cost dimensions.
        /// </summary>
        /// <param name="Object">A cost dimension to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CostDimension costDimension
                   ? CompareTo(costDimension)
                   : throw new ArgumentException("The given object is not a cost dimension!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CostDimension)

        /// <summary>
        /// Compares two cost dimensions.
        /// </summary>
        /// <param name="CostDimension">A cost dimension to compare with.</param>
        public Int32 CompareTo(CostDimension CostDimension)

            => String.Compare(InternalId,
                              CostDimension.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CostDimension> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cost dimensions for equality.
        /// </summary>
        /// <param name="Object">A cost dimension to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CostDimension costDimension &&
                   Equals(costDimension);

        #endregion

        #region Equals(CostDimension)

        /// <summary>
        /// Compares two cost dimensions for equality.
        /// </summary>
        /// <param name="CostDimension">A cost dimension to compare with.</param>
        public Boolean Equals(CostDimension CostDimension)

            => String.Equals(InternalId,
                             CostDimension.InternalId,
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
