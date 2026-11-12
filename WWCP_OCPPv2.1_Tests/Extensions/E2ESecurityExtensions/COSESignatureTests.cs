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

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

using cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.E2ESecurityExtensions
{

    /// <summary>
    /// A signature may be a COSE_Sign1 over the deterministic CBOR of its message, next to
    /// or instead of the JWS over its JSON (white paper "Signed OCPP Messages and Signer
    /// Roles", section 4.2; OCPP-CBOR-Concept, decision 3). Its protected header is that of
    /// the JWS, and it is held to the same: the frame, the time, and once only.
    /// </summary>
    /// <remarks>
    /// As in SignaturePolicySemanticsTests each test signs a message as an adapter does,
    /// takes what goes on the wire, parses it as the receiver does and verifies it as the
    /// receiving adapter does. Nothing is sent over a network.
    /// </remarks>
    [TestFixture]
    public class COSESignatureTests
    {

        #region Data

        private static readonly SourceRouting  toTheStation  = SourceRouting.To(NetworkingNode_Id.Parse("cs01"));
        private static readonly Request_Id     theRequestId  = Request_Id.Parse("4711");

        /// <summary>
        /// The requests that have a sample with every property of their schema.
        /// </summary>
        public static IEnumerable<String> SampledRequests
            => MessageSamples.All.Keys.Where(name => name.EndsWith("Request"));

        #endregion

        #region (private) helpers

        /// <summary>
        /// A signature policy as a node's adapter binds it: it knows the CBOR of a message.
        /// </summary>
        private static SignaturePolicy APolicy(SignatureFormats Formats = SignatureFormats.COSE)

            => new () {
                   Formats        = Formats,
                   CBORPayloadOf  = OCPPMessagesAsCBOR.SignedPayloadOf
               };

        private static SignaturePolicy AStationThatWants(VerificationRuleActions  Action,
                                                         KeyPair?                 KeyPair   = null)

            => APolicy().AddVerificationRules([
                   new VerificationRule(ResetRequest.DefaultJSONLDContext, 1, Action, KeyPair)
               ]);

        private static ResetRequest AResetRequest()
            => new (toTheStation, ResetType.Immediate, RequestId: theRequestId);

        /// <summary>
        /// Sign a Reset request, and return what goes on the wire as JSON.
        /// </summary>
        private static JObject SignedOnTheWire(SignaturePolicy    Signer,
                                               ResetRequest       Request,
                                               params SignInfo[]  SignInfos)
        {

            Assert.That(Signer.SignRequestMessage(Request, Request.ToJSON(true), out var errorResponse, SignInfos),
                        Is.True,
                        errorResponse);

            return Request.ToJSON(false);

        }

        private static ResetRequest Received(JObject      JSON,
                                             Request_Id?  RequestId   = null)

            => ResetRequest.Parse(JSON, RequestId ?? theRequestId, toTheStation, NetworkPath.Empty);

        private static Boolean Verify(SignaturePolicy  Verifier,
                                      ResetRequest     Request,
                                      out String?      ErrorResponse)

            => Verifier.VerifyRequestMessage(Request, Request.ToJSON(true), out ErrorResponse, Request.ToJSON(false));

        #endregion


        #region ACOSESignature_IsValid_BehindAJSONConnection()

        /// <summary>
        /// A COSE signature signs the CBOR of the message, which every node that has the
        /// class of the message builds the same way. In a JSON message it is the bytes of
        /// its COSE_Sign1, and is verified there as well.
        /// </summary>
        [Test]
        public void ACOSESignature_IsValid_BehindAJSONConnection()
        {

            var onTheWire  = SignedOnTheWire(APolicy(), AResetRequest(), ECCKeyPair.GenerateKeys()!.ToSignInfo1());
            var signatures = onTheWire["signatures"] as JArray;

            Assert.That(signatures,                      Has.Count.EqualTo(1));
            Assert.That(signatures![0]["cose"]?.Type,    Is.EqualTo(JTokenType.String));
            Assert.That(signatures [0]["protected"],     Is.Null, "A COSE signature is no JWS");

            var received   = Received(onTheWire);

            Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAll), received, out var errorResponse),
                        Is.True,
                        errorResponse);

            var signature  = received.Signatures.Single();

            Assert.Multiple(() => {
                Assert.That(signature.IsCOSE,                                           Is.True);
                Assert.That(signature.IsJWS,                                            Is.False);
                Assert.That(signature.Status,                                           Is.EqualTo(VerificationStatus.ValidSignature));
                Assert.That(signature.ProtectedHeader?["alg"]?.Value<String>(),         Is.EqualTo("ES256"));
                Assert.That(signature.ProtectedHeader?["OCPPAction"]?.Value<String>(),  Is.EqualTo("Reset"));
                Assert.That(signature.ProtectedHeader?["OCPPMessageId"]?.Value<String>(), Is.EqualTo(theRequestId.ToString()));
                Assert.That(signature.KeyId,                                            Is.Not.Empty, "The key the signature carries");
            });

        }

        #endregion

        #region ACOSESignature_IsValid_BehindACBORConnection()

        /// <summary>
        /// In the CBOR of a message a COSE signature is the COSE_Sign1 it is, under tag 18,
        /// and what was read from CBOR verifies as what was read from JSON does.
        /// </summary>
        [Test]
        public void ACOSESignature_IsValid_BehindACBORConnection()
        {

            var onTheWire = SignedOnTheWire(APolicy(), AResetRequest(), ECCKeyPair.GenerateKeys()!.ToSignInfo1());

            Assert.That(OCPPMessagesAsCBOR.TryRequestToCBOR("Reset", onTheWire, theRequestId, toTheStation, NetworkPath.Empty, out var cbor, out _, out var e1),
                        Is.True, e1);

            var bytes     = cbor.ToByteArray();
            var read      = CBORValue.Parse(bytes);

            Assert.That(read.TryGetValue(CBORValue.FromText("signatures"), out var signatures),  Is.True);
            Assert.That(signatures.AsArray(),                                                     Has.Count.EqualTo(1));
            Assert.That(signatures.AsArray()[0].Kind,                                             Is.EqualTo(CBORValueKind.Tagged));
            Assert.That(COSESign1.TryParse(signatures.AsArray()[0], out var sign1, out var e2),   Is.True, e2);
            Assert.That(sign1!.IsDetached,                                                        Is.True);

            Assert.That(OCPPMessagesAsCBOR.TryRequestToJSON("Reset", read, theRequestId, toTheStation, NetworkPath.Empty, out _, out var request, out var e3),
                        Is.True, e3);

            var received  = (ResetRequest) request!;

            Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAll), received, out var errorResponse),
                        Is.True,
                        errorResponse);

            Assert.That(received.Signatures.Single().Status, Is.EqualTo(VerificationStatus.ValidSignature));

        }

        #endregion

        #region BothSignatures_StandNextToEachOther()

        /// <summary>
        /// A node set to write both writes both: neither signs the other, and both are valid.
        /// </summary>
        [Test]
        public void BothSignatures_StandNextToEachOther()
        {

            var onTheWire = SignedOnTheWire(APolicy(SignatureFormats.Both), AResetRequest(), ECCKeyPair.GenerateKeys()!.ToSignInfo1());
            var received  = Received(onTheWire);

            Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAll), received, out var errorResponse),
                        Is.True,
                        errorResponse);

            Assert.Multiple(() => {
                Assert.That(received.Signatures.Count(),                                  Is.EqualTo(2));
                Assert.That(received.Signatures.Count(signature => signature.IsJWS),      Is.EqualTo(1));
                Assert.That(received.Signatures.Count(signature => signature.IsCOSE),     Is.EqualTo(1));
                Assert.That(received.Signatures.All  (signature => signature.Status == VerificationStatus.ValidSignature), Is.True);
            });

        }

        #endregion

        #region ByDefault_ANodeWritesTheJWSAlone()

        [Test]
        public void ByDefault_ANodeWritesTheJWSAlone()
        {

            var onTheWire = SignedOnTheWire(new SignaturePolicy(), AResetRequest(), ECCKeyPair.GenerateKeys()!.ToSignInfo1());
            var received  = Received(onTheWire);

            Assert.That(received.Signatures.Single().IsJWS, Is.True);

        }

        #endregion

        #region AMessageThatWasChanged_IsRefused()

        [Test]
        public void AMessageThatWasChanged_IsRefused()
        {

            var onTheWire = SignedOnTheWire(APolicy(), AResetRequest(), ECCKeyPair.GenerateKeys()!.ToSignInfo1());

            onTheWire["type"] = ResetType.OnIdle.ToString();

            var received  = Received(onTheWire);

            Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAll), received, out var errorResponse),
                        Is.False,
                        "A changed message got past its COSE signature!");

            Assert.That(received.Signatures.Single().Status, Is.EqualTo(VerificationStatus.InvalidSignature));

        }

        #endregion

        #region ASignatureForAnotherFrame_IsRefused()

        /// <summary>
        /// The protected header names the message id, as that of a JWS does: the same
        /// message under another id is another frame.
        /// </summary>
        [Test]
        public void ASignatureForAnotherFrame_IsRefused()
        {

            var onTheWire = SignedOnTheWire(APolicy(), AResetRequest(), ECCKeyPair.GenerateKeys()!.ToSignInfo1());
            var received  = Received(onTheWire, Request_Id.Parse("4712"));

            Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAll), received, out var errorResponse),
                        Is.False);

            Assert.That(received.Signatures.Single().Status, Is.EqualTo(VerificationStatus.WrongFrame));

        }

        #endregion

        #region TheSameMessageASecondTime_IsAReplay()

        [Test]
        public void TheSameMessageASecondTime_IsAReplay()
        {

            var onTheWire = SignedOnTheWire(APolicy(), AResetRequest(), ECCKeyPair.GenerateKeys()!.ToSignInfo1());
            var station   = AStationThatWants(VerificationRuleActions.VerifyAll);

            Assert.That(Verify(station, Received(onTheWire), out var first),   Is.True, first);

            var again     = Received(onTheWire);

            Assert.That(Verify(station, again, out _),                         Is.False);
            Assert.That(again.Signatures.Single().Status,                      Is.EqualTo(VerificationStatus.Replayed));

        }

        #endregion

        #region TheKeyOfTheRule_IsTheOnlyOneThatSigns()

        [Test]
        public void TheKeyOfTheRule_IsTheOnlyOneThatSigns()
        {

            var theirs    = ECCKeyPair.GenerateKeys()!;
            var another   = ECCKeyPair.GenerateKeys()!;

            var byTheirs  = Received(SignedOnTheWire(APolicy(), AResetRequest(), theirs. ToSignInfo1()));
            var byAnother = Received(SignedOnTheWire(APolicy(), AResetRequest(), another.ToSignInfo1()));

            Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAll, theirs), byTheirs,  out var e1),  Is.True, e1);
            Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAll, theirs), byAnother, out _),       Is.False);
            Assert.That(byAnother.Signatures.Single().Status,                                                         Is.EqualTo(VerificationStatus.UnknownSigner));

        }

        #endregion

        #region ANodeWithoutTheCBOROfTheMessage_CannotVerifyIt()

        /// <summary>
        /// A COSE signature takes the class of the message to verify. A policy nobody told
        /// what the CBOR of a message is refuses it - it does not wave it through.
        /// </summary>
        [Test]
        public void ANodeWithoutTheCBOROfTheMessage_CannotVerifyIt()
        {

            var onTheWire = SignedOnTheWire(APolicy(), AResetRequest(), ECCKeyPair.GenerateKeys()!.ToSignInfo1());
            var received  = Received(onTheWire);

            var station   = new SignaturePolicy().AddVerificationRules([
                                new VerificationRule(ResetRequest.DefaultJSONLDContext, 1, VerificationRuleActions.VerifyAll)
                            ]);

            Assert.That(Verify(station, received, out _),      Is.False);
            Assert.That(received.Signatures.Single().Status,   Is.EqualTo(VerificationStatus.BrokenSignature));

        }

        #endregion

        #region ANodeWithoutTheCBOROfTheMessage_CannotSignIt()

        [Test]
        public void ANodeWithoutTheCBOROfTheMessage_CannotSignIt()
        {

            var request = AResetRequest();

            Assert.That(new SignaturePolicy { Formats = SignatureFormats.COSE }.
                            SignRequestMessage(request, request.ToJSON(true), out var errorResponse, ECCKeyPair.GenerateKeys()!.ToSignInfo1()),
                        Is.False);

            Assert.That(errorResponse, Does.Contain("no CBOR"));

        }

        #endregion

        #region EveryRequest_IsSignedAndVerified_BehindJSONAndBehindCBOR(Name)

        /// <summary>
        /// What a COSE signature signs is the same wherever the message is read: the sample
        /// of every request is signed, crosses JSON and crosses CBOR, and verifies behind both.
        /// </summary>
        [TestCaseSource(nameof(SampledRequests))]
        public void EveryRequest_IsSignedAndVerified_BehindJSONAndBehindCBOR(String Name)
        {

            var action       = Name[..^"Request".Length];
            var destination  = SourceRouting.CSMS;
            var sample       = JObject.Parse(MessageSamples.All[Name]);

            // A sample may bring signatures of its own, which sign nothing here.
            sample.Remove("signatures");

            Assert.That(OCPPMessagesAsCBOR.TryReadRequest(action, sample, theRequestId, destination, NetworkPath.Empty, out var asked, out var e1),
                        Is.True, e1);

            var request      = (IRequest) asked!;

            Assert.That(APolicy().SignRequestMessage(request, (JObject) sample.DeepClone(), out var e2, ECCKeyPair.GenerateKeys()!.ToSignInfo1()),
                        Is.True, e2);

            var onTheWire    = (JObject) sample.DeepClone();
            onTheWire["signatures"] = new JArray(request.Signatures.Select(signature => signature.ToJSON()));

            #region Behind a JSON connection

            Assert.That(OCPPMessagesAsCBOR.TryReadRequest(action, onTheWire, theRequestId, destination, NetworkPath.Empty, out var readFromJSON, out var e3),
                        Is.True, e3);

            var behindJSON   = (IRequest) readFromJSON!;

            Assert.That(APolicy().AddVerificationRules([ new VerificationRule(behindJSON.Context, 1, VerificationRuleActions.VerifyAll) ]).
                            VerifyRequestMessage(behindJSON, onTheWire, out var e4, onTheWire),
                        Is.True, e4);

            #endregion

            #region Behind a CBOR connection

            Assert.That(OCPPMessagesAsCBOR.TryRequestToCBOR(action, onTheWire, theRequestId, destination, NetworkPath.Empty, out var cbor, out _, out var e5),
                        Is.True, e5);

            Assert.That(OCPPMessagesAsCBOR.TryRequestToJSON(action, CBORValue.Parse(cbor.ToByteArray()), theRequestId, destination, NetworkPath.Empty, out var json, out var readFromCBOR, out var e6),
                        Is.True, e6);

            var behindCBOR   = (IRequest) readFromCBOR!;

            Assert.That(APolicy().AddVerificationRules([ new VerificationRule(behindCBOR.Context, 1, VerificationRuleActions.VerifyAll) ]).
                            VerifyRequestMessage(behindCBOR, json!, out var e7, json),
                        Is.True, e7);

            #endregion

        }

        #endregion

    }

}
