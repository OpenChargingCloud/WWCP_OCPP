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

using System.Collections;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.LC;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    public class ChargingStationAdapter : IRemoteChargingStation
    {

        #region Properties

        public ICSMSNode                    CSMS                   { get; }
        public IChargingStation             ChargingStation        { get; }
        public IChargingStationNode         ChargingStationNode    { get; }




        public IRoamingNetwork?             RoamingNetwork
            => ChargingStation.RoamingNetwork;

        public IChargingStationOperator?    Operator
            => ChargingStation.Operator;

        public IChargingStationOperator?    SubOperator
            => ChargingStation.SubOperator;

        public IChargingPool?               ChargingPool
            => ChargingStation.ChargingPool;

        public WWCP.ChargingStation_Id      Id
            => ChargingStation.Id;

        public I18NString                   Name
            => ChargingStation.Name;

        public I18NString                   Description
            => ChargingStation.Description;

        public String?                      DataSource
            => ChargingStation.DataSource;

        public Boolean                      Published
            => ChargingStation.Published;

        public Boolean                      Disabled
            => ChargingStation.Disabled;


        #region POI Data

        public ReactiveSet<Brand> Brands => throw new NotImplementedException();

        public ReactiveSet<OpenDataLicense> DataLicenses => throw new NotImplementedException();

        public Address? Address { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string? OpenStreetMapNodeId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public GeoCoordinate? GeoLocation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Address? EntranceAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public GeoCoordinate? EntranceLocation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public I18NString ArrivalInstructions => throw new NotImplementedException();

        public OpeningTimes OpeningTimes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ReactiveSet<ParkingSpace> ParkingSpaces => throw new NotImplementedException();

        public ReactiveSet<UIFeatures> UIFeatures => throw new NotImplementedException();

        public ReactiveSet<AuthenticationModes> AuthenticationModes => throw new NotImplementedException();

        public ReactiveSet<PaymentOptions> PaymentOptions => throw new NotImplementedException();

        public AccessibilityTypes? Accessibility { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ReactiveSet<ChargingStationFeature> Features => throw new NotImplementedException();

        public String? PhysicalReference => throw new NotImplementedException();

        public ReactiveSet<URL> PhotoURLs => throw new NotImplementedException();

        public PhoneNumber? HotlinePhoneNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Address ExitAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public GeoCoordinate? ExitLocation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsFreeOfCharge { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string? ServiceIdentification { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string? ModelCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string? HubjectStationId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion

        #region Energy Data

        public GridConnectionTypes? GridConnection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        public decimal? MaxCurrent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Timestamped<decimal>? MaxCurrentRealTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ReactiveSet<Timestamped<decimal>> MaxCurrentPrognoses => throw new NotImplementedException();



        public decimal? MaxPower { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Timestamped<decimal>? MaxPowerRealTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ReactiveSet<Timestamped<decimal>> MaxPowerPrognoses => throw new NotImplementedException();



        public decimal? MaxCapacity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Timestamped<decimal>? MaxCapacityRealTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ReactiveSet<Timestamped<decimal>> MaxCapacityPrognoses => throw new NotImplementedException();



        public WWCP.EnergyMix? EnergyMix { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Timestamped<WWCP.EnergyMix>? EnergyMixRealTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public EnergyMixPrognosis? EnergyMixPrognoses { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion


        public WWCP.ChargingStation_Id RemoteChargingStationId
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public String RemoteEVSEIdPrefix
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public IRemoteChargingStation? RemoteChargingStation
            => throw new NotImplementedException();


        #endregion

        #region Events

        public event OnChargingStationDataChangedDelegate?          OnDataChanged;
        public event OnChargingStationStatusChangedDelegate?        OnStatusChanged;
        public event OnChargingStationAdminStatusChangedDelegate?   OnAdminStatusChanged;
        public event OnEVSEDataChangedDelegate?                     OnEVSEDataChanged;
        public event OnEVSEStatusChangedDelegate?                   OnEVSEStatusChanged;
        public event OnEVSEAdminStatusChangedDelegate?              OnEVSEAdminStatusChanged;
        public event OnPropertyChangedDelegate?                     OnPropertyChanged;
        public event OnRemoteStartRequestDelegate?                  OnRemoteStartRequest;
        public event OnRemoteStartResponseDelegate?                 OnRemoteStartResponse;
        public event OnRemoteStopRequestDelegate?                   OnRemoteStopRequest;
        public event OnRemoteStopResponseDelegate?                  OnRemoteStopResponse;
        public event OnReserveRequestDelegate?                      OnReserveRequest;
        public event OnReserveResponseDelegate?                     OnReserveResponse;
        public event OnNewReservationDelegate?                      OnNewReservation;
        public event OnCancelReservationRequestDelegate?            OnCancelReservationRequest;
        public event OnCancelReservationResponseDelegate?           OnCancelReservationResponse;
        public event OnReservationCanceledDelegate?                 OnReservationCanceled;
        public event OnNewChargingSessionDelegate?                  OnNewChargingSession;
        public event OnNewChargeDetailRecordDelegate?               OnNewChargeDetailRecord;
        public event OnAuthorizeStartRequestDelegate?               OnAuthorizeStartRequest;
        public event OnAuthorizeStartResponseDelegate?              OnAuthorizeStartResponse;
        public event OnAuthorizeStopRequestDelegate?                OnAuthorizeStopRequest;
        public event OnAuthorizeStopResponseDelegate?               OnAuthorizeStopResponse;

        #endregion

        #region Constructor(s)

        public ChargingStationAdapter(ICSMSNode             CSMS,
                                      IChargingStation      ChargingStation,
                                      IChargingStationNode  ChargingStationNode)
        {

            this.CSMS                 = CSMS;
            this.ChargingStation      = ChargingStation;
            this.ChargingStationNode  = ChargingStationNode;

        }

        #endregion


        #region Reservations

        public IEnumerable<ChargingReservation> ChargingReservations
            => throw new NotImplementedException();

        public TimeSpan MaxReservationDuration
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public Task<ReservationResult> Reserve(DateTime? StartTime = null, TimeSpan? Duration = null, ChargingReservation_Id? ReservationId = null, ChargingReservation_Id? LinkedReservationId = null, EMobilityProvider_Id? ProviderId = null, RemoteAuthentication? RemoteAuthentication = null, Auth_Path? AuthenticationPath = null, ChargingProduct? ChargingProduct = null, IEnumerable<AuthenticationToken>? AuthTokens = null, IEnumerable<EMobilityAccount_Id>? eMAIds = null, IEnumerable<uint>? PINs = null, DateTime? Timestamp = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null, CancellationToken CancellationToken = default)
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

        #endregion

        #region AuthorizeStart-/Stop

        public Boolean DisableAuthorization
        {

            get => ChargingStation.DisableAuthorization;

            set {
                ChargingStation.DisableAuthorization = value;
            }

        }


        public Task<AuthStartResult> AuthorizeStart(LocalAuthentication LocalAuthentication, ChargingLocation? ChargingLocation = null, ChargingProduct? ChargingProduct = null, ChargingSession_Id? SessionId = null, ChargingSession_Id? CPOPartnerSessionId = null, ChargingStationOperator_Id? OperatorId = null, DateTime? Timestamp = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null, CancellationToken CancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<AuthStopResult> AuthorizeStop(ChargingSession_Id SessionId, LocalAuthentication LocalAuthentication, ChargingLocation? ChargingLocation = null, ChargingSession_Id? CPOPartnerSessionId = null, ChargingStationOperator_Id? OperatorId = null, DateTime? Timestamp = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null, CancellationToken CancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region RemoteStart(                  ChargingProduct, ReservationId, SessionId, ProviderId, RemoteAuthentication, AdditionalSessionInfos, AuthenticationPath, RequestTimestamp, EventTrackingId, RequestTimeout, CancellationToken)

        public Task<RemoteStartResult> RemoteStart(ChargingProduct?         ChargingProduct          = null,
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

            => RemoteStart(null,
                           ChargingProduct,
                           ReservationId,
                           SessionId,
                           ProviderId,
                           RemoteAuthentication,
                           AdditionalSessionInfos,
                           AuthenticationPath,

                           RequestTimestamp,
                           EventTrackingId,
                           RequestTimeout,
                           CancellationToken);

        #endregion

        #region RemoteStart(ChargingLocation, ChargingProduct, ReservationId, SessionId, ProviderId, RemoteAuthentication, AdditionalSessionInfos, AuthenticationPath, RequestTimestamp, EventTrackingId, RequestTimeout, CancellationToken)

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

            var response = await CSMS.StartCharging(

                                     Destination:                        SourceRouting.To(ChargingStationNode.Id),
                                     RequestStartTransactionRequestId:   RemoteStart_Id.NewRandom,
                                     IdToken:                            new IdToken(
                                                                             Value:             eMAId,
                                                                             Type:              IdTokenType.eMAID,
                                                                             AdditionalInfos:   null,
                                                                             CustomData:        null
                                                                         ),
                                     EVSEId:                             EVSE_Id.Parse(1),
                                     ChargingProfile:                    null,
                                     GroupIdToken:                       null,
                                     TransactionLimits:                  null,

                                     SignKeys:                           null,
                                     SignInfos:                          null,
                                     Signatures:                         null,

                                     CustomData:                         new CustomData(
                                                                             VendorId:     Vendor_Id.GraphDefined,
                                                                             CustomData:   JSONObject.Create(

                                                                                               ProviderId.HasValue
                                                                                                   ? new JProperty("providerId",              ProviderId.        ToString())
                                                                                                   : null,

                                                                                               AdditionalSessionInfos is not null
                                                                                                   ? new JProperty("additionalSessionInfos",  AdditionalSessionInfos)
                                                                                                   : null,

                                                                                               AuthenticationPath.HasValue
                                                                                                   ? new JProperty("authenticationPath",      AuthenticationPath.ToString())
                                                                                                   : null

                                                                                           )
                                                                         ),

                                     RequestId:                          null,
                                     RequestTimestamp:                   RequestTimestamp,
                                     RequestTimeout:                     null,
                                     EventTrackingId:                    EventTrackingId,
                                     CancellationToken:                  CancellationToken

                                 );


            var result = response.Status.ToString() switch {

                #region Accepted

                RequestStartStopStatus.Const.Accepted

                    => RemoteStartResult.Success(
                           Session:   new ChargingSession(
                                          Id:                        SessionId ?? ChargingSession_Id.NewRandom(),
                                          EventTrackingId:           EventTracking_Id.New,
                                          RoamingNetwork:            RoamingNetwork,
                                          CSORoamingProviderStart:   null,
                                          EMPRoamingProviderStart:   null,
                                          Timestamp:                 null,
                                          CustomData:                null,
                                          InternalData:              null
                                      ),
                           Sender:    System_Id.Local,
                           Runtime:   null
                       ),

                #endregion

                #region Rejected

                // Sadly this could be anything!
                RequestStartStopStatus.Const.Rejected

                    => RemoteStartResult.Rejected(
                           Sender:    System_Id.Local,
                           Runtime:   null
                       ),

                #endregion

                #region ...or default

                _ => RemoteStartResult.OutOfService(
                         Sender:    System_Id.Local,
                         Runtime:   null
                     )

                #endregion

            };

            return result;

        }

        #endregion

        #region RemoteStop(SessionId, ReservationHandling = null, ProviderId = null, RemoteAuthentication = null, AuthenticationPath = null, RequestTimestamp = null, EventTrackingId = null, RequestTimeout = null, CancellationToken = default)

        public async Task<RemoteStopResult> RemoteStop(ChargingSession_Id     SessionId,
                                                       ReservationHandling?   ReservationHandling    = null,
                                                       EMobilityProvider_Id?  ProviderId             = null,
                                                       RemoteAuthentication?  RemoteAuthentication   = null,
                                                       Auth_Path?             AuthenticationPath     = null,

                                                       DateTime?              RequestTimestamp       = null,
                                                       EventTracking_Id?      EventTrackingId        = null,
                                                       TimeSpan?              RequestTimeout         = null,
                                                       CancellationToken      CancellationToken      = default)
        {

            var response = await CSMS.StopCharging(

                                     Destination:         SourceRouting.To(ChargingStationNode.Id),
                                     TransactionId:       Transaction_Id.Parse(SessionId.ToString()),

                                     SignKeys:            null,
                                     SignInfos:           null,
                                     Signatures:          null,

                                     CustomData:          new CustomData(
                                                              VendorId:     Vendor_Id.GraphDefined,
                                                              CustomData:   JSONObject.Create(

                                                                                RemoteAuthentication?.RemoteIdentification is not null
                                                                                    ? new JProperty("eMAId",        RemoteAuthentication.RemoteIdentification.ToString())
                                                                                    : null,

                                                                                ProviderId.HasValue
                                                                                    ? new JProperty("providerId",   ProviderId.ToString())
                                                                                    : null

                                                                            )
                                                          ),

                                     RequestId:           null,
                                     RequestTimestamp:    null,
                                     RequestTimeout:      null,
                                     EventTrackingId:     EventTrackingId,
                                     CancellationToken:   CancellationToken

                                 );


            var result = response.Status.ToString() switch {

                #region Accepted

                RequestStartStopStatus.Const.Accepted

                    => RemoteStopResult.Success(
                           SessionId:   SessionId,
                           Sender:    System_Id.Local,
                           Runtime:   null
                       ),

                #endregion

                #region Rejected

                // Sadly this could be anything!
                RequestStartStopStatus.Const.Rejected

                    => RemoteStopResult.Rejected(
                           SessionId:   SessionId,
                           Sender:      System_Id.Local,
                           Runtime:     null
                       ),

                #endregion

                #region ...or default

                _ => RemoteStopResult.OutOfService(
                         SessionId:   SessionId,
                         Sender:      System_Id.Local,
                         Runtime:     null
                     )

                #endregion

            };

            return result;

        }

        #endregion


        #region AdminStatus

        public IEnumerable<Timestamped<ChargingStationAdminStatusTypes>> AdminStatusSchedule(Func<DateTime, bool>? TimestampFilter = null, Func<ChargingStationAdminStatusTypes, bool>? AdminStatusFilter = null, ulong? Skip = null, ulong? Take = null)
        {
            throw new NotImplementedException();
        }

        public void SetAdminStatus(ChargingStationAdminStatusTypes NewAdminStatus, Context? DataSource = null)
        {
            throw new NotImplementedException();
        }

        public void SetAdminStatus(Timestamped<ChargingStationAdminStatusTypes> NewTimestampedAdminStatus, Context? DataSource = null)
        {
            throw new NotImplementedException();
        }

        public void SetAdminStatus(IEnumerable<Timestamped<ChargingStationAdminStatusTypes>> NewAdminStatusList, ChangeMethods ChangeMethod = ChangeMethods.Replace, Context? DataSource = null)
        {
            throw new NotImplementedException();
        }

        public void SetAdminStatus(ChargingStationAdminStatusTypes NewAdminStatus, DateTime Timestamp, Context? DataSource = null)
        {
            throw new NotImplementedException();
        }
        public Timestamped<ChargingStationAdminStatusTypes> AdminStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion

        #region Status

        public IEnumerable<Timestamped<ChargingStationStatusTypes>> StatusSchedule(Func<DateTime, bool>? TimestampFilter = null, Func<ChargingStationStatusTypes, bool>? StatusFilter = null, ulong? Skip = null, ulong? Take = null)
        {
            throw new NotImplementedException();
        }

        public void SetStatus(ChargingStationStatusTypes NewStatus, Context? DataSource = null)
        {
            throw new NotImplementedException();
        }

        public void SetStatus(Timestamped<ChargingStationStatusTypes> NewTimestampedStatus, Context? DataSource = null)
        {
            throw new NotImplementedException();
        }

        public void SetStatus(IEnumerable<Timestamped<ChargingStationStatusTypes>> NewStatusList, ChangeMethods ChangeMethod = ChangeMethods.Replace, Context? DataSource = null)
        {
            throw new NotImplementedException();
        }

        public void SetStatus(ChargingStationStatusTypes NewStatus, DateTime Timestamp, Context? DataSource = null)
        {
            throw new NotImplementedException();
        }
        public Timestamped<ChargingStationStatusTypes> Status { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }



        public Func<EVSEStatusReport, ChargingStationStatusTypes> StatusAggregationDelegate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion


        #region EVSEs

        public IEnumerator<IEVSE> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }


        public IVotingSender<DateTime, EventTracking_Id, User_Id, IChargingStation, IEVSE, bool> OnEVSEAddition => throw new NotImplementedException();

        public IVotingSender<DateTime, EventTracking_Id, User_Id, IChargingStation, IEVSE, IEVSE, bool> OnEVSEUpdate => throw new NotImplementedException();

        public IVotingSender<DateTime, EventTracking_Id, User_Id, IChargingStation, IEVSE, bool> OnEVSERemoval => throw new NotImplementedException();

        public IEnumerable<IEVSE> EVSEs => throw new NotImplementedException();


        public IRemoteEVSE AddEVSE(IRemoteEVSE EVSE, Action<IRemoteEVSE>? Configurator = null, Action<IRemoteEVSE>? OnSuccess = null, Action<WWCP.ChargingStation, WWCP.EVSE_Id>? OnError = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EVSEStatus>> GetEVSEStatus(DateTime Timestamp, CancellationToken CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public void AddMapping(WWCP.EVSE_Id LocalEVSEId, WWCP.EVSE_Id RemoteEVSEId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WWCP.EVSE_Id> EVSEIds(IncludeEVSEDelegate? IncludeEVSEs = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EVSEAdminStatus> EVSEAdminStatus(IncludeEVSEDelegate? IncludeEVSEs = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<WWCP.EVSE_Id, IEnumerable<Timestamped<EVSEAdminStatusTypes>>>> EVSEAdminStatusSchedule(IncludeEVSEDelegate? IncludeEVSEs = null, Func<DateTime, bool>? TimestampFilter = null, Func<EVSEAdminStatusTypes, bool>? StatusFilter = null, ulong? Skip = null, ulong? Take = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EVSEStatus> EVSEStatus(IncludeEVSEDelegate? IncludeEVSEs = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<WWCP.EVSE_Id, IEnumerable<Timestamped<EVSEStatusTypes>>>> EVSEStatusSchedule(IncludeEVSEDelegate? IncludeEVSEs = null, Func<DateTime, bool>? TimestampFilter = null, Func<EVSEStatusTypes, bool>? StatusFilter = null, ulong? Skip = null, ulong? Take = null)
        {
            throw new NotImplementedException();
        }

        public bool ContainsEVSE(IEVSE EVSE)
        {
            throw new NotImplementedException();
        }

        public bool ContainsEVSE(WWCP.EVSE_Id EVSEId)
        {
            throw new NotImplementedException();
        }

        public IEVSE GetEVSEById(WWCP.EVSE_Id EVSEId)
        {
            throw new NotImplementedException();
        }

        public bool TryGetEVSEById(WWCP.EVSE_Id EVSEId, out IEVSE EVSE)
        {
            throw new NotImplementedException();
        }

        public Task<AddEVSEResult> AddEVSE(IEVSE EVSE, Action<IEVSE, EventTracking_Id>? OnSuccess = null, Action<IChargingStation, IEVSE, EventTracking_Id>? OnError = null, bool SkipAddedNotifications = false, Func<ChargingStationOperator_Id, WWCP.EVSE_Id, bool>? AllowInconsistentOperatorIds = null, EventTracking_Id? EventTrackingId = null, User_Id? CurrentUserId = null)
        {
            throw new NotImplementedException();
        }

        public Task<AddEVSEResult> AddEVSEIfNotExists(IEVSE EVSE, Action<IEVSE, EventTracking_Id>? OnSuccess = null, bool SkipAddedNotifications = false, Func<ChargingStationOperator_Id, WWCP.EVSE_Id, bool>? AllowInconsistentOperatorIds = null, EventTracking_Id? EventTrackingId = null, User_Id? CurrentUserId = null)
        {
            throw new NotImplementedException();
        }

        public Task<AddOrUpdateEVSEResult> AddOrUpdateEVSE(IEVSE EVSE, Action<IEVSE, EventTracking_Id>? OnAdditionSuccess = null, Action<IEVSE, IEVSE, EventTracking_Id>? OnUpdateSuccess = null, Action<IChargingStation, IEVSE, EventTracking_Id>? OnError = null, bool SkipAddOrUpdatedUpdatedNotifications = false, Func<ChargingStationOperator_Id, WWCP.EVSE_Id, bool>? AllowInconsistentOperatorIds = null, EventTracking_Id? EventTrackingId = null, User_Id? CurrentUserId = null)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateEVSEResult> UpdateEVSE(IEVSE EVSE, Action<IEVSE, IEVSE, EventTracking_Id>? OnSuccess = null, Action<IChargingStation, IEVSE, EventTracking_Id>? OnError = null, bool SkipUpdatedNotifications = false, Func<ChargingStationOperator_Id, WWCP.EVSE_Id, bool>? AllowInconsistentOperatorIds = null, EventTracking_Id? EventTrackingId = null, User_Id? CurrentUserId = null)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateEVSEResult> UpdateEVSE(IEVSE EVSE, Action<IEVSE> UpdateDelegate, Action<IEVSE, IEVSE, EventTracking_Id>? OnSuccess = null, Action<IChargingStation, IEVSE, EventTracking_Id>? OnError = null, bool SkipUpdatedNotifications = false, Func<ChargingStationOperator_Id, WWCP.EVSE_Id, bool>? AllowInconsistentOperatorIds = null, EventTracking_Id? EventTrackingId = null, User_Id? CurrentUserId = null)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteEVSEResult> RemoveEVSE(WWCP.EVSE_Id Id, Action<IEVSE, EventTracking_Id>? OnSuccess = null, Action<IChargingStation, IEVSE, EventTracking_Id>? OnError = null, bool SkipRemovedNotifications = false, EventTracking_Id? EventTrackingId = null, User_Id? CurrentUserId = null)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Sessions

        public IEnumerable<ChargingSession> ChargingSessions
            => throw new NotImplementedException();

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
        #endregion





        public IId AuthId => throw new NotImplementedException();



        public JObject CustomData => throw new NotImplementedException();


        public bool IsEmpty => throw new NotImplementedException();

        public bool IsNotEmpty => throw new NotImplementedException();

        public DateTime LastChangeDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }








        public IChargingStation UpdateWith(IChargingStation OtherChargingStation)
        {
            throw new NotImplementedException();
        }

        public JObject ToJSON(bool Embedded = false, InfoStatus ExpandRoamingNetworkId = InfoStatus.ShowIdOnly, InfoStatus ExpandChargingStationOperatorId = InfoStatus.ShowIdOnly, InfoStatus ExpandChargingPoolId = InfoStatus.ShowIdOnly, InfoStatus ExpandEVSEIds = InfoStatus.Expanded, InfoStatus ExpandBrandIds = InfoStatus.ShowIdOnly, InfoStatus ExpandDataLicenses = InfoStatus.ShowIdOnly, CustomJObjectSerializerDelegate<IChargingStation>? CustomChargingStationSerializer = null, CustomJObjectSerializerDelegate<IEVSE>? CustomEVSESerializer = null, CustomJObjectSerializerDelegate<ChargingConnector>? CustomChargingConnectorSerializer = null)
        {
            throw new NotImplementedException();
        }


        #region InternalData

        public UserDefinedDictionary InternalData => throw new NotImplementedException();


        public void DeleteProperty<T>(ref T? FieldToChange, [CallerMemberName] string PropertyName = "")
        {
            throw new NotImplementedException();
        }

        public object? GetInternalData(string Key)
        {
            throw new NotImplementedException();
        }

        public T? GetInternalDataAs<T>(string Key)
        {
            throw new NotImplementedException();
        }

        public void IfDefined(string Key, Action<object> ValueDelegate)
        {
            throw new NotImplementedException();
        }

        public void IfDefinedAs<T>(string Key, Action<T> ValueDelegate)
        {
            throw new NotImplementedException();
        }

        public bool IsDefined(string Key)
        {
            throw new NotImplementedException();
        }

        public bool IsDefined(string Key, object? Value)
        {
            throw new NotImplementedException();
        }

        public void PropertyChanged<T>(string PropertyName, T OldValue, T NewValue, Context? DataSource = null, EventTracking_Id? EventTrackingId = null)
        {
            throw new NotImplementedException();
        }

        public SetPropertyResult SetInternalData(string Key, object? NewValue, object? OldValue = null, Context? DataSource = null, EventTracking_Id? EventTrackingId = null)
        {
            throw new NotImplementedException();
        }

        public void SetProperty<T>(ref T FieldToChange, T NewValue, Context? DataSource = null, EventTracking_Id? EventTrackingId = null, [CallerMemberName] string PropertyName = "")
        {
            throw new NotImplementedException();
        }

        public bool TryGetInternalData(string Key, out object? Value)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInternalDataAs<T>(string Key, out T? Value)
        {
            throw new NotImplementedException();
        }

        #endregion


        public bool Equals(IChargingStation? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IChargingStation? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object? obj)
        {
            throw new NotImplementedException();
        }



    }

}
