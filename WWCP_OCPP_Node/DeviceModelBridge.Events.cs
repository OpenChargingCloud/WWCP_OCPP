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

using System.Globalization;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Certificates;

using OCPPv1_6 = cloud.charging.open.protocols.OCPPv1_6;
using OCPPv2_1 = cloud.charging.open.protocols.OCPPv2_1;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// What the CSMS is told of the time servers and the name servers without
    /// asking: the security events of the Secure Time Synchronization white
    /// paper (section 3.2.6) and of the Secure DNS white paper (section 3.1.4),
    /// and the events of the monitors it set (NTS concept B.12, SecureDNS
    /// concept B.11).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Told through the queue of each OCPP node handed last whose version a
    /// connection is configured for, so that an event outlives an outage and,
    /// where the queue is kept on disk, a restart. A node that dials no CSMS of
    /// a version queues nothing for it. That matters most for the name
    /// servers, since a CSMS dialled by its name is out of reach precisely
    /// while the names do not resolve.
    /// </para>
    /// <para>
    /// A change made through OCPP is the CSMS's own and is not told back, as
    /// B.12 point 7 says; its monitors are asked all the same, because the
    /// node writes a value the way it keeps it, which need not be the way it
    /// was sent - an IPv6 address short, a port written out. The whole of what
    /// changed is in the log; the techInfo of a security event carries as much
    /// of it as fits. A pin learned on first use is a change of the
    /// configuration by "tofu", and told as such, once: not again as the news
    /// of the handshake it was learned at. A server believed again is in the
    /// log alone.
    /// </para>
    /// <para>
    /// SettingSystemTime is told when a check finds the clock at least the
    /// legal group's threshold off, and again only after a check found it
    /// closer (B.4 point 6): the node does not set its clock, so an offset is
    /// the same offset until something else sets it.
    /// </para>
    /// </remarks>
    public sealed partial class DeviceModelBridge
    {

        #region (private) FollowNTSEvents()

        /// <summary>
        /// Tell the CSMS what the node says of its time servers.
        /// </summary>
        private void FollowNTSEvents()
        {

            node.OnNTSGroupChanged += (group, changes, origin) => {
                if (!ChangeOrigin.IsOCPP(origin))
                    TellCSMS("ReconfigurationOfSecurityParameters",
                             $"nts.groups.{group.Id}: {String.Join("; ", changes)} (by {origin ?? "nobody said"})");
                CheckMonitors();
            };

            node.OnNTSGroupAdded   += (group, origin) => {
                if (!ChangeOrigin.IsOCPP(origin))
                    TellCSMS("ReconfigurationOfSecurityParameters", $"nts.groups.{group.Id} added (by {origin ?? "nobody said"})");
                CheckMonitors();
            };

            node.OnNTSGroupRemoved += (group, origin) => {
                if (!ChangeOrigin.IsOCPP(origin))
                    TellCSMS("ReconfigurationOfSecurityParameters", $"nts.groups.{group.Id} removed (by {origin ?? "nobody said"})");
                CheckMonitors();
            };

            node.OnNTSGroupLimited  += WhenNTSGroupLimited;

            node.OnNTSOffsetReached += WhenNTSOffsetReached;

            node.OnServerNews += (judgement, news) => {

                if (judgement.Service != CertificateUsages.NTS)
                    return;

                switch (news)
                {

                    case ServerNews.Refused:
                        TellCSMS("InvalidTimeServerCertificate",
                                 $"{judgement.Server}: {judgement.Outcome}, certificate {judgement.CertificateFingerprint ?? "none"}, root {judgement.RootFingerprint ?? "none"}");
                        break;

                    case ServerNews.CertificateChanged:
                        TellCSMS("TimeServerCertificateChanged",
                                 $"{judgement.Server}: certificate {judgement.Previously?.Certificate} -> {judgement.CertificateFingerprint}, " +
                                 $"root {judgement.Previously?.Root ?? "unknown"} -> {judgement.RootFingerprint ?? "unknown"}");
                        break;

                }

                CheckMonitors();

            };

        }

        #endregion

        #region (private) FollowDNSEvents()

        /// <summary>
        /// Tell the CSMS what the node says of its name servers.
        /// </summary>
        private void FollowDNSEvents()
        {

            node.OnDNSConfigurationChanged += (changes, origin) => {
                if (!ChangeOrigin.IsOCPP(origin))
                    TellCSMS("ReconfigurationOfSecurityParameters",
                             $"dns: {String.Join("; ", changes)} (by {origin ?? "nobody said"})",
                             CertificateUsages.DNS);
                CheckMonitors();
            };

            node.OnServerNews += (judgement, news) => {

                if (judgement.Service != CertificateUsages.DNS)
                    return;

                switch (news)
                {

                    case ServerNews.Refused:
                        TellCSMS("InvalidNameServerCertificate",
                                 $"{judgement.Server}: {judgement.Outcome}, certificate {judgement.CertificateFingerprint ?? "none"}, root {judgement.RootFingerprint ?? "none"}",
                                 CertificateUsages.DNS);
                        break;

                    case ServerNews.Tolerated:
                    case ServerNews.Recorded:
                        TellCSMS("NameServerPinMismatchTolerated",
                                 $"{judgement.Server}: {judgement.Outcome}, certificate {judgement.CertificateFingerprint ?? "none"}, root {judgement.RootFingerprint ?? "none"}, " +
                                 $"held to {judgement.HeldTo?.Text ?? "nothing"}",
                                 CertificateUsages.DNS);
                        break;

                    case ServerNews.CertificateChanged:
                        TellCSMS("NameServerCertificateChanged",
                                 $"{judgement.Server}: certificate {judgement.Previously?.Certificate} -> {judgement.CertificateFingerprint}, " +
                                 $"root {judgement.Previously?.Root ?? "unknown"} -> {judgement.RootFingerprint ?? "unknown"}",
                                 CertificateUsages.DNS);
                        break;

                }

                CheckMonitors();

            };

        }

        #endregion


        #region (internal) WhenNTSGroupLimited(Group, Limited)

        /// <summary>
        /// The legal group reached MaxErrors, or produced a time again after it.
        /// </summary>
        /// <remarks>
        /// Of its own, so that a test can say it without making a group fail
        /// ten rounds in a row.
        /// </remarks>
        internal void WhenNTSGroupLimited(NTSGroup  Group,
                                          Boolean   Limited)
        {

            if (Group.IsLegal)
                TellCSMS(Limited ? "LegalTimeLimited" : "LegalTimeRestored",
                         Limited ? $"nts.groups.{Group.Id}: {Group.ErrorsInARow} errors in a row"
                                 : $"nts.groups.{Group.Id}: a round produced a time again");

            CheckMonitors();

        }

        #endregion

        #region (internal) WhenNTSOffsetReached(Group, Offset)

        /// <summary>
        /// A check found the clock at least the legal group's threshold off.
        /// </summary>
        /// <remarks>
        /// Of its own, so that a test can say it without a time server whose
        /// clock is off.
        /// </remarks>
        internal void WhenNTSOffsetReached(NTSGroup  Group,
                                           TimeSpan  Offset)
        {
            TellCSMS("SettingSystemTime",
                     $"nts.groups.{Group.Id}: the clock is {Offset.TotalSeconds.ToString("+0.000;-0.000;0.000", CultureInfo.InvariantCulture)} s off, " +
                     $"the threshold is {Group.OffsetReportingThreshold?.TotalSeconds.ToString("0", CultureInfo.InvariantCulture) ?? "none"} s");
        }

        #endregion


        #region (private) TellCSMS(Type, TechInfo, Service = NTS)

        /// <summary>
        /// Queue a security event at each OCPP node handed last whose version a
        /// connection is configured for, and write the whole of it into the
        /// log, tagged with the service it is of: the time servers' or the name
        /// servers'.
        /// </summary>
        private void TellCSMS(String  Type,
                              String  TechInfo,
                              String  Service   = CertificateUsages.NTS)
        {

            var tells2_1 = dialsOCPP2_1();
            var tells1_6 = dialsOCPP1_6();

            if (!tells2_1 && !tells1_6)
                return;

            node.Log.Notice($"Security event {Type} for the CSMS: {TechInfo}", Service, "ocpp", "security");

            OCPPv2_1.IDeviceModelNode?           node2_1;
            OCPPv1_6.CP.IConfigurationKeysNode?  node1_6;

            lock (componentsLock)
            {
                node2_1 = deviceModelNode;
                node1_6 = configurationKeysNode;
            }

            if (tells2_1 && node2_1 is not null)
                Observe(node2_1.QueueSecurityEventAsync(OCPPv2_1.SecurityEventType.Parse(Type), TechInfo), Type);

            if (tells1_6 && node1_6 is not null)
                Observe(node1_6.QueueSecurityEventAsync(OCPPv1_6.SecurityEvent.Parse(Type),     TechInfo), Type);

        }

        #endregion

        #region (private) CheckMonitors()

        /// <summary>
        /// Ask the monitors the CSMS set whether what they watch changed.
        /// </summary>
        private void CheckMonitors()
        {

            OCPPv2_1.IDeviceModelNode? node2_1;

            lock (componentsLock)
            {
                node2_1 = deviceModelNode;
            }

            if (node2_1 is not null && node2_1.VariableMonitors.All.Count > 0)
                Observe(node2_1.CheckMonitorsAsync(), "monitor events");

        }

        #endregion

        #region (private) Observe(Sending, What)

        /// <summary>
        /// Let a sending run on, and write into the log where it failed: it was
        /// queued before, and is tried again.
        /// </summary>
        private void Observe(Task     Sending,
                             String   What)
        {
            Sending.ContinueWith(sending => node.Log.Warning($"Telling the CSMS of {What} failed: {sending.Exception?.GetBaseException().Message}", "ocpp"),
                                 TaskContinuationOptions.OnlyOnFaulted);
        }

        #endregion

    }

}
