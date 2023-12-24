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

using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages;
using cloud.charging.open.protocols.OCPP.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS
{

    /// <summary>
    /// The networking node HTTP WebSocket client runs on a networking node
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class NetworkingNodeWSClient : AOCPPWebSocketClient,
                                                  INetworkingNodeWebSocketClient,
                                                  INetworkingNodeIncomingMessages,
                                                  INetworkingNodeOutgoingMessagesEvents
    {

        #region Data

        /// <summary>
        /// The default HTTP user agent string.
        /// </summary>
        public new const String  DefaultHTTPUserAgent   = $"GraphDefined OCPP {Version.String} NN WebSocket Client";

        private const    String  LogfileName            = "NetworkingNodeWSClient.log";

        #endregion

        #region Properties

        #endregion

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<StatusInfo>?                                CustomStatusInfoSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<ClearMonitoringResult>?                     CustomClearMonitoringResultSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<ChargingStation>?                           CustomChargingStationSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<OCPP.Signature>?                            CustomSignatureSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<CustomData>?                                CustomCustomDataSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<EventData>?                                 CustomEventDataSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<Component>?                                 CustomComponentSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<SetVariableResult>?                         CustomSetVariableResultSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<GetVariableResult>?                         CustomGetVariableResultSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringResult>?                       CustomSetMonitoringResultSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                                      CustomEVSESerializer                                        { get; set; }
        public CustomJObjectSerializerDelegate<Variable>?                                  CustomVariableSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<VariableMonitoring>?                        CustomVariableMonitoringSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<ReportData>?                                CustomReportDataSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<VariableAttribute>?                         CustomVariableAttributeSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<VariableCharacteristics>?                   CustomVariableCharacteristicsSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<MonitoringData>?                            CustomMonitoringDataSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<OCSPRequestData>?                           CustomOCSPRequestDataSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<IdToken>?                                   CustomIdTokenSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalInfo>?                            CustomAdditionalInfoSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<ChargingNeeds>?                             CustomChargingNeedsSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ACChargingParameters>?                      CustomACChargingParametersSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<DCChargingParameters>?                      CustomDCChargingParametersSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<V2XChargingParameters>?                     CustomV2XChargingParametersSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<EVEnergyOffer>?                             CustomEVEnergyOfferSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<EVPowerSchedule>?                           CustomEVPowerScheduleSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<EVPowerScheduleEntry>?                      CustomEVPowerScheduleEntrySerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<EVAbsolutePriceSchedule>?                   CustomEVAbsolutePriceScheduleSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?              CustomEVAbsolutePriceScheduleEntrySerializer                { get; set; }
        public CustomJObjectSerializerDelegate<EVPriceRule>?                               CustomEVPriceRuleSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<Transaction>?                               CustomTransactionSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<MeterValue>?                                CustomMeterValueSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<SampledValue>?                              CustomSampledValueSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<SignedMeterValue>?                          CustomSignedMeterValueSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<UnitsOfMeasure>?                            CustomUnitsOfMeasureSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?                          CustomChargingScheduleSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<LimitBeyondSoC>?                            CustomLimitBeyondSoCSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<CompositeSchedule>?                         CustomCompositeScheduleSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                    CustomChargingSchedulePeriodSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                          CustomV2XFreqWattEntrySerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                        CustomV2XSignalWattEntrySerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariff>?                               CustomSalesTariffSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariffEntry>?                          CustomSalesTariffEntrySerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<RelativeTimeInterval>?                      CustomRelativeTimeIntervalSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ConsumptionCost>?                           CustomConsumptionCostSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<Cost>?                                      CustomCostSerializer                                        { get; set; }

        public CustomJObjectSerializerDelegate<AbsolutePriceSchedule>?                     CustomAbsolutePriceScheduleSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<PriceRuleStack>?                            CustomPriceRuleStackSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<PriceRule>?                                 CustomPriceRuleSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<TaxRule>?                                   CustomTaxRuleSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<OverstayRuleList>?                          CustomOverstayRuleListSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<OverstayRule>?                              CustomOverstayRuleSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalService>?                         CustomAdditionalServiceSerializer                           { get; set; }

        public CustomJObjectSerializerDelegate<PriceLevelSchedule>?                        CustomPriceLevelScheduleSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<PriceLevelScheduleEntry>?                   CustomPriceLevelScheduleEntrySerializer                     { get; set; }

        public CustomJObjectSerializerDelegate<ChargingProfile>?                           CustomChargingProfileSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<MessageInfo>?                               CustomMessageInfoSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<MessageContent>?                            CustomMessageContentSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<CertificateHashData>?                       CustomCertificateHashDataSerializer                         { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate <OCPP.Signature>?                            CustomBinarySignatureSerializer                             { get; set; }


        // Overlay Networking Extensions
        public CustomJObjectSerializerDelegate<NetworkTopologyInformation>?                CustomNetworkTopologyInformationSerializer                  { get; set; }

        #endregion

        #region Events

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new networking node websocket client running on a networking node
        /// and connecting to a CSMS to invoke methods.
        /// </summary>
        /// <param name="NetworkingNodeIdentity">The unique identification of this networking node.</param>
        /// <param name="From">The source URI of the websocket message.</param>
        /// <param name="To">The destination URI of the websocket message.</param>
        /// 
        /// <param name="RemoteURL">The remote URL of the HTTP endpoint to connect to.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this HTTP/websocket client.</param>
        /// <param name="RemoteCertificateValidator">The remote SSL/TLS certificate validator.</param>
        /// <param name="ClientCertificateSelector">A delegate to select a TLS client certificate.</param>
        /// <param name="ClientCert">The SSL/TLS client certificate to use of HTTP authentication.</param>
        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
        /// <param name="URLPathPrefix">An optional default URL path prefix.</param>
        /// <param name="HTTPAuthentication">The WebService-Security username/password.</param>
        /// <param name="RequestTimeout">An optional Request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="LoggingContext">An optional context for logging client methods.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public NetworkingNodeWSClient(NetworkingNode_Id                    NetworkingNodeIdentity,
                                      String                               From,
                                      String                               To,

                                      URL                                  RemoteURL,
                                      HTTPHostname?                        VirtualHostname              = null,
                                      String?                              Description                  = null,
                                      Boolean?                             PreferIPv4                   = null,
                                      RemoteCertificateValidationHandler?  RemoteCertificateValidator   = null,
                                      LocalCertificateSelectionHandler?    ClientCertificateSelector    = null,
                                      X509Certificate?                     ClientCert                   = null,
                                      SslProtocols?                        TLSProtocol                  = null,
                                      String                               HTTPUserAgent                = DefaultHTTPUserAgent,
                                      IHTTPAuthentication?                 HTTPAuthentication           = null,
                                      TimeSpan?                            RequestTimeout               = null,
                                      TransmissionRetryDelayDelegate?      TransmissionRetryDelay       = null,
                                      UInt16?                              MaxNumberOfRetries           = 3,
                                      UInt32?                              InternalBufferSize           = null,

                                      IEnumerable<String>?                 SecWebSocketProtocols        = null,
                                      NetworkingMode?                      NetworkingMode               = null,

                                      Boolean                              DisableWebSocketPings        = false,
                                      TimeSpan?                            WebSocketPingEvery           = null,
                                      TimeSpan?                            SlowNetworkSimulationDelay   = null,

                                      Boolean                              DisableMaintenanceTasks      = false,
                                      TimeSpan?                            MaintenanceEvery             = null,

                                      String?                              LoggingPath                  = null,
                                      String                               LoggingContext               = null, //CPClientLogger.DefaultContext,
                                      LogfileCreatorDelegate?              LogfileCreator               = null,
                                      HTTPClientLogger?                    HTTPLogger                   = null,
                                      DNSClient?                           DNSClient                    = null)

            : base(NetworkingNodeIdentity,
                   From,
                   To,

                   RemoteURL,
                   VirtualHostname,
                   Description,
                   PreferIPv4,
                   RemoteCertificateValidator,
                   ClientCertificateSelector,
                   ClientCert,
                   TLSProtocol,
                   HTTPUserAgent,
                   HTTPAuthentication,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,

                   SecWebSocketProtocols,
                   NetworkingMode,

                   DisableWebSocketPings,
                   WebSocketPingEvery,
                   SlowNetworkSimulationDelay,

                   DisableMaintenanceTasks,
                   MaintenanceEvery,

                   LoggingPath,
                   LoggingContext,
                   LogfileCreator,
                   HTTPLogger,
                   DNSClient)

        {

            #region Initial checks

            if (NetworkingNodeIdentity.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(NetworkingNodeIdentity),  "The given networking node identification must not be null or empty!");

            if (From.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(From),                     "The given websocket message source must not be null or empty!");

            if (To.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(To),                       "The given websocket message destination must not be null or empty!");

            #endregion

            this.NetworkingMode           = NetworkingMode ?? OCPP.WebSockets.NetworkingMode.Standard;

            //this.Logger                   = new ChargePointwebsocketClient.CPClientLogger(this,
            //                                                                         LoggingPath,
            //                                                                         LoggingContext,
            //                                                                         LogfileCreator);

            #region Reflect "Receive_XXX" messages and wire them...

            foreach (var method in typeof(NetworkingNodeWSClient).
                                       GetMethods(BindingFlags.Public | BindingFlags.Instance).
                                            Where(method            => method.Name.StartsWith("Receive_") &&
                                                 (method.ReturnType == typeof(Task<Tuple<OCPP_JSONResponseMessage?,   OCPP_JSONErrorMessage?>>) ||
                                                  method.ReturnType == typeof(Task<Tuple<OCPP_BinaryResponseMessage?, OCPP_JSONErrorMessage?>>))))
            {

                var processorName = method.Name[8..];

                if (incomingMessageProcessorsLookup.ContainsKey(processorName))
                    throw new ArgumentException("Duplicate processor name: " + processorName);

                incomingMessageProcessorsLookup.Add(processorName,
                                                    method);

            }

            #endregion

        }

        #endregion


    }

}
