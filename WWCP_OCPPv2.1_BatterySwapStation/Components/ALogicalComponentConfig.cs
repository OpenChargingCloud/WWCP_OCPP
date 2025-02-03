﻿/*
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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An abstract logical component configuration.
    /// </summary>
    public abstract class ALogicalComponentConfig : ComponentConfig
    {

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract logical component configuration.
        /// </summary>
        /// <param name="Name">The case insensitive name of the component. Name should be taken from the list of standardized component names whenever possible.</param>
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// 
        /// <param name="VariableConfigs">An optional enumeration of variable configurations.</param>
        /// <param name="Description">An optional multi-language description of the logical component.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public ALogicalComponentConfig(String                        Name,
                                       String?                       Instance          = null,

                                       IEnumerable<VariableConfig>?  VariableConfigs   = null,
                                       I18NString?                   Description       = null,

                                       CustomData?                   CustomData        = null)

            : base(Name,
                   Instance,
                   null,
                   VariableConfigs,
                   Description,
                   CustomData)

        { }


        /// <summary>
        /// Create a new abstract logical component configuration.
        /// </summary>
        /// <param name="Name">The case insensitive name of the component. Name should be taken from the list of standardized component names whenever possible.</param>
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// 
        /// <param name="Description">An optional multi-language description of the logical component.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public ALogicalComponentConfig(String       Name,
                                       String?      Instance          = null,
                                       I18NString?  Description       = null,

                                       CustomData?  CustomData        = null)

            : base(Name,
                   Instance,
                   null,
                   null,
                   Description,
                   CustomData)

        { }

        #endregion

    }

}
