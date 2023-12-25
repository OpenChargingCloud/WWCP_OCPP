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
using System.Security.Cryptography;
using System.Collections.Concurrent;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.NN;
using cloud.charging.open.protocols.OCPPv2_1.NN;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region (class) ChargingStationConnector

    /// <summary>
    /// A connector at a charging station.
    /// </summary>
    public class ChargingStationConnector
    {

        #region Properties

        public Connector_Id    Id               { get; }
        public ConnectorType  ConnectorType    { get; }

        #endregion

        #region ChargingStationConnector(Id, ConnectorType)

        public ChargingStationConnector(Connector_Id    Id,
                                        ConnectorType  ConnectorType)
        {

            this.Id             = Id;
            this.ConnectorType  = ConnectorType;

        }

        #endregion

    }

    #endregion

    #region (class) ChargingStationEVSE

    /// <summary>
    /// An EVSE at a charging station.
    /// </summary>
    public class ChargingStationEVSE
    {

        #region Properties

        public EVSE_Id            Id                       { get; }

        public Reservation_Id?    ReservationId            { get; set; }

        public OperationalStatus  AdminStatus              { get; set; }

        public ConnectorStatus    Status                   { get; set; }


        public String?            MeterType                { get; set; }
        public String?            MeterSerialNumber        { get; set; }
        public String?            MeterPublicKey           { get; set; }


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


        public ChargingTariff?    DefaultChargingTariff    { get; set; }

        #endregion

        #region ChargingStationEVSE(Id, AdminStatus, ...)

        public ChargingStationEVSE(EVSE_Id                                 Id,
                                   OperationalStatus                       AdminStatus,
                                   String?                                 MeterType           = null,
                                   String?                                 MeterSerialNumber   = null,
                                   String?                                 MeterPublicKey      = null,
                                   IEnumerable<ChargingStationConnector>?  Connectors          = null)
        {

            this.Id                 = Id;
            this.AdminStatus        = AdminStatus;
            this.MeterType          = MeterType;
            this.MeterSerialNumber  = MeterSerialNumber;
            this.MeterPublicKey     = MeterPublicKey;
            this.connectors         = Connectors is not null && Connectors.Any()
                                          ? new HashSet<ChargingStationConnector>(Connectors)
                                          : new HashSet<ChargingStationConnector>();

        }

        #endregion


        #region Connectors

        private readonly HashSet<ChargingStationConnector> connectors;

        public IEnumerable<ChargingStationConnector> Connectors
            => connectors;

        public Boolean TryGetConnector(Connector_Id ConnectorId, out ChargingStationConnector? Connector)
        {

            Connector = connectors.FirstOrDefault(connector => connector.Id == ConnectorId);

            return Connector is not null;

        }

        #endregion


    }

    #endregion


    /// <summary>
    /// A networking node for testing.
    /// </summary>
    public partial class TestNetworkingNode : INetworkingNode,
                                              IEventSender
    {

        #region Data

        private          readonly  HashSet<SignaturePolicy>                                                                 signaturePolicies           = [];

        private                    Int64                                                                                    internalRequestId           = 800000;

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of this networking node.
        /// </summary>
        public NetworkingNode_Id        Id                          { get; }

        /// <summary>
        /// The networking node vendor identification.
        /// </summary>
        [Mandatory]
        public String                   VendorName                  { get; }

        /// <summary>
        ///  The networking node model identification.
        /// </summary>
        [Mandatory]
        public String                   Model                       { get; }


        /// <summary>
        /// The optional multi-language networking node description.
        /// </summary>
        [Optional]
        public I18NString?              Description                 { get; }

        /// <summary>
        /// The optional serial number of the networking node.
        /// </summary>
        [Optional]
        public String?                  SerialNumber                { get; }

        /// <summary>
        /// The optional firmware version of the networking node.
        /// </summary>
        [Optional]
        public String?                  FirmwareVersion             { get; }

        /// <summary>
        /// The modem of the networking node.
        /// </summary>
        [Optional]
        public Modem?                   Modem                       { get; }


        public CustomData?              CustomData                  { get; set; }


        /// <summary>
        /// The time span between heartbeat requests.
        /// </summary>
        public TimeSpan                 SendHeartbeatEvery          { get; set; }

        /// <summary>
        /// The time at the CSMS.
        /// </summary>
        public DateTime?                CSMSTime                    { get; set; }

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


        #region ToDo's

        public URL RemoteURL => throw new NotImplementedException();

        public HTTPHostname? VirtualHostname => throw new NotImplementedException();

        public RemoteCertificateValidationHandler? RemoteCertificateValidator => throw new NotImplementedException();

        public X509Certificate? ClientCert => throw new NotImplementedException();

        public SslProtocols TLSProtocol => throw new NotImplementedException();

        public bool PreferIPv4 => throw new NotImplementedException();

        public string HTTPUserAgent => throw new NotImplementedException();

        public TimeSpan RequestTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TransmissionRetryDelayDelegate TransmissionRetryDelay => throw new NotImplementedException();

        public ushort MaxNumberOfRetries => throw new NotImplementedException();

        public bool UseHTTPPipelining => throw new NotImplementedException();

        public HTTPClientLogger? HTTPLogger => throw new NotImplementedException();

        #endregion


        /// <summary>
        /// The enumeration of all signature policies.
        /// </summary>
        public IEnumerable<SignaturePolicy>  SignaturePolicies
            => signaturePolicies;

        /// <summary>
        /// The currently active signature policy.
        /// </summary>
        public SignaturePolicy               SignaturePolicy
            => signaturePolicies.First();


        public  ActingAsCS               AsCS                        { get; }

        public  ActingAsCSMS             AsCSMS                      { get; }


        public  INetworkingNodeIN        IN                          { get; }

        public  INetworkingNodeOUT       OUT                         { get; }


        public CS.INetworkingNodeOutgoingMessages? CSClient
            => AsCS.CSClient;


        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => this.Id.ToString();

        public DNSClient  DNSClient                 { get; }

        #endregion

        #region Events

        // as Charging Station (meaning: as a HTTP Web Socket client)

        #endregion

        #region Custom JSON serializer delegates

        #region Charging Station Request  Messages
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.BootNotificationRequest>?                             CustomBootNotificationRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.FirmwareStatusNotificationRequest>?                   CustomFirmwareStatusNotificationRequestSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.PublishFirmwareStatusNotificationRequest>?            CustomPublishFirmwareStatusNotificationRequestSerializer     { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.HeartbeatRequest>?                                    CustomHeartbeatRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyEventRequest>?                                  CustomNotifyEventRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.SecurityEventNotificationRequest>?                    CustomSecurityEventNotificationRequestSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyReportRequest>?                                 CustomNotifyReportRequestSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyMonitoringReportRequest>?                       CustomNotifyMonitoringReportRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.LogStatusNotificationRequest>?                        CustomLogStatusNotificationRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<            DataTransferRequest>?                                 CustomDataTransferRequestSerializer                          { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.SignCertificateRequest>?                              CustomSignCertificateRequestSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.Get15118EVCertificateRequest>?                        CustomGet15118EVCertificateRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetCertificateStatusRequest>?                         CustomGetCertificateStatusRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.GetCRLRequest>?                                       CustomGetCRLRequestSerializer                                { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.ReservationStatusUpdateRequest>?                      CustomReservationStatusUpdateRequestSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.AuthorizeRequest>?                                    CustomAuthorizeRequestSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyEVChargingNeedsRequest>?                        CustomNotifyEVChargingNeedsRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.TransactionEventRequest>?                             CustomTransactionEventRequestSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.StatusNotificationRequest>?                           CustomStatusNotificationRequestSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.MeterValuesRequest>?                                  CustomMeterValuesRequestSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyChargingLimitRequest>?                          CustomNotifyChargingLimitRequestSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.ClearedChargingLimitRequest>?                         CustomClearedChargingLimitRequestSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.ReportChargingProfilesRequest>?                       CustomReportChargingProfilesRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyEVChargingScheduleRequest>?                     CustomNotifyEVChargingScheduleRequestSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyPriorityChargingRequest>?                       CustomNotifyPriorityChargingRequestSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.PullDynamicScheduleUpdateRequest>?                    CustomPullDynamicScheduleUpdateRequestSerializer             { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyDisplayMessagesRequest>?                        CustomNotifyDisplayMessagesRequestSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CS.NotifyCustomerInformationRequest>?                    CustomNotifyCustomerInformationRequestSerializer             { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate <            BinaryDataTransferRequest>?                           CustomBinaryDataTransferRequestSerializer                    { get; set; }

        #endregion

        #region CSMS Response Messages
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.BootNotificationResponse>?                       CustomBootNotificationResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.FirmwareStatusNotificationResponse>?             CustomFirmwareStatusNotificationResponseSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.PublishFirmwareStatusNotificationResponse>?      CustomPublishFirmwareStatusNotificationResponseSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.HeartbeatResponse>?                              CustomHeartbeatResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyEventResponse>?                            CustomNotifyEventResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SecurityEventNotificationResponse>?              CustomSecurityEventNotificationResponseSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyReportResponse>?                           CustomNotifyReportResponseSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyMonitoringReportResponse>?                 CustomNotifyMonitoringReportResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.LogStatusNotificationResponse>?                  CustomLogStatusNotificationResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<              DataTransferResponse>?                           CustomDataTransferResponseSerializer                         { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.SignCertificateResponse>?                        CustomSignCertificateResponseSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.Get15118EVCertificateResponse>?                  CustomGet15118EVCertificateResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetCertificateStatusResponse>?                   CustomGetCertificateStatusResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.GetCRLResponse>?                                 CustomGetCRLResponseSerializer                               { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ReservationStatusUpdateResponse>?                CustomReservationStatusUpdateResponseSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.AuthorizeResponse>?                              CustomAuthorizeResponseSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyEVChargingNeedsResponse>?                  CustomNotifyEVChargingNeedsResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.TransactionEventResponse>?                       CustomTransactionEventResponseSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.StatusNotificationResponse>?                     CustomStatusNotificationResponseSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.MeterValuesResponse>?                            CustomMeterValuesResponseSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyChargingLimitResponse>?                    CustomNotifyChargingLimitResponseSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ClearedChargingLimitResponse>?                   CustomClearedChargingLimitResponseSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.ReportChargingProfilesResponse>?                 CustomReportChargingProfilesResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyEVChargingScheduleResponse>?               CustomNotifyEVChargingScheduleResponseSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyPriorityChargingResponse>?                 CustomNotifyPriorityChargingResponseSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse>?              CustomPullDynamicScheduleUpdateResponseSerializer            { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyDisplayMessagesResponse>?                  CustomNotifyDisplayMessagesResponseSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.NotifyCustomerInformationResponse>?              CustomNotifyCustomerInformationResponseSerializer            { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate <              BinaryDataTransferResponse>?                     CustomBinaryDataTransferResponseSerializer                   { get; set; }

        #endregion


        public CustomJObjectSerializerDelegate<NotifyNetworkTopologyRequest>?                CustomNotifyNetworkTopologyRequestSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<NotifyNetworkTopologyResponse>?               CustomNotifyNetworkTopologyResponseSerializer          { get; set; }


        #region Data Structures

        public CustomJObjectSerializerDelegate<StatusInfo>?                                          CustomStatusInfoSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<EVSEStatusInfo<SetDefaultChargingTariffStatus>>?      CustomEVSEStatusInfoSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<EVSEStatusInfo<RemoveDefaultChargingTariffStatus>>?   CustomEVSEStatusInfoSerializer2                              { get; set; }
        public CustomJObjectSerializerDelegate<OCPP.Signature>?                                      CustomSignatureSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<Firmware>?                                            CustomFirmwareSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<ComponentVariable>?                                   CustomComponentVariableSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<Component>?                                           CustomComponentSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                                                CustomEVSESerializer                                         { get; set; }
        public CustomJObjectSerializerDelegate<Variable>?                                            CustomVariableSerializer                                     { get; set; }
        public CustomJObjectSerializerDelegate<PeriodicEventStreamParameters>?                       CustomPeriodicEventStreamParametersSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<LogParameters>?                                       CustomLogParametersSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<SetVariableData>?                                     CustomSetVariableDataSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<GetVariableData>?                                     CustomGetVariableDataSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringData>?                                   CustomSetMonitoringDataSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<NetworkConnectionProfile>?                            CustomNetworkConnectionProfileSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<VPNConfiguration>?                                    CustomVPNConfigurationSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<APNConfiguration>?                                    CustomAPNConfigurationSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<CertificateHashData>?                                 CustomCertificateHashDataSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<AuthorizationData>?                                   CustomAuthorizationDataSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<IdToken>?                                             CustomIdTokenSerializer                                      { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalInfo>?                                      CustomAdditionalInfoSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<IdTokenInfo>?                                         CustomIdTokenInfoSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<MessageContent>?                                      CustomMessageContentSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ChargingProfile>?                                     CustomChargingProfileSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<LimitBeyondSoC>?                                      CustomLimitBeyondSoCSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedule>?                                    CustomChargingScheduleSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                              CustomChargingSchedulePeriodSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                                    CustomV2XFreqWattEntrySerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                                  CustomV2XSignalWattEntrySerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariff>?                                         CustomSalesTariffSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<SalesTariffEntry>?                                    CustomSalesTariffEntrySerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<RelativeTimeInterval>?                                CustomRelativeTimeIntervalSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<ConsumptionCost>?                                     CustomConsumptionCostSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<Cost>?                                                CustomCostSerializer                                         { get; set; }

        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AbsolutePriceSchedule>?    CustomAbsolutePriceScheduleSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRuleStack>?           CustomPriceRuleStackSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRule>?                CustomPriceRuleSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.TaxRule>?                  CustomTaxRuleSerializer                                      { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRuleList>?         CustomOverstayRuleListSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRule>?             CustomOverstayRuleSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AdditionalService>?        CustomAdditionalServiceSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelSchedule>?       CustomPriceLevelScheduleSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntrySerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<TransactionLimits>?                                   CustomTransactionLimitsSerializer                            { get; set; }

        public CustomJObjectSerializerDelegate<ChargingProfileCriterion>?                            CustomChargingProfileCriterionSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<ClearChargingProfile>?                                CustomClearChargingProfileSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<MessageInfo>?                                         CustomMessageInfoSerializer                                  { get; set; }



        public CustomJObjectSerializerDelegate<ChargingStation>?                                     CustomChargingStationSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<EventData>?                                           CustomEventDataSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<ReportData>?                                          CustomReportDataSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<VariableAttribute>?                                   CustomVariableAttributeSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<VariableCharacteristics>?                             CustomVariableCharacteristicsSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<MonitoringData>?                                      CustomMonitoringDataSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<VariableMonitoring>?                                  CustomVariableMonitoringSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<OCSPRequestData>?                                     CustomOCSPRequestDataSerializer                              { get; set; }

        public CustomJObjectSerializerDelegate<ChargingNeeds>?                                       CustomChargingNeedsSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<ACChargingParameters>?                                CustomACChargingParametersSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<DCChargingParameters>?                                CustomDCChargingParametersSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<V2XChargingParameters>?                               CustomV2XChargingParametersSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<EVEnergyOffer>?                                       CustomEVEnergyOfferSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<EVPowerSchedule>?                                     CustomEVPowerScheduleSerializer                              { get; set; }
        public CustomJObjectSerializerDelegate<EVPowerScheduleEntry>?                                CustomEVPowerScheduleEntrySerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EVAbsolutePriceSchedule>?                             CustomEVAbsolutePriceScheduleSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?                        CustomEVAbsolutePriceScheduleEntrySerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<EVPriceRule>?                                         CustomEVPriceRuleSerializer                                  { get; set; }

        public CustomJObjectSerializerDelegate<Transaction>?                                         CustomTransactionSerializer                                  { get; set; }
        public CustomJObjectSerializerDelegate<MeterValue>?                                          CustomMeterValueSerializer                                   { get; set; }
        public CustomJObjectSerializerDelegate<SampledValue>?                                        CustomSampledValueSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<SignedMeterValue>?                                    CustomSignedMeterValueSerializer                             { get; set; }
        public CustomJObjectSerializerDelegate<UnitsOfMeasure>?                                      CustomUnitsOfMeasureSerializer                               { get; set; }

        public CustomJObjectSerializerDelegate<SetVariableResult>?                                   CustomSetVariableResultSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<GetVariableResult>?                                   CustomGetVariableResultSerializer                            { get; set; }
        public CustomJObjectSerializerDelegate<SetMonitoringResult>?                                 CustomSetMonitoringResultSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<ClearMonitoringResult>?                               CustomClearMonitoringResultSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<CompositeSchedule>?                                   CustomCompositeScheduleSerializer                            { get; set; }


        // Binary Data Streams Extensions
        public CustomBinarySerializerDelegate<OCPP.Signature>?                                       CustomBinarySignatureSerializer                              { get; set; }


        // E2E Security Extensions



        // E2E Charging Tariff Extensions
        public CustomJObjectSerializerDelegate<ChargingTariff>?                                      CustomChargingTariffSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<Price>?                                               CustomPriceSerializer                                        { get; set; }
        public CustomJObjectSerializerDelegate<TariffElement>?                                       CustomTariffElementSerializer                                { get; set; }
        public CustomJObjectSerializerDelegate<PriceComponent>?                                      CustomPriceComponentSerializer                               { get; set; }
        public CustomJObjectSerializerDelegate<TaxRate>?                                             CustomTaxRateSerializer                                      { get; set; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?                                  CustomTariffRestrictionsSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                                           CustomEnergyMixSerializer                                    { get; set; }
        public CustomJObjectSerializerDelegate<EnergySource>?                                        CustomEnergySourceSerializer                                 { get; set; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?                                 CustomEnvironmentalImpactSerializer                          { get; set; }


        // Overlay Networking Extensions
        public CustomJObjectSerializerDelegate<NetworkTopologyInformation>?                          CustomNetworkTopologyInformationSerializer             { get; set; }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new networking node for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this networking node.</param>
        public TestNetworkingNode(NetworkingNode_Id     Id,
                                  String                VendorName,
                                  String                Model,

                                  I18NString?           Description               = null,
                                  String?               SerialNumber              = null,
                                  String?               FirmwareVersion           = null,
                                  Modem?                Modem                     = null,

                                  Boolean               DisableSendHeartbeats     = false,
                                  TimeSpan?             SendHeartbeatEvery        = null,

                                  Boolean               DisableMaintenanceTasks   = false,
                                  TimeSpan?             MaintenanceEvery          = null,

                                  TimeSpan?             DefaultRequestTimeout     = null,
                                  IHTTPAuthentication?  HTTPAuthentication        = null,
                                  DNSClient?            DNSClient                 = null,

                                  SignaturePolicy?      SignaturePolicy           = null)

        {

            if (Id.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(Id),          "The given networking node identification must not be null or empty!");

            if (VendorName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(VendorName),  "The given networking node vendor must not be null or empty!");

            if (Model.     IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Model),       "The given networking node model must not be null or empty!");

            this.Id                       = Id;
            this.VendorName               = VendorName;
            this.Model                    = Model;
            this.Description              = Description;
            this.SerialNumber             = SerialNumber;
            this.FirmwareVersion          = FirmwareVersion;
            this.Modem                    = Modem;


            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "HTTPSSEs"));

            this.DNSClient                = DNSClient ?? new DNSClient(SearchForIPv6DNSServers: false);

            this.signaturePolicies.Add(SignaturePolicy ?? new SignaturePolicy());

         //   this.EnqueuedRequests         = [];


            this.AsCSMS                   = new ActingAsCSMS(

                                                NetworkingNode:            this,
                                                RequireAuthentication:     false,
                                                DefaultRequestTimeout:     this.DefaultRequestTimeout,
                                                HTTPUploadPort:            null,
                                                DNSClient:                 this.DNSClient,

                                                SignaturePolicy:           this.SignaturePolicy

                                            );

            this.AsCS                     = new ActingAsCS(

                                                NetworkingNode:            this,
                                                VendorName:                this.VendorName,
                                                Model:                     this.Model,

                                                Description:               this.Description,
                                                SerialNumber:              this.SerialNumber,
                                                FirmwareVersion:           this.FirmwareVersion,
                                                Modem:                     this.Modem,

                                                DisableSendHeartbeats:     this.DisableSendHeartbeats,
                                                SendHeartbeatEvery:        this.SendHeartbeatEvery,

                                                DisableMaintenanceTasks:   this.DisableMaintenanceTasks,
                                                MaintenanceEvery:          this.MaintenanceEvery,

                                                DefaultRequestTimeout:     this.DefaultRequestTimeout,
                                                HTTPAuthentication:        null,
                                                DNSClient:                 this.DNSClient,

                                                SignaturePolicy:           this.SignaturePolicy

                                            );

            this.IN   = new INN (this);
            this.OUT  = new OUTT(this);

        }

        #endregion


        #region ConnectWebSocket(...)

        public Task<HTTPResponse?> ConnectWebSocket(String                               From,
                                                    String                               To,

                                                    URL                                  RemoteURL,
                                                    HTTPHostname?                        VirtualHostname              = null,
                                                    String?                              Description                  = null,
                                                    RemoteCertificateValidationHandler?  RemoteCertificateValidator   = null,
                                                    LocalCertificateSelectionHandler?    ClientCertificateSelector    = null,
                                                    X509Certificate?                     ClientCert                   = null,
                                                    SslProtocols?                        TLSProtocol                  = null,
                                                    Boolean?                             PreferIPv4                   = null,
                                                    String?                              HTTPUserAgent                = null,
                                                    IHTTPAuthentication?                 HTTPAuthentication           = null,
                                                    TimeSpan?                            RequestTimeout               = null,
                                                    TransmissionRetryDelayDelegate?      TransmissionRetryDelay       = null,
                                                    UInt16?                              MaxNumberOfRetries           = null,
                                                    UInt32?                              InternalBufferSize           = null,

                                                    IEnumerable<String>?                 SecWebSocketProtocols        = null,
                                                    NetworkingMode?                      NetworkingMode               = null,

                                                    Boolean                              DisableMaintenanceTasks      = false,
                                                    TimeSpan?                            MaintenanceEvery             = null,
                                                    Boolean                              DisableWebSocketPings        = false,
                                                    TimeSpan?                            WebSocketPingEvery           = null,
                                                    TimeSpan?                            SlowNetworkSimulationDelay   = null,

                                                    String?                              LoggingPath                  = null,
                                                    String?                              LoggingContext               = null,
                                                    LogfileCreatorDelegate?              LogfileCreator               = null,
                                                    HTTPClientLogger?                    HTTPLogger                   = null,
                                                    DNSClient?                           DNSClient                    = null)

            => AsCS.ConnectWebSocket(From,
                                     To,

                                     RemoteURL,
                                     VirtualHostname,
                                     Description,
                                     RemoteCertificateValidator,
                                     ClientCertificateSelector,
                                     ClientCert,
                                     TLSProtocol,
                                     PreferIPv4,
                                     HTTPUserAgent,
                                     HTTPAuthentication,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,

                                     SecWebSocketProtocols,
                                     NetworkingMode,

                                     DisableMaintenanceTasks,
                                     MaintenanceEvery,
                                     DisableWebSocketPings,
                                     WebSocketPingEvery,
                                     SlowNetworkSimulationDelay,

                                     LoggingPath,
                                     LoggingContext,
                                     LogfileCreator,
                                     HTTPLogger,
                                     DNSClient);

        #endregion


        #region HandleErrors(Module, Caller, ExceptionOccured)

        private Task HandleErrors(String     Module,
                                  String     Caller,
                                  Exception  ExceptionOccured)
        {

            DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

            return Task.CompletedTask;

        }

        #endregion


        public Boolean LookupNetworkingNode(NetworkingNode_Id NetworkingNodeId, out ICommunicationChannel? CommunicationChannel)
        {

            //var lookUpNetworkingNodeId = NetworkingNodeId;

            ////if (reachableViaNetworkingHubs.TryGetValue(lookUpNetworkingNodeId, out var networkingHubId))
            ////    lookUpNetworkingNodeId = networkingHubId;

            //if (reachableChargingStations.TryGetValue(lookUpNetworkingNodeId, out var networkingNodeChannel) &&
            //    networkingNodeChannel?.Item1 is not null)
            //{
            //    NetworkingNodeChannel = networkingNodeChannel.Item1;
            //    return true;
            //}

            CommunicationChannel = null;
            return false;

        }


        #region NextRequestId

        public Request_Id NextRequestId
        {
            get
            {

                Interlocked.Increment(ref internalRequestId);

                return Request_Id.Parse(internalRequestId.ToString());

            }
        }

        public string? ClientCloseMessage => throw new NotImplementedException();

        #endregion


    }

}
