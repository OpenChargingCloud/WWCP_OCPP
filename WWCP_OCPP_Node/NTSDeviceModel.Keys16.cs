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

using System.Text.RegularExpressions;

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Configuration;

using OCPP16 = cloud.charging.open.protocols.OCPPv1_6;
using OCPP21 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The configuration keys of OCPP 1.6 the Secure Time Synchronization white
    /// paper gives the time servers (section 3.3): read from the node when a
    /// central system asks, and a change of one made as the CSMS of OCPP 2.1
    /// makes it - so that what one version may change, and what it comes to,
    /// is the other's as well.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A key is turned into the entries of a SetVariables request of the NTP
    /// controllers and handed to the same Device Model, with ACID where it is
    /// several: the same checks, the same refusals, the same log line naming
    /// the central system. Where they are all accepted the key is, and
    /// rejected otherwise - OCPP 1.6 has no reason to give.
    /// </para>
    /// <para>
    /// A list (CSL) is per server, in the order of "ntp.&lt;group&gt;.servers",
    /// which is the order of the group's servers in the file, switched-off ones
    /// included. A shorter list's last value holds for the servers after it, an
    /// empty entry takes the one before it - the default for the first - and a
    /// longer list is rejected. "ntp.&lt;group&gt;.servers" itself renames the
    /// servers at their places, adds new ones at the end, which take the
    /// ports, priority, root group and pin rules of the last one but none of
    /// its pins, and takes away the ones beyond its end.
    /// </para>
    /// <para>
    /// OCPP 1.6 changes one key at a time, so minInterval and maxInterval each
    /// set the one interval the node checks at, where 2.1 has to set both at
    /// once. Keys of the paper the node does not have - preflight, separate
    /// NTS-KE servers, signed responses and the common "ntp.reboot.*" - are
    /// not listed, and a change of one is NotSupported.
    /// Groups cannot be made or taken away through 1.6.
    /// </para>
    /// <para>
    /// "ntske.&lt;group&gt;.aead" is a list like the others but for an empty
    /// entry, which is a server whose key exchange offers every algorithm, the
    /// server choosing - not the one before it. Read, a server reads as
    /// AEADAlgorithm does: what its entry names, or else what its last key
    /// exchange agreed on.
    /// </para>
    /// <para>
    /// The common "ntp.timeAdjustmentReportingThreshold" is ClockCtrlr's
    /// TimeAdjustmentReportingThreshold, and so the legal group's: listed
    /// where there is one, and "" for never, as in 2.1.
    /// </para>
    /// </remarks>
    public sealed partial class NTSDeviceModel : OCPP16.CP.IConfigurationKeySource
    {

        #region Data

        /// <summary>
        /// The keys of every group, by the name after "ntp.&lt;group&gt;." or
        /// "ntske.&lt;group&gt;.": the variable of NTPClientGroupController it
        /// is, or of NTPClientController for a list - and its default.
        /// </summary>
        private static readonly (String Key, String Variable, Boolean PerServer, String Default)[] groupKeys16 = [
            ("ntp.enabled",              "Enabled",          false,  ""),
            ("ntp.servers",              "ServerURL",        true,   ""),
            ("ntp.ports",                "ServerPort",       true,   "123"),
            ("ntp.mode",                 "Mode",             false,  "NTSv4"),
            ("ntp.priority",             "Priority",         true,   "0"),
            ("ntp.minInterval",          "MinInterval",      false,  ""),
            ("ntp.maxInterval",          "MaxInterval",      false,  ""),
            ("ntp.timeout",              "Timeout",          false,  ""),
            ("ntp.minServers",           "MinServers",       false,  ""),
            ("ntp.maxDeviation",         "MaxDeviation",     false,  ""),
            ("ntp.errorLogging",         "ErrorLogging",     false,  ""),
            ("ntp.maxErrors",            "MaxErrors",        false,  ""),
            ("ntp.rootCAs",              "RootCAs",          true,   ""),
            ("ntp.delayAsym",            "DelayAsymmetry",   true,   "0"),
            ("ntp.delayVarAsym",         "DelayVarAsymmetry",true,   "0"),
            ("ntske.ports",              "NTSKEServerPort",  true,   "4460"),
            ("ntske.onPinMismatch",      "OnPinMismatch",    true,   "Refuse"),
            ("ntske.trustOnFirstUse",    "TrustOnFirstUse",  true,   "None"),
            ("ntske.aead",               "AEADAlgorithm",    true,   ""),
            ("ntske.minRefresh",         "MinRefresh",       true,   "43200"),
            ("ntske.maxRefresh",         "MaxRefresh",       true,   "86400")
        ];

        /// <summary>
        /// The keys of the legal group alone.
        /// </summary>
        private static readonly (String Key, String Variable)[] legalKeys16 = [
            ("ntp.authority",            "LegalTimeAuthority"),
            ("ntp.tolerance",            "LegalTimeTolerance"),
            ("ntp.maxAge",               "LegalTimeMaxAge")
        ];

        /// <summary>
        /// The pins of the n-th server, numbered from 1.
        /// </summary>
        private static readonly (String Key, String Variable)[] pinKeys16 = [
            ("ntske.certificatePins",    "CertificatePins"),
            ("ntske.rootPins",           "RootPins")
        ];

        /// <summary>
        /// The common key of the reporting threshold, ClockCtrlr's
        /// TimeAdjustmentReportingThreshold.
        /// </summary>
        private const String thresholdKey16 = "ntp.timeAdjustmentReportingThreshold";

        private static readonly Regex key16 = new (@"^(ntp|ntske)\.([a-z][a-z0-9-]*)\.([A-Za-z]+)(?:\.([1-9][0-9]*))?$",
                                                   RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        #endregion


        #region Keys

        /// <summary>
        /// The keys of every group there is now, with one pair of pin keys per
        /// server of it.
        /// </summary>
        public IEnumerable<String> Keys
        {
            get
            {

                var keys = new List<String>();

                if (node.NTSGroupOf(NTSGroup.LegalId) is not null)
                    keys.Add(thresholdKey16);

                foreach (var group in Groups)
                {

                    foreach (var (key, _, _, _) in groupKeys16)
                        keys.Add(NameOf(key, group));

                    if (String.Equals(group, NTSGroup.LegalId, StringComparison.OrdinalIgnoreCase))
                        foreach (var (key, _) in legalKeys16)
                            keys.Add(NameOf(key, group));

                    var count = ClientsOf(group).Count();

                    foreach (var (key, _) in pinKeys16)
                        for (var n = 1; n <= count; n++)
                            keys.Add($"{NameOf(key, group)}.{n}");

                }

                return keys;

            }
        }

        #endregion

        #region Get(Key)

        /// <summary>
        /// The key of this name with what it says now, or null where it is none
        /// of the time servers' keys.
        /// </summary>
        public OCPP16.ConfigurationKey? Get(String Key)
        {

            if (IsThresholdKey(Key))
                return node.NTSGroupOf(NTSGroup.LegalId) is not null
                           ? new OCPP16.ConfigurationKey(thresholdKey16, OCPP16.AccessRights.ReadWrite, ClockValue("TimeAdjustmentReportingThreshold") ?? "")
                           : null;

            if (!TryKey(Key, out var group, out var name, out var variable, out var perServer, out var number))
                return null;

            var clients = ClientsOf(group).ToArray();
            String? value;

            if (number is Int32 n)
            {

                if (n > clients.Length)
                    return null;

                value = ClientValue(clients[n - 1], variable);

            }

            else if (perServer)
                value = String.Join(",", clients.Select(client => ClientValue(client, variable) ?? ""));

            else if (variable == "Mode")
                value = "NTSv4";

            else
                value = GroupValue(group, variable);

            return value is null
                       ? null
                       : new OCPP16.ConfigurationKey(name, OCPP16.AccessRights.ReadWrite, value);

        }

        #endregion

        #region ChangeAsync(Key, Value, Sender, CancellationToken = default)

        /// <summary>
        /// Change the key of this name as the CSMS of OCPP 2.1 changes the
        /// variables it stands for - or answer null where it is none of the
        /// time servers' keys.
        /// </summary>
        public async Task<OCPP16.ConfigurationStatus?> ChangeAsync(String             Key,
                                                                   String             Value,
                                                                   String?            Sender,
                                                                   CancellationToken  CancellationToken = default)
        {

            if (IsThresholdKey(Key))
                return Get(Key) is null
                           ? null
                           : await SetAsync([ new OCPP21.SetVariableData(new OCPP21.Component(OCPP21.ClockController.ComponentName),
                                                                         new OCPP21.Variable("TimeAdjustmentReportingThreshold"),
                                                                         Value.Trim()) ],
                                            Sender,
                                            CancellationToken);

            if (Get(Key) is null ||
                !TryKey(Key, out var group, out _, out var variable, out var perServer, out var number))
            {
                return null;
            }

            var clients  = ClientsOf(group).ToArray();
            var entries  = new List<OCPP21.SetVariableData>();

            if (number is Int32 n)
                entries.Add(ClientEntry(clients[n - 1], variable, Value.Trim()));

            else if (variable == "Mode")
                return String.Equals(Value.Trim(), "NTSv4", StringComparison.OrdinalIgnoreCase)
                           ? OCPP16.ConfigurationStatus.Accepted
                           : OCPP16.ConfigurationStatus.Rejected;

            else if (variable is "MinInterval" or "MaxInterval")
            {
                entries.Add(GroupEntry(group, "MinInterval", Value.Trim()));
                entries.Add(GroupEntry(group, "MaxInterval", Value.Trim()));
            }

            else if (variable == "ServerURL")
            {
                if (!TryServers(group, clients, Value, entries))
                    return OCPP16.ConfigurationStatus.Rejected;
            }

            else if (perServer)
            {

                if (!TryList(Value, clients.Length, DefaultOf(variable), out var values, EmptyIsUnset: variable == "AEADAlgorithm"))
                    return OCPP16.ConfigurationStatus.Rejected;

                for (var index = 0; index < clients.Length; index++)
                    entries.Add(ClientEntry(clients[index], variable, values[index]));

            }

            else
                entries.Add(GroupEntry(group, variable, Value.Trim()));

            if (entries.Count == 0)
                return OCPP16.ConfigurationStatus.Accepted;

            return await SetAsync(entries, Sender, CancellationToken);

        }

        #endregion


        #region (private) SetAsync(Entries, Sender, CancellationToken)

        /// <summary>
        /// The entries a key stands for, set as the CSMS of OCPP 2.1 sets them,
        /// with ACID where they are several: accepted where all of them are.
        /// </summary>
        private async Task<OCPP16.ConfigurationStatus> SetAsync(List<OCPP21.SetVariableData>  Entries,
                                                                String?                       Sender,
                                                                CancellationToken             CancellationToken)
        {

            OCPP21.ComponentConfig[] components = [ new OCPP21.ClockController(this), .. OCPP21.NTPController.AllOf(this) ];

            var results = await OCPP21.DeviceModel.SetVariablesAsync(
                                    new OCPP21.CSMS.SetVariablesRequest(
                                        Destination:           SourceRouting.CSMS,
                                        VariableData:          Entries,
                                        DataConsistencyModel:  Entries.Count > 1 ? OCPP21.DataConsistencyModel.ACID : null,
                                        NetworkPath:           NetworkingNode_Id.TryParse(Sender ?? "", out var sender)
                                                                   ? NetworkPath.From(sender)
                                                                   : NetworkPath.Empty
                                    ),
                                    (name, evse) => components.Where(component => String.Equals(component.Name, name, StringComparison.OrdinalIgnoreCase)),
                                    CancellationToken,
                                    NewInstance
                                );

            return results.All(result => result.AttributeStatus == OCPP21.SetVariableStatus.Accepted)
                       ? OCPP16.ConfigurationStatus.Accepted
                       : OCPP16.ConfigurationStatus.Rejected;

        }

        #endregion


        #region (private) TryServers(Group, Clients, Value, Entries)

        /// <summary>
        /// The entries "ntp.&lt;group&gt;.servers" stands for: a new host for a
        /// server at its place, a new server at the end with what the last one
        /// is held to beside its pins, and the group's list without the servers
        /// beyond the end.
        /// </summary>
        private Boolean TryServers(String                        Group,
                                   String[]                      Clients,
                                   String                        Value,
                                   List<OCPP21.SetVariableData>  Entries)
        {

            if (!TryList(Value, null, "", out var hosts) || hosts.Any(host => host.Length == 0))
                return false;

            var ids    = new List<String>();
            var taken  = new HashSet<String>(Groups.SelectMany(ClientsOf), StringComparer.OrdinalIgnoreCase);

            for (var index = 0; index < hosts.Count; index++)
            {

                if (index < Clients.Length)
                {

                    ids.Add(Clients[index]);

                    if (!String.Equals(ClientValue(Clients[index], "ServerURL"), hosts[index], StringComparison.OrdinalIgnoreCase))
                        Entries.Add(ClientEntry(Clients[index], "ServerURL", hosts[index]));

                    continue;

                }

                // Named as the node names a server that has no id of its own,
                // and made with what the last server is held to beside its
                // pins: what "the last value repeats" says of every other list.
                var id = Enumerable.Range(1, Int32.MaxValue - 1).Select(n => $"{Group}-{n}").First(candidate => !taken.Contains(candidate));

                taken.Add(id);
                ids.  Add(id);

                Entries.Add(ClientEntry(id, "ServerURL", hosts[index]));

                if (Clients.Length > 0)
                {

                    foreach (var variable in new[] { "ServerPort", "NTSKEServerPort", "Priority", "RootCAs", "OnPinMismatch", "TrustOnFirstUse" })
                        if (ClientValue(Clients[^1], variable) is String last && last.Length > 0)
                            Entries.Add(ClientEntry(id, variable, last));

                    // The AEAD algorithm its entry offers, and not what
                    // AEADAlgorithm reads where it names none - the one its
                    // last key exchange agreed on, which the new server would
                    // then be held to offer alone.
                    if (AEADSetFor(Clients[^1]) is { Length: > 0 } aead)
                        Entries.Add(ClientEntry(id, "AEADAlgorithm", aead));

                }

            }

            if (!ids.SequenceEqual(Clients, StringComparer.OrdinalIgnoreCase))
                Entries.Insert(0, GroupEntry(Group, "Clients", String.Join(",", ids)));

            return true;

        }

        #endregion

        #region (private static) TryList(Value, Count, Default, out Values)

        /// <summary>
        /// A comma separated list as the paper reads it: an empty entry is the
        /// one before it - the default for the first - and where a count is
        /// given, a shorter list's last value holds for the rest, and a longer
        /// one is no list for it.
        /// </summary>
        /// <param name="EmptyIsUnset">Whether an empty entry is a server for which nothing is set, rather than the one before it: for the AEAD algorithm, where "" offers every one.</param>
        private static Boolean TryList(String                 Value,
                                       Int32?                 Count,
                                       String                 Default,
                                       out List<String>       Values,
                                       Boolean                EmptyIsUnset = false)
        {

            Values = [];

            var written = Value.Trim().Length == 0 ? [] : Value.Split(',').Select(entry => entry.Trim()).ToArray();

            if (Count is Int32 count && written.Length > count)
                return false;

            var previous = Default;

            foreach (var entry in written)
            {
                previous = entry.Length > 0 || EmptyIsUnset ? entry : previous;
                Values.Add(previous);
            }

            while (Count is Int32 wanted && Values.Count < wanted)
                Values.Add(previous);

            return true;

        }

        #endregion

        #region (private static) TryKey(Key, out Group, out Name, out Variable, out PerServer, out Number)

        /// <summary>
        /// A key of the paper this node has: its group, its name as the paper
        /// writes it, the variable it stands for, whether it is a list per
        /// server, and the number of the server for a pin key.
        /// </summary>
        private Boolean TryKey(String       Key,
                               out String   Group,
                               out String   Name,
                               out String   Variable,
                               out Boolean  PerServer,
                               out Int32?   Number)
        {

            Group      = "";
            Name       = "";
            Variable   = "";
            PerServer  = false;
            Number     = null;

            var match  = key16.Match(Key.Trim());

            if (!match.Success)
                return false;

            var prefix  = match.Groups[1].Value.ToLowerInvariant();
            var group   = Groups.FirstOrDefault(id => String.Equals(id, match.Groups[2].Value, StringComparison.OrdinalIgnoreCase));
            var name    = $"{prefix}.{match.Groups[3].Value}";

            if (group is null)
                return false;

            Group = group;

            if (match.Groups[4].Success)
            {

                if (pinKeys16.FirstOrDefault(pin => String.Equals(pin.Key, name, StringComparison.OrdinalIgnoreCase)) is not { Key: not null } pin)
                    return false;

                Number    = Int32.Parse(match.Groups[4].Value);
                Variable  = pin.Variable;
                Name      = $"{NameOf(pin.Key, group)}.{Number}";
                return true;

            }

            if (groupKeys16.FirstOrDefault(entry => String.Equals(entry.Key, name, StringComparison.OrdinalIgnoreCase)) is { Key: not null } known)
            {
                Variable   = known.Variable;
                PerServer  = known.PerServer;
                Name       = NameOf(known.Key, group);
                return true;
            }

            if (String.Equals(group, NTSGroup.LegalId, StringComparison.OrdinalIgnoreCase) &&
                legalKeys16.FirstOrDefault(entry => String.Equals(entry.Key, name, StringComparison.OrdinalIgnoreCase)) is { Key: not null } legal)
            {
                Variable  = legal.Variable;
                Name      = NameOf(legal.Key, group);
                return true;
            }

            return false;

        }

        #endregion

        #region (private static) IsThresholdKey(Key)

        private static Boolean IsThresholdKey(String Key)

            => String.Equals(Key.Trim(), thresholdKey16, StringComparison.OrdinalIgnoreCase);

        #endregion

        #region (private static) NameOf / DefaultOf / GroupEntry / ClientEntry

        /// <summary>
        /// "ntp.minServers" of the group "legal": "ntp.legal.minServers".
        /// </summary>
        private static String NameOf(String  Key,
                                     String  Group)
        {
            var dot = Key.IndexOf('.');
            return $"{Key[..dot]}.{Group}{Key[dot..]}";
        }

        private static String DefaultOf(String Variable)

            => groupKeys16.FirstOrDefault(entry => entry.Variable == Variable).Default ?? "";

        private static OCPP21.SetVariableData GroupEntry(String  Group,
                                                         String  Variable,
                                                         String  Value)

            => new (new OCPP21.Component("NTPClientGroupController", Group), new OCPP21.Variable(Variable), Value);

        private static OCPP21.SetVariableData ClientEntry(String  Client,
                                                          String  Variable,
                                                          String  Value)

            => new (new OCPP21.Component("NTPClientController", Client), new OCPP21.Variable(Variable), Value);

        #endregion

    }

}
