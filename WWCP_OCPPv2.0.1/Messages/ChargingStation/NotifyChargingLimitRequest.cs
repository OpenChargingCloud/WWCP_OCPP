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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1.CS
{

    /// <summary>
    /// A notify charging limit request.
    /// </summary>
    public class NotifyChargingLimitRequest : ARequest<NotifyChargingLimitRequest>
    {

        #region Properties

        /// <summary>
        /// The charging limit, its source and whether it is grid critical.
        /// </summary>
        [Mandatory]
        public ChargingLimit                  ChargingLimit        { get; }

        /// <summary>
        /// Limits for the available power or current over time, as set by the external source.
        /// </summary>
        [Mandatory]
        public IEnumerable<ChargingSchedule>  ChargingSchedules    { get; }

        /// <summary>
        /// The optional EVSE identification, when the charging schedule contained in this notification applies to an EVSE.
        /// </summary>
        [Optional]
        public EVSE_Id?                       EVSEId               { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a notify charging limit request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChargingLimit">The charging limit, its source and whether it is grid critical.</param>
        /// <param name="ChargingSchedules">Limits for the available power or current over time, as set by the external source.</param>
        /// <param name="EVSEId">An optional EVSE identification, when the charging schedule contained in this notification applies to an EVSE.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public NotifyChargingLimitRequest(ChargeBox_Id                   ChargeBoxId,
                                          ChargingLimit                  ChargingLimit,
                                          IEnumerable<ChargingSchedule>  ChargingSchedules,
                                          EVSE_Id?                       EVSEId              = null,
                                          CustomData?                    CustomData          = null,

                                          Request_Id?                    RequestId           = null,
                                          DateTime?                      RequestTimestamp    = null,
                                          TimeSpan?                      RequestTimeout      = null,
                                          EventTracking_Id?              EventTrackingId     = null,
                                          CancellationToken              CancellationToken   = default)

            : base(ChargeBoxId,
                   "NotifyChargingLimit",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            if (!ChargingSchedules.Any())
                throw new ArgumentException("The given enumeration of meter values must not be empty!",
                                            nameof(ChargingSchedules));

            this.ChargingLimit      = ChargingLimit;
            this.ChargingSchedules  = ChargingSchedules.Distinct();
            this.EVSEId             = EVSEId;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:NotifyChargingLimitRequest",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //       "javaType": "CustomData",
        //       "type": "object",
        //       "properties": {
        //         "vendorId": {
        //           "type": "string",
        //           "maxLength": 255
        //         }
        //       },
        //       "required": [
        //         "vendorId"
        //       ]
        //     },
        //     "ChargingLimitSourceEnumType": {
        //       "description": "Charging_ Limit. Charging_ Limit_ Source. Charging_ Limit_ Source_ Code\r\nurn:x-enexis:ecdm:uid:1:570845\r\nRepresents the source of the charging limit.\r\n",
        //       "javaType": "ChargingLimitSourceEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "EMS",
        //         "Other",
        //         "SO",
        //         "CSO"
        //       ]
        //     },
        //     "ChargingRateUnitEnumType": {
        //       "description": "Charging_ Schedule. Charging_ Rate_ Unit. Charging_ Rate_ Unit_ Code\r\nurn:x-oca:ocpp:uid:1:569238\r\nThe unit of measure Limit is expressed in.\r\n",
        //       "javaType": "ChargingRateUnitEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "W",
        //         "A"
        //       ]
        //     },
        //     "CostKindEnumType": {
        //       "description": "Cost. Cost_ Kind. Cost_ Kind_ Code\r\nurn:x-oca:ocpp:uid:1:569243\r\nThe kind of cost referred to in the message element amount\r\n",
        //       "javaType": "CostKindEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "CarbonDioxideEmission",
        //         "RelativePricePercentage",
        //         "RenewableGenerationPercentage"
        //       ]
        //     },
        //     "ChargingLimitType": {
        //       "description": "Charging_ Limit\r\nurn:x-enexis:ecdm:uid:2:234489\r\n",
        //       "javaType": "ChargingLimit",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "chargingLimitSource": {
        //           "$ref": "#/definitions/ChargingLimitSourceEnumType"
        //         },
        //         "isGridCritical": {
        //           "description": "Charging_ Limit. Is_ Grid_ Critical. Indicator\r\nurn:x-enexis:ecdm:uid:1:570847\r\nIndicates whether the charging limit is critical for the grid.\r\n",
        //           "type": "boolean"
        //         }
        //       },
        //       "required": [
        //         "chargingLimitSource"
        //       ]
        //     },
        //     "ChargingSchedulePeriodType": {
        //       "description": "Charging_ Schedule_ Period\r\nurn:x-oca:ocpp:uid:2:233257\r\nCharging schedule period structure defines a time period in a charging schedule.\r\n",
        //       "javaType": "ChargingSchedulePeriod",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "startPeriod": {
        //           "description": "Charging_ Schedule_ Period. Start_ Period. Elapsed_ Time\r\nurn:x-oca:ocpp:uid:1:569240\r\nStart of the period, in seconds from the start of schedule. The value of StartPeriod also defines the stop time of the previous period.\r\n",
        //           "type": "integer"
        //         },
        //         "limit": {
        //           "description": "Charging_ Schedule_ Period. Limit. Measure\r\nurn:x-oca:ocpp:uid:1:569241\r\nCharging rate limit during the schedule period, in the applicable chargingRateUnit, for example in Amperes (A) or Watts (W). Accepts at most one digit fraction (e.g. 8.1).\r\n",
        //           "type": "number"
        //         },
        //         "numberPhases": {
        //           "description": "Charging_ Schedule_ Period. Number_ Phases. Counter\r\nurn:x-oca:ocpp:uid:1:569242\r\nThe number of phases that can be used for charging. If a number of phases is needed, numberPhases=3 will be assumed unless another number is given.\r\n",
        //           "type": "integer"
        //         },
        //         "phaseToUse": {
        //           "description": "Values: 1..3, Used if numberPhases=1 and if the EVSE is capable of switching the phase connected to the EV, i.e. ACPhaseSwitchingSupported is defined and true. It’s not allowed unless both conditions above are true. If both conditions are true, and phaseToUse is omitted, the Charging Station / EVSE will make the selection on its own.\r\n\r\n",
        //           "type": "integer"
        //         }
        //       },
        //       "required": [
        //         "startPeriod",
        //         "limit"
        //       ]
        //     },
        //     "ChargingScheduleType": {
        //       "description": "Charging_ Schedule\r\nurn:x-oca:ocpp:uid:2:233256\r\nCharging schedule structure defines a list of charging periods, as used in: GetCompositeSchedule.conf and ChargingProfile. \r\n",
        //       "javaType": "ChargingSchedule",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "id": {
        //           "description": "Identifies the ChargingSchedule.\r\n",
        //           "type": "integer"
        //         },
        //         "startSchedule": {
        //           "description": "Charging_ Schedule. Start_ Schedule. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569237\r\nStarting point of an absolute schedule. If absent the schedule will be relative to start of charging.\r\n",
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "duration": {
        //           "description": "Charging_ Schedule. Duration. Elapsed_ Time\r\nurn:x-oca:ocpp:uid:1:569236\r\nDuration of the charging schedule in seconds. If the duration is left empty, the last period will continue indefinitely or until end of the transaction if chargingProfilePurpose = TxProfile.\r\n",
        //           "type": "integer"
        //         },
        //         "chargingRateUnit": {
        //           "$ref": "#/definitions/ChargingRateUnitEnumType"
        //         },
        //         "chargingSchedulePeriod": {
        //           "type": "array",
        //           "additionalItems": false,
        //           "items": {
        //             "$ref": "#/definitions/ChargingSchedulePeriodType"
        //           },
        //           "minItems": 1,
        //           "maxItems": 1024
        //         },
        //         "minChargingRate": {
        //           "description": "Charging_ Schedule. Min_ Charging_ Rate. Numeric\r\nurn:x-oca:ocpp:uid:1:569239\r\nMinimum charging rate supported by the EV. The unit of measure is defined by the chargingRateUnit. This parameter is intended to be used by a local smart charging algorithm to optimize the power allocation for in the case a charging process is inefficient at lower charging rates. Accepts at most one digit fraction (e.g. 8.1)\r\n",
        //           "type": "number"
        //         },
        //         "salesTariff": {
        //           "$ref": "#/definitions/SalesTariffType"
        //         }
        //       },
        //       "required": [
        //         "id",
        //         "chargingRateUnit",
        //         "chargingSchedulePeriod"
        //       ]
        //     },
        //     "ConsumptionCostType": {
        //       "description": "Consumption_ Cost\r\nurn:x-oca:ocpp:uid:2:233259\r\n",
        //       "javaType": "ConsumptionCost",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "startValue": {
        //           "description": "Consumption_ Cost. Start_ Value. Numeric\r\nurn:x-oca:ocpp:uid:1:569246\r\nThe lowest level of consumption that defines the starting point of this consumption block. The block interval extends to the start of the next interval.\r\n",
        //           "type": "number"
        //         },
        //         "cost": {
        //           "type": "array",
        //           "additionalItems": false,
        //           "items": {
        //             "$ref": "#/definitions/CostType"
        //           },
        //           "minItems": 1,
        //           "maxItems": 3
        //         }
        //       },
        //       "required": [
        //         "startValue",
        //         "cost"
        //       ]
        //     },
        //     "CostType": {
        //       "description": "Cost\r\nurn:x-oca:ocpp:uid:2:233258\r\n",
        //       "javaType": "Cost",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "costKind": {
        //           "$ref": "#/definitions/CostKindEnumType"
        //         },
        //         "amount": {
        //           "description": "Cost. Amount. Amount\r\nurn:x-oca:ocpp:uid:1:569244\r\nThe estimated or actual cost per kWh\r\n",
        //           "type": "integer"
        //         },
        //         "amountMultiplier": {
        //           "description": "Cost. Amount_ Multiplier. Integer\r\nurn:x-oca:ocpp:uid:1:569245\r\nValues: -3..3, The amountMultiplier defines the exponent to base 10 (dec). The final value is determined by: amount * 10 ^ amountMultiplier\r\n",
        //           "type": "integer"
        //         }
        //       },
        //       "required": [
        //         "costKind",
        //         "amount"
        //       ]
        //     },
        //     "RelativeTimeIntervalType": {
        //       "description": "Relative_ Timer_ Interval\r\nurn:x-oca:ocpp:uid:2:233270\r\n",
        //       "javaType": "RelativeTimeInterval",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "start": {
        //           "description": "Relative_ Timer_ Interval. Start. Elapsed_ Time\r\nurn:x-oca:ocpp:uid:1:569279\r\nStart of the interval, in seconds from NOW.\r\n",
        //           "type": "integer"
        //         },
        //         "duration": {
        //           "description": "Relative_ Timer_ Interval. Duration. Elapsed_ Time\r\nurn:x-oca:ocpp:uid:1:569280\r\nDuration of the interval, in seconds.\r\n",
        //           "type": "integer"
        //         }
        //       },
        //       "required": [
        //         "start"
        //       ]
        //     },
        //     "SalesTariffEntryType": {
        //       "description": "Sales_ Tariff_ Entry\r\nurn:x-oca:ocpp:uid:2:233271\r\n",
        //       "javaType": "SalesTariffEntry",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "relativeTimeInterval": {
        //           "$ref": "#/definitions/RelativeTimeIntervalType"
        //         },
        //         "ePriceLevel": {
        //           "description": "Sales_ Tariff_ Entry. E_ Price_ Level. Unsigned_ Integer\r\nurn:x-oca:ocpp:uid:1:569281\r\nDefines the price level of this SalesTariffEntry (referring to NumEPriceLevels). Small values for the EPriceLevel represent a cheaper TariffEntry. Large values for the EPriceLevel represent a more expensive TariffEntry.\r\n",
        //           "type": "integer",
        //           "minimum": 0.0
        //         },
        //         "consumptionCost": {
        //           "type": "array",
        //           "additionalItems": false,
        //           "items": {
        //             "$ref": "#/definitions/ConsumptionCostType"
        //           },
        //           "minItems": 1,
        //           "maxItems": 3
        //         }
        //       },
        //       "required": [
        //         "relativeTimeInterval"
        //       ]
        //     },
        //     "SalesTariffType": {
        //       "description": "Sales_ Tariff\r\nurn:x-oca:ocpp:uid:2:233272\r\nNOTE: This dataType is based on dataTypes from &lt;&lt;ref-ISOIEC15118-2,ISO 15118-2&gt;&gt;.\r\n",
        //       "javaType": "SalesTariff",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "id": {
        //           "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nSalesTariff identifier used to identify one sales tariff. An SAID remains a unique identifier for one schedule throughout a charging session.\r\n",
        //           "type": "integer"
        //         },
        //         "salesTariffDescription": {
        //           "description": "Sales_ Tariff. Sales. Tariff_ Description\r\nurn:x-oca:ocpp:uid:1:569283\r\nA human readable title/short description of the sales tariff e.g. for HMI display purposes.\r\n",
        //           "type": "string",
        //           "maxLength": 32
        //         },
        //         "numEPriceLevels": {
        //           "description": "Sales_ Tariff. Num_ E_ Price_ Levels. Counter\r\nurn:x-oca:ocpp:uid:1:569284\r\nDefines the overall number of distinct price levels used across all provided SalesTariff elements.\r\n",
        //           "type": "integer"
        //         },
        //         "salesTariffEntry": {
        //           "type": "array",
        //           "additionalItems": false,
        //           "items": {
        //             "$ref": "#/definitions/SalesTariffEntryType"
        //           },
        //           "minItems": 1,
        //           "maxItems": 1024
        //         }
        //       },
        //       "required": [
        //         "id",
        //         "salesTariffEntry"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "chargingSchedule": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/ChargingScheduleType"
        //       },
        //       "minItems": 1
        //     },
        //     "evseId": {
        //       "description": "The charging schedule contained in this notification applies to an EVSE. evseId must be &gt; 0.\r\n",
        //       "type": "integer"
        //     },
        //     "chargingLimit": {
        //       "$ref": "#/definitions/ChargingLimitType"
        //     }
        //   },
        //   "required": [
        //     "chargingLimit"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomNotifyChargingLimitRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify charging limit request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomNotifyChargingLimitRequestParser">A delegate to parse custom notify charging limit requests.</param>
        public static NotifyChargingLimitRequest Parse(JObject                                                   JSON,
                                                       Request_Id                                                RequestId,
                                                       ChargeBox_Id                                              ChargeBoxId,
                                                       CustomJObjectParserDelegate<NotifyChargingLimitRequest>?  CustomNotifyChargingLimitRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var notifyChargingLimitRequest,
                         out var errorResponse,
                         CustomNotifyChargingLimitRequestParser))
            {
                return notifyChargingLimitRequest!;
            }

            throw new ArgumentException("The given JSON representation of a notify charging limit request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out NotifyChargingLimitRequest, out ErrorResponse, CustomNotifyChargingLimitRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify charging limit request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="NotifyChargingLimitRequest">The parsed notify charging limit request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyChargingLimitRequestParser">A delegate to parse custom notify charging limit requests.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       Request_Id                                                RequestId,
                                       ChargeBox_Id                                              ChargeBoxId,
                                       out NotifyChargingLimitRequest?                           NotifyChargingLimitRequest,
                                       out String?                                               ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyChargingLimitRequest>?  CustomNotifyChargingLimitRequestParser)
        {

            try
            {

                NotifyChargingLimitRequest = null;

                #region ChargingLimit        [mandatory]

                if (!JSON.ParseMandatoryJSON("chargingLimit",
                                             "charging limit",
                                             OCPPv2_0_1.ChargingLimit.TryParse,
                                             out ChargingLimit? ChargingLimit,
                                             out ErrorResponse) ||
                     ChargingLimit is null)
                {
                    return false;
                }

                #endregion

                #region ChargingSchedules    [optional]

                if (JSON.ParseOptionalHashSet("chargingSchedule",
                                              "charging schedule",
                                              ChargingSchedule.TryParse,
                                              out HashSet<ChargingSchedule> ChargingSchedules,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVSEId               [optional]

                if (JSON.ParseOptional("evseId",
                                       "EVSE identification",
                                       EVSE_Id.TryParse,
                                       out EVSE_Id EVSEId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId          [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                NotifyChargingLimitRequest = new NotifyChargingLimitRequest(ChargeBoxId,
                                                                            ChargingLimit,
                                                                            ChargingSchedules,
                                                                            EVSEId,
                                                                            CustomData,
                                                                            RequestId);

                if (CustomNotifyChargingLimitRequestParser is not null)
                    NotifyChargingLimitRequest = CustomNotifyChargingLimitRequestParser(JSON,
                                                                                        NotifyChargingLimitRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyChargingLimitRequest  = null;
                ErrorResponse               = "The given JSON representation of a notify charging limit request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyChargingLimitRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyChargingLimitRequestSerializer">A delegate to serialize custom NotifyChargingLimit requests.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyChargingLimitRequest>?  CustomNotifyChargingLimitRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingSchedule>?            CustomChargingScheduleSerializer             = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?      CustomChargingSchedulePeriodSerializer       = null,
                              CustomJObjectSerializerDelegate<SalesTariff>?                 CustomSalesTariffSerializer                  = null,
                              CustomJObjectSerializerDelegate<SalesTariffEntry>?            CustomSalesTariffEntrySerializer             = null,
                              CustomJObjectSerializerDelegate<RelativeTimeInterval>?        CustomRelativeTimeIntervalSerializer         = null,
                              CustomJObjectSerializerDelegate<ConsumptionCost>?             CustomConsumptionCostSerializer              = null,
                              CustomJObjectSerializerDelegate<Cost>?                        CustomCostSerializer                         = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("chargingLimit",      ChargingLimit.ToJSON()),

                           ChargingSchedules.Any()
                               ? new JProperty("chargingSchedule",   new JArray(ChargingSchedules.Select(chargingSchedule => chargingSchedule.ToJSON(CustomChargingScheduleSerializer,
                                                                                                                                                     CustomChargingSchedulePeriodSerializer,
                                                                                                                                                     CustomSalesTariffSerializer,
                                                                                                                                                     CustomSalesTariffEntrySerializer,
                                                                                                                                                     CustomRelativeTimeIntervalSerializer,
                                                                                                                                                     CustomConsumptionCostSerializer,
                                                                                                                                                     CustomCostSerializer,
                                                                                                                                                     CustomCustomDataSerializer))))
                               : null,

                           EVSEId.HasValue
                               ? new JProperty("evseId",             EVSEId.       Value.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.   ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomNotifyChargingLimitRequestSerializer is not null
                       ? CustomNotifyChargingLimitRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyChargingLimitRequest1, NotifyChargingLimitRequest2)

        /// <summary>
        /// Compares two notify charging limit requests for equality.
        /// </summary>
        /// <param name="NotifyChargingLimitRequest1">A notify charging limit request.</param>
        /// <param name="NotifyChargingLimitRequest2">Another notify charging limit request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyChargingLimitRequest? NotifyChargingLimitRequest1,
                                           NotifyChargingLimitRequest? NotifyChargingLimitRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyChargingLimitRequest1, NotifyChargingLimitRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyChargingLimitRequest1 is null || NotifyChargingLimitRequest2 is null)
                return false;

            return NotifyChargingLimitRequest1.Equals(NotifyChargingLimitRequest2);

        }

        #endregion

        #region Operator != (NotifyChargingLimitRequest1, NotifyChargingLimitRequest2)

        /// <summary>
        /// Compares two notify charging limit requests for inequality.
        /// </summary>
        /// <param name="NotifyChargingLimitRequest1">A notify charging limit request.</param>
        /// <param name="NotifyChargingLimitRequest2">Another notify charging limit request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyChargingLimitRequest? NotifyChargingLimitRequest1,
                                           NotifyChargingLimitRequest? NotifyChargingLimitRequest2)

            => !(NotifyChargingLimitRequest1 == NotifyChargingLimitRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyChargingLimitRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify charging limit requests for equality.
        /// </summary>
        /// <param name="Object">A notify charging limit request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyChargingLimitRequest notifyChargingLimitRequest &&
                   Equals(notifyChargingLimitRequest);

        #endregion

        #region Equals(NotifyChargingLimitRequest)

        /// <summary>
        /// Compares two notify charging limit requests for equality.
        /// </summary>
        /// <param name="NotifyChargingLimitRequest">A notify charging limit request to compare with.</param>
        public override Boolean Equals(NotifyChargingLimitRequest? NotifyChargingLimitRequest)

            => NotifyChargingLimitRequest is not null &&

               ChargingLimit.Equals(NotifyChargingLimitRequest.ChargingLimit) &&

               ChargingSchedules.Count().Equals(NotifyChargingLimitRequest.ChargingSchedules.Count())     &&
               ChargingSchedules.All(data => NotifyChargingLimitRequest.ChargingSchedules.Contains(data)) &&

            ((!EVSEId.HasValue && !NotifyChargingLimitRequest.EVSEId.HasValue) ||
               EVSEId.HasValue &&  NotifyChargingLimitRequest.EVSEId.HasValue && EVSEId.Value.Equals(NotifyChargingLimitRequest.EVSEId.Value)) &&

               base.  GenericEquals(NotifyChargingLimitRequest);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return ChargingLimit.    GetHashCode()       * 7 ^
                       ChargingSchedules.CalcHashCode()      * 5 ^
                      (EVSEId?.          GetHashCode() ?? 0) * 3 ^

                       base.             GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => ChargingLimit.ToString();

        #endregion

    }

}
