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
using org.GraphDefined.Vanaheimr.Hermod.SSH;
using org.GraphDefined.Vanaheimr.Hermod.Messaging;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.SSH;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using OCPPv1_6 = cloud.charging.open.protocols.OCPPv1_6;
using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

using cloud.charging.open.protocols.OCPP.Node.Connections;

using cloud.charging.open.protocols.OCPP.Node.KeyPins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// What the white paper "OCPP over SSH" asks of the station beyond dialling: its SSH client in the
    /// Device Model (SSHClientController, section 9.2), pin sets a signer it believes signed (section
    /// 10.4), one key per purpose and their rotation (sections 10.1 and 10.3), and the security events
    /// (section 9.4).
    /// </summary>
    public abstract partial class OCPPDiallingNode
    {

        #region Data

        // The trust each connection over SSH was dialled with last: what its
        // server showed, and where a signed pin set puts its pins at once.
        protected readonly ConcurrentDictionary<String, SshHostKeyTrust>  sshTrusts  = new ();

        protected SshClientKey? sftpKey;

        #endregion

        #region Properties

        /// <summary>
        /// The key this station signs in with for SFTP: a key of its own, as every purpose has one
        /// (decision 9).
        /// </summary>
        public SshClientKey SftpKey

            => sftpKey ??= new SshClientKey(System.IO.Path.Combine(Connections.Path, SshClientKey.DirectoryName),
                                            $"chargingstation-sftp@{Environment.MachineName}",
                                            "id_ed25519_sftp");

        #endregion


        #region (private) KeyOf(Purpose)

        /// <summary>
        /// The station's key of the given purpose; null for NTS and DNS, whose keys are the node's.
        /// </summary>
        protected SshClientKey? KeyOf(OCPPv2_1.SSHKeyPurpose Purpose)

            => Purpose switch {
                   OCPPv2_1.SSHKeyPurpose.Station  => SshKey,
                   OCPPv2_1.SSHKeyPurpose.SFTP  => SftpKey,
                   _                            => null
               };

        #endregion

        #region (internal) FollowSSHClientComponents(Node)

        /// <summary>
        /// Give the OCPP 2.1 node an SSHClientController for the station's keys and one for every
        /// connection over SSH there is now, and none for one there is not.
        /// </summary>
        public void FollowSSHClientComponents(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode? Node)
        {

            if (Node is null)
                return;

            var source       = new SSHClientSource(this);
            var connections  = source.Connections.ToArray();

            foreach (var shown in SSHClientControllersOf(Node).ToArray())
                if (shown.Connection is not null && !connections.Contains(shown.Connection, StringComparer.Ordinal))
                    RemoveComponentFrom(Node, shown.Name, shown.Instance);

            if (!SSHClientControllersOf(Node).Any(shown => shown.Connection is null))
                AddComponentTo(Node, new OCPPv2_1.SSHClientController(source));

            foreach (var connection in connections)
                if (!SSHClientControllersOf(Node).Any(shown => shown.Connection == connection))
                    AddComponentTo(Node, new OCPPv2_1.SSHClientController(source, connection));

        }

        #endregion

        #region (internal) AttachSSHClient(Node)

        /// <summary>
        /// What the OCPP 2.1 node answers of the SSH client: the controller, SSHKeySigned, and the
        /// CustomTrigger "SignSSHKey".
        /// </summary>
        public void AttachSSHClient(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode Node)
        {

            FollowSSHClientComponents(Node);

            AnswerCustomTriggersOf(Node, CustomTrigger);
            Node.OCPP.IN.OnSSHKeySigned  += SSHKeySigned;

        }

        #endregion


        #region (private) SignedByPinSetSigner(Request, Connection, WithoutTimeWindow, out Signer, out Why)

        /// <summary>
        /// Whether a signer this station believes signed the given SetVariables request for the given
        /// connection: one of the pin set signers on its Connections page (white paper "OCPP over SSH",
        /// section 10.4; "Key Pins and Raw Public Keys", section 5.7, way 1).
        /// </summary>
        /// <remarks>
        /// <para>
        /// Without a time window for a pin set: it carries a version of its own, which the store compares
        /// with the one it holds and keeps under its lock, so that an older one cannot come back - and a
        /// station whose clock was never set, the very one that learns a key on first use, would otherwise
        /// refuse every pin set, or take any, depending on how wrong its clock is. With the time window, by
        /// this station's clock, for what has no version to stop a replay: how a connection believes its
        /// server beyond its pins.
        /// </para>
        /// <para>
        /// The signature names the station as its destination: by the identity it has on this connection
        /// - which is what its CSMS knows it as, and signs for, where the request comes through the Local
        /// Controller at the other end - or by the id of its OCPP node, which an incoming request is
        /// addressed to (see <see cref="NodeIdentityOf"/>). The message id may be "*" with a nonce: a Local
        /// Controller sends a request on under a message id of its own.
        /// </para>
        /// </remarks>
        /// <param name="Request">The request.</param>
        /// <param name="Connection">The connection whose variables it writes.</param>
        /// <param name="WithoutTimeWindow">Whether the signature's time is left out: for a pin set only.</param>
        /// <param name="Signer">Who signed it, as the Connections page says.</param>
        /// <param name="Why">Why it is not taken as signed.</param>
        protected Boolean SignedByPinSetSigner(OCPPv2_1.CSMS.SetVariablesRequest?   Request,
                                             ConnectionEntry                      Connection,
                                             Boolean                              WithoutTimeWindow,
                                             [NotNullWhen(true)]  out String?     Signer,
                                             [NotNullWhen(false)] out String?     Why)
        {

            // As this station's OCPP 2.1 node serializes it: what its signer signed.
            var json = Request?.ToJSON(true,
                                       Node21.OCPP.CustomSetVariablesRequestSerializer,
                                       Node21.OCPP.CustomSetVariableDataSerializer,
                                       Node21.OCPP.CustomComponentSerializer,
                                       Node21.OCPP.CustomEVSESerializer,
                                       Node21.OCPP.CustomVariableSerializer,
                                       Node21.OCPP.CustomSignatureSerializer,
                                       Node21.OCPP.CustomCustomDataSerializer);

            return PinSetSignatures.SignedBy(Request,
                                             json,
                                             Connections.PinSetSigners,
                                             NodeIdentityOf(Connection) ?? Node21.Id,
                                             OCPPv2_1.Version.WebSocketSubProtocolId,
                                             Connection.IsSSH ? "SSH" : "TLS",
                                             WithoutTimeWindow,
                                             () => TimeProvider.GetUtcNow(),
                                             out Signer,
                                             out Why,
                                             $"{NodeWording.Noun}");

        }

        #endregion

        #region (internal) NodeIdentityOf(Connection)

        /// <summary>
        /// Who this station is on the given connection, as the server at the other end names it: the
        /// networking node its CSMS knows it as, where it comes through that server - or null, where it
        /// is the id of its OCPP node.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Over a WebSocket, as WWCPWebSocketServer names a client: the last segment of the URL's path
        /// ("wss://lc.local/ocpp/CS001" is "CS001"), else the common name of its client certificate, else
        /// the login of its credentials. A server that took the certificate by its key pin names nobody by
        /// it, and goes on to the login - this station cannot tell which it does, and every station of
        /// OCPP-J names itself in the path anyway.
        /// </para>
        /// <para>
        /// Over SSH, as WWCPSshServer names a client: the node its HELLO connects as, the last segment of
        /// the path, else its login - the URL's, or that of its credentials.
        /// </para>
        /// <para>
        /// The node itself has a fixed id, which an incoming request is addressed to whatever its sender
        /// called the station: a signature names the one its CSMS knows, so the one of the connection is
        /// what it is verified against, the node's id as well.
        /// </para>
        /// </remarks>
        public NetworkingNode_Id? NodeIdentityOf(ConnectionEntry Connection)
        {

            var login = Connection.AuthenticationId is String authenticationId
                            ? Connections.Authentications.FirstOrDefault(entry => entry.Id == authenticationId)?.Login
                            : null;

            if (Connection.IsSSH)
            {

                if (WWCPSshClient.ConnectAsFrom(Connection.URL) is NetworkingNode_Id connectsAs)
                    return connectsAs;

                return (Connection.URL.Login ?? login) is String sshLogin
                           ? NetworkingNode_Id.TryParse(sshLogin)
                           : null;

            }

            var path = Connection.URL.Path.ToString();

            if (NetworkingNode_Id.TryParse(path[(path.LastIndexOf('/') + 1)..]) is NetworkingNode_Id fromPath)
                return fromPath;

            if (Connection.CertificateId is String certificateId &&
                ClientCertificates.Entries.FirstOrDefault(entry => entry.Id == certificateId)?.Certificate is System.Security.Cryptography.X509Certificates.X509Certificate2 certificate &&
                NetworkingNode_Id.TryParse(certificate.GetNameInfo(System.Security.Cryptography.X509Certificates.X509NameType.SimpleName, forIssuer: false)) is NetworkingNode_Id fromCertificate)
                return fromCertificate;

            return login is not null
                       ? NetworkingNode_Id.TryParse(login)
                       : null;

        }

        #endregion

        #region (private) PinSetTaken(Connection, Learned, Confirmed, Version, Signer)

        /// <summary>
        /// A signed pin set was taken for a connection: its trust is held to the pins at once, and a
        /// probation ends - confirmed, or with the server it learned refused.
        /// </summary>
        protected void PinSetTaken(String   Connection,
                                 String?  Learned,
                                 Boolean  Confirmed,
                                 UInt64   Version,
                                 String   Signer)
        {

            var entry = Connections.Connections.FirstOrDefault(entry => entry.Id == Connection);

            if (entry is null)
                return;

            var onIt = probations.OfEndpoint(Connection);

            // A connection to a server refused is written down as not believed
            // before anything else: before its trust takes the pins, which ends
            // the probation, and before anything is said of it. In between a
            // request of that server would get through the gate.
            if (Learned is not null && !Confirmed)
                probations.NoLongerBelieved(onIt, $"This {NodeWording.Noun} no longer believes its server by the host key {Learned} it learned on its first " +
                                                  $"connection: the signed pin set of version {Version} does not have it, and the connection is closed.");

            if (sshTrusts.TryGetValue(Connection, out var trust))
                trust.TakePins(entry.SshHostKeys);

            probations.TryStopClock(Connection);

            foreach (var probation in onIt)
                probations.Remove(probation);

            if (Learned is not null && Confirmed)
            {
                Log.Notice($"'{entry.Description}': the signed pin set of version {Version}, signed by '{Signer}', confirms the host key " +
                           $"{Learned} it learned on its first connection; its probation ends.", "ocpp", "connections", "ssh", "security");
                SshSecurityEvent(entry, "ReconfigurationOfSecurityParameters", $"Host key {Learned} of '{entry.Id}' confirmed by a signed pin set of version {Version}");
            }

            else if (Learned is not null)
            {

                Log.Warning($"'{entry.Description}': the signed pin set of version {Version}, signed by '{Signer}', does not have the host key " +
                            $"{Learned} it learned on its first connection - that server is not the one its CSMS knows. The connection is made anew, " +
                            $"held to the signed pins ({entry.SshHostKey}).", "ocpp", "connections", "ssh", "security");

                SshSecurityEvent(entry, "InvalidCsmsHostKey", $"Learned host key {Learned} of '{entry.Id}' is not among the signed pins of version {Version}");

                foreach (var probation in onIt)
                    if (probation is SshMessageConnection current)
                        _ = current.Close(MessageCloseCode.PolicyViolation, "The host key of this server is not among the signed pins.");

            }

            else
                SshSecurityEvent(entry, "ReconfigurationOfSecurityParameters", $"Host keys of '{entry.Id}' set by a signed pin set of version {Version}");

        }

        #endregion

        #region (internal) SshSecurityEvent(Connection, Type, TechInfo)

        /// <summary>
        /// Report a security event of the white paper "OCPP over SSH" (section 9.4) through the OCPP node
        /// of the connection's version - or of both, where it is the station's own.
        /// </summary>
        public void SshSecurityEvent(ConnectionEntry?  Connection,
                                       String            Type,
                                       String            TechInfo)
        {

            try
            {

                if (Connection is null || Connection.OCPPVersion == OCPPVersion.OCPP2_1)
                    QueueSecurityEvent(OCPPVersion.OCPP2_1, Type, TechInfo.Length > 255 ? TechInfo[..255] : TechInfo);

                if (Connection is null || Connection.OCPPVersion == OCPPVersion.OCPP1_6)
                    QueueSecurityEvent(OCPPVersion.OCPP1_6, Type, TechInfo.Length > 255 ? TechInfo[..255] : TechInfo);

            }
            catch (Exception e)
            {
                Log.Warning($"The security event '{Type}' could not be queued: {e.Message}", "ocpp", "ssh", "security");
            }

        }

        #endregion


        #region (private) CustomTrigger(Request)

        /// <summary>
        /// TriggerMessage of CustomTrigger "SignSSHKey" - "SignSSHKey:SFTP" for the key of SFTP: a new key
        /// is made, and shown to the CSMS in SignSSHKey (section 10.3). The keys of NTS and DNS are the
        /// node's, and are not rotated this way.
        /// </summary>
        protected Task<OCPPv2_1.CS.TriggerMessageResponse?> CustomTrigger(OCPPv2_1.CSMS.TriggerMessageRequest Request)
        {

            var trigger = Request.CustomTrigger ?? "";

            if (!trigger.StartsWith("SignSSHKey", StringComparison.Ordinal))
                return Task.FromResult<OCPPv2_1.CS.TriggerMessageResponse?>(null);

            // The keys this station signs in with at its file servers, one per file
            // server and login: "SignSSHKey:SFTP:<peer>:<login>" asks for one of them,
            // and "SignSSHKey:SFTP" for all of them while there is a file server.
            if (trigger == "SignSSHKey:SFTP" || trigger.StartsWith("SignSSHKey:SFTP:", StringComparison.Ordinal))
            {

                var asked = trigger.Split(':');

                var keys  = asked.Length == 4
                                ? FileTransferLogins(asked[2]).Contains(asked[3], StringComparer.Ordinal)
                                      ? [ (Peer: asked[2], Login: asked[3]) ]
                                      : []
                                : asked.Length == 2
                                      ? [.. FileServers.SelectMany(fileServer => FileTransferLogins(fileServer.Id).Select(login => (Peer: fileServer.Id, Login: login)))]
                                      : Array.Empty<(String Peer, String Login)>();

                if (asked.Length != 2 || keys.Length > 0)
                {

                    if (keys.Length == 0)
                        return Task.FromResult<OCPPv2_1.CS.TriggerMessageResponse?>(
                                   new OCPPv2_1.CS.TriggerMessageResponse(Request, OCPPv2_1.TriggerMessageStatus.Rejected,
                                                                          new OCPPv2_1.StatusInfo("NotFound", $"'{trigger}' names no login this {NodeWording.Noun} has at a file server: SignSSHKey:SFTP:<peer>:<login>."))
                               );

                    _ = Task.Run(async () => {
                            foreach (var (peer, login) in keys)
                                await ShowFileTransferKey(peer, login, "its CSMS asked for it");
                        });

                    return Task.FromResult<OCPPv2_1.CS.TriggerMessageResponse?>(
                               new OCPPv2_1.CS.TriggerMessageResponse(Request, OCPPv2_1.TriggerMessageStatus.Accepted)
                           );

                }

                // No file server: "SignSSHKey:SFTP" is the one key of that purpose, as before.

            }

            var purposeText = trigger.Length > "SignSSHKey".Length && trigger["SignSSHKey".Length] == ':'
                                  ? trigger["SignSSHKey:".Length..]
                                  : trigger == "SignSSHKey" ? "Station" : "";

            if (!OCPPv2_1.SSHKeyPurposeExtensions.TryParse(purposeText, out var purpose) || KeyOf(purpose) is null)
                return Task.FromResult<OCPPv2_1.CS.TriggerMessageResponse?>(
                           new OCPPv2_1.CS.TriggerMessageResponse(Request, OCPPv2_1.TriggerMessageStatus.Rejected,
                                                                  new OCPPv2_1.StatusInfo("NotSupported", $"'{trigger}' names no key this {NodeWording.Noun} rotates: OCPP or SFTP."))
                       );

            _ = Task.Run(() => RotateSSHKey(purpose, "its CSMS asked for it"));

            return Task.FromResult<OCPPv2_1.CS.TriggerMessageResponse?>(
                       new OCPPv2_1.CS.TriggerMessageResponse(Request, OCPPv2_1.TriggerMessageStatus.Accepted)
                   );

        }

        #endregion

        #region (internal) ShowFileTransferKey(Peer, Login, Why)

        /// <summary>
        /// Show the CSMS the key this station signs in with under the given
        /// login at the given file server, in a SignSSHKey that names both:
        /// what the CSMS enters at that server, and confirms with SSHKeySigned
        /// (white paper "SFTP for Logfile Uploads and Firmware Downloads",
        /// section 9.2). The key is made now where there is none yet.
        /// </summary>
        public async Task<OCPPv2_1.CSMS.SignSSHKeyResponse?> ShowFileTransferKey(String  Peer,
                                                                                   String  Login,
                                                                                   String  Why)
        {

            if (FileTransferPublicKey(Peer, Login) is not String line)
                return null;

            try
            {

                var response = await Node21.OCPP.OUT.SignSSHKey(new OCPPv2_1.CS.SignSSHKeyRequest(SourceRouting.To(NetworkingNode_Id.CSMS),
                                                                                                   OCPPv2_1.SSHKeyPurpose.SFTP,
                                                                                                   line,
                                                                                                   Peer,
                                                                                                   Login));

                Log.Notice($"The key this {NodeWording.Noun} signs in with as '{Login}' at the file server '{Peer}' was shown to the CSMS in SignSSHKey, as {Why}: " +
                           $"it answered {OCPPv2_1.GenericStatusExtensions.AsText(response.Status)}{(response.StatusInfo is not null ? $" ({response.StatusInfo.ReasonCode})" : "")}.",
                           "ocpp", "ssh", "sftp", "security");

                return response;

            }
            catch (Exception e)
            {
                Log.Warning($"The key of '{Login}' at the file server '{Peer}' could not be shown to the CSMS: {e.Message}", "ocpp", "ssh", "sftp", "security");
                return null;
            }

        }

        #endregion

        #region (internal) RotateSSHKey(Purpose, Why)

        /// <summary>
        /// Make the next key of the given purpose - where there is none yet - and show it to the CSMS in
        /// SignSSHKey. The station keeps signing in with the current key until SSHKeySigned confirms the
        /// next one.
        /// </summary>
        public async Task<OCPPv2_1.CSMS.SignSSHKeyResponse?> RotateSSHKey(OCPPv2_1.SSHKeyPurpose  Purpose,
                                                                           String                  Why)
        {

            if (KeyOf(Purpose) is not SshClientKey key)
                return null;

            try
            {

                await key.MakeIfNeeded();

                var next      = await key.MakePending();
                var response  = await Node21.OCPP.OUT.SignSSHKey(new OCPPv2_1.CS.SignSSHKeyRequest(SourceRouting.To(NetworkingNode_Id.CSMS),
                                                                                                    Purpose,
                                                                                                    next));

                Log.Notice($"The SSH key of {OCPPv2_1.SSHKeyPurposeExtensions.AsText(Purpose)} is rotated, as {Why}: the next one was shown to the CSMS in SignSSHKey, " +
                           $"which answered {OCPPv2_1.GenericStatusExtensions.AsText(response.Status)}{(response.StatusInfo is not null ? $" ({response.StatusInfo.ReasonCode})" : "")}.",
                           "ocpp", "ssh", "security");

                return response;

            }
            catch (Exception e)
            {
                Log.Warning($"The SSH key of {OCPPv2_1.SSHKeyPurposeExtensions.AsText(Purpose)} could not be rotated: {e.Message}", "ocpp", "ssh", "security");
                return null;
            }

        }

        #endregion

        #region (private) RotateIfOld()

        /// <summary>
        /// Rotate every key older than its maximum age, where none is pending yet.
        /// </summary>
        protected async Task RotateIfOld()
        {
            foreach (var purpose in new[] { OCPPv2_1.SSHKeyPurpose.Station, OCPPv2_1.SSHKeyPurpose.SFTP })
            {
                if (KeyOf(purpose) is SshClientKey key                                       &&
                    key.MaxAgeDays > 0                                                       &&
                    key.MadeAt is DateTimeOffset madeAt                                      &&
                    madeAt + TimeSpan.FromDays(key.MaxAgeDays) < TimeProvider.GetUtcNow()   &&
                    key.PendingPublicKeyLine is null)
                {
                    await RotateSSHKey(purpose, $"it is older than {key.MaxAgeDays} day(s)");
                }
            }
        }

        #endregion

        #region (private) SSHKeySigned(...)

        /// <summary>
        /// The CSMS confirms that a key is enrolled at every server the station dials for its purpose: the
        /// pending one becomes the key, and the old one's private half goes; the current one keeps the
        /// certificate that came with it.
        /// </summary>
        protected async Task<OCPPv2_1.CS.SSHKeySignedResponse> SSHKeySigned(DateTimeOffset                        Timestamp,
                                                                          IEventSender                          Sender,
                                                                          IMessageConnection                    Connection,
                                                                          OCPPv2_1.CSMS.SSHKeySignedRequest     Request,
                                                                          CancellationToken                     CancellationToken)
        {

            // A key of a login at a file server: the CSMS says that the server holds it,
            // and from then on the station signs in with it.
            if (Request.Purpose == OCPPv2_1.SSHKeyPurpose.SFTP && (Request.Peer is not null || Request.Login is not null))
            {

                if (Request.Peer is not String peer || Request.Login is not String login)
                    return new OCPPv2_1.CS.SSHKeySignedResponse(Request, OCPPv2_1.GenericStatus.Rejected,
                                                                new OCPPv2_1.StatusInfo("InvalidValue", "A key of a file server is named by its peer and its login, both."));

                if (ConfirmFileTransferKey(peer, login, Request.PublicKey) is String notConfirmed)
                    return new OCPPv2_1.CS.SSHKeySignedResponse(Request, OCPPv2_1.GenericStatus.Rejected,
                                                                new OCPPv2_1.StatusInfo("UnknownKey", notConfirmed.Length > 1024 ? notConfirmed[..1024] : notConfirmed));

                SshSecurityEvent(null, "ReconfigurationOfSecurityParameters", $"The key of '{login}' at the file server '{peer}' is confirmed as enrolled there, and used from now on.");

                return new OCPPv2_1.CS.SSHKeySignedResponse(Request, OCPPv2_1.GenericStatus.Accepted);

            }

            if (KeyOf(Request.Purpose) is not SshClientKey key)
                return new OCPPv2_1.CS.SSHKeySignedResponse(Request, OCPPv2_1.GenericStatus.Rejected,
                                                            new OCPPv2_1.StatusInfo("NotSupported", $"This {NodeWording.Noun} has no key of {OCPPv2_1.SSHKeyPurposeExtensions.AsText(Request.Purpose)} of its own."));

            if (key.PendingPublicKeyLine is String pending && SameKey(pending, Request.PublicKey))
            {

                await key.Promote(Request.Certificate, CancellationToken);

                SshSecurityEvent(null, "ReconfigurationOfSecurityParameters", $"SSH key of {OCPPv2_1.SSHKeyPurposeExtensions.AsText(Request.Purpose)} rotated: {key.Fingerprint}");

                return new OCPPv2_1.CS.SSHKeySignedResponse(Request, OCPPv2_1.GenericStatus.Accepted);

            }

            if (key.PublicKeyLine is String current && SameKey(current, Request.PublicKey))
            {
                key.KeepCertificate(Request.Certificate);
                return new OCPPv2_1.CS.SSHKeySignedResponse(Request, OCPPv2_1.GenericStatus.Accepted);
            }

            return new OCPPv2_1.CS.SSHKeySignedResponse(Request, OCPPv2_1.GenericStatus.Rejected,
                                                        new OCPPv2_1.StatusInfo("UnknownKey", $"It is neither the pending nor the current key of {OCPPv2_1.SSHKeyPurposeExtensions.AsText(Request.Purpose)}."));

        }

        /// <summary>
        /// Whether two lines of authorized_keys name the same key: its type and its base64, whatever the
        /// comment.
        /// </summary>
        protected static Boolean SameKey(String Line1, String Line2)

            => String.Join(' ', Line1.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2)) ==
               String.Join(' ', Line2.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2));

        #endregion


        #region (private class) SSHClientSource

        /// <summary>
        /// What SSHClientController reads and writes of this station.
        /// </summary>
        protected sealed class SSHClientSource(OCPPDiallingNode Station) : OCPPv2_1.ISSHClientControllerSource,
                                                                        OCPPv2_1.ISetVariablesSource
        {

            public IEnumerable<String> Connections

                => [.. Station.Connections.Connections.Where(entry => entry.IsSSH).Select(entry => entry.Id)];

            public IEnumerable<String> ConnectionVariables          => [ "HostKeyPins", "HostKeyPinsVersion", "TrustOnFirstUse", "ProbationTime", "OnPinMismatch", "HostKeyFingerprint", "LearnedHostKey" ];
            public IEnumerable<String> WritableConnectionVariables  => [ "HostKeyPins", "HostKeyPinsVersion", "TrustOnFirstUse", "ProbationTime", "OnPinMismatch" ];
            public IEnumerable<String> Purposes                     => [ "Station", "NTS", "SFTP", "DNS" ];
            public IEnumerable<String> KeyVariables                 => [ "ClientPublicKey", "ClientPendingPublicKey", "ClientCertificate", "ClientKeyMaxAge", "ClientKeyAlgorithms" ];
            public IEnumerable<String> WritableKeyVariables         => [ "ClientKeyMaxAge" ];

            public String? ConnectionValue(String Connection, String Variable)
            {

                if (Station.Connections.Connections.FirstOrDefault(entry => entry.Id == Connection) is not ConnectionEntry entry)
                    return null;

                return Variable switch {
                           "HostKeyPins"         => entry.SshHostKey ?? "",
                           "HostKeyPinsVersion"  => entry.SshHostKeysVersion.ToString(),
                           "TrustOnFirstUse"     => entry.SshTrustOnFirstUse.ToString(),
                           "ProbationTime"       => ((Int64) entry.SshProbationTime.TotalSeconds).ToString(),
                           "OnPinMismatch"       => entry.SshOnPinMismatch.ToString(),
                           "HostKeyFingerprint"  => Station.sshTrusts.TryGetValue(Connection, out var trust) ? trust.Shown ?? "" : "",
                           "LearnedHostKey"      => entry.SshLearnedHostKey ?? "",
                           _                     => null
                       };

            }

            public String? KeyValue(String Variable, String? Purpose)
            {

                if (Variable == "ClientKeyAlgorithms")
                    return "ssh-ed25519";

                if (!OCPPv2_1.SSHKeyPurposeExtensions.TryParse(Purpose ?? "", out var purpose))
                    return null;

                // The keys of NTS and DNS are the node's: shown, and neither rotated
                // nor aged here. Each its own - the one the station signs in with at
                // its time servers, and the one for its name servers over SSH (white
                // paper "DNS over SSH", section 7.3) - made at the first read.
                if (Station.KeyOf(purpose) is not SshClientKey key)
                    return Variable == "ClientPublicKey"
                               ? (purpose == OCPPv2_1.SSHKeyPurpose.DNS
                                      ? Station.DNSSSHClientPublicKey
                                      : Station.NTSKEClientPublicKey) ?? ""
                               : Variable == "ClientKeyMaxAge" ? "0" : "";

                return Variable switch {
                           "ClientPublicKey"         => key.PublicKeyLine ?? Made(key),
                           "ClientPendingPublicKey"  => key.PendingPublicKeyLine ?? "",
                           "ClientCertificate"       => key.Certificate ?? "",
                           "ClientKeyMaxAge"         => key.MaxAgeDays.ToString(),
                           _                         => null
                       };

            }

            /// <summary>
            /// A key is made at the first read of its public half, as the paper says.
            /// </summary>
            private static String Made(SshClientKey Key)
            {
                try
                {
                    Key.MakeIfNeeded().GetAwaiter().GetResult();
                    return Key.PublicKeyLine ?? "";
                }
                catch
                {
                    return "";
                }
            }

            public async Task<IReadOnlyList<OCPPv2_1.SetVariableResult>> SetVariablesAsync(IReadOnlyList<OCPPv2_1.SetVariableEntry>  Entries,
                                                                                           OCPPv2_1.DataConsistencyModel             DataConsistencyModel,
                                                                                           CancellationToken                         CancellationToken)
            {

                var results = new OCPPv2_1.SetVariableResult[Entries.Count];

                OCPPv2_1.SetVariableResult Result(Int32 Index, OCPPv2_1.SetVariableStatus Status, String? ReasonCode = null, String? AdditionalInfo = null)

                    => results[Index] = new (
                                            AttributeStatus:      Status,
                                            Component:            Entries[Index].Data.Component,
                                            Variable:             Entries[Index].Data.Variable,
                                            AttributeType:        Entries[Index].Variable.Attributes?.Type ?? OCPPv2_1.AttributeTypes.Actual,
                                            AttributeStatusInfo:  ReasonCode is not null ? new OCPPv2_1.StatusInfo(ReasonCode, AdditionalInfo) : null
                                        );

                foreach (var group in Enumerable.Range(0, Entries.Count).GroupBy(index => (Entries[index].Component as OCPPv2_1.SSHClientController)?.Connection))
                {

                    #region The station's keys: how old one may get

                    if (group.Key is null)
                    {
                        foreach (var index in group)
                        {

                            var entry = Entries[index];

                            if (entry.Variable.Name != "ClientKeyMaxAge")
                                Result(index, OCPPv2_1.SetVariableStatus.Rejected, "ReadOnly", $"SSHClientController.{entry.Variable.Name} is read-only!");

                            else if (!OCPPv2_1.SSHKeyPurposeExtensions.TryParse(entry.Data.Variable.Instance ?? "", out var purpose) ||
                                     Station.KeyOf(purpose) is not SshClientKey key)
                                Result(index, OCPPv2_1.SetVariableStatus.Rejected, "NotSupported", $"Only the keys of OCPP and SFTP are this {NodeWording.Noun}'s own; the keys of NTS and DNS are the node's.");

                            else if (!UInt32.TryParse(entry.Data.AttributeValue, out var days))
                                Result(index, OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue", $"'{entry.Data.AttributeValue}' is no number of days.");

                            else
                            {
                                key.MaxAgeDays = days;
                                Result(index, OCPPv2_1.SetVariableStatus.Accepted);
                            }

                        }
                        continue;
                    }

                    #endregion

                    #region A connection: only from a pin set a signer this station believes signed

                    var connection = group.Key;

                    if (Station.Connections.CopyOf(connection) is not ConnectionEntry connectionEntry)
                    {
                        foreach (var index in group)
                            Result(index, OCPPv2_1.SetVariableStatus.UnknownComponent, "NotFound", $"This {NodeWording.Noun} has no connection '{connection}' any more.");
                        continue;
                    }

                    // Who signed it, without a time window: a pin set is kept from
                    // coming back by its version. What has no version is held to the
                    // time window below.
                    if (!Station.SignedByPinSetSigner(Entries[group.First()].Request, connectionEntry, WithoutTimeWindow: true, out var signer, out var why))
                    {
                        foreach (var index in group)
                            Result(index, OCPPv2_1.SetVariableStatus.Rejected, "Unsigned", why);
                        continue;
                    }

                    var values = new Dictionary<String, (Int32 Index, String Value)>(StringComparer.OrdinalIgnoreCase);

                    foreach (var index in group)
                    {
                        if (!OCPPv2_1.SSHClientController.TryCheck(connection, Entries[index].Variable.Name, Entries[index].Data.AttributeValue, out var refusal))
                            results[index] = new (OCPPv2_1.SetVariableStatus.Rejected, Entries[index].Data.Component, Entries[index].Data.Variable,
                                                  Entries[index].Variable.Attributes?.Type ?? OCPPv2_1.AttributeTypes.Actual, refusal);
                        else
                            values[Entries[index].Variable.Name] = (index, Entries[index].Data.AttributeValue);
                    }

                    var hasPins     = values.TryGetValue("HostKeyPins",        out var pins);
                    var hasVersion  = values.TryGetValue("HostKeyPinsVersion", out var version);

                    if (hasPins != hasVersion)
                    {
                        foreach (var one in new[] { pins, version }.Where(one => one.Value is not null))
                            Result(one.Index, OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue",
                                   "A pin set is HostKeyPins and HostKeyPinsVersion in one request: the version is what keeps an older one from coming back.");
                    }

                    else if (hasPins)
                    {

                        if (!UInt64.TryParse(version.Value, out var versionNumber))
                        {
                            Result(version.Index, OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue", $"'{version.Value}' is no version.");
                            Result(pins.Index,    OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue", "The pin set has no version.");
                        }

                        else if (!Station.Connections.TryTakeSignedHostKeys(connection, pins.Value, versionNumber, out var learned, out var confirmed, out var refused))
                        {
                            Result(pins.Index,    OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue", refused);
                            Result(version.Index, OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue", refused);
                        }

                        else
                        {
                            Station.PinSetTaken(connection, learned, confirmed, versionNumber, signer);
                            Result(pins.Index,    OCPPv2_1.SetVariableStatus.Accepted);
                            Result(version.Index, OCPPv2_1.SetVariableStatus.Accepted);
                        }

                    }

                    var trustEntries = new[] { "TrustOnFirstUse", "ProbationTime", "OnPinMismatch" }.
                                           Where (name => values.ContainsKey(name)).
                                           Select(name => (Name: name, values[name].Index, values[name].Value)).
                                           ToArray();

                    // No version keeps an older one of these from coming back: they
                    // are taken within the time window of the signature, by this
                    // station's clock, as before pin sets were taken without one.
                    if (trustEntries.Length > 0 &&
                        !Station.SignedByPinSetSigner(Entries[group.First()].Request, connectionEntry, WithoutTimeWindow: false, out _, out var notNow))
                    {
                        foreach (var one in trustEntries)
                            Result(one.Index, OCPPv2_1.SetVariableStatus.Rejected, "OutOfTime", notNow);
                    }

                    else if (trustEntries.Length > 0)
                    {

                        var settings = new SshTrustSettings(trustEntries.FirstOrDefault(one => one.Name == "TrustOnFirstUse").Value,
                                                            trustEntries.FirstOrDefault(one => one.Name == "OnPinMismatch").Value,
                                                            trustEntries.FirstOrDefault(one => one.Name == "ProbationTime").Value is String seconds && Int64.TryParse(seconds, out var s) ? s : null);

                        var changed  = Station.Connections.TryChangeSshTrust(connection, settings, out var notChanged);

                        foreach (var one in trustEntries)
                            if (changed)
                                Result(one.Index, OCPPv2_1.SetVariableStatus.Accepted);
                            else
                                Result(one.Index, OCPPv2_1.SetVariableStatus.Rejected, "InvalidValue", notChanged);

                        if (changed)
                            Station.SshSecurityEvent(Station.Connections.Connections.FirstOrDefault(entry => entry.Id == connection),
                                                     "ReconfigurationOfSecurityParameters", $"How '{connection}' believes its server, set by a signed pin set");

                    }

                    #endregion

                }

                await Task.CompletedTask;

                return results;

            }

        }

        #endregion

    }

}
