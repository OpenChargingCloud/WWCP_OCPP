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

using org.GraphDefined.Vanaheimr.Norn.Monitoring;
using org.GraphDefined.Vanaheimr.Norn.NTS;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Configuration;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// What the OCPP Device Model shows of how this node keeps its time:
    /// the components NTPController, NTPClientGroupController with a group's
    /// id as its instance, and NTPClientController with a server's id - and
    /// the standard ClockCtrlr, filled from the legal group - read from the
    /// node each time, never kept here.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The node is the one place the configuration lives. This only says it in
    /// the words of the Secure Time Synchronization white paper (section 3.2),
    /// from the JSON the web page reads, at the moment a CSMS asks. A value
    /// kept here would be a second truth, left behind by every change from the
    /// page, and by a pin learned on first use.
    /// </para>
    /// <para>
    /// Every variable of the paper that the node can answer is here; the rest
    /// (Preflight, the delay asymmetries, a separate NTS-KE server,
    /// NoCertTimeCheckAfterReboot, MinRefresh and MaxRefresh, signed
    /// responses) is not, so that a CSMS asking for them learns they are
    /// unknown rather than reading an empty value as an answer.
    /// NTS-DeviceModel-Concept.md B.6 to B.9 say where each value comes from.
    /// </para>
    /// <para>
    /// Which groups and servers there are is read from the node as well, but
    /// the components standing for them are made once: whoever registers them
    /// follows the node's OnNTSGroupAdded, OnNTSGroupRemoved and
    /// OnNTSGroupChanged - see ChargingStation.ConfigureNTS.
    /// </para>
    /// <para>
    /// Made with a log, it takes the writes of a SetVariables request as well,
    /// and makes the groups and servers a request names anew - see
    /// NTSDeviceModel.Writing.cs.
    /// </para>
    /// </remarks>
    public sealed partial class NTSDeviceModel : protocols.OCPPv2_1.INTPControllerSource,
                                         protocols.OCPPv2_1.IClockControllerSource
    {

        #region Data

        private static readonly (String Name, Func<WWCPNode, String> Value)[] controllerVariables = [
            ("SuggestedGroups",     node             => String.Join(",", NTSSection.SuggestedIds.Where(id => node.NTSGroups.All(group => group.Id != id)))),
            ("LegalTime",           node             => BooleanOf  (node.ClockJSON()["legal"])),
            ("LegalTimeReason",     node             => ReasonOf   (node.ClockJSON()["why"]))
        ];

        private static readonly (String Name, Func<JObject, NTSGroup, String> Value)[] groupVariables = [
            ("Enabled",             (json, group)    => BooleanOf  (json["enabled"])),
            ("MinServers",          (json, group)    => IntegerOf  (Child(json["settings"], "minServers"))),
            ("MaxDeviation",        (json, group)    => SecondsOf  (Child(json["settings"], "maxDeviationSeconds"))),
            ("MinInterval",         (json, group)    => IntegerOf  (Child(json["settings"], "checkEverySeconds"))),
            ("MaxInterval",         (json, group)    => IntegerOf  (Child(json["settings"], "checkEverySeconds"))),
            ("Timeout",             (json, group)    => SecondsOf  (Child(json["settings"], "timeoutSeconds"))),
            ("ErrorLogging",        (json, group)    => IntegerOf  (Child(json["settings"], "errorLogging"))),
            ("MaxErrors",           (json, group)    => IntegerOf  (Child(json["settings"], "maxErrors"))),
            ("Limited",             (json, group)    => BooleanOf  (Child(json["errors"],   "limited"))),
            ("ErrorsInARow",        (json, group)    => IntegerOf  (Child(json["errors"],   "inARow"))),
            ("LastCheck",           (json, group)    => DateTimeOf (group.LastCheck)),
            ("LastOffset",          (json, group)    => SecondsOf  (group.LastCheckOffset)),
            ("LegalTimeAuthority",  (json, group)    => TextOf     (Child(json["settings"], "legalTimeAuthority"))),
            ("LegalTimeTolerance",  (json, group)    => SecondsOf  (Child(json["settings"], "legalTimeToleranceSeconds"))),
            ("LegalTimeMaxAge",     (json, group)    => IntegerOf  (Child(json["settings"], "legalTimeMaxAgeSeconds")))
        ];

        private static readonly (String Name, Func<JObject, String> Value)[] clientVariables = [
            ("Enabled",                 server       => BooleanOf  (server["enabled"])),
            ("ServerURL",               server       => HostOf     (server["hostname"])),
            ("ServerPort",              server       => IntegerOf  (server["ntpPort"])),
            ("Mode",                    server       => "NTSv4"),
            ("Priority",                server       => IntegerOf  (server["priority"])),
            ("NTSKEServerPort",         server       => IntegerOf  (server["ntsKEPort"])),
            ("CertificatePins",         server       => PinsOf     (server["heldTo"], "certificates")),
            ("RootPins",                server       => PinsOf     (server["heldTo"], "roots")),
            ("OnPinMismatch",           server       => OptionOf   (Child(server["heldTo"], "onMismatch"),      "Refuse")),
            ("TrustOnFirstUse",         server       => OptionOf   (Child(server["heldTo"], "trustOnFirstUse"), "None")),
            ("RootCAs",                 server       => TextOf     (Child(server["heldTo"], "rootGroup"))),
            ("AEADAlgorithm",           server       => AEADOf     (server["aead"], server["aeadAlgorithm"])),
            ("MinRefresh",              server       => RefreshOf  (server).Min),
            ("MaxRefresh",              server       => RefreshOf  (server).Max),
            ("Problem",                 server       => ProblemOf  (server["judgement"])),
            ("CertificateFingerprint",  server       => TextOf     (server["certificate"])),
            ("LastKeyExchange",         server       => DateTimeOf (server["lastExchange"])),
            ("Cookies",                 server       => IntegerOf  (server["cookies"]))
        ];

        private static readonly (String Name, Func<WWCPNode, String?, String?> Value)[] clockVariables = [
            ("DateTime",            (node, instance) => DateTimeOf (node.TimeProvider.GetUtcNow())),
            ("NtpServerUri",        (node, instance) => NtpServerUriOf(node, instance)),
            ("NtpSource",           (node, instance) => "manual"),
            ("TimeAdjustmentReportingThreshold",
                                    (node, instance) => SecondsOf(node.NTSGroupOf(NTSGroup.LegalId)?.OffsetReportingThreshold)),
            ("TimeSource",          (node, instance) => node.NTSEnabled ? "NTP,RealTimeClock" : "RealTimeClock")
        ];

        private static readonly Dictionary<String, Func<WWCPNode, String>> controllerVariableNamed
            = controllerVariables.ToDictionary(variable => variable.Name, variable => variable.Value, StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<String, Func<JObject, NTSGroup, String>> groupVariableNamed
            = groupVariables.     ToDictionary(variable => variable.Name, variable => variable.Value, StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<String, Func<JObject, String>> clientVariableNamed
            = clientVariables.    ToDictionary(variable => variable.Name, variable => variable.Value, StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<String, Func<WWCPNode, String?, String?>> clockVariableNamed
            = clockVariables.     ToDictionary(variable => variable.Name, variable => variable.Value, StringComparer.OrdinalIgnoreCase);

        private readonly WWCPNode node;

        #endregion

        #region Properties

        /// <summary>
        /// The variables of NTPController this source answers, besides
        /// ClientGroups, in the order of the paper.
        /// </summary>
        public static IEnumerable<String>  ControllerVariables
            => controllerVariables.Select(variable => variable.Name);

        /// <summary>
        /// The variables of each NTPClientGroupController this source answers,
        /// besides Clients, in the order of the paper.
        /// </summary>
        public static IEnumerable<String>  GroupVariables
            => groupVariables.Select(variable => variable.Name);

        /// <summary>
        /// The variables of each NTPClientController this source answers, in the
        /// order of the paper.
        /// </summary>
        public static IEnumerable<String>  ClientVariables
            => clientVariables.Select(variable => variable.Name);

        /// <summary>
        /// The variables of ClockCtrlr this source answers.
        /// </summary>
        public static IEnumerable<String>  ClockVariables
            => clockVariables.Select(variable => variable.Name);

        /// <summary>
        /// The ids of the node's groups of time servers, in their order.
        /// </summary>
        public IEnumerable<String>         Groups
            => node.NTSGroups.Select(group => group.Id);

        IEnumerable<String> protocols.OCPPv2_1.INTPControllerSource.  ControllerVariables
            => ControllerVariables;

        IEnumerable<String> protocols.OCPPv2_1.INTPControllerSource.  GroupVariables
            => GroupVariables;

        IEnumerable<String> protocols.OCPPv2_1.INTPControllerSource.  ClientVariables
            => ClientVariables;

        // The node asks its time servers by NTS only, so a CSMS is offered no
        // other mode: neither NTPv4 nor NTPv4TLS.
        IEnumerable<String>? protocols.OCPPv2_1.INTPControllerSource. SupportedClientValues(String Variable)
            => String.Equals(Variable, "Mode", StringComparison.OrdinalIgnoreCase)
                   ? [ "NTSv4" ]
                   : null;

        IEnumerable<String> protocols.OCPPv2_1.IClockControllerSource.Variables
            => ClockVariables;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The Device Model view of the time servers of the given node.
        /// </summary>
        /// <param name="Node">The node whose groups of time servers are shown, asked anew at every read.</param>
        public NTSDeviceModel(WWCPNode Node)
        {
            this.node = Node;
        }

        #endregion


        #region ClientsOf(Group)

        /// <summary>
        /// The ids of the servers of the given group, in the order of its list:
        /// none for a group the node does not have.
        /// </summary>
        /// <param name="Group">The id of the group, in any case.</param>
        public IEnumerable<String> ClientsOf(String Group)

            => GroupOf(Group)?.ServerIds ?? [];

        #endregion

        #region ControllerValue(Variable)

        /// <summary>
        /// The value of a variable of NTPController, as OCPP writes it, or null
        /// where this source does not answer it.
        /// </summary>
        /// <param name="Variable">The name of the variable, in any case.</param>
        public String? ControllerValue(String Variable)

            => controllerVariableNamed.TryGetValue(Variable, out var value)
                   ? value(node)
                   : null;

        #endregion

        #region GroupValue(Group, Variable)

        /// <summary>
        /// The value of a variable of the NTPClientGroupController of the given
        /// group, as OCPP writes it: "" where the node has no value yet, and
        /// null where there is no such group or no such variable.
        /// </summary>
        /// <param name="Group">The id of the group, in any case.</param>
        /// <param name="Variable">The name of the variable, in any case.</param>
        public String? GroupValue(String  Group,
                                  String  Variable)

            => groupVariableNamed.TryGetValue(Variable, out var value) &&
               GroupOf(Group) is NTSGroup group
                   ? value(node.NTSConfigurationJSON(group), group)
                   : null;

        #endregion

        #region ClientValue(Client, Variable)

        /// <summary>
        /// The value of a variable of the NTPClientController of the given
        /// server, as OCPP writes it: "" where the node has no value yet, and
        /// null where there is no such server or no such variable.
        /// </summary>
        /// <remarks>
        /// A server id names one server across every group, so the first group
        /// naming it is the one.
        /// </remarks>
        /// <param name="Client">The id of the server, in any case.</param>
        /// <param name="Variable">The name of the variable, in any case.</param>
        public String? ClientValue(String  Client,
                                   String  Variable)
        {

            if (!clientVariableNamed.TryGetValue(Variable, out var value))
                return null;

            return ServerOf(Client) is JObject server
                       ? value(server)
                       : null;

        }

        #endregion

        #region (private) ServerOf(Client) / AEADSetFor(Client)

        /// <summary>
        /// The server of that id as its group shows it, or null.
        /// </summary>
        private JObject? ServerOf(String Client)
        {

            var group = node.NTSGroups.FirstOrDefault(group => group.ServerIds.Contains(Client, StringComparer.OrdinalIgnoreCase));

            return group is not null &&
                   node.NTSConfigurationJSON(group)["timeSources"] is JArray servers
                       ? servers.OfType<JObject>().FirstOrDefault(server => String.Equals(TextOf(server["id"]), Client, StringComparison.OrdinalIgnoreCase))
                       : null;

        }

        /// <summary>
        /// The one AEAD algorithm the server's own entry offers, or "" where it
        /// names none - not what AEADAlgorithm reads then, which is what the
        /// last key exchange agreed on.
        /// </summary>
        private String AEADSetFor(String Client)

            => TextOf(ServerOf(Client)?["aead"]);

        #endregion

        #region ClockValue(Variable, Instance = null)

        /// <summary>
        /// The value of a variable of ClockCtrlr, as OCPP writes it, or null
        /// where this source does not answer it.
        /// </summary>
        /// <param name="Variable">The name of the variable, in any case.</param>
        /// <param name="Instance">The instance of the variable: "1" to "9" for NtpServerUri.</param>
        public String? ClockValue(String   Variable,
                                  String?  Instance   = null)

            => clockVariableNamed.TryGetValue(Variable, out var value)
                   ? value(node, Instance)
                   : null;

        String? protocols.OCPPv2_1.IClockControllerSource.Value(String   Variable,
                                                               String?  Instance)

            => ClockValue(Variable, Instance);

        #endregion


        #region (private) GroupOf(Group)

        private NTSGroup? GroupOf(String Group)

            => node.NTSGroups.FirstOrDefault(group => String.Equals(group.Id, Group, StringComparison.OrdinalIgnoreCase));

        #endregion

        #region (private static) NtpServerUriOf(Node, Instance)

        /// <summary>
        /// The k-th server the legal group asks, in the order it asks them -
        /// by priority, then as its list names them - as "nts://host", with
        /// the port where it is not the default one; "" where it asks fewer,
        /// and for every k where the node has no legal group.
        /// </summary>
        /// <remarks>
        /// Only the servers that are switched on: ClockCtrlr says where the
        /// time comes from, and a server switched off is not asked.
        /// </remarks>
        private static String? NtpServerUriOf(WWCPNode  Node,
                                              String?   Instance)
        {

            if (!Byte.TryParse(Instance, NumberStyles.None, CultureInfo.InvariantCulture, out var k) || k < 1)
                return null;

            var asked = Node.NTSGroupOf(NTSGroup.LegalId)?.Sources.Bands().SelectMany(band => band).ToArray() ?? [];

            return k <= asked.Length
                       ? UriOf(asked[k - 1])
                       : "";

        }

        private static String UriOf(NTSServerEndpoint Server)

            => Server.NTSKEPort == NTSClient.DefaultNTSKE_Port
                   ? $"nts://{Server.Hostname.Trimmed}"
                   : $"nts://{Server.Hostname.Trimmed}:{Server.NTSKEPort.ToUInt16()}";

        #endregion

        #region (private static) Child(Token, Key)

        /// <summary>
        /// What an object holds under a key, or null where there is no object:
        /// the node writes null for pins, a judgement or a key exchange it does
        /// not have, and a null holds nothing to be asked for.
        /// </summary>
        private static JToken? Child(JToken?  Token,
                                     String   Key)

            => (Token as JObject)?[Key];

        #endregion

        #region (private static) BooleanOf, IntegerOf, SecondsOf, TextOf, HostOf, DateTimeOf

        /// <summary>
        /// A boolean as OCPP writes one, or "" for none.
        /// </summary>
        private static String BooleanOf(JToken? Token)

            => Token?.Type == JTokenType.Boolean
                   ? (Token.Value<Boolean>() ? "true" : "false")
                   : "";

        /// <summary>
        /// A whole number, or "" for none.
        /// </summary>
        /// <summary>
        /// When the server's key exchange is made again, in whole seconds: what
        /// its own entry sets, and the node's defaults for what it does not, by
        /// the node's rule (NTSServerConfiguration.RefreshOf).
        /// </summary>
        private static (String Min, String Max) RefreshOf(JObject Server)
        {

            static TimeSpan? SecondsIn(JToken? Token)

                => Token?.Type is JTokenType.Integer or JTokenType.Float
                       ? TimeSpan.FromSeconds(Token.Value<Double>())
                       : null;

            var (min, max) = NTSServerConfiguration.RefreshOf(SecondsIn(Server["minRefreshSeconds"]),
                                                              SecondsIn(Server["maxRefreshSeconds"]));

            return (((Int64) min.TotalSeconds).ToString(CultureInfo.InvariantCulture),
                    ((Int64) max.TotalSeconds).ToString(CultureInfo.InvariantCulture));

        }

        private static String IntegerOf(JToken? Token)

            => Token?.Type is JTokenType.Integer or JTokenType.Float
                   ? Token.Value<Int64>().ToString(CultureInfo.InvariantCulture)
                   : "";

        /// <summary>
        /// A number of seconds with no digits it does not need, the same on
        /// every machine: ten seconds are "10" and a tenth is "0.1".
        /// </summary>
        private static String SecondsOf(JToken? Token)

            => Token?.Type is JTokenType.Integer or JTokenType.Float
                   ? Token.Value<Decimal>().ToString("0.###", CultureInfo.InvariantCulture)
                   : "";

        /// <summary>
        /// A span of time in seconds, as above, or "" for none.
        /// </summary>
        private static String SecondsOf(TimeSpan? Span)

            => Span.HasValue
                   ? ((Decimal) Span.Value.TotalSeconds).ToString("0.###", CultureInfo.InvariantCulture)
                   : "";

        /// <summary>
        /// A text, or "" for none.
        /// </summary>
        private static String TextOf(JToken? Token)

            => Token?.Type == JTokenType.String
                   ? Token.Value<String>() ?? ""
                   : "";

        /// <summary>
        /// A host name as it was written, or "" for none: the node writes the
        /// name with the dot that ends every name in DNS, "ptbtime1.ptb.de.",
        /// which nobody types.
        /// </summary>
        private static String HostOf(JToken? Token)

            => TextOf(Token).TrimEnd('.');

        /// <summary>
        /// A point in time as OCPP 2.x writes one, or "" for none: RFC 3339 in
        /// UTC with at most three digits of the second, where the node writes
        /// seven.
        /// </summary>
        private static String DateTimeOf(JToken? Token)
        {

            DateTimeOffset at;

            if (Token?.Type == JTokenType.Date)
                at = Token.Value<DateTimeOffset>();

            else if (Token?.Type != JTokenType.String ||
                     !DateTimeOffset.TryParse(Token.Value<String>(),
                                              CultureInfo.InvariantCulture,
                                              DateTimeStyles.AssumeUniversal,
                                              out at))
            {
                return "";
            }

            return DateTimeOf(at);

        }

        /// <summary>
        /// A point in time as OCPP 2.x writes one, or "" for none.
        /// </summary>
        private static String DateTimeOf(DateTimeOffset? At)

            => At.HasValue
                   ? At.Value.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture)
                   : "";

        #endregion

        #region (private static) OptionOf(Token, Otherwise)

        /// <summary>
        /// A word of the node's ("record", "certificate") as an entry of a
        /// valuesList ("Record", "Certificate"), or what a missing one means.
        /// </summary>
        private static String OptionOf(JToken?  Token,
                                       String   Otherwise)
        {

            var word = TextOf(Token);

            return word.Length > 0
                       ? Char.ToUpperInvariant(word[0]) + word[1..]
                       : Otherwise;

        }

        #endregion

        #region (private static) ReasonOf(Token)

        /// <summary>
        /// Why the time is not legal time, in the words of the valuesList of
        /// NTPController.LegalTimeReason, or "" while it is.
        /// </summary>
        private static String ReasonOf(JToken? Token)

            => TextOf(Token) switch {
                   "notClaimed"    => "NotClaimed",
                   "ntsOff"        => "Disabled",
                   "neverChecked"  => "NeverChecked",
                   "stale"         => "Stale",
                   "offBy"         => "OffBy",
                   _               => ""
               };

        #endregion

        #region (private static) AEADOf(Set, Agreed)

        /// <summary>
        /// The one AEAD algorithm the server's entry offers, where it names
        /// one; otherwise the one its last key exchange agreed on, as the paper
        /// spells it ("AES-SIV-CMAC-256" for the node's "AES_SIV_CMAC_256"), or
        /// "" before the first one.
        /// </summary>
        /// <param name="Set">What the server's own entry names, as the node shows it: "aead".</param>
        /// <param name="Agreed">What the last key exchange agreed on: "aeadAlgorithm".</param>
        private static String AEADOf(JToken?  Set,
                                     JToken?  Agreed)

            => TextOf(Set) is { Length: > 0 } set
                   ? set
                   : TextOf(Agreed).Replace('_', '-');

        #endregion

        #region (private static) ProblemOf(Judgement)

        /// <summary>
        /// "true" while the last key exchange with the server was refused for
        /// its certificate, and "false" otherwise - before the first one as
        /// well.
        /// </summary>
        private static String ProblemOf(JToken? Judgement)

            => Judgement is JObject judgement && judgement["accepted"]?.Type == JTokenType.Boolean && !judgement.Value<Boolean>("accepted")
                   ? "true"
                   : "false";

        #endregion

        #region (private static) PinsOf(HeldTo, Key)

        /// <summary>
        /// The fingerprints of one kind a server is held to, comma separated, or
        /// "" where it is held to none.
        /// </summary>
        private static String PinsOf(JToken?  HeldTo,
                                     String   Key)

            => Child(HeldTo, Key) is JArray pins
                   ? String.Join(",", pins.Values<String>().OfType<String>())
                   : "";

        #endregion

    }

}
