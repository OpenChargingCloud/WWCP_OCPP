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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Styx;
using Telegram.Bot.Types;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.tests
{

    /// <summary>
    /// Unit tests for charging stations sending messages to the CSMS.
    /// </summary>
    [TestFixture]
    public class ChargingStationMessagesTests : AChargingStationTests
    {

        #region ChargingStation_Init_Test()

        /// <summary>
        /// A test for creating charging stations.
        /// </summary>
        [Test]
        public void ChargingStation_Init_Test()
        {

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01     is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                Assert.AreEqual("GraphDefined OEM #1",  chargingStation1.Vendor);
                Assert.AreEqual("GraphDefined OEM #2",  chargingStation2.Vendor);
                Assert.AreEqual("GraphDefined OEM #3",  chargingStation3.Vendor);

            }

        }

        #endregion

        #region ChargingStation_SendBootNotifications_Test()

        /// <summary>
        /// A test for sending boot notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task ChargingStation_SendBootNotifications_Test()
        {

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01     is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var bootNotificationRequests = new List<CS.BootNotificationRequest>();

                testCSMS01.OnBootNotificationRequest += async (timestamp, sender, bootNotificationRequest) => {
                    bootNotificationRequests.Add(bootNotificationRequest);
                };

                var response1 = await chargingStation1.SendBootNotification(BootReasons.PowerUp);

                Assert.AreEqual(ResultCodes.OK,                            response1.Result.ResultCode);
                Assert.AreEqual(RegistrationStatus.Accepted,               response1.Status);

                Assert.AreEqual(1,                                         bootNotificationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,              bootNotificationRequests.First().ChargeBoxId);
                //Assert.AreEqual(chargingStation1.ChargingStationVendor,        bootNotificationRequests.First().ChargingStationVendor);
                //Assert.AreEqual(chargingStation1.ChargingStationSerialNumber,  bootNotificationRequests.First().ChargingStationSerialNumber);
                //Assert.AreEqual(chargingStation1.ChargeBoxSerialNumber,    bootNotificationRequests.First().ChargeBoxSerialNumber);
                //Assert.AreEqual(chargingStation1.Iccid,                    bootNotificationRequests.First().Iccid);
                //Assert.AreEqual(chargingStation1.IMSI,                     bootNotificationRequests.First().IMSI);
                //Assert.AreEqual(chargingStation1.MeterType,                bootNotificationRequests.First().MeterType);
                //Assert.AreEqual(chargingStation1.MeterSerialNumber,        bootNotificationRequests.First().MeterSerialNumber);

            }

        }

        #endregion

        #region ChargingStation_SendSendHeartbeats_Test()

        /// <summary>
        /// A test for sending heartbeats to the CSMS.
        /// </summary>
        [Test]
        public async Task ChargingStation_SendSendHeartbeats_Test()
        {

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01     is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var heartbeatRequests = new List<CS.HeartbeatRequest>();

                testCSMS01.OnHeartbeatRequest += async (timestamp, sender, heartbeatRequest) => {
                    heartbeatRequests.Add(heartbeatRequest);
                };


                var response1 = await chargingStation1.SendHeartbeat();


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.IsTrue  (Timestamp.Now - response1.CurrentTime < TimeSpan.FromSeconds(10));

                Assert.AreEqual(1,                              heartbeatRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   heartbeatRequests.First().ChargeBoxId);

            }

        }

        #endregion


        #region ChargingStation_Authorize_Test()

        /// <summary>
        /// A test for authorizing id tokens against the CSMS.
        /// </summary>
        [Test]
        public async Task ChargingStation_Authorize_Test()
        {

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01     is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var authorizeRequests = new List<CS.AuthorizeRequest>();

                testCSMS01.OnAuthorizeRequest += async (timestamp, sender, authorizeRequest) => {
                    authorizeRequests.Add(authorizeRequest);
                };

                var idToken   = IdToken.NewRandomRFID();
                var response1 = await chargingStation1.Authorize(idToken);


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(AuthorizationStatus.Accepted,   response1.IdTokenInfo.Status);

                Assert.AreEqual(1,                              authorizeRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   authorizeRequests.First().ChargeBoxId);
                Assert.AreEqual(idToken,                        authorizeRequests.First().IdToken);

            }

        }

        #endregion

        #region ChargingStation_TransactionEvent_Test()

        /// <summary>
        /// A test for sending a transaction event to the CSMS.
        /// </summary>
        [Test]
        public async Task ChargingStation_TransactionEvent_Test()
        {

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01     is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var transactionEventRequests = new List<CS.TransactionEventRequest>();

                testCSMS01.OnTransactionEventRequest += async (timestamp, sender, transactionEventRequest) => {
                    transactionEventRequests.Add(transactionEventRequest);
                };

                var evseId          = EVSE_Id.     Parse(1);
                var connectorId     = Connector_Id.Parse(1);
                var idToken         = IdToken.     NewRandomRFID();
                var startTimestamp  = Timestamp.Now;
                var meterStart      = 1234UL;
                var reservationId   = Reservation_Id.NewRandom;

                var response1       = await chargingStation1.SendTransactionEvent(

                                          EventType:            TransactionEvents.Started,
                                          Timestamp:            startTimestamp,
                                          TriggerReason:        TriggerReasons.Authorized,
                                          SequenceNumber:       0,
                                          TransactionInfo:      new Transaction(
                                                                    TransactionId:       Transaction_Id.NewRandom,
                                                                    ChargingState:       ChargingStates.Charging,
                                                                    TimeSpentCharging:   TimeSpan.FromSeconds(3),
                                                                    StoppedReason:       null,
                                                                    RemoteStartId:       null,
                                                                    CustomData:          null
                                                                ),

                                          Offline:              null,
                                          NumberOfPhasesUsed:   null,
                                          CableMaxCurrent:      null,
                                          ReservationId:        reservationId,
                                          IdToken:              idToken,
                                          EVSE:                 new EVSE(
                                                                    Id:                  evseId,
                                                                    ConnectorId:         connectorId,
                                                                    CustomData:          null
                                                                ),
                                          MeterValues:          new MeterValue[] {
                                                                    new MeterValue(
                                                                        SampledValues:   new SampledValue[] {

                                                                                             new SampledValue(
                                                                                                 Value:              meterStart,
                                                                                                 Context:            ReadingContexts.TransactionBegin,
                                                                                                 Measurand:          Measurands.Energy_Active_Export_Interval,
                                                                                                 Phase:              Phases.L1,
                                                                                                 Location:           MeasurementLocations.Outlet,
                                                                                                 SignedMeterValue:   new SignedMeterValue(
                                                                                                                         SignedMeterData:   meterStart.ToString(),
                                                                                                                         SigningMethod:     "secp256r1",
                                                                                                                         EncodingMethod:    "base64",
                                                                                                                         PublicKey:         "0x1234",
                                                                                                                         CustomData:        null
                                                                                                                     ),
                                                                                                 UnitOfMeasure:      UnitsOfMeasure.kW(
                                                                                                                         Multiplier:   0,
                                                                                                                         CustomData:   null
                                                                                                                     ),
                                                                                                 CustomData:         null
                                                                                             )

                                                                                         },
                                                                        Timestamp:       startTimestamp,
                                                                        CustomData:      null
                                                                    )
                                                                },
                                          CustomData:           null

                                      );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                //Assert.AreEqual(AuthorizationStatus.Accepted,   response1.IdTokenInfo.Status);
                //Assert.IsTrue  (response1.TransactionId.IsNotNullOrEmpty);

                Assert.AreEqual(1,                              transactionEventRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   transactionEventRequests.First().ChargeBoxId);
                //Assert.AreEqual(connectorId,                    transactionEventRequests.First().ConnectorId);
                //Assert.AreEqual(idToken,                        transactionEventRequests.First().IdTag);
                //Assert.AreEqual(startTimestamp.ToIso8601(),     transactionEventRequests.First().StartTimestamp.ToIso8601());
                //Assert.AreEqual(meterStart,                     transactionEventRequests.First().MeterStart);
                //Assert.AreEqual(reservationId,                  transactionEventRequests.First().ReservationId);

            }

        }

        #endregion


    }

}
