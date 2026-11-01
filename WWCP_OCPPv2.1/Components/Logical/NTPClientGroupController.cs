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
    /// An NTP client group controller of the Secure Time Synchronization white
    /// paper (section 3.2.3): one group of time servers, asked together in
    /// rounds, with its quorum, its schedule and its error handling. The
    /// instance is the group id.
    ///
    /// Well-known instances are:
    ///   - "legal" for legal time, which dynamic tariffs and everything else
    ///     calibration law rests on use
    ///   - "local" for a time consistent across the charging stations of a
    ///     location, e.g. from time servers on Local Controllers
    /// </summary>
    /// <remarks>
    /// Its variables are those of the paper that the given source can answer,
    /// read from it at every read, and Clients, read from the clients of the
    /// group. The variables of legal time exist in the legal group only.
    /// </remarks>
    public class NTPClientGroupController : ALogicalComponentConfig
    {

        #region Data

        /// <summary>
        /// The id of the group whose time is legal time.
        /// </summary>
        public const String LegalGroup = "legal";

        /// <summary>
        /// The variables of the paper, in its order.
        /// </summary>
        internal static readonly ControllerVariable[] Variables = [

            new ("Enabled",             DataTypes.Boolean,     "Whether the group is asked. A disabled group keeps its clients.",                                                                                   Writable: true),
            new ("Clients",             DataTypes.String,      "The ids of the NTPClientController instances of this group, comma separated.",                                                                     Writable: true),
            new ("MinServers",          DataTypes.Integer,     "The quorum: how many clients of one priority must answer in a round for its time to count.",                         MinLimit: 1,      MaxLimit: 255,                     Writable: true),
            new ("MaxDeviation",        DataTypes.Decimal,     "The spread between the answers of one round that is accepted silently; a larger one is logged.",                       MinLimit: 0.001m, MaxLimit: 3600,  InSeconds: true,  Writable: true),
            new ("MinInterval",         DataTypes.Integer,     "The shortest time between the start of two rounds.",                                                                  MinLimit: 10,     MaxLimit: 86400, InSeconds: true,  Writable: true, MayBeEmpty: true),
            new ("MaxInterval",         DataTypes.Integer,     "The longest time between the start of two rounds; each round is due at a random time between both.",                 MinLimit: 10,     MaxLimit: 86400, InSeconds: true,  Writable: true, MayBeEmpty: true),
            new ("Preflight",           DataTypes.Integer,     "After this long without a round, every client is first sent a preflight packet whose answer is ignored.",          MinLimit: 1,      MaxLimit: 86400, InSeconds: true,  Writable: true, MayBeEmpty: true),
            new ("Timeout",             DataTypes.Decimal,     "How long one exchange with a client may take.",                                                                       MinLimit: 0.1m,   MaxLimit: 3600,  InSeconds: true,  Writable: true, MayBeEmpty: true),
            new ("ErrorLogging",        DataTypes.Integer,     "The number of failed rounds in a row that leads to an entry in the log of the group.",                                MinLimit: 1,      MaxLimit: 65535,                   Writable: true),
            new ("MaxErrors",           DataTypes.Integer,     "The number of failed rounds in a row after which what rests on the time of the group is limited.",                   MinLimit: 1,      MaxLimit: 65535,                   Writable: true),
            new ("Limited",             DataTypes.Boolean,     "True from the moment MaxErrors is reached until a round produces a time again."),
            new ("ErrorsInARow",        DataTypes.Integer,     "The number of failed rounds in a row so far."),
            new ("LastCheck",           DataTypes.DateTime,    "When the last round that produced a time took place."),
            new ("LastOffset",          DataTypes.Decimal,     "The offset of the clock of this charging station against the time of that round.",                                                                 InSeconds: true),
            new ("LegalTimeAuthority",  DataTypes.String,      "Who stands behind the time of the group, e.g. \"PTB\". Without it the time is not legal time.",                                        MaxLimit: 80,                      Writable: true),
            new ("LegalTimeTolerance",  DataTypes.Decimal,     "How far the clock of this charging station may be from the time of the group and still be legal time.",              MinLimit: 0.001m, MaxLimit: 60,    InSeconds: true,  Writable: true),
            new ("LegalTimeMaxAge",     DataTypes.Integer,     "How old the last round that produced a time may be and still be legal time.",                                         MinLimit: 10,     MaxLimit: 86400, InSeconds: true,  Writable: true)

        ];

        /// <summary>
        /// The variables only the legal group has.
        /// </summary>
        public static readonly IReadOnlySet<String> LegalOnly = new HashSet<String>(StringComparer.OrdinalIgnoreCase) {
                                                                    "LegalTimeAuthority",
                                                                    "LegalTimeTolerance",
                                                                    "LegalTimeMaxAge"
                                                                };

        #endregion

        #region Properties

        /// <summary>
        /// The id of the group: the instance.
        /// </summary>
        public String   Group      { get; }

        /// <summary>
        /// Whether this is the group whose time is legal time.
        /// </summary>
        public Boolean  IsLegal    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create the NTP client group controller of the given group of the given source.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        /// <param name="Group">The id of the group; its instance.</param>
        /// <param name="Description">An optional multi-language description of the controller.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public NTPClientGroupController(INTPControllerSource  Source,
                                        String                Group,
                                        I18NString?           Description   = null,
                                        CustomData?           CustomData    = null)

            : base(nameof(NTPClientGroupController),
                   Group,
                   Description ?? I18NString.Create($"The group '{Group}' of time servers of this charging station (Secure Time Synchronization)."),
                   CustomData)

        {

            if (!Source.Groups.Contains(Group, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException($"The source has no group '{Group}'!", nameof(Group));

            this.Group    = Group;
            this.IsLegal  = String.Equals(Group, LegalGroup, StringComparison.OrdinalIgnoreCase);

            var answered  = new HashSet<String>(Source.GroupVariables, StringComparer.OrdinalIgnoreCase) {
                                "Clients"
                            };

            foreach (var variable in Variables.Where(variable => answered.Contains(variable.Name) &&
                                                                 (IsLegal || !LegalOnly.Contains(variable.Name))))
                variableConfigs.Add(
                    variable.ToVariableConfig(variable.Name == "Clients"
                                                  ? () => String.Join(",", Source.ClientsOf(Group))
                                                  : () => Source.GroupValue(Group, variable.Name),
                                              Writes: false)
                );

        }

        #endregion


        #region (static) AllOf(Source)

        /// <summary>
        /// The NTP client group controllers of every group of the given source.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        public static IEnumerable<NTPClientGroupController> AllOf(INTPControllerSource Source)

            => Source.Groups.
                      Distinct(StringComparer.OrdinalIgnoreCase).
                      Select  (group => new NTPClientGroupController(Source, group));

        #endregion

    }

}
