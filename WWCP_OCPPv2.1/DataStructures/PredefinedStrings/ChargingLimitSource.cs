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
    /// Extension methods for charging limit sources.
    /// </summary>
    public static class ChargingLimitSourceExtensions
    {

        /// <summary>
        /// Indicates whether this charging limit source is null or empty.
        /// </summary>
        /// <param name="ChargingLimitSource">A charging limit source.</param>
        public static Boolean IsNullOrEmpty(this ChargingLimitSource? ChargingLimitSource)
            => !ChargingLimitSource.HasValue || ChargingLimitSource.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this charging limit source is null or empty.
        /// </summary>
        /// <param name="ChargingLimitSource">A charging limit source.</param>
        public static Boolean IsNotNullOrEmpty(this ChargingLimitSource? ChargingLimitSource)
            => ChargingLimitSource.HasValue && ChargingLimitSource.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A charging limit source.
    /// </summary>
    public readonly struct ChargingLimitSource : IId,
                                                 IEquatable<ChargingLimitSource>,
                                                 IComparable<ChargingLimitSource>
    {

        #region Data

        private readonly static Dictionary<String, ChargingLimitSource>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                   InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this charging limit source is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this charging limit source is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the charging limit source.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging limit source based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a charging limit source.</param>
        private ChargingLimitSource(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static ChargingLimitSource Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new ChargingLimitSource(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a charging limit source.
        /// </summary>
        /// <param name="Text">A text representation of a charging limit source.</param>
        public static ChargingLimitSource Parse(String Text)
        {

            if (TryParse(Text, out var chargingLimitSource))
                return chargingLimitSource;

            throw new ArgumentException($"Invalid text representation of a charging limit source: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as charging limit source.
        /// </summary>
        /// <param name="Text">A text representation of a charging limit source.</param>
        public static ChargingLimitSource? TryParse(String Text)
        {

            if (TryParse(Text, out var chargingLimitSource))
                return chargingLimitSource;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ChargingLimitSource)

        /// <summary>
        /// Try to parse the given text as charging limit source.
        /// </summary>
        /// <param name="Text">A text representation of a charging limit source.</param>
        /// <param name="ChargingLimitSource">The parsed charging limit source.</param>
        public static Boolean TryParse(String Text, out ChargingLimitSource ChargingLimitSource)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out ChargingLimitSource))
                    ChargingLimitSource = Register(Text);

                return true;

            }

            ChargingLimitSource = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charging limit source.
        /// </summary>
        public ChargingLimitSource Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Indicates that an Energy Management System has sent a charging limit.
        /// </summary>
        public static ChargingLimitSource EMS      { get; }
            = Register("EMS");

        /// <summary>
        /// Indicates that an external source, not being an EMS or system operator, has sent a charging limit.
        /// </summary>
        public static ChargingLimitSource Other    { get; }
            = Register("Other");

        /// <summary>
        /// Indicates that a System Operator (DSO or TSO) has sent a charging limit.
        /// </summary>
        public static ChargingLimitSource SO       { get; }
            = Register("SO");

        /// <summary>
        /// Indicates that the CSO has set this charging profile.
        /// </summary>
        public static ChargingLimitSource CSO      { get; }
            = Register("CSO");

        #endregion


        #region Operator overloading

        #region Operator == (ChargingLimitSource1, ChargingLimitSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingLimitSource1">A charging limit source.</param>
        /// <param name="ChargingLimitSource2">Another charging limit source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingLimitSource ChargingLimitSource1,
                                           ChargingLimitSource ChargingLimitSource2)

            => ChargingLimitSource1.Equals(ChargingLimitSource2);

        #endregion

        #region Operator != (ChargingLimitSource1, ChargingLimitSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingLimitSource1">A charging limit source.</param>
        /// <param name="ChargingLimitSource2">Another charging limit source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingLimitSource ChargingLimitSource1,
                                           ChargingLimitSource ChargingLimitSource2)

            => !ChargingLimitSource1.Equals(ChargingLimitSource2);

        #endregion

        #region Operator <  (ChargingLimitSource1, ChargingLimitSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingLimitSource1">A charging limit source.</param>
        /// <param name="ChargingLimitSource2">Another charging limit source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingLimitSource ChargingLimitSource1,
                                          ChargingLimitSource ChargingLimitSource2)

            => ChargingLimitSource1.CompareTo(ChargingLimitSource2) < 0;

        #endregion

        #region Operator <= (ChargingLimitSource1, ChargingLimitSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingLimitSource1">A charging limit source.</param>
        /// <param name="ChargingLimitSource2">Another charging limit source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingLimitSource ChargingLimitSource1,
                                           ChargingLimitSource ChargingLimitSource2)

            => ChargingLimitSource1.CompareTo(ChargingLimitSource2) <= 0;

        #endregion

        #region Operator >  (ChargingLimitSource1, ChargingLimitSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingLimitSource1">A charging limit source.</param>
        /// <param name="ChargingLimitSource2">Another charging limit source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingLimitSource ChargingLimitSource1,
                                          ChargingLimitSource ChargingLimitSource2)

            => ChargingLimitSource1.CompareTo(ChargingLimitSource2) > 0;

        #endregion

        #region Operator >= (ChargingLimitSource1, ChargingLimitSource2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingLimitSource1">A charging limit source.</param>
        /// <param name="ChargingLimitSource2">Another charging limit source.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingLimitSource ChargingLimitSource1,
                                           ChargingLimitSource ChargingLimitSource2)

            => ChargingLimitSource1.CompareTo(ChargingLimitSource2) >= 0;

        #endregion

        #endregion

        #region IComparable<ChargingLimitSource> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging limit sources.
        /// </summary>
        /// <param name="Object">A charging limit source to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ChargingLimitSource chargingLimitSource
                   ? CompareTo(chargingLimitSource)
                   : throw new ArgumentException("The given object is not charging limit source!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingLimitSource)

        /// <summary>
        /// Compares two charging limit sources.
        /// </summary>
        /// <param name="ChargingLimitSource">A charging limit source to compare with.</param>
        public Int32 CompareTo(ChargingLimitSource ChargingLimitSource)

            => String.Compare(InternalId,
                              ChargingLimitSource.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ChargingLimitSource> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging limit sources for equality.
        /// </summary>
        /// <param name="Object">A charging limit source to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingLimitSource chargingLimitSource &&
                   Equals(chargingLimitSource);

        #endregion

        #region Equals(ChargingLimitSource)

        /// <summary>
        /// Compares two charging limit sources for equality.
        /// </summary>
        /// <param name="ChargingLimitSource">A charging limit source to compare with.</param>
        public Boolean Equals(ChargingLimitSource ChargingLimitSource)

            => String.Equals(InternalId,
                             ChargingLimitSource.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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
