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
    /// A notify EV charging needs request.
    /// </summary>
    public class NotifyEVChargingNeedsRequest : ARequest<NotifyEVChargingNeedsRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/notifyEVChargingNeedsRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The EVSE and connector to which the EV is connected to.
        /// </summary>
        [Mandatory]
        public EVSE_Id          EVSEId               { get; }

        /// <summary>
        /// The characteristics of the energy delivery required.
        /// </summary>
        [Mandatory]
        public ChargingNeeds    ChargingNeeds        { get; }

        /// <summary>
        /// An optional timestamp when the EV charging needs had been received,
        /// e.g. when the charging station was offline.
        /// </summary>
        [Optional]
        public DateTimeOffset?  ReceivedTimestamp    { get; }

        /// <summary>
        /// The optional maximum number of schedule tuples the EV supports for:
        ///   - ISO 15118-2:  Schedule tuples in SASchedule (both Pmax and Tariff)
        ///   - ISO 15118-20: PowerScheduleEntry, PriceRule and PriceLevelScheduleEntries
        /// </summary>
        [Optional]
        public UInt16?          MaxScheduleTuples    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a notify EV charging needs request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="EVSEId">The EVSE and connector to which the EV is connected to.</param>
        /// <param name="ChargingNeeds">The characteristics of the energy delivery required.</param>
        /// <param name="ReceivedTimestamp">An optional timestamp when the EV charging needs had been received, e.g. when the charging station was offline.</param>
        /// <param name="MaxScheduleTuples">An optional maximum number of schedule tuples per schedule the car supports.</param>
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
        public NotifyEVChargingNeedsRequest(SourceRouting            Destination,
                                            EVSE_Id                  EVSEId,
                                            ChargingNeeds            ChargingNeeds,
                                            DateTimeOffset?          ReceivedTimestamp     = null,
                                            UInt16?                  MaxScheduleTuples     = null,

                                            IEnumerable<KeyPair>?    SignKeys              = null,
                                            IEnumerable<SignInfo>?   SignInfos             = null,
                                            IEnumerable<Signature>?  Signatures            = null,

                                            CustomData?              CustomData            = null,

                                            Request_Id?              RequestId             = null,
                                            DateTimeOffset?          RequestTimestamp      = null,
                                            TimeSpan?                RequestTimeout        = null,
                                            EventTracking_Id?        EventTrackingId       = null,
                                            NetworkPath?             NetworkPath           = null,
                                            SerializationFormats?    SerializationFormat   = null,
                                            CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(NotifyEVChargingNeedsRequest)[..^7],

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

            this.EVSEId             = EVSEId;
            this.ChargingNeeds      = ChargingNeeds;
            this.ReceivedTimestamp  = ReceivedTimestamp;
            this.MaxScheduleTuples  = MaxScheduleTuples;


            unchecked
            {

                hashCode = this.EVSEId.            GetHashCode()       * 11 ^
                           this.ChargingNeeds.     GetHashCode()       *  7 ^
                          (this.ReceivedTimestamp?.GetHashCode() ?? 0) *  5 ^
                          (this.MaxScheduleTuples?.GetHashCode() ?? 0) *  3 ^
                           base.                   GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:NotifyEVChargingNeedsRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "ControlModeEnumType": {
        //             "description": "Indicates whether EV wants to operate in Dynamic or Scheduled mode. When absent, Scheduled mode is assumed for backwards compatibility. +\r\n*ISO 15118-20:* +\r\nServiceSelectionReq(SelectedEnergyTransferService)",
        //             "javaType": "ControlModeEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "ScheduledControl",
        //                 "DynamicControl"
        //             ]
        //         },
        //         "DERControlEnumType": {
        //             "javaType": "DERControlEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "EnterService",
        //                 "FreqDroop",
        //                 "FreqWatt",
        //                 "FixedPFAbsorb",
        //                 "FixedPFInject",
        //                 "FixedVar",
        //                 "Gradients",
        //                 "HFMustTrip",
        //                 "HFMayTrip",
        //                 "HVMustTrip",
        //                 "HVMomCess",
        //                 "HVMayTrip",
        //                 "LimitMaxDischarge",
        //                 "LFMustTrip",
        //                 "LVMustTrip",
        //                 "LVMomCess",
        //                 "LVMayTrip",
        //                 "PowerMonitoringMustTrip",
        //                 "VoltVar",
        //                 "VoltWatt",
        //                 "WattPF",
        //                 "WattVar"
        //             ]
        //         },
        //         "EnergyTransferModeEnumType": {
        //             "description": "Mode of energy transfer requested by the EV.",
        //             "javaType": "EnergyTransferModeEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "AC_single_phase",
        //                 "AC_two_phase",
        //                 "AC_three_phase",
        //                 "DC",
        //                 "AC_BPT",
        //                 "AC_BPT_DER",
        //                 "AC_DER",
        //                 "DC_BPT",
        //                 "DC_ACDP",
        //                 "DC_ACDP_BPT",
        //                 "WPT"
        //             ]
        //         },
        //         "IslandingDetectionEnumType": {
        //             "javaType": "IslandingDetectionEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "NoAntiIslandingSupport",
        //                 "RoCoF",
        //                 "UVP_OVP",
        //                 "UFP_OFP",
        //                 "VoltageVectorShift",
        //                 "ZeroCrossingDetection",
        //                 "OtherPassive",
        //                 "ImpedanceMeasurement",
        //                 "ImpedanceAtFrequency",
        //                 "SlipModeFrequencyShift",
        //                 "SandiaFrequencyShift",
        //                 "SandiaVoltageShift",
        //                 "FrequencyJump",
        //                 "RCLQFactor",
        //                 "OtherActive"
        //             ]
        //         },
        //         "MobilityNeedsModeEnumType": {
        //             "description": "Value of EVCC indicates that EV determines min/target SOC and departure time. +\r\nA value of EVCC_SECC indicates that charging station or CSMS may also update min/target SOC and departure time. +\r\n*ISO 15118-20:* +\r\nServiceSelectionReq(SelectedEnergyTransferService)",
        //             "javaType": "MobilityNeedsModeEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "EVCC",
        //                 "EVCC_SECC"
        //             ]
        //         },
        //         "ACChargingParametersType": {
        //             "description": "EV AC charging parameters for ISO 15118-2",
        //             "javaType": "ACChargingParameters",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "energyAmount": {
        //                     "description": "Amount of energy requested (in Wh). This includes energy required for preconditioning.\r\nRelates to: +\r\n*ISO 15118-2*: AC_EVChargeParameterType: EAmount +\r\n*ISO 15118-20*: Dynamic/Scheduled_SEReqControlModeType: EVTargetEnergyRequest",
        //                     "type": "number"
        //                 },
        //                 "evMinCurrent": {
        //                     "description": "Minimum current (amps) supported by the electric vehicle (per phase).\r\nRelates to: +\r\n*ISO 15118-2*: AC_EVChargeParameterType: EVMinCurrent",
        //                     "type": "number"
        //                 },
        //                 "evMaxCurrent": {
        //                     "description": "Maximum current (amps) supported by the electric vehicle (per phase). Includes cable capacity.\r\nRelates to: +\r\n*ISO 15118-2*: AC_EVChargeParameterType: EVMaxCurrent",
        //                     "type": "number"
        //                 },
        //                 "evMaxVoltage": {
        //                     "description": "Maximum voltage supported by the electric vehicle.\r\nRelates to: +\r\n*ISO 15118-2*: AC_EVChargeParameterType: EVMaxVoltage",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "energyAmount",
        //                 "evMinCurrent",
        //                 "evMaxCurrent",
        //                 "evMaxVoltage"
        //             ]
        //         },
        //         "ChargingNeedsType": {
        //             "javaType": "ChargingNeeds",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "acChargingParameters": {
        //                     "$ref": "#/definitions/ACChargingParametersType"
        //                 },
        //                 "derChargingParameters": {
        //                     "$ref": "#/definitions/DERChargingParametersType"
        //                 },
        //                 "evEnergyOffer": {
        //                     "$ref": "#/definitions/EVEnergyOfferType"
        //                 },
        //                 "requestedEnergyTransfer": {
        //                     "$ref": "#/definitions/EnergyTransferModeEnumType"
        //                 },
        //                 "dcChargingParameters": {
        //                     "$ref": "#/definitions/DCChargingParametersType"
        //                 },
        //                 "v2xChargingParameters": {
        //                     "$ref": "#/definitions/V2XChargingParametersType"
        //                 },
        //                 "availableEnergyTransfer": {
        //                     "description": "Modes of energy transfer that are marked as available by EV.",
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/EnergyTransferModeEnumType"
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "controlMode": {
        //                     "$ref": "#/definitions/ControlModeEnumType"
        //                 },
        //                 "mobilityNeedsMode": {
        //                     "$ref": "#/definitions/MobilityNeedsModeEnumType"
        //                 },
        //                 "departureTime": {
        //                     "description": "Estimated departure time of the EV. +\r\n*ISO 15118-2:* AC/DC_EVChargeParameterType: DepartureTime +\r\n*ISO 15118-20:* Dynamic/Scheduled_SEReqControlModeType: DepartureTIme",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "requestedEnergyTransfer"
        //             ]
        //         },
        //         "DCChargingParametersType": {
        //             "description": "EV DC charging parameters for ISO 15118-2",
        //             "javaType": "DCChargingParameters",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "evMaxCurrent": {
        //                     "description": "Maximum current (in A) supported by the electric vehicle. Includes cable capacity.\r\nRelates to: +\r\n*ISO 15118-2*: DC_EVChargeParameterType:EVMaximumCurrentLimit",
        //                     "type": "number"
        //                 },
        //                 "evMaxVoltage": {
        //                     "description": "Maximum voltage supported by the electric vehicle.\r\nRelates to: +\r\n*ISO 15118-2*: DC_EVChargeParameterType: EVMaximumVoltageLimit",
        //                     "type": "number"
        //                 },
        //                 "evMaxPower": {
        //                     "description": "Maximum power (in W) supported by the electric vehicle. Required for DC charging.\r\nRelates to: +\r\n*ISO 15118-2*: DC_EVChargeParameterType: EVMaximumPowerLimit",
        //                     "type": "number"
        //                 },
        //                 "evEnergyCapacity": {
        //                     "description": "Capacity of the electric vehicle battery (in Wh).\r\nRelates to: +\r\n*ISO 15118-2*: DC_EVChargeParameterType: EVEnergyCapacity",
        //                     "type": "number"
        //                 },
        //                 "energyAmount": {
        //                     "description": "Amount of energy requested (in Wh). This inludes energy required for preconditioning.\r\nRelates to: +\r\n*ISO 15118-2*: DC_EVChargeParameterType: EVEnergyRequest",
        //                     "type": "number"
        //                 },
        //                 "stateOfCharge": {
        //                     "description": "Energy available in the battery (in percent of the battery capacity)\r\nRelates to: +\r\n*ISO 15118-2*: DC_EVChargeParameterType: DC_EVStatus: EVRESSSOC",
        //                     "type": "integer",
        //                     "minimum": 0.0,
        //                     "maximum": 100.0
        //                 },
        //                 "fullSoC": {
        //                     "description": "Percentage of SoC at which the EV considers the battery fully charged. (possible values: 0 - 100)\r\nRelates to: +\r\n*ISO 15118-2*: DC_EVChargeParameterType: FullSOC",
        //                     "type": "integer",
        //                     "minimum": 0.0,
        //                     "maximum": 100.0
        //                 },
        //                 "bulkSoC": {
        //                     "description": "Percentage of SoC at which the EV considers a fast charging process to end. (possible values: 0 - 100)\r\nRelates to: +\r\n*ISO 15118-2*: DC_EVChargeParameterType: BulkSOC",
        //                     "type": "integer",
        //                     "minimum": 0.0,
        //                     "maximum": 100.0
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "evMaxCurrent",
        //                 "evMaxVoltage"
        //             ]
        //         },
        //         "DERChargingParametersType": {
        //             "description": "DERChargingParametersType is used in ChargingNeedsType during an ISO 15118-20 session for AC_BPT_DER to report the inverter settings related to DER control that were agreed between EVSE and EV.\r\n\r\nFields starting with \"ev\" contain values from the EV.\r\nOther fields contain a value that is supported by both EV and EVSE.\r\n\r\nDERChargingParametersType type is only relevant in case of an ISO 15118-20 AC_BPT_DER/AC_DER charging session.\r\n\r\nNOTE: All these fields have values greater or equal to zero (i.e. are non-negative)",
        //             "javaType": "DERChargingParameters",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "evSupportedDERControl": {
        //                     "description": "DER control functions supported by EV. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType:DERControlFunctions (bitmap)",
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/DERControlEnumType"
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "evOverExcitedMaxDischargePower": {
        //                     "description": "Rated maximum injected active power by EV, at specified over-excited power factor (overExcitedPowerFactor). +\r\nIt can also be defined as the rated maximum discharge power at the rated minimum injected reactive power value. This means that if the EV is providing reactive power support, and it is requested to discharge at max power (e.g. to satisfy an EMS request), the EV may override the request and discharge up to overExcitedMaximumDischargePower to meet the minimum reactive power requirements. +\r\nCorresponds to the WOvPF attribute in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVOverExcitedMaximumDischargePower",
        //                     "type": "number"
        //                 },
        //                 "evOverExcitedPowerFactor": {
        //                     "description": "EV power factor when injecting (over excited) the minimum reactive power. +\r\nCorresponds to the OvPF attribute in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVOverExcitedPowerFactor",
        //                     "type": "number"
        //                 },
        //                 "evUnderExcitedMaxDischargePower": {
        //                     "description": "Rated maximum injected active power by EV supported at specified under-excited power factor (EVUnderExcitedPowerFactor). +\r\nIt can also be defined as the rated maximum dischargePower at the rated minimum absorbed reactive power value.\r\nThis means that if the EV is providing reactive power support, and it is requested to discharge at max power (e.g. to satisfy an EMS request), the EV may override the request and discharge up to underExcitedMaximumDischargePower to meet the minimum reactive power requirements. +\r\nThis corresponds to the WUnPF attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVUnderExcitedMaximumDischargePower",
        //                     "type": "number"
        //                 },
        //                 "evUnderExcitedPowerFactor": {
        //                     "description": "EV power factor when injecting (under excited) the minimum reactive power. +\r\nCorresponds to the OvPF attribute in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVUnderExcitedPowerFactor",
        //                     "type": "number"
        //                 },
        //                 "maxApparentPower": {
        //                     "description": "Rated maximum total apparent power, defined by min(EV, EVSE) in va.\r\nCorresponds to the VAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumApparentPower",
        //                     "type": "number"
        //                 },
        //                 "maxChargeApparentPower": {
        //                     "description": "Rated maximum absorbed apparent power, defined by min(EV, EVSE) in va. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\n    Corresponds to the ChaVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeApparentPower",
        //                     "type": "number"
        //                 },
        //                 "maxChargeApparentPower_L2": {
        //                     "description": "Rated maximum absorbed apparent power on phase L2, defined by min(EV, EVSE) in va.\r\nCorresponds to the ChaVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeApparentPower_L2",
        //                     "type": "number"
        //                 },
        //                 "maxChargeApparentPower_L3": {
        //                     "description": "Rated maximum absorbed apparent power on phase L3, defined by min(EV, EVSE) in va.\r\nCorresponds to the ChaVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeApparentPower_L3",
        //                     "type": "number"
        //                 },
        //                 "maxDischargeApparentPower": {
        //                     "description": "Rated maximum injected apparent power, defined by min(EV, EVSE) in va. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\n    Corresponds to the DisVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeApparentPower",
        //                     "type": "number"
        //                 },
        //                 "maxDischargeApparentPower_L2": {
        //                     "description": "Rated maximum injected apparent power on phase L2, defined by min(EV, EVSE) in va. +\r\n    Corresponds to the DisVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeApparentPower_L2",
        //                     "type": "number"
        //                 },
        //                 "maxDischargeApparentPower_L3": {
        //                     "description": "Rated maximum injected apparent power on phase L3, defined by min(EV, EVSE) in va. +\r\n    Corresponds to the DisVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeApparentPower_L3",
        //                     "type": "number"
        //                 },
        //                 "maxChargeReactivePower": {
        //                     "description": "Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\nCorresponds to the AvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeReactivePower",
        //                     "type": "number"
        //                 },
        //                 "maxChargeReactivePower_L2": {
        //                     "description": "Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars on phase L2. +\r\nCorresponds to the AvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeReactivePower_L2",
        //                     "type": "number"
        //                 },
        //                 "maxChargeReactivePower_L3": {
        //                     "description": "Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars on phase L3. +\r\nCorresponds to the AvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeReactivePower_L3",
        //                     "type": "number"
        //                 },
        //                 "minChargeReactivePower": {
        //                     "description": "Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargeReactivePower",
        //                     "type": "number"
        //                 },
        //                 "minChargeReactivePower_L2": {
        //                     "description": "Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars on phase L2. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargeReactivePower_L2",
        //                     "type": "number"
        //                 },
        //                 "minChargeReactivePower_L3": {
        //                     "description": "Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars on phase L3. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargeReactivePower_L3",
        //                     "type": "number"
        //                 },
        //                 "maxDischargeReactivePower": {
        //                     "description": "Rated maximum injected reactive power, defined by min(EV, EVSE), in vars. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\nCorresponds to the IvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeReactivePower",
        //                     "type": "number"
        //                 },
        //                 "maxDischargeReactivePower_L2": {
        //                     "description": "Rated maximum injected reactive power, defined by min(EV, EVSE), in vars on phase L2. +\r\nCorresponds to the IvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeReactivePower_L2",
        //                     "type": "number"
        //                 },
        //                 "maxDischargeReactivePower_L3": {
        //                     "description": "Rated maximum injected reactive power, defined by min(EV, EVSE), in vars on phase L3. +\r\nCorresponds to the IvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeReactivePower_L3",
        //                     "type": "number"
        //                 },
        //                 "minDischargeReactivePower": {
        //                     "description": "Rated minimum injected reactive power, defined by max(EV, EVSE), in vars. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\n        *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargeReactivePower",
        //                     "type": "number"
        //                 },
        //                 "minDischargeReactivePower_L2": {
        //                     "description": "Rated minimum injected reactive power, defined by max(EV, EVSE), in var on phase L2. +\r\n        *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargeReactivePower_L2",
        //                     "type": "number"
        //                 },
        //                 "minDischargeReactivePower_L3": {
        //                     "description": "Rated minimum injected reactive power, defined by max(EV, EVSE), in var on phase L3. +\r\n        *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargeReactivePower_L3",
        //                     "type": "number"
        //                 },
        //                 "nominalVoltage": {
        //                     "description": "Line voltage supported by EVSE and EV.\r\n        *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVNominalVoltage",
        //                     "type": "number"
        //                 },
        //                 "nominalVoltageOffset": {
        //                     "description": "The nominal AC voltage (rms) offset between the Charging Station's electrical connection point and the utility\u2019s point of common coupling. +\r\n        *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVNominalVoltageOffset",
        //                     "type": "number"
        //                 },
        //                 "maxNominalVoltage": {
        //                     "description": "Maximum AC rms voltage, as defined by min(EV, EVSE)  to operate with. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumNominalVoltage",
        //                     "type": "number"
        //                 },
        //                 "minNominalVoltage": {
        //                     "description": "Minimum AC rms voltage, as defined by max(EV, EVSE)  to operate with. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumNominalVoltage",
        //                     "type": "number"
        //                 },
        //                 "evInverterManufacturer": {
        //                     "description": "Manufacturer of the EV inverter. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterManufacturer",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "evInverterModel": {
        //                     "description": "Model name of the EV inverter. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterModel",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "evInverterSerialNumber": {
        //                     "description": "Serial number of the EV inverter. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterSerialNumber",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "evInverterSwVersion": {
        //                     "description": "Software version of EV inverter. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterSwVersion",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "evInverterHwVersion": {
        //                     "description": "Hardware version of EV inverter. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterHwVersion",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "evIslandingDetectionMethod": {
        //                     "description": "Type of islanding detection method. Only mandatory when islanding detection is required at the site, as set in the ISO 15118 Service Details configuration. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVIslandingDetectionMethod",
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/IslandingDetectionEnumType"
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "evIslandingTripTime": {
        //                     "description": "Time after which EV will trip if an island has been detected. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVIslandingTripTime",
        //                     "type": "number"
        //                 },
        //                 "evMaximumLevel1DCInjection": {
        //                     "description": "Maximum injected DC current allowed at level 1 charging. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumLevel1DCInjection",
        //                     "type": "number"
        //                 },
        //                 "evDurationLevel1DCInjection": {
        //                     "description": "Maximum allowed duration of DC injection at level 1 charging. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVDurationLevel1DCInjection",
        //                     "type": "number"
        //                 },
        //                 "evMaximumLevel2DCInjection": {
        //                     "description": "Maximum injected DC current allowed at level 2 charging. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumLevel2DCInjection",
        //                     "type": "number"
        //                 },
        //                 "evDurationLevel2DCInjection": {
        //                     "description": "Maximum allowed duration of DC injection at level 2 charging. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVDurationLevel2DCInjection",
        //                     "type": "number"
        //                 },
        //                 "evReactiveSusceptance": {
        //                     "description": "\tMeasure of the susceptibility of the circuit to reactance, in Siemens (S). +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVReactiveSusceptance",
        //                     "type": "number"
        //                 },
        //                 "evSessionTotalDischargeEnergyAvailable": {
        //                     "description": "Total energy value, in Wh, that EV is allowed to provide during the entire V2G session. The value is independent of the V2X Cycling area. Once this value reaches the value of 0, the EV may block any attempt to discharge in order to protect the battery health.\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVSessionTotalDischargeEnergyAvailable",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
        //         },
        //         "EVAbsolutePriceScheduleEntryType": {
        //             "description": "An entry in price schedule over time for which EV is willing to discharge.",
        //             "javaType": "EVAbsolutePriceScheduleEntry",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "duration": {
        //                     "description": "The amount of seconds of this entry.",
        //                     "type": "integer"
        //                 },
        //                 "evPriceRule": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/EVPriceRuleType"
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
        //                 "evPriceRule"
        //             ]
        //         },
        //         "EVAbsolutePriceScheduleType": {
        //             "description": "Price schedule of EV energy offer.",
        //             "javaType": "EVAbsolutePriceSchedule",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "timeAnchor": {
        //                     "description": "Starting point in time of the EVEnergyOffer.",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "currency": {
        //                     "description": "Currency code according to ISO 4217.",
        //                     "type": "string",
        //                     "maxLength": 3
        //                 },
        //                 "evAbsolutePriceScheduleEntries": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/EVAbsolutePriceScheduleEntryType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 1024
        //                 },
        //                 "priceAlgorithm": {
        //                     "description": "ISO 15118-20 URN of price algorithm: Power, PeakPower, StackedEnergy.",
        //                     "type": "string",
        //                     "maxLength": 2000
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "timeAnchor",
        //                 "currency",
        //                 "priceAlgorithm",
        //                 "evAbsolutePriceScheduleEntries"
        //             ]
        //         },
        //         "EVEnergyOfferType": {
        //             "description": "A schedule of the energy amount over time that EV is willing to discharge. A negative value indicates the willingness to discharge under specific conditions, a positive value indicates that the EV currently is not able to offer energy to discharge. ",
        //             "javaType": "EVEnergyOffer",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "evAbsolutePriceSchedule": {
        //                     "$ref": "#/definitions/EVAbsolutePriceScheduleType"
        //                 },
        //                 "evPowerSchedule": {
        //                     "$ref": "#/definitions/EVPowerScheduleType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "evPowerSchedule"
        //             ]
        //         },
        //         "EVPowerScheduleEntryType": {
        //             "description": "An entry in schedule of the energy amount over time that EV is willing to discharge. A negative value indicates the willingness to discharge under specific conditions, a positive value indicates that the EV currently is not able to offer energy to discharge.",
        //             "javaType": "EVPowerScheduleEntry",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "duration": {
        //                     "description": "The duration of this entry.",
        //                     "type": "integer"
        //                 },
        //                 "power": {
        //                     "description": "Defines maximum amount of power for the duration of this EVPowerScheduleEntry to be discharged from the EV battery through EVSE power outlet. Negative values are used for discharging.",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "duration",
        //                 "power"
        //             ]
        //         },
        //         "EVPowerScheduleType": {
        //             "description": "Schedule of EV energy offer.",
        //             "javaType": "EVPowerSchedule",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "evPowerScheduleEntries": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/EVPowerScheduleEntryType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 1024
        //                 },
        //                 "timeAnchor": {
        //                     "description": "The time that defines the starting point for the EVEnergyOffer.",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "timeAnchor",
        //                 "evPowerScheduleEntries"
        //             ]
        //         },
        //         "EVPriceRuleType": {
        //             "description": "An entry in price schedule over time for which EV is willing to discharge.",
        //             "javaType": "EVPriceRule",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "energyFee": {
        //                     "description": "Cost per kWh.",
        //                     "type": "number"
        //                 },
        //                 "powerRangeStart": {
        //                     "description": "The EnergyFee applies between this value and the value of the PowerRangeStart of the subsequent EVPriceRule. If the power is below this value, the EnergyFee of the previous EVPriceRule applies. Negative values are used for discharging.",
        //                     "type": "number"
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
        //         "V2XChargingParametersType": {
        //             "description": "Charging parameters for ISO 15118-20, also supporting V2X charging/discharging.+\r\nAll values are greater or equal to zero, with the exception of EVMinEnergyRequest, EVMaxEnergyRequest, EVTargetEnergyRequest, EVMinV2XEnergyRequest and EVMaxV2XEnergyRequest.",
        //             "javaType": "V2XChargingParameters",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "minChargePower": {
        //                     "description": "Minimum charge power in W, defined by max(EV, EVSE).\r\nThis field represents the sum of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1.\r\nRelates to:\r\n*ISO 15118-20*: BPT_AC/DC_CPDReqEnergyTransferModeType: EVMinimumChargePower",
        //                     "type": "number"
        //                 },
        //                 "minChargePower_L2": {
        //                     "description": "Minimum charge power on phase L2 in W, defined by max(EV, EVSE).\r\nRelates to:\r\n*ISO 15118-20*: BPT_AC/DC_CPDReqEnergyTransferModeType: EVMinimumChargePower_L2",
        //                     "type": "number"
        //                 },
        //                 "minChargePower_L3": {
        //                     "description": "Minimum charge power on phase L3 in W, defined by max(EV, EVSE).\r\nRelates to:\r\n*ISO 15118-20*: BPT_AC/DC_CPDReqEnergyTransferModeType: EVMinimumChargePower_L3",
        //                     "type": "number"
        //                 },
        //                 "maxChargePower": {
        //                     "description": "Maximum charge (absorbed) power in W, defined by min(EV, EVSE) at unity power factor. +\r\nThis field represents the sum of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1.\r\nIt corresponds to the ChaWMax attribute in the IEC 61850.\r\nIt is usually equivalent to the rated apparent power of the EV when discharging (ChaVAMax) in IEC 61850. +\r\n\r\nRelates to: \r\n*ISO 15118-20*: BPT_AC/DC_CPDReqEnergyTransferModeType: EVMaximumChargePower",
        //                     "type": "number"
        //                 },
        //                 "maxChargePower_L2": {
        //                     "description": "Maximum charge power on phase L2 in W, defined by min(EV, EVSE)\r\nRelates to: \r\n*ISO 15118-20*: BPT_AC/DC_CPDReqEnergyTransferModeType: EVMaximumChargePower_L2",
        //                     "type": "number"
        //                 },
        //                 "maxChargePower_L3": {
        //                     "description": "Maximum charge power on phase L3 in W, defined by min(EV, EVSE)\r\nRelates to: \r\n*ISO 15118-20*: BPT_AC/DC_CPDReqEnergyTransferModeType: EVMaximumChargePower_L3",
        //                     "type": "number"
        //                 },
        //                 "minDischargePower": {
        //                     "description": "Minimum discharge (injected) power in W, defined by max(EV, EVSE) at unity power factor. Value &gt;= 0. +\r\nThis field represents the sum of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1. +\r\nIt corresponds to the WMax attribute in the IEC 61850.\r\nIt is usually equivalent to the rated apparent power of the EV when discharging (VAMax attribute in the IEC 61850).\r\n\r\nRelates to:\r\n*ISO 15118-20*: BPT_AC/DC_CPDReqEnergyTransferModeType: EVMinimumDischargePower",
        //                     "type": "number"
        //                 },
        //                 "minDischargePower_L2": {
        //                     "description": "Minimum discharge power on phase L2 in W, defined by max(EV, EVSE).  Value &gt;= 0.\r\nRelates to:\r\n*ISO 15118-20*: BPT_AC/DC_CPDReqEnergyTransferModeType: EVMinimumDischargePower_L2",
        //                     "type": "number"
        //                 },
        //                 "minDischargePower_L3": {
        //                     "description": "Minimum discharge power on phase L3 in W, defined by max(EV, EVSE).  Value &gt;= 0.\r\nRelates to:\r\n*ISO 15118-20*: BPT_AC/DC_CPDReqEnergyTransferModeType: EVMinimumDischargePower_L3",
        //                     "type": "number"
        //                 },
        //                 "maxDischargePower": {
        //                     "description": "Maximum discharge (injected) power in W, defined by min(EV, EVSE) at unity power factor.  Value &gt;= 0.\r\nThis field represents the sum of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1.\r\nRelates to:\r\n*ISO 15118-20*: BPT_AC/DC_CPDReqEnergyTransferModeType: EVMaximumDischargePower",
        //                     "type": "number"
        //                 },
        //                 "maxDischargePower_L2": {
        //                     "description": "Maximum discharge power on phase L2 in W, defined by min(EV, EVSE).  Value &gt;= 0.\r\nRelates to:\r\n*ISO 15118-20*: BPT_AC/DC_CPDReqEnergyTransferModeType: EVMaximumDischargePowe_L2",
        //                     "type": "number"
        //                 },
        //                 "maxDischargePower_L3": {
        //                     "description": "Maximum discharge power on phase L3 in W, defined by min(EV, EVSE).  Value &gt;= 0.\r\nRelates to:\r\n*ISO 15118-20*: BPT_AC/DC_CPDReqEnergyTransferModeType: EVMaximumDischargePower_L3",
        //                     "type": "number"
        //                 },
        //                 "minChargeCurrent": {
        //                     "description": "Minimum charge current in A, defined by max(EV, EVSE)\r\nRelates to: \r\n*ISO 15118-20*: BPT_DC_CPDReqEnergyTransferModeType: EVMinimumChargeCurrent",
        //                     "type": "number"
        //                 },
        //                 "maxChargeCurrent": {
        //                     "description": "Maximum charge current in A, defined by min(EV, EVSE)\r\nRelates to: \r\n*ISO 15118-20*: BPT_DC_CPDReqEnergyTransferModeType: EVMaximumChargeCurrent",
        //                     "type": "number"
        //                 },
        //                 "minDischargeCurrent": {
        //                     "description": "Minimum discharge current in A, defined by max(EV, EVSE).  Value &gt;= 0.\r\nRelates to: \r\n*ISO 15118-20*: BPT_DC_CPDReqEnergyTransferModeType: EVMinimumDischargeCurrent",
        //                     "type": "number"
        //                 },
        //                 "maxDischargeCurrent": {
        //                     "description": "Maximum discharge current in A, defined by min(EV, EVSE).  Value &gt;= 0.\r\nRelates to: \r\n*ISO 15118-20*: BPT_DC_CPDReqEnergyTransferModeType: EVMaximumDischargeCurrent",
        //                     "type": "number"
        //                 },
        //                 "minVoltage": {
        //                     "description": "Minimum voltage in V, defined by max(EV, EVSE)\r\nRelates to:\r\n*ISO 15118-20*: BPT_DC_CPDReqEnergyTransferModeType: EVMinimumVoltage",
        //                     "type": "number"
        //                 },
        //                 "maxVoltage": {
        //                     "description": "Maximum voltage in V, defined by min(EV, EVSE)\r\nRelates to:\r\n*ISO 15118-20*: BPT_DC_CPDReqEnergyTransferModeType: EVMaximumVoltage",
        //                     "type": "number"
        //                 },
        //                 "evTargetEnergyRequest": {
        //                     "description": "Energy to requested state of charge in Wh\r\nRelates to:\r\n*ISO 15118-20*: Dynamic/Scheduled_SEReqControlModeType: EVTargetEnergyRequest",
        //                     "type": "number"
        //                 },
        //                 "evMinEnergyRequest": {
        //                     "description": "Energy to minimum allowed state of charge in Wh\r\nRelates to:\r\n*ISO 15118-20*: Dynamic/Scheduled_SEReqControlModeType: EVMinimumEnergyRequest",
        //                     "type": "number"
        //                 },
        //                 "evMaxEnergyRequest": {
        //                     "description": "Energy to maximum state of charge in Wh\r\nRelates to:\r\n*ISO 15118-20*: Dynamic/Scheduled_SEReqControlModeType: EVMaximumEnergyRequest",
        //                     "type": "number"
        //                 },
        //                 "evMinV2XEnergyRequest": {
        //                     "description": "Energy (in Wh) to minimum state of charge for cycling (V2X) activity. \r\nPositive value means that current state of charge is below V2X range.\r\nRelates to:\r\n*ISO 15118-20*: Dynamic_SEReqControlModeType: EVMinimumV2XEnergyRequest",
        //                     "type": "number"
        //                 },
        //                 "evMaxV2XEnergyRequest": {
        //                     "description": "Energy (in Wh) to maximum state of charge for cycling (V2X) activity.\r\nNegative value indicates that current state of charge is above V2X range.\r\nRelates to:\r\n*ISO 15118-20*: Dynamic_SEReqControlModeType: EVMaximumV2XEnergyRequest",
        //                     "type": "number"
        //                 },
        //                 "targetSoC": {
        //                     "description": "Target state of charge at departure as percentage.\r\nRelates to:\r\n*ISO 15118-20*: BPT_DC_CPDReqEnergyTransferModeType: TargetSOC",
        //                     "type": "integer",
        //                     "minimum": 0.0,
        //                     "maximum": 100.0
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
        //         "evseId": {
        //             "description": "Defines the EVSE and connector to which the EV is connected. EvseId may not be 0.",
        //             "type": "integer",
        //             "minimum": 1.0
        //         },
        //         "maxScheduleTuples": {
        //             "description": "Contains the maximum elements the EV supports for: +\r\n- ISO 15118-2: schedule tuples in SASchedule (both Pmax and Tariff). +\r\n- ISO 15118-20: PowerScheduleEntry, PriceRule and PriceLevelScheduleEntries.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "chargingNeeds": {
        //             "$ref": "#/definitions/ChargingNeedsType"
        //         },
        //         "timestamp": {
        //             "description": "Time when EV charging needs were received. +\r\nField can be added when charging station was offline when charging needs were received.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "evseId",
        //         "chargingNeeds"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a notify EV charging needs request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyEVChargingNeedsRequestParser">A delegate to parse custom notify EV charging needs requests.</param>
        public static NotifyEVChargingNeedsRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         SourceRouting                                               Destination,
                                                         NetworkPath                                                 NetworkPath,
                                                         DateTimeOffset?                                             RequestTimestamp                           = null,
                                                         TimeSpan?                                                   RequestTimeout                             = null,
                                                         EventTracking_Id?                                           EventTrackingId                            = null,
                                                         CustomJObjectParserDelegate<NotifyEVChargingNeedsRequest>?  CustomNotifyEVChargingNeedsRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var notifyEVChargingNeedsRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomNotifyEVChargingNeedsRequestParser))
            {
                return notifyEVChargingNeedsRequest;
            }

            throw new ArgumentException("The given JSON representation of a notify EV charging needs request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out NotifyEVChargingNeedsRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a notify EV charging needs request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyEVChargingNeedsRequest">The parsed notify EV charging needs request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyEVChargingNeedsRequestParser">A delegate to parse custom notify EV charging needs requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       SourceRouting                                               Destination,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out NotifyEVChargingNeedsRequest?      NotifyEVChargingNeedsRequest,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       DateTimeOffset?                                             RequestTimestamp                           = null,
                                       TimeSpan?                                                   RequestTimeout                             = null,
                                       EventTracking_Id?                                           EventTrackingId                            = null,
                                       CustomJObjectParserDelegate<NotifyEVChargingNeedsRequest>?  CustomNotifyEVChargingNeedsRequestParser   = null)
        {

            try
            {

                NotifyEVChargingNeedsRequest = null;

                #region EVSEId               [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "EVSE identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingNeeds        [mandatory]

                if (!JSON.ParseMandatoryJSON("chargingNeeds",
                                             "charging needs",
                                             OCPPv2_1.ChargingNeeds.TryParse,
                                             out ChargingNeeds? ChargingNeeds,
                                             out ErrorResponse) ||
                     ChargingNeeds is null)
                {
                    return false;
                }

                #endregion

                #region ReceivedTimestamp    [optional]

                if (JSON.ParseOptional("timestamp",
                                       "received timestamp",
                                       out DateTime? ReceivedTimestamp,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxScheduleTuples    [optional]

                if (JSON.ParseOptional("maxScheduleTuples",
                                       "max schedule tuples",
                                       out UInt16? MaxScheduleTuples,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

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


                NotifyEVChargingNeedsRequest = new NotifyEVChargingNeedsRequest(

                                                   Destination,
                                                   EVSEId,
                                                   ChargingNeeds,
                                                   ReceivedTimestamp,
                                                   MaxScheduleTuples,

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

                if (CustomNotifyEVChargingNeedsRequestParser is not null)
                    NotifyEVChargingNeedsRequest = CustomNotifyEVChargingNeedsRequestParser(JSON,
                                                                                            NotifyEVChargingNeedsRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyEVChargingNeedsRequest  = null;
                ErrorResponse                 = "The given JSON representation of a notify EV charging needs request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyEVChargingNeedsRequestSerializer = null, CustomChargingNeedsSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyEVChargingNeedsRequestSerializer">A delegate to serialize custom NotifyEVChargingNeeds requests.</param>
        /// <param name="CustomChargingNeedsSerializer">A delegate to serialize custom charging needs.</param>
        /// <param name="CustomACChargingParametersSerializer">A delegate to serialize custom AC charging parameters.</param>
        /// <param name="CustomDCChargingParametersSerializer">A delegate to serialize custom DC charging parameters.</param>
        /// <param name="CustomV2XChargingParametersSerializer">A delegate to serialize custom V2X charging parameters.</param>
        /// <param name="CustomEVEnergyOfferSerializer">A delegate to serialize custom ev energy offers.</param>
        /// <param name="CustomEVPowerScheduleSerializer">A delegate to serialize custom ev power schedules.</param>
        /// <param name="CustomEVPowerScheduleEntrySerializer">A delegate to serialize custom ev power schedule entries.</param>
        /// <param name="CustomEVAbsolutePriceScheduleSerializer">A delegate to serialize custom ev absolute price schedules.</param>
        /// <param name="CustomEVAbsolutePriceScheduleEntrySerializer">A delegate to serialize custom charging limits.</param>
        /// <param name="CustomEVPriceRuleSerializer">A delegate to serialize custom ev price rules.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                         IncludeJSONLDContext                           = false,
                              CustomJObjectSerializerDelegate<NotifyEVChargingNeedsRequest>?  CustomNotifyEVChargingNeedsRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingNeeds>?                 CustomChargingNeedsSerializer                  = null,
                              CustomJObjectSerializerDelegate<ACChargingParameters>?          CustomACChargingParametersSerializer           = null,
                              CustomJObjectSerializerDelegate<DCChargingParameters>?          CustomDCChargingParametersSerializer           = null,
                              CustomJObjectSerializerDelegate<V2XChargingParameters>?         CustomV2XChargingParametersSerializer          = null,
                              CustomJObjectSerializerDelegate<EVEnergyOffer>?                 CustomEVEnergyOfferSerializer                  = null,
                              CustomJObjectSerializerDelegate<EVPowerSchedule>?               CustomEVPowerScheduleSerializer                = null,
                              CustomJObjectSerializerDelegate<EVPowerScheduleEntry>?          CustomEVPowerScheduleEntrySerializer           = null,
                              CustomJObjectSerializerDelegate<EVAbsolutePriceSchedule>?       CustomEVAbsolutePriceScheduleSerializer        = null,
                              CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?  CustomEVAbsolutePriceScheduleEntrySerializer   = null,
                              CustomJObjectSerializerDelegate<EVPriceRule>?                   CustomEVPriceRuleSerializer                    = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",            DefaultJSONLDContext.   ToString())
                               : null,

                                 new JProperty("evseId",              EVSEId.           Value),

                                 new JProperty("chargingNeeds",       ChargingNeeds.          ToJSON(CustomChargingNeedsSerializer,
                                                                                                     CustomACChargingParametersSerializer,
                                                                                                     CustomDCChargingParametersSerializer,
                                                                                                     CustomV2XChargingParametersSerializer,
                                                                                                     CustomEVEnergyOfferSerializer,
                                                                                                     CustomEVPowerScheduleSerializer,
                                                                                                     CustomEVPowerScheduleEntrySerializer,
                                                                                                     CustomEVAbsolutePriceScheduleSerializer,
                                                                                                     CustomEVAbsolutePriceScheduleEntrySerializer,
                                                                                                     CustomEVPriceRuleSerializer,
                                                                                                     CustomCustomDataSerializer)),

                           ReceivedTimestamp.HasValue
                               ? new JProperty("timestamp",           ReceivedTimestamp.Value.ToISO8601())
                               : null,

                           MaxScheduleTuples.HasValue
                               ? new JProperty("maxScheduleTuples",   MaxScheduleTuples.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.             ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomNotifyEVChargingNeedsRequestSerializer is not null
                       ? CustomNotifyEVChargingNeedsRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyEVChargingNeedsRequest1, NotifyEVChargingNeedsRequest2)

        /// <summary>
        /// Compares two notify EV charging needs requests for equality.
        /// </summary>
        /// <param name="NotifyEVChargingNeedsRequest1">A notify EV charging needs request.</param>
        /// <param name="NotifyEVChargingNeedsRequest2">Another notify EV charging needs request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyEVChargingNeedsRequest? NotifyEVChargingNeedsRequest1,
                                           NotifyEVChargingNeedsRequest? NotifyEVChargingNeedsRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyEVChargingNeedsRequest1, NotifyEVChargingNeedsRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyEVChargingNeedsRequest1 is null || NotifyEVChargingNeedsRequest2 is null)
                return false;

            return NotifyEVChargingNeedsRequest1.Equals(NotifyEVChargingNeedsRequest2);

        }

        #endregion

        #region Operator != (NotifyEVChargingNeedsRequest1, NotifyEVChargingNeedsRequest2)

        /// <summary>
        /// Compares two notify EV charging needs requests for inequality.
        /// </summary>
        /// <param name="NotifyEVChargingNeedsRequest1">A notify EV charging needs request.</param>
        /// <param name="NotifyEVChargingNeedsRequest2">Another notify EV charging needs request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyEVChargingNeedsRequest? NotifyEVChargingNeedsRequest1,
                                           NotifyEVChargingNeedsRequest? NotifyEVChargingNeedsRequest2)

            => !(NotifyEVChargingNeedsRequest1 == NotifyEVChargingNeedsRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyEVChargingNeedsRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify EV charging needs requests for equality.
        /// </summary>
        /// <param name="Object">A notify EV charging needs request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyEVChargingNeedsRequest notifyEVChargingNeedsRequest &&
                   Equals(notifyEVChargingNeedsRequest);

        #endregion

        #region Equals(NotifyEVChargingNeedsRequest)

        /// <summary>
        /// Compares two notify EV charging needs requests for equality.
        /// </summary>
        /// <param name="NotifyEVChargingNeedsRequest">A notify EV charging needs request to compare with.</param>
        public override Boolean Equals(NotifyEVChargingNeedsRequest? NotifyEVChargingNeedsRequest)

            => NotifyEVChargingNeedsRequest is not null &&

               EVSEId.       Equals(NotifyEVChargingNeedsRequest.EVSEId)        &&
               ChargingNeeds.Equals(NotifyEVChargingNeedsRequest.ChargingNeeds) &&

            ((!ReceivedTimestamp.HasValue && !NotifyEVChargingNeedsRequest.ReceivedTimestamp.HasValue) ||
               ReceivedTimestamp.HasValue &&  NotifyEVChargingNeedsRequest.ReceivedTimestamp.HasValue && ReceivedTimestamp.Value.Equals(NotifyEVChargingNeedsRequest.ReceivedTimestamp.Value)) &&

            ((!MaxScheduleTuples.HasValue && !NotifyEVChargingNeedsRequest.MaxScheduleTuples.HasValue) ||
               MaxScheduleTuples.HasValue &&  NotifyEVChargingNeedsRequest.MaxScheduleTuples.HasValue && MaxScheduleTuples.Value.Equals(NotifyEVChargingNeedsRequest.MaxScheduleTuples.Value)) &&

               base.GenericEquals(NotifyEVChargingNeedsRequest);

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

            => $"EVSE Id: {EVSEId}: {ChargingNeeds}{(ReceivedTimestamp.HasValue ? ", received: " + ReceivedTimestamp : "")}{(MaxScheduleTuples.HasValue ? ", max schedule tuples: " + MaxScheduleTuples : "")}";

        #endregion

    }

}
