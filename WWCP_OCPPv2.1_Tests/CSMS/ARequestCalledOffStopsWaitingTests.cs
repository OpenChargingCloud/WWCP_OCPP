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

using System.Diagnostics;

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// A request whose cancellation token is cancelled while it waits for its
    /// answer stops waiting.
    /// </summary>
    /// <remarks>
    /// SendJSONRequestAndWait and SendBinaryRequestAndWait waited in a loop of
    /// Task.Delay(25, the request's token). Once that token was cancelled,
    /// every delay threw at once, the loop caught it and went round again, and
    /// the thread it ran on spun, throwing, until the request timed out -
    /// every request still waiting when its node was stopped. On the four-core
    /// runners of ChargingStationCLI's CI a dump of the test host showed ten of
    /// its thirteen busy pool threads in that loop and 119,472 cancellations
    /// on the heap, and tests that take a third of a second took twenty.
    ///
    /// The token is cancelled from the request's SentAction, which the adapter
    /// calls once the request is out and written down, so the loop is reached
    /// with a cancelled token whatever the scheduler does.
    /// </remarks>
    [TestFixture]
    public class ARequestCalledOffStopsWaitingTests : AChargingStationTests
    {

        /// <summary>
        /// Long enough that a request waiting for it is told apart from one that
        /// stopped when it was called off.
        /// </summary>
        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(10);

        /// <summary>
        /// What a request called off may take at most, scheduling included.
        /// </summary>
        private static readonly TimeSpan StopsWithin    = TimeSpan.FromSeconds(3);


        #region AJSONRequestCalledOffStopsWaiting()

        [Test]
        public async Task AJSONRequestCalledOffStopsWaiting()
        {

            Assert.That(chargingStation1, Is.Not.Null);

            using var calledOff  = new CancellationTokenSource();

            var took             = Stopwatch.StartNew();

            var sendRequestState = await chargingStation1!.OCPP.OUT.SendJSONRequestAndWait(
                                             new OCPP_JSONRequestMessage(
                                                 RequestTimestamp:    Timestamp.Now,
                                                 EventTrackingId:     EventTracking_Id.New,
                                                 NetworkingMode:      NetworkingMode.Standard,
                                                 Destination:         SourceRouting.CSMS,
                                                 NetworkPath:         NetworkPath.From(chargingStation1.Id),
                                                 RequestId:           Request_Id.NewRandom(),
                                                 Action:              "Heartbeat",
                                                 Payload:             new JObject(),
                                                 RequestTimeout:      Timestamp.Now + RequestTimeout,
                                                 CancellationToken:   calledOff.Token
                                             ),
                                             sentMessageResult => calledOff.Cancel()
                                         );

            took.Stop();

            Assert.Multiple(() => {
                Assert.That(calledOff.IsCancellationRequested,  Is.True,                "the request was never sent, which proves nothing either way");
                Assert.That(took.Elapsed,                       Is.LessThan(StopsWithin), "a request called off waited on");
                Assert.That(sendRequestState,                   Is.Not.Null);
            });

        }

        #endregion

        #region ABinaryRequestCalledOffStopsWaiting()

        [Test]
        public async Task ABinaryRequestCalledOffStopsWaiting()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,         Is.Not.Null);
                Assert.That(chargingStation1,  Is.Not.Null);
            });

            using var calledOff  = new CancellationTokenSource();

            var took             = Stopwatch.StartNew();

            var sendRequestState = await testCSMS1!.OCPP.OUT.SendBinaryRequestAndWait(
                                             new OCPP_BinaryRequestMessage(
                                                 RequestTimestamp:    Timestamp.Now,
                                                 EventTrackingId:     EventTracking_Id.New,
                                                 NetworkingMode:      NetworkingMode.Standard,
                                                 Destination:         SourceRouting.To(chargingStation1!.Id),
                                                 NetworkPath:         NetworkPath.From(testCSMS1.Id),
                                                 RequestId:           Request_Id.NewRandom(),
                                                 Action:              "BinaryDataTransfer",
                                                 Payload:             "Hello world!".ToUTF8Bytes(),
                                                 RequestTimeout:      Timestamp.Now + RequestTimeout,
                                                 CancellationToken:   calledOff.Token
                                             ),
                                             sentMessageResult => calledOff.Cancel()
                                         );

            took.Stop();

            Assert.Multiple(() => {
                Assert.That(calledOff.IsCancellationRequested,  Is.True,                "the request was never sent, which proves nothing either way");
                Assert.That(took.Elapsed,                       Is.LessThan(StopsWithin), "a request called off waited on");
                Assert.That(sendRequestState,                   Is.Not.Null);
            });

        }

        #endregion

    }

}
