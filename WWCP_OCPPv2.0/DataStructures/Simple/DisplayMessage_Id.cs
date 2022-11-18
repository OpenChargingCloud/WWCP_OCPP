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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extention methods for display message identifications.
    /// </summary>
    public static class DisplayMessageIdExtentions
    {

        /// <summary>
        /// Indicates whether this display message identification is null or empty.
        /// </summary>
        /// <param name="DisplayMessageId">A display message identification.</param>
        public static Boolean IsNullOrEmpty(this DisplayMessage_Id? DisplayMessageId)
            => !DisplayMessageId.HasValue || DisplayMessageId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this display message identification is null or empty.
        /// </summary>
        /// <param name="DisplayMessageId">A display message identification.</param>
        public static Boolean IsNotNullOrEmpty(this DisplayMessage_Id? DisplayMessageId)
            => DisplayMessageId.HasValue && DisplayMessageId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A display message identification.
    /// </summary>
    public readonly struct DisplayMessage_Id : IId,
                                               IEquatable<DisplayMessage_Id>,
                                               IComparable<DisplayMessage_Id>
    {

        #region Data

        /// <summary>
        /// The nummeric value of the transaction identification.
        /// </summary>
        public readonly UInt64 Value;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => Value == 0;

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => Value != 0;

        /// <summary>
        /// The length of the tag identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) Value.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new display message identification.
        /// </summary>
        /// <param name="Number">A number.</param>
        private DisplayMessage_Id(UInt64 Number)
        {
            this.Value = Number;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random display message identification.
        /// </summary>
        public static DisplayMessage_Id NewRandom

#pragma warning disable SCS0005 // Weak random number generator.
            => new ((UInt64) Random.Shared.Next(Int32.MaxValue));
#pragma warning restore SCS0005 // Weak random number generator.

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a display message identification.
        /// </summary>
        /// <param name="Text">A text representation of a display message identification.</param>
        public static DisplayMessage_Id Parse(String Text)
        {

            if (TryParse(Text, out var displayMessageId))
                return displayMessageId;

            throw new ArgumentException("Invalid text representation of a display message identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Number)

        /// <summary>
        /// Parse the given number as a display message identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a display message identification.</param>
        public static DisplayMessage_Id Parse(UInt64 Number)

            => new (Number);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a display message identification.
        /// </summary>
        /// <param name="Text">A text representation of a display message identification.</param>
        public static DisplayMessage_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var displayMessageId))
                return displayMessageId;

            return null;

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a display message identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a display message identification.</param>
        public static DisplayMessage_Id? TryParse(UInt64 Number)
        {

            if (TryParse(Number, out var displayMessageId))
                return displayMessageId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out DisplayMessageId)

        /// <summary>
        /// Try to parse the given string as a display message identification.
        /// </summary>
        /// <param name="Text">A text representation of a display message identification.</param>
        /// <param name="DisplayMessageId">The parsed display message identification.</param>
        public static Boolean TryParse(String Text, out DisplayMessage_Id DisplayMessageId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                UInt64.TryParse(Text, out var number))
            {
                DisplayMessageId = new DisplayMessage_Id(number);
                return true;
            }

            DisplayMessageId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out DisplayMessageId)

        /// <summary>
        /// Try to parse the given number as a display message identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a display message identification.</param>
        /// <param name="DisplayMessageId">The parsed display message identification.</param>
        public static Boolean TryParse(UInt64 Number, out DisplayMessage_Id DisplayMessageId)
        {

            DisplayMessageId = new DisplayMessage_Id(Number);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this display message identification.
        /// </summary>
        public DisplayMessage_Id Clone

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (DisplayMessageId1, DisplayMessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayMessageId1">A display message identification.</param>
        /// <param name="DisplayMessageId2">Another display message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DisplayMessage_Id DisplayMessageId1,
                                           DisplayMessage_Id DisplayMessageId2)

            => DisplayMessageId1.Equals(DisplayMessageId2);

        #endregion

        #region Operator != (DisplayMessageId1, DisplayMessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayMessageId1">A display message identification.</param>
        /// <param name="DisplayMessageId2">Another display message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DisplayMessage_Id DisplayMessageId1,
                                           DisplayMessage_Id DisplayMessageId2)

            => !DisplayMessageId1.Equals(DisplayMessageId2);

        #endregion

        #region Operator <  (DisplayMessageId1, DisplayMessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayMessageId1">A display message identification.</param>
        /// <param name="DisplayMessageId2">Another display message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (DisplayMessage_Id DisplayMessageId1,
                                          DisplayMessage_Id DisplayMessageId2)

            => DisplayMessageId1.CompareTo(DisplayMessageId2) < 0;

        #endregion

        #region Operator <= (DisplayMessageId1, DisplayMessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayMessageId1">A display message identification.</param>
        /// <param name="DisplayMessageId2">Another display message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (DisplayMessage_Id DisplayMessageId1,
                                           DisplayMessage_Id DisplayMessageId2)

            => DisplayMessageId1.CompareTo(DisplayMessageId2) <= 0;

        #endregion

        #region Operator >  (DisplayMessageId1, DisplayMessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayMessageId1">A display message identification.</param>
        /// <param name="DisplayMessageId2">Another display message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (DisplayMessage_Id DisplayMessageId1,
                                          DisplayMessage_Id DisplayMessageId2)

            => DisplayMessageId1.CompareTo(DisplayMessageId2) > 0;

        #endregion

        #region Operator >= (DisplayMessageId1, DisplayMessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DisplayMessageId1">A display message identification.</param>
        /// <param name="DisplayMessageId2">Another display message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (DisplayMessage_Id DisplayMessageId1,
                                           DisplayMessage_Id DisplayMessageId2)

            => DisplayMessageId1.CompareTo(DisplayMessageId2) >= 0;

        #endregion

        #endregion

        #region IComparable<DisplayMessageId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two display message identifications.
        /// </summary>
        /// <param name="Object">A display message identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is DisplayMessage_Id displayMessageId
                   ? CompareTo(displayMessageId)
                   : throw new ArgumentException("The given object is not a display message identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(DisplayMessageId)

        /// <summary>
        /// Compares two display message identifications.
        /// </summary>
        /// <param name="DisplayMessageId">A display message identification to compare with.</param>
        public Int32 CompareTo(DisplayMessage_Id DisplayMessageId)

            => Value.CompareTo(DisplayMessageId.Value);

        #endregion

        #endregion

        #region IEquatable<DisplayMessageId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two display message identifications for equality.
        /// </summary>
        /// <param name="Object">A display message identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DisplayMessage_Id displayMessageId &&
                   Equals(displayMessageId);

        #endregion

        #region Equals(DisplayMessageId)

        /// <summary>
        /// Compares two display message identifications for equality.
        /// </summary>
        /// <param name="DisplayMessageId">A display message identification to compare with.</param>
        public Boolean Equals(DisplayMessage_Id DisplayMessageId)

            => Value.Equals(DisplayMessageId.Value);

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
