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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Logical Component responsible for configuration relating to the reporting of sampled meter data.
    /// </summary>
    public class SampledDataCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// If this variable reports a value of true, Sampled Data is enabled.
        /// </summary>
        public Boolean?                 Enabled                { get; set; }

        /// <summary>
        /// If this variable reports a value of true, Sampled Data is supported.
        /// </summary>
        public Boolean?                 Available              { get; set; }

        /// <summary>
        /// If set to true, the charging station includes signed meter values in the MeterValuesRequest to the CSMS.
        ///  When a charging station does not support signed meter values it SHALL NOT report this variable.
        /// </summary>
        public Boolean?                 SignReadings           { get; set; }

        /// <summary>
        /// Sampled measurand(s) to be taken at the start of any transaction to be included in the meterValues field of the first TransactionEventRequest message send at the start of a transaction (eventType = Started).
        ///  The charging station reports the list of supported Measurands in VariableCharacteristicsType.valuesList of this variable.
        ///  This way the CSMS knows which Measurands it can put in the SampledDataTxStartedMeasurands.
        ///  If the charging station has a meter, recommended to use as default: \"Energy.Active.Import.Register\"
        /// </summary>
        [Mandatory]
        public IEnumerable<Measurand>?  TxStartedMeasurands    { get; set; }

        /// <summary>
        /// Sampled measurands to be included in the meterValues element of every TransactionEventRequest (eventType = Updated), every SampledDataTxUpdatedInterval seconds from the start of the transaction.
        /// The charging station reports the list of supported Measurands in VariableCharacteristicsType.valuesList of this variable.
        /// This way the CSMS knows which Measurands it can put in the SampledDataTxUpdatedMeasurands.
        /// If the charging station has a meter, recommended to use as default: "Energy.Active.Import.Register"
        /// </summary>
        [Mandatory]
        public IEnumerable<Measurand>?  TxUpdatedMeasurands    { get; set; }

        /// <summary>
        /// Sampled measurands to be included in the meterValues element of TransactionEventRequest (eventType = Ended), every TxEndedSampleInterval seconds from the start of the transaction.
        /// The charging station reports the list of supported Measurands in VariableCharacteristicsType.valuesList of this variable.
        /// This way the CSMS knows which Measurands it can put in the TxEndedSampledData.
        /// When left empty, no sampled measurands SHALL be put into the TransactionEventRequest (eventType = Ended).
        /// </summary>
        [Mandatory]
        public IEnumerable<Measurand>?  TxEndedMeasurands      { get; set; }

        /// <summary>
        /// Interval in seconds between sampling of metering (or other) data, intended to be transmitted via TransactionEventRequest (eventType = Updated) messages.
        ///  For transaction data (evseId>0), samples are acquired and transmitted periodically at this interval from the start of the charging transaction.
        ///  A value of "0" (numeric zero), by convention, is to be interpreted to mean that no sampled data should be transmitted during the transaction.
        /// </summary>
        [Mandatory]
        public TimeSpan?                TxUpdatedInterval      { get; set; }

        /// <summary>
        /// Interval in seconds between sampling of metering (or other) data, intended to be transmitted in the TransactionEventRequest (eventType = Ended) message.
        /// For transaction data (evseId>0), samples are acquired and transmitted only in the TransactionEventRequest(eventType = Ended) message.
        /// A value of "0" (numeric zero), by convention, is to be interpreted to mean that only the values taken at the start and end of a transaction should be transmitted (no intermediate values).
        /// </summary>
        [Mandatory]
        public TimeSpan?                TxEndedInterval        { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new clock controller.
        /// </summary>
        /// <param name="Enabled">If this variable reports a value of true, Sampled Data is enabled.</param>
        /// <param name="Available">If this variable reports a value of true, Sampled Data is supported.</param>
        /// <param name="SignReadings">If set to true, the charging station includes signed meter values in the MeterValuesRequest to the CSMS.</param>
        /// <param name="TxEndedMeasurands">Sampled measurands to be included in the meterValues element of TransactionEventRequest (eventType = Ended), every TxEndedSampleInterval seconds from the start of the transaction.</param>
        /// <param name="TxEndedInterval">Interval in seconds between sampling of metering (or other) data, intended to be transmitted in the TransactionEventRequest (eventType = Ended) message.</param>
        /// <param name="TxStartedMeasurands">Sampled measurand(s) to be taken at the start of any transaction to be included in the meterValues field of the first TransactionEventRequest message send at the start of a transaction (eventType = Started).</param>
        /// <param name="TxUpdatedMeasurands">Sampled measurands to be included in the meterValues element of every TransactionEventRequest (eventType = Updated), every SampledDataTxUpdatedInterval seconds from the start of the transaction.</param>
        /// <param name="TxUpdatedInterval">Interval in seconds between sampling of metering (or other) data, intended to be transmitted via TransactionEventRequest (eventType = Updated) messages.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SampledDataCtrlr(IEnumerable<Measurand>  TxStartedMeasurands,
                                IEnumerable<Measurand>  TxUpdatedMeasurands,
                                IEnumerable<Measurand>  TxEndedMeasurands,
                                TimeSpan                TxUpdatedInterval,
                                TimeSpan                TxEndedInterval,

                                Boolean?                Enabled        = null,
                                Boolean?                Available      = null,
                                Boolean?                SignReadings   = null,

                                String?                 Instance       = null,
                                CustomData?             CustomData     = null)

            : base(nameof(SampledDataCtrlr),
                   Instance,
                   I18NString.Create("Logical Component responsible for configuration relating to the reporting of sampled meter data."),
                   CustomData)

        {

            this.Enabled              = Enabled;
            this.Available            = Available;
            this.SignReadings         = SignReadings;
            this.TxEndedMeasurands    = TxEndedMeasurands;
            this.TxEndedInterval      = TxEndedInterval;
            this.TxStartedMeasurands  = TxStartedMeasurands;
            this.TxUpdatedMeasurands  = TxUpdatedMeasurands;
            this.TxUpdatedInterval    = TxUpdatedInterval;


            #region Enabled

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Enabled",
                    ValueGetter:      () => this.Enabled.HasValue
                                                ? this.Enabled.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("If this variable reports a value of true, Sampled Data is enabled.")

                )
            );

            #endregion

            #region Available

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Available",
                    ValueGetter:      () => this.Available.HasValue
                                                ? this.Available.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("If this variable reports a value of true, Sampled Data is supported.")

                )
            );

            #endregion

            #region SignReadings

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "SignReadings",
                    ValueGetter:      () => this.SignReadings.HasValue
                                                ? this.SignReadings.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("If set to true, the charging station includes signed meter values in the MeterValuesRequest to the CSMS. When a charging station does not support signed meter values it SHALL NOT report this variable.")

                )
            );

            #endregion

            #region TxStartedMeasurands

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TxStartedMeasurands",
                    ValueGetter:      () => this.TxStartedMeasurands.Any()
                                                ? this.TxStartedMeasurands.AggregateWith(',')
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.MemberList,
                                          MaxLimit:    999 // Defined by the implementer!
                                      ),

                    Description:      I18NString.Create(
                                          "Sampled measurand(s) to be taken at the start of any transaction to be included in the " +
                                          "meterValues field of the first TransactionEventRequest message send at the start of a " +
                                          "transaction (eventType = Started). The charging station reports the list of supported " +
                                          "Measurands in VariableCharacteristicsType.valuesList of this variable. " +
                                          "This way the CSMS knows which Measurands it can put in the SampledDataTxStartedMeasurands. " +
                                          "If the charging station has a meter, recommended to use as default: \"Energy.Active.Import.Register\""
                                      )

                )
            );

            #endregion

            #region TxUpdatedMeasurands

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TxUpdatedMeasurands",
                    ValueGetter:      () => this.TxUpdatedMeasurands.Any()
                                                ? this.TxUpdatedMeasurands.AggregateWith(',')
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.MemberList,
                                          MaxLimit:    999 // Defined by the implementer!
                                      ),

                    Description:      I18NString.Create(
                                          "Sampled measurands to be included in the meterValues element of every TransactionEventRequest " +
                                          "(eventType = Updated), every SampledDataTxUpdatedInterval seconds from the start of the transaction. " +
                                          "The charging station reports the list of supported Measurands in VariableCharacteristicsType.valuesList " +
                                          "of this variable. This way the CSMS knows which Measurands it can put in the SampledDataTxUpdatedMeasurands. " +
                                          "If the charging station has a meter, recommended to use as default: \"Energy.Active.Import.Register\""
                                      )

                )
            );

            #endregion

            #region TxEndedMeasurands

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TxEndedMeasurands",
                    ValueGetter:      () => this.TxEndedMeasurands.Any()
                                                ? this.TxEndedMeasurands.AggregateWith(',')
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.MemberList,
                                          MaxLimit:    999 // Defined by the implementer!
                                      ),

                    Description:      I18NString.Create("Sampled measurands to be included in the meterValues element of TransactionEventRequest (eventType = Ended), every TxEndedSampleInterval seconds from the start of the transaction.")

                )
            );

            #endregion

            #region TxEndedInterval

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TxEndedInterval",
                    ValueGetter:      () => this.TxEndedInterval.HasValue
                                                ? ((UInt32) Math.Round(this.TxEndedInterval.Value.TotalSeconds)).ToString()
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          Unit:        UnitsOfMeasure.TimeSpan(),
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create(
                                          "Interval in seconds between sampling of metering (or other) data, intended to be transmitted " +
                                          "in the TransactionEventRequest (eventType = Ended) message. For transaction data (evseId>0), " +
                                          "samples are acquired and transmitted only in the TransactionEventRequest (eventType = Ended) message. " +
                                          "A value of \"0\" (numeric zero), by convention, is to be interpreted to mean that only the values " +
                                          "taken at the start and end of a transaction should be transmitted (no intermediate values)."
                                      )

                )
            );

            #endregion

            #region TxUpdatedInterval

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TxUpdatedInterval",
                    ValueGetter:      () => this.TxUpdatedInterval.HasValue
                                                ? ((UInt32) Math.Round(this.TxUpdatedInterval.Value.TotalSeconds)).ToString()
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          Unit:        UnitsOfMeasure.TimeSpan(),
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create(
                                          "Interval in seconds between sampling of metering (or other) data, intended to be transmitted via " +
                                          "TransactionEventRequest (eventType = Updated) messages. For transaction data (evseId>0), samples " +
                                          "are acquired and transmitted periodically at this interval from the start of the charging transaction. " +
                                          "A value of \"0\" (numeric zero), by convention, is to be interpreted to mean that no sampled data should " +
                                          "be\r\ntransmitted during the transaction."
                                      )

                )
            );

            #endregion


        }

        #endregion


    }

}
