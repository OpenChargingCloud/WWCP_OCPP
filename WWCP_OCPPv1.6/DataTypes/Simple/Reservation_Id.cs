/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A reservation identification.
    /// </summary>
    public readonly struct Reservation_Id : IId,
                                            IEquatable<Reservation_Id>,
                                            IComparable<Reservation_Id>
    {

        #region Data

        /// <summary>
        /// The nummeric value of the reservation identification.
        /// </summary>
        public readonly UInt64 Value;

        private static readonly Random random = new Random();

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => false;

        /// <summary>
        /// The length of this identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) Value.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new reservation identification.
        /// </summary>
        /// <param name="Token">An integer.</param>
        private Reservation_Id(UInt64 Token)
        {
            this.Value = Token;
        }

        #endregion


        #region (static) Random

        /// <summary>
        /// Create a new random reservation identification.
        /// </summary>
        public static Reservation_Id Random

            => new Reservation_Id((UInt64) random.Next());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a reservation identification.
        /// </summary>
        /// <param name="Text">A text representation of a reservation identification.</param>
        public static Reservation_Id Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a reservation identification must not be null or empty!");

            #endregion

            if (TryParse(Text, out Reservation_Id reservationId))
                return reservationId;

            throw new ArgumentNullException(nameof(Text), "The given text representation of a reservation identification is invalid!");

        }

        #endregion

        #region (static) Parse   (Integer)

        /// <summary>
        /// Parse the given number as a reservation identification.
        /// </summary>
        public static Reservation_Id Parse(UInt64 Integer)
            => new Reservation_Id(Integer);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a reservation identification.
        /// </summary>
        /// <param name="Text">A text representation of a reservation identification.</param>
        public static Reservation_Id? TryParse(String Text)
        {

            if (TryParse(Text, out Reservation_Id reservationId))
                return reservationId;

            return null;

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a reservation identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a reservation identification.</param>
        public static Reservation_Id? TryParse(UInt64 Number)
        {

            if (TryParse(Number, out Reservation_Id reservationId))
                return reservationId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out ReservationId)

        /// <summary>
        /// Try to parse the given string as a reservation identification.
        /// </summary>
        /// <param name="Text">A text representation of a reservation identification.</param>
        /// <param name="ReservationId">The parsed reservation identification.</param>
        public static Boolean TryParse(String Text, out Reservation_Id ReservationId)
        {

            #region Initial checks

            Text = Text?.Trim();

            if (Text.IsNullOrEmpty())
            {
                ReservationId = default;
                return false;
            }

            #endregion

            if (UInt64.TryParse(Text, out UInt64 number))
            {
                ReservationId = new Reservation_Id(number);
                return true;
            }

            ReservationId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out ReservationId)

        /// <summary>
        /// Try to parse the given number as a reservation identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a reservation identification.</param>
        /// <param name="ReservationId">The parsed reservation identification.</param>
        public static Boolean TryParse(UInt64 Number, out Reservation_Id ReservationId)
        {

            ReservationId = new Reservation_Id(Number);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this reservation identification.
        /// </summary>
        public Reservation_Id Clone
            => new Reservation_Id(Value);

        #endregion


        #region Operator overloading

        #region Operator == (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">A reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Reservation_Id ReservationId1,
                                           Reservation_Id ReservationId2)

            => ReservationId1.Equals(ReservationId2);

        #endregion

        #region Operator != (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">A reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Reservation_Id ReservationId1,
                                           Reservation_Id ReservationId2)

            => !(ReservationId1 == ReservationId2);

        #endregion

        #region Operator <  (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">A reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Reservation_Id ReservationId1,
                                          Reservation_Id ReservationId2)

            => ReservationId1.CompareTo(ReservationId2) < 0;

        #endregion

        #region Operator <= (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">A reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Reservation_Id ReservationId1,
                                           Reservation_Id ReservationId2)

            => !(ReservationId1 > ReservationId2);

        #endregion

        #region Operator >  (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">A reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Reservation_Id ReservationId1,
                                          Reservation_Id ReservationId2)

            => ReservationId1.CompareTo(ReservationId2) > 0;

        #endregion

        #region Operator >= (ReservationId1, ReservationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId1">A reservation identification.</param>
        /// <param name="ReservationId2">Another reservation identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Reservation_Id ReservationId1,
                                           Reservation_Id ReservationId2)

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

            => Object is Reservation_Id reservationId
                   ? CompareTo(reservationId)
                   : throw new ArgumentException("The given object is not a reservation identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ReservationId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReservationId">An object to compare with.</param>
        public Int32 CompareTo(Reservation_Id ReservationId)

            => Value.CompareTo(ReservationId.Value);

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

            => Object is Reservation_Id reservationId &&
                   Equals(reservationId);

        #endregion

        #region Equals(ReservationId)

        /// <summary>
        /// Compares two reservation identifications for equality.
        /// </summary>
        /// <param name="ReservationId">A reservation identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Reservation_Id ReservationId)

            => Value.Equals(ReservationId.Value);

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
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Value.ToString();

        #endregion

    }

}
