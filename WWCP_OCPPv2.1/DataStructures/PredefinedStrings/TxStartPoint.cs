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
    /// Extension methods for transaction start points.
    /// </summary>
    public static class TxStartPointExtensions
    {

        /// <summary>
        /// Indicates whether this transaction start point is null or empty.
        /// </summary>
        /// <param name="TxStartPoint">A transaction start point.</param>
        public static Boolean IsNullOrEmpty(this TxStartPoint? TxStartPoint)
            => !TxStartPoint.HasValue || TxStartPoint.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this transaction start point is null or empty.
        /// </summary>
        /// <param name="TxStartPoint">A transaction start point.</param>
        public static Boolean IsNotNullOrEmpty(this TxStartPoint? TxStartPoint)
            => TxStartPoint.HasValue && TxStartPoint.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A transaction start point.
    /// </summary>
    public readonly struct TxStartPoint : IId,
                                          IEquatable<TxStartPoint>,
                                          IComparable<TxStartPoint>
    {

        #region Data

        private readonly static Dictionary<String, TxStartPoint>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                             InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this transaction start point is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this transaction start point is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the transaction start point.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new transaction start point based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a transaction start point.</param>
        private TxStartPoint(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static TxStartPoint Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new TxStartPoint(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a transaction start point.
        /// </summary>
        /// <param name="Text">A text representation of a transaction start point.</param>
        public static TxStartPoint Parse(String Text)
        {

            if (TryParse(Text, out var txStartStopPoint))
                return txStartStopPoint;

            throw new ArgumentException("The given text representation of a transaction start point is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as transaction start point.
        /// </summary>
        /// <param name="Text">A text representation of a transaction start point.</param>
        public static TxStartPoint? TryParse(String Text)
        {

            if (TryParse(Text, out var txStartStopPoint))
                return txStartStopPoint;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TxStartPoint)

        /// <summary>
        /// Try to parse the given text as transaction start point.
        /// </summary>
        /// <param name="Text">A text representation of a transaction start point.</param>
        /// <param name="TxStartPoint">The parsed transaction start point.</param>
        public static Boolean TryParse(String Text, out TxStartPoint TxStartPoint)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out TxStartPoint))
                    TxStartPoint = Register(Text);

                return true;

            }

            TxStartPoint = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this transaction start point.
        /// </summary>
        public TxStartPoint Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// An object (probably an EV) is detected in the parking/charging bay.
        /// </summary>
        public static TxStartPoint ParkingBayOccupancy { get; }
            = Register("ParkingBayOccupancy");

        /// <summary>
        /// Both ends of the Charging Cable are connected (if this can be detected, otherwise detection of a cable being plugged in to the socket), or for wireless: initial communication between EVSE and EV is established.
        /// </summary>
        public static TxStartPoint EVConnected { get; }
            = Register("EVConnected");

        /// <summary>
        /// Driver or EV is authorized, this can also be some form of anonymous authorization like a start button.
        /// </summary>
        public static TxStartPoint Authorized { get; }
            = Register("Authorized");

        /// <summary>
        /// All preconditions are met, power can flow. In case of a wired charger, the cable is properly connected, driver is authorized,
        /// power relay is closed etc. This does not mean that the EV is read to charge it’s battery, it might, for example, be to warm.
        /// </summary>
        public static TxStartPoint PowerPathClosed { get; }
            = Register("PowerPathClosed");

        /// <summary>
        /// Energy is being transferred between EV and EVSE.
        /// </summary>
        public static TxStartPoint EnergyTransfer { get; }
            = Register("EnergyTransfer");

        /// <summary>
        /// Signed data is received from the energy meter which is required by some legislation.
        /// There are countries that require signed metering data before a billable transaction can be started.
        /// </summary>
        public static TxStartPoint DataSigned { get; }
            = Register("DataSigned");

        #endregion


        #region Operator overloading

        #region Operator == (TxStartPoint1, TxStartPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStartPoint1">A transaction start point.</param>
        /// <param name="TxStartPoint2">Another transaction start point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TxStartPoint TxStartPoint1,
                                           TxStartPoint TxStartPoint2)

            => TxStartPoint1.Equals(TxStartPoint2);

        #endregion

        #region Operator != (TxStartPoint1, TxStartPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStartPoint1">A transaction start point.</param>
        /// <param name="TxStartPoint2">Another transaction start point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TxStartPoint TxStartPoint1,
                                           TxStartPoint TxStartPoint2)

            => !TxStartPoint1.Equals(TxStartPoint2);

        #endregion

        #region Operator <  (TxStartPoint1, TxStartPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStartPoint1">A transaction start point.</param>
        /// <param name="TxStartPoint2">Another transaction start point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TxStartPoint TxStartPoint1,
                                          TxStartPoint TxStartPoint2)

            => TxStartPoint1.CompareTo(TxStartPoint2) < 0;

        #endregion

        #region Operator <= (TxStartPoint1, TxStartPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStartPoint1">A transaction start point.</param>
        /// <param name="TxStartPoint2">Another transaction start point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TxStartPoint TxStartPoint1,
                                           TxStartPoint TxStartPoint2)

            => TxStartPoint1.CompareTo(TxStartPoint2) <= 0;

        #endregion

        #region Operator >  (TxStartPoint1, TxStartPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStartPoint1">A transaction start point.</param>
        /// <param name="TxStartPoint2">Another transaction start point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TxStartPoint TxStartPoint1,
                                          TxStartPoint TxStartPoint2)

            => TxStartPoint1.CompareTo(TxStartPoint2) > 0;

        #endregion

        #region Operator >= (TxStartPoint1, TxStartPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStartPoint1">A transaction start point.</param>
        /// <param name="TxStartPoint2">Another transaction start point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TxStartPoint TxStartPoint1,
                                           TxStartPoint TxStartPoint2)

            => TxStartPoint1.CompareTo(TxStartPoint2) >= 0;

        #endregion

        #endregion

        #region IComparable<TxStartPoint> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two transaction start points.
        /// </summary>
        /// <param name="Object">A transaction start point to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TxStartPoint txStartStopPoint
                   ? CompareTo(txStartStopPoint)
                   : throw new ArgumentException("The given object is not transaction start point!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TxStartPoint)

        /// <summary>
        /// Compares two transaction start points.
        /// </summary>
        /// <param name="TxStartPoint">A transaction start point to compare with.</param>
        public Int32 CompareTo(TxStartPoint TxStartPoint)

            => String.Compare(InternalId,
                              TxStartPoint.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TxStartPoint> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two transaction start points for equality.
        /// </summary>
        /// <param name="Object">A transaction start point to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TxStartPoint txStartStopPoint &&
                   Equals(txStartStopPoint);

        #endregion

        #region Equals(TxStartPoint)

        /// <summary>
        /// Compares two transaction start points for equality.
        /// </summary>
        /// <param name="TxStartPoint">A transaction start point to compare with.</param>
        public Boolean Equals(TxStartPoint TxStartPoint)

            => String.Equals(InternalId,
                             TxStartPoint.InternalId,
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
