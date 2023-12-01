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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

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

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(networkingNode1);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                networkingNode1         is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var nnResetRequests = new ConcurrentList<ResetRequest>();
                var csResetRequests = new ConcurrentList<ResetRequest>();

                networkingNode1.AsCS.OnResetRequest += (timestamp, sender, resetRequest) => {
                    nnResetRequests.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                chargingStation1.OnResetRequest += (timestamp, sender, resetRequest) => {
                    csResetRequests.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                var resetType  = ResetType.Immediate;
                var response   = await testCSMS01.Reset(
                                     NetworkingNodeId:    chargingStation1.Id,
                                     ResetType:           resetType,
                                     CustomData:          null
                                 );

                Assert.AreEqual(ResultCode.OK,          response.Result.ResultCode);
                Assert.AreEqual(ResetStatus.Accepted,   response.Status);

                Assert.AreEqual(1,                      nnResetRequests.Count);
                Assert.AreEqual(chargingStation1.Id,    nnResetRequests.First().NetworkingNodeId);
                Assert.AreEqual(resetType,              nnResetRequests.First().ResetType);

                Assert.AreEqual(1,                      csResetRequests.Count);
                Assert.AreEqual(chargingStation1.Id,    csResetRequests.First().NetworkingNodeId);
                Assert.AreEqual(resetType,              csResetRequests.First().ResetType);

            }

        }

        #endregion



    }

}
