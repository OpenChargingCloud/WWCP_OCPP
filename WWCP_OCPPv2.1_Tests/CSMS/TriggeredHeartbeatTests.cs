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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// A CSMS asks a charging station for a heartbeat with TriggerMessage: OCPP 2.0.1 and
    /// 2.1 list Heartbeat in MessageTriggerEnumType. The test station used to answer
    /// "Rejected", as if it were no longer allowed.
    /// </summary>
    [TestFixture]
    public class TriggeredHeartbeatTests : AChargingStationTests
    {

        #region AHeartbeatAskedFor_IsAccepted_AndSent()

        /// <summary>
        /// The station accepts the trigger and sends the CSMS a heartbeat.
        /// </summary>
        [Test]
        public async Task AHeartbeatAskedFor_IsAccepted_AndSent()
        {

            Assert.That(testCSMS1,         Is.Not.Null);
            Assert.That(chargingStation1,  Is.Not.Null);

            var heartbeat = new TaskCompletionSource<HeartbeatRequest>(TaskCreationOptions.RunContinuationsAsynchronously);

            OCPPv2_1.NetworkingNode.OnHeartbeatRequestReceivedDelegate received = (timestamp, sender, connection, request, cancellationToken) => {
                if (request.NetworkPath.Source == chargingStation1!.Id)
                    heartbeat.TrySetResult(request);
                return Task.CompletedTask;
            };

            testCSMS1!.OCPP.IN.OnHeartbeatRequestReceived += received;

            try
            {

                var response = await testCSMS1.TriggerMessage(
                                         Destination:       SourceRouting.To(chargingStation1!.Id),
                                         RequestedMessage:  MessageTrigger.Heartbeat
                                     );

                var arrived  = await Task.WhenAny(heartbeat.Task, Task.Delay(TimeSpan.FromSeconds(5))) == heartbeat.Task;

                Assert.Multiple(() => {
                    Assert.That(response.Result.ResultCode,  Is.EqualTo(ResultCode.OK), $"{response.Result}");
                    Assert.That(response.Status,             Is.EqualTo(TriggerMessageStatus.Accepted));
                    Assert.That(arrived,                     Is.True, "No heartbeat reached the CSMS within five seconds.");
                });

            }
            finally
            {
                testCSMS1!.OCPP.IN.OnHeartbeatRequestReceived -= received;
            }

        }

        #endregion

    }

}
