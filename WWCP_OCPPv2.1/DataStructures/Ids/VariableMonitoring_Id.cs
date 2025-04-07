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
    /// Extension methods for variable monitoring identifications.
    /// </summary>
    public static class VariableMonitoringIdExtensions
    {

        /// <summary>
        /// Indicates whether this variable monitoring identification is null or empty.
        /// </summary>
        /// <param name="VariableMonitoringId">A variable monitoring identification.</param>
        public static Boolean IsNullOrEmpty(this VariableMonitoring_Id? VariableMonitoringId)
            => !VariableMonitoringId.HasValue || VariableMonitoringId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this variable monitoring identification is null or empty.
        /// </summary>
        /// <param name="VariableMonitoringId">A variable monitoring identification.</param>
        public static Boolean IsNotNullOrEmpty(this VariableMonitoring_Id? VariableMonitoringId)
            => VariableMonitoringId.HasValue && VariableMonitoringId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A variable monitoring identification.
    /// </summary>
    public readonly struct VariableMonitoring_Id : IId,
                                                   IEquatable<VariableMonitoring_Id>,
                                                   IComparable<VariableMonitoring_Id>
    {

        #region Data

        /// <summary>
        /// The numeric value of the transaction identification.
        /// </summary>
        public readonly UInt64 Value;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => Value == 0;

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => Value != 0;

        /// <summary>
        /// The length of the variable monitoring identification.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) Value.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new variable monitoring identification based on the given number.
        /// </summary>
        /// <param name="Number">A numeric representation of a transaction identification.</param>
        private VariableMonitoring_Id(UInt64 Number)
        {
            this.Value = Number;
        }

        #endregion


        #region (static) NewRandom

        /// <summary>
        /// Create a new random variable monitoring identification.
        /// </summary>
        public static VariableMonitoring_Id NewRandom

#pragma warning disable SCS0005 // Weak random number generator.
            => new ((UInt64) Random.Shared.Next(Int32.MaxValue));
#pragma warning restore SCS0005 // Weak random number generator.

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a variable monitoring identification.
        /// </summary>
        /// <param name="Text">A text representation of a variable monitoring identification.</param>
        public static VariableMonitoring_Id Parse(String Text)
        {

            if (TryParse(Text, out var variableMonitoringId))
                return variableMonitoringId;

            throw new ArgumentException($"Invalid text representation of a variable monitoring identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Number)

        /// <summary>
        /// Parse the given number as a variable monitoring identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a variable monitoring identification.</param>
        public static VariableMonitoring_Id Parse(UInt64 Number)

            => new (Number);

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a variable monitoring identification.
        /// </summary>
        /// <param name="Text">A text representation of a variable monitoring identification.</param>
        public static VariableMonitoring_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var variableMonitoringId))
                return variableMonitoringId;

            return null;

        }

        #endregion

        #region (static) TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a variable monitoring identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a variable monitoring identification.</param>
        public static VariableMonitoring_Id? TryParse(UInt64 Number)
        {

            if (TryParse(Number, out var variableMonitoringId))
                return variableMonitoringId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,   out VariableMonitoringId)

        /// <summary>
        /// Try to parse the given text as a variable monitoring identification.
        /// </summary>
        /// <param name="Text">A text representation of a variable monitoring identification.</param>
        /// <param name="VariableMonitoringId">The parsed variable monitoring identification.</param>
        public static Boolean TryParse(String Text, out VariableMonitoring_Id VariableMonitoringId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                UInt64.TryParse(Text, out var number))
            {
                VariableMonitoringId = new VariableMonitoring_Id(number);
                return true;
            }

            VariableMonitoringId = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Number, out VariableMonitoringId)

        /// <summary>
        /// Try to parse the given number as a variable monitoring identification.
        /// </summary>
        /// <param name="Number">A numeric representation of a variable monitoring identification.</param>
        /// <param name="VariableMonitoringId">The parsed variable monitoring identification.</param>
        public static Boolean TryParse(UInt64 Number, out VariableMonitoring_Id VariableMonitoringId)
        {

            VariableMonitoringId = new VariableMonitoring_Id(Number);

            return true;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this variable monitoring identification.
        /// </summary>
        public VariableMonitoring_Id Clone()

            => new (Value);

        #endregion


        #region Operator overloading

        #region Operator == (VariableMonitoringId1, VariableMonitoringId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableMonitoringId1">A variable monitoring identification.</param>
        /// <param name="VariableMonitoringId2">Another variable monitoring identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VariableMonitoring_Id VariableMonitoringId1,
                                           VariableMonitoring_Id VariableMonitoringId2)

            => VariableMonitoringId1.Equals(VariableMonitoringId2);

        #endregion

        #region Operator != (VariableMonitoringId1, VariableMonitoringId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableMonitoringId1">A variable monitoring identification.</param>
        /// <param name="VariableMonitoringId2">Another variable monitoring identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VariableMonitoring_Id VariableMonitoringId1,
                                           VariableMonitoring_Id VariableMonitoringId2)

            => !VariableMonitoringId1.Equals(VariableMonitoringId2);

        #endregion

        #region Operator <  (VariableMonitoringId1, VariableMonitoringId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableMonitoringId1">A variable monitoring identification.</param>
        /// <param name="VariableMonitoringId2">Another variable monitoring identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (VariableMonitoring_Id VariableMonitoringId1,
                                          VariableMonitoring_Id VariableMonitoringId2)

            => VariableMonitoringId1.CompareTo(VariableMonitoringId2) < 0;

        #endregion

        #region Operator <= (VariableMonitoringId1, VariableMonitoringId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableMonitoringId1">A variable monitoring identification.</param>
        /// <param name="VariableMonitoringId2">Another variable monitoring identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (VariableMonitoring_Id VariableMonitoringId1,
                                           VariableMonitoring_Id VariableMonitoringId2)

            => VariableMonitoringId1.CompareTo(VariableMonitoringId2) <= 0;

        #endregion

        #region Operator >  (VariableMonitoringId1, VariableMonitoringId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableMonitoringId1">A variable monitoring identification.</param>
        /// <param name="VariableMonitoringId2">Another variable monitoring identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (VariableMonitoring_Id VariableMonitoringId1,
                                          VariableMonitoring_Id VariableMonitoringId2)

            => VariableMonitoringId1.CompareTo(VariableMonitoringId2) > 0;

        #endregion

        #region Operator >= (VariableMonitoringId1, VariableMonitoringId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableMonitoringId1">A variable monitoring identification.</param>
        /// <param name="VariableMonitoringId2">Another variable monitoring identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (VariableMonitoring_Id VariableMonitoringId1,
                                           VariableMonitoring_Id VariableMonitoringId2)

            => VariableMonitoringId1.CompareTo(VariableMonitoringId2) >= 0;

        #endregion

        #endregion

        #region IComparable<VariableMonitoringId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two variable monitoring identifications.
        /// </summary>
        /// <param name="Object">A variable monitoring identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is VariableMonitoring_Id variableMonitoringId
                   ? CompareTo(variableMonitoringId)
                   : throw new ArgumentException("The given object is not a variable monitoring identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(VariableMonitoringId)

        /// <summary>
        /// Compares two variable monitoring identifications.
        /// </summary>
        /// <param name="VariableMonitoringId">A variable monitoring identification to compare with.</param>
        public Int32 CompareTo(VariableMonitoring_Id VariableMonitoringId)

            => Value.CompareTo(VariableMonitoringId.Value);

        #endregion

        #endregion

        #region IEquatable<VariableMonitoringId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two variable monitoring identifications for equality.
        /// </summary>
        /// <param name="Object">A variable monitoring identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VariableMonitoring_Id variableMonitoringId &&
                   Equals(variableMonitoringId);

        #endregion

        #region Equals(VariableMonitoringId)

        /// <summary>
        /// Compares two variable monitoring identifications for equality.
        /// </summary>
        /// <param name="VariableMonitoringId">A variable monitoring identification to compare with.</param>
        public Boolean Equals(VariableMonitoring_Id VariableMonitoringId)

            => Value.Equals(VariableMonitoringId.Value);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
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
