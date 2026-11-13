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

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SSH;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP.WebSockets;

using WWCPSignatureFormats = cloud.charging.open.protocols.WWCP.SignatureFormats;

using cloud.charging.open.protocols.WWCP.Node.Configuration;

using cloud.charging.open.protocols.OCPP.Node.KeyPins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    /// <summary>
    /// What is at the other end of a connection.
    /// </summary>
    /// <remarks>
    /// A label, and deliberately no more than one. Nothing in this station
    /// reads it to decide anything: every connection is dialled on its own
    /// merits. Whether a spare should wait for its main one to fail is a real
    /// question with a real answer, and it is not answered here - a guess at
    /// it now is something that would have to be unpicked when somebody
    /// decides properly.
    /// </remarks>
    public enum ConnectionType
    {

        /// <summary>
        /// The charging station management system this station reports to.
        /// </summary>
        CSMS,

        /// <summary>
        /// A second management system, for when the first cannot be reached.
        /// </summary>
        CSMSBackup,

        /// <summary>
        /// A local controller, normally in the same installation.
        /// </summary>
        LocalController

    }


    /// <summary>
    /// Which OCPP this station speaks on a connection.
    /// </summary>
    /// <remarks>
    /// Written down rather than worked out, because there is nothing to work
    /// it out from. This station is two nodes - an OCPP 1.6 charge point and
    /// an OCPP 2.1 charging station - and which of them dials is not something
    /// a URL says. The version is negotiated in the WebSocket sub-protocol
    /// during the handshake, and by then the node has already been chosen.
    /// </remarks>
    public enum OCPPVersion
    {

        /// <summary>
        /// OCPP 1.6, still what most of the installed base speaks.
        /// </summary>
        OCPP1_6,

        /// <summary>
        /// OCPP 2.1.
        /// </summary>
        OCPP2_1

    }


    /// <summary>
    /// One place this charging station dials, and what it proves itself with
    /// when it gets there.
    /// </summary>
    /// <remarks>
    /// The credentials are referred to rather than repeated. A station that
    /// reaches its management system twice - once normally and once through a
    /// spare address - proves itself the same way both times, and a password
    /// written down twice is a password that gets changed once.
    ///
    /// At most one way of proving itself, because that is one decision and a
    /// connection that carried two would leave somebody guessing which one
    /// was actually used. None at all is allowed and is what a test back end
    /// on the bench wants; the page says plainly what that means.
    /// </remarks>
    public sealed class ConnectionEntry : IPinnedTLSEndpoint
    {

        #region Data

        /// <summary>
        /// The longest a description may be written.
        /// </summary>
        public const Int32 MaxDescriptionLength  = 120;

        /// <summary>
        /// The longest a URL may be written.
        /// </summary>
        public const Int32 MaxURLLength          = 500;

        /// <summary>
        /// The schemes a charging station may be told to dial.
        /// </summary>
        /// <remarks>
        /// OCPP is a WebSocket protocol, so ws and wss are what this is for.
        /// http and https are here because a URL is frequently written down
        /// that way and means the same endpoint - refusing it would be
        /// pedantry, and taking it silently would hide which one is meant, so
        /// it is taken and said out loud on the page.
        ///
        /// And ssh: the same OCPP over the SSH subsystem
        /// "ocpp@open.charging.cloud", signed in with this station's own key -
        /// "ssh://cs001@csms.example.org:2222".
        /// </remarks>
        public static readonly URIScheme[] AllowedSchemes = [
                                                                URIScheme.ws,
                                                                URIScheme.wss,
                                                                URIScheme.http,
                                                                URIScheme.https,
                                                                URIScheme.ssh
                                                            ];

        /// <summary>
        /// The longest a host key may be written: a public-key line of an RSA
        /// key of 4096 bits, and room to spare.
        /// </summary>
        public const Int32 MaxHostKeyLength      = 1000;

        /// <summary>
        /// The most host keys one connection pins: the current ones of its
        /// server and room for a change, as HostKeyPins of the white paper
        /// "OCPP over SSH" allows.
        /// </summary>
        public const Int32 MaxHostKeys           = 16;

        /// <summary>
        /// How long a host key learned on first use waits for a signed pin set
        /// that confirms it, where the connection does not say: 10 minutes.
        /// </summary>
        public static readonly TimeSpan DefaultSshProbationTime  = TimeSpan.FromMinutes(10);

        /// <summary>
        /// The shortest probation a connection may give a learned host key. A
        /// short one is the stricter one - an unconfirmed key is held for less
        /// time - so it is limited only by what a CSMS can answer in.
        /// </summary>
        public static readonly TimeSpan MinSshProbationTime      = TimeSpan.FromSeconds(1);

        /// <summary>
        /// The longest probation a connection may give a key learned on first
        /// use, a host key or a key pin.
        /// </summary>
        public static readonly TimeSpan MaxProbationTime         = TimeSpan.FromDays(7);

        /// <summary>
        /// The most key pins one wss:// connection holds: the current key of its
        /// server, the announced next one, and room for a transition ("Key Pins
        /// and Raw Public Keys", section 5.5).
        /// </summary>
        public const Int32 MaxServerKeyPins      = KeyPinPolicy.MaxPins;

        /// <summary>
        /// How long a key pin learned on first use waits for a signed pin set
        /// that confirms it, where the connection does not say: 10 minutes, as
        /// for a host key.
        /// </summary>
        public static readonly TimeSpan DefaultServerKeyPinProbationTime  = TimeSpan.FromMinutes(10);

        /// <summary>
        /// What the servers of the OCPP connections are to the node, where it
        /// judges their certificates: the usage a TLS root of the store is
        /// kept for, and the mark of a group of roots - "ocppca-&lt;group&gt;".
        /// </summary>
        public const String PinsService          = "ocpp";

        /// <summary>
        /// Where a connection says what its server is held to, in the file and
        /// as a page sends it: under the keys every server of this node is
        /// held to something with - see <see cref="ServerPins"/>.
        /// </summary>
        public const String PinsKey              = "pins";

        /// <summary>
        /// The key the version of HTTP the WebSocket of a ws:// or wss://
        /// connection is opened over stands under: "auto", "1.1", "2" or "3".
        /// </summary>
        public const String HTTPVersionKey       = "httpVersion";

        /// <summary>
        /// The key the way the messages of a connection are written stands
        /// under: "json", "cbor" or "auto".
        /// </summary>
        public const String EncodingKey          = "encoding";

        /// <summary>
        /// The key the signatures a connection writes stand under: "jws",
        /// "cose", "both" - or "policy", which is saying nothing.
        /// </summary>
        public const String SignatureFormatsKey  = "signatureFormats";

        /// <summary>
        /// What a connection says where it leaves its signatures to the policy.
        /// </summary>
        public const String SignaturesAsThePolicySays = "policy";

        /// <summary>
        /// Where a page says what it showed the server held to.
        /// </summary>
        public const String PinsAsShownKey       = ServerPins.AsShownKey;

        #endregion

        #region Properties

        /// <summary>
        /// What this connection is called here.
        /// </summary>
        public String          Id                   { get; }

        /// <summary>
        /// What somebody wrote down that it is, e.g. "CSMS, main".
        /// </summary>
        public String          Description          { get; internal set; }

        /// <summary>
        /// Where it goes.
        /// </summary>
        public URL             URL                  { get; internal set; }

        /// <summary>
        /// What is at the other end.
        /// </summary>
        public ConnectionType  ConnectionType       { get; internal set; }

        /// <summary>
        /// Which OCPP this station speaks here, and so which of its two nodes
        /// does the dialling.
        /// </summary>
        public OCPPVersion     OCPPVersion          { get; internal set; }

        /// <summary>
        /// Whether this station connects here by itself - at start, and again
        /// after the connection drops.
        /// </summary>
        /// <remarks>
        /// This is what decides whether a connection is used at all. Off, it
        /// stays written down and nothing happens: an address kept ready for a
        /// day somebody needs it, which can still be tried by hand from the
        /// page. On, the station connects when it starts and keeps coming
        /// back.
        ///
        /// Off by default, which is the quieter of the two wrong answers. A
        /// station that was never told to connect is noticed by whoever
        /// configured it; a station that connects somewhere nobody meant it to
        /// is noticed by whatever is at the other end, later.
        /// </remarks>
        public Boolean         AutoConnect          { get; internal set; }

        /// <summary>
        /// The credentials this connection proves itself with, by their
        /// identification - or null.
        /// </summary>
        public String?         AuthenticationId     { get; internal set; }

        /// <summary>
        /// The client certificate this connection proves itself with, by its
        /// identification - or null.
        /// </summary>
        public String?         CertificateId        { get; internal set; }

        /// <summary>
        /// The host keys the server of an ssh:// connection may show, separated
        /// by commas: SHA-256 fingerprints ("SHA256:...") or public-key lines,
        /// at most <see cref="MaxHostKeys"/> - or null for none.
        /// </summary>
        /// <remarks>
        /// A set, so that a server that is about to change its host key can be
        /// pinned with both for the time of the change. Without one, the
        /// connection is dialled only where it trusts on first use (see
        /// <see cref="SshTrustOnFirstUse"/>). Written as HostKeyPins of the white
        /// paper "OCPP over SSH" is.
        /// </remarks>
        public String?         SshHostKey           { get; internal set; }

        /// <summary>
        /// The host keys pinned, one by one; none where nothing is.
        /// </summary>
        public IReadOnlyList<String>  SshHostKeys

            => SplitHostKeys(SshHostKey);

        /// <summary>
        /// Whether an ssh:// connection that pins no host key learns the one
        /// its server shows at the first handshake - on probation until a signed
        /// pin set confirms it. Never, unless it says so.
        /// </summary>
        public SshTrustOnFirstUse  SshTrustOnFirstUse   { get; internal set; }

        /// <summary>
        /// What a host key that matches none of the pins leads to: refused,
        /// unless the connection says to record or accept it.
        /// </summary>
        public SshOnPinMismatch    SshOnPinMismatch     { get; internal set; }

        /// <summary>
        /// How long a host key learned on first use waits for a signed pin set
        /// that confirms it.
        /// </summary>
        public TimeSpan        SshProbationTime     { get; internal set; } = DefaultSshProbationTime;

        /// <summary>
        /// The version of the signed pin set the host keys came from; 0 where
        /// none did. A signed pin set is taken only with a higher one, so that
        /// an older one cannot be brought back.
        /// </summary>
        public UInt64          SshHostKeysVersion   { get; internal set; }

        /// <summary>
        /// The SHA-256 fingerprint of the host key learned on first use, while
        /// it is on probation - or null.
        /// </summary>
        public String?         SshLearnedHostKey    { get; internal set; }

        /// <summary>
        /// When <see cref="SshLearnedHostKey"/> was learned.
        /// </summary>
        public DateTimeOffset? SshLearnedAt         { get; internal set; }

        /// <summary>
        /// The key pins of the keys the server of a wss:// connection may show,
        /// at most <see cref="MaxServerKeyPins"/>; none where nothing is pinned.
        /// </summary>
        /// <remarks>
        /// SHA-256 over the DER encoded SubjectPublicKeyInfo of a key, as a DANE
        /// record "3 1 1" holds it ("Key Pins and Raw Public Keys", section
        /// 5.1). Set, they are the whole check of the server: no chain, no name,
        /// no validity period - which is what lets a station believe its Local
        /// Controller, which no CA issues a certificate for, and before its
        /// clock is set. A set, so that a server about to change its key can be
        /// pinned with both for the time of the change.
        /// </remarks>
        public IReadOnlyList<KeyPin>  ServerKeyPins            { get; internal set; } = [];

        /// <summary>
        /// The version of the signed pin set the key pins came from; 0 where
        /// none did. A signed pin set is taken only with a higher one, so that
        /// an older one cannot be brought back.
        /// </summary>
        public UInt64                 ServerKeyPinsVersion     { get; internal set; }

        /// <summary>
        /// Whether the key pins are those of a signed pin set that confirmed a
        /// key pin learned on first use.
        /// </summary>
        public Boolean                ServerKeyPinsConfirmed   { get; internal set; }

        /// <summary>
        /// Whether a key-pinned wss:// connection offers raw public keys (RFC
        /// 7250). Off unless it says so.
        /// </summary>
        /// <remarks>
        /// Prefer offers them, and takes the key of a server that knows none
        /// in its certificate, checked by the same pins. Require offers them
        /// alone: a server that shows a certificate is turned away, whatever
        /// key is in it. The key pin rules do not ask how the key came.
        /// </remarks>
        public RawPublicKeyMode       RawPublicKeys            { get; internal set; }

        /// <summary>
        /// Whether a wss:// connection that pins no key learns the key pin of the
        /// one its server shows at the first handshake - on probation until a
        /// signed pin set confirms it. Never, unless it says so.
        /// </summary>
        public KeyTrustOnFirstUse     TrustOnFirstUse          { get; internal set; }

        /// <summary>
        /// The key pin learned on first use, while it is on probation - or null.
        /// </summary>
        public KeyPin?                ServerKeyPinLearned      { get; internal set; }

        /// <summary>
        /// When <see cref="ServerKeyPinLearned"/> was learned.
        /// </summary>
        public DateTimeOffset?        ServerKeyPinLearnedAt    { get; internal set; }

        /// <summary>
        /// How long a key pin learned on first use waits for a signed pin set
        /// that confirms it.
        /// </summary>
        public TimeSpan               ServerKeyPinProbationTime { get; internal set; } = DefaultServerKeyPinProbationTime;

        /// <summary>
        /// Where the key pins come from: configured - by somebody, or by a
        /// signed pin set -, learned on first use and on probation, or learned
        /// and confirmed since.
        /// </summary>
        public KeyPinState            ServerKeyPinState

            => ServerKeyPins.Count == 0 && ServerKeyPinLearned is not null
                   ? KeyPinState.Learned
                   : ServerKeyPins.Count > 0 && ServerKeyPinsConfirmed
                         ? KeyPinState.Confirmed
                         : KeyPinState.Configured;

        /// <summary>
        /// Whether the server of this connection is known by the key pin of its
        /// key rather than by a CA: a wss:// connection with key pins, or one
        /// that trusts on first use ("Key Pins and Raw Public Keys", section
        /// 5.4).
        /// </summary>
        /// <remarks>
        /// One or the other, never both, and never whichever fits: a key-pinned
        /// connection checks no chain, no name and no validity period, and does
        /// not fall back to the CA where the key matches no pin. One that is not
        /// key-pinned is checked by the CA as it always was.
        /// </remarks>
        public Boolean                IsKeyPinned

            => IsSecure &&
               (ServerKeyPins.Count > 0 || TrustOnFirstUse == KeyTrustOnFirstUse.Key);

        /// <summary>
        /// What the server of a wss:// connection checked by its CA is held to
        /// beside that: the certificates it may show, the roots its chain may
        /// end at - each by the fingerprint of the certificate or by the pin of
        /// its key -, the group of roots that may vouch for it at all, what a
        /// mismatch comes to, and what is learned on first use. Null for
        /// nothing.
        /// </summary>
        /// <remarks>
        /// What a time server, a name server and a meter of this station are
        /// held to, under the same names and judged by the same judgement: a
        /// pin narrows what the CA check believes and never widens it, and a
        /// CA of the operator's own comes in through a group of roots, not
        /// through a pin.
        ///
        /// Never beside key pins: a connection is checked by its CA or known
        /// by its key - see <see cref="CheckOneWay"/>.
        /// </remarks>
        public ServerPins?            Pins                     { get; internal set; }

        /// <summary>
        /// The version of the signed pin set the roots came from; 0 where none
        /// did. A signed pin set is taken only with a higher one.
        /// </summary>
        public UInt64                 PinsVersion              { get; internal set; }

        /// <summary>
        /// The version of HTTP the WebSocket of this connection is opened
        /// over: one of them and nothing else, or whichever the server chooses.
        /// </summary>
        /// <remarks>
        /// "auto", and what a connection that says nothing gets: with TLS the
        /// station offers HTTP/2 and HTTP/1.1 and the server chooses; without,
        /// there is nothing to agree with, and it is HTTP/1.1. "1.1" opens the
        /// WebSocket by the Upgrade of HTTP/1.1 (RFC 6455), "2" by the extended
        /// CONNECT of HTTP/2 (RFC 8441) - and a server that does not take that
        /// is not connected to some other way. Who the station is, what it
        /// shows for it and what its server is held to are the same over all
        /// of them. Not said of an ssh:// connection, which has no HTTP.
        /// </remarks>
        public WebSocketHTTPVersion   HTTPVersion              { get; internal set; } = WebSocketHTTPVersion.Auto;

        /// <summary>
        /// How the OCPP messages of this connection are written: JSON in text
        /// frames, as OCPP defines it, or CBOR in binary frames.
        /// </summary>
        /// <remarks>
        /// "json", and what a connection that says nothing gets. "cbor"
        /// offers the CBOR subprotocol of its OCPP version and nothing else
        /// ("ocpp2.1cbor") - a back end that does not speak it is not
        /// connected to some other way. "auto" offers the CBOR subprotocol
        /// first and the JSON ones after it, and takes what the back end
        /// chooses. The same over a WebSocket and over ssh://, which agree on
        /// the same subprotocols, and for OCPP 1.6 ("ocpp1.6cbor") as for OCPP 2.1.
        /// </remarks>
        public MessageEncodings       Encoding                 { get; internal set; } = MessageEncodings.JSON;

        /// <summary>
        /// Which signatures the messages this station signs for this connection
        /// get: the JWS over their JSON, the COSE_Sign1 over their CBOR, or
        /// both - or null, where the signature policy of the station decides.
        /// </summary>
        /// <remarks>
        /// What a message is signed with is a matter of who reads it, and so of
        /// the connection: a back end that verifies COSE signatures gets them,
        /// another one beside it the JWS. It says nothing of what the station
        /// verifies - every signature it can read. A COSE signature signs the
        /// CBOR of a message whatever the connection carries, JSON or CBOR.
        /// </remarks>
        public WWCPSignatureFormats?  SignatureFormats         { get; internal set; }

        /// <summary>
        /// When this was written down.
        /// </summary>
        public DateTimeOffset  CreatedAt            { get; }

        /// <summary>
        /// Whether the connection is made over TLS, which decides what a TLS
        /// client certificate and a channel-bound password can mean here.
        /// </summary>
        public Boolean         IsSecure

            => URL.Scheme == URIScheme.wss ||
               URL.Scheme == URIScheme.https;

        /// <summary>
        /// Whether the connection is made over SSH rather than as an HTTP
        /// WebSocket - encrypted, and the server known by its host key rather
        /// than by a TLS certificate.
        /// </summary>
        public Boolean         IsSSH

            => URL.Scheme == URIScheme.ssh;

        /// <summary>
        /// What is worth saying about this connection that is not an error:
        /// something configured that cannot do what it looks like it does.
        /// </summary>
        /// <remarks>
        /// Worked out rather than remembered, because most of it depends on
        /// something other than this record - whether the credentials it
        /// names still exist, and whether the URL it was given can carry them.
        /// </remarks>
        public List<String>    Warnings             { get; } = [];

        #endregion

        #region Constructor(s)

        /// <summary>
        /// One connection.
        /// </summary>
        public ConnectionEntry(String          Id,
                               String          Description,
                               URL             URL,
                               ConnectionType  ConnectionType,
                               DateTimeOffset  CreatedAt)
        {

            this.Id              = Id;
            this.Description     = Description;
            this.URL             = URL;
            this.ConnectionType  = ConnectionType;
            this.CreatedAt       = CreatedAt;

        }

        #endregion


        #region (static) Named(Text)

        /// <summary>
        /// The text with the spaces taken off, or null when there was nothing
        /// but spaces.
        /// </summary>
        /// <remarks>
        /// "No credentials" and "credentials whose identification is the empty
        /// string" are the same thing arriving from a form, and only one of
        /// them is worth storing.
        /// </remarks>
        public static String? Named(String? Text)
        {
            var text = Text?.Trim();
            return String.IsNullOrEmpty(text) ? null : text;
        }

        #endregion

        #region (static) TryParseVersion(Text, out Version)

        /// <summary>
        /// Which OCPP, written the way the API writes it.
        /// </summary>
        public static Boolean TryParseVersion(String? Text, out OCPPVersion Version)
        {

            switch (Text?.Trim().ToLowerInvariant().Replace(".", "").Replace("_", ""))
            {

                case "ocpp16":
                case "16":
                    Version = OCPPVersion.OCPP1_6;
                    return true;

                case "ocpp21":
                case "21":
                    Version = OCPPVersion.OCPP2_1;
                    return true;

                default:
                    Version = OCPPVersion.OCPP2_1;
                    return false;

            }

        }

        /// <summary>
        /// How a version is written in the API and in the file.
        /// </summary>
        public static String AsText(OCPPVersion Version)

            => Version == OCPPVersion.OCPP1_6
                   ? "OCPP1.6"
                   : "OCPP2.1";

        #endregion

        #region (static) TryParseType(Text, out Type)

        /// <summary>
        /// What is at the other end, written the way the API writes it.
        /// </summary>
        public static Boolean TryParseType(String? Text, out ConnectionType Type)
        {

            switch (Text?.Trim().ToLowerInvariant())
            {

                case "csms":
                    Type = ConnectionType.CSMS;
                    return true;

                case "csmsbackup":
                    Type = ConnectionType.CSMSBackup;
                    return true;

                case "localcontroller":
                    Type = ConnectionType.LocalController;
                    return true;

                default:
                    Type = ConnectionType.CSMS;
                    return false;

            }

        }

        #endregion

        #region (static) Validate(Description, URLText, TypeText, out URL, out Type, out Error)

        /// <summary>
        /// Everything that can be said to be wrong with a connection before it
        /// is written down, in the words somebody typing it should read.
        /// </summary>
        /// <remarks>
        /// One place rather than two, so that adding one and changing one
        /// cannot come to disagree about what is allowed.
        ///
        /// Whether the credentials it names exist is not asked here: this
        /// record does not know what else is configured, and the store that
        /// does asks that question.
        /// </remarks>
        public static Boolean Validate(String?                          Description,
                                       String?                          URLText,
                                       String?                          TypeText,
                                       String?                          VersionText,
                                       out URL                          URL,
                                       out ConnectionType               Type,
                                       out OCPPVersion                  Version,
                                       [NotNullWhen(false)] out String? Error)
        {

            URL      = default;
            Type     = ConnectionType.CSMS;
            Version  = OCPPVersion.OCPP2_1;
            Error    = null;

            var description = (Description ?? "").Trim();
            var urlText     = (URLText     ?? "").Trim();

            if (description.Length == 0)
            {
                Error = "Say what this connection is - that is what tells it from the others.";
                return false;
            }

            if (description.Length > MaxDescriptionLength)
            {
                Error = $"A description may be at most {MaxDescriptionLength} characters.";
                return false;
            }

            if (urlText.Length == 0)
            {
                Error = "A connection needs somewhere to go.";
                return false;
            }

            if (urlText.Length > MaxURLLength)
            {
                Error = $"A URL may be at most {MaxURLLength} characters.";
                return false;
            }

            // Insisted on before parsing, because Hermod's parser puts
            // "https://" in front of anything that has no scheme. Everywhere
            // else that is a convenience; here the scheme is the whole
            // question of whether there is TLS at all, and everything this
            // station says about a connection - whether a client certificate
            // can be shown, whether a password goes out in the clear - is read
            // off it. A guessed scheme would have the station quietly claiming
            // to be secure because somebody left four characters out.
            if (!urlText.Contains("://", StringComparison.Ordinal))
            {
                Error = "The URL says no scheme - write ws://, wss:// or ssh:// in front of it. " +
                        "Which one it is decides whether this connection is encrypted, so it is not guessed at here.";
                return false;
            }

            if (!org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.TryParse(urlText, out var url))
            {
                Error = $"'{urlText}' cannot be read as a URL.";
                return false;
            }

            if (url.Scheme is null)
            {
                Error = "The URL says no scheme - write ws://, wss:// or ssh:// in front of it.";
                return false;
            }

            if (!AllowedSchemes.Contains(url.Scheme))
            {
                Error = $"'{url.Scheme}' is not something this station dials. OCPP goes over ws, wss or ssh.";
                return false;
            }

            if (!TryParseType(TypeText, out var type))
            {
                Error = $"'{TypeText}' is not a kind of connection this station knows.";
                return false;
            }

            // Left out, it is the newer one. A station configured by somebody
            // who did not think about it should not quietly be speaking the
            // older protocol.
            if (!String.IsNullOrWhiteSpace(VersionText) && !TryParseVersion(VersionText, out Version))
            {
                Error = $"'{VersionText}' is not an OCPP version this station speaks. It speaks OCPP1.6 and OCPP2.1.";
                return false;
            }

            URL   = url;
            Type  = type;

            return true;

        }

        #endregion

        #region (static) SplitHostKeys(Text)

        /// <summary>
        /// The host keys of a set as it is written: separated by commas or line
        /// breaks, trimmed, the empty ones left out.
        /// </summary>
        public static IReadOnlyList<String> SplitHostKeys(String? Text)

            => (Text ?? "").Split([ ',', '\n', '\r' ], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        #endregion

        #region (static) ValidateHostKey(URL, Text, out HostKey, out Error, MD5Kept = false)

        /// <summary>
        /// The host keys a connection pins, as they are written down - or the
        /// one sentence that says what is wrong with them.
        /// </summary>
        /// <remarks>
        /// Read as the SSH client reads them, so that what is taken here is what
        /// is checked when the station dials: SHA-256 fingerprints, or
        /// public-key lines, at most <see cref="MaxHostKeys"/> of them. An MD5
        /// fingerprint is refused with the way to its SHA-256 one (white paper
        /// "OCPP over SSH", decision 3); one already in the file is kept, so
        /// that the connection is not lost, and it is not dialled until it is
        /// replaced (see <see cref="PinsMD5"/>). Only for ssh://: the server of
        /// a WebSocket is known by its TLS certificate, and a host key written
        /// down beside it would look like a check nothing makes.
        /// </remarks>
        /// <param name="URL">Where the connection goes.</param>
        /// <param name="Text">The host keys as written, separated by commas; nothing for none.</param>
        /// <param name="HostKey">The host keys, trimmed and separated by ", "; null for none.</param>
        /// <param name="Error">What is wrong with them.</param>
        /// <param name="MD5Kept">Whether an MD5 fingerprint is kept rather than refused: as read from the file.</param>
        public static Boolean ValidateHostKey(URL                              URL,
                                              String?                          Text,
                                              out String?                      HostKey,
                                              [NotNullWhen(false)] out String? Error,
                                              Boolean                          MD5Kept   = false)
        {

            var keys  = SplitHostKeys(Text);

            HostKey   = null;
            Error     = null;

            if (keys.Count == 0)
                return true;

            if (URL.Scheme != URIScheme.ssh)
            {
                Error    = "A host key is what an ssh:// connection checks its server by. This one goes over a WebSocket, " +
                           "whose server is known by its TLS certificate.";
                return false;
            }

            if (keys.Count > MaxHostKeys)
            {
                Error    = $"A connection pins at most {MaxHostKeys} host keys, and these are {keys.Count}.";
                return false;
            }

            foreach (var key in keys)
            {

                if (key.Length > MaxHostKeyLength)
                {
                    Error    = $"A host key may be written in at most {MaxHostKeyLength} characters.";
                    return false;
                }

                if (!MD5Kept && key.StartsWith("MD5:", StringComparison.OrdinalIgnoreCase))
                {
                    Error    = $"'{key}' is an MD5 fingerprint, and MD5 fingerprints are no longer taken: write its SHA-256 one, " +
                               "which 'ssh-keygen -l -f' prints for the server's public host key, or the public-key line itself.";
                    return false;
                }

                try
                {
                    HostKeyPolicy.Pin(key);
                }
                catch (ArgumentException)
                {
                    Error    = $"'{key}' is neither a fingerprint (SHA256:...) nor a public-key line (ssh-ed25519 AAAA...).";
                    return false;
                }

            }

            HostKey = String.Join(", ", keys.Distinct(StringComparer.Ordinal));

            return true;

        }

        /// <summary>
        /// Whether one of the host keys pinned is an MD5 fingerprint, which is
        /// no longer taken: one kept from the file, which is not dialled with.
        /// </summary>
        public Boolean PinsMD5

            => SshHostKeys.Any(key => key.StartsWith("MD5:", StringComparison.OrdinalIgnoreCase));

        #endregion

        #region (static) TryParseTrustOnFirstUse(Text, out Value) / TryParseOnPinMismatch(Text, out Value)

        /// <summary>
        /// "None" or "HostKey", in any case; nothing for "None".
        /// </summary>
        public static Boolean TryParseTrustOnFirstUse(String? Text, out SshTrustOnFirstUse Value)
        {

            Value = SshTrustOnFirstUse.None;

            if (String.IsNullOrWhiteSpace(Text))
                return true;

            return Enum.TryParse(Text.Trim(), ignoreCase: true, out Value) && Enum.IsDefined(Value);

        }

        /// <summary>
        /// "Refuse", "Record" or "Accept", in any case; nothing for "Refuse".
        /// </summary>
        public static Boolean TryParseOnPinMismatch(String? Text, out SshOnPinMismatch Value)
        {

            Value = SshOnPinMismatch.Refuse;

            if (String.IsNullOrWhiteSpace(Text))
                return true;

            return Enum.TryParse(Text.Trim(), ignoreCase: true, out Value) && Enum.IsDefined(Value);

        }

        #endregion

        #region (static) ValidateSshTrust(TrustText, MismatchText, ProbationSeconds, out Trust, out Mismatch, out Probation, out Error)

        /// <summary>
        /// How an ssh:// connection believes its server where nothing or
        /// something else is pinned, as it is written - or the one sentence that
        /// says what is wrong with it.
        /// </summary>
        public static Boolean ValidateSshTrust(String?                          TrustText,
                                               String?                          MismatchText,
                                               Int64?                           ProbationSeconds,
                                               out SshTrustOnFirstUse           Trust,
                                               out SshOnPinMismatch             Mismatch,
                                               out TimeSpan                     Probation,
                                               [NotNullWhen(false)] out String? Error)
        {

            Mismatch   = SshOnPinMismatch.Refuse;
            Probation  = DefaultSshProbationTime;
            Error      = null;

            if (!TryParseTrustOnFirstUse(TrustText, out Trust))
            {
                Error = $"'{TrustText}' says nothing of trust on first use: it is 'None' or 'HostKey'.";
                return false;
            }

            if (!TryParseOnPinMismatch(MismatchText, out Mismatch))
            {
                Error = $"'{MismatchText}' says nothing of a host key that matches no pin: it is 'Refuse', 'Record' or 'Accept'.";
                return false;
            }

            if (ProbationSeconds is Int64 seconds)
            {

                if (seconds < MinSshProbationTime.TotalSeconds || seconds > TimeSpan.FromDays(7).TotalSeconds)
                {
                    Error = $"A probation of {seconds} s is not one: it lasts from {MinSshProbationTime.TotalSeconds:0} s to 7 days.";
                    return false;
                }

                Probation = TimeSpan.FromSeconds(seconds);

            }

            return true;

        }

        #endregion

        #region (static) ValidateServerKeyPins(URL, Text, out Pins, out Error)

        /// <summary>
        /// The key pins a wss:// connection pins, as they are written down - or
        /// the one sentence that says what is wrong with them.
        /// </summary>
        /// <remarks>
        /// Separated by commas, spaces or line breaks; each one 64 hexadecimal
        /// digits, with or without colons between the bytes, or curl's
        /// "sha256//" and base64 - read as KeyPin reads them, and kept as 64
        /// lowercase hexadecimal digits. At most <see cref="MaxServerKeyPins"/>.
        /// Only for wss:// and https://: a ws:// connection makes no TLS
        /// handshake whose key could be checked, and the server of an ssh:// one
        /// is known by its host key - a key pin written down beside either
        /// would look like a check nothing makes.
        /// </remarks>
        /// <param name="URL">Where the connection goes.</param>
        /// <param name="Text">The key pins as written; nothing for none.</param>
        /// <param name="Pins">The key pins, each once, in the order written.</param>
        /// <param name="Error">What is wrong with them.</param>
        public static Boolean ValidateServerKeyPins(URL                              URL,
                                                    String?                          Text,
                                                    out IReadOnlyList<KeyPin>        Pins,
                                                    [NotNullWhen(false)] out String? Error)
        {

            Pins   = [];
            Error  = null;

            var written = SplitServerKeyPins(Text);

            if (written.Count == 0)
                return true;

            if (URL.Scheme != URIScheme.wss && URL.Scheme != URIScheme.https)
            {
                Error = URL.Scheme == URIScheme.ssh
                            ? "A key pin is what a wss:// connection checks the key of its server by. This one goes over SSH, " +
                              "whose server is known by its host key."
                            : "A key pin is what a wss:// connection checks the key of its server by. This one makes no TLS " +
                              "handshake, so there is no key to check.";
                return false;
            }

            var pins = new List<KeyPin>();

            foreach (var one in written)
            {

                if (!KeyPin.TryParse(one, out var pin))
                {
                    Error = $"'{one}' is not a key pin: that is 64 hexadecimal digits - SHA-256 over the key's SubjectPublicKeyInfo, " +
                             "with or without colons - or 'sha256//' and the base64 of the same.";
                    return false;
                }

                if (!pins.Contains(pin))
                    pins.Add(pin);

            }

            if (pins.Count > MaxServerKeyPins)
            {
                Error = $"A connection pins at most {MaxServerKeyPins} keys - the current one, the next one and room for a change - " +
                        $"and these are {pins.Count}.";
                return false;
            }

            Pins = pins;
            return true;

        }

        /// <summary>
        /// The key pins of a set as it is written: separated by commas,
        /// semicolons, spaces or line breaks, the empty ones left out.
        /// </summary>
        public static IReadOnlyList<String> SplitServerKeyPins(String? Text)

            => (Text ?? "").Split([ ',', ';', ' ', '\t', '\n', '\r' ], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        /// <summary>
        /// The key pins as one text, as the page and the log write them.
        /// </summary>
        public static String AsText(IEnumerable<KeyPin> Pins)

            => KeyPinText.Of(Pins);

        #endregion

        #region (static) ValidateKeyPinTrust(URL, RawText, TrustText, ProbationSeconds, out Raw, out Trust, out Probation, out Error)

        /// <summary>
        /// Whether a wss:// connection offers raw public keys, whether it learns
        /// the key pin of the first key shown, and how long that waits for a
        /// signed pin set - or the one sentence that says what is wrong.
        /// </summary>
        /// <remarks>
        /// "Off" and "None" are what every connection says, and are taken of
        /// every URL; anything else only of a wss:// or https:// one. Whether
        /// raw public keys go with the key pins there are is asked by
        /// <see cref="CheckKeyPinned"/>, once those are known.
        /// </remarks>
        public static Boolean ValidateKeyPinTrust(URL                              URL,
                                                  String?                          RawText,
                                                  String?                          TrustText,
                                                  Int64?                           ProbationSeconds,
                                                  out RawPublicKeyMode             Raw,
                                                  out KeyTrustOnFirstUse           Trust,
                                                  out TimeSpan                     Probation,
                                                  [NotNullWhen(false)] out String? Error)
        {

            Raw        = RawPublicKeyMode.Off;
            Trust      = KeyTrustOnFirstUse.None;
            Probation  = DefaultServerKeyPinProbationTime;
            Error      = null;

            if (!String.IsNullOrWhiteSpace(RawText) &&
                !(Enum.TryParse(RawText.Trim(), ignoreCase: true, out Raw) && Enum.IsDefined(Raw)))
            {
                Raw    = RawPublicKeyMode.Off;
                Error  = $"'{RawText}' says nothing of raw public keys: it is 'Off', 'Prefer' or 'Require'.";
                return false;
            }

            if (!String.IsNullOrWhiteSpace(TrustText) &&
                !(Enum.TryParse(TrustText.Trim(), ignoreCase: true, out Trust) && Enum.IsDefined(Trust)))
            {
                Trust  = KeyTrustOnFirstUse.None;
                Error  = $"'{TrustText}' says nothing of trust on first use: it is 'None' or 'Key'.";
                return false;
            }

            var tls = URL.Scheme == URIScheme.wss || URL.Scheme == URIScheme.https;

            if (!tls && Raw != RawPublicKeyMode.Off)
            {
                Error  = "Raw public keys are a way of a TLS handshake, and this connection makes none.";
                return false;
            }

            if (!tls && Trust != KeyTrustOnFirstUse.None)
            {
                Error  = URL.Scheme == URIScheme.ssh
                             ? "Trust on first use of a key pin is for a wss:// connection; one over SSH learns a host key, as its own field says."
                             : "Trust on first use of a key pin is for a wss:// connection, and this one makes no TLS handshake to learn a key from.";
                return false;
            }

            if (ProbationSeconds is Int64 seconds)
            {

                if (seconds < MinSshProbationTime.TotalSeconds || seconds > MaxProbationTime.TotalSeconds)
                {
                    Error = $"A probation of {seconds} s is not one: it lasts from {MinSshProbationTime.TotalSeconds:0} s to 7 days.";
                    return false;
                }

                Probation = TimeSpan.FromSeconds(seconds);

            }

            return true;

        }

        /// <summary>
        /// What the entry of a connection says its server is held to, under
        /// <see cref="PinsKey"/> - or the one sentence that says what is wrong.
        /// </summary>
        /// <param name="URL">Where the connection goes.</param>
        /// <param name="JSON">What stands under "pins", or under "pinsAsShown"; null or JSON null for nothing.</param>
        /// <param name="Path">Where it is, to say which one is wrong.</param>
        /// <param name="Pins">What the server is held to; null for nothing.</param>
        /// <param name="Error">What is wrong.</param>
        public static Boolean ValidatePins(URL                              URL,
                                           JToken?                          JSON,
                                           String                           Path,
                                           out ServerPins?                  Pins,
                                           [NotNullWhen(false)] out String? Error)
        {

            Pins   = null;
            Error  = null;

            if (JSON is null || JSON.Type == JTokenType.Null)
                return true;

            if (JSON is not JObject entry)
            {
                Error = $"'{Path}' is no object: it holds what the server is held to under 'certificateFingerprints', 'rootFingerprints', 'rootGroup', 'onMismatch' and 'trustOnFirstUse'.";
                return false;
            }

            if (!ServerPins.TryParse(entry, Path, out Pins, out Error))
                return false;

            if (Pins is not null && URL.Scheme != URIScheme.wss && URL.Scheme != URIScheme.https)
            {
                Pins   = null;
                Error  = $"'{URL}' makes no TLS handshake, so its server shows no certificate to hold it to.";
                return false;
            }

            return true;

        }

        /// <summary>
        /// One way or the other: a connection whose server is known by its key
        /// is not held to a certificate, a root or a group of roots as well -
        /// its key is the whole check, and nothing a CA says is asked.
        /// </summary>
        public static Boolean CheckOneWay(IReadOnlyList<KeyPin>            KeyPins,
                                          KeyTrustOnFirstUse               Trust,
                                          ServerPins?                      Pins,
                                          [NotNullWhen(false)] out String? Error)
        {

            Error = null;

            if (Pins is not null && Pins.SaysAnything && (KeyPins.Count > 0 || Trust == KeyTrustOnFirstUse.Key))
            {
                Error = "A connection is checked by its CA or known by the key of its server, not both: one with key pins, or learning " +
                        "the key on first use, checks no chain, so a certificate, a root or a group of roots it is held to would never be asked. " +
                        "Take the key pins away, or what it is held to.";
                return false;
            }

            return true;

        }

        /// <summary>
        /// Raw public keys only where the connection is key-pinned: by key pins,
        /// or by trust on first use ("Key Pins and Raw Public Keys", section
        /// 7.2). A connection checked by its CA never accepts a raw key.
        /// </summary>
        public static Boolean CheckKeyPinned(IReadOnlyList<KeyPin>            Pins,
                                             RawPublicKeyMode                 Raw,
                                             KeyTrustOnFirstUse               Trust,
                                             [NotNullWhen(false)] out String? Error)
        {

            Error = null;

            if (Raw != RawPublicKeyMode.Off && Pins.Count == 0 && Trust == KeyTrustOnFirstUse.None)
            {
                Error = "Raw public keys are offered only where the server is known by its key: pin its key, or let it learn the " +
                        "first one on probation. A connection checked by its CA never accepts a raw key.";
                return false;
            }

            return true;

        }

        #endregion

        #region (static) WrittenAt(Token)

        /// <summary>
        /// The time written in a file, or null where there is none to read.
        /// </summary>
        /// <remarks>
        /// Read in no culture: a file parsed by Newtonsoft has its timestamps
        /// as dates already, and one of those asked for as text comes back
        /// month first, which the culture of the machine reads day first -
        /// the 13th of a month as no date at all, and the 3rd as the month.
        /// </remarks>
        public static DateTimeOffset? WrittenAt(JToken? Token)

            => Token is JValue { Value: not null } value
                   ? value.Type == JTokenType.Date
                         ? value.Value is DateTimeOffset withOffset
                               ? withOffset.ToUniversalTime()
                               : new DateTimeOffset(value.Value<DateTime>().ToUniversalTime())
                         : DateTimeOffset.TryParse(value.Value<String>(),
                                                   System.Globalization.CultureInfo.InvariantCulture,
                                                   System.Globalization.DateTimeStyles.AssumeUniversal |
                                                   System.Globalization.DateTimeStyles.AdjustToUniversal,
                                                   out var written)
                               ? written
                               : null
                   : null;

        #endregion

        #region (static) TryParse(JSON, out Entry, out Error)

        /// <summary>
        /// One connection as it stands in the file, or the one sentence that
        /// says what is wrong with it.
        /// </summary>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out ConnectionEntry?  Entry,
                                       [NotNullWhen(false)] out String?           Error)
        {

            Entry = null;

            var id = JSON.Value<String>("id")?.Trim();

            if (String.IsNullOrEmpty(id))
            {
                Error = "A connection without an identification.";
                return false;
            }

            if (!Validate(JSON.Value<String>("description"),
                          JSON.Value<String>("url"),
                          JSON.Value<String>("connectionType"),
                          JSON.Value<String>("ocppVersion"),
                          out var url,
                          out var type,
                          out var version,
                          out Error))
            {
                return false;
            }

            if (!ValidateHostKey(url, JSON.Value<String>("sshHostKey"), out var hostKey, out Error, MD5Kept: true))
                return false;

            if (!ValidateSshTrust(JSON.Value<String>("sshTrustOnFirstUse"),
                                  JSON.Value<String>("sshOnPinMismatch"),
                                  JSON.Value<Int64?>("sshProbationTime"),
                                  out var trustOnFirstUse,
                                  out var onPinMismatch,
                                  out var probationTime,
                                  out Error))
                return false;

            var learnedAt = WrittenAt(JSON["sshLearnedAt"]);

            // The key pins of a wss:// connection: a list, as they are written,
            // or one text of them, as somebody may write them by hand.
            var pinsText = JSON["serverKeyPins"] switch {
                               JArray list  => String.Join(",", list.Select(pin => pin.Type == JTokenType.String ? pin.Value<String>() : pin.ToString())),
                               JValue value => value.Type == JTokenType.Null ? null : value.ToString(),
                               _            => null
                           };

            if (!ValidateServerKeyPins(url, pinsText, out var serverKeyPins, out Error))
                return false;

            if (!ValidateKeyPinTrust(url,
                                     JSON.Value<String>("rawPublicKeys"),
                                     JSON.Value<String>("trustOnFirstUse"),
                                     JSON.Value<Int64?>("serverKeyPinProbationTime"),
                                     out var rawPublicKeys,
                                     out var keyTrustOnFirstUse,
                                     out var keyPinProbationTime,
                                     out Error))
                return false;

            if (!CheckKeyPinned(serverKeyPins, rawPublicKeys, keyTrustOnFirstUse, out Error))
                return false;

            if (!ValidatePins(url, JSON[PinsKey], $"connection '{id}'.{PinsKey}", out var pins, out Error) ||
                !CheckOneWay(serverKeyPins, keyTrustOnFirstUse, pins, out Error))
            {
                return false;
            }

            if (!TryParseHTTPVersion(url, JSON.Value<String>(HTTPVersionKey), out var httpVersion, out Error))
                return false;

            if (!TryParseEncoding(version, JSON.Value<String>(EncodingKey), out var encoding, out Error))
                return false;

            if (!TryParseSignatureFormats(version, JSON.Value<String>(SignatureFormatsKey), out _, out var signatureFormats, out Error))
                return false;

            // A key pin learned on first use is on probation only where nothing
            // is pinned, and only where the connection still learns.
            var learnsKeyPin       = serverKeyPins.Count == 0 && keyTrustOnFirstUse == KeyTrustOnFirstUse.Key;

            var keyPinLearned      = learnsKeyPin && KeyPin.TryParse(JSON.Value<String>("serverKeyPinLearned"), out var learnedPin)
                                         ? learnedPin
                                         : null;

            var keyPinLearnedAt    = keyPinLearned is not null
                                         ? WrittenAt(JSON["serverKeyPinLearnedAt"])
                                         : null;

            Entry = new ConnectionEntry(
                        id,
                        (JSON.Value<String>("description") ?? "").Trim(),
                        url,
                        type,
                        AuthenticationEntry.Written(JSON.Value<String>("createdAt"))
                    ) {
                        OCPPVersion         = version,
                        AutoConnect  = JSON.Value<Boolean?>("autoConnect") ?? false,
                        AuthenticationId    = Named(JSON.Value<String>("authenticationId")),
                        CertificateId       = Named(JSON.Value<String>("certificateId")),
                        SshHostKey          = hostKey,
                        SshTrustOnFirstUse  = trustOnFirstUse,
                        SshOnPinMismatch    = onPinMismatch,
                        SshProbationTime    = probationTime,
                        SshHostKeysVersion  = JSON.Value<UInt64?>("sshHostKeysVersion") ?? 0,
                        SshLearnedHostKey   = hostKey is null ? Named(JSON.Value<String>("sshLearnedHostKey")) : null,
                        SshLearnedAt        = hostKey is null ? learnedAt : null,
                        ServerKeyPins              = serverKeyPins,
                        ServerKeyPinsVersion       = serverKeyPins.Count > 0 ? JSON.Value<UInt64?>("serverKeyPinsVersion") ?? 0 : 0,
                        ServerKeyPinsConfirmed     = serverKeyPins.Count > 0 && JSON.Value<String>("serverKeyPinState") == nameof(KeyPinState.Confirmed),
                        RawPublicKeys              = rawPublicKeys,
                        TrustOnFirstUse            = keyTrustOnFirstUse,
                        ServerKeyPinProbationTime  = keyPinProbationTime,
                        ServerKeyPinLearned        = keyPinLearned,
                        ServerKeyPinLearnedAt      = keyPinLearnedAt,
                        Pins                       = pins,
                        PinsVersion                = pins is { Roots.Count: > 0 } ? JSON.Value<UInt64?>("pinsVersion") ?? 0 : 0,
                        HTTPVersion                = httpVersion ?? WebSocketHTTPVersion.Auto,
                        Encoding                   = encoding    ?? MessageEncodings.JSON,
                        SignatureFormats           = signatureFormats
                    };

            Error = null;
            return true;

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// What this connection looks like from outside.
        /// </summary>
        /// <remarks>
        /// The same shape on disk and in the web interface, because there is
        /// nothing secret here: a connection names its credentials, it does
        /// not hold them.
        /// </remarks>
        public JObject ToJSON()
        {

            var json = new JObject(
                           new JProperty("id",                  Id),
                           new JProperty("description",         Description),
                           new JProperty("url",                 URL.ToString()),
                           new JProperty("connectionType",      ConnectionType.ToString()),
                           new JProperty("ocppVersion",         AsText(OCPPVersion)),
                           new JProperty("autoConnect",  AutoConnect),
                           new JProperty("secure",              IsSecure),
                           new JProperty("transport",           IsSSH ? "ssh" : "websocket"),
                           new JProperty("createdAt",           CreatedAt.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
                       );

            if (AuthenticationId is not null)  json.Add(new JProperty("authenticationId",  AuthenticationId));
            if (CertificateId    is not null)  json.Add(new JProperty("certificateId",     CertificateId));
            if (SshHostKey       is not null)  json.Add(new JProperty("sshHostKey",        SshHostKey));

            if (IsSSH)
            {

                json.Add(new JProperty("sshTrustOnFirstUse",  SshTrustOnFirstUse.ToString()));
                json.Add(new JProperty("sshOnPinMismatch",    SshOnPinMismatch.  ToString()));
                json.Add(new JProperty("sshProbationTime",    (Int64) SshProbationTime.TotalSeconds));

                if (SshHostKeysVersion > 0)
                    json.Add(new JProperty("sshHostKeysVersion",  SshHostKeysVersion));

                if (SshLearnedHostKey is not null)
                {
                    json.Add(new JProperty("sshLearnedHostKey",  SshLearnedHostKey));
                    json.Add(new JProperty("sshLearnedAt",       SshLearnedAt?.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")));
                }

            }

            // How the server of a TLS connection is known: by its CA, or by
            // the key pin of its key ("Key Pins and Raw Public Keys", 7.1).
            if (IsSecure)
            {

                json.Add(new JProperty("keyPinned",  IsKeyPinned));

                if (ServerKeyPins.Count > 0)
                    json.Add(new JProperty("serverKeyPins",         new JArray(ServerKeyPins.Select(pin => pin.ToHex()))));

                if (ServerKeyPinsVersion > 0)
                    json.Add(new JProperty("serverKeyPinsVersion",  ServerKeyPinsVersion));

                json.Add(new JProperty("serverKeyPinState",          ServerKeyPinState. ToString()));
                json.Add(new JProperty("rawPublicKeys",              RawPublicKeys.     ToString()));
                json.Add(new JProperty("trustOnFirstUse",            TrustOnFirstUse.   ToString()));
                json.Add(new JProperty("serverKeyPinProbationTime",  (Int64) ServerKeyPinProbationTime.TotalSeconds));

                if (ServerKeyPinLearned is not null)
                {
                    json.Add(new JProperty("serverKeyPinLearned",    ServerKeyPinLearned.ToHex()));
                    json.Add(new JProperty("serverKeyPinLearnedAt",  ServerKeyPinLearnedAt?.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")));
                }

                // What a connection checked by its CA is held to beside that,
                // as the entry of a time server, a name server or a meter
                // writes it.
                if (Pins is not null && Pins.SaysAnything)
                {

                    var pins = new JObject();

                    Pins.WriteInto(pins);

                    json.Add(new JProperty(PinsKey, pins));

                    if (PinsVersion > 0)
                        json.Add(new JProperty("pinsVersion", PinsVersion));

                }

            }

            // Said where it is not left to the server: "auto" is what a
            // connection that says nothing gets.
            if (!IsSSH && HTTPVersion != WebSocketHTTPVersion.Auto)
                json.Add(new JProperty(HTTPVersionKey, AsText(HTTPVersion)));

            // Said where it is not what OCPP defines: "json" is what a
            // connection that says nothing gets.
            if (Encoding != MessageEncodings.JSON)
                json.Add(new JProperty(EncodingKey, AsText(Encoding)));

            // Said where the connection says it: nothing is what the policy says.
            if (SignatureFormats.HasValue)
                json.Add(new JProperty(SignatureFormatsKey, AsText(SignatureFormats)));

            if (Warnings.Count > 0)
                json.Add(new JProperty("warnings", new JArray(Warnings)));

            return json;

        }

        #endregion

        #region BindsItsPassword

        /// <summary>
        /// Whether a one-time password is bound to the TLS session of this
        /// connection: over HTTP/3 alone, whose QUIC exports the keying
        /// material one is bound with, and so only where the connection
        /// names HTTP/3 and nothing else. Where the version is left to the
        /// server, it may come to a TLS stream that exports nothing.
        /// </summary>
        public Boolean BindsItsPassword

            => !IsSSH &&
                HTTPVersion == WebSocketHTTPVersion.HTTP3;

        #endregion

        #region (static) NotOfAStore(Description, URL, ConnectionType, CreatedAt, ...)

        /// <summary>
        /// A connection no store holds and nothing writes anywhere: what a
        /// connection test is given from a form, so that it reads one shape
        /// whether it tests a connection of the store or one that is still
        /// being filled in. Nothing of it is checked here.
        /// </summary>
        public static ConnectionEntry NotOfAStore(String                 Description,
                                                  URL                    URL,
                                                  ConnectionType         ConnectionType,
                                                  DateTimeOffset         CreatedAt,
                                                  OCPPVersion            OCPPVersion,
                                                  Boolean                AutoConnect,
                                                  String?                AuthenticationId,
                                                  String?                CertificateId,
                                                  String?                SshHostKey,
                                                  SshTrustOnFirstUse     SshTrustOnFirstUse,
                                                  SshOnPinMismatch       SshOnPinMismatch,
                                                  TimeSpan               SshProbationTime,
                                                  IReadOnlyList<KeyPin>  ServerKeyPins,
                                                  RawPublicKeyMode       RawPublicKeys,
                                                  KeyTrustOnFirstUse     TrustOnFirstUse,
                                                  TimeSpan               ServerKeyPinProbationTime,
                                                  ServerPins?            Pins,
                                                  WebSocketHTTPVersion   HTTPVersion,
                                                  MessageEncodings       Encoding)

            => new (
                   "",
                   Description,
                   URL,
                   ConnectionType,
                   CreatedAt
               ) {
                   OCPPVersion                = OCPPVersion,
                   AutoConnect                = AutoConnect,
                   AuthenticationId           = Named(AuthenticationId),
                   CertificateId              = Named(CertificateId),
                   SshHostKey                 = SshHostKey,
                   SshTrustOnFirstUse         = SshTrustOnFirstUse,
                   SshOnPinMismatch           = SshOnPinMismatch,
                   SshProbationTime           = SshProbationTime,
                   ServerKeyPins              = ServerKeyPins,
                   RawPublicKeys              = RawPublicKeys,
                   TrustOnFirstUse            = TrustOnFirstUse,
                   ServerKeyPinProbationTime  = ServerKeyPinProbationTime,
                   Pins                       = Pins,
                   HTTPVersion                = HTTPVersion,
                   Encoding                   = Encoding
               };

        #endregion

        #region Copy()

        /// <summary>
        /// This connection as it is now, in an entry of its own: what the store
        /// puts back where a change of it could not be written. Its warnings
        /// are not copied; the store works them out whenever it is asked.
        /// </summary>
        public ConnectionEntry Copy()

            => new (Id, Description, URL, ConnectionType, CreatedAt) {
                   OCPPVersion       = OCPPVersion,
                   AutoConnect       = AutoConnect,
                   AuthenticationId  = AuthenticationId,
                   CertificateId       = CertificateId,
                   SshHostKey          = SshHostKey,
                   SshTrustOnFirstUse  = SshTrustOnFirstUse,
                   SshOnPinMismatch    = SshOnPinMismatch,
                   SshProbationTime    = SshProbationTime,
                   SshHostKeysVersion  = SshHostKeysVersion,
                   SshLearnedHostKey   = SshLearnedHostKey,
                   SshLearnedAt        = SshLearnedAt,
                   ServerKeyPins              = ServerKeyPins,
                   ServerKeyPinsVersion       = ServerKeyPinsVersion,
                   ServerKeyPinsConfirmed     = ServerKeyPinsConfirmed,
                   RawPublicKeys              = RawPublicKeys,
                   TrustOnFirstUse            = TrustOnFirstUse,
                   ServerKeyPinLearned        = ServerKeyPinLearned,
                   ServerKeyPinLearnedAt      = ServerKeyPinLearnedAt,
                   ServerKeyPinProbationTime  = ServerKeyPinProbationTime,
                   Pins                       = Pins,
                   PinsVersion                = PinsVersion,
                   HTTPVersion                = HTTPVersion,
                   Encoding                   = Encoding,
                   SignatureFormats           = SignatureFormats
               };

        #endregion

        #region TryParseHTTPVersion(URL, Text, out Version, out Error) / AsText(Version)

        /// <summary>
        /// The version of HTTP a connection names - "auto", "1.1", "2" or "3" -
        /// or the one sentence that says what is wrong. Nothing for nothing:
        /// a connection that does not say is not told what it said.
        /// </summary>
        /// <param name="URL">Where the connection goes: an ssh:// connection has no HTTP, and names no version of it.</param>
        /// <param name="Text">What stands under <see cref="HTTPVersionKey"/>.</param>
        /// <param name="Version">The version, or null where none is named.</param>
        /// <param name="Error">What is wrong.</param>
        public static Boolean TryParseHTTPVersion(URL                             URL,
                                                  String?                         Text,
                                                  out WebSocketHTTPVersion?       Version,
                                                  [NotNullWhen(false)] out String?  Error)
        {

            Version  = null;
            Error    = null;

            var text = Text?.Trim().ToLowerInvariant();

            if (text is null || text.Length == 0)
                return true;

            switch (text)
            {

                case "auto":  Version = WebSocketHTTPVersion.Auto;     break;
                case "1.1":   Version = WebSocketHTTPVersion.HTTP1_1;  break;
                case "2":     Version = WebSocketHTTPVersion.HTTP2;    break;
                case "3":     Version = WebSocketHTTPVersion.HTTP3;    break;

                default:
                    Error = $"'{Text}' is no version of HTTP a WebSocket is opened over: it is \"auto\", \"1.1\", \"2\" or \"3\".";
                    return false;

            }

            if (URL.Scheme == URIScheme.ssh && Version != WebSocketHTTPVersion.Auto)
            {
                Version  = null;
                Error    = "It names a version of HTTP, and an ssh:// connection speaks no HTTP to open a WebSocket over.";
                return false;
            }

            // HTTP/3 is HTTP over QUIC, and QUIC is TLS and nothing else.
            if (Version == WebSocketHTTPVersion.HTTP3 && URL.Scheme != URIScheme.wss && URL.Scheme != URIScheme.https)
            {
                Version  = null;
                Error    = "It names HTTP/3, which is HTTP over TLS and nothing else: a WebSocket over HTTP/3 needs a wss:// address.";
                return false;
            }

            return true;

        }

        /// <summary>
        /// A version of HTTP as a connection names it.
        /// </summary>
        public static String AsText(WebSocketHTTPVersion Version)

            => Version switch {
                   WebSocketHTTPVersion.HTTP1_1  => "1.1",
                   WebSocketHTTPVersion.HTTP2    => "2",
                   WebSocketHTTPVersion.HTTP3    => "3",
                   _                             => "auto"
               };

        /// <summary>
        /// A version of HTTP a WebSocket was opened over, as a connection names it.
        /// </summary>
        public static String AsText(HTTPVersion Version)

            => Version == org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPVersion.HTTP_3_0
                   ? "3"
                   : Version == org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPVersion.HTTP_2_0
                         ? "2"
                         : "1.1";

        #endregion

        #region TryParseEncoding(OCPPVersion, Text, out Encoding, out Error) / AsText(Encoding)

        /// <summary>
        /// How a connection says its messages are written - "json", "cbor" or
        /// "auto" - or the one sentence that says what is wrong. Nothing for
        /// nothing: a connection that does not say is not told what it said.
        /// </summary>
        /// <param name="OCPPVersion">Which OCPP the connection speaks: every version the station speaks has a CBOR form.</param>
        /// <param name="Text">What stands under <see cref="EncodingKey"/>.</param>
        /// <param name="Encoding">The encoding, or null where none is named.</param>
        /// <param name="Error">What is wrong.</param>
        public static Boolean TryParseEncoding(OCPPVersion                       OCPPVersion,
                                               String?                           Text,
                                               out MessageEncodings?             Encoding,
                                               [NotNullWhen(false)] out String?  Error)
        {

            Encoding  = null;
            Error     = null;

            var text  = Text?.Trim().ToLowerInvariant();

            if (text is null || text.Length == 0)
                return true;

            switch (text)
            {

                case "json":  Encoding = MessageEncodings.JSON;  break;
                case "cbor":  Encoding = MessageEncodings.CBOR;  break;
                case "auto":  Encoding = MessageEncodings.Auto;  break;

                default:
                    Error = $"'{Text}' is no way the messages of a connection are written: it is \"json\", \"cbor\" or \"auto\".";
                    return false;

            }

            return true;

        }

        /// <summary>
        /// The way messages are written as a connection names it.
        /// </summary>
        public static String AsText(MessageEncodings Encoding)

            => Encoding switch {
                   MessageEncodings.CBOR  => "cbor",
                   MessageEncodings.Auto  => "auto",
                   _                      => "json"
               };

        #endregion

        #region TryParseSignatureFormats(OCPPVersion, Text, out Said, out Formats, out Error) / AsText(Formats)

        /// <summary>
        /// Which signatures a connection says it writes - "jws", "cose" or
        /// "both", or "policy" for what the signature policy of the station
        /// says - or the one sentence that says what is wrong.
        /// </summary>
        /// <param name="OCPPVersion">Which OCPP the connection speaks: every version the station speaks has COSE signatures.</param>
        /// <param name="Text">What stands under <see cref="SignatureFormatsKey"/>.</param>
        /// <param name="Said">Whether anything was said: a change that says nothing keeps what there is.</param>
        /// <param name="Formats">The signatures, or null for what the policy says.</param>
        /// <param name="Error">What is wrong.</param>
        public static Boolean TryParseSignatureFormats(OCPPVersion                       OCPPVersion,
                                                       String?                           Text,
                                                       out Boolean                       Said,
                                                       out WWCPSignatureFormats?         Formats,
                                                       [NotNullWhen(false)] out String?  Error)
        {

            Said     = false;
            Formats  = null;
            Error    = null;

            var text = Text?.Trim().ToLowerInvariant();

            if (text is null || text.Length == 0)
                return true;

            switch (text)
            {

                case "jws":                      Formats = WWCPSignatureFormats.JWS;   break;
                case "cose":                     Formats = WWCPSignatureFormats.COSE;  break;
                case "both":                     Formats = WWCPSignatureFormats.Both;  break;
                case SignaturesAsThePolicySays:  Formats = null;                       break;

                default:
                    Error = $"'{Text}' names no signatures a connection writes: it is \"jws\", \"cose\", \"both\" or \"policy\".";
                    return false;

            }

            Said = true;
            return true;

        }

        /// <summary>
        /// The signatures a connection writes as it names them.
        /// </summary>
        public static String AsText(WWCPSignatureFormats? Formats)

            => Formats switch {
                   WWCPSignatureFormats.JWS   => "jws",
                   WWCPSignatureFormats.COSE  => "cose",
                   WWCPSignatureFormats.Both  => "both",
                   _                          => SignaturesAsThePolicySays
               };

        #endregion

        #region (override) ToString()

        public override String ToString()

            => $"{Description} ({ConnectionType} over {AsText(OCPPVersion)}, {URL})";

        #endregion

    }

}
