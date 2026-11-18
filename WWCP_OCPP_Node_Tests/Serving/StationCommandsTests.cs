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

using org.GraphDefined.Vanaheimr.Hermod;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.CommandLine;
using cloud.charging.open.protocols.WWCP.Node.Configuration;
using cloud.charging.open.protocols.WWCP.Node.TestKit;

using cloud.charging.open.protocols.OCPP.Node.Serving.CommandLine;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests.Serving
{

    /// <summary>
    /// What stationKeys and stationPins are before any kind of node registers
    /// them: commands for a node that serves charging stations and for no
    /// other, never found by their type - so that the kind that registers them
    /// by hand has each of them once, and Styx does not refuse both as a
    /// command found twice.
    /// </summary>
    /// <remarks>
    /// What they answer is tested where a node that serves charging stations
    /// is: with the local controller, and with the gateway in its words.
    /// </remarks>
    public class StationCommandsTests
    {

        #region Data

        private String     directory  = default!;
        private WWCPNode?  node;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeANode()
        {

            directory = ServingTestDirectories.TemporaryDirectory("stationcommands");
            Directory.CreateDirectory(directory);

            var configFile = Path.Combine(directory, WWCPConfigFile.DefaultFileName);

            File.WriteAllText(configFile, """{ "dns": { "enabled": false }, "nts": { "groups": { "legal": { "enabled": false } } } }""");

            node = new WWCPNode(
                       HTTPPort:          IPPort.Parse(TestPorts.Free()),
                       AccountsPath:      Path.Combine(directory, "accounts"),
                       ConfigFile:        new WWCPConfigFile(configFile),
                       CertificatesPath:  Path.Combine(directory, "certificates"),
                       LogToConsole:      false,
                       BridgeDebugLog:    false
                   );

        }

        [TearDown]
        public async Task LetItGo()
        {

            if (node is not null)
                await node.DisposeAsync();

            node = null;

            ServingTestDirectories.Remove(directory);

        }

        #endregion


        #region ANodeThatServesNoStations_GetsNeither()

        /// <summary>
        /// A node that serves no charging stations has nothing to list: neither
        /// command is made for its command line, and the refusal says why.
        /// </summary>
        [Test]
        public void ANodeThatServesNoStations_GetsNeither()
        {

            using var cli = new NodeCLI(node!);

            Assert.Multiple(() => {

                Assert.That(() => new StationKeysCommand(cli, "stations"),
                            Throws.ArgumentException.With.Message.StartsWith($"The {node!.Kind.Name} of this command line serves no charging stations."));

                Assert.That(() => new StationPinsCommand(cli, "stations"),
                            Throws.ArgumentException.With.Message.StartsWith($"The {node!.Kind.Name} of this command line serves no charging stations."));

            });

        }

        #endregion

        #region TheyAreNeverFoundByTheirType()

        /// <summary>
        /// Searched for commands, this assembly gives neither: they need the
        /// resource they ask for, which only the kind of node that registers
        /// them knows - and a command found by its type beside the one
        /// registered by hand would be run by neither.
        /// </summary>
        [Test]
        public void TheyAreNeverFoundByTheirType()
        {

            using var cli = new NodeCLI(node!, typeof(StationKeysCommand).Assembly);

            Assert.Multiple(() => {
                Assert.That(cli.Commands.OfType<StationKeysCommand>(), Is.Empty);
                Assert.That(cli.Commands.OfType<StationPinsCommand>(), Is.Empty);
                Assert.That(StationKeysCommand.CommandName,            Is.EqualTo("stationKeys"));
                Assert.That(StationPinsCommand.CommandName,            Is.EqualTo("stationPins"));
            });

        }

        #endregion

    }

}
