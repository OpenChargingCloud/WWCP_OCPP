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

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.OCPP.Node.KeyPins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    /// <summary>
    /// What a connection is given as JSON - by a page, by the console -
    /// read into what its store takes.
    /// </summary>
    public static class ConnectionFields
    {

        #region KeyPinsOf(JSON, Name)

        /// <summary>
        /// The key pins under the given name as one text, separated by
        /// commas, whether they came as a list or as a text; null where
        /// there is nothing.
        /// </summary>
        public static String? KeyPinsOf(JObject JSON, String Name)

            => JSON[Name] switch {
                   JArray list                                  => String.Join(",", list.Select(pin => pin.Type == JTokenType.String
                                                                                                           ? pin.Value<String>()
                                                                                                           : pin.ToString(Newtonsoft.Json.Formatting.None))),
                   JValue { Type: JTokenType.Null } or null     => null,
                   var other                                    => other.ToString()
               };

        #endregion

        #region KeyPinTrustOf(JSON)

        /// <summary>
        /// Whether a wss:// connection offers raw public keys and learns a
        /// key pin on first use, and how long that waits; null where nothing
        /// of it is said.
        /// </summary>
        public static ServerKeyPinSettings? KeyPinTrustOf(JObject JSON)
        {

            var probation = JSON["serverKeyPinProbationTime"];

            if (JSON["rawPublicKeys"] is null && JSON["trustOnFirstUse"] is null && probation is null)
                return null;

            return new ServerKeyPinSettings(JSON.Value<String>("rawPublicKeys"),
                                            JSON.Value<String>("trustOnFirstUse"),
                                            SecondsOf(probation));

        }

        #endregion

        #region SshTrustOf(JSON)

        /// <summary>
        /// How an ssh:// connection believes its server beyond its pins;
        /// null where nothing of it is said.
        /// </summary>
        public static SshTrustSettings? SshTrustOf(JObject JSON)
        {

            var probation = JSON["sshProbationTime"];

            if (JSON["sshTrustOnFirstUse"] is null && JSON["sshOnPinMismatch"] is null && probation is null)
                return null;

            return new SshTrustSettings(JSON.Value<String>("sshTrustOnFirstUse"),
                                        JSON.Value<String>("sshOnPinMismatch"),
                                        SecondsOf(probation));

        }

        #endregion


        #region (private static) SecondsOf(Probation)

        // A number, a number written as a text, nothing - and -1 for what
        // is neither, which the store refuses in its own words.
        private static Int64? SecondsOf(JToken? Probation)

            => Probation?.Type switch {
                   JTokenType.Integer                                                                => Probation.Value<Int64>(),
                   JTokenType.String when Int64.TryParse(Probation.Value<String>(), out var parsed)  => parsed,
                   JTokenType.Null or null                                                           => null,
                   _                                                                                 => -1
               };

        #endregion

    }

}
