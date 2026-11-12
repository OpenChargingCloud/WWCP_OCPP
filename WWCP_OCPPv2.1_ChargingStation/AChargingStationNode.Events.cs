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

using QueuedMessage = cloud.charging.open.protocols.OCPP.QueuedMessage;
using OfflineQueue  = cloud.charging.open.protocols.OCPP.OfflineQueue;
using ResultCode    = cloud.charging.open.protocols.OCPP.ResultCode;
using Outboxes      = cloud.charging.open.protocols.OCPP.Outboxes;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// What a charging station tells its CSMS of itself: the events of the
    /// monitors the CSMS set, and security events - kept in a queue until the
    /// CSMS took them.
    /// </summary>
    public abstract partial class AChargingStationNode
    {

        #region Data

        /// <summary>
        /// The longest techInfo of a security event (OCPP 2.1,
        /// SecurityEventNotificationRequest): a longer one is cut there - the
        /// whole of it belongs in the station's own log.
        /// </summary>
        public const Int32 MaxTechInfoLength = 255;

        /// <summary>
        /// The longest type of a security event.
        /// </summary>
        public const Int32 MaxSecurityEventTypeLength = 50;

        private const String securityEventKind = "SecurityEventNotification";

        /// <summary>
        /// The requests a violation of the signature policy was told of: one
        /// security event per request, however many of its entries were refused.
        /// </summary>
        private readonly System.Runtime.CompilerServices.ConditionalWeakTable<ISignableMessage, Object> violationsTold = [];

        /// <summary>
        /// The security event that tells a back end how many messages waiting for it were let go.
        /// </summary>
        public  const String QueueOverflow     = "QueueOverflow";
        private const String notifyEventKind   = "NotifyEvent";

        private          Outboxes?      queues;
        private          DateTimeOffset nextMonitorsCheck;

        #endregion

        #region Properties

        /// <summary>
        /// The monitors the CSMS set on this station's variables.
        /// </summary>
        /// <remarks>
        /// Settable, so that a node built anew for the same station - after its
        /// EVSEs changed - keeps the monitors of the one before.
        /// </remarks>
        public VariableMonitors  VariableMonitors      { get; set; } = new();

        /// <summary>
        /// The security events and monitor events not yet taken by the CSMS: in
        /// memory, unless a queue kept in a file is given.
        /// </summary>
        public OfflineQueue      OfflineQueue          { get; set; } = new();

        /// <summary>
        /// How long to wait before the queue is tried again after the CSMS did
        /// not take a message.
        /// </summary>
        public TimeSpan          OfflineQueueRetry
        {
            get => Queues.Retry;
            set => Queues.Retry = value;
        }

        /// <summary>
        /// Whether what the queue holds waits rather than goes: queued all the
        /// same, and sent by <see cref="FlushOfflineQueueAsync"/> once this is
        /// false again.
        /// </summary>
        /// <remarks>
        /// For a station whose CSMS has not accepted its BootNotification yet,
        /// and which may tell it nothing else until then.
        /// </remarks>
        public Boolean           HoldOfflineQueue
        {
            get => Queues.IsHeld(Queues.CSMS);
            set => Queues.Hold  (Queues.CSMS, value);
        }

        /// <summary>
        /// How often the node reads the variables its monitors watch by itself,
        /// beside whenever it is told one may have changed: a value read live
        /// changes without anybody saying so.
        /// </summary>
        public TimeSpan          MonitorsCheckEvery    { get; set; } = TimeSpan.FromSeconds(10);

        #endregion


        #region CheckMonitorsAsync(Which = null)

        /// <summary>
        /// Read every monitored variable - those the filter lets through, where
        /// one is given - and send the events their monitors make of what
        /// changed: through the queue, so that none is lost while the CSMS is
        /// out of reach. Answers how many there were.
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

        #region VariableChanged(Component, Variable = null)

        /// <summary>
        /// Say that a variable of a component - or any of its variables, where
        /// none is named - may have changed: its monitors are asked.
        /// </summary>
        /// <param name="Component">The component, with its instance.</param>
        /// <param name="Variable">The variable, or null for every one of the component.</param>
        public Task<Int32> VariableChanged(Component  Component,
                                           Variable?  Variable = null)

            => CheckMonitorsAsync((component, variable) => SameComponent(component, Component) &&
                                                           (Variable is null || SameVariable(variable, Variable)));

        #endregion


        #region (private) Queues

        /// <summary>
        /// A queue for each destination this node tells things to: the one of its
        /// CSMS is <see cref="OfflineQueue"/>.
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

        #region OfflineQueueFor(Destination) / HoldOfflineQueueFor(Destination, Held)

        /// <summary>
        /// The queue of what waits for the given destination: <see cref="OfflineQueue"/>
        /// for the CSMS, and one of its own for any other back end - a successor,
        /// during a migration - bounded by <see cref="Outboxes.OtherMaxMessages"/>
        /// and <see cref="Outboxes.OtherMaxAge"/>.
        /// </summary>
        /// <param name="Destination">Where its messages go.</param>
        public OfflineQueue OfflineQueueFor(NetworkingNode_Id Destination)
            => Queues.QueueFor(Destination);

        /// <summary>
        /// Let what is queued for the given destination wait, or go again: for a
        /// back end that has not accepted this station yet. For the CSMS, as
        /// <see cref="HoldOfflineQueue"/>.
        /// </summary>
        /// <param name="Destination">Where its messages go.</param>
        /// <param name="Held">Whether they wait.</param>
        public void HoldOfflineQueueFor(NetworkingNode_Id  Destination,
                                        Boolean            Held)

            => Queues.Hold(Destination, Held);

        #endregion

        #region (private) TellOfPolicyViolation(Violation)

        /// <summary>
        /// Tell the CSMS of a request that was not signed as the signature
        /// policy wants it, refused or executed all the same: a security event
        /// of the kind of the violation, whose techInfo names the action and
        /// the reason (white paper "Signed OCPP Messages and Signer Roles",
        /// section 5.4). A request is told of once.
        /// </summary>
        private void TellOfPolicyViolation(PolicyViolation Violation)
        {

            // What a connection may not carry is no fault of a signature, and section 5.4
            // names no security event for it: whoever asked is told with the SecurityError.
            if (Violation.Kind == PolicyViolationKinds.ConnectionNotAuthorized)
                return;

            if (!violationsTold.TryAdd(Violation.Message, Violation))
                return;

            var type = Violation.Kind switch {
                           PolicyViolationKinds.SignatureMissing     => SecurityEventType.MessageSignatureMissing,
                           PolicyViolationKinds.SignatureInvalid     => SecurityEventType.MessageSignatureInvalid,
                           PolicyViolationKinds.SignerUnknown        => SecurityEventType.MessageSignerUnknown,
                           PolicyViolationKinds.Replayed             => SecurityEventType.MessageReplayed,
                           PolicyViolationKinds.SignatureExpired     => SecurityEventType.MessageSignatureExpired,
                           _                                         => SecurityEventType.MessageSignerNotAuthorized
                       };

            try
            {

                _ = QueueSecurityEventAsync(
                        type,
                        $"{Violation.Action}{(Violation.Refused ? "" : ", executed all the same")}: {Violation.Reason}"
                    ).ContinueWith(
                        task => DebugX.LogException(task.Exception!, $"Charging station '{Id}': telling of a '{type}'"),
                        TaskContinuationOptions.OnlyOnFaulted
                    );

            }
            catch (Exception e)
            {
                DebugX.LogException(e, $"Charging station '{Id}': telling of a '{type}'");
            }

        }

        #endregion

        #region QueueSecurityEventAsync(Type, TechInfo = null, Timestamp = null, Destination = null)

        /// <summary>
        /// Send a security event, kept until the CSMS took it.
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
        /// Send events in one NotifyEvent, kept until the CSMS took it - or let
        /// go first, where the queue is full, before any security event.
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
        /// Send what waits for the given destination - the CSMS where null -
        /// oldest first, until it does not take a message, which is tried again
        /// after <see cref="OfflineQueueRetry"/>. A back end that lost messages of
        /// its queue for room or for age is told how many first. Answers how many
        /// were taken.
        /// </summary>
        /// <param name="Destination">Whom to tell; the CSMS where null.</param>
        public Task<Int32> FlushOfflineQueueAsync(NetworkingNode_Id? Destination = null)

            => Queues.FlushAsync(Destination ?? Queues.CSMS);

        #endregion


        #region (private) RetryOfflineQueue()

        /// <summary>
        /// Try the queues again, at the node's maintenance, where something waits
        /// in one and its time to be tried again has come.
        /// </summary>
        private void RetryOfflineQueue()
        {

            foreach (var destination in Queues.DueForRetry())
                _ = Task.Run(async () => {
                        try
                        {
                            await FlushOfflineQueueAsync(destination);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogException(e, $"Charging station '{Id}': the offline queue of '{destination}'");
                        }
                    });

        }

        #endregion

        #region (private) CheckMonitorsInTime()

        /// <summary>
        /// Read the monitored variables at the node's maintenance, where there
        /// are monitors and it is time.
        /// </summary>
        private void CheckMonitorsInTime()
        {

            if (Now < nextMonitorsCheck || VariableMonitors.All.Count == 0)
                return;

            nextMonitorsCheck = Now + MonitorsCheckEvery;

            _ = Task.Run(async () => {
                    try
                    {
                        await CheckMonitorsAsync();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogException(e, $"Charging station '{Id}': checking its monitors");
                    }
                });

        }

        #endregion

        #region (private) TrySendAsync(Message)

        /// <summary>
        /// Send one message of the queue, answering whether the CSMS took it.
        /// A message of a kind nobody sends any more counts as taken, and goes.
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
                                                 DateTimeOffset.Parse(Message.Payload.Value<String>("timestamp") ?? "", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                                                 Message.Payload.Value<String>("techInfo"),
                                                 Destination: SourceRouting.To(Destination)
                                             );
                        return security.Result.ResultCode == ResultCode.OK;

                    case notifyEventKind:
                        var notify   = await this.NotifyEvent(
                                                 Now,
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
                DebugX.LogException(e, $"Charging station '{Id}': sending a queued {Message.Kind}");
                return false;
            }

        }

        #endregion

        #region (private) TellOfOverflowAsync(Dropped, Destination)

        /// <summary>
        /// Tell the given destination how many messages of its queue were let go,
        /// for room or for age, with a security event "QueueOverflow" - answering
        /// whether it took it.
        /// </summary>
        private async Task<Boolean> TellOfOverflowAsync(Int64              Dropped,
                                                        NetworkingNode_Id  Destination)
        {

            try
            {

                var response = await this.SendSecurityEventNotification(
                                         SecurityEventType.Parse(QueueOverflow),
                                         Now,
                                         $"{Dropped} message(s) waiting for this back end were let go, for room or for age, while it could not be reached.",
                                         Destination: SourceRouting.To(Destination)
                                     );

                return response.Result.ResultCode == ResultCode.OK;

            }
            catch (Exception e)
            {
                DebugX.LogException(e, $"Charging station '{Id}': telling '{Destination}' of messages let go");
                return false;
            }

        }

        #endregion

        #region (private static) SameComponent(A, B) / SameVariable(A, B)

        private static Boolean SameComponent(Component A, Component B)

            => String.Equals(A.Name, B.Name, StringComparison.OrdinalIgnoreCase) &&
               ComponentConfigStore.SameInstance(A.Instance, B.Instance)        &&
               A.EVSE?.Id == B.EVSE?.Id;

        private static Boolean SameVariable(Variable A, Variable B)

            => String.Equals(A.Name, B.Name, StringComparison.OrdinalIgnoreCase) &&
               ComponentConfigStore.SameInstance(A.Instance, B.Instance);

        #endregion

    }

}
