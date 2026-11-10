/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// A charging station cannot read what its CSMS answered, and tells the CSMS so with
    /// a response error - over OCPP 2.1, which has one, and not over OCPP 2.0.1, which
    /// has none.
    /// </summary>
    /// <remarks>
    /// "A CALLRESULTERROR is sent back on receipt of a CALLRESULT that contains errors to
    /// notify the sender that the response could not be processed", under the id of that
    /// CALLRESULT (OCPP 2.1 Edition 2, Part 4, 4.2.3 and 4.1.4). A node of this library sent
    /// none: whoever answered never heard that its answer was of no use.
    /// </remarks>
    [TestFixture]
    public class AnAnswerTheStationCannotReadTests : ACSMSTests
    {

        #region Data

        private const String              Password = "1234abcd";

        private TestChargingStationNode?  chargingStation;

        #endregion

        #region StopTheChargingStation()

        [TearDown]
        public async Task StopTheChargingStation()
        {

            if (chargingStation is not null)
                await chargingStation.Stop();

            chargingStation = null;

        }

        #endregion


        #region (private) AStationSpeaking(Id, Subprotocol)

        /// <summary>
        /// A charging station connected to the CSMS, speaking the given OCPP version only.
        /// </summary>
        private async Task<TestChargingStationNode> AStationSpeaking(String Id,
                                                                     String Subprotocol)
        {

            chargingStation = new TestChargingStationNode(
                                  Id:                     NetworkingNode_Id.Parse(Id),
                                  VendorName:             "GraphDefined",
                                  Model:                  "CS.1",
                                  DisableSendHeartbeats:  true,
                                  HTTPAPI_Disabled:       true,
                                  WebAPI_Disabled:        true,
                                  DNSClient:              testCSMS1!.DNSClient
                              );

            testCSMS1!.AddOrUpdateHTTPBasicAuth(chargingStation.Id, Password);

            var upgrade = await chargingStation.ConnectOCPPWebSocketClient(
                                    NextHopNetworkingNodeId:  NetworkingNode_Id.CSMS,
                                    RemoteURL:                URL.Parse($"http://127.0.0.1:{testBackendWebSockets1!.IPPort}/{chargingStation.Id}"),
                                    HTTPAuthentication:       HTTPBasicAuthentication.Create(chargingStation.Id.ToString(), Password),
                                    SecWebSocketProtocols:    [ Subprotocol ],
                                    DisableWebSocketPings:    true
                                );

            Assert.That(upgrade.HTTPStatusCode, Is.EqualTo(HTTPStatusCode.SwitchingProtocols));

            return chargingStation;

        }

        #endregion

        #region (private) Listen(Heard)

        /// <summary>
        /// Every response error the CSMS hears, JSON or binary, completes the given task.
        /// </summary>
        /// <returns>What stops listening, for the test's finally.</returns>
        private Action Listen(TaskCompletionSource<(Request_Id RequestId, ResultCode ErrorCode, String? ErrorDescription, String Format)> Heard)
        {

            OCPPv2_1.NetworkingNode.OnJSONResponseErrorMessageReceivedDelegate   jsonHeard    = (timestamp, sender, connection, error, cancellationToken) => {
                Heard.TrySetResult((error.RequestId, error.ErrorCode, error.ErrorDescription, "JSON"));
                return Task.CompletedTask;
            };

            OCPPv2_1.NetworkingNode.OnBinaryResponseErrorMessageReceivedDelegate binaryHeard  = (timestamp, sender, connection, error, cancellationToken) => {
                Heard.TrySetResult((error.RequestId, error.ErrorCode, error.ErrorDescription, "Binary"));
                return Task.CompletedTask;
            };

            testCSMS1!.OCPP.IN.OnJSONResponseErrorMessageReceived   += jsonHeard;
            testCSMS1!.OCPP.IN.OnBinaryResponseErrorMessageReceived += binaryHeard;

            return () => {
                testCSMS1!.OCPP.IN.OnJSONResponseErrorMessageReceived   -= jsonHeard;
                testCSMS1!.OCPP.IN.OnBinaryResponseErrorMessageReceived -= binaryHeard;
            };

        }

        #endregion


        #region OverOCPP21_AnAnswerTheStationCannotRead_IsReportedToTheCSMS(Format)

        /// <summary>
        /// Over OCPP 2.1 the station tells the CSMS that it cannot read its answer, in the
        /// format of the answer, under the request id of the answer, with FormationViolation.
        /// </summary>
        [TestCase("JSON")]
        [TestCase("Binary")]
        public async Task OverOCPP21_AnAnswerTheStationCannotRead_IsReportedToTheCSMS(String Format)
        {

            var station    = await AStationSpeaking(Format == "JSON" ? "CS-UNREAD-01" : "CS-UNREAD-02", "ocpp2.1");
            var requestId  = Request_Id.Parse(Format == "JSON" ? "4801" : "4802");
            var heard      = new TaskCompletionSource<(Request_Id RequestId, ResultCode ErrorCode, String? ErrorDescription, String Format)>(TaskCreationOptions.RunContinuationsAsynchronously);
            var stop       = Listen(heard);

            station.OCPP.CustomDataTransferResponseParser        = (json,   response) => throw new ArgumentException("This station cannot read it!");
            station.OCPP.CustomBinaryDataTransferResponseParser  = (binary, response) => throw new ArgumentException("This station cannot read it!");

            try
            {

                var result = Format == "JSON"

                                 ? (await station.TransferData(
                                              VendorId:             Vendor_Id. GraphDefined,
                                              MessageId:            Message_Id.GraphDefined_TestMessage,
                                              Data:                 "Hello world!",
                                              RequestId:            requestId,
                                              RequestTimeout:       TimeSpan.FromSeconds(10)
                                          )).Result

                                 : (await station.TransferBinaryData(
                                              VendorId:             Vendor_Id. GraphDefined,
                                              MessageId:            Message_Id.GraphDefined_TestMessage,
                                              Data:                 "Hello world!".ToUTF8Bytes(),
                                              RequestId:            requestId,
                                              RequestTimeout:       TimeSpan.FromSeconds(10),
                                              SerializationFormat:  SerializationFormats.BinaryTextIds
                                          )).Result;

                Assert.That(result.ResultCode, Is.EqualTo(ResultCode.FormationViolation), $"The station read the answer after all: {result}");

                var arrived = await Task.WhenAny(heard.Task, Task.Delay(TimeSpan.FromSeconds(5))) == heard.Task;

                Assert.That(arrived, Is.True, "The station did not tell the CSMS that it cannot read its answer.");

                Assert.Multiple(() => {
                    Assert.That(heard.Task.Result.RequestId,         Is.EqualTo(requestId), "Not under the id of the answer.");
                    Assert.That(heard.Task.Result.ErrorCode,         Is.EqualTo(ResultCode.FormationViolation));
                    Assert.That(heard.Task.Result.ErrorDescription,  Does.StartWith("The answer cannot be read"));
                    Assert.That(heard.Task.Result.Format,            Is.EqualTo(Format), "Not in the format of the answer.");
                });

            }
            finally
            {
                stop();
            }

        }

        #endregion

        #region OverOCPP201_AnAnswerTheStationCannotRead_IsNotReported()

        /// <summary>
        /// Over OCPP 2.0.1, which has no CALLRESULTERROR, the station keeps it to itself.
        /// </summary>
        [Test]
        public async Task OverOCPP201_AnAnswerTheStationCannotRead_IsNotReported()
        {

            var station  = await AStationSpeaking("CS-UNREAD-03", "ocpp2.0.1");
            var heard    = new TaskCompletionSource<(Request_Id RequestId, ResultCode ErrorCode, String? ErrorDescription, String Format)>(TaskCreationOptions.RunContinuationsAsynchronously);
            var stop     = Listen(heard);

            station.OCPP.CustomDataTransferResponseParser = (json, response) => throw new ArgumentException("This station cannot read it!");

            try
            {

                var transfer = await station.TransferData(
                                         VendorId:        Vendor_Id. GraphDefined,
                                         MessageId:       Message_Id.GraphDefined_TestMessage,
                                         Data:            "Hello world!",
                                         RequestId:       Request_Id.Parse("4803"),
                                         RequestTimeout:  TimeSpan.FromSeconds(10)
                                     );

                Assert.That(transfer.Result.ResultCode, Is.EqualTo(ResultCode.FormationViolation), $"The station read the answer after all: {transfer.Result}");

                var arrived = await Task.WhenAny(heard.Task, Task.Delay(TimeSpan.FromSeconds(2))) == heard.Task;

                Assert.That(arrived, Is.False, $"A response error went over OCPP 2.0.1: {(arrived ? heard.Task.Result : default)}");

            }
            finally
            {
                stop();
            }

        }

        #endregion

    }

}
