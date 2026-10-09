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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

using System.Diagnostics.CodeAnalysis;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A charging transaction.
    /// </summary>
    public class Transaction : ACustomData,
                               ICBORSerializable<Transaction>,
                               IEquatable<Transaction>
    {

        #region Properties

        /// <summary>
        /// The unique identification of the transaction.
        /// </summary>
        [Mandatory]
        public Transaction_Id          Id                       { get; }

        /// <summary>
        /// The optional current charging state.
        /// </summary>
        [Optional]
        public ChargingStates?         ChargingState            { get; }

        /// <summary>
        /// The optional total time that energy flowed from the EVSE to the EV during this transaction.
        /// Note: TimeSpentCharging must be smaller or equal to the duration of the transaction.
        /// </summary>
        [Optional]
        public TimeSpan?               TimeSpentCharging        { get; }

        /// <summary>
        /// The optional reason why the transaction was stopped.
        /// MAY only be omitted when reason is "Local".
        /// </summary>
        [Optional]
        public StopTransactionReason?  StoppedReason            { get; }

        /// <summary>
        /// The optional remote start identification of the related request start transaction
        /// request to match the request with this transaction.
        /// </summary>
        [Optional]
        public RemoteStart_Id?         RemoteStartId            { get; }

        /// <summary>
        /// The optional operation mode that is in use at this time.
        /// </summary>
        [Optional]
        public OperationMode?          OperationMode            { get; }

        /// <summary>
        /// Optional maximum cost/energy/time limits for this transaction.
        /// </summary>
        [Optional]
        public TransactionLimits?      TransactionLimits        { get; }

        /// <summary>
        /// The current preconditioning status of the BMS in the EV.
        /// Default value is Unknown.
        /// </summary>
        public PreconditioningStatus?  PreconditioningStatus    { get; }

        /// <summary>
        ///  True when EVSE electronics are in sleep mode for this transaction.
        ///  Default value(when absent) is false.
        /// </summary>
        public Boolean?                EVSESleep                { get; }

        /// <summary>
        /// The optional unique charging tariff identification used for the transaction.
        /// </summary>
        [Optional]
        public Tariff_Id?              TariffId                 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new additional case insensitive authorization identifier.
        /// </summary>
        /// <param name="Id">The unique identification of the transaction.</param>
        /// <param name="ChargingState">The optional current charging state.</param>
        /// <param name="TimeSpentCharging">The optional total time that energy flowed from the EVSE to the EV during this transaction.</param>
        /// <param name="StoppedReason">The optional reason why the transaction was stopped. MAY only be omitted when reason is "Local".</param>
        /// <param name="RemoteStartId">The optional remote start identification of the related request start transaction request to match the request with this transaction.</param>
        /// <param name="OperationMode">An optional operation mode that is in use at this time.</param>
        /// <param name="TransactionLimits">Optional maximum cost/energy/time limits for this transaction.</param>
        /// <param name="PreconditioningStatus">The current preconditioning status of the BMS in the EV. Default value is Unknown.</param>
        /// <param name="EVSESleep">True when EVSE electronics are in sleep mode for this transaction. Default value(when absent) is false.</param>
        /// <param name="TariffId">An optional unique charging tariff identification used for the transaction.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public Transaction(Transaction_Id          Id,
                           ChargingStates?         ChargingState           = null,
                           TimeSpan?               TimeSpentCharging       = null,
                           StopTransactionReason?  StoppedReason           = null,
                           RemoteStart_Id?         RemoteStartId           = null,
                           OperationMode?          OperationMode           = null,
                           TransactionLimits?      TransactionLimits       = null,
                           PreconditioningStatus?  PreconditioningStatus   = null,
                           Boolean?                EVSESleep               = null,
                           Tariff_Id?              TariffId                = null,
                           CustomData?             CustomData              = null)

            : base(CustomData)

        {

            this.Id                     = Id;
            this.ChargingState          = ChargingState;
            this.TimeSpentCharging      = TimeSpentCharging;
            this.StoppedReason          = StoppedReason;
            this.OperationMode          = OperationMode;
            this.RemoteStartId          = RemoteStartId;
            this.TransactionLimits      = TransactionLimits;
            this.PreconditioningStatus  = PreconditioningStatus;
            this.EVSESleep              = EVSESleep;
            this.TariffId               = TariffId;

            unchecked
            {

                hashCode = this.Id.                    GetHashCode()       * 31 ^
                          (this.ChargingState?.        GetHashCode() ?? 0) * 29 ^
                          (this.TimeSpentCharging?.    GetHashCode() ?? 0) * 23 ^
                          (this.StoppedReason?.        GetHashCode() ?? 0) * 19 ^
                          (this.RemoteStartId?.        GetHashCode() ?? 0) * 17 ^
                          (this.OperationMode?.        GetHashCode() ?? 0) * 13 ^
                          (this.TransactionLimits?.    GetHashCode() ?? 0) * 11 ^
                          (this.TariffId?.             GetHashCode() ?? 0) *  7 ^
                          (this.PreconditioningStatus?.GetHashCode() ?? 0) *  5 ^
                          (this.EVSESleep?.            GetHashCode() ?? 0) *  3 ^
                           base.                       GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "Transaction",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "transactionId": {
        //             "description": "This contains the Id of the transaction.",
        //             "type": "string",
        //             "maxLength": 36
        //         },
        //         "chargingState": {
        //             "$ref": "#/definitions/ChargingStateEnumType"
        //         },
        //         "timeSpentCharging": {
        //             "description": "Contains the total time that energy flowed from EVSE to EV during the transaction (in seconds). Note that timeSpentCharging is smaller or equal to the duration of the transaction.",
        //             "type": "integer"
        //         },
        //         "stoppedReason": {
        //             "$ref": "#/definitions/ReasonEnumType"
        //         },
        //         "remoteStartId": {
        //             "description": "The ID given to remote start request (&lt;&lt;requeststarttransactionrequest, RequestStartTransactionRequest&gt;&gt;. This enables to CSMS to match the started transaction to the given start request.",
        //             "type": "integer"
        //         },
        //         "operationMode": {
        //             "$ref": "#/definitions/OperationModeEnumType"
        //         },
        //         "tariffId": {
        //             "description": "*(2.1)* Id of tariff in use for transaction",
        //             "type": "string",
        //             "maxLength": 60
        //         },
        //         "transactionLimit": {
        //             "$ref": "#/definitions/TransactionLimitType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "transactionId"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomTransactionParser = null)

        /// <summary>
        /// Parse the given JSON representation of a transaction.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomTransactionParser">A delegate to parse custom transaction JSON objects.</param>
        public static Transaction Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<Transaction>?  CustomTransactionParser   = null)
        {

            if (TryParse(JSON,
                         out var transaction,
                         out var errorResponse,
                         CustomTransactionParser) &&
                transaction is not null)
            {
                return transaction;
            }

            throw new ArgumentException("The given JSON representation of a transaction is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Transaction, CustomTransactionParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a transaction.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Transaction">The parsed transaction.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject           JSON,
                                       out Transaction?  Transaction,
                                       out String?       ErrorResponse)

            => TryParse(JSON,
                        out Transaction,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a transaction.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Transaction">The parsed transaction.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTransactionParser">A delegate to parse custom transaction JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       out Transaction?                           Transaction,
                                       out String?                                ErrorResponse,
                                       CustomJObjectParserDelegate<Transaction>?  CustomTransactionParser)
        {

            try
            {

                Transaction = default;

                #region TransactionId            [mandatory]

                if (!JSON.ParseMandatory("transactionId",
                                         "transaction identification",
                                         Transaction_Id.TryParse,
                                         out Transaction_Id TransactionId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingState            [optional]

                if (JSON.ParseOptional("chargingState",
                                       "charging state",
                                       ChargingStatesExtensions.TryParse,
                                       out ChargingStates? ChargingState,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region TimeSpentCharging        [optional]

                if (JSON.ParseOptional("timeSpentCharging",
                                       "time spent charging",
                                       out TimeSpan? TimeSpentCharging,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StoppedReason            [optional]

                if (JSON.ParseOptional("stoppedReason",
                                       "stopped reason",
                                       StopTransactionReason.TryParse,
                                       out StopTransactionReason? StoppedReason,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region RemoteStartId            [optional]

                if (JSON.ParseOptional("remoteStartId",
                                       "remote start identification",
                                       RemoteStart_Id.TryParse,
                                       out RemoteStart_Id? RemoteStartId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region OperationMode            [optional]

                if (JSON.ParseOptional("operationMode",
                                       "operation mode",
                                       OCPPv2_1.OperationMode.TryParse,
                                       out OperationMode? OperationMode,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region TransactionLimits        [optional]

                if (JSON.ParseOptionalJSON("transactionLimit",
                                           "transaction limit",
                                           OCPPv2_1.TransactionLimits.TryParse,
                                           out TransactionLimits? TransactionLimits,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region PreconditioningStatus    [optional]

                if (JSON.ParseOptional("preconditioningStatus",
                                       "preconditioning status",
                                       OCPPv2_1.PreconditioningStatus.TryParse,
                                       out PreconditioningStatus? PreconditioningStatus,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EVSESleep                [optional]

                if (JSON.ParseOptional("evseSleep",
                                       "evse sleep",
                                       out Boolean? EVSESleep,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region TariffId                 [optional]

                if (JSON.ParseOptional("tariffId",
                                       "tariff identification",
                                       Tariff_Id.TryParse,
                                       out Tariff_Id? TariffId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData               [optional]

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


                Transaction = new Transaction(

                                  TransactionId,
                                  ChargingState,
                                  TimeSpentCharging,
                                  StoppedReason,
                                  RemoteStartId,
                                  OperationMode,
                                  TransactionLimits,
                                  PreconditioningStatus,
                                  EVSESleep,
                                  TariffId,

                                  CustomData

                              );

                if (CustomTransactionParser is not null)
                    Transaction = CustomTransactionParser(JSON,
                                                          Transaction);

                return true;

            }
            catch (Exception e)
            {
                Transaction    = default;
                ErrorResponse  = "The given JSON representation of a transaction is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTransactionSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTransactionSerializer">A delegate to serialize custom transaction objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Transaction>?  CustomTransactionSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?   CustomCustomDataSerializer    = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("transactionId",           Id.   Value),

                           ChargingState.HasValue
                               ? new JProperty("chargingState",           ChargingState.   Value.AsText())
                               : null,

                           TimeSpentCharging.HasValue
                               ? new JProperty("timeSpentCharging",       (UInt32) Math.Round(TimeSpentCharging.Value.TotalSeconds, 0))
                               : null,

                           StoppedReason.HasValue
                               ? new JProperty("stoppedReason",           StoppedReason.   Value.ToString())
                               : null,

                           RemoteStartId.HasValue
                               ? new JProperty("remoteStartId",           RemoteStartId.   Value.Value)
                               : null,

                           OperationMode.HasValue
                               ? new JProperty("operationMode",           OperationMode.   Value.ToString())
                               : null,

                           TransactionLimits is not null
                               ? new JProperty("transactionLimit",        TransactionLimits.     ToJSON())
                               : null,


                           TariffId.HasValue
                               ? new JProperty("tariffId",                TariffId.        Value.ToString())
                               : null,


                           CustomData is not null
                               ? new JProperty("customData",              CustomData.            ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomTransactionSerializer is not null
                       ? CustomTransactionSerializer(this, json)
                       : json;

        }

        #endregion


        #region (static) TryParseCBOR(CBOR, out Transaction, out ErrorResponse, CustomTransactionParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a transaction.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Transaction">The transaction.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out Transaction?  Transaction,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out Transaction,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a transaction.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Transaction">The transaction.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTransactionParser">An optional delegate to read custom transactions.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out Transaction?           Transaction,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<Transaction>?  CustomTransactionParser)
        {

            try
            {

                Transaction = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a transaction is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("transactionId",
                                             "transaction identification",
                                             out var IdText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!Transaction_Id.TryParse(IdText, out var Id))
                {
                    ErrorResponse = $"Invalid transaction identification '{IdText}'!";
                    return false;
                }

                ChargingStates? ChargingState = null;

                if (CBOR.ParseOptionalText("chargingState",
                                           "charging state",
                                           out var ChargingStateText,
                                           out ErrorResponse))
                {

                    if (!ChargingStatesExtensions.TryParse(ChargingStateText!, out var ChargingStateValue))
                    {
                        ErrorResponse = $"Invalid charging state '{ChargingStateText}'!";
                        return false;
                    }

                    ChargingState = ChargingStateValue;

                }

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("timeSpentCharging",
                                        "time spent charging",
                                        OCPPCBORExtensions.TryParseDuration,
                                        out TimeSpan? TimeSpentCharging,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                StopTransactionReason? StoppedReason = null;

                if (CBOR.ParseOptionalText("stoppedReason",
                                           "stopped reason",
                                           out var StoppedReasonText,
                                           out ErrorResponse))
                {

                    if (!StopTransactionReason.TryParse(StoppedReasonText!, out var StoppedReasonValue))
                    {
                        ErrorResponse = $"Invalid stopped reason '{StoppedReasonText}'!";
                        return false;
                    }

                    StoppedReason = StoppedReasonValue;

                }

                if (ErrorResponse is not null)
                    return false;

                RemoteStart_Id? RemoteStartId = null;

                if (CBOR.ParseOptionalUInt64("remoteStartId",
                                             "remote start identification",
                                             out var RemoteStartIdNumber,
                                             out ErrorResponse))
                {

                    if (RemoteStartIdNumber is not UInt64 RemoteStartIdValue || RemoteStartIdValue > UInt64.MaxValue || !RemoteStart_Id.TryParse((UInt64) RemoteStartIdValue, out var RemoteStartIdId))
                    {
                        ErrorResponse = $"Invalid remote start identification '{RemoteStartIdNumber}'!";
                        return false;
                    }

                    RemoteStartId = RemoteStartIdId;

                }

                if (ErrorResponse is not null)
                    return false;

                OperationMode? OperationMode = null;

                if (CBOR.ParseOptionalText("operationMode",
                                           "operation mode",
                                           out var OperationModeText,
                                           out ErrorResponse))
                {

                    if (!OCPPv2_1.OperationMode.TryParse(OperationModeText!, out var OperationModeValue))
                    {
                        ErrorResponse = $"Invalid operation mode '{OperationModeText}'!";
                        return false;
                    }

                    OperationMode = OperationModeValue;

                }

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("transactionLimit",
                                   "transaction limits",
                                   OCPPv2_1.TransactionLimits.TryParseCBOR,
                                   out TransactionLimits? TransactionLimits,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                PreconditioningStatus? PreconditioningStatus = null;

                if (CBOR.ParseOptionalText("preconditioningStatus",
                                           "preconditioning status",
                                           out var PreconditioningStatusText,
                                           out ErrorResponse))
                {

                    if (!OCPPv2_1.PreconditioningStatus.TryParse(PreconditioningStatusText!, out var PreconditioningStatusValue))
                    {
                        ErrorResponse = $"Invalid preconditioning status '{PreconditioningStatusText}'!";
                        return false;
                    }

                    PreconditioningStatus = PreconditioningStatusValue;

                }

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalBoolean("evseSleep",
                                          "EVSE sleep",
                                          out var EVSESleep,
                                          out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                Tariff_Id? TariffId = null;

                if (CBOR.ParseOptionalText("tariffId",
                                           "tariff identification",
                                           out var TariffIdText,
                                           out ErrorResponse))
                {

                    if (!Tariff_Id.TryParse(TariffIdText!, out var TariffIdValue))
                    {
                        ErrorResponse = $"Invalid tariff identification '{TariffIdText}'!";
                        return false;
                    }

                    TariffId = TariffIdValue;

                }

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                Transaction = new Transaction(
                                  Id,
                                  ChargingState,
                                  TimeSpentCharging,
                                  StoppedReason,
                                  RemoteStartId,
                                  OperationMode,
                                  TransactionLimits,
                                  PreconditioningStatus,
                                  EVSESleep,
                                  TariffId,
                                  CustomData
                              );

                if (CustomTransactionParser is not null)
                    Transaction = CustomTransactionParser(CBOR,
                                             Transaction);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                Transaction  = default;
                ErrorResponse  = "The given CBOR representation of a transaction is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<Transaction>.TryParse(CBOR, out Transaction, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a transaction - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<Transaction>.TryParse(CBORValue                         CBOR,
                                                             out Transaction                  Value,
                                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomTransactionSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this transaction: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomTransactionSerializer">A delegate to serialize custom transactions.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<Transaction>? CustomTransactionSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("transactionId",           CBORValue.FromText(Id.Value)),
                           ("chargingState",           OCPPCBORExtensions.Text(ChargingState?.AsText())),
                           ("timeSpentCharging",       TimeSpentCharging?.ToCBOR()),
                           ("stoppedReason",           OCPPCBORExtensions.Text(StoppedReason?.ToString())),
                           ("remoteStartId",           OCPPCBORExtensions.UInt(RemoteStartId?.Value)),
                           ("operationMode",           OCPPCBORExtensions.Text(OperationMode?.ToString())),
                           ("transactionLimit",        TransactionLimits?.ToCBOR()),
                           ("preconditioningStatus",   OCPPCBORExtensions.Text(PreconditioningStatus?.ToString())),
                           ("evseSleep",               OCPPCBORExtensions.Flag(EVSESleep)),
                           ("tariffId",                OCPPCBORExtensions.Text(TariffId?.ToString())),
                           ("customData",              CustomData?.ToCBOR())
                       );

            return CustomTransactionSerializer is not null
                       ? CustomTransactionSerializer(this, cbor)
                       : cbor;

        }

        #endregion

        #region Operator overloading

        #region Operator == (Transaction1, Transaction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Transaction1">A transaction.</param>
        /// <param name="Transaction2">Another transaction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Transaction? Transaction1,
                                           Transaction? Transaction2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(Transaction1, Transaction2))
                return true;

            // If one is null, but not both, return false.
            if (Transaction1 is null || Transaction2 is null)
                return false;

            return Transaction1.Equals(Transaction2);

        }

        #endregion

        #region Operator != (Transaction1, Transaction2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Transaction1">A transaction.</param>
        /// <param name="Transaction2">Another transaction.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Transaction? Transaction1,
                                           Transaction? Transaction2)

            => !(Transaction1 == Transaction2);

        #endregion

        #endregion

        #region IEquatable<Transaction> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two transactions for equality.
        /// </summary>
        /// <param name="Object">A transaction to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Transaction transaction &&
                   Equals(transaction);

        #endregion

        #region Equals(Transaction)

        /// <summary>
        /// Compares two transactions for equality.
        /// </summary>
        /// <param name="Transaction">A transaction to compare with.</param>
        public Boolean Equals(Transaction? Transaction)

            => Transaction is not null &&

               Id.Equals(Transaction.Id) &&

            ((!ChargingState.        HasValue    && !Transaction.ChargingState.        HasValue) ||
               ChargingState.        HasValue    &&  Transaction.ChargingState.        HasValue    && ChargingState.        Value.Equals(Transaction.ChargingState.        Value)) &&

            ((!TimeSpentCharging.    HasValue    && !Transaction.TimeSpentCharging.    HasValue)    ||
               TimeSpentCharging.    HasValue    &&  Transaction.TimeSpentCharging.    HasValue    && TimeSpentCharging.    Value.Equals(Transaction.TimeSpentCharging.    Value)) &&

            ((!StoppedReason.        HasValue    && !Transaction.StoppedReason.        HasValue)    ||
               StoppedReason.        HasValue    &&  Transaction.StoppedReason.        HasValue    && StoppedReason.        Value.Equals(Transaction.StoppedReason.        Value)) &&

            ((!RemoteStartId.        HasValue    && !Transaction.RemoteStartId.        HasValue)    ||
               RemoteStartId.        HasValue    &&  Transaction.RemoteStartId.        HasValue    && RemoteStartId.        Value.Equals(Transaction.RemoteStartId.        Value)) &&

            ((!OperationMode.        HasValue    && !Transaction.OperationMode.        HasValue)    ||
               OperationMode.        HasValue    &&  Transaction.OperationMode.        HasValue    && OperationMode.        Value.Equals(Transaction.OperationMode.        Value)) &&

             ((TransactionLimits     is null     &&  Transaction.TransactionLimits     is null     ) ||
               TransactionLimits     is not null &&  Transaction.TransactionLimits     is not null && TransactionLimits.          Equals(Transaction.TransactionLimits))           &&

            ((!PreconditioningStatus.HasValue    && !Transaction.PreconditioningStatus.HasValue)    ||
               PreconditioningStatus.HasValue    &&  Transaction.PreconditioningStatus.HasValue    && PreconditioningStatus.Value.Equals(Transaction.PreconditioningStatus.Value)) &&

            ((!EVSESleep.            HasValue    && !Transaction.EVSESleep.            HasValue)    ||
               EVSESleep.            HasValue    &&  Transaction.EVSESleep.            HasValue    && EVSESleep.            Value.Equals(Transaction.EVSESleep.            Value)) &&

            ((!TariffId.     HasValue    && !Transaction.TariffId.     HasValue) ||
               TariffId.     HasValue    &&  Transaction.TariffId.     HasValue    && TariffId.     Value.Equals(Transaction.TariffId.     Value)) &&

               base.Equals(Transaction);

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

                   Id,

                   ChargingState.HasValue
                       ? ", charging state:"       + ChargingState.Value.AsText()
                       : "",

                   TimeSpentCharging.HasValue
                       ? ", time spent charging: " + Math.Round(TimeSpentCharging.Value.TotalSeconds, 0) + " sec"
                       : "",

                   StoppedReason.HasValue
                       ? ", stopped reason: "      + StoppedReason.Value.ToString()
                       : "",

                   RemoteStartId.HasValue
                       ? ", remote start id: "     + RemoteStartId.Value.ToString()
                       : "",

                   OperationMode.HasValue
                       ? ", operation mode: "      + OperationMode.Value.ToString()
                       : ""

               );

        #endregion

    }

}
