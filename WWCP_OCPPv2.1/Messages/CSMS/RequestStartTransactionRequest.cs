﻿/*
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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The request start transaction request.
    /// </summary>
    public class RequestStartTransactionRequest : ARequest<RequestStartTransactionRequest>
    {

        #region Properties

        /// <summary>
        /// Request identification given by the server to this start request.
        /// The charging station might return this in the TransactionEventRequest, letting the server know which transaction was started for this request.
        /// </summary>
        [Mandatory]
        public RemoteStart_Id    RequestStartTransactionRequestId    { get; }

        /// <summary>
        /// The identification token to start the charging transaction.
        /// </summary>
        [Mandatory]
        public IdToken           IdToken                             { get; }

        /// <summary>
        /// An optional EVSE identification on which the charging
        /// transaction should be started (SHALL be > 0).
        /// </summary>
        [Optional]
        public EVSE_Id?          EVSEId                              { get; }

        /// <summary>
        /// An optional charging profile to be used by the charging station
        /// for the requested charging transaction.
        /// The 'ChargingProfilePurpose' MUST be set to 'TxProfile'.
        /// </summary>
        public ChargingProfile?  ChargingProfile                     { get; }

        /// <summary>
        /// The optional group identification that the charging station must use to start a transaction.
        /// </summary>
        [Optional]
        public IdToken?          GroupIdToken                        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new request start transaction request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RequestStartTransactionRequestId">Request identification given by the server to this start request. The charging station might return this in the TransactionEventRequest, letting the server know which transaction was started for this request.</param>
        /// <param name="IdToken">The identification token to start the charging transaction.</param>
        /// <param name="EVSEId">An optional EVSE identification on which the charging transaction should be started (SHALL be > 0).</param>
        /// <param name="ChargingProfile">An optional charging profile to be used by the charging station for the requested charging transaction.</param>
        /// <param name="GroupIdToken">An optional group identifier.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public RequestStartTransactionRequest(ChargeBox_Id        ChargeBoxId,
                                              RemoteStart_Id      RequestStartTransactionRequestId,
                                              IdToken             IdToken,
                                              EVSE_Id?            EVSEId              = null,
                                              ChargingProfile?    ChargingProfile     = null,
                                              IdToken?            GroupIdToken        = null,
                                              CustomData?         CustomData          = null,

                                              Request_Id?         RequestId           = null,
                                              DateTime?           RequestTimestamp    = null,
                                              TimeSpan?           RequestTimeout      = null,
                                              EventTracking_Id?   EventTrackingId     = null,
                                              CancellationToken?  CancellationToken   = null)

            : base(ChargeBoxId,
                   "RequestStartTransaction",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.RequestStartTransactionRequestId  = RequestStartTransactionRequestId;
            this.IdToken                           = IdToken;
            this.EVSEId                            = EVSEId;
            this.ChargingProfile                   = ChargingProfile;
            this.GroupIdToken                      = GroupIdToken;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:RequestStartTransactionRequest",
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
        //     "ChargingProfileKindEnumType": {
        //       "description": "Charging_ Profile. Charging_ Profile_ Kind. Charging_ Profile_ Kind_ Code\r\nurn:x-oca:ocpp:uid:1:569232\r\nIndicates the kind of schedule.\r\n",
        //       "javaType": "ChargingProfileKindEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Absolute",
        //         "Recurring",
        //         "Relative"
        //       ]
        //     },
        //     "ChargingProfilePurposeEnumType": {
        //       "description": "Charging_ Profile. Charging_ Profile_ Purpose. Charging_ Profile_ Purpose_ Code\r\nurn:x-oca:ocpp:uid:1:569231\r\nDefines the purpose of the schedule transferred by this profile\r\n",
        //       "javaType": "ChargingProfilePurposeEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "ChargingStationExternalConstraints",
        //         "ChargingStationMaxProfile",
        //         "TxDefaultProfile",
        //         "TxProfile"
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
        //     "IdTokenEnumType": {
        //       "description": "Enumeration of possible idToken types.\r\n",
        //       "javaType": "IdTokenEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Central",
        //         "eMAID",
        //         "ISO14443",
        //         "ISO15693",
        //         "KeyCode",
        //         "Local",
        //         "MacAddress",
        //         "NoAuthorization"
        //       ]
        //     },
        //     "RecurrencyKindEnumType": {
        //       "description": "Charging_ Profile. Recurrency_ Kind. Recurrency_ Kind_ Code\r\nurn:x-oca:ocpp:uid:1:569233\r\nIndicates the start point of a recurrence.\r\n",
        //       "javaType": "RecurrencyKindEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Daily",
        //         "Weekly"
        //       ]
        //     },
        //     "AdditionalInfoType": {
        //       "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.\r\n",
        //       "javaType": "AdditionalInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "additionalIdToken": {
        //           "description": "This field specifies the additional IdToken.\r\n",
        //           "type": "string",
        //           "maxLength": 36
        //         },
        //         "type": {
        //           "description": "This defines the type of the additionalIdToken. This is a custom type, so the implementation needs to be agreed upon by all involved parties.\r\n",
        //           "type": "string",
        //           "maxLength": 50
        //         }
        //       },
        //       "required": [
        //         "additionalIdToken",
        //         "type"
        //       ]
        //     },
        //     "ChargingProfileType": {
        //       "description": "Charging_ Profile\r\nurn:x-oca:ocpp:uid:2:233255\r\nA ChargingProfile consists of ChargingSchedule, describing the amount of power or current that can be delivered per time interval.\r\n",
        //       "javaType": "ChargingProfile",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "id": {
        //           "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nId of ChargingProfile.\r\n",
        //           "type": "integer"
        //         },
        //         "stackLevel": {
        //           "description": "Charging_ Profile. Stack_ Level. Counter\r\nurn:x-oca:ocpp:uid:1:569230\r\nValue determining level in hierarchy stack of profiles. Higher values have precedence over lower values. Lowest level is 0.\r\n",
        //           "type": "integer"
        //         },
        //         "chargingProfilePurpose": {
        //           "$ref": "#/definitions/ChargingProfilePurposeEnumType"
        //         },
        //         "chargingProfileKind": {
        //           "$ref": "#/definitions/ChargingProfileKindEnumType"
        //         },
        //         "recurrencyKind": {
        //           "$ref": "#/definitions/RecurrencyKindEnumType"
        //         },
        //         "validFrom": {
        //           "description": "Charging_ Profile. Valid_ From. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569234\r\nPoint in time at which the profile starts to be valid. If absent, the profile is valid as soon as it is received by the Charging Station.\r\n",
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "validTo": {
        //           "description": "Charging_ Profile. Valid_ To. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569235\r\nPoint in time at which the profile stops to be valid. If absent, the profile is valid until it is replaced by another profile.\r\n",
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "chargingSchedule": {
        //           "type": "array",
        //           "additionalItems": false,
        //           "items": {
        //             "$ref": "#/definitions/ChargingScheduleType"
        //           },
        //           "minItems": 1,
        //           "maxItems": 3
        //         },
        //         "transactionId": {
        //           "description": "SHALL only be included if ChargingProfilePurpose is set to TxProfile. The transactionId is used to match the profile to a specific transaction.\r\n",
        //           "type": "string",
        //           "maxLength": 36
        //         }
        //       },
        //       "required": [
        //         "id",
        //         "stackLevel",
        //         "chargingProfilePurpose",
        //         "chargingProfileKind",
        //         "chargingSchedule"
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
        //     "IdTokenType": {
        //       "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.\r\n",
        //       "javaType": "IdToken",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "additionalInfo": {
        //           "type": "array",
        //           "additionalItems": false,
        //           "items": {
        //             "$ref": "#/definitions/AdditionalInfoType"
        //           },
        //           "minItems": 1
        //         },
        //         "idToken": {
        //           "description": "IdToken is case insensitive. Might hold the hidden id of an RFID tag, but can for example also contain a UUID.\r\n",
        //           "type": "string",
        //           "maxLength": 36
        //         },
        //         "type": {
        //           "$ref": "#/definitions/IdTokenEnumType"
        //         }
        //       },
        //       "required": [
        //         "idToken",
        //         "type"
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
        //     "evseId": {
        //       "description": "Number of the EVSE on which to start the transaction. EvseId SHALL be &gt; 0\r\n",
        //       "type": "integer"
        //     },
        //     "groupIdToken": {
        //       "$ref": "#/definitions/IdTokenType"
        //     },
        //     "idToken": {
        //       "$ref": "#/definitions/IdTokenType"
        //     },
        //     "remoteStartId": {
        //       "description": "Id given by the server to this start request. The Charging Station might return this in the &lt;&lt;transactioneventrequest, TransactionEventRequest&gt;&gt;, letting the server know which transaction was started for this request. Use to start a transaction.\r\n",
        //       "type": "integer"
        //     },
        //     "chargingProfile": {
        //       "$ref": "#/definitions/ChargingProfileType"
        //     }
        //   },
        //   "required": [
        //     "remoteStartId",
        //     "idToken"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomRequestStartTransactionRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a request start transaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomRequestStartTransactionRequestParser">A delegate to parse custom request start transaction requests.</param>
        public static RequestStartTransactionRequest Parse(JObject                                                       JSON,
                                                           Request_Id                                                    RequestId,
                                                           ChargeBox_Id                                                  ChargeBoxId,
                                                           CustomJObjectParserDelegate<RequestStartTransactionRequest>?  CustomRequestStartTransactionRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var requestStartTransactionRequest,
                         out var errorResponse,
                         CustomRequestStartTransactionRequestParser))
            {
                return requestStartTransactionRequest!;
            }

            throw new ArgumentException("The given JSON representation of a request start transaction request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out RequestStartTransactionRequest, out ErrorResponse, CustomRequestStartTransactionRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a request start transaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RequestStartTransactionRequest">The parsed request start transaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       Request_Id                           RequestId,
                                       ChargeBox_Id                         ChargeBoxId,
                                       out RequestStartTransactionRequest?  RequestStartTransactionRequest,
                                       out String?                          ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out RequestStartTransactionRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a request start transaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RequestStartTransactionRequest">The parsed request start transaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRequestStartTransactionRequestParser">A delegate to parse custom request start transaction requests.</param>
        public static Boolean TryParse(JObject                                                       JSON,
                                       Request_Id                                                    RequestId,
                                       ChargeBox_Id                                                  ChargeBoxId,
                                       out RequestStartTransactionRequest?                           RequestStartTransactionRequest,
                                       out String?                                                   ErrorResponse,
                                       CustomJObjectParserDelegate<RequestStartTransactionRequest>?  CustomRequestStartTransactionRequestParser)
        {

            try
            {

                RequestStartTransactionRequest = null;

                #region RequestStartTransactionRequestId    [mandatory]

                if (!JSON.ParseMandatory("remoteStartId",
                                         "remote start identification",
                                         RemoteStart_Id.TryParse,
                                         out RemoteStart_Id RequestStartTransactionRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IdToken                             [mandatory]

                if (!JSON.ParseMandatoryJSON("idToken",
                                             "identification token",
                                             OCPPv2_1.IdToken.TryParse,
                                             out IdToken? IdToken,
                                             out ErrorResponse) ||
                     IdToken is null)
                {
                    return false;
                }

                #endregion

                #region EVSEId                              [optional]

                if (JSON.ParseOptional("evseId",
                                       "EVSE identification",
                                       EVSE_Id.TryParse,
                                       out EVSE_Id? EVSEId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingProfile                     [optional]

                if (JSON.ParseOptionalJSON("chargingProfile",
                                           "charging profile",
                                           OCPPv2_1.ChargingProfile.TryParse,
                                           out ChargingProfile ChargingProfile,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region GroupIdToken                        [optional]

                if (JSON.ParseOptionalJSON("groupIdToken",
                                           "group identification token",
                                           OCPPv2_1.IdToken.TryParse,
                                           out IdToken? GroupIdToken,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                          [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId                         [optional, OCPP_CSE]

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


                RequestStartTransactionRequest = new RequestStartTransactionRequest(ChargeBoxId,
                                                                                    RequestStartTransactionRequestId,
                                                                                    IdToken,
                                                                                    EVSEId,
                                                                                    ChargingProfile,
                                                                                    GroupIdToken,
                                                                                    CustomData,
                                                                                    RequestId);

                if (CustomRequestStartTransactionRequestParser is not null)
                    RequestStartTransactionRequest = CustomRequestStartTransactionRequestParser(JSON,
                                                                                                RequestStartTransactionRequest);

                return true;

            }
            catch (Exception e)
            {
                RequestStartTransactionRequest  = null;
                ErrorResponse                   = "The given JSON representation of a request start transaction request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomRequestStartTransactionRequestSerializer = null, CustomIdTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRequestStartTransactionRequestSerializer">A delegate to serialize custom request start transaction requests.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom IdTokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomChargingProfileSerializer">A delegate to serialize custom charging profiles.</param>
        /// <param name="CustomChargingScheduleSerializer">A delegate to serialize custom charging schedule requests.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RequestStartTransactionRequest>?  CustomRequestStartTransactionRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<IdToken>?                         CustomIdTokenSerializer                          = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?                  CustomAdditionalInfoSerializer                   = null,
                              CustomJObjectSerializerDelegate<ChargingProfile>?                 CustomChargingProfileSerializer                  = null,
                              CustomJObjectSerializerDelegate<ChargingSchedule>?                CustomChargingScheduleSerializer                 = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?          CustomChargingSchedulePeriodSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("remoteStartId",     RequestStartTransactionRequestId.Value),
                                 new JProperty("idToken",           IdToken.        ToJSON(CustomIdTokenSerializer,
                                                                                           CustomAdditionalInfoSerializer,
                                                                                           CustomCustomDataSerializer)),

                           EVSEId.HasValue
                               ? new JProperty("evseId",            EVSEId.Value.Value)
                               : null,

                           ChargingProfile is not null
                               ? new JProperty("chargingProfile",   ChargingProfile.ToJSON(CustomChargingProfileSerializer,
                                                                                           CustomChargingScheduleSerializer,
                                                                                           CustomChargingSchedulePeriodSerializer))
                               : null,

                           GroupIdToken is not null
                               ? new JProperty("groupIdToken",      GroupIdToken.   ToJSON(CustomIdTokenSerializer,
                                                                                           CustomAdditionalInfoSerializer,
                                                                                           CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomRequestStartTransactionRequestSerializer is not null
                       ? CustomRequestStartTransactionRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (RequestStartTransactionRequest1, RequestStartTransactionRequest2)

        /// <summary>
        /// Compares two request start transaction requests for equality.
        /// </summary>
        /// <param name="RequestStartTransactionRequest1">A request start transaction request.</param>
        /// <param name="RequestStartTransactionRequest2">Another request start transaction request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RequestStartTransactionRequest? RequestStartTransactionRequest1,
                                           RequestStartTransactionRequest? RequestStartTransactionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RequestStartTransactionRequest1, RequestStartTransactionRequest2))
                return true;

            // If one is null, but not both, return false.
            if (RequestStartTransactionRequest1 is null || RequestStartTransactionRequest2 is null)
                return false;

            return RequestStartTransactionRequest1.Equals(RequestStartTransactionRequest2);

        }

        #endregion

        #region Operator != (RequestStartTransactionRequest1, RequestStartTransactionRequest2)

        /// <summary>
        /// Compares two request start transaction requests for inequality.
        /// </summary>
        /// <param name="RequestStartTransactionRequest1">A request start transaction request.</param>
        /// <param name="RequestStartTransactionRequest2">Another request start transaction request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RequestStartTransactionRequest? RequestStartTransactionRequest1,
                                           RequestStartTransactionRequest? RequestStartTransactionRequest2)

            => !(RequestStartTransactionRequest1 == RequestStartTransactionRequest2);

        #endregion

        #endregion

        #region IEquatable<RequestStartTransactionRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two request start transaction requests for equality.
        /// </summary>
        /// <param name="Object">A request start transaction request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RequestStartTransactionRequest requestStartTransactionRequest &&
                   Equals(requestStartTransactionRequest);

        #endregion

        #region Equals(RequestStartTransactionRequest)

        /// <summary>
        /// Compares two request start transaction requests for equality.
        /// </summary>
        /// <param name="RequestStartTransactionRequest">A request start transaction request to compare with.</param>
        public override Boolean Equals(RequestStartTransactionRequest? RequestStartTransactionRequest)

            => RequestStartTransactionRequest is not null &&

               RequestStartTransactionRequestId.Equals(RequestStartTransactionRequest.RequestStartTransactionRequestId) &&
               IdToken.                         Equals(RequestStartTransactionRequest.IdToken)                          &&

            ((!EVSEId.         HasValue    && !RequestStartTransactionRequest.EVSEId.         HasValue)    ||
              (EVSEId.         HasValue    &&  RequestStartTransactionRequest.EVSEId.         HasValue    && EVSEId.   Value.Equals(RequestStartTransactionRequest.EVSEId.Value)))    &&

             ((ChargingProfile is     null &&  RequestStartTransactionRequest.ChargingProfile is     null) ||
              (ChargingProfile is not null &&  RequestStartTransactionRequest.ChargingProfile is not null && ChargingProfile.Equals(RequestStartTransactionRequest.ChargingProfile))) &&

             ((GroupIdToken    is     null &&  RequestStartTransactionRequest.GroupIdToken    is     null) ||
              (GroupIdToken    is not null &&  RequestStartTransactionRequest.GroupIdToken    is not null && GroupIdToken.   Equals(RequestStartTransactionRequest.GroupIdToken)))    &&

               base.GenericEquals(RequestStartTransactionRequest);

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

                return RequestStartTransactionRequestId.GetHashCode()       * 13 ^
                       IdToken.                         GetHashCode()       * 11 ^
                      (EVSEId?.                         GetHashCode() ?? 0) *  7 ^
                      (ChargingProfile?.                GetHashCode() ?? 0) *  5 ^
                      (GroupIdToken?.                   GetHashCode() ?? 0) *  3 ^

                       base.                            GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   RequestStartTransactionRequestId, ", ",
                   IdToken,

                   EVSEId.HasValue
                       ? " at " + IdToken : "",

                   ChargingProfile is not null
                       ? " with profile"
                       : ""

               );

        #endregion

    }

}
