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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.LC;
using cloud.charging.open.protocols.WWCP;
using Newtonsoft.Json.Linq;
using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;


#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    public class ChargingStationOperatorAdapter : IRemoteChargingStationOperator
    {

        public ICSMSNode                 CSMS                       { get; }
        public IChargingStationOperator  ChargingStationOperator    { get; }

        public ChargingStationOperatorAdapter(ICSMSNode                 CSMS,
                                              IChargingStationOperator  ChargingStationOperator)
        {

            this.CSMS                     = CSMS;
            this.ChargingStationOperator  = ChargingStationOperator;

        }

        public event OnRemoteStartRequestDelegate OnRemoteStartRequest;
        public event OnRemoteStartResponseDelegate OnRemoteStartResponse;
        public event OnRemoteStopRequestDelegate OnRemoteStopRequest;
        public event OnRemoteStopResponseDelegate OnRemoteStopResponse;
        public event OnReserveRequestDelegate? OnReserveRequest;
        public event OnReserveResponseDelegate? OnReserveResponse;
        public event OnNewReservationDelegate? OnNewReservation;
        public event OnCancelReservationRequestDelegate? OnCancelReservationRequest;
        public event OnCancelReservationResponseDelegate? OnCancelReservationResponse;
        public event OnReservationCanceledDelegate? OnReservationCanceled;
        public event OnNewChargingSessionDelegate OnNewChargingSession;
        public event OnNewChargeDetailRecordDelegate OnNewChargeDetailRecord;


        public TimeSpan MaxReservationDuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEnumerable<ChargingReservation> ChargingReservations => throw new NotImplementedException();

        public IEnumerable<ChargingSession> ChargingSessions => throw new NotImplementedException();



        public async Task<RemoteStartResult> RemoteStart(ChargingLocation         ChargingLocation,
                                                         ChargingProduct?         ChargingProduct          = null,
                                                         ChargingReservation_Id?  ReservationId            = null,
                                                         ChargingSession_Id?      SessionId                = null,
                                                         EMobilityProvider_Id?    ProviderId               = null,
                                                         RemoteAuthentication?    RemoteAuthentication     = null,
                                                         JObject?                 AdditionalSessionInfos   = null,
                                                         Auth_Path?               AuthenticationPath       = null,

                                                         DateTime?                RequestTimestamp         = null,
                                                         EventTracking_Id?        EventTrackingId          = null,
                                                         TimeSpan?                RequestTimeout           = null,
                                                         CancellationToken        CancellationToken        = default)
        {

            #region Initial checks

            if (ChargingLocation.IsNull())
                return RemoteStartResult.UnknownLocation(System_Id.Local);

            var eMAId = RemoteAuthentication?.RemoteIdentification?.ToString();

            if (eMAId.IsNullOrEmpty())
                return RemoteStartResult.InvalidCredentials(System_Id.Local);

            #endregion


            //var response = await CSMS.StartCharging(
            //                         DestinationId:                      ChargingStationNode.Id,
            //                         RequestStartTransactionRequestId:   RemoteStart_Id.NewRandom,
            //                         IdToken:                            new IdToken(
            //                                                                 Value:             eMAId,
            //                                                                 Type:              IdTokenType.eMAID,
            //                                                                 AdditionalInfos:   null,
            //                                                                 CustomData:        null
            //                                                             ),
            //                         EVSEId:                             EVSE_Id.Parse(0),
            //                         ChargingProfile:                    null,
            //                         GroupIdToken:                       null,
            //                         TransactionLimits:                  null,

            //                         SignKeys:                           null,
            //                         SignInfos:                          null,
            //                         Signatures:                         null,

            //                         CustomData:                         new CustomData(
            //                                                                 VendorId:     Vendor_Id.GraphDefined,
            //                                                                 CustomData:   JSONObject.Create(
            //                                                                                   ProviderId.HasValue
            //                                                                                       ? new JProperty("providerId", ProviderId.ToString())
            //                                                                                       : null
            //                                                                               )
            //                                                             ),

            //                         RequestId:                          null,
            //                         RequestTimestamp:                   null,
            //                         RequestTimeout:                     null,
            //                         EventTrackingId:                    null,
            //                         CancellationToken:                  default
            //                     );

            return RemoteStartResult.OutOfService(System_Id.Local);

        }

























        public Task<RemoteStopResult> RemoteStop(ChargingSession_Id SessionId, ReservationHandling? ReservationHandling = null, EMobilityProvider_Id? ProviderId = null, RemoteAuthentication? RemoteAuthentication = null, Auth_Path? AuthenticationPath = null, DateTime? Timestamp = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null, CancellationToken CancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool TryGetChargingReservationById(ChargingReservation_Id ReservationId, out ChargingReservation? ChargingReservation)
        {
            throw new NotImplementedException();
        }

        public bool TryGetChargingReservationsById(ChargingReservation_Id ReservationId, out ChargingReservationCollection? ChargingReservations)
        {
            throw new NotImplementedException();
        }

        public ChargingReservation? GetChargingReservationById(ChargingReservation_Id ReservationId)
        {
            throw new NotImplementedException();
        }

        public ChargingReservationCollection? GetChargingReservationsById(ChargingReservation_Id ReservationId)
        {
            throw new NotImplementedException();
        }

        public Task<ReservationResult> Reserve(ChargingLocation ChargingLocation, ChargingReservationLevel ReservationLevel = ChargingReservationLevel.EVSE, DateTime? StartTime = null, TimeSpan? Duration = null, ChargingReservation_Id? ReservationId = null, ChargingReservation_Id? LinkedReservationId = null, EMobilityProvider_Id? ProviderId = null, RemoteAuthentication? RemoteAuthentication = null, Auth_Path? AuthenticationPath = null, ChargingProduct? ChargingProduct = null, IEnumerable<AuthenticationToken>? AuthTokens = null, IEnumerable<EMobilityAccount_Id>? eMAIds = null, IEnumerable<uint>? PINs = null, DateTime? Timestamp = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null, CancellationToken CancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CancelReservationResult> CancelReservation(ChargingReservation_Id ReservationId, ChargingReservationCancellationReason Reason, DateTime? Timestamp = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null, CancellationToken CancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool ContainsChargingSessionId(ChargingSession_Id ChargingSessionId)
        {
            throw new NotImplementedException();
        }

        public ChargingSession? GetChargingSessionById(ChargingSession_Id ChargingSessionId)
        {
            throw new NotImplementedException();
        }

        public bool TryGetChargingSessionById(ChargingSession_Id ChargingSessionId, [NotNullWhen(true)] out ChargingSession? ChargingSession)
        {
            throw new NotImplementedException();
        }
    }

}
