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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Styx;

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

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                Assert.AreEqual("GraphDefined OEM #1",  chargingStation1.VendorName);
                Assert.AreEqual("GraphDefined OEM #2",  chargingStation2.VendorName);
                Assert.AreEqual("GraphDefined OEM #3",  chargingStation3.VendorName);

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

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var bootNotificationRequests = new List<CS.BootNotificationRequest>();

                testCSMS01.OnBootNotificationRequest += async (timestamp, sender, bootNotificationRequest) => {
                    bootNotificationRequests.Add(bootNotificationRequest);
                };

                var reason     = BootReasons.PowerUp;
                var response1  = await chargingStation1.SendBootNotification(reason);

                Assert.AreEqual (ResultCodes.OK,                         response1.Result.ResultCode);
                Assert.AreEqual (RegistrationStatus.Accepted,            response1.Status);

                Assert.AreEqual (1,                                      bootNotificationRequests.Count);
                Assert.AreEqual (chargingStation1.ChargeBoxId,           bootNotificationRequests.First().ChargeBoxId);
                Assert.AreEqual (reason,                                 bootNotificationRequests.First().Reason);

                var chargingStation = bootNotificationRequests.First().ChargingStation;

                Assert.IsNotNull(chargingStation);
                if (chargingStation is not null)
                {

                    Assert.AreEqual(chargingStation1.Model,              chargingStation.Model);
                    Assert.AreEqual(chargingStation1.VendorName,         chargingStation.VendorName);
                    Assert.AreEqual(chargingStation1.SerialNumber,       chargingStation.SerialNumber);
                    Assert.AreEqual(chargingStation1.FirmwareVersion,    chargingStation.FirmwareVersion);

                    var modem = chargingStation.Modem;

                    Assert.IsNotNull(modem);
                    if (modem is not null)
                    {
                        Assert.AreEqual(chargingStation1.Modem!.ICCID,   modem.ICCID);
                        Assert.AreEqual(chargingStation1.Modem!.IMSI,    modem.IMSI);
                    }

                }

            }

        }

        #endregion

        #region ChargingStation_SendHeartbeats_Test()

        /// <summary>
        /// A test for sending heartbeats to the CSMS.
        /// </summary>
        [Test]
        public async Task ChargingStation_SendHeartbeats_Test()
        {

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
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

        #region ChargingStation_StatusNotification_Test()

        /// <summary>
        /// A test for sending status notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task ChargingStation_StatusNotification_Test()
        {

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var statusNotificationRequests = new List<CS.StatusNotificationRequest>();

                testCSMS01.OnStatusNotificationRequest += async (timestamp, sender, statusNotificationRequest) => {
                    statusNotificationRequests.Add(statusNotificationRequest);
                };

                var evseId           = EVSE_Id.     Parse(1);
                var connectorId      = Connector_Id.Parse(1);
                var connectorStatus  = ConnectorStatus.Available;
                var statusTimestamp  = Timestamp.Now;

                var response1        = await chargingStation1.SendStatusNotification(
                                           EVSEId:        evseId,
                                           ConnectorId:   connectorId,
                                           Timestamp:     statusTimestamp,
                                           Status:        connectorStatus,
                                           CustomData:    null
                                       );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              statusNotificationRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   statusNotificationRequests.First().ChargeBoxId);
                Assert.AreEqual(evseId,                         statusNotificationRequests.First().EVSEId);
                Assert.AreEqual(connectorId,                    statusNotificationRequests.First().ConnectorId);
                Assert.AreEqual(connectorStatus,                statusNotificationRequests.First().ConnectorStatus);
                Assert.AreEqual(statusTimestamp.ToIso8601(),    statusNotificationRequests.First().Timestamp.ToIso8601());

            }

        }

        #endregion

        #region ChargingStation_SendMeterValues_Test()

        /// <summary>
        /// A test for sending meter values to the CSMS.
        /// </summary>
        [Test]
        public async Task ChargingStation_SendMeterValues_Test()
        {

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var meterValuesRequests = new List<CS.MeterValuesRequest>();

                testCSMS01.OnMeterValuesRequest += async (timestamp, sender, meterValuesRequest) => {
                    meterValuesRequests.Add(meterValuesRequest);
                };

                var evseId       = EVSE_Id.Parse(1);
                var meterValues  = new MeterValue[] {
                                       new MeterValue(
                                           new SampledValue[] {
                                               new SampledValue(
                                                   Value:              1.01M,
                                                   Context:            ReadingContexts.TransactionBegin,
                                                   Measurand:          Measurands.Current_Import,
                                                   Phase:              Phases.L1,
                                                   Location:           MeasurementLocations.Outlet,
                                                   SignedMeterValue:   new SignedMeterValue(
                                                                           SignedMeterData:   "1.01",
                                                                           SigningMethod:     "secp256r1_1.01",
                                                                           EncodingMethod:    "base64_1.01",
                                                                           PublicKey:         "pubkey_1.01",
                                                                           CustomData:        null
                                                                       ),
                                                   UnitOfMeasure:      UnitsOfMeasure.kW(
                                                                           Multiplier:   1,
                                                                           CustomData:   null
                                                                       ),
                                                   CustomData:         null
                                               ),
                                               new SampledValue(
                                                   Value:              1.02M,
                                                   Context:            ReadingContexts.TransactionBegin,
                                                   Measurand:          Measurands.Voltage,
                                                   Phase:              Phases.L2,
                                                   Location:           MeasurementLocations.Inlet,
                                                   SignedMeterValue:   new SignedMeterValue(
                                                                           SignedMeterData:   "1.02",
                                                                           SigningMethod:     "secp256r1_1.02",
                                                                           EncodingMethod:    "base64_1.02",
                                                                           PublicKey:         "pubkey_1.02",
                                                                           CustomData:        null
                                                                       ),
                                                   UnitOfMeasure:      UnitsOfMeasure.kW(
                                                                           Multiplier:   2,
                                                                           CustomData:   null
                                                                       ),
                                                   CustomData:         null
                                               )
                                           },
                                           Timestamp.Now - TimeSpan.FromMinutes(5)
                                       ),
                                       new MeterValue(
                                           new SampledValue[] {
                                               new SampledValue(
                                                   Value:              2.01M,
                                                   Context:            ReadingContexts.TransactionEnd,
                                                   Measurand:          Measurands.Current_Offered,
                                                   Phase:              Phases.L3,
                                                   Location:           MeasurementLocations.Cable,
                                                   SignedMeterValue:   new SignedMeterValue(
                                                                           SignedMeterData:   "2.01",
                                                                           SigningMethod:     "secp256r1_2.01",
                                                                           EncodingMethod:    "base64_2.01",
                                                                           PublicKey:         "pubkey_2.01",
                                                                           CustomData:        null
                                                                       ),
                                                   UnitOfMeasure:      UnitsOfMeasure.kW(
                                                                           Multiplier:   3,
                                                                           CustomData:   null
                                                                       ),
                                                   CustomData:         null
                                               ),
                                               new SampledValue(
                                                   Value:              2.02M,
                                                   Context:            ReadingContexts.TransactionEnd,
                                                   Measurand:          Measurands.Frequency,
                                                   Phase:              Phases.N,
                                                   Location:           MeasurementLocations.EV,
                                                   SignedMeterValue:   new SignedMeterValue(
                                                                           SignedMeterData:   "2.02",
                                                                           SigningMethod:     "secp256r1_2.02",
                                                                           EncodingMethod:    "base64_2.02",
                                                                           PublicKey:         "pubkey_2.02",
                                                                           CustomData:        null
                                                                       ),
                                                   UnitOfMeasure:      UnitsOfMeasure.kW(
                                                                           Multiplier:   4,
                                                                           CustomData:   null
                                                                       ),
                                                   CustomData:         null
                                               )
                                           },
                                           Timestamp.Now
                                       )
                                   };

                var response1    = await chargingStation1.SendMeterValues(
                                       EVSEId:        evseId,
                                       MeterValues:   meterValues,
                                       CustomData:    null
                                   );


                Assert.AreEqual (ResultCodes.OK,                                                  response1.Result.ResultCode);

                Assert.AreEqual (1,                                                               meterValuesRequests.Count);
                Assert.AreEqual (chargingStation1.ChargeBoxId,                                    meterValuesRequests.First().ChargeBoxId);
                Assert.AreEqual (evseId,                                                          meterValuesRequests.First().EVSEId);

                Assert.AreEqual (meterValues.Length,                                              meterValuesRequests.First().MeterValues.Count());
                Assert.IsTrue   (meterValues.ElementAt(0).Timestamp - meterValuesRequests.First().MeterValues.ElementAt(0).Timestamp < TimeSpan.FromSeconds(2));
                Assert.IsTrue   (meterValues.ElementAt(1).Timestamp - meterValuesRequests.First().MeterValues.ElementAt(1).Timestamp < TimeSpan.FromSeconds(2));

                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.Count(),                  meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.Count());
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.Count(),                  meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.Count());

                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Value,       meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Value);
                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Value,       meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Value);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Value,       meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Value);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Value,       meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Value);

                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Context,     meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Context);
                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Context,     meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Context);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Context,     meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Context);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Context,     meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Context);

                //Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Format,      meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Format);
                //Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Format,      meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Format);
                //Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Format,      meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Format);
                //Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Format,      meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Format);

                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Measurand,   meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Measurand);
                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Measurand,   meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Measurand);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Measurand,   meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Measurand);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Measurand,   meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Measurand);

                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Phase,       meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Phase);
                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Phase,       meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Phase);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Phase,       meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Phase);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Phase,       meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Phase);

                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Location,    meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Location);
                Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Location,    meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Location);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Location,    meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Location);
                Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Location,    meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Location);

                //Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(0).Unit,        meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(0).Unit);
                //Assert.AreEqual (meterValues.ElementAt(0).SampledValues.ElementAt(1).Unit,        meterValuesRequests.First().MeterValues.ElementAt(0).SampledValues.ElementAt(1).Unit);
                //Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(0).Unit,        meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(0).Unit);
                //Assert.AreEqual (meterValues.ElementAt(1).SampledValues.ElementAt(1).Unit,        meterValuesRequests.First().MeterValues.ElementAt(1).SampledValues.ElementAt(1).Unit);

            }

        }

        #endregion


        #region ChargingStation_TransferData_Test()

        /// <summary>
        /// A test for transfering data to the CSMS.
        /// </summary>
        [Test]
        public async Task ChargingStation_TransferData_Test()
        {

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var dataTransferRequests = new List<CS.DataTransferRequest>();

                testCSMS01.OnIncomingDataTransferRequest += async (timestamp, sender, dataTransferRequest) => {
                    dataTransferRequests.Add(dataTransferRequest);
                };

                var vendorId   = "GraphDefined OEM";
                var messageId  = RandomExtensions.RandomString(10);
                var data       = RandomExtensions.RandomString(40);

                var response1  = await chargingStation1.TransferData(
                                     VendorId:    vendorId,
                                     MessageId:   messageId,
                                     Data:        data,
                                     CustomData:  null
                                 );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
                Assert.AreEqual(data.Reverse(),                 response1.Data);

                Assert.AreEqual(1,                              dataTransferRequests.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   dataTransferRequests.First().ChargeBoxId);
                Assert.AreEqual(vendorId,                       dataTransferRequests.First().VendorId);
                Assert.AreEqual(messageId,                      dataTransferRequests.First().MessageId);
                Assert.AreEqual(data,                           dataTransferRequests.First().Data);

            }

        }

        #endregion

        #region ChargingStation_SendFirmwareStatusNotification_Test()

        /// <summary>
        /// A test for sending firmware status notifications to the CSMS.
        /// </summary>
        [Test]
        public async Task ChargingStation_SendFirmwareStatusNotification_Test()
        {

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var firmwareStatusNotifications = new List<CS.FirmwareStatusNotificationRequest>();

                testCSMS01.OnFirmwareStatusNotificationRequest += async (timestamp, sender, firmwareStatusNotification) => {
                    firmwareStatusNotifications.Add(firmwareStatusNotification);
                };

                var status     = FirmwareStatus.Installed;

                var response1  = await chargingStation1.SendFirmwareStatusNotification(
                                     Status:       status,
                                     CustomData:   null
                                 );


                Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);

                Assert.AreEqual(1,                              firmwareStatusNotifications.Count);
                Assert.AreEqual(chargingStation1.ChargeBoxId,   firmwareStatusNotifications.First().ChargeBoxId);
                Assert.AreEqual(status,                         firmwareStatusNotifications.First().Status);

            }

        }

        #endregion


    }

}
