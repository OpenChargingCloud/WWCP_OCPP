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

using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.CentralSystem
{

    /// <summary>
    /// A restart is its owner's: whoever runs the charge point knows what
    /// restarting it means, and whether something is charging that it waits
    /// for. The test charge point restarts nothing, and used to be the only one
    /// asked - so a charge point built on it accepted every reset and did
    /// nothing.
    /// </summary>
    [TestFixture]
    public class ResetOwnerTests : AChargePointTests
    {

        #region AReset_IsTheOwnersToAnswer_AndTheNodesWhereItSaysNothing()

        /// <summary>
        /// The owner refuses a hard reset, and that is the answer; the owner says
        /// nothing of a soft one, and the node answers as it did.
        /// </summary>
        [Test]
        public async Task AReset_IsTheOwnersToAnswer_AndTheNodesWhereItSaysNothing()
        {

            Assert.That(testCentralSystem01,  Is.Not.Null);
            Assert.That(chargePoint1,         Is.Not.Null);

            var asked = new List<ResetType>();

            chargePoint1!.OnResetByOwner = (request, cancellationToken) => {

                asked.Add(request.ResetType);

                return Task.FromResult<ResetResponse?>(
                           request.ResetType == ResetType.Hard
                               ? new ResetResponse(request, ResetStatus.Rejected)
                               : null
                       );

            };

            try
            {

                var hard  = await testCentralSystem01!.Reset(
                                      Destination:  SourceRouting.To(chargePoint1.Id),
                                      ResetType:    ResetType.Hard
                                  );

                var soft  = await testCentralSystem01.Reset(
                                      Destination:  SourceRouting.To(chargePoint1.Id),
                                      ResetType:    ResetType.Soft
                                  );

                Assert.Multiple(() => {
                    Assert.That(hard.Status,  Is.EqualTo(ResetStatus.Rejected), "the owner's answer did not reach the central system");
                    Assert.That(soft.Status,  Is.EqualTo(ResetStatus.Accepted), "an owner that said nothing was not followed by the node's own answer");
                    Assert.That(asked,        Is.EqualTo(new[] { ResetType.Hard, ResetType.Soft }));
                });

            }
            finally
            {
                chargePoint1.OnResetByOwner = null;
            }

        }

        #endregion

    }

}
