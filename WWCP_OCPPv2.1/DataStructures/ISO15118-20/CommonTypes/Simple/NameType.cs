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
    /// Extension methods for names.
    /// </summary>
    public static class NameExtensions
    {

        /// <summary>
        /// Indicates whether this name is null or empty.
        /// </summary>
        /// <param name="Name">A name.</param>
        public static Boolean IsNullOrEmpty(this Name? Name)
            => !Name.HasValue || Name.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this name is null or empty.
        /// </summary>
        /// <param name="Name">A name.</param>
        public static Boolean IsNotNullOrEmpty(this Name? Name)
            => Name.HasValue && Name.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A name.
    /// [max 80]
    /// </summary>
    public readonly struct Name : IId,
                                  IEquatable<Name>,
                                  IComparable<Name>
    {

        #region Data

        /// <summary>
        /// The internal name.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this name is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this name is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the name.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new name based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a name.</param>
        private Name(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a name.
        /// </summary>
        /// <param name="Text">A text representation of a name.</param>
        public static Name Parse(String Text)
        {

            if (TryParse(Text, out var name))
                return name;

            throw new ArgumentException($"Invalid text representation of a name: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a name.
        /// </summary>
        /// <param name="Text">A text representation of a name.</param>
        public static Name? TryParse(String Text)
        {

            if (TryParse(Text, out var name))
                return name;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out Name)

        /// <summary>
        /// Try to parse the given text as a name.
        /// </summary>
        /// <param name="Text">A text representation of a name.</param>
        /// <param name="Name">The parsed name.</param>
        public static Boolean TryParse(String Text, out Name Name)
        {

            Text = Text.Trim().SubstringMax(80);

            if (Text.IsNotNullOrEmpty())
            {
                Name = new Name(Text);
                return true;
            }

            Name = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this name.
        /// </summary>
        public Name Clone

            => new(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (Name1, Name2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Name1">A name.</param>
        /// <param name="Name2">Another name.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Name Name1,
                                           Name Name2)

            => Name1.Equals(Name2);

        #endregion

        #region Operator != (Name1, Name2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Name1">A name.</param>
        /// <param name="Name2">Another name.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Name Name1,
                                           Name Name2)

            => !Name1.Equals(Name2);

        #endregion

        #region Operator <  (Name1, Name2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Name1">A name.</param>
        /// <param name="Name2">Another name.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Name Name1,
                                          Name Name2)

            => Name1.CompareTo(Name2) < 0;

        #endregion

        #region Operator <= (Name1, Name2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Name1">A name.</param>
        /// <param name="Name2">Another name.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Name Name1,
                                           Name Name2)

            => Name1.CompareTo(Name2) <= 0;

        #endregion

        #region Operator >  (Name1, Name2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Name1">A name.</param>
        /// <param name="Name2">Another name.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Name Name1,
                                          Name Name2)

            => Name1.CompareTo(Name2) > 0;

        #endregion

        #region Operator >= (Name1, Name2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Name1">A name.</param>
        /// <param name="Name2">Another name.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Name Name1,
                                           Name Name2)

            => Name1.CompareTo(Name2) >= 0;

        #endregion

        #endregion

        #region IComparable<Name> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two names.
        /// </summary>
        /// <param name="Object">A name to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Name name
                   ? CompareTo(name)
                   : throw new ArgumentException("The given object is not a name!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Name)

        /// <summary>
        /// Compares two names.
        /// </summary>
        /// <param name="Name">A name to compare with.</param>
        public Int32 CompareTo(Name Name)

            => String.Compare(InternalId,
                              Name.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<Name> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two names for equality.
        /// </summary>
        /// <param name="Object">A name to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Name name &&
                   Equals(name);

        #endregion

        #region Equals(Name)

        /// <summary>
        /// Compares two names for equality.
        /// </summary>
        /// <param name="Name">A name to compare with.</param>
        public Boolean Equals(Name Name)

            => String.Equals(InternalId,
                             Name.InternalId,
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
