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
    /// Extension methods for file paths.
    /// </summary>
    public static class FilePathExtensions
    {

        /// <summary>
        /// Indicates whether this file path is null or empty.
        /// </summary>
        /// <param name="FilePath">A file path.</param>
        public static Boolean IsNullOrEmpty(this FilePath? FilePath)
            => !FilePath.HasValue || FilePath.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this file path is null or empty.
        /// </summary>
        /// <param name="FilePath">A file path.</param>
        public static Boolean IsNotNullOrEmpty(this FilePath? FilePath)
            => FilePath.HasValue && FilePath.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A file path.
    /// </summary>
    public readonly struct FilePath : IId,
                                      IEquatable<FilePath>,
                                      IComparable<FilePath>
    {

        #region Data

        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this file path is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this file path is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the file path.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new file path based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a file path.</param>
        private FilePath(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a file path.
        /// </summary>
        /// <param name="Text">A text representation of a file path.</param>
        public static FilePath Parse(String Text)
        {

            if (TryParse(Text, out var filePath))
                return filePath;

            throw new ArgumentException($"Invalid text representation of a file path: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as file path.
        /// </summary>
        /// <param name="Text">A text representation of a file path.</param>
        public static FilePath? TryParse(String Text)
        {

            if (TryParse(Text, out var filePath))
                return filePath;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out FilePath)

        /// <summary>
        /// Try to parse the given text as file path.
        /// </summary>
        /// <param name="Text">A text representation of a file path.</param>
        /// <param name="FilePath">The parsed file path.</param>
        public static Boolean TryParse(String                            Text,
                                       [NotNullWhen(true)] out FilePath  FilePath)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                FilePath = new FilePath(Text);
                return true;
            }

            FilePath = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this file path.
        /// </summary>
        public FilePath Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// An empty file path.
        /// </summary>
        public static FilePath  Empty    { get; }
            = new ("");

        #endregion


        #region Operator overloading

        #region Operator == (FilePath1, FilePath2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FilePath1">A file path.</param>
        /// <param name="FilePath2">Another file path.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (FilePath FilePath1,
                                           FilePath FilePath2)

            => FilePath1.Equals(FilePath2);

        #endregion

        #region Operator != (FilePath1, FilePath2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FilePath1">A file path.</param>
        /// <param name="FilePath2">Another file path.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (FilePath FilePath1,
                                           FilePath FilePath2)

            => !FilePath1.Equals(FilePath2);

        #endregion

        #region Operator <  (FilePath1, FilePath2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FilePath1">A file path.</param>
        /// <param name="FilePath2">Another file path.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (FilePath FilePath1,
                                          FilePath FilePath2)

            => FilePath1.CompareTo(FilePath2) < 0;

        #endregion

        #region Operator <= (FilePath1, FilePath2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FilePath1">A file path.</param>
        /// <param name="FilePath2">Another file path.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (FilePath FilePath1,
                                           FilePath FilePath2)

            => FilePath1.CompareTo(FilePath2) <= 0;

        #endregion

        #region Operator >  (FilePath1, FilePath2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FilePath1">A file path.</param>
        /// <param name="FilePath2">Another file path.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (FilePath FilePath1,
                                          FilePath FilePath2)

            => FilePath1.CompareTo(FilePath2) > 0;

        #endregion

        #region Operator >= (FilePath1, FilePath2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FilePath1">A file path.</param>
        /// <param name="FilePath2">Another file path.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (FilePath FilePath1,
                                           FilePath FilePath2)

            => FilePath1.CompareTo(FilePath2) >= 0;

        #endregion

        #endregion

        #region IComparable<FilePath> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two file paths.
        /// </summary>
        /// <param name="Object">A file path to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is FilePath filePath
                   ? CompareTo(filePath)
                   : throw new ArgumentException("The given object is not file path!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(FilePath)

        /// <summary>
        /// Compares two file paths.
        /// </summary>
        /// <param name="FilePath">A file path to compare with.</param>
        public Int32 CompareTo(FilePath FilePath)

            => String.Compare(InternalId,
                              FilePath.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<FilePath> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two file paths for equality.
        /// </summary>
        /// <param name="Object">A file path to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is FilePath filePath &&
                   Equals(filePath);

        #endregion

        #region Equals(FilePath)

        /// <summary>
        /// Compares two file paths for equality.
        /// </summary>
        /// <param name="FilePath">A file path to compare with.</param>
        public Boolean Equals(FilePath FilePath)

            => String.Equals(InternalId,
                             FilePath.InternalId,
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
