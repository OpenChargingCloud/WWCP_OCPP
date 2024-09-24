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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A charging transaction.
    /// </summary>
    public class Transaction : ACustomData,
                               IEquatable<Transaction>
    {

        #region Properties

        /// <summary>
        /// The unique identification of the transaction.
        /// </summary>
        [Mandatory]
        public Transaction_Id          TransactionId            { get; }

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
        public Tariff_Id?              ChargingTariffId         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new additional case insensitive authorization identifier.
        /// </summary>
        /// <param name="TransactionId">The unique identification of the transaction.</param>
        /// <param name="ChargingState">The optional current charging state.</param>
        /// <param name="TimeSpentCharging">The optional total time that energy flowed from the EVSE to the EV during this transaction.</param>
        /// <param name="StoppedReason">The optional reason why the transaction was stopped. MAY only be omitted when reason is "Local".</param>
        /// <param name="RemoteStartId">The optional remote start identification of the related request start transaction request to match the request with this transaction.</param>
        /// <param name="OperationMode">An optional operation mode that is in use at this time.</param>
        /// <param name="TransactionLimits">Optional maximum cost/energy/time limits for this transaction.</param>
        /// <param name="PreconditioningStatus">The current preconditioning status of the BMS in the EV. Default value is Unknown.</param>
        /// <param name="EVSESleep">True when EVSE electronics are in sleep mode for this transaction. Default value(when absent) is false.</param>
        /// <param name="ChargingTariffId">An optional unique charging tariff identification used for the transaction.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public Transaction(Transaction_Id          TransactionId,
                           ChargingStates?         ChargingState           = null,
                           TimeSpan?               TimeSpentCharging       = null,
                           StopTransactionReason?  StoppedReason           = null,
                           RemoteStart_Id?         RemoteStartId           = null,
                           OperationMode?          OperationMode           = null,
                           TransactionLimits?      TransactionLimits       = null,
                           PreconditioningStatus?  PreconditioningStatus   = null,
                           Boolean?                EVSESleep               = null,
                           Tariff_Id?              ChargingTariffId        = null,
                           CustomData?             CustomData              = null)

            : base(CustomData)

        {

            this.TransactionId          = TransactionId;
            this.ChargingState          = ChargingState;
            this.TimeSpentCharging      = TimeSpentCharging;
            this.StoppedReason          = StoppedReason;
            this.OperationMode          = OperationMode;
            this.RemoteStartId          = RemoteStartId;
            this.TransactionLimits      = TransactionLimits;
            this.PreconditioningStatus  = PreconditioningStatus;
            this.EVSESleep              = EVSESleep;
            this.ChargingTariffId       = ChargingTariffId;

            unchecked
            {

                hashCode = this.TransactionId.         GetHashCode()       * 31 ^
                          (this.ChargingState?.        GetHashCode() ?? 0) * 29 ^
                          (this.TimeSpentCharging?.    GetHashCode() ?? 0) * 23 ^
                          (this.StoppedReason?.        GetHashCode() ?? 0) * 19 ^
                          (this.RemoteStartId?.        GetHashCode() ?? 0) * 17 ^
                          (this.OperationMode?.        GetHashCode() ?? 0) * 13 ^
                          (this.TransactionLimits?.    GetHashCode() ?? 0) * 11 ^
                          (this.ChargingTariffId?.     GetHashCode() ?? 0) *  7 ^
                          (this.PreconditioningStatus?.GetHashCode() ?? 0) *  5 ^
                          (this.EVSESleep?.            GetHashCode() ?? 0) *  3 ^
                           base.                       GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


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

                #region ChargingTariffId         [optional]

                if (JSON.ParseOptional("tariffId",
                                       "charging tariff identification",
                                       Tariff_Id.TryParse,
                                       out Tariff_Id? ChargingTariffId,
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
                                  ChargingTariffId,

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

                                 new JProperty("transactionId",           TransactionId.   Value),

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

                           PreconditioningStatus is not null
                               ? new JProperty("preconditioningStatus",   PreconditioningStatus. ToString())
                               : null,

                           EVSESleep is not null
                               ? new JProperty("evseSleep",               EVSESleep.Value)
                               : null,

                           ChargingTariffId.HasValue
                               ? new JProperty("tariffId",                ChargingTariffId.Value.ToString())
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

               TransactionId.Equals(Transaction.TransactionId) &&

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

            ((!ChargingTariffId.     HasValue    && !Transaction.ChargingTariffId.     HasValue) ||
               ChargingTariffId.     HasValue    &&  Transaction.ChargingTariffId.     HasValue    && ChargingTariffId.     Value.Equals(Transaction.ChargingTariffId.     Value)) &&

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

                   TransactionId,

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
