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
    /// Extension methods for operation modes.
    /// </summary>
    public static class OperationModeExtensions
    {

        /// <summary>
        /// Indicates whether this operation mode is null or empty.
        /// </summary>
        /// <param name="OperationMode">A operation mode.</param>
        public static Boolean IsNullOrEmpty(this OperationMode? OperationMode)
            => !OperationMode.HasValue || OperationMode.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this operation mode is null or empty.
        /// </summary>
        /// <param name="OperationMode">A operation mode.</param>
        public static Boolean IsNotNullOrEmpty(this OperationMode? OperationMode)
            => OperationMode.HasValue && OperationMode.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A operation mode.
    /// </summary>
    public readonly struct OperationMode : IId,
                                           IEquatable<OperationMode>,
                                           IComparable<OperationMode>
    {

        #region Data

        private readonly static Dictionary<String, OperationMode>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                             InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this operation mode is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this operation mode is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the operation mode.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new operation mode based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an operation mode.</param>
        private OperationMode(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static OperationMode Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new OperationMode(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an operation mode.
        /// </summary>
        /// <param name="Text">A text representation of an operation mode.</param>
        public static OperationMode Parse(String Text)
        {

            if (TryParse(Text, out var operationMode))
                return operationMode;

            throw new ArgumentException($"Invalid text representation of an operation mode: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as operation mode.
        /// </summary>
        /// <param name="Text">A text representation of an operation mode.</param>
        public static OperationMode? TryParse(String Text)
        {

            if (TryParse(Text, out var operationMode))
                return operationMode;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out OperationMode)

        /// <summary>
        /// Try to parse the given text as operation mode.
        /// </summary>
        /// <param name="Text">A text representation of an operation mode.</param>
        /// <param name="OperationMode">The parsed operation mode.</param>
        public static Boolean TryParse(String Text, out OperationMode OperationMode)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out OperationMode))
                    OperationMode = Register(Text);

                return true;

            }

            OperationMode = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this operation mode.
        /// </summary>
        public OperationMode Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Minimize energy consumption by having the EV either on standby or in sleep.
        /// </summary>
        public static OperationMode Idle                  { get; }
            = Register("Idle");

        /// <summary>
        /// Classic charging or smart charging mode.
        /// </summary>
        public static OperationMode ChargingOnly          { get; }
            = Register("ChargingOnly");

        /// <summary>
        /// Control of setpoint by the CSMS or some secondary actor that relais through the CSMS.
        /// </summary>
        public static OperationMode CentralSetpoint       { get; }
            = Register("CentralSetpoint");

        /// <summary>
        /// Control of setpoint by an external actor on the charging station.
        /// </summary>
        public static OperationMode ExternalSetpoint      { get; }
            = Register("ExternalSetpoint");

        /// <summary>
        /// Control of (dis)charging limits by an external actor on the charging station.
        /// </summary>
        public static OperationMode ExternalLimits        { get; }
            = Register("ExternalLimits");

        /// <summary>
        /// Frequency support with control by the CSMS or some secondary actor that relais through the CSMS.
        /// </summary>
        public static OperationMode CentralFrequency      { get; }
            = Register("CentralFrequency");

        /// <summary>
        /// Frequency support with control in the charging station.
        /// </summary>
        public static OperationMode LocalFrequency        { get; }
            = Register("LocalFrequency");

        /// <summary>
        /// Load balancing by the charging station.
        /// </summary>
        public static OperationMode LocalLoadBalancing    { get; }
            = Register("LocalLoadBalancing");

        #endregion


        #region Operator overloading

        #region Operator == (OperationMode1, OperationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OperationMode1">An operation mode.</param>
        /// <param name="OperationMode2">Another operation mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (OperationMode OperationMode1,
                                           OperationMode OperationMode2)

            => OperationMode1.Equals(OperationMode2);

        #endregion

        #region Operator != (OperationMode1, OperationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OperationMode1">An operation mode.</param>
        /// <param name="OperationMode2">Another operation mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (OperationMode OperationMode1,
                                           OperationMode OperationMode2)

            => !OperationMode1.Equals(OperationMode2);

        #endregion

        #region Operator <  (OperationMode1, OperationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OperationMode1">An operation mode.</param>
        /// <param name="OperationMode2">Another operation mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (OperationMode OperationMode1,
                                          OperationMode OperationMode2)

            => OperationMode1.CompareTo(OperationMode2) < 0;

        #endregion

        #region Operator <= (OperationMode1, OperationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OperationMode1">An operation mode.</param>
        /// <param name="OperationMode2">Another operation mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (OperationMode OperationMode1,
                                           OperationMode OperationMode2)

            => OperationMode1.CompareTo(OperationMode2) <= 0;

        #endregion

        #region Operator >  (OperationMode1, OperationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OperationMode1">An operation mode.</param>
        /// <param name="OperationMode2">Another operation mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (OperationMode OperationMode1,
                                          OperationMode OperationMode2)

            => OperationMode1.CompareTo(OperationMode2) > 0;

        #endregion

        #region Operator >= (OperationMode1, OperationMode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OperationMode1">An operation mode.</param>
        /// <param name="OperationMode2">Another operation mode.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (OperationMode OperationMode1,
                                           OperationMode OperationMode2)

            => OperationMode1.CompareTo(OperationMode2) >= 0;

        #endregion

        #endregion

        #region IComparable<OperationMode> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two operation modes.
        /// </summary>
        /// <param name="Object">An operation mode to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is OperationMode operationMode
                   ? CompareTo(operationMode)
                   : throw new ArgumentException("The given object is not operation mode!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(OperationMode)

        /// <summary>
        /// Compares two operation modes.
        /// </summary>
        /// <param name="OperationMode">An operation mode to compare with.</param>
        public Int32 CompareTo(OperationMode OperationMode)

            => String.Compare(InternalId,
                              OperationMode.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<OperationMode> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two operation modes for equality.
        /// </summary>
        /// <param name="Object">An operation mode to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is OperationMode operationMode &&
                   Equals(operationMode);

        #endregion

        #region Equals(OperationMode)

        /// <summary>
        /// Compares two operation modes for equality.
        /// </summary>
        /// <param name="OperationMode">An operation mode to compare with.</param>
        public Boolean Equals(OperationMode OperationMode)

            => String.Equals(InternalId,
                             OperationMode.InternalId,
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
