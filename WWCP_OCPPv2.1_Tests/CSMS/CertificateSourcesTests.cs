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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

using CertificateAnswer   = cloud.charging.open.protocols.OCPP.CertificateAnswer;
using CertificateHashes   = cloud.charging.open.protocols.OCPP.CertificateHashes;
using ICertificateSource  = cloud.charging.open.protocols.OCPP.ICertificateSource;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// The root certificates a CSMS installs, lists and deletes at a charging
    /// station, over the wire: several per type and per group, named by the
    /// hashes OCPP names them by, kept by a source of their own where one was
    /// added for their type - and deleted from every source that has them, or
    /// from none.
    /// </summary>
    [TestFixture]
    public class CertificateSourcesTests : AChargingStationTests
    {

        #region (class) AKeepingSource

        /// <summary>
        /// A source of the types given, keeping what it is given in memory and
        /// remembering who sent what - and failing every deletion where it is
        /// told to.
        /// </summary>
        private sealed class AKeepingSource(params String[] Types) : ICertificateSource
        {

            public List<(String Type, String? Group, X509Certificate2 Certificate, String? Sender)>  Kept      { get; } = [];
            public Boolean                                                                           Refuses   { get; set; }

            public IEnumerable<String> CertificateTypes
                => Types;

            public Task<CertificateAnswer> InstallAsync(String CertificateType, String? CertificateGroup, X509Certificate2 Certificate, String? Sender, CancellationToken CancellationToken = default)
            {
                Kept.Add((CertificateType, CertificateGroup, Certificate, Sender));
                return Task.FromResult(CertificateAnswer.Accepted);
            }

            public IEnumerable<X509Certificate2> Installed(String CertificateType)
                => Kept.Where(kept => kept.Type == CertificateType).Select(kept => kept.Certificate);

            public CertificateAnswer? CheckDeletion(CertificateHashes Hashes)
                => Kept.Any(kept => Hashes.Names(kept.Certificate))
                       ? Refuses
                             ? CertificateAnswer.Failed("InUse", "A configured server's chain ends at it.")
                             : CertificateAnswer.Accepted
                       : null;

            public Task<CertificateAnswer> DeleteAsync(CertificateHashes Hashes, String? Sender, CancellationToken CancellationToken = default)
            {
                Kept.RemoveAll(kept => Hashes.Names(kept.Certificate));
                return Task.FromResult(CertificateAnswer.Accepted);
            }

        }

        #endregion

        #region (helpers) Root(Subject, Serial) / Install(...) / Installed(...) / Delete(...)

        /// <summary>
        /// A self-signed root, with the given serial number where one is given.
        /// </summary>
        private static X509Certificate2 Root(String   Subject,
                                             Byte[]?  Serial = null)
        {

            using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);

            var request = new System.Security.Cryptography.X509Certificates.CertificateRequest($"CN={Subject}", key, HashAlgorithmName.SHA256);

            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));

            if (Serial is null)
                return request.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddDays(1));

            using var made = request.Create(new X500DistinguishedName($"CN={Subject}"),
                                            X509SignatureGenerator.CreateForECDsa(key),
                                            DateTimeOffset.UtcNow.AddMinutes(-1),
                                            DateTimeOffset.UtcNow.AddDays(1),
                                            Serial);

            return X509CertificateLoader.LoadCertificate(made.RawData);

        }

        private Task<InstallCertificateResponse> Install(InstallCertificateUse  Type,
                                                         X509Certificate2       Certificate,
                                                         String?                Group = null)

            => testCSMS1!.InstallCertificate(
                   Destination:       SourceRouting.To(chargingStation1!.Id),
                   CertificateType:   Type,
                   Certificate:       OCPP.Certificate.Parse(Certificate.ExportCertificatePem()),
                   CertificateGroup:  Group is null ? null : CertificateGroup.Parse(Group)
               );

        private Task<GetInstalledCertificateIdsResponse> Installed(params GetCertificateIdUse[] Types)

            => testCSMS1!.GetInstalledCertificateIds(
                   Destination:       SourceRouting.To(chargingStation1!.Id),
                   CertificateTypes:  Types
               );

        private Task<DeleteCertificateResponse> Delete(CertificateHashData Hashes)

            => testCSMS1!.DeleteCertificate(
                   Destination:          SourceRouting.To(chargingStation1!.Id),
                   CertificateHashData:  Hashes
               );

        private static CertificateHashData HashDataOf(X509Certificate2 Certificate)
        {

            var hashes = CertificateHashes.Of(Certificate);

            return new CertificateHashData(HashAlgorithm.SHA256, hashes.IssuerNameHash, hashes.IssuerKeyHash, hashes.SerialNumber);

        }

        #endregion


        #region SeveralRootsOfATypeAreListedEachWithItsHashes_Test()

        /// <summary>
        /// Two roots of one type in two groups and a root of another type are
        /// each listed with their own type; the hashes are those of OCSP - the
        /// issuer's name and key as BouncyCastle reads them out of the
        /// certificate, hashed - and the same certificate again, in the same
        /// group or in another one, is listed once.
        /// </summary>
        [Test]
        public async Task SeveralRootsOfATypeAreListedEachWithItsHashes_Test()
        {

            using var legal   = Root("Legal Time CA");
            using var backup  = Root("Backup Time CA");
            using var v2g     = Root("V2G Root");

            var answers = new[] {
                              await Install(InstallCertificateUse.NetworkTimeRootCertificate, legal,  "legalCA"),
                              await Install(InstallCertificateUse.NetworkTimeRootCertificate, legal,  "legalCA"),
                              await Install(InstallCertificateUse.NetworkTimeRootCertificate, legal,  "backupCA"),
                              await Install(InstallCertificateUse.NetworkTimeRootCertificate, backup, "backupCA"),
                              await Install(InstallCertificateUse.V2GRootCertificate,         v2g)
                          };

            var time    = await Installed(GetCertificateIdUse.NetworkTimeRootCertificate);
            var kept    = chargingStation1!.CertificateSources.Installed([ "NetworkTimeRootCertificate" ]);
            var all     = await Installed();

            var bc      = new Org.BouncyCastle.X509.X509CertificateParser().ReadCertificate(legal.RawData);
            var listed  = time.CertificateHashDataChain.Select(chain => chain.CertificateHashData).
                                                        Single(hashData => hashData.SerialNumber == bc.SerialNumber.ToString(16));

            Assert.Multiple(() => {

                Assert.That(answers.Select(answer => answer.Status),  Has.All.EqualTo(InstallCertificateStatus.Accepted));

                Assert.That(time.Status,                              Is.EqualTo(GetInstalledCertificateStatus.Accepted));
                Assert.That(time.CertificateHashDataChain.Count(),    Is.EqualTo(2),  "the same root twice in one group, and in two groups, is listed once");
                Assert.That(kept.Count,                               Is.EqualTo(2),  "by the source, and not only in the response");
                Assert.That(time.CertificateHashDataChain.Select(chain => chain.CertificateType),
                            Has.All.EqualTo(GetCertificateIdUse.NetworkTimeRootCertificate));

                Assert.That(listed.HashAlgorithm,     Is.EqualTo(HashAlgorithm.SHA256));
                Assert.That(listed.IssuerNameHash,    Is.EqualTo(Convert.ToHexStringLower(SHA256.HashData(bc.IssuerDN.GetEncoded()))));
                Assert.That(listed.IssuerPublicKeyHash,
                            Is.EqualTo(Convert.ToHexStringLower(SHA256.HashData(bc.CertificateStructure.SubjectPublicKeyInfo.PublicKey.GetBytes()))));

                Assert.That(all.CertificateHashDataChain.Select(chain => chain.CertificateType.ToString()).Order(),
                            Is.EqualTo(new[] { "NetworkTimeRootCertificate", "NetworkTimeRootCertificate", "V2GRootCertificate" }),
                            "a request naming no type lists every type");

            });

        }

        #endregion

        #region ASerialNumberIsWrittenWithoutItsLeadingZeros_Test()

        /// <summary>
        /// A serial number is written in lower case and without the leading
        /// zeros OCPP leaves out - and the certificate is found by it again,
        /// with them or without.
        /// </summary>
        [Test]
        public async Task ASerialNumberIsWrittenWithoutItsLeadingZeros_Test()
        {

            using var root = Root("Leading Zeros CA", [ 0x00, 0x0A, 0xBC, 0xDE ]);

            await Install(InstallCertificateUse.V2GRootCertificate, root);

            var listed    = (await Installed(GetCertificateIdUse.V2GRootCertificate)).CertificateHashDataChain.Single().CertificateHashData;
            var deleted   = await Delete(new CertificateHashData(listed.HashAlgorithm, listed.IssuerNameHash.ToUpperInvariant(), listed.IssuerPublicKeyHash, "000ABCDE"));
            var after     = await Installed(GetCertificateIdUse.V2GRootCertificate);

            Assert.Multiple(() => {
                Assert.That(listed.SerialNumber,  Is.EqualTo("abcde"));
                Assert.That(deleted.Status,       Is.EqualTo(DeleteCertificateStatus.Accepted));
                Assert.That(after.Status,         Is.EqualTo(GetInstalledCertificateStatus.NotFound));
            });

        }

        #endregion

        #region WhatCannotBeInstalledIsRejectedAndKeepsNothing_Test()

        /// <summary>
        /// A group longer than OCPP takes is rejected with why, and nothing of it
        /// is kept.
        /// </summary>
        [Test]
        public async Task WhatCannotBeInstalledIsRejectedAndKeepsNothing_Test()
        {

            using var root = Root("Too Long A Group CA");

            var response = await Install(InstallCertificateUse.NetworkTimeRootCertificate, root, new String('g', 51));
            var listed   = await Installed(GetCertificateIdUse.NetworkTimeRootCertificate);

            Assert.Multiple(() => {
                Assert.That(response.Status,                Is.EqualTo(InstallCertificateStatus.Rejected));
                Assert.That(response.StatusInfo?.ReasonCode, Is.EqualTo("InvalidValue"));
                Assert.That(listed.Status,                  Is.EqualTo(GetInstalledCertificateStatus.NotFound));
            });

        }

        #endregion

        #region ASourceOfATypeKeepsItsCertificates_Test()

        /// <summary>
        /// A source added for a type is handed its certificates with their group
        /// and the CSMS that sent them, and is what is listed for that type; the
        /// other types stay in memory.
        /// </summary>
        [Test]
        public async Task ASourceOfATypeKeepsItsCertificates_Test()
        {

            var source = new AKeepingSource("NetworkTimeRootCertificate");

            chargingStation1!.CertificateSources.Add(source);

            using var legal  = Root("Legal Time CA");
            using var v2g    = Root("V2G Root");

            await Install(InstallCertificateUse.NetworkTimeRootCertificate, legal, "legalCA");
            await Install(InstallCertificateUse.V2GRootCertificate,         v2g);

            var time = await Installed(GetCertificateIdUse.NetworkTimeRootCertificate);

            Assert.Multiple(() => {
                Assert.That(source.Kept.Select(kept => (kept.Type, kept.Group, kept.Certificate.Thumbprint)),
                            Is.EqualTo(new[] { ("NetworkTimeRootCertificate", (String?) "legalCA", legal.Thumbprint) }));
                Assert.That(source.Kept.Single().Sender,                                         Is.EqualTo("CSMS"),  "the CSMS, which a request straight from it does not name");
                Assert.That(time.CertificateHashDataChain.Single().CertificateHashData.SerialNumber,
                            Is.EqualTo(CertificateHashes.Of(legal).SerialNumber));
                Assert.That(chargingStation1.CertificateSources.Default.Installed("V2GRootCertificate").Single().Thumbprint,
                            Is.EqualTo(v2g.Thumbprint));
            });

        }

        #endregion

        #region ARootKeptByTwoSourcesIsDeletedFromBothOrFromNeither_Test()

        /// <summary>
        /// One root kept for two types by two sources: a deletion that one of
        /// them refuses is failed and leaves it with both, and one neither
        /// refuses takes it from both.
        /// </summary>
        [Test]
        public async Task ARootKeptByTwoSourcesIsDeletedFromBothOrFromNeither_Test()
        {

            var time  = new AKeepingSource("NetworkTimeRootCertificate");
            var names = new AKeepingSource("NameServerRootCertificate") { Refuses = true };

            chargingStation1!.CertificateSources.Add(time);
            chargingStation1. CertificateSources.Add(names);

            using var root = Root("Both Services CA");

            await Install(InstallCertificateUse.NetworkTimeRootCertificate, root);
            await Install(InstallCertificateUse.NameServerRootCertificate,  root);

            var refused   = await Delete(HashDataOf(root));
            var keptBy    = (time.Kept.Count, names.Kept.Count);

            names.Refuses = false;

            var deleted   = await Delete(HashDataOf(root));
            var again     = await Delete(HashDataOf(root));

            Assert.Multiple(() => {
                Assert.That(refused.Status,                 Is.EqualTo(DeleteCertificateStatus.Failed));
                Assert.That(refused.StatusInfo?.ReasonCode, Is.EqualTo("InUse"));
                Assert.That(keptBy,                         Is.EqualTo((1, 1)),  "a refused deletion took the root from a source");
                Assert.That(deleted.Status,                 Is.EqualTo(DeleteCertificateStatus.Accepted));
                Assert.That((time.Kept.Count, names.Kept.Count),  Is.EqualTo((0, 0)));
                Assert.That(again.Status,                   Is.EqualTo(DeleteCertificateStatus.NotFound));
            });

        }

        #endregion

    }

}
