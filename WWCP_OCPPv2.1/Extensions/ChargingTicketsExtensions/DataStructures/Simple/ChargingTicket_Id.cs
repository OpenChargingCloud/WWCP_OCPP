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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for charging ticket identifications.
    /// </summary>
    public static class ChargingTicketIdExtensions
    {

        /// <summary>
        /// Indicates whether this charging ticket identification is null or empty.
        /// </summary>
        /// <param name="TicketId">A charging ticket identification.</param>
        public static Boolean IsNullOrEmpty(this ChargingTicket_Id? ChargingTicketId)
            => !ChargingTicketId.HasValue || ChargingTicketId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this charging ticket identification is NOT null or empty.
        /// </summary>
        /// <param name="TicketId">A charging ticket identification.</param>
        public static Boolean IsNotNullOrEmpty(this ChargingTicket_Id? ChargingTicketId)
            => ChargingTicketId.HasValue && ChargingTicketId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a charging ticket.
    /// </summary>
    public readonly struct ChargingTicket_Id : IId<ChargingTicket_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this charging ticket identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this charging ticket identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the charging ticket identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging ticket identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a charging ticket identification.</param>
        private ChargingTicket_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom(EMPId, Length = 20)

        /// <summary>
        /// Create a new random charging ticket identification based
        /// on an e-mobility provider Identification, the current timestamp
        /// and a random number.
        /// </summary>
        /// <param name="Length">The expected length of the charging ticket identification.</param>
        public static ChargingTicket_Id NewRandom(String   EMPId,
                                                  Byte     Length         = 30,
                                                  String?  RandomSuffix   = null)

            => new ($"{EMPId}-{Timestamp.Now.ToUnixTimestamp()}-{RandomSuffix ?? RandomExtensions.RandomString(Length)}");

        #endregion

        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as a charging ticket identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket identification.</param>
        public static ChargingTicket_Id Parse(String Text)
        {

            if (TryParse(Text, out var ticketId))
                return ticketId;

            throw new ArgumentException($"Invalid text representation of a charging ticket identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a charging ticket identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket identification.</param>
        public static ChargingTicket_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var ticketId))
                return ticketId;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out ChargingTicketId)

        /// <summary>
        /// Try to parse the given text as a charging ticket identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging ticket identification.</param>
        /// <param name="TicketId">The parsed charging ticket identification.</param>
        public static Boolean TryParse(String Text, out ChargingTicket_Id ChargingTicketId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ChargingTicketId = new ChargingTicket_Id(Text);
                    return true;
                }
                catch
                { }
            }

            ChargingTicketId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charging ticket identification.
        /// </summary>
        public ChargingTicket_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (ChargingTicketId1, ChargingTicketId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TicketId1">A charging ticket identification.</param>
        /// <param name="TicketId2">Another charging ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingTicket_Id ChargingTicketId1,
                                           ChargingTicket_Id ChargingTicketId2)

            => ChargingTicketId1.Equals(ChargingTicketId2);

        #endregion

        #region Operator != (ChargingTicketId1, ChargingTicketId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TicketId1">A charging ticket identification.</param>
        /// <param name="TicketId2">Another charging ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingTicket_Id ChargingTicketId1,
                                           ChargingTicket_Id ChargingTicketId2)

            => !ChargingTicketId1.Equals(ChargingTicketId2);

        #endregion

        #region Operator <  (ChargingTicketId1, ChargingTicketId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TicketId1">A charging ticket identification.</param>
        /// <param name="TicketId2">Another charging ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingTicket_Id ChargingTicketId1,
                                          ChargingTicket_Id ChargingTicketId2)

            => ChargingTicketId1.CompareTo(ChargingTicketId2) < 0;

        #endregion

        #region Operator <= (ChargingTicketId1, ChargingTicketId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TicketId1">A charging ticket identification.</param>
        /// <param name="TicketId2">Another charging ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingTicket_Id ChargingTicketId1,
                                           ChargingTicket_Id ChargingTicketId2)

            => ChargingTicketId1.CompareTo(ChargingTicketId2) <= 0;

        #endregion

        #region Operator >  (ChargingTicketId1, ChargingTicketId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TicketId1">A charging ticket identification.</param>
        /// <param name="TicketId2">Another charging ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingTicket_Id ChargingTicketId1,
                                          ChargingTicket_Id ChargingTicketId2)

            => ChargingTicketId1.CompareTo(ChargingTicketId2) > 0;

        #endregion

        #region Operator >= (ChargingTicketId1, ChargingTicketId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TicketId1">A charging ticket identification.</param>
        /// <param name="TicketId2">Another charging ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingTicket_Id ChargingTicketId1,
                                           ChargingTicket_Id ChargingTicketId2)

            => ChargingTicketId1.CompareTo(ChargingTicketId2) >= 0;

        #endregion

        #endregion

        #region IComparable<TicketId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging ticket identifications.
        /// </summary>
        /// <param name="Object">A charging ticket identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ChargingTicket_Id ticketId
                   ? CompareTo(ticketId)
                   : throw new ArgumentException("The given object is not a charging ticket identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingTicketId)

        /// <summary>
        /// Compares two charging ticket identifications.
        /// </summary>
        /// <param name="TicketId">A charging ticket identification to compare with.</param>
        public Int32 CompareTo(ChargingTicket_Id ChargingTicketId)

            => String.Compare(InternalId,
                              ChargingTicketId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TicketId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging ticket identifications for equality.
        /// </summary>
        /// <param name="Object">A charging ticket identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingTicket_Id ticketId &&
                   Equals(ticketId);

        #endregion

        #region Equals(ChargingTicketId)

        /// <summary>
        /// Compares two charging ticket identifications for equality.
        /// </summary>
        /// <param name="TicketId">A charging ticket identification to compare with.</param>
        public Boolean Equals(ChargingTicket_Id ChargingTicketId)

            => String.Equals(InternalId,
                             ChargingTicketId.InternalId,
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
