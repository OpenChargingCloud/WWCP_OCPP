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

using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.CentralSystem
{

    /// <summary>
    /// The security events a charge point queues for its central system, over
    /// the wire.
    /// </summary>
    [TestFixture]
    public class SecurityEventTests : AChargePointTests
    {

        #region AQueuedSecurityEventIsSentWithItsTechInfoCut_Test()

        /// <summary>
        /// A security event of the time paper, queued while the central system
        /// is there, goes at once with a techInfo cut at 255 characters, and
        /// leaves nothing in the queue.
        /// </summary>
        [Test]
        public async Task AQueuedSecurityEventIsSentWithItsTechInfoCut_Test()
        {

            var received = new ConcurrentList<SecurityEventNotificationRequest>();

            testCentralSystem01!.OCPP.IN.OnSecurityEventNotificationRequestReceived += (timestamp, sender, connection, request, ct) => {
                received.TryAdd(request);
                return Task.CompletedTask;
            };

            await chargePoint1!.QueueSecurityEventAsync(SecurityEvent.TimeServerCertificateChanged, new String('x', 300));

            Assert.Multiple(() => {
                Assert.That(received.Single().Type,              Is.EqualTo(SecurityEvent.TimeServerCertificateChanged));
                Assert.That(received.Single().TechInfo?.Length,  Is.EqualTo(255));
                Assert.That(chargePoint1.OfflineQueue.Pending,   Is.Empty);
            });

        }

        #endregion

        #region AnEventNobodyTakesStaysInTheQueue_Test()

        /// <summary>
        /// A charge point that reaches no central system keeps its event.
        /// </summary>
        [Test]
        public async Task AnEventNobodyTakesStaysInTheQueue_Test()
        {

            await chargePoint2!.QueueSecurityEventAsync(SecurityEvent.LegalTimeLimited, "legal: 10 errors in a row");

            Assert.That(chargePoint2.OfflineQueue.Pending.Single().Payload.Value<String>("type"),  Is.EqualTo("LegalTimeLimited"));

        }

        #endregion

    }

}
