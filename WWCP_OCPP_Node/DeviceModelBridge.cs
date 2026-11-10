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

using OCPPv1_6 = cloud.charging.open.protocols.OCPPv1_6;
using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// How a node shows its name servers and its time servers to OCPP: the
    /// components and variables of OCPP 2.1, the configuration keys of OCPP
    /// 1.6, the roots for both, and the security events of the Secure DNS and
    /// the Secure Time Synchronization white papers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Made once per node, and handed every OCPP node built for it by
    /// <see cref="Configure(OCPPv2_1.IDeviceModelNode)"/> and
    /// <see cref="Configure(OCPPv1_6.CP.IConfigurationKeysNode)"/>: a node that
    /// builds its OCPP nodes anew - a charging station whose EVSEs changed -
    /// hands it the new ones, and from then on those are the ones followed and
    /// told.
    /// </para>
    /// <para>
    /// Nothing is copied: the components read the node at every read, so a
    /// change from the page, the command line or a pin learned on first use is
    /// what the next GetVariables reads. Which servers and groups there are
    /// changes while an OCPP node stays, though, so its components follow every
    /// change the node says it made, once it let go of its configuration.
    /// </para>
    /// </remarks>
    public sealed partial class DeviceModelBridge
    {

        #region Data

        private readonly  WWCPNode                         node;
        private readonly  Func<Boolean>                    dialsOCPP2_1;
        private readonly  Func<Boolean>                    dialsOCPP1_6;

        // The OCPP nodes handed last: the 2.1 one is the one whose components
        // follow the servers and groups, and both are the ones told of events.
        private readonly  Lock                             componentsLock = new();
        private           OCPPv2_1.IDeviceModelNode?       deviceModelNode;
        private           OCPPv1_6.CP.IConfigurationKeysNode?  configurationKeysNode;

        #endregion

        #region Properties

        /// <summary>
        /// What OCPP reads and changes of the time servers.
        /// </summary>
        public NTSDeviceModel       NTS                    { get; }

        /// <summary>
        /// The roots for time servers a CSMS installs, lists and deletes.
        /// </summary>
        public NTSRootCertificates  NTSRootCertificates    { get; }

        /// <summary>
        /// What OCPP reads and changes of the name servers.
        /// </summary>
        public DNSDeviceModel       DNS                    { get; }

        /// <summary>
        /// The roots for name servers a CSMS installs, lists and deletes.
        /// </summary>
        public DNSRootCertificates  DNSRootCertificates    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The bridges of the given node, following it from now on.
        /// </summary>
        /// <param name="Node">The node, which stays the only truth.</param>
        /// <param name="NamesToKeepResolving">The names the node dials its CSMS by - leaving out those dialled by an address - which a change of the name servers must keep resolving.</param>
        /// <param name="DialsOCPP2_1">Whether a connection of OCPP 2.1 is configured, so that a security event is queued for it.</param>
        /// <param name="DialsOCPP1_6">The same for OCPP 1.6.</param>
        public DeviceModelBridge(WWCPNode                   Node,
                                 Func<IEnumerable<String>>  NamesToKeepResolving,
                                 Func<Boolean>              DialsOCPP2_1,
                                 Func<Boolean>              DialsOCPP1_6)
        {

            this.node          = Node;
            this.dialsOCPP2_1  = DialsOCPP2_1;
            this.dialsOCPP1_6  = DialsOCPP1_6;

            this.NTS                  = new NTSDeviceModel(Node, sentence => Node.Log.Notice(sentence, "nts", "ocpp"));
            this.NTSRootCertificates  = new NTSRootCertificates(Node);
            this.DNS                  = new DNSDeviceModel(Node.DNSConfigurationJSON,
                                                           Node.TryUpdateDNSConfiguration,
                                                           (change, names, within, cancellationToken) => Node.ResolveWithAsync(change, names, within, cancellationToken),
                                                           NamesToKeepResolving,
                                                           sentence => Node.Log.Notice(sentence, "dns", "ocpp"),
                                                           () => LegalTimeNamesOf(Node));
            this.DNSRootCertificates  = new DNSRootCertificates(Node, NamesToKeepResolving, () => LegalTimeNamesOf(Node));

            // The components first, so that a monitor asked about a change
            // below finds the component the change made.
            Node.OnNTSGroupAdded            += (group, origin)          => FollowNTSGroups();
            Node.OnNTSGroupRemoved          += (group, origin)          => FollowNTSGroups();
            Node.OnNTSGroupChanged          += (group, changes, origin) => FollowNTSGroups();
            Node.OnDNSConfigurationChanged  += (changes, origin)        => FollowDNSServers();

            FollowNTSEvents();
            FollowDNSEvents();

        }

        #endregion


        #region Configure(Node)

        /// <summary>
        /// Show OCPP 2.1 how the node resolves names and keeps its time:
        /// DNSClientController and a DNSServerController for every name server,
        /// by its id; the standard ClockCtrlr, NTPController, an
        /// NTPClientGroupController for every group of time servers and an
        /// NTPClientController for every server; and the roots for both.
        /// </summary>
        /// <remarks>
        /// The given node is the one followed and told from now on. A CSMS
        /// changes the name servers through TryUpdateDNSConfiguration, as the
        /// page does, and only after the names of the CSMS connections were
        /// resolved with the change; it adds a server or a group by naming it
        /// in DNSClientController.Servers, NTPController.ClientGroups or
        /// NTPClientGroupController.Clients.
        /// </remarks>
        /// <param name="Node">A node of OCPP 2.1 with a Device Model.</param>
        public void Configure(OCPPv2_1.IDeviceModelNode Node)
        {
            lock (componentsLock)
            {

                deviceModelNode = Node;

                // SyncNetworkTime and TestTimeServer, answered with the node's
                // time servers - see DeviceModelBridge.NetworkTime.cs.
                ServeNetworkTimeOnDemand(Node);

                // A server or group a request names anew is made by the
                // model, from the entries of the same request. The name
                // servers' components come first, as a report lists them.
                Node.AddNewInstancesSource(DNS);
                Node.CertificateSources.Add(DNSRootCertificates);
                Node.AddComponent(new OCPPv2_1.DNSClientController(DNS));
                FollowDNSServers(Node);

                Node.AddNewInstancesSource(NTS);
                Node.CertificateSources.Add(NTSRootCertificates);
                Node.AddComponent(new OCPPv2_1.ClockController(NTS));
                Node.AddComponent(new OCPPv2_1.NTPController  (NTS));
                FollowNTSGroups(Node);

                // QueryDNS, answered with the node's name resolution - see
                // DeviceModelBridge.QueryDNS.cs.
                if (Node is OCPPv2_1.IQueryDNSNode queryDNSNode)
                    queryDNSNode.QueryDNSProcessor = (timestamp, sender, connection, request, cancellationToken) => QueryDNS(request, cancellationToken);

            }
        }

        #endregion

        #region Configure(ChargePoint)

        /// <summary>
        /// What OCPP 1.6 sees of how the node resolves names and keeps its time:
        /// the roots a central system installs, lists and deletes as
        /// NameServerRootCertificate and NetworkTimeRootCertificate, kept in the
        /// node's store as for 2.1, and the configuration keys of the two white
        /// papers, read from the node and changed as 2.1 changes them.
        /// </summary>
        /// <param name="ChargePoint">A node of OCPP 1.6.</param>
        public void Configure(OCPPv1_6.CP.IConfigurationKeysNode ChargePoint)
        {
            lock (componentsLock)
            {

                configurationKeysNode = ChargePoint;

                // SyncNetworkTime and TestTimeServer, answered with the node's
                // time servers - see DeviceModelBridge.NetworkTime.cs.
                ServeNetworkTimeOnDemand(ChargePoint);

                ChargePoint.CertificateSources.Add(DNSRootCertificates);
                ChargePoint.AddConfigurationKeySource(DNS);

                ChargePoint.CertificateSources.Add(NTSRootCertificates);
                ChargePoint.AddConfigurationKeySource(NTS);

                // QueryDNS, answered with the node's name resolution - see
                // DeviceModelBridge.QueryDNS.cs.
                if (ChargePoint is OCPPv1_6.IQueryDNSNode queryDNSNode)
                    queryDNSNode.QueryDNSProcessor = (timestamp, sender, connection, request, cancellationToken) => QueryDNS(request, cancellationToken);

            }
        }

        #endregion


        #region (internal static) LegalTimeNamesOf(Node)

        /// <summary>
        /// The names of the NTS-KE servers of the legal time: those of the
        /// legal group's servers that are switched on, leaving out those named
        /// by an address, which need no name resolution. None while the legal
        /// group is switched off, as nothing is asked of them then.
        /// </summary>
        /// <remarks>
        /// A change of the name servers, or a root taken away, must not stop
        /// all of them resolving (SecureDNS-DeviceModel-Concept.md B.13 point
        /// 6). The node does the key exchange with the host of a server, so
        /// that host is its NTS-KE server.
        /// </remarks>
        /// <param name="Node">The node whose legal group it is.</param>
        internal static IEnumerable<String> LegalTimeNamesOf(WWCPNode Node)

            => Node.NTSGroups.
                    Where     (group  => group.IsLegal && group.Enabled).
                    SelectMany(group  => group.Sources.Sources).
                    Where     (source => source.Enabled).
                    Select    (source => source.Hostname.Trimmed).
                    Where     (host   => !System.Net.IPAddress.TryParse(host.Trim('[', ']'), out _)).
                    Distinct  (StringComparer.OrdinalIgnoreCase).
                    ToArray   ();

        #endregion

        #region (private) FollowDNSServers([Node])

        /// <summary>
        /// Give the OCPP 2.1 node handed last a DNSServerController for every
        /// name server there is now, and none for one there is not.
        /// </summary>
        /// <remarks>
        /// Told by the node after every change of the name resolution, servers
        /// added, taken away or given other ids among them, once it let go of
        /// its configuration, so that what is read here is what was saved.
        /// </remarks>
        private void FollowDNSServers()
        {
            lock (componentsLock)
            {
                if (deviceModelNode is not null)
                    FollowDNSServers(deviceModelNode);
            }
        }

        private void FollowDNSServers(OCPPv2_1.IDeviceModelNode Node)
        {

            var servers = DNS.ServerIds.
                              Distinct(StringComparer.OrdinalIgnoreCase).
                              ToArray ();

            foreach (var shown in Node.GetComponentConfigs(nameof(OCPPv2_1.DNSServerController)).OfType<OCPPv2_1.DNSServerController>().ToArray())
                if (!servers.Contains(shown.Server, StringComparer.OrdinalIgnoreCase))
                    Node.RemoveComponent(shown.Name, shown.Instance);

            // A component already there is kept: the node adds none twice. One
            // taken away since it was read here is not added at all, and the
            // node tells of that change next.
            foreach (var server in servers)
            {
                try
                {
                    Node.AddComponent(new OCPPv2_1.DNSServerController(DNS, server));
                }
                catch (ArgumentException)
                { }
            }

        }

        #endregion

        #region (private) FollowNTSGroups([Node])

        /// <summary>
        /// Give the OCPP 2.1 node handed last a component for every group of
        /// time servers and every server there is now, and none for one there
        /// is not.
        /// </summary>
        /// <remarks>
        /// Told by the node after every group made, taken away or changed, a
        /// change of the servers' ids among them, and after it let go of its
        /// configuration, so that what is read here is what was saved. A node
        /// handed in the meantime is the one followed from then on: it takes
        /// the lock to say so, and reads the groups as they are then.
        /// </remarks>
        private void FollowNTSGroups()
        {
            lock (componentsLock)
            {
                if (deviceModelNode is not null)
                    FollowNTSGroups(deviceModelNode);
            }
        }

        private void FollowNTSGroups(OCPPv2_1.IDeviceModelNode Node)
        {

            var groups   = NTS.Groups.
                               Distinct  (StringComparer.OrdinalIgnoreCase).
                               ToArray   ();

            // A server belongs to the first group naming it, as for
            // NTPClientController.AllOf.
            var clients  = groups.
                               SelectMany(group  => NTS.ClientsOf(group).Select(client => (Group: group, Client: client))).
                               DistinctBy(member => member.Client, StringComparer.OrdinalIgnoreCase).
                               ToArray   ();

            foreach (var shown in Node.GetComponentConfigs(nameof(OCPPv2_1.NTPClientGroupController)).OfType<OCPPv2_1.NTPClientGroupController>().ToArray())
                if (!groups.Contains(shown.Group, StringComparer.OrdinalIgnoreCase))
                    Node.RemoveComponent(shown.Name, shown.Instance);

            // Gone from its group, or moved to another one, whose description
            // it would otherwise go on giving.
            foreach (var shown in Node.GetComponentConfigs(nameof(OCPPv2_1.NTPClientController)).OfType<OCPPv2_1.NTPClientController>().ToArray())
                if (!clients.Any(member => String.Equals(member.Client, shown.Client, StringComparison.OrdinalIgnoreCase) &&
                                           String.Equals(member.Group,  shown.Group,  StringComparison.OrdinalIgnoreCase)))
                    Node.RemoveComponent(shown.Name, shown.Instance);

            // A component already there is kept: the node adds none twice. One
            // taken away since it was read here is not added at all, and the
            // node tells of that change next.
            foreach (var group in groups)
            {
                try
                {
                    Node.AddComponent(new OCPPv2_1.NTPClientGroupController(NTS, group));
                }
                catch (ArgumentException)
                { }
            }

            foreach (var (group, client) in clients)
            {
                try
                {
                    Node.AddComponent(new OCPPv2_1.NTPClientController(NTS, group, client));
                }
                catch (ArgumentException)
                { }
            }

        }

        #endregion

    }

}
