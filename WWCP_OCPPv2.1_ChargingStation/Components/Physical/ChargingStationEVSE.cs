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

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    public delegate Task OnWebPaymentURLChanged (DateTime            Timestamp,
                                                 EVSE_Id             EVSEId,
                                                 String              QRCodeURL,
                                                 TimeSpan            RemainingTime,
                                                 DateTimeOffset      EndTime,
                                                 CancellationToken   CancellationToken);


    #region (class) EVSESpec(...)

    public class EVSESpec(OperationalStatus            AdminStatus,
                          IEnumerable<ConnectorType>?  ConnectorTypes       = null,
                          String?                      MeterType            = null,
                          String?                      MeterSerialNumber    = null,
                          String?                      MeterPublicKey       = null)

        : EVSESpec0(AdminStatus,
                    MeterType,
                    MeterSerialNumber,
                    MeterPublicKey)

    {

        public IEnumerable<ConnectorType>  ConnectorTypes    { get; set; } = ConnectorTypes ?? [];


    }

    #endregion

    #region EVSESpec0(...)

    public class EVSESpec0(OperationalStatus  AdminStatus,
                           String?            MeterType           = null,
                           String?            MeterSerialNumber   = null,
                           String?            MeterPublicKey      = null)
    {

        public OperationalStatus  AdminStatus              { get; set; } = AdminStatus;
        public String?            MeterType                { get; set; } = MeterType;
        public String?            MeterSerialNumber        { get; set; } = MeterSerialNumber;
        public String?            MeterPublicKey           { get; set; } = MeterPublicKey;

    }

    #endregion


    #region ChargingStationEVSE

    /// <summary>
    /// An EVSE at a charging station.
    /// </summary>
    public class ChargingStationEVSE : EVSESpec0
    {

        #region Data

        private   readonly AChargingStationNode                                          parentChargingStation;

        protected readonly ConcurrentDictionary<Connector_Id, ChargingStationConnector>  connectors         = [];
        public             ConcurrentDictionary<String, List<ComponentConfig>>           ComponentConfigs   = [];

        #endregion

        #region Properties

        public EVSE_Id            Id                       { get; }

        public Reservation_Id?    ReservationId            { get; set; }

        public ConnectorStatus    Status                   { get; set; }

        public Boolean            IsReserved               { get; set; }

        public Boolean            IsCharging               { get; set; }

        public IdToken?           IdToken                  { get; set; }

        public IdToken?           GroupIdToken             { get; set; }

        public Transaction_Id?    TransactionId            { get; set; }

        public RemoteStart_Id?    RemoteStartId            { get; set; }

        public ChargingProfile?   ChargingProfile          { get; set; }


        public DateTime?          StartTimestamp           { get; set; }

        public Decimal?           MeterStartValue          { get; set; }

        public String?            SignedStartMeterValue    { get; set; }

        public DateTime?          StopTimestamp            { get; set; }

        public Decimal?           MeterStopValue           { get; set; }

        public String?            SignedStopMeterValue     { get; set; }

        public Tariff?            DefaultChargingTariff    { get; set; }


        public String?            WebPaymentsURL           { get; private set; }
        public DateTimeOffset?    WebPaymentsEndTime       { get; private set; }
        public TimeSpan?          WebPaymentsRemaningTime
            => WebPaymentsEndTime - DateTimeOffset.UtcNow;

        #endregion

        #region Controllers

        #region Generic

        public IEnumerable<T> GetComponentConfigs<T>(String         Name,
                                                     Connector_Id?  ConnectorId   = null,
                                                     String?        Instance      = null)

            where T : ComponentConfig

            => ComponentConfigs.TryGetValue(Name, out var controllerList)
                   ? controllerList.Where  (controller => controller.Name     == Name &&
                                                          controller.EVSE?.Id == Id   &&
                                                          (!ConnectorId.HasValue        || controller.EVSE?.ConnectorId == ConnectorId) &&
                                                          ( Instance.   IsNullOrEmpty() || controller.Instance          == Instance)).
                                    Cast<T>()
                   : [];

        #endregion


        /// <summary>
        /// The Web Payments Controller
        /// </summary>
        public WebPaymentsCtrlr? WebPaymentsController
            => GetComponentConfigs<WebPaymentsCtrlr>(nameof(WebPaymentsCtrlr)).FirstOrDefault();

        #endregion

        #region Event

        public event OnWebPaymentURLChanged? OnWebPaymentURLChanged;

        #endregion

        #region ChargingStationEVSE(Id, AdminStatus, ...)

        internal ChargingStationEVSE(AChargingStationNode         ChargingStation,
                                     EVSE_Id                      Id,
                                     OperationalStatus            AdminStatus,
                                     IEnumerable<ConnectorType>?  ConnectorTypes      = null,
                                     String?                      MeterType           = null,
                                     String?                      MeterSerialNumber   = null,
                                     String?                      MeterPublicKey      = null)

            : base(AdminStatus)

        {

            this.parentChargingStation  = ChargingStation;
            this.Id                     = Id;
            this.AdminStatus            = AdminStatus;
            this.MeterType              = MeterType;
            this.MeterSerialNumber      = MeterSerialNumber;
            this.MeterPublicKey         = MeterPublicKey;

            foreach (var connectorType in ConnectorTypes ?? [])
                AddConnector(connectorType);

            AddComponent(new WebPaymentsCtrlr(
                             EVSE:             new EVSE(Id),
                             Enabled:          null,
                             URLTemplate:      null,
                             ValidityTime:     null,
                             HashAlgorithm:    null,
                             SharedSecret:     null,
                             Length:           null,
                             Encoding:         null,
                             QRCodeQuality:    null,
                             Signature:        null,

                             Instance:         null,
                             CustomData:       null
                         ));

        }

        #endregion


        #region Connectors

        public IEnumerable<ChargingStationConnector> Connectors
            => connectors.Values;

        public Boolean TryGetConnector(Connector_Id ConnectorId, [NotNullWhen(true)] out ChargingStationConnector? Connector)
            => connectors.TryGetValue(ConnectorId, out Connector);


        public ChargingStationConnector? AddConnector(ConnectorType ConnectorType)
        {

            lock (connectors)
            {

                var connector = new ChargingStationConnector(
                                    this,
                                    Connector_Id.Parse((Byte) (connectors.Count + 1)),
                                    ConnectorType
                                );

                return connectors.TryAdd(connector.Id, connector)
                           ? connector
                           : null;

            }

        }

        #endregion


        public void AddComponent(ComponentConfig Component)
        {

            ComponentConfigs.AddOrUpdate(
                                 Component.Name,
                                 name => [Component],
                                 (name, list) => list.AddAndReturnList(Component)
                             );

        }




        #region (Timer) DoMaintenance(State)

        public async Task DoMaintenanceAsync(AChargingStationNode  ChargingStation,
                                             Object?               State)
        {

            await Task.Delay(1);

            var webPaymentsController = WebPaymentsController;

            if (webPaymentsController is not null &&
                webPaymentsController.Enabled == true &&
                webPaymentsController.URLTemplate.HasValue &&
                webPaymentsController.SharedSecret.IsNotNullOrEmpty() &&
                (!WebPaymentsEndTime.HasValue || Timestamp.Now > WebPaymentsEndTime.Value == true))
            {

                var (url,
                     remainingTime,
                     endTime) = TOTPGenerator.GenerateURL(
                                    webPaymentsController.URLTemplate.Value,
                                    webPaymentsController.SharedSecret,
                                    webPaymentsController.ValidityTime,
                                    webPaymentsController.Length
                                );

                WebPaymentsURL     = url;
                WebPaymentsEndTime = endTime;

                await LogEvent(
                          OnWebPaymentURLChanged,
                          logger => logger.Invoke(
                                        Timestamp.Now,
                                        Id,
                                        WebPaymentsURL,
                                        remainingTime,
                                        endTime,
                                        CancellationToken.None
                                    )
                      );

            }

        }

        #endregion







        public JObject ToJSON()
        {

            var json = JSONObject.Create(

                                 new JProperty("id",            Id.                      ToString()),

                           WebPaymentsController?.Enabled == true
                               ? new JProperty("webPayments",   JSONObject.Create(

                                                                    new JProperty("enabled",        WebPaymentsController.Enabled),
                                                                    // URLTemplate
                                                                    new JProperty("validityTime",   WebPaymentsController.ValidityTime.TotalSeconds),
                                                                    // HashAlgorithm
                                                                    // SharedSecret
                                                                    // Length
                                                                    // Encoding
                                                                    // QRCodeQuality
                                                                    // Signature
                                                                    new JProperty("totp",           WebPaymentsController.ValidityTime)

                                                                ))
                               : null

                       );

            return json;

        }


        #region (private) LogEvent     (Logger, LogHandler,    ...)

        private Task LogEvent<TDelegate>(TDelegate?                                         Logger,
                                         Func<TDelegate, Task>                              LogHandler,
                                         [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                         [CallerMemberName()]                       String  OCPPCommand   = "")

            where TDelegate : Delegate

            => parentChargingStation.LogEvent(
                   nameof(ChargingStationEVSE),
                   Logger,
                   LogHandler,
                   EventName,
                   OCPPCommand
               );

        #endregion

        #region (private) HandleErrors (Caller, ExceptionOccurred)

        private Task HandleErrors(String     Caller,
                                  Exception  ExceptionOccurred)

            => parentChargingStation.HandleErrors(
                   nameof(ChargingStationEVSE),
                   Caller,
                   ExceptionOccurred
               );

        #endregion


    }

    #endregion

}
