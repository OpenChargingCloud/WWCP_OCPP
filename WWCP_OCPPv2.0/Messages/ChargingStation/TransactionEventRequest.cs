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

namespace cloud.charging.open.protocols.OCPPv2_0.CS
{

    /// <summary>
    /// A transaction event request.
    /// </summary>
    public class TransactionEventRequest : ARequest<TransactionEventRequest>
    {

        #region Properties

        /// <summary>
        /// The type of this transaction event.
        /// The first event of a transaction SHALL be of type "started", the last of type "ended".
        /// All others should be of type "updated".
        /// </summary>
        [Mandatory]
        public TransactionEvents        EventType             { get; }

        /// <summary>
        /// The timestamp at which this transaction event occurred.
        /// </summary>
        [Mandatory]
        public DateTime                 Timestamp             { get; }

        /// <summary>
        /// The reason the charging station sends this message.
        /// </summary>
        [Mandatory]
        public TriggerReasons           TriggerReason         { get; }

        /// <summary>
        /// This incremental sequence number, helps to determine whether all messages of a transaction have been received.
        /// </summary>
        public UInt32                   SequenceNumber        { get; }

        /// <summary>
        /// Transaction related information.
        /// </summary>
        [Mandatory]
        public Transaction              TransactionInfo       { get; }

        /// <summary>
        /// The optional indication whether this transaction event happened when the charging station was offline.
        /// </summary>
        [Optional]
        public Boolean?                 Offline               { get; }

        /// <summary>
        /// The optional numer of electrical phases used, when the charging station is able to report it.
        /// </summary>
        [Optional]
        public Byte?                    NumberOfPhasesUsed    { get; }

        /// <summary>
        /// The optional maximum current of the connected cable in amperes.
        /// </summary>
        [Optional]
        public Int16?                   CableMaxCurrent       { get; }

        /// <summary>
        /// The optional unqiue reservation identification of the reservation that terminated as a result of this transaction.
        /// </summary>
        [Optional]
        public Reservation_Id?          ReservationId         { get; }

        /// <summary>
        /// The optional identification token for which a transaction has to be/was started.
        /// Is required when the EV driver becomes authorized for this transaction.
        /// The identification token should only be send once in a TransactionEventRequest for every authorization done for this transaction.
        /// </summary>
        [Optional]
        public IdToken?                 IdToken               { get; }

        /// <summary>
        /// The optional indication of the EVSE (and connector) used.
        /// </summary>
        [Optional]
        public EVSE?                    EVSE                  { get; }

        /// <summary>
        /// The optional enumeration of meter values.
        /// </summary>
        [Optional]
        public IEnumerable<MeterValue>  MeterValues           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a transaction event request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// 
        /// <param name="EventType">The type of this transaction event. The first event of a transaction SHALL be of type "started", the last of type "ended". All others should be of type "updated".</param>
        /// <param name="Timestamp">The timestamp at which this transaction event occurred.</param>
        /// <param name="TriggerReason">The reason the charging station sends this message.</param>
        /// <param name="SequenceNumber">This incremental sequence number, helps to determine whether all messages of a transaction have been received.</param>
        /// <param name="TransactionInfo">Transaction related information.</param>
        /// 
        /// <param name="Offline">An optional indication whether this transaction event happened when the charging station was offline.</param>
        /// <param name="NumberOfPhasesUsed">An optional numer of electrical phases used, when the charging station is able to report it.</param>
        /// <param name="CableMaxCurrent">An optional maximum current of the connected cable in amperes.</param>
        /// <param name="ReservationId">An optional unqiue reservation identification of the reservation that terminated as a result of this transaction.</param>
        /// <param name="IdToken">An optional identification token for which a transaction has to be/was started.</param>
        /// <param name="EVSE">An optional indication of the EVSE (and connector) used.</param>
        /// <param name="MeterValues">An optional enumeration of meter values.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public TransactionEventRequest(ChargeBox_Id              ChargeBoxId,

