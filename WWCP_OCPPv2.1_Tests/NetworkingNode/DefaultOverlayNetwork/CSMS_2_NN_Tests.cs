/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Org.BouncyCastle.Security;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.NetworkingNode.OverlayNetwork.Default
{

    /// <summary>
    /// Unit tests for a charging station, a networking nodes and a CSMS connected via a (default) overlay network.
    /// This means the charging station communicates with the networking node via classical HTTP Web Socket JSON messages,
    /// and the networking node communicates with the CSMS via extended HTTP Web Socket JSON messages.
    /// 
    /// CS  --[classic JSON]->  NN  --[Overlay Network JSON]->  CSMS
    /// 
    /// All messages are digitally signed via a signature policy.
    /// </summary>
    [TestFixture]
    [NonParallelizable]
    public class CSMS_2_NN_Tests : ADefaultOverlayNetwork
    {

        // CSMS -> Networking Node

        #region CSMS_2_NN_SendReset_Test()

        /// <summary>
        /// A test for resetting a networking node via the CSMS.
        /// </summary>
        [Test]
        public async Task CSMS_2_NN_SendReset_Test()
        {

            Assert.Multiple(() => {
                Assert.That(localController,          Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (localController         is not null &&
                lcOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                //var nnResetRequestsSent             = new ConcurrentList<ResetRequest>();
                //var nnJSONMessageRequestsSent       = new ConcurrentList<OCPP_JSONRequestMessage>();
                //var csResetRequests                 = new ConcurrentList<ResetRequest>();
                //var nnJSONResponseMessagesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
                //var nnResetResponsesReceived        = new ConcurrentList<ResetResponse>();

                //networkingNode.OCPP.OUT.OnResetRequestSent             += (timestamp, sender, resetRequest) => {
                //    nnResetRequestsSent.TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.OUT.OnJSONMessageRequestSent       += (timestamp, sender, connection, jsonRequestMessage, ct) => {
                //    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                //    return Task.CompletedTask;
                //};

                //chargingStation.        OnResetRequest                 += (timestamp, sender, connection, resetRequest) => {
                //    csResetRequests.               TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnJSONMessageResponseReceived  += (timestamp, sender, connection, jsonResponseMessage, ct) => {
                //    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnResetResponseReceived        += (timestamp, sender, resetRequest, resetResponse, runtime) => {
                //    nnResetResponsesReceived.      TryAdd(resetResponse);
                //    return Task.CompletedTask;
                //};


                var resetType  = ResetType.Immediate;
                var response   = await CSMS.Reset(
                                           Destination:    SourceRouting.To(localController.Id),
                                           ResetType:      resetType
                                       );


                Assert.Multiple(() => {

                    //// Networking Node Request OUT
                    //Assert.That(nnResetRequestsSent.           Count,   Is.EqualTo(1), "The Reset request did not leave the networking node!");
                    //var nnResetRequest = nnResetRequestsSent.First();
                    //Assert.That(nnResetRequest.DestinationId,       Is.EqualTo(chargingStation.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Length,      Is.EqualTo(1));
                    //Assert.That(nnResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Request OUT
                    //Assert.That(nnJSONMessageRequestsSent.     Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Charging Station Request IN
                    //Assert.That(csResetRequests.               Count,   Is.EqualTo(1), "The Reset request did not reach the charging station!");
                    //var csResetRequest = csResetRequests.First();
                    ////Assert.That(csResetRequest.DestinationId,       Is.EqualTo(chargingStation.Id));   // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Length,      Is.EqualTo(1));                     // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Response IN
                    //Assert.That(nnJSONResponseMessagesReceived.Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Networking Node Response IN
                    //Assert.That(nnResetResponsesReceived.      Count,   Is.EqualTo(1), "The Reset response did not reach the networking node!");
                    //var nnResetResponse = nnResetResponsesReceived.First();
                    //Assert.That(nnResetResponse.Request.RequestId,      Is.EqualTo(nnResetRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                        Is.EqualTo(ResetStatus.Accepted));

                });

            }

        }

        #endregion

        #region CSMS_2_NN_TransferData_Test()

        /// <summary>
        /// A test for sending signed vendor-specific data form the CSMS to a networking node.
        /// </summary>
        [Test]
        public async Task CSMS_2_NN_TransferData_Test()
        {

            Assert.Multiple(() => {
                Assert.That(localController,          Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (localController         is not null &&
                lcOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                var csmsDataTransferRequestsSent       = new ConcurrentList<DataTransferRequest>();
                var nnJSONMessageRequestsReceived      = new ConcurrentList<OCPP_JSONRequestMessage>();
                var nnDataTransferRequestsReceived     = new ConcurrentList<DataTransferRequest>();
                var nnDataTransferResponsesSent        = new ConcurrentList<DataTransferResponse>();
                var nnJSONResponseMessagesSent         = new ConcurrentList<OCPP_JSONResponseMessage>();
                var csmsDataTransferResponsesReceived  = new ConcurrentList<DataTransferResponse>();

                CSMS.           OCPP.OUT.OnDataTransferRequestSent      += (timestamp, sender, connection, dataTransferRequest, sentMessageResult, ct) => {
                    csmsDataTransferRequestsSent.   TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnJSONRequestMessageReceived   += (timestamp, sender, connection, jsonRequestMessage, ct) => {
                    nnJSONMessageRequestsReceived.  TryAdd(jsonRequestMessage);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnDataTransferRequestReceived  += (timestamp, sender, connection, dataTransferRequest, ct) => {
                    nnDataTransferRequestsReceived. TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.OUT.OnDataTransferResponseSent     += (timestamp, sender, connection, dataTransferRequest, dataTransferResponse, runtime, sentMessageResult, ct) => {
                    nnDataTransferResponsesSent.    TryAdd(dataTransferResponse);
                    return Task.CompletedTask;
                };

                localController.OCPP.OUT.OnJSONResponseMessageSent      += (timestamp, sender, connection, jsonResponseMessage, sentMessageResult, ct) => {
                    nnJSONResponseMessagesSent.     TryAdd(jsonResponseMessage);
                    return Task.CompletedTask;
                };

                CSMS.           OCPP.IN. OnDataTransferResponseReceived += (timestamp, sender, connection, dataTransferRequest, dataTransferResponse, runtime, ct) => {
                    csmsDataTransferResponsesReceived.TryAdd(dataTransferResponse);
                    return Task.CompletedTask;
                };


                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.GraphDefined_TestMessage;
                var data       = "Hello world!";
                var response   = await CSMS.TransferData(
                                           Destination:   SourceRouting.To( localController.Id),
                                           VendorId:      vendorId,
                                           MessageId:     messageId,
                                           Data:          data
                                       );


                Assert.Multiple(() => {

                    // Charging Station Request OUT
                    Assert.That(csmsDataTransferRequestsSent.     Count,                  Is.EqualTo(1), "The DataTransfer request did not leave the CSMS!");
                    var csmsDataTransferRequest = csmsDataTransferRequestsSent.First();
                    Assert.That(csmsDataTransferRequest.DestinationId,                Is.EqualTo(localController.Id));
                    Assert.That(csmsDataTransferRequest.NetworkPath.Length,               Is.EqualTo(1));
                    Assert.That(csmsDataTransferRequest.NetworkPath.Source,               Is.EqualTo(CSMS.Id));
                    Assert.That(csmsDataTransferRequest.VendorId,                         Is.EqualTo(vendorId));
                    Assert.That(csmsDataTransferRequest.MessageId,                        Is.EqualTo(messageId));
                    Assert.That(csmsDataTransferRequest.Data?.ToString(),                 Is.EqualTo(data));

                    Assert.That(csmsDataTransferRequest.Signatures.Any(),                 Is.True, "The outgoing DataTransfer request is not signed!");


                    // Networking Node JSON Request IN
                    Assert.That(nnJSONMessageRequestsReceived.  Count,                    Is.EqualTo(1), "The DataTransfer JSON request did not reach the networking node!");
                    var nnJSONMessageRequest = nnJSONMessageRequestsReceived.First();
                    Assert.That(nnJSONMessageRequest.Destination,                   Is.EqualTo(localController.Id));
                    Assert.That(nnJSONMessageRequest.NetworkPath.Length,                  Is.EqualTo(1));
                    Assert.That(nnJSONMessageRequest.NetworkPath.Source,                  Is.EqualTo(NetworkingNode_Id.CSMS));  // Because of "standard" networking mode!
                    Assert.That(nnJSONMessageRequest.NetworkPath.Last,                    Is.EqualTo(NetworkingNode_Id.CSMS));  // Because of "standard" networking mode!


                    // Networking Node Request IN
                    Assert.That(nnDataTransferRequestsReceived. Count,                    Is.EqualTo(1), "The DataTransfer request did not reach the networking node!");
                    var nnDataTransferRequestReceived = nnDataTransferRequestsReceived.First();
                    Assert.That(nnDataTransferRequestReceived.DestinationId,          Is.EqualTo(localController.Id));
                    Assert.That(nnDataTransferRequestReceived.NetworkPath.Length,         Is.EqualTo(1));
                    Assert.That(nnDataTransferRequestReceived.NetworkPath.Source,         Is.EqualTo(NetworkingNode_Id.CSMS));  // Because of "standard" networking mode!
                    Assert.That(nnDataTransferRequestReceived.NetworkPath.Last,           Is.EqualTo(NetworkingNode_Id.CSMS));  // Because of "standard" networking mode!
                    Assert.That(nnDataTransferRequestReceived.VendorId,                   Is.EqualTo(vendorId));
                    Assert.That(nnDataTransferRequestReceived.MessageId,                  Is.EqualTo(messageId));
                    Assert.That(nnDataTransferRequestReceived.Data?.ToString(),           Is.EqualTo(data));

                    Assert.That(nnDataTransferRequestReceived.Signatures.Any(),           Is.True, "The incoming DataTransfer request is not signed!");
                    var nnDataTransferRequestSignature = nnDataTransferRequestReceived.Signatures.First();
                    Assert.That(nnDataTransferRequestSignature.Status,                    Is.EqualTo(VerificationStatus.ValidSignature));


                    // Networking Node Response OUT
                    Assert.That(nnDataTransferResponsesSent.    Count,                    Is.EqualTo(1), "The DataTransfer response did not leave the networking node!");
                    var nnDataTransferResponseSent = nnDataTransferResponsesSent.First();
                    Assert.That(nnDataTransferResponseSent.DestinationId,             Is.EqualTo(NetworkingNode_Id.CSMS));  // Because of "standard" networking mode!
                    Assert.That(nnDataTransferResponseSent.NetworkPath.Length,            Is.EqualTo(1));
                    Assert.That(nnDataTransferResponseSent.NetworkPath.Source,            Is.EqualTo(localController.Id));
                    Assert.That(nnDataTransferResponseSent.NetworkPath.Last,              Is.EqualTo(localController.Id));
                    Assert.That(nnDataTransferResponseSent.Data?.ToString(),              Is.EqualTo(data.Reverse()));

                    Assert.That(nnDataTransferResponseSent.Signatures.Any(),              Is.True, "The DataTransfer response is not signed!");


                    // Networking Node JSON Response OUT
                    Assert.That(nnJSONResponseMessagesSent.     Count,                    Is.EqualTo(1), "The DataTransfer JSON response did not leave the networking node!");


                    // Charging Station Response IN
                    Assert.That(csmsDataTransferResponsesReceived.Count,                  Is.EqualTo(1), "The DataTransfer response did not reach the CSMS!");
                    var csmsDataTransferResponse = csmsDataTransferResponsesReceived.First();
                    Assert.That(csmsDataTransferResponse.Request.RequestId,               Is.EqualTo(nnDataTransferRequestReceived.RequestId));

                    Assert.That(csmsDataTransferResponse.Signatures.Any(),                Is.True, "The incoming DataTransfer response is not signed!");
                    var csDataTransferResponseSignature = csmsDataTransferResponse.Signatures.First();
                    Assert.That(csDataTransferResponseSignature.Status,                   Is.EqualTo(VerificationStatus.ValidSignature));


                    // Result
                    Assert.That(response.Result.ResultCode,                               Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                          Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data?.ToString(),                                Is.EqualTo(data.Reverse()));


                });

            }

        }

        #endregion

        #region CSMS_2_NN_TransferSecureData_Test()

        /// <summary>
        /// A test for sending signed and encypted vendor-specific data form the CSMS to a networking node.
        /// </summary>
        [Test]
        public async Task CSMS_2_NN_TransferSecureData_Test()
        {

            Assert.Multiple(() => {
                Assert.That(localController,         Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (localController        is not null &&
                lcOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                var csmsSecureDataTransferRequestsSent       = new ConcurrentList<SecureDataTransferRequest>();
                var nnBinaryMessageRequestsReceived          = new ConcurrentList<OCPP_BinaryRequestMessage>();
                var nnSecureDataTransferRequestsReceived     = new ConcurrentList<SecureDataTransferRequest>();
                var nnSecureDataTransferResponsesSent        = new ConcurrentList<SecureDataTransferResponse>();
                var nnBinaryResponseMessagesSent             = new ConcurrentList<OCPP_BinaryResponseMessage>();
                var csmsSecureDataTransferResponsesReceived  = new ConcurrentList<SecureDataTransferResponse>();

                CSMS.           OCPP.OUT.OnSecureDataTransferRequestSent      += (timestamp, sender, connection, dataTransferRequest, sentMessageResult, ct) => {
                    csmsSecureDataTransferRequestsSent.   TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnBinaryRequestMessageReceived       += (timestamp, sender, connection, binaryRequestMessage, ct) => {
                    nnBinaryMessageRequestsReceived.  TryAdd(binaryRequestMessage);
                    return Task.CompletedTask;
                };

                localController.OCPP.IN. OnSecureDataTransferRequestReceived  += (timestamp, sender, connection, dataTransferRequest, ct) => {
                    nnSecureDataTransferRequestsReceived. TryAdd(dataTransferRequest);
                    return Task.CompletedTask;
                };

                localController.OCPP.OUT.OnSecureDataTransferResponseSent     += (timestamp, sender, connection, dataTransferRequest, dataTransferResponse, runtime, sentMessageResult, ct) => {
                    nnSecureDataTransferResponsesSent.    TryAdd(dataTransferResponse);
                    return Task.CompletedTask;
                };

                localController.OCPP.OUT.OnBinaryResponseMessageSent          += (timestamp, sender, connection, binaryResponseMessage, sentMessageResult, ct) => {
                    nnBinaryResponseMessagesSent.     TryAdd(binaryResponseMessage);
                    return Task.CompletedTask;
                };

                CSMS.           OCPP.IN. OnSecureDataTransferResponseReceived += (timestamp, sender, connection, dataTransferRequest, dataTransferResponse, runtime, ct) => {
                    csmsSecureDataTransferResponsesReceived.TryAdd(dataTransferResponse);
                    return Task.CompletedTask;
                };


                var data        = "Hello world!";
                var response    = await CSMS.TransferSecureData(
                                            Destination:    SourceRouting.To(localController.Id),
                                            Parameter:      0,
                                            KeyId:          1,
                                            Payload:        data.ToUTF8Bytes()
                                        );

                var secureData  = response.Decrypt(CSMS.GetDecryptionKey(response.NetworkPath.Source, response.KeyId)).ToUTF8String();


                Assert.Multiple(() => {

                    // Charging Station Request OUT
                    Assert.That(csmsSecureDataTransferRequestsSent.     Count,                  Is.EqualTo(1), "The SecureDataTransfer request did not leave the CSMS!");
                    var csmsSecureDataTransferRequest = csmsSecureDataTransferRequestsSent.First();
                    Assert.That(csmsSecureDataTransferRequest.DestinationId,                Is.EqualTo(localController.Id));
                    Assert.That(csmsSecureDataTransferRequest.NetworkPath.Length,               Is.EqualTo(1));
                    Assert.That(csmsSecureDataTransferRequest.NetworkPath.Source,               Is.EqualTo(CSMS.Id));
                  //  Assert.That(csmsSecureDataTransferRequest.VendorId,                         Is.EqualTo(vendorId));
                  //  Assert.That(csmsSecureDataTransferRequest.MessageId,                        Is.EqualTo(messageId));
                  //  Assert.That(csmsSecureDataTransferRequest.SecureData?.ToString(),                 Is.EqualTo(data));

                    Assert.That(csmsSecureDataTransferRequest.Signatures.Any(),                 Is.True, "The outgoing SecureDataTransfer request is not signed!");


                    // Networking Node Binary Request IN
                    Assert.That(nnBinaryMessageRequestsReceived.  Count,                    Is.EqualTo(1), "The SecureDataTransfer JSON request did not reach the networking node!");
                    var nnBinaryMessageRequest = nnBinaryMessageRequestsReceived.First();
                    Assert.That(nnBinaryMessageRequest.Destination,                         Is.EqualTo(localController.Id));
                    Assert.That(nnBinaryMessageRequest.NetworkPath.Length,                  Is.EqualTo(1));
                    Assert.That(nnBinaryMessageRequest.NetworkPath.Source,                  Is.EqualTo(NetworkingNode_Id.CSMS));  // Because of "standard" networking mode!
                    Assert.That(nnBinaryMessageRequest.NetworkPath.Last,                    Is.EqualTo(NetworkingNode_Id.CSMS));  // Because of "standard" networking mode!


                    // Networking Node Request IN
                    Assert.That(nnSecureDataTransferRequestsReceived. Count,                    Is.EqualTo(1), "The SecureDataTransfer request did not reach the networking node!");
                    var nnSecureDataTransferRequestReceived = nnSecureDataTransferRequestsReceived.First();
                    Assert.That(nnSecureDataTransferRequestReceived.DestinationId,          Is.EqualTo(localController.Id));
                    Assert.That(nnSecureDataTransferRequestReceived.NetworkPath.Length,         Is.EqualTo(1));
                    Assert.That(nnSecureDataTransferRequestReceived.NetworkPath.Source,         Is.EqualTo(NetworkingNode_Id.CSMS));  // Because of "standard" networking mode!
                    Assert.That(nnSecureDataTransferRequestReceived.NetworkPath.Last,           Is.EqualTo(NetworkingNode_Id.CSMS));  // Because of "standard" networking mode!
                //    Assert.That(nnSecureDataTransferRequestReceived.VendorId,                   Is.EqualTo(vendorId));
                //    Assert.That(nnSecureDataTransferRequestReceived.MessageId,                  Is.EqualTo(messageId));
                //    Assert.That(nnSecureDataTransferRequestReceived.SecureData?.ToString(),           Is.EqualTo(data));

                    Assert.That(nnSecureDataTransferRequestReceived.Signatures.Any(),           Is.True, "The incoming SecureDataTransfer request is not signed!");
                    var nnSecureDataTransferRequestSignature = nnSecureDataTransferRequestReceived.Signatures.First();
                    Assert.That(nnSecureDataTransferRequestSignature.Status,                    Is.EqualTo(VerificationStatus.ValidSignature));


                    // Networking Node Response OUT
                    Assert.That(nnSecureDataTransferResponsesSent.    Count,                    Is.EqualTo(1), "The SecureDataTransfer response did not leave the networking node!");
                    var nnSecureDataTransferResponseSent = nnSecureDataTransferResponsesSent.First();
                    Assert.That(nnSecureDataTransferResponseSent.DestinationId,             Is.EqualTo(NetworkingNode_Id.CSMS));  // Because of "standard" networking mode!
                    Assert.That(nnSecureDataTransferResponseSent.NetworkPath.Length,            Is.EqualTo(1));
                    Assert.That(nnSecureDataTransferResponseSent.NetworkPath.Source,            Is.EqualTo(localController.Id));
                    Assert.That(nnSecureDataTransferResponseSent.NetworkPath.Last,              Is.EqualTo(localController.Id));
                //    Assert.That(nnSecureDataTransferResponseSent.SecureData?.ToString(),              Is.EqualTo(data.Reverse()));

                    Assert.That(nnSecureDataTransferResponseSent.Signatures.Any(),              Is.True, "The SecureDataTransfer response is not signed!");


                    // Networking Node Binary Response OUT
                    Assert.That(nnBinaryResponseMessagesSent.     Count,                    Is.EqualTo(1), "The SecureDataTransfer JSON response did not leave the networking node!");


                    // Charging Station Response IN
                    Assert.That(csmsSecureDataTransferResponsesReceived.Count,                  Is.EqualTo(1), "The SecureDataTransfer response did not reach the CSMS!");
                    var csmsSecureDataTransferResponse = csmsSecureDataTransferResponsesReceived.First();
                    Assert.That(csmsSecureDataTransferResponse.Request.RequestId,               Is.EqualTo(nnSecureDataTransferRequestReceived.RequestId));

                    Assert.That(csmsSecureDataTransferResponse.Signatures.Any(),                Is.True, "The incoming SecureDataTransfer response is not signed!");
                    var csSecureDataTransferResponseSignature = csmsSecureDataTransferResponse.Signatures.First();
                    Assert.That(csSecureDataTransferResponseSignature.Status,                   Is.EqualTo(VerificationStatus.ValidSignature));


                    // Result
                    Assert.That(response.Result.ResultCode,                               Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                          Is.EqualTo(SecureDataTransferStatus.Accepted));
                    Assert.That(secureData,                                               Is.EqualTo(data.Reverse()));


                });

            }

        }

        #endregion


        #region CSMS_2_NN_SendEncryptedReset_Test1()

        /// <summary>
        /// A test for sending an encrypted reset request to a networking node.
        /// </summary>
        [Test]
        public async Task CSMS_2_NN_SendEncryptedReset_Test1()
        {

            Assert.Multiple(() => {
                Assert.That(localController,          Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (localController         is not null &&
                lcOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                //var nnResetRequestsSent             = new ConcurrentList<ResetRequest>();
                //var nnJSONMessageRequestsSent       = new ConcurrentList<OCPP_JSONRequestMessage>();
                //var csResetRequests                 = new ConcurrentList<ResetRequest>();
                //var nnJSONResponseMessagesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
                //var nnResetResponsesReceived        = new ConcurrentList<ResetResponse>();

                //networkingNode.OCPP.OUT.OnResetRequestSent             += (timestamp, sender, resetRequest) => {
                //    nnResetRequestsSent.TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.OUT.OnJSONMessageRequestSent       += (timestamp, sender, connection, jsonRequestMessage, ct) => {
                //    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                //    return Task.CompletedTask;
                //};

                //chargingStation.        OnResetRequest                 += (timestamp, sender, connection, resetRequest) => {
                //    csResetRequests.               TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnJSONMessageResponseReceived  += (timestamp, sender, connection, jsonResponseMessage, ct) => {
                //    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnResetResponseReceived        += (timestamp, sender, resetRequest, resetResponse, runtime) => {
                //    nnResetResponsesReceived.      TryAdd(resetResponse);
                //    return Task.CompletedTask;
                //};

                var reset = new ResetRequest(
                                SourceRouting.To(localController.Id),
                                ResetType.Immediate
                            ).ToJSON(
                                  CSMS.OCPP.CustomResetRequestSerializer,
                                  CSMS.OCPP.CustomSignatureSerializer,
                                  CSMS.OCPP.CustomCustomDataSerializer
                              ).ToUTF8Bytes();

                var key         = new Byte[32]; // 256-bit AES key
                new SecureRandom().NextBytes(key);

                var secureDataTransfer = SecureDataTransferRequest.Encrypt(
                                             SourceRouting.To(localController.Id),
                                             0,
                                             1,
                                             key,
                                             1,
                                             1,
                                             reset
                                         );



                var resetType  = ResetType.Immediate;
                var response   = await CSMS.Reset(
                                           Destination:  SourceRouting.To(localController.Id),
                                           ResetType:    resetType
                                       );


                Assert.Multiple(() => {

                    //// Networking Node Request OUT
                    //Assert.That(nnResetRequestsSent.           Count,   Is.EqualTo(1), "The Reset request did not leave the networking node!");
                    //var nnResetRequest = nnResetRequestsSent.First();
                    //Assert.That(nnResetRequest.DestinationId,       Is.EqualTo(chargingStation.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Length,      Is.EqualTo(1));
                    //Assert.That(nnResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Request OUT
                    //Assert.That(nnJSONMessageRequestsSent.     Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Charging Station Request IN
                    //Assert.That(csResetRequests.               Count,   Is.EqualTo(1), "The Reset request did not reach the charging station!");
                    //var csResetRequest = csResetRequests.First();
                    ////Assert.That(csResetRequest.DestinationId,       Is.EqualTo(chargingStation.Id));   // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Length,      Is.EqualTo(1));                     // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Response IN
                    //Assert.That(nnJSONResponseMessagesReceived.Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Networking Node Response IN
                    //Assert.That(nnResetResponsesReceived.      Count,   Is.EqualTo(1), "The Reset response did not reach the networking node!");
                    //var nnResetResponse = nnResetResponsesReceived.First();
                    //Assert.That(nnResetResponse.Request.RequestId,      Is.EqualTo(nnResetRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                        Is.EqualTo(ResetStatus.Accepted));

                });

            }

        }

        #endregion

        #region CSMS_2_NN_SendEncryptedReset_Test2()

        /// <summary>
        /// A test for sending an encrypted reset request to a networking node.
        /// </summary>
        [Test]
        public async Task CSMS_2_NN_SendEncryptedReset_Test2()
        {

            Assert.Multiple(() => {
                Assert.That(localController,          Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (localController         is not null &&
                lcOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                //var nnResetRequestsSent             = new ConcurrentList<ResetRequest>();
                //var nnJSONMessageRequestsSent       = new ConcurrentList<OCPP_JSONRequestMessage>();
                //var csResetRequests                 = new ConcurrentList<ResetRequest>();
                //var nnJSONResponseMessagesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
                //var nnResetResponsesReceived        = new ConcurrentList<ResetResponse>();

                //networkingNode.OCPP.OUT.OnResetRequestSent             += (timestamp, sender, resetRequest) => {
                //    nnResetRequestsSent.TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.OUT.OnJSONMessageRequestSent       += (timestamp, sender, connection, jsonRequestMessage, ct) => {
                //    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                //    return Task.CompletedTask;
                //};

                //chargingStation.        OnResetRequest                 += (timestamp, sender, connection, resetRequest) => {
                //    csResetRequests.               TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnJSONMessageResponseReceived  += (timestamp, sender, connection, jsonResponseMessage, ct) => {
                //    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnResetResponseReceived        += (timestamp, sender, resetRequest, resetResponse, runtime) => {
                //    nnResetResponsesReceived.      TryAdd(resetResponse);
                //    return Task.CompletedTask;
                //};


                var resetType  = ResetType.Immediate;
                var response   = await CSMS.Reset(
                                           Destination:    SourceRouting.To(localController.Id),
                                           ResetType:          resetType
                                       );


                Assert.Multiple(() => {

                    //// Networking Node Request OUT
                    //Assert.That(nnResetRequestsSent.           Count,   Is.EqualTo(1), "The Reset request did not leave the networking node!");
                    //var nnResetRequest = nnResetRequestsSent.First();
                    //Assert.That(nnResetRequest.DestinationId,       Is.EqualTo(chargingStation.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Length,      Is.EqualTo(1));
                    //Assert.That(nnResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Request OUT
                    //Assert.That(nnJSONMessageRequestsSent.     Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Charging Station Request IN
                    //Assert.That(csResetRequests.               Count,   Is.EqualTo(1), "The Reset request did not reach the charging station!");
                    //var csResetRequest = csResetRequests.First();
                    ////Assert.That(csResetRequest.DestinationId,       Is.EqualTo(chargingStation.Id));   // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Length,      Is.EqualTo(1));                     // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Response IN
                    //Assert.That(nnJSONResponseMessagesReceived.Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Networking Node Response IN
                    //Assert.That(nnResetResponsesReceived.      Count,   Is.EqualTo(1), "The Reset response did not reach the networking node!");
                    //var nnResetResponse = nnResetResponsesReceived.First();
                    //Assert.That(nnResetResponse.Request.RequestId,      Is.EqualTo(nnResetRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                        Is.EqualTo(ResetStatus.Accepted));

                });

            }

        }

        #endregion




        #region CSMS_2_NN_DeleteFile_Test()

        /// <summary>
        /// A test for deleteing a file from a networking node via the CSMS.
        /// </summary>
        [Test]
        public async Task CSMS_2_NN_DeleteFile_Test()
        {

            Assert.Multiple(() => {
                Assert.That(localController,         Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (localController        is not null &&
                lcOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                //var nnResetRequestsSent             = new ConcurrentList<ResetRequest>();
                //var nnJSONMessageRequestsSent       = new ConcurrentList<OCPP_JSONRequestMessage>();
                //var csResetRequests                 = new ConcurrentList<ResetRequest>();
                //var nnJSONResponseMessagesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
                //var nnResetResponsesReceived        = new ConcurrentList<ResetResponse>();

                //networkingNode.OCPP.OUT.OnResetRequestSent             += (timestamp, sender, resetRequest) => {
                //    nnResetRequestsSent.TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.OUT.OnJSONMessageRequestSent       += (timestamp, sender, connection, jsonRequestMessage, ct) => {
                //    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                //    return Task.CompletedTask;
                //};

                //chargingStation.        OnResetRequest                 += (timestamp, sender, connection, resetRequest) => {
                //    csResetRequests.               TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnJSONMessageResponseReceived  += (timestamp, sender, connection, jsonResponseMessage, ct) => {
                //    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnResetResponseReceived        += (timestamp, sender, resetRequest, resetResponse, runtime) => {
                //    nnResetResponsesReceived.      TryAdd(resetResponse);
                //    return Task.CompletedTask;
                //};


                var resetType  = ResetType.Immediate;
                var response   = await CSMS.DeleteFile(
                                           Destination:    SourceRouting.To(localController.Id),
                                           FileName:           FilePath.Parse("/test.txt")
                                       );


                Assert.Multiple(() => {

                    //// Networking Node Request OUT
                    //Assert.That(nnResetRequestsSent.           Count,   Is.EqualTo(1), "The Reset request did not leave the networking node!");
                    //var nnResetRequest = nnResetRequestsSent.First();
                    //Assert.That(nnResetRequest.DestinationId,       Is.EqualTo(chargingStation.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Length,      Is.EqualTo(1));
                    //Assert.That(nnResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Request OUT
                    //Assert.That(nnJSONMessageRequestsSent.     Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Charging Station Request IN
                    //Assert.That(csResetRequests.               Count,   Is.EqualTo(1), "The Reset request did not reach the charging station!");
                    //var csResetRequest = csResetRequests.First();
                    ////Assert.That(csResetRequest.DestinationId,       Is.EqualTo(chargingStation.Id));   // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Length,      Is.EqualTo(1));                     // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Response IN
                    //Assert.That(nnJSONResponseMessagesReceived.Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Networking Node Response IN
                    //Assert.That(nnResetResponsesReceived.      Count,   Is.EqualTo(1), "The Reset response did not reach the networking node!");
                    //var nnResetResponse = nnResetResponsesReceived.First();
                    //Assert.That(nnResetResponse.Request.RequestId,      Is.EqualTo(nnResetRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                        Is.EqualTo(DeleteFileStatus.Success));

                });

            }

        }

        #endregion

        #region CSMS_2_NN_GetFile_Test()

        /// <summary>
        /// A test for getting a file from a networking node via the CSMS.
        /// </summary>
        [Test]
        public async Task CSMS_2_NN_GetFile_Test()
        {

            Assert.Multiple(() => {
                Assert.That(localController,         Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (localController        is not null &&
                lcOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                //var nnResetRequestsSent             = new ConcurrentList<ResetRequest>();
                //var nnJSONMessageRequestsSent       = new ConcurrentList<OCPP_JSONRequestMessage>();
                //var csResetRequests                 = new ConcurrentList<ResetRequest>();
                //var nnJSONResponseMessagesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
                //var nnResetResponsesReceived        = new ConcurrentList<ResetResponse>();

                //networkingNode.OCPP.OUT.OnResetRequestSent             += (timestamp, sender, resetRequest) => {
                //    nnResetRequestsSent.TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.OUT.OnJSONMessageRequestSent       += (timestamp, sender, connection, jsonRequestMessage, ct) => {
                //    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                //    return Task.CompletedTask;
                //};

                //chargingStation.        OnResetRequest                 += (timestamp, sender, connection, resetRequest) => {
                //    csResetRequests.               TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnJSONMessageResponseReceived  += (timestamp, sender, connection, jsonResponseMessage, ct) => {
                //    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnResetResponseReceived        += (timestamp, sender, resetRequest, resetResponse, runtime) => {
                //    nnResetResponsesReceived.      TryAdd(resetResponse);
                //    return Task.CompletedTask;
                //};


                var resetType  = ResetType.Immediate;
                var response   = await CSMS.GetFile(
                                           Destination:    SourceRouting.To(localController.Id),
                                           FileName:       FilePath.Parse("/test.txt")
                                       );


                Assert.Multiple(() => {

                    //// Networking Node Request OUT
                    //Assert.That(nnResetRequestsSent.           Count,   Is.EqualTo(1), "The Reset request did not leave the networking node!");
                    //var nnResetRequest = nnResetRequestsSent.First();
                    //Assert.That(nnResetRequest.DestinationId,       Is.EqualTo(chargingStation.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Length,      Is.EqualTo(1));
                    //Assert.That(nnResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Request OUT
                    //Assert.That(nnJSONMessageRequestsSent.     Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Charging Station Request IN
                    //Assert.That(csResetRequests.               Count,   Is.EqualTo(1), "The Reset request did not reach the charging station!");
                    //var csResetRequest = csResetRequests.First();
                    ////Assert.That(csResetRequest.DestinationId,       Is.EqualTo(chargingStation.Id));   // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Length,      Is.EqualTo(1));                     // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Response IN
                    //Assert.That(nnJSONResponseMessagesReceived.Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Networking Node Response IN
                    //Assert.That(nnResetResponsesReceived.      Count,   Is.EqualTo(1), "The Reset response did not reach the networking node!");
                    //var nnResetResponse = nnResetResponsesReceived.First();
                    //Assert.That(nnResetResponse.Request.RequestId,      Is.EqualTo(nnResetRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                        Is.EqualTo(GetFileStatus.Success));

                });

            }

        }

        #endregion

        #region CSMS_2_NN_SendFile_Test()

        /// <summary>
        /// A test for sending a file from the CSMS to a networking node.
        /// </summary>
        [Test]
        public async Task CSMS_2_NN_SendFile_Test()
        {

            Assert.Multiple(() => {
                Assert.That(localController,         Is.Not.Null);
                Assert.That(lcOCPPWebSocketServer,   Is.Not.Null);
                Assert.That(CSMS,                    Is.Not.Null);
                Assert.That(csmsWSServer,            Is.Not.Null);
            });

            if (localController        is not null &&
                lcOCPPWebSocketServer  is not null &&
                CSMS                   is not null &&
                csmsWSServer           is not null)
            {

                //var nnResetRequestsSent             = new ConcurrentList<ResetRequest>();
                //var nnJSONMessageRequestsSent       = new ConcurrentList<OCPP_JSONRequestMessage>();
                //var csResetRequests                 = new ConcurrentList<ResetRequest>();
                //var nnJSONResponseMessagesReceived  = new ConcurrentList<OCPP_JSONResponseMessage>();
                //var nnResetResponsesReceived        = new ConcurrentList<ResetResponse>();

                //networkingNode.OCPP.OUT.OnResetRequestSent             += (timestamp, sender, resetRequest) => {
                //    nnResetRequestsSent.TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.OUT.OnJSONMessageRequestSent       += (timestamp, sender, connection, jsonRequestMessage, ct) => {
                //    nnJSONMessageRequestsSent.     TryAdd(jsonRequestMessage);
                //    return Task.CompletedTask;
                //};

                //chargingStation.        OnResetRequest                 += (timestamp, sender, connection, resetRequest) => {
                //    csResetRequests.               TryAdd(resetRequest);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnJSONMessageResponseReceived  += (timestamp, sender, connection, jsonResponseMessage, ct) => {
                //    nnJSONResponseMessagesReceived.TryAdd(jsonResponseMessage);
                //    return Task.CompletedTask;
                //};

                //networkingNode.OCPP.IN. OnResetResponseReceived        += (timestamp, sender, resetRequest, resetResponse, runtime) => {
                //    nnResetResponsesReceived.      TryAdd(resetResponse);
                //    return Task.CompletedTask;
                //};


                var resetType  = ResetType.Immediate;
                var response   = await CSMS.SendFile(
                                           Destination:    SourceRouting.To(   localController.Id),
                                           FileName:          FilePath.Parse("/test.txt"),
                                           FileContent:       "Hello world!".ToUTF8Bytes(),
                                           FileContentType:   ContentType.Text.Plain
                                       );


                Assert.Multiple(() => {

                    //// Networking Node Request OUT
                    //Assert.That(nnResetRequestsSent.           Count,   Is.EqualTo(1), "The Reset request did not leave the networking node!");
                    //var nnResetRequest = nnResetRequestsSent.First();
                    //Assert.That(nnResetRequest.DestinationId,       Is.EqualTo(chargingStation.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Length,      Is.EqualTo(1));
                    //Assert.That(nnResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));
                    //Assert.That(nnResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Request OUT
                    //Assert.That(nnJSONMessageRequestsSent.     Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Charging Station Request IN
                    //Assert.That(csResetRequests.               Count,   Is.EqualTo(1), "The Reset request did not reach the charging station!");
                    //var csResetRequest = csResetRequests.First();
                    ////Assert.That(csResetRequest.DestinationId,       Is.EqualTo(chargingStation.Id));   // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Length,      Is.EqualTo(1));                     // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Source,      Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    ////Assert.That(csResetRequest.NetworkPath.Last,        Is.EqualTo(networkingNode.Id));    // Because of "standard" networking mode!
                    //Assert.That(csResetRequest.ResetType,               Is.EqualTo(resetType));

                    //// Networking Node JSON Response IN
                    //Assert.That(nnJSONResponseMessagesReceived.Count,   Is.EqualTo(1), "The Reset JSON request did not leave the networking node!");

                    //// Networking Node Response IN
                    //Assert.That(nnResetResponsesReceived.      Count,   Is.EqualTo(1), "The Reset response did not reach the networking node!");
                    //var nnResetResponse = nnResetResponsesReceived.First();
                    //Assert.That(nnResetResponse.Request.RequestId,      Is.EqualTo(nnResetRequest.RequestId));


                    // Result
                    Assert.That(response.Result.ResultCode,             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                        Is.EqualTo(SendFileStatus.Success));

                });

            }

        }

        #endregion


    }

}
