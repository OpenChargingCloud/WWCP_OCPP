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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node.Certificates;

using cloud.charging.open.protocols.OCPP.Node;

using OCPPC  = cloud.charging.open.protocols.OCPP;
using OCPP21 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// The roots for time servers a CSMS installs, lists and deletes as
    /// NetworkTimeRootCertificate, against the node's certificate store - and
    /// RootCAs, which holds a server to the roots of one group of them.
    /// </summary>
    /// <remarks>
    /// Through the OCPP nodes handed to the bridge, built and never started, as in
    /// NTSDeviceModelWritingTests; the node is the same one.
    /// </remarks>
        public abstract class NTSRootCertificatesTests : ABridgeTests
    {

        #region Data

        private const String NetworkTime = NTSRootCertificates.CertificateType;

        private String            directory  = "";
        private IBridgedNode?  station;
        private X509Certificate2  root       = default!;
        private X509Certificate2  other      = default!;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void AStationWithALegalGroup()
        {

            directory  = TestNodes.TemporaryDirectory("nts-root-certificates");
            station    = Nodes.New(directory, Configuration());
            root       = Root("Legal Time CA");
            other      = Root("Another Time CA");

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


        #region ARootInstalledInAGroupIsATimeServerRootMarkedForIt()

        /// <summary>
        /// A root installed in a group is a TLS root of the store for the time
        /// servers, marked for its group - and written into the metrological
        /// log with the CSMS that installed it.
        /// </summary>
        [Test]
        public async Task ARootInstalledInAGroupIsATimeServerRootMarkedForIt()
        {

            var before  = station!.Node.Log.LastId;
            var answer  = await Install(root, "LegalCA");
            var entry   = TLSRoot(root);

            Assert.Multiple(() => {
                Assert.That(answer.Status,                                                Is.EqualTo(OCPPC.CertificateAnswerStatus.Accepted));
                Assert.That(entry?.Usages?.Select(usage => usage.ToString()),           Is.EquivalentTo(new[] { "nts", "ntsca-legalca" }));
                Assert.That(Metrological(before),                                         Has.Some.Contains("NetworkTimeRootCertificate").
                                                                                              And.Contains("in group LegalCA").
                                                                                              And.Contains("by ocpp:CSMS"));
            });

        }

        #endregion

        #region TheSameRootInAnotherGroupIsMarkedForBoth()

        /// <summary>
        /// The same root installed in a second group is one root marked for
        /// both, and installed in no group it stays marked as it was.
        /// </summary>
        [Test]
        public async Task TheSameRootInAnotherGroupIsMarkedForBoth()
        {

            await Install(root, "legalCA");
            await Install(root, "backupCA");
            await Install(root, null);

            Assert.Multiple(() => {
                Assert.That(station!.Node.Certificates.ByKind(CertificateKind.TLSRoot).Count,     Is.EqualTo(1));
                Assert.That(TLSRoot(root)?.Usages?.Select(usage => usage.ToString()),       Is.EquivalentTo(new[] { "nts", "ntsca-legalca", "ntsca-backupca" }));
            });

        }

        #endregion

        #region WhatCannotBeARootOfAGroupIsRejectedAndNothingIsKept()

        /// <summary>
        /// A group the store cannot mark, and a certificate that is no root,
        /// are rejected with why - and the store keeps nothing of either.
        /// </summary>
        [Test]
        public async Task WhatCannotBeARootOfAGroupIsRejectedAndNothingIsKept()
        {

            using var leaf = Leaf("time.example.invalid", root);

            var group    = await Install(root, "legal CA");
            var notRoot  = await Install(leaf, null);

            Assert.Multiple(() => {
                Assert.That(group.Status,             Is.EqualTo(OCPPC.CertificateAnswerStatus.Rejected));
                Assert.That(group.AdditionalInfo,     Does.Contain("cannot name a group of roots"));
                Assert.That(notRoot.Status,           Is.EqualTo(OCPPC.CertificateAnswerStatus.Rejected));
                Assert.That(notRoot.ReasonCode,       Is.EqualTo("InvalidCertificate"));
                Assert.That(station!.Node.Certificates.Entries,  Is.Empty);
            });

        }

        #endregion

        #region TheRootsThatVouchForTimeServersAreListed()

        /// <summary>
        /// Listed are the roots that vouch for time servers, whoever put them
        /// into the store: those for the time servers, and those for every use
        /// - and not those for the name servers alone.
        /// </summary>
        [Test]
        public void TheRootsThatVouchForTimeServersAreListed()
        {

            using var everyUse  = Root("Every Use CA");
            using var names     = Root("Resolver CA");

            Import(root,     [ "nts" ]);
            Import(everyUse, null);
            Import(names,    [ "dns" ]);

            var listed = station!.OCPPv2_1Node.CertificateSources.Installed([ NetworkTime ]).
                                                                  Select(installed => installed.Certificate.Thumbprint);

            Assert.That(listed,  Is.EquivalentTo(new[] { root.Thumbprint, everyUse.Thumbprint }));

        }

        #endregion

        #region ARootIsDeletedForTheTimeServersAndStaysForAnotherService()

        /// <summary>
        /// A root for the time servers alone is deleted; one kept for the
        /// meters as well loses the time servers and its marks and stays; one
        /// for every use is not deleted for the time servers alone, and stays.
        /// </summary>
        [Test]
        public async Task ARootIsDeletedForTheTimeServersAndStaysForAnotherService()
        {

            using var everyUse = Root("Every Use CA");

            Import(root,     [ "nts", "ntsca-legalca" ]);
            Import(other,    [ "nts", "ntsca-legalca", "meter" ]);
            Import(everyUse, null);

            var sources  = station!.OCPPv2_1Node.CertificateSources;
            var before   = station.Node.Log.LastId;

            var alone    = await sources.DeleteAsync(OCPPC.CertificateHashes.Of(root),     "CSMS");
            var shared   = await sources.DeleteAsync(OCPPC.CertificateHashes.Of(other),    "CSMS");
            var forAll   = await sources.DeleteAsync(OCPPC.CertificateHashes.Of(everyUse), "CSMS");

            Assert.Multiple(() => {

                Assert.That(alone.Status,     Is.EqualTo(OCPPC.CertificateAnswerStatus.Accepted));
                Assert.That(TLSRoot(root),    Is.Null);

                Assert.That(shared.Status,    Is.EqualTo(OCPPC.CertificateAnswerStatus.Accepted));
                Assert.That(TLSRoot(other)?.Usages?.Select(usage => usage.ToString()),  Is.EqualTo(new[] { "meter" }));

                Assert.That(forAll.Status,      Is.EqualTo(OCPPC.CertificateAnswerStatus.Failed));
                Assert.That(forAll.ReasonCode,  Is.EqualTo("ForEveryUse"));
                Assert.That(TLSRoot(everyUse),  Is.Not.Null);

                Assert.That(Metrological(before),  Has.Exactly(2).Contains("deleted by ocpp:CSMS"));

            });

        }

        #endregion

        #region TheChargePointKeepsItsTimeServerRootsInTheStoreAsWell()

        /// <summary>
        /// A root installed through OCPP 1.6 lands in the same store, and is
        /// listed through OCPP 2.1.
        /// </summary>
        [Test]
        public async Task TheChargePointKeepsItsTimeServerRootsInTheStoreAsWell()
        {

            var answer = await station!.OCPPv1_6Node.CertificateSources.InstallAsync(NetworkTime, "legalCA", OCPPC.Certificate.Parse(root.ExportCertificatePem()), "CentralSystem");

            Assert.Multiple(() => {
                Assert.That(answer.Status,  Is.EqualTo(OCPPC.CertificateAnswerStatus.Accepted));
                Assert.That(station.OCPPv2_1Node.CertificateSources.Installed([ NetworkTime ]).Single().Certificate.Thumbprint,
                            Is.EqualTo(root.Thumbprint));
            });

        }

        #endregion

        #region RootCAsIsTheGroupOfRootsAServerIsHeldTo()

        /// <summary>
        /// RootCAs is the group of roots a server is held to: written into its
        /// entry beside its pins, read back as it was written, taken away by an
        /// empty value - and refused where the store could not mark a group of
        /// that name.
        /// </summary>
        [Test]
        public async Task RootCAsIsTheGroupOfRootsAServerIsHeldTo()
        {

            var set      = await Set(Client("ptb-a", "RootCAs", "legalCA"));
            var written  = Server("ptb-a");
            var read     = Get("ptb-a", "RootCAs");

            var wrong    = await Set(Client("ptb-a", "RootCAs", "legal CA"));
            var emptied  = await Set(Client("ptb-a", "RootCAs", ""));

            Assert.Multiple(() => {

                Assert.That(set.Single().AttributeStatus,         Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(written?.Value<String>("rootGroup"),  Is.EqualTo("legalCA"));
                Assert.That(written?.Value<String>("rootFingerprint"),  Is.Not.Null,  "the pin beside it");
                Assert.That(read.AttributeValue,                  Is.EqualTo("legalCA"));

                Assert.That(wrong.Single().AttributeStatus,       Is.EqualTo(OCPP21.SetVariableStatus.Rejected));
                Assert.That(wrong.Single().AttributeStatusInfo?.AdditionalInfo,  Does.Contain("cannot name a group of roots here"),  "refused by the station, before the node is asked");
                Assert.That(emptied.Single().AttributeStatus,     Is.EqualTo(OCPP21.SetVariableStatus.Accepted));
                Assert.That(Server("ptb-a")?.Value<String>("rootGroup"),  Is.Null);

            });

        }

        #endregion


        #region (private) Install(Certificate, Group) / Import(Certificate, Usages) / TLSRoot(Certificate) / Metrological(Since)

        private Task<OCPPC.CertificateAnswer> Install(X509Certificate2  Certificate,
                                                     String?           Group)

            => station!.OCPPv2_1Node.CertificateSources.InstallAsync(NetworkTime, Group, OCPPC.Certificate.Parse(Certificate.ExportCertificatePem()), "CSMS");

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

        private String[] Metrological(UInt64 Since)

            => [.. station!.Node.Log.Recent(100, Since, null).Where(entry => entry.Metrological).Select(entry => entry.Message)];

        #endregion

        #region (private) Set(Entries) / Get(Client, Variable) / Server(Id)

        private sealed record Entry(String Client, String Variable, String Value);

        private static Entry Client(String Client, String Variable, String Value)
            => new (Client, Variable, Value);

        private async Task<OCPP21.SetVariableResult[]> Set(params Entry[] Entries)
        {

            var node = station!.OCPPv2_1Node;

            return [.. await OCPP21.DeviceModel.SetVariablesAsync(
                                 new OCPP21.CSMS.SetVariablesRequest(
                                     Destination:   SourceRouting.To(node.Id),
                                     VariableData:  [.. Entries.Select(entry => new OCPP21.SetVariableData(
                                                                                    Component:       new OCPP21.Component(Name: "NTPClientController", Instance: entry.Client),
                                                                                    Variable:        new OCPP21.Variable (Name: entry.Variable),
                                                                                    AttributeValue:  entry.Value
                                                                                ))]
                                 ),
                                 (name, evse) => node.GetComponentConfigs(name, evse),
                                 default,
                                 node.NewInstanceOf)];

        }

        private OCPP21.GetVariableResult Get(String  Client,
                                             String  Variable)
        {

            var node = station!.OCPPv2_1Node;

            return OCPP21.DeviceModel.GetVariables([ new OCPP21.GetVariableData(new OCPP21.Component("NTPClientController", Client), new OCPP21.Variable(Variable)) ],
                                                   (name, evse) => node.GetComponentConfigs(name, evse)).Single();

        }

        /// <summary>
        /// A server of the legal group as the configuration file says it now.
        /// </summary>
        private JObject? Server(String Id)

            => (JObject.Parse(File.ReadAllText(station!.Node.ConfigFile.Path))["nts"]?["groups"]?["legal"]?["servers"] as JArray)?.
                   OfType<JObject>().
                   FirstOrDefault(server => server.Value<String>("id") == Id);

        #endregion

        #region (private static) Root(Name) / Leaf(Name, Issuer) / Configuration()

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

        private static X509Certificate2 Leaf(String            Name,
                                             X509Certificate2  Issuer)
        {

            using var issuerKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            using var key       = ECDsa.Create(ECCurve.NamedCurves.nistP256);

            // An issuer of its own, as the root above carries no key to sign with.
            var issuerRequest   = new CertificateRequest(Issuer.SubjectName, issuerKey, HashAlgorithmName.SHA256);

            issuerRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));

            using var issuer    = issuerRequest.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(365));

            var request         = new CertificateRequest($"CN={Name}", key, HashAlgorithmName.SHA256);

            using var made      = request.Create(issuer, DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(90), Guid.NewGuid().ToByteArray());

            return X509CertificateLoader.LoadCertificate(made.RawData);

        }

        /// <summary>
        /// A legal group of one server, given an id and pinned to a root.
        /// </summary>
        private static JObject Configuration()
        {

            var configuration = TestNodes.Offline;

            configuration["nts"] = JObject.Parse("""
                                       { "groups": { "legal": { "enabled": false, "servers": [
                                           { "id": "ptb-a", "hostname": "ptbtime1.example.invalid",
                                             "rootFingerprint": "9d2e4f6a8b0c1d3e5f7a9b1c3d5e7f9a0b2c4d6e8f0a1b3c5d7e9f1a2b4c6d8e" },
                                           "ptbtime2.example.invalid" ] } } }
                                       """);

            return configuration;

        }

        #endregion

    }

}
