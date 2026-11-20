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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.Serving;
using cloud.charging.open.protocols.WWCP.Node.Certificates;
using cloud.charging.open.protocols.WWCP.Node.Web;
using cloud.charging.open.protocols.WWCP.Node.Logins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The part of the JSON API that is about the charging station server: its
    /// socket, the chains it accepts and the charging stations that may sign
    /// in. What it presents is the node's certificate store's - TLS server
    /// identities told the listener "ocpp", and their server keys.
    /// </summary>
    /// <remarks>
    /// Three pages worth of routes, in three groups, and the groups are not the
    /// same resource. Reading is reading; changing the server and the list of
    /// charging stations is "stations", a day's work on a site; generating a
    /// key and naming a certificate authority is "certificates", which is
    /// neither. Which resource "stations" is, is the kind's to say - see
    /// <see cref="StationsResource"/>.
    /// </remarks>
    public abstract partial class OCPPServingHTTPAPI
    {

        #region (private) RegisterOCPPServerRoutes()

        /// <summary>
        /// Everything under /v1/configuration/ocpp-server.
        /// </summary>
        private void RegisterOCPPServerRoutes()
        {

            var root = HTTPPath.Root + "v1/configuration/ocpp-server";

            AddHandler(root,                          GetOCPPServer,            HTTPMethod.GET);
            AddHandler(root,                          PutOCPPServer,            HTTPMethod.PUT);

            // What the station port presents is the node's: TLS server
            // identities of its store told the listener "ocpp", and the server
            // keys under /v1/certificates/serverKeys - see WWCP_Node.

            AddHandler(root + "trust",                GetTrust,                 HTTPMethod.GET);
            AddHandler(root + "trust",                PostTrust,                HTTPMethod.POST);
            AddHandler(root + "trust/{id}",           PutTrust,                 HTTPMethod.PUT);
            AddHandler(root + "trust/{id}",           DeleteTrust,              HTTPMethod.DELETE);

            AddHandler(root + "groups",               PostGroup,                HTTPMethod.POST);
            AddHandler(root + "groups/{id}",          PutGroup,                 HTTPMethod.PUT);
            AddHandler(root + "groups/{id}",          DeleteGroup,              HTTPMethod.DELETE);

            AddHandler(root + "logins",             GetStations,              HTTPMethod.GET);
            AddHandler(root + "logins",             PostStation,              HTTPMethod.POST);
            AddHandler(root + "logins/{id}",        PutStation,               HTTPMethod.PUT);
            AddHandler(root + "logins/{id}",        DeleteStation,            HTTPMethod.DELETE);

            AddHandler(root + "logins/{id}/totp",     PutStationTOTP,         HTTPMethod.PUT);
            AddHandler(root + "logins/{id}/totp",     DeleteStationTOTP,      HTTPMethod.DELETE);
            AddHandler(root + "logins/{id}/totp/binding", PutStationTOTPBinding, HTTPMethod.PUT);
            AddHandler(root + "logins/{id}/password", DeleteStationPassword,  HTTPMethod.DELETE);

            AddHandler(root + "logins/{id}/sshKeys",               PostStationSSHKey,    HTTPMethod.POST);
            AddHandler(root + "logins/{id}/sshKeys/{fingerprint}", DeleteStationSSHKey,  HTTPMethod.DELETE);

            AddHandler(root + "logins/{id}/keyPins",               PostStationKeyPin,    HTTPMethod.POST);
            AddHandler(root + "logins/{id}/keyPins/{pin}",         DeleteStationKeyPin,  HTTPMethod.DELETE);

        }

        #endregion


        #region (private) GetOCPPServer(Request) / PutOCPPServer(Request)

        /// <summary>
        /// GET /api/v1/configuration/ocpp-server: the server the charging
        /// stations connect to.
        /// </summary>
        private Task<HTTPResponse> GetOCPPServer(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Read(StationsResource), false, out _, out var refused))
                return Task.FromResult(refused);

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.OCPPServerConfigurationJSON())
                   );

        }

        /// <summary>
        /// PUT /api/v1/configuration/ocpp-server: change it.
        /// </summary>
        /// <remarks>
        /// Answers with the whole thing as it now stands - including
        /// <c>state.waitingForARestart</c>, which is how the page finds out
        /// that the port it just changed is not the port that is listening.
        /// </remarks>
        private Task<HTTPResponse> PutOCPPServer(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (!Serving.TryUpdateOCPPServerConfiguration(json, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));

            Log.Info($"'{user.Id}' changed the charging station server.", "ocpp", "station", "config", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.OCPPServerConfigurationJSON())
                   );

        }

        #endregion

        #region (private) The chains this controller accepts

        /// <summary>
        /// GET .../trust: which certificate authorities a charging station may
        /// be vouched for by.
        /// </summary>
        private Task<HTTPResponse> GetTrust(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Read(NodeResources.Certificates), false, out _, out var refused))
                return Task.FromResult(refused);

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.ClientTrust.ToJSON())
                   );

        }

        /// <summary>
        /// POST .../trust with {"pem", "name"}: accept charging stations whose
        /// certificate leads to the one in this file.
        /// </summary>
        private Task<HTTPResponse> PostTrust(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(NodeResources.Certificates), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            var pem = json.Value<String>("pem") ?? "";

            if (pem.Length == 0)
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest, "A 'pem' with the certificate authority in it is required."));

            if (!Serving.ClientTrust.TryAdd(pem, json.Value<String>("name"), out var id, out var warnings, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));

            Log.Notice($"'{user.Id}' added the accepted chain '{id}'.", "ocpp", "tls", "trust", "web");

            return Task.FromResult(
                       JSONResponse(
                           Request,
                           HTTPStatusCode.Created,
                           new JObject(
                               new JProperty("id",        id),
                               new JProperty("warnings",  new JArray(warnings))
                           )
                       )
                   );

        }

        /// <summary>
        /// PUT .../trust/{id} with {"enabled"} and/or {"name"}: switch a chain
        /// off without losing it, or give it another name.
        /// </summary>
        private Task<HTTPResponse> PutTrust(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(NodeResources.Certificates), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (json.Value<String>("name") is { Length: > 0 } name &&
                !Serving.ClientTrust.TryRename(id, name, out var renameError, out var renameNotSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, renameError, renameNotSaved));

            if (json.Value<Boolean?>("enabled") is Boolean enabled &&
                !Serving.ClientTrust.TrySetEnabled(id, enabled, out var enableError, out var enableNotSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, enableError, enableNotSaved));

            Log.Info($"'{user.Id}' changed the accepted chain '{id}'.", "ocpp", "tls", "trust", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.ClientTrust.ToJSON())
                   );

        }

        /// <summary>
        /// DELETE .../trust/{id}: stop accepting stations that lead to it.
        /// </summary>
        private Task<HTTPResponse> DeleteTrust(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(NodeResources.Certificates), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (!Serving.ClientTrust.TryRemove(id, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.NotFound, error, notSaved));

            Log.Notice($"'{user.Id}' removed the accepted chain '{id}'.", "ocpp", "tls", "trust", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.ClientTrust.ToJSON())
                   );

        }

        #endregion

        #region (private) The charging stations that may sign in

        /// <summary>
        /// GET .../logins: which charging stations may sign in.
        /// </summary>
        private Task<HTTPResponse> GetStations(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Read(StationsResource), false, out _, out var refused))
                return Task.FromResult(refused);

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.WebSocketLogins.ToJSON())
                   );

        }

        /// <summary>
        /// What a charging station or a login group is, as it was sent under
        /// "description": see <see cref="ConnectionFields.TryDescriptionOf"/>.
        /// </summary>
        private static Boolean TryDescriptionOf(JObject                           JSON,
                                                out I18NString?                   Description,
                                                [NotNullWhen(false)] out String?  Error)

            => ConnectionFields.TryDescriptionOf(JSON, out Description, out Error);

        /// <summary>
        /// POST .../logins with {"id", "password", "group", "description"}: add a
        /// charging station, or give one a new password.
        /// </summary>
        /// <remarks>
        /// The password is given: none is made up here. It is kept only as a
        /// hash, so whoever sets it has to know it to type it into the
        /// charging station.
        /// </remarks>
        private Task<HTTPResponse> PostStation(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            var id = json.Value<String>("id") ?? "";

            if (!TryDescriptionOf(json, out var description, out var wrongDescription))
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest, wrongDescription));

            if (!Serving.WebSocketLogins.TrySetPasswordAuthentication(
                     id,
                     json.Value<String>("password"),
                     json.Value<String>("group"),
                     description,
                     out var error,
                     out var notSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));
            }

            Log.Notice($"'{user.Id}' set the password of the charging station '{id}'.", "ocpp", "station", "auth", "web");

            var response = new JObject(
                               new JProperty("id",        id),
                               new JProperty("logins",    Serving.WebSocketLogins.ToJSON())
                           );

            return Task.FromResult(JSONResponse(Request, HTTPStatusCode.OK, response));

        }

        /// <summary>
        /// PUT .../logins/{id} with {"enabled"}, {"group"},
        /// {"localController"} and/or {"gateway"}: let a charging station in,
        /// stop letting it in, move it to another group, or mark it as a local
        /// controller's or a gateway's login, which may send in the name of
        /// the stations behind it - without losing what it signs in with. A
        /// login is the one or the other: true for both is refused.
        /// </summary>
        private Task<HTTPResponse> PutStation(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            var enabled          = json.Value<Boolean?>("enabled");
            var group            = json.Value<String>("group");
            var marking          = json["localController"];
            var gatewayMarking   = json["gateway"];

            if (marking is not null && marking.Type is not (JTokenType.Boolean or JTokenType.Null))
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest, "'localController' is true or false."));

            if (gatewayMarking is not null && gatewayMarking.Type is not (JTokenType.Boolean or JTokenType.Null))
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest, "'gateway' is true or false."));

            var localController  = marking?.Type == JTokenType.Boolean
                                       ? marking.Value<Boolean>()
                                       : (Boolean?) null;

            var gateway          = gatewayMarking?.Type == JTokenType.Boolean
                                       ? gatewayMarking.Value<Boolean>()
                                       : (Boolean?) null;

            if (enabled is null && group is null && localController is null && gateway is null)
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest, "An 'enabled' of true or false, a 'group', or a 'localController' or a 'gateway' of true or false, is required."));

            if (localController == true && gateway == true)
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest, "A login is a local controller's or a gateway's, not both."));

            if (group is not null &&
                !Serving.WebSocketLogins.TrySetGroup(id, group, out var groupError, out var groupNotSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.NotFound, groupError, groupNotSaved));
            }

            if (enabled is Boolean wanted &&
                !Serving.WebSocketLogins.TrySetEnabled(id, wanted, out var error, out var notSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.NotFound, error, notSaved));
            }

            if (localController is Boolean marked &&
                !Serving.WebSocketLogins.TrySetLocalController(id, marked, out var markError, out var markNotSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.NotFound, markError, markNotSaved));
            }

            // After the local controller's mark: marking the one takes the
            // other away, and both true was refused above.
            if (gateway is Boolean markedAsGateway &&
                !Serving.WebSocketLogins.TrySetGateway(id, markedAsGateway, out var gatewayError, out var gatewayNotSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.NotFound, gatewayError, gatewayNotSaved));
            }

            if (localController is Boolean isController)
                Log.Notice(isController
                               ? $"'{user.Id}' marked '{id}' as a local controller."
                               : $"'{user.Id}' took the mark of a local controller from '{id}'.",
                           "ocpp", "station", "security", "web");

            if (gateway is Boolean isGateway)
                Log.Notice(isGateway
                               ? $"'{user.Id}' marked '{id}' as a gateway."
                               : $"'{user.Id}' took the mark of a gateway from '{id}'.",
                           "ocpp", "station", "security", "web");

            if (group is not null)
                Log.Info($"'{user.Id}' moved the charging station '{id}' into the login group '{group}'.", "ocpp", "station", "auth", "web");

            if (enabled is Boolean said)
                Log.Info($"'{user.Id}' {(said ? "let in" : "shut out")} the charging station '{id}'.", "ocpp", "station", "auth", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.WebSocketLogins.ToJSON())
                   );

        }

        /// <summary>
        /// PUT .../logins/{id}/totp with {"sharedSecret", "validitySeconds",
        /// "length", "alphabet", "hashAlgorithm", "group", "description"}: let a
        /// charging station in with a one-time token.
        /// </summary>
        /// <remarks>
        /// An empty shared secret means "make one up", and it comes back in
        /// this response. Unlike a password it does not become unreadable
        /// afterwards - it cannot, because the controller has to compute tokens
        /// from it - but it is never put into the list the page reads, so this
        /// is still the only place it is handed out.
        /// </remarks>
        private Task<HTTPResponse> PutStationTOTP(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            TOTPHashAlgorithm? hash = null;

            if (json.Value<String>("hashAlgorithm")?.Trim() is { Length: > 0 } hashText)
            {

                hash = hashText.ToUpperInvariant() switch {
                           "SHA256"  => TOTPHashAlgorithm.SHA256,
                           "SHA384"  => TOTPHashAlgorithm.SHA384,
                           "SHA512"  => TOTPHashAlgorithm.SHA512,
                           _         => null
                       };

                if (hash is null)
                    return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest,
                                                     $"'{hashText}' is not a TOTP hash algorithm; there are SHA256, SHA384 and SHA512."));

            }

            if (!TryDescriptionOf(json, out var description, out var wrongDescription))
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest, wrongDescription));

            var seconds      = json.Value<Double?>("validitySeconds");
            var sharedSecret = json.Value<String>("sharedSecret");

            if (!Serving.WebSocketLogins.TrySetTOTPAuthentication(
                     id,
                     sharedSecret,
                     seconds.HasValue ? TimeSpan.FromSeconds(seconds.Value) : null,
                     json.Value<UInt32?>("length"),
                     json.Value<String>("alphabet"),
                     hash,
                     json.Value<String>("group"),
                     description,
                     out var error,
                     out var notSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));
            }

            Log.Notice($"'{user.Id}' set the TOTP configuration of the charging station '{id}'.", "ocpp", "station", "auth", "web");

            var response = new JObject(
                               new JProperty("id",        id),
                               new JProperty("logins",    Serving.WebSocketLogins.ToJSON())
                           );

            // None was sent, so one was made up: shown once here, as somebody
            // is standing in front of the charging station now.
            if (String.IsNullOrWhiteSpace(sharedSecret) &&
                Serving.WebSocketLogins.TryGet(id, out var login) &&
                login.TOTP is TOTPSettings totp)
            {
                response.Add("sharedSecret", totp.SharedSecret);
            }

            return Task.FromResult(JSONResponse(Request, HTTPStatusCode.OK, response));

        }

        /// <summary>
        /// PUT .../logins/{id}/totp/binding with {"tlsChannelBinding": true
        /// or false}: whether the one-time token of a charging station has to
        /// be bound to the TLS session of its connection.
        /// </summary>
        /// <remarks>
        /// Its own request, so that it leaves the shared secret alone: the
        /// secret cannot be read back, and setting the token anew to change
        /// this would mean carrying a new secret to the station.
        /// </remarks>
        private Task<HTTPResponse> PutStationTOTPBinding(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (json["tlsChannelBinding"] is not JValue { Type: JTokenType.Boolean } binding)
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest,
                                                 "Say whether the token has to be bound: { \"tlsChannelBinding\": true } or false."));

            var bound = binding.Value<Boolean>();

            if (!Serving.WebSocketLogins.TrySetTOTPBinding(id, bound, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.NotFound, error, notSaved));

            Log.Notice($"'{user.Id}' {(bound ? "asked for" : "no longer asks for")} the one-time token of the charging station '{id}' to be bound to its connection.",
                       "ocpp", "station", "auth", "web");

            return Task.FromResult(JSONResponse(Request, HTTPStatusCode.OK, Serving.WebSocketLogins.ToJSON()));

        }

        /// <summary>
        /// DELETE .../logins/{id}/totp: stop letting a charging station in
        /// with a one-time token.
        /// </summary>
        private Task<HTTPResponse> DeleteStationTOTP(HTTPRequest Request)

            => Task.FromResult(TakeCredentialAway(Request,
                                                  Serving.WebSocketLogins.TryClearTOTP,
                                                  "TOTP configuration"));

        /// <summary>
        /// DELETE .../logins/{id}/password: stop letting a charging station
        /// in with a password.
        /// </summary>
        private Task<HTTPResponse> DeleteStationPassword(HTTPRequest Request)

            => Task.FromResult(TakeCredentialAway(Request,
                                                  Serving.WebSocketLogins.TryClearPassword,
                                                  "password"));

        /// <summary>
        /// POST .../logins/{id}/sshKeys with {"line", "label", "group",
        /// "description"}: give a charging station an SSH key for OCPP over SSH,
        /// adding the station where it is not listed yet.
        /// </summary>
        /// <remarks>
        /// The line is a public key, as one line of an authorized_keys file;
        /// nothing about it is secret, and the answer is the whole list as
        /// every other change here answers.
        /// </remarks>
        private Task<HTTPResponse> PostStationSSHKey(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (!TryDescriptionOf(json, out var description, out var wrongDescription))
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest, wrongDescription));

            if (!Serving.TryAddStationSSHKey(
                     id,
                     json.Value<String>("line"),
                     json.Value<String>("label"),
                     json.Value<String>("group"),
                     description,
                     user.Id.ToString(),
                     out var key,
                     out var error,
                     out var notSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));
            }

            Log.Notice($"'{user.Id}' gave the charging station '{id}' the SSH key {key}.", "ocpp", "station", "auth", "ssh", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, new JObject(
                           new JProperty("id",           id),
                           new JProperty("fingerprint",  key.Fingerprint),
                           new JProperty("logins",       Serving.WebSocketLogins.ToJSON())
                       ))
                   );

        }

        /// <summary>
        /// DELETE .../logins/{id}/sshKeys/{fingerprint}: take an SSH key away
        /// from a charging station. A session it opened ends.
        /// </summary>
        /// <remarks>
        /// The fingerprint as OpenSSH writes it, "SHA256:...", escaped - or
        /// without its "SHA256:" and in the URL-safe Base64 alphabet, as the
        /// page sends it.
        /// </remarks>
        private Task<HTTPResponse> DeleteStationSSHKey(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (Request.ParsedURLParameters.ElementAtOrDefault(1)?.Trim() is not { Length: > 0 } fingerprintText)
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest, "The fingerprint of the key is required."));

            var fingerprint = UserSSHKey.FingerprintFromURL(fingerprintText);

            if (!Serving.WebSocketLogins.TryRemoveSSHKey(id, fingerprint, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.NotFound, error, notSaved));

            Log.Notice($"'{user.Id}' took the SSH key {fingerprint} of the charging station '{id}' away.", "ocpp", "station", "auth", "ssh", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.WebSocketLogins.ToJSON())
                   );

        }

        /// <summary>
        /// POST .../logins/{id}/keyPins with {"pin", "label", "group",
        /// "description"}: let a charging station in by the key pin of its TLS client
        /// key, adding the station where it is not listed yet.
        /// </summary>
        /// <remarks>
        /// The pin is SHA-256 over the station's public key - 64 hexadecimal
        /// digits, with or without colons, or "sha256//" and base64 - as the
        /// station shows it at commissioning. Nothing about it is secret.
        /// </remarks>
        private Task<HTTPResponse> PostStationKeyPin(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (!TryDescriptionOf(json, out var description, out var wrongDescription))
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest, wrongDescription));

            if (!Serving.WebSocketLogins.TryAddKeyPin(
                     id,
                     json.Value<String>("pin"),
                     json.Value<String>("label"),
                     json.Value<String>("group"),
                     description,
                     user.Id.ToString(),
                     out var keyPin,
                     out var error,
                     out var notSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));
            }

            Log.Notice($"'{user.Id}' gave the charging station '{id}' the key pin {keyPin.Pin}.", "ocpp", "station", "auth", "tls", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, new JObject(
                           new JProperty("id",        id),
                           new JProperty("keyPin",    keyPin.Pin.ToHex()),
                           new JProperty("logins",    Serving.WebSocketLogins.ToJSON())
                       ))
                   );

        }

        /// <summary>
        /// DELETE .../logins/{id}/keyPins/{pin}: stop letting a charging
        /// station in by a key pin. A connection it opened stays open; the next
        /// one is refused.
        /// </summary>
        /// <remarks>
        /// The pin as 64 hexadecimal digits, which is what the page sends: the
        /// other notations hold a '/' or ':', which a path is better without.
        /// </remarks>
        private Task<HTTPResponse> DeleteStationKeyPin(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (Request.ParsedURLParameters.ElementAtOrDefault(1)?.Trim() is not { Length: > 0 } pin)
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest, "The key pin is required."));

            if (!Serving.WebSocketLogins.TryRemoveKeyPin(id, pin, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.NotFound, error, notSaved));

            Log.Notice($"'{user.Id}' took the key pin {pin} of the charging station '{id}' away.", "ocpp", "station", "auth", "tls", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.WebSocketLogins.ToJSON())
                   );

        }

        /// <summary>
        /// What the two routes above have in common.
        /// </summary>
        private HTTPResponse TakeCredentialAway(HTTPRequest       Request,
                                                TryClearDelegate  Clear,
                                                String            What)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return refused;

            if (!TryGetId(Request, out var id, out var badRequest))
                return badRequest;

            if (!Clear(id, out var error, out var notSaved))
                return NotChanged(Request, HTTPStatusCode.NotFound, error, notSaved);

            Log.Notice($"'{user.Id}' took the {What} of the charging station '{id}' away.", "ocpp", "station", "auth", "web");

            return JSONResponse(Request, HTTPStatusCode.OK, Serving.WebSocketLogins.ToJSON());

        }

        /// <summary>
        /// One of the store's "take this credential away" methods, with what
        /// they promise: the error where they refuse. Without it, the compiler
        /// could not tell that the error handed on above is never null.
        /// </summary>
        private delegate Boolean TryClearDelegate(String                            Id,
                                                  [NotNullWhen(false)] out String?  Error,
                                                  out Boolean                       NotSaved);

        /// <summary>
        /// DELETE .../logins/{id}: forget a charging station.
        /// </summary>
        private Task<HTTPResponse> DeleteStation(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (!Serving.WebSocketLogins.TryRemove(id, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.NotFound, error, notSaved));

            // What the kind of node forgets with it - a CSMS, the certificates
            // it knows of the station.
            var alsoForgotten = Serving.StationLoginRemoved(id);

            Log.Notice($"'{user.Id}' removed the charging station '{id}'{alsoForgotten}.", "ocpp", "station", "auth", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.WebSocketLogins.ToJSON())
                   );

        }

        #endregion

        #region (private) The groups that decide what a login may do

        /// <summary>
        /// POST .../groups with {"id", "name", "enabled", "authMethods",
        /// "securityProfiles", "sshChannels", "description"}: make a login group.
        /// </summary>
        private Task<HTTPResponse> PostGroup(HTTPRequest Request)
        {

            // Asked before the body is looked at, like every other route here:
            // somebody who may not change anything should be told that and not
            // what this controller thinks of their JSON.
            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out _, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            return Task.FromResult(WriteGroup(Request, json.Value<String>("id"), json));

        }

        /// <summary>
        /// PUT .../groups/{id}: change what a login group allows.
        /// </summary>
        private Task<HTTPResponse> PutGroup(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out _, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            return Task.FromResult(WriteGroup(Request, id, json));

        }

        /// <summary>
        /// Making a group and changing one are the same write; only where the
        /// identification comes from differs.
        /// </summary>
        /// <remarks>
        /// Everything about a group is replaced, not merged. A form that offers
        /// the whole group sends the whole group, and merging would make
        /// clearing the last authentication method impossible - which is
        /// exactly the move somebody makes to shut a group out.
        /// </remarks>
        private HTTPResponse WriteGroup(HTTPRequest  Request,
                                        String?      Id,
                                        JObject      JSON)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return refused;

            #region The ways in it accepts

            var methods = new List<AuthMethod>();

            if (JSON["authMethods"] is JArray methodArray)
                foreach (var token in methodArray)
                {

                    var method = AuthMethodExtensions.TryParseAuthMethod(token.Value<String>());

                    if (method is null)
                        return ErrorJSON(Request, HTTPStatusCode.BadRequest,
                                         $"'{token}' is not a way of signing in that this {Node.NodeNoun} knows; " +
                                          "there are basic, totp, certificate, sshkey and keypin.");

                    methods.Add(method.Value);

                }

            #endregion

            #region How its members may authenticate - OCPP's security profiles

            // What a group said of the transport before: refused rather than
            // passed over, which would leave its members with nothing.
            if (JSON.ContainsKey("securityProfiles"))
                return ErrorJSON(Request, HTTPStatusCode.BadRequest,
                                 "'securityProfiles' of a login group is no more: say how its members may authenticate in 'clientAuthentication', " +
                                 $"by {String.Join(", ", WebSocketClientAuthentications.Names.Select(name => $"\"{name}\""))}.");

            var authentications = new List<WebSocketClientAuthentication>();

            if (JSON["clientAuthentication"] is JArray authenticationArray)
                foreach (var token in authenticationArray)
                {

                    if (token.Type != JTokenType.String ||
                        !WebSocketClientAuthentications.TryParse(token.Value<String>(), out var authentication))
                        return ErrorJSON(Request, HTTPStatusCode.BadRequest,
                                         $"'{token}' is no way a client may authenticate; they are {String.Join(", ", WebSocketClientAuthentications.Names)}.");

                    authentications.Add(authentication);

                }

            #endregion

            #region What its stations have over SSH besides OCPP

            // Replaced like everything else about a group: a form that sends
            // none of it takes it away.
            var channels = new List<String>();

            if (JSON["sshChannels"] is JToken said)
            {

                if (said is not JArray channelArray ||
                    channelArray.Any(token => token.Type != JTokenType.String))
                    return ErrorJSON(Request, HTTPStatusCode.BadRequest, "'sshChannels' must be a list of the names of SSH subsystems.");

                channels.AddRange(channelArray.Select(token => token.Value<String>()!));

            }

            #endregion

            if (!TryDescriptionOf(JSON, out var description, out var wrongDescription))
                return ErrorJSON(Request, HTTPStatusCode.BadRequest, wrongDescription);

            if (!Serving.WebSocketLogins.TryAddOrUpdateGroup(
                     Id,
                     JSON.Value<String>("name"),
                     JSON.Value<Boolean?>("enabled") ?? true,
                     methods,
                     authentications,
                     description,
                     channels,
                     out var error,
                     out var notSaved))
            {
                return NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved);
            }

            Log.Notice($"'{user.Id}' wrote the login group '{Id}'.", "ocpp", "station", "auth", "web");

            return JSONResponse(Request, HTTPStatusCode.OK, Serving.WebSocketLogins.ToJSON());

        }

        /// <summary>
        /// DELETE .../groups/{id}: forget a login group nobody is in.
        /// </summary>
        private Task<HTTPResponse> DeleteGroup(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (!Serving.WebSocketLogins.TryRemoveGroup(id, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.Conflict, error, notSaved));

            Log.Notice($"'{user.Id}' removed the login group '{id}'.", "ocpp", "station", "auth", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.WebSocketLogins.ToJSON())
                   );

        }

        #endregion

        #region (private static) TryGetId(Request, out Id, out ErrorResponse)

        /// <summary>
        /// The identification out of the path, or the 400 that says there was
        /// none.
        /// </summary>
        private static Boolean TryGetId(HTTPRequest         Request,
                                        out String          Id,
                                        out HTTPResponse    ErrorResponse)
        {

            Id             = Request.ParsedURLParameters.FirstOrDefault()?.Trim() ?? "";
            ErrorResponse  = default!;

            if (Id.Length == 0)
            {
                ErrorResponse = ErrorJSON(Request, HTTPStatusCode.BadRequest, "An identification is required.");
                return false;
            }

            return true;

        }

        #endregion

    }

}
