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

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.CentralSystem
{

    /// <summary>
    /// SyncNetworkTime and TestTimeServer of OCPP 1.6 (Secure Time
    /// Synchronization, section 3.3.5) over a connection: answered at once,
    /// with a reason where nothing is asked, and told afterwards.
    /// </summary>
    [TestFixture]
    public class CS_Messages_NTSExtensions_Tests : AChargePointTests
    {

        #region ATestTimeServerNamesItsServerAndIsToldAfterwards()

        /// <summary>
        /// OCPP 1.6 names the server by its group and its name, and is told
        /// the outcome without client ids, which it does not have.
        /// </summary>
        [Test]
        public async Task ATestTimeServerNamesItsServerAndIsToldAfterwards()
        {

            Assert.That(testCentralSystem01,  Is.Not.Null);
            Assert.That(chargePoint1,         Is.Not.Null);

            var chargePoint  = chargePoint1!;
            var told         = new TaskCompletionSource<CP.NotifyNetworkTimeResultRequest>(TaskCreationOptions.RunContinuationsAsynchronously);
            var asked        = new List<CS.TestTimeServerRequest>();
            CS.NotifyNetworkTimeResultResponse? sent = null;

            testCentralSystem01!.OCPP.IN.OnNotifyNetworkTimeResultRequestReceived += (timestamp, sender, connection, request, ct) => {
                told.TrySetResult(request);
                return Task.CompletedTask;
            };

            chargePoint.TestTimeServerProcessor = (timestamp, sender, connection, request, ct) => {

                asked.Add(request);

                _ = Task.Run(async () => {

                    await Task.Delay(50);

                    sent = await chargePoint.OCPP.OUT.NotifyNetworkTimeResult(
                              new CP.NotifyNetworkTimeResultRequest(
                                  Destination:           SourceRouting.To(NetworkingNode_Id.CentralSystem),
                                  NetworkTimeRequestId:  request.NetworkTimeRequestId,
                                  GeneratedAt:           DateTimeOffset.UtcNow,
                                  Test:                  new TimeServerTest(
                                                             Host:      request.Server,
                                                             OK:        true,
                                                             Duration:  0.094m,
                                                             Steps:     [ new TimeServerTestStep(0.094m, "TimeRequest", "Notice", "Answered.") ],
                                                             Offset:    -0.0021m
                                                         ),
                                  RequestId:             chargePoint.OCPP.NextRequestId,
                                  RequestTimeout:        chargePoint.OCPP.DefaultRequestTimeout
                              )
                          );

                });

                return Task.FromResult(CP.TestTimeServerResponse.Accepted(request));

            };

            var response      = await testCentralSystem01.TestTimeServer(
                                          Destination:           SourceRouting.To(chargePoint.Id),
                                          NetworkTimeRequestId:  4712,
                                          Server:                "ptbtime2.ptb.de",
                                          Group:                 "local"
                                      );

            if (await Task.WhenAny(told.Task, Task.Delay(TimeSpan.FromSeconds(10))) != told.Task)
                Assert.Fail($"The central system was told nothing; the charge point's sending came back with: {sent?.Result}");

            var notification  = await told.Task;

            Assert.Multiple(() => {

                Assert.That(response.Result.ResultCode,          Is.EqualTo(ResultCode.OK));
                Assert.That(response.Status,                     Is.EqualTo(GenericStatus.Accepted));

                Assert.That(asked.Single().Server,               Is.EqualTo("ptbtime2.ptb.de"));
                Assert.That(asked.Single().Group,                Is.EqualTo("local"));

                Assert.That(notification.NetworkTimeRequestId,   Is.EqualTo(4712));
                Assert.That(notification.Test?.Client,           Is.Null, "OCPP 1.6 has no client ids");
                Assert.That(notification.Test?.Host,             Is.EqualTo("ptbtime2.ptb.de"));
                Assert.That(notification.Test?.OK,               Is.True);

            });

        }

        #endregion

        #region ARejectionSaysWhyInOCPP16Too()

        /// <summary>
        /// A charge point given no processors says why it asks nobody - with a
        /// status info, which responses of OCPP 1.6 otherwise lack.
        /// </summary>
        [Test]
        public async Task ARejectionSaysWhyInOCPP16Too()
        {

            Assert.That(testCentralSystem01,  Is.Not.Null);
            Assert.That(chargePoint1,         Is.Not.Null);

            chargePoint1!.SyncNetworkTimeProcessor  = null;
            chargePoint1. TestTimeServerProcessor   = null;

            var synced  = await testCentralSystem01!.SyncNetworkTime(SourceRouting.To(chargePoint1.Id), 1);
            var tested  = await testCentralSystem01. TestTimeServer (SourceRouting.To(chargePoint1.Id), 2, "ptbtime1.ptb.de");

            Assert.Multiple(() => {
                Assert.That(synced.Status,                  Is.EqualTo(GenericStatus.Rejected));
                Assert.That(synced.StatusInfo?.ReasonCode,  Is.EqualTo("UnknownGroup"));
                Assert.That(tested.Status,                  Is.EqualTo(GenericStatus.Rejected));
                Assert.That(tested.StatusInfo?.ReasonCode,  Is.EqualTo("UnknownServer"));
            });

        }

        #endregion

    }

}
