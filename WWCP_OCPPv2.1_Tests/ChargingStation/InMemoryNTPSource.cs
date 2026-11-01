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

namespace cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation
{

    /// <summary>
    /// A source of the NTP controllers holding its values in dictionaries a
    /// test changes: a "legal" group of the four PTB servers and a "local"
    /// group of two Local Controllers, answering what the reference
    /// implementation has - no Preflight, no delay asymmetries, no separate
    /// NTS-KE server, no root CA groups, no signed responses.
    /// </summary>
    public sealed class InMemoryNTPSource : INTPControllerSource
    {

        #region Properties

        public Dictionary<String, String>                       Controller   { get; } = new (StringComparer.OrdinalIgnoreCase) {
            ["SuggestedGroups"]  = "grid",
            ["LegalTime"]        = "true",
            ["LegalTimeReason"]  = ""
        };

        /// <summary>
        /// The group ids, in their order.
        /// </summary>
        public List<String>                                     GroupIds     { get; } = [ "legal", "local" ];

        public Dictionary<String, Dictionary<String, String>>   Group        { get; } = new (StringComparer.OrdinalIgnoreCase) {

            ["legal"] = new (StringComparer.OrdinalIgnoreCase) {
                ["Enabled"]             = "true",
                ["MinServers"]          = "2",
                ["MaxDeviation"]        = "60",
                ["MinInterval"]         = "900",
                ["MaxInterval"]         = "900",
                ["Timeout"]             = "10",
                ["ErrorLogging"]        = "5",
                ["MaxErrors"]           = "10",
                ["Limited"]             = "false",
                ["ErrorsInARow"]        = "0",
                ["LastCheck"]           = "2026-10-30T12:00:00.000Z",
                ["LastOffset"]          = "0.012",
                ["LegalTimeAuthority"]  = "PTB",
                ["LegalTimeTolerance"]  = "1",
                ["LegalTimeMaxAge"]     = "3600"
            },

            // Saying "PTB" here as well, to show that the local group never
            // reports what only the legal group has.
            ["local"] = new (StringComparer.OrdinalIgnoreCase) {
                ["Enabled"]             = "false",
                ["MinServers"]          = "1",
                ["MaxDeviation"]        = "60",
                ["MinInterval"]         = "60",
                ["MaxInterval"]         = "60",
                ["Timeout"]             = "2",
                ["ErrorLogging"]        = "5",
                ["MaxErrors"]           = "50",
                ["Limited"]             = "false",
                ["ErrorsInARow"]        = "0",
                ["LastCheck"]           = "",
                ["LastOffset"]          = "",
                ["LegalTimeAuthority"]  = "PTB"
            }

        };

        /// <summary>
        /// The client ids of each group, in their order.
        /// </summary>
        public Dictionary<String, List<String>>                 Members      { get; } = new (StringComparer.OrdinalIgnoreCase) {
            ["legal"]  = [ "ptb1", "ptb2", "ptb3", "ptb4" ],
            ["local"]  = [ "lc1", "lc2" ]
        };

        public Dictionary<String, Dictionary<String, String>>   Client       { get; } = new (StringComparer.OrdinalIgnoreCase) {
            ["ptb1"]  = Server("ptbtime1.ptb.de"),
            ["ptb2"]  = Server("ptbtime2.ptb.de"),
            ["ptb3"]  = Server("ptbtime3.ptb.de"),
            ["ptb4"]  = Server("ptbtime4.ptb.de"),
            ["lc1"]   = Server("lc1.local"),
            ["lc2"]   = Server("lc2.local")
        };

        #endregion


        #region INTPControllerSource

        public IEnumerable<String>  ControllerVariables
            => Controller.Keys;

        public IEnumerable<String>  GroupVariables
            => [ "Enabled", "MinServers", "MaxDeviation", "MinInterval", "MaxInterval", "Timeout", "ErrorLogging", "MaxErrors",
                 "Limited", "ErrorsInARow", "LastCheck", "LastOffset", "LegalTimeAuthority", "LegalTimeTolerance", "LegalTimeMaxAge" ];

        public IEnumerable<String>  ClientVariables
            => [ "Enabled", "ServerURL", "ServerPort", "Mode", "Priority", "NTSKEServerPort", "CertificatePins", "RootPins",
                 "OnPinMismatch", "TrustOnFirstUse", "MinRefresh", "MaxRefresh", "AEADAlgorithm",
                 "Problem", "CertificateFingerprint", "LastKeyExchange", "Cookies" ];

        public IEnumerable<String>  Groups
            => GroupIds;

        public IEnumerable<String> ClientsOf(String Group)

            => Members.TryGetValue(Group, out var clients)
                   ? clients
                   : [];

        public String? ControllerValue(String Variable)

            => Controller.TryGetValue(Variable, out var value)
                   ? value
                   : null;

        public String? GroupValue(String Group, String Variable)

            => GroupVariables.Contains(Variable, StringComparer.OrdinalIgnoreCase) &&
               this.Group.TryGetValue(Group, out var values)
                   ? values.GetValueOrDefault(Variable, "")
                   : null;

        public String? ClientValue(String Client, String Variable)

            => ClientVariables.Contains(Variable, StringComparer.OrdinalIgnoreCase) &&
               this.Client.TryGetValue(Client, out var values)
                   ? values.GetValueOrDefault(Variable, "")
                   : null;

        #endregion


        #region (private static) Server(Hostname)

        private static Dictionary<String, String> Server(String Hostname)

            => new (StringComparer.OrdinalIgnoreCase) {
                   ["Enabled"]                 = "true",
                   ["ServerURL"]               = Hostname,
                   ["ServerPort"]              = "123",
                   ["Mode"]                    = "NTSv4",
                   ["Priority"]                = "0",
                   ["NTSKEServerPort"]         = "4460",
                   ["CertificatePins"]         = "",
                   ["RootPins"]                = "",
                   ["OnPinMismatch"]           = "Refuse",
                   ["TrustOnFirstUse"]         = "None",
                   ["MinRefresh"]              = "1800",
                   ["MaxRefresh"]              = "1800",
                   ["AEADAlgorithm"]           = "AES-SIV-CMAC-256",
                   ["Problem"]                 = "false",
                   ["CertificateFingerprint"]  = "",
                   ["LastKeyExchange"]         = "2026-10-30T11:45:00.000Z",
                   ["Cookies"]                 = "8"
               };

        #endregion

    }

}
