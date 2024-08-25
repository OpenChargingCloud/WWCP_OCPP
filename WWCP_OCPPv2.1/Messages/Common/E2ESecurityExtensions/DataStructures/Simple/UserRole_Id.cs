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
    /// Extension methods for user role identifications.
    /// </summary>
    public static class UserRoleIdExtensions
    {

        /// <summary>
        /// Indicates whether this user role identification is null or empty.
        /// </summary>
        /// <param name="UserRoleId">An user role identification.</param>
        public static Boolean IsNullOrEmpty(this UserRole_Id? UserRoleId)
            => !UserRoleId.HasValue || UserRoleId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this user role identification is null or empty.
        /// </summary>
        /// <param name="UserRoleId">An user role identification.</param>
        public static Boolean IsNotNullOrEmpty(this UserRole_Id? UserRoleId)
            => UserRoleId.HasValue && UserRoleId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An user role identification.
    /// </summary>
    public readonly struct UserRole_Id : IId,
                                         IEquatable<UserRole_Id>,
                                         IComparable<UserRole_Id>
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
        /// The length of the user role identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new user role identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an user role identification.</param>
        private UserRole_Id(String Text)
        {
            this.InternalId  = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an user role identification.
        /// </summary>
        /// <param name="Text">A text representation of an user role identification.</param>
        public static UserRole_Id Parse(String Text)
        {

            if (TryParse(Text, out var userRole))
                return userRole;

            throw new ArgumentException($"Invalid text representation of an user role identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an user role identification.
        /// </summary>
        /// <param name="Text">A text representation of an user role identification.</param>
        public static UserRole_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var userRole))
                return userRole;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out UserRoleId)

        /// <summary>
        /// Try to parse the given text as an user role identification.
        /// </summary>
        /// <param name="Text">A text representation of an user role identification.</param>
        /// <param name="UserRoleId">The parsed user role identification.</param>
        public static Boolean TryParse(String Text, out UserRole_Id UserRoleId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                UserRoleId = new UserRole_Id(Text);
                return true;
            }

            UserRoleId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this user role identification.
        /// </summary>
        public UserRole_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (UserRoleId1, UserRoleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserRoleId1">An user role identification.</param>
        /// <param name="UserRoleId2">Another user role identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (UserRole_Id UserRoleId1,
                                           UserRole_Id UserRoleId2)

            => UserRoleId1.Equals(UserRoleId2);

        #endregion

        #region Operator != (UserRoleId1, UserRoleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserRoleId1">An user role identification.</param>
        /// <param name="UserRoleId2">Another user role identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (UserRole_Id UserRoleId1,
                                           UserRole_Id UserRoleId2)

            => !UserRoleId1.Equals(UserRoleId2);

        #endregion

        #region Operator <  (UserRoleId1, UserRoleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserRoleId1">An user role identification.</param>
        /// <param name="UserRoleId2">Another user role identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (UserRole_Id UserRoleId1,
                                          UserRole_Id UserRoleId2)

            => UserRoleId1.CompareTo(UserRoleId2) < 0;

        #endregion

        #region Operator <= (UserRoleId1, UserRoleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserRoleId1">An user role identification.</param>
        /// <param name="UserRoleId2">Another user role identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (UserRole_Id UserRoleId1,
                                           UserRole_Id UserRoleId2)

            => UserRoleId1.CompareTo(UserRoleId2) <= 0;

        #endregion

        #region Operator >  (UserRoleId1, UserRoleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserRoleId1">An user role identification.</param>
        /// <param name="UserRoleId2">Another user role identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (UserRole_Id UserRoleId1,
                                          UserRole_Id UserRoleId2)

            => UserRoleId1.CompareTo(UserRoleId2) > 0;

        #endregion

        #region Operator >= (UserRoleId1, UserRoleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserRoleId1">An user role identification.</param>
        /// <param name="UserRoleId2">Another user role identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (UserRole_Id UserRoleId1,
                                           UserRole_Id UserRoleId2)

            => UserRoleId1.CompareTo(UserRoleId2) >= 0;

        #endregion

        #endregion

        #region IComparable<UserRoleId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two user role identifications.
        /// </summary>
        /// <param name="Object">An user role identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is UserRole_Id userRole
                   ? CompareTo(userRole)
                   : throw new ArgumentException("The given object is not an user role identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(UserRoleId)

        /// <summary>
        /// Compares two user role identifications.
        /// </summary>
        /// <param name="UserRoleId">An user role identification to compare with.</param>
        public Int32 CompareTo(UserRole_Id UserRoleId)

            => String.Compare(InternalId,
                              UserRoleId.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<UserRoleId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two user role identifications for equality.
        /// </summary>
        /// <param name="Object">An user role identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UserRole_Id userRole &&
                   Equals(userRole);

        #endregion

        #region Equals(UserRoleId)

        /// <summary>
        /// Compares two user role identifications for equality.
        /// </summary>
        /// <param name="UserRoleId">An user role identification to compare with.</param>
        public Boolean Equals(UserRole_Id UserRoleId)

            => String.Equals(InternalId,
                             UserRoleId.InternalId,
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
