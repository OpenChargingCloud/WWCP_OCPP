/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// Extension methods for meter identifications.
    /// </summary>
    public static class MeterIdExtensions
    {

        /// <summary>
        /// Indicates whether this meter identification is null or empty.
        /// </summary>
        /// <param name="MeterId">A meter identification.</param>
        public static Boolean IsNullOrEmpty(this Meter_Id? MeterId)
            => !MeterId.HasValue || MeterId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this meter identification is null or empty.
        /// </summary>
        /// <param name="MeterId">A meter identification.</param>
        public static Boolean IsNotNullOrEmpty(this Meter_Id? MeterId)
            => MeterId.HasValue && MeterId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A meter identification.
    /// </summary>
    public readonly struct Meter_Id : IId,
                                      IEquatable<Meter_Id>,
                                      IComparable<Meter_Id>
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
        /// The length of the meter identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new meter identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a meter identification.</param>
        private Meter_Id(String Text)
        {
            this.InternalId  = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a meter identification.
        /// </summary>
        /// <param name="Text">A text representation of a meter identification.</param>
        public static Meter_Id Parse(String Text)
        {

            if (TryParse(Text, out var meterId))
                return meterId;

            throw new ArgumentException($"Invalid text representation of a meter identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a meter identification.
        /// </summary>
        /// <param name="Text">A text representation of a meter identification.</param>
        public static Meter_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var meterId))
                return meterId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out MeterId)

        /// <summary>
        /// Try to parse the given text as a meter identification.
        /// </summary>
        /// <param name="Text">A text representation of a meter identification.</param>
        /// <param name="MeterId">The parsed meter identification.</param>
        public static Boolean TryParse(String Text, out Meter_Id MeterId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                MeterId = new Meter_Id(Text);
                return true;
            }

            MeterId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this meter identification.
        /// </summary>
        public Meter_Id Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (MeterId1, MeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterId1">A meter identification.</param>
        /// <param name="MeterId2">Another meter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Meter_Id MeterId1,
                                           Meter_Id MeterId2)

            => MeterId1.Equals(MeterId2);

        #endregion

        #region Operator != (MeterId1, MeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterId1">A meter identification.</param>
        /// <param name="MeterId2">Another meter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Meter_Id MeterId1,
                                           Meter_Id MeterId2)

            => !MeterId1.Equals(MeterId2);

        #endregion

        #region Operator <  (MeterId1, MeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterId1">A meter identification.</param>
        /// <param name="MeterId2">Another meter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Meter_Id MeterId1,
                                          Meter_Id MeterId2)

            => MeterId1.CompareTo(MeterId2) < 0;

        #endregion

        #region Operator <= (MeterId1, MeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterId1">A meter identification.</param>
        /// <param name="MeterId2">Another meter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Meter_Id MeterId1,
                                           Meter_Id MeterId2)

            => MeterId1.CompareTo(MeterId2) <= 0;

        #endregion

        #region Operator >  (MeterId1, MeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterId1">A meter identification.</param>
        /// <param name="MeterId2">Another meter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Meter_Id MeterId1,
                                          Meter_Id MeterId2)

            => MeterId1.CompareTo(MeterId2) > 0;

        #endregion

        #region Operator >= (MeterId1, MeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterId1">A meter identification.</param>
        /// <param name="MeterId2">Another meter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Meter_Id MeterId1,
                                           Meter_Id MeterId2)

            => MeterId1.CompareTo(MeterId2) >= 0;

        #endregion

        #endregion

        #region IComparable<MeterId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two meter identifications.
        /// </summary>
        /// <param name="Object">A meter identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Meter_Id meterId
                   ? CompareTo(meterId)
                   : throw new ArgumentException("The given object is not a meter identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(MeterId)

        /// <summary>
        /// Compares two meter identifications.
        /// </summary>
        /// <param name="MeterId">A meter identification to compare with.</param>
        public Int32 CompareTo(Meter_Id MeterId)

            => String.Compare(InternalId,
                              MeterId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<MeterId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two meter identifications for equality.
        /// </summary>
        /// <param name="Object">A meter identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Meter_Id meterId &&
                   Equals(meterId);

        #endregion

        #region Equals(MeterId)

        /// <summary>
        /// Compares two meter identifications for equality.
        /// </summary>
        /// <param name="MeterId">A meter identification to compare with.</param>
        public Boolean Equals(Meter_Id MeterId)

            => String.Equals(InternalId,
                             MeterId.InternalId,
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
