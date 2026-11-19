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

using cloud.charging.open.protocols.WWCP.Node;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node.Connections;
using cloud.charging.open.protocols.WWCP.Node.KeyPins;
using cloud.charging.open.protocols.OCPP.Node.KeyPins;

using OCPPCommon = cloud.charging.open.protocols.OCPP;
using OCPPv1_6   = cloud.charging.open.protocols.OCPPv1_6;
using OCPPv2_1   = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// What a node that dials over OCPP does of its own with the device model
    /// of its OCPP 2.1 node: nothing, unless it says otherwise.
    /// </summary>
    public partial class OCPPDialling
    {

        #region (override) SubProtocolsOf(Connection)

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
        public override String[] SubProtocolsOf(ConnectionEntry Connection)

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

        #region (override) CBORFrameOf(Text) / TryCBORAsJSON(Frame, out JSON, out Error) / IsCBORSubprotocol(Subprotocol)

        /// <summary>
        /// The request of a connection test as the frame of a connection that
        /// agreed on CBOR: the same array, with nothing in its payload.
        /// </summary>
        public override Byte[] CBORFrameOf(String Text)

            => OCPPCommon.WebSockets.OCPP_CBORFrame.Join(
                   JArray.Parse(Text),
                   org.GraphDefined.Vanaheimr.Illias.CBORValue.FromMap(
                       Array.Empty<KeyValuePair<org.GraphDefined.Vanaheimr.Illias.CBORValue, org.GraphDefined.Vanaheimr.Illias.CBORValue>>()
                   )
               ).ToByteArray();

        /// <summary>
        /// What the given CBOR frame of OCPP stands for, as the JSON array.
        /// </summary>
        public override Boolean TryCBORAsJSON(Byte[]                                                     Frame,
                                                 [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]  out Newtonsoft.Json.Linq.JArray?  JSON,
                                                 [System.Diagnostics.CodeAnalysis.NotNullWhen(false)] out String?                       Error)

            => OCPPCommon.WebSockets.OCPP_CBORFrame.TryAsJSON(Frame, out JSON, out Error);

        /// <summary>
        /// Whether the given subprotocol is OCPP as CBOR.
        /// </summary>
        public override Boolean IsCBORSubprotocol(String? Subprotocol)

            => OCPPCommon.WebSockets.OCPPSubprotocols.IsCBOR(Subprotocol);

        #endregion

        #region (override) SshSubsystem / ConnectOverSSH(Connection, Node, Login, SshConnection, CancellationToken)

        /// <summary>
        /// OCPP goes over the SSH subsystem of the white paper "OCPP over SSH".
        /// </summary>
        public override String? SshSubsystem

            => OCPPCommon.SSH.OCPPSshServer.OCPPSubsystem;

        /// <summary>
        /// Open the given connection over SSH by the OCPP node of its version,
        /// named as over a WebSocket - see Dial.
        /// </summary>
        public override async Task<(IWWCPClient? Client, Boolean IsConnected, Exception? Error)>

            ConnectOverSSH(ConnectionEntry                                          Connection,
                           AWWCPNetworkingNode                                      Node,
                           org.GraphDefined.Vanaheimr.Hermod.SSH.Client.SshClientOptions  Login,
                           Func<CancellationToken, ValueTask<org.GraphDefined.Vanaheimr.Hermod.SSH.Client.SshClient>>?  SshConnection,
                           CancellationToken                                        CancellationToken)

        {

            var result = await NodeOf(Connection.OCPPVersion).ConnectOCPPClient(
                                   RemoteURL:                 Connection.URL,
                                   NextHopNetworkingNodeId:   WWCPNode.NextHopOf(Connection),
                                   RoutingNetworkingNodeIds:  WWCPNode.CSMSNamesOf(Connection),
                                   Ssh:                       Login,
                                   SshConnection:             SshConnection,
                                   Subprotocols:              SubProtocolsOf(Connection),
                                   DNSClient:                 WWCPNode.DNSClient,
                                   PingEvery:                 WWCPNode.SshPingEvery,
                                   CancellationToken:         CancellationToken
                               );

            return (result.Client, result.IsConnected, result.Error);

        }

        #endregion

        #region (override) ProbationRules(Probation, Action, Payload)

        /// <summary>
        /// While a learned key pin is on probation, nothing but a pin set of
        /// OCPP is executed - see KeyPinGate.
        /// </summary>
        public override String? ProbationRules(Probation                      Probation,
                                                  String                         Action,
                                                  Newtonsoft.Json.Linq.JObject?  Payload)

            => KeyPinGate.Rules(Probation, Action, Payload, $"{WWCPNode.NodeNoun}");

        #endregion

        #region (override) ConnectOverWebSocket(Connection, Basic, Validator, Certificates, TOTP, TOTPLogin, CancellationToken)

        /// <summary>
        /// Open the WebSocket of the given connection, by the OCPP node of
        /// its version: under a name of its own, and the primary under the
        /// name of the CSMS as well - see NextHopOf and CSMSNamesOf.
        /// </summary>
        /// <remarks>
        /// What every node that dials says is said here. A node that has more
        /// to say of its lines - the networking mode, how often it pings, how
        /// long it waits, which versions of TLS it speaks - opens it itself.
        /// </remarks>
        /// <param name="Connection">The connection.</param>
        /// <param name="Basic">The password it signs in with, where it has one.</param>
        /// <param name="Validator">How the server is believed: by its key pins, by its CA, or not asked where there is no TLS.</param>
        /// <param name="Certificates">The client certificate it holds up, with its chain, where it has one.</param>
        /// <param name="TOTP">The one-time password it signs in with, where it has one.</param>
        /// <param name="TOTPLogin">The login the one-time password is sent under.</param>
        /// <param name="CancellationToken">Ends the attempt.</param>
        public override Task<HTTPResponse> ConnectOverWebSocket(ConnectionEntry                                                 Connection,
                                                                  IHTTPAuthentication?                                            Basic,
                                                                  RemoteTLSServerCertificateValidationHandler<IWebSocketClient>?  Validator,
                                                                  X509Certificate2[]?                                             Certificates,
                                                                  TOTPConfig?                                                     TOTP,
                                                                  String?                                                         TOTPLogin,
                                                                  CancellationToken                                               CancellationToken)

            => NodeOf(Connection.OCPPVersion).ConnectOCPPWebSocketClient(
                   RemoteURL:                   Connection.URL,
                   NetworkingMode:              WWCPNode.NetworkingModeOf(Connection),
                   NextHopNetworkingNodeId:     WWCPNode.NextHopOf(Connection),
                   RoutingNetworkingNodeIds:    WWCPNode.CSMSNamesOf(Connection),
                   SecWebSocketProtocols:       SubProtocolsOf(Connection),
                   HTTPAuthentication:          Basic,
                   RemoteCertificateValidator:  Validator,
                   ClientCertificates:          WWCPNode.CertificateShownAlone(Certificates),
                   ClientCertificateChain:      WWCPNode.CertificateWithItsChain(Certificates),
                   TOTPConfig:                  TOTP,
                   TOTPLogin:                   TOTPLogin,
                   WebSocketOver:               Connection.HTTPVersion,
                   DNSClient:                   WWCPNode.DNSClient,
                   CancellationToken:           CancellationToken
               );

        #endregion

        #region (override) DiallingNodes / DiallingNodeOrNoneOf(Version)

        /// <summary>
        /// The OCPP nodes this one dials with.
        /// </summary>
        public override IEnumerable<AWWCPNetworkingNode> DiallingNodes

            => Nodes;

        /// <summary>
        /// The OCPP node that dials a connection of the given version.
        /// </summary>
        public override AWWCPNetworkingNode? DiallingNodeOrNoneOf(OCPPVersion Version)

            => NodeOrNoneOf(Version);

        #endregion

        #region (protected) NodeOf(Version) / NodeOrNoneOf(Version)

        /// <summary>
        /// The OCPP node that speaks the given version.
        /// </summary>
        public OCPPCommon.NetworkingNode.AOCPPNetworkingNode NodeOf(OCPPVersion Version)

            => NodeOrNoneOf(Version)
                   ?? throw new InvalidOperationException($"This {WWCPNode.NodeNoun} does not speak {ConnectionEntry.AsText(Version)}.");

        /// <summary>
        /// The OCPP node that speaks the given version, or none where this
        /// node does not speak it.
        /// </summary>
        public OCPPCommon.NetworkingNode.AOCPPNetworkingNode? NodeOrNoneOf(OCPPVersion Version)

            => Version == OCPPVersion.OCPP1_6
                   ? Node16
                   : Node21;

        #endregion

        #region (override) SetSignatureFormatsOf(Connection, Client) / SignatureFormatsOf(Connection)

        /// <summary>
        /// Which signatures the messages over the given client get, said to
        /// the OCPP node of the connection's version.
        /// </summary>
        public override void SetSignatureFormatsOf(ConnectionEntry  Connection,
                                                      IWWCPClient      Client)
        {
            if (Connection.OCPPVersion == OCPPVersion.OCPP1_6)
                Node16!.OCPP.SetSignatureFormats(Client, Connection.SignatureFormats);
            else
                Node21.OCPP.SetSignatureFormats(Client, Connection.SignatureFormats);
        }

        /// <summary>
        /// Which signatures the OCPP node of this station writes for the given
        /// connection now - or null, where its signature policy decides.
        /// </summary>
        public override cloud.charging.open.protocols.WWCP.SignatureFormats? SignatureFormatsOf(ConnectionEntry Connection)

            => Connection.OCPPVersion == OCPPVersion.OCPP1_6
                   ? Node16!.OCPP.SignatureFormatsFor(WWCPNode.NextHopOf(Connection))
                   : Node21.OCPP.SignatureFormatsFor(WWCPNode.NextHopOf(Connection));

        #endregion

        #region (protected) DialAgain(Old16, Old21, Because)

        /// <summary>
        /// Hang up what the replaced OCPP nodes were connected to and dial it
        /// again, through the nodes that replaced them.
        /// </summary>
        /// <param name="Old16">The OCPP 1.6 node replaced.</param>
        /// <param name="Old21">The OCPP 2.1 node replaced.</param>
        /// <param name="Because">Why, as a sentence without its full stop.</param>
        public Task DialAgain(OCPPv1_6.NetworkingNode.AOCPPNetworkingNode?  Old16,
                                 OCPPv2_1.NetworkingNode.AOCPPNetworkingNode?  Old21,
                                 String                                        Because)

            => WWCPNode.DialAgain([ Old16, Old21 ], Because);

        #endregion

        #region (virtual) SSHClientControllersOf(Node) / AddComponentTo(Node, Component) / RemoveComponentFrom(Node, Name, Instance)

        /// <summary>
        /// The SSHClientControllers the device model of the given node has.
        /// </summary>
        public virtual IEnumerable<OCPPv2_1.SSHClientController> SSHClientControllersOf(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode Node)

            => [];

        /// <summary>
        /// Give the device model of the given node a component.
        /// </summary>
        public virtual void AddComponentTo(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode  Node,
                                              OCPPv2_1.ComponentConfig                     Component)
        { }

        /// <summary>
        /// Take a component from the device model of the given node.
        /// </summary>
        public virtual void RemoveComponentFrom(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode  Node,
                                                   String                                       Name,
                                                   String?                                      Instance)
        { }

        #endregion

        #region (virtual) AnswerCustomTriggersOf(Node, Answer)

        /// <summary>
        /// Have the given node answer a TriggerMessage for a custom trigger
        /// with what is given.
        /// </summary>
        public virtual void AnswerCustomTriggersOf(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode                                    Node,
                                                      Func<OCPPv2_1.CSMS.TriggerMessageRequest, Task<OCPPv2_1.CS.TriggerMessageResponse?>>  Answer)
        { }

        #endregion

    }

}
