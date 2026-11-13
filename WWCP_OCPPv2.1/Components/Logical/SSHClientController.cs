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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Where SSHClientController reads how a charging station signs in over SSH and believes its servers.
    /// </summary>
    /// <remarks>
    /// The controller keeps no value of its own: whatever holds the configuration answers each read, so
    /// that a change made elsewhere - a pin set taken, a key learned on first use - is what the next read
    /// sees. A source that is an <see cref="ISetVariablesSource"/> as well takes the writes of a
    /// SetVariables request, all of the request's at once, and with the request: a pin set is taken only
    /// where a signer the station trusts signed it.
    /// </remarks>
    public interface ISSHClientControllerSource
    {

        /// <summary>
        /// The station's connections over SSH, by the names that are the instances of the controller.
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
        /// The purposes the station has an SSH key for: "Station", "NTS", "SFTP".
        /// </summary>
        IEnumerable<String>  Purposes                      { get; }

        /// <summary>
        /// The variables of the station's keys this source answers.
        /// </summary>
        IEnumerable<String>  KeyVariables                  { get; }

        /// <summary>
        /// The variables of the station's keys this source writes.
        /// </summary>
        IEnumerable<String>  WritableKeyVariables          { get; }

        /// <summary>
        /// The value of a variable of a connection, in the words of the white paper; null for none.
        /// </summary>
        String? ConnectionValue(String Connection, String Variable);

        /// <summary>
        /// The value of a variable of the station's keys: of one purpose, or - for ClientKeyAlgorithms - of
        /// none; null for none.
        /// </summary>
        String? KeyValue(String Variable, String? Purpose);

    }


    /// <summary>
    /// The SSH client controller of the white paper "OCPP over SSH" (section 9.2; decision 5): the
    /// instance named after a connection holds its host key pins and how they are checked; the instance
    /// without a name holds the station's own keys, one variable instance per purpose.
    /// </summary>
    public class SSHClientController : ALogicalComponentConfig
    {

        #region Data

        /// <summary>
        /// The variables of a connection, in the order of the paper.
        /// </summary>
        internal static readonly ControllerVariable[] ConnectionVariables = [

            new ("HostKeyPins",         DataTypes.String,      "Comma separated SHA-256 fingerprints, or public-key lines, of the host keys the server of this connection may show; taken only from a signed pin set.", MaxItems: 16, Writable: true, MayBeEmpty: true),
            new ("HostKeyPinsVersion",  DataTypes.Integer,     "The version of the signed pin set the pins came from: a pin set is taken only with a higher one.",                                                         MinLimit: 0,                         Writable: true),
            new ("TrustOnFirstUse",     DataTypes.OptionList,  "Whether a connection that pins nothing learns the host key of the first handshake, on probation.",                                                          ValuesList: [ "None", "HostKey" ],   Writable: true),
            new ("ProbationTime",       DataTypes.Integer,     "How long a learned host key waits for a signed pin set that confirms it.",                                                                                 MinLimit: 1, MaxLimit: 604800, InSeconds: true, Writable: true),
            new ("OnPinMismatch",       DataTypes.OptionList,  "What a host key that matches none of the pins leads to.",                                                                                                  ValuesList: [ "Refuse", "Record", "Accept" ], Writable: true),
            new ("HostKeyFingerprint",  DataTypes.String,      "The SHA-256 fingerprint of the host key the server showed at the last handshake of this connection."),
            new ("LearnedHostKey",      DataTypes.String,      "The SHA-256 fingerprint of the host key learned on first use while it is on probation; empty otherwise.")

        ];

        /// <summary>
        /// The variables of the station's keys, in the order of the paper.
        /// </summary>
        internal static readonly ControllerVariable[] KeyVariables = [

            new ("ClientPublicKey",         DataTypes.String,      "The public key the station signs in with for this purpose, as a line of authorized_keys."),
            new ("ClientPendingPublicKey",  DataTypes.String,      "The next key of this purpose during a rotation; empty otherwise."),
            new ("ClientCertificate",       DataTypes.String,      "The OpenSSH user certificate of this purpose's key, where the operator signs station keys."),
            new ("ClientKeyMaxAge",         DataTypes.Integer,     "The age in days after which the station rotates this key by itself; 0 only when asked.", MinLimit: 0, Writable: true),
            new ("ClientKeyAlgorithms",     DataTypes.MemberList,  "The key types the station can make.", ValuesList: [ "ssh-ed25519", "ecdsa-sha2-nistp256" ])

        ];

        #endregion

        #region Properties

        /// <summary>
        /// The connection this instance is of; null for the station's keys.
        /// </summary>
        public String?  Connection    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The instance of the station's keys: one variable instance per purpose.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        public SSHClientController(ISSHClientControllerSource Source)

            : base(nameof(SSHClientController),
                   null,
                   I18NString.Create("The SSH keys this charging station signs in with (OCPP over SSH)."),
                   null)

        {

            var setter    = Source as ISetVariablesSource;
            var answered  = new HashSet<String>(Source.KeyVariables, StringComparer.OrdinalIgnoreCase);
            var written   = new HashSet<String>(setter is not null ? Source.WritableKeyVariables : [], StringComparer.OrdinalIgnoreCase);

            foreach (var variable in KeyVariables.Where(variable => answered.Contains(variable.Name)))
            {

                if (variable.Name == "ClientKeyAlgorithms")
                {
                    variableConfigs.Add(variable.ToVariableConfig(() => Source.KeyValue(variable.Name, null), false));
                    continue;
                }

                foreach (var purpose in Source.Purposes)
                    variableConfigs.Add(variable.ToVariableConfig(() => Source.KeyValue(variable.Name, purpose),
                                                                  written.Contains(variable.Name),
                                                                  Instance:   purpose,
                                                                  Monitored:  true));

            }

            if (written.Count > 0)
                SetVariablesSource = setter;

        }

        /// <summary>
        /// The instance of one connection over SSH.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        /// <param name="Connection">The connection; the instance.</param>
        public SSHClientController(ISSHClientControllerSource  Source,
                                   String                      Connection)

            : base(nameof(SSHClientController),
                   Connection,
                   I18NString.Create($"How this charging station believes the server of its connection '{Connection}' over SSH."),
                   null)

        {

            if (!Source.Connections.Contains(Connection, StringComparer.Ordinal))
                throw new ArgumentException($"The source has no connection '{Connection}'!", nameof(Connection));

            this.Connection = Connection;

            var setter    = Source as ISetVariablesSource;
            var answered  = new HashSet<String>(Source.ConnectionVariables, StringComparer.OrdinalIgnoreCase);
            var written   = new HashSet<String>(setter is not null ? Source.WritableConnectionVariables : [], StringComparer.OrdinalIgnoreCase);

            // Each may be monitored: the pins change by a signed pin set, the
            // learned key at a first handshake.
            foreach (var variable in ConnectionVariables.Where(variable => answered.Contains(variable.Name)))
                variableConfigs.Add(variable.ToVariableConfig(() => Source.ConnectionValue(Connection, variable.Name),
                                                              written.Contains(variable.Name),
                                                              Monitored: true));

            if (written.Count > 0)
                SetVariablesSource = setter;

        }

        #endregion


        #region (static) TryCheck(Connection, Variable, Value, out Refusal)

        /// <summary>
        /// Whether the given variable of a connection can be set to the given value by its type, its limits
        /// and its values alone - or why not, as a SetVariables result says it.
        /// </summary>
        public static Boolean TryCheck(String                                                                    Connection,
                                       String                                                                    Variable,
                                       String?                                                                   Value,
                                       [System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out StatusInfo?     Refusal)
        {

            var variable = ConnectionVariables.Concat(KeyVariables).FirstOrDefault(variable => String.Equals(variable.Name, Variable, StringComparison.OrdinalIgnoreCase));

            if (variable is null)
            {
                Refusal = new StatusInfo("NotFound", $"Unknown variable '{Variable}' of component '{nameof(SSHClientController)}'!");
                return false;
            }

            return variable.TryCheck(Value, $"{nameof(SSHClientController)}[{Connection}].{variable.Name}", out Refusal);

        }

        #endregion

    }

}
