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

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.WWCP.Node.Configuration;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving
{

    /// <summary>
    /// The "commissioning" section: whether a tool may speak OCPP to this node
    /// on the node's own SSH server, as an account that may commission it - a
    /// charging station, a local controller, a gateway or a CSMS alike.
    /// </summary>
    /// <remarks>
    /// Off unless the file says otherwise: a node that has never been told to
    /// let a tool speak OCPP to it does not. Switched on and off without a
    /// restart, since the SSH server is the node's and keeps running either
    /// way.
    /// </remarks>
    /// <param name="Enabled">Whether commissioning over SSH is served; off where not said.</param>
    public sealed record CommissioningConfiguration(Boolean? Enabled = null)
    {

        #region Data

        /// <summary>
        /// The name of this section in the configuration file.
        /// </summary>
        public const String SectionName = "commissioning";

        #endregion

        #region Properties

        /// <summary>
        /// Whether commissioning over SSH is served.
        /// </summary>
        public Boolean IsEnabled
            => Enabled == true;

        #endregion


        #region (static) TryParse(JSON, out Configuration, out Error)

        /// <summary>
        /// The commissioning section of the configuration file.
        /// </summary>
        /// <remarks>
        /// A field it does not know is an error rather than one passed over:
        /// there is one switch here, and somebody who wrote another believed
        /// they had configured something.
        /// </remarks>
        public static Boolean TryParse(JObject                                              JSON,
                                       [NotNullWhen(true)]  out CommissioningConfiguration? Configuration,
                                       [NotNullWhen(false)] out String?                     Error)
        {

            Configuration = null;

            if (JSON.Properties().FirstOrDefault(property => property.Name != "enabled") is JProperty unknown)
            {
                Error = $"'{SectionName}.{unknown.Name}' is not known: the section has 'enabled' and nothing else.";
                return false;
            }

            if (!ConfigurationReader.TryReadBoolean(JSON, "enabled", SectionName, out var enabled, out Error))
                return false;

            Configuration = new CommissioningConfiguration(enabled);
            return true;

        }

        /// <summary>
        /// The commissioning section, where the given configuration file has
        /// one: null where it has none, and an error where what it has is not
        /// the section.
        /// </summary>
        /// <param name="File">The whole configuration file.</param>
        /// <param name="Configuration">The section, or null where there is none.</param>
        /// <param name="Error">What is wrong with it.</param>
        public static Boolean TryParseSectionOf(JObject                           File,
                                                out CommissioningConfiguration?   Configuration,
                                                [NotNullWhen(false)] out String?  Error)
        {

            Configuration  = null;
            Error          = null;

            if (File[SectionName] is not JToken token || token.Type == JTokenType.Null)
                return true;

            if (token is not JObject section)
            {
                Error = $"'{SectionName}' must be a JSON object.";
                return false;
            }

            return TryParse(section, out Configuration, out Error);

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// The section as it is written to the file - only what it has to say.
        /// </summary>
        public JObject ToJSON()
        {

            var json = new JObject();

            if (Enabled.HasValue)
                json.Add("enabled", Enabled.Value);

            return json;

        }

        #endregion

        #region (override) ToString()

        public override String ToString()

            => IsEnabled
                   ? "commissioning over SSH"
                   : "no commissioning over SSH";

        #endregion

    }

}
