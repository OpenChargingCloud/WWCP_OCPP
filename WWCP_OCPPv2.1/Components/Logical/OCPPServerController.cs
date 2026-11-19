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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Where OCPPServerController reads the OCPP server of a node that the
    /// charging stations below it connect to, and what a change of it waits for.
    /// </summary>
    /// <remarks>
    /// The controller keeps no value of its own: whatever holds the configuration
    /// answers each read, so that a change made on a page or in the file is what
    /// the next read sees. A source that is an <see cref="ISetVariablesSource"/>
    /// as well takes the writes of a SetVariables request, all of the request's
    /// at once.
    /// </remarks>
    public interface IOCPPServerControllerSource
    {

        /// <summary>
        /// The variables of OCPPServerController this source answers.
        /// </summary>
        IEnumerable<String>  Variables           { get; }

        /// <summary>
        /// The variables of OCPPServerController this source writes. None,
        /// unless it says so.
        /// </summary>
        IEnumerable<String>  WritableVariables
            => [];

        /// <summary>
        /// The value of a variable, as OCPPServerController spells it; null
        /// where the source has none.
        /// </summary>
        /// <param name="Variable">The name of the variable, as the controller spells it.</param>
        String? Value(String Variable);

    }


    /// <summary>
    /// The OCPP server of a node that charging stations - and local controllers -
    /// below it connect to: a local controller's station port, or a gateway's.
    /// It has no instance.
    /// </summary>
    /// <remarks>
    /// <para>
    /// OCPP 2.1 has no component for this: a charging station is a client and
    /// has no stations below it. So this is a controller of its own, named as
    /// the controllers of the white papers are, and its variables are the
    /// section "webSocketServer" of the node's configuration file, its OCPP over SSH
    /// and its logging, as the node's page shows them - besides what the server
    /// is doing, read-only.
    /// </para>
    /// <para>
    /// A change of what the server listens with - its address, port, OCPP
    /// versions, TLS version, connections, pings, revocation checks, and the
    /// OCPP versions, message size and pings over SSH - is written down at once
    /// and takes effect at the next start: such a variable is answered
    /// "RebootRequired", and WaitingForARestart names it until then. Everything
    /// else takes effect at once.
    /// </para>
    /// </remarks>
    public class OCPPServerController : ALogicalComponentConfig
    {

        #region Data

        /// <summary>
        /// The variables of the controller, in the order of the node's page.
        /// </summary>
        internal static readonly ControllerVariable[] Variables = [

            new ("Enabled",                     DataTypes.Boolean,       "Whether the node listens for charging stations at all - and for a CSMS that dials it on the same port. Takes effect at once.",                                                                                       Writable: true),
            new ("Address",                     DataTypes.String,        "The IP address of the interface it listens on; empty while it listens on every one. Takes effect at the next start.",                                                      MaxLimit: 45,                        Writable: true),
            new ("Port",                        DataTypes.Integer,       "The TCP port it listens on - and, for HTTP/3, the UDP port of the same number. Takes effect at the next start.",                                                           MinLimit: 1, MaxLimit: 65535,        Writable: true),
            new ("SecurityProfiles",            DataTypes.MemberList,    "The OCPP security profiles a charging station may connect with. Takes effect at once; 2 and 3 alone need a server certificate or a server key.",                          ValuesList: [ "1", "2", "3" ],       Writable: true),
            new ("Subprotocols",                DataTypes.MemberList,    "The OCPP versions offered, as WebSocket subprotocols, in the order they are preferred. Takes effect at the next start.",                                                   ValuesList: [ "ocpp2.1", "ocpp2.0.1", "ocpp1.6" ], Writable: true),
            new ("ReachableAs",                 DataTypes.String,        "The names and addresses the charging stations reach the node under, comma separated: what its server certificate has to be valid for. Takes effect at once.",            MaxLimit: 8000, MaxItems: 32,        Writable: true, MayBeEmpty: true),
            new ("MinimumTLSVersion",           DataTypes.OptionList,    "The oldest version of TLS accepted. Takes effect at the next start.",                                                                                                      ValuesList: [ "1.2", "1.3" ],        Writable: true),
            new ("CheckCertificateRevocation",  DataTypes.Boolean,       "Whether a charging station's certificate is checked against its issuer's revocation list. Takes effect at the next start.",                                                                                       Writable: true),
            new ("MaxConnections",              DataTypes.Integer,       "How many charging stations may be connected at once. Takes effect at the next start.",                                                                                     MinLimit: 1, MaxLimit: 10000,        Writable: true),
            new ("PingEvery",                   DataTypes.Integer,       "How often a silent connection is pinged, which is how a station that went away is noticed. Takes effect at the next start.",                                              MinLimit: 5, MaxLimit: 3600, InSeconds: true, Writable: true),
            new ("HTTP3",                       DataTypes.Boolean,       "Whether WebSockets are taken over HTTP/3 as well, on the UDP port of the same number - over TLS only. Takes effect at once.",                                                                                   Writable: true),

            new ("SSHEnabled",                  DataTypes.Boolean,       "Whether the charging stations may connect over SSH as well. Takes effect at once.",                                                                                                                            Writable: true),
            new ("SSHSubprotocols",             DataTypes.MemberList,    "The OCPP versions offered over SSH; empty while those of the WebSocket port are. Takes effect at the next start.",                                                        ValuesList: [ "ocpp2.1", "ocpp2.0.1", "ocpp1.6" ], Writable: true),
            new ("SSHMaxMessageSize",           DataTypes.Integer,       "The largest OCPP message taken and sent over SSH, in bytes. Takes effect at the next start.",                                                                              MinLimit: 65536, MaxLimit: 67108864, Writable: true),
            new ("SSHPingEvery",                DataTypes.Integer,       "How often a connection over SSH is pinged. Takes effect at the next start.",                                                                                               MinLimit: 5, MaxLimit: 3600, InSeconds: true, Writable: true),

            new ("LogConnections",              DataTypes.Boolean,       "Whether charging stations connecting and disconnecting are logged. Takes effect at once.",                                                                                                                     Writable: true),
            new ("LogAuthentication",           DataTypes.Boolean,       "Whether refused sign-ins and failed TLS handshakes are logged. Takes effect at once.",                                                                                                                         Writable: true),
            new ("LogMessages",                 DataTypes.Boolean,       "Whether every OCPP message is logged, by name and identification. Takes effect at once.",                                                                                                                      Writable: true),
            new ("LogPayloads",                 DataTypes.Boolean,       "Whether the contents of those messages are logged as well. Switched on only together with LogPayloadsUntil, as they name charging cards and meter readings.",                                                 Writable: true),
            new ("LogPayloadsUntil",            DataTypes.DateTime,      "When the contents stop being logged again; empty while there is no such window. Switching LogPayloads off ends one.",                                                                                        Writable: true),
            new ("LogPings",                    DataTypes.Boolean,       "Whether the WebSocket pings that keep a connection open are logged. Takes effect at once.",                                                                                                                    Writable: true),

            new ("Running",                     DataTypes.Boolean,       "Whether the server is listening now."),
            new ("TLS",                         DataTypes.Boolean,       "Whether it speaks TLS - decided when it starts."),
            new ("URL",                         DataTypes.String,        "The URL the charging stations connect to."),
            new ("Connections",                 DataTypes.Integer,       "How many connections it has now."),
            new ("WaitingForARestart",          DataTypes.String,        "The variables changed since the server started that take effect at the next start, comma separated; empty when there are none."),
            new ("SSHURL",                      DataTypes.String,        "The URL the charging stations connect to over SSH; empty where they do not."),
            new ("SSHConnections",              DataTypes.Integer,       "How many connections over SSH it has now.")

        ];

        /// <summary>
        /// The names of the variables of the controller.
        /// </summary>
        public static IEnumerable<String> VariableNames
            => Variables.Select(variable => variable.Name);

        /// <summary>
        /// The names of the variables a CSMS may write.
        /// </summary>
        public static IEnumerable<String> WritableVariableNames
            => Variables.Where(variable => variable.Writable).Select(variable => variable.Name);

        /// <summary>
        /// The variables whose change takes effect at the next start, by the
        /// name the node's configuration file and its page give the field.
        /// </summary>
        public static readonly IReadOnlyDictionary<String, String> RestartFields = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase) {
            { "Address",                     "address"                    },
            { "Port",                        "port"                       },
            { "Subprotocols",                "subprotocols"               },
            { "MinimumTLSVersion",           "minTLSVersion"              },
            { "CheckCertificateRevocation",  "checkCertificateRevocation" },
            { "MaxConnections",              "maxConnections"             },
            { "PingEvery",                   "pingEverySeconds"           },
            { "SSHSubprotocols",             "ssh.subprotocols"           },
            { "SSHMaxMessageSize",           "ssh.maxMessageSize"         },
            { "SSHPingEvery",                "ssh.pingEverySeconds"       }
        };

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The OCPP server controller of the given source.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        /// <param name="Description">An optional multi-language description of the controller.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public OCPPServerController(IOCPPServerControllerSource  Source,
                                    I18NString?                  Description   = null,
                                    CustomData?                  CustomData    = null)

            : base(nameof(OCPPServerController),
                   null,
                   Description ?? I18NString.Create("The OCPP server the charging stations below this node connect to."),
                   CustomData)

        {

            // Written only by a source that takes a change: every write goes to
            // it with the others of its request, since what one variable may be
            // depends on the others - security profiles 2 and 3 alone on a port
            // without a certificate, the contents of messages logged without a
            // window.
            var setter    = Source as ISetVariablesSource;
            var answered  = new HashSet<String>(Source.Variables, StringComparer.OrdinalIgnoreCase);
            var written   = new HashSet<String>(setter is not null ? Source.WritableVariables : [], StringComparer.OrdinalIgnoreCase);

            // Each may be monitored: a value changes on the page and in the
            // file as well, without the CSMS asking.
            foreach (var variable in Variables.Where(variable => answered.Contains(variable.Name)))
                variableConfigs.Add(variable.ToVariableConfig(() => Source.Value(variable.Name),
                                                              written.Contains(variable.Name),
                                                              Monitored: true));

            if (written.Count > 0)
                SetVariablesSource = setter;

        }

        #endregion


        #region (static) TryCheck(Variable, Value, out Refusal)

        /// <summary>
        /// Whether the given variable can be set to the given value by its type,
        /// its limits and its values alone - or why not, as a SetVariables result
        /// says it. Whether the value fits the rest of the configuration is for
        /// the source to ask.
        /// </summary>
        /// <param name="Variable">The name of the variable, in any case.</param>
        /// <param name="Value">The value as it was sent.</param>
        /// <param name="Refusal">Why the value cannot be set.</param>
        public static Boolean TryCheck(String                                Variable,
                                       String?                               Value,
                                       [NotNullWhen(false)] out StatusInfo?  Refusal)
        {

            var variable = Variables.FirstOrDefault(variable => String.Equals(variable.Name, Variable, StringComparison.OrdinalIgnoreCase));

            if (variable is null)
            {
                Refusal = new StatusInfo("NotFound", $"Unknown variable '{Variable}' of component '{nameof(OCPPServerController)}'!");
                return false;
            }

            var where = $"{nameof(OCPPServerController)}.{variable.Name}";

            if (!variable.Writable)
            {
                Refusal = new StatusInfo("ReadOnly", $"{where} is read-only!");
                return false;
            }

            // What is empty and may not be: a port listens somewhere, offers
            // some version and some profile - the String type alone would let
            // an empty one through.
            if (String.IsNullOrWhiteSpace(Value) && !variable.MayBeEmpty)
            {
                Refusal = new StatusInfo("InvalidValue", $"'{where}' must not be empty!");
                return false;
            }

            if (variable.DataType == DataTypes.DateTime)
            {

                Refusal = null;

                if (!String.IsNullOrWhiteSpace(Value) &&
                    !DateTimeOffset.TryParse(Value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out _))
                {
                    Refusal = new StatusInfo("InvalidValue", $"'{where}' is a date and time as RFC 3339 writes it, not \"{Value}\"!");
                }

                return Refusal is null;

            }

            return variable.TryCheck(Value, where, out Refusal);

        }

        #endregion

    }

}
