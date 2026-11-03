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

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The configuration keys a charge point reads from somewhere else, beside
    /// its own - see <see cref="IConfigurationKeySource"/>.
    /// </summary>
    public abstract partial class AChargePointNode
    {

        #region Data

        private IConfigurationKeySource[] configurationKeySources = [];

        #endregion


        #region AddConfigurationKeySource(Source) / ConfigurationKeySources

        /// <summary>
        /// Add a source of configuration keys, asked before the charge point's
        /// own keys and after the sources added before it.
        /// </summary>
        public void AddConfigurationKeySource(IConfigurationKeySource Source)
        {

            IConfigurationKeySource[] before, after;

            do
            {
                before  = configurationKeySources;
                after   = [ .. before.Where(source => !ReferenceEquals(source, Source)), Source ];
            }
            while (!ReferenceEquals(Interlocked.CompareExchange(ref configurationKeySources, after, before), before));

        }

        /// <summary>
        /// The sources of configuration keys, in the order they are asked.
        /// </summary>
        public IReadOnlyList<IConfigurationKeySource> ConfigurationKeySources
            => configurationKeySources;

        #endregion

    }

}
