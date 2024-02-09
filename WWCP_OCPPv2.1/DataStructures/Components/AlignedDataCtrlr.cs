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
    /// HTTP Basic Authentication security settings between the charging station and the CSMS.
    /// </summary>
    public class AlignedDataCtrlr : ComponentConfig
    {

        #region Properties

        /// <summary>
        /// If this variable reports a value of true, Clock-Aligned Data is supported.
        /// </summary>
        public Boolean       Enabled              { get; set; }

        /// <summary>
        /// If this variable reports a value of true, Aligned Data is supported.
        /// </summary>
        public Boolean       Available            { get; set; }

        /// <summary>
        /// Size (in seconds) of the clock-aligned data interval, intended to be transmitted in the MeterValuesRequest message.
        /// </summary>
        public UInt32        Interval             { get; set; }

        /// <summary>
        /// Clock-aligned measurand(s) to be included in MeterValuesRequest, every AlignedDataInterval seconds.
        /// </summary>
        public List<Object>  Measurands           { get; set; }

        /// <summary>
        /// If set to true, the Charging Station SHALL not send clock aligned meter values when a transaction is ongoing.
        /// </summary>
        public Boolean       SendDuringIdle       { get; set; }

        /// <summary>
        /// If set to true, the Charging Station SHALL include signed meter values in the TransactionEventRequest to the CSMS.
        /// </summary>
        public Boolean       SignReadings         { get; set; }

        /// <summary>
        /// Size (in seconds) of the clock-aligned data interval, intended to be transmitted in the TransactionEventRequest (eventType = Ended) message.
        /// </summary>
        public UInt32        TxEndedInterval      { get; set; }

        /// <summary>
        /// Clock-aligned periodic measurand(s) to be included in the meterValues element of TransactionEventRequest (eventType = Ended) for every TxEndedAlignedDataInterval of the transaction.
        /// </summary>
        public List<Object>  TxEndedMeasurands    { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new constant stream data.
        /// </summary>
        /// <param name="Identity">The charging station identity.</param>
        /// <param name="BasicAuthPassword">The HTTP Basic Authentication password</param>
        /// <param name="OrganizationName">The organization name that is to be used for checking a security certificate.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public AlignedDataCtrlr(Boolean       Enabled,
                                Boolean       Available,
                                UInt32        Interval,
                                List<Object>  Measurands,
                                Boolean       SendDuringIdle,
                                Boolean       SignReadings,
                                UInt32        TxEndedInterval,
                                List<Object>  TxEndedMeasurands,

                                String?       Instance     = null,
                                CustomData?   CustomData   = null)

            : base(nameof(AlignedDataCtrlr),
                   Instance,
                   null,
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

                           Description:       I18NString.Create("If this variable reports a value of true, Clock-Aligned Data is supported."),

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

                           Description:       I18NString.Create("If this variable reports a value of true, Aligned Data is supported."),

                           CustomData:        null

                       ),

                       #endregion

                       #region Interval

                       new VariableConfig(

                           Name:              "Interval",
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

                           Description:       I18NString.Create("Size (in seconds) of the clock-aligned data interval, intended to be transmitted in the MeterValuesRequest message."),

                           CustomData:        null

                       ),

                       #endregion

                       #region Measurands

                       new VariableConfig(

                           Name:              "Measurands",
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

                           Description:       I18NString.Create("Clock-aligned measurand(s) to be included in MeterValuesRequest, every AlignedDataInterval seconds."),

                           CustomData:        null

                       ),

                       #endregion

                       #region SendDuringIdle

                       new VariableConfig(

                           Name:              "SendDuringIdle",
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

                           Description:       I18NString.Create("If set to true, the Charging Station SHALL not send clock aligned meter values when a transaction is ongoing."),

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

                           Description:       I18NString.Create("If set to true, the Charging Station SHALL include signed meter values in the TransactionEventRequest to the CSMS."),

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

                           Description:       I18NString.Create("Size (in seconds) of the clock-aligned data interval, intended to be transmitted in the TransactionEventRequest (eventType = Ended) message."),

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

                           Description:       I18NString.Create("Clock-aligned periodic measurand(s) to be included in the meterValues element of TransactionEventRequest (eventType = Ended) for every TxEndedAlignedDataInterval of the transaction."),

                           CustomData:        null

                       ),

                       #endregion

                   },
                   I18NString.Create("Logical Component responsible for configuration relating to the reporting of clock-aligned meter data."),
                   CustomData)

        {

            this.Enabled            = Enabled;
            this.Available          = Available;
            this.Interval           = Interval;
            this.Measurands         = Measurands;
            this.SendDuringIdle     = SendDuringIdle;
            this.SignReadings       = SignReadings;
            this.TxEndedInterval    = TxEndedInterval;
            this.TxEndedMeasurands  = TxEndedMeasurands;

        }

        #endregion


    }

}
