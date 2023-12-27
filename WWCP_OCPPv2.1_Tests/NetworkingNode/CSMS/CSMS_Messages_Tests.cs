/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.NetworkingNode.CSMS
{

    /// <summary>
    /// Unit tests for charging stations sending messages to the CSMS.
    /// </summary>
    [TestFixture]
    public class CSMS_Messages_Tests : AChargingStationWithNetworkingNodeTests
    {

        #region Reset_ChargingStation_Test()

        /// <summary>
        /// A test for sending a reset message to a charging station.
        /// </summary>
        [Test]
        public async Task Reset_ChargingStation_Test()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS01,                       Is.Not.Null);
                Assert.That(testBackendWebSockets01,          Is.Not.Null);
                Assert.That(networkingNode1,                  Is.Not.Null);
//                Assert.That(testNetworkingNodeWebSockets01,   Is.Not.Null);
                Assert.That(chargingStation1,                 Is.Not.Null);
                Assert.That(chargingStation2,                 Is.Not.Null);
                Assert.That(chargingStation3,                 Is.Not.Null);
            });

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                networkingNode1         is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var csmsResetRequests   = new ConcurrentList<ResetRequest>();
                var nnResetRequestsIN   = new ConcurrentList<ResetRequest>();
                var nnResetRequestsFWD  = new ConcurrentList<Tuple<ResetRequest, ForwardingDecision<ResetRequest, ResetResponse>>>();
                var nnResetRequestsOUT  = new ConcurrentList<ResetRequest>();
                var csResetRequests     = new ConcurrentList<ResetRequest>();

                testCSMS01.             OnResetRequest += (timestamp, sender,             resetRequest) => {
                    csmsResetRequests.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                networkingNode1.IN.     OnResetRequest += (timestamp, sender, connection, resetRequest) => {
                    nnResetRequestsIN.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                networkingNode1.FORWARD.OnResetLogging += (timestamp, sender, connection, resetRequest, forwardingDecision) => {
                    nnResetRequestsFWD.TryAdd(new Tuple<ResetRequest, ForwardingDecision<ResetRequest, ResetResponse>>(resetRequest, forwardingDecision));
                    return Task.CompletedTask;
                };

                networkingNode1.OUT.    OnResetRequest += (timestamp, sender,             resetRequest) => {
                    nnResetRequestsOUT.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                chargingStation1.       OnResetRequest += (timestamp, sender, connection, resetRequest) => {
                    csResetRequests.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                // Charging Station 1 is reachable via the networking node 1!
                // Good old "static routing" ;)
                testCSMS01.AddRedirect(chargingStation1.Id,
                                       networkingNode1.Id);


                var resetType  = ResetType.Immediate;
                var response   = await testCSMS01.Reset(
                                     DestinationNodeId:   chargingStation1.Id,
                                     ResetType:           resetType,
                                     CustomData:          null
                                 );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                      Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                 Is.EqualTo(ResetStatus.Accepted));

                    Assert.That(csmsResetRequests. Count,                        Is.EqualTo(1), "The ResetRequest did not leave the CSMS!");

                    Assert.That(nnResetRequestsIN. Count,                        Is.EqualTo(1), "The ResetRequest did not reach the INPUT of the networking node!");
                    Assert.That(nnResetRequestsIN. First().DestinationNodeId,    Is.EqualTo(chargingStation1.Id));
                    Assert.That(nnResetRequestsIN. First().NetworkPath.Length,   Is.EqualTo(1));
                    Assert.That(nnResetRequestsIN. First().NetworkPath.Source,   Is.EqualTo(testCSMS01.      Id.ToNetworkingNodeId));
                    Assert.That(nnResetRequestsIN. First().NetworkPath.Last,     Is.EqualTo(testCSMS01.      Id.ToNetworkingNodeId));

                    Assert.That(nnResetRequestsFWD.Count,                        Is.EqualTo(1), "The ResetRequest did not reach the FORWARD of the networking node!");

                    Assert.That(nnResetRequestsOUT.Count,                        Is.EqualTo(1), "The ResetRequest did not reach the OUTPUT of the networking node!");
                    Assert.That(nnResetRequestsOUT.First().DestinationNodeId,    Is.EqualTo(chargingStation1.Id));
                    Assert.That(nnResetRequestsOUT.First().NetworkPath.Length,   Is.EqualTo(1));
                    Assert.That(nnResetRequestsOUT.First().NetworkPath.Source,   Is.EqualTo(testCSMS01.      Id.ToNetworkingNodeId));
                    Assert.That(nnResetRequestsOUT.First().NetworkPath.Last,     Is.EqualTo(testCSMS01.      Id.ToNetworkingNodeId));

                    Assert.That(csResetRequests.   Count,                        Is.EqualTo(1), "The ResetRequest did not reach the charging station!");
                    // Because of 'standard' networking mode towards the charging station!
                    Assert.That(csResetRequests.   First().DestinationNodeId,    Is.EqualTo(NetworkingNode_Id.Zero));
                    Assert.That(csResetRequests.   First().NetworkPath.Length,   Is.EqualTo(0));

                });

            }

        }

        #endregion



    }

}
