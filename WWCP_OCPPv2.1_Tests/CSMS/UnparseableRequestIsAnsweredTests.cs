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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// A request the receiver cannot parse has to be answered, not swallowed.
    /// </summary>
    /// <remarks>
    /// Every factory on OCPP_JSONRequestErrorMessage builds its error with
    /// SourceRouting.Zero, because the place where parsing fails knows the request
    /// identification and the action but not who sent them. A message addressed to nobody is
    /// dropped as "UnknownClient", so the sender used to wait out its whole request timeout
    /// for an answer that had been written and thrown away - and could not tell a rejected
    /// message from a lost one.
    ///
    /// Found through TriggerMessage_Test, where a charging station sent a StatusNotification
    /// with an empty connectorStatus and never heard back. That particular message is valid
    /// again, so this test produces a malformed one deliberately: it is the addressing that
    /// is under test here, not any one message type.
    /// </remarks>
    [TestFixture]
    public class UnparseableRequestIsAnsweredTests : AChargingStationTests
    {

        #region AMalformedRequestIsAnsweredWithARequestError()

        [Test]
        public async Task AMalformedRequestIsAnsweredWithARequestError()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,         Is.Not.Null);
                Assert.That(chargingStation1,  Is.Not.Null);
            });

            // A StatusNotification the CSMS cannot parse: connectorStatus is empty.
            var sendRequestState = await chargingStation1!.OCPP.OUT.SendJSONRequestAndWait(
                                             new OCPP_JSONRequestMessage(
                                                 RequestTimestamp:   Timestamp.Now,
                                                 EventTrackingId:    EventTracking_Id.New,
                                                 NetworkingMode:     NetworkingMode.Standard,
                                                 Destination:        SourceRouting.CSMS,
                                                 NetworkPath:        NetworkPath.From(chargingStation1.Id),
                                                 RequestId:          Request_Id.NewRandom(),
                                                 Action:             "StatusNotification",
                                                 Payload:            new JObject(
                                                                         new JProperty("timestamp",         Timestamp.Now.ToISO8601()),
                                                                         new JProperty("connectorStatus",   ""),
                                                                         new JProperty("evseId",            1),
                                                                         new JProperty("connectorId",       1)
                                                                     ),
                                                 RequestTimeout:     Timestamp.Now + TimeSpan.FromSeconds(10)
                                             )
                                         );

            Assert.Multiple(() => {

                Assert.That(sendRequestState.JSONRequestErrorMessage,  Is.Not.Null,
                            "The CSMS could not parse the request and answered with nothing at all!");

                Assert.That(sendRequestState.JSONRequestErrorMessage?.ErrorCode,
                                                                       Is.EqualTo(ResultCode.FormationViolation));

                // Nothing to assert about the destination here: in standard networking mode the
                // error frame is [4, messageId, errorCode, description, details] and carries no
                // destination at all, so what arrives always reads as Zero. The addressing is what
                // lets it be routed in the first place - that it arrives is the whole proof.

            });

        }

        #endregion

    }

}
