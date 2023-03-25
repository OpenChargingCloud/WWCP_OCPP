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

namespace cloud.charging.open.protocols.OCPPv2_0
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
        public Transaction_Id   TransactionId        { get; }

        /// <summary>
        /// The optional current charging state.
        /// </summary>
        [Optional]
        public ChargingStates?  ChargingState        { get; }

        /// <summary>
        /// The optional total time that energy flowed from the EVSE to the EV during this transaction.
        /// Note: TimeSpentCharging must be smaller or equal to the duration of the transaction.
        /// </summary>
        [Optional]
        public TimeSpan?        TimeSpentCharging    { get; }

        /// <summary>
        /// The optional reason why the transaction was stopped.
        /// MAY only be omitted when reason is "Local".
        /// </summary>
        [Optional]
        public Reasons?         StoppedReason        { get; }

        /// <summary>
        /// The optional remote start identification of the related request start transaction
        /// request to match the request with this transaction.
        /// </summary>
        public RemoteStart_Id?  RemoteStartId        { get; }

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
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public Transaction(Transaction_Id   TransactionId,
                           ChargingStates?  ChargingState       = null,
                           TimeSpan?        TimeSpentCharging   = null,
                           Reasons?         StoppedReason       = null,
                           RemoteStart_Id?  RemoteStartId       = null,
                           CustomData?      CustomData          = null)

            : base(CustomData)

        {

            this.TransactionId      = TransactionId;
            this.ChargingState      = ChargingState;
            this.TimeSpentCharging  = TimeSpentCharging;
            this.StoppedReason      = StoppedReason;
            this.RemoteStartId      = RemoteStartId;

        }

        #endregion


        #region Documentation

        // "TransactionType": {
        //   "description": "Transaction\r\nurn:x-oca:ocpp:uid:2:233318\r\n",
        //   "javaType": "Transaction",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "transactionId": {
        //       "description": "This contains the Id of the transaction.\r\n",
        //       "type": "string",
        //       "maxLength": 36
        //     },
        //     "chargingState": {
        //       "$ref": "#/definitions/ChargingStateEnumType"
        //     },
        //     "timeSpentCharging": {
        //       "description": "Transaction. Time_ Spent_ Charging. Elapsed_ Time\r\nurn:x-oca:ocpp:uid:1:569415\r\nContains the total time that energy flowed from EVSE to EV during the transaction (in seconds). Note that timeSpentCharging is smaller or equal to the duration of the transaction.\r\n",
        //       "type": "integer"
        //     },
        //     "stoppedReason": {
        //       "$ref": "#/definitions/ReasonEnumType"
        //     },
        //     "remoteStartId": {
        //       "description": "The ID given to remote start request (&lt;&lt;requeststarttransactionrequest, RequestStartTransactionRequest&gt;&gt;. This enables to CSMS to match the started transaction to the given start request.\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "transactionId"
        //   ]
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
                         CustomTransactionParser))
            {
                return transaction!;
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

                #region TransactionId        [mandatory]

                if (!JSON.ParseMandatory("transactionId",
                                         "transaction identification",
                                         Transaction_Id.TryParse,
                                         out Transaction_Id TransactionId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingState        [optional]

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

                #region TimeSpentCharging    [optional]

                if (JSON.ParseOptional("timeSpentCharging",
                                       "time spent charging",
                                       out UInt32? timeSpentCharging,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var TimeSpentCharging = timeSpentCharging.HasValue
                                            ? new TimeSpan?(TimeSpan.FromSeconds(timeSpentCharging.Value))
                                            : null;

                #endregion

                #region StoppedReason        [optional]

                if (JSON.ParseOptional("stoppedReason",
                                       "stopped reason",
                                       ReasonsExtensions.TryParse,
                                       out Reasons? StoppedReason,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region RemoteStartId        [optional]

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

                #region CustomData           [optional]

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


                Transaction = new Transaction(TransactionId,
                                              ChargingState,
                                              TimeSpentCharging,
                                              StoppedReason,
                                              RemoteStartId,
                                              CustomData);

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

                                 new JProperty("transactionId",      TransactionId.Value),

                           ChargingState.HasValue
                               ? new JProperty("chargingState",      ChargingState.Value.AsText())
                               : null,

                           TimeSpentCharging.HasValue
                               ? new JProperty("timeSpentCharging",  (UInt32) Math.Round(TimeSpentCharging.Value.TotalSeconds, 0))
                               : null,

                           StoppedReason.HasValue
                               ? new JProperty("stoppedReason",      StoppedReason.Value.AsText())
                               : null,

                           RemoteStartId.HasValue
                               ? new JProperty("remoteStartId",      RemoteStartId.Value.Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.         ToJSON(CustomCustomDataSerializer))
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

            ((!ChargingState.    HasValue && !Transaction.ChargingState.    HasValue) ||
               ChargingState.    HasValue &&  Transaction.ChargingState.    HasValue && ChargingState.    Value.Equals(Transaction.ChargingState.    Value)) &&

            ((!TimeSpentCharging.HasValue && !Transaction.TimeSpentCharging.HasValue) ||
               TimeSpentCharging.HasValue &&  Transaction.TimeSpentCharging.HasValue && TimeSpentCharging.Value.Equals(Transaction.TimeSpentCharging.Value)) &&

            ((!StoppedReason.    HasValue && !Transaction.StoppedReason.    HasValue) ||
               StoppedReason.    HasValue &&  Transaction.StoppedReason.    HasValue && StoppedReason.    Value.Equals(Transaction.StoppedReason.    Value)) &&

            ((!RemoteStartId.    HasValue && !Transaction.RemoteStartId.    HasValue) ||
               RemoteStartId.    HasValue &&  Transaction.RemoteStartId.    HasValue && RemoteStartId.    Value.Equals(Transaction.RemoteStartId.    Value)) &&

               base.         Equals(Transaction);

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

                return TransactionId.     GetHashCode()       * 13 ^
                      (ChargingState?.    GetHashCode() ?? 0) * 11 ^
                      (TimeSpentCharging?.GetHashCode() ?? 0) *  7 ^
                      (StoppedReason?.    GetHashCode() ?? 0) *  5 ^
                      (RemoteStartId?.    GetHashCode() ?? 0) *  3 ^

                       base.              GetHashCode();

            }
        }

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
                       ? ", stopped reason: "      + StoppedReason.Value.AsText()
                       : "",

                   RemoteStartId.HasValue
                       ? ", remote start id: "     + RemoteStartId.Value.ToString()
                       : ""

               );

        #endregion

    }

}
