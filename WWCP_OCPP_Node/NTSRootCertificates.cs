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

using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Certificates;
using cloud.charging.open.protocols.WWCP.Node.Configuration;
using cloud.charging.open.protocols.WWCP.Node.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The roots for time servers a CSMS installs, lists and deletes as
    /// NetworkTimeRootCertificate (Secure Time Synchronization, section 3.2.5):
    /// TLS roots of the node's certificate store for "nts", marked for their
    /// certificateGroup - in both OCPP versions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The store is the one place they are kept, so that a root installed
    /// through OCPP is on the certificate page, and one uploaded there is
    /// listed to a CSMS. A group is the usage "ntsca-&lt;group&gt;" beside
    /// "nts": a server whose RootCAs names the group is anchored at those
    /// roots alone - see ServerPins.RootGroup.
    /// </para>
    /// <para>
    /// Listed are the roots that vouch for time servers: those for "nts", and
    /// those never told what they are for, which vouch for every server. Such
    /// a root is not deleted for the time servers alone - it would be gone for
    /// every service - and is deleted on the certificate page. A root kept for
    /// another service as well loses "nts" and its marks, and stays for that.
    /// </para>
    /// <para>
    /// Every installation and deletion is written into the metrological log
    /// with the CSMS that asked: these roots decide which servers may vouch
    /// for the legal time.
    /// </para>
    /// </remarks>
    /// <param name="Node">The node whose store keeps them.</param>
    public sealed class NTSRootCertificates(WWCPNode Node) : ICertificateSource
    {

        #region Data

        /// <summary>
        /// The OCPP name of the type.
        /// </summary>
        public const String CertificateType = "NetworkTimeRootCertificate";

        private const String markPrefix = CertificateUsages.NTS + "ca-";

        private readonly ConcurrentDictionary<String, X509Certificate2> loaded = new (StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Properties

        /// <inheritdoc/>
        public IEnumerable<String> CertificateTypes
            => [ CertificateType ];

        #endregion


        #region InstallAsync(CertificateType, CertificateGroup, Certificate, Sender, CancellationToken = default)

        /// <inheritdoc/>
        public Task<CertificateAnswer> InstallAsync(String             CertificateType,
                                                    String?            CertificateGroup,
                                                    X509Certificate2   Certificate,
                                                    String?            Sender,
                                                    CancellationToken  CancellationToken = default)
        {

            if (CertificateGroup is not null && !ServerPins.IsRootGroup(CertificateGroup))
                return Task.FromResult(CertificateAnswer.Rejected("InvalidValue",
                                                                  $"'{CertificateGroup}' cannot name a group of roots for time servers here: up to {ServerPins.MaxRootGroupLength} letters, digits, '-' or '_'."));

            var store    = Node.Certificates;
            var existing = store.Registrations(CertificateEntry.ThumbprintOf(Certificate)[..CertificateEntry.IdLength]).
                                 FirstOrDefault(entry => entry.Kind == CertificateKind.TLSRoot);

            // What it is for from now on: what it was for, and the time
            // servers, and the group. A root for every use is for the time
            // servers already, and keeps every usage the store offers where a
            // group narrows it to a list.
            IReadOnlyList<String>? usages = existing is null
                                                ? [ CertificateUsages.NTS ]
                                                : existing.Usages?.Select(usage => usage.ToString()).ToArray();

            if (existing is not null && usages is not null && !usages.Contains(CertificateUsages.NTS))
                usages = [ .. usages, CertificateUsages.NTS ];

            if (CertificateGroup is not null)
            {

                var mark = ServerPins.MarkOf(CertificateUsages.NTS, CertificateGroup);

                usages ??= [ .. store.UsagesFor(CertificateKind.TLSRoot).Select(usage => usage.ToString()) ];

                if (!usages.Contains(mark))
                    usages = [ .. usages, mark ];

            }

            if (!store.Import(Encoding.ASCII.GetBytes(Certificate.ExportCertificatePem()),
                              CertificateKind.TLSRoot,
                              null,
                              existing?.Label,
                              usages,
                              out var entry,
                              out var error,
                              out var notSaved))
            {
                return Task.FromResult(notSaved
                                           ? CertificateAnswer.Failed  ("InternalError",      error)
                                           : CertificateAnswer.Rejected("InvalidCertificate", error));
            }

            Node.Log.Metrological(LogLevel.Notice,
                                  $"NetworkTimeRootCertificate '{entry.Label}' ({entry.Id}) installed{(CertificateGroup is not null ? $" in group {CertificateGroup}" : "")} " +
                                  $"by {ChangeOrigin.OCPP(Sender ?? "CSMS")}: for {CertificateUsages.Describe(entry.Usages)}.",
                                  "nts", "certificates", "security");

            return Task.FromResult(CertificateAnswer.Accepted);

        }

        #endregion

        #region Installed(CertificateType)

        /// <inheritdoc/>
        public IEnumerable<X509Certificate2> Installed(String CertificateType)
        {

            var certificates = new List<X509Certificate2>();

            foreach (var entry in VouchingRoots())
                if (Loaded(entry) is X509Certificate2 certificate)
                    certificates.Add(certificate);

            return certificates;

        }

        #endregion

        #region CheckDeletionAsync(Hashes, CancellationToken = default)

        /// <inheritdoc/>
        public Task<CertificateAnswer?> CheckDeletionAsync(CertificateHashes  Hashes,
                                                           CancellationToken  CancellationToken = default)

            => Task.FromResult(CheckDeletion(Hashes));

        #endregion

        #region (private) CheckDeletion(Hashes)

        private CertificateAnswer? CheckDeletion(CertificateHashes Hashes)
        {

            if (Named(Hashes) is not CertificateEntry entry)
                return null;

            return entry.Usages is null
                       ? CertificateAnswer.Failed("ForEveryUse",
                                                  $"The root '{entry.Label}' vouches for every service, and is deleted on the certificate page rather than for the time servers alone.")
                       : CertificateAnswer.Accepted;

        }

        #endregion

        #region DeleteAsync(Hashes, Sender, CancellationToken = default)

        /// <inheritdoc/>
        public Task<CertificateAnswer> DeleteAsync(CertificateHashes  Hashes,
                                                   String?            Sender,
                                                   CancellationToken  CancellationToken = default)
        {

            if (Named(Hashes) is not CertificateEntry entry)
                return Task.FromResult(CertificateAnswer.NotFound);

            if (entry.Usages is null)
                return Task.FromResult(CheckDeletion(Hashes)!);

            var store   = Node.Certificates;
            var others  = entry.Usages.Select(usage => usage.ToString()).
                                       Where (usage => usage != CertificateUsages.NTS && !usage.StartsWith(markPrefix, StringComparison.Ordinal)).
                                       ToArray();

            String?  error;
            Boolean  notSaved;

            var done = others.Length == 0
                           ? store.Remove   (entry.Id, CertificateKind.TLSRoot,         out error,         out notSaved)
                           : store.SetUsages(entry.Id, CertificateKind.TLSRoot, others, out _, out error, out notSaved);

            if (!done)
                return Task.FromResult(CertificateAnswer.Failed(notSaved ? "InternalError" : "InvalidCertificate", error));

            loaded.TryRemove(entry.Id, out _);

            Node.Log.Metrological(LogLevel.Notice,
                                  $"NetworkTimeRootCertificate '{entry.Label}' ({entry.Id}) deleted by {ChangeOrigin.OCPP(Sender ?? "CSMS")}" +
                                  (others.Length > 0 ? $": kept for {String.Join(", ", others)}." : "."),
                                  "nts", "certificates", "security");

            return Task.FromResult(CertificateAnswer.Accepted);

        }

        #endregion


        #region (private) VouchingRoots() / Named(Hashes) / Loaded(Entry)

        /// <summary>
        /// The TLS roots of the store that vouch for time servers: for "nts", or
        /// for every use.
        /// </summary>
        private IEnumerable<CertificateEntry> VouchingRoots()

            => Node.Certificates.ByKind(CertificateKind.TLSRoot).
                                 Where (entry => entry.IsFor(CertificateUsage.NTS));

        /// <summary>
        /// The root of these hashes among them, or null.
        /// </summary>
        private CertificateEntry? Named(CertificateHashes Hashes)

            => VouchingRoots().FirstOrDefault(entry => Loaded(entry) is X509Certificate2 certificate &&
                                                       Hashes.Names(certificate));

        /// <summary>
        /// The certificate of an entry, loaded once and kept: what is listed
        /// stays this source's, and is not disposed of by whoever lists it.
        /// </summary>
        private X509Certificate2? Loaded(CertificateEntry Entry)
        {

            if (loaded.TryGetValue(Entry.Id, out var certificate))
                return certificate;

            if (!Node.Certificates.TryLoad(Entry, out certificate, out _))
                return null;

            return loaded.GetOrAdd(Entry.Id, certificate);

        }

        #endregion

    }

}
