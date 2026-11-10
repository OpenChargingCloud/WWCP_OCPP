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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures
{

    /// <summary>
    /// NotifyNetworkTimeResult with the outcome of a test - the sample of
    /// ExtensionMessageSamples has the outcome of a round - and what a
    /// NotifyNetworkTimeResult with both or neither is.
    /// </summary>
    [TestFixture]
    public class NetworkTimeResultTests
    {

        #region Data

        private const String WithATest = """
            { "requestId": 4712, "generatedAt": "2026-11-10T14:04:01Z",
              "test": { "client": "ptb2", "host": "ptbtime2.ptb.de", "ok": false, "failedAt": "TimeRequest",
                        "duration": 3.096, "offset": 0.0012, "roundTrip": 0.019,
                        "steps": [ { "elapsed": 0,     "stage": "Configuration", "level": "Info",  "text": "Asking ptbtime2.ptb.de." },
                                   { "elapsed": 0.094, "stage": "KeyExchange",   "level": "Notice", "text": "The key exchange succeeded." },
                                   { "elapsed": 3.096, "stage": "TimeRequest",   "level": "Error", "text": "No answer within 3 s." } ] } }
            """;

        #endregion


        #region TheOutcomeOfATestIsReadAndWrittenAsItIs()

        /// <summary>
        /// The outcome of a test is read, and what is written from it is what was read -
        /// as JSON, and through CBOR, its steps in their order.
        /// </summary>
        [Test]
        public void TheOutcomeOfATestIsReadAndWrittenAsItIs()
        {

            var sample = JObject.Parse(WithATest);

            Assert.That(NotifyNetworkTimeResultRequest.TryParse(sample,
                                                                Request_Id.Parse("1"),
                                                                SourceRouting.CSMS,
                                                                NetworkPath.Empty,
                                                                out var request,
                                                                out var error), Is.True, error);

            var cbor = CBORValue.Parse(request!.ToCBOR().ToByteArray());

            Assert.That(NotifyNetworkTimeResultRequest.TryParseCBOR(cbor,
                                                                    Request_Id.Parse("1"),
                                                                    SourceRouting.CSMS,
                                                                    NetworkPath.Empty,
                                                                    out var fromCBOR,
                                                                    out var cborError), Is.True, cborError);

            Assert.Multiple(() => {

                Assert.That(JToken.DeepEquals(AsTheSchemaSaysTests.Normalized(request.ToJSON()),   AsTheSchemaSaysTests.Normalized(sample)), Is.True, request.ToJSON().ToString());
                Assert.That(JToken.DeepEquals(AsTheSchemaSaysTests.Normalized(fromCBOR!.ToJSON()), AsTheSchemaSaysTests.Normalized(sample)), Is.True, fromCBOR.ToJSON().ToString());

                Assert.That(request.Round,                                   Is.Null);
                Assert.That(request.Test?.FailedAt,                          Is.EqualTo("TimeRequest"));
                Assert.That(request.Test?.Steps.Select(step => step.Stage),  Is.EqualTo(new[] { "Configuration", "KeyExchange", "TimeRequest" }));
                Assert.That(fromCBOR.Test,                                   Is.EqualTo(request.Test));

            });

        }

        #endregion

        #region BothOrNeitherIsRefused()

        /// <summary>
        /// Exactly one of the outcome of a round and the outcome of a test.
        /// </summary>
        [Test]
        public void BothOrNeitherIsRefused()
        {

            var test     = JObject.Parse(WithATest)["test"]!;

            var neither  = new JObject(new JProperty("requestId", 1), new JProperty("generatedAt", "2026-11-10T14:04:01Z"));
            var both     = new JObject(new JProperty("requestId", 1), new JProperty("generatedAt", "2026-11-10T14:04:01Z"),
                                       new JProperty("test",  test.DeepClone()),
                                       new JProperty("round", JObject.Parse("""{ "group": "legal", "ok": false, "duration": 0, "answered": 0, "required": 2, "clients": [] }""")));

            Assert.Multiple(() => {
                Assert.That(NotifyNetworkTimeResultRequest.TryParse(neither, Request_Id.Parse("1"), SourceRouting.CSMS, NetworkPath.Empty, out _, out var neitherError), Is.False);
                Assert.That(neitherError,  Does.Contain("Exactly one"));
                Assert.That(NotifyNetworkTimeResultRequest.TryParse(both,    Request_Id.Parse("1"), SourceRouting.CSMS, NetworkPath.Empty, out _, out var bothError),    Is.False);
                Assert.That(bothError,     Does.Contain("Exactly one"));
            });

        }

        #endregion

    }

}
