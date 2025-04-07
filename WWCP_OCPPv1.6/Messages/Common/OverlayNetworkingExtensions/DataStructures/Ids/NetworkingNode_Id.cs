///*
// * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Affero GPL license, Version 3.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.gnu.org/licenses/agpl.html
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using org.GraphDefined.Vanaheimr.Illias;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv1_6
//{

//    /// <summary>
//    /// Extension methods for networking node identifications.
//    /// </summary>
//    public static class NetworkingNodeIdExtensions
//    {

//        /// <summary>
//        /// Indicates whether this networking node identification is null or empty.
//        /// </summary>
//        /// <param name="NetworkingNodeId">A networking node identification.</param>
//        public static Boolean IsNullOrEmpty(this NetworkingNode_Id? NetworkingNodeId)
//            => !NetworkingNodeId.HasValue || NetworkingNodeId.Value.IsNullOrEmpty;

//        /// <summary>
//        /// Indicates whether this networking node identification is null or empty.
//        /// </summary>
//        /// <param name="NetworkingNodeId">A networking node identification.</param>
//        public static Boolean IsNotNullOrEmpty(this NetworkingNode_Id? NetworkingNodeId)
//            => NetworkingNodeId.HasValue && NetworkingNodeId.Value.IsNotNullOrEmpty;

//    }


//    /// <summary>
//    /// A networking node identification.
//    /// </summary>
//    public readonly struct NetworkingNode_Id : IId,
//                                               IEquatable<NetworkingNode_Id>,
//                                               IComparable<NetworkingNode_Id>
//    {

//        #region Data

//        /// <summary>
//        /// The internal identification.
//        /// </summary>
//        private readonly String InternalId;

//        #endregion

//        #region Properties

//        /// <summary>
//        /// Indicates whether this identification is null or empty.
//        /// </summary>
//        public readonly Boolean IsNullOrEmpty
//            => InternalId.IsNullOrEmpty();

//        /// <summary>
//        /// Indicates whether this identification is NOT null or empty.
//        /// </summary>
//        public readonly Boolean IsNotNullOrEmpty
//            => InternalId.IsNotNullOrEmpty();

//        /// <summary>
//        /// The length of the networking node identification.
//        /// </summary>
//        public readonly UInt64 Length
//            => (UInt64) (InternalId?.Length ?? 0);

//        #endregion

//        #region Constructor(s)

//        /// <summary>
//        /// Create a new networking node identification based on the given string.
//        /// </summary>
//        /// <param name="String">The string representation of the networking node identification.</param>
//        private NetworkingNode_Id(String String)
//        {
//            this.InternalId  = String;
//        }

//        #endregion


//        #region (static) NewRandom(Length = 30)

//        /// <summary>
//        /// Create a new random networking node identification.
//        /// </summary>
//        /// <param name="Length">The expected length of the networking node identification.</param>
//        public static NetworkingNode_Id NewRandom(Byte Length = 30)

//            => new (RandomExtensions.RandomString(Length));

//        #endregion

//        #region (static) Parse   (Text)

//        /// <summary>
//        /// Parse the given string as networking node identification.
//        /// </summary>
//        /// <param name="Text">A text representation of a networking node identification.</param>
//        public static NetworkingNode_Id Parse(String Text)
//        {

//            if (TryParse(Text, out var networkingNodeId))
//                return networkingNodeId;

//            throw new ArgumentException($"Invalid text representation of a networking node identification: '{Text}'!",
//                                        nameof(Text));

//        }

//        #endregion

//        #region (static) TryParse(Text)

//        /// <summary>
//        /// Try to parse the given text as networking node identification.
//        /// </summary>
//        /// <param name="Text">A text representation of a networking node identification.</param>
//        public static NetworkingNode_Id? TryParse(String Text)
//        {

//            if (TryParse(Text, out var networkingNodeId))
//                return networkingNodeId;

//            return null;

//        }

//        #endregion

//        #region (static) TryParse(Text, out NetworkingNodeId)

//        /// <summary>
//        /// Try to parse the given text as networking node identification.
//        /// </summary>
//        /// <param name="Text">A text representation of a networking node identification.</param>
//        /// <param name="NetworkingNodeId">The parsed networking node identification.</param>
//        public static Boolean TryParse(String Text, out NetworkingNode_Id NetworkingNodeId)
//        {

//            Text = Text.Trim();

//            if (Text.IsNotNullOrEmpty())
//            {
//                NetworkingNodeId = new NetworkingNode_Id(Text.Trim());
//                return true;
//            }

//            NetworkingNodeId = default;
//            return false;

//        }

//        #endregion

//        #region Clone

//        /// <summary>
//        /// Clone this networking node identification.
//        /// </summary>
//        public NetworkingNode_Id Clone

//            => new (
//                   InternalId.CloneString()
//               );

//        #endregion


//        #region Static definitions

//        /// <summary>
//        /// Do not route this message over multiple hops.
//        /// </summary>
//        public static NetworkingNode_Id  Zero    { get; }
//            = new ("0");

//        /// <summary>
//        /// Route this message to the next Charging Station Management System (OCPP v1.x: Central System).
//        /// </summary>
//        public static NetworkingNode_Id  CSMS    { get; }
//            = new ("CSMS");

