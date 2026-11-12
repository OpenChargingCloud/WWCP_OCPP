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

using System.Globalization;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using QueuedMessage       = cloud.charging.open.protocols.OCPP.QueuedMessage;
using OfflineQueue        = cloud.charging.open.protocols.OCPP.OfflineQueue;
using ResultCode          = cloud.charging.open.protocols.OCPP.ResultCode;
using Outboxes            = cloud.charging.open.protocols.OCPP.Outboxes;
using CertificateSources  = cloud.charging.open.protocols.OCPP.CertificateSources;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.LC;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.LocalController
{

    /// <summary>
    /// The Device Model of the local controller itself: its components, which a CSMS reads and
    /// writes with GetVariables, SetVariables, GetReport and GetBaseReport addressed to this node,
    /// and what it tells its CSMS of itself - the events of monitors, and security events.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The same Device Model a charging station has (<see cref="IDeviceModelNode"/>), so that the
    /// components and the bridges that work with a station's - EVQICtrlr, the DNS and time
    /// controllers of WWCP_OCPP_Node - are added to a local controller by the same calls.
    /// </para>
    /// <para>
    /// It answers requests for this node only. A request for a station behind the controller never
    /// meets it: the adapter hands it to FORWARD by its destination before any handler runs, and it
    /// reaches the station as it was sent, to be answered by the station's own Device Model under the
    /// station's own rules.
    /// </para>
    /// <para>
    /// A local controller has no EVSEs: a component at an EVSE is none of its.
    /// </para>
    /// </remarks>
    public abstract partial class ALocalControllerNode
    {

        #region Data

        /// <summary>
        /// The longest techInfo of a security event (OCPP 2.1, SecurityEventNotificationRequest):
        /// a longer one is cut there - the whole of it belongs in the controller's own log.
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

        private readonly List<INewInstancesSource>  newInstancesSources  = [];
        private          Outboxes?                  queues;
        private          DateTimeOffset             nextMonitorsCheck;

        #endregion

        #region Properties

        /// <summary>
        /// Where the certificates a CSMS installs, lists and deletes at this local controller are
        /// kept, by their use: in memory, and by a source of their own for the types one was added for.
        /// </summary>
        public CertificateSources  CertificateSources    { get; } = new();

        /// <summary>
        /// The monitors the CSMS set on this local controller's variables.
        /// </summary>
        public VariableMonitors    VariableMonitors      { get; set; } = new();

        /// <summary>
        /// The security events and monitor events of this local controller not yet taken by the CSMS:
        /// in memory, unless a queue kept in a file is given.
        /// </summary>
        public OfflineQueue        OfflineQueue          { get; set; } = new();

        /// <summary>
        /// How often the node reads the variables its monitors watch by itself, beside whenever it is
        /// told one may have changed: a value read live changes without anybody saying so.
        /// </summary>
        public TimeSpan            MonitorsCheckEvery    { get; set; } = TimeSpan.FromSeconds(10);

        #endregion


        #region AddComponent        (Component)

        /// <summary>
        /// Add the given component to this local controller's own Device Model, unless a component with
        /// the same name and instance is already there - EVQICtrlr, say.
        /// </summary>
        /// <param name="Component">A component configuration.</param>
        public Boolean AddComponent(ComponentConfig Component)

            => Component.EVSE is null &&
               OCPP.TryAddComponentConfig(Component);

        #endregion

        #region RemoveComponent     (Name, Instance = null, EVSE = null)

        /// <summary>
        /// Remove the component with the given name and instance from this local controller's own Device
        /// Model.
        /// </summary>
        /// <param name="Name">The case insensitive name of the component.</param>
        /// <param name="Instance">The optional case insensitive instance of the component.</param>
        /// <param name="EVSE">The optional EVSE of the component: a local controller has none.</param>
        public Boolean RemoveComponent(String   Name,
                                       String?  Instance   = null,
                                       EVSE?    EVSE       = null)

            => EVSE is null &&
               OCPP.TryRemoveComponentConfig(Name, Instance);

        #endregion

        #region GetComponentConfigs (Name, EVSE = null)

        /// <summary>
        /// The components of the given name of this local controller's own Device Model; none at an EVSE.
        /// </summary>
        /// <param name="Name">The case insensitive name of the component.</param>
        /// <param name="EVSE">The optional EVSE of the component: a local controller has none.</param>
        public IEnumerable<ComponentConfig> GetComponentConfigs(String  Name,
                                                                EVSE?   EVSE   = null)

            => EVSE is null && OCPP.TryGetComponentConfig(Name, out var componentConfigs)
                   ? componentConfigs
                   : [];

        #endregion

        #region AllComponentConfigs

        /// <summary>
        /// All components of this local controller's own Device Model, as they are at this moment.
        /// </summary>
        public IEnumerable<ComponentConfig> AllComponentConfigs

            => [.. OCPP.ComponentConfigs];

        #endregion

        #region AddNewInstancesSource(Source)

        /// <summary>
        /// Let the given source make the instances of its components that a SetVariables request names
        /// and this local controller does not have yet - see <see cref="INewInstancesSource"/>.
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
        /// <param name="Component">A component, with an instance this local controller does not have.</param>
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
        /// Send a security event of this local controller, kept until the CSMS took it.
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
                                DebugX.LogException(e, $"Local controller '{Id}': the offline queue of '{destination}'");
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
                            DebugX.LogException(e, $"Local controller '{Id}': checking its monitors");
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
                        var security = await this.SendSecurityEventNotification(
                                                 SecurityEventType.Parse(Message.Payload.Value<String>("type") ?? ""),
                                                 DateTimeOffset.Parse(Message.Payload.Value<String>("timestamp") ?? "", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).UtcDateTime,
                                                 Message.Payload.Value<String>("techInfo"),
                                                 Destination: SourceRouting.To(Destination)
                                             );
                        return security.Result.ResultCode == ResultCode.OK;

                    case notifyEventKind:
                        var notify   = await this.NotifyEvent(
                                                 Now.UtcDateTime,
                                                 0,
                                                 [.. (Message.Payload["eventData"] as JArray ?? []).OfType<JObject>().Select(json => OCPPv2_1.EventData.Parse(json))],
                                                 Destination: SourceRouting.To(Destination)
                                             );
                        return notify.Result.ResultCode == ResultCode.OK;

                    default:
                        return true;

                }

            }
            catch (Exception e)
            {
                DebugX.LogException(e, $"Local controller '{Id}': sending a queued {Message.Kind}");
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

                var response = await this.SendSecurityEventNotification(
                                         SecurityEventType.Parse(QueueOverflow),
                                         Now.UtcDateTime,
                                         $"{Dropped} message(s) waiting for this back end were let go, for room or for age, while it could not be reached.",
                                         Destination: SourceRouting.To(Destination)
                                     );

                return response.Result.ResultCode == ResultCode.OK;

            }
            catch (Exception e)
            {
                DebugX.LogException(e, $"Local controller '{Id}': telling '{Destination}' of messages let go");
                return false;
            }

        }

        #endregion

    }

}
