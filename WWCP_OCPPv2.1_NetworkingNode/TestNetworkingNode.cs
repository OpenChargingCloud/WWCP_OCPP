﻿/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.NN;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A networking node for testing.
    /// </summary>
    public partial class TestNetworkingNode : ANetworkingNode
    {

        #region Constructor(s)

        /// <summary>
        /// Create a new networking node for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this networking node.</param>
        public TestNetworkingNode(NetworkingNode_Id  Id,
                                  String             VendorName,
                                  String             Model,
                                  I18NString?        Description                 = null,
                                  String?            SerialNumber                = null,
                                  String?            FirmwareVersion             = null,
                                  Modem?             Modem                       = null,

                                  SignaturePolicy?   SignaturePolicy             = null,
                                  SignaturePolicy?   ForwardingSignaturePolicy   = null,

                                  Boolean            DisableSendHeartbeats       = false,
                                  TimeSpan?          SendHeartbeatsEvery         = null,
                                  TimeSpan?          DefaultRequestTimeout       = null,

                                  Boolean            DisableMaintenanceTasks     = false,
                                  TimeSpan?          MaintenanceEvery            = null,
                                  DNSClient?         DNSClient                   = null)

            : base(Id,
                   VendorName,
                   Model,
                   Description,
                   SerialNumber,
                   FirmwareVersion,
                   Modem,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   DisableSendHeartbeats,
                   SendHeartbeatsEvery,
                   DefaultRequestTimeout,

                   DisableMaintenanceTasks,
                   MaintenanceEvery,
                   DNSClient)

        {

            //Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "HTTPSSEs"));




            // CS

            #region OnReset

            OCPP.IN.OnReset += async (timestamp,
                                      sender,
                                      connection,
                                      request,
                                      cancellationToken) => {

                OCPPv2_1.CS.ResetResponse? response = null;

                DebugX.Log($"Charging Station '{Id}': Incoming '{request.ResetType}' reset request{(request.EVSEId.HasValue ? $" for EVSE '{request.EVSEId}" : "")}'!");

                // ResetType

                // Reset entire charging station
                if (!request.EVSEId.HasValue)
                {

                    response = new OCPPv2_1.CS.ResetResponse(
                                    Request:       request,
                                    NetworkPath:   NetworkPath.From(Id),
                                    Status:        ResetStatus.Accepted,
                                    StatusInfo:    null,
                                    CustomData:    null
                                );

                }

                // Unknown EVSE
                else
                {

                    response = new OCPPv2_1.CS.ResetResponse(
                                    Request:      request,
                                    Status:       ResetStatus.Rejected,
                                    StatusInfo:   null,
                                    CustomData:   null
                                );

                }

                return response;

            };


            OCPP.FORWARD.OnResetRequest += (timestamp,
                                            sender,
                                            connection,
                                            request,
                                            cancellationToken) =>

                Task.FromResult(
                    new ForwardingDecision<OCPPv2_1.CSMS.ResetRequest, OCPPv2_1.CS.ResetResponse>(
                        request,
                        ForwardingResult.FORWARD
                    )
                );

            #endregion


            #region BinaryDataStreamsExtensions

            #region OnDeleteFile

            OCPP.IN.OnDeleteFile += (timestamp,
                                     sender,
                                     connection,
                                     request,
                                     cancellationToken) => {

                return Task.FromResult(
                           new DeleteFileResponse(
                               Request:       request,
                               NetworkPath:   NetworkPath.From(Id),
                               FileName:      request.FileName,
                               Status:        DeleteFileStatus.Success
                           )
                       );

            };


            OCPP.FORWARD.OnDeleteFileRequest += (timestamp,
                                                 sender,
                                                 connection,
                                                 request,
                                                 cancellationToken) =>

                Task.FromResult(
                    new ForwardingDecision<DeleteFileRequest, DeleteFileResponse>(
                        request,
                        ForwardingResult.FORWARD
                    )
                );

            #endregion

            #region OnGetFile

            OCPP.IN.OnGetFile += (timestamp,
                                  sender,
                                  connection,
                                  request,
                                  cancellationToken) => {

                var fileContent = "Hello world!".ToUTF8Bytes();

                return Task.FromResult(
                           new GetFileResponse(
                               Request:           request,
                               NetworkPath:       NetworkPath.From(Id),
                               FileName:          request.FileName,
                               Status:            GetFileStatus.Success,
                               FileContent:       fileContent,
                               FileContentType:   ContentType.Text.Plain,
                               FileSHA256:        SHA256.HashData(fileContent),
                               FileSHA512:        SHA512.HashData(fileContent)
                           )
                       );

            };

            #endregion

            #region OnListDirectory

            OCPP.IN.OnListDirectory += (timestamp,
                                        sender,
                                        connection,
                                        request,
                                        cancellationToken) => {

                return Task.FromResult(
                           new ListDirectoryResponse(
                               Request:            request,
                               NetworkPath:        NetworkPath.From(Id),
                               DirectoryPath:      request.DirectoryPath,
                               Status:             ListDirectoryStatus.Success,
                               DirectoryListing:   new DirectoryListing()
                           )
                       );

            };

            #endregion

            #region OnSendFile

            OCPP.IN.OnSendFile += (timestamp,
                                   sender,
                                   connection,
                                   request,
                                   cancellationToken) => {

                return Task.FromResult(
                           new SendFileResponse(
                               Request:       request,
                               NetworkPath:   NetworkPath.From(Id),
                               FileName:      request.FileName,
                               Status:        SendFileStatus.Success
                           )
                       );

            };

            #endregion

            #endregion

            // Bidirectional

            #region OnBinaryDataTransfer

            OCPP.IN.OnBinaryDataTransfer += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             cancellationToken) => {

                DebugX.Log($"Charging Station '{Id}': Incoming BinaryDataTransfer request: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data?.ToHexString() ?? "-"}!");

                // VendorId
                // MessageId
                // Data

                var responseBinaryData = request.Data;

                if (request.Data is not null)
                    responseBinaryData = request.Data.Reverse();

                return Task.FromResult(
                           request.VendorId == Vendor_Id.GraphDefined

                               ? new BinaryDataTransferResponse(
                                       Request:                request,
                                       NetworkPath:            NetworkPath.From(Id),
                                       Status:                 BinaryDataTransferStatus.Accepted,
                                       AdditionalStatusInfo:   null,
                                       Data:                   responseBinaryData
                                   )

                               : new BinaryDataTransferResponse(
                                       Request:                request,
                                       NetworkPath:            NetworkPath.From(Id),
                                       Status:                 BinaryDataTransferStatus.Rejected,
                                       AdditionalStatusInfo:   null,
                                       Data:                   responseBinaryData
                                   )
                       );

            };


            OCPP.FORWARD.OnBinaryDataTransferRequest += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         cancellationToken) =>

                Task.FromResult(
                    new ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>(
                        request,
                        ForwardingResult.FORWARD
                    )
                );

            #endregion

            #region OnDataTransfer

            OCPP.IN.OnDataTransfer += async (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             cancellationToken) => {

                // VendorId
                // MessageId
                // Data

                DebugX.Log("OnIncomingDataTransfer: " + request.VendorId  + ", " +
                                                        request.MessageId + ", " +
                                                        request.Data);


                var responseData = request.Data;

                if (request.Data is not null)
                {

                    if      (request.Data.Type == JTokenType.String)
                        responseData = request.Data.ToString().Reverse();

                    else if (request.Data.Type == JTokenType.Object) {

                        var responseObject = new JObject();

                        foreach (var property in (request.Data as JObject)!)
                        {
                            if (property.Value?.Type == JTokenType.String)
                                responseObject.Add(property.Key,
                                                    property.Value.ToString().Reverse());
                        }

                        responseData = responseObject;

                    }

                    else if (request.Data.Type == JTokenType.Array) {

                        var responseArray = new JArray();

                        foreach (var element in (request.Data as JArray)!)
                        {
                            if (element?.Type == JTokenType.String)
                                responseArray.Add(element.ToString().Reverse());
                        }

                        responseData = responseArray;

                    }

                }


                var response =  request.VendorId == Vendor_Id.GraphDefined

                                    ? new DataTransferResponse(
                                          Request:       request,
                                          NetworkPath:   NetworkPath.From(Id),
                                          Status:        DataTransferStatus.Accepted,
                                          Data:          responseData,
                                          StatusInfo:    null,
                                          CustomData:    null
                                      )

                                    : new DataTransferResponse(
                                          Request:       request,
                                          NetworkPath:   NetworkPath.From(Id),
                                          Status:        DataTransferStatus.Rejected,
                                          Data:          null,
                                          StatusInfo:    null,
                                          CustomData:    null
                                      );


                return response;

            };


            OCPP.FORWARD.OnDataTransferRequest += (timestamp,
                                                   sender,
                                                   connection,
                                                   request,
                                                   cancellationToken) => {

                if (request.Data?.ToString() == "Please REJECT!")
                {

                    var response = new DataTransferResponse(
                                       request,
                                       Result.Filtered("This message is not allowed!")
                                   );

                    return Task.FromResult(
                               new ForwardingDecision<DataTransferRequest, DataTransferResponse>(
                                   request,
                                   ForwardingResult.REJECT,
                                   response,
                                   response.ToJSON(
                                       OCPP.CustomDataTransferResponseSerializer,
                                       OCPP.CustomStatusInfoSerializer,
                                       OCPP.CustomSignatureSerializer,
                                       OCPP.CustomCustomDataSerializer
                                   ),
                                   "The message was REJECTED!"
                               )
                           );

                }

                else
                    return Task.FromResult(
                               new ForwardingDecision<DataTransferRequest, DataTransferResponse>(
                                   request,
                                   ForwardingResult.FORWARD
                               )
                           );

            };

            #endregion

            #region OnSecureDataTransfer

            OCPP.IN.OnSecureDataTransfer += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             cancellationToken) => {

                DebugX.Log($"Charging Station '{Id}': Incoming SecureDataTransfer request!");

                // VendorId
                // MessageId
                // Data

                var responseSecureData = request.Ciphertext;

                var xxx = request.Decrypt();


                //if (request.Data is not null)
                //    responseSecureData = request.Data.Reverse();

                return Task.FromResult(
                           request.Ciphertext is not null

                               ? new SecureDataTransferResponse(
                                       Request:                request,
                                       NetworkPath:            NetworkPath.From(Id),
                                       Status:                 SecureDataTransferStatus.Accepted,
                                       AdditionalStatusInfo:   null,
                                       Ciphertext:             responseSecureData
                                   )

                               : new SecureDataTransferResponse(
                                       Request:                request,
                                       NetworkPath:            NetworkPath.From(Id),
                                       Status:                 SecureDataTransferStatus.Rejected,
                                       AdditionalStatusInfo:   null,
                                       Ciphertext:             responseSecureData
                                   )
                       );

            };


            //OCPP.FORWARD.OnSecureDataTransferRequest += (timestamp,
            //                                             sender,
            //                                             connection,
            //                                             request,
            //                                             cancellationToken) =>

            //    Task.FromResult(
            //        new ForwardingDecision<SecureDataTransferRequest, SecureDataTransferResponse>(
            //            request,
            //            ForwardingResult.FORWARD
            //        )
            //    );

            #endregion


            // CSMS

            #region OnBinaryDataTransfer

            OCPP.IN.OnBootNotification += (timestamp,
                                           sender,
                                           connection,
                                           request,
                                           cancellationToken) => {

                DebugX.Log($"'{Id}': Incoming BootNotification request: {request.ChargingStation.SerialNumber}, {request.Reason}");

                // ChargingStation
                // Reason

                return Task.FromResult(
                           new BootNotificationResponse(
                               Request:       request,
                               NetworkPath:   NetworkPath.From(Id),
                               Status:        RegistrationStatus.Accepted,
                               CurrentTime:   Timestamp.Now,
                               Interval:      TimeSpan.FromMinutes(5)
                           )
                       );

            };


            OCPP.FORWARD.OnBinaryDataTransferRequest += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         cancellationToken) =>

                Task.FromResult(
                    new ForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>(
                        request,
                        ForwardingResult.FORWARD
                    )
                );

            #endregion

            OCPP.FORWARD.OnBootNotificationRequest += (timestamp,
                                                       sender,
                                                       connection,
                                                       request,
                                                       cancellationToken) =>

                Task.FromResult(
                    new ForwardingDecision<OCPPv2_1.CS.BootNotificationRequest, BootNotificationResponse>(
                        request,
                        ForwardingResult.FORWARD
                    )
                );


        }

        #endregion


        #region HandleErrors(Module, Caller, ExceptionOccured)

        public override Task HandleErrors(String     Module,
                                          String     Caller,
                                          Exception  ExceptionOccured)
        {

            DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

            return Task.CompletedTask;

        }

        #endregion

        #region HandleErrors(Module, Caller, ErrorResponse)

        public override Task HandleErrors(String  Module,
                                          String  Caller,
                                          String  ErrorResponse)
        {

            DebugX.Log($"{Module}.{Caller}: {ErrorResponse}");

            return Task.CompletedTask;

        }

        #endregion


    }

}
