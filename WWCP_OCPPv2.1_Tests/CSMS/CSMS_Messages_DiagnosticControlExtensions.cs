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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// Unit tests for a central system sending messages to charge points.
    /// </summary>
    [TestFixture]
    public class CS_Messages_DiagnosticControlExtensions_Tests : AChargingStationTests
    {

        #region CentralSystem_SwipeRFIDCardRequest_Test()

        /// <summary>
        /// A test for sending a SwipeRFIDCardRequest.
        /// </summary>
        [Test]
        public async Task CentralSystem_SwipeRFIDCardRequest_Test()
        {

            //Assert.Multiple(() => {
            //    Assert.That(testCentralSystem01,      Is.Not.Null);
            //    Assert.That(testBackendWebSockets01,  Is.Not.Null);
            //    Assert.That(chargePoint1,             Is.Not.Null);
            //    Assert.That(chargePoint2,             Is.Not.Null);
            //    Assert.That(chargePoint3,             Is.Not.Null);
            //});

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var notifyWebPaymentStartedRequests = new List<DataTransferRequest>();

                chargingStation1.OCPP.IN.OnDataTransferRequestReceived += (timestamp, sender, connection, notifyWebPaymentStartedRequest, ct) => {
                    notifyWebPaymentStartedRequests.Add(notifyWebPaymentStartedRequest);
                    return Task.CompletedTask;
                };

                var response   = await testCSMS1.SwipeRFIDCard(
                                           Destination:      SourceRouting.To(chargingStation1.Id),
                                           IdToken:          IdToken. TryParseRFID4("aabbccdd")!,
                                           ReaderId:         EVSE_Id. Parse(1),
                                           ProcessingDelay:  TimeSpan.FromMilliseconds(50)
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                                        Is.EqualTo(DataTransferStatus.Accepted));
                    //Assert.That(response.Data,                                                          Is.Not.Null);
                    //Assert.That(response.Data?.Type,                                                    Is.EqualTo(JTokenType.Object));
                    //Assert.That(response.Data?["key"]?.Value<String>()?.Reverse(),                      Is.EqualTo(data["key"]?.Value<String>()));

                    Assert.That(notifyWebPaymentStartedRequests.Count,                                  Is.EqualTo(1));
                    var notifyWebPaymentStartedRequest = notifyWebPaymentStartedRequests.First();
                    Assert.That(notifyWebPaymentStartedRequest.DestinationId,                           Is.EqualTo(chargingStation1.Id));

                });

            }

        }

        #endregion

    }

}
