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

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// A restart is its owner's: whoever runs the charging station knows what
    /// restarting it means, and whether something is charging that it waits
    /// for. The test station restarts nothing, and used to be the only one
    /// asked - so a station built on it accepted every reset and did nothing.
    /// </summary>
    [TestFixture]
    public class ResetOwnerTests : AChargingStationTests
    {

        #region AReset_IsTheOwnersToAnswer_AndTheNodesWhereItSaysNothing()

        /// <summary>
        /// The owner says it restarts once nothing charges any more, and that is
        /// the answer; the owner says nothing, and the node answers as it did.
        /// </summary>
        [Test]
        public async Task AReset_IsTheOwnersToAnswer_AndTheNodesWhereItSaysNothing()
        {

            Assert.That(testCSMS1,         Is.Not.Null);
            Assert.That(chargingStation1,  Is.Not.Null);

            var asked = new List<ResetType>();

            chargingStation1!.OnResetByOwner = (request, cancellationToken) => {

                asked.Add(request.ResetType);

                return Task.FromResult<ResetResponse?>(
                           request.ResetType == ResetType.OnIdle
                               ? new ResetResponse(request, ResetStatus.Scheduled)
                               : null
                       );

            };

            try
            {

                var onIdle     = await testCSMS1!.Reset(
                                           Destination:  SourceRouting.To(chargingStation1.Id),
                                           ResetType:    ResetType.OnIdle
                                       );

                var immediate  = await testCSMS1.Reset(
                                           Destination:  SourceRouting.To(chargingStation1.Id),
                                           ResetType:    ResetType.Immediate
                                       );

                Assert.Multiple(() => {
                    Assert.That(onIdle.Status,     Is.EqualTo(ResetStatus.Scheduled), "the owner's answer did not reach the CSMS");
                    Assert.That(immediate.Status,  Is.EqualTo(ResetStatus.Accepted),  "an owner that said nothing was not followed by the node's own answer");
                    Assert.That(asked,             Is.EqualTo(new[] { ResetType.OnIdle, ResetType.Immediate }));
                });

            }
            finally
            {
                chargingStation1.OnResetByOwner = null;
            }

        }

        #endregion

    }

}
