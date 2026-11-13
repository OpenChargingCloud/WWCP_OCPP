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

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.SSH;
using org.GraphDefined.Vanaheimr.Hermod.SSH.Client;
using org.GraphDefined.Vanaheimr.Hermod.Messaging;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using OCPPCommon = cloud.charging.open.protocols.OCPP;
using OCPPv1_6   = cloud.charging.open.protocols.OCPPv1_6;
using OCPPv2_1   = cloud.charging.open.protocols.OCPPv2_1;

using cloud.charging.open.protocols.WWCP.SSH;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP.Node.Connections;

using cloud.charging.open.protocols.OCPP.Node.KeyPins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// Dialling the back ends this station was told about.
    /// </summary>
    /// <remarks>
    /// Everything that decides how a connection is made is configuration - the
    /// URL, the credentials, the certificate, whether to come back after a
    /// drop - and this file is the only place that turns it into an actual
    /// call. What is configured lives in <see cref="ConnectionStore"/> and can
    /// be changed from the web interface while the station runs; what has been
    /// dialled lives here.
    /// </remarks>
    public abstract partial class OCPPDiallingNode
    {

        #region Data

        /// <summary>
        /// What became of each connection, by its identification.
        /// </summary>
        protected readonly Dictionary<String, ConnectionState> dialled = [];

        /// <summary>
        /// How long a connection test stays connected before closing again.
        /// </summary>
        /// <remarks>
        /// Long enough for a back end that greets its stations to get a word
        /// in, short enough that somebody pressing the button is still looking
        /// at the screen when it finishes. It is a test of whether the
        /// connection can be made, not a conversation.
        /// </remarks>
        public static readonly TimeSpan TestHoldsFor = TimeSpan.FromSeconds(2);

        /// <summary>
        /// How long a connection test waits for the answer to what it asked in OCPP.
        /// </summary>
        /// <remarks>
        /// A back end answers a Heartbeat in milliseconds. One that has not
        /// within this is not slow: nothing there speaks OCPP on this
        /// connection, and that is what the test says.
        /// </remarks>
        public static TimeSpan TestAnswerWithin { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// What a connection test asks in OCPP once it is connected, where it
        /// is not told: a Heartbeat.
        /// </summary>
        public const String TestAsksHeartbeat = "heartbeat";

        /// <summary>
        /// A connection test that asks nothing in OCPP: everything up to the
        /// handshake, as it was before it asked anything.
        /// </summary>
        public const String TestAsksNothing   = "none";

        /// <summary>
        /// One DialAgain at a time - see there.
        /// </summary>
        protected readonly SemaphoreSlim dialAgainLock = new (1, 1);

        /// <summary>
        /// The SSH clients this station closed, or is closing: what they say
        /// from then on is not a loss, nor anything to come back from.
        /// </summary>
        /// <remarks>
        /// What a WebSocket client's reconnect policy taken away is for those:
        /// an SSH client keeps its policy, and is told to stop by being closed.
        /// Kept weakly: a client let go of is forgotten by its node, and here
        /// with it - a station that fails over for hours lets go of many.
        /// </remarks>
        protected readonly ConditionalWeakTable<WWCPSshClient, Object> lettingGo = new();

        /// <summary>
        /// How long connecting over SSH may take - the TCP connection, the
        /// version exchange and the key exchange - before an attempt fails.
        /// </summary>
        public static readonly TimeSpan SshConnectTimeout  = TimeSpan.FromSeconds(15);

        /// <summary>
        /// How often a connection over SSH asks its server whether it is still
        /// there, so that a server gone without a word is noticed - and come
        /// back to - rather than waited for.
        /// </summary>
        public static readonly TimeSpan SshKeepAliveEvery  = TimeSpan.FromSeconds(30);

        /// <summary>
        /// How often a connection over SSH sends a PING, as a WebSocket of this
        /// station does: after three left without an answer, or without a word
        /// at all from the server, the connection is taken for dead and dialled
        /// again (white paper "OCPP over SSH", decision 11).
        /// </summary>
        public static readonly TimeSpan SshPingEvery       = TimeSpan.FromSeconds(30);

        // The connections whose server is believed by a key that was learned on
        // first use and no signed pin set confirmed yet - a host key over SSH, a
        // key pin over a wss:// WebSocket - with what they were dialled for, those
        // whose server is not believed any more, and the clock of each probation:
        // what the gate of the OCPP nodes asks, one registry for both kinds
        // (WWCP_OCPP_Node's Probations).
        protected readonly Probations  probations  = new ();

        /// <summary>
        /// The connections on probation and those not believed any more, for a
        /// test that asks what the gate would say at a given moment.
        /// </summary>
        public Probations Probations
            => probations;

        #endregion

        #region Properties

        /// <summary>
        /// What happened to each configured connection the last time this
        /// station tried it, by identification.
        /// </summary>
        /// <remarks>
        /// One sentence each, the same one that went into the log. Kept so
        /// that somebody arriving at the station later can be told without
        /// having to find the moment it started in the log.
        /// </remarks>
        public IReadOnlyDictionary<String, String> DialledConnections
        {
            get
            {
                lock (dialled)
                    return dialled.ToDictionary(entry => entry.Key, entry => entry.Value.Said);
            }
        }

        /// <summary>
        /// Where each connection this station dialled stands, by
        /// identification.
        /// </summary>
        /// <remarks>
        /// The sentence of DialledConnections, and around it what the
        /// Connections page says beside each connection: whether it is
        /// connected, since when, what was dialled - a connection changed
        /// is dialled as it was until the station has taken the change - and,
        /// while it is tried again, when the next attempt is.
        /// </remarks>
        public IReadOnlyDictionary<String, ConnectionState> ConnectionStates
        {
            get
            {
                lock (dialled)
                    return new Dictionary<String, ConnectionState>(dialled);
            }
        }

        /// <summary>
        /// How many WebSocket clients the two OCPP nodes are holding.
        /// </summary>
        /// <remarks>
        /// One connection made by this station is one client here. It is the
        /// only way from outside to see that a connection test left nothing
        /// behind, which is the thing about a test that would otherwise go
        /// unnoticed until a back end complained about duplicate stations.
        /// </remarks>
        public Int32 OCPPWebSocketClientCount

            => Nodes.Sum(node => node.OCPPWebSocketClients.Count());

        #endregion


        #region (private) DialConfiguredConnections(CancellationToken = default)

        /// <summary>
        /// Dial everything this station was told to dial.
        /// </summary>
        /// <remarks>
        /// After the servers are listening, and deliberately so: somebody
        /// watching the Logs page sees each connection come up or fail, rather
        /// than having to reload afterwards to find out how it went. It is the
        /// same reason the V2G link starts where it does.
        ///
        /// Nothing here throws. A station whose back end is unreachable is a
        /// station that still has to open its door, run its display and answer
        /// its web interface - not least because the web interface is where
        /// somebody fixes the address that was wrong.
        ///
        /// Only the ones set to connect by themselves. The rest stay written
        /// down and are not touched - an address kept ready for the day
        /// somebody needs it, which can still be tried by hand from the page.
        ///
        /// Peer by peer, each on its own, in the order they were written down,
        /// and none of them affected by how another went: one endpoint of each,
        /// and the next one where that cannot be reached (see
        /// OCPPDiallingNode.Peers.cs). The oldest of each OCPP version is the
        /// primary, the back end in charge, which is told what the station says
        /// by itself; every other one is registered with and answered, and told
        /// nothing by itself (see IsPrimary).
        /// </remarks>
        protected Task DialConfiguredConnections(CancellationToken CancellationToken = default)

            => DialPeers(CancellationToken);

        #endregion

        #region (private) WhatItProvesItselfWith(Connection, out Basic, out TOTP, out TOTPLogin, out Certificates, out Says, out Error)

        /// <summary>
        /// The credentials or the certificate a connection names, looked up
        /// and turned into what an HTTP client takes.
        /// </summary>
        /// <remarks>
        /// One place, because connecting and testing a connection have to
        /// agree about what it would use - a test that proves itself
        /// differently from the real thing is a test of something else.
        ///
        /// <paramref name="Says"/> is for whoever is reading: a sentence
        /// naming what will be shown, without any part of the secret in it.
        ///
        /// A one-time password goes under its login, in "Authorization:
        /// TOTP", which is what an OCPP server verifies. One bound to the TLS
        /// session is bound on a connection that names HTTP/3 and nothing
        /// else, where the TLS of QUIC exports the keying material to bind it
        /// with. On any other connection it is not sent at all: the station's
        /// TLS exports none there, and sending it unbound would be less than
        /// what was configured.
        /// </remarks>
        /// <summary>
        /// Whether a one-time password can be bound to the TLS session of
        /// this connection: one that is opened over HTTP/3 and nothing else.
        /// </summary>
        /// <remarks>
        /// Known before it is made, which is what is needed: a connection left
        /// the choice of the version may come to an SslStream, which exports
        /// nothing to bind a password with.
        /// </remarks>
        public static Boolean BindsItsPassword(ConnectionEntry Connection)

            => Connection.BindsItsPassword;

        protected Boolean WhatItProvesItselfWith(ConnectionEntry             Connection,
                                               out IHTTPAuthentication?    Basic,
                                               out TOTPConfig?             TOTP,
                                               out String?                 TOTPLogin,
                                               out X509Certificate2[]?     Certificates,
                                               out String                  Says,
                                               out String?                 Error)
        {

            Basic         = null;
            TOTP          = null;
            TOTPLogin     = null;
            Certificates  = null;
            Says          = "nothing - it identifies itself to whoever is at the other end not at all";
            Error         = null;

            if (Connection.AuthenticationId is not null)
            {

                var credentials = Connections.Authentications.
                                      FirstOrDefault(entry => entry.Id == Connection.AuthenticationId);

                if (credentials is null)
                {
                    Error = "it names credentials that are not configured here";
                    return false;
                }

                Basic  = credentials.ToBasicAuthentication();
                TOTP   = credentials.ToTOTPConfig();

                if (Basic is null && TOTP is null)
                {
                    Error = $"it uses '{credentials.Description}', which has no secret set";
                    return false;
                }

                if (TOTP is not null && credentials.TLSChannelBinding && !BindsItsPassword(Connection))
                {
                    Error = $"it uses '{credentials.Description}', a one-time password bound to the TLS session, and one is bound only on " +
                             $"a connection that names HTTP/3 and nothing else: over HTTP/1.1 and HTTP/2 this {NodeWording.Noun}'s TLS exports no " +
                             "keying material to bind one with. Name HTTP/3 for it, or switch the binding off to have it sent unbound";
                    TOTP  = null;
                    return false;
                }

                if (TOTP is not null)
                    TOTPLogin = credentials.Login;

                Says = TOTP is not null
                           ? $"HTTP TOTP as '{credentials.Login}' ('{credentials.Description}'), " +
                             (credentials.TLSChannelBinding ? "bound to the TLS session of the connection" : "unbound")
                           : $"HTTP Basic as '{credentials.Login}' ('{credentials.Description}')";

            }

            if (Connection.CertificateId is not null)
            {

                var key = ClientCertificates.Entries.
                              FirstOrDefault(entry => entry.Id == Connection.CertificateId);

                if (key?.Certificate is null)
                {
                    Error = $"it names a client certificate this {NodeWording.Noun} does not have";
                    return false;
                }

                // A certificate whose private key this runtime could not
                // attach cannot be shown in a handshake. Said here rather than
                // left to a TLS error that names neither the certificate nor
                // the reason.
                if (!key.CanBeHeldUp)
                {
                    Error = $"it would show a certificate this runtime cannot present ({key.CannotBeHeldUp})";
                    return false;
                }

                Certificates  = [ key.Certificate ];
                Says          = $"a TLS client certificate, '{key.Subject}' ({key.Algorithm})";

            }

            return true;

        }

        #endregion

        #region (private static) SubProtocolsOf(Connection)

        /// <summary>
        /// What this station offers for the given connection, in the WebSocket
        /// handshake and over ssh:// alike.
        /// </summary>
        /// <remarks>
        /// 2.1 offers 2.0.1 as well, which is what the OCPP node does and what
        /// most back ends of that generation actually answer with. A
        /// connection that says "cbor" offers "ocpp2.1cbor" and nothing else;
        /// one that says "auto" offers it first, and the others after it.
        /// OCPP 1.6 the same, with "ocpp1.6cbor".
        /// </remarks>
        protected static String[] SubProtocolsOf(ConnectionEntry Connection)

            => Connection.OCPPVersion == OCPPVersion.OCPP1_6
                   ? [.. OCPPCommon.WebSockets.OCPPSubprotocols.Offer(
                             [ OCPPv1_6.Version.WebSocketSubProtocolId ],
                             Connection.Encoding
                         )]
                   : [.. OCPPCommon.WebSockets.OCPPSubprotocols.Offer(
                             [ OCPPv2_1.Version.WebSocketSubProtocolId, "ocpp2.0.1" ],
                             Connection.Encoding
                         )];

        #endregion

        #region (private static) CBORFrameOf(Text)

        /// <summary>
        /// The request of a connection test as the frame of a connection that
        /// agreed on CBOR: the same array, with nothing in its payload.
        /// </summary>
        protected static Byte[] CBORFrameOf(String Text)

            => OCPPCommon.WebSockets.OCPP_CBORFrame.Join(
                   JArray.Parse(Text),
                   org.GraphDefined.Vanaheimr.Illias.CBORValue.FromMap(
                       Array.Empty<KeyValuePair<org.GraphDefined.Vanaheimr.Illias.CBORValue, org.GraphDefined.Vanaheimr.Illias.CBORValue>>()
                   )
               ).ToByteArray();

        #endregion

        #region (private) Dial(Connection, CancellationToken)

        /// <summary>
        /// One connection: what it proves itself with, and whether it got
        /// through.
        /// </summary>
        protected async Task Dial(ConnectionEntry    Connection,
                                CancellationToken  CancellationToken)
        {

            var where = $"'{Connection.Description}' ({Connection.URL})";

            try
            {

                if (Connection.IsSSH)
                {
                    await DialOverSSH(Connection, where, CancellationToken);
                    return;
                }

                if (!WhatItProvesItselfWith(Connection, out var basic, out var totp, out var totpLogin, out var certificates, out _, out var wrong))
                {
                    Fail(Connection, ConnectionStatus.NotDialled, $"Not dialled: {where} {wrong}.");
                    return;
                }

                // Where it went when it was dialled: a first key pin is written
                // down for this one, and not for wherever it was changed to since.
                var dialledURL  = Connection.URL;

                // Its server known by the key pin of its key rather than by a
                // CA: the key and nothing else is checked, at every handshake,
                // against the key pins the connection has at that moment.
                var keyPins     = KeyPinTrustFor(Connection, out var notByKeyPin);

                if (notByKeyPin is not null)
                {
                    Fail(Connection, ConnectionStatus.NotDialled, $"Not dialled: {where} {notByKeyPin}.");
                    return;
                }

                // Checked by its CA otherwise, and judged as this station judges
                // every server it connects to - a time server, a name server, a
                // meter: its name, its chain, and what the connection holds it
                // to at that moment.
                var validator   = keyPins is not null
                                      ? keyPins.Validator
                                      : Connection.IsSecure
                                            ? JudgedAsAServer(Connection.Id, dialledURL)
                                            : null;

                Log.Info($"Dialling {where} as {ConnectionEntry.AsText(Connection.OCPPVersion)} " +
                         $"{Connection.ConnectionType}" +
                         (keyPins is not null ? $"; {WhatItBelieves(keyPins)}" : "") +
                         (keyPins is null && Connection.Pins?.Text is String heldTo ? $"; its server is held to {heldTo}" : "") +
                         ".", "ocpp", "connections");

                // Under a name of its own, and the primary under the name of the
                // CSMS as well - see NextHopOf and CSMSNamesOf.
                var response = await ConnectOverWebSocket(Connection, basic, validator, certificates, totp, totpLogin, CancellationToken);

                // 101 and nothing else: an HTTP answer that is not an upgrade
                // is a web server being polite, not a back end.
                //
                // Nor is every answer one the far end gave. An attempt that
                // ended before it could ask anything - nothing listening, a
                // name that does not resolve, a TLS handshake that failed - is
                // answered by the client itself, with a 400 that looks like a
                // real one. What gives it away is that it answers no request:
                // there was none. So a station whose back end is not there says
                // it could not be reached, rather than sending somebody looking
                // for a server that answered 400 and never existed.
                var client     = ClientOf(Connection);
                var unreached  = response.HTTPRequest is null;

                RememberClient(Connection, client);

                // Which version of HTTP it came to: where the server chose, this
                // is the only place that says.
                if (response.HTTPStatusCode == HTTPStatusCode.SwitchingProtocols && client?.HTTPVersionInUse is HTTPVersion over)
                    Log.Info($"The WebSocket to {where} was opened over HTTP/{ConnectionEntry.AsText(over)}" +
                             (Connection.HTTPVersion == WebSocketHTTPVersion.Auto ? ", which its server chose" : "") +
                             ".", "ocpp", "connections");

                // How its messages are written: where the back end chose, this
                // is the only place that says.
                if (response.HTTPStatusCode == HTTPStatusCode.SwitchingProtocols &&
                    Connection.Encoding     != OCPPCommon.WebSockets.MessageEncodings.JSON &&
                    SubprotocolInUseAt(Connection.URL) is String agreed)
                {
                    Log.Info($"The connection to {where} agreed on {agreed}: its messages are " +
                             (OCPPCommon.WebSockets.OCPPSubprotocols.IsCBOR(agreed) ? "CBOR" : "JSON") +
                             (Connection.Encoding == OCPPCommon.WebSockets.MessageEncodings.Auto ? ", which its back end chose" : "") +
                             ".", "ocpp", "connections");
                }

                if (keyPins is not null && client is not null)
                    FollowKeyPins(Connection, client, keyPins, dialledURL, response.HTTPStatusCode == HTTPStatusCode.SwitchingProtocols);

                if (response.HTTPStatusCode != HTTPStatusCode.SwitchingProtocols)
                {

                    // Whether it goes on, asked of the client rather than read
                    // from the answer: a first attempt that found nothing, or a
                    // back end not ready yet, is tried again by itself; an
                    // answer that means no - a wrong address, a wrong password
                    // - is an answer, and is not.
                    var keepsTrying = client?.KeepsTrying == true;

                    Fail(Connection,
                         keepsTrying ? ConnectionStatus.Trying
                                     : unreached ? ConnectionStatus.Failed
                                                 : ConnectionStatus.Refused,
                         (keyPins?.Refused == true
                              ? $"{where} was not believed: {keyPins.WhyRefused()}"
                              : unreached
                              ? $"{where} could not be reached: the attempt ended before anything could be asked of it"
                              : $"{where} did not become a WebSocket connection: {response.HTTPStatusCode}") +
                         (keepsTrying
                              ? "; it is tried again by itself, and said here when it gets through."
                              : unreached
                                    ? "; it is not tried again."
                                    : "; that answer is final, and it is not tried again."));

                    if (client is not null && keepsTrying)
                        Follow(Connection, client, Connected: false);

                    return;

                }

                // A connection that says "cbor" speaks nothing else. A back end
                // that took the handshake without agreeing on the CBOR
                // subprotocol is let go of before anything is said to it.
                if (Connection.Encoding == OCPPCommon.WebSockets.MessageEncodings.CBOR &&
                    !OCPPCommon.WebSockets.OCPPSubprotocols.IsCBOR(SubprotocolInUseAt(Connection.URL)))
                {

                    await Disconnect(client, "No CBOR subprotocol was agreed on");

                    Fail(Connection,
                         ConnectionStatus.Refused,
                         $"{where} took the connection without agreeing on {String.Join(", ", SubProtocolsOf(Connection))}: " +
                         "it does not speak OCPP as CBOR, and a connection that says \"cbor\" speaks nothing else; it is not tried again.");

                    return;

                }

                // Which signatures the messages for this connection get, said before
                // the first of them is signed.
                if (client is IWWCPClient itsClient)
                {
                    if (Connection.OCPPVersion == OCPPVersion.OCPP1_6)
                        Node16!.OCPP.SetSignatureFormats(itsClient, Connection.SignatureFormats);
                    else
                        Node21.OCPP.SetSignatureFormats(itsClient, Connection.SignatureFormats);
                }

                if (client is not null)
                    Follow(Connection, client, Connected: true);

                else
                    Log.Warning($"'{Connection.Description}' connected, but the client that did it could not be found again, " +
                                 "so nothing could be said about what becomes of it.", "ocpp", "connections");

                Note(Connection, "Connected, and will come back by itself if it drops.");

                ConnectionIsUp(Connection);

            }
            catch (Exception e)
            {
                // Anything at all: a name that does not resolve, a refused
                // socket, a certificate the other end will not accept. One
                // connection that cannot be made must not take the station
                // down with it.
                Fail(Connection, ConnectionStatus.Failed, $"{where} could not be reached: {e.Message}");
            }

        }

        #endregion

        #region (internal static) NextHopOf(Connection)

        /// <summary>
        /// The name the far end of a connection has in its OCPP node's routing
        /// table: one of its own.
        /// </summary>
        /// <remarks>
        /// Every connection used to be "CentralSystem" in 1.6 and "CSMS" in 2.1,
        /// and a node has one route by a name: to whichever connection came up
        /// last. With two back ends of one version - a CSMS and its successor,
        /// during a migration - what the station said by itself went to the one
        /// that had connected or reconnected last, and a BootNotification asked
        /// for the second one went to that one too. A request is answered over
        /// the connection it came over, whatever the name.
        /// </remarks>
        public static NetworkingNode_Id NextHopOf(ConnectionEntry Connection)
            => NetworkingNode_Id.Parse($"connection-{Connection.Id}");

        #endregion

        #region (internal) IsPrimary(Connection)

        /// <summary>
        /// Whether the connection leads to the back end in charge of its OCPP
        /// node: the oldest connection of its version that dials by itself, or
        /// another endpoint of the peer it belongs to - the one dialled where
        /// that cannot be reached.
        /// </summary>
        /// <remarks>
        /// The oldest, because that is the one this station has always dialled
        /// first. Every other connection of the version leads to a back end the
        /// station registers with as well and answers, and tells nothing by
        /// itself. Which one is in charge is to be said by a switchover later;
        /// until then it is this one.
        /// </remarks>
        public Boolean IsPrimary(ConnectionEntry Connection)
        {

            var oldest = Connections.Connections.
                             Where  (one => one.AutoConnect && one.OCPPVersion == Connection.OCPPVersion).
                             OrderBy(one => one.CreatedAt).
                             FirstOrDefault();

            if (oldest is null)
                return false;

            if (oldest.Id == Connection.Id)
                return true;

            return Connections.PeerOf(oldest.Id) is PeerEntry peer &&
                   peer.EndpointIds.Contains(Connection.Id);

        }

        #endregion

        #region (private) CSMSNamesOf(Connection)

        /// <summary>
        /// The names a connection is reached by beside its own: the CSMS's - by
        /// which a node sends what it has to say, a security event, an event of
        /// a monitor, a heartbeat - for the primary, and none for any other.
        /// </summary>
        protected NetworkingNode_Id[] CSMSNamesOf(ConnectionEntry Connection)

            => IsPrimary(Connection)
                   ? [ Connection.OCPPVersion == OCPPVersion.OCPP1_6
                           ? NetworkingNode_Id.CentralSystem
                           : NetworkingNode_Id.CSMS ]
                   : [];

        #endregion

        #region (private) ClientOf(Connection)

        /// <summary>
        /// The client the node made for this connection when it was dialled.
        /// </summary>
        /// <remarks>
        /// Made inside the call and kept by the node, whether it got through or
        /// not; the last one to the connection's address is the one this dial
        /// made.
        /// </remarks>
        protected WebSocketClient? ClientOf(ConnectionEntry Connection)

            => (NodeOrNoneOf(Connection.OCPPVersion)?.OCPPWebSocketClients ?? []).
               OfType<WebSocketClient>().
               LastOrDefault(one => one.RemoteURL == Connection.URL);

        #endregion

        #region (internal) HTTPVersionInUseAt(URL)

        /// <summary>
        /// The version of HTTP the WebSocket this station has to the given
        /// address was opened over - "1.1", "2" or "3" - or null, where it has none.
        /// </summary>
        public String? HTTPVersionInUseAt(URL URL)

            => Nodes.SelectMany(node => node.OCPPWebSocketClients).
                    OfType<WebSocketClient>().
                    LastOrDefault(one => one.RemoteURL == URL) is { Connected: true, HTTPVersionInUse: HTTPVersion over }

                   ? ConnectionEntry.AsText(over)
                   : null;

        #endregion

        #region (internal) SignatureFormatsOf(Connection)

        /// <summary>
        /// Which signatures the OCPP node of this station writes for the given
        /// connection now - or null, where its signature policy decides.
        /// </summary>
        public cloud.charging.open.protocols.WWCP.SignatureFormats? SignatureFormatsOf(ConnectionEntry Connection)

            => Connection.OCPPVersion == OCPPVersion.OCPP1_6
                   ? Node16!.OCPP.SignatureFormatsFor(NextHopOf(Connection))
                   : Node21.OCPP.SignatureFormatsFor(NextHopOf(Connection));

        #endregion

        #region (internal) SubprotocolInUseOf(Connection)

        /// <summary>
        /// The subprotocol the given connection agreed on - "ocpp2.1",
        /// "ocpp2.1cbor" - over a WebSocket or over ssh://, or null where it
        /// is not connected.
        /// </summary>
        /// <remarks>
        /// Asked of the client that was made for the connection when it was
        /// dialled. A WebSocket says it in its handshake, an ssh:// connection
        /// in the WELCOME its server answers the HELLO with.
        /// </remarks>
        public String? SubprotocolInUseOf(ConnectionEntry Connection)

            => LiveOf(Connection)?.Client switch {

                   WWCPWebSocketClient { Connected: true } webSocketClient
                       => webSocketClient.WebSocketClientConnection?.Subprotocol,

                   WWCPSshClient sshClient
                       => sshClient.MessageClient.Connection is SshMessageConnection { IsAlive: true } connection
                              ? connection.Subprotocol
                              : null,

                   // One that was dialled without a peer of its own.
                   _   => Connection.IsSSH
                              ? null
                              : SubprotocolInUseAt(Connection.URL)

               };

        #endregion

        #region (internal) SubprotocolInUseAt(URL)

        /// <summary>
        /// The subprotocol the WebSocket this station has to the given address
        /// agreed on - "ocpp2.1", "ocpp2.1cbor" - or null, where it has none.
        /// </summary>
        public String? SubprotocolInUseAt(URL URL)

            => Nodes.SelectMany(node => node.OCPPWebSocketClients).
                    OfType<WWCPWebSocketClient>().
                    LastOrDefault(one => one.RemoteURL == URL) is { Connected: true } client

                   ? client.WebSocketClientConnection?.Subprotocol
                   : null;

        #endregion

        #region (private) NodeOf(Version)

        /// <summary>
        /// The OCPP node that speaks the given version.
        /// </summary>
        protected OCPPCommon.NetworkingNode.AOCPPNetworkingNode NodeOf(OCPPVersion Version)

            => NodeOrNoneOf(Version)
                   ?? throw new InvalidOperationException($"This {NodeWording.Noun} does not speak {ConnectionEntry.AsText(Version)}.");

        /// <summary>
        /// The OCPP node that speaks the given version, or none where this
        /// node does not speak it.
        /// </summary>
        protected OCPPCommon.NetworkingNode.AOCPPNetworkingNode? NodeOrNoneOf(OCPPVersion Version)

            => Version == OCPPVersion.OCPP1_6
                   ? Node16
                   : Node21;

        #endregion

        #region (private) SshLoginOf(Connection, Key, out Login, out Trust, out Says, out Error)

        /// <summary>
        /// Who an ssh:// connection signs in as, with what, and which host key
        /// it believes.
        /// </summary>
        /// <remarks>
        /// One place, for the same reason as WhatItProvesItselfWith: dialling
        /// and testing a connection have to agree about what it shows.
        ///
        /// Always this station's own key. A password on top where the
        /// connection names credentials - HTTP Basic ones, whose login and
        /// password are what an SSH server asks for - and only sent where the
        /// server asks for one, after it proved itself with its host key. Who
        /// signs in is the URL's login, or the credentials', or else the name
        /// of the OCPP node that dials.
        /// </remarks>
        /// <param name="ForTest">Whether it is for a connection test: one that pins nothing believes the first host key shown, and writes nothing down.</param>
        protected Boolean SshLoginOf(ConnectionEntry                            Connection,
                                   ISshHostKey                                Key,
                                   [NotNullWhen(true)]  out SshClientOptions? Login,
                                   [NotNullWhen(true)]  out SshHostKeyTrust?  Trust,
                                   out String                                 Says,
                                   [NotNullWhen(false)] out String?           Error,
                                   Boolean                                    ForTest   = false)
        {

            Login  = null;
            Trust  = null;
            Says   = "";
            Error  = null;

            if (Connection.CertificateId is not null)
            {
                Error = "it names a TLS client certificate, and an ssh:// connection makes no TLS handshake to show one in";
                return false;
            }

            if (Connection.PinsMD5)
            {
                Error = "it pins an MD5 fingerprint, and MD5 fingerprints are no longer taken: replace it by the SHA-256 one, which the connection test shows";
                return false;
            }

            if (!ForTest                                                   &&
                Connection.SshHostKey         is null                      &&
                Connection.SshLearnedHostKey  is null                      &&
                Connection.SshTrustOnFirstUse == SshTrustOnFirstUse.None)
            {
                Error = "it pins no host key and does not trust on first use, so nothing says which server to believe: pin its host key - " +
                        "the connection test shows it - or let it learn the first one, on probation";
                return false;
            }

            String? password       = null;
            String? passwordLogin  = null;
            String? passwordOf     = null;

            if (Connection.AuthenticationId is not null)
            {

                var credentials = Connections.Authentications.
                                      FirstOrDefault(entry => entry.Id == Connection.AuthenticationId);

                if (credentials is null)
                {
                    Error = "it names credentials that are not configured here";
                    return false;
                }

                if (credentials.Kind == AuthenticationKind.TOTP)
                {
                    Error = $"it uses '{credentials.Description}', a TOTP password, which is answered over HTTP and not over SSH";
                    return false;
                }

                if (String.IsNullOrEmpty(credentials.Password))
                {
                    Error = $"it uses '{credentials.Description}', which has no secret set";
                    return false;
                }

                password       = credentials.Password;
                passwordLogin  = credentials.Login;
                passwordOf     = credentials.Description;

            }

            var urlLogin = Connection.URL.Login;

            if (urlLogin is not null && passwordLogin is not null && urlLogin != passwordLogin)
            {
                Error = $"the URL signs in as '{urlLogin}' and '{passwordOf}' as '{passwordLogin}', and it is one login";
                return false;
            }

            var username  = urlLogin ?? passwordLogin ?? NodeOf(Connection.OCPPVersion).Id.ToString();

            Trust  = new SshHostKeyTrust(
                         Connection.SshHostKeys,
                         Connection.SshLearnedHostKey,
                         ForTest ? SshTrustOnFirstUse.HostKey : Connection.SshTrustOnFirstUse,
                         Connection.SshOnPinMismatch,
                         PasswordMayBeSent: password is not null,
                         Connection.URL.Host.ToString(),
                         Connection.URL.Port ?? IPPort.Parse(22)
                     );

            // The station's DNS client resolves the server's name, as it
            // resolves a WebSocket's: never the operating system's resolver.
            Login  = new SshClientOptions {
                         Username           = username,
                         DNSClient          = DNSClient,
                         VerifyHostKey      = Trust.Verify,
                         Credentials        = [ Key ],
                         Password           = password,
                         ConnectTimeout     = SshConnectTimeout,
                         KeepAliveInterval  = SshKeepAliveEvery,
                         // Terrapin (CVE-2023-48795): a server without strict KEX
                         // is not signed in to (white paper "OCPP over SSH", decision 10).
                         RequireStrictKex   = true
                     };

            Says   = $"this {NodeWording.Noun}'s SSH key ({SshFingerprint.Sha256(Key.PublicKeyBlob)}) as '{username}'" +
                     (passwordOf is not null
                          ? $", and the password of '{passwordOf}' where the server asks for one"
                          : "");

            return true;

        }

        #endregion

        #region (private) DialOverSSH(Connection, Where, CancellationToken)

        /// <summary>
        /// One ssh:// connection: who it signs in as, and whether it got
        /// through.
        /// </summary>
        /// <remarks>
        /// What is final and what is tried again, as over a WebSocket: a server
        /// not there yet is tried again by itself, and one that answered no is
        /// not - a host key that is not the one believed, a login it did not
        /// take, the OCPP subsystem refused. Trying again would not change the
        /// answer, only fill the log with it.
        /// </remarks>
        protected async Task DialOverSSH(ConnectionEntry    Connection,
                                       String             Where,
                                       CancellationToken  CancellationToken)
        {

            var node  = NodeOf(Connection.OCPPVersion);
            var key   = await SshKey.MakeIfNeeded(CancellationToken);

            if (!SshLoginOf(Connection, key, out var login, out var trust, out var says, out var wrong))
            {
                Fail(Connection, ConnectionStatus.NotDialled, $"Not dialled: {Where} {wrong}.");
                return;
            }

            // Where it went when it was dialled: a first host key is written
            // down for this one, and not for wherever it was changed to since.
            var dialledURL = Connection.URL;

            Log.Info($"Dialling {Where} as {ConnectionEntry.AsText(Connection.OCPPVersion)} {Connection.ConnectionType} over SSH, " +
                     $"signing in with {says}; {WhatItBelieves(trust)}.",
                     "ocpp", "connections");

            sshTrusts[Connection.Id] = trust;

            trust.OnMismatchTolerated += why => {
                Log.Warning($"{Where}: {why}.", "ocpp", "connections", "ssh", "security");
                if (Connection.SshOnPinMismatch == SshOnPinMismatch.Record)
                    SshSecurityEvent(Connection, "InvalidCsmsHostKey", $"Host key {trust.Shown} of '{Connection.Id}' matches no pin; went ahead (Record)");
            };

            // The host becomes an SSH peer of this station, and OCPP one more
            // channel of the connection the station keeps to it - which whatever
            // else shares it may have had made before (OCPPDiallingNode.SSHPeers.cs).
            var peer    = SshPeerFor(Connection, login, trust);

            // Named as over a WebSocket - see Dial.
            var result  = await node.ConnectOCPPClient(
                                    RemoteURL:                 Connection.URL,
                                    NextHopNetworkingNodeId:   NextHopOf(Connection),
                                    RoutingNetworkingNodeIds:  CSMSNamesOf(Connection),
                                    Ssh:                       login,
                                    SshConnection:             peer is not null ? peer.Connection.GetAsync : null,
                                    Subprotocols:              SubProtocolsOf(Connection),
                                    DNSClient:                 DNSClient,
                                    PingEvery:                 SshPingEvery,
                                    CancellationToken:         CancellationToken
                                );

            var client  = result.Client as WWCPSshClient;

            // The peer lasts as long as the station holds on to the client.
            if (peer is not null)
            {
                if (client is not null)
                    sshPeerOf[client] = peer;
                else
                    await ReleaseSshPeer(peer);
            }

            RememberClient(Connection, client);

            if (!result.IsConnected)
            {

                var final        = trust.Refused || IsFinal(result.Error);
                var keepsTrying  = !final && client is not null && node.ReconnectPolicy is not null;

                if (final && client is not null)
                    await LetGo(client, "Not tried again.");

                ReportFinal(Connection, trust, result.Error);

                Fail(Connection,
                     keepsTrying ? ConnectionStatus.Trying
                                 : final ? ConnectionStatus.Refused
                                         : ConnectionStatus.Failed,
                     WhyNotOverSSH(Where, trust, result.Error) +
                     (keepsTrying
                          ? "; it is tried again by itself, and said here when it gets through."
                          : final
                                ? "; that answer is final, and it is not tried again."
                                : "; it is not tried again."));

                if (keepsTrying)
                    FollowOverSSH(Connection, dialledURL, client!, trust, Connected: false);

                return;

            }

            // As over a WebSocket: a connection that says "cbor" speaks nothing else.
            if (Connection.Encoding == OCPPCommon.WebSockets.MessageEncodings.CBOR &&
                client is not null                                                 &&
                !OCPPCommon.WebSockets.OCPPSubprotocols.IsCBOR(client.MessageClient.Connection?.Subprotocol))
            {

                await LetGo(client, "No CBOR subprotocol was agreed on.");

                Fail(Connection,
                     ConnectionStatus.Refused,
                     $"{Where} took the connection without agreeing on {String.Join(", ", SubProtocolsOf(Connection))}: " +
                     "it does not speak OCPP as CBOR, and a connection that says \"cbor\" speaks nothing else; it is not tried again.");

                return;

            }

            // How its messages are written: where the server chose, this is the only place that says.
            if (Connection.Encoding != OCPPCommon.WebSockets.MessageEncodings.JSON &&
                client?.MessageClient.Connection?.Subprotocol is String agreed)
            {
                Log.Info($"The connection to {Where} agreed on {agreed}: its messages are " +
                         (OCPPCommon.WebSockets.OCPPSubprotocols.IsCBOR(agreed) ? "CBOR" : "JSON") +
                         (Connection.Encoding == OCPPCommon.WebSockets.MessageEncodings.Auto ? ", which its server chose" : "") +
                         ".", "ocpp", "connections");
            }

            // As over a WebSocket: which signatures the messages for this connection get.
            if (client is not null)
            {
                if (Connection.OCPPVersion == OCPPVersion.OCPP1_6)
                    Node16!.OCPP.SetSignatureFormats(client, Connection.SignatureFormats);
                else
                    Node21.OCPP.SetSignatureFormats(client, Connection.SignatureFormats);
            }

            if (client is not null)
            {
                LearnIfFirst(Connection, dialledURL, trust, client, client.MessageClient.Connection);
                FollowOverSSH(Connection, dialledURL, client, trust, Connected: true);
            }

            else
                Log.Warning($"'{Connection.Description}' connected, but the client that did it could not be found again, " +
                             "so nothing could be said about what becomes of it.", "ocpp", "connections");

            Note(Connection, "Connected, and will come back by itself if it drops.");

            ConnectionIsUp(Connection);

        }

        #endregion

        #region (private static) IsFinal(Error) / WhyNotOverSSH(Where, Trust, Error) / ReasonOf(Error)

        /// <summary>
        /// Whether an answer of the server is one that trying again would not
        /// change: a login it did not take, the OCPP subsystem refused, or a
        /// server without strict KEX. A refusal that says when to come back -
        /// "connection-exists", with its retryAfter - is not final.
        /// </summary>
        protected static Boolean IsFinal(Exception? Error)

            => FinalOrNot(Error) switch {
                   SshMessageRefusedException refused  => refused.RetryAfter is null,
                   SshSubsystemRefusedException        => true,
                   SshAuthenticationException          => true,
                   SshStrictKexRequiredException       => true,
                   _                                   => false
               };

        /// <summary>
        /// The error that says why, where it is one of those whose answer
        /// decides: the given one, or what it wraps.
        /// </summary>
        protected static Exception? FinalOrNot(Exception? Error)

            => Error is SshAuthenticationException or SshMessageRefusedException or SshSubsystemRefusedException or SshStrictKexRequiredException
                   ? Error
                   : Error?.GetBaseException();

        /// <summary>
        /// What a connection over SSH believes its server by, as the end of a
        /// sentence.
        /// </summary>
        protected static String WhatItBelieves(SshHostKeyTrust Trust)

            => Trust.Pinned is not null
                   ? $"its server must show one of the host keys pinned ({Trust.Pinned})" +
                     (Trust.OnPinMismatch != SshOnPinMismatch.Refuse && !Trust.PasswordMayBeSent
                          ? $", and another one is believed all the same ({Trust.OnPinMismatch})"
                          : "")
                   : Trust.Learned is String learned
                         ? $"its server must show the host key learned on its first connection ({learned}), which is on probation"
                         : "no host key is pinned, so the first one its server shows is learned, on probation until a signed pin set confirms it";

        /// <summary>
        /// The sentence that says why an ssh:// connection did not get through.
        /// </summary>
        protected static String WhyNotOverSSH(String Where, SshHostKeyTrust Trust, Exception? Error)
        {

            if (Trust.Refused)
                return $"{Where} was not believed: {Trust.WhyRefused()}";

            var error = FinalOrNot(Error);

            return error switch {
                       SshAuthenticationException  => $"{Where} did not let this {NodeWording.Noun} in: {error.Message} " +
                                                       $"Its authorized_keys needs this {NodeWording.Noun}'s SSH key, which the Connections page shows",
                       SshStrictKexRequiredException
                                                   => $"{Where} does not signal strict KEX, which this {NodeWording.Noun} requires against Terrapin (CVE-2023-48795): " +
                                                       "its SSH server needs OpenSSH 9.6 or later, or another with strict KEX",
                       SshMessageRefusedException { RetryAfter: TimeSpan after }
                                                   => $"{Where} let this {NodeWording.Noun} in, and keeps another connection of it that still answers " +
                                                      $"({error.Message}); it is tried again in {after.TotalSeconds:0} s at the earliest",
                       SshMessageRefusedException  => $"{Where} let this {NodeWording.Noun} in, and refused it the OCPP subsystem: {error.Message}",
                       SshSubsystemRefusedException
                                                   => $"{Where} let this {NodeWording.Noun} in, and refused it the OCPP subsystem: " +
                                                       $"it serves no OCPP over SSH, or an access rule there denies it to this {NodeWording.Noun}",
                       null                        => $"{Where} could not be reached over SSH",
                       _                           => $"{Where} could not be reached over SSH: {ReasonOf(error)}"
                   };

        }

        /// <summary>
        /// What went wrong, in words that are the same on every station: the
        /// name of a socket error rather than the operating system's own words
        /// for it, which are translated.
        /// </summary>
        protected static String ReasonOf(Exception Error)

            => Error is SocketException socket
                   ? socket.SocketErrorCode.ToString()
                   : Error.Message;

        #endregion

        #region (private) FollowOverSSH(Connection, DialledURL, Client, Trust, Connected)

        /// <summary>
        /// Say what becomes of a connection dialled over SSH from here on - as
        /// Follow does for one over a WebSocket.
        /// </summary>
        /// <remarks>
        /// The client comes back by itself, after a drop and until it gets
        /// through at all. What it is told by the server that is final - a host
        /// key not believed, a login not taken - ends that: it is closed, and
        /// said so - before the first connection as after a drop, since the
        /// client says of every attempt of its own that failed. Nothing of it
        /// is said once this station let go of it.
        /// </remarks>
        protected void FollowOverSSH(ConnectionEntry  Connection,
                                   URL              DialledURL,
                                   WWCPSshClient    Client,
                                   SshHostKeyTrust  Trust,
                                   Boolean          Connected)
        {

            var connectedBefore = Connected;

            Client.OnConnected        += (timestamp, transport, connection, networkingNodeId, networkingMode, eventTrackingId, cancellationToken) => {

                if (IsLetGo(Client))
                    return Task.CompletedTask;

                LearnIfFirst(Connection, DialledURL, Trust, Client, connection);

                Note(Connection, connectedBefore
                                     ? "Connected again, and will come back by itself if it drops."
                                     : "Connected, and will come back by itself if it drops.");

                connectedBefore = true;

                ConnectionIsUp(Connection);

                return Task.CompletedTask;

            };

            Client.OnConnectionClosed += (timestamp, transport, connection, networkingNodeId, close, cancellationToken) => {

                probations.Remove(connection);

                if (!IsLetGo(Client))
                    Fail(Connection, ConnectionStatus.Lost,
                         $"'{Connection.Description}' was lost ({close.Code?.ToString() ?? "without a word"}" +
                         $"{(String.IsNullOrWhiteSpace(close.Reason) ? "" : $": {close.Reason}")}); it comes back by itself.");

                return Task.CompletedTask;

            };

            // Asked before every attempt, so this is where a server that shows
            // another host key than before is noticed, whichever attempt it is.
            Trust.OnRefused           += why => {

                if (IsLetGo(Client))
                    return;

                ReportFinal(Connection, Trust, null);

                Fail(Connection, ConnectionStatus.Refused,
                     $"'{Connection.Description}' was not believed when it was tried again: {why}; " +
                      "that answer is final, and it is not tried again.");

                _ = Task.Run(() => LetGo(Client, "Not believed."));

            };

            // After every attempt the client made by itself that failed: before
            // the first connection, and to come back after a drop.
            Client.OnConnectFailed    += (timestamp, client, error, attempt, nextAttemptIn, cancellationToken) => {

                if (IsLetGo(Client) || Trust.Refused)
                    return Task.CompletedTask;

                if (IsFinal(error))
                {

                    ReportFinal(Connection, Trust, error);

                    Fail(Connection, ConnectionStatus.Refused,
                         WhyNotOverSSH($"'{Connection.Description}'", Trust, error) +
                         " when it was tried again; that answer is final, and it is not tried again.");

                    _ = Task.Run(() => LetGo(Client, "Not tried again."));

                }

                else if (nextAttemptIn is TimeSpan delay)
                {
                    Log.Debug($"'{Connection.Description}': trying again in {delay.TotalSeconds:F1} s (attempt {attempt + 1}).", "ocpp", "connections");
                    NextAttempt(Connection, attempt + 1, delay);
                }

                else
                    Fail(Connection, ConnectionStatus.Failed,
                         $"{WhyNotOverSSH($"'{Connection.Description}'", Trust, error)}; it was tried {attempt} time(s), and is not tried again.");

                return Task.CompletedTask;

            };

        }

        #endregion

        #region (private) LearnIfFirst(Connection, DialledURL, Trust, Client, Current)

        /// <summary>
        /// Write down the host key a connection that trusts on first use
        /// learned, and keep the connection on probation while no signed pin
        /// set confirmed it (white paper "OCPP over SSH", section 10.4).
        /// </summary>
        /// <remarks>
        /// The connection made now is on probation before the host key is
        /// said in the log or in a security event.
        /// </remarks>
        /// <param name="Current">The connection over SSH made now.</param>
        protected void LearnIfFirst(ConnectionEntry       Connection,
                                  URL                   DialledURL,
                                  SshHostKeyTrust       Trust,
                                  WWCPSshClient         Client,
                                  SshMessageConnection? Current)
        {

            if (!Trust.OnProbation || Trust.Learned is not String learned)
                return;

            // On probation before anything is said of it: until then the gate
            // would let everything through that its server asks.
            if (Current is not null)
                probations.Enter(Current, new Probation(Connection.Id, Client, Trust, () => Trust.OnProbation));

            if (Connections.TryLearnHostKey(Connection.Id, DialledURL, learned, out var notWritten) &&
                ReportedLearned(Connection, learned))
                Log.Warning($"'{Connection.Description}' learned the host key {learned} on its first connection. It is on probation for " +
                            $"{Connection.SshProbationTime.TotalMinutes:0.#} minute(s): until a signed pin set confirms it, this {NodeWording.Noun} " +
                             "executes nothing its server asks but such a pin set.",
                            "ocpp", "connections", "ssh", "security");

            else if (notWritten is not null)
                Log.Warning($"'{Connection.Description}' learned the host key {learned} on its first connection, and it could not be " +
                            $"written down: {notWritten}. It is on probation all the same.",
                            "ocpp", "connections", "ssh", "security");

            StartProbationClock(Connection, Trust, Client, learned);

        }

        #endregion

        #region (private) ReportFinal(Connection, Trust, Error) / ReportedLearned(Connection, Learned)

        /// <summary>
        /// The security event of an answer that is final: a host key not believed, or a server without
        /// strict KEX (white paper "OCPP over SSH", section 9.4).
        /// </summary>
        protected void ReportFinal(ConnectionEntry  Connection,
                                 SshHostKeyTrust  Trust,
                                 Exception?       Error)
        {

            if (Trust.Refused)
                SshSecurityEvent(Connection, "InvalidCsmsHostKey", $"Host key {Trust.Shown} of '{Connection.Id}' refused: {Trust.WhyRefused()}");

            else if (FinalOrNot(Error) is SshStrictKexRequiredException)
                SshSecurityEvent(Connection, "InvalidSSHAlgorithms", $"The server of '{Connection.Id}' does not signal strict KEX");

        }

        /// <summary>
        /// The security event of a host key learned on first use - said, and then true.
        /// </summary>
        protected Boolean ReportedLearned(ConnectionEntry  Connection,
                                        String           Learned)
        {
            SshSecurityEvent(Connection, "CsmsHostKeyLearned", $"Host key {Learned} of '{Connection.Id}' learned on first use, on probation");
            return true;
        }

        #endregion

        #region (private) StartProbationClock(Connection, Trust, Client, Learned)

        /// <summary>
        /// Forget the learned host key where no signed pin set confirmed it in
        /// time, and let the connection go, so that it learns again after its
        /// back-off: a key learned is never kept unconfirmed, and a server that
        /// answered first cannot lock the real one out.
        /// </summary>
        protected void StartProbationClock(ConnectionEntry  Connection,
                                         SshHostKeyTrust  Trust,
                                         WWCPSshClient    Client,
                                         String           Learned)
        {

            var learnedAt  = Connections.Connections.FirstOrDefault(entry => entry.Id == Connection.Id)?.SshLearnedAt ?? TimeProvider.GetUtcNow();
            var remaining  = learnedAt + Connection.SshProbationTime - TimeProvider.GetUtcNow();
            var clock      = new CancellationTokenSource();

            if (!probations.TryStartClock(Connection.Id, clock))
            {
                clock.Dispose();
                return;
            }

            _ = Task.Run(async () => {

                try
                {
                    if (remaining > TimeSpan.Zero)
                        await Task.Delay(remaining, TimeProvider, clock.Token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                finally
                {
                    probations.ClockDone(Connection.Id, clock);
                }

                // Confirmed meanwhile: a signed pin set ended the probation.
                if (!Trust.OnProbation || Trust.Learned != Learned)
                    return;

                // Not believed before its trust forgets the key, which ends the
                // probation, and before anything is said of it: a server nobody
                // confirmed would be free in between - while the log says so, and
                // until the connection is closed.
                var onIt = probations.OfTrust(Trust);

                probations.NoLongerBelieved(onIt, $"This {NodeWording.Noun} no longer believes its server by the host key {Learned} it learned on its first " +
                                                  $"connection: no signed pin set confirmed it within {Connection.SshProbationTime.TotalSeconds:0} s, and the connection is closed.");

                Connections.TryForgetLearnedHostKey(Connection.Id, Learned);
                Trust.Forget();

                Log.Warning($"'{Connection.Description}': no signed pin set confirmed the host key {Learned} within " +
                            $"{Connection.SshProbationTime.TotalMinutes:0.#} minute(s), so it is forgotten, and the connection is made anew.",
                            "ocpp", "connections", "ssh", "security");

                foreach (var probation in onIt)
                {
                    if (probation is SshMessageConnection current)
                        await current.Close(MessageCloseCode.PolicyViolation, "The host key of this server was not confirmed in time.");
                }

                // And the SSH connection OCPP was a channel of: coming back as a
                // new channel of it would be coming back to the server nobody
                // confirmed, without a handshake to learn from.
                await DropSshConnectionOf(Client);

            });

        }

        #endregion

        #region (private) GateOnProbation(Connection, Action, Payload)

        /// <summary>
        /// Whether a request that came over the given connection is processed:
        /// over a connection on probation, nothing but a pin set - whose
        /// signature is checked where it is set. Everything else is answered
        /// with a SecurityError.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Over SSH a pin set for the SSH client: SetVariables of
        /// SSHClientController, or ChangeConfiguration of an SSHClient key in
        /// OCPP 1.6. Over a wss:// WebSocket one of key pins: SetVariables of the
        /// TLSClientController instance of this very connection, or
        /// ChangeConfiguration of evqi.serverKeyPins and evqi.serverKeyPinsVersion
        /// in OCPP 1.6 ("Key Pins and Raw Public Keys", sections 7.2 and 7.3).
        /// Each only over its own kind of connection.
        /// </para>
        /// <para>
        /// Over a wss:// WebSocket also what a CSMS reads to find that instance:
        /// GetVariables, and GetReport, of TLSClientController alone. Of any
        /// instance, since the instance is named by this station's id of the
        /// connection, which the CSMS does not know before it read the URL, the
        /// key pin shown or the one learned of each: what that tells is the
        /// station's servers and the key pins of their public keys, nothing a
        /// server on probation could use against it. GetBaseReport is refused:
        /// it names no component, and reports the whole Device Model.
        /// </para>
        /// <para>
        /// A connection whose learned key pin a signed pin set or a change of
        /// trust ended without confirming it is refused everything until it is
        /// closed - after its answer, which goes over it - see
        /// ChargingStation.TLSClient.cs.
        /// </para>
        /// </remarks>
        protected String? GateOnProbation(IMessageConnection  Connection,
                                        String              Action,
                                        JObject?            Payload)
        {

            // Off probation, and about to be closed: its server is not believed -
            // asked first by the registry. Then the rules of the probation's kind.
            return probations.Gate(Connection, Action, Payload, (probation, action, payload) =>
                       probation.Trust is SshHostKeyTrust sshTrust
                           ? GateOnSshProbation(sshTrust, action, payload)
                           : KeyPinGate.Rules(probation, action, payload, $"{NodeWording.Noun}"));

        }

        /// <summary>
        /// What a connection over SSH on probation lets through: a pin set for
        /// the SSH client, SetVariables of SSHClientController, or
        /// ChangeConfiguration of an SSHClient key in OCPP 1.6.
        /// </summary>
        protected static String? GateOnSshProbation(SshHostKeyTrust  sshTrust,
                                                  String           Action,
                                                  JObject?         Payload)
        {

            if (Action == "SetVariables" &&
                Payload?["setVariableData"] is JArray entries && entries.Count > 0 &&
                entries.All(entry => entry?["component"]?["name"]?.Value<String>() == "SSHClientController"))
                return null;

            if (Action == "ChangeConfiguration" &&
                Payload?["key"]?.Value<String>() is String key && key.StartsWith("SSHClient.", StringComparison.Ordinal))
                return null;

            return $"This {NodeWording.Noun} believes its server by the host key {sshTrust.Learned}, which it learned on its first connection " +
                    "and which is on probation: until a signed pin set confirms it, nothing but such a pin set is executed.";


        }

        #endregion

        #region (private) IsOnProbation(Client)

        /// <summary>
        /// Whether the server of the given client - an SSH client, or a
        /// WebSocket client - is believed by a key learned on its first
        /// connection that no signed pin set confirmed yet.
        /// </summary>
        protected Boolean IsOnProbation(Object Client)

            => probations.IsOnProbation(Client);

        #endregion

        #region (private) LetGo(Client, Reason) / IsLetGo(Client)

        /// <summary>
        /// Close an SSH client this station is done with, and say nothing more
        /// of what it says from here on.
        /// </summary>
        protected async Task LetGo(WWCPSshClient Client, String Reason)
        {

            lettingGo.AddOrUpdate(Client, Reason);

            try
            {
                await Disconnect(Client, Reason);
            }
            catch (Exception e)
            {
                Log.Debug($"A connection to {Client.TransportDescription} did not close cleanly: {e.Message}", "ocpp", "connections");
            }

            // And the SSH peer it was a channel of, with its connection.
            await ReleaseSshPeerOf(Client);

        }

        /// <summary>
        /// Whether this station let go of the given SSH client.
        /// </summary>
        protected Boolean IsLetGo(WWCPSshClient Client)

            => lettingGo.TryGetValue(Client, out _);

        #endregion

        #region (private static) Disconnect(Client, Reason)

        /// <summary>
        /// Close a client this station is done with, for good, and have the
        /// node that made it forget it.
        /// </summary>
        /// <remarks>
        /// A node keeps every client it made, closed or not, until it is told
        /// to let go of one: a station that went from endpoint to endpoint
        /// while none could be reached gathered a client for every one it
        /// dialled. Closed for good: a WebSocket client without its reconnect
        /// policy, an SSH client told to connect no more.
        /// </remarks>
        /// <param name="Client">The client: a WebSocket or an SSH client.</param>
        /// <param name="Reason">Why, sent with the close.</param>
        protected static async Task Disconnect(Object? Client, String Reason)
        {

            var node = (Client switch {
                            WWCPWebSocketClient webSocketClient  => webSocketClient.NetworkingNode,
                            WWCPSshClient       sshClient        => sshClient.NetworkingNode,
                            _                                    => null
                        }) as AWWCPNetworkingNode;

            if (node is not null && Client is IWWCPClient client)
            {
                await node.DisconnectClient(client, Reason);
                return;
            }

            switch (Client)
            {

                case WebSocketClient webSocketClient:
                    webSocketClient.ReconnectPolicy = null;
                    await webSocketClient.Close(WebSocketFrame.ClosingStatusCode.NormalClosure, Reason);
                    break;

                case WWCPSshClient sshClient:
                    await sshClient.Close(MessageCloseCode.NormalClosure, Reason);
                    break;

            }

        }

        #endregion

        #region (private) Follow(Connection, Client, Connected)

        /// <summary>
        /// Say what becomes of a dialled connection from here on.
        /// </summary>
        /// <remarks>
        /// The client comes back by itself - after a drop, and until it gets
        /// through at all: the node gave it its policy before its first
        /// attempt, see BuildOCPPNodes. What becomes of it is said where what
        /// happened when it was dialled is said, in the log and in
        /// DialledConnections, which otherwise went on saying "Connected" of a
        /// connection gone for an hour, and "did not become a WebSocket
        /// connection" of one made long since.
        ///
        /// Not while this station hangs up: HangUp takes the policy away first,
        /// and a close this station asked for is not a loss. Every event below
        /// asks for the policy before it says anything, so that one arriving
        /// late from a client being closed does not bring back a connection
        /// HangUp has just forgotten.
        /// </remarks>
        /// <param name="Connection">The connection, as it is written down.</param>
        /// <param name="Client">The client the node made for it.</param>
        /// <param name="Connected">Whether the dial got through, so that the next connection the client opens is "again".</param>
        protected void Follow(ConnectionEntry  Connection,
                            WebSocketClient  Client,
                            Boolean          Connected)
        {

            var connectedBefore = Connected;

            Client.OnCloseMessageReceived += (timestamp, sender, connection, frame, eventTrackingId, statusCode, reason, cancellationToken) => {

                if (Client.ReconnectPolicy is not null)
                    Fail(Connection, ConnectionStatus.Lost,
                         $"'{Connection.Description}' was lost ({(UInt16) statusCode} {statusCode}" +
                         $"{(String.IsNullOrWhiteSpace(reason) ? "" : $": {reason}")}); it comes back by itself.");

                return Task.CompletedTask;

            };

            // A connection that went without a word - a back end that crashed,
            // a socket reset underneath, pings no longer answered - arrives
            // above as well: the client reports it with a close frame of its
            // own making, 1006.

            // Before every attempt the client makes, and so what the page says
            // of when the next one is.
            Client.OnReconnecting += (timestamp, sender, attempt, delay, cancellationToken) => {

                if (Client.ReconnectPolicy is null)
                    return Task.CompletedTask;

                Log.Debug($"'{Connection.Description}': trying again in {delay.TotalSeconds:F1} s (attempt {attempt}).", "ocpp", "connections");

                NextAttempt(Connection, attempt, delay);

                return Task.CompletedTask;

            };

            // After every attempt, whatever came of it. One that ended the
            // client's trying is the end of "it comes back by itself": a back
            // end that came back as something that will not have this station
            // - a 401 after its passwords were changed, a 404 after the station
            // was removed from it. Without this the station went on promising a
            // connection that nothing was trying to make any more.
            Client.ResponseLogDelegate += (timestamp, sender, request, response) => {

                if (Client.ReconnectPolicy is not null && !Client.KeepsTrying)
                    Fail(Connection, ConnectionStatus.Refused,
                         response.HTTPStatusCode != HTTPStatusCode.SwitchingProtocols
                             ? $"'{Connection.Description}' answered {response.HTTPStatusCode} when it was tried again; " +
                                "that answer is final, and it is not tried again."
                             : $"'{Connection.Description}' ended the connection in a way that is not tried again " +
                               $"({Client.ClientCloseMessage ?? "nothing more was said"}).");

                return Task.CompletedTask;

            };

            // Sent for every connection the client opens from here on: the first
            // one a connection that failed at the start gets, or the next one
            // after a loss.
            Client.OnWebSocketConnectionAccepted += (timestamp, sender, connection, response, cancellationToken) => {

                if (Client.ReconnectPolicy is null)
                    return Task.CompletedTask;

                Note(Connection, connectedBefore
                                     ? "Connected again, and will come back by itself if it drops."
                                     : "Connected, and will come back by itself if it drops.");

                connectedBefore = true;

                ConnectionIsUp(Connection);

                return Task.CompletedTask;

            };

        }

        #endregion

        #region (private) SendWhatWaits(Connection)

        /// <summary>
        /// Send what waits in the queue of the connection's version of OCPP,
        /// now that the CSMS can be reached and has accepted this station: what
        /// was said before is not left until the next thing there is to say.
        /// </summary>
        /// <remarks>
        /// Not waited for. A CSMS slow to answer holds up the queue and not the
        /// connection, and what it does not take stays in the queue.
        /// </remarks>
        protected void SendWhatWaits(ConnectionEntry Connection)
        {

            var sending = FlushOfflineQueueOf(Connection.OCPPVersion);

            _ = sending.ContinueWith(task => Log.Warning($"Sending what waited for '{Connection.Description}' failed: {task.Exception?.GetBaseException().Message}", "ocpp", "connections"),
                                     TaskContinuationOptions.OnlyOnFaulted);

        }

        #endregion

        #region (private) Note(Connection, What) / Fail(Connection, Status, What)

        /// <summary>
        /// Say that the connection is connected.
        /// </summary>
        protected void Note(ConnectionEntry Connection, String What)
        {

            Record(Connection, ConnectionStatus.Connected, What);

            Log.Info($"'{Connection.Description}': {What}", "ocpp", "connections");

        }

        /// <summary>
        /// Say that the connection is not, and how it stands.
        /// </summary>
        protected void Fail(ConnectionEntry Connection, ConnectionStatus Status, String What)
        {

            Record(Connection, Status, What);

            // A warning and not an error: the station is doing what it can,
            // and the thing that is wrong is normally somewhere else and
            // normally fixable from the page this message is visible on.
            Log.Warning(What, "ocpp", "connections");

        }

        protected void Record(ConnectionEntry Connection, ConnectionStatus Status, String What)
        {

            lock (dialled)
                dialled[Connection.Id] = new ConnectionState(
                                             Status,
                                             TimeProvider.GetUtcNow(),
                                             What,
                                             Connection.Description,
                                             Connection.URL,
                                             Connection.OCPPVersion
                                         );

            // Whether its peer goes on to its next endpoint.
            EndpointStatus(Connection, Status);

        }

        #endregion

        #region (private) NextAttempt(Connection, Attempt, Delay)

        /// <summary>
        /// When the next attempt is, beside how the connection stands - which
        /// does not change with it: a connection lost ten minutes ago and
        /// tried for the fifth time is still lost since ten minutes ago.
        /// </summary>
        protected void NextAttempt(ConnectionEntry Connection, UInt32 Attempt, TimeSpan Delay)
        {

            lock (dialled)
                if (dialled.TryGetValue(Connection.Id, out var state))
                    dialled[Connection.Id] = state with {
                                                 Attempt        = Attempt,
                                                 NextAttemptAt  = TimeProvider.GetUtcNow() + Delay
                                             };

            // One attempt too many, and its peer goes on to its next endpoint.
            EndpointRetries(Connection);

        }

        #endregion

        #region TestConnection(Id, CancellationToken = default)

        /// <summary>
        /// Make this one connection, once, write down everything that happened,
        /// and close it again.
        /// </summary>
        /// <remarks>
        /// For the button beside a connection on the page, and for the reason
        /// that button exists: a connection that is only written down is never
        /// tried, so the first time anybody finds out whether the address, the
        /// certificate and the password are right is the day it is switched on
        /// - which is normally the day it has to work.
        ///
        /// It is its own client and not one of the OCPP nodes'. A test that
        /// left a client behind in the node would be a test that changed the
        /// station, and running it twice would leave two. What it measures is
        /// everything up to and including the WebSocket handshake - the name,
        /// the route, the TLS, the credentials, the sub-protocol - which is
        /// where connections actually fail.
        ///
        /// It closes politely rather than dropping the socket: a back end that
        /// is told the connection is going away logs a test, and one that is
        /// left hanging logs a fault.
        ///
        /// <b>It tests what it is handed, not what is stored.</b> The fields
        /// arrive from the form somebody is looking at, which is the only
        /// answer that is never surprising: the button beside a connection
        /// being written down for the first time has nothing stored to test,
        /// and the button beside one being edited would otherwise test the
        /// version on disk while somebody watches the version on screen. What
        /// is stored is not touched either way - a test writes nothing.
        /// </remarks>
        /// <param name="Description">What it is; only for the log and the answer.</param>
        /// <param name="URL">Where it goes.</param>
        /// <param name="ConnectionType">What is at the other end; a label.</param>
        /// <param name="OCPPVersion">Which OCPP, and so which sub-protocols are offered.</param>
        /// <param name="AutoConnect">Whether this is one the station would connect to by itself, which the answer mentions but does not act on.</param>
        /// <param name="AuthenticationId">The credentials to prove itself with, by identification.</param>
        /// <param name="CertificateId">The client certificate to prove itself with, by identification.</param>
        /// <param name="SshHostKey">The host keys the server of an ssh:// connection may show, separated by commas; none to be told which one it shows.</param>
        /// <param name="SshTrust">How an ssh:// connection believes its server beyond its pins: what a mismatch leads to, and whether it would learn a key.</param>
        /// <param name="ServerKeyPins">The key pins of the keys the server of a wss:// connection may show, separated by commas; none for one checked by its CA, or one that would learn the key.</param>
        /// <param name="KeyPinTrust">Whether a wss:// connection offers raw public keys and would learn a key pin on first use.</param>
        /// <param name="Pins">What the server of a wss:// connection checked by its CA is held to beside that, as the form writes it under "pins"; nothing for nothing.</param>
        /// <param name="HTTPVersion">Which version of HTTP a WebSocket is opened over; nothing for what the server chooses.</param>
        /// <param name="OCPPRequest">What is asked in OCPP once it is connected: "heartbeat", which it is where nothing is said, or "none".</param>
        /// <param name="Encoding">How its messages are written - "json", "cbor" or "auto"; nothing for "json".</param>
        public async Task<JObject> TestConnection(String?                Description,
                                                  String?                URL,
                                                  String?                ConnectionType,
                                                  String?                OCPPVersion           = null,
                                                  Boolean                AutoConnect           = false,
                                                  String?                AuthenticationId      = null,
                                                  String?                CertificateId         = null,
                                                  CancellationToken      CancellationToken     = default,
                                                  String?                SshHostKey            = null,
                                                  SshTrustSettings?      SshTrust              = null,
                                                  String?                ServerKeyPins         = null,
                                                  ServerKeyPinSettings?  KeyPinTrust           = null,
                                                  JToken?                Pins                  = null,
                                                  String?                HTTPVersion           = null,
                                                  String?                OCPPRequest           = null,
                                                  String?                Encoding              = null)
        {

            var clock  = Stopwatch.StartNew();
            var steps  = new JArray();

            // What is asked in OCPP once it is connected, and what came of it.
            TestRequest? asked = null;

            // The key pin of the key the server of a wss:// connection showed,
            // whether it was believed or not: what somebody commissioning the
            // station pins it to.
            KeyPin? shownKeyPin = null;

            void Step(String Level, String Text)
                => steps.Add(new JObject(
                       new JProperty("at_ms",  clock.ElapsedMilliseconds),
                       new JProperty("level",  Level),
                       new JProperty("text",   Text)
                   ));

            JObject Done(ConnectionEntry? Connection, Boolean OK)
            {

                clock.Stop();

                var result = new JObject(
                                 new JProperty("description",  Connection?.Description ?? (Description ?? "").Trim()),
                                 new JProperty("url",          Connection?.URL.ToString() ?? (URL ?? "").Trim()),
                                 new JProperty("ok",           OK),
                                 new JProperty("runtime_ms",   clock.ElapsedMilliseconds),
                                 new JProperty("steps",        steps)
                             );

                if (shownKeyPin is not null)
                    result.Add(new JProperty("serverKeyPin", shownKeyPin.ToHex()));

                if (asked is not null && asked.WasSent)
                    result.Add(new JProperty("ocpp", asked.ToJSON()));

                return result;

            }

            #region Is it even a connection

            // The same rules as writing one down, and the same sentences: a
            // test of a half-filled form should say what is missing rather
            // than fail at a socket with something less useful.
            if (!ConnectionEntry.Validate(Description, URL, ConnectionType, OCPPVersion,
                                          out var url, out var type, out var version, out var notYet) ||
                !ConnectionEntry.TryParseHTTPVersion(url, HTTPVersion, out var httpVersion, out notYet) ||
                !ConnectionEntry.TryParseEncoding(version, Encoding, out var encoding, out notYet) ||
                !ConnectionEntry.ValidateHostKey(url, SshHostKey, out var hostKey, out notYet) ||
                !ConnectionEntry.ValidateSshTrust(SshTrust?.TrustOnFirstUse, SshTrust?.OnPinMismatch, SshTrust?.ProbationSeconds,
                                                  out var trustOnFirstUse, out var onPinMismatch, out var probationTime, out notYet) ||
                !ConnectionEntry.ValidateServerKeyPins(url, ServerKeyPins, out var serverKeyPins, out notYet) ||
                !ConnectionEntry.ValidateKeyPinTrust(url, KeyPinTrust?.RawPublicKeys, KeyPinTrust?.TrustOnFirstUse, KeyPinTrust?.ProbationSeconds,
                                                     out var rawPublicKeys, out var keyTrustOnFirstUse, out var keyPinProbationTime, out notYet) ||
                !ConnectionEntry.CheckKeyPinned(serverKeyPins, rawPublicKeys, keyTrustOnFirstUse, out notYet) ||
                !ConnectionEntry.ValidatePins(url, Pins, ConnectionEntry.PinsKey, out var pins, out notYet) ||
                !ConnectionEntry.CheckOneWay(serverKeyPins, keyTrustOnFirstUse, pins, out notYet))
            {
                Step("error", notYet);
                return Done(null, false);
            }

            switch ((OCPPRequest ?? "").Trim().ToLowerInvariant())
            {

                case "":
                case TestAsksHeartbeat:
                    asked = new TestRequest("Heartbeat", clock, TimeProvider);
                    break;

                case TestAsksNothing:
                    break;

                default:
                    Step("error", $"'{OCPPRequest!.Trim()}' is nothing a connection test asks in OCPP: it is '{TestAsksHeartbeat}', " +
                                  $"which it is where nothing is said, or '{TestAsksNothing}'.");
                    return Done(null, false);

            }

            // A throwaway, so that everything below reads one shape whether it
            // came from the store or from a form. Nothing writes it anywhere.
            var connection = ConnectionEntry.NotOfAStore(
                                 Description:                Description!.Trim(),
                                 URL:                        url,
                                 ConnectionType:             type,
                                 CreatedAt:                  TimeProvider.GetUtcNow(),
                                 OCPPVersion:                version,
                                 AutoConnect:                AutoConnect,
                                 AuthenticationId:           AuthenticationId,
                                 CertificateId:              CertificateId,
                                 SshHostKey:                 hostKey,
                                 SshTrustOnFirstUse:         trustOnFirstUse,
                                 SshOnPinMismatch:           onPinMismatch,
                                 SshProbationTime:           probationTime,
                                 ServerKeyPins:              serverKeyPins,
                                 RawPublicKeys:              rawPublicKeys,
                                 TrustOnFirstUse:            keyTrustOnFirstUse,
                                 ServerKeyPinProbationTime:  keyPinProbationTime,
                                 Pins:                       pins,
                                 HTTPVersion:                httpVersion ?? WebSocketHTTPVersion.Auto,
                                 Encoding:                   encoding    ?? OCPPCommon.WebSockets.MessageEncodings.JSON
                             );

            // What this station made of the certificate of a server checked by
            // its CA: its name, its chain, and what the form holds it to.
            protocols.WWCP.Node.ServerJudgement? judged = null;

            #endregion

            Log.Info($"Testing the connection to '{connection.Description}' ({connection.URL}) ...", "ocpp", "connections", "test");

            #region What is going to be tried

            Step("info", $"{ConnectionEntry.AsText(connection.OCPPVersion)} to {connection.URL}, " +
                         $"written down as {connection.ConnectionType}.");

            IHTTPAuthentication?  basic         = null;
            TOTPConfig?           totp          = null;
            String?               totpLogin     = null;
            X509Certificate2[]?   certificates  = null;
            SshClientOptions?     sshLogin      = null;
            SshHostKeyTrust?      sshTrust      = null;
            ServerKeyPinTrust?    keyPins       = null;
            SslPolicyErrors?      caSaid        = null;

            if (connection.IsSSH)
            {

                Step("info", $"The URL is an ssh:// one: OCPP goes over the SSH subsystem {OCPPCommon.SSH.OCPPSshServer.OCPPSubsystem}, " +
                              "encrypted, and its server is known by its host key.");

                // Made here where there is none yet: a test of an ssh://
                // connection is a test of signing in with it.
                var key = await SshKey.MakeIfNeeded(CancellationToken);

                if (!SshLoginOf(connection, key, out sshLogin, out sshTrust, out var signsIn, out var notSSH, ForTest: true))
                {
                    Step("error", $"Not tried: {notSSH}.");
                    Log.Warning($"The connection test for '{connection.Description}' was not run: {notSSH}.", "ocpp", "connections", "test");
                    return Done(connection, false);
                }

                Step("info", $"It will sign in with {signsIn}. The server lets it in where its authorized_keys has this line: " +
                             $"{SshKey.PublicKeyLine}");

                Step(sshTrust.Pinned is not null ? "info" : "warning",
                     sshTrust.Pinned is not null
                         ? $"Its server must show one of the host keys pinned: {sshTrust.Pinned}."
                         : sshTrust.Learned is String learned
                               ? $"Its server must show the host key learned on its first connection, {learned}, which is on probation."
                               : "No host key is pinned. Whatever key the server shows is said below and believed for this test; " +
                                 "a test writes nothing down" +
                                 (connection.SshTrustOnFirstUse == SshTrustOnFirstUse.HostKey
                                      ? $", so the first connection this {NodeWording.Noun} makes learns it, on probation."
                                      : $", and without a pin or trust on first use the {NodeWording.Noun} does not dial it."));

            }
            else
            {

                if (connection.IsKeyPinned)
                {

                    // Held to what the form says, as the dialling would be: a
                    // test writes nothing down, so a key it learns is believed
                    // for the test only.
                    keyPins = new ServerKeyPinTrust(connection, () => connection);

                    Step(connection.ServerKeyPins.Count > 0 ? "info" : "warning",
                         connection.ServerKeyPins.Count > 0
                             ? $"The URL is a TLS one, and its server is known by the key pin of its key: it must show one of {ConnectionEntry.AsText(connection.ServerKeyPins)}. " +
                                "Its certificate's chain, name and validity are not checked."
                             : "The URL is a TLS one, and its server is known by its key, of which none is pinned. Whatever key it shows is said " +
                               $"below and believed for this test; a test writes nothing down, so the first connection this {NodeWording.Noun} makes learns it, " +
                               "on probation.");

                    if (connection.RawPublicKeys != RawPublicKeyMode.Off)
                        Step("warning", connection.RawPublicKeys == RawPublicKeyMode.Require
                                            ? $"It requires raw public keys, which this {NodeWording.Noun}'s TLS stack cannot offer yet: a certificate is refused, " +
                                              $"and the {NodeWording.Noun} does not dial it."
                                            : $"It prefers raw public keys, which this {NodeWording.Noun}'s TLS stack cannot offer yet: the key is checked in the certificate.");

                }

                else
                    Step(connection.IsSecure ? "info" : "warning",
                         connection.IsSecure
                             ? $"The URL is a TLS one, so the certificate of whatever answers will be checked against this {NodeWording.Noun}'s trust store."
                             : "The URL is not a TLS one. Nothing on this connection is encrypted or authenticated at the transport layer.");

                if (!WhatItProvesItselfWith(connection, out basic, out totp, out totpLogin, out certificates, out var proves, out var wrong))
                {
                    Step("error", $"Not tried: {wrong}.");
                    Log.Warning($"The connection test for '{connection.Description}' was not run: {wrong}.", "ocpp", "connections", "test");
                    return Done(connection, false);
                }

                Step("info", $"It will prove itself with {proves}.");

            }

            if (!connection.AutoConnect)
                Step("info", $"This is not one the {NodeWording.Noun} connects to by itself. A test that gets through does not " +
                             "change that - it only says that it could.");

            #endregion

            #region Does the name resolve

            var host = connection.URL.Host.ToString();

            // With this station's DNS client, as everything the station dials
            // is: what it finds is what the probe below connects to. A name it
            // finds nothing for ends the test here - the connection itself
            // would not get further, and no other resolver is asked. The
            // socket used to be handed the name, and the operating system
            // answered for a station whose name servers did not.
            System.Net.IPAddress[] addresses;

            try
            {

                addresses = [.. await NameResolution.ResolveAsync(host, DNSClient, CancellationToken)];

                if (!System.Net.IPAddress.TryParse(host.Trim('[', ']'), out _))
                    Step("info", $"'{host}' resolves to {String.Join(", ", addresses.Take(8).Select(address => address.ToString()))}.");

            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {

                Step("error", $"'{host}' could not be looked up with this {NodeWording.Noun}'s name servers: {e.Message} " +
                              "Nothing else is asked, so the rest was not tried.");

                Log.Warning($"The connection test for '{connection.Description}' could not look up '{host}': {e.Message}",
                            "ocpp", "connections", "test");

                return Done(connection, false);

            }

            #endregion

            #region Can the socket even be opened

            // Asked separately, and it is the difference between the two
            // answers somebody actually needs: "nothing is there" and "something
            // is there and it said no". The WebSocket client cannot tell them
            // apart - when its connection is refused it has no stream to read
            // and answers itself with a 400 that carries a Date, a
            // Content-Type and a Content-Length, indistinguishable from a real
            // one. Measured, against a port nothing was listening on. So the
            // reachable-or-not question is settled here, before anything can
            // invent an answer to it.
            var port = connection.URL.Port?.ToUInt16() ?? (connection.IsSSH    ? (UInt16)  22
                                                         : connection.IsSecure ? (UInt16) 443
                                                                               : (UInt16)  80);

            try
            {

                using var probe = new TcpClient();

                await probe.ConnectAsync(addresses, port, CancellationToken);

                Step("info", $"A TCP connection to {host}:{port} was accepted.");

            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {

                // The name of the socket error and never the operating
                // system's own words for it: those are translated, and a
                // German station and an English one would say different things
                // about the same fault.
                var why = e is SocketException socket
                              ? socket.SocketErrorCode.ToString()
                              : e.GetType().Name;

                Step("error", $"Nothing accepted a TCP connection on {host}:{port} ({why}). " +
                              $"There is nothing to speak {(connection.IsSSH ? "SSH" : "WebSocket")} to, so the rest was not tried.");

                Log.Warning($"The connection test for '{connection.Description}' found nothing at {host}:{port}: {why}.",
                            "ocpp", "connections", "test");

                return Done(connection, false);

            }

            #endregion

            if (connection.IsSSH)
                return await TestOverSSH(connection, sshLogin!, sshTrust!, asked, Step, Done, CancellationToken);

            WebSocketClient? client = null;

            try
            {

                #region Open it

                // Key-pinned, the key is checked as the dialling checks it. CA
                // checked, the answer is SslStream's own - no policy error, or no
                // connection - and the key pin of the key is only looked at, for
                // somebody who wants to pin it.
                client = new WebSocketClient(
                             connection.URL,
                             DNSClient:                   DNSClient,
                             HTTPAuthentication:          basic,
                             SecWebSocketProtocols:       SubProtocolsOf(connection),
                             RemoteCertificateValidator:  keyPins is not null
                                                              ? keyPins.Validator
                                                              : connection.IsSecure
                                                                    ? (sender, certificate, chain, webSocketClient, policyErrors) => {

                                                                          shownKeyPin  = certificate is not null ? KeyPin.FromCertificate(certificate) : null;
                                                                          caSaid       = policyErrors;

                                                                          // As the dialling judges it, but only asked: a test
                                                                          // learns nothing, remembers nothing, and logs nothing
                                                                          // as the judgement of a connection in effect.
                                                                          judged       = JudgeServer(ConnectionEntry.PinsService,
                                                                                                     connection.URL.Host.ToString(),
                                                                                                     connection.Pins,
                                                                                                     certificate,
                                                                                                     chain,
                                                                                                     policyErrors,
                                                                                                     Evidence:  false,
                                                                                                     Learn:     null,
                                                                                                     Remember:  false);

                                                                          return judged.Accepted
                                                                                     ? TLSValidationResult.Success()
                                                                                     : TLSValidationResult.Failed(WhyRefused(judged));

                                                                      }
                                                                    : null,
                             ClientCertificates:          certificates,
                             TOTPConfig:                  totp,
                             TOTPLogin:                   totpLogin,
                             DisableWebSocketPings:       true,
                             DisableLogging:              true
                         );

                // Whether the handshake came to a CBOR subprotocol: known once it is through.
                var speaksCBOR = false;

                client.OnTextMessageReceived   += (timestamp, sender, conn, frame, tracking, message, token) => {
                    // The answer to what the test asked is said where it is waited for.
                    if (asked?.IsAnsweredBy(message) != true)
                        Step("info", $"Received: {(message.Length > 400 ? message[..400] + " ..." : message)}");
                    return Task.CompletedTask;
                };

                client.OnBinaryMessageReceived += (timestamp, sender, conn, frame, tracking, message, token) => {

                    // What a connection that agreed on CBOR carries, shown as the JSON it stands for.
                    if (speaksCBOR && OCPPCommon.WebSockets.OCPP_CBORFrame.TryAsJSON(message, out var shown, out _))
                    {

                        var text = shown.ToString(Newtonsoft.Json.Formatting.None);

                        if (asked?.IsAnsweredBy(text) != true)
                            Step("info", $"Received, as CBOR: {(text.Length > 400 ? text[..400] + " ..." : text)}");

                    }

                    else
                        Step("info", $"Received {message.Length} byte(s) of binary.");

                    return Task.CompletedTask;

                };

                client.OnCloseMessageReceived  += (timestamp, sender, conn, frame, tracking, status, reason, token) => {
                    Step("info", $"The other end closed the connection: {status}{(reason is not null ? $" ({reason})" : "")}.");
                    return Task.CompletedTask;
                };

                // As the dialling would: over the version of HTTP the form names.
                client.WebSocketOver = connection.HTTPVersion;

                Step("info", $"Connecting, offering {String.Join(", ", SubProtocolsOf(connection))}" +
                             connection.HTTPVersion switch {
                                 WebSocketHTTPVersion.HTTP3    => ", over HTTP/3 and nothing else",
                                 WebSocketHTTPVersion.HTTP2    => ", over HTTP/2 and nothing else",
                                 WebSocketHTTPVersion.HTTP1_1  => ", over HTTP/1.1 and nothing else",
                                 _                             => connection.IsSecure
                                                                      ? WebSocketAlternativeServices.TryGetHTTP3(connection.URL, out _)
                                                                            ? ", over HTTP/3, where the server said before that it is reached"
                                                                            : ", over HTTP/2 or HTTP/1.1 as the server chooses"
                                                                      : ", over HTTP/1.1: without TLS there is nothing to agree on another with"
                             } +
                             " ...");

                var (_, response) = await client.Connect(
                                              MaxNumberOfRetries:  0,
                                              CancellationToken:   CancellationToken
                                          );

                #endregion

                #region What came back

                if (keyPins is not null)
                    shownKeyPin = keyPins.Shown;

                if (keyPins?.LastValidation is KeyPinValidation validation)
                    Step(validation.IsAccepted ? "info" : "error",
                         validation.Outcome switch {
                             KeyPinOutcome.Accepted  => $"The server showed a key whose key pin is {validation.Pin}, one of those pinned.",
                             KeyPinOutcome.Learned   => $"The server showed a key whose key pin is {validation.Pin}. None is pinned, so it was believed for this test. " +
                                                         "Pin it to decide yourself.",
                             _                       => $"Not believed: {keyPins.WhyRefused()}. Nothing was asked of it."
                         });

                else if (judged is not null)
                {

                    Step(judged.Accepted ? "info" : "error",
                         judged.Accepted
                             ? $"The server showed the certificate {judged.CertificateFingerprint}, which this {NodeWording.Noun} accepts" +
                               (judged.RootFingerprint is not null ? $": its chain ends at the root {judged.RootFingerprint} ({judged.RootKeyPin})" : "") +
                               $". Its key has the key pin {shownKeyPin}."
                             : $"The server showed the certificate {judged.CertificateFingerprint}, which this {NodeWording.Noun} does not accept ({judged.Outcome}), " +
                               $"for a key whose key pin is {shownKeyPin}. Pinned as a key pin, that key would be believed by itself, without a CA.");

                    // Everything the judgement went through, in its own words:
                    // which root vouched for it, what it is held to, what it
                    // showed instead.
                    foreach (var (level, text) in judged.Steps)
                        Step(level, text);

                    if (judged.Accepted && connection.Pins is { TrustOnFirstUse: not protocols.WWCP.Node.Configuration.TrustOnFirstUse.None } learning)
                        Step("info", $"A test writes nothing down: the first connection this {NodeWording.Noun} makes holds the server to its {protocols.WWCP.Node.Configuration.TrustOnFirstUseExtensions.AsText(learning.TrustOnFirstUse)}.");

                }

                else if (shownKeyPin is not null)
                    Step(caSaid == SslPolicyErrors.None ? "info" : "error",
                         caSaid == SslPolicyErrors.None
                             ? $"The server showed a certificate this {NodeWording.Noun}'s trust store accepts, for a key whose key pin is {shownKeyPin}."
                             : $"The server showed a certificate this {NodeWording.Noun}'s trust store does not accept ({caSaid}), for a key whose key pin is " +
                               $"{shownKeyPin}. Pinned, that key would be believed by itself, without a CA.");

                if (response.HTTPStatusCode != HTTPStatusCode.SwitchingProtocols)
                {

                    Step("error", $"It did not become a WebSocket connection: {response.HTTPStatusCode}.");

                    // The socket was accepted a moment ago, so whatever is
                    // there either refused the upgrade or dropped the
                    // connection before answering. The headers below are worth
                    // reading only in the first case, and the client writes
                    // the same shape in both - so they are offered as what the
                    // client has rather than as what the far end said.
                    Step("info", $"What the client has of the answer: " +
                                 String.Join("; ", response.Take(8).Select(header => $"{header.Key}: {header.Value}")) +
                                 ". The socket was accepted, so something is there - it either would not upgrade, " +
                                 "or went away before saying anything.");

                    Log.Warning($"The connection test for '{connection.Description}' did not get through: {response.HTTPStatusCode}.",
                                "ocpp", "connections", "test");

                    return Done(connection, false);

                }

                // Read out of the headers rather than off a typed property:
                // on a response this is a single header that may simply not be
                // there, which is what a server that ignores sub-protocols
                // sends - and is worth saying, because an OCPP back end that
                // does that is a back end that has not agreed to speak OCPP.
                if (client.HTTPVersionInUse is HTTPVersion over)
                    Step("info", $"The WebSocket was opened over HTTP/{ConnectionEntry.AsText(over)}" +
                                 (connection.HTTPVersion == WebSocketHTTPVersion.Auto && connection.IsSecure ? ", which the server chose" : "") +
                                 ".");

                var agreedOn = response.FirstOrDefault(header => header.Key.Equals("Sec-WebSocket-Protocol",
                                                                                   StringComparison.OrdinalIgnoreCase)).
                                        Value?.ToString();

                Step("notice", agreedOn is not null && agreedOn.Length > 0
                                   ? $"Connected. The handshake was accepted for {agreedOn}."
                                   : "Connected, but the handshake named no sub-protocol. Whatever is there took the " +
                                     "connection without agreeing to speak OCPP.");

                speaksCBOR = OCPPCommon.WebSockets.OCPPSubprotocols.IsCBOR(agreedOn);

                if (speaksCBOR)
                    Step("info", "Its messages are CBOR in binary frames.");

                // As the dialling would: a connection that says "cbor" speaks nothing else.
                else if (connection.Encoding == OCPPCommon.WebSockets.MessageEncodings.CBOR)
                {

                    Step("error", $"It did not agree on {String.Join(", ", SubProtocolsOf(connection))}: whatever is there does not speak " +
                                  $"OCPP as CBOR, and a connection that says \"cbor\" speaks nothing else. The {NodeWording.Noun} would let go of it again.");

                    Log.Warning($"The connection test for '{connection.Description}' got through, and its back end does not speak CBOR.",
                                "ocpp", "connections", "test");

                    return Done(connection, false);

                }

                #endregion

                #region Ask it something in OCPP

                // The handshake says that something took the connection. Whether
                // it speaks OCPP on it - whether a request goes there, is read,
                // and its answer comes back - is only known once it was asked.
                var answered = asked is null ||
                               await asked.Ask(
                                         async text => speaksCBOR
                                                           ? await client.SendBinaryMessage(CBORFrameOf(text), CancellationToken: CancellationToken) == SentStatus.Success
                                                           : await client.SendTextMessage  (text,              CancellationToken: CancellationToken) == SentStatus.Success,
                                         Step,
                                         TestAnswerWithin,
                                         CancellationToken
                                     );

                #endregion

                #region Hold it briefly, then let go

                Step("info", $"Staying connected for {TestHoldsFor.TotalSeconds:0.#} second(s), to see whether anything is said.");

                await Task.Delay(TestHoldsFor, CancellationToken);

                Step("info", "Closing.");

                await client.Close(
                          WebSocketFrame.ClosingStatusCode.NormalClosure,
                          "Connection test finished",
                          CancellationToken: CancellationToken
                      );

                if (!answered)
                {

                    Step("error", $"Closed. The connection can be made, and what the {NodeWording.Noun} asked on it was not answered.");

                    Log.Warning($"The connection test for '{connection.Description}' got through, and its {asked!.Action} was not answered.",
                                "ocpp", "connections", "test");

                    return Done(connection, false);

                }

                Step("notice", "Closed. The connection can be made.");

                #endregion

                Log.Notice($"The connection test for '{connection.Description}' got through.", "ocpp", "connections", "test");

                return Done(connection, true);

            }
            catch (Exception e)
            {

                shownKeyPin ??= keyPins?.Shown;

                if (keyPins is not null && keyPins.Refused)
                    Step("error", $"Not believed: {keyPins.WhyRefused()}. Nothing was asked of it.");

                Step("error", $"{e.GetType().Name}: {e.Message}");

                Log.Warning($"The connection test for '{connection.Description}' failed: {e.Message}", "ocpp", "connections", "test");

                return Done(connection, false);

            }
            finally
            {
                // Whatever happened, nothing of this test is left connected.
                if (client is not null)
                {
                    try
                    {
                        client.ReconnectPolicy = null;
                        await client.Close();
                    }
                    catch
                    { }
                }
            }

        }

        #endregion

        #region (private class) TestRequest

        /// <summary>
        /// What a connection test asks in OCPP once it is connected, and what
        /// came of it: the request as it was sent, the answer as it came, and
        /// how long that took.
        /// </summary>
        /// <remarks>
        /// A Heartbeat, and written by hand: it is the one request that is the
        /// same in OCPP 1.6 and 2.x, asks nothing of the back end but the time,
        /// and changes nothing there - a BootNotification would register a
        /// station that is only being tried. It does not go through the OCPP
        /// nodes, as the test's connection does not: nothing of a test reaches
        /// them.
        ///
        /// An error is an answer. A back end that has not accepted a
        /// BootNotification of this station may answer anything else with a
        /// CALLERROR (OCPP 2.x: SecurityError), and it has then read a
        /// request and answered it in OCPP, which is what was asked.
        /// </remarks>
        protected sealed class TestRequest(String Action, Stopwatch Clock, TimeProvider TimeProvider)
        {

            private readonly TaskCompletionSource<JArray> answer = new (TaskCreationOptions.RunContinuationsAsynchronously);

            private Int64?           sentAt_ms;
            private Int64?           answeredAt_ms;
            private DateTimeOffset?  answeredAt;
            private JArray?          answered;

            /// <summary>The action of the request: "Heartbeat".</summary>
            public String   Action   { get; } = Action;

            /// <summary>What its answer is known by.</summary>
            public String   Id       { get; } = "test-" + Guid.NewGuid().ToString("N")[..12];

            /// <summary>The request as it is sent: an OCPP CALL.</summary>
            public JArray   Message
                => new (2, Id, Action, new JObject());

            /// <summary>Whether it was sent at all: not where the connection did not come that far.</summary>
            public Boolean  WasSent
                => sentAt_ms.HasValue;

            /// <summary>
            /// Whether the given text is the answer to this request - a
            /// CALLRESULT or a CALLERROR with its id - which is then kept.
            /// </summary>
            public Boolean IsAnsweredBy(String Text)
            {

                try
                {

                    if (JToken.Parse(Text) is JArray { Count: >= 2 } message &&
                        message[0].Type == JTokenType.Integer &&
                        message[0].Value<Int32>() is 3 or 4   &&
                        message[1].Type == JTokenType.String  &&
                        message[1].Value<String>() == Id)
                    {

                        answeredAt_ms ??= Clock.ElapsedMilliseconds;
                        answeredAt    ??= TimeProvider.GetUtcNow();

                        return answer.TrySetResult(message);

                    }

                }
                catch (Newtonsoft.Json.JsonException)
                { }

                return false;

            }

            /// <summary>
            /// Send it, wait for its answer, and say what came of it.
            /// </summary>
            /// <param name="Send">Sends a text over the connection, and says whether it left.</param>
            /// <param name="Step">Writes down what happened.</param>
            /// <param name="Within">How long the answer is waited for.</param>
            /// <returns>Whether it was answered, by a result or by an error.</returns>
            public async Task<Boolean> Ask(Func<String, Task<Boolean>>  Send,
                                           Action<String, String>       Step,
                                           TimeSpan                     Within,
                                           CancellationToken            CancellationToken)
            {

                var text = Message.ToString(Newtonsoft.Json.Formatting.None);

                Step("info", $"Asking in OCPP, to see whether it is spoken here: {text}");

                sentAt_ms = Clock.ElapsedMilliseconds;

                if (!await Send(text))
                {
                    Step("error", $"The {Action} could not be sent: the connection was gone.");
                    return false;
                }

                try
                {
                    answered = await answer.Task.WaitAsync(Within, CancellationToken);
                }
                catch (TimeoutException)
                {
                    Step("error", $"No answer to the {Action} within {Within.TotalSeconds.ToString("0.#", System.Globalization.CultureInfo.InvariantCulture)} second(s). Whatever is there took the " +
                                   "connection, and does not answer OCPP on it.");
                    return false;
                }

                var took = answeredAt_ms!.Value - sentAt_ms.Value;
                var said = answered.ToString(Newtonsoft.Json.Formatting.None);

                if (said.Length > 400)
                    said = said[..400] + " ...";

                if (answered[0].Value<Int32>() == 3)
                {

                    // What a Heartbeat is answered with: the time of the back
                    // end, which says how far this station's clock is from it.
                    var theirs = answered.Count > 2 && answered[2] is JObject payload &&
                                 payload["currentTime"] is JValue { Value: not null } currentTime
                                     ? currentTime.Type == JTokenType.Date
                                           ? new DateTimeOffset(currentTime.Value<DateTime>().ToUniversalTime())
                                           : DateTimeOffset.TryParse(currentTime.Value<String>(),
                                                                     System.Globalization.CultureInfo.InvariantCulture,
                                                                     System.Globalization.DateTimeStyles.AssumeUniversal |
                                                                     System.Globalization.DateTimeStyles.AdjustToUniversal,
                                                                     out var parsed)
                                                 ? parsed
                                                 : (DateTimeOffset?) null
                                     : null;

                    Step("notice", $"Answered after {took} ms: {said}" +
                                   (theirs is { } time
                                        ? $" Its clock says {time.UtcDateTime:yyyy-MM-ddTHH:mm:ss.fffZ}, " +
                                          $"{Math.Abs((Int64) (time - answeredAt!.Value).TotalMilliseconds)} ms " +
                                          $"{(time >= answeredAt.Value ? "ahead of" : "behind")} this {NodeWording.Noun}'s."
                                        : ""));

                }

                else
                    Step("warning", $"Answered after {took} ms, with an error: {said} OCPP is spoken here. A back end answers so " +
                                     "where it does not take the request - one that has not accepted a BootNotification of this " +
                                     $"{NodeWording.Noun} yet may.");

                return true;

            }

            /// <summary>
            /// What was asked and what came of it, for the answer of the test.
            /// </summary>
            public JObject ToJSON()
            {

                var json = new JObject(
                               new JProperty("request",   Action),
                               new JProperty("sent",      Message),
                               new JProperty("sentAt_ms", sentAt_ms),
                               new JProperty("answered",  answered is not null)
                           );

                if (answered is not null)
                {
                    json.Add(new JProperty("answer",        answered));
                    json.Add(new JProperty("error",         answered[0].Value<Int32>() == 4));
                    json.Add(new JProperty("roundTrip_ms",  answeredAt_ms!.Value - sentAt_ms!.Value));
                }

                return json;

            }

        }

        #endregion

        #region TestConnection(Fields, CancellationToken = default)

        /// <summary>
        /// Test a connection by its fields as JSON: what the page sends of the
        /// form somebody is looking at, and what a connection that is written
        /// down says of itself.
        /// </summary>
        /// <param name="Fields">The fields of the connection, and "ocppRequest" for what is asked in OCPP.</param>
        public Task<JObject> TestConnection(JObject            Fields,
                                            CancellationToken  CancellationToken   = default)

            => TestConnection(Fields.Value<String>("description"),
                              Fields.Value<String>("url"),
                              Fields.Value<String>("connectionType"),
                              Fields.Value<String>("ocppVersion"),
                              Fields.Value<Boolean?>("autoConnect") ?? false,
                              Fields.Value<String>("authenticationId"),
                              Fields.Value<String>("certificateId"),
                              CancellationToken,
                              Fields.Value<String>("sshHostKey"),
                              ConnectionFields.SshTrustOf(Fields),
                              ConnectionFields.KeyPinsOf(Fields, "serverKeyPins"),
                              ConnectionFields.KeyPinTrustOf(Fields),
                              Fields[ConnectionEntry.PinsKey],
                              Fields.Value<String>(ConnectionEntry.HTTPVersionKey),
                              Fields.Value<String>(TestRequestKey),
                              Fields.Value<String>(ConnectionEntry.EncodingKey));

        /// <summary>
        /// Under which name a test is told what to ask in OCPP: "ocppRequest".
        /// </summary>
        public const String TestRequestKey = "ocppRequest";

        #endregion

        #region (private) TestOverSSH(Connection, Login, Trust, Asked, Step, Done, CancellationToken)

        /// <summary>
        /// The rest of a connection test over SSH, once a TCP connection could
        /// be made: sign in, ask for the OCPP subsystem, hold it briefly, and
        /// let go.
        /// </summary>
        /// <remarks>
        /// Its own client, as over a WebSocket, and not one of the OCPP nodes':
        /// nothing it is told reaches them, and nothing of it is left behind.
        /// </remarks>
        protected async Task<JObject> TestOverSSH(ConnectionEntry                      Connection,
                                                SshClientOptions                     Login,
                                                SshHostKeyTrust                      Trust,
                                                TestRequest?                         Asked,
                                                Action<String, String>               Step,
                                                Func<ConnectionEntry?, Boolean, JObject>  Done,
                                                CancellationToken                    CancellationToken)
        {

            var subprotocols  = SubProtocolsOf(Connection);

            // Whether the server took a CBOR subprotocol: known once it is connected.
            var speaksCBOR    = false;

            var client        = new WWCPSshClient(
                                    NodeOf(Connection.OCPPVersion),
                                    WWCPSshClient.OptionsFrom(Connection.URL, Login, OCPPCommon.SSH.OCPPSshServer.OCPPSubsystem, subprotocols, PingEvery: SshPingEvery),
                                    ConnectAs: WWCPSshClient.ConnectAsFrom(Connection.URL)
                                );

            client.MessageClient.OnTextMessageReceived   += (timestamp, sender, connection, text, cancellationToken) => {
                // The answer to what the test asked is said where it is waited for.
                if (Asked?.IsAnsweredBy(text) != true)
                    Step("info", $"Received: {(text.Length > 400 ? text[..400] + " ..." : text)}");
                return Task.CompletedTask;
            };

            client.MessageClient.OnBinaryMessageReceived += (timestamp, sender, connection, bytes, cancellationToken) => {

                // What a connection that agreed on CBOR carries, shown as the JSON it stands for.
                if (speaksCBOR && OCPPCommon.WebSockets.OCPP_CBORFrame.TryAsJSON(bytes, out var shown, out _))
                {

                    var text = shown.ToString(Newtonsoft.Json.Formatting.None);

                    if (Asked?.IsAnsweredBy(text) != true)
                        Step("info", $"Received, as CBOR: {(text.Length > 400 ? text[..400] + " ..." : text)}");

                }

                else
                    Step("info", $"Received {bytes.Length} byte(s) of binary.");

                return Task.CompletedTask;

            };

            client.OnConnectionClosed                    += (timestamp, transport, connection, networkingNodeId, close, cancellationToken) => {
                if (close.ByPeer)
                    Step("info", $"The other end closed the connection: {close.Code?.ToString() ?? "without a word"}" +
                                 $"{(String.IsNullOrWhiteSpace(close.Reason) ? "" : $" ({close.Reason})")}.");
                return Task.CompletedTask;
            };

            String ShownKey()
                => $"{Trust.ShownKind} {Trust.Shown}";

            try
            {

                Step("info", $"Connecting as '{Login.Username}', asking for {OCPPCommon.SSH.OCPPSshServer.OCPPSubsystem} " +
                             $"and offering {String.Join(", ", subprotocols)} ...");

                var connection = await client.Connect(CancellationToken);

                speaksCBOR = OCPPCommon.WebSockets.OCPPSubprotocols.IsCBOR(connection.Subprotocol);

                // As the dialling would: a connection that says "cbor" speaks nothing else.
                if (!speaksCBOR && Connection.Encoding == OCPPCommon.WebSockets.MessageEncodings.CBOR)
                {

                    Step("error", $"The server did not take {String.Join(", ", subprotocols)}: it does not speak OCPP as CBOR, and a " +
                                  $"connection that says \"cbor\" speaks nothing else. The {NodeWording.Noun} would let go of it again.");

                    Log.Warning($"The connection test for '{Connection.Description}' got through, and its back end does not speak CBOR.",
                                "ocpp", "connections", "test");

                    return Done(Connection, false);

                }

                Step(Trust.Mismatched ? "warning" : "info",
                     Trust.Mismatched
                         ? $"The server showed the host key {ShownKey()}, which is none of those pinned; it was believed all the same, as OnPinMismatch " +
                           $"says '{Trust.OnPinMismatch}'. A host key pin is the only check of the server, so it may be anybody."
                         : Trust.Pinned is not null
                               ? $"The server showed the host key {ShownKey()}, one of those pinned."
                               : $"The server showed the host key {ShownKey()}. None is pinned, so it was believed for this test. Pin it to decide yourself.");

                Step("notice", connection.Subprotocol is String agreedOn && agreedOn.Length > 0
                                   ? $"Connected. The server let '{Login.Username}' in and took {agreedOn}."
                                   : $"Connected, and the server let '{Login.Username}' in, but it named no OCPP version. Whatever is " +
                                      "there took the connection without agreeing to speak OCPP.");

                // As over a WebSocket: whether OCPP is spoken on it is only
                // known once it was asked.
                var answered = Asked is null ||
                               await Asked.Ask(
                                         async text => (speaksCBOR
                                                            ? await client.SendBinaryMessage(CBORFrameOf(text),
                                                                                             TimeProvider.GetUtcNow(),
                                                                                             org.GraphDefined.Vanaheimr.Illias.EventTracking_Id.New,
                                                                                             CancellationToken)
                                                            : await client.SendJSONMessage  (JArray.Parse(text),
                                                                                             TimeProvider.GetUtcNow(),
                                                                                             org.GraphDefined.Vanaheimr.Illias.EventTracking_Id.New,
                                                                                             CancellationToken)).Result == SentMessageResults.Success,
                                         Step,
                                         TestAnswerWithin,
                                         CancellationToken
                                     );

                Step("info", $"Staying connected for {TestHoldsFor.TotalSeconds:0.#} second(s), to see whether anything is said.");

                await Task.Delay(TestHoldsFor, CancellationToken);

                Step("info", "Closing.");

                await client.Close(MessageCloseCode.NormalClosure, "Connection test finished");

                if (!answered)
                {

                    Step("error", $"Closed. The connection can be made, and what the {NodeWording.Noun} asked on it was not answered.");

                    Log.Warning($"The connection test for '{Connection.Description}' got through, and its {Asked!.Action} was not answered.",
                                "ocpp", "connections", "test");

                    return Done(Connection, false);

                }

                Step("notice", "Closed. The connection can be made.");

                Log.Notice($"The connection test for '{Connection.Description}' got through.", "ocpp", "connections", "test");

                return Done(Connection, true);

            }
            catch (Exception e)
            {

                if (Trust.Refused)
                    Step("error", $"Not believed: {Trust.WhyRefused()}. Nothing was signed in.");

                else
                {

                    if (Trust.Shown is not null)
                        Step("info", $"The server showed the host key {ShownKey()}{(Trust.Pinned is not null ? ", one of those pinned" : "")}.");

                    var error = FinalOrNot(e) ?? e;

                    Step("error", error switch {
                                      SshAuthenticationException  => $"The server did not let '{Login.Username}' in: {error.Message} " +
                                                                      $"Its authorized_keys needs this {NodeWording.Noun}'s key: {SshKey.PublicKeyLine}",
                                      SshStrictKexRequiredException
                                                                  => $"The server does not signal strict KEX, which this {NodeWording.Noun} requires against Terrapin " +
                                                                     "(CVE-2023-48795): its SSH server needs OpenSSH 9.6 or later.",
                                      SshMessageRefusedException  => $"The server let '{Login.Username}' in, and refused the OCPP subsystem: {error.Message}",
                                      SshSubsystemRefusedException
                                                                  => $"The server let '{Login.Username}' in, and refused the OCPP subsystem: " +
                                                                      $"it serves no OCPP over SSH, or an access rule there denies it to this {NodeWording.Noun}.",
                                      _                           => $"{error.GetType().Name}: {ReasonOf(error)}"
                                  });

                }

                Log.Warning($"The connection test for '{Connection.Description}' failed: " +
                            $"{(Trust.Refused ? Trust.WhyRefused() : ReasonOf(e.GetBaseException()))}", "ocpp", "connections", "test");

                return Done(Connection, false);

            }
            finally
            {
                // Whatever happened, nothing of this test is left connected.
                try
                {
                    await client.DisposeAsync();
                }
                catch
                { }
            }

        }

        #endregion

        #region (private) DialAgain(Old16, Old21, Because)

        /// <summary>
        /// After a change of the EVSEs, or of what this station says it is, or
        /// after a restart: hang up what the OCPP nodes it replaced were connected to, stop
        /// them, and dial again through the new ones what this station dials
        /// at its start.
        /// </summary>
        /// <remarks>
        /// A connection belongs to the node that made it, and nothing moves it
        /// to another. Left where it was, the old node went on answering the
        /// CSMS with the EVSEs as they had been, and nothing this station said
        /// afterwards reached the CSMS: it went through the new nodes, which
        /// had no connection. So it is hung up and dialled again, and the CSMS
        /// sees the connection close and come back - and the new nodes'
        /// BootNotification, which says what this station is now.
        ///
        /// Dialled again only where this station is started and the old nodes
        /// had dialled something. One at a time, so that a second change does
        /// not hang up what the first is still dialling, but after it: the
        /// first one's dialling goes through the nodes the second one replaces.
        /// </remarks>
        /// <param name="Old16">The OCPP 1.6 node replaced.</param>
        /// <param name="Old21">The OCPP 2.1 node replaced.</param>
        /// <param name="Because">Why, as a sentence without its full stop: "The EVSEs of this charging station changed".</param>
        protected async Task DialAgain(OCPPv1_6.NetworkingNode.AOCPPNetworkingNode?  Old16,
                                     OCPPv2_1.NetworkingNode.AOCPPNetworkingNode?  Old21,
                                     String                               Because)
        {

            // Nothing more is asked of a CSMS for nodes let go of.
            EndRegistrations(Old16, Old21);

            await dialAgainLock.WaitAsync();

            try
            {

                var clients     = Old16.OCPPWebSocketClients.OfType<WebSocketClient>().
                                      Concat(Old21.OCPPWebSocketClients.OfType<WebSocketClient>()).
                                      ToArray();

                var sshClients  = Old16.WWCPSshClients.
                                      Concat(Old21.WWCPSshClients).
                                      ToArray();

                // Before the nodes stop, which closes them too - as a loss.
                foreach (var sshClient in sshClients)
                    await LetGo(sshClient, $"{Because}; it connects again.");

                // As HangUp does: without its policy first, so that the close
                // is neither a loss nor something to come back from.
                foreach (var client in clients)
                {
                    try
                    {
                        client.ReconnectPolicy = null;
                        await client.Close(WebSocketFrame.ClosingStatusCode.NormalClosure,
                                           $"{Because}; it connects again.");
                    }
                    catch (Exception e)
                    {
                        Log.Debug($"A connection to {client.RemoteURL} did not close cleanly: {e.Message}", "ocpp", "connections");
                    }
                }

                foreach (var node in new AWWCPNetworkingNode[] { Old16, Old21 })
                {
                    try
                    {
                        await node.Stop(Message: $"{Because}.");
                    }
                    catch (Exception e)
                    {
                        Log.Warning($"Stopping the OCPP node '{node.Id}' that was replaced failed: {e.Message}", "ocpp");
                    }
                }

                if (started && clients.Length + sshClients.Length > 0)
                {

                    Log.Notice($"{Because}, so what the OCPP nodes it replaced were connected to was hung up " +
                               $"({clients.Length + sshClients.Length} connection(s)); it is dialled again.", "ocpp", "connections");

                    await DialConfiguredConnections();

                }

            }
            finally
            {
                dialAgainLock.Release();
            }

        }

        #endregion

        #region (private) HangUp()

        /// <summary>
        /// Close every connection this station made.
        /// </summary>
        /// <remarks>
        /// Closed rather than dropped, because a close that says so is a close
        /// that does not start a reconnect: the policy deliberately ignores a
        /// shutdown the application asked for. A station restarting would
        /// otherwise spend its last second dialling.
        /// </remarks>
        protected async Task HangUp()
        {

            // No peer goes on to another endpoint, nor is dialled again.
            LetGoOfPeers();

            // Nothing more is asked of a CSMS, nor waited for.
            EndRegistrations(Node16, Node21);

            foreach (var client in Nodes.SelectMany(node => node.OCPPWebSocketClients).
                                         OfType<WebSocketClient>().
                                         ToArray())
            {
                try
                {
                    await Disconnect(client, $"This {NodeWording.Noun} hangs up.");
                }
                catch (Exception e)
                {
                    Log.Debug($"A connection to {client.RemoteURL} did not close cleanly: {e.Message}", "ocpp", "connections");
                }
            }

            foreach (var sshClient in Nodes.SelectMany(node => node.WWCPSshClients).ToArray())
                await LetGo(sshClient, $"This {NodeWording.Noun} hangs up.");

            lock (dialled)
                dialled.Clear();

        }

        #endregion

    }

}
