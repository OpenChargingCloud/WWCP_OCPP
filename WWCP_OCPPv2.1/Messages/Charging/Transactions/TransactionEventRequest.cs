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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A transaction event request.
    /// </summary>
    public class TransactionEventRequest : ARequest<TransactionEventRequest>,
                                           IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/transactionEventRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext            Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The type of this transaction event.
        /// The first event of a transaction SHALL be of type "started", the last of type "ended".
        /// All others should be of type "updated".
        /// </summary>
        [Mandatory]
        public TransactionEvents        EventType                { get; }

        /// <summary>
        /// The timestamp at which this transaction event occurred.
        /// </summary>
        [Mandatory]
        public DateTime                 Timestamp                { get; }

        /// <summary>
        /// The reason the charging station sends this message.
        /// </summary>
        [Mandatory]
        public TriggerReason            TriggerReason            { get; }

        /// <summary>
        /// This incremental sequence number, helps to determine whether all messages of a transaction have been received.
        /// </summary>
        public UInt32                   SequenceNumber           { get; }

        /// <summary>
        /// Transaction related information.
        /// </summary>
        [Mandatory]
        public Transaction              TransactionInfo          { get; }

        /// <summary>
        /// The optional indication whether this transaction event happened when the charging station was offline.
        /// </summary>
        [Optional]
        public Boolean?                 Offline                  { get; }

        /// <summary>
        /// The optional numer of electrical phases used, when the charging station is able to report it.
        /// </summary>
        [Optional]
        public Byte?                    NumberOfPhasesUsed       { get; }

        /// <summary>
        /// The optional maximum current of the connected cable in amperes.
        /// </summary>
        [Optional]
        public Ampere?                  CableMaxCurrent          { get; }

        /// <summary>
        /// The optional unqiue reservation identification of the reservation that terminated as a result of this transaction.
        /// </summary>
        [Optional]
        public Reservation_Id?          ReservationId            { get; }

        /// <summary>
        /// The optional identification token for which a transaction has to be/was started.
        /// Is required when the EV driver becomes authorized for this transaction.
        /// The identification token should only be send once in a TransactionEventRequest for every authorization done for this transaction.
        /// </summary>
        [Optional]
        public IdToken?                 IdToken                  { get; }

        /// <summary>
        /// The optional indication of the EVSE (and connector) used.
        /// </summary>
        [Optional]
        public EVSE?                    EVSE                     { get; }

        /// <summary>
        /// The optional enumeration of meter values.
        /// </summary>
        [Optional]
        public IEnumerable<MeterValue>  MeterValues              { get; }

        /// <summary>
        /// The optional current status of the battery management system within the EV.
        /// </summary>
        public PreconditioningStatus?   PreconditioningStatus    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a transaction event request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="EventType">The type of this transaction event. The first event of a transaction SHALL be of type "started", the last of type "ended". All others should be of type "updated".</param>
        /// <param name="Timestamp">The timestamp at which this transaction event occurred.</param>
        /// <param name="TriggerReason">The reason the charging station sends this message.</param>
        /// <param name="SequenceNumber">This incremental sequence number, helps to determine whether all messages of a transaction have been received.</param>
        /// <param name="TransactionInfo">Transaction related information.</param>
        /// 
        /// <param name="Offline">An optional indication whether this transaction event happened when the charging station was offline.</param>
        /// <param name="NumberOfPhasesUsed">An optional numer of electrical phases used, when the charging station is able to report it.</param>
        /// <param name="CableMaxCurrent">An optional maximum current of the connected cable in amperes.</param>
        /// <param name="ReservationId">An optional unqiue reservation identification of the reservation that terminated as a result of this transaction.</param>
        /// <param name="IdToken">An optional identification token for which a transaction has to be/was started.</param>
        /// <param name="EVSE">An optional indication of the EVSE (and connector) used.</param>
        /// <param name="MeterValues">An optional enumeration of meter values.</param>
        /// <param name="PreconditioningStatus">The optional current status of the battery management system within the EV.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public TransactionEventRequest(SourceRouting             Destination,
                                       TransactionEvents         EventType,
                                       DateTime                  Timestamp,
                                       TriggerReason             TriggerReason,
                                       UInt32                    SequenceNumber,
                                       Transaction               TransactionInfo,

                                       Boolean?                  Offline                 = null,
                                       Byte?                     NumberOfPhasesUsed      = null,
                                       Ampere?                   CableMaxCurrent         = null,
                                       Reservation_Id?           ReservationId           = null,
                                       IdToken?                  IdToken                 = null,
                                       EVSE?                     EVSE                    = null,
                                       IEnumerable<MeterValue>?  MeterValues             = null,
                                       PreconditioningStatus?    PreconditioningStatus   = null,

                                       IEnumerable<KeyPair>?     SignKeys                = null,
                                       IEnumerable<SignInfo>?    SignInfos               = null,
                                       IEnumerable<Signature>?   Signatures              = null,

                                       CustomData?               CustomData              = null,

                                       Request_Id?               RequestId               = null,
                                       DateTime?                 RequestTimestamp        = null,
                                       TimeSpan?                 RequestTimeout          = null,
                                       EventTracking_Id?         EventTrackingId         = null,
                                       NetworkPath?              NetworkPath             = null,
                                       SerializationFormats?     SerializationFormat     = null,
                                       CancellationToken         CancellationToken       = default)

            : base(Destination,
                   nameof(TransactionEventRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.EventType              = EventType;
            this.Timestamp              = Timestamp;
            this.TriggerReason          = TriggerReason;
            this.SequenceNumber         = SequenceNumber;
            this.TransactionInfo        = TransactionInfo;

            this.Offline                = Offline;
            this.NumberOfPhasesUsed     = NumberOfPhasesUsed;
            this.CableMaxCurrent        = CableMaxCurrent;
            this.ReservationId          = ReservationId;
            this.IdToken                = IdToken;
            this.EVSE                   = EVSE;
            this.MeterValues            = MeterValues?.Distinct() ?? Array.Empty<MeterValue>();
            this.PreconditioningStatus  = PreconditioningStatus;


            unchecked
            {

                hashCode = this.EventType.             GetHashCode()       * 43 ^
                           this.Timestamp.             GetHashCode()       * 41 ^
                           this.TriggerReason.         GetHashCode()       * 37 ^
                           this.SequenceNumber.        GetHashCode()       * 31 ^
                           this.TransactionInfo.       GetHashCode()       * 29 ^
                          (this.Offline?.              GetHashCode() ?? 0) * 23 ^
                          (this.NumberOfPhasesUsed?.   GetHashCode() ?? 0) * 19 ^
                          (this.CableMaxCurrent?.      GetHashCode() ?? 0) * 17 ^
                          (this.ReservationId?.        GetHashCode() ?? 0) * 13 ^
                          (this.IdToken?.              GetHashCode() ?? 0) * 11 ^
                          (this.EVSE?.                 GetHashCode() ?? 0) *  7 ^
                          (this.PreconditioningStatus?.GetHashCode() ?? 0) *  5 ^
                           this.MeterValues.           CalcHashCode()      *  3 ^
                           base.                       GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:TransactionEventRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "ChargingStateEnumType": {
        //             "description": "Current charging state, is required when state\r\nhas changed. Omitted when there is no communication between EVSE and EV, because no cable is plugged in.",
        //             "javaType": "ChargingStateEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "EVConnected",
        //                 "Charging",
        //                 "SuspendedEV",
        //                 "SuspendedEVSE",
        //                 "Idle"
        //             ]
        //         },
        //         "CostDimensionEnumType": {
        //             "description": "Type of cost dimension: energy, power, time, etc.",
        //             "javaType": "CostDimensionEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Energy",
        //                 "MaxCurrent",
        //                 "MinCurrent",
        //                 "MaxPower",
        //                 "MinPower",
        //                 "IdleTIme",
        //                 "ChargingTime"
        //             ]
        //         },
        //         "LocationEnumType": {
        //             "description": "Indicates where the measured value has been sampled. Default =  \"Outlet\"",
        //             "javaType": "LocationEnum",
        //             "type": "string",
        //             "default": "Outlet",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Body",
        //                 "Cable",
        //                 "EV",
        //                 "Inlet",
        //                 "Outlet",
        //                 "Upstream"
        //             ]
        //         },
        //         "MeasurandEnumType": {
        //             "description": "Type of measurement. Default = \"Energy.Active.Import.Register\"",
        //             "javaType": "MeasurandEnum",
        //             "type": "string",
        //             "default": "Energy.Active.Import.Register",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Current.Export",
        //                 "Current.Export.Offered",
        //                 "Current.Export.Minimum",
        //                 "Current.Import",
        //                 "Current.Import.Offered",
        //                 "Current.Import.Minimum",
        //                 "Current.Offered",
        //                 "Display.PresentSOC",
        //                 "Display.MinimumSOC",
        //                 "Display.TargetSOC",
        //                 "Display.MaximumSOC",
        //                 "Display.RemainingTimeToMinimumSOC",
        //                 "Display.RemainingTimeToTargetSOC",
        //                 "Display.RemainingTimeToMaximumSOC",
        //                 "Display.ChargingComplete",
        //                 "Display.BatteryEnergyCapacity",
        //                 "Display.InletHot",
        //                 "Energy.Active.Export.Interval",
        //                 "Energy.Active.Export.Register",
        //                 "Energy.Active.Import.Interval",
        //                 "Energy.Active.Import.Register",
        //                 "Energy.Active.Import.CableLoss",
        //                 "Energy.Active.Import.LocalGeneration.Register",
        //                 "Energy.Active.Net",
        //                 "Energy.Active.Setpoint.Interval",
        //                 "Energy.Apparent.Export",
        //                 "Energy.Apparent.Import",
        //                 "Energy.Apparent.Net",
        //                 "Energy.Reactive.Export.Interval",
        //                 "Energy.Reactive.Export.Register",
        //                 "Energy.Reactive.Import.Interval",
        //                 "Energy.Reactive.Import.Register",
        //                 "Energy.Reactive.Net",
        //                 "EnergyRequest.Target",
        //                 "EnergyRequest.Minimum",
        //                 "EnergyRequest.Maximum",
        //                 "EnergyRequest.Minimum.V2X",
        //                 "EnergyRequest.Maximum.V2X",
        //                 "EnergyRequest.Bulk",
        //                 "Frequency",
        //                 "Power.Active.Export",
        //                 "Power.Active.Import",
        //                 "Power.Active.Setpoint",
        //                 "Power.Active.Residual",
        //                 "Power.Export.Minimum",
        //                 "Power.Export.Offered",
        //                 "Power.Factor",
        //                 "Power.Import.Offered",
        //                 "Power.Import.Minimum",
        //                 "Power.Offered",
        //                 "Power.Reactive.Export",
        //                 "Power.Reactive.Import",
        //                 "SoC",
        //                 "Voltage",
        //                 "Voltage.Minimum",
        //                 "Voltage.Maximum"
        //             ]
        //         },
        //         "OperationModeEnumType": {
        //             "description": "*(2.1)* The _operationMode_ that is currently in effect for the transaction.",
        //             "javaType": "OperationModeEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Idle",
        //                 "ChargingOnly",
        //                 "CentralSetpoint",
        //                 "ExternalSetpoint",
        //                 "ExternalLimits",
        //                 "CentralFrequency",
        //                 "LocalFrequency",
        //                 "LocalLoadBalancing"
        //             ]
        //         },
        //         "PhaseEnumType": {
        //             "description": "Indicates how the measured value is to be interpreted. For instance between L1 and neutral (L1-N) Please note that not all values of phase are applicable to all Measurands. When phase is absent, the measured value is interpreted as an overall value.",
        //             "javaType": "PhaseEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "L1",
        //                 "L2",
        //                 "L3",
        //                 "N",
        //                 "L1-N",
        //                 "L2-N",
        //                 "L3-N",
        //                 "L1-L2",
        //                 "L2-L3",
        //                 "L3-L1"
        //             ]
        //         },
        //         "PreconditioningStatusEnumType": {
        //             "description": "*(2.1)* The current preconditioning status of the BMS in the EV. Default value is Unknown.",
        //             "javaType": "PreconditioningStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Unknown",
        //                 "Ready",
        //                 "NotReady",
        //                 "Preconditioning"
        //             ]
        //         },
        //         "ReadingContextEnumType": {
        //             "description": "Type of detail value: start, end or sample. Default = \"Sample.Periodic\"",
        //             "javaType": "ReadingContextEnum",
        //             "type": "string",
        //             "default": "Sample.Periodic",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Interruption.Begin",
        //                 "Interruption.End",
        //                 "Other",
        //                 "Sample.Clock",
        //                 "Sample.Periodic",
        //                 "Transaction.Begin",
        //                 "Transaction.End",
        //                 "Trigger"
        //             ]
        //         },
        //         "ReasonEnumType": {
        //             "description": "The _stoppedReason_ is the reason/event that initiated the process of stopping the transaction. It will normally be the user stopping authorization via card (Local or MasterPass) or app (Remote), but it can also be CSMS revoking authorization (DeAuthorized), or disconnecting the EV when TxStopPoint = EVConnected (EVDisconnected). Most other reasons are related to technical faults or energy limitations. +\r\nMAY only be omitted when _stoppedReason_ is \"Local\"",
        //             "javaType": "ReasonEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "DeAuthorized",
        //                 "EmergencyStop",
        //                 "EnergyLimitReached",
        //                 "EVDisconnected",
        //                 "GroundFault",
        //                 "ImmediateReset",
        //                 "MasterPass",
        //                 "Local",
        //                 "LocalOutOfCredit",
        //                 "Other",
        //                 "OvercurrentFault",
        //                 "PowerLoss",
        //                 "PowerQuality",
        //                 "Reboot",
        //                 "Remote",
        //                 "SOCLimitReached",
        //                 "StoppedByEV",
        //                 "TimeLimitReached",
        //                 "Timeout",
        //                 "ReqEnergyTransferRejected"
        //             ]
        //         },
        //         "TariffCostEnumType": {
        //             "description": "Type of cost: normal or the minimum or maximum cost.",
        //             "javaType": "TariffCostEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "NormalCost",
        //                 "MinCost",
        //                 "MaxCost"
        //             ]
        //         },
        //         "TransactionEventEnumType": {
        //             "description": "This contains the type of this event.\r\nThe first TransactionEvent of a transaction SHALL contain: \"Started\" The last TransactionEvent of a transaction SHALL contain: \"Ended\" All others SHALL contain: \"Updated\"",
        //             "javaType": "TransactionEventEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Ended",
        //                 "Started",
        //                 "Updated"
        //             ]
        //         },
        //         "TriggerReasonEnumType": {
        //             "description": "Reason the Charging Station sends this message to the CSMS",
        //             "javaType": "TriggerReasonEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "AbnormalCondition",
        //                 "Authorized",
        //                 "CablePluggedIn",
        //                 "ChargingRateChanged",
        //                 "ChargingStateChanged",
        //                 "CostLimitReached",
        //                 "Deauthorized",
        //                 "EnergyLimitReached",
        //                 "EVCommunicationLost",
        //                 "EVConnectTimeout",
        //                 "EVDeparted",
        //                 "EVDetected",
        //                 "LimitSet",
        //                 "MeterValueClock",
        //                 "MeterValuePeriodic",
        //                 "OperationModeChanged",
        //                 "RemoteStart",
        //                 "RemoteStop",
        //                 "ResetCommand",
        //                 "RunningCost",
        //                 "SignedDataReceived",
        //                 "SoCLimitReached",
        //                 "StopAuthorized",
        //                 "TariffChanged",
        //                 "TariffNotAccepted",
        //                 "TimeLimitReached",
        //                 "Trigger",
        //                 "TxResumed",
        //                 "UnlockCommand"
        //             ]
        //         },
        //         "AdditionalInfoType": {
        //             "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.",
        //             "javaType": "AdditionalInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "additionalIdToken": {
        //                     "description": "*(2.1)* This field specifies the additional IdToken.",
        //                     "type": "string",
        //                     "maxLength": 255
        //                 },
        //                 "type": {
        //                     "description": "_additionalInfo_ can be used to send extra information to CSMS in addition to the regular authorization with _IdToken_. _AdditionalInfo_ contains one or more custom _types_, which need to be agreed upon by all parties involved. When the _type_ is not supported, the CSMS/Charging Station MAY ignore the _additionalInfo_.",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "additionalIdToken",
        //                 "type"
        //             ]
        //         },
        //         "ChargingPeriodType": {
        //             "description": "A ChargingPeriodType consists of a start time, and a list of possible values that influence this period, for example: amount of energy charged this period, maximum current during this period etc.",
        //             "javaType": "ChargingPeriod",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "dimensions": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/CostDimensionType"
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "tariffId": {
        //                     "description": "Unique identifier of the Tariff that was used to calculate cost. If not provided, then cost was calculated by some other means.",
        //                     "type": "string",
        //                     "maxLength": 60
        //                 },
        //                 "startPeriod": {
        //                     "description": "Start timestamp of charging period. A period ends when the next period starts. The last period ends when the session ends.",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "startPeriod"
        //             ]
        //         },
        //         "CostDetailsType": {
        //             "description": "CostDetailsType contains the cost as calculated by Charging Station based on provided TariffType.\r\n\r\nNOTE: Reservation is not shown as a _chargingPeriod_, because it took place outside of the transaction.",
        //             "javaType": "CostDetails",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "chargingPeriods": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/ChargingPeriodType"
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "totalCost": {
        //                     "$ref": "#/definitions/TotalCostType"
        //                 },
        //                 "totalUsage": {
        //                     "$ref": "#/definitions/TotalUsageType"
        //                 },
        //                 "failureToCalculate": {
        //                     "description": "If set to true, then Charging Station has failed to calculate the cost.",
        //                     "type": "boolean"
        //                 },
        //                 "failureReason": {
        //                     "description": "Optional human-readable reason text in case of failure to calculate.",
        //                     "type": "string",
        //                     "maxLength": 500
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "totalCost",
        //                 "totalUsage"
        //             ]
        //         },
        //         "CostDimensionType": {
        //             "description": "Volume consumed of cost dimension.",
        //             "javaType": "CostDimension",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "type": {
        //                     "$ref": "#/definitions/CostDimensionEnumType"
        //                 },
        //                 "volume": {
        //                     "description": "Volume of the dimension consumed, measured according to the dimension type.",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "type",
        //                 "volume"
        //             ]
        //         },
        //         "EVSEType": {
        //             "description": "Electric Vehicle Supply Equipment",
        //             "javaType": "EVSE",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "id": {
        //                     "description": "EVSE Identifier. This contains a number (&gt; 0) designating an EVSE of the Charging Station.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "connectorId": {
        //                     "description": "An id to designate a specific connector (on an EVSE) by connector index number.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "id"
        //             ]
        //         },
        //         "IdTokenType": {
        //             "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.",
        //             "javaType": "IdToken",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "additionalInfo": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/AdditionalInfoType"
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "idToken": {
        //                     "description": "*(2.1)* IdToken is case insensitive. Might hold the hidden id of an RFID tag, but can for example also contain a UUID.",
        //                     "type": "string",
        //                     "maxLength": 255
        //                 },
        //                 "type": {
        //                     "description": "*(2.1)* Enumeration of possible idToken types. Values defined in Appendix as IdTokenEnumStringType.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "idToken",
        //                 "type"
        //             ]
        //         },
        //         "MeterValueType": {
        //             "description": "Collection of one or more sampled values in MeterValuesRequest and TransactionEvent. All sampled values in a MeterValue are sampled at the same point in time.",
        //             "javaType": "MeterValue",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "sampledValue": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/SampledValueType"
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "timestamp": {
        //                     "description": "Timestamp for measured value(s).",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "timestamp",
        //                 "sampledValue"
        //             ]
        //         },
        //         "PriceType": {
        //             "description": "Price with and without tax. At least one of _exclTax_, _inclTax_ must be present.",
        //             "javaType": "Price",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "exclTax": {
        //                     "description": "Price/cost excluding tax. Can be absent if _inclTax_ is present.",
        //                     "type": "number"
        //                 },
        //                 "inclTax": {
        //                     "description": "Price/cost including tax. Can be absent if _exclTax_ is present.",
        //                     "type": "number"
        //                 },
        //                 "taxRates": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/TaxRateType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 5
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
        //         },
        //         "SampledValueType": {
        //             "description": "Single sampled value in MeterValues. Each value can be accompanied by optional fields.\r\n\r\nTo save on mobile data usage, default values of all of the optional fields are such that. The value without any additional fields will be interpreted, as a register reading of active import energy in Wh (Watt-hour) units.",
        //             "javaType": "SampledValue",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "value": {
        //                     "description": "Indicates the measured value.",
        //                     "type": "number"
        //                 },
        //                 "measurand": {
        //                     "$ref": "#/definitions/MeasurandEnumType"
        //                 },
        //                 "context": {
        //                     "$ref": "#/definitions/ReadingContextEnumType"
        //                 },
        //                 "phase": {
        //                     "$ref": "#/definitions/PhaseEnumType"
        //                 },
        //                 "location": {
        //                     "$ref": "#/definitions/LocationEnumType"
        //                 },
        //                 "signedMeterValue": {
        //                     "$ref": "#/definitions/SignedMeterValueType"
        //                 },
        //                 "unitOfMeasure": {
        //                     "$ref": "#/definitions/UnitOfMeasureType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "value"
        //             ]
        //         },
        //         "SignedMeterValueType": {
        //             "description": "Represent a signed version of the meter value.",
        //             "javaType": "SignedMeterValue",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "signedMeterData": {
        //                     "description": "Base64 encoded, contains the signed data from the meter in the format specified in _encodingMethod_, which might contain more then just the meter value. It can contain information like timestamps, reference to a customer etc.",
        //                     "type": "string",
        //                     "maxLength": 32768
        //                 },
        //                 "signingMethod": {
        //                     "description": "*(2.1)* Method used to create the digital signature. Optional, if already included in _signedMeterData_. Standard values for this are defined in Appendix as SigningMethodEnumStringType.",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "encodingMethod": {
        //                     "description": "Format used by the energy meter to encode the meter data. For example: OCMF or EDL.",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "publicKey": {
        //                     "description": "*(2.1)* Base64 encoded, sending depends on configuration variable _PublicKeyWithSignedMeterValue_.",
        //                     "type": "string",
        //                     "maxLength": 2500
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "signedMeterData",
        //                 "encodingMethod"
        //             ]
        //         },
        //         "TaxRateType": {
        //             "description": "Tax percentage",
        //             "javaType": "TaxRate",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "type": {
        //                     "description": "Type of this tax, e.g.  \"Federal \",  \"State\", for information on receipt.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "tax": {
        //                     "description": "Tax percentage",
        //                     "type": "number"
        //                 },
        //                 "stack": {
        //                     "description": "Stack level for this type of tax. Default value, when absent, is 0. +\r\n_stack_ = 0: tax on net price; +\r\n_stack_ = 1: tax added on top of _stack_ 0; +\r\n_stack_ = 2: tax added on top of _stack_ 1, etc. ",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "type",
        //                 "tax"
        //             ]
        //         },
        //         "TotalCostType": {
        //             "description": "This contains the cost calculated during a transaction. It is used both for running cost and final cost of the transaction.",
        //             "javaType": "TotalCost",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "currency": {
        //                     "description": "Currency of the costs in ISO 4217 Code.",
        //                     "type": "string",
        //                     "maxLength": 3
        //                 },
        //                 "typeOfCost": {
        //                     "$ref": "#/definitions/TariffCostEnumType"
        //                 },
        //                 "fixed": {
        //                     "$ref": "#/definitions/PriceType"
        //                 },
        //                 "energy": {
        //                     "$ref": "#/definitions/PriceType"
        //                 },
        //                 "chargingTime": {
        //                     "$ref": "#/definitions/PriceType"
        //                 },
        //                 "idleTime": {
        //                     "$ref": "#/definitions/PriceType"
        //                 },
        //                 "reservationTime": {
        //                     "$ref": "#/definitions/PriceType"
        //                 },
        //                 "reservationFixed": {
        //                     "$ref": "#/definitions/PriceType"
        //                 },
        //                 "total": {
        //                     "$ref": "#/definitions/TotalPriceType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "currency",
        //                 "typeOfCost",
        //                 "total"
        //             ]
        //         },
        //         "TotalPriceType": {
        //             "description": "Total cost with and without tax. Contains the total of energy, charging time, idle time, fixed and reservation costs including and/or excluding tax.",
        //             "javaType": "TotalPrice",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "exclTax": {
        //                     "description": "Price/cost excluding tax. Can be absent if _inclTax_ is present.",
        //                     "type": "number"
        //                 },
        //                 "inclTax": {
        //                     "description": "Price/cost including tax. Can be absent if _exclTax_ is present.",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
        //         },
        //         "TotalUsageType": {
        //             "description": "This contains the calculated usage of energy, charging time and idle time during a transaction.",
        //             "javaType": "TotalUsage",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "energy": {
        //                     "type": "number"
        //                 },
        //                 "chargingTime": {
        //                     "description": "Total duration of the charging session (including the duration of charging and not charging), in seconds.",
        //                     "type": "integer"
        //                 },
        //                 "idleTime": {
        //                     "description": "Total duration of the charging session where the EV was not charging (no energy was transferred between EVSE and EV), in seconds.",
        //                     "type": "integer"
        //                 },
        //                 "reservationTime": {
        //                     "description": "Total time of reservation in seconds.",
        //                     "type": "integer"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "energy",
        //                 "chargingTime",
        //                 "idleTime"
        //             ]
        //         },
        //         "TransactionLimitType": {
        //             "description": "Cost, energy, time or SoC limit for a transaction.",
        //             "javaType": "TransactionLimit",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "maxCost": {
        //                     "description": "Maximum allowed cost of transaction in currency of tariff.",
        //                     "type": "number"
        //                 },
        //                 "maxEnergy": {
        //                     "description": "Maximum allowed energy in Wh to charge in transaction.",
        //                     "type": "number"
        //                 },
        //                 "maxTime": {
        //                     "description": "Maximum duration of transaction in seconds from start to end.",
        //                     "type": "integer"
        //                 },
        //                 "maxSoC": {
        //                     "description": "Maximum State of Charge of EV in percentage.",
        //                     "type": "integer",
        //                     "minimum": 0.0,
        //                     "maximum": 100.0
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
        //         },
        //         "TransactionType": {
        //             "javaType": "Transaction",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "transactionId": {
        //                     "description": "This contains the Id of the transaction.",
        //                     "type": "string",
        //                     "maxLength": 36
        //                 },
        //                 "chargingState": {
        //                     "$ref": "#/definitions/ChargingStateEnumType"
        //                 },
        //                 "timeSpentCharging": {
        //                     "description": "Contains the total time that energy flowed from EVSE to EV during the transaction (in seconds). Note that timeSpentCharging is smaller or equal to the duration of the transaction.",
        //                     "type": "integer"
        //                 },
        //                 "stoppedReason": {
        //                     "$ref": "#/definitions/ReasonEnumType"
        //                 },
        //                 "remoteStartId": {
        //                     "description": "The ID given to remote start request (&lt;&lt;requeststarttransactionrequest, RequestStartTransactionRequest&gt;&gt;. This enables to CSMS to match the started transaction to the given start request.",
        //                     "type": "integer"
        //                 },
        //                 "operationMode": {
        //                     "$ref": "#/definitions/OperationModeEnumType"
        //                 },
        //                 "tariffId": {
        //                     "description": "*(2.1)* Id of tariff in use for transaction",
        //                     "type": "string",
        //                     "maxLength": 60
        //                 },
        //                 "transactionLimit": {
        //                     "$ref": "#/definitions/TransactionLimitType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "transactionId"
        //             ]
        //         },
        //         "UnitOfMeasureType": {
        //             "description": "Represents a UnitOfMeasure with a multiplier",
        //             "javaType": "UnitOfMeasure",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "unit": {
        //                     "description": "Unit of the value. Default = \"Wh\" if the (default) measurand is an \"Energy\" type.\r\nThis field SHALL use a value from the list Standardized Units of Measurements in Part 2 Appendices. \r\nIf an applicable unit is available in that list, otherwise a \"custom\" unit might be used.",
        //                     "type": "string",
        //                     "default": "Wh",
        //                     "maxLength": 20
        //                 },
        //                 "multiplier": {
        //                     "description": "Multiplier, this value represents the exponent to base 10. I.e. multiplier 3 means 10 raised to the 3rd power. Default is 0. +\r\nThe _multiplier_ only multiplies the value of the measurand. It does not specify a conversion between units, for example, kW and W.",
        //                     "type": "integer",
        //                     "default": 0
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
        //         },
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "costDetails": {
        //             "$ref": "#/definitions/CostDetailsType"
        //         },
        //         "eventType": {
        //             "$ref": "#/definitions/TransactionEventEnumType"
        //         },
        //         "meterValue": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/MeterValueType"
        //             },
        //             "minItems": 1
        //         },
        //         "timestamp": {
        //             "description": "The date and time at which this transaction event occurred.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "triggerReason": {
        //             "$ref": "#/definitions/TriggerReasonEnumType"
        //         },
        //         "seqNo": {
        //             "description": "Incremental sequence number, helps with determining if all messages of a transaction have been received.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "offline": {
        //             "description": "Indication that this transaction event happened when the Charging Station was offline. Default = false, meaning: the event occurred when the Charging Station was online.",
        //             "type": "boolean",
        //             "default": false
        //         },
        //         "numberOfPhasesUsed": {
        //             "description": "If the Charging Station is able to report the number of phases used, then it SHALL provide it.\r\nWhen omitted the CSMS may be able to determine the number of phases used as follows: +\r\n1: The numberPhases in the currently used ChargingSchedule. +\r\n2: The number of phases provided via device management.",
        //             "type": "integer",
        //             "minimum": 0.0,
        //             "maximum": 3.0
        //         },
        //         "cableMaxCurrent": {
        //             "description": "The maximum current of the connected cable in Ampere (A).",
        //             "type": "integer"
        //         },
        //         "reservationId": {
        //             "description": "This contains the Id of the reservation that terminates as a result of this transaction.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "preconditioningStatus": {
        //             "$ref": "#/definitions/PreconditioningStatusEnumType"
        //         },
        //         "evseSleep": {
        //             "description": "*(2.1)* True when EVSE electronics are in sleep mode for this transaction. Default value (when absent) is false.",
        //             "type": "boolean"
        //         },
        //         "transactionInfo": {
        //             "$ref": "#/definitions/TransactionType"
        //         },
        //         "evse": {
        //             "$ref": "#/definitions/EVSEType"
        //         },
        //         "idToken": {
        //             "$ref": "#/definitions/IdTokenType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "eventType",
        //         "timestamp",
        //         "triggerReason",
        //         "seqNo",
        //         "transactionInfo"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a transaction event request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomTransactionEventRequestParser">A delegate to parse custom transaction event requests.</param>
        public static TransactionEventRequest Parse(JObject                                                JSON,
                                                    Request_Id                                             RequestId,
                                                    SourceRouting                                          Destination,
                                                    NetworkPath                                            NetworkPath,
                                                    DateTime?                                              RequestTimestamp                      = null,
                                                    TimeSpan?                                              RequestTimeout                        = null,
                                                    EventTracking_Id?                                      EventTrackingId                       = null,
                                                    CustomJObjectParserDelegate<TransactionEventRequest>?  CustomTransactionEventRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var transactionEventRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomTransactionEventRequestParser))
            {
                return transactionEventRequest;
            }

            throw new ArgumentException("The given JSON representation of a transaction event request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out TransactionEventRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a transaction event request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="TransactionEventRequest">The parsed transaction event request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomTransactionEventRequestParser">A delegate to parse custom transaction event requests.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       Request_Id                                             RequestId,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out TransactionEventRequest?      TransactionEventRequest,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       DateTime?                                              RequestTimestamp                      = null,
                                       TimeSpan?                                              RequestTimeout                        = null,
                                       EventTracking_Id?                                      EventTrackingId                       = null,
                                       CustomJObjectParserDelegate<TransactionEventRequest>?  CustomTransactionEventRequestParser   = null)
        {

            try
            {

                TransactionEventRequest = null;

                #region EventType                [mandatory]

                if (!JSON.ParseMandatory("eventType",
                                         "event type",
                                         TransactionEventsExtensions.TryParse,
                                         out TransactionEvents EventType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timestamp                [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "timestamp",
                                         out DateTime Timestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region TriggerReason            [mandatory]

                if (!JSON.ParseMandatory("triggerReason",
                                         "trigger reason",
                                         OCPPv2_1.TriggerReason.TryParse,
                                         out TriggerReason TriggerReason,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SequenceNumber           [mandatory]

                if (!JSON.ParseMandatory("seqNo",
                                         "sequence number",
                                         out UInt32 SequenceNumber,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region TransactionInfo          [mandatory]

                if (!JSON.ParseMandatoryJSON("transactionInfo",
                                             "transaction info",
                                             OCPPv2_1.Transaction.TryParse,
                                             out Transaction? Transaction,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (Transaction is null)
                    return false;

                #endregion


                #region Offline                  [optional]

                if (JSON.ParseOptional("offline",
                                       "offline",
                                       out Boolean? Offline,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region NumberOfPhasesUsed       [optional]

                if (JSON.ParseOptional("numberOfPhasesUsed",
                                       "number of phases used",
                                       out Byte? NumberOfPhasesUsed,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CableMaxCurrent          [optional]

                if (JSON.ParseOptional("cableMaxCurrent",
                                       "cable max current",
                                       Ampere.TryParse,
                                       out Ampere? CableMaxCurrent,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ReservationId            [optional]

                if (JSON.ParseOptional("reservationId",
                                       "reservation identification",
                                       Reservation_Id.TryParse,
                                       out Reservation_Id? ReservationId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region IdToken                  [optional]

                if (JSON.ParseOptionalJSON("idToken",
                                           "identification token",
                                           OCPPv2_1.IdToken.TryParse,
                                           out IdToken? IdToken,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVSE                     [optional]

                if (JSON.ParseOptionalJSON("evse",
                                           "EVSE",
                                           OCPPv2_1.EVSE.TryParse,
                                           out EVSE? EVSE,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MeterValues              [optional]

                if (JSON.ParseOptionalHashSet("meterValue",
                                              "meter values",
                                              MeterValue.TryParse,
                                              out HashSet<MeterValue> MeterValues,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region PreconditioningStatus    [optional]

                if (JSON.ParseOptional("preconditioningStatus",
                                       "preconditioning status",
                                       OCPPv2_1.PreconditioningStatus.TryParse,
                                       out PreconditioningStatus? PreconditioningStatus,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Signatures               [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData               [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                TransactionEventRequest = new TransactionEventRequest(

                                              Destination,

                                              EventType,
                                              Timestamp,
                                              TriggerReason,
                                              SequenceNumber,
                                              Transaction,

                                              Offline,
                                              NumberOfPhasesUsed,
                                              CableMaxCurrent,
                                              ReservationId,
                                              IdToken,
                                              EVSE,
                                              MeterValues,
                                              PreconditioningStatus,

                                              null,
                                              null,
                                              Signatures,

                                              CustomData,

                                              RequestId,
                                              RequestTimestamp,
                                              RequestTimeout,
                                              EventTrackingId,
                                              NetworkPath

                                          );

                if (CustomTransactionEventRequestParser is not null)
                    TransactionEventRequest = CustomTransactionEventRequestParser(JSON,
                                                                                  TransactionEventRequest);

                return true;

            }
            catch (Exception e)
            {
                TransactionEventRequest  = null;
                ErrorResponse            = "The given JSON representation of a transaction event request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTransactionEventRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTransactionEventRequestSerializer">A delegate to serialize custom TransactionEvent requests.</param>#
        /// <param name="CustomTransactionSerializer">A delegate to serialize custom transaction objects.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomMeterValueSerializer">A delegate to serialize custom meter values.</param>
        /// <param name="CustomSampledValueSerializer">A delegate to serialize custom sampled values.</param>
        /// <param name="CustomSignedMeterValueSerializer">A delegate to serialize custom signed meter values.</param>
        /// <param name="CustomUnitsOfMeasureSerializer">A delegate to serialize custom units of measure.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                    IncludeJSONLDContext                      = false,
                              CustomJObjectSerializerDelegate<TransactionEventRequest>?  CustomTransactionEventRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Transaction>?              CustomTransactionSerializer               = null,
                              CustomJObjectSerializerDelegate<IdToken>?                  CustomIdTokenSerializer                   = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?           CustomAdditionalInfoSerializer            = null,
                              CustomJObjectSerializerDelegate<EVSE>?                     CustomEVSESerializer                      = null,
                              CustomJObjectSerializerDelegate<MeterValue>?               CustomMeterValueSerializer                = null,
                              CustomJObjectSerializerDelegate<SampledValue>?             CustomSampledValueSerializer              = null,
                              CustomJObjectSerializerDelegate<SignedMeterValue>?         CustomSignedMeterValueSerializer          = null,
                              CustomJObjectSerializerDelegate<UnitsOfMeasure>?           CustomUnitsOfMeasureSerializer            = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",                DefaultJSONLDContext.       ToString())
                               : null,

                                 new JProperty("eventType",               EventType.                  AsText()),
                                 new JProperty("timestamp",               Timestamp.                  ToISO8601()),
                                 new JProperty("triggerReason",           TriggerReason.              ToString()),
                                 new JProperty("seqNo",                   SequenceNumber),
                                 new JProperty("transactionInfo",         TransactionInfo.            ToJSON(CustomTransactionSerializer,
                                                                                                             CustomCustomDataSerializer)),

                           Offline.HasValue
                               ? new JProperty("offline",                 Offline.                    Value)
                               : null,

                           NumberOfPhasesUsed.HasValue
                               ? new JProperty("numberOfPhasesUsed",      NumberOfPhasesUsed.         Value)
                               : null,

                           CableMaxCurrent.HasValue
                               ? new JProperty("cableMaxCurrent",         CableMaxCurrent.            Value.IntegerValue)
                               : null,

                           ReservationId.HasValue
                               ? new JProperty("reservationId",           ReservationId.              Value.Value)
                               : null,

                           IdToken is not null
                               ? new JProperty("idToken",                 IdToken.                    ToJSON(CustomIdTokenSerializer,
                                                                                                             CustomAdditionalInfoSerializer,
                                                                                                             CustomCustomDataSerializer))
                               : null,

                           EVSE is not null
                               ? new JProperty("evse",                    EVSE.                       ToJSON(CustomEVSESerializer,
                                                                                                             CustomCustomDataSerializer))
                               : null,

                           MeterValues.Any()
                               ? new JProperty("meterValue",              new JArray(MeterValues.Select(meterValue => meterValue.ToJSON(CustomMeterValueSerializer,
                                                                                                                                        CustomSampledValueSerializer,
                                                                                                                                        CustomSignedMeterValueSerializer,
                                                                                                                                        CustomUnitsOfMeasureSerializer,
                                                                                                                                        CustomCustomDataSerializer))))
                               : null,

                           PreconditioningStatus.HasValue
                               ? new JProperty("preconditioningStatus",   PreconditioningStatus.Value.ToString())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",              new JArray(Signatures. Select(signature  => signature. ToJSON(CustomSignatureSerializer,
                                                                                                                                        CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",              CustomData.                 ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomTransactionEventRequestSerializer is not null
                       ? CustomTransactionEventRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (TransactionEventRequest1, TransactionEventRequest2)

        /// <summary>
        /// Compares two transaction event requests for equality.
        /// </summary>
        /// <param name="TransactionEventRequest1">A transaction event request.</param>
        /// <param name="TransactionEventRequest2">Another transaction event request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (TransactionEventRequest? TransactionEventRequest1,
                                           TransactionEventRequest? TransactionEventRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TransactionEventRequest1, TransactionEventRequest2))
                return true;

            // If one is null, but not both, return false.
            if (TransactionEventRequest1 is null || TransactionEventRequest2 is null)
                return false;

            return TransactionEventRequest1.Equals(TransactionEventRequest2);

        }

        #endregion

        #region Operator != (TransactionEventRequest1, TransactionEventRequest2)

        /// <summary>
        /// Compares two transaction event requests for inequality.
        /// </summary>
        /// <param name="TransactionEventRequest1">A transaction event request.</param>
        /// <param name="TransactionEventRequest2">Another transaction event request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (TransactionEventRequest? TransactionEventRequest1,
                                           TransactionEventRequest? TransactionEventRequest2)

            => !(TransactionEventRequest1 == TransactionEventRequest2);

        #endregion

        #endregion

        #region IEquatable<TransactionEventRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two transaction event requests for equality.
        /// </summary>
        /// <param name="Object">A transaction event request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TransactionEventRequest transactionEventRequest &&
                   Equals(transactionEventRequest);

        #endregion

        #region Equals(TransactionEventRequest)

        /// <summary>
        /// Compares two transaction event requests for equality.
        /// </summary>
        /// <param name="TransactionEventRequest">A transaction event request to compare with.</param>
        public override Boolean Equals(TransactionEventRequest? TransactionEventRequest)

            => TransactionEventRequest is not null &&

               EventType.      Equals(TransactionEventRequest.EventType)       &&
               Timestamp.      Equals(TransactionEventRequest.Timestamp)       &&
               TriggerReason.  Equals(TransactionEventRequest.TriggerReason)   &&
               SequenceNumber. Equals(TransactionEventRequest.SequenceNumber)  &&
               TransactionInfo.Equals(TransactionEventRequest.TransactionInfo) &&

            ((!Offline.              HasValue    && !TransactionEventRequest.Offline.              HasValue)    ||
               Offline.              HasValue    &&  TransactionEventRequest.Offline.              HasValue    && Offline.              Value.Equals(TransactionEventRequest.Offline.              Value)) &&

            ((!NumberOfPhasesUsed.   HasValue    && !TransactionEventRequest.NumberOfPhasesUsed.   HasValue)    ||
               NumberOfPhasesUsed.   HasValue    &&  TransactionEventRequest.NumberOfPhasesUsed.   HasValue    && NumberOfPhasesUsed.   Value.Equals(TransactionEventRequest.NumberOfPhasesUsed.   Value)) &&

            ((!CableMaxCurrent.      HasValue    && !TransactionEventRequest.CableMaxCurrent.      HasValue)    ||
               CableMaxCurrent.      HasValue    &&  TransactionEventRequest.CableMaxCurrent.      HasValue    && CableMaxCurrent.      Value.Equals(TransactionEventRequest.CableMaxCurrent.      Value)) &&

            ((!ReservationId.        HasValue    && !TransactionEventRequest.ReservationId.        HasValue)    ||
               ReservationId.        HasValue    &&  TransactionEventRequest.ReservationId.        HasValue    && ReservationId.        Value.Equals(TransactionEventRequest.ReservationId.        Value)) &&

             ((IdToken               is     null &&  TransactionEventRequest.IdToken               is     null) ||
              (IdToken               is not null &&  TransactionEventRequest.IdToken               is not null && IdToken.                    Equals(TransactionEventRequest.IdToken)))                    &&

             ((EVSE                  is     null &&  TransactionEventRequest.EVSE                  is     null) ||
              (EVSE                  is not null &&  TransactionEventRequest.EVSE                  is not null && EVSE.                       Equals(TransactionEventRequest.EVSE)))                       &&

            ((!PreconditioningStatus.HasValue    && !TransactionEventRequest.PreconditioningStatus.HasValue)    ||
               PreconditioningStatus.HasValue    &&  TransactionEventRequest.PreconditioningStatus.HasValue    && PreconditioningStatus.Value.Equals(TransactionEventRequest.PreconditioningStatus.Value)) &&

               MeterValues.Count().Equals(TransactionEventRequest.MeterValues.Count()) &&
               MeterValues.All(energyTransferMode => TransactionEventRequest.MeterValues.Contains(energyTransferMode)) &&

               base.    GenericEquals(TransactionEventRequest);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Timestamp} {EventType}, {TriggerReason}";

        #endregion

    }

}
