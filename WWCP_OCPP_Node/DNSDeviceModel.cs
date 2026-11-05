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

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// What the OCPP Device Model shows of how this node resolves names: the
    /// component DNSClientController for name resolution as a whole, and the
    /// component DNSServerController once per name server, its id the instance
    /// - read from the node each time, never kept here.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The node is the one place the configuration lives. This only says it in
    /// the words of the Secure DNS white paper (section 3.1), from the JSON the
    /// web page reads, at the moment a CSMS asks. A value kept here would be a
    /// second truth, left behind by every change from the page, from the
    /// command line, and by a pin learned on first use.
    /// </para>
    /// <para>
    /// Every variable of the paper that the node can answer is here; the rest
    /// (AuthenticationName, Enabled and Priority of a server) is not, so that
    /// a CSMS asking for them learns they are unknown rather than reading an
    /// empty value as an answer.
    /// SecureDNS-DeviceModel-Concept.md B.7 and B.8 say where each value
    /// comes from.
    /// </para>
    /// <para>
    /// Made with a way to change the node, it takes the writes of a
    /// SetVariables request as well - see DNSDeviceModel.Writing.cs.
    /// </para>
    /// </remarks>
    public sealed partial class DNSDeviceModel : protocols.OCPPv2_1.IDNSControllerSource
    {

        #region Data

        /// <summary>
        /// The component of name resolution as a whole. It has no instance.
        /// </summary>
        public const String  ClientComponent  = "DNSClientController";

        /// <summary>
        /// The component of one name server, its id the instance.
        /// </summary>
        public const String  ServerComponent  = "DNSServerController";

        /// <summary>
        /// How many name servers there may be when the node does not say: the
        /// 16 its file takes.
        /// </summary>
        public const UInt16  DefaultMaxServers = 16;

        private static readonly (String Name, Func<JObject, String> Value)[] clientVariables = [
            ("Enabled",           dns    => BooleanOf  (dns["enabled"])),
            ("QueryTimeout",      dns    => SecondsOf  (Child(dns["settings"], "queryTimeoutSeconds"))),
            ("MaxRetries",        dns    => IntegerOf  (Child(dns["settings"], "maxRetries"))),
            ("RecursionDesired",  dns    => RecursionOf(Child(dns["settings"], "recursionDesired"))),
            ("UseCache",          dns    => BooleanOf  (Child(dns["settings"], "useCache"))),
            ("DNSSECOK",          dns    => BooleanOf  (Child(dns["settings"], "dnssecOK"))),
            ("FollowCNAMEs",      dns    => BooleanOf  (Child(dns["settings"], "followCNAMEs"))),
            ("MaxCNAMEFollows",   dns    => IntegerOf  (Child(dns["settings"], "maxCNAMEFollows"))),
            ("ServerSource",      dns    => SourceOf   (dns["serversFrom"])),
            ("Servers",           dns    => String.Join(",", IdsIn(dns))),
            ("MaxServers",        dns    => MaxServersIn(dns).ToString(CultureInfo.InvariantCulture)),
            ("UDPPayloadSize",    dns    => IntegerOf  (Child(dns["fixed"],    "udpPayloadSize"))),
            ("ClientSubnet",      dns    => TextOf     (Child(dns["fixed"],    "clientSubnet"))),
            ("EncryptedOnly",     dns    => BooleanOf  (Child(dns["settings"], "encryptedOnly")))
        ];

        private static readonly (String Name, Func<JObject, String> Value)[] serverVariables = [
            ("Address",                 server => TextOf     (server["address"])),
            ("Port",                    server => IntegerOf  (server["port"])),
            ("Transport",               server => TextOf     (server["transport"])),
            ("QueryTimeout",            server => SecondsOf  (server["queryTimeoutSeconds"])),
            ("CertificatePins",         server => PinsOf     (server, "certificateFingerprint")),
            ("RootPins",                server => PinsOf     (server, "rootFingerprint")),
            ("OnPinMismatch",           server => OptionOf   (server["onMismatch"],      "Refuse")),
            ("TrustOnFirstUse",         server => OptionOf   (server["trustOnFirstUse"], "None")),
            ("RootCAs",                 server => TextOf     (server["rootGroup"])),
            ("Problem",                 server => ProblemOf  (server["judgement"])),
            ("CertificateStatus",       server => OptionOf   (Child(server["judgement"], "outcome"), "")),
            ("CertificateFingerprint",  server => TextOf     (Child(server["judgement"], "certificate"))),
            ("RootFingerprint",         server => TextOf     (Child(server["judgement"], "root"))),
            ("KnownSince",              server => DateTimeOf (Child(server["known"],     "since")))
        ];

        private static readonly Dictionary<String, Func<JObject, String>> clientVariableNamed
            = clientVariables.ToDictionary(variable => variable.Name, variable => variable.Value, StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<String, Func<JObject, String>> serverVariableNamed
            = serverVariables.ToDictionary(variable => variable.Name, variable => variable.Value, StringComparer.OrdinalIgnoreCase);

        private readonly Func<JObject> section;

        #endregion

        #region Properties

        /// <summary>
        /// The variables of DNSClientController, in the order of the paper.
        /// </summary>
        public static IEnumerable<String>  ClientVariables
            => clientVariables.Select(variable => variable.Name);

        /// <summary>
        /// The variables of each DNSServerController, in the order of the paper.
        /// </summary>
        public static IEnumerable<String>  ServerVariables
            => serverVariables.Select(variable => variable.Name);

        /// <summary>
        /// The ids of the name servers, in the order the node keeps them: the
        /// instances of DNSServerController.
        /// </summary>
        public IEnumerable<String>         ServerIds
            => IdsIn(section());

        IEnumerable<String> protocols.OCPPv2_1.IDNSControllerSource.ClientVariables
            => ClientVariables;

        IEnumerable<String> protocols.OCPPv2_1.IDNSControllerSource.ServerVariables
            => ServerVariables;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The Device Model view of the name resolution a node describes.
        /// </summary>
        /// <param name="Section">Where the node says how it resolves names, as WWCPNode.DNSConfigurationJSON does: asked anew at every read.</param>
        public DNSDeviceModel(Func<JObject> Section)
        {
            this.section = Section;
        }

        #endregion


        #region ClientValue(Variable)

        /// <summary>
        /// The value of a variable of DNSClientController, as OCPP writes it, or
        /// null where the component has no such variable.
        /// </summary>
        /// <param name="Variable">The name of the variable, in any case.</param>
        public String? ClientValue(String Variable)

            => ClientValueIn(section(), Variable);

        #endregion

        #region ServerValue(Server, Variable)

        /// <summary>
        /// The value of a variable of the given name server, as OCPP writes it,
        /// or null where there is no such server or no such variable.
        /// </summary>
        /// <param name="Server">The id of the server, in any case.</param>
        /// <param name="Variable">The name of the variable, in any case.</param>
        public String? ServerValue(String  Server,
                                   String  Variable)

            => ServerValueIn(section(), Server, Variable);

        #endregion


        #region (static) ClientValueIn(Section, Variable)

        /// <summary>
        /// The value of a variable of DNSClientController in the given
        /// description of name resolution, or null where there is no such
        /// variable.
        /// </summary>
        /// <param name="Section">How a node resolves names, as WWCPNode.DNSConfigurationJSON says it.</param>
        /// <param name="Variable">The name of the variable, in any case.</param>
        public static String? ClientValueIn(JObject  Section,
                                            String   Variable)

            => clientVariableNamed.TryGetValue(Variable, out var value)
                   ? value(Section)
                   : null;

        #endregion

        #region (static) ServerValueIn(Section, Server, Variable)

        /// <summary>
        /// The value of a variable of the given name server of the given
        /// description of name resolution, or null where there is no such server
        /// or no such variable.
        /// </summary>
        /// <remarks>
        /// A server is found by the id the node names it with, which it keeps
        /// whichever servers are added or taken away around it, where the file
        /// gives it one; the node gives it one by its place where it does not -
        /// see DNSConfiguration.Ids.
        /// </remarks>
        /// <param name="Section">How a node resolves names, as WWCPNode.DNSConfigurationJSON says it.</param>
        /// <param name="Server">The id of the server, in any case.</param>
        /// <param name="Variable">The name of the variable, in any case.</param>
        public static String? ServerValueIn(JObject  Section,
                                            String   Server,
                                            String   Variable)

            => serverVariableNamed.TryGetValue(Variable, out var value) &&
               ServerIn(Section, Server) is JObject server
                   ? value(server)
                   : null;

        #endregion

        #region (static) IdsIn(Section)

        /// <summary>
        /// The ids of the name servers of the given description of name
        /// resolution, in its order.
        /// </summary>
        /// <param name="Section">How a node resolves names, as WWCPNode.DNSConfigurationJSON says it.</param>
        public static IReadOnlyList<String> IdsIn(JObject Section)

            => [.. (Section["servers"] as JArray ?? []).OfType<JObject>().
                                                       Select(server => server.Value<String>("id")).
                                                       OfType<String>()];

        #endregion


        #region (private static) ServerIn(Section, Server)

        /// <summary>
        /// The entry of the given name server, or null.
        /// </summary>
        private static JObject? ServerIn(JObject  Section,
                                         String   Server)

            => (Section["servers"] as JArray ?? []).OfType<JObject>().
                                                    FirstOrDefault(server => String.Equals(server.Value<String>("id"), Server, StringComparison.OrdinalIgnoreCase));

        #endregion

        #region (private static) MaxServersIn(Section)

        private static UInt16 MaxServersIn(JObject Section)

            => Child(Section["limits"], "maxServers") is JValue { Type: JTokenType.Integer } most
                   ? most.Value<UInt16>()
                   : DefaultMaxServers;

        #endregion

        #region (private static) Child(Token, Key)

        /// <summary>
        /// What an object holds under a key, or null where there is no object:
        /// the node writes null for a judgement or a known certificate it does
        /// not have, and a null holds nothing to be asked for.
        /// </summary>
        private static JToken? Child(JToken?  Token,
                                     String   Key)

            => (Token as JObject)?[Key];

        #endregion

        #region (private static) BooleanOf, IntegerOf, SecondsOf, TextOf, DateTimeOf

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
        /// A text, or "" for none.
        /// </summary>
        private static String TextOf(JToken? Token)

            => Token?.Type == JTokenType.String
                   ? Token.Value<String>() ?? ""
                   : "";

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

            return at.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture);

        }

        #endregion

        #region (private static) RecursionOf(Token)

        /// <summary>
        /// Whether the client asks for recursion: "Set" and "Unset" where it was
        /// told so, "Default" where it leaves the bit as each query has it.
        /// </summary>
        private static String RecursionOf(JToken? Token)

            => Token?.Type == JTokenType.Boolean
                   ? (Token.Value<Boolean>() ? "Set" : "Unset")
                   : "Default";

        #endregion

        #region (private static) SourceOf(Token)

        /// <summary>
        /// Where the node says its name servers are from, as ServerSource says
        /// it: "Manual" for the file's, "System" for the ones it was started
        /// with - the operating system's, for the node's own client.
        /// </summary>
        private static String SourceOf(JToken? Token)

            => TextOf(Token).ToLowerInvariant() switch {
                   "file"   => "Manual",
                   "start"  => "System",
                   _        => ""
               };

        #endregion

        #region (private static) OptionOf(Token, Otherwise)

        /// <summary>
        /// A word of the node's ("pinMismatch", "record") as an entry of a
        /// valuesList ("PinMismatch", "Record"), or what a missing one means.
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

        #region (private static) ProblemOf(Judgement)

        /// <summary>
        /// "true" while the last handshake with the server was refused, and
        /// "false" otherwise - before the first one as well.
        /// </summary>
        private static String ProblemOf(JToken? Judgement)

            => Judgement is JObject judgement && judgement["accepted"]?.Type == JTokenType.Boolean && !judgement.Value<Boolean>("accepted")
                   ? "true"
                   : "false";

        #endregion

        #region (private static) PinsOf(Server, Key)

        /// <summary>
        /// The fingerprints of one kind a server is held to, comma separated: a
        /// single one is written under the key, several under the key with an
        /// "s", and a file may have both.
        /// </summary>
        private static String PinsOf(JObject  Server,
                                     String   Key)
        {

            var pins = new List<String>();

            if (Server[Key] is JValue { Type: JTokenType.String } one)
                pins.Add(one.Value<String>()!);

            if (Server[Key + "s"] is JArray several)
                pins.AddRange(several.Values<String>().OfType<String>());

            return String.Join(",", pins.Distinct(StringComparer.OrdinalIgnoreCase));

        }

        #endregion

    }

}
