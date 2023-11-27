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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for list directory statuss.
    /// </summary>
    public static class ListDirectoryStatusExtensions
    {

        /// <summary>
        /// Indicates whether this list directory status is null or empty.
        /// </summary>
        /// <param name="ListDirectoryStatus">A list directory status.</param>
        public static Boolean IsNullOrEmpty(this ListDirectoryStatus? ListDirectoryStatus)
            => !ListDirectoryStatus.HasValue || ListDirectoryStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this list directory status is null or empty.
        /// </summary>
        /// <param name="ListDirectoryStatus">A list directory status.</param>
        public static Boolean IsNotNullOrEmpty(this ListDirectoryStatus? ListDirectoryStatus)
            => ListDirectoryStatus.HasValue && ListDirectoryStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A list directory status.
    /// </summary>
    public readonly struct ListDirectoryStatus : IId,
                                                 IEquatable<ListDirectoryStatus>,
                                                 IComparable<ListDirectoryStatus>
    {

        #region Data

        private readonly static Dictionary<String, ListDirectoryStatus>  lookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly        String                                InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this list directory status is null or empty.
        /// </summary>
        public readonly Boolean  IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this list directory status is NOT null or empty.
        /// </summary>
        public readonly Boolean  IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the list directory status.
        /// </summary>
        public readonly UInt64   Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new list directory status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a list directory status.</param>
        private ListDirectoryStatus(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static ListDirectoryStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new ListDirectoryStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a list directory status.
        /// </summary>
        /// <param name="Text">A text representation of a list directory status.</param>
        public static ListDirectoryStatus Parse(String Text)
        {

            if (TryParse(Text, out var listDirectoryStatus))
                return listDirectoryStatus;

            throw new ArgumentException($"Invalid text representation of a list directory status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as list directory status.
        /// </summary>
        /// <param name="Text">A text representation of a list directory status.</param>
        public static ListDirectoryStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var listDirectoryStatus))
                return listDirectoryStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ListDirectoryStatus)

        /// <summary>
        /// Try to parse the given text as list directory status.
        /// </summary>
        /// <param name="Text">A text representation of a list directory status.</param>
        /// <param name="ListDirectoryStatus">The parsed list directory status.</param>
        public static Boolean TryParse(String Text, out ListDirectoryStatus ListDirectoryStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out ListDirectoryStatus))
                    ListDirectoryStatus = Register(Text);

                return true;

            }

            ListDirectoryStatus = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this list directory status.
        /// </summary>
        public ListDirectoryStatus Clone

            => new(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The ListDirectoryRequest has been accepted and the file is included in the response.
        /// </summary>
        public static ListDirectoryStatus Success             { get; }
            = Register("Success");

        /// <summary>
        /// The ListDirectoryRequest has been accepted, but the file was not found.
        /// </summary>
        public static ListDirectoryStatus NotFound            { get; }
            = Register("NotFound");

        /// <summary>
        /// The ListDirectoryRequest has been rejected.
        /// </summary>
        public static ListDirectoryStatus Rejected            { get; }
            = Register("Rejected");

        /// <summary>
        /// The digital signature(s) of the message is/are invalid.
        /// </summary>
        public static ListDirectoryStatus InvalidSignature    { get; }
            = Register("InvalidSignature");

        #endregion


        #region Operator overloading

        #region Operator == (ListDirectoryStatus1, ListDirectoryStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ListDirectoryStatus1">A list directory status.</param>
        /// <param name="ListDirectoryStatus2">Another list directory status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ListDirectoryStatus ListDirectoryStatus1,
                                           ListDirectoryStatus ListDirectoryStatus2)

            => ListDirectoryStatus1.Equals(ListDirectoryStatus2);

        #endregion

        #region Operator != (ListDirectoryStatus1, ListDirectoryStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ListDirectoryStatus1">A list directory status.</param>
        /// <param name="ListDirectoryStatus2">Another list directory status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ListDirectoryStatus ListDirectoryStatus1,
                                           ListDirectoryStatus ListDirectoryStatus2)

            => !ListDirectoryStatus1.Equals(ListDirectoryStatus2);

        #endregion

        #region Operator <  (ListDirectoryStatus1, ListDirectoryStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ListDirectoryStatus1">A list directory status.</param>
        /// <param name="ListDirectoryStatus2">Another list directory status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ListDirectoryStatus ListDirectoryStatus1,
                                          ListDirectoryStatus ListDirectoryStatus2)

            => ListDirectoryStatus1.CompareTo(ListDirectoryStatus2) < 0;

        #endregion

        #region Operator <= (ListDirectoryStatus1, ListDirectoryStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ListDirectoryStatus1">A list directory status.</param>
        /// <param name="ListDirectoryStatus2">Another list directory status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ListDirectoryStatus ListDirectoryStatus1,
                                           ListDirectoryStatus ListDirectoryStatus2)

            => ListDirectoryStatus1.CompareTo(ListDirectoryStatus2) <= 0;

        #endregion

        #region Operator >  (ListDirectoryStatus1, ListDirectoryStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ListDirectoryStatus1">A list directory status.</param>
        /// <param name="ListDirectoryStatus2">Another list directory status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ListDirectoryStatus ListDirectoryStatus1,
                                          ListDirectoryStatus ListDirectoryStatus2)

            => ListDirectoryStatus1.CompareTo(ListDirectoryStatus2) > 0;

        #endregion

        #region Operator >= (ListDirectoryStatus1, ListDirectoryStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ListDirectoryStatus1">A list directory status.</param>
        /// <param name="ListDirectoryStatus2">Another list directory status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ListDirectoryStatus ListDirectoryStatus1,
                                           ListDirectoryStatus ListDirectoryStatus2)

            => ListDirectoryStatus1.CompareTo(ListDirectoryStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<ListDirectoryStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two list directory statuss.
        /// </summary>
        /// <param name="Object">A list directory status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ListDirectoryStatus listDirectoryStatus
                   ? CompareTo(listDirectoryStatus)
                   : throw new ArgumentException("The given object is not list directory status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ListDirectoryStatus)

        /// <summary>
        /// Compares two list directory statuss.
        /// </summary>
        /// <param name="ListDirectoryStatus">A list directory status to compare with.</param>
        public Int32 CompareTo(ListDirectoryStatus ListDirectoryStatus)

            => String.Compare(InternalId,
                              ListDirectoryStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ListDirectoryStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two list directory statuss for equality.
        /// </summary>
        /// <param name="Object">A list directory status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ListDirectoryStatus listDirectoryStatus &&
                   Equals(listDirectoryStatus);

        #endregion

        #region Equals(ListDirectoryStatus)

        /// <summary>
        /// Compares two list directory statuss for equality.
        /// </summary>
        /// <param name="ListDirectoryStatus">A list directory status to compare with.</param>
        public Boolean Equals(ListDirectoryStatus ListDirectoryStatus)

            => String.Equals(InternalId,
                             ListDirectoryStatus.InternalId,
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
