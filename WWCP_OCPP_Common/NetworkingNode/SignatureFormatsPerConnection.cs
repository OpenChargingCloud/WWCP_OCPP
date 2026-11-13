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

using System.Collections.Concurrent;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPP.NetworkingNode
{

    /// <summary>
    /// Which signatures the messages of a node get, by the connection they leave over:
    /// the JWS over their JSON, the COSE_Sign1 over their CBOR, or both.
    ///
    /// A node signs before it sends, so its signature policy asks for each message
    /// (SignaturePolicy.FormatsOf), and this answers by the route of the message. The
    /// same for every version of OCPP.
    /// </summary>
    /// <param name="Routing">The routes of the node.</param>
    public sealed class SignatureFormatsPerConnection(Routing Routing)
    {

        #region Data

        // What a connection says of the signatures of the messages that leave over it:
        // by the node at its far end, and by the transport that reaches it - a client,
        // which is one connection, whatever names its far end is reached under.
        private readonly ConcurrentDictionary<NetworkingNode_Id, SignatureFormats>  signatureFormatsTo    = [];
        private readonly System.Runtime.CompilerServices.ConditionalWeakTable<Object, Object>  signatureFormatsOver  = [];

        #endregion


        #region Set(Destination, Formats)

        /// <summary>
        /// Say which signatures the messages for the given networking node get - the JWS
        /// over their JSON, the COSE_Sign1 over their CBOR, or both - or, with null, that
        /// they get what the signature policy of this node says.
        ///
        /// It is said of the next node on the way: a message for a node behind it is
        /// signed the way the connection it leaves over says.
        /// </summary>
        /// <param name="Destination">The node at the far end of a connection.</param>
        /// <param name="Formats">Which signatures are written; null for what the policy says.</param>
        public void Set(NetworkingNode_Id  Destination,
                        SignatureFormats?  Formats)
        {

            if (Formats.HasValue)
                signatureFormatsTo[Destination] = Formats.Value;

            else
                signatureFormatsTo.TryRemove(Destination, out _);

        }

        #endregion

        #region Set(Client, Formats)

        /// <summary>
        /// Say which signatures the messages that leave over the given client get, under
        /// whatever names its far end is reached - or, with null, that they get what the
        /// signature policy of this node says.
        /// </summary>
        /// <param name="Client">A client of this node: the connection to one far end.</param>
        /// <param name="Formats">Which signatures are written; null for what the policy says.</param>
        public void Set(IWWCPClient        Client,
                        SignatureFormats?  Formats)
        {

            // Kept for as long as the client is: one that was let go of takes it along.
            if (Formats.HasValue)
                signatureFormatsOver.AddOrUpdate(Client, Formats.Value);

            else
                signatureFormatsOver.Remove(Client);

        }

        #endregion

        #region For(Destination)

        /// <summary>
        /// Which signatures the messages for the given networking node get, where its
        /// connection says so - or null, where the signature policy of this node decides.
        /// </summary>
        /// <param name="Destination">The next node on the way of a message.</param>
        public SignatureFormats? For(NetworkingNode_Id Destination)
        {

            if (signatureFormatsTo.TryGetValue(Destination, out var said))
                return said;

            if (Routing.LookupNetworkingNode(Destination, out var reachability) &&
                reachability.Transport is Object transport                                     &&
                signatureFormatsOver.TryGetValue(transport, out var over))
            {
                return (SignatureFormats) over;
            }

            return null;

        }

        #endregion

        #region Of(Message)

        /// <summary>
        /// What the signature policy asks for a message it is about to sign: a request
        /// and a message without an answer leave for the next node of their destination,
        /// a response for the node its request came from.
        /// </summary>
        public SignatureFormats? Of(ISignableMessage Message)
        {

            return Message switch {
                       IRequest  request   => For(request.Destination.Next),
                       IResponse response  => response.RequestMessage is IRequest asked
                                                  ? For(asked.NetworkPath.Source)
                                                  : null,
                       IMessage  message   => For(message.Destination.Next),
                       _                   => null
                   };

        }

        #endregion



    }

}
