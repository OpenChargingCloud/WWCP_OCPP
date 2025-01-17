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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.LC;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
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

    public class LocalControllerAdapter : IRemoteChargingPool
    {

        public ICSMSNode             CSMS                   { get; }
        public IChargingPool         ChargingPool           { get; }
        public ILocalControllerNode  LocalControllerNode    { get; }


        public LocalControllerAdapter(ICSMSNode             CSMS,
                                      IChargingPool         ChargingPool,
                                      ILocalControllerNode  LocalControllerNode)
        {

            this.CSMS                 = CSMS;
            this.ChargingPool         = ChargingPool;
            this.LocalControllerNode  = LocalControllerNode;

        }



        public WWCP.ChargingPool_Id Id => throw new NotImplementedException();

        public I18NString Name => throw new NotImplementedException();

        public I18NString Description => throw new NotImplementedException();

        public IRoamingNetwork? RoamingNetwork => throw new NotImplementedException();

        public IChargingStationOperator? Operator => throw new NotImplementedException();

        public IChargingStationOperator? SubOperator => throw new NotImplementedException();

        public IRemoteChargingPool? RemoteChargingPool => throw new NotImplementedException();

        public ReactiveSet<Brand> Brands => throw new NotImplementedException();

        public ReactiveSet<DataLicense> DataLicenses => throw new NotImplementedException();

        public Languages? LocationLanguage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Address? Address { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public GeoCoordinate? GeoLocation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Time_Zone? TimeZone { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Address? EntranceAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public GeoCoordinate? EntranceLocation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public I18NString ArrivalInstructions => throw new NotImplementedException();

        public OpeningTimes OpeningTimes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool? ChargingWhenClosed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ReactiveSet<UIFeatures> UIFeatures => throw new NotImplementedException();

        public ReactiveSet<AuthenticationModes> AuthenticationModes => throw new NotImplementedException();

        public ReactiveSet<PaymentOptions> PaymentOptions => throw new NotImplementedException();

        public AccessibilityType? Accessibility { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ReactiveSet<ChargingPoolFeature> Features => throw new NotImplementedException();

        public ReactiveSet<Facilities> Facilities => throw new NotImplementedException();

        public ReactiveSet<URL> PhotoURLs => throw new NotImplementedException();

        public PhoneNumber? HotlinePhoneNumber => throw new NotImplementedException();

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
        public Address ExitAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public GeoCoordinate? ExitLocation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Func<ChargingStationStatusReport, ChargingPoolStatusTypes>? StatusAggregationDelegate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEnumerable<IChargingStation> ChargingStations => throw new NotImplementedException();

        public IVotingSender<DateTime, User_Id, IChargingPool, IChargingStation, bool> OnChargingStationAddition => throw new NotImplementedException();

        public IVotingSender<DateTime, User_Id, IChargingPool, IChargingStation, IChargingStation, bool> OnChargingStationUpdate => throw new NotImplementedException();

        public IVotingSender<DateTime, User_Id, IChargingPool, IChargingStation, bool> OnChargingStationRemoval => throw new NotImplementedException();

        public IVotingSender<DateTime, User_Id, IChargingStation, IEVSE, bool> OnEVSEAddition => throw new NotImplementedException();

        public IVotingSender<DateTime, User_Id, IChargingStation, IEVSE, IEVSE, bool> OnEVSEUpdate => throw new NotImplementedException();

        public IVotingSender<DateTime, User_Id, IChargingStation, IEVSE, bool> OnEVSERemoval => throw new NotImplementedException();

        public IEnumerable<IEVSE> EVSEs => throw new NotImplementedException();

        public IEnumerable<WWCP.IEnergyMeter> EnergyMeters => throw new NotImplementedException();

        public string? DataSource => throw new NotImplementedException();

        public JObject CustomData => throw new NotImplementedException();

        public UserDefinedDictionary InternalData => throw new NotImplementedException();

        public bool IsEmpty => throw new NotImplementedException();

        public bool IsNotEmpty => throw new NotImplementedException();

        public DateTime LastChangeDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Timestamped<ChargingPoolAdminStatusTypes> AdminStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Timestamped<ChargingPoolStatusTypes> Status { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TimeSpan MaxReservationDuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEnumerable<ChargingReservation> ChargingReservations => throw new NotImplementedException();

        public IEnumerable<ChargingSession> ChargingSessions => throw new NotImplementedException();

        public bool DisableAuthorization { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IId AuthId => throw new NotImplementedException();

        public ReactiveSet<RootCAInfo> MobilityRootCAs => throw new NotImplementedException();

        public ParkingType? ParkingType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        IEnumerable<RootCAInfo> IChargingPool.MobilityRootCAs => throw new NotImplementedException();

        public IEnumerable<EVRoamingPartnerInfo> EVRoamingPartners => throw new NotImplementedException();

        public IEnumerable<Languages> LocationLanguages => throw new NotImplementedException();

        IEnumerable<DataLicense> IChargingPool.DataLicenses => throw new NotImplementedException();

        event OnChargingPoolDataChangedDelegate IChargingPool.OnDataChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event OnChargingPoolAdminStatusChangedDelegate IChargingPool.OnAdminStatusChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event OnChargingPoolStatusChangedDelegate IChargingPool.OnStatusChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        public event OnRemoteChargingPoolDataChangedDelegate? OnDataChanged;
        public event OnRemoteChargingPoolAdminStatusChangedDelegate? OnAdminStatusChanged;
        public event OnRemoteChargingPoolStatusChangedDelegate? OnStatusChanged;
        public event OnChargingStationDataChangedDelegate? OnChargingStationDataChanged;
        public event OnChargingStationStatusChangedDelegate? OnChargingStationStatusChanged;
        public event OnChargingStationAdminStatusChangedDelegate? OnChargingStationAdminStatusChanged;
        public event OnEVSEDataChangedDelegate OnEVSEDataChanged;
        public event OnEVSEStatusChangedDelegate OnEVSEStatusChanged;
        public event OnEVSEAdminStatusChangedDelegate OnEVSEAdminStatusChanged;
        public event OnPropertyChangedDelegate? OnPropertyChanged;
        public event OnReserveRequestDelegate? OnReserveRequest;
        public event OnReserveResponseDelegate? OnReserveResponse;
        public event OnNewReservationDelegate? OnNewReservation;
        public event OnCancelReservationRequestDelegate? OnCancelReservationRequest;
        public event OnCancelReservationResponseDelegate? OnCancelReservationResponse;
        public event OnReservationCanceledDelegate? OnReservationCanceled;
        public event OnRemoteStartRequestDelegate OnRemoteStartRequest;
        public event OnRemoteStartResponseDelegate OnRemoteStartResponse;
        public event OnRemoteStopRequestDelegate OnRemoteStopRequest;
        public event OnRemoteStopResponseDelegate OnRemoteStopResponse;
        public event OnNewChargingSessionDelegate OnNewChargingSession;
        public event OnNewChargeDetailRecordDelegate OnNewChargeDetailRecord;
        public event OnAuthorizeStartRequestDelegate OnAuthorizeStartRequest;
        public event OnAuthorizeStartResponseDelegate OnAuthorizeStartResponse;
        public event OnAuthorizeStopRequestDelegate OnAuthorizeStopRequest;
        public event OnAuthorizeStopResponseDelegate OnAuthorizeStopResponse;

        public IEnumerable<WWCP.ChargingStation_Id> ChargingStationIds(IncludeChargingStationDelegate? IncludeStations = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ChargingStationAdminStatus> ChargingStationAdminStatus(IncludeChargingStationDelegate? IncludeStations = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ChargingStationStatus> ChargingStationStatus(IncludeChargingStationDelegate? IncludeStations = null)
        {
            throw new NotImplementedException();
        }

        public Task<WWCP.AddChargingStationResult> AddChargingStation(IChargingStation ChargingStation, Action<IChargingStation, EventTracking_Id>? OnSuccess = null, Action<IChargingPool, IChargingStation, EventTracking_Id>? OnError = null, bool SkipAddedNotifications = false, Func<ChargingStationOperator_Id, WWCP.ChargingStation_Id, bool>? AllowInconsistentOperatorIds = null, EventTracking_Id? EventTrackingId = null, User_Id? CurrentUserId = null)
        {
            throw new NotImplementedException();
        }

        public Task<WWCP.AddChargingStationResult> AddChargingStationIfNotExists(IChargingStation ChargingStation, Action<IChargingStation, EventTracking_Id>? OnSuccess = null, bool SkipAddedNotifications = false, Func<ChargingStationOperator_Id, WWCP.ChargingStation_Id, bool>? AllowInconsistentOperatorIds = null, EventTracking_Id? EventTrackingId = null, User_Id? CurrentUserId = null)
        {
            throw new NotImplementedException();
        }

        public Task<WWCP.AddOrUpdateChargingStationResult> AddOrUpdateChargingStation(IChargingStation ChargingStation, Action<IChargingStation, EventTracking_Id>? OnAdditionSuccess = null, Action<IChargingStation, IChargingStation, EventTracking_Id>? OnUpdateSuccess = null, Action<IChargingPool, IChargingStation, EventTracking_Id>? OnError = null, bool SkipAddOrUpdatedUpdatedNotifications = false, Func<ChargingStationOperator_Id, WWCP.ChargingStation_Id, bool>? AllowInconsistentOperatorIds = null, EventTracking_Id? EventTrackingId = null, User_Id? CurrentUserId = null)
        {
            throw new NotImplementedException();
        }

        public Task<WWCP.UpdateChargingStationResult> UpdateChargingStation(IChargingStation ChargingStation, Action<IChargingStation, IChargingStation, EventTracking_Id>? OnUpdateSuccess = null, Action<IChargingPool, IChargingStation, EventTracking_Id>? OnError = null, bool SkipUpdatedNotifications = false, Func<ChargingStationOperator_Id, WWCP.ChargingStation_Id, bool>? AllowInconsistentOperatorIds = null, EventTracking_Id? EventTrackingId = null, User_Id? CurrentUserId = null)
        {
            throw new NotImplementedException();
        }

        public Task<WWCP.UpdateChargingStationResult> UpdateChargingStation(IChargingStation ChargingStation, Action<IChargingStation> UpdateDelegate, Action<IChargingStation, IChargingStation, EventTracking_Id>? OnUpdateSuccess = null, Action<IChargingPool, IChargingStation, EventTracking_Id>? OnError = null, bool SkipUpdatedNotifications = false, Func<ChargingStationOperator_Id, WWCP.ChargingStation_Id, bool>? AllowInconsistentOperatorIds = null, EventTracking_Id? EventTrackingId = null, User_Id? CurrentUserId = null)
        {
            throw new NotImplementedException();
        }

        public Task<WWCP.DeleteChargingStationResult> RemoveChargingStation(WWCP.ChargingStation_Id Id, Action<IChargingStation, EventTracking_Id>? OnSuccess = null, Action<IChargingPool, IChargingStation, EventTracking_Id>? OnError = null, bool SkipRemovedNotifications = false, EventTracking_Id? EventTrackingId = null, User_Id? CurrentUserId = null)
        {
            throw new NotImplementedException();
        }

        public ChargingPool Clone()
        {
            throw new NotImplementedException();
        }

        public bool ContainsChargingStation(IChargingStation ChargingStation)
        {
            throw new NotImplementedException();
        }

        public bool ContainsChargingStation(WWCP.ChargingStation_Id ChargingStationId)
        {
            throw new NotImplementedException();
        }

        public IChargingStation? GetChargingStationById(WWCP.ChargingStation_Id ChargingStationId)
        {
            throw new NotImplementedException();
        }

        public bool TryGetChargingStationById(WWCP.ChargingStation_Id ChargingStationId, out IChargingStation? ChargingStation)
        {
            throw new NotImplementedException();
        }

        public bool TryGetChargingStationByEVSEId(WWCP.EVSE_Id EVSEId, out IChargingStation? ChargingStation)
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

        public IEnumerable<Tuple<WWCP.EVSE_Id, IEnumerable<Timestamped<EVSEAdminStatusType>>>> EVSEAdminStatusSchedule(IncludeEVSEDelegate? IncludeEVSEs = null, Func<DateTime, bool>? TimestampFilter = null, Func<EVSEAdminStatusType, bool>? StatusFilter = null, ulong? Skip = null, ulong? Take = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EVSEStatus> EVSEStatus(IncludeEVSEDelegate? IncludeEVSEs = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<WWCP.EVSE_Id, IEnumerable<Timestamped<EVSEStatusType>>>> EVSEStatusSchedule(IncludeEVSEDelegate? IncludeEVSEs = null, Func<DateTime, bool>? TimestampFilter = null, Func<EVSEStatusType, bool>? StatusFilter = null, ulong? Skip = null, ulong? Take = null)
        {
            throw new NotImplementedException();
        }

        public bool ContainsEVSE(WWCP.EVSE EVSE)
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

        public bool TryGetEVSEById(WWCP.EVSE_Id EVSEId, out IEVSE? EVSE)
        {
            throw new NotImplementedException();
        }

        public ChargingPool UpdateWith(ChargingPool OtherChargingPool)
        {
            throw new NotImplementedException();
        }

        public JObject ToJSON(bool Embedded = false, InfoStatus ExpandRoamingNetworkId = InfoStatus.ShowIdOnly, InfoStatus ExpandChargingStationOperatorId = InfoStatus.ShowIdOnly, InfoStatus ExpandChargingStationIds = InfoStatus.Expanded, InfoStatus ExpandEVSEIds = InfoStatus.Hidden, InfoStatus ExpandBrandIds = InfoStatus.ShowIdOnly, InfoStatus ExpandDataLicenses = InfoStatus.ShowIdOnly, CustomJObjectSerializerDelegate<IChargingPool>? CustomChargingPoolSerializer = null, CustomJObjectSerializerDelegate<IChargingStation>? CustomChargingStationSerializer = null, CustomJObjectSerializerDelegate<IEVSE>? CustomEVSESerializer = null, CustomJObjectSerializerDelegate<ChargingConnector>? CustomChargingConnectorSerializer = null)
        {
            throw new NotImplementedException();
        }

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

        public IEnumerable<Timestamped<ChargingPoolAdminStatusTypes>> AdminStatusSchedule(Func<DateTime, bool>? TimestampFilter = null, Func<ChargingPoolAdminStatusTypes, bool>? AdminStatusFilter = null, ulong? Skip = null, ulong? Take = null)
        {
            throw new NotImplementedException();
        }

        public void SetAdminStatus(ChargingPoolAdminStatusTypes NewAdminStatus, Context? DataSource = null)
        {
            throw new NotImplementedException();
        }

        public void SetAdminStatus(Timestamped<ChargingPoolAdminStatusTypes> NewTimestampedAdminStatus, Context? DataSource = null)
        {
            throw new NotImplementedException();
        }

        public void SetAdminStatus(IEnumerable<Timestamped<ChargingPoolAdminStatusTypes>> NewAdminStatusList, ChangeMethods ChangeMethod = ChangeMethods.Replace, Context? DataSource = null)
        {
            throw new NotImplementedException();
        }

        public void SetAdminStatus(ChargingPoolAdminStatusTypes NewAdminStatus, DateTime Timestamp, Context? DataSource = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Timestamped<ChargingPoolStatusTypes>> StatusSchedule(Func<DateTime, bool>? TimestampFilter = null, Func<ChargingPoolStatusTypes, bool>? StatusFilter = null, ulong? Skip = null, ulong? Take = null)
        {
            throw new NotImplementedException();
        }

        public void SetStatus(ChargingPoolStatusTypes NewStatus, Context? DataSource = null)
        {
            throw new NotImplementedException();
        }

        public void SetStatus(Timestamped<ChargingPoolStatusTypes> NewTimestampedStatus, Context? DataSource = null)
        {
            throw new NotImplementedException();
        }

        public void SetStatus(IEnumerable<Timestamped<ChargingPoolStatusTypes>> NewStatusList, ChangeMethods ChangeMethod = ChangeMethods.Replace, Context? DataSource = null)
        {
            throw new NotImplementedException();
        }

        public void SetStatus(ChargingPoolStatusTypes NewStatus, DateTime Timestamp, Context? DataSource = null)
        {
            throw new NotImplementedException();
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

            var response = await CSMS.StartCharging(
                                     Destination:                        SourceRouting.To(LocalControllerNode.Id),
                                     RequestStartTransactionRequestId:   RemoteStart_Id.NewRandom,
                                     IdToken:                            new IdToken(
                                                                             Value:             RemoteAuthentication.RemoteIdentification.Value.ToString(),
                                                                             Type:              IdTokenType.eMAID,
                                                                             AdditionalInfos:   null,
                                                                             CustomData:        null
                                                                         ),
                                     EVSEId:                             EVSE_Id.Parse(0),
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
                                                                                                   ? new JProperty("providerId", ProviderId.ToString())
                                                                                                   : null
                                                                                           )
                                                                         ),

                                     RequestId:                          null,
                                     RequestTimestamp:                   null,
                                     RequestTimeout:                     null,
                                     EventTrackingId:                    null,
                                     CancellationToken:                  default
                                 );

            return RemoteStartResult.OutOfService(System_Id.Local);

        }

        public Task<RemoteStopResult> RemoteStop(ChargingSession_Id SessionId, ReservationHandling? ReservationHandling = null, EMobilityProvider_Id? ProviderId = null, RemoteAuthentication? RemoteAuthentication = null, Auth_Path? AuthenticationPath = null, DateTime? Timestamp = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null, CancellationToken CancellationToken = default)
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

        public bool Equals(IChargingPool? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IChargingPool? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object? obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IChargingStation> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public Task<AuthStartResult> AuthorizeStart(LocalAuthentication LocalAuthentication, ChargingLocation? ChargingLocation = null, ChargingProduct? ChargingProduct = null, ChargingSession_Id? SessionId = null, ChargingSession_Id? CPOPartnerSessionId = null, ChargingStationOperator_Id? OperatorId = null, DateTime? Timestamp = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null, CancellationToken CancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<AuthStopResult> AuthorizeStop(ChargingSession_Id SessionId, LocalAuthentication LocalAuthentication, ChargingLocation? ChargingLocation = null, ChargingSession_Id? CPOPartnerSessionId = null, ChargingStationOperator_Id? OperatorId = null, DateTime? Timestamp = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null, CancellationToken CancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

}
