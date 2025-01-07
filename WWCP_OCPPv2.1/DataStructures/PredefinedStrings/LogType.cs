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
    /// Extension methods for log types.
    /// </summary>
    public static class LogTypeExtensions
    {

        /// <summary>
        /// Indicates whether this log type is null or empty.
        /// </summary>
        /// <param name="LogType">A log type.</param>
        public static Boolean IsNullOrEmpty(this LogType? LogType)
            => !LogType.HasValue || LogType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this log type is null or empty.
        /// </summary>
        /// <param name="LogType">A log type.</param>
        public static Boolean IsNotNullOrEmpty(this LogType? LogType)
            => LogType.HasValue && LogType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A log type.
    /// </summary>
    public readonly struct LogType : IId,
                                     IEquatable<LogType>,
                                     IComparable<LogType>
    {

        #region Data

        private readonly static Dictionary<String, LogType>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                       InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this log type is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this log type is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the log type.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new log type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a log type.</param>
        private LogType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static LogType Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new LogType(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a log type.
        /// </summary>
        /// <param name="Text">A text representation of a log type.</param>
        public static LogType Parse(String Text)
        {

            if (TryParse(Text, out var logType))
                return logType;

            throw new ArgumentException($"Invalid text representation of a log type: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a log type.
        /// </summary>
        /// <param name="Text">A text representation of a log type.</param>
        public static LogType? TryParse(String Text)
        {

            if (TryParse(Text, out var logType))
                return logType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out LogType)

        /// <summary>
        /// Try to parse the given text as a log type.
        /// </summary>
        /// <param name="Text">A text representation of a log type.</param>
        /// <param name="LogType">The parsed log type.</param>
        public static Boolean TryParse(String Text, out LogType LogType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out LogType))
                    LogType = Register(Text);

                return true;

            }

            LogType = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this log type.
        /// </summary>
        public LogType Clone

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// This contains the field definition of a diagnostics log file.
        /// </summary>
        public static LogType DiagnosticsLog      { get; }
            = Register("DiagnosticsLog");

        /// <summary>
        /// Sent by the Central System to the Charge Point to request that the Charge Point uploads the security log.
        /// </summary>
        public static LogType SecurityLog         { get; }
            = Register("eMAID");

        /// <summary>
        /// The log of sampled measurements from the DataCollector component.
        /// </summary>
        public static LogType DataCollectorLog    { get; }
            = Register("DataCollectorLog");

        #endregion


        #region Operator overloading

        #region Operator == (LogType1, LogType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LogType1">A log type.</param>
        /// <param name="LogType2">Another log type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (LogType LogType1,
                                           LogType LogType2)

            => LogType1.Equals(LogType2);

        #endregion

        #region Operator != (LogType1, LogType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LogType1">A log type.</param>
        /// <param name="LogType2">Another log type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (LogType LogType1,
                                           LogType LogType2)

            => !LogType1.Equals(LogType2);

        #endregion

        #region Operator <  (LogType1, LogType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LogType1">A log type.</param>
        /// <param name="LogType2">Another log type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (LogType LogType1,
                                          LogType LogType2)

            => LogType1.CompareTo(LogType2) < 0;

        #endregion

        #region Operator <= (LogType1, LogType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LogType1">A log type.</param>
        /// <param name="LogType2">Another log type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (LogType LogType1,
                                           LogType LogType2)

            => LogType1.CompareTo(LogType2) <= 0;

        #endregion

        #region Operator >  (LogType1, LogType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LogType1">A log type.</param>
        /// <param name="LogType2">Another log type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (LogType LogType1,
                                          LogType LogType2)

            => LogType1.CompareTo(LogType2) > 0;

        #endregion

        #region Operator >= (LogType1, LogType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LogType1">A log type.</param>
        /// <param name="LogType2">Another log type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (LogType LogType1,
                                           LogType LogType2)

            => LogType1.CompareTo(LogType2) >= 0;

        #endregion

        #endregion

        #region IComparable<LogType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two log types.
        /// </summary>
        /// <param name="Object">A log type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is LogType logType
                   ? CompareTo(logType)
                   : throw new ArgumentException("The given object is not a log type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(LogType)

        /// <summary>
        /// Compares two log types.
        /// </summary>
        /// <param name="LogType">A log type to compare with.</param>
        public Int32 CompareTo(LogType LogType)

            => String.Compare(InternalId,
                              LogType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<LogType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two log types for equality.
        /// </summary>
        /// <param name="Object">A log type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LogType logType &&
                   Equals(logType);

        #endregion

        #region Equals(LogType)

        /// <summary>
        /// Compares two log types for equality.
        /// </summary>
        /// <param name="LogType">A log type to compare with.</param>
        public Boolean Equals(LogType LogType)

            => String.Equals(InternalId,
                             LogType.InternalId,
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
