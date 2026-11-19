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

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Connections;

using OCPPVersion    = cloud.charging.open.protocols.WWCP.Node.Connections.OCPPVersion;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    #region (enum) RegistrationState

    /// <summary>
    /// Where an OCPP node of a node stands with its back end.
    /// </summary>
    public enum RegistrationState
    {

        /// <summary>
        /// No BootNotification sent yet: no connection was up.
        /// </summary>
        NotAsked,

        /// <summary>
        /// A BootNotification is on its way.
        /// </summary>
        Asking,

        /// <summary>
        /// The back end accepted the node.
        /// </summary>
        Accepted,

        /// <summary>
        /// The back end has not decided yet, and may ask things meanwhile.
        /// </summary>
        Pending,

        /// <summary>
        /// The back end will not have the node, for now.
        /// </summary>
        Rejected,

        /// <summary>
        /// The back end did not answer the BootNotification.
        /// </summary>
        Unanswered

    }

    #endregion

    /// <summary>
    /// Registering with the back end: the BootNotification each OCPP node of
    /// this node sends once a connection is up - whoever dialled it - and what
    /// it may say before and after. The same for a charging station, a local
    /// controller and a gateway: what is a kind's own is asked of it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// As OCPP has it. A back end that answers Pending may still ask things,
    /// and is answered. Pending and Rejected come with how long to wait before
    /// asking again; where that is 0, and where the back end did not answer at
    /// all, it is a minute. Accepted comes with how often to send a heartbeat,
    /// which is sent from then on - by the OCPP node itself, where the kind
    /// says it beats by itself (<see cref="NodeBeatsByItself"/>), and from
    /// here otherwise.
    /// </para>
    /// <para>
    /// Each connection registers on its own: after the start, when it is first
    /// up, and again when it comes back only where it was not accepted yet -
    /// one that went unanswered asks at once, one told to wait waits. A node
    /// with two connections of its version - a CSMS and its successor, during a
    /// migration - is registered with both, and each back end that accepted it
    /// gets its heartbeat at the interval it gave. What waits waits for the
    /// primary, the back end in charge, alone (see IsPrimary).
    /// </para>
    /// <para>
    /// Not on a kind that has no back end: a CSMS (see
    /// <see cref="RegistersWithABackEnd"/>).
    /// </para>
    /// </remarks>
    public abstract partial class OCPPServingNode
    {

        #region (class) Registrations

        /// <summary>
        /// The registrations of one OCPP node: one per connection.
        /// </summary>
        protected sealed class Registrations(String                         Version,
                                             OCPPv2_1.BootReason            Reason)
        {

            /// <summary>
            /// "1.6" or "2.1".
            /// </summary>
            public String                                            Version         { get; } = Version;

            /// <summary>
            /// Why the node registers: it powered up, or was built anew.
            /// </summary>
            public OCPPv2_1.BootReason                               Reason          { get; } = Reason;

            /// <summary>
            /// Cancelled once the node is let go of: nothing is asked for it any more.
            /// </summary>
            public CancellationTokenSource                           Ended           { get; } = new();

            /// <summary>
            /// By the identification of the connection.
            /// </summary>
            public ConcurrentDictionary<String, Registration>        ByConnection    { get; } = new();

        }

        #endregion

        #region (class) Registration

        /// <summary>
        /// Where one connection of an OCPP node stands with its back end, and the
        /// next time it asks.
        /// </summary>
        protected sealed class Registration
        {

            private readonly Func<ConnectionEntry, NetworkingNode_Id> destinationOf;

            public Registration(Registrations                             Node,
                                ConnectionEntry                           Connection,
                                Boolean                                   Primary,
                                Func<ConnectionEntry, NetworkingNode_Id>  DestinationOf)
            {
                this.Version        = Node.Version;
                this.Reason         = Node.Reason;
                this.Ended          = Node.Ended;
                this.Connection     = Connection;
                this.Primary        = Primary;
                this.destinationOf  = DestinationOf;
            }

            public String                   Version      { get; }
            public OCPPv2_1.BootReason      Reason       { get; }

            /// <summary>
            /// The connection, whose line on the connections page says how it stands.
            /// </summary>
            public ConnectionEntry          Connection   { get; }

            /// <summary>
            /// Whether its back end is the one in charge, whose acceptance lets go of what waits.
            /// </summary>
            public Boolean                  Primary      { get; }

            /// <summary>
            /// Where its BootNotification and its heartbeats go: the connection's own name,
            /// or in the overlay network the CSMS's - see WWCPNode.DestinationOf.
            /// </summary>
            public NetworkingNode_Id        Destination
                => destinationOf(Connection);

            public RegistrationState        State        { get; set; } = RegistrationState.NotAsked;
            public DateTimeOffset?          Since        { get; set; }

            /// <summary>
            /// Accepted, how often a heartbeat goes; otherwise how long it waits before asking again.
            /// </summary>
            public TimeSpan?                Interval     { get; set; }
            public DateTimeOffset?          NextTry      { get; set; }
            public String?                  Problem      { get; set; }

            /// <summary>
            /// The node's: cancelled once it is let go of.
            /// </summary>
            public CancellationTokenSource  Ended        { get; }

            /// <summary>
            /// The BootNotification on its way, if one is: cancelled when another
            /// one takes its place.
            /// </summary>
            public CancellationTokenSource? Asking       { get; set; }

            /// <summary>
            /// How many were asked: only the answer to the last one counts.
            /// </summary>
            public Int32                    Asks         { get; set; }

            /// <summary>
            /// Whether it was ended - its connection hung up - and is no longer
            /// one of its node's: an answer taken after says nothing of how the
            /// connection stands. Set and read under a lock of it.
            /// </summary>
            public Boolean                  Gone         { get; set; }

            /// <summary>
            /// The heartbeat sent from here, where the node does not send it
            /// itself: cancelled when it changes.
            /// </summary>
            public CancellationTokenSource? Beating      { get; set; }

            /// <summary>
            /// What this back end is being told, one after the other.
            /// </summary>
            public Task                     Telling      { get; set; } = Task.CompletedTask;

        }

        #endregion

        #region Data

        /// <summary>
        /// How long to wait before asking again where the back end gave no time -
        /// a Pending or a Rejected with an interval of 0 - or did not answer.
        /// </summary>
        public static readonly TimeSpan  DefaultRegistrationRetry   = TimeSpan.FromMinutes(1);

        /// <summary>
        /// The shortest heartbeat interval kept to, whatever a back end asks for.
        /// </summary>
        public static readonly TimeSpan  ShortestHeartbeatInterval  = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The registrations of each OCPP node, one per connection.
        /// </summary>
        protected readonly ConditionalWeakTable<AWWCPNetworkingNode, Registrations>  registrations  = new();

        /// <summary>
        /// For tests: awaited once a BootNotification over the given connection
        /// was answered, before the answer is taken - to have something happen
        /// in between that otherwise happens there only by chance.
        /// </summary>
        public Func<ConnectionEntry, Task>?  WhenABootNotificationIsAnswered { get; set; }

        #endregion


        #region (protected virtual) What a kind of node says of its registration

        /// <summary>
        /// Whether this node registers with a back end at all: yes, unless the
        /// kind says otherwise - a CSMS has none.
        /// </summary>
        protected virtual Boolean RegistersWithABackEnd
            => true;

        /// <summary>
        /// Whether the given OCPP node sends its heartbeats to the primary by
        /// itself, once it was accepted: no, unless the kind says otherwise -
        /// then they are sent from here, as they are to every other back end.
        /// </summary>
        protected virtual Boolean NodeBeatsByItself(AWWCPNetworkingNode Node)
            => false;

        /// <summary>
        /// Hold back, or let go of, what the given OCPP node keeps for its back
        /// end until it was accepted. Nothing, unless the kind keeps something.
        /// </summary>
        protected virtual void HoldWhatWaits(AWWCPNetworkingNode  Node,
                                             Boolean              Hold)
        { }

        /// <summary>
        /// What the kind tells a back end that accepted the given OCPP node -
        /// on the acceptance, and whenever the connection comes back to one that
        /// had accepted it. Nothing, unless the kind says something: a charging
        /// station, how its EVSEs stand.
        /// </summary>
        protected virtual void WhenAccepted(AWWCPNetworkingNode  Node,
                                            Registration         Registration)
        { }

        /// <summary>
        /// Who the given OCPP node of OCPP 2.1 says it is in its BootNotification.
        /// What the node says of itself, unless the kind says more.
        /// </summary>
        protected virtual OCPPv2_1.ChargingStation BootNotificationIdentity(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode Node)

            => new (
                   Kind.Product,
                   "Open Charging Cloud",
                   null,
                   Version
               );

        /// <summary>
        /// Send the BootNotification of the given OCPP node to the given back
        /// end, and say what came of it. OCPP 2.1, as every node speaks it,
        /// unless the kind says otherwise - one with a node of OCPP 1.6 sends
        /// that one's itself.
        /// </summary>
        /// <param name="Node">The OCPP node that registers.</param>
        /// <param name="Destination">Where it goes: the connection's own name.</param>
        /// <param name="Reason">Why it registers.</param>
        /// <param name="CancellationToken">Cancelled where it no longer counts.</param>
        protected virtual async Task<(Boolean Answered, String Status, TimeSpan Interval, String? Problem)>

            SendBootNotification(AWWCPNetworkingNode  Node,
                                 NetworkingNode_Id    Destination,
                                 OCPPv2_1.BootReason  Reason,
                                 CancellationToken    CancellationToken)

        {

            if (Node is not OCPPv2_1.NetworkingNode.AOCPPNetworkingNode node21)
                return (false, "", TimeSpan.Zero, $"'{Node.Id}' is no OCPP node this {NodeNoun} knows how to register.");

            var response = await node21.OCPP.OUT.BootNotification(
                                     new OCPPv2_1.CS.BootNotificationRequest(
                                         Destination:        SourceRouting.To(Destination),
                                         ChargingStation:    BootNotificationIdentity(node21),
                                         Reason:             Reason,
                                         NetworkPath:        NetworkPath.From(node21.Id),
                                         CancellationToken:  CancellationToken
                                     )
                                 );

            return response.Result.ResultCode == ResultCode.OK
                       ? (true,  response.Status.ToString(), response.Interval, null)
                       : (false, "", TimeSpan.Zero, response.Result.Description ?? response.Result.ResultCode.ToString());

        }

        /// <summary>
        /// Send a heartbeat of the given OCPP node to the given back end.
        /// OCPP 2.1, as every node speaks it, unless the kind says otherwise.
        /// </summary>
        protected virtual async Task SendHeartbeat(AWWCPNetworkingNode  Node,
                                                   NetworkingNode_Id    Destination,
                                                   CancellationToken    CancellationToken)
        {

            if (Node is OCPPv2_1.NetworkingNode.AOCPPNetworkingNode node21)
                await node21.OCPP.OUT.Heartbeat(
                          new OCPPv2_1.CS.HeartbeatRequest(
                              Destination:        SourceRouting.To(Destination),
                              NetworkPath:        NetworkPath.From(node21.Id),
                              CancellationToken:  CancellationToken
                          )
                      );

        }

        #endregion


        #region RegistrationJSON()

        /// <summary>
        /// Where each OCPP node stands with its back end: the primary's
        /// registration, by the version of OCPP it speaks - "1.6" where this
        /// node speaks it, and "2.1". Empty where it registers with nobody.
        /// </summary>
        public JObject RegistrationJSON()
        {

            var json = new JObject();

            if (!RegistersWithABackEnd)
                return json;

            if (Node16 is { } node16)
                json.Add(new JProperty("1.6", RegistrationJSON(PrimaryRegistrationOf(node16))));

            json.Add(new JProperty("2.1", RegistrationJSON(PrimaryRegistrationOf(Node21))));

            return json;

        }

        private static JObject RegistrationJSON(Registration? Registration)

            => new (
                   new JProperty("state",     (Registration?.State ?? RegistrationState.NotAsked).ToString()),
                   new JProperty("since",     Registration?.Since?.  ToString("o")),
                   new JProperty("interval",  Registration?.Interval?.TotalSeconds),
                   new JProperty("nextTry",   Registration?.NextTry?.ToString("o")),
                   new JProperty("problem",   Registration?.Problem)
               );

        #endregion

        #region (protected) NewRegistrations(Reason, Nodes)

        /// <summary>
        /// OCPP nodes just built, none of them registered, for the given reason:
        /// what they keep for their back end waits until it accepts them. A node
        /// nobody said this of registers as one that powered up.
        /// </summary>
        protected void NewRegistrations(OCPPv2_1.BootReason           Reason,
                                        params AWWCPNetworkingNode[]  Nodes)
        {
            foreach (var node in Nodes)
            {
                registrations.AddOrUpdate(node, new Registrations(VersionOf(node), Reason));
                HoldWhatWaits(node, true);
            }
        }

        private String VersionOf(AWWCPNetworkingNode Node)

            => Node is OCPPv1_6.NetworkingNode.AOCPPNetworkingNode
                   ? "1.6"
                   : "2.1";

        #endregion

        #region (protected) RegistrationsOf(Node) / RegistrationOf(Node, Connection) / PrimaryRegistrationOf(Node) / NodeOf(Connection)

        protected Registrations RegistrationsOf(AWWCPNetworkingNode Node)

            => registrations.GetValue(Node, node => new Registrations(VersionOf(node), OCPPv2_1.BootReason.PowerUp));

        /// <summary>
        /// The registration of the given connection with the given node: made
        /// when the connection is first up.
        /// </summary>
        protected Registration RegistrationOf(AWWCPNetworkingNode  Node,
                                              ConnectionEntry      Connection)
        {

            var nodeRegistrations = RegistrationsOf(Node);

            return nodeRegistrations.ByConnection.GetOrAdd(
                       Connection.Id,
                       _ => new Registration(nodeRegistrations, Connection, IsPrimary(Connection), DestinationOf)
                   );

        }

        protected Registration? PrimaryRegistrationOf(AWWCPNetworkingNode Node)

            => registrations.TryGetValue(Node, out var nodeRegistrations)
                   ? nodeRegistrations.ByConnection.Values.FirstOrDefault(registration => registration.Primary)
                   : null;

        /// <summary>
        /// The OCPP node a connection is of: the one of OCPP 1.6 where it
        /// speaks that and there is one, the one of OCPP 2.1 otherwise.
        /// </summary>
        protected AWWCPNetworkingNode NodeOf(ConnectionEntry Connection)

            => Connection.OCPPVersion == OCPPVersion.OCPP1_6 && Node16 is { } node16
                   ? node16
                   : Node21;

        #endregion

        #region (protected override) EndRegistrations(Nodes) / EndRegistrationOf(Connection)

        /// <summary>
        /// Nothing more is asked for these nodes: they were let go of, or this
        /// node is stopping.
        /// </summary>
        protected internal override void EndRegistrations(params AWWCPNetworkingNode?[] Nodes)
        {
            foreach (var node in Nodes)
                if (node is not null && registrations.TryGetValue(node, out var nodeRegistrations))
                    nodeRegistrations.Ended.Cancel();
        }

        /// <summary>
        /// Nothing more is asked of the back end over the given connection, nor
        /// waited for: no BootNotification, no heartbeat.
        /// </summary>
        protected override void EndRegistrationOf(ConnectionEntry Connection)
        {

            if (registrations.TryGetValue(NodeOf(Connection), out var nodeRegistrations) &&
                nodeRegistrations.ByConnection.TryRemove(Connection.Id, out var registration))
            {

                // An answer on its way is taken as one to a registration gone.
                lock (registration)
                    registration.Gone = true;

                StopTheHeartbeat(registration);
                registration.Asking?.Cancel();

            }

        }

        #endregion


        #region (protected override) ConnectionIsUp(Connection)

        /// <summary>
        /// A connection is up: send what waits where it leads to the primary and
        /// that one accepted this node, and register where its back end did not
        /// accept it yet.
        /// </summary>
        protected internal override void ConnectionIsUp(ConnectionEntry Connection)
        {

            if (!RegistersWithABackEnd)
                return;

            var node          = NodeOf(Connection);
            var registration  = RegistrationOf(node, Connection);

            switch (registration.State)
            {

                case RegistrationState.Accepted:
                    if (registration.Primary)
                        SendWhatWaits(Connection);

                    // What changed while the connection was down was told nobody.
                    WhenAccepted(node, registration);
                    return;

                // Told to wait: Rejected may not ask before its time is up,
                // and Pending is asked again when it is.
                case RegistrationState.Pending or RegistrationState.Rejected
                    when registration.NextTry > TimeProvider.GetUtcNow():
                    return;

                // Not asked yet, not answered - or asked over a connection that
                // was lost before the answer came, which is not waited for.
                default:
                    _ = RegisterAsync(node, registration, ByTheClock: false);
                    return;

            }

        }

        #endregion

        #region (protected) RegisterAsync(Node, Registration, ByTheClock, Triggered = false)

        /// <summary>
        /// Send the node's BootNotification, and do what the answer says.
        /// </summary>
        /// <remarks>
        /// Never for a node let go of or one already accepted. A connection
        /// that comes up asks anew, and the answer to what was asked before -
        /// over a connection that is gone - no longer counts; the clock asks
        /// only where nothing is on its way. Nothing here throws: a back end
        /// that cannot be asked is asked again later.
        /// </remarks>
        /// <param name="Node">The node to register.</param>
        /// <param name="Registration">Where it stands.</param>
        /// <param name="ByTheClock">Whether the time to ask again is up, rather than a connection having come up.</param>
        /// <param name="Triggered">Whether it was asked for - a TriggerMessage, the page, the command line: sent although the back end accepted this node already, and with that reason.</param>
        protected async Task RegisterAsync(AWWCPNetworkingNode  Node,
                                           Registration         Registration,
                                           Boolean              ByTheClock,
                                           Boolean              Triggered = false)
        {

            CancellationTokenSource asking;
            Int32                   ask;

            lock (Registration)
            {

                if (Registration.Ended.IsCancellationRequested ||
                    (Registration.State == RegistrationState.Accepted && !Triggered) ||
                    (ByTheClock && Registration.State == RegistrationState.Asking))
                    return;

                Registration.Asking?.Cancel();

                asking                = CancellationTokenSource.CreateLinkedTokenSource(Registration.Ended.Token);
                ask                   = ++Registration.Asks;

                Registration.Asking   = asking;
                Registration.State    = RegistrationState.Asking;
                Registration.NextTry  = null;

            }

            Boolean StillCounts()
            {
                lock (Registration)
                    return ask == Registration.Asks && !Registration.Ended.IsCancellationRequested && !Registration.Gone;
            }

            try
            {

                var (answered, status, interval, problem) = await SendBootNotification(
                                                                      Node,
                                                                      Registration.Destination,
                                                                      Triggered
                                                                          ? OCPPv2_1.BootReason.Triggered
                                                                          : Registration.Reason,
                                                                      asking.Token
                                                                  );

                if (WhenABootNotificationIsAnswered is Func<ConnectionEntry, Task> hook)
                    await hook(Registration.Connection);

                if (!StillCounts())
                    return;

                if (!answered)
                {
                    Unanswered(Node, Registration, problem ?? "no answer");
                    return;
                }

                Registration.Since    = TimeProvider.GetUtcNow();
                Registration.Problem  = null;

                if (status == "Accepted")
                {
                    Accepted(Node, Registration, interval);
                    return;
                }

                var wait = interval > TimeSpan.Zero ? interval : DefaultRegistrationRetry;

                var what = $"{BackEndOf(Registration)} has not accepted this {NodeNoun}'s OCPP {Registration.Version} node yet ({status}); " +
                           $"it asks again in {wait.TotalSeconds:F0} s, and tells it nothing else until then.";

                // Ended since it was asked whether it still counts: not said.
                lock (Registration)
                {

                    if (Registration.Gone)
                        return;

                    Registration.State     = status == "Pending" ? RegistrationState.Pending : RegistrationState.Rejected;
                    Registration.Interval  = wait;

                    Note(Registration.Connection, $"Connected, but {what}");

                }

                StopTheHeartbeat(Registration);

                if (Registration.State == RegistrationState.Rejected)
                    Log.Warning($"Registering: {what}", "ocpp", "connections");
                else
                    Log.Info   ($"Registering: {what}", "ocpp", "connections");

                AskAgain(Node, Registration, wait);

            }
            catch (Exception e)
            {
                if (StillCounts())
                    Unanswered(Node, Registration, e.Message);
            }
            finally
            {

                lock (Registration)
                    if (Registration.Asking == asking)
                        Registration.Asking = null;

                asking.Dispose();

            }

        }

        #endregion

        #region (private) Accepted(Node, Registration, Interval)

        /// <summary>
        /// The back end accepted the node. The primary: what waited is let go
        /// and sent. Heartbeats go from then on, at the interval it gave - by
        /// the node itself to the primary, where the kind says so.
        /// </summary>
        private void Accepted(AWWCPNetworkingNode  Node,
                              Registration         Registration,
                              TimeSpan             Interval)
        {

            var every = Interval >= ShortestHeartbeatInterval ? Interval : ShortestHeartbeatInterval;

            var what  = $"{BackEndOf(Registration)} accepted this {NodeNoun}'s OCPP {Registration.Version} node, " +
                        $"which sends it a heartbeat every {every.TotalSeconds:F0} s.";

            // Ended since it was asked whether it still counts - its connection
            // hung up, an endpoint gone into reserve: it is not said to be
            // connected, nor sent heartbeats.
            lock (Registration)
            {

                if (Registration.Gone)
                    return;

                Registration.State     = RegistrationState.Accepted;
                Registration.Interval  = Interval;

                Log.Notice($"Registering: {what}", "ocpp", "connections");

                Note(Registration.Connection, $"Connected; {what}");

            }

            if (Registration.Primary)
            {
                HoldWhatWaits(Node, false);
                SendWhatWaits(Registration.Connection);
            }

            if (!Registration.Primary || !NodeBeatsByItself(Node))
                StartTheHeartbeat(Node, Registration, every);

            WhenAccepted(Node, Registration);

        }

        #endregion

        #region (private) StartTheHeartbeat(Node, Registration, Every) / (protected static) StopTheHeartbeat(Registration)

        /// <summary>
        /// Send heartbeats to the given back end, where the node does not.
        /// </summary>
        /// <remarks>
        /// One that cannot be sent - the connection is down for the moment - is
        /// not sent, and the next one is tried at its time.
        /// </remarks>
        private void StartTheHeartbeat(AWWCPNetworkingNode  Node,
                                       Registration         Registration,
                                       TimeSpan             Every)
        {

            CancellationTokenSource beating;

            lock (Registration)
            {
                Registration.Beating?.Cancel();
                beating = Registration.Beating = CancellationTokenSource.CreateLinkedTokenSource(Registration.Ended.Token);
            }

            _ = Task.Run(async () => {

                    while (!beating.IsCancellationRequested)
                    {

                        try
                        {
                            await Task.Delay(Every, TimeProvider, beating.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }

                        try
                        {
                            await SendHeartbeat(Node, Registration.Destination, beating.Token);
                        }
                        catch (Exception e) when (e is not OperationCanceledException)
                        {
                            Log.Debug($"A heartbeat to {BackEndOf(Registration)} could not be sent: {e.Message}", "ocpp", "connections");
                        }

                    }

                });

        }

        protected static void StopTheHeartbeat(Registration Registration)
        {
            lock (Registration)
            {
                Registration.Beating?.Cancel();
                Registration.Beating = null;
            }
        }

        #endregion

        #region (protected static) BackEndOf(Registration)

        /// <summary>
        /// Who a registration is with, for whoever reads the log.
        /// </summary>
        protected static String BackEndOf(Registration Registration)

            => Registration.Primary
                   ? "the CSMS"
                   : $"the back end of '{Registration.Connection.Description}'";

        #endregion

        #region (private) Unanswered(Node, Registration, Problem)

        /// <summary>
        /// The BootNotification got no answer: asked again in a minute, or as
        /// soon as a connection comes back.
        /// </summary>
        private void Unanswered(AWWCPNetworkingNode  Node,
                                Registration         Registration,
                                String               Problem)
        {

            Registration.State     = RegistrationState.Unanswered;
            Registration.Interval  = DefaultRegistrationRetry;
            Registration.Problem   = Problem;

            StopTheHeartbeat(Registration);

            Log.Warning($"Registering: {BackEndOf(Registration)} did not answer the BootNotification of this {NodeNoun}'s OCPP {Registration.Version} node " +
                        $"({Problem}); it asks again in {DefaultRegistrationRetry.TotalSeconds:F0} s.",
                        "ocpp", "connections");

            AskAgain(Node, Registration, DefaultRegistrationRetry);

        }

        #endregion

        #region (private) AskAgain(Node, Registration, After)

        private void AskAgain(AWWCPNetworkingNode  Node,
                              Registration         Registration,
                              TimeSpan             After)
        {

            Registration.NextTry = TimeProvider.GetUtcNow() + After;

            _ = Task.Run(async () => {

                    try
                    {
                        await Task.Delay(After, TimeProvider, Registration.Ended.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }

                    await RegisterAsync(Node, Registration, ByTheClock: true);

                });

        }

        #endregion

    }

}
