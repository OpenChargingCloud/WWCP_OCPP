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

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    /// <summary>
    /// The roots for its back ends a CSMS installs, lists and deletes as
    /// CSMSRootCertificate - CentralSystemRootCertificate in OCPP 1.6: TLS
    /// roots of the station's certificate store for "ocpp", marked for their
    /// certificateGroup.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The store is the one place they are kept, so that a root installed
    /// through OCPP is on the certificate page and vouches for the server of
    /// a wss:// connection at its next handshake, and one uploaded there is
    /// listed to a CSMS. A group is the usage "ocppca-&lt;group&gt;" beside
    /// "ocpp": a connection whose pins name the group is anchored at those
    /// roots alone - see ServerPins.RootGroup.
    /// </para>
    /// <para>
    /// Listed are the roots that vouch for a back end: those for "ocpp", and
    /// those never told what they are for, which vouch for every server. Such
    /// a root is not deleted for the back ends alone - it would be gone for
    /// every service - and is deleted on the certificate page. A root kept for
    /// another service as well loses "ocpp" and its marks, and stays for that.
    /// </para>
    /// <para>
    /// Without this source a station whose CSMS has a CA of the operator's own
    /// kept what InstallCertificate sent in memory, where no handshake asks,
    /// and lost it at its next start.
    /// </para>
    /// </remarks>
    /// <param name="Node">The station whose store keeps them.</param>
    public sealed class CSMSRootCertificates(WWCPNode Node) : ICertificateSource
    {

        #region Data

        /// <summary>
        /// The OCPP 2.x name of the type.
        /// </summary>
        public const String CertificateType     = "CSMSRootCertificate";

        /// <summary>
        /// The OCPP 1.6 name of the type.
        /// </summary>
        public const String CertificateType1_6  = "CentralSystemRootCertificate";

        private const String usage       = ConnectionEntry.PinsService;
        private const String markPrefix  = usage + "ca-";

        private readonly ConcurrentDictionary<String, X509Certificate2> loaded = new (StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Properties

        /// <inheritdoc/>
        public IEnumerable<String> CertificateTypes
            => [ CertificateType, CertificateType1_6 ];

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
                                                                  $"'{CertificateGroup}' cannot name a group of roots for back ends here: up to {ServerPins.MaxRootGroupLength} letters, digits, '-' or '_'."));

            var store    = Node.Certificates;
            var existing = store.Registrations(CertificateEntry.ThumbprintOf(Certificate)[..CertificateEntry.IdLength]).
                                 FirstOrDefault(entry => entry.Kind == CertificateKind.TLSRoot);

            // What it is for from now on: what it was for, and the back ends,
            // and the group. A root for every use is for the back ends
            // already, and keeps every usage the store offers where a group
            // narrows it to a list.
            IReadOnlyList<String>? usages = existing is null
                                                ? [ usage ]
                                                : existing.Usages?.Select(one => one.ToString()).ToArray();

            if (existing is not null && usages is not null && !usages.Contains(usage))
                usages = [ .. usages, usage ];

            if (CertificateGroup is not null)
            {

                var mark = ServerPins.MarkOf(usage, CertificateGroup);

                usages ??= [ .. store.UsagesFor(CertificateKind.TLSRoot).Select(one => one.ToString()) ];

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

            Node.Log.Notice($"{CertificateType} '{entry.Label}' ({entry.Id}) installed{(CertificateGroup is not null ? $" in group {CertificateGroup}" : "")} " +
                            $"by {ChangeOrigin.OCPP(Sender ?? "CSMS")}: for {CertificateUsages.Describe(entry.Usages)}. " +
                             "It vouches for the server of a wss:// connection from the next handshake on.",
                            "ocpp", "certificates", "security");

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
                                                  $"The root '{entry.Label}' vouches for every service, and is deleted on the certificate page rather than for the back ends alone.")
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
            var others  = entry.Usages.Select(one => one.ToString()).
                                       Where (one => one != usage && !one.StartsWith(markPrefix, StringComparison.Ordinal)).
                                       ToArray();

            String?  error;
            Boolean  notSaved;

            var done = others.Length == 0
                           ? store.Remove   (entry.Id, CertificateKind.TLSRoot,         out error,         out notSaved)
                           : store.SetUsages(entry.Id, CertificateKind.TLSRoot, others, out _, out error, out notSaved);

            if (!done)
                return Task.FromResult(CertificateAnswer.Failed(notSaved ? "InternalError" : "InvalidCertificate", error));

            loaded.TryRemove(entry.Id, out _);

            Node.Log.Notice($"{CertificateType} '{entry.Label}' ({entry.Id}) deleted by {ChangeOrigin.OCPP(Sender ?? "CSMS")}" +
                            (others.Length > 0 ? $": kept for {String.Join(", ", others)}." : "."),
                            "ocpp", "certificates", "security");

            return Task.FromResult(CertificateAnswer.Accepted);

        }

        #endregion


        #region (private) VouchingRoots() / Named(Hashes) / Loaded(Entry)

        /// <summary>
        /// The TLS roots of the store that vouch for a back end: for "ocpp", or
        /// for every use.
        /// </summary>
        private IEnumerable<CertificateEntry> VouchingRoots()

            => Node.Certificates.ByKind(CertificateKind.TLSRoot).
                                 Where (entry => entry.IsFor(usage));

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
