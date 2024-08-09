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

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// Extension methods for price level schedule identifications.
    /// </summary>
    public static class PriceLevelScheduleIdExtensions
    {

        /// <summary>
        /// Indicates whether this price level schedule identification is null or empty.
        /// </summary>
        /// <param name="PriceLevelScheduleId">A price level schedule identification.</param>
        public static Boolean IsNullOrEmpty(this PriceLevelSchedule_Id? PriceLevelScheduleId)
            => !PriceLevelScheduleId.HasValue || PriceLevelScheduleId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this price level schedule identification is null or empty.
        /// </summary>
        /// <param name="PriceLevelScheduleId">A price level schedule identification.</param>
        public static Boolean IsNotNullOrEmpty(this PriceLevelSchedule_Id? PriceLevelScheduleId)
            => PriceLevelScheduleId.HasValue && PriceLevelScheduleId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A price level schedule identification.
    /// 
    /// xs:pattern value="[\i-[:]][\c-[:]]*"/
    /// </summary>
    public readonly struct PriceLevelSchedule_Id : IId,
                                                   IEquatable<PriceLevelSchedule_Id>,
                                                   IComparable<PriceLevelSchedule_Id>
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
        /// The length of the price level schedule identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new price level schedule identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a price level schedule identification.</param>
        private PriceLevelSchedule_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a price level schedule identification.
        /// </summary>
        /// <param name="Text">A text representation of a price level schedule identification.</param>
        public static PriceLevelSchedule_Id Parse(String Text)
        {

            if (TryParse(Text, out var priceLevelScheduleId))
                return priceLevelScheduleId;

            throw new ArgumentException($"Invalid text representation of a price level schedule identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a price level schedule identification.
        /// </summary>
        /// <param name="Text">A text representation of a price level schedule identification.</param>
        public static PriceLevelSchedule_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var priceLevelScheduleId))
                return priceLevelScheduleId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out PriceLevelScheduleId)

        /// <summary>
        /// Try to parse the given text as a price level schedule identification.
        /// </summary>
        /// <param name="Text">A text representation of a price level schedule identification.</param>
        /// <param name="PriceLevelScheduleId">The parsed price level schedule identification.</param>
        public static Boolean TryParse(String Text, out PriceLevelSchedule_Id PriceLevelScheduleId)
        {

            Text = Text.Trim();

            //ToDo: xs:hexBinary, length: 8

            if (Text.IsNotNullOrEmpty())
            {
                PriceLevelScheduleId = new PriceLevelSchedule_Id(Text);
                return true;
            }

            PriceLevelScheduleId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this price level schedule identification.
        /// </summary>
        public PriceLevelSchedule_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (PriceLevelScheduleId1, PriceLevelScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceLevelScheduleId1">A price level schedule identification.</param>
        /// <param name="PriceLevelScheduleId2">Another price level schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PriceLevelSchedule_Id PriceLevelScheduleId1,
                                           PriceLevelSchedule_Id PriceLevelScheduleId2)

            => PriceLevelScheduleId1.Equals(PriceLevelScheduleId2);

        #endregion

        #region Operator != (PriceLevelScheduleId1, PriceLevelScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceLevelScheduleId1">A price level schedule identification.</param>
        /// <param name="PriceLevelScheduleId2">Another price level schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PriceLevelSchedule_Id PriceLevelScheduleId1,
                                           PriceLevelSchedule_Id PriceLevelScheduleId2)

            => !PriceLevelScheduleId1.Equals(PriceLevelScheduleId2);

        #endregion

        #region Operator <  (PriceLevelScheduleId1, PriceLevelScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceLevelScheduleId1">A price level schedule identification.</param>
        /// <param name="PriceLevelScheduleId2">Another price level schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PriceLevelSchedule_Id PriceLevelScheduleId1,
                                          PriceLevelSchedule_Id PriceLevelScheduleId2)

            => PriceLevelScheduleId1.CompareTo(PriceLevelScheduleId2) < 0;

        #endregion

        #region Operator <= (PriceLevelScheduleId1, PriceLevelScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceLevelScheduleId1">A price level schedule identification.</param>
        /// <param name="PriceLevelScheduleId2">Another price level schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PriceLevelSchedule_Id PriceLevelScheduleId1,
                                           PriceLevelSchedule_Id PriceLevelScheduleId2)

            => PriceLevelScheduleId1.CompareTo(PriceLevelScheduleId2) <= 0;

        #endregion

        #region Operator >  (PriceLevelScheduleId1, PriceLevelScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceLevelScheduleId1">A price level schedule identification.</param>
        /// <param name="PriceLevelScheduleId2">Another price level schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PriceLevelSchedule_Id PriceLevelScheduleId1,
                                          PriceLevelSchedule_Id PriceLevelScheduleId2)

            => PriceLevelScheduleId1.CompareTo(PriceLevelScheduleId2) > 0;

        #endregion

        #region Operator >= (PriceLevelScheduleId1, PriceLevelScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PriceLevelScheduleId1">A price level schedule identification.</param>
        /// <param name="PriceLevelScheduleId2">Another price level schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PriceLevelSchedule_Id PriceLevelScheduleId1,
                                           PriceLevelSchedule_Id PriceLevelScheduleId2)

            => PriceLevelScheduleId1.CompareTo(PriceLevelScheduleId2) >= 0;

        #endregion

        #endregion

        #region IComparable<PriceLevelScheduleId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two price level schedule identifications.
        /// </summary>
        /// <param name="Object">A price level schedule identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PriceLevelSchedule_Id priceLevelScheduleId
                   ? CompareTo(priceLevelScheduleId)
                   : throw new ArgumentException("The given object is not a price level schedule identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PriceLevelScheduleId)

        /// <summary>
        /// Compares two price level schedule identifications.
        /// </summary>
        /// <param name="PriceLevelScheduleId">A price level schedule identification to compare with.</param>
        public Int32 CompareTo(PriceLevelSchedule_Id PriceLevelScheduleId)

            => String.Compare(InternalId,
                              PriceLevelScheduleId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<PriceLevelScheduleId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two price level schedule identifications for equality.
        /// </summary>
        /// <param name="Object">A price level schedule identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PriceLevelSchedule_Id priceLevelScheduleId &&
                   Equals(priceLevelScheduleId);

        #endregion

        #region Equals(PriceLevelScheduleId)

        /// <summary>
        /// Compares two price level schedule identifications for equality.
        /// </summary>
        /// <param name="PriceLevelScheduleId">A price level schedule identification to compare with.</param>
        public Boolean Equals(PriceLevelSchedule_Id PriceLevelScheduleId)

            => String.Equals(InternalId,
                             PriceLevelScheduleId.InternalId,
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
