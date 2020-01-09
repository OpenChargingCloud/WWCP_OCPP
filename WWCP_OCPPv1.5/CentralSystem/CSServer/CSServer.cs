/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/GraphDefined/WWCP_OCPP>
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

using System;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_5
{

    /// <summary>
    /// An OCPP v1.5 HTTP/SOAP/XML Central System Server API.
    /// </summary>
    public class CSServer : ASOAPServer
    {

        #region Data

        /// <summary>
        /// The default HTTP/SOAP/XML server name.
        /// </summary>
        public new const           String           DefaultHTTPServerName  = "GraphDefined OCPP " + Version.Number + " HTTP/SOAP/XML Central System API";

        /// <summary>
        /// The default HTTP/SOAP/XML server TCP port.
        /// </summary>
        public new static readonly IPPort           DefaultHTTPServerPort  = IPPort.Parse(2010);

        /// <summary>
        /// The default HTTP/SOAP/XML server URI prefix.
        /// </summary>
        public new static readonly HTTPPath          DefaultURIPrefix       = HTTPPath.Parse("/");

        /// <summary>
        /// The default HTTP/SOAP/XML content type.
        /// </summary>
        public new static readonly HTTPContentType  DefaultContentType     = HTTPContentType.XMLTEXT_UTF8;

        /// <summary>
        /// The default request timeout.
        /// </summary>
        public new static readonly TimeSpan         DefaultRequestTimeout  = TimeSpan.FromMinutes(1);

        #endregion

        #region Events

        #region OnBootNotification

        /// <summary>
        /// An event sent whenever a boot notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler           OnBootNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a boot notification was sent.
        /// </summary>
        public event AccessLogHandler            OnBootNotificationSOAPResponse;

        /// <summary>
        /// An event sent whenever a boot notification was received.
        /// </summary>
   //     public event OnBootNotificationDelegate  OnBootNotificationRequest;

        #endregion

        #region OnHeartbeat

        /// <summary>
        /// An event sent whenever a heartbeat SOAP request was received.
        /// </summary>
        public event RequestLogHandler    OnHeartbeatSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a heartbeat was sent.
        /// </summary>
        public event AccessLogHandler     OnHeartbeatSOAPResponse;

        /// <summary>
        /// An event sent whenever a heartbeat was received.
        /// </summary>
  //      public event OnHeartbeatDelegate  OnHeartbeatRequest;

        #endregion


        #region OnAuthorize

        /// <summary>
        /// An event sent whenever an authorize SOAP request was received.
        /// </summary>
        public event RequestLogHandler                OnAuthorizeSOAPRequest;

        /// <summary>
        /// An event sent whenever an authorize SOAP response was sent.
        /// </summary>
        public event AccessLogHandler                 OnAuthorizeSOAPResponse;

        /// <summary>
        /// An event sent whenever an authorize request was received.
        /// </summary>
        public event OnAuthorizeStartRequestDelegate  OnAuthorizeRequest;

        #endregion

        #region OnStartTransaction

        /// <summary>
        /// An event sent whenever a start transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler    OnStartTransactionSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a start transaction request was sent.
        /// </summary>
        public event AccessLogHandler     OnStartTransactionSOAPResponse;

        /// <summary>
        /// An event sent whenever a start transaction request was received.
        /// </summary>
        public event OnAuthorizeStartRequestDelegate  OnStartTransactionRequest;

        #endregion

        #region OnStatusNotification

        /// <summary>
        /// An event sent whenever a status notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler    OnStatusNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a status notification request was sent.
        /// </summary>
        public event AccessLogHandler     OnStatusNotificationSOAPResponse;

        /// <summary>
        /// An event sent whenever a status notification request was received.
        /// </summary>
        public event OnAuthorizeStartRequestDelegate  OnStatusNotificationRequest;

        #endregion

        #region OnMeterValues

        /// <summary>
        /// An event sent whenever a meter values SOAP request was received.
        /// </summary>
        public event RequestLogHandler    OnMeterValuesSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a meter values request was sent.
        /// </summary>
        public event AccessLogHandler     OnMeterValuesSOAPResponse;

        /// <summary>
        /// An event sent whenever a meter values request was received.
        /// </summary>
        public event OnAuthorizeStartRequestDelegate  OnMeterValuesRequest;

        #endregion

        #region OnStopTransaction

        /// <summary>
        /// An event sent whenever a stop transaction SOAP request was received.
        /// </summary>
        public event RequestLogHandler          OnStopTransactionSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a stop transaction request was sent.
        /// </summary>
        public event AccessLogHandler           OnStopTransactionSOAPResponse;

        /// <summary>
        /// An event sent whenever a stop transaction request was received.
        /// </summary>
   //     public event OnStopTransactionDelegate  OnStopTransactionRequest;

        #endregion


        #region OnDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer SOAP request was received.
        /// </summary>
        public event RequestLogHandler       OnDataTransferSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a data transfer request was sent.
        /// </summary>
        public event AccessLogHandler        OnDataTransferSOAPResponse;

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
 //       public event OnDataTransferDelegate  OnDataTransferRequest;

        #endregion

        #region OnDiagnosticsStatusNotification

        /// <summary>
        /// An event sent whenever a diagnostics status notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler                        OnDiagnosticsStatusNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a diagnostics status notification request was sent.
        /// </summary>
        public event AccessLogHandler                         OnDiagnosticsStatusNotificationSOAPResponse;

        /// <summary>
        /// An event sent whenever a diagnostics status notification request was received.
        /// </summary>
 //       public event OnDiagnosticsStatusNotificationDelegate  OnDiagnosticsStatusNotificationRequest;

        #endregion

        #region OnFirmwareStatusNotification

        /// <summary>
        /// An event sent whenever a firmware status notification SOAP request was received.
        /// </summary>
        public event RequestLogHandler                     OnFirmwareStatusNotificationSOAPRequest;

        /// <summary>
        /// An event sent whenever a SOAP response to a firmware status notification request was sent.
        /// </summary>
        public event AccessLogHandler                      OnFirmwareStatusNotificationSOAPResponse;

        /// <summary>
        /// An event sent whenever a firmware status notification request was received.
        /// </summary>
  //      public event OnFirmwareStatusNotificationDelegate  OnFirmwareStatusNotificationRequest;

        #endregion

        #endregion

        #region Constructor(s)

        #region CSServer(HTTPServerName, TCPPort = default, URIPrefix = default, ContentType = default, DNSClient = null, AutoStart = false)

        /// <summary>
        /// Initialize a new HTTP server for the OCPP HTTP/SOAP/XML Central System API.
        /// </summary>
        /// <param name="HTTPServerName">An optional identification string for the HTTP server.</param>
        /// <param name="TCPPort">An optional TCP port for the HTTP server.</param>
        /// <param name="URIPrefix">An optional prefix for the HTTP URIs.</param>
        /// <param name="ContentType">An optional HTTP content type to use.</param>
        /// <param name="RegisterHTTPRootService">Register HTTP root services for sending a notice to clients connecting via HTML or plain text.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        /// <param name="AutoStart">Start the server immediately.</param>
        public CSServer(String          HTTPServerName           = DefaultHTTPServerName,
                        IPPort?         TCPPort                  = null,
                        HTTPPath?        URIPrefix                = null,
                        HTTPContentType ContentType              = null,
                        Boolean         RegisterHTTPRootService  = true,
                        DNSClient       DNSClient                = null,
                        Boolean         AutoStart                = false)

            : base(HTTPServerName.IsNotNullOrEmpty() ? HTTPServerName : DefaultHTTPServerName,
                   TCPPort     ?? DefaultHTTPServerPort,
                   URIPrefix   ?? DefaultURIPrefix,
                   ContentType ?? DefaultContentType,
                   RegisterHTTPRootService,
                   DNSClient,
                   AutoStart: false)

        {

            RegisterURITemplates();

            if (AutoStart)
                Start();

        }

        #endregion

        #region CSServer(SOAPServer, URIPrefix = DefaultURIPrefix)

        /// <summary>
        /// Use the given HTTP server for the OCPP HTTP/SOAP/XML Central System API.
        /// </summary>
        /// <param name="SOAPServer">A SOAP server.</param>
        /// <param name="URIPrefix">An optional prefix for the HTTP URIs.</param>
        public CSServer(SOAPServer  SOAPServer,
                        HTTPPath?    URIPrefix = null)

            : base(SOAPServer,
                   URIPrefix ?? DefaultURIPrefix)

        {

            RegisterURITemplates();

        }

        #endregion

        #endregion


        #region RegisterURITemplates()

        /// <summary>
        /// Register all URI templates for this SOAP API.
        /// </summary>
        protected void RegisterURITemplates()
        {


        }

        #endregion


    }

}
