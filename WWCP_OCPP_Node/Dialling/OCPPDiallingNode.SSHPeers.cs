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

using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.SSH;
using org.GraphDefined.Vanaheimr.Hermod.SSH.Client;

using cloud.charging.open.protocols.WWCP.SSH;
using cloud.charging.open.protocols.WWCP.Node.SecureShell;
using cloud.charging.open.protocols.OCPP.SSH;
using cloud.charging.open.protocols.OCPP.Node.KeyPins;

using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

using cloud.charging.open.protocols.OCPP.Node.Connections;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The SSH peers of this station: the hosts of its "ssh://" connections,
    /// each with the one SSH connection the station keeps to it. OCPP is a
    /// channel of that connection, and so is whatever else goes to that host
    /// over SSH and says SSHConnection Shared - the key exchange of a time
    /// server (section 8.3 of the white paper "NTS Key Establishment over
    /// SSH"), name resolution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The connection is the station's, not OCPP's. Whoever needs it first has
    /// it made: a station that starts cold may have the time over it before a
    /// single OCPP message went over it. The station signs in on it with its
    /// own SSH key, under the login of the connection's entry, and the host is
    /// believed by what that entry holds it to: no second key pair, no pin of
    /// its own, no connection of its own for the others.
    /// </para>
    /// <para>
    /// Nothing but OCPP goes over a connection whose host is held to no pinned
    /// key: one that is believed by a key learned on its first connection and
    /// not confirmed yet, or that would learn its key from whoever asked first.
    /// On probation the station sends nothing but its BootNotification, and
    /// executes nothing but a signed pin set (section 10.4 of the white paper
    /// "OCPP over SSH"); time or names from that host would be time or names
    /// from a server nobody vouched for yet.
    /// </para>
    /// </remarks>
    public abstract partial class OCPPDiallingNode
    {

        #region Data

        // The peer each SSH client of OCPP is a channel of, for as long as the
        // station holds on to that client.
        protected readonly ConcurrentDictionary<WWCPSshClient, SSHPeer> sshPeerOf = new ();

        #endregion

        #region (protected override) SSHPeerClientKey, SSHPeerClientPublicKey, DefaultSSHPeerUser, SSHPeerHostsToBe

        /// <summary>
        /// The key this station signs in with at the SSH peers of its
        /// configuration: its own, the one of the purpose "Station" - the key
        /// it is known by wherever it signs in over SSH, and the one OCPP is a
        /// channel under as well. Made at the first need.
        /// </summary>
        protected override ISshHostKey? SSHPeerClientKey
        {
            get
            {
                try
                {
                    return SshKey.MakeIfNeeded().GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Log.Error($"The SSH key of this {NodeWording.Noun} could not be had for its SSH peers: {e.Message}", "ocpp", "connections", "ssh", "security");
                    return null;
                }
            }
        }

        /// <summary>
        /// The public key this station signs in with at its SSH peers, as a
        /// line of an authorized_keys file.
        /// </summary>
        public override String? SSHPeerClientPublicKey

            => SSHPeerClientKey is not null
                   ? SshKey.PublicKeyLine
                   : null;

        /// <summary>
        /// The user name this station signs in under at an SSH peer whose entry
        /// names none: what it calls itself in OCPP.
        /// </summary>
        protected override String DefaultSSHPeerUser

            => Node21.Id.ToString();

        /// <summary>
        /// The hosts of the connections this station dials over SSH: each
        /// becomes an SSH peer when it is first dialled, and is one a name
        /// server may be given to share the connection of before that - the
        /// host of a network connection profile with an ssh:// URL, as section
        /// 7.2 of the white paper "DNS over SSH" has it. A station that starts
        /// cold has its names over that connection before OCPP went over it.
        /// </summary>
        protected override IEnumerable<String> SSHPeerHostsToBe

            // Asked while the node is still being made, by what configures
            // its name servers: a node that has no connections yet has none
            // over SSH.
            => Connections is null
                   ? []
                   : [.. Connections.Connections.Where (entry => entry.IsSSH).
                                                 Select(entry => entry.URL.Host.ToString())];

        #endregion

        #region (private) SshPeerFor(Connection, Login, Trust)

        /// <summary>
        /// The SSH peer the host of the given connection becomes: with a
        /// connection of the station's own to it, made with the given login when
        /// the first one needs it. Null where that host is a peer already - of
        /// another connection of this station, with a login and a trust of its
        /// own - or its name is none a peer goes by: that connection is dialled
        /// for OCPP alone, as before.
        /// </summary>
        protected SSHPeer? SshPeerFor(ConnectionEntry   Connection,
                                    SshClientOptions  Login,
                                    SshHostKeyTrust   Trust)
        {

            if (!DomainName.TryParse(Connection.URL.Host.ToString(), out var host, out _) || host is null)
                return null;

            if (SSHPeerTo(host) is not null)
                return null;

            var port  = Connection.URL.Port?.ToUInt16() ?? 22;

            var peer  = new SSHPeer(
                            host,
                            port,
                            new SshSharedConnection(
                                Connection.URL.Host.ToString(),
                                port,
                                Login with { DNSClient = Login.DNSClient ?? DNSClient }
                            ),
                            WhyNot:     subsystem => subsystem == OCPPSshServer.OCPPSubsystem
                                                         ? null
                                                         : Trust.Pins.Count == 0
                                                               ? "its host is held to no pinned host key yet, and nothing but OCPP goes over it until a signed pin set says which it is."
                                                               : null,
                            PublicKey:  () => SshKey.PublicKeyLine
                        );

            // Another one became the peer of that host meanwhile.
            if (AddSSHPeer(peer) is SSHPeer before)
            {
                AddSSHPeer(before);
                return null;
            }

            return peer;

        }

        #endregion

        #region (private) DropSshConnectionOf(Client)

        /// <summary>
        /// Close the SSH connection the given client of OCPP is a channel of,
        /// for everything on it. The host stays a peer: whoever needs the
        /// connection next has it made anew, with a handshake of its own - which
        /// is what a host that is believed no longer, or by something else than
        /// before, has to go through again.
        /// </summary>
        protected async Task DropSshConnectionOf(WWCPSshClient Client)
        {

            if (!sshPeerOf.TryGetValue(Client, out var peer) || peer.Connection.Current is not SshClient ssh)
                return;

            try
            {
                await ssh.DisposeAsync();
            }
            catch (Exception e)
            {
                Log.Debug($"The SSH connection to {peer.Host.Trimmed} did not close cleanly: {e.Message}", "ocpp", "connections", "ssh");
            }

        }

        #endregion

        #region (private) ReleaseSshPeerOf(Client) / ReleaseSshPeer(Peer)

        /// <summary>
        /// The station is done with the given SSH client of OCPP: the peer it
        /// was a channel of is no peer any more, and its connection is closed -
        /// for everything that shared it.
        /// </summary>
        protected async Task ReleaseSshPeerOf(WWCPSshClient Client)
        {

            if (sshPeerOf.TryRemove(Client, out var peer))
                await ReleaseSshPeer(peer);

        }

        /// <summary>
        /// Remove the given SSH peer and close its connection, unless an SSH
        /// client of OCPP the station still holds on to is a channel of it.
        /// </summary>
        protected async Task ReleaseSshPeer(SSHPeer Peer)
        {

            if (sshPeerOf.Values.Any(other => ReferenceEquals(other, Peer)))
                return;

            RemoveSSHPeer(Peer);

            try
            {
                await Peer.Connection.DisposeAsync();
            }
            catch (Exception e)
            {
                Log.Debug($"The SSH connection to {Peer.Host.Trimmed} did not close cleanly: {e.Message}", "ocpp", "connections", "ssh");
            }

        }

        #endregion


    }

}
