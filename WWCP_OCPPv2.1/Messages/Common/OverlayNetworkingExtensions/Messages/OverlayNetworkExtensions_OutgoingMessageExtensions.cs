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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Networking node extension methods of the OCPP Overlay Network Extensions.
    /// </summary>
    public static class OverlayNetworkExtensions_OutgoingMessageExtensions
    {

        #region NotifyNetworkTopology (this NetworkingNode, NetworkTopologyInformation, ...)

        /// <summary>
        /// Send network topology information.
        /// </summary>
        /// <param name="NetworkingNode">A networking node.</param>
        /// <param name="NetworkTopologyInformation">Network topology information.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="Destination"></param>
        /// <param name="NetworkPath"></param>
        /// 
        /// <param name="SignKeys"></param>
        /// <param name="SignInfos"></param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="SerializationFormat"></param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<SentMessageResult>

            NotifyNetworkTopology(this NetworkingNode.INetworkingNode  NetworkingNode,
                                  NetworkTopologyInformation           NetworkTopologyInformation,
                                  CustomData?                          CustomData            = null,

                                  SourceRouting?                       Destination           = null,
                                  NetworkPath?                         NetworkPath           = null,

                                  IEnumerable<KeyPair>?                SignKeys              = null,
                                  IEnumerable<SignInfo>?               SignInfos             = null,
                                  IEnumerable<Signature>?              Signatures            = null,

                                  Request_Id?                          RequestId             = null,
                                  DateTimeOffset?                      RequestTimestamp      = null,
                                  EventTracking_Id?                    EventTrackingId       = null,
                                  SerializationFormats?                SerializationFormat   = null,
                                  CancellationToken                    CancellationToken     = default)


                => NetworkingNode.OCPP.OUT.NotifyNetworkTopology(
                       new NotifyNetworkTopologyMessage(

                           Destination ?? SourceRouting.CSMS,

                           NetworkTopologyInformation,

                           SignKeys,
                           SignInfos,
                           Signatures,

                           CustomData,

                           RequestId        ?? NetworkingNode.OCPP.NextRequestId,
                           RequestTimestamp ?? Timestamp.Now,
                           EventTrackingId  ?? EventTracking_Id.New,
                           NetworkPath      ?? NetworkPath.From(NetworkingNode.Id),
                           SerializationFormat,
                           CancellationToken

                       )
                   );

        #endregion

    }

}
