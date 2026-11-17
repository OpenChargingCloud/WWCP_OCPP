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
using System.Globalization;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using QueuedMessage       = cloud.charging.open.protocols.OCPP.QueuedMessage;
using OfflineQueue        = cloud.charging.open.protocols.OCPP.OfflineQueue;
using ResultCode          = cloud.charging.open.protocols.OCPP.ResultCode;
using Outboxes            = cloud.charging.open.protocols.OCPP.Outboxes;
using CertificateSources  = cloud.charging.open.protocols.OCPP.CertificateSources;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.WebSockets;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A node of OCPP 2.1 with a Device Model of its own that is no charging
    /// station - a local controller, a gateway: its components, which a CSMS
    /// reads and writes with GetVariables, SetVariables, GetReport and
    /// GetBaseReport addressed to this node, and what it tells its CSMS of
    /// itself - the events of monitors, and security events.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The same Device Model a charging station has (<see cref="IDeviceModelNode"/>), so that the
    /// components and the bridges that work with a station's - EVQICtrlr, OCPPServerController, the
    /// DNS and time controllers of WWCP_OCPP_Node - are added to such a node by the same calls.
    /// </para>
    /// <para>
    /// It answers requests for this node only, once <see cref="AnswerFromOwnDeviceModel"/> was
    /// called. A request for a station behind the node never meets it: the adapter hands it to
    /// FORWARD by its destination before any handler runs, and it reaches the station as it was
    /// sent, to be answered by the station's own Device Model under the station's own rules.
    /// </para>
    /// <para>
    /// Such a node has no EVSEs: a component at an EVSE is none of its.
    /// </para>
    /// </remarks>
    public abstract class ADeviceModelNode : AOCPPNetworkingNode,
                                             IDeviceModelNode
    {

        #region Data

        /// <summary>
        /// The longest techInfo of a security event (OCPP 2.1, SecurityEventNotificationRequest):
        /// a longer one is cut there - the whole of it belongs in the node's own log.
        /// </summary>
        public const Int32 MaxTechInfoLength = 255;

        /// <summary>
        /// The longest type of a security event.
        /// </summary>
        public const Int32 MaxSecurityEventTypeLength = 50;

        /// <summary>
        /// The security event that tells a back end how many messages waiting for it were let go.
        /// </summary>
        public  const String QueueOverflow      = "QueueOverflow";

        private const String securityEventKind  = "SecurityEventNotification";
        private const String notifyEventKind    = "NotifyEvent";

        /// <summary>
        /// The actions such a node decides variable by variable for its own Device Model, and not as a
        /// whole: what is written, read and reported is a variable of a component, and the component
        /// says which resource it belongs to. A charging station decides SetVariableMonitoring so as
        /// well; such a node sets no monitors over OCPP yet.
        /// </summary>
        public static readonly IReadOnlyList<String> ActionsDecidedPerEntry = [
                                                                                 "SetVariables",
                                                                                 "GetVariables",
                                                                                 "GetBaseReport",
                                                                                 "GetReport"
                                                                             ];

        private readonly List<INewInstancesSource>  newInstancesSources  = [];
        private          Outboxes?                  queues;
        private          DateTimeOffset             nextMonitorsCheck;

        /// <summary>
        /// The reports of this node's own Device Model accepted by GetBaseReport or GetReport, until
        /// their response is sent: the NotifyReport requests follow it. Keyed by the request itself.
        /// </summary>
        private readonly ConcurrentDictionary<Object, IReadOnlyList<ReportData>> pendingReports = new (ReferenceEqualityComparer.Instance);

        #endregion

        #region Properties

        /// <summary>
        /// Where the certificates a CSMS installs, lists and deletes at this node are kept, by their
        /// use: in memory, and by a source of their own for the types one was added for.
        /// </summary>
        public CertificateSources  CertificateSources           { get; } = new();

        /// <summary>
        /// The monitors the CSMS set on this node's variables.
        /// </summary>
        public VariableMonitors    VariableMonitors             { get; set; } = new();

        /// <summary>
        /// The security events and monitor events of this node not yet taken by the CSMS: in memory,
        /// unless a queue kept in a file is given.
        /// </summary>
        public OfflineQueue        OfflineQueue                 { get; set; } = new();

        /// <summary>
        /// How often the node reads the variables its monitors watch by itself, beside whenever it is
        /// told one may have changed: a value read live changes without anybody saying so.
        /// </summary>
        public TimeSpan            MonitorsCheckEvery           { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// The largest number of report data in one NotifyReport request of this node's own report; a
        /// longer report is sent in several parts.
        /// </summary>
        public UInt16              ReportDataPerNotifyReport    { get; set; } = 100;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new node of OCPP 2.1 with a Device Model of its own.
        /// </summary>
        public ADeviceModelNode(NetworkingNode_Id  Id,
                                I18NString?        Description                 = null,
                                CustomData?        CustomData                  = null,

                                SignaturePolicy?   SignaturePolicy             = null,
                                SignaturePolicy?   ForwardingSignaturePolicy   = null,

                                HTTPExtAPI?        HTTPExtAPI                  = null,
                                WebSocketServer?   ControlWebSocketServer      = null,

                                Boolean            DisableSendHeartbeats       = false,
                                TimeSpan?          SendHeartbeatsEvery         = null,
                                TimeSpan?          DefaultRequestTimeout       = null,

                                Boolean            DisableMaintenanceTasks     = false,
                                TimeSpan?          MaintenanceEvery            = null,
                                IDNSClient?        DNSClient                   = null,

                                TimeProvider?      Clock                       = null)

            : base(Id,
                   Description,
                   CustomData,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   HTTPExtAPI,
                   ControlWebSocketServer,

                   DisableSendHeartbeats,
                   SendHeartbeatsEvery,
                   DefaultRequestTimeout,

                   DisableMaintenanceTasks,
                   MaintenanceEvery,
                   DNSClient,
                   Clock)

        { }

        #endregion


        #region AddComponent        (Component)

        /// <summary>
        /// Add the given component to this node's own Device Model, unless a component with the same name
        /// and instance is already there - EVQICtrlr, say.
        /// </summary>
        /// <param name="Component">A component configuration.</param>
        public Boolean AddComponent(ComponentConfig Component)

            => Component.EVSE is null &&
               OCPP.TryAddComponentConfig(Component);

        #endregion

        #region RemoveComponent     (Name, Instance = null, EVSE = null)

        /// <summary>
        /// Remove the component with the given name and instance from this node's own Device Model.
        /// </summary>
        /// <param name="Name">The case insensitive name of the component.</param>
        /// <param name="Instance">The optional case insensitive instance of the component.</param>
        /// <param name="EVSE">The optional EVSE of the component: this node has none.</param>
        public Boolean RemoveComponent(String   Name,
                                       String?  Instance   = null,
                                       EVSE?    EVSE       = null)

            => EVSE is null &&
               OCPP.TryRemoveComponentConfig(Name, Instance);

        #endregion

        #region GetComponentConfigs (Name, EVSE = null)

        /// <summary>
        /// The components of the given name of this node's own Device Model; none at an EVSE.
        /// </summary>
        /// <param name="Name">The case insensitive name of the component.</param>
        /// <param name="EVSE">The optional EVSE of the component: this node has none.</param>
        public IEnumerable<ComponentConfig> GetComponentConfigs(String  Name,
                                                                EVSE?   EVSE   = null)

            => EVSE is null && OCPP.TryGetComponentConfig(Name, out var componentConfigs)
                   ? componentConfigs
                   : [];

        #endregion

        #region AllComponentConfigs

        /// <summary>
        /// All components of this node's own Device Model, as they are at this moment.
        /// </summary>
        public IEnumerable<ComponentConfig> AllComponentConfigs

            => [.. OCPP.ComponentConfigs];

        #endregion

        #region AddNewInstancesSource(Source)

        /// <summary>
        /// Let the given source make the instances of its components that a SetVariables request names
        /// and this node does not have yet - see <see cref="INewInstancesSource"/>.
        /// </summary>
        /// <param name="Source">A source of new instances.</param>
        public void AddNewInstancesSource(INewInstancesSource Source)
        {
            lock (newInstancesSources)
            {
                if (!newInstancesSources.Contains(Source))
                    newInstancesSources.Add(Source);
            }
        }

        #endregion

        #region NewInstanceOf       (Component, Request)

        /// <summary>
        /// The instance the given SetVariables request makes of the given component, from the first
        /// source of new instances of that component that makes one - or null where none does.
        /// </summary>
        /// <param name="Component">A component, with an instance this node does not have.</param>
        /// <param name="Request">The whole request.</param>
        public ComponentConfig? NewInstanceOf(Component            Component,
                                              SetVariablesRequest  Request)
        {

            INewInstancesSource[] sources;

            lock (newInstancesSources)
            {
                sources = [.. newInstancesSources];
            }

            return sources.Where (source => source.NewInstanceComponents.Contains(Component.Name, StringComparer.OrdinalIgnoreCase)).
                           Select(source => source.NewInstance(Component, Request)).
                           FirstOrDefault(made => made is not null);

        }

        #endregion


        #region CheckMonitorsAsync(Which = null)

        /// <summary>
        /// Read every monitored variable - those the filter lets through, where one is given - and send
        /// the events their monitors make of what changed: through the queue, so that none is lost while
        /// the CSMS is out of reach. Answers how many there were.
        /// </summary>
        /// <param name="Which">Which monitored variables to read; all where null.</param>
        public async Task<Int32> CheckMonitorsAsync(Func<Component, Variable, Boolean>? Which = null)
        {

            var events = VariableMonitors.Check((name, evse) => GetComponentConfigs(name, evse), Now, Which);

            if (events.Count > 0)
                await QueueEventsAsync(events);

            return events.Count;

        }

        #endregion

        #region QueueSecurityEventAsync(Type, TechInfo = null, Timestamp = null, Destination = null)

        /// <summary>
        /// Send a security event of this node, kept until the CSMS took it.
        /// </summary>
        /// <param name="Type">What happened.</param>
        /// <param name="TechInfo">What a technician needs to know of it: cut at <see cref="MaxTechInfoLength"/> characters.</param>
        /// <param name="Timestamp">When it happened; now where null.</param>
        /// <param name="Destination">Whom to tell; the CSMS where null.</param>
        public Task QueueSecurityEventAsync(SecurityEventType    Type,
                                            String?              TechInfo      = null,
                                            DateTimeOffset?      Timestamp     = null,
                                            NetworkingNode_Id?   Destination   = null)
        {

            if (Type.Text.Length > MaxSecurityEventTypeLength)
                throw new ArgumentException($"The type of a security event has at most {MaxSecurityEventTypeLength} characters.", nameof(Type));

            var payload = new JObject(
                              new JProperty("type",       Type.Text),
                              new JProperty("timestamp",  (Timestamp ?? Now).ToString("o"))
                          );

            if (TechInfo is not null)
                payload["techInfo"] = TechInfo.Length > MaxTechInfoLength ? TechInfo[..MaxTechInfoLength] : TechInfo;

            Queues.QueueFor(Destination ?? Queues.CSMS).Enqueue(securityEventKind, payload, Evidence: true);

            return FlushOfflineQueueAsync(Destination);

        }

        #endregion

        #region QueueEventsAsync(EventData, Destination = null)

        /// <summary>
        /// Send events in one NotifyEvent, kept until the CSMS took it.
        /// </summary>
        public Task QueueEventsAsync(IEnumerable<EventData>  EventData,
                                     NetworkingNode_Id?       Destination   = null)
        {

            var events = EventData.ToArray();

            if (events.Length == 0)
                return Task.CompletedTask;

            Queues.QueueFor(Destination ?? Queues.CSMS).Enqueue(notifyEventKind,
                                 new JObject(new JProperty("eventData", new JArray(events.Select(eventData => eventData.ToJSON())))),
                                 Evidence: false);

            return FlushOfflineQueueAsync(Destination);

        }

        #endregion

        #region FlushOfflineQueueAsync(Destination = null)

        /// <summary>
        /// Send what waits for the given destination - the CSMS where null - oldest first, until it does
        /// not take a message. Answers how many were taken.
        /// </summary>
        /// <param name="Destination">Whom to tell; the CSMS where null.</param>
        public Task<Int32> FlushOfflineQueueAsync(NetworkingNode_Id? Destination = null)

            => Queues.FlushAsync(Destination ?? Queues.CSMS);

        #endregion


        #region (protected override) DoMaintenanceAsync(State)

        /// <summary>
        /// Try the queues again, and read the monitored variables, where it is time.
        /// </summary>
        protected override async Task DoMaintenanceAsync(Object? State)
        {

            await base.DoMaintenanceAsync(State);

            // Nothing is made where nothing was ever queued or monitored.
            if (queues is not null)
                foreach (var destination in queues.DueForRetry())
                    _ = Task.Run(async () => {
                            try
                            {
                                await FlushOfflineQueueAsync(destination);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogException(e, $"'{Id}': the offline queue of '{destination}'");
                            }
                        });

            if (Now >= nextMonitorsCheck && VariableMonitors.All.Count > 0)
            {

                nextMonitorsCheck = Now + MonitorsCheckEvery;

                _ = Task.Run(async () => {
                        try
                        {
                            await CheckMonitorsAsync();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogException(e, $"'{Id}': checking its monitors");
                        }
                    });

            }

        }

        #endregion


        #region (private) Queues

        /// <summary>
        /// A queue for each destination this node tells things to: the one of its CSMS is <see cref="OfflineQueue"/>.
        /// </summary>
        private Outboxes Queues

            => LazyInitializer.EnsureInitialized(
                   ref queues,
                   () => new Outboxes(
                             DefaultNextHopNetworkingNodeId,
                             () => OfflineQueue,
                             TrySendAsync,
                             TellOfOverflowAsync,
                             Clock
                         )
               );

        #endregion

        #region (private) TrySendAsync(Message, Destination)

        /// <summary>
        /// Send one message of the queue, answering whether the CSMS took it. A message of a kind nobody
        /// sends any more counts as taken, and goes.
        /// </summary>
        private async Task<Boolean> TrySendAsync(QueuedMessage      Message,
                                                 NetworkingNode_Id  Destination)
        {

            try
            {

                switch (Message.Kind)
                {

                    case securityEventKind:
                        var security = await SendSecurityEventNotificationAsync(
                                                 SecurityEventType.Parse(Message.Payload.Value<String>("type") ?? ""),
                                                 DateTimeOffset.Parse(Message.Payload.Value<String>("timestamp") ?? "", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).UtcDateTime,
                                                 Message.Payload.Value<String>("techInfo"),
                                                 SourceRouting.To(Destination)
                                             );
                        return security.Result.ResultCode == ResultCode.OK;

                    case notifyEventKind:
                        var notify   = await OCPP.OUT.NotifyEvent(
                                                 new NotifyEventRequest(
                                                     SourceRouting.To(Destination),
                                                     Now.UtcDateTime,
                                                     0,
                                                     [.. (Message.Payload["eventData"] as JArray ?? []).OfType<JObject>().Select(json => OCPPv2_1.EventData.Parse(json))],
                                                     RequestId:          OCPP.NextRequestId,
                                                     RequestTimestamp:   Now,
                                                     RequestTimeout:     OCPP.DefaultRequestTimeout,
                                                     EventTrackingId:    EventTracking_Id.New,
                                                     NetworkPath:        NetworkPath.From(Id)
                                                 )
                                             );
                        return notify.Result.ResultCode == ResultCode.OK;

                    default:
                        return true;

                }

            }
            catch (Exception e)
            {
                DebugX.LogException(e, $"'{Id}': sending a queued {Message.Kind}");
                return false;
            }

        }

        #endregion

        #region (private) TellOfOverflowAsync(Dropped, Destination)

        /// <summary>
        /// Tell the given destination how many messages of its queue were let go, for room or for age,
        /// with a security event "QueueOverflow" - answering whether it took it.
        /// </summary>
        private async Task<Boolean> TellOfOverflowAsync(Int64              Dropped,
                                                        NetworkingNode_Id  Destination)
        {

            try
            {

                var response = await SendSecurityEventNotificationAsync(
                                         SecurityEventType.Parse(QueueOverflow),
                                         Now.UtcDateTime,
                                         $"{Dropped} message(s) waiting for this back end were let go, for room or for age, while it could not be reached.",
                                         SourceRouting.To(Destination)
                                     );

                return response.Result.ResultCode == ResultCode.OK;

            }
            catch (Exception e)
            {
                DebugX.LogException(e, $"'{Id}': telling '{Destination}' of messages let go");
                return false;
            }

        }

        #endregion


        #region (protected) AnswerFromOwnDeviceModel()

        /// <summary>
        /// Answer GetBaseReport, GetReport, GetVariables and SetVariables addressed to this node itself
        /// from its own Device Model - each entry decided by the signature policy by the resource of its
        /// component - and send the NotifyReport requests of an accepted report once its response is out.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Two paths that never meet. OCPP.IN sees only the requests addressed to this node itself - the
        /// adapter compares the destination with this node's id (or an anycast id it accepts) before any
        /// handler runs - and they are answered here. OCPP.FORWARD sees only the requests for others, and
        /// the node hands them on as its own filters say: to the station they name, which answers them
        /// from its own Device Model under its own rules.
        /// </para>
        /// <para>
        /// Who may read, set and be told which of the node's own variables is decided variable by
        /// variable, by the resource of each one's component, as a charging station decides its own: the
        /// policy table decides none of these requests as a whole. A report leaves out what the signers
        /// may not read, and one of which nothing is left is an EmptyResultSet. In the mode "Off" of the
        /// policy, the default, everything is answered as before.
        /// </para>
        /// <para>
        /// Each entry of a SetVariables request reaches the source of its component together with the
        /// whole request - its sender by the network path, and its signatures - so that a source can check
        /// the right a variable needs: EVQICtrlr[TLSServer].ActiveKeyPin is meant to need
        /// "certificates:edit" ("Signed OCPP Messages and Signer Roles", Q12).
        /// </para>
        /// <para>
        /// Called once, from the constructor of the node that answers so.
        /// </para>
        /// </remarks>
        protected void AnswerFromOwnDeviceModel()
        {

            foreach (var action in ActionsDecidedPerEntry)
                OCPP.SignaturePolicy.ActionsDecidedPerEntry.Add(action);

            #region GetBaseReport

            OCPP.IN.OnGetBaseReport += (timestamp,
                                        sender,
                                        connection,
                                        request,
                                        ct) => {

                var (status, reportData) = DeviceModel.BaseReport(
                                               request,
                                               AllComponentConfigs,
                                               OCPP.SignaturePolicy
                                           );

                // The report follows the response, see OnGetBaseReportResponseSent.
                if (status == GenericDeviceModelStatus.Accepted)
                    pendingReports[request] = reportData;

                return Task.FromResult(
                           new GetBaseReportResponse(
                               Request:      request,
                               Status:       status,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            OCPP.OUT.OnGetBaseReportResponseSent += (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     response,
                                                     runtime,
                                                     sentMessageResult,
                                                     ct) => {

                SendPendingReport(
                    request,
                    (Int32) request.GetBaseReportRequestId,
                    SourceRouting.To(request.NetworkPath.Source),
                    sentMessageResult
                );

                return Task.CompletedTask;

            };

            #endregion

            #region GetReport

            OCPP.IN.OnGetReport += (timestamp,
                                    sender,
                                    connection,
                                    request,
                                    ct) => {

                var (status, reportData) = DeviceModel.Report(
                                               request,
                                               AllComponentConfigs,
                                               OCPP.SignaturePolicy
                                           );

                // The report follows the response, see OnGetReportResponseSent.
                if (status == GenericDeviceModelStatus.Accepted)
                    pendingReports[request] = reportData;

                return Task.FromResult(
                           new GetReportResponse(
                               Request:      request,
                               Status:       status,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            OCPP.OUT.OnGetReportResponseSent += (timestamp,
                                                 sender,
                                                 connection,
                                                 request,
                                                 response,
                                                 runtime,
                                                 sentMessageResult,
                                                 ct) => {

                SendPendingReport(
                    request,
                    request.GetReportRequestId,
                    SourceRouting.To(request.NetworkPath.Source),
                    sentMessageResult
                );

                return Task.CompletedTask;

            };

            #endregion

            #region GetVariables

            OCPP.IN.OnGetVariables += (timestamp,
                                       sender,
                                       connection,
                                       request,
                                       ct) =>

                Task.FromResult(
                    new GetVariablesResponse(
                        Request:     request,
                        Results:     DeviceModel.GetVariables(
                                         request,
                                         (name, evse) => GetComponentConfigs(name, evse),
                                         OCPP.SignaturePolicy
                                     ),
                        CustomData:  null
                    )
                );

            #endregion

            #region SetVariables

            OCPP.IN.OnSetVariables += async (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             ct) =>

                new SetVariablesResponse(
                    Request:             request,
                    SetVariableResults:  await DeviceModel.SetVariablesAsync(
                                                   request,
                                                   (name, evse) => GetComponentConfigs(name, evse),
                                                   OCPP.SignaturePolicy,
                                                   ct,
                                                   NewInstanceOf
                                               ),
                    CustomData:          null
                );

            #endregion

        }

        #endregion

        #region (private) SendPendingReport(Request, ReportRequestId, Destination, SentMessageResult)

        /// <summary>
        /// Send the report of this node's own Device Model accepted for the given request, now that its
        /// response is out.
        /// </summary>
        /// <remarks>
        /// The response of a report forwarded for a station is sent by this node too, and is not among
        /// the pending ones: the station sends its own report.
        /// </remarks>
        /// <param name="Request">The GetBaseReport or GetReport request.</param>
        /// <param name="ReportRequestId">The request id the NotifyReport requests refer to.</param>
        /// <param name="Destination">Whom to send the report to.</param>
        /// <param name="SentMessageResult">How sending the response went.</param>
        private void SendPendingReport(Object              Request,
                                       Int32               ReportRequestId,
                                       SourceRouting       Destination,
                                       SentMessageResults  SentMessageResult)
        {

            if (!pendingReports.TryRemove(Request, out var reportData) ||
                SentMessageResult != SentMessageResults.Success)
            {
                return;
            }

            // Not awaited here: this runs while the response is being sent, and the
            // answers to the NotifyReport requests come in over that same connection.
            _ = Task.Run(async () => {

                try
                {

                    var generatedAt = Now;

                    await DeviceModel.SendReportAsync(
                              reportData,
                              ReportDataPerNotifyReport,
                              (sequenceNumber, part, toBeContinued) => OCPP.OUT.NotifyReport(
                                                                           new NotifyReportRequest(
                                                                               Destination,
                                                                               ReportRequestId,
                                                                               sequenceNumber,
                                                                               generatedAt.UtcDateTime,
                                                                               part,
                                                                               toBeContinued,
                                                                               RequestId:          OCPP.NextRequestId,
                                                                               RequestTimestamp:   Now,
                                                                               RequestTimeout:     OCPP.DefaultRequestTimeout,
                                                                               EventTrackingId:    EventTracking_Id.New,
                                                                               NetworkPath:        NetworkPath.From(Id)
                                                                           )
                                                                       )
                          );

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, $"'{Id}': NotifyReport {ReportRequestId}");
                }

            });

        }

        #endregion

        #region (private) SendSecurityEventNotificationAsync(Type, Timestamp, TechInfo, Destination)

        /// <summary>
        /// Send one security event to the given destination.
        /// </summary>
        private Task<SecurityEventNotificationResponse> SendSecurityEventNotificationAsync(SecurityEventType  Type,
                                                                                           DateTime           Timestamp,
                                                                                           String?            TechInfo,
                                                                                           SourceRouting      Destination)

            => OCPP.OUT.SecurityEventNotification(
                   new SecurityEventNotificationRequest(
                       Destination,
                       Type,
                       Timestamp,
                       TechInfo,
                       RequestId:          OCPP.NextRequestId,
                       RequestTimestamp:   Now,
                       RequestTimeout:     OCPP.DefaultRequestTimeout,
                       EventTrackingId:    EventTracking_Id.New,
                       NetworkPath:        NetworkPath.From(Id)
                   )
               );

        #endregion

    }

}
