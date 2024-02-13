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
        public Boolean?  Enabled                     { get; set; }

        /// <summary>
        /// Whether Tariff/cost is supported.
        /// </summary>
        public Boolean?  Available                   { get; set; }

        /// <summary>
        /// Message (and/or tariff information) to be shown to an EV Driver when there is no driver specific tariff information available.
        /// </summary>
        public String?   TariffFallbackMessage       { get; set; }

        /// <summary>
        /// Message to be shown to an EV Driver when the Charging Station cannot retrieve the cost for a transaction at the end of the transaction. Currency string Currency used by this Charging Station in a ISO 4217 formatted currency code.
        /// </summary>
        public String?   TotalCostFallbackMessage    { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new tariff cost controller.
        /// </summary>
        /// <param name="Enabled">Whether Tariff/cost is enabled.</param>
        /// <param name="Available">Whether Tariff/cost is supported.</param>
        /// <param name="TariffFallbackMessage">Message (and/or tariff information) to be shown to an EV Driver when there is no driver specific tariff information available.</param>
        /// <param name="TotalCostFallbackMessage">Message to be shown to an EV Driver when the Charging Station cannot retrieve the cost for a transaction at the end of the transaction. Currency string Currency used by this Charging Station in a ISO 4217 formatted currency code.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public TariffCostCtrlr(Boolean?      Enabled,
                               Boolean?      Available,
                               String?       TariffFallbackMessage,
                               String?       TotalCostFallbackMessage,

                               String?       Instance     = null,
                               CustomData?   CustomData   = null)

            : base(nameof(TariffCostCtrlr),
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

                           Description:       I18NString.Create("Whether Tariff/cost is enabled."),

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

                           Description:       I18NString.Create("Whether Tariff/cost is supported."),

                           CustomData:        null

                       ),

                       #endregion

                       #region TariffFallbackMessage

                       new VariableConfig(

                           Name:              "TariffFallbackMessage",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.String
                                                   )
                                               },

                           Description:       I18NString.Create("Message (and/or tariff information) to be shown to an EV Driver when there is no driver specific tariff information available."),

                           CustomData:        null

                       ),

                       #endregion

                       #region TotalCostFallbackMessage

                       new VariableConfig(

                           Name:              "TotalCostFallbackMessage",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.String
                                                   )
                                               },

                           Description:       I18NString.Create("Message to be shown to an EV Driver when the Charging Station cannot retrieve the cost for a transaction at the end of the transaction. Currency string Currency used by this Charging Station in a ISO 4217 formatted currency code."),

                           CustomData:        null

                       ),

                       #endregion


                   },
                   I18NString.Create("Logical Component responsible for configuration relating to tariff and cost display."),
                   CustomData)

        {

            this.Enabled                   = Enabled;
            this.Available                 = Available;
            this.TariffFallbackMessage     = TariffFallbackMessage;
            this.TotalCostFallbackMessage  = TotalCostFallbackMessage;

        }

        #endregion


    }

}
