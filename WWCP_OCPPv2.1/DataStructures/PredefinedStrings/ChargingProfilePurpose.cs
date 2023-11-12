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
    /// Extension methods for charging profile purposes.
    /// </summary>
    public static class ChargingProfilePurposeExtensions
    {

        /// <summary>
        /// Indicates whether this charging profile purpose is null or empty.
        /// </summary>
        /// <param name="ChargingProfilePurpose">A charging profile purpose.</param>
        public static Boolean IsNullOrEmpty(this ChargingProfilePurpose? ChargingProfilePurpose)
            => !ChargingProfilePurpose.HasValue || ChargingProfilePurpose.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this charging profile purpose is null or empty.
        /// </summary>
        /// <param name="ChargingProfilePurpose">A charging profile purpose.</param>
        public static Boolean IsNotNullOrEmpty(this ChargingProfilePurpose? ChargingProfilePurpose)
            => ChargingProfilePurpose.HasValue && ChargingProfilePurpose.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A charging profile purpose.
    /// </summary>
    public readonly struct ChargingProfilePurpose : IId,
                                                    IEquatable<ChargingProfilePurpose>,
                                                    IComparable<ChargingProfilePurpose>
    {

        #region Data

        private readonly static Dictionary<String, ChargingProfilePurpose>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                      InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this charging profile purpose is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this charging profile purpose is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the charging profile purpose.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging profile purpose based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a charging profile purpose.</param>
        private ChargingProfilePurpose(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static ChargingProfilePurpose Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new ChargingProfilePurpose(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a charging profile purpose.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile purpose.</param>
        public static ChargingProfilePurpose Parse(String Text)
        {

            if (TryParse(Text, out var chargingProfilePurpose))
                return chargingProfilePurpose;

            throw new ArgumentException($"Invalid text representation of a charging profile purpose: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charging profile purpose.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile purpose.</param>
        public static ChargingProfilePurpose? TryParse(String Text)
        {

            if (TryParse(Text, out var chargingProfilePurpose))
                return chargingProfilePurpose;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ChargingProfilePurpose)

        /// <summary>
        /// Try to parse the given text as a charging profile purpose.
        /// </summary>
        /// <param name="Text">A text representation of a charging profile purpose.</param>
        /// <param name="ChargingProfilePurpose">The parsed charging profile purpose.</param>
        public static Boolean TryParse(String Text, out ChargingProfilePurpose ChargingProfilePurpose)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out ChargingProfilePurpose))
                    ChargingProfilePurpose = Register(Text);

                return true;

            }

            ChargingProfilePurpose = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charging profile purpose.
        /// </summary>
        public ChargingProfilePurpose Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Additional constraints that will be incorporated into a local power schedule. Only valid for a charging station.
        /// Therefore the EVSE identification MUST be 0 in the SetChargingProfileRequest message.
        /// </summary>
        public static ChargingProfilePurpose ChargingStationExternalConstraints    { get; }
            = Register("ChargingStationExternalConstraints");

        /// <summary>
        /// Configuration for the maximum power or current available
        /// for an entire charging station.
        /// </summary>
        public static ChargingProfilePurpose ChargePointMaxProfile                 { get; }
            = Register("ChargePointMaxProfile");

        /// <summary>
        /// This profile is used in place of a Tx(Default)Profile when priority charging is requested,
        /// either locally on the charging station or by a UsePriorityChargingRequest from the CSMS.
        /// </summary>
        public static ChargingProfilePurpose PriorityCharging                      { get; }
            = Register("ApplicationReset");

        /// <summary>
        /// This profile adds capacity from local generation. Its capacity is added on top of other
        /// charging profiles.
        /// </summary>
        public static ChargingProfilePurpose LocalGeneration                       { get; }
            = Register("LocalGeneration");

        /// <summary>
        /// Default profile to be used for new transactions.
        /// </summary>
        public static ChargingProfilePurpose TxDefaultProfile                      { get; }
            = Register("TxDefaultProfile");

        /// <summary>
        /// Profile with constraints to be imposed by the charging station
        /// on the current transaction. A profile with this purpose
        /// SHALL cease to be valid when the transaction terminates.
        /// </summary>
        public static ChargingProfilePurpose TxProfile                             { get; }
            = Register("TxProfile");

        #endregion


        #region Operator overloading

        #region Operator == (ChargingProfilePurpose1, ChargingProfilePurpose2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfilePurpose1">A charging profile purpose.</param>
        /// <param name="ChargingProfilePurpose2">Another charging profile purpose.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingProfilePurpose ChargingProfilePurpose1,
                                           ChargingProfilePurpose ChargingProfilePurpose2)

            => ChargingProfilePurpose1.Equals(ChargingProfilePurpose2);

        #endregion

        #region Operator != (ChargingProfilePurpose1, ChargingProfilePurpose2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfilePurpose1">A charging profile purpose.</param>
        /// <param name="ChargingProfilePurpose2">Another charging profile purpose.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingProfilePurpose ChargingProfilePurpose1,
                                           ChargingProfilePurpose ChargingProfilePurpose2)

            => !ChargingProfilePurpose1.Equals(ChargingProfilePurpose2);

        #endregion

        #region Operator <  (ChargingProfilePurpose1, ChargingProfilePurpose2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfilePurpose1">A charging profile purpose.</param>
        /// <param name="ChargingProfilePurpose2">Another charging profile purpose.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingProfilePurpose ChargingProfilePurpose1,
                                          ChargingProfilePurpose ChargingProfilePurpose2)

            => ChargingProfilePurpose1.CompareTo(ChargingProfilePurpose2) < 0;

        #endregion

        #region Operator <= (ChargingProfilePurpose1, ChargingProfilePurpose2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfilePurpose1">A charging profile purpose.</param>
        /// <param name="ChargingProfilePurpose2">Another charging profile purpose.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingProfilePurpose ChargingProfilePurpose1,
                                           ChargingProfilePurpose ChargingProfilePurpose2)

            => ChargingProfilePurpose1.CompareTo(ChargingProfilePurpose2) <= 0;

        #endregion

        #region Operator >  (ChargingProfilePurpose1, ChargingProfilePurpose2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfilePurpose1">A charging profile purpose.</param>
        /// <param name="ChargingProfilePurpose2">Another charging profile purpose.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingProfilePurpose ChargingProfilePurpose1,
                                          ChargingProfilePurpose ChargingProfilePurpose2)

            => ChargingProfilePurpose1.CompareTo(ChargingProfilePurpose2) > 0;

        #endregion

        #region Operator >= (ChargingProfilePurpose1, ChargingProfilePurpose2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfilePurpose1">A charging profile purpose.</param>
        /// <param name="ChargingProfilePurpose2">Another charging profile purpose.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingProfilePurpose ChargingProfilePurpose1,
                                           ChargingProfilePurpose ChargingProfilePurpose2)

            => ChargingProfilePurpose1.CompareTo(ChargingProfilePurpose2) >= 0;

        #endregion

        #endregion

        #region IComparable<ChargingProfilePurpose> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging profile purposes.
        /// </summary>
        /// <param name="Object">A charging profile purpose to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ChargingProfilePurpose chargingProfilePurpose
                   ? CompareTo(chargingProfilePurpose)
                   : throw new ArgumentException("The given object is not a charging profile purpose!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingProfilePurpose)

        /// <summary>
        /// Compares two charging profile purposes.
        /// </summary>
        /// <param name="ChargingProfilePurpose">A charging profile purpose to compare with.</param>
        public Int32 CompareTo(ChargingProfilePurpose ChargingProfilePurpose)

            => String.Compare(InternalId,
                              ChargingProfilePurpose.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ChargingProfilePurpose> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging profile purposes for equality.
        /// </summary>
        /// <param name="Object">A charging profile purpose to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingProfilePurpose chargingProfilePurpose &&
                   Equals(chargingProfilePurpose);

        #endregion

        #region Equals(ChargingProfilePurpose)

        /// <summary>
        /// Compares two charging profile purposes for equality.
        /// </summary>
        /// <param name="ChargingProfilePurpose">A charging profile purpose to compare with.</param>
        public Boolean Equals(ChargingProfilePurpose ChargingProfilePurpose)

            => String.Equals(InternalId,
                             ChargingProfilePurpose.InternalId,
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
