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
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using cloud.charging.open.protocols.OCPPv2_0_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1.tests
{

    /// <summary>
    /// Central system test defaults.
    /// </summary>
    public abstract class ACSMSTests
    {

        #region Data

        protected TestCSMS?        testCSMS01;
        protected CSMSWSServer?    testBackendWebSockets01;

        protected List<LogData1>?  csmsWebSocketTextMessagesReceived;
        protected List<LogData2>?  csmsWebSocketTextMessageResponsesSent;
        protected List<LogData1>?  csmsWebSocketTextMessagesSent;
        protected List<LogData2>?  csmsWebSocketTextMessageResponsesReceived;

        #endregion


        #region SetupOnce()

        [OneTimeSetUp]
        public virtual void SetupOnce()
        {

        }

        #endregion

        #region SetupEachTest()

        [SetUp]
        public virtual void SetupEachTest()
        {

            Timestamp.Reset();

            testCSMS01      = new TestCSMS(
                                  CSMSId:                  CSMS_Id.Parse("OCPPTest01"),
                                  RequireAuthentication:   true,
                                  HTTPUploadPort:          IPPort.Parse(9100),
                                  DNSClient:               new DNSClient(
                                                               SearchForIPv6DNSServers: false,
                                                               SearchForIPv4DNSServers: false
                                                           )
                              );

            ClassicAssert.IsNotNull(testCSMS01);

            testBackendWebSockets01  = testCSMS01.CreateWebSocketService(
                                           TCPPort:                 IPPort.Parse(9101),
                                           DisableWebSocketPings:   true,
                                           AutoStart:               true
                                       );

            ClassicAssert.IsNotNull(testBackendWebSockets01);


            csmsWebSocketTextMessagesReceived          = new List<LogData1>();
            csmsWebSocketTextMessageResponsesSent      = new List<LogData2>();
            csmsWebSocketTextMessagesSent              = new List<LogData1>();
            csmsWebSocketTextMessageResponsesReceived  = new List<LogData2>();

            testBackendWebSockets01.OnTextMessageReceived         += async (timestamp, webSocketServer, webSocketConnection, eventTrackingId, requestMessage, cancellationToken) => {
                csmsWebSocketTextMessagesReceived.        Add(new LogData1(timestamp, requestMessage));
            };

            testBackendWebSockets01.OnTextMessageResponseSent     += async (timestamp, webSocketServer, webSocketConnection, eventTrackingId, requestTimestamp, requestMessage, responseTimestamp, responseMessage) => {
                csmsWebSocketTextMessageResponsesSent.    Add(new LogData2(requestTimestamp, requestMessage, responseTimestamp, responseMessage ?? "-"));
            };

            testBackendWebSockets01.OnTextMessageSent             += async (timestamp, webSocketServer, webSocketConnection, eventTrackingId, requestMessage, cancellationToken) => {
                csmsWebSocketTextMessagesSent.            Add(new LogData1(timestamp, requestMessage));
            };

            testBackendWebSockets01.OnTextMessageResponseReceived += async (timestamp, webSocketServer, webSocketConnection, eventTrackingId, requestTimestamp, requestMessage, responseTimestamp, responseMessage) => {
                csmsWebSocketTextMessageResponsesReceived.Add(new LogData2(requestTimestamp, requestMessage, responseTimestamp, responseMessage ?? "-"));
            };

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public virtual void ShutdownEachTest()
        {

            testBackendWebSockets01?.Shutdown();

            testCSMS01               = null;
            testBackendWebSockets01  = null;

        }

        #endregion

        #region ShutdownOnce()

        [OneTimeTearDown]
        public virtual void ShutdownOnce()
        {

        }

        #endregion

    }

}