                                       TransactionEvents         EventType,
                                       DateTime                  Timestamp,
                                       TriggerReasons            TriggerReason,
                                       UInt32                    SequenceNumber,
                                       Transaction               TransactionInfo,

                                       Boolean?                  Offline              = null,
                                       Byte?                     NumberOfPhasesUsed   = null,
                                       Int16?                    CableMaxCurrent      = null,
                                       Reservation_Id?           ReservationId        = null,
                                       IdToken?                  IdToken              = null,
                                       EVSE?                     EVSE                 = null,
                                       IEnumerable<MeterValue>?  MeterValues          = null,

                                       CustomData?               CustomData           = null,
                                       Request_Id?               RequestId            = null,
                                       DateTime?                 RequestTimestamp     = null,
                                       TimeSpan?                 RequestTimeout       = null,
                                       EventTracking_Id?         EventTrackingId      = null,
                                       CancellationToken?        CancellationToken    = null)

            : base(ChargeBoxId,
                   "TransactionEvent",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.EventType           = EventType;
            this.Timestamp           = Timestamp;
            this.TriggerReason       = TriggerReason;
            this.SequenceNumber      = SequenceNumber;
            this.TransactionInfo     = TransactionInfo;

            this.Offline             = Offline;
            this.NumberOfPhasesUsed  = NumberOfPhasesUsed;
            this.CableMaxCurrent     = CableMaxCurrent;
            this.ReservationId       = ReservationId;
            this.IdToken             = IdToken;
            this.EVSE                = EVSE;
            this.MeterValues         = MeterValues?.Distinct() ?? Array.Empty<MeterValue>();

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:TransactionEventRequest",
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
        //     "ChargingStateEnumType": {
        //       "description": "Transaction. State. Transaction_ State_ Code\r\nurn:x-oca:ocpp:uid:1:569419\r\nCurrent charging state, is required when state\r\nhas changed.\r\n",
        //       "javaType": "ChargingStateEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Charging",
        //         "EVConnected",
        //         "SuspendedEV",
        //         "SuspendedEVSE",
        //         "Idle"
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
        //     "LocationEnumType": {
        //       "description": "Sampled_ Value. Location. Location_ Code\r\nurn:x-oca:ocpp:uid:1:569265\r\nIndicates where the measured value has been sampled. Default =  \"Outlet\"\r\n\r\n",
        //       "javaType": "LocationEnum",
        //       "type": "string",
        //       "default": "Outlet",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Body",
        //         "Cable",
        //         "EV",
        //         "Inlet",
        //         "Outlet"
        //       ]
        //     },
        //     "MeasurandEnumType": {
        //       "description": "Sampled_ Value. Measurand. Measurand_ Code\r\nurn:x-oca:ocpp:uid:1:569263\r\nType of measurement. Default = \"Energy.Active.Import.Register\"\r\n",
        //       "javaType": "MeasurandEnum",
        //       "type": "string",
        //       "default": "Energy.Active.Import.Register",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Current.Export",
        //         "Current.Import",
        //         "Current.Offered",
        //         "Energy.Active.Export.Register",
        //         "Energy.Active.Import.Register",
        //         "Energy.Reactive.Export.Register",
        //         "Energy.Reactive.Import.Register",
        //         "Energy.Active.Export.Interval",
        //         "Energy.Active.Import.Interval",
        //         "Energy.Active.Net",
        //         "Energy.Reactive.Export.Interval",
        //         "Energy.Reactive.Import.Interval",
        //         "Energy.Reactive.Net",
        //         "Energy.Apparent.Net",
        //         "Energy.Apparent.Import",
        //         "Energy.Apparent.Export",
        //         "Frequency",
        //         "Power.Active.Export",
        //         "Power.Active.Import",
        //         "Power.Factor",
        //         "Power.Offered",
        //         "Power.Reactive.Export",
        //         "Power.Reactive.Import",
        //         "SoC",
        //         "Voltage"
        //       ]
        //     },
        //     "PhaseEnumType": {
        //       "description": "Sampled_ Value. Phase. Phase_ Code\r\nurn:x-oca:ocpp:uid:1:569264\r\nIndicates how the measured value is to be interpreted. For instance between L1 and neutral (L1-N) Please note that not all values of phase are applicable to all Measurands. When phase is absent, the measured value is interpreted as an overall value.\r\n",
        //       "javaType": "PhaseEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "L1",
        //         "L2",
        //         "L3",
        //         "N",
        //         "L1-N",
        //         "L2-N",
        //         "L3-N",
        //         "L1-L2",
        //         "L2-L3",
        //         "L3-L1"
        //       ]
        //     },
        //     "ReadingContextEnumType": {
        //       "description": "Sampled_ Value. Context. Reading_ Context_ Code\r\nurn:x-oca:ocpp:uid:1:569261\r\nType of detail value: start, end or sample. Default = \"Sample.Periodic\"\r\n",
        //       "javaType": "ReadingContextEnum",
        //       "type": "string",
        //       "default": "Sample.Periodic",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Interruption.Begin",
        //         "Interruption.End",
        //         "Other",
        //         "Sample.Clock",
        //         "Sample.Periodic",
        //         "Transaction.Begin",
        //         "Transaction.End",
        //         "Trigger"
        //       ]
        //     },
        //     "ReasonEnumType": {
        //       "description": "Transaction. Stopped_ Reason. EOT_ Reason_ Code\r\nurn:x-oca:ocpp:uid:1:569413\r\nThis contains the reason why the transaction was stopped. MAY only be omitted when Reason is \"Local\".\r\n",
        //       "javaType": "ReasonEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "DeAuthorized",
        //         "EmergencyStop",
        //         "EnergyLimitReached",
        //         "EVDisconnected",
        //         "GroundFault",
        //         "ImmediateReset",
        //         "Local",
        //         "LocalOutOfCredit",
        //         "MasterPass",
        //         "Other",
        //         "OvercurrentFault",
        //         "PowerLoss",
        //         "PowerQuality",
        //         "Reboot",
        //         "Remote",
        //         "SOCLimitReached",
        //         "StoppedByEV",
        //         "TimeLimitReached",
        //         "Timeout"
        //       ]
        //     },
        //     "TransactionEventEnumType": {
        //       "description": "This contains the type of this event.\r\nThe first TransactionEvent of a transaction SHALL contain: \"Started\" The last TransactionEvent of a transaction SHALL contain: \"Ended\" All others SHALL contain: \"Updated\"\r\n",
        //       "javaType": "TransactionEventEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Ended",
        //         "Started",
        //         "Updated"
        //       ]
        //     },
        //     "TriggerReasonEnumType": {
        //       "description": "Reason the Charging Station sends this message to the CSMS\r\n",
        //       "javaType": "TriggerReasonEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Authorized",
        //         "CablePluggedIn",
        //         "ChargingRateChanged",
        //         "ChargingStateChanged",
        //         "Deauthorized",
        //         "EnergyLimitReached",
        //         "EVCommunicationLost",
        //         "EVConnectTimeout",
        //         "MeterValueClock",
        //         "MeterValuePeriodic",
        //         "TimeLimitReached",
        //         "Trigger",
        //         "UnlockCommand",
        //         "StopAuthorized",
        //         "EVDeparted",
        //         "EVDetected",
        //         "RemoteStop",
        //         "RemoteStart",
        //         "AbnormalCondition",
        //         "SignedDataReceived",
        //         "ResetCommand"
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
        //     "EVSEType": {
        //       "description": "EVSE\r\nurn:x-oca:ocpp:uid:2:233123\r\nElectric Vehicle Supply Equipment\r\n",
        //       "javaType": "EVSE",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "id": {
        //           "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nEVSE Identifier. This contains a number (&gt; 0) designating an EVSE of the Charging Station.\r\n",
        //           "type": "integer"
        //         },
        //         "connectorId": {
        //           "description": "An id to designate a specific connector (on an EVSE) by connector index number.\r\n",
        //           "type": "integer"
        //         }
        //       },
        //       "required": [
        //         "id"
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
        //     "MeterValueType": {
        //       "description": "Meter_ Value\r\nurn:x-oca:ocpp:uid:2:233265\r\nCollection of one or more sampled values in MeterValuesRequest and TransactionEvent. All sampled values in a MeterValue are sampled at the same point in time.\r\n",
        //       "javaType": "MeterValue",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "sampledValue": {
        //           "type": "array",
        //           "additionalItems": false,
        //           "items": {
        //             "$ref": "#/definitions/SampledValueType"
        //           },
        //           "minItems": 1
        //         },
        //         "timestamp": {
        //           "description": "Meter_ Value. Timestamp. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569259\r\nTimestamp for measured value(s).\r\n",
        //           "type": "string",
        //           "format": "date-time"
        //         }
        //       },
        //       "required": [
        //         "timestamp",
        //         "sampledValue"
        //       ]
        //     },
        //     "SampledValueType": {
        //       "description": "Sampled_ Value\r\nurn:x-oca:ocpp:uid:2:233266\r\nSingle sampled value in MeterValues. Each value can be accompanied by optional fields.\r\n\r\nTo save on mobile data usage, default values of all of the optional fields are such that. The value without any additional fields will be interpreted, as a register reading of active import energy in Wh (Watt-hour) units.\r\n",
        //       "javaType": "SampledValue",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "value": {
        //           "description": "Sampled_ Value. Value. Measure\r\nurn:x-oca:ocpp:uid:1:569260\r\nIndicates the measured value.\r\n\r\n",
        //           "type": "number"
        //         },
        //         "context": {
        //           "$ref": "#/definitions/ReadingContextEnumType"
        //         },
        //         "measurand": {
        //           "$ref": "#/definitions/MeasurandEnumType"
        //         },
        //         "phase": {
        //           "$ref": "#/definitions/PhaseEnumType"
        //         },
        //         "location": {
        //           "$ref": "#/definitions/LocationEnumType"
        //         },
        //         "signedMeterValue": {
        //           "$ref": "#/definitions/SignedMeterValueType"
        //         },
        //         "unitOfMeasure": {
        //           "$ref": "#/definitions/UnitOfMeasureType"
        //         }
        //       },
        //       "required": [
        //         "value"
        //       ]
        //     },
        //     "SignedMeterValueType": {
        //       "description": "Represent a signed version of the meter value.\r\n",
        //       "javaType": "SignedMeterValue",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "signedMeterData": {
        //           "description": "Base64 encoded, contains the signed data which might contain more then just the meter value. It can contain information like timestamps, reference to a customer etc.\r\n",
        //           "type": "string",
        //           "maxLength": 2500
        //         },
        //         "signingMethod": {
        //           "description": "Method used to create the digital signature.\r\n",
        //           "type": "string",
        //           "maxLength": 50
        //         },
        //         "encodingMethod": {
        //           "description": "Method used to encode the meter values before applying the digital signature algorithm.\r\n",
        //           "type": "string",
        //           "maxLength": 50
        //         },
        //         "publicKey": {
        //           "description": "Base64 encoded, sending depends on configuration variable _PublicKeyWithSignedMeterValue_.\r\n",
        //           "type": "string",
        //           "maxLength": 2500
        //         }
        //       },
        //       "required": [
        //         "signedMeterData",
        //         "signingMethod",
        //         "encodingMethod",
        //         "publicKey"
        //       ]
        //     },
        //     "TransactionType": {
        //       "description": "Transaction\r\nurn:x-oca:ocpp:uid:2:233318\r\n",
        //       "javaType": "Transaction",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "transactionId": {
        //           "description": "This contains the Id of the transaction.\r\n",
        //           "type": "string",
        //           "maxLength": 36
        //         },
        //         "chargingState": {
        //           "$ref": "#/definitions/ChargingStateEnumType"
        //         },
        //         "timeSpentCharging": {
        //           "description": "Transaction. Time_ Spent_ Charging. Elapsed_ Time\r\nurn:x-oca:ocpp:uid:1:569415\r\nContains the total time that energy flowed from EVSE to EV during the transaction (in seconds). Note that timeSpentCharging is smaller or equal to the duration of the transaction.\r\n",
        //           "type": "integer"
        //         },
        //         "stoppedReason": {
        //           "$ref": "#/definitions/ReasonEnumType"
        //         },
        //         "remoteStartId": {
        //           "description": "The ID given to remote start request (&lt;&lt;requeststarttransactionrequest, RequestStartTransactionRequest&gt;&gt;. This enables to CSMS to match the started transaction to the given start request.\r\n",
        //           "type": "integer"
        //         }
        //       },
        //       "required": [
        //         "transactionId"
        //       ]
        //     },
        //     "UnitOfMeasureType": {
        //       "description": "Represents a UnitOfMeasure with a multiplier\r\n",
        //       "javaType": "UnitOfMeasure",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "unit": {
        //           "description": "Unit of the value. Default = \"Wh\" if the (default) measurand is an \"Energy\" type.\r\nThis field SHALL use a value from the list Standardized Units of Measurements in Part 2 Appendices. \r\nIf an applicable unit is available in that list, otherwise a \"custom\" unit might be used.\r\n",
        //           "type": "string",
        //           "default": "Wh",
        //           "maxLength": 20
        //         },
        //         "multiplier": {
        //           "description": "Multiplier, this value represents the exponent to base 10. I.e. multiplier 3 means 10 raised to the 3rd power. Default is 0.\r\n",
        //           "type": "integer",
        //           "default": 0
        //         }
        //       }
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "eventType": {
        //       "$ref": "#/definitions/TransactionEventEnumType"
        //     },
        //     "meterValue": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/MeterValueType"
        //       },
        //       "minItems": 1
        //     },
        //     "timestamp": {
        //       "description": "The date and time at which this transaction event occurred.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "triggerReason": {
        //       "$ref": "#/definitions/TriggerReasonEnumType"
        //     },
        //     "seqNo": {
        //       "description": "Incremental sequence number, helps with determining if all messages of a transaction have been received.\r\n",
        //       "type": "integer"
        //     },
        //     "offline": {
        //       "description": "Indication that this transaction event happened when the Charging Station was offline. Default = false, meaning: the event occurred when the Charging Station was online.\r\n",
        //       "type": "boolean",
        //       "default": false
        //     },
        //     "numberOfPhasesUsed": {
        //       "description": "If the Charging Station is able to report the number of phases used, then it SHALL provide it. When omitted the CSMS may be able to determine the number of phases used via device management.\r\n",
        //       "type": "integer"
        //     },
        //     "cableMaxCurrent": {
        //       "description": "The maximum current of the connected cable in Ampere (A).\r\n",
        //       "type": "integer"
        //     },
        //     "reservationId": {
        //       "description": "This contains the Id of the reservation that terminates as a result of this transaction.\r\n",
        //       "type": "integer"
        //     },
        //     "transactionInfo": {
        //       "$ref": "#/definitions/TransactionType"
        //     },
        //     "evse": {
        //       "$ref": "#/definitions/EVSEType"
        //     },
        //     "idToken": {
        //       "$ref": "#/definitions/IdTokenType"
        //     }
        //   },
        //   "required": [
        //     "eventType",
        //     "timestamp",
        //     "triggerReason",
        //     "seqNo",
        //     "transactionInfo"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomTransactionEventRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a transaction event request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomTransactionEventRequestParser">A delegate to parse custom transaction event requests.</param>
        public static TransactionEventRequest Parse(JObject                                                JSON,
                                                    Request_Id                                             RequestId,
                                                    ChargeBox_Id                                           ChargeBoxId,
                                                    CustomJObjectParserDelegate<TransactionEventRequest>?  CustomTransactionEventRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var transactionEventRequest,
                         out var errorResponse,
                         CustomTransactionEventRequestParser))
            {
                return transactionEventRequest!;
            }

            throw new ArgumentException("The given JSON representation of a transaction event request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out TransactionEventRequest, out ErrorResponse, CustomTransactionEventRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a transaction event request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="TransactionEventRequest">The parsed transaction event request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTransactionEventRequestParser">A delegate to parse custom transaction event requests.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       Request_Id                                             RequestId,
                                       ChargeBox_Id                                           ChargeBoxId,
                                       out TransactionEventRequest?                           TransactionEventRequest,
                                       out String?                                            ErrorResponse,
                                       CustomJObjectParserDelegate<TransactionEventRequest>?  CustomTransactionEventRequestParser)
        {

            try
            {

                TransactionEventRequest = null;

                #region EventType             [mandatory]

                if (!JSON.ParseMandatory("eventType",
                                         "event type",
                                         TransactionEventsExtensions.TryParse,
                                         out TransactionEvents EventType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timestamp             [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "timestamp",
                                         out DateTime Timestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region TriggerReason         [mandatory]

                if (!JSON.ParseMandatory("triggerReason",
                                         "trigger reason",
                                         TriggerReasonsExtensions.TryParse,
                                         out TriggerReasons TriggerReason,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SequenceNumber        [mandatory]

                if (!JSON.ParseMandatory("seqNo",
                                         "sequence number",
                                         out UInt32 SequenceNumber,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region TransactionInfo       [mandatory]

                if (!JSON.ParseMandatoryJSON("transactionInfo",
                                             "transaction info",
                                             OCPPv2_0.Transaction.TryParse,
                                             out Transaction? Transaction,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (Transaction is null)
                    return false;

                #endregion


                #region Offline               [optional]

                if (JSON.ParseOptional("offline",
                                       "offline",
                                       out Boolean? Offline,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region NumberOfPhasesUsed    [optional]

                if (JSON.ParseOptional("numberOfPhasesUsed",
                                       "number of phases used",
                                       out Byte? NumberOfPhasesUsed,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CableMaxCurrent       [optional]

                if (JSON.ParseOptional("cableMaxCurrent",
                                       "cable max current",
                                       out Int16? CableMaxCurrent,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ReservationId         [optional]

                if (JSON.ParseOptional("reservationId",
                                       "reservation identification",
                                       Reservation_Id.TryParse,
                                       out Reservation_Id? ReservationId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region IdToken               [optional]

                if (JSON.ParseOptionalJSON("idToken",
                                           "identification token",
                                           OCPPv2_0.IdToken.TryParse,
                                           out IdToken? IdToken,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVSE                  [optional]

                if (JSON.ParseOptionalJSON("evse",
                                           "EVSE",
                                           OCPPv2_0.EVSE.TryParse,
                                           out EVSE? EVSE,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MeterValues           [optional]

                if (JSON.ParseOptionalHashSet("meterValue",
                                              "meter values",
                                              MeterValue.TryParse,
                                              out HashSet<MeterValue> MeterValues,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region CustomData            [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId           [optional, OCPP_CSE]

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


                TransactionEventRequest = new TransactionEventRequest(ChargeBoxId,

                                                                      EventType,
                                                                      Timestamp,
                                                                      TriggerReason,
                                                                      SequenceNumber,
                                                                      Transaction,

                                                                      Offline,
                                                                      NumberOfPhasesUsed,
                                                                      CableMaxCurrent,
                                                                      ReservationId,
                                                                      IdToken,
                                                                      EVSE,
                                                                      MeterValues,

                                                                      CustomData,
                                                                      RequestId);

                if (CustomTransactionEventRequestParser is not null)
                    TransactionEventRequest = CustomTransactionEventRequestParser(JSON,
                                                                                  TransactionEventRequest);

                return true;

            }
            catch (Exception e)
            {
                TransactionEventRequest  = null;
                ErrorResponse            = "The given JSON representation of a transaction event request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTransactionEventRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTransactionEventRequestSerializer">A delegate to serialize custom TransactionEvent requests.</param>#
        /// <param name="CustomTransactionSerializer">A delegate to serialize custom transaction objects.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomMeterValueSerializer">A delegate to serialize custom meter values.</param>
        /// <param name="CustomSampledValueSerializer">A delegate to serialize custom sampled values.</param>
        /// <param name="CustomSignedMeterValueSerializer">A delegate to serialize custom signed meter values.</param>
        /// <param name="CustomUnitsOfMeasureSerializer">A delegate to serialize custom units of measure.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TransactionEventRequest>?  CustomTransactionEventRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Transaction>?              CustomTransactionSerializer               = null,
                              CustomJObjectSerializerDelegate<IdToken>?                  CustomIdTokenSerializer                   = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?           CustomAdditionalInfoSerializer            = null,
                              CustomJObjectSerializerDelegate<EVSE>?                     CustomEVSESerializer                      = null,
                              CustomJObjectSerializerDelegate<MeterValue>?               CustomMeterValueSerializer                = null,
                              CustomJObjectSerializerDelegate<SampledValue>?             CustomSampledValueSerializer              = null,
                              CustomJObjectSerializerDelegate<SignedMeterValue>?         CustomSignedMeterValueSerializer          = null,
                              CustomJObjectSerializerDelegate<UnitsOfMeasure>?           CustomUnitsOfMeasureSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var JSON = JSONObject.Create(

                                 new JProperty("eventType",           EventType.               AsText()),
                                 new JProperty("timestamp",           Timestamp.               ToIso8601()),
                                 new JProperty("triggerReason",       TriggerReason.           AsText()),
                                 new JProperty("seqNo",               SequenceNumber),
                                 new JProperty("transactionInfo",     TransactionInfo.         ToJSON(CustomTransactionSerializer,
                                                                                                      CustomCustomDataSerializer)),

                           Offline.HasValue
                               ? new JProperty("offline",             Offline.           Value)
                               : null,

                           NumberOfPhasesUsed.HasValue
                               ? new JProperty("numberOfPhasesUsed",  NumberOfPhasesUsed.Value)
                               : null,

                           CableMaxCurrent.HasValue
                               ? new JProperty("cableMaxCurrent",     CableMaxCurrent.   Value)
                               : null,

                           ReservationId.HasValue
                               ? new JProperty("reservationId",       ReservationId.     Value.ToString())
                               : null,

                           IdToken is not null
                               ? new JProperty("idToken",             IdToken.                 ToJSON(CustomIdTokenSerializer,
                                                                                                      CustomAdditionalInfoSerializer,
                                                                                                      CustomCustomDataSerializer))
                               : null,

                           EVSE is not null
                               ? new JProperty("evse",                EVSE.                    ToJSON(CustomEVSESerializer,
                                                                                                      CustomCustomDataSerializer))
                               : null,

                           MeterValues.Any()
                               ? new JProperty("meterValue",          new JArray(MeterValues.Select(meterValue => meterValue.ToJSON(CustomMeterValueSerializer,
                                                                                                                                    CustomSampledValueSerializer,
                                                                                                                                    CustomSignedMeterValueSerializer,
                                                                                                                                    CustomUnitsOfMeasureSerializer,
                                                                                                                                    CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomTransactionEventRequestSerializer is not null
                       ? CustomTransactionEventRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (TransactionEventRequest1, TransactionEventRequest2)

        /// <summary>
        /// Compares two transaction event requests for equality.
        /// </summary>
        /// <param name="TransactionEventRequest1">A transaction event request.</param>
        /// <param name="TransactionEventRequest2">Another transaction event request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (TransactionEventRequest? TransactionEventRequest1,
                                           TransactionEventRequest? TransactionEventRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TransactionEventRequest1, TransactionEventRequest2))
                return true;

            // If one is null, but not both, return false.
            if (TransactionEventRequest1 is null || TransactionEventRequest2 is null)
                return false;

            return TransactionEventRequest1.Equals(TransactionEventRequest2);

        }

        #endregion

        #region Operator != (TransactionEventRequest1, TransactionEventRequest2)

        /// <summary>
        /// Compares two transaction event requests for inequality.
        /// </summary>
        /// <param name="TransactionEventRequest1">A transaction event request.</param>
        /// <param name="TransactionEventRequest2">Another transaction event request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (TransactionEventRequest? TransactionEventRequest1,
                                           TransactionEventRequest? TransactionEventRequest2)

            => !(TransactionEventRequest1 == TransactionEventRequest2);

        #endregion

        #endregion

        #region IEquatable<TransactionEventRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two transaction event requests for equality.
        /// </summary>
        /// <param name="Object">A transaction event request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TransactionEventRequest transactionEventRequest &&
                   Equals(transactionEventRequest);

        #endregion

        #region Equals(TransactionEventRequest)

        /// <summary>
        /// Compares two transaction event requests for equality.
        /// </summary>
        /// <param name="TransactionEventRequest">A transaction event request to compare with.</param>
        public override Boolean Equals(TransactionEventRequest? TransactionEventRequest)

            => TransactionEventRequest is not null &&

               EventType.      Equals(TransactionEventRequest.EventType)       &&
               Timestamp.      Equals(TransactionEventRequest.Timestamp)       &&
               TriggerReason.  Equals(TransactionEventRequest.TriggerReason)   &&
               SequenceNumber. Equals(TransactionEventRequest.SequenceNumber)  &&
               TransactionInfo.Equals(TransactionEventRequest.TransactionInfo) &&

            ((!Offline.           HasValue    && !TransactionEventRequest.Offline.           HasValue)    ||
               Offline.           HasValue    &&  TransactionEventRequest.Offline.           HasValue    && Offline.           Value.Equals(TransactionEventRequest.Offline.           Value)) &&

            ((!NumberOfPhasesUsed.HasValue    && !TransactionEventRequest.NumberOfPhasesUsed.HasValue)    ||
               NumberOfPhasesUsed.HasValue    &&  TransactionEventRequest.NumberOfPhasesUsed.HasValue    && NumberOfPhasesUsed.Value.Equals(TransactionEventRequest.NumberOfPhasesUsed.Value)) &&

            ((!CableMaxCurrent.   HasValue    && !TransactionEventRequest.CableMaxCurrent.   HasValue)    ||
               CableMaxCurrent.   HasValue    &&  TransactionEventRequest.CableMaxCurrent.   HasValue    && CableMaxCurrent.   Value.Equals(TransactionEventRequest.CableMaxCurrent.   Value)) &&

            ((!ReservationId.     HasValue    && !TransactionEventRequest.ReservationId.     HasValue)    ||
               ReservationId.     HasValue    &&  TransactionEventRequest.ReservationId.     HasValue    && ReservationId.     Value.Equals(TransactionEventRequest.ReservationId.     Value)) &&

             ((IdToken            is     null &&  TransactionEventRequest.IdToken            is     null) ||
              (IdToken            is not null &&  TransactionEventRequest.IdToken            is not null && IdToken.                 Equals(TransactionEventRequest.IdToken))) &&

             ((EVSE               is     null &&  TransactionEventRequest.EVSE               is     null) ||
              (EVSE               is not null &&  TransactionEventRequest.EVSE               is not null && EVSE.                    Equals(TransactionEventRequest.EVSE))) &&

               base.    GenericEquals(TransactionEventRequest);

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

                return EventType.          GetHashCode()       * 41 ^
                       Timestamp.          GetHashCode()       * 37 ^
                       TriggerReason.      GetHashCode()       * 31 ^
                       SequenceNumber.     GetHashCode()       * 29 ^
                       TransactionInfo.    GetHashCode()       * 23 ^

                      (Offline?.           GetHashCode() ?? 0) * 19 ^
                      (NumberOfPhasesUsed?.GetHashCode() ?? 0) * 17 ^
                      (CableMaxCurrent?.   GetHashCode() ?? 0) * 13 ^
                      (ReservationId?.     GetHashCode() ?? 0) * 11 ^
                      (IdToken?.           GetHashCode() ?? 0) *  7 ^
                      (EVSE?.              GetHashCode() ?? 0) *  5 ^
                       MeterValues.        CalcHashCode()      *  3 ^

                       base.               GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Timestamp.ToIso8601(),
                   " ",
                   EventType, ", ",
                   TriggerReason.AsText()

               );

        #endregion

    }

}
