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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Logical Component responsible for configuration relating to the use of
    /// a local cache for authorization for battery swap station use.
    /// </summary>
    public class AuthCacheCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// If this variable exists and reports a value of true, authorization cache is supported, but not necessarily enabled.
        /// </summary>
        public Boolean?                      Available               { get; set; }

        /// <summary>
        /// If this variable exists and reports a value of true, authorization cache is enabled.
        /// </summary>
        public Boolean?                      Enabled                 { get; set; }

        /// <summary>
        /// Indicates in seconds how long it takes until a token expires in the authorization cache since it is last used.
        /// </summary>
        public TimeSpan?                     LifeTime                { get; set; }

        /// <summary>
        /// Indicates the number of bytes currently used by the authorization cache.
        /// MaxLimit indicates the maximum number of bytes that can be used by the authorization cache.
        /// </summary>
        public UInt32?                       Storage                 { get; set; }

        /// <summary>
        /// Cache Entry Replacement Policy: least recently used, least frequently used, first in first out, other custom mechanism.
        /// </summary>
        public CacheEntryReplacementPolicy?  Policy                  { get; set; }

        /// <summary>
        /// When set to true this variable disables the behavior to request authorization for an idToken that is stored in the cache with a status other than accepted, as stated in C10.FR.03 and C12.FR.05.
        /// </summary>
        public Boolean?                      DisablePostAuthorize    { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new authorization cache controller.
        /// </summary>
        /// <param name="Available">If this variable exists and reports a value of true, authorization cache is supported, but not necessarily enabled.</param>
        /// <param name="Enabled">If this variable exists and reports a value of true, authorization cache is enabled.</param>
        /// <param name="LifeTime">Indicates in seconds how long it takes until a token expires in the authorization cache since it is last used.</param>
        /// <param name="Storage">Indicates the number of bytes currently used by the authorization cache. MaxLimit indicates the maximum number of bytes that can be used by the authorization cache.</param>
        /// <param name="Policy">Cache Entry Replacement Policy: least recently used, least frequently used, first in first out, other custom mechanism.</param>
        /// <param name="DisablePostAuthorize">When set to true this variable disables the behavior to request authorization for an idToken that is stored in the cache with a status other than accepted, as stated in C10.FR.03 and C12.FR.05.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public AuthCacheCtrlr(Boolean?                      Available              = null,
                              Boolean?                      Enabled                = null,
                              TimeSpan?                     LifeTime               = null,
                              UInt32?                       Storage                = null,
                              CacheEntryReplacementPolicy?  Policy                 = null,
                              Boolean?                      DisablePostAuthorize   = null,

                              String?                       Instance               = null,
                              CustomData?                   CustomData             = null)

            : base(nameof(AuthCacheCtrlr),
                   Instance,
                   I18NString.Create("Logical Component responsible for configuration relating to the use of a local cache for authorization for battery swap station use."),
                   CustomData)

        {

            this.Enabled               = Enabled;
            this.Available             = Available;
            this.LifeTime              = LifeTime;
            this.Storage               = Storage;
            this.Policy                = Policy;
            this.DisablePostAuthorize  = DisablePostAuthorize;


            #region Available

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Available",
                    ValueGetter:      () => this.Available.HasValue
                                                ? this.Available.Value
                                                      ? "true"
                                                      : "false"
                                                : null,
                    Instance:         null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("If this variable exists and reports a value of true, authorization cache is supported, but not necessarily enabled.")

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
                    Instance:         null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("If this variable exists and reports a value of true, authorization cache is enabled.")

                )
            );

            #endregion

            #region LifeTime

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "LifeTime",
                    ValueGetter:      () => this.LifeTime.HasValue
                                                ? ((UInt32) Math.Round(this.LifeTime.Value.TotalSeconds)).ToString()
                                                : null,
                    Instance:         null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          Unit:        UnitsOfMeasure.TimeSpan(),
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Indicates in seconds how long it takes until a token expires in the authorization cache since it is last used.")

                )
            );

            #endregion

            #region Storage

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Storage",
                    ValueGetter:      () => this.Storage?.ToString(),
                    Instance:         null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Indicates the number of bytes currently used by the authorization cache. MaxLimit indicates the maximum number of bytes that can be used by the authorization cache.")

                )
            );

            #endregion

            #region Policy

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Policy",
                    ValueGetter:      () => this.Policy?.ToString(),
                    Instance:         null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.OptionList
                                      ),

                    Description:      I18NString.Create("Cache Entry Replacement Policy: least recently used, least frequently used, first in first out, other custom mechanism.")

                )
            );

            #endregion

            #region DisablePostAuthorize   (Does this still exist in OCPP v2.1?)

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "DisablePostAuthorize",
                    ValueGetter:      () => this.DisablePostAuthorize.HasValue
                                                ? this.DisablePostAuthorize.Value
                                                      ? "true"
                                                      : "false"
                                                : null,
                    Instance:         null,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("When set to true this variable disables the behavior to request authorization for an idToken that is stored in the cache with a status other than Accepted, as stated in C10.FR.03 and C12.FR.05.")

                )
            );

            #endregion


        }

        #endregion


    }

}
