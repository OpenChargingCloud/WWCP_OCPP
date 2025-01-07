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
    /// Extension methods for an ISO 15118 schema version.
    /// </summary>
    public static class ISO15118SchemaVersionExtensions
    {

        /// <summary>
        /// Indicates whether this ISO 15118 schema version is null or empty.
        /// </summary>
        /// <param name="ISO15118SchemaVersion">An ISO 15118 schema version.</param>
        public static Boolean IsNullOrEmpty(this ISO15118SchemaVersion? ISO15118SchemaVersion)
            => !ISO15118SchemaVersion.HasValue || ISO15118SchemaVersion.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this ISO 15118 schema version is null or empty.
        /// </summary>
        /// <param name="ISO15118SchemaVersion">An ISO 15118 schema version.</param>
        public static Boolean IsNotNullOrEmpty(this ISO15118SchemaVersion? ISO15118SchemaVersion)
            => ISO15118SchemaVersion.HasValue && ISO15118SchemaVersion.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An ISO 15118 schema version.
    /// </summary>
    public readonly struct ISO15118SchemaVersion : IId,
                                                   IEquatable<ISO15118SchemaVersion>,
                                                   IComparable<ISO15118SchemaVersion>
    {

        #region Data

        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this ISO 15118 schema version is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this ISO 15118 schema version is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the ISO 15118 schema version.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ISO 15118 schema version based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an ISO 15118 schema version.</param>
        private ISO15118SchemaVersion(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an ISO 15118 schema version.
        /// </summary>
        /// <param name="Text">A text representation of an ISO 15118 schema version.</param>
        public static ISO15118SchemaVersion Parse(String Text)
        {

            if (TryParse(Text, out var iso15118SchemaVersion))
                return iso15118SchemaVersion;

            throw new ArgumentException("The given text representation of an ISO 15118 schema version is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as ISO 15118 schema version.
        /// </summary>
        /// <param name="Text">A text representation of an ISO 15118 schema version.</param>
        public static ISO15118SchemaVersion? TryParse(String Text)
        {

            if (TryParse(Text, out var iso15118SchemaVersion))
                return iso15118SchemaVersion;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ISO15118SchemaVersion)

        /// <summary>
        /// Try to parse the given text as ISO 15118 schema version.
        /// </summary>
        /// <param name="Text">A text representation of an ISO 15118 schema version.</param>
        /// <param name="ISO15118SchemaVersion">The parsed ISO 15118 schema version.</param>
        public static Boolean TryParse(String Text, out ISO15118SchemaVersion ISO15118SchemaVersion)
        {

            #region Initial checks

            Text = Text.Trim();

            if (Text.IsNullOrEmpty())
            {
                ISO15118SchemaVersion = default;
                return false;
            }

            #endregion

            try
            {
                ISO15118SchemaVersion = new ISO15118SchemaVersion(Text);
                return true;
            }
            catch (Exception)
            { }

            ISO15118SchemaVersion = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this ISO 15118 schema version.
        /// </summary>
        public ISO15118SchemaVersion Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (ISO15118SchemaVersion1, ISO15118SchemaVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ISO15118SchemaVersion1">ISO 15118 schema version.</param>
        /// <param name="ISO15118SchemaVersion2">Other ISO 15118 schema version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ISO15118SchemaVersion ISO15118SchemaVersion1,
                                           ISO15118SchemaVersion ISO15118SchemaVersion2)

            => ISO15118SchemaVersion1.Equals(ISO15118SchemaVersion2);

        #endregion

        #region Operator != (ISO15118SchemaVersion1, ISO15118SchemaVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ISO15118SchemaVersion1">ISO 15118 schema version.</param>
        /// <param name="ISO15118SchemaVersion2">Other ISO 15118 schema version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ISO15118SchemaVersion ISO15118SchemaVersion1,
                                           ISO15118SchemaVersion ISO15118SchemaVersion2)

            => !ISO15118SchemaVersion1.Equals(ISO15118SchemaVersion2);

        #endregion

        #region Operator <  (ISO15118SchemaVersion1, ISO15118SchemaVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ISO15118SchemaVersion1">ISO 15118 schema version.</param>
        /// <param name="ISO15118SchemaVersion2">Other ISO 15118 schema version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ISO15118SchemaVersion ISO15118SchemaVersion1,
                                          ISO15118SchemaVersion ISO15118SchemaVersion2)

            => ISO15118SchemaVersion1.CompareTo(ISO15118SchemaVersion2) < 0;

        #endregion

        #region Operator <= (ISO15118SchemaVersion1, ISO15118SchemaVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ISO15118SchemaVersion1">ISO 15118 schema version.</param>
        /// <param name="ISO15118SchemaVersion2">Other ISO 15118 schema version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ISO15118SchemaVersion ISO15118SchemaVersion1,
                                           ISO15118SchemaVersion ISO15118SchemaVersion2)

            => ISO15118SchemaVersion1.CompareTo(ISO15118SchemaVersion2) <= 0;

        #endregion

        #region Operator >  (ISO15118SchemaVersion1, ISO15118SchemaVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ISO15118SchemaVersion1">ISO 15118 schema version.</param>
        /// <param name="ISO15118SchemaVersion2">Other ISO 15118 schema version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ISO15118SchemaVersion ISO15118SchemaVersion1,
                                          ISO15118SchemaVersion ISO15118SchemaVersion2)

            => ISO15118SchemaVersion1.CompareTo(ISO15118SchemaVersion2) > 0;

        #endregion

        #region Operator >= (ISO15118SchemaVersion1, ISO15118SchemaVersion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ISO15118SchemaVersion1">ISO 15118 schema version.</param>
        /// <param name="ISO15118SchemaVersion2">Other ISO 15118 schema version.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ISO15118SchemaVersion ISO15118SchemaVersion1,
                                           ISO15118SchemaVersion ISO15118SchemaVersion2)

            => ISO15118SchemaVersion1.CompareTo(ISO15118SchemaVersion2) >= 0;

        #endregion

        #endregion

        #region IComparable<ISO15118SchemaVersion> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two ISO 15118 schema version sets.
        /// </summary>
        /// <param name="Object">ISO 15118 schema version to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ISO15118SchemaVersion iso15118SchemaVersion
                   ? CompareTo(iso15118SchemaVersion)
                   : throw new ArgumentException("The given object is not ISO 15118 schema version!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ISO15118SchemaVersion)

        /// <summary>
        /// Compares two ISO 15118 schema version sets.
        /// </summary>
        /// <param name="ISO15118SchemaVersion">ISO 15118 schema version to compare with.</param>
        public Int32 CompareTo(ISO15118SchemaVersion ISO15118SchemaVersion)

            => String.Compare(InternalId,
                              ISO15118SchemaVersion.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<ISO15118SchemaVersion> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ISO 15118 schema version sets for equality.
        /// </summary>
        /// <param name="Object">ISO 15118 schema version to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ISO15118SchemaVersion iso15118SchemaVersion &&
                   Equals(iso15118SchemaVersion);

        #endregion

        #region Equals(ISO15118SchemaVersion)

        /// <summary>
        /// Compares two ISO 15118 schema version sets for equality.
        /// </summary>
        /// <param name="ISO15118SchemaVersion">ISO 15118 schema version to compare with.</param>
        public Boolean Equals(ISO15118SchemaVersion ISO15118SchemaVersion)

            => String.Equals(InternalId,
                             ISO15118SchemaVersion.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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
