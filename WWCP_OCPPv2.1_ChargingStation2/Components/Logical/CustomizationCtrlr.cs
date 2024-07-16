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
    /// Logical Component responsible for configuration relating to custom vendor-specific implementations,
    /// using the DataTransfer message and CustomData extensions.
    /// </summary>
    public class CustomizationCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// This standard configuration variable can be used to enable/disable custom
        /// implementations that the Charging Station supports.The instance name of the
        /// variable matches the vendorId of the customization in CustomData or
        /// DataTransfer messages.
        /// </summary>
        public Boolean?  CustomImplementationEnabled    { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new customization controller.
        /// </summary>
        /// <param name="CustomImplementationEnabled">This standard configuration variable can be used to enable/disable custom implementations that the Charging Station supports.The instance name of the variable matches the vendorId of the customization in CustomData or DataTransfer messages.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public CustomizationCtrlr(Boolean?     CustomImplementationEnabled   = null,

                                  String?      Instance                      = null,
                                  CustomData?  CustomData                    = null)

            : base(nameof(CustomizationCtrlr),
                   Instance,
                   I18NString.Create("Logical Component responsible for configuration relating to custom vendor-specific implementations, using the DataTransfer message and CustomData extensions."),
                   CustomData)

        {

            this.CustomImplementationEnabled = CustomImplementationEnabled;


            #region Enabled

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Enabled",
                    ValueGetter:      () => this.CustomImplementationEnabled.HasValue
                                                ? this.CustomImplementationEnabled.Value
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
                                          "This standard configuration variable can be used to enable/disable custom implementations that " +
                                          "the charging station supports. The instance name of the variable matches the vendorId of the " +
                                          "customization in CustomData or DataTransfer messages."
                                      )

                )
            );

            #endregion

        }

        #endregion


    }

}
