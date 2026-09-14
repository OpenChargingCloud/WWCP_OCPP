/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.LC;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.OverlayNetworking.OverlayNetwork.Default
{

    /// <summary>
    /// A BootNotification addressed to NetworkingNode_Id.CSMS is for the CSMS itself,
    /// so it is delivered to IN and never to the forwarding processor.
    /// </summary>
    [TestFixture]
    [NonParallelizable]
    public class CS_2_CSMS_Delivery : ADefaultOverlayNetwork
    {

        [Test]
        public async Task ACSMSAddressedRequestIsDeliveredNotForwarded()
        {

            Assert.That(chargingStation,  Is.Not.Null);
            Assert.That(CSMS,             Is.Not.Null);

            var csmsJsonIn    = new ConcurrentList<String>();
            var csmsForward   = new ConcurrentList<String>();
            var csmsIn        = new ConcurrentList<String>();

            CSMS!.OCPP.IN.     OnJSONRequestMessageReceived        += (t, s, c, m, ct) => { csmsJsonIn. TryAdd(m.Destination.ToString()); return Task.CompletedTask; };
            CSMS. OCPP.FORWARD.OnBootNotificationRequestFiltered   += (t, s, c, q, d, ct) => { csmsForward.TryAdd(q.ChargingStation.Model); return Task.CompletedTask; };
            CSMS. OCPP.IN.     OnBootNotificationRequestReceived   += (t, s, c, q, ct) => { csmsIn.     TryAdd(q.ChargingStation.Model); return Task.CompletedTask; };

            var response = await chargingStation!.SendBootNotification(BootReason.PowerUp);

            Assert.Multiple(() => {

                Assert.That(csmsJsonIn.Count, Is.EqualTo(1), "The BootNotification did not reach the CSMS!");

                // NetworkingNode_Id.CSMS is one of this node's anycast identifications, so a
                // request addressed to it is for this node. It is delivered to IN and must never
                // reach FORWARD - a test that counts BootNotifications on the forwarding
                // processor counts a message that was delivered correctly as missing.
                Assert.That(CSMS.OCPP.IN.AnycastIds, Does.Contain(NetworkingNode_Id.CSMS));
                Assert.That(csmsForward.Count, Is.EqualTo(0), "A request addressed to this node must not be forwarded!");
                Assert.That(csmsIn.     Count, Is.EqualTo(1), "The BootNotification was not delivered to the CSMS!");

                Assert.That(response.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response.Status,            Is.EqualTo(RegistrationStatus.Accepted));

            });

        }

    }

}
