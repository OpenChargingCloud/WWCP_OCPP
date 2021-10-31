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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using social.OpenData.UsersAPI;

using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A central system for testing.
    /// </summary>
    public class TestCentralSystem : IEventSender
    {

        #region Data

        private readonly HashSet<ICentralSystemServer> centralSystemServers;

        private readonly Dictionary<ChargeBox_Id, Tuple<ICentralSystem, DateTime>> reachableChargingBoxes;

        private readonly UsersAPI    TestAPI;

        private readonly OCPPWebAPI  WebAPI;

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


        /// <summary>
        /// Require a HTTP Basic Authentication of all charging boxes.
        /// </summary>
        public Boolean RequireAuthentication { get; }


        /// <summary>
        /// The unique identifications of all connected or reachable charge boxes.
        /// </summary>
        public IEnumerable<ChargeBox_Id> ChargeBoxIds
            => reachableChargingBoxes.Values.SelectMany(box => box.Item1.ChargeBoxIds);


        public DNSClient DNSClient { get; }

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


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event OnAuthorizeRequestDelegate   OnAuthorizeRequest;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        public event OnAuthorizeResponseDelegate  OnAuthorizeResponse;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event OnStartTransactionRequestDelegate   OnStartTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        public event OnStartTransactionResponseDelegate  OnStartTransactionResponse;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event OnStatusNotificationRequestDelegate   OnStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        public event OnStatusNotificationResponseDelegate  OnStatusNotificationResponse;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event OnMeterValuesRequestDelegate   OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        public event OnMeterValuesResponseDelegate  OnMeterValuesResponse;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event OnStopTransactionRequestDelegate   OnStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        public event OnStopTransactionResponseDelegate  OnStopTransactionResponse;

        #endregion


        #region OnIncomingDataTransfer

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate   OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate  OnIncomingDataTransferResponse;

        #endregion

        #region OnDiagnosticsStatusNotification

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationRequestDelegate   OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseDelegate  OnDiagnosticsStatusNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a heartbeat request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event sent whenever a response to a heartbeat was sent.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate  OnFirmwareStatusNotificationResponse;

        #endregion



        // WebSocket events
        public event OnNewTCPConnectionDelegate                 OnNewTCPConnection;

        public event OnNewWebSocketConnectionDelegate           OnNewWebSocketConnection;

        public event OnWebSocketMessageDelegate                 OnMessage;


        public event OnWebSocketTextMessageRequestDelegate      OnTextMessageRequest;

        public event OnWebSocketTextMessageDelegate             OnTextMessage;

        public event OnWebSocketTextMessageResponseDelegate     OnTextMessageResponse;


        public event OnWebSocketBinaryMessageRequestDelegate    OnBinaryMessageRequest;

        public event OnWebSocketBinaryMessageDelegate           OnBinaryMessage;

        public event OnWebSocketBinaryMessageResponseDelegate   OnBinaryMessageResponse;


        public event OnWebSocketMessageDelegate                 OnPingMessage;

        public event OnWebSocketMessageDelegate                 OnPongMessage;


        public event OnCloseMessageDelegate                     OnCloseMessage;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new central system for testing.
        /// </summary>
        /// <param name="CentralSystemId">The unique identification of this central system.</param>
        /// <param name="RequireAuthentication">Require a HTTP Basic Authentication of all charging boxes.</param>
        public TestCentralSystem(CentralSystem_Id  CentralSystemId,
                                 Boolean           RequireAuthentication   = true,
                                 DNSClient         DNSClient               = null)
        {

            if (CentralSystemId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(CentralSystemId), "The given central system identification must not be null or empty!");

            this.CentralSystemId         = CentralSystemId;
            this.RequireAuthentication   = RequireAuthentication;
            this.centralSystemServers    = new HashSet<ICentralSystemServer>();
            this.reachableChargingBoxes  = new Dictionary<ChargeBox_Id, Tuple<ICentralSystem, DateTime>>();

            Directory.CreateDirectory("HTTPSSEs");

            this.TestAPI                 = new UsersAPI(HTTPServerPort:        IPPort.Parse(3500),
                                                        HTTPServerName:        "GraphDefined OCPP Test Central System",
                                                        HTTPServiceName:       "GraphDefined OCPP Test Central System Service",
                                                        APIRobotEMailAddress:  EMailAddress.Parse("GraphDefined OCPP Test Central System Robot <robot@charging.cloud>"),
                                                        SMTPClient:            new NullMailer(),
                                                        DNSClient:             DNSClient,
                                                        Autostart:             true);

            this.WebAPI                  = new OCPPWebAPI(this,
                                                          TestAPI.HTTPServer);

            this.DNSClient               = DNSClient;

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
                                                                  DNSClient ?? this.DNSClient,
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
                                                                RequireAuthentication,
                                                                DNSClient ?? this.DNSClient,
                                                                AutoStart);

            Attach(centralSystemServer);


            #region OnNewTCPConnection

            centralSystemServer.OnNewTCPConnection += async (Timestamp,
                                                             WebSocketServer,
                                                             NewWebSocketConnection,
                                                             EventTrackingId,
                                                             CancellationToken) => {

                OnNewTCPConnection?.Invoke(Timestamp,
                                           WebSocketServer,
                                           NewWebSocketConnection,
                                           EventTrackingId,
                                           CancellationToken);

            };

            #endregion

            #region OnNewWebSocketConnection

            centralSystemServer.OnNewWebSocketConnection += async (Timestamp,
                                                                   WebSocketServer,
                                                                   NewWebSocketConnection,
                                                                   EventTrackingId,
                                                                   CancellationToken) => {

                OnNewWebSocketConnection?.Invoke(Timestamp,
                                                 WebSocketServer,
                                                 NewWebSocketConnection,
                                                 EventTrackingId,
                                                 CancellationToken);

            };

            #endregion


            //OnMessage;


            #region OnTextMessageRequest

            centralSystemServer.OnTextMessageRequest += async (Timestamp,
                                                               WebSocketServer,
                                                               Sender,
                                                               TextRequestMessage,
                                                               EventTrackingId,
                                                               CancellationToken) => {

                OnTextMessageRequest?.Invoke(Timestamp,
                                             WebSocketServer,
                                             Sender,
                                             TextRequestMessage,
                                             EventTrackingId,
                                             CancellationToken);

            };

            #endregion

            #region OnTextMessageResponse

            centralSystemServer.OnTextMessageResponse += async (Timestamp,
                                                                WebSocketServer,
                                                                Sender,
                                                                TextRequestMessage,
                                                                TextResponseMessage,
                                                                EventTrackingId,
                                                                CancellationToken) => {

                OnTextMessageResponse?.Invoke(Timestamp,
                                              WebSocketServer,
                                              Sender,
                                              TextRequestMessage,
                                              TextResponseMessage,
                                              EventTrackingId,
                                              CancellationToken);

            };

            #endregion


            #region OnBinaryMessageRequest

            centralSystemServer.OnBinaryMessageRequest += async (Timestamp,
                                                                 WebSocketServer,
                                                                 Sender,
                                                                 BinaryRequestMessage,
                                                                 EventTrackingId,
                                                                 CancellationToken) => {

                OnBinaryMessageRequest?.Invoke(Timestamp,
                                               WebSocketServer,
                                               Sender,
                                               BinaryRequestMessage,
                                               EventTrackingId,
                                               CancellationToken);

            };

            #endregion

            #region OnBinaryMessageResponse

            centralSystemServer.OnBinaryMessageResponse += async (Timestamp,
                                                                  WebSocketServer,
                                                                  Sender,
                                                                  BinaryRequestMessage,
                                                                  BinaryResponseMessage,
                                                                  EventTrackingId,
                                                                  CancellationToken) => {

                OnBinaryMessageResponse?.Invoke(Timestamp,
                                                WebSocketServer,
                                                Sender,
                                                BinaryRequestMessage,
                                                BinaryResponseMessage,
                                                EventTrackingId,
                                                CancellationToken);

            };

            #endregion


            //OnPingMessage;

            //OnPongMessage;


            //OnCloseMessage;


            return centralSystemServer;

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


            if (CentralSystemServer is CentralSystemWSServer centralSystemWSServer)
            {
                centralSystemWSServer.OnNewCentralSystemWSConnection += async (LogTimestamp,
                                                                               CentralSystem,
                                                                               Connection,
                                                                               EventTrackingId,
                                                                               CancellationToken) =>
                {

                    if (Connection.TryGetCustomData("chargeBoxId", out ChargeBox_Id chargeBoxId))
                    {
                        //ToDo: lock(...)
                        if (!reachableChargingBoxes.ContainsKey(chargeBoxId))
                            reachableChargingBoxes.Add(chargeBoxId, new Tuple<ICentralSystem, DateTime>(CentralSystem, Timestamp.Now));
                        else
                            reachableChargingBoxes[chargeBoxId]   = new Tuple<ICentralSystem, DateTime>(CentralSystem, Timestamp.Now);
                    }

                };
            }


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
                                                      Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnBootNotificationRequest));
                }

                                                                 #endregion


                Console.WriteLine("OnBootNotification: " + Request.ChargeBoxId             + ", " +
                                                           Request.ChargePointVendor       + ", " +
                                                           Request.ChargePointModel        + ", " +
                                                           Request.ChargePointSerialNumber + ", " +
                                                           Request.ChargeBoxSerialNumber);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {

                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));

                    //if (Sender is CentralSystemSOAPServer centralSystemSOAPServer)

                }
                else
                {

                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);

                    //if (Sender is CentralSystemSOAPServer centralSystemSOAPServer)

                }


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


                Console.WriteLine("OnHeartbeat: " + Request.ChargeBoxId);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {

                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));

                    //if (Sender is CentralSystemSOAPServer centralSystemSOAPServer)

                }
                else
                {

                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);

                    //if (Sender is CentralSystemSOAPServer centralSystemSOAPServer)

                }


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


            #region OnAuthorize

            CentralSystemServer.OnAuthorize += async (LogTimestamp,
                                                      Sender,
                                                      Request,
                                                      CancellationToken) => {

                #region Send OnAuthorizeRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnAuthorizeRequest?.Invoke(requestTimestamp,
                                               this,
                                               Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnAuthorizeRequest));
                }

                #endregion


                Console.WriteLine("OnAuthorize: " + Request.ChargeBoxId + ", " +
                                                    Request.IdTag);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {

                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));

                    //if (Sender is CentralSystemSOAPServer centralSystemSOAPServer)

                }
                else
                {

                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);

                    //if (Sender is CentralSystemSOAPServer centralSystemSOAPServer)

                }

                await Task.Delay(100);

                var response = new AuthorizeResponse(Request:    Request,
                                                     IdTagInfo:  new IdTagInfo(Status:      AuthorizationStatus.Accepted,
                                                                               ExpiryDate:  Timestamp.Now.AddDays(3)));


                #region Send OnAuthorizeResponse event

                try
                {

                    OnAuthorizeResponse?.Invoke(Timestamp.Now,
                                                this,
                                                Request,
                                                response,
                                                Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnAuthorizeResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnStartTransaction

            CentralSystemServer.OnStartTransaction += async (LogTimestamp,
                                                             Sender,
                                                             Request,
                                                             CancellationToken) => {

                #region Send OnStartTransactionRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnStartTransactionRequest?.Invoke(requestTimestamp,
                                                      this,
                                                      Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStartTransactionRequest));
                }

                #endregion


                Console.WriteLine("OnStartTransaction: " + Request.ChargeBoxId + ", " +
                                                           Request.ConnectorId + ", " +
                                                           Request.IdTag + ", " +
                                                           Request.StartTimestamp + ", " +
                                                           Request.MeterStart + ", " +
                                                           Request.ReservationId ?? "-");

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {

                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));

                    //if (Sender is CentralSystemSOAPServer centralSystemSOAPServer)

                }
                else
                {

                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);

                    //if (Sender is CentralSystemSOAPServer centralSystemSOAPServer)

                }

                await Task.Delay(100);

                var response = new StartTransactionResponse(Request:        Request,
                                                            TransactionId:  Transaction_Id.Random,
                                                            IdTagInfo:      new IdTagInfo(Status:      AuthorizationStatus.Accepted,
                                                                                          ExpiryDate:  Timestamp.Now.AddDays(3)));


                #region Send OnStartTransactionResponse event

                try
                {

                    OnStartTransactionResponse?.Invoke(Timestamp.Now,
                                                       this,
                                                       Request,
                                                       response,
                                                       Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStartTransactionResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnStatusNotification

            CentralSystemServer.OnStatusNotification += async (LogTimestamp,
                                                               Sender,
                                                               Request,
                                                               CancellationToken) => {

                #region Send OnStatusNotificationRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnStatusNotificationRequest?.Invoke(requestTimestamp,
                                                      this,
                                                      Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStatusNotificationRequest));
                }

                #endregion


                Console.WriteLine("OnStatusNotification: " + Request.ConnectorId     + ", " +
                                                             Request.Status          + ", " +
                                                             Request.ErrorCode       + ", " +
                                                             Request.Info            + ", " +
                                                             Request.StatusTimestamp + ", " +
                                                             Request.VendorId        + ", " +
                                                             Request.VendorErrorCode);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);
                }

                await Task.Delay(100);

                var response = new StatusNotificationResponse(Request);


                #region Send OnStatusNotificationResponse event

                try
                {

                    OnStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                         this,
                                                         Request,
                                                         response,
                                                         Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStatusNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnMeterValues

            CentralSystemServer.OnMeterValues += async (LogTimestamp,
                                                        Sender,
                                                        Request,
                                                        CancellationToken) => {

                #region Send OnMeterValuesRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnMeterValuesRequest?.Invoke(requestTimestamp,
                                                      this,
                                                      Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnMeterValuesRequest));
                }

                                                            #endregion


                Console.WriteLine("OnMeterValues: " + Request.ConnectorId + ", " +
                                                      Request.TransactionId);

                Console.WriteLine(Request.MeterValues.SafeSelect(meterValue => meterValue.Timestamp.ToIso8601() +
                                          meterValue.SampledValues.SafeSelect(sampledValue => sampledValue.Context + ", " + sampledValue.Value + ", " + sampledValue.Value).AggregateWith("; ")).AggregateWith(Environment.NewLine));

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);
                }

                await Task.Delay(100);

                var response = new MeterValuesResponse(Request);


                #region Send OnMeterValuesResponse event

                try
                {

                    OnMeterValuesResponse?.Invoke(Timestamp.Now,
                                                       this,
                                                       Request,
                                                       response,
                                                       Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnMeterValuesResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnStopTransaction

            CentralSystemServer.OnStopTransaction += async (LogTimestamp,
                                                            Sender,
                                                            Request,
                                                            CancellationToken) => {

                #region Send OnStopTransactionRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnStopTransactionRequest?.Invoke(requestTimestamp,
                                                     this,
                                                     Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStopTransactionRequest));
                }

                #endregion


                Console.WriteLine("OnStopTransaction: " + Request.TransactionId + ", " +
                                                          Request.IdTag + ", " +
                                                          Request.Timestamp.ToIso8601() + ", " +
                                                          Request.MeterStop + ", " +
                                                          Request.Reason);

                Console.WriteLine(Request.TransactionData.SafeSelect(transactionData => transactionData.Timestamp.ToIso8601() +
                                          transactionData.SampledValues.SafeSelect(sampledValue => sampledValue.Context + ", " + sampledValue.Value + ", " + sampledValue.Value).AggregateWith("; ")).AggregateWith(Environment.NewLine));

                await Task.Delay(100);

                var response = new StopTransactionResponse(Request:    Request,
                                                           IdTagInfo:  new IdTagInfo(Status:      AuthorizationStatus.Accepted,
                                                                                     ExpiryDate:  Timestamp.Now.AddDays(3)));


                #region Send OnStopTransactionResponse event

                try
                {

                    OnStopTransactionResponse?.Invoke(Timestamp.Now,
                                                      this,
                                                      Request,
                                                      response,
                                                      Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnStopTransactionResponse));
                }

                #endregion

                return response;

            };

            #endregion


            #region OnIncomingDataTransfer

            CentralSystemServer.OnIncomingDataTransfer += async (LogTimestamp,
                                                                 Sender,
                                                                 Request,
                                                                 CancellationToken) => {

                #region Send OnIncomingDataRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnIncomingDataTransferRequest?.Invoke(requestTimestamp,
                                                          this,
                                                          Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnIncomingDataTransferRequest));
                }

                #endregion


                Console.WriteLine("OnIncomingDataTransfer: " + Request.VendorId  + ", " +
                                                               Request.MessageId + ", " +
                                                               Request.Data);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);
                }

                await Task.Delay(100);

                var response = new DataTransferResponse(Request:  Request,
                                                        Status:   DataTransferStatus.Accepted,
                                                        Data:     Request.Data.Reverse());


                #region Send OnIncomingDataResponse event

                try
                {

                    OnIncomingDataTransferResponse?.Invoke(Timestamp.Now,
                                                           this,
                                                           Request,
                                                           response,
                                                           Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnIncomingDataTransferResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnDiagnosticsStatusNotification

            CentralSystemServer.OnDiagnosticsStatusNotification += async (LogTimestamp,
                                                                          Sender,
                                                                          Request,
                                                                          CancellationToken) => {

                #region Send OnDiagnosticsStatusNotificationRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnDiagnosticsStatusNotificationRequest?.Invoke(requestTimestamp,
                                                                   this,
                                                                   Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
                }

                #endregion


                Console.WriteLine("OnDiagnosticsStatusNotification: " + Request.Status);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);
                }

                await Task.Delay(100);

                var response = new DiagnosticsStatusNotificationResponse(Request);


                #region Send OnDiagnosticsStatusResponse event

                try
                {

                    OnDiagnosticsStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                    this,
                                                                    Request,
                                                                    response,
                                                                    Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion

            #region OnFirmwareStatusNotification

            CentralSystemServer.OnFirmwareStatusNotification += async (LogTimestamp,
                                                                       Sender,
                                                                       Request,
                                                                       CancellationToken) => {

                #region Send OnFirmwareStatusNotificationRequest event

                var requestTimestamp = Timestamp.Now;

                try
                {

                    OnFirmwareStatusNotificationRequest?.Invoke(requestTimestamp,
                                                                this,
                                                                Request);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnFirmwareStatusNotificationRequest));
                }

                #endregion


                Console.WriteLine("OnFirmwareStatus: " + Request.Status);

                if (!reachableChargingBoxes.ContainsKey(Request.ChargeBoxId))
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes.Add(Request.ChargeBoxId, new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now));
                }
                else
                {
                    if (Sender is CentralSystemWSServer centralSystemWSServer)
                        reachableChargingBoxes[Request.ChargeBoxId] = new Tuple<ICentralSystem, DateTime>(centralSystemWSServer, Timestamp.Now);
                }

                await Task.Delay(100);

                var response = new FirmwareStatusNotificationResponse(Request);


                #region Send OnFirmwareStatusResponse event

                try
                {

                    OnFirmwareStatusNotificationResponse?.Invoke(Timestamp.Now,
                                                                 this,
                                                                 Request,
                                                                 response,
                                                                 Timestamp.Now - requestTimestamp);

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(TestCentralSystem) + "." + nameof(OnFirmwareStatusNotificationResponse));
                }

                #endregion

                return response;

            };

            #endregion

        }

        #endregion


        public void AddBasicAuth(String Login, String Password)
        {

            foreach (var centralSystemServer in centralSystemServers)
            {
                if (centralSystemServer is CentralSystemWSServer centralSystemWSServer)
                {
                    centralSystemWSServer.ChargingBoxLogins.Add(Login, Password);
                }
            }

        }



        #region Reset                 (ChargeBoxId, ResetType, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ResetType">The type of reset that the charge point should perform.</param>
        public async Task<CP.ResetResponse> Reset(ChargeBox_Id      ChargeBoxId,
                                                  ResetTypes        ResetType,
                                                  EventTracking_Id  EventTrackingId   = null)
        {

            var request = new ResetRequest(ChargeBoxId,
                                           ResetType,
                                           Request_Id.Random(),
                                           null,
                                           EventTrackingId);

            CP.ResetResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.Reset(request);

            else
                response = new CP.ResetResponse(request, ResetStatus.Rejected);

            return response;

        }

        #endregion

        #region ChangeAvailability    (ChargeBoxId, ConnectorId, Availability, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The identification of the connector for which its availability should be changed. Id '0' (zero) is used if the availability of the entire charge point and all its connectors should be changed.</param>
        /// <param name="Availability">The new availability of the charge point or charge point connector.</param>
        public async Task<CP.ChangeAvailabilityResponse> ChangeAvailability(ChargeBox_Id      ChargeBoxId,
                                                                            Connector_Id      ConnectorId,
                                                                            Availabilities    Availability,
                                                                            EventTracking_Id  EventTrackingId   = null)
        {

            var request = new ChangeAvailabilityRequest(ChargeBoxId,
                                                        ConnectorId,
                                                        Availability,
                                                        Request_Id.Random(),
                                                        null,
                                                        EventTrackingId);

            CP.ChangeAvailabilityResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.ChangeAvailability(request);

            else
                response = new CP.ChangeAvailabilityResponse(request, AvailabilityStatus.Rejected);

            return response;

        }

        #endregion

        #region GetConfiguration      (ChargeBoxId, Keys = null, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Keys">An optional enumeration of keys for which the configuration is requested. Return all keys if empty.</param>
        public async Task<CP.GetConfigurationResponse> GetConfiguration(ChargeBox_Id         ChargeBoxId,
                                                                        IEnumerable<String>  Keys              = null,
                                                                        EventTracking_Id     EventTrackingId   = null)
        {

            var request = new GetConfigurationRequest(ChargeBoxId,
                                                      Keys,
                                                      Request_Id.Random(),
                                                      null,
                                                      EventTrackingId);

            CP.GetConfigurationResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.GetConfiguration(request);

            else
                response = new CP.GetConfigurationResponse(request,
                                                           new ConfigurationKey[0],
                                                           Keys);

            return response;

        }

        #endregion

        #region ChangeConfiguration   (ChargeBoxId, Key, Value, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Key">The name of the configuration setting to change.</param>
        /// <param name="Value">The new value as string for the setting.</param>
        public async Task<CP.ChangeConfigurationResponse> ChangeConfiguration(ChargeBox_Id      ChargeBoxId,
                                                                              String            Key,
                                                                              String            Value,
                                                                              EventTracking_Id  EventTrackingId   = null)
        {

            var request = new ChangeConfigurationRequest(ChargeBoxId,
                                                         Key,
                                                         Value,
                                                         Request_Id.Random(),
                                                         null,
                                                         EventTrackingId);

            CP.ChangeConfigurationResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.ChangeConfiguration(request);

            else
                response = new CP.ChangeConfigurationResponse(request, ConfigurationStatus.Rejected);

            return response;

        }

        #endregion

        #region DataTransfer          (ChargeBoxId, VendorId, MessageId = null, Data = null, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification field.</param>
        /// <param name="Data">Optional message data as text without specified length or format.</param>
        public async Task<CP.DataTransferResponse> DataTransfer(ChargeBox_Id      ChargeBoxId,
                                                                String            VendorId,
                                                                String            MessageId         = null,
                                                                String            Data              = null,
                                                                EventTracking_Id  EventTrackingId   = null)
        {

            var request = new DataTransferRequest(ChargeBoxId,
                                                  VendorId,
                                                  MessageId,
                                                  Data,
                                                  Request_Id.Random(),
                                                  null,
                                                  EventTrackingId);

            CP.DataTransferResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.DataTransfer(request);

            else
                response = new CP.DataTransferResponse(request, DataTransferStatus.Rejected);

            return response;

        }

        #endregion

        #region GetDiagnostics        (ChargeBoxId, Location, StartTime = null, StopTime = null, Retries = null, RetryInterval = null,...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Location">The URI where the diagnostics file shall be uploaded to.</param>
        /// <param name="StartTime">The timestamp of the oldest logging information to include in the diagnostics.</param>
        /// <param name="StopTime">The timestamp of the latest logging information to include in the diagnostics.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to upload the diagnostics before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
        public async Task<CP.GetDiagnosticsResponse> GetDiagnostics(ChargeBox_Id      ChargeBoxId,
                                                                    String            Location,
                                                                    DateTime?         StartTime         = null,
                                                                    DateTime?         StopTime          = null,
                                                                    Byte?             Retries           = null,
                                                                    TimeSpan?         RetryInterval     = null,
                                                                    EventTracking_Id  EventTrackingId   = null)
        {

            var request = new GetDiagnosticsRequest(ChargeBoxId,
                                                    Location,
                                                    StartTime,
                                                    StopTime,
                                                    Retries,
                                                    RetryInterval,
                                                    Request_Id.Random(),
                                                    null,
                                                    EventTrackingId);

            CP.GetDiagnosticsResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.GetDiagnostics(request);

            else
                response = new CP.GetDiagnosticsResponse(request);

            return response;

        }

        #endregion

        #region TriggerMessage        (ChargeBoxId, RequestedMessage, ConnectorId = null,...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RequestedMessage">The message to trigger.</param>
        /// <param name="ConnectorId">Optional connector identification whenever the message applies to a specific connector.</param>
        public async Task<CP.TriggerMessageResponse> TriggerMessage(ChargeBox_Id      ChargeBoxId,
                                                                    MessageTriggers   RequestedMessage,
                                                                    Connector_Id?     ConnectorId       = null,
                                                                    EventTracking_Id  EventTrackingId   = null)
        {

            var request = new TriggerMessageRequest(ChargeBoxId,
                                                    RequestedMessage,
                                                    ConnectorId,
                                                    Request_Id.Random(),
                                                    null,
                                                    EventTrackingId);

            CP.TriggerMessageResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.TriggerMessage(request);

            else
                response = new CP.TriggerMessageResponse(request, TriggerMessageStatus.Rejected);

            return response;

        }

        #endregion

        #region UpdateFirmware        (ChargeBoxId, Location, RetrieveDate, Retries = null, RetryInterval = null, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Location">The URI where to download the firmware.</param>
        /// <param name="RetrieveDate">The timestamp after which the charge point must retrieve the firmware.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to download the firmware before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
        public async Task<CP.UpdateFirmwareResponse> UpdateFirmware(ChargeBox_Id      ChargeBoxId,
                                                                    String            Location,
                                                                    DateTime          RetrieveDate,
                                                                    Byte?             Retries           = null,
                                                                    TimeSpan?         RetryInterval     = null,
                                                                    EventTracking_Id  EventTrackingId   = null)
        {

            var request = new UpdateFirmwareRequest(ChargeBoxId,
                                                    Location,
                                                    RetrieveDate,
                                                    Retries,
                                                    RetryInterval,
                                                    Request_Id.Random(),
                                                    null,
                                                    EventTrackingId);

            CP.UpdateFirmwareResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.UpdateFirmware(request);

            else
                response = new CP.UpdateFirmwareResponse(request);

            return response;

        }

        #endregion


        #region ReserveNow            (ChargeBoxId, ConnectorId, ReservationId, ExpiryDate, IdTag, ParentIdTag = null, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The identification of the connector to be reserved. A value of 0 means that the reservation is not for a specific connector.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// <param name="ExpiryDate">The timestamp when the reservation ends.</param>
        /// <param name="IdTag">The identifier for which the charge point has to reserve a connector.</param>
        /// <param name="ParentIdTag">An optional ParentIdTag.</param>
        public async Task<CP.ReserveNowResponse> ReserveNow(ChargeBox_Id      ChargeBoxId,
                                                            Connector_Id      ConnectorId,
                                                            Reservation_Id    ReservationId,
                                                            DateTime          ExpiryDate,
                                                            IdToken           IdTag,
                                                            IdToken?          ParentIdTag       = null,
                                                            EventTracking_Id  EventTrackingId   = null)
        {

            var request = new ReserveNowRequest(ChargeBoxId,
                                                ConnectorId,
                                                ReservationId,
                                                ExpiryDate,
                                                IdTag,
                                                ParentIdTag,
                                                Request_Id.Random(),
                                                null,
                                                EventTrackingId);

            CP.ReserveNowResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.ReserveNow(request);

            else
                response = new CP.ReserveNowResponse(request, ReservationStatus.Rejected);

            return response;

        }

        #endregion

        #region CancelReservation     (ChargeBoxId, ReservationId, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        public async Task<CP.CancelReservationResponse> CancelReservation(ChargeBox_Id      ChargeBoxId,
                                                                          Reservation_Id    ReservationId,
                                                                          EventTracking_Id  EventTrackingId   = null)
        {

            var request = new CancelReservationRequest(ChargeBoxId,
                                                       ReservationId,
                                                       Request_Id.Random(),
                                                       null,
                                                       EventTrackingId);

            CP.CancelReservationResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.CancelReservation(request);

            else
                response = new CP.CancelReservationResponse(request, CancelReservationStatus.Rejected);

            return response;

        }

        #endregion

        #region RemoteStartTransaction(ChargeBoxId, IdTag, ConnectorId = null, ChargingProfile = null, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="IdTag">The identification tag to start the charging transaction.</param>
        /// <param name="ConnectorId">An optional connector identification on which the charging transaction should be started (SHALL be > 0).</param>
        /// <param name="ChargingProfile">An optional charging profile to be used by the charge point for the requested charging transaction.</param>
        public async Task<CP.RemoteStartTransactionResponse> RemoteStartTransaction(ChargeBox_Id      ChargeBoxId,
                                                                                    IdToken           IdTag,
                                                                                    Connector_Id?     ConnectorId       = null,
                                                                                    ChargingProfile   ChargingProfile   = null,
                                                                                    EventTracking_Id  EventTrackingId   = null)
        {

            var request = new RemoteStartTransactionRequest(ChargeBoxId,
                                                            IdTag,
                                                            ConnectorId,
                                                            ChargingProfile,
                                                            Request_Id.Random(),
                                                            null,
                                                            EventTrackingId);

            CP.RemoteStartTransactionResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.RemoteStartTransaction(request);

            else
                response = new CP.RemoteStartTransactionResponse(request, RemoteStartStopStatus.Rejected);

            return response;

        }

        #endregion

        #region RemoteStopTransaction (ChargeBoxId, TransactionId, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="TransactionId">The identification of the transaction which the charge point is requested to stop.</param>
        public async Task<CP.RemoteStopTransactionResponse> RemoteStopTransaction(ChargeBox_Id      ChargeBoxId,
                                                                                  Transaction_Id    TransactionId,
                                                                                  EventTracking_Id  EventTrackingId   = null)
        {

            var request = new RemoteStopTransactionRequest(ChargeBoxId,
                                                           TransactionId,
                                                           Request_Id.Random(),
                                                           null,
                                                           EventTrackingId);

            CP.RemoteStopTransactionResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.RemoteStopTransaction(request);

            else
                response = new CP.RemoteStopTransactionResponse(request, RemoteStartStopStatus.Rejected);

            return response;

        }

        #endregion

        #region SetChargingProfile    (ChargeBoxId, ConnectorId, ChargingProfile, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The connector to which the charging profile applies. If connectorId = 0, the message contains an overall limit for the charge point.</param>
        /// <param name="ChargingProfile">The charging profile to be set.</param>
        public async Task<CP.SetChargingProfileResponse> SetChargingProfile(ChargeBox_Id      ChargeBoxId,
                                                                            Connector_Id      ConnectorId,
                                                                            ChargingProfile   ChargingProfile,
                                                                            EventTracking_Id  EventTrackingId   = null)
        {

            var request = new SetChargingProfileRequest(ChargeBoxId,
                                                        ConnectorId,
                                                        ChargingProfile,
                                                        Request_Id.Random(),
                                                        null,
                                                        EventTrackingId);

            CP.SetChargingProfileResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.SetChargingProfile(request);

            else
                response = new CP.SetChargingProfileResponse(request, ChargingProfileStatus.NotSupported);

            return response;

        }

        #endregion

        #region ClearChargingProfile  (ChargeBoxId, ChargingProfileId, ConnectorId, ChargingProfilePurpose, StackLevel, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChargingProfileId">The optional identification of the charging profile to clear.</param>
        /// <param name="ConnectorId">The optional connector for which to clear the charging profiles. Connector identification 0 specifies the charging profile for the overall charge point. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.</param>
        /// <param name="ChargingProfilePurpose">The optional purpose of the charging profiles that will be cleared, if they meet the other criteria in the request.</param>
        /// <param name="StackLevel">The optional stack level for which charging profiles will be cleared, if they meet the other criteria in the request.</param>
        public async Task<CP.ClearChargingProfileResponse> ClearChargingProfile(ChargeBox_Id              ChargeBoxId,
                                                                                ChargingProfile_Id?       ChargingProfileId        = null,
                                                                                Connector_Id?             ConnectorId              = null,
                                                                                ChargingProfilePurposes?  ChargingProfilePurpose   = null,
                                                                                UInt32?                   StackLevel               = null,
                                                                                EventTracking_Id          EventTrackingId          = null)
        {

            var request = new ClearChargingProfileRequest(ChargeBoxId,
                                                          ChargingProfileId,
                                                          ConnectorId,
                                                          ChargingProfilePurpose,
                                                          StackLevel,
                                                          Request_Id.Random(),
                                                          null,
                                                          EventTrackingId);

            CP.ClearChargingProfileResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.ClearChargingProfile(request);

            else
                response = new CP.ClearChargingProfileResponse(request, ClearChargingProfileStatus.Unknown);

            return response;

        }

        #endregion

        #region GetCompositeSchedule  (ChargeBoxId, ConnectorId, Duration, ChargingRateUnit = null, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The connector identification for which the schedule is requested. Connector identification 0 will calculate the expected consumption for the grid connection.</param>
        /// <param name="Duration">The length of requested schedule.</param>
        /// <param name="ChargingRateUnit">Can optionally be used to force a power or current profile.</param>
        public async Task<CP.GetCompositeScheduleResponse> GetCompositeSchedule(ChargeBox_Id        ChargeBoxId,
                                                                                Connector_Id        ConnectorId,
                                                                                TimeSpan            Duration,
                                                                                ChargingRateUnits?  ChargingRateUnit   = null,
                                                                                EventTracking_Id    EventTrackingId    = null)
        {

            var request = new GetCompositeScheduleRequest(ChargeBoxId,
                                                          ConnectorId,
                                                          Duration,
                                                          ChargingRateUnit,
                                                          Request_Id.Random(),
                                                          null,
                                                          EventTrackingId);

            CP.GetCompositeScheduleResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.GetCompositeSchedule(request);

            else
                response = new CP.GetCompositeScheduleResponse(request,
                                                               GetCompositeScheduleStatus.Rejected,
                                                               ConnectorId);

            return response;

        }

        #endregion

        #region UnlockConnector       (ChargeBoxId, ConnectorId, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The identifier of the connector to be unlocked.</param>
        public async Task<CP.UnlockConnectorResponse> UnlockConnector(ChargeBox_Id      ChargeBoxId,
                                                                      Connector_Id      ConnectorId,
                                                                      EventTracking_Id  EventTrackingId   = null)
        {

            var request = new UnlockConnectorRequest(ChargeBoxId,
                                                     ConnectorId,
                                                     Request_Id.Random(),
                                                     null,
                                                     EventTrackingId);

            CP.UnlockConnectorResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.UnlockConnector(request);

            else
                response = new CP.UnlockConnectorResponse(request, UnlockStatus.NotSupported);

            return response;

        }

        #endregion


        #region GetLocalListVersion   (ChargeBoxId, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        public async Task<CP.GetLocalListVersionResponse> GetLocalListVersion(ChargeBox_Id      ChargeBoxId,
                                                                              EventTracking_Id  EventTrackingId   = null)
        {

            var request = new GetLocalListVersionRequest(ChargeBoxId,
                                                         Request_Id.Random(),
                                                         null,
                                                         EventTrackingId);

            CP.GetLocalListVersionResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.GetLocalListVersion(request);

            else
                response = new CP.GetLocalListVersionResponse(request, 0);

            return response;

        }

        #endregion

        #region SendLocalList         (ChargeBoxId, ListVersion, UpdateType, LocalAuthorizationList = null, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ListVersion">In case of a full update this is the version number of the full list. In case of a differential update it is the version number of the list after the update has been applied.</param>
        /// <param name="UpdateType">The type of update (full or differential).</param>
        /// <param name="LocalAuthorizationList">In case of a full update this contains the list of values that form the new local authorization list. In case of a differential update it contains the changes to be applied to the local authorization list in the charge point. Maximum number of AuthorizationData elements is available in the configuration key: SendLocalListMaxLength.</param>
        public async Task<CP.SendLocalListResponse> SendLocalList(ChargeBox_Id                    ChargeBoxId,
                                                                  UInt64                          ListVersion,
                                                                  UpdateTypes                     UpdateType,
                                                                  IEnumerable<AuthorizationData>  LocalAuthorizationList   = null,
                                                                  EventTracking_Id                EventTrackingId          = null)
        {

            var request = new SendLocalListRequest(ChargeBoxId,
                                                   ListVersion,
                                                   UpdateType,
                                                   LocalAuthorizationList,
                                                   Request_Id.Random(),
                                                   null,
                                                   EventTrackingId);

            CP.SendLocalListResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.SendLocalList(request);

            else
                response = new CP.SendLocalListResponse(request, UpdateStatus.NotSupported);

            return response;

        }

        #endregion

        #region ClearCache            (ChargeBoxId, ...)

        /// <summary>
        /// Create a new reset request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        public async Task<CP.ClearCacheResponse> ClearCache(ChargeBox_Id      ChargeBoxId,
                                                            EventTracking_Id  EventTrackingId   = null)
        {

            var request = new ClearCacheRequest(ChargeBoxId,
                                                Request_Id.Random(),
                                                null,
                                                EventTrackingId);

            CP.ClearCacheResponse response;

            if (reachableChargingBoxes.TryGetValue(ChargeBoxId, out Tuple<ICentralSystem, DateTime> webSocketServer))
                response = await webSocketServer.Item1.ClearCache(request);

            else
                response = new CP.ClearCacheResponse(request, ClearCacheStatus.Rejected);

            return response;

        }

        #endregion

    }

}
