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
using cloud.charging.open.protocols.OCPPv1_6.NetworkingNode;
using cloud.charging.open.protocols.OCPPv1_6.WebSockets;

using static cloud.charging.open.protocols.OCPPv1_6.tests.DataTypes.AsTheSchemaSaysTests;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.DataTypes
{

    /// <summary>
    /// The adapter works on JSON, and translates at its edge what a connection that
    /// agreed on "ocpp1.6cbor" carries. Every action it has a processor for is one
    /// it translates through the class of the message - a processor added without
    /// that would be carried as what its JSON says, value by value - and what it
    /// translates comes back as it went.
    /// </summary>
    [TestFixture]
    public class EveryActionCrossesCBORTests
    {

        #region Data

        private static readonly Request_Id     requestId    = Request_Id.Parse("1");
        private static readonly SourceRouting  destination  = SourceRouting.To(NetworkingNode_Id.CentralSystem);

        /// <summary>
        /// The actions whose request arrives as a JSON object: the names of the
        /// Receive_... methods of the incoming adapter that take one and answer.
        /// </summary>
        public static IEnumerable<String> Actions
            => typeof(OCPPWebSocketAdapterIN).
                   GetMethods(BindingFlags.Public | BindingFlags.Instance).
                   Where  (method => method.Name.StartsWith("Receive_") &&
                                     method.ReturnType.IsGenericType &&
                                     method.ReturnType.GetGenericArguments()[0].Name == "OCPP_Response" &&
                                     method.GetParameters().Any(parameter => parameter.ParameterType == typeof(JObject))).
                   Select (method => method.Name[8..]).
                   Distinct().
                   Order();

        /// <summary>
        /// The requests that have a sample with every property of their schema.
        /// </summary>
        public static IEnumerable<String> SampledRequests
            => MessageSamples.FromTheSchemas.Keys.Where(name => name.EndsWith("Request"));

        #endregion


        #region ThereAreActions()

        /// <summary>
        /// The search above finds the processors - an empty list would pass everything below.
        /// </summary>
        [Test]
        public void ThereAreActions()
        {
            Assert.That(Actions.Count(), Is.GreaterThan(40));
        }

        #endregion

        #region IsTranslated(Action)

        [TestCaseSource(nameof(Actions))]
        public void IsTranslated(String Action)
        {
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
            var sample   = JObject.Parse(MessageSamples.FromTheSchemas[Name]);

            Assert.That(OCPPMessagesAsCBOR.KnowsRequest(action), Is.True, $"The request of '{action}' is carried as what its JSON says.");

            Assert.That(OCPPMessagesAsCBOR.TryRequestToCBOR(action, sample, requestId, destination, NetworkPath.Empty, out var cbor, out var sent, out var e1),
                        Is.True, e1);

            Assert.That(OCPPMessagesAsCBOR.TryRequestToJSON(action, CBORValue.Parse(cbor.ToByteArray()), requestId, destination, NetworkPath.Empty, out var json, out var received, out var e2),
                        Is.True, e2);

            Assert.That(JToken.DeepEquals(Normalized(json!), Normalized(sample)), Is.True,
                        $"Came back: {Normalized(json!).ToString(Newtonsoft.Json.Formatting.None)}{Environment.NewLine}" +
                        $"Went:      {Normalized(sample).ToString(Newtonsoft.Json.Formatting.None)}");

            if (!MessageSamples.FromTheSchemas.TryGetValue(action + "Response", out var responseText))
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

        #region AWebPayment_IsADataTransferOfAClassOfItsOwn_ThroughCBOR()

        /// <summary>
        /// NotifyWebPaymentStarted and NotifyWebPaymentFailed are DataTransfer requests
        /// of OCPP 1.6 with a class of their own: written as CBOR as every DataTransfer
        /// is, and read back as what they are.
        /// </summary>
        [Test]
        public void AWebPayment_IsADataTransferOfAClassOfItsOwn_ThroughCBOR()
        {

            var started  = new CS.NotifyWebPaymentStartedRequest(
                               destination,
                               Connector_Id.Parse(1),
                               TimeSpan.FromSeconds(120),
                               RequestId: requestId
                           );

            var cbor     = CBORValue.Parse(started.ToCBOR().ToByteArray());

            Assert.That(CS.NotifyWebPaymentStartedRequest.TryParseCBOR(cbor, requestId, destination, NetworkPath.Empty, out var read, out var e1),
                        Is.True, e1);

            Assert.Multiple(() => {
                Assert.That(read!.ConnectorId,                 Is.EqualTo(started.ConnectorId));
                Assert.That(read.Timeout,                      Is.EqualTo(started.Timeout));
                Assert.That(read.VendorId,                     Is.EqualTo(started.VendorId));
                Assert.That(read.MessageId,                    Is.EqualTo(started.MessageId));
            });

            // On the wire it is the action "DataTransfer", translated through its class.
            Assert.That(OCPPMessagesAsCBOR.TryRequestToCBOR("DataTransfer", started.ToJSON(), requestId, destination, NetworkPath.Empty, out var wire, out _, out var e2),
                        Is.True, e2);

            Assert.That(OCPPMessagesAsCBOR.TryRequestToJSON("DataTransfer", CBORValue.Parse(wire.ToByteArray()), requestId, destination, NetworkPath.Empty, out var back, out _, out var e3),
                        Is.True, e3);

            Assert.That(JToken.DeepEquals(Normalized(back!), Normalized(started.ToJSON())), Is.True, back!.ToString(Newtonsoft.Json.Formatting.None));

        }

        #endregion

    }

}
