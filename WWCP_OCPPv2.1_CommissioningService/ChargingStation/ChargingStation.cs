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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using org.GraphDefined.Vanaheimr.Hermod.HTTP.Notifications;

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CMS
{

    public delegate Boolean ChargingStationProviderDelegate(ChargingStation_Id ChargingStationId, out ChargingStation ChargingStation);

    public delegate JObject ChargingStationToJSONDelegate(ChargingStation  ChargingStation,
                                                          Boolean          Embedded            = false,
                                                          InfoStatus       ExpandTags          = InfoStatus.ShowIdOnly,
                                                          Boolean          IncludeCryptoHash   = true);

    /// <summary>
    /// Extension methods for chargeBoxs.
    /// </summary>
    public static class ChargingStationExtensions
    {

        #region ToJSON(this ChargingStations, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of chargeBoxs.
        /// </summary>
        /// <param name="ChargingStations">An enumeration of chargeBoxs.</param>
        /// <param name="Skip">The optional number of chargeBoxs to skip.</param>
        /// <param name="Take">The optional number of chargeBoxs to return.</param>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        public static JArray ToJSON(this IEnumerable<ChargingStation>  ChargingStations,
                                    UInt64?                      Skip                  = null,
                                    UInt64?                      Take                  = null,
                                    Boolean                      Embedded              = false,
                                    InfoStatus                   ExpandTags            = InfoStatus.ShowIdOnly,
                                    ChargingStationToJSONDelegate      ChargingStationToJSON       = null,
                                    Boolean                      IncludeCryptoHash     = true)


            => ChargingStations?.Any() != true

                   ? new JArray()

                   : new JArray(ChargingStations.
                                    Where     (chargeBox => chargeBox != null).
                                    OrderBy   (chargeBox => chargeBox.Id).
                                    SkipTakeFilter(Skip, Take).
                                    SafeSelect(chargeBox => ChargingStationToJSON != null
                                                                    ? ChargingStationToJSON (chargeBox,
                                                                                       Embedded,
                                                                                       ExpandTags,
                                                                                       IncludeCryptoHash)

                                                                    : chargeBox.ToJSON(Embedded,
                                                                                       ExpandTags,
                                                                                       IncludeCryptoHash)));

        #endregion

    }



    /// <summary>
    /// A charging station.
    /// </summary>
    public class ChargingStation : AEntity<ChargingStation_Id,
                                           ChargingStation>
    {

        /// <summary>
        /// A charging station connector.
        /// </summary>
        public class ChargingStationConnector
        {

            public Connector_Id       Id                       { get; }

            public OperationalStatus  OperationalStatus        { get; set; }


            public Boolean            IsReserved               { get; set; }

            public Boolean            IsCharging               { get; set; }

            public IdToken            IdToken                  { get; set; }

          //  public IdTagInfo          IdTagInfo                { get; set; }

            public Transaction_Id     TransactionId            { get; set; }

            public ChargingProfile    ChargingProfile          { get; set; }


            public DateTime           StartTimestamp           { get; set; }

            public UInt64             MeterStartValue          { get; set; }

            public String             SignedStartMeterValue    { get; set; }


            public DateTime           StopTimestamp            { get; set; }

            public UInt64             MeterStopValue           { get; set; }

            public String             SignedStopMeterValue     { get; set; }


            public ChargingStationConnector(Connector_Id       Id,
                                            OperationalStatus  OperationalStatus)
            {

                this.Id                 = Id;
                this.OperationalStatus  = OperationalStatus;

            }


        }


        #region Data

        /// <summary>
        /// The default max size of the charging station status history.
        /// </summary>
        public const UInt16 DefaultChargingStationStatusHistorySize = 50;

        /// <summary>
        /// The default JSON-LD context of organizations.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://charging.cloud/contexts/OCPP/chargeBox");

        /// <summary>
        /// The default time span between expected heartbeats.
        /// </summary>
        public readonly TimeSpan DefaultExpectHeartbeatEvery = TimeSpan.FromMinutes(5);

        protected static readonly TimeSpan SemaphoreSlimTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The default maintenance interval.
        /// </summary>
        public readonly TimeSpan DefaultMaintenanceEvery = TimeSpan.FromMinutes(1);
        private static readonly SemaphoreSlim MaintenanceSemaphore = new (1, 1);
        private readonly Timer MaintenanceTimer;

        private readonly Timer SendHeartbeatTimer;

        //public Tuple<String, String> HTTPBasicAuth { get; }
        public DNSClient DNSClient { get; }

        #endregion

        #region Properties

        #region API

        private Object _API;

        /// <summary>
        /// The UsersAPI of this organization.
        /// </summary>
        internal Object API
        {

            get
            {
                return _API;
            }

            set
            {

                if (_API == value)
                    return;

                if (_API != null)
                    throw new ArgumentException("Illegal attempt to change the API of this organization!");

                _API = value ?? throw new ArgumentException("Illegal attempt to delete the API reference of this organization!");

            }

        }

        #endregion


        /// <summary>
        /// The charge point vendor identification.
        /// </summary>
        public String                   ChargePointVendor           { get; }

        /// <summary>
        ///  The charge point model identification.
        /// </summary>
        public String                   ChargePointModel            { get; }


        /// <summary>
        /// The optional multi-language charging station description.
        /// </summary>
        public I18NString?              Description                 { get; }

        /// <summary>
        /// The optional serial number of the charge point.
        /// </summary>
        public String?                  ChargePointSerialNumber     { get; }

        /// <summary>
        /// The optional serial number of the charge point.
        /// </summary>
        //public String?                  ChargingStationSerialNumber       { get; }

        /// <summary>
        /// The optional firmware version of the charge point.
        /// </summary>
        public String?                  FirmwareVersion             { get; }

        /// <summary>
        /// The optional ICCID of the charge point's SIM card.
        /// </summary>
        public String?                  Iccid                       { get; }

        /// <summary>
        /// The optional IMSI of the charge point’s SIM card.
        /// </summary>
        public String?                  IMSI                        { get; }

        /// <summary>
        /// The optional meter type of the main power meter of the charge point.
        /// </summary>
        public String?                  MeterType                   { get; }

        /// <summary>
        /// The optional serial number of the main power meter of the charge point.
        /// </summary>
        public String?                  MeterSerialNumber           { get; }

        /// <summary>
        /// The optional public key of the main power meter of the charge point.
        /// </summary>
        public String?                  MeterPublicKey              { get; }


        /// <summary>
        /// The time span between expected heartbeat requests.
        /// </summary>
        public TimeSpan                 ExpectHeartbeatEvery        { get; set; }

        /// <summary>
        /// The time at the central system.
        /// </summary>
        public DateTime?                CSMSTime           { get; private set; }

        /// <summary>
        /// The default request timeout for all requests.
        /// </summary>
        public TimeSpan                 DefaultRequestTimeout       { get; }




        /// <summary>
        /// The maintenance interval.
        /// </summary>
        public TimeSpan                 MaintenanceEvery            { get; }

        /// <summary>
        /// Disable all maintenance tasks.
        /// </summary>
        public Boolean                  DisableMaintenanceTasks     { get; set; }

        /// <summary>
        /// Disable all heartbeats.
        /// </summary>
        public Boolean                  DisableSendHeartbeats       { get; set; }






        // Controlled by the central system!

        private readonly Dictionary<Connector_Id, ChargingStationConnector> connectors;

        public IEnumerable<ChargingStationConnector> Connectors
            => connectors.Values;


        //public readonly Dictionary<String, ConfigurationData> Configuration;

        #endregion

        #region Events

        // Outgoing messages (to central system)

        #region OnBootNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a boot notification request will be sent to the central system.
        /// </summary>
        public event OnBootNotificationRequestReceivedDelegate?   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        public event OnBootNotificationResponseSentDelegate?  OnBootNotificationResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat request will be sent to the central system.
        /// </summary>
        public event OnHeartbeatRequestReceivedDelegate?   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        public event OnHeartbeatResponseSentDelegate?  OnHeartbeatResponse;

        #endregion


        #region OnAuthorizeRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize request will be sent to the central system.
        /// </summary>
        public event OnAuthorizeRequestReceivedDelegate?   OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event OnAuthorizeResponseSentDelegate?  OnAuthorizeResponse;

        #endregion

        #region OnStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a status notification request will be sent to the central system.
        /// </summary>
        public event OnStatusNotificationRequestReceivedDelegate?   OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification request was received.
        /// </summary>
        public event OnStatusNotificationResponseSentDelegate?  OnStatusNotificationResponse;

        #endregion

        #region OnMeterValuesRequest/-Response

        /// <summary>
        /// An event fired whenever a meter values request will be sent to the central system.
        /// </summary>
        public event OnMeterValuesRequestReceivedDelegate?   OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values request was received.
        /// </summary>
        public event OnMeterValuesResponseSentDelegate?  OnMeterValuesResponse;

        #endregion


        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer request will be sent to the central system.
        /// </summary>
        public event OnDataTransferRequestSentDelegate?       OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        public event OnDataTransferResponseReceivedDelegate?  OnDataTransferResponse;

        #endregion

        #region OnFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a firmware status notification request will be sent to the central system.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestReceivedDelegate?   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseSentDelegate?  OnFirmwareStatusNotificationResponse;

        #endregion



        // Incoming messages (from central system)

        #region OnResetRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnResetRequestSentDelegate?   OnResetRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnResetResponseReceivedDelegate?  OnResetResponse;

        #endregion

        #region ChangeAvailabilityRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeAvailabilityRequestSentDelegate?   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnChangeAvailabilityResponseReceivedDelegate?  OnChangeAvailabilityResponse;

        #endregion

        #region OnIncomingDataTransferRequest/-Response

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnDataTransferRequestReceivedDelegate?  OnDataTransferRequestReceived;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OnDataTransferResponseSentDelegate?     OnDataTransferResponseSent;

        #endregion

        #region TriggerMessageRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnTriggerMessageRequestSentDelegate?   OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnTriggerMessageResponseReceivedDelegate?  OnTriggerMessageResponse;

        #endregion

        #region UpdateFirmwareRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUpdateFirmwareRequestSentDelegate?   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseReceivedDelegate?  OnUpdateFirmwareResponse;

        #endregion


        #region OnReserveNowRequest/-Response

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        public event OnReserveNowRequestSentDelegate?   OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a response to a reserve now request was sent.
        /// </summary>
        public event OnReserveNowResponseReceivedDelegate?  OnReserveNowResponse;

        #endregion

        #region OnCancelReservationRequest/-Response

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationRequestSentDelegate?   OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a response to a cancel reservation request was sent.
        /// </summary>
        public event OnCancelReservationResponseReceivedDelegate?  OnCancelReservationResponse;

        #endregion

        #region SetChargingProfileRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSetChargingProfileRequestSentDelegate?   OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnSetChargingProfileResponseReceivedDelegate?  OnSetChargingProfileResponse;

        #endregion

        #region ClearChargingProfileRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearChargingProfileRequestSentDelegate?   OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnClearChargingProfileResponseReceivedDelegate?  OnClearChargingProfileResponse;

        #endregion

        #region GetCompositeScheduleRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetCompositeScheduleRequestSentDelegate?   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetCompositeScheduleResponseReceivedDelegate?  OnGetCompositeScheduleResponse;

        #endregion

        #region UnlockConnectorRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUnlockConnectorRequestSentDelegate?   OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnUnlockConnectorResponseReceivedDelegate?  OnUnlockConnectorResponse;

        #endregion


        #region GetLocalListVersionRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetLocalListVersionRequestSentDelegate?   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetLocalListVersionResponseReceivedDelegate?  OnGetLocalListVersionResponse;

        #endregion

        #region SendLocalListRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSendLocalListRequestSentDelegate?   OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnSendLocalListResponseReceivedDelegate?  OnSendLocalListResponse;

        #endregion

        #region ClearCacheRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearCacheRequestSentDelegate?   OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnClearCacheResponseReceivedDelegate?  OnClearCacheResponse;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge point for testing.
        /// </summary>
        /// <param name="Id">The charging station identification.</param>
        /// <param name="NumberOfConnectors">Number of available connectors.</param>
        /// <param name="ChargePointVendor">The charge point vendor identification.</param>
        /// <param name="ChargePointModel">The charge point model identification.</param>
        /// 
        /// <param name="Description">An optional multi-language charging station description.</param>
        /// <param name="ChargePointSerialNumber">An optional serial number of the charge point.</param>
        /// <param name="ChargingStationSerialNumber">An optional serial number of the charge point.</param>
        /// <param name="FirmwareVersion">An optional firmware version of the charge point.</param>
        /// <param name="Iccid">An optional ICCID of the charge point's SIM card.</param>
        /// <param name="IMSI">An optional IMSI of the charge point’s SIM card.</param>
        /// <param name="MeterType">An optional meter type of the main power meter of the charge point.</param>
        /// <param name="MeterSerialNumber">An optional serial number of the main power meter of the charge point.</param>
        /// <param name="MeterPublicKey">An optional public key of the main power meter of the charge point.</param>
        /// 
        /// <param name="ExpectHeartbeatEvery">The time span between expected heartbeat.</param>
        public ChargingStation(ChargingStation_Id                                Id,
                         Byte                                        NumberOfConnectors,
                         String                                      ChargePointVendor,
                         String                                      ChargePointModel,

                         I18NString?                                 Description                         = null,
                         String?                                     ChargePointSerialNumber             = null,
                         //String?                                     ChargingStationSerialNumber               = null,
                         String?                                     FirmwareVersion                     = null,
                         String?                                     Iccid                               = null,
                         String?                                     IMSI                                = null,
                         String?                                     MeterType                           = null,
                         String?                                     MeterSerialNumber                   = null,
                         String?                                     MeterPublicKey                      = null,

                         TimeSpan?                                   ExpectHeartbeatEvery                = null,

                         IEnumerable<ANotification>?                 Notifications                       = null,

                         //IEnumerable<Organization2ChargingStationEdge>?    Organization2ChargingStationEdges         = null,
                         IEnumerable<ChargingStation2ChargingStationEdge>?       ChargingStation2ChargingStationInEdges          = null,
                         IEnumerable<ChargingStation2ChargingStationEdge>?       ChargingStation2ChargingStationOutEdges         = null,

                         JObject?                                    CustomData                          = default,
                         IEnumerable<AttachedFile>?                  AttachedFiles                       = default,
                         JSONLDContext?                              JSONLDContext                       = default,
                         String?                                     DataSource                          = default,
                         DateTime?                                   LastChange                          = default)

            : base(Id,
                   JSONLDContext ?? DefaultJSONLDContext,
                   null,
                   Description,
                   null,
                   CustomData,
                   null,
                   LastChange,
                   DataSource)

        {

            if (ChargePointVendor.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargePointVendor),  "The given charge point vendor must not be null or empty!");

            if (ChargePointModel.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargePointModel),   "The given charge point model must not be null or empty!");


            this.connectors               = new Dictionary<Connector_Id, ChargingStationConnector>();
            //for (var i = 1; i <= NumberOfConnectors; i++)
            //{
            //    this.connectors.Add(Connector_Id.Parse(i.ToString()),
            //                        new ChargePointConnector(Connector_Id.Parse(i.ToString()),
            //                                                 Availabilities.Inoperative));
            //}

            //this.Configuration            = new Dictionary<String, ConfigurationData>();

            this.ChargePointVendor        = ChargePointVendor;
            this.ChargePointModel         = ChargePointModel;

            this.Description              = Description;
            this.ChargePointSerialNumber  = ChargePointSerialNumber;
            //this.ChargingStationSerialNumber    = ChargingStationSerialNumber;
            this.FirmwareVersion          = FirmwareVersion;
            this.Iccid                    = Iccid;
            this.IMSI                     = IMSI;
            this.MeterType                = MeterType;
            this.MeterSerialNumber        = MeterSerialNumber;
            this.MeterPublicKey           = MeterPublicKey;

            this.ExpectHeartbeatEvery     = ExpectHeartbeatEvery ?? DefaultExpectHeartbeatEvery;

            //this.HTTPBasicAuth            = HTTPBasicAuth;

        }

        #endregion


        #region ToJSON(CustomChargingStationSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        public override JObject ToJSON(Boolean  Embedded   = false)

            => ToJSON(Embedded:    false,
                      ExpandTags:  InfoStatus.ShowIdOnly);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingStationSerializer">A delegate to serialize custom charging stationes.</param>
        public JObject ToJSON(Boolean                                      Embedded                    = false,
                              InfoStatus                                   ExpandTags                  = InfoStatus.ShowIdOnly,
                              Boolean                                      IncludeLastChange           = true,
                              CustomJObjectSerializerDelegate<ChargingStation>?  CustomChargingStationSerializer   = null)
        {

            var json = base.ToJSON(Embedded,
                                   IncludeLastChange,
                                   null,
                                   new JProperty?[] {

                                       ChargePointSerialNumber.IsNotNullOrEmpty()
                                           ? new JProperty("chargePointVendor",         ChargePointVendor)
                                           : null,

                                       ChargePointSerialNumber.IsNotNullOrEmpty()
                                           ? new JProperty("chargePointModel",          ChargePointModel)
                                           : null,

                                       ChargePointSerialNumber.IsNotNullOrEmpty()
                                           ? new JProperty("chargePointSerialNumber",   ChargePointSerialNumber)
                                           : null,

                                       //ChargingStationSerialNumber.  IsNotNullOrEmpty()
                                       //    ? new JProperty("chargeBoxSerialNumber",     ChargingStationSerialNumber)
                                       //    : null,

                                       FirmwareVersion.        IsNotNullOrEmpty()
                                           ? new JProperty("firmwareVersion",           FirmwareVersion)
                                           : null,

                                       Iccid.                  IsNotNullOrEmpty()
                                           ? new JProperty("iccid",                     Iccid)
                                           : null,

                                       IMSI.                   IsNotNullOrEmpty()
                                           ? new JProperty("imsi",                      IMSI)
                                           : null,

                                       MeterType.              IsNotNullOrEmpty()
                                           ? new JProperty("meterType",                 MeterType)
                                           : null,

                                       MeterSerialNumber.      IsNotNullOrEmpty()
                                           ? new JProperty("meterSerialNumber",         MeterSerialNumber)
                                           : null,

                                       MeterPublicKey.         IsNotNullOrEmpty()
                                           ? new JProperty("meterPublicKey",            MeterPublicKey)
                                           : null

                                   });

            return CustomChargingStationSerializer is not null
                       ? CustomChargingStationSerializer(this, json)
                       : json;

        }

        #endregion


        #region CopyAllLinkedDataFromBase(OldChargingStation)

        public override void CopyAllLinkedDataFromBase(ChargingStation OldChargingStation)
        {

            //if (OldChargingStation._User2ChargingStation_Edges.Any() && !_User2ChargingStation_Edges.Any())
            //{

            //    AddUsers(OldChargingStation._User2ChargingStation_Edges);

            //    foreach (var edge in _User2ChargingStation_Edges)
            //        edge.Target = this;

            //}

            //if (OldChargingStation._ChargingStation2ChargingStation_InEdges.Any() && !_ChargingStation2ChargingStation_InEdges.Any())
            //{

            //    AddEdges(OldChargingStation._ChargingStation2ChargingStation_InEdges);

            //    foreach (var edge in _ChargingStation2ChargingStation_InEdges)
            //        edge.Target = this;

            //}

            //if (OldChargingStation._ChargingStation2ChargingStation_OutEdges.Any() && !_ChargingStation2ChargingStation_OutEdges.Any())
            //{

            //    AddEdges(OldChargingStation._ChargingStation2ChargingStation_OutEdges);

            //    foreach (var edge in _ChargingStation2ChargingStation_OutEdges)
            //        edge.Source = this;

            //}

            //if (OldChargingStation._Notifications.SafeAny() && !_Notifications.SafeAny())
            //    _Notifications.Add(OldChargingStation._Notifications);

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargingStationId1, ChargingStationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingStationId1">A chargeBox identification.</param>
        /// <param name="ChargingStationId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingStation ChargingStationId1, ChargingStation ChargingStationId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ChargingStationId1, ChargingStationId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ChargingStationId1 == null) || ((Object) ChargingStationId2 == null))
                return false;

            return ChargingStationId1.Equals(ChargingStationId2);

        }

        #endregion

        #region Operator != (ChargingStationId1, ChargingStationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingStationId1">A chargeBox identification.</param>
        /// <param name="ChargingStationId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingStation ChargingStationId1, ChargingStation ChargingStationId2)
            => !(ChargingStationId1 == ChargingStationId2);

        #endregion

        #region Operator <  (ChargingStationId1, ChargingStationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingStationId1">A chargeBox identification.</param>
        /// <param name="ChargingStationId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingStation ChargingStationId1, ChargingStation ChargingStationId2)
        {

            if ((Object) ChargingStationId1 == null)
                throw new ArgumentNullException(nameof(ChargingStationId1), "The given ChargingStationId1 must not be null!");

            return ChargingStationId1.CompareTo(ChargingStationId2) < 0;

        }

        #endregion

        #region Operator <= (ChargingStationId1, ChargingStationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingStationId1">A chargeBox identification.</param>
        /// <param name="ChargingStationId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingStation ChargingStationId1, ChargingStation ChargingStationId2)
            => !(ChargingStationId1 > ChargingStationId2);

        #endregion

        #region Operator >  (ChargingStationId1, ChargingStationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingStationId1">A chargeBox identification.</param>
        /// <param name="ChargingStationId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingStation ChargingStationId1, ChargingStation ChargingStationId2)
        {

            if ((Object) ChargingStationId1 == null)
                throw new ArgumentNullException(nameof(ChargingStationId1), "The given ChargingStationId1 must not be null!");

            return ChargingStationId1.CompareTo(ChargingStationId2) > 0;

        }

        #endregion

        #region Operator >= (ChargingStationId1, ChargingStationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingStationId1">A chargeBox identification.</param>
        /// <param name="ChargingStationId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingStation ChargingStationId1, ChargingStation ChargingStationId2)
            => !(ChargingStationId1 < ChargingStationId2);

        #endregion

        #endregion

        #region IComparable<ChargingStation> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Int32 CompareTo(Object Object)

            => Object is ChargingStation ChargingStation
                   ? CompareTo(ChargingStation)
                   : throw new ArgumentException("The given object is not an chargeBox!", nameof(Object));

        #endregion

        #region CompareTo(ChargingStation)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingStation">An chargeBox object to compare with.</param>
        public override Int32 CompareTo(ChargingStation ChargingStation)

            => ChargingStation is null
                   ? throw new ArgumentNullException(nameof(ChargingStation), "The given chargeBox must not be null!")
                   : Id.CompareTo(ChargingStation.Id);

        #endregion

        #endregion

        #region IEquatable<ChargingStation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ChargingStation ChargingStation &&
                  Equals(ChargingStation);

        #endregion

        #region Equals(ChargingStation)

        /// <summary>
        /// Compares two chargeBoxs for equality.
        /// </summary>
        /// <param name="ChargingStation">An chargeBox to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ChargingStation ChargingStation)

            => ChargingStation is ChargingStation &&
                   Id.Equals(ChargingStation.Id);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Get the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => Id.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => Id.ToString();

        #endregion


        #region ToBuilder(NewChargingStationId = null)

        /// <summary>
        /// Return a builder for this chargeBox.
        /// </summary>
        /// <param name="NewChargingStationId">An optional new chargeBox identification.</param>
        public Builder ToBuilder(ChargingStation_Id? NewChargingStationId = null)

            => new (NewChargingStationId ?? Id,
                    1, //NumberOfConnectors,
                    ChargePointVendor,
                    ChargePointModel,

                    Description,
                    ChargePointSerialNumber,
                    //ChargingStationSerialNumber,
                    FirmwareVersion,
                    Iccid,
                    IMSI,
                    MeterType,
                    MeterSerialNumber,
                    MeterPublicKey,

                    ExpectHeartbeatEvery,

                    null, //_Notifications,

                    //_User2ChargingStation_Edges,
                    null, //_ChargingStation2ChargingStation_InEdges,
                    null, //_ChargingStation2ChargingStation_OutEdges,

                    CustomData,
                    null, //AttachedFiles,
                    JSONLDContext,
                    DataSource,
                    LastChangeDate);

        #endregion

        #region (class) Builder

        /// <summary>
        /// An chargeBox builder.
        /// </summary>
        public new class Builder : AEntity<ChargingStation_Id, ChargingStation>.Builder
        {

            #region Properties

            /// <summary>
            /// The charge point vendor identification.
            /// </summary>
            public String                   ChargePointVendor           { get; set; }

            /// <summary>
            ///  The charge point model identification.
            /// </summary>
            public String                   ChargePointModel            { get; set; }


            /// <summary>
            /// The optional multi-language charging station description.
            /// </summary>
            public I18NString               Description                 { get; set; }

            /// <summary>
            /// The optional serial number of the charge point.
            /// </summary>
            public String                   ChargePointSerialNumber     { get; set; }

            /// <summary>
            /// The optional serial number of the charge point.
            /// </summary>
           // public String                   ChargingStationSerialNumber       { get; set; }

            /// <summary>
            /// The optional firmware version of the charge point.
            /// </summary>
            public String                   FirmwareVersion             { get; set; }

            /// <summary>
            /// The optional ICCID of the charge point's SIM card.
            /// </summary>
            public String                   Iccid                       { get; set; }

            /// <summary>
            /// The optional IMSI of the charge point’s SIM card.
            /// </summary>
            public String                   IMSI                        { get; set; }

            /// <summary>
            /// The optional meter type of the main power meter of the charge point.
            /// </summary>
            public String                   MeterType                   { get; set; }

            /// <summary>
            /// The optional serial number of the main power meter of the charge point.
            /// </summary>
            public String                   MeterSerialNumber           { get; set; }

            /// <summary>
            /// The optional public key of the main power meter of the charge point.
            /// </summary>
            public String                   MeterPublicKey              { get; set; }


            /// <summary>
            /// The time span between expected heartbeat requests.
            /// </summary>
            public TimeSpan                 ExpectHeartbeatEvery        { get; set; }

            /// <summary>
            /// The time at the central system.
            /// </summary>
            public DateTime?                CSMSTime           { get; set; }



            /// <summary>
            /// An collection of multi-language tags and their relevance.
            /// </summary>
            [Optional]
            public Tags                   Tags                 { get; set; }

            /// <summary>
            /// The user will be shown in chargeBox listings.
            /// </summary>
            [Mandatory]
            public Boolean                IsDisabled           { get; set; }

            /// <summary>
            /// An enumeration of attached files.
            /// </summary>
            [Optional]
            public HashSet<AttachedFile>  AttachedFiles        { get; }

            #endregion

            #region Edges

            #region User          -> ChargingStation edges

            //protected readonly List<User2ChargingStationEdge> _User2ChargingStation_Edges;

            //public IEnumerable<User2ChargingStationEdge> User2ChargingStationEdges
            //    => _User2ChargingStation_Edges;


            //#region LinkUser(Edge)

            //public User2ChargingStationEdge

            //    LinkUser(User2ChargingStationEdge Edge)

            //    => _User2ChargingStation_Edges.AddAndReturnElement(Edge);

            //#endregion

            //#region LinkUser(Source, EdgeLabel, PrivacyLevel = PrivacyLevel.World)

            //public User2ChargingStationEdge

            //    LinkUser(User                    Source,
            //             User2ChargingStationEdgeLabel  EdgeLabel,
            //             PrivacyLevel            PrivacyLevel = PrivacyLevel.World)

            //    => _User2ChargingStation_Edges.
            //           AddAndReturnElement(new User2ChargingStationEdge(Source,
            //                                                                                        EdgeLabel,
            //                                                                                        this,
            //                                                                                        PrivacyLevel));

            //#endregion


            //#region User2ChargingStationInEdges     (User)

            ///// <summary>
            ///// The edge labels of all (incoming) edges between the given user and this chargeBox.
            ///// </summary>
            //public IEnumerable<User2ChargingStationEdge> User2ChargingStationInEdges(User User)

            //    => _User2ChargingStation_Edges.
            //           Where(edge => edge.Source == User);

            //#endregion

            //#region User2ChargingStationInEdgeLabels(User)

            ///// <summary>
            ///// The edge labels of all (incoming) edges between the given user and this chargeBox.
            ///// </summary>
            //public IEnumerable<User2ChargingStationEdgeLabel> User2ChargingStationInEdgeLabels(User User)

            //    => _User2ChargingStation_Edges.
            //           Where (edge => edge.Source == User).
            //           Select(edge => edge.EdgeLabel);

            //#endregion

            //public IEnumerable<User2ChargingStationEdge>

            //    Add(IEnumerable<User2ChargingStationEdge> Edges)

            //        => _User2ChargingStation_Edges.AddAndReturnList(Edges);


            //#region UnlinkUser(EdgeLabel, User)

            //public void UnlinkUser(User2ChargingStationEdgeLabel  EdgeLabel,
            //                       User                    User)
            //{

            //    var edges = _User2ChargingStation_Edges.
            //                    Where(edge => edge.EdgeLabel == EdgeLabel &&
            //                                  edge.Source    == User).
            //                    ToArray();

            //    foreach (var edge in edges)
            //        _User2ChargingStation_Edges.Remove(edge);

            //}

            //#endregion

            //public Boolean RemoveInEdge(User2ChargingStationEdge Edge)
            //    => _User2ChargingStation_Edges.Remove(Edge);

            #endregion

            #region ChargingStation <-> ChargingStation edges

            protected readonly List<ChargingStation2ChargingStationEdge> _ChargingStation2ChargingStation_InEdges;

            public IEnumerable<ChargingStation2ChargingStationEdge> ChargingStation2ChargingStationInEdges
                => _ChargingStation2ChargingStation_InEdges;

            #region AddInEdge (Edge)

            public ChargingStation2ChargingStationEdge

                AddInEdge(ChargingStation2ChargingStationEdge Edge)

                => _ChargingStation2ChargingStation_InEdges.AddAndReturnElement(Edge);

            #endregion

            #region AddInEdge (EdgeLabel, SourceChargingStation, PrivacyLevel = PrivacyLevel.World)

            public ChargingStation2ChargingStationEdge

                AddInEdge (ChargingStation2ChargingStationEdgeLabel  EdgeLabel,
                           ChargingStation                           SourceChargingStation,
                           PrivacyLevel                              PrivacyLevel = PrivacyLevel.World)

                => _ChargingStation2ChargingStation_InEdges. AddAndReturnElement(new ChargingStation2ChargingStationEdge(SourceChargingStation,
                                                                                                                         EdgeLabel,
                                                                                                                         this,
                                                                                                                         PrivacyLevel));

            #endregion

            public IEnumerable<ChargingStation2ChargingStationEdge>

                AddInEdges(IEnumerable<ChargingStation2ChargingStationEdge> Edges)

                    => _ChargingStation2ChargingStation_InEdges.AddAndReturnList(Edges);

            #region RemoveInEdges(EdgeLabel, TargetChargingStation)

            public Boolean RemoveInEdge(ChargingStation2ChargingStationEdge Edge)
                => _ChargingStation2ChargingStation_InEdges.Remove(Edge);

            #endregion

            #region RemoveInEdges (EdgeLabel, SourceChargingStation)

            public void RemoveInEdges(ChargingStation2ChargingStationEdgeLabel EdgeLabel,
                                      ChargingStation SourceChargingStation)
            {

                var edges = _ChargingStation2ChargingStation_OutEdges.
                                Where(edge => edge.EdgeLabel == EdgeLabel &&
                                              edge.Source == SourceChargingStation).
                                ToArray();

                foreach (var edge in edges)
                    _ChargingStation2ChargingStation_InEdges.Remove(edge);

            }

            #endregion



            protected readonly List<ChargingStation2ChargingStationEdge> _ChargingStation2ChargingStation_OutEdges;

            public IEnumerable<ChargingStation2ChargingStationEdge> ChargingStation2ChargingStationOutEdges
                => _ChargingStation2ChargingStation_OutEdges;

            #region AddOutEdge(Edge)

            public ChargingStation2ChargingStationEdge

                AddOutEdge(ChargingStation2ChargingStationEdge Edge)

                => _ChargingStation2ChargingStation_OutEdges.AddAndReturnElement(Edge);

            #endregion

            #region AddOutEdge(EdgeLabel, TargetChargingStation, PrivacyLevel = PrivacyLevel.World)

            public ChargingStation2ChargingStationEdge

                AddOutEdge(ChargingStation2ChargingStationEdgeLabel  EdgeLabel,
                           ChargingStation                    TargetChargingStation,
                           PrivacyLevel                    PrivacyLevel = PrivacyLevel.World)

                => _ChargingStation2ChargingStation_OutEdges.AddAndReturnElement(new ChargingStation2ChargingStationEdge(this,
                                                                                                                                                    EdgeLabel,
                                                                                                                                                    TargetChargingStation,
                                                                                                                                                    PrivacyLevel));

            #endregion

            public IEnumerable<ChargingStation2ChargingStationEdge>

                AddOutEdges(IEnumerable<ChargingStation2ChargingStationEdge> Edges)

                    => _ChargingStation2ChargingStation_OutEdges.AddAndReturnList(Edges);

            #region RemoveOutEdges(EdgeLabel, TargetChargingStation)

            public Boolean RemoveOutEdge(ChargingStation2ChargingStationEdge Edge)
                => _ChargingStation2ChargingStation_OutEdges.Remove(Edge);

            #endregion

            #region RemoveOutEdges(EdgeLabel, TargetChargingStation)

            public void RemoveOutEdges(ChargingStation2ChargingStationEdgeLabel  EdgeLabel,
                                       ChargingStation                    TargetChargingStation)
            {

                var edges = _ChargingStation2ChargingStation_OutEdges.
                                Where(edge => edge.EdgeLabel == EdgeLabel &&
                                              edge.Target    == TargetChargingStation).
                                ToArray();

                foreach (var edge in edges)
                    _ChargingStation2ChargingStation_OutEdges.Remove(edge);

            }

            #endregion

            #endregion

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new chargeBox builder.
            /// </summary>
            /// <param name="Id">The unique identification of the chargeBox.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(ChargingStation_Id                                 Id,
                           Byte                                               NumberOfConnectors,
                           String                                             ChargePointVendor,
                           String                                             ChargePointModel,

                           I18NString?                                        Description                               = null,
                           String?                                            ChargePointSerialNumber                   = null,
                           //String?                                            ChargingStationSerialNumber               = null,
                           String?                                            FirmwareVersion                           = null,
                           String?                                            Iccid                                     = null,
                           String?                                            IMSI                                      = null,
                           String?                                            MeterType                                 = null,
                           String?                                            MeterSerialNumber                         = null,
                           String?                                            MeterPublicKey                            = null,

                           TimeSpan?                                          ExpectHeartbeatEvery                      = null,

                           IEnumerable<ANotification>?                        Notifications                             = null,

                           //IEnumerable<User2ChargingStationEdge>?             User2ChargingStationEdges                 = null,
                           IEnumerable<ChargingStation2ChargingStationEdge>?  ChargingStation2ChargingStationInEdges    = null,
                           IEnumerable<ChargingStation2ChargingStationEdge>?  ChargingStation2ChargingStationOutEdges   = null,

                           JObject?                                           CustomData                                = null,
                           IEnumerable<AttachedFile>?                         AttachedFiles                             = null,
                           JSONLDContext?                                     JSONLDContext                             = null,
                           String?                                            DataSource                                = null,
                           DateTime?                                          Created                                   = null,
                           DateTime?                                          LastChange                                = null)

                : base(Id,
                       JSONLDContext ?? DefaultJSONLDContext,
                       Created,
                       LastChange,
                       null,
                       CustomData,
                       null,
                       DataSource)

            {


                this.AttachedFiles                        = AttachedFiles.SafeAny() ? new HashSet<AttachedFile>(AttachedFiles) : new HashSet<AttachedFile>();

                this._Notifications                       = new NotificationStore();

                if (Notifications.SafeAny())
                    _Notifications.Add(Notifications);

               // this._User2ChargingStation_Edges             = User2ChargingStationEdges.           IsNeitherNullNorEmpty() ? new List<User2ChargingStationEdge>        (User2ChargingStationEdges)            : new List<User2ChargingStationEdge>();
                this._ChargingStation2ChargingStation_InEdges   = ChargingStation2ChargingStationInEdges. IsNeitherNullNorEmpty() ? new List<ChargingStation2ChargingStationEdge>(ChargingStation2ChargingStationInEdges)  : new List<ChargingStation2ChargingStationEdge>();
                this._ChargingStation2ChargingStation_OutEdges  = ChargingStation2ChargingStationOutEdges.IsNeitherNullNorEmpty() ? new List<ChargingStation2ChargingStationEdge>(ChargingStation2ChargingStationOutEdges) : new List<ChargingStation2ChargingStationEdge>();

            }

            #endregion


            #region Notifications

            private readonly NotificationStore _Notifications;

            #region (internal) AddNotification(Notification,                           OnUpdate = null)

            internal T AddNotification<T>(T          Notification,
                                          Action<T>  OnUpdate  = null)

                where T : ANotification

                => _Notifications.Add(Notification,
                                      OnUpdate);

            #endregion

            #region (internal) AddNotification(Notification, NotificationMessageType,  OnUpdate = null)

            internal T AddNotification<T>(T                        Notification,
                                          NotificationMessageType  NotificationMessageType,
                                          Action<T>                OnUpdate  = null)

                where T : ANotification

                => _Notifications.Add(Notification,
                                      NotificationMessageType,
                                      OnUpdate);

            #endregion

            #region (internal) AddNotification(Notification, NotificationMessageTypes, OnUpdate = null)

            internal T AddNotification<T>(T                                     Notification,
                                          IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                          Action<T>                             OnUpdate  = null)

                where T : ANotification

                => _Notifications.Add(Notification,
                                      NotificationMessageTypes,
                                      OnUpdate);

            #endregion


            #region GetNotifications  (NotificationMessageType = null)

            public IEnumerable<ANotification> GetNotifications(NotificationMessageType?  NotificationMessageType = null)
            {
                lock (_Notifications)
                {
                    return _Notifications.GetNotifications(NotificationMessageType);
                }
            }

            #endregion

            #region GetNotificationsOf(params NotificationMessageTypes)

            public IEnumerable<T> GetNotificationsOf<T>(params NotificationMessageType[] NotificationMessageTypes)

                where T : ANotification

            {

                lock (_Notifications)
                {
                    return _Notifications.GetNotificationsOf<T>(NotificationMessageTypes);
                }

            }

            #endregion

            #region GetNotifications  (NotificationMessageTypeFilter)

            public IEnumerable<ANotification> GetNotifications(Func<NotificationMessageType, Boolean> NotificationMessageTypeFilter)
            {
                lock (_Notifications)
                {
                    return _Notifications.GetNotifications(NotificationMessageTypeFilter);
                }
            }

            #endregion

            #region GetNotificationsOf(NotificationMessageTypeFilter)

            public IEnumerable<T> GetNotificationsOf<T>(Func<NotificationMessageType, Boolean> NotificationMessageTypeFilter)

                where T : ANotification

            {

                lock (_Notifications)
                {
                    return _Notifications.GetNotificationsOf<T>(NotificationMessageTypeFilter);
                }

            }

            #endregion


            #region GetNotificationInfos()

            public JObject GetNotificationInfos()

                => JSONObject.Create(new JProperty("user", JSONObject.Create(

                                         //new JProperty("name",               EMail.OwnerName),
                                         //new JProperty("email",              EMail.Address.ToString())

                                         //MobilePhone.HasValue
                                         //    ? new JProperty("phoneNumber",  MobilePhone.Value.ToString())
                                         //    : null

                                     )),
                                     new JProperty("notifications",  _Notifications.ToJSON()));

            #endregion


            #region (internal) RemoveNotification(NotificationType,                           OnRemoval = null)

            //internal T RemoveNotification<T>(T          NotificationType,
            //                                 Action<T>  OnRemoval  = null)

            //    where T : ANotification

            //    => _Notifications.Remove(NotificationType,
            //                             OnRemoval);

            #endregion

            #endregion


            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the chargeBox.
            /// </summary>
            /// <param name="Builder">An chargeBox builder.</param>
            public static implicit operator ChargingStation(Builder Builder)

                => Builder?.ToImmutable;


            /// <summary>
            /// Return an immutable version of the chargeBox.
            /// </summary>
            public ChargingStation ToImmutable
            {
                get
                {

                    //if (!Branch.HasValue || Branch.Value.IsNullOrEmpty)
                    //    throw new ArgumentNullException(nameof(Branch), "The given branch must not be null or empty!");

                    return new ChargingStation(Id,
                                         1,
                                         ChargePointVendor,
                                         ChargePointModel,

                                         Description,
                                         ChargePointSerialNumber,
                                         //ChargingStationSerialNumber,
                                         FirmwareVersion,
                                         Iccid,
                                         IMSI,
                                         MeterType,
                                         MeterSerialNumber,
                                         MeterPublicKey,

                                         ExpectHeartbeatEvery,

                                                          _Notifications,

                                                          //_User2ChargingStation_Edges,
                                                          _ChargingStation2ChargingStation_InEdges,
                                                          _ChargingStation2ChargingStation_OutEdges,

                                                          CustomData,
                                                          AttachedFiles,
                                                          JSONLDContext,
                                                          DataSource,
                                                          LastChangeDate);

                }
            }

            #endregion


            #region CopyAllLinkedDataFromBase(OldChargingStation)

            public override void CopyAllLinkedDataFromBase(ChargingStation OldChargingStation)
            {

                //if (OldChargingStation._User2ChargingStation_Edges.Any() && !_User2ChargingStation_Edges.Any())
                //{

                //    Add(OldChargingStation._User2ChargingStation_Edges);

                //    foreach (var edge in _User2ChargingStation_Edges)
                //        edge.Target = this;

                //}

                //if (OldChargingStation._ChargingStation2ChargingStation_InEdges.Any() && !_ChargingStation2ChargingStation_InEdges.Any())
                //{

                //    AddInEdges(OldChargingStation._ChargingStation2ChargingStation_InEdges);

                //    foreach (var edge in _ChargingStation2ChargingStation_InEdges)
                //        edge.Target = this;

                //}

                //if (OldChargingStation._ChargingStation2ChargingStation_OutEdges.Any() && !_ChargingStation2ChargingStation_OutEdges.Any())
                //{

                //    AddOutEdges(OldChargingStation._ChargingStation2ChargingStation_OutEdges);

                //    foreach (var edge in _ChargingStation2ChargingStation_OutEdges)
                //        edge.Source = this;

                //}

                //if (OldChargingStation._Notifications.SafeAny() && !_Notifications.SafeAny())
                //    _Notifications.Add(OldChargingStation._Notifications);

            }

            #endregion


            #region Operator overloading

            #region Operator == (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A chargeBox builder.</param>
            /// <param name="BuilderId2">Another chargeBox builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator == (Builder BuilderId1, Builder BuilderId2)
            {

                // If both are null, or both are same instance, return true.
                if (Object.ReferenceEquals(BuilderId1, BuilderId2))
                    return true;

                // If one is null, but not both, return false.
                if ((BuilderId1 is null) || (BuilderId2 is null))
                    return false;

                return BuilderId1.Equals(BuilderId2);

            }

            #endregion

            #region Operator != (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A chargeBox builder.</param>
            /// <param name="BuilderId2">Another chargeBox builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator != (Builder BuilderId1, Builder BuilderId2)
                => !(BuilderId1 == BuilderId2);

            #endregion

            #region Operator <  (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A chargeBox builder.</param>
            /// <param name="BuilderId2">Another chargeBox builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator < (Builder BuilderId1, Builder BuilderId2)
            {

                if (BuilderId1 is null)
                    throw new ArgumentNullException(nameof(BuilderId1), "The given BuilderId1 must not be null!");

                return BuilderId1.CompareTo(BuilderId2) < 0;

            }

            #endregion

            #region Operator <= (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A chargeBox builder.</param>
            /// <param name="BuilderId2">Another chargeBox builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator <= (Builder BuilderId1, Builder BuilderId2)
                => !(BuilderId1 > BuilderId2);

            #endregion

            #region Operator >  (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A chargeBox builder.</param>
            /// <param name="BuilderId2">Another chargeBox builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator > (Builder BuilderId1, Builder BuilderId2)
            {

                if (BuilderId1 is null)
                    throw new ArgumentNullException(nameof(BuilderId1), "The given BuilderId1 must not be null!");

                return BuilderId1.CompareTo(BuilderId2) > 0;

            }

            #endregion

            #region Operator >= (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A chargeBox builder.</param>
            /// <param name="BuilderId2">Another chargeBox builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator >= (Builder BuilderId1, Builder BuilderId2)
                => !(BuilderId1 < BuilderId2);

            #endregion

            #endregion

            #region IComparable<Builder> Members

            #region CompareTo(Object)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Object">An object to compare with.</param>
            public override Int32 CompareTo(Object Object)

                => Object is Builder Builder
                       ? CompareTo(Builder)
                       : throw new ArgumentException("The given object is not an chargeBox!");

            #endregion

            #region CompareTo(ChargingStation)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="ChargingStation">An chargeBox object to compare with.</param>
            public override Int32 CompareTo(ChargingStation? ChargingStation)

                => ChargingStation is null
                       ? throw new ArgumentNullException(nameof(ChargingStation), "The given chargeBox must not be null!")
                       : Id.CompareTo(ChargingStation.Id);


            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder">An chargeBox object to compare with.</param>
            public Int32 CompareTo(Builder Builder)

                => Builder is null
                       ? throw new ArgumentNullException(nameof(Builder), "The given chargeBox must not be null!")
                       : Id.CompareTo(Builder.Id);

            #endregion

            #endregion

            #region IEquatable<Builder> Members

            #region Equals(Object)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Object">An object to compare with.</param>
            /// <returns>true|false</returns>
            public override Boolean Equals(Object Object)

                => Object is Builder Builder &&
                      Equals(Builder);

            #endregion

            #region Equals(ChargingStation)

            /// <summary>
            /// Compares two chargeBoxs for equality.
            /// </summary>
            /// <param name="ChargingStation">An chargeBox to compare with.</param>
            /// <returns>True if both match; False otherwise.</returns>
            public override Boolean Equals(ChargingStation? ChargingStation)

                => ChargingStation is not null &&
                       Id.Equals(ChargingStation.Id);


            /// <summary>
            /// Compares two chargeBoxs for equality.
            /// </summary>
            /// <param name="Builder">An chargeBox to compare with.</param>
            /// <returns>True if both match; False otherwise.</returns>
            public Boolean Equals(Builder Builder)

                => Builder is not null &&
                       Id.Equals(Builder.Id);

            #endregion

            #endregion

            #region (override) GetHashCode()

            /// <summary>
            /// Get the hash code of this object.
            /// </summary>
            public override Int32 GetHashCode()
                => Id.GetHashCode();

            #endregion

            #region (override) ToString()

            /// <summary>
            /// Return a text representation of this object.
            /// </summary>
            public override String ToString()
                => Id.ToString();

            #endregion

        }

        #endregion

    }

}
