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
    /// Extension methods for transaction stop points.
    /// </summary>
    public static class TxStopPointExtensions
    {

        /// <summary>
        /// Indicates whether this transaction stop point is null or empty.
        /// </summary>
        /// <param name="TxStopPoint">A transaction stop point.</param>
        public static Boolean IsNullOrEmpty(this TxStopPoint? TxStopPoint)
            => !TxStopPoint.HasValue || TxStopPoint.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this transaction stop point is null or empty.
        /// </summary>
        /// <param name="TxStopPoint">A transaction stop point.</param>
        public static Boolean IsNotNullOrEmpty(this TxStopPoint? TxStopPoint)
            => TxStopPoint.HasValue && TxStopPoint.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A transaction stop point.
    /// </summary>
    public readonly struct TxStopPoint : IId,
                                              IEquatable<TxStopPoint>,
                                              IComparable<TxStopPoint>
    {

        #region Data

        private readonly static Dictionary<String, TxStopPoint>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                             InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this transaction stop point is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this transaction stop point is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the transaction stop point.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new transaction stop point based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a transaction stop point.</param>
        private TxStopPoint(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static TxStopPoint Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new TxStopPoint(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a transaction stop point.
        /// </summary>
        /// <param name="Text">A text representation of a transaction stop point.</param>
        public static TxStopPoint Parse(String Text)
        {

            if (TryParse(Text, out var txStartStopPoint))
                return txStartStopPoint;

            throw new ArgumentException("The given text representation of a transaction stop point is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as transaction stop point.
        /// </summary>
        /// <param name="Text">A text representation of a transaction stop point.</param>
        public static TxStopPoint? TryParse(String Text)
        {

            if (TryParse(Text, out var txStartStopPoint))
                return txStartStopPoint;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TxStopPoint)

        /// <summary>
        /// Try to parse the given text as transaction stop point.
        /// </summary>
        /// <param name="Text">A text representation of a transaction stop point.</param>
        /// <param name="TxStopPoint">The parsed transaction stop point.</param>
        public static Boolean TryParse(String Text, out TxStopPoint TxStopPoint)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out TxStopPoint))
                    TxStopPoint = Register(Text);

                return true;

            }

            TxStopPoint = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this transaction stop point.
        /// </summary>
        public TxStopPoint Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// An object (probably an EV) is detected in the parking/charging bay.
        /// </summary>
        public static TxStopPoint ParkingBayOccupancy { get; }
            = Register("ParkingBayOccupancy");

        /// <summary>
        /// Both ends of the Charging Cable are connected (if this can be detected, otherwise detection of a cable being plugged in to the socket), or for wireless: initial communication between EVSE and EV is established.
        /// </summary>
        public static TxStopPoint EVConnected { get; }
            = Register("EVConnected");

        /// <summary>
        /// Driver or EV is authorized, this can also be some form of anonymous authorization like a start button.
        /// </summary>
        public static TxStopPoint Authorized { get; }
            = Register("Authorized");

        /// <summary>
        /// All preconditions are met, power can flow. In case of a wired charger, the cable is properly connected, driver is authorized,
        /// power relay is closed etc. This does not mean that the EV is read to charge it’s battery, it might, for example, be to warm.
        /// </summary>
        public static TxStopPoint PowerPathClosed { get; }
            = Register("PowerPathClosed");

        /// <summary>
        /// Energy is being transferred between EV and EVSE.
        /// </summary>
        public static TxStopPoint EnergyTransfer { get; }
            = Register("EnergyTransfer");

        /// <summary>
        /// Signed data is received from the energy meter which is required by some legislation.There are countries that require signed metering data before a billable transaction can be started.
        /// </summary>
        public static TxStopPoint DataSigned { get; }
            = Register("DataSigned");

        #endregion


        #region Operator overloading

        #region Operator == (TxStopPoint1, TxStopPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStopPoint1">A transaction stop point.</param>
        /// <param name="TxStopPoint2">Another transaction stop point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TxStopPoint TxStopPoint1,
                                           TxStopPoint TxStopPoint2)

            => TxStopPoint1.Equals(TxStopPoint2);

        #endregion

        #region Operator != (TxStopPoint1, TxStopPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStopPoint1">A transaction stop point.</param>
        /// <param name="TxStopPoint2">Another transaction stop point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TxStopPoint TxStopPoint1,
                                           TxStopPoint TxStopPoint2)

            => !TxStopPoint1.Equals(TxStopPoint2);

        #endregion

        #region Operator <  (TxStopPoint1, TxStopPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStopPoint1">A transaction stop point.</param>
        /// <param name="TxStopPoint2">Another transaction stop point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TxStopPoint TxStopPoint1,
                                          TxStopPoint TxStopPoint2)

            => TxStopPoint1.CompareTo(TxStopPoint2) < 0;

        #endregion

        #region Operator <= (TxStopPoint1, TxStopPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStopPoint1">A transaction stop point.</param>
        /// <param name="TxStopPoint2">Another transaction stop point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TxStopPoint TxStopPoint1,
                                           TxStopPoint TxStopPoint2)

            => TxStopPoint1.CompareTo(TxStopPoint2) <= 0;

        #endregion

        #region Operator >  (TxStopPoint1, TxStopPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStopPoint1">A transaction stop point.</param>
        /// <param name="TxStopPoint2">Another transaction stop point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TxStopPoint TxStopPoint1,
                                          TxStopPoint TxStopPoint2)

            => TxStopPoint1.CompareTo(TxStopPoint2) > 0;

        #endregion

        #region Operator >= (TxStopPoint1, TxStopPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStopPoint1">A transaction stop point.</param>
        /// <param name="TxStopPoint2">Another transaction stop point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TxStopPoint TxStopPoint1,
                                           TxStopPoint TxStopPoint2)

            => TxStopPoint1.CompareTo(TxStopPoint2) >= 0;

        #endregion

        #endregion

        #region IComparable<TxStopPoint> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two transaction stop points.
        /// </summary>
        /// <param name="Object">A transaction stop point to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TxStopPoint txStartStopPoint
                   ? CompareTo(txStartStopPoint)
                   : throw new ArgumentException("The given object is not transaction stop point!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TxStopPoint)

        /// <summary>
        /// Compares two transaction stop points.
        /// </summary>
        /// <param name="TxStopPoint">A transaction stop point to compare with.</param>
        public Int32 CompareTo(TxStopPoint TxStopPoint)

            => String.Compare(InternalId,
                              TxStopPoint.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TxStopPoint> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two transaction stop points for equality.
        /// </summary>
        /// <param name="Object">A transaction stop point to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TxStopPoint txStartStopPoint &&
                   Equals(txStartStopPoint);

        #endregion

        #region Equals(TxStopPoint)

        /// <summary>
        /// Compares two transaction stop points for equality.
        /// </summary>
        /// <param name="TxStopPoint">A transaction stop point to compare with.</param>
        public Boolean Equals(TxStopPoint TxStopPoint)

            => String.Equals(InternalId,
                             TxStopPoint.InternalId,
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
