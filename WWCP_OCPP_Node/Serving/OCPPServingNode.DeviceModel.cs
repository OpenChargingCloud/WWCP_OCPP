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

using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The server the charging stations connect to, as the device model of the
    /// node's OCPP 2.1 node shows it to its CSMS: OCPPServerController, read and
    /// written as the node's page reads and writes it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// What may read and write it is not decided here: a kind says which resource
    /// of its role model the component is - "stations" of a local controller -
    /// as it does for its other components.
    /// </para>
    /// <para>
    /// Nothing keeps a CSMS from switching off the port it came in on, or taking
    /// away the profile or HTTP/3 it came in with: it may. What switches the
    /// server, HTTP/3 or OCPP over SSH on or off waits for the answer to be out,
    /// so that the CSMS learns that its change was taken before the connection
    /// it asked over may go.
    /// </para>
    /// </remarks>
    public abstract partial class OCPPServingNode
    {

        #region AttachOCPPServerController(Node)

        /// <summary>
        /// Give the given OCPP 2.1 node OCPPServerController: the server the
        /// charging stations connect to, for its CSMS to read and to write.
        /// </summary>
        /// <param name="Node">The OCPP 2.1 node of this node.</param>
        public void AttachOCPPServerController(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode Node)
        {

            if (Node is not OCPPv2_1.IDeviceModelNode deviceModel)
                return;

            deviceModel.RemoveComponent(nameof(OCPPv2_1.OCPPServerController), null);

            if (!deviceModel.AddComponent(new OCPPv2_1.OCPPServerController(new OCPPServerControllerSource(this))))
                Log.Warning($"The OCPP node '{Node.Id}' did not take {nameof(OCPPv2_1.OCPPServerController)}: the CSMS cannot read the charging station server.",
                            "ocpp", "station", "config");

            // When the answer to a SetVariables request is out, which what it
            // switches off waits for - told here as well, should the node not
            // have attached its TLS client: told twice, it is the same.
            Node.OCPP.OUT.OnSetVariablesResponseSent += (timestamp, sender, connection, request, response, runtime, sentMessageResult, cancellationToken) => {
                OCPPDialling.responsesSent.Sent(request);
                return Task.CompletedTask;
            };

        }

        #endregion


        #region (private class) OCPPServerControllerSource

        /// <summary>
        /// What OCPPServerController reads and writes of a node: what the node's
        /// page reads (<see cref="OCPPServerConfigurationJSON"/>), and every
        /// change of a request at once through what the page changes it with
        /// (<see cref="TryUpdateOCPPServerConfiguration(JObject, out String?, out Boolean, Task?)"/>).
        /// </summary>
        /// <param name="Node">The node.</param>
        private sealed class OCPPServerControllerSource(OCPPServingNode Node) : OCPPv2_1.IOCPPServerControllerSource,
                                                                                OCPPv2_1.ISetVariablesSource
        {

            #region IOCPPServerControllerSource

            public IEnumerable<String> Variables

                => OCPPv2_1.OCPPServerController.VariableNames;

            public IEnumerable<String> WritableVariables

                => OCPPv2_1.OCPPServerController.WritableVariableNames;

            public String? Value(String Variable)
            {

                var json   = Node.OCPPServerConfigurationJSON();
                var ssh    = json["ssh"]     as JObject;
                var log    = json["logging"] as JObject;
                var state  = json["state"]   as JObject;

                return Variable switch {

                           "Enabled"                     => Bool   (json ["enabled"]),
                           "Address"                     => Text   (json ["address"]),
                           "Port"                        => Number (json ["port"]),
                           "SecurityProfiles"            => List   (json ["securityProfiles"]),
                           "Subprotocols"                => List   (json ["subprotocols"]),
                           "ReachableAs"                 => List   (json ["reachableAs"]),
                           "MinimumTLSVersion"           => Text   (json ["minTLSVersion"]),
                           "CheckCertificateRevocation"  => Bool   (json ["checkCertificateRevocation"]),
                           "MaxConnections"              => Number (json ["maxConnections"]),
                           "PingEvery"                   => Number (json ["pingEverySeconds"]),
                           "HTTP3"                       => Bool   (json ["http3"]),

                           "SSHEnabled"                  => Bool   (ssh?["enabled"]),
                           "SSHSubprotocols"             => List   (ssh?["subprotocols"]),
                           "SSHMaxMessageSize"           => Number (ssh?["maxMessageSize"]),
                           "SSHPingEvery"                => Number (ssh?["pingEverySeconds"]),

                           "LogConnections"              => Bool   (log?["connections"]),
                           "LogAuthentication"           => Bool   (log?["authentication"]),
                           "LogMessages"                 => Bool   (log?["messages"]),
                           "LogPayloads"                 => Bool   (log?["payloads"]),
                           "LogPayloadsUntil"            => Text   (log?["payloadsUntil"]),
                           "LogPings"                    => Bool   (log?["pings"]),

                           "Running"                     => Bool   (state?["running"]),
                           "TLS"                         => Bool   (state?["tls"]),
                           "URL"                         => Text   (state?["url"]),
                           "Connections"                 => Number (state?["connections"]),
                           "WaitingForARestart"          => String.Join(",", WaitingFor(state?["waitingForARestart"])),
                           "SSHURL"                      => Text   (ssh?["state"]?["url"]),
                           "SSHConnections"              => Number (ssh?["state"]?["connections"]),

                           _                             => null

                       };

            }

            #endregion

            #region ISetVariablesSource

            public async Task<IReadOnlyList<OCPPv2_1.SetVariableResult>> SetVariablesAsync(IReadOnlyList<OCPPv2_1.SetVariableEntry>  Entries,
                                                                                           OCPPv2_1.DataConsistencyModel             DataConsistencyModel,
                                                                                           CancellationToken                         CancellationToken)
            {

                var results = new OCPPv2_1.SetVariableResult[Entries.Count];

                OCPPv2_1.SetVariableResult Result(Int32 Index, OCPPv2_1.SetVariableStatus Status, String? ReasonCode = null, String? AdditionalInfo = null)

                    => results[Index] = new (
                                            AttributeStatus:      Status,
                                            Component:            Entries[Index].Data.Component,
                                            Variable:             Entries[Index].Data.Variable,
                                            AttributeType:        Entries[Index].Variable.Attributes?.Type ?? OCPPv2_1.AttributeTypes.Actual,
                                            AttributeStatusInfo:  ReasonCode is not null ? new OCPPv2_1.StatusInfo(ReasonCode, AdditionalInfo) : null
                                        );

                #region Each value by itself: its type, its limits, its values

                var taken = new List<Int32>();

                for (var index = 0; index < Entries.Count; index++)
                {

                    if (!OCPPv2_1.OCPPServerController.TryCheck(Entries[index].Variable.Name, Entries[index].Data.AttributeValue, out var refusal))
                        results[index] = new (OCPPv2_1.SetVariableStatus.Rejected, Entries[index].Data.Component, Entries[index].Data.Variable,
                                              Entries[index].Variable.Attributes?.Type ?? OCPPv2_1.AttributeTypes.Actual, refusal);

                    else
                        taken.Add(index);

                }

                if (taken.Count == 0)
                    return results;

                #endregion

                // Asked while the request is still being handled, and not once
                // the change is made: the response may be out by then.
                var answered = Node.OCPPDialling.responsesSent.Answered(Entries[taken[0]].Request);

                #region All of them at once, as one change of the section

                if (TrySet(taken, answered, out var error, out var notSaved))
                    TakenAs(taken);

                // One change of the request's at a time, where the request says
                // that each stands by itself: one value that cannot be had - a
                // profile of TLS alone on a port without a certificate - does not
                // keep the others out.
                else if (!notSaved &&
                         taken.Count > 1 &&
                         DataConsistencyModel == OCPPv2_1.DataConsistencyModel.IndependentRequests)
                {
                    foreach (var index in taken)
                    {
                        if (TrySet([ index ], answered, out var oneError, out var oneNotSaved))
                            TakenAs([ index ]);
                        else
                            Result(index, OCPPv2_1.SetVariableStatus.Rejected, oneNotSaved ? "InternalError" : "InvalidValue", oneError);
                    }
                }

                else
                    foreach (var index in taken)
                        Result(index, OCPPv2_1.SetVariableStatus.Rejected, notSaved ? "InternalError" : "InvalidValue", error);

                #endregion

                await Task.CompletedTask;

                return results;


                // Accepted, or RebootRequired where it takes effect at the next
                // start - by what the server is waiting for now, so that a value
                // set back to the one it runs with is in effect.
                void TakenAs(IEnumerable<Int32> Indices)
                {

                    var waiting = new HashSet<String>(WaitingFor(Node.OCPPServerConfigurationJSON()["state"]?["waitingForARestart"]), StringComparer.OrdinalIgnoreCase);

                    foreach (var index in Indices)
                        Result(index, waiting.Contains(Entries[index].Variable.Name)
                                          ? OCPPv2_1.SetVariableStatus.RebootRequired
                                          : OCPPv2_1.SetVariableStatus.Accepted);

                }

                // The given entries as one change of the section "ocppServer".
                Boolean TrySet(IReadOnlyList<Int32> Indices, Task Answered, out String? Error, out Boolean NotSaved)
                {

                    var update = new JObject();

                    foreach (var index in Indices)
                        Add(update, Entries[index].Variable.Name, Entries[index].Data.AttributeValue ?? "");

                    if (!Node.TryUpdateOCPPServerConfiguration(update, out Error, out NotSaved, Answered))
                        return false;

                    Node.Log.Notice($"Over OCPP, {String.Join(", ", Indices.Select(index => $"{nameof(OCPPv2_1.OCPPServerController)}.{Entries[index].Variable.Name}"))} " +
                                    $"{(Indices.Count == 1 ? "was" : "were")} set.",
                                    "ocpp", "station", "config");

                    return true;

                }

            }

            #endregion


            #region (private static) Add(Update, Variable, Value)

            /// <summary>
            /// The given variable, set to the given value, as the section
            /// "ocppServer" of the configuration file writes it.
            /// </summary>
            private static void Add(JObject  Update,
                                    String   Variable,
                                    String   Value)
            {

                switch (Variable)
                {

                    case "Enabled":                     Update["enabled"]                     = Boolean.Parse(Value);           break;
                    case "Address":                     Update["address"]                     = Value.Trim();                   break;
                    case "Port":                        Update["port"]                        = Int64.Parse(Value, CultureInfo.InvariantCulture); break;
                    case "SecurityProfiles":            Update["securityProfiles"]            = new JArray(Members(Value).Select(profile => Int32.Parse(profile, CultureInfo.InvariantCulture))); break;
                    case "Subprotocols":                Update["subprotocols"]                = new JArray(Members(Value));     break;
                    case "ReachableAs":                 Update["reachableAs"]                 = new JArray(Members(Value));     break;
                    case "MinimumTLSVersion":           Update["minTLSVersion"]               = Value.Trim();                   break;
                    case "CheckCertificateRevocation":  Update["checkCertificateRevocation"]  = Boolean.Parse(Value);           break;
                    case "MaxConnections":              Update["maxConnections"]              = Int64.Parse(Value, CultureInfo.InvariantCulture); break;
                    case "PingEvery":                   Update["pingEverySeconds"]            = Int64.Parse(Value, CultureInfo.InvariantCulture); break;
                    case "HTTP3":                       Update["http3"]                       = Boolean.Parse(Value);           break;

                    case "SSHEnabled":                  Part(Update, "ssh")    ["enabled"]          = Boolean.Parse(Value);       break;
                    case "SSHSubprotocols":             Part(Update, "ssh")    ["subprotocols"]     = new JArray(Members(Value)); break;
                    case "SSHMaxMessageSize":           Part(Update, "ssh")    ["maxMessageSize"]   = Int64.Parse(Value, CultureInfo.InvariantCulture); break;
                    case "SSHPingEvery":                Part(Update, "ssh")    ["pingEverySeconds"] = Int64.Parse(Value, CultureInfo.InvariantCulture); break;

                    case "LogConnections":              Part(Update, "logging")["connections"]      = Boolean.Parse(Value);       break;
                    case "LogAuthentication":           Part(Update, "logging")["authentication"]   = Boolean.Parse(Value);       break;
                    case "LogMessages":                 Part(Update, "logging")["messages"]         = Boolean.Parse(Value);       break;
                    case "LogPayloads":                 Part(Update, "logging")["payloads"]         = Boolean.Parse(Value);       break;
                    case "LogPings":                    Part(Update, "logging")["pings"]            = Boolean.Parse(Value);       break;
                    case "LogPayloadsUntil":            Part(Update, "logging")["payloadsUntil"]    = DateTimeOffset.Parse(Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime().ToString("o"); break;

                }

                static JObject Part(JObject Update, String Name)
                {

                    if (Update[Name] is not JObject part)
                        Update[Name] = part = new JObject();

                    return part;

                }

            }

            #endregion

            #region (private static) Members(Value)

            private static String[] Members(String Value)

                => Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            #endregion

            #region (private static) WaitingFor(Fields)

            /// <summary>
            /// The variables of the given fields of the configuration file that
            /// are waiting for a restart.
            /// </summary>
            private static IEnumerable<String> WaitingFor(JToken? Fields)
            {

                var fields = new HashSet<String>((Fields as JArray)?.Values<String>().OfType<String>() ?? [], StringComparer.OrdinalIgnoreCase);

                return OCPPv2_1.OCPPServerController.RestartFields.
                           Where (restart => fields.Contains(restart.Value)).
                           Select(restart => restart.Key);

            }

            #endregion

            #region (private static) Bool(Token) / Text(Token) / Number(Token) / List(Token)

            private static String Bool(JToken? Token)

                => Token?.Type == JTokenType.Boolean && Token.Value<Boolean>()
                       ? "true"
                       : "false";

            private static String Text(JToken? Token)

                => Token is JValue { Value: not null } value
                       ? Convert.ToString(value.Value, CultureInfo.InvariantCulture) ?? ""
                       : "";

            private static String Number(JToken? Token)

                => Token?.Type is JTokenType.Integer or JTokenType.Float
                       ? ((Int64) Math.Round(Token.Value<Double>())).ToString(CultureInfo.InvariantCulture)
                       : "";

            private static String List(JToken? Token)

                => Token is JArray array
                       ? String.Join(",", array.Select(Text))
                       : "";

            #endregion

        }

        #endregion

    }

}
