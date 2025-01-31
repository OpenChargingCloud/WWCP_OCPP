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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The ChangeTransactionTariff request.
    /// </summary>
    public class ChangeTransactionTariffRequest : ARequest<ChangeTransactionTariffRequest>,
                                                  IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/changeTransactionTariff");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The transaction identification of the transaction to change the tariff for.
        /// </summary>
        [Mandatory]
        public Transaction_Id  TransactionId    { get; }

        /// <summary>
        /// The tariff the be applied to the transaction.
        /// </summary>
        [Mandatory]
        public Tariff          Tariff           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ChangeTransactionTariff request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="TransactionId">The transaction identification of the transaction to change the tariff for.</param>
        /// <param name="Tariff">The tariff the be applied to the transaction.</param>
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
        public ChangeTransactionTariffRequest(SourceRouting            Destination,
                                              Transaction_Id           TransactionId,
                                              Tariff                   Tariff,

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
                   nameof(ChangeTransactionTariffRequest)[..^7],

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

            this.TransactionId  = TransactionId;
            this.Tariff         = Tariff;

            unchecked
            {
                hashCode = this.TransactionId.GetHashCode() * 5 ^
                           this.Tariff.       GetHashCode() * 3 ^
                           base.              GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:ChangeTransactionTariffRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "DayOfWeekEnumType": {
        //             "javaType": "DayOfWeekEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Monday",
        //                 "Tuesday",
        //                 "Wednesday",
        //                 "Thursday",
        //                 "Friday",
        //                 "Saturday",
        //                 "Sunday"
        //             ]
        //         },
        //         "EvseKindEnumType": {
        //             "description": "Type of EVSE (AC, DC) this tariff applies to.",
        //             "javaType": "EvseKindEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "AC",
        //                 "DC"
        //             ]
        //         },
        //         "MessageFormatEnumType": {
        //             "description": "Format of the message.",
        //             "javaType": "MessageFormatEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "ASCII",
        //                 "HTML",
        //                 "URI",
        //                 "UTF8",
        //                 "QRCODE"
        //             ]
        //         },
        //         "MessageContentType": {
        //             "description": "Contains message details, for a message to be displayed on a Charging Station.",
        //             "javaType": "MessageContent",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "format": {
        //                     "$ref": "#/definitions/MessageFormatEnumType"
        //                 },
        //                 "language": {
        //                     "description": "Message language identifier. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.",
        //                     "type": "string",
        //                     "maxLength": 8
        //                 },
        //                 "content": {
        //                     "description": "*(2.1)* Required. Message contents. +\r\nMaximum length supported by Charging Station is given in OCPPCommCtrlr.FieldLength[\"MessageContentType.content\"].\r\n    Maximum length defaults to 1024.",
        //                     "type": "string",
        //                     "maxLength": 1024
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "format",
        //                 "content"
        //             ]
        //         },
        //         "PriceType": {
        //             "description": "Price with and without tax. At least one of _exclTax_, _inclTax_ must be present.",
        //             "javaType": "Price",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "exclTax": {
        //                     "description": "Price/cost excluding tax. Can be absent if _inclTax_ is present.",
        //                     "type": "number"
        //                 },
        //                 "inclTax": {
        //                     "description": "Price/cost including tax. Can be absent if _exclTax_ is present.",
        //                     "type": "number"
        //                 },
        //                 "taxRates": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/TaxRateType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 5
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
        //         },
        //         "TariffConditionsFixedType": {
        //             "description": "These conditions describe if a FixedPrice applies at start of the transaction.\r\n\r\nWhen more than one restriction is set, they are to be treated as a logical AND. All need to be valid before this price is active.\r\n\r\nNOTE: _startTimeOfDay_ and _endTimeOfDay_ are in local time, because it is the time in the tariff as it is shown to the EV driver at the Charging Station.\r\nA Charging Station will convert this to the internal time zone that it uses (which is recommended to be UTC, see section Generic chapter 3.1) when performing cost calculation.",
        //             "javaType": "TariffConditionsFixed",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "startTimeOfDay": {
        //                     "description": "Start time of day in local time. +\r\nFormat as per RFC 3339: time-hour \":\" time-minute  +\r\nMust be in 24h format with leading zeros. Hour/Minute separator: \":\"\r\nRegex: ([0-1][0-9]\\|2[0-3]):[0-5][0-9]",
        //                     "type": "string"
        //                 },
        //                 "endTimeOfDay": {
        //                     "description": "End time of day in local time. Same syntax as _startTimeOfDay_. +\r\n    If end time &lt; start time then the period wraps around to the next day. +\r\n    To stop at end of the day use: 00:00.",
        //                     "type": "string"
        //                 },
        //                 "dayOfWeek": {
        //                     "description": "Day(s) of the week this is tariff applies.",
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/DayOfWeekEnumType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 7
        //                 },
        //                 "validFromDate": {
        //                     "description": "Start date in local time, for example: 2015-12-24.\r\nValid from this day (inclusive). +\r\nFormat as per RFC 3339: full-date  + \r\n\r\nRegex: ([12][0-9]{3})-(0[1-9]\\|1[0-2])-(0[1-9]\\|[12][0-9]\\|3[01])",
        //                     "type": "string"
        //                 },
        //                 "validToDate": {
        //                     "description": "End date in local time, for example: 2015-12-27.\r\n    Valid until this day (exclusive). Same syntax as _validFromDate_.",
        //                     "type": "string"
        //                 },
        //                 "evseKind": {
        //                     "$ref": "#/definitions/EvseKindEnumType"
        //                 },
        //                 "paymentBrand": {
        //                     "description": "For which payment brand this (adhoc) tariff applies. Can be used to add a surcharge for certain payment brands.\r\n    Based on value of _additionalIdToken_ from _idToken.additionalInfo.type_ = \"PaymentBrand\".",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "paymentRecognition": {
        //                     "description": "Type of adhoc payment, e.g. CC, Debit.\r\n    Based on value of _additionalIdToken_ from _idToken.additionalInfo.type_ = \"PaymentRecognition\".",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
        //         },
        //         "TariffConditionsType": {
        //             "description": "These conditions describe if and when a TariffEnergyType or TariffTimeType applies during a transaction.\r\n\r\nWhen more than one restriction is set, they are to be treated as a logical AND. All need to be valid before this price is active.\r\n\r\nFor reverse energy flow (discharging) negative values of energy, power and current are used.\r\n\r\nNOTE: _minXXX_ (where XXX = Kwh/A/Kw) must be read as \"closest to zero\", and _maxXXX_ as \"furthest from zero\". For example, a *charging* power range from 10 kW to 50 kWh is given by _minPower_ = 10000 and _maxPower_ = 50000, and a *discharging* power range from -10 kW to -50 kW is given by _minPower_ = -10 and _maxPower_ = -50.\r\n\r\nNOTE: _startTimeOfDay_ and _endTimeOfDay_ are in local time, because it is the time in the tariff as it is shown to the EV driver at the Charging Station.\r\nA Charging Station will convert this to the internal time zone that it uses (which is recommended to be UTC, see section Generic chapter 3.1) when performing cost calculation.",
        //             "javaType": "TariffConditions",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "startTimeOfDay": {
        //                     "description": "Start time of day in local time. +\r\nFormat as per RFC 3339: time-hour \":\" time-minute  +\r\nMust be in 24h format with leading zeros. Hour/Minute separator: \":\"\r\nRegex: ([0-1][0-9]\\|2[0-3]):[0-5][0-9]",
        //                     "type": "string"
        //                 },
        //                 "endTimeOfDay": {
        //                     "description": "End time of day in local time. Same syntax as _startTimeOfDay_. +\r\n    If end time &lt; start time then the period wraps around to the next day. +\r\n    To stop at end of the day use: 00:00.",
        //                     "type": "string"
        //                 },
        //                 "dayOfWeek": {
        //                     "description": "Day(s) of the week this is tariff applies.",
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/DayOfWeekEnumType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 7
        //                 },
        //                 "validFromDate": {
        //                     "description": "Start date in local time, for example: 2015-12-24.\r\nValid from this day (inclusive). +\r\nFormat as per RFC 3339: full-date  + \r\n\r\nRegex: ([12][0-9]{3})-(0[1-9]\\|1[0-2])-(0[1-9]\\|[12][0-9]\\|3[01])",
        //                     "type": "string"
        //                 },
        //                 "validToDate": {
        //                     "description": "End date in local time, for example: 2015-12-27.\r\n    Valid until this day (exclusive). Same syntax as _validFromDate_.",
        //                     "type": "string"
        //                 },
        //                 "evseKind": {
        //                     "$ref": "#/definitions/EvseKindEnumType"
        //                 },
        //                 "minEnergy": {
        //                     "description": "Minimum consumed energy in Wh, for example 20000 Wh.\r\n    Valid from this amount of energy (inclusive) being used.",
        //                     "type": "number"
        //                 },
        //                 "maxEnergy": {
        //                     "description": "Maximum consumed energy in Wh, for example 50000 Wh.\r\n    Valid until this amount of energy (exclusive) being used.",
        //                     "type": "number"
        //                 },
        //                 "minCurrent": {
        //                     "description": "Sum of the minimum current (in Amperes) over all phases, for example 5 A.\r\n    When the EV is charging with more than, or equal to, the defined amount of current, this price is/becomes active. If the charging current is or becomes lower, this price is not or no longer valid and becomes inactive. +\r\n    This is NOT about the minimum current over the entire transaction.",
        //                     "type": "number"
        //                 },
        //                 "maxCurrent": {
        //                     "description": "Sum of the maximum current (in Amperes) over all phases, for example 20 A.\r\n      When the EV is charging with less than the defined amount of current, this price becomes/is active. If the charging current is or becomes higher, this price is not or no longer valid and becomes inactive.\r\n      This is NOT about the maximum current over the entire transaction.",
        //                     "type": "number"
        //                 },
        //                 "minPower": {
        //                     "description": "Minimum power in W, for example 5000 W.\r\n      When the EV is charging with more than, or equal to, the defined amount of power, this price is/becomes active.\r\n      If the charging power is or becomes lower, this price is not or no longer valid and becomes inactive.\r\n      This is NOT about the minimum power over the entire transaction.",
        //                     "type": "number"
        //                 },
        //                 "maxPower": {
        //                     "description": "Maximum power in W, for example 20000 W.\r\n      When the EV is charging with less than the defined amount of power, this price becomes/is active.\r\n      If the charging power is or becomes higher, this price is not or no longer valid and becomes inactive.\r\n      This is NOT about the maximum power over the entire transaction.",
        //                     "type": "number"
        //                 },
        //                 "minTime": {
        //                     "description": "Minimum duration in seconds the transaction (charging &amp; idle) MUST last (inclusive).\r\n      When the duration of a transaction is longer than the defined value, this price is or becomes active.\r\n      Before that moment, this price is not yet active.",
        //                     "type": "integer"
        //                 },
        //                 "maxTime": {
        //                     "description": "Maximum duration in seconds the transaction (charging &amp; idle) MUST last (exclusive).\r\n      When the duration of a transaction is shorter than the defined value, this price is or becomes active.\r\n      After that moment, this price is no longer active.",
        //                     "type": "integer"
        //                 },
        //                 "minChargingTime": {
        //                     "description": "Minimum duration in seconds the charging MUST last (inclusive).\r\n      When the duration of a charging is longer than the defined value, this price is or becomes active.\r\n      Before that moment, this price is not yet active.",
        //                     "type": "integer"
        //                 },
        //                 "maxChargingTime": {
        //                     "description": "Maximum duration in seconds the charging MUST last (exclusive).\r\n      When the duration of a charging is shorter than the defined value, this price is or becomes active.\r\n      After that moment, this price is no longer active.",
        //                     "type": "integer"
        //                 },
        //                 "minIdleTime": {
        //                     "description": "Minimum duration in seconds the idle period (i.e. not charging) MUST last (inclusive).\r\n      When the duration of the idle time is longer than the defined value, this price is or becomes active.\r\n      Before that moment, this price is not yet active.",
        //                     "type": "integer"
        //                 },
        //                 "maxIdleTime": {
        //                     "description": "Maximum duration in seconds the idle period (i.e. not charging) MUST last (exclusive).\r\n      When the duration of idle time is shorter than the defined value, this price is or becomes active.\r\n      After that moment, this price is no longer active.",
        //                     "type": "integer"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
        //         },
        //         "TariffEnergyPriceType": {
        //             "description": "Tariff with optional conditions for an energy price.",
        //             "javaType": "TariffEnergyPrice",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "priceKwh": {
        //                     "description": "Price per kWh (excl. tax) for this element.",
        //                     "type": "number"
        //                 },
        //                 "conditions": {
        //                     "$ref": "#/definitions/TariffConditionsType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "priceKwh"
        //             ]
        //         },
        //         "TariffEnergyType": {
        //             "description": "Price elements and tax for energy",
        //             "javaType": "TariffEnergy",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "prices": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/TariffEnergyPriceType"
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "taxRates": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/TaxRateType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 5
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "prices"
        //             ]
        //         },
        //         "TariffFixedPriceType": {
        //             "description": "Tariff with optional conditions for a fixed price.",
        //             "javaType": "TariffFixedPrice",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "conditions": {
        //                     "$ref": "#/definitions/TariffConditionsFixedType"
        //                 },
        //                 "priceFixed": {
        //                     "description": "Fixed price  for this element e.g. a start fee.",
        //                     "type": "number"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "priceFixed"
        //             ]
        //         },
        //         "TariffFixedType": {
        //             "javaType": "TariffFixed",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "prices": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/TariffFixedPriceType"
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "taxRates": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/TaxRateType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 5
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "prices"
        //             ]
        //         },
        //         "TariffTimePriceType": {
        //             "description": "Tariff with optional conditions for a time duration price.",
        //             "javaType": "TariffTimePrice",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "priceMinute": {
        //                     "description": "Price per minute (excl. tax) for this element.",
        //                     "type": "number"
        //                 },
        //                 "conditions": {
        //                     "$ref": "#/definitions/TariffConditionsType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "priceMinute"
        //             ]
        //         },
        //         "TariffTimeType": {
        //             "description": "Price elements and tax for time",
        //             "javaType": "TariffTime",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "prices": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/TariffTimePriceType"
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "taxRates": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/TaxRateType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 5
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "prices"
        //             ]
        //         },
        //         "TariffType": {
        //             "description": "A tariff is described by fields with prices for:\r\nenergy,\r\ncharging time,\r\nidle time,\r\nfixed fee,\r\nreservation time,\r\nreservation fixed fee. +\r\nEach of these fields may have (optional) conditions that specify when a price is applicable. +\r\nThe _description_ contains a human-readable explanation of the tariff to be shown to the user. +\r\nThe other fields are parameters that define the tariff. These are used by the charging station to calculate the price.",
        //             "javaType": "Tariff",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "tariffId": {
        //                     "description": "Unique id of tariff",
        //                     "type": "string",
        //                     "maxLength": 60
        //                 },
        //                 "description": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/MessageContentType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 10
        //                 },
        //                 "currency": {
        //                     "description": "Currency code according to ISO 4217",
        //                     "type": "string",
        //                     "maxLength": 3
        //                 },
        //                 "energy": {
        //                     "$ref": "#/definitions/TariffEnergyType"
        //                 },
        //                 "validFrom": {
        //                     "description": "Time when this tariff becomes active. When absent, it is immediately active.",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "chargingTime": {
        //                     "$ref": "#/definitions/TariffTimeType"
        //                 },
        //                 "idleTime": {
        //                     "$ref": "#/definitions/TariffTimeType"
        //                 },
        //                 "fixedFee": {
        //                     "$ref": "#/definitions/TariffFixedType"
        //                 },
        //                 "reservationTime": {
        //                     "$ref": "#/definitions/TariffTimeType"
        //                 },
        //                 "reservationFixed": {
        //                     "$ref": "#/definitions/TariffFixedType"
        //                 },
        //                 "minCost": {
        //                     "$ref": "#/definitions/PriceType"
        //                 },
        //                 "maxCost": {
        //                     "$ref": "#/definitions/PriceType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "tariffId",
        //                 "currency"
        //             ]
        //         },
        //         "TaxRateType": {
        //             "description": "Tax percentage",
        //             "javaType": "TaxRate",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "type": {
        //                     "description": "Type of this tax, e.g.  \"Federal \",  \"State\", for information on receipt.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "tax": {
        //                     "description": "Tax percentage",
        //                     "type": "number"
        //                 },
        //                 "stack": {
        //                     "description": "Stack level for this type of tax. Default value, when absent, is 0. +\r\n_stack_ = 0: tax on net price; +\r\n_stack_ = 1: tax added on top of _stack_ 0; +\r\n_stack_ = 2: tax added on top of _stack_ 1, etc. ",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "type",
        //                 "tax"
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
        //         "tariff": {
        //             "$ref": "#/definitions/TariffType"
        //         },
        //         "transactionId": {
        //             "description": "Transaction id for new tariff.",
        //             "type": "string",
        //             "maxLength": 36
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "transactionId",
        //         "tariff"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a ChangeTransactionTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomChangeTransactionTariffRequestParser">A delegate to parse custom setTariffs requests.</param>
        public static ChangeTransactionTariffRequest Parse(JObject                                                       JSON,
                                                           Request_Id                                                    RequestId,
                                                           SourceRouting                                                 Destination,
                                                           NetworkPath                                                   NetworkPath,
                                                           DateTime?                                                     RequestTimestamp                             = null,
                                                           TimeSpan?                                                     RequestTimeout                               = null,
                                                           EventTracking_Id?                                             EventTrackingId                              = null,
                                                           CustomJObjectParserDelegate<ChangeTransactionTariffRequest>?  CustomChangeTransactionTariffRequestParser   = null,
                                                           CustomJObjectParserDelegate<Tariff>?                          CustomTariffParser                           = null,
                                                           CustomJObjectParserDelegate<MessageContent>?                  CustomMessageContentParser                   = null,
                                                           CustomJObjectParserDelegate<Price>?                           CustomPriceParser                            = null,
                                                           CustomJObjectParserDelegate<TaxRate>?                         CustomTaxRateParser                          = null,
                                                           CustomJObjectParserDelegate<TariffConditions>?                CustomTariffConditionsParser                 = null,
                                                           CustomJObjectParserDelegate<TariffEnergy>?                    CustomTariffEnergyParser                     = null,
                                                           CustomJObjectParserDelegate<TariffEnergyPrice>?               CustomTariffEnergyPriceParser                = null,
                                                           CustomJObjectParserDelegate<TariffTime>?                      CustomTariffTimeParser                       = null,
                                                           CustomJObjectParserDelegate<TariffTimePrice>?                 CustomTariffTimePriceParser                  = null,
                                                           CustomJObjectParserDelegate<TariffFixed>?                     CustomTariffFixedParser                      = null,
                                                           CustomJObjectParserDelegate<TariffFixedPrice>?                CustomTariffFixedPriceParser                 = null,
                                                           CustomJObjectParserDelegate<Signature>?                       CustomSignatureParser                        = null,
                                                           CustomJObjectParserDelegate<CustomData>?                      CustomCustomDataParser                       = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var changeTransactionTariffRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomChangeTransactionTariffRequestParser,
                         CustomTariffParser,
                         CustomMessageContentParser,
                         CustomPriceParser,
                         CustomTaxRateParser,
                         CustomTariffConditionsParser,
                         CustomTariffEnergyParser,
                         CustomTariffEnergyPriceParser,
                         CustomTariffTimeParser,
                         CustomTariffTimePriceParser,
                         CustomTariffFixedParser,
                         CustomTariffFixedPriceParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return changeTransactionTariffRequest;
            }

            throw new ArgumentException("The given JSON representation of a ChangeTransactionTariff request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out ChangeTransactionTariffRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a ChangeTransactionTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ChangeTransactionTariffRequest">The parsed setTariffs request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomChangeTransactionTariffRequestParser">A delegate to parse custom setTariffs requests.</param>
        public static Boolean TryParse(JObject                                                       JSON,
                                       Request_Id                                                    RequestId,
                                       SourceRouting                                                 Destination,
                                       NetworkPath                                                   NetworkPath,
                                       [NotNullWhen(true)]  out ChangeTransactionTariffRequest?      ChangeTransactionTariffRequest,
                                       [NotNullWhen(false)] out String?                              ErrorResponse,
                                       DateTime?                                                     RequestTimestamp                             = null,
                                       TimeSpan?                                                     RequestTimeout                               = null,
                                       EventTracking_Id?                                             EventTrackingId                              = null,
                                       CustomJObjectParserDelegate<ChangeTransactionTariffRequest>?  CustomChangeTransactionTariffRequestParser   = null,
                                       CustomJObjectParserDelegate<Tariff>?                          CustomTariffParser                           = null,
                                       CustomJObjectParserDelegate<MessageContent>?                  CustomMessageContentParser                   = null,
                                       CustomJObjectParserDelegate<Price>?                           CustomPriceParser                            = null,
                                       CustomJObjectParserDelegate<TaxRate>?                         CustomTaxRateParser                          = null,
                                       CustomJObjectParserDelegate<TariffConditions>?                CustomTariffConditionsParser                 = null,
                                       CustomJObjectParserDelegate<TariffEnergy>?                    CustomTariffEnergyParser                     = null,
                                       CustomJObjectParserDelegate<TariffEnergyPrice>?               CustomTariffEnergyPriceParser                = null,
                                       CustomJObjectParserDelegate<TariffTime>?                      CustomTariffTimeParser                       = null,
                                       CustomJObjectParserDelegate<TariffTimePrice>?                 CustomTariffTimePriceParser                  = null,
                                       CustomJObjectParserDelegate<TariffFixed>?                     CustomTariffFixedParser                      = null,
                                       CustomJObjectParserDelegate<TariffFixedPrice>?                CustomTariffFixedPriceParser                 = null,
                                       CustomJObjectParserDelegate<Signature>?                       CustomSignatureParser                        = null,
                                       CustomJObjectParserDelegate<CustomData>?                      CustomCustomDataParser                       = null)
        {

            try
            {

                ChangeTransactionTariffRequest = null;

                #region TransactionId    [mandatory]

                if (!JSON.ParseMandatory("transactionId",
                                         "transaction identification",
                                         Transaction_Id.TryParse,
                                         out Transaction_Id TransactionId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Tariff           [mandatory]

                if (!JSON.ParseMandatoryJSON("tariff",
                                             "tariff",
                                             OCPPv2_1.Tariff.TryParse,
                                             out Tariff? Tariff,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures       [optional, OCPP_CSE]

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

                #region CustomData       [optional]

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


                ChangeTransactionTariffRequest = new ChangeTransactionTariffRequest(

                                                     Destination,
                                                     TransactionId,
                                                     Tariff,

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

                if (CustomChangeTransactionTariffRequestParser is not null)
                    ChangeTransactionTariffRequest = CustomChangeTransactionTariffRequestParser(JSON,
                                                                                                ChangeTransactionTariffRequest);

                return true;

            }
            catch (Exception e)
            {
                ChangeTransactionTariffRequest  = null;
                ErrorResponse                   = "The given JSON representation of a ChangeTransactionTariff request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChangeTransactionTariffRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChangeTransactionTariffRequestSerializer">A delegate to serialize custom setTariffs requests.</param>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom MessageContent objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom TaxRate JSON objects.</param>
        /// <param name="CustomTariffConditionsSerializer">A delegate to serialize custom TariffConditions JSON objects.</param>
        /// <param name="CustomTariffEnergySerializer">A delegate to serialize custom TariffEnergy JSON objects.</param>
        /// <param name="CustomTariffEnergyPriceSerializer">A delegate to serialize custom TariffEnergyPrice JSON objects.</param>
        /// <param name="CustomTariffTimeSerializer">A delegate to serialize custom TariffTime JSON objects.</param>
        /// <param name="CustomTariffTimePriceSerializer">A delegate to serialize custom TariffTimePrice JSON objects.</param>
        /// <param name="CustomTariffFixedSerializer">A delegate to serialize custom TariffFixed JSON objects.</param>
        /// <param name="CustomTariffFixedPriceSerializer">A delegate to serialize custom TariffFixedPrice JSON objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                           IncludeJSONLDContext                             = false,
                              CustomJObjectSerializerDelegate<ChangeTransactionTariffRequest>?  CustomChangeTransactionTariffRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Tariff>?                          CustomTariffSerializer                           = null,
                              CustomJObjectSerializerDelegate<MessageContent>?                  CustomMessageContentSerializer                   = null,
                              CustomJObjectSerializerDelegate<Price>?                           CustomPriceSerializer                            = null,
                              CustomJObjectSerializerDelegate<TaxRate>?                         CustomTaxRateSerializer                          = null,
                              CustomJObjectSerializerDelegate<TariffConditions>?                CustomTariffConditionsSerializer                 = null,
                              CustomJObjectSerializerDelegate<TariffEnergy>?                    CustomTariffEnergySerializer                     = null,
                              CustomJObjectSerializerDelegate<TariffEnergyPrice>?               CustomTariffEnergyPriceSerializer                = null,
                              CustomJObjectSerializerDelegate<TariffTime>?                      CustomTariffTimeSerializer                       = null,
                              CustomJObjectSerializerDelegate<TariffTimePrice>?                 CustomTariffTimePriceSerializer                  = null,
                              CustomJObjectSerializerDelegate<TariffFixed>?                     CustomTariffFixedSerializer                      = null,
                              CustomJObjectSerializerDelegate<TariffFixedPrice>?                CustomTariffFixedPriceSerializer                 = null,
                              CustomJObjectSerializerDelegate<Signature>?                       CustomSignatureSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",        DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("transactionId",   TransactionId.       ToString()),
                                 new JProperty("tariff",          Tariff.              ToJSON(CustomTariffSerializer,
                                                                                              CustomMessageContentSerializer,
                                                                                              CustomPriceSerializer,
                                                                                              CustomTaxRateSerializer,
                                                                                              CustomTariffConditionsSerializer,
                                                                                              CustomTariffEnergySerializer,
                                                                                              CustomTariffEnergyPriceSerializer,
                                                                                              CustomTariffTimeSerializer,
                                                                                              CustomTariffTimePriceSerializer,
                                                                                              CustomTariffFixedSerializer,
                                                                                              CustomTariffFixedPriceSerializer,
                                                                                              CustomSignatureSerializer,
                                                                                              CustomCustomDataSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomChangeTransactionTariffRequestSerializer is not null
                       ? CustomChangeTransactionTariffRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChangeTransactionTariffRequest1, ChangeTransactionTariffRequest2)

        /// <summary>
        /// Compares two ChangeTransactionTariff requests for equality.
        /// </summary>
        /// <param name="ChangeTransactionTariffRequest1">A ChangeTransactionTariff request.</param>
        /// <param name="ChangeTransactionTariffRequest2">Another setTariff request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ChangeTransactionTariffRequest? ChangeTransactionTariffRequest1,
                                           ChangeTransactionTariffRequest? ChangeTransactionTariffRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChangeTransactionTariffRequest1, ChangeTransactionTariffRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ChangeTransactionTariffRequest1 is null || ChangeTransactionTariffRequest2 is null)
                return false;

            return ChangeTransactionTariffRequest1.Equals(ChangeTransactionTariffRequest2);

        }

        #endregion

        #region Operator != (ChangeTransactionTariffRequest1, ChangeTransactionTariffRequest2)

        /// <summary>
        /// Compares two ChangeTransactionTariff requests for inequality.
        /// </summary>
        /// <param name="ChangeTransactionTariffRequest1">A ChangeTransactionTariff request.</param>
        /// <param name="ChangeTransactionTariffRequest2">Another setTariff request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ChangeTransactionTariffRequest? ChangeTransactionTariffRequest1,
                                           ChangeTransactionTariffRequest? ChangeTransactionTariffRequest2)

            => !(ChangeTransactionTariffRequest1 == ChangeTransactionTariffRequest2);

        #endregion

        #endregion

        #region IEquatable<ChangeTransactionTariffRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ChangeTransactionTariffRequest requests for equality.
        /// </summary>
        /// <param name="Object">A ChangeTransactionTariffRequest request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChangeTransactionTariffRequest changeTransactionTariffRequest &&
                   Equals(changeTransactionTariffRequest);

        #endregion

        #region Equals(ChangeTransactionTariffRequest)

        /// <summary>
        /// Compares two ChangeTransactionTariffRequest requests for equality.
        /// </summary>
        /// <param name="ChangeTransactionTariffRequest">A ChangeTransactionTariffRequest request to compare with.</param>
        public override Boolean Equals(ChangeTransactionTariffRequest? ChangeTransactionTariffRequest)

            => ChangeTransactionTariffRequest is not null &&

               TransactionId.Equals(ChangeTransactionTariffRequest.TransactionId) &&
               Tariff.       Equals(ChangeTransactionTariffRequest.Tariff)        &&

               base.GenericEquals(ChangeTransactionTariffRequest);

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

            => $"Change tariff '{Tariff.Id}' for transaction '{TransactionId}'";

        #endregion

    }

}
