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

using cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonTypes;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// The abstract price schedule.
    /// </summary>
    public abstract class APriceSchedule : IEquatable<APriceSchedule>
    {

        #region Properties

        /// <summary>
        /// The unique identification of the price schedule.
        /// </summary>
        [Mandatory]
        public PriceSchedule_Id  Id             { get; }

        /// <summary>
        /// The time anchor of the price schedule.
        /// </summary>
        [Mandatory]
        public DateTime          TimeAnchor     { get; }

        /// <summary>
        /// The description of the price schedule.
        /// </summary>
        [Optional]
        public String?           Description    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new price schedule.
        /// </summary>
        /// <param name="Id">An unique identification of the price schedule.</param>
        /// <param name="TimeAnchor">A time anchor of the price schedule.</param>
        /// <param name="Description">An optional description of the price schedule.</param>
        public APriceSchedule(PriceSchedule_Id  Id,
                              DateTime          TimeAnchor,
                              String?           Description   = null)
        {

            this.Id           = Id;
            this.TimeAnchor   = TimeAnchor;
            this.Description  = Description;

            unchecked
            {

                hashCode = Id.          GetHashCode()       * 7 ^
                           TimeAnchor.  GetHashCode()       * 5 ^
                          (Description?.GetHashCode() ?? 0) * 3 ^
                           base.        GetHashCode();

            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (PriceSchedule1, PriceSchedule2)

        /// <summary>
        /// Compares two price schedules for equality.
        /// </summary>
        /// <param name="PriceSchedule1">A price schedule.</param>
        /// <param name="PriceSchedule2">Another price schedule.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (APriceSchedule? PriceSchedule1,
                                           APriceSchedule? PriceSchedule2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(PriceSchedule1, PriceSchedule2))
                return true;

            // If one is null, but not both, return false.
            if (PriceSchedule1 is null || PriceSchedule2 is null)
                return false;

            return PriceSchedule1.Equals(PriceSchedule2);

        }

        #endregion

        #region Operator != (PriceSchedule1, PriceSchedule2)

        /// <summary>
        /// Compares two price schedules for inequality.
        /// </summary>
        /// <param name="PriceSchedule1">A price schedule.</param>
        /// <param name="PriceSchedule2">Another price schedule.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (APriceSchedule? PriceSchedule1,
                                           APriceSchedule? PriceSchedule2)

            => !(PriceSchedule1 == PriceSchedule2);

        #endregion

        #endregion

        #region IEquatable<PriceSchedule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two price schedules for equality.
        /// </summary>
        /// <param name="Object">A price schedule to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is APriceSchedule priceSchedule &&
                   Equals(priceSchedule);

        #endregion

        #region Equals(PriceSchedule)

        /// <summary>
        /// Compares two price schedules for equality.
        /// </summary>
        /// <param name="PriceSchedule">A price schedule to compare with.</param>
        public Boolean Equals(APriceSchedule? PriceSchedule)

            => PriceSchedule is not null &&

               TimeAnchor.     Equals(PriceSchedule.TimeAnchor)      &&
               Id.Equals(PriceSchedule.Id) &&

               String.Equals(Description, PriceSchedule.Description);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   $"{Id} ({TimeAnchor})",

                   Description.IsNotNullOrEmpty()
                       ? $", '{Description}'"
                       : ""

               );

        #endregion

    }

}
