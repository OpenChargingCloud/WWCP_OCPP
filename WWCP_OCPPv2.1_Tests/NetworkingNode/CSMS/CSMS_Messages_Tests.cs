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
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

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

            ClassicAssert.IsNotNull(testCSMS01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(networkingNode1);
            ClassicAssert.IsNotNull(chargingStation1);
            ClassicAssert.IsNotNull(chargingStation2);
            ClassicAssert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                networkingNode1         is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var nnResetRequests = new ConcurrentList<ResetRequest>();
                var csResetRequests = new ConcurrentList<ResetRequest>();

                networkingNode1.AsCS.OnResetRequest += (timestamp, sender, connection, resetRequest) => {
                    nnResetRequests.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                chargingStation1.OnResetRequest += (timestamp, sender, connection, resetRequest) => {
                    csResetRequests.TryAdd(resetRequest);
                    return Task.CompletedTask;
                };

                var resetType  = ResetType.Immediate;
                var response   = await testCSMS01.Reset(
                                     NetworkingNodeId:    chargingStation1.Id,
                                     ResetType:           resetType,
                                     CustomData:          null
                                 );

                ClassicAssert.AreEqual(ResultCode.OK,          response.Result.ResultCode);
                ClassicAssert.AreEqual(ResetStatus.Accepted,   response.Status);

                ClassicAssert.AreEqual(1,                      nnResetRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.Id,    nnResetRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual(resetType,              nnResetRequests.First().ResetType);

                ClassicAssert.AreEqual(1,                      csResetRequests.Count);
                ClassicAssert.AreEqual(chargingStation1.Id,    csResetRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual(resetType,              csResetRequests.First().ResetType);

            }

        }

        #endregion



    }

}
