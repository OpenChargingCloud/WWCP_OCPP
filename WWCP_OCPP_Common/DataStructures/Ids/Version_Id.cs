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
 * See the License for the specific EVSE governing permissions and
 * limitations under the License.
 */

#region Usings

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// Extension methods for version identifications.
    /// </summary>
    public static class VersionIdExtensions
    {

        /// <summary>
        /// Indicates whether this version identification is null or empty.
        /// </summary>
        /// <param name="VersionId">A version identification.</param>
        public static Boolean IsNullOrEmpty(this Version_Id? VersionId)
            => !VersionId.HasValue || VersionId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this version identification is NOT null or empty.
        /// </summary>
        /// <param name="VersionId">A version identification.</param>
        public static Boolean IsNotNullOrEmpty(this Version_Id? VersionId)
            => VersionId.HasValue && VersionId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a version.
    /// </summary>
    public readonly struct Version_Id : IId<Version_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this version identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this version identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the version identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new version identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a version identification.</param>
        private Version_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a version identification.
        /// </summary>
        /// <param name="Text">A text representation of a version identification.</param>
        public static Version_Id Parse(String Text)
        {

            if (TryParse(Text, out var versionId))
                return versionId;

            throw new ArgumentException($"Invalid text representation of a version identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a version identification.
        /// </summary>
        /// <param name="Text">A text representation of a version identification.</param>
        public static Version_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var versionId))
                return versionId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out VersionId)

        /// <summary>
        /// Try to parse the given text as a version identification.
        /// </summary>
        /// <param name="Text">A text representation of a version identification.</param>
        /// <param name="VersionId">The parsed version identification.</param>
        public static Boolean TryParse(String Text, out Version_Id VersionId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    VersionId = new Version_Id(Text);
                    return true;
                }
                catch
                { }
            }

            VersionId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this version identification.
        /// </summary>
        public Version_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (VersionId1, VersionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionId1">A version identification.</param>
        /// <param name="VersionId2">Another version identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Version_Id VersionId1,
                                           Version_Id VersionId2)

            => VersionId1.Equals(VersionId2);

        #endregion

        #region Operator != (VersionId1, VersionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionId1">A version identification.</param>
        /// <param name="VersionId2">Another version identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Version_Id VersionId1,
                                           Version_Id VersionId2)

            => !VersionId1.Equals(VersionId2);

        #endregion

        #region Operator <  (VersionId1, VersionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionId1">A version identification.</param>
        /// <param name="VersionId2">Another version identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Version_Id VersionId1,
                                          Version_Id VersionId2)

            => VersionId1.CompareTo(VersionId2) < 0;

        #endregion

        #region Operator <= (VersionId1, VersionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionId1">A version identification.</param>
        /// <param name="VersionId2">Another version identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Version_Id VersionId1,
                                           Version_Id VersionId2)

            => VersionId1.CompareTo(VersionId2) <= 0;

        #endregion

        #region Operator >  (VersionId1, VersionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionId1">A version identification.</param>
        /// <param name="VersionId2">Another version identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Version_Id VersionId1,
                                          Version_Id VersionId2)

            => VersionId1.CompareTo(VersionId2) > 0;

        #endregion

        #region Operator >= (VersionId1, VersionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionId1">A version identification.</param>
        /// <param name="VersionId2">Another version identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Version_Id VersionId1,
                                           Version_Id VersionId2)

            => VersionId1.CompareTo(VersionId2) >= 0;

        #endregion

        #endregion

        #region IComparable<VersionId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two version identifications.
        /// </summary>
        /// <param name="Object">A version identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Version_Id versionId
                   ? CompareTo(versionId)
                   : throw new ArgumentException("The given object is not a version identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(VersionId)

        /// <summary>
        /// Compares two version identifications.
        /// </summary>
        /// <param name="VersionId">A version identification to compare with.</param>
        public Int32 CompareTo(Version_Id VersionId)

            => String.Compare(InternalId,
                              VersionId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<VersionId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two version identifications for equality.
        /// </summary>
        /// <param name="Object">A version identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Version_Id versionId &&
                   Equals(versionId);

        #endregion

        #region Equals(VersionId)

        /// <summary>
        /// Compares two version identifications for equality.
        /// </summary>
        /// <param name="VersionId">A version identification to compare with.</param>
        public Boolean Equals(Version_Id VersionId)

            => String.Equals(InternalId,
                             VersionId.InternalId,
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
