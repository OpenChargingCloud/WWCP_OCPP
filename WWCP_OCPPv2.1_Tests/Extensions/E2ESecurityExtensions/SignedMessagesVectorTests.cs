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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.E2ESecurityExtensions
{

    /// <summary>
    /// The test vectors of the white paper "Signed OCPP Messages and Signer Roles":
    /// OCPP-J frames signed by another implementation (Node.js, OpenSSL, ECMAScript's own
    /// JSON.stringify for RFC 8785), and what a verifier must decide about them.
    /// </summary>
    /// <remarks>
    /// signed-messages-vectors.json is a copy of SignedMessages/test-vectors/signed-messages-vectors.json
    /// in OpenChargingTechnology/Whitepapers, made by SignedMessages/tools/generate-test-vectors.mjs there.
    /// </remarks>
    [TestFixture]
    public class SignedMessagesVectorTests
    {

        #region Data

        private static readonly Lazy<JObject> vectors = new (() => {

            var assembly = typeof(SignedMessagesVectorTests).Assembly;
            var name     = assembly.GetManifestResourceNames().Single(resource => resource.EndsWith("signed-messages-vectors.json"));

            using var reader = new StreamReader(assembly.GetManifestResourceStream(name)!);
            return JObject.Parse(reader.ReadToEnd());

        });

        #endregion

        #region (private) Vector(Id) / Cases

        private static JObject Vector(String Id)
            => (JObject) vectors.Value["vectors"]!.Single(vector => vector["id"]!.Value<String>() == Id);

        private static IEnumerable<TestCaseData> Cases

            => vectors.Value["vectors"]!.Select(vector => new TestCaseData(vector["id"]!.Value<String>()).SetArgDisplayNames(vector["id"]!.Value<String>()!));

        #endregion

        #region (private) Verifier(Vector)

        private static SignaturePolicy Verifier(JObject Vector)
        {

            var verifier    = (JObject) Vector["verifier"]!;
            var parameters  = (JObject) vectors.Value["parameters"]!;
            var now         = DateTimeOffset.FromUnixTimeSeconds(verifier["now"]!.Value<Int64>());
            var action      = Enum.Parse<VerificationRuleActions>(verifier["rule"]!["action"]!.Value<String>()!);
            var keyPair     = verifier["rule"]!["key"] is JToken key
                                  ? new KeyPair(Convert.FromHexString(key.Value<String>()!))
                                  : null;

            return new SignaturePolicy {
                       Now              = () => now,
                       AcceptFormat0    = verifier["acceptFormat0"]!.Value<Boolean>(),
                       MaxClockSkew     = TimeSpan.FromSeconds(parameters["maxClockSkew"]!.   Value<Int32>()),
                       MaxSignatureAge  = TimeSpan.FromSeconds(parameters["maxSignatureAge"]!.Value<Int32>())
                   }.
                   AddVerificationRules([
                       new VerificationRule(ResetRequest.              DefaultJSONLDContext, 1, action, keyPair),
                       new VerificationRule(GetLocalListVersionRequest.DefaultJSONLDContext, 1, action, keyPair),
                       new VerificationRule(ResetResponse.             DefaultJSONLDContext, 1, action, keyPair)
                   ]).
                   BindTo(NetworkingNode_Id.Parse(verifier["nodeId"]!.Value<String>()!),
                          verifier["ocppVersion"]!.Value<String>()!);

        }

        #endregion

        #region (private) Request(Frame, NodeId)

        /// <summary>
        /// A request as the adapter of the verifying node receives it over a direct OCPP-J
        /// connection: the frame names no destination, so the node itself is it.
        /// </summary>
        private static (IRequest Request, JObject Payload, JObject Parsed) Request(JArray Frame, NetworkingNode_Id NodeId)
        {

            var messageId    = Request_Id.Parse(Frame[1]!.Value<String>()!);
            var payload      = (JObject) Frame[3]!;
            var destination  = SourceRouting.To(NodeId);

            switch (Frame[2]!.Value<String>())
            {

                case "Reset":
                    var reset = ResetRequest.Parse(payload, messageId, destination, NetworkPath.Empty);
                    return (reset, payload, reset.ToJSON(true));

                case "GetLocalListVersion":
                    var getLocalListVersion = GetLocalListVersionRequest.Parse(payload, messageId, destination, NetworkPath.Empty);
                    return (getLocalListVersion, payload, getLocalListVersion.ToJSON(true));

                default:
                    throw new ArgumentException($"No vector action '{Frame[2]}' here!");

            }

        }

        #endregion


        #region TheVectorsAreThere()

        [Test]
        public void TheVectorsAreThere()
        {
            Assert.That(vectors.Value["vectors"]!.Count(), Is.GreaterThanOrEqualTo(15));
        }

        #endregion

        #region TheVerifierDecidesAsTheVectorSays(Id)

        [TestCaseSource(nameof(Cases))]
        public void TheVerifierDecidesAsTheVectorSays(String Id)
        {

            var vector    = Vector(Id);
            var policy    = Verifier(vector);
            var nodeId    = policy.NodeId!.Value;
            var expected  = (JObject) vector["expected"]!;
            var results   = (JArray)  expected["results"]!;
            var frames    = vector["frames"]!.Select(frame => frame.Value<String>()!).ToArray();

            Assert.That(frames.Length, Is.EqualTo(results.Count));

            for (var i = 0; i < frames.Length; i++)
            {

                var frame = JSONMessages.ParseArray(frames[i]);

                ISignableMessage  message;
                JObject           payload;
                Boolean           accepted;
                String?           errorResponse;

                if (frame[0]!.Value<Int32>() == 2)
                {
                    (var request, payload, var parsed) = Request(frame, nodeId);
                    message   = request;
                    accepted  = policy.VerifyRequestMessage(request, parsed, out errorResponse, payload);
                }
                else
                {
                    var (request, _, _) = Request(JSONMessages.ParseArray(vector["request"]!.Value<String>()!), NetworkingNode_Id.Parse("cs01"));
                    payload           = (JObject) frame[2]!;
                    var response      = ResetResponse.Parse((ResetRequest) request, payload, SourceRouting.To(nodeId), NetworkPath.Empty);
                    message           = response;
                    accepted          = policy.VerifyResponseMessage(response, response.ToJSON(true), out errorResponse, payload);
                }

                // The RFC 8785 form and the signing input are the other implementation's.
                var withoutSignatures = (JObject) payload.DeepClone();
                withoutSignatures.Remove("signatures");

                Assert.Multiple(() => {

                    Assert.That(JSONCanonicalization.Canonicalize(withoutSignatures),
                                Is.EqualTo(expected["canonicalPayload"]!.Value<String>()),
                                "The RFC 8785 form differs from the other implementation's!");

                    Assert.That(message.Signatures.Single().Protected + "." + JWS.Base64URL(JSONCanonicalization.CanonicalizeToUTF8(withoutSignatures)),
                                Is.EqualTo(expected["signingInput"]!.Value<String>()),
                                "The signing input differs from the other implementation's!");

                    Assert.That(accepted, Is.EqualTo(results[i]["accepted"]!.Value<Boolean>()),
                                $"Frame {i + 1}: {errorResponse}");

                    Assert.That(message.Signatures.Single().Status.ToString(), Is.EqualTo(results[i]["status"]!.Value<String>()),
                                $"Frame {i + 1}: {errorResponse}");

                });

            }

        }

        #endregion

    }

}
