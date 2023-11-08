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

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// Extension methods for absolute price schedule identifications.
    /// </summary>
    public static class AbsolutePriceScheduleIdExtensions
    {

        /// <summary>
        /// Indicates whether this absolute price schedule identification is null or empty.
        /// </summary>
        /// <param name="AbsolutePriceScheduleId">An absolute price schedule identification.</param>
        public static Boolean IsNullOrEmpty(this AbsolutePriceSchedule_Id? AbsolutePriceScheduleId)
            => !AbsolutePriceScheduleId.HasValue || AbsolutePriceScheduleId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this absolute price schedule identification is null or empty.
        /// </summary>
        /// <param name="AbsolutePriceScheduleId">An absolute price schedule identification.</param>
        public static Boolean IsNotNullOrEmpty(this AbsolutePriceSchedule_Id? AbsolutePriceScheduleId)
            => AbsolutePriceScheduleId.HasValue && AbsolutePriceScheduleId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An absolute price schedule identification.
    /// 
    /// xs:pattern value="[\i-[:]][\c-[:]]*"/
    /// </summary>
    public readonly struct AbsolutePriceSchedule_Id : IId,
                                               IEquatable<AbsolutePriceSchedule_Id>,
                                               IComparable<AbsolutePriceSchedule_Id>
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
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the absolute price schedule identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new absolute price schedule identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an absolute price schedule identification.</param>
        private AbsolutePriceSchedule_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an absolute price schedule identification.
        /// </summary>
        /// <param name="Text">A text representation of an absolute price schedule identification.</param>
        public static AbsolutePriceSchedule_Id Parse(String Text)
        {

            if (TryParse(Text, out var absolutePriceScheduleId))
                return absolutePriceScheduleId;

            throw new ArgumentException($"Invalid text representation of an absolute price schedule identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an absolute price schedule identification.
        /// </summary>
        /// <param name="Text">A text representation of an absolute price schedule identification.</param>
        public static AbsolutePriceSchedule_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var absolutePriceScheduleId))
                return absolutePriceScheduleId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out AbsolutePriceScheduleId)

        /// <summary>
        /// Try to parse the given text as an absolute price schedule identification.
        /// </summary>
        /// <param name="Text">A text representation of an absolute price schedule identification.</param>
        /// <param name="AbsolutePriceScheduleId">The parsed absolute price schedule identification.</param>
        public static Boolean TryParse(String Text, out AbsolutePriceSchedule_Id AbsolutePriceScheduleId)
        {

            Text = Text.Trim();

            //ToDo: xs:hexBinary, length: 8

            if (Text.IsNotNullOrEmpty())
            {
                AbsolutePriceScheduleId = new AbsolutePriceSchedule_Id(Text);
                return true;
            }

            AbsolutePriceScheduleId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this absolute price schedule identification.
        /// </summary>
        public AbsolutePriceSchedule_Id Clone

            => new(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (AbsolutePriceScheduleId1, AbsolutePriceScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AbsolutePriceScheduleId1">An absolute price schedule identification.</param>
        /// <param name="AbsolutePriceScheduleId2">Another absolute price schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AbsolutePriceSchedule_Id AbsolutePriceScheduleId1,
                                           AbsolutePriceSchedule_Id AbsolutePriceScheduleId2)

            => AbsolutePriceScheduleId1.Equals(AbsolutePriceScheduleId2);

        #endregion

        #region Operator != (AbsolutePriceScheduleId1, AbsolutePriceScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AbsolutePriceScheduleId1">An absolute price schedule identification.</param>
        /// <param name="AbsolutePriceScheduleId2">Another absolute price schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AbsolutePriceSchedule_Id AbsolutePriceScheduleId1,
                                           AbsolutePriceSchedule_Id AbsolutePriceScheduleId2)

            => !AbsolutePriceScheduleId1.Equals(AbsolutePriceScheduleId2);

        #endregion

        #region Operator <  (AbsolutePriceScheduleId1, AbsolutePriceScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AbsolutePriceScheduleId1">An absolute price schedule identification.</param>
        /// <param name="AbsolutePriceScheduleId2">Another absolute price schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AbsolutePriceSchedule_Id AbsolutePriceScheduleId1,
                                          AbsolutePriceSchedule_Id AbsolutePriceScheduleId2)

            => AbsolutePriceScheduleId1.CompareTo(AbsolutePriceScheduleId2) < 0;

        #endregion

        #region Operator <= (AbsolutePriceScheduleId1, AbsolutePriceScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AbsolutePriceScheduleId1">An absolute price schedule identification.</param>
        /// <param name="AbsolutePriceScheduleId2">Another absolute price schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AbsolutePriceSchedule_Id AbsolutePriceScheduleId1,
                                           AbsolutePriceSchedule_Id AbsolutePriceScheduleId2)

            => AbsolutePriceScheduleId1.CompareTo(AbsolutePriceScheduleId2) <= 0;

        #endregion

        #region Operator >  (AbsolutePriceScheduleId1, AbsolutePriceScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AbsolutePriceScheduleId1">An absolute price schedule identification.</param>
        /// <param name="AbsolutePriceScheduleId2">Another absolute price schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AbsolutePriceSchedule_Id AbsolutePriceScheduleId1,
                                          AbsolutePriceSchedule_Id AbsolutePriceScheduleId2)

            => AbsolutePriceScheduleId1.CompareTo(AbsolutePriceScheduleId2) > 0;

        #endregion

        #region Operator >= (AbsolutePriceScheduleId1, AbsolutePriceScheduleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AbsolutePriceScheduleId1">An absolute price schedule identification.</param>
        /// <param name="AbsolutePriceScheduleId2">Another absolute price schedule identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AbsolutePriceSchedule_Id AbsolutePriceScheduleId1,
                                           AbsolutePriceSchedule_Id AbsolutePriceScheduleId2)

            => AbsolutePriceScheduleId1.CompareTo(AbsolutePriceScheduleId2) >= 0;

        #endregion

        #endregion

        #region IComparable<AbsolutePriceScheduleId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two absolute price schedule identifications.
        /// </summary>
        /// <param name="Object">An absolute price schedule identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is AbsolutePriceSchedule_Id absolutePriceScheduleId
                   ? CompareTo(absolutePriceScheduleId)
                   : throw new ArgumentException("The given object is not an absolute price schedule identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AbsolutePriceScheduleId)

        /// <summary>
        /// Compares two absolute price schedule identifications.
        /// </summary>
        /// <param name="AbsolutePriceScheduleId">An absolute price schedule identification to compare with.</param>
        public Int32 CompareTo(AbsolutePriceSchedule_Id AbsolutePriceScheduleId)

            => String.Compare(InternalId,
                              AbsolutePriceScheduleId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<AbsolutePriceScheduleId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two absolute price schedule identifications for equality.
        /// </summary>
        /// <param name="Object">An absolute price schedule identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AbsolutePriceSchedule_Id absolutePriceScheduleId &&
                   Equals(absolutePriceScheduleId);

        #endregion

        #region Equals(AbsolutePriceScheduleId)

        /// <summary>
        /// Compares two absolute price schedule identifications for equality.
        /// </summary>
        /// <param name="AbsolutePriceScheduleId">An absolute price schedule identification to compare with.</param>
        public Boolean Equals(AbsolutePriceSchedule_Id AbsolutePriceScheduleId)

            => String.Equals(InternalId,
                             AbsolutePriceScheduleId.InternalId,
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
