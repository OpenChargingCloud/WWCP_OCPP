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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The common interface of all central systems channels.
    /// CSMS might have multiple channels, e.g. a SOAP and a WebSockets channel.
    /// </summary>
    public interface ICSMSChannel : ICSMSOutgoingMessages,
                                    ICSMSOutgoingMessagesEvents,
                                    ICSMSIncomingMessages,
                                    ICSMSIncomingMessagesEvents
    {

        void AddStaticRouting   (NetworkingNode_Id DestinationNodeId,
                                 NetworkingNode_Id NetworkingHubId);

        void RemoveStaticRouting(NetworkingNode_Id DestinationNodeId,
                                 NetworkingNode_Id NetworkingHubId);


        /// <summary>
        /// Start the HTTP web socket listener thread.
        /// </summary>
        void Start();

        /// <summary>
        /// Shutdown the HTTP web socket listener thread.
        /// </summary>
        /// <param name="Message">An optional shutdown message.</param>
        /// <param name="Wait">Wait until the server finally shutted down.</param>
        Task Shutdown(String?  Message   = null,
                      Boolean  Wait      = true);

    }

}
