/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// The DNS client controller of the Secure DNS white paper (section 3.1.1):
    /// what applies to name resolution as a whole. It has no instance.
    /// </summary>
    /// <remarks>
    /// Its variables are those of the paper that the given source can answer,
    /// read from it at every read. A variable it cannot answer is left out, so
    /// that a CSMS asking for it learns it is unknown rather than reading an
    /// empty value as an answer.
    /// </remarks>
    public class DNSClientController : ALogicalComponentConfig
    {

        #region Data

        /// <summary>
        /// The variables of the paper, in its order.
        /// </summary>
        internal static readonly ControllerVariable[] Variables = [

            new ("Enabled",          DataTypes.Boolean,     "Whether names are resolved at all. Switched off, no name is resolved, the CSMS's included.",                                                   Writable: true),
            new ("QueryTimeout",     DataTypes.Decimal,     "How long one query may take.",                                                            MinLimit: 0.1m, MaxLimit: 120, InSeconds: true,  Writable: true),
            new ("MaxRetries",       DataTypes.Integer,     "How often an unanswered query is sent again.",                                            MinLimit: 0,    MaxLimit: 255,                   Writable: true),
            new ("RecursionDesired", DataTypes.OptionList,  "Whether queries ask the server to recurse: Set, Unset, or Default to leave each query as it is.", ValuesList: [ "Set", "Unset", "Default" ],  Writable: true),
            new ("UseCache",         DataTypes.Boolean,     "Whether answers are kept for their time to live.",                                                                                                             Writable: true),
            new ("DNSSECOK",         DataTypes.Boolean,     "Whether queries set the DNSSEC OK bit (RFC 3225).",                                                                                                            Writable: true),
            new ("FollowCNAMEs",     DataTypes.Boolean,     "Whether CNAME records are followed.",                                                                                                                          Writable: true),
            new ("MaxCNAMEFollows",  DataTypes.Integer,     "How many CNAME records are followed at most.",                                            MinLimit: 0,    MaxLimit: 255,                   Writable: true),
            new ("MaxServers",       DataTypes.Integer,     "The number of DNSServerController instances this charging station offers."),
            new ("UDPPayloadSize",   DataTypes.Integer,     "The EDNS(0) UDP payload size this charging station announces."),
            new ("ClientSubnet",     DataTypes.String,      "The EDNS Client Subnet sent with queries (RFC 7871), or empty for none."),
            new ("EncryptedOnly",    DataTypes.Boolean,     "Whether name servers over an unencrypted transport are refused.",                                                                                              Writable: true),
            new ("ServerSource",     DataTypes.OptionList,  "System: the name servers of the operating system or the network. Manual: those of DNSServerController.", ValuesList: [ "System", "Manual" ], Writable: true)

        ];

        #endregion

        #region (static) TryCheck(Variable, Value, out Refusal)

        /// <summary>
        /// Whether the given variable of the paper can be set to the given value
        /// by its type, its limits and its values alone - or why not, as a
        /// SetVariables result says it. Whether the value fits the rest of the
        /// configuration is for the source to ask.
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
                Refusal = new StatusInfo("NotFound", $"Unknown variable '{Variable}' of component '{nameof(DNSClientController)}'!");
                return false;
            }

            return variable.TryCheck(Value, $"{nameof(DNSClientController)}.{variable.Name}", out Refusal);

        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create the DNS client controller of the given source.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        /// <param name="Description">An optional multi-language description of the controller.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DNSClientController(IDNSControllerSource  Source,
                                   I18NString?           Description   = null,
                                   CustomData?           CustomData    = null)

            : base(nameof(DNSClientController),
                   null,
                   Description ?? I18NString.Create("How this charging station resolves names (Secure DNS)."),
                   CustomData)

        {

            // Written only by a source that takes a change: every write goes to
            // it with the others of its request, since what one variable may be
            // depends on the others.
            var setter   = Source as ISetVariablesSource;
            var answered = new HashSet<String>(Source.ClientVariables, StringComparer.OrdinalIgnoreCase);
            var written  = new HashSet<String>(setter is not null ? Source.WritableClientVariables : [], StringComparer.OrdinalIgnoreCase);

            foreach (var variable in Variables.Where(variable => answered.Contains(variable.Name)))
                variableConfigs.Add(
                    variable.ToVariableConfig(() => Source.ClientValue(variable.Name),
                                              written.Contains(variable.Name))
                );

            if (written.Count > 0)
                SetVariablesSource = setter;

        }

        #endregion

    }

}
