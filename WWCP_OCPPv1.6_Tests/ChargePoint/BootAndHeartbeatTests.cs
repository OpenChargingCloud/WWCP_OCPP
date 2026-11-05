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
using System.Diagnostics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint
{

    /// <summary>
    /// What a charge point does with the answer to its BootNotification, and
    /// with a queue it was told to hold.
    /// </summary>
    /// <remarks>
    /// An accepted BootNotification starts the heartbeats at the interval it
    /// gives, and a heartbeat is sent. A pending one starts none: its interval
    /// is how long to wait before the next BootNotification. Until one is
    /// accepted, a charge point may tell its central system nothing else, and
    /// a queue held keeps what it is given until it is let go.
    ///
    /// A plain WebSocket server stands in for the central system, so that it
    /// can answer Pending, which the test central system never does.
    /// </remarks>
    [TestFixture]
    public class BootAndHeartbeatTests
    {

        #region (class) CentralSystem

        /// <summary>
        /// A central system that answers a BootNotification as it is told to,
        /// a Heartbeat with its time and anything else with nothing, and keeps
        /// every frame it is sent.
        /// </summary>
        private sealed class CentralSystem : IAsyncDisposable
        {

            private readonly ConcurrentQueue<JArray> received = new();

            public WebSocketServer  Server     { get; }

            public String           Status     { get; set; } = "Accepted";
            public Int32            Interval   { get; set; } = 5;

            public CentralSystem()
            {

                Server = new WebSocketServer(RequireAuthentication: false, DisableWebSocketPings: true, AutoStart: true);

                Server.OnTextMessageReceived += async (timestamp, server, connection, frame, eventTrackingId, text, cancellationToken) => {

                    var message = JArray.Parse(text);

                    received.Enqueue(message);

                    if (message[0].Value<Int32>() != 2)
                        return;

                    var answer = message[2].Value<String>() switch {

                                     "BootNotification"  => new JObject(
                                                                new JProperty("status",       Status),
                                                                new JProperty("currentTime",  DateTimeOffset.UtcNow.ToString("o")),
                                                                new JProperty("interval",     Interval)
                                                            ),

                                     "Heartbeat"         => new JObject(
                                                                new JProperty("currentTime",  DateTimeOffset.UtcNow.ToString("o"))
                                                            ),

                                     _                   => new JObject()

                                 };

                    await Server.SendTextMessage(connection, new JArray(3, message[1], answer).ToString(Formatting.None));

                };

            }

            public URL URL
                => URL.Parse($"http://127.0.0.1:{Server.IPPort}/CP-BOOT");

            /// <summary>
            /// The first request of the given action it was sent, once there
            /// is one or the given time has passed.
            /// </summary>
            public async Task<JArray?> Sent(String Action, TimeSpan Within)
            {

                var watch = Stopwatch.StartNew();

                do
                {

                    var found = received.FirstOrDefault(message => message[0].Value<Int32>() == 2 &&
                                                                   message[2].Value<String>() == Action);

                    if (found is not null)
                        return found;

                    await Task.Delay(20);

                }
                while (watch.Elapsed < Within);

                return null;

            }

            public async ValueTask DisposeAsync()
                => await Server.Shutdown();

        }

        #endregion

        #region Data

        private CentralSystem?        centralSystem;
        private TestChargePointNode?  chargePoint;

        #endregion

        #region SetUp / TearDown

        [SetUp]
        public void MakeACentralSystem()
        {
            centralSystem = new CentralSystem();
        }

        [TearDown]
        public async Task TakeItAwayAgain()
        {

            if (chargePoint is not null)
                await chargePoint.Stop();

            if (centralSystem is not null)
                await centralSystem.DisposeAsync();

            chargePoint    = null;
            centralSystem  = null;

        }

        #endregion

        #region (private) Connected(DisableSendHeartbeats)

        /// <summary>
        /// A charge point connected to the central system.
        /// </summary>
        private async Task<TestChargePointNode> Connected(Boolean DisableSendHeartbeats)
        {

            chargePoint = new TestChargePointNode(
                              ChargeBoxId:            NetworkingNode_Id.Parse("CP-BOOT"),
                              Connectors:             [ new ConnectorSpec(Availabilities.Operative, PhysicalReference: "#1", MaxPower: Watt.FromKW(11)) ],
                              ChargePointVendor:      "GraphDefined",
                              ChargePointModel:       "VCP.Boot",
                              DisableSendHeartbeats:  DisableSendHeartbeats
                          );

            var response = await chargePoint.ConnectOCPPWebSocketClient(
                                     RemoteURL:                centralSystem!.URL,
                                     NextHopNetworkingNodeId:  NetworkingNode_Id.CentralSystem,
                                     DisableWebSocketPings:    true
                                 );

            Assert.That(response.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols), "the charge point did not connect");

            return chargePoint;

        }

        #endregion


        #region AnAcceptedBootNotificationStartsTheHeartbeatsAtItsInterval()

        /// <summary>
        /// A charge point built without heartbeats sends them once its
        /// BootNotification is accepted, every interval it was given.
        /// </summary>
        [Test]
        public async Task AnAcceptedBootNotificationStartsTheHeartbeatsAtItsInterval()
        {

            var chargePoint = await Connected(DisableSendHeartbeats: true);

            var response    = await chargePoint.SendBootNotification();

            Assert.Multiple(() => {
                Assert.That(response.Status,                     Is.EqualTo(RegistrationStatus.Accepted));
                Assert.That(chargePoint.DisableSendHeartbeats,  Is.False);
                Assert.That(chargePoint.SendHeartbeatsEvery,    Is.EqualTo(TimeSpan.FromSeconds(5)));
            });

            Assert.That(await centralSystem!.Sent("Heartbeat", TimeSpan.FromSeconds(10)), Is.Not.Null, "no heartbeat was sent");

        }

        #endregion

        #region APendingBootNotificationStartsNoHeartbeats()

        /// <summary>
        /// A pending BootNotification stops the heartbeats of a charge point
        /// built with them: its interval is how long to wait before the next
        /// BootNotification.
        /// </summary>
        [Test]
        public async Task APendingBootNotificationStartsNoHeartbeats()
        {

            centralSystem!.Status = "Pending";

            var chargePoint = await Connected(DisableSendHeartbeats: false);

            var response    = await chargePoint.SendBootNotification();

            Assert.Multiple(() => {
                Assert.That(response.Status,                     Is.EqualTo(RegistrationStatus.Pending));
                Assert.That(chargePoint.DisableSendHeartbeats,  Is.True, "a pending charge point sends heartbeats");
            });

        }

        #endregion

        #region AHeldQueueWaitsUntilItIsLetGo()

        /// <summary>
        /// A security event queued while the queue is held stays in it, and
        /// goes once the queue is let go and sent.
        /// </summary>
        [Test]
        public async Task AHeldQueueWaitsUntilItIsLetGo()
        {

            var chargePoint = await Connected(DisableSendHeartbeats: true);

            chargePoint.HoldOfflineQueue = true;

            await chargePoint.QueueSecurityEventAsync(SecurityEvent.TimeServerCertificateChanged, "held");

            Assert.Multiple(async () => {
                Assert.That(await centralSystem!.Sent("SecurityEventNotification", TimeSpan.FromMilliseconds(500)),  Is.Null, "a held queue sent its event");
                Assert.That(chargePoint.OfflineQueue.Pending,                                                       Has.Count.EqualTo(1));
            });

            chargePoint.HoldOfflineQueue = false;

            var taken = await chargePoint.FlushOfflineQueueAsync();

            Assert.Multiple(async () => {
                Assert.That(taken,                                                                                 Is.EqualTo(1));
                Assert.That(await centralSystem!.Sent("SecurityEventNotification", TimeSpan.FromSeconds(5)),       Is.Not.Null);
                Assert.That(chargePoint.OfflineQueue.Pending,                                                       Is.Empty);
            });

        }

        #endregion

    }

}
