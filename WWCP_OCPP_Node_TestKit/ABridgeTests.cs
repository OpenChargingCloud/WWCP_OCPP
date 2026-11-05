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

using System.Net;
using System.Text;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using cloud.charging.open.protocols.WWCP.Node;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.TestKit
{

    /// <summary>
    /// The kit's tests, asked of a kind of node: a suite derives a fixture
    /// from each and says how its kind makes a node with its bridges.
    /// </summary>
    public abstract class ABridgeTests
    {

        /// <summary>
        /// How the kind of node under test makes a node with its bridges.
        /// </summary>
        protected abstract IBridgedNodes Nodes { get; }

    }


    /// <summary>
    /// The kit's tests that need a node started, with its web interface and
    /// an account to sign in with: those that compare what a page saves with
    /// what OCPP saves.
    /// </summary>
    public abstract class AStartedBridgeTests : ABridgeTests
    {

        #region Properties

        /// <summary>
        /// The node, started.
        /// </summary>
        protected IBridgedNode  Station      { get; private set; } = default!;

        /// <summary>
        /// The password it made up for its first start.
        /// </summary>
        protected String        Password     { get; private set; } = default!;

        /// <summary>
        /// Where its web interface is.
        /// </summary>
        protected String        BaseURL      { get; private set; } = default!;

        /// <summary>
        /// Its directory.
        /// </summary>
        protected String        Directory    { get; private set; } = default!;

        #endregion

        #region What this node is made of

        /// <summary>
        /// What its configuration file says.
        /// </summary>
        protected virtual JObject Configuration
            => TestNodes.Offline;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public async Task StartTheNode()
        {

            Directory  = TestNodes.TemporaryDirectory("bridge");
            Station    = await Nodes.Started(Directory, Configuration);
            BaseURL    = Station.Node.WebInterfaceURL.ToString();

            // After the start, and not before: the account is made where the
            // accounts are read, which is asynchronous.
            Password   = Station.Node.GeneratedPassword
                             ?? throw new InvalidOperationException("The node did not make up a password for its first start!");

        }

        [TearDown]
        public async Task StopTheNode()
        {

            if (Station is not null)
                await Station.DisposeAsync();

            TestNodes.Remove(Directory);

        }

        #endregion

        #region (protected) SignedIn()

        /// <summary>
        /// A browser signed in as the account the node made for its first start.
        /// </summary>
        protected async Task<HttpClient> SignedIn()
        {

            var http      = new HttpClient(new HttpClientHandler { CookieContainer = new CookieContainer(), UseCookies = true }) {
                                BaseAddress = new Uri(BaseURL)
                            };

            var response  = await http.PostAsync($"{WWCPNode.ExtAPIPath.ToString().Trim('/')}/login",
                                                 new FormUrlEncodedContent([
                                                     new KeyValuePair<String, String>("login",     WWCPNode.DefaultAdminUser),
                                                     new KeyValuePair<String, String>("password",  Password)
                                                 ]));

            Assert.That(response.IsSuccessStatusCode, Is.True,
                        $"Signing in failed with {(Int32) response.StatusCode}, and every assertion below it would say so instead.");

            return http;

        }

        #endregion

        #region (protected static) JSONBody(...)

        /// <summary>
        /// A JSON body of the given properties.
        /// </summary>
        protected static StringContent JSONBody(params JProperty[] Properties)

            => new (new JObject(Properties).ToString(),
                    Encoding.UTF8,
                    "application/json");

        #endregion

    }


    /// <summary>
    /// What the kit's tests make their nodes in, and of.
    /// </summary>
    public static class TestNodes
    {

        #region Offline

        /// <summary>
        /// A configuration with the time client switched off.
        /// </summary>
        /// <remarks>
        /// Written before a node is built, because that is when it is read,
        /// and switched off there rather than afterwards, because a started
        /// node would otherwise schedule its first check of the clock. A test
        /// run has no business asking a public time server anything.
        /// </remarks>
        public static JObject Offline

            => new (
                   new JProperty("nts", new JObject(
                       new JProperty("groups", new JObject(
                           new JProperty("legal", new JObject(
                               new JProperty("enabled", false)
                           ))
                       ))
                   ))
               );

        #endregion

        #region TemporaryDirectory(Purpose)

        /// <summary>
        /// A directory of its own for one test, so that no two of them read
        /// each other's accounts or configuration.
        /// </summary>
        public static String TemporaryDirectory(String Purpose)

            => Path.Combine(
                   Path.GetTempPath(),
                   $"bridge-{Purpose}-{Guid.NewGuid().ToString("N")[..12]}"
               );

        #endregion

        #region Remove(Directory)

        /// <summary>
        /// Take a test's directory away again.
        /// </summary>
        /// <remarks>
        /// A directory that survives a failed run is untidy and nothing more,
        /// so this never throws: failing a teardown over it would hide the
        /// failure that actually matters.
        /// </remarks>
        public static void Remove(String? Directory)
        {

            try
            {
                if (Directory is not null && System.IO.Directory.Exists(Directory))
                    System.IO.Directory.Delete(Directory, true);
            }
            catch (IOException)
            { }
            catch (UnauthorizedAccessException)
            { }

        }

        #endregion

    }


    /// <summary>
    /// A clock a test can move.
    /// </summary>
    public sealed class MovableClock(DateTimeOffset Start) : TimeProvider
    {

        /// <summary>
        /// What the clock says now.
        /// </summary>
        public DateTimeOffset Now { get; set; } = Start;

        /// <inheritdoc/>
        public override DateTimeOffset GetUtcNow() => Now;

    }

}
