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
using System.Threading;
using System.Net.Security;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using cloud.charging.open.protocols.OCPPv1_6.CS;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP.v1_2;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The charge point WebSockets client runs on a charge point
    /// and connects to a central system to invoke methods.
    /// </summary>
    public partial class ChargePointWSClient : IHTTPClient,
                                               ICPClient
    {

        #region Data

        /// <summary>
        /// The default HTTP user agent string.
        /// </summary>
        public const           String  DefaultHTTPUserAgent  = "GraphDefined OCPP " + Version.Number + " CP WebSocket Client";

        /// <summary>
        /// The default remote TCP port to connect to.
        /// </summary>
        public static readonly IPPort  DefaultRemotePort     = IPPort.Parse(443);


        private Socket           TCPSocket;
        private MyNetworkStream  TCPStream;
        private SslStream        TLSStream;
        private Stream           HTTPStream;

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this charge box.
        /// </summary>
        public ChargeBox_Id    ChargeBoxIdentity   { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => ChargeBoxIdentity.ToString();

        /// <summary>
        /// The source URI of the SOAP message.
        /// </summary>
        public String          From                { get; }

        /// <summary>
        /// The destination URI of the SOAP message.
        /// </summary>
        public String          To                  { get; }


        /// <summary>
        /// The attached OCPP CP client (HTTP/SOAP client) logger.
        /// </summary>
        public ChargePointSOAPClient.CPClientLogger Logger              { get; }



        /// <summary>
        /// The remote URL of the HTTP endpoint to connect to.
        /// </summary>
        public URL                                  RemoteURL                     { get; }

        /// <summary>
        /// The virtual HTTP hostname to connect to.
        /// </summary>
        public HTTPHostname?                        VirtualHostname               { get; }

        /// <summary>
        /// An optional description of this HTTP client.
        /// </summary>
        public String                               Description                   { get; set; }

        /// <summary>
        /// The remote SSL/TLS certificate validator.
        /// </summary>
        public RemoteCertificateValidationCallback  RemoteCertificateValidator    { get; }

        /// <summary>
        /// A delegate to select a TLS client certificate.
        /// </summary>
        public LocalCertificateSelectionCallback    ClientCertificateSelector     { get; }

        /// <summary>
        /// The SSL/TLS client certificate to use of HTTP authentication.
        /// </summary>
        public X509Certificate                      ClientCert                    { get; }

        /// <summary>
        /// The HTTP user agent identification.
        /// </summary>
        public String                               HTTPUserAgent                 { get; }

        /// <summary>
        /// The timeout for upstream requests.
        /// </summary>
        public TimeSpan                             RequestTimeout                { get; set; }

        /// <summary>
        /// The delay between transmission retries.
        /// </summary>
        public TransmissionRetryDelayDelegate       TransmissionRetryDelay        { get; }

        /// <summary>
        /// The maximum number of retries when communicationg with the remote OICP service.
        /// </summary>
        public UInt16                               MaxNumberOfRetries            { get; }

        /// <summary>
        /// Whether to pipeline multiple HTTP request through a single HTTP/TCP connection.
        /// </summary>
        public Boolean                              UseHTTPPipelining             { get; }

        /// <summary>
        /// The CPO client (HTTP client) logger.
        /// </summary>
        public HTTPClientLogger                     HTTPLogger                    { get; set; }




        /// <summary>
        /// The DNS client defines which DNS servers to use.
        /// </summary>
        public DNSClient                            DNSClient                     { get; }





        ///// <summary>
        ///// The Hostname to which the HTTPClient connects.
        ///// </summary>
        //public HTTPHostname     Hostname            { get; }



        /// <summary>
        /// Our local IP port.
        /// </summary>
        public IPPort           LocalPort           { get; private set; }

        /// <summary>
        /// The IP Address to connect to.
        /// </summary>
        public IIPAddress       RemoteIPAddress     { get; protected set; }

        /// <summary>
        /// The IP port to connect to.
        /// </summary>
        public IPPort           RemotePort          { get; }



        public Int32 Available
                    => TCPSocket.Available;

        public Boolean Connected
            => TCPSocket.Connected;

        public LingerOption LingerState
        {
            get
            {
                return TCPSocket.LingerState;
            }
            set
            {
                TCPSocket.LingerState = value;
            }
        }

        public Boolean NoDelay
        {
            get
            {
                return TCPSocket.NoDelay;
            }
            set
            {
                TCPSocket.NoDelay = value;
            }
        }

        public Byte TTL
        {
            get
            {
                return (Byte) TCPSocket.Ttl;
            }
            set
            {
                TCPSocket.Ttl = value;
            }
        }


        #endregion

        #region Events

        #region OnBootNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a boot notification Request will be send to the central system.
        /// </summary>
        public event OnBootNotificationRequestDelegate   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a boot notification SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler             OnBootNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler            OnBootNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a boot notification Request was received.
        /// </summary>
        public event OnBootNotificationResponseDelegate  OnBootNotificationResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat Request will be send to the central system.
        /// </summary>
        public event OnHeartbeatRequestDelegate   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a heartbeat SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler      OnHeartbeatSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler     OnHeartbeatSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a heartbeat Request was received.
        /// </summary>
        public event OnHeartbeatResponseDelegate  OnHeartbeatResponse;

        #endregion


        #region OnAuthorizeRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize Request will be send to the central system.
        /// </summary>
        public event OnAuthorizeRequestDelegate   OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever an authorize SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler      OnAuthorizeSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler     OnAuthorizeSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to an authorize Request was received.
        /// </summary>
        public event OnAuthorizeResponseDelegate  OnAuthorizeResponse;

        #endregion

        #region OnStartTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a start transaction Request will be send to the central system.
        /// </summary>
        public event OnStartTransactionRequestDelegate   OnStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a start transaction SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler             OnStartTransactionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a start transaction SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler            OnStartTransactionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a start transaction Request was received.
        /// </summary>
        public event OnStartTransactionResponseDelegate  OnStartTransactionResponse;

        #endregion

        #region OnStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a status notification Request will be send to the central system.
        /// </summary>
        public event OnStatusNotificationRequestDelegate   OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a status notification SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler               OnStatusNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler              OnStatusNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a status notification Request was received.
        /// </summary>
        public event OnStatusNotificationResponseDelegate  OnStatusNotificationResponse;

        #endregion

        #region OnMeterValuesRequest/-Response

        /// <summary>
        /// An event fired whenever a meter values Request will be send to the central system.
        /// </summary>
        public event OnMeterValuesRequestDelegate   OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a meter values SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler        OnMeterValuesSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler       OnMeterValuesSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a meter values Request was received.
        /// </summary>
        public event OnMeterValuesResponseDelegate  OnMeterValuesResponse;

        #endregion

        #region OnStopTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a stop transaction Request will be send to the central system.
        /// </summary>
        public event OnStopTransactionRequestDelegate   OnStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a stop transaction SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler            OnStopTransactionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a stop transaction SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler           OnStopTransactionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a stop transaction Request was received.
        /// </summary>
        public event OnStopTransactionResponseDelegate  OnStopTransactionResponse;

        #endregion


        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer Request will be send to the central system.
        /// </summary>
        public event OnDataTransferRequestDelegate   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a data transfer SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler         OnDataTransferSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler        OnDataTransferSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a data transfer Request was received.
        /// </summary>
        public event OnDataTransferResponseDelegate  OnDataTransferResponse;

        #endregion

        #region OnDiagnosticsStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a diagnostics status notification Request will be send to the central system.
        /// </summary>
        public event OnDiagnosticsStatusNotificationRequestDelegate   OnDiagnosticsStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a diagnostics status notification SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler                          OnDiagnosticsStatusNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a diagnostics status notification SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler                         OnDiagnosticsStatusNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a diagnostics status notification Request was received.
        /// </summary>
        public event OnDiagnosticsStatusNotificationResponseDelegate  OnDiagnosticsStatusNotificationResponse;

        #endregion

        #region OnFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a firmware status notification Request will be send to the central system.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a firmware status notification SOAP Request will be send to the central system.
        /// </summary>
        public event ClientRequestLogHandler                       OnFirmwareStatusNotificationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification SOAP Request was received.
        /// </summary>
        public event ClientResponseLogHandler                      OnFirmwareStatusNotificationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification Request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate  OnFirmwareStatusNotificationResponse;

        #endregion

        #endregion

        #region Constructor(s)

        #region ChargePointWSClient(Request.ChargeBoxId, Hostname, ..., LoggingContext = CPClientLogger.DefaultContext, ...)

        /// <summary>
        /// Create a new charge point SOAP client running on a charge point
        /// and connecting to a central system to invoke methods.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of this charge box.</param>
        /// <param name="From">The source URI of the SOAP message.</param>
        /// <param name="To">The destination URI of the SOAP message.</param>
        /// 
        /// <param name="RemoteURL">The remote URL of the HTTP endpoint to connect to.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this HTTP/SOAP client.</param>
        /// <param name="RemoteCertificateValidator">The remote SSL/TLS certificate validator.</param>
        /// <param name="ClientCertificateSelector">A delegate to select a TLS client certificate.</param>
        /// <param name="ClientCert">The SSL/TLS client certificate to use of HTTP authentication.</param>
        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
        /// <param name="URLPathPrefix">An optional default URL path prefix.</param>
        /// <param name="WSSLoginPassword">The WebService-Security username/password.</param>
        /// <param name="RequestTimeout">An optional Request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
        /// <param name="UseHTTPPipelining">Whether to pipeline multiple HTTP Request through a single HTTP/TCP connection.</param>
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="LoggingContext">An optional context for logging client methods.</param>
        /// <param name="LogFileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public ChargePointWSClient(ChargeBox_Id                         ChargeBoxIdentity,
                                   String                               From,
                                   String                               To,

                                   URL                                  RemoteURL,
                                   HTTPHostname?                        VirtualHostname              = null,
                                   String                               Description                  = null,
                                   RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
                                   LocalCertificateSelectionCallback    ClientCertificateSelector    = null,
                                   X509Certificate                      ClientCert                   = null,
                                   String                               HTTPUserAgent                = DefaultHTTPUserAgent,
                                   HTTPPath?                            URLPathPrefix                = null,
                                   Tuple<String, String>                WSSLoginPassword             = null,
                                   TimeSpan?                            RequestTimeout               = null,
                                   TransmissionRetryDelayDelegate       TransmissionRetryDelay       = null,
                                   UInt16?                              MaxNumberOfRetries           = 3,
                                   Boolean                              UseHTTPPipelining            = false,

                                   String                               LoggingPath                  = null,
                                   String                               LoggingContext               = ChargePointSOAPClient.CPClientLogger.DefaultContext,
                                   LogfileCreatorDelegate               LogFileCreator               = null,
                                   HTTPClientLogger                     HTTPLogger                   = null,
                                   DNSClient                            DNSClient                    = null)

        {

            #region Initial checks

            if (ChargeBoxIdentity.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null or empty!");

            if (From.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(From),               "The given SOAP message source must not be null or empty!");

            if (To.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(To),                 "The given SOAP message destination must not be null or empty!");

            #endregion

            this.RemoteURL                   = RemoteURL;
            this.VirtualHostname             = VirtualHostname;
            this.Description                 = Description;
            this.RemoteCertificateValidator  = RemoteCertificateValidator;
            this.ClientCertificateSelector   = ClientCertificateSelector;
            this.ClientCert                  = ClientCert;
            this.HTTPUserAgent               = HTTPUserAgent;
            //this.URLPathPrefix               = URLPathPrefix;
            //this.WSSLoginPassword            = WSSLoginPassword;
            this.RequestTimeout              = RequestTimeout     ?? TimeSpan.FromSeconds(10);
            this.TransmissionRetryDelay      = TransmissionRetryDelay;
            this.MaxNumberOfRetries          = MaxNumberOfRetries ?? 3;
            this.UseHTTPPipelining           = UseHTTPPipelining;
            this.HTTPLogger                  = HTTPLogger;
            this.DNSClient                   = DNSClient;

            this.ChargeBoxIdentity           = ChargeBoxIdentity;
            this.From                        = From;
            this.To                          = To;

            //this.Logger                      = new ChargePointSOAPClient.CPClientLogger(this,
            //                                                                            LoggingPath,
            //                                                                            LoggingContext,
            //                                                                            LogFileCreator);

        }

        #endregion

        #region ChargePointWSClient(Request.ChargeBoxId, Logger, Hostname, ...)

        /// <summary>
        /// Create a new charge point SOAP client.
        /// </summary>
        /// <param name="ChargeBoxIdentity">A unqiue identification of this client.</param>
        /// <param name="From">The source URI of the SOAP message.</param>
        /// <param name="To">The destination URI of the SOAP message.</param>
        /// 
        /// <param name="Hostname">The OCPP hostname to connect to.</param>
        /// <param name="RemotePort">An optional OCPP TCP port to connect to.</param>
        /// <param name="RemoteCertificateValidator">A delegate to verify the remote TLS certificate.</param>
        /// <param name="ClientCertificateSelector">A delegate to select a TLS client certificate.</param>
        /// <param name="HTTPVirtualHost">An optional HTTP virtual host name to use.</param>
        /// <param name="URLPrefix">An default URI prefix.</param>
        /// <param name="HTTPUserAgent">An optional HTTP user agent to use.</param>
        /// <param name="RequestTimeout">An optional timeout for upstream queries.</param>
        /// <param name="MaxNumberOfRetries">The default number of maximum transmission retries.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        //public ChargePointWSClient(String                               ChargeBoxIdentity,
        //                             String                               From,
        //                             String                               To,

        //                             CPClientLogger                       Logger,
        //                             HTTPHostname                         Hostname,
        //                             IPPort?                              RemotePort                   = null,
        //                             RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
        //                             LocalCertificateSelectionCallback    ClientCertificateSelector    = null,
        //                             HTTPHostname?                        HTTPVirtualHost              = null,
        //                             HTTPPath?                            URLPrefix                    = null,
        //                             String                               HTTPUserAgent                = DefaultHTTPUserAgent,
        //                             TimeSpan?                            RequestTimeout               = null,
        //                             Byte?                                MaxNumberOfRetries           = DefaultMaxNumberOfRetries,
        //                             DNSClient                            DNSClient                    = null)

        //    : base(Request.ChargeBoxId,
        //           Hostname,
        //           RemotePort ?? DefaultRemotePort,
        //           RemoteCertificateValidator,
        //           ClientCertificateSelector,
        //           HTTPVirtualHost,
        //           URLPrefix ?? DefaultURLPrefix,
        //           null,
        //           HTTPUserAgent,
        //           RequestTimeout,
        //           null,
        //           MaxNumberOfRetries,
        //           DNSClient)

        //{

        //    #region Initial checks

        //    if (ChargeBoxIdentity.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null or empty!");

        //    if (From.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(From),               "The given SOAP message source must not be null or empty!");

        //    if (To.IsNullOrEmpty())
        //        throw new ArgumentNullException(nameof(To),                 "The given SOAP message destination must not be null or empty!");

        //    #endregion

        //    this.From    = From;
        //    this.To      = To;

        //    this.Logger  = Logger ?? throw new ArgumentNullException(nameof(Hostname), "The given hostname must not be null or empty!");

        //}

        #endregion

        #endregion



        #region Connect(Request, RequestLogDelegate = null, ResponseLogDelegate = null, Timeout = null, CancellationToken = null)

        /// <summary>
        /// Execute the given HTTP request and return its result.
        /// </summary>
        /// <param name="CancellationToken">A cancellation token.</param>
        /// <param name="EventTrackingId"></param>
        /// <param name="RequestTimeout">An optional timeout.</param>
        /// <param name="NumberOfRetry">The number of retransmissions of this request.</param>
        public async Task<HTTPResponse> Connect(CancellationToken?        CancellationToken     = null,
                                                EventTracking_Id          EventTrackingId       = null,
                                                TimeSpan?                 RequestTimeout        = null,
                                                Byte                      NumberOfRetry         = 0)
        {

            HTTPResponse Response = null;

            try
            {

                //Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

                #region Data

                var HTTPHeaderBytes   = new Byte[0];
                var HTTPBodyBytes     = new Byte[0];
                var sw                = new Stopwatch();

                #endregion

                #region Create TCP connection (possibly also do DNS lookups)

                Boolean restart;

                do
                {

                    restart = false;

                    if (TCPSocket == null)
                    {

                        System.Net.IPEndPoint _FinalIPEndPoint = null;

                        if (RemoteIPAddress == null)
                        {

                            if      (IPAddress.IsIPv4Localhost(RemoteURL.Hostname))
                                RemoteIPAddress = IPv4Address.Localhost;

                            else if (IPAddress.IsIPv6Localhost(RemoteURL.Hostname))
                                RemoteIPAddress = IPv6Address.Localhost;

                            else if (IPAddress.IsIPv4(RemoteURL.Hostname.Name))
                                RemoteIPAddress = IPv4Address.Parse(RemoteURL.Hostname.Name);

                            else if (IPAddress.IsIPv6(RemoteURL.Hostname.Name))
                                RemoteIPAddress = IPv6Address.Parse(RemoteURL.Hostname.Name);

                            #region DNS lookup...

                            if (RemoteIPAddress == null)
                            {

                                var IPv4AddressLookupTask  = DNSClient.
                                                                 Query<A>(RemoteURL.Hostname.Name).
                                                                 ContinueWith(query => query.Result.Select(ARecord    => ARecord.IPv4Address));

                                var IPv6AddressLookupTask  = DNSClient.
                                                                 Query<AAAA>(RemoteURL.Hostname.Name).
                                                                 ContinueWith(query => query.Result.Select(AAAARecord => AAAARecord.IPv6Address));

                                await Task.WhenAll(IPv4AddressLookupTask,
                                                   IPv6AddressLookupTask).
                                           ConfigureAwait(false);


                                if (IPv4AddressLookupTask.Result.Any())
                                    RemoteIPAddress = IPv4AddressLookupTask.Result.First();

                                else if (IPv6AddressLookupTask.Result.Any())
                                    RemoteIPAddress = IPv6AddressLookupTask.Result.First();


                                if (RemoteIPAddress == null || RemoteIPAddress.GetBytes() == null)
                                    throw new Exception("DNS lookup failed!");

                            }

                            #endregion

                        }

                        _FinalIPEndPoint = new System.Net.IPEndPoint(new System.Net.IPAddress(RemoteIPAddress.GetBytes()),
                                                                     RemotePort.ToInt32());

                        sw.Start();

                        //TCPClient = new TcpClient();
                        //TCPClient.Connect(_FinalIPEndPoint);
                        //TCPClient.ReceiveTimeout = (Int32) RequestTimeout.Value.TotalMilliseconds;


                        if (RemoteIPAddress.IsIPv4)
                            TCPSocket = new Socket(AddressFamily.InterNetwork,
                                                   SocketType.Stream,
                                                   ProtocolType.Tcp);

                        else if (RemoteIPAddress.IsIPv6)
                            TCPSocket = new Socket(AddressFamily.InterNetworkV6,
                                                   SocketType.Stream,
                                                   ProtocolType.Tcp);

                        TCPSocket.SendTimeout    = (Int32) RequestTimeout.Value.TotalMilliseconds;
                        TCPSocket.ReceiveTimeout = (Int32) RequestTimeout.Value.TotalMilliseconds;
                        TCPSocket.Connect(_FinalIPEndPoint);
                        TCPSocket.ReceiveTimeout = (Int32) RequestTimeout.Value.TotalMilliseconds;

                    }

                    TCPStream = new MyNetworkStream(TCPSocket, true) {
                                    ReadTimeout = (Int32) RequestTimeout.Value.TotalMilliseconds
                                };

                #endregion

                #region Create (Crypto-)Stream

                    if (RemoteCertificateValidator != null)
                    {

                        if (TLSStream == null)
                        {

                            TLSStream = new SslStream(TCPStream,
                                                      false,
                                                      RemoteCertificateValidator,
                                                      ClientCertificateSelector,
                                                      EncryptionPolicy.RequireEncryption)
                            {

                                ReadTimeout = (Int32) RequestTimeout.Value.TotalMilliseconds

                            };

                            HTTPStream = TLSStream;

                            try
                            {

                                await TLSStream.AuthenticateAsClientAsync(RemoteURL.Hostname.Name,
                                                                          null,
                                                                          SslProtocols.Tls12,
                                                                          false);//, new X509CertificateCollection(new X509Certificate[] { ClientCert }), SslProtocols.Default, true);

                            }
                            catch (Exception e)
                            {
                                TCPSocket = null;
                                restart = true;
                            }

                        }

                    }

                    else
                    {
                        TLSStream   = null;
                        HTTPStream  = TCPStream;
                    }

                    HTTPStream.ReadTimeout = (Int32) RequestTimeout.Value.TotalMilliseconds;

                }
                while (restart);

                #endregion

                this.LocalPort        = IPSocket.FromIPEndPoint(TCPStream.Socket.LocalEndPoint).Port;
                //Request.HTTPSource    = new HTTPSource(IPSocket.FromIPEndPoint(TCPStream.Socket.LocalEndPoint));
                //this.RemotePort       = IPSocket.FromIPEndPoint(TCPStream.Socket.RemoteEndPoint).Port;

                #region Call the optional HTTP request log delegate

                //try
                //{

                //    if (RequestLogDelegate != null)
                //        await Task.WhenAll(RequestLogDelegate.GetInvocationList().
                //                           Cast<ClientRequestLogHandler>().
                //                           Select(e => e(DateTime.UtcNow,
                //                                         this,
                //                                         Request))).
                //                           ConfigureAwait(false);

                //}
                //catch (Exception e)
                //{
                //    DebugX.Log(e, nameof(HTTPClient) + "." + nameof(RequestLogDelegate));
                //}

                #endregion

                #region Send Request

                HTTPStream.Write(String.Concat(Request.EntireRequestHeader, "\r\n\r\n").
                                        ToUTF8Bytes());

                //var RequestBodyLength = Request.HTTPBody == null
                //                            ? Request.ContentLength.HasValue ? (Int32) Request.ContentLength.Value : 0
                //                            : Request.ContentLength.HasValue ? Math.Min((Int32)Request.ContentLength.Value, Request.HTTPBody.Length) : Request.HTTPBody.Length;

                //if (RequestBodyLength > 0)
                //    HTTPStream.Write(Request.HTTPBody, 0, RequestBodyLength);

                //DebugX.LogT("HTTPClient (" + Request.HTTPMethod + " " + Request.URL + ") sent request of " + Request.EntirePDU.Length + " bytes at " + sw.ElapsedMilliseconds + "ms!");

                var _InternalHTTPStream  = new MemoryStream();

                #endregion

                #region Wait timeout for the server to react!


                while (!TCPStream.DataAvailable)
                {

                    if (sw.ElapsedMilliseconds >= RequestTimeout.Value.TotalMilliseconds)
                        throw new HTTPTimeoutException(sw.Elapsed);

                    Thread.Sleep(1);

                }

                //DebugX.LogT("HTTPClient (" + Request.HTTPMethod + " " + Request.URL + ") got first response after " + sw.ElapsedMilliseconds + "ms!");

                #endregion


                #region Read the HTTP header

                var currentDataLength    = 0;
                var CurrentHeaderLength  = 0;
                var HeaderEndsAt         = 0;

                var _Buffer = new Byte[65536];

                do
                {

                    currentDataLength = HTTPStream.Read(_Buffer, 0, _Buffer.Length);

                    if (currentDataLength > 0)
                    {

                        if (currentDataLength > 3 || CurrentHeaderLength > 3)
                        {

                            for (var pos = 3; pos < _Buffer.Length; pos++)
                            {
                                if (_Buffer[pos    ] == 0x0a &&
                                    _Buffer[pos - 1] == 0x0d &&
                                    _Buffer[pos - 2] == 0x0a &&
                                    _Buffer[pos - 3] == 0x0d)
                                {
                                    HeaderEndsAt = pos - 3;
                                    break;
                                }
                            }

                            if (HeaderEndsAt > 0)
                            {

                                Array.Resize(ref HTTPHeaderBytes, CurrentHeaderLength + HeaderEndsAt);
                                Array.Copy(_Buffer, 0, HTTPHeaderBytes, CurrentHeaderLength, HeaderEndsAt);
                                CurrentHeaderLength += HeaderEndsAt;

                                // We already read a bit of the HTTP body!
                                if (HeaderEndsAt + 4 < currentDataLength)
                                {
                                    Array.Resize(ref HTTPBodyBytes, currentDataLength - 4 - HeaderEndsAt);
                                    Array.Copy(_Buffer, HeaderEndsAt + 4, HTTPBodyBytes, 0, HTTPBodyBytes.Length);
                                }

                            }

                            else
                            {
                                Array.Resize(ref HTTPHeaderBytes, CurrentHeaderLength + _Buffer.Length);
                                Array.Copy(_Buffer, 0, HTTPHeaderBytes, CurrentHeaderLength, _Buffer.Length);
                                CurrentHeaderLength += _Buffer.Length;
                                Thread.Sleep(1);
                            }

                        }

                    }

                } while (HeaderEndsAt == 0 &&
                         sw.ElapsedMilliseconds < HTTPStream.ReadTimeout);

                if (HTTPHeaderBytes.Length == 0)
                    throw new ApplicationException("[" + DateTime.UtcNow.ToString() + "] Could not find the end of the HTTP protocol header!");

                Response = HTTPResponse.Parse(HTTPHeaderBytes.ToUTF8String(),
                                              Request,
                                              HTTPBodyBytes,
                                              NumberOfRetry);

                #endregion


                _Buffer = new Byte[50 * 1024 * 1024];

                #region A single fixed-lenght HTTP request -> read '$Content-Length' bytes...

                // Copy only the number of bytes given within
                // the HTTP header element 'Content-Length'!
                if (Response.ContentLength.HasValue && Response.ContentLength.Value > 0)
                {

                    // Test via:
                    // var aaa = new HTTPClient("www.networksorcery.com").GET("/enp/rfc/rfc1000.txt").ExecuteReturnResult().Result;

                    var _StillToRead = (Int32) Response.ContentLength.Value - Response.HTTPBody.Length;

                    do
                    {

                        while (//TCPStream.DataAvailable &&  <= Does not work as expected!
                               _StillToRead > 0)
                        {

                            currentDataLength = HTTPStream.Read(_Buffer, 0, Math.Min(_Buffer.Length, _StillToRead));

                            if (currentDataLength > 0)
                            {
                                var OldSize = Response.HTTPBody.Length;
                                Response.ResizeBody(OldSize + currentDataLength);
                                Array.Copy(_Buffer, 0, Response.HTTPBody, OldSize, currentDataLength);
                                _StillToRead -= currentDataLength;
                            }

                        }

                        OnDataRead?.Invoke(sw.Elapsed,
                                           Response.ContentLength.Value - (UInt64) _StillToRead,
                                           Response.ContentLength.Value);

                        if (_StillToRead <= 0)
                            break;

                        Thread.Sleep(1);

                    }
                    while (sw.ElapsedMilliseconds < HTTPStream.ReadTimeout);

                }

                #endregion

                #region ...or chunked transport...

                else if (Response.TransferEncoding == "chunked")
                {

                    DebugX.Log("HTTP Client: Chunked Transport detected...");

                    try
                    {

                        Response.NewContentStream();
                        var chunkedStream            = new MemoryStream();

                        // Write the first buffer (without the HTTP header) to the chunked stream...
                        chunkedStream.Write(HTTPBodyBytes, 0, HTTPBodyBytes.Length);

                        var chunkedArray             = chunkedStream.ToArray();
                        var decodedStream            = new MemoryStream();
                        var currentPosition          = 2;
                        var lastPosition             = 0;
                        var currentBlockNumber       = 1UL;
                        var chunkedDecodingFinished  = 0;

                        do
                        {

                            #region Read more data from network

                            if (TCPStream.DataAvailable)
                            {

                                do
                                {

                                    currentDataLength = HTTPStream.Read(_Buffer, 0, _Buffer.Length);

                                    if (currentDataLength > 0)
                                        chunkedStream.Write(_Buffer, 0, currentDataLength);

                                    OnChunkDataRead?.Invoke(sw.Elapsed,
                                                            (UInt32) currentDataLength,
                                                            (UInt64) chunkedStream.Length);

                                    if (sw.ElapsedMilliseconds > HTTPStream.ReadTimeout)
                                        chunkedDecodingFinished = 2;

                                } while (TCPStream.DataAvailable && chunkedDecodingFinished == 0);

                                chunkedArray = chunkedStream.ToArray();

                            }

                            #endregion

                            #region Process chunks

                            if (chunkedArray.Length >= currentPosition &&
                                chunkedArray[currentPosition - 1] == '\n' &&
                                chunkedArray[currentPosition - 2] == '\r')
                            {

                                var BlockLength = chunkedArray.ReadTEBlockLength(lastPosition,
                                                                                 currentPosition - lastPosition - 2);

                                // End of stream reached?
                                if (BlockLength == 0)
                                {
                                    Response.ContentStreamToArray(decodedStream);
                                    chunkedDecodingFinished = 1;
                                    break;
                                }

                                // Read a new block...
                                if (currentPosition + BlockLength <= chunkedArray.Length)
                                {

                                    OnChunkBlockFound?.Invoke(sw.Elapsed,
                                                              currentBlockNumber,
                                                              (UInt32) BlockLength,
                                                              (UInt64) decodedStream.Length);

                                    currentBlockNumber++;

                                    decodedStream.Write(chunkedArray, currentPosition, BlockLength);
                                    currentPosition += BlockLength;

                                    if (currentPosition < chunkedArray.Length &&
                                        chunkedArray[currentPosition] == 0x0d)
                                    {
                                        currentPosition++;
                                    }

                                    if (currentPosition < chunkedArray.Length - 1 &&
                                        chunkedArray[currentPosition] == 0x0a)
                                    {
                                        currentPosition++;
                                    }

                                    lastPosition = currentPosition;
                                    currentPosition++;

                                }

                            }

                            #endregion

                            else
                                currentPosition++;

                            if (sw.ElapsedMilliseconds > HTTPStream.ReadTimeout)
                                chunkedDecodingFinished = 2;

                        } while (chunkedDecodingFinished == 0);

                        if (chunkedDecodingFinished == 2)
                            DebugX.Log("HTTP Client: Chunked decoding timeout!");
                        else
                            DebugX.Log("HTTP Client: Chunked decoding finished!");

                    }
                    catch (Exception e)
                    {
                        DebugX.Log("HTTP Client: Chunked decoding failed: " + e.Message);
                    }

                }

                #endregion

                #region ...or just connect HTTP stream to network stream!

                else
                {
                    Response.ContentStreamToArray();
                }

                #endregion

                #region Close connection if requested!

                if (Response.Connection == null ||
                    Response.Connection == "close")
                {

                    if (TLSStream != null)
                    {
                        TLSStream.Close();
                        TLSStream = null;
                    }

                    if (TCPSocket != null)
                    {
                        TCPSocket.Close();
                        //TCPClient.Dispose();
                        TCPSocket = null;
                    }

                    HTTPStream = null;

                }

                #endregion

            }
            catch (HTTPTimeoutException e)
            {

                #region Create a HTTP response for the exception...

                Response = new HTTPResponse.Builder(Request,
                                                    HTTPStatusCode.RequestTimeout)
                {

                    ContentType  = HTTPContentType.JSON_UTF8,
                    Content      = JSONObject.Create(new JProperty("timeout",     (Int32) e.Timeout.TotalMilliseconds),
                                                     new JProperty("message",     e.Message),
                                                     new JProperty("stackTrace",  e.StackTrace)).
                                              ToUTF8Bytes()

                };

                #endregion

                if (TLSStream != null)
                {
                    TLSStream.Close();
                    TLSStream = null;
                }

                if (TCPSocket != null)
                {
                    TCPSocket.Close();
                    //TCPClient.Dispose();
                    TCPSocket = null;
                }

            }
            catch (Exception e)
            {

                #region Create a HTTP response for the exception...

                while (e.InnerException != null)
                    e = e.InnerException;

                Response = new HTTPResponse.Builder(Request,
                                                    HTTPStatusCode.BadRequest)
                {

                    ContentType  = HTTPContentType.JSON_UTF8,
                    Content      = JSONObject.Create(new JProperty("message",     e.Message),
                                                     new JProperty("stackTrace",  e.StackTrace)).
                                              ToUTF8Bytes()

                };

                #endregion

                if (TLSStream != null)
                {
                    TLSStream.Close();
                    TLSStream = null;
                }

                if (TCPSocket != null)
                {
                    TCPSocket.Close();
                    //TCPClient.Dispose();
                    TCPSocket = null;
                }

            }


            #region Call the optional HTTP response log delegate

            //try
            //{

            //    if (ResponseLogDelegate != null)
            //        await Task.WhenAll(ResponseLogDelegate.GetInvocationList().
            //                           Cast<ClientResponseLogHandler>().
            //                           Select(e => e(DateTime.UtcNow,
            //                                         this,
            //                                         Request,
            //                                         Response))).
            //                           ConfigureAwait(false);

            //}
            //catch (Exception e2)
            //{
            //    DebugX.Log(e2, nameof(HTTPClient) + "." + nameof(ResponseLogDelegate));
            //}

            #endregion

            return Response;

        }

        #endregion



        private async Task<String> SendRequest(String RequestData)
        {

            HTTPStream.Write(RequestData.ToUTF8Bytes());

            HTTPStream.Write("\r\n\r\n".ToUTF8Bytes());

            return "OK";

        }



        #region SendBootNotification             (Request, ...)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="Request">A boot notification request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<BootNotificationResponse>>

            SendBootNotification(BootNotificationRequest  Request,

                                 DateTime?                Timestamp           = null,
                                 CancellationToken?       CancellationToken   = null,
                                 EventTracking_Id         EventTrackingId     = null,
                                 TimeSpan?                RequestTimeout      = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given boot notification request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<BootNotificationResponse> result = null;

            #endregion

            #region Send OnBootNotificationRequest event

            try
            {

                OnBootNotificationRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                  this,
                                                  Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnBootNotificationRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                    VirtualHostname,
                                                    true,
                                                    null,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    ClientCert,
                                                    HTTPUserAgent,
                                                    URLPathPrefix,
                                                    WSSLoginPassword,
                                                    RequestTimeout,
                                                    TransmissionRetryDelay,
                                                    MaxNumberOfRetries,
                                                    UseHTTPPipelining,
                                                    HTTPLogger,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/BootNotification",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "BootNotification",
                                                 RequestLogDelegate:   OnBootNotificationSOAPRequest,
                                                 ResponseLogDelegate:  OnBootNotificationSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request, BootNotificationResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<BootNotificationResponse>(httpresponse,
                                                                                                       new BootNotificationResponse(
                                                                                                           Request,
                                                                                                           Result.Format(
                                                                                                               "Invalid SOAP => " +
                                                                                                               httpresponse.HTTPBody.ToUTF8String()
                                                                                                           )
                                                                                                       ),
                                                                                                       IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return new HTTPResponse<BootNotificationResponse>(httpresponse,
                                                                                                       new BootNotificationResponse(
                                                                                                           Request,
                                                                                                           Result.Server(
                                                                                                                httpresponse.HTTPStatusCode.ToString() +
                                                                                                                " => " +
                                                                                                                httpresponse.HTTPBody.      ToUTF8String()
                                                                                                           )
                                                                                                       ),
                                                                                                       IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<BootNotificationResponse>.ExceptionThrown(new BootNotificationResponse(
                                                                                                                       Request,
                                                                                                                       Result.Format(exception.Message +
                                                                                                                                     " => " +
                                                                                                                                     exception.StackTrace)),
                                                                                                                   exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<BootNotificationResponse>.OK(new BootNotificationResponse(Request, Result.OK("Nothing to upload!")));


            #region Send OnBootNotificationResponse event

            try
            {

                OnBootNotificationResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                   this,
                                                   Request,
                                                   result.Content,
                                                   org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnBootNotificationResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region SendHeartbeat                    (Request, ...)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="Request">A heartbeat request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<HeartbeatResponse>>

            SendHeartbeat(HeartbeatRequest    Request,

                          DateTime?           Timestamp           = null,
                          CancellationToken?  CancellationToken   = null,
                          EventTracking_Id    EventTrackingId     = null,
                          TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given heartbeat Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<HeartbeatResponse> result = null;

            #endregion

            #region Send OnHeartbeatRequest event

            try
            {

                OnHeartbeatRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                           this,
                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnHeartbeatRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                    VirtualHostname,
                                                    true,
                                                    null,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    ClientCert,
                                                    HTTPUserAgent,
                                                    URLPathPrefix,
                                                    WSSLoginPassword,
                                                    RequestTimeout,
                                                    TransmissionRetryDelay,
                                                    MaxNumberOfRetries,
                                                    UseHTTPPipelining,
                                                    HTTPLogger,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/Heartbeat",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "Heartbeat",
                                                 RequestLogDelegate:   OnHeartbeatSOAPRequest,
                                                 ResponseLogDelegate:  OnHeartbeatSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request, HeartbeatResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<HeartbeatResponse>(httpresponse,
                                                                                                new HeartbeatResponse(
                                                                                                    Request,
                                                                                                    Result.Format(
                                                                                                        "Invalid SOAP => " +
                                                                                                        httpresponse.HTTPBody.ToUTF8String()
                                                                                                    )
                                                                                                ),
                                                                                                IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return new HTTPResponse<HeartbeatResponse>(httpresponse,
                                                                                                new HeartbeatResponse(
                                                                                                    Request,
                                                                                                    Result.Server(
                                                                                                         httpresponse.HTTPStatusCode.ToString() +
                                                                                                         " => " +
                                                                                                         httpresponse.HTTPBody.      ToUTF8String()
                                                                                                    )
                                                                                                ),
                                                                                                IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<HeartbeatResponse>.ExceptionThrown(new HeartbeatResponse(
                                                                                                                Request,
                                                                                                                Result.Format(exception.Message +
                                                                                                                              " => " +
                                                                                                                              exception.StackTrace)),
                                                                                                            exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<HeartbeatResponse>.OK(new HeartbeatResponse(Request, Result.OK("Nothing to upload!")));


            #region Send OnHeartbeatResponse event

            try
            {

                OnHeartbeatResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                            this,
                                            Request,
                                            result.Content,
                                            org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnHeartbeatResponse));
            }

            #endregion

            return result;

        }

        #endregion


        #region Authorize                        (Request, ...)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="Request">An authorize request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<AuthorizeResponse>>

            Authorize(AuthorizeRequest    Request,

                      DateTime?           Timestamp           = null,
                      CancellationToken?  CancellationToken   = null,
                      EventTracking_Id    EventTrackingId     = null,
                      TimeSpan?           RequestTimeout      = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given authorize Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<AuthorizeResponse> result = null;

            #endregion

            #region Send OnAuthorizeRequest event

            try
            {

                OnAuthorizeRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                           this,
                                           Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnAuthorizeRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                    VirtualHostname,
                                                    true,
                                                    null,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    ClientCert,
                                                    HTTPUserAgent,
                                                    URLPathPrefix,
                                                    WSSLoginPassword,
                                                    RequestTimeout,
                                                    TransmissionRetryDelay,
                                                    MaxNumberOfRetries,
                                                    UseHTTPPipelining,
                                                    HTTPLogger,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/Authorize",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "Authorize",
                                                 RequestLogDelegate:   OnAuthorizeSOAPRequest,
                                                 ResponseLogDelegate:  OnAuthorizeSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                                                      AuthorizeResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<AuthorizeResponse>(httpresponse,
                                                                                                new AuthorizeResponse(
                                                                                                    Request,
                                                                                                    Result.Format(
                                                                                                        "Invalid SOAP => " +
                                                                                                        httpresponse.HTTPBody.ToUTF8String()
                                                                                                    )
                                                                                                ),
                                                                                                IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return new HTTPResponse<AuthorizeResponse>(httpresponse,
                                                                                                new AuthorizeResponse(
                                                                                                    Request,
                                                                                                    Result.Server(
                                                                                                         httpresponse.HTTPStatusCode.ToString() +
                                                                                                         " => " +
                                                                                                         httpresponse.HTTPBody.      ToUTF8String()
                                                                                                    )
                                                                                                ),
                                                                                                IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<AuthorizeResponse>.ExceptionThrown(new AuthorizeResponse(
                                                                                                                Request,
                                                                                                                Result.Format(exception.Message +
                                                                                                                              " => " +
                                                                                                                              exception.StackTrace)),
                                                                                                            exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<AuthorizeResponse>.OK(new AuthorizeResponse(Request, Result.OK("Nothing to upload!")));


            #region Send OnAuthorizeResponse event

            try
            {

                OnAuthorizeResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                            this,
                                            Request,
                                            result.Content,
                                            org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnAuthorizeResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region StartTransaction                 (Request, ...)

        /// <summary>
        /// Start a charging process at the given connector.
        /// </summary>
        /// <param name="Request">A start transaction request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<StartTransactionResponse>>

            StartTransaction(StartTransactionRequest  Request,

                             DateTime?                Timestamp          = null,
                             CancellationToken?       CancellationToken  = null,
                             EventTracking_Id         EventTrackingId    = null,
                             TimeSpan?                RequestTimeout     = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given start transaction Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<StartTransactionResponse> result = null;

            #endregion

            #region Send OnStartTransactionRequest event

            try
            {

                OnStartTransactionRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                  this,
                                                  Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnStartTransactionRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                    VirtualHostname,
                                                    true,
                                                    null,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    ClientCert,
                                                    HTTPUserAgent,
                                                    URLPathPrefix,
                                                    WSSLoginPassword,
                                                    RequestTimeout,
                                                    TransmissionRetryDelay,
                                                    MaxNumberOfRetries,
                                                    UseHTTPPipelining,
                                                    HTTPLogger,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/StartTransaction",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "StartTransaction",
                                                 RequestLogDelegate:   OnStartTransactionSOAPRequest,
                                                 ResponseLogDelegate:  OnStartTransactionSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request, StartTransactionResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<StartTransactionResponse>(httpresponse,
                                                                                                       new StartTransactionResponse(
                                                                                                           Request,
                                                                                                           Result.Format(
                                                                                                               "Invalid SOAP => " +
                                                                                                               httpresponse.HTTPBody.ToUTF8String()
                                                                                                           )
                                                                                                       ),
                                                                                                       IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return new HTTPResponse<StartTransactionResponse>(httpresponse,
                                                                                                       new StartTransactionResponse(
                                                                                                           Request,
                                                                                                           Result.Server(
                                                                                                                httpresponse.HTTPStatusCode.ToString() +
                                                                                                                " => " +
                                                                                                                httpresponse.HTTPBody.      ToUTF8String()
                                                                                                           )
                                                                                                       ),
                                                                                                       IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<StartTransactionResponse>.ExceptionThrown(new StartTransactionResponse(
                                                                                                                       Request,
                                                                                                                       Result.Format(exception.Message +
                                                                                                                                     " => " +
                                                                                                                                     exception.StackTrace)),
                                                                                                                   exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<StartTransactionResponse>.OK(new StartTransactionResponse(Request, Result.OK("Nothing to upload!")));


            #region Send OnStartTransactionResponse event

            try
            {

                OnStartTransactionResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                   this,
                                                   Request,
                                                   result.Content,
                                                   org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnStartTransactionResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region SendStatusNotification           (Request, ...)

        /// <summary>
        /// Send a status notification for the given connector.
        /// </summary>
        /// <param name="Request">A status notification request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<StatusNotificationResponse>>

            SendStatusNotification(StatusNotificationRequest  Request,

                                   DateTime?                  Timestamp          = null,
                                   CancellationToken?         CancellationToken  = null,
                                   EventTracking_Id           EventTrackingId    = null,
                                   TimeSpan?                  RequestTimeout     = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given status notification Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<StatusNotificationResponse> result = null;

            #endregion

            #region Send OnStatusNotificationRequest event

            try
            {

                OnStatusNotificationRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                    this,
                                                    Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnStatusNotificationRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                    VirtualHostname,
                                                    true,
                                                    null,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    ClientCert,
                                                    HTTPUserAgent,
                                                    URLPathPrefix,
                                                    WSSLoginPassword,
                                                    RequestTimeout,
                                                    TransmissionRetryDelay,
                                                    MaxNumberOfRetries,
                                                    UseHTTPPipelining,
                                                    HTTPLogger,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/StatusNotification",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "StatusNotification",
                                                 RequestLogDelegate:   OnStatusNotificationSOAPRequest,
                                                 ResponseLogDelegate:  OnStatusNotificationSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                                                      StatusNotificationResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<StatusNotificationResponse>(httpresponse,
                                                                                                         new StatusNotificationResponse(
                                                                                                             Request,
                                                                                                             Result.Format(
                                                                                                                 "Invalid SOAP => " +
                                                                                                                 httpresponse.HTTPBody.ToUTF8String()
                                                                                                             )
                                                                                                         ),
                                                                                                         IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return new HTTPResponse<StatusNotificationResponse>(httpresponse,
                                                                                                         new StatusNotificationResponse(
                                                                                                             Request,
                                                                                                             Result.Server(
                                                                                                                  httpresponse.HTTPStatusCode.ToString() +
                                                                                                                  " => " +
                                                                                                                  httpresponse.HTTPBody.      ToUTF8String()
                                                                                                             )
                                                                                                         ),
                                                                                                         IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<StatusNotificationResponse>.ExceptionThrown(new StatusNotificationResponse(
                                                                                                                         Request,
                                                                                                                         Result.Format(exception.Message +
                                                                                                                                       " => " +
                                                                                                                                       exception.StackTrace)),
                                                                                                                     exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<StatusNotificationResponse>.OK(new StatusNotificationResponse(Request,
                                                                                                    Result.OK("Nothing to upload!")));


            #region Send OnStatusNotificationResponse event

            try
            {

                OnStatusNotificationResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                     this,
                                                     Request,
                                                     result.Content,
                                                     org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnStatusNotificationResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region SendMeterValues                  (Request, ...)

        /// <summary>
        /// Send a meter values for the given connector.
        /// </summary>
        /// <param name="Request">A meter values request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<MeterValuesResponse>>

            SendMeterValues(MeterValuesRequest  Request,

                            DateTime?           Timestamp          = null,
                            CancellationToken?  CancellationToken  = null,
                            EventTracking_Id    EventTrackingId    = null,
                            TimeSpan?           RequestTimeout     = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given meter values Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<MeterValuesResponse> result = null;

            #endregion

            #region Send OnMeterValuesRequest event

            try
            {

                OnMeterValuesRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                             this,
                                             Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnMeterValuesRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                    VirtualHostname,
                                                    true,
                                                    null,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    ClientCert,
                                                    HTTPUserAgent,
                                                    URLPathPrefix,
                                                    WSSLoginPassword,
                                                    RequestTimeout,
                                                    TransmissionRetryDelay,
                                                    MaxNumberOfRetries,
                                                    UseHTTPPipelining,
                                                    HTTPLogger,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/MeterValues",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "MeterValues",
                                                 RequestLogDelegate:   OnMeterValuesSOAPRequest,
                                                 ResponseLogDelegate:  OnMeterValuesSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                                                      MeterValuesResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<MeterValuesResponse>(httpresponse,
                                                                                                  new MeterValuesResponse(
                                                                                                      Request,
                                                                                                      Result.Format(
                                                                                                          "Invalid SOAP => " +
                                                                                                          httpresponse.HTTPBody.ToUTF8String()
                                                                                                      )
                                                                                                  ),
                                                                                                  IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return new HTTPResponse<MeterValuesResponse>(httpresponse,
                                                                                                  new MeterValuesResponse(
                                                                                                      Request,
                                                                                                      Result.Server(
                                                                                                           httpresponse.HTTPStatusCode.ToString() +
                                                                                                           " => " +
                                                                                                           httpresponse.HTTPBody.      ToUTF8String()
                                                                                                      )
                                                                                                  ),
                                                                                                  IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<MeterValuesResponse>.ExceptionThrown(new MeterValuesResponse(
                                                                                                                  Request,
                                                                                                                  Result.Format(exception.Message +
                                                                                                                                " => " +
                                                                                                                                exception.StackTrace)),
                                                                                                              exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<MeterValuesResponse>.OK(new MeterValuesResponse(Request,
                                                                                      Result.OK("Nothing to upload!")));


            #region Send OnMeterValuesResponse event

            try
            {

                OnMeterValuesResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                              this,
                                              Request,
                                              result.Content,
                                              org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnMeterValuesResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region StopTransaction                  (Request, ...)

        /// <summary>
        /// Stop a charging process at the given connector.
        /// </summary>
        /// <param name="Request">A stop transaction request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<StopTransactionResponse>>

            StopTransaction(StopTransactionRequest  Request,

                            DateTime?               Timestamp          = null,
                            CancellationToken?      CancellationToken  = null,
                            EventTracking_Id        EventTrackingId    = null,
                            TimeSpan?               RequestTimeout     = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given stop transaction Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<StopTransactionResponse> result = null;

            #endregion

            #region Send OnStopTransactionRequest event

            try
            {

                OnStopTransactionRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                 this,
                                                 Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnStopTransactionRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                    VirtualHostname,
                                                    true,
                                                    null,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    ClientCert,
                                                    HTTPUserAgent,
                                                    URLPathPrefix,
                                                    WSSLoginPassword,
                                                    RequestTimeout,
                                                    TransmissionRetryDelay,
                                                    MaxNumberOfRetries,
                                                    UseHTTPPipelining,
                                                    HTTPLogger,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/StopTransaction",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "StopTransaction",
                                                 RequestLogDelegate:   OnStopTransactionSOAPRequest,
                                                 ResponseLogDelegate:  OnStopTransactionSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request, StopTransactionResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<StopTransactionResponse>(httpresponse,
                                                                                                      new StopTransactionResponse(
                                                                                                          Request,
                                                                                                          Result.Format(
                                                                                                              "Invalid SOAP => " +
                                                                                                              httpresponse.HTTPBody.ToUTF8String()
                                                                                                          )
                                                                                                      ),
                                                                                                      IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return new HTTPResponse<StopTransactionResponse>(httpresponse,
                                                                                                      new StopTransactionResponse(
                                                                                                          Request,
                                                                                                          Result.Server(
                                                                                                               httpresponse.HTTPStatusCode.ToString() +
                                                                                                               " => " +
                                                                                                               httpresponse.HTTPBody.      ToUTF8String()
                                                                                                          )
                                                                                                      ),
                                                                                                      IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<StopTransactionResponse>.ExceptionThrown(new StopTransactionResponse(
                                                                                                                      Request,
                                                                                                                      Result.Format(exception.Message +
                                                                                                                                    " => " +
                                                                                                                                    exception.StackTrace)),
                                                                                                                  exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<StopTransactionResponse>.OK(new StopTransactionResponse(Request,
                                                                                              Result.OK("Nothing to upload!")));


            #region Send OnStopTransactionResponse event

            try
            {

                OnStopTransactionResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                  this,
                                                  Request,
                                                  result.Content,
                                                  org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnStopTransactionResponse));
            }

            #endregion

            return result;

        }

        #endregion


        #region TransferData                     (Request, ...)

        /// <summary>
        /// Send the given vendor-specific data to the central system.
        /// </summary>
        /// <param name="Request">A data transfer request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<CS.DataTransferResponse>>

            TransferData(DataTransferRequest  Request,

                         DateTime?            Timestamp          = null,
                         CancellationToken?   CancellationToken  = null,
                         EventTracking_Id     EventTrackingId    = null,
                         TimeSpan?            RequestTimeout     = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given data transfer Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<CS.DataTransferResponse> result = null;

            #endregion

            #region Send OnDataTransferRequest event

            try
            {

                OnDataTransferRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                              this,
                                              Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                    VirtualHostname,
                                                    true,
                                                    null,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    ClientCert,
                                                    HTTPUserAgent,
                                                    URLPathPrefix,
                                                    WSSLoginPassword,
                                                    RequestTimeout,
                                                    TransmissionRetryDelay,
                                                    MaxNumberOfRetries,
                                                    UseHTTPPipelining,
                                                    HTTPLogger,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/DataTransfer",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "DataTransfer",
                                                 RequestLogDelegate:   OnDataTransferSOAPRequest,
                                                 ResponseLogDelegate:  OnDataTransferSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                                                      CS.DataTransferResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<CS.DataTransferResponse>(httpresponse,
                                                                                                      new CS.DataTransferResponse(
                                                                                                          Request,
                                                                                                          Result.Format(
                                                                                                              "Invalid SOAP => " +
                                                                                                              httpresponse.HTTPBody.ToUTF8String()
                                                                                                          )
                                                                                                      ),
                                                                                                      IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return new HTTPResponse<CS.DataTransferResponse>(httpresponse,
                                                                                                      new CS.DataTransferResponse(
                                                                                                          Request,
                                                                                                          Result.Server(
                                                                                                               httpresponse.HTTPStatusCode.ToString() +
                                                                                                               " => " +
                                                                                                               httpresponse.HTTPBody.      ToUTF8String()
                                                                                                          )
                                                                                                      ),
                                                                                                      IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<CS.DataTransferResponse>.ExceptionThrown(new CS.DataTransferResponse(
                                                                                                                      Request,
                                                                                                                      Result.Format(exception.Message +
                                                                                                                                    " => " +
                                                                                                                                    exception.StackTrace)),
                                                                                                                  exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<CS.DataTransferResponse>.OK(new CS.DataTransferResponse(Request,
                                                                                              Result.OK("Nothing to upload!")));


            #region Send OnDataTransferResponse event

            try
            {

                OnDataTransferResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                               this,
                                               Request,
                                               result.Content,
                                               org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region SendDiagnosticsStatusNotification(Request, ...)

        /// <summary>
        /// Send a diagnostics status notification to the central system.
        /// </summary>
        /// <param name="Request">A diagnostics status notification request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<DiagnosticsStatusNotificationResponse>>

            SendDiagnosticsStatusNotification(DiagnosticsStatusNotificationRequest  Request,

                                              DateTime?                             Timestamp          = null,
                                              CancellationToken?                    CancellationToken  = null,
                                              EventTracking_Id                      EventTrackingId    = null,
                                              TimeSpan?                             RequestTimeout     = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given diagnostics status notification Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<DiagnosticsStatusNotificationResponse> result = null;

            #endregion

            #region Send OnDiagnosticsStatusNotificationRequest event

            try
            {

                OnDiagnosticsStatusNotificationRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                               this,
                                                               Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnDiagnosticsStatusNotificationRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                    VirtualHostname,
                                                    true,
                                                    null,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    ClientCert,
                                                    HTTPUserAgent,
                                                    URLPathPrefix,
                                                    WSSLoginPassword,
                                                    RequestTimeout,
                                                    TransmissionRetryDelay,
                                                    MaxNumberOfRetries,
                                                    UseHTTPPipelining,
                                                    HTTPLogger,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/DiagnosticsStatusNotification",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "DiagnosticsStatusNotification",
                                                 RequestLogDelegate:   OnDiagnosticsStatusNotificationSOAPRequest,
                                                 ResponseLogDelegate:  OnDiagnosticsStatusNotificationSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                                                      DiagnosticsStatusNotificationResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<DiagnosticsStatusNotificationResponse>(httpresponse,
                                                                                                                    new DiagnosticsStatusNotificationResponse(
                                                                                                                        Request,
                                                                                                                        Result.Format(
                                                                                                                            "Invalid SOAP => " +
                                                                                                                            httpresponse.HTTPBody.ToUTF8String()
                                                                                                                        )
                                                                                                                    ),
                                                                                                                    IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return new HTTPResponse<DiagnosticsStatusNotificationResponse>(httpresponse,
                                                                                                                    new DiagnosticsStatusNotificationResponse(
                                                                                                                        Request,
                                                                                                                        Result.Server(
                                                                                                                             httpresponse.HTTPStatusCode.ToString() +
                                                                                                                             " => " +
                                                                                                                             httpresponse.HTTPBody.      ToUTF8String()
                                                                                                                        )
                                                                                                                    ),
                                                                                                                    IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<DiagnosticsStatusNotificationResponse>.ExceptionThrown(new DiagnosticsStatusNotificationResponse(
                                                                                                                                    Request,
                                                                                                                                    Result.Format(exception.Message +
                                                                                                                                                  " => " +
                                                                                                                                                  exception.StackTrace)),
                                                                                                                                exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<DiagnosticsStatusNotificationResponse>.OK(new DiagnosticsStatusNotificationResponse(Request,
                                                                                                                          Result.OK("Nothing to upload!")));


            #region Send OnDiagnosticsStatusNotificationResponse event

            try
            {

                OnDiagnosticsStatusNotificationResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                                this,
                                                                Request,
                                                                result.Content,
                                                                org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnDiagnosticsStatusNotificationResponse));
            }

            #endregion

            return result;

        }

        #endregion

        #region SendFirmwareStatusNotification   (Request, ...)

        /// <summary>
        /// Send a firmware status notification to the central system.
        /// </summary>
        /// <param name="Request">A firmware status notification request.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<HTTPResponse<FirmwareStatusNotificationResponse>>

            SendFirmwareStatusNotification(FirmwareStatusNotificationRequest  Request,

                                           DateTime?                          Timestamp          = null,
                                           CancellationToken?                 CancellationToken  = null,
                                           EventTracking_Id                   EventTrackingId    = null,
                                           TimeSpan?                          RequestTimeout     = null)

        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request), "The given firmware status notification Request must not be null!");


            if (!Timestamp.HasValue)
                Timestamp = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            if (!CancellationToken.HasValue)
                CancellationToken = new CancellationTokenSource().Token;

            if (EventTrackingId == null)
                EventTrackingId = EventTracking_Id.New;

            if (!RequestTimeout.HasValue)
                RequestTimeout = this.RequestTimeout;


            HTTPResponse<FirmwareStatusNotificationResponse> result = null;

            #endregion

            #region Send OnFirmwareStatusNotificationRequest event

            try
            {

                OnFirmwareStatusNotificationRequest?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                            this,
                                                            Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnFirmwareStatusNotificationRequest));
            }

            #endregion


            using (var _OCPPClient = new SOAPClient(RemoteURL,
                                                    VirtualHostname,
                                                    true,
                                                    null,
                                                    RemoteCertificateValidator,
                                                    ClientCertificateSelector,
                                                    ClientCert,
                                                    HTTPUserAgent,
                                                    URLPathPrefix,
                                                    WSSLoginPassword,
                                                    RequestTimeout,
                                                    TransmissionRetryDelay,
                                                    MaxNumberOfRetries,
                                                    UseHTTPPipelining,
                                                    HTTPLogger,
                                                    DNSClient))
            {

                result = await _OCPPClient.Query(SOAP.Encapsulation(Request.ChargeBoxId,
                                                                    "/FirmwareStatusNotification",
                                                                    null,
                                                                    From,
                                                                    To,
                                                                    Request.ToXML()),
                                                 "FirmwareStatusNotification",
                                                 RequestLogDelegate:   OnFirmwareStatusNotificationSOAPRequest,
                                                 ResponseLogDelegate:  OnFirmwareStatusNotificationSOAPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout,

                                                 #region OnSuccess

                                                 OnSuccess: XMLResponse => XMLResponse.ConvertContent(Request,
                                                                                                      FirmwareStatusNotificationResponse.Parse),

                                                 #endregion

                                                 #region OnSOAPFault

                                                 OnSOAPFault: (timestamp, soapclient, httpresponse) => {

                                                     SendSOAPError(timestamp, this, httpresponse.Content);

                                                     return new HTTPResponse<FirmwareStatusNotificationResponse>(httpresponse,
                                                                                                                 new FirmwareStatusNotificationResponse(
                                                                                                                     Request,
                                                                                                                     Result.Format(
                                                                                                                         "Invalid SOAP => " +
                                                                                                                         httpresponse.HTTPBody.ToUTF8String()
                                                                                                                     )
                                                                                                                 ),
                                                                                                                 IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnHTTPError

                                                 OnHTTPError: (timestamp, soapclient, httpresponse) => {

                                                     SendHTTPError(timestamp, this, httpresponse);

                                                     return new HTTPResponse<FirmwareStatusNotificationResponse>(httpresponse,
                                                                                                                 new FirmwareStatusNotificationResponse(
                                                                                                                     Request,
                                                                                                                     Result.Server(
                                                                                                                          httpresponse.HTTPStatusCode.ToString() +
                                                                                                                          " => " +
                                                                                                                          httpresponse.HTTPBody.      ToUTF8String()
                                                                                                                     )
                                                                                                                 ),
                                                                                                                 IsFault: true);

                                                 },

                                                 #endregion

                                                 #region OnException

                                                 OnException: (timestamp, sender, exception) => {

                                                     SendException(timestamp, sender, exception);

                                                     return HTTPResponse<FirmwareStatusNotificationResponse>.ExceptionThrown(new FirmwareStatusNotificationResponse(
                                                                                                                                 Request,
                                                                                                                                 Result.Format(exception.Message +
                                                                                                                                               " => " +
                                                                                                                                               exception.StackTrace)),
                                                                                                                             exception);

                                                 }

                                                 #endregion

                                                );

            }

            if (result == null)
                result = HTTPResponse<FirmwareStatusNotificationResponse>.OK(new FirmwareStatusNotificationResponse(Request,
                                                                                                                    Result.OK("Nothing to upload!")));


            #region Send OnFirmwareStatusNotificationResponse event

            try
            {

                OnFirmwareStatusNotificationResponse?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                             this,
                                                             Request,
                                                             result.Content,
                                                             org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - Timestamp.Value);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargePointWSClient) + "." + nameof(OnFirmwareStatusNotificationResponse));
            }

            #endregion

            return result;

        }

        #endregion




        #region Close()

        public void Close()
        {

            try
            {
                if (HTTPStream != null)
                {
                    HTTPStream.Close();
                    HTTPStream.Dispose();
                }
            }
            catch (Exception)
            { }

            try
            {
                if (TLSStream != null)
                {
                    TLSStream.Close();
                    TLSStream.Dispose();
                }
            }
            catch (Exception)
            { }

            try
            {
                if (TCPStream != null)
                {
                    TCPStream.Close();
                    TCPStream.Dispose();
                }
            }
            catch (Exception)
            { }

            try
            {
                if (TCPSocket != null)
                {
                    TCPSocket.Close();
                    //TCPClient.Dispose();
                }
            }
            catch (Exception)
            { }

        }

        #endregion

        #region Dispose()

        /// <summary>
        /// Dispose this object.
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #endregion

    }

}
