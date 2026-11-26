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

using cloud.charging.open.protocols.WWCP.Node.Certificates;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// What a root of the station's certificate store is for, in a sentence:
    /// the context GetInstalledCertificateIds sends along with it.
    /// </summary>
    public static class RootContext
    {

        /// <summary>
        /// The context of a root kept for the given use: what the type is for,
        /// the name of the type in the other OCPP version where it has one, the
        /// groups the root is marked for - or that it was never told what it is
        /// for, and so vouches for the servers of every service, which is why
        /// the same root is listed under more than one type.
        /// </summary>
        /// <param name="Purpose">What the type is for: "a root for the servers of the back ends".</param>
        /// <param name="Entry">The root as the store keeps it, or null where it is not known there.</param>
        /// <param name="MarkPrefix">The prefix of the usages that mark its groups: "ocppca-".</param>
        /// <param name="OtherName">The name of the type in the other OCPP version: "OCPP 1.6: CentralSystemRootCertificate".</param>
        public static String Of(String             Purpose,
                                CertificateEntry?  Entry,
                                String             MarkPrefix,
                                String?            OtherName = null)
        {

            var parts = new List<String> { Purpose };

            if (OtherName is not null)
                parts.Add(OtherName);

            if (Entry is not null)
            {

                if (Entry.Usages is null)
                    parts.Add("never told what it is for, it vouches for the servers of every service");

                else
                {

                    var groups = Entry.Usages.Select(usage => usage.ToString()).
                                              Where (usage => usage.StartsWith(MarkPrefix, StringComparison.Ordinal)).
                                              Select(usage => $"'{usage[MarkPrefix.Length..]}'").
                                              ToArray();

                    if (groups.Length == 1)
                        parts.Add($"of the group {groups[0]}");

                    else if (groups.Length > 1)
                        parts.Add($"of the groups {String.Join(", ", groups)}");

                }

            }

            return String.Join("; ", parts) + ".";

        }

    }

}
