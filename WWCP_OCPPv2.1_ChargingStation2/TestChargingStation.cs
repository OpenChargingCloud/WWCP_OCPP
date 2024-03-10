/*
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

using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A networking node for testing.
    /// </summary>
    public partial class TestChargingStation : AChargingStation
    {

        #region Properties


        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new networking node for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this networking node.</param>
        public TestChargingStation(NetworkingNode_Id  Id,
                                  String             VendorName,
                                  String             Model,
                                  I18NString?        Description                 = null,
                                  String?            SerialNumber                = null,
                                  String?            FirmwareVersion             = null,
                                  Modem?             Modem                       = null,

                                  SignaturePolicy?   SignaturePolicy             = null,
                                  SignaturePolicy?   ForwardingSignaturePolicy   = null,

                                  IPPort?            HTTPUploadPort              = null,
                                  IPPort?            HTTPDownloadPort            = null,

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

                   HTTPUploadPort,
                   HTTPDownloadPort,

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

                var secureData          = request.Decrypt(GetDecryptionKey(request.NetworkPath.Source, request.KeyId)).ToUTF8String();
                var responseSecureData  = secureData?.Reverse();
                var keyId               = (UInt16) 1;

                return Task.FromResult(
                           request.Ciphertext is not null

                               ? SecureDataTransferResponse.Encrypt(
                                     Request:                request,
                                     Status:                 SecureDataTransferStatus.Accepted,
                                     DestinationNodeId:      request.NetworkPath.Source,
                                     Parameter:              0,
                                     KeyId:                  keyId,
                                     Key:                    GetEncryptionKey    (request.NetworkPath.Source, keyId),
                                     Nonce:                  GetEncryptionNonce  (request.NetworkPath.Source, keyId),
                                     Counter:                GetEncryptionCounter(request.NetworkPath.Source, keyId),
                                     Payload:                responseSecureData?.ToUTF8Bytes() ?? [],
                                     AdditionalStatusInfo:   null,

                                     SignKeys:               null,
                                     SignInfos:              null,
                                     Signatures:             null,

                                     EventTrackingId:        request.EventTrackingId,
                                     NetworkPath:            NetworkPath.From(Id)

                                 )

                               : new SecureDataTransferResponse(
                                     Request:                request,
                                     NetworkPath:            NetworkPath.From(Id),
                                     Status:                 SecureDataTransferStatus.Rejected,
                                     AdditionalStatusInfo:   null,
                                     Ciphertext:             responseSecureData?.ToUTF8Bytes()
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


    }

}
