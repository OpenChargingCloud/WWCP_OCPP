/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using NUnit.Framework;

using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests
{

    /// <summary>
    /// Unit tests for web socket connection setups.
    /// </summary>
    [TestFixture]
    public class WebSocketConnectionSetupTests : AChargePointTests
    {

        #region Sec-WebSocket-Protocol: ocpp1.6, ocpp2.0  (

        ///// <summary>
        ///// A test for sending data to a charge point.
        ///// </summary>
        //[Test]
        //public async Task CentralSystem_DataTransfer_Rejected_Test()
        //{

        //    Assert.IsNotNull(testCentralSystem01);
        //    Assert.IsNotNull(testBackendWebSockets01);
        //    Assert.IsNotNull(chargingStation1);
        //    Assert.IsNotNull(chargingStation2);
        //    Assert.IsNotNull(chargingStation3);

        //    if (testCentralSystem01     is not null &&
        //        testBackendWebSockets01 is not null &&
        //        chargingStation1        is not null &&
        //        chargingStation2        is not null &&
        //        chargingStation3        is not null)
        //    {

        //        var dataTransferRequests = new List<DataTransferRequest>();

        //        chargingStation1.OnIncomingDataTransferRequest += async (timestamp, sender, dataTransferRequest) => {
        //            dataTransferRequests.Add(dataTransferRequest);
        //        };

        //        var vendorId   = "ACME Inc.";
        //        var messageId  = "hello";
        //        var data       = "world!";
        //        var response1  = await testCentralSystem01.DataTransfer(chargingStation1.ChargeBoxId,
        //                                                                vendorId,
        //                                                                messageId,
        //                                                                data);

        //        Assert.AreEqual(ResultCodes.OK,                 response1.Result.ResultCode);
        //        Assert.AreEqual(DataTransferStatus.Rejected,    response1.Status);

        //        Assert.AreEqual(1,                              dataTransferRequests.Count);
        //        Assert.AreEqual(chargingStation1.ChargeBoxId,   dataTransferRequests.First().ChargeBoxId);
        //        Assert.AreEqual(vendorId,                       dataTransferRequests.First().VendorId);
        //        Assert.AreEqual(messageId,                      dataTransferRequests.First().MessageId);
        //        Assert.AreEqual(data,                           dataTransferRequests.First().Data);

        //    }

        //}

        #endregion


        #region Sec-WebSocket-Protocol: ocpp2.0, ocpp1.6  (

        // Accept only v1.6

        #endregion

        #region Sec-WebSocket-Protocol: ocpp2.0 (

        // Close web socket connection

        #endregion

        #region ChargeBoxId in URL: webServices/ocpp/CP3211

        // Unknown ChargeBoxId => HTTP 404 & close web socket connection

        #endregion


    }

}
