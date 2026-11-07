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
using System.Globalization;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.OCPPv2_1;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Configuration;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// How a CSMS changes the time servers of this node: the writes of a
    /// SetVariables request to NTPController, NTPClientGroupController,
    /// NTPClientController and ClockCtrlr, put into the node as the web page
    /// puts a save.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The node replaces a group's list of servers as a whole, so every change
    /// that touches a server sends the whole list, each server with its id and
    /// with what the request found it held to ("pinsAsShown"), so that a pin
    /// learned on first use in the meantime is kept. A list that is no longer
    /// the one the request read - a server added on the page meanwhile - would
    /// be written away by it, so the change is refused as ValueChanged instead,
    /// while nothing else can change the groups.
    /// </para>
    /// <para>
    /// With IndependentRequests every entry is a change of its own, in the
    /// order of the request. A new group or server cannot be one: a server
    /// needs its host name in the same change, so these need ACID or BASE.
    /// With ACID and BASE all entries handed over at once are one change, all
    /// of it or nothing: groups named anew in ClientGroups are made first, the
    /// groups' changes go into the node in one save, and groups no longer
    /// named are taken away last. A change refused after groups were made
    /// takes them away again.
    /// </para>
    /// <para>
    /// ClockCtrlr.NtpServerUri is the legal group by position: the k-th server
    /// it asks, as the request found them. Writing it replaces that server's
    /// host name, the next free position adds one in the band of the last, and
    /// "" takes one away. NTS-DeviceModel-Concept.md B.6 to B.10 say all of this
    /// at length.
    /// </para>
    /// </remarks>
    public sealed partial class NTSDeviceModel : ISetVariablesSource,
                                                 INewInstancesSource
    {

        #region Data

        /// <summary>
        /// The most characters of the additionalInfo of a result (OCPP 2.1).
        /// </summary>
        private const Int32 MaxAdditionalInfo = 1024;

        /// <summary>
        /// The unit of a BASE request that makes and takes away groups: the
        /// groups as a whole, together with what a new group is given.
        /// </summary>
        private const String GroupsUnit = "groups";

        private static readonly String[] writableControllerVariables = [
            "ClientGroups"
        ];

        private static readonly String[] writableGroupVariables = [
            "Enabled", "Clients", "MinServers", "MaxDeviation", "MinInterval", "MaxInterval", "Timeout", "ErrorLogging", "MaxErrors",
            "LegalTimeAuthority", "LegalTimeTolerance", "LegalTimeMaxAge"
        ];

        private static readonly String[] writableClientVariables = [
            "Enabled", "ServerURL", "ServerPort", "NTSKEServerPort", "Priority", "CertificatePins", "RootPins", "OnPinMismatch", "TrustOnFirstUse",
            "RootCAs", "AEADAlgorithm", "MinRefresh", "MaxRefresh", "DelayAsymmetry", "DelayVarAsymmetry"
        ];

        private static readonly String[] writableClockVariables = [
            "NtpServerUri", "TimeAdjustmentReportingThreshold", "TimeSource"
        ];

        /// <summary>
        /// The keys of a group of the "nts" section the variables of
        /// NTPClientGroupController are written under, but for Clients.
        /// </summary>
        private static readonly Dictionary<String, String> groupKeys = new (StringComparer.OrdinalIgnoreCase) {
            ["Enabled"]             = "enabled",
            ["MinServers"]          = "minServers",
            ["MaxDeviation"]        = "maxDeviationSeconds",
            ["MinInterval"]         = "checkEverySeconds",
            ["MaxInterval"]         = "checkEverySeconds",
            ["Timeout"]             = "timeoutSeconds",
            ["ErrorLogging"]        = "errorLogging",
            ["MaxErrors"]           = "maxErrors",
            ["LegalTimeAuthority"]  = "legalTimeAuthority",
            ["LegalTimeTolerance"]  = "legalTimeToleranceSeconds",
            ["LegalTimeMaxAge"]     = "legalTimeMaxAgeSeconds"
        };

        private static readonly Regex groupInError   = new (@"nts\.groups\.([a-z][a-z0-9-]*)(?:\.servers\[(\d+)\])?", RegexOptions.CultureInvariant);

        private readonly Boolean          writes;
        private readonly Action<String>?  log;

        /// <summary>
        /// One request at a time: two would each hold a table of the groups
        /// that the other one's changes make wrong.
        /// </summary>
        private readonly SemaphoreSlim    oneRequestAtATime = new (1, 1);

        #endregion

        #region Properties

        IEnumerable<String> protocols.OCPPv2_1.INTPControllerSource.  WritableControllerVariables
            => writes ? writableControllerVariables : [];

        IEnumerable<String> protocols.OCPPv2_1.INTPControllerSource.  WritableGroupVariables
            => writes ? writableGroupVariables      : [];

        IEnumerable<String> protocols.OCPPv2_1.INTPControllerSource.  WritableClientVariables
            => writes ? writableClientVariables     : [];

        IEnumerable<String> protocols.OCPPv2_1.IClockControllerSource.WritableVariables
            => writes ? writableClockVariables      : [];

        /// <summary>
        /// The components of which a request may name an instance this node
        /// does not have yet: a group named anew in ClientGroups, and a server
        /// named anew in Clients.
        /// </summary>
        public IEnumerable<String> NewInstanceComponents
            => writes ? [ nameof(NTPClientGroupController), nameof(NTPClientController) ] : [];

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The Device Model view of the time servers of the given node, which a
        /// CSMS can change as well.
        /// </summary>
        /// <param name="Node">The node whose groups of time servers are shown and changed, asked anew at every read and at every request.</param>
        /// <param name="Log">Where a change is said before it is made.</param>
        public NTSDeviceModel(WWCPNode         Node,
                              Action<String>?  Log)

            : this(Node)

        {

            this.writes  = true;
            this.log     = Log;

        }

        #endregion


        #region NewInstance(Component, Request)

        /// <summary>
        /// The controller a request makes of a group or a server this node
        /// does not have yet, standing in for it while the request is set: a
        /// group the request's ClientGroups names, or a server the Clients of
        /// one of its groups names - and nothing else.
        /// </summary>
        /// <param name="Component">NTPClientGroupController or NTPClientController, with an instance this node does not have.</param>
        /// <param name="Request">The whole request.</param>
        public ComponentConfig? NewInstance(Component            Component,
                                            SetVariablesRequest  Request)
        {

            if (!writes || Component.Instance is not String instance)
                return null;

            if (String.Equals(Component.Name, nameof(NTPClientGroupController), StringComparison.OrdinalIgnoreCase))
                return NTSSection.IsGroupId(instance) &&
                       GroupOf(instance) is null &&
                       Request.VariableData.Any(entry => String.Equals(entry.Component.Name, nameof(NTPController), StringComparison.OrdinalIgnoreCase) &&
                                                         String.Equals(entry.Variable. Name, "ClientGroups",        StringComparison.OrdinalIgnoreCase) &&
                                                         ListOf(entry.AttributeValue).Contains(instance))
                           ? NTPClientGroupController.NewInstance(this, instance)
                           : null;

            if (String.Equals(Component.Name, nameof(NTPClientController), StringComparison.OrdinalIgnoreCase))
            {

                if (!NTSServerConfiguration.IsServerId(instance) ||
                    node.NTSGroups.Any(group => group.ServerIds.Contains(instance, StringComparer.OrdinalIgnoreCase)))
                {
                    return null;
                }

                var named = Request.VariableData.FirstOrDefault(entry => String.Equals(entry.Component.Name, nameof(NTPClientGroupController), StringComparison.OrdinalIgnoreCase) &&
                                                                         String.Equals(entry.Variable. Name, "Clients",                          StringComparison.OrdinalIgnoreCase) &&
                                                                         entry.Component.Instance is not null &&
                                                                         ListOf(entry.AttributeValue).Contains(instance));

                return named is not null
                           ? NTPClientController.NewInstance(this, named.Component.Instance!, instance)
                           : null;

            }

            return null;

        }

        #endregion

        #region BaseUnitOf(Component)

        /// <summary>
        /// What is set as one with BASE: a group with its servers, the legal
        /// group with ClockCtrlr - and the groups as a whole, with whatever a
        /// group made in the same request is given.
        /// </summary>
        /// <param name="Component">A component of this source.</param>
        public Object BaseUnitOf(ComponentConfig Component)

            => Component switch {
                   NTPClientGroupController { IsNew: false } group   => UnitOf(group.Group),
                   NTPClientController client when GroupOf(client.Group) is not null
                                                                     => UnitOf(client.Group),
                   ClockController                                   => UnitOf(NTSGroup.LegalId),
                   _                                                 => GroupsUnit
               };

        private static String UnitOf(String Group)
            => "group:" + Group.ToLowerInvariant();

        #endregion

        #region SetVariablesAsync(Entries, DataConsistencyModel, CancellationToken)

        /// <summary>
        /// Put the given writes to the NTP controllers and ClockCtrlr into the node.
        /// </summary>
        /// <param name="Entries">Entries of this source's components, in request order.</param>
        /// <param name="DataConsistencyModel">IndependentRequests, ACID or BASE.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        public async Task<IReadOnlyList<SetVariableResult>> SetVariablesAsync(IReadOnlyList<SetVariableEntry>  Entries,
                                                                              DataConsistencyModel             DataConsistencyModel,
                                                                              CancellationToken                CancellationToken)
        {

            var results = new SetVariableResult?[Entries.Count];

            if (!writes)
            {
                for (var index = 0; index < Entries.Count; index++)
                    results[index] = Result(Entries[index], SetVariableStatus.Rejected, "ReadOnly", $"'{Where(Entries[index])}' is read-only!");
                return [.. results.Select(result => result!)];
            }

            await oneRequestAtATime.WaitAsync(CancellationToken).ConfigureAwait(false);

            try
            {

                var table  = Table();
                var items  = new List<Item>();

                #region Every entry by itself

                for (var index = 0; index < Entries.Count; index++)
                {

                    var entry  = Entries[index];
                    var item   = ItemOf(index, entry);
                    var value  = entry.Data.AttributeValue ?? "";

                    if (item is null)
                    {
                        results[index] = Result(entry, SetVariableStatus.Rejected, "ReadOnly", $"'{Where(entry)}' is read-only!");
                        continue;
                    }

                    if (!TryCheck(item, out var refusal))
                    {
                        results[index] = Result(entry, SetVariableStatus.Rejected, refusal.ReasonCode, refusal.AdditionalInfo);
                        continue;
                    }

                    // Compare and set: the CSMS changes nothing it has not seen.
                    var now = ValueNow(item);

                    if (entry.Data.OldAttributeValue is String old && old != now)
                    {
                        results[index] = Result(entry, SetVariableStatus.Rejected, "ValueChanged", $"'{Where(entry)}' is \"{now}\" now, not \"{old}\"!");
                        continue;
                    }

                    items.Add(item);

                }

                // Which of two values is meant cannot be told.
                foreach (var twice in items.GroupBy(item => item.Key).Where(group => group.Count() > 1).ToArray())
                    foreach (var item in twice)
                    {
                        results[item.Index] = Result(item.Entry, SetVariableStatus.Rejected, "InvalidValue", $"'{Where(item.Entry)}' is set more than once in this request!");
                        items.Remove(item);
                    }

                #endregion

                if (DataConsistencyModel == DataConsistencyModel.ACID || DataConsistencyModel == DataConsistencyModel.BASE)
                {

                    // All of it or nothing: what was not refused itself is rolled
                    // back, as the entries refused before anything was tried.
                    if (results.Any(result => result is not null))
                    {
                        foreach (var item in items)
                            results[item.Index] = Result(item.Entry, SetVariableStatus.Rolledback, "Unspecified", "Not set, because another entry of the same change could not be!");
                    }

                    else if (items.Count > 0)
                        await ApplyAsync(items, table, results, true, CancellationToken).ConfigureAwait(false);

                }

                else
                    foreach (var item in items)
                    {

                        if (item.IsNew)
                        {
                            results[item.Index] = Result(item.Entry, SetVariableStatus.Rejected, "InvalidValue",
                                                         $"'{Where(item.Entry)}' is not there yet: a new group or server is made together with what it needs, with the DataConsistencyModel ACID or BASE!");
                            continue;
                        }

                        await ApplyAsync([ item ], table, results, false, CancellationToken).ConfigureAwait(false);

                        // What the next entry is changed from: what this one left.
                        table = Table();

                    }

                return [.. results.Select(result => result!)];

            }
            finally
            {
                oneRequestAtATime.Release();
            }

        }

        #endregion


        #region (private) ApplyAsync(Change, Table, Results, Atomic, CancellationToken)

        /// <summary>
        /// Put one change into the node, and say for each of its entries what
        /// came of it.
        /// </summary>
        private async Task ApplyAsync(IReadOnlyList<Item>                Change,
                                      IReadOnlyDictionary<String, Group>  Table,
                                      SetVariableResult?[]                Results,
                                      Boolean                             Atomic,
                                      CancellationToken                   CancellationToken)
        {

            var groups    = Table.ToDictionary(pair => pair.Key, pair => pair.Value.Copy(), StringComparer.OrdinalIgnoreCase);
            var patches   = new Dictionary<String, JObject>(StringComparer.OrdinalIgnoreCase);
            var add       = new List<String>();
            var remove    = new List<String>();

            #region Groups made and taken away

            if (Change.FirstOrDefault(item => item.Kind == Kind.ClientGroups) is Item clientGroups)
            {

                var named = ListOf(clientGroups.Value);

                if (named.FirstOrDefault(id => !NTSSection.IsGroupId(id)) is String wrong)
                {
                    Refuse(Change, [ clientGroups ], Results, Atomic, "InvalidValue",
                           $"'{wrong}' cannot be the id of a group, which is lower-case letters, digits and hyphens, beginning with a letter!");
                    return;
                }

                add   .AddRange(named.Where (id => !groups.ContainsKey(id)).Distinct());
                remove.AddRange(groups.Keys.Where(id => !named.Contains(id, StringComparer.OrdinalIgnoreCase)));

                if (remove.FirstOrDefault(id => groups[id].Enabled) is String switchedOn)
                {
                    Refuse(Change, [ clientGroups ], Results, Atomic, "InvalidValue",
                           $"'{nameof(NTPClientGroupController)}[{switchedOn}]' is switched on: switch it off before taking it away!");
                    return;
                }

                foreach (var id in add)
                    groups[id] = new Group(id, Exists: false, Enabled: false, CheckEvery: null, Servers: []);

            }

            #endregion

            #region What changes in each group

            foreach (var item in Change.Where(item => item.Kind is Kind.Group or Kind.Client or Kind.NtpServerUri or Kind.ReportingThreshold))
            {

                var id = item.Group!;

                if (remove.Contains(id, StringComparer.OrdinalIgnoreCase))
                {
                    Refuse(Change, [ item ], Results, Atomic, "InvalidValue", $"'{Where(item.Entry)}' is of a group this request takes away!");
                    return;
                }

                if (!groups.ContainsKey(id))
                {
                    Refuse(Change, [ item ], Results, Atomic, "InvalidValue", $"'{Where(item.Entry)}' is of a group this station does not have!");
                    return;
                }

                if (item.Kind == Kind.Group && !item.Is("Clients"))
                {

                    var patch = patches.TryGetValue(id, out var existing) ? existing : patches[id] = new JObject();

                    // Both intervals are the one the node checks at, written once.
                    if (item.Is("MinInterval") || item.Is("MaxInterval"))
                        continue;

                    if (item.Is("Timeout") && item.Value.Length == 0)
                    {
                        Refuse(Change, [ item ], Results, Atomic, "InvalidValue", $"'{Where(item.Entry)}' cannot be emptied here: give it the time it is to have!");
                        return;
                    }

                    patch[groupKeys[item.Name]] = GroupJSON(item.Name, item.Value);

                }

                // ClockCtrlr's threshold is the legal group's, and an empty one
                // is never - not the five seconds the key left out would be.
                else if (item.Kind == Kind.ReportingThreshold)
                {

                    var patch = patches.TryGetValue(id, out var existing) ? existing : patches[id] = new JObject();

                    patch[NTSConfiguration.OffsetReportingThresholdKey] = item.Value.Length == 0
                                                                              ? new JValue(false)
                                                                              : new JValue(Int64.Parse(item.Value, CultureInfo.InvariantCulture));

                }

            }

            // MinInterval and MaxInterval are the one interval the node checks
            // at: both are written, and only to the same value.
            foreach (var byGroup in Change.Where(item => item.Is("MinInterval") || item.Is("MaxInterval")).GroupBy(item => item.Group!, StringComparer.OrdinalIgnoreCase))
            {

                var now = groups[byGroup.Key].CheckEvery?.ToString(CultureInfo.InvariantCulture) ?? "";
                var min = byGroup.FirstOrDefault(item => item.Is("MinInterval"))?.Value ?? now;
                var max = byGroup.FirstOrDefault(item => item.Is("MaxInterval"))?.Value ?? now;

                if (min != max)
                {
                    Refuse(Change, [.. byGroup], Results, Atomic, "InvalidValue",
                           $"'{nameof(NTPClientGroupController)}[{byGroup.Key}]' checks at one interval, not between two: set MinInterval and MaxInterval to the same value, in one request with the DataConsistencyModel ACID!");
                    return;
                }

                if (min.Length == 0)
                {
                    Refuse(Change, [.. byGroup], Results, Atomic, "InvalidValue",
                           $"'{nameof(NTPClientGroupController)}[{byGroup.Key}]' cannot be left without an interval: give it the one it is to check at!");
                    return;
                }

                var patch = patches.TryGetValue(byGroup.Key, out var existing) ? existing : patches[byGroup.Key] = new JObject();
                patch["checkEverySeconds"] = new JValue(Int64.Parse(min, CultureInfo.InvariantCulture));

            }

            #endregion

            #region The servers of each group they change in

            foreach (var byGroup in Change.Where(item => item.Kind is Kind.Client or Kind.NtpServerUri || item.Is("Clients")).GroupBy(item => item.Group!, StringComparer.OrdinalIgnoreCase))
            {

                var group   = groups[byGroup.Key];
                var servers = group.Servers;
                var touched = new Dictionary<String, Item>(StringComparer.OrdinalIgnoreCase);

                #region ClockCtrlr.NtpServerUri, by the positions the request found

                var asked   = Table.TryGetValue(byGroup.Key, out var read) ? read.Asked() : [];
                var next    = asked.Count + 1;

                foreach (var item in byGroup.Where(item => item.Kind == Kind.NtpServerUri).OrderBy(item => item.Position))
                {

                    var at = item.Position!.Value;

                    if (at <= asked.Count)
                    {

                        var server = servers.FirstOrDefault(server => server.SameAs(asked[at - 1]));

                        if (server is null)
                            continue;

                        if (!touched.TryAdd(server.Key, item))
                        {
                            Refuse(Change, [ item, touched[server.Key] ], Results, Atomic, "InvalidValue", $"'{Where(item.Entry)}' and '{Where(touched[server.Key].Entry)}' change the same server!");
                            return;
                        }

                        if (item.Value.Length == 0)
                        {
                            servers.Remove(server);
                            continue;
                        }

                        if (!TryParseServer(item.Value, Where(item.Entry), out var host, out var port, out var problem))
                        {
                            Refuse(Change, [ item ], Results, Atomic, "InvalidValue", problem);
                            return;
                        }

                        server.Rename(host, port);
                        continue;

                    }

                    // An unused position emptied: nothing to do, and nothing wrong.
                    if (item.Value.Length == 0)
                        continue;

                    if (at != next)
                    {
                        Refuse(Change, [ item ], Results, Atomic, "InvalidValue",
                               $"'{Where(item.Entry)}' is beyond the servers the legal group asks: the next free one is {ClockController.ComponentName}.NtpServerUri[{next}]!");
                        return;
                    }

                    if (!TryParseServer(item.Value, Where(item.Entry), out var newHost, out var newPort, out var newProblem))
                    {
                        Refuse(Change, [ item ], Results, Atomic, "InvalidValue", newProblem);
                        return;
                    }

                    // In the band of the last server asked, so that it counts
                    // towards the same quorum.
                    var added = Server.New(null, newHost, newPort, asked.Count > 0 ? asked[^1].Priority : (Byte) 0, group.DefaultNTSKEPort, group.DefaultNTPPort);

                    servers.Add(added);
                    touched[added.Key] = item;
                    next++;

                }

                #endregion

                #region Clients: servers taken out, and new ones put in

                if (byGroup.FirstOrDefault(item => item.Is("Clients")) is Item clients)
                {

                    var named = ListOf(clients.Value);

                    foreach (var server in servers.Where(server => server.Id is not null && !named.Contains(server.Id, StringComparer.OrdinalIgnoreCase)).ToArray())
                    {

                        if (touched.TryGetValue(server.Key, out var other))
                        {
                            Refuse(Change, [ clients, other ], Results, Atomic, "InvalidValue", $"'{Where(clients.Entry)}' takes away the server '{Where(other.Entry)}' changes!");
                            return;
                        }

                        servers.Remove(server);

                    }

                    foreach (var id in named.Where(id => !servers.Any(server => String.Equals(server.Id, id, StringComparison.OrdinalIgnoreCase))))
                    {

                        var url = Change.FirstOrDefault(item => item.Kind == Kind.Client && item.IsNew && String.Equals(item.Client, id, StringComparison.OrdinalIgnoreCase) && item.Is("ServerURL"));

                        if (url is null)
                        {
                            Refuse(Change, [ clients ], Results, Atomic, "InvalidValue",
                                   $"'{nameof(NTPClientController)}[{id}]' is new: set its ServerURL in the same request, with the DataConsistencyModel ACID or BASE!");
                            return;
                        }

                        if (!TryParseServer(url.Value, Where(url.Entry), out var host, out var port, out var problem))
                        {
                            Refuse(Change, [ url ], Results, Atomic, "InvalidValue", problem);
                            return;
                        }

                        servers.Add(Server.New(id, host, port, 0, group.DefaultNTSKEPort, group.DefaultNTPPort));

                    }

                }

                #endregion

                #region What is set of each server

                foreach (var item in byGroup.Where(item => item.Kind == Kind.Client))
                {

                    var server = servers.FirstOrDefault(server => String.Equals(server.Id, item.Client, StringComparison.OrdinalIgnoreCase));

                    if (server is null)
                    {
                        Refuse(Change, [ item ], Results, Atomic, "InvalidValue", $"'{Where(item.Entry)}' is of a server this request takes away!");
                        return;
                    }

                    if (touched.TryGetValue(server.Key, out var other) && other.Kind == Kind.NtpServerUri)
                    {
                        Refuse(Change, [ item, other ], Results, Atomic, "InvalidValue", $"'{Where(item.Entry)}' and '{Where(other.Entry)}' change the same server!");
                        return;
                    }

                    touched.TryAdd(server.Key, item);

                    if (item.IsNew && item.Is("ServerURL"))
                        continue;

                    if (!server.TrySet(item.Name, item.Value, Where(item.Entry), out var problem))
                    {
                        Refuse(Change, [ item ], Results, Atomic, "InvalidValue", problem);
                        return;
                    }

                }

                // A server renamed keeps no pin of the certificate of the old
                // one, unless the same change says which ones it keeps.
                foreach (var server in servers.Where(server => server.Renamed))
                    if (!byGroup.Any(item => item.Kind == Kind.Client && item.Is("CertificatePins") && String.Equals(item.Client, server.Id, StringComparison.OrdinalIgnoreCase)))
                        server.Certificates.Clear();

                #endregion

                var patch = patches.TryGetValue(byGroup.Key, out var existing) ? existing : patches[byGroup.Key] = new JObject();
                patch["servers"] = new JArray(servers.Select(server => server.ToJSON(group.DefaultNTSKEPort, group.DefaultNTPPort)));

            }

            #endregion

            #region ClockCtrlr.TimeSource: the one the NTS configuration says, or none

            foreach (var item in Change.Where(item => item.Kind == Kind.TimeSource))
            {

                var now = ClockValue("TimeSource") ?? "";

                if (!SameList(item.Value, now))
                {
                    Refuse(Change, [ item ], Results, Atomic, "InvalidValue",
                           $"'{Where(item.Entry)}' follows the time servers here: it is \"{now}\" while the legal group is switched {(node.NTSEnabled ? "on" : "off")}!");
                    return;
                }

            }

            #endregion

            if (add.Count == 0 && remove.Count == 0 && patches.Count == 0)
            {
                foreach (var item in Change)
                    Results[item.Index] = Result(item.Entry, SetVariableStatus.Accepted);
                return;
            }

            var origin    = ChangeOrigin.OCPP(SenderOf(Change));
            var elsewhere = false;

            log?.Invoke($"A CSMS changes the time servers of this charging station (by {origin}):" +
                        (add.   Count > 0 ? $" groups made: {String.Join(", ", add)};"    : "") +
                        (patches.Count > 0 ? $" {new JObject(patches.Select(patch => new JProperty(patch.Key, patch.Value))).ToString(Formatting.None)};" : "") +
                        (remove.Count > 0 ? $" groups taken away: {String.Join(", ", remove)};" : ""));

            #region Groups made first

            var made = new List<String>();

            foreach (var id in add)
            {

                if (!node.TryAddNTSGroup(id, null, out _, out var error, out var notSaved, origin))
                {

                    await TakeAwayAsync(made, origin).ConfigureAwait(false);

                    Refuse(Change, [.. Change.Where(item => item.Kind == Kind.ClientGroups)], Results, Atomic, notSaved ? "InternalError" : "InvalidValue",
                           notSaved ? $"The group '{id}' could not be saved, and nothing of this change was put into effect: {error}" : Rewritten(error, patches));

                    return;

                }

                made.Add(id);

            }

            #endregion

            #region The groups' changes, in one save

            if (patches.Count > 0)
            {

                // Made from the groups as the node holds them when nothing else
                // can change them: a list of servers changed elsewhere since the
                // request read it would lose that change to the list sent back.
                if (!node.TryUpdateNTSConfigurations(now => {

                                                         foreach (var (id, patch) in patches)
                                                             if (patch["servers"] is not null &&
                                                                 now.FirstOrDefault(group => group.Id == id) is NTSGroup changed &&
                                                                 !SameServers(ServersOf(node.NTSConfigurationJSON(changed)), Table.TryGetValue(id, out var was) ? was.Servers : []))
                                                             {
                                                                 elsewhere = true;
                                                                 return null;
                                                             }

                                                         return [.. patches.Select(patch => (now.First(group => group.Id == patch.Key), patch.Value))];

                                                     },
                                                     out var error,
                                                     out var refused,
                                                     out var notSaved,
                                                     origin))
                {

                    await TakeAwayAsync(made, origin).ConfigureAwait(false);

                    if (notSaved)
                        Refuse(Change, Change, Results, false, "InternalError",
                               $"The time servers could not be saved, and nothing of this change was put into effect: {error}");

                    else
                        Refuse(Change, CausedBy(Change, error, refused, patches), Results, Atomic, "InvalidValue", Rewritten(error, patches));

                    return;

                }

                if (elsewhere)
                {

                    await TakeAwayAsync(made, origin).ConfigureAwait(false);

                    Refuse(Change, Change, Results, false, "ValueChanged",
                           "The time servers were changed elsewhere while this request was made, and nothing of it was put into effect: read them again!");

                    return;

                }

            }

            #endregion

            #region Groups taken away last

            foreach (var id in remove)
            {

                var (removed, error, notSaved) = await node.TryRemoveNTSGroupAsync(id, origin).ConfigureAwait(false);

                // Too late to take the rest back: it is in effect, and only the
                // entry that wanted the group gone says that it is not.
                if (!removed)
                {

                    foreach (var item in Change)
                        Results[item.Index] = item.Kind == Kind.ClientGroups
                                                  ? Result(item.Entry, SetVariableStatus.Rejected, notSaved ? "InternalError" : "InvalidValue",
                                                           Shortened($"The group '{id}' could not be taken away, while the rest of this change is in effect: {error}"))
                                                  : Result(item.Entry, SetVariableStatus.Accepted);

                    return;

                }

            }

            #endregion

            foreach (var item in Change)
                Results[item.Index] = Result(item.Entry, SetVariableStatus.Accepted);

        }

        #endregion

        #region (private) TakeAwayAsync(Made, Origin)

        /// <summary>
        /// Take away again the groups a refused change made: switched off and
        /// without servers, there is nothing they could have done meanwhile.
        /// </summary>
        private async Task TakeAwayAsync(IEnumerable<String>  Made,
                                         String               Origin)
        {
            foreach (var id in Made)
                await node.TryRemoveNTSGroupAsync(id, Origin).ConfigureAwait(false);
        }

        #endregion


        #region (private) Table()

        /// <summary>
        /// Every group as the node has it now, with its servers as the page
        /// would send them back.
        /// </summary>
        private Dictionary<String, Group> Table()

            => node.NTSGroups.ToDictionary(group => group.Id,
                                           group => Group.Of(group.Id, node.NTSConfigurationJSON(group)),
                                           StringComparer.OrdinalIgnoreCase);

        #endregion

        #region (private) ItemOf(Index, Entry)

        /// <summary>
        /// An entry as what it changes, or null where it is not one this source
        /// writes.
        /// </summary>
        private Item? ItemOf(Int32             Index,
                             SetVariableEntry  Entry)
        {

            var name   = Entry.Variable.Name;
            var value  = Entry.Data.AttributeValue ?? "";

            return Entry.Component switch {

                NTPController
                    when writableControllerVariables.Contains(name, StringComparer.OrdinalIgnoreCase)
                    => new Item(Index, Entry, Kind.ClientGroups, null, null, name, value, null, false),

                NTPClientGroupController group
                    when writableGroupVariables.Contains(name, StringComparer.OrdinalIgnoreCase)
                    => new Item(Index, Entry, Kind.Group, group.Group, null, name, value, null, group.IsNew),

                NTPClientController client
                    when writableClientVariables.Contains(name, StringComparer.OrdinalIgnoreCase)
                    => new Item(Index, Entry, Kind.Client, client.IsNew ? client.Group : GroupOfClient(client.Client) ?? client.Group, client.Client, name, value, null, client.IsNew),

                ClockController
                    when String.Equals(name, "NtpServerUri", StringComparison.OrdinalIgnoreCase) &&
                         Byte.TryParse(Entry.Variable.Instance, NumberStyles.None, CultureInfo.InvariantCulture, out var position)
                    => new Item(Index, Entry, Kind.NtpServerUri, NTSGroup.LegalId, null, name, value, position, false),

                ClockController
                    when String.Equals(name, "TimeAdjustmentReportingThreshold", StringComparison.OrdinalIgnoreCase)
                    => new Item(Index, Entry, Kind.ReportingThreshold, NTSGroup.LegalId, null, name, value, null, false),

                ClockController
                    when String.Equals(name, "TimeSource", StringComparison.OrdinalIgnoreCase)
                    => new Item(Index, Entry, Kind.TimeSource, null, null, name, value, null, false),

                _ => null

            };

        }

        private String? GroupOfClient(String Client)

            => node.NTSGroups.FirstOrDefault(group => group.ServerIds.Contains(Client, StringComparer.OrdinalIgnoreCase))?.Id;

        #endregion

        #region (private) TryCheck(Item, out Refusal)

        /// <summary>
        /// Whether the value of an entry is one its variable can be set to by
        /// its type, its limits and its values - as the paper or the standard
        /// says them - or why not.
        /// </summary>
        private Boolean TryCheck(Item                                  Item,
                                 [NotNullWhen(false)] out StatusInfo?  Refusal)

            => Item.Kind switch {
                   Kind.ClientGroups               => NTPController.           TryCheck(              Item.Name, Item.Value, out Refusal),
                   Kind.Group                      => NTPClientGroupController.TryCheck(Item.Group!,  Item.Name, Item.Value, out Refusal),
                   Kind.Client                     => NTPClientController.     TryCheck(this, Item.Client!, Item.Name, Item.Value, out Refusal),
                   _                               => ClockController.         TryCheck(              Item.Name, Item.Value, out Refusal)
               };

        #endregion

        #region (private) ValueNow(Item)

        /// <summary>
        /// What the variable of an entry reads now, as a compare and set asks
        /// it: "" for a group or server that is not there yet.
        /// </summary>
        private String ValueNow(Item Item)

            => Item.IsNew
                   ? ""
                   : Item.Kind switch {
                         Kind.ClientGroups  => String.Join(",", Groups),
                         Kind.Group         => Item.Is("Clients")
                                                   ? String.Join(",", ClientsOf(Item.Group!))
                                                   : GroupValue(Item.Group!, Item.Name) ?? "",
                         Kind.Client        => ClientValue(Item.Client!, Item.Name) ?? "",
                         _                  => ClockValue(Item.Name, Item.Entry.Variable.Instance) ?? ""
                     };

        #endregion


        #region (private static) GroupJSON(Variable, Value)

        /// <summary>
        /// A value of NTPClientGroupController as its key in a group of the
        /// "nts" section says it.
        /// </summary>
        private static JToken GroupJSON(String  Variable,
                                        String  Value)

            => Variable.ToLowerInvariant() switch {
                   "enabled"             => new JValue(String.Equals(Value, "true", StringComparison.OrdinalIgnoreCase)),
                   "legaltimeauthority"  => new JValue(Value),
                   "minservers"          => new JValue(Int64.  Parse(Value, CultureInfo.InvariantCulture)),
                   "errorlogging"        => new JValue(Int64.  Parse(Value, CultureInfo.InvariantCulture)),
                   "maxerrors"           => new JValue(Int64.  Parse(Value, CultureInfo.InvariantCulture)),
                   "legaltimemaxage"     => new JValue(Int64.  Parse(Value, CultureInfo.InvariantCulture)),
                   _                     => new JValue(Decimal.Parse(Value, CultureInfo.InvariantCulture))
               };

        #endregion

        #region (private static) TryParseServer(Value, Where, out Host, out Port, out Problem)

        /// <summary>
        /// A server as ServerURL and NtpServerUri name it: a host name, or
        /// nts://host[:port] with the port of its key exchange. Any other
        /// scheme is refused: this node speaks NTS only.
        /// </summary>
        private static Boolean TryParseServer(String                            Value,
                                              String                            Where,
                                              out String                        Host,
                                              out UInt16?                       Port,
                                              [NotNullWhen(false)] out String?  Problem)
        {

            Host     = "";
            Port     = null;
            Problem  = null;

            var text = Value.Trim();

            if (text.Contains("://", StringComparison.Ordinal))
            {

                if (!text.StartsWith("nts://", StringComparison.OrdinalIgnoreCase))
                {
                    Problem = $"'{Where}' is a host name or nts://host[:port]: this charging station asks its time servers by NTS only, not by \"{text}\"!";
                    return false;
                }

                text = text["nts://".Length..].TrimEnd('/');

                var colon = text.LastIndexOf(':');

                if (colon > 0)
                {

                    if (!UInt16.TryParse(text[(colon + 1)..], NumberStyles.None, CultureInfo.InvariantCulture, out var port) || port == 0)
                    {
                        Problem = $"'{Where}' names no port from 1 to 65535 in \"{Value}\"!";
                        return false;
                    }

                    Port = port;
                    text = text[..colon];

                }

            }

            if (text.Length == 0)
            {
                Problem = $"'{Where}' names no host!";
                return false;
            }

            Host = text.TrimEnd('.');
            return true;

        }

        #endregion

        #region (private static) SenderOf(Change)

        /// <summary>
        /// Who sent the request of a change, by the start of its network path:
        /// "CSMS" for the one the node is connected to directly, and for a
        /// request that came by no path at all.
        /// </summary>
        private static String SenderOf(IReadOnlyList<Item> Change)

            => Change.FirstOrDefault()?.Entry.Request?.NetworkPath is { Length: > 0 } path
                   ? path.Source.ToString()
                   : "CSMS";

        #endregion

        #region (private static) Refuse(Change, Causing, Results, Atomic, ReasonCode, AdditionalInfo)

        /// <summary>
        /// Refuse a change: the entries that caused it are rejected, and with ACID
        /// and BASE the others are rolled back - without them, all of it is
        /// rejected.
        /// </summary>
        private static void Refuse(IReadOnlyList<Item>   Change,
                                   IReadOnlyList<Item>   Causing,
                                   SetVariableResult?[]  Results,
                                   Boolean               Atomic,
                                   String                ReasonCode,
                                   String                AdditionalInfo)
        {

            var causing = Causing.Count > 0 ? Causing : Change;
            var info    = Shortened(AdditionalInfo);

            foreach (var item in Change)
                Results[item.Index] = !Atomic || causing.Contains(item)
                                          ? Result(item.Entry, SetVariableStatus.Rejected,   ReasonCode,    info)
                                          : Result(item.Entry, SetVariableStatus.Rolledback, "Unspecified", Shortened($"Not set, because the same change could not be made: {info}"));

        }

        private static String Shortened(String AdditionalInfo)

            => AdditionalInfo.Length > MaxAdditionalInfo
                   ? AdditionalInfo[..(MaxAdditionalInfo - 1)] + "…"
                   : AdditionalInfo;

        #endregion

        #region (private static) CausedBy(Change, Error, Refused, Patches)

        /// <summary>
        /// The entries a refusal of the node is about: those of the server it
        /// names by its place in the list sent, or else those of the group it
        /// names or blames - or none, where it names nothing.
        /// </summary>
        private static IReadOnlyList<Item> CausedBy(IReadOnlyList<Item>                    Change,
                                                    String                                 Error,
                                                    NTSGroup?                              Refused,
                                                    IReadOnlyDictionary<String, JObject>   Patches)
        {

            var match = groupInError.Match(Error);
            var group = match.Success ? match.Groups[1].Value : Refused?.Id;

            if (group is null)
                return [];

            if (match.Success &&
                match.Groups[2].Success &&
                IdOf(Patches, group, match.Groups[2].Value) is String id)
            {

                var ofServer = Change.Where(item => String.Equals(item.Client, id, StringComparison.OrdinalIgnoreCase) ||
                                                    (item.Is("Clients") && String.Equals(item.Group, group, StringComparison.OrdinalIgnoreCase))).ToArray();

                if (ofServer.Length > 0)
                    return ofServer;

            }

            return [.. Change.Where(item => String.Equals(item.Group, group, StringComparison.OrdinalIgnoreCase))];

        }

        private static String? IdOf(IReadOnlyDictionary<String, JObject>  Patches,
                                    String                                Group,
                                    String                                Index)

            => Patches.TryGetValue(Group, out var patch) &&
               patch["servers"] is JArray servers &&
               Int32.TryParse(Index, NumberStyles.None, CultureInfo.InvariantCulture, out var at) &&
               at < servers.Count
                   ? servers[at]?["id"]?.Value<String>()
                   : null;

        #endregion

        #region (private static) Rewritten(Error, Patches)

        /// <summary>
        /// A refusal of the node in the words of the Device Model: a server it
        /// names by its place in the list sent becomes the client of its id, and
        /// a group the controller of it.
        /// </summary>
        private static String Rewritten(String                                Error,
                                        IReadOnlyDictionary<String, JObject>  Patches)

            => groupInError.Replace(Error, match =>
                   match.Groups[2].Success && IdOf(Patches, match.Groups[1].Value, match.Groups[2].Value) is String id
                       ? $"{nameof(NTPClientController)}[{id}]"
                       : $"{nameof(NTPClientGroupController)}[{match.Groups[1].Value}]");

        #endregion

        #region (private static) ServersOf(Group), SameServers(Now, Read)

        /// <summary>
        /// The servers of a group, as its JSON says them, as the page would send
        /// them back.
        /// </summary>
        private static List<Server> ServersOf(JObject Group)

            => Group["timeSources"] is JArray sources
                   ? [.. sources.OfType<JObject>().Select(Server.Of)]
                   : [];

        /// <summary>
        /// Whether a group's servers are still the ones a request read: the same
        /// ones, as the same entries.
        /// </summary>
        private static Boolean SameServers(IReadOnlyList<Server>  Now,
                                           IReadOnlyList<Server>  Read)

            => Now.Count == Read.Count &&
               Now.Zip(Read).All(pair => pair.First.Same(pair.Second));

        #endregion

        #region (private static) ListOf(Value), SameList(One, Other)

        /// <summary>
        /// The entries of a comma separated list.
        /// </summary>
        private static List<String> ListOf(String? Value)

            => [.. (Value ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];

        /// <summary>
        /// Whether two comma separated lists say the same, in the same order,
        /// in any case.
        /// </summary>
        private static Boolean SameList(String  One,
                                        String  Other)

            => ListOf(One).SequenceEqual(ListOf(Other), StringComparer.OrdinalIgnoreCase);

        #endregion

        #region (private static) Where(Entry), Result(Entry, Status, ReasonCode = null, AdditionalInfo = null)

        private static String Where(SetVariableEntry Entry)

            => $"{Entry.Data.Component.Name}{(Entry.Data.Component.Instance is String instance ? $"[{instance}]" : "")}." +
               $"{Entry.Data.Variable. Name}{(Entry.Data.Variable. Instance is String which    ? $"[{which}]"    : "")}";

        private static SetVariableResult Result(SetVariableEntry   Entry,
                                                SetVariableStatus  Status,
                                                String?            ReasonCode       = null,
                                                String?            AdditionalInfo   = null)

            => new (
                   AttributeStatus:       Status,
                   Component:             Entry.Data.Component,
                   Variable:              Entry.Data.Variable,
                   AttributeType:         Entry.Variable.Attributes?.Type ?? AttributeTypes.Actual,
                   AttributeStatusInfo:   ReasonCode is not null
                                              ? new StatusInfo(ReasonCode, AdditionalInfo)
                                              : null
               );

        #endregion


        #region (private) Kind, Item

        /// <summary>
        /// What an entry changes.
        /// </summary>
        private enum Kind
        {
            ClientGroups,
            Group,
            Client,
            NtpServerUri,
            ReportingThreshold,
            TimeSource
        }

        /// <summary>
        /// An entry that may be tried: what it changes, of which group and
        /// server, and to what.
        /// </summary>
        private sealed record Item(Int32             Index,
                                   SetVariableEntry  Entry,
                                   Kind              Kind,
                                   String?           Group,
                                   String?           Client,
                                   String            Name,
                                   String            Value,
                                   Byte?             Position,
                                   Boolean           IsNew)
        {

            public Boolean Is(String Variable)
                => String.Equals(Name, Variable, StringComparison.OrdinalIgnoreCase);

            /// <summary>
            /// The same variable of the same thing: set twice in one request.
            /// </summary>
            public String Key
                => $"{Kind}/{Group?.ToLowerInvariant()}/{Client?.ToLowerInvariant()}/{Name.ToLowerInvariant()}/{Position}";

        }

        #endregion

        #region (private) Group

        /// <summary>
        /// A group as a request found it: whether it is switched on, its
        /// interval, and its servers.
        /// </summary>
        private sealed record Group(String        Id,
                                    Boolean       Exists,
                                    Boolean       Enabled,
                                    Int64?        CheckEvery,
                                    List<Server>  Servers)
        {

            public UInt16 DefaultNTSKEPort { get; init; } = 4460;
            public UInt16 DefaultNTPPort   { get; init; } = 123;

            public static Group Of(String   Id,
                                   JObject  JSON)

                => new (Id,
                        Exists:      true,
                        Enabled:     JSON["enabled"]?.Type == JTokenType.Boolean && JSON.Value<Boolean>("enabled"),
                        CheckEvery:  JSON["settings"]?["checkEverySeconds"]?.Type is JTokenType.Integer or JTokenType.Float
                                         ? JSON["settings"]!["checkEverySeconds"]!.Value<Int64>()
                                         : null,
                        Servers:     ServersOf(JSON)) {

                       DefaultNTSKEPort  = JSON["limits"]?["defaultNTSKEPort"]?.Value<UInt16>() ?? 4460,
                       DefaultNTPPort    = JSON["limits"]?["defaultNTPPort"]?  .Value<UInt16>() ?? 123

                   };

            public Group Copy()
                => this with { Servers = [.. Servers.Select(server => server.Clone())] };

            /// <summary>
            /// The servers the group asks, in the order it asks them: by
            /// priority, then as its list names them - what NtpServerUri counts.
            /// </summary>
            public IReadOnlyList<Server> Asked()
                => [.. Servers.Where(server => server.Enabled).OrderBy(server => server.Priority)];

        }

        #endregion

        #region (private) Server

        /// <summary>
        /// A time server as the page sends it back: its entry, and what the
        /// request found it held to.
        /// </summary>
        private sealed class Server
        {

            private static Int32 made;

            public String?       Id               { get; private set; }
            public String        Hostname         { get; private set; } = "";
            public Byte          Priority         { get; private set; }
            public UInt16?       NTSKEPort        { get; private set; }
            public UInt16?       NTPPort          { get; private set; }
            public Boolean       Enabled          { get; private set; } = true;
            public List<String>  Certificates     { get; private set; } = [];
            public List<String>  Roots            { get; private set; } = [];
            public String        OnMismatch       { get; private set; } = "refuse";
            public String?       TrustOnFirstUse  { get; private set; }
            public String?       RootGroup        { get; private set; }
            public String?       AEAD             { get; private set; }
            public UInt32?       MinRefresh       { get; private set; }
            public UInt32?       MaxRefresh       { get; private set; }
            public Decimal?      DelayAsymmetry   { get; private set; }
            public Decimal?      DelayVarAsymmetry { get; private set; }
            public JObject?      Shown            { get; private set; }
            public Boolean       Renamed          { get; private set; }

            /// <summary>
            /// What tells servers apart within one change: the id, or for a new
            /// server without one, a number of its own.
            /// </summary>
            public String        Key              { get; private set; } = "";

            public static Server Of(JObject JSON)
            {

                var heldTo = JSON["heldTo"] as JObject;
                var server = new Server {
                                 Id               = JSON["id"]?.Value<String>(),
                                 Hostname         = (JSON["hostname"]?.Value<String>() ?? "").TrimEnd('.'),
                                 Priority         = JSON["priority"] ?.Value<Byte>()   ?? 0,
                                 NTSKEPort        = JSON["ntsKEPort"]?.Value<UInt16>(),
                                 NTPPort          = JSON["ntpPort"]  ?.Value<UInt16>(),
                                 Enabled          = JSON["enabled"]  ?.Value<Boolean>() ?? true,
                                 Certificates     = PinsIn(heldTo, "certificate"),
                                 Roots            = PinsIn(heldTo, "root"),
                                 OnMismatch       = heldTo?["onMismatch"]?.Value<String>() ?? "refuse",
                                 TrustOnFirstUse  = heldTo?["trustOnFirstUse"]?.Value<String>() is String learn && learn is "root" or "certificate" ? learn : null,
                                 RootGroup        = heldTo?["rootGroup"]?.Value<String>(),
                                 // What its own entry offers, which the node
                                 // shows beside what was agreed on: carried
                                 // through every change of the list, or a
                                 // change of anything else would take it away.
                                 AEAD             = JSON["aead"]?.Value<String>() is { Length: > 0 } aead ? aead : null,
                                 // And when its key exchange is made again, where
                                 // its own entry sets that, by the same rule.
                                 MinRefresh       = JSON["minRefreshSeconds"]?.Type is JTokenType.Integer or JTokenType.Float ? JSON["minRefreshSeconds"]!.Value<UInt32>() : null,
                                 MaxRefresh       = JSON["maxRefreshSeconds"]?.Type is JTokenType.Integer or JTokenType.Float ? JSON["maxRefreshSeconds"]!.Value<UInt32>() : null,
                                 // And how its offsets are corrected.
                                 DelayAsymmetry   = JSON["delayAsymmetryMs"] ?.Type is JTokenType.Integer or JTokenType.Float ? JSON["delayAsymmetryMs"]! .Value<Decimal>() : null,
                                 DelayVarAsymmetry = JSON["delayVarAsymmetry"]?.Type is JTokenType.Integer or JTokenType.Float ? JSON["delayVarAsymmetry"]!.Value<Decimal>() : null
                             };

                server.Key   = server.Id ?? $"#{Interlocked.Increment(ref made)}";
                server.Shown = server.PinsJSON();

                return server;

            }

            public static Server New(String?  Id,
                                     String   Hostname,
                                     UInt16?  NTSKEPort,
                                     Byte     Priority,
                                     UInt16   DefaultNTSKEPort,
                                     UInt16   DefaultNTPPort)

                => new () {
                       Id         = Id,
                       Hostname   = Hostname,
                       Priority   = Priority,
                       NTSKEPort  = NTSKEPort ?? DefaultNTSKEPort,
                       NTPPort    = DefaultNTPPort,
                       Key        = Id ?? $"#{Interlocked.Increment(ref made)}"
                   };

            public Server Clone()

                => new () {
                       Id               = Id,
                       Hostname         = Hostname,
                       Priority         = Priority,
                       NTSKEPort        = NTSKEPort,
                       NTPPort          = NTPPort,
                       Enabled          = Enabled,
                       Certificates     = [.. Certificates],
                       Roots            = [.. Roots],
                       OnMismatch       = OnMismatch,
                       TrustOnFirstUse  = TrustOnFirstUse,
                       RootGroup        = RootGroup,
                       AEAD             = AEAD,
                       MinRefresh       = MinRefresh,
                       MaxRefresh       = MaxRefresh,
                       DelayAsymmetry   = DelayAsymmetry,
                       DelayVarAsymmetry = DelayVarAsymmetry,
                       Shown            = Shown,
                       Renamed          = Renamed,
                       Key              = Key
                   };

            /// <summary>
            /// The server another one of a table is: by its id.
            /// </summary>
            public Boolean SameAs(Server Other)
                => Key == Other.Key;

            /// <summary>
            /// The same server with the same entry: what a list changed elsewhere
            /// is told apart by.
            /// </summary>
            public Boolean Same(Server Other)
                => ToJSON(0, 0).ToString(Formatting.None) == Other.ToJSON(0, 0).ToString(Formatting.None);

            public void Rename(String   Host,
                               UInt16?  Port)
            {

                if (!String.Equals(Hostname, Host, StringComparison.OrdinalIgnoreCase))
                    Renamed = true;

                Hostname = Host;

                if (Port.HasValue)
                    NTSKEPort = Port;

            }

            public Boolean TrySet(String                            Variable,
                                  String                            Value,
                                  String                            Where,
                                  [NotNullWhen(false)] out String?  Problem)
            {

                Problem = null;

                switch (Variable.ToLowerInvariant())
                {

                    case "enabled":
                        Enabled = String.Equals(Value, "true", StringComparison.OrdinalIgnoreCase);
                        break;

                    case "serverurl":
                        if (!TryParseServer(Value, Where, out var host, out var port, out Problem))
                            return false;
                        Rename(host, port);
                        break;

                    case "serverport":
                        NTPPort = UInt16.Parse(Value, CultureInfo.InvariantCulture);
                        break;

                    case "ntskeserverport":
                        NTSKEPort = UInt16.Parse(Value, CultureInfo.InvariantCulture);
                        break;

                    case "priority":
                        Priority = Byte.Parse(Value, CultureInfo.InvariantCulture);
                        break;

                    case "certificatepins":
                        Certificates = ListOf(Value);
                        break;

                    case "rootpins":
                        Roots = ListOf(Value);
                        break;

                    case "onpinmismatch":
                        OnMismatch = Value.ToLowerInvariant();
                        break;

                    case "trustonfirstuse":
                        TrustOnFirstUse = String.Equals(Value, "None", StringComparison.OrdinalIgnoreCase) ? null : Value.ToLowerInvariant();
                        break;

                    // The group of the roots installed as NetworkTimeRootCertificate
                    // with that certificateGroup: the node holds the server to the
                    // roots of its store marked for the group, and to no other.
                    case "rootcas":
                        if (Value.Length > 0 && !ServerPins.IsRootGroup(Value))
                        {
                            Problem = $"{Where}: '{Value}' cannot name a group of roots here: up to {ServerPins.MaxRootGroupLength} letters, digits, '-' or '_'.";
                            return false;
                        }
                        RootGroup = Value.Length > 0 ? Value : null;
                        break;

                    // The one algorithm its key exchange offers, written as the
                    // node names it, whatever its case; "" offers every one
                    // again, the server choosing.
                    case "aeadalgorithm":
                        if (Value.Length == 0)
                            AEAD = null;
                        else if (NTSServerConfiguration.TryParseAEAD(Value, out var aead))
                            AEAD = NTSServerConfiguration.AEADNameOf(aead);
                        else
                        {
                            Problem = $"{Where}: '{Value}' is not an AEAD algorithm this charging station offers, which are " +
                                      $"{String.Join(" and ", NTSServerConfiguration.AEADAlgorithmsOffered.Select(NTSServerConfiguration.AEADNameOf))}.";
                            return false;
                        }
                        break;

                    // When its key exchange is made again, in seconds: set on its
                    // entry, which the node holds to its bounds, and the latest
                    // never before the earliest.
                    case "minrefresh":
                    case "maxrefresh":
                        if (!UInt32.TryParse(Value, NumberStyles.None, CultureInfo.InvariantCulture, out var seconds) ||
                            seconds < NTSServerConfiguration.MinRefreshSeconds ||
                            seconds > NTSServerConfiguration.MaxRefreshSeconds)
                        {
                            Problem = $"{Where}: '{Value}' is not a number of seconds from {NTSServerConfiguration.MinRefreshSeconds} to {NTSServerConfiguration.MaxRefreshSeconds}.";
                            return false;
                        }
                        if (Variable.Equals("MinRefresh", StringComparison.OrdinalIgnoreCase))
                            MinRefresh = seconds;
                        else
                            MaxRefresh = seconds;
                        break;

                    // How its offsets are corrected: a number of milliseconds
                    // added to each, and where a hold-up of the delays is, at most
                    // a half either way; "" or 0 corrects nothing.
                    case "delayasymmetry":
                    case "delayvarasymmetry":
                        var limit = Variable.Equals("DelayAsymmetry", StringComparison.OrdinalIgnoreCase)
                                        ? (Decimal) NTSServerConfiguration.MaxDelayAsymmetryMs
                                        : (Decimal) NTSServerConfiguration.MaxDelayVarAsymmetry;
                        Decimal? correction = null;
                        if (Value.Length > 0)
                        {
                            if (!Decimal.TryParse(Value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var number) ||
                                number < -limit || number > limit)
                            {
                                Problem = $"{Where}: '{Value}' is not a number from {(-limit).ToString(CultureInfo.InvariantCulture)} to {limit.ToString(CultureInfo.InvariantCulture)}.";
                                return false;
                            }
                            correction = number != 0 ? number : null;
                        }
                        if (Variable.Equals("DelayAsymmetry", StringComparison.OrdinalIgnoreCase))
                            DelayAsymmetry = correction;
                        else
                            DelayVarAsymmetry = correction;
                        break;

                }

                return true;

            }

            /// <summary>
            /// What the server is held to, in the keys a group's list writes it
            /// under - as the page's keysOf writes them.
            /// </summary>
            public JObject PinsJSON()
            {

                var json          = new JObject();
                var certificates  = Certificates.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
                var roots         = Roots.       Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

                if      (certificates.Length == 1)  json["certificateFingerprint"]   = certificates[0];
                else if (certificates.Length >  1)  json["certificateFingerprints"]  = new JArray(certificates);

                if      (roots.Length == 1)         json["rootFingerprint"]          = roots[0];
                else if (roots.Length >  1)         json["rootFingerprints"]         = new JArray(roots);

                var something = certificates.Length > 0 || roots.Length > 0 || TrustOnFirstUse is not null;

                if (OnMismatch != "refuse" && something)
                    json["onMismatch"] = OnMismatch;

                if (TrustOnFirstUse is not null)
                    json["trustOnFirstUse"] = TrustOnFirstUse;

                // No pin, so nothing a mismatch comes to is said for it alone.
                if (RootGroup is not null)
                    json["rootGroup"] = RootGroup;

                return json;

            }

            /// <summary>
            /// The server as a group's list says it - with the usual left out,
            /// as the page leaves it out - and what the request found it held to.
            /// </summary>
            public JObject ToJSON(UInt16  DefaultNTSKEPort,
                                  UInt16  DefaultNTPPort)
            {

                var json = new JObject(new JProperty("hostname", Hostname));

                if (Id is not null)                                            json["id"]         = Id;
                if (Priority != 0)                                             json["priority"]   = Priority;
                if (NTSKEPort is UInt16 keyPort && keyPort != DefaultNTSKEPort) json["ntsKEPort"]  = keyPort;
                if (NTPPort   is UInt16 ntpPort && ntpPort != DefaultNTPPort)   json["ntpPort"]    = ntpPort;
                if (!Enabled)                                                  json["enabled"]    = false;
                if (AEAD is not null)                                          json["aead"]       = AEAD;
                if (MinRefresh is UInt32 minRefresh)                           json["minRefreshSeconds"] = minRefresh;
                if (MaxRefresh is UInt32 maxRefresh)                           json["maxRefreshSeconds"] = maxRefresh;
                if (DelayAsymmetry    is Decimal shift)                        json["delayAsymmetryMs"]  = shift;
                if (DelayVarAsymmetry is Decimal asymmetry)                    json["delayVarAsymmetry"] = asymmetry;

                json.Merge(PinsJSON());

                if (Shown is not null)
                    json[ServerPins.AsShownKey] = Shown;

                return json;

            }

            private static List<String> PinsIn(JObject?  HeldTo,
                                               String    Key)
            {

                var pins = new List<String>();

                if (HeldTo?[Key + "s"] is JArray several)
                    pins.AddRange(several.Values<String>().OfType<String>());

                else if (HeldTo?[Key] is JValue { Type: JTokenType.String } one)
                    pins.Add(one.Value<String>()!);

                return [.. pins.Distinct(StringComparer.OrdinalIgnoreCase)];

            }

        }

        #endregion

    }

}
