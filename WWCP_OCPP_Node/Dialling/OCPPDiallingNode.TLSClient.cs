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

using org.GraphDefined.Vanaheimr.Hermod;

using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

using cloud.charging.open.protocols.OCPP.Node.Connections;

using cloud.charging.open.protocols.OCPP.Node.KeyPins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// What the white paper "Key Pins and TLS 1.3 Raw Public Keys for Local Controllers" asks of the
    /// station's Device Model (work package K4): its TLS client in TLSClientController, an instance for
    /// every wss:// connection and one for its own key (sections 7.1 and 7.2), and pin sets taken from a
    /// SetVariables request a signer it believes signed (section 5.7).
    /// </summary>
    /// <remarks>
    /// WWCP_OCPP_Node's TLSClientControllerSource, given the station's key pins (OCPPDiallingNode.KeyPins.cs),
    /// the key pins of its client certificates, and the signature check of OCPPDiallingNode.SSHClient.cs:
    /// the same signers, without a time window for a pin set, whose version keeps an older one from
    /// coming back.
    /// </remarks>
    public abstract partial class OCPPDiallingNode
    {

        #region Data

        // When the response to a SetVariables request is out: what a signed pin
        // set or a change of trust ends - a connection dropped, or dialled
        // again - waits for it, since it is the connection the answer goes over.
        protected readonly ResponsesSent            responsesSent  = new ();

        protected TLSClientControllerSource         tlsClientSource = default!;

        #endregion


        #region (private) NewTLSClientSource()

        /// <summary>
        /// What TLSClientController reads and writes of this station.
        /// </summary>
        protected TLSClientControllerSource NewTLSClientSource()

            => new (keyPinning,
                    ClientKeyPins,
                    (request, endpoint, withoutTimeWindow, out signer, out why) => SignedByPinSetSigner(request, EntryOf(endpoint), withoutTimeWindow, out signer, out why),
                    responsesSent,
                    NodeWording.Noun);

        #endregion

        #region (internal) FollowTLSClientComponents(Node)

        /// <summary>
        /// Give the OCPP 2.1 node a TLSClientController for the station's own key and one for every
        /// connection over TLS there is now, and none for one there is not.
        /// </summary>
        /// <remarks>
        /// Told by the store after every change it wrote, before the station dials what changed, so that a
        /// connection dialled again has its instance by then.
        /// </remarks>
        public void FollowTLSClientComponents(OCPPv2_1.IDeviceModelNode? Node)

            => tlsClientSource.FollowComponents(Node, warning => Log.Warning(warning, "ocpp", "connections", "keypins"));

        #endregion

        #region (internal) AttachTLSClient(Node)

        /// <summary>
        /// What the OCPP 2.1 node answers of the TLS client: TLSClientController, and when the answer to
        /// a SetVariables request is out.
        /// </summary>
        public void AttachTLSClient(OCPPv2_1.NetworkingNode.AOCPPNetworkingNode Node)
        {

            FollowTLSClientComponents(Node as OCPPv2_1.IDeviceModelNode);

            Node.OCPP.OUT.OnSetVariablesResponseSent += (timestamp, sender, connection, request, response, runtime, sentMessageResult, cancellationToken) => {
                responsesSent.Sent(request);
                return Task.CompletedTask;
            };

        }

        #endregion

        #region (private) ClientKeyPins()

        /// <summary>
        /// The key pins of the TLS client keys this station presents: those of the client certificates its
        /// wss:// connections name, the oldest connection's first, each once.
        /// </summary>
        /// <remarks>
        /// A station of this kind has no one key of its own: each connection names the certificate it
        /// shows (certificateId, which the paper calls clientKeyId, section 7.1). Normally that is one
        /// certificate for all of them; during a renewal, or with back ends of several operators, it is
        /// more, and a Local Controller must know the one each connection to it shows. So every one is
        /// said, comma separated as ServerKeyPins are, the oldest connection's - the one in charge, where it
        /// dials by itself - first. A connection that names no certificate, or one whose certificate has
        /// not come back yet, shows no key, and adds none.
        /// </remarks>
        protected virtual IEnumerable<KeyPin> ClientKeyPins()
        {

            var certificates = ClientCertificates.Entries.ToDictionary(entry => entry.Id);
            var pins         = new List<KeyPin>();

            foreach (var connection in Connections.Connections.Where(entry => entry.IsSecure && entry.CertificateId is not null).OrderBy(entry => entry.CreatedAt))
            {

                if (!certificates.TryGetValue(connection.CertificateId!, out var certificate) || certificate.Certificate is null)
                    continue;

                try
                {
                    var pin = KeyPin.FromCertificate(certificate.Certificate);
                    if (!pins.Contains(pin))
                        pins.Add(pin);
                }
                catch
                {
                    // A key this runtime cannot read is no key it presents either.
                }

            }

            return pins;

        }

        #endregion

    }

}
