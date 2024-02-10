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
    /// Logical Component responsible for configuration relating to the use of a local cache for authorization for Charging Station use.
    /// </summary>
    public class AuthCacheCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// If this variable exists, the Charging Station supports an Authorization Cache.
        /// </summary>
        public Boolean?                      Enabled                 { get; set; }

        /// <summary>
        /// If this variable reports a value of true, Authorization Cache is supported.
        /// </summary>
        public Boolean?                      Available               { get; set; }

        /// <summary>
        /// Indicates in seconds how long it takes until a token expires in the authorization cache since it is last used.
        /// </summary>
        public UInt32?                       LifeTime                { get; set; }

        /// <summary>
        /// Cache Entry Replacement Policy: (LRU,LFU) LeastRecentlyUsed or LeastFrequentlyUsed. Allowed values: LRU, LFU.
        /// </summary>
        public CacheEntryReplacementPolicy?  Policy                  { get; set; }

        /// <summary>
        /// When set to true this variable disables the behavior to request authorization for an idToken that is stored in the cache with a status other than Accepted, as stated in C10.FR.03 and C12.FR.05.
        /// </summary>
        public Boolean?                      DisablePostAuthorize    { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new authorization cache controller.
        /// </summary>
        /// 
        /// <param name="Enabled">If this variable exists, the Charging Station supports an authorization cache.</param>
        /// <param name="Available">If this variable reports a value of true, authorization cache is supported.</param>
        /// <param name="LifeTime">Indicates in seconds how long it takes until a token expires in the authorization cache since it is last used.</param>
        /// <param name="Policy">Cache Entry Replacement Policy: (LRU,LFU) LeastRecentlyUsed or LeastFrequentlyUsed. Allowed values: LRU, LFU.</param>
        /// <param name="DisablePostAuthorize">When set to true this variable disables the behavior to request authorization for an idToken that is stored in the cache with a status other than Accepted, as stated in C10.FR.03 and C12.FR.05.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public AuthCacheCtrlr(Boolean?                      Enabled                = null,
                              Boolean?                      Available              = null,
                              UInt32?                       LifeTime               = null,
                              CacheEntryReplacementPolicy?  Policy                 = null,
                              Boolean?                      DisablePostAuthorize   = null,

                              String?                       Instance               = null,
                              CustomData?                   CustomData             = null)

            : base(nameof(AuthCacheCtrlr),
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

                           Description:       I18NString.Create("If this variable exists, the Charging Station supports an Authorization Cache."),

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

                           Description:       I18NString.Create("If this variable reports a value of true, Authorization Cache is supported."),

                           CustomData:        null

                       ),

                       #endregion

                       #region LifeTime

                       new VariableConfig(

                           Name:              "LifeTime",
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

                           Description:       I18NString.Create("Indicates in seconds how long it takes until a token expires in the authorization cache since it is last used."),

                           CustomData:        null

                       ),

                       #endregion

                       #region Policy

                       new VariableConfig(

                           Name:              "Policy",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.OptionList
                                                   )
                                               },

                           Description:       I18NString.Create("Cache Entry Replacement Policy: (LRU,LFU) LeastRecentlyUsed or LeastFrequentlyUsed. Allowed values: LRU, LFU."),

                           CustomData:        null

                       ),

                       #endregion

                       #region DisablePostAuthorize

                       new VariableConfig(

                           Name:              "DisablePostAuthorize",
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

                           Description:       I18NString.Create("When set to true this variable disables the behavior to request authorization for an idToken that is stored in the cache with a status other than Accepted, as stated in C10.FR.03 and C12.FR.05."),

                           CustomData:        null

                       ),

                       #endregion

                   },
                   I18NString.Create("Logical Component responsible for configuration relating to the use of a local cache for authorization for Charging Station use."),
                   CustomData)

        {

            this.Enabled               = Enabled;
            this.Available             = Available;
            this.LifeTime              = LifeTime;
            this.Policy                = Policy;
            this.DisablePostAuthorize  = DisablePostAuthorize;

        }

        #endregion


    }

}
