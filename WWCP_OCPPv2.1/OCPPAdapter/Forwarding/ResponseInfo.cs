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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// Information about expected OCPP responses.
    /// </summary>
    /// <param name="RequestId">The request identification the request came with; it is forwarded under another, its key here.</param>
    /// <param name="RequestContext">The JSON-LD context of the request.</param>
    /// <param name="SourceNodeId">The networking node that sent the request, where its answer goes.</param>
    /// <param name="Timeout">When an answer is no longer waited for.</param>
    /// <param name="CameOver">The connection the request came over, which its answer goes back over: whatever name its sender wrote, which may lead to another connection - two CSMS behind one gateway both call themselves "CSMS".</param>
    public class ResponseInfo(Request_Id           RequestId,
                              JSONLDContext        RequestContext,
                              NetworkingNode_Id    SourceNodeId,
                              DateTimeOffset       Timeout,
                              IMessageConnection?  CameOver   = null)
    {

        public Request_Id         RequestId         { get; } = RequestId;
        public JSONLDContext      RequestContext    { get; } = RequestContext;
        public NetworkingNode_Id  SourceNodeId      { get; } = SourceNodeId;
        public DateTimeOffset     Timeout           { get; } = Timeout;
        public IMessageConnection? CameOver         { get; } = CameOver;

        /// <summary>
        /// Told of the answer when it comes, before it goes on: its payload, or null
        /// for a request error.
        /// </summary>
        public Func<JObject?, Task>? OnAnswer       { get; init; }

        /// <summary>
        /// Whether the answer is this node's own and goes on to nobody: the answer to
        /// a request it sent on behalf of another (see SendOnBehalfOf).
        /// </summary>
        public Boolean            KeepAnswer        { get; init; }


    }

}
