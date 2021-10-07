/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.Threading.Tasks;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A central system for testing.
    /// </summary>
    public class TestCentralSystem
    {

        /// <summary>
        /// Create a new central system for testing.
        /// </summary>
        /// <param name="CentralSystemServer"></param>
        public TestCentralSystem(ICentralSystemServer CentralSystemServer)
        {

            if (CentralSystemServer == null)
                throw new ArgumentNullException(nameof(CentralSystemServer), "The given central system must not be null!");


            // Wire events...

            #region OnBootNotification

            CentralSystemServer.OnBootNotification += (Timestamp,
                                                       Sender,
                                                       CancellationToken,
                                                       EventTrackingId,
                                                       Request) =>

                Task.FromResult(new BootNotificationResponse(Request:            Request,
                                                             Status:             RegistrationStatus.Accepted,
                                                             CurrentTime:        org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                             HeartbeatInterval:  TimeSpan.FromMinutes(5)));

            #endregion

            #region OnHeartbeat

            CentralSystemServer.OnHeartbeat += (Timestamp,
                                                Sender,
                                                CancellationToken,
                                                EventTrackingId,
                                                Request) =>

                Task.FromResult(new HeartbeatResponse(Request:      Request,
                                                      CurrentTime:  org.GraphDefined.Vanaheimr.Illias.Timestamp.Now));

            #endregion

            #region OnStartTransaction

            CentralSystemServer.OnStartTransaction += (Timestamp,
                                                       Sender,
                                                       CancellationToken,
                                                       EventTrackingId,
                                                       Request) =>

                Task.FromResult(new StartTransactionResponse(Request:        Request,
                                                             TransactionId:  Transaction_Id.Random,
                                                             IdTagInfo:      new IdTagInfo(Status:      AuthorizationStatus.Accepted,
                                                                                           ExpiryDate:  org.GraphDefined.Vanaheimr.Illias.Timestamp.Now.AddDays(3))));

            #endregion

            #region OnStatusNotification

            CentralSystemServer.OnStatusNotification += (Timestamp,
                                                         Sender,
                                                         CancellationToken,
                                                         EventTrackingId,
                                                         Request) =>

                Task.FromResult(new StatusNotificationResponse(Request));

            #endregion

            #region OnMeterValues

            CentralSystemServer.OnMeterValues += (Timestamp,
                                                  Sender,
                                                  CancellationToken,
                                                  EventTrackingId,
                                                  Request) =>

                Task.FromResult(new MeterValuesResponse(Request));

            #endregion

            #region OnStopTransaction

            CentralSystemServer.OnStopTransaction += (Timestamp,
                                                      Sender,
                                                      CancellationToken,
                                                      EventTrackingId,
                                                      Request) =>

                Task.FromResult(new StopTransactionResponse(Request:    Request,
                                                            IdTagInfo:  new IdTagInfo(Status:      AuthorizationStatus.Accepted,
                                                                                      ExpiryDate:  org.GraphDefined.Vanaheimr.Illias.Timestamp.Now.AddDays(3))));

            #endregion


            #region OnIncomingDataTransfer

            CentralSystemServer.OnIncomingDataTransfer += (Timestamp,
                                                           Sender,
                                                           CancellationToken,
                                                           EventTrackingId,
                                                           Request) =>

                Task.FromResult(new DataTransferResponse(Request:  Request,
                                                         Status:   DataTransferStatus.Accepted,
                                                         Data:     "1234!"));

            #endregion

            #region OnDiagnosticsStatusNotification

            CentralSystemServer.OnDiagnosticsStatusNotification += (Timestamp,
                                                                    Sender,
                                                                    CancellationToken,
                                                                    EventTrackingId,
                                                                    Request) =>

                Task.FromResult(new DiagnosticsStatusNotificationResponse(Request));

            #endregion

            #region OnFirmwareStatusNotification

            CentralSystemServer.OnFirmwareStatusNotification += (Timestamp,
                                                                 Sender,
                                                                 CancellationToken,
                                                                 EventTrackingId,
                                                                 Request) =>

                Task.FromResult(new FirmwareStatusNotificationResponse(Request));

            #endregion

        }

    }

}
