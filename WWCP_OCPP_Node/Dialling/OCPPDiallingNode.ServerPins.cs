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
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Configuration;

using cloud.charging.open.protocols.OCPP.Node.Connections;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The server of a wss:// connection that is checked by its CA: judged as
    /// this station judges every server it connects to - a time server, a name
    /// server, a meter - and held to what its connection says, under the same
    /// names.
    /// </summary>
    /// <remarks>
    /// Is the certificate issued for the server's name? Does its chain end at a
    /// root this machine trusts - or at a TLS root of this station's own store
    /// kept for "ocpp", or at one of the group of roots the connection names?
    /// And is it a certificate, and a root, the connection holds it to, by
    /// fingerprint or by the pin of its key? See
    /// <see cref="WWCPNode.JudgeServer"/> and <see cref="ServerPins"/>.
    ///
    /// The other way a wss:// connection knows its server - by the key pin of
    /// its key and nothing else - is in OCPPDiallingNode.KeyPins.cs, and a
    /// connection goes one way or the other, never both.
    /// </remarks>
    public abstract partial class OCPPDiallingNode
    {

        #region Data

        /// <summary>
        /// What a TLS root of the certificate store is kept for where it may
        /// vouch for the servers of the OCPP connections, and for nothing else.
        /// </summary>
        public const String  OCPPCertificateUsage  = ConnectionEntry.PinsService;

        #endregion


        #region (private) JudgedAsAServer(ConnectionId, DialledURL)

        /// <summary>
        /// What the WebSocket client of a wss:// connection checked by its CA
        /// asks at every TLS handshake: the node's judgement of the certificate
        /// its server showed, with what the connection holds it to at that
        /// moment.
        /// </summary>
        /// <param name="ConnectionId">The connection.</param>
        /// <param name="DialledURL">Where it went when it was dialled: what is learned on first use is written down for this one, and not for wherever it was changed to since.</param>
        protected RemoteTLSServerCertificateValidationHandler<IWebSocketClient> JudgedAsAServer(String  ConnectionId,
                                                                                               URL     DialledURL)

            => (sender, certificate, chain, client, policyErrors) => {

                   var judgement = JudgeConnection(ConnectionId, DialledURL, certificate, chain, policyErrors);

                   return judgement.Accepted
                              ? TLSValidationResult.Success()
                              : TLSValidationResult.Failed(WhyRefused(judgement));

               };

        #endregion

        #region (internal) JudgeConnection(ConnectionId, DialledURL, Certificate, Chain, PolicyErrors)

        /// <summary>
        /// Whether to go ahead with the certificate the server of a wss://
        /// connection showed.
        /// </summary>
        public ServerJudgement JudgeConnection(String             ConnectionId,
                                                 URL                DialledURL,
                                                 X509Certificate2?  Certificate,
                                                 X509Chain?         Chain,
                                                 SslPolicyErrors    PolicyErrors)
        {

            // As it is written down now, so that a pin taken away or a root
            // learned since it was dialled counts from this handshake on. Not
            // where it goes elsewhere by now: that one is dialled anew, and
            // this client is on its way out.
            var now   = Connections.CopyOf(ConnectionId);
            var pins  = now is not null && now.URL == DialledURL && !now.IsKeyPinned
                            ? now.Pins
                            : null;

            return JudgeServer(ConnectionsCertificateUsage,
                               DialledURL.Host.ToString(),
                               pins,
                               Certificate,
                               Chain,
                               PolicyErrors,
                               Evidence:  false,
                               Learn:     (what, fingerprint) => Connections.TryLearnPin(ConnectionId, DialledURL, what, fingerprint));

        }

        #endregion

        #region (private static) WhyRefused(Judgement)

        /// <summary>
        /// Every step that refused it, and not only the last: "refused, as its
        /// configuration says" says nothing without what it was held to and
        /// what it showed.
        /// </summary>
        protected static String WhyRefused(ServerJudgement Judgement)
        {

            var refusals = Judgement.Steps.Where (step => step.Level == "error").
                                           Select(step => step.Text).
                                           ToArray();

            return refusals.Length > 0
                       ? String.Join(" ", refusals)
                       : $"The certificate was not accepted: {Judgement.Outcome}.";

        }

        #endregion

        #region (internal) TryTakeSignedServerCAKeyPins(Connection, Pins, Version, Signer, out Error)

        /// <summary>
        /// Take the key pins of a signed pin set as the roots the chain of the
        /// server of a wss:// connection may end at, once a signer this station
        /// believes was found to have signed them: what a variable beside
        /// ServerKeyPins hands over.
        /// </summary>
        /// <remarks>
        /// Only with a higher version than the one the connection holds. The
        /// pins are those of keys - the SHA-256 over the SubjectPublicKeyInfo
        /// of a CA - and replace every root the connection was held to. They
        /// narrow what the CA check believes: the chain still has to end at a
        /// root this station trusts, and a root of the operator's own comes in
        /// through the certificate store, not through a pin. The connection is
        /// dialled again, so that a server whose chain ends elsewhere is not
        /// kept.
        /// </remarks>
        /// <param name="Connection">The connection, by its identification.</param>
        /// <param name="Pins">The key pins of the pin set, separated by commas.</param>
        /// <param name="Version">The version of the pin set.</param>
        /// <param name="Signer">Who signed it, for the log.</param>
        /// <param name="Error">Why the pins were not taken.</param>
        public Boolean TryTakeSignedServerCAKeyPins(String                           Connection,
                                                      String                           Pins,
                                                      UInt64                           Version,
                                                      String                           Signer,
                                                      [NotNullWhen(false)] out String? Error)
        {

            if (!Connections.TryTakeSignedServerCAKeyPins(Connection, Pins, Version, out Error))
                return false;

            if (Connections.CopyOf(Connection) is ConnectionEntry entry)
            {

                Log.Notice($"'{entry.Description}': the signed pin set of version {Version}, signed by '{Signer}', holds its server to the root " +
                           $"{String.Join(" or ", entry.Pins?.Roots ?? [])}.", "ocpp", "connections", "pins", "security");

                KeyPinSecurityEvent(entry, "ReconfigurationOfSecurityParameters",
                                    $"Root pins of endpoint '{entry.Id}' ({entry.URL}) set by a signed pin set of version {Version}");

            }

            return true;

        }

        #endregion

    }

}
