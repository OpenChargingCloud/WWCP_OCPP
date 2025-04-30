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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using nts = org.GraphDefined.Vanaheimr.Norn.NTS;

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// Charging Station Management System test defaults.
    /// </summary>
    public abstract class ACSMSTests
    {

        #region Data

        protected DNSClient?                    dnsClient;

        #endregion

        #region CSMS #1 Data

        protected TestCSMSNode?                 testCSMS1;
        protected OCPPWebSocketServer?          testBackendWebSockets1;

        protected List<LogJSONRequest>?         csms1WebSocketJSONRequestsSent;
        protected List<LogJSONRequest>?         csms1WebSocketJSONResponsesSent;
        protected List<LogJSONRequest>?         csms1WebSocketJSONRequestErrorsSent;
        protected List<LogJSONRequest>?         csms1WebSocketJSONResponseErrorsSent;
        protected List<LogJSONRequest>?         csms1WebSocketJSONSendMessagesSent;

        protected List<LogJSONRequest>?         csms1WebSocketJSONRequestsReceived;
        protected List<LogJSONRequest>?         csms1WebSocketJSONResponsesReceived;
        protected List<LogJSONRequest>?         csms1WebSocketJSONRequestErrorsReceived;
        protected List<LogJSONRequest>?         csms1WebSocketJSONResponseErrorsReceived;
        protected List<LogJSONRequest>?         csms1WebSocketJSONSendMessagesReceived;

        protected List<LogBinaryRequest>?       csms1WebSocketBinaryRequestsSent;
        protected List<LogBinaryRequest>?       csms1WebSocketBinaryResponsesSent;
        protected List<LogBinaryRequest>?       csms1WebSocketBinaryRequestErrorsSent;
        protected List<LogBinaryRequest>?       csms1WebSocketBinaryResponseErrorsSent;
        protected List<LogBinaryRequest>?       csms1WebSocketBinarySendMessagesSent;

        protected List<LogBinaryRequest>?       csms1WebSocketBinaryRequestsReceived;
        protected List<LogBinaryRequest>?       csms1WebSocketBinaryResponsesReceived;
        protected List<LogBinaryRequest>?       csms1WebSocketBinaryRequestErrorsReceived;
        protected List<LogBinaryRequest>?       csms1WebSocketBinaryResponseErrorsReceived;
        protected List<LogBinaryRequest>?       csms1WebSocketBinarySendMessagesReceived;

        #endregion

        #region CSMS #2 Data

        protected TestCSMSNode?                 testCSMS2;
        protected OCPPWebSocketServer?          testBackendWebSockets2;

        protected List<LogJSONRequest>?         csms2WebSocketJSONRequestsSent;
        protected List<LogJSONRequest>?         csms2WebSocketJSONResponsesSent;
        protected List<LogJSONRequest>?         csms2WebSocketJSONRequestErrorsSent;
        protected List<LogJSONRequest>?         csms2WebSocketJSONResponseErrorsSent;
        protected List<LogJSONRequest>?         csms2WebSocketJSONSendMessagesSent;

        protected List<LogJSONRequest>?         csms2WebSocketJSONRequestsReceived;
        protected List<LogJSONRequest>?         csms2WebSocketJSONResponsesReceived;
        protected List<LogJSONRequest>?         csms2WebSocketJSONRequestErrorsReceived;
        protected List<LogJSONRequest>?         csms2WebSocketJSONResponseErrorsReceived;
        protected List<LogJSONRequest>?         csms2WebSocketJSONSendMessagesReceived;

        protected List<LogBinaryRequest>?       csms2WebSocketBinaryRequestsSent;
        protected List<LogBinaryRequest>?       csms2WebSocketBinaryResponsesSent;
        protected List<LogBinaryRequest>?       csms2WebSocketBinaryRequestErrorsSent;
        protected List<LogBinaryRequest>?       csms2WebSocketBinaryResponseErrorsSent;
        protected List<LogBinaryRequest>?       csms2WebSocketBinarySendMessagesSent;

        protected List<LogBinaryRequest>?       csms2WebSocketBinaryRequestsReceived;
        protected List<LogBinaryRequest>?       csms2WebSocketBinaryResponsesReceived;
        protected List<LogBinaryRequest>?       csms2WebSocketBinaryRequestErrorsReceived;
        protected List<LogBinaryRequest>?       csms2WebSocketBinaryResponseErrorsReceived;
        protected List<LogBinaryRequest>?       csms2WebSocketBinarySendMessagesReceived;

        #endregion

        #region CSMS #3 Data

        protected TestCSMSNode?                 testCSMS3;
        protected OCPPWebSocketServer?          testBackendWebSockets3;

        protected List<LogJSONRequest>?         csms3WebSocketJSONRequestsSent;
        protected List<LogJSONRequest>?         csms3WebSocketJSONResponsesSent;
        protected List<LogJSONRequest>?         csms3WebSocketJSONRequestErrorsSent;
        protected List<LogJSONRequest>?         csms3WebSocketJSONResponseErrorsSent;
        protected List<LogJSONRequest>?         csms3WebSocketJSONSendMessagesSent;

        protected List<LogJSONRequest>?         csms3WebSocketJSONRequestsReceived;
        protected List<LogJSONRequest>?         csms3WebSocketJSONResponsesReceived;
        protected List<LogJSONRequest>?         csms3WebSocketJSONRequestErrorsReceived;
        protected List<LogJSONRequest>?         csms3WebSocketJSONResponseErrorsReceived;
        protected List<LogJSONRequest>?         csms3WebSocketJSONSendMessagesReceived;

        protected List<LogBinaryRequest>?       csms3WebSocketBinaryRequestsSent;
        protected List<LogBinaryRequest>?       csms3WebSocketBinaryResponsesSent;
        protected List<LogBinaryRequest>?       csms3WebSocketBinaryRequestErrorsSent;
        protected List<LogBinaryRequest>?       csms3WebSocketBinaryResponseErrorsSent;
        protected List<LogBinaryRequest>?       csms3WebSocketBinarySendMessagesSent;

        protected List<LogBinaryRequest>?       csms3WebSocketBinaryRequestsReceived;
        protected List<LogBinaryRequest>?       csms3WebSocketBinaryResponsesReceived;
        protected List<LogBinaryRequest>?       csms3WebSocketBinaryRequestErrorsReceived;
        protected List<LogBinaryRequest>?       csms3WebSocketBinaryResponseErrorsReceived;
        protected List<LogBinaryRequest>?       csms3WebSocketBinarySendMessagesReceived;

        #endregion


        #region SetupOnce()

        [OneTimeSetUp]
        public virtual Task SetupOnce()
        {

            dnsClient = new DNSClient();

            #region CSMS #1

            csms1WebSocketJSONRequestsSent                = [];
            csms1WebSocketJSONResponsesSent               = [];
            csms1WebSocketJSONRequestErrorsSent           = [];
            csms1WebSocketJSONResponseErrorsSent          = [];
            csms1WebSocketJSONSendMessagesSent            = [];

            csms1WebSocketJSONRequestsReceived            = [];
            csms1WebSocketJSONResponsesReceived           = [];
            csms1WebSocketJSONRequestErrorsReceived       = [];
            csms1WebSocketJSONResponseErrorsReceived      = [];
            csms1WebSocketJSONSendMessagesReceived        = [];

            csms1WebSocketBinaryRequestsSent              = [];
            csms1WebSocketBinaryResponsesSent             = [];
            csms1WebSocketBinaryRequestErrorsSent         = [];
            csms1WebSocketBinaryResponseErrorsSent        = [];
            csms1WebSocketBinarySendMessagesSent          = [];

            csms1WebSocketBinaryRequestsReceived          = [];
            csms1WebSocketBinaryResponsesReceived         = [];
            csms1WebSocketBinaryRequestErrorsReceived     = [];
            csms1WebSocketBinaryResponseErrorsReceived    = [];
            csms1WebSocketBinarySendMessagesReceived      = [];


            testCSMS1                                     = new TestCSMSNode(

                                                                Id:                                      NetworkingNode_Id.Parse("OCPPTest01"),
                                                                VendorName:                              "GraphDefined",
                                                                Model:                                   "OCPP-Testing-Model1",
                                                                SerialNumber:                            null,
                                                                SoftwareVersion:                         null,
                                                                Description:                             null,
                                                                CustomData:                              null,

                                                                ClientCAKeyPair:                         null,
                                                                ClientCACertificate:                     null,

                                                                SignaturePolicy:                         null,
                                                                ForwardingSignaturePolicy:               null,

                                                                HTTPAPI:                                 null,
                                                                HTTPAPI_Disabled:                        false,
                                                                HTTPAPI_Port:                            null,
                                                                HTTPAPI_ServerName:                      null,
                                                                HTTPAPI_ServiceName:                     null,
                                                                HTTPAPI_RobotEMailAddress:               null,
                                                                HTTPAPI_RobotGPGPassphrase:              null,
                                                                HTTPAPI_EventLoggingDisabled:            true,

                                                                HTTPDownloadAPI:                         null,
                                                                HTTPDownloadAPI_Disabled:                false,
                                                                HTTPDownloadAPI_Path:                    null,
                                                                HTTPDownloadAPI_FileSystemPath:          null,

                                                                HTTPUploadAPI:                           null,
                                                                HTTPUploadAPI_Disabled:                  false,
                                                                HTTPUploadAPI_Path:                      null,
                                                                HTTPUploadAPI_FileSystemPath:            null,

                                                                WebPaymentsAPI:                          null,
                                                                WebPaymentsAPI_Disabled:                 false,
                                                                WebPaymentsAPI_Path:                     null,

                                                                WebAPI:                                  null,
                                                                WebAPI_Disabled:                         false,
                                                                WebAPI_Path:                             null,

                                                                NTSServer:                               (csmsNode) => new nts.NTSServer(
                                                                                                                           Description:   I18NString.Create("Secure Time Server"),
                                                                                                                           NTSKEPort:     IPPort.Parse(7777),
                                                                                                                           NTSPort:       IPPort.Parse(1234),
                                                                                                                           KeyPair:       nts.KeyPair.GenerateECKeys(1)
                                                                                                                       ),
                                                                NTSServer_Disabled:                      false,

                                                                AutoCreatedChargingStationsAccessType:   null,

                                                                DefaultRequestTimeout:                   null,

                                                                DisableSendHeartbeats:                   true,
                                                                SendHeartbeatsEvery:                     null,

                                                                DisableMaintenanceTasks:                 false,
                                                                MaintenanceEvery:                        null,

                                                                SMTPClient:                              null,
                                                                DNSClient:                               dnsClient

                                                            );

            Assert.That(testCSMS1,               Is.Not.Null);

            testBackendWebSockets1                        = testCSMS1.AttachWebSocketServer(

                                                                HTTPServiceName:              null,
                                                                IPAddress:                    null,
                                                                TCPPort:                      IPPort.Parse(9101),
                                                                Description:                  I18NString.Create($"{testCSMS1.Id} HTTP WebSocket Server"),

                                                                RequireAuthentication:        true,
                                                                SecWebSocketProtocols:        null,
                                                                DisableWebSocketPings:        true,
                                                                WebSocketPingEvery:           null,
                                                                SlowNetworkSimulationDelay:   null,

                                                                ServerCertificateSelector:    null,
                                                                ClientCertificateValidator:   null,
                                                                LocalCertificateSelector:     null,
                                                                AllowedTLSProtocols:          null,
                                                                ClientCertificateRequired:    null,
                                                                CheckCertificateRevocation:   null,

                                                                ServerThreadNameCreator:      null,
                                                                ServerThreadPrioritySetter:   null,
                                                                ServerThreadIsBackground:     null,
                                                                ConnectionIdBuilder:          null,
                                                                ConnectionTimeout:            null,
                                                                MaxClientConnections:         null,

                                                                AutoStart:                    true

                                                            );

            Assert.That(testBackendWebSockets1,  Is.Not.Null);


            testBackendWebSockets1.OnJSONMessageSent     += (timestamp,
                                                             webSocketServer,
                                                             webSocketConnection,
                                                             messageTimestamp,
                                                             eventTrackingId,
                                                             message,
                                                             sentStatus,
                                                             cancellationToken) => {

                switch (message[0].Value<Byte>())
                {

                    case 2: // JSON Request Message
                        csms1WebSocketJSONRequestsSent.      Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 3: // JSON Response Message
                        csms1WebSocketJSONResponsesSent.     Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 4: // JSON Request Error Message
                        csms1WebSocketJSONRequestErrorsSent. Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 5: // JSON Response Error Message
                        csms1WebSocketJSONResponseErrorsSent.Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 6: // JSON Send Message
                        csms1WebSocketJSONSendMessagesSent.  Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                }

                return Task.CompletedTask;

            };

            testBackendWebSockets1.OnJSONMessageReceived += (timestamp,
                                                             webSocketServer,
                                                             webSocketConnection,
                                                             messageTimestamp,
                                                             eventTrackingId,
                                                             sourceNodeId,
                                                             message,
                                                             cancellationToken) => {

                switch (message[0].Value<Byte>())
                {

                    case 2: // JSON Request Message
                        csms1WebSocketJSONRequestsReceived.      Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 3: // JSON Response Message
                        csms1WebSocketJSONResponsesReceived.     Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 4: // JSON Request Error Message
                        csms1WebSocketJSONRequestErrorsReceived. Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 5: // JSON Response Error Message
                        csms1WebSocketJSONResponseErrorsReceived.Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 6: // JSON Send Message
                        csms1WebSocketJSONSendMessagesReceived.  Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                }

                return Task.CompletedTask;

            };

            #endregion

            #region CSMS #2

            csms2WebSocketJSONRequestsSent                = [];
            csms2WebSocketJSONResponsesSent               = [];
            csms2WebSocketJSONRequestErrorsSent           = [];
            csms2WebSocketJSONResponseErrorsSent          = [];
            csms2WebSocketJSONSendMessagesSent            = [];

            csms2WebSocketJSONRequestsReceived            = [];
            csms2WebSocketJSONResponsesReceived           = [];
            csms2WebSocketJSONRequestErrorsReceived       = [];
            csms2WebSocketJSONResponseErrorsReceived      = [];
            csms2WebSocketJSONSendMessagesReceived        = [];

            csms2WebSocketBinaryRequestsSent              = [];
            csms2WebSocketBinaryResponsesSent             = [];
            csms2WebSocketBinaryRequestErrorsSent         = [];
            csms2WebSocketBinaryResponseErrorsSent        = [];
            csms2WebSocketBinarySendMessagesSent          = [];

            csms2WebSocketBinaryRequestsReceived          = [];
            csms2WebSocketBinaryResponsesReceived         = [];
            csms2WebSocketBinaryRequestErrorsReceived     = [];
            csms2WebSocketBinaryResponseErrorsReceived    = [];
            csms2WebSocketBinarySendMessagesReceived      = [];


            testCSMS2                                     = new TestCSMSNode(

                                                                Id:                                      NetworkingNode_Id.Parse("OCPPTest02"),
                                                                VendorName:                              "GraphDefined",
                                                                Model:                                   "OCPP-Testing-Model2",
                                                                SerialNumber:                            null,
                                                                SoftwareVersion:                         null,
                                                                Description:                             null,
                                                                CustomData:                              null,

                                                                ClientCAKeyPair:                         null,
                                                                ClientCACertificate:                     null,

                                                                SignaturePolicy:                         null,
                                                                ForwardingSignaturePolicy:               null,

                                                                HTTPAPI:                                 null,
                                                                HTTPAPI_Disabled:                        false,
                                                                HTTPAPI_Port:                            null,
                                                                HTTPAPI_ServerName:                      null,
                                                                HTTPAPI_ServiceName:                     null,
                                                                HTTPAPI_RobotEMailAddress:               null,
                                                                HTTPAPI_RobotGPGPassphrase:              null,
                                                                HTTPAPI_EventLoggingDisabled:            true,

                                                                HTTPDownloadAPI:                         null,
                                                                HTTPDownloadAPI_Disabled:                false,
                                                                HTTPDownloadAPI_Path:                    null,
                                                                HTTPDownloadAPI_FileSystemPath:          null,

                                                                HTTPUploadAPI:                           null,
                                                                HTTPUploadAPI_Disabled:                  false,
                                                                HTTPUploadAPI_Path:                      null,
                                                                HTTPUploadAPI_FileSystemPath:            null,

                                                                WebPaymentsAPI:                          null,
                                                                WebPaymentsAPI_Disabled:                 false,
                                                                WebPaymentsAPI_Path:                     null,

                                                                WebAPI:                                  null,
                                                                WebAPI_Disabled:                         false,
                                                                WebAPI_Path:                             null,

                                                                AutoCreatedChargingStationsAccessType:   null,

                                                                DefaultRequestTimeout:                   null,

                                                                DisableSendHeartbeats:                   true,
                                                                SendHeartbeatsEvery:                     null,

                                                                DisableMaintenanceTasks:                 false,
                                                                MaintenanceEvery:                        null,

                                                                SMTPClient:                              null,
                                                                DNSClient:                               dnsClient

                                                            );

            Assert.That(testCSMS2,               Is.Not.Null);

            testBackendWebSockets2                        = testCSMS2.AttachWebSocketServer(

                                                                HTTPServiceName:              null,
                                                                IPAddress:                    null,
                                                                TCPPort:                      IPPort.Parse(9201),
                                                                Description:                  I18NString.Create($"{testCSMS2.Id} HTTP WebSocket Server"),

                                                                RequireAuthentication:        true,
                                                                SecWebSocketProtocols:        null,
                                                                DisableWebSocketPings:        true,
                                                                WebSocketPingEvery:           null,
                                                                SlowNetworkSimulationDelay:   null,

                                                                ServerCertificateSelector:    null,
                                                                ClientCertificateValidator:   null,
                                                                LocalCertificateSelector:     null,
                                                                AllowedTLSProtocols:          null,
                                                                ClientCertificateRequired:    null,
                                                                CheckCertificateRevocation:   null,

                                                                ServerThreadNameCreator:      null,
                                                                ServerThreadPrioritySetter:   null,
                                                                ServerThreadIsBackground:     null,
                                                                ConnectionIdBuilder:          null,
                                                                ConnectionTimeout:            null,
                                                                MaxClientConnections:         null,

                                                                AutoStart:                    true

                                                            );

            Assert.That(testBackendWebSockets2,  Is.Not.Null);


            testBackendWebSockets2.OnJSONMessageSent     += (timestamp,
                                                             webSocketServer,
                                                             webSocketConnection,
                                                             messageTimestamp,
                                                             eventTrackingId,
                                                             message,
                                                             sentStatus,
                                                             cancellationToken) => {

                switch (message[0].Value<Byte>())
                {

                    case 2: // JSON Request Message
                        csms2WebSocketJSONRequestsSent.      Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 3: // JSON Response Message
                        csms2WebSocketJSONResponsesSent.     Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 4: // JSON Request Error Message
                        csms2WebSocketJSONRequestErrorsSent. Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 5: // JSON Response Error Message
                        csms2WebSocketJSONResponseErrorsSent.Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 6: // JSON Send Message
                        csms2WebSocketJSONSendMessagesSent.  Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                }

                return Task.CompletedTask;

            };

            testBackendWebSockets2.OnJSONMessageReceived += (timestamp,
                                                             webSocketServer,
                                                             webSocketConnection,
                                                             messageTimestamp,
                                                             eventTrackingId,
                                                             sourceNodeId,
                                                             message,
                                                             cancellationToken) => {

                switch (message[0].Value<Byte>())
                {

                    case 2: // JSON Request Message
                        csms2WebSocketJSONRequestsReceived.      Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 3: // JSON Response Message
                        csms2WebSocketJSONResponsesReceived.     Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 4: // JSON Request Error Message
                        csms2WebSocketJSONRequestErrorsReceived. Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 5: // JSON Response Error Message
                        csms2WebSocketJSONResponseErrorsReceived.Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 6: // JSON Send Message
                        csms2WebSocketJSONSendMessagesReceived.  Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                }

                return Task.CompletedTask;

            };

            #endregion

            #region CSMS #3

            csms3WebSocketJSONRequestsSent                = [];
            csms3WebSocketJSONResponsesSent               = [];
            csms3WebSocketJSONRequestErrorsSent           = [];
            csms3WebSocketJSONResponseErrorsSent          = [];
            csms3WebSocketJSONSendMessagesSent            = [];

            csms3WebSocketJSONRequestsReceived            = [];
            csms3WebSocketJSONResponsesReceived           = [];
            csms3WebSocketJSONRequestErrorsReceived       = [];
            csms3WebSocketJSONResponseErrorsReceived      = [];
            csms3WebSocketJSONSendMessagesReceived        = [];

            csms3WebSocketBinaryRequestsSent              = [];
            csms3WebSocketBinaryResponsesSent             = [];
            csms3WebSocketBinaryRequestErrorsSent         = [];
            csms3WebSocketBinaryResponseErrorsSent        = [];
            csms3WebSocketBinarySendMessagesSent          = [];

            csms3WebSocketBinaryRequestsReceived          = [];
            csms3WebSocketBinaryResponsesReceived         = [];
            csms3WebSocketBinaryRequestErrorsReceived     = [];
            csms3WebSocketBinaryResponseErrorsReceived    = [];
            csms3WebSocketBinarySendMessagesReceived      = [];


            testCSMS3                                     = new TestCSMSNode(

                                                                Id:                                      NetworkingNode_Id.Parse("OCPPTest03"),
                                                                VendorName:                              "GraphDefined",
                                                                Model:                                   "OCPP-Testing-Model3",
                                                                SerialNumber:                            null,
                                                                SoftwareVersion:                         null,
                                                                Description:                             null,
                                                                CustomData:                              null,

                                                                ClientCAKeyPair:                         null,
                                                                ClientCACertificate:                     null,

                                                                SignaturePolicy:                         null,
                                                                ForwardingSignaturePolicy:               null,

                                                                HTTPAPI:                                 null,
                                                                HTTPAPI_Disabled:                        false,
                                                                HTTPAPI_Port:                            null,
                                                                HTTPAPI_ServerName:                      null,
                                                                HTTPAPI_ServiceName:                     null,
                                                                HTTPAPI_RobotEMailAddress:               null,
                                                                HTTPAPI_RobotGPGPassphrase:              null,
                                                                HTTPAPI_EventLoggingDisabled:            true,

                                                                HTTPDownloadAPI:                         null,
                                                                HTTPDownloadAPI_Disabled:                false,
                                                                HTTPDownloadAPI_Path:                    null,
                                                                HTTPDownloadAPI_FileSystemPath:          null,

                                                                HTTPUploadAPI:                           null,
                                                                HTTPUploadAPI_Disabled:                  false,
                                                                HTTPUploadAPI_Path:                      null,
                                                                HTTPUploadAPI_FileSystemPath:            null,

                                                                WebPaymentsAPI:                          null,
                                                                WebPaymentsAPI_Disabled:                 false,
                                                                WebPaymentsAPI_Path:                     null,

                                                                WebAPI:                                  null,
                                                                WebAPI_Disabled:                         false,
                                                                WebAPI_Path:                             null,

                                                                AutoCreatedChargingStationsAccessType:   null,

                                                                DefaultRequestTimeout:                   null,

                                                                DisableSendHeartbeats:                   true,
                                                                SendHeartbeatsEvery:                     null,

                                                                DisableMaintenanceTasks:                 false,
                                                                MaintenanceEvery:                        null,

                                                                SMTPClient:                              null,
                                                                DNSClient:                               dnsClient

                                                            );

            Assert.That(testCSMS3,               Is.Not.Null);

            testBackendWebSockets3                        = testCSMS3.AttachWebSocketServer(

                                                                HTTPServiceName:              null,
                                                                IPAddress:                    null,
                                                                TCPPort:                      IPPort.Parse(9301),
                                                                Description:                  I18NString.Create($"{testCSMS3.Id} HTTP WebSocket Server"),

                                                                RequireAuthentication:        true,
                                                                SecWebSocketProtocols:        null,
                                                                DisableWebSocketPings:        true,
                                                                WebSocketPingEvery:           null,
                                                                SlowNetworkSimulationDelay:   null,

                                                                ServerCertificateSelector:    null,
                                                                ClientCertificateValidator:   null,
                                                                LocalCertificateSelector:     null,
                                                                AllowedTLSProtocols:          null,
                                                                ClientCertificateRequired:    null,
                                                                CheckCertificateRevocation:   null,

                                                                ServerThreadNameCreator:      null,
                                                                ServerThreadPrioritySetter:   null,
                                                                ServerThreadIsBackground:     null,
                                                                ConnectionIdBuilder:          null,
                                                                ConnectionTimeout:            null,
                                                                MaxClientConnections:         null,

                                                                AutoStart:                    true

                                                            );

            Assert.That(testBackendWebSockets3,  Is.Not.Null);


            testBackendWebSockets3.OnJSONMessageSent     += (timestamp,
                                                             webSocketServer,
                                                             webSocketConnection,
                                                             messageTimestamp,
                                                             eventTrackingId,
                                                             message,
                                                             sentStatus,
                                                             cancellationToken) => {

                switch (message[0].Value<Byte>())
                {

                    case 2: // JSON Request Message
                        csms3WebSocketJSONRequestsSent.      Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 3: // JSON Response Message
                        csms3WebSocketJSONResponsesSent.     Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 4: // JSON Request Error Message
                        csms3WebSocketJSONRequestErrorsSent. Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 5: // JSON Response Error Message
                        csms3WebSocketJSONResponseErrorsSent.Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 6: // JSON Send Message
                        csms3WebSocketJSONSendMessagesSent.  Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                }

                return Task.CompletedTask;

            };

            testBackendWebSockets3.OnJSONMessageReceived += (timestamp,
                                                             webSocketServer,
                                                             webSocketConnection,
                                                             messageTimestamp,
                                                             eventTrackingId,
                                                             sourceNodeId,
                                                             message,
                                                             cancellationToken) => {

                switch (message[0].Value<Byte>())
                {

                    case 2: // JSON Request Message
                        csms3WebSocketJSONRequestsReceived.      Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 3: // JSON Response Message
                        csms3WebSocketJSONResponsesReceived.     Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 4: // JSON Request Error Message
                        csms3WebSocketJSONRequestErrorsReceived. Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 5: // JSON Response Error Message
                        csms3WebSocketJSONResponseErrorsReceived.Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                    case 6: // JSON Send Message
                        csms3WebSocketJSONSendMessagesReceived.  Add(new LogJSONRequest(messageTimestamp, message));
                        break;

                }

                return Task.CompletedTask;

            };


            #endregion

            return Task.CompletedTask;

        }

        #endregion

        #region SetupEachTest()

        [SetUp]
        public virtual Task SetupEachTest()
        {

            Timestamp.Reset();

            #region CSMS #1

            csms1WebSocketJSONRequestsSent?.            Clear();
            csms1WebSocketJSONResponsesSent?.           Clear();
            csms1WebSocketJSONRequestErrorsSent?.       Clear();
            csms1WebSocketJSONResponseErrorsSent?.      Clear();
            csms1WebSocketJSONSendMessagesSent?.        Clear();

            csms1WebSocketJSONRequestsReceived?.        Clear();
            csms1WebSocketJSONResponsesReceived?.       Clear();
            csms1WebSocketJSONRequestErrorsReceived?.   Clear();
            csms1WebSocketJSONResponseErrorsReceived?.  Clear();
            csms1WebSocketJSONSendMessagesReceived?.    Clear();

            csms1WebSocketBinaryRequestsSent?.          Clear();
            csms1WebSocketBinaryResponsesSent?.         Clear();
            csms1WebSocketBinaryRequestErrorsSent?.     Clear();
            csms1WebSocketBinaryResponseErrorsSent?.    Clear();
            csms1WebSocketBinarySendMessagesSent?.      Clear();

            csms1WebSocketBinaryRequestsReceived?.      Clear();
            csms1WebSocketBinaryResponsesReceived?.     Clear();
            csms1WebSocketBinaryRequestErrorsReceived?. Clear();
            csms1WebSocketBinaryResponseErrorsReceived?.Clear();
            csms1WebSocketBinarySendMessagesReceived?.  Clear();

            #endregion

            #region CSMS #2

            csms2WebSocketJSONRequestsSent?.            Clear();
            csms2WebSocketJSONResponsesSent?.           Clear();
            csms2WebSocketJSONRequestErrorsSent?.       Clear();
            csms2WebSocketJSONResponseErrorsSent?.      Clear();
            csms2WebSocketJSONSendMessagesSent?.        Clear();

            csms2WebSocketJSONRequestsReceived?.        Clear();
            csms2WebSocketJSONResponsesReceived?.       Clear();
            csms2WebSocketJSONRequestErrorsReceived?.   Clear();
            csms2WebSocketJSONResponseErrorsReceived?.  Clear();
            csms2WebSocketJSONSendMessagesReceived?.    Clear();

            csms2WebSocketBinaryRequestsSent?.          Clear();
            csms2WebSocketBinaryResponsesSent?.         Clear();
            csms2WebSocketBinaryRequestErrorsSent?.     Clear();
            csms2WebSocketBinaryResponseErrorsSent?.    Clear();
            csms2WebSocketBinarySendMessagesSent?.      Clear();

            csms2WebSocketBinaryRequestsReceived?.      Clear();
            csms2WebSocketBinaryResponsesReceived?.     Clear();
            csms2WebSocketBinaryRequestErrorsReceived?. Clear();
            csms2WebSocketBinaryResponseErrorsReceived?.Clear();
            csms2WebSocketBinarySendMessagesReceived?.  Clear();

            #endregion

            #region CSMS #3

            csms3WebSocketJSONRequestsSent?.            Clear();
            csms3WebSocketJSONResponsesSent?.           Clear();
            csms3WebSocketJSONRequestErrorsSent?.       Clear();
            csms3WebSocketJSONResponseErrorsSent?.      Clear();
            csms3WebSocketJSONSendMessagesSent?.        Clear();

            csms3WebSocketJSONRequestsReceived?.        Clear();
            csms3WebSocketJSONResponsesReceived?.       Clear();
            csms3WebSocketJSONRequestErrorsReceived?.   Clear();
            csms3WebSocketJSONResponseErrorsReceived?.  Clear();
            csms3WebSocketJSONSendMessagesReceived?.    Clear();

            csms3WebSocketBinaryRequestsSent?.          Clear();
            csms3WebSocketBinaryResponsesSent?.         Clear();
            csms3WebSocketBinaryRequestErrorsSent?.     Clear();
            csms3WebSocketBinaryResponseErrorsSent?.    Clear();
            csms3WebSocketBinarySendMessagesSent?.      Clear();

            csms3WebSocketBinaryRequestsReceived?.      Clear();
            csms3WebSocketBinaryResponsesReceived?.     Clear();
            csms3WebSocketBinaryRequestErrorsReceived?. Clear();
            csms3WebSocketBinaryResponseErrorsReceived?.Clear();
            csms3WebSocketBinarySendMessagesReceived?.  Clear();

            #endregion

            return Task.CompletedTask;

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public virtual Task ShutdownEachTest()
        {
            return Task.CompletedTask;
        }

        #endregion

        #region ShutdownOnce()

        [OneTimeTearDown]
        public virtual async Task ShutdownOnce()
        {

            if (testBackendWebSockets1 is not null)
                await testBackendWebSockets1.Shutdown();

            if (testBackendWebSockets2 is not null)
                await testBackendWebSockets2.Shutdown();

            if (testBackendWebSockets3 is not null)
                await testBackendWebSockets3.Shutdown();

            testCSMS1               = null;
            testCSMS2               = null;
            testCSMS3               = null;

            testBackendWebSockets1  = null;
            testBackendWebSockets2  = null;
            testBackendWebSockets3  = null;

        }

        #endregion

    }

}
