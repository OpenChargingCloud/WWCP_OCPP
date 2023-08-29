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

using cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonTypes;
using System;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// The abstract price schedule.
    /// </summary>
    public abstract class PriceSchedule : IEquatable<PriceSchedule>
    {

        #region Properties

        /// <summary>
        /// The time anchor of the price schedule.
        /// </summary>
        [Mandatory]
        public DateTime          TimeAnchor         { get; }

        /// <summary>
        /// The unique identification of the price schedule.
        /// </summary>
        [Mandatory]
        public PriceSchedule_Id  PriceScheduleId    { get; }

        /// <summary>
        /// The description of the price schedule.
        /// </summary>
        [Optional]
        public Description?      Description        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new price schedule.
        /// </summary>
        /// <param name="TimeAnchor">A time anchor of the price schedule.</param>
        /// <param name="PriceScheduleId">An unique identification of the price schedule.</param>
        /// <param name="Description">An optional description of the price schedule.</param>
        public PriceSchedule(DateTime          TimeAnchor,
                             PriceSchedule_Id  PriceScheduleId,
                             Description?      Description   = null)
        {

            this.TimeAnchor       = TimeAnchor;
            this.PriceScheduleId  = PriceScheduleId;
            this.Description      = Description;

            unchecked
            {

                hashCode = TimeAnchor.     GetHashCode()       * 7 ^
                           PriceScheduleId.GetHashCode()       * 5 ^
                          (Description?.   GetHashCode() ?? 0) * 3 ^

                           base.GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion


        #region Operator overloading

        #region Operator == (PriceSchedule1, PriceSchedule2)

        /// <summary>
        /// Compares two price schedules for equality.
        /// </summary>
        /// <param name="PriceSchedule1">A price schedule.</param>
        /// <param name="PriceSchedule2">Another price schedule.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (PriceSchedule? PriceSchedule1,
                                           PriceSchedule? PriceSchedule2)
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
        public static Boolean operator != (PriceSchedule? PriceSchedule1,
                                           PriceSchedule? PriceSchedule2)

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

            => Object is PriceSchedule priceSchedule &&
                   Equals(priceSchedule);

        #endregion

        #region Equals(PriceSchedule)

        /// <summary>
        /// Compares two price schedules for equality.
        /// </summary>
        /// <param name="PriceSchedule">A price schedule to compare with.</param>
        public Boolean Equals(PriceSchedule? PriceSchedule)

            => PriceSchedule is not null &&

               TimeAnchor.     Equals(PriceSchedule.TimeAnchor)      &&
               PriceScheduleId.Equals(PriceSchedule.PriceScheduleId) &&

            ((!Description.HasValue && !PriceSchedule.Description.HasValue) ||
              (Description.HasValue &&  PriceSchedule.Description.HasValue && Description.Value.Equals(PriceSchedule.Description.Value)));

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

            => $"{PriceScheduleId} ({TimeAnchor}){(Description.HasValue
                                                       ? ", description: " + Description.Value
                                                       : "")}";

        #endregion

    }

}
