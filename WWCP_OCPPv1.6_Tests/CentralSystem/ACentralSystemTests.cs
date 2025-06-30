﻿/*
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
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests
{

    /// <summary>
    /// Central system test defaults.
    /// </summary>
    public abstract class ACentralSystemTests
    {

        #region Data

        protected TestCentralSystemNode?  testCentralSystem01;
        protected OCPPWebSocketServer?    testBackendWebSockets01;

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

            testCentralSystem01      = new TestCentralSystemNode(
                                           Id:                     NetworkingNode_Id.Parse("OCPPTest01"),
                                           VendorName:             "GraphDefined",
                                           Model:                  "CSSimulator1",
                                           HTTPUploadPort:         IPPort.Parse(9100),
                                           DNSClient:              new DNSClient(
                                                                       SearchForIPv6DNSServers: false,
                                                                       SearchForIPv4DNSServers: false
                                                                   )
                                       );

            ClassicAssert.IsNotNull(testCentralSystem01);

            testBackendWebSockets01  = testCentralSystem01.AttachWebSocketServer(
                                           TCPPort:    IPPort.Parse(9101),
                                           AutoStart:  true
                                       );

            ClassicAssert.IsNotNull(testBackendWebSockets01);

            testCentralSystem01.AddOrUpdateHTTPBasicAuth(NetworkingNode_Id.Parse("test01"), "1234abcd");
            testCentralSystem01.AddOrUpdateHTTPBasicAuth(NetworkingNode_Id.Parse("GD001"),  "1234abcd");

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public virtual void ShutdownEachTest()
        {

            testBackendWebSockets01?.Shutdown();

            testCentralSystem01      = null;
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
