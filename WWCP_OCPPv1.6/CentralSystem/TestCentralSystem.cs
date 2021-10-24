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
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A central system for testing.
    /// </summary>
    public class TestCentralSystem : IEventSender
    {

        #region Data

        private readonly HashSet<ICentralSystemServer> centralSystemServers;

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this central system.
        /// </summary>
        public CentralSystem_Id  CentralSystemId    { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => CentralSystemId.ToString();

        /// <summary>
        /// An enumeration of central system servers.
        /// </summary>
        public IEnumerable<ICentralSystemServer> CentralSystemServers
            => centralSystemServers;

        #endregion

        #region Events

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification request was received.
        /// </summary>
        event BootNotificationRequestDelegate   OnBootNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a boot notification was sent.
        /// </summary>
        event BootNotificationResponseDelegate  OnBootNotificationResponse;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event HeartbeatRequestDelegate   OnHeartbeatRequest;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        public event HeartbeatResponseDelegate  OnHeartbeatResponse;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new central system for testing.
        /// </summary>
        /// <param name="CentralSystemId">The unique identification of this central system.</param>
        public TestCentralSystem(CentralSystem_Id CentralSystemId)
        {

            if (CentralSystemId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(CentralSystemId), "The given central system identification must not be null or empty!");

            this.CentralSystemId       = CentralSystemId;
            this.centralSystemServers  = new HashSet<ICentralSystemServer>();

        }

        #endregion


        #region Attach(CentralSystemServer)

        public void Attach(ICentralSystemServer CentralSystemServer)
        {

            #region Initial checks

            if (CentralSystemServer is null)
                throw new ArgumentNullException(nameof(CentralSystemServer), "The given central system must not be null!");

            #endregion


            centralSystemServers.Add(CentralSystemServer);


            // Wire events...

            #region OnBootNotification

            CentralSystemServer.OnBootNotification += async (LogTimestamp,
                                                             Sender,
                                                             Request,
                                                             CancellationToken) => {

                #region Send OnBootNotificationRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnBootNotificationRequest?.Invoke(requestTimestamp,
                                                      this,
                                                      EventTracking_Id.New,
                                                      Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnBootNotificationRequest));
                }

                #endregion


                await Task.Delay(100);


                var response = new BootNotificationResponse(Request:            Request,
                                                            Status:             RegistrationStatus.Accepted,
                                                            CurrentTime:        Timestamp.Now,
                                                            HeartbeatInterval:  TimeSpan.FromMinutes(5));


                #region Send OnBootNotificationResponse event

                try
                {

                    OnBootNotificationResponse?.Invoke(Timestamp.Now,
                                                       this,
                                                       Request,
                                                       response,
                                                       Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnBootNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnHeartbeat

            CentralSystemServer.OnHeartbeat += async (LogTimestamp,
                                                      Sender,
                                                      Request,
                                                      CancellationToken) => {

                #region Send OnHeartbeatRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnHeartbeatRequest?.Invoke(requestTimestamp,
                                               this,
                                               Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnHeartbeatRequest));
                }

                #endregion


                await Task.Delay(100);


                var response = new HeartbeatResponse(Request:      Request,
                                                     CurrentTime:  Timestamp.Now);


                #region Send OnHeartbeatResponse event

                try
                {

                    OnHeartbeatResponse?.Invoke(Timestamp.Now,
                                                this,
                                                Request,
                                                response,
                                                Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnHeartbeatResponse));
                }

                #endregion

                return response;

            };

            #endregion


            #region OnStartTransaction

            CentralSystemServer.OnStartTransaction += (Timestamp,
                                                       Sender,
                                                       Request,
                                                       CancellationToken) =>

                Task.FromResult(new StartTransactionResponse(Request:        Request,
                                                             TransactionId:  Transaction_Id.Random,
                                                             IdTagInfo:      new IdTagInfo(Status:      AuthorizationStatus.Accepted,
                                                                                           ExpiryDate:  org.GraphDefined.Vanaheimr.Illias.Timestamp.Now.AddDays(3))));

            #endregion

            #region OnStatusNotification

            CentralSystemServer.OnStatusNotification += (Timestamp,
                                                         Sender,
                                                         Request,
                                                         CancellationToken) =>

                Task.FromResult(new StatusNotificationResponse(Request));

            #endregion

            #region OnMeterValues

            CentralSystemServer.OnMeterValues += (Timestamp,
                                                  Sender,
                                                  Request,
                                                  CancellationToken) =>

                Task.FromResult(new MeterValuesResponse(Request));

            #endregion

            #region OnStopTransaction

            CentralSystemServer.OnStopTransaction += (Timestamp,
                                                      Sender,
                                                      Request,
                                                      CancellationToken) =>

                Task.FromResult(new StopTransactionResponse(Request:    Request,
                                                            IdTagInfo:  new IdTagInfo(Status:      AuthorizationStatus.Accepted,
                                                                                      ExpiryDate:  org.GraphDefined.Vanaheimr.Illias.Timestamp.Now.AddDays(3))));

            #endregion


            #region OnIncomingDataTransfer

            CentralSystemServer.OnIncomingDataTransfer += (Timestamp,
                                                           Sender,
                                                           Request,
                                                           CancellationToken) =>

                Task.FromResult(new DataTransferResponse(Request:  Request,
                                                         Status:   DataTransferStatus.Accepted,
                                                         Data:     "1234!"));

            #endregion

            #region OnDiagnosticsStatusNotification

            CentralSystemServer.OnDiagnosticsStatusNotification += (Timestamp,
                                                                    Sender,
                                                                    Request,
                                                                    CancellationToken) =>

                Task.FromResult(new DiagnosticsStatusNotificationResponse(Request));

            #endregion

            #region OnFirmwareStatusNotification

            CentralSystemServer.OnFirmwareStatusNotification += (Timestamp,
                                                                 Sender,
                                                                 Request,
                                                                 CancellationToken) =>

                Task.FromResult(new FirmwareStatusNotificationResponse(Request));

            #endregion

        }

        #endregion

        #region CreateSOAPService(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/SOAP.
        /// </summary>
        /// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="ServiceName">The TCP service name shown e.g. on service startup.</param>
        /// <param name="URLPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="ContentType">An optional HTTP content type to use.</param>
        /// <param name="RegisterHTTPRootService">Register HTTP root services for sending a notice to clients connecting via HTML or plain text.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CentralSystemSOAPServer CreateSOAPService(String           HTTPServerName            = CentralSystemSOAPServer.DefaultHTTPServerName,
                                                         IPPort?          TCPPort                   = null,
                                                         String           ServiceName               = null,
                                                         HTTPPath?        URLPrefix                 = null,
                                                         HTTPContentType  ContentType               = null,
                                                         Boolean          RegisterHTTPRootService   = true,
                                                         DNSClient        DNSClient                 = null,
                                                         Boolean          AutoStart                 = false)
        {

            var centralSystemServer = new CentralSystemSOAPServer(HTTPServerName,
                                                                  TCPPort,
                                                                  ServiceName,
                                                                  URLPrefix,
                                                                  ContentType,
                                                                  RegisterHTTPRootService,
                                                                  DNSClient,
                                                                  AutoStart);

            Attach(centralSystemServer);

            return centralSystemServer;

        }

        #endregion

        #region CreateWebSocketService(...)

        /// <summary>
        /// Create a new central system for testing using HTTP/WebSocket.
        /// </summary>
        /// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        /// <param name="IPAddress">An IP address to listen on.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CentralSystemWSServer CreateWebSocketService(String      HTTPServerName   = CentralSystemWSServer.DefaultHTTPServerName,
                                                            IIPAddress  IPAddress        = null,
                                                            IPPort?     TCPPort          = null,
                                                            DNSClient   DNSClient        = null,
                                                            Boolean     AutoStart        = false)
        {

            var centralSystemServer = new CentralSystemWSServer(HTTPServerName,
                                                                IPAddress,
                                                                TCPPort,
                                                                DNSClient,
                                                                AutoStart);

            Attach(centralSystemServer);

            return centralSystemServer;

        }

        #endregion


        #region Reset(ChargeBoxId, ResetType, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ResetType">The type of reset that the charge point should perform.</param>
        public async Task<CP.ResetResponse> Reset(ChargeBox_Id      ChargeBoxId,
                                                  ResetTypes        ResetType,
                                                  EventTracking_Id  EventTrackingId    = null)
        {

            CentralSystemSOAPClient CPClient = null;

            var response = await CPClient.Reset(new ResetRequest(ChargeBoxId,
                                                                 ResetType,
                                                                 Request_Id.Random(),
                                                                 null,
                                                                 EventTrackingId));

            return response.Content;

        }

        #endregion


    }

}
