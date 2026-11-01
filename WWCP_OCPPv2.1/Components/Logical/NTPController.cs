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
    /// The NTP controller of the Secure Time Synchronization white paper
    /// (section 3.2.2): what applies to time synchronization as a whole -
    /// which groups of time servers there are, and whether the time is legal
    /// time. It has no instance.
    /// </summary>
    /// <remarks>
    /// Its variables are those of the paper that the given source can answer,
    /// read from it at every read, and ClientGroups, read from the groups of
    /// the source. A variable the source cannot answer is left out, so that a
    /// CSMS asking for it learns it is unknown rather than reading an empty
    /// value as an answer.
    /// </remarks>
    public class NTPController : ALogicalComponentConfig
    {

        #region Data

        /// <summary>
        /// The variables of the paper, in its order.
        /// </summary>
        internal static readonly ControllerVariable[] Variables = [

            new ("ClientGroups",     DataTypes.String,      "The ids of the NTPClientGroupController instances, comma separated. A new id creates a group, disabled and without clients; an id taken out removes the group.",  Writable: true),
            new ("SuggestedGroups",  DataTypes.String,      "The ids of groups this charging station knows a meaning for and does not have yet, comma separated."),
            new ("LegalTime",        DataTypes.Boolean,     "Whether the time of this charging station is legal time now."),
            new ("LegalTimeReason",  DataTypes.OptionList,  "Why the time is not legal time: no authority is named, the legal group is disabled, never checked, checked too long ago, or the clock was too far from it. Empty while the time is legal time.",
                                                            ValuesList: [ "NotClaimed", "Disabled", "NeverChecked", "Stale", "OffBy" ])

        ];

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create the NTP controller of the given source.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        /// <param name="Description">An optional multi-language description of the controller.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public NTPController(INTPControllerSource  Source,
                             I18NString?           Description   = null,
                             CustomData?           CustomData    = null)

            : base(nameof(NTPController),
                   null,
                   Description ?? I18NString.Create("How this charging station keeps its time (Secure Time Synchronization)."),
                   CustomData)

        {

            var answered = new HashSet<String>(Source.ControllerVariables, StringComparer.OrdinalIgnoreCase) {
                               "ClientGroups"
                           };

            foreach (var variable in Variables.Where(variable => answered.Contains(variable.Name)))
                variableConfigs.Add(
                    variable.ToVariableConfig(variable.Name == "ClientGroups"
                                                  ? () => String.Join(",", Source.Groups)
                                                  : () => Source.ControllerValue(variable.Name),
                                              Writes: false)
                );

        }

        #endregion


        #region (static) AllOf(Source)

        /// <summary>
        /// The NTP controller of the given source, followed by the controller of
        /// every group and of every client of it.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        public static IEnumerable<ALogicalComponentConfig> AllOf(INTPControllerSource Source)

            => [ new NTPController(Source),
                 .. NTPClientGroupController.AllOf(Source),
                 .. NTPClientController.     AllOf(Source) ];

        #endregion

    }

}
