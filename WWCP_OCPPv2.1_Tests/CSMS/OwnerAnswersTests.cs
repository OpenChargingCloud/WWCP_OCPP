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
    /// A stuck cable and how the EVSEs stand are the owner's: whoever runs the
    /// charging station knows whether somebody is charging at a connector, and
    /// what to report of it. The test station knows neither, and used to be
    /// the only one asked - so a station built on it said "Unlocked" to every
    /// UnlockConnector, and reported the test station's EVSEs.
    /// </summary>
    [TestFixture]
    public class OwnerAnswersTests : AChargingStationTests
    {

        #region AnUnlockConnector_IsTheOwnersToAnswer_AndTheNodesWhereItSaysNothing()

        /// <summary>
        /// The owner says somebody charges at EVSE 1, and that is the answer;
        /// the owner says nothing of another, and the node answers as it did.
        /// </summary>
        [Test]
        public async Task AnUnlockConnector_IsTheOwnersToAnswer_AndTheNodesWhereItSaysNothing()
        {

            Assert.That(testCSMS1,         Is.Not.Null);
            Assert.That(chargingStation1,  Is.Not.Null);

            var asked = new List<EVSE_Id>();

            chargingStation1!.OnUnlockConnectorByOwner = (request, cancellationToken) => {

                asked.Add(request.EVSEId);

                return Task.FromResult<UnlockConnectorResponse?>(
                           request.EVSEId == EVSE_Id.Parse(1)
                               ? new UnlockConnectorResponse(request, UnlockStatus.OngoingAuthorizedTransaction)
                               : null
                       );

            };

            try
            {

                var charging  = await testCSMS1!.UnlockConnector(
                                          Destination:  SourceRouting.To(chargingStation1.Id),
                                          EVSEId:       EVSE_Id.     Parse(1),
                                          ConnectorId:  Connector_Id.Parse(1)
                                      );

                var unknown   = await testCSMS1.UnlockConnector(
                                          Destination:  SourceRouting.To(chargingStation1.Id),
                                          EVSEId:       EVSE_Id.     Parse(250),
                                          ConnectorId:  Connector_Id.Parse(1)
                                      );

                Assert.Multiple(() => {
                    Assert.That(charging.Status,  Is.EqualTo(UnlockStatus.OngoingAuthorizedTransaction),  "the owner's answer did not reach the CSMS");
                    Assert.That(unknown. Status,  Is.EqualTo(UnlockStatus.UnlockFailed),                  "an owner that said nothing was not followed by the node's own answer");
                    Assert.That(asked,            Is.EqualTo(new[] { EVSE_Id.Parse(1), EVSE_Id.Parse(250) }));
                });

            }
            finally
            {
                chargingStation1.OnUnlockConnectorByOwner = null;
            }

        }

        #endregion

        #region ATriggerMessage_IsTheOwnersToAnswerFirst_AndTheNodesWhereItSaysNothing()

        /// <summary>
        /// The owner answers a trigger for a StatusNotification without an
        /// EVSE, which the node rejects; the owner says nothing of a
        /// Heartbeat, and the node answers as it did.
        /// </summary>
        [Test]
        public async Task ATriggerMessage_IsTheOwnersToAnswerFirst_AndTheNodesWhereItSaysNothing()
        {

            Assert.That(testCSMS1,         Is.Not.Null);
            Assert.That(chargingStation1,  Is.Not.Null);

            var asked = new List<MessageTrigger>();

            chargingStation1!.OnTriggerMessageByOwner = (request, cancellationToken) => {

                asked.Add(request.RequestedMessage);

                return Task.FromResult<TriggerMessageResponse?>(
                           request.RequestedMessage == MessageTrigger.StatusNotification
                               ? new TriggerMessageResponse(request, TriggerMessageStatus.Accepted)
                               : null
                       );

            };

            try
            {

                var status     = await testCSMS1!.TriggerMessage(
                                           Destination:       SourceRouting.To(chargingStation1.Id),
                                           RequestedMessage:  MessageTrigger.StatusNotification
                                       );

                var heartbeat  = await testCSMS1.TriggerMessage(
                                           Destination:       SourceRouting.To(chargingStation1.Id),
                                           RequestedMessage:  MessageTrigger.Heartbeat
                                       );

                var meters     = await testCSMS1.TriggerMessage(
                                           Destination:       SourceRouting.To(chargingStation1.Id),
                                           RequestedMessage:  MessageTrigger.MeterValues
                                       );

                Assert.Multiple(() => {
                    Assert.That(status.   Status,  Is.EqualTo(TriggerMessageStatus.Accepted),  "the owner's answer did not reach the CSMS");
                    Assert.That(heartbeat.Status,  Is.EqualTo(TriggerMessageStatus.Accepted),  "an owner that said nothing was not followed by the node's own answer");
                    Assert.That(meters.   Status,  Is.EqualTo(TriggerMessageStatus.Rejected),  "an owner that said nothing was not followed by the node's own answer");
                    Assert.That(asked,             Is.EqualTo(new[] { MessageTrigger.StatusNotification, MessageTrigger.Heartbeat, MessageTrigger.MeterValues }));
                });

            }
            finally
            {
                chargingStation1.OnTriggerMessageByOwner = null;
            }

        }

        #endregion

    }

}
