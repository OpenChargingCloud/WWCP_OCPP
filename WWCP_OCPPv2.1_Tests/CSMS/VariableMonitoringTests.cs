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

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

using OfflineQueue = cloud.charging.open.protocols.OCPP.OfflineQueue;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// The monitors a CSMS sets on a charging station's variables, the events
    /// they send, and the queue security events and monitor events wait in
    /// while the CSMS is out of reach - over the wire.
    /// </summary>
    /// <remarks>
    /// The station's NTP controllers read an in-memory source, whose values a
    /// test changes; the station is then asked to check its monitors, as the
    /// station program asks it whenever its node says something changed.
    /// </remarks>
    [TestFixture]
    public class VariableMonitoringTests : AChargingStationTests
    {

        #region (helpers) Monitor(...) / Set(...) / Received(...)

        private static SetMonitoringData Monitor(MonitorType             Type,
                                                 String                  Component,
                                                 String?                 Instance,
                                                 String                  Variable,
                                                 Decimal                 Value      = 0,
                                                 Severities              Severity   = Severities.Alert,
                                                 VariableMonitoring_Id?  Id         = null)

            => new (Value, Type, Severity, new Component(Component, Instance), new Variable(Variable), Id);

        private async Task<SetMonitoringResult[]> Set(params SetMonitoringData[] Monitors)

            => [.. (await testCSMS1!.SetVariableMonitoring(SourceRouting.To(chargingStation1!.Id), Monitors)).SetMonitoringResults];

        /// <summary>
        /// The NotifyEvent requests the CSMS receives from now on.
        /// </summary>
        private ConcurrentList<NotifyEventRequest> Received()
        {

            var received = new ConcurrentList<NotifyEventRequest>();

            testCSMS1!.OCPP.IN.OnNotifyEventRequestReceived += (timestamp, sender, connection, request, ct) => {
                received.TryAdd(request);
                return Task.CompletedTask;
            };

            return received;

        }

        private IEnumerable<EventData> EventsIn(ConcurrentList<NotifyEventRequest> Received)
            => Received.SelectMany(request => request.EventData);

        #endregion


        #region ADeltaMonitorTellsAChangeOnce_Test()

        /// <summary>
        /// A Delta monitor on a group's Limited is told the change of it once, as
        /// a NotifyEvent naming the monitor, and nothing while it stays.
        /// </summary>
        [Test]
        public async Task ADeltaMonitorTellsAChangeOnce_Test()
        {

            var received = Received();
            var set      = await Set(Monitor(MonitorType.Delta, "NTPClientGroupController", "legal", "Limited"));

            var before   = await chargingStation1!.CheckMonitorsAsync();

            ntpSource1!.Group["legal"]["Limited"] = "true";

            var changed  = await chargingStation1.CheckMonitorsAsync();
            var again    = await chargingStation1.CheckMonitorsAsync();

            var told     = EventsIn(received).ToArray();

            Assert.Multiple(() => {

                Assert.That(set.Single().Status,             Is.EqualTo(SetMonitoringStatus.Accepted));
                Assert.That(set.Single().VariableMonitoringId, Is.Not.Null,  "the station gives a new monitor its id");

                Assert.That((before, changed, again),        Is.EqualTo((0, 1, 0)));
                Assert.That(told.Length,                     Is.EqualTo(1));
                Assert.That(told[0].Trigger,                 Is.EqualTo(EventTriggers.Delta));
                Assert.That(told[0].ActualValue,             Is.EqualTo("true"));
                Assert.That(told[0].Component.Instance,      Is.EqualTo("legal"));
                Assert.That(told[0].VariableMonitoringId,    Is.EqualTo(set.Single().VariableMonitoringId));
                Assert.That(told[0].EventNotificationType,   Is.EqualTo(EventNotificationType.CustomMonitor));

                Assert.That(chargingStation1.OfflineQueue.Pending,  Is.Empty,  "the CSMS took it");

            });

        }

        #endregion

        #region ANumberIsToldWhereItMovedByTheDelta_Test()

        /// <summary>
        /// A Delta monitor of a number tells a move of at least its value from
        /// where it told last, and not the smaller steps before it.
        /// </summary>
        [Test]
        public async Task ANumberIsToldWhereItMovedByTheDelta_Test()
        {

            var received = Received();

            await Set(Monitor(MonitorType.Delta, "NTPClientGroupController", "legal", "LastOffset", Value: 0.5m));

            ntpSource1!.Group["legal"]["LastOffset"] = "0.3";
            var small = await chargingStation1!.CheckMonitorsAsync();

            ntpSource1.Group["legal"]["LastOffset"] = "0.6";
            var large = await chargingStation1.CheckMonitorsAsync();

            Assert.Multiple(() => {
                Assert.That((small, large),                          Is.EqualTo((0, 1)));
                Assert.That(EventsIn(received).Single().ActualValue,  Is.EqualTo("0.6"));
            });

        }

        #endregion

        #region AThresholdIsToldWhenCrossedAndWhenCleared_Test()

        /// <summary>
        /// An UpperThreshold is told when the value goes above it, not again
        /// while it stays above, and once more, cleared, when it comes back.
        /// </summary>
        [Test]
        public async Task AThresholdIsToldWhenCrossedAndWhenCleared_Test()
        {

            var received = Received();

            await Set(Monitor(MonitorType.UpperThreshold, "NTPClientGroupController", "legal", "ErrorsInARow", Value: 3));

            ntpSource1!.Group["legal"]["ErrorsInARow"] = "5";
            var above  = await chargingStation1!.CheckMonitorsAsync();

            ntpSource1.Group["legal"]["ErrorsInARow"] = "6";
            var stays  = await chargingStation1.CheckMonitorsAsync();

            ntpSource1.Group["legal"]["ErrorsInARow"] = "1";
            var back   = await chargingStation1.CheckMonitorsAsync();

            var told   = EventsIn(received).ToArray();

            Assert.Multiple(() => {
                Assert.That((above, stays, back),  Is.EqualTo((1, 0, 1)));
                Assert.That(told[0].Trigger,       Is.EqualTo(EventTriggers.Alerting));
                Assert.That(told[0].Cleared,       Is.Null);
                Assert.That(told[1].Cleared,       Is.True);
                Assert.That(told[1].ActualValue,   Is.EqualTo("1"));
            });

        }

        #endregion

        #region AMonitorIsRefusedWhereItCannotBeSet_Test()

        /// <summary>
        /// Each monitor of a request on its own: refused for an unknown
        /// component or variable, a kind not supported, a variable that cannot
        /// be monitored, a threshold of a text, an id the station never gave,
        /// and one the same as a monitor there is.
        /// </summary>
        [Test]
        public async Task AMonitorIsRefusedWhereItCannotBeSet_Test()
        {

            var first   = await Set(Monitor(MonitorType.Delta, "NTPClientGroupController", "legal", "Enabled"));

            var results = await Set(
                              Monitor(MonitorType.Delta,          "NobodysController",         null,     "Enabled"),
                              Monitor(MonitorType.Delta,          "NTPClientGroupController",  "legal",  "Nothing"),
                              Monitor(MonitorType.Periodic,       "NTPClientGroupController",  "legal",  "Enabled", Value: 60),
                              Monitor(MonitorType.Delta,          "ClockCtrlr",                null,     "DateTime"),
                              Monitor(MonitorType.UpperThreshold, "NTPClientGroupController",  "legal",  "LegalTimeAuthority", Value: 1),
                              Monitor(MonitorType.Delta,          "NTPClientGroupController",  "legal",  "MinServers", Id: VariableMonitoring_Id.Parse(4711)),
                              Monitor(MonitorType.Delta,          "NTPClientGroupController",  "legal",  "Enabled")
                          );

            Assert.Multiple(() => {
                Assert.That(first.Single().Status,  Is.EqualTo(SetMonitoringStatus.Accepted));
                Assert.That(results.Select(result => result.Status),
                            Is.EqualTo(new[] {
                                SetMonitoringStatus.UnknownComponent,
                                SetMonitoringStatus.UnknownVariable,
                                SetMonitoringStatus.UnsupportedMonitorType,
                                SetMonitoringStatus.Rejected,
                                SetMonitoringStatus.Rejected,
                                SetMonitoringStatus.Rejected,
                                SetMonitoringStatus.Duplicate
                            }));
                Assert.That(chargingStation1!.VariableMonitors.All.Count,  Is.EqualTo(1),  "a refused monitor was kept");
            });

        }

        #endregion

        #region MonitorsAreReportedClearedAndLeveled_Test()

        /// <summary>
        /// GetMonitoringReport is followed by a NotifyMonitoringReport of the
        /// monitors; a monitor cleared is gone, one never set is not found,
        /// and a report of none is an empty result. A monitor less severe than
        /// the level set tells nothing, and a base without the CSMS's monitors
        /// takes them all away.
        /// </summary>
        [Test]
        public async Task MonitorsAreReportedClearedAndLeveled_Test()
        {

            var reports  = new ConcurrentList<NotifyMonitoringReportRequest>();

            testCSMS1!.OCPP.IN.OnNotifyMonitoringReportRequestReceived += (timestamp, sender, connection, request, ct) => {
                reports.TryAdd(request);
                return Task.CompletedTask;
            };

            var to       = SourceRouting.To(chargingStation1!.Id);
            var set      = await Set(Monitor(MonitorType.Delta,          "NTPClientGroupController", "legal", "Limited",      Severity: Severities.Warning),
                                     Monitor(MonitorType.UpperThreshold, "NTPClientGroupController", "legal", "ErrorsInARow", Value: 3));

            var report   = await testCSMS1.GetMonitoringReport(to, 17, [], [ new ComponentVariable(new Component("NTPClientGroupController", "legal")) ]);

            for (var waited = 0; reports.Count == 0 && waited < 10_000; waited += 20)
                await Task.Delay(20);

            // Warning is 7, Error 5: the Limited monitor tells nothing below Error.
            await testCSMS1.SetMonitoringLevel(to, Severities.Error);

            ntpSource1!.Group["legal"]["Limited"] = "true";
            var leveled  = await chargingStation1.CheckMonitorsAsync();

            var cleared  = await testCSMS1.ClearVariableMonitoring(to, [ set[0].VariableMonitoringId!.Value, VariableMonitoring_Id.Parse(4711) ]);
            await testCSMS1.SetMonitoringBase(to, MonitoringBase.HardWiredOnly);
            var empty    = await testCSMS1.GetMonitoringReport(to, 18, [], []);

            Assert.Multiple(() => {

                Assert.That(report.Status,  Is.EqualTo(GenericDeviceModelStatus.Accepted));
                Assert.That(reports.Single().NotifyMonitoringReportRequestId,  Is.EqualTo(17));
                Assert.That(reports.Single().MonitoringData.SelectMany(data => data.VariableMonitorings).Select(monitor => monitor.Type),
                            Is.EquivalentTo(new[] { MonitorType.Delta, MonitorType.UpperThreshold }));

                Assert.That(leveled,        Is.EqualTo(0),  "a monitor less severe than the level told");

                Assert.That(cleared.ClearMonitoringResults.Select(result => result.Status),
                            Is.EqualTo(new[] { ClearMonitoringStatus.Accepted, ClearMonitoringStatus.NotFound }));

                Assert.That(empty.Status,   Is.EqualTo(GenericDeviceModelStatus.EmptyResultSet));
                Assert.That(chargingStation1.VariableMonitors.All,  Is.Empty);

            });

        }

        #endregion

        #region ASecurityEventIsSentWithItsTechInfoCut_Test()

        /// <summary>
        /// A security event queued while the CSMS is there goes at once, with a
        /// techInfo cut at 255 characters, and leaves nothing in the queue.
        /// </summary>
        [Test]
        public async Task ASecurityEventIsSentWithItsTechInfoCut_Test()
        {

            var received = new ConcurrentList<SecurityEventNotificationRequest>();

            testCSMS1!.OCPP.IN.OnSecurityEventNotificationRequestReceived += (timestamp, sender, connection, request, ct) => {
                received.TryAdd(request);
                return Task.CompletedTask;
            };

            await chargingStation1!.QueueSecurityEventAsync(SecurityEventType.LegalTimeLimited, new String('x', 300));

            Assert.Multiple(() => {
                Assert.That(received.Single().Type,             Is.EqualTo(SecurityEventType.LegalTimeLimited));
                Assert.That(received.Single().TechInfo?.Length, Is.EqualTo(255));
                Assert.That(chargingStation1.OfflineQueue.Pending,  Is.Empty);
            });

        }

        #endregion

        #region AnEventKeptWhileTheCSMSIsAwayGoesAfterARestart_Test()

        /// <summary>
        /// A station that reaches no CSMS keeps its security event in the file of
        /// its queue; the same station started again, and reaching it, sends it
        /// - once.
        /// </summary>
        [Test]
        public async Task AnEventKeptWhileTheCSMSIsAwayGoesAfterARestart_Test()
        {

            var directory = Path.Combine(Path.GetTempPath(), "ocpp-queue-" + Guid.NewGuid().ToString("N")[..12]);
            var file      = Path.Combine(directory, "queue.jsonl");
            var received  = new ConcurrentList<SecurityEventNotificationRequest>();

            testCSMS1!.OCPP.IN.OnSecurityEventNotificationRequestReceived += (timestamp, sender, connection, request, ct) => {
                received.TryAdd(request);
                return Task.CompletedTask;
            };

            var id        = NetworkingNode_Id.Parse("GD-QUEUE1");
            var away      = Station(id, file);

            try
            {

                await away.QueueSecurityEventAsync(SecurityEventType.InvalidTimeServerCertificate, "ptbtime1.ptb.de: refused");

                var keptAway   = away.OfflineQueue.Pending.Count;
                var onDisk     = File.Exists(file);

                await away.Stop();

                var back       = Station(id, file);

                try
                {

                    var keptAfter = back.OfflineQueue.Pending.Count;

                    testCSMS1.AddOrUpdateHTTPBasicAuth(id, "1234abcd");

                    await back.ConnectOCPPWebSocketClient(
                              NextHopNetworkingNodeId:  NetworkingNode_Id.CSMS,
                              RemoteURL:                URL.Parse($"http://127.0.0.1:{testBackendWebSockets1!.IPPort}/{id}"),
                              HTTPAuthentication:       HTTPBasicAuthentication.Create(id.ToString(), "1234abcd"),
                              DisableWebSocketPings:    true
                          );

                    var sent = await back.FlushOfflineQueueAsync();

                    Assert.Multiple(() => {
                        Assert.That(keptAway,                        Is.EqualTo(1),  "nobody took it");
                        Assert.That(onDisk,                          Is.True);
                        Assert.That(keptAfter,                       Is.EqualTo(1),  "the queue did not outlive the restart");
                        Assert.That(sent,                            Is.EqualTo(1));
                        Assert.That(received.Single().Type,          Is.EqualTo(SecurityEventType.InvalidTimeServerCertificate));
                        Assert.That(received.Single().TechInfo,      Is.EqualTo("ptbtime1.ptb.de: refused"));
                        Assert.That(back.OfflineQueue.Pending,       Is.Empty);
                        Assert.That(new OfflineQueue(file).Pending,  Is.Empty,  "the file still holds it");
                    });

                }
                finally
                {
                    await back.Stop();
                }

            }
            finally
            {
                try { Directory.Delete(directory, recursive: true); } catch (IOException) { }
            }

        }

        private TestChargingStationNode Station(NetworkingNode_Id  Id,
                                                String             QueueFile)

            => new (Id:                     Id,
                    VendorName:             "GraphDefined OEM #1",
                    Model:                  "VCP.Q",
                    Description:            I18NString.Create(Languages.en, "A station whose CSMS is away"),
                    DisableSendHeartbeats:  true,
                    HTTPAPI_Disabled:       true,
                    WebAPI_Disabled:        true,
                    DNSClient:              testCSMS1!.DNSClient) {
                   OfflineQueue = new OfflineQueue(QueueFile)
               };

        #endregion

    }

}
