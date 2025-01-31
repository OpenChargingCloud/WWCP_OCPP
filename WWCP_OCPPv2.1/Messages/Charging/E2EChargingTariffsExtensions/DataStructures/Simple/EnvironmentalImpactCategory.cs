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
    /// Extension methods for environmental impact categories.
    /// </summary>
    public static class EnvironmentalImpactCategoryExtensions
    {

        /// <summary>
        /// Indicates whether this environmental impact category is null or empty.
        /// </summary>
        /// <param name="EnvironmentalImpactCategory">An environmental impact category.</param>
        public static Boolean IsNullOrEmpty(this EnvironmentalImpactCategory? EnvironmentalImpactCategory)
            => !EnvironmentalImpactCategory.HasValue || EnvironmentalImpactCategory.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this environmental impact category is NOT null or empty.
        /// </summary>
        /// <param name="EnvironmentalImpactCategory">An environmental impact category.</param>
        public static Boolean IsNotNullOrEmpty(this EnvironmentalImpactCategory? EnvironmentalImpactCategory)
            => EnvironmentalImpactCategory.HasValue && EnvironmentalImpactCategory.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// Environmental impact categories.
    /// </summary>
    public readonly struct EnvironmentalImpactCategory : IId<EnvironmentalImpactCategory>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this environmental impact category is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this environmental impact category is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the environmental impact category.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new environmental impact category based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an environmental impact category.</param>
        private EnvironmentalImpactCategory(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an environmental impact category.
        /// </summary>
        /// <param name="Text">A text representation of an environmental impact category.</param>
        public static EnvironmentalImpactCategory Parse(String Text)
        {

            if (TryParse(Text, out var environmentalImpactCategory))
                return environmentalImpactCategory;

            throw new ArgumentException($"Invalid text representation of an environmental impact category: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an environmental impact category.
        /// </summary>
        /// <param name="Text">A text representation of an environmental impact category.</param>
        public static EnvironmentalImpactCategory? TryParse(String Text)
        {

            if (TryParse(Text, out var environmentalImpactCategory))
                return environmentalImpactCategory;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EnvironmentalImpactCategory)

        /// <summary>
        /// Try to parse the given text as an environmental impact category.
        /// </summary>
        /// <param name="Text">A text representation of an environmental impact category.</param>
        /// <param name="EnvironmentalImpactCategory">The parsed environmental impact category.</param>
        public static Boolean TryParse(String Text, out EnvironmentalImpactCategory EnvironmentalImpactCategory)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    EnvironmentalImpactCategory = new EnvironmentalImpactCategory(Text);
                    return true;
                }
                catch
                { }
            }

            EnvironmentalImpactCategory = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this environmental impact category.
        /// </summary>
        public EnvironmentalImpactCategory Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Produced nuclear waste in gramms per kWh.
        /// </summary>
        public static EnvironmentalImpactCategory NUCLEAR_WASTE
            => new ("NUCLEAR_WASTE");

        /// <summary>
        /// Exhausted carbon dioxide in gramms per kWh.
        /// </summary>
        public static EnvironmentalImpactCategory CARBON_DIOXIDE
            => new ("CARBON_DIOXIDE");

        #endregion


        #region Operator overloading

        #region Operator == (EnvironmentalImpactCategory1, EnvironmentalImpactCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpactCategory1">An environmental impact category.</param>
        /// <param name="EnvironmentalImpactCategory2">Another environmental impact category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EnvironmentalImpactCategory EnvironmentalImpactCategory1,
                                           EnvironmentalImpactCategory EnvironmentalImpactCategory2)

            => EnvironmentalImpactCategory1.Equals(EnvironmentalImpactCategory2);

        #endregion

        #region Operator != (EnvironmentalImpactCategory1, EnvironmentalImpactCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpactCategory1">An environmental impact category.</param>
        /// <param name="EnvironmentalImpactCategory2">Another environmental impact category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EnvironmentalImpactCategory EnvironmentalImpactCategory1,
                                           EnvironmentalImpactCategory EnvironmentalImpactCategory2)

            => !EnvironmentalImpactCategory1.Equals(EnvironmentalImpactCategory2);

        #endregion

        #region Operator <  (EnvironmentalImpactCategory1, EnvironmentalImpactCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpactCategory1">An environmental impact category.</param>
        /// <param name="EnvironmentalImpactCategory2">Another environmental impact category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EnvironmentalImpactCategory EnvironmentalImpactCategory1,
                                          EnvironmentalImpactCategory EnvironmentalImpactCategory2)

            => EnvironmentalImpactCategory1.CompareTo(EnvironmentalImpactCategory2) < 0;

        #endregion

        #region Operator <= (EnvironmentalImpactCategory1, EnvironmentalImpactCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpactCategory1">An environmental impact category.</param>
        /// <param name="EnvironmentalImpactCategory2">Another environmental impact category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EnvironmentalImpactCategory EnvironmentalImpactCategory1,
                                           EnvironmentalImpactCategory EnvironmentalImpactCategory2)

            => EnvironmentalImpactCategory1.CompareTo(EnvironmentalImpactCategory2) <= 0;

        #endregion

        #region Operator >  (EnvironmentalImpactCategory1, EnvironmentalImpactCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpactCategory1">An environmental impact category.</param>
        /// <param name="EnvironmentalImpactCategory2">Another environmental impact category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EnvironmentalImpactCategory EnvironmentalImpactCategory1,
                                          EnvironmentalImpactCategory EnvironmentalImpactCategory2)

            => EnvironmentalImpactCategory1.CompareTo(EnvironmentalImpactCategory2) > 0;

        #endregion

        #region Operator >= (EnvironmentalImpactCategory1, EnvironmentalImpactCategory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnvironmentalImpactCategory1">An environmental impact category.</param>
        /// <param name="EnvironmentalImpactCategory2">Another environmental impact category.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EnvironmentalImpactCategory EnvironmentalImpactCategory1,
                                           EnvironmentalImpactCategory EnvironmentalImpactCategory2)

            => EnvironmentalImpactCategory1.CompareTo(EnvironmentalImpactCategory2) >= 0;

        #endregion

        #endregion

        #region IComparable<EnvironmentalImpactCategory> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two environmental impact categories.
        /// </summary>
        /// <param name="Object">An environmental impact category to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EnvironmentalImpactCategory environmentalImpactCategory
                   ? CompareTo(environmentalImpactCategory)
                   : throw new ArgumentException("The given object is not an environmental impact category!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EnvironmentalImpactCategory)

        /// <summary>
        /// Compares two environmental impact categories.
        /// </summary>
        /// <param name="EnvironmentalImpactCategory">An environmental impact category to compare with.</param>
        public Int32 CompareTo(EnvironmentalImpactCategory EnvironmentalImpactCategory)

            => String.Compare(InternalId,
                              EnvironmentalImpactCategory.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<EnvironmentalImpactCategory> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two environmental impact categories for equality.
        /// </summary>
        /// <param name="Object">An environmental impact category to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EnvironmentalImpactCategory environmentalImpactCategory &&
                   Equals(environmentalImpactCategory);

        #endregion

        #region Equals(EnvironmentalImpactCategory)

        /// <summary>
        /// Compares two environmental impact categories for equality.
        /// </summary>
        /// <param name="EnvironmentalImpactCategory">An environmental impact category to compare with.</param>
        public Boolean Equals(EnvironmentalImpactCategory EnvironmentalImpactCategory)

            => String.Equals(InternalId,
                             EnvironmentalImpactCategory.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToUpper().GetHashCode() ?? 0;

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
