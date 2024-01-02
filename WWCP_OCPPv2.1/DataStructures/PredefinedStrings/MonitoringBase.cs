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
    /// Extension methods for monitoring bases.
    /// </summary>
    public static class MonitoringBaseExtensions
    {

        /// <summary>
        /// Indicates whether this monitoring base is null or empty.
        /// </summary>
        /// <param name="MonitoringBase">A monitoring base.</param>
        public static Boolean IsNullOrEmpty(this MonitoringBase? MonitoringBase)
            => !MonitoringBase.HasValue || MonitoringBase.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this monitoring base is null or empty.
        /// </summary>
        /// <param name="MonitoringBase">A monitoring base.</param>
        public static Boolean IsNotNullOrEmpty(this MonitoringBase? MonitoringBase)
            => MonitoringBase.HasValue && MonitoringBase.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A monitoring base.
    /// </summary>
    public readonly struct MonitoringBase : IId,
                                            IEquatable<MonitoringBase>,
                                            IComparable<MonitoringBase>
    {

        #region Data

        private readonly static Dictionary<String, MonitoringBase>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                              InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this monitoring base is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this monitoring base is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the monitoring base.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new monitoring base based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a monitoring base.</param>
        private MonitoringBase(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static MonitoringBase Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new MonitoringBase(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a monitoring base.
        /// </summary>
        /// <param name="Text">A text representation of a monitoring base.</param>
        public static MonitoringBase Parse(String Text)
        {

            if (TryParse(Text, out var monitoringBase))
                return monitoringBase;

            throw new ArgumentException($"Invalid text representation of a monitoring base: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a monitoring base.
        /// </summary>
        /// <param name="Text">A text representation of a monitoring base.</param>
        public static MonitoringBase? TryParse(String Text)
        {

            if (TryParse(Text, out var monitoringBase))
                return monitoringBase;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out MonitoringBase)

        /// <summary>
        /// Try to parse the given text as a monitoring base.
        /// </summary>
        /// <param name="Text">A text representation of a monitoring base.</param>
        /// <param name="MonitoringBase">The parsed monitoring base.</param>
        public static Boolean TryParse(String Text, out MonitoringBase MonitoringBase)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out MonitoringBase))
                    MonitoringBase = Register(Text);

                return true;

            }

            MonitoringBase = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this monitoring base.
        /// </summary>
        public MonitoringBase Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Activate all pre-configured monitors.
        /// </summary>
        public static MonitoringBase All               { get; }
            = Register("All");

        /// <summary>
        /// Activate the default monitoring settings as recommended by the manufacturer.
        /// This is a subset of all preconfigured monitors.
        /// </summary>
        public static MonitoringBase FactoryDefault    { get; }
            = Register("FactoryDefault");

        /// <summary>
        /// Clears all custom monitors and disables all pre-configured monitors.
        /// </summary>
        public static MonitoringBase HardWiredOnly     { get; }
            = Register("HardWiredOnly");

        #endregion


        #region Operator overloading

        #region Operator == (MonitoringBase1, MonitoringBase2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MonitoringBase1">A monitoring base.</param>
        /// <param name="MonitoringBase2">Another monitoring base.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MonitoringBase MonitoringBase1,
                                           MonitoringBase MonitoringBase2)

            => MonitoringBase1.Equals(MonitoringBase2);

        #endregion

        #region Operator != (MonitoringBase1, MonitoringBase2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MonitoringBase1">A monitoring base.</param>
        /// <param name="MonitoringBase2">Another monitoring base.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MonitoringBase MonitoringBase1,
                                           MonitoringBase MonitoringBase2)

            => !MonitoringBase1.Equals(MonitoringBase2);

        #endregion

        #region Operator <  (MonitoringBase1, MonitoringBase2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MonitoringBase1">A monitoring base.</param>
        /// <param name="MonitoringBase2">Another monitoring base.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (MonitoringBase MonitoringBase1,
                                          MonitoringBase MonitoringBase2)

            => MonitoringBase1.CompareTo(MonitoringBase2) < 0;

        #endregion

        #region Operator <= (MonitoringBase1, MonitoringBase2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MonitoringBase1">A monitoring base.</param>
        /// <param name="MonitoringBase2">Another monitoring base.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (MonitoringBase MonitoringBase1,
                                           MonitoringBase MonitoringBase2)

            => MonitoringBase1.CompareTo(MonitoringBase2) <= 0;

        #endregion

        #region Operator >  (MonitoringBase1, MonitoringBase2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MonitoringBase1">A monitoring base.</param>
        /// <param name="MonitoringBase2">Another monitoring base.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (MonitoringBase MonitoringBase1,
                                          MonitoringBase MonitoringBase2)

            => MonitoringBase1.CompareTo(MonitoringBase2) > 0;

        #endregion

        #region Operator >= (MonitoringBase1, MonitoringBase2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MonitoringBase1">A monitoring base.</param>
        /// <param name="MonitoringBase2">Another monitoring base.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (MonitoringBase MonitoringBase1,
                                           MonitoringBase MonitoringBase2)

            => MonitoringBase1.CompareTo(MonitoringBase2) >= 0;

        #endregion

        #endregion

        #region IComparable<MonitoringBase> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two monitoring bases.
        /// </summary>
        /// <param name="Object">A monitoring base to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is MonitoringBase monitoringBase
                   ? CompareTo(monitoringBase)
                   : throw new ArgumentException("The given object is not a monitoring base!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(MonitoringBase)

        /// <summary>
        /// Compares two monitoring bases.
        /// </summary>
        /// <param name="MonitoringBase">A monitoring base to compare with.</param>
        public Int32 CompareTo(MonitoringBase MonitoringBase)

            => String.Compare(InternalId,
                              MonitoringBase.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<MonitoringBase> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two monitoring bases for equality.
        /// </summary>
        /// <param name="Object">A monitoring base to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MonitoringBase monitoringBase &&
                   Equals(monitoringBase);

        #endregion

        #region Equals(MonitoringBase)

        /// <summary>
        /// Compares two monitoring bases for equality.
        /// </summary>
        /// <param name="MonitoringBase">A monitoring base to compare with.</param>
        public Boolean Equals(MonitoringBase MonitoringBase)

            => String.Equals(InternalId,
                             MonitoringBase.InternalId,
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
