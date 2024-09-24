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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Logical Component responsible for configuration relating to transaction characteristics and behaviour.
    /// </summary>
    public class TxCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// Interval in seconds from between "starting" of a transaction until incipient transaction is automatically canceled, due to failure of EV driver to (correctly) insert the charging cable connector(s) into the appropriate socket(s).
        /// The Charging Station SHALL go back to the original state, probably: 'Available'.
        /// "Starting" might be the swiping of the RFID, pressing a start button, a RequestStartTransactionRequest being received etc.
        /// </summary>
        public TimeSpan                   EVConnectionTimeOut         { get; set; }

        /// <summary>
        /// With this configuration variable the Charging Station can be configured to allow charging before having received a BootNotificationResponse with RegistrationStatus: Accepted.
        /// See: Transactions before being accepted by a CSMS.
        /// </summary>
        public Boolean?                   TxBeforeAcceptedEnabled     { get; set; }

        /// <summary>
        /// Defines when the Charging Station starts a new transaction: first transactioneventRequest: eventType = Started.
        /// When any event in the given list occurs, the Charging Station SHALL start a transaction.
        /// The Charging Station SHALL only send the Started event once for every transaction.
        /// It is advised to put all events that should be part of a transaction in the list, in case the start event never occurs.
        /// Because the possible events don’t always have to come in the same order it is possible to provide a list of events.
        /// Which ever comes first will then cause a transaction to be started.
        /// For example: EVConnected, Authorized would mean that a transaction is started when an EV is detected (Cable is connected), or when an EV Driver swipes his RFID card and the CSMS successfully authorizes the ID for charging.
        /// </summary>
        public IEnumerable<TxStartPoint>  TxStartPoint                { get; set; }

        /// <summary>
        /// Defines when the Charging Station ends a transaction: last transactioneventRequest: eventType = Ended.
        /// When any event in the given list is no longer valid, the Charging Station SHALL end the transaction.
        /// The Charging Station SHALL only send the Ended event once for every transaction.
        /// MaxEnergyOnInvalidId integer Maximum amount of energy in Wh delivered when an identifier is deauthorized by the CSMS after start of a transaction.
        /// </summary>
        public IEnumerable<TxStopPoint>   TxStopPoint                 { get; set; }

        /// <summary>
        /// Whether the Charging Station will stop an ongoing transaction when it receives a non-accepted authorization status in TransactionEventResponse for this transaction.
        /// </summary>
        public Boolean?                   StopTxOnInvalidId           { get; set; }

        /// <summary>
        /// Maximum amount of energy in Wh delivered when an identifier is deauthorized by the CSMS after start of a transaction.
        /// </summary>
        public WattHour?                  MaxEnergyOnInvalidId        { get; set; }

        /// <summary>
        /// When set to true, the Charging Station SHALL administratively stop the transaction when the cable is unplugged from the EV.
        /// </summary>
        public Boolean                    StopTxOnEVSideDisconnect    { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new clock controller.
        /// </summary>
        /// <param name="EVConnectionTimeOut">Interval in seconds from between "starting" of a transaction until incipient transaction is automatically canceled, due to failure of EV driver to (correctly) insert the charging cable connector(s) into the appropriate socket(s). The Charging Station SHALL go back to the original state, probably: 'Available'. "Starting" might be the swiping of the RFID, pressing a start button, a RequestStartTransactionRequest being received etc.</param>
        /// <param name="TxBeforeAcceptedEnabled">With this configuration variable the Charging Station can be configured to allow charging before having received a BootNotificationResponse with RegistrationStatus: Accepted. See: Transactions before being accepted by a CSMS.</param>
        /// <param name="TxStartPoint">Defines when the Charging Station starts a new transaction: first transactioneventRequest: eventType = Started. When any event in the given list occurs, the Charging Station SHALL start a transaction The Charging Station SHALL only send the Started event once for every transaction. It is advised to put all events that should be part of a transaction in the list, in case the start event never occurs. Because the possible events don’t always have to come in the same order it is possible to provide a list of events. Which ever comes first will then cause a transaction to be started. For example: EVConnected, Authorized would mean that a transaction is started when an EV is detected (Cable is connected), or when an EV Driver swipes his RFID card and the CSMS successfully authorizes the ID for charging.</param>
        /// <param name="TxStopPoint">Defines when the Charging Station ends a transaction: last transactioneventRequest: eventType = Ended. When any event in the given list is no longer valid, the Charging Station SHALL end the transaction. The Charging Station SHALL only send the Ended event once for every transaction. MaxEnergyOnInvalidId integer Maximum amount of energy in Wh delivered when an identifier is deauthorized by the CSMS after start of a transaction.</param>
        /// <param name="StopTxOnInvalidId">Whether the Charging Station will stop an ongoing transaction when it receives a non-accepted authorization status in TransactionEventResponse for this transaction.</param>
        /// <param name="MaxEnergyOnInvalidId">Maximum amount of energy in Wh delivered when an identifier is deauthorized by the CSMS after start of a transaction.</param>
        /// <param name="StopTxOnEVSideDisconnect">When set to true, the Charging Station SHALL administratively stop the transaction when the cable is unplugged from the EV.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public TxCtrlr(TimeSpan                   EVConnectionTimeOut,
                       Boolean                    StopTxOnEVSideDisconnect,
                       IEnumerable<TxStartPoint>  TxStartPoint,
                       IEnumerable<TxStopPoint>   TxStopPoint,

                       Boolean?                   TxBeforeAcceptedEnabled   = null,
                       Boolean?                   StopTxOnInvalidId         = null,
                       WattHour?                  MaxEnergyOnInvalidId      = null,

                       String?                    Instance                  = null,
                       CustomData?                CustomData                = null)

            : base(nameof(TxCtrlr),
                   Instance,
                   I18NString.Create("Logical Component responsible for configuration relating to transaction characteristics and behaviour."),
                   CustomData)

        {

            this.EVConnectionTimeOut       = EVConnectionTimeOut;
            this.StopTxOnEVSideDisconnect  = StopTxOnEVSideDisconnect;
            this.TxStartPoint              = TxStartPoint;
            this.TxStopPoint               = TxStopPoint;

            this.TxBeforeAcceptedEnabled   = TxBeforeAcceptedEnabled;
            this.StopTxOnInvalidId         = StopTxOnInvalidId;
            this.MaxEnergyOnInvalidId      = MaxEnergyOnInvalidId;


            #region EVConnectionTimeOut

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "EVConnectionTimeOut",
                    ValueGetter:      () => ((UInt32) Math.Round(this.EVConnectionTimeOut.TotalSeconds)).ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          Unit:        UnitsOfMeasure.TimeSpan(),
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create(
                                          "Interval in seconds from between \"starting\" of a transaction until incipient transaction is automatically canceled, " +
                                          "due to failure of EV driver to (correctly) insert the charging cable connector(s) into the appropriate socket(s). " +
                                          "The Charging Station SHALL go back to the original state, probably: 'Available'. \"Starting\" might be the swiping of " +
                                          "the RFID, pressing a start button, a RequestStartTransactionRequest being received etc."
                                      )

                )
            );

            #endregion

            #region TxBeforeAcceptedEnabled

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TxBeforeAcceptedEnabled",
                    ValueGetter:      () => this.TxBeforeAcceptedEnabled.HasValue
                                                ? this.TxBeforeAcceptedEnabled.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create(
                                          "With this configuration variable the Charging Station can be configured to allow charging before " +
                                          "having received a BootNotificationResponse with RegistrationStatus: Accepted. " +
                                          "See: Transactions before being accepted by a CSMS."
                                      )

                )
            );

            #endregion

            #region TxStartPoint

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TxStartPoint",
                    ValueGetter:      () => this.TxStartPoint.Any()
                                                ? this.TxStartPoint.AggregateWith(',')
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.MemberList
                                      ),

                    Description:      I18NString.Create("Defines when the Charging Station starts a new transaction: first transactioneventRequest: eventType = Started. When any event in the given list occurs, the Charging Station SHALL start a transaction The Charging Station SHALL only send the Started event once for every transaction. It is advised to put all events that should be part of a transaction in the list, in case the start event never occurs. Because the possible events don’t always have to come in the same order it is possible to provide a list of events. Which ever comes first will then cause a transaction to be started. For example: EVConnected, Authorized would mean that a transaction is started when an EV is detected (Cable is connected), or when an EV Driver swipes his RFID card and the CSMS successfully authorizes the ID for charging.")

                )
            );

            #endregion

            #region TxStopPoint

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TxStopPoint",
                    ValueGetter:      () => this.TxStopPoint.Any()
                                                ? this.TxStopPoint.AggregateWith(',')
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.MemberList
                                      ),

                    Description:      I18NString.Create("Defines when the Charging Station ends a transaction: last transactioneventRequest: eventType = Ended. When any event in the given list is no longer valid, the Charging Station SHALL end the transaction. The Charging Station SHALL only send the Ended event once for every transaction. MaxEnergyOnInvalidId integer Maximum amount of energy in Wh delivered when an identifier is deauthorized by the CSMS after start of a transaction.")

                )
            );

            #endregion

            #region StopTxOnInvalidId

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "StopTxOnInvalidId",
                    ValueGetter:      () => this.StopTxOnInvalidId.HasValue
                                                ? this.StopTxOnInvalidId.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Whether the Charging Station will stop an ongoing transaction when it receives a non-accepted authorization status in TransactionEventResponse for this transaction.")

                )
            );

            #endregion

            #region StopTxOnEVSideDisconnect

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "StopTxOnEVSideDisconnect",
                    ValueGetter:      () => this.StopTxOnEVSideDisconnect
                                                ? "true"
                                                : "false",

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("When set to true, the Charging Station SHALL administratively stop the transaction when the cable is unplugged from the EV.")

                )
            );

            #endregion

            #region MaxEnergyOnInvalidId

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "MaxEnergyOnInvalidId",
                    ValueGetter:      () => this.MaxEnergyOnInvalidId.HasValue
                                                ? ((UInt32) Math.Round(this.MaxEnergyOnInvalidId.Value.Value)).ToString()
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          Unit:        UnitsOfMeasure.Wh(),
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Maximum amount of energy in Wh delivered when an identifier is deauthorized by the CSMS after start of a transaction.")

                )
            );

            #endregion


        }

        #endregion


    }

}
