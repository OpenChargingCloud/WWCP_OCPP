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

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP.Node.Connections;
using cloud.charging.open.protocols.OCPP.Node.Serving;
using cloud.charging.open.protocols.WWCP.Node.Certificates;
using cloud.charging.open.protocols.WWCP.Node.Web;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The part of the JSON API that is about the charging station server: its
    /// socket, the certificates it presents, the chains it accepts and the
    /// charging stations that may sign in.
    /// </summary>
    /// <remarks>
    /// Four pages worth of routes, in four groups, and the groups are not the
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

            AddHandler(root + "certificates",         GetCertificates,          HTTPMethod.GET);
            AddHandler(root + "certificates",         PostCertificateRequest,   HTTPMethod.POST);
            AddHandler(root + "certificates/inspect", PostCertificateInspect,   HTTPMethod.POST);
            AddHandler(root + "certificates/upload",  PostCertificateUpload,    HTTPMethod.POST);
            AddHandler(root + "certificates/{id}/csr", GetCertificateRequest,   HTTPMethod.GET);
            AddHandler(root + "certificates/{id}",    PutCertificate,           HTTPMethod.PUT);
            AddHandler(root + "certificates/{id}",    DeleteCertificate,        HTTPMethod.DELETE);

            AddHandler(root + "serverKeys",           PostServerKey,            HTTPMethod.POST);
            AddHandler(root + "serverKeys/switch",    PostServerKeySwitch,      HTTPMethod.POST);

            AddHandler(root + "trust",                GetTrust,                 HTTPMethod.GET);
            AddHandler(root + "trust",                PostTrust,                HTTPMethod.POST);
            AddHandler(root + "trust/{id}",           PutTrust,                 HTTPMethod.PUT);
            AddHandler(root + "trust/{id}",           DeleteTrust,              HTTPMethod.DELETE);

            AddHandler(root + "groups",               PostGroup,                HTTPMethod.POST);
            AddHandler(root + "groups/{id}",          PutGroup,                 HTTPMethod.PUT);
            AddHandler(root + "groups/{id}",          DeleteGroup,              HTTPMethod.DELETE);

            AddHandler(root + "stations",             GetStations,              HTTPMethod.GET);
            AddHandler(root + "stations",             PostStation,              HTTPMethod.POST);
            AddHandler(root + "stations/{id}",        PutStation,               HTTPMethod.PUT);
            AddHandler(root + "stations/{id}",        DeleteStation,            HTTPMethod.DELETE);

            AddHandler(root + "stations/{id}/totp",     PutStationTOTP,         HTTPMethod.PUT);
            AddHandler(root + "stations/{id}/totp",     DeleteStationTOTP,      HTTPMethod.DELETE);
            AddHandler(root + "stations/{id}/totp/binding", PutStationTOTPBinding, HTTPMethod.PUT);
            AddHandler(root + "stations/{id}/password", DeleteStationPassword,  HTTPMethod.DELETE);

            AddHandler(root + "stations/{id}/sshKeys",               PostStationSSHKey,    HTTPMethod.POST);
            AddHandler(root + "stations/{id}/sshKeys/{fingerprint}", DeleteStationSSHKey,  HTTPMethod.DELETE);

            AddHandler(root + "stations/{id}/keyPins",               PostStationKeyPin,    HTTPMethod.POST);
            AddHandler(root + "stations/{id}/keyPins/{pin}",         DeleteStationKeyPin,  HTTPMethod.DELETE);

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

        #region (private) The certificates this controller presents

        /// <summary>
        /// GET .../certificates: the keys, their signing requests and the
        /// certificates that answered them.
        /// </summary>
        private Task<HTTPResponse> GetCertificates(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Read(NodeResources.Certificates), false, out _, out var refused))
                return Task.FromResult(refused);

            // Recomputed before it is shown: what is wrong with a certificate
            // depends on the names this controller is reachable under and on
            // what day it is, and both change without the certificate moving.
            Serving.ServerCertificates.CheckExpiry(Serving.OCPPServerSettings.ReachableAs ?? []);

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.ServerCertificates.ToJSON())
                   );

        }

        /// <summary>
        /// POST .../certificates with {"subject", "algorithm"}: generate a key
        /// pair and the signing request that goes with it.
        /// </summary>
        /// <remarks>
        /// The request comes back in the response as well as being kept, so
        /// that the page can offer it for download without a second round trip.
        /// The private key does not come back, and there is no route that would
        /// hand it out.
        /// </remarks>
        private Task<HTTPResponse> PostCertificateRequest(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(NodeResources.Certificates), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            var reachableAs = Serving.OCPPServerSettings.ReachableAs ?? [];

            // Asked first, because without it the store would refuse for the
            // want of a subject - and the subject is not what is missing.
            if (reachableAs.Count == 0)
                return Task.FromResult(
                           ErrorJSON(
                               Request,
                               HTTPStatusCode.BadRequest,
                               $"This {NodeWording.Noun} has not been told what it is reachable as, and a signing request without " +
                               "those names produces a certificate no charging station will accept. Fill in 'reachable as' first."
                           )
                       );

            if (!Serving.ServerCertificates.TryCreateKey(
                     json.Value<String>("subject") ?? (reachableAs.Count > 0 ? reachableAs[0] : ""),
                     reachableAs,
                     json.Value<String>("algorithm"),
                     out var id,
                     out var csr,
                     out var error,
                     out var notSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));
            }

            Log.Notice($"'{user.Id}' had this {NodeWording.Noun} generate the key '{id}'.", "ocpp", "tls", "web");

            return Task.FromResult(
                       JSONResponse(
                           Request,
                           HTTPStatusCode.Created,
                           new JObject(
                               new JProperty("id",   id),
                               new JProperty("csr",  csr)
                           )
                       )
                   );

        }

        /// <summary>
        /// GET .../certificates/{id}/csr: the signing request, as a file to
        /// hand to a certificate authority.
        /// </summary>
        private Task<HTTPResponse> GetCertificateRequest(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Read(NodeResources.Certificates), false, out _, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (!Serving.ServerCertificates.TryReadCSR(id, out var csr, out var error))
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.NotFound, error));

            return Task.FromResult(
                       new HTTPResponse.Builder(Request) {
                           HTTPStatusCode  = HTTPStatusCode.OK,
                           ContentType     = HTTPContentType.Text.PLAIN,
                           Content         = Encoding.UTF8.GetBytes(csr),
                           CacheControl    = "no-store",
                           ContentDisposition = HTTPContentDisposition.Attachment($"{id}.csr.pem")
                       }.WithCommonSecurityHeaders().AsImmutable
                   );

        }

        /// <summary>
        /// PUT .../certificates/{id} with {"pem"}: take in the certificate that
        /// answers a signing request, and whatever intermediates came with it.
        /// </summary>
        /// <remarks>
        /// The identification in the path is what the caller believes it is
        /// uploading for; which key the certificate actually belongs to is
        /// worked out from the certificate itself, and a mismatch is an error
        /// rather than a silent filing under the right name. Somebody who
        /// uploads the wrong file to the right row should be told.
        /// </remarks>
        private Task<HTTPResponse> PutCertificate(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(NodeResources.Certificates), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            var pem = json.Value<String>("pem") ?? "";

            if (pem.Length == 0)
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest, "A 'pem' with the certificate in it is required."));

            if (!Serving.ServerCertificates.TryAddCertificate(
                     pem,
                     Serving.OCPPServerSettings.ReachableAs ?? [],
                     out var actual,
                     out var warnings,
                     out var error,
                     out var notSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));
            }

            if (actual != id)
                return Task.FromResult(
                           ErrorJSON(
                               Request,
                               HTTPStatusCode.Conflict,
                               $"This certificate belongs to the key '{actual}', not to '{id}'. It has been taken in under '{actual}'."
                           )
                       );

            Log.Notice($"'{user.Id}' uploaded a certificate for the key '{actual}'.", "ocpp", "tls", "web");

            return Task.FromResult(
                       JSONResponse(
                           Request,
                           HTTPStatusCode.OK,
                           new JObject(
                               new JProperty("id",        actual),
                               new JProperty("warnings",  new JArray(warnings))
                           )
                       )
                   );

        }

        /// <summary>
        /// POST .../certificates/inspect with {"pem"} or {"content", "password"}:
        /// what a text or a file holds, certificate by certificate, and which
        /// key of this controller each belongs to - without taking anything in.
        /// </summary>
        /// <remarks>
        /// A private key that came along is not handed back: the key of a
        /// certificate this controller presents is the one it generated, and
        /// none arrives. The certificate it came with is said to be refused for
        /// it, and the text that goes back into the page's box is the
        /// certificates alone.
        /// </remarks>
        private Task<HTTPResponse> PostCertificateInspect(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(NodeResources.Certificates), true, out _, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (!TryReadContent(json, out var bytes, out var contentError))
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest, contentError));

            var inspection = CertificateStore.Inspect(bytes, json.Value<String>("password"));

            if (inspection.Error is not null)
                return Task.FromResult(
                           ErrorJSON(Request, HTTPStatusCode.BadRequest, inspection.Error,
                                     inspection.PasswordWanted ? new JProperty("passwordWanted", true) : null)
                       );

            var found = inspection.Certificates.Select(certificate => (certificate, keyId: KeyOf(certificate))).ToList();

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, new JObject(
                           new JProperty("certificates", new JArray(found.Select(one => new JObject(
                               new JProperty("id",             one.certificate.Id),
                               new JProperty("thumbprint",     one.certificate.Thumbprint),
                               new JProperty("label",          one.certificate.CommonName),
                               new JProperty("subject",        one.certificate.Subject),
                               new JProperty("issuer",         one.certificate.Issuer),
                               new JProperty("notBefore",      one.certificate.NotBefore.UtcDateTime),
                               new JProperty("notAfter",       one.certificate.NotAfter. UtcDateTime),
                               new JProperty("chainLength",    one.certificate.ChainLength),
                               new JProperty("hasPrivateKey",  one.certificate.HasPrivateKey),
                               new JProperty("keyId",          one.keyId),
                               new JProperty("refusal",        RefusalOf(one.certificate, one.keyId))
                           )))),
                           new JProperty("pem",          String.Concat(inspection.Certificates.Select(certificate => WithoutKey(certificate.Pem))))
                       ))
                   );

        }

        /// <summary>
        /// POST .../certificates/upload with {"pem"} or {"content", "password"}:
        /// every certificate in a text or a file taken in under the key of this
        /// controller it belongs to - each answered on its own, {"taken": [{"id",
        /// "label", "warnings"}], "refused": [{"label", "error"}]}, and refused
        /// as a whole, with 400, where none was taken in.
        /// </summary>
        /// <remarks>
        /// Which key a certificate is for is worked out from the certificate,
        /// as it is for one uploaded under a key: the page that pastes several
        /// at once - a renewal for each key, say - need not know which is
        /// whose. One that came with its private key is refused for it.
        /// </remarks>
        private Task<HTTPResponse> PostCertificateUpload(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(NodeResources.Certificates), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            if (!TryReadContent(json, out var bytes, out var contentError))
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest, contentError));

            var inspection = CertificateStore.Inspect(bytes, json.Value<String>("password"));

            if (inspection.Error is not null)
                return Task.FromResult(
                           ErrorJSON(Request, HTTPStatusCode.BadRequest, inspection.Error,
                                     inspection.PasswordWanted ? new JProperty("passwordWanted", true) : null)
                       );

            var taken   = new JArray();
            var refusedOnes = new JArray();

            foreach (var certificate in inspection.Certificates)
            {

                if (certificate.HasPrivateKey)
                {
                    refusedOnes.Add(new JObject(new JProperty("label", certificate.CommonName),
                                                new JProperty("error", RefusalOf(certificate, KeyOf(certificate)))));
                    continue;
                }

                if (!Serving.ServerCertificates.TryAddCertificate(
                         WithoutKey(certificate.Pem),
                         Serving.OCPPServerSettings.ReachableAs ?? [],
                         out var id,
                         out var warnings,
                         out var error,
                         out var notSaved))
                {

                    if (notSaved)
                        return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));

                    refusedOnes.Add(new JObject(new JProperty("label", certificate.CommonName),
                                                new JProperty("error", error)));
                    continue;

                }

                Log.Notice($"'{user.Id}' uploaded a certificate for the key '{id}'.", "ocpp", "tls", "web");

                taken.Add(new JObject(new JProperty("id",        id),
                                      new JProperty("label",     certificate.CommonName),
                                      new JProperty("warnings",  new JArray(warnings))));

            }

            var answer = new JObject(new JProperty("taken",   taken),
                                     new JProperty("refused", refusedOnes));

            if (taken.Count == 0)
            {
                answer.Add("error", refusedOnes.Count == 1
                                        ? refusedOnes[0]!.Value<String>("error")
                                        : $"None of the {refusedOnes.Count} certificates was taken in.");
                return Task.FromResult(JSONResponse(Request, HTTPStatusCode.BadRequest, answer));
            }

            return Task.FromResult(JSONResponse(Request, HTTPStatusCode.OK, answer));

        }

        /// <summary>
        /// Which key of this controller the leaf of an inspected certificate belongs to, or nothing.
        /// </summary>
        private String? KeyOf(InspectedCertificate Certificate)
        {

            var collection = new X509Certificate2Collection();

            collection.ImportFromPem(Certificate.Pem);

            try
            {
                return collection.Count > 0
                           ? Serving.ServerCertificates.KeyOf(collection[0])
                           : null;
            }
            finally
            {
                foreach (var one in collection)
                    one.Dispose();
            }

        }

        /// <summary>
        /// Why a certificate in a text would not be taken in, as far as can be
        /// said before trying: a key that came along, or no key here it is for.
        /// </summary>
        private static String? RefusalOf(InspectedCertificate  Certificate,
                                         String?               KeyId)

            => Certificate.HasPrivateKey
                   ? "That certificate came with its private key, and none arrives here: the key of a certificate " +
                     $"this {NodeWording.Noun} presents is the one it generated for its signing request. " +
                     "Take the certificate on its own."
                   : KeyId is null
                         ? $"It belongs to no key of this {NodeWording.Noun}. A certificate is only usable here if it " +
                           "answers a signing request made here."
                         : null;

        /// <summary>
        /// A PEM with every private key block taken out of it.
        /// </summary>
        private static String WithoutKey(String Pem)

            => Regex.Replace(Pem, @"-----BEGIN [A-Z ]*PRIVATE KEY-----[\s\S]*?-----END [A-Z ]*PRIVATE KEY-----\s*", "");

        /// <summary>
        /// DELETE .../certificates/{id}: throw a key and its certificate away.
        /// </summary>
        private Task<HTTPResponse> DeleteCertificate(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(NodeResources.Certificates), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (!Serving.ServerCertificates.TryRemove(id, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.Conflict, error, notSaved));

            Log.Notice($"'{user.Id}' removed the key '{id}'.", "ocpp", "tls", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.ServerCertificates.ToJSON())
                   );

        }

        #endregion

        #region (private) The server key charging stations pin

        /// <summary>
        /// POST .../serverKeys with {"algorithm", "next"}: generate a server key
        /// that charging stations may pin, presented in a carrier certificate -
        /// the current one where nothing is presented yet, or with "next" the
        /// one announced for the next rollover.
        /// </summary>
        /// <remarks>
        /// Answers with its key pin and the whole store, as the page reads it.
        /// The private key does not come back, as for every other key here.
        /// </remarks>
        private Task<HTTPResponse> PostServerKey(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(NodeResources.Certificates), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            var asNext = json.Value<Boolean?>("next") ?? false;

            if (!Serving.ServerCertificates.TryCreateServerKey(
                     json.Value<String>("algorithm"),
                     asNext,
                     out var id,
                     out var keyPin,
                     out var error,
                     out var notSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));
            }

            Log.Notice($"'{user.Id}' had this {NodeWording.Noun} generate the {(asNext ? "next " : "")}server key '{id}' with the key pin {keyPin}.", "ocpp", "tls", "web");

            return Task.FromResult(
                       JSONResponse(
                           Request,
                           HTTPStatusCode.Created,
                           new JObject(
                               new JProperty("id",            id),
                               new JProperty("keyPin",        keyPin.ToHex()),
                               new JProperty("next",          asNext),
                               new JProperty("certificates",  Serving.ServerCertificates.ToJSON())
                           )
                       )
                   );

        }

        /// <summary>
        /// POST .../serverKeys/switch: present the announced next key from now
        /// on. Charging stations that pinned only the key before are refused
        /// from their next handshake on.
        /// </summary>
        private Task<HTTPResponse> PostServerKeySwitch(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(NodeResources.Certificates), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!Serving.ServerCertificates.TrySwitchToNextKey(out var id, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.Conflict, error, notSaved));

            Log.Notice($"'{user.Id}' switched the charging station server to the server key '{id}'.", "ocpp", "tls", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.ServerCertificates.ToJSON())
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
        /// GET .../stations: which charging stations may sign in.
        /// </summary>
        private Task<HTTPResponse> GetStations(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Read(StationsResource), false, out _, out var refused))
                return Task.FromResult(refused);

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.StationLogins.ToJSON())
                   );

        }

        /// <summary>
        /// POST .../stations with {"id", "password", "group", "note"}: add a
        /// charging station, or give one a new password.
        /// </summary>
        /// <remarks>
        /// An empty password means "make one up", and the made-up one comes
        /// back in this response and nowhere else - it is kept only as a hash,
        /// so a second look is a second password. The page has to show it once
        /// and say so.
        /// </remarks>
        private Task<HTTPResponse> PostStation(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            var id = json.Value<String>("id") ?? "";

            if (!Serving.StationLogins.TrySetPassword(
                     id,
                     json.Value<String>("password"),
                     json.Value<String>("group"),
                     json.Value<String>("note"),
                     out var generated,
                     out var error,
                     out var notSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));
            }

            Log.Notice($"'{user.Id}' set the password of the charging station '{id}'.", "ocpp", "station", "auth", "web");

            var response = new JObject(
                               new JProperty("id",        id),
                               new JProperty("stations",  Serving.StationLogins.ToJSON())
                           );

            if (generated is not null)
                response.Add("password", generated);

            return Task.FromResult(JSONResponse(Request, HTTPStatusCode.OK, response));

        }

        /// <summary>
        /// PUT .../stations/{id} with {"enabled"} and/or {"group"}: let a
        /// charging station in, stop letting it in, or move it to another
        /// group - without losing what it signs in with.
        /// </summary>
        private Task<HTTPResponse> PutStation(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (!TryParseJSONObject(Request, out var json, out var errorResponse))
                return Task.FromResult(errorResponse);

            var enabled  = json.Value<Boolean?>("enabled");
            var group    = json.Value<String>("group");

            if (enabled is null && group is null)
                return Task.FromResult(ErrorJSON(Request, HTTPStatusCode.BadRequest, "An 'enabled' of true or false, or a 'group', is required."));

            if (group is not null &&
                !Serving.StationLogins.TrySetGroup(id, group, out var groupError, out var groupNotSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.NotFound, groupError, groupNotSaved));
            }

            if (enabled is Boolean wanted &&
                !Serving.StationLogins.TrySetEnabled(id, wanted, out var error, out var notSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.NotFound, error, notSaved));
            }

            if (group is not null)
                Log.Info($"'{user.Id}' moved the charging station '{id}' into the login group '{group}'.", "ocpp", "station", "auth", "web");

            if (enabled is Boolean said)
                Log.Info($"'{user.Id}' {(said ? "let in" : "shut out")} the charging station '{id}'.", "ocpp", "station", "auth", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.StationLogins.ToJSON())
                   );

        }

        /// <summary>
        /// PUT .../stations/{id}/totp with {"sharedSecret", "validitySeconds",
        /// "length", "alphabet", "hashAlgorithm", "group", "note"}: let a
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

            var seconds = json.Value<Double?>("validitySeconds");

            if (!Serving.StationLogins.TrySetTOTP(
                     id,
                     json.Value<String>("sharedSecret"),
                     seconds.HasValue ? TimeSpan.FromSeconds(seconds.Value) : null,
                     json.Value<UInt32?>("length"),
                     json.Value<String>("alphabet"),
                     hash,
                     json.Value<String>("group"),
                     json.Value<String>("note"),
                     out var generated,
                     out var error,
                     out var notSaved))
            {
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved));
            }

            Log.Notice($"'{user.Id}' set the TOTP configuration of the charging station '{id}'.", "ocpp", "station", "auth", "web");

            var response = new JObject(
                               new JProperty("id",        id),
                               new JProperty("stations",  Serving.StationLogins.ToJSON())
                           );

            if (generated is not null)
                response.Add("sharedSecret", generated);

            return Task.FromResult(JSONResponse(Request, HTTPStatusCode.OK, response));

        }

        /// <summary>
        /// PUT .../stations/{id}/totp/binding with {"tlsChannelBinding": true
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

            if (!Serving.StationLogins.TrySetTOTPBinding(id, bound, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.NotFound, error, notSaved));

            Log.Notice($"'{user.Id}' {(bound ? "asked for" : "no longer asks for")} the one-time token of the charging station '{id}' to be bound to its connection.",
                       "ocpp", "station", "auth", "web");

            return Task.FromResult(JSONResponse(Request, HTTPStatusCode.OK, Serving.StationLogins.ToJSON()));

        }

        /// <summary>
        /// DELETE .../stations/{id}/totp: stop letting a charging station in
        /// with a one-time token.
        /// </summary>
        private Task<HTTPResponse> DeleteStationTOTP(HTTPRequest Request)

            => Task.FromResult(TakeCredentialAway(Request,
                                                  Serving.StationLogins.TryClearTOTP,
                                                  "TOTP configuration"));

        /// <summary>
        /// DELETE .../stations/{id}/password: stop letting a charging station
        /// in with a password.
        /// </summary>
        private Task<HTTPResponse> DeleteStationPassword(HTTPRequest Request)

            => Task.FromResult(TakeCredentialAway(Request,
                                                  Serving.StationLogins.TryClearPassword,
                                                  "password"));

        /// <summary>
        /// POST .../stations/{id}/sshKeys with {"line", "label", "group",
        /// "note"}: give a charging station an SSH key for OCPP over SSH,
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

            if (!Serving.TryAddStationSSHKey(
                     id,
                     json.Value<String>("line"),
                     json.Value<String>("label"),
                     json.Value<String>("group"),
                     json.Value<String>("note"),
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
                           new JProperty("stations",     Serving.StationLogins.ToJSON())
                       ))
                   );

        }

        /// <summary>
        /// DELETE .../stations/{id}/sshKeys/{fingerprint}: take an SSH key away
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

            if (!Serving.StationLogins.TryRemoveSSHKey(id, fingerprint, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.NotFound, error, notSaved));

            Log.Notice($"'{user.Id}' took the SSH key {fingerprint} of the charging station '{id}' away.", "ocpp", "station", "auth", "ssh", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.StationLogins.ToJSON())
                   );

        }

        /// <summary>
        /// POST .../stations/{id}/keyPins with {"pin", "label", "group",
        /// "note"}: let a charging station in by the key pin of its TLS client
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

            if (!Serving.StationLogins.TryAddKeyPin(
                     id,
                     json.Value<String>("pin"),
                     json.Value<String>("label"),
                     json.Value<String>("group"),
                     json.Value<String>("note"),
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
                           new JProperty("stations",  Serving.StationLogins.ToJSON())
                       ))
                   );

        }

        /// <summary>
        /// DELETE .../stations/{id}/keyPins/{pin}: stop letting a charging
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

            if (!Serving.StationLogins.TryRemoveKeyPin(id, pin, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.NotFound, error, notSaved));

            Log.Notice($"'{user.Id}' took the key pin {pin} of the charging station '{id}' away.", "ocpp", "station", "auth", "tls", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.StationLogins.ToJSON())
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

            return JSONResponse(Request, HTTPStatusCode.OK, Serving.StationLogins.ToJSON());

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
        /// DELETE .../stations/{id}: forget a charging station.
        /// </summary>
        private Task<HTTPResponse> DeleteStation(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, Permission.Edit(StationsResource), true, out var user, out var refused))
                return Task.FromResult(refused);

            if (!TryGetId(Request, out var id, out var badRequest))
                return Task.FromResult(badRequest);

            if (!Serving.StationLogins.TryRemove(id, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.NotFound, error, notSaved));

            Log.Notice($"'{user.Id}' removed the charging station '{id}'.", "ocpp", "station", "auth", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.StationLogins.ToJSON())
                   );

        }

        #endregion

        #region (private) The groups that decide what a login may do

        /// <summary>
        /// POST .../groups with {"id", "name", "enabled", "authMethods",
        /// "securityProfiles", "sshChannels", "note"}: make a login group.
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
                                         $"'{token}' is not a way of signing in that this {NodeWording.Noun} knows; " +
                                          "there are basic, totp, certificate, sshkey and keypin.");

                    methods.Add(method.Value);

                }

            #endregion

            #region The OCPP security profiles it accepts

            var profiles = new List<Byte>();

            if (JSON["securityProfiles"] is JArray profileArray)
                foreach (var token in profileArray)
                {

                    if (token.Type != JTokenType.Integer)
                        return ErrorJSON(Request, HTTPStatusCode.BadRequest, $"'{token}' is not an OCPP security profile.");

                    profiles.Add(token.Value<Byte>());

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

            if (!Serving.StationLogins.TryAddOrUpdateGroup(
                     Id,
                     JSON.Value<String>("name"),
                     JSON.Value<Boolean?>("enabled") ?? true,
                     methods,
                     profiles,
                     JSON.Value<String>("note"),
                     channels,
                     out var error,
                     out var notSaved))
            {
                return NotChanged(Request, HTTPStatusCode.BadRequest, error, notSaved);
            }

            Log.Notice($"'{user.Id}' wrote the login group '{Id}'.", "ocpp", "station", "auth", "web");

            return JSONResponse(Request, HTTPStatusCode.OK, Serving.StationLogins.ToJSON());

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

            if (!Serving.StationLogins.TryRemoveGroup(id, out var error, out var notSaved))
                return Task.FromResult(NotChanged(Request, HTTPStatusCode.Conflict, error, notSaved));

            Log.Notice($"'{user.Id}' removed the login group '{id}'.", "ocpp", "station", "auth", "web");

            return Task.FromResult(
                       JSONResponse(Request, HTTPStatusCode.OK, Serving.StationLogins.ToJSON())
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
