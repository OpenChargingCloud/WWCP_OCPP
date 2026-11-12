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
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Where TLSClientController reads how a charging station believes the TLS servers of its
    /// connections, and which key it presents to them.
    /// </summary>
    /// <remarks>
    /// The controller keeps no value of its own: whatever holds the configuration answers each read, so
    /// that a change made elsewhere - a pin set taken, a key learned on first use, a handshake made - is
    /// what the next read sees. A source that is an <see cref="ISetVariablesSource"/> as well takes the
    /// writes of a SetVariables request, all of the request's at once, and with the request: a pin set is
    /// taken only where a signer the station trusts signed it, and only with a higher version (white
    /// paper "Key Pins and TLS 1.3 Raw Public Keys for Local Controllers", section 5.7).
    /// </remarks>
    public interface ITLSClientControllerSource
    {

        /// <summary>
        /// The station's connections over TLS (wss://), by the names that are the instances of the controller:
        /// the connection ids of the station, as SSHClientController names its instances.
        /// </summary>
        IEnumerable<String>  Connections                   { get; }

        /// <summary>
        /// The variables of a connection this source answers.
        /// </summary>
        IEnumerable<String>  ConnectionVariables           { get; }

        /// <summary>
        /// The variables of a connection this source writes.
        /// </summary>
        IEnumerable<String>  WritableConnectionVariables   { get; }

        /// <summary>
        /// The variables of the station's own TLS client key this source answers: "ClientKeyPin".
        /// </summary>
        IEnumerable<String>  KeyVariables                  { get; }

        /// <summary>
        /// The value of a variable of a connection, in the words of the white paper; null for none.
        /// </summary>
        /// <param name="Connection">The connection: an instance of the controller.</param>
        /// <param name="Variable">The variable, as the paper spells it.</param>
        String? ConnectionValue(String Connection, String Variable);

        /// <summary>
        /// The value of a variable of the station's own TLS client key; null for none.
        /// </summary>
        /// <param name="Variable">The variable, as the paper spells it.</param>
        String? KeyValue(String Variable);

    }


    /// <summary>
    /// The TLS client controller of the white paper "Key Pins and TLS 1.3 Raw Public Keys for Local
    /// Controllers" (sections 5.5 to 5.7, 7.1 and 7.2): the instance named after a connection holds the
    /// key pins of its server and how they are checked; the instance without a name holds the key pin of
    /// the station's own TLS client key.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The paper puts the variables of a connection into NetworkConfiguration, one instance per network
    /// profile slot. A station of this kind has no slots: its connections have ids of their own, as on the
    /// SSH path, so they become the instances of a controller of their own, as SSHClientController's do.
    /// URL tells a CSMS which endpoint an instance is.
    /// </para>
    /// <para>
    /// The paper's EVQICtrlr[TLSClient].KeyPin is ClientKeyPin of the instance without a name here: under
    /// the station's own component, read-only, so that a CSMS can authorize the station's key at its
    /// Local Controller (section 5.8).
    /// </para>
    /// </remarks>
    public class TLSClientController : ALogicalComponentConfig
    {

        #region Data

        /// <summary>
        /// The variables of a connection, in the order of the paper, and those this kind adds.
        /// </summary>
        internal static readonly ControllerVariable[] ConnectionVariables = [

            new ("ServerKeyPins",         DataTypes.String,      "Comma separated key pins - SHA-256 over the SubjectPublicKeyInfo, 64 hexadecimal digits, with or without colons - of the keys the server of this connection may present. Set, the connection is key-pinned: chain, name and validity are not checked. Taken only from a signed pin set, with a higher ServerKeyPinsVersion.", MaxItems: KeyPinPolicy.MaxPins, Writable: true, MayBeEmpty: true),
            new ("ServerKeyPinsVersion",  DataTypes.Integer,     "The version of the signed pin set the pins came from: a pin set is taken only with a higher one.",                                                                       MinLimit: 0,                                       Writable: true),
            new ("ServerKeyPinState",     DataTypes.OptionList,  "Where the pins come from: configured, learned on first use and on probation, or learned and confirmed by a signed pin set.",                                  ValuesList: [ "Configured", "Learned", "Confirmed" ]),
            new ("RawPublicKeys",         DataTypes.OptionList,  "Whether the connection offers raw public keys (RFC 7250); only with ServerKeyPins or TrustOnFirstUse = Key.",                                                ValuesList: [ "Off", "Prefer", "Require" ],         Writable: true),
            new ("TrustOnFirstUse",       DataTypes.OptionList,  "Whether a connection that pins nothing learns the key of the first handshake, on probation.",                                                               ValuesList: [ "None", "Key" ],                      Writable: true),
            new ("ProbationTime",         DataTypes.Integer,     "How long a learned key pin waits for a signed pin set that confirms it.",                                                                                   MinLimit: 1, MaxLimit: 604800, InSeconds: true,     Writable: true),
            new ("LearnedKeyPin",         DataTypes.String,      "The key pin learned on first use while it is on probation; empty otherwise."),
            new ("KeyPinShown",           DataTypes.String,      "The key pin of the key the server of this connection presented at its last handshake."),
            new ("URL",                   DataTypes.String,      "The URL of the server of this connection.")

        ];

        /// <summary>
        /// The variables of the station's own TLS client key.
        /// </summary>
        internal static readonly ControllerVariable[] KeyVariables = [

            new ("ClientKeyPin",          DataTypes.String,      "The key pin of the TLS client key this station presents, raw or in its certificate: for its Local Controller's list of station keys.")

        ];

        /// <summary>
        /// The variables whose value is a list of key pins, each checked by its syntax.
        /// </summary>
        private static readonly HashSet<String> keyPinLists = new (StringComparer.OrdinalIgnoreCase) { "ServerKeyPins" };

        #endregion

        #region Properties

        /// <summary>
        /// The connection this instance is of; null for the station's own key.
        /// </summary>
        public String?  Connection    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The instance of the station's own TLS client key.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        public TLSClientController(ITLSClientControllerSource Source)

            : base(nameof(TLSClientController),
                   null,
                   I18NString.Create("The TLS client key this charging station presents to its servers."),
                   null)

        {

            var answered = new HashSet<String>(Source.KeyVariables, StringComparer.OrdinalIgnoreCase);

            // Read-only, whatever the source: the paper lets no CSMS write a station's key.
            foreach (var variable in KeyVariables.Where(variable => answered.Contains(variable.Name)))
                variableConfigs.Add(variable.ToVariableConfig(() => Source.KeyValue(variable.Name),
                                                              false,
                                                              Monitored: true));

        }

        /// <summary>
        /// The instance of one connection over TLS.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        /// <param name="Connection">The connection; the instance.</param>
        public TLSClientController(ITLSClientControllerSource  Source,
                                   String                      Connection)

            : base(nameof(TLSClientController),
                   Connection,
                   I18NString.Create($"How this charging station believes the TLS server of its connection '{Connection}'."),
                   null)

        {

            if (!Source.Connections.Contains(Connection, StringComparer.Ordinal))
                throw new ArgumentException($"The source has no connection '{Connection}'!", nameof(Connection));

            this.Connection = Connection;

            var setter    = Source as ISetVariablesSource;
            var answered  = new HashSet<String>(Source.ConnectionVariables, StringComparer.OrdinalIgnoreCase);
            var written   = new HashSet<String>(setter is not null ? Source.WritableConnectionVariables : [], StringComparer.OrdinalIgnoreCase);

            // Each may be monitored: the pins change by a signed pin set (a Delta monitor
            // on ServerKeyPins tells a CSMS when a station has the next pin, section 5.5),
            // the learned and the shown key at a handshake.
            foreach (var variable in ConnectionVariables.Where(variable => answered.Contains(variable.Name)))
                variableConfigs.Add(variable.ToVariableConfig(() => Source.ConnectionValue(Connection, variable.Name),
                                                              written.Contains(variable.Name),
                                                              Monitored: true));

            if (written.Count > 0)
                SetVariablesSource = setter;

        }

        #endregion


        #region (static) AllOf(Source)

        /// <summary>
        /// The instance of the station's own key, and one for each connection of the given source.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        public static IEnumerable<TLSClientController> AllOf(ITLSClientControllerSource Source)

            => [ new TLSClientController(Source),
                 .. Source.Connections.Select(connection => new TLSClientController(Source, connection)) ];

        #endregion

        #region (static) TryCheck(Connection, Variable, Value, out Refusal)

        /// <summary>
        /// Whether the given variable of a connection - or of the station's own key, where the connection is
        /// null - can be set to the given value by its type, its limits, its values and, for key pins, their
        /// syntax alone; or why not, as a SetVariables result says it.
        /// </summary>
        /// <remarks>
        /// A variable the paper does not let a CSMS write is refused as "ReadOnly", as the Device Model refuses
        /// it. Whether the value fits the rest of the configuration - a version higher than the one held, a
        /// signer the station trusts, RawPublicKeys only with pins - is for the source that puts it into effect.
        /// A key pin is read as Hermod's <see cref="KeyPin.TryParse"/> reads it: 64 hexadecimal digits, with or
        /// without colons between the bytes, or curl's "sha256//" and base64 (section 5.1).
        /// </remarks>
        /// <param name="Connection">The connection, the instance; null for the station's own key.</param>
        /// <param name="Variable">The name of the variable, in any case.</param>
        /// <param name="Value">The value as it was sent.</param>
        /// <param name="Refusal">Why the value cannot be set.</param>
        public static Boolean TryCheck(String?                               Connection,
                                       String                                Variable,
                                       String?                               Value,
                                       [NotNullWhen(false)] out StatusInfo?  Refusal)
        {

            var variable = (Connection is null ? KeyVariables : ConnectionVariables).
                               FirstOrDefault(variable => String.Equals(variable.Name, Variable, StringComparison.OrdinalIgnoreCase));

            if (variable is null)
            {
                Refusal = new StatusInfo("NotFound", $"Unknown variable '{Variable}' of component '{nameof(TLSClientController)}{(Connection is not null ? $"[{Connection}]" : "")}'!");
                return false;
            }

            var where = $"{nameof(TLSClientController)}{(Connection is not null ? $"[{Connection}]" : "")}.{variable.Name}";

            if (!variable.Writable)
            {
                Refusal = new StatusInfo("ReadOnly", $"{where} is read-only!");
                return false;
            }

            if (!variable.TryCheck(Value, where, out Refusal))
                return false;

            if (keyPinLists.Contains(variable.Name))
                return TryCheckKeyPins(Value, where, out Refusal);

            return true;

        }

        #endregion

        #region (internal static) TryCheckKeyPins(Value, Where, out Refusal)

        /// <summary>
        /// Whether every entry of the given comma separated list is a key pin, or why not. An empty list is one:
        /// no pins.
        /// </summary>
        /// <param name="Value">The value as it was sent.</param>
        /// <param name="Where">The variable as a sentence names it.</param>
        /// <param name="Refusal">Why the value is no list of key pins.</param>
        internal static Boolean TryCheckKeyPins(String?                               Value,
                                                String                                Where,
                                                [NotNullWhen(false)] out StatusInfo?  Refusal)
        {

            Refusal = null;

            if (String.IsNullOrWhiteSpace(Value))
                return true;

            foreach (var entry in Value.Split(',', StringSplitOptions.TrimEntries))
            {

                if (entry.Length == 0)
                {
                    Refusal = new StatusInfo("InvalidValue", $"'{Where}' has an empty entry: key pins are separated by single commas!");
                    return false;
                }

                if (!KeyPin.TryParse(entry, out _))
                {
                    Refusal = new StatusInfo("InvalidValue", $"'{Where}': \"{entry}\" is no key pin - 64 hexadecimal digits, with or without colons between the bytes!");
                    return false;
                }

            }

            return true;

        }

        #endregion

    }

}
