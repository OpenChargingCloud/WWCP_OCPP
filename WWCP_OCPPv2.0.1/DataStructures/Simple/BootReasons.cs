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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Extention methods for boot reasons.
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

        #region Properties

        /// <summary>
        /// The boot reason.
        /// </summary>
        public String  Text    { get; }


        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => Text.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => Text.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the boot reason.
        /// </summary>
        public UInt64 Length
            => (UInt64) (Text?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new boot reason.
        /// </summary>
        /// <param name="Text">The string representation of the boot reason.</param>
        private BootReason(String Text)
        {
            this.Text = Text;
        }

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

            throw new ArgumentException("Invalid text representation of a boot reason: '" + Text + "'!",
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
                BootReason = new BootReason(Text);
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
                   new String(Text?.ToCharArray())
               );

        #endregion


        #region Statics

        /// <summary>
        /// Application reset
        /// </summary>
        public readonly static BootReason ApplicationReset
            = new ("ApplicationReset");

        /// <summary>
        /// Firmware update
        /// </summary>
        public readonly static BootReason FirmwareUpdate
            = new ("FirmwareUpdate");

        /// <summary>
        /// Local reset
        /// </summary>
        public readonly static BootReason LocalReset
            = new ("LocalReset");

        /// <summary>
        /// Power up
        /// </summary>
        public readonly static BootReason PowerUp
            = new ("PowerUp");

        /// <summary>
        /// Remote reset
        /// </summary>
        public readonly static BootReason RemoteReset
            = new ("RemoteReset");

        /// <summary>
        /// Scheduled reset
        /// </summary>
        public readonly static BootReason ScheduledReset
            = new ("ScheduledReset");

        /// <summary>
        /// Triggered.
        /// </summary>
        public readonly static BootReason Triggered
            = new ("Triggered");

        /// <summary>
        /// Unknown boot reason
        /// </summary>
        public readonly static BootReason Unknown
            = new ("Unknown");

        /// <summary>
        /// Watchdog
        /// </summary>
        public readonly static BootReason Watchdog
            = new ("Watchdog");

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

            => String.Compare(Text,
                              BootReason.Text,
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

            => String.Equals(Text,
                             BootReason.Text,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => Text?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Text ?? "";

        #endregion

    }

}
