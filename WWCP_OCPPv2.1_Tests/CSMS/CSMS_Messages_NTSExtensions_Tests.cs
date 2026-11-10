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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// SyncNetworkTime and TestTimeServer of the white paper "Secure Time
    /// Synchronization", section 3.2.7, over a connection: the CSMS asks, the
    /// station answers at once and tells the outcome afterwards in a
    /// NotifyNetworkTimeResult request carrying the CSMS's requestId.
    /// </summary>
    /// <remarks>
    /// The station's processors stand in for a node's time servers here, as
    /// the DeviceModelBridge of WWCP_OCPP_Node does in a node; its own tests
    /// ask real ones.
    /// </remarks>
    [TestFixture]
    public class CSMS_Messages_NTSExtensions_Tests : AChargingStationTests
    {

        #region ASyncNetworkTimeIsAnsweredAtOnceAndToldAfterwards()

        /// <summary>
        /// Accepted at once, and the outcome of the round arrives at the CSMS
        /// with the requestId it chose.
        /// </summary>
        [Test]
        public async Task ASyncNetworkTimeIsAnsweredAtOnceAndToldAfterwards()
        {

            Assert.That(testCSMS1,         Is.Not.Null);
            Assert.That(chargingStation1,  Is.Not.Null);

            var station  = chargingStation1!;
            var told     = Told(testCSMS1!);
            var asked    = new List<SyncNetworkTimeRequest>();

            station.SyncNetworkTimeProcessor = (timestamp, sender, connection, request, cancellationToken) => {

                asked.Add(request);

                // Told afterwards, as a node's round is.
                _ = Task.Run(async () => {

                    await Task.Delay(50);

                    await station.NotifyNetworkTimeResult(
                              request.NetworkTimeRequestId,
                              DateTimeOffset.UtcNow,
                              Round: new NetworkTimeRound(
                                         Group:      request.GroupOrLegal,
                                         OK:         true,
                                         Duration:   0.021m,
                                         Answered:   1,
                                         Required:   1,
                                         Clients:    [ new NetworkTimeClient("ptbtime1.ptb.de", true, "ptb1", -0.0021m, 0.0183m, NetworkTimeClient.ReusedKeyExchange) ],
                                         Offset:     -0.0021m,
                                         LegalTime:  true
                                     )
                          );

                });

                return Task.FromResult(SyncNetworkTimeResponse.Accepted(request));

            };

            var response      = await testCSMS1!.SyncNetworkTime(
                                          Destination:           SourceRouting.To(station.Id),
                                          NetworkTimeRequestId:  4711
                                      );

            var notification  = await told.Task.WaitAsync(TimeSpan.FromSeconds(10));

            Assert.Multiple(() => {

                Assert.That(response.Result.ResultCode,               Is.EqualTo(ResultCode.OK));
                Assert.That(response.Status,                          Is.EqualTo(GenericStatus.Accepted));

                Assert.That(asked,                                    Has.Count.EqualTo(1));
                Assert.That(asked[0].Group,                           Is.Null, "no group is the legal one, and none is sent");
                Assert.That(asked[0].GroupOrLegal,                    Is.EqualTo("legal"));

                Assert.That(notification.NetworkTimeRequestId,        Is.EqualTo(4711));
                Assert.That(notification.Test,                        Is.Null);
                Assert.That(notification.Round?.Group,                Is.EqualTo("legal"));
                Assert.That(notification.Round?.OK,                   Is.True);
                Assert.That(notification.Round?.LegalTime,            Is.True);
                Assert.That(notification.Round?.Clients.Single().Client,  Is.EqualTo("ptb1"));
                Assert.That(notification.Round?.Clients.Single().Offset,  Is.EqualTo(-0.0021m));

            });

        }

        #endregion

        #region ATestTimeServerTellsItsStepsAfterwards()

        /// <summary>
        /// The steps of a test arrive in the order they happened, with their
        /// stages, and the stage it ended at.
        /// </summary>
        [Test]
        public async Task ATestTimeServerTellsItsStepsAfterwards()
        {

            Assert.That(testCSMS1,         Is.Not.Null);
            Assert.That(chargingStation1,  Is.Not.Null);

            var station  = chargingStation1!;
            var told     = Told(testCSMS1!);

            station.TestTimeServerProcessor = (timestamp, sender, connection, request, cancellationToken) => {

                _ = Task.Run(async () => {

                    await Task.Delay(50);

                    await station.NotifyNetworkTimeResult(
                              request.NetworkTimeRequestId,
                              DateTimeOffset.UtcNow,
                              Test: new TimeServerTest(
                                        Host:      "ptbtime2.ptb.de",
                                        OK:        false,
                                        Duration:  3.096m,
                                        Steps:     [
                                                       new TimeServerTestStep(0.000m, "Configuration", "Info",   "Asking ptbtime2.ptb.de: key exchange on port 4460, time on port 123, 3 second(s) allowed."),
                                                       new TimeServerTestStep(0.094m, "KeyExchange",   "Notice", "The key exchange succeeded: AEAD_AES_SIV_CMAC_256, 8 cookie(s)."),
                                                       new TimeServerTestStep(3.096m, "TimeRequest",   "Error",  "The NTP request to 192.0.2.104:123 failed (NTPTimeout): no answer within 3 s.")
                                                   ],
                                        Client:    request.Client,
                                        FailedAt:  "TimeRequest"
                                    )
                          );

                });

                return Task.FromResult(TestTimeServerResponse.Accepted(request));

            };

            var response      = await testCSMS1!.TestTimeServer(
                                          Destination:           SourceRouting.To(station.Id),
                                          NetworkTimeRequestId:  4712,
                                          Client:                "ptb2"
                                      );

            var notification  = await told.Task.WaitAsync(TimeSpan.FromSeconds(10));

            Assert.Multiple(() => {

                Assert.That(response.Status,                          Is.EqualTo(GenericStatus.Accepted));

                Assert.That(notification.NetworkTimeRequestId,        Is.EqualTo(4712));
                Assert.That(notification.Round,                       Is.Null);
                Assert.That(notification.Test?.Client,                Is.EqualTo("ptb2"));
                Assert.That(notification.Test?.OK,                    Is.False);
                Assert.That(notification.Test?.FailedAt,              Is.EqualTo("TimeRequest"));
                Assert.That(notification.Test?.Steps.Select(step => step.Stage),
                            Is.EqualTo(new[] { "Configuration", "KeyExchange", "TimeRequest" }));

            });

        }

        #endregion

        #region AStationWithNothingToAskRejectsWithAReason()

        /// <summary>
        /// A station that is given no processors - no time servers - says so,
        /// and nothing follows.
        /// </summary>
        [Test]
        public async Task AStationWithNothingToAskRejectsWithAReason()
        {

            Assert.That(testCSMS1,         Is.Not.Null);
            Assert.That(chargingStation1,  Is.Not.Null);

            var station   = chargingStation1!;

            station.SyncNetworkTimeProcessor  = null;
            station.TestTimeServerProcessor   = null;

            var synced    = await testCSMS1!.SyncNetworkTime(SourceRouting.To(station.Id), 1);
            var tested    = await testCSMS1.  TestTimeServer (SourceRouting.To(station.Id), 2, "ptb1");

            Assert.Multiple(() => {
                Assert.That(synced.Status,                  Is.EqualTo(GenericStatus.Rejected));
                Assert.That(synced.StatusInfo?.ReasonCode,  Is.EqualTo("UnknownGroup"));
                Assert.That(tested.Status,                  Is.EqualTo(GenericStatus.Rejected));
                Assert.That(tested.StatusInfo?.ReasonCode,  Is.EqualTo("UnknownClient"));
            });

        }

        #endregion


        #region (private static) Told(CSMS)

        /// <summary>
        /// The first NotifyNetworkTimeResult request the CSMS receives.
        /// </summary>
        private static TaskCompletionSource<NotifyNetworkTimeResultRequest> Told(TestCSMSNode CSMS)
        {

            var told = new TaskCompletionSource<NotifyNetworkTimeResultRequest>(TaskCreationOptions.RunContinuationsAsynchronously);

            CSMS.OCPP.IN.OnNotifyNetworkTimeResultRequestReceived += (timestamp, sender, connection, request, cancellationToken) => {
                told.TrySetResult(request);
                return Task.CompletedTask;
            };

            return told;

        }

        #endregion

    }

}
