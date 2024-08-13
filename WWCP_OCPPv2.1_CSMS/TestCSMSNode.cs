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

using BCx509 = Org.BouncyCastle.X509;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Utilities;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A Charging Station Management System for testing
    /// </summary>
    public partial class TestCSMSNode : ACSMSNode,
                                        ICSMSNode
    {

        #region Constructor(s)

        /// <summary>
        /// Create a new charging station management system for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this charging station management system.</param>
        public TestCSMSNode(NetworkingNode_Id         Id,
                            String                    VendorName,
                            String                    Model,
                            String?                   SerialNumber                     = null,
                            String?                   SoftwareVersion                  = null,
                            I18NString?               Description                      = null,
                            CustomData?               CustomData                       = null,

                            AsymmetricCipherKeyPair?  ClientCAKeyPair                  = null,
                            BCx509.X509Certificate?   ClientCACertificate              = null,

                            SignaturePolicy?          SignaturePolicy                  = null,
                            SignaturePolicy?          ForwardingSignaturePolicy        = null,

                            Boolean                   HTTPAPI_Disabled                 = false,
                            IPPort?                   HTTPAPI_Port                     = null,
                            String?                   HTTPAPI_ServerName               = null,
                            String?                   HTTPAPI_ServiceName              = null,
                            EMailAddress?             HTTPAPI_RobotEMailAddress        = null,
                            String?                   HTTPAPI_RobotGPGPassphrase       = null,

                            DownloadAPI?              HTTPDownloadAPI                  = null,
                            Boolean                   HTTPDownloadAPI_Disabled         = false,
                            HTTPPath?                 HTTPDownloadAPI_Path             = null,
                            String?                   HTTPDownloadAPI_FileSystemPath   = null,

                            UploadAPI?                HTTPUploadAPI                    = null,
                            Boolean                   HTTPUploadAPI_Disabled           = false,
                            HTTPPath?                 HTTPUploadAPI_Path               = null,
                            String?                   HTTPUploadAPI_FileSystemPath     = null,

                            //HTTPPath?                 FirmwareDownloadAPIPath          = null,
                            //HTTPPath?                 LogfilesUploadAPIPath            = null,
                            //HTTPPath?                 DiagnosticsUploadAPIPath         = null,

                            QRCodeAPI?                QRCodeAPI                        = null,
                            Boolean                   QRCodeAPI_Disabled               = false,
                            HTTPPath?                 QRCodeAPI_Path                   = null,

                            Boolean                   WebAPI_Disabled                  = false,
                            HTTPPath?                 WebAPI_Path                      = null,

                            TimeSpan?                 DefaultRequestTimeout            = null,

                            Boolean                   DisableSendHeartbeats            = false,
                            TimeSpan?                 SendHeartbeatsEvery              = null,

                            Boolean                   DisableMaintenanceTasks          = false,
                            TimeSpan?                 MaintenanceEvery                 = null,

                            ISMTPClient?              SMTPClient                       = null,
                            DNSClient?                DNSClient                        = null)

            : base(Id,
                   VendorName,
                   Model,
                   SerialNumber,
                   SoftwareVersion,
                   Description,
                   CustomData,

                   ClientCAKeyPair,
                   ClientCACertificate,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   HTTPAPI_Disabled,
                   HTTPAPI_Port,
                   HTTPAPI_ServerName,
                   HTTPAPI_ServiceName,
                   HTTPAPI_RobotEMailAddress,
                   HTTPAPI_RobotGPGPassphrase,

                   HTTPDownloadAPI,
                   HTTPDownloadAPI_Disabled,
                   HTTPDownloadAPI_Path,
                   HTTPDownloadAPI_FileSystemPath,

                   HTTPUploadAPI,
                   HTTPUploadAPI_Disabled,
                   HTTPUploadAPI_Path,
                   HTTPUploadAPI_FileSystemPath,

                   //FirmwareDownloadAPIPath,
                   //LogfilesUploadAPIPath,
                   //DiagnosticsUploadAPIPath,

                   QRCodeAPI,
                   QRCodeAPI_Disabled,
                   QRCodeAPI_Path,

                   WebAPI_Disabled,
                   WebAPI_Path,

                   DefaultRequestTimeout,

                   DisableSendHeartbeats,
                   SendHeartbeatsEvery,

                   DisableMaintenanceTasks,
                   MaintenanceEvery,

                   SMTPClient,
                   DNSClient)

        {

            //Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "HTTPSSEs"));


            // CSMS -> CS


            #region CS -> CSMS Message Processing

            #region Certificates

            #region OnGet15118EVCertificate

            OCPP.IN.OnGet15118EVCertificate += (timestamp,
                                                sender,
                                                connection,
                                                request,
                                                cancellationToken) => {

                DebugX.Log("OnGet15118EVCertificate: " + request.DestinationId);

                // ISO15118SchemaVersion
                // CertificateAction
                // EXIRequest
                // MaximumContractCertificateChains
                // PrioritizedEMAIds

                return Task.FromResult(
                           new Get15118EVCertificateResponse(
                               Request:              request,
                               Status:               ISO15118EVCertificateStatus.Accepted,
                               EXIResponse:          EXIData.Parse("0x1234"),
                               RemainingContracts:   null,
                               StatusInfo:           null,
                               CustomData:           null
                           )
                       );

            };

            #endregion

            #region OnGetCertificateStatus

            OCPP.IN.OnGetCertificateStatus += (timestamp,
                                               sender,
                                               connection,
                                               request,
                                               cancellationToken) => {

                DebugX.Log("OnGetCertificateStatus: " + request.DestinationId);

                // OCSPRequestData

                return Task.FromResult(
                           new GetCertificateStatusResponse(
                               Request:      request,
                               Status:       GetCertificateStatus.Accepted,
                               OCSPResult:   OCSPResult.Parse("ok!"),
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnGetCRL

            OCPP.IN.OnGetCRL += (timestamp,
                                 sender,
                                 connection,
                                 request,
                                 cancellationToken) => {

                DebugX.Log("OnGetCRL: " + request.DestinationId);

                // GetCRLRequestId
                // CertificateHashData

                return Task.FromResult(
                           new GetCRLResponse(
                               Request:           request,
                               GetCRLRequestId:   request.GetCRLRequestId,
                               Status:            GenericStatus.Accepted,
                               StatusInfo:        null,
                               CustomData:        null
                           )
                       );

            };

            #endregion

            #region OnSignCertificate

            OCPP.IN.OnSignCertificate += async (timestamp,
                                                sender,
                                                connection,
                                                request,
                                                cancellationToken) => {

                DebugX.Log("OnSignCertificate: " + request.DestinationId);

                // CSR
                // CertificateType

                Pkcs10CertificationRequest?  parsedCSR       = null;
                String?                      errorResponse   = null;

                if (!request.CertificateType.HasValue ||
                     request.CertificateType.Value == CertificateSigningUse.ChargingStationCertificate)
                {

                    try
                    {

                        using (var reader = new StringReader(request.CSR))
                        {
                            var pemReader = new PemReader(reader);
                            parsedCSR     = (Pkcs10CertificationRequest) pemReader.ReadObject();
                        }

                    } catch (Exception e)
                    {
                        errorResponse = "The certificate signing request could not be parsed: " + e.Message;
                    }

                    if (parsedCSR is null)
                        errorResponse = "The certificate signing request could not be parsed!";

                    else
                    {

                        if (!parsedCSR.Verify())
                            errorResponse = "The certificate signing request could not be verified!";

                        else if (ClientCAKeyPair     is null)
                            errorResponse = "No ClientCA key pair available!";

                        else if (ClientCACertificate is null)
                            errorResponse = "No ClientCA certificcate available!";

                        else
                        {

                            #region Sign the client certificate (background process!)

                            //ToDo: Find better ways to do this!
                            var delayedResponse = Task.Run(async () => {

                                try
                                {

                                    await Task.Delay(100);

                                    var secureRandom  = new SecureRandom();
                                    var now           = Timestamp.Now;

                                    var certificateGenerator = new X509V3CertificateGenerator();
                                    certificateGenerator.SetIssuerDN    (ClientCACertificate.SubjectDN);
                                    certificateGenerator.SetSubjectDN   (parsedCSR.GetCertificationRequestInfo().Subject);
                                    certificateGenerator.SetPublicKey   (parsedCSR.GetPublicKey());
                                    certificateGenerator.SetSerialNumber(BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), secureRandom));
                                    certificateGenerator.SetNotBefore   (now.AddDays(-1));
                                    certificateGenerator.SetNotAfter    (now.AddMonths(3));

                                    certificateGenerator.AddExtension   (X509Extensions.KeyUsage,
                                                                         critical: true,
                                                                         new KeyUsage(
                                                                             KeyUsage.NonRepudiation |
                                                                             KeyUsage.DigitalSignature |
                                                                             KeyUsage.KeyEncipherment
                                                                         ));

                                    certificateGenerator.AddExtension   (X509Extensions.ExtendedKeyUsage,
                                                                         critical: true,
                                                                         new ExtendedKeyUsage(KeyPurposeID.id_kp_clientAuth));

                                    var newClientCertificate = certificateGenerator.Generate(
                                                                   new Asn1SignatureFactory(
                                                                       "SHA256WithRSAEncryption",
                                                                       ClientCAKeyPair.Private,
                                                                       secureRandom
                                                                   )
                                                               );


                                    await Task.Delay(3000);


                                    await OCPP.OUT.CertificateSigned(
                                              new CertificateSignedRequest(
                                                  request.NetworkPath.Source,
                                                  CertificateChain.From(ClientCACertificate, newClientCertificate),
                                                  request.CertificateType ?? CertificateSigningUse.ChargingStationCertificate
                                              )
                                          );

                                } catch (Exception e)
                                {
                                    DebugX.LogException(e, "The client certificate could not be signed!");
                                }

                            },
                            cancellationToken);

                            #endregion

                        }

                    }

                }
                else
                    errorResponse = $"Invalid CertificateSigningUse: '{request.CertificateType?.ToString() ?? "-"}'!";

                return errorResponse is not null

                           ? new SignCertificateResponse(
                                 Request:      request,
                                 Status:       GenericStatus.Rejected,
                                 Result:       Result.GenericError(errorResponse),
                                 StatusInfo:   null,
                                 CustomData:   null
                             )

                           : new SignCertificateResponse(
                                 Request:      request,
                                 Status:       GenericStatus.Accepted,
                                 StatusInfo:   null,
                                 CustomData:   null
                             );

            };

            #endregion

            #endregion

            #region Charging

            #region OnAuthorize

            OCPP.IN.OnAuthorize += (timestamp,
                                    sender,
                                    connection,
                                    request,
                                    cancellationToken) => {

                DebugX.Log("OnAuthorize: " + request.DestinationId + ", " +
                                             request.IdToken);

                // IdToken
                // Certificate
                // ISO15118CertificateHashData

                return Task.FromResult(
                           new AuthorizeResponse(
                               Request:             request,
                               IdTokenInfo:         new IdTokenInfo(
                                                        Status:                AuthorizationStatus.Accepted,
                                                        CacheExpiryDateTime:   Timestamp.Now.AddDays(3)
                                                    ),
                               CertificateStatus:   AuthorizeCertificateStatus.Accepted,
                               CustomData:          null
                           )
                       );

            };

            #endregion

            #region OnClearedChargingLimit

            OCPP.IN.OnClearedChargingLimit += (timestamp,
                                               sender,
                                               connection,
                                               request,
                                               cancellationToken) => {

                DebugX.Log("OnClearedChargingLimit: " + request.DestinationId);

                // ChargingLimitSource
                // EVSEId

                return Task.FromResult(
                           new ClearedChargingLimitResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnMeterValues

            OCPP.IN.OnMeterValues += (timestamp,
                                      sender,
                                      connection,
                                      request,
                                      cancellationToken) => {

                DebugX.Log("OnMeterValues: " + request.EVSEId);

                DebugX.Log(request.MeterValues.SafeSelect(meterValue => meterValue.Timestamp.ToIso8601() +
                                                                        meterValue.SampledValues.SafeSelect(sampledValue => sampledValue.Context + ", " + sampledValue.Value + ", " + sampledValue.Value).AggregateWith("; ")).AggregateWith(Environment.NewLine));

                // EVSEId
                // MeterValues

                return Task.FromResult(
                           new MeterValuesResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnNotifyChargingLimit

            OCPP.IN.OnNotifyChargingLimit += (timestamp,
                                              sender,
                                              connection,
                                              request,
                                              cancellationToken) => {

                DebugX.Log("OnNotifyChargingLimit: " + request.DestinationId);

                // ChargingLimit
                // ChargingSchedules
                // EVSEId

                return Task.FromResult(
                           new NotifyChargingLimitResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnNotifyEVChargingNeeds

            OCPP.IN.OnNotifyEVChargingNeeds += (timestamp,
                                                sender,
                                                connection,
                                                request,
                                                cancellationToken) => {

                DebugX.Log("OnNotifyEVChargingNeeds: " + request.DestinationId);

                // EVSEId
                // ChargingNeeds
                // MaxScheduleTuples

                return Task.FromResult(
                           new NotifyEVChargingNeedsResponse(
                               Request:      request,
                               Status:       NotifyEVChargingNeedsStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnNotifyEVChargingSchedule

            OCPP.IN.OnNotifyEVChargingSchedule += (timestamp,
                                                   sender,
                                                   connection,
                                                   request,
                                                   cancellationToken) => {

                DebugX.Log("OnNotifyEVChargingSchedule: " + request.DestinationId);

                // TimeBase
                // EVSEId
                // ChargingSchedule
                // SelectedScheduleTupleId
                // PowerToleranceAcceptance

                return Task.FromResult(
                           new NotifyEVChargingScheduleResponse(
                               Request:      request,
                               Status:       GenericStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnNotifyPriorityCharging

            OCPP.IN.OnNotifyPriorityCharging += (timestamp,
                                                 sender,
                                                 connection,
                                                 request,
                                                 cancellationToken) => {

                DebugX.Log("OnNotifyPriorityCharging: " + request.DestinationId);

                // TransactionId
                // Activated

                return Task.FromResult(
                           new NotifyPriorityChargingResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            // NotifySettlement

            #region OnPullDynamicScheduleUpdate

            OCPP.IN.OnPullDynamicScheduleUpdate += (timestamp,
                                                    sender,
                                                    connection,
                                                    request,
                                                    cancellationToken) => {

                DebugX.Log("OnPullDynamicScheduleUpdate: " + request.DestinationId);

                // ChargingProfileId

                return Task.FromResult(
                           new PullDynamicScheduleUpdateResponse(

                               Request:               request,

                               Limit:                 ChargingRateValue.Parse( 1, ChargingRateUnits.Watts),
                               Limit_L2:              ChargingRateValue.Parse( 2, ChargingRateUnits.Watts),
                               Limit_L3:              ChargingRateValue.Parse( 3, ChargingRateUnits.Watts),

                               DischargeLimit:        ChargingRateValue.Parse(-4, ChargingRateUnits.Watts),
                               DischargeLimit_L2:     ChargingRateValue.Parse(-5, ChargingRateUnits.Watts),
                               DischargeLimit_L3:     ChargingRateValue.Parse(-6, ChargingRateUnits.Watts),

                               Setpoint:              ChargingRateValue.Parse( 7, ChargingRateUnits.Watts),
                               Setpoint_L2:           ChargingRateValue.Parse( 8, ChargingRateUnits.Watts),
                               Setpoint_L3:           ChargingRateValue.Parse( 9, ChargingRateUnits.Watts),

                               SetpointReactive:      ChargingRateValue.Parse(10, ChargingRateUnits.Watts),
                               SetpointReactive_L2:   ChargingRateValue.Parse(11, ChargingRateUnits.Watts),
                               SetpointReactive_L3:   ChargingRateValue.Parse(12, ChargingRateUnits.Watts),

                               CustomData:            null

                           )
                       );

            };

            #endregion

            #region OnReportChargingProfiles

            OCPP.IN.OnReportChargingProfiles += (timestamp,
                                                 sender,
                                                 connection,
                                                 request,
                                                 cancellationToken) => {

                DebugX.Log("OnReportChargingProfiles: " + request.DestinationId);

                // ReportChargingProfilesRequestId
                // ChargingLimitSource
                // EVSEId
                // ChargingProfiles
                // ToBeContinued

                return Task.FromResult(
                           new ReportChargingProfilesResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnReservationStatusUpdate

            OCPP.IN.OnReservationStatusUpdate += (timestamp,
                                                  sender,
                                                  connection,
                                                  request,
                                                  cancellationToken) => {

                DebugX.Log("OnReservationStatusUpdate: " + request.DestinationId);

                // ReservationId
                // ReservationUpdateStatus

                return Task.FromResult(
                           new ReservationStatusUpdateResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnStatusNotification

            OCPP.IN.OnStatusNotification += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             cancellationToken) => {

                DebugX.Log($"OnStatusNotification: {request.EVSEId}/{request.ConnectorId} => {request.ConnectorStatus}");

                // Timestamp
                // ConnectorStatus
                // EVSEId
                // ConnectorId

                return Task.FromResult(
                           new StatusNotificationResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnTransactionEvent

            OCPP.IN.OnTransactionEvent += (timestamp,
                                           sender,
                                           connection,
                                           request,
                                           cancellationToken) => {

                DebugX.Log("OnTransactionEvent: " + request.DestinationId + ", " +
                                                    request.IdToken);

                // ChargingStationId
                // EventType
                // Timestamp
                // TriggerReason
                // SequenceNumber
                // TransactionInfo
                // 
                // Offline
                // NumberOfPhasesUsed
                // CableMaxCurrent
                // ReservationId
                // IdToken
                // EVSE
                // MeterValues
                // PreconditioningStatus

                return Task.FromResult(
                           // Plugfest 2024-04 fixes...
                           request.TriggerReason == TriggerReason.Authorized ||
                           request.TriggerReason == TriggerReason.RemoteStart

                               ? new TransactionEventResponse(
                                     Request:                  request,
                                     TotalCost:                null,
                                     ChargingPriority:         null,
                                     IdTokenInfo:              new IdTokenInfo(
                                                                   Status:                AuthorizationStatus.Accepted,
                                                                   CacheExpiryDateTime:   Timestamp.Now + TimeSpan.FromDays(1)
                                                               ),
                                     UpdatedPersonalMessage:   null,
                                     CustomData:               null
                                 )

                               : new TransactionEventResponse(
                                     Request:                  request,
                                     TotalCost:                null,
                                     ChargingPriority:         null,
                                     IdTokenInfo:              null,
                                     UpdatedPersonalMessage:   null,
                                     CustomData:               null
                                 )

                       );

            };

            #endregion

            #endregion

            #region Customer

            #region OnNotifyCustomerInformation

            OCPP.IN.OnNotifyCustomerInformation += (timestamp,
                                                    sender,
                                                    connection,
                                                    request,
                                                    cancellationToken) => {

                DebugX.Log("OnNotifyCustomerInformation: " + request.DestinationId);

                // NotifyCustomerInformationRequestId
                // Data
                // SequenceNumber
                // GeneratedAt
                // ToBeContinued

                return Task.FromResult(
                           new NotifyCustomerInformationResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnNotifyDisplayMessages

            OCPP.IN.OnNotifyDisplayMessages += (timestamp,
                                                sender,
                                                connection,
                                                request,
                                                cancellationToken) => {

                //DebugX.Log("OnNotifyDisplayMessages: " + Request.EVSEId);

                //DebugX.Log(Request.NotifyDisplayMessages.SafeSelect(meterValue => meterValue.Timestamp.ToIso8601() +
                //                          meterValue.SampledValues.SafeSelect(sampledValue => sampledValue.Context + ", " + sampledValue.Value + ", " + sampledValue.Value).AggregateWith("; ")).AggregateWith(Environment.NewLine));

                // NotifyDisplayMessagesRequestId
                // MessageInfos
                // ToBeContinued

                return Task.FromResult(
                           new NotifyDisplayMessagesResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #endregion

            #region Firmware

            #region OnBootNotification

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

            #endregion

            #region OnFirmwareStatusNotification

            OCPP.IN.OnFirmwareStatusNotification += (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     cancellationToken) => {

                DebugX.Log("OnFirmwareStatus: " + request.Status);

                // Status
                // UpdateFirmwareRequestId

                return Task.FromResult(
                           new FirmwareStatusNotificationResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnHeartbeat

            OCPP.IN.OnHeartbeat += (timestamp,
                                    sender,
                                    connection,
                                    request,
                                    cancellationToken) => {

                DebugX.Log("OnHeartbeat: " + request.DestinationId);

                return Task.FromResult(
                           new HeartbeatResponse(
                               Request:       request,
                               CurrentTime:   Timestamp.Now,
                               CustomData:    null
                           )
                       );

            };

            #endregion

            #region OnPublishFirmwareStatusNotification

            OCPP.IN.OnPublishFirmwareStatusNotification += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            cancellationToken) => {

                DebugX.Log("OnPublishFirmwareStatusNotification: " + request.DestinationId);

                // Status
                // PublishFirmwareStatusNotificationRequestId
                // DownloadLocations

                return Task.FromResult(
                           new PublishFirmwareStatusNotificationResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #endregion

            #region Monitoring

            #region OnLogStatusNotification

            OCPP.IN.OnLogStatusNotification += (timestamp,
                                                sender,
                                                connection,
                                                request,
                                                cancellationToken) => {

                DebugX.Log("OnLogStatusNotification: " + request.DestinationId);

                // Status
                // LogRquestId

                return Task.FromResult(
                           new LogStatusNotificationResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnNotifyEvent

            OCPP.IN.OnNotifyEvent += (timestamp,
                                      sender,
                                      connection,
                                      request,
                                      cancellationToken) => {

                DebugX.Log("OnNotifyEvent: " + request.DestinationId);

                // GeneratedAt
                // SequenceNumber
                // EventData
                // ToBeContinued

                return Task.FromResult(
                           new NotifyEventResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnNotifyMonitoringReport

            OCPP.IN.OnNotifyMonitoringReport += (timestamp,
                                                 sender,
                                                 connection,
                                                 request,
                                                 cancellationToken) => {

                DebugX.Log("OnNotifyMonitoringReport: " + request.DestinationId);

                // NotifyMonitoringReportRequestId
                // SequenceNumber
                // GeneratedAt
                // MonitoringData
                // ToBeContinued

                return Task.FromResult(
                           new NotifyMonitoringReportResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnNotifyReport

            OCPP.IN.OnNotifyReport += (timestamp,
                                       sender,
                                       connection,
                                       request,
                                       cancellationToken) => {

                DebugX.Log("OnNotifyReport: " + request.DestinationId);

                // NotifyReportRequestId
                // SequenceNumber
                // GeneratedAt
                // ReportData

                return Task.FromResult(
                           new NotifyReportResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnSecurityEventNotification

            OCPP.IN.OnSecurityEventNotification += (timestamp,
                                                    sender,
                                                    connection,
                                                    request,
                                                    cancellationToken) => {

                DebugX.Log("OnSecurityEventNotification: " + request.DestinationId);

                // Type
                // Timestamp
                // TechInfo

                return Task.FromResult(
                           new SecurityEventNotificationResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #endregion


            #region OnDataTransfer

            OCPP.IN.OnDataTransfer +=  (timestamp,
                                        sender,
                                        connection,
                                        request,
                                        cancellationToken) => {

                DebugX.Log($"CSMS '{Id}': Incoming DataTransfer: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data?.ToString() ?? "-"}!");

                // VendorId
                // MessageId
                // Data

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


                return Task.FromResult(

                           request.VendorId == Vendor_Id.GraphDefined

                               ? new (
                                     Request:      request,
                                     Status:       DataTransferStatus.Accepted,
                                     Data:         responseData,
                                     StatusInfo:   null,
                                     CustomData:   null
                                 )

                               : new DataTransferResponse(
                                     Request:      request,
                                     Status:       DataTransferStatus.Rejected,
                                     Data:         null,
                                     StatusInfo:   null,
                                     CustomData:   null
                               )

                       );

            };

            #endregion


            #region BinaryDataStreams Extensions

            #region OnBinaryDataTransfer

            OCPP.IN.OnBinaryDataTransfer += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             cancellationToken) => {

                DebugX.Log($"CSMS '{Id}': Incoming BinaryDataTransfer: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data?.ToHexString() ?? "-"}!");

                // VendorId
                // MessageId
                // BinaryData

                var responseBinaryData = Array.Empty<Byte>();

                if (request.Data is not null)
                    responseBinaryData = ((Byte[]) request.Data.Clone()).Reverse();

                return Task.FromResult(

                           request.VendorId == Vendor_Id.GraphDefined

                               ? new BinaryDataTransferResponse(
                                     Request:                request,
                                     Status:                 BinaryDataTransferStatus.Accepted,
                                     AdditionalStatusInfo:   null,
                                     Data:                   responseBinaryData
                                 )

                               : new BinaryDataTransferResponse(
                                     Request:                request,
                                     Status:                 BinaryDataTransferStatus.Rejected,
                                     AdditionalStatusInfo:   null,
                                     Data:                   responseBinaryData
                                 )

                       );

            };

            #endregion

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

            #region E2E Security Extensions

            #region OnSecureDataTransfer

            OCPP.IN.OnSecureDataTransfer += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             cancellationToken) => {

                DebugX.Log($"Local Controller '{Id}': Incoming SecureDataTransfer request!");

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
                                     DestinationId:          request.NetworkPath.Source,
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

            #endregion

            #endregion

            #region Overlay Networking Extensions

            #region OnIncomingNotifyNetworkTopology

            OCPP.IN.OnNotifyNetworkTopology += (timestamp,
                                                sender,
                                                connection,
                                                request,
                                                cancellationToken) => {

                DebugX.Log("OnIncomingNotifyNetworkTopology: " + request.NetworkTopologyInformation);

                // NetworkTopologyInformation

                return Task.FromResult(
                           new NotifyNetworkTopologyResponse(
                               Request:      request,
                               Status:       NetworkTopologyStatus.Accepted,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #endregion

            #endregion

            // Firmware API download messages
            // Logdata API upload messages
            // Diagnostics API upload messages
            // QR-Code API messages

        }

        #endregion

    }

}
