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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.KeyPins
{

    /// <summary>
    /// Whether a signer a node believes signed a pin set (white paper "OCPP over SSH", section 10.4;
    /// "Key Pins and Raw Public Keys", section 5.7, way 1).
    /// </summary>
    public static class PinSetSignatures
    {

        #region SignedBy(Request, RequestJSON, Signers, Identity, SubProtocol, Over, WithoutTimeWindow, Now, out Signer, out Why, NodeName = "node")

        /// <summary>
        /// Whether one of the given signers signed the given request - one of a pin set, or one that
        /// changes how a connection believes its server.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Without a time window for a pin set: it carries a version of its own, which the store compares
        /// with the one it holds and keeps under its lock, so that an older one cannot come back - and a
        /// node whose clock was never set, the very one that learns a key on first use, would otherwise
        /// refuse every pin set, or take any, depending on how wrong its clock is. With the time window, by
        /// the node's clock, for what has no version to stop a replay.
        /// </para>
        /// <para>
        /// The signature names the node as its destination, by the given identity. The message id may be
        /// "*" with a nonce: a Local Controller sends a request on under a message id of its own.
        /// </para>
        /// </remarks>
        /// <param name="Request">The request: of OCPP 2.1 or of OCPP 1.6.</param>
        /// <param name="RequestJSON">The request as JSON, with its JSON-LD context, as the node serializes it.</param>
        /// <param name="Signers">The signers believed.</param>
        /// <param name="Identity">Who the request is for: the node, by the identity the signer signs for.</param>
        /// <param name="SubProtocol">The WebSocket subprotocol of the OCPP version.</param>
        /// <param name="Over">What the connection goes over, for a sentence: "SSH", "TLS".</param>
        /// <param name="WithoutTimeWindow">Whether the signature's time is left out: for a pin set only.</param>
        /// <param name="Now">The node's clock.</param>
        /// <param name="Signer">Who signed it, as the signer is described.</param>
        /// <param name="Why">Why it is not taken as signed.</param>
        /// <param name="NodeName">What the node is called in a sentence: "station", "local controller".</param>
        public static Boolean SignedBy(IRequest?                          Request,
                                       JObject?                           RequestJSON,
                                       IReadOnlyList<PinSetSigner>        Signers,
                                       NetworkingNode_Id                  Identity,
                                       String                             SubProtocol,
                                       String                             Over,
                                       Boolean                            WithoutTimeWindow,
                                       Func<DateTimeOffset>               Now,
                                       [NotNullWhen(true)]  out String?   Signer,
                                       [NotNullWhen(false)] out String?   Why,
                                       String                             NodeName   = "node")
        {

            Signer  = null;
            Why     = null;

            if (Request is null || RequestJSON is null)
            {
                Why = "The request is not known, so who signed it is not either.";
                return false;
            }

            if (Signers.Count == 0)
            {
                Why = $"This {NodeName} believes no signer of pin sets yet: one is written down on its Connections page, from commissioning.";
                return false;
            }

            if (!Request.Signatures.Any())
            {
                Why = $"A pin set for a connection over {Over} is taken only from a SetVariables request signed by a signer this " +
                      $"{NodeName} believes, and this one is not signed.";
                return false;
            }

            String? why = null;

            foreach (var signer in Signers)
            {

                var policy = new SignaturePolicy(VerificationRules: [ new VerificationRule(Request.Context,
                                                                                           0,
                                                                                           VerificationRuleActions.VerifyAny,
                                                                                           new KeyPair(signer.Bytes),
                                                                                           WithoutTimeWindow: WithoutTimeWindow) ]) {
                                 Now = Now
                             }.BindTo(Identity, SubProtocol);

                if (policy.VerifyRequestMessage(Request, RequestJSON, out var error))
                {
                    Signer = signer.Description;
                    return true;
                }

                // Why the signer's own signature was not taken: another key is
                // no reason worth saying.
                if (Request.Signatures.Any(signature => signature.Status != VerificationStatus.UnknownSigner))
                    why ??= error;

            }

            Why = $"None of the request's signatures is a valid one of a signer this {NodeName} believes" + (why is not null ? $": {why}" : ".");
            return false;

        }

        #endregion

    }

}
