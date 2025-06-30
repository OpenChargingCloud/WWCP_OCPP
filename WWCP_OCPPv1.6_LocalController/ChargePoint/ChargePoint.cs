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

using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.NetworkingNode
{

    public delegate Boolean ChargePointProviderDelegate(ChargePoint_Id ChargePointId, out ChargePoint ChargePoint);

    public delegate JObject ChargePointToJSONDelegate(ChargePoint  ChargePoint,
                                                      Boolean      Embedded            = false,
                                                      InfoStatus   ExpandTags          = InfoStatus.ShowIdOnly,
                                                      Boolean      IncludeCryptoHash   = true);

    /// <summary>
    /// Extension methods for chargeBoxs.
    /// </summary>
    public static class ChargePointExtensions
    {

        #region ToJSON(this ChargePoints, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of chargeBoxs.
        /// </summary>
        /// <param name="ChargePoints">An enumeration of chargeBoxs.</param>
        /// <param name="Skip">The optional number of chargeBoxs to skip.</param>
        /// <param name="Take">The optional number of chargeBoxs to return.</param>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        public static JArray ToJSON(this IEnumerable<ChargePoint>  ChargePoints,
                                    UInt64?                      Skip                  = null,
                                    UInt64?                      Take                  = null,
                                    Boolean                      Embedded              = false,
                                    InfoStatus                   ExpandTags            = InfoStatus.ShowIdOnly,
                                    ChargePointToJSONDelegate      ChargePointToJSON       = null,
                                    Boolean                      IncludeCryptoHash     = true)


            => ChargePoints?.Any() != true

                   ? new JArray()

                   : new JArray(ChargePoints.
                                    Where     (chargeBox => chargeBox != null).
                                    OrderBy   (chargeBox => chargeBox.Id).
                                    SkipTakeFilter(Skip, Take).
                                    SafeSelect(chargeBox => ChargePointToJSON != null
                                                                    ? ChargePointToJSON (chargeBox,
                                                                                       Embedded,
                                                                                       ExpandTags,
                                                                                       IncludeCryptoHash)

                                                                    : chargeBox.ToJSON(Embedded,
                                                                                       ExpandTags,
                                                                                       IncludeCryptoHash)));

        #endregion

    }



    /// <summary>
    /// A charge point.
    /// </summary>
    public class ChargePoint : AEntity<ChargePoint_Id,
                                       ChargePoint>
    {

        /// <summary>
        /// A charge point connector.
        /// </summary>
        public class ChargePointConnector
        {

            public Connector_Id       Id                       { get; }

            public Availabilities     Availability             { get; set; }


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


            public ChargePointConnector(Connector_Id    Id,
                                        Availabilities  Availability)
            {

                this.Id            = Id;
                this.Availability  = Availability;

            }


        }


        #region Data

        /// <summary>
        /// The default max size of the charge point status history.
        /// </summary>
        public const UInt16 DefaultChargePointStatusHistorySize = 50;

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
        /// The optional serial number of the charge point.
        /// </summary>
        public String?                  ChargePointSerialNumber     { get; }

        /// <summary>
        /// The optional serial number of the charge point.
        /// </summary>
        //public String?                  ChargePointSerialNumber       { get; }

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

        private readonly Dictionary<Connector_Id, ChargePointConnector> connectors;

        public IEnumerable<ChargePointConnector> Connectors
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
        public event OnDataTransferRequestSentDelegate?   OnDataTransferRequest;

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
        /// <param name="Id">The charge point identification.</param>
        /// <param name="NumberOfConnectors">Number of available connectors.</param>
        /// <param name="ChargePointVendor">The charge point vendor identification.</param>
        /// <param name="ChargePointModel">The charge point model identification.</param>
        /// 
        /// <param name="Description">An optional multi-language charge point description.</param>
        /// <param name="ChargePointSerialNumber">An optional serial number of the charge point.</param>
        /// <param name="ChargePointSerialNumber">An optional serial number of the charge point.</param>
        /// <param name="FirmwareVersion">An optional firmware version of the charge point.</param>
        /// <param name="Iccid">An optional ICCID of the charge point's SIM card.</param>
        /// <param name="IMSI">An optional IMSI of the charge point’s SIM card.</param>
        /// <param name="MeterType">An optional meter type of the main power meter of the charge point.</param>
        /// <param name="MeterSerialNumber">An optional serial number of the main power meter of the charge point.</param>
        /// <param name="MeterPublicKey">An optional public key of the main power meter of the charge point.</param>
        /// 
        /// <param name="ExpectHeartbeatEvery">The time span between expected heartbeat.</param>
        public ChargePoint(ChargePoint_Id                                Id,
                         Byte                                        NumberOfConnectors,
                         String                                      ChargePointVendor,
                         String                                      ChargePointModel,

                         I18NString?                                 Description                         = null,
                         String?                                     ChargePointSerialNumber             = null,
                         //String?                                     ChargePointSerialNumber               = null,
                         String?                                     FirmwareVersion                     = null,
                         String?                                     Iccid                               = null,
                         String?                                     IMSI                                = null,
                         String?                                     MeterType                           = null,
                         String?                                     MeterSerialNumber                   = null,
                         String?                                     MeterPublicKey                      = null,

                         TimeSpan?                                   ExpectHeartbeatEvery                = null,

                         IEnumerable<ANotification>?                 Notifications                       = null,

                         //IEnumerable<Organization2ChargePointEdge>?    Organization2ChargePointEdges         = null,
                         IEnumerable<ChargePoint2ChargePointEdge>?       ChargePoint2ChargePointInEdges          = null,
                         IEnumerable<ChargePoint2ChargePointEdge>?       ChargePoint2ChargePointOutEdges         = null,

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


            this.connectors               = new Dictionary<Connector_Id, ChargePointConnector>();
            //for (var i = 1; i <= NumberOfConnectors; i++)
            //{
            //    this.connectors.Add(Connector_Id.Parse(i.ToString()),
            //                        new ChargePointConnector(Connector_Id.Parse(i.ToString()),
            //                                                 Availabilities.Inoperative));
            //}

            //this.Configuration            = new Dictionary<String, ConfigurationData>();

            this.ChargePointVendor        = ChargePointVendor;
            this.ChargePointModel         = ChargePointModel;

            this.ChargePointSerialNumber  = ChargePointSerialNumber;
            //this.ChargePointSerialNumber    = ChargePointSerialNumber;
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


        #region ToJSON(CustomChargePointSerializer = null)

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
        /// <param name="CustomChargePointSerializer">A delegate to serialize custom charge pointes.</param>
        public JObject ToJSON(Boolean                                      Embedded                    = false,
                              InfoStatus                                   ExpandTags                  = InfoStatus.ShowIdOnly,
                              Boolean                                      IncludeLastChange           = true,
                              CustomJObjectSerializerDelegate<ChargePoint>?  CustomChargePointSerializer   = null)
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

                                       //ChargePointSerialNumber.  IsNotNullOrEmpty()
                                       //    ? new JProperty("chargeBoxSerialNumber",     ChargePointSerialNumber)
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

            return CustomChargePointSerializer is not null
                       ? CustomChargePointSerializer(this, json)
                       : json;

        }

        #endregion


        #region CopyAllLinkedDataFromBase(OldChargePoint)

        public override void CopyAllLinkedDataFromBase(ChargePoint OldChargePoint)
        {

            //if (OldChargePoint._User2ChargePoint_Edges.Any() && !_User2ChargePoint_Edges.Any())
            //{

            //    AddUsers(OldChargePoint._User2ChargePoint_Edges);

            //    foreach (var edge in _User2ChargePoint_Edges)
            //        edge.Target = this;

            //}

            //if (OldChargePoint._ChargePoint2ChargePoint_InEdges.Any() && !_ChargePoint2ChargePoint_InEdges.Any())
            //{

            //    AddEdges(OldChargePoint._ChargePoint2ChargePoint_InEdges);

            //    foreach (var edge in _ChargePoint2ChargePoint_InEdges)
            //        edge.Target = this;

            //}

            //if (OldChargePoint._ChargePoint2ChargePoint_OutEdges.Any() && !_ChargePoint2ChargePoint_OutEdges.Any())
            //{

            //    AddEdges(OldChargePoint._ChargePoint2ChargePoint_OutEdges);

            //    foreach (var edge in _ChargePoint2ChargePoint_OutEdges)
            //        edge.Source = this;

            //}

            //if (OldChargePoint._Notifications.SafeAny() && !_Notifications.SafeAny())
            //    _Notifications.Add(OldChargePoint._Notifications);

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargePointId1, ChargePointId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargePointId1">A chargeBox identification.</param>
        /// <param name="ChargePointId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargePoint ChargePointId1, ChargePoint ChargePointId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ChargePointId1, ChargePointId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ChargePointId1 == null) || ((Object) ChargePointId2 == null))
                return false;

            return ChargePointId1.Equals(ChargePointId2);

        }

        #endregion

        #region Operator != (ChargePointId1, ChargePointId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargePointId1">A chargeBox identification.</param>
        /// <param name="ChargePointId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargePoint ChargePointId1, ChargePoint ChargePointId2)
            => !(ChargePointId1 == ChargePointId2);

        #endregion

        #region Operator <  (ChargePointId1, ChargePointId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargePointId1">A chargeBox identification.</param>
        /// <param name="ChargePointId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargePoint ChargePointId1, ChargePoint ChargePointId2)
        {

            if ((Object) ChargePointId1 == null)
                throw new ArgumentNullException(nameof(ChargePointId1), "The given ChargePointId1 must not be null!");

            return ChargePointId1.CompareTo(ChargePointId2) < 0;

        }

        #endregion

        #region Operator <= (ChargePointId1, ChargePointId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargePointId1">A chargeBox identification.</param>
        /// <param name="ChargePointId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargePoint ChargePointId1, ChargePoint ChargePointId2)
            => !(ChargePointId1 > ChargePointId2);

        #endregion

        #region Operator >  (ChargePointId1, ChargePointId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargePointId1">A chargeBox identification.</param>
        /// <param name="ChargePointId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargePoint ChargePointId1, ChargePoint ChargePointId2)
        {

            if ((Object) ChargePointId1 == null)
                throw new ArgumentNullException(nameof(ChargePointId1), "The given ChargePointId1 must not be null!");

            return ChargePointId1.CompareTo(ChargePointId2) > 0;

        }

        #endregion

        #region Operator >= (ChargePointId1, ChargePointId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargePointId1">A chargeBox identification.</param>
        /// <param name="ChargePointId2">Another chargeBox identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargePoint ChargePointId1, ChargePoint ChargePointId2)
            => !(ChargePointId1 < ChargePointId2);

        #endregion

        #endregion

        #region IComparable<ChargePoint> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Int32 CompareTo(Object Object)

            => Object is ChargePoint ChargePoint
                   ? CompareTo(ChargePoint)
                   : throw new ArgumentException("The given object is not an chargeBox!", nameof(Object));

        #endregion

        #region CompareTo(ChargePoint)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargePoint">An chargeBox object to compare with.</param>
        public override Int32 CompareTo(ChargePoint ChargePoint)

            => ChargePoint is null
                   ? throw new ArgumentNullException(nameof(ChargePoint), "The given chargeBox must not be null!")
                   : Id.CompareTo(ChargePoint.Id);

        #endregion

        #endregion

        #region IEquatable<ChargePoint> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ChargePoint ChargePoint &&
                  Equals(ChargePoint);

        #endregion

        #region Equals(ChargePoint)

        /// <summary>
        /// Compares two chargeBoxs for equality.
        /// </summary>
        /// <param name="ChargePoint">An chargeBox to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ChargePoint ChargePoint)

            => ChargePoint is ChargePoint &&
                   Id.Equals(ChargePoint.Id);

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


        #region ToBuilder(NewChargePointId = null)

        /// <summary>
        /// Return a builder for this chargeBox.
        /// </summary>
        /// <param name="NewChargePointId">An optional new chargeBox identification.</param>
        public Builder ToBuilder(ChargePoint_Id? NewChargePointId = null)

            => new (NewChargePointId ?? Id,
                    1, //NumberOfConnectors,
                    ChargePointVendor,
                    ChargePointModel,

                    Description,
                    ChargePointSerialNumber,
                    //ChargePointSerialNumber,
                    FirmwareVersion,
                    Iccid,
                    IMSI,
                    MeterType,
                    MeterSerialNumber,
                    MeterPublicKey,

                    ExpectHeartbeatEvery,

                    null, //_Notifications,

                    //_User2ChargePoint_Edges,
                    null, //_ChargePoint2ChargePoint_InEdges,
                    null, //_ChargePoint2ChargePoint_OutEdges,

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
        public new class Builder : AEntity<ChargePoint_Id, ChargePoint>.Builder
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
            /// The optional multi-language charge point description.
            /// </summary>
            public I18NString               Description                 { get; set; }

            /// <summary>
            /// The optional serial number of the charge point.
            /// </summary>
            public String                   ChargePointSerialNumber     { get; set; }

            /// <summary>
            /// The optional serial number of the charge point.
            /// </summary>
           // public String                   ChargePointSerialNumber       { get; set; }

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

            #region User          -> ChargePoint edges

            //protected readonly List<User2ChargePointEdge> _User2ChargePoint_Edges;

            //public IEnumerable<User2ChargePointEdge> User2ChargePointEdges
            //    => _User2ChargePoint_Edges;


            //#region LinkUser(Edge)

            //public User2ChargePointEdge

            //    LinkUser(User2ChargePointEdge Edge)

            //    => _User2ChargePoint_Edges.AddAndReturnElement(Edge);

            //#endregion

            //#region LinkUser(Source, EdgeLabel, PrivacyLevel = PrivacyLevel.World)

            //public User2ChargePointEdge

            //    LinkUser(User                    Source,
            //             User2ChargePointEdgeLabel  EdgeLabel,
            //             PrivacyLevel            PrivacyLevel = PrivacyLevel.World)

            //    => _User2ChargePoint_Edges.
            //           AddAndReturnElement(new User2ChargePointEdge(Source,
            //                                                                                        EdgeLabel,
            //                                                                                        this,
            //                                                                                        PrivacyLevel));

            //#endregion


            //#region User2ChargePointInEdges     (User)

            ///// <summary>
            ///// The edge labels of all (incoming) edges between the given user and this chargeBox.
            ///// </summary>
            //public IEnumerable<User2ChargePointEdge> User2ChargePointInEdges(User User)

            //    => _User2ChargePoint_Edges.
            //           Where(edge => edge.Source == User);

            //#endregion

            //#region User2ChargePointInEdgeLabels(User)

            ///// <summary>
            ///// The edge labels of all (incoming) edges between the given user and this chargeBox.
            ///// </summary>
            //public IEnumerable<User2ChargePointEdgeLabel> User2ChargePointInEdgeLabels(User User)

            //    => _User2ChargePoint_Edges.
            //           Where (edge => edge.Source == User).
            //           Select(edge => edge.EdgeLabel);

            //#endregion

            //public IEnumerable<User2ChargePointEdge>

            //    Add(IEnumerable<User2ChargePointEdge> Edges)

            //        => _User2ChargePoint_Edges.AddAndReturnList(Edges);


            //#region UnlinkUser(EdgeLabel, User)

            //public void UnlinkUser(User2ChargePointEdgeLabel  EdgeLabel,
            //                       User                    User)
            //{

            //    var edges = _User2ChargePoint_Edges.
            //                    Where(edge => edge.EdgeLabel == EdgeLabel &&
            //                                  edge.Source    == User).
            //                    ToArray();

            //    foreach (var edge in edges)
            //        _User2ChargePoint_Edges.Remove(edge);

            //}

            //#endregion

            //public Boolean RemoveInEdge(User2ChargePointEdge Edge)
            //    => _User2ChargePoint_Edges.Remove(Edge);

            #endregion

            #region ChargePoint <-> ChargePoint edges

            protected readonly List<ChargePoint2ChargePointEdge> _ChargePoint2ChargePoint_InEdges;

            public IEnumerable<ChargePoint2ChargePointEdge> ChargePoint2ChargePointInEdges
                => _ChargePoint2ChargePoint_InEdges;

            #region AddInEdge (Edge)

            public ChargePoint2ChargePointEdge

                AddInEdge(ChargePoint2ChargePointEdge Edge)

                => _ChargePoint2ChargePoint_InEdges.AddAndReturnElement(Edge);

            #endregion

            #region AddInEdge (EdgeLabel, SourceChargePoint, PrivacyLevel = PrivacyLevel.World)

            public ChargePoint2ChargePointEdge

                AddInEdge (ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                           ChargePoint                    SourceChargePoint,
                           PrivacyLevel                    PrivacyLevel = PrivacyLevel.World)

                => _ChargePoint2ChargePoint_InEdges. AddAndReturnElement(new ChargePoint2ChargePointEdge(SourceChargePoint,
                                                                                                                                                    EdgeLabel,
                                                                                                                                                    this,
                                                                                                                                                    PrivacyLevel));

            #endregion

            public IEnumerable<ChargePoint2ChargePointEdge>

                AddInEdges(IEnumerable<ChargePoint2ChargePointEdge> Edges)

                    => _ChargePoint2ChargePoint_InEdges.AddAndReturnList(Edges);

            #region RemoveInEdges(EdgeLabel, TargetChargePoint)

            public Boolean RemoveInEdge(ChargePoint2ChargePointEdge Edge)
                => _ChargePoint2ChargePoint_InEdges.Remove(Edge);

            #endregion

            #region RemoveInEdges (EdgeLabel, SourceChargePoint)

            public void RemoveInEdges(ChargePoint2ChargePointEdgeLabel EdgeLabel,
                                      ChargePoint SourceChargePoint)
            {

                var edges = _ChargePoint2ChargePoint_OutEdges.
                                Where(edge => edge.EdgeLabel == EdgeLabel &&
                                              edge.Source == SourceChargePoint).
                                ToArray();

                foreach (var edge in edges)
                    _ChargePoint2ChargePoint_InEdges.Remove(edge);

            }

            #endregion



            protected readonly List<ChargePoint2ChargePointEdge> _ChargePoint2ChargePoint_OutEdges;

            public IEnumerable<ChargePoint2ChargePointEdge> ChargePoint2ChargePointOutEdges
                => _ChargePoint2ChargePoint_OutEdges;

            #region AddOutEdge(Edge)

            public ChargePoint2ChargePointEdge

                AddOutEdge(ChargePoint2ChargePointEdge Edge)

                => _ChargePoint2ChargePoint_OutEdges.AddAndReturnElement(Edge);

            #endregion

            #region AddOutEdge(EdgeLabel, TargetChargePoint, PrivacyLevel = PrivacyLevel.World)

            public ChargePoint2ChargePointEdge

                AddOutEdge(ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                           ChargePoint                    TargetChargePoint,
                           PrivacyLevel                    PrivacyLevel = PrivacyLevel.World)

                => _ChargePoint2ChargePoint_OutEdges.AddAndReturnElement(new ChargePoint2ChargePointEdge(this,
                                                                                                                                                    EdgeLabel,
                                                                                                                                                    TargetChargePoint,
                                                                                                                                                    PrivacyLevel));

            #endregion

            public IEnumerable<ChargePoint2ChargePointEdge>

                AddOutEdges(IEnumerable<ChargePoint2ChargePointEdge> Edges)

                    => _ChargePoint2ChargePoint_OutEdges.AddAndReturnList(Edges);

            #region RemoveOutEdges(EdgeLabel, TargetChargePoint)

            public Boolean RemoveOutEdge(ChargePoint2ChargePointEdge Edge)
                => _ChargePoint2ChargePoint_OutEdges.Remove(Edge);

            #endregion

            #region RemoveOutEdges(EdgeLabel, TargetChargePoint)

            public void RemoveOutEdges(ChargePoint2ChargePointEdgeLabel  EdgeLabel,
                                       ChargePoint                    TargetChargePoint)
            {

                var edges = _ChargePoint2ChargePoint_OutEdges.
                                Where(edge => edge.EdgeLabel == EdgeLabel &&
                                              edge.Target    == TargetChargePoint).
                                ToArray();

                foreach (var edge in edges)
                    _ChargePoint2ChargePoint_OutEdges.Remove(edge);

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
            public Builder(ChargePoint_Id                             Id,
                           Byte                                       NumberOfConnectors,
                           String                                     ChargePointVendor,
                           String                                     ChargePointModel,

                           I18NString?                                Description                       = null,
                           String?                                    ChargePointSerialNumber           = null,
                           //String                                     ChargePointSerialNumber           = null,
                           String?                                    FirmwareVersion                   = null,
                           String?                                    Iccid                             = null,
                           String?                                    IMSI                              = null,
                           String?                                    MeterType                         = null,
                           String?                                    MeterSerialNumber                 = null,
                           String?                                    MeterPublicKey                    = null,

                           TimeSpan?                                  ExpectHeartbeatEvery              = null,

                           IEnumerable<ANotification>?                Notifications                     = null,

                           //IEnumerable<User2ChargePointEdge>?         User2ChargePointEdges             = null,
                           IEnumerable<ChargePoint2ChargePointEdge>?  ChargePoint2ChargePointInEdges    = null,
                           IEnumerable<ChargePoint2ChargePointEdge>?  ChargePoint2ChargePointOutEdges   = null,

                           JObject?                                   CustomData                        = null,
                           IEnumerable<AttachedFile>?                 AttachedFiles                     = null,
                           JSONLDContext?                             JSONLDContext                     = null,
                           String?                                    DataSource                        = null,
                           DateTime?                                  Created                           = null,
                           DateTime?                                  LastChange                        = null)

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

               // this._User2ChargePoint_Edges             = User2ChargePointEdges.           IsNeitherNullNorEmpty() ? new List<User2ChargePointEdge>        (User2ChargePointEdges)            : new List<User2ChargePointEdge>();
                this._ChargePoint2ChargePoint_InEdges   = ChargePoint2ChargePointInEdges. IsNeitherNullNorEmpty() ? new List<ChargePoint2ChargePointEdge>(ChargePoint2ChargePointInEdges)  : new List<ChargePoint2ChargePointEdge>();
                this._ChargePoint2ChargePoint_OutEdges  = ChargePoint2ChargePointOutEdges.IsNeitherNullNorEmpty() ? new List<ChargePoint2ChargePointEdge>(ChargePoint2ChargePointOutEdges) : new List<ChargePoint2ChargePointEdge>();

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
            public static implicit operator ChargePoint(Builder Builder)

                => Builder?.ToImmutable;


            /// <summary>
            /// Return an immutable version of the chargeBox.
            /// </summary>
            public ChargePoint ToImmutable
            {
                get
                {

                    //if (!Branch.HasValue || Branch.Value.IsNullOrEmpty)
                    //    throw new ArgumentNullException(nameof(Branch), "The given branch must not be null or empty!");

                    return new ChargePoint(Id,
                                         1,
                                         ChargePointVendor,
                                         ChargePointModel,

                                         Description,
                                         ChargePointSerialNumber,
                                         //ChargePointSerialNumber,
                                         FirmwareVersion,
                                         Iccid,
                                         IMSI,
                                         MeterType,
                                         MeterSerialNumber,
                                         MeterPublicKey,

                                         ExpectHeartbeatEvery,

                                                          _Notifications,

                                                          //_User2ChargePoint_Edges,
                                                          _ChargePoint2ChargePoint_InEdges,
                                                          _ChargePoint2ChargePoint_OutEdges,

                                                          CustomData,
                                                          AttachedFiles,
                                                          JSONLDContext,
                                                          DataSource,
                                                          LastChangeDate);

                }
            }

            #endregion


            #region CopyAllLinkedDataFromBase(OldChargePoint)

            public override void CopyAllLinkedDataFromBase(ChargePoint OldChargePoint)
            {

                //if (OldChargePoint._User2ChargePoint_Edges.Any() && !_User2ChargePoint_Edges.Any())
                //{

                //    Add(OldChargePoint._User2ChargePoint_Edges);

                //    foreach (var edge in _User2ChargePoint_Edges)
                //        edge.Target = this;

                //}

                //if (OldChargePoint._ChargePoint2ChargePoint_InEdges.Any() && !_ChargePoint2ChargePoint_InEdges.Any())
                //{

                //    AddInEdges(OldChargePoint._ChargePoint2ChargePoint_InEdges);

                //    foreach (var edge in _ChargePoint2ChargePoint_InEdges)
                //        edge.Target = this;

                //}

                //if (OldChargePoint._ChargePoint2ChargePoint_OutEdges.Any() && !_ChargePoint2ChargePoint_OutEdges.Any())
                //{

                //    AddOutEdges(OldChargePoint._ChargePoint2ChargePoint_OutEdges);

                //    foreach (var edge in _ChargePoint2ChargePoint_OutEdges)
                //        edge.Source = this;

                //}

                //if (OldChargePoint._Notifications.SafeAny() && !_Notifications.SafeAny())
                //    _Notifications.Add(OldChargePoint._Notifications);

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

            #region CompareTo(ChargePoint)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="ChargePoint">An chargeBox object to compare with.</param>
            public override Int32 CompareTo(ChargePoint? ChargePoint)

                => ChargePoint is null
                       ? throw new ArgumentNullException(nameof(ChargePoint), "The given chargeBox must not be null!")
                       : Id.CompareTo(ChargePoint.Id);


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

            #region Equals(ChargePoint)

            /// <summary>
            /// Compares two chargeBoxs for equality.
            /// </summary>
            /// <param name="ChargePoint">An chargeBox to compare with.</param>
            /// <returns>True if both match; False otherwise.</returns>
            public override Boolean Equals(ChargePoint? ChargePoint)

                => ChargePoint is not null &&
                       Id.Equals(ChargePoint.Id);


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
