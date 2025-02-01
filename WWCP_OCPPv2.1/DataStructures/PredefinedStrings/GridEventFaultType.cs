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
    /// Extension methods for grid event fault types.
    /// </summary>
    public static class GridEventFaultTypeExtensions
    {

        /// <summary>
        /// Indicates whether this grid event fault type is null or empty.
        /// </summary>
        /// <param name="GridEventFaultType">A grid event fault type.</param>
        public static Boolean IsNullOrEmpty(this GridEventFaultType? GridEventFaultType)
            => !GridEventFaultType.HasValue || GridEventFaultType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this grid event fault type is null or empty.
        /// </summary>
        /// <param name="GridEventFaultType">A grid event fault type.</param>
        public static Boolean IsNotNullOrEmpty(this GridEventFaultType? GridEventFaultType)
            => GridEventFaultType.HasValue && GridEventFaultType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A grid event fault type.
    /// </summary>
    public readonly struct GridEventFaultType : IId,
                                                IEquatable<GridEventFaultType>,
                                                IComparable<GridEventFaultType>
    {

        #region Data

        private readonly static Dictionary<String, GridEventFaultType>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                      InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this grid event fault type is null or empty.
        /// </summary>
        public readonly  Boolean                      IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this grid event fault type is NOT null or empty.
        /// </summary>
        public readonly  Boolean                      IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the grid event fault type.
        /// </summary>
        public readonly  UInt64                       Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered grid event fault types.
        /// </summary>
        public static    IEnumerable<GridEventFaultType>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new grid event fault type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a grid event fault type.</param>
        private GridEventFaultType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static GridEventFaultType Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new GridEventFaultType(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a grid event fault type.
        /// </summary>
        /// <param name="Text">A text representation of a grid event fault type.</param>
        public static GridEventFaultType Parse(String Text)
        {

            if (TryParse(Text, out var gridEventFaultType))
                return gridEventFaultType;

            throw new ArgumentException($"Invalid text representation of a grid event fault type: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a grid event fault type.
        /// </summary>
        /// <param name="Text">A text representation of a grid event fault type.</param>
        public static GridEventFaultType? TryParse(String Text)
        {

            if (TryParse(Text, out var gridEventFaultType))
                return gridEventFaultType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out GridEventFaultType)

        /// <summary>
        /// Try to parse the given text as a grid event fault type.
        /// </summary>
        /// <param name="Text">A text representation of a grid event fault type.</param>
        /// <param name="GridEventFaultType">The parsed grid event fault type.</param>
        public static Boolean TryParse(String Text, out GridEventFaultType GridEventFaultType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out GridEventFaultType))
                    GridEventFaultType = Register(Text);

                return true;

            }

            GridEventFaultType = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this grid event fault type.
        /// </summary>
        public GridEventFaultType Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// CurrentImbalance
        /// </summary>
        public static GridEventFaultType  CurrentImbalance    { get; }
            = Register("CurrentImbalance");

        /// <summary>
        /// LocalEmergency
        /// </summary>
        public static GridEventFaultType  LocalEmergency      { get; }
            = Register("LocalEmergency");

        /// <summary>
        /// LowInputPower
        /// </summary>
        public static GridEventFaultType  LowInputPower       { get; }
            = Register("LowInputPower");

        /// <summary>
        /// OverCurrent
        /// </summary>
        public static GridEventFaultType  OverCurrent         { get; }
            = Register("OverCurrent");

        /// <summary>
        /// OverFrequency
        /// </summary>
        public static GridEventFaultType  OverFrequency       { get; }
            = Register("OverFrequency");

        /// <summary>
        /// OverVoltage
        /// </summary>
        public static GridEventFaultType  OverVoltage         { get; }
            = Register("OverVoltage");

        /// <summary>
        /// PhaseRotation
        /// </summary>
        public static GridEventFaultType  PhaseRotation       { get; }
            = Register("PhaseRotation");

        /// <summary>
        /// RemoteEmergency
        /// </summary>
        public static GridEventFaultType  RemoteEmergency     { get; }
            = Register("RemoteEmergency");

        /// <summary>
        /// UnderFrequency
        /// </summary>
        public static GridEventFaultType  UnderFrequency      { get; }
            = Register("UnderFrequency");

        /// <summary>
        /// UnderVoltage
        /// </summary>
        public static GridEventFaultType  UnderVoltage        { get; }
            = Register("UnderVoltage");

        /// <summary>
        /// VoltageImbalance
        /// </summary>
        public static GridEventFaultType  VoltageImbalance    { get; }
            = Register("VoltageImbalance");

        #endregion


        #region Operator overloading

        #region Operator == (GridEventFaultType1, GridEventFaultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GridEventFaultType1">A grid event fault type.</param>
        /// <param name="GridEventFaultType2">Another grid event fault type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (GridEventFaultType GridEventFaultType1,
                                           GridEventFaultType GridEventFaultType2)

            => GridEventFaultType1.Equals(GridEventFaultType2);

        #endregion

        #region Operator != (GridEventFaultType1, GridEventFaultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GridEventFaultType1">A grid event fault type.</param>
        /// <param name="GridEventFaultType2">Another grid event fault type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (GridEventFaultType GridEventFaultType1,
                                           GridEventFaultType GridEventFaultType2)

            => !GridEventFaultType1.Equals(GridEventFaultType2);

        #endregion

        #region Operator <  (GridEventFaultType1, GridEventFaultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GridEventFaultType1">A grid event fault type.</param>
        /// <param name="GridEventFaultType2">Another grid event fault type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (GridEventFaultType GridEventFaultType1,
                                          GridEventFaultType GridEventFaultType2)

            => GridEventFaultType1.CompareTo(GridEventFaultType2) < 0;

        #endregion

        #region Operator <= (GridEventFaultType1, GridEventFaultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GridEventFaultType1">A grid event fault type.</param>
        /// <param name="GridEventFaultType2">Another grid event fault type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (GridEventFaultType GridEventFaultType1,
                                           GridEventFaultType GridEventFaultType2)

            => GridEventFaultType1.CompareTo(GridEventFaultType2) <= 0;

        #endregion

        #region Operator >  (GridEventFaultType1, GridEventFaultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GridEventFaultType1">A grid event fault type.</param>
        /// <param name="GridEventFaultType2">Another grid event fault type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (GridEventFaultType GridEventFaultType1,
                                          GridEventFaultType GridEventFaultType2)

            => GridEventFaultType1.CompareTo(GridEventFaultType2) > 0;

        #endregion

        #region Operator >= (GridEventFaultType1, GridEventFaultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GridEventFaultType1">A grid event fault type.</param>
        /// <param name="GridEventFaultType2">Another grid event fault type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (GridEventFaultType GridEventFaultType1,
                                           GridEventFaultType GridEventFaultType2)

            => GridEventFaultType1.CompareTo(GridEventFaultType2) >= 0;

        #endregion

        #endregion

        #region IComparable<GridEventFaultType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two grid event fault types.
        /// </summary>
        /// <param name="Object">A grid event fault type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is GridEventFaultType gridEventFaultType
                   ? CompareTo(gridEventFaultType)
                   : throw new ArgumentException("The given object is not a grid event fault type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(GridEventFaultType)

        /// <summary>
        /// Compares two grid event fault types.
        /// </summary>
        /// <param name="GridEventFaultType">A grid event fault type to compare with.</param>
        public Int32 CompareTo(GridEventFaultType GridEventFaultType)

            => String.Compare(InternalId,
                              GridEventFaultType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<GridEventFaultType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two grid event fault types for equality.
        /// </summary>
        /// <param name="Object">A grid event fault type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GridEventFaultType gridEventFaultType &&
                   Equals(gridEventFaultType);

        #endregion

        #region Equals(GridEventFaultType)

        /// <summary>
        /// Compares two grid event fault types for equality.
        /// </summary>
        /// <param name="GridEventFaultType">A grid event fault type to compare with.</param>
        public Boolean Equals(GridEventFaultType GridEventFaultType)

            => String.Equals(InternalId,
                             GridEventFaultType.InternalId,
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
