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
    /// An OCPP reservation identification.
    /// </summary>
    public struct Reservation_Id : IId,
                                   IEquatable<Reservation_Id>,
                                   IComparable<Reservation_Id>
    {

        #region Data

        private readonly UInt64 _Value;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new OCPP reservation identification.
        /// </summary>
        /// <param name="Token">An integer.</param>
        private Reservation_Id(UInt64 Token)
        {
            this._Value = Token;
        }

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given string as an OCPP reservation identification.
        /// </summary>
        public static Reservation_Id Parse(String Text)
            => new Reservation_Id(UInt64.Parse(Text));

        #endregion

        #region Parse(Integer)

        /// <summary>
        /// Parse the given integer as an OCPP reservation identification.
        /// </summary>
        public static Reservation_Id Parse(UInt64 Integer)
            => new Reservation_Id(Integer);

        #endregion

        #region TryParse(Text,    out ReservationId)

        /// <summary>
        /// Parse the given string as an OCPP reservation identification.
        /// </summary>
        public static Boolean TryParse(String Text, out Reservation_Id ReservationId)
        {

            UInt64 _Integer;

            if (UInt64.TryParse(Text, out _Integer))
            {
                ReservationId = new Reservation_Id(_Integer);
                return true;
            }

            ReservationId = default(Reservation_Id);
            return false;

        }

        #endregion

        #region TryParse(Integer, out ReservationId)

        /// <summary>
        /// Parse the given integer as an OCPP reservation identification.
        /// </summary>
        public static Boolean TryParse(UInt64 Integer, out Reservation_Id ReservationId)
        {

            ReservationId = new Reservation_Id(Integer);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this reservation identification.
        /// </summary>
        public Reservation_Id Clone
            => new Reservation_Id(_Value);

        #endregion


        #region Operator overloading

        #region Operator == (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">An reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Reservation_Id ReservationId1, Reservation_Id ReservationId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ReservationId1, ReservationId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ReservationId1 == null) || ((Object) ReservationId2 == null))
                return false;

            if ((Object) ReservationId1 == null)
                throw new ArgumentNullException(nameof(ReservationId1),  "The given reservation identification must not be null!");

            return ReservationId1.Equals(ReservationId2);

        }

        #endregion

        #region Operator != (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">An reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Reservation_Id ReservationId1, Reservation_Id ReservationId2)
            => !(ReservationId1 == ReservationId2);

        #endregion

        #region Operator <  (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">An reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Reservation_Id ReservationId1, Reservation_Id ReservationId2)
        {

            if ((Object) ReservationId1 == null)
                throw new ArgumentNullException(nameof(ReservationId1),  "The given reservation identification must not be null!");

            return ReservationId1.CompareTo(ReservationId2) < 0;

        }

        #endregion

        #region Operator <= (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">An reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Reservation_Id ReservationId1, Reservation_Id ReservationId2)
            => !(ReservationId1 > ReservationId2);

        #endregion

        #region Operator >  (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">An reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Reservation_Id ReservationId1, Reservation_Id ReservationId2)
        {

            if ((Object) ReservationId1 == null)
                throw new ArgumentNullException(nameof(ReservationId1),  "The given reservation identification must not be null!");

            return ReservationId1.CompareTo(ReservationId2) > 0;

        }

        #endregion

        #region Operator >= (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">An reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Reservation_Id ReservationId1, Reservation_Id ReservationId2)
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

            // Check if the given object is a reservation identification.
            if (!(Object is Reservation_Id))
                throw new ArgumentException("The given object is not a reservation identification!", nameof(Object));

            return CompareTo((Reservation_Id) Object);

        }

        #endregion

        #region CompareTo(ReservationId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId">An object to compare with.</param>
        public Int32 CompareTo(Reservation_Id ReservationId)
        {

            if ((Object) ReservationId == null)
                throw new ArgumentNullException(nameof(ReservationId),  "The given reservation identification must not be null!");

            return _Value.CompareTo(ReservationId._Value);

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

            // Check if the given object is a reservation identification.
            if (!(Object is Reservation_Id))
                return false;

            return this.Equals((Reservation_Id) Object);

        }

        #endregion

        #region Equals(ReservationId)

        /// <summary>
        /// Compares two reservation identifications for equality.
        /// </summary>
        /// <param name="ReservationId">An reservation identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Reservation_Id ReservationId)
        {

            if ((Object) ReservationId == null)
                return false;

            return _Value.Equals(ReservationId._Value);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => _Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
            => _Value.ToString();

        #endregion


    }

}
