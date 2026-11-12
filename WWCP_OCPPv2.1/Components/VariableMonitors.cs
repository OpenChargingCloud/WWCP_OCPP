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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A monitor a CSMS set on a variable, and what it compares the variable
    /// with.
    /// </summary>
    public sealed class VariableMonitor
    {

        #region Properties

        public VariableMonitoring_Id  Id            { get; }
        public Component              Component     { get; }
        public Variable               Variable      { get; }
        public MonitorType            Type          { get; }
        public Decimal                Value         { get; }
        public Severities             Severity      { get; }
        public Boolean                Transaction   { get; }

        /// <summary>
        /// The value a delta is measured from: the variable's when the monitor
        /// was set, and since then when it last told of a change.
        /// </summary>
        public String?                Reference     { get; internal set; }

        /// <summary>
        /// Whether a threshold is crossed now, and was told so.
        /// </summary>
        public Boolean                Tripped       { get; internal set; }

        #endregion

        #region Constructor(s)

        public VariableMonitor(VariableMonitoring_Id  Id,
                               Component              Component,
                               Variable               Variable,
                               MonitorType            Type,
                               Decimal                Value,
                               Severities             Severity,
                               Boolean                Transaction,
                               String?                Reference)
        {

            this.Id           = Id;
            this.Component    = Component;
            this.Variable     = Variable;
            this.Type         = Type;
            this.Value        = Value;
            this.Severity     = Severity;
            this.Transaction  = Transaction;
            this.Reference    = Reference;

        }

        #endregion

        #region ToVariableMonitoring()

        /// <summary>
        /// The monitor as a NotifyMonitoringReport says it.
        /// </summary>
        public VariableMonitoring ToVariableMonitoring()

            => new (Id, Transaction, Value, Type, Severity, EventNotificationType.CustomMonitor);

        #endregion

    }


    /// <summary>
    /// The monitors a CSMS set on the variables of a charging station
    /// (OCPP 2.1 N04, N05, N06), and the events they make of a change.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A monitor is set on a variable whose characteristics say it supports
    /// monitoring, and is of the kind Delta, UpperThreshold or LowerThreshold:
    /// the periodic ones would need a clock of their own, and nothing here
    /// asks for them. A threshold is for a number.
    /// </para>
    /// <para>
    /// The monitors do not watch anything themselves. Whoever knows that a
    /// value may have changed asks <see cref="Check"/>, which reads every
    /// monitored variable, compares it with what its monitor saw last, and
    /// answers the events to send - so that a value read live from somewhere
    /// else, as the time servers' are, is monitored without a copy of it.
    /// </para>
    /// <para>
    /// Kept in memory: a charging station's custom monitors are its CSMS's,
    /// which sets them again after a restart.
    /// </para>
    /// </remarks>
    public sealed class VariableMonitors
    {

        #region Data

        private readonly Lock                   gate      = new();
        private readonly List<VariableMonitor>  monitors  = [];
        private          UInt64                 lastId;

        #endregion

        #region Properties

        /// <summary>
        /// The least severe monitor whose events are sent: all of them by
        /// default (SetMonitoringLevel).
        /// </summary>
        public Severities  Level    { get; private set; } = Severities.Debug;

        /// <summary>
        /// Every monitor there is.
        /// </summary>
        public IReadOnlyList<VariableMonitor> All
        {
            get
            {
                lock (gate)
                    return [.. monitors];
            }
        }

        #endregion


        #region Set(Request, ComponentConfigs, SignaturePolicy)

        /// <summary>
        /// Set the monitors of the given SetVariableMonitoring request, each
        /// decided by itself: a monitor changes what the station does with
        /// its variable, so one on a variable the signers of the request may
        /// not edit is "Rejected" with the reason code "NotAuthorized" (white
        /// paper "Signed OCPP Messages and Signer Roles", section 5.2).
        /// </summary>
        /// <param name="Request">A SetVariableMonitoring request whose signatures are verified.</param>
        /// <param name="ComponentConfigs">The components of the station.</param>
        /// <param name="SignaturePolicy">The node's signature policy.</param>
        public IReadOnlyList<SetMonitoringResult> Set(SetVariableMonitoringRequest   Request,
                                                      ComponentConfigLookupDelegate  ComponentConfigs,
                                                      SignaturePolicy                SignaturePolicy)

            => [.. Request.MonitoringData.SelectMany(data =>
                       SignaturePolicy.DecideVariable(
                           Request,
                           data.Component.Name,
                           data.Variable. Name,
                           data.Component.Instance,
                           (Int32?) data.Component.EVSE?.Id.Value,
                           "edit",
                           out var reason
                       )
                           ? Set([ data ], ComponentConfigs)
                           : [
                                 new SetMonitoringResult(
                                     SetMonitoringStatus.Rejected,
                                     data.MonitorType,
                                     data.Severity,
                                     data.Component,
                                     data.Variable,
                                     data.VariableMonitoringId,
                                     new StatusInfo("NotAuthorized", reason)
                                 )
                             ])];

        #endregion

        #region Set(MonitoringData, ComponentConfigs)

        /// <summary>
        /// Set the monitors of a SetVariableMonitoring request, each on its own.
        /// </summary>
        /// <param name="MonitoringData">The monitors asked for.</param>
        /// <param name="ComponentConfigs">The components of the station.</param>
        public IReadOnlyList<SetMonitoringResult> Set(IEnumerable<SetMonitoringData>  MonitoringData,
                                                      ComponentConfigLookupDelegate   ComponentConfigs)
        {

            var results = new List<SetMonitoringResult>();

            lock (gate)
            {
                foreach (var data in MonitoringData)
                {

                    var (status, info) = Check(data, ComponentConfigs, out var reference);
                    var id             = data.VariableMonitoringId;

                    if (status == SetMonitoringStatus.Accepted)
                    {

                        if (id is VariableMonitoring_Id given)
                            monitors.RemoveAll(monitor => monitor.Id == given);

                        else
                            id = VariableMonitoring_Id.Parse(++lastId);

                        monitors.Add(new VariableMonitor(id.Value, data.Component, data.Variable, data.MonitorType, data.Value,
                                                         data.Severity, data.Transaction ?? false, reference));

                    }

                    results.Add(new SetMonitoringResult(status, data.MonitorType, data.Severity, data.Component, data.Variable, id, info));

                }
            }

            return results;

        }

        #endregion

        #region Clear(Ids)

        /// <summary>
        /// Clear the monitors of a ClearVariableMonitoring request.
        /// </summary>
        public IReadOnlyList<ClearMonitoringResult> Clear(IEnumerable<VariableMonitoring_Id> Ids)
        {

            lock (gate)
                return [.. Ids.Select(id => new ClearMonitoringResult(
                                                monitors.RemoveAll(monitor => monitor.Id == id) > 0
                                                    ? ClearMonitoringStatus.Accepted
                                                    : ClearMonitoringStatus.NotFound,
                                                id
                                            ))];

        }

        #endregion

        #region SetLevel(Severity) / SetBase(MonitoringBase)

        /// <summary>
        /// Send the events of the monitors of this severity and the more severe
        /// ones from now on (SetMonitoringLevel).
        /// </summary>
        public void SetLevel(Severities Severity)
        {
            Level = Severity;
        }

        /// <summary>
        /// Keep every monitor (All), or none a CSMS set (FactoryDefault,
        /// HardWiredOnly): there are no others here.
        /// </summary>
        public void SetBase(MonitoringBase Base)
        {
            if (Base != MonitoringBase.All)
                lock (gate)
                    monitors.Clear();
        }

        #endregion

        #region Report(Criteria, ComponentVariables)

        /// <summary>
        /// The monitors a GetMonitoringReport asks for: of the kinds its criteria
        /// name and on the components and variables it names - all of them
        /// where it names none - one entry per monitored variable.
        /// </summary>
        public IReadOnlyList<MonitoringData> Report(IEnumerable<MonitoringCriterion>  Criteria,
                                                    IEnumerable<ComponentVariable>   ComponentVariables)
        {

            var criteria  = Criteria.ToArray();
            var named     = ComponentVariables.ToArray();

            lock (gate)
                return [.. monitors.Where   (monitor => criteria.Length == 0 || criteria.Any(criterion => Meets(monitor, criterion))).
                                    Where   (monitor => named.   Length == 0 || named.   Any(cv        => Names(cv, monitor))).
                                    GroupBy (monitor => (Key(monitor.Component), Key(monitor.Variable))).
                                    Select  (group   => new MonitoringData(group.First().Component,
                                                                           group.First().Variable,
                                                                           [.. group.Select(monitor => monitor.ToVariableMonitoring())]))];

        }

        #endregion

        #region Check(ComponentConfigs, Now, Which = null)

        /// <summary>
        /// Read every monitored variable - those the filter lets through, where
        /// one is given - and answer the events their monitors make of what
        /// changed since they saw it last.
        /// </summary>
        /// <remarks>
        /// A delta of a number is told where it moved at least the monitor's
        /// value from where it was last told of, and of anything else at every
        /// change. A threshold is told when it is crossed, and once more,
        /// cleared, when it is crossed back. A monitor less severe than
        /// <see cref="Level"/> moves on with the value, and tells nothing. A
        /// variable that is not there any more tells nothing either.
        /// </remarks>
        /// <param name="ComponentConfigs">The components of the station.</param>
        /// <param name="Now">When the values were read.</param>
        /// <param name="Which">Which monitored variables to read; all where null.</param>
        public IReadOnlyList<EventData> Check(ComponentConfigLookupDelegate             ComponentConfigs,
                                              DateTimeOffset                            Now,
                                              Func<Component, Variable, Boolean>?       Which = null)
        {

            var events = new List<EventData>();

            lock (gate)
            {
                foreach (var monitor in monitors)
                {

                    if (Which is not null && !Which(monitor.Component, monitor.Variable))
                        continue;

                    if (!DeviceModel.TryFind(monitor.Component, monitor.Variable, ComponentConfigs, out _, out var variableConfig, out _))
                        continue;

                    var value    = variableConfig.Value ?? "";
                    var told     = monitor.Severity.AsNumber() <= Level.AsNumber();
                    var trigger  = EventTriggers.Delta;
                    Boolean? cleared = null;

                    if (monitor.Type == MonitorType.Delta)
                    {

                        if (!Moved(monitor.Reference, value, monitor.Value))
                            continue;

                        monitor.Reference = value;

                    }

                    else
                    {

                        if (!Decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
                            continue;

                        var beyond = monitor.Type == MonitorType.UpperThreshold
                                         ? number > monitor.Value
                                         : number < monitor.Value;

                        if (beyond == monitor.Tripped)
                            continue;

                        monitor.Tripped = beyond;
                        trigger         = EventTriggers.Alerting;
                        cleared         = beyond ? null : true;

                    }

                    if (told)
                        events.Add(new EventData(
                                       EventId:                Event_Id.NewRandom,
                                       Timestamp:              Now,
                                       Trigger:                trigger,
                                       ActualValue:            value.Length > MaxActualValueLength ? value[..MaxActualValueLength] : value,
                                       EventNotificationType:  EventNotificationType.CustomMonitor,
                                       Component:              monitor.Component,
                                       Variable:               monitor.Variable,
                                       Severity:               monitor.Severity,
                                       Cleared:                cleared,
                                       VariableMonitoringId:   monitor.Id
                                   ));

                }
            }

            return events;

        }

        /// <summary>
        /// The longest actual value an event carries (OCPP 2.1, EventDataType).
        /// </summary>
        public const Int32 MaxActualValueLength = 2500;

        #endregion


        #region (private) Check(Data, ComponentConfigs, out Reference)

        /// <summary>
        /// Whether a monitor may be set as asked, and why not - with the value
        /// it starts from.
        /// </summary>
        private (SetMonitoringStatus Status, StatusInfo? Info) Check(SetMonitoringData              Data,
                                                                     ComponentConfigLookupDelegate  ComponentConfigs,
                                                                     out String?                    Reference)
        {

            Reference = null;

            if (Data.MonitorType != MonitorType.Delta          &&
                Data.MonitorType != MonitorType.UpperThreshold &&
                Data.MonitorType != MonitorType.LowerThreshold)
            {
                return (SetMonitoringStatus.UnsupportedMonitorType,
                        new StatusInfo("UnsupportedParam", $"Monitors of the kind {Data.MonitorType} are not supported: Delta, UpperThreshold and LowerThreshold are."));
            }

            if (!DeviceModel.TryFind(Data.Component, Data.Variable, ComponentConfigs, out var componentConfig, out var variableConfig, out var notFound))
                return (componentConfig is null ? SetMonitoringStatus.UnknownComponent : SetMonitoringStatus.UnknownVariable, notFound);

            if (variableConfig.Characteristics?.SupportsMonitoring != true)
                return (SetMonitoringStatus.Rejected,
                        new StatusInfo("UnsupportedParam", $"'{Data.Variable.Name}' of '{Data.Component.Name}' cannot be monitored."));

            Reference = variableConfig.Value;

            if (Data.MonitorType != MonitorType.Delta &&
                !Decimal.TryParse(Reference, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            {
                return (SetMonitoringStatus.Rejected,
                        new StatusInfo("ValueOutOfRange", $"A threshold is for a number, and '{Data.Variable.Name}' is none."));
            }

            if (Data.Value < 0 && Data.MonitorType == MonitorType.Delta)
                return (SetMonitoringStatus.Rejected,
                        new StatusInfo("ValueOutOfRange", "A delta is not negative."));

            if (Data.VariableMonitoringId is VariableMonitoring_Id id &&
                !monitors.Any(monitor => monitor.Id == id))
            {
                return (SetMonitoringStatus.Rejected,
                        new StatusInfo("NotFound", $"No monitor {id} to replace: the station gives a new monitor its id."));
            }

            if (monitors.Any(monitor => monitor.Id       != Data.VariableMonitoringId &&
                                        monitor.Type     == Data.MonitorType          &&
                                        monitor.Severity == Data.Severity             &&
                                        Key(monitor.Component) == Key(Data.Component) &&
                                        Key(monitor.Variable)  == Key(Data.Variable)))
            {
                return (SetMonitoringStatus.Duplicate, null);
            }

            return (SetMonitoringStatus.Accepted, null);

        }

        #endregion

        #region (private static) Moved(Reference, Value, Delta) / Meets / Names / Key

        /// <summary>
        /// Whether a value moved from the one a delta is measured from: by at
        /// least the delta, for numbers; at all, for anything else.
        /// </summary>
        private static Boolean Moved(String?  Reference,
                                     String   Value,
                                     Decimal  Delta)
        {

            if (Decimal.TryParse(Reference, NumberStyles.Float, CultureInfo.InvariantCulture, out var before) &&
                Decimal.TryParse(Value,     NumberStyles.Float, CultureInfo.InvariantCulture, out var now))
            {
                return now != before && Math.Abs(now - before) >= Delta;
            }

            return !String.Equals(Reference ?? "", Value, StringComparison.Ordinal);

        }

        private static Boolean Meets(VariableMonitor      Monitor,
                                     MonitoringCriterion  Criterion)

            => Criterion switch {
                   MonitoringCriterion.ThresholdMonitoring  => Monitor.Type == MonitorType.UpperThreshold || Monitor.Type == MonitorType.LowerThreshold,
                   MonitoringCriterion.DeltaMonitoring      => Monitor.Type == MonitorType.Delta,
                   _                                        => false
               };

        /// <summary>
        /// Whether a component and variable of a request name a monitor's: the
        /// same component, any instance of it where none is named, and any
        /// variable of it where none is named.
        /// </summary>
        private static Boolean Names(ComponentVariable  Named,
                                     VariableMonitor    Monitor)

            => String.Equals(Named.Component.Name, Monitor.Component.Name, StringComparison.OrdinalIgnoreCase)                    &&
               (Named.Component.Instance is null || ComponentConfigStore.SameInstance(Named.Component.Instance, Monitor.Component.Instance)) &&
               (Named.Variable is null ||
                   String.Equals(Named.Variable.Name, Monitor.Variable.Name, StringComparison.OrdinalIgnoreCase) &&
                   (Named.Variable.Instance is null || ComponentConfigStore.SameInstance(Named.Variable.Instance, Monitor.Variable.Instance)));

        private static String Key(Component Component)
            => $"{Component.Name.ToLowerInvariant()}|{Component.Instance?.ToLowerInvariant()}|{Component.EVSE?.Id}|{Component.EVSE?.ConnectorId}";

        private static String Key(Variable Variable)
            => $"{Variable.Name.ToLowerInvariant()}|{Variable.Instance?.ToLowerInvariant()}";

        #endregion

    }

}
