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
    /// An abstract physical component configuration.
    /// </summary>
    public abstract class APhysicalComponentConfig : ComponentConfig
    {

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract logical component configuration.
        /// </summary>
        /// <param name="Name">The case insensitive name of the component. Name should be taken from the list of standardized component names whenever possible.</param>
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="EVSE">An optional EVSE when component is located at EVSE level, also specifies the connector when component is located at connector level.</param>
        /// 
        /// <param name="VariableConfigs">An optional enumeration of variable configurations.</param>
        /// <param name="Description">An optional multi-language description of the logical component.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public APhysicalComponentConfig(String                        Name,
                                        String?                       Instance          = null,
                                        EVSE?                         EVSE              = null,

                                        IEnumerable<VariableConfig>?  VariableConfigs   = null,
                                        I18NString?                   Description       = null,

                                        CustomData?                   CustomData        = null)

            : base(Name,
                   Instance,
                   EVSE,
                   VariableConfigs,
                   Description,
                   CustomData)

        { }


        /// <summary>
        /// Create a new abstract logical component configuration.
        /// </summary>
        /// <param name="Name">The case insensitive name of the component. Name should be taken from the list of standardized component names whenever possible.</param>
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="EVSE">An optional EVSE when component is located at EVSE level, also specifies the connector when component is located at connector level.</param>
        /// 
        /// <param name="Description">An optional multi-language description of the logical component.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public APhysicalComponentConfig(String       Name,
                                        String?      Instance          = null,
                                        I18NString?  Description       = null,
                                        EVSE?        EVSE              = null,

                                        CustomData?  CustomData        = null)

            : base(Name,
                   Instance,
                   EVSE,
                   null,
                   Description,
                   CustomData)

        { }

        #endregion

    }

}
