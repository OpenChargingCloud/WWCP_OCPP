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

using System.Collections;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A network path.
    /// </summary>
    public class NetworkPath : IEnumerable<NetworkingNode_Id>,
                               IEquatable<NetworkPath>,
                               IComparable<NetworkPath>,
                               IComparable
    {

        #region Data

        private readonly List<NetworkingNode_Id> networkingNodeIds = [];

        #endregion

        #region Properties

        /// <summary>
        /// The ordered list of networking node identifications along the network path.
        /// </summary>
        [Mandatory]
        public IEnumerable<NetworkingNode_Id>  NetworkingNodeIds

            => networkingNodeIds;


        /// <summary>
        /// The length of the network path.
        /// </summary>
        [Mandatory]
        public UInt16                          Length

            => (UInt16) networkingNodeIds.Count;


        /// <summary>
        /// The first networking node aka the origin of the network path
        /// and thus often a charging station identification.
        /// </summary>
        [Optional]
        public NetworkingNode_Id               Source

            => networkingNodeIds.Count > 0
                   ? networkingNodeIds.First()
                   : NetworkingNode_Id.Zero;


        /// <summary>
        /// The last networking node aka the sender of the current message.
        /// </summary>
        [Optional]
        public NetworkingNode_Id               Last

            => networkingNodeIds.Count > 0
                   ? networkingNodeIds.Last()
                   : NetworkingNode_Id.Zero;


        /// <summary>
        /// An empty network path.
        /// </summary>
        public static NetworkPath               Empty    { get; }

            = new();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new a network path, based on the given enumeration of networking node identifications.
        /// </summary>
        /// <param name="NetworkingNodeIds">An optional ordered list of networking node identifications along the network path.</param>
        public NetworkPath(IEnumerable<NetworkingNode_Id>? NetworkingNodeIds = null)
        {

            if (NetworkingNodeIds is not null && NetworkingNodeIds.Any())
                networkingNodeIds.AddRange(NetworkingNodeIds);

        }

        /// <summary>
        /// Create a new a network path, based on the given array of networking node identifications.
        /// </summary>
        /// <param name="NetworkingNodeIds">An optional ordered list of networking node identifications along the network path.</param>
        public NetworkPath(params NetworkingNode_Id[] NetworkingNodeIds)
        {

            if (NetworkingNodeIds is not null && NetworkingNodeIds.Length > 0)
                networkingNodeIds.AddRange(NetworkingNodeIds);

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSONArray, CustomNetworkPathParser = null)

        /// <summary>
        /// Parse the given JSON array representation of a network path.
        /// </summary>
        /// <param name="JSONArray">The JSON array to be parsed.</param>
        /// <param name="CustomNetworkPathParser">An optional delegate to parse custom NetworkPath JSON objects.</param>
        public static NetworkPath Parse(JArray                                    JSONArray,
                                        CustomJArrayParserDelegate<NetworkPath>?  CustomNetworkPathParser   = null)
        {

            if (TryParse(JSONArray,
                         out var networkPath,
                         out var errorResponse,
                         CustomNetworkPathParser) &&
                networkPath is not null)
            {
                return networkPath;
            }

            throw new ArgumentException("The given JSON array representation of a network path is invalid: " + errorResponse,
                                        nameof(JSONArray));

        }

        #endregion

        #region (static) TryParse(JSONArray, out NetworkPath, out ErrorResponse, CustomNetworkPathParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON array representation of a network path.
        /// </summary>
        /// <param name="JSONArray">The JSON array to be parsed.</param>
        /// <param name="NetworkPath">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JArray            JSONArray,
                                       out NetworkPath?  NetworkPath,
                                       out String?       ErrorResponse)

            => TryParse(JSONArray,
                        out NetworkPath,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON array representation of a network path.
        /// </summary>
        /// <param name="JSONArray">The JSON array to be parsed.</param>
        /// <param name="NetworkPath">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNetworkPathParser">An optional delegate to parse custom network path JSON objects.</param>
        public static Boolean TryParse(JArray                                    JSONArray,
                                       out NetworkPath?                          NetworkPath,
                                       out String?                               ErrorResponse,
                                       CustomJArrayParserDelegate<NetworkPath>?  CustomNetworkPathParser)
        {

            try
            {

                NetworkPath    = null;
                ErrorResponse  = null;

                var networkingNodeIds = new List<NetworkingNode_Id>();

                foreach (var id in JSONArray)
                {

                    if (id.Type == JTokenType.String &&
                        NetworkingNode_Id.TryParse(id?.Value<String>() ?? "", out var networkingNodeId))
                    {
                        networkingNodeIds.Add(networkingNodeId);
                    }

                    else
                    {
                        ErrorResponse = $"The given networking node identification is invalid: '{id?.Value<String>() ?? ""}'!";
                        return false;
                    }

                }

                NetworkPath = new NetworkPath(
                                  networkingNodeIds
                              );

                if (CustomNetworkPathParser is not null)
                    NetworkPath = CustomNetworkPathParser(JSONArray,
                                                          NetworkPath);

                return true;

            }
            catch (Exception e)
            {
                NetworkPath    = default;
                ErrorResponse  = "The given JSON representation of a network path is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomNetworkPathSerializer = null)

        /// <summary>
        /// Return a JSON array representation of this object.
        /// </summary>
        /// <param name="CustomNetworkPathSerializer">A delegate to serialize custom network path JSON arrays.</param>
        public JArray ToJSON(CustomJArraySerializerDelegate<NetworkPath>?  CustomNetworkPathSerializer   = null)
        {

            var jsonArray = new JArray(NetworkingNodeIds.Select(networkingNodeId => networkingNodeId.ToString()));

            return CustomNetworkPathSerializer is not null
                       ? CustomNetworkPathSerializer(this, jsonArray)
                       : jsonArray;

        }

        #endregion


        #region (static) From(NetworkingNodeId)

        /// <summary>
        /// Create a new network path from the given networking node identification.
        /// </summary>
        /// <param name="NetworkingNodeId">A networking node identification.</param>
        public static NetworkPath From(NetworkingNode_Id NetworkingNodeId)
            => new (NetworkingNodeId);

        #endregion

        #region Append(NetworkingNodeId)

        /// <summary>
        /// Append the given networking node identification to the network path, but
        /// ignore the given networking node identification if it is already the last one.
        /// </summary>
        /// <param name="NetworkingNodeId">A networking node identification.</param>
        public NetworkPath Append(NetworkingNode_Id NetworkingNodeId)

            => networkingNodeIds.Count  == 0 ||
               networkingNodeIds.Last() != NetworkingNodeId

                   ? new (networkingNodeIds.ToList().AddAndReturnList(NetworkingNodeId))

                   : this;

        #endregion


        #region IEnumerable members

        public IEnumerator<NetworkingNode_Id> GetEnumerator()
            => networkingNodeIds.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => networkingNodeIds.GetEnumerator();

        #endregion

        #region Operator overloading

        #region Operator == (NetworkPath1, NetworkPath2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkPath1">A  network path.</param>
        /// <param name="NetworkPath2">Another network path.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NetworkPath? NetworkPath1,
                                           NetworkPath? NetworkPath2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NetworkPath1, NetworkPath2))
                return true;

            // If one is null, but not both, return false.
            if (NetworkPath1 is null || NetworkPath2 is null)
                return false;

            return NetworkPath1.Equals(NetworkPath2);

        }

        #endregion

        #region Operator != (NetworkPath1, NetworkPath2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkPath1">A  network path.</param>
        /// <param name="NetworkPath2">Another network path.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NetworkPath? NetworkPath1,
                                           NetworkPath? NetworkPath2)

            => !(NetworkPath1 == NetworkPath2);

        #endregion

        #region Operator <  (NetworkPath1, NetworkPath2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkPath1">A network path.</param>
        /// <param name="NetworkPath2">Another network path.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (NetworkPath? NetworkPath1,
                                          NetworkPath? NetworkPath2)
        {

            if (NetworkPath1 is null)
                throw new ArgumentNullException(nameof(NetworkPath1), "The given network path must not be null!");

            return NetworkPath1.CompareTo(NetworkPath2) < 0;

        }

        #endregion

        #region Operator <= (NetworkPath1, NetworkPath2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkPath1">A network path.</param>
        /// <param name="NetworkPath2">Another network path.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (NetworkPath? NetworkPath1,
                                           NetworkPath? NetworkPath2)

            => !(NetworkPath1 > NetworkPath2);

        #endregion

        #region Operator >  (NetworkPath1, NetworkPath2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkPath1">A network path.</param>
        /// <param name="NetworkPath2">Another network path.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (NetworkPath? NetworkPath1,
                                          NetworkPath? NetworkPath2)
        {

            if (NetworkPath1 is null)
                throw new ArgumentNullException(nameof(NetworkPath1), "The given network path must not be null!");

            return NetworkPath1.CompareTo(NetworkPath2) > 0;

        }

        #endregion

        #region Operator >= (NetworkPath1, NetworkPath2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkPath1">A network path.</param>
        /// <param name="NetworkPath2">Another network path.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (NetworkPath? NetworkPath1,
                                           NetworkPath? NetworkPath2)

            => !(NetworkPath1 < NetworkPath2);

        #endregion

        #endregion

        #region IComparable<NetworkPath> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two network paths.
        /// </summary>
        /// <param name="Object">A network path to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is NetworkPath networkPath
                   ? CompareTo(networkPath)
                   : throw new ArgumentException("The given object is not a network path!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(NetworkPath)

        /// <summary>
        /// Compares two network paths.
        /// </summary>
        /// <param name="NetworkPath">A network path to compare with.</param>
        public Int32 CompareTo(NetworkPath? NetworkPath)
        {

            if (NetworkPath is null)
                throw new ArgumentNullException(nameof(NetworkPath),
                                                "The given network path must not be null!");

            for (var i = 0; i < Math.Min(NetworkingNodeIds.Count(), NetworkPath.NetworkingNodeIds.Count()); i++)
            {

                var c = NetworkingNodeIds.ElementAt(i).CompareTo(NetworkPath.NetworkingNodeIds.ElementAt(i));

                if (c != 0)
                    return c;

            }

            return NetworkingNodeIds.Count().CompareTo(NetworkPath.NetworkingNodeIds.Count());

        }

        #endregion

        #endregion

        #region IEquatable<NetworkPath> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two a network path for equality..
        /// </summary>
        /// <param name="Object">Charging needs to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NetworkPath networkPath &&
                   Equals(networkPath);

        #endregion

        #region Equals(NetworkPath)

        /// <summary>
        /// Compares two a network path for equality.
        /// </summary>
        /// <param name="NetworkPath">Charging needs to compare with.</param>
        public Boolean Equals(NetworkPath? NetworkPath)
        {

            if (NetworkPath is null)
                return false;

            if (!NetworkingNodeIds.Count().Equals(NetworkPath.NetworkingNodeIds.Count()))
                return false;

            for (var i = 0; i < Math.Min(NetworkingNodeIds.Count(), NetworkPath.NetworkingNodeIds.Count()); i++)
            {

                if (!NetworkingNodeIds.ElementAt(i).Equals(NetworkPath.NetworkingNodeIds.ElementAt(i)))
                    return false;

            }

            return true;

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => networkingNodeIds.CalcHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => NetworkingNodeIds.AggregateWith(", ");

        #endregion

    }

}
