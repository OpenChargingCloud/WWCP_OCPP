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

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using cloud.charging.open.protocols.WWCP.Node.Certificates;

using cloud.charging.open.protocols.OCPP.Node;

using OCPPC  = cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// The roots for name servers a CSMS installs, lists and deletes as
    /// NameServerRootCertificate, against the node's certificate store - and a
    /// root the names of the CSMS resolve through, which is not deleted.
    /// </summary>
    /// <remarks>
    /// Through the OCPP nodes handed to the bridge, built and never started,
    /// as in NTSRootCertificatesTests; the name server that is asked is one
    /// of Hermod's own on the loopback, over TLS.
    /// </remarks>
        public abstract class DNSRootCertificatesTests : ABridgeTests
    {

        #region Data

        private const String NameServer = DNSRootCertificates.CertificateType;

        private String            directory  = "";
        private IBridgedNode?  station;
        private X509Certificate2  root       = default!;
        private X509Certificate2  other      = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void AStation()
        {

            directory  = TestNodes.TemporaryDirectory("dns-root-certificates");
            station    = Nodes.New(directory, TestNodes.Offline);
            root       = Root("Resolver CA");
            other      = Root("Another Resolver CA");

        }

        [TearDown]
        public async Task TakeItAwayAgain()
        {

            root. Dispose();
            other.Dispose();

            if (station is not null)
                await station.DisposeAsync();

            station = null;

            TestNodes.Remove(directory);

        }

        #endregion


        #region ARootInstalledInAGroupIsANameServerRootMarkedForIt()

        /// <summary>
        /// A root installed in a group is a TLS root of the store for the name
        /// servers, marked for its group - and logged with the CSMS that
        /// installed it.
        /// </summary>
        [Test]
        public async Task ARootInstalledInAGroupIsANameServerRootMarkedForIt()
        {

            var before  = station!.Node.Log.LastId;
            var answer  = await Install(root, "CPO");

            Assert.Multiple(() => {
                Assert.That(answer.Status,                                            Is.EqualTo(OCPPC.CertificateAnswerStatus.Accepted));
                Assert.That(TLSRoot(root)?.Usages?.Select(usage => usage.ToString()),  Is.EquivalentTo(new[] { "dns", "dnsca-cpo" }));
                Assert.That(Logged(before),                                           Has.Some.Contains("NameServerRootCertificate").
                                                                                          And.Contains("in group CPO").
                                                                                          And.Contains("by ocpp:CSMS"));
            });

        }

        #endregion

        #region WhatCannotBeAGroupIsRejectedAndNothingIsKept()

        /// <summary>
        /// A group the store cannot mark is rejected with why, and the store
        /// keeps nothing.
        /// </summary>
        [Test]
        public async Task WhatCannotBeAGroupIsRejectedAndNothingIsKept()
        {

            var answer = await Install(root, "the CPO");

            Assert.Multiple(() => {
                Assert.That(answer.Status,                  Is.EqualTo(OCPPC.CertificateAnswerStatus.Rejected));
                Assert.That(answer.AdditionalInfo,          Does.Contain("cannot name a group of roots for name servers"));
                Assert.That(station!.Node.Certificates.Entries,  Is.Empty);
            });

        }

        #endregion

        #region TheRootsThatVouchForNameServersAreListed()

        /// <summary>
        /// Listed are the roots that vouch for name servers, whoever put them
        /// into the store: those for the name servers, and those for every use
        /// - and not those for the time servers alone.
        /// </summary>
        [Test]
        public void TheRootsThatVouchForNameServersAreListed()
        {

            using var everyUse  = Root("Every Use CA");
            using var time      = Root("Legal Time CA");

            Import(root,     [ "dns" ]);
            Import(everyUse, null);
            Import(time,     [ "nts" ]);

            var listed = station!.OCPPv2_1Node.CertificateSources.Installed([ NameServer ]).
                                                                  Select(installed => installed.Certificate.Thumbprint);

            Assert.That(listed,  Is.EquivalentTo(new[] { root.Thumbprint, everyUse.Thumbprint }));

        }

        #endregion

        #region ARootIsDeletedForTheNameServersAndStaysForAnotherService()

        /// <summary>
        /// A root for the name servers alone is deleted; one kept for the
        /// meters as well loses the name servers and its marks and stays; one
        /// for the time servers as well goes from both, as DeleteCertificate
        /// names a certificate and not a type; one for every use is not
        /// deleted for the name servers alone, and stays.
        /// </summary>
        [Test]
        public async Task ARootIsDeletedForTheNameServersAndStaysForAnotherService()
        {

            using var everyUse  = Root("Every Use CA");
            using var time      = Root("Time and Names CA");

            Import(root,     [ "dns", "dnsca-cpo" ]);
            Import(other,    [ "dns", "dnsca-cpo", "meter" ]);
            Import(time,     [ "dns", "nts" ]);
            Import(everyUse, null);

            var sources  = station!.OCPPv2_1Node.CertificateSources;
            var before   = station.Node.Log.LastId;

            var alone    = await sources.DeleteAsync(OCPPC.CertificateHashes.Of(root),     "CSMS");
            var shared   = await sources.DeleteAsync(OCPPC.CertificateHashes.Of(other),    "CSMS");
            var both     = await sources.DeleteAsync(OCPPC.CertificateHashes.Of(time),     "CSMS");
            var forAll   = await sources.DeleteAsync(OCPPC.CertificateHashes.Of(everyUse), "CSMS");

            Assert.Multiple(() => {

                Assert.That(alone.Status,     Is.EqualTo(OCPPC.CertificateAnswerStatus.Accepted));
                Assert.That(TLSRoot(root),    Is.Null);

                Assert.That(shared.Status,    Is.EqualTo(OCPPC.CertificateAnswerStatus.Accepted));
                Assert.That(TLSRoot(other)?.Usages?.Select(usage => usage.ToString()),  Is.EqualTo(new[] { "meter" }));

                Assert.That(both.Status,      Is.EqualTo(OCPPC.CertificateAnswerStatus.Accepted));
                Assert.That(TLSRoot(time),    Is.Null);

                Assert.That(forAll.Status,      Is.EqualTo(OCPPC.CertificateAnswerStatus.Failed));
                Assert.That(forAll.ReasonCode,  Is.EqualTo("ForEveryUse"));
                Assert.That(TLSRoot(everyUse),  Is.Not.Null);

                Assert.That(Logged(before).Where(line => line.StartsWith("NameServerRootCertificate")),
                            Has.Exactly(3).Contains("deleted by ocpp:CSMS"));
                Assert.That(Logged(before).Where(line => line.StartsWith("NetworkTimeRootCertificate")),
                            Has.Exactly(1).Contains("deleted by ocpp:CSMS"));

            });

        }

        #endregion

        #region TheChargePointKeepsItsNameServerRootsInTheStoreAsWell()

        /// <summary>
        /// A root installed through OCPP 1.6 lands in the same store, and is
        /// listed through OCPP 2.1.
        /// </summary>
        [Test]
        public async Task TheChargePointKeepsItsNameServerRootsInTheStoreAsWell()
        {

            var answer = await station!.OCPPv1_6Node.CertificateSources.InstallAsync(NameServer, "CPO", OCPPC.Certificate.Parse(root.ExportCertificatePem()), "CentralSystem");

            Assert.Multiple(() => {
                Assert.That(answer.Status,  Is.EqualTo(OCPPC.CertificateAnswerStatus.Accepted));
                Assert.That(station.OCPPv2_1Node.CertificateSources.Installed([ NameServer ]).Single().Certificate.Thumbprint,
                            Is.EqualTo(root.Thumbprint));
            });

        }

        #endregion

        #region ARootTheNamesOfTheCSMSResolveThroughIsKept()

        /// <summary>
        /// The one root a name server over TLS is believed through, while the
        /// CSMS is dialled by a name: deleting it would leave the name without
        /// a name server to resolve it, so it fails, and the root is kept
        /// (Secure DNS T21). Another root goes, as the name still resolves
        /// without it.
        /// </summary>
        [Test]
        public async Task ARootTheNamesOfTheCSMSResolveThroughIsKept()
        {

            using var serverCertificate = ServerCertificate();

            var server = new DNSServer(
                             new AuthoritativeDNSRequestHandler(
                                 new InMemoryDNSZone().
                                     Add(new A(
                                             DomainName. Parse("csms.example.test."),
                                             DNSQueryClasses.IN,
                                             TimeSpan.   FromMinutes(5),
                                             IPv4Address.Parse("127.0.0.42")
                                         ))
                             ),
                             new DNSServerOptions {
                                 EnableUDPUnicast      = false,
                                 EnableUDPMulticast    = false,
                                 EnableTCPUnicast      = false,
                                 EnableTLSUnicast      = true,
                                 TLSUnicastSocket      = new IPSocket(IPv4Address.Localhost, IPPort.Parse(0)),
                                 TLSServerCertificate  = serverCertificate
                             }
                         );

            await server.Start();

            try
            {

                var port = server.ActiveTLSUnicastSocket!.Value.Port;

                Assert.That(station!.Node.TryUpdateDNSConfiguration(JObject.Parse($$"""
                                { "servers": [ { "address": "127.0.0.1", "port": {{port.ToUInt16()}}, "transport": "TLS" } ], "maxRetries": 0 }
                                """), out var error),
                            Is.True, error);

                using var resolverRoot = X509CertificateLoader.LoadCertificate(serverCertificate.RawData);

                Import(resolverRoot, [ "dns" ]);
                Import(other,        [ "dns" ]);

                var sources  = new OCPPC.CertificateSources();
                sources.Add(new DNSRootCertificates(station.Node, () => [ "csms.example.test" ]));

                var kept     = await sources.DeleteAsync(OCPPC.CertificateHashes.Of(resolverRoot), "CSMS");
                var gone     = await sources.DeleteAsync(OCPPC.CertificateHashes.Of(other),        "CSMS");

                Assert.Multiple(() => {

                    Assert.That(kept.Status,              Is.EqualTo(OCPPC.CertificateAnswerStatus.Failed));
                    Assert.That(kept.ReasonCode,          Is.EqualTo("CSMSUnreachable"));
                    Assert.That(kept.AdditionalInfo,      Does.Contain("'csms.example.test' would not resolve").
                                                          And.Contains($"tls://127.0.0.1:{port} untrusted"));
                    Assert.That(TLSRoot(resolverRoot),    Is.Not.Null);

                    Assert.That(gone.Status,              Is.EqualTo(OCPPC.CertificateAnswerStatus.Accepted));
                    Assert.That(TLSRoot(other),           Is.Null);

                });

            }
            finally
            {
                await server.Stop();
            }

        }

        #endregion

        #region TheStationsNodesAskItsOwnSource()

        /// <summary>
        /// NameServerRootCertificate is the type of the node's own source on
        /// both OCPP nodes, beside the one for the time servers.
        /// </summary>
        [Test]
        public void TheStationsNodesAskItsOwnSource()
        {
            Assert.Multiple(() => {
                Assert.That(station!.OCPPv2_1Node.CertificateSources.For(NameServer),  Is.InstanceOf<DNSRootCertificates>());
                Assert.That(station. OCPPv1_6Node.CertificateSources.For(NameServer),  Is.InstanceOf<DNSRootCertificates>());
                Assert.That(station. OCPPv2_1Node.CertificateSources.For(NTSRootCertificates.CertificateType),  Is.InstanceOf<NTSRootCertificates>());
            });
        }

        #endregion


        #region (private) Install(Certificate, Group) / Import(Certificate, Usages) / TLSRoot(Certificate) / Logged(Since)

        private Task<OCPPC.CertificateAnswer> Install(X509Certificate2  Certificate,
                                                     String?           Group)

            => station!.OCPPv2_1Node.CertificateSources.InstallAsync(NameServer, Group, OCPPC.Certificate.Parse(Certificate.ExportCertificatePem()), "CSMS");

        /// <summary>
        /// A TLS root put into the store as the certificate page puts it.
        /// </summary>
        private void Import(X509Certificate2  Certificate,
                            String[]?         Usages)
        {
            Assert.That(station!.Node.Certificates.Import(Encoding.ASCII.GetBytes(Certificate.ExportCertificatePem()), CertificateKind.TLSRoot, null, null, Usages, out _, out var error),
                        Is.True,
                        error);
        }

        private CertificateEntry? TLSRoot(X509Certificate2 Certificate)

            => station!.Node.Certificates.ByKind(CertificateKind.TLSRoot).
                                     FirstOrDefault(entry => entry.Thumbprint == CertificateEntry.ThumbprintOf(Certificate));

        private String[] Logged(UInt64 Since)

            => [.. station!.Node.Log.Recent(100, Since, null).Select(entry => entry.Message)];

        #endregion

        #region (private static) Root(Name) / ServerCertificate()

        private static X509Certificate2 Root(String Name)
        {

            using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);

            var request = new CertificateRequest($"CN={Name}", key, HashAlgorithmName.SHA256);

            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
            request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, true));

            using var made = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(365));

            // Without its key: a root of the store must carry none.
            return X509CertificateLoader.LoadCertificate(made.RawData);

        }

        /// <summary>
        /// A certificate for the loopback that signed itself, with its key: the
        /// name server's own, and its root.
        /// </summary>
        private static X509Certificate2 ServerCertificate()
        {

            using var rsa  = RSA.Create(2048);

            var request    = new CertificateRequest("CN=localhost", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var names      = new SubjectAlternativeNameBuilder();

            names.AddDnsName  ("localhost");
            names.AddIpAddress(System.Net.IPAddress.Loopback);

            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, false));
            request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension([ new Oid("1.3.6.1.5.5.7.3.1") ], false));
            request.CertificateExtensions.Add(names.Build());

            using var created = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-5), DateTimeOffset.UtcNow.AddDays(1));

            return X509CertificateLoader.LoadPkcs12(created.Export(X509ContentType.Pfx), null, X509KeyStorageFlags.Exportable);

        }

        #endregion

    }

}
