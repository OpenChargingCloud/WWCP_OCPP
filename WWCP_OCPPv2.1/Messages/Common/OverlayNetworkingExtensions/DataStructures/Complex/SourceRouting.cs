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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A source routing.
    /// </summary>
    public class SourceRouting : IEnumerable<NetworkingNode_Id>,
                                 IEquatable<SourceRouting>,
                                 IComparable<SourceRouting>,
                                 IComparable
    {

        #region Data

        private readonly List<NetworkingNode_Id> networkingNodeIds = [];

        #endregion

        #region Properties

        /// <summary>
        /// The ordered list of networking node identifications along the source routing.
        /// </summary>
        [Mandatory]
        public IEnumerable<NetworkingNode_Id>  NetworkingNodeIds

            => networkingNodeIds;


        /// <summary>
        /// The length of the source routing.
        /// </summary>
        [Mandatory]
        public UInt16                          Length

            => (UInt16) networkingNodeIds.Count;


        /// <summary>
        /// The first networking node aka the origin of the source routing
        /// and thus often a charging station identification.
        /// </summary>
        [Optional]
        public NetworkingNode_Id               Next

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
        /// An empty source routing.
        /// </summary>
        public static SourceRouting            Empty    { get; }

            = new();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new a source routing, based on the given enumeration of networking node identifications.
        /// </summary>
        /// <param name="NetworkingNodeIds">An optional ordered list of networking node identifications along the source routing.</param>
        public SourceRouting(IEnumerable<NetworkingNode_Id>? NetworkingNodeIds = null)
        {

            if (NetworkingNodeIds is not null && NetworkingNodeIds.Any())
                networkingNodeIds.AddRange(NetworkingNodeIds);

        }

        /// <summary>
        /// Create a new a source routing, based on the given array of networking node identifications.
        /// </summary>
        /// <param name="NetworkingNodeIds">An optional ordered list of networking node identifications along the source routing.</param>
        public SourceRouting(params NetworkingNode_Id[] NetworkingNodeIds)
        {

            if (NetworkingNodeIds is not null && NetworkingNodeIds.Length > 0)
                networkingNodeIds.AddRange(NetworkingNodeIds);

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSONArray, CustomSourceRoutingParser = null)

        /// <summary>
        /// Parse the given JSON array representation of a source routing.
        /// </summary>
        /// <param name="JSONArray">The JSON array to be parsed.</param>
        /// <param name="CustomSourceRoutingParser">An optional delegate to parse custom SourceRouting JSON objects.</param>
        public static SourceRouting Parse(JArray                                      JSONArray,
                                          CustomJArrayParserDelegate<SourceRouting>?  CustomSourceRoutingParser   = null)
        {

            if (TryParse(JSONArray,
                         out var sourceRouting,
                         out var errorResponse,
                         CustomSourceRoutingParser) &&
                sourceRouting is not null)
            {
                return sourceRouting;
            }

            throw new ArgumentException("The given JSON array representation of a source routing is invalid: " + errorResponse,
                                        nameof(JSONArray));

        }

        #endregion

        #region (static) TryParse(JSONArray, out SourceRouting, out ErrorResponse, CustomSourceRoutingParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON array representation of a source routing.
        /// </summary>
        /// <param name="JSONArray">The JSON array to be parsed.</param>
        /// <param name="SourceRouting">The parsed source routing.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JArray              JSONArray,
                                       out SourceRouting?  SourceRouting,
                                       out String?         ErrorResponse)

            => TryParse(JSONArray,
                        out SourceRouting,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON array representation of a source routing.
        /// </summary>
        /// <param name="JSONArray">The JSON array to be parsed.</param>
        /// <param name="SourceRouting">The parsed source routing.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSourceRoutingParser">An optional delegate to parse custom source routing JSON objects.</param>
        public static Boolean TryParse(JArray                                      JSONArray,
                                       out SourceRouting?                          SourceRouting,
                                       out String?                                 ErrorResponse,
                                       CustomJArrayParserDelegate<SourceRouting>?  CustomSourceRoutingParser)
        {

            try
            {

                SourceRouting  = null;
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

                SourceRouting = new SourceRouting(
                                    networkingNodeIds
                                );

                if (CustomSourceRoutingParser is not null)
                    SourceRouting = CustomSourceRoutingParser(JSONArray,
                                                              SourceRouting);

                return true;

            }
            catch (Exception e)
            {
                SourceRouting  = default;
                ErrorResponse  = "The given JSON representation of a source routing is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomSourceRoutingSerializer = null)

        /// <summary>
        /// Return a JSON array representation of this object.
        /// </summary>
        /// <param name="CustomSourceRoutingSerializer">A delegate to serialize custom source routing JSON arrays.</param>
        public JArray ToJSON(CustomJArraySerializerDelegate<SourceRouting>?  CustomSourceRoutingSerializer   = null)
        {

            var jsonArray = new JArray(NetworkingNodeIds.Select(networkingNodeId => networkingNodeId.ToString()));

            return CustomSourceRoutingSerializer is not null
                       ? CustomSourceRoutingSerializer(this, jsonArray)
                       : jsonArray;

        }

        #endregion


        #region Static definitions

        /// <summary>
        /// Do not route this message over multiple hops.
        /// </summary>
        public static SourceRouting Broadcast     { get; }
            = new (NetworkingNode_Id.Broadcast);

        /// <summary>
        /// Do not route this message over multiple hops.
        /// </summary>
        public static SourceRouting Zero    { get; }
            = new (NetworkingNode_Id.Zero);

        /// <summary>
        /// Route this message to the next Charging Station Management System.
        /// </summary>
        public static SourceRouting CSMS    { get; }
            = new (NetworkingNode_Id.CSMS);

        #endregion


        #region (static) To(NetworkingNodeId)

        /// <summary>
        /// Create a new source routing to the given networking node identification.
        /// </summary>
        /// <param name="NetworkingNodeId">A networking node identification.</param>
        public static SourceRouting To(NetworkingNode_Id NetworkingNodeId)

            => new (NetworkingNodeId);


        /// <summary>
        /// Create a new source routing to the given networking node identification.
        /// </summary>
        /// <param name="NetworkingNodeId">A networking node identification.</param>
        public static SourceRouting? To(NetworkingNode_Id? NetworkingNodeId)

            => NetworkingNodeId.HasValue
                   ? new (NetworkingNodeId.Value)
                   : null;

        #endregion

        #region RemoveFirst()

        /// <summary>
        /// Remove the first networking node identification from the source routing,
        /// except when it is the final networking node identification.
        /// </summary>
        public SourceRouting RemoveFirst()

            => networkingNodeIds.Count > 1
                   ? new (networkingNodeIds.Skip(1))
                   : this;

        #endregion


        #region IEnumerable members

        public IEnumerator<NetworkingNode_Id> GetEnumerator()
            => networkingNodeIds.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => networkingNodeIds.GetEnumerator();

        #endregion

        #region Operator overloading

        #region Operator == (SourceRouting1, SourceRouting2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SourceRouting1">A source routing.</param>
        /// <param name="SourceRouting2">Another source routing.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SourceRouting? SourceRouting1,
                                           SourceRouting? SourceRouting2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SourceRouting1, SourceRouting2))
                return true;

            // If one is null, but not both, return false.
            if (SourceRouting1 is null || SourceRouting2 is null)
                return false;

            return SourceRouting1.Equals(SourceRouting2);

        }

        #endregion

        #region Operator != (SourceRouting1, SourceRouting2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SourceRouting1">A source routing.</param>
        /// <param name="SourceRouting2">Another source routing.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SourceRouting? SourceRouting1,
                                           SourceRouting? SourceRouting2)

            => !(SourceRouting1 == SourceRouting2);

        #endregion

        #region Operator <  (SourceRouting1, SourceRouting2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SourceRouting1">A source routing.</param>
        /// <param name="SourceRouting2">Another source routing.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SourceRouting? SourceRouting1,
                                          SourceRouting? SourceRouting2)
        {

            if (SourceRouting1 is null)
                throw new ArgumentNullException(nameof(SourceRouting1), "The given source routing must not be null!");

            return SourceRouting1.CompareTo(SourceRouting2) < 0;

        }

        #endregion

        #region Operator <= (SourceRouting1, SourceRouting2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SourceRouting1">A source routing.</param>
        /// <param name="SourceRouting2">Another source routing.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SourceRouting? SourceRouting1,
                                           SourceRouting? SourceRouting2)

            => !(SourceRouting1 > SourceRouting2);

        #endregion

        #region Operator >  (SourceRouting1, SourceRouting2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SourceRouting1">A source routing.</param>
        /// <param name="SourceRouting2">Another source routing.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SourceRouting? SourceRouting1,
                                          SourceRouting? SourceRouting2)
        {

            if (SourceRouting1 is null)
                throw new ArgumentNullException(nameof(SourceRouting1), "The given source routing must not be null!");

            return SourceRouting1.CompareTo(SourceRouting2) > 0;

        }

        #endregion

        #region Operator >= (SourceRouting1, SourceRouting2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SourceRouting1">A source routing.</param>
        /// <param name="SourceRouting2">Another source routing.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SourceRouting? SourceRouting1,
                                           SourceRouting? SourceRouting2)

            => !(SourceRouting1 < SourceRouting2);

        #endregion

        #endregion

        #region IComparable<SourceRouting> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two source routings.
        /// </summary>
        /// <param name="Object">A source routing to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SourceRouting sourceRouting
                   ? CompareTo(sourceRouting)
                   : throw new ArgumentException("The given object is not a source routing!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SourceRouting)

        /// <summary>
        /// Compares two source routings.
        /// </summary>
        /// <param name="Destination">A source routing to compare with.</param>
        public Int32 CompareTo(SourceRouting? SourceRouting)
        {

            if (SourceRouting is null)
                throw new ArgumentNullException(nameof(SourceRouting),
                                                "The given source routing must not be null!");

            for (var i = 0; i < Math.Min(NetworkingNodeIds.Count(), SourceRouting.NetworkingNodeIds.Count()); i++)
            {

                var c = NetworkingNodeIds.ElementAt(i).CompareTo(SourceRouting.NetworkingNodeIds.ElementAt(i));

                if (c != 0)
                    return c;

            }

            return NetworkingNodeIds.Count().CompareTo(SourceRouting.NetworkingNodeIds.Count());

        }

        #endregion

        #endregion

        #region IEquatable<SourceRouting> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two a source routing for equality..
        /// </summary>
        /// <param name="Object">Charging needs to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SourceRouting sourceRouting &&
                   Equals(sourceRouting);

        #endregion

        #region Equals(SourceRouting)

        /// <summary>
        /// Compares two a source routing for equality.
        /// </summary>
        /// <param name="Destination">Charging needs to compare with.</param>
        public Boolean Equals(SourceRouting? SourceRouting)
        {

            if (SourceRouting is null)
                return false;

            if (!NetworkingNodeIds.Count().Equals(SourceRouting.NetworkingNodeIds.Count()))
                return false;

            for (var i = 0; i < Math.Min(NetworkingNodeIds.Count(), SourceRouting.NetworkingNodeIds.Count()); i++)
            {

                if (!NetworkingNodeIds.ElementAt(i).Equals(SourceRouting.NetworkingNodeIds.ElementAt(i)))
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
