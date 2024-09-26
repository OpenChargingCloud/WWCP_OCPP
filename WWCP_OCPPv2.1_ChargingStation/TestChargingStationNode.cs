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
using System.Collections.ObjectModel;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Mail;

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A charging station node for testing.
    /// </summary>
    public partial class TestChargingStationNode : AChargingStationNode,
                                                   IChargingStationNode
    {

        #region Constructor(s)

        /// <summary>
        /// Create a new networking node for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this networking node.</param>
        public TestChargingStationNode(NetworkingNode_Id       Id,
                                       String                  VendorName,
                                       String                  Model,
                                       I18NString?             Description                    = null,
                                       String?                 SerialNumber                   = null,
                                       String?                 FirmwareVersion                = null,
                                       Modem?                  Modem                          = null,

                                       IEnumerable<EVSESpec>?  EVSEs                          = null,
                                       IEnergyMeter?           UplinkEnergyMeter              = null,

                                       TimeSpan?               DefaultRequestTimeout          = null,

                                       SignaturePolicy?        SignaturePolicy                = null,
                                       SignaturePolicy?        ForwardingSignaturePolicy      = null,

                                       Boolean                 HTTPAPI_Disabled               = false,
                                       IPPort?                 HTTPAPI_Port                   = null,
                                       String?                 HTTPAPI_ServerName             = null,
                                       String?                 HTTPAPI_ServiceName            = null,
                                       EMailAddress?           HTTPAPI_RobotEMailAddress      = null,
                                       String?                 HTTPAPI_RobotGPGPassphrase     = null,
                                       Boolean                 HTTPAPI_EventLoggingDisabled   = false,

                                       WebAPI?                 WebAPI                         = null,
                                       Boolean                 WebAPI_Disabled                = false,
                                       HTTPPath?               WebAPI_Path                    = null,

                                       Boolean                 DisableSendHeartbeats          = false,
                                       TimeSpan?               SendHeartbeatsEvery            = null,

                                       Boolean                 DisableMaintenanceTasks        = false,
                                       TimeSpan?               MaintenanceEvery               = null,

                                       CustomData?             CustomData                     = null,
                                       DNSClient?              DNSClient                      = null)

            : base(Id,
                   VendorName,
                   Model,
                   Description,
                   SerialNumber,
                   FirmwareVersion,
                   Modem,

                   EVSEs,
                   UplinkEnergyMeter,

                   DefaultRequestTimeout,

                   SignaturePolicy,
                   ForwardingSignaturePolicy,

                   HTTPAPI_Disabled,
                   HTTPAPI_Port,
                   HTTPAPI_ServerName,
                   HTTPAPI_ServiceName,
                   HTTPAPI_RobotEMailAddress,
                   HTTPAPI_RobotGPGPassphrase,
                   HTTPAPI_EventLoggingDisabled,

                   WebAPI,
                   WebAPI_Disabled,
                   WebAPI_Path,

                   DisableSendHeartbeats,
                   SendHeartbeatsEvery,

                   DisableMaintenanceTasks,
                   MaintenanceEvery,

                   CustomData,
                   DNSClient)

        #endregion

        {

            this.OnQRCodeChanged += (timestamp, chargingStation, evseId, qrCodeURL, remainingTime, endTime, ct) => {
                DebugX.Log($"New QR-Code URL for EVSE '{chargingStation.Id}/{evseId}': {qrCodeURL} ({(UInt32) remainingTime.TotalSeconds} sec.)");
                return Task.CompletedTask;
            };


            //Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "HTTPSSEs"));


            #region Certificates

            #region OnCertificateSigned

            OCPP.IN.OnCertificateSigned += (timestamp,
                                            sender,
                                            connection,
                                            request,
                                            ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming CertificateSigned request{(request.CertificateType.HasValue ? $"(certificate type: {request.CertificateType.Value})" : "")}!");

                // CertificateChain
                // CertificateType

                return Task.FromResult(
                           new CertificateSignedResponse(
                               Request:      request,
                               Status:       request.CertificateChain.FirstOrDefault()?.Parsed is not null
                                                 ? CertificateSignedStatus.Accepted
                                                 : CertificateSignedStatus.Rejected,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnInstallCertificate

            OCPP.IN.OnInstallCertificate += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming InstallCertificate request (certificate type: {request.CertificateType}!");

                // CertificateType
                // Certificate

                var success = certificates.AddOrUpdate(request.CertificateType,
                                                           a    => request.Certificate,
                                                          (b,c) => request.Certificate);

                return Task.FromResult(
                           new InstallCertificateResponse(
                               Request:      request,
                               Status:       request.Certificate?.Parsed is not null
                                                 ? CertificateStatus.Accepted
                                                 : CertificateStatus.Rejected,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnGetInstalledCertificateIds

            OCPP.IN.OnGetInstalledCertificateIds += (timestamp,
                                                     sender,
                                                     connection,
                                                     request,
                                                     ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming GetInstalledCertificateIds request for certificate types: {request.CertificateTypes.Select(certificateType => certificateType).AggregateWith(", ")}!");

                // CertificateTypes

                var certificateHashDataChain = new List<CertificateHashDataChain>();

                foreach (var certificateType in request.CertificateTypes)
                {

                    if (certificates.TryGetValue(InstallCertificateUse.Parse(certificateType.ToString()), out var cert))
                        certificateHashDataChain.Add(
                            new CertificateHashDataChain(
                                new CertificateHashData(
                                    HashAlgorithm:         HashAlgorithms.SHA256,
                                    IssuerNameHash:        cert.Parsed?.Issuer               ?? "-",
                                    IssuerPublicKeyHash:   cert.Parsed?.GetPublicKeyString() ?? "-",
                                    SerialNumber:          cert.Parsed?.SerialNumber         ?? "-",
                                    CustomData:            null
                                ),
                                GetCertificateIdUse.CSMSRootCertificate
                            )
                        );

                }

                return Task.FromResult(
                           new GetInstalledCertificateIdsResponse(
                               Request:                    request,
                               Status:                     GetInstalledCertificateStatus.Accepted,
                               CertificateHashDataChain:   certificateHashDataChain,
                               StatusInfo:                 null,
                               CustomData:                 null
                           )
                       );

            };

            #endregion

            #region OnDeleteCertificate

            OCPP.IN.OnDeleteCertificate += (timestamp,
                                            sender,
                                            connection,
                                            request,
                                            ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming DeleteCertificate request!");

                // CertificateHashData

                var certKV  = certificates.FirstOrDefault(certificateKV => request.CertificateHashData.SerialNumber == certificateKV.Value.Parsed?.SerialNumber);

                var success = certificates.TryRemove(certKV);

                return Task.FromResult(
                           new DeleteCertificateResponse(
                               Request:      request,
                               Status:       success
                                                 ? DeleteCertificateStatus.Accepted
                                                 : DeleteCertificateStatus.NotFound,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnNotifyCRL

            OCPP.IN.OnNotifyCRL += (timestamp,
                                    sender,
                                    connection,
                                    request,
                                    ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming NotifyCRL request!");

                // NotifyCRLRequestId
                // Availability
                // Location

                return Task.FromResult(
                           new NotifyCRLResponse(
                               Request:      request,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #endregion

            #region Charging

            #region ChargingProfiles

            #region OnSetChargingProfile

            OCPP.IN.OnSetChargingProfile += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             ct) => {

                SetChargingProfileResponse? response = null;

                DebugX.Log($"Charging station '{Id}': Incoming SetChargingProfile for '{request.EVSEId}'!");

                // EVSEId
                // ChargingProfile

                // ToDo: lock(connectors)

                if (request.EVSEId.Value == 0)
                {

                    foreach (var evse in evses.Values)
                    {

                        if (!request.ChargingProfile.TransactionId.HasValue)
                            evse.ChargingProfile = request.ChargingProfile;

                        else if (evse.TransactionId == request.ChargingProfile.TransactionId.Value)
                            evse.ChargingProfile = request.ChargingProfile;

                    }

                    response = new SetChargingProfileResponse(
                                   request,
                                   ChargingProfileStatus.Accepted
                               );

                }
                else if (evses.TryGetValue(request.EVSEId, out var evse))
                {

                    evse.ChargingProfile = request.ChargingProfile;

                    response = new SetChargingProfileResponse(
                                   request,
                                   ChargingProfileStatus.Accepted
                               );

                }

                response ??= new SetChargingProfileResponse(
                                 request,
                                 ChargingProfileStatus.Rejected
                             );

                return Task.FromResult(response);

            };

            #endregion

            #region OnGetChargingProfiles

            OCPP.IN.OnGetChargingProfiles += (timestamp,
                                              sender,
                                              connection,
                                              request,
                                              ct) => {

                GetChargingProfilesResponse? response = null;

                DebugX.Log($"Charging station '{Id}': Incoming GetChargingProfiles request ({request.GetChargingProfilesRequestId}) for '{request.EVSEId}'!");

                // GetChargingProfilesRequestId
                // ChargingProfile
                // EVSEId

                if (request.EVSEId.HasValue && evses.ContainsKey(request.EVSEId.Value))
                {

                    //evses[Request.EVSEId.Value].ChargingProfile = Request.ChargingProfile;

                    response = new GetChargingProfilesResponse(
                                   request,
                                   GetChargingProfileStatus.Accepted
                               );

                }
                else
                   response = new GetChargingProfilesResponse(
                                  request,
                                  GetChargingProfileStatus.Unknown
                              );

                return Task.FromResult(response);

            };

            #endregion

            #region OnClearChargingProfile

            OCPP.IN.OnClearChargingProfile += (timestamp,
                                               sender,
                                               connection,
                                               request,
                                               ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming ClearChargingProfile request for charging profile identification '{request.ChargingProfileId}'!");

                // ChargingProfileId
                // ChargingProfileCriteria

                return Task.FromResult(
                           new ClearChargingProfileResponse(
                               Request:      request,
                               Status:       ClearChargingProfileStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #endregion

            #region Tariffs

            #region OnChangeTransactionTariff

            OCPP.IN.OnChangeTransactionTariff +=
                (timestamp, sender, connection, request, ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming ChangeTransactionTariff '{request.Tariff}' for transaction '{request.TransactionId}'!");

                return Task.FromResult(
                           new ChangeTransactionTariffResponse(
                               Request:      request,
                               Status:       //transactions.ContainsKey(request.TransactionId)
                                             //    ? TariffStatus.Accepted
                                             //    : TariffStatus.Rejected,
                                             TariffStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnClearTariffs

            OCPP.IN.OnClearTariffs +=
                (timestamp, sender, connection, request, ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming ClearTariffs '{request}'!");

                // TariffIds
                // TariffKind

                return Task.FromResult(
                           new ClearTariffsResponse(
                               Request:               request,
                               ClearTariffsResults:   [
                                                          new ClearTariffsResult(
                                                              TariffId:    Tariff_Id.New(),
                                                              Status:      TariffStatus.Accepted,
                                                              StatusInfo:  null
                                                          )
                                                      ],
                               CustomData:            null
                           )
                       );

            };

            #endregion

            #region OnGetTariffs

            OCPP.IN.OnGetTariffs +=
                (timestamp, sender, connection, request, ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming ClearTariffs '{request}'!");

                // TariffIds
                // TariffKind

                return Task.FromResult(
                           new GetTariffsResponse(
                               Request:             request,
                               Status:              TariffStatus.Accepted,
                               TariffAssignments:   [
                                                        new TariffAssignment(
                                                            TariffId:    Tariff_Id.New(),
                                                            TariffKind:  TariffKinds.DefaultTariff,
                                                            EVSEIds:     [ EVSE_Id.Parse(1), EVSE_Id.Parse(2)],
                                                            IdTokens:    [IdToken.NewRandomRFID4(), IdToken.NewRandomRFID7() ]
                                                        )
                                                    ],
                               StatusInfo:          null,
                               CustomData:          null
                           )
                       );

            };

            #endregion

            #region OnSetDefaultTariff

            OCPP.IN.OnSetDefaultTariff +=
                (timestamp, sender, connection, request, ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming DefaultTariff '{request.Tariff}' for {(request.EVSEId.Value == 0 ? "all EVSEs" : $"for EVSE '{request.EVSEId}'")}!");

                return Task.FromResult(
                           new SetDefaultTariffResponse(
                               Request:      request,
                               Status:       request.EVSEId.Value <= 2
                                                 ? TariffStatus.Accepted
                                                 : TariffStatus.Rejected,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #endregion

            #region Transactions

            #region OnCancelReservation

            OCPP.IN.OnCancelReservation += (timestamp,
                                            sender,
                                            connection,
                                            request,
                                            ct) => {

                var success = reservations.ContainsKey(request.ReservationId)
                                  ? reservations.TryRemove(request.ReservationId, out _)
                                  : true;

                DebugX.Log($"Charging station '{Id}': Incoming CancelReservation request for reservation id '{request.ReservationId}': {(success ? "accepted" : "rejected")}!");

                // ReservationId

                return Task.FromResult(
                           new CancelReservationResponse(
                               Request:      request,
                               Status:       success
                                                 ? CancelReservationStatus.Accepted
                                                 : CancelReservationStatus.Rejected,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnGetTransactionStatus

            OCPP.IN.OnGetTransactionStatus += (timestamp,
                                               sender,
                                               connection,
                                               request,
                                               ct) => {

                GetTransactionStatusResponse? response = null;

                DebugX.Log($"Charging station '{Id}': Incoming GetTransactionStatus for '{request.TransactionId}'!");

                // TransactionId

                if (request.TransactionId.HasValue)
                {

                    var foundEVSE =  evses.Values.FirstOrDefault(evse => request.TransactionId == evse.TransactionId);

                    if (foundEVSE is not null)
                    {

                        response = new GetTransactionStatusResponse(
                                       request,
                                       MessagesInQueue:    false,
                                       OngoingIndicator:   true
                                   );

                    }
                    else
                    {

                        response = new GetTransactionStatusResponse(
                                       request,
                                       MessagesInQueue:    false,
                                       OngoingIndicator:   true
                                   );

                    }

                }
                else
                {

                    response = new GetTransactionStatusResponse(
                                   request,
                                   MessagesInQueue:    false,
                                   OngoingIndicator:   true
                               );

                }

                return Task.FromResult(response);

            };

            #endregion

            #region OnQRCodeScanned

            OCPP.IN.OnQRCodeScanned += (timestamp,
                                        sender,
                                        connection,
                                        request,
                                        ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming QRCodeScanned request (EVSE id: {request.EVSEId}, timeout: '{request.Timeout.TotalSeconds} secs)!");

                return Task.FromResult(
                           new QRCodeScannedResponse(
                               Request:     request,
                               CustomData:  null
                           )
                       );

            };

            #endregion

            #region OnRequestStartTransaction

            OCPP.IN.OnRequestStartTransaction += (timestamp,
                                                  sender,
                                                  connection,
                                                  request,
                                                  ct) => {

                RequestStartTransactionResponse? response = null;

                DebugX.Log($"Charging station '{Id}': Incoming RequestStartTransaction for '{(request.EVSEId?.ToString() ?? "-")}'!");

                // ToDo: lock(evses)

                if (request.EVSEId.HasValue &&
                    evses.TryGetValue(request.EVSEId.Value, out var evse) &&
                    !evse.IsCharging)
                {

                    evse.IsCharging              = true;
                    evse.TransactionId           = Transaction_Id.NewRandom;
                    evse.RemoteStartId           = request.RequestStartTransactionRequestId;

                    evse.StartTimestamp          = Timestamp.Now;
                    evse.MeterStartValue         = 0;
                    evse.SignedStartMeterValue   = "0";

                    evse.StopTimestamp           = null;
                    evse.MeterStopValue          = null;
                    evse.SignedStopMeterValue    = null;

                    evse.IdToken                 = request.IdToken;
                    evse.GroupIdToken            = request.GroupIdToken;
                    evse.ChargingProfile         = request.ChargingProfile;

                    _ = Task.Run(async () => {

                        await Task.Delay(500);

                        await this.SendTransactionEvent(

                                  EventType:            TransactionEvents.Started,
                                  Timestamp:            evse.StartTimestamp.Value,
                                  TriggerReason:        TriggerReason.RemoteStart,
                                  SequenceNumber:       1,
                                  TransactionInfo:      new Transaction(
                                                            TransactionId:       evse.TransactionId.Value,
                                                            ChargingState:       ChargingStates.Charging,
                                                            TimeSpentCharging:   TimeSpan.Zero,
                                                            StoppedReason:       null,
                                                            RemoteStartId:       request.RequestStartTransactionRequestId,
                                                            CustomData:          null
                                                        ),

                                  Offline:              false,
                                  NumberOfPhasesUsed:   3,
                                  CableMaxCurrent:      Ampere.ParseA(32),
                                  ReservationId:        evse.ReservationId,
                                  IdToken:              evse.IdToken,
                                  EVSE:                 new EVSE(
                                                            Id:            evse.Id,
                                                            ConnectorId:   evse.Connectors.First().Id,
                                                            CustomData:    null
                                                        ),
                                  MeterValues:          new[] {
                                                            new MeterValue(
                                                                Timestamp:       evse.StartTimestamp.Value,
                                                                SampledValues:   new[] {
                                                                                     new SampledValue(
                                                                                         Value:                 evse.MeterStartValue.Value,
                                                                                         Context:               ReadingContext.TransactionBegin,
                                                                                         Measurand:             Measurand.Current_Export,
                                                                                         Phase:                 null,
                                                                                         MeasurementLocation:   MeasurementLocation.Outlet,
                                                                                         SignedMeterValue:      new SignedMeterValue(
                                                                                                                    SignedMeterData:   evse.SignedStartMeterValue,
                                                                                                                    SigningMethod:     "secp256r1",
                                                                                                                    EncodingMethod:    "base64",
                                                                                                                    PublicKey:         "04cafebabe",
                                                                                                                    CustomData:        null
                                                                                                                ),
                                                                                         UnitOfMeasure:         null,
                                                                                         CustomData:            null
                                                                                     )
                                                                                 }
                                                            )
                                                        },
                                  CustomData:           null

                              );

                    },
                    CancellationToken.None);

                    response = new RequestStartTransactionResponse(
                                   Request:         request,
                                   Status:          RequestStartStopStatus.Accepted,
                                   TransactionId:   evse.TransactionId,
                                   StatusInfo:      null,
                                   CustomData:      null
                               );

                }
                else
                    response = new RequestStartTransactionResponse(
                                   request,
                                   RequestStartStopStatus.Rejected
                               );

                return Task.FromResult(response);

            };

            #endregion

            #region OnRequestStopTransaction

            OCPP.IN.OnRequestStopTransaction += (timestamp,
                                                 sender,
                                                 connection,
                                                 request,
                                                 ct) => {

                RequestStopTransactionResponse? response = null;

                DebugX.Log($"Charging station '{Id}': Incoming RequestStopTransaction for '{request.TransactionId}'!");

                // TransactionId

                // ToDo: lock(evses)

                var evse = evses.Values.FirstOrDefault(evse => request.TransactionId == evse.TransactionId);

                if (evse is not null)
                {

                    evse.IsCharging             = false;

                    evse.StopTimestamp          = Timestamp.Now;
                    evse.MeterStopValue         = 123;
                    evse.SignedStopMeterValue   = "123";

                    _ = Task.Run(async () => {

                        await this.SendTransactionEvent(

                                  EventType:            TransactionEvents.Ended,
                                  Timestamp:            evse.StopTimestamp.Value,
                                  TriggerReason:        TriggerReason.RemoteStop,
                                  SequenceNumber:       2,
                                  TransactionInfo:      new Transaction(
                                                            TransactionId:       evse.TransactionId!.Value,
                                                            ChargingState:       ChargingStates.Idle,
                                                            TimeSpentCharging:   evse.StopTimestamp - evse.StartTimestamp,
                                                            StoppedReason:       StopTransactionReason.Remote,
                                                            RemoteStartId:       evse.RemoteStartId,
                                                            CustomData:          null
                                                        ),

                                  Offline:              false,
                                  NumberOfPhasesUsed:   3,
                                  CableMaxCurrent:      Ampere.ParseA(32),
                                  ReservationId:        evse.ReservationId,
                                  IdToken:              evse.IdToken,
                                  EVSE:                 new EVSE(
                                                            Id:            evse.Id,
                                                            ConnectorId:   evse.Connectors.First().Id,
                                                            CustomData:    null
                                                        ),
                                  MeterValues:          [
                                                            new MeterValue(
                                                                Timestamp:       evse.StopTimestamp.Value,
                                                                SampledValues:   [
                                                                                     new SampledValue(
                                                                                         Value:                 evse.MeterStopValue.Value,
                                                                                         Context:               ReadingContext.TransactionEnd,
                                                                                         Measurand:             Measurand.Current_Export,
                                                                                         Phase:                 null,
                                                                                         MeasurementLocation:   MeasurementLocation.Outlet,
                                                                                         SignedMeterValue:      new SignedMeterValue(
                                                                                                                    SignedMeterData:   evse.SignedStopMeterValue,
                                                                                                                    SigningMethod:     "secp256r1",
                                                                                                                    EncodingMethod:    "base64",
                                                                                                                    PublicKey:         "04cafebabe",
                                                                                                                    CustomData:        null
                                                                                                                ),
                                                                                         UnitOfMeasure:         null,
                                                                                         CustomData:            null
                                                                                     )
                                                                                 ]
                                                            )
                                                        ],
                                  CustomData:           null

                              );

                    },
                    CancellationToken.None);

                    response = new RequestStopTransactionResponse(
                                   request,
                                   RequestStartStopStatus.Accepted
                               );

                }
                else
                    response = new RequestStopTransactionResponse(
                                   request,
                                   RequestStartStopStatus.Rejected
                               );

                return Task.FromResult(response);

            };

            #endregion

            #region OnReserveNow

            OCPP.IN.OnReserveNow += (timestamp,
                                     sender,
                                     connection,
                                     request,
                                     ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming ReserveNow request (reservation id: {request.Id}, idToken: '{request.IdToken.Value}'{(request.EVSEId.HasValue ? $", evseId: '{request.EVSEId.Value}'" : "")})!");

                // ReservationId
                // ExpiryDate
                // IdToken
                // ConnectorType
                // EVSEId
                // GroupIdToken

                var success = reservations.TryAdd(request.Id,
                                                  request.Id);

                return Task.FromResult(
                           new ReserveNowResponse(
                               Request:      request,
                               Status:       success
                                                 ? ReservationStatus.Accepted
                                                 : ReservationStatus.Rejected,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #endregion


            #region OnGetCompositeSchedule

            OCPP.IN.OnGetCompositeSchedule += (timestamp,
                                               sender,
                                               connection,
                                               request,
                                               ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming GetCompositeSchedule request for the next {request.Duration.TotalMinutes} minutes of EVSE '{request.EVSEId}'!");

                // Duration,
                // EVSEId,
                // ChargingRateUnit

                return Task.FromResult(
                           new GetCompositeScheduleResponse(
                               Request:      request,
                               Status:       GenericStatus.Accepted,
                               Schedule:     null,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnNotifyAllowedEnergyTransfer

            OCPP.IN.OnNotifyAllowedEnergyTransfer += (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming NotifyAllowedEnergyTransfer request allowing energy transfer modes: '{request.AllowedEnergyTransferModes.Select(mode => mode.ToString()).AggregateWith(", ")}'!");

                // AllowedEnergyTransferModes

                return Task.FromResult(
                           new NotifyAllowedEnergyTransferResponse(
                               Request:      request,
                               Status:       NotifyAllowedEnergyTransferStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnUnlockConnector

            OCPP.IN.OnUnlockConnector +=  (timestamp,
                                           sender,
                                           connection,
                                           request,
                                           ct) => {

                UnlockConnectorResponse? response = null;

                DebugX.Log($"Charging station '{Id}': Incoming UnlockConnector request for EVSE '{request.EVSEId}' and connector '{request.ConnectorId}'!");

                // EVSEId
                // ConnectorId

                // ToDo: lock(connectors)

                if (evses.TryGetValue    (request.EVSEId,      out var evse) &&
                    evse. TryGetConnector(request.ConnectorId, out var connector))
                {

                    // What to do here?!

                    response = new UnlockConnectorResponse(
                                   Request:      request,
                                   Status:       UnlockStatus.Unlocked,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }
                else
                    response = new UnlockConnectorResponse(
                                   Request:      request,
                                   Status:       UnlockStatus.UnlockFailed,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                return Task.FromResult(response);

            };

            #endregion

            #region OnUpdateDynamicSchedule

            OCPP.IN.OnUpdateDynamicSchedule += (timestamp,
                                                sender,
                                                connection,
                                                request,
                                                ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming UpdateDynamicSchedule request for charging profile '{request.ChargingProfileId}'!");

                // ChargingProfileId

                // Limit
                // Limit_L2
                // Limit_L3

                // DischargeLimit
                // DischargeLimit_L2
                // DischargeLimit_L3

                // Setpoint
                // Setpoint_L2
                // Setpoint_L3

                // SetpointReactive
                // SetpointReactive_L2
                // SetpointReactive_L3

                return Task.FromResult(
                           new UpdateDynamicScheduleResponse(
                               Request:      request,
                               Status:       ChargingProfileStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnUsePriorityCharging

            OCPP.IN.OnUsePriorityCharging += (timestamp,
                                              sender,
                                              connection,
                                              request,
                                              ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming UsePriorityCharging request for transaction '{request.TransactionId}': {(request.Activate ? "active" : "disabled")}!");

                // TransactionId
                // Activate

                return Task.FromResult(
                           new UsePriorityChargingResponse(
                               Request:      request,
                               Status:       GenericStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #endregion

            #region Common

            #region BinaryDataStreamsExtensions

            #region Files

            #region OnDeleteFile

            OCPP.IN.OnDeleteFile += (timestamp,
                                     sender,
                                     connection,
                                     request,
                                     ct) => {

                DebugX.Log($"Charging Station '{Id}': Incoming DeleteFile request: {request.FileName}!");

                return Task.FromResult(
                           request.FileName.ToString() == "/hello/world.txt"

                               ? new DeleteFileResponse(
                                     Request:      request,
                                     FileName:     request.FileName,
                                     Status:       DeleteFileStatus.Success,
                                     CustomData:   null
                                 )

                               : new DeleteFileResponse(
                                     Request:      request,
                                     FileName:     request.FileName,
                                     Status:       DeleteFileStatus.NotFound,
                                     CustomData:   null
                                 )
                       );

            };


            OCPP.FORWARD.OnDeleteFileRequestFilter += (timestamp,
                                                       sender,
                                                       connection,
                                                       request,
                                                       cancellationToken) =>

                Task.FromResult(
                    RequestForwardingDecision<DeleteFileRequest, DeleteFileResponse>.FORWARD(request)
                );

            #endregion

            #region OnGetFile

            OCPP.IN.OnGetFile += (timestamp,
                                  sender,
                                  connection,
                                  request,
                                  ct) => {


                DebugX.Log($"Charging Station '{Id}' GetFile: {request.FileName}!");

                var fileContent = "Hello world!".ToUTF8Bytes();

                return Task.FromResult(
                           request.FileName.ToString() == "/hello/world.txt"

                               ? new GetFileResponse(
                                     Request:           request,
                                     FileName:          request.FileName,
                                     Status:            GetFileStatus.Success,
                                     FileContent:       fileContent,
                                     FileContentType:   ContentType.Text.Plain,
                                     FileSHA256:        SHA256.HashData(fileContent),
                                     FileSHA512:        SHA512.HashData(fileContent)
                                 )

                               : new GetFileResponse(
                                     Request:           request,
                                     FileName:          request.FileName,
                                     Status:            GetFileStatus.NotFound
                                 )
                       );

            };

            #endregion

            #region OnListDirectory

            OCPP.IN.OnListDirectory += (timestamp,
                                        sender,
                                        connection,
                                        request,
                                        ct) => {

                var directoryListing = new DirectoryListing();
                directoryListing.AddFile("/hello/world.txt");

                DebugX.Log($"Charging Station '{Id}': Incoming ListDirectory request: {request.DirectoryPath}!");

                return Task.FromResult(
                           request.DirectoryPath.ToString() == "/hello"

                               ? new ListDirectoryResponse(
                                     Request:            request,
                                     DirectoryPath:      request.DirectoryPath,
                                     Status:             ListDirectoryStatus.Success,
                                     DirectoryListing:   new DirectoryListing(),
                                     CustomData:         null
                                 )

                               : new ListDirectoryResponse(
                                     Request:            request,
                                     DirectoryPath:      request.DirectoryPath,
                                     Status:             ListDirectoryStatus.NotFound,
                                     DirectoryListing:   null,
                                     CustomData:         null
                                 )
                       );

            };

            #endregion

            #region OnSendFile

            OCPP.IN.OnSendFile += (timestamp,
                                   sender,
                                   connection,
                                   request,
                                   ct) => {

                DebugX.Log($"Charging Station '{Id}': Incoming SendFile request: {request.FileName}!");

                return Task.FromResult(
                           request.FileName.ToString() == "/hello/world.txt"

                               ? new SendFileResponse(
                                     Request:      request,
                                     FileName:     request.FileName,
                                     Status:       SendFileStatus.Success,
                                     CustomData:   null
                                 )

                               : new SendFileResponse(
                                     Request:      request,
                                     FileName:     request.FileName,
                                     Status:       SendFileStatus.NotFound,
                                     CustomData:   null
                                 )
                       );

            };

            #endregion

            #endregion


            #region OnBinaryDataTransfer

            OCPP.IN.OnBinaryDataTransfer += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             ct) => {

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


            OCPP.FORWARD.OnBinaryDataTransferRequestFilter += (timestamp,
                                                               sender,
                                                               connection,
                                                               request,
                                                               cancellationToken) =>

                Task.FromResult(
                    new RequestForwardingDecision<BinaryDataTransferRequest, BinaryDataTransferResponse>(
                        request,
                        ForwardingDecisions.FORWARD
                    )
                );

            #endregion

            #endregion

            #region E2E Security Extensions

            #region SignaturePolicy

            #region OnAddSignaturePolicy

            OCPP.IN.OnAddSignaturePolicy += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming AddSignaturePolicy!");

                // Message

                return Task.FromResult(
                           new AddSignaturePolicyResponse(
                               Request:      request,
                               Status:       GenericStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnUpdateSignaturePolicy

            OCPP.IN.OnUpdateSignaturePolicy += (timestamp,
                                                sender,
                                                connection,
                                                request,
                                                ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming UpdateSignaturePolicy!");

                // Message

                return Task.FromResult(
                           new UpdateSignaturePolicyResponse(
                               Request:      request,
                               Status:       GenericStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnDeleteSignaturePolicy

            OCPP.IN.OnDeleteSignaturePolicy += (timestamp,
                                                sender,
                                                connection,
                                                request,
                                                ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming DeleteSignaturePolicy!");

                // Message

                return Task.FromResult(
                           new DeleteSignaturePolicyResponse(
                               Request:      request,
                               Status:       GenericStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #endregion

            #region UserRoles

            #region OnAddUserRole

            OCPP.IN.OnAddUserRole += (timestamp,
                                      sender,
                                      connection,
                                      request,
                                      ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming AddUserRole!");

                // Message

                return Task.FromResult(
                           new AddUserRoleResponse(
                               Request:      request,
                               Status:       GenericStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnUpdateUserRole

            OCPP.IN.OnUpdateUserRole += (timestamp,
                                         sender,
                                         connection,
                                         request,
                                         ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming UpdateUserRole!");

                // Message

                return Task.FromResult(
                           new UpdateUserRoleResponse(
                               Request:      request,
                               Status:       GenericStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnDeleteUserRole

            OCPP.IN.OnDeleteUserRole += (timestamp,
                                         sender,
                                         connection,
                                         request,
                                         ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming DeleteUserRole!");

                // Message

                return Task.FromResult(
                           new DeleteUserRoleResponse(
                               Request:      request,
                               Status:       GenericStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );


            };

            #endregion

            #endregion

            #region OnSecureDataTransfer

            OCPP.IN.OnSecureDataTransfer += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             ct) => {

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
                                     Destination:            SourceRouting.To(request.NetworkPath.Source),
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

            #endregion


            #region OnDataTransfer

            OCPP.IN.OnDataTransfer += (timestamp,
                                       sender,
                                       connection,
                                       request,
                                       ct) => {

                DebugX.Log($"Charging Station '{Id}': Incoming DataTransfer: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data?.ToString() ?? "-"}!");

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


                return Task.FromResult(response);

            };


            OCPP.FORWARD.OnDataTransferRequestFilter += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         ct) => {

                if (request.Data?.ToString() == "Please REJECT!")
                {

                    var response = new DataTransferResponse(
                                       request,
                                       DataTransferStatus.Rejected,
                                       Result: Result.Filtered("This message is not allowed!")
                                   );

                    return Task.FromResult(
                               new RequestForwardingDecision<DataTransferRequest, DataTransferResponse>(
                                   request,
                                   ForwardingDecisions.REJECT,
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
                               new RequestForwardingDecision<DataTransferRequest, DataTransferResponse>(
                                   request,
                                   ForwardingDecisions.FORWARD
                               )
                           );

            };

            #endregion

            #region OnMessageTransfer

            OCPP.IN.OnMessageTransferMessageReceived += (timestamp,
                                                         sender,
                                                         connection,
                                                         request,
                                                         ct) => {

                DebugX.Log($"Charging Station '{Id}': Incoming MessageTransfer: {request.VendorId}.{request.MessageId?.ToString() ?? "-"}: {request.Data?.ToString() ?? "-"}!");

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

                return Task.CompletedTask;

            };


            OCPP.FORWARD.OnMessageTransferMessageFilter += (timestamp,
                                                            sender,
                                                            connection,
                                                            request,
                                                            ct) => {

                if (request.Data?.ToString() == "Please REJECT!")
                {

                    //var response = new DataTransferResponse(
                    //                   request,
                    //                   DataTransferStatus.Rejected,
                    //                   Result: Result.Filtered("This message is not allowed!")
                    //               );

                    return Task.FromResult(
                               new MessageForwardingDecision<MessageTransferMessage>(
                                   request,
                                   ForwardingDecisions.REJECT,
                                   //response,
                                   //response.ToJSON(
                                   //    OCPP.CustomDataTransferResponseSerializer,
                                   //    OCPP.CustomStatusInfoSerializer,
                                   //    OCPP.CustomSignatureSerializer,
                                   //    OCPP.CustomCustomDataSerializer
                                   //),
                                   RejectMessage: "The message was REJECTED!"
                               )
                           );

                }

                else
                    return Task.FromResult(
                               MessageForwardingDecision<MessageTransferMessage>.FORWARD(request)
                           );

            };

            #endregion

            #endregion

            #region Customer

            #region OnClearDisplayMessage

            OCPP.IN.OnClearDisplayMessage += (timestamp,
                                              sender,
                                              connection,
                                              request,
                                              ct) => {

                ClearDisplayMessageResponse? response = null;

                DebugX.Log($"Charging station '{Id}': Incoming ClearDisplayMessage request ({request.DisplayMessageId})!");

                // DisplayMessageId

                if (displayMessages.TryGetValue(request.DisplayMessageId, out var messageInfo) &&
                    displayMessages.TryRemove(new KeyValuePair<DisplayMessage_Id, MessageInfo>(request.DisplayMessageId, messageInfo))) {

                    response = new ClearDisplayMessageResponse(
                                   request,
                                   ClearMessageStatus.Accepted
                               );

                }

                else
                    response = new ClearDisplayMessageResponse(
                                   request,
                                   ClearMessageStatus.Unknown
                               );

                return Task.FromResult(response);

            };

            #endregion

            #region OnCostUpdated

            OCPP.IN.OnCostUpdated += (timestamp,
                                      sender,
                                      connection,
                                      request,
                                      ct) => {

                CostUpdatedResponse? response = null;

                DebugX.Log($"Charging station '{Id}': Incoming CostUpdated request '{request.TotalCost}' for transaction '{request.TransactionId}'!");

                // TotalCost
                // TransactionId

                if (transactions.ContainsKey(request.TransactionId)) {

                    totalCosts.AddOrUpdate(request.TransactionId,
                                           request.TotalCost,
                                           (transactionId, totalCost) => request.TotalCost);

                    response = new CostUpdatedResponse(
                                   request
                               );

                }

                else
                    response = new CostUpdatedResponse(
                                   request,
                                   Result.GenericError($"Unknown transaction identification '{request.TransactionId}'!")
                               );

                return Task.FromResult(response);

            };

            #endregion

            #region OnCustomerInformation

            OCPP.IN.OnCustomerInformation += (timestamp,
                                              sender,
                                              connection,
                                              request,
                                              ct) => {

                    var command   = new String[] {

                                        request.Report
                                            ? "report"
                                            : "",

                                        request.Clear
                                            ? "clear"
                                            : "",

                                    }.Where(text => text.IsNotNullOrEmpty()).
                                      AggregateWith(" and ");

                    var customer  = request.IdToken is not null
                                       ? $"IdToken: {request.IdToken.Value}"
                                       : request.CustomerCertificate is not null
                                             ? $"certificate s/n: {request.CustomerCertificate.SerialNumber}"
                                             : request.CustomerIdentifier.HasValue
                                                   ? $"customer identifier: {request.CustomerIdentifier.Value}"
                                                   : "-";


                    DebugX.Log($"Charging station '{Id}': Incoming CustomerInformation request ({request.CustomerInformationRequestId}) to {command} for customer '{customer}'!");

                    // CustomerInformationRequestId
                    // Report
                    // Clear
                    // CustomerIdentifier
                    // IdToken
                    // CustomerCertificate

                    _ = Task.Run(async () => {

                        await this.NotifyCustomerInformation(
                                  NotifyCustomerInformationRequestId:   request.CustomerInformationRequestId,
                                  Data:                                 customer,
                                  SequenceNumber:                       1,
                                  GeneratedAt:                          Timestamp.Now,
                                  ToBeContinued:                        false,
                                  CustomData:                           null
                              );

                    },
                    CancellationToken.None);

                    return Task.FromResult(
                               new CustomerInformationResponse(
                                   request,
                                   CustomerInformationStatus.Accepted
                               )
                           );

            };

            #endregion

            #region OnGetDisplayMessages

            OCPP.IN.OnGetDisplayMessages += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             ct) => {

                    DebugX.Log($"Charging station '{Id}': Incoming GetDisplayMessages request ({request.GetDisplayMessagesRequestId})!");

                    // GetDisplayMessagesRequestId
                    // Ids
                    // Priority
                    // State

                    _ = Task.Run(async () => {

                        var filteredDisplayMessages = displayMessages.Values.
                                                          Where(displayMessage =>  request.Ids is null || !request.Ids.Any() || request.Ids.Contains(displayMessage.Id)).
                                                          Where(displayMessage => !request.State.   HasValue || (displayMessage.State.HasValue && displayMessage.State.Value == request.State.   Value)).
                                                          Where(displayMessage => !request.Priority.HasValue ||  displayMessage.Priority                                     == request.Priority.Value).
                                                          ToArray();

                        await this.NotifyDisplayMessages(
                                  NotifyDisplayMessagesRequestId:   request.GetDisplayMessagesRequestId,
                                  MessageInfos:                     filteredDisplayMessages,
                                  ToBeContinued:                    false,
                                  CustomData:                       null
                              );

                    },
                    CancellationToken.None);

                    return Task.FromResult(
                               new GetDisplayMessagesResponse(
                                   request,
                                   GetDisplayMessagesStatus.Accepted
                               )
                           );

            };

            #endregion

            #region OnSetDisplayMessage

            OCPP.IN.OnSetDisplayMessage += (timestamp,
                                            sender,
                                            connection,
                                            request,
                                            ct) => {

                SetDisplayMessageResponse? response = null;

                DebugX.Log($"Charging station '{Id}': Incoming SetDisplayMessage '{request.Message.Message.Content}'!");

                // Message

                if (displayMessages.TryAdd(request.Message.Id,
                                           request.Message)) {

                    response = new SetDisplayMessageResponse(
                                   Request:      request,
                                   Status:       DisplayMessageStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }

                else
                    response = new SetDisplayMessageResponse(
                                   Request:      request,
                                   Status:       DisplayMessageStatus.Rejected,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                return Task.FromResult(response);

            };

            #endregion

            #endregion

            #region DeviceModel

            #region VariableMonitoring

            #region OnSetMonitoringBase

            OCPP.IN.OnSetMonitoringBase += (timestamp,
                                            sender,
                                            connection,
                                            request,
                                            ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming SetMonitoringBase request accepted.");

                // MonitoringBase

                return Task.FromResult(
                           new SetMonitoringBaseResponse(
                               Request:      request,
                               Status:       GenericDeviceModelStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnGetMonitoringReport

            OCPP.IN.OnGetMonitoringReport += (timestamp,
                                              sender,
                                              connection,
                                              request,
                                              ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming GetMonitoringReport request ({request.GetMonitoringReportRequestId}) accepted.");

                // GetMonitoringReportRequestId
                // MonitoringCriteria
                // ComponentVariables

                return Task.FromResult(
                           new GetMonitoringReportResponse(
                               Request:      request,
                               Status:       GenericDeviceModelStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnSetMonitoringLevel

            OCPP.IN.OnSetMonitoringLevel += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming SetMonitoringLevel request accepted.");

                // Severity

                return Task.FromResult(
                           new SetMonitoringLevelResponse(
                               Request:      request,
                               Status:       GenericStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnSetVariableMonitoring

            OCPP.IN.OnSetVariableMonitoring += (timestamp,
                                                sender,
                                                connection,
                                                request,
                                                ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming SetMonitoringLevel request accepted.");

                // MonitoringData

                return Task.FromResult(
                           new SetVariableMonitoringResponse(
                               Request:                request,
                               SetMonitoringResults:   request.MonitoringData.Select(setMonitoringData => new SetMonitoringResult(
                                                                                                              Status:                 SetMonitoringStatus.Accepted,
                                                                                                              MonitorType:            setMonitoringData.MonitorType,
                                                                                                              Severity:               setMonitoringData.Severity,
                                                                                                              Component:              setMonitoringData.Component,
                                                                                                              Variable:               setMonitoringData.Variable,
                                                                                                              VariableMonitoringId:   setMonitoringData.VariableMonitoringId,
                                                                                                              StatusInfo:             null,
                                                                                                              CustomData:             null
                                                                                                          )),
                               CustomData:             null
                           )
                       );

            };

            #endregion

            #region OnClearVariableMonitoring

            OCPP.IN.OnClearVariableMonitoring += (timestamp,
                                                  sender,
                                                  connection,
                                                  request,
                                                  ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming ClearVariableMonitoring request (VariableMonitoringIds: {request.VariableMonitoringIds.AggregateWith(", ")})");

                // VariableMonitoringIds

                return Task.FromResult(
                           new ClearVariableMonitoringResponse(
                               Request:                  request,
                               ClearMonitoringResults:   request.VariableMonitoringIds.Select(variableMonitoringId => new ClearMonitoringResult(
                                                                                                                          Status:       ClearMonitoringStatus.Accepted,
                                                                                                                          Id:           variableMonitoringId,
                                                                                                                          StatusInfo:   null,
                                                                                                                          CustomData:   null
                                                                                                                      )),
                               CustomData:               null
                           )
                       );

            };

            #endregion

            #endregion

            #region Variables

            #region OnGetBaseReport

            OCPP.IN.OnGetBaseReport += (timestamp,
                                        sender,
                                        connection,
                                        request,
                                        ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming GetBaseReport request ({request.GetBaseReportRequestId}) accepted.");

                // GetBaseReportRequestId
                // ReportBase

                return Task.FromResult(
                           new GetBaseReportResponse(
                               Request:      request,
                               Status:       GenericDeviceModelStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnGetReport

            OCPP.IN.OnGetReport += (timestamp,
                                    sender,
                                    connection,
                                    request,
                                    ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming GetReport request ({request.GetReportRequestId}) accepted.");

                // GetReportRequestId
                // ComponentCriteria
                // ComponentVariables

                return Task.FromResult(
                           new GetReportResponse(
                               Request:      request,
                               Status:       GenericDeviceModelStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnSetVariables

            OCPP.IN.OnSetVariables += (timestamp,
                                       sender,
                                       connection,
                                       request,
                                       ct) => {

                DebugX.Log($"Charging station '{Id}': {request.VariableData.Count()} SetVariables request(s)");

                #region Require 'admin' user role / digital signature

                var userRoles = GetValidUserRolesFor(request);

                if (userRoles.IsMissing(ChargingStationSettings.UserRoles.Admin))
                    return Task.FromResult(
                        new SetVariablesResponse(
                            Request:             request,
                            SetVariableResults:  request.VariableData.Select(
                                                     setVariableData => new SetVariableResult(
                                                                            AttributeStatus:       SetVariableStatus.Rejected,
                                                                            Component:             setVariableData.Component,
                                                                            Variable:              setVariableData.Variable,
                                                                            AttributeType:         setVariableData.AttributeType,
                                                                            AttributeStatusInfo:   new StatusInfo(
                                                                                                       "error",
                                                                                                       $"Access denied!"
                                                                                                   )
                                                                        )
                                                 ),
                            CustomData:          null
                        )
                    );

                #endregion


                #region Independent Requests (default)

                var setVariableResults = new List<SetVariableResult>();

                if (request.DataConsistencyModel == null ||
                         request.DataConsistencyModel == DataConsistencyModel.IndependentRequests)
                {
                    foreach (var setVariableData in request.VariableData)
                    {

                        var componentInstanceFound  = false;
                        var variableFound           = false;
                        var variableInstanceFound   = false;

                        var componentConfigList     = GetComponentConfigs(
                                                          setVariableData.Component.Name,
                                                          setVariableData.Component.EVSE
                                                      );

                        if (componentConfigList is not null)
                        {

                            foreach (var componentConfig in componentConfigList)
                            {
                                if (componentConfig.Instance == setVariableData.Component.Instance)
                                {

                                    componentInstanceFound = true;

                                    foreach (var variableConfig in componentConfig.VariableConfigs)
                                    {

                                        if (variableConfig.Name == setVariableData.Variable.Name)
                                        {

                                            variableFound = true;

                                            if (variableConfig.Instance == setVariableData.Variable.Instance)
                                            {

                                                variableInstanceFound = true;

                                                if (variableConfig.Attributes?.Mutability == MutabilityTypes.ReadOnly)
                                                    setVariableResults.Add(
                                                        new SetVariableResult(
                                                            AttributeStatus:       SetVariableStatus.Rejected,
                                                            Component:             setVariableData.Component,
                                                            Variable:              setVariableData.Variable,
                                                            AttributeType:         variableConfig.Attributes?.Type,
                                                            AttributeStatusInfo:   new StatusInfo(
                                                                                       "error",
                                                                                       $"{setVariableData.Component.Name}{(setVariableData.Component.Instance is not null ? $"({setVariableData.Component.Instance})" : "")}/" +
                                                                                       $"{setVariableData.Variable.Name}{(setVariableData.Variable.Instance is not null ? $"({setVariableData.Variable.Instance})" : "")} is read-only!"
                                                                                   )
                                                        )
                                                    );

                                                else // Mutability == ReadWrite | WriteOnly
                                                {

                                                    //ToDo: Check User Access Rights!

                                                    var rr = variableConfig.Set(setVariableData.AttributeValue,
                                                                                setVariableData.OldAttributeValue);

                                                    if (rr.ErrorMessage is not null)
                                                        setVariableResults.Add(
                                                            new SetVariableResult(
                                                                AttributeStatus:       SetVariableStatus.Rejected,
                                                                Component:             setVariableData.Component,
                                                                Variable:              setVariableData.Variable,
                                                                AttributeType:         variableConfig.Attributes?.Type,
                                                                AttributeStatusInfo:   new StatusInfo("error", rr.ErrorMessage),
                                                                CustomData:            null
                                                            )
                                                        );

                                                    else
                                                        setVariableResults.Add(
                                                            new SetVariableResult(
                                                                AttributeStatus:       SetVariableStatus.Accepted,
                                                                Component:             setVariableData.Component,
                                                                Variable:              setVariableData.Variable,
                                                                AttributeType:         variableConfig.Attributes?.Type,
                                                                AttributeStatusInfo:   null,
                                                                CustomData:            null
                                                            )
                                                        );

                                                }

                                            }

                                        }

                                    }

                                    if (!variableFound)
                                        setVariableResults.Add(
                                            new SetVariableResult(
                                                AttributeStatus:   SetVariableStatus.UnknownVariable,
                                                Component:         setVariableData.Component,
                                                Variable:          setVariableData.Variable
                                            )
                                        );

                                    else if (!variableInstanceFound)
                                        setVariableResults.Add(
                                            new SetVariableResult(
                                                AttributeStatus:   SetVariableStatus.UnknownVariable,
                                                Component:         setVariableData.Component,
                                                Variable:          setVariableData.Variable
                                            )
                                        );

                                }
                            }

                            if (!componentInstanceFound)
                                setVariableResults.Add(
                                    new SetVariableResult(
                                        AttributeStatus:   SetVariableStatus.UnknownComponent,
                                        Component:         setVariableData.Component,
                                        Variable:          setVariableData.Variable
                                    )
                                );

                        }

                        else
                            setVariableResults.Add(
                                new SetVariableResult(
                                    AttributeStatus:      SetVariableStatus.UnknownComponent,
                                    Component:            setVariableData.Component,
                                    Variable:             setVariableData.Variable,
                                    AttributeStatusInfo:  new StatusInfo(
                                                              "error",
                                                              $"Unknown component '{setVariableData.Component.Name}'!"
                                                          )
                                )
                            );

                    }
                }

                #endregion

                #region BASE

                else if (request.DataConsistencyModel == DataConsistencyModel.BASE)
                {

                }

                #endregion

                #region ACID

                else if (request.DataConsistencyModel == DataConsistencyModel.ACID)
                {

                }

                #endregion

                #region unknown... reject!

                else
                    setVariableResults.AddRange(
                        request.VariableData.Select(
                            setVariableData => new SetVariableResult(
                                                   AttributeStatus:       SetVariableStatus.Rejected,
                                                   Component:             setVariableData.Component,
                                                   Variable:              setVariableData.Variable,
                                                   AttributeType:         setVariableData.AttributeType,
                                                   AttributeStatusInfo:   new StatusInfo(
                                                                              "error",
                                                                              $"Unknown data consistency model '{request.DataConsistencyModel}'!"
                                                                          )
                                               )
                        )
                    );

                #endregion


                return Task.FromResult(
                           new SetVariablesResponse(
                               Request:             request,
                               SetVariableResults:  setVariableResults,
                               CustomData:          null
                           )
                       );

            };

            #endregion

            #region OnGetVariables

            OCPP.IN.OnGetVariables += (timestamp,
                                       sender,
                                       connection,
                                       request,
                                       ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming GetVariables request accepted.");

                var getVariableResults = new List<GetVariableResult>();

                foreach (var getVariableData in request.VariableData)
                {

                    var componentInstanceFound  = false;
                    var variableFound           = false;
                    var variableInstanceFound   = false;

                    var componentConfigList     = GetComponentConfigs(
                                                      getVariableData.Component.Name,
                                                      getVariableData.Component.EVSE
                                                  );

                    if (componentConfigList.Any())
                    {
                        foreach (var componentConfig in componentConfigList)
                        {
                            if (componentConfig.Instance == getVariableData.Component.Instance)
                            {

                                componentInstanceFound = true;

                                foreach (var variableConfig in componentConfig.VariableConfigs)
                                {

                                    if (variableConfig.Name == getVariableData.Variable.Name)
                                    {

                                        variableFound = true;

                                        if (variableConfig.Instance == getVariableData.Variable.Instance)
                                        {

                                            variableInstanceFound = true;

                                            getVariableResults.Add(new GetVariableResult(
                                                                       AttributeStatus:       GetVariableStatus.Accepted,
                                                                       Component:             getVariableData.Component,
                                                                       Variable:              getVariableData.Variable,
                                                                       AttributeValue:        variableConfig.Value,
                                                                       AttributeType:         variableConfig.Attributes?.Type,
                                                                       AttributeStatusInfo:   null,
                                                                       CustomData:            null
                                                                   ));

                                        }

                                    }

                                }

                                if (!variableFound)
                                    getVariableResults.Add(
                                        new GetVariableResult(
                                            AttributeStatus:      GetVariableStatus.UnknownVariable,
                                            Component:            getVariableData.Component,
                                            Variable:             getVariableData.Variable,
                                            AttributeStatusInfo:  new StatusInfo(
                                                                      "error",
                                                                      $"Variable '{getVariableData.Variable.Name}' of component '{getVariableData.Component}' not found!"
                                                                  )
                                        )
                                    );

                                else if (!variableInstanceFound)
                                    getVariableResults.Add(
                                        new GetVariableResult(
                                            AttributeStatus:      GetVariableStatus.UnknownVariable,
                                            Component:            getVariableData.Component,
                                            Variable:             getVariableData.Variable,
                                            AttributeStatusInfo:  new StatusInfo(
                                                                      "error",
                                                                      $"Instance '{getVariableData.Variable.Instance}' of variable '{getVariableData.Component}' / '{getVariableData.Variable.Name}' not found!"
                                                                  )
                                        )
                                    );

                            }
                        }

                        if (!componentInstanceFound)
                            getVariableResults.Add(
                                new GetVariableResult(
                                    AttributeStatus:      GetVariableStatus.UnknownComponent,
                                    Component:            getVariableData.Component,
                                    Variable:             getVariableData.Variable,
                                    AttributeStatusInfo:  new StatusInfo(
                                                                "error",
                                                                $"Instance of component '{getVariableData.Component.Name}' not found!"
                                                            )
                                )
                            );

                    }

                    else
                        getVariableResults.Add(
                            new GetVariableResult(
                                AttributeStatus:      GetVariableStatus.UnknownComponent,
                                Component:            getVariableData.Component,
                                Variable:             getVariableData.Variable,
                                AttributeStatusInfo:  new StatusInfo(
                                                          "error",
                                                          $"Unknown component '{getVariableData.Component.Name}'!"
                                                      )
                            )
                        );

                }

                return Task.FromResult(
                           new GetVariablesResponse(
                               Request:     request,
                               Results:     getVariableResults,
                               CustomData:  null
                           )
                       );

            };

            #endregion

            #endregion


            #region OnChangeAvailability

            OCPP.IN.OnChangeAvailability += (timestamp,
                                             sender,
                                             connection,
                                             request,
                                             ct) => {

                ChangeAvailabilityResponse? response = null;

                DebugX.Log($"Charging station '{Id}': Incoming ChangeAvailability request {request.OperationalStatus.AsText()}{(request.EVSE is not null ? $" for EVSE '{request.EVSE.Id}'{(request.EVSE.ConnectorId.HasValue ? $"/{request.EVSE.ConnectorId}" : "")}" : "")}!");

                // OperationalStatus
                // EVSE

                // Operational status of the entire charging station
                if (request.EVSE is null)
                {

                    response = new ChangeAvailabilityResponse(
                                   Request:      request,
                                   Status:       ChangeAvailabilityStatus.Accepted,
                                   CustomData:   null
                               );

                }

                // Operational status for an EVSE and maybe a connector
                else
                {

                    if (!evses.TryGetValue(request.EVSE.Id, out var evse))
                    {

                        // Unknown EVSE identification
                        response = new ChangeAvailabilityResponse(
                                       Request:      request,
                                       Status:       ChangeAvailabilityStatus.Rejected,
                                       CustomData:   null
                                   );

                    }
                    else
                    {

                        if (request.EVSE.ConnectorId.HasValue &&
                           !evse.Connectors.Any(connector => connector.Id == request.EVSE.ConnectorId.Value))
                        {

                            // Unknown connector identification
                            response = new ChangeAvailabilityResponse(
                                           Request:      request,
                                           Status:       ChangeAvailabilityStatus.Rejected,
                                           CustomData:   null
                                       );

                        }
                        else
                        {

                            response = new ChangeAvailabilityResponse(
                                           Request:      request,
                                           Status:       ChangeAvailabilityStatus.Accepted,
                                           CustomData:   null
                                       );

                        }

                    }

                }

                return Task.FromResult(response);

            };

            #endregion

            #region OnGetLog

            OCPP.IN.OnGetLog += (timestamp,
                                 sender,
                                 connection,
                                 request,
                                 ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming GetLog request ({request.LogRequestId}) accepted.");

                // LogType
                // LogRequestId
                // Log
                // Retries
                // RetryInterval

                return Task.FromResult(
                           new GetLogResponse(
                               Request:      request,
                               Status:       LogStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnSetNetworkProfile

            OCPP.IN.OnSetNetworkProfile += (timestamp,
                                            sender,
                                            connection,
                                            request,
                                            ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming SetNetworkProfile request for configuration slot {request.ConfigurationSlot}!");

                // ConfigurationSlot
                // NetworkConnectionProfile

                return Task.FromResult(
                           new SetNetworkProfileResponse(
                               Request:      request,
                               Status:       SetNetworkProfileStatus.Accepted,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnTriggerMessage

            OCPP.IN.OnTriggerMessage += (timestamp,
                                         sender,
                                         connection,
                                         request,
                                         ct) => {

                TriggerMessageResponse? response = null;

                DebugX.Log($"Charging station '{Id}': Incoming TriggerMessage request for '{request.RequestedMessage}'{(request.EVSE is not null ? $" at EVSE '{request.EVSE.Id}'" : "")}!");

                // RequestedMessage
                // EVSE

                _ = Task.Run(async () => {

                    if (request.RequestedMessage == MessageTrigger.BootNotification)
                    {
                        await this.SendBootNotification(
                                  BootReason: BootReason.Triggered,
                                  CustomData: null
                              );
                    }


                        // LogStatusNotification
                        // DiagnosticsStatusNotification
                        // FirmwareStatusNotification

                        // Seems not to be allowed any more!
                        //case MessageTriggers.Heartbeat:
                        //    await this.SendHeartbeat(
                        //              CustomData:   null
                        //          );
                        //    break;

                        // MeterValues
                        // SignChargingStationCertificate

                    else if (request.RequestedMessage == MessageTrigger.StatusNotification &&
                             request.EVSE is not null)
                    {
                        await this.SendStatusNotification(
                                  EVSEId:        request.EVSE.Id,
                                  ConnectorId:   Connector_Id.Parse(1),
                                  Timestamp:     Timestamp.Now,
                                  Status:        evses[request.EVSE.Id].Status,
                                  CustomData:    null
                              );
                    }

                },
                CancellationToken.None);


                if (request.RequestedMessage == MessageTrigger.BootNotification ||
                    request.RequestedMessage == MessageTrigger.LogStatusNotification ||
                    request.RequestedMessage == MessageTrigger.DiagnosticsStatusNotification ||
                    request.RequestedMessage == MessageTrigger.FirmwareStatusNotification ||
                  //MessageTriggers.Heartbeat
                    request.RequestedMessage == MessageTrigger.SignChargingStationCertificate)
                {

                    response = new TriggerMessageResponse(
                                   request,
                                   TriggerMessageStatus.Accepted
                               );

                }



                if (response == null &&
                   (request.RequestedMessage == MessageTrigger.MeterValues ||
                    request.RequestedMessage == MessageTrigger.StatusNotification))
                {

                    response = request.EVSE is not null

                                   ? new TriggerMessageResponse(
                                         request,
                                         TriggerMessageStatus.Accepted
                                     )

                                   : new TriggerMessageResponse(
                                         request,
                                         TriggerMessageStatus.Rejected
                                     );

                }

                response ??= new TriggerMessageResponse(
                                 request,
                                 TriggerMessageStatus.Rejected
                             );

                return Task.FromResult(response);

            };

            #endregion

            #endregion

            #region Firmware

            #region OnPublishFirmware

            OCPP.IN.OnPublishFirmware += (timestamp,
                                          sender,
                                          connection,
                                          request,
                                          ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming PublishFirmware request ({request.PublishFirmwareRequestId}) for '" + request.DownloadLocation + "'.");

                // PublishFirmwareRequestId
                // DownloadLocation
                // MD5Checksum
                // Retries
                // RetryInterval

                return Task.FromResult(
                           new PublishFirmwareResponse(
                               Request:      request,
                               Status:       GenericStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnReset

            OCPP.IN.OnReset += async (timestamp,
                                      sender,
                                      connection,
                                      request,
                                      ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming '{request.ResetType}' reset request{(request.EVSEId.HasValue ? $" for EVSE '{request.EVSEId}" : "")}'!");

                // ResetType

                ResetResponse? response = null;

                // Reset entire charging station
                if (!request.EVSEId.HasValue)
                {

                    response = new ResetResponse(
                                    Request:      request,
                                    Status:       ResetStatus.Accepted,
                                    StatusInfo:   null,
                                    CustomData:   null
                                );

                }

                // Only reset the given EVSE
                else if (evses.ContainsKey(request.EVSEId.Value))
                {

                    response = new ResetResponse(
                                   Request:      request,
                                   Status:       ResetStatus.Accepted,
                                   StatusInfo:   null,
                                   CustomData:   null
                               );

                }

                // Unknown EVSE
                else
                {

                    response = new ResetResponse(
                                    Request:      request,
                                    Status:       ResetStatus.Rejected,
                                    StatusInfo:   null,
                                    CustomData:   null
                                );

                }

                return response;

            };

            #endregion

            #region OnUnpublishFirmware

            OCPP.IN.OnUnpublishFirmware += (timestamp,
                                            sender,
                                            connection,
                                            request,
                                            ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming UnpublishFirmware request for '" + request.MD5Checksum + "'.");

                // MD5Checksum

                return Task.FromResult(
                           new UnpublishFirmwareResponse(
                               Request:      request,
                               Status:       UnpublishFirmwareStatus.Unpublished,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnUpdateFirmware

            OCPP.IN.OnUpdateFirmware += (timestamp,
                                         sender,
                                         connection,
                                         request,
                                         ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming UpdateFirmware request ({request.UpdateFirmwareRequestId}) for '" + request.Firmware.FirmwareURL + "'.");

                // Firmware,
                // UpdateFirmwareRequestId
                // Retries
                // RetryIntervals

                return Task.FromResult(
                           new UpdateFirmwareResponse(
                               Request:      request,
                               Status:       UpdateFirmwareStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #endregion

            #region Grid

            #region OnAFRRSignal

            OCPP.IN.OnAFRRSignal += (timestamp,
                                     sender,
                                     connection,
                                     request,
                                     ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming AFRRSignal '{request.Signal}' for timestamp '{request.ActivationTimestamp}'!");

                // ActivationTimestamp
                // Signal

                return Task.FromResult(
                           new AFRRSignalResponse(
                               Request:      request,
                               Status:       request.ActivationTimestamp < Timestamp.Now - TimeSpan.FromDays(1)
                                                 ? GenericStatus.Rejected
                                                 : GenericStatus.Accepted,
                               StatusInfo:   null,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #endregion

            #region LocalList

            #region OnGetLocalListVersion

            OCPP.IN.OnGetLocalListVersion += (timestamp,
                                              sender,
                                              connection,
                                              request,
                                              ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming GetLocalListVersion request!");

                // none

                return Task.FromResult(
                           new GetLocalListVersionResponse(
                               Request:         request,
                               VersionNumber:   0,
                               CustomData:      null
                           )
                       );

            };

            #endregion

            #region OnSendLocalList

            OCPP.IN.OnSendLocalList += (timestamp,
                                        sender,
                                        connection,
                                        request,
                                        ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming SendLocalList request: '{request.UpdateType.AsText()}' version '{request.VersionNumber}'!");

                // VersionNumber
                // UpdateType
                // LocalAuthorizationList

                return Task.FromResult(
                           new SendLocalListResponse(
                               Request:      request,
                               Status:       SendLocalListStatus.Accepted,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #region OnClearCache

            OCPP.IN.OnClearCache += (timestamp,
                                     sender,
                                     connection,
                                     request,
                                     ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming ClearCache request!");

                // none

                return Task.FromResult(
                           new ClearCacheResponse(
                               Request:      request,
                               Status:       ClearCacheStatus.Accepted,
                               CustomData:   null
                           )
                       );

            };

            #endregion

            #endregion




            // E2E Charging Tariffs Extensions

            #region OnSetDefaultE2EChargingTariff

            OCPP.IN.OnSetDefaultE2EChargingTariff += (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      ct) => {

                SetDefaultE2EChargingTariffResponse? response = null;

                DebugX.Log($"Charging station '{Id}': Incoming SetDefaultChargingTariff!");

                List<EVSEStatusInfo<SetDefaultE2EChargingTariffStatus>>? evseStatusInfos = null;

                if (!request.ChargingTariff.Verify(out var err))
                {
                    response = new SetDefaultE2EChargingTariffResponse(
                                   Request:      request,
                                   Status:       SetDefaultE2EChargingTariffStatus.InvalidSignature,
                                   StatusInfo:   new StatusInfo(
                                                     ReasonCode:       "Invalid charging tariff signature(s)!",
                                                     AdditionalInfo:   err,
                                                     CustomData:       null
                                                 ),
                                   CustomData:   null
                               );
                }

                else if (!request.EVSEIds.Any())
                {

                    foreach (var evse in evses.Values)
                        evse.DefaultChargingTariff = request.ChargingTariff;

                    response = new SetDefaultE2EChargingTariffResponse(
                                   Request:           request,
                                   Status:            SetDefaultE2EChargingTariffStatus.Accepted,
                                   StatusInfo:        null,
                                   EVSEStatusInfos:   null,
                                   CustomData:        null
                               );

                }

                else
                {

                    foreach (var evseId in request.EVSEIds)
                    {
                        if (!evses.ContainsKey(evseId))
                        {
                            response = new SetDefaultE2EChargingTariffResponse(
                                           Request:   request,
                                           Status:    SetDefaultE2EChargingTariffStatus.Rejected,
                                           Result:    Result.GenericError(
                                                          $"Invalid EVSE identification: {evseId}"
                                                      )
                                       );
                        }
                    }

                    if (response == null)
                    {

                        evseStatusInfos = [];

                        foreach (var evseId in request.EVSEIds)
                        {

                            evses[evseId].DefaultChargingTariff = request.ChargingTariff;

                            evseStatusInfos.Add(new EVSEStatusInfo<SetDefaultE2EChargingTariffStatus>(
                                                    EVSEId:           evseId,
                                                    Status:           SetDefaultE2EChargingTariffStatus.Accepted,
                                                    ReasonCode:       null,
                                                    AdditionalInfo:   null,
                                                    CustomData:       null
                                                ));

                        }

                        response = new SetDefaultE2EChargingTariffResponse(
                                       Request:           request,
                                       Status:            SetDefaultE2EChargingTariffStatus.Accepted,
                                       StatusInfo:        null,
                                       EVSEStatusInfos:   evseStatusInfos,
                                       CustomData:        null
                                   );

                    }

                }

                return Task.FromResult(response);

            };

            #endregion

            #region OnGetDefaultChargingTariff

            OCPP.IN.OnGetDefaultChargingTariff += (timestamp,
                                                   sender,
                                                   connection,
                                                   request,
                                                   ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming GetDefaultChargingTariff!");

                var chargingTariffGroups  = request.EVSEIds.Any()
                                                ? evses.Values.Where(evse => request.EVSEIds.Contains(evse.Id)).GroupBy(evse => evse.DefaultChargingTariff?.Id.ToString() ?? "")
                                                : evses.Values.                                                 GroupBy(evse => evse.DefaultChargingTariff?.Id.ToString() ?? "");

                var chargingTariffMap     = chargingTariffGroups.
                                                Where (group => group.Key != "").
                                                Select(group => new KeyValuePair<Tariff_Id, IEnumerable<EVSE_Id>>(
                                                                    group.First().DefaultChargingTariff!.Id,
                                                                    group.Select(evse => evse.Id)
                                                                ));

                return Task.FromResult(
                           new GetDefaultChargingTariffResponse(
                               Request:             request,
                               Status:              GenericStatus.Accepted,
                               StatusInfo:          null,
                               ChargingTariffs:     chargingTariffGroups.
                                                        Where (group => group.Key != "").
                                                        Select(group => group.First().DefaultChargingTariff).
                                                        Cast<Tariff>(),
                               ChargingTariffMap:   chargingTariffMap.Any()
                                                        ? new ReadOnlyDictionary<Tariff_Id, IEnumerable<EVSE_Id>>(
                                                              new Dictionary<Tariff_Id, IEnumerable<EVSE_Id>>(
                                                                  chargingTariffMap
                                                              )
                                                          )
                                                        : null,
                               CustomData:          null
                           )
                       );

            };

            #endregion

            #region OnRemoveDefaultChargingTariff

            OCPP.IN.OnRemoveDefaultChargingTariff += (timestamp,
                                                      sender,
                                                      connection,
                                                      request,
                                                      ct) => {

                DebugX.Log($"Charging station '{Id}': Incoming RemoveDefaultChargingTariff!");

                var evseIds          = request.EVSEIds.Any()
                                           ? request.EVSEIds
                                           : evses.Keys;

                var evseStatusInfos  = new List<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>();

                foreach (var evseId in evseIds)
                {

                    if (evses[evseId].DefaultChargingTariff is null)
                    {
                        evseStatusInfos.Add(new EVSEStatusInfo<RemoveDefaultChargingTariffStatus>(
                                                evseId,
                                                RemoveDefaultChargingTariffStatus.NotFound
                                            ));
                        continue;
                    }

                    if (!request.ChargingTariffId.HasValue)
                    {
                        evses[evseId].DefaultChargingTariff = null;
                        continue;
                    }

                    var chargingTariff = evses[evseId].DefaultChargingTariff;

                    if (chargingTariff is null)
                    {
                        evseStatusInfos.Add(new EVSEStatusInfo<RemoveDefaultChargingTariffStatus>(
                                                evseId,
                                                RemoveDefaultChargingTariffStatus.Accepted
                                            ));
                        continue;
                    }

                    if (chargingTariff.Id == request.ChargingTariffId.Value)
                    {
                        evses[evseId].DefaultChargingTariff = null;
                        evseStatusInfos.Add(new EVSEStatusInfo<RemoveDefaultChargingTariffStatus>(
                                                evseId,
                                                RemoveDefaultChargingTariffStatus.Accepted
                                            ));
                        continue;
                    }

                    evseStatusInfos.Add(new EVSEStatusInfo<RemoveDefaultChargingTariffStatus>(
                                            evseId,
                                            RemoveDefaultChargingTariffStatus.NotFound
                                        ));

                }

                return Task.FromResult(
                           new RemoveDefaultChargingTariffResponse(
                               Request:           request,
                               Status:            RemoveDefaultChargingTariffStatus.Accepted,
                               StatusInfo:        null,
                               EVSEStatusInfos:   evseStatusInfos,
                               CustomData:        null
                           )
                       );

            };

            #endregion


        }

    }

}
