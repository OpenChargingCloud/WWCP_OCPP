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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// DER charging parameters.
    /// 
    /// ISO 15118-20 session for AC_BPT_DER to report the inverter settings related to DER control that were agreed between EVSE and EV.
    /// Fields starting with "ev" contain values from the EV.
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
        public IEnumerable<DERControlType>            EVSupportedDERControls                    { get; }

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
        public Watt?                                  EVOverExcitedMaxDischargePower            { get; }

        /// <summary>
        /// EV power factor when injecting (over excited) the minimum reactive power.
        /// Corresponds to the OvPF attribute in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVOverExcitedPowerFactor
        /// </summary>
        [Optional]
        public Decimal?                               EVOverExcitedPowerFactor                  { get; }

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
        public Watt?                                  EVUnderExcitedMaxDischargePower           { get; }

        /// <summary>
        /// EV power factor when injecting (under excited) the minimum reactive power.
        /// Corresponds to the OvPF attribute in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVUnderExcitedPowerFactor
        /// </summary>
        [Optional]
        public Decimal?                               EVUnderExcitedPowerFactor                 { get; }

        /// <summary>
        /// Rated maximum total apparent power, defined by min(EV, EVSE) in va.
        /// Corresponds to the VAMaxRtg in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumApparentPower
        /// </summary>
        [Optional]
        public VoltAmpere?                            MaxApparentPower                          { get; }

        /// <summary>
        /// Rated maximum absorbed apparent power, defined by min(EV, EVSE) in va.
        /// This field represents the sum of all phases, unless values are provided for L2 and L3,
        /// in which case this field represents phase L1.
        /// Corresponds to the ChaVAMaxRtg in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeApparentPower
        /// </summary>
        [Optional]
        public VoltAmpere?                            MaxChargeApparentPower                    { get; }

        /// <summary>
        /// Rated maximum absorbed apparent power on phase L2, defined by min(EV, EVSE) in va.
        /// Corresponds to the ChaVAMaxRtg in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeApparentPower_L2
        /// </summary>
        [Optional]
        public VoltAmpere?                            MaxChargeApparentPower_L2                 { get; }

        /// <summary>
        /// Rated maximum absorbed apparent power on phase L3, defined by min(EV, EVSE) in va.
        /// Corresponds to the ChaVAMaxRtg in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeApparentPower_L3
        /// </summary>
        [Optional]
        public VoltAmpere?                            MaxChargeApparentPower_L3                 { get; }

        /// <summary>
        /// Rated maximum injected apparent power, defined by min(EV, EVSE) in va.
        /// This field represents the sum of all phases, unless values are provided for L2 and L3,
        /// in which case this field represents phase L1.
        /// Corresponds to the DisVAMaxRtg in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeApparentPower
        /// </summary>
        [Optional]
        public VoltAmpere?                            MaxDischargeApparentPower                 { get; }

        /// <summary>
        /// Rated maximum injected apparent power on phase L2, defined by min(EV, EVSE) in va.
        /// Corresponds to the DisVAMaxRtg in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeApparentPower_L2
        /// </summary>
        [Optional]
        public VoltAmpere?                            MaxDischargeApparentPower_L2              { get; }

        /// <summary>
        /// Rated maximum injected apparent power on phase L3, defined by min(EV, EVSE) in va.
        /// Corresponds to the DisVAMaxRtg in IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeApparentPower_L3
        /// </summary>
        [Optional]
        public VoltAmpere?                            MaxDischargeApparentPower_L3              { get; }

        /// <summary>
        /// Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars.
        /// This field represents the sum of all phases, unless values are provided for L2 and L3,
        /// in which case this field represents phase L1.
        /// Corresponds to the AvarMax attribute in the IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeReactivePower
        /// </summary>
        [Optional]
        public VoltAmpereReactive?                    MaxChargeReactivePower                    { get; }

        /// <summary>
        /// Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars on phase L2.
        /// Corresponds to the AvarMax attribute in the IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeReactivePower_L2
        /// </summary>
        [Optional]
        public VoltAmpereReactive?                    MaxChargeReactivePower_L2                 { get; }

        /// <summary>
        /// Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars on phase L3.
        /// Corresponds to the AvarMax attribute in the IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeReactivePower_L3
        /// </summary>
        [Optional]
        public VoltAmpereReactive?                    MaxChargeReactivePower_L3                 { get; }

        /// <summary>
        /// Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars.
        /// This field represents the sum of all phases, unless values are provided for L2 and L3,
        /// in which case this field represents phase L1.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargeReactivePower
        /// </summary>
        [Optional]
        public VoltAmpereReactive?                    MinChargeReactivePower                    { get; }

        /// <summary>
        /// Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars on phase L2.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargeReactivePower_L2
        /// </summary>
        [Optional]
        public VoltAmpereReactive?                    MinChargeReactivePower_L2                 { get; }

        /// <summary>
        /// Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars on phase L3.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargeReactivePower_L3
        /// </summary>
        [Optional]
        public VoltAmpereReactive?                    MinChargeReactivePower_L3                 { get; }

        /// <summary>
        /// Rated maximum injected reactive power, defined by min(EV, EVSE), in vars.
        /// This field represents the sum of all phases, unless values are provided for L2 and L3,
        /// in which case this field represents phase L1.
        /// Corresponds to the IvarMax attribute in the IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeReactivePower
        /// </summary>
        [Optional]
        public VoltAmpereReactive?                    MaxDischargeReactivePower                 { get; }

        /// <summary>
        /// Rated maximum injected reactive power, defined by min(EV, EVSE), in vars on phase L2.
        /// Corresponds to the IvarMax attribute in the IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeReactivePower_L2
        /// </summary>
        [Optional]
        public VoltAmpereReactive?                    MaxDischargeReactivePower_L2              { get; }

        /// <summary>
        /// Rated maximum injected reactive power, defined by min(EV, EVSE), in vars on phase L3.
        /// Corresponds to the IvarMax attribute in the IEC 61850.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeReactivePower_L3
        /// </summary>
        [Optional]
        public VoltAmpereReactive?                    MaxDischargeReactivePower_L3              { get; }

        /// <summary>
        /// Rated minimum injected reactive power, defined by max(EV, EVSE), in vars.
        /// This field represents the sum of all phases, unless values are provided for L2 and L3,
        /// in which case this field represents phase L1.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargeReactivePower
        /// </summary>
        [Optional]
        public VoltAmpereReactive?                    MinDischargeReactivePower                 { get; }

        /// <summary>
        /// Rated minimum injected reactive power, defined by max(EV, EVSE), in var on phase L2.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargeReactivePower_L2
        /// </summary>
        [Optional]
        public VoltAmpereReactive?                    MinDischargeReactivePower_L2              { get; }

        /// <summary>
        /// Rated minimum injected reactive power, defined by max(EV, EVSE), in var on phase L3.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargeReactivePower_L3
        /// </summary>
        [Optional]
        public VoltAmpereReactive?                    MinDischargeReactivePower_L3              { get; }

        /// <summary>
        /// Line voltage supported by EVSE and EV.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVNominalVoltage
        /// </summary>
        [Optional]
        public Volt?                                  NominalVoltage                            { get; }

        /// <summary>
        /// The nominal AC voltage (rms) offset between the Charging Station's electrical
        /// connection point and the utility\u2019s point of common coupling.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVNominalVoltageOffset
        /// </summary>
        [Optional]
        public Volt?                                  NominalVoltageOffset                      { get; }

        /// <summary>
        /// Maximum AC rms voltage, as defined by min(EV, EVSE) to operate with.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumNominalVoltage
        /// </summary>
        [Optional]
        public Volt?                                  MaxNominalVoltage                         { get; }

        /// <summary>
        /// Minimum AC rms voltage, as defined by max(EV, EVSE) to operate with.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumNominalVoltage
        /// </summary>
        [Optional]
        public Volt?                                  MinNominalVoltage                         { get; }

        /// <summary>
        /// Manufacturer of the EV inverter.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterManufacturer
        /// </summary>
        [Optional]
        public String?                                EVInverterManufacturer                    { get; }

        /// <summary>
        /// Model name of the EV inverter.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterModel
        /// </summary>
        [Optional]
        public String?                                EVInverterModel                           { get; }

        /// <summary>
        /// Serial number of the EV inverter.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterSerialNumber
        /// </summary>
        [Optional]
        public String?                                EVInverterSerialNumber                    { get; }

        /// <summary>
        /// Software version of EV inverter.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterSwVersion
        /// </summary>
        [Optional]
        public String?                                EVInverterSWVersion                       { get; }

        /// <summary>
        /// Hardware version of EV inverter.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterHwVersion
        /// </summary>
        [Optional]
        public String?                                EVInverterHWVersion                       { get; }

        /// <summary>
        /// Type of islanding detection method. Only mandatory when islanding detection is
        /// required at the site, as set in the ISO 15118 Service Details configuration.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVIslandingDetectionMethod
        /// </summary>
        [Optional]
        public IEnumerable<IslandingDetectionMethod>  EVIslandingDetectionMethod                { get; }

        /// <summary>
        /// Time after which EV will trip if an island has been detected.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVIslandingTripTime
        /// </summary>
        [Optional]
        public TimeSpan?                              EVIslandingTripTime                       { get; }

        /// <summary>
        /// Maximum injected DC current allowed at level 1 charging.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumLevel1DCInjection
        /// </summary>
        [Optional]
        public Ampere?                                EVMaximumLevel1DCInjection                { get; }

        /// <summary>
        /// Maximum allowed duration of DC injection at level 1 charging.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVDurationLevel1DCInjection
        /// </summary>
        [Optional]
        public TimeSpan?                              EVDurationLevel1DCInjection               { get; }

        /// <summary>
        /// Maximum injected DC current allowed at level 2 charging.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumLevel2DCInjection
        /// </summary>
        [Optional]
        public Ampere?                                EVMaximumLevel2DCInjection                { get; }

        /// <summary>
        /// Maximum allowed duration of DC injection at level 2 charging.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVDurationLevel2DCInjection
        /// </summary>
        [Optional]
        public TimeSpan?                              EVDurationLevel2DCInjection               { get; }

        /// <summary>
        /// Measure of the susceptibility of the circuit to reactance, in Siemens (S).
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVReactiveSusceptance
        /// </summary>
        [Optional]
        public Siemens?                               EVReactiveSusceptance                     { get; }

        /// <summary>
        /// Total energy value, in Wh, that EV is allowed to provide during the entire V2G session.
        /// The value is independent of the V2X Cycling area. Once this value reaches the value
        /// of 0, the EV may block any attempt to discharge in order to protect the battery health.
        /// *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVSessionTotalDischargeEnergyAvailable
        /// </summary>
        [Optional]
        public WattHour?                              EVSessionTotalDischargeEnergyAvailable    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new DER charging parameters.
        /// </summary>
        /// <param name="EVSupportedDERControls">DER control functions supported by EV.</param>
        /// <param name="EVOverExcitedMaxDischargePower">Rated maximum injected active power by EV, at specified over-excited power factor (overExcitedPowerFactor).</param>
        /// <param name="EVOverExcitedPowerFactor">EV power factor when injecting (over excited) the minimum reactive power.</param>
        /// <param name="EVUnderExcitedMaxDischargePower">Rated maximum injected active power by EV supported at specified under-excited power factor (EVUnderExcitedPowerFactor).</param>
        /// <param name="EVUnderExcitedPowerFactor">EV power factor when injecting (under excited) the minimum reactive power.</param>
        /// 
        /// <param name="MaxApparentPower">Rated maximum total apparent power, defined by min(EV, EVSE) in va.</param>
        /// <param name="MaxChargeApparentPower">Rated maximum absorbed apparent power, defined by min(EV, EVSE) in va.</param>
        /// <param name="MaxChargeApparentPower_L2">Rated maximum absorbed apparent power on phase L2, defined by min(EV, EVSE) in va.</param>
        /// <param name="MaxChargeApparentPower_L3">Rated maximum absorbed apparent power on phase L3, defined by min(EV, EVSE) in va.</param>
        /// <param name="MaxDischargeApparentPower">Rated maximum injected apparent power, defined by min(EV, EVSE) in va.</param>
        /// <param name="MaxDischargeApparentPower_L2">Rated maximum injected apparent power on phase L2, defined by min(EV, EVSE) in va.</param>
        /// <param name="MaxDischargeApparentPower_L3">Rated maximum injected apparent power on phase L3, defined by min(EV, EVSE) in va.</param>
        /// 
        /// <param name="MaxChargeReactivePower">Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars.</param>
        /// <param name="MaxChargeReactivePower_L2">Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars on phase L2.</param>
        /// <param name="MaxChargeReactivePower_L3">Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars on phase L3.</param>
        /// <param name="MinChargeReactivePower">Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars.</param>
        /// <param name="MinChargeReactivePower_L2">Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars on phase L2.</param>
        /// <param name="MinChargeReactivePower_L3">Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars on phase L3.</param>
        /// 
        /// <param name="MaxDischargeReactivePower">Rated maximum injected reactive power, defined by min(EV, EVSE), in vars.</param>
        /// <param name="MaxDischargeReactivePower_L2">Rated maximum injected reactive power, defined by min(EV, EVSE), in vars on phase L2.</param>
        /// <param name="MaxDischargeReactivePower_L3">Rated maximum injected reactive power, defined by min(EV, EVSE), in vars on phase L3.</param>
        /// <param name="MinDischargeReactivePower">Rated minimum injected reactive power, defined by max(EV, EVSE), in vars.</param>
        /// <param name="MinDischargeReactivePower_L2">Rated minimum injected reactive power, defined by max(EV, EVSE), in vars on phase L2.</param>
        /// <param name="MinDischargeReactivePower_L3">Rated minimum injected reactive power, defined by max(EV, EVSE), in vars on phase L3.</param>
        /// 
        /// <param name="NominalVoltage">Line voltage supported by EVSE and EV.</param>
        /// <param name="NominalVoltageOffset">The nominal AC voltage (rms) offset between the Charging Station's electrical connection point and the utility\u2019s point of common coupling.</param>
        /// <param name="MaxNominalVoltage">Maximum AC rms voltage, as defined by min(EV, EVSE) to operate with.</param>
        /// <param name="MinNominalVoltage">Minimum AC rms voltage, as defined by max(EV, EVSE) to operate with.</param>
        /// 
        /// <param name="EVInverterManufacturer">Manufacturer of the EV inverter.</param>
        /// <param name="EVInverterModel">Model name of the EV inverter.</param>
        /// <param name="EVInverterSerialNumber">Serial number of the EV inverter.</param>
        /// <param name="EVInverterSWVersion">Software version of EV inverter.</param>
        /// <param name="EVInverterHWVersion">Hardware version of EV inverter.</param>
        /// 
        /// <param name="EVIslandingDetectionMethod">Type of islanding detection method.</param>
        /// <param name="EVIslandingTripTime">Time after which EV will trip if an island has been detected.</param>
        /// <param name="EVMaximumLevel1DCInjection">Maximum injected DC current allowed at level 1 charging.</param>
        /// <param name="EVDurationLevel1DCInjection">Maximum allowed duration of DC injection at level 1 charging.</param>
        /// <param name="EVMaximumLevel2DCInjection">Maximum injected DC current allowed at level 2 charging.</param>
        /// <param name="EVDurationLevel2DCInjection">Maximum injected DC current allowed at level 2 charging.</param>
        /// <param name="EVReactiveSusceptance">Measure of the susceptibility of the circuit to reactance, in Siemens (S).</param>
        /// <param name="EVSessionTotalDischargeEnergyAvailable">Total energy value, in Wh, that EV is allowed to provide during the entire V2G session.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DERChargingParameters(IEnumerable<DERControlType>?            EVSupportedDERControls                   = null,
                                     Watt?                                   EVOverExcitedMaxDischargePower           = null,
                                     Decimal?                                EVOverExcitedPowerFactor                 = null,
                                     Watt?                                   EVUnderExcitedMaxDischargePower          = null,
                                     Decimal?                                EVUnderExcitedPowerFactor                = null,

                                     VoltAmpere?                             MaxApparentPower                         = null,
                                     VoltAmpere?                             MaxChargeApparentPower                   = null,
                                     VoltAmpere?                             MaxChargeApparentPower_L2                = null,
                                     VoltAmpere?                             MaxChargeApparentPower_L3                = null,
                                     VoltAmpere?                             MaxDischargeApparentPower                = null,
                                     VoltAmpere?                             MaxDischargeApparentPower_L2             = null,
                                     VoltAmpere?                             MaxDischargeApparentPower_L3             = null,

                                     VoltAmpereReactive?                     MaxChargeReactivePower                   = null,
                                     VoltAmpereReactive?                     MaxChargeReactivePower_L2                = null,
                                     VoltAmpereReactive?                     MaxChargeReactivePower_L3                = null,
                                     VoltAmpereReactive?                     MinChargeReactivePower                   = null,
                                     VoltAmpereReactive?                     MinChargeReactivePower_L2                = null,
                                     VoltAmpereReactive?                     MinChargeReactivePower_L3                = null,

                                     VoltAmpereReactive?                     MaxDischargeReactivePower                = null,
                                     VoltAmpereReactive?                     MaxDischargeReactivePower_L2             = null,
                                     VoltAmpereReactive?                     MaxDischargeReactivePower_L3             = null,
                                     VoltAmpereReactive?                     MinDischargeReactivePower                = null,
                                     VoltAmpereReactive?                     MinDischargeReactivePower_L2             = null,
                                     VoltAmpereReactive?                     MinDischargeReactivePower_L3             = null,

                                     Volt?                                   NominalVoltage                           = null,
                                     Volt?                                   NominalVoltageOffset                     = null,
                                     Volt?                                   MaxNominalVoltage                        = null,
                                     Volt?                                   MinNominalVoltage                        = null,

                                     String?                                 EVInverterManufacturer                   = null,
                                     String?                                 EVInverterModel                          = null,
                                     String?                                 EVInverterSerialNumber                   = null,
                                     String?                                 EVInverterSWVersion                      = null,
                                     String?                                 EVInverterHWVersion                      = null,

                                     IEnumerable<IslandingDetectionMethod>?  EVIslandingDetectionMethod               = null,
                                     TimeSpan?                               EVIslandingTripTime                      = null,
                                     Ampere?                                 EVMaximumLevel1DCInjection               = null,
                                     TimeSpan?                               EVDurationLevel1DCInjection              = null,
                                     Ampere?                                 EVMaximumLevel2DCInjection               = null,
                                     TimeSpan?                               EVDurationLevel2DCInjection              = null,
                                     Siemens?                                EVReactiveSusceptance                    = null,
                                     WattHour?                               EVSessionTotalDischargeEnergyAvailable   = null,

                                     CustomData?                             CustomData                               = null)

            : base(CustomData)

        {

            this.EVSupportedDERControls                  = EVSupportedDERControls?.Distinct() ?? [];
            this.EVOverExcitedMaxDischargePower          = EVOverExcitedMaxDischargePower;
            this.EVOverExcitedPowerFactor                = EVOverExcitedPowerFactor;
            this.EVUnderExcitedMaxDischargePower         = EVUnderExcitedMaxDischargePower;
            this.EVUnderExcitedPowerFactor               = EVUnderExcitedPowerFactor;

            this.MaxApparentPower                        = MaxApparentPower;
            this.MaxChargeApparentPower                  = MaxChargeApparentPower;
            this.MaxChargeApparentPower_L2               = MaxChargeApparentPower_L2;
            this.MaxChargeApparentPower_L3               = MaxChargeApparentPower_L3;
            this.MaxDischargeApparentPower               = MaxDischargeApparentPower;
            this.MaxDischargeApparentPower_L2            = MaxDischargeApparentPower_L2;
            this.MaxDischargeApparentPower_L3            = MaxDischargeApparentPower_L3;

            this.MaxChargeReactivePower                  = MaxChargeReactivePower;
            this.MaxChargeReactivePower_L2               = MaxChargeReactivePower_L2;
            this.MaxChargeReactivePower_L3               = MaxChargeReactivePower_L3;
            this.MinChargeReactivePower                  = MinChargeReactivePower;
            this.MinChargeReactivePower_L2               = MinChargeReactivePower_L2;
            this.MinChargeReactivePower_L3               = MinChargeReactivePower_L3;

            this.MaxDischargeReactivePower               = MaxDischargeReactivePower;
            this.MaxDischargeReactivePower_L2            = MaxDischargeReactivePower_L2;
            this.MaxDischargeReactivePower_L3            = MaxDischargeReactivePower_L3;
            this.MinDischargeReactivePower               = MinDischargeReactivePower;
            this.MinDischargeReactivePower_L2            = MinDischargeReactivePower_L2;
            this.MinDischargeReactivePower_L3            = MinDischargeReactivePower_L3;

            this.NominalVoltage                          = NominalVoltage;
            this.NominalVoltageOffset                    = NominalVoltageOffset;
            this.MaxNominalVoltage                       = MaxNominalVoltage;
            this.MinNominalVoltage                       = MinNominalVoltage;

            this.EVInverterManufacturer                  = EVInverterManufacturer;
            this.EVInverterModel                         = EVInverterModel;
            this.EVInverterSerialNumber                  = EVInverterSerialNumber;
            this.EVInverterSWVersion                     = EVInverterSWVersion;
            this.EVInverterHWVersion                     = EVInverterHWVersion;

            this.EVIslandingDetectionMethod              = EVIslandingDetectionMethod?.Distinct() ?? [];
            this.EVIslandingTripTime                     = EVIslandingTripTime;
            this.EVMaximumLevel1DCInjection              = EVMaximumLevel1DCInjection;
            this.EVDurationLevel1DCInjection             = EVDurationLevel1DCInjection;
            this.EVMaximumLevel2DCInjection              = EVMaximumLevel2DCInjection;
            this.EVDurationLevel2DCInjection             = EVDurationLevel2DCInjection;
            this.EVReactiveSusceptance                   = EVReactiveSusceptance;
            this.EVSessionTotalDischargeEnergyAvailable  = EVSessionTotalDischargeEnergyAvailable;

            unchecked
            {

                hashCode =  this.EVSupportedDERControls.                 CalcHashCode()       * 191 ^
                           (this.EVOverExcitedMaxDischargePower?.        GetHashCode()  ?? 0) * 181 ^
                           (this.EVOverExcitedPowerFactor?.              GetHashCode()  ?? 0) * 179 ^
                           (this.EVUnderExcitedMaxDischargePower?.       GetHashCode()  ?? 0) * 173 ^
                           (this.EVUnderExcitedPowerFactor?.             GetHashCode()  ?? 0) * 167 ^

                           (this.MaxApparentPower?.                      GetHashCode()  ?? 0) * 163 ^
                           (this.MaxChargeApparentPower?.                GetHashCode()  ?? 0) * 157 ^
                           (this.MaxChargeApparentPower_L2?.             GetHashCode()  ?? 0) * 151 ^
                           (this.MaxChargeApparentPower_L3?.             GetHashCode()  ?? 0) * 149 ^
                           (this.MaxDischargeApparentPower?.             GetHashCode()  ?? 0) * 139 ^
                           (this.MaxDischargeApparentPower_L2?.          GetHashCode()  ?? 0) * 137 ^
                           (this.MaxDischargeApparentPower_L3?.          GetHashCode()  ?? 0) * 131 ^

                           (this.MaxChargeReactivePower?.                GetHashCode()  ?? 0) * 127 ^
                           (this.MaxChargeReactivePower_L2?.             GetHashCode()  ?? 0) * 113 ^
                           (this.MaxChargeReactivePower_L3?.             GetHashCode()  ?? 0) * 109 ^
                           (this.MinChargeReactivePower?.                GetHashCode()  ?? 0) * 107 ^
                           (this.MinChargeReactivePower_L2?.             GetHashCode()  ?? 0) * 103 ^
                           (this.MinChargeReactivePower_L3?.             GetHashCode()  ?? 0) * 101 ^

                           (this.MaxDischargeReactivePower?.             GetHashCode()  ?? 0) *  97 ^
                           (this.MaxDischargeReactivePower_L2?.          GetHashCode()  ?? 0) *  89 ^
                           (this.MaxDischargeReactivePower_L3?.          GetHashCode()  ?? 0) *  83 ^
                           (this.MinDischargeReactivePower?.             GetHashCode()  ?? 0) *  79 ^
                           (this.MinDischargeReactivePower_L2?.          GetHashCode()  ?? 0) *  73 ^
                           (this.MinDischargeReactivePower_L3?.          GetHashCode()  ?? 0) *  71 ^

                           (this.NominalVoltage?.                        GetHashCode()  ?? 0) *  67 ^
                           (this.NominalVoltageOffset?.                  GetHashCode()  ?? 0) *  61 ^
                           (this.MaxNominalVoltage?.                     GetHashCode()  ?? 0) *  59 ^
                           (this.MinNominalVoltage?.                     GetHashCode()  ?? 0) *  53 ^

                           (this.EVInverterManufacturer?.                GetHashCode()  ?? 0) *  47 ^
                           (this.EVInverterModel?.                       GetHashCode()  ?? 0) *  43 ^
                           (this.EVInverterSerialNumber?.                GetHashCode()  ?? 0) *  41 ^
                           (this.EVInverterSWVersion?.                   GetHashCode()  ?? 0) *  37 ^
                           (this.EVInverterHWVersion?.                   GetHashCode()  ?? 0) *  31 ^

                            this.EVIslandingDetectionMethod.             CalcHashCode()       *  29 ^
                           (this.EVIslandingTripTime?.                   GetHashCode()  ?? 0) *  23 ^
                           (this.EVMaximumLevel1DCInjection?.            GetHashCode()  ?? 0) *  17 ^
                           (this.EVDurationLevel1DCInjection?.           GetHashCode()  ?? 0) *  13 ^
                           (this.EVMaximumLevel2DCInjection?.            GetHashCode()  ?? 0) *  11 ^
                           (this.EVDurationLevel2DCInjection?.           GetHashCode()  ?? 0) *   7 ^
                           (this.EVReactiveSusceptance?.                 GetHashCode()  ?? 0) *   5 ^
                           (this.EVSessionTotalDischargeEnergyAvailable?.GetHashCode()  ?? 0) *   3 ^

                            base.                                        GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "DERChargingParametersType is used in ChargingNeedsType during an ISO 15118-20 session for AC_BPT_DER to report the inverter settings related to DER control that were agreed between EVSE and EV.\r\n\r\nFields starting with \"ev\" contain values from the EV.\r\nOther fields contain a value that is supported by both EV and EVSE.\r\n\r\nDERChargingParametersType type is only relevant in case of an ISO 15118-20 AC_BPT_DER/AC_DER charging session.\r\n\r\nNOTE: All these fields have values greater or equal to zero (i.e. are non-negative)",
        //     "javaType": "DERChargingParameters",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "evSupportedDERControl": {
        //             "description": "DER control functions supported by EV. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType:DERControlFunctions (bitmap)",
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/DERControlEnumType"
        //             },
        //             "minItems": 1
        //         },
        //         "evOverExcitedMaxDischargePower": {
        //             "description": "Rated maximum injected active power by EV, at specified over-excited power factor (overExcitedPowerFactor). +\r\nIt can also be defined as the rated maximum discharge power at the rated minimum injected reactive power value. This means that if the EV is providing reactive power support, and it is requested to discharge at max power (e.g. to satisfy an EMS request), the EV may override the request and discharge up to overExcitedMaximumDischargePower to meet the minimum reactive power requirements. +\r\nCorresponds to the WOvPF attribute in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVOverExcitedMaximumDischargePower",
        //             "type": "number"
        //         },
        //         "evOverExcitedPowerFactor": {
        //             "description": "EV power factor when injecting (over excited) the minimum reactive power. +\r\nCorresponds to the OvPF attribute in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVOverExcitedPowerFactor",
        //             "type": "number"
        //         },
        //         "evUnderExcitedMaxDischargePower": {
        //             "description": "Rated maximum injected active power by EV supported at specified under-excited power factor (EVUnderExcitedPowerFactor). +\r\nIt can also be defined as the rated maximum dischargePower at the rated minimum absorbed reactive power value.\r\nThis means that if the EV is providing reactive power support, and it is requested to discharge at max power (e.g. to satisfy an EMS request), the EV may override the request and discharge up to underExcitedMaximumDischargePower to meet the minimum reactive power requirements. +\r\nThis corresponds to the WUnPF attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVUnderExcitedMaximumDischargePower",
        //             "type": "number"
        //         },
        //         "evUnderExcitedPowerFactor": {
        //             "description": "EV power factor when injecting (under excited) the minimum reactive power. +\r\nCorresponds to the OvPF attribute in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVUnderExcitedPowerFactor",
        //             "type": "number"
        //         },
        //         "maxApparentPower": {
        //             "description": "Rated maximum total apparent power, defined by min(EV, EVSE) in va.\r\nCorresponds to the VAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumApparentPower",
        //             "type": "number"
        //         },
        //         "maxChargeApparentPower": {
        //             "description": "Rated maximum absorbed apparent power, defined by min(EV, EVSE) in va. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\n    Corresponds to the ChaVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeApparentPower",
        //             "type": "number"
        //         },
        //         "maxChargeApparentPower_L2": {
        //             "description": "Rated maximum absorbed apparent power on phase L2, defined by min(EV, EVSE) in va.\r\nCorresponds to the ChaVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeApparentPower_L2",
        //             "type": "number"
        //         },
        //         "maxChargeApparentPower_L3": {
        //             "description": "Rated maximum absorbed apparent power on phase L3, defined by min(EV, EVSE) in va.\r\nCorresponds to the ChaVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeApparentPower_L3",
        //             "type": "number"
        //         },
        //         "maxDischargeApparentPower": {
        //             "description": "Rated maximum injected apparent power, defined by min(EV, EVSE) in va. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\n    Corresponds to the DisVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeApparentPower",
        //             "type": "number"
        //         },
        //         "maxDischargeApparentPower_L2": {
        //             "description": "Rated maximum injected apparent power on phase L2, defined by min(EV, EVSE) in va. +\r\n    Corresponds to the DisVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeApparentPower_L2",
        //             "type": "number"
        //         },
        //         "maxDischargeApparentPower_L3": {
        //             "description": "Rated maximum injected apparent power on phase L3, defined by min(EV, EVSE) in va. +\r\n    Corresponds to the DisVAMaxRtg in IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeApparentPower_L3",
        //             "type": "number"
        //         },
        //         "maxChargeReactivePower": {
        //             "description": "Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\nCorresponds to the AvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeReactivePower",
        //             "type": "number"
        //         },
        //         "maxChargeReactivePower_L2": {
        //             "description": "Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars on phase L2. +\r\nCorresponds to the AvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeReactivePower_L2",
        //             "type": "number"
        //         },
        //         "maxChargeReactivePower_L3": {
        //             "description": "Rated maximum absorbed reactive power, defined by min(EV, EVSE), in vars on phase L3. +\r\nCorresponds to the AvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumChargeReactivePower_L3",
        //             "type": "number"
        //         },
        //         "minChargeReactivePower": {
        //             "description": "Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargeReactivePower",
        //             "type": "number"
        //         },
        //         "minChargeReactivePower_L2": {
        //             "description": "Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars on phase L2. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargeReactivePower_L2",
        //             "type": "number"
        //         },
        //         "minChargeReactivePower_L3": {
        //             "description": "Rated minimum absorbed reactive power, defined by max(EV, EVSE), in vars on phase L3. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumChargeReactivePower_L3",
        //             "type": "number"
        //         },
        //         "maxDischargeReactivePower": {
        //             "description": "Rated maximum injected reactive power, defined by min(EV, EVSE), in vars. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\nCorresponds to the IvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeReactivePower",
        //             "type": "number"
        //         },
        //         "maxDischargeReactivePower_L2": {
        //             "description": "Rated maximum injected reactive power, defined by min(EV, EVSE), in vars on phase L2. +\r\nCorresponds to the IvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeReactivePower_L2",
        //             "type": "number"
        //         },
        //         "maxDischargeReactivePower_L3": {
        //             "description": "Rated maximum injected reactive power, defined by min(EV, EVSE), in vars on phase L3. +\r\nCorresponds to the IvarMax attribute in the IEC 61850. +\r\n    *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumDischargeReactivePower_L3",
        //             "type": "number"
        //         },
        //         "minDischargeReactivePower": {
        //             "description": "Rated minimum injected reactive power, defined by max(EV, EVSE), in vars. +\r\n    This field represents the sum of all phases, unless values are provided for L2 and L3,\r\n    in which case this field represents phase L1. +\r\n        *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargeReactivePower",
        //             "type": "number"
        //         },
        //         "minDischargeReactivePower_L2": {
        //             "description": "Rated minimum injected reactive power, defined by max(EV, EVSE), in var on phase L2. +\r\n        *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargeReactivePower_L2",
        //             "type": "number"
        //         },
        //         "minDischargeReactivePower_L3": {
        //             "description": "Rated minimum injected reactive power, defined by max(EV, EVSE), in var on phase L3. +\r\n        *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumDischargeReactivePower_L3",
        //             "type": "number"
        //         },
        //         "nominalVoltage": {
        //             "description": "Line voltage supported by EVSE and EV.\r\n        *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVNominalVoltage",
        //             "type": "number"
        //         },
        //         "nominalVoltageOffset": {
        //             "description": "The nominal AC voltage (rms) offset between the Charging Station's electrical connection point and the utility\u2019s point of common coupling. +\r\n        *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVNominalVoltageOffset",
        //             "type": "number"
        //         },
        //         "maxNominalVoltage": {
        //             "description": "Maximum AC rms voltage, as defined by min(EV, EVSE)  to operate with. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumNominalVoltage",
        //             "type": "number"
        //         },
        //         "minNominalVoltage": {
        //             "description": "Minimum AC rms voltage, as defined by max(EV, EVSE)  to operate with. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMinimumNominalVoltage",
        //             "type": "number"
        //         },
        //         "evInverterManufacturer": {
        //             "description": "Manufacturer of the EV inverter. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterManufacturer",
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "evInverterModel": {
        //             "description": "Model name of the EV inverter. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterModel",
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "evInverterSerialNumber": {
        //             "description": "Serial number of the EV inverter. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterSerialNumber",
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "evInverterSwVersion": {
        //             "description": "Software version of EV inverter. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterSwVersion",
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "evInverterHwVersion": {
        //             "description": "Hardware version of EV inverter. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVInverterHwVersion",
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "evIslandingDetectionMethod": {
        //             "description": "Type of islanding detection method. Only mandatory when islanding detection is required at the site, as set in the ISO 15118 Service Details configuration. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVIslandingDetectionMethod",
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/IslandingDetectionEnumType"
        //             },
        //             "minItems": 1
        //         },
        //         "evIslandingTripTime": {
        //             "description": "Time after which EV will trip if an island has been detected. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVIslandingTripTime",
        //             "type": "number"
        //         },
        //         "evMaximumLevel1DCInjection": {
        //             "description": "Maximum injected DC current allowed at level 1 charging. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumLevel1DCInjection",
        //             "type": "number"
        //         },
        //         "evDurationLevel1DCInjection": {
        //             "description": "Maximum allowed duration of DC injection at level 1 charging. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVDurationLevel1DCInjection",
        //             "type": "number"
        //         },
        //         "evMaximumLevel2DCInjection": {
        //             "description": "Maximum injected DC current allowed at level 2 charging. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVMaximumLevel2DCInjection",
        //             "type": "number"
        //         },
        //         "evDurationLevel2DCInjection": {
        //             "description": "Maximum allowed duration of DC injection at level 2 charging. +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVDurationLevel2DCInjection",
        //             "type": "number"
        //         },
        //         "evReactiveSusceptance": {
        //             "description": "\tMeasure of the susceptibility of the circuit to reactance, in Siemens (S). +\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVReactiveSusceptance",
        //             "type": "number"
        //         },
        //         "evSessionTotalDischargeEnergyAvailable": {
        //             "description": "Total energy value, in Wh, that EV is allowed to provide during the entire V2G session. The value is independent of the V2X Cycling area. Once this value reaches the value of 0, the EV may block any attempt to discharge in order to protect the battery health.\r\n       *ISO 15118-20*: DER_BPT_AC_CPDReqEnergyTransferModeType: EVSessionTotalDischargeEnergyAvailable",
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
        public static DERChargingParameters Parse(JObject                                              JSON,
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

                #region EVSupportedDERControls                    [optional]

                if (JSON.ParseOptionalHashSet("evOverExcitedMaxDischargePower",
                                              "EV over excited max discharge power",
                                              DERControlType.TryParse,
                                              out HashSet<DERControlType> EVSupportedDERControls,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVOverExcitedMaxDischargePower            [optional]

                if (JSON.ParseOptional("evOverExcitedMaxDischargePower",
                                       "EV over excited maximum discharge power",
                                       out Watt? EVOverExcitedMaxDischargePower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVOverExcitedPowerFactor                  [optional]

                if (JSON.ParseOptional("evOverExcitedPowerFactor",
                                       "EV evOverExcitedPowerFactor",
                                       out Decimal? EVOverExcitedPowerFactor,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVUnderExcitedMaxDischargePower           [optional]

                if (JSON.ParseOptional("evUnderExcitedMaxDischargePower",
                                       "EV evUnderExcitedMaxDischargePower",
                                       out Watt? EVUnderExcitedMaxDischargePower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVUnderExcitedPowerFactor                 [optional]

                if (JSON.ParseOptional("evUnderExcitedPowerFactor",
                                       "EV under excited power factor",
                                       out Decimal? EVUnderExcitedPowerFactor,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region MaxApparentPower                          [optional]

                if (JSON.ParseOptional("maxApparentPower",
                                       "maximum apparent power",
                                       out VoltAmpere? MaxApparentPower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxChargeApparentPower                    [optional]

                if (JSON.ParseOptional("maxChargeApparentPower",
                                       "maximum charge apparentPower",
                                       out VoltAmpere? MaxChargeApparentPower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxChargeApparentPower_L2                 [optional]

                if (JSON.ParseOptional("maxChargeApparentPower_L2",
                                       "maximum charge apparentPower L2",
                                       out VoltAmpere? MaxChargeApparentPower_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxChargeApparentPower_L3                 [optional]

                if (JSON.ParseOptional("maxChargeApparentPower_L3",
                                       "maximum charge apparentPower L3",
                                       out VoltAmpere? MaxChargeApparentPower_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxDischargeApparentPower                 [optional]

                if (JSON.ParseOptional("maxDischargeApparentPower",
                                       "maximum discharge apparentPower",
                                       out VoltAmpere? MaxDischargeApparentPower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxDischargeApparentPower_L2              [optional]

                if (JSON.ParseOptional("maxDischargeApparentPower_L2",
                                       "maximum discharge apparentPower L2",
                                       out VoltAmpere? MaxDischargeApparentPower_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxDischargeApparentPower_L3              [optional]

                if (JSON.ParseOptional("maxDischargeApparentPower_L3",
                                       "maximum discharge apparentPower L3",
                                       out VoltAmpere? MaxDischargeApparentPower_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region MaxChargeReactivePower                    [optional]

                if (JSON.ParseOptional("maxChargeReactivePower",
                                       "maximum charge reactivePower",
                                       out VoltAmpereReactive? MaxChargeReactivePower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxChargeReactivePower_L2                 [optional]

                if (JSON.ParseOptional("maxChargeReactivePower_L2",
                                       "maximum charge reactivePower L2",
                                       out VoltAmpereReactive? MaxChargeReactivePower_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxChargeReactivePower_L3                 [optional]

                if (JSON.ParseOptional("maxChargeReactivePower_L3",
                                       "maximum charge reactivePower L3",
                                       out VoltAmpereReactive? MaxChargeReactivePower_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MinChargeReactivePower                    [optional]

                if (JSON.ParseOptional("minChargeReactivePower",
                                       "minimum charge reactivePower",
                                       out VoltAmpereReactive? MinChargeReactivePower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MinChargeReactivePower_L2                 [optional]

                if (JSON.ParseOptional("minChargeReactivePower_L2",
                                       "minimum charge reactivePower L2",
                                       out VoltAmpereReactive? MinChargeReactivePower_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MinChargeReactivePower_L3                 [optional]

                if (JSON.ParseOptional("minChargeReactivePower_L3",
                                       "minimum charge reactivePower L3",
                                       out VoltAmpereReactive? MinChargeReactivePower_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region MaxDischargeReactivePower                 [optional]

                if (JSON.ParseOptional("maxDischargeReactivePower",
                                       "maximum discharge reactivePower",
                                       out VoltAmpereReactive? MaxDischargeReactivePower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxDischargeReactivePower_L2              [optional]

                if (JSON.ParseOptional("maxDischargeReactivePower_L2",
                                       "maximum discharge reactivePower L2",
                                       out VoltAmpereReactive? MaxDischargeReactivePower_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxDischargeReactivePower_L3              [optional]

                if (JSON.ParseOptional("maxDischargeReactivePower_L3",
                                       "maximum discharge reactivePower L3",
                                       out VoltAmpereReactive? MaxDischargeReactivePower_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MinDischargeReactivePower                 [optional]

                if (JSON.ParseOptional("minDischargeReactivePower",
                                       "minimum discharge reactivePower",
                                       out VoltAmpereReactive? MinDischargeReactivePower,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MinDischargeReactivePower_L2              [optional]

                if (JSON.ParseOptional("minDischargeReactivePower_L2",
                                       "minimum discharge reactivePower L2",
                                       out VoltAmpereReactive? MinDischargeReactivePower_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MinDischargeReactivePower_L3              [optional]

                if (JSON.ParseOptional("minDischargeReactivePower_L3",
                                       "minimum discharge reactivePower L3",
                                       out VoltAmpereReactive? MinDischargeReactivePower_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region NominalVoltage                            [optional]

                if (JSON.ParseOptional("nominalVoltage",
                                       "nominal voltage",
                                       out Volt? NominalVoltage,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region NominalVoltageOffset                      [optional]

                if (JSON.ParseOptional("nominalVoltageOffset",
                                       "nominal voltage offset",
                                       out Volt? NominalVoltageOffset,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxNominalVoltage                         [optional]

                if (JSON.ParseOptional("maxNominalVoltage",
                                       "maximum nominal voltage",
                                       out Volt? MaxNominalVoltage,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MinNominalVoltage                         [optional]

                if (JSON.ParseOptional("minNominalVoltage",
                                       "minimum nominal voltage",
                                       out Volt? MinNominalVoltage,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region EVInverterManufacturer                    [optional]

                var EVInverterManufacturer = JSON.GetString("evInverterManufacturer");

                #endregion

                #region EVInverterModel                           [optional]

                var EVInverterModel = JSON.GetString("evInverterModel");

                #endregion

                #region EVInverterSerialNumber                    [optional]

                var EVInverterSerialNumber = JSON.GetString("evInverterSerialNumber");

                #endregion

                #region EVInverterSWVersion                       [optional]

                var EVInverterSWVersion = JSON.GetString("evInverterSwVersion");

                #endregion

                #region EVInverterHWVersion                       [optional]

                var EVInverterHWVersion = JSON.GetString("evInverterHwVersion");

                #endregion


                #region EVIslandingDetectionMethod                [optional]

                if (JSON.ParseOptionalHashSet("evIslandingDetectionMethod",
                                              "EV over excited max discharge power",
                                              IslandingDetectionMethod.TryParse,
                                              out HashSet<IslandingDetectionMethod> EVIslandingDetectionMethod,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVIslandingTripTime                       [optional]

                if (JSON.ParseOptional("evIslandingTripTime",
                                       "EV islanding trip time",
                                       out TimeSpan? EVIslandingTripTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVMaximumLevel1DCInjection                [optional]

                if (JSON.ParseOptional("evMaximumLevel1DCInjection",
                                       "EV maximum level1 DC injection",
                                       out Ampere? EVMaximumLevel1DCInjection,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVDurationLevel1DCInjection               [optional]

                if (JSON.ParseOptional("evDurationLevel1DCInjection",
                                       "EV duration level1 DC injection",
                                       out TimeSpan? EVDurationLevel1DCInjection,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVMaximumLevel2DCInjection                [optional]

                if (JSON.ParseOptional("evMaximumLevel2DCInjection",
                                       "EV maximum level2 DC injection",
                                       out Ampere? EVMaximumLevel2DCInjection,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVDurationLevel2DCInjection               [optional]

                if (JSON.ParseOptional("evDurationLevel2DCInjection",
                                       "EV duration level2 DC injection",
                                       out TimeSpan? EVDurationLevel2DCInjection,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVReactiveSusceptance                     [optional]

                if (JSON.ParseOptional("evReactiveSusceptance",
                                       "EV reactive susceptance",
                                       out Siemens? EVReactiveSusceptance,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVSessionTotalDischargeEnergyAvailable    [optional]

                if (JSON.ParseOptional("evSessionTotalDischargeEnergyAvailable",
                                       "EV session total discharge energy available",
                                       out WattHour? EVSessionTotalDischargeEnergyAvailable,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region CustomData                                [optional]

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

                                            EVSupportedDERControls,
                                            EVOverExcitedMaxDischargePower,
                                            EVOverExcitedPowerFactor,
                                            EVUnderExcitedMaxDischargePower,
                                            EVUnderExcitedPowerFactor,

                                            MaxApparentPower,
                                            MaxChargeApparentPower,
                                            MaxChargeApparentPower_L2,
                                            MaxChargeApparentPower_L3,
                                            MaxDischargeApparentPower,
                                            MaxDischargeApparentPower_L2,
                                            MaxDischargeApparentPower_L3,

                                            MaxChargeReactivePower,
                                            MaxChargeReactivePower_L2,
                                            MaxChargeReactivePower_L3,
                                            MinChargeReactivePower,
                                            MinChargeReactivePower_L2,
                                            MinChargeReactivePower_L3,

                                            MaxDischargeReactivePower,
                                            MaxDischargeReactivePower_L2,
                                            MaxDischargeReactivePower_L3,
                                            MinDischargeReactivePower,
                                            MinDischargeReactivePower_L2,
                                            MinDischargeReactivePower_L3,

                                            NominalVoltage,
                                            NominalVoltageOffset,
                                            MaxNominalVoltage,
                                            MinNominalVoltage,

                                            EVInverterManufacturer,
                                            EVInverterModel,
                                            EVInverterSerialNumber,
                                            EVInverterSWVersion,
                                            EVInverterHWVersion,

                                            EVIslandingDetectionMethod,
                                            EVIslandingTripTime,
                                            EVMaximumLevel1DCInjection,
                                            EVDurationLevel1DCInjection,
                                            EVMaximumLevel2DCInjection,
                                            EVDurationLevel2DCInjection,
                                            EVReactiveSusceptance,
                                            EVSessionTotalDischargeEnergyAvailable,

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
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                           EVSupportedDERControls.Any()
                               ? new JProperty("evSupportedDERControl",                    new JArray(EVSupportedDERControls.Select(evSupportedDERControl => evSupportedDERControl.ToString())))
                               : null,

                           EVOverExcitedMaxDischargePower.        HasValue
                               ? new JProperty("evOverExcitedMaxDischargePower",           EVOverExcitedMaxDischargePower.        Value)
                               : null,

                           EVOverExcitedPowerFactor.              HasValue
                               ? new JProperty("evOverExcitedPowerFactor",                 EVOverExcitedPowerFactor.              Value)
                               : null,

                           EVUnderExcitedMaxDischargePower.       HasValue
                               ? new JProperty("evUnderExcitedMaxDischargePower",          EVUnderExcitedMaxDischargePower.       Value)
                               : null,

                           EVUnderExcitedPowerFactor.             HasValue
                               ? new JProperty("evUnderExcitedPowerFactor",                EVUnderExcitedPowerFactor.             Value)
                               : null,


                           MaxApparentPower.                      HasValue
                               ? new JProperty("maxApparentPower",                         MaxApparentPower.                      Value)
                               : null,

                           MaxChargeApparentPower.                HasValue
                               ? new JProperty("maxChargeApparentPower",                   MaxChargeApparentPower.                Value)
                               : null,

                           MaxChargeApparentPower_L2.             HasValue
                               ? new JProperty("maxChargeApparentPower_L2",                MaxChargeApparentPower_L2.             Value)
                               : null,

                           MaxChargeApparentPower_L3.             HasValue
                               ? new JProperty("maxChargeApparentPower_L3",                MaxChargeApparentPower_L3.             Value)
                               : null,

                           MaxDischargeApparentPower.             HasValue
                               ? new JProperty("maxDischargeApparentPower",                MaxDischargeApparentPower.             Value)
                               : null,

                           MaxDischargeApparentPower_L2.          HasValue
                               ? new JProperty("maxDischargeApparentPower_L2",             MaxDischargeApparentPower_L2.          Value)
                               : null,

                           MaxDischargeApparentPower_L3.          HasValue
                               ? new JProperty("maxDischargeApparentPower_L3",             MaxDischargeApparentPower_L3.          Value)
                               : null,


                           MaxChargeReactivePower.                HasValue
                               ? new JProperty("maxChargeReactivePower",                   MaxChargeReactivePower.                Value)
                               : null,

                           MaxChargeReactivePower_L2.             HasValue
                               ? new JProperty("maxChargeReactivePower_L2",                MaxChargeReactivePower_L2.             Value)
                               : null,

                           MaxChargeReactivePower_L3.             HasValue
                               ? new JProperty("maxChargeReactivePower_L3",                MaxChargeReactivePower_L3.             Value)
                               : null,

                           MinChargeReactivePower.                HasValue
                               ? new JProperty("minChargeReactivePower",                   MinChargeReactivePower.                Value)
                               : null,

                           MinChargeReactivePower_L2.             HasValue
                               ? new JProperty("minChargeReactivePower_L2",                MinChargeReactivePower_L2.             Value)
                               : null,

                           MinChargeReactivePower_L3.             HasValue
                               ? new JProperty("minChargeReactivePower_L3",                MinChargeReactivePower_L3.             Value)
                               : null,


                           MaxDischargeReactivePower.             HasValue
                               ? new JProperty("maxDischargeReactivePower",                MaxDischargeReactivePower.             Value)
                               : null,

                           MaxDischargeReactivePower_L2.          HasValue
                               ? new JProperty("maxDischargeReactivePower_L2",             MaxDischargeReactivePower_L2.          Value)
                               : null,

                           MaxDischargeReactivePower_L3.          HasValue
                               ? new JProperty("maxDischargeReactivePower_L3",             MaxDischargeReactivePower_L3.          Value)
                               : null,

                           MinDischargeReactivePower.             HasValue
                               ? new JProperty("minDischargeReactivePower",                MinDischargeReactivePower.             Value)
                               : null,

                           MinDischargeReactivePower_L2.          HasValue
                               ? new JProperty("minDischargeReactivePower_L2",             MinDischargeReactivePower_L2.          Value)
                               : null,

                           MinDischargeReactivePower_L3.          HasValue
                               ? new JProperty("minDischargeReactivePower_L3",             MinDischargeReactivePower_L3.          Value)
                               : null,


                           NominalVoltage.                        HasValue
                               ? new JProperty("nominalVoltage",                           NominalVoltage.                        Value)
                               : null,

                           NominalVoltageOffset.                  HasValue
                               ? new JProperty("nominalVoltageOffset",                     NominalVoltageOffset.                  Value)
                               : null,

                           MaxNominalVoltage.                     HasValue
                               ? new JProperty("maxNominalVoltage",                        MaxNominalVoltage.                     Value)
                               : null,

                           MinNominalVoltage.                     HasValue
                               ? new JProperty("minNominalVoltage",                        MinNominalVoltage.                     Value)
                               : null,


                           EVInverterManufacturer.IsNotNullOrEmpty()
                               ? new JProperty("evInverterManufacturer",                   EVInverterManufacturer)
                               : null,

                           EVInverterModel.       IsNotNullOrEmpty()
                               ? new JProperty("evInverterModel",                          EVInverterModel)
                               : null,

                           EVInverterSerialNumber.IsNotNullOrEmpty()
                               ? new JProperty("evInverterSerialNumber",                   EVInverterSerialNumber)
                               : null,

                           EVInverterSWVersion.   IsNotNullOrEmpty()
                               ? new JProperty("evInverterSwVersion",                      EVInverterSWVersion)
                               : null,

                           EVInverterHWVersion.   IsNotNullOrEmpty()
                               ? new JProperty("evInverterHwVersion",                      EVInverterHWVersion)
                               : null,


                           EVIslandingDetectionMethod.Any()
                               ? new JProperty("evIslandingDetectionMethod",               new JArray(EVIslandingDetectionMethod.Select(evIslandingDetectionMethod => evIslandingDetectionMethod.ToString())))
                               : null,

                           EVIslandingTripTime.                   HasValue
                               ? new JProperty("evIslandingTripTime",                      EVIslandingTripTime.                   Value)
                               : null,

                           EVMaximumLevel1DCInjection.            HasValue
                               ? new JProperty("evMaximumLevel1DCInjection",               EVMaximumLevel1DCInjection.            Value)
                               : null,

                           EVDurationLevel1DCInjection.           HasValue
                               ? new JProperty("evDurationLevel1DCInjection",              EVDurationLevel1DCInjection.           Value)
                               : null,

                           EVMaximumLevel2DCInjection.            HasValue
                               ? new JProperty("evMaximumLevel2DCInjection",               EVMaximumLevel2DCInjection.            Value)
                               : null,

                           EVDurationLevel2DCInjection.           HasValue
                               ? new JProperty("evDurationLevel2DCInjection",              EVDurationLevel2DCInjection.           Value)
                               : null,

                           EVReactiveSusceptance.                 HasValue
                               ? new JProperty("evReactiveSusceptance",                    EVReactiveSusceptance.                 Value)
                               : null,

                           EVSessionTotalDischargeEnergyAvailable.HasValue
                               ? new JProperty("evSessionTotalDischargeEnergyAvailable",   EVSessionTotalDischargeEnergyAvailable.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                               CustomData.ToJSON(CustomCustomDataSerializer))
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

               EVSupportedDERControls.ToHashSet().    SetEquals(DERChargingParameters.EVSupportedDERControls) &&
              (EVOverExcitedMaxDischargePower?.          Equals(DERChargingParameters.EVOverExcitedMaxDischargePower)         ?? DERChargingParameters.EVOverExcitedMaxDischargePower.        HasValue == false) &&
              (EVOverExcitedPowerFactor?.                Equals(DERChargingParameters.EVOverExcitedPowerFactor)               ?? DERChargingParameters.EVOverExcitedPowerFactor.              HasValue == false) &&
              (EVUnderExcitedMaxDischargePower?.         Equals(DERChargingParameters.EVUnderExcitedMaxDischargePower)        ?? DERChargingParameters.EVUnderExcitedMaxDischargePower.       HasValue == false) &&
              (EVUnderExcitedPowerFactor?.               Equals(DERChargingParameters.EVUnderExcitedPowerFactor)              ?? DERChargingParameters.EVUnderExcitedPowerFactor.             HasValue == false) &&

              (MaxApparentPower?.                        Equals(DERChargingParameters.MaxApparentPower)                       ?? DERChargingParameters.MaxApparentPower.                      HasValue == false) &&
              (MaxChargeApparentPower?.                  Equals(DERChargingParameters.MaxChargeApparentPower)                 ?? DERChargingParameters.MaxChargeApparentPower.                HasValue == false) &&
              (MaxChargeApparentPower_L2?.               Equals(DERChargingParameters.MaxChargeApparentPower_L2)              ?? DERChargingParameters.MaxChargeApparentPower_L2.             HasValue == false) &&
              (MaxChargeApparentPower_L3?.               Equals(DERChargingParameters.MaxChargeApparentPower_L3)              ?? DERChargingParameters.MaxChargeApparentPower_L3.             HasValue == false) &&
              (MaxDischargeApparentPower?.               Equals(DERChargingParameters.MaxDischargeApparentPower)              ?? DERChargingParameters.MaxDischargeApparentPower.             HasValue == false) &&
              (MaxDischargeApparentPower_L2?.            Equals(DERChargingParameters.MaxDischargeApparentPower_L2)           ?? DERChargingParameters.MaxDischargeApparentPower_L2.          HasValue == false) &&
              (MaxDischargeApparentPower_L3?.            Equals(DERChargingParameters.MaxDischargeApparentPower_L3)           ?? DERChargingParameters.MaxDischargeApparentPower_L3.          HasValue == false) &&

              (MaxChargeReactivePower?.                  Equals(DERChargingParameters.MaxChargeReactivePower)                 ?? DERChargingParameters.MaxChargeReactivePower.                HasValue == false) &&
              (MaxChargeReactivePower_L2?.               Equals(DERChargingParameters.MaxChargeReactivePower_L2)              ?? DERChargingParameters.MaxChargeReactivePower_L2.             HasValue == false) &&
              (MaxChargeReactivePower_L3?.               Equals(DERChargingParameters.MaxChargeReactivePower_L3)              ?? DERChargingParameters.MaxChargeReactivePower_L3.             HasValue == false) &&
              (MinChargeReactivePower?.                  Equals(DERChargingParameters.MinChargeReactivePower)                 ?? DERChargingParameters.MinChargeReactivePower.                HasValue == false) &&
              (MinChargeReactivePower_L2?.               Equals(DERChargingParameters.MinChargeReactivePower_L2)              ?? DERChargingParameters.MinChargeReactivePower_L2.             HasValue == false) &&
              (MinChargeReactivePower_L3?.               Equals(DERChargingParameters.MinChargeReactivePower_L3)              ?? DERChargingParameters.MinChargeReactivePower_L3.             HasValue == false) &&

              (MaxDischargeReactivePower?.               Equals(DERChargingParameters.MaxDischargeReactivePower)              ?? DERChargingParameters.MaxDischargeReactivePower.             HasValue == false) &&
              (MaxDischargeReactivePower_L2?.            Equals(DERChargingParameters.MaxDischargeReactivePower_L2)           ?? DERChargingParameters.MaxDischargeReactivePower_L2.          HasValue == false) &&
              (MaxDischargeReactivePower_L3?.            Equals(DERChargingParameters.MaxDischargeReactivePower_L3)           ?? DERChargingParameters.MaxDischargeReactivePower_L3.          HasValue == false) &&
              (MinDischargeReactivePower?.               Equals(DERChargingParameters.MinDischargeReactivePower)              ?? DERChargingParameters.MinDischargeReactivePower.             HasValue == false) &&
              (MinDischargeReactivePower_L2?.            Equals(DERChargingParameters.MinDischargeReactivePower_L2)           ?? DERChargingParameters.MinDischargeReactivePower_L2.          HasValue == false) &&
              (MinDischargeReactivePower_L3?.            Equals(DERChargingParameters.MinDischargeReactivePower_L3)           ?? DERChargingParameters.MinDischargeReactivePower_L3.          HasValue == false) &&

              (NominalVoltage?.                          Equals(DERChargingParameters.NominalVoltage)                         ?? DERChargingParameters.NominalVoltage.                        HasValue == false) &&
              (NominalVoltageOffset?.                    Equals(DERChargingParameters.NominalVoltageOffset)                   ?? DERChargingParameters.NominalVoltageOffset.                  HasValue == false) &&
              (MaxNominalVoltage?.                       Equals(DERChargingParameters.MaxNominalVoltage)                      ?? DERChargingParameters.MaxNominalVoltage.                     HasValue == false) &&
              (MinNominalVoltage?.                       Equals(DERChargingParameters.MinNominalVoltage)                      ?? DERChargingParameters.MinNominalVoltage.                     HasValue == false) &&

              (EVInverterManufacturer?.                  Equals(DERChargingParameters.EVInverterManufacturer)                 ?? DERChargingParameters.EVInverterManufacturer is null) &&
              (EVInverterModel?.                         Equals(DERChargingParameters.EVInverterModel)                        ?? DERChargingParameters.EVInverterModel        is null) &&
              (EVInverterSerialNumber?.                  Equals(DERChargingParameters.EVInverterSerialNumber)                 ?? DERChargingParameters.EVInverterSerialNumber is null) &&
              (EVInverterSWVersion?.                     Equals(DERChargingParameters.EVInverterSWVersion)                    ?? DERChargingParameters.EVInverterSWVersion    is null) &&
              (EVInverterHWVersion?.                     Equals(DERChargingParameters.EVInverterHWVersion)                    ?? DERChargingParameters.EVInverterHWVersion    is null) &&

               EVIslandingDetectionMethod.ToHashSet().SetEquals(DERChargingParameters.EVIslandingDetectionMethod) &&
              (EVIslandingTripTime?.                     Equals(DERChargingParameters.EVIslandingTripTime)                    ?? DERChargingParameters.EVIslandingTripTime.                   HasValue == false) &&
              (EVMaximumLevel1DCInjection?.              Equals(DERChargingParameters.EVMaximumLevel1DCInjection)             ?? DERChargingParameters.EVMaximumLevel1DCInjection.            HasValue == false) &&
              (EVDurationLevel1DCInjection?.             Equals(DERChargingParameters.EVDurationLevel1DCInjection)            ?? DERChargingParameters.EVDurationLevel1DCInjection.           HasValue == false) &&
              (EVMaximumLevel2DCInjection?.              Equals(DERChargingParameters.EVMaximumLevel2DCInjection)             ?? DERChargingParameters.EVMaximumLevel2DCInjection.            HasValue == false) &&
              (EVDurationLevel2DCInjection?.             Equals(DERChargingParameters.EVDurationLevel2DCInjection)            ?? DERChargingParameters.EVDurationLevel2DCInjection.           HasValue == false) &&
              (EVReactiveSusceptance?.                   Equals(DERChargingParameters.EVReactiveSusceptance)                  ?? DERChargingParameters.EVReactiveSusceptance.                 HasValue == false) &&
              (EVSessionTotalDischargeEnergyAvailable?.  Equals(DERChargingParameters.EVSessionTotalDischargeEnergyAvailable) ?? DERChargingParameters.EVSessionTotalDischargeEnergyAvailable.HasValue == false) &&

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

            => new String[] {

                   EVSupportedDERControls.Any()
                       ? $"EV Supported DER Controls: '{EVSupportedDERControls.AggregateWith(", ")}'"
                       : "",

                   EVIslandingDetectionMethod.Any()
                       ? $"EV Islanding Detection Methods: '{EVIslandingDetectionMethod.AggregateWith(", ")}'"
                       : ""

               }.AggregateWith(", ");

        #endregion

    }

}
