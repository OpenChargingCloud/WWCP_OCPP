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

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Certificates;
using cloud.charging.open.protocols.WWCP.Node.Configuration;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The roots for name servers a CSMS installs, lists and deletes as
    /// NameServerRootCertificate (Secure DNS, section 3.1.3): TLS roots of the
    /// node's certificate store for "dns", marked for their certificateGroup -
    /// in both OCPP versions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The store is the one place they are kept, so that a root installed
    /// through OCPP is on the certificate page, and one uploaded there is
    /// listed to a CSMS. A group is the usage "dnsca-&lt;group&gt;" beside
    /// "dns": a name server whose RootCAs names the group is anchored at those
    /// roots alone - see ServerPins.RootGroup.
    /// </para>
    /// <para>
    /// Listed are the roots that vouch for name servers: those for "dns", and
    /// those never told what they are for, which vouch for every server. Such
    /// a root is not deleted for the name servers alone - it would be gone for
    /// every service - and is deleted on the certificate page. A root kept for
    /// another service as well loses "dns" and its marks, and stays for that.
    /// </para>
    /// <para>
    /// A root is not deleted where the names of the CSMS would no longer
    /// resolve without it: the name servers are asked with the store as it
    /// would be afterwards, as for a change of the name servers themselves
    /// (Secure DNS T21). Its connections are let go of once it is gone, by
    /// the node.
    /// </para>
    /// </remarks>
    /// <param name="Node">The node whose store keeps them.</param>
    /// <param name="NamesToKeepResolving">The names the CSMS connections are dialled by.</param>
    public sealed class DNSRootCertificates(WWCPNode                    Node,
                                            Func<IEnumerable<String>>?  NamesToKeepResolving = null) : ICertificateSource
    {

        #region Data

        /// <summary>
        /// The OCPP name of the type.
        /// </summary>
        public const String CertificateType = "NameServerRootCertificate";

        private const String markPrefix = CertificateUsages.DNS + "ca-";

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
                                                                  $"'{CertificateGroup}' cannot name a group of roots for name servers here: up to {ServerPins.MaxRootGroupLength} letters, digits, '-' or '_'."));

            var store    = Node.Certificates;
            var existing = store.Registrations(CertificateEntry.ThumbprintOf(Certificate)[..CertificateEntry.IdLength]).
                                 FirstOrDefault(entry => entry.Kind == CertificateKind.TLSRoot);

            // What it is for from now on: what it was for, and the name
            // servers, and the group. A root for every use is for the name
            // servers already, and keeps every usage the store offers where a
            // group narrows it to a list.
            IReadOnlyList<String>? usages = existing is null
                                                ? [ CertificateUsages.DNS ]
                                                : existing.Usages?.Select(usage => usage.ToString()).ToArray();

            if (existing is not null && usages is not null && !usages.Contains(CertificateUsages.DNS))
                usages = [ .. usages, CertificateUsages.DNS ];

            if (CertificateGroup is not null)
            {

                var mark = ServerPins.MarkOf(CertificateUsages.DNS, CertificateGroup);

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

            Node.Log.Notice($"NameServerRootCertificate '{entry.Label}' ({entry.Id}) installed{(CertificateGroup is not null ? $" in group {CertificateGroup}" : "")} " +
                            $"by {ChangeOrigin.OCPP(Sender ?? "CSMS")}: for {CertificateUsages.Describe(entry.Usages)}.",
                            "dns", "certificates", "security");

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
        /// <remarks>
        /// The names of the CSMS are resolved twice, with the store as it is
        /// and without the root, and a name that resolves now and would not
        /// then keeps the root. Where they do not resolve now either, the root
        /// is not what stops them. Name servers asked without TLS owe nothing
        /// to a root, so there is nothing to ask where all of them are.
        /// </remarks>
        public async Task<CertificateAnswer?> CheckDeletionAsync(CertificateHashes  Hashes,
                                                                 CancellationToken  CancellationToken = default)
        {

            if (Named(Hashes) is not CertificateEntry entry)
                return null;

            if (entry.Usages is null)
                return ForEveryUse(entry);

            var names = NamesToKeepResolving?.Invoke().ToArray() ?? [];

            if (names.Length == 0 || !AnyServerOverTLS())
                return CertificateAnswer.Accepted;

            var now   = Node.ResolveWithAsync(new JObject(), names, DNSDeviceModel.PreflightTime, CancellationToken);
            var then  = Node.ResolveWithAsync(new JObject(), names, DNSDeviceModel.PreflightTime, CancellationToken, WithoutRoots: [ entry.Thumbprint ]);

            await Task.WhenAll(now, then).ConfigureAwait(false);

            var lost  = names.Where(name => now.Result.Resolves(name) && !then.Result.Resolves(name)).ToArray();

            if (lost.Length == 0)
                return CertificateAnswer.Accepted;

            var servers = then.Result.Judgements.Count > 0
                              ? String.Join(", ", then.Result.Judgements.Select(judgement => $"{judgement.Server} {judgement.Outcome}"))
                              : "no name server answered";

            return CertificateAnswer.Failed("CSMSUnreachable",
                                            $"Without the root '{entry.Label}', '{String.Join("', '", lost)}' would not resolve: {servers}!");

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
                return Task.FromResult(ForEveryUse(entry));

            var store   = Node.Certificates;
            var others  = entry.Usages.Select(usage => usage.ToString()).
                                       Where (usage => usage != CertificateUsages.DNS && !usage.StartsWith(markPrefix, StringComparison.Ordinal)).
                                       ToArray();

            String?  error;
            Boolean  notSaved;

            var done = others.Length == 0
                           ? store.Remove   (entry.Id, CertificateKind.TLSRoot,         out error,         out notSaved)
                           : store.SetUsages(entry.Id, CertificateKind.TLSRoot, others, out _, out error, out notSaved);

            if (!done)
                return Task.FromResult(CertificateAnswer.Failed(notSaved ? "InternalError" : "InvalidCertificate", error));

            loaded.TryRemove(entry.Id, out _);

            Node.Log.Notice($"NameServerRootCertificate '{entry.Label}' ({entry.Id}) deleted by {ChangeOrigin.OCPP(Sender ?? "CSMS")}" +
                            (others.Length > 0 ? $": kept for {String.Join(", ", others)}." : "."),
                            "dns", "certificates", "security");

            return Task.FromResult(CertificateAnswer.Accepted);

        }

        #endregion


        #region (private) VouchingRoots() / Named(Hashes) / Loaded(Entry)

        /// <summary>
        /// The TLS roots of the store that vouch for name servers: for "dns", or
        /// for every use.
        /// </summary>
        private IEnumerable<CertificateEntry> VouchingRoots()

            => Node.Certificates.ByKind(CertificateKind.TLSRoot).
                                 Where (entry => entry.IsFor(CertificateUsage.DNS));

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

        #region (private) ForEveryUse(Entry) / AnyServerOverTLS()

        /// <summary>
        /// The no to deleting a root never told what it is for.
        /// </summary>
        private static CertificateAnswer ForEveryUse(CertificateEntry Entry)

            => CertificateAnswer.Failed("ForEveryUse",
                                        $"The root '{Entry.Label}' vouches for every service, and is deleted on the certificate page rather than for the name servers alone.");

        /// <summary>
        /// Whether any configured name server is asked over TLS, and so judged
        /// by its certificate.
        /// </summary>
        private Boolean AnyServerOverTLS()

            => (Node.DNSConfigurationJSON()["servers"] as JArray ?? []).
                   Any(server => server["transport"]?.Value<String>()?.ToUpperInvariant() is String transport &&
                                 transport is not "UDP" and not "TCP");

        #endregion

    }

}
