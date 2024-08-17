﻿/*
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The SetChargingProfile request.
    /// </summary>
    public class SetChargingProfileRequest : ARequest<SetChargingProfileRequest>,
                                             IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/setChargingProfileRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The EVSE identification to which the charging profile applies.
        /// For TxDefaultProfile an evseId=0 applies the profile to each individual evse.
        /// For ChargingStationMaxProfile and ChargingStationExternalConstraints an evseId = 0 contains an overal limit for the whole charging station.
        /// </summary>
        [Mandatory]
        public EVSE_Id          EVSEId             { get; }

        /// <summary>
        /// The charging profile to be set.
        /// </summary>
        [Mandatory]
        public ChargingProfile  ChargingProfile    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetChargingProfile request.
        /// </summary>
        /// <param name="SourceRouting">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="EVSEId">The EVSE identification to which the charging profile applies.</param>
        /// <param name="ChargingProfile">The charging profile to be set.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public SetChargingProfileRequest(SourceRouting            Destination,
                                         EVSE_Id                  EVSEId,
                                         ChargingProfile          ChargingProfile,

                                         IEnumerable<KeyPair>?    SignKeys              = null,
                                         IEnumerable<SignInfo>?   SignInfos             = null,
                                         IEnumerable<Signature>?  Signatures            = null,

                                         CustomData?              CustomData            = null,

                                         Request_Id?              RequestId             = null,
                                         DateTime?                RequestTimestamp      = null,
                                         TimeSpan?                RequestTimeout        = null,
                                         EventTracking_Id?        EventTrackingId       = null,
                                         NetworkPath?             NetworkPath           = null,
                                         SerializationFormats?    SerializationFormat   = null,
                                         CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(SetChargingProfileRequest)[..^7],

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

            this.EVSEId           = EVSEId;
            this.ChargingProfile  = ChargingProfile;

            unchecked
            {
                hashCode = this.EVSEId.         GetHashCode() * 5 ^
                           this.ChargingProfile.GetHashCode() * 3 ^
                           base.                GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SetChargingProfileRequest",
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
        //       "description": "Charging_ Profile. Charging_ Profile_ Kind. Charging_ Profile_ Kind_ Code\r\nurn:x-oca:ocpp:uid:1:569232\r\nIndicates the kind of schedule.",
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
        //       "description": "Charging_ Schedule. Charging_ Rate_ Unit. Charging_ Rate_ Unit_ Code\r\nurn:x-oca:ocpp:uid:1:569238\r\nThe unit of measure Limit is expressed in.",
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
        //     "RecurrencyKindEnumType": {
        //       "description": "Charging_ Profile. Recurrency_ Kind. Recurrency_ Kind_ Code\r\nurn:x-oca:ocpp:uid:1:569233\r\nIndicates the start point of a recurrence.",
        //       "javaType": "RecurrencyKindEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Daily",
        //         "Weekly"
        //       ]
        //     },
        //     "ChargingProfileType": {
        //       "description": "Charging_ Profile\r\nurn:x-oca:ocpp:uid:2:233255\r\nA ChargingProfile consists of ChargingSchedule, describing the amount of power or current that can be delivered per time interval.",
        //       "javaType": "ChargingProfile",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "id": {
        //           "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nId of ChargingProfile.",
        //           "type": "integer"
        //         },
        //         "stackLevel": {
        //           "description": "Charging_ Profile. Stack_ Level. Counter\r\nurn:x-oca:ocpp:uid:1:569230\r\nValue determining level in hierarchy stack of profiles. Higher values have precedence over lower values. Lowest level is 0.",
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
        //           "description": "Charging_ Profile. Valid_ From. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569234\r\nPoint in time at which the profile starts to be valid. If absent, the profile is valid as soon as it is received by the Charging Station.",
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "validTo": {
        //           "description": "Charging_ Profile. Valid_ To. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569235\r\nPoint in time at which the profile stops to be valid. If absent, the profile is valid until it is replaced by another profile.",
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
        //           "description": "SHALL only be included if ChargingProfilePurpose is set to TxProfile. The transactionId is used to match the profile to a specific transaction.",
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
        //       "description": "Charging_ Schedule_ Period\r\nurn:x-oca:ocpp:uid:2:233257\r\nCharging schedule period structure defines a time period in a charging schedule.",
        //       "javaType": "ChargingSchedulePeriod",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "startPeriod": {
        //           "description": "Charging_ Schedule_ Period. Start_ Period. Elapsed_ Time\r\nurn:x-oca:ocpp:uid:1:569240\r\nStart of the period, in seconds from the start of schedule. The value of StartPeriod also defines the stop time of the previous period.",
        //           "type": "integer"
        //         },
        //         "limit": {
        //           "description": "Charging_ Schedule_ Period. Limit. Measure\r\nurn:x-oca:ocpp:uid:1:569241\r\nCharging rate limit during the schedule period, in the applicable chargingRateUnit, for example in Amperes (A) or Watts (W). Accepts at most one digit fraction (e.g. 8.1).",
        //           "type": "number"
        //         },
        //         "numberPhases": {
        //           "description": "Charging_ Schedule_ Period. Number_ Phases. Counter\r\nurn:x-oca:ocpp:uid:1:569242\r\nThe number of phases that can be used for charging. If a number of phases is needed, numberPhases=3 will be assumed unless another number is given.",
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
        //           "description": "Identifies the ChargingSchedule.",
        //           "type": "integer"
        //         },
        //         "startSchedule": {
        //           "description": "Charging_ Schedule. Start_ Schedule. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569237\r\nStarting point of an absolute schedule. If absent the schedule will be relative to start of charging.",
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "duration": {
        //           "description": "Charging_ Schedule. Duration. Elapsed_ Time\r\nurn:x-oca:ocpp:uid:1:569236\r\nDuration of the charging schedule in seconds. If the duration is left empty, the last period will continue indefinitely or until end of the transaction if chargingProfilePurpose = TxProfile.",
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
        //           "description": "Consumption_ Cost. Start_ Value. Numeric\r\nurn:x-oca:ocpp:uid:1:569246\r\nThe lowest level of consumption that defines the starting point of this consumption block. The block interval extends to the start of the next interval.",
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
        //           "description": "Relative_ Timer_ Interval. Start. Elapsed_ Time\r\nurn:x-oca:ocpp:uid:1:569279\r\nStart of the interval, in seconds from NOW.",
        //           "type": "integer"
        //         },
        //         "duration": {
        //           "description": "Relative_ Timer_ Interval. Duration. Elapsed_ Time\r\nurn:x-oca:ocpp:uid:1:569280\r\nDuration of the interval, in seconds.",
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
        //           "description": "Sales_ Tariff_ Entry. E_ Price_ Level. Unsigned_ Integer\r\nurn:x-oca:ocpp:uid:1:569281\r\nDefines the price level of this SalesTariffEntry (referring to NumEPriceLevels). Small values for the EPriceLevel represent a cheaper TariffEntry. Large values for the EPriceLevel represent a more expensive TariffEntry.",
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
        //       "description": "Sales_ Tariff\r\nurn:x-oca:ocpp:uid:2:233272\r\nNOTE: This dataType is based on dataTypes from &lt;&lt;ref-ISOIEC15118-2,ISO 15118-2&gt;&gt;.",
        //       "javaType": "SalesTariff",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "id": {
        //           "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nSalesTariff identifier used to identify one sales tariff. An SAID remains a unique identifier for one schedule throughout a charging session.",
        //           "type": "integer"
        //         },
        //         "salesTariffDescription": {
        //           "description": "Sales_ Tariff. Sales. Tariff_ Description\r\nurn:x-oca:ocpp:uid:1:569283\r\nA human readable title/short description of the sales tariff e.g. for HMI display purposes.",
        //           "type": "string",
        //           "maxLength": 32
        //         },
        //         "numEPriceLevels": {
        //           "description": "Sales_ Tariff. Num_ E_ Price_ Levels. Counter\r\nurn:x-oca:ocpp:uid:1:569284\r\nDefines the overall number of distinct price levels used across all provided SalesTariff elements.",
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
        //       "description": "For TxDefaultProfile an evseId=0 applies the profile to each individual evse. For ChargingStationMaxProfile and ChargingStationExternalConstraints an evseId=0 contains an overal limit for the whole Charging Station.",
        //       "type": "integer"
        //     },
        //     "chargingProfile": {
        //       "$ref": "#/definitions/ChargingProfileType"
        //     }
        //   },
        //   "required": [
        //     "evseId",
        //     "chargingProfile"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomSetChargingProfileRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SetChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="SourceRouting">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomSetChargingProfileRequestParser">A delegate to parse custom SetChargingProfile requests.</param>
        public static SetChargingProfileRequest Parse(JObject                                                  JSON,
                                                      Request_Id                                               RequestId,
                                                      SourceRouting                                            SourceRouting,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTime?                                                RequestTimestamp                        = null,
                                                      TimeSpan?                                                RequestTimeout                          = null,
                                                      EventTracking_Id?                                        EventTrackingId                         = null,
                                                      CustomJObjectParserDelegate<SetChargingProfileRequest>?  CustomSetChargingProfileRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                             SourceRouting,
                         NetworkPath,
                         out var setChargingProfileRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSetChargingProfileRequestParser))
            {
                return setChargingProfileRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetChargingProfile request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out SetChargingProfileRequest, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SetChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="SourceRouting">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetChargingProfileRequest">The parsed SetChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetChargingProfileRequestParser">A delegate to parse custom SetChargingProfile requests.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       SourceRouting                                            SourceRouting,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out SetChargingProfileRequest?      SetChargingProfileRequest,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                RequestTimestamp                        = null,
                                       TimeSpan?                                                RequestTimeout                          = null,
                                       EventTracking_Id?                                        EventTrackingId                         = null,
                                       CustomJObjectParserDelegate<SetChargingProfileRequest>?  CustomSetChargingProfileRequestParser   = null)
        {

            try
            {

                SetChargingProfileRequest = null;

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

                #region ChargingProfile      [mandatory]

                if (!JSON.ParseMandatoryJSON("chargingProfile",
                                             "charging profiles",
                                             OCPPv2_1.ChargingProfile.TryParse,
                                             out ChargingProfile? ChargingProfile,
                                             out ErrorResponse) ||
                     ChargingProfile is null)
                {
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetChargingProfileRequest = new SetChargingProfileRequest(

                                                    SourceRouting,
                                                EVSEId,
                                                ChargingProfile,

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

                if (CustomSetChargingProfileRequestParser is not null)
                    SetChargingProfileRequest = CustomSetChargingProfileRequestParser(JSON,
                                                                                      SetChargingProfileRequest);

                return true;

            }
            catch (Exception e)
            {
                SetChargingProfileRequest  = null;
                ErrorResponse              = "The given JSON representation of a SetChargingProfile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetChargingProfileRequestSerializer = null, CustomChargingProfileSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetChargingProfileRequestSerializer">A delegate to serialize custom SetChargingProfile requests.</param>
        /// <param name="CustomChargingProfileSerializer">A delegate to serialize custom charging profiles.</param>
        /// <param name="CustomLimitBeyondSoCSerializer">A delegate to serialize custom charging schedules.</param>
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
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetChargingProfileRequest>?                           CustomSetChargingProfileRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingProfile>?                                     CustomChargingProfileSerializer             = null,
                              CustomJObjectSerializerDelegate<LimitBeyondSoC>?                                      CustomLimitBeyondSoCSerializer              = null,
                              CustomJObjectSerializerDelegate<ChargingSchedule>?                                    CustomChargingScheduleSerializer            = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                              CustomChargingSchedulePeriodSerializer      = null,
                              CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                                    CustomV2XFreqWattEntrySerializer            = null,
                              CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                                  CustomV2XSignalWattEntrySerializer          = null,
                              CustomJObjectSerializerDelegate<SalesTariff>?                                         CustomSalesTariffSerializer                 = null,
                              CustomJObjectSerializerDelegate<SalesTariffEntry>?                                    CustomSalesTariffEntrySerializer            = null,
                              CustomJObjectSerializerDelegate<RelativeTimeInterval>?                                CustomRelativeTimeIntervalSerializer        = null,
                              CustomJObjectSerializerDelegate<ConsumptionCost>?                                     CustomConsumptionCostSerializer             = null,
                              CustomJObjectSerializerDelegate<Cost>?                                                CustomCostSerializer                        = null,

                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AbsolutePriceSchedule>?    CustomAbsolutePriceScheduleSerializer       = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRuleStack>?           CustomPriceRuleStackSerializer              = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRule>?                CustomPriceRuleSerializer                   = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.TaxRule>?                  CustomTaxRuleSerializer                     = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRuleList>?         CustomOverstayRuleListSerializer            = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRule>?             CustomOverstayRuleSerializer                = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AdditionalService>?        CustomAdditionalServiceSerializer           = null,

                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelSchedule>?       CustomPriceLevelScheduleSerializer          = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntrySerializer     = null,

                              CustomJObjectSerializerDelegate<Signature>?                                           CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("evseId",            EVSEId.Value),
                                 new JProperty("chargingProfile",   ChargingProfile.ToJSON(CustomChargingProfileSerializer,
                                                                                           CustomLimitBeyondSoCSerializer,
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

                                                                                           CustomCustomDataSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetChargingProfileRequestSerializer is not null
                       ? CustomSetChargingProfileRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetChargingProfileRequest1, SetChargingProfileRequest2)

        /// <summary>
        /// Compares two SetChargingProfile requests for equality.
        /// </summary>
        /// <param name="SetChargingProfileRequest1">A SetChargingProfile request.</param>
        /// <param name="SetChargingProfileRequest2">Another SetChargingProfile request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetChargingProfileRequest? SetChargingProfileRequest1,
                                           SetChargingProfileRequest? SetChargingProfileRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetChargingProfileRequest1, SetChargingProfileRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetChargingProfileRequest1 is null || SetChargingProfileRequest2 is null)
                return false;

            return SetChargingProfileRequest1.Equals(SetChargingProfileRequest2);

        }

        #endregion

        #region Operator != (SetChargingProfileRequest1, SetChargingProfileRequest2)

        /// <summary>
        /// Compares two SetChargingProfile requests for inequality.
        /// </summary>
        /// <param name="SetChargingProfileRequest1">A SetChargingProfile request.</param>
        /// <param name="SetChargingProfileRequest2">Another SetChargingProfile request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetChargingProfileRequest? SetChargingProfileRequest1,
                                           SetChargingProfileRequest? SetChargingProfileRequest2)

            => !(SetChargingProfileRequest1 == SetChargingProfileRequest2);

        #endregion

        #endregion

        #region IEquatable<SetChargingProfileRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetChargingProfile requests for equality.
        /// </summary>
        /// <param name="Object">A SetChargingProfile request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetChargingProfileRequest setChargingProfileRequest &&
                   Equals(setChargingProfileRequest);

        #endregion

        #region Equals(SetChargingProfileRequest)

        /// <summary>
        /// Compares two SetChargingProfile requests for equality.
        /// </summary>
        /// <param name="SetChargingProfileRequest">A SetChargingProfile request to compare with.</param>
        public override Boolean Equals(SetChargingProfileRequest? SetChargingProfileRequest)

            => SetChargingProfileRequest is not null &&

               EVSEId.         Equals(SetChargingProfileRequest.EVSEId)          &&
               ChargingProfile.Equals(SetChargingProfileRequest.ChargingProfile) &&

               base.    GenericEquals(SetChargingProfileRequest);

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

            => $"Set charging profile '{ChargingProfile.ChargingProfileId}' for EVSE {EVSEId}";

        #endregion

    }

}
