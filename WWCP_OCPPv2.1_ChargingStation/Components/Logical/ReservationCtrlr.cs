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
    /// Logical Component responsible for configuration relating to reservations.
    /// </summary>
    public class ReservationCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// Whether reservation is enabled.
        /// </summary>
        public Boolean?  Enabled            { get; set; }

        /// <summary>
        /// Whether reservation is supported.
        /// </summary>
        public Boolean?  Available          { get; set; }

        /// <summary>
        /// When this configuration variable is present and set to true:
        /// The charging station supports reservation without a specified EVSE Id.
        /// </summary>
        public Boolean?  NonEvseSpecific    { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new reservation controller.
        /// </summary>
        /// <param name="Enabled">Whether reservation is enabled.</param>
        /// <param name="Available">Whether reservation is supported.</param>
        /// <param name="NonEvseSpecific">When this configuration variable is present and set to true: The charging station supports reservation without a specified EVSE Id.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public ReservationCtrlr(Boolean?     Enabled,
                                Boolean?     Available,
                                Boolean?     NonEvseSpecific,

                                String?      Instance     = null,
                                CustomData?  CustomData   = null)

            : base(nameof(ReservationCtrlr),
                   Instance,
                   I18NString.Create("Logical Component responsible for configuration relating to reservations."),
                   CustomData)

        {

            this.Enabled          = Enabled;
            this.Available        = Available;
            this.NonEvseSpecific  = NonEvseSpecific;


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

                    Description:      I18NString.Create("Whether reservation is enabled.")

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

                    Description:      I18NString.Create("Whether reservation is supported.")

                )
            );

            #endregion

            #region NonEvseSpecific

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "NonEvseSpecific",
                    ValueGetter:      () => this.NonEvseSpecific.HasValue
                                                ? this.NonEvseSpecific.Value
                                                      ? "true"
                                                      : "false"
                                                : null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create(
                                          "When this configuration variable is present and set to true: " +
                                          "The charging station supports reservation without a specified EVSE Id."
                                      )

                )
            );

            #endregion


        }

        #endregion


    }

}
