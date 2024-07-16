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
    /// Extension methods for network topology statuss.
    /// </summary>
    public static class NetworkTopologyStatusExtensions
    {

        /// <summary>
        /// Indicates whether this network topology status is null or empty.
        /// </summary>
        /// <param name="NetworkTopologyStatus">A network topology status.</param>
        public static Boolean IsNullOrEmpty(this NetworkTopologyStatus? NetworkTopologyStatus)
            => !NetworkTopologyStatus.HasValue || NetworkTopologyStatus.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this network topology status is null or empty.
        /// </summary>
        /// <param name="NetworkTopologyStatus">A network topology status.</param>
        public static Boolean IsNotNullOrEmpty(this NetworkTopologyStatus? NetworkTopologyStatus)
            => NetworkTopologyStatus.HasValue && NetworkTopologyStatus.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A network topology status.
    /// </summary>
    public readonly struct NetworkTopologyStatus : IId,
                                                   IEquatable<NetworkTopologyStatus>,
                                                   IComparable<NetworkTopologyStatus>
    {

        #region Data

        private readonly static Dictionary<String, NetworkTopologyStatus>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                          InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the result code.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new network topology status based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of the result code.</param>
        private NetworkTopologyStatus(String Text)
        {
            this.InternalId  = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static NetworkTopologyStatus Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new NetworkTopologyStatus(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a network topology status.
        /// </summary>
        /// <param name="Text">A text representation of a network topology status.</param>
        public static NetworkTopologyStatus Parse(String Text)
        {

            if (TryParse(Text, out var networkTopologyStatus))
                return networkTopologyStatus;

            throw new ArgumentException($"Invalid text representation of a network topology status: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a network topology status.
        /// </summary>
        /// <param name="Text">A text representation of a network topology status.</param>
        public static NetworkTopologyStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var networkTopologyStatus))
                return networkTopologyStatus;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out networkTopologyStatus)

        /// <summary>
        /// Try to parse the given text as a network topology status.
        /// </summary>
        /// <param name="Text">A text representation of a network topology status.</param>
        /// <param name="networkTopologyStatus">The parsed network topology status.</param>
        public static Boolean TryParse(String Text, out NetworkTopologyStatus NetworkTopologyStatus)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out NetworkTopologyStatus))
                    NetworkTopologyStatus = Register(Text);

                return true;

            }

            NetworkTopologyStatus = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this network topology status.
        /// </summary>
        public NetworkTopologyStatus Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The Network Topology Information was accepted and processed.
        /// </summary>
        public static NetworkTopologyStatus  Accepted           { get; }
            = Register("Accepted");

        /// <summary>
        /// The Network Topology Information was accepted, but not yet processed.
        /// </summary>
        public static NetworkTopologyStatus  Pending            { get; }
            = Register("Pending");

        /// <summary>
        /// The Network Topology Information was accepted, but processing is not supported.
        /// </summary>
        public static NetworkTopologyStatus  NotSupported       { get; }
            = Register("NotSupported");

        /// <summary>
        /// An internal error occurred and the receiver was not able to process the Network Topology Information successfully.
        /// </summary>
        public static NetworkTopologyStatus  Error              { get; }
            = Register("Error");

        /// <summary>
        /// The signatures of the Network Topology Information (request) could not be verified.
        /// </summary>
        public static NetworkTopologyStatus  SignatureErrors    { get; }
            = Register("SignatureErrors");

        /// <summary>
        /// The Network Topology Information was rejected.
        /// </summary>
        public static NetworkTopologyStatus  Rejected           { get; }
            = Register("Rejected");


        public static NetworkTopologyStatus  UnknownClient      { get; }
            = Register("UnknownClient");

        public static NetworkTopologyStatus  NetworkError       { get; }
            = Register("NetworkError");

        public static NetworkTopologyStatus  Timeout            { get; }
            = Register("Timeout");

        #endregion


        #region Operator overloading

        #region Operator == (NetworkTopologyStatus1, NetworkTopologyStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkTopologyStatus1">A network topology status.</param>
        /// <param name="NetworkTopologyStatus2">Another network topology status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NetworkTopologyStatus NetworkTopologyStatus1,
                                           NetworkTopologyStatus NetworkTopologyStatus2)

            => NetworkTopologyStatus1.Equals(NetworkTopologyStatus2);

        #endregion

        #region Operator != (NetworkTopologyStatus1, NetworkTopologyStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkTopologyStatus1">A network topology status.</param>
        /// <param name="NetworkTopologyStatus2">Another network topology status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NetworkTopologyStatus NetworkTopologyStatus1,
                                           NetworkTopologyStatus NetworkTopologyStatus2)

            => !NetworkTopologyStatus1.Equals(NetworkTopologyStatus2);

        #endregion

        #region Operator <  (NetworkTopologyStatus1, NetworkTopologyStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkTopologyStatus1">A network topology status.</param>
        /// <param name="NetworkTopologyStatus2">Another network topology status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (NetworkTopologyStatus NetworkTopologyStatus1,
                                          NetworkTopologyStatus NetworkTopologyStatus2)

            => NetworkTopologyStatus1.CompareTo(NetworkTopologyStatus2) < 0;

        #endregion

        #region Operator <= (NetworkTopologyStatus1, NetworkTopologyStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkTopologyStatus1">A network topology status.</param>
        /// <param name="NetworkTopologyStatus2">Another network topology status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (NetworkTopologyStatus NetworkTopologyStatus1,
                                           NetworkTopologyStatus NetworkTopologyStatus2)

            => NetworkTopologyStatus1.CompareTo(NetworkTopologyStatus2) <= 0;

        #endregion

        #region Operator >  (NetworkTopologyStatus1, NetworkTopologyStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkTopologyStatus1">A network topology status.</param>
        /// <param name="NetworkTopologyStatus2">Another network topology status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (NetworkTopologyStatus NetworkTopologyStatus1,
                                          NetworkTopologyStatus NetworkTopologyStatus2)

            => NetworkTopologyStatus1.CompareTo(NetworkTopologyStatus2) > 0;

        #endregion

        #region Operator >= (NetworkTopologyStatus1, NetworkTopologyStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkTopologyStatus1">A network topology status.</param>
        /// <param name="NetworkTopologyStatus2">Another network topology status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (NetworkTopologyStatus NetworkTopologyStatus1,
                                           NetworkTopologyStatus NetworkTopologyStatus2)

            => NetworkTopologyStatus1.CompareTo(NetworkTopologyStatus2) >= 0;

        #endregion

        #endregion

        #region IComparable<networkTopologyStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two network topology statuss.
        /// </summary>
        /// <param name="Object">A network topology status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is NetworkTopologyStatus networkTopologyStatus
                   ? CompareTo(networkTopologyStatus)
                   : throw new ArgumentException("The given object is not a network topology status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(networkTopologyStatus)

        /// <summary>
        /// Compares two network topology statuss.
        /// </summary>
        /// <param name="networkTopologyStatus">A network topology status to compare with.</param>
        public Int32 CompareTo(NetworkTopologyStatus networkTopologyStatus)

            => String.Compare(InternalId,
                              networkTopologyStatus.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<networkTopologyStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two network topology statuss for equality.
        /// </summary>
        /// <param name="Object">A network topology status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NetworkTopologyStatus networkTopologyStatus &&
                   Equals(networkTopologyStatus);

        #endregion

        #region Equals(networkTopologyStatus)

        /// <summary>
        /// Compares two network topology statuss for equality.
        /// </summary>
        /// <param name="networkTopologyStatus">A network topology status to compare with.</param>
        public Boolean Equals(NetworkTopologyStatus networkTopologyStatus)

            => String.Equals(InternalId,
                             networkTopologyStatus.InternalId,
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
