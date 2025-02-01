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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// DER charging parameters.
    /// 
    /// ISO 15118-20 session for AC_BPT_DER to report the inverter settings related to DER control that were agreed between EVSE and EV.
    /// Fields starting with \"ev\" contain values from the EV.
    /// Other fields contain a value that is supported by both EV and EVSE.
    /// DERChargingParametersType type is only relevant in case of an ISO 15118-20 AC_BPT_DER/AC_DER charging session.
    /// 
    /// NOTE: All these fields have values greater or equal to zero (i.e. are non-negative)
    /// </summary>
    public class DERChargingParameters : ACustomData,
                                         IEquatable<DERChargingParameters>
    {

        #region Properties

        /// <summary>
        /// DER control functions supported by EV.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType:DERControlFunctions (bitmap)
        /// </summary>
        [Optional]
        public IEnumerable<DERControlType>          EVSupportedDERControls                    { get; }

        /// <summary>
        /// Rated maximum injected active power by EV, at specified over-excited power factor (overExcitedPowerFactor).
        /// It can also be defined as the rated maximum discharge power at the rated minimum injected reactive power value.
        /// This means that if the EV is providing reactive power support, and it is requested to discharge at max power
        /// (e.g. to satisfy an EMS request), the EV may override the request and discharge up to overExcitedMaximumDischargePower
        /// to meet the minimum reactive power requirements.
        /// Corresponds to the WOvPF attribute in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVOverExcitedMaximumDischargePower
        /// </summary>
        [Optional]
        public Decimal                              EVOverExcitedMaxDischargePower            { get; }

        /// <summary>
        /// EV power factor when injecting (over excited) the minimum reactive power.
        /// Corresponds to the OvPF attribute in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVOverExcitedPowerFactor
        /// </summary>
        [Optional]
        public Decimal                              EVOverExcitedPowerFactor                  { get; }

        /// <summary>
        /// Rated maximum injected active power by EV supported at specified under-excited power factor (EVUnderExcitedPowerFactor).
        /// It can also be defined as the rated maximum dischargePower at the rated minimum absorbed reactive power value.
        /// This means that if the EV is providing reactive power support, and it is requested to discharge at max power
        /// (e.g. to satisfy an EMS request), the EV may override the request and discharge up to underExcitedMaximumDischargePower
        /// to meet the minimum reactive power requirements.
        /// This corresponds to the WUnPF attribute in the IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVUnderExcitedMaximumDischargePower
        /// </summary>
        [Optional]
        public Decimal                              EVUnderExcitedMaxDischargePower           { get; }

        /// <summary>
        /// EV power factor when injecting (under excited) the minimum reactive power.
        /// Corresponds to the OvPF attribute in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVUnderExcitedPowerFactor
        /// </summary>
        [Optional]
        public Decimal                              EVUnderExcitedPowerFactor                 { get; }

        /// <summary>
        /// Rated maximum total apparent power, defined by min(EV, EVSE) in va.
        /// Corresponds to the VAMaxRtg in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumApparentPower
        /// </summary>
        [Optional]
        public Decimal                              MaxApparentPower                          { get; }

        /// <summary>
        /// Rated maximum absorbed apparent power, defined by min(EV, EVSE) in va.
        /// This field represents the sum of all phases, unless values are provided for L2 and L3,
        /// in which case this field represents phase L1.
        /// Corresponds to the ChaVAMaxRtg in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeApparentPower
        /// </summary>
        [Optional]
        public Decimal                              MaxChargeApparentPower                    { get; }

        /// <summary>
        /// Rated maximum absorbed apparent power on phase L2, defined by min(EV, EVSE) in va.
        /// Corresponds to the ChaVAMaxRtg in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeApparentPower_L2
        /// </summary>
        [Optional]
        public Decimal                              MaxChargeApparentPower_L2                 { get; }

        /// <summary>
        /// Rated maximum absorbed apparent power on phase L3, defined by min(EV, EVSE) in va.
        /// Corresponds to the ChaVAMaxRtg in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeApparentPower_L3
        /// </summary>
        [Optional]
        public Decimal                              MaxChargeApparentPower_L3                 { get; }

        /// <summary>
        /// Rated maximum injected apparent power, defined by min(EV, EVSE) in va.
        /// This field represents the sum of all phases, unless values are provided for L2 and L3,
        /// in which case this field represents phase L1.
        /// Corresponds to the DisVAMaxRtg in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeApparentPower
        /// </summary>
        [Optional]
        public Decimal                              MaxDischargeApparentPower                 { get; }

        /// <summary>
        /// Rated maximum injected apparent power on phase L2, defined by min(EV, EVSE) in va.
        /// Corresponds to the DisVAMaxRtg in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeApparentPower_L2
        /// </summary>
        [Optional]
        public Decimal                              MaxDischargeApparentPower_L2              { get; }

        /// <summary>
        /// Rated maximum injected apparent power on phase L3, defined by min(EV, EVSE) in va.
        /// Corresponds to the DisVAMaxRtg in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeApparentPower_L3
        /// </summary>
        [Optional]
        public Decimal                              MaxDischargeApparentPower_L3              { get; }

        /// <summary>
        /// Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars.
        /// This field represents the sum of all phases, unless values are provided for L2 and L3,
        /// in which case this field represents phase L1.
        /// Corresponds to the AvarMax attribute in the IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeReactivePower
        /// </summary>
        [Optional]
        public Decimal                              MaxChargeReactivePower                    { get; }

        /// <summary>
        /// Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars on phase L2.
        /// Corresponds to the AvarMax attribute in the IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeReactivePower_L2
        /// </summary>
        [Optional]
        public Decimal                              MaxChargeReactivePower_L2                 { get; }

        /// <summary>
        /// Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars on phase L3.
        /// Corresponds to the AvarMax attribute in the IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeReactivePower_L3
        /// </summary>
        [Optional]
        public Decimal                              MaxChargeReactivePower_L3                 { get; }

        /// <summary>
        /// Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars.
        /// This field represents the sum of all phases, unless values are provided for L2 and L3,
        /// in which case this field represents phase L1.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargeReactivePower
        /// </summary>
        [Optional]
        public Decimal                              MinChargeReactivePower                    { get; }

        /// <summary>
        /// Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars on phase L2.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargeReactivePower_L2
        /// </summary>
        [Optional]
        public Decimal                              MinChargeReactivePower_L2                 { get; }

        /// <summary>
        /// Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars on phase L3.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargeReactivePower_L3
        /// </summary>
        [Optional]
        public Decimal                              MinChargeReactivePower_L3                 { get; }

        /// <summary>
        /// Rated maximum injected reactive power, defined by min(EV, EVSE), in vars.
        /// This field represents the sum of all phases, unless values are provided for L2 and L3,
        /// in which case this field represents phase L1.
        /// Corresponds to the IvarMax attribute in the IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeReactivePower
        /// </summary>
        [Optional]
        public Decimal                              MaxDischargeReactivePower                 { get; }

        /// <summary>
        /// Rated maximum injected reactive power, defined by min(EV, EVSE), in vars on phase L2.
        /// Corresponds to the IvarMax attribute in the IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeReactivePower_L2
        /// </summary>
        [Optional]
        public Decimal                              MaxDischargeReactivePower_L2              { get; }

        /// <summary>
        /// Rated maximum injected reactive power, defined by min(EV, EVSE), in vars on phase L3.
        /// Corresponds to the IvarMax attribute in the IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeReactivePower_L3
        /// </summary>
        [Optional]
        public Decimal                              MaxDischargeReactivePower_L3              { get; }

        /// <summary>
        /// Rated minimum injected reactive power, defined by max(EV, EVSE), in vars.
        /// This field represents the sum of all phases, unless values are provided for L2 and L3,
        /// in which case this field represents phase L1.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargeReactivePower
        /// </summary>
        [Optional]
        public Decimal                              MinDischargeReactivePower                 { get; }

        /// <summary>
        /// Rated minimum injected reactive power, defined by max(EV, EVSE), in var on phase L2.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargeReactivePower_L2
        /// </summary>
        [Optional]
        public Decimal                              MinDischargeReactivePower_L2              { get; }

        /// <summary>
        /// Rated minimum injected reactive power, defined by max(EV, EVSE), in var on phase L3.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargeReactivePower_L3
        /// </summary>
        [Optional]
        public Decimal                              MinDischargeReactivePower_L3              { get; }

        /// <summary>
        /// Line voltage supported by EVSE and EV.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVNominalVoltage
        /// </summary>
        [Optional]
        public Decimal                              NominalVoltage                            { get; }

        /// <summary>
        /// The nominal AC voltage (rms) offset between the Charging Station's electrical
        /// connection point and the utility\u2019s point of common coupling.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVNominalVoltageOffset
        /// </summary>
        [Optional]
        public Decimal                              NominalVoltageOffset                      { get; }

        /// <summary>
        /// Maximum AC rms voltage, as defined by min(EV, EVSE) to operate with.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumNominalVoltage
        /// </summary>
        [Optional]
        public Decimal                              MaxNominalVoltage                         { get; }

        /// <summary>
        /// Minimum AC rms voltage, as defined by max(EV, EVSE) to operate with.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumNominalVoltage
        /// </summary>
        [Optional]
        public Decimal                              MinNominalVoltage                         { get; }

        /// <summary>
        /// Manufacturer of the EV inverter.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterManufacturer
        /// </summary>
        [Optional]
        public Decimal                              EVInverterManufacturer                    { get; }

        /// <summary>
        /// Model name of the EV inverter.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterModel
        /// </summary>
        [Optional]
        public Decimal                              EVInverterModel                           { get; }

        /// <summary>
        /// Serial number of the EV inverter.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterSerialNumber
        /// </summary>
        [Optional]
        public Decimal                              EVInverterSerialNumber                    { get; }

        /// <summary>
        /// Software version of EV inverter.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterSwVersion
        /// </summary>
        [Optional]
        public Decimal                              EVInverterSWVersion                       { get; }

        /// <summary>
        /// Hardware version of EV inverter.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterHwVersion
        /// </summary>
        [Optional]
        public Decimal                              EVInverterHWVersion                       { get; }

        /// <summary>
        /// Type of islanding detection method. Only mandatory when islanding detection is
        /// required at the site, as set in the ISO 15118 Service Details configuration.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVIslandingDetectionMethod
        /// </summary>
        [Optional]
        public IEnumerable<IslandingDetectionType>  EVIslandingDetectionMethod                { get; }

        /// <summary>
        /// Time after which EV will trip if an island has been detected.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVIslandingTripTime
        /// </summary>
        [Optional]
        public Decimal                              EVIslandingTripTime                       { get; }

        /// <summary>
        /// Maximum injected DC current allowed at level 1 charging.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumLevel1DCInjection
        /// </summary>
        [Optional]
        public Decimal                              EVMaximumLevel1DCInjection                { get; }

        /// <summary>
        /// Maximum allowed duration of DC injection at level 1 charging.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVDurationLevel1DCInjection
        /// </summary>
        [Optional]
        public Decimal                              EVDurationLevel1DCInjection               { get; }

        /// <summary>
        /// Maximum injected DC current allowed at level 2 charging.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumLevel2DCInjection
        /// </summary>
        [Optional]
        public Decimal                              EVMaximumLevel2DCInjection                { get; }

        /// <summary>
        /// Maximum injected DC current allowed at level 2 charging.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumLevel2DCInjection
        /// </summary>
        [Optional]
        public Decimal                              EVDurationLevel2DCInjection               { get; }

        /// <summary>
        /// Measure of the susceptibility of the circuit to reactance, in Siemens (S).
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVReactiveSusceptance
        /// </summary>
        [Optional]
        public Decimal                              EVReactiveSusceptance                     { get; }

        /// <summary>
        /// Total energy value, in Wh, that EV is allowed to provide during the entire V2G session.
        /// The value is independent of the V2X Cycling area. Once this value reaches the value
        /// of 0, the EV may block any attempt to discharge in order to protect the battery health.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVSessionTotalDischargeEnergyAvailable
        /// </summary>
        [Optional]
        public Decimal                              EVSessionTotalDischargeEnergyAvailable    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DER charging parameters.
        /// </summary>
        /// <param name="EnergyAmount">The amount of energy requested (in Wh). This includes energy required for preconditioning.</param>
        /// <param name="EVMinCurrent">The minimum current (in A) supported by the electric vehicle (per phase).</param>
        /// <param name="EVMaxCurrent">The maximum current (in A) supported by the electric vehicle (per phase) including the maximum cable capacity.</param>
        /// <param name="EVMaxVoltage">The maximum voltage (in V) supported by the electric vehicle.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DERChargingParameters(WattHour     EnergyAmount,
                                     Ampere       EVMinCurrent,
                                     Ampere       EVMaxCurrent,
                                     Volt         EVMaxVoltage,
                                     CustomData?  CustomData   = null)

            : base(CustomData)

        {

            //this.EnergyAmount  = EnergyAmount;
            //this.EVMinCurrent  = EVMinCurrent;
            //this.EVMaxCurrent  = EVMaxCurrent;
            //this.EVMaxVoltage  = EVMaxVoltage;

            unchecked
            {

                hashCode = EnergyAmount.GetHashCode() * 11 ^
                           EVMinCurrent.GetHashCode() *  7 ^
                           EVMaxCurrent.GetHashCode() *  5 ^
                           EVMaxVoltage.GetHashCode() *  3 ^
                           base.        GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "*(2.1)* DERChargingParametersType is used in ChargingNeedsType during an ISO 15118-20 session for AC_BPT_DER to report the inverter settings related to DER control that were agreed between EVSE and EV.\r\n\r\nFields starting with \"ev\" contain values from the EV.\r\nOther fields contain a value that is supported by both EV and EVSE.\r\n\r\nDERChargingParametersType type is only relevant in case of an ISO 15118-20 AC_BPT_DER/AC_DER charging session.\r\n\r\nNOTE: All these fields have values greater or equal to zero (i.e. are non-negative)\r\n\r\n",
        //     "javaType": "DERChargingParameters",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "evSupportedDERControl": {
        //             "description": "DER control functions supported by EV. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType:DERControlFunctions (bitmap)\r\n",
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/DERControlEnumType"
        //             },
        //             "minItems": 1
        //         },
        //         "evOverExcitedMaxDischargePower": {
        //             "description": "Rated maximum injected active power by EV, at specified over-excited power factor (overExcitedPowerFactor). +\r\nIt can also be defined as the rated maximum discharge power at the rated minimum injected reactive power value. This means that if the EV is providing reactive power support, and it is requested to discharge at max power (e.g. to satisfy an EMS request), the EV may override the request and discharge up to overExcitedMaximumDischargePower to meet the minimum reactive power requirements. +\r\nCorresponds to the WOvPF attribute in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVOverExcitedMaximumDischargePower\r\n",
        //             "type": "number"
        //         },
        //         "evOverExcitedPowerFactor": {
        //             "description": "EV power factor when injecting (over excited) the minimum reactive power. +\r\nCorresponds to the OvPF attribute in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVOverExcitedPowerFactor\r\n",
        //             "type": "number"
        //         },
        //         "evUnderExcitedMaxDischargePower": {
        //             "description": "Rated maximum injected active power by EV supported at specified under-excited power factor (EVUnderExcitedPowerFactor). +\r\nIt can also be defined as the rated maximum dischargePower at the rated minimum absorbed reactive power value.\r\nThis means that if the EV is providing reactive power support, and it is requested to discharge at max power (e.g. to satisfy an EMS request), the EV may override the request and discharge up to underExcitedMaximumDischargePower to meet the minimum reactive power requirements. +\r\nThis corresponds to the WUnPF attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVUnderExcitedMaximumDischargePower\r\n",
        //             "type": "number"
        //         },
        //         "evUnderExcitedPowerFactor": {
        //             "description": "EV power factor when injecting (under excited) the minimum reactive power. +\r\nCorresponds to the OvPF attribute in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVUnderExcitedPowerFactor\r\n",
        //             "type": "number"
        //         },
        //         "maxApparentPower": {
        //             "description": "Rated maximum total apparent power, defined by min(EV, EVSE) in va.\r\nCorresponds to the VAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumApparentPower\r\n",
        //             "type": "number"
        //         },
        //         "maxChargeApparentPower": {
        //             "description": "Rated maximum absorbed apparent power, defined by min(EV, EVSE) in va. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\n    Corresponds to the ChaVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeApparentPower\r\n",
        //             "type": "number"
        //         },
        //         "maxChargeApparentPower_L2": {
        //             "description": "Rated maximum absorbed apparent power on phase L2, defined by min(EV, EVSE) in va.\r\nCorresponds to the ChaVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeApparentPower_L2\r\n",
        //             "type": "number"
        //         },
        //         "maxChargeApparentPower_L3": {
        //             "description": "Rated maximum absorbed apparent power on phase L3, defined by min(EV, EVSE) in va.\r\nCorresponds to the ChaVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeApparentPower_L3\r\n",
        //             "type": "number"
        //         },
        //         "maxDischargeApparentPower": {
        //             "description": "Rated maximum injected apparent power, defined by min(EV, EVSE) in va. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\n    Corresponds to the DisVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeApparentPower\r\n",
        //             "type": "number"
        //         },
        //         "maxDischargeApparentPower_L2": {
        //             "description": "Rated maximum injected apparent power on phase L2, defined by min(EV, EVSE) in va. +\r\n    Corresponds to the DisVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeApparentPower_L2\r\n",
        //             "type": "number"
        //         },
        //         "maxDischargeApparentPower_L3": {
        //             "description": "Rated maximum injected apparent power on phase L3, defined by min(EV, EVSE) in va. +\r\n    Corresponds to the DisVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeApparentPower_L3\r\n",
        //             "type": "number"
        //         },
        //         "maxChargeReactivePower": {
        //             "description": "Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\nCorresponds to the AvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeReactivePower\r\n",
        //             "type": "number"
        //         },
        //         "maxChargeReactivePower_L2": {
        //             "description": "Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars on phase L2. +\r\nCorresponds to the AvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeReactivePower_L2\r\n",
        //             "type": "number"
        //         },
        //         "maxChargeReactivePower_L3": {
        //             "description": "Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars on phase L3. +\r\nCorresponds to the AvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeReactivePower_L3\r\n",
        //             "type": "number"
        //         },
        //         "minChargeReactivePower": {
        //             "description": "Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargeReactivePower\r\n",
        //             "type": "number"
        //         },
        //         "minChargeReactivePower_L2": {
        //             "description": "Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars on phase L2. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargeReactivePower_L2\r\n",
        //             "type": "number"
        //         },
        //         "minChargeReactivePower_L3": {
        //             "description": "Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars on phase L3. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargeReactivePower_L3\r\n",
        //             "type": "number"
        //         },
        //         "maxDischargeReactivePower": {
        //             "description": "Rated maximum injected reactive power, defined by min(EV, EVSE), in vars. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\nCorresponds to the IvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeReactivePower\r\n",
        //             "type": "number"
        //         },
        //         "maxDischargeReactivePower_L2": {
        //             "description": "Rated maximum injected reactive power, defined by min(EV, EVSE), in vars on phase L2. +\r\nCorresponds to the IvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeReactivePower_L2\r\n",
        //             "type": "number"
        //         },
        //         "maxDischargeReactivePower_L3": {
        //             "description": "Rated maximum injected reactive power, defined by min(EV, EVSE), in vars on phase L3. +\r\nCorresponds to the IvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeReactivePower_L3\r\n",
        //             "type": "number"
        //         },
        //         "minDischargeReactivePower": {
        //             "description": "Rated minimum injected reactive power, defined by max(EV, EVSE), in vars. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\n        *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargeReactivePower\r\n",
        //             "type": "number"
        //         },
        //         "minDischargeReactivePower_L2": {
        //             "description": "Rated minimum injected reactive power, defined by max(EV, EVSE), in var on phase L2. +\r\n        *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargeReactivePower_L2\r\n",
        //             "type": "number"
        //         },
        //         "minDischargeReactivePower_L3": {
        //             "description": "Rated minimum injected reactive power, defined by max(EV, EVSE), in var on phase L3. +\r\n        *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargeReactivePower_L3\r\n",
        //             "type": "number"
        //         },
        //         "nominalVoltage": {
        //             "description": "Line voltage supported by EVSE and EV.\r\n        *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVNominalVoltage\r\n",
        //             "type": "number"
        //         },
        //         "nominalVoltageOffset": {
        //             "description": "The nominal AC voltage (rms) offset between the Charging Station's electrical connection point and the utility\u2019s point of common coupling. +\r\n        *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVNominalVoltageOffset\r\n",
        //             "type": "number"
        //         },
        //         "maxNominalVoltage": {
        //             "description": "Maximum AC rms voltage, as defined by min(EV, EVSE)  to operate with. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumNominalVoltage\r\n",
        //             "type": "number"
        //         },
        //         "minNominalVoltage": {
        //             "description": "Minimum AC rms voltage, as defined by max(EV, EVSE)  to operate with. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumNominalVoltage\r\n",
        //             "type": "number"
        //         },
        //         "evInverterManufacturer": {
        //             "description": "Manufacturer of the EV inverter. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterManufacturer\r\n",
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "evInverterModel": {
        //             "description": "Model name of the EV inverter. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterModel\r\n",
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "evInverterSerialNumber": {
        //             "description": "Serial number of the EV inverter. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterSerialNumber\r\n",
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "evInverterSwVersion": {
        //             "description": "Software version of EV inverter. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterSwVersion\r\n",
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "evInverterHwVersion": {
        //             "description": "Hardware version of EV inverter. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterHwVersion\r\n",
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "evIslandingDetectionMethod": {
        //             "description": "Type of islanding detection method. Only mandatory when islanding detection is required at the site, as set in the ISO 15118 Service Details configuration. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVIslandingDetectionMethod\r\n",
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/IslandingDetectionEnumType"
        //             },
        //             "minItems": 1
        //         },
        //         "evIslandingTripTime": {
        //             "description": "Time after which EV will trip if an island has been detected. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVIslandingTripTime\r\n",
        //             "type": "number"
        //         },
        //         "evMaximumLevel1DCInjection": {
        //             "description": "Maximum injected DC current allowed at level 1 charging. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumLevel1DCInjection\r\n",
        //             "type": "number"
        //         },
        //         "evDurationLevel1DCInjection": {
        //             "description": "Maximum allowed duration of DC injection at level 1 charging. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVDurationLevel1DCInjection\r\n",
        //             "type": "number"
        //         },
        //         "evMaximumLevel2DCInjection": {
        //             "description": "Maximum injected DC current allowed at level 2 charging. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumLevel2DCInjection\r\n",
        //             "type": "number"
        //         },
        //         "evDurationLevel2DCInjection": {
        //             "description": "Maximum allowed duration of DC injection at level 2 charging. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVDurationLevel2DCInjection\r\n",
        //             "type": "number"
        //         },
        //         "evReactiveSusceptance": {
        //             "description": "\tMeasure of the susceptibility of the circuit to reactance, in Siemens (S). +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVReactiveSusceptance\r\n\r\n\r\n",
        //             "type": "number"
        //         },
        //         "evSessionTotalDischargeEnergyAvailable": {
        //             "description": "Total energy value, in Wh, that EV is allowed to provide during the entire V2G session. The value is independent of the V2X Cycling area. Once this value reaches the value of 0, the EV may block any attempt to discharge in order to protect the battery health.\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVSessionTotalDischargeEnergyAvailable\r\n\r\n\r\n",
        //             "type": "number"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomDERChargingParametersParser = null)

        /// <summary>
        /// Parse the given JSON representation of DER charging parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomDERChargingParametersParser">A delegate to parse custom DER charging parameters JSON objects.</param>
        public static DERChargingParameters Parse(JObject                                             JSON,
                                                 CustomJObjectParserDelegate<DERChargingParameters>?  CustomDERChargingParametersParser   = null)
        {

            if (TryParse(JSON,
                         out var derChargingParameters,
                         out var errorResponse,
                         CustomDERChargingParametersParser))
            {
                return derChargingParameters;
            }

            throw new ArgumentException("The given JSON representation of DER charging parameters is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out DERChargingParameters, out ErrorResponse, CustomDERChargingParametersParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of DER charging parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DERChargingParameters">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out DERChargingParameters?  DERChargingParameters,
                                       [NotNullWhen(false)] out String?                 ErrorResponse)

            => TryParse(JSON,
                        out DERChargingParameters,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of DER charging parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DERChargingParameters">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDERChargingParametersParser">A delegate to parse custom DER charging parameters JSON objects.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       [NotNullWhen(true)]  out DERChargingParameters?      DERChargingParameters,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       CustomJObjectParserDelegate<DERChargingParameters>?  CustomDERChargingParametersParser)
        {

            try
            {

                DERChargingParameters = default;

                #region EnergyAmount    [mandatory]

                if (!JSON.ParseMandatory("energyAmount",
                                         "energy amount",
                                         WattHour.TryParse,
                                         out WattHour EnergyAmount,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVMinCurrent    [mandatory]

                if (!JSON.ParseMandatory("evMinCurrent",
                                         "ev min current",
                                         Ampere.TryParse,
                                         out Ampere EVMinCurrent,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVMaxCurrent    [mandatory]

                if (!JSON.ParseMandatory("evMaxCurrent",
                                         "ev max current",
                                         Ampere.TryParse,
                                         out Ampere EVMaxCurrent,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVMaxVoltage    [mandatory]

                if (!JSON.ParseMandatory("evMaxVoltage",
                                         "ev max voltage",
                                         Volt.TryParse,
                                         out Volt EVMaxVoltage,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData      [optional]

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


                DERChargingParameters = new DERChargingParameters(
                                            EnergyAmount,
                                            EVMinCurrent,
                                            EVMaxCurrent,
                                            EVMaxVoltage,
                                            CustomData
                                        );

                if (CustomDERChargingParametersParser is not null)
                    DERChargingParameters = CustomDERChargingParametersParser(JSON,
                                                                              DERChargingParameters);

                return true;

            }
            catch (Exception e)
            {
                DERChargingParameters  = default;
                ErrorResponse          = "The given JSON representation of DER charging parameters is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomDERChargingParametersSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDERChargingParametersSerializer">A delegate to serialize custom DER charging parameters.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DERChargingParameters>?  CustomDERChargingParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 //new JProperty("energyAmount",   EnergyAmount.IntegerValue),
                                 //new JProperty("evMinCurrent",   EVMinCurrent.IntegerValue),
                                 //new JProperty("evMaxCurrent",   EVMaxCurrent.IntegerValue),
                                 //new JProperty("evMaxVoltage",   EVMaxVoltage.IntegerValue),

                           CustomData is not null
                               ? new JProperty("customData",     CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDERChargingParametersSerializer is not null
                       ? CustomDERChargingParametersSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (DERChargingParameters1, DERChargingParameters2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERChargingParameters1">DER charging parameters.</param>
        /// <param name="DERChargingParameters2">Another DER charging parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DERChargingParameters? DERChargingParameters1,
                                           DERChargingParameters? DERChargingParameters2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DERChargingParameters1, DERChargingParameters2))
                return true;

            // If one is null, but not both, return false.
            if (DERChargingParameters1 is null || DERChargingParameters2 is null)
                return false;

            return DERChargingParameters1.Equals(DERChargingParameters2);

        }

        #endregion

        #region Operator != (DERChargingParameters1, DERChargingParameters2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DERChargingParameters1">DER charging parameters.</param>
        /// <param name="DERChargingParameters2">Another DER charging parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DERChargingParameters? DERChargingParameters1,
                                           DERChargingParameters? DERChargingParameters2)

            => !(DERChargingParameters1 == DERChargingParameters2);

        #endregion

        #endregion

        #region IEquatable<DERChargingParameters> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DER charging parameters for equality..
        /// </summary>
        /// <param name="Object">DER charging parameters to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DERChargingParameters derChargingParameters &&
                   Equals(derChargingParameters);

        #endregion

        #region Equals(DERChargingParameters)

        /// <summary>
        /// Compares two DER charging parameters for equality.
        /// </summary>
        /// <param name="DERChargingParameters">DER charging parameters to compare with.</param>
        public Boolean Equals(DERChargingParameters? DERChargingParameters)

            => DERChargingParameters is not null &&

               //EnergyAmount.Equals(DERChargingParameters.EnergyAmount) &&
               //EVMinCurrent.Equals(DERChargingParameters.EVMinCurrent) &&
               //EVMaxCurrent.Equals(DERChargingParameters.EVMaxCurrent) &&
               //EVMaxVoltage.Equals(DERChargingParameters.EVMaxVoltage) &&

               base.Equals(DERChargingParameters);

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

            => String.Concat(""
                   //EnergyAmount, " Wh, ",
                   //EVMinCurrent, " A, ",
                   //EVMaxCurrent, " A, ",
                   //EVMaxVoltage, " V"
               );

        #endregion

    }

}
