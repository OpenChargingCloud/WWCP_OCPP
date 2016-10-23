/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
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

using System;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// An OCPP identification tag info.
    /// </summary>
    public class IdTagInfo
    {

        #region Properties

        /// <summary>
        /// The authentication result.
        /// </summary>
        public AuthorizationStatus  Status        { get; }

        /// <summary>
        /// An optional date at which the idTag should be removed
        /// from the authorization cache.
        /// </summary>
        public DateTime?            ExpiryDate    { get; }

        /// <summary>
        /// An optional the parent-identifier.
        /// </summary>
        public IdToken              ParentIdTag   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new OCPP identification tag info.
        /// </summary>
        /// <param name="Status">The authentication result.</param>
        /// <param name="ExpiryDate">An optional date at which the idTag should be removed from the authorization cache.</param>
        /// <param name="ParentIdTag">An optional the parent-identifier.</param>
        public IdTagInfo(AuthorizationStatus  Status,
                         DateTime?            ExpiryDate   = null,
                         IdToken              ParentIdTag  = null)
        {

            this.Status       = Status;
            this.ExpiryDate   = ExpiryDate ?? new DateTime?();
            this.ParentIdTag  = ParentIdTag;

        }

        #endregion


        #region Operator overloading

        #region Operator == (IdTagInfo1, IdTagInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTagInfo1">An id tag info.</param>
        /// <param name="IdTagInfo2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (IdTagInfo IdTagInfo1, IdTagInfo IdTagInfo2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(IdTagInfo1, IdTagInfo2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) IdTagInfo1 == null) || ((Object) IdTagInfo2 == null))
                return false;

            if ((Object) IdTagInfo1 == null)
                throw new ArgumentNullException(nameof(IdTagInfo1),  "The given id tag info must not be null!");

            return IdTagInfo1.Equals(IdTagInfo2);

        }

        #endregion

        #region Operator != (IdTagInfo1, IdTagInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTagInfo1">An id tag info.</param>
        /// <param name="IdTagInfo2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (IdTagInfo IdTagInfo1, IdTagInfo IdTagInfo2)
            => !(IdTagInfo1 == IdTagInfo2);

        #endregion

        #endregion

        #region IEquatable<IdTagInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            // Check if the given object is a id tag info.
            var IdTagInfo = Object as IdTagInfo;
            if ((Object) IdTagInfo == null)
                return false;

            return this.Equals(IdTagInfo);

        }

        #endregion

        #region Equals(IdTagInfo)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="IdTagInfo">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(IdTagInfo IdTagInfo)
        {

            if ((Object) IdTagInfo == null)
                return false;

            return Status.Equals(IdTagInfo.Status) &&

                   ((!ExpiryDate.HasValue && !IdTagInfo.ExpiryDate.HasValue) ||
                     (ExpiryDate.HasValue && IdTagInfo.ExpiryDate.HasValue && ExpiryDate.Value.Equals(IdTagInfo.ExpiryDate.Value))) &&

                   ((ParentIdTag == null && IdTagInfo.ParentIdTag == null) ||
                    (ParentIdTag != null && IdTagInfo.ParentIdTag != null && ParentIdTag.Equals(IdTagInfo.ParentIdTag)));

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return (ParentIdTag != null
                            ? ParentIdTag.GetHashCode() * 17
                            : 0) ^

                       (ExpiryDate.HasValue
                            ? ExpiryDate.GetHashCode()  * 11
                            : 0) ^

                       Status.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Status,

                             ParentIdTag != null
                                 ? " (" + ParentIdTag.Value + ")"
                                 : "",

                             ExpiryDate.HasValue
                                 ? " valid till " + ExpiryDate.Value.ToIso8601()
                                 : "");

        #endregion


    }

}
