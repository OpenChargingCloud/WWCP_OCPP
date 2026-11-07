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

using NUnit.Framework;

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.CentralSystem
{

    /// <summary>
    /// The root certificates a central system installs, lists and deletes at
    /// a charge point - the Security Extensions' messages, with the
    /// certificate group and the types the Secure Time Synchronization and
    /// Secure Name Resolution papers add - over the wire.
    /// </summary>
    [TestFixture]
    public class CertificateTests : AChargePointTests
    {

        #region (class) AKeepingSource

        /// <summary>
        /// A source of the types given, keeping what it is given in memory.
        /// </summary>
        private sealed class AKeepingSource(params String[] Types) : ICertificateSource
        {

            public List<(String Type, String? Group, X509Certificate2 Certificate)>  Kept  { get; } = [];

            public IEnumerable<String> CertificateTypes
                => Types;

            public Task<CertificateAnswer> InstallAsync(String CertificateType, String? CertificateGroup, X509Certificate2 Certificate, String? Sender, CancellationToken CancellationToken = default)
            {
                Kept.Add((CertificateType, CertificateGroup, Certificate));
                return Task.FromResult(CertificateAnswer.Accepted);
            }

            public IEnumerable<X509Certificate2> Installed(String CertificateType)
                => Kept.Where(kept => kept.Type == CertificateType).Select(kept => kept.Certificate);

            public Task<CertificateAnswer?> CheckDeletionAsync(CertificateHashes Hashes, CancellationToken CancellationToken = default)
                => Task.FromResult(Kept.Any(kept => Hashes.Names(kept.Certificate)) ? CertificateAnswer.Accepted : null);

            public Task<CertificateAnswer> DeleteAsync(CertificateHashes Hashes, String? Sender, CancellationToken CancellationToken = default)
            {
                Kept.RemoveAll(kept => Hashes.Names(kept.Certificate));
                return Task.FromResult(CertificateAnswer.Accepted);
            }

        }

        #endregion

        #region (helper) Root(Subject)

        private static X509Certificate2 Root(String Subject)
        {

            using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);

            var request = new System.Security.Cryptography.X509Certificates.CertificateRequest($"CN={Subject}", key, HashAlgorithmName.SHA256);

            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));

            return request.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddDays(1));

        }

        #endregion


        #region ARootOfAGroupIsInstalledListedAndDeleted_Test()

        /// <summary>
        /// A root for time servers in a group is installed, listed with the
        /// hashes OCPP names it by, and deleted by them - after which none is
        /// listed.
        /// </summary>
        [Test]
        public async Task ARootOfAGroupIsInstalledListedAndDeleted_Test()
        {

            using var root     = Root("Legal Time CA");
            var hashes         = CertificateHashes.Of(root);
            var centralSystem  = testCentralSystem01!;
            var to             = SourceRouting.To(chargePoint1!.Id);

            var installed      = await centralSystem.InstallCertificate(to, CertificateUse.NetworkTimeRootCertificate,
                                                                         Certificate.Parse(root.ExportCertificatePem()),
                                                                         CertificateGroup.Parse("legalCA"));

            var listed         = await centralSystem.GetInstalledCertificateIds(to, CertificateUse.NetworkTimeRootCertificate);
            var otherType      = await centralSystem.GetInstalledCertificateIds(to, CertificateUse.NameServerRootCertificate);

            var deleted        = await centralSystem.DeleteCertificate(to, new CertificateHashData(HashAlgorithms.SHA256,
                                                                                                     hashes.IssuerNameHash,
                                                                                                     hashes.IssuerKeyHash,
                                                                                                     hashes.SerialNumber));

            var after          = await centralSystem.GetInstalledCertificateIds(to, CertificateUse.NetworkTimeRootCertificate);

            Assert.Multiple(() => {

                Assert.That(installed.Status,  Is.EqualTo(CertificateStatus.Accepted));

                Assert.That(listed.Status,     Is.EqualTo(GetInstalledCertificateStatus.Accepted));
                Assert.That(listed.CertificateHashData.Single().SerialNumber,         Is.EqualTo(hashes.SerialNumber));
                Assert.That(listed.CertificateHashData.Single().IssuerPublicKeyHash,  Is.EqualTo(hashes.IssuerKeyHash));

                Assert.That(otherType.Status,  Is.EqualTo(GetInstalledCertificateStatus.NotFound));

                Assert.That(deleted.Status,    Is.EqualTo(DeleteCertificateStatus.Accepted));
                Assert.That(after.Status,      Is.EqualTo(GetInstalledCertificateStatus.NotFound));

            });

        }

        #endregion

        #region ASourceOfATypeIsHandedItsRootsWithTheirGroup_Test()

        /// <summary>
        /// A source added for the roots of name servers is handed those, with
        /// their group, and the root of another type stays in memory.
        /// </summary>
        [Test]
        public async Task ASourceOfATypeIsHandedItsRootsWithTheirGroup_Test()
        {

            var source = new AKeepingSource("NameServerRootCertificate");

            chargePoint1!.CertificateSources.Add(source);

            using var names    = Root("Resolver CA");
            using var time     = Root("Time CA");
            var centralSystem  = testCentralSystem01!;
            var to             = SourceRouting.To(chargePoint1.Id);

            await centralSystem.InstallCertificate(to, CertificateUse.NameServerRootCertificate,  Certificate.Parse(names.ExportCertificatePem()), CertificateGroup.Parse("resolvers"));
            await centralSystem.InstallCertificate(to, CertificateUse.NetworkTimeRootCertificate, Certificate.Parse(time. ExportCertificatePem()));

            Assert.Multiple(() => {
                Assert.That(source.Kept.Select(kept => (kept.Type, kept.Group, kept.Certificate.Thumbprint)),
                            Is.EqualTo(new[] { ("NameServerRootCertificate", (String?) "resolvers", names.Thumbprint) }));
                Assert.That(chargePoint1.CertificateSources.Default.Installed("NetworkTimeRootCertificate").Single().Thumbprint,
                            Is.EqualTo(time.Thumbprint));
            });

        }

        #endregion

    }

}
