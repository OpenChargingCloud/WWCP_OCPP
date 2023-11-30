/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using org.GraphDefined.Vanaheimr.Hermod.HTTP.Notifications;

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    public delegate Boolean ChargeBoxProviderDelegate(ChargingStation_Id ChargeBoxId, out ChargeBox ChargeBox);

    public delegate JObject ChargeBoxToJSONDelegate(ChargeBox     ChargeBox,
                                                    Boolean       Embedded                 = false,
                                                    InfoStatus    ExpandTags               = InfoStatus.ShowIdOnly,
                                                    Boolean       IncludeCryptoHash        = true);

    /// <summary>
    /// Extension methods for chargeBoxs.
    /// </summary>
    public static class ChargeBoxExtensions
    {

        #region ToJSON(this ChargeBoxs, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of chargeBoxs.
        /// </summary>
        /// <param name="ChargeBoxs">An enumeration of chargeBoxs.</param>
        /// <param name="Skip">The optional number of chargeBoxs to skip.</param>
        /// <param name="Take">The optional number of chargeBoxs to return.</param>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        public static JArray ToJSON(this IEnumerable<ChargeBox>  ChargeBoxs,
                                    UInt64?                      Skip                  = null,
                                    UInt64?                      Take                  = null,
                                    Boolean                      Embedded              = false,
                                    InfoStatus                   ExpandTags            = InfoStatus.ShowIdOnly,
                                    ChargeBoxToJSONDelegate      ChargeBoxToJSON       = null,
                                    Boolean                      IncludeCryptoHash     = true)


            => ChargeBoxs?.Any() != true

                   ? new JArray()

                   : new JArray(ChargeBoxs.
                                    Where     (chargeBox => chargeBox != null).
                                    OrderBy   (chargeBox => chargeBox.Id).
                                    SkipTakeFilter(Skip, Take).
                                    SafeSelect(chargeBox => ChargeBoxToJSON != null
                                                                    ? ChargeBoxToJSON (chargeBox,
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
    public class ChargeBox : AEntity<ChargingStation_Id,
                                     ChargeBox>
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
        public const UInt16 DefaultChargeBoxStatusHistorySize = 50;

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
        //public String?                  ChargeBoxSerialNumber       { get; }

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
        public event OnBootNotificationRequestDelegate?   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        public event OnBootNotificationResponseDelegate?  OnBootNotificationResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat request will be sent to the central system.
        /// </summary>
        public event OnHeartbeatRequestDelegate?   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        public event OnHeartbeatResponseDelegate?  OnHeartbeatResponse;

        #endregion


        #region OnAuthorizeRequest/-Response

        /// <summary>
        /// An event fired whenever an authorize request will be sent to the central system.
        /// </summary>
        public event OnAuthorizeRequestDelegate?   OnAuthorizeRequest;

        /// <summary>
        /// An event fired whenever a response to an authorize request was received.
        /// </summary>
        public event OnAuthorizeResponseDelegate?  OnAuthorizeResponse;

        #endregion

        #region OnStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a status notification request will be sent to the central system.
        /// </summary>
        public event OnStatusNotificationRequestDelegate?   OnStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a status notification request was received.
        /// </summary>
        public event OnStatusNotificationResponseDelegate?  OnStatusNotificationResponse;

        #endregion

        #region OnMeterValuesRequest/-Response

        /// <summary>
        /// An event fired whenever a meter values request will be sent to the central system.
        /// </summary>
        public event OnMeterValuesRequestDelegate?   OnMeterValuesRequest;

        /// <summary>
        /// An event fired whenever a response to a meter values request was received.
        /// </summary>
        public event OnMeterValuesResponseDelegate?  OnMeterValuesResponse;

        #endregion


        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer request will be sent to the central system.
        /// </summary>
        public event OnDataTransferRequestDelegate?   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        public event OnDataTransferResponseDelegate?  OnDataTransferResponse;

        #endregion

        #region OnFirmwareStatusNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a firmware status notification request will be sent to the central system.
        /// </summary>
        public event OnFirmwareStatusNotificationRequestDelegate?   OnFirmwareStatusNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a firmware status notification request was received.
        /// </summary>
        public event OnFirmwareStatusNotificationResponseDelegate?  OnFirmwareStatusNotificationResponse;

        #endregion



        // Incoming messages (from central system)

        #region OnResetRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnResetRequestDelegate?   OnResetRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnResetResponseDelegate?  OnResetResponse;

        #endregion

        #region ChangeAvailabilityRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        #endregion

        #region OnIncomingDataTransferRequest/-Response

        /// <summary>
        /// An event sent whenever a data transfer request was received.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate?   OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate?  OnIncomingDataTransferResponse;

        #endregion

        #region TriggerMessageRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        #endregion

        #region UpdateFirmwareRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        #endregion


        #region OnReserveNowRequest/-Response

        /// <summary>
        /// An event sent whenever a reserve now request was received.
        /// </summary>
        public event OnReserveNowRequestDelegate?   OnReserveNowRequest;

        /// <summary>
        /// An event sent whenever a response to a reserve now request was sent.
        /// </summary>
        public event OnReserveNowResponseDelegate?  OnReserveNowResponse;

        #endregion

        #region OnCancelReservationRequest/-Response

        /// <summary>
        /// An event sent whenever a cancel reservation request was received.
        /// </summary>
        public event OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        /// <summary>
        /// An event sent whenever a response to a cancel reservation request was sent.
        /// </summary>
        public event OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        #endregion

        #region SetChargingProfileRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        #endregion

        #region ClearChargingProfileRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        #endregion

        #region GetCompositeScheduleRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        #endregion

        #region UnlockConnectorRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        #endregion


        #region GetLocalListVersionRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        #endregion

        #region SendLocalListRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        #endregion

        #region ClearCacheRequest/-Response

        /// <summary>
        /// An event sent whenever a reset request was received.
        /// </summary>
        public event OnClearCacheRequestDelegate?   OnClearCacheRequest;

        /// <summary>
        /// An event sent whenever a response to a reset request was sent.
        /// </summary>
        public event OnClearCacheResponseDelegate?  OnClearCacheResponse;

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
        /// <param name="ChargeBoxSerialNumber">An optional serial number of the charge point.</param>
        /// <param name="FirmwareVersion">An optional firmware version of the charge point.</param>
        /// <param name="Iccid">An optional ICCID of the charge point's SIM card.</param>
        /// <param name="IMSI">An optional IMSI of the charge point’s SIM card.</param>
        /// <param name="MeterType">An optional meter type of the main power meter of the charge point.</param>
        /// <param name="MeterSerialNumber">An optional serial number of the main power meter of the charge point.</param>
        /// <param name="MeterPublicKey">An optional public key of the main power meter of the charge point.</param>
        /// 
        /// <param name="ExpectHeartbeatEvery">The time span between expected heartbeat.</param>
        public ChargeBox(ChargingStation_Id                                Id,
                         Byte                                        NumberOfConnectors,
                         String                                      ChargePointVendor,
                         String                                      ChargePointModel,

                         I18NString?                                 Description                         = null,
                         String?                                     ChargePointSerialNumber             = null,
                         //String?                                     ChargeBoxSerialNumber               = null,
                         String?                                     FirmwareVersion                     = null,
                         String?                                     Iccid                               = null,
                         String?                                     IMSI                                = null,
                         String?                                     MeterType                           = null,
                         String?                                     MeterSerialNumber                   = null,
                         String?                                     MeterPublicKey                      = null,

                         TimeSpan?                                   ExpectHeartbeatEvery                = null,

                         IEnumerable<ANotification>?                 Notifications                       = null,

                         //IEnumerable<Organization2ChargeBoxEdge>?    Organization2ChargeBoxEdges         = null,
                         IEnumerable<ChargeBox2ChargeBoxEdge>?       ChargeBox2ChargeBoxInEdges          = null,
                         IEnumerable<ChargeBox2ChargeBoxEdge>?       ChargeBox2ChargeBoxOutEdges         = null,

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
            //this.ChargeBoxSerialNumber    = ChargeBoxSerialNumber;
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


        #region ToJSON(CustomChargeBoxSerializer = null)

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
        /// <param name="CustomChargeBoxSerializer">A delegate to serialize custom charging stationes.</param>
        public JObject ToJSON(Boolean                                      Embedded                    = false,
                              InfoStatus                                   ExpandTags                  = InfoStatus.ShowIdOnly,
                              Boolean                                      IncludeLastChange           = true,
                              CustomJObjectSerializerDelegate<ChargeBox>?  CustomChargeBoxSerializer   = null)
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

                                       //ChargeBoxSerialNumber.  IsNotNullOrEmpty()
                                       //    ? new JProperty("chargeBoxSerialNumber",     ChargeBoxSerialNumber)
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

            return CustomChargeBoxSerializer is not null
                       ? CustomChargeBoxSerializer(this, json)
                       : json;

        }

        #endregion


        #region CopyAllLinkedDataFromBase(OldChargeBox)

        public override void CopyAllLinkedDataFromBase(ChargeBox OldChargeBox)
        {

            //if (OldChargeBox._User2ChargeBox_Edges.Any() && !_User2ChargeBox_Edges.Any())
            //{

            //    AddUsers(OldChargeBox._User2ChargeBox_Edges);

            //    foreach (var edge in _User2ChargeBox_Edges)
            //        edge.Target = this;

            //}

            //if (OldChargeBox._ChargeBox2ChargeBox_InEdges.Any() && !_ChargeBox2ChargeBox_InEdges.Any())
            //{

            //    AddEdges(OldChargeBox._ChargeBox2ChargeBox_InEdges);

            //    foreach (var edge in _ChargeBox2ChargeBox_InEdges)
            //        edge.Target = this;

            //}

            //if (OldChargeBox._ChargeBox2ChargeBox_OutEdges.Any() && !_ChargeBox2ChargeBox_OutEdges.Any())
            //{

            //    AddEdges(OldChargeBox._ChargeBox2ChargeBox_OutEdges);

            //    foreach (var edge in _ChargeBox2ChargeBox_OutEdges)
            //        edge.Source = this;

            //}

            //if (OldChargeBox._Notifications.SafeAny() && !_Notifications.SafeAny())
            //    _Notifications.Add(OldChargeBox._Notifications);

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">A chargeBox identification.</param>
        /// <param name="ChargeBoxId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargeBox ChargeBoxId1, ChargeBox ChargeBoxId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ChargeBoxId1, ChargeBoxId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ChargeBoxId1 == null) || ((Object) ChargeBoxId2 == null))
                return false;

            return ChargeBoxId1.Equals(ChargeBoxId2);

        }

        #endregion

        #region Operator != (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">A chargeBox identification.</param>
        /// <param name="ChargeBoxId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargeBox ChargeBoxId1, ChargeBox ChargeBoxId2)
            => !(ChargeBoxId1 == ChargeBoxId2);

        #endregion

        #region Operator <  (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">A chargeBox identification.</param>
        /// <param name="ChargeBoxId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargeBox ChargeBoxId1, ChargeBox ChargeBoxId2)
        {

            if ((Object) ChargeBoxId1 == null)
                throw new ArgumentNullException(nameof(ChargeBoxId1), "The given ChargeBoxId1 must not be null!");

            return ChargeBoxId1.CompareTo(ChargeBoxId2) < 0;

        }

        #endregion

        #region Operator <= (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">A chargeBox identification.</param>
        /// <param name="ChargeBoxId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargeBox ChargeBoxId1, ChargeBox ChargeBoxId2)
            => !(ChargeBoxId1 > ChargeBoxId2);

        #endregion

        #region Operator >  (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">A chargeBox identification.</param>
        /// <param name="ChargeBoxId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargeBox ChargeBoxId1, ChargeBox ChargeBoxId2)
        {

            if ((Object) ChargeBoxId1 == null)
                throw new ArgumentNullException(nameof(ChargeBoxId1), "The given ChargeBoxId1 must not be null!");

            return ChargeBoxId1.CompareTo(ChargeBoxId2) > 0;

        }

        #endregion

        #region Operator >= (ChargeBoxId1, ChargeBoxId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBoxId1">A chargeBox identification.</param>
        /// <param name="ChargeBoxId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargeBox ChargeBoxId1, ChargeBox ChargeBoxId2)
            => !(ChargeBoxId1 < ChargeBoxId2);

        #endregion

        #endregion

        #region IComparable<ChargeBox> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Int32 CompareTo(Object Object)

            => Object is ChargeBox ChargeBox
                   ? CompareTo(ChargeBox)
                   : throw new ArgumentException("The given object is not an chargeBox!", nameof(Object));

        #endregion

        #region CompareTo(ChargeBox)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargeBox">An chargeBox object to compare with.</param>
        public override Int32 CompareTo(ChargeBox ChargeBox)

            => ChargeBox is null
                   ? throw new ArgumentNullException(nameof(ChargeBox), "The given chargeBox must not be null!")
                   : Id.CompareTo(ChargeBox.Id);

        #endregion

        #endregion

        #region IEquatable<ChargeBox> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ChargeBox ChargeBox &&
                  Equals(ChargeBox);

        #endregion

        #region Equals(ChargeBox)

        /// <summary>
        /// Compares two chargeBoxs for equality.
        /// </summary>
        /// <param name="ChargeBox">An chargeBox to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ChargeBox ChargeBox)

            => ChargeBox is ChargeBox &&
                   Id.Equals(ChargeBox.Id);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
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


        #region ToBuilder(NewChargeBoxId = null)

        /// <summary>
        /// Return a builder for this chargeBox.
        /// </summary>
        /// <param name="NewChargeBoxId">An optional new chargeBox identification.</param>
        public Builder ToBuilder(ChargingStation_Id? NewChargeBoxId = null)

            => new (NewChargeBoxId ?? Id,
                    1, //NumberOfConnectors,
                    ChargePointVendor,
                    ChargePointModel,

                    Description,
                    ChargePointSerialNumber,
                    //ChargeBoxSerialNumber,
                    FirmwareVersion,
                    Iccid,
                    IMSI,
                    MeterType,
                    MeterSerialNumber,
                    MeterPublicKey,

                    ExpectHeartbeatEvery,

                    null, //_Notifications,

                    //_User2ChargeBox_Edges,
                    null, //_ChargeBox2ChargeBox_InEdges,
                    null, //_ChargeBox2ChargeBox_OutEdges,

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
        public new class Builder : AEntity<ChargingStation_Id, ChargeBox>.Builder
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
           // public String                   ChargeBoxSerialNumber       { get; set; }

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

            #region User          -> ChargeBox edges

            //protected readonly List<User2ChargeBoxEdge> _User2ChargeBox_Edges;

            //public IEnumerable<User2ChargeBoxEdge> User2ChargeBoxEdges
            //    => _User2ChargeBox_Edges;


            //#region LinkUser(Edge)

            //public User2ChargeBoxEdge

            //    LinkUser(User2ChargeBoxEdge Edge)

            //    => _User2ChargeBox_Edges.AddAndReturnElement(Edge);

            //#endregion

            //#region LinkUser(Source, EdgeLabel, PrivacyLevel = PrivacyLevel.World)

            //public User2ChargeBoxEdge

            //    LinkUser(User                    Source,
            //             User2ChargeBoxEdgeLabel  EdgeLabel,
            //             PrivacyLevel            PrivacyLevel = PrivacyLevel.World)

            //    => _User2ChargeBox_Edges.
            //           AddAndReturnElement(new User2ChargeBoxEdge(Source,
            //                                                                                        EdgeLabel,
            //                                                                                        this,
            //                                                                                        PrivacyLevel));

            //#endregion


            //#region User2ChargeBoxInEdges     (User)

            ///// <summary>
            ///// The edge labels of all (incoming) edges between the given user and this chargeBox.
            ///// </summary>
            //public IEnumerable<User2ChargeBoxEdge> User2ChargeBoxInEdges(User User)

            //    => _User2ChargeBox_Edges.
            //           Where(edge => edge.Source == User);

            //#endregion

            //#region User2ChargeBoxInEdgeLabels(User)

            ///// <summary>
            ///// The edge labels of all (incoming) edges between the given user and this chargeBox.
            ///// </summary>
            //public IEnumerable<User2ChargeBoxEdgeLabel> User2ChargeBoxInEdgeLabels(User User)

            //    => _User2ChargeBox_Edges.
            //           Where (edge => edge.Source == User).
            //           Select(edge => edge.EdgeLabel);

            //#endregion

            //public IEnumerable<User2ChargeBoxEdge>

            //    Add(IEnumerable<User2ChargeBoxEdge> Edges)

            //        => _User2ChargeBox_Edges.AddAndReturnList(Edges);


            //#region UnlinkUser(EdgeLabel, User)

            //public void UnlinkUser(User2ChargeBoxEdgeLabel  EdgeLabel,
            //                       User                    User)
            //{

            //    var edges = _User2ChargeBox_Edges.
            //                    Where(edge => edge.EdgeLabel == EdgeLabel &&
            //                                  edge.Source    == User).
            //                    ToArray();

            //    foreach (var edge in edges)
            //        _User2ChargeBox_Edges.Remove(edge);

            //}

            //#endregion

            //public Boolean RemoveInEdge(User2ChargeBoxEdge Edge)
            //    => _User2ChargeBox_Edges.Remove(Edge);

            #endregion

            #region ChargeBox <-> ChargeBox edges

            protected readonly List<ChargeBox2ChargeBoxEdge> _ChargeBox2ChargeBox_InEdges;

            public IEnumerable<ChargeBox2ChargeBoxEdge> ChargeBox2ChargeBoxInEdges
                => _ChargeBox2ChargeBox_InEdges;

            #region AddInEdge (Edge)

            public ChargeBox2ChargeBoxEdge

                AddInEdge(ChargeBox2ChargeBoxEdge Edge)

                => _ChargeBox2ChargeBox_InEdges.AddAndReturnElement(Edge);

            #endregion

            #region AddInEdge (EdgeLabel, SourceChargeBox, PrivacyLevel = PrivacyLevel.World)

            public ChargeBox2ChargeBoxEdge

                AddInEdge (ChargeBox2ChargeBoxEdgeLabel  EdgeLabel,
                           ChargeBox                    SourceChargeBox,
                           PrivacyLevel                    PrivacyLevel = PrivacyLevel.World)

                => _ChargeBox2ChargeBox_InEdges. AddAndReturnElement(new ChargeBox2ChargeBoxEdge(SourceChargeBox,
                                                                                                                                                    EdgeLabel,
                                                                                                                                                    this,
                                                                                                                                                    PrivacyLevel));

            #endregion

            public IEnumerable<ChargeBox2ChargeBoxEdge>

                AddInEdges(IEnumerable<ChargeBox2ChargeBoxEdge> Edges)

                    => _ChargeBox2ChargeBox_InEdges.AddAndReturnList(Edges);

            #region RemoveInEdges(EdgeLabel, TargetChargeBox)

            public Boolean RemoveInEdge(ChargeBox2ChargeBoxEdge Edge)
                => _ChargeBox2ChargeBox_InEdges.Remove(Edge);

            #endregion

            #region RemoveInEdges (EdgeLabel, SourceChargeBox)

            public void RemoveInEdges(ChargeBox2ChargeBoxEdgeLabel EdgeLabel,
                                      ChargeBox SourceChargeBox)
            {

                var edges = _ChargeBox2ChargeBox_OutEdges.
                                Where(edge => edge.EdgeLabel == EdgeLabel &&
                                              edge.Source == SourceChargeBox).
                                ToArray();

                foreach (var edge in edges)
                    _ChargeBox2ChargeBox_InEdges.Remove(edge);

            }

            #endregion



            protected readonly List<ChargeBox2ChargeBoxEdge> _ChargeBox2ChargeBox_OutEdges;

            public IEnumerable<ChargeBox2ChargeBoxEdge> ChargeBox2ChargeBoxOutEdges
                => _ChargeBox2ChargeBox_OutEdges;

            #region AddOutEdge(Edge)

            public ChargeBox2ChargeBoxEdge

                AddOutEdge(ChargeBox2ChargeBoxEdge Edge)

                => _ChargeBox2ChargeBox_OutEdges.AddAndReturnElement(Edge);

            #endregion

            #region AddOutEdge(EdgeLabel, TargetChargeBox, PrivacyLevel = PrivacyLevel.World)

            public ChargeBox2ChargeBoxEdge

                AddOutEdge(ChargeBox2ChargeBoxEdgeLabel  EdgeLabel,
                           ChargeBox                    TargetChargeBox,
                           PrivacyLevel                    PrivacyLevel = PrivacyLevel.World)

                => _ChargeBox2ChargeBox_OutEdges.AddAndReturnElement(new ChargeBox2ChargeBoxEdge(this,
                                                                                                                                                    EdgeLabel,
                                                                                                                                                    TargetChargeBox,
                                                                                                                                                    PrivacyLevel));

            #endregion

            public IEnumerable<ChargeBox2ChargeBoxEdge>

                AddOutEdges(IEnumerable<ChargeBox2ChargeBoxEdge> Edges)

                    => _ChargeBox2ChargeBox_OutEdges.AddAndReturnList(Edges);

            #region RemoveOutEdges(EdgeLabel, TargetChargeBox)

            public Boolean RemoveOutEdge(ChargeBox2ChargeBoxEdge Edge)
                => _ChargeBox2ChargeBox_OutEdges.Remove(Edge);

            #endregion

            #region RemoveOutEdges(EdgeLabel, TargetChargeBox)

            public void RemoveOutEdges(ChargeBox2ChargeBoxEdgeLabel  EdgeLabel,
                                       ChargeBox                    TargetChargeBox)
            {

                var edges = _ChargeBox2ChargeBox_OutEdges.
                                Where(edge => edge.EdgeLabel == EdgeLabel &&
                                              edge.Target    == TargetChargeBox).
                                ToArray();

                foreach (var edge in edges)
                    _ChargeBox2ChargeBox_OutEdges.Remove(edge);

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
            public Builder(ChargingStation_Id                             Id,
                         Byte                                        NumberOfConnectors,
                         String                                      ChargePointVendor,
                         String                                      ChargePointModel,

                         I18NString                                  Description                         = null,
                         String                                      ChargePointSerialNumber             = null,
                         //String                                      ChargeBoxSerialNumber               = null,
                         String                                      FirmwareVersion                     = null,
                         String                                      Iccid                               = null,
                         String                                      IMSI                                = null,
                         String                                      MeterType                           = null,
                         String                                      MeterSerialNumber                   = null,
                         String                                      MeterPublicKey                      = null,

                         TimeSpan?                                   ExpectHeartbeatEvery                = null,

                           IEnumerable<ANotification>                  Notifications                       = null,

                           //IEnumerable<User2ChargeBoxEdge>          User2ChargeBoxEdges              = null,
                           IEnumerable<ChargeBox2ChargeBoxEdge>  ChargeBox2ChargeBoxInEdges    = null,
                           IEnumerable<ChargeBox2ChargeBoxEdge>  ChargeBox2ChargeBoxOutEdges   = null,

                           JObject                                     CustomData                          = default,
                           IEnumerable<AttachedFile>                   AttachedFiles                       = default,
                           JSONLDContext?                              JSONLDContext                       = default,
                           String                                      DataSource                          = default,
                           DateTime?                                   LastChange                          = default)

                : base(Id,
                       JSONLDContext ?? DefaultJSONLDContext,
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

               // this._User2ChargeBox_Edges             = User2ChargeBoxEdges.           IsNeitherNullNorEmpty() ? new List<User2ChargeBoxEdge>        (User2ChargeBoxEdges)            : new List<User2ChargeBoxEdge>();
                this._ChargeBox2ChargeBox_InEdges   = ChargeBox2ChargeBoxInEdges. IsNeitherNullNorEmpty() ? new List<ChargeBox2ChargeBoxEdge>(ChargeBox2ChargeBoxInEdges)  : new List<ChargeBox2ChargeBoxEdge>();
                this._ChargeBox2ChargeBox_OutEdges  = ChargeBox2ChargeBoxOutEdges.IsNeitherNullNorEmpty() ? new List<ChargeBox2ChargeBoxEdge>(ChargeBox2ChargeBoxOutEdges) : new List<ChargeBox2ChargeBoxEdge>();

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
            public static implicit operator ChargeBox(Builder Builder)

                => Builder?.ToImmutable;


            /// <summary>
            /// Return an immutable version of the chargeBox.
            /// </summary>
            public ChargeBox ToImmutable
            {
                get
                {

                    //if (!Branch.HasValue || Branch.Value.IsNullOrEmpty)
                    //    throw new ArgumentNullException(nameof(Branch), "The given branch must not be null or empty!");

                    return new ChargeBox(Id,
                                         1,
                                         ChargePointVendor,
                                         ChargePointModel,

                                         Description,
                                         ChargePointSerialNumber,
                                         //ChargeBoxSerialNumber,
                                         FirmwareVersion,
                                         Iccid,
                                         IMSI,
                                         MeterType,
                                         MeterSerialNumber,
                                         MeterPublicKey,

                                         ExpectHeartbeatEvery,

                                                          _Notifications,

                                                          //_User2ChargeBox_Edges,
                                                          _ChargeBox2ChargeBox_InEdges,
                                                          _ChargeBox2ChargeBox_OutEdges,

                                                          CustomData,
                                                          AttachedFiles,
                                                          JSONLDContext,
                                                          DataSource,
                                                          LastChangeDate);

                }
            }

            #endregion


            #region CopyAllLinkedDataFromBase(OldChargeBox)

            public override void CopyAllLinkedDataFromBase(ChargeBox OldChargeBox)
            {

                //if (OldChargeBox._User2ChargeBox_Edges.Any() && !_User2ChargeBox_Edges.Any())
                //{

                //    Add(OldChargeBox._User2ChargeBox_Edges);

                //    foreach (var edge in _User2ChargeBox_Edges)
                //        edge.Target = this;

                //}

                //if (OldChargeBox._ChargeBox2ChargeBox_InEdges.Any() && !_ChargeBox2ChargeBox_InEdges.Any())
                //{

                //    AddInEdges(OldChargeBox._ChargeBox2ChargeBox_InEdges);

                //    foreach (var edge in _ChargeBox2ChargeBox_InEdges)
                //        edge.Target = this;

                //}

                //if (OldChargeBox._ChargeBox2ChargeBox_OutEdges.Any() && !_ChargeBox2ChargeBox_OutEdges.Any())
                //{

                //    AddOutEdges(OldChargeBox._ChargeBox2ChargeBox_OutEdges);

                //    foreach (var edge in _ChargeBox2ChargeBox_OutEdges)
                //        edge.Source = this;

                //}

                //if (OldChargeBox._Notifications.SafeAny() && !_Notifications.SafeAny())
                //    _Notifications.Add(OldChargeBox._Notifications);

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

            #region CompareTo(ChargeBox)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="ChargeBox">An chargeBox object to compare with.</param>
            public override Int32 CompareTo(ChargeBox? ChargeBox)

                => ChargeBox is null
                       ? throw new ArgumentNullException(nameof(ChargeBox), "The given chargeBox must not be null!")
                       : Id.CompareTo(ChargeBox.Id);


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

            #region Equals(ChargeBox)

            /// <summary>
            /// Compares two chargeBoxs for equality.
            /// </summary>
            /// <param name="ChargeBox">An chargeBox to compare with.</param>
            /// <returns>True if both match; False otherwise.</returns>
            public override Boolean Equals(ChargeBox? ChargeBox)

                => ChargeBox is not null &&
                       Id.Equals(ChargeBox.Id);


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
            /// Get the hashcode of this object.
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
