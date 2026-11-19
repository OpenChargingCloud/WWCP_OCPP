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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.SSH;

using OCPPv1_6 = cloud.charging.open.protocols.OCPPv1_6;
using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

using cloud.charging.open.protocols.OCPP.SSH;
using cloud.charging.open.protocols.WWCP.SSH;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node.Web;
using cloud.charging.open.protocols.WWCP.Node.SecureShell;
using cloud.charging.open.protocols.WWCP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.Serving;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// Commissioning over SSH: a tool speaking OCPP to this node on the node's
    /// own SSH server, as its back end would - to a charging station, a local
    /// controller, a gateway and a CSMS alike.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>One subsystem, "ocpp@open.charging.cloud", and one handler.</b> Who
    /// signed in says what comes: an account of the node that may run
    /// <see cref="CommissioningResource"/> - the system administrator, and on
    /// a charging station the installer, unless the file says otherwise - is a
    /// commissioning tool; one of the WebSocket logins with an SSH key is a
    /// charging station, where this node serves stations over SSH (see
    /// OCPPServingNode.OCPPServer.cs). The node's SSH server has let either in
    /// by its key; the handler hands each to the servers of its kind, and
    /// those to the OCPP node of the version its HELLO chose.
    /// </para>
    /// <para>
    /// <b>The tool is a networking node of its own</b>, named by its account:
    /// it does not connect as the CSMS, nor as this node. A route is one per
    /// destination, and the newest connection to a destination takes it: a
    /// tool connecting as the CSMS would take the route to the CSMS away from
    /// the line up, and with it everything this node sends its CSMS. Under a
    /// name of its own, the tool gets the answers to its requests, and the
    /// line up keeps everything else.
    /// </para>
    /// <para>
    /// <b>What it asks is for this node.</b> A frame of plain OCPP names no
    /// destination, and what comes over a connection the node accepted is on
    /// its way to the CSMS - a charging station's, below a local controller. A
    /// tool's connection is read as the node's line up instead (see
    /// LineUpExtensions): what it asks, this node answers, whatever is below
    /// it.
    /// </para>
    /// </remarks>
    public abstract partial class OCPPServingNode
    {

        #region Data

        /// <summary>
        /// The resource of the role model an account needs a right to for
        /// commissioning over SSH: read, whether it is served and where;
        /// edited, switched on or off; run, the tool let in.
        /// </summary>
        public const String  CommissioningResource  = "commissioning";

        /// <summary>
        /// What a tool's connection carries, beside its being a line up: that
        /// it is a commissioning tool's.
        /// </summary>
        public const String  CommissioningToolKey   = "ocpp.commissioningTool";

        /// <summary>
        /// The commissioning server of the OCPP 2.1 node there is now: built
        /// with every node, so that a node rebuilt has one too.
        /// </summary>
        private OCPPSshServer?  commissioningServer;

        /// <summary>
        /// The commissioning server of the OCPP 1.6 node, where there is one:
        /// a tool that speaks OCPP 1.6 is handed to it by its HELLO (white
        /// paper "OCPP over SSH", decision 13).
        /// </summary>
        private OCPPSshServer?  commissioningServer16;

        /// <summary>
        /// Whether commissioning over SSH is served now.
        /// </summary>
        private volatile Boolean commissioningServed;

        /// <summary>
        /// One change of the commissioning section at a time.
        /// </summary>
        private readonly SemaphoreSlim commissioningLock = new (1, 1);

        /// <summary>
        /// The subsystem on the node's SSH server now - for the stations, the
        /// tools or both - or null where it serves neither.
        /// </summary>
        private SSHSubsystem?  ocppSubsystem;

        private readonly Object ocppSubsystemLock = new ();

        #endregion

        #region Properties

        /// <summary>
        /// What the configuration file says about commissioning over SSH.
        /// </summary>
        public CommissioningConfiguration      Commissioning          { get; private set; } = new ();

        /// <summary>
        /// Whether commissioning over SSH is served now.
        /// </summary>
        public Boolean                         CommissioningServed
            => commissioningServed;

        /// <summary>
        /// What an account needs to be let in to it.
        /// </summary>
        public static Permission               CommissioningPermission
            => Permission.Run(CommissioningResource);

        /// <summary>
        /// Where a commissioning tool connects - the node's SSH server, as its
        /// own account - or null where it cannot: not served, or no SSH server.
        /// </summary>
        public String?                         CommissioningURL
            => commissioningServed ? SSHURL : null;

        /// <summary>
        /// The commissioning tools connected now, by the names they connected
        /// under.
        /// </summary>
        public IEnumerable<NetworkingNode_Id>  CommissioningTools
            => [ .. commissioningServer?.  ConnectedNetworkingNodeIds ?? [],
                 .. commissioningServer16?.ConnectedNetworkingNodeIds ?? [] ];

        #endregion


        #region (private) AttachCommissioning()

        /// <summary>
        /// The commissioning servers of the OCPP nodes this node has now,
        /// which the nodes stop when they are stopped: attached, and served
        /// only once the subsystem is on the node's SSH server and serves
        /// accounts.
        /// </summary>
        private void AttachCommissioning()
        {

            // OCPP 2.1 as JSON, and as CBOR for a tool that offers it. JSON
            // first, so that a tool that offers both is spoken to as before.
            commissioningServer = Node21.AttachOCPPSshSubsystem(Subprotocols: [ "ocpp2.1", "ocpp2.0.1", OCPPv2_1.Version.WebSocketSubProtocolIdCBOR ]);

            HoldToTheTools(commissioningServer, Node21.Id);

            // And OCPP 1.6 where this node speaks it.
            if (Node16 is { } node16)
            {
                commissioningServer16 = node16.AttachOCPPSshSubsystem(Subprotocols: [ "ocpp1.6", OCPPv1_6.Version.WebSocketSubProtocolIdCBOR ]);
                HoldToTheTools(commissioningServer16, node16.Id);
            }

            else
                commissioningServer16 = null;

        }

        #endregion

        #region (private) HoldToTheTools(Server, NodeId)

        /// <summary>
        /// A commissioning server lets in what is served to it and nothing
        /// else: an account, while commissioning is served, under a name that
        /// is its own - and reads what the tool asks as asked of this node.
        /// </summary>
        /// <param name="Server">A commissioning server.</param>
        /// <param name="NodeId">The name of the OCPP node it is attached to.</param>
        private void HoldToTheTools(OCPPSshServer      Server,
                                    NetworkingNode_Id  NodeId)
        {

            Server.MessageServer.ValidateHello = (messageServer, context, hello, subprotocol, cancellationToken) =>
                                                     ValueTask.FromResult(ToolVerdict(Server, context, hello, NodeId));

            WireCommissioningLogging(Server);

        }

        /// <summary>
        /// Whether the tool that sent the given HELLO is served, and as what.
        /// </summary>
        private SshHelloVerdict ToolVerdict(OCPPSshServer        Server,
                                            SshSubsystemContext  Context,
                                            SshMessageHello      Hello,
                                            NetworkingNode_Id    NodeId)
        {

            if (!commissioningServed)
                return SshHelloVerdict.Refuse(WWCPSshServer.RefusalNotPermitted, $"Commissioning over SSH is not served by this {Kind.Name}.");

            // Handed here by the one handler of the subsystem, and asked all
            // the same: what is let in here is let in as this node's back end.
            if (SSHPrincipalFor(Context.Session).Kind != SSHPrincipal.Account)
                return SshHelloVerdict.Refuse(WWCPSshServer.RefusalNotPermitted, $"'{Context.Session.Username}' is no account of this {Kind.Name}.");

            if (WhyNoCommissioningToolMayBe(Context.Session.Username, NodeId) is String why)
                return SshHelloVerdict.Refuse(WWCPSshServer.RefusalNotPermitted, why);

            var verdict = Server.Validate(Context.Session.Username, Hello);

            if (!verdict.Accepted)
                return verdict;

            var customData = new Dictionary<String, Object?>(verdict.CustomData ?? new Dictionary<String, Object?>()) {
                                 [LineUpExtensions.LineUpKey]  = true,
                                 [CommissioningToolKey]        = true
                             };

            return SshHelloVerdict.Accept(verdict.WelcomeProperties, customData);

        }

        #endregion

        #region (static) IsACommissioningTool(Connection)

        /// <summary>
        /// Whether the given connection is a commissioning tool's: an account
        /// this node let in to speak to it as its CSMS would.
        /// </summary>
        /// <param name="Connection">A connection this node accepted.</param>
        public static Boolean IsACommissioningTool(org.GraphDefined.Vanaheimr.Hermod.Messaging.IMessageConnection Connection)

            => Connection.TryGetCustomData(CommissioningToolKey) is true;

        #endregion

        #region (static) WhyNoCommissioningToolMayBe(Login, NodeId)

        /// <summary>
        /// Why a tool may not connect under the given login, or null where it
        /// may: not as the CSMS, which would take the line up's route, and not
        /// as this node, nor as no node at all.
        /// </summary>
        /// <param name="Login">The SSH login - the name of an account.</param>
        /// <param name="NodeId">The name of the OCPP node it is connecting to.</param>
        public static String? WhyNoCommissioningToolMayBe(String             Login,
                                                          NetworkingNode_Id  NodeId)
        {

            if (!NetworkingNode_Id.TryParse(Login, out var tool))
                return null;    // refused by the server's own check, in its own words

            if (tool == NetworkingNode_Id.CSMS || tool == NetworkingNode_Id.CentralSystem)
                return $"'{Login}' is what this node calls its CSMS - a commissioning tool connects under a name of its own.";

            if (tool == NetworkingNode_Id.Broadcast || tool == NetworkingNode_Id.Zero)
                return $"'{Login}' names no single networking node.";

            if (tool == NodeId)
                return $"'{Login}' is this node's own name.";

            return null;

        }

        #endregion

        #region (private) WireCommissioningLogging(Server)

        /// <summary>
        /// Who connected for commissioning and went away, in the event log.
        /// Who signed in, or was refused at the door, the node's SSH server
        /// says itself.
        /// </summary>
        private void WireCommissioningLogging(OCPPSshServer Server)
        {

            Server.OnNetworkingNodeConnectionAccepted += (timestamp, transport, connection, networkingNodeId, networkingMode, eventTrackingId, cancellationToken) => {

                Log.Notice(
                    $"'{networkingNodeId?.ToString() ?? connection.RemoteSocket.ToString()}' is connected for commissioning over SSH from {connection.RemoteSocket}" +
                    $" ({connection.Subprotocol ?? "no subprotocol"}).",
                    "ocpp", "ssh", "commissioning"
                );

                return Task.CompletedTask;

            };

            Server.OnNetworkingNodeConnectionClosed += (timestamp, transport, connection, networkingNodeId, close, cancellationToken) => {

                Log.Notice(
                    $"'{networkingNodeId?.ToString() ?? connection.RemoteSocket.ToString()}' went away from commissioning over SSH" +
                    $" ({(close.ByPeer ? "it closed" : "closed here")}{(close.Code.HasValue ? $", {close.Code}" : "")}{(close.Reason.IsNullOrEmpty() ? "" : $": {close.Reason}")}).",
                    "ocpp", "ssh", "commissioning"
                );

                return Task.CompletedTask;

            };

        }

        #endregion


        #region (private) OCPPSubsystemHandler / ServeTheOCPPSubsystem(Anew = false)

        /// <summary>
        /// The one handler of the subsystem: an account to the commissioning
        /// servers, a WebSocket login to the servers of the stations - each
        /// pair of which hands it on by the version its HELLO chose.
        /// </summary>
        private SshSubsystemHandler OCPPSubsystemHandler

            => (context, cancellationToken) => {

                   var (first, second) = SSHPrincipalFor(context.Session).Kind == SSHPrincipal.Account
                                             ? (commissioningServer, commissioningServer16)
                                             : (ocppSshServer,       ocppSshServer16);

                   return first is null
                              ? ValueTask.CompletedTask
                              : second is null
                                    ? first.Handler(context, cancellationToken)
                                    : SshMessageServer.RoutedBySubprotocol(first.MessageServer, second.MessageServer)(context, cancellationToken);

               };

        /// <summary>
        /// The subsystem on the node's SSH server while it serves the stations
        /// or the tools, and gone where it serves neither. Where it stays, who
        /// may no longer have it is asked at once - a station when OCPP over
        /// SSH was switched off, a tool when commissioning was - and ended.
        /// </summary>
        /// <param name="Anew">Put on anew, the sessions of the one before ending: after the OCPP nodes were rebuilt.</param>
        /// <returns>Why it could not be put on, or null.</returns>
        private String? ServeTheOCPPSubsystem(Boolean Anew = false)
        {

            lock (ocppSubsystemLock)
            {

                if (!ocppOverSSHServed && !commissioningServed)
                {

                    if (ocppSubsystem is not null)
                    {
                        ocppSubsystem = null;
                        RemoveSSHSubsystem(OCPPSshServer.OCPPSubsystem);
                    }

                    return null;

                }

                if (ocppSubsystem is not null && !Anew)
                {
                    RecheckSSHSessions();
                    return null;
                }

                if (ocppSubsystem is not null)
                {
                    ocppSubsystem = null;
                    RemoveSSHSubsystem(OCPPSshServer.OCPPSubsystem);
                }

                var subsystem = new SSHSubsystem(
                                    OCPPSshServer.OCPPSubsystem,
                                    OCPPOverSSHLabel,
                                    OCPPSubsystemHandler,
                                    CommissioningPermission,
                                    () => commissioningServed
                                              ? null
                                              : $"commissioning over SSH is not served by this {Kind.Name}"
                                );

                try
                {
                    AddSSHSubsystem(subsystem);
                    ocppSubsystem = subsystem;
                    return null;
                }
                catch (Exception e)
                {
                    return e.Message;
                }

            }

        }

        #endregion


        #region (protected) StartCommissioning() / StopCommissioning(Why)

        /// <summary>
        /// Serve commissioning over SSH from the start, where the file says so.
        /// </summary>
        protected void StartCommissioning()
        {
            if (Commissioning.IsEnabled)
                ServeCommissioning();
        }

        /// <summary>
        /// Serve commissioning over SSH no longer: every tool connected is let
        /// go of, and the subsystem goes where it serves no station either.
        /// </summary>
        /// <param name="Why">Why, for the log.</param>
        protected void StopCommissioning(String Why)
        {

            if (!commissioningServed)
                return;

            commissioningServed = false;

            ServeTheOCPPSubsystem();

            Log.Info($"Commissioning over SSH is no longer served: {Why}.", "ocpp", "ssh", "commissioning");

        }

        #endregion

        #region (private) ServeCommissioning()

        /// <summary>
        /// Serve commissioning over SSH from now on: the subsystem on the
        /// node's SSH server, for the accounts that may run it.
        /// </summary>
        /// <remarks>
        /// Said out loud where it cannot work: switched on with the SSH server
        /// off is a door painted on a wall.
        /// </remarks>
        private void ServeCommissioning()
        {

            if (commissioningServer is null || commissioningServed)
                return;

            // Before the subsystem is there, so that a tool asking for it the
            // moment it is finds itself let in.
            commissioningServed = true;

            if (ServeTheOCPPSubsystem() is String problem)
            {

                commissioningServed = false;

                Log.Critical($"Commissioning over SSH could not be served: {problem}", "ocpp", "ssh", "commissioning");
                return;

            }

            if (!SSHEnabled)
                Log.Warning(
                    $"Commissioning over SSH is switched on, but the SSH server of this {Kind.Name} is off - so no tool can reach it. " +
                    "Switch SSH on under Configuration.",
                    "ocpp", "ssh", "commissioning"
                );

            else
                Log.Notice(
                    $"A commissioning tool may speak {String.Join(", ", CommissioningSubprotocols)} to this {Kind.Name} over SSH: " +
                    $"{SSHURL} as an account that may {CommissioningPermission}, subsystem {OCPPSshServer.OCPPSubsystem}, host key {SSHHostKey ?? "not known yet"}.",
                    "ocpp", "ssh", "commissioning"
                );

        }

        #endregion

        #region (private) CommissioningSubprotocols

        /// <summary>
        /// What a tool may speak to this node.
        /// </summary>
        private IEnumerable<String> CommissioningSubprotocols

            => (commissioningServer?.  MessageServer.Options.Subprotocols ?? []).
                   Concat(commissioningServer16?.MessageServer.Options.Subprotocols ?? []);

        #endregion


        #region CommissioningJSON() / TryUpdateCommissioningConfiguration(JSON, out Error [, out NotSaved])

        /// <summary>
        /// Commissioning over SSH, as the web interface reads it: what the
        /// file says, whether it is served, where a tool connects and who is
        /// connected.
        /// </summary>
        public JObject CommissioningJSON()

            => new (
                   new JProperty("enabled",       Commissioning.IsEnabled),
                   new JProperty("served",        commissioningServed),
                   new JProperty("sshServer",     SSHEnabled),
                   new JProperty("url",           SSHEnabled ? SSHURL : null),
                   new JProperty("hostKey",       SSHHostKey),
                   new JProperty("subsystem",     OCPPSshServer.OCPPSubsystem),
                   new JProperty("subprotocols",  new JArray(CommissioningSubprotocols)),
                   new JProperty("node",          Node21.Id.ToString()),
                   new JProperty("permission",    CommissioningPermission.ToString()),
                   new JProperty("tools",         new JArray(CommissioningTools.Select(tool => tool.ToString()).Order())),
                   new JProperty("file",          ConfigFile.Path)
               );

        /// <summary>
        /// Switch commissioning over SSH on or off.
        /// </summary>
        public Boolean TryUpdateCommissioningConfiguration(JObject                           JSON,
                                                           [NotNullWhen(false)] out String?  Error)

            => TryUpdateCommissioningConfiguration(JSON, out Error, out _);

        /// <summary>
        /// Switch commissioning over SSH on or off: written to the file, then
        /// served or not from that moment, without a restart. Switched off,
        /// every tool connected is let go of.
        /// </summary>
        /// <param name="JSON">The new section.</param>
        /// <param name="Error">What is wrong with it, or what stood in the way.</param>
        /// <param name="NotSaved">True where the configuration file could not be read or written: nothing about the change was wrong, and nothing was changed.</param>
        public Boolean TryUpdateCommissioningConfiguration(JObject                           JSON,
                                                           [NotNullWhen(false)] out String?  Error,
                                                           out Boolean                       NotSaved)
        {

            NotSaved = false;

            if (!CommissioningConfiguration.TryParse(JSON, out var wanted, out Error))
                return false;

            commissioningLock.Wait();

            try
            {

                if (wanted == Commissioning)
                    return true;

                if (!ConfigFile.TryReplaceSection(CommissioningConfiguration.SectionName, wanted.ToJSON(), out Error))
                {
                    NotSaved = true;
                    return false;
                }

                Commissioning = wanted;

                if (wanted.IsEnabled)
                    ServeCommissioning();
                else
                    StopCommissioning("switched off");

                return true;

            }
            finally
            {
                commissioningLock.Release();
            }

        }

        #endregion

    }

}
