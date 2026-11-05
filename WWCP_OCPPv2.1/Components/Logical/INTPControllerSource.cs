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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Where the NTP controllers read how a charging station keeps its time.
    /// </summary>
    /// <remarks>
    /// The controllers keep no value of their own: whatever holds the
    /// configuration answers each read, in the words of the Secure Time
    /// Synchronization white paper (section 3.2), so that a change made
    /// elsewhere is what the next read sees.
    ///
    /// A client belongs to exactly one group, and a client id is unique across
    /// all groups. NTPController.ClientGroups and
    /// NTPClientGroupController.Clients are read from Groups and ClientsOf, so
    /// they cannot disagree with the instances.
    /// </remarks>
    public interface INTPControllerSource
    {

        /// <summary>
        /// The variables of NTPController this source can answer, besides
        /// ClientGroups.
        /// </summary>
        IEnumerable<String>  ControllerVariables    { get; }

        /// <summary>
        /// The variables of NTPClientGroupController this source can answer,
        /// besides Clients. The variables only the legal group has are asked of
        /// the legal group only.
        /// </summary>
        IEnumerable<String>  GroupVariables         { get; }

        /// <summary>
        /// The variables of NTPClientController this source can answer.
        /// </summary>
        IEnumerable<String>  ClientVariables        { get; }

        /// <summary>
        /// The variables of NTPController a CSMS may set through this source,
        /// ClientGroups among them where groups can be made and taken away.
        /// Only a source that is an ISetVariablesSource as well writes any;
        /// none by default.
        /// </summary>
        IEnumerable<String>  WritableControllerVariables
            => [];

        /// <summary>
        /// The variables of NTPClientGroupController a CSMS may set through this
        /// source, Clients among them where servers can be added and taken
        /// away; none by default.
        /// </summary>
        IEnumerable<String>  WritableGroupVariables
            => [];

        /// <summary>
        /// The variables of NTPClientController a CSMS may set through this
        /// source; none by default.
        /// </summary>
        IEnumerable<String>  WritableClientVariables
            => [];

        /// <summary>
        /// The ids of the groups, in their order.
        /// </summary>
        IEnumerable<String>  Groups                 { get; }

        /// <summary>
        /// The ids of the clients of the given group, in the order in which
        /// clients of the same priority are listed; none for a group that is
        /// not there.
        /// </summary>
        /// <param name="Group">The id of the group, in any case.</param>
        IEnumerable<String> ClientsOf(String Group);

        /// <summary>
        /// The value of a variable of NTPController, or null where there is no
        /// such variable.
        /// </summary>
        /// <param name="Variable">The name of the variable, in any case.</param>
        String? ControllerValue(String Variable);

        /// <summary>
        /// The value of a variable of the given group, or null where there is no
        /// such group or no such variable.
        /// </summary>
        /// <param name="Group">The id of the group, in any case.</param>
        /// <param name="Variable">The name of the variable, in any case.</param>
        String? GroupValue(String  Group,
                           String  Variable);

        /// <summary>
        /// The value of a variable of the given client, or null where there is
        /// no such client or no such variable.
        /// </summary>
        /// <param name="Client">The id of the client, in any case.</param>
        /// <param name="Variable">The name of the variable, in any case.</param>
        String? ClientValue(String  Client,
                            String  Variable);

        /// <summary>
        /// The values of the paper this source supports of a variable of
        /// NTPClientController that has a values list - the modes it can ask a
        /// time server in, say - or null where it supports every one; every one
        /// by default.
        /// </summary>
        /// <remarks>
        /// The valuesList a CSMS reads lists only these, in the paper's order,
        /// and a value the source does not support is refused like one the
        /// paper does not have (STS 3.2.4: "The valuesList shall list only
        /// what the charging station supports").
        /// </remarks>
        /// <param name="Variable">The name of the variable, as the paper spells it.</param>
        IEnumerable<String>? SupportedClientValues(String Variable)
            => null;

    }

}
