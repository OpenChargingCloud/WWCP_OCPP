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
    /// Extension methods for periodic event stream identifications.
    /// </summary>
    public static class PeriodicEventStreamIdExtensions
    {

        /// <summary>
        /// Indicates whether this periodic event stream identification is null or empty.
        /// </summary>
        /// <param name="PeriodicEventStreamId">A periodic event stream identification.</param>
        public static Boolean IsNullOrEmpty(this PeriodicEventStream_Id? PeriodicEventStreamId)
            => !PeriodicEventStreamId.HasValue || PeriodicEventStreamId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this periodic event stream identification is null or empty.
        /// </summary>
        /// <param name="PeriodicEventStreamId">A periodic event stream identification.</param>
        public static Boolean IsNotNullOrEmpty(this PeriodicEventStream_Id? PeriodicEventStreamId)
            => PeriodicEventStreamId.HasValue && PeriodicEventStreamId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A periodic event stream identification.
    /// </summary>
    public readonly struct PeriodicEventStream_Id : IId,
                                                    IEquatable<PeriodicEventStream_Id>,
                                                    IComparable<PeriodicEventStream_Id>
    {

        #region Data

        /// <summary>
        /// The nummeric value of the periodic event stream identification.
        /// </summary>
        public readonly UInt64 Value;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => false;

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => false;

        /// <summary>
        /// The length of this identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) Value.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new periodic event stream identification based on the given number.
        /// </summary>
        /// <param name="Number">A numeric representation of a periodic event stream identification.</param>
        private PeriodicEventStream_Id(UInt64 Number)
        {
            this.Value = Number;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random periodic event stream identification.
        /// </summary>
        public static PeriodicEventStream_Id NewRandom

#pragma warning disable SCS0005 // Weak random number generator.
            => new ((UInt64) Random.Shared.Next(Int32.MaxValue));
#pragma warning restore SCS0005 // Weak random number generator.

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a periodic event stream identification.
        /// </summary>
        /// <param name="Text">A text representation of a periodic event stream identification.</param>
        public static PeriodicEventStream_Id Parse(String Text)
        {

            if (TryParse(Text, out var periodicEventStreamId))
                return periodicEventStreamId;

            throw new ArgumentException($"Invalid text representation of a periodic event stream identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Number)

        /// <summary>
        /// Parse the given number as a periodic event stream identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a periodic event stream identification.</param>
        public static PeriodicEventStream_Id Parse(UInt64 Number)

            => new (Number);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a periodic event stream identification.
        /// </summary>
        /// <param name="Text">A text representation of a periodic event stream identification.</param>
        public static PeriodicEventStream_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var periodicEventStreamId))
                return periodicEventStreamId;

            return null;

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a periodic event stream identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a periodic event stream identification.</param>
        public static PeriodicEventStream_Id? TryParse(UInt64 Number)
        {

            if (TryParse(Number, out var periodicEventStreamId))
                return periodicEventStreamId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out PeriodicEventStreamId)

        /// <summary>
        /// Try to parse the given text as a periodic event stream identification.
        /// </summary>
        /// <param name="Text">A text representation of a periodic event stream identification.</param>
        /// <param name="PeriodicEventStreamId">The parsed periodic event stream identification.</param>
        public static Boolean TryParse(String Text, out PeriodicEventStream_Id PeriodicEventStreamId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                UInt64.TryParse(Text, out var number))
            {
                PeriodicEventStreamId = new PeriodicEventStream_Id(number);
                return true;
            }

            PeriodicEventStreamId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out PeriodicEventStreamId)

        /// <summary>
        /// Try to parse the given number as a periodic event stream identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a periodic event stream identification.</param>
        /// <param name="PeriodicEventStreamId">The parsed periodic event stream identification.</param>
        public static Boolean TryParse(UInt64 Number, out PeriodicEventStream_Id PeriodicEventStreamId)
        {

            PeriodicEventStreamId = new PeriodicEventStream_Id(Number);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this periodic event stream identification.
        /// </summary>
        public PeriodicEventStream_Id Clone

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (PeriodicEventStreamId1, PeriodicEventStreamId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PeriodicEventStreamId1">A periodic event stream identification.</param>
        /// <param name="PeriodicEventStreamId2">Another periodic event stream identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PeriodicEventStream_Id PeriodicEventStreamId1,
                                           PeriodicEventStream_Id PeriodicEventStreamId2)

            => PeriodicEventStreamId1.Equals(PeriodicEventStreamId2);

        #endregion

        #region Operator != (PeriodicEventStreamId1, PeriodicEventStreamId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PeriodicEventStreamId1">A periodic event stream identification.</param>
        /// <param name="PeriodicEventStreamId2">Another periodic event stream identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PeriodicEventStream_Id PeriodicEventStreamId1,
                                           PeriodicEventStream_Id PeriodicEventStreamId2)

            => !PeriodicEventStreamId1.Equals(PeriodicEventStreamId2);

        #endregion

        #region Operator <  (PeriodicEventStreamId1, PeriodicEventStreamId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PeriodicEventStreamId1">A periodic event stream identification.</param>
        /// <param name="PeriodicEventStreamId2">Another periodic event stream identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PeriodicEventStream_Id PeriodicEventStreamId1,
                                          PeriodicEventStream_Id PeriodicEventStreamId2)

            => PeriodicEventStreamId1.CompareTo(PeriodicEventStreamId2) < 0;

        #endregion

        #region Operator <= (PeriodicEventStreamId1, PeriodicEventStreamId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PeriodicEventStreamId1">A periodic event stream identification.</param>
        /// <param name="PeriodicEventStreamId2">Another periodic event stream identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PeriodicEventStream_Id PeriodicEventStreamId1,
                                           PeriodicEventStream_Id PeriodicEventStreamId2)

            => PeriodicEventStreamId1.CompareTo(PeriodicEventStreamId2) <= 0;

        #endregion

        #region Operator >  (PeriodicEventStreamId1, PeriodicEventStreamId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PeriodicEventStreamId1">A periodic event stream identification.</param>
        /// <param name="PeriodicEventStreamId2">Another periodic event stream identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PeriodicEventStream_Id PeriodicEventStreamId1,
                                          PeriodicEventStream_Id PeriodicEventStreamId2)

            => PeriodicEventStreamId1.CompareTo(PeriodicEventStreamId2) > 0;

        #endregion

        #region Operator >= (PeriodicEventStreamId1, PeriodicEventStreamId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PeriodicEventStreamId1">A periodic event stream identification.</param>
        /// <param name="PeriodicEventStreamId2">Another periodic event stream identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PeriodicEventStream_Id PeriodicEventStreamId1,
                                           PeriodicEventStream_Id PeriodicEventStreamId2)

            => PeriodicEventStreamId1.CompareTo(PeriodicEventStreamId2) >= 0;

        #endregion

        #endregion

        #region IComparable<PeriodicEventStreamId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two periodic event stream identifications.
        /// </summary>
        /// <param name="Object">A periodic event stream identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PeriodicEventStream_Id periodicEventStreamId
                   ? CompareTo(periodicEventStreamId)
                   : throw new ArgumentException("The given object is not a periodic event stream identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PeriodicEventStreamId)

        /// <summary>
        /// Compares two periodic event stream identifications.
        /// </summary>
        /// <param name="PeriodicEventStreamId">A periodic event stream identification to compare with.</param>
        public Int32 CompareTo(PeriodicEventStream_Id PeriodicEventStreamId)

            => Value.CompareTo(PeriodicEventStreamId.Value);

        #endregion

        #endregion

        #region IEquatable<PeriodicEventStreamId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two periodic event stream identifications for equality.
        /// </summary>
        /// <param name="Object">A periodic event stream identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PeriodicEventStream_Id periodicEventStreamId &&
                   Equals(periodicEventStreamId);

        #endregion

        #region Equals(PeriodicEventStreamId)

        /// <summary>
        /// Compares two periodic event stream identifications for equality.
        /// </summary>
        /// <param name="PeriodicEventStreamId">A periodic event stream identification to compare with.</param>
        public Boolean Equals(PeriodicEventStream_Id PeriodicEventStreamId)

            => Value.Equals(PeriodicEventStreamId.Value);

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
