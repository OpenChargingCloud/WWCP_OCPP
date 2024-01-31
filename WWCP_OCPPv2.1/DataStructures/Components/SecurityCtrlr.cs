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
    public class SecurityCtrlr : ComponentConfig
    {

        #region Properties

        /// <summary>
        /// The charging station identity.
        /// </summary>
        public String  Identity             { get; set; }

        /// <summary>
        /// The HTTP Basic Authentication password.
        /// </summary>
        public String  BasicAuthPassword    { get; set; }

        /// <summary>
        /// The organization name that is to be used for checking a security certificate.
        /// </summary>
        public String  OrganizationName     { get; set; }

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
        public SecurityCtrlr(String       Identity,
                             String       BasicAuthPassword,
                             String       OrganizationName,

                             String?      Instance     = null,
                             CustomData?  CustomData   = null)

            : base("SecurityCtrlr",
                   Instance,
                   null,
                   new[] {

                       #region Identity

                       new VariableConfig(

                           Name:              "Identity",
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

                           Description:       I18NString.Create("The charging station identity (identifierString)."),

                           CustomData:        null

                       ),

                       #endregion

                       #region BasicAuthPassword

                       new VariableConfig(

                           Name:              "BasicAuthPassword",
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

                           Description:       I18NString.Create("The HTTP Basic Authentication password (16-40 alpha-numeric and special characters allowed bypasswordString). This configuration variable is write-only, sothat it cannot be accidentally stored in plaintext by the CSMS when it reads out allconfiguration variables. This configuration variable is required unless only \"security profile 3 - TLS withclient side certificates\" is implemented."),

                           CustomData:        null

                       ),

                       #endregion

                       #region OrganizationName

                       new VariableConfig(

                           Name:              "OrganizationName",
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

                           Description:       I18NString.Create("Organization name that is to be used for checking a security certificate."),

                           CustomData:        null

                       )

                       #endregion

                   },
                   I18NString.Create("HTTP Basic Authentication security settings between the charging station and the CSMS."),
                   CustomData)

        {

            this.BasicAuthPassword  = BasicAuthPassword;
            this.Identity           = Identity;
            this.OrganizationName   = OrganizationName;

        }

        #endregion


    }

}
