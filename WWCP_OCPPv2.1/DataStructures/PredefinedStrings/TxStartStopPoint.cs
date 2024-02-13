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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for transaction start or stop points.
    /// </summary>
    public static class TxStartStopPointExtensions
    {

        /// <summary>
        /// Indicates whether this transaction start or stop point is null or empty.
        /// </summary>
        /// <param name="TxStartStopPoint">A transaction start or stop point.</param>
        public static Boolean IsNullOrEmpty(this TxStartStopPoint? TxStartStopPoint)
            => !TxStartStopPoint.HasValue || TxStartStopPoint.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this transaction start or stop point is null or empty.
        /// </summary>
        /// <param name="TxStartStopPoint">A transaction start or stop point.</param>
        public static Boolean IsNotNullOrEmpty(this TxStartStopPoint? TxStartStopPoint)
            => TxStartStopPoint.HasValue && TxStartStopPoint.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A transaction start or stop point.
    /// </summary>
    public readonly struct TxStartStopPoint : IId,
                                              IEquatable<TxStartStopPoint>,
                                              IComparable<TxStartStopPoint>
    {

        #region Data

        private readonly static Dictionary<String, TxStartStopPoint>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                             InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this transaction start or stop point is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this transaction start or stop point is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the transaction start or stop point.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new transaction start or stop point based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a transaction start or stop point.</param>
        private TxStartStopPoint(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static TxStartStopPoint Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new TxStartStopPoint(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a transaction start or stop point.
        /// </summary>
        /// <param name="Text">A text representation of a transaction start or stop point.</param>
        public static TxStartStopPoint Parse(String Text)
        {

            if (TryParse(Text, out var txStartStopPoint))
                return txStartStopPoint;

            throw new ArgumentException("The given text representation of a transaction start or stop point is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as transaction start or stop point.
        /// </summary>
        /// <param name="Text">A text representation of a transaction start or stop point.</param>
        public static TxStartStopPoint? TryParse(String Text)
        {

            if (TryParse(Text, out var txStartStopPoint))
                return txStartStopPoint;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TxStartStopPoint)

        /// <summary>
        /// Try to parse the given text as transaction start or stop point.
        /// </summary>
        /// <param name="Text">A text representation of a transaction start or stop point.</param>
        /// <param name="TxStartStopPoint">The parsed transaction start or stop point.</param>
        public static Boolean TryParse(String Text, out TxStartStopPoint TxStartStopPoint)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out TxStartStopPoint))
                    TxStartStopPoint = Register(Text);

                return true;

            }

            TxStartStopPoint = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this transaction start or stop point.
        /// </summary>
        public TxStartStopPoint Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// An object (probably an EV) is detected in the parking/charging bay.
        /// </summary>
        public static TxStartStopPoint ParkingBayOccupancy { get; }
            = Register("ParkingBayOccupancy");

        /// <summary>
        /// Both ends of the Charging Cable are connected (if this can be detected, otherwise detection of a cable being plugged in to the socket), or for wireless: initial communication between EVSE and EV is established.
        /// </summary>
        public static TxStartStopPoint EVConnected { get; }
            = Register("EVConnected");

        /// <summary>
        /// Driver or EV is authorized, this can also be some form of anonymous authorization like a start button.
        /// </summary>
        public static TxStartStopPoint Authorized { get; }
            = Register("Authorized");

        /// <summary>
        /// Signed data is received from the energy meter which is required by some legislation.There are countries that require signed metering data before a billable transaction can be started.
        /// </summary>
        public static TxStartStopPoint DataSigned { get; }
            = Register("DataSigned");

        /// <summary>
        /// All preconditions are met, power can flow. In case of a wired charger, the cable is properly connected, driver is authorized,
        /// power relay is closed etc.This does not mean that the EV is read to charge it’s battery, it might, for example, be to warm.
        /// </summary>
        public static TxStartStopPoint PowerPathClosed { get; }
            = Register("PowerPathClosed");

        /// <summary>
        /// Energy is being transferred between EV and EVSE.
        /// </summary>
        public static TxStartStopPoint EnergyTransfer { get; }
            = Register("EnergyTransfer");

        #endregion


        #region Operator overloading

        #region Operator == (TxStartStopPoint1, TxStartStopPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStartStopPoint1">A transaction start or stop point.</param>
        /// <param name="TxStartStopPoint2">Another transaction start or stop point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TxStartStopPoint TxStartStopPoint1,
                                           TxStartStopPoint TxStartStopPoint2)

            => TxStartStopPoint1.Equals(TxStartStopPoint2);

        #endregion

        #region Operator != (TxStartStopPoint1, TxStartStopPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStartStopPoint1">A transaction start or stop point.</param>
        /// <param name="TxStartStopPoint2">Another transaction start or stop point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TxStartStopPoint TxStartStopPoint1,
                                           TxStartStopPoint TxStartStopPoint2)

            => !TxStartStopPoint1.Equals(TxStartStopPoint2);

        #endregion

        #region Operator <  (TxStartStopPoint1, TxStartStopPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStartStopPoint1">A transaction start or stop point.</param>
        /// <param name="TxStartStopPoint2">Another transaction start or stop point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TxStartStopPoint TxStartStopPoint1,
                                          TxStartStopPoint TxStartStopPoint2)

            => TxStartStopPoint1.CompareTo(TxStartStopPoint2) < 0;

        #endregion

        #region Operator <= (TxStartStopPoint1, TxStartStopPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStartStopPoint1">A transaction start or stop point.</param>
        /// <param name="TxStartStopPoint2">Another transaction start or stop point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TxStartStopPoint TxStartStopPoint1,
                                           TxStartStopPoint TxStartStopPoint2)

            => TxStartStopPoint1.CompareTo(TxStartStopPoint2) <= 0;

        #endregion

        #region Operator >  (TxStartStopPoint1, TxStartStopPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStartStopPoint1">A transaction start or stop point.</param>
        /// <param name="TxStartStopPoint2">Another transaction start or stop point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TxStartStopPoint TxStartStopPoint1,
                                          TxStartStopPoint TxStartStopPoint2)

            => TxStartStopPoint1.CompareTo(TxStartStopPoint2) > 0;

        #endregion

        #region Operator >= (TxStartStopPoint1, TxStartStopPoint2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TxStartStopPoint1">A transaction start or stop point.</param>
        /// <param name="TxStartStopPoint2">Another transaction start or stop point.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TxStartStopPoint TxStartStopPoint1,
                                           TxStartStopPoint TxStartStopPoint2)

            => TxStartStopPoint1.CompareTo(TxStartStopPoint2) >= 0;

        #endregion

        #endregion

        #region IComparable<TxStartStopPoint> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two transaction start or stop points.
        /// </summary>
        /// <param name="Object">A transaction start or stop point to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TxStartStopPoint txStartStopPoint
                   ? CompareTo(txStartStopPoint)
                   : throw new ArgumentException("The given object is not transaction start or stop point!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TxStartStopPoint)

        /// <summary>
        /// Compares two transaction start or stop points.
        /// </summary>
        /// <param name="TxStartStopPoint">A transaction start or stop point to compare with.</param>
        public Int32 CompareTo(TxStartStopPoint TxStartStopPoint)

            => String.Compare(InternalId,
                              TxStartStopPoint.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TxStartStopPoint> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two transaction start or stop points for equality.
        /// </summary>
        /// <param name="Object">A transaction start or stop point to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TxStartStopPoint txStartStopPoint &&
                   Equals(txStartStopPoint);

        #endregion

        #region Equals(TxStartStopPoint)

        /// <summary>
        /// Compares two transaction start or stop points for equality.
        /// </summary>
        /// <param name="TxStartStopPoint">A transaction start or stop point to compare with.</param>
        public Boolean Equals(TxStartStopPoint TxStartStopPoint)

            => String.Equals(InternalId,
                             TxStartStopPoint.InternalId,
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
