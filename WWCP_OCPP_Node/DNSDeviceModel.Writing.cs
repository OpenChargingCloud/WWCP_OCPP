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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using cloud.charging.open.protocols.OCPPv2_1;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Configuration;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// Change how a node resolves names, as WWCPNode.TryUpdateDNSConfiguration
    /// does: by a change made from the configuration as it is while nothing
    /// else can change it, and what the change does not mention stays as it is.
    /// </summary>
    /// <param name="Change">The change, in the shape of the "dns" section of the configuration file, from the section as the node says it then; null for none.</param>
    /// <param name="Error">Why nothing was changed.</param>
    /// <param name="NotSaved">True where the configuration file could not be read or written, and nothing was changed.</param>
    /// <param name="Origin">Who makes the change, as ChangeOrigin says it: "ocpp:CSMS".</param>
    public delegate Boolean TryUpdateDNSConfigurationDelegate(Func<JObject, JObject?>           Change,
                                                              [NotNullWhen(false)] out String?  Error,
                                                              out Boolean                       NotSaved,
                                                              String?                           Origin = null);

    /// <summary>
    /// Resolve names as a node would with a change of its name resolution in
    /// effect, without putting it into effect, as WWCPNode.ResolveWithAsync
    /// does.
    /// </summary>
    /// <param name="Change">The change, in the shape of the "dns" section; an empty one asks the configuration in effect.</param>
    /// <param name="Names">The names to resolve.</param>
    /// <param name="Within">How long all of them may take together.</param>
    /// <param name="CancellationToken">A cancellation token.</param>
    public delegate Task<DNSPreflight> DNSPreflightDelegate(JObject              Change,
                                                            IEnumerable<String>  Names,
                                                            TimeSpan             Within,
                                                            CancellationToken    CancellationToken);


    /// <summary>
    /// How a CSMS changes the name resolution of this node: the writes of a
    /// SetVariables request to DNSClientController and DNSServerController,
    /// put into the node as the web page puts a save.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Every name server is known by its id, the instance of its
    /// DNSServerController, which it keeps whatever its address becomes and
    /// whichever servers are added or taken away around it. DNSClientController
    /// .Servers says which servers there are, in their order: an id named anew
    /// adds a server, whose Address the same change must set, and an id left
    /// out takes its server away. The entries of a server the request adds
    /// reach this model through a stand-in for it - see NewInstance.
    /// </para>
    /// <para>
    /// The node replaces its list of name servers as a whole, so every change
    /// that touches a server sends the whole list, each server with its id.
    /// The servers are held in a table of their own for the whole request.
    /// What each server was held to is sent along as the request found it
    /// ("pinsAsShown"), so that a pin learned on first use in the meantime is
    /// kept.
    /// </para>
    /// <para>
    /// ServerSource says where the list is from. System gives it back to the
    /// servers the node was started with, the operating system's, and
    /// takes it out of the file; it cannot be one change with a server or with
    /// Servers, and a server changed after it in the same request is one of
    /// those servers. Manual writes the servers asked now into the file, as a
    /// change of a server does anyway.
    /// </para>
    /// <para>
    /// With IndependentRequests every entry is a change of its own, in the
    /// order of the request. A new server cannot be one: it needs its Address
    /// in the same change, so it needs ACID or BASE, as a new time server does
    /// - see NTSDeviceModel.Writing.cs. That way a new encrypted server never
    /// starts as a server over UDP either. With ACID and BASE all entries
    /// handed over at once are one change, all of it or nothing; with BASE,
    /// DNSClientController is one with the servers it adds.
    /// </para>
    /// <para>
    /// Before a change is put into effect, the names of the CSMS connections
    /// are resolved with it (SecureDNS §3.1.3): a name that resolves now and
    /// would not resolve then makes it CSMSUnreachable. A CSMS must not cut
    /// itself off, and nothing would show it before the next connection.
    /// SecureDNS-DeviceModel-Concept.md B.6 and B.9 say all of this at length.
    /// </para>
    /// <para>
    /// The names of the NTS-KE servers of the legal time are resolved with it
    /// as well (B.13 point 6). A change after which none of them would resolve
    /// while one does now is NTSKEUnreachable: the legal time would be lost.
    /// One that loses some of them and keeps another is accepted, and says
    /// which it loses in the status info of every entry, with the same reason
    /// code, and in the log.
    /// </para>
    /// <para>
    /// The node is handed each change together with its list as it is while
    /// nothing else can change it. A list that is no longer the one the table
    /// holds - a server added on the page while the request was being made -
    /// would be written away by the whole list sent back, so the change is
    /// refused as ValueChanged instead: the CSMS reads again and decides. Each
    /// change names the CSMS that made it, as the node writes it down: "ocpp:"
    /// and the start of the request's network path.
    /// </para>
    /// </remarks>
    public sealed partial class DNSDeviceModel : ISetVariablesSource,
                                                 INewInstancesSource
    {

        #region Data

        /// <summary>
        /// How long the names of the CSMS connections may take to resolve
        /// before a change is put into effect.
        /// </summary>
        public static readonly TimeSpan PreflightTime = TimeSpan.FromSeconds(10);

        /// <summary>
        /// The reason code of a change, or of a deleted root, after which an
        /// NTS-KE server of the legal time would no longer resolve: of a
        /// refusal where none of them would, of the status info of an accepted
        /// change where others still would.
        /// </summary>
        public const String NTSKEUnreachable = "NTSKEUnreachable";

        /// <summary>
        /// The most characters of the additionalInfo of a result (OCPP 2.1).
        /// </summary>
        private const Int32 MaxAdditionalInfo = 1024;

        /// <summary>
        /// What DNSClientController is set as one with in BASE, together with
        /// the servers its Servers adds.
        /// </summary>
        private const String ClientUnit = "client";

        private static readonly String[] writableClientVariables = [
            "Enabled", "QueryTimeout", "MaxRetries", "RecursionDesired", "UseCache", "DNSSECOK", "FollowCNAMEs", "MaxCNAMEFollows", "EncryptedOnly", "ServerSource", "Servers"
        ];

        private static readonly String[] writableServerVariables = [
            "Address", "Port", "Transport", "QueryTimeout", "CertificatePins", "RootPins", "OnPinMismatch", "TrustOnFirstUse", "RootCAs"
        ];

        /// <summary>
        /// The keys of the "dns" section the variables of DNSClientController
        /// are written under.
        /// </summary>
        private static readonly Dictionary<String, String> clientKeys = new (StringComparer.OrdinalIgnoreCase) {
            ["Enabled"]          = "enabled",
            ["QueryTimeout"]     = "queryTimeoutSeconds",
            ["MaxRetries"]       = "maxRetries",
            ["RecursionDesired"] = "recursionDesired",
            ["UseCache"]         = "useCache",
            ["DNSSECOK"]         = "dnssecOK",
            ["FollowCNAMEs"]     = "followCNAMEs",
            ["MaxCNAMEFollows"]  = "maxCNAMEFollows",
            ["EncryptedOnly"]    = "encryptedOnly"
        };

        private static readonly Regex serverInError = new (@"dns\.servers\[(\d*)\]", RegexOptions.CultureInvariant);

        private readonly TryUpdateDNSConfigurationDelegate?  update;
        private readonly DNSPreflightDelegate?               preflight;
        private readonly Func<IEnumerable<String>>?          namesToKeepResolving;
        private readonly Func<IEnumerable<String>>?          legalTimeNames;
        private readonly Action<String>?                     log;

        /// <summary>
        /// One request at a time: two would each hold a table of the servers
        /// that the other one's changes make wrong.
        /// </summary>
        private readonly SemaphoreSlim                       oneRequestAtATime = new (1, 1);

        #endregion

        #region Properties

        IEnumerable<String> IDNSControllerSource.WritableClientVariables
            => update is not null ? writableClientVariables : [];

        IEnumerable<String> IDNSControllerSource.WritableServerVariables
            => update is not null ? writableServerVariables : [];

        /// <summary>
        /// The component of which a request may name an instance this node
        /// does not have yet: a name server its DNSClientController.Servers
        /// names anew.
        /// </summary>
        public IEnumerable<String> NewInstanceComponents
            => update is not null ? [ ServerComponent ] : [];

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The Device Model view of the name resolution a node describes, which
        /// a CSMS can change as well.
        /// </summary>
        /// <param name="Section">Where the node says how it resolves names, as WWCPNode.DNSConfigurationJSON does: asked anew at every read and at every request.</param>
        /// <param name="Update">How a change is put into the node, as WWCPNode.TryUpdateDNSConfiguration does it.</param>
        /// <param name="Preflight">How names are resolved with a change before it is put into effect, as WWCPNode.ResolveWithAsync does it.</param>
        /// <param name="NamesToKeepResolving">The names a change must not stop resolving: those the CSMS connections are dialled by.</param>
        /// <param name="Log">Where a change is said before it is made.</param>
        /// <param name="LegalTimeNames">The names of the NTS-KE servers of the legal time, of which a change must not stop all resolving.</param>
        public DNSDeviceModel(Func<JObject>                      Section,
                              TryUpdateDNSConfigurationDelegate  Update,
                              DNSPreflightDelegate               Preflight,
                              Func<IEnumerable<String>>          NamesToKeepResolving,
                              Action<String>?                    Log              = null,
                              Func<IEnumerable<String>>?         LegalTimeNames   = null)

            : this(Section)

        {

            this.update                = Update;
            this.preflight             = Preflight;
            this.namesToKeepResolving  = NamesToKeepResolving;
            this.legalTimeNames        = LegalTimeNames;
            this.log                   = Log;

        }

        #endregion


        #region NewInstance(Component, Request)

        /// <summary>
        /// The controller a request makes of a name server this node does not
        /// have yet, standing in for it while the request is set: a server the
        /// request's DNSClientController.Servers names - and nothing else.
        /// </summary>
        /// <param name="Component">DNSServerController, with an instance this node does not have.</param>
        /// <param name="Request">The whole request.</param>
        public ComponentConfig? NewInstance(Component            Component,
                                            SetVariablesRequest  Request)
        {

            if (update is null ||
                Component.Instance is not String instance ||
                !String.Equals(Component.Name, ServerComponent, StringComparison.OrdinalIgnoreCase) ||
                !DNSConfiguration.IsServerId(instance) ||
                ServerIds.Contains(instance, StringComparer.OrdinalIgnoreCase))
            {
                return null;
            }

            return Request.VariableData.Any(entry => String.Equals(entry.Component.Name, ClientComponent, StringComparison.OrdinalIgnoreCase) &&
                                                     String.Equals(entry.Variable. Name, "Servers",       StringComparison.OrdinalIgnoreCase) &&
                                                     ListOf(entry.AttributeValue ?? "").Contains(instance))
                       ? DNSServerController.NewInstance(this, instance)
                       : null;

        }

        #endregion

        #region BaseUnitOf(Component)

        /// <summary>
        /// What is set as one with BASE: each name server the node has by
        /// itself, and DNSClientController with the servers its Servers adds.
        /// </summary>
        /// <param name="Component">A component of this source.</param>
        public Object BaseUnitOf(ComponentConfig Component)

            => Component is DNSServerController { IsNew: false } server
                   ? "server:" + server.Server.ToLowerInvariant()
                   : ClientUnit;

        #endregion

        #region SetVariablesAsync(Entries, DataConsistencyModel, CancellationToken)

        /// <summary>
        /// Put the given writes to the DNS controllers into the node.
        /// </summary>
        /// <param name="Entries">Entries of DNSClientController and DNSServerController, in request order.</param>
        /// <param name="DataConsistencyModel">IndependentRequests, ACID or BASE.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        public async Task<IReadOnlyList<SetVariableResult>> SetVariablesAsync(IReadOnlyList<SetVariableEntry>  Entries,
                                                                              DataConsistencyModel             DataConsistencyModel,
                                                                              CancellationToken                CancellationToken)
        {

            var results = new SetVariableResult?[Entries.Count];

            if (update is null)
            {
                for (var index = 0; index < Entries.Count; index++)
                    results[index] = Result(Entries[index], SetVariableStatus.Rejected, "ReadOnly", $"'{Where(Entries[index])}' is read-only!");
                return [.. results.Select(result => result!)];
            }

            await oneRequestAtATime.WaitAsync(CancellationToken).ConfigureAwait(false);

            try
            {

                var read   = section();
                var table  = TableServer.TableOf(read);
                var items  = new List<Item>();

                #region Every entry by itself

                for (var index = 0; index < Entries.Count; index++)
                {

                    var entry  = Entries[index];
                    var server = (entry.Component as DNSServerController)?.Server;
                    var name   = entry.Variable.Name;
                    var value  = entry.Data.AttributeValue ?? "";

                    var writes = (server is null ? writableClientVariables : writableServerVariables).Contains(name, StringComparer.OrdinalIgnoreCase);

                    var fits   = server is not null
                                     ? DNSServerController.TryCheck(server, name, value, out var refusal)
                                     : DNSClientController.TryCheck(        name, value, out refusal);

                    // A server the request adds has nothing to read yet.
                    var now    = server is not null
                                     ? ServerValueIn(read, server, name) ?? ""
                                     : ClientValueIn(read,         name);

                    if (!writes)
                        results[index] = Result(entry, SetVariableStatus.Rejected, "ReadOnly", $"'{Where(entry)}' is read-only!");

                    else if (!fits)
                        results[index] = Result(entry, SetVariableStatus.Rejected, refusal!.ReasonCode, refusal.AdditionalInfo);

                    // Compare and set: the CSMS changes nothing it has not seen.
                    else if (entry.Data.OldAttributeValue is String old && old != now)
                        results[index] = Result(entry, SetVariableStatus.Rejected, "ValueChanged", $"'{Where(entry)}' is \"{now}\" now, not \"{old}\"!");

                    else if (server is not null && String.Equals(name, "Address", StringComparison.OrdinalIgnoreCase) && value.Trim().Length == 0)
                        results[index] = Result(entry, SetVariableStatus.Rejected, "InvalidValue",
                                                $"'{Where(entry)}' cannot be empty: a name server is taken away by leaving its id out of {ClientComponent}.Servers!");

                    else if (server is null && String.Equals(name, "Servers", StringComparison.OrdinalIgnoreCase) &&
                             ServersRefusal(value, MaxServersIn(read)) is (String reasonCode, String why))
                        results[index] = Result(entry, SetVariableStatus.Rejected, reasonCode, why);

                    else
                        items.Add(new Item(index, entry, server, name, value));

                }

                // Which of two values is meant cannot be told.
                foreach (var twice in items.GroupBy(item => (Server: item.Server?.ToLowerInvariant(), Name: item.Name.ToLowerInvariant())).Where(group => group.Count() > 1).ToArray())
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
                    foreach (var change in IndependentChanges(items, results))
                        await ApplyAsync(change, table, results, false, CancellationToken).ConfigureAwait(false);

                return [.. results.Select(result => result!)];

            }
            finally
            {
                oneRequestAtATime.Release();
            }

        }

        #endregion


        #region (private static) ServersRefusal(Value, MaxServers)

        /// <summary>
        /// Why a value of DNSClientController.Servers cannot be the servers of
        /// this node, as the reason code and the sentence of a result - or
        /// null where it can.
        /// </summary>
        private static (String ReasonCode, String Why)? ServersRefusal(String  Value,
                                                                       UInt16  MaxServers)
        {

            var ids = ListOf(Value);

            if (ids.Count == 0)
                return ("InvalidValue", $"'{ClientComponent}.Servers' names no name server, and without one this charging station cannot reach its CSMS by name!");

            if (ids.FirstOrDefault(id => !DNSConfiguration.IsServerId(id)) is String wrong)
                return ("InvalidValue", $"'{ClientComponent}.Servers': '{wrong}' cannot be the id of a name server, which is lower-case letters, digits and hyphens, beginning with a letter, and at most {NTSServerConfiguration.MaxIdLength} of them!");

            if (ids.GroupBy(id => id).FirstOrDefault(group => group.Count() > 1) is { } twice)
                return ("InvalidValue", $"'{ClientComponent}.Servers' names '{twice.Key}' more than once!");

            if (ids.Count > MaxServers)
                return ("ValueOutOfRange", $"'{ClientComponent}.Servers' names {ids.Count} name servers, and this charging station takes at most {MaxServers}!");

            return null;

        }

        #endregion

        #region (private static) IndependentChanges(Items, Results)

        /// <summary>
        /// The changes of a request of independent entries: every entry by
        /// itself. An entry of a server the request adds is refused: a new server
        /// is made together with its Address, with ACID or BASE - and a Servers
        /// entry that adds one, alone, has no Address to make it with.
        /// </summary>
        private static IEnumerable<IReadOnlyList<Item>> IndependentChanges(IReadOnlyList<Item>   Items,
                                                                          SetVariableResult?[]  Results)
        {

            foreach (var item in Items)
            {

                if (item.Entry.Component is DNSServerController { IsNew: true })
                {
                    Results[item.Index] = Result(item.Entry, SetVariableStatus.Rejected, "InvalidValue",
                                                 $"'{Where(item.Entry)}' is not there yet: a new name server is made together with its Address, with the DataConsistencyModel ACID or BASE!");
                    continue;
                }

                yield return [ item ];

            }

        }

        #endregion

        #region (private) ApplyAsync(Change, Table, Results, Atomic, CancellationToken)

        /// <summary>
        /// Put one change into the node, after the pre-flight, and say for each
        /// of its entries what came of it. The table is what the node holds
        /// afterwards, where the change went in.
        /// </summary>
        private async Task ApplyAsync(IReadOnlyList<Item>   Change,
                                      List<TableServer>     Table,
                                      SetVariableResult?[]  Results,
                                      Boolean               Atomic,
                                      CancellationToken     CancellationToken)
        {

            var candidate   = Table.Select(server => server.Clone()).ToList();
            var section     = new JObject();
            var transported = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
            var servers     = Change.FirstOrDefault(item => item.Server is null && item.Is("Servers"));

            #region The change, in the shape of the "dns" section

            // Which servers there are first, and in which order: a server added
            // is there before anything is said about it, with the address its
            // entry gives it below, and one left out is gone with its pins.
            if (servers is not null)
            {

                var ids = ListOf(servers.Value);

                foreach (var id in ids.Where(id => !Has(candidate, id)))
                    if (!Change.Any(item => Is(item.Server, id) && item.IsAddress))
                    {
                        Refuse(Change, [ servers, .. Change.Where(item => Is(item.Server, id)) ], Results, Atomic, "InvalidValue",
                               $"'{ServerComponent}[{id}]' is new: set its Address in the same request, with the DataConsistencyModel ACID or BASE!");
                        return;
                    }

                var gone = Change.Where(item => item.Server is String id && Has(candidate, id) && !ids.Contains(id, StringComparer.OrdinalIgnoreCase)).ToArray();

                if (gone.Length > 0)
                {
                    Refuse(Change, [ servers, .. gone ], Results, Atomic, "InvalidValue",
                           $"'{ServerComponent}[{gone[0].Server}]' is taken away by {ClientComponent}.Servers in the same change, and cannot be set as well!");
                    return;
                }

                candidate = [.. ids.Select(id => candidate.FirstOrDefault(server => Is(server.Id, id)) ?? new TableServer(id, ""))];

            }

            // Addresses first, so that a server is where it is meant to be
            // before anything is said about how it is asked.
            foreach (var item in Change.OrderBy(item => item.IsAddress ? 0 : 1))
            {

                if (item.Server is not String id)
                {

                    // Said with the list of servers - see below and above.
                    if (!item.Is("ServerSource") && !item.Is("Servers"))
                        section[clientKeys[item.Name]] = ClientJSON(item.Name, item.Value);

                    continue;

                }

                if (candidate.FirstOrDefault(server => Is(server.Id, id)) is not TableServer server)
                {
                    Refuse(Change, [ item ], Results, Atomic, "InvalidValue",
                           $"'{ServerComponent}[{id}]' is not a name server of this charging station: add it through {ClientComponent}.Servers in the same request!");
                    return;
                }

                switch (item.Name.ToLowerInvariant())
                {

                    case "address":
                        server.Address         = item.Value.Trim();
                        break;

                    case "port":
                        server.Port            = item.Value.Length > 0 ? UInt16.Parse(item.Value, CultureInfo.InvariantCulture) : null;
                        break;

                    // The values of the paper in its own case: UDP, TLS, HTTPS_GET.
                    case "transport":
                        transported.TryAdd(server.Id, server.Transport);
                        server.Transport       = item.Value.ToUpperInvariant();
                        break;

                    case "querytimeout":
                        server.QueryTimeout    = item.Value.Length > 0 ? Decimal.Parse(item.Value, CultureInfo.InvariantCulture) : null;
                        break;

                    case "certificatepins":
                        server.CertificatePins = ListOf(item.Value);
                        break;

                    case "rootpins":
                        server.RootPins        = ListOf(item.Value);
                        break;

                    case "onpinmismatch":
                        server.OnMismatch      = item.Value.ToLowerInvariant();
                        break;

                    case "trustonfirstuse":
                        server.TrustOnFirstUse = item.Value.ToLowerInvariant();
                        break;

                    // The certificateGroup of the roots, as far as the store can
                    // mark a group of that name; an empty one is none.
                    case "rootcas":

                        var group = item.Value.Trim();

                        if (group.Length > 0 && !ServerPins.IsRootGroup(group))
                        {
                            Refuse(Change, [ item ], Results, Atomic, "InvalidValue",
                                   $"'{ServerComponent}[{id}].RootCAs': '{group}' cannot name a group of roots here: up to {ServerPins.MaxRootGroupLength} letters, digits, '-' or '_'.");
                            return;
                        }

                        server.RootGroup       = group.Length > 0 ? group : null;
                        break;

                }

            }

            // The port rule: a server switched to another transport without
            // being given a port keeps its port only where it was not the one
            // of its old transport - "UDP on 53" must not become TLS on 53.
            foreach (var (id, before) in transported)
                if (candidate.FirstOrDefault(server => Is(server.Id, id)) is TableServer server &&
                    !Change.Any(item => Is(item.Server, id) && item.Is("Port")) &&
                    server.Port == DefaultPortOf(before))
                {
                    server.Port = null;
                }

            // Where the servers are from: System gives them back to the ones
            // the node was started with - the operating system's - which the
            // node reads in "servers": null; Manual writes the ones it asks now
            // into the file, where a change of a server writes them anyway.
            var source    = Change.FirstOrDefault(item => item.Server is null && item.Is("ServerSource"));
            var toSystem  = String.Equals(source?.Value, "System", StringComparison.OrdinalIgnoreCase);
            var toManual  = String.Equals(source?.Value, "Manual", StringComparison.OrdinalIgnoreCase);
            var fromFile  = String.Equals(this.section()["serversFrom"]?.Value<String>(), "file", StringComparison.OrdinalIgnoreCase);
            var listed    = servers is not null || Change.Any(item => item.Server is not null);

            if (toSystem && listed)
            {
                Refuse(Change, [ source! ], Results, Atomic, "InvalidValue",
                       $"'{ClientComponent}.ServerSource' System gives the name servers back to those this charging station was started with, which cannot be one change with an entry of {ServerComponent} or with {ClientComponent}.Servers!");
                return;
            }

            if (toSystem)
                section["servers"] = JValue.CreateNull();

            else if (listed || toManual)
                section["servers"] = new JArray(candidate.Select(server => server.ToJSON()));

            #endregion

            if (!SomethingChanges(section, Table, toSystem && fromFile || toManual && !fromFile))
            {
                foreach (var item in Change)
                    Results[item.Index] = Result(item.Entry, SetVariableStatus.Accepted);
                return;
            }

            #region The pre-flight

            // The names of the CSMS connections, none of which a change may
            // stop resolving, and those of the NTS-KE servers of the legal
            // time, not all of which it may stop resolving (B.13 point 6). A
            // name of both is a CSMS's.
            var names    = namesToKeepResolving?.Invoke().ToArray() ?? [];
            var legal    = legalTimeNames?.Invoke().
                                           Where   (name => !names.Contains(name, StringComparer.OrdinalIgnoreCase)).
                                           Distinct(StringComparer.OrdinalIgnoreCase).
                                           ToArray () ?? [];
            var warning  = (String?) null;

            if (names.Length > 0 && section["enabled"]?.Type == JTokenType.Boolean && !section.Value<Boolean>("enabled"))
            {
                Refuse(Change, Change, Results, false, "CSMSUnreachable",
                       $"Name resolution cannot be switched off while the CSMS is dialled by its name: '{String.Join("', '", names)}'!");
                return;
            }

            if (names.Length > 0 || legal.Length > 0)
            {

                var now    = preflight!(new JObject(), [ .. names, .. legal ], PreflightTime, CancellationToken);
                var then   = preflight!(section,       [ .. names, .. legal ], PreflightTime, CancellationToken);

                await Task.WhenAll(now, then).ConfigureAwait(false);

                if (then.Result.Error is String error)
                {
                    Refuse(Change, CausedBy(Change, error, candidate), Results, Atomic, "InvalidValue", Rewritten(error, candidate));
                    return;
                }

                var asked = then.Result.Judgements.Count > 0
                                ? String.Join(", ", then.Result.Judgements.Select(judgement => $"{judgement.Server} {judgement.Outcome}"))
                                : "no name server answered";

                var lost  = names.Where(name => now.Result.Resolves(name) && !then.Result.Resolves(name)).ToArray();

                if (lost.Length > 0)
                {
                    Refuse(Change, Change, Results, false, "CSMSUnreachable",
                           $"'{String.Join("', '", lost)}' would not resolve: {asked}!");
                    return;
                }

                // The legal time is lost only where none of its servers would
                // resolve while one does now: a server that does not resolve
                // now blocks nothing, so that a broken configuration can be
                // repaired, and one that would resolve with the change keeps it.
                var legalNow   = legal.Where(name =>  now. Result.Resolves(name)).ToArray();
                var legalThen  = legal.Where(name =>  then.Result.Resolves(name)).ToArray();
                var legalLost  = legalNow.Where(name => !then.Result.Resolves(name)).ToArray();

                if (legalNow.Length > 0 && legalThen.Length == 0)
                {
                    Refuse(Change, Change, Results, false, NTSKEUnreachable,
                           $"'{String.Join("', '", legalNow)}' would not resolve, and the legal time would have no NTS-KE server left: {asked}!");
                    return;
                }

                if (legalLost.Length > 0)
                {
                    warning = $"'{String.Join("', '", legalLost)}' of the legal time would not resolve any more, '{String.Join("', '", legalThen)}' still would: {asked}.";
                    if (warning.Length > MaxAdditionalInfo)
                        warning = warning[..(MaxAdditionalInfo - 1)] + "…";
                }

            }

            #endregion

            var origin    = ChangeOrigin.OCPP(SenderOf(Change));
            var elsewhere = false;

            log?.Invoke($"A CSMS changes the name resolution of this charging station (by {origin}): {section.ToString(Formatting.None)}");

            // Made from the list as the node holds it when nothing else can
            // change it: a list changed elsewhere since the table was read
            // would lose that change to the whole list sent back.
            if (!update!(current => {

                             if (section["servers"] is not null && !SameServers(current, Table))
                             {
                                 elsewhere = true;
                                 return null;
                             }

                             return section;

                         },
                         out var refused,
                         out var notSaved,
                         origin))
            {

                if (notSaved)
                    Refuse(Change, Change, Results, false, "InternalError",
                           $"The name resolution could not be saved, and nothing of it was put into effect: {refused}");

                else
                    Refuse(Change, CausedBy(Change, refused, candidate), Results, Atomic, "InvalidValue", Rewritten(refused, candidate));

                return;

            }

            if (elsewhere)
            {
                Refuse(Change, Change, Results, false, "ValueChanged",
                       "The name servers were changed elsewhere while this request was made, and nothing of it was put into effect: read them again!");
                return;
            }

            // In effect: from now on the servers are held to what was sent - or,
            // given back, are the ones the node was started with.
            var held = toSystem
                           ? TableServer.TableOf(this.section())
                           : candidate;

            Table.Clear();
            Table.AddRange(held);

            foreach (var server in Table)
                server.ShownAsSent();

            // Taken, and said where it costs the legal time a server: to the
            // CSMS with every entry, and in the log.
            if (warning is not null)
                log?.Invoke($"A CSMS changed the name resolution of this node (by {origin}), and the legal time loses NTS-KE servers: {warning}");

            foreach (var item in Change)
                Results[item.Index] = warning is null
                                          ? Result(item.Entry, SetVariableStatus.Accepted)
                                          : Result(item.Entry, SetVariableStatus.Accepted, NTSKEUnreachable, warning);

        }

        #endregion

        #region (private static) SomethingChanges(Section, Table, SourceChanges)

        /// <summary>
        /// Whether the given change says anything the node does not have yet:
        /// a setting of the client, another list of servers, or another place
        /// they are from.
        /// </summary>
        private static Boolean SomethingChanges(JObject            Section,
                                                List<TableServer>  Table,
                                                Boolean            SourceChanges)

            => SourceChanges ||
               Section.Properties().Any(property => property.Name != "servers") ||
               (Section["servers"] is JArray servers &&
                !JToken.DeepEquals(servers, new JArray(Table.Select(server => server.ToJSON()))));

        #endregion

        #region (private static) SameServers(Section, Table)

        /// <summary>
        /// Whether the node holds the servers the table holds, in that order,
        /// with those ids and those timeouts - as the node tells servers apart,
        /// so that "2001:DB8::53" and its short form, or a port left out and the
        /// port of its transport, are the same server.
        /// </summary>
        private static Boolean SameServers(JObject            Section,
                                           List<TableServer>  Table)
        {

            var held   = ServersIn(Section["servers"] as JArray ?? []);
            var meant  = ServersIn(new JArray(Table.Select(server => server.ToJSON())));

            return held is not null && meant is not null && held.SequenceEqual(meant);

        }

        /// <summary>
        /// The servers of a list, or null where one of them cannot be read.
        /// </summary>
        private static (String? Id, DNSServerConfig Server, TimeSpan? QueryTimeout)[]? ServersIn(JArray Servers)
        {

            var servers = new List<(String? Id, DNSServerConfig Server, TimeSpan? QueryTimeout)>();

            foreach (var json in Servers)
            {

                if (!DNSConfiguration.TryParseServer(json, out var server, out _))
                    return null;

                servers.Add(((json as JObject)?.Value<String>("id"), server, server.QueryTimeout));

            }

            return [.. servers];

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
            var info    = AdditionalInfo.Length > MaxAdditionalInfo
                              ? AdditionalInfo[..(MaxAdditionalInfo - 1)] + "…"
                              : AdditionalInfo;

            foreach (var item in Change)
                Results[item.Index] = !Atomic || causing.Contains(item)
                                          ? Result(item.Entry, SetVariableStatus.Rejected,   ReasonCode,    info)
                                          : Result(item.Entry, SetVariableStatus.Rolledback, "Unspecified", $"Not set, because the same change could not be made: {info}");

        }

        #endregion

        #region (private static) CausedBy(Change, Error, Candidate)

        /// <summary>
        /// The entries of the server a refusal of the node names, by its place
        /// in the list that was sent - or none, where it names none.
        /// </summary>
        private static IReadOnlyList<Item> CausedBy(IReadOnlyList<Item>  Change,
                                                    String               Error,
                                                    List<TableServer>    Candidate)
        {

            var match = serverInError.Match(Error);

            if (!match.Success || !Int32.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var index) ||
                index >= Candidate.Count)
            {
                return [];
            }

            return [.. Change.Where(item => Is(item.Server, Candidate[index].Id))];

        }

        #endregion

        #region (private static) Rewritten(Error, Candidate)

        /// <summary>
        /// A refusal of the node in the words of the Device Model: the server it
        /// names by its place in the list that was sent becomes the server of
        /// that id.
        /// </summary>
        private static String Rewritten(String             Error,
                                        List<TableServer>  Candidate)

            => serverInError.Replace(Error, match =>
                   Int32.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var index) &&
                   index < Candidate.Count
                       ? $"{ServerComponent}[{Candidate[index].Id}]"
                       : ServerComponent);

        #endregion

        #region (private static) Has(Servers, Id), Is(Id, Other)

        /// <summary>
        /// Whether a server of the given id is among the given ones.
        /// </summary>
        private static Boolean Has(IEnumerable<TableServer>  Servers,
                                   String                    Id)

            => Servers.Any(server => Is(server.Id, Id));

        /// <summary>
        /// Whether two ids name the same server, as an OCPP instance is named:
        /// in any case.
        /// </summary>
        private static Boolean Is(String?  Id,
                                  String   Other)

            => String.Equals(Id, Other, StringComparison.OrdinalIgnoreCase);

        #endregion

        #region (private static) ClientJSON(Variable, Value)

        /// <summary>
        /// A value of DNSClientController as its key in the "dns" section says
        /// it: "Default" recursion as the null that leaves it to the server.
        /// </summary>
        private static JToken ClientJSON(String  Variable,
                                         String  Value)

            => Variable.ToLowerInvariant() switch {

                   "querytimeout"      => new JValue(Decimal.Parse(Value, CultureInfo.InvariantCulture)),
                   "maxretries"        => new JValue(Int64.  Parse(Value, CultureInfo.InvariantCulture)),
                   "maxcnamefollows"   => new JValue(Int64.  Parse(Value, CultureInfo.InvariantCulture)),

                   "recursiondesired"  => Value.ToLowerInvariant() switch {
                                              "set"    => new JValue(true),
                                              "unset"  => new JValue(false),
                                              _        => JValue.CreateNull()
                                          },

                   _                   => new JValue(String.Equals(Value, "true", StringComparison.OrdinalIgnoreCase))

               };

        #endregion

        #region (private static) ListOf(Value), DefaultPortOf(Transport)

        /// <summary>
        /// The entries of a comma separated list.
        /// </summary>
        private static List<String> ListOf(String Value)

            => [.. Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];

        /// <summary>
        /// The port a name server is asked on when nothing else is said, by its
        /// transport, as the node's DNS client has it.
        /// </summary>
        private static UInt16 DefaultPortOf(String Transport)

            => Transport.ToUpperInvariant() switch {
                   "TLS"                             => 853,
                   var http when http.StartsWith("HTTPS", StringComparison.Ordinal) => 443,
                   var http when http.StartsWith("HTTP",  StringComparison.Ordinal) => 80,
                   _                                 => 53
               };

        #endregion

        #region (private static) Where(Entry), Result(Entry, Status, ReasonCode = null, AdditionalInfo = null)

        private static String Where(SetVariableEntry Entry)

            => Entry.Component is DNSServerController server
                   ? $"{ServerComponent}[{server.Server}].{Entry.Variable.Name}"
                   : $"{ClientComponent}.{Entry.Variable.Name}";

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


        #region (private) Item

        /// <summary>
        /// An entry that may be tried: which server, or the client, and what.
        /// </summary>
        private sealed record Item(Int32             Index,
                                   SetVariableEntry  Entry,
                                   String?           Server,
                                   String            Name,
                                   String            Value)
        {

            public Boolean IsAddress
                => Server is not null && Is("Address");

            public Boolean Is(String Variable)
                => String.Equals(Name, Variable, StringComparison.OrdinalIgnoreCase);

        }

        #endregion

        #region (private) TableServer

        /// <summary>
        /// One server of the table a request holds, as the node's file says it -
        /// and what it was held to when the request found it, or since this
        /// request set it.
        /// </summary>
        private sealed class TableServer(String  Id,
                                         String  Address)
        {

            public String        Id               { get; } = Id;
            public String        Address          { get; set; } = Address;
            public UInt16?       Port             { get; set; }
            public String        Transport        { get; set; } = "UDP";
            public Decimal?      QueryTimeout     { get; set; }
            public List<String>  CertificatePins  { get; set; } = [];
            public List<String>  RootPins         { get; set; } = [];
            public String        OnMismatch       { get; set; } = "refuse";
            public String        TrustOnFirstUse  { get; set; } = "none";

            /// <summary>
            /// The group of roots the server is held to, which no variable
            /// says: it stays with the server, as the fingerprints do, also where
            /// the request gives it another address.
            /// </summary>
            public String?       RootGroup        { get; set; }

            /// <summary>
            /// What the server was held to as last seen: null for a server new
            /// in this request, which the node has nothing to compare with.
            /// </summary>
            public JObject?      AsShown          { get; private set; }


            /// <summary>
            /// The servers of a description of name resolution, in its order,
            /// each by the id the node names it with.
            /// </summary>
            public static List<TableServer> TableOf(JObject Section)
            {

                var table = new List<TableServer>();

                foreach (var json in (Section["servers"] as JArray ?? []).OfType<JObject>())
                {

                    var server = new TableServer(json.Value<String>("id") ?? "", json.Value<String>("address") ?? "") {
                                     Port             = json["port"]?.Type == JTokenType.Integer ? json.Value<UInt16>("port") : null,
                                     Transport        = json.Value<String>("transport") ?? "UDP",
                                     QueryTimeout     = json["queryTimeoutSeconds"]?.Type is JTokenType.Integer or JTokenType.Float ? json["queryTimeoutSeconds"]!.AsDecimal() : null,
                                     CertificatePins  = ListOf(PinsOf(json, "certificateFingerprint")),
                                     RootPins         = ListOf(PinsOf(json, "rootFingerprint")),
                                     OnMismatch       = json.Value<String>("onMismatch")      ?? "refuse",
                                     TrustOnFirstUse  = json.Value<String>("trustOnFirstUse") ?? "none",
                                     RootGroup        = json.Value<String>("rootGroup")
                                 };

                    server.ShownAsSent();

                    table.Add(server);

                }

                return table;

            }

            public TableServer Clone()

                => new (Id, Address) {
                       Port             = Port,
                       Transport        = Transport,
                       QueryTimeout     = QueryTimeout,
                       CertificatePins  = [.. CertificatePins],
                       RootPins         = [.. RootPins],
                       OnMismatch       = OnMismatch,
                       TrustOnFirstUse  = TrustOnFirstUse,
                       RootGroup        = RootGroup,
                       AsShown          = AsShown
                   };

            /// <summary>
            /// From now on, what the server is held to is what it was shown with.
            /// </summary>
            public void ShownAsSent()
            {
                AsShown = PinsJSON();
            }

            /// <summary>
            /// What it is held to, in the keys of the node's file. A mismatch
            /// only where it can come about: with a pin, or one to be learned,
            /// and not with a group of roots alone.
            /// </summary>
            private JObject PinsJSON()
            {

                var json = new JObject();

                if (CertificatePins.Count > 0)
                    json["certificateFingerprints"] = new JArray(CertificatePins);

                if (RootPins.Count > 0)
                    json["rootFingerprints"]        = new JArray(RootPins);

                if (TrustOnFirstUse != "none")
                    json["trustOnFirstUse"]         = TrustOnFirstUse;

                if (OnMismatch != "refuse" && json.Count > 0)
                    json["onMismatch"]              = OnMismatch;

                if (RootGroup is not null)
                    json["rootGroup"]               = RootGroup;

                return json;

            }

            /// <summary>
            /// The server as the node's file keeps it, with its id: written into
            /// the file, it stays the server's whatever is added or taken away
            /// around it.
            /// </summary>
            public JObject ToJSON()
            {

                var json = new JObject(
                               new JProperty("id",         Id),
                               new JProperty("address",    Address),
                               new JProperty("transport",  Transport)
                           );

                if (Port.HasValue)
                    json["port"]                 = Port.Value;

                if (QueryTimeout.HasValue)
                    json["queryTimeoutSeconds"]  = QueryTimeout.Value;

                json.Merge(PinsJSON());

                if (AsShown is not null)
                    json[ServerPins.AsShownKey]  = AsShown.DeepClone();

                return json;

            }

        }

        #endregion

    }

}
