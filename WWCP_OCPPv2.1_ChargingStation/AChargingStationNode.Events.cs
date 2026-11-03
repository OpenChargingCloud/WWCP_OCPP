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
        private const String notifyEventKind   = "NotifyEvent";

        private readonly SemaphoreSlim  flushing        = new (1, 1);
        private          DateTimeOffset nextQueueTry;
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
        public TimeSpan          OfflineQueueRetry     { get; set; } = TimeSpan.FromSeconds(30);

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


        #region QueueSecurityEventAsync(Type, TechInfo = null, Timestamp = null)

        /// <summary>
        /// Send a security event, kept until the CSMS took it.
        /// </summary>
        /// <param name="Type">What happened.</param>
        /// <param name="TechInfo">What a technician needs to know of it: cut at <see cref="MaxTechInfoLength"/> characters.</param>
        /// <param name="Timestamp">When it happened; now where null.</param>
        public Task QueueSecurityEventAsync(SecurityEventType  Type,
                                            String?            TechInfo    = null,
                                            DateTimeOffset?    Timestamp   = null)
        {

            if (Type.Text.Length > MaxSecurityEventTypeLength)
                throw new ArgumentException($"The type of a security event has at most {MaxSecurityEventTypeLength} characters.", nameof(Type));

            var payload = new JObject(
                              new JProperty("type",       Type.Text),
                              new JProperty("timestamp",  (Timestamp ?? Now).ToString("o"))
                          );

            if (TechInfo is not null)
                payload["techInfo"] = TechInfo.Length > MaxTechInfoLength ? TechInfo[..MaxTechInfoLength] : TechInfo;

            OfflineQueue.Enqueue(securityEventKind, payload, Evidence: true);

            return FlushOfflineQueueAsync();

        }

        #endregion

        #region QueueEventsAsync(EventData)

        /// <summary>
        /// Send events in one NotifyEvent, kept until the CSMS took it - or let
        /// go first, where the queue is full, before any security event.
        /// </summary>
        public Task QueueEventsAsync(IEnumerable<EventData> EventData)
        {

            var events = EventData.ToArray();

            if (events.Length == 0)
                return Task.CompletedTask;

            OfflineQueue.Enqueue(notifyEventKind,
                                 new JObject(new JProperty("eventData", new JArray(events.Select(eventData => eventData.ToJSON())))),
                                 Evidence: false);

            return FlushOfflineQueueAsync();

        }

        #endregion

        #region FlushOfflineQueueAsync()

        /// <summary>
        /// Send what the queue holds, oldest first, until the CSMS does not take
        /// a message - which is tried again after <see cref="OfflineQueueRetry"/>.
        /// Answers how many were taken.
        /// </summary>
        public async Task<Int32> FlushOfflineQueueAsync()
        {

            await flushing.WaitAsync();

            try
            {

                var taken = 0;

                while (OfflineQueue.Pending.FirstOrDefault() is QueuedMessage message)
                {

                    if (!await TrySendAsync(message))
                    {
                        nextQueueTry = Now + OfflineQueueRetry;
                        break;
                    }

                    OfflineQueue.Remove(message);
                    taken++;

                }

                return taken;

            }
            finally
            {
                flushing.Release();
            }

        }

        #endregion


        #region (private) RetryOfflineQueue()

        /// <summary>
        /// Try the queue again, at the node's maintenance, where something waits
        /// in it and its time to be tried again has come.
        /// </summary>
        private void RetryOfflineQueue()
        {

            if (Now < nextQueueTry || OfflineQueue.Pending.Count == 0 || flushing.CurrentCount == 0)
                return;

            nextQueueTry = Now + OfflineQueueRetry;

            _ = Task.Run(async () => {
                    try
                    {
                        await FlushOfflineQueueAsync();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogException(e, $"Charging station '{Id}': the offline queue");
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
        private async Task<Boolean> TrySendAsync(QueuedMessage Message)
        {

            try
            {

                switch (Message.Kind)
                {

                    case securityEventKind:
                        var security = await this.SendSecurityEventNotification(
                                                 SecurityEventType.Parse(Message.Payload.Value<String>("type") ?? ""),
                                                 DateTimeOffset.Parse(Message.Payload.Value<String>("timestamp") ?? "", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                                                 Message.Payload.Value<String>("techInfo")
                                             );
                        return security.Result.ResultCode == ResultCode.OK;

                    case notifyEventKind:
                        var notify   = await this.NotifyEvent(
                                                 Now,
                                                 0,
                                                 [.. (Message.Payload["eventData"] as JArray ?? []).OfType<JObject>().Select(json => OCPPv2_1.EventData.Parse(json))]
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
