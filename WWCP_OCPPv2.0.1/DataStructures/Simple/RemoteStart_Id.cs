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

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Extension methods for remote start identifications.
    /// </summary>
    public static class RemoteStartIdExtensions
    {

        /// <summary>
        /// Indicates whether this remote start identification is null or empty.
        /// </summary>
        /// <param name="RemoteStartId">A remote start identification.</param>
        public static Boolean IsNullOrEmpty(this RemoteStart_Id? RemoteStartId)
            => !RemoteStartId.HasValue || RemoteStartId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this remote start identification is null or empty.
        /// </summary>
        /// <param name="RemoteStartId">A remote start identification.</param>
        public static Boolean IsNotNullOrEmpty(this RemoteStart_Id? RemoteStartId)
            => RemoteStartId.HasValue && RemoteStartId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A remote start identification.
    /// </summary>
    public readonly struct RemoteStart_Id : IId,
                                            IEquatable<RemoteStart_Id>,
                                            IComparable<RemoteStart_Id>
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
        /// The length of the remote start identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) Value.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new remote start identification based on the given number.
        /// </summary>
        /// <param name="Number">A numeric representation of a remote start identification.</param>
        private RemoteStart_Id(UInt64 Number)
        {
            this.Value = Number;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random remote start identification.
        /// </summary>
        public static RemoteStart_Id NewRandom

#pragma warning disable SCS0005 // Weak random number generator.
            => new ((UInt64) Random.Shared.Next(Int32.MaxValue));
#pragma warning restore SCS0005 // Weak random number generator.

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a remote start identification.
        /// </summary>
        /// <param name="Text">A text representation of a remote start identification.</param>
        public static RemoteStart_Id Parse(String Text)
        {

            if (TryParse(Text, out var chargingProfileId))
                return chargingProfileId;

            throw new ArgumentException("Invalid text representation of a remote start identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Number)

        /// <summary>
        /// Parse the given number as a remote start identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a remote start identification.</param>
        public static RemoteStart_Id Parse(UInt64 Number)

            => new (Number);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a remote start identification.
        /// </summary>
        /// <param name="Text">A text representation of a remote start identification.</param>
        public static RemoteStart_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var chargingProfileId))
                return chargingProfileId;

            return null;

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a remote start identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a remote start identification.</param>
        public static RemoteStart_Id? TryParse(UInt64 Number)
        {

            if (TryParse(Number, out var chargingProfileId))
                return chargingProfileId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out RemoteStartId)

        /// <summary>
        /// Try to parse the given text as a remote start identification.
        /// </summary>
        /// <param name="Text">A text representation of a remote start identification.</param>
        /// <param name="RemoteStartId">The parsed remote start identification.</param>
        public static Boolean TryParse(String Text, out RemoteStart_Id RemoteStartId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                UInt64.TryParse(Text, out var number))
            {
                RemoteStartId = new RemoteStart_Id(number);
                return true;
            }

            RemoteStartId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out RemoteStartId)

        /// <summary>
        /// Try to parse the given number as a remote start identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a remote start identification.</param>
        /// <param name="RemoteStartId">The parsed remote start identification.</param>
        public static Boolean TryParse(UInt64 Number, out RemoteStart_Id RemoteStartId)
        {

            RemoteStartId = new RemoteStart_Id(Number);

            return true;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this remote start identification.
        /// </summary>
        public RemoteStart_Id Clone

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (RemoteStartId1, RemoteStartId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteStartId1">A remote start identification.</param>
        /// <param name="RemoteStartId2">Another remote start identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RemoteStart_Id RemoteStartId1,
                                           RemoteStart_Id RemoteStartId2)

            => RemoteStartId1.Equals(RemoteStartId2);

        #endregion

        #region Operator != (RemoteStartId1, RemoteStartId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteStartId1">A remote start identification.</param>
        /// <param name="RemoteStartId2">Another remote start identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RemoteStart_Id RemoteStartId1,
                                           RemoteStart_Id RemoteStartId2)

            => !RemoteStartId1.Equals(RemoteStartId2);

        #endregion

        #region Operator <  (RemoteStartId1, RemoteStartId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteStartId1">A remote start identification.</param>
        /// <param name="RemoteStartId2">Another remote start identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (RemoteStart_Id RemoteStartId1,
                                          RemoteStart_Id RemoteStartId2)

            => RemoteStartId1.CompareTo(RemoteStartId2) < 0;

        #endregion

        #region Operator <= (RemoteStartId1, RemoteStartId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteStartId1">A remote start identification.</param>
        /// <param name="RemoteStartId2">Another remote start identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (RemoteStart_Id RemoteStartId1,
                                           RemoteStart_Id RemoteStartId2)

            => RemoteStartId1.CompareTo(RemoteStartId2) <= 0;

        #endregion

        #region Operator >  (RemoteStartId1, RemoteStartId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteStartId1">A remote start identification.</param>
        /// <param name="RemoteStartId2">Another remote start identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (RemoteStart_Id RemoteStartId1,
                                          RemoteStart_Id RemoteStartId2)

            => RemoteStartId1.CompareTo(RemoteStartId2) > 0;

        #endregion

        #region Operator >= (RemoteStartId1, RemoteStartId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoteStartId1">A remote start identification.</param>
        /// <param name="RemoteStartId2">Another remote start identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (RemoteStart_Id RemoteStartId1,
                                           RemoteStart_Id RemoteStartId2)

            => RemoteStartId1.CompareTo(RemoteStartId2) >= 0;

        #endregion

        #endregion

        #region IComparable<RemoteStartId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two remote start identifications.
        /// </summary>
        /// <param name="Object">A remote start identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is RemoteStart_Id chargingProfileId
                   ? CompareTo(chargingProfileId)
                   : throw new ArgumentException("The given object is not a remote start identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RemoteStartId)

        /// <summary>
        /// Compares two remote start identifications.
        /// </summary>
        /// <param name="RemoteStartId">A remote start identification to compare with.</param>
        public Int32 CompareTo(RemoteStart_Id RemoteStartId)

            => Value.CompareTo(RemoteStartId.Value);

        #endregion

        #endregion

        #region IEquatable<RemoteStartId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two remote start identifications for equality.
        /// </summary>
        /// <param name="Object">A remote start identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RemoteStart_Id chargingProfileId &&
                   Equals(chargingProfileId);

        #endregion

        #region Equals(RemoteStartId)

        /// <summary>
        /// Compares two remote start identifications for equality.
        /// </summary>
        /// <param name="RemoteStartId">A remote start identification to compare with.</param>
        public Boolean Equals(RemoteStart_Id RemoteStartId)

            => Value.Equals(RemoteStartId.Value);

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
