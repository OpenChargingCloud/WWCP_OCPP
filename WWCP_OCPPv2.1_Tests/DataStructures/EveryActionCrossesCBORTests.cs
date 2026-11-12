/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Reflection;

using NUnit.Framework;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

using static cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures.AsTheSchemaSaysTests;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures
{

    /// <summary>
    /// The adapter works on JSON, and translates at its edge what a connection that
    /// agreed on "ocpp2.1cbor" carries. Every action it has a processor for is one
    /// it can translate - a processor added without that would answer over JSON and
    /// refuse over CBOR - and what it translates comes back as it went.
    /// </summary>
    [TestFixture]
    public class EveryActionCrossesCBORTests
    {

        #region Data

        private static readonly Request_Id     requestId    = Request_Id.Parse("1");
        private static readonly SourceRouting  destination  = SourceRouting.CSMS;

        /// <summary>
        /// The actions whose request arrives as a JSON object: the names of the
        /// Receive_... methods of the incoming adapter that take one and answer.
        /// </summary>
        public static IEnumerable<String> Actions

            => typeof(OCPPWebSocketAdapterIN).
                   GetMethods(BindingFlags.Public | BindingFlags.Instance).
                   Where  (method => method.Name.StartsWith("Receive_") &&
                                     method.ReturnType == typeof(Task<OCPP_Response>) &&
                                     method.GetParameters().Length > 7 &&
                                     method.GetParameters()[6].ParameterType == typeof(JObject)).
                   Select (method => method.Name[8..]).
                   Distinct().
                   Order();

        /// <summary>
        /// The actions that are messages without an answer.
        /// </summary>
        private static Boolean HasNoAnswer(String Action)

            => typeof(OCPPv2_1.CS.HeartbeatRequest).Assembly.GetTypes().
                   Any(type => type.IsClass && type.IsPublic && type.Name == Action + "Message");

        /// <summary>
        /// The actions whose answer is a message of the binary format of the
        /// BinaryDataStreams extension. That format is not spoken over a CBOR
        /// connection, and its messages have no CBOR form yet: over CBOR such a
        /// request is answered with a request error.
        /// </summary>
        private static readonly String[] AnsweredInTheBinaryFormat = [ "GetFile" ];

        /// <summary>
        /// The messages that have a sample with every property of their schema.
        /// </summary>
        public static IEnumerable<String> SampledRequests
            => MessageSamples.All.Keys.Where(name => name.EndsWith("Request"));

        #endregion


        #region ThereAreActions()

        /// <summary>
        /// The search above finds the processors - an empty list would pass everything below.
        /// </summary>
        [Test]
        public void ThereAreActions()
        {
            Assert.That(Actions.Count(), Is.GreaterThan(100));
        }

        #endregion

        #region IsTranslated(Action)

        [TestCaseSource(nameof(Actions))]
        public void IsTranslated(String Action)
        {

            if (HasNoAnswer(Action))
                Assert.Pass($"'{Action}' is a message without an answer.");

            if (AnsweredInTheBinaryFormat.Contains(Action))
                Assert.Pass($"'{Action}' is answered in the binary format, which has no CBOR form yet.");

            Assert.Multiple(() => {
                Assert.That(OCPPMessagesAsCBOR.KnowsRequest (Action),  Is.True, $"The request of '{Action}' is not translated between JSON and CBOR.");
                Assert.That(OCPPMessagesAsCBOR.KnowsResponse(Action),  Is.True, $"The response to '{Action}' is not translated between JSON and CBOR.");
            });

        }

        #endregion

        #region ARequestAndItsResponse_ComeBackAsTheyWent(Name)

        /// <summary>
        /// The sample of a request, taken to CBOR bytes and back the way the adapter
        /// does it, is the sample again - and so is the sample of its response.
        /// </summary>
        [TestCaseSource(nameof(SampledRequests))]
        public void ARequestAndItsResponse_ComeBackAsTheyWent(String Name)
        {

            var action   = Name[..^"Request".Length];
            var sample   = JObject.Parse(MessageSamples.All[Name]);

            Assert.That(OCPPMessagesAsCBOR.TryRequestToCBOR(action, sample, requestId, destination, NetworkPath.Empty, out var cbor, out var sent, out var e1),
                        Is.True, e1);

            Assert.That(OCPPMessagesAsCBOR.TryRequestToJSON(action, CBORValue.Parse(cbor.ToByteArray()), requestId, destination, NetworkPath.Empty, out var json, out var received, out var e2),
                        Is.True, e2);

            Assert.That(JToken.DeepEquals(Normalized(json!), Normalized(sample)), Is.True,
                        $"Came back: {Normalized(json!).ToString(Newtonsoft.Json.Formatting.None)}{Environment.NewLine}" +
                        $"Went:      {Normalized(sample).ToString(Newtonsoft.Json.Formatting.None)}");

            if (!MessageSamples.All.TryGetValue(action + "Response", out var responseText))
                return;

            var response = JObject.Parse(responseText);

            // Written by whoever answers, with the request as it read it...
            Assert.That(OCPPMessagesAsCBOR.TryResponseToCBOR(action, received!, response, requestId, destination, NetworkPath.Empty, out var responseCBOR, out var e3),
                        Is.True, e3);

            // ...and read by whoever asked, with the request as it sent it.
            Assert.That(OCPPMessagesAsCBOR.TryResponseToJSON(action, sent!, CBORValue.Parse(responseCBOR.ToByteArray()), requestId, destination, NetworkPath.Empty, out var responseJSON, out var e4),
                        Is.True, e4);

            Assert.That(JToken.DeepEquals(Normalized(responseJSON!), Normalized(response)), Is.True,
                        $"Came back: {Normalized(responseJSON!).ToString(Newtonsoft.Json.Formatting.None)}{Environment.NewLine}" +
                        $"Went:      {Normalized(response).ToString(Newtonsoft.Json.Formatting.None)}");

        }

        #endregion

        #region AnActionNobodyKnows_IsNotTranslated()

        [Test]
        public void AnActionNobodyKnows_IsNotTranslated()
        {

            Assert.Multiple(() => {

                Assert.That(OCPPMessagesAsCBOR.TryRequestToCBOR("NobodyKnowsThis", [], requestId, destination, NetworkPath.Empty, out _, out _, out var e1),  Is.False);
                Assert.That(e1,  Does.Contain("NobodyKnowsThis"));

                Assert.That(OCPPMessagesAsCBOR.TryRequestToJSON("NobodyKnowsThis", CBORValue.FromMap([]), requestId, destination, NetworkPath.Empty, out _, out _, out var e2),  Is.False);
                Assert.That(e2,  Does.Contain("NobodyKnowsThis"));

            });

        }

        #endregion

        #region AMessageWithoutAnAnswer_ComesBackAsItWent()

        /// <summary>
        /// A message without an answer, whose class writes no CBOR, is carried as what
        /// its JSON says.
        /// </summary>
        [Test]
        public void AMessageWithoutAnAnswer_ComesBackAsItWent()
        {

            var message = new JObject(
                              new JProperty("vendorId",   "GraphDefined"),
                              new JProperty("messageId",  "hello"),
                              new JProperty("data",       new JObject(
                                                              new JProperty("text",    "world"),
                                                              new JProperty("number",  23),
                                                              new JProperty("list",    new JArray(1, 2, 3))
                                                          ))
                          );

            Assert.That(OCPPMessagesAsCBOR.TryMessageToCBOR("MessageTransfer", message, requestId, destination, NetworkPath.Empty, out var cbor, out var e1),
                        Is.True, e1);

            Assert.That(OCPPMessagesAsCBOR.TryMessageToJSON("MessageTransfer", CBORValue.Parse(cbor.ToByteArray()), requestId, destination, NetworkPath.Empty, out var json, out var e2),
                        Is.True, e2);

            Assert.That(JToken.DeepEquals(json, message), Is.True, json!.ToString(Newtonsoft.Json.Formatting.None));

        }

        #endregion

    }

}
