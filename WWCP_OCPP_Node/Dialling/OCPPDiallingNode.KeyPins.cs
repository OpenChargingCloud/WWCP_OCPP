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

using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP.Node.Connections;

using cloud.charging.open.protocols.OCPP.Node.KeyPins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// What the white paper "Key Pins and Raw Public Keys for Local Controllers"
    /// asks of the station as a TLS client: a wss:// connection whose server -
    /// typically its Local Controller, which no CA issues a certificate for - is
    /// known by the key pin of its key rather than by a CA (section 5.4); trust
    /// on first use where it is configured, and a key pin learned that way on
    /// probation until a signed pin set confirms it (sections 5.6 and 5.7); and
    /// the security events of section 7.5.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Decided by WWCP_OCPP_Node's ServerKeyPinning, which a Local Controller
    /// uses for its line up to the CSMS as well; the station gives it its
    /// connections (ConnectionStore), its log, its security events and how a
    /// connection's state is said, and dials.
    /// </para>
    /// <para>
    /// What an ssh:// connection does with the host key of its server, done for
    /// the key of a TLS server: the same probation, the same gate, the same
    /// clock - one registry of probations for both (probations). The signed pin
    /// set itself arrives over the OCPP Device Model, as SetVariables of
    /// TLSClientController (ChargingStation.TLSClient.cs).
    /// </para>
    /// </remarks>
    public abstract partial class OCPPDiallingNode
    {

        #region Data

        // The key pins of the wss:// connections: what their servers showed,
        // what each WebSocket client asks at every handshake, and the probations.
        protected ServerKeyPinning  keyPinning = default!;

        #endregion


        #region (private) NewKeyPinning()

        /// <summary>
        /// The key pins of the connections the store keeps, told to this
        /// station's log and to its CSMS.
        /// </summary>
        protected ServerKeyPinning NewKeyPinning()

            => new (Connections,
                    probations,
                    Notice:         text                       => Log.Notice (text, "ocpp", "connections", "keypins", "security"),
                    Warning:        text                       => Log.Warning(text, "ocpp", "connections", "keypins", "security"),
                    SecurityEvent:  (endpoint, type, techInfo) => KeyPinSecurityEvent(EntryOf(endpoint), type, techInfo),
                    Failed:         (endpoint, status, what)   => Fail(EntryOf(endpoint),
                                                                       status == PinnedConnectionStatus.Trying ? ConnectionStatus.Trying : ConnectionStatus.Lost,
                                                                       what),
                    TimeProvider:   TimeProvider,
                    NodeName:       $"{NodeWording.Noun}");

        /// <summary>
        /// The connection the core speaks of: the store's own copies are
        /// connection entries already.
        /// </summary>
        protected ConnectionEntry EntryOf(IPinnedTLSEndpoint Endpoint)

            => Endpoint as ConnectionEntry
                   ?? Connections.CopyOf(Endpoint.Id)
                   ?? throw new ArgumentException($"'{Endpoint.Id}' is no connection of this {NodeWording.Noun}.", nameof(Endpoint));

        #endregion

        #region (internal) KeyPinTrustOf(ConnectionId)

        /// <summary>
        /// The trust the given connection was dialled with last, where it is
        /// key-pinned: what its server showed, and the validator its WebSocket
        /// client asks.
        /// </summary>
        public ServerKeyPinTrust? KeyPinTrustOf(String ConnectionId)

            => keyPinning.TrustOf(ConnectionId);

        #endregion

        #region (private) KeyPinTrustFor(Connection, out NotDialled)

        /// <summary>
        /// The trust a wss:// connection is dialled with, where it is key-pinned;
        /// null where it is checked by its CA, as it always was.
        /// </summary>
        /// <param name="Connection">The connection.</param>
        /// <param name="NotDialled">Why it is not dialled at all: it requires raw public keys, which this station's TLS stack cannot offer yet.</param>
        protected ServerKeyPinTrust? KeyPinTrustFor(ConnectionEntry  Connection,
                                                  out String?      NotDialled)

            => keyPinning.TrustFor(Connection.Copy(), out NotDialled);

        #endregion

        #region (private) FollowKeyPins(Connection, Client, Trust, DialledURL, Connected)

        /// <summary>
        /// Follow a key-pinned connection beside what Follow says of it: a key
        /// pin learned on the connection made now and on every one after, and a
        /// server not believed when it is tried again.
        /// </summary>
        protected void FollowKeyPins(ConnectionEntry    Connection,
                                   WebSocketClient    Client,
                                   ServerKeyPinTrust  Trust,
                                   URL                DialledURL,
                                   Boolean            Connected)

            => keyPinning.Follow(Connection, Client, Trust, DialledURL, Connected);

        #endregion

        #region (private static) WhatItBelieves(Trust)

        /// <summary>
        /// What a key-pinned connection believes its server by, as the end of a
        /// sentence.
        /// </summary>
        protected static String WhatItBelieves(ServerKeyPinTrust Trust)

            => ServerKeyPinning.WhatItBelieves(Trust);

        #endregion

        #region (internal) TryTakeSignedServerKeyPins(Connection, Pins, Version, Signer, out Error, AfterAnswer = null)

        /// <summary>
        /// Take the key pins of a signed pin set for a wss:// connection: what
        /// ServerKeyPins and ServerKeyPinsVersion of TLSClientController (OCPP
        /// 2.x) or evqi.serverKeyPins (OCPP 1.6) hand over, once a signer this
        /// station believes was found to have signed them ("Key Pins and Raw
        /// Public Keys", sections 5.7, 7.2 and 7.3).
        /// </summary>
        /// <param name="Connection">The connection, by its identification.</param>
        /// <param name="Pins">The key pins of the pin set, separated by commas.</param>
        /// <param name="Version">The version of the pin set.</param>
        /// <param name="Signer">Who signed it, for the log.</param>
        /// <param name="Error">Why the pins were not taken.</param>
        /// <param name="AfterAnswer">Done once the answer to the request that brought the pin set is out: a connection to a server the pins refuse is dropped then, and refused everything until. At once, where nothing is answered.</param>
        public Boolean TryTakeSignedServerKeyPins(String                           Connection,
                                                    String                           Pins,
                                                    UInt64                           Version,
                                                    String                           Signer,
                                                    [NotNullWhen(false)] out String? Error,
                                                    Task?                            AfterAnswer   = null)

            => keyPinning.TryTakeSignedServerKeyPins(Connection, Pins, Version, Signer, out Error, AfterAnswer);

        #endregion

        #region (internal) KeyPinSecurityEvent(Connection, Type, TechInfo)

        /// <summary>
        /// Report a security event of the white paper "Key Pins and Raw Public
        /// Keys" (section 7.5) through the OCPP node of the connection's
        /// version: InvalidCsmsCertificate is InvalidCentralSystemCertificate in
        /// OCPP 1.6, the new ones are the same in both.
        /// </summary>
        public void KeyPinSecurityEvent(ConnectionEntry  Connection,
                                          String           Type,
                                          String           TechInfo)

            => SshSecurityEvent(Connection,
                                Connection.OCPPVersion == OCPPVersion.OCPP1_6 && Type == "InvalidCsmsCertificate"
                                    ? "InvalidCentralSystemCertificate"
                                    : Type,
                                TechInfo);

        #endregion

    }

}
