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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.CPwithCS
{

    /// <summary>
    /// A request whose cancellation token is cancelled while it waits for its
    /// answer stops waiting - as in OCPP 2.1, see its ARequestCalledOffStopsWaitingTests.
    /// </summary>
    /// <remarks>
    /// The wait was a loop of Task.Delay(25, the request's token), which spun a
    /// thread of the pool, throwing, from the cancellation until the request
    /// timed out. The token is cancelled from the request's SentAction, so the
    /// loop is reached with a cancelled token whatever the scheduler does.
    /// </remarks>
    [TestFixture]
    public class ARequestCalledOffStopsWaitingTests : ACPwithCSTests
    {

        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan StopsWithin    = TimeSpan.FromSeconds(3);


        #region AJSONRequestCalledOffStopsWaiting()

        [Test]
        public async Task AJSONRequestCalledOffStopsWaiting()
        {

            Assert.That(chargePoint, Is.Not.Null);

            using var calledOff  = new CancellationTokenSource();

            var took             = Stopwatch.StartNew();

            var sendRequestState = await chargePoint!.OCPP.OUT.SendJSONRequestAndWait(
                                             new OCPP_JSONRequestMessage(
                                                 RequestTimestamp:    Timestamp.Now,
                                                 EventTrackingId:     EventTracking_Id.New,
                                                 NetworkingMode:      NetworkingMode.Standard,
                                                 Destination:         SourceRouting.To(NetworkingNode_Id.CentralSystem),
                                                 NetworkPath:         NetworkPath.From(chargePoint.Id),
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

            Assert.That(chargePoint, Is.Not.Null);

            using var calledOff  = new CancellationTokenSource();

            var took             = Stopwatch.StartNew();

            var sendRequestState = await chargePoint!.OCPP.OUT.SendBinaryRequestAndWait(
                                             new OCPP_BinaryRequestMessage(
                                                 RequestTimestamp:    Timestamp.Now,
                                                 EventTrackingId:     EventTracking_Id.New,
                                                 NetworkingMode:      NetworkingMode.Standard,
                                                 Destination:         SourceRouting.To(NetworkingNode_Id.CentralSystem),
                                                 NetworkPath:         NetworkPath.From(chargePoint.Id),
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
