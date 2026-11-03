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

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.CentralSystem
{

    /// <summary>
    /// Configuration keys a charge point reads from a source when asked, over
    /// the wire: keys numbered by a list that comes and goes with the list,
    /// asked before the charge point's own, and changed by the source.
    /// </summary>
    [TestFixture]
    public class ConfigurationKeySourceTests : AChargePointTests
    {

        #region (class) AListSource

        /// <summary>
        /// "list.servers" and one "list.pin.n" for every server, live from a
        /// list a test changes; "list.secret" write-only. A change of a pin is
        /// accepted, of the servers rejected where it is empty, and asks for a
        /// reboot where it says so.
        /// </summary>
        private sealed class AListSource : IConfigurationKeySource
        {

            public List<String>                     Servers  { get; } = [ "a", "b" ];
            public Dictionary<Int32, String>        Pins     { get; } = [];
            public List<(String Key, String Value, String? Sender)>  Changes  { get; } = [];

            public IEnumerable<String> Keys
                => [ "list.servers", "list.secret", .. Servers.Select((_, index) => $"list.pin.{index + 1}") ];

            public ConfigurationKey? Get(String Key)
            {

                if (String.Equals(Key, "list.servers", StringComparison.OrdinalIgnoreCase))
                    return new ConfigurationKey("list.servers", AccessRights.ReadWrite, String.Join(",", Servers));

                if (String.Equals(Key, "list.secret", StringComparison.OrdinalIgnoreCase))
                    return new ConfigurationKey("list.secret", AccessRights.WriteOnly, "s3cr3t");

                if (Key.StartsWith("list.pin.", StringComparison.OrdinalIgnoreCase) &&
                    Int32.TryParse(Key["list.pin.".Length..], out var n) && n >= 1 && n <= Servers.Count)
                {
                    return new ConfigurationKey($"list.pin.{n}", AccessRights.ReadWrite, Pins.GetValueOrDefault(n, ""));
                }

                return null;

            }

            public Task<ConfigurationStatus?> ChangeAsync(String Key, String Value, String? Sender, CancellationToken CancellationToken = default)
            {

                if (Get(Key) is null)
                    return Task.FromResult<ConfigurationStatus?>(null);

                Changes.Add((Key, Value, Sender));

                if (String.Equals(Key, "list.servers", StringComparison.OrdinalIgnoreCase))
                {

                    if (Value.Length == 0)
                        return Task.FromResult<ConfigurationStatus?>(ConfigurationStatus.Rejected);

                    Servers.Clear();
                    Servers.AddRange(Value.Split(','));
                    return Task.FromResult<ConfigurationStatus?>(ConfigurationStatus.RebootRequired);

                }

                if (Key.StartsWith("list.pin.", StringComparison.OrdinalIgnoreCase))
                    Pins[Int32.Parse(Key["list.pin.".Length..])] = Value;

                return Task.FromResult<ConfigurationStatus?>(ConfigurationStatus.Accepted);

            }

        }

        #endregion


        #region TheKeysOfASourceComeAndGoWithItsList_Test()

        /// <summary>
        /// GetConfiguration without keys lists the source's keys as they are now,
        /// beside the charge point's own; a write-only one without its value. A
        /// key numbered beyond the list is unknown, and known once the list
        /// grows.
        /// </summary>
        [Test]
        public async Task TheKeysOfASourceComeAndGoWithItsList_Test()
        {

            var source = new AListSource();

            chargePoint1!.AddConfigurationKeySource(source);

            var to      = SourceRouting.To(chargePoint1.Id);
            var all     = await testCentralSystem01!.GetConfiguration(to);
            var beyond  = await testCentralSystem01.GetConfiguration(to, [ "list.pin.3" ]);

            source.Servers.Add("c");

            var grown   = await testCentralSystem01.GetConfiguration(to, [ "LIST.PIN.3" ]);

            Assert.Multiple(() => {

                Assert.That(all.ConfigurationKeys.Select(key => key.Key),
                            Is.SupersetOf(new[] { "list.servers", "list.secret", "list.pin.1", "list.pin.2", "hello", "changeMe" }));
                Assert.That(all.ConfigurationKeys.Single(key => key.Key == "list.servers").Value,  Is.EqualTo("a,b"));
                Assert.That(all.ConfigurationKeys.Single(key => key.Key == "list.secret").Value,   Is.Null,  "a write-only value was sent");

                Assert.That(beyond.UnknownKeys,                       Is.EqualTo(new[] { "list.pin.3" }));
                Assert.That(grown.ConfigurationKeys.Single().Key,     Is.EqualTo("list.pin.3"));

            });

        }

        #endregion

        #region AChangeIsTheSourcesToAnswer_Test()

        /// <summary>
        /// ChangeConfiguration of a key of the source is the source's to answer:
        /// accepted, rejected, or asking for a reboot - and named with the
        /// central system that asked. The charge point's own keys and the
        /// unknown ones are as they were.
        /// </summary>
        [Test]
        public async Task AChangeIsTheSourcesToAnswer_Test()
        {

            var source = new AListSource();

            chargePoint1!.AddConfigurationKeySource(source);

            var to       = SourceRouting.To(chargePoint1.Id);
            var pin      = await testCentralSystem01!.ChangeConfiguration(to, "list.pin.2", "beef");
            var empty    = await testCentralSystem01.ChangeConfiguration(to, "list.servers", "");
            var servers  = await testCentralSystem01.ChangeConfiguration(to, "list.servers", "x,y,z");
            var own      = await testCentralSystem01.ChangeConfiguration(to, "changeMe", "later");
            var unknown  = await testCentralSystem01.ChangeConfiguration(to, "list.nothing", "1");

            Assert.Multiple(() => {
                Assert.That((pin.Status, empty.Status, servers.Status),
                            Is.EqualTo((ConfigurationStatus.Accepted, ConfigurationStatus.Rejected, ConfigurationStatus.RebootRequired)));
                Assert.That(source.Pins[2],                  Is.EqualTo("beef"));
                Assert.That(source.Servers,                  Is.EqualTo(new[] { "x", "y", "z" }));
                Assert.That(source.Changes[0].Sender,        Is.Not.Null.And.Not.Empty);
                Assert.That(own.Status,                      Is.EqualTo(ConfigurationStatus.Accepted));
                Assert.That(unknown.Status,                  Is.EqualTo(ConfigurationStatus.NotSupported));
            });

        }

        #endregion

    }

}
