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
using System.Security.Authentication;

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.WWCP.Node.Configuration;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using cloud.charging.open.protocols.WWCP.Node.Connections;
using cloud.charging.open.protocols.WWCP.Node.Logins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving
{

    /// <summary>
    /// The "webSocketServer" section of the configuration file: the HTTP
    /// WebSocket server the charging stations below a node connect to, and the
    /// back ends above it that dial it.
    /// </summary>
    /// <remarks>
    /// One section for every node that serves WebSockets - a local controller,
    /// a gateway, a CSMS, a charging station - and each reads what it has.
    ///
    /// <b>One port for every subprotocol.</b> The server listens on one port,
    /// authenticates whoever connects as the section says, and hands each
    /// WebSocket by the subprotocol its client asks for to the node that
    /// speaks it: OCPP 1.6 to the node of OCPP 1.6, OCPP 2.0.1 and 2.1 to the
    /// node of OCPP 2.1 - and whatever else may come to whatever speaks it.
    /// Nothing in here is a subprotocol's: how a client authenticates is said
    /// by 'clientAuthentication', which OCPP calls its security profiles.
    ///
    /// As in the DNS and NTS sections, null means "the file does not say": what
    /// is missing keeps whatever the local controller was given at
    /// construction, and what it was given falls back to the defaults here.
    ///
    /// <b>Switched off unless somebody says otherwise.</b> This is the one
    /// setting in this file that opens a port to a network rather than reaching
    /// out to one, and the address it opens on is every interface, because
    /// charging stations arrive from the local network and never from the
    /// loopback address. A controller that started listening for them merely
    /// because it was updated to a version that can would be a port nobody
    /// chose to open.
    ///
    /// <b>Nothing secret lives here.</b> The passwords the charging stations
    /// sign in with and the private key of the server certificate are secrets,
    /// and this file is an ordinary one that anybody who can read the directory
    /// may read - see the configuration file. They live beside it
    /// in files of their own, with permissions of their own.
    /// </remarks>
    /// <param name="Enabled">Whether this node listens for WebSockets at all.</param>
    /// <param name="Address">Which interface it listens on; every one unless said otherwise.</param>
    /// <param name="TCPPort">Which TCP port it listens on, for every subprotocol.</param>
    /// <param name="ClientAuthentication">How a client may authenticate: by HTTP authentication without TLS or over it, or by its TLS client certificate.</param>
    /// <param name="Subprotocols">Which subprotocols are offered, e.g. the versions of OCPP.</param>
    /// <param name="ReachableAs">The names and addresses the charging stations reach this controller under - what a certificate has to be valid for.</param>
    /// <param name="MinimumTLSVersion">The oldest version of TLS accepted.</param>
    /// <param name="CheckCertificateRevocation">Whether a charging station's certificate is checked against its issuer's revocation list.</param>
    /// <param name="MaxConnections">How many charging stations may be connected at once.</param>
    /// <param name="PingEvery">How often a silent connection is pinged, which is how a station that went away is noticed.</param>
    /// <param name="Logging">What of this server ends up in the event log.</param>
    /// <param name="SSH">Whether, and how, the charging stations may connect over SSH as well.</param>
    /// <param name="HTTP3">Whether WebSockets are taken over HTTP/3 as well, on the UDP port of the same number; not where not said.</param>
    public sealed record WebSocketServerConfiguration(Boolean?                                         Enabled                      = null,
                                                      IIPAddress?                                      Address                      = null,
                                                      IPPort?                                          TCPPort                      = null,
                                                      IReadOnlyList<WebSocketClientAuthentication>?    ClientAuthentication         = null,
                                                      IReadOnlyList<String>?                           Subprotocols                 = null,
                                                      IReadOnlyList<String>?                           ReachableAs                  = null,
                                                      SslProtocols?                                    MinimumTLSVersion            = null,
                                                      Boolean?                                         CheckCertificateRevocation   = null,
                                                      UInt32?                                          MaxConnections               = null,
                                                      TimeSpan?                                        PingEvery                    = null,
                                                      WebSocketServerLogging?                          Logging                      = null,
                                                      WebSocketServerSSH?                              SSH                          = null,
                                                      Boolean?                                         HTTP3                        = null)
    {

        #region Data

        /// <summary>
        /// The name of this section in the configuration file.
        /// </summary>
        public const String  SectionName  = "webSocketServer";

        /// <summary>
        /// The TCP port the charging stations connect to, unless another is
        /// given.
        /// </summary>
        /// <remarks>
        /// Beside the web interface of this local controller (2350) and not on
        /// it: the two serve different networks, present different
        /// certificates and let in different callers, and a single port would
        /// make one set of rules out of two.
        /// </remarks>
        public static readonly IPPort         DefaultTCPPort             = IPPort.Parse(2351);

        /// <summary>
        /// Which subprotocols are offered when nothing says otherwise.
        /// </summary>
        /// <remarks>
        /// Not OCPP 1.6: a local controller that offers it can be talked to by
        /// a station that speaks nothing else, and that is a decision about
        /// what this site supports rather than a default to arrive at by
        /// accident.
        /// </remarks>
        public static readonly String[]       DefaultSubprotocols        = [ "ocpp2.1", "ocpp2.0.1" ];

        /// <summary>
        /// The subprotocols that may be offered at all: the versions of OCPP
        /// a node speaks - OCPP 2.1 as JSON and as CBOR, which the node of
        /// OCPP 2.1 reads as it reads JSON.
        /// </summary>
        public static readonly String[]       KnownSubprotocols          = [ "ocpp1.6", "ocpp2.0.1", "ocpp2.1", OCPPv2_1.Version.WebSocketSubProtocolIdCBOR ];

        /// <summary>
        /// How a client may authenticate, when nothing says otherwise.
        /// </summary>
        /// <remarks>
        /// Every way, which sounds laxer than it is: a way is only usable if
        /// the rest of the configuration supports it. Without a server
        /// certificate there is no TLS and only HTTP authentication without it
        /// can be spoken at all, and a client certificate has to be one from
        /// an issuer this node was told to trust.
        /// </remarks>
        public static readonly WebSocketClientAuthentication[]  DefaultClientAuthentication  = WebSocketClientAuthentications.All;

        /// <summary>
        /// How many charging stations may be connected at once, when nothing
        /// says otherwise.
        /// </summary>
        public const           UInt32         DefaultMaxConnections      = 250;

        /// <summary>
        /// The most that may be configured - a limit on the limit, so that a
        /// mistyped number cannot turn into a memory reservation.
        /// </summary>
        public const           UInt32         MaxMaxConnections          = 10000;

        /// <summary>
        /// How often a silent connection is pinged, when nothing says
        /// otherwise.
        /// </summary>
        /// <remarks>
        /// This is how a charging station that went away without closing its
        /// connection is noticed, so it has to be shorter than the idle timeout
        /// of whatever is between the two - which over mobile networks and
        /// behind NAT is frequently a minute or two.
        /// </remarks>
        public static readonly TimeSpan       DefaultPingEvery           = TimeSpan.FromSeconds(30);

        /// <summary>
        /// The oldest version of TLS accepted, when nothing says otherwise.
        /// </summary>
        /// <remarks>
        /// TLS 1.2 and not 1.3, although 1.3 is the better answer: charging
        /// stations are long-lived, frequently cannot be updated, and a
        /// controller that refused 1.2 would refuse a good part of what is
        /// installed. Nothing older is offered at all.
        /// </remarks>
        public const           SslProtocols   DefaultMinimumTLSVersion   = SslProtocols.Tls12 | SslProtocols.Tls13;

        /// <summary>
        /// How many names this controller may be reachable under.
        /// </summary>
        public const           Int32          MaxReachableAs             = 32;

        /// <summary>
        /// The longest such a name may be written; what a DNS name may be.
        /// </summary>
        public const           Int32          MaxNameLength              = 253;

        /// <summary>
        /// The shortest and longest ping interval that may be configured, in
        /// seconds.
        /// </summary>
        public const           Double         MinPingEverySeconds        = 5;
        public const           Double         MaxPingEverySeconds        = 3600;

        #endregion

        #region Properties

        /// <summary>
        /// Whether the server is on: off where not said.
        /// </summary>
        public Boolean IsEnabled
            => Enabled == true;

        /// <summary>
        /// Whether WebSockets are taken over HTTP/3 as well: not where not
        /// said. HTTP/3 is QUIC, and opens a UDP port of the same number beside
        /// the TCP port, which nobody is given who did not ask.
        /// </summary>
        public Boolean TakesHTTP3
            => HTTP3 == true;

        /// <summary>
        /// Whether this section says anything at all.
        /// </summary>
        public Boolean IsEmpty

            => !Enabled.HasValue                     &&
                Address                   is null    &&
               !TCPPort.HasValue                     &&
                ClientAuthentication      is null    &&
                Subprotocols              is null    &&
                ReachableAs               is null    &&
               !MinimumTLSVersion.HasValue           &&
               !CheckCertificateRevocation.HasValue  &&
               !MaxConnections.HasValue              &&
               !PingEvery.HasValue                   &&
               !HTTP3.HasValue                       &&
               (Logging is null || Logging.IsEmpty)  &&
               (SSH     is null || SSH.    IsEmpty);

        #endregion


        #region (static) TryParse(JSON, out Configuration, out Error)

        /// <summary>
        /// The "webSocketServer" section, or the one sentence that says what
        /// is wrong with it.
        /// </summary>
        public static Boolean TryParse(JObject                                               JSON,
                                       [NotNullWhen(true)]  out WebSocketServerConfiguration? Configuration,
                                       [NotNullWhen(false)] out String?                       Error,
                                       String?                                                NodeNoun   = null)
        {

            Configuration  = null;
            Error          = null;

            #region What this section said before

            // A port of its own for OCPP 1.6 and OCPP's security profiles were
            // fields of this section's former self, "ocppServer": what reads
            // them now would believe they were obeyed.
            if (JSON.ContainsKey("port16"))
            {
                Error = $"'{SectionName}.port16' is no more: every subprotocol is served on '{SectionName}.port', " +
                        $"OCPP 1.6 where '{SectionName}.subprotocols' names \"ocpp1.6\".";
                return false;
            }

            if (JSON.ContainsKey("securityProfiles"))
            {
                Error = $"'{SectionName}.securityProfiles' is no more: say how a client may authenticate in '{SectionName}.clientAuthentication', " +
                        $"by {String.Join(", ", WebSocketClientAuthentications.Names.Select(name => $"\"{name}\""))} - OCPP's security profiles 1, 2 and 3.";
                return false;
            }

            #endregion

            if (!ConfigurationReader.TryReadBoolean(JSON, "enabled",                     SectionName, out var enabled,     out Error) ||
                !ConfigurationReader.TryReadPort   (JSON, "port",                        SectionName, out var port,        out Error) ||
                !ConfigurationReader.TryReadBoolean(JSON, "http3",                       SectionName, out var http3,       out Error) ||
                !ConfigurationReader.TryReadBoolean(JSON, "checkCertificateRevocation",  SectionName, out var checkCRL,    out Error) ||
                !ConfigurationReader.TryReadUInt32 (JSON, "maxConnections",              SectionName, 1, MaxMaxConnections, out var maxConnections, out Error) ||
                !ConfigurationReader.TryReadSeconds(JSON, "pingEverySeconds",            SectionName, MinPingEverySeconds, MaxPingEverySeconds, out var pingEvery, out Error) ||
                !ConfigurationReader.TryReadString (JSON, "address",                     SectionName, 45,  out var addressText,    out Error) ||
                !ConfigurationReader.TryReadString (JSON, "minTLSVersion",               SectionName, 8,   out var tlsVersionText, out Error) ||
                !ConfigurationReader.TryReadStrings(JSON, "subprotocols",                SectionName, KnownSubprotocols.Length, 32, out var subprotocols, out Error) ||
                !ConfigurationReader.TryReadStrings(JSON, "reachableAs",                 SectionName, MaxReachableAs, MaxNameLength, out var reachableAs, out Error))
            {
                return false;
            }

            #region The interface to listen on

            IIPAddress? address = null;

            if (addressText is not null && !IPAddress.TryParse(addressText, out address))
            {
                Error = $"'{SectionName}.address' must be an IP address, e.g. \"0.0.0.0\" for every interface.";
                return false;
            }

            #endregion

            #region The oldest version of TLS accepted

            SslProtocols? minimumTLSVersion = null;

            if (tlsVersionText is not null)
            {

                minimumTLSVersion = tlsVersionText switch {
                                        "1.2"  => SslProtocols.Tls12 | SslProtocols.Tls13,
                                        "1.3"  => SslProtocols.Tls13,
                                        _      => null
                                    };

                if (!minimumTLSVersion.HasValue)
                {
                    Error = $"'{SectionName}.minTLSVersion' must be \"1.2\" or \"1.3\". Nothing older than TLS 1.2 is offered.";
                    return false;
                }

            }

            #endregion

            #region Which OCPP versions are offered

            if (subprotocols is not null)
            {

                if (subprotocols.Count == 0)
                {
                    Error = $"'{SectionName}.subprotocols' must name at least one subprotocol, or a client has nothing to ask for.";
                    return false;
                }

                var unknown = subprotocols.FirstOrDefault(subprotocol => !KnownSubprotocols.Contains(subprotocol));

                if (unknown is not null)
                {
                    Error = $"'{SectionName}.subprotocols': '{unknown}' is not a subprotocol this {NodeNoun ?? NodeWording.Noun} speaks ({String.Join(", ", KnownSubprotocols)}).";
                    return false;
                }

                subprotocols = [.. subprotocols.Distinct()];

            }

            #endregion

            #region How a client may authenticate

            IReadOnlyList<WebSocketClientAuthentication>? clientAuthentication = null;

            if (JSON["clientAuthentication"] is JToken authenticationToken && authenticationToken.Type != JTokenType.Null)
            {

                var ways = $"{String.Join(", ", WebSocketClientAuthentications.Names.Select(name => $"\"{name}\""))}";

                if (authenticationToken is not JArray authenticationArray)
                {
                    Error = $"'{SectionName}.clientAuthentication' must be an array of the ways a client may authenticate: {ways}.";
                    return false;
                }

                var authentications = new List<WebSocketClientAuthentication>();

                foreach (var token in authenticationArray)
                {

                    if (token.Type != JTokenType.String ||
                        !WebSocketClientAuthentications.TryParse(token.Value<String>(), out var authentication))
                    {
                        Error = $"'{SectionName}.clientAuthentication': {token.ToString(Newtonsoft.Json.Formatting.None)} is no way a client may authenticate; they are {ways}.";
                        return false;
                    }

                    if (!authentications.Contains(authentication))
                        authentications.Add(authentication);

                }

                if (authentications.Count == 0)
                {
                    Error = $"'{SectionName}.clientAuthentication' must allow at least one way, or no client can connect at all. Switch the server off instead.";
                    return false;
                }

                clientAuthentication = [.. authentications.Order()];

            }

            #endregion

            #region The names the charging stations reach this controller under

            if (reachableAs is not null)
            {
                foreach (var name in reachableAs)
                {
                    if (!IPAddress.TryParse(name, out _) &&
                        !DomainName.TryParse(name, out _, out var problem))
                    {
                        Error = $"'{SectionName}.reachableAs': '{name}' is neither an IP address nor a domain name. {problem}";
                        return false;
                    }
                }

                reachableAs = [.. reachableAs.Distinct(StringComparer.OrdinalIgnoreCase)];
            }

            #endregion

            #region What of this server ends up in the event log

            WebSocketServerLogging? logging = null;

            if (JSON[WebSocketServerLogging.FieldName] is JToken loggingToken && loggingToken.Type != JTokenType.Null)
            {

                if (loggingToken is not JObject loggingJSON)
                {
                    Error = $"'{SectionName}.{WebSocketServerLogging.FieldName}' must be a JSON object.";
                    return false;
                }

                if (!WebSocketServerLogging.TryParse(loggingJSON, out logging, out Error))
                    return false;

            }

            #endregion

            #region OCPP over SSH

            WebSocketServerSSH? ssh = null;

            if (JSON[WebSocketServerSSH.FieldName] is JToken sshToken && sshToken.Type != JTokenType.Null)
            {

                if (sshToken is not JObject sshJSON)
                {
                    Error = $"'{WebSocketServerSSH.Path}' must be a JSON object.";
                    return false;
                }

                if (!WebSocketServerSSH.TryParse(sshJSON, out ssh, out Error, NodeNoun))
                    return false;

            }

            #endregion

            Configuration = new WebSocketServerConfiguration(
                                enabled,
                                address,
                                port,
                                clientAuthentication,
                                subprotocols,
                                reachableAs,
                                minimumTLSVersion,
                                checkCRL,
                                maxConnections,
                                pingEvery,
                                logging,
                                ssh,
                                http3
                            );

            return true;

        }

        #endregion

        #region Merge(Update) / Effective()

        /// <summary>
        /// This section with the fields the update mentions written over it,
        /// and the ones it does not mention left as they are.
        /// </summary>
        /// <remarks>
        /// The same rule the configuration file follows, in memory: a page that
        /// offers three checkboxes sends three checkboxes, and must not be able
        /// to take the port with it.
        /// </remarks>
        public WebSocketServerConfiguration Merge(WebSocketServerConfiguration? Update)

            => Update is null
                   ? this
                   : new WebSocketServerConfiguration(
                         Update.Enabled                    ?? Enabled,
                         Update.Address                    ?? Address,
                         Update.TCPPort                    ?? TCPPort,
                         Update.ClientAuthentication       ?? ClientAuthentication,
                         Update.Subprotocols               ?? Subprotocols,
                         Update.ReachableAs                ?? ReachableAs,
                         Update.MinimumTLSVersion          ?? MinimumTLSVersion,
                         Update.CheckCertificateRevocation ?? CheckCertificateRevocation,
                         Update.MaxConnections             ?? MaxConnections,
                         Update.PingEvery                  ?? PingEvery,
                         (Logging ?? new WebSocketServerLogging()).Merge(Update.Logging),
                         (SSH     ?? new WebSocketServerSSH())    .Merge(Update.SSH),
                         Update.HTTP3                      ?? HTTP3
                     );

        /// <summary>
        /// This section with every field filled in.
        /// </summary>
        /// <remarks>
        /// Switched off is the default, and the only default here that is a
        /// decision rather than a number - see the remarks on this record.
        /// </remarks>
        public WebSocketServerConfiguration Effective()

            => new (
                   Enabled                    ?? false,
                   Address                    ?? IPv4Address.Any,
                   TCPPort                    ?? DefaultTCPPort,
                   ClientAuthentication       ?? DefaultClientAuthentication,
                   Subprotocols               ?? DefaultSubprotocols,
                   ReachableAs                ?? [],
                   MinimumTLSVersion          ?? DefaultMinimumTLSVersion,
                   CheckCertificateRevocation ?? false,
                   MaxConnections             ?? DefaultMaxConnections,
                   PingEvery                  ?? DefaultPingEvery,
                   (Logging ?? new WebSocketServerLogging()).Effective(),
                   (SSH     ?? new WebSocketServerSSH())    .Effective(),
                   HTTP3                      ?? false
               );

        #endregion

        #region NeedsARestartFor(Update)

        /// <summary>
        /// Which of the fields in an update cannot be put into effect while the
        /// server is running, and will be waiting at the next start.
        /// </summary>
        /// <remarks>
        /// <b>Why some and not others.</b> The certificate, the accepted
        /// chains, the charging station logins and what is logged are read
        /// afresh every time they are needed, so changing them changes what
        /// happens next. The port, the interface, the OCPP versions on offer,
        /// the TLS versions and whether a client certificate is asked for are
        /// decided when the socket is opened and the server is built on top of
        /// it - and rebuilding that under a running OCPP node would leave the
        /// node holding a server nobody is connected to.
        ///
        /// Saying which is which is the point: a page that quietly does nothing
        /// is worse than one that says "at the next start".
        /// </remarks>
        public IReadOnlyList<String> NeedsARestartFor(WebSocketServerConfiguration Update)
        {

            var fields = new List<String>();

            if (Update.Address is not null && !Update.Address.Equals(Address))
                fields.Add("address");

            if (Update.TCPPort.HasValue && Update.TCPPort != TCPPort)
                fields.Add("port");

            if (Update.Subprotocols is not null && Subprotocols is not null &&
                !Update.Subprotocols.SequenceEqual(Subprotocols))
                fields.Add("subprotocols");

            if (Update.MinimumTLSVersion.HasValue && Update.MinimumTLSVersion != MinimumTLSVersion)
                fields.Add("minTLSVersion");

            if (Update.MaxConnections.HasValue && Update.MaxConnections != MaxConnections)
                fields.Add("maxConnections");

            if (Update.PingEvery.HasValue && Update.PingEvery != PingEvery)
                fields.Add("pingEverySeconds");

            if (Update.CheckCertificateRevocation.HasValue && Update.CheckCertificateRevocation != CheckCertificateRevocation)
                fields.Add("checkCertificateRevocation");

            // How a client may authenticate is deliberately not in this list.
            // A client is asked for a certificate on every connection whether
            // or not one is a way in, and what is done with the one it sends -
            // or does not send - is decided per connection. So narrowing or
            // widening the ways is in effect as soon as it is saved.

            if (Update.SSH is not null)
                fields.AddRange((SSH ?? new WebSocketServerSSH()).NeedsARestartFor(Update.SSH));

            return fields;

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// The section as it is written to the file; what this local controller
        /// was not told about is not written.
        /// </summary>
        public JObject ToJSON()
        {

            var json = new JObject();

            if (Enabled.HasValue)                     json.Add("enabled",                     Enabled.Value);
            if (Address is not null)                  json.Add("address",                     Address.ToString());
            if (TCPPort.HasValue)                     json.Add("port",                        TCPPort.Value.ToUInt16());
            if (HTTP3.HasValue)                       json.Add("http3",                       HTTP3.Value);
            if (ClientAuthentication is not null)     json.Add("clientAuthentication",        new JArray(ClientAuthentication.Select(authentication => authentication.ToText())));
            if (Subprotocols is not null)             json.Add("subprotocols",                new JArray(Subprotocols));
            if (ReachableAs is not null)              json.Add("reachableAs",                 new JArray(ReachableAs));
            if (MinimumTLSVersion.HasValue)           json.Add("minTLSVersion",               TLSVersionText(MinimumTLSVersion.Value));
            if (CheckCertificateRevocation.HasValue)  json.Add("checkCertificateRevocation",  CheckCertificateRevocation.Value);
            if (MaxConnections.HasValue)              json.Add("maxConnections",              MaxConnections.Value);
            if (PingEvery.HasValue)                   json.Add("pingEverySeconds",            PingEvery.Value.TotalSeconds);

            if (Logging is not null && !Logging.IsEmpty)
                json.Add(WebSocketServerLogging.FieldName, Logging.ToJSON());

            if (SSH is not null && !SSH.IsEmpty)
                json.Add(WebSocketServerSSH.FieldName, SSH.ToJSON());

            return json;

        }

        #endregion

        #region (static) TLSVersionText(Protocols)

        /// <summary>
        /// How a minimum TLS version is written in the file.
        /// </summary>
        public static String TLSVersionText(SslProtocols Protocols)

            => Protocols.HasFlag(SslProtocols.Tls12)
                   ? "1.2"
                   : "1.3";

        #endregion

        #region (override) ToString()

        public override String ToString()

            => !IsEnabled
                   ? "WebSocket server off"
                   : $"WebSocket server on {Address?.ToString() ?? "every interface"}" +
                     (TCPPort.HasValue ? $":{TCPPort}"            : "") +
                     (TakesHTTP3       ? ", over HTTP/3 as well"  : "");

        #endregion

    }

}
