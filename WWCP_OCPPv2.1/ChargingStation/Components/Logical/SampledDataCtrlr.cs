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
        /// If set to true, the Charging Station includes signed meter values in the MeterValuesRequest to the CSMS.
        /// </summary>
        public Boolean?                 SignReadings           { get; set; }

        /// <summary>
        /// Sampled measurands to be included in the meterValues element of TransactionEventRequest (eventType = Ended), every TxEndedSampleInterval seconds from the start of the transaction.
        /// </summary>
        public IEnumerable<Measurand>?  TxEndedMeasurands      { get; set; }

        /// <summary>
        /// Interval in seconds between sampling of metering (or other) data, intended to be transmitted in the TransactionEventRequest (eventType = Ended) message.
        /// </summary>
        public TimeSpan?                TxEndedInterval        { get; set; }

        /// <summary>
        /// Sampled measurand(s) to be taken at the start of any transaction to be included in the meterValues field of the first TransactionEventRequest message send at the start of a transaction (eventType = Started).
        /// </summary>
        public IEnumerable<Measurand>?  TxStartedMeasurands    { get; set; }

        /// <summary>
        /// Sampled measurands to be included in the meterValues element of every TransactionEventRequest (eventType = Updated), every SampledDataTxUpdatedInterval seconds from the start of the transaction.
        /// </summary>
        public IEnumerable<Measurand>?  TxUpdatedMeasurands    { get; set; }

        /// <summary>
        /// Interval in seconds between sampling of metering (or other) data, intended to be transmitted via TransactionEventRequest (eventType = Updated) messages.
        /// </summary>
        public TimeSpan?                TxUpdatedInterval      { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new clock controller.
        /// </summary>
        /// <param name="Enabled">If this variable reports a value of true, Sampled Data is enabled.</param>
        /// <param name="Available">If this variable reports a value of true, Sampled Data is supported.</param>
        /// <param name="SignReadings">If set to true, the Charging Station includes signed meter values in the MeterValuesRequest to the CSMS.</param>
        /// <param name="TxEndedMeasurands">Sampled measurands to be included in the meterValues element of TransactionEventRequest (eventType = Ended), every TxEndedSampleInterval seconds from the start of the transaction.</param>
        /// <param name="TxEndedInterval">Interval in seconds between sampling of metering (or other) data, intended to be transmitted in the TransactionEventRequest (eventType = Ended) message.</param>
        /// <param name="TxStartedMeasurands">Sampled measurand(s) to be taken at the start of any transaction to be included in the meterValues field of the first TransactionEventRequest message send at the start of a transaction (eventType = Started).</param>
        /// <param name="TxUpdatedMeasurands">Sampled measurands to be included in the meterValues element of every TransactionEventRequest (eventType = Updated), every SampledDataTxUpdatedInterval seconds from the start of the transaction.</param>
        /// <param name="TxUpdatedInterval">Interval in seconds between sampling of metering (or other) data, intended to be transmitted via TransactionEventRequest (eventType = Updated) messages.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SampledDataCtrlr(Boolean?                 Enabled               = null,
                                Boolean?                 Available             = null,
                                Boolean?                 SignReadings          = null,
                                IEnumerable<Measurand>?  TxEndedMeasurands     = null,
                                TimeSpan?                TxEndedInterval       = null,
                                IEnumerable<Measurand>?  TxStartedMeasurands   = null,
                                IEnumerable<Measurand>?  TxUpdatedMeasurands   = null,
                                TimeSpan?                TxUpdatedInterval     = null,

                                String?                  Instance              = null,
                                CustomData?              CustomData            = null)

            : base(nameof(SampledDataCtrlr),
                   Instance,
                   new[] {

                       #region Enabled

                       new VariableConfig(

                           Name:              "Enabled",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Boolean
                                                   )
                                               },

                           Description:       I18NString.Create("If this variable reports a value of true, Sampled Data is enabled."),

                           CustomData:        null

                       ),

                       #endregion

                       #region Available

                       new VariableConfig(

                           Name:              "Available",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Boolean
                                                   )
                                               },

                           Description:       I18NString.Create("If this variable reports a value of true, Sampled Data is supported."),

                           CustomData:        null

                       ),

                       #endregion

                       #region SignReadings

                       new VariableConfig(

                           Name:              "SignReadings",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Boolean
                                                   )
                                               },

                           Description:       I18NString.Create("If set to true, the charging station includes signed meter values in the MeterValuesRequest to the CSMS."),

                           CustomData:        null

                       ),

                       #endregion

                       #region TxEndedMeasurands

                       new VariableConfig(

                           Name:              "TxEndedMeasurands",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.MemberList
                                                   )
                                               },

                           Description:       I18NString.Create("Sampled measurands to be included in the meterValues element of TransactionEventRequest (eventType = Ended), every TxEndedSampleInterval seconds from the start of the transaction."),

                           CustomData:        null

                       ),

                       #endregion

                       #region TxEndedInterval

                       new VariableConfig(

                           Name:              "TxEndedInterval",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("Interval in seconds between sampling of metering (or other) data, intended to be transmitted in the TransactionEventRequest (eventType = Ended) message."),

                           CustomData:        null

                       ),

                       #endregion

                       #region TxStartedMeasurands

                       new VariableConfig(

                           Name:              "TxStartedMeasurands",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.MemberList
                                                   )
                                               },

                           Description:       I18NString.Create("Sampled measurand(s) to be taken at the start of any transaction to be included in the meterValues field of the first TransactionEventRequest message send at the start of a transaction (eventType = Started)."),

                           CustomData:        null

                       ),

                       #endregion

                       #region TxUpdatedMeasurands

                       new VariableConfig(

                           Name:              "TxUpdatedMeasurands",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.MemberList
                                                   )
                                               },

                           Description:       I18NString.Create("Sampled measurands to be included in the meterValues element of every TransactionEventRequest (eventType = Updated), every SampledDataTxUpdatedInterval seconds from the start of the transaction."),

                           CustomData:        null

                       ),

                       #endregion

                       #region TxUpdatedInterval

                       new VariableConfig(

                           Name:              "TxUpdatedInterval",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("Interval in seconds between sampling of metering (or other) data, intended to be transmitted via TransactionEventRequest (eventType = Updated) messages."),

                           CustomData:        null

                       ),

                       #endregion

                   },
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

        }

        #endregion


    }

}
