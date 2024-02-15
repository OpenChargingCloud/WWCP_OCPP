/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// A notify EV charging schedule request.
    /// </summary>
    public class NotifyEVChargingScheduleRequest : ARequest<NotifyEVChargingScheduleRequest>
    {

        #region Properties

        /// <summary>
        /// The charging periods contained within the charging schedule
        /// are relative to this time base.
        /// </summary>
        [Mandatory]
        public DateTime          TimeBase            { get; }

        /// <summary>
        /// The charging schedule applies to this EVSE.
        /// </summary>
        [Mandatory]
        public EVSE_Id           EVSEId              { get; }

        /// <summary>
        /// Planned energy consumption of the EV over time.
        /// Always relative to the time base.
        /// </summary>
        [Mandatory]
        public ChargingSchedule  ChargingSchedule    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a notify EV charging schedule request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="TimeBase">The charging periods contained within the charging schedule are relative to this time base.</param>
        /// <param name="EVSEId">The charging schedule applies to this EVSE.</param>
        /// <param name="ChargingSchedule">Planned energy consumption of the EV over time. Always relative to the time base.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public NotifyEVChargingScheduleRequest(ChargeBox_Id       ChargeBoxId,
                                               DateTime           TimeBase,
                                               EVSE_Id            EVSEId,
                                               ChargingSchedule   ChargingSchedule,
                                               CustomData?        CustomData          = null,

                                               Request_Id?        RequestId           = null,
                                               DateTime?          RequestTimestamp    = null,
                                               TimeSpan?          RequestTimeout      = null,
                                               EventTracking_Id?  EventTrackingId     = null,
                                               CancellationToken  CancellationToken   = default)

            : base(ChargeBoxId,
                   "NotifyEVChargingSchedule",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.TimeBase          = TimeBase;
            this.EVSEId            = EVSEId;
            this.ChargingSchedule  = ChargingSchedule;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:NotifyEVChargingScheduleRequest",
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
        //     "timeBase": {
        //       "description": "Periods contained in the charging profile are relative to this point in time.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "chargingSchedule": {
        //       "$ref": "#/definitions/ChargingScheduleType"
        //     },
        //     "evseId": {
        //       "description": "The charging schedule contained in this notification applies to an EVSE. EvseId must be &gt; 0.\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "timeBase",
        //     "evseId",
        //     "chargingSchedule"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomNotifyEVChargingScheduleRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify EV charging schedule request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomNotifyEVChargingScheduleRequestParser">An optional delegate to parse custom notify EV charging schedule requests.</param>
        public static NotifyEVChargingScheduleRequest Parse(JObject                                                        JSON,
                                                            Request_Id                                                     RequestId,
                                                            ChargeBox_Id                                                   ChargeBoxId,
                                                            CustomJObjectParserDelegate<NotifyEVChargingScheduleRequest>?  CustomNotifyEVChargingScheduleRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var notifyEVChargingNeedsRequest,
                         out var errorResponse,
                         CustomNotifyEVChargingScheduleRequestParser))
            {
                return notifyEVChargingNeedsRequest!;
            }

            throw new ArgumentException("The given JSON representation of a notify EV charging schedule request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out NotifyEVChargingScheduleRequest, out ErrorResponse, CustomNotifyEVChargingScheduleRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify EV charging schedule request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="NotifyEVChargingScheduleRequest">The parsed notify EV charging schedule request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyEVChargingScheduleRequestParser">An optional delegate to parse custom notify EV charging schedule requests.</param>
        public static Boolean TryParse(JObject                                                        JSON,
                                       Request_Id                                                     RequestId,
                                       ChargeBox_Id                                                   ChargeBoxId,
                                       out NotifyEVChargingScheduleRequest?                           NotifyEVChargingScheduleRequest,
                                       out String?                                                    ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyEVChargingScheduleRequest>?  CustomNotifyEVChargingScheduleRequestParser)
        {

            try
            {

                NotifyEVChargingScheduleRequest = null;

                #region TimeBase            [mandatory]

                if (!JSON.ParseMandatory("timeBase",
                                         "time base",
                                         out DateTime TimeBase,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVSEId              [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "EVSE identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingSchedule    [mandatory]

                if (!JSON.ParseMandatoryJSON("chargingSchedule",
                                             "charging schedule",
                                             OCPPv2_0_1.ChargingSchedule.TryParse,
                                             out ChargingSchedule? ChargingSchedule,
                                             out ErrorResponse) ||
                     ChargingSchedule is null)
                {
                    return false;
                }

                #endregion

                #region CustomData          [optional]

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

                #region ChargeBoxId         [optional, OCPP_CSE]

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


                NotifyEVChargingScheduleRequest = new NotifyEVChargingScheduleRequest(
                                                      ChargeBoxId,
                                                      TimeBase,
                                                      EVSEId,
                                                      ChargingSchedule,
                                                      CustomData,
                                                      RequestId
                                                  );

                if (CustomNotifyEVChargingScheduleRequestParser is not null)
                    NotifyEVChargingScheduleRequest = CustomNotifyEVChargingScheduleRequestParser(JSON,
                                                                                            NotifyEVChargingScheduleRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyEVChargingScheduleRequest  = null;
                ErrorResponse                 = "The given JSON representation of a notify EV charging schedule request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyEVChargingScheduleRequestSerializer = null, CustomChargingNeedsSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyEVChargingScheduleRequestSerializer">A delegate to serialize custom NotifyEVChargingNeeds requests.</param>
        /// <param name="CustomChargingScheduleSerializer">A delegate to serialize custom charging schedules.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        /// <param name="CustomSalesTariffSerializer">A delegate to serialize custom salesTariffs.</param>
        /// <param name="CustomSalesTariffEntrySerializer">A delegate to serialize custom salesTariffEntrys.</param>
        /// <param name="CustomRelativeTimeIntervalSerializer">A delegate to serialize custom relativeTimeIntervals.</param>
        /// <param name="CustomConsumptionCostSerializer">A delegate to serialize custom consumptionCosts.</param>
        /// <param name="CustomCostSerializer">A delegate to serialize custom costs.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyEVChargingScheduleRequest>?  CustomNotifyEVChargingScheduleRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingSchedule>?                 CustomChargingScheduleSerializer                  = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?           CustomChargingSchedulePeriodSerializer            = null,
                              CustomJObjectSerializerDelegate<SalesTariff>?                      CustomSalesTariffSerializer                       = null,
                              CustomJObjectSerializerDelegate<SalesTariffEntry>?                 CustomSalesTariffEntrySerializer                  = null,
                              CustomJObjectSerializerDelegate<RelativeTimeInterval>?             CustomRelativeTimeIntervalSerializer              = null,
                              CustomJObjectSerializerDelegate<ConsumptionCost>?                  CustomConsumptionCostSerializer                   = null,
                              CustomJObjectSerializerDelegate<Cost>?                             CustomCostSerializer                              = null,
                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataSerializer                        = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("timeBase",           TimeBase.ToIso8601()),
                                 new JProperty("evseId",             EVSEId.  Value),

                                 new JProperty("chargingSchedule",   ChargingSchedule.ToJSON(CustomChargingScheduleSerializer,
                                                                                             CustomChargingSchedulePeriodSerializer,
                                                                                             CustomSalesTariffSerializer,
                                                                                             CustomSalesTariffEntrySerializer,
                                                                                             CustomRelativeTimeIntervalSerializer,
                                                                                             CustomConsumptionCostSerializer,
                                                                                             CustomCostSerializer,
                                                                                             CustomCustomDataSerializer)),

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomNotifyEVChargingScheduleRequestSerializer is not null
                       ? CustomNotifyEVChargingScheduleRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyEVChargingScheduleRequest1, NotifyEVChargingScheduleRequest2)

        /// <summary>
        /// Compares two notify EV charging schedule requests for equality.
        /// </summary>
        /// <param name="NotifyEVChargingScheduleRequest1">A notify EV charging schedule request.</param>
        /// <param name="NotifyEVChargingScheduleRequest2">Another notify EV charging schedule request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyEVChargingScheduleRequest? NotifyEVChargingScheduleRequest1,
                                           NotifyEVChargingScheduleRequest? NotifyEVChargingScheduleRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyEVChargingScheduleRequest1, NotifyEVChargingScheduleRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyEVChargingScheduleRequest1 is null || NotifyEVChargingScheduleRequest2 is null)
                return false;

            return NotifyEVChargingScheduleRequest1.Equals(NotifyEVChargingScheduleRequest2);

        }

        #endregion

        #region Operator != (NotifyEVChargingScheduleRequest1, NotifyEVChargingScheduleRequest2)

        /// <summary>
        /// Compares two notify EV charging schedule requests for inequality.
        /// </summary>
        /// <param name="NotifyEVChargingScheduleRequest1">A notify EV charging schedule request.</param>
        /// <param name="NotifyEVChargingScheduleRequest2">Another notify EV charging schedule request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyEVChargingScheduleRequest? NotifyEVChargingScheduleRequest1,
                                           NotifyEVChargingScheduleRequest? NotifyEVChargingScheduleRequest2)

            => !(NotifyEVChargingScheduleRequest1 == NotifyEVChargingScheduleRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyEVChargingScheduleRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify EV charging schedule requests for equality.
        /// </summary>
        /// <param name="Object">A notify EV charging schedule request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyEVChargingScheduleRequest notifyEVChargingNeedsRequest &&
                   Equals(notifyEVChargingNeedsRequest);

        #endregion

        #region Equals(NotifyEVChargingScheduleRequest)

        /// <summary>
        /// Compares two notify EV charging schedule requests for equality.
        /// </summary>
        /// <param name="NotifyEVChargingScheduleRequest">A notify EV charging schedule request to compare with.</param>
        public override Boolean Equals(NotifyEVChargingScheduleRequest? NotifyEVChargingScheduleRequest)

            => NotifyEVChargingScheduleRequest is not null &&

               TimeBase.        Equals(NotifyEVChargingScheduleRequest.TimeBase)         &&
               EVSEId.          Equals(NotifyEVChargingScheduleRequest.EVSEId)           &&
               ChargingSchedule.Equals(NotifyEVChargingScheduleRequest.ChargingSchedule) &&

               base.GenericEquals(NotifyEVChargingScheduleRequest);

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

                return TimeBase.        GetHashCode() * 7 ^
                       EVSEId.          GetHashCode() * 5 ^
                       ChargingSchedule.GetHashCode() * 3 ^

                       base.            GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

             => $"{TimeBase} - EVSE Id: {EVSEId}: {ChargingSchedule}";

        #endregion

    }

}
