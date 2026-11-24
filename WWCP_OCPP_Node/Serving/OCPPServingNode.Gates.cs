/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Collections.Frozen;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node.KeyPins;
using cloud.charging.open.protocols.WWCP.Node.Connections;

using OCPPv2_1                    = cloud.charging.open.protocols.OCPPv2_1;
using RequestForwardingDecision  = cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.RequestForwardingDecision;
using MessageForwardingDecision  = cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.MessageForwardingDecision;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The gates of a node the charging stations connect to: what only the CSMS asks of the node, it
    /// asks over the node's own line up to it - and what a charging station sends on goes up to the
    /// CSMS, under the station's own name, and to no other station.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A request in the frame of the overlay network names its destination and where it comes from as
    /// its sender wrote them: a station on the station port can address this node and call itself the
    /// CSMS. Without the first gate it could install an SSH key for another station's identity and sign
    /// in as that station next (InstallSSHKey), lock another station out (DeleteSSHKey), switch the key
    /// every station of the site pins (SetVariables of EVQICtrlr[TLSServer].ActiveKeyPin), change the
    /// port every station connects to (SetVariables of OCPPServerController), reset the node, or read
    /// its Device Model and its stations' keys. Without the second it could send another station a
    /// Reset, a SetVariables or a RequestStartTransaction as the CSMS, or the CSMS a TransactionEvent in
    /// another station's name.
    /// </para>
    /// <para>
    /// Decided by the connection, which the request does not carry: so at the door, and not in each of
    /// the handlers. Not by the signer's rights either, which signed requests to the node are held to:
    /// this says only that a station is not the CSMS. What a station sends on to the CSMS, and what a
    /// station may ask of the node - a DataTransfer, say - passes as before.
    /// </para>
    /// <para>
    /// A kind of node gives its OCPP 2.1 node both gates with <see cref="AttachRequestGate"/> and
    /// <see cref="AttachForwardingGate"/>, once, after it built it.
    /// </para>
    /// </remarks>
    public abstract partial class OCPPServingNode
    {

        #region (static) CSMSOnlyActions

        /// <summary>
        /// The actions only a CSMS asks of a node it manages: every request of OCPP 2.1 that a CSMS sends -
        /// the requests of the namespace OCPPv2_1.CSMS, Reset, SetVariables, InstallSSHKey and all the
        /// others, also those added later - and the networking-node requests that manage a node: its
        /// signature policies, its user roles and its files.
        /// </summary>
        /// <remarks>
        /// Found by the request types rather than listed, so that a request the library adds for the CSMS
        /// is held back here without anyone having to remember it. The few requests of that namespace a
        /// station sends - NotifyDERAlarm, say - are no loss: a station sends them to the CSMS, not to this
        /// node.
        /// </remarks>
        public static IReadOnlySet<String> CSMSOnlyActions { get; }

            = CSMSRequestTypes().
                  Concat([
                      typeof(OCPPv2_1.NetworkingNode.SetSignaturePolicyRequest),
                      typeof(OCPPv2_1.NetworkingNode.GetSignaturePolicyRequest),
                      typeof(OCPPv2_1.NetworkingNode.AddSignerRoleRequest),
                      typeof(OCPPv2_1.NetworkingNode.UpdateSignerRoleRequest),
                      typeof(OCPPv2_1.NetworkingNode.DeleteSignerRoleRequest),
                      typeof(OCPPv2_1.NetworkingNode.GetSignerRolesRequest),
                      typeof(OCPPv2_1.NetworkingNode.GetFileRequest),
                      typeof(OCPPv2_1.NetworkingNode.SendFileRequest),
                      typeof(OCPPv2_1.NetworkingNode.DeleteFileRequest),
                      typeof(OCPPv2_1.NetworkingNode.ListDirectoryRequest)
                  ]).
                  Select(type => type.Name[..^"Request".Length]).
                  ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The request types of OCPP 2.1 a CSMS sends.
        /// </summary>
        private static IEnumerable<Type> CSMSRequestTypes()
        {

            var csms = typeof(OCPPv2_1.CSMS.ResetRequest);

            return csms.Assembly.GetTypes().
                       Where(type => type.Namespace == csms.Namespace         &&
                                     type.IsClass && !type.IsAbstract         &&
                                     type.Name.EndsWith("Request", StringComparison.Ordinal) &&
                                     IsARequest(type));

        }

        private static Boolean IsARequest(Type Type)
        {

            for (var baseType = Type.BaseType; baseType is not null; baseType = baseType.BaseType)
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(OCPPv2_1.ARequest<>))
                    return true;

            return false;

        }

        #endregion


        #region (protected) AttachRequestGate(Node)

        /// <summary>
        /// Give the given OCPP 2.1 node its gate: what only the CSMS asks comes over the node's line up to
        /// it - and over a line up on probation, nothing but what may come over it.
        /// </summary>
        /// <remarks>
        /// The node's only gate: nothing else sets one.
        /// </remarks>
        /// <param name="Node">The OCPP 2.1 node of this node.</param>
        protected void AttachRequestGate(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode Node)
        {

            Node.OCPP.IN.RequestGate = (connection, action, payload) => OnlyTheCSMS(connection, action, payload) ??
                                                                        GateOnProbation(connection, action, payload);

        }

        #endregion

        #region (protected) AttachForwardingGate(Node)

        /// <summary>
        /// Give the given OCPP 2.1 node's forwarding its gate: what a charging station sends on goes up to
        /// the CSMS, under the station's own name, and to no other station.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A frame in the overlay network names where it goes and who sent it, as its sender wrote them.
        /// Forwarded as written, a station could send another station a Reset, a SetVariables or a
        /// RequestStartTransaction and call itself the CSMS, and the other station would take it for one of
        /// its CSMS's - or send the CSMS a TransactionEvent in another station's name.
        /// </para>
        /// <para>
        /// So, for what comes over a connection this node accepted: a request, or a message, whose sender
        /// is not the station of that connection is dropped - not refused, as a refusal would go to the
        /// sender it names, which did not send it. One whose destination is reached over this node's own
        /// servers, another station, is refused, and the station is told so. What goes up over the node's
        /// line to the CSMS, and everything that comes down it, passes as before - unless that line is on
        /// probation.
        /// </para>
        /// <para>
        /// A station is the one end of its path: a networking node behind a station, which would send on
        /// in the names of the nodes behind it, is not something a station of this node is.
        /// </para>
        /// </remarks>
        /// <param name="Node">The OCPP 2.1 node of this node.</param>
        protected void AttachForwardingGate(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode Node)
        {

            // The gates first, and what passes them where its target says: one
            // filter, as the forwarding takes the first filter's decision.
            Node.OCPP.FORWARD.OnAnyJSONRequestFilter       += (timestamp, sender, connection, request, cancellationToken) =>
                Task.FromResult(AsRequestDecision(WhyNotForward(Node, connection, request.Action, request.Destination, request.NetworkPath),
                                                  () => RouteByTarget(Node, connection, request.Action, request.Destination, request.NetworkPath)));

            Node.OCPP.FORWARD.OnAnyBinaryRequestFilter     += (timestamp, sender, connection, request, cancellationToken) =>
                Task.FromResult(AsRequestDecision(WhyNotForward(Node, connection, request.Action, request.Destination, request.NetworkPath),
                                                  () => RouteByTarget(Node, connection, request.Action, request.Destination, request.NetworkPath)));

            Node.OCPP.FORWARD.OnAnyJSONSendMessageFilter   += (timestamp, sender, connection, message, cancellationToken) =>
                Task.FromResult(AsMessageDecision(WhyNotForward(Node, connection, message.Action, message.Destination, message.NetworkPath),
                                                  () => RouteByTarget(Node, connection, message.Action, message.Destination, message.NetworkPath)));

            Node.OCPP.FORWARD.OnAnyBinarySendMessageFilter += (timestamp, sender, connection, message, cancellationToken) =>
                Task.FromResult(AsMessageDecision(WhyNotForward(Node, connection, message.Action, message.Destination, message.NetworkPath),
                                                  () => RouteByTarget(Node, connection, message.Action, message.Destination, message.NetworkPath)));

        }

        #endregion


        #region (protected) WhyNotForward(Node, Connection, Action, Destination, NetworkPath)

        /// <summary>
        /// What is wrong with forwarding what came over the given connection - a forged sender, another
        /// station as the destination, or a line up on probation - or nothing.
        /// </summary>
        protected (Boolean Forged, String Why)? WhyNotForward(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode  Node,
                                                             IMessageConnection                           Connection,
                                                             String                                       Action,
                                                             SourceRouting                                Destination,
                                                             NetworkPath                                  NetworkPath)
        {

            // Over the node's line to the CSMS: the CSMS's to send - unless the
            // line is on probation, or its CSMS is not believed any more.
            if (Connection.Role != ConnectionRole.Server)
                return WhyNotForwardOnProbation(Connection, Action) is String onProbation
                           ? (false, onProbation)
                           : null;

            // A line the CSMS made itself: this node accepted it, and it is
            // the CSMS's all the same - let in by what a connection of this node
            // names, and never on probation, which is for a key this node
            // learned of a server it connected to.
            if (IsInbound(Connection))
                return null;

            var station = Connection.NetworkingNodeIdOf();

            if (station is null || NetworkPath.Source != station.Value)
            {
                var why = $"The charging station '{station}' sent '{Action}' for '{Destination.Next}' as '{NetworkPath.Source}': dropped, a station sends in its own name.";
                Log.Warning(why, "ocpp", "station", "security");
                return (true, why);
            }

            // Reached over this node's own servers, and not the CSMS that
            // connected over one of them: another station.
            if (Node.Routing.LookupNetworkingNode(Destination.Next, out var reachability) &&
                reachability.Client is null &&
                !LeadsToInbound(Destination.Next))
            {
                var why = $"'{Action}' from the charging station '{station}' is not sent on to '{Destination.Next}': a charging station's requests go to the CSMS, not to another station of this {NodeNoun}.";
                Log.Warning(why, "ocpp", "station", "security");
                return (false, why);
            }

            return null;

        }

        private static RequestForwardingDecision AsRequestDecision((Boolean Forged, String Why)?            Refusal,
                                                                   Func<(SourceRouting? To, String? Refused)>  Route)
        {

            if (Refusal is { Forged: true } forged)
                return new RequestForwardingDecision(ForwardingDecisions.DROP,   LogMessage:    forged.Why);

            if (Refusal is { } refusal)
                return new RequestForwardingDecision(ForwardingDecisions.REJECT, RejectMessage: refusal.Why);

            return Route() switch {
                       { Refused: String why } => new RequestForwardingDecision(ForwardingDecisions.REJECT,  RejectMessage:  why),
                       { To:      { } to     } => new RequestForwardingDecision(ForwardingDecisions.FORWARD, NewDestination: to),
                       _                       => new RequestForwardingDecision(ForwardingDecisions.NEXT)
                   };

        }

        private static MessageForwardingDecision AsMessageDecision((Boolean Forged, String Why)?            Refusal,
                                                                   Func<(SourceRouting? To, String? Refused)>  Route)
        {

            // A message is not answered, so not refused either: dropped.
            if (Refusal is { } refusal)
                return new MessageForwardingDecision(ForwardingDecisions.DROP, LogMessage: refusal.Why);

            return Route() switch {
                       { Refused: String why } => new MessageForwardingDecision(ForwardingDecisions.DROP,    LogMessage:     why),
                       { To:      { } to     } => new MessageForwardingDecision(ForwardingDecisions.FORWARD, NewDestination: to),
                       _                       => new MessageForwardingDecision(ForwardingDecisions.NEXT)
                   };

        }

        #endregion

        #region (protected) WhyNotForwardOnProbation(Connection, Action)

        /// <summary>
        /// Why a request that came over the given connection is not passed on to a station, where the
        /// connection is on probation; null where it is not.
        /// </summary>
        protected String? WhyNotForwardOnProbation(IMessageConnection  Connection,
                                                   String              Action)

            => probations.Gate(Connection, Action, null, (probation, action, payload) =>
                   $"This {NodeNoun} believes its CSMS by the key pin {(probation.Trust as ServerKeyPinTrust)?.Learned}, which it learned on its first " +
                   $"connection and which is on probation: until a signed pin set confirms it, nothing it sends is passed on to a station ('{action}').");

        #endregion

        #region (protected) OnlyTheCSMS(Connection, Action, Payload)

        /// <summary>
        /// Why a request addressed to this node itself is not processed: where it is one of the
        /// <see cref="CSMSOnlyActions"/> and came over a connection this node accepted rather than over
        /// the one it made - or null. A connection the CSMS made itself, written down as a
        /// connection of this node, is its line up as much as one the node made - and a commissioning
        /// tool's, an account let in to speak to this node as its CSMS would (see
        /// OCPPServingNode.Commissioning.cs).
        /// </summary>
        protected String? OnlyTheCSMS(IMessageConnection  Connection,
                                      String              Action,
                                      JObject?            Payload)

            => Connection.Role == ConnectionRole.Server &&
               !IsInbound(Connection) &&
               !IsACommissioningTool(Connection) &&
               CSMSOnlyActions.Contains(Action)
                   ? $"'{Action}' is for this {NodeNoun}'s CSMS to ask, over the {NodeNoun}'s own connection to it - not for a charging station connected to the {NodeNoun}."
                   : null;

        #endregion

    }

}
