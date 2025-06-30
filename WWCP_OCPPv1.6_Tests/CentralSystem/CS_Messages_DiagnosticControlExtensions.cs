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

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.CentralSystem
{

    /// <summary>
    /// Unit tests for a central system sending messages to charge points.
    /// </summary>
    [TestFixture]
    public class CS_Messages_DiagnosticControlExtensions_Tests : AChargePointTests
    {

        #region CentralSystem_SwipeRFIDCardRequest_Test()

        /// <summary>
        /// A test for sending a SwipeRFIDCardRequest.
        /// </summary>
        [Test]
        public async Task CentralSystem_SwipeRFIDCardRequest_Test()
        {

            Assert.Multiple(() => {
                Assert.That(testCentralSystem01,      Is.Not.Null);
                Assert.That(testBackendWebSockets01,  Is.Not.Null);
                Assert.That(chargePoint1,             Is.Not.Null);
                Assert.That(chargePoint2,             Is.Not.Null);
                Assert.That(chargePoint3,             Is.Not.Null);
            });

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1            is not null &&
                chargePoint2            is not null &&
                chargePoint3            is not null)
            {

                var notifyWebPaymentStartedRequests = new List<DataTransferRequest>();

                chargePoint1.OCPP.IN.OnDataTransferRequestReceived += (timestamp, sender, connection, notifyWebPaymentStartedRequest, ct) => {
                    notifyWebPaymentStartedRequests.Add(notifyWebPaymentStartedRequest);
                    return Task.CompletedTask;
                };

                var response   = await testCentralSystem01.SwipeRFIDCard(
                                           Destination:      SourceRouting.To(chargePoint1.Id),
                                           IdTag:            IdToken.     Parse("aabbccdd"),
                                           ReaderId:         Connector_Id.Parse(1),
                                           ProcessingDelay:  TimeSpan.    FromMilliseconds(50)
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                                        Is.EqualTo(DataTransferStatus.Accepted));
                    //Assert.That(response.Data,                                                          Is.Not.Null);
                    //Assert.That(response.Data?.Type,                                                    Is.EqualTo(JTokenType.Object));
                    //Assert.That(response.Data?["key"]?.Value<String>()?.Reverse(),                      Is.EqualTo(data["key"]?.Value<String>()));

                    Assert.That(notifyWebPaymentStartedRequests.Count,                                  Is.EqualTo(1));
                    var notifyWebPaymentStartedRequest = notifyWebPaymentStartedRequests.First();
                    Assert.That(notifyWebPaymentStartedRequest.DestinationId,                           Is.EqualTo(chargePoint1.Id));

                });

            }

        }

        #endregion

    }

}
