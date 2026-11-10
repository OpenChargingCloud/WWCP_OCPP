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
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.E2ESecurityExtensions
{

    /// <summary>
    /// What a signature policy does with the messages it signs and verifies.
    /// </summary>
    /// <remarks>
    /// Each test signs a message as an adapter does (the message with its JSON-LD context),
    /// takes the JSON that goes on the wire (without the context), parses it as the receiver
    /// does, and verifies it as the receiving adapter does. Nothing is sent over a network.
    /// The white paper "Signed OCPP Messages and Signer Roles" (OpenChargingTechnology/
    /// Whitepapers, SignedMessages) lists what these tests pin down as findings F1 to F3
    /// and F9 of its chapter 2, and as phase 1 of its chapter 7.
    /// </remarks>
    [TestFixture]
    public class SignaturePolicySemanticsTests
    {

        #region Data

        private static readonly SourceRouting  toTheStation  = SourceRouting.To(NetworkingNode_Id.Parse("cs01"));

        #endregion

        #region (private) Sign(...) / Received(...) / Verify(...)

        /// <summary>
        /// Sign a Reset request by the given policy and keys, and return what goes on the wire.
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

        private static ResetRequest AResetRequest(ResetType? ResetType = null)

            => new (toTheStation, ResetType ?? OCPPv2_1.ResetType.Immediate);

        private static ResetRequest Received(JObject JSON)

            => ResetRequest.Parse(JSON, Request_Id.NewRandom(), toTheStation, NetworkPath.Empty);

        private static Boolean Verify(SignaturePolicy  Verifier,
                                      ResetRequest     Request,
                                      out String?      ErrorResponse)

            => Verifier.VerifyRequestMessage(Request, Request.ToJSON(true), out ErrorResponse);

        private static SignaturePolicy AStationThatWants(VerificationRuleActions  Action,
                                                         KeyPair?                 KeyPair   = null)

            => new SignaturePolicy().AddVerificationRules([
                   new VerificationRule(ResetRequest.DefaultJSONLDContext, 1, Action, KeyPair)
               ]);

        #endregion


        #region Verification

        #region ARequestWhoseSignaturesWereTakenOffIsRefusedByAVerifyAllRule()

        /// <summary>
        /// Taking the signatures off a message must be no way past a rule that wants it
        /// signed. It was: without signatures, the policy's default action decided
        /// (AcceptUnverified), not the rule.
        /// </summary>
        [Test]
        public void ARequestWhoseSignaturesWereTakenOffIsRefusedByAVerifyAllRule()
        {

            var onTheWire = SignedOnTheWire(new SignaturePolicy(), AResetRequest(), ECCKeyPair.GenerateKeys()!.ToSignInfo1());

            Assert.That(onTheWire["signatures"], Is.Not.Null);
            onTheWire.Remove("signatures");

            var received  = Received(onTheWire);

            Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAll), received, out var errorResponse),
                        Is.False,
                        "A request without its signatures got past a rule that wants it signed!");

            Assert.That(errorResponse, Does.Contain("no signature"));

        }

        #endregion

        #region AnUnsignedRequestIsRefusedByAVerifyAnyRule()

        /// <summary>
        /// "VerifyAny" wants at least one valid signature; none at all is not one.
        /// </summary>
        [Test]
        public void AnUnsignedRequestIsRefusedByAVerifyAnyRule()
        {

            var received = Received(AResetRequest().ToJSON(false));

            Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAny), received, out _),
                        Is.False,
                        "An unsigned request got past a rule that wants one valid signature!");

        }

        #endregion

        #region AnUnsignedRequestPassesWhereNothingAsksForSignatures()

        /// <summary>
        /// A station without rules behaves like a standard station: signatures are our own
        /// extension, and a standard CSMS sends none.
        /// </summary>
        [Test]
        public void AnUnsignedRequestPassesWhereNothingAsksForSignatures()
        {

            var received = Received(AResetRequest().ToJSON(false));

            Assert.That(Verify(new SignaturePolicy(), received, out var errorResponse), Is.True, errorResponse);

        }

        #endregion

        #region AnAcceptUnverifiedRuleLetsAnUnsignedRequestThrough()

        [Test]
        public void AnAcceptUnverifiedRuleLetsAnUnsignedRequestThrough()
        {

            var received = Received(AResetRequest().ToJSON(false));

            Assert.That(Verify(AStationThatWants(VerificationRuleActions.AcceptUnverified), received, out var errorResponse),
                        Is.True,
                        errorResponse);

        }

        #endregion

        #region ARequestSignedByTheKeyItNamesPassesARuleWithoutAKey()

        /// <summary>
        /// A rule without a key pair checks the mathematics only: whom the key belongs to
        /// is for the node's user roles to say. This is how every existing set-up uses it.
        /// </summary>
        [Test]
        public void ARequestSignedByTheKeyItNamesPassesARuleWithoutAKey()
        {

            var received = Received(SignedOnTheWire(new SignaturePolicy(), AResetRequest(), ECCKeyPair.GenerateKeys()!.ToSignInfo1()));

            Assert.Multiple(() => {
                Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAll), received, out var errorResponse), Is.True, errorResponse);
                Assert.That(received.Signatures.Single().Status, Is.EqualTo(VerificationStatus.ValidSignature));
            });

        }

        #endregion

        #region ATamperedRequestIsRefused()

        [Test]
        public void ATamperedRequestIsRefused()
        {

            var onTheWire = SignedOnTheWire(new SignaturePolicy(), AResetRequest(ResetType.OnIdle), ECCKeyPair.GenerateKeys()!.ToSignInfo1());

            Assert.That(onTheWire["type"]?.Value<String>(), Is.EqualTo("OnIdle"));
            onTheWire["type"] = "Immediate";

            var received  = Received(onTheWire);

            Assert.Multiple(() => {
                Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAll), received, out _), Is.False,
                            "A request whose reset type was changed after signing passed!");
                Assert.That(received.Signatures.Single().Status, Is.EqualTo(VerificationStatus.InvalidSignature));
            });

        }

        #endregion

        #region ASignatureMovedToAnotherActionIsRefused()

        /// <summary>
        /// What is signed is the payload together with the JSON-LD context of its message,
        /// and the receiver takes that context from the action of the frame. A signed
        /// ClearCache payload put into a GetLocalListVersion frame - both payloads hold
        /// nothing but their signatures - therefore no longer verifies.
        /// </summary>
        [Test]
        public void ASignatureMovedToAnotherActionIsRefused()
        {

            var clearCache = new ClearCacheRequest(toTheStation);

            Assert.That(new SignaturePolicy().SignRequestMessage(clearCache,
                                                                 clearCache.ToJSON(true),
                                                                 out var signingError,
                                                                 ECCKeyPair.GenerateKeys()!.ToSignInfo1()),
                        Is.True,
                        signingError);

            var moved     = GetLocalListVersionRequest.Parse(clearCache.ToJSON(false), Request_Id.NewRandom(), toTheStation, NetworkPath.Empty);

            var verifier  = new SignaturePolicy().AddVerificationRules([
                                new VerificationRule(GetLocalListVersionRequest.DefaultJSONLDContext, 1, VerificationRuleActions.VerifyAll)
                            ]);

            Assert.Multiple(() => {
                Assert.That(moved.Signatures.Count(), Is.EqualTo(1));
                Assert.That(verifier.VerifyRequestMessage(moved, moved.ToJSON(true), out _), Is.False,
                            "A ClearCache signature verified as one of GetLocalListVersion!");
                Assert.That(moved.Signatures.Single().Status, Is.EqualTo(VerificationStatus.InvalidSignature));
            });

        }

        #endregion

        #region ARuleThatNamesAKeyRefusesAnotherSigner()

        /// <summary>
        /// A rule with a key pair accepts signatures by that key only. It used to ignore the
        /// key and take the one the signature itself names, so a stranger's key passed.
        /// </summary>
        [Test]
        public void ARuleThatNamesAKeyRefusesAnotherSigner()
        {

            var trusted   = ECCKeyPair.GenerateKeys()!;
            var stranger  = ECCKeyPair.GenerateKeys()!;

            var received  = Received(SignedOnTheWire(new SignaturePolicy(), AResetRequest(), stranger.ToSignInfo1("mallory")));

            Assert.Multiple(() => {
                Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAll, trusted), received, out _), Is.False,
                            "A request signed by a stranger passed a rule that names another key!");
                Assert.That(received.Signatures.Single().Status, Is.EqualTo(VerificationStatus.UnknownSigner));
            });

        }

        #endregion

        #region ARuleThatNamesAKeyAcceptsItsSigner()

        [Test]
        public void ARuleThatNamesAKeyAcceptsItsSigner()
        {

            var trusted   = ECCKeyPair.GenerateKeys()!;

            var received  = Received(SignedOnTheWire(new SignaturePolicy(), AResetRequest(), trusted.ToSignInfo1("admin")));

            Assert.Multiple(() => {
                Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAll, trusted), received, out var errorResponse), Is.True, errorResponse);
                Assert.That(received.Signatures.Single().Status, Is.EqualTo(VerificationStatus.ValidSignature));
            });

        }

        #endregion

        #region VerifyAnyAcceptsTheNamedSignerBesideAStranger()

        /// <summary>
        /// "VerifyAny" is satisfied by the signature of the key it names, whatever else the
        /// message carries; the stranger's signature is still told apart.
        /// </summary>
        [Test]
        public void VerifyAnyAcceptsTheNamedSignerBesideAStranger()
        {

            var trusted   = ECCKeyPair.GenerateKeys()!;
            var stranger  = ECCKeyPair.GenerateKeys()!;

            var received  = Received(SignedOnTheWire(new SignaturePolicy(), AResetRequest(), stranger.ToSignInfo1("mallory"), trusted.ToSignInfo1("admin")));

            Assert.Multiple(() => {
                Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAny, trusted), received, out var errorResponse), Is.True, errorResponse);
                Assert.That(received.Signatures.Single(signature => signature.Name == "mallory").Status, Is.EqualTo(VerificationStatus.UnknownSigner));
                Assert.That(received.Signatures.Single(signature => signature.Name == "admin").  Status, Is.EqualTo(VerificationStatus.ValidSignature));
            });

        }

        #endregion

        #region DropAndRejectRulesRefuseTheRequest(Action)

        /// <summary>
        /// "Drop" and "Reject" used to mark the signatures and let the message through.
        /// </summary>
        [TestCase(VerificationRuleActions.Drop,   VerificationStatus.DropMessage)]
        [TestCase(VerificationRuleActions.Reject, VerificationStatus.RejectMessage)]
        public void DropAndRejectRulesRefuseTheRequest(VerificationRuleActions  Action,
                                                       VerificationStatus       Status)
        {

            var received = Received(SignedOnTheWire(new SignaturePolicy(), AResetRequest(), ECCKeyPair.GenerateKeys()!.ToSignInfo1()));

            Assert.Multiple(() => {
                Assert.That(Verify(AStationThatWants(Action), received, out _), Is.False,
                            $"A '{Action}' rule let the request through!");
                Assert.That(received.Signatures.Single().Status, Is.EqualTo(Status));
            });

        }

        #endregion

        #region AReplayedRequestStillVerifies()

        /// <summary>
        /// Today's gap, written down so that it is not forgotten: nothing in a signature binds
        /// the message ID, the destination or a time, so the same signed request verifies as
        /// often as it is sent. Phase 2 of the white paper (a protected header with the OCPP
        /// context, and a replay cache) closes it, and turns this test around.
        /// </summary>
        [Test]
        public void AReplayedRequestStillVerifies()
        {

            var trusted    = ECCKeyPair.GenerateKeys()!;
            var onTheWire  = SignedOnTheWire(new SignaturePolicy(), AResetRequest(), trusted.ToSignInfo1());
            var station    = AStationThatWants(VerificationRuleActions.VerifyAll, trusted);

            Assert.Multiple(() => {
                Assert.That(Verify(station, Received((JObject) onTheWire.DeepClone()), out var first),  Is.True, first);
                Assert.That(Verify(station, Received((JObject) onTheWire.DeepClone()), out var second), Is.True, second);
            });

        }

        #endregion

        #endregion

        #region Signing

        #region AForwardUnsignedRuleDoesNotSign()

        /// <summary>
        /// Every matching rule with a key used to sign, whatever its action said.
        /// </summary>
        [Test]
        public void AForwardUnsignedRuleDoesNotSign()
        {

            var signer   = new SignaturePolicy().AddSigningRules([
                               new SigningRule(ResetRequest.DefaultJSONLDContext, 1, SigningRuleActions.ForwardUnsigned, ECCKeyPair.GenerateKeys()!)
                           ]);

            var request  = AResetRequest();

            Assert.Multiple(() => {
                Assert.That(signer.SignRequestMessage(request, request.ToJSON(true), out var errorResponse), Is.True, errorResponse);
                Assert.That(request.Signatures, Is.Empty, "A 'ForwardUnsigned' rule signed the request!");
            });

        }

        #endregion

        #region DropAndRejectSigningRulesRefuseToSend(Action)

        [TestCase(SigningRuleActions.Drop)]
        [TestCase(SigningRuleActions.Reject)]
        public void DropAndRejectSigningRulesRefuseToSend(SigningRuleActions Action)
        {

            var signer   = new SignaturePolicy().AddSigningRules([
                               new SigningRule(ResetRequest.DefaultJSONLDContext, 1, Action, ECCKeyPair.GenerateKeys()!)
                           ]);

            var request  = AResetRequest();

            Assert.Multiple(() => {
                Assert.That(signer.SignRequestMessage(request, request.ToJSON(true), out _), Is.False,
                            $"A '{Action}' signing rule let the request go out!");
                Assert.That(request.Signatures, Is.Empty);
            });

        }

        #endregion

        #region TheRuleOfHighestPriorityDecides()

        /// <summary>
        /// A "ForwardUnsigned" rule of higher priority keeps a "Sign" rule of lower priority
        /// from signing.
        /// </summary>
        [Test]
        public void TheRuleOfHighestPriorityDecides()
        {

            var signer   = new SignaturePolicy().AddSigningRules([
                               new SigningRule(ResetRequest.DefaultJSONLDContext,                                            1, SigningRuleActions.Sign,            ECCKeyPair.GenerateKeys()!),
                               new SigningRule(JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/..."), 2, SigningRuleActions.ForwardUnsigned, ECCKeyPair.GenerateKeys()!)
                           ]);

            var request  = AResetRequest();

            Assert.Multiple(() => {
                Assert.That(signer.SignRequestMessage(request, request.ToJSON(true), out var errorResponse), Is.True, errorResponse);
                Assert.That(request.Signatures, Is.Empty);
            });

        }

        #endregion

        #region EveryMatchingSignRuleSigns()

        /// <summary>
        /// Below a "Sign" rule of highest priority, every matching "Sign" rule adds its
        /// signature, a pattern ending in "..." included.
        /// </summary>
        [Test]
        public void EveryMatchingSignRuleSigns()
        {

            var signer   = new SignaturePolicy().AddSigningRules([
                               new SigningRule(ResetRequest.DefaultJSONLDContext,                                            2, SigningRuleActions.Sign, ECCKeyPair.GenerateKeys()!),
                               new SigningRule(JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/..."), 1, SigningRuleActions.Sign, ECCKeyPair.GenerateKeys()!)
                           ]);

            var request  = AResetRequest();

            Assert.Multiple(() => {
                Assert.That(signer.SignRequestMessage(request, request.ToJSON(true), out var errorResponse), Is.True, errorResponse);
                Assert.That(request.Signatures.Count(), Is.EqualTo(2));
            });

        }

        #endregion

        #region TheDefaultKeySignsWhatNoRuleCovers()

        /// <summary>
        /// A policy whose default action is "Sign" signs with its default key. It did not:
        /// the default action and key were never read.
        /// </summary>
        [Test]
        public void TheDefaultKeySignsWhatNoRuleCovers()
        {

            var defaultKey  = ECCKeyPair.GenerateKeys()!;
            var signer      = new SignaturePolicy(DefaultSigningAction:   SigningRuleActions.Sign,
                                                  DefaultSigningKeyPair:  defaultKey);

            var request     = AResetRequest();

            Assert.Multiple(() => {
                Assert.That(signer.SignRequestMessage(request, request.ToJSON(true), out var errorResponse), Is.True, errorResponse);
                Assert.That(request.Signatures.Count(), Is.EqualTo(1), "The default key did not sign!");
                Assert.That(request.Signatures.FirstOrDefault()?.KeyId, Is.EqualTo(defaultKey.PublicKeyBytes));
            });

        }

        #endregion

        #endregion

    }

}
