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
    /// Logical Component responsible for configuration relating to
    /// the reporting of clock-aligned meter data.
    /// </summary>
    public class AlignedDataCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// Clock-aligned measurand(s) to be included in MeterValuesRequest, every AlignedDataInterval seconds.
        /// </summary>
        [Mandatory]
        public IEnumerable<Measurand>  Measurands           { get; set; }

        /// <summary>
        /// Size (in seconds) of the clock-aligned data interval, intended to be transmitted in the MeterValuesRequest message.
        /// This is the size (in seconds) of the set of evenly spaced aggregation intervals per day, starting at 00:00:00 (midnight).
        /// For example, a value of 900 (15 minutes) indicates that every day should be broken into 96 15-minute intervals.
        /// When clock aligned data is being transmitted, the interval in question is identified by the start time and
        /// (optional) duration interval value, represented according to the ISO8601 standard.
        /// All "per-period" data (e.g.energy readings) should be accumulated (for "flow" type measurands such as energy),
        /// or averaged (for other values) across the entire interval (or partial interval, at the beginning or end of a transaction),
        /// and transmitted(if so enabled) at the end of each interval, bearing the interval start time timestamp.
        /// A value of "0" (numeric zero), by convention, is to be interpreted to mean that no clock-aligned data should be transmitted.
        /// </summary>
        [Mandatory]
        public TimeSpan                Interval             { get; set; }

        /// <summary>
        /// Size (in seconds) of the clock-aligned data interval, intended to be transmitted in the TransactionEventRequest (eventType = Ended) message.
        /// This is the size (in seconds) of the set of evenly spaced aggregation intervals per day, starting at 00:00:00 (midnight).
        /// For example, a value of 900 (15 minutes) indicates that every day should be broken into 96 15-minute intervals.
        /// When clock aligned data is being collected, the interval in question is identified by the start time and (optional) duration interval value,
        /// represented according to the ISO8601 standard. All "per-period" data (e.g.energy readings) should be accumulated (for "flow" type measurands
        /// such as energy), or averaged(for other values) across the entire interval(or partial interval, at the beginning or end of a transaction),
        /// and transmitted(if so enabled) at the end of the transaction in 1 TransactionEventRequest(eventType = Ended) message.
        /// A value of "0" (numeric zero), by convention, is to be interpreted to mean that only the values taken at the start and end of a transaction
        /// should be transmitted (no intermediate values).
        /// </summary>
        [Mandatory]
        public TimeSpan                TxEndedInterval      { get; set; }

        /// <summary>
        /// Clock-aligned periodic measurand(s) to be included in the meterValues element of TransactionEventRequest (eventType = Ended) for every TxEndedAlignedDataInterval of the transaction.
        /// When left empty, no Clock-aligned measurands SHALL be put into the TransactionEventRequest (eventType = Ended).
        /// </summary>
        [Mandatory]
        public IEnumerable<Measurand>  TxEndedMeasurands    { get; set; }

        /// <summary>
        /// If this variable reports a value of true, Aligned Data is supported.
        /// </summary>
        [Optional]
        public Boolean?                Available            { get; set; }

        /// <summary>
        /// If this variable reports a value of true, Clock-Aligned Data is supported.
        /// </summary>
        [Optional]
        public Boolean?                Enabled              { get; set; }

        /// <summary>
        /// If set to true, the charging station SHALL not send clock aligned meter values when a transaction is ongoing.
        /// When an EVSE is specified, it SHALL stop sending the clock aligned meter values for this EVSE when it has an ongoing transaction.
        /// When no EVSE is specified, it SHALL stop sending the clock aligned meter values when any transaction is ongoing on this charging station.
        /// </summary>
        [Optional]
        public Boolean?                SendDuringIdle       { get; set; }

        /// <summary>
        /// If set to true, the charging station SHALL include signed meter values in the TransactionEventRequest to the CSMS.
        /// When a charging station does not support signed meter values it SHALL NOT report this variable.
        /// </summary>
        [Optional]
        public Boolean?                SignReadings         { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new aligned data controller.
        /// </summary>
        /// <param name="Measurands">Clock-aligned measurand(s) to be included in MeterValuesRequest, every AlignedDataInterval seconds.</param>
        /// <param name="Interval">Size (in seconds) of the clock-aligned data interval, intended to be transmitted in the MeterValuesRequest message.</param>
        /// <param name="TxEndedMeasurands">Clock-aligned periodic measurand(s) to be included in the meterValues element of TransactionEventRequest (eventType = Ended) for every TxEndedAlignedDataInterval of the transaction.</param>
        /// <param name="TxEndedInterval">Size (in seconds) of the clock-aligned data interval, intended to be transmitted in the TransactionEventRequest (eventType = Ended) message.</param>
        /// 
        /// <param name="Available">If this variable reports a value of true, Aligned Data is supported.</param>
        /// <param name="Enabled">If this variable reports a value of true, Clock-Aligned Data is supported.</param>
        /// <param name="SendDuringIdle">If set to true, the charging station SHALL not send clock aligned meter values when a transaction is ongoing.</param>
        /// <param name="SignReadings">If set to true, the charging station SHALL include signed meter values in the TransactionEventRequest to the CSMS.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public AlignedDataCtrlr(IEnumerable<Measurand>  Measurands,
                                TimeSpan                Interval,
                                IEnumerable<Measurand>  TxEndedMeasurands,
                                TimeSpan                TxEndedInterval,

                                Boolean?                Available        = null,
                                Boolean?                Enabled          = null,
                                Boolean?                SendDuringIdle   = null,
                                Boolean?                SignReadings     = null,

                                String?                 Instance         = null,
                                CustomData?             CustomData       = null)

            : base(nameof(AlignedDataCtrlr),
                   Instance,
                   I18NString.Create("Logical Component responsible for configuration relating to the reporting of clock-aligned meter data."),
                   CustomData)

        {

            this.Measurands         = Measurands.       Distinct();
            this.Interval           = Interval;
            this.TxEndedMeasurands  = TxEndedMeasurands.Distinct();
            this.TxEndedInterval    = TxEndedInterval;

            this.Available          = Available;
            this.Enabled            = Enabled;
            this.SendDuringIdle     = SendDuringIdle;
            this.SignReadings       = SignReadings;


            #region Measurands

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Measurands",
                    Instance:         null,
                    ValueGetter:      () => this.Measurands.Any()
                                                ? this.Measurands.AggregateWith(',')
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.MemberList,
                                          ValuesList:  Measurand.Values.Select(measurand => measurand.ToString()),
                                          MaxLimit:    999 // Defined by the implementer!
                                      ),

                    Description:      I18NString.Create(
                                          "Clock-aligned measurand(s) to be included in MeterValuesRequest, every AlignedDataInterval seconds."
                                      )

                )
            );

            #endregion

            #region Interval

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Interval",
                    Instance:         null,
                    ValueGetter:      () => ((UInt32) Math.Round(this.Interval.TotalSeconds)).ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          Unit:        UnitsOfMeasure.TimeSpan(),
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create(
                                          "Size (in seconds) of the clock-aligned data interval, intended to be transmitted in the MeterValuesRequest message." +
                                          "This is the size (in seconds) of the set of evenly spaced aggregation intervals per day, starting at 00:00:00 (midnight)." +
                                          "For example, a value of 900 (15 minutes) indicates that every day should be broken into 96 15-minute intervals." +
                                          "When clock aligned data is being transmitted, the interval in question is identified by the start time and" +
                                          "(optional) duration interval value, represented according to the ISO8601 standard." +
                                          "All \"per-period\" data (e.g.energy readings) should be accumulated (for \"flow\" type measurands such as energy)," +
                                          "or averaged (for other values) across the entire interval (or partial interval, at the beginning or end of a transaction)," +
                                          "and transmitted(if so enabled) at the end of each interval, bearing the interval start time timestamp." +
                                          "A value of \"0\" (numeric zero), by convention, is to be interpreted to mean that no clock-aligned data should be transmitted."
                                      )

                )
            );

            #endregion

            #region TxEndedMeasurands

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TxEndedMeasurands",
                    Instance:         null,
                    ValueGetter:      () => this.TxEndedMeasurands.Any()
                                                ? this.TxEndedMeasurands.AggregateWith(',')
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.MemberList,
                                          ValuesList:  Measurand.Values.Select(measurand => measurand.ToString()),
                                          MaxLimit:    999 // Defined by the implementer!
                                      ),

                    Description:      I18NString.Create(
                                          "Clock-aligned periodic measurand(s) to be included in the meterValues element of TransactionEventRequest (eventType = Ended) for every TxEndedAlignedDataInterval of the transaction. " +
                                          "When left empty, no Clock-aligned measurands SHALL be put into the TransactionEventRequest (eventType = Ended)."
                                      )

                )
            );

            #endregion

            #region TxEndedInterval

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TxEndedInterval",
                    Instance:         null,
                    ValueGetter:      () => ((UInt32) Math.Round(this.TxEndedInterval.TotalSeconds)).ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          Unit:        UnitsOfMeasure.TimeSpan(),
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create(
                                          "Size (in seconds) of the clock-aligned data interval, intended to be transmitted in the TransactionEventRequest (eventType = Ended) message. " +
                                          "This is the size (in seconds) of the set of evenly spaced aggregation intervals per day, starting at 00:00:00 (midnight). " +
                                          "For example, a value of 900 (15 minutes) indicates that every day should be broken into 96 15-minute intervals. " +
                                          "When clock aligned data is being collected, the interval in question is identified by the start time and (optional) duration interval value, " +
                                          "represented according to the ISO8601 standard. All \"per-period\" data (e.g.energy readings) should be accumulated (for \"flow\" type measurands " +
                                          "such as energy), or averaged(for other values) across the entire interval(or partial interval, at the beginning or end of a transaction), " +
                                          "and transmitted(if so enabled) at the end of the transaction in 1 TransactionEventRequest(eventType = Ended) message. " +
                                          "A value of \"0\" (numeric zero), by convention, is to be interpreted to mean that only the values taken at the start and end of a transaction " +
                                          "should be transmitted (no intermediate values)."
                                      )

                )
            );

            #endregion


            #region Available

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Available",
                    Instance:         null,
                    ValueGetter:      () => this.Available.HasValue
                                                ? this.Available.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("If this variable reports a value of true, Aligned Data is supported.")

                )
            );

            #endregion

            #region Enabled

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Enabled",
                    Instance:         null,
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

                    Description:      I18NString.Create("If this variable reports a value of true, Clock-Aligned Data is supported.")

                )
            );

            #endregion

            #region SendDuringIdle

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "SendDuringIdle",
                    Instance:         null,
                    ValueGetter:      () => this.SendDuringIdle.HasValue
                                                ? this.SendDuringIdle.Value
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
                                          "If set to true, the charging station SHALL not send clock aligned meter values when a transaction is ongoing. " +
                                          "When an EVSE is specified, it SHALL stop sending the clock aligned meter values for this EVSE when it has an ongoing transaction. " +
                                          "When no EVSE is specified, it SHALL stop sending the clock aligned meter values when any transaction is ongoing on this charging station."
                                      )

                )
            );

            #endregion

            #region SignReadings

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "SignReadings",
                    Instance:         null,
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

                    Description:      I18NString.Create(
                                          "If set to true, the charging station SHALL include signed meter values in the TransactionEventRequest to the CSMS. " +
                                          "When a charging station does not support signed meter values it SHALL NOT report this variable."
                                      )

                )
            );

            #endregion


        }

        #endregion


    }

}
