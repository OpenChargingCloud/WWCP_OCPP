/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.tests.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.OverCBOR
{

    // A node is told, shows and keeps a message as JSON, however the message crossed
    // the wire: the events of its adapter, the frame of a log, and what waits in a
    // queue for a connection - which may be another one than the message was meant for.

    #region A CBOR frame, shown as JSON

    /// <summary>
    /// The frame of a CBOR connection, for a log: the JSON array it stands for.
    /// </summary>
    [TestFixture]
    public class ACBORFrameAsJSONTests
    {

        private static CBORValue Map(params (String Key, CBORValue Value)[] Entries)
            => CBORValue.FromMap(Entries.Select(entry => new KeyValuePair<CBORValue, CBORValue>(CBORValue.FromText(entry.Key), entry.Value)));

        [Test]
        public void ARequest_IsTheJSONArrayItStandsFor()
        {

            var frame = OCPP_CBORFrame.Join(
                            new JArray(2, "4711", "Reset", new JObject()),
                            Map(("type", CBORValue.FromText("OnIdle")), ("evseId", CBORValue.FromInt64(1)))
                        ).ToByteArray();

            Assert.That(OCPP_CBORFrame.TryAsJSON(frame, out var json, out var errorResponse), Is.True, errorResponse);

            Assert.That(JToken.DeepEquals(json,
                                          new JArray(2, "4711", "Reset", new JObject(new JProperty("type", "OnIdle"), new JProperty("evseId", 1)))),
                        Is.True,
                        json?.ToString(Newtonsoft.Json.Formatting.None));

        }

        [Test]
        public void AMessageOfTheOverlayNetwork_KeepsItsDestinationAndItsPath()
        {

            var frame = OCPP_CBORFrame.Join(
                            new JArray(2, new JArray("lc1", "csms1"), new JArray("cs1"), "4711", "Heartbeat", new JObject()),
                            Map()
                        ).ToByteArray();

            Assert.That(OCPP_CBORFrame.TryAsJSON(frame, out var json, out var errorResponse), Is.True, errorResponse);

            Assert.That(JToken.DeepEquals(json,
                                          new JArray(2, new JArray("lc1", "csms1"), new JArray("cs1"), "4711", "Heartbeat", new JObject())),
                        Is.True,
                        json?.ToString(Newtonsoft.Json.Formatting.None));

        }

        [Test]
        public void ABinaryPayload_IsShownAsText()
        {

            var bytes = new Byte[] { 0x01, 0x02, 0x03, 0xff };

            var frame = OCPP_CBORFrame.Join(
                            new JArray(2, "4711", "BinaryDataTransfer", new JObject()),
                            CBORValue.FromBytes(bytes)
                        ).ToByteArray();

            Assert.That(OCPP_CBORFrame.TryAsJSON(frame, out var json, out var errorResponse), Is.True, errorResponse);

            Assert.Multiple(() => {
                Assert.That(json!.Count,                 Is.EqualTo(4));
                Assert.That(json[3].Type,                Is.EqualTo(JTokenType.String));
                Assert.That(json[3].Value<String>(),     Is.EqualTo(bytes.ToBase64URL()));
            });

        }

        [Test]
        public void WhatIsNoMessage_IsNotShown()
        {

            Assert.Multiple(() => {

                Assert.That(OCPP_CBORFrame.TryAsJSON([ 0xff, 0x00 ], out var json1, out var e1),                           Is.False);
                Assert.That(json1,                                                                                         Is.Null);
                Assert.That(e1,                                                                                            Is.Not.Null);

                Assert.That(OCPP_CBORFrame.TryAsJSON(CBORValue.FromText("Reset").ToByteArray(), out _, out var e2),        Is.False);
                Assert.That(e2,                                                                                            Is.Not.Null);

            });

        }

    }

    #endregion

    #region The events of the adapter, over CBOR

    /// <summary>
    /// Over a CBOR connection a node is told of its messages as of JSON messages:
    /// who logs the events of the adapter logs the same, whatever the wire carried.
    /// </summary>
    [TestFixture]
    public class EventsOverCBORTests : AChargingStationTests
    {

        protected override MessageEncodings Encoding => MessageEncodings.CBOR;

        [Test]
        public async Task ARequestAndItsResponse_AreToldAsJSON_AtBothEnds()
        {

            OverCBOR.AgreedOnCBOR(chargingStation1);

            var requestsSent       = new ConcurrentList<OCPP_JSONRequestMessage>();
            var requestsReceived   = new ConcurrentList<OCPP_JSONRequestMessage>();
            var responsesSent      = new ConcurrentList<OCPP_JSONResponseMessage>();
            var responsesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
            var binary             = new ConcurrentList<String>();

            chargingStation1!.OCPP.OUT.OnJSONRequestMessageSent      += (timestamp, sender, connection, message, result, ct) => { requestsSent.     TryAdd(message); return Task.CompletedTask; };
            testCSMS1!.       OCPP.IN. OnJSONRequestMessageReceived  += (timestamp, sender, connection, message, ct)         => { requestsReceived. TryAdd(message); return Task.CompletedTask; };
            testCSMS1.        OCPP.OUT.OnJSONResponseMessageSent     += (timestamp, sender, connection, message, result, ct) => { responsesSent.    TryAdd(message); return Task.CompletedTask; };
            chargingStation1. OCPP.IN. OnJSONResponseMessageReceived += (timestamp, sender, connection, message, ct)         => { responsesReceived.TryAdd(message); return Task.CompletedTask; };

            chargingStation1. OCPP.OUT.OnBinaryRequestMessageSent      += (timestamp, sender, connection, message, result, ct) => { binary.TryAdd("request sent");      return Task.CompletedTask; };
            testCSMS1.        OCPP.IN. OnBinaryRequestMessageReceived  += (timestamp, sender, connection, message, ct)         => { binary.TryAdd("request received");  return Task.CompletedTask; };
            testCSMS1.        OCPP.OUT.OnBinaryResponseMessageSent     += (timestamp, sender, connection, message, result, ct) => { binary.TryAdd("response sent");     return Task.CompletedTask; };
            chargingStation1. OCPP.IN. OnBinaryResponseMessageReceived += (timestamp, sender, connection, message, ct)         => { binary.TryAdd("response received"); return Task.CompletedTask; };

            var response = await chargingStation1.SendSecurityEventNotification(
                                     Type:       SecurityEventType.Parse("TamperDetectionActivated"),
                                     Timestamp:  DateTimeOffset.Parse("2026-11-12T10:11:12.123Z"),
                                     TechInfo:   "over CBOR"
                                 );

            await EveryMessageHandled();

            Assert.That(response.Result.ResultCode, Is.EqualTo(ResultCode.OK));

            var sent      = requestsSent.     Where(message => message.Action.ToString() == "SecurityEventNotification").ToArray();
            var received  = requestsReceived. Where(message => message.Action.ToString() == "SecurityEventNotification").ToArray();

            Assert.Multiple(() => {

                Assert.That(sent,                                  Has.Length.EqualTo(1));
                Assert.That(received,                              Has.Length.EqualTo(1));

                Assert.That(binary,                                Is.Empty, "A CBOR message is no message of the binary format.");

            });

            var answered  = responsesSent.    Where(message => message.RequestId == sent[0].RequestId).ToArray();
            var heard     = responsesReceived.Where(message => message.RequestId == sent[0].RequestId).ToArray();

            Assert.Multiple(() => {

                Assert.That(received[0].RequestId,                 Is.EqualTo(sent[0].RequestId));

                Assert.That(sent[0].Payload["type"]?.     Value<String>(),  Is.EqualTo("TamperDetectionActivated"));
                Assert.That(sent[0].Payload["techInfo"]?. Value<String>(),  Is.EqualTo("over CBOR"));

                // What the CSMS is told is the JSON the charging station was told of.
                Assert.That(JToken.DeepEquals(received[0].Payload, sent[0].Payload),  Is.True,
                            $"sent: {sent[0].Payload.ToString(Newtonsoft.Json.Formatting.None)}, received: {received[0].Payload.ToString(Newtonsoft.Json.Formatting.None)}");

                Assert.That(answered,                              Has.Length.EqualTo(1));
                Assert.That(heard,                                 Has.Length.EqualTo(1));

                if (answered.Length == 1 && heard.Length == 1)
                    Assert.That(JToken.DeepEquals(heard[0].Payload, answered[0].Payload),  Is.True);

            });

        }

    }

    #endregion

    #region The queues, over CBOR

    /// <summary>
    /// What a charging station keeps for a back end is kept as JSON, and goes
    /// over the connection that is there when it goes - here one that speaks CBOR.
    /// </summary>
    [TestFixture]
    public class QueuesPerDestination_OverCBOR_Tests : QueuesPerDestinationTests
    {

        protected override MessageEncodings Encoding => MessageEncodings.CBOR;

        [Test]
        public async Task BothConnectionsSpeakCBOR()
        {

            var station      = await AStationConnectedToBoth("GD-QD-CBOR");

            var subprotocols = station.WWCPWebSocketClients.
                                   OfType<WWCPWebSocketClient>().
                                   Select(client => client.WebSocketClientConnection?.Subprotocol).
                                   ToArray();

            Assert.That(subprotocols,  Is.EqualTo(new[] { Version.WebSocketSubProtocolIdCBOR, Version.WebSocketSubProtocolIdCBOR }));

        }

    }

    /// <summary>
    /// A queue that was written to its file is read again and sent over CBOR.
    /// </summary>
    [TestFixture]
    public class AQueueFromItsFile_OverCBOR_Tests : AChargingStationTests
    {

        protected override MessageEncodings Encoding => MessageEncodings.CBOR;

        [Test]
        public async Task WhatWasKeptInAFile_IsSentOverCBOR()
        {

            OverCBOR.AgreedOnCBOR(chargingStation1);

            var file = Path.Combine(Path.GetTempPath(), $"ocpp-queue-{Guid.NewGuid():N}.jsonl");

            try
            {

                var told = new ConcurrentList<SecurityEventNotificationRequest>();

                testCSMS1!.OCPP.IN.OnSecurityEventNotificationRequestReceived += (timestamp, sender, connection, request, ct) => {
                    told.TryAdd(request);
                    return Task.CompletedTask;
                };

                // Kept while nothing may be sent, in a file.
                chargingStation1!.OfflineQueue      = new OfflineQueue(file);
                chargingStation1. HoldOfflineQueue  = true;

                await chargingStation1.QueueSecurityEventAsync(
                          SecurityEventType.Parse("KeptInAFile"),
                          "written as JSON",
                          DateTimeOffset.Parse("2026-11-12T10:11:12.123Z")
                      );

                Assert.That(File.Exists(file),                                    Is.True);
                Assert.That(told.Any(request => request.Type.ToString() == "KeptInAFile"),  Is.False);

                // The file says nothing of how its messages will be sent.
                var line = JObject.Parse(File.ReadAllLines(file).Single(text => text.Trim().Length > 0));

                Assert.Multiple(() => {
                    Assert.That(line["kind"]?.Value<String>(),                  Is.EqualTo("SecurityEventNotification"));
                    Assert.That(line["payload"]?["type"]?.Value<String>(),      Is.EqualTo("KeptInAFile"));
                    Assert.That(line.Properties().Select(property => property.Name),
                                Is.EquivalentTo(new[] { "id", "kind", "queued", "evidence", "payload" }));
                });

                // Another start: the queue is what its file says.
                chargingStation1.OfflineQueue      = new OfflineQueue(file);

                Assert.That(chargingStation1.OfflineQueue.Pending,               Has.Count.EqualTo(1));

                chargingStation1.HoldOfflineQueue  = false;

                var taken = await chargingStation1.FlushOfflineQueueAsync();

                await EveryMessageHandled();

                var kept = told.Where(request => request.Type.ToString() == "KeptInAFile").ToArray();

                Assert.Multiple(() => {
                    Assert.That(taken,                                           Is.EqualTo(1));
                    Assert.That(chargingStation1.OfflineQueue.Pending,           Is.Empty);
                    Assert.That(kept,                                            Has.Length.EqualTo(1));
                    Assert.That(kept.FirstOrDefault()?.TechInfo,                 Is.EqualTo("written as JSON"));
                    Assert.That(kept.FirstOrDefault()?.Timestamp,                Is.EqualTo(DateTimeOffset.Parse("2026-11-12T10:11:12.123Z")));
                });

            }
            finally
            {
                chargingStation1!.OfflineQueue = new OfflineQueue();
                File.Delete(file);
                File.Delete(file + ".new");
            }

        }

    }

    #endregion

}