//        #endregion


//        #region Operator overloading

//        #region Operator == (NetworkingNodeId1, NetworkingNodeId2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="NetworkingNodeId1">A networking node identification.</param>
//        /// <param name="NetworkingNodeId2">Another networking node identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator == (NetworkingNode_Id NetworkingNodeId1,
//                                           NetworkingNode_Id NetworkingNodeId2)

//            => NetworkingNodeId1.Equals(NetworkingNodeId2);

//        #endregion

//        #region Operator != (NetworkingNodeId1, NetworkingNodeId2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="NetworkingNodeId1">A networking node identification.</param>
//        /// <param name="NetworkingNodeId2">Another networking node identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator != (NetworkingNode_Id NetworkingNodeId1,
//                                           NetworkingNode_Id NetworkingNodeId2)

//            => !NetworkingNodeId1.Equals(NetworkingNodeId2);

//        #endregion

//        #region Operator <  (NetworkingNodeId1, NetworkingNodeId2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="NetworkingNodeId1">A networking node identification.</param>
//        /// <param name="NetworkingNodeId2">Another networking node identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator < (NetworkingNode_Id NetworkingNodeId1,
//                                          NetworkingNode_Id NetworkingNodeId2)

//            => NetworkingNodeId1.CompareTo(NetworkingNodeId2) < 0;

//        #endregion

//        #region Operator <= (NetworkingNodeId1, NetworkingNodeId2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="NetworkingNodeId1">A networking node identification.</param>
//        /// <param name="NetworkingNodeId2">Another networking node identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator <= (NetworkingNode_Id NetworkingNodeId1,
//                                           NetworkingNode_Id NetworkingNodeId2)

//            => NetworkingNodeId1.CompareTo(NetworkingNodeId2) <= 0;

//        #endregion

//        #region Operator >  (NetworkingNodeId1, NetworkingNodeId2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="NetworkingNodeId1">A networking node identification.</param>
//        /// <param name="NetworkingNodeId2">Another networking node identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator > (NetworkingNode_Id NetworkingNodeId1,
//                                          NetworkingNode_Id NetworkingNodeId2)

//            => NetworkingNodeId1.CompareTo(NetworkingNodeId2) > 0;

//        #endregion

//        #region Operator >= (NetworkingNodeId1, NetworkingNodeId2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="NetworkingNodeId1">A networking node identification.</param>
//        /// <param name="NetworkingNodeId2">Another networking node identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator >= (NetworkingNode_Id NetworkingNodeId1,
//                                           NetworkingNode_Id NetworkingNodeId2)

//            => NetworkingNodeId1.CompareTo(NetworkingNodeId2) >= 0;

//        #endregion

//        #endregion

//        #region IComparable<NetworkingNodeId> Members

//        #region CompareTo(Object)

//        /// <summary>
//        /// Compares two networking node identifications.
//        /// </summary>
//        /// <param name="Object">A networking node identification to compare with.</param>
//        public Int32 CompareTo(Object? Object)

//            => Object is NetworkingNode_Id networkingNodeId
//                   ? CompareTo(networkingNodeId)
//                   : throw new ArgumentException("The given object is not a networking node identification!",
//                                                 nameof(Object));

//        #endregion

//        #region CompareTo(NetworkingNodeId)

//        /// <summary>
//        /// Compares two networking node identifications.
//        /// </summary>
//        /// <param name="NetworkingNodeId">A networking node identification to compare with.</param>
//        public Int32 CompareTo(NetworkingNode_Id NetworkingNodeId)

//            => String.Compare(InternalId,
//                              NetworkingNodeId.InternalId,
//                              StringComparison.OrdinalIgnoreCase);

//        #endregion

//        #endregion

//        #region IEquatable<NetworkingNodeId> Members

//        #region Equals(Object)

//        /// <summary>
//        /// Compares two networking node identifications for equality.
//        /// </summary>
//        /// <param name="Object">A networking node identification to compare with.</param>
//        public override Boolean Equals(Object? Object)

//            => Object is NetworkingNode_Id networkingNodeId &&
//                   Equals(networkingNodeId);

//        #endregion

//        #region Equals(NetworkingNodeId)

//        /// <summary>
//        /// Compares two networking node identifications for equality.
//        /// </summary>
//        /// <param name="NetworkingNodeId">A networking node identification to compare with.</param>
//        public Boolean Equals(NetworkingNode_Id NetworkingNodeId)

//            => String.Equals(InternalId,
//                             NetworkingNodeId.InternalId,
//                             StringComparison.OrdinalIgnoreCase);

//        #endregion

//        #endregion

//        #region (override) GetHashCode()

//        /// <summary>
//        /// Return the hash code of this object.
//        /// </summary>
//        public override Int32 GetHashCode()

//            => InternalId?.ToLower().GetHashCode() ?? 0;

//        #endregion

//        #region (override) ToString()

//        /// <summary>
//        /// Return a text representation of this object.
//        /// </summary>
//        public override String ToString()

//            => InternalId ?? "";

//        #endregion

//    }

//}
