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
using System.Text.RegularExpressions;

using org.GraphDefined.Vanaheimr.Hermod;

using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.KeyPins
{

    /// <summary>
    /// Whether a signer the node believes signed the given SetVariables request for the given
    /// connection - see <see cref="PinSetSignatures.SignedBy"/>.
    /// </summary>
    /// <param name="Request">The request.</param>
    /// <param name="Endpoint">The connection whose variables it writes.</param>
    /// <param name="WithoutTimeWindow">Whether the signature's time is left out: for a pin set only.</param>
    /// <param name="Signer">Who signed it.</param>
    /// <param name="Why">Why it is not taken as signed.</param>
    public delegate Boolean PinSetSignatureCheck(OCPPv2_1.CSMS.SetVariablesRequest?  Request,
                                                 IPinnedTLSEndpoint                  Endpoint,
                                                 Boolean                             WithoutTimeWindow,
                                                 [NotNullWhen(true)]  out String?    Signer,
                                                 [NotNullWhen(false)] out String?    Why);


    /// <summary>
    /// What TLSClientController of the white paper "Key Pins and TLS 1.3 Raw Public Keys for Local
    /// Controllers" reads and writes of a node (sections 7.1 and 7.2): an instance for every wss://
    /// connection and one for the node's own key, and pin sets taken from a SetVariables request a signer
    /// the node believes signed (section 5.7).
    /// </summary>
    /// <remarks>
    /// The signers, the signature check - without a time window for a pin set, whose version keeps an
    /// older one from coming back - and the store and the probation behind it
    /// (<see cref="ServerKeyPinning"/>).
    /// </remarks>
    /// <param name="Pinning">The key pins of the node's connections.</param>
    /// <param name="ClientKeyPins">The key pins of the TLS client keys the node presents.</param>
    /// <param name="SignedBy">Whether a signer the node believes signed a request.</param>
    /// <param name="Responses">When the response to a SetVariables request is out.</param>
    /// <param name="NodeName">What the node is called in a sentence: "station", "local controller".</param>
    public sealed class TLSClientControllerSource(ServerKeyPinning           Pinning,
                                                  Func<IEnumerable<KeyPin>>  ClientKeyPins,
                                                  PinSetSignatureCheck       SignedBy,
                                                  ResponsesSent              Responses,
                                                  String                     NodeName   = "node") : OCPPv2_1.ITLSClientControllerSource,
                                                                                                    OCPPv2_1.ISetVariablesSource
    {

        #region Data

        /// <summary>
        /// The settings of how a connection believes its server beyond its pins, as the paper names them.
        /// </summary>
        private static readonly String[] trustVariables = [ "RawPublicKeys", "TrustOnFirstUse", "ProbationTime" ];

        private readonly Object componentsLock = new ();

        #endregion


        #region FollowComponents(Node, Warning)

        /// <summary>
        /// Give the given OCPP 2.1 node a TLSClientController for its own key and one for every
        /// connection over TLS there is now, and none for one there is not.
        /// </summary>
        /// <remarks>
        /// Called by whoever keeps the connections after every change it wrote, before it dials what
        /// changed, so that a connection dialled again has its instance by then. A failure is said in the
        /// log and kept here: the dialling told after this must not miss the change for it.
        /// </remarks>
        /// <param name="Node">The node, where there is one yet.</param>
        /// <param name="Warning">A sentence for the log.</param>
        public void FollowComponents(OCPPv2_1.IDeviceModelNode?  Node,
                                     Action<String>              Warning)
        {

            if (Node is null)
                return;

            try
            {

                lock (componentsLock)
                {

                    var connections  = Connections.ToArray();
                    var shown        = Node.GetComponentConfigs(nameof(OCPPv2_1.TLSClientController)).OfType<OCPPv2_1.TLSClientController>().ToArray();

                    foreach (var instance in shown)
                        if (instance.Connection is not null && !connections.Contains(instance.Connection, StringComparer.Ordinal))
                            Node.RemoveComponent(instance.Name, instance.Instance);

                    if (!shown.Any(instance => instance.Connection is null))
                        Node.AddComponent(new OCPPv2_1.TLSClientController(this));

                    foreach (var connection in connections)
                        if (!shown.Any(instance => instance.Connection == connection))
                            Node.AddComponent(new OCPPv2_1.TLSClientController(this, connection));

                }

            }
            catch (Exception e)
            {
                Warning($"The TLSClientController instances could not follow the connections: {e.Message}");
            }

        }

        #endregion


        #region ITLSClientControllerSource

        public IEnumerable<String> Connections

            => [.. Pinning.Endpoints.Secure.Select(endpoint => endpoint.Id)];

        public IEnumerable<String> ConnectionVariables          => [ "ServerKeyPins", "ServerKeyPinsVersion", "ServerKeyPinState", "RawPublicKeys", "TrustOnFirstUse",
                                                                     "ProbationTime", "LearnedKeyPin", "KeyPinShown", "URL" ];
        public IEnumerable<String> WritableConnectionVariables  => [ "ServerKeyPins", "ServerKeyPinsVersion", "RawPublicKeys", "TrustOnFirstUse", "ProbationTime" ];
        public IEnumerable<String> KeyVariables                 => [ "ClientKeyPin" ];

        public String? ConnectionValue(String Connection, String Variable)
        {

            if (Pinning.Endpoints.CopyOf(Connection) is not IPinnedTLSEndpoint entry || !entry.IsSecure)
                return null;

            var trust    = Pinning.TrustOf(Connection);

            // One learned while it could not be written down is held by its
            // trust alone, and on probation all the same.
            var learned  = entry.ServerKeyPinLearned ?? trust?.Learned;

            return Variable switch {
                       "ServerKeyPins"         => String.Join(",", entry.ServerKeyPins.Select(pin => pin.ToHex())),
                       "ServerKeyPinsVersion"  => entry.ServerKeyPinsVersion.ToString(),
                       "ServerKeyPinState"     => (entry.ServerKeyPins.Count == 0 && learned is not null ? KeyPinState.Learned : entry.ServerKeyPinState).ToString(),
                       "RawPublicKeys"         => entry.RawPublicKeys.ToString(),
                       "TrustOnFirstUse"       => entry.TrustOnFirstUse.ToString(),
                       "ProbationTime"         => ((Int64) entry.ServerKeyPinProbationTime.TotalSeconds).ToString(),
                       "LearnedKeyPin"         => entry.ServerKeyPins.Count == 0 ? learned?.ToHex() ?? "" : "",
                       "KeyPinShown"           => trust?.Shown?.ToHex() ?? "",
                       "URL"                   => WithoutPassword(entry.URL.ToString()),
                       _                       => null
                   };

        }

        public String? KeyValue(String Variable)

            => Variable == "ClientKeyPin"
                   ? String.Join(",", ClientKeyPins().Select(pin => pin.ToHex()))
                   : null;

        /// <summary>
        /// A URL as a CSMS may read it: without a password somebody wrote into it.
        /// </summary>
        private static String WithoutPassword(String URL)

            => Regex.Replace(URL, "^([A-Za-z][A-Za-z0-9+.-]*://[^/@:]*):[^/@]*@", "$1@");

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

            foreach (var group in Enumerable.Range(0, Entries.Count).GroupBy(index => (Entries[index].Component as OCPPv2_1.TLSClientController)?.Connection))
            {

                #region The node's own key: read-only

                // The Device Model refuses these before they get here, as the
                // instance has no source to write them: said again, should one
                // come all the same.
                if (group.Key is null)
                {
                    foreach (var index in group)
                        Result(index, OCPPv2_1.SetVariableStatus.Rejected, "ReadOnly", $"TLSClientController.{Entries[index].Variable.Name} is read-only!");
                    continue;
                }

                #endregion

                #region A connection: only from a pin set a signer the node believes signed

                var connection = group.Key;

                if (Pinning.Endpoints.CopyOf(connection) is not IPinnedTLSEndpoint entry || !entry.IsSecure)
                {
                    foreach (var index in group)
                        Result(index, OCPPv2_1.SetVariableStatus.UnknownComponent, "NotFound", $"This {NodeName} has no wss:// connection '{connection}' any more.");
                    continue;
                }

                var request = Entries[group.First()].Request;

                if (!SignedBy(request, entry, WithoutTimeWindow: true, out var signer, out var why))
                {
                    foreach (var index in group)
                        Result(index, OCPPv2_1.SetVariableStatus.Rejected, "Unsigned", why);
                    continue;
                }

                var values = new Dictionary<String, (Int32 Index, String Value)>(StringComparer.OrdinalIgnoreCase);

                foreach (var index in group)
                {
                    if (!OCPPv2_1.TLSClientController.TryCheck(connection, Entries[index].Variable.Name, Entries[index].Data.AttributeValue, out var refusal))
                        results[index] = new (OCPPv2_1.SetVariableStatus.Rejected, Entries[index].Data.Component, Entries[index].Data.Variable,
                                              Entries[index].Variable.Attributes?.Type ?? OCPPv2_1.AttributeTypes.Actual, refusal);
                    else
                        values[Entries[index].Variable.Name] = (index, Entries[index].Data.AttributeValue);
                }

                #endregion

                #region The pin set: ServerKeyPins and ServerKeyPinsVersion, together

                var hasPins     = values.TryGetValue("ServerKeyPins",        out var pins);
                var hasVersion  = values.TryGetValue("ServerKeyPinsVersion", out var version);

                if (hasPins != hasVersion)
                {
                    foreach (var one in new[] { pins, version }.Where(one => one.Value is not null))
                        Result(one.Index, OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue",
                               "A pin set is ServerKeyPins and ServerKeyPinsVersion in one request: the version is what keeps an older one from coming back.");
                }

                else if (hasPins)
                {

                    if (!UInt64.TryParse(version.Value, out var versionNumber))
                    {
                        Result(version.Index, OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue", $"'{version.Value}' is no version.");
                        Result(pins.Index,    OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue", "The pin set has no version.");
                    }

                    // Responses.Answered is asked here, while the request is still
                    // being handled, and not once the pins are taken: the response
                    // may be out by then.
                    else if (!Pinning.TryTakeSignedServerKeyPins(connection, pins.Value, versionNumber, signer, out var refused, Responses.Answered(request)))
                    {
                        Result(pins.Index,    OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue", refused);
                        Result(version.Index, OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue", refused);
                    }

                    else
                    {
                        Result(pins.Index,    OCPPv2_1.SetVariableStatus.Accepted);
                        Result(version.Index, OCPPv2_1.SetVariableStatus.Accepted);
                    }

                }

                #endregion

                #region How it believes its server beyond its pins: within the time window

                var trustEntries = trustVariables.
                                       Where (name => values.ContainsKey(name)).
                                       Select(name => (Name: name, values[name].Index, values[name].Value)).
                                       ToArray();

                if (trustEntries.Length > 0)
                {

                    // No version keeps an older one of these from coming back,
                    // so the signature's time does, by the node's clock: a node
                    // without a set clock takes them with a pin set's help only -
                    // from the page, or once its clock is set.
                    if (!SignedBy(request, entry, WithoutTimeWindow: false, out _, out var notNow))
                    {
                        foreach (var one in trustEntries)
                            Result(one.Index, OCPPv2_1.SetVariableStatus.Rejected, "OutOfTime", notNow);
                        continue;
                    }

                    // .NET's SslStream offers no raw public key: a connection that
                    // requires them is not dialled, and a CSMS that asked for it
                    // from afar would have cut the node off from this server.
                    var require = trustEntries.Where(one => one.Name == "RawPublicKeys" &&
                                                            String.Equals(one.Value?.Trim(), nameof(RawPublicKeyMode.Require), StringComparison.OrdinalIgnoreCase)).
                                               ToArray();

                    foreach (var one in require)
                        Result(one.Index, OCPPv2_1.SetVariableStatus.Rejected, "NotSupported",
                               $"This {NodeName}'s TLS stack cannot offer raw public keys yet, and a connection that requires them would not be dialled: " +
                                "Prefer checks the same key pins in a certificate.");

                    var taken = trustEntries.Except(require).ToArray();

                    if (taken.Length == 0)
                        continue;

                    var settings = new ServerKeyPinSettings(taken.FirstOrDefault(one => one.Name == "RawPublicKeys").  Value,
                                                            taken.FirstOrDefault(one => one.Name == "TrustOnFirstUse").Value,
                                                            taken.FirstOrDefault(one => one.Name == "ProbationTime").  Value is String seconds && Int64.TryParse(seconds, out var s) ? s : null);

                    var changed = Pinning.TryChangeKeyPinTrust(connection, settings, signer, Responses.Answered(request), out var notChanged);

                    foreach (var one in taken)
                        if (changed)
                            Result(one.Index, OCPPv2_1.SetVariableStatus.Accepted);
                        else
                            Result(one.Index, OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue", notChanged);

                }

                #endregion

            }

            await Task.CompletedTask;

            return results;

        }

        #endregion

    }

}
