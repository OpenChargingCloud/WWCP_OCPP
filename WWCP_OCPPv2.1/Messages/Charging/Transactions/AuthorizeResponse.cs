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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The Authorize response.
    /// </summary>
    public class AuthorizeResponse : AResponse<AuthorizeRequest,
                                               AuthorizeResponse>,
                                     IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/authorizeResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identification token info.
        /// </summary>
        [Mandatory]
        public IdTokenInfo                      IdTokenInfo               { get; }

        /// <summary>
        /// The optional certificate status information.
        /// When all certificates are valid: return 'Accepted', but when one of the certificates was revoked, return 'CertificateRevoked'.
        /// </summary>
        [Optional]
        public AuthorizeCertificateStatus?      CertificateStatus         { get; }

        /// <summary>
        /// The optional enumeration of allowed energy transfer modes the EV can choose from. If omitted this defaults to charging only.
        /// </summary>
        [Optional]
        public IEnumerable<EnergyTransferMode>  AllowedEnergyTransfers    { get; }

        /// <summary>
        /// The optional charging tariff.
        /// </summary>
        [Optional]
        public Tariff?                          Tariff                    { get; }

        /// <summary>
        /// The optional maximum cost/energy/time limit allowed for this charging session.
        /// </summary>
        [Optional]
        public TransactionLimits?               TransactionLimits         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an Authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="IdTokenInfo">The identification token info.</param>
        /// <param name="CertificateStatus">An optional certificate status information.</param>
        /// <param name="Tariff">An optional charging tariff.</param>
        /// <param name="AllowedEnergyTransfers">An optional enumeration of allowed energy transfer modes the EV can choose from. If omitted this defaults to charging only.</param>
        /// 
        /// <param name="TransactionLimits">Optional maximum cost/energy/time limit allowed for this charging session.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public AuthorizeResponse(AuthorizeRequest                  Request,
                                 IdTokenInfo                       IdTokenInfo,
                                 AuthorizeCertificateStatus?       CertificateStatus        = null,
                                 IEnumerable<EnergyTransferMode>?  AllowedEnergyTransfers   = null,
                                 Tariff?                           Tariff                   = null,
                                 TransactionLimits?                TransactionLimits        = null,

                                 Result?                           Result                   = null,
                                 DateTime?                         ResponseTimestamp        = null,

                                 SourceRouting?                    Destination              = null,
                                 NetworkPath?                      NetworkPath              = null,

                                 IEnumerable<KeyPair>?             SignKeys                 = null,
                                 IEnumerable<SignInfo>?            SignInfos                = null,
                                 IEnumerable<Signature>?           Signatures               = null,

                                 CustomData?                       CustomData               = null,

                                 SerializationFormats?             SerializationFormat      = null,
                                 CancellationToken                 CancellationToken        = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.IdTokenInfo             = IdTokenInfo;
            this.CertificateStatus       = CertificateStatus;
            this.AllowedEnergyTransfers  = AllowedEnergyTransfers?.Distinct() ?? [];
            this.Tariff                  = Tariff;
            this.TransactionLimits       = TransactionLimits;

            unchecked
            {

                hashCode = this.IdTokenInfo.           GetHashCode()        * 13 ^
                          (this.CertificateStatus?.    GetHashCode()  ?? 0) * 11 ^
                           this.AllowedEnergyTransfers.CalcHashCode()       *  7 ^
                          (this.Tariff?.               GetHashCode()  ?? 0) *  5 ^
                          (this.TransactionLimits?.    GetHashCode()  ?? 0) *  3 ^
                           base.                       GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:AuthorizeResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "AuthorizationStatusEnumType": {
        //             "description": "Current status of the ID Token.",
        //             "javaType": "AuthorizationStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Blocked",
        //                 "ConcurrentTx",
        //                 "Expired",
        //                 "Invalid",
        //                 "NoCredit",
        //                 "NotAllowedTypeEVSE",
        //                 "NotAtThisLocation",
        //                 "NotAtThisTime",
        //                 "Unknown"
        //             ]
        //         },
        //         "AuthorizeCertificateStatusEnumType": {
        //             "description": "Certificate status information. \r\n- if all certificates are valid: return 'Accepted'.\r\n- if one of the certificates was revoked, return 'CertificateRevoked'.",
        //             "javaType": "AuthorizeCertificateStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "SignatureError",
        //                 "CertificateExpired",
        //                 "CertificateRevoked",
        //                 "NoCertificateAvailable",
        //                 "CertChainError",
        //                 "ContractCancelled"
        //             ]
        //         },
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
        //         "EnergyTransferModeEnumType": {
        //             "javaType": "EnergyTransferModeEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "AC_single_phase",
        //                 "AC_two_phase",
        //                 "AC_three_phase",
        //                 "DC",
        //                 "AC_BPT",
        //                 "AC_BPT_DER",
        //                 "AC_DER",
        //                 "DC_BPT",
        //                 "DC_ACDP",
        //                 "DC_ACDP_BPT",
        //                 "WPT"
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
        //         "AdditionalInfoType": {
        //             "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.",
        //             "javaType": "AdditionalInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "additionalIdToken": {
        //                     "description": "*(2.1)* This field specifies the additional IdToken.",
        //                     "type": "string",
        //                     "maxLength": 255
        //                 },
        //                 "type": {
        //                     "description": "_additionalInfo_ can be used to send extra information to CSMS in addition to the regular authorization with _IdToken_. _AdditionalInfo_ contains one or more custom _types_, which need to be agreed upon by all parties involved. When the _type_ is not supported, the CSMS/Charging Station MAY ignore the _additionalInfo_.",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "additionalIdToken",
        //                 "type"
        //             ]
        //         },
        //         "IdTokenInfoType": {
        //             "description": "Contains status information about an identifier.\r\nIt is advised to not stop charging for a token that expires during charging, as ExpiryDate is only used for caching purposes. If ExpiryDate is not given, the status has no end date.",
        //             "javaType": "IdTokenInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "status": {
        //                     "$ref": "#/definitions/AuthorizationStatusEnumType"
        //                 },
        //                 "cacheExpiryDateTime": {
        //                     "description": "Date and Time after which the token must be considered invalid.",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "chargingPriority": {
        //                     "description": "Priority from a business point of view. Default priority is 0, The range is from -9 to 9. Higher values indicate a higher priority. The chargingPriority in &lt;&lt;transactioneventresponse,TransactionEventResponse&gt;&gt; overrules this one. ",
        //                     "type": "integer"
        //                 },
        //                 "groupIdToken": {
        //                     "$ref": "#/definitions/IdTokenType"
        //                 },
        //                 "language1": {
        //                     "description": "Preferred user interface language of identifier user. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.",
        //                     "type": "string",
        //                     "maxLength": 8
        //                 },
        //                 "language2": {
        //                     "description": "Second preferred user interface language of identifier user. Don\u2019t use when language1 is omitted, has to be different from language1. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.",
        //                     "type": "string",
        //                     "maxLength": 8
        //                 },
        //                 "evseId": {
        //                     "description": "Only used when the IdToken is only valid for one or more specific EVSEs, not for the entire Charging Station.",
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "type": "integer",
        //                         "minimum": 0.0
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "personalMessage": {
        //                     "$ref": "#/definitions/MessageContentType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "status"
        //             ]
        //         },
        //         "IdTokenType": {
        //             "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.",
        //             "javaType": "IdToken",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "additionalInfo": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/AdditionalInfoType"
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "idToken": {
        //                     "description": "*(2.1)* IdToken is case insensitive. Might hold the hidden id of an RFID tag, but can for example also contain a UUID.",
        //                     "type": "string",
        //                     "maxLength": 255
        //                 },
        //                 "type": {
        //                     "description": "*(2.1)* Enumeration of possible idToken types. Values defined in Appendix as IdTokenEnumStringType.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "idToken",
        //                 "type"
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
        //         "idTokenInfo": {
        //             "$ref": "#/definitions/IdTokenInfoType"
        //         },
        //         "certificateStatus": {
        //             "$ref": "#/definitions/AuthorizeCertificateStatusEnumType"
        //         },
        //         "allowedEnergyTransfer": {
        //             "description": "*(2.1)* List of allowed energy transfer modes the EV can choose from. If omitted this defaults to charging only.",
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/EnergyTransferModeEnumType"
        //             },
        //             "minItems": 1
        //         },
        //         "tariff": {
        //             "$ref": "#/definitions/TariffType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "idTokenInfo"
        //     ]
        // }
        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an Authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomAuthorizeResponseParser">A delegate to parse custom authorize responses.</param>
        public static AuthorizeResponse Parse(AuthorizeRequest                                 Request,
                                              JObject                                          JSON,
                                              SourceRouting                                    Destination,
                                              NetworkPath                                      NetworkPath,
                                              DateTime?                                        ResponseTimestamp               = null,
                                              CustomJObjectParserDelegate<AuthorizeResponse>?  CustomAuthorizeResponseParser   = null,
                                              CustomJObjectParserDelegate<IdTokenInfo>?        CustomIdTokenInfoParser         = null,
                                              CustomJObjectParserDelegate<IdToken>?            CustomIdTokenParser             = null,
                                              CustomJObjectParserDelegate<AdditionalInfo>?     CustomAdditionalInfoParser      = null,
                                              CustomJObjectParserDelegate<MessageContent>?     CustomMessageContentParser      = null,
                                              CustomJObjectParserDelegate<TransactionLimits>?  CustomTransactionLimitsParser   = null,
                                              CustomJObjectParserDelegate<Signature>?          CustomSignatureParser           = null,
                                              CustomJObjectParserDelegate<CustomData>?         CustomCustomDataParser          = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var authorizeResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomAuthorizeResponseParser,
                         CustomIdTokenInfoParser,
                         CustomIdTokenParser,
                         CustomAdditionalInfoParser,
                         CustomMessageContentParser,
                         CustomTransactionLimitsParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return authorizeResponse;
            }

            throw new ArgumentException("The given JSON representation of an Authorize response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out AuthorizeResponse, out ErrorResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an Authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="AuthorizeResponse">The parsed authorize response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomAuthorizeResponseParser">A delegate to parse custom authorize responses.</param>
        public static Boolean TryParse(AuthorizeRequest                                 Request,
                                       JObject                                          JSON,
                                       SourceRouting                                    Destination,
                                       NetworkPath                                      NetworkPath,
                                       [NotNullWhen(true)]  out AuthorizeResponse?      AuthorizeResponse,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       DateTime?                                        ResponseTimestamp               = null,
                                       CustomJObjectParserDelegate<AuthorizeResponse>?  CustomAuthorizeResponseParser   = null,
                                       CustomJObjectParserDelegate<IdTokenInfo>?        CustomIdTokenInfoParser         = null,
                                       CustomJObjectParserDelegate<IdToken>?            CustomIdTokenParser             = null,
                                       CustomJObjectParserDelegate<AdditionalInfo>?     CustomAdditionalInfoParser      = null,
                                       CustomJObjectParserDelegate<MessageContent>?     CustomMessageContentParser      = null,
                                       CustomJObjectParserDelegate<TransactionLimits>?  CustomTransactionLimitsParser   = null,
                                       CustomJObjectParserDelegate<Signature>?          CustomSignatureParser           = null,
                                       CustomJObjectParserDelegate<CustomData>?         CustomCustomDataParser          = null)
        {

            try
            {

                AuthorizeResponse = null;

                #region IdTokenInfo               [mandatory]

                if (!JSON.ParseMandatoryJSON("idTokenInfo",
                                             "identification tag information",
                                             OCPPv2_1.IdTokenInfo.TryParse,
                                             out IdTokenInfo? IdTokenInfo,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (IdTokenInfo is null)
                    return false;

                #endregion

                #region CertificateStatus         [optional]

                if (JSON.ParseOptional("certificateStatus",
                                       "certificate status",
                                       AuthorizeCertificateStatus.TryParse,
                                       out AuthorizeCertificateStatus? CertificateStatus,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region AllowedEnergyTransfers    [optional]

                if (JSON.ParseOptionalHashSet("allowedEnergyTransfer",
                                              "allowed energy transfer",
                                              EnergyTransferMode.TryParse,
                                              out HashSet<EnergyTransferMode> AllowedEnergyTransfers,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Tariff                    [optional]

                if (JSON.ParseOptionalJSON("tariff",
                                           "charging tariff",
                                           OCPPv2_1.Tariff.TryParse,
                                           out Tariff? Tariff,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region TransactionLimits         [optional]

                if (JSON.ParseOptionalJSON("transactionLimit",
                                           "transaction limits",
                                           OCPPv2_1.TransactionLimits.TryParse,
                                           out TransactionLimits? TransactionLimits,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                [optional, OCPP_CSE]

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

                #region CustomData                [optional]

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


                AuthorizeResponse = new AuthorizeResponse(

                                        Request,
                                        IdTokenInfo,
                                        CertificateStatus,
                                        AllowedEnergyTransfers,
                                        Tariff,
                                        TransactionLimits,

                                        null,
                                        ResponseTimestamp,

                                        Destination,
                                        NetworkPath,

                                        null,
                                        null,
                                        Signatures,

                                        CustomData

                                    );

                if (CustomAuthorizeResponseParser is not null)
                    AuthorizeResponse = CustomAuthorizeResponseParser(JSON,
                                                                      AuthorizeResponse);

                return true;

            }
            catch (Exception e)
            {
                AuthorizeResponse  = null;
                ErrorResponse      = "The given JSON representation of an Authorize response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAuthorizeResponseSerializer = null, CustomIdTokenInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizeResponseSerializer">A delegate to serialize custom authorize responses.</param>
        /// <param name="CustomIdTokenInfoSerializer">A delegate to serialize custom identification token infos.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional infos.</param>
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom message contents.</param>
        /// <param name="CustomTransactionLimitsSerializer">A delegate to serialize custom transaction limits.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                              IncludeJSONLDContext                = false,
                              CustomJObjectSerializerDelegate<AuthorizeResponse>?  CustomAuthorizeResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<IdTokenInfo>?        CustomIdTokenInfoSerializer         = null,
                              CustomJObjectSerializerDelegate<IdToken>?            CustomIdTokenSerializer             = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?     CustomAdditionalInfoSerializer      = null,
                              CustomJObjectSerializerDelegate<MessageContent>?     CustomMessageContentSerializer      = null,
                              CustomJObjectSerializerDelegate<TransactionLimits>?  CustomTransactionLimitsSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",                DefaultJSONLDContext.       ToString())
                               : null,

                                 new JProperty("idTokenInfo",             IdTokenInfo.                ToJSON(CustomIdTokenInfoSerializer,
                                                                                                             CustomIdTokenSerializer,
                                                                                                             CustomAdditionalInfoSerializer,
                                                                                                             CustomMessageContentSerializer,
                                                                                                             CustomCustomDataSerializer)),

                           CertificateStatus.HasValue
                               ? new JProperty("certificateStatus",       CertificateStatus.    Value.ToString())
                               : null,

                           AllowedEnergyTransfers.Any()
                               ? new JProperty("allowedEnergyTransfer",   new JArray(AllowedEnergyTransfers.Select(allowedEnergyTransfer => allowedEnergyTransfer.ToString())))
                               : null,

                           TransactionLimits is not null
                               ? new JProperty("transactionLimits",       TransactionLimits.          ToJSON(CustomTransactionLimitsSerializer,
                                                                                                             CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",              new JArray(Signatures.            Select(signature             => signature.            ToJSON(CustomSignatureSerializer,
                                                                                                                                                                         CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",              CustomData.                 ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAuthorizeResponseSerializer is not null
                       ? CustomAuthorizeResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The Authorize failed because of a request error.
        /// </summary>
        /// <param name="Request">The Authorize request.</param>
        public static AuthorizeResponse RequestError(AuthorizeRequest         Request,
                                                     EventTracking_Id         EventTrackingId,
                                                     ResultCode               ErrorCode,
                                                     String?                  ErrorDescription    = null,
                                                     JObject?                 ErrorDetails        = null,
                                                     DateTime?                ResponseTimestamp   = null,

                                                     SourceRouting?           Destination         = null,
                                                     NetworkPath?             NetworkPath         = null,

                                                     IEnumerable<KeyPair>?    SignKeys            = null,
                                                     IEnumerable<SignInfo>?   SignInfos           = null,
                                                     IEnumerable<Signature>?  Signatures          = null,

                                                     CustomData?              CustomData          = null)

            => new (

                   Request,
                   IdTokenInfo.Error,
                   null,
                   null,
                   null,
                   null,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The Authorize failed.
        /// </summary>
        /// <param name="Request">The Authorize request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AuthorizeResponse FormationViolation(AuthorizeRequest  Request,
                                                           String            ErrorDescription)

            => new (Request,
                    IdTokenInfo.ParsingError,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The Authorize failed.
        /// </summary>
        /// <param name="Request">The Authorize request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AuthorizeResponse SignatureError(AuthorizeRequest  Request,
                                                       String            ErrorDescription)

            => new (Request,
                    IdTokenInfo.SignatureError,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The Authorize failed.
        /// </summary>
        /// <param name="Request">The Authorize request.</param>
        /// <param name="Description">An optional error description.</param>
        public static AuthorizeResponse Failed(AuthorizeRequest  Request,
                                               String?           Description   = null)

            => new (Request,
                    IdTokenInfo.Error,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The Authorize failed because of an exception.
        /// </summary>
        /// <param name="Request">The Authorize request.</param>
        /// <param name="Exception">The exception.</param>
        public static AuthorizeResponse ExceptionOccured(AuthorizeRequest  Request,
                                                         Exception         Exception)

            => new (Request,
                    IdTokenInfo.Error,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizeResponse1, AuthorizeResponse2)

        /// <summary>
        /// Compares two Authorize responses for equality.
        /// </summary>
        /// <param name="AuthorizeResponse1">A authorize response.</param>
        /// <param name="AuthorizeResponse2">Another authorize response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AuthorizeResponse? AuthorizeResponse1,
                                           AuthorizeResponse? AuthorizeResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AuthorizeResponse1, AuthorizeResponse2))
                return true;

            // If one is null, but not both, return false.
            if (AuthorizeResponse1 is null || AuthorizeResponse2 is null)
                return false;

            return AuthorizeResponse1.Equals(AuthorizeResponse2);

        }

        #endregion

        #region Operator != (AuthorizeResponse1, AuthorizeResponse2)

        /// <summary>
        /// Compares two Authorize responses for inequality.
        /// </summary>
        /// <param name="AuthorizeResponse1">A authorize response.</param>
        /// <param name="AuthorizeResponse2">Another authorize response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AuthorizeResponse? AuthorizeResponse1,
                                           AuthorizeResponse? AuthorizeResponse2)

            => !(AuthorizeResponse1 == AuthorizeResponse2);

        #endregion

        #endregion

        #region IEquatable<AuthorizeResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Authorize responses for equality.
        /// </summary>
        /// <param name="Object">An Authorize response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthorizeResponse authorizeResponse &&
                   Equals(authorizeResponse);

        #endregion

        #region Equals(AuthorizeResponse)

        /// <summary>
        /// Compares two Authorize responses for equality.
        /// </summary>
        /// <param name="AuthorizeResponse">An Authorize response to compare with.</param>
        public override Boolean Equals(AuthorizeResponse? AuthorizeResponse)

            => AuthorizeResponse is not null &&

               IdTokenInfo.Equals(AuthorizeResponse.IdTokenInfo) &&

            ((!CertificateStatus.HasValue    && !AuthorizeResponse.CertificateStatus.HasValue) ||
               CertificateStatus.HasValue    &&  AuthorizeResponse.CertificateStatus.HasValue    && CertificateStatus.Value.Equals(AuthorizeResponse.CertificateStatus.Value)) &&

             ((Tariff            is null     &&  AuthorizeResponse.Tariff                is null)  ||
               Tariff            is not null &&  AuthorizeResponse.Tariff            is not null && Tariff.                 Equals(AuthorizeResponse.Tariff))                  &&

             ((TransactionLimits is null     &&  AuthorizeResponse.TransactionLimits is null)  ||
               TransactionLimits is not null &&  AuthorizeResponse.TransactionLimits is not null && TransactionLimits.      Equals(AuthorizeResponse.TransactionLimits))       &&

               AllowedEnergyTransfers.ToHashSet().SetEquals(AuthorizeResponse.AllowedEnergyTransfers) &&

               base.GenericEquals(AuthorizeResponse);

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

                   IdTokenInfo.ToString(),

                   AllowedEnergyTransfers.Any()
                       ? $", with {AllowedEnergyTransfers.Count()} allowed energy transfer(s)"
                       : "",

                   Tariff is not null
                       ? $", with tariff {Tariff.Id}"
                       : "",

                   TransactionLimits is not null
                       ? $", with transaction limit(s): {TransactionLimits}"
                       : ""

               );

        #endregion

    }

}
