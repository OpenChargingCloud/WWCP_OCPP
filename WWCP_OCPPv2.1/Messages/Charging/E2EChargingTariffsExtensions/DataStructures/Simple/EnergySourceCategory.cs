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
    /// Extension methods for energy source categories.
    /// </summary>
    public static class EnergySourceCategoryExtensions
    {

        /// <summary>
        /// Indicates whether this energy source category is null or empty.
        /// </summary>
        /// <param name="EnergySourceCategory">An energy source category.</param>
        public static Boolean IsNullOrEmpty(this EnergySourceCategory? EnergySourceCategory)
            => !EnergySourceCategory.HasValue || EnergySourceCategory.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this energy source category is NOT null or empty.
        /// </summary>
        /// <param name="EnergySourceCategory">An energy source category.</param>
        public static Boolean IsNotNullOrEmpty(this EnergySourceCategory? EnergySourceCategory)
            => EnergySourceCategory.HasValue && EnergySourceCategory.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// Energy source categories.
    /// </summary>
    public readonly struct EnergySourceCategory : IId<EnergySourceCategory>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this energy source category is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this energy source category is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the energy source category.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new energy source category based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a energy source category.</param>
        private EnergySourceCategory(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a energy source category.
        /// </summary>
        /// <param name="Text">A text representation of a energy source category.</param>
        public static EnergySourceCategory Parse(String Text)
        {

            if (TryParse(Text, out var energySourceCategory))
                return energySourceCategory;

            throw new ArgumentException($"Invalid text representation of a energy source category: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a energy source category.
        /// </summary>
        /// <param name="Text">A text representation of a energy source category.</param>
        public static EnergySourceCategory? TryParse(String Text)
        {

            if (TryParse(Text, out var energySourceCategory))
                return energySourceCategory;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EnergySourceCategory)

        /// <summary>
        /// Try to parse the given text as a energy source category.
        /// </summary>
        /// <param name="Text">A text representation of a energy source category.</param>
        /// <param name="EnergySourceCategory">The parsed energy source category.</param>
        public static Boolean TryParse(String Text, out EnergySourceCategory EnergySourceCategory)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    EnergySourceCategory = new EnergySourceCategory(Text);
                    return true;
                }
                catch
                { }
            }

            EnergySourceCategory = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this energy source category.
        /// </summary>
        public EnergySourceCategory Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Nuclear power sources.
        /// </summary>
        public static EnergySourceCategory NUCLEAR
            => new ("NUCLEAR");

        /// <summary>
        /// All kinds of fossil power sources.
        /// </summary>
        public static EnergySourceCategory GENERAL_FOSSIL
            => new ("GENERAL_FOSSIL");

        /// <summary>
        /// Fossil power from coal.
        /// </summary>
        public static EnergySourceCategory COAL
            => new ("COAL");

        /// <summary>
        /// Fossil power from gas.
        /// </summary>
        public static EnergySourceCategory GAS
            => new ("GAS");

        /// <summary>
        /// All kinds of regenerative power sources.
        /// </summary>
        public static EnergySourceCategory GENERAL_GREEN
            => new ("GENERAL_GREEN");

        /// <summary>
        /// Regenerative power from PV.
        /// </summary>
        public static EnergySourceCategory SOLAR
            => new ("SOLAR");

        /// <summary>
        /// Regenerative power from wind turbines.
        /// </summary>
        public static EnergySourceCategory WIND
            => new ("WIND");

        /// <summary>
        /// Regenerative power from water turbines.
        /// </summary>
        public static EnergySourceCategory WATER
            => new ("WATER");

        #endregion


        #region Operator overloading

        #region Operator == (EnergySourceCategory1, EnergySourceCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySourceCategory1">An energy source category.</param>
        /// <param name="EnergySourceCategory2">Another energy source category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EnergySourceCategory EnergySourceCategory1,
                                           EnergySourceCategory EnergySourceCategory2)

            => EnergySourceCategory1.Equals(EnergySourceCategory2);

        #endregion

        #region Operator != (EnergySourceCategory1, EnergySourceCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySourceCategory1">An energy source category.</param>
        /// <param name="EnergySourceCategory2">Another energy source category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EnergySourceCategory EnergySourceCategory1,
                                           EnergySourceCategory EnergySourceCategory2)

            => !EnergySourceCategory1.Equals(EnergySourceCategory2);

        #endregion

        #region Operator <  (EnergySourceCategory1, EnergySourceCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySourceCategory1">An energy source category.</param>
        /// <param name="EnergySourceCategory2">Another energy source category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EnergySourceCategory EnergySourceCategory1,
                                          EnergySourceCategory EnergySourceCategory2)

            => EnergySourceCategory1.CompareTo(EnergySourceCategory2) < 0;

        #endregion

        #region Operator <= (EnergySourceCategory1, EnergySourceCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySourceCategory1">An energy source category.</param>
        /// <param name="EnergySourceCategory2">Another energy source category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EnergySourceCategory EnergySourceCategory1,
                                           EnergySourceCategory EnergySourceCategory2)

            => EnergySourceCategory1.CompareTo(EnergySourceCategory2) <= 0;

        #endregion

        #region Operator >  (EnergySourceCategory1, EnergySourceCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySourceCategory1">An energy source category.</param>
        /// <param name="EnergySourceCategory2">Another energy source category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EnergySourceCategory EnergySourceCategory1,
                                          EnergySourceCategory EnergySourceCategory2)

            => EnergySourceCategory1.CompareTo(EnergySourceCategory2) > 0;

        #endregion

        #region Operator >= (EnergySourceCategory1, EnergySourceCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergySourceCategory1">An energy source category.</param>
        /// <param name="EnergySourceCategory2">Another energy source category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EnergySourceCategory EnergySourceCategory1,
                                           EnergySourceCategory EnergySourceCategory2)

            => EnergySourceCategory1.CompareTo(EnergySourceCategory2) >= 0;

        #endregion

        #endregion

        #region IComparable<EnergySourceCategory> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two energy source categories.
        /// </summary>
        /// <param name="Object">An energy source category to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EnergySourceCategory energySourceCategory
                   ? CompareTo(energySourceCategory)
                   : throw new ArgumentException("The given object is not a energy source category!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EnergySourceCategory)

        /// <summary>
        /// Compares two energy source categories.
        /// </summary>
        /// <param name="EnergySourceCategory">An energy source category to compare with.</param>
        public Int32 CompareTo(EnergySourceCategory EnergySourceCategory)

            => String.Compare(InternalId,
                              EnergySourceCategory.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<EnergySourceCategory> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two energy source categories for equality.
        /// </summary>
        /// <param name="Object">An energy source category to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EnergySourceCategory energySourceCategory &&
                   Equals(energySourceCategory);

        #endregion

        #region Equals(EnergySourceCategory)

        /// <summary>
        /// Compares two energy source categories for equality.
        /// </summary>
        /// <param name="EnergySourceCategory">An energy source category to compare with.</param>
        public Boolean Equals(EnergySourceCategory EnergySourceCategory)

            => String.Equals(InternalId,
                             EnergySourceCategory.InternalId,
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
