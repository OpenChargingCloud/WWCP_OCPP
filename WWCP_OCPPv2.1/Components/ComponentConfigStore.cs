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

using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The component configurations of a networking node, an EVSE or a connector.
    /// </summary>
    /// <remarks>
    /// Names and instances are compared case insensitively, as OCPP 2.1 says, and
    /// a component is held at most once per name, instance and EVSE. Readers get
    /// a snapshot that later changes leave alone, so the store may be read and
    /// changed from several threads at once.
    /// </remarks>
    public class ComponentConfigStore : IEnumerable<ComponentConfig>
    {

        #region Data

        private readonly ConcurrentDictionary<String, ComponentConfig[]>  componentConfigs   = new (StringComparer.OrdinalIgnoreCase);
        private readonly Lock                                             writeLock          = new();

        #endregion


        #region TryAdd   (ComponentConfig)

        /// <summary>
        /// Add the given component configuration, unless a component with the
        /// same name, instance and EVSE is already there.
        /// </summary>
        /// <param name="ComponentConfig">A component configuration.</param>
        public Boolean TryAdd(ComponentConfig ComponentConfig)
        {
            lock (writeLock)
            {

                var present = componentConfigs.TryGetValue(ComponentConfig.Name, out var components)
                                  ? components
                                  : [];

                if (present.Any(component => IsSame(component, ComponentConfig.Name, ComponentConfig.Instance, ComponentConfig.EVSE)))
                    return false;

                componentConfigs[ComponentConfig.Name] = [.. present, ComponentConfig];
                return true;

            }
        }

        #endregion

        #region TryRemove(Name, Instance = null, EVSE = null)

        /// <summary>
        /// Remove the component with the given name, instance and EVSE.
        /// </summary>
        /// <param name="Name">The case insensitive name of the component.</param>
        /// <param name="Instance">The optional case insensitive instance of the component.</param>
        /// <param name="EVSE">The optional EVSE of the component.</param>
        /// <param name="ComponentConfig">The removed component configuration.</param>
        public Boolean TryRemove(String                                     Name,
                                 String?                                    Instance,
                                 EVSE?                                      EVSE,
                                 [NotNullWhen(true)] out ComponentConfig?  ComponentConfig)
        {
            lock (writeLock)
            {

                ComponentConfig = null;

                if (!componentConfigs.TryGetValue(Name, out var components))
                    return false;

                ComponentConfig = components.FirstOrDefault(component => IsSame(component, Name, Instance, EVSE));

                if (ComponentConfig is null)
                    return false;

                var removed = ComponentConfig;
                var left    = components.Where(component => !ReferenceEquals(component, removed)).ToArray();

                if (left.Length == 0)
                    componentConfigs.TryRemove(Name, out _);

                else
                    componentConfigs[Name] = left;

                return true;

            }
        }

        #endregion

        #region TryGet   (Name, out ComponentConfigs)

        /// <summary>
        /// Try to get all component configurations of the given name.
        /// </summary>
        /// <param name="Name">The case insensitive name of the component.</param>
        /// <param name="ComponentConfigs">The component configurations of the given name.</param>
        public Boolean TryGet(String                                                 Name,
                              [NotNullWhen(true)] out IEnumerable<ComponentConfig>?  ComponentConfigs)
        {

            if (componentConfigs.TryGetValue(Name, out var components))
            {
                ComponentConfigs = components;
                return true;
            }

            ComponentConfigs = null;
            return false;

        }

        #endregion

        #region Get      (Name, Instance)

        /// <summary>
        /// All component configurations of the given name and instance.
        /// </summary>
        /// <param name="Name">The case insensitive name of the component.</param>
        /// <param name="Instance">The optional case insensitive instance of the component.</param>
        public IEnumerable<ComponentConfig> Get(String   Name,
                                                String?  Instance)

            => componentConfigs.TryGetValue(Name, out var components)
                   ? components.Where(component => SameInstance(component.Instance, Instance))
                   : [];

        #endregion


        #region (static) SameInstance(Instance1, Instance2)

        /// <summary>
        /// Whether the given instances are the same, compared case insensitively.
        /// No instance is the same as no instance only.
        /// </summary>
        /// <param name="Instance1">An optional instance.</param>
        /// <param name="Instance2">Another optional instance.</param>
        public static Boolean SameInstance(String?  Instance1,
                                           String?  Instance2)

            => String.Equals(Instance1, Instance2, StringComparison.OrdinalIgnoreCase);

        #endregion

        #region (private static) IsSame(ComponentConfig, Name, Instance, EVSE)

        private static Boolean IsSame(ComponentConfig  ComponentConfig,
                                      String           Name,
                                      String?          Instance,
                                      EVSE?            EVSE)

            => String.Equals(ComponentConfig.Name, Name, StringComparison.OrdinalIgnoreCase) &&
               SameInstance (ComponentConfig.Instance, Instance)                          &&
               Equals       (ComponentConfig.EVSE,     EVSE);

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// All component configurations, as they are at this moment.
        /// </summary>
        public IEnumerator<ComponentConfig> GetEnumerator()

            => componentConfigs.Values.SelectMany(components => components).ToList().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()

            => GetEnumerator();

        #endregion

    }

}
