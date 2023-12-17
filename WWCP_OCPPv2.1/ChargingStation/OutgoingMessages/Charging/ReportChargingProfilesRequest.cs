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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A report charging profiles request.
    /// </summary>
    public class ReportChargingProfilesRequest : ARequest<ReportChargingProfilesRequest>,
                                                 IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/reportChargingProfilesRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                 Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The request identification used to match the GetChargingProfilesRequest message with the resulting ReportChargingProfilesRequest messages.
        /// When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.
        /// </summary>
        [Mandatory]
        public Int32                         ReportChargingProfilesRequestId    { get; }

        /// <summary>
        /// The source that has installed this charging profile.
        /// </summary>
        [Mandatory]
        public ChargingLimitSource           ChargingLimitSource                { get; }

        /// <summary>
        /// The evse to which the charging profile applies.
        /// If evseId = 0, the message contains an overall limit for the charging station.
        /// </summary>
        [Mandatory]
        public EVSE_Id                       EVSEId                             { get; }

        /// <summary>
        /// The enumeration of charging profiles.
        /// </summary>
        [Mandatory]
        public IEnumerable<ChargingProfile>  ChargingProfiles                   { get; }

        /// <summary>
        /// The optional "to be continued" indicator whether another part of the charging profiles follows.
        /// Default value when omitted is false.
        /// </summary>
        [Optional]
        public Boolean?                      ToBeContinued                      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a report charging profiles request.
        /// </summary>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="ReportChargingProfilesRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting ReportChargingProfilesRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
        /// <param name="ChargingLimitSource">The source that has installed this charging profile.</param>
        /// <param name="EVSEId">The evse to which the charging profile applies. If evseId = 0, the message contains an overall limit for the charging station.</param>
        /// <param name="ChargingProfiles">The enumeration of charging profiles.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the charging profiles follows. Default value when omitted is false.</param>
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
        public ReportChargingProfilesRequest(NetworkingNode_Id             NetworkingNodeId,
                                             Int32                         ReportChargingProfilesRequestId,
                                             ChargingLimitSource           ChargingLimitSource,
                                             EVSE_Id                       EVSEId,
                                             IEnumerable<ChargingProfile>  ChargingProfiles,
                                             Boolean?                      ToBeContinued       = null,

                                             IEnumerable<KeyPair>?         SignKeys            = null,
                                             IEnumerable<SignInfo>?        SignInfos           = null,
                                             IEnumerable<OCPP.Signature>?  Signatures          = null,

                                             CustomData?                   CustomData          = null,

                                             Request_Id?                   RequestId           = null,
                                             DateTime?                     RequestTimestamp    = null,
                                             TimeSpan?                     RequestTimeout      = null,
                                             EventTracking_Id?             EventTrackingId     = null,
                                             NetworkPath?                  NetworkPath         = null,
                                             CancellationToken             CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(ReportChargingProfilesRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   CancellationToken)

        {

            if (!ChargingProfiles.Any())
                throw new ArgumentException("The given enumeration of charging profiles must not be empty!",
                                            nameof(ChargingProfiles));

            this.ReportChargingProfilesRequestId  = ReportChargingProfilesRequestId;
            this.ChargingLimitSource              = ChargingLimitSource;
            this.EVSEId                           = EVSEId;
            this.ChargingProfiles                 = ChargingProfiles.Distinct();
            this.ToBeContinued                    = ToBeContinued;


            unchecked
            {

                hashCode = this.ReportChargingProfilesRequestId.GetHashCode()       * 17 ^
                           this.ChargingLimitSource.            GetHashCode()       * 13 ^
                           this.EVSEId.                         GetHashCode()       * 11 ^
                           this.ChargingProfiles.               CalcHashCode()      *  7 ^
                          (this.ToBeContinued?.                 GetHashCode() ?? 0) *  5 ^
                          (this.CustomData?.                    GetHashCode() ?? 0) *  3 ^
                           base.                                GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ReportChargingProfilesRequest",
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
        //       "description": "Source that has installed this charging profile.\r\n",
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
        //     "requestId": {
        //       "description": "Id used to match the &lt;&lt;getchargingprofilesrequest, GetChargingProfilesRequest&gt;&gt; message with the resulting ReportChargingProfilesRequest messages. When the CSMS provided a requestId in the &lt;&lt;getchargingprofilesrequest, GetChargingProfilesRequest&gt;&gt;, this field SHALL contain the same value.\r\n",
        //       "type": "integer"
        //     },
        //     "chargingLimitSource": {
        //       "$ref": "#/definitions/ChargingLimitSourceEnumType"
        //     },
        //     "chargingProfile": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/ChargingProfileType"
        //       },
        //       "minItems": 1
        //     },
        //     "tbc": {
        //       "description": "To Be Continued. Default value when omitted: false. false indicates that there are no further messages as part of this report.\r\n",
        //       "type": "boolean",
        //       "default": false
        //     },
        //     "evseId": {
        //       "description": "The evse to which the charging profile applies. If evseId = 0, the message contains an overall limit for the Charging Station.\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "requestId",
        //     "chargingLimitSource",
        //     "evseId",
        //     "chargingProfile"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomReportChargingProfilesRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a report charging profiles request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomReportChargingProfilesRequestParser">A delegate to parse custom report charging profiles requests.</param>
        public static ReportChargingProfilesRequest Parse(JObject                                                      JSON,
                                                          Request_Id                                                   RequestId,
                                                          NetworkingNode_Id                                            NetworkingNodeId,
                                                          NetworkPath                                                  NetworkPath,
                                                          CustomJObjectParserDelegate<ReportChargingProfilesRequest>?  CustomReportChargingProfilesRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var reportChargingProfilesRequest,
                         out var errorResponse,
                         CustomReportChargingProfilesRequestParser) &&
                reportChargingProfilesRequest is not null)
            {
                return reportChargingProfilesRequest;
            }

            throw new ArgumentException("The given JSON representation of a report charging profiles request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out ReportChargingProfilesRequest, out ErrorResponse, CustomReportChargingProfilesRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a report charging profiles request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ReportChargingProfilesRequest">The parsed report charging profiles request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       Request_Id                          RequestId,
                                       NetworkingNode_Id                   NetworkingNodeId,
                                       NetworkPath                         NetworkPath,
                                       out ReportChargingProfilesRequest?  ReportChargingProfilesRequest,
                                       out String?                         ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out ReportChargingProfilesRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a report charging profiles request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="ReportChargingProfilesRequest">The parsed report charging profiles request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomReportChargingProfilesRequestParser">A delegate to parse custom report charging profiles requests.</param>
        public static Boolean TryParse(JObject                                                      JSON,
                                       Request_Id                                                   RequestId,
                                       NetworkingNode_Id                                            NetworkingNodeId,
                                       NetworkPath                                                  NetworkPath,
                                       out ReportChargingProfilesRequest?                           ReportChargingProfilesRequest,
                                       out String?                                                  ErrorResponse,
                                       CustomJObjectParserDelegate<ReportChargingProfilesRequest>?  CustomReportChargingProfilesRequestParser)
        {

            try
            {

                ReportChargingProfilesRequest = null;

                #region ReportChargingProfilesRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "report charging profiles request identification",
                                         out Int32 ReportChargingProfilesRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingLimitSource                [mandatory]

                if (!JSON.ParseMandatory("chargingLimitSource",
                                         "charging limit source",
                                         OCPPv2_1.ChargingLimitSource.TryParse,
                                         out ChargingLimitSource ChargingLimitSource,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVSEId                             [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "evse identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingProfiles                   [mandatory]

                if (!JSON.ParseMandatoryHashSet("chargingProfile",
                                                "charging profiles",
                                                ChargingProfile.TryParse,
                                                out HashSet<ChargingProfile> ChargingProfiles,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ToBeContinued                      [optional]

                if (JSON.ParseOptional("tbc",
                                       "to be continued",
                                       out Boolean? ToBeContinued,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                         [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                         [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ReportChargingProfilesRequest = new ReportChargingProfilesRequest(

                                                    NetworkingNodeId,
                                                    ReportChargingProfilesRequestId,
                                                    ChargingLimitSource,
                                                    EVSEId,
                                                    ChargingProfiles,
                                                    ToBeContinued,

                                                    null,
                                                    null,
                                                    Signatures,

                                                    CustomData,

                                                    RequestId,
                                                    null,
                                                    null,
                                                    null,
                                                    NetworkPath

                                                );

                if (CustomReportChargingProfilesRequestParser is not null)
                    ReportChargingProfilesRequest = CustomReportChargingProfilesRequestParser(JSON,
                                                                                              ReportChargingProfilesRequest);

                return true;

            }
            catch (Exception e)
            {
                ReportChargingProfilesRequest  = null;
                ErrorResponse                  = "The given JSON representation of a report charging profiles request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomReportChargingProfilesRequestSerializer = null, CustomChargingProfileSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomReportChargingProfilesRequestSerializer">A delegate to serialize custom ReportChargingProfiles requests.</param>
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
        public JObject ToJSON(CustomJObjectSerializerDelegate<ReportChargingProfilesRequest>?                       CustomReportChargingProfilesRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingProfile>?                                     CustomChargingProfileSerializer                 = null,
                              CustomJObjectSerializerDelegate<LimitBeyondSoC>?                                      CustomLimitBeyondSoCSerializer                  = null,
                              CustomJObjectSerializerDelegate<ChargingSchedule>?                                    CustomChargingScheduleSerializer                = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                              CustomChargingSchedulePeriodSerializer          = null,
                              CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                                    CustomV2XFreqWattEntrySerializer                = null,
                              CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                                  CustomV2XSignalWattEntrySerializer              = null,
                              CustomJObjectSerializerDelegate<SalesTariff>?                                         CustomSalesTariffSerializer                     = null,
                              CustomJObjectSerializerDelegate<SalesTariffEntry>?                                    CustomSalesTariffEntrySerializer                = null,
                              CustomJObjectSerializerDelegate<RelativeTimeInterval>?                                CustomRelativeTimeIntervalSerializer            = null,
                              CustomJObjectSerializerDelegate<ConsumptionCost>?                                     CustomConsumptionCostSerializer                 = null,
                              CustomJObjectSerializerDelegate<Cost>?                                                CustomCostSerializer                            = null,

                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AbsolutePriceSchedule>?    CustomAbsolutePriceScheduleSerializer           = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRuleStack>?           CustomPriceRuleStackSerializer                  = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRule>?                CustomPriceRuleSerializer                       = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.TaxRule>?                  CustomTaxRuleSerializer                         = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRuleList>?         CustomOverstayRuleListSerializer                = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRule>?             CustomOverstayRuleSerializer                    = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AdditionalService>?        CustomAdditionalServiceSerializer               = null,

                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelSchedule>?       CustomPriceLevelScheduleSerializer              = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntrySerializer         = null,

                              CustomJObjectSerializerDelegate<OCPP.Signature>?                                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestId",             ReportChargingProfilesRequestId),
                                 new JProperty("chargingLimitSource",   ChargingLimitSource.ToString()),
                                 new JProperty("evseId",                EVSEId.             Value),

                                 new JProperty("chargingProfile",       new JArray(ChargingProfiles.Select(chargingProfile => chargingProfile.ToJSON(CustomChargingProfileSerializer,
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
                                     
                                                                                                                                                     CustomCustomDataSerializer)))),

                           ToBeContinued.HasValue
                               ? new JProperty("tbc",                   ToBeContinued.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",            new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                   CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",            CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomReportChargingProfilesRequestSerializer is not null
                       ? CustomReportChargingProfilesRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ReportChargingProfilesRequest1, ReportChargingProfilesRequest2)

        /// <summary>
        /// Compares two report charging profiles requests for equality.
        /// </summary>
        /// <param name="ReportChargingProfilesRequest1">A report charging profiles request.</param>
        /// <param name="ReportChargingProfilesRequest2">Another report charging profiles request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ReportChargingProfilesRequest? ReportChargingProfilesRequest1,
                                           ReportChargingProfilesRequest? ReportChargingProfilesRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ReportChargingProfilesRequest1, ReportChargingProfilesRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ReportChargingProfilesRequest1 is null || ReportChargingProfilesRequest2 is null)
                return false;

            return ReportChargingProfilesRequest1.Equals(ReportChargingProfilesRequest2);

        }

        #endregion

        #region Operator != (ReportChargingProfilesRequest1, ReportChargingProfilesRequest2)

        /// <summary>
        /// Compares two report charging profiles requests for inequality.
        /// </summary>
        /// <param name="ReportChargingProfilesRequest1">A report charging profiles request.</param>
        /// <param name="ReportChargingProfilesRequest2">Another report charging profiles request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ReportChargingProfilesRequest? ReportChargingProfilesRequest1,
                                           ReportChargingProfilesRequest? ReportChargingProfilesRequest2)

            => !(ReportChargingProfilesRequest1 == ReportChargingProfilesRequest2);

        #endregion

        #endregion

        #region IEquatable<ReportChargingProfilesRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two report charging profiles requests for equality.
        /// </summary>
        /// <param name="Object">A report charging profiles request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ReportChargingProfilesRequest reportChargingProfilesRequest &&
                   Equals(reportChargingProfilesRequest);

        #endregion

        #region Equals(ReportChargingProfilesRequest)

        /// <summary>
        /// Compares two report charging profiles requests for equality.
        /// </summary>
        /// <param name="ReportChargingProfilesRequest">A report charging profiles request to compare with.</param>
        public override Boolean Equals(ReportChargingProfilesRequest? ReportChargingProfilesRequest)

            => ReportChargingProfilesRequest is not null &&

               ReportChargingProfilesRequestId.Equals(ReportChargingProfilesRequest.ReportChargingProfilesRequestId) &&
               ChargingLimitSource.            Equals(ReportChargingProfilesRequest.ChargingLimitSource)             &&
               EVSEId.                         Equals(ReportChargingProfilesRequest.EVSEId)                          &&

               ChargingProfiles.Count().Equals(ReportChargingProfilesRequest.ChargingProfiles.Count())     &&
               ChargingProfiles.All(data => ReportChargingProfilesRequest.ChargingProfiles.Contains(data)) &&

            ((!ToBeContinued.HasValue && !ReportChargingProfilesRequest.ToBeContinued.HasValue) ||
               ToBeContinued.HasValue &&  ReportChargingProfilesRequest.ToBeContinued.HasValue && ToBeContinued.Value.Equals(ReportChargingProfilesRequest.ToBeContinued.Value)) &&

               base.GenericEquals(ReportChargingProfilesRequest);

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

            => $"{ReportChargingProfilesRequestId}, {ChargingLimitSource}, {EVSEId}, {ChargingProfiles.Count()} charging profile(s)";

        #endregion

    }

}
