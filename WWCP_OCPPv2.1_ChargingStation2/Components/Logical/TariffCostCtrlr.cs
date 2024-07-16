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
    /// Logical Component responsible for configuration relating to tariff and cost display.
    /// </summary>
    public class TariffCostCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// Whether Tariff/cost is enabled.
        /// </summary>
        public Boolean?    Enabled                     { get; set; }

        /// <summary>
        /// Whether Tariff/cost is supported.
        /// </summary>
        public Boolean?    Available                   { get; set; }

        /// <summary>
        /// Whether Ccost is enabled.
        /// </summary>
        public Boolean?    CostEnabled                 { get; set; }

        /// <summary>
        /// Whether Cost is supported.
        /// </summary>
        public Boolean?    CostAvailable               { get; set; }

        /// <summary>
        /// Message (and/or tariff information) to be shown to an EV Driver when there is no driver specific tariff information available.
        /// </summary>
        public I18NString  TariffFallbackMessage       { get; set; }

        /// <summary>
        /// Message to be shown to an EV Driver when the charging station cannot retrieve the cost for a transaction at the end of the transaction.
        /// </summary>
        public I18NString  TotalCostFallbackMessage    { get; set; }

        /// <summary>
        /// Currency used by this charging station in a ISO 4217 [ISO4217] formatted currency code.
        /// </summary>
        public Currency    Currency                    { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new tariff cost controller.
        /// </summary>
        /// <param name="TariffFallbackMessage">Message (and/or tariff information) to be shown to an EV Driver when there is no driver specific tariff information available.</param>
        /// <param name="TotalCostFallbackMessage">Message to be shown to an EV Driver when the charging station cannot retrieve the cost for a transaction at the end of the transaction.</param>
        /// <param name="Currency">Currency used by this charging station in a ISO 4217 [ISO4217] formatted currency code.</param>
        /// 
        /// <param name="Enabled">Whether Tariff/cost is enabled.</param>
        /// <param name="Available">Whether Tariff/cost is supported.</param>
        /// <param name="CostEnabled">Whether Tariff/cost is enabled.</param>
        /// <param name="CostAvailable">Whether Tariff/cost is supported.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public TariffCostCtrlr(I18NString    TariffFallbackMessage,
                               I18NString    TotalCostFallbackMessage,
                               Currency      Currency,

                               Boolean?      Enabled,
                               Boolean?      Available,
                               Boolean?      CostEnabled,
                               Boolean?      CostAvailable,

                               String?       Instance     = null,
                               CustomData?   CustomData   = null)

            : base(nameof(TariffCostCtrlr),
                   Instance,
                   I18NString.Create("Logical Component responsible for configuration relating to tariff and cost display."),
                   CustomData)

        {

            this.TariffFallbackMessage     = TariffFallbackMessage;
            this.TotalCostFallbackMessage  = TotalCostFallbackMessage;
            this.Currency                  = Currency;

            this.Enabled                   = Enabled;
            this.Available                 = Available;
            this.CostEnabled               = CostEnabled;
            this.CostAvailable             = CostAvailable;


            #region TariffFallbackMessage

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TariffFallbackMessage",
                    ValueGetter:      () => this.TariffFallbackMessage.FirstText(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String
                                      ),

                    Description:      I18NString.Create("Message (and/or tariff information) to be shown to an EV driver when there is no driver specific tariff information available.")

                )
            );

            #endregion

            #region TotalCostFallbackMessage

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TotalCostFallbackMessage",
                    ValueGetter:      () => this.TotalCostFallbackMessage.FirstText(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String
                                      ),

                    Description:      I18NString.Create("Message to be shown to an EV Driver when the charging station cannot retrieve the cost for a transaction at the end of the transaction.")

                )
            );

            #endregion

            #region Currency

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Currency",
                    ValueGetter:      () => this.Currency.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String,
                                          MaxLimit:    3
                                      ),

                    Description:      I18NString.Create("Currency used by this charging station in a ISO 4217 [ISO4217] formatted currency code.")

                )
            );

            #endregion

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

                    Description:      I18NString.Create("Whether Tariff/cost is enabled.")

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
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Whether Tariff/cost is supported.")

                )
            );

            #endregion

            #region Enabled   (Cost)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Enabled",
                    Instance:         "Cost",
                    ValueGetter:      () => this.CostEnabled.HasValue
                                                ? this.CostEnabled.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Whether Cost is enabled.")

                )
            );

            #endregion

            #region Available (Cost)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Available",
                    Instance:         "Cost",
                    ValueGetter:      () => this.CostAvailable.HasValue
                                                ? this.CostAvailable.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Whether Cost is supported.")

                )
            );

            #endregion


        }

        #endregion


    }

}
