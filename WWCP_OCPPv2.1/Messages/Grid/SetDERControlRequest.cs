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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The SetDERControl request.
    /// </summary>
    public class SetDERControlRequest : ARequest<SetDERControlRequest>,
                                        IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/setDERControlRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext          Context
            => DefaultJSONLDContext;

        /// <summary>
        /// True if this is a default Distributed Energy Resource (DER) control.
        /// </summary>
        [Mandatory]
        public Boolean                IsDefault                    { get; }

        /// <summary>
        /// The identification of Distributed Energy Resource (DER) control setting.
        /// </summary>
        [Mandatory]
        public DERControl_Id          ControlId                    { get; }

        /// <summary>
        /// The Distributed Energy Resource (DER) control type.
        /// </summary>
        [Mandatory]
        public DERControlType?        ControlType                  { get; }

        /// <summary>
        /// The optional curve of the Distributed Energy Resource (DER) control.
        /// </summary>
        [Optional]
        public DERCurve?              Curve                        { get; }

        /// <summary>
        /// The optional enter service of the Distributed Energy Resource (DER) control.
        /// </summary>
        [Optional]
        public DEREnterService?       EnterService                 { get; }

        /// <summary>
        /// The optional fixed power factor absorb of the Distributed Energy Resource (DER) control.
        /// </summary>
        [Optional]
        public DERFixedPowerFactor?   FixedPowerFactorAbsorbing    { get; }

        /// <summary>
        /// The optional fixed power factor inject of the Distributed Energy Resource (DER) control.
        /// </summary>
        [Optional]
        public DERFixedPowerFactor?   FixedPowerFactorInjecting    { get; }

        /// <summary>
        /// The optional fixed var of the Distributed Energy Resource (DER) control.
        /// </summary>
        [Optional]
        public DERFixedVAR?           FixedVAR                  { get; }

        /// <summary>
        /// The optional frequency droop of the Distributed Energy Resource (DER) control.
        /// </summary>
        [Optional]
        public DERFrequencyDroop?     FrequencyDroop            { get; }

        /// <summary>
        /// The optional gradient of the Distributed Energy Resource (DER) control.
        /// </summary>
        [Optional]
        public DERGradient?           Gradient                  { get; }

        /// <summary>
        /// The optional limit max discharge of the Distributed Energy Resource (DER) control.
        /// </summary>
        [Optional]
        public DERLimitMaxDischarge?  LimitMaxDischarge         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetDERControl request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="IsDefault">True if this is a default Distributed Energy Resource (DER) control.</param>
        /// <param name="ControlId">The identification of Distributed Energy Resource (DER) control setting.</param>
        /// <param name="ControlType">The Distributed Energy Resource (DER) control type.</param>
        /// <param name="Curve">The optional curve of the Distributed Energy Resource (DER) control.</param>
        /// <param name="EnterService">The optional enter service of the Distributed Energy Resource (DER) control.</param>
        /// <param name="FixedPowerFactorAbsorbing">The optional fixed power factor absorb of the Distributed Energy Resource (DER) control.</param>
        /// <param name="FixedPowerFactorInjecting">The optional fixed power factor inject of the Distributed Energy Resource (DER) control.</param>
        /// <param name="FixedVAR">The optional fixed var of the Distributed Energy Resource (DER) control.</param>
        /// <param name="FrequencyDroop">The optional frequency droop of the Distributed Energy Resource (DER) control.</param>
        /// <param name="Gradient">The optional gradient of the Distributed Energy Resource (DER) control.</param>
        /// <param name="LimitMaxDischarge">The optional limit max discharge of the Distributed Energy Resource (DER) control.</param>
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
        public SetDERControlRequest(SourceRouting            Destination,
                                    Boolean                  IsDefault,
                                    DERControl_Id            ControlId,
                                    DERControlType           ControlType,
                                    DERCurve?                Curve                       = null,
                                    DEREnterService?         EnterService                = null,
                                    DERFixedPowerFactor?     FixedPowerFactorAbsorbing   = null,
                                    DERFixedPowerFactor?     FixedPowerFactorInjecting   = null,
                                    DERFixedVAR?             FixedVAR                    = null,
                                    DERFrequencyDroop?       FrequencyDroop              = null,
                                    DERGradient?             Gradient                    = null,
                                    DERLimitMaxDischarge?    LimitMaxDischarge           = null,

                                    IEnumerable<KeyPair>?    SignKeys                    = null,
                                    IEnumerable<SignInfo>?   SignInfos                   = null,
                                    IEnumerable<Signature>?  Signatures                  = null,

                                    CustomData?              CustomData                  = null,

                                    Request_Id?              RequestId                   = null,
                                    DateTimeOffset?          RequestTimestamp            = null,
                                    TimeSpan?                RequestTimeout              = null,
                                    EventTracking_Id?        EventTrackingId             = null,
                                    NetworkPath?             NetworkPath                 = null,
                                    SerializationFormats?    SerializationFormat         = null,
                                    CancellationToken        CancellationToken           = default)

            : base(Destination,
                   nameof(SetDERControlRequest)[..^7],

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

            this.IsDefault                  = IsDefault;
            this.ControlId                  = ControlId;
            this.ControlType                = ControlType;
            this.Curve                      = Curve;
            this.EnterService               = EnterService;
            this.FixedPowerFactorAbsorbing  = FixedPowerFactorAbsorbing;
            this.FixedPowerFactorInjecting  = FixedPowerFactorInjecting;
            this.FixedVAR                   = FixedVAR;
            this.FrequencyDroop             = FrequencyDroop;
            this.Gradient                   = Gradient;
            this.LimitMaxDischarge          = LimitMaxDischarge;

            unchecked
            {

                hashCode = this.IsDefault.                 GetHashCode()       * 37 ^
                           this.ControlId.                 GetHashCode()       * 31 ^
                           this.ControlType.               GetHashCode()       * 29 ^
                          (this.Curve?.                    GetHashCode() ?? 0) * 23 ^
                          (this.EnterService?.             GetHashCode() ?? 0) * 19 ^
                          (this.FixedPowerFactorAbsorbing?.GetHashCode() ?? 0) * 17 ^
                          (this.FixedPowerFactorInjecting?.GetHashCode() ?? 0) * 13 ^
                          (this.FixedVAR?.                 GetHashCode() ?? 0) * 11 ^
                          (this.FrequencyDroop?.           GetHashCode() ?? 0) *  7 ^
                          (this.Gradient?.                 GetHashCode() ?? 0) *  5 ^
                          (this.LimitMaxDischarge?.        GetHashCode() ?? 0) *  3 ^
                           base.                           GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:SetDERControlRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "DERControlEnumType": {
        //             "description": "Type of control.  Determines which setting field below is used.\r\n\r\n",
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
        //         "DERUnitEnumType": {
        //             "description": "Unit of the setpoint.\r\n",
        //             "javaType": "DERUnitEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Not_Applicable",
        //                 "PctMaxW",
        //                 "PctMaxVar",
        //                 "PctWAvail",
        //                 "PctVarAvail",
        //                 "PctEffectiveV"
        //             ]
        //         },
        //         "PowerDuringCessationEnumType": {
        //             "description": "Parameter is only sent, if the EV has to feed-in power or reactive power during fault-ride through (FRT) as defined by HVMomCess curve and LVMomCess curve.\r\n\r\n\r\n",
        //             "javaType": "PowerDuringCessationEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Active",
        //                 "Reactive"
        //             ]
        //         },
        //         "DERCurvePointsType": {
        //             "javaType": "DERCurvePoints",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "x": {
        //                     "description": "The data value of the X-axis (independent) variable, depending on the curve type.\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "y": {
        //                     "description": "The data value of the Y-axis (dependent) variable, depending on the  &lt;&lt;cmn_derunitenumtype&gt;&gt; of the curve. If _y_ is power factor, then a positive value means DER is absorbing reactive power (under-excited), a negative value when DER is injecting reactive power (over-excited).\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "x",
        //                 "y"
        //             ]
        //         },
        //         "DERCurveType": {
        //             "javaType": "DERCurve",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "curveData": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/DERCurvePointsType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 10
        //                 },
        //                 "hysteresis": {
        //                     "$ref": "#/definitions/HysteresisType"
        //                 },
        //                 "priority": {
        //                     "description": "Priority of curve (0=highest)\r\n\r\n\r\n",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "reactivePowerParams": {
        //                     "$ref": "#/definitions/ReactivePowerParamsType"
        //                 },
        //                 "voltageParams": {
        //                     "$ref": "#/definitions/VoltageParamsType"
        //                 },
        //                 "yUnit": {
        //                     "$ref": "#/definitions/DERUnitEnumType"
        //                 },
        //                 "responseTime": {
        //                     "description": "Open loop response time, the time to ramp up to 90% of the new target in response to the change in voltage, in seconds. A value of 0 is used to mean no limit. When not present, the device should follow its default behavior.\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "startTime": {
        //                     "description": "Point in time when this curve will become activated. Only absent when _default_ is true.\r\n\r\n",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "duration": {
        //                     "description": "Duration in seconds that this curve will be active. Only absent when _default_ is true.\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "priority",
        //                 "yUnit",
        //                 "curveData"
        //             ]
        //         },
        //         "EnterServiceType": {
        //             "javaType": "EnterService",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "priority": {
        //                     "description": "Priority of setting (0=highest)\r\n\r\n",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "highVoltage": {
        //                     "description": "Enter service voltage high\r\n",
        //                     "type": "number"
        //                 },
        //                 "lowVoltage": {
        //                     "description": "Enter service voltage low\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "highFreq": {
        //                     "description": "Enter service frequency high\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "lowFreq": {
        //                     "description": "Enter service frequency low\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "delay": {
        //                     "description": "Enter service delay\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "randomDelay": {
        //                     "description": "Enter service randomized delay\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "rampRate": {
        //                     "description": "Enter service ramp rate in seconds\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "priority",
        //                 "highVoltage",
        //                 "lowVoltage",
        //                 "highFreq",
        //                 "lowFreq"
        //             ]
        //         },
        //         "FixedPFType": {
        //             "javaType": "FixedPF",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "priority": {
        //                     "description": "Priority of setting (0=highest)\r\n",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "displacement": {
        //                     "description": "Power factor, cos(phi), as value between 0..1.\r\n",
        //                     "type": "number"
        //                 },
        //                 "excitation": {
        //                     "description": "True when absorbing reactive power (under-excited), false when injecting reactive power (over-excited).\r\n",
        //                     "type": "boolean"
        //                 },
        //                 "startTime": {
        //                     "description": "Time when this setting becomes active\r\n",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "duration": {
        //                     "description": "Duration in seconds that this setting is active.\r\n",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "priority",
        //                 "displacement",
        //                 "excitation"
        //             ]
        //         },
        //         "FixedVarType": {
        //             "javaType": "FixedVar",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "priority": {
        //                     "description": "Priority of setting (0=highest)\r\n",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "setpoint": {
        //                     "description": "The value specifies a target var output interpreted as a signed percentage (-100 to 100). \r\n    A negative value refers to charging, whereas a positive one refers to discharging.\r\n    The value type is determined by the unit field.\r\n",
        //                     "type": "number"
        //                 },
        //                 "unit": {
        //                     "$ref": "#/definitions/DERUnitEnumType"
        //                 },
        //                 "startTime": {
        //                     "description": "Time when this setting becomes active.\r\n",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "duration": {
        //                     "description": "Duration in seconds that this setting is active.\r\n",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "priority",
        //                 "setpoint",
        //                 "unit"
        //             ]
        //         },
        //         "FreqDroopType": {
        //             "javaType": "FreqDroop",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "priority": {
        //                     "description": "Priority of setting (0=highest)\r\n\r\n\r\n",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "overFreq": {
        //                     "description": "Over-frequency start of droop\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "underFreq": {
        //                     "description": "Under-frequency start of droop\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "overDroop": {
        //                     "description": "Over-frequency droop per unit, oFDroop\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "underDroop": {
        //                     "description": "Under-frequency droop per unit, uFDroop\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "responseTime": {
        //                     "description": "Open loop response time in seconds\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "startTime": {
        //                     "description": "Time when this setting becomes active\r\n\r\n\r\n",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "duration": {
        //                     "description": "Duration in seconds that this setting is active\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "priority",
        //                 "overFreq",
        //                 "underFreq",
        //                 "overDroop",
        //                 "underDroop",
        //                 "responseTime"
        //             ]
        //         },
        //         "GradientType": {
        //             "javaType": "Gradient",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "priority": {
        //                     "description": "Id of setting\r\n\r\n\r\n",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "gradient": {
        //                     "description": "Default ramp rate in seconds (0 if not applicable)\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "softGradient": {
        //                     "description": "Soft-start ramp rate in seconds (0 if not applicable)\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "priority",
        //                 "gradient",
        //                 "softGradient"
        //             ]
        //         },
        //         "HysteresisType": {
        //             "javaType": "Hysteresis",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "hysteresisHigh": {
        //                     "description": "High value for return to normal operation after a grid event, in absolute value. This value adopts the same unit as defined by yUnit\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "hysteresisLow": {
        //                     "description": "Low value for return to normal operation after a grid event, in absolute value. This value adopts the same unit as defined by yUnit\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "hysteresisDelay": {
        //                     "description": "Delay in seconds, once grid parameter within HysteresisLow and HysteresisHigh, for the EV to return to normal operation after a grid event.\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "hysteresisGradient": {
        //                     "description": "Set default rate of change (ramp rate %/s) for the EV to return to normal operation after a grid event\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
        //         },
        //         "LimitMaxDischargeType": {
        //             "javaType": "LimitMaxDischarge",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "priority": {
        //                     "description": "Priority of setting (0=highest)\r\n\r\n\r\n",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "pctMaxDischargePower": {
        //                     "description": "Only for PowerMonitoring. +\r\n    The value specifies a percentage (0 to 100) of the rated maximum discharge power of EV. \r\n    The PowerMonitoring curve becomes active when power exceeds this percentage.\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "powerMonitoringMustTrip": {
        //                     "$ref": "#/definitions/DERCurveType"
        //                 },
        //                 "startTime": {
        //                     "description": "Time when this setting becomes active\r\n\r\n\r\n",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "duration": {
        //                     "description": "Duration in seconds that this setting is active\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "priority"
        //             ]
        //         },
        //         "ReactivePowerParamsType": {
        //             "javaType": "ReactivePowerParams",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "vRef": {
        //                     "description": "Only for VoltVar curve: The nominal ac voltage (rms) adjustment to the voltage curve points for Volt-Var curves (percentage).\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "autonomousVRefEnable": {
        //                     "description": "Only for VoltVar: Enable/disable autonomous VRef adjustment\r\n\r\n\r\n",
        //                     "type": "boolean"
        //                 },
        //                 "autonomousVRefTimeConstant": {
        //                     "description": "Only for VoltVar: Adjustment range for VRef time constant\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
        //         },
        //         "VoltageParamsType": {
        //             "javaType": "VoltageParams",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "hv10MinMeanValue": {
        //                     "description": "EN 50549-1 chapter 4.9.3.4\r\n    Voltage threshold for the 10 min time window mean value monitoring.\r\n    The 10 min mean is recalculated up to every 3 s. \r\n    If the present voltage is above this threshold for more than the time defined by _hv10MinMeanValue_, the EV must trip.\r\n    This value is mandatory if _hv10MinMeanTripDelay_ is set.\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "hv10MinMeanTripDelay": {
        //                     "description": "Time for which the voltage is allowed to stay above the 10 min mean value. \r\n    After this time, the EV must trip.\r\n    This value is mandatory if OverVoltageMeanValue10min is set.\r\n\r\n\r\n",
        //                     "type": "number"
        //                 },
        //                 "powerDuringCessation": {
        //                     "$ref": "#/definitions/PowerDuringCessationEnumType"
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
        //         "isDefault": {
        //             "description": "True if this is a default DER control\r\n\r\n",
        //             "type": "boolean"
        //         },
        //         "controlId": {
        //             "description": "Unique id of this control, e.g. UUID\r\n\r\n",
        //             "type": "string",
        //             "maxLength": 36
        //         },
        //         "controlType": {
        //             "$ref": "#/definitions/DERControlEnumType"
        //         },
        //         "curve": {
        //             "$ref": "#/definitions/DERCurveType"
        //         },
        //         "enterService": {
        //             "$ref": "#/definitions/EnterServiceType"
        //         },
        //         "fixedPFAbsorb": {
        //             "$ref": "#/definitions/FixedPFType"
        //         },
        //         "fixedPFInject": {
        //             "$ref": "#/definitions/FixedPFType"
        //         },
        //         "fixedVar": {
        //             "$ref": "#/definitions/FixedVarType"
        //         },
        //         "freqDroop": {
        //             "$ref": "#/definitions/FreqDroopType"
        //         },
        //         "gradient": {
        //             "$ref": "#/definitions/GradientType"
        //         },
        //         "limitMaxDischarge": {
        //             "$ref": "#/definitions/LimitMaxDischargeType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "isDefault",
        //         "controlId",
        //         "controlType"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SetDERControl request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetDERControlRequestParser">A delegate to parse custom SetDERControl requests.</param>
        public static SetDERControlRequest Parse(JObject                                             JSON,
                                                 Request_Id                                          RequestId,
                                                 SourceRouting                                       Destination,
                                                 NetworkPath                                         NetworkPath,
                                                 DateTimeOffset?                                     RequestTimestamp                   = null,
                                                 TimeSpan?                                           RequestTimeout                     = null,
                                                 EventTracking_Id?                                   EventTrackingId                    = null,
                                                 CustomJObjectParserDelegate<SetDERControlRequest>?  CustomSetDERControlRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var setDERControlRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSetDERControlRequestParser))
            {
                return setDERControlRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetDERControl request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SetDERControlRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SetDERControl request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetDERControlRequest">The parsed SetDERControl request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetDERControlRequestParser">A delegate to parse custom SetDERControl requests.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       Request_Id                                          RequestId,
                                       SourceRouting                                       Destination,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out SetDERControlRequest?      SetDERControlRequest,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       DateTimeOffset?                                     RequestTimestamp                   = null,
                                       TimeSpan?                                           RequestTimeout                     = null,
                                       EventTracking_Id?                                   EventTrackingId                    = null,
                                       CustomJObjectParserDelegate<SetDERControlRequest>?  CustomSetDERControlRequestParser   = null)
        {

            try
            {

                SetDERControlRequest = null;

                #region IsDefault                    [mandatory]

                if (!JSON.ParseMandatory("isDefault",
                                         "is default",
                                         out Boolean IsDefault,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ControlId                    [mandatory]

                if (!JSON.ParseMandatory("controlId",
                                         "control identification",
                                         DERControl_Id.TryParse,
                                         out DERControl_Id ControlId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ControlType                  [mandatory]

                if (!JSON.ParseMandatory("controlType",
                                         "control type",
                                         DERControlType.TryParse,
                                         out DERControlType ControlType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Curve                        [optional]

                if (JSON.ParseOptionalJSON("curve",
                                           "curve",
                                           DERCurve.TryParse,
                                           out DERCurve? Curve,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EnterService                 [optional]

                if (JSON.ParseOptionalJSON("enterService",
                                           "enter service",
                                           DEREnterService.TryParse,
                                           out DEREnterService? EnterService,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region FixedPowerFactorAbsorbing    [optional]

                if (JSON.ParseOptionalJSON("fixedPFAbsorb",
                                           "fixed power factor absorbing",
                                           DERFixedPowerFactor.TryParse,
                                           out DERFixedPowerFactor? FixedPowerFactorAbsorbing,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region FixedPowerFactorInjecting    [optional]

                if (JSON.ParseOptionalJSON("fixedPFInject",
                                           "fixed power factor injecting",
                                           DERFixedPowerFactor.TryParse,
                                           out DERFixedPowerFactor? FixedPowerFactorInjecting,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region FixedVAR                     [optional]

                if (JSON.ParseOptionalJSON("fixedVar",
                                           "fixed VAR",
                                           DERFixedVAR.TryParse,
                                           out DERFixedVAR? FixedVAR,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region FrequencyDroop               [optional]

                if (JSON.ParseOptionalJSON("freqDroop",
                                           "frequency droop",
                                           DERFrequencyDroop.TryParse,
                                           out DERFrequencyDroop? FrequencyDroop,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Gradient                     [optional]

                if (JSON.ParseOptionalJSON("gradient",
                                           "gradient",
                                           DERGradient.TryParse,
                                           out DERGradient? Gradient,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region LimitMaxDischarge            [optional]

                if (JSON.ParseOptionalJSON("limitMaxDischarge",
                                           "limit max discharge",
                                           DERLimitMaxDischarge.TryParse,
                                           out DERLimitMaxDischarge? LimitMaxDischarge,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Signatures                   [optional, OCPP_CSE]

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

                #region CustomData                   [optional]

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


                SetDERControlRequest = new SetDERControlRequest(

                                           Destination,
                                           IsDefault,
                                           ControlId,
                                           ControlType,
                                           Curve,
                                           EnterService,
                                           FixedPowerFactorAbsorbing,
                                           FixedPowerFactorInjecting,
                                           FixedVAR,
                                           FrequencyDroop,
                                           Gradient,
                                           LimitMaxDischarge,

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

                if (CustomSetDERControlRequestParser is not null)
                    SetDERControlRequest = CustomSetDERControlRequestParser(JSON,
                                                                            SetDERControlRequest);

                return true;

            }
            catch (Exception e)
            {
                SetDERControlRequest  = null;
                ErrorResponse         = "The given JSON representation of a SetDERControl request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetDERControlRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetDERControlRequestSerializer">A delegate to serialize custom SetDERControl requests.</param>
        /// <param name="CustomDERCurveSerializer">A delegate to serialize custom DER curve.</param>
        /// <param name="CustomDERCurvePointSerializer">A delegate to serialize custom DER curve point.</param>
        /// <param name="CustomHysteresisSerializer">A delegate to serialize custom hysteresis.</param>
        /// <param name="CustomReactivePowerParametersSerializer">A delegate to serialize custom reactivePowerParameters.</param>
        /// <param name="CustomVoltageParametersSerializer">A delegate to serialize custom reactivePowerParameters.</param>
        /// <param name="CustomDEREnterServiceSerializer">A delegate to serialize custom DEREnterService.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        /// <param name="CustomFixedVARSerializer">A delegate to serialize custom FixedVAR.</param>
        /// <param name="CustomFrequencyDroopSerializer">A delegate to serialize custom FrequencyDroop.</param>
        /// <param name="CustomDERGradientSerializer">A delegate to serialize custom DERGradient.</param>
        /// <param name="CustomDERLimitMaxDischargeSerializer">A delegate to serialize custom DERLimitMaxDischarge JSON objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                    IncludeJSONLDContext                      = false,
                              CustomJObjectSerializerDelegate<SetDERControlRequest>?     CustomSetDERControlRequestSerializer      = null,
                              CustomJObjectSerializerDelegate<DERCurve>?                 CustomDERCurveSerializer                  = null,
                              CustomJObjectSerializerDelegate<DERCurvePoint>?            CustomDERCurvePointSerializer             = null,
                              CustomJObjectSerializerDelegate<Hysteresis>?               CustomHysteresisSerializer                = null,
                              CustomJObjectSerializerDelegate<ReactivePowerParameters>?  CustomReactivePowerParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<VoltageParameters>?        CustomVoltageParametersSerializer         = null,
                              CustomJObjectSerializerDelegate<DEREnterService>?          CustomDEREnterServiceSerializer           = null,
                              CustomJObjectSerializerDelegate<DERFixedPowerFactor>?      CustomFixedPowerFactorSerializer          = null,
                              CustomJObjectSerializerDelegate<DERFixedVAR>?              CustomFixedVARSerializer                  = null,
                              CustomJObjectSerializerDelegate<DERFrequencyDroop>?        CustomFrequencyDroopSerializer            = null,
                              CustomJObjectSerializerDelegate<DERGradient>?              CustomDERGradientSerializer               = null,
                              CustomJObjectSerializerDelegate<DERLimitMaxDischarge>?     CustomDERLimitMaxDischargeSerializer      = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",            DefaultJSONLDContext.     ToString())
                               : null,

                                 new JProperty("isDefault",           IsDefault),
                                 new JProperty("controlId",           ControlId.                ToString()),
                                 new JProperty("controlType",         ControlType.              ToString()),

                           Curve                     is not null
                               ? new JProperty("curve",               Curve.                    ToJSON(CustomDERCurveSerializer,
                                                                                                       CustomDERCurvePointSerializer,
                                                                                                       CustomHysteresisSerializer,
                                                                                                       CustomReactivePowerParametersSerializer,
                                                                                                       CustomVoltageParametersSerializer,
                                                                                                       CustomCustomDataSerializer))
                               : null,

                           EnterService              is not null
                               ? new JProperty("enterService",        EnterService.             ToJSON(CustomDEREnterServiceSerializer,
                                                                                                       CustomCustomDataSerializer))
                               : null,

                           FixedPowerFactorAbsorbing is not null
                               ? new JProperty("fixedPFAbsorb",       FixedPowerFactorAbsorbing.ToJSON(CustomFixedPowerFactorSerializer,
                                                                                                       CustomCustomDataSerializer))
                               : null,

                           FixedPowerFactorInjecting is not null
                               ? new JProperty("fixedPFInject",       FixedPowerFactorInjecting.ToJSON(CustomFixedPowerFactorSerializer,
                                                                                                       CustomCustomDataSerializer))
                               : null,

                           FixedVAR                  is not null
                               ? new JProperty("fixedVar",            FixedVAR.                 ToJSON(CustomFixedVARSerializer,
                                                                                                       CustomCustomDataSerializer))
                               : null,

                           FrequencyDroop            is not null
                               ? new JProperty("freqDroop",           FrequencyDroop.           ToJSON(CustomFrequencyDroopSerializer,
                                                                                                       CustomCustomDataSerializer))
                               : null,

                           Gradient                  is not null
                               ? new JProperty("gradient",            Gradient.                 ToJSON(CustomDERGradientSerializer,
                                                                                                       CustomCustomDataSerializer))
                               : null,

                           LimitMaxDischarge         is not null
                               ? new JProperty("limitMaxDischarge",   LimitMaxDischarge.        ToJSON(CustomDERLimitMaxDischargeSerializer,
                                                                                                       CustomDERCurveSerializer,
                                                                                                       CustomDERCurvePointSerializer,
                                                                                                       CustomHysteresisSerializer,
                                                                                                       CustomReactivePowerParametersSerializer,
                                                                                                       CustomVoltageParametersSerializer,
                                                                                                       CustomCustomDataSerializer))
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.               ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetDERControlRequestSerializer is not null
                       ? CustomSetDERControlRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetDERControlRequest1, SetDERControlRequest2)

        /// <summary>
        /// Compares two SetDERControl requests for equality.
        /// </summary>
        /// <param name="SetDERControlRequest1">A SetDERControl request.</param>
        /// <param name="SetDERControlRequest2">Another SetDERControl request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetDERControlRequest? SetDERControlRequest1,
                                           SetDERControlRequest? SetDERControlRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetDERControlRequest1, SetDERControlRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetDERControlRequest1 is null || SetDERControlRequest2 is null)
                return false;

            return SetDERControlRequest1.Equals(SetDERControlRequest2);

        }

        #endregion

        #region Operator != (SetDERControlRequest1, SetDERControlRequest2)

        /// <summary>
        /// Compares two SetDERControl requests for inequality.
        /// </summary>
        /// <param name="SetDERControlRequest1">A SetDERControl request.</param>
        /// <param name="SetDERControlRequest2">Another SetDERControl request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetDERControlRequest? SetDERControlRequest1,
                                           SetDERControlRequest? SetDERControlRequest2)

            => !(SetDERControlRequest1 == SetDERControlRequest2);

        #endregion

        #endregion

        #region IEquatable<SetDERControlRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetDERControl requests for equality.
        /// </summary>
        /// <param name="Object">A SetDERControl request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetDERControlRequest setDERControlRequest &&
                   Equals(setDERControlRequest);

        #endregion

        #region Equals(SetDERControlRequest)

        /// <summary>
        /// Compares two SetDERControl requests for equality.
        /// </summary>
        /// <param name="SetDERControlRequest">A SetDERControl request to compare with.</param>
        public override Boolean Equals(SetDERControlRequest? SetDERControlRequest)

            => SetDERControlRequest is not null &&

               IsDefault.  Equals(SetDERControlRequest.IsDefault)   &&
               ControlId.  Equals(SetDERControlRequest.ControlId)   &&
               ControlType.Equals(SetDERControlRequest.ControlType) &&

             ((Curve                     is     null && SetDERControlRequest.Curve                     is null) ||
              (Curve                     is not null && SetDERControlRequest.Curve                     is not null && Curve.                    Equals(SetDERControlRequest.Curve)))                     &&

             ((EnterService              is     null && SetDERControlRequest.EnterService              is null) ||
              (EnterService              is not null && SetDERControlRequest.EnterService              is not null && EnterService.             Equals(SetDERControlRequest.EnterService)))              &&

             ((FixedPowerFactorAbsorbing is     null && SetDERControlRequest.FixedPowerFactorAbsorbing is null) ||
              (FixedPowerFactorAbsorbing is not null && SetDERControlRequest.FixedPowerFactorAbsorbing is not null && FixedPowerFactorAbsorbing.Equals(SetDERControlRequest.FixedPowerFactorAbsorbing))) &&

             ((FixedPowerFactorInjecting is     null && SetDERControlRequest.FixedPowerFactorInjecting is null) ||
              (FixedPowerFactorInjecting is not null && SetDERControlRequest.FixedPowerFactorInjecting is not null && FixedPowerFactorInjecting.Equals(SetDERControlRequest.FixedPowerFactorInjecting))) &&

             ((FixedVAR                  is     null && SetDERControlRequest.FixedVAR                  is null) ||
              (FixedVAR                  is not null && SetDERControlRequest.FixedVAR                  is not null && FixedVAR.                 Equals(SetDERControlRequest.FixedVAR)))                  &&

             ((FrequencyDroop            is     null && SetDERControlRequest.FrequencyDroop            is null) ||
              (FrequencyDroop            is not null && SetDERControlRequest.FrequencyDroop            is not null && FrequencyDroop.           Equals(SetDERControlRequest.FrequencyDroop)))            &&

             ((Gradient                  is     null && SetDERControlRequest.Gradient                  is null) ||
              (Gradient                  is not null && SetDERControlRequest.Gradient                  is not null && Gradient.                 Equals(SetDERControlRequest.Gradient)))                  &&

             ((LimitMaxDischarge         is     null && SetDERControlRequest.LimitMaxDischarge         is null) ||
              (LimitMaxDischarge         is not null && SetDERControlRequest.LimitMaxDischarge         is not null && LimitMaxDischarge.        Equals(SetDERControlRequest.LimitMaxDischarge)))         &&

               base.GenericEquals(SetDERControlRequest);

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

            => String.Concat(

                   $"{ControlId} ({ControlType}, {(IsDefault ? "default" : "not default")}): ",

                   Curve is not null
                       ? $", curve: '{Curve}'"
                       : "",

                   EnterService is not null
                       ? $", enter service: '{EnterService}'"
                       : "",

                   FixedPowerFactorAbsorbing is not null
                       ? $", fixedPF absorbing: '{FixedPowerFactorAbsorbing}'"
                       : "",

                   FixedPowerFactorInjecting is not null
                       ? $", fixedPF injecting: '{FixedPowerFactorInjecting}'"
                       : "",

                   FixedVAR is not null
                       ? $", fixed VAR: '{FixedVAR}'"
                       : "",

                   FrequencyDroop is not null
                       ? $", frequency droop: '{FrequencyDroop}'"
                       : "",

                   Gradient is not null
                       ? $", gradient: '{Gradient}'"
                       : "",

                   LimitMaxDischarge is not null
                       ? $", limit max discharge: '{LimitMaxDischarge}'"
                       : ""

                );

        #endregion

    }

}
