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
    /// Extension methods for list directory formats.
    /// </summary>
    public static class ListDirectoryFormatExtensions
    {

        /// <summary>
        /// Indicates whether this list directory format is null or empty.
        /// </summary>
        /// <param name="ListDirectoryFormat">A list directory format.</param>
        public static Boolean IsNullOrEmpty(this ListDirectoryFormat? ListDirectoryFormat)
            => !ListDirectoryFormat.HasValue || ListDirectoryFormat.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this list directory format is null or empty.
        /// </summary>
        /// <param name="ListDirectoryFormat">A list directory format.</param>
        public static Boolean IsNotNullOrEmpty(this ListDirectoryFormat? ListDirectoryFormat)
            => ListDirectoryFormat.HasValue && ListDirectoryFormat.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A list directory format.
    /// </summary>
    public readonly struct ListDirectoryFormat : IId,
                                                 IEquatable<ListDirectoryFormat>,
                                                 IComparable<ListDirectoryFormat>
    {

        #region Data

        private readonly static Dictionary<String, ListDirectoryFormat>  lookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly        String                                InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this list directory format is null or empty.
        /// </summary>
        public readonly Boolean  IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this list directory format is NOT null or empty.
        /// </summary>
        public readonly Boolean  IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the list directory format.
        /// </summary>
        public readonly UInt64   Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new list directory format based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a list directory format.</param>
        private ListDirectoryFormat(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static ListDirectoryFormat Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new ListDirectoryFormat(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a list directory format.
        /// </summary>
        /// <param name="Text">A text representation of a list directory format.</param>
        public static ListDirectoryFormat Parse(String Text)
        {

            if (TryParse(Text, out var listDirectoryFormat))
                return listDirectoryFormat;

            throw new ArgumentException($"Invalid text representation of a list directory format: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as list directory format.
        /// </summary>
        /// <param name="Text">A text representation of a list directory format.</param>
        public static ListDirectoryFormat? TryParse(String Text)
        {

            if (TryParse(Text, out var listDirectoryFormat))
                return listDirectoryFormat;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ListDirectoryFormat)

        /// <summary>
        /// Try to parse the given text as list directory format.
        /// </summary>
        /// <param name="Text">A text representation of a list directory format.</param>
        /// <param name="ListDirectoryFormat">The parsed list directory format.</param>
        public static Boolean TryParse(String Text, out ListDirectoryFormat ListDirectoryFormat)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out ListDirectoryFormat))
                    ListDirectoryFormat = Register(Text);

                return true;

            }

            ListDirectoryFormat = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this list directory format.
        /// </summary>
        public ListDirectoryFormat Clone

            => new(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// JSON
        /// </summary>
        public static ListDirectoryFormat JSON                { get; }
            = Register("JSON");

        /// <summary>
        /// JSON with (file) meta data.
        /// </summary>
        public static ListDirectoryFormat JSONWithMetadata    { get; }
            = Register("JSONWithMetadata");

        /// <summary>
        /// An UNIX-like text based tree view.
        /// </summary>
        public static ListDirectoryFormat TreeView            { get; }
            = Register("TreeView");

        /// <summary>
        /// A text list.
        /// </summary>
        public static ListDirectoryFormat PrefixList          { get; }
            = Register("PrefixList");

        #endregion


        #region Operator overloading

        #region Operator == (ListDirectoryFormat1, ListDirectoryFormat2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ListDirectoryFormat1">A list directory format.</param>
        /// <param name="ListDirectoryFormat2">Another list directory format.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ListDirectoryFormat ListDirectoryFormat1,
                                           ListDirectoryFormat ListDirectoryFormat2)

            => ListDirectoryFormat1.Equals(ListDirectoryFormat2);

        #endregion

        #region Operator != (ListDirectoryFormat1, ListDirectoryFormat2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ListDirectoryFormat1">A list directory format.</param>
        /// <param name="ListDirectoryFormat2">Another list directory format.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ListDirectoryFormat ListDirectoryFormat1,
                                           ListDirectoryFormat ListDirectoryFormat2)

            => !ListDirectoryFormat1.Equals(ListDirectoryFormat2);

        #endregion

        #region Operator <  (ListDirectoryFormat1, ListDirectoryFormat2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ListDirectoryFormat1">A list directory format.</param>
        /// <param name="ListDirectoryFormat2">Another list directory format.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ListDirectoryFormat ListDirectoryFormat1,
                                          ListDirectoryFormat ListDirectoryFormat2)

            => ListDirectoryFormat1.CompareTo(ListDirectoryFormat2) < 0;

        #endregion

        #region Operator <= (ListDirectoryFormat1, ListDirectoryFormat2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ListDirectoryFormat1">A list directory format.</param>
        /// <param name="ListDirectoryFormat2">Another list directory format.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ListDirectoryFormat ListDirectoryFormat1,
                                           ListDirectoryFormat ListDirectoryFormat2)

            => ListDirectoryFormat1.CompareTo(ListDirectoryFormat2) <= 0;

        #endregion

        #region Operator >  (ListDirectoryFormat1, ListDirectoryFormat2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ListDirectoryFormat1">A list directory format.</param>
        /// <param name="ListDirectoryFormat2">Another list directory format.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ListDirectoryFormat ListDirectoryFormat1,
                                          ListDirectoryFormat ListDirectoryFormat2)

            => ListDirectoryFormat1.CompareTo(ListDirectoryFormat2) > 0;

        #endregion

        #region Operator >= (ListDirectoryFormat1, ListDirectoryFormat2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ListDirectoryFormat1">A list directory format.</param>
        /// <param name="ListDirectoryFormat2">Another list directory format.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ListDirectoryFormat ListDirectoryFormat1,
                                           ListDirectoryFormat ListDirectoryFormat2)

            => ListDirectoryFormat1.CompareTo(ListDirectoryFormat2) >= 0;

        #endregion

        #endregion

        #region IComparable<ListDirectoryFormat> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two list directory formats.
        /// </summary>
        /// <param name="Object">A list directory format to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ListDirectoryFormat listDirectoryFormat
                   ? CompareTo(listDirectoryFormat)
                   : throw new ArgumentException("The given object is not list directory format!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ListDirectoryFormat)

        /// <summary>
        /// Compares two list directory formats.
        /// </summary>
        /// <param name="ListDirectoryFormat">A list directory format to compare with.</param>
        public Int32 CompareTo(ListDirectoryFormat ListDirectoryFormat)

            => String.Compare(InternalId,
                              ListDirectoryFormat.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ListDirectoryFormat> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two list directory formats for equality.
        /// </summary>
        /// <param name="Object">A list directory format to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ListDirectoryFormat listDirectoryFormat &&
                   Equals(listDirectoryFormat);

        #endregion

        #region Equals(ListDirectoryFormat)

        /// <summary>
        /// Compares two list directory formats for equality.
        /// </summary>
        /// <param name="ListDirectoryFormat">A list directory format to compare with.</param>
        public Boolean Equals(ListDirectoryFormat ListDirectoryFormat)

            => String.Equals(InternalId,
                             ListDirectoryFormat.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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
