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
    /// One entry of a SetVariables request, with the component and the
    /// variable it was found to mean.
    /// </summary>
    /// <param name="Data">The entry as it was sent.</param>
    /// <param name="Component">The component configuration it names.</param>
    /// <param name="Variable">The variable configuration it names.</param>
    public sealed record SetVariableEntry(SetVariableData  Data,
                                          ComponentConfig  Component,
                                          VariableConfig   Variable);


    /// <summary>
    /// Where variables are set together: it is handed every entry of a
    /// SetVariables request that is its at once, instead of one at a time.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For variables that only make sense together: a change that must be
    /// written as a whole, or entries that refer to one another, such as a new
    /// name server and its transport in the same request. A variable is the
    /// source's when its own configuration names the source, or else its
    /// component's does - see <see cref="VariableConfig.SetVariablesSource"/>
    /// and <see cref="ComponentConfig.SetVariablesSource"/>.
    /// </para>
    /// <para>
    /// Each source is a change of its own, under every consistency model:
    /// entries of other sources and other components in the same request are
    /// not affected by it. With IndependentRequests the source is handed all
    /// its entries, and an entry already refused for its attribute type or
    /// for being read-only is left out. With ACID it is handed all its entries
    /// or, when one of them was refused, none: they are then rolled back. With
    /// BASE the same holds for each of its units, one call per unit - see
    /// <see cref="BaseUnitOf"/>. Everything else, from the values to the
    /// order in which they are put into effect, is the source's to decide.
    /// </para>
    /// </remarks>
    public interface ISetVariablesSource
    {

        /// <summary>
        /// What is set as one with BASE: the entries of all components this
        /// gives the same unit form one change. The component itself, unless
        /// the source says otherwise - a group of time servers with its
        /// clients, say.
        /// </summary>
        /// <param name="Component">A component with a variable of this source.</param>
        Object BaseUnitOf(ComponentConfig Component)
            => Component;

        /// <summary>
        /// Set the given entries.
        /// </summary>
        /// <param name="Entries">The entries of one change of this source, in request order.</param>
        /// <param name="DataConsistencyModel">How the request wants them set: IndependentRequests, ACID or BASE.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        /// <returns>One result per entry, in the order of the entries.</returns>
        Task<IReadOnlyList<SetVariableResult>> SetVariablesAsync(IReadOnlyList<SetVariableEntry>  Entries,
                                                                 DataConsistencyModel             DataConsistencyModel,
                                                                 CancellationToken                CancellationToken);

    }

}
