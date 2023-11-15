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
    /// Extension methods for boot reasons.
    /// </summary>
    public static class BootReasonExtensions
    {

        /// <summary>
        /// Indicates whether this boot reason is null or empty.
        /// </summary>
        /// <param name="BootReason">A boot reason.</param>
        public static Boolean IsNullOrEmpty(this BootReason? BootReason)
            => !BootReason.HasValue || BootReason.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this boot reason is null or empty.
        /// </summary>
        /// <param name="BootReason">A boot reason.</param>
        public static Boolean IsNotNullOrEmpty(this BootReason? BootReason)
            => BootReason.HasValue && BootReason.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A boot reason.
    /// </summary>
    public readonly struct BootReason : IId,
                                        IEquatable<BootReason>,
                                        IComparable<BootReason>
    {

        #region Data

        private readonly static Dictionary<String, BootReason>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                          InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this boot reason is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this boot reason is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the boot reason.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new boot reason based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a boot reason.</param>
        private BootReason(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static BootReason Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new BootReason(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a boot reason.
        /// </summary>
        /// <param name="Text">A text representation of a boot reason.</param>
        public static BootReason Parse(String Text)
        {

            if (TryParse(Text, out var bootReason))
                return bootReason;

            throw new ArgumentException($"Invalid text representation of a boot reason: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a boot reason.
        /// </summary>
        /// <param name="Text">A text representation of a boot reason.</param>
        public static BootReason? TryParse(String Text)
        {

            if (TryParse(Text, out var bootReason))
                return bootReason;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out BootReason)

        /// <summary>
        /// Try to parse the given text as a boot reason.
        /// </summary>
        /// <param name="Text">A text representation of a boot reason.</param>
        /// <param name="BootReason">The parsed boot reason.</param>
        public static Boolean TryParse(String Text, out BootReason BootReason)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out BootReason))
                    BootReason = Register(Text);

                return true;

            }

            BootReason = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this boot reason.
        /// </summary>
        public BootReason Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Application reset
        /// </summary>
        public static BootReason ApplicationReset    { get; }
            = Register("ApplicationReset");

        /// <summary>
        /// Firmware update
        /// </summary>
        public static BootReason FirmwareUpdate      { get; }
            = Register("FirmwareUpdate");

        /// <summary>
        /// Local reset
        /// </summary>
        public static BootReason LocalReset          { get; }
            = Register("LocalReset");

        /// <summary>
        /// Power up
        /// </summary>
        public static BootReason PowerUp             { get; }
            = Register("PowerUp");

        /// <summary>
        /// Remote reset
        /// </summary>
        public static BootReason RemoteReset         { get; }
            = Register("RemoteReset");

        /// <summary>
        /// Scheduled reset
        /// </summary>
        public static BootReason ScheduledReset      { get; }
            = Register("ScheduledReset");

        /// <summary>
        /// Triggered
        /// </summary>
        public static BootReason Triggered           { get; }
            = Register("Triggered");

        /// <summary>
        /// Unknown boot reason
        /// </summary>
        public static BootReason Unknown             { get; }
            = Register("Unknown");

        /// <summary>
        /// Watchdog
        /// </summary>
        public static BootReason Watchdog            { get; }
            = Register("Watchdog");

        #endregion


        #region Operator overloading

        #region Operator == (BootReason1, BootReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BootReason1">A boot reason.</param>
        /// <param name="BootReason2">Another boot reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BootReason BootReason1,
                                           BootReason BootReason2)

            => BootReason1.Equals(BootReason2);

        #endregion

        #region Operator != (BootReason1, BootReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BootReason1">A boot reason.</param>
        /// <param name="BootReason2">Another boot reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BootReason BootReason1,
                                           BootReason BootReason2)

            => !BootReason1.Equals(BootReason2);

        #endregion

        #region Operator <  (BootReason1, BootReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BootReason1">A boot reason.</param>
        /// <param name="BootReason2">Another boot reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BootReason BootReason1,
                                          BootReason BootReason2)

            => BootReason1.CompareTo(BootReason2) < 0;

        #endregion

        #region Operator <= (BootReason1, BootReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BootReason1">A boot reason.</param>
        /// <param name="BootReason2">Another boot reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BootReason BootReason1,
                                           BootReason BootReason2)

            => BootReason1.CompareTo(BootReason2) <= 0;

        #endregion

        #region Operator >  (BootReason1, BootReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BootReason1">A boot reason.</param>
        /// <param name="BootReason2">Another boot reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BootReason BootReason1,
                                          BootReason BootReason2)

            => BootReason1.CompareTo(BootReason2) > 0;

        #endregion

        #region Operator >= (BootReason1, BootReason2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BootReason1">A boot reason.</param>
        /// <param name="BootReason2">Another boot reason.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BootReason BootReason1,
                                           BootReason BootReason2)

            => BootReason1.CompareTo(BootReason2) >= 0;

        #endregion

        #endregion

        #region IComparable<BootReason> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two boot reasons.
        /// </summary>
        /// <param name="Object">A boot reason to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is BootReason bootReason
                   ? CompareTo(bootReason)
                   : throw new ArgumentException("The given object is not a boot reason!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(BootReason)

        /// <summary>
        /// Compares two boot reasons.
        /// </summary>
        /// <param name="BootReason">A boot reason to compare with.</param>
        public Int32 CompareTo(BootReason BootReason)

            => String.Compare(InternalId,
                              BootReason.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<BootReason> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two boot reasons for equality.
        /// </summary>
        /// <param name="Object">A boot reason to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BootReason bootReason &&
                   Equals(bootReason);

        #endregion

        #region Equals(BootReason)

        /// <summary>
        /// Compares two boot reasons for equality.
        /// </summary>
        /// <param name="BootReason">A boot reason to compare with.</param>
        public Boolean Equals(BootReason BootReason)

            => String.Equals(InternalId,
                             BootReason.InternalId,
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
