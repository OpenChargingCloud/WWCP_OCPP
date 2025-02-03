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
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The ChargingScheduleUpdate.
    /// </summary>
    public class ChargingScheduleUpdate : ACustomData,
                                          IEquatable<ChargingScheduleUpdate>
    {

        #region Properties

        /// <summary>
        /// Optional charging rate limit in chargingRateUnit.
        /// </summary>
        [Optional]
        public ChargingRateValue?  Limit                  { get; }

        /// <summary>
        /// Optional charging rate limit in chargingRateUnit on phase L2.
        /// </summary>
        [Optional]
        public ChargingRateValue?  Limit_L2               { get; }

        /// <summary>
        /// Optional charging rate limit in chargingRateUnit on phase L3.
        /// </summary>
        [Optional]
        public ChargingRateValue?  Limit_L3               { get; }


        /// <summary>
        /// Optional discharging limit in chargingRateUnit.
        /// </summary>
        [Optional]
        public ChargingRateValue?  DischargeLimit         { get; }

        /// <summary>
        /// Optional discharging limit in chargingRateUnit on phase L2.
        /// </summary>
        [Optional]
        public ChargingRateValue?  DischargeLimit_L2      { get; }

        /// <summary>
        /// Optional discharging limit in chargingRateUnit on phase L3.
        /// </summary>
        [Optional]
        public ChargingRateValue?  DischargeLimit_L3      { get; }


        /// <summary>
        /// Optional setpoint in chargingRateUnit.
        /// </summary>
        [Optional]
        public ChargingRateValue?  Setpoint               { get; }

        /// <summary>
        /// Optional setpoint in chargingRateUnit on phase L2.
        /// </summary>
        [Optional]
        public ChargingRateValue?  Setpoint_L2            { get; }

        /// <summary>
        /// Optional setpoint in chargingRateUnit on phase L3.
        /// </summary>
        [Optional]
        public ChargingRateValue?  Setpoint_L3            { get; }


        /// <summary>
        /// Optional setpoint for reactive power (or current) in chargingRateUnit.
        /// </summary>
        [Optional]
        public ChargingRateValue?  SetpointReactive       { get; }

        /// <summary>
        /// Optional setpoint for reactive power (or current) in chargingRateUnit on phase L2.
        /// </summary>
        [Optional]
        public ChargingRateValue?  SetpointReactive_L2    { get; }

        /// <summary>
        /// Optional setpoint for reactive power (or current) in chargingRateUnit on phase L3.
        /// </summary>
        [Optional]
        public ChargingRateValue?  SetpointReactive_L3    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ChargingScheduleUpdate.
        /// </summary>
        /// <param name="Limit">Optional charging rate limit in chargingRateUnit (&gt;= 0).</param>
        /// <param name="Limit_L2">Optional charging rate limit in chargingRateUnit on phase L2 (&gt;= 0).</param>
        /// <param name="Limit_L3">Optional charging rate limit in chargingRateUnit on phase L3 (&gt;= 0).</param>
        /// 
        /// <param name="DischargeLimit">Optional discharging limit in chargingRateUnit (&lt;= 0).</param>
        /// <param name="DischargeLimit_L2">Optional discharging limit in chargingRateUnit on phase L2 (&lt;= 0).</param>
        /// <param name="DischargeLimit_L3">Optional discharging limit in chargingRateUnit on phase L3 (&lt;= 0).</param>
        /// 
        /// <param name="Setpoint">Optional setpoint in chargingRateUnit.</param>
        /// <param name="Setpoint_L2">Optional setpoint in chargingRateUnit on phase L2.</param>
        /// <param name="Setpoint_L3">Optional setpoint in chargingRateUnit on phase L3.</param>
        /// 
        /// <param name="SetpointReactive">Optional setpoint for reactive power (or current) in chargingRateUnit.</param>
        /// <param name="SetpointReactive_L2">Optional setpoint for reactive power (or current) in chargingRateUnit on phase L2.</param>
        /// <param name="SetpointReactive_L3">Optional setpoint for reactive power (or current) in chargingRateUnit on phase L3.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public ChargingScheduleUpdate(ChargingRateValue?  Limit                 = null,
                                      ChargingRateValue?  Limit_L2              = null,
                                      ChargingRateValue?  Limit_L3              = null,

                                      ChargingRateValue?  DischargeLimit        = null,
                                      ChargingRateValue?  DischargeLimit_L2     = null,
                                      ChargingRateValue?  DischargeLimit_L3     = null,

                                      ChargingRateValue?  Setpoint              = null,
                                      ChargingRateValue?  Setpoint_L2           = null,
                                      ChargingRateValue?  Setpoint_L3           = null,

                                      ChargingRateValue?  SetpointReactive      = null,
                                      ChargingRateValue?  SetpointReactive_L2   = null,
                                      ChargingRateValue?  SetpointReactive_L3   = null,

                                      CustomData?         CustomData            = null)

            : base(CustomData)

        {

            #region (Discharge)Limit checks

            if (Limit.HasValue && Limit.Value.Value < 0)
                throw new ArgumentException($"The given charging rate limit (for phase L1) {Limit.Value.Value} must not be negative!",
                                            nameof(Limit));

            if (Limit_L2.HasValue && Limit_L2.Value.Value < 0)
                throw new ArgumentException($"The given charging rate limit for phase L2 {Limit_L2.Value.Value} must not be negative!",
                                            nameof(Limit_L2));

            if (Limit_L3.HasValue && Limit_L3.Value.Value < 0)
                throw new ArgumentException($"The given charging rate limit for phase L3 {Limit_L3.Value.Value} must not be negative!",
                                            nameof(Limit_L3));


            if (DischargeLimit.HasValue && DischargeLimit.Value.Value > 0)
                throw new ArgumentException($"The given discharging rate limit (for phase L1) {DischargeLimit.Value.Value} must not be positive!",
                                            nameof(DischargeLimit));

            if (DischargeLimit_L2.HasValue && DischargeLimit_L2.Value.Value > 0)
                throw new ArgumentException($"The given discharging rate limit for phase L2 {DischargeLimit_L2.Value.Value} must not be positive!",
                                            nameof(DischargeLimit_L2));

            if (DischargeLimit_L3.HasValue && DischargeLimit_L3.Value.Value > 0)
                throw new ArgumentException($"The given discharging rate limit for phase L3 {DischargeLimit_L3.Value.Value} must not be positive!",
                                            nameof(DischargeLimit_L3));

            #endregion

            this.Limit                = Limit;
            this.Limit_L2             = Limit_L2;
            this.Limit_L3             = Limit_L3;

            this.DischargeLimit       = DischargeLimit;
            this.DischargeLimit_L2    = DischargeLimit_L2;
            this.DischargeLimit_L3    = DischargeLimit_L3;

            this.Setpoint             = Setpoint;
            this.Setpoint_L2          = Setpoint_L2;
            this.Setpoint_L3          = Setpoint_L3;

            this.SetpointReactive     = SetpointReactive;
            this.SetpointReactive_L2  = SetpointReactive_L2;
            this.SetpointReactive_L3  = SetpointReactive_L3;

            unchecked
            {

                hashCode = (Limit?.              GetHashCode() ?? 0) * 43 ^
                           (Limit_L2?.           GetHashCode() ?? 0) * 41 ^
                           (Limit_L3?.           GetHashCode() ?? 0) * 37 ^

                           (DischargeLimit?.     GetHashCode() ?? 0) * 31 ^
                           (DischargeLimit_L2?.  GetHashCode() ?? 0) * 29 ^
                           (DischargeLimit_L3?.  GetHashCode() ?? 0) * 23 ^

                           (Setpoint?.           GetHashCode() ?? 0) * 17 ^
                           (Setpoint_L2?.        GetHashCode() ?? 0) * 13 ^
                           (Setpoint_L3?.        GetHashCode() ?? 0) * 11 ^

                           (SetpointReactive?.   GetHashCode() ?? 0) *  7 ^
                           (SetpointReactive_L2?.GetHashCode() ?? 0) *  5 ^
                           (SetpointReactive_L3?.GetHashCode() ?? 0) *  3 ^

                            base.                GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:ChargingScheduleUpdate",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "ChargingProfileStatusEnumType": {
        //             "description": "Result of request.",
        //             "javaType": "ChargingProfileStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected"
        //             ]
        //         },
        //         "ChargingScheduleUpdateType": {
        //             "description": "Updates to a ChargingSchedulePeriodType for dynamic charging profiles.",
        //             "javaType": "ChargingScheduleUpdate",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "limit": {
        //                     "description": "Optional only when not required by the _operationMode_, as in CentralSetpoint, ExternalSetpoint, ExternalLimits, LocalFrequency,  LocalLoadBalancing. +\r\nCharging rate limit during the schedule period, in the applicable _chargingRateUnit_. \r\nThis SHOULD be a non-negative value; a negative value is only supported for backwards compatibility with older systems that use a negative value to specify a discharging limit.\r\nFor AC this field represents the sum of all phases, unless values are provided for L2 and L3, in which case this field represents phase L1.",
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
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
        //         },
        //         "StatusInfoType": {
        //             "description": "Element providing more information about the status.",
        //             "javaType": "StatusInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "reasonCode": {
        //                     "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "additionalInfo": {
        //                     "description": "Additional text to provide detailed information.",
        //                     "type": "string",
        //                     "maxLength": 1024
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "reasonCode"
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
        //         "scheduleUpdate": {
        //             "$ref": "#/definitions/ChargingScheduleUpdateType"
        //         },
        //         "status": {
        //             "$ref": "#/definitions/ChargingProfileStatusEnumType"
        //         },
        //         "statusInfo": {
        //             "$ref": "#/definitions/StatusInfoType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of a ChargingScheduleUpdate.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomChargingScheduleUpdateParser">A delegate to parse custom ChargingScheduleUpdate responses.</param>
        public static ChargingScheduleUpdate Parse(JObject                                               JSON,
                                                   CustomJObjectParserDelegate<ChargingScheduleUpdate>?  CustomChargingScheduleUpdateParser   = null,
                                                   CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {


            if (TryParse(JSON,
                         out var chargingScheduleUpdate,
                         out var errorResponse,
                         CustomChargingScheduleUpdateParser,
                         CustomCustomDataParser))
            {
                return chargingScheduleUpdate;
            }

            throw new ArgumentException("The given JSON representation of a ChargingScheduleUpdate response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingScheduleUpdate, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a component variable.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingScheduleUpdate">The parsed ChargingScheduleUpdate.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       [NotNullWhen(true)]  out ChargingScheduleUpdate?  ChargingScheduleUpdate,
                                       [NotNullWhen(false)] out String?                  ErrorResponse)

            => TryParse(JSON,
                        out ChargingScheduleUpdate,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a ChargingScheduleUpdate.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingScheduleUpdate">The parsed ChargingScheduleUpdate.</param>
        /// <param name="CustomChargingScheduleUpdateParser">A delegate to parse custom ChargingScheduleUpdates.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       [NotNullWhen(true)]  out ChargingScheduleUpdate?      ChargingScheduleUpdate,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingScheduleUpdate>?  CustomChargingScheduleUpdateParser   = null,
                                       CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {

            try
            {

                ChargingScheduleUpdate = null;

                #region Limit                  [optional]

                if (JSON.ParseOptional("limit",
                                       "charging rate limit",
                                       out ChargingRateValue? Limit,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Limit_L2               [optional]

                if (JSON.ParseOptional("limit_L2",
                                       "charging rate limit on phase L2",
                                       out ChargingRateValue? Limit_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Limit_L3               [optional]

                if (JSON.ParseOptional("limit_L3",
                                       "charging rate limit on phase L3",
                                       out ChargingRateValue? Limit_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region DischargeLimit         [optional]

                if (JSON.ParseOptional("dischargeLimit",
                                       "discharging rate limit",
                                       out ChargingRateValue? DischargeLimit,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region DischargeLimit_L2      [optional]

                if (JSON.ParseOptional("dischargeLimit_L2",
                                       "discharging rate limit on phase L2",
                                       out ChargingRateValue? DischargeLimit_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region DischargeLimit_L3      [optional]

                if (JSON.ParseOptional("dischargeLimit_L3",
                                       "discharging rate limit on phase L3",
                                       out ChargingRateValue? DischargeLimit_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Setpoint               [optional]

                if (JSON.ParseOptional("setpoint",
                                       "charging rate setpoint",
                                       out ChargingRateValue? Setpoint,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Setpoint_L2            [optional]

                if (JSON.ParseOptional("setpoint_L2",
                                       "charging rate setpoint on phase L2",
                                       out ChargingRateValue? Setpoint_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Setpoint_L3            [optional]

                if (JSON.ParseOptional("setpoint_L3",
                                       "charging rate setpoint on phase L3",
                                       out ChargingRateValue? Setpoint_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region SetpointReactive       [optional]

                if (JSON.ParseOptional("setpointReactive",
                                       "charging rate setpoint reactive",
                                       out ChargingRateValue? SetpointReactive,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region SetpointReactive_L2    [optional]

                if (JSON.ParseOptional("setpointReactive_L2",
                                       "charging rate setpoint reactive on phase L2",
                                       out ChargingRateValue? SetpointReactive_L2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region SetpointReactive_L3    [optional]

                if (JSON.ParseOptional("setpointReactive_L3",
                                       "charging rate setpoint reactive on phase L3",
                                       out ChargingRateValue? SetpointReactive_L3,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Signatures             [optional, OCPP_CSE]

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

                #region CustomData             [optional]

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


                ChargingScheduleUpdate = new ChargingScheduleUpdate(

                                             Limit,
                                             Limit_L2,
                                             Limit_L3,

                                             DischargeLimit,
                                             DischargeLimit_L2,
                                             DischargeLimit_L3,

                                             Setpoint,
                                             Setpoint_L2,
                                             Setpoint_L3,

                                             SetpointReactive,
                                             SetpointReactive_L2,
                                             SetpointReactive_L3,

                                             CustomData

                                         );

                if (CustomChargingScheduleUpdateParser is not null)
                    ChargingScheduleUpdate = CustomChargingScheduleUpdateParser(JSON,
                                                                                ChargingScheduleUpdate);

                return true;

            }
            catch (Exception e)
            {
                ChargingScheduleUpdate  = null;
                ErrorResponse           = "The given JSON representation of a ChargingScheduleUpdate response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChargingScheduleUpdateSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingScheduleUpdateSerializer">A delegate to serialize custom ChargingScheduleUpdate responses.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingScheduleUpdate>?  CustomChargingScheduleUpdateSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create(

                           Limit.              HasValue
                               ? new JProperty("limit",                 Limit.              Value.Value)
                               : null,

                           Limit_L2.           HasValue
                               ? new JProperty("limit_L2",              Limit_L2.           Value.Value)
                               : null,

                           Limit_L3.           HasValue
                               ? new JProperty("limit_L3",              Limit_L3.           Value.Value)
                               : null,


                           DischargeLimit.     HasValue
                               ? new JProperty("dischargeLimit",        DischargeLimit.     Value.Value)
                               : null,

                           DischargeLimit_L2.  HasValue
                               ? new JProperty("dischargeLimit_L2",     DischargeLimit_L2.  Value.Value)
                               : null,

                           DischargeLimit_L3.  HasValue
                               ? new JProperty("dischargeLimit_L3",     DischargeLimit_L3.  Value.Value)
                               : null,


                           Setpoint.           HasValue
                               ? new JProperty("setpoint",              Setpoint.           Value.Value)
                               : null,

                           Setpoint_L2.        HasValue
                               ? new JProperty("setpoint_L2",           Setpoint_L2.        Value.Value)
                               : null,

                           Setpoint_L3.        HasValue
                               ? new JProperty("setpoint_L3",           Setpoint_L3.        Value.Value)
                               : null,


                           SetpointReactive.   HasValue
                               ? new JProperty("setpointReactive",      SetpointReactive.   Value.Value)
                               : null,

                           SetpointReactive_L2.HasValue
                               ? new JProperty("setpointReactive_L2",   SetpointReactive_L2.Value.Value)
                               : null,

                           SetpointReactive_L3.HasValue
                               ? new JProperty("setpointReactive_L3",   SetpointReactive_L3.Value.Value)
                               : null,


                           CustomData is not null
                               ? new JProperty("customData",            CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomChargingScheduleUpdateSerializer is not null
                       ? CustomChargingScheduleUpdateSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargingScheduleUpdate1, ChargingScheduleUpdate2)

        /// <summary>
        /// Compares two ChargingScheduleUpdate responses for equality.
        /// </summary>
        /// <param name="ChargingScheduleUpdate1">A ChargingScheduleUpdate response.</param>
        /// <param name="ChargingScheduleUpdate2">Another ChargingScheduleUpdate response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ChargingScheduleUpdate? ChargingScheduleUpdate1,
                                           ChargingScheduleUpdate? ChargingScheduleUpdate2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChargingScheduleUpdate1, ChargingScheduleUpdate2))
                return true;

            // If one is null, but not both, return false.
            if (ChargingScheduleUpdate1 is null || ChargingScheduleUpdate2 is null)
                return false;

            return ChargingScheduleUpdate1.Equals(ChargingScheduleUpdate2);

        }

        #endregion

        #region Operator != (ChargingScheduleUpdate1, ChargingScheduleUpdate2)

        /// <summary>
        /// Compares two ChargingScheduleUpdate responses for inequality.
        /// </summary>
        /// <param name="ChargingScheduleUpdate1">A ChargingScheduleUpdate response.</param>
        /// <param name="ChargingScheduleUpdate2">Another ChargingScheduleUpdate response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ChargingScheduleUpdate? ChargingScheduleUpdate1,
                                           ChargingScheduleUpdate? ChargingScheduleUpdate2)

            => !(ChargingScheduleUpdate1 == ChargingScheduleUpdate2);

        #endregion

        #endregion

        #region IEquatable<ChargingScheduleUpdate> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ChargingScheduleUpdate responses for equality.
        /// </summary>
        /// <param name="Object">A ChargingScheduleUpdate response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingScheduleUpdate chargingScheduleUpdate &&
                   Equals(chargingScheduleUpdate);

        #endregion

        #region Equals(ChargingScheduleUpdate)

        /// <summary>
        /// Compares two ChargingScheduleUpdate responses for equality.
        /// </summary>
        /// <param name="ChargingScheduleUpdate">A ChargingScheduleUpdate response to compare with.</param>
        public Boolean Equals(ChargingScheduleUpdate? ChargingScheduleUpdate)

            => ChargingScheduleUpdate is not null &&

             ((!Limit.              HasValue && !ChargingScheduleUpdate.Limit.              HasValue) ||
                Limit.              HasValue &&  ChargingScheduleUpdate.Limit.              HasValue &&
                Limit.              Value.Equals(ChargingScheduleUpdate.Limit.              Value))  &&

             ((!Limit_L2.           HasValue && !ChargingScheduleUpdate.Limit_L2.           HasValue) ||
                Limit_L2.           HasValue &&  ChargingScheduleUpdate.Limit_L2.           HasValue &&
                Limit_L2.           Value.Equals(ChargingScheduleUpdate.Limit_L2.           Value))  &&

             ((!Limit_L3.           HasValue && !ChargingScheduleUpdate.Limit_L3.           HasValue) ||
                Limit_L3.           HasValue &&  ChargingScheduleUpdate.Limit_L3.           HasValue &&
                Limit_L3.           Value.Equals(ChargingScheduleUpdate.Limit_L3.           Value))  &&


             ((!DischargeLimit.     HasValue && !ChargingScheduleUpdate.DischargeLimit.     HasValue) ||
                DischargeLimit.     HasValue &&  ChargingScheduleUpdate.DischargeLimit.     HasValue &&
                DischargeLimit.     Value.Equals(ChargingScheduleUpdate.DischargeLimit.     Value))  &&

             ((!DischargeLimit_L2.  HasValue && !ChargingScheduleUpdate.DischargeLimit_L2.  HasValue) ||
                DischargeLimit_L2.  HasValue &&  ChargingScheduleUpdate.DischargeLimit_L2.  HasValue &&
                DischargeLimit_L2.  Value.Equals(ChargingScheduleUpdate.DischargeLimit_L2.  Value))  &&

             ((!DischargeLimit_L3.  HasValue && !ChargingScheduleUpdate.DischargeLimit_L3.  HasValue) ||
                DischargeLimit_L3.  HasValue &&  ChargingScheduleUpdate.DischargeLimit_L3.  HasValue &&
                DischargeLimit_L3.  Value.Equals(ChargingScheduleUpdate.DischargeLimit_L3.  Value))  &&


             ((!Setpoint.           HasValue && !ChargingScheduleUpdate.Setpoint.           HasValue) ||
                Setpoint.           HasValue &&  ChargingScheduleUpdate.Setpoint.           HasValue &&
                Setpoint.           Value.Equals(ChargingScheduleUpdate.Setpoint.           Value))  &&

             ((!Setpoint_L2.        HasValue && !ChargingScheduleUpdate.Setpoint_L2.        HasValue) ||
                Setpoint_L2.        HasValue &&  ChargingScheduleUpdate.Setpoint_L2.        HasValue &&
                Setpoint_L2.        Value.Equals(ChargingScheduleUpdate.Setpoint_L2.        Value))  &&

             ((!Setpoint_L3.        HasValue && !ChargingScheduleUpdate.Setpoint_L3.        HasValue) ||
                Setpoint_L3.        HasValue &&  ChargingScheduleUpdate.Setpoint_L3.        HasValue &&
                Setpoint_L3.        Value.Equals(ChargingScheduleUpdate.Setpoint_L3.        Value))  &&


             ((!SetpointReactive.   HasValue && !ChargingScheduleUpdate.SetpointReactive.   HasValue) ||
                SetpointReactive.   HasValue &&  ChargingScheduleUpdate.SetpointReactive.   HasValue &&
                SetpointReactive.   Value.Equals(ChargingScheduleUpdate.SetpointReactive.   Value))  &&

             ((!SetpointReactive_L2.HasValue && !ChargingScheduleUpdate.SetpointReactive_L2.HasValue) ||
                SetpointReactive_L2.HasValue &&  ChargingScheduleUpdate.SetpointReactive_L2.HasValue &&
                SetpointReactive_L2.Value.Equals(ChargingScheduleUpdate.SetpointReactive_L2.Value))  &&

             ((!SetpointReactive_L3.HasValue && !ChargingScheduleUpdate.SetpointReactive_L3.HasValue) ||
                SetpointReactive_L3.HasValue &&  ChargingScheduleUpdate.SetpointReactive_L3.HasValue &&
                SetpointReactive_L3.Value.Equals(ChargingScheduleUpdate.SetpointReactive_L3.Value))  &&

               base.Equals(ChargingScheduleUpdate);

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

            => new[] {

                   Limit.HasValue
                       ? "limit: "                + Limit.              Value.ToString()
                       : null,

                   Limit_L2.HasValue
                       ? "limit L2: "             + Limit_L2.           Value.ToString()
                       : null,

                   Limit_L3.HasValue
                       ? "limit L3: "             + Limit_L3.           Value.ToString()
                       : null,


                   DischargeLimit.HasValue
                       ? "discharge limit: "      + DischargeLimit.     Value.ToString()
                       : null,

                   DischargeLimit_L2.HasValue
                       ? "discharge limit L2: "   + DischargeLimit_L2.  Value.ToString()
                       : null,

                   DischargeLimit_L3.HasValue
                       ? "discharge limit L3: "   + DischargeLimit_L3.  Value.ToString()
                       : null,


                   Setpoint.HasValue
                       ? "setpoint: "             + Setpoint.           Value.ToString()
                       : null,

                   Setpoint_L2.HasValue
                       ? "setpoint L2: "          + Setpoint_L2.        Value.ToString()
                       : null,

                   Setpoint_L3.HasValue
                       ? "setpoint L3: "          + Setpoint_L3.        Value.ToString()
                       : null,


                   SetpointReactive.HasValue
                       ? "setpoint reactive: "    + SetpointReactive.   Value.ToString()
                       : null,

                   SetpointReactive_L2.HasValue
                       ? "setpoint reactive L2: " + SetpointReactive_L2.Value.ToString()
                       : null,

                   SetpointReactive_L3.HasValue
                       ? "setpoint reactive L3: " + SetpointReactive_L3.Value.ToString()
                       : null

               }.Where(value => value is not null).
                 AggregateWith(", ");

        #endregion

    }

}
