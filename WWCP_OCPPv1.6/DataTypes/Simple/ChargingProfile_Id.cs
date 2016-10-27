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

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// An OCPP charging profile identification.
    /// </summary>
    public class ChargingProfile_Id
    {

        #region Properties

        /// <summary>
        /// The value of the charging profile identification.
        /// </summary>
        public UInt64 Value { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new OCPP charging profile identification.
        /// </summary>
        /// <param name="Token">An integer.</param>
        private ChargingProfile_Id(UInt64 Token)
        {
            this.Value = Token;
        }

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given string as an OCPP charging profile identification.
        /// </summary>
        public static ChargingProfile_Id Parse(String Text)
            => new ChargingProfile_Id(UInt64.Parse(Text));

        #endregion

        #region Parse(Integer)

        /// <summary>
        /// Parse the given integer as an OCPP charging profile identification.
        /// </summary>
        public static ChargingProfile_Id Parse(UInt64 Integer)
            => new ChargingProfile_Id(Integer);

        #endregion

        #region TryParse(Text,    out ReservationId)

        /// <summary>
        /// Parse the given string as an OCPP charging profile identification.
        /// </summary>
        public static Boolean TryParse(String Text, out ChargingProfile_Id ReservationId)
        {

            UInt64 _Integer;

            if (UInt64.TryParse(Text, out _Integer))
            {
                ReservationId = new ChargingProfile_Id(_Integer);
                return true;
            }

            ReservationId = null;
            return false;

        }

        #endregion

        #region TryParse(Integer, out ReservationId)

        /// <summary>
        /// Parse the given integer as an OCPP charging profile identification.
        /// </summary>
        public static Boolean TryParse(UInt64 Integer, out ChargingProfile_Id ReservationId)
        {

            ReservationId = new ChargingProfile_Id(Integer);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charging profile identification.
        /// </summary>
        public ChargingProfile_Id Clone
            => new ChargingProfile_Id(Value);

        #endregion


        #region Operator overloading

        #region Operator == (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">An charging profile identification.</param>
        /// <param name="ReservationId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingProfile_Id ReservationId1, ChargingProfile_Id ReservationId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ReservationId1, ReservationId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ReservationId1 == null) || ((Object) ReservationId2 == null))
                return false;

            if ((Object) ReservationId1 == null)
                throw new ArgumentNullException(nameof(ReservationId1),  "The given charging profile identification must not be null!");

            return ReservationId1.Equals(ReservationId2);

        }

        #endregion

        #region Operator != (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">An charging profile identification.</param>
        /// <param name="ReservationId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingProfile_Id ReservationId1, ChargingProfile_Id ReservationId2)
            => !(ReservationId1 == ReservationId2);

        #endregion

        #region Operator <  (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">An charging profile identification.</param>
        /// <param name="ReservationId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingProfile_Id ReservationId1, ChargingProfile_Id ReservationId2)
        {

            if ((Object) ReservationId1 == null)
                throw new ArgumentNullException(nameof(ReservationId1),  "The given charging profile identification must not be null!");

            return ReservationId1.CompareTo(ReservationId2) < 0;

        }

        #endregion

        #region Operator <= (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">An charging profile identification.</param>
        /// <param name="ReservationId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingProfile_Id ReservationId1, ChargingProfile_Id ReservationId2)
            => !(ReservationId1 > ReservationId2);

        #endregion

        #region Operator >  (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">An charging profile identification.</param>
        /// <param name="ReservationId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingProfile_Id ReservationId1, ChargingProfile_Id ReservationId2)
        {

            if ((Object) ReservationId1 == null)
                throw new ArgumentNullException(nameof(ReservationId1),  "The given charging profile identification must not be null!");

            return ReservationId1.CompareTo(ReservationId2) > 0;

        }

        #endregion

        #region Operator >= (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">An charging profile identification.</param>
        /// <param name="ReservationId2">Another charging profile identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingProfile_Id ReservationId1, ChargingProfile_Id ReservationId2)
            => !(ReservationId1 < ReservationId2);

        #endregion

        #endregion

        #region IComparable<ReservationId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object),  "The given object must not be null!");

            // Check if the given object is a charging profile identification.
            var ChargingProfile_Id = Object as ChargingProfile_Id;
            if ((Object) ChargingProfile_Id == null)
                throw new ArgumentException("The given object is not a charging profile identification!", nameof(Object));

            return CompareTo(ChargingProfile_Id);

        }

        #endregion

        #region CompareTo(ChargingProfile_Id)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId">An object to compare with.</param>
        public Int32 CompareTo(ChargingProfile_Id ReservationId)
        {

            if ((Object) ReservationId == null)
                throw new ArgumentNullException(nameof(ReservationId),  "The given charging profile identification must not be null!");

            return Value.CompareTo(ReservationId.Value);

        }

        #endregion

        #endregion

        #region IEquatable<ReservationId> Members

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

            // Check if the given object is a charging profile identification.
            var ChargingProfile_Id = Object as ChargingProfile_Id;
            if ((Object) ChargingProfile_Id == null)
                return false;

            return this.Equals(ChargingProfile_Id);

        }

        #endregion

        #region Equals(ReservationId)

        /// <summary>
        /// Compares two charging profile identifications for equality.
        /// </summary>
        /// <param name="ReservationId">An charging profile identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ChargingProfile_Id ReservationId)
        {

            if ((Object) ReservationId == null)
                return false;

            return Value.Equals(ReservationId.Value);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
            => Value.ToString();

        #endregion


    }

}
