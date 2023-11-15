/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonTypes
{

    /// <summary>
    /// Extension methods for descriptions.
    /// [max 160]
    /// </summary>
    public static class DescriptionExtensions
    {

        /// <summary>
        /// Indicates whether this description is null or empty.
        /// </summary>
        /// <param name="Description">A description.</param>
        public static Boolean IsNullOrEmpty(this Description? Description)
            => !Description.HasValue || Description.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this description is null or empty.
        /// </summary>
        /// <param name="Description">A description.</param>
        public static Boolean IsNotNullOrEmpty(this Description? Description)
            => Description.HasValue && Description.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A description.
    /// </summary>
    public readonly struct Description : IId,
                                         IEquatable<Description>,
                                         IComparable<Description>
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
        /// The length of the description.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new description based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a description.</param>
        private Description(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a description.
        /// </summary>
        /// <param name="Text">A text representation of a description.</param>
        public static Description Parse(String Text)
        {

            if (TryParse(Text, out var description))
                return description;

            throw new ArgumentException($"Invalid text representation of a description: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a description.
        /// </summary>
        /// <param name="Text">A text representation of a description.</param>
        public static Description? TryParse(String Text)
        {

            if (TryParse(Text, out var description))
                return description;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out Description)

        /// <summary>
        /// Try to parse the given text as a description.
        /// </summary>
        /// <param name="Text">A text representation of a description.</param>
        /// <param name="Description">The parsed description.</param>
        public static Boolean TryParse(String Text, out Description Description)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                Description = new Description(Text.SubstringMax(160));
                return true;
            }

            Description = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this description.
        /// </summary>
        public Description Clone

            => new(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (Description1, Description2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Description1">A description.</param>
        /// <param name="Description2">Another description.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Description Description1,
                                           Description Description2)

            => Description1.Equals(Description2);

        #endregion

        #region Operator != (Description1, Description2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Description1">A description.</param>
        /// <param name="Description2">Another description.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Description Description1,
                                           Description Description2)

            => !Description1.Equals(Description2);

        #endregion

        #region Operator <  (Description1, Description2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Description1">A description.</param>
        /// <param name="Description2">Another description.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Description Description1,
                                          Description Description2)

            => Description1.CompareTo(Description2) < 0;

        #endregion

        #region Operator <= (Description1, Description2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Description1">A description.</param>
        /// <param name="Description2">Another description.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Description Description1,
                                           Description Description2)

            => Description1.CompareTo(Description2) <= 0;

        #endregion

        #region Operator >  (Description1, Description2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Description1">A description.</param>
        /// <param name="Description2">Another description.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Description Description1,
                                          Description Description2)

            => Description1.CompareTo(Description2) > 0;

        #endregion

        #region Operator >= (Description1, Description2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Description1">A description.</param>
        /// <param name="Description2">Another description.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Description Description1,
                                           Description Description2)

            => Description1.CompareTo(Description2) >= 0;

        #endregion

        #endregion

        #region IComparable<Description> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two descriptions.
        /// </summary>
        /// <param name="Object">A description to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Description description
                   ? CompareTo(description)
                   : throw new ArgumentException("The given object is not a description!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Description)

        /// <summary>
        /// Compares two descriptions.
        /// </summary>
        /// <param name="Description">A description to compare with.</param>
        public Int32 CompareTo(Description Description)

            => String.Compare(InternalId,
                              Description.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<Description> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two descriptions for equality.
        /// </summary>
        /// <param name="Object">A description to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Description description &&
                   Equals(description);

        #endregion

        #region Equals(Description)

        /// <summary>
        /// Compares two descriptions for equality.
        /// </summary>
        /// <param name="Description">A description to compare with.</param>
        public Boolean Equals(Description Description)

            => String.Equals(InternalId,
                             Description.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.GetHashCode() ?? 0;

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
