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
        internal static readonly DNSVariable[] Variables = [

            new ("Enabled",          DataTypes.Boolean,     "Whether names are resolved at all. Switched off, no name is resolved, the CSMS's included."),
            new ("QueryTimeout",     DataTypes.Decimal,     "How long one query may take.",                                                            MinLimit: 0.1m, MaxLimit: 120, InSeconds: true),
            new ("MaxRetries",       DataTypes.Integer,     "How often an unanswered query is sent again.",                                            MinLimit: 0,    MaxLimit: 255),
            new ("RecursionDesired", DataTypes.OptionList,  "Whether queries ask the server to recurse: Set, Unset, or Default to leave each query as it is.", ValuesList: [ "Set", "Unset", "Default" ]),
            new ("UseCache",         DataTypes.Boolean,     "Whether answers are kept for their time to live."),
            new ("DNSSECOK",         DataTypes.Boolean,     "Whether queries set the DNSSEC OK bit (RFC 3225)."),
            new ("FollowCNAMEs",     DataTypes.Boolean,     "Whether CNAME records are followed."),
            new ("MaxCNAMEFollows",  DataTypes.Integer,     "How many CNAME records are followed at most.",                                            MinLimit: 0,    MaxLimit: 255),
            new ("MaxServers",       DataTypes.Integer,     "The number of DNSServerController instances this charging station offers."),
            new ("UDPPayloadSize",   DataTypes.Integer,     "The EDNS(0) UDP payload size this charging station announces."),
            new ("ClientSubnet",     DataTypes.String,      "The EDNS Client Subnet sent with queries (RFC 7871), or empty for none."),
            new ("EncryptedOnly",    DataTypes.Boolean,     "Whether name servers over an unencrypted transport are refused."),
            new ("ServerSource",     DataTypes.OptionList,  "System: the name servers of the operating system or the network. Manual: those of DNSServerController.", ValuesList: [ "System", "Manual" ])

        ];

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

            var answered = new HashSet<String>(Source.ClientVariables, StringComparer.OrdinalIgnoreCase);

            foreach (var variable in Variables.Where(variable => answered.Contains(variable.Name)))
                variableConfigs.Add(
                    variable.ToVariableConfig(() => Source.ClientValue(variable.Name))
                );

        }

        #endregion

    }

}
