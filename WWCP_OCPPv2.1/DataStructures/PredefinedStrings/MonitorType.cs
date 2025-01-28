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
    /// Extension methods for monitor types.
    /// </summary>
    public static class MonitorTypeExtensions
    {

        /// <summary>
        /// Indicates whether this monitor type is null or empty.
        /// </summary>
        /// <param name="MonitorType">A monitor type.</param>
        public static Boolean IsNullOrEmpty(this MonitorType? MonitorType)
            => !MonitorType.HasValue || MonitorType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this monitor type is null or empty.
        /// </summary>
        /// <param name="MonitorType">A monitor type.</param>
        public static Boolean IsNotNullOrEmpty(this MonitorType? MonitorType)
            => MonitorType.HasValue && MonitorType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A monitor type.
    /// </summary>
    public readonly struct MonitorType : IId,
                                         IEquatable<MonitorType>,
                                         IComparable<MonitorType>
    {

        #region Data

        private readonly static Dictionary<String, MonitorType>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                           InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this monitor type is null or empty.
        /// </summary>
        public readonly  Boolean                IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this monitor type is NOT null or empty.
        /// </summary>
        public readonly  Boolean                IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the monitor type.
        /// </summary>
        public readonly  UInt64                 Length
            => (UInt64) (InternalId?.Length ?? 0);

        /// <summary>
        /// All registered monitor types.
        /// </summary>
        public static IEnumerable<MonitorType>  All
            => lookup.Values;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new monitor type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a monitor type.</param>
        private MonitorType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static MonitorType Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new MonitorType(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a monitor type.
        /// </summary>
        /// <param name="Text">A text representation of a monitor type.</param>
        public static MonitorType Parse(String Text)
        {

            if (TryParse(Text, out var monitorType))
                return monitorType;

            throw new ArgumentException($"Invalid text representation of a monitor type: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a monitor type.
        /// </summary>
        /// <param name="Text">A text representation of a monitor type.</param>
        public static MonitorType? TryParse(String Text)
        {

            if (TryParse(Text, out var monitorType))
                return monitorType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out MonitorType)

        /// <summary>
        /// Try to parse the given text as a monitor type.
        /// </summary>
        /// <param name="Text">A text representation of a monitor type.</param>
        /// <param name="MonitorType">The parsed monitor type.</param>
        public static Boolean TryParse(String Text, out MonitorType MonitorType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out MonitorType))
                    MonitorType = Register(Text);

                return true;

            }

            MonitorType = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this monitor type.
        /// </summary>
        public MonitorType Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Triggers an event notice when the actual value of the variable rises above the "monitorValue".
        /// </summary>
        public static MonitorType UpperThreshold          { get; }
            = Register("UpperThreshold");

        /// <summary>
        /// Triggers an event notice when the actual value of the variable drops below "monitorValue".
        /// </summary>
        public static MonitorType LowerThreshold          { get; }
            = Register("LowerThreshold");

        /// <summary>
        /// Triggers an event notice when the actual value has changed more than plus or minus "monitorValue" since the
        /// time that this monitor was set or since the last time this event notice was sent, whichever was last.
        /// For variables that are not numeric, like boolean, string or enumerations, a monitor of type Delta will
        /// trigger an event notice whenever the variable changes, regardless of the value of monitorValue.
        /// </summary>
        public static MonitorType Delta                   { get; }
            = Register("Delta");

        /// <summary>
        /// Triggers an event notice every "monitorValue" seconds interval, starting from the time that this monitor was set.
        /// </summary>
        public static MonitorType Periodic                { get; }
            = Register("Periodic");

        /// <summary>
        /// Triggers an event notice every "monitorValue" seconds interval, starting from the nearest clock-aligned interval
        /// after this monitor was set.
        /// </summary>
        /// <example>A monitorValue of 900 will trigger event notices at 0, 15, 30 and 45 minutes after the hour, every hour.</example>
        public static MonitorType PeriodicClockAligned    { get; }
            = Register("PeriodicClockAligned");

        /// <summary>
        /// Triggers an event notice when the actual value differs from the target value more than plus
        /// or minus monitorValue since the time that this monitor was set or since the last time this
        /// event notice was sent, whichever was last. Behavior of this type of monitor for a variable that
        /// is not numeric, is not defined.
        /// </summary>
        /// <example>When target = 100, monitorValue = 10, then an event is triggered when actual &lt; 90 or actual &gt; 110.</example>
        public static MonitorType TargetDelta             { get; }
            = Register("TargetDelta");

        /// <summary>
        /// Triggers an event notice when the actual value differs from the target value more than plus
        /// or minus (monitorValue * target value) since the time that this monitor was set or since the
        /// last time this event notice was sent, whichever was last. Behavior of this type of monitor for a
        /// variable that is not numeric, is not defined.
        /// </summary>
        /// <example>When target = 100, monitorValue = 0.1, then an event is triggered when actual &lt; 90 or actual &gt; 110.</example>
        public static MonitorType TargetDeltaRelative     { get; }
            = Register("TargetDeltaRelative");

        #endregion


        #region Operator overloading

        #region Operator == (MonitorType1, MonitorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MonitorType1">A monitor type.</param>
        /// <param name="MonitorType2">Another monitor type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MonitorType MonitorType1,
                                           MonitorType MonitorType2)

            => MonitorType1.Equals(MonitorType2);

        #endregion

        #region Operator != (MonitorType1, MonitorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MonitorType1">A monitor type.</param>
        /// <param name="MonitorType2">Another monitor type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MonitorType MonitorType1,
                                           MonitorType MonitorType2)

            => !MonitorType1.Equals(MonitorType2);

        #endregion

        #region Operator <  (MonitorType1, MonitorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MonitorType1">A monitor type.</param>
        /// <param name="MonitorType2">Another monitor type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (MonitorType MonitorType1,
                                          MonitorType MonitorType2)

            => MonitorType1.CompareTo(MonitorType2) < 0;

        #endregion

        #region Operator <= (MonitorType1, MonitorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MonitorType1">A monitor type.</param>
        /// <param name="MonitorType2">Another monitor type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (MonitorType MonitorType1,
                                           MonitorType MonitorType2)

            => MonitorType1.CompareTo(MonitorType2) <= 0;

        #endregion

        #region Operator >  (MonitorType1, MonitorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MonitorType1">A monitor type.</param>
        /// <param name="MonitorType2">Another monitor type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (MonitorType MonitorType1,
                                          MonitorType MonitorType2)

            => MonitorType1.CompareTo(MonitorType2) > 0;

        #endregion

        #region Operator >= (MonitorType1, MonitorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MonitorType1">A monitor type.</param>
        /// <param name="MonitorType2">Another monitor type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (MonitorType MonitorType1,
                                           MonitorType MonitorType2)

            => MonitorType1.CompareTo(MonitorType2) >= 0;

        #endregion

        #endregion

        #region IComparable<MonitorType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two monitor types.
        /// </summary>
        /// <param name="Object">A monitor type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is MonitorType monitorType
                   ? CompareTo(monitorType)
                   : throw new ArgumentException("The given object is not a monitor type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(MonitorType)

        /// <summary>
        /// Compares two monitor types.
        /// </summary>
        /// <param name="MonitorType">A monitor type to compare with.</param>
        public Int32 CompareTo(MonitorType MonitorType)

            => String.Compare(InternalId,
                              MonitorType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<MonitorType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two monitor types for equality.
        /// </summary>
        /// <param name="Object">A monitor type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MonitorType monitorType &&
                   Equals(monitorType);

        #endregion

        #region Equals(MonitorType)

        /// <summary>
        /// Compares two monitor types for equality.
        /// </summary>
        /// <param name="MonitorType">A monitor type to compare with.</param>
        public Boolean Equals(MonitorType MonitorType)

            => String.Equals(InternalId,
                             MonitorType.InternalId,
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
