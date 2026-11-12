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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.OverlayNetworking.OverlayNetwork.Default
{

    /// <summary>
    /// A local controller that dials its CSMS in the standard networking mode, with a charging station
    /// behind it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A message in the standard networking mode names no destination: the controller takes whatever comes
    /// over its uplink as its own. A request of the CSMS for the station behind it would be carried out by
    /// the controller and answered as if by the station - a Reset would reset the controller.
    /// </para>
    /// <para>
    /// So the CSMS sends a request for the station over that connection not at all, and is told the station
    /// is unknown or unreachable this way. To reach it, the controller dials in the overlay network.
    /// </para>
    /// </remarks>
    [TestFixture]
    [NonParallelizable]
    public class StandardUplink_Tests : ADefaultOverlayNetwork
    {

        protected override NetworkingMode UplinkNetworkingMode
            => NetworkingMode.Standard;


        #region ARequestForTheStationBehind_IsNotCarriedOutByTheController()

        /// <summary>
        /// A Reset for the station behind the controller reaches neither the controller nor the station: the
        /// CSMS is told the station is unreachable.
        /// </summary>
        [Test]
        public async Task ARequestForTheStationBehind_IsNotCarriedOutByTheController()
        {

            Assert.That(CSMS,             Is.Not.Null);
            Assert.That(localController,  Is.Not.Null);
            Assert.That(chargingStation,  Is.Not.Null);

            var atTheController  = new ConcurrentList<ResetRequest>();
            var atTheStation     = new ConcurrentList<ResetRequest>();

            localController!.OCPP.IN.OnResetRequestReceived += (timestamp, sender, connection, request, ct) => {
                atTheController.TryAdd(request);
                return Task.CompletedTask;
            };

            chargingStation!.OCPP.IN.OnResetRequestReceived += (timestamp, sender, connection, request, ct) => {
                atTheStation.TryAdd(request);
                return Task.CompletedTask;
            };

            // The station is behind the controller, as the CSMS was told or learned before.
            CSMS!.Routing.AddOrUpdateStaticRouting(
                chargingStation.Id,
                localController.Id
            );

            try
            {

                var response = await CSMS.Reset(
                                         Destination:  SourceRouting.To(chargingStation.Id),
                                         ResetType:    ResetType.Immediate
                                     );

                // Whatever was sent would have arrived by now.
                await Task.Delay(TimeSpan.FromMilliseconds(500));

                Assert.Multiple(() => {
                    Assert.That(atTheController.Count,       Is.EqualTo(0), $"the controller carried out a Reset for the station behind it: {response.Status} {response.Result}");
                    Assert.That(atTheStation.   Count,       Is.EqualTo(0));
                    Assert.That(response.Result.ResultCode,  Is.EqualTo(ResultCode.UnknownClient), response.Result.ToString());
                });

            }
            finally
            {
                CSMS.Routing.RemoveStaticRouting(chargingStation.Id, localController.Id);
            }

        }

        #endregion

        #region ARequestForTheController_ReachesIt()

        /// <summary>
        /// A Reset for the controller itself reaches it over its standard connection, as before.
        /// </summary>
        [Test]
        public async Task ARequestForTheController_ReachesIt()
        {

            Assert.That(CSMS,             Is.Not.Null);
            Assert.That(localController,  Is.Not.Null);

            var atTheController = new ConcurrentList<ResetRequest>();

            localController!.OCPP.IN.OnResetRequestReceived += (timestamp, sender, connection, request, ct) => {
                atTheController.TryAdd(request);
                return Task.CompletedTask;
            };

            var response = await CSMS!.Reset(
                                       Destination:  SourceRouting.To(localController.Id),
                                       ResetType:    ResetType.Immediate
                                   );

            Assert.Multiple(() => {
                Assert.That(response.Result.ResultCode,  Is.EqualTo(ResultCode.OK), response.Result.ToString());
                Assert.That(response.Status,             Is.EqualTo(ResetStatus.Accepted));
                Assert.That(atTheController.Count,       Is.EqualTo(1));
            });

        }

        #endregion

    }

}
