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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for battery swap events.
    /// </summary>
    public static class BatterySwapEventExtensions
    {

        /// <summary>
        /// Indicates whether this battery swap event is null or empty.
        /// </summary>
        /// <param name="BatterySwapEvent">A battery swap event.</param>
        public static Boolean IsNullOrEmpty(this BatterySwapEvent? BatterySwapEvent)
            => !BatterySwapEvent.HasValue || BatterySwapEvent.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this battery swap event is null or empty.
        /// </summary>
        /// <param name="BatterySwapEvent">A battery swap event.</param>
        public static Boolean IsNotNullOrEmpty(this BatterySwapEvent? BatterySwapEvent)
            => BatterySwapEvent.HasValue && BatterySwapEvent.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A battery swap event.
    /// </summary>
    public readonly struct BatterySwapEvent : IId,
                                              IEquatable<BatterySwapEvent>,
                                              IComparable<BatterySwapEvent>
    {

        #region Data

        private readonly static Dictionary<String, BatterySwapEvent>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                          InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this battery swap event is null or empty.
        /// </summary>
        public readonly  Boolean                        IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this battery swap event is NOT null or empty.
        /// </summary>
        public readonly  Boolean                        IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the battery swap event.
        /// </summary>
        public readonly  UInt64                         Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered battery swap events.
        /// </summary>
        public static    IEnumerable<BatterySwapEvent>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new battery swap event based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a battery swap event.</param>
        private BatterySwapEvent(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static BatterySwapEvent Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new BatterySwapEvent(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a battery swap event.
        /// </summary>
        /// <param name="Text">A text representation of a battery swap event.</param>
        public static BatterySwapEvent Parse(String Text)
        {

            if (TryParse(Text, out var batterySwapEvent))
                return batterySwapEvent;

            throw new ArgumentException($"Invalid text representation of a battery swap event: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a battery swap event.
        /// </summary>
        /// <param name="Text">A text representation of a battery swap event.</param>
        public static BatterySwapEvent? TryParse(String Text)
        {

            if (TryParse(Text, out var batterySwapEvent))
                return batterySwapEvent;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out BatterySwapEvent)

        /// <summary>
        /// Try to parse the given text as a battery swap event.
        /// </summary>
        /// <param name="Text">A text representation of a battery swap event.</param>
        /// <param name="BatterySwapEvent">The parsed battery swap event.</param>
        public static Boolean TryParse(String Text, out BatterySwapEvent BatterySwapEvent)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out BatterySwapEvent))
                    BatterySwapEvent = Register(Text);

                return true;

            }

            BatterySwapEvent = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this battery swap event.
        /// </summary>
        public BatterySwapEvent Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Battery In
        /// </summary>
        public static BatterySwapEvent  BatteryIn            { get; }
            = Register("BatteryIn");

        /// <summary>
        /// Battery Out
        /// </summary>
        public static BatterySwapEvent  BatteryOut           { get; }
            = Register("BatteryOut");

        /// <summary>
        /// Battery Out Timeout
        /// </summary>
        public static BatterySwapEvent  BatteryOutTimeout    { get; }
            = Register("BatteryOutTimeout");

        #endregion


        #region Operator overloading

        #region Operator == (BatterySwapEvent1, BatterySwapEvent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BatterySwapEvent1">A battery swap event.</param>
        /// <param name="BatterySwapEvent2">Another battery swap event.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BatterySwapEvent BatterySwapEvent1,
                                           BatterySwapEvent BatterySwapEvent2)

            => BatterySwapEvent1.Equals(BatterySwapEvent2);

        #endregion

        #region Operator != (BatterySwapEvent1, BatterySwapEvent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BatterySwapEvent1">A battery swap event.</param>
        /// <param name="BatterySwapEvent2">Another battery swap event.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BatterySwapEvent BatterySwapEvent1,
                                           BatterySwapEvent BatterySwapEvent2)

            => !BatterySwapEvent1.Equals(BatterySwapEvent2);

        #endregion

        #region Operator <  (BatterySwapEvent1, BatterySwapEvent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BatterySwapEvent1">A battery swap event.</param>
        /// <param name="BatterySwapEvent2">Another battery swap event.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BatterySwapEvent BatterySwapEvent1,
                                          BatterySwapEvent BatterySwapEvent2)

            => BatterySwapEvent1.CompareTo(BatterySwapEvent2) < 0;

        #endregion

        #region Operator <= (BatterySwapEvent1, BatterySwapEvent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BatterySwapEvent1">A battery swap event.</param>
        /// <param name="BatterySwapEvent2">Another battery swap event.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BatterySwapEvent BatterySwapEvent1,
                                           BatterySwapEvent BatterySwapEvent2)

            => BatterySwapEvent1.CompareTo(BatterySwapEvent2) <= 0;

        #endregion

        #region Operator >  (BatterySwapEvent1, BatterySwapEvent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BatterySwapEvent1">A battery swap event.</param>
        /// <param name="BatterySwapEvent2">Another battery swap event.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BatterySwapEvent BatterySwapEvent1,
                                          BatterySwapEvent BatterySwapEvent2)

            => BatterySwapEvent1.CompareTo(BatterySwapEvent2) > 0;

        #endregion

        #region Operator >= (BatterySwapEvent1, BatterySwapEvent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BatterySwapEvent1">A battery swap event.</param>
        /// <param name="BatterySwapEvent2">Another battery swap event.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BatterySwapEvent BatterySwapEvent1,
                                           BatterySwapEvent BatterySwapEvent2)

            => BatterySwapEvent1.CompareTo(BatterySwapEvent2) >= 0;

        #endregion

        #endregion

        #region IComparable<BatterySwapEvent> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two battery swap events.
        /// </summary>
        /// <param name="Object">A battery swap event to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is BatterySwapEvent batterySwapEvent
                   ? CompareTo(batterySwapEvent)
                   : throw new ArgumentException("The given object is not a battery swap event!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(BatterySwapEvent)

        /// <summary>
        /// Compares two battery swap events.
        /// </summary>
        /// <param name="BatterySwapEvent">A battery swap event to compare with.</param>
        public Int32 CompareTo(BatterySwapEvent BatterySwapEvent)

            => String.Compare(InternalId,
                              BatterySwapEvent.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<BatterySwapEvent> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two battery swap events for equality.
        /// </summary>
        /// <param name="Object">A battery swap event to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BatterySwapEvent batterySwapEvent &&
                   Equals(batterySwapEvent);

        #endregion

        #region Equals(BatterySwapEvent)

        /// <summary>
        /// Compares two battery swap events for equality.
        /// </summary>
        /// <param name="BatterySwapEvent">A battery swap event to compare with.</param>
        public Boolean Equals(BatterySwapEvent BatterySwapEvent)

            => String.Equals(InternalId,
                             BatterySwapEvent.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
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
