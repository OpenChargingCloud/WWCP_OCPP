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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Web;
using cloud.charging.open.protocols.WWCP.Node.Logging;
using cloud.charging.open.protocols.WWCP.Node.Configuration;

using cloud.charging.open.protocols.OCPP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.KeyPins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The HTTP API of a node that dials a CSMS, for what it dials: its
    /// connections and what they prove themselves with, the keys and
    /// certificates it holds up, the state of each connection, and the test
    /// of one. The same routes for a charging station and for a local
    /// controller, each of which adds what else it has.
    /// </summary>
    public abstract class OCPPDiallingHTTPAPI : NodeHTTPAPI
    {

        #region Properties

        /// <summary>
        /// The node whose connections these are.
        /// </summary>
        protected OCPPDiallingNode  Dialling               { get; }

        /// <summary>
        /// The resource a user needs a right to for reading, changing and
        /// testing the connections.
        /// </summary>
        protected String            ConnectionsResource    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The API of a node that dials a CSMS, on the given HTTP server.
        /// </summary>
        /// <param name="ConnectionsResource">The resource a user needs a right to for the connections.</param>
        protected OCPPDiallingHTTPAPI(HTTPServer        HTTPServer,
                                      OCPPDiallingNode  Node,
                                      HTTPExtAPI        ExtAPI,
                                      EventLog          Log,
                                      String            ConnectionsResource,
                                      HTTPPath?         APIPath   = null,
                                      String?           Version   = null)

            : base(HTTPServer,
                   Node,
                   ExtAPI,
                   Log,
                   APIPath,
                   Version)

        {

            this.Dialling             = Node;
            this.ConnectionsResource  = ConnectionsResource;

            RegisterConnectionRoutes();

        }

        #endregion

        #region (private) RegisterConnectionRoutes()

        private void RegisterConnectionRoutes()
        {

            AddHandler(HTTPPath.Root + "v1/status/connections", GetConnectionStates, HTTPMethod.GET);
            AddHandler(HTTPPath.Root + "v1/configuration/authentications",        GetConnectionsAndAuth,     HTTPMethod.GET);
            AddHandler(HTTPPath.Root + "v1/configuration/authentications",        PostAuthentication,        HTTPMethod.POST);
            AddHandler(HTTPPath.Root + "v1/configuration/authentications/update", PostUpdateAuthentication,  HTTPMethod.POST);
            AddHandler(HTTPPath.Root + "v1/configuration/authentications/remove", PostRemoveAuthentication,  HTTPMethod.POST);
            AddHandler(HTTPPath.Root + "v1/configuration/connections",            GetConnectionsAndAuth,     HTTPMethod.GET);
            AddHandler(HTTPPath.Root + "v1/configuration/connections",            PostConnection,            HTTPMethod.POST);
            AddHandler(HTTPPath.Root + "v1/configuration/connections/update",     PostUpdateConnection,      HTTPMethod.POST);
            AddHandler(HTTPPath.Root + "v1/configuration/connections/remove",     PostRemoveConnection,      HTTPMethod.POST);
            AddHandler(HTTPPath.Root + "v1/configuration/connections/test",       PostTestConnection,        HTTPMethod.POST);
            AddHandler(HTTPPath.Root + "v1/configuration/connections/pinSetSigners",         PostPinSetSigner,        HTTPMethod.POST);
            AddHandler(HTTPPath.Root + "v1/configuration/connections/pinSetSigners/remove",  PostRemovePinSetSigner,  HTTPMethod.POST);
            AddHandler(HTTPPath.Root + "v1/configuration/certificates",        GetClientCertificates,   HTTPMethod.GET);
            AddHandler(HTTPPath.Root + "v1/configuration/certificates",        PostClientKey,           HTTPMethod.POST);
            AddHandler(HTTPPath.Root + "v1/configuration/certificates/import", PostClientCertificate,   HTTPMethod.POST);
            AddHandler(HTTPPath.Root + "v1/configuration/certificates/remove", PostRemoveClientKey,     HTTPMethod.POST);

        }

        #endregion


        /// <summary>
        /// The whole picture both pages work from: the credentials, the
        /// connections, and the client certificates a connection may name.
        /// </summary>
        /// <remarks>
        /// One answer for two pages rather than two answers, because the
        /// Connections page needs the credentials in order to offer them and
        /// the Authentication page needs the connections in order to say which
        /// ones would break. Two routes returning it is a convenience for
        /// whoever reads the API; they are deliberately the same handler, so
        /// the two pages can never come to disagree about what exists.
        ///
        /// The passwords and shared secrets are not in here. What is in here
        /// is whether each set of credentials has one - see
        /// <see cref="AuthenticationEntry.ToJSON"/>, which leaves them out
        /// unless it is writing the file itself.
        /// </remarks>
        protected Task<HTTPResponse> GetConnectionsAndAuth(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Read(ConnectionsResource), false, out _, out var refused))
                return Task.FromResult(refused);

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, ConnectionsJSON())
                   );

        }

        /// <summary>
        /// GET /api/v1/status/connections: where each connection this station
        /// dialled stands.
        /// </summary>
        /// <remarks>
        /// Asked again and again by the Connections page while it is open, so
        /// that a connection lost or back again is seen without a reload -
        /// which is why it is its own route and not the whole configuration
        /// every few seconds. At the permission that reads the configuration,
        /// because what it says - the addresses, what the back end answered -
        /// is the configuration's.
        /// </remarks>
        protected Task<HTTPResponse> GetConnectionStates(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Read(ConnectionsResource), false, out _, out var refused))
                return Task.FromResult(refused);

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, ConnectionStatesJSON())
                   );

        }

        /// <summary>
        /// POST /api/v1/configuration/authentications with
        /// {"description", "kind", "login", "secret", ...}: one set of
        /// credentials this station can prove itself with.
        /// </summary>
        /// <remarks>
        /// At the permission that changes how this station reaches the outside
        /// world, the same as the certificates and the name servers: a login
        /// for a back end is a statement about how this station gets to one,
        /// and nothing about what the equipment is or what it measures.
        /// </remarks>
        protected Task<HTTPResponse> PostAuthentication(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(ConnectionsResource), true, out _, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (!Dialling.Connections.TryAddAuthentication(json.Value<String>("description"),
                                                           json.Value<String>("kind"),
                                                           json.Value<String>("login"),
                                                           json.Value<String>("secret"),
                                                           out var id,
                                                           out var error,
                                                           out var notSaved,
                                                           json.Value<Double?> ("validitySeconds"),
                                                           json.Value<UInt32?>("length"),
                                                           json.Value<String> ("alphabet"),
                                                           json.Value<String> ("hashAlgorithm"),
                                                           json.Value<Boolean?>("tlsChannelBinding")))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));
            }

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.Created,
                                    new JObject(
                                        new JProperty("id",          id),
                                        new JProperty("connections", ConnectionsJSON())
                                    ))
                   );

        }

        /// <summary>
        /// POST /api/v1/configuration/authentications/update with {"id", ...}.
        /// </summary>
        /// <remarks>
        /// A secret left out means the one already there stays. That is what
        /// makes it possible to correct a description without being shown the
        /// password - and being shown it is exactly what this station never
        /// does.
        /// </remarks>
        protected Task<HTTPResponse> PostUpdateAuthentication(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(ConnectionsResource), true, out _, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (!Dialling.Connections.TryUpdateAuthentication(json.Value<String>("id"),
                                                              json.Value<String>("description"),
                                                              json.Value<String>("kind"),
                                                              json.Value<String>("login"),
                                                              json.Value<String>("secret"),
                                                              out var error,
                                                              out var notSaved,
                                                              json.Value<Double?> ("validitySeconds"),
                                                              json.Value<UInt32?>("length"),
                                                              json.Value<String> ("alphabet"),
                                                              json.Value<String> ("hashAlgorithm"),
                                                              json.Value<Boolean?>("tlsChannelBinding")))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));
            }

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, ConnectionsJSON())
                   );

        }

        /// <summary>
        /// POST /api/v1/configuration/authentications/remove with {"id"}.
        /// </summary>
        /// <remarks>
        /// Refused while a connection is using it, and the refusal names the
        /// connection: the alternative leaves a station that cannot dial home
        /// and tells nobody why.
        /// </remarks>
        protected Task<HTTPResponse> PostRemoveAuthentication(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(ConnectionsResource), true, out _, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (!Dialling.Connections.TryRemoveAuthentication(json.Value<String>("id"), out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, ConnectionsJSON())
                   );

        }

        /// <summary>
        /// POST /api/v1/configuration/connections with
        /// {"description", "url", "connectionType", ...}: one place this
        /// station dials.
        /// </summary>
        protected async Task<HTTPResponse> PostConnection(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(ConnectionsResource), true, out _, out var refused))
                return refused;

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return errorResponse;

            if (!TryPinsOf(json, out var pins, out var wrongPins))
                return NotChanged(Request, HTTPStatusCode.BadRequest, wrongPins, false);

            if (!Dialling.Connections.TryAddConnection(json.Value<String>("description"),
                                                       json.Value<String>("url"),
                                                       json.Value<String>("connectionType"),
                                                       json.Value<Boolean?>("autoConnect"),
                                                       json.Value<String>("authenticationId"),
                                                       json.Value<String>("certificateId"),
                                                       out var id,
                                                       out var error,
                                                       out var notSaved,
                                                       json.Value<String>("ocppVersion"),
                                                       json.Value<String>("sshHostKey"),
                                                       ConnectionFields.SshTrustOf(json),
                                                       ConnectionFields.KeyPinsOf(json, "serverKeyPins"),
                                                       ConnectionFields.KeyPinTrustOf(json),
                                                       pins,
                                                       json.Value<String>(ConnectionEntry.HTTPVersionKey),
                                                       json.Value<String>(ConnectionEntry.EncodingKey),
                                                       json.Value<String>(ConnectionEntry.SignatureFormatsKey)))
            {
                return NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved);
            }

            await SshKeyIfNeeded(id, Request.CancellationToken);

            return JSONResponse(Request, HTTPStatusCode.Created,
                                new JObject(
                                    new JProperty("id",          id),
                                    new JProperty("connections", ConnectionsJSON())
                                ));

        }

        /// <summary>
        /// POST /api/v1/configuration/connections/update with {"id", ...}.
        /// </summary>
        /// <remarks>
        /// The host key as the page showed it comes back beside the one in the
        /// form: where somebody left it as it was, the one there is now stays -
        /// written down meanwhile by the first connection, perhaps. The same
        /// for the key pins of a wss:// connection, which a signed pin set may
        /// have replaced meanwhile.
        /// </remarks>
        protected async Task<HTTPResponse> PostUpdateConnection(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(ConnectionsResource), true, out _, out var refused))
                return refused;

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return errorResponse;

            if (!TryPinsOf(json, out var pins, out var wrongPins))
                return NotChanged(Request, HTTPStatusCode.BadRequest, wrongPins, false);

            if (!Dialling.Connections.TryUpdateConnection(json.Value<String>("id"),
                                                          json.Value<String>("description"),
                                                          json.Value<String>("url"),
                                                          json.Value<String>("connectionType"),
                                                          json.Value<Boolean?>("autoConnect"),
                                                          json.Value<String>("authenticationId"),
                                                          json.Value<String>("certificateId"),
                                                          out var error,
                                                          out var notSaved,
                                                          json.Value<String>("ocppVersion"),
                                                          json.Value<String>("sshHostKey"),
                                                          json.Value<String>("sshHostKeyAsShown"),
                                                          ConnectionFields.SshTrustOf(json),
                                                          ConnectionFields.KeyPinsOf(json, "serverKeyPins"),
                                                          ConnectionFields.KeyPinsOf(json, "serverKeyPinsAsShown"),
                                                          ConnectionFields.KeyPinTrustOf(json),
                                                          pins,
                                                          json.Value<String>(ConnectionEntry.HTTPVersionKey),
                                                          json.Value<String>(ConnectionEntry.EncodingKey),
                                                          json.Value<String>(ConnectionEntry.SignatureFormatsKey)))
            {
                return NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved);
            }

            await SshKeyIfNeeded(json.Value<String>("id"), Request.CancellationToken);

            return JSONResponse(Request, HTTPStatusCode.OK, ConnectionsJSON());

        }

        /// <summary>
        /// POST /api/v1/configuration/connections/remove with {"id"}.
        /// </summary>
        /// <remarks>
        /// The credentials it was using stay behind: they are frequently the
        /// ones whatever replaces it will use.
        /// </remarks>
        protected Task<HTTPResponse> PostRemoveConnection(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(ConnectionsResource), true, out _, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (!Dialling.Connections.TryRemoveConnection(json.Value<String>("id"), out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, ConnectionsJSON())
                   );

        }

        /// <summary>
        /// POST /api/v1/configuration/connections/test with the fields of a
        /// connection: make it once, and say everything that happened.
        ///
        /// The fields rather than an identification, because the page offers
        /// this beside a connection being written down for the first time as
        /// well as beside one that already exists - and in both cases what
        /// somebody means by "test it" is what is on the screen. Nothing is
        /// stored either way.
        /// </summary>
        /// <remarks>
        /// At the diagnostics permission and not at the one that reads the
        /// configuration, for the same reason the name server and time server
        /// tests are: this makes the station open a connection to a host and
        /// show it a credential. That is more than it sounds like to hand to
        /// everybody who may look at a page, and it is less than changing what
        /// the station is configured to do - which is why it is not the
        /// network-settings permission either.
        ///
        /// It holds the request open for as long as the test takes, which is
        /// the connection plus about two seconds. The page's own deadline for
        /// a write is comfortably longer.
        /// </remarks>
        protected async Task<HTTPResponse> PostTestConnection(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Run (ConnectionsResource), true, out var user, out var refused))
                return refused;

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return errorResponse;

            Log.Info($"'{user.Id}' asked this station to test a connection.", "ocpp", "connections", "test", "web");

            // With "ocppRequest" for what is asked in OCPP once it is
            // connected: "heartbeat", which it is where nothing is said, or "none".
            return JSONResponse(
                       Request,
                       HTTPStatusCode.OK,
                       await Dialling.TestConnection(json, Request.CancellationToken)
                   );

        }

        /// <summary>
        /// POST /api/v1/configuration/connections/pinSetSigners with
        /// {"publicKey", "description"}: a key a pin set for a connection over
        /// SSH is believed of - the CSMS's signing anchor.
        /// </summary>
        protected Task<HTTPResponse> PostPinSetSigner(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(ConnectionsResource), true, out _, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (!Dialling.Connections.TryAddPinSetSigner(json.Value<String>("publicKey"), json.Value<String>("description"), out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));

            return Task.FromResult(JSONResponse(Request, HTTPStatusCode.Created, ConnectionsJSON()));

        }

        /// <summary>
        /// POST /api/v1/configuration/connections/pinSetSigners/remove with
        /// {"publicKey"}: no pin set signed by it is believed any more.
        /// </summary>
        protected Task<HTTPResponse> PostRemovePinSetSigner(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(ConnectionsResource), true, out _, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (!Dialling.Connections.TryRemovePinSetSigner(json.Value<String>("publicKey"), out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));

            return Task.FromResult(JSONResponse(Request, HTTPStatusCode.OK, ConnectionsJSON()));

        }

        /// <summary>
        /// GET /api/v1/configuration/certificates: the keys and certificates
        /// this station holds up when it dials a back end.
        /// </summary>
        /// <remarks>
        /// Reading them needs no more than reading anything else here: a
        /// certificate is a signature over a public key, a signing request is
        /// meant to be handed to somebody, and the private keys are the one
        /// thing that never appears in this answer at all.
        /// </remarks>
        protected Task<HTTPResponse> GetClientCertificates(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Read(ConnectionsResource), false, out _, out var refused))
                return Task.FromResult(refused);

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Dialling.ClientCertificates.ToJSON())
                   );

        }

        /// <summary>
        /// POST /api/v1/configuration/certificates with {"subject", "algorithm"}:
        /// a new key, and the signing request to be handed to a certificate
        /// authority.
        /// </summary>
        /// <remarks>
        /// At the same permission as the name servers and the time server, and
        /// for the same reason: this is a statement about how the station
        /// reaches the outside world. It is not about what the equipment is or
        /// may deliver, and it is not about calibration - a certificate here
        /// says who this station is to a back end, and nothing about what it
        /// measures.
        /// </remarks>
        protected Task<HTTPResponse> PostClientKey(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(ConnectionsResource), true, out _, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (!Dialling.ClientCertificates.TryCreateKey(json.Value<String>("subject")   ?? "",
                                                         json.Value<String>("algorithm"),
                                                         out var id,
                                                         out var csr,
                                                         out var error,
                                                         out var notSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));
            }

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.Created,
                                    new JObject(
                                        new JProperty("id",           id),
                                        new JProperty("csr",          csr),
                                        new JProperty("certificates", Dialling.ClientCertificates.ToJSON())
                                    ))
                   );

        }

        /// <summary>
        /// POST /api/v1/configuration/certificates/import with {"pem"}: the
        /// certificate a certificate authority sent back, and whatever
        /// intermediates came with it.
        /// </summary>
        /// <remarks>
        /// What could not be used is refused here, while somebody is looking at
        /// the screen, rather than at the next handshake. What may legitimately
        /// look wrong from inside a station - a chain it cannot verify - comes
        /// back as a warning beside a certificate that was taken in.
        /// </remarks>
        protected Task<HTTPResponse> PostClientCertificate(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(ConnectionsResource), true, out _, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (!Dialling.ClientCertificates.TryAddCertificate(json.Value<String>("pem") ?? "",
                                                              out var id,
                                                              out var warnings,
                                                              out var error,
                                                              out var notSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));
            }

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK,
                                    new JObject(
                                        new JProperty("id",           id),
                                        new JProperty("warnings",     new JArray(warnings)),
                                        new JProperty("certificates", Dialling.ClientCertificates.ToJSON())
                                    ))
                   );

        }

        /// <summary>
        /// POST /api/v1/configuration/certificates/remove with {"id"}: take a
        /// key and everything belonging to it away, for good.
        /// </summary>
        /// <remarks>
        /// A POST rather than a DELETE because every other identification in
        /// this API travels in the body rather than in the path, and one route
        /// shaped differently from the rest is a route somebody gets wrong.
        /// </remarks>
        protected Task<HTTPResponse> PostRemoveClientKey(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(ConnectionsResource), true, out _, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (!Dialling.ClientCertificates.TryRemove(json.Value<String>("id") ?? "", out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Dialling.ClientCertificates.ToJSON())
                   );

        }

        /// <summary>
        /// What the server of a wss:// connection checked by its CA is held to,
        /// as it was sent under "pins" - with what the page showed it held to,
        /// under "pinsAsShown", where it says so - or null, where neither was
        /// sent, to keep what it is held to.
        /// </summary>
        /// <remarks>
        /// Whether it is a wss:// connection at all is asked where the URL is
        /// known. What is read here is whether the words are ones a server can
        /// be held to: the same ones, and refused with the same sentences, as
        /// those of a time server, a name server and a meter.
        /// </remarks>
        protected static Boolean TryPinsOf(JObject                           JSON,
                                         out ServerPinsChange?        Pins,
                                         [NotNullWhen(false)] out String?  Error)
        {

            Pins   = null;
            Error  = null;

            var sent   = JSON[ConnectionEntry.PinsKey];
            var shown  = JSON[ConnectionEntry.PinsAsShownKey];

            if (sent is null && shown is null)
                return true;

            if (!TryServerPins(sent,  ConnectionEntry.PinsKey,        out var sentPins,  out Error) ||
                !TryServerPins(shown, ConnectionEntry.PinsAsShownKey, out var shownPins, out Error))
            {
                return false;
            }

            Pins = new ServerPinsChange(sentPins, shownPins, shown is not null);
            return true;

        }

        protected static Boolean TryServerPins(JToken?                                              JSON,
                                             String                                               Path,
                                             out protocols.WWCP.Node.Configuration.ServerPins?    Pins,
                                             [NotNullWhen(false)] out String?                     Error)
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

            return protocols.WWCP.Node.Configuration.ServerPins.TryParse(entry, Path, out Pins, out Error);

        }

        /// <summary>
        /// What both pages are handed, in one place.
        /// </summary>
        protected JObject ConnectionsJSON()
        {

            var json = Dialling.Connections.ToJSON();

            // What this station made of the certificate the server of each
            // wss:// connection checked by its CA showed last: what its page
            // offers to hold it to, and says it came to.
            foreach (var connection in json["connections"]?.OfType<JObject>() ?? [])
            {

                // Which version of HTTP the WebSocket it has now was opened
                // over: where the server chose, the entry does not say.
                if (URL.TryParse(connection.Value<String>("url") ?? "", out var dialled) &&
                    Dialling.HTTPVersionInUseAt(dialled) is String httpVersionInUse)
                {
                    connection.Add(new JProperty("httpVersionInUse", httpVersionInUse));
                }

                // Which subprotocol it agreed on, and so how its messages are
                // written: where the back end chose, the entry does not say.
                // Over a WebSocket and over ssh:// alike.
                if (connection.Value<String>("id") is String connectionId &&
                    Dialling.Connections.Connections.FirstOrDefault(entry => entry.Id == connectionId) is ConnectionEntry itsEntry &&
                    Dialling.SubprotocolInUseOf(itsEntry) is String subprotocolInUse)
                {
                    connection.Add(new JProperty("subprotocolInUse", subprotocolInUse));
                }

                if (connection.Value<Boolean?>("secure")    != true ||
                    connection.Value<Boolean?>("keyPinned") == true ||
                    !URL.TryParse(connection.Value<String>("url") ?? "", out var url))
                {
                    continue;
                }

                if (Dialling.LastJudgementOf(Dialling.ConnectionsCertificateUsage, url.Host.ToString()) is { } judgement)
                    connection.Add(new JProperty("judgement", judgement.ToJSON()));

            }

            // Which certificates a connection may point at. Read from the
            // certificate store rather than remembered here, so that one
            // removed on the page next door is gone from this list too.
            // The key this station signs in with over SSH, for the line that
            // goes into a server's authorized_keys - null until the first
            // ssh:// connection made it.
            json.Add(new JProperty("sshClientKey", Dialling.SshKey.ToJSON()));

            json.Add(
                new JProperty("certificates",
                    new JArray(
                        Dialling.ClientCertificates.Entries.Select(entry =>
                            new JObject(
                                new JProperty("id",              entry.Id),
                                new JProperty("subject",         entry.Subject),
                                new JProperty("algorithm",       entry.Algorithm),
                                new JProperty("hasCertificate",  entry.Certificate is not null),
                                new JProperty("canBeHeldUp",     entry.CanBeHeldUp)
                            )))));

            // Where each connection stands, so that the page does not first
            // draw every connection without it and then again with it.
            json.Merge(ConnectionStatesJSON());

            return json;

        }

        /// <summary>
        /// Where each connection this station dialled stands, by
        /// identification, and what time it is here.
        /// </summary>
        /// <remarks>
        /// The time, because the page says how long ago and how soon, and the
        /// clock it would otherwise ask is the browser's - which is not this
        /// station's, and on a laptop in a car park not always right either.
        /// </remarks>
        protected JObject ConnectionStatesJSON()

            => new (
                   new JProperty("timestamp",  Dialling.TimeProvider.GetUtcNow().UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                   new JProperty("states",     new JObject(
                       Dialling.ConnectionStates.Select(entry => new JProperty(entry.Key, entry.Value.ToJSON()))
                   ))
               );

        /// <summary>
        /// The key this station signs in with over SSH, made where the
        /// connection just written down is an ssh:// one: its public half is
        /// what somebody has to hand the server before the first connection,
        /// and the page that just saved it is where it is shown.
        /// </summary>
        /// <remarks>
        /// A key that cannot be made does not undo what was written down: the
        /// connection stands, and says why when it is dialled.
        /// </remarks>
        protected async Task SshKeyIfNeeded(String? ConnectionId, CancellationToken CancellationToken)
        {

            if (Dialling.SshKey.Key is not null ||
                !Dialling.Connections.Connections.Any(entry => entry.Id == ConnectionId && entry.IsSSH))
                return;

            try
            {
                await Dialling.SshKey.MakeIfNeeded(CancellationToken);
            }
            catch (Exception e)
            {
                Log.Warning($"The key this station signs in with over SSH could not be made: {e.Message}", "ocpp", "connections", "ssh");
            }

        }

    }

}
