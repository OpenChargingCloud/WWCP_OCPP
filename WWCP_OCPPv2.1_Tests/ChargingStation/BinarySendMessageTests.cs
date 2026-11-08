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

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation
{

    /// <summary>
    /// A binary SEND - a message that wants no answer - is told from a binary CALL by its first byte, as
    /// the JSON messages tell them apart: 6 for a SEND, 2 for a CALL. With the same byte for both, a
    /// SEND was read as a request, which the receiver then answered.
    /// </summary>
    [TestFixture]
    public class BinarySendMessageTests : AChargingStationTests
    {

        #region ASendIsNoRequest_AndARequestNoSend

        /// <summary>
        /// The bytes of a SEND parse as a SEND and not as a request, and those of a request as a request
        /// and not as a SEND.
        /// </summary>
        [Test]
        public void ASendIsNoRequest_AndARequestNoSend()
        {

            var send     = new OCPP_BinarySendMessage(Timestamp.Now,
                                                      EventTracking_Id.New,
                                                      NetworkingMode.Standard,
                                                      SourceRouting.CSMS,
                                                      NetworkPath.Empty,
                                                      Request_Id.NewRandom(),
                                                      "TestSend",
                                                      "Hello world!".ToUTF8Bytes()).ToByteArray();

            var request  = new OCPP_BinaryRequestMessage(Timestamp.Now,
                                                         EventTracking_Id.New,
                                                         NetworkingMode.Standard,
                                                         SourceRouting.CSMS,
                                                         NetworkPath.Empty,
                                                         Request_Id.NewRandom(),
                                                         "TestRequest",
                                                         "Hello world!".ToUTF8Bytes()).ToByteArray();

            Assert.Multiple(() => {

                Assert.That(send[0],                                                          Is.EqualTo(6), "SEND, as in JSON");
                Assert.That(request[0],                                                       Is.EqualTo(2), "CALL, as in JSON");

                Assert.That(OCPP_BinarySendMessage.   TryParse(send,    out var asSend,    out var sendError),    Is.True,  sendError);
                Assert.That(asSend?.Action,                                                   Is.EqualTo("TestSend"));
                Assert.That(OCPP_BinaryRequestMessage.TryParse(send,    out _,             out _),                Is.False, "a SEND read as a request");

                Assert.That(OCPP_BinaryRequestMessage.TryParse(request, out var asRequest, out var requestError), Is.True,  requestError);
                Assert.That(asRequest?.Action,                                                Is.EqualTo("TestRequest"));
                Assert.That(OCPP_BinarySendMessage.   TryParse(request, out _,             out _),                Is.False, "a request read as a SEND");

            });

        }

        #endregion

        #region ABinarySend_ArrivesAsASend_NotAsARequest

        /// <summary>
        /// A binary SEND from a charging station reaches the CSMS as a SEND - and not as a request, which
        /// the CSMS would answer.
        /// </summary>
        [Test]
        public async Task ABinarySend_ArrivesAsASend_NotAsARequest()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,         Is.Not.Null);
                Assert.That(chargingStation1,  Is.Not.Null);
            });

            var sends     = new ConcurrentQueue<OCPP_BinarySendMessage>();
            var requests  = new ConcurrentQueue<OCPP_BinaryRequestMessage>();

            testCSMS1!.OCPP.IN.OnBinarySendMessageReceived     += (timestamp, sender, connection, message, ct) => { sends.   Enqueue(message); return Task.CompletedTask; };
            testCSMS1. OCPP.IN.OnBinaryRequestMessageReceived  += (timestamp, sender, connection, message, ct) => { requests.Enqueue(message); return Task.CompletedTask; };

            var messageId = Request_Id.NewRandom();

            await chargingStation1!.OCPP.OUT.SendBinarySendMessage(
                      new OCPP_BinarySendMessage(Timestamp.Now,
                                                 EventTracking_Id.New,
                                                 NetworkingMode.Standard,
                                                 SourceRouting.CSMS,
                                                 NetworkPath.From(chargingStation1.Id),
                                                 messageId,
                                                 "TestSend",
                                                 "Hello world!".ToUTF8Bytes())
                  );

            var until = DateTime.UtcNow + TimeSpan.FromSeconds(10);

            while (sends.IsEmpty && requests.IsEmpty && DateTime.UtcNow < until)
                await Task.Delay(50);

            Assert.Multiple(() => {
                Assert.That(requests.Select(request => request.Action),  Is.Empty,        "read as a request");
                Assert.That(sends.   Select(send    => send.MessageId),   Has.Member(messageId));
                Assert.That(sends.   Select(send    => send.Action),      Has.Member("TestSend"));
            });

        }

        #endregion

    }

}
