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

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Messaging;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.KeyPins
{

    /// <summary>
    /// How a connection that is not believed stands, as whoever dials it says it.
    /// </summary>
    public enum PinnedConnectionStatus
    {

        /// <summary>
        /// It is tried again by itself.
        /// </summary>
        Trying,

        /// <summary>
        /// It was connected, is not any more, and comes back by itself.
        /// </summary>
        Lost

    }


    /// <summary>
    /// What the white paper "Key Pins and Raw Public Keys for Local Controllers" asks of a node as a TLS
    /// client: a wss:// connection whose server - typically a Local Controller, which no CA issues a
    /// certificate for - is known by the key pin of its key rather than by a CA (section 5.4); trust on
    /// first use where it is configured, and a key pin learned that way on probation until a signed pin
    /// set confirms it (sections 5.6 and 5.7); and the security events of section 7.5.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For whoever keeps the connections - a charging station its many, a Local Controller its line up
    /// to the CSMS - through <see cref="IPinnedTLSEndpoints"/>. What it cannot know itself comes as
    /// delegates: what goes into the log, how a security event is sent, and how a connection's state is
    /// said. None of them is waited for: what has to stand before an answer leaves - a connection on
    /// probation, or not believed any more - is written down before any of them is called.
    /// </para>
    /// <para>
    /// The signed pin set itself arrives over the OCPP Device Model, as SetVariables of
    /// TLSClientController (<see cref="TLSClientControllerSource"/>), or as a configuration key of OCPP
    /// 1.6, and is handed over in <see cref="TryTakeSignedServerKeyPins"/>.
    /// </para>
    /// </remarks>
    public sealed class ServerKeyPinning
    {

        #region Data

        // The trust each key-pinned connection was dialled with last: what its
        // server showed, and what its WebSocket client asks at every handshake.
        private readonly ConcurrentDictionary<String, ServerKeyPinTrust>  trusts  = new ();

        private readonly String                                                 nodeName;
        private readonly Action<String>                                         notice;
        private readonly Action<String>                                         warning;
        private readonly Action<IPinnedTLSEndpoint, String, String>             securityEvent;
        private readonly Action<IPinnedTLSEndpoint, PinnedConnectionStatus, String>  failed;

        #endregion

        #region Properties

        /// <summary>
        /// Where the connections are written down.
        /// </summary>
        public IPinnedTLSEndpoints  Endpoints     { get; }

        /// <summary>
        /// The connections on probation, and those not believed any more: what the gate asks.
        /// </summary>
        public Probations           Probations    { get; }

        /// <summary>
        /// The clock of the probations.
        /// </summary>
        public TimeProvider         TimeProvider  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The key pins of the given connections.
        /// </summary>
        /// <param name="Endpoints">Where the connections are written down.</param>
        /// <param name="Probations">The connections on probation, shared with every other kind of connection of the node.</param>
        /// <param name="Notice">A sentence for the log, of something that went as it should.</param>
        /// <param name="Warning">A sentence for the log, of something to look at.</param>
        /// <param name="SecurityEvent">Send a security event of section 7.5: the connection, its type, its techInfo.</param>
        /// <param name="Failed">Say how a connection stands that is not believed.</param>
        /// <param name="TimeProvider">The clock of the probations.</param>
        /// <param name="NodeName">What the node is called in a sentence: "station", "local controller".</param>
        public ServerKeyPinning(IPinnedTLSEndpoints                                          Endpoints,
                                Probations                                                   Probations,
                                Action<String>                                               Notice,
                                Action<String>                                               Warning,
                                Action<IPinnedTLSEndpoint, String, String>                   SecurityEvent,
                                Action<IPinnedTLSEndpoint, PinnedConnectionStatus, String>   Failed,
                                TimeProvider?                                                TimeProvider   = null,
                                String                                                       NodeName       = "node")
        {

            this.nodeName       = NodeName;
            this.Endpoints      = Endpoints;
            this.Probations     = Probations;
            this.notice         = Notice;
            this.warning        = Warning;
            this.securityEvent  = SecurityEvent;
            this.failed         = Failed;
            this.TimeProvider   = TimeProvider ?? System.TimeProvider.System;

        }

        #endregion


        #region TrustOf(EndpointId)

        /// <summary>
        /// The trust the given connection was dialled with last, where it is key-pinned: what its server
        /// showed, and the validator its WebSocket client asks.
        /// </summary>
        public ServerKeyPinTrust? TrustOf(String EndpointId)

            => trusts.TryGetValue(EndpointId, out var trust)
                   ? trust
                   : null;

        #endregion

        #region TrustFor(Endpoint, out NotDialled)

        /// <summary>
        /// The trust a wss:// connection is dialled with, where it is key-pinned; null where it is checked
        /// by its CA.
        /// </summary>
        /// <param name="Endpoint">The connection, as it is dialled.</param>
        /// <param name="NotDialled">Why it is not dialled at all: it requires raw public keys, which .NET's TLS stack cannot offer yet.</param>
        public ServerKeyPinTrust? TrustFor(IPinnedTLSEndpoint  Endpoint,
                                           out String?         NotDialled)
        {

            NotDialled = null;

            if (!Endpoint.IsKeyPinned)
                return null;

            // .NET's SslStream offers no raw public key, and a certificate is
            // what such a connection refuses: dialled, it would only be turned
            // away, and report its server's certificate as invalid when it is
            // this node that cannot do what was asked.
            if (Endpoint.RawPublicKeys == RawPublicKeyMode.Require)
            {
                NotDialled = $"requires raw public keys, which this {nodeName}'s TLS stack cannot offer yet. Prefer or Off check the same " +
                             "key pins in a certificate";
                return null;
            }

            var id     = Endpoint.Id;
            var trust  = new ServerKeyPinTrust(Endpoint, () => Endpoints.CopyOf(id));

            // Reported once per change of the verdict, at the first attempt as at
            // every one after: the client keeps trying with its back-off, as
            // after any TLS handshake that failed, and would otherwise report
            // the same key again at every attempt.
            trust.OnRefused += (validation, newVerdict) => {
                if (newVerdict)
                    ReportRefused(Endpoint, trust);
            };

            trusts[id] = trust;

            return trust;

        }

        #endregion

        #region Follow(Endpoint, Client, Trust, DialledURL, Connected)

        /// <summary>
        /// Follow a key-pinned connection: a key pin learned on the connection made now and on every one
        /// after - the first, or one learned again after a probation ran out - and a server not believed
        /// when it is tried again.
        /// </summary>
        /// <remarks>
        /// The client keeps its policy: a key that matches none of the pins is refused at every
        /// handshake, and tried again after the back-off, as a certificate a CA does not vouch for is.
        /// Nothing is believed meanwhile, and a key pinned or learned later - the real server back, a
        /// signed pin set - is taken at the next attempt.
        /// </remarks>
        /// <param name="Endpoint">The connection, as it is written down.</param>
        /// <param name="Client">The client made for it.</param>
        /// <param name="Trust">The trust its server is believed by.</param>
        /// <param name="DialledURL">Where it went when it was dialled.</param>
        /// <param name="Connected">Whether the dial got through: the connection made now is then on probation, where it learned its key pin.</param>
        public void Follow(IPinnedTLSEndpoint  Endpoint,
                           WebSocketClient     Client,
                           ServerKeyPinTrust   Trust,
                           URL                 DialledURL,
                           Boolean             Connected)
        {

            if (Connected)
                LearnIfFirst(Endpoint, DialledURL, Trust, Client, (Client as WWCPWebSocketClient)?.WebSocketClientConnection);

            Client.OnWebSocketConnectionAccepted += (timestamp, sender, connection, response, cancellationToken) => {

                if (Client.ReconnectPolicy is not null)
                    LearnIfFirst(Endpoint, DialledURL, Trust, Client, connection);

                return Task.CompletedTask;

            };

            Client.OnCloseMessageReceived        += (timestamp, sender, connection, frame, eventTrackingId, statusCode, reason, cancellationToken) => {
                Probations.Remove(connection);
                return Task.CompletedTask;
            };

            Trust.OnRefused                      += (validation, newVerdict) => {

                if (Client.ReconnectPolicy is null)
                    return;

                failed(Endpoint, PinnedConnectionStatus.Trying,
                       $"'{Endpoint.Description}' was not believed when it was tried again: {Trust.WhyRefused()}; " +
                        "it is tried again by itself, and said here when it gets through.");

            };

        }

        #endregion

        #region (static) WhatItBelieves(Trust)

        /// <summary>
        /// What a key-pinned connection believes its server by, as the end of a sentence.
        /// </summary>
        public static String WhatItBelieves(ServerKeyPinTrust Trust)

            => Trust.Pinned is String pinned
                   ? $"its server must show a key whose key pin is one of those pinned ({pinned}), and nothing else of its certificate is checked"
                   : Trust.Learned is KeyPin learned
                         ? $"its server must show the key whose key pin was learned on its first connection ({learned}), which is on probation"
                         : "no key is pinned, so the key pin of the first key its server shows is learned, on probation until a signed pin set confirms it";

        #endregion

        #region LearnIfFirst(Endpoint, DialledURL, Trust, Client, Current)

        /// <summary>
        /// Write down the key pin a wss:// connection that trusts on first use learned, and keep the
        /// connection on probation while no signed pin set confirmed it ("Key Pins and Raw Public Keys",
        /// section 5.6).
        /// </summary>
        /// <remarks>
        /// The connection made now is on probation before anything is said of it: until then the gate
        /// would let everything through that its server asks.
        /// </remarks>
        /// <param name="Endpoint">The connection, as it is written down.</param>
        /// <param name="DialledURL">Where it went when it was dialled.</param>
        /// <param name="Trust">The trust that learned the key pin.</param>
        /// <param name="Client">The client that made the connection.</param>
        /// <param name="Current">The connection made now.</param>
        public void LearnIfFirst(IPinnedTLSEndpoint   Endpoint,
                                 URL                  DialledURL,
                                 ServerKeyPinTrust    Trust,
                                 Object               Client,
                                 IMessageConnection?  Current)
        {

            if (Trust.Learned is not KeyPin learned)
                return;

            if (Current is not null)
                Probations.Enter(Current, new Probation(Endpoint.Id, Client, Trust, () => Trust.OnProbation));

            if (Endpoints.TryLearnServerKeyPin(Endpoint.Id, DialledURL, learned, out var notWritten))
            {

                securityEvent(Endpoint, "KeyPinLearned", $"Key pin {learned} of endpoint '{Endpoint.Id}' ({DialledURL}) learned on first use, on probation");

                warning($"'{Endpoint.Description}' learned the key pin {learned} of its server's key on its first connection. It is on probation for " +
                        $"{Endpoint.ServerKeyPinProbationTime.TotalMinutes:0.#} minute(s): until a signed pin set confirms it, this {nodeName} " +
                        $"executes nothing its server asks but such a pin set.");

            }

            else if (notWritten is not null)
                warning($"'{Endpoint.Description}' learned the key pin {learned} of its server's key on its first connection, and it could not be " +
                        $"written down: {notWritten}. It is on probation all the same.");

            StartProbationClock(Endpoint, Trust, learned);

        }

        #endregion

        #region (private) StartProbationClock(Endpoint, Trust, Learned)

        /// <summary>
        /// Forget the learned key pin where no signed pin set confirmed it in time, and drop the
        /// connection, so that it learns again after its back-off: a key pin learned is never kept
        /// unconfirmed, and a server that answered first cannot lock the real one out.
        /// </summary>
        private void StartProbationClock(IPinnedTLSEndpoint  Endpoint,
                                         ServerKeyPinTrust   Trust,
                                         KeyPin              Learned)
        {

            var now        = Endpoints.CopyOf(Endpoint.Id);
            var learnedAt  = now?.ServerKeyPinLearnedAt     ?? TimeProvider.GetUtcNow();
            var probation  = now?.ServerKeyPinProbationTime ?? Endpoint.ServerKeyPinProbationTime;
            var remaining  = learnedAt + probation - TimeProvider.GetUtcNow();
            var clock      = new CancellationTokenSource();

            if (!Probations.TryStartClock(Endpoint.Id, clock))
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
                    Probations.ClockDone(Endpoint.Id, clock);
                }

                // Confirmed meanwhile: a signed pin set ended the probation.
                if (!Trust.OnProbation || Trust.Learned != Learned)
                    return;

                // Not believed before its trust forgets the key, which ends the
                // probation, and before anything is said of it: a server nobody
                // confirmed would be free in between - while the security event
                // goes out, and until the connection is closed.
                var onIt = Probations.OfTrust(Trust);

                Probations.NoLongerBelieved(onIt, $"This {nodeName} no longer believes its server by the key pin {Learned} it learned on its first " +
                                                  $"connection: no signed pin set confirmed it within {probation.TotalSeconds:0} s, and the connection is closed.");

                Endpoints.TryForgetLearnedServerKeyPin(Endpoint.Id, Learned);
                Trust.Forget();

                securityEvent(Endpoint, "KeyPinProbationExpired",
                              $"Key pin {Learned} of endpoint '{Endpoint.Id}' ({Trust.URL}) not confirmed within {probation.TotalSeconds:0} s; discarded");

                warning($"'{Endpoint.Description}': no signed pin set confirmed the key pin {Learned} within " +
                        $"{probation.TotalMinutes:0.#} minute(s), so it is forgotten, and the connection is made anew.");

                foreach (var connection in onIt)
                {

                    // Closed underneath its client, which takes that for a loss
                    // and comes back after its back-off - and learns again.
                    if (connection is WebSocketClientConnection current)
                    {

                        await current.Close(WebSocketFrame.ClosingStatusCode.PolicyViolation, "The key of this server was not confirmed in time.");

                        failed(Endpoint, PinnedConnectionStatus.Lost,
                               $"'{Endpoint.Description}' was closed: no signed pin set confirmed the key pin of its server in time. " +
                                "It comes back by itself, and learns again.");

                    }

                }

            });

        }

        #endregion

        #region (private) ReportRefused(Endpoint, Trust)

        /// <summary>
        /// The security event of a key that matches none of the key pins: InvalidCsmsCertificate, as
        /// A00.FR.310 reports a certificate a station did not accept ("Key Pins and Raw Public Keys",
        /// section 7.5). Whoever sends it for OCPP 1.6 calls it InvalidCentralSystemCertificate.
        /// </summary>
        private void ReportRefused(IPinnedTLSEndpoint  Endpoint,
                                   ServerKeyPinTrust   Trust)

            => securityEvent(Endpoint, "InvalidCsmsCertificate",
                             $"Key pin mismatch: {Trust.Shown?.ToHex() ?? "no key"} at endpoint '{Endpoint.Id}' ({Trust.URL})");

        #endregion


        #region TryTakeSignedServerKeyPins(EndpointId, Pins, Version, Signer, out Error, AfterAnswer = null)

        /// <summary>
        /// Take the key pins of a signed pin set for a wss:// connection: what ServerKeyPins and
        /// ServerKeyPinsVersion of TLSClientController (OCPP 2.x) or evqi.serverKeyPins (OCPP 1.6) hand
        /// over, once a signer believed was found to have signed them ("Key Pins and Raw Public Keys",
        /// sections 5.7, 7.2 and 7.3).
        /// </summary>
        /// <remarks>
        /// Only with a higher version than the one the connection holds: the store decides that. Its
        /// trust is held to the pins from the next handshake on, and a probation ends: confirmed, where
        /// the pins have the key pin learned; otherwise with that server refused - the connection is
        /// dropped once the answer is out, and comes back held to the signed pins. Where the store does
        /// not take the pins, nothing changes and nothing is said.
        /// </remarks>
        /// <param name="EndpointId">The connection.</param>
        /// <param name="Pins">The key pins of the pin set, separated by commas.</param>
        /// <param name="Version">The version of the pin set.</param>
        /// <param name="Signer">Who signed it, for the log.</param>
        /// <param name="Error">Why the pins were not taken.</param>
        /// <param name="AfterAnswer">Done once the answer to the request that brought the pin set is out: a connection to a server the pins refuse is dropped then, and refused everything until. At once, where nothing is answered.</param>
        public Boolean TryTakeSignedServerKeyPins(String                           EndpointId,
                                                  String                           Pins,
                                                  UInt64                           Version,
                                                  String                           Signer,
                                                  [NotNullWhen(false)] out String? Error,
                                                  Task?                            AfterAnswer   = null)
        {

            // A key pin learned on first use that could not be written down is
            // held only by its trust. The signed pin set judges that one all the
            // same: otherwise a connection on probation to a server its CSMS does
            // not know would go on, and without the gate. Read before the store
            // writes the pins, which ends what the trust says it learned.
            var held = TrustOf(EndpointId)?.Learned;

            // Held on probation while the store writes the pins: the write ends
            // what the trust says it learned, before it is known whether the
            // server is believed afterwards.
            var onIt = Probations.Hold(Probations.OfEndpoint(EndpointId));

            try
            {

                if (!Endpoints.TryTakeSignedServerKeyPins(EndpointId, Pins, Version, out var learned, out var confirmed, out Error))
                    return false;

                if (learned is null && held is not null)
                {
                    learned    = held;
                    confirmed  = Endpoints.CopyOf(EndpointId)?.ServerKeyPins.Contains(held) == true;
                }

                ServerKeyPinSetTaken(EndpointId, onIt, learned, confirmed, Version, Signer, AfterAnswer ?? Task.CompletedTask);

                return true;

            }
            finally
            {
                Probations.Release(onIt);
            }

        }

        #endregion

        #region (private) ServerKeyPinSetTaken(EndpointId, Learned, Confirmed, Version, Signer, AfterAnswer)

        /// <summary>
        /// A signed pin set was taken for a wss:// connection: a probation ends - confirmed, or with the
        /// server it learned refused.
        /// </summary>
        /// <remarks>
        /// A connection to a server refused is written down as not believed before anything is said of
        /// it, and only then taken off probation: in between a request of that server would get through
        /// the gate. It is closed once the answer to the pin set is out, which goes over it: closed
        /// before, it took the answer with it.
        /// </remarks>
        private void ServerKeyPinSetTaken(String                             EndpointId,
                                          IReadOnlyList<IMessageConnection>  onIt,
                                          KeyPin?                            Learned,
                                          Boolean  Confirmed,
                                          UInt64   Version,
                                          String   Signer,
                                          Task     AfterAnswer)
        {

            if (Endpoints.CopyOf(EndpointId) is not IPinnedTLSEndpoint entry)
                return;

            if (Learned is not null && !Confirmed)
                Probations.NoLongerBelieved(onIt, $"This {nodeName} no longer believes its server by the key pin {Learned} it learned on its first " +
                                                  $"connection: the signed pin set of version {Version} does not have it, and the connection is closed.");

            else
                foreach (var probation in onIt)
                    Probations.Remove(probation);

            // The pins are what the next handshake is held to; nothing learned
            // is believed beside them.
            TrustOf(EndpointId)?.Forget();

            Probations.TryStopClock(EndpointId);

            var pins = KeyPinText.Of(entry.ServerKeyPins);

            if (Learned is not null && Confirmed)
            {

                notice($"'{entry.Description}': the signed pin set of version {Version}, signed by '{Signer}', confirms the key pin " +
                       $"{Learned} it learned on its first connection; its probation ends.");

                securityEvent(entry, "KeyPinConfirmed",
                              $"Key pin {Learned} of endpoint '{entry.Id}' ({entry.URL}) confirmed by the signed pin set of version {Version}");

            }

            else if (Learned is not null)
            {

                warning($"'{entry.Description}': the signed pin set of version {Version}, signed by '{Signer}', does not have the key pin " +
                        $"{Learned} it learned on its first connection - that server is not the one its CSMS knows. The connection is made anew, " +
                        $"held to the signed pins ({pins}).");

                securityEvent(entry, "KeyPinRejected",
                              $"Key pin {Learned} of endpoint '{entry.Id}' ({entry.URL}) is not among the signed pins of version {Version}, which replace it");

                foreach (var probation in onIt)
                    if (probation is WebSocketClientConnection current)
                        _ = Task.Run(async () => {
                                await AfterAnswer;
                                await current.Close(WebSocketFrame.ClosingStatusCode.PolicyViolation, "The key of this server is not among the signed pins.");
                            });

            }

            else
                securityEvent(entry, "ReconfigurationOfSecurityParameters",
                              $"Key pins of endpoint '{entry.Id}' ({entry.URL}) set by a signed pin set of version {Version}");

        }

        #endregion

        #region TryChangeKeyPinTrust(EndpointId, Settings, Signer, AfterAnswer, out Error)

        /// <summary>
        /// Change how a wss:// connection believes its server beyond its key pins, from a signed
        /// SetVariables: written down by the store, and then <see cref="TrustChanged"/>.
        /// </summary>
        /// <remarks>
        /// Held on probation while the store writes: a write that switches trust on first use off forgets
        /// the key pin learned, which ends the probation before the connection is written down as not
        /// believed.
        /// </remarks>
        /// <param name="EndpointId">The connection.</param>
        /// <param name="Settings">What changes.</param>
        /// <param name="Signer">Who signed the change, for the log.</param>
        /// <param name="AfterAnswer">Done once the answer to the change is out.</param>
        /// <param name="Error">Why nothing was changed.</param>
        public Boolean TryChangeKeyPinTrust(String                            EndpointId,
                                            ServerKeyPinSettings              Settings,
                                            String                            Signer,
                                            Task                              AfterAnswer,
                                            [NotNullWhen(false)] out String?  Error)
        {

            var onIt = Probations.Hold(Probations.OfEndpoint(EndpointId));

            try
            {

                if (!Endpoints.TryChangeKeyPinTrust(EndpointId, Settings, out var forgotten, out var keyPinnedChanged, out Error))
                    return false;

                TrustChanged(EndpointId, forgotten, keyPinnedChanged, Signer, AfterAnswer);

                return true;

            }
            finally
            {
                Probations.Release(onIt);
            }

        }

        #endregion

        #region TrustChanged(EndpointId, Forgotten, KeyPinnedChanged, Signer, AfterAnswer)

        /// <summary>
        /// A signed SetVariables changed how a wss:// connection believes its server beyond its pins. Where
        /// that forgot a key pin learned on probation - trust on first use switched off - the probation ends
        /// with it: nothing believes that key any more. Where the connection is key-pinned now and was not,
        /// or the other way round, it is dialled again, once the answer is out.
        /// </summary>
        /// <remarks>
        /// A connection made with a key that is no longer believed is refused everything until it is hung
        /// up - the gate let nothing but a pin set through while it was on probation, and would let
        /// everything through once it is not. It is hung up by being dialled again: as one that is no longer
        /// key-pinned, it is checked by its CA.
        /// </remarks>
        /// <param name="EndpointId">The connection.</param>
        /// <param name="Forgotten">The key pin learned on probation that went with trust on first use; null where none did.</param>
        /// <param name="KeyPinnedChanged">Whether it has to be dialled again.</param>
        /// <param name="Signer">Who signed the change, for the log.</param>
        /// <param name="AfterAnswer">Done once the answer to the change is out.</param>
        public void TrustChanged(String   EndpointId,
                                 KeyPin?  Forgotten,
                                 Boolean  KeyPinnedChanged,
                                 String   Signer,
                                 Task     AfterAnswer)
        {

            if (Endpoints.CopyOf(EndpointId) is not IPinnedTLSEndpoint entry)
                return;

            if (Forgotten is not null)
            {

                Probations.NoLongerBelieved(Probations.OfEndpoint(EndpointId),
                                            $"This {nodeName} no longer believes its server by the key pin {Forgotten} it learned on its first " +
                                             "connection: trust on first use was switched off, and the connection is made anew.");

                TrustOf(EndpointId)?.Forget();

                Probations.TryStopClock(EndpointId);

                warning($"'{entry.Description}': a signed SetVariables, signed by '{Signer}', switched trust on first use off, so the key pin " +
                        $"{Forgotten} it learned on its first connection is forgotten and the connection is made anew, checked by its CA.");

            }

            securityEvent(entry, "ReconfigurationOfSecurityParameters",
                          $"How endpoint '{entry.Id}' ({entry.URL}) believes its server, set by a signed SetVariables" +
                          (Forgotten is not null ? $"; learned key pin {Forgotten} forgotten" : ""));

            if (KeyPinnedChanged)
                _ = Task.Run(async () => {
                        await AfterAnswer;
                        Endpoints.DialAgain(EndpointId);
                    });

        }

        #endregion

    }

}
