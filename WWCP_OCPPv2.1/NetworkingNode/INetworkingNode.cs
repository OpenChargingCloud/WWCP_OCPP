﻿/*
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

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The common interface of all networking nodes channels.
    /// NetworkingNode might have multiple channels, e.g. a SOAP and a WebSockets channel.
    /// </summary>
    public interface INetworkingNodeChannel : INetworkingNodeClient,
                                              INetworkingNodeServer
    {


    }

    /// <summary>
    /// The common interface of all networking nodes.
    /// </summary>
    public interface INetworkingNode : INetworkingNodeClient,
                                       INetworkingNodeServerLogger
    {

        NetworkingNode_Id                    Id                        { get; }

        TimeSpan                             DefaultRequestTimeout     { get; }

        Request_Id                           NextRequestId             { get; }

        SignaturePolicy?                     SignaturePolicy           { get; }


        IEnumerable<INetworkingNodeChannel>  NetworkingNodeChannels    { get; }


    }

}