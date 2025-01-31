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
    /// Extension methods for access rights.
    /// </summary>
    public static class AccessRightExtensions
    {

        /// <summary>
        /// Indicates whether this access right is null or empty.
        /// </summary>
        /// <param name="AccessRight">An access right.</param>
        public static Boolean IsNullOrEmpty(this AccessRight? AccessRight)
            => !AccessRight.HasValue || AccessRight.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this access right is null or empty.
        /// </summary>
        /// <param name="AccessRight">An access right.</param>
        public static Boolean IsNotNullOrEmpty(this AccessRight? AccessRight)
            => AccessRight.HasValue && AccessRight.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An access right.
    /// </summary>
    public readonly struct AccessRight : IId,
                                         IEquatable<AccessRight>,
                                         IComparable<AccessRight>
    {

        #region Data

        private readonly static Dictionary<String, AccessRight>  lookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly        String                                    InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this access right is null or empty.
        /// </summary>
        public readonly Boolean  IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this access right is NOT null or empty.
        /// </summary>
        public readonly Boolean  IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the access right.
        /// </summary>
        public readonly UInt64   Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new access right based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an access right.</param>
        private AccessRight(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static AccessRight Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new AccessRight(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an access right.
        /// </summary>
        /// <param name="Text">A text representation of an access right.</param>
        public static AccessRight Parse(String Text)
        {

            if (TryParse(Text, out var secureDataTransferStatus))
                return secureDataTransferStatus;

            throw new ArgumentException($"Invalid text representation of an access right: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as access right.
        /// </summary>
        /// <param name="Text">A text representation of an access right.</param>
        public static AccessRight? TryParse(String Text)
        {

            if (TryParse(Text, out var secureDataTransferStatus))
                return secureDataTransferStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out AccessRight)

        /// <summary>
        /// Try to parse the given text as access right.
        /// </summary>
        /// <param name="Text">A text representation of an access right.</param>
        /// <param name="AccessRight">The parsed access right.</param>
        public static Boolean TryParse(String Text, out AccessRight AccessRight)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out AccessRight))
                    AccessRight = Register(Text);

                return true;

            }

            AccessRight = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this access right.
        /// </summary>
        public AccessRight Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// ReadOnly
        /// </summary>
        public static AccessRight  ReadOnly      { get; }
            = Register("ReadOnly");

        /// <summary>
        /// ReadWrite
        /// </summary>
        public static AccessRight  ReadWrite     { get; }
            = Register("ReadWrite");

        /// <summary>
        /// AppendOnly
        /// </summary>
        public static AccessRight  AppendOnly    { get; }
            = Register("AppendOnly");

        #endregion


        #region Operator overloading

        #region Operator == (AccessRight1, AccessRight2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessRight1">An access right.</param>
        /// <param name="AccessRight2">Another access right.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AccessRight AccessRight1,
                                           AccessRight AccessRight2)

            => AccessRight1.Equals(AccessRight2);

        #endregion

        #region Operator != (AccessRight1, AccessRight2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessRight1">An access right.</param>
        /// <param name="AccessRight2">Another access right.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AccessRight AccessRight1,
                                           AccessRight AccessRight2)

            => !AccessRight1.Equals(AccessRight2);

        #endregion

        #region Operator <  (AccessRight1, AccessRight2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessRight1">An access right.</param>
        /// <param name="AccessRight2">Another access right.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AccessRight AccessRight1,
                                          AccessRight AccessRight2)

            => AccessRight1.CompareTo(AccessRight2) < 0;

        #endregion

        #region Operator <= (AccessRight1, AccessRight2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessRight1">An access right.</param>
        /// <param name="AccessRight2">Another access right.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AccessRight AccessRight1,
                                           AccessRight AccessRight2)

            => AccessRight1.CompareTo(AccessRight2) <= 0;

        #endregion

        #region Operator >  (AccessRight1, AccessRight2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessRight1">An access right.</param>
        /// <param name="AccessRight2">Another access right.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AccessRight AccessRight1,
                                          AccessRight AccessRight2)

            => AccessRight1.CompareTo(AccessRight2) > 0;

        #endregion

        #region Operator >= (AccessRight1, AccessRight2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AccessRight1">An access right.</param>
        /// <param name="AccessRight2">Another access right.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AccessRight AccessRight1,
                                           AccessRight AccessRight2)

            => AccessRight1.CompareTo(AccessRight2) >= 0;

        #endregion

        #endregion

        #region IComparable<AccessRight> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two access rights.
        /// </summary>
        /// <param name="Object">An access right to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is AccessRight secureDataTransferStatus
                   ? CompareTo(secureDataTransferStatus)
                   : throw new ArgumentException("The given object is not access right!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AccessRight)

        /// <summary>
        /// Compares two access rights.
        /// </summary>
        /// <param name="AccessRight">An access right to compare with.</param>
        public Int32 CompareTo(AccessRight AccessRight)

            => String.Compare(InternalId,
                              AccessRight.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<AccessRight> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two access rights for equality.
        /// </summary>
        /// <param name="Object">An access right to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AccessRight secureDataTransferStatus &&
                   Equals(secureDataTransferStatus);

        #endregion

        #region Equals(AccessRight)

        /// <summary>
        /// Compares two access rights for equality.
        /// </summary>
        /// <param name="AccessRight">An access right to compare with.</param>
        public Boolean Equals(AccessRight AccessRight)

            => String.Equals(InternalId,
                             AccessRight.InternalId,
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
