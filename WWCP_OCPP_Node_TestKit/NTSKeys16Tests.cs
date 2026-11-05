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

using NUnit.Framework;

using cloud.charging.open.protocols.OCPP.Node;

using OCPP16 = cloud.charging.open.protocols.OCPPv1_6;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// The configuration keys of OCPP 1.6 for the time servers (Secure Time
    /// Synchronization, section 3.3), as the node's OCPP 1.6 node answers
    /// them: read from the node, and changed as OCPP 2.1 changes them.
    /// </summary>
    /// <remarks>
    /// The node is much the one of NTSDeviceModelWritingTests: a legal
    /// group of four - one given an id, pinned to a root and asked after the
    /// others, one on a port of its own, and the last switched off and on a
    /// port of its own as well - and a local group of one, built and never
    /// started.
    /// </remarks>
        public abstract class NTSKeys16Tests : ABridgeTests
    {

        #region Data

        private const String  ARoot       = "9d2e4f6a8b0c1d3e5f7a9b1c3d5e7f9a0b2c4d6e8f0a1b3c5d7e9f1a2b4c6d8e";
        private const String  APin        = "0f1e2d3c4b5a69788796a5b4c3d2e1f00f1e2d3c4b5a69788796a5b4c3d2e1f0";

        private String            directory  = "";
        private IBridgedNode?  station;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void AStationWithALegalAndALocalGroup()
        {

            directory  = TestNodes.TemporaryDirectory("nts-keys16");
            station    = Nodes.New(directory, Configuration());

        }

        [TearDown]
        public async Task TakeItAwayAgain()
        {

            if (station is not null)
                await station.DisposeAsync();

            station = null;

            TestNodes.Remove(directory);

        }

        #endregion


        private OCPP16.CP.IConfigurationKeySource Keys
            => station!.OCPPv1_6Node.ConfigurationKeySources.OfType<NTSDeviceModel>().Single();

        private String? Value(String Key)
            => Keys.Get(Key)?.Value;

        private Task<OCPP16.ConfigurationStatus?> Change(String Key, String Value)
            => Keys.ChangeAsync(Key, Value, "CS-16");


        #region TheKeysOfEveryGroupAreReadFromTheNode()

        /// <summary>
        /// Every group has the keys of the paper the node can answer, the legal
        /// group those of legal time as well, and one pair of pin keys per
        /// server; a list is per server in the order of the file, and a key is
        /// found whatever its case and named as the paper writes it.
        /// </summary>
        [Test]
        public void TheKeysOfEveryGroupAreReadFromTheNode()
        {

            var keys = Keys.Keys.ToArray();

            Assert.Multiple(() => {

                Assert.That(keys,  Is.SupersetOf(new[] { "ntp.legal.minServers", "ntp.legal.authority", "ntske.legal.certificatePins.4", "ntp.local.servers" }));
                Assert.That(keys,  Has.None.EqualTo("ntske.legal.certificatePins.5").And.None.EqualTo("ntp.local.authority").And.None.EqualTo("ntp.legal.delayAsym"));

                Assert.That(Value("ntp.legal.servers"),         Is.EqualTo("ptbtime1.example.invalid,ptbtime2.example.invalid,ptbtime3.example.invalid,ptbtime4.example.invalid"));
                Assert.That(Value("ntske.legal.ports"),         Is.EqualTo("4460,4460,4461,4462"));
                Assert.That(Value("ntp.legal.priority"),        Is.EqualTo("1,0,0,0"));
                Assert.That(Value("ntske.legal.onPinMismatch"), Is.EqualTo("Record,Refuse,Refuse,Refuse"));
                Assert.That(Value("ntske.legal.rootPins.1"),    Is.EqualTo(ARoot));
                Assert.That(Value("ntp.legal.minServers"),      Is.EqualTo("2"));
                Assert.That(Value("ntp.legal.mode"),            Is.EqualTo("NTSv4"));

                Assert.That(Keys.Get("NTP.LEGAL.MINSERVERS")?.Key,  Is.EqualTo("ntp.legal.minServers"));
                Assert.That(Keys.Get("ntp.legal.delayAsym"),        Is.Null);
                Assert.That(Keys.Get("ntske.legal.rootPins.5"),     Is.Null);
                Assert.That(Keys.Get("ntp.grid.minServers"),        Is.Null,  "a group the station does not have");

            });

        }

        #endregion

        #region AKeyIsChangedAsOCPP21ChangesItsVariable()

        /// <summary>
        /// A key is changed as OCPP 2.1 changes the variable it stands for: in
        /// effect, in the file, named with the central system - and refused
        /// where 2.1 refuses it. minInterval alone sets the one interval.
        /// </summary>
        [Test]
        public async Task AKeyIsChangedAsOCPP21ChangesItsVariable()
        {

            var before    = station!.Node.Log.LastId;

            var three     = await Change("ntp.legal.minServers",  "3");
            var none      = await Change("ntp.legal.minServers",  "0");
            var interval  = await Change("ntp.legal.minInterval", "600");
            var mode      = await Change("ntp.legal.mode",        "NTPv4");
            var nts       = await Change("ntp.legal.mode",        "ntsv4");

            Assert.Multiple(() => {

                Assert.That((three, none, interval),          Is.EqualTo((OCPP16.ConfigurationStatus.Accepted, OCPP16.ConfigurationStatus.Rejected, OCPP16.ConfigurationStatus.Accepted)));
                Assert.That((mode, nts),                      Is.EqualTo((OCPP16.ConfigurationStatus.Rejected, OCPP16.ConfigurationStatus.Accepted)));

                Assert.That(Value("ntp.legal.minServers"),    Is.EqualTo("3"));
                Assert.That(OnDisk()?["minServers"]?.Value<Int32>(),         Is.EqualTo(3));
                Assert.That(Value("ntp.legal.maxInterval"),   Is.EqualTo("600"),  "the one interval the node checks at");

                Assert.That(station.Node.Log.Recent(100, before, null).Select(entry => entry.Message),
                            Has.Some.Contains("(by ocpp:CS-16)"));

            });

        }

        #endregion

        #region AListIsReadAsThePaperReadsIt()

        /// <summary>
        /// A list shorter than the servers holds its last value for the rest, an
        /// empty entry is the one before it, and a longer list is refused.
        /// </summary>
        [Test]
        public async Task AListIsReadAsThePaperReadsIt()
        {

            var one     = await Change("ntske.legal.ports", "4470");
            var afterOne = Value("ntske.legal.ports");

            var gaps    = await Change("ntske.legal.ports", "4471,,4472");
            var afterGaps = Value("ntske.legal.ports");

            var longer  = await Change("ntske.legal.ports", "1,2,3,4,5");

            Assert.Multiple(() => {
                Assert.That((one, gaps, longer),  Is.EqualTo((OCPP16.ConfigurationStatus.Accepted, OCPP16.ConfigurationStatus.Accepted, OCPP16.ConfigurationStatus.Rejected)));
                Assert.That(afterOne,             Is.EqualTo("4470,4470,4470,4470"));
                Assert.That(afterGaps,            Is.EqualTo("4471,4471,4472,4472"));
                Assert.That(Value("ntske.legal.ports"),  Is.EqualTo("4471,4471,4472,4472"),  "a refused list changed something");
            });

        }

        #endregion

        #region TheAEADAlgorithmIsAListWhoseEmptyEntryOffersEveryOne()

        /// <summary>
        /// "ntske.&lt;group&gt;.aead" is a list per server like the others, but
        /// for an empty entry: a server whose key exchange offers every
        /// algorithm, not the one before it. Read, a server whose entry names
        /// none reads what its last key exchange agreed on - "" before the
        /// first.
        /// </summary>
        [Test]
        public async Task TheAEADAlgorithmIsAListWhoseEmptyEntryOffersEveryOne()
        {

            var before   = Value("ntske.legal.aead");

            var set      = await Change("ntske.legal.aead", "AES-SIV-CMAC-256,,aes-128-gcm-siv");
            var after    = Value("ntske.legal.aead");
            var written  = OnDisk()?["servers"]?.Select(server => server is JObject entry ? entry.Value<String>("aead") : null).ToArray();

            var other    = await Change("ntske.legal.aead", "AES-256-GCM-SIV");
            var cleared  = await Change("ntske.legal.aead", "");

            Assert.Multiple(() => {

                Assert.That(before,                       Is.EqualTo(",,,"));
                Assert.That(set,                          Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(after,                        Is.EqualTo("AES-SIV-CMAC-256,,AES-128-GCM-SIV,AES-128-GCM-SIV"),  "the last value holds for the rest");
                Assert.That(written,                      Is.EqualTo(new String?[] { "AES-SIV-CMAC-256", null, "AES-128-GCM-SIV", "AES-128-GCM-SIV" }));

                Assert.That(other,                        Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                Assert.That(cleared,                      Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(Value("ntske.legal.aead"),    Is.EqualTo(",,,"));

            });

        }

        #endregion

        #region ANewServerOffersWhatTheLastOnesEntryNames()

        /// <summary>
        /// A server added by the list of servers offers what the last one's
        /// entry names - and not what its last key exchange agreed on, which
        /// would hold the new server to that one algorithm.
        /// </summary>
        [Test]
        public async Task ANewServerOffersWhatTheLastOnesEntryNames()
        {

            await Change("ntske.legal.aead", ",,,AES-SIV-CMAC-256");

            var grown    = await Change("ntp.legal.servers", "ptbtime1.example.invalid,ptbtime2.example.invalid,ptbtime3.example.invalid,ptbtime4.example.invalid,ptbtime5.example.invalid");

            Assert.Multiple(() => {
                Assert.That(grown,                        Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(Value("ntske.legal.aead"),    Is.EqualTo(",,,AES-SIV-CMAC-256,AES-SIV-CMAC-256"));
            });

        }

        #endregion

        #region TheServersAreRenamedAddedAndTakenAwayByTheirPlaces()

        /// <summary>
        /// The list of servers renames a server at its place - keeping its id
        /// and its pins - adds one at the end with the ports, priority and pin
        /// rules of the last but none of its pins, and takes away the ones
        /// beyond its end.
        /// </summary>
        [Test]
        public async Task TheServersAreRenamedAddedAndTakenAwayByTheirPlaces()
        {

            var idsBefore  = station!.Node.NTSGroups.Single(group => group.IsLegal).ServerIds.ToArray();

            var grown      = await Change("ntp.legal.servers", "ptbtime9.example.invalid,ptbtime2.example.invalid,ptbtime3.example.invalid,ptbtime4.example.invalid,ptbtime5.example.invalid");
            var idsGrown   = station.Node.NTSGroups.Single(group => group.IsLegal).ServerIds.ToArray();
            var newPort    = Value("ntske.legal.ports");
            var pinOfFirst = Value("ntske.legal.rootPins.1");
            var pinOfNew   = Value("ntske.legal.rootPins.5");

            var shrunk     = await Change("ntp.legal.servers", "ptbtime9.example.invalid,ptbtime2.example.invalid");

            Assert.Multiple(() => {

                Assert.That(grown,                         Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(idsGrown[..4],                 Is.EqualTo(idsBefore),  "a server lost its id at its place");
                Assert.That(idsGrown,                      Has.Length.EqualTo(5));
                Assert.That(Value("ntp.legal.servers"),    Is.Not.Null);
                Assert.That(newPort,                       Is.EqualTo("4460,4460,4461,4462,4462"),  "the new server took the last one's port");
                Assert.That(pinOfFirst,                    Is.EqualTo(ARoot),  "the renamed server kept its root pin");
                Assert.That(pinOfNew,                      Is.EqualTo(""),     "the new server took the last one's pins");

                Assert.That(shrunk,                        Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(Value("ntp.legal.servers"),    Is.EqualTo("ptbtime9.example.invalid,ptbtime2.example.invalid"));

            });

        }

        #endregion

        #region TheNthServersPinsAreItsOwn()

        /// <summary>
        /// The pins of the n-th server are that server's: set there and nowhere
        /// else, and a server beyond the list has no key to set.
        /// </summary>
        [Test]
        public async Task TheNthServersPinsAreItsOwn()
        {

            var set     = await Change("ntske.legal.certificatePins.2", APin);
            var beyond  = await Change("ntske.legal.certificatePins.9", APin);

            Assert.Multiple(() => {
                Assert.That(set,                                        Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(Value("ntske.legal.certificatePins.2"),     Is.EqualTo(APin));
                Assert.That(Value("ntske.legal.certificatePins.3"),     Is.EqualTo(""));
                Assert.That(beyond,                                     Is.Null,  "a key the source does not have is not its to answer");
            });

        }

        #endregion


        #region TheReportingThresholdIsTheCommonKey()

        /// <summary>
        /// "ntp.timeAdjustmentReportingThreshold" is ClockCtrlr's threshold and
        /// so the legal group's: five seconds where nothing says otherwise,
        /// changed as 2.1 changes it, and emptied for never.
        /// </summary>
        [Test]
        public async Task TheReportingThresholdIsTheCommonKey()
        {

            var listed  = Keys.Keys.ToArray();
            var five    = Value("NTP.TIMEADJUSTMENTREPORTINGTHRESHOLD");
            var named   = Keys.Get("NTP.TIMEADJUSTMENTREPORTINGTHRESHOLD")?.Key;

            var nine    = await Change("ntp.timeAdjustmentReportingThreshold", " 9 ");
            var nineOn  = OnDisk()?["offsetReportingThresholdSeconds"]?.Value<Int32>();

            var zero    = await Change("ntp.timeAdjustmentReportingThreshold", "0");
            var never   = await Change("ntp.timeAdjustmentReportingThreshold", "");

            Assert.Multiple(() => {

                Assert.That(listed,  Does.Contain("ntp.timeAdjustmentReportingThreshold"));
                Assert.That(five,    Is.EqualTo("5"));
                Assert.That(named,   Is.EqualTo("ntp.timeAdjustmentReportingThreshold"),  "named as the paper writes it");

                Assert.That(nine,    Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(nineOn,  Is.EqualTo(9));
                Assert.That(zero,    Is.EqualTo(OCPP16.ConfigurationStatus.Rejected));
                Assert.That(never,   Is.EqualTo(OCPP16.ConfigurationStatus.Accepted));
                Assert.That(Value("ntp.timeAdjustmentReportingThreshold"),  Is.EqualTo(""));
                Assert.That(station!.Node.LegalTime.OffsetReportingThreshold,    Is.Null);

            });

        }

        #endregion


        #region (private) OnDisk()

        private JObject? OnDisk()

            => JObject.Parse(File.ReadAllText(station!.Node.ConfigFile.Path))["nts"]?["groups"]?["legal"] as JObject;

        #endregion

        #region (private static) Configuration()

        private static JObject Configuration()
        {

            var configuration = TestNodes.Offline;

            configuration["nts"] = JObject.Parse($$"""
                                       { "groups": {
                                           "legal": { "enabled": false, "minServers": 2, "checkEverySeconds": 900, "legalTimeAuthority": "PTB",
                                                      "servers": [
                                                          { "id": "ptb-a", "hostname": "ptbtime1.example.invalid", "priority": 1,
                                                            "rootFingerprint": "{{ARoot}}", "onMismatch": "record", "trustOnFirstUse": "certificate" },
                                                          "ptbtime2.example.invalid",
                                                          { "hostname": "ptbtime3.example.invalid", "ntsKEPort": 4461 },
                                                          { "hostname": "ptbtime4.example.invalid", "ntsKEPort": 4462, "enabled": false } ] },
                                           "local": { "enabled": false, "servers": [ "lc1.example.invalid" ] } } }
                                       """);

            return configuration;

        }

        #endregion

    }

}
