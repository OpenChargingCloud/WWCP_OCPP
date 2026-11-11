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
    /// does - under the message id and destination of the frame - and verifies it as the
    /// receiving adapter does, against the payload as received. Nothing is sent over a network.
    /// The white paper "Signed OCPP Messages and Signer Roles" (OpenChargingTechnology/
    /// Whitepapers, SignedMessages) lists what these tests pin down as findings F1 to F4, F6,
    /// F7 and F9 of its chapter 2, and as phases 1 and 2 of its chapter 7.
    /// </remarks>
    [TestFixture]
    public class SignaturePolicySemanticsTests
    {

        #region Data

        private static readonly SourceRouting  toTheStation    = SourceRouting.To(NetworkingNode_Id.Parse("cs01"));
        private static readonly SourceRouting  toAnother       = SourceRouting.To(NetworkingNode_Id.Parse("cs02"));
        private static readonly Request_Id     theRequestId    = Request_Id.Parse("4711");

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

            => new (toTheStation, ResetType ?? OCPPv2_1.ResetType.Immediate, RequestId: theRequestId);

        /// <summary>
        /// The request as the receiver parses it: under the message id and the destination of its frame.
        /// </summary>
        private static ResetRequest Received(JObject         JSON,
                                             Request_Id?     RequestId     = null,
                                             SourceRouting?  Destination   = null)

            => ResetRequest.Parse(JSON, RequestId ?? theRequestId, Destination ?? toTheStation, NetworkPath.Empty);

        /// <summary>
        /// Verify as the receiving adapter does: against the payload as received, when it is given.
        /// </summary>
        private static Boolean Verify(SignaturePolicy  Verifier,
                                      ResetRequest     Request,
                                      out String?      ErrorResponse,
                                      JObject?         ReceivedJSON   = null)

            => Verifier.VerifyRequestMessage(Request, Request.ToJSON(true), out ErrorResponse, ReceivedJSON);

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
        /// The protected header names the action, and the receiver takes the action from the
        /// frame. A signed ClearCache payload put into a GetLocalListVersion frame - both
        /// payloads hold nothing but their signatures, so the mathematics still holds - is
        /// refused because its header names another action.
        /// </summary>
        [Test]
        public void ASignatureMovedToAnotherActionIsRefused()
        {

            var clearCache = new ClearCacheRequest(toTheStation, RequestId: theRequestId);

            Assert.That(new SignaturePolicy().SignRequestMessage(clearCache,
                                                                 clearCache.ToJSON(true),
                                                                 out var signingError,
                                                                 ECCKeyPair.GenerateKeys()!.ToSignInfo1()),
                        Is.True,
                        signingError);

            var onTheWire = clearCache.ToJSON(false);
            var moved     = GetLocalListVersionRequest.Parse(onTheWire, theRequestId, toTheStation, NetworkPath.Empty);

            var verifier  = new SignaturePolicy().AddVerificationRules([
                                new VerificationRule(GetLocalListVersionRequest.DefaultJSONLDContext, 1, VerificationRuleActions.VerifyAll)
                            ]);

            Assert.Multiple(() => {
                Assert.That(moved.Signatures.Count(), Is.EqualTo(1));
                Assert.That(verifier.VerifyRequestMessage(moved, moved.ToJSON(true), out var errorResponse, onTheWire), Is.False,
                            "A ClearCache signature verified as one of GetLocalListVersion!");
                Assert.That(moved.Signatures.Single().Status, Is.EqualTo(VerificationStatus.WrongFrame));
                Assert.That(errorResponse, Does.Contain("ClearCache"));
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

        #region AReplayedRequestIsRefused()

        /// <summary>
        /// Phase 1 wrote this gap down as "AReplayedRequestStillVerifies": nothing bound the
        /// message id, the destination or a time, so the same signed request verified as often
        /// as it was sent. The protected header and the replay cache close it: the second
        /// time, the signature has been seen.
        /// </summary>
        [Test]
        public void AReplayedRequestIsRefused()
        {

            var trusted    = ECCKeyPair.GenerateKeys()!;
            var onTheWire  = SignedOnTheWire(new SignaturePolicy(), AResetRequest(), trusted.ToSignInfo1());
            var station    = AStationThatWants(VerificationRuleActions.VerifyAll, trusted);
            var replayed   = Received((JObject) onTheWire.DeepClone());

            Assert.Multiple(() => {
                Assert.That(Verify(station, Received((JObject) onTheWire.DeepClone()), out var first,  onTheWire), Is.True,  first);
                Assert.That(Verify(station, replayed,                                  out var second, onTheWire), Is.False, "A replayed request verified!");
                Assert.That(replayed.Signatures.Single().Status, Is.EqualTo(VerificationStatus.Replayed));
            });

        }

        #endregion

        #region ASignatureForAnotherMessageIdIsRefused()

        /// <summary>
        /// The same signed payload under another message id is another frame.
        /// </summary>
        [Test]
        public void ASignatureForAnotherMessageIdIsRefused()
        {

            var trusted    = ECCKeyPair.GenerateKeys()!;
            var onTheWire  = SignedOnTheWire(new SignaturePolicy(), AResetRequest(), trusted.ToSignInfo1());
            var received   = Received(onTheWire, Request_Id.Parse("4712"));

            Assert.Multiple(() => {
                Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAll, trusted), received, out var errorResponse, onTheWire), Is.False);
                Assert.That(received.Signatures.Single().Status, Is.EqualTo(VerificationStatus.WrongFrame));
                Assert.That(errorResponse, Does.Contain("4711"));
            });

        }

        #endregion

        #region ARequestSignedForAnotherStationIsRefused()

        /// <summary>
        /// A request signed for cs01 does not execute on cs02, which trusts the same key:
        /// the destination is in the protected header.
        /// </summary>
        [Test]
        public void ARequestSignedForAnotherStationIsRefused()
        {

            var trusted    = ECCKeyPair.GenerateKeys()!;
            var csms       = new SignaturePolicy().BindTo(NetworkingNode_Id.CSMS, Version.WebSocketSubProtocolId);
            var onTheWire  = SignedOnTheWire(csms, AResetRequest(), trusted.ToSignInfo1());
            var received   = Received(onTheWire, Destination: toAnother);

            var cs02       = AStationThatWants(VerificationRuleActions.VerifyAll, trusted).
                                 BindTo(NetworkingNode_Id.Parse("cs02"), Version.WebSocketSubProtocolId);

            Assert.Multiple(() => {
                Assert.That(Verify(cs02, received, out var errorResponse, onTheWire), Is.False, "A request for cs01 executed on cs02!");
                Assert.That(received.Signatures.Single().Status, Is.EqualTo(VerificationStatus.WrongFrame));
                Assert.That(errorResponse, Does.Contain("cs01"));
            });

        }

        #endregion

        #region ASignatureForAnotherProtocolVersionIsRefused()

        [Test]
        public void ASignatureForAnotherProtocolVersionIsRefused()
        {

            var trusted    = ECCKeyPair.GenerateKeys()!;
            var csms       = new SignaturePolicy().BindTo(NetworkingNode_Id.CSMS, "ocpp2.0.1");
            var onTheWire  = SignedOnTheWire(csms, AResetRequest(), trusted.ToSignInfo1());
            var received   = Received(onTheWire);

            var station    = AStationThatWants(VerificationRuleActions.VerifyAll, trusted).
                                 BindTo(NetworkingNode_Id.Parse("cs01"), Version.WebSocketSubProtocolId);

            Assert.Multiple(() => {
                Assert.That(Verify(station, received, out var errorResponse, onTheWire), Is.False);
                Assert.That(received.Signatures.Single().Status, Is.EqualTo(VerificationStatus.WrongFrame));
                Assert.That(errorResponse, Does.Contain("ocpp2.0.1"));
            });

        }

        #endregion

        #region AnExpiredSignatureIsRefused() / ASignatureFromTheFutureIsRefused()

        /// <summary>
        /// Without "exp", a signature is valid for MaxSignatureAge after its "iat".
        /// </summary>
        [Test]
        public void AnExpiredSignatureIsRefused()
        {

            var trusted    = ECCKeyPair.GenerateKeys()!;
            var onTheWire  = SignedOnTheWire(new SignaturePolicy(), AResetRequest(), trusted.ToSignInfo1());
            var received   = Received(onTheWire);

            var station    = AStationThatWants(VerificationRuleActions.VerifyAll, trusted);
            station.Now    = () => DateTimeOffset.UtcNow + station.MaxSignatureAge + TimeSpan.FromSeconds(2);

            Assert.Multiple(() => {
                Assert.That(Verify(station, received, out _, onTheWire), Is.False, "An expired signature verified!");
                Assert.That(received.Signatures.Single().Status, Is.EqualTo(VerificationStatus.OutOfTime));
            });

        }

        /// <summary>
        /// A signature made further in the future than MaxClockSkew is refused.
        /// </summary>
        [Test]
        public void ASignatureFromTheFutureIsRefused()
        {

            var trusted    = ECCKeyPair.GenerateKeys()!;
            var signer     = new SignaturePolicy { Now = () => DateTimeOffset.UtcNow + TimeSpan.FromMinutes(10) };
            var onTheWire  = SignedOnTheWire(signer, AResetRequest(), trusted.ToSignInfo1());
            var received   = Received(onTheWire);

            Assert.Multiple(() => {
                Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAll, trusted), received, out _, onTheWire), Is.False);
                Assert.That(received.Signatures.Single().Status, Is.EqualTo(VerificationStatus.OutOfTime));
            });

        }

        #endregion

        #region AFieldTheParserDoesNotKnowIsCovered()

        /// <summary>
        /// The signature covers the payload as received (RFC 8785), not what the parser
        /// understood of it: a field the receiver does not know is signed too, and changing
        /// it breaks the signature (finding F7).
        /// </summary>
        [Test]
        public void AFieldTheParserDoesNotKnowIsCovered()
        {

            var trusted   = ECCKeyPair.GenerateKeys()!;
            var request   = AResetRequest();
            var withField = request.ToJSON(true);
            withField["vendorHint"] = "keep the lights on";

            Assert.That(new SignaturePolicy().SignRequestMessage(request, withField, out var signingError, trusted.ToSignInfo1()), Is.True, signingError);

            var onTheWire = request.ToJSON(false);
            onTheWire["vendorHint"] = "keep the lights on";

            var changed   = (JObject) onTheWire.DeepClone();
            changed["vendorHint"] = "turn the lights off";

            var received  = Received(onTheWire);
            var tampered  = Received(changed);

            Assert.Multiple(() => {
                Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAll, trusted), received, out var errorResponse, onTheWire), Is.True, errorResponse);
                Assert.That(Verify(AStationThatWants(VerificationRuleActions.VerifyAll, trusted), tampered, out _,                 changed),   Is.False, "A changed field the parser does not know verified!");
                Assert.That(tampered.Signatures.Single().Status, Is.EqualTo(VerificationStatus.InvalidSignature));
            });

        }

        #endregion

        #region AFormat0SignatureCountsAsUnverified()

        /// <summary>
        /// A signature of the old format binds neither the frame nor a time. It is not
        /// accepted by default, and even where it is, it never satisfies a rule that wants
        /// signatures (section 4.2 of the white paper).
        /// </summary>
        [Test]
        public void AFormat0SignatureCountsAsUnverified()
        {

            var trusted   = ECCKeyPair.GenerateKeys()!;
            var request   = AResetRequest();

            // As signature policies signed before phase 2: the compact JSON with its context,
            // hashed with SHA-256, a DER signature, and the raw public key as the key id.
            var plainText = request.ToJSON(true).ToString(Newtonsoft.Json.Formatting.None, SignableMessage.DefaultJSONConverters);
            var signer    = Org.BouncyCastle.Security.SignerUtilities.GetSigner("NONEwithECDSA");
            signer.Init(true, trusted.PrivateKey);
            signer.BlockUpdate(System.Security.Cryptography.SHA256.HashData(plainText.ToUTF8Bytes()));
            request.AddSignature(new Signature(trusted.PublicKey.Q.GetEncoded(), signer.GenerateSignature()));

            Assert.That(request.Signatures.Single().IsJWS, Is.False);

            var onTheWire = request.ToJSON(false);

            var strict    = AStationThatWants(VerificationRuleActions.VerifyAny);
            var lenient   = AStationThatWants(VerificationRuleActions.VerifyAny);
            lenient.AcceptFormat0 = true;

            var first     = Received(onTheWire);
            var second    = Received(onTheWire);
            var third     = Received(onTheWire);

            Assert.Multiple(() => {
                Assert.That(Verify(strict,  first,  out _, onTheWire), Is.False);
                Assert.That(first. Signatures.Single().Status, Is.EqualTo(VerificationStatus.Format0NotAccepted));
                Assert.That(Verify(lenient, second, out _, onTheWire), Is.False);
                Assert.That(second.Signatures.Single().Status, Is.EqualTo(VerificationStatus.Unverified));
                Assert.That(Verify(AStationThatWants(VerificationRuleActions.AcceptUnverified), third, out var errorResponse, onTheWire), Is.True, errorResponse);
            });

        }

        #endregion

        #region AResponseToAnotherRequestIsRefused()

        /// <summary>
        /// A signed response names the message id of its request and, when the request was
        /// signed, the hash of the request's signatures. Here a CSMS signs two requests under
        /// the same message id; the station's signed answer to the first one is refused as
        /// the answer to the second.
        /// </summary>
        [Test]
        public void AResponseToAnotherRequestIsRefused()
        {

            var csmsKey       = ECCKeyPair.GenerateKeys()!;
            var stationKey    = ECCKeyPair.GenerateKeys()!;

            var csms          = new SignaturePolicy().BindTo(NetworkingNode_Id.CSMS,             Version.WebSocketSubProtocolId);
            var station       = new SignaturePolicy().BindTo(NetworkingNode_Id.Parse("cs01"),    Version.WebSocketSubProtocolId);

            var immediately   = AResetRequest(ResetType.Immediate);
            var onIdle        = AResetRequest(ResetType.OnIdle);
            SignedOnTheWire(csms, immediately, csmsKey.ToSignInfo1());
            SignedOnTheWire(csms, onIdle,      csmsKey.ToSignInfo1());

            // The station answers the first request as received.
            var received      = Received(immediately.ToJSON(false));
            var response      = new ResetResponse(received, ResetStatus.Accepted);

            Assert.That(station.SignResponseMessage(response, response.ToJSON(true), out var signingError, stationKey.ToSignInfo1()), Is.True, signingError);

            var onTheWire     = response.ToJSON(false);
            var csmsVerifies  = new SignaturePolicy().AddVerificationRules([
                                    new VerificationRule(ResetResponse.DefaultJSONLDContext, 1, VerificationRuleActions.VerifyAll)
                                ]).BindTo(NetworkingNode_Id.CSMS, Version.WebSocketSubProtocolId);

            var asTheAnswer   = ResetResponse.Parse(immediately, onTheWire, SourceRouting.CSMS, NetworkPath.Empty);
            var asAnother     = ResetResponse.Parse(onIdle,      onTheWire, SourceRouting.CSMS, NetworkPath.Empty);

            Assert.Multiple(() => {
                Assert.That(csmsVerifies.VerifyResponseMessage(asAnother,   asAnother.  ToJSON(true), out var refused,  onTheWire), Is.False, "A response verified as the answer to another request!");
                Assert.That(asAnother.Signatures.Single().Status, Is.EqualTo(VerificationStatus.WrongFrame));
                Assert.That(csmsVerifies.VerifyResponseMessage(asTheAnswer, asTheAnswer.ToJSON(true), out var accepted, onTheWire), Is.True, accepted);
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
