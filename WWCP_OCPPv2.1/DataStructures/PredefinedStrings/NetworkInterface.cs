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
    /// Extension methods for network interfaces.
    /// </summary>
    public static class NetworkInterfaceExtensions
    {

        /// <summary>
        /// Indicates whether this network interface is null or empty.
        /// </summary>
        /// <param name="NetworkInterface">A network interface.</param>
        public static Boolean IsNullOrEmpty(this NetworkInterface? NetworkInterface)
            => !NetworkInterface.HasValue || NetworkInterface.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this network interface is null or empty.
        /// </summary>
        /// <param name="NetworkInterface">A network interface.</param>
        public static Boolean IsNotNullOrEmpty(this NetworkInterface? NetworkInterface)
            => NetworkInterface.HasValue && NetworkInterface.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A network interface.
    /// </summary>
    public readonly struct NetworkInterface : IId,
                                              IEquatable<NetworkInterface>,
                                              IComparable<NetworkInterface>
    {

        #region Data

        private readonly static Dictionary<String, NetworkInterface>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                                InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this network interface is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this network interface is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the network interface.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new network interface based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a network interface.</param>
        private NetworkInterface(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static NetworkInterface Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new NetworkInterface(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a network interface.
        /// </summary>
        /// <param name="Text">A text representation of a network interface.</param>
        public static NetworkInterface Parse(String Text)
        {

            if (TryParse(Text, out var networkInterface))
                return networkInterface;

            throw new ArgumentException($"Invalid text representation of a network interface: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a network interface.
        /// </summary>
        /// <param name="Text">A text representation of a network interface.</param>
        public static NetworkInterface? TryParse(String Text)
        {

            if (TryParse(Text, out var networkInterface))
                return networkInterface;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out NetworkInterface)

        /// <summary>
        /// Try to parse the given text as a network interface.
        /// </summary>
        /// <param name="Text">A text representation of a network interface.</param>
        /// <param name="NetworkInterface">The parsed network interface.</param>
        public static Boolean TryParse(String Text, out NetworkInterface NetworkInterface)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out NetworkInterface))
                    NetworkInterface = Register(Text);

                return true;

            }

            NetworkInterface = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this network interface.
        /// </summary>
        public NetworkInterface Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Use wired connection 0.
        /// </summary>
        public static NetworkInterface Wired0       { get; }
            = Register("Wired0");

        /// <summary>
        /// Use wired connection 1.
        /// </summary>
        public static NetworkInterface Wired1       { get; }
            = Register("Wired1");

        /// <summary>
        /// Use wired connection 2.
        /// </summary>
        public static NetworkInterface Wired2       { get; }
            = Register("Wired2");

        /// <summary>
        /// Use wired connection 3.
        /// </summary>
        public static NetworkInterface Wired3       { get; }
            = Register("Wired3");


        /// <summary>
        /// Use wireless connection 0.
        /// </summary>
        public static NetworkInterface Wireless0    { get; }
            = Register("Wireless0");

        /// <summary>
        /// Use wireless connection 1.
        /// </summary>
        public static NetworkInterface Wireless1    { get; }
            = Register("Wireless1");

        /// <summary>
        /// Use wireless connection 2.
        /// </summary>
        public static NetworkInterface Wireless2    { get; }
            = Register("Wireless2");

        /// <summary>
        /// Use wireless connection 3.
        /// </summary>
        public static NetworkInterface Wireless3    { get; }
            = Register("Wireless3");

        #endregion


        #region Operator overloading

        #region Operator == (NetworkInterface1, NetworkInterface2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkInterface1">A network interface.</param>
        /// <param name="NetworkInterface2">Another network interface.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NetworkInterface NetworkInterface1,
                                           NetworkInterface NetworkInterface2)

            => NetworkInterface1.Equals(NetworkInterface2);

        #endregion

        #region Operator != (NetworkInterface1, NetworkInterface2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkInterface1">A network interface.</param>
        /// <param name="NetworkInterface2">Another network interface.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NetworkInterface NetworkInterface1,
                                           NetworkInterface NetworkInterface2)

            => !NetworkInterface1.Equals(NetworkInterface2);

        #endregion

        #region Operator <  (NetworkInterface1, NetworkInterface2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkInterface1">A network interface.</param>
        /// <param name="NetworkInterface2">Another network interface.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (NetworkInterface NetworkInterface1,
                                          NetworkInterface NetworkInterface2)

            => NetworkInterface1.CompareTo(NetworkInterface2) < 0;

        #endregion

        #region Operator <= (NetworkInterface1, NetworkInterface2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkInterface1">A network interface.</param>
        /// <param name="NetworkInterface2">Another network interface.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (NetworkInterface NetworkInterface1,
                                           NetworkInterface NetworkInterface2)

            => NetworkInterface1.CompareTo(NetworkInterface2) <= 0;

        #endregion

        #region Operator >  (NetworkInterface1, NetworkInterface2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkInterface1">A network interface.</param>
        /// <param name="NetworkInterface2">Another network interface.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (NetworkInterface NetworkInterface1,
                                          NetworkInterface NetworkInterface2)

            => NetworkInterface1.CompareTo(NetworkInterface2) > 0;

        #endregion

        #region Operator >= (NetworkInterface1, NetworkInterface2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkInterface1">A network interface.</param>
        /// <param name="NetworkInterface2">Another network interface.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (NetworkInterface NetworkInterface1,
                                           NetworkInterface NetworkInterface2)

            => NetworkInterface1.CompareTo(NetworkInterface2) >= 0;

        #endregion

        #endregion

        #region IComparable<NetworkInterface> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two network interfaces.
        /// </summary>
        /// <param name="Object">A network interface to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is NetworkInterface networkInterface
                   ? CompareTo(networkInterface)
                   : throw new ArgumentException("The given object is not a network interface!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(NetworkInterface)

        /// <summary>
        /// Compares two network interfaces.
        /// </summary>
        /// <param name="NetworkInterface">A network interface to compare with.</param>
        public Int32 CompareTo(NetworkInterface NetworkInterface)

            => String.Compare(InternalId,
                              NetworkInterface.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<NetworkInterface> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two network interfaces for equality.
        /// </summary>
        /// <param name="Object">A network interface to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NetworkInterface networkInterface &&
                   Equals(networkInterface);

        #endregion

        #region Equals(NetworkInterface)

        /// <summary>
        /// Compares two network interfaces for equality.
        /// </summary>
        /// <param name="NetworkInterface">A network interface to compare with.</param>
        public Boolean Equals(NetworkInterface NetworkInterface)

            => String.Equals(InternalId,
                             NetworkInterface.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
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
