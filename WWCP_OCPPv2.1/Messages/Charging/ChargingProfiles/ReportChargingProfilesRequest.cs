﻿/*
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
    /// The ReportChargingProfiles request.
    /// </summary>
    public class ReportChargingProfilesRequest : ARequest<ReportChargingProfilesRequest>,
                                                 IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/reportChargingProfilesRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                 Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The request identification used to match the GetChargingProfilesRequest message with the resulting ReportChargingProfilesRequest messages.
        /// When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.
        /// </summary>
        [Mandatory]
        public Int32                         ReportChargingProfilesRequestId    { get; }

        /// <summary>
        /// The source that has installed this charging profile.
        /// </summary>
        [Mandatory]
        public ChargingLimitSource           ChargingLimitSource                { get; }

        /// <summary>
        /// The evse to which the charging profile applies.
        /// If evseId = 0, the message contains an overall limit for the charging station.
        /// </summary>
        [Mandatory]
        public EVSE_Id                       EVSEId                             { get; }

        /// <summary>
        /// The enumeration of charging profiles.
        /// </summary>
        [Mandatory]
        public IEnumerable<ChargingProfile>  ChargingProfiles                   { get; }

        /// <summary>
        /// The optional "to be continued" indicator whether another part of the charging profiles follows.
        /// Default value when omitted is false.
        /// </summary>
        [Optional]
        public Boolean?                      ToBeContinued                      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a ReportChargingProfiles request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ReportChargingProfilesRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting ReportChargingProfilesRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
        /// <param name="ChargingLimitSource">The source that has installed this charging profile.</param>
        /// <param name="EVSEId">The evse to which the charging profile applies. If evseId = 0, the message contains an overall limit for the charging station.</param>
        /// <param name="ChargingProfiles">The enumeration of charging profiles.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the charging profiles follows. Default value when omitted is false.</param>
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
        public ReportChargingProfilesRequest(SourceRouting                 Destination,
                                             Int32                         ReportChargingProfilesRequestId,
                                             ChargingLimitSource           ChargingLimitSource,
                                             EVSE_Id                       EVSEId,
                                             IEnumerable<ChargingProfile>  ChargingProfiles,
                                             Boolean?                      ToBeContinued         = null,

                                             IEnumerable<KeyPair>?         SignKeys              = null,
                                             IEnumerable<SignInfo>?        SignInfos             = null,
                                             IEnumerable<Signature>?       Signatures            = null,

                                             CustomData?                   CustomData            = null,

                                             Request_Id?                   RequestId             = null,
                                             DateTime?                     RequestTimestamp      = null,
                                             TimeSpan?                     RequestTimeout        = null,
                                             EventTracking_Id?             EventTrackingId       = null,
                                             NetworkPath?                  NetworkPath           = null,
                                             SerializationFormats?         SerializationFormat   = null,
                                             CancellationToken             CancellationToken     = default)

            : base(Destination,
                   nameof(ReportChargingProfilesRequest)[..^7],

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

            if (!ChargingProfiles.Any())
                throw new ArgumentException("The given enumeration of charging profiles must not be empty!",
                                            nameof(ChargingProfiles));

            this.ReportChargingProfilesRequestId  = ReportChargingProfilesRequestId;
            this.ChargingLimitSource              = ChargingLimitSource;
            this.EVSEId                           = EVSEId;
            this.ChargingProfiles                 = ChargingProfiles.Distinct();
            this.ToBeContinued                    = ToBeContinued;

            unchecked
            {

                hashCode = this.ReportChargingProfilesRequestId.GetHashCode()       * 17 ^
                           this.ChargingLimitSource.            GetHashCode()       * 13 ^
                           this.EVSEId.                         GetHashCode()       * 11 ^
                           this.ChargingProfiles.               CalcHashCode()      *  7 ^
                          (this.ToBeContinued?.                 GetHashCode() ?? 0) *  5 ^
                          (this.CustomData?.                    GetHashCode() ?? 0) *  3 ^
                           base.                                GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:ReportChargingProfilesRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "ChargingProfileKindEnumType": {
        //             "description": "Indicates the kind of schedule.",
        //             "javaType": "ChargingProfileKindEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Absolute",
        //                 "Recurring",
        //                 "Relative",
        //                 "Dynamic"
        //             ]
        //         },
        //         "ChargingProfilePurposeEnumType": {
        //             "description": "Defines the purpose of the schedule transferred by this profile",
        //             "javaType": "ChargingProfilePurposeEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "ChargingStationExternalConstraints",
        //                 "ChargingStationMaxProfile",
        //                 "TxDefaultProfile",
        //                 "TxProfile",
        //                 "PriorityCharging",
        //                 "LocalGeneration"
        //             ]
        //         },
        //         "ChargingRateUnitEnumType": {
        //             "description": "The unit of measure in which limits and setpoints are expressed.",
        //             "javaType": "ChargingRateUnitEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "W",
        //                 "A"
        //             ]
        //         },
        //         "CostKindEnumType": {
        //             "description": "The kind of cost referred to in the message element amount",
        //             "javaType": "CostKindEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "CarbonDioxideEmission",
        //                 "RelativePricePercentage",
        //                 "RenewableGenerationPercentage"
        //             ]
        //         },
        //         "OperationModeEnumType": {
        //             "description": "*(2.1)* Charging operation mode to use during this time interval. When absent defaults to `ChargingOnly`.",
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
        //         "RecurrencyKindEnumType": {
        //             "description": "Indicates the start point of a recurrence.",
        //             "javaType": "RecurrencyKindEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Daily",
        //                 "Weekly"
        //             ]
        //         },
        //         "AbsolutePriceScheduleType": {
        //             "description": "The AbsolutePriceScheduleType is modeled after the same type that is defined in ISO 15118-20, such that if it is supplied by an EMSP as a signed EXI message, the conversion from EXI to JSON (in OCPP) and back to EXI (for ISO 15118-20) does not change the digest and therefore does not invalidate the signature.\r\n\r\nimage::images/AbsolutePriceSchedule-Simple.png[]",
        //             "javaType": "AbsolutePriceSchedule",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "timeAnchor": {
        //                     "description": "Starting point of price schedule.",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "priceScheduleID": {
        //                     "description": "Unique ID of price schedule",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "priceScheduleDescription": {
        //                     "description": "Description of the price schedule.",
        //                     "type": "string",
        //                     "maxLength": 160
        //                 },
        //                 "currency": {
        //                     "description": "Currency according to ISO 4217.",
        //                     "type": "string",
        //                     "maxLength": 3
        //                 },
        //                 "language": {
        //                     "description": "String that indicates what language is used for the human readable strings in the price schedule. Based on ISO 639.",
        //                     "type": "string",
        //                     "maxLength": 8
        //                 },
        //                 "priceAlgorithm": {
        //                     "description": "A string in URN notation which shall uniquely identify an algorithm that defines how to compute an energy fee sum for a specific power profile based on the EnergyFee information from the PriceRule elements.",
        //                     "type": "string",
        //                     "maxLength": 2000
        //                 },
        //                 "minimumCost": {
        //                     "$ref": "#/definitions/RationalNumberType"
        //                 },
        //                 "maximumCost": {
        //                     "$ref": "#/definitions/RationalNumberType"
        //                 },
        //                 "priceRuleStacks": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/PriceRuleStackType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 1024
        //                 },
        //                 "taxRules": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/TaxRuleType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 10
        //                 },
        //                 "overstayRuleList": {
        //                     "$ref": "#/definitions/OverstayRuleListType"
        //                 },
        //                 "additionalSelectedServices": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/AdditionalSelectedServicesType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 5
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "timeAnchor",
        //                 "priceScheduleID",
        //                 "currency",
        //                 "language",
        //                 "priceAlgorithm",
        //                 "priceRuleStacks"
        //             ]
        //         },
        //         "AdditionalSelectedServicesType": {
        //             "description": "Part of ISO 15118-20 price schedule.",
        //             "javaType": "AdditionalSelectedServices",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "serviceFee": {
        //                     "$ref": "#/definitions/RationalNumberType"
        //                 },
        //                 "serviceName": {
        //                     "description": "Human readable string to identify this service.",
        //                     "type": "string",
        //                     "maxLength": 80
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "serviceName",
        //                 "serviceFee"
        //             ]
        //         },
        //         "ChargingProfileType": {
        //             "description": "A ChargingProfile consists of 1 to 3 ChargingSchedules with a list of ChargingSchedulePeriods, describing the amount of power or current that can be delivered per time interval.\r\n\r\nimage::images/ChargingProfile-Simple.png[]",
        //             "javaType": "ChargingProfile",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "id": {
        //                     "description": "Id of ChargingProfile. Unique within charging station. Id can have a negative value. This is useful to distinguish charging profiles from an external actor (external constraints) from charging profiles received from CSMS.",
        //                     "type": "integer"
        //                 },
        //                 "stackLevel": {
        //                     "description": "Value determining level in hierarchy stack of profiles. Higher values have precedence over lower values. Lowest level is 0.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "chargingProfilePurpose": {
        //                     "$ref": "#/definitions/ChargingProfilePurposeEnumType"
        //                 },
        //                 "chargingProfileKind": {
        //                     "$ref": "#/definitions/ChargingProfileKindEnumType"
        //                 },
        //                 "recurrencyKind": {
        //                     "$ref": "#/definitions/RecurrencyKindEnumType"
        //                 },
        //                 "validFrom": {
        //                     "description": "Point in time at which the profile starts to be valid. If absent, the profile is valid as soon as it is received by the Charging Station.",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "validTo": {
        //                     "description": "Point in time at which the profile stops to be valid. If absent, the profile is valid until it is replaced by another profile.",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "transactionId": {
        //                     "description": "SHALL only be included if ChargingProfilePurpose is set to TxProfile in a SetChargingProfileRequest. The transactionId is used to match the profile to a specific transaction.",
        //                     "type": "string",
        //                     "maxLength": 36
        //                 },
        //                 "maxOfflineDuration": {
        //                     "description": "*(2.1)* Period in seconds that this charging profile remains valid after the Charging Station has gone offline. After this period the charging profile becomes invalid for as long as it is offline and the Charging Station reverts back to a valid profile with a lower stack level. \r\nIf _invalidAfterOfflineDuration_ is true, then this charging profile will become permanently invalid.\r\nA value of 0 means that the charging profile is immediately invalid while offline. When the field is absent, then  no timeout applies and the charging profile remains valid when offline.",
        //                     "type": "integer"
        //                 },
        //                 "chargingSchedule": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/ChargingScheduleType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 3
        //                 },
        //                 "invalidAfterOfflineDuration": {
        //                     "description": "*(2.1)* When set to true this charging profile will not be valid anymore after being offline for more than _maxOfflineDuration_. +\r\n    When absent defaults to false.",
        //                     "type": "boolean"
        //                 },
        //                 "dynUpdateInterval": {
        //                     "description": "*(2.1)*  Interval in seconds after receipt of last update, when to request a profile update by sending a PullDynamicScheduleUpdateRequest message.\r\n    A value of 0 or no value means that no update interval applies. +\r\n    Only relevant in a dynamic charging profile.",
        //                     "type": "integer"
        //                 },
        //                 "dynUpdateTime": {
        //                     "description": "*(2.1)* Time at which limits or setpoints in this charging profile were last updated by a PullDynamicScheduleUpdateRequest or UpdateDynamicScheduleRequest or by an external actor. +\r\n    Only relevant in a dynamic charging profile.",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "priceScheduleSignature": {
        //                     "description": "*(2.1)* ISO 15118-20 signature for all price schedules in _chargingSchedules_. +\r\nNote: for 256-bit elliptic curves (like secp256k1) the ECDSA signature is 512 bits (64 bytes) and for 521-bit curves (like secp521r1) the signature is 1042 bits. This equals 131 bytes, which can be encoded as base64 in 176 bytes.",
        //                     "type": "string",
        //                     "maxLength": 256
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "id",
        //                 "stackLevel",
        //                 "chargingProfilePurpose",
        //                 "chargingProfileKind",
        //                 "chargingSchedule"
        //             ]
        //         },
        //         "ChargingSchedulePeriodType": {
        //             "description": "Charging schedule period structure defines a time period in a charging schedule. It is used in: CompositeScheduleType and in ChargingScheduleType. When used in a NotifyEVChargingScheduleRequest only _startPeriod_, _limit_, _limit_L2_, _limit_L3_ are relevant.",
        //             "javaType": "ChargingSchedulePeriod",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "startPeriod": {
        //                     "description": "Start of the period, in seconds from the start of schedule. The value of StartPeriod also defines the stop time of the previous period.",
        //                     "type": "integer"
        //                 },
        //                 "limit": {
        //                     "description": "Optional only when not required by the _operationMode_, as in CentralSetpoint, ExternalSetpoint, ExternalLimits, LocalFrequency,  LocalLoadBalancing. +\r\nCharging rate limit during the schedule period, in the applicable _chargingRateUnit_. \r\nThis SHOULD be a non-negative value; a negative value is only supported for backwards compatibility with older systems that use a negative value to specify a discharging limit.\r\nWhen using _chargingRateUnit_ = `W`, this field represents the sum of the power of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1.",
        //                     "type": "number"
        //                 },
        //                 "limit_L2": {
        //                     "description": "*(2.1)* Charging rate limit on phase L2  in the applicable _chargingRateUnit_. ",
        //                     "type": "number"
        //                 },
        //                 "limit_L3": {
        //                     "description": "*(2.1)* Charging rate limit on phase L3  in the applicable _chargingRateUnit_. ",
        //                     "type": "number"
        //                 },
        //                 "numberPhases": {
        //                     "description": "The number of phases that can be used for charging. +\r\nFor a DC EVSE this field should be omitted. +\r\nFor an AC EVSE a default value of _numberPhases_ = 3 will be assumed if the field is absent.",
        //                     "type": "integer",
        //                     "minimum": 0.0,
        //                     "maximum": 3.0
        //                 },
        //                 "phaseToUse": {
        //                     "description": "Values: 1..3, Used if numberPhases=1 and if the EVSE is capable of switching the phase connected to the EV, i.e. ACPhaseSwitchingSupported is defined and true. It\u2019s not allowed unless both conditions above are true. If both conditions are true, and phaseToUse is omitted, the Charging Station / EVSE will make the selection on its own.",
        //                     "type": "integer",
        //                     "minimum": 0.0,
        //                     "maximum": 3.0
        //                 },
        //                 "dischargeLimit": {
        //                     "description": "*(2.1)* Limit in _chargingRateUnit_ that the EV is allowed to discharge with. Note, these are negative values in order to be consistent with _setpoint_, which can be positive and negative.  +\r\nFor AC this field represents the sum of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1.",
        //                     "type": "number",
        //                     "maximum": 0.0
        //                 },
        //                 "dischargeLimit_L2": {
        //                     "description": "*(2.1)* Limit in _chargingRateUnit_ on phase L2 that the EV is allowed to discharge with. ",
        //                     "type": "number",
        //                     "maximum": 0.0
        //                 },
        //                 "dischargeLimit_L3": {
        //                     "description": "*(2.1)* Limit in _chargingRateUnit_ on phase L3 that the EV is allowed to discharge with. ",
        //                     "type": "number",
        //                     "maximum": 0.0
        //                 },
        //                 "setpoint": {
        //                     "description": "*(2.1)* Setpoint in _chargingRateUnit_ that the EV should follow as close as possible. Use negative values for discharging. +\r\nWhen a limit and/or _dischargeLimit_ are given the overshoot when following _setpoint_ must remain within these values.\r\nThis field represents the sum of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1.",
        //                     "type": "number"
        //                 },
        //                 "setpoint_L2": {
        //                     "description": "*(2.1)* Setpoint in _chargingRateUnit_ that the EV should follow on phase L2 as close as possible.",
        //                     "type": "number"
        //                 },
        //                 "setpoint_L3": {
        //                     "description": "*(2.1)* Setpoint in _chargingRateUnit_ that the EV should follow on phase L3 as close as possible. ",
        //                     "type": "number"
        //                 },
        //                 "setpointReactive": {
        //                     "description": "*(2.1)* Setpoint for reactive power (or current) in _chargingRateUnit_ that the EV should follow as closely as possible. Positive values for inductive, negative for capacitive reactive power or current.  +\r\nThis field represents the sum of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1.",
        //                     "type": "number"
        //                 },
        //                 "setpointReactive_L2": {
        //                     "description": "*(2.1)* Setpoint for reactive power (or current) in _chargingRateUnit_ that the EV should follow on phase L2 as closely as possible. ",
        //                     "type": "number"
        //                 },
        //                 "setpointReactive_L3": {
        //                     "description": "*(2.1)* Setpoint for reactive power (or current) in _chargingRateUnit_ that the EV should follow on phase L3 as closely as possible. ",
        //                     "type": "number"
        //                 },
        //                 "preconditioningRequest": {
        //                     "description": "*(2.1)* If  true, the EV should attempt to keep the BMS preconditioned for this time interval.",
        //                     "type": "boolean"
        //                 },
        //                 "evseSleep": {
        //                     "description": "*(2.1)* If true, the EVSE must turn off power electronics/modules associated with this transaction. Default value when absent is false.",
        //                     "type": "boolean"
        //                 },
        //                 "v2xBaseline": {
        //                     "description": "*(2.1)* Power value that, when present, is used as a baseline on top of which values from _v2xFreqWattCurve_ and _v2xSignalWattCurve_ are added.",
        //                     "type": "number"
        //                 },
        //                 "operationMode": {
        //                     "$ref": "#/definitions/OperationModeEnumType"
        //                 },
        //                 "v2xFreqWattCurve": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/V2XFreqWattPointType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 20
        //                 },
        //                 "v2xSignalWattCurve": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/V2XSignalWattPointType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 20
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "startPeriod"
        //             ]
        //         },
        //         "ChargingScheduleType": {
        //             "description": "Charging schedule structure defines a list of charging periods, as used in: NotifyEVChargingScheduleRequest and ChargingProfileType. When used in a NotifyEVChargingScheduleRequest only _duration_ and _chargingSchedulePeriod_ are relevant and _chargingRateUnit_ must be 'W'. +\r\nAn ISO 15118-20 session may provide either an _absolutePriceSchedule_ or a _priceLevelSchedule_. An ISO 15118-2 session can only provide a_salesTariff_ element. The field _digestValue_ is used when price schedule or sales tariff are signed.\r\n\r\nimage::images/ChargingSchedule-Simple.png[]",
        //             "javaType": "ChargingSchedule",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "id": {
        //                     "type": "integer"
        //                 },
        //                 "limitAtSoC": {
        //                     "$ref": "#/definitions/LimitAtSoCType"
        //                 },
        //                 "startSchedule": {
        //                     "description": "Starting point of an absolute schedule or recurring schedule.",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "duration": {
        //                     "description": "Duration of the charging schedule in seconds. If the duration is left empty, the last period will continue indefinitely or until end of the transaction in case startSchedule is absent.",
        //                     "type": "integer"
        //                 },
        //                 "chargingRateUnit": {
        //                     "$ref": "#/definitions/ChargingRateUnitEnumType"
        //                 },
        //                 "minChargingRate": {
        //                     "description": "Minimum charging rate supported by the EV. The unit of measure is defined by the chargingRateUnit. This parameter is intended to be used by a local smart charging algorithm to optimize the power allocation for in the case a charging process is inefficient at lower charging rates. ",
        //                     "type": "number"
        //                 },
        //                 "powerTolerance": {
        //                     "description": "*(2.1)* Power tolerance when following EVPowerProfile.",
        //                     "type": "number"
        //                 },
        //                 "signatureId": {
        //                     "description": "*(2.1)* Id of this element for referencing in a signature.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "digestValue": {
        //                     "description": "*(2.1)* Base64 encoded hash (SHA256 for ISO 15118-2, SHA512 for ISO 15118-20) of the EXI price schedule element. Used in signature.",
        //                     "type": "string",
        //                     "maxLength": 88
        //                 },
        //                 "useLocalTime": {
        //                     "description": "*(2.1)* Defaults to false. When true, disregard time zone offset in dateTime fields of  _ChargingScheduleType_ and use unqualified local time at Charging Station instead.\r\n This allows the same `Absolute` or `Recurring` charging profile to be used in both summer and winter time.",
        //                     "type": "boolean"
        //                 },
        //                 "chargingSchedulePeriod": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/ChargingSchedulePeriodType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 1024
        //                 },
        //                 "randomizedDelay": {
        //                     "description": "*(2.1)* Defaults to 0. When _randomizedDelay_ not equals zero, then the start of each &lt;&lt;cmn_chargingscheduleperiodtype,ChargingSchedulePeriodType&gt;&gt; is delayed by a randomly chosen number of seconds between 0 and _randomizedDelay_.  Only allowed for TxProfile and TxDefaultProfile.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "salesTariff": {
        //                     "$ref": "#/definitions/SalesTariffType"
        //                 },
        //                 "absolutePriceSchedule": {
        //                     "$ref": "#/definitions/AbsolutePriceScheduleType"
        //                 },
        //                 "priceLevelSchedule": {
        //                     "$ref": "#/definitions/PriceLevelScheduleType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "id",
        //                 "chargingRateUnit",
        //                 "chargingSchedulePeriod"
        //             ]
        //         },
        //         "ConsumptionCostType": {
        //             "javaType": "ConsumptionCost",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "startValue": {
        //                     "description": "The lowest level of consumption that defines the starting point of this consumption block. The block interval extends to the start of the next interval.",
        //                     "type": "number"
        //                 },
        //                 "cost": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/CostType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 3
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "startValue",
        //                 "cost"
        //             ]
        //         },
        //         "CostType": {
        //             "javaType": "Cost",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "costKind": {
        //                     "$ref": "#/definitions/CostKindEnumType"
        //                 },
        //                 "amount": {
        //                     "description": "The estimated or actual cost per kWh",
        //                     "type": "integer"
        //                 },
        //                 "amountMultiplier": {
        //                     "description": "Values: -3..3, The amountMultiplier defines the exponent to base 10 (dec). The final value is determined by: amount * 10 ^ amountMultiplier",
        //                     "type": "integer"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "costKind",
        //                 "amount"
        //             ]
        //         },
        //         "LimitAtSoCType": {
        //             "javaType": "LimitAtSoC",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "soc": {
        //                     "description": "The SoC value beyond which the charging rate limit should be applied.",
        //                     "type": "integer",
        //                     "minimum": 0.0,
        //                     "maximum": 100.0
        //                 },
        //                 "limit": {
        //                     "description": "Charging rate limit beyond the SoC value.\r\nThe unit is defined by _chargingSchedule.chargingRateUnit_.",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "soc",
        //                 "limit"
        //             ]
        //         },
        //         "OverstayRuleListType": {
        //             "description": "Part of ISO 15118-20 price schedule.",
        //             "javaType": "OverstayRuleList",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "overstayPowerThreshold": {
        //                     "$ref": "#/definitions/RationalNumberType"
        //                 },
        //                 "overstayRule": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/OverstayRuleType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 5
        //                 },
        //                 "overstayTimeThreshold": {
        //                     "description": "Time till overstay is applied in seconds.",
        //                     "type": "integer"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "overstayRule"
        //             ]
        //         },
        //         "OverstayRuleType": {
        //             "description": "Part of ISO 15118-20 price schedule.",
        //             "javaType": "OverstayRule",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "overstayFee": {
        //                     "$ref": "#/definitions/RationalNumberType"
        //                 },
        //                 "overstayRuleDescription": {
        //                     "description": "Human readable string to identify the overstay rule.",
        //                     "type": "string",
        //                     "maxLength": 32
        //                 },
        //                 "startTime": {
        //                     "description": "Time in seconds after trigger of the parent Overstay Rules for this particular fee to apply.",
        //                     "type": "integer"
        //                 },
        //                 "overstayFeePeriod": {
        //                     "description": "Time till overstay will be reapplied",
        //                     "type": "integer"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "startTime",
        //                 "overstayFeePeriod",
        //                 "overstayFee"
        //             ]
        //         },
        //         "PriceLevelScheduleEntryType": {
        //             "description": "Part of ISO 15118-20 price schedule.",
        //             "javaType": "PriceLevelScheduleEntry",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "duration": {
        //                     "description": "The amount of seconds that define the duration of this given PriceLevelScheduleEntry.",
        //                     "type": "integer"
        //                 },
        //                 "priceLevel": {
        //                     "description": "Defines the price level of this PriceLevelScheduleEntry (referring to NumberOfPriceLevels). Small values for the PriceLevel represent a cheaper PriceLevelScheduleEntry. Large values for the PriceLevel represent a more expensive PriceLevelScheduleEntry.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "duration",
        //                 "priceLevel"
        //             ]
        //         },
        //         "PriceLevelScheduleType": {
        //             "description": "The PriceLevelScheduleType is modeled after the same type that is defined in ISO 15118-20, such that if it is supplied by an EMSP as a signed EXI message, the conversion from EXI to JSON (in OCPP) and back to EXI (for ISO 15118-20) does not change the digest and therefore does not invalidate the signature.",
        //             "javaType": "PriceLevelSchedule",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "priceLevelScheduleEntries": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/PriceLevelScheduleEntryType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 100
        //                 },
        //                 "timeAnchor": {
        //                     "description": "Starting point of this price schedule.",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "priceScheduleId": {
        //                     "description": "Unique ID of this price schedule.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "priceScheduleDescription": {
        //                     "description": "Description of the price schedule.",
        //                     "type": "string",
        //                     "maxLength": 32
        //                 },
        //                 "numberOfPriceLevels": {
        //                     "description": "Defines the overall number of distinct price level elements used across all PriceLevelSchedules.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "timeAnchor",
        //                 "priceScheduleId",
        //                 "numberOfPriceLevels",
        //                 "priceLevelScheduleEntries"
        //             ]
        //         },
        //         "PriceRuleStackType": {
        //             "description": "Part of ISO 15118-20 price schedule.",
        //             "javaType": "PriceRuleStack",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "duration": {
        //                     "description": "Duration of the stack of price rules.  he amount of seconds that define the duration of the given PriceRule(s).",
        //                     "type": "integer"
        //                 },
        //                 "priceRule": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/PriceRuleType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 8
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "duration",
        //                 "priceRule"
        //             ]
        //         },
        //         "PriceRuleType": {
        //             "description": "Part of ISO 15118-20 price schedule.",
        //             "javaType": "PriceRule",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "parkingFeePeriod": {
        //                     "description": "The duration of the parking fee period (in seconds).\r\nWhen the time enters into a ParkingFeePeriod, the ParkingFee will apply to the session. .",
        //                     "type": "integer"
        //                 },
        //                 "carbonDioxideEmission": {
        //                     "description": "Number of grams of CO2 per kWh.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "renewableGenerationPercentage": {
        //                     "description": "Percentage of the power that is created by renewable resources.",
        //                     "type": "integer",
        //                     "minimum": 0.0,
        //                     "maximum": 100.0
        //                 },
        //                 "energyFee": {
        //                     "$ref": "#/definitions/RationalNumberType"
        //                 },
        //                 "parkingFee": {
        //                     "$ref": "#/definitions/RationalNumberType"
        //                 },
        //                 "powerRangeStart": {
        //                     "$ref": "#/definitions/RationalNumberType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "energyFee",
        //                 "powerRangeStart"
        //             ]
        //         },
        //         "RationalNumberType": {
        //             "description": "Part of ISO 15118-20 price schedule.",
        //             "javaType": "RationalNumber",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "exponent": {
        //                     "description": "The exponent to base 10 (dec)",
        //                     "type": "integer"
        //                 },
        //                 "value": {
        //                     "description": "Value which shall be multiplied.",
        //                     "type": "integer"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "exponent",
        //                 "value"
        //             ]
        //         },
        //         "RelativeTimeIntervalType": {
        //             "javaType": "RelativeTimeInterval",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "start": {
        //                     "description": "Start of the interval, in seconds from NOW.",
        //                     "type": "integer"
        //                 },
        //                 "duration": {
        //                     "description": "Duration of the interval, in seconds.",
        //                     "type": "integer"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "start"
        //             ]
        //         },
        //         "SalesTariffEntryType": {
        //             "javaType": "SalesTariffEntry",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "relativeTimeInterval": {
        //                     "$ref": "#/definitions/RelativeTimeIntervalType"
        //                 },
        //                 "ePriceLevel": {
        //                     "description": "Defines the price level of this SalesTariffEntry (referring to NumEPriceLevels). Small values for the EPriceLevel represent a cheaper TariffEntry. Large values for the EPriceLevel represent a more expensive TariffEntry.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "consumptionCost": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/ConsumptionCostType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 3
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "relativeTimeInterval"
        //             ]
        //         },
        //         "SalesTariffType": {
        //             "description": "A SalesTariff provided by a Mobility Operator (EMSP) .\r\nNOTE: This dataType is based on dataTypes from &lt;&lt;ref-ISOIEC15118-2,ISO 15118-2&gt;&gt;.",
        //             "javaType": "SalesTariff",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "id": {
        //                     "description": "SalesTariff identifier used to identify one sales tariff. An SAID remains a unique identifier for one schedule throughout a charging session.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "salesTariffDescription": {
        //                     "description": "A human readable title/short description of the sales tariff e.g. for HMI display purposes.",
        //                     "type": "string",
        //                     "maxLength": 32
        //                 },
        //                 "numEPriceLevels": {
        //                     "description": "Defines the overall number of distinct price levels used across all provided SalesTariff elements.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "salesTariffEntry": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/SalesTariffEntryType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 1024
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "id",
        //                 "salesTariffEntry"
        //             ]
        //         },
        //         "TaxRuleType": {
        //             "description": "Part of ISO 15118-20 price schedule.",
        //             "javaType": "TaxRule",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "taxRuleID": {
        //                     "description": "Id for the tax rule.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "taxRuleName": {
        //                     "description": "Human readable string to identify the tax rule.",
        //                     "type": "string",
        //                     "maxLength": 100
        //                 },
        //                 "taxIncludedInPrice": {
        //                     "description": "Indicates whether the tax is included in any price or not.",
        //                     "type": "boolean"
        //                 },
        //                 "appliesToEnergyFee": {
        //                     "description": "Indicates whether this tax applies to Energy Fees.",
        //                     "type": "boolean"
        //                 },
        //                 "appliesToParkingFee": {
        //                     "description": "Indicates whether this tax applies to Parking Fees.",
        //                     "type": "boolean"
        //                 },
        //                 "appliesToOverstayFee": {
        //                     "description": "Indicates whether this tax applies to Overstay Fees.",
        //                     "type": "boolean"
        //                 },
        //                 "appliesToMinimumMaximumCost": {
        //                     "description": "Indicates whether this tax applies to Minimum/Maximum Cost.",
        //                     "type": "boolean"
        //                 },
        //                 "taxRate": {
        //                     "$ref": "#/definitions/RationalNumberType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "taxRuleID",
        //                 "appliesToEnergyFee",
        //                 "appliesToParkingFee",
        //                 "appliesToOverstayFee",
        //                 "appliesToMinimumMaximumCost",
        //                 "taxRate"
        //             ]
        //         },
        //         "V2XFreqWattPointType": {
        //             "description": "*(2.1)* A point of a frequency-watt curve.",
        //             "javaType": "V2XFreqWattPoint",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "frequency": {
        //                     "description": "Net frequency in Hz.",
        //                     "type": "number"
        //                 },
        //                 "power": {
        //                     "description": "Power in W to charge (positive) or discharge (negative) at specified frequency.",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "frequency",
        //                 "power"
        //             ]
        //         },
        //         "V2XSignalWattPointType": {
        //             "description": "*(2.1)* A point of a signal-watt curve.",
        //             "javaType": "V2XSignalWattPoint",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "signal": {
        //                     "description": "Signal value from an AFRRSignalRequest.",
        //                     "type": "integer"
        //                 },
        //                 "power": {
        //                     "description": "Power in W to charge (positive) or discharge (negative) at specified frequency.",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "signal",
        //                 "power"
        //             ]
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
        //         "requestId": {
        //             "description": "Id used to match the &lt;&lt;getchargingprofilesrequest, GetChargingProfilesRequest&gt;&gt; message with the resulting ReportChargingProfilesRequest messages. When the CSMS provided a requestId in the &lt;&lt;getchargingprofilesrequest, GetChargingProfilesRequest&gt;&gt;, this field SHALL contain the same value.",
        //             "type": "integer"
        //         },
        //         "chargingLimitSource": {
        //             "description": "Source that has installed this charging profile. Values defined in Appendix as ChargingLimitSourceEnumStringType.",
        //             "type": "string",
        //             "maxLength": 20
        //         },
        //         "chargingProfile": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/ChargingProfileType"
        //             },
        //             "minItems": 1
        //         },
        //         "tbc": {
        //             "description": "To Be Continued. Default value when omitted: false. false indicates that there are no further messages as part of this report.",
        //             "type": "boolean",
        //             "default": false
        //         },
        //         "evseId": {
        //             "description": "The evse to which the charging profile applies. If evseId = 0, the message contains an overall limit for the Charging Station.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "requestId",
        //         "chargingLimitSource",
        //         "evseId",
        //         "chargingProfile"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a ReportChargingProfiles request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomReportChargingProfilesRequestParser">A delegate to parse custom ReportChargingProfiles requests.</param>
        public static ReportChargingProfilesRequest Parse(JObject                                                      JSON,
                                                          Request_Id                                                   RequestId,
                                                          SourceRouting                                                Destination,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTime?                                                    RequestTimestamp                            = null,
                                                          TimeSpan?                                                    RequestTimeout                              = null,
                                                          EventTracking_Id?                                            EventTrackingId                             = null,
                                                          CustomJObjectParserDelegate<ReportChargingProfilesRequest>?  CustomReportChargingProfilesRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var reportChargingProfilesRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomReportChargingProfilesRequestParser))
            {
                return reportChargingProfilesRequest;
            }

            throw new ArgumentException("The given JSON representation of a ReportChargingProfiles request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out ReportChargingProfilesRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a ReportChargingProfiles request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="ReportChargingProfilesRequest">The parsed ReportChargingProfiles request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomReportChargingProfilesRequestParser">A delegate to parse custom ReportChargingProfiles requests.</param>
        public static Boolean TryParse(JObject                                                      JSON,
                                       Request_Id                                                   RequestId,
                                       SourceRouting                                                Destination,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out ReportChargingProfilesRequest?      ReportChargingProfilesRequest,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTime?                                                    RequestTimestamp                            = null,
                                       TimeSpan?                                                    RequestTimeout                              = null,
                                       EventTracking_Id?                                            EventTrackingId                             = null,
                                       CustomJObjectParserDelegate<ReportChargingProfilesRequest>?  CustomReportChargingProfilesRequestParser   = null)
        {

            try
            {

                ReportChargingProfilesRequest = null;

                #region ReportChargingProfilesRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "ReportChargingProfiles request identification",
                                         out Int32 ReportChargingProfilesRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingLimitSource                [mandatory]

                if (!JSON.ParseMandatory("chargingLimitSource",
                                         "charging limit source",
                                         OCPPv2_1.ChargingLimitSource.TryParse,
                                         out ChargingLimitSource ChargingLimitSource,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVSEId                             [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "evse identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingProfiles                   [mandatory]

                if (!JSON.ParseMandatoryHashSet("chargingProfile",
                                                "charging profiles",
                                                ChargingProfile.TryParse,
                                                out HashSet<ChargingProfile> ChargingProfiles,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ToBeContinued                      [optional]

                if (JSON.ParseOptional("tbc",
                                       "to be continued",
                                       out Boolean? ToBeContinued,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                         [optional, OCPP_CSE]

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

                #region CustomData                         [optional]

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


                ReportChargingProfilesRequest = new ReportChargingProfilesRequest(

                                                    Destination,
                                                    ReportChargingProfilesRequestId,
                                                    ChargingLimitSource,
                                                    EVSEId,
                                                    ChargingProfiles,
                                                    ToBeContinued,

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

                if (CustomReportChargingProfilesRequestParser is not null)
                    ReportChargingProfilesRequest = CustomReportChargingProfilesRequestParser(JSON,
                                                                                              ReportChargingProfilesRequest);

                return true;

            }
            catch (Exception e)
            {
                ReportChargingProfilesRequest  = null;
                ErrorResponse                  = "The given JSON representation of a ReportChargingProfiles request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomReportChargingProfilesRequestSerializer = null, CustomChargingProfileSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomReportChargingProfilesRequestSerializer">A delegate to serialize custom ReportChargingProfiles requests.</param>
        /// <param name="CustomChargingProfileSerializer">A delegate to serialize custom charging profiles.</param>
        /// <param name="CustomLimitAtSoCSerializer">A delegate to serialize custom charging schedules.</param>
        /// <param name="CustomChargingScheduleSerializer">A delegate to serialize custom charging schedules.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        /// <param name="CustomV2XFreqWattEntrySerializer">A delegate to serialize custom V2X Frequency-Watt entrys.</param>
        /// <param name="CustomV2XSignalWattEntrySerializer">A delegate to serialize custom V2X Signal-Watt entrys.</param>
        /// <param name="CustomSalesTariffSerializer">A delegate to serialize custom sales tariffs.</param>
        /// <param name="CustomSalesTariffEntrySerializer">A delegate to serialize custom sales tariff entries.</param>
        /// <param name="CustomRelativeTimeIntervalSerializer">A delegate to serialize custom relative time intervals.</param>
        /// <param name="CustomConsumptionCostSerializer">A delegate to serialize custom consumption costs.</param>
        /// <param name="CustomCostSerializer">A delegate to serialize custom costs.</param>
        /// 
        /// <param name="CustomAbsolutePriceScheduleSerializer">A delegate to serialize custom absolute price schedules.</param>
        /// <param name="CustomPriceRuleStackSerializer">A delegate to serialize custom price rule stacks.</param>
        /// <param name="CustomPriceRuleSerializer">A delegate to serialize custom price rules.</param>
        /// <param name="CustomTaxRuleSerializer">A delegate to serialize custom tax rules.</param>
        /// <param name="CustomOverstayRuleListSerializer">A delegate to serialize custom overstay rule lists.</param>
        /// <param name="CustomOverstayRuleSerializer">A delegate to serialize custom overstay rules.</param>
        /// <param name="CustomAdditionalServiceSerializer">A delegate to serialize custom additional services.</param>
        /// 
        /// <param name="CustomPriceLevelScheduleSerializer">A delegate to serialize custom price level schedules.</param>
        /// <param name="CustomPriceLevelScheduleEntrySerializer">A delegate to serialize custom price level schedule entries.</param>
        /// 
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                                               IncludeJSONLDContext                            = false,
                              CustomJObjectSerializerDelegate<ReportChargingProfilesRequest>?                       CustomReportChargingProfilesRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingProfile>?                                     CustomChargingProfileSerializer                 = null,
                              CustomJObjectSerializerDelegate<LimitAtSoC>?                                      CustomLimitAtSoCSerializer                  = null,
                              CustomJObjectSerializerDelegate<ChargingSchedule>?                                    CustomChargingScheduleSerializer                = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                              CustomChargingSchedulePeriodSerializer          = null,
                              CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                                    CustomV2XFreqWattEntrySerializer                = null,
                              CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                                  CustomV2XSignalWattEntrySerializer              = null,
                              CustomJObjectSerializerDelegate<SalesTariff>?                                         CustomSalesTariffSerializer                     = null,
                              CustomJObjectSerializerDelegate<SalesTariffEntry>?                                    CustomSalesTariffEntrySerializer                = null,
                              CustomJObjectSerializerDelegate<RelativeTimeInterval>?                                CustomRelativeTimeIntervalSerializer            = null,
                              CustomJObjectSerializerDelegate<ConsumptionCost>?                                     CustomConsumptionCostSerializer                 = null,
                              CustomJObjectSerializerDelegate<Cost>?                                                CustomCostSerializer                            = null,

                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AbsolutePriceSchedule>?    CustomAbsolutePriceScheduleSerializer           = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRuleStack>?           CustomPriceRuleStackSerializer                  = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRule>?                CustomPriceRuleSerializer                       = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.TaxRule>?                  CustomTaxRuleSerializer                         = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRuleList>?         CustomOverstayRuleListSerializer                = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRule>?             CustomOverstayRuleSerializer                    = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AdditionalSelectedService>?        CustomAdditionalServiceSerializer               = null,

                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelSchedule>?       CustomPriceLevelScheduleSerializer              = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntrySerializer         = null,

                              CustomJObjectSerializerDelegate<Signature>?                                           CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",              DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("requestId",             ReportChargingProfilesRequestId),
                                 new JProperty("chargingLimitSource",   ChargingLimitSource. ToString()),
                                 new JProperty("evseId",                EVSEId.Value),

                                 new JProperty("chargingProfile",       new JArray(ChargingProfiles.Select(chargingProfile => chargingProfile.ToJSON(CustomChargingProfileSerializer,
                                                                                                                                                     CustomLimitAtSoCSerializer,
                                                                                                                                                     CustomChargingScheduleSerializer,
                                                                                                                                                     CustomChargingSchedulePeriodSerializer,
                                                                                                                                                     CustomV2XFreqWattEntrySerializer,
                                                                                                                                                     CustomV2XSignalWattEntrySerializer,
                                                                                                                                                     CustomSalesTariffSerializer,
                                                                                                                                                     CustomSalesTariffEntrySerializer,
                                                                                                                                                     CustomRelativeTimeIntervalSerializer,
                                                                                                                                                     CustomConsumptionCostSerializer,
                                                                                                                                                     CustomCostSerializer,

                                                                                                                                                     CustomAbsolutePriceScheduleSerializer,
                                                                                                                                                     CustomPriceRuleStackSerializer,
                                                                                                                                                     CustomPriceRuleSerializer,
                                                                                                                                                     CustomTaxRuleSerializer,
                                                                                                                                                     CustomOverstayRuleListSerializer,
                                                                                                                                                     CustomOverstayRuleSerializer,
                                                                                                                                                     CustomAdditionalServiceSerializer,

                                                                                                                                                     CustomPriceLevelScheduleSerializer,
                                                                                                                                                     CustomPriceLevelScheduleEntrySerializer,                                     

                                                                                                                                                     CustomCustomDataSerializer)))),

                           ToBeContinued.HasValue
                               ? new JProperty("tbc",                   ToBeContinued.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",            new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                   CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",            CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomReportChargingProfilesRequestSerializer is not null
                       ? CustomReportChargingProfilesRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ReportChargingProfilesRequest1, ReportChargingProfilesRequest2)

        /// <summary>
        /// Compares two ReportChargingProfiles requests for equality.
        /// </summary>
        /// <param name="ReportChargingProfilesRequest1">A ReportChargingProfiles request.</param>
        /// <param name="ReportChargingProfilesRequest2">Another ReportChargingProfiles request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ReportChargingProfilesRequest? ReportChargingProfilesRequest1,
                                           ReportChargingProfilesRequest? ReportChargingProfilesRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ReportChargingProfilesRequest1, ReportChargingProfilesRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ReportChargingProfilesRequest1 is null || ReportChargingProfilesRequest2 is null)
                return false;

            return ReportChargingProfilesRequest1.Equals(ReportChargingProfilesRequest2);

        }

        #endregion

        #region Operator != (ReportChargingProfilesRequest1, ReportChargingProfilesRequest2)

        /// <summary>
        /// Compares two ReportChargingProfiles requests for inequality.
        /// </summary>
        /// <param name="ReportChargingProfilesRequest1">A ReportChargingProfiles request.</param>
        /// <param name="ReportChargingProfilesRequest2">Another ReportChargingProfiles request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ReportChargingProfilesRequest? ReportChargingProfilesRequest1,
                                           ReportChargingProfilesRequest? ReportChargingProfilesRequest2)

            => !(ReportChargingProfilesRequest1 == ReportChargingProfilesRequest2);

        #endregion

        #endregion

        #region IEquatable<ReportChargingProfilesRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ReportChargingProfiles requests for equality.
        /// </summary>
        /// <param name="Object">A ReportChargingProfiles request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ReportChargingProfilesRequest reportChargingProfilesRequest &&
                   Equals(reportChargingProfilesRequest);

        #endregion

        #region Equals(ReportChargingProfilesRequest)

        /// <summary>
        /// Compares two ReportChargingProfiles requests for equality.
        /// </summary>
        /// <param name="ReportChargingProfilesRequest">A ReportChargingProfiles request to compare with.</param>
        public override Boolean Equals(ReportChargingProfilesRequest? ReportChargingProfilesRequest)

            => ReportChargingProfilesRequest is not null &&

               ReportChargingProfilesRequestId.Equals(ReportChargingProfilesRequest.ReportChargingProfilesRequestId) &&
               ChargingLimitSource.            Equals(ReportChargingProfilesRequest.ChargingLimitSource)             &&
               EVSEId.                         Equals(ReportChargingProfilesRequest.EVSEId)                          &&

               ChargingProfiles.Count().Equals(ReportChargingProfilesRequest.ChargingProfiles.Count())     &&
               ChargingProfiles.All(data => ReportChargingProfilesRequest.ChargingProfiles.Contains(data)) &&

            ((!ToBeContinued.HasValue && !ReportChargingProfilesRequest.ToBeContinued.HasValue) ||
               ToBeContinued.HasValue &&  ReportChargingProfilesRequest.ToBeContinued.HasValue && ToBeContinued.Value.Equals(ReportChargingProfilesRequest.ToBeContinued.Value)) &&

               base.GenericEquals(ReportChargingProfilesRequest);

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

            => $"{ReportChargingProfilesRequestId}, {ChargingLimitSource}, {EVSEId}, {ChargingProfiles.Count()} charging profile(s)";

        #endregion

    }

}
