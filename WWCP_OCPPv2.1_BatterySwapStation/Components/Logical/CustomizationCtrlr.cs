/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

namespace cloud.charging.open.protocols.OCPPv2_1.BSS
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
        public Boolean?             CustomImplementationEnabled    { get; set; }


        /// <summary>
        /// This variable defines the names of custom triggers that a battery swap station
        /// supports in a customTrigger field of TriggerMessageRequest.
        /// </summary>
        public IEnumerable<String>  CustomTriggers                 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new customization controller.
        /// </summary>
        /// <param name="CustomImplementationEnabled">This standard configuration variable can be used to enable/disable custom implementations that the Charging Station supports.The instance name of the variable matches the vendorId of the customization in CustomData or DataTransfer messages.</param>
        /// <param name="CustomTriggers">This variable defines the names of custom triggers that a battery swap station supports in a customTrigger field of TriggerMessageRequest.</param>
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public CustomizationCtrlr(Boolean?              CustomImplementationEnabled   = null,
                                  IEnumerable<String>?  CustomTriggers                = null,

                                  String?               Instance                      = null,
                                  CustomData?           CustomData                    = null)

            : base(nameof(CustomizationCtrlr),
                   Instance,
                   I18NString.Create("Logical Component responsible for configuration relating to custom vendor-specific implementations, using the DataTransfer message and CustomData extensions."),
                   CustomData)

        {

            this.CustomImplementationEnabled  = CustomImplementationEnabled;
            this.CustomTriggers               = CustomTriggers ?? [];

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
                                          "the battery swap station supports. The instance name of the variable matches the vendorId of the " +
                                          "customization in CustomData or DataTransfer messages."
                                      )

                )
            );

            #endregion

            #region CustomTriggers

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "CustomTriggers",
                    ValueGetter:      () => this.CustomTriggers.AggregateWith(", "),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.MemberList
                                      ),

                    Description:      I18NString.Create(
                                          "This variable defines the names of custom triggers that a battery swap station supports" +
                                          " in a customTrigger field of TriggerMessageRequest."
                                      )

                )
            );

            #endregion

        }

        #endregion




    }

}
