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

using NUnit.Framework;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Serving
{

    /// <summary>
    /// The Device Model of a node as its OCPP Device Model page shows it: each
    /// component by the name OCPP gives it, each variable with what it says
    /// now, a value OCPP keeps WriteOnly hidden from all but a systemadmin, a
    /// getter that throws said so in its place, and the configuration keys of
    /// OCPP 1.6 by name.
    /// </summary>
    [TestFixture]
    public class DeviceModelTreeTests
    {

        #region (private static) Components()

        private static ComponentConfig[] Components(Func<String?> Password)
            => [
                   new ComponentConfig(
                       "SecurityCtrlr",
                       VariableConfigs: [
                           new VariableConfig("OrganizationName",  () => "GraphDefined",
                                              Attributes:       new VariableAttribute(Mutability: MutabilityTypes.ReadWrite, Persistent: true),
                                              Characteristics:  new VariableCharacteristics(DataType: DataTypes.String, MaxLimit: 50)),
                           new VariableConfig("BasicAuthPassword", Password,
                                              Attributes:       new VariableAttribute(Mutability: MutabilityTypes.WriteOnly)) { IsSecret = true }
                       ]
                   ),
                   new ComponentConfig(
                       "Connector",
                       EVSE:             new EVSE(EVSE_Id.Parse(1), Connector_Id.Parse(2)),
                       VariableConfigs:  [ new VariableConfig("Available", () => throw new InvalidOperationException("The meter is gone.")) ]
                   ),
                   new ComponentConfig(
                       "DNSClientCtrlr",
                       Instance:         "ptb1",
                       VariableConfigs:  [ new VariableConfig("Enabled", () => "true", Attributes: new VariableAttribute(Mutability: MutabilityTypes.ReadOnly)) ]
                   )
               ];

        #endregion


        #region ComponentsAreNamedAsOCPPNamesThem_AndSortedByName()

        [Test]
        public void ComponentsAreNamedAsOCPPNamesThem_AndSortedByName()
        {

            var tree = OCPPServingNode.ComponentsJSON(Components(() => "s3cr3t-s3cr3t-s3cr3t"), ShowWriteOnly: false);

            Assert.That(tree.Properties().Select(property => property.Name),
                        Is.EqualTo(new[] { "Connector @ EVSE 1/2", "DNSClientCtrlr[ptb1]", "SecurityCtrlr" }));

            var organization = (JObject) tree["SecurityCtrlr"]!["OrganizationName"]!;

            Assert.Multiple(() => {
                Assert.That(organization["value"]?.Value<String>(),       Is.EqualTo("GraphDefined"));
                Assert.That(organization["mutability"]?.Value<String>(),  Is.EqualTo("ReadWrite"));
                Assert.That(organization["type"]?.Value<String>(),        Is.EqualTo("Actual"));
                Assert.That(organization["persistent"]?.Value<Boolean>(), Is.True);
                Assert.That(organization["characteristics"]?["maxLimit"]?.Value<Decimal>(), Is.EqualTo(50));
                Assert.That(tree["DNSClientCtrlr[ptb1]"]!["Enabled"]!["mutability"]?.Value<String>(), Is.EqualTo("ReadOnly"));
            });

        }

        #endregion

        #region AWriteOnlyValueIsHidden_ButNotFromASystemadmin()

        [Test]
        public void AWriteOnlyValueIsHidden_ButNotFromASystemadmin()
        {

            var hidden  = (JObject) OCPPServingNode.ComponentsJSON(Components(() => "s3cr3t-s3cr3t-s3cr3t"), ShowWriteOnly: false)["SecurityCtrlr"]!["BasicAuthPassword"]!;
            var shown   = (JObject) OCPPServingNode.ComponentsJSON(Components(() => "s3cr3t-s3cr3t-s3cr3t"), ShowWriteOnly: true )["SecurityCtrlr"]!["BasicAuthPassword"]!;

            Assert.Multiple(() => {

                Assert.That(hidden.ContainsKey("value"),                Is.False, "A WriteOnly value was shown.");
                Assert.That(hidden["hidden"]?.Value<Boolean>(),         Is.True);
                Assert.That(hidden.ToString(),                          Does.Not.Contain("s3cr3t"));

                Assert.That(shown["value"]?.Value<String>(),            Is.EqualTo("s3cr3t-s3cr3t-s3cr3t"));
                Assert.That(shown.ContainsKey("hidden"),                Is.False);
                Assert.That(shown["mutability"]?.Value<String>(),       Is.EqualTo("WriteOnly"));
                Assert.That(shown["secret"]?.Value<Boolean>(),          Is.True);

            });

        }

        #endregion

        #region AGetterThatThrowsIsSaidSo_AndTheRestIsThere()

        [Test]
        public void AGetterThatThrowsIsSaidSo_AndTheRestIsThere()
        {

            var tree       = OCPPServingNode.ComponentsJSON(Components(() => null), ShowWriteOnly: true);
            var available  = (JObject) tree["Connector @ EVSE 1/2"]!["Available"]!;

            Assert.Multiple(() => {
                Assert.That(available["value"]?.Type,                     Is.EqualTo(JTokenType.Null));
                Assert.That(available["error"]?.Value<String>(),          Is.EqualTo("The meter is gone."));
                Assert.That(tree["SecurityCtrlr"]!["BasicAuthPassword"]!["value"]?.Type, Is.EqualTo(JTokenType.Null), "A WriteOnly value that is not set is null.");
                Assert.That(tree.Count,                                   Is.EqualTo(3));
            });

        }

        #endregion

        #region TwoComponentsOfTheSameNameAreBothThere()

        [Test]
        public void TwoComponentsOfTheSameNameAreBothThere()
        {

            var tree = OCPPServingNode.ComponentsJSON([ new ComponentConfig("ClockCtrlr"), new ComponentConfig("ClockCtrlr") ], ShowWriteOnly: false);

            Assert.That(tree.Properties().Select(property => property.Name), Is.EqualTo(new[] { "ClockCtrlr", "ClockCtrlr (2)" }));

        }

        #endregion

        #region ConfigurationKeysOfOCPP16_ByName()

        private sealed class KeySource(params OCPPv1_6.ConfigurationKey[] All) : OCPPv1_6.CP.IConfigurationKeySource
        {

            public IEnumerable<String> Keys
                => All.Select(key => key.Key);

            public OCPPv1_6.ConfigurationKey? Get(String Key)
                => All.FirstOrDefault(key => key.Key.Equals(Key, StringComparison.OrdinalIgnoreCase));

            public Task<OCPPv1_6.ConfigurationStatus?> ChangeAsync(String Key, String Value, String? Sender, CancellationToken CancellationToken = default)
                => Task.FromResult<OCPPv1_6.ConfigurationStatus?>(null);

        }

        [Test]
        public void ConfigurationKeysOfOCPP16_ByName()
        {

            var tree = OCPPServingNode.ConfigurationKeysJSON([
                           new KeySource(new OCPPv1_6.ConfigurationKey("ntske.legal.aead",  OCPPv1_6.AccessRights.ReadWrite, "AES-SIV-CMAC-256"),
                                    new OCPPv1_6.ConfigurationKey("AuthorizationKey",  OCPPv1_6.AccessRights.WriteOnly)),
                           new KeySource(new OCPPv1_6.ConfigurationKey("dns.servers",       OCPPv1_6.AccessRights.ReadOnly,  "ns1"))
                       ]);

            Assert.Multiple(() => {
                Assert.That(tree.Properties().Select(property => property.Name), Is.EqualTo(new[] { "AuthorizationKey", "dns.servers", "ntske.legal.aead" }));
                Assert.That(tree["ntske.legal.aead"]!["value"]?.Value<String>(),     Is.EqualTo("AES-SIV-CMAC-256"));
                Assert.That(tree["ntske.legal.aead"]!["readonly"]?.Value<Boolean>(), Is.False);
                Assert.That(tree["dns.servers"]!["readonly"]?.Value<Boolean>(),      Is.True);
                Assert.That(tree["AuthorizationKey"]!["hidden"]?.Value<Boolean>(),   Is.True);
            });

        }

        #endregion

    }

}
