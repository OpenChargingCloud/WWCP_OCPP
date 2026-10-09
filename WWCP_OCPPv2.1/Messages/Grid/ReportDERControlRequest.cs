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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The ReportDERControl request.
    /// </summary>
    public class ReportDERControlRequest : ARequest<ReportDERControlRequest>,
                                           IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/reportDERControlRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext          Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The requestId requestId of the correlated GetDERControlRequest.
        /// </summary>
        [Mandatory]
        public Int32                  GetDERControlRequestId       { get; }

        /// <summary>
        /// The reported curves of the Distributed Energy Resource (DER) controls - "curve".
        /// </summary>
        [Optional]
        public IEnumerable<DERControlReport<DERCurve>>              Curves    { get; }

        /// <summary>
        /// The reported enter services of the Distributed Energy Resource (DER) controls - "enterService".
        /// </summary>
        [Optional]
        public IEnumerable<DERControlReport<DEREnterService>>       EnterServices    { get; }

        /// <summary>
        /// The reported fixed power factors absorbing of the Distributed Energy Resource (DER) controls - "fixedPFAbsorb".
        /// </summary>
        [Optional]
        public IEnumerable<DERControlReport<DERFixedPowerFactor>>   FixedPowerFactorsAbsorbing    { get; }

        /// <summary>
        /// The reported fixed power factors injecting of the Distributed Energy Resource (DER) controls - "fixedPFInject".
        /// </summary>
        [Optional]
        public IEnumerable<DERControlReport<DERFixedPowerFactor>>   FixedPowerFactorsInjecting    { get; }

        /// <summary>
        /// The reported fixed vars of the Distributed Energy Resource (DER) controls - "fixedVar".
        /// </summary>
        [Optional]
        public IEnumerable<DERControlReport<DERFixedVAR>>           FixedVARs    { get; }

        /// <summary>
        /// The reported frequency droops of the Distributed Energy Resource (DER) controls - "freqDroop".
        /// </summary>
        [Optional]
        public IEnumerable<DERControlReport<DERFrequencyDroop>>     FrequencyDroops    { get; }

        /// <summary>
        /// The reported gradients of the Distributed Energy Resource (DER) controls - "gradient".
        /// </summary>
        [Optional]
        public IEnumerable<DERControlReport<DERGradient>>           Gradients    { get; }

        /// <summary>
        /// The reported limits of the maximum discharge of the Distributed Energy Resource (DER) controls - "limitMaxDischarge".
        /// </summary>
        [Optional]
        public IEnumerable<DERControlReport<DERLimitMaxDischarge>>  LimitMaxDischarges    { get; }

        /// <summary>
        /// The optional "to be continued" indicator whether another part of the DER control report follows.
        /// Default value when omitted is false.
        /// </summary>
        [Optional]
        public Boolean?               ToBeContinued             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ReportDERControl request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="GetDERControlRequestId">The requestId requestId of the correlated GetDERControlRequest.</param>
        /// <param name="Curves">The reported curves of the DER controls.</param>
        /// <param name="EnterServices">The reported enter services of the DER controls.</param>
        /// <param name="FixedPowerFactorsAbsorbing">The reported fixed power factors absorbing of the DER controls.</param>
        /// <param name="FixedPowerFactorsInjecting">The reported fixed power factors injecting of the DER controls.</param>
        /// <param name="FixedVARs">The reported fixed vars of the DER controls.</param>
        /// <param name="FrequencyDroops">The reported frequency droops of the DER controls.</param>
        /// <param name="Gradients">The reported gradients of the DER controls.</param>
        /// <param name="LimitMaxDischarges">The reported limits of the maximum discharge of the DER controls.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the DER control report follows.</param>
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
        public ReportDERControlRequest(SourceRouting            Destination,
                                       Int32                    GetDERControlRequestId,
                                       IEnumerable<DERControlReport<DERCurve>>?              Curves                     = null,
                                       IEnumerable<DERControlReport<DEREnterService>>?       EnterServices              = null,
                                       IEnumerable<DERControlReport<DERFixedPowerFactor>>?   FixedPowerFactorsAbsorbing = null,
                                       IEnumerable<DERControlReport<DERFixedPowerFactor>>?   FixedPowerFactorsInjecting = null,
                                       IEnumerable<DERControlReport<DERFixedVAR>>?           FixedVARs                  = null,
                                       IEnumerable<DERControlReport<DERFrequencyDroop>>?     FrequencyDroops            = null,
                                       IEnumerable<DERControlReport<DERGradient>>?           Gradients                  = null,
                                       IEnumerable<DERControlReport<DERLimitMaxDischarge>>?  LimitMaxDischarges         = null,
                                       Boolean?                                              ToBeContinued              = null,

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
                   nameof(ReportDERControlRequest)[..^7],

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

            this.GetDERControlRequestId     = GetDERControlRequestId;
            this.Curves                     = Curves?.ToArray() ?? [];
            this.EnterServices              = EnterServices?.ToArray() ?? [];
            this.FixedPowerFactorsAbsorbing = FixedPowerFactorsAbsorbing?.ToArray() ?? [];
            this.FixedPowerFactorsInjecting = FixedPowerFactorsInjecting?.ToArray() ?? [];
            this.FixedVARs                  = FixedVARs?.ToArray() ?? [];
            this.FrequencyDroops            = FrequencyDroops?.ToArray() ?? [];
            this.Gradients                  = Gradients?.ToArray() ?? [];
            this.LimitMaxDischarges         = LimitMaxDischarges?.ToArray() ?? [];
            this.ToBeContinued              = ToBeContinued;

            unchecked
            {

                hashCode = this.GetDERControlRequestId.    GetHashCode()       * 31 ^
                           this.Curves.CalcHashCode() * 29 ^
                           this.EnterServices.CalcHashCode() * 23 ^
                           this.FixedPowerFactorsAbsorbing.CalcHashCode() * 19 ^
                           this.FixedPowerFactorsInjecting.CalcHashCode() * 17 ^
                           this.FixedVARs.CalcHashCode() * 13 ^
                           this.FrequencyDroops.CalcHashCode() * 11 ^
                           this.Gradients.CalcHashCode() * 7 ^
                           this.LimitMaxDischarges.CalcHashCode() * 5 ^
                          (this.ToBeContinued?.            GetHashCode() ?? 0) *  3 ^
                           base.                           GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:ReportDERControlRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "DERControlEnumType": {
        //             "description": "Type of DER curve\r\n\r\n",
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
        //             "description": "Unit of the Y-axis of DER curve\r\n",
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
        //         "DERCurveGetType": {
        //             "javaType": "DERCurveGet",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "curve": {
        //                     "$ref": "#/definitions/DERCurveType"
        //                 },
        //                 "id": {
        //                     "description": "Id of DER curve\r\n\r\n",
        //                     "type": "string",
        //                     "maxLength": 36
        //                 },
        //                 "curveType": {
        //                     "$ref": "#/definitions/DERControlEnumType"
        //                 },
        //                 "isDefault": {
        //                     "description": "True if this is a default curve\r\n\r\n",
        //                     "type": "boolean"
        //                 },
        //                 "isSuperseded": {
        //                     "description": "True if this setting is superseded by a higher priority setting (i.e. lower value of _priority_)\r\n\r\n",
        //                     "type": "boolean"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "id",
        //                 "curveType",
        //                 "isDefault",
        //                 "isSuperseded",
        //                 "curve"
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
        //         "EnterServiceGetType": {
        //             "javaType": "EnterServiceGet",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "enterService": {
        //                     "$ref": "#/definitions/EnterServiceType"
        //                 },
        //                 "id": {
        //                     "description": "Id of setting\r\n\r\n",
        //                     "type": "string",
        //                     "maxLength": 36
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "id",
        //                 "enterService"
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
        //         "FixedPFGetType": {
        //             "javaType": "FixedPFGet",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "fixedPF": {
        //                     "$ref": "#/definitions/FixedPFType"
        //                 },
        //                 "id": {
        //                     "description": "Id of setting.\r\n",
        //                     "type": "string",
        //                     "maxLength": 36
        //                 },
        //                 "isDefault": {
        //                     "description": "True if setting is a default control.\r\n",
        //                     "type": "boolean"
        //                 },
        //                 "isSuperseded": {
        //                     "description": "True if this setting is superseded by a lower priority setting.\r\n",
        //                     "type": "boolean"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "id",
        //                 "isDefault",
        //                 "isSuperseded",
        //                 "fixedPF"
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
        //         "FixedVarGetType": {
        //             "javaType": "FixedVarGet",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "fixedVar": {
        //                     "$ref": "#/definitions/FixedVarType"
        //                 },
        //                 "id": {
        //                     "description": "Id of setting\r\n\r\n",
        //                     "type": "string",
        //                     "maxLength": 36
        //                 },
        //                 "isDefault": {
        //                     "description": "True if setting is a default control.\r\n",
        //                     "type": "boolean"
        //                 },
        //                 "isSuperseded": {
        //                     "description": "True if this setting is superseded by a lower priority setting\r\n\r\n",
        //                     "type": "boolean"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "id",
        //                 "isDefault",
        //                 "isSuperseded",
        //                 "fixedVar"
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
        //         "FreqDroopGetType": {
        //             "javaType": "FreqDroopGet",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "freqDroop": {
        //                     "$ref": "#/definitions/FreqDroopType"
        //                 },
        //                 "id": {
        //                     "description": "Id of setting\r\n\r\n",
        //                     "type": "string",
        //                     "maxLength": 36
        //                 },
        //                 "isDefault": {
        //                     "description": "True if setting is a default control.\r\n",
        //                     "type": "boolean"
        //                 },
        //                 "isSuperseded": {
        //                     "description": "True if this setting is superseded by a higher priority setting (i.e. lower value of _priority_)\r\n\r\n",
        //                     "type": "boolean"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "id",
        //                 "isDefault",
        //                 "isSuperseded",
        //                 "freqDroop"
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
        //         "GradientGetType": {
        //             "javaType": "GradientGet",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "gradient": {
        //                     "$ref": "#/definitions/GradientType"
        //                 },
        //                 "id": {
        //                     "description": "Id of setting\r\n\r\n",
        //                     "type": "string",
        //                     "maxLength": 36
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "id",
        //                 "gradient"
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
        //         "LimitMaxDischargeGetType": {
        //             "javaType": "LimitMaxDischargeGet",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "id": {
        //                     "description": "Id of setting\r\n\r\n",
        //                     "type": "string",
        //                     "maxLength": 36
        //                 },
        //                 "isDefault": {
        //                     "description": "True if setting is a default control.\r\n",
        //                     "type": "boolean"
        //                 },
        //                 "isSuperseded": {
        //                     "description": "True if this setting is superseded by a higher priority setting (i.e. lower value of _priority_)\r\n\r\n",
        //                     "type": "boolean"
        //                 },
        //                 "limitMaxDischarge": {
        //                     "$ref": "#/definitions/LimitMaxDischargeType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "id",
        //                 "isDefault",
        //                 "isSuperseded",
        //                 "limitMaxDischarge"
        //             ]
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
        //         "curve": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/DERCurveGetType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 24
        //         },
        //         "enterService": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/EnterServiceGetType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 24
        //         },
        //         "fixedPFAbsorb": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/FixedPFGetType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 24
        //         },
        //         "fixedPFInject": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/FixedPFGetType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 24
        //         },
        //         "fixedVar": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/FixedVarGetType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 24
        //         },
        //         "freqDroop": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/FreqDroopGetType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 24
        //         },
        //         "gradient": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/GradientGetType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 24
        //         },
        //         "limitMaxDischarge": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/LimitMaxDischargeGetType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 24
        //         },
        //         "requestId": {
        //             "description": "RequestId from GetDERControlRequest.\r\n",
        //             "type": "integer"
        //         },
        //         "tbc": {
        //             "description": "To Be Continued. Default value when omitted: false. +\r\nFalse indicates that there are no further messages as part of this report.\r\n",
        //             "type": "boolean"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "requestId"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a ReportDERControl request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomReportDERControlRequestParser">A delegate to parse custom ReportDERControl requests.</param>
        public static ReportDERControlRequest Parse(JObject                                                JSON,
                                                    Request_Id                                             RequestId,
                                                    SourceRouting                                          Destination,
                                                    NetworkPath                                            NetworkPath,
                                                    DateTimeOffset?                                        RequestTimestamp                      = null,
                                                    TimeSpan?                                              RequestTimeout                        = null,
                                                    EventTracking_Id?                                      EventTrackingId                       = null,
                                                    CustomJObjectParserDelegate<ReportDERControlRequest>?  CustomReportDERControlRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var reportDERControlRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomReportDERControlRequestParser))
            {
                return reportDERControlRequest;
            }

            throw new ArgumentException("The given JSON representation of a ReportDERControl request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out ReportDERControlRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a ReportDERControl request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ReportDERControlRequest">The parsed ReportDERControl request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomReportDERControlRequestParser">A delegate to parse custom ReportDERControl requests.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       Request_Id                                             RequestId,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out ReportDERControlRequest?      ReportDERControlRequest,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       DateTimeOffset?                                        RequestTimestamp                      = null,
                                       TimeSpan?                                              RequestTimeout                        = null,
                                       EventTracking_Id?                                      EventTrackingId                       = null,
                                       CustomJObjectParserDelegate<ReportDERControlRequest>?  CustomReportDERControlRequestParser   = null)
        {

            try
            {

                ReportDERControlRequest = null;

                #region GetDERControlRequestId       [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "request identification",
                                         out Int32 GetDERControlRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Curves                       [optional]

                if (!DERControlReport<DERCurve>.TryParseList(JSON,
                                                    "curve",
                                                    "curve",
                                                    DERCurve.TryParse,
                                                    out var Curves,
                                                    out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EnterServices                [optional]

                if (!DERControlReport<DEREnterService>.TryParseList(JSON,
                                                    "enterService",
                                                    "enterService",
                                                    DEREnterService.TryParse,
                                                    out var EnterServices,
                                                    out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region FixedPowerFactorsAbsorbing   [optional]

                if (!DERControlReport<DERFixedPowerFactor>.TryParseList(JSON,
                                                    "fixedPFAbsorb",
                                                    "fixedPF",
                                                    DERFixedPowerFactor.TryParse,
                                                    out var FixedPowerFactorsAbsorbing,
                                                    out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region FixedPowerFactorsInjecting   [optional]

                if (!DERControlReport<DERFixedPowerFactor>.TryParseList(JSON,
                                                    "fixedPFInject",
                                                    "fixedPF",
                                                    DERFixedPowerFactor.TryParse,
                                                    out var FixedPowerFactorsInjecting,
                                                    out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region FixedVARs                    [optional]

                if (!DERControlReport<DERFixedVAR>.TryParseList(JSON,
                                                    "fixedVar",
                                                    "fixedVar",
                                                    DERFixedVAR.TryParse,
                                                    out var FixedVARs,
                                                    out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region FrequencyDroops              [optional]

                if (!DERControlReport<DERFrequencyDroop>.TryParseList(JSON,
                                                    "freqDroop",
                                                    "freqDroop",
                                                    DERFrequencyDroop.TryParse,
                                                    out var FrequencyDroops,
                                                    out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Gradients                    [optional]

                if (!DERControlReport<DERGradient>.TryParseList(JSON,
                                                    "gradient",
                                                    "gradient",
                                                    DERGradient.TryParse,
                                                    out var Gradients,
                                                    out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region LimitMaxDischarges           [optional]

                if (!DERControlReport<DERLimitMaxDischarge>.TryParseList(JSON,
                                                    "limitMaxDischarge",
                                                    "limitMaxDischarge",
                                                    DERLimitMaxDischarge.TryParse,
                                                    out var LimitMaxDischarges,
                                                    out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ToBeContinued                [optional]

                if (JSON.ParseOptional("tbc",
                                       "to be continued",
                                       out Boolean? ToBeContinued,
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


                ReportDERControlRequest = new ReportDERControlRequest(

                                              Destination,
                                              GetDERControlRequestId,
                                              Curves,
                                              EnterServices,
                                              FixedPowerFactorsAbsorbing,
                                              FixedPowerFactorsInjecting,
                                              FixedVARs,
                                              FrequencyDroops,
                                              Gradients,
                                              LimitMaxDischarges,
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

                if (CustomReportDERControlRequestParser is not null)
                    ReportDERControlRequest = CustomReportDERControlRequestParser(JSON,
                                                                                  ReportDERControlRequest);

                return true;

            }
            catch (Exception e)
            {
                ReportDERControlRequest  = null;
                ErrorResponse            = "The given JSON representation of a ReportDERControl request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomReportDERControlRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomReportDERControlRequestSerializer">A delegate to serialize custom ReportDERControl requests.</param>
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
                              CustomJObjectSerializerDelegate<ReportDERControlRequest>?  CustomReportDERControlRequestSerializer   = null,
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

                                 new JProperty("requestId",           GetDERControlRequestId),

                           Curves.Any()
                               ? new JProperty("curve",               new JArray(Curves.Select(report => report.ToJSON("curve", control => control.ToJSON(CustomDERCurveSerializer, CustomDERCurvePointSerializer, CustomHysteresisSerializer, CustomReactivePowerParametersSerializer, CustomVoltageParametersSerializer, CustomCustomDataSerializer), CustomCustomDataSerializer))))
                               : null,

                           EnterServices.Any()
                               ? new JProperty("enterService",        new JArray(EnterServices.Select(report => report.ToJSON("enterService", control => control.ToJSON(CustomDEREnterServiceSerializer, CustomCustomDataSerializer), CustomCustomDataSerializer))))
                               : null,

                           FixedPowerFactorsAbsorbing.Any()
                               ? new JProperty("fixedPFAbsorb",       new JArray(FixedPowerFactorsAbsorbing.Select(report => report.ToJSON("fixedPF", control => control.ToJSON(CustomFixedPowerFactorSerializer, CustomCustomDataSerializer), CustomCustomDataSerializer))))
                               : null,

                           FixedPowerFactorsInjecting.Any()
                               ? new JProperty("fixedPFInject",       new JArray(FixedPowerFactorsInjecting.Select(report => report.ToJSON("fixedPF", control => control.ToJSON(CustomFixedPowerFactorSerializer, CustomCustomDataSerializer), CustomCustomDataSerializer))))
                               : null,

                           FixedVARs.Any()
                               ? new JProperty("fixedVar",            new JArray(FixedVARs.Select(report => report.ToJSON("fixedVar", control => control.ToJSON(CustomFixedVARSerializer, CustomCustomDataSerializer), CustomCustomDataSerializer))))
                               : null,

                           FrequencyDroops.Any()
                               ? new JProperty("freqDroop",           new JArray(FrequencyDroops.Select(report => report.ToJSON("freqDroop", control => control.ToJSON(CustomFrequencyDroopSerializer, CustomCustomDataSerializer), CustomCustomDataSerializer))))
                               : null,

                           Gradients.Any()
                               ? new JProperty("gradient",            new JArray(Gradients.Select(report => report.ToJSON("gradient", control => control.ToJSON(CustomDERGradientSerializer, CustomCustomDataSerializer), CustomCustomDataSerializer))))
                               : null,

                           LimitMaxDischarges.Any()
                               ? new JProperty("limitMaxDischarge",   new JArray(LimitMaxDischarges.Select(report => report.ToJSON("limitMaxDischarge", control => control.ToJSON(CustomDERLimitMaxDischargeSerializer, CustomDERCurveSerializer, CustomDERCurvePointSerializer, CustomHysteresisSerializer, CustomReactivePowerParametersSerializer, CustomVoltageParametersSerializer, CustomCustomDataSerializer), CustomCustomDataSerializer))))
                               : null,

                           ToBeContinued.HasValue
                               ? new JProperty("tbc",                 ToBeContinued.Value)
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.               ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomReportDERControlRequestSerializer is not null
                       ? CustomReportDERControlRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, ..., out ReportDERControlRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a ReportDERControl request - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="ReportDERControlRequest">The ReportDERControl request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomReportDERControlRequestParser">A delegate to read custom ReportDERControl requests.</param>
        public static Boolean TryParseCBOR(CBORValue                                           CBOR,
                                           Request_Id                                          RequestId,
                                           SourceRouting                                       Destination,
                                           NetworkPath                                         NetworkPath,
                                           [NotNullWhen(true)]  out ReportDERControlRequest?   ReportDERControlRequest,
                                           [NotNullWhen(false)] out String?                    ErrorResponse,
                                           DateTimeOffset?                                     RequestTimestamp                      = null,
                                           TimeSpan?                                           RequestTimeout                        = null,
                                           EventTracking_Id?                                   EventTrackingId                       = null,
                                           CustomCBORParserDelegate<ReportDERControlRequest>?  CustomReportDERControlRequestParser   = null)
        {

            try
            {

                ReportDERControlRequest = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a ReportDERControl request is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryInt64("requestId",
                                               "request identification",
                                               out var GetDERControlRequestIdNumber,
                                               out ErrorResponse))
                {
                    return false;
                }

                if (GetDERControlRequestIdNumber < Int32.MinValue || GetDERControlRequestIdNumber > Int32.MaxValue)
                {
                    ErrorResponse = $"Invalid request identification '{GetDERControlRequestIdNumber}'!";
                    return false;
                }

                var GetDERControlRequestId = (Int32) GetDERControlRequestIdNumber;

                CBOR.ParseOptionalList<DERControlReport<DERCurve>>("curve",
                                                     "curve",
                                                     (CBORValue item, out DERControlReport<DERCurve>? report, out String? errorResponse)
                                                         => DERControlReport<DERCurve>.TryParseCBOR(item, "curve", OCPPv2_1.DERCurve.TryParseCBOR, out report, out errorResponse),
                                                     out var Curves,
                                                     out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<DERControlReport<DEREnterService>>("enterService",
                                                     "enterService",
                                                     (CBORValue item, out DERControlReport<DEREnterService>? report, out String? errorResponse)
                                                         => DERControlReport<DEREnterService>.TryParseCBOR(item, "enterService", OCPPv2_1.DEREnterService.TryParseCBOR, out report, out errorResponse),
                                                     out var EnterServices,
                                                     out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<DERControlReport<DERFixedPowerFactor>>("fixedPFAbsorb",
                                                     "fixedPFAbsorb",
                                                     (CBORValue item, out DERControlReport<DERFixedPowerFactor>? report, out String? errorResponse)
                                                         => DERControlReport<DERFixedPowerFactor>.TryParseCBOR(item, "fixedPF", OCPPv2_1.DERFixedPowerFactor.TryParseCBOR, out report, out errorResponse),
                                                     out var FixedPowerFactorsAbsorbing,
                                                     out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<DERControlReport<DERFixedPowerFactor>>("fixedPFInject",
                                                     "fixedPFInject",
                                                     (CBORValue item, out DERControlReport<DERFixedPowerFactor>? report, out String? errorResponse)
                                                         => DERControlReport<DERFixedPowerFactor>.TryParseCBOR(item, "fixedPF", OCPPv2_1.DERFixedPowerFactor.TryParseCBOR, out report, out errorResponse),
                                                     out var FixedPowerFactorsInjecting,
                                                     out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<DERControlReport<DERFixedVAR>>("fixedVar",
                                                     "fixedVar",
                                                     (CBORValue item, out DERControlReport<DERFixedVAR>? report, out String? errorResponse)
                                                         => DERControlReport<DERFixedVAR>.TryParseCBOR(item, "fixedVar", OCPPv2_1.DERFixedVAR.TryParseCBOR, out report, out errorResponse),
                                                     out var FixedVARs,
                                                     out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<DERControlReport<DERFrequencyDroop>>("freqDroop",
                                                     "freqDroop",
                                                     (CBORValue item, out DERControlReport<DERFrequencyDroop>? report, out String? errorResponse)
                                                         => DERControlReport<DERFrequencyDroop>.TryParseCBOR(item, "freqDroop", OCPPv2_1.DERFrequencyDroop.TryParseCBOR, out report, out errorResponse),
                                                     out var FrequencyDroops,
                                                     out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<DERControlReport<DERGradient>>("gradient",
                                                     "gradient",
                                                     (CBORValue item, out DERControlReport<DERGradient>? report, out String? errorResponse)
                                                         => DERControlReport<DERGradient>.TryParseCBOR(item, "gradient", OCPPv2_1.DERGradient.TryParseCBOR, out report, out errorResponse),
                                                     out var Gradients,
                                                     out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<DERControlReport<DERLimitMaxDischarge>>("limitMaxDischarge",
                                                     "limitMaxDischarge",
                                                     (CBORValue item, out DERControlReport<DERLimitMaxDischarge>? report, out String? errorResponse)
                                                         => DERControlReport<DERLimitMaxDischarge>.TryParseCBOR(item, "limitMaxDischarge", OCPPv2_1.DERLimitMaxDischarge.TryParseCBOR, out report, out errorResponse),
                                                     out var LimitMaxDischarges,
                                                     out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalBoolean("tbc",
                                       "to be continued",
                                       out var ToBeContinued,
                                       out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<Signature>("signatures",
                                               "cryptographic signatures",
                                               OCPPCBORExtensions.TryParseSignature,
                                               out var Signatures,
                                               out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;


                ReportDERControlRequest = new ReportDERControlRequest(

                                              Destination,
                                              GetDERControlRequestId,
                                              Curves,
                                              EnterServices,
                                              FixedPowerFactorsAbsorbing,
                                              FixedPowerFactorsInjecting,
                                              FixedVARs,
                                              FrequencyDroops,
                                              Gradients,
                                              LimitMaxDischarges,
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

                if (CustomReportDERControlRequestParser is not null)
                    ReportDERControlRequest = CustomReportDERControlRequestParser(CBOR,
                                                                                 ReportDERControlRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ReportDERControlRequest = null;
                ErrorResponse = "The given CBOR representation of a ReportDERControl request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomReportDERControlRequestSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this ReportDERControl request: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomReportDERControlRequestSerializer">A delegate to serialize custom ReportDERControl requests.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<ReportDERControlRequest>? CustomReportDERControlRequestSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("requestId",                 CBORValue.FromInt64(GetDERControlRequestId)),
                           ("curve",                     OCPPCBORExtensions.Array(Curves, report => report.ToCBOR("curve", control => control.ToCBOR()))),
                           ("enterService",              OCPPCBORExtensions.Array(EnterServices, report => report.ToCBOR("enterService", control => control.ToCBOR()))),
                           ("fixedPFAbsorb",             OCPPCBORExtensions.Array(FixedPowerFactorsAbsorbing, report => report.ToCBOR("fixedPF", control => control.ToCBOR()))),
                           ("fixedPFInject",             OCPPCBORExtensions.Array(FixedPowerFactorsInjecting, report => report.ToCBOR("fixedPF", control => control.ToCBOR()))),
                           ("fixedVar",                  OCPPCBORExtensions.Array(FixedVARs, report => report.ToCBOR("fixedVar", control => control.ToCBOR()))),
                           ("freqDroop",                 OCPPCBORExtensions.Array(FrequencyDroops, report => report.ToCBOR("freqDroop", control => control.ToCBOR()))),
                           ("gradient",                  OCPPCBORExtensions.Array(Gradients, report => report.ToCBOR("gradient", control => control.ToCBOR()))),
                           ("limitMaxDischarge",         OCPPCBORExtensions.Array(LimitMaxDischarges, report => report.ToCBOR("limitMaxDischarge", control => control.ToCBOR()))),
                           ("tbc",                       OCPPCBORExtensions.Flag(ToBeContinued)),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomReportDERControlRequestSerializer is not null
                       ? CustomReportDERControlRequestSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ReportDERControlRequest1, ReportDERControlRequest2)

        /// <summary>
        /// Compares two ReportDERControl requests for equality.
        /// </summary>
        /// <param name="ReportDERControlRequest1">A ReportDERControl request.</param>
        /// <param name="ReportDERControlRequest2">Another ReportDERControl request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ReportDERControlRequest? ReportDERControlRequest1,
                                           ReportDERControlRequest? ReportDERControlRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ReportDERControlRequest1, ReportDERControlRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ReportDERControlRequest1 is null || ReportDERControlRequest2 is null)
                return false;

            return ReportDERControlRequest1.Equals(ReportDERControlRequest2);

        }

        #endregion

        #region Operator != (ReportDERControlRequest1, ReportDERControlRequest2)

        /// <summary>
        /// Compares two ReportDERControl requests for inequality.
        /// </summary>
        /// <param name="ReportDERControlRequest1">A ReportDERControl request.</param>
        /// <param name="ReportDERControlRequest2">Another ReportDERControl request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ReportDERControlRequest? ReportDERControlRequest1,
                                           ReportDERControlRequest? ReportDERControlRequest2)

            => !(ReportDERControlRequest1 == ReportDERControlRequest2);

        #endregion

        #endregion

        #region IEquatable<ReportDERControlRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ReportDERControl requests for equality.
        /// </summary>
        /// <param name="Object">A ReportDERControl request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ReportDERControlRequest reportDERControlRequest &&
                   Equals(reportDERControlRequest);

        #endregion

        #region Equals(ReportDERControlRequest)

        /// <summary>
        /// Compares two ReportDERControl requests for equality.
        /// </summary>
        /// <param name="ReportDERControlRequest">A ReportDERControl request to compare with.</param>
        public override Boolean Equals(ReportDERControlRequest? ReportDERControlRequest)

            => ReportDERControlRequest is not null &&

               GetDERControlRequestId.Equals(ReportDERControlRequest.GetDERControlRequestId) &&

               Curves.SequenceEqual(ReportDERControlRequest.Curves) &&

               EnterServices.SequenceEqual(ReportDERControlRequest.EnterServices) &&

               FixedPowerFactorsAbsorbing.SequenceEqual(ReportDERControlRequest.FixedPowerFactorsAbsorbing) &&

               FixedPowerFactorsInjecting.SequenceEqual(ReportDERControlRequest.FixedPowerFactorsInjecting) &&

               FixedVARs.SequenceEqual(ReportDERControlRequest.FixedVARs) &&

               FrequencyDroops.SequenceEqual(ReportDERControlRequest.FrequencyDroops) &&

               Gradients.SequenceEqual(ReportDERControlRequest.Gradients) &&

               LimitMaxDischarges.SequenceEqual(ReportDERControlRequest.LimitMaxDischarges) &&

             ((ToBeContinued             is     null && ReportDERControlRequest.ToBeContinued             is null) ||
              (ToBeContinued             is not null && ReportDERControlRequest.ToBeContinued             is not null && ToBeContinued.            Equals(ReportDERControlRequest.ToBeContinued)))             &&

               base.GenericEquals(ReportDERControlRequest);

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

                   GetDERControlRequestId.ToString(),

                   ToBeContinued.HasValue
                       ? ToBeContinued.Value
                             ? " [tbc!]: "
                             : " [end]: "
                       : " [end]: ",

                   Curves.Any()
                       ? $", curves: {Curves.AggregateWith(", ")}"
                       : "",

                   EnterServices.Any()
                       ? $", enter services: {EnterServices.AggregateWith(", ")}"
                       : "",

                   FixedPowerFactorsAbsorbing.Any()
                       ? $", fixed power factors absorbing: {FixedPowerFactorsAbsorbing.AggregateWith(", ")}"
                       : "",

                   FixedPowerFactorsInjecting.Any()
                       ? $", fixed power factors injecting: {FixedPowerFactorsInjecting.AggregateWith(", ")}"
                       : "",

                   FixedVARs.Any()
                       ? $", fixed vars: {FixedVARs.AggregateWith(", ")}"
                       : "",

                   FrequencyDroops.Any()
                       ? $", frequency droops: {FrequencyDroops.AggregateWith(", ")}"
                       : "",

                   Gradients.Any()
                       ? $", gradients: {Gradients.AggregateWith(", ")}"
                       : "",

                   LimitMaxDischarges.Any()
                       ? $", limits of the maximum discharge: {LimitMaxDischarges.AggregateWith(", ")}"
                       : ""

                );

        #endregion

    }

}
