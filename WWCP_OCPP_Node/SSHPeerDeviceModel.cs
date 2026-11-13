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
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.OCPPv2_1;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Configuration;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The SSH peers of a node as the OCPP Device Model has them:
    /// SSHPeerController, read from the "ssh.peers" of the node's configuration
    /// at every read, and written there by a SetVariables request.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A peer comes and goes through Peers of the instance without a name. One
    /// that is added needs its Address and a host key or a CA to hold it to in
    /// the same request, with ACID or BASE: a peer nobody could believe is
    /// never stored, so there is no moment in between.
    /// </para>
    /// <para>
    /// What is written is the whole list, as the node takes it. A change made
    /// elsewhere between the read and the write of one request is not seen;
    /// OldAttributeValue guards a single variable against it.
    /// </para>
    /// </remarks>
    public sealed partial class SSHPeerDeviceModel : ISSHPeerControllerSource,
                                             ISetVariablesSource,
                                             INewInstancesSource
    {

        #region Data

        /// <summary>
        /// The name of the component.
        /// </summary>
        public const String Component = nameof(SSHPeerController);

        private static readonly String[] nodeVariables          = [ "Peers", "ClientPublicKey" ];
        private static readonly String[] writableNodeVariables  = [ "Peers" ];
        private static readonly String[] peerVariables          = [ "Address", "Port", "UserName", "HostKeyPins", "HostCAs", "OnPinMismatch", "HostKeyFingerprint", "Connected", "Problem" ];
        private static readonly String[] writablePeerVariables  = [ "Address", "Port", "UserName", "HostKeyPins", "HostCAs", "OnPinMismatch" ];

        private static readonly Regex    peerId                 = new ("^[A-Za-z0-9][A-Za-z0-9._-]{0,31}$", RegexOptions.CultureInvariant);

        private readonly WWCPNode        node;
        private readonly Action<String>? log;
        private readonly SemaphoreSlim   oneRequestAtATime      = new (1, 1);

        #endregion

        #region Properties

        /// <inheritdoc/>
        public IEnumerable<String>  PeerIds
            => node.ConfiguredSSHPeers.Select(peer => peer.Id);

        /// <inheritdoc/>
        public IEnumerable<String>  NodeVariables            => nodeVariables;

        /// <inheritdoc/>
        public IEnumerable<String>  WritableNodeVariables    => writableNodeVariables;

        /// <inheritdoc/>
        public IEnumerable<String>  PeerVariables            => peerVariables;

        /// <inheritdoc/>
        public IEnumerable<String>  WritablePeerVariables    => writablePeerVariables;

        /// <summary>
        /// The component whose instances a SetVariables request may add.
        /// </summary>
        public IEnumerable<String>  NewInstanceComponents    => [ Component ];

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The SSH peers of the given node.
        /// </summary>
        /// <param name="Node">The node.</param>
        /// <param name="Log">Where a change is said, as a sentence.</param>
        public SSHPeerDeviceModel(WWCPNode         Node,
                                  Action<String>?  Log   = null)
        {
            this.node  = Node;
            this.log   = Log;
        }

        #endregion


        #region NodeValue(Variable)

        /// <inheritdoc/>
        public String? NodeValue(String Variable)

            => Variable.ToLowerInvariant() switch {
                   "peers"            => String.Join(",", PeerIds),
                   "clientpublickey"  => node.SSHPeerClientPublicKey ?? "",
                   _                  => null
               };

        #endregion

        #region PeerValue(Peer, Variable)

        /// <inheritdoc/>
        public String? PeerValue(String Peer, String Variable)
        {

            if (node.ConfiguredSSHPeers.FirstOrDefault(peer => String.Equals(peer.Id, Peer, StringComparison.OrdinalIgnoreCase)) is not SSHPeerConfiguration entry)
                return null;

            var judgement = node.SSHPeerHostKeyJudgementOf(entry.Id);

            return Variable.ToLowerInvariant() switch {
                       "address"             => entry.Address.Trimmed,
                       "port"                => entry.Port.ToString(CultureInfo.InvariantCulture),
                       "username"            => entry.User ?? "",
                       "hostkeypins"         => String.Join(",", entry.Trust.Pins),
                       "hostcas"             => String.Join(",", entry.Trust.CAs),
                       "onpinmismatch"       => entry.Trust.OnMismatch.ToString(),
                       "hostkeyfingerprint"  => judgement?.Shown ?? "",
                       "connected"           => node.SSHPeerTo(entry.Address)?.Connection.Current is not null ? "true" : "false",
                       "problem"             => judgement is { Accepted: false } ? "true" : "false",
                       _                     => null
                   };

        }

        #endregion


        #region NewInstance(Component, Request)

        /// <summary>
        /// The stand-in for a peer the node does not have, while the request
        /// that adds it is set: one the request's Peers names - and nothing
        /// else.
        /// </summary>
        public ComponentConfig? NewInstance(Component            Component,
                                            SetVariablesRequest  Request)
        {

            if (Component.Instance is not String instance ||
                !String.Equals(Component.Name, SSHPeerDeviceModel.Component, StringComparison.OrdinalIgnoreCase) ||
                !peerId.IsMatch(instance) ||
                PeerIds.Contains(instance, StringComparer.OrdinalIgnoreCase))
            {
                return null;
            }

            return Request.VariableData.Any(entry => String.Equals(entry.Component.Name, SSHPeerDeviceModel.Component, StringComparison.OrdinalIgnoreCase) &&
                                                     entry.Component.Instance is null &&
                                                     String.Equals(entry.Variable.Name, "Peers", StringComparison.OrdinalIgnoreCase) &&
                                                     ListOf(entry.AttributeValue ?? "").Contains(instance, StringComparer.OrdinalIgnoreCase))
                       ? SSHPeerController.NewInstance(this, instance)
                       : null;

        }

        #endregion

        #region BaseUnitOf(Component)

        /// <summary>
        /// What is set as one with BASE: each peer the node has by itself, and
        /// the instance without a name with the peers its Peers adds.
        /// </summary>
        public Object BaseUnitOf(ComponentConfig Component)

            => Component is SSHPeerController { IsNew: false, Peer: String peer }
                   ? "peer:" + peer.ToLowerInvariant()
                   : "peers";

        #endregion

        #region SetVariablesAsync(Entries, DataConsistencyModel, CancellationToken)

        /// <summary>
        /// Set variables of the SSH peers: all of the given entries at once
        /// with ACID and BASE, each by itself otherwise.
        /// </summary>
        public async Task<IReadOnlyList<SetVariableResult>> SetVariablesAsync(IReadOnlyList<SetVariableEntry>  Entries,
                                                                              DataConsistencyModel             DataConsistencyModel,
                                                                              CancellationToken                CancellationToken)
        {

            var results = new SetVariableResult?[Entries.Count];
            var atomic  = DataConsistencyModel == DataConsistencyModel.ACID ||
                          DataConsistencyModel == DataConsistencyModel.BASE;

            await oneRequestAtATime.WaitAsync(CancellationToken);

            try
            {

                if (atomic)
                {

                    var table = TableOf(node.ConfiguredSSHPeers);

                    for (var i = 0; i < Entries.Count; i++)
                    {
                        if (!TryApply(table, Entries[i], Entries, out var code, out var info))
                        {
                            for (var j = 0; j < Entries.Count; j++)
                                results[j] = j == i
                                                 ? Result(Entries[j], SetVariableStatus.Rejected,   code,          info)
                                                 : Result(Entries[j], SetVariableStatus.Rolledback, "Unspecified", $"{Where(Entries[i])} was refused.");
                            return results!;
                        }
                    }

                    var (saved, reason, why) = await Save(table, SenderOf(Entries));

                    for (var j = 0; j < Entries.Count; j++)
                        results[j] = saved
                                         ? Result(Entries[j], SetVariableStatus.Accepted)
                                         : Result(Entries[j], SetVariableStatus.Rejected, reason, why);

                    return results!;

                }

                for (var i = 0; i < Entries.Count; i++)
                {

                    // A peer that is added is its name in Peers and what it is
                    // held to, which no single entry says.
                    if (Entries[i].Component is SSHPeerController { IsNew: true } ||
                        AddsAPeer(Entries[i]))
                    {
                        results[i] = Result(Entries[i], SetVariableStatus.Rejected, "InvalidValue",
                                            $"{Where(Entries[i])}: an SSH peer is added by Peers together with its Address and a host key or a CA to hold it to, in one request with the data consistency model ACID or BASE.");
                        continue;
                    }

                    var table = TableOf(node.ConfiguredSSHPeers);

                    if (!TryApply(table, Entries[i], [ Entries[i] ], out var code, out var info))
                    {
                        results[i] = Result(Entries[i], SetVariableStatus.Rejected, code, info);
                        continue;
                    }

                    var (saved, reason, why) = await Save(table, SenderOf(Entries));

                    results[i] = saved
                                     ? Result(Entries[i], SetVariableStatus.Accepted)
                                     : Result(Entries[i], SetVariableStatus.Rejected, reason, why);

                }

                return results!;

            }
            finally
            {
                oneRequestAtATime.Release();
            }

        }

        #endregion


        #region (private) Row, TableOf(Peers)

        /// <summary>
        /// A peer as a request changes it.
        /// </summary>
        private sealed class Row(String Id)
        {

            public String        Id          { get; }      = Id;
            public String        Address     { get; set; } = "";
            public UInt16?       Port        { get; set; }
            public String?       User        { get; set; }
            public List<String>  Pins        { get; set; } = [];
            public List<String>  CAs         { get; set; } = [];
            public String        OnMismatch  { get; set; } = "refuse";

            public JObject ToJSON()
            {

                var json = new JObject(
                               new JProperty("id",       Id),
                               new JProperty("address",  Address)
                           );

                if (Port is not null)        json.Add("port",         Port.Value);
                if (User is not null)        json.Add("user",         User);
                if (Pins.Count > 0)          json.Add("hostKeyPins",  new JArray(Pins));
                if (CAs. Count > 0)          json.Add("hostCAs",      new JArray(CAs));
                if (OnMismatch != "refuse")  json.Add("onMismatch",   OnMismatch);

                return json;

            }

        }

        private static List<Row> TableOf(IEnumerable<SSHPeerConfiguration> Peers)

            => [.. Peers.Select(peer => new Row(peer.Id) {
                                             Address     = peer.Address.Trimmed,
                                             Port        = peer.Trust.Port,
                                             User        = peer.User,
                                             Pins        = [.. peer.Trust.Pins],
                                             CAs         = [.. peer.Trust.CAs],
                                             OnMismatch  = peer.Trust.OnMismatch.AsText()
                                         })];

        #endregion

        #region (private) TryApply(Table, Entry, Together, out ReasonCode, out AdditionalInfo)

        /// <summary>
        /// Make the change of one entry in the given table, or say why not.
        /// </summary>
        /// <param name="Table">The peers as they are, with the entries before applied.</param>
        /// <param name="Entry">The entry.</param>
        /// <param name="Together">The entries that are set as one with it.</param>
        private Boolean TryApply(List<Row>                        Table,
                                 SetVariableEntry                 Entry,
                                 IReadOnlyList<SetVariableEntry>  Together,
                                 out String                       ReasonCode,
                                 out String                       AdditionalInfo)
        {

            ReasonCode      = "InvalidValue";
            AdditionalInfo  = "";

            var peer      = (Entry.Component as SSHPeerController)?.Peer;
            var variable  = Entry.Variable.Name;
            var value     = Entry.Data.AttributeValue ?? "";

            if (!(peer is null ? writableNodeVariables : writablePeerVariables).Contains(variable, StringComparer.OrdinalIgnoreCase))
            {
                ReasonCode      = "ReadOnly";
                AdditionalInfo  = $"{Where(Entry)} is read-only.";
                return false;
            }

            if (!SSHPeerController.TryCheck(peer, variable, value, out var refusal))
            {
                ReasonCode      = refusal.ReasonCode;
                AdditionalInfo  = refusal.AdditionalInfo ?? "";
                return false;
            }

            // Set only where it still is what the sender believes it to be.
            if (Entry.Data.OldAttributeValue is String believed &&
                !(Entry.Component is SSHPeerController { IsNew: true }) &&
                believed != ((peer is null ? NodeValue(variable) : PeerValue(peer, variable)) ?? ""))
            {
                ReasonCode      = "ValueChanged";
                AdditionalInfo  = $"{Where(Entry)} is no longer '{believed}'.";
                return false;
            }

            #region Peers: who comes and who goes

            if (peer is null)
            {

                var ids = ListOf(value);

                if (ids.FirstOrDefault(id => !peerId.IsMatch(id)) is String bad)
                {
                    AdditionalInfo = $"{Where(Entry)}: '{bad}' is no name of a peer: letters, digits, '.', '_' and '-', and at most 32 of them.";
                    return false;
                }

                if (ids.GroupBy(id => id, StringComparer.OrdinalIgnoreCase).FirstOrDefault(group => group.Count() > 1) is { } twice)
                {
                    AdditionalInfo = $"{Where(Entry)} names '{twice.Key}' twice.";
                    return false;
                }

                var rows = ids.Select(id => Table.FirstOrDefault(row => String.Equals(row.Id, id, StringComparison.OrdinalIgnoreCase)) ?? new Row(id)).ToList();

                Table.Clear();
                Table.AddRange(rows);

                return true;

            }

            #endregion

            // A peer Peers of the same change adds is in the table only once
            // that entry was applied: whichever comes first makes it.
            var row = Table.FirstOrDefault(row => String.Equals(row.Id, peer, StringComparison.OrdinalIgnoreCase));

            if (row is null)
            {

                var added = Together.Any(other => other.Component is SSHPeerController { Peer: null } &&
                                                  String.Equals(other.Variable.Name, "Peers", StringComparison.OrdinalIgnoreCase) &&
                                                  ListOf(other.Data.AttributeValue ?? "").Contains(peer, StringComparer.OrdinalIgnoreCase));

                if (!added)
                {
                    ReasonCode      = "NotFound";
                    AdditionalInfo  = $"This node has no SSH peer '{peer}': add it with {Component}.Peers in the same request.";
                    return false;
                }

                row = new Row(peer);
                Table.Add(row);

            }

            switch (variable.ToLowerInvariant())
            {

                case "address":
                    row.Address     = value.Trim();
                    break;

                case "port":
                    row.Port        = UInt16.Parse(value, CultureInfo.InvariantCulture);
                    break;

                case "username":
                    row.User        = value.Trim().Length > 0 ? value.Trim() : null;
                    break;

                case "hostkeypins":
                    row.Pins        = ListOf(value);
                    break;

                case "hostcas":
                    row.CAs         = ListOf(value);
                    break;

                case "onpinmismatch":
                    row.OnMismatch  = value.Trim().ToLowerInvariant();
                    break;

            }

            return true;

        }

        #endregion

        #region (private) Save(Table, Sender)

        /// <summary>
        /// Have the node take the peers as the table has them: read as its
        /// configuration file is read, saved there, and in effect at once.
        /// </summary>
        private async Task<(Boolean Saved, String ReasonCode, String AdditionalInfo)> Save(List<Row>  Table,
                                                                                         String     Sender)
        {

            // A name Peers took out and put in again would be the same peer:
            // the order of the list is the order of the file.
            var change = new JObject(new JProperty("peers", new JArray(Table.Select(row => row.ToJSON()))));

            String InWords(String Error)
            {

                for (var i = 0; i < Table.Count; i++)
                    Error = Error.Replace($"ssh.peers[{i}]", $"{Component}[{Table[i].Id}]", StringComparison.Ordinal);

                return Error.Length > 1024 ? Error[..1024] : Error;

            }

            if (!SSHConfiguration.TryParse(change, out _, out var wrong))
                return (false, "InvalidValue", InWords(wrong));

            var changed = await node.TryUpdateSSHConfiguration(change);

            if (changed.Error is String error)
                return (false, changed.NotSaved ? "InternalError" : "InvalidValue", InWords(error));

            log?.Invoke($"The SSH peers of this node were set over OCPP by {Sender}: {(Table.Count > 0 ? String.Join(", ", Table.Select(row => $"'{row.Id}' ({row.Address})")) : "none")}.");

            return (true, "", "");

        }

        #endregion

        #region (private static) Helpers

        private static List<String> ListOf(String Value)

            => [.. Value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)];

        private Boolean AddsAPeer(SetVariableEntry Entry)

            => Entry.Component is SSHPeerController { Peer: null } &&
               String.Equals(Entry.Variable.Name, "Peers", StringComparison.OrdinalIgnoreCase) &&
               ListOf(Entry.Data.AttributeValue ?? "").Any(id => !PeerIds.Contains(id, StringComparer.OrdinalIgnoreCase));

        private static String SenderOf(IReadOnlyList<SetVariableEntry> Entries)

            => Entries.FirstOrDefault()?.Request?.NetworkPath is { Length: > 0 } path
                   ? path.Source.ToString()
                   : "CSMS";

        private static String Where(SetVariableEntry Entry)

            => Entry.Component is SSHPeerController { Peer: String peer }
                   ? $"{Component}[{peer}].{Entry.Variable.Name}"
                   : $"{Component}.{Entry.Variable.Name}";

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

    }

}
