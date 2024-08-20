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

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The routing of messages within the overlay network.
    /// </summary>
    public class Routing
    {

        #region Data

        private readonly ConcurrentDictionary<NetworkingNode_Id, List<Reachability>>  reachableNetworkingNodes = [];

        #endregion


        #region LookupNetworkingNode (DestinationId, out Reachability)

        public Boolean LookupNetworkingNode(NetworkingNode_Id                      DestinationId,
                                            [NotNullWhen(true)] out Reachability?  Reachability)
        {

            if (reachableNetworkingNodes.TryGetValue(DestinationId, out var reachabilityList) &&
                reachabilityList is not null &&
                reachabilityList.Count > 0)
            {

                var reachability = reachabilityList.OrderBy(entry => entry.Priority).First();

                if (reachability.NetworkingHub.HasValue)
                {

                    var visitedIds = new HashSet<NetworkingNode_Id>();

                    do
                    {

                        if (reachability.NetworkingHub.HasValue)
                        {

                            visitedIds.Add(reachability.NetworkingHub.Value);

                            if (reachableNetworkingNodes.TryGetValue(reachability.NetworkingHub.Value, out var reachability2List) &&
                                reachability2List is not null &&
                                reachability2List.Count > 0)
                            {
                                reachability = reachability2List.OrderBy(entry => entry.Priority).First();
                            }

                            // Loop detected!
                            if (reachability.NetworkingHub.HasValue && visitedIds.Contains(reachability.NetworkingHub.Value))
                                break;

                        }

                    } while (reachability.OCPPWebSocketClient is null &&
                             reachability.OCPPWebSocketServer is null);

                }

                Reachability = reachability;
                return true;

            }

            Reachability = null;
            return false;

        }

        #endregion


        #region AddOrUpdateStaticRouting     (DestinationId,  WebSocketClient,        Priority = 0, Weight = 1, Timestamp = null, Timeout = null)

        public List<Reachability>

            AddOrUpdateStaticRouting(NetworkingNode_Id     DestinationId,
                                     IOCPPWebSocketClient  WebSocketClient,
                                     Byte?                 Priority    = 0,
                                     Byte?                 Weight      = 1,
                                     DateTime?             Timestamp   = null,
                                     DateTime?             Timeout     = null)


            => reachableNetworkingNodes.AddOrUpdate(

                   DestinationId,

                   (id)                   => [
                                                 new Reachability(
                                                     DestinationId,
                                                     WebSocketClient,
                                                     Priority,
                                                     Weight,
                                                     Timeout
                                                 )
                                             ],

                   (id, reachabilityList) => {

                       var reachability = new Reachability(
                                              DestinationId,
                                              WebSocketClient,
                                              Priority,
                                              Weight,
                                              Timeout
                                          );

                       if (reachabilityList is null)
                           return [ reachability ];

                       else
                       {

                           // For thread-safety!
                           var updatedReachabilityList = new List<Reachability>();
                           updatedReachabilityList.AddRange(reachabilityList.Where(entry => entry.Priority != reachability.Priority));
                           updatedReachabilityList.Add     (reachability);

                           return updatedReachabilityList;

                       }

                   }

               );

        #endregion

        #region AddOrUpdateStaticRouting     (DestinationIds, WebSocketClient,        Priority = 0, Weight = 1, Timestamp = null, Timeout = null)

        public void AddOrUpdateStaticRouting(IEnumerable<NetworkingNode_Id>  DestinationIds,
                                             IOCPPWebSocketClient            WebSocketClient,
                                             Byte?                           Priority    = 0,
                                             Byte?                           Weight      = 1,
                                             DateTime?                       Timestamp   = null,
                                             DateTime?                       Timeout     = null)
        {

            foreach (var destinationId in DestinationIds)
                AddOrUpdateStaticRouting(
                    destinationId,
                    WebSocketClient,
                    Priority,
                    Weight,
                    Timestamp,
                    Timeout
                );

        }

        #endregion


        #region AddOrUpdateStaticRouting     (DestinationId,  WebSocketServer,        Priority = 0, Weight = 1, Timestamp = null, Timeout = null)

        public List<Reachability>

            AddOrUpdateStaticRouting(NetworkingNode_Id     DestinationId,
                                     IOCPPWebSocketServer  WebSocketServer,
                                     Byte?                 Priority    = 0,
                                     Byte?                 Weight      = 1,
                                     DateTime?             Timestamp   = null,
                                     DateTime?             Timeout     = null)


            => reachableNetworkingNodes.AddOrUpdate(

                   DestinationId,

                   (id)                   => [
                                                 new Reachability(
                                                     DestinationId,
                                                     WebSocketServer,
                                                     Priority,
                                                     Weight,
                                                     Timeout
                                                 )
                                             ],

                   (id, reachabilityList) => {

                       var reachability = new Reachability(
                                              DestinationId,
                                              WebSocketServer,
                                              Priority,
                                              Weight,   
                                              Timeout
                                          );

                       if (reachabilityList is null)
                           return [reachability];

                       else
                       {

                           // For thread-safety!
                           var updatedReachabilityList = new List<Reachability>();
                           updatedReachabilityList.AddRange(reachabilityList.Where(entry => entry.Priority != reachability.Priority));
                           updatedReachabilityList.Add(reachability);

                           return updatedReachabilityList;

                       }

                   }

               );

        #endregion

        #region AddOrUpdateStaticRouting     (DestinationIds, WebSocketClient,        Priority = 0, Weight = 1, Timestamp = null, Timeout = null)

        public void AddOrUpdateStaticRouting(IEnumerable<NetworkingNode_Id>  DestinationIds,
                                             IOCPPWebSocketServer            WebSocketServer,
                                             Byte?                           Priority    = 0,
                                             Byte?                           Weight      = 1,
                                             DateTime?                       Timestamp   = null,
                                             DateTime?                       Timeout     = null)
        {

            foreach (var destinationId in DestinationIds)
                AddOrUpdateStaticRouting(
                    destinationId,
                    WebSocketServer,
                    Priority,
                    Weight,
                    Timestamp,
                    Timeout
                );

        }

        #endregion


        #region AddOrUpdateStaticRouting     (DestinationId,  NetworkingHubId,        Priority = 0, Weight = 1, Timestamp = null, Timeout = null)

        public List<Reachability>

            AddOrUpdateStaticRouting(NetworkingNode_Id  DestinationId,
                                     NetworkingNode_Id  NetworkingHubId,
                                     Byte?              Priority    = 0,
                                     Byte?              Weight      = 1,
                                     DateTime?          Timestamp   = null,
                                     DateTime?          Timeout     = null)


            => reachableNetworkingNodes.AddOrUpdate(

                   DestinationId,

                   (id)                   => [
                                                 new Reachability(
                                                     DestinationId,
                                                     NetworkingHubId,
                                                     Priority,
                                                     Weight,
                                                     Timeout
                                                 )
                                             ],

                   (id, reachabilityList) => {

                       var reachability = new Reachability(
                                                 DestinationId,
                                                 NetworkingHubId,
                                                 Priority,
                                                 Weight,
                                                 Timeout
                                             );

                       if (reachabilityList is null)
                           return [ reachability ];

                       else
                       {

                           // For thread-safety!
                           var updatedReachabilityList = new List<Reachability>();
                           updatedReachabilityList.AddRange(reachabilityList.Where(entry => entry.Priority != reachability.Priority));
                           updatedReachabilityList.Add     (reachability);

                           return updatedReachabilityList;

                       }

                   }

               );

        #endregion

        #region AddOrUpdateStaticRouting     (DestinationIds, NetworkingHubId,        Priority = 0, Weight = 1, Timestamp = null, Timeout = null)

        public void AddOrUpdateStaticRouting(IEnumerable<NetworkingNode_Id>  DestinationIds,
                                             NetworkingNode_Id               NetworkingHubId,
                                             Byte?                           Priority    = 0,
                                             Byte?                           Weight      = 1,
                                             DateTime?                       Timestamp   = null,
                                             DateTime?                       Timeout     = null)
        {

            foreach (var destinationId in DestinationIds)
                AddOrUpdateStaticRouting(
                    destinationId,
                    NetworkingHubId,
                    Priority,
                    Weight,
                    Timestamp,
                    Timeout
                );

        }

        #endregion


        #region RemoveStaticRouting          (DestinationId,  NetworkingHubId = null, Priority = 0)

        public void RemoveStaticRouting(NetworkingNode_Id   DestinationId,
                                        NetworkingNode_Id?  NetworkingHubId   = null,
                                        Byte?               Priority          = 0)
        {

            if (!NetworkingHubId.HasValue)
            {
                reachableNetworkingNodes.TryRemove(DestinationId, out _);
                return;
            }

            if (reachableNetworkingNodes.TryGetValue(DestinationId, out var reachabilityList) &&
                reachabilityList is not null &&
                reachabilityList.Count > 0)
            {

                // For thread-safety!
                var updatedReachabilityList = new List<Reachability>(reachabilityList.Where(entry => entry.NetworkingHub == NetworkingHubId && (!Priority.HasValue || entry.Priority != (Priority ?? 0))));

                if (updatedReachabilityList.Count > 0)
                    reachableNetworkingNodes.TryUpdate(
                        DestinationId,
                        updatedReachabilityList,
                        reachabilityList
                    );

                else
                    reachableNetworkingNodes.TryRemove(DestinationId, out _);

            }

            //csmsChannel.Item1.RemoveStaticRouting(DestinationId,
            //                                      NetworkingHubId);

        }

        #endregion



        public JArray ToJSON()
        {

            var json = new JArray(
                           reachableNetworkingNodes.
                               OrderBy(kvp   => kvp.Key).
                               GroupBy(kvp   => kvp.Key).
                               Select (group => new JArray(
                                                    group.Key.ToString(),
                                                    new JArray(group.SelectMany(kvpList => new JArray(kvpList.Value.Select(reachability => reachability.ToJSON(IgnoreDestinationId: true)))))
                                                ))
                       );

            return json;

        }


    }

}
